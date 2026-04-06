
using NotesKeeper.Core.ServiceContracts.NoteServiceContracts;
using NotesKeeper.Core.ServiceContracts.ReminderServiceContracts;

namespace NotesKeeperWebApi.Services;

public class ReminderNotificationService : BackgroundService
{
    // private readonly IReminderGetService _reminderGetService;
    // private readonly INoteGetService _noteGetService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReminderNotificationService> _logger;
    public ReminderNotificationService(ILogger<ReminderNotificationService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckRemindersAndNotifyUsers();
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task CheckRemindersAndNotifyUsers()
    {
        using var scope = _scopeFactory.CreateScope();
        var reminderGetService = scope.ServiceProvider.GetRequiredService<IReminderGetService>();
        var noteGetService = scope.ServiceProvider.GetRequiredService<INoteGetService>();
        var reminders = await reminderGetService.GetDueReminders(-1); // reminders between now and 1 minute late

        if(reminders is not null)
        {            
            foreach(var reminder in reminders)
            {                
                if(reminder.NoteId is null)
                    continue;

                var note = await noteGetService.GetNote(reminder.NoteId.Value);
                
                if(note == null)
                    continue;

                var notification = new
                {
                    note.UserId,
                    NoteId = note.Id,
                    NoteTitle = note.Title,
                    ReminderMessage = reminder.Message,
                    TimeSince = (DateTime.UtcNow - reminder.DateTime).TotalMinutes,                    
                };

                // send notification to user
                _logger.LogInformation("Sent Notification {Notification}", notification);
            }
        }        
    }
}
