using NotesKeeper.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.DTOs.IdentityDTOs
{
    public class AuthenticationResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
