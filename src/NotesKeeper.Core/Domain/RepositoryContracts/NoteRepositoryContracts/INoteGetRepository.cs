using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts
{
    /// <summary>
    /// Defines methods for retrieving note information using various criteria.
    /// </summary>
    public interface INoteGetRepository
    {
        /// <summary>
        /// Retrieves the note associated with the specified note identifier.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="Note"/> object if found; otherwise, <see langword="null"/>.</returns>
        public Task<Note?> GetNote(int noteId);

        /// <summary>
        /// Retrieves the first note that satisfies the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions used to filter notes.</param>
        /// <returns>A note that matches the provided predicate; or <see langword="null"/> if no match is found.</returns>
        public Task<Note?> GetNote(Expression<Predicate<Note>> predicate);

        /// <summary>
        /// Retrieves the notes that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions used to filter notes.</param>
        /// <returns>A collection of notes that match the provided predicate; or <see langword="null"/> if no match is found.</returns>
        public Task<IEnumerable<Note>?> GetNotes(Expression<Predicate<Note>> predicate);
    }
}
