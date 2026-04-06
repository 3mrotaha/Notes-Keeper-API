using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts;
using NotesKeeper.Core.DTOs.NoteDTOs;
using NotesKeeper.Core.DTOs.TagDTOs;
using NotesKeeper.Core.Mappings;
using NotesKeeper.Core.ServiceContracts.NoteServiceContracts;

namespace NotesKeeper.Core.Services
{
    public class NoteService : INoteAddService, INoteGetService, INoteUpdateService, INoteDeleteService
    {
        private readonly INoteAddRepository _noteAddRepository;
        private readonly INoteGetRepository _noteGetRepository;
        private readonly INoteUpdateRepository _noteUpdateRepository;
        private readonly INoteDeleteRepository _noteDeleteRepository;
        private readonly ILogger<NoteService> _logger;

        public NoteService(
            INoteAddRepository noteAddRepository,
            INoteGetRepository noteGetRepository,
            INoteUpdateRepository noteUpdateRepository,
            INoteDeleteRepository noteDeleteRepository,
            ILogger<NoteService> logger)
        {
            _noteAddRepository = noteAddRepository;
            _noteGetRepository = noteGetRepository;
            _noteUpdateRepository = noteUpdateRepository;
            _noteDeleteRepository = noteDeleteRepository;
            _logger = logger;
        }

        public async Task<NoteResponse?> AddNote(NoteAddRequest request)
        {
            _logger.LogDebug("AddNote called for UserId {UserId}, Title: {Title}", request.UserId, request.Title);
            Note note = request.ToNote();
            int id = await _noteAddRepository.AddNote(note);
            if (id == -1)
            {
                _logger.LogWarning("AddNote failed for UserId {UserId} — repository returned -1", request.UserId);
                return null;
            }

            note.Id = id;
            _logger.LogInformation("Note {NoteId} created successfully for UserId {UserId}", id, request.UserId);
            return note.ToNoteResponse();
        }

        public async Task<NoteResponse?> GetNote(int noteId)
        {
            _logger.LogDebug("GetNote called for NoteId {NoteId}", noteId);
            Note? note = await _noteGetRepository.GetNote(noteId);
            if (note is null)
            {
                _logger.LogWarning("GetNote: Note {NoteId} not found", noteId);
                return null;
            }

            _logger.LogInformation("Note {NoteId} retrieved successfully", noteId);
            return note.ToNoteResponse();
        }

        public async Task<NoteResponse?> UpdateNote(int noteId, NoteUpdateRequest request)
        {
            _logger.LogDebug("UpdateNote called for NoteId {NoteId}", noteId);
            Note note = request.ToNote();
            note.Id = noteId;
            Note? updated = await _noteUpdateRepository.UpdateNoteContent(note);
            if (updated is null)
            {
                _logger.LogWarning("UpdateNote: Note {NoteId} not found or update failed", noteId);
                return null;
            }

            _logger.LogInformation("Note {NoteId} updated successfully", noteId);
            return updated.ToNoteResponse();
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            _logger.LogDebug("DeleteNote called for NoteId {NoteId}", noteId);
            bool deleted = await _noteDeleteRepository.DeleteNote(noteId);
            if (!deleted)
                _logger.LogWarning("DeleteNote: Note {NoteId} not found or delete failed", noteId);
            else
                _logger.LogInformation("Note {NoteId} deleted successfully", noteId);

            return deleted;
        }

        public async Task<IEnumerable<NoteResponse>?> GetNotes(Guid userId, string title)
        {
            _logger.LogDebug("GetNotes called for UserId {UserId} with Title filter '{Title}'", userId, title);
            var notes = await _noteGetRepository.GetNotes(note => note.UserId == userId && note.Title != null && note.Title.Contains(title));
            if (notes is null)
            {
                _logger.LogWarning("GetNotes: no results for UserId {UserId}, Title '{Title}'", userId, title);
                return null;
            }

            var result = notes.Select(n => n.ToNoteResponse()).ToList();
            _logger.LogInformation("GetNotes returned {Count} note(s) for UserId {UserId}", result.Count, userId);
            return result;
        }

        public async Task<IEnumerable<NoteResponse>?> GetUserNotes(Guid userId)
        {
            _logger.LogDebug("GetUserNotes called for UserId {UserId}", userId);
            var notes = await _noteGetRepository.GetNotes(note => note.UserId == userId);
            if (notes is null)
            {
                _logger.LogWarning("GetUserNotes: no notes found for UserId {UserId}", userId);
                return null;
            }

            var result = notes.Select(n => n.ToNoteResponse()).ToList();
            _logger.LogInformation("GetUserNotes returned {Count} note(s) for UserId {UserId}", result.Count, userId);
            return result;
        }

        public async Task<IEnumerable<NoteResponse>?> GetNotesByTag(Guid userId, int tagId)
        {
            _logger.LogDebug("GetNotesByTag called for UserId {UserId}, TagId {TagId}", userId, tagId);
            var notes = await _noteGetRepository.GetNotes(note => note.UserId == userId
                                                            && note.TagsAssignments != null
                                                            && note.TagsAssignments.Any(ta => ta.TagId == tagId));
            if (notes is null)
            {
                _logger.LogWarning("GetNotesByTag: no notes found for UserId {UserId}, TagId {TagId}", userId, tagId);
                return null;
            }

            var result = notes.Select(n => n.ToNoteResponse()).ToList();
            _logger.LogInformation("GetNotesByTag returned {Count} note(s) for UserId {UserId}, TagId {TagId}", result.Count, userId, tagId);
            return result;
        }

        public async Task<IEnumerable<NoteResponse>?> GetNotesByTagName(Guid userId, string tagName)
        {
            _logger.LogDebug("GetNotesByTagName called for UserId {UserId}, TagName '{TagName}'", userId, tagName);
            var notes = await _noteGetRepository.GetNotes(note => note.UserId == userId
                                                            && note.TagsAssignments != null
                                                            && note.TagsAssignments.Any(ta => ta.Tag != null && ta.Tag.Name == tagName));
            if (notes is null)
            {
                _logger.LogWarning("GetNotesByTagName: no notes for UserId {UserId}, TagName '{TagName}'", userId, tagName);
                return null;
            }

            var result = notes.Select(n => n.ToNoteResponse()).ToList();
            _logger.LogInformation("GetNotesByTagName returned {Count} note(s) for UserId {UserId}", result.Count, userId);
            return result;
        }

        public async Task<NoteResponse?> AssignTagToNote(int noteId, int tagId)
        {
            _logger.LogDebug("AssignTagToNote called for NoteId {NoteId}, TagId {TagId}", noteId, tagId);
            Note? note = await _noteUpdateRepository.UpdateNoteTag(noteId, tagId);
            if (note is null)
            {
                _logger.LogWarning("AssignTagToNote: Note {NoteId} or Tag {TagId} not found", noteId, tagId);
                return null;
            }

            _logger.LogInformation("Tag {TagId} assigned to Note {NoteId}", tagId, noteId);
            return note.ToNoteResponse();
        }

        public async Task<NoteResponse?> RemoveTagFromNote(int noteId, int tagId)
        {
            _logger.LogDebug("RemoveTagFromNote called for NoteId {NoteId}, TagId {TagId}", noteId, tagId);
            Note? note = await _noteUpdateRepository.RemoveNoteTag(noteId, tagId);
            if (note is null)
            {
                _logger.LogWarning("RemoveTagFromNote: Note {NoteId} or assignment for Tag {TagId} not found", noteId, tagId);
                return null;
            }

            _logger.LogInformation("Tag {TagId} removed from Note {NoteId}", tagId, noteId);
            return note.ToNoteResponse();
        }

        public async Task<NoteResponse?> AssignReminderToNote(int noteId, int reminderId)
        {
            _logger.LogDebug("AssignReminderToNote called for NoteId {NoteId}, ReminderId {ReminderId}", noteId, reminderId);
            Note? note = await _noteUpdateRepository.UpdateNoteReminder(noteId, reminderId);
            if (note is null)
            {
                _logger.LogWarning("AssignReminderToNote: Note {NoteId} or Reminder {ReminderId} not found", noteId, reminderId);
                return null;
            }

            _logger.LogInformation("Reminder {ReminderId} assigned to Note {NoteId}", reminderId, noteId);
            return note.ToNoteResponse();
        }

        public async Task<NoteResponse?> RemoveReminderFromNote(int noteId, int reminderId)
        {
            _logger.LogDebug("RemoveReminderFromNote called for NoteId {NoteId}, ReminderId {ReminderId}", noteId, reminderId);
            Note? note = await _noteUpdateRepository.RemoveNoteReminder(noteId, reminderId);
            if (note is null)
            {
                _logger.LogWarning("RemoveReminderFromNote: Note {NoteId} or Reminder {ReminderId} not found", noteId, reminderId);
                return null;
            }

            _logger.LogInformation("Reminder {ReminderId} removed from Note {NoteId}", reminderId, noteId);
            return note.ToNoteResponse();
        }
    }
}
