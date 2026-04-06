namespace NotesKeeper.Core.ServiceContracts.TagServiceContracts
{
    /// <summary>
    /// Defines a contract for deleting tags.
    /// </summary>
    public interface ITagDeleteService
    {
        /// <summary>
        /// Deletes the tag associated with the specified tag identifier.
        /// </summary>
        /// <param name="tagId">The unique identifier of the tag to delete.</param>
        /// <returns><see langword="true"/> if the tag was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteTag(int tagId);
    }
}
