using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.Domain.Interfaces;
using NotesKeeper.Core.DTOs.TagDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.Entities
{
    public class Tag : IAtDateCreatable, ISoftDeletable
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid UserId { get; set; }
        public string? Comment { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<TagsAssignments> TagsAssignments { get; set; } = new HashSet<TagsAssignments>();
        public virtual ICollection<Note> Notes { get; set; } = new HashSet<Note>();
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
