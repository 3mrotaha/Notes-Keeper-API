using Microsoft.AspNetCore.Mvc;
using NotesKeeper.Core.DTOs.NoteDTOs;
using NotesKeeper.Core.DTOs.ReminderDTOs;
using NotesKeeper.Core.ServiceContracts.NoteServiceContracts;
using NotesKeeper.Core.ServiceContracts.ReminderServiceContracts;

namespace NotesKeeper.UI.Controllers
{
    [TypeFilter(typeof(UserCheckFilter))]
    public class NoteController : CustomControllerBase
    {
        private readonly INoteAddService _noteAddService;
        private readonly INoteDeleteService _noteDeleteService;
        private readonly INoteGetService _noteGetService;
        private readonly INoteUpdateService _noteUpdateService;
        private readonly ILogger<NoteController> _logger;

        public NoteController(
            INoteAddService noteAddService,
            INoteDeleteService noteDeleteService,
            INoteGetService noteGetService,
            INoteUpdateService noteUpdateService,
            ILogger<NoteController> logger)
        {
            _noteAddService = noteAddService;
            _noteDeleteService = noteDeleteService;
            _noteGetService = noteGetService;
            _noteUpdateService = noteUpdateService;
            _logger = logger;
        }

        // add note
        [HttpPost]
        public async Task<IActionResult> AddNote([FromHeader] Guid userId, NoteAddRequest noteRequest)
        {
            _logger.LogDebug("AddNote called with request: {@NoteRequest}", noteRequest);

            if(userId != noteRequest.UserId)
            {
                _logger.LogWarning("AddNote failed — request not authorized: {@NoteRequest} - Issuer UserId {userId}", noteRequest, userId);
                return Problem(detail: "user Id don't match request.userId", statusCode: StatusCodes.Status401Unauthorized);
            }

            NoteResponse? response = await _noteAddService.AddNote(noteRequest);

            if (response == null)
            {
                _logger.LogWarning("AddNote failed — service returned null for request: {@NoteRequest}", noteRequest);
                return Problem(detail: "Failed to add note", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Note added successfully. NoteId: {NoteId}", response.Id);
            return Ok(response);
        }

        // delete note
        [HttpDelete("{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            _logger.LogDebug("DeleteNote called for NoteId: {NoteId}", noteId);

            bool isDeleted = await _noteDeleteService.DeleteNote(noteId);
            if (!isDeleted)
            {
                _logger.LogWarning("DeleteNote failed — note not found or could not be deleted. NoteId: {NoteId}", noteId);
                return Problem(detail: "Failed to delete note", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Note deleted successfully. NoteId: {NoteId}", noteId);
            return Ok(new { Message = "Note deleted successfully" });
        }

        // get note by id
        [HttpGet("{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> GetNote(int noteId)
        {
            _logger.LogDebug("GetNote called for NoteId: {NoteId}", noteId);

            NoteResponse? response = await _noteGetService.GetNote(noteId);
            if (response == null)
            {
                _logger.LogWarning("GetNote returned null. Note not found. NoteId: {NoteId}", noteId);
                return Problem(detail: "Failed to get note", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Note retrieved successfully. NoteId: {NoteId}", noteId);
            return Ok(response);
        }

        // update note
        [HttpPut("{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> UpdateNote(int noteId, NoteUpdateRequest noteRequest)
        {
            _logger.LogDebug("UpdateNote called for NoteId: {NoteId}", noteId);

            NoteResponse? response = await _noteUpdateService.UpdateNote(noteId, noteRequest);
            if (response == null)
            {
                _logger.LogWarning("UpdateNote returned null. Note not found or update failed. NoteId: {NoteId}", noteId);
                return Problem(detail: "Failed to update note", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Note updated successfully. NoteId: {NoteId}", noteId);
            return Ok(response);
        }

        // get notes by title
        [HttpGet("search/{title}")]
        public async Task<IActionResult> GetNotes([FromHeader] Guid userId, string title)
        {
            _logger.LogDebug("GetNotes called for UserId: {UserId}, Title: {Title}", userId, title);

            IEnumerable<NoteResponse>? response = await _noteGetService.GetNotes(userId, title);
            if (response == null)
            {
                _logger.LogWarning("GetNotes returned null for UserId: {UserId}, Title: {Title}", userId, title);
                return Problem(detail: "Failed to get notes", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Notes retrieved for UserId: {UserId}, Title: {Title}. Count: {Count}", userId, title, response.Count());
            return Ok(response);
        }

        // get notes by user id
        [HttpGet("user")]
        public async Task<IActionResult> GetUserNotes([FromHeader] Guid userId)
        {
            _logger.LogDebug("GetUserNotes called for UserId: {UserId}", userId);

            IEnumerable<NoteResponse>? response = await _noteGetService.GetUserNotes(userId);
            if (response == null)
            {
                _logger.LogWarning("GetUserNotes returned null for UserId: {UserId}", userId);
                return Problem(detail: "Failed to get user notes", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("User notes retrieved for UserId: {UserId}. Count: {Count}", userId, response.Count());
            return Ok(response);
        }

        // get notes by tag id
        [HttpGet("tag/{tagId:int}")]
        public async Task<IActionResult> GetNotesByTag([FromHeader] Guid userId, int tagId)
        {
            _logger.LogDebug("GetNotesByTag called for UserId: {UserId}, TagId: {TagId}", userId, tagId);

            IEnumerable<NoteResponse>? response = await _noteGetService.GetNotesByTag(userId, tagId);
            if (response == null)
            {
                _logger.LogWarning("GetNotesByTag returned null for UserId: {UserId}, TagId: {TagId}", userId, tagId);
                return Problem(detail: "Failed to get notes by tag", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Notes by tag retrieved for UserId: {UserId}, TagId: {TagId}. Count: {Count}", userId, tagId, response.Count());
            return Ok(response);
        }

        // get notes by tag name
        [HttpGet("tag/search/{tagName}")]
        public async Task<IActionResult> GetNotesByTagName([FromHeader] Guid userId, string tagName)
        {
            _logger.LogDebug("GetNotesByTagName called for UserId: {UserId}, TagName: {TagName}", userId, tagName);

            IEnumerable<NoteResponse>? response = await _noteGetService.GetNotesByTagName(userId, tagName);
            if (response == null)
            {
                _logger.LogWarning("GetNotesByTagName returned null for UserId: {UserId}, TagName: {TagName}", userId, tagName);
                return Problem(detail: "Failed to get notes by tag name", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Notes by tag name retrieved for UserId: {UserId}, TagName: {TagName}. Count: {Count}", userId, tagName, response.Count());
            return Ok(response);
        }

        // assign tag to note
        [HttpPut("tag/assign/{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> UpdateNoteTags(int noteId, [FromBody] int tagId)
        {
            _logger.LogDebug("UpdateNoteTags (assign) called for NoteId: {NoteId}, TagId: {TagId}", noteId, tagId);

            NoteResponse? response = await _noteUpdateService.AssignTagToNote(noteId, tagId);
            if (response == null)
            {
                _logger.LogWarning("AssignTagToNote returned null for NoteId: {NoteId}, TagId: {TagId}", noteId, tagId);
                return Problem(detail: "Failed to update note tags", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tag {TagId} assigned to Note {NoteId} successfully", tagId, noteId);
            return Ok(new { Message = "Note tags updated successfully", Note = response });
        }

        // remove tag from note
        [HttpPut("tag/remove/{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> DeleteNoteTags(int noteId, [FromBody] int tagId)
        {
            _logger.LogDebug("DeleteNoteTags (remove) called for NoteId: {NoteId}, TagId: {TagId}", noteId, tagId);

            NoteResponse? response = await _noteUpdateService.RemoveTagFromNote(noteId, tagId);
            if (response == null)
            {
                _logger.LogWarning("RemoveTagFromNote returned null for NoteId: {NoteId}, TagId: {TagId}", noteId, tagId);
                return Problem(detail: "Failed to delete note tags", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tag {TagId} removed from Note {NoteId} successfully", tagId, noteId);
            return Ok(new { Message = "Note tags deleted successfully", Note = response });
        }

        // add a new reminder to note
        [HttpPost("reminder/{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> UpdateNoteReminder(int noteId, [FromBody] ReminderAddRequest reminderAddRequest, [FromServices] IReminderAddService reminderAddService)
        {
            _logger.LogDebug("AddNoteReminder called for NoteId: {NoteId}", noteId);

            if (reminderAddRequest == null || reminderAddService == null)
            {
                _logger.LogWarning("AddNoteReminder — missing reminder request or service for NoteId: {NoteId}", noteId);
                return Problem(detail: "Can't fetch reminder details", statusCode: StatusCodes.Status400BadRequest);
            }

            var reminder = await reminderAddService.AddReminder(reminderAddRequest);
            if (reminder == null)
            {
                _logger.LogWarning("AddReminder returned null for NoteId: {NoteId}", noteId);
                return Problem(detail: "failed to add the new reminder", statusCode: StatusCodes.Status400BadRequest);
            }

            var note = await _noteUpdateService.AssignReminderToNote(noteId, reminder.Id);
            if (note == null)
            {
                _logger.LogWarning("AssignReminderToNote returned null for NoteId: {NoteId}, ReminderId: {ReminderId}", noteId, reminder.Id);
                return Problem(detail: "Failed to update note", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Reminder {ReminderId} added and assigned to Note {NoteId} successfully", reminder.Id, noteId);
            return Ok(note);
        }

        // update an existing reminder on a note
        [HttpPut("reminder/{noteId:int}/{reminderId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> UpdateNoteReminder(int noteId, [FromRoute] int reminderId, [FromBody] ReminderUpdateRequest reminderUpdateRequest, [FromServices] IReminderUpdateService reminderUpdateService)
        {
            _logger.LogDebug("UpdateNoteReminder called for NoteId: {NoteId}, ReminderId: {ReminderId}", noteId, reminderId);

            if (reminderUpdateRequest == null || reminderUpdateService == null)
            {
                _logger.LogWarning("UpdateNoteReminder — missing reminder request or service for NoteId: {NoteId}, ReminderId: {ReminderId}", noteId, reminderId);
                return Problem(detail: "Can't fetch reminder details", statusCode: StatusCodes.Status400BadRequest);
            }

            var reminder = await reminderUpdateService.UpdateReminder(reminderId, reminderUpdateRequest);
            if (reminder == null)
            {
                _logger.LogWarning("UpdateReminder returned null for ReminderId: {ReminderId}", reminderId);
                return Problem(detail: "failed to update the new reminder", statusCode: StatusCodes.Status400BadRequest);
            }

            var note = await _noteUpdateService.AssignReminderToNote(noteId, reminder.Id);
            if (note == null)
            {
                _logger.LogWarning("AssignReminderToNote returned null after update for NoteId: {NoteId}, ReminderId: {ReminderId}", noteId, reminder.Id);
                return Problem(detail: "Failed to update note", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Reminder {ReminderId} updated and reassigned to Note {NoteId} successfully", reminder.Id, noteId);
            return Ok(note);
        }

        // remove a reminder from a note
        [HttpDelete("reminder/{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        public async Task<IActionResult> UpdateNoteReminder(int noteId, [FromBody] int reminderId, [FromServices] IReminderDeleteService reminderDeleteService)
        {
            _logger.LogDebug("RemoveNoteReminder called for NoteId: {NoteId}, ReminderId: {ReminderId}", noteId, reminderId);

            if (reminderDeleteService == null)
            {
                _logger.LogWarning("RemoveNoteReminder — reminderDeleteService is null for NoteId: {NoteId}", noteId);
                return Problem(detail: "Can't fetch reminder details", statusCode: StatusCodes.Status400BadRequest);
            }

            var note = await _noteUpdateService.RemoveReminderFromNote(noteId, reminderId);
            if (note == null)
            {
                _logger.LogWarning("RemoveReminderFromNote returned null for NoteId: {NoteId}, ReminderId: {ReminderId}", noteId, reminderId);
                return Problem(detail: "Failed to update note", statusCode: StatusCodes.Status400BadRequest);
            }

            bool isDeleted = await reminderDeleteService.DeleteReminder(reminderId);
            if (isDeleted == false)
            {
                _logger.LogWarning("DeleteReminder returned false for ReminderId: {ReminderId}", reminderId);
                return Problem(detail: "failed to update the new reminder", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Reminder {ReminderId} removed from Note {NoteId} successfully", reminderId, noteId);
            return Ok(note);
        }        
    }
}