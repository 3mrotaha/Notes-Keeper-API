using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NotesKeeper.Core.DTOs.ReminderDTOs
{
    public class ReminderUpdateRequest
    {
        [Required(ErrorMessage = "{0} Can't be null or empty")]
        public int NoteId { get; set; }

        [MaxLength(500, ErrorMessage = "Tag {0} Can't exceed the max length of {1} characters")]
        public string? Message { get; set; }

        [Required(ErrorMessage = "{0} Can't be null or empty")]
        public DateTime DateTime { get; set; }
        
        public int FallbackTime { get; set; } = 0;

        public Reminder ToReminder()
        {
            return new Reminder
            {
                NoteId = this.NoteId,
                Message = this.Message,
                DateTime = this.DateTime,
                FallbackTime = this.FallbackTime
            };
        }
    }
}
