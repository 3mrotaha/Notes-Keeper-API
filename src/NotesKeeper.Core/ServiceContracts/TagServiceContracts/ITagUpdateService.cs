using NotesKeeper.Core.DTOs.TagDTOs;

namespace NotesKeeper.Core.ServiceContracts.TagServiceContracts
{
    /// <summary>
    /// Defines a contract for updating tag information.
    /// </summary>
    public interface ITagUpdateService
    {
        /// <summary>
        /// Updates the tag identified by the specified identifier with the provided details.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to update.</param>
        /// <param name="request">The tag update request containing the new tag details. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="TagResponse"/> representing the updated tag, or <see langword="null"/> if the tag was not found.</returns>
        Task<TagResponse?> UpdateTag(int tagId, TagUpdateRequest request);
    }
}
