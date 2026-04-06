namespace NotesKeeper.Core.ServiceContracts.NoteServiceContracts
{
    /// <summary>
    /// Defines a contract for deleting notes.
    /// </summary>
    public interface INoteDeleteService
    {
        /// <summary>
        /// Deletes the note associated with the specified note identifier.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to delete.</param>
        /// <returns><see langword="true"/> if the note was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteNote(int noteId);
    }
}
