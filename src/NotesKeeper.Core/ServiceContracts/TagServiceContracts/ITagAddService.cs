using NotesKeeper.Core.DTOs.TagDTOs;

namespace NotesKeeper.Core.ServiceContracts.TagServiceContracts
{
    /// <summary>
    /// Defines a contract for adding new tags.
    /// </summary>
    public interface ITagAddService
    {
        /// <summary>
        /// Adds a new tag based on the provided request and returns the created tag details.
        /// </summary>
        /// <param name="request">The tag creation request containing the tag details. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="TagResponse"/> representing the newly created tag, or <see langword="null"/> if the tag could not be created.</returns>
        Task<TagResponse?> AddTag(TagAddRequest request);
    }
}
