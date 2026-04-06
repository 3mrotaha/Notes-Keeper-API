using NotesKeeper.Core.DTOs.ReminderDTOs;

namespace NotesKeeper.Core.ServiceContracts.ReminderServiceContracts
{
    /// <summary>
    /// Defines a contract for adding new reminders.
    /// </summary>
    public interface IReminderAddService
    {
        /// <summary>
        /// Adds a new reminder based on the provided request and returns the created reminder details.
        /// </summary>
        /// <param name="request">The reminder creation request containing the reminder details. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="ReminderResponse"/> representing the newly created reminder, or <see langword="null"/> if the reminder could not be created.</returns>
        Task<ReminderResponse?> AddReminder(ReminderAddRequest request);
    }
}
