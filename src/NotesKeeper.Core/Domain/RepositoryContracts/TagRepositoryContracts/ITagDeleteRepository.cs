using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts
{
    /// <summary>
    /// Defines the contract for deleting tags from the repository.
    /// </summary>
    public interface ITagDeleteRepository
    {
        /// <summary>
        /// Deletes the tag associated with the specified tag identifier.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to delete. Must correspond to an existing tag.</param>
        /// <returns><see langword="true"/> if the tag was successfully deleted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> DeleteTag(int tagId);

        /// <summary>
        /// Deletes the first tag that matches the specified criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions a tag must satisfy to be deleted.</param>
        /// <returns><see langword="true"/> if a tag was successfully deleted; otherwise, <see langword="false"/>.</returns>
        public Task<bool> DeleteTag(Expression<Predicate<Tag>> predicate);
    }
}
