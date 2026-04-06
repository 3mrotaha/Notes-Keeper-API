using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.DTOs.IdentityDTOs;
using NotesKeeper.Core.Enums;
using NotesKeeper.Core.ServiceContracts.JwtServiceContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NotesKeeper.Core.Services.JwtService
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        private readonly int JwtExpDateInt;
        private readonly int jwtRefreshExpDateInt;
        private readonly string jwtKey;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            string? jwtKeyString = _configuration["Jwt:Key"];
            string? jwtExpDate = _configuration["Jwt:Exp"];
            string? jwtRefreshExpDate = _configuration["Jwt:RefreshTokenExp"];

            if (string.IsNullOrEmpty(jwtExpDate)
                    || string.IsNullOrEmpty(jwtRefreshExpDate)
                    || string.IsNullOrEmpty(jwtKeyString))
            {
                throw new ArgumentNullException("Authorization configuration are null or empty");
            }
            else if (!int.TryParse(jwtExpDate, out JwtExpDateInt) || !int.TryParse(jwtRefreshExpDate, out jwtRefreshExpDateInt))
            {
                throw new ArgumentException("Couldn't convert JWT Expiration dates to integer");
            }

            jwtKey = jwtKeyString;
        }

        public AuthenticationResponse GenerateJwtToken(ApplicationUser user, string userRole)
        {
            _logger.LogDebug("GenerateJwtToken called for UserId {UserId}, Role {Role}", user.Id, userRole);

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(userRole))
            {
                throw new ArgumentNullException("User Information can't be null");
            }

            AuthenticationResponse response = new AuthenticationResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                Role = userRole
            };

            // 1. expiration dates
            response.Expiration = DateTime.UtcNow.AddMinutes(JwtExpDateInt);
            response.RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(jwtRefreshExpDateInt);

            // 2. claims
            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, userRole.ToString())
            };

            // 3. secret key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            // 4. signing credentials
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 5. create token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: response.Expiration,
                signingCredentials: signingCredentials
            );

            // 6. generate token string
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            response.Token = tokenHandler.WriteToken(token);

            // 7. generate refresh token
            response.RefreshToken = GenerateRefreshToken();

            _logger.LogInformation("JWT token generated for UserId {UserId} with Role {Role}, expires {Expiration}", user.Id, userRole, response.Expiration);
            return response;
        }

        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            _logger.LogDebug("GetPrincipalFromExpiredToken called");

            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateLifetime = false // we are validating an expired token, so we don't care about the token's lifetime
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("GetPrincipalFromExpiredToken: token algorithm mismatch — token rejected");
                throw new SecurityTokenException("Invalid token");
            }

            _logger.LogDebug("GetPrincipalFromExpiredToken: token validated successfully");
            return principal;
        }
    }
}