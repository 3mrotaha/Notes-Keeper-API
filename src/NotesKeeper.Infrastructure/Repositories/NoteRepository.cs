using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts;
using NotesKeeper.Infrastructure.ApplicationDbContext;
using System.Linq.Expressions;

namespace NotesKeeper.Infrastructure.Repositories
{
    public class NoteRepository : INoteAddRepository, INoteGetRepository, INoteUpdateRepository, INoteDeleteRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<NoteRepository> _logger;

        public NoteRepository(AppDbContext dbContext, ILogger<NoteRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> AddNote(Note note)
        {
            _logger.LogDebug("AddNote DB operation started for UserId {UserId}", note.UserId);
            _dbContext.Notes.Add(note);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("Note {NoteId} persisted to DB for UserId {UserId}", note.Id, note.UserId);
                return note.Id;
            }

            _logger.LogWarning("AddNote SaveChanges returned 0 rows for UserId {UserId}", note.UserId);
            return -1;
        }

        public async Task<Note?> GetNote(int noteId)
        {
            _logger.LogDebug("GetNote DB query for NoteId {NoteId}", noteId);
            var note = await _dbContext.Notes.AsNoTracking()
                                            .Include(n => n.TagsAssignments)
                                            .ThenInclude(ta => ta.Tag)
                                            .Include(n => n.Reminder)
                                            .FirstOrDefaultAsync(n => n.Id == noteId);
            if (note is null)
                _logger.LogWarning("GetNote: NoteId {NoteId} not found in DB", noteId);

            return note;
        }

        public async Task<Note?> GetNote(Expression<Predicate<Note>> predicate)
        {
            _logger.LogDebug("GetNote (predicate) DB query initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            return await _dbContext.Notes.AsNoTracking()
                                        .FirstOrDefaultAsync(funcExpression);
        }

        public async Task<Note?> UpdateNoteContent(Note note)
        {
            _logger.LogDebug("UpdateNoteContent DB operation for NoteId {NoteId}", note.Id);
            Note? existing = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == note.Id);
            if (existing == null)
            {
                _logger.LogWarning("UpdateNoteContent: NoteId {NoteId} not found in DB", note.Id);
                return null;
            }

            existing.Title = note.Title;
            existing.NoteBody = note.NoteBody;
            
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Note {NoteId} content updated in DB", note.Id);
            return existing;
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            _logger.LogDebug("DeleteNote DB operation for NoteId {NoteId}", noteId);
            Note? note = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                _logger.LogWarning("DeleteNote: NoteId {NoteId} not found in DB", noteId);
                return false;
            }

            _dbContext.Notes.Remove(note);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
                _logger.LogInformation("Note {NoteId} deleted from DB", noteId);
            else
                _logger.LogWarning("DeleteNote: SaveChanges returned 0 rows for NoteId {NoteId}", noteId);

            return result > 0;
        }

        public async Task<bool> DeleteNote(Expression<Predicate<Note>> predicate)
        {
            _logger.LogDebug("DeleteNote (predicate) DB operation initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            Note? note = await _dbContext.Notes.FirstOrDefaultAsync(funcExpression);
            if (note == null)
            {
                _logger.LogWarning("DeleteNote (predicate): no matching note found in DB");
                return false;
            }

            _dbContext.Notes.Remove(note);
            int result = await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Note {NoteId} deleted from DB via predicate", note.Id);
            return result > 0;
        }

        public async Task<IEnumerable<Note>?> GetNotes(Expression<Predicate<Note>> predicate)
        {
            _logger.LogDebug("GetNotes (predicate) DB query initiated");
            var funcExpression = ExpressionConverter.ToFuncExpression(predicate);
            return await _dbContext.Notes.AsNoTracking()
                                        .Where(funcExpression)
                                        .ToListAsync();
        }

        public async Task<Note?> UpdateNoteTag(int noteId, int tagId)
        {
            _logger.LogDebug("UpdateNoteTag called for NoteId {NoteId}, TagId {TagId}", noteId, tagId);
            Note? note = await _dbContext.Notes.Include(n => n.TagsAssignments)
                                                .ThenInclude(ta => ta.Tag)
                                                .FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                _logger.LogWarning("UpdateNoteTag: NoteId {NoteId} not found in DB", noteId);
                return null;
            }

            Tag? tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == note.UserId);
            if (tag == null)
            {
                _logger.LogWarning("UpdateNoteTag: TagId {TagId} not found or does not belong to UserId {UserId}", tagId, note.UserId);
                return null;
            }

            if (note.TagsAssignments == null)
            {
                note.TagsAssignments = new List<TagsAssignments>();
            }

            if (!note.TagsAssignments.Any(ta => ta.TagId == tagId))
            {
                note.TagsAssignments.Add(new TagsAssignments { NoteId = noteId, TagId = tagId });
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Tag {TagId} assigned to Note {NoteId} in DB", tagId, noteId);
            return note;
        }

        public async Task<Note?> RemoveNoteTag(int noteId, int tagId)
        {
            _logger.LogDebug("RemoveNoteTag called for NoteId {NoteId}, TagId {TagId}", noteId, tagId);
            Note? note = await _dbContext.Notes.Include(n => n.TagsAssignments)
                                                .ThenInclude(ta => ta.Tag)
                                                .FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                _logger.LogWarning("RemoveNoteTag: NoteId {NoteId} not found in DB", noteId);
                return null;
            }

            TagsAssignments? assignment = note.TagsAssignments?.FirstOrDefault(ta => ta.TagId == tagId);
            if (assignment == null)
            {
                _logger.LogWarning("RemoveNoteTag: No assignment for TagId {TagId} on NoteId {NoteId}", tagId, noteId);
                return null;
            }

            // _dbContext.TagsAssignments.Remove(assignment);
            _dbContext.Entry(assignment).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Tag {TagId} removed from Note {NoteId} in DB", tagId, noteId);
            return note;
        }

        public async Task<Note?> UpdateNoteReminder(int noteId, int reminderId)
        {
            _logger.LogDebug("UpdateNoteReminder called for NoteId {NoteId}, ReminderId {ReminderId}", noteId, reminderId);
            var note = await _dbContext.Notes.Include(n => n.Reminder)
                                        .Where(n => n.Id == noteId)
                                        .FirstOrDefaultAsync();
            if (note == null)
            {
                _logger.LogWarning("UpdateNoteReminder: NoteId {NoteId} not found in DB", noteId);
                return null;
            }

            var reminder = await _dbContext.Reminders.Where(r => r.Id == reminderId)
                                                    .FirstOrDefaultAsync();
            if (reminder == null)
            {
                _logger.LogWarning("UpdateNoteReminder: ReminderId {ReminderId} not found in DB", reminderId);
                return null;
            }

            note.Reminder = reminder;
            note.ReminderId = reminderId;
            reminder.NoteId = noteId;

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Reminder {ReminderId} assigned to Note {NoteId} in DB", reminderId, noteId);
            return note;
        }

        public async Task<Note?> RemoveNoteReminder(int noteId, int reminderId)
        {
            _logger.LogDebug("RemoveNoteReminder called for NoteId {NoteId}, ReminderId {ReminderId}", noteId, reminderId);
            var note = await _dbContext.Notes.Where(n => n.Id == noteId)
                                        .FirstOrDefaultAsync();
            if (note == null)
            {
                _logger.LogWarning("RemoveNoteReminder: NoteId {NoteId} not found in DB", noteId);
                return null;
            }

            var reminder = await _dbContext.Reminders.Where(r => r.Id == reminderId)
                                                    .FirstOrDefaultAsync();
            if (reminder == null)
            {
                _logger.LogWarning("RemoveNoteReminder: ReminderId {ReminderId} not found in DB", reminderId);
                return null;
            }

            reminder.NoteId = null;
            note.Reminder = null;
            note.ReminderId = null;

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Reminder {ReminderId} removed from Note {NoteId} in DB", reminderId, noteId);
            return note;
        }
    }
}
