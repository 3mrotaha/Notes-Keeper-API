using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.DTOs.ReminderDTOs;
using NotesKeeper.Core.DTOs.TagDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NotesKeeper.Core.DTOs.NoteDTOs
{
    public class NoteUpdateRequest
    {
        [Required(ErrorMessage = "{0} Can't be null")]
        public Guid UserId { get; set; }
        
        [Required(ErrorMessage = "{0} Can't be null")]
        [MaxLength(500, ErrorMessage = "{0} Exceeds maximum length {1} characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "{0} Can't be null")]
        public string? NoteBody { get; set; }
        public int ReminderId {get; set;}
        public List<TagResponse>? Tags { get; set; }
        public Note ToNote()
        {
            return new Note
            {
                UserId = this.UserId,
                Title = this.Title,
                NoteBody = this.NoteBody,
            };
        }
    }
}
