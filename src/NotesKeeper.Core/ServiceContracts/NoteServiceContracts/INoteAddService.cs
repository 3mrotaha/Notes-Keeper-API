using NotesKeeper.Core.DTOs.NoteDTOs;

namespace NotesKeeper.Core.ServiceContracts.NoteServiceContracts
{
    /// <summary>
    /// Defines a contract for adding new notes.
    /// </summary>
    public interface INoteAddService
    {
        /// <summary>
        /// Adds a new note based on the provided request and returns the created note details.
        /// </summary>
        /// <param name="request">The note creation request containing the note details. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="NoteResponse"/> representing the newly created note, or <see langword="null"/> if the note could not be created.</returns>
        Task<NoteResponse?> AddNote(NoteAddRequest request);
    }
}
