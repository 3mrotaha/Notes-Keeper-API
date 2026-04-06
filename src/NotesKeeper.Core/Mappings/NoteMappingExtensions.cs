using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.DTOs.NoteDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Mappings
{
    public static class NoteMappingExtensions
    {
        public static NoteResponse ToNoteResponse(this Note note)
        {
            var noteResponse = new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                NoteBody = note.NoteBody,
                CreatedAt = note.CreatedAt,
                UserId = note.UserId,
                Tags = note.TagsAssignments != null ? note.TagsAssignments.Select(t => t.Tag?.ToTagResponse()).ToList() : null,
                Reminder = note.Reminder != null ? note.Reminder.ToReminderResponse() : null
            };

            return noteResponse;
        }
    }
}
