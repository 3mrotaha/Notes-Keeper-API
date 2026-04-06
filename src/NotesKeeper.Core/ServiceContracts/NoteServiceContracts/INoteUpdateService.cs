using NotesKeeper.Core.DTOs.NoteDTOs;
using NotesKeeper.Core.DTOs.TagDTOs;

namespace NotesKeeper.Core.ServiceContracts.NoteServiceContracts
{
    /// <summary>
    /// Defines a contract for updating note information.
    /// </summary>
    public interface INoteUpdateService
    {
        /// <summary>
        /// Updates the note identified by the specified identifier with the provided details.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to update.</param>
        /// <param name="request">The note update request containing the new note details. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="NoteResponse"/> representing the updated note, or <see langword="null"/> if the note was not found.</returns>
        Task<NoteResponse?> UpdateNote(int noteId, NoteUpdateRequest request);

        /// <summary>
        /// Updates the tags associated with a note by assigning a new tag to the note. Returns the updated note if successful; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to update.</param>
        /// <param name="tagId">The unique identifier of the tag to assign to the note.</param>
        /// <returns>The updated note object if the update is successful; otherwise, <see langword="null"/>.</returns>
        Task<NoteResponse?> AssignTagToNote(int noteId, int tagId);

        /// <summary>
        /// Removes a tag from a note and returns the updated note.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note from which the tag will be removed.</param>
        /// <param name="tagId">The unique identifier of the tag to be removed.</param>
        /// <returns>The updated note object if the removal is successful; otherwise, <see langword="null"/>.</returns>
        Task<NoteResponse?> RemoveTagFromNote(int noteId, int tagId);

        /// <summary>
        /// Updates the reminders associated with a note by assigning a new reminder to the note. Returns the updated note if successful; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to update.</param>
        /// <param name="reminderId">The unique identifier of the reminder to assign to the note.</param>
        /// <returns>The updated note object if the update is successful; otherwise, <see langword="null"/>.</returns>
        Task<NoteResponse?> AssignReminderToNote(int noteId, int reminderId);
        
        /// <summary>
        /// Removes a reminder from a note and returns the updated note.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note from which the reminder will be removed.</param>
        /// <param name="reminderId">The unique identifier of the reminder to be removed.</param>
        /// <returns>The updated note object if the removal is successful; otherwise, <see langword="null"/>.</returns>
        Task<NoteResponse?> RemoveReminderFromNote(int noteId, int reminderId);
    }
}
