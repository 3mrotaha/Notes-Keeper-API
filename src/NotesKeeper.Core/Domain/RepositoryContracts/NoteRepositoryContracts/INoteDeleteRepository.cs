using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts
{
    /// <summary>
    /// Defines the contract for deleting notes from the repository.
    /// </summary>
    public interface INoteDeleteRepository
    {
        /// <summary>
        /// Deletes the note associated with the specified note identifier.
        /// </summary>
        /// <param name="noteId">The unique identifier of the note to delete. Must correspond to an existing note.</param>
        /// <returns><see langword="true"/> if the note was successfully deleted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> DeleteNote(int noteId);

        /// <summary>
        /// Deletes the first note that matches the specified criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions a note must satisfy to be deleted.</param>
        /// <returns><see langword="true"/> if a note was successfully deleted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> DeleteNote(Expression<Predicate<Note>> predicate);
    }
}
