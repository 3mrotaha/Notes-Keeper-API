using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts
{
    /// <summary>
    /// Provides an interface for adding new reminders to a data store.
    /// </summary>
    public interface IReminderAddRepository
    {
        /// <summary>
        /// Adds a new reminder to the data store and returns the unique identifier of the created reminder.
        /// </summary>
        /// <param name="reminder">A <see cref="Reminder"/> object containing the details of the reminder to add. Must not be <see langword="null"/>.</param>
        /// <returns>An integer representing the unique identifier of the newly created reminder. Returns -1 if the reminder could not be added.</returns>
        public Task<int> AddReminder(Reminder reminder);
    }
}
