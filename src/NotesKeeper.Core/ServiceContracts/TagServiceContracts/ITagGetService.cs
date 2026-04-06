using System.Linq.Expressions;
using NotesKeeper.Core.DTOs.TagDTOs;

namespace NotesKeeper.Core.ServiceContracts.TagServiceContracts
{
    /// <summary>
    /// Defines methods for retrieving tag information.
    /// </summary>
    public interface ITagGetService
    {
        /// <summary>
        /// Retrieves the tag associated with the specified tag identifier.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to retrieve.</param>
        /// <returns>A <see cref="TagResponse"/> if found; otherwise, <see langword="null"/>.</returns>
        Task<TagResponse?> GetTag(int tagId);

        /// <summary>
        /// Retrieves the tags associated with the specified tag name.
        /// </summary>
        /// <param name="name">The name of the tags to retrieve.</param>
        /// <returns>A collection of <see cref="TagResponse"/> objects if found; otherwise, <see langword="null"/>.</returns>
        Task<IEnumerable<TagResponse>?> GetTags(string name);

        /// <summary>
        /// Retrieves all tags associated with the specified user identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose tags are to be retrieved.</param>
        /// <returns>A collection of <see cref="TagResponse"/> objects associated with the specified user; otherwise, <see langword="null"/> if no tags are found.</returns>
        Task<IEnumerable<TagResponse>?> GetUserTags(Guid userId);
    }
}
