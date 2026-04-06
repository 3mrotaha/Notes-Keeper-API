using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts
{
    /// <summary>
    /// Defines a contract for updating note information in a data store.
    /// </summary>
    public interface INoteUpdateRepository
    {
        /// <summary>
        /// Removes a tag from a note and returns the updated note.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note from which the tag will be removed.</param>
        /// <param name="tagId">The unique identifier of the tag to be removed.</param>
        /// <returns>The updated note object if the removal is successful; otherwise, <see langword="null"/>.</returns>
        Task<Note?> RemoveNoteTag(int noteId, int tagId);

        /// <summary>
        /// Updates the specified note with new information and returns the updated note.
        /// </summary>
        /// <param name="note">The note object containing the updated information. Must not be <see langword="null"/>.</param>
        /// <returns>The updated note object if the update is successful; otherwise, <see langword="null"/>.</returns>
        public Task<Note?> UpdateNoteContent(Note note);

        /// <summary>
        /// Updates the tags associated with a note by assigning a new tag to the note. Returns the updated note if successful; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to update.</param>
        /// <param name="tagId">The unique identifier of the tag to assign to the note.</param>
        /// <returns>The updated note object if the update is successful; otherwise, <see langword="null"/>.</returns>
        public Task<Note?> UpdateNoteTag(int noteId, int tagId);

        /// <summary>
        /// Updates the reminders associated with a note by assigning a new reminder to the note. Returns the updated note if successful; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to update.</param
        /// <param name="reminderId">The unique identifier of the reminder to assign to the note.</param>
        /// <returns>The updated note object if the update is successful; otherwise, <see langword="null"/>.</returns>
        /// <remarks>This method is intended to update the reminder associated with a note. If the note already has a reminder
        /// associated with it, the existing reminder will be replaced with the new reminder specified by <paramref name="reminderId"/>.</remarks>
        public Task<Note?> UpdateNoteReminder(int noteId, int reminderId);

        /// <summary>
        /// Removes a reminder from a note and returns the updated note.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note from which the reminder will be removed.</param>
        /// <param name="reminderId">The unique identifier of the reminder to be removed.</param>
        /// <returns>The updated note object if the removal is successful; otherwise, <see langword="null"/>.</returns>
        /// <remarks>This method is intended to remove a reminder associated with a note. If the note has a reminder associated with it that matches the specified <paramref name="reminderId"/>, the reminder will be removed from the note.</remarks>
        public Task<Note?> RemoveNoteReminder(int noteId, int reminderId);
    }
}
