using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NotesKeeper.Core.DTOs.NoteDTOs
{
    public class NoteAddRequest
    {
        [Required(ErrorMessage = "Can't create a note for unknow user {0}")]        
        public Guid UserId { get; set; }
        
        [Required(ErrorMessage = "{0} Can't be null")]
        [MaxLength(500, ErrorMessage = "{0} Exceeds maximum length {1} characters")]
        public string? Title { get; set; } = string.Empty;        
        
        [Required(ErrorMessage = "{0} Can't be null")]
        public string? NoteBody { get; set; } = string.Empty;

        public Note ToNote()
        {
            return new Note
            {
                UserId = this.UserId,
                Title = this.Title,
                NoteBody = this.NoteBody
            };
        }
    }
}
