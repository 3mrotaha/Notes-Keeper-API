using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.DTOs.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Mappings
{
    public static class RegisterMappingExtensions
    {
        public static ApplicationUser ToUser(this RegisterDTO registerDto)
        {
            return new ApplicationUser
            {
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                Email = registerDto.Email,
                UserName = registerDto.UserName
            };
        }
        
    }
}
