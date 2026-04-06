using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NotesKeeper.Core.DTOs.IdentityDTOs
{
    public class TokenModel
    {
        [Required(ErrorMessage = "{0} is required")]
        public string? Token { get; set; }
        
        [Required(ErrorMessage = "{0} is required")]
        public string? RefreshToken { get; set; }
    }
}
