using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts
{
    /// <summary>
    /// Provides an interface for adding new notes to a data store.
    /// </summary>
    public interface INoteAddRepository
    {
        /// <summary>
        /// Adds a new note to the data store and returns the unique identifier of the created note.
        /// </summary>
        /// <param name="note">A <see cref="Note"/> object containing the details of the note to add. Must not be <see langword="null"/>.</param>
        /// <returns>An integer representing the unique identifier of the newly created note. Returns -1 if the note could not be added.</returns>
        public Task<int> AddNote(Note note);
    }
}
