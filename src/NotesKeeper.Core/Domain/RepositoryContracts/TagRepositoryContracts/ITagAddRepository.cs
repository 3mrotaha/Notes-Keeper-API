using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts
{
    /// <summary>
    /// Provides an interface for adding new tags to a data store.
    /// </summary>
    public interface ITagAddRepository
    {
        /// <summary>
        /// Adds a new tag to the data store and returns the unique identifier of the created tag.
        /// </summary>
        /// <param name="tag">A <see cref="Tag"/> object containing the details of the tag to add. Must not be <see langword="null"/>.</param>
        /// <returns>An integer representing the unique identifier of the newly created tag. Returns -1 if the tag could not be added.</returns>
        public Task<int> AddTag(Tag tag);
    }
}
