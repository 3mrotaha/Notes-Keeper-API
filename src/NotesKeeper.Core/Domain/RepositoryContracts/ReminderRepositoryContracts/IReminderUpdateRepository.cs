using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts
{
    /// <summary>
    /// Defines a contract for updating reminder information in a data store.
    /// </summary>
    public interface IReminderUpdateRepository
    {
        /// <summary>
        /// Updates the specified reminder with new information and returns the updated reminder.
        /// </summary>
        /// <param name="reminder">The reminder object containing the updated information. Must not be <see langword="null"/>.</param>
        /// <returns>The updated reminder object if the update is successful; otherwise, <see langword="null"/>.</returns>
        public Task<Reminder?> UpdateReminder(Reminder reminder);
    }
}
