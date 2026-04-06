using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts
{
    /// <summary>
    /// Defines a contract for updating tag information in a data store.
    /// </summary>
    public interface ITagUpdateRepository
    {
        /// <summary>
        /// Updates the specified tag with new information and returns the updated tag.
        /// </summary>
        /// <param name="tag">The tag object containing the updated information. Must not be <see langword="null"/>.</param>
        /// <returns>The updated tag object if the update is successful; otherwise, <see langword="null"/>.</returns>
        public Task<Tag?> UpdateTag(Tag tag);
    }
}
