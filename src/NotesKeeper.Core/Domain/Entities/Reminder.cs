using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public int? NoteId { get; set; }
        public string? Message { get; set; }
        public DateTime DateTime { get; set; }
        public int FallbackTime { get; set; } = 0;
    }
}
