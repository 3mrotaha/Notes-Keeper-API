using Microsoft.AspNetCore.Identity;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public string? RefreshToken { get; set; } = null!;
        public DateTime? RefreshTokenExpiration { get; set; }
        public virtual ICollection<Note> Notes { get; set; } = new HashSet<Note>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    }
}
