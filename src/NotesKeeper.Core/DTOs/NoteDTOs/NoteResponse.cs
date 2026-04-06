using System;
using System.Collections.Generic;
using System.Text;
using NotesKeeper.Core.DTOs.ReminderDTOs;
using NotesKeeper.Core.DTOs.TagDTOs;

namespace NotesKeeper.Core.DTOs.NoteDTOs
{
    public class NoteResponse
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public List<TagResponse?>? Tags { get; set; }
        public ReminderResponse? Reminder { get; set; }
        public string? Title { get; set; }
        public string? NoteBody { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
