using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts
{
    /// <summary>
    /// Defines methods for retrieving reminder information using various criteria.
    /// </summary>
    public interface IReminderGetRepository
    {
        /// <summary>
        /// Retrieves the reminder associated with the specified reminder identifier.
        /// </summary>
        /// <param name="reminderId">The unique identifier of the reminder to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="Reminder"/> object if found; otherwise, <see langword="null"/>.</returns>
        public Task<Reminder?> GetReminder(int reminderId);

        /// <summary>
        /// Retrieves the first reminder that satisfies the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions used to filter reminders.</param>
        /// <returns>A reminder that matches the provided predicate; or <see langword="null"/> if no match is found.</returns>
        public Task<Reminder?> GetReminder(Expression<Predicate<Reminder>> predicate);

        /// <summary>
        /// Retrieves the first reminder that satisfies the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions used to filter reminders.</param>
        /// <returns>A reminder that matches the provided predicate; or <see langword="null"/> if no match is found.</returns>
        public Task<IEnumerable<Reminder>?> GetReminders(Expression<Predicate<Reminder>> predicate);
    }
}
