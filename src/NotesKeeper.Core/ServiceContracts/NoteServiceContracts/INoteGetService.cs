using NotesKeeper.Core.DTOs.NoteDTOs;

namespace NotesKeeper.Core.ServiceContracts.NoteServiceContracts
{
    /// <summary>
    /// Defines methods for retrieving note information.
    /// </summary>
    public interface INoteGetService
    {
        /// <summary>
        /// Retrieves the note associated with the specified note identifier.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to retrieve.</param>
        /// <returns>A <see cref="NoteResponse"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<NoteResponse?> GetNote(int noteId);
        Task<IEnumerable<NoteResponse>?> GetUserNotes(Guid userId);
        Task<IEnumerable<NoteResponse>?> GetNotes(Guid userId, string title);
        Task<IEnumerable<NoteResponse>?> GetNotesByTag(Guid userId, int tagId);
        Task<IEnumerable<NoteResponse>?> GetNotesByTagName(Guid userId, string tagName);
    }
}
