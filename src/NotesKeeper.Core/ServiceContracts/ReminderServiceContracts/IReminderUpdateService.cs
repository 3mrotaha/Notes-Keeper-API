using NotesKeeper.Core.DTOs.ReminderDTOs;

namespace NotesKeeper.Core.ServiceContracts.ReminderServiceContracts
{
    /// <summary>
    /// Defines a contract for updating reminder information.
    /// </summary>
    public interface IReminderUpdateService
    {
        /// <summary>
        /// Updates the reminder identified by the specified identifier with the provided details.
        /// </summary>
        /// <param name="reminderId">The unique identifier of the reminder to update.</param>
        /// <param name="request">The reminder update request containing the new reminder details. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="ReminderResponse"/> representing the updated reminder, or <see langword="null"/> if the reminder was not found.</returns>
        Task<ReminderResponse?> UpdateReminder(int reminderId, ReminderUpdateRequest request);
    }
}
