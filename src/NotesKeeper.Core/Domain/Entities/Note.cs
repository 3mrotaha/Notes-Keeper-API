using NotesKeeper.Core.Domain.IdentityEntities;
using NotesKeeper.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.Entities
{
    public class Note : IAtDateCreatable
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? NoteBody { get; set; }
        public int? ReminderId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<TagsAssignments> TagsAssignments { get; set; } = new HashSet<TagsAssignments>();
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual Reminder? Reminder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
