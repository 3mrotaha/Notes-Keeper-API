using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NotesKeeper.Core.DTOs.IdentityDTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email should be a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password Can't be less than 8 characters")]
        [DataType(DataType.Password, ErrorMessage = "Password must be a valid password.")]
        public string Password { get; set; }
    }
}
