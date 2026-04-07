using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.DTOs.IdentityDTOs;
using NotesKeeper.Core.Enums;
using NotesKeeper.Core.Mappings;
using NotesKeeper.Core.ServiceContracts.JwtServiceContracts;
using System.Security.Claims;

namespace NotesKeeper.UI.Controllers
{
    [AllowAnonymous]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _roleManager = roleManager;
            _logger = logger;
        }

        private IEnumerable<string> _catchErrors()
        {
            return ModelState.Values.SelectMany(error => error.Errors).Select(error => error.ErrorMessage);
        }

        [HttpGet("email-token-gen/{userId:guid}")]
        public async Task<IActionResult> GenerateEmailConfirmationLink(Guid userId)
        {
            _logger.LogDebug("GenerateEmailConfirmationLink called for UserId {UserId}", userId);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("GenerateEmailConfirmationLink: UserId {UserId} not found", userId);
                return NotFound();
            }

            string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string? confirmationLink = Url.Action("ConfirmUserEmail", "Account", new { userId = user.Id, emailConfirmationToken }, Request.Scheme);

            if (confirmationLink == null)
            {
                _logger.LogError("GenerateEmailConfirmationLink: failed to build confirmation URL for UserId {UserId}", userId);
                return BadRequest("Unable to generate email confirmation link.");
            }

            _logger.LogInformation("Email confirmation link generated for UserId {UserId}", userId);
            return Ok(new { ConfirmationLink = confirmationLink });
        }

        [HttpGet("confirm-email/{userId:guid}")]
        public async Task<IActionResult> ConfirmUserEmail(Guid userId, [FromQuery] string emailConfirmationToken)
        {
            _logger.LogDebug("ConfirmUserEmail called for UserId {UserId}", userId);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("ConfirmUserEmail: UserId {UserId} not found", userId);
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                var roles = await _userManager.GetRolesAsync(user);
                AuthenticationResponse authResponse = _jwtService.GenerateJwtToken(user, roles.FirstOrDefault() ?? ApplicationUserRole.User.ToString());
                user.RefreshToken = authResponse.RefreshToken;
                user.RefreshTokenExpiration = authResponse.RefreshTokenExpiration;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Email confirmed and user signed in for UserId {UserId}", userId);
                return Ok(authResponse);
            }
            else
            {
                _logger.LogWarning("ConfirmUserEmail: email confirmation failed for UserId {UserId}. Errors: {Errors}",
                    userId, string.Join(", ", result.Errors.Select(e => e.Description)));
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("register")]
        [EnableRateLimiting("sliding")] // prevent spam accounts
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            _logger.LogDebug("Register called for Email {Email}, Role {Role}", registerDto.Email, registerDto.Role);
            if (!ModelState.IsValid)
            {
                string Errors = string.Join(Environment.NewLine, _catchErrors());
                _logger.LogWarning("Register: model validation failed for {Email}. Errors: {Errors}", registerDto.Email, Errors);
                return Problem(detail: Errors, statusCode: StatusCodes.Status400BadRequest);
            }

            var user = registerDto.ToUser();
            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(registerDto.Role.ToString()))
                    await _roleManager.CreateAsync(new ApplicationRole { Name = registerDto.Role.ToString() });

                if (!await _userManager.IsInRoleAsync(user, registerDto.Role.ToString()))
                    await _userManager.AddToRoleAsync(user, registerDto.Role.ToString());

                _logger.LogInformation("User {Email} registered successfully with role {Role}", registerDto.Email, registerDto.Role);
                return RedirectToAction("GenerateEmailConfirmationLink", "Account", new { UserId = user.Id });
            }
            else
            {
                string Errors = string.Join(Environment.NewLine, result.Errors.Select(error => error.Description));
                _logger.LogWarning("Register: user creation failed for {Email}. Errors: {Errors}", registerDto.Email, Errors);
                return Problem(detail: Errors, statusCode: StatusCodes.Status400BadRequest);
            }
        }

        [HttpPost("login")]
        [EnableRateLimiting("sliding")] // to protect against brute force attacks
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            _logger.LogDebug("Login attempt for Email {Email}", loginDto.Email);
            if (!ModelState.IsValid)
            {
                string Errors = string.Join(Environment.NewLine, _catchErrors());
                _logger.LogWarning("Login: model validation failed for {Email}", loginDto.Email);
                return Problem(detail: Errors);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login: user not found for Email {Email}", loginDto.Email);
                return Problem(detail: "Invalid email or password.", statusCode: StatusCodes.Status401Unauthorized);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            var Roles = await _userManager.GetRolesAsync(user);
            if (result.Succeeded)
            {
                AuthenticationResponse authResponse = _jwtService.GenerateJwtToken(user, Roles.FirstOrDefault() ?? ApplicationUserRole.User.ToString());
                user.RefreshToken = authResponse.RefreshToken;
                user.RefreshTokenExpiration = authResponse.RefreshTokenExpiration;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("User {Email} (UserId {UserId}) logged in successfully", loginDto.Email, user.Id);
                return Ok(authResponse);
            }
            else
            {
                _logger.LogWarning("Login: failed sign-in for Email {Email}. Locked: {IsLockedOut}", loginDto.Email, result.IsLockedOut);
                return Problem(detail: "Invalid email or password.", statusCode: StatusCodes.Status401Unauthorized);
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!ModelState.IsValid)
            {
                string Errors = string.Join(Environment.NewLine, _catchErrors());
                return Problem(detail: Errors);
            }

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User {UserId} logged out", userId);
            return NoContent();
        }

        [HttpGet("IsEmailAlreadyUsed")]
        public async Task<IActionResult> IsEmailAlreadyUsed(string email)
        {
            _logger.LogDebug("IsEmailAlreadyUsed check for {Email}", email);
            var user = await _userManager.FindByEmailAsync(email);
            return Ok(new { IsEmailAlreadyUsed = user != null });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            _logger.LogDebug("RefreshToken called");
            if (tokenModel == null || tokenModel.Token == null)
            {
                _logger.LogWarning("RefreshToken: null or missing token in request body");
                return BadRequest("Invalid client request");
            }

            ClaimsPrincipal? claimsPrincipal;
            try
            {
                claimsPrincipal = _jwtService.GetPrincipalFromExpiredToken(tokenModel.Token);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "RefreshToken: invalid or tampered JWT presented");
                return BadRequest("Invalid jwt token");
            }

            if (claimsPrincipal == null)
            {
                _logger.LogWarning("RefreshToken: principal could not be extracted from token");
                return BadRequest("Invalid jwt token");
            }

            string? email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                _logger.LogWarning("RefreshToken: invalid refresh token for Email {Email} — user missing, token mismatch, or expired", email);
                return BadRequest("Invalid refresh token");
            }

            string Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? ApplicationUserRole.User.ToString();
            AuthenticationResponse authResponse = _jwtService.GenerateJwtToken(user, Role);

            user.RefreshToken = authResponse.RefreshToken;
            user.RefreshTokenExpiration = authResponse.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("Token refreshed successfully for UserId {UserId}", user.Id);
            return Ok(authResponse);
        }
    }
}
