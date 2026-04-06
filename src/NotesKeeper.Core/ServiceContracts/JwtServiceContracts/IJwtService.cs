using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.DTOs.IdentityDTOs;
using NotesKeeper.Core.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace NotesKeeper.Core.ServiceContracts.JwtServiceContracts
{
    public interface IJwtService
    {
        AuthenticationResponse GenerateJwtToken(ApplicationUser user, string userRole);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
