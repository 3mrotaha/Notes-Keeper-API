using NotesKeeper.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.Entities
{
    public class TagsAssignments: IAtDateCreatable
    {
        public int TagId { get; set; }
        public int NoteId { get; set; }

        public virtual Note? Note { get; set; }
        public virtual Tag? Tag { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
