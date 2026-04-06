using System.Linq.Expressions;
using NotesKeeper.Core.DTOs.ReminderDTOs;

namespace NotesKeeper.Core.ServiceContracts.ReminderServiceContracts
{
    /// <summary>
    /// Defines methods for retrieving reminder information.
    /// </summary>
    public interface IReminderGetService
    {
        /// <summary>
        /// Retrieves the reminder associated with the specified reminder identifier.
        /// </summary>
        /// <param name="reminderId">The unique identifier of the reminder to retrieve.</param>
        /// <returns>A <see cref="ReminderResponse"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<ReminderResponse?> GetReminder(int reminderId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        Task<List<ReminderResponse>?> GetDueReminders(int minutes);
    }
}
