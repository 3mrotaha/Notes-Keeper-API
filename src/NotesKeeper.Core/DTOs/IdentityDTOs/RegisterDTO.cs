using Microsoft.AspNetCore.Mvc;
using NotesKeeper.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace NotesKeeper.Core.DTOs.IdentityDTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MinLength(2, ErrorMessage = "Full name must be at least 2 characters long.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [DataType(DataType.Text, ErrorMessage = "Full name must be a valid text.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is Required.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Phone number must be valid")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Remote(action: "IsEmailAlreadyUsed", controller: "Account", ErrorMessage = "Email is already in use.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password Can't be less than 8 characters")]
        [DataType(DataType.Password, ErrorMessage = "Password must be a valid password.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(ApplicationUserRole), ErrorMessage = "Invalid role specified.")]
        public ApplicationUserRole Role { get; set; } = ApplicationUserRole.User;
    }
}
