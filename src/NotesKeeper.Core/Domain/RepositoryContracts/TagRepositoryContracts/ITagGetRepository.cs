using NotesKeeper.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts
{
    /// <summary>
    /// Defines methods for retrieving tag information using various criteria.
    /// </summary>
    public interface ITagGetRepository
    {
        /// <summary>
        /// Retrieves the tag associated with the specified tag identifier.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="Tag"/> object if found; otherwise, <see langword="null"/>.</returns>
        public Task<Tag?> GetTag(int tagId);

        /// <summary>
        /// Retrieves the tag associated with the specified tag name.
        /// </summary>
        /// <param name="name">The name of the tag to retrieve. This parameter cannot be null or empty.</param>
        /// <returns>A <see cref="Tag"/> object if found; otherwise, <see langword="null"/>.</returns>
        public Task<Tag?> GetTag(string name);

        /// <summary>
        /// Retrieves the first tag that satisfies the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions used to filter tags.</param>
        /// <returns>A tag that matches the provided predicate; or <see langword="null"/> if no match is found.</returns>
        public Task<Tag?> GetTag(Expression<Predicate<Tag>> predicate);
        /// <summary>
        /// Retrieves a collection of tags that satisfy the specified filter criteria.
        /// </summary>
        /// <param name="predicate">An expression that defines the conditions used to filter tags.</param>
        /// <returns>A collection of tags that match the provided predicate; or <see langword="null"/> if no matches are found.</returns>
        /// <remarks>
        Task<IEnumerable<Tag>?> GetTags(Expression<Predicate<Tag>> value);
    }
}
