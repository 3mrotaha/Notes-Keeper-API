using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts;
using NotesKeeper.Core.DTOs.ReminderDTOs;
using NotesKeeper.Core.Mappings;
using NotesKeeper.Core.ServiceContracts.ReminderServiceContracts;

namespace NotesKeeper.Core.Services
{
    public class ReminderService : IReminderAddService, IReminderGetService, IReminderUpdateService, IReminderDeleteService
    {
        private readonly IReminderAddRepository _reminderAddRepository;
        private readonly IReminderGetRepository _reminderGetRepository;
        private readonly IReminderUpdateRepository _reminderUpdateRepository;
        private readonly IReminderDeleteRepository _reminderDeleteRepository;
        private readonly ILogger<ReminderService> _logger;

        public ReminderService(
            IReminderAddRepository reminderAddRepository,
            IReminderGetRepository reminderGetRepository,
            IReminderUpdateRepository reminderUpdateRepository,
            IReminderDeleteRepository reminderDeleteRepository,
            ILogger<ReminderService> logger)
        {
            _reminderAddRepository = reminderAddRepository;
            _reminderGetRepository = reminderGetRepository;
            _reminderUpdateRepository = reminderUpdateRepository;
            _reminderDeleteRepository = reminderDeleteRepository;
            _logger = logger;
        }

        public async Task<ReminderResponse?> AddReminder(ReminderAddRequest request)
        {
            _logger.LogDebug("AddReminder called");
            Reminder reminder = request.ToReminder();
            int id = await _reminderAddRepository.AddReminder(reminder);
            if (id == -1)
            {
                _logger.LogWarning("AddReminder failed — repository returned -1");
                return null;
            }

            reminder.Id = id;
            _logger.LogInformation("Reminder {ReminderId} created successfully", id);
            return reminder.ToReminderResponse();
        }

        public async Task<ReminderResponse?> GetReminder(int reminderId)
        {
            _logger.LogDebug("GetReminder called for ReminderId {ReminderId}", reminderId);
            Reminder? reminder = await _reminderGetRepository.GetReminder(reminderId);
            if (reminder is null)
            {
                _logger.LogWarning("GetReminder: Reminder {ReminderId} not found", reminderId);
                return null;
            }

            _logger.LogInformation("Reminder {ReminderId} retrieved successfully", reminderId);
            return reminder.ToReminderResponse();
        }

        public async Task<ReminderResponse?> UpdateReminder(int reminderId, ReminderUpdateRequest request)
        {
            _logger.LogDebug("UpdateReminder called for ReminderId {ReminderId}", reminderId);
            Reminder reminder = request.ToReminder();
            reminder.Id = reminderId;
            Reminder? updated = await _reminderUpdateRepository.UpdateReminder(reminder);
            if (updated is null)
            {
                _logger.LogWarning("UpdateReminder: Reminder {ReminderId} not found or update failed", reminderId);
                return null;
            }

            _logger.LogInformation("Reminder {ReminderId} updated successfully", reminderId);
            return updated.ToReminderResponse();
        }

        public async Task<bool> DeleteReminder(int reminderId)
        {
            _logger.LogDebug("DeleteReminder called for ReminderId {ReminderId}", reminderId);
            bool deleted = await _reminderDeleteRepository.DeleteReminder(reminderId);
            if (!deleted)
                _logger.LogWarning("DeleteReminder: Reminder {ReminderId} not found or delete failed", reminderId);
            else
                _logger.LogInformation("Reminder {ReminderId} deleted successfully", reminderId);

            return deleted;
        }

        public async Task<List<ReminderResponse>?> GetDueReminders(int minutes)
        {            
            var now = DateTime.UtcNow;
            var reminders = await _reminderGetRepository.GetReminders(r => r.DateTime <= now && r.DateTime > now.AddMinutes(minutes));
            if (reminders is null)
            {
                _logger.LogInformation("GetReminders: No Reminders");
                return null;
            }

            _logger.LogInformation("Reminders retrieved successfully");
            return reminders.Select(r => r.ToReminderResponse()).ToList();
        }
    }
}
