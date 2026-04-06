namespace NotesKeeper.Core.ServiceContracts.ReminderServiceContracts
{
    /// <summary>
    /// Defines a contract for deleting reminders.
    /// </summary>
    public interface IReminderDeleteService
    {
        /// <summary>
        /// Deletes the reminder associated with the specified reminder identifier.
        /// </summary>
        /// <param name="reminderId">The unique identifier of the reminder to delete.</param>
        /// <returns><see langword="true"/> if the reminder was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteReminder(int reminderId);
    }
}
