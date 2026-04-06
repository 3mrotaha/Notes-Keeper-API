using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts;
using NotesKeeper.Infrastructure.ApplicationDbContext;
using System.Linq.Expressions;

namespace NotesKeeper.Infrastructure.Repositories
{
    public class ReminderRepository : IReminderAddRepository, IReminderGetRepository, IReminderUpdateRepository, IReminderDeleteRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ReminderRepository> _logger;

        public ReminderRepository(AppDbContext dbContext, ILogger<ReminderRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> AddReminder(Reminder reminder)
        {
            _logger.LogDebug("AddReminder DB operation started");
            _dbContext.Reminders.Add(reminder);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Reminder {ReminderId} persisted to DB", reminder.Id);
                return reminder.Id;
            }

            _logger.LogWarning("AddReminder SaveChanges returned 0 rows");
            return -1;
        }

        public async Task<Reminder?> GetReminder(int reminderId)
        {
            _logger.LogDebug("GetReminder DB query for ReminderId {ReminderId}", reminderId);
            var reminder = await _dbContext.Reminders.AsNoTracking()
                                                .FirstOrDefaultAsync(r => r.Id == reminderId);
            if (reminder is null)
                _logger.LogWarning("GetReminder: ReminderId {ReminderId} not found in DB", reminderId);

            return reminder;
        }

        public async Task<Reminder?> GetReminder(Expression<Predicate<Reminder>> predicate)
        {
            _logger.LogDebug("GetReminder (predicate) DB query initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            return await _dbContext.Reminders.AsNoTracking()
                                            .FirstOrDefaultAsync(funcExpression);
        }

        public async Task<Reminder?> UpdateReminder(Reminder reminder)
        {
            _logger.LogDebug("UpdateReminder DB operation for ReminderId {ReminderId}", reminder.Id);
            Reminder? existing = await _dbContext.Reminders.FirstOrDefaultAsync(r => r.Id == reminder.Id);
            if (existing == null)
            {
                _logger.LogWarning("UpdateReminder: ReminderId {ReminderId} not found in DB", reminder.Id);
                return null;
            }

            _dbContext.Entry(existing).CurrentValues.SetValues(reminder);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Reminder {ReminderId} updated in DB", reminder.Id);
            return existing;
        }

        public async Task<bool> DeleteReminder(int reminderId)
        {
            _logger.LogDebug("DeleteReminder DB operation for ReminderId {ReminderId}", reminderId);
            Reminder? reminder = await _dbContext.Reminders.FirstOrDefaultAsync(r => r.Id == reminderId);
            if (reminder == null)
            {
                _logger.LogWarning("DeleteReminder: ReminderId {ReminderId} not found in DB", reminderId);
                return false;
            }

            _dbContext.Reminders.Remove(reminder);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
                _logger.LogInformation("Reminder {ReminderId} deleted from DB", reminderId);
            else
                _logger.LogWarning("DeleteReminder: SaveChanges returned 0 rows for ReminderId {ReminderId}", reminderId);

            return result > 0;
        }

        public async Task<bool> DeleteReminder(Expression<Predicate<Reminder>> predicate)
        {
            _logger.LogDebug("DeleteReminder (predicate) DB operation initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            Reminder? reminder = await _dbContext.Reminders.FirstOrDefaultAsync(funcExpression);
            if (reminder == null)
            {
                _logger.LogWarning("DeleteReminder (predicate): no matching reminder found in DB");
                return false;
            }

            _dbContext.Reminders.Remove(reminder);
            int result = await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Reminder {ReminderId} deleted from DB via predicate", reminder.Id);
            return result > 0;
        }

        public async Task<IEnumerable<Reminder>?> GetReminders(Expression<Predicate<Reminder>> predicate)
        {
            _logger.LogDebug("GetReminder (predicate) DB query initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            return await _dbContext.Reminders.Where(funcExpression).ToListAsync();
        }

    }
}
