using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.DTOs.ReminderDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Mappings
{
    public static class ReminderMappingExtensions
    {
        public static ReminderResponse ToReminderResponse(this Reminder reminder)
        {
            return new ReminderResponse
            {
                Id = reminder.Id,
                DateTime = reminder.DateTime,
                FallbackTime = reminder.FallbackTime,
                Message = reminder.Message,
                NoteId = reminder.NoteId                
            };
        }
    }
}
