using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts
{
    /// <summary>
    /// Defines the contract for deleting reminders from the repository.
    /// </summary>
    public interface IReminderDeleteRepository
    {
        /// <summary>
        /// Deletes the reminder associated with the specified reminder identifier.
        /// </summary>
        /// <param name="reminderId">The unique identifier of the reminder to delete. Must correspond to an existing reminder.</param>
        /// <returns><see langword="true"/> if the reminder was successfully deleted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> DeleteReminder(int reminderId);

        /// <summary>
        /// Deletes the first reminder that matches the specified criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions a reminder must satisfy to be deleted.</param>
        /// <returns><see langword="true"/> if a reminder was successfully deleted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> DeleteReminder(Expression<Predicate<Reminder>> predicate);
    }
}
