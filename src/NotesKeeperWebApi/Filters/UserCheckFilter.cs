using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using NotesKeeper.Core.ServiceContracts.JwtServiceContracts;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// This filter is used to check if the user id in the request body matches the user id in the JWT token, this is to prevent users from adding tags for other users by changing the user id in the request body, this filter will be applied to the AddTag action in the TagController, and it will read the user id from the request body and compare it with the user id in the JWT token, if they don't match, it will return an unauthorized response
/// </summary>
public class UserCheckFilter : IAsyncAuthorizationFilter
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserCheckFilter> _logger;

    public UserCheckFilter(IJwtService jwtService, ILogger<UserCheckFilter> logger)
    {
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        string? remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        string userId = context.HttpContext.Request.Headers["UserId"].FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("UserCheckFilter: Missing UserId header from {RemoteIp}", remoteIp);
            context.Result = new BadRequestObjectResult("User id is required.");
            return;
        }

        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("UserCheckFilter: Missing Authorization header for UserId {UserId} from {RemoteIp}", userId, remoteIp);
            context.Result = new UnauthorizedResult();
            return;
        }

        System.Security.Claims.ClaimsPrincipal principal;
        try
        {
            principal = _jwtService.GetPrincipalFromExpiredToken(token);
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogError(ex, "UserCheckFilter: Invalid or tampered JWT received from {RemoteIp}", remoteIp);
            context.Result = new UnauthorizedResult();
            return;
        }

        string? tokenUserId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (tokenUserId != userId)
        {
            _logger.LogWarning("UserCheckFilter: UserId claim '{TokenUserId}' does not match header UserId '{HeaderUserId}' from {RemoteIp}", tokenUserId, userId, remoteIp);
            context.Result = new UnauthorizedResult();
            return;
        }

        _logger.LogDebug("UserCheckFilter: UserId {UserId} authorized from {RemoteIp}", userId, remoteIp);
    }
}
