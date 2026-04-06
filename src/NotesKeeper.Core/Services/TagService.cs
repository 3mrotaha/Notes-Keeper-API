using Microsoft.Extensions.Logging;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts;
using NotesKeeper.Core.DTOs.TagDTOs;
using NotesKeeper.Core.Mappings;
using NotesKeeper.Core.ServiceContracts.TagServiceContracts;

namespace NotesKeeper.Core.Services
{
    public class TagService : ITagAddService, ITagGetService, ITagUpdateService, ITagDeleteService
    {
        private readonly ITagAddRepository _tagAddRepository;
        private readonly ITagGetRepository _tagGetRepository;
        private readonly ITagUpdateRepository _tagUpdateRepository;
        private readonly ITagDeleteRepository _tagDeleteRepository;
        private readonly ILogger<TagService> _logger;

        public TagService(
            ITagAddRepository tagAddRepository,
            ITagGetRepository tagGetRepository,
            ITagUpdateRepository tagUpdateRepository,
            ITagDeleteRepository tagDeleteRepository,
            ILogger<TagService> logger)
        {
            _tagAddRepository = tagAddRepository;
            _tagGetRepository = tagGetRepository;
            _tagUpdateRepository = tagUpdateRepository;
            _tagDeleteRepository = tagDeleteRepository;
            _logger = logger;
        }

        public async Task<TagResponse?> AddTag(TagAddRequest request)
        {
            _logger.LogDebug("AddTag called for UserId {UserId}, Name: '{Name}'", request.UserId, request.Name);
            Tag tag = request.ToTag();
            int id = await _tagAddRepository.AddTag(tag);
            if (id == -1)
            {
                _logger.LogWarning("AddTag failed for UserId {UserId} — repository returned -1", request.UserId);
                return null;
            }

            tag.Id = id;
            _logger.LogInformation("Tag {TagId} '{Name}' created for UserId {UserId}", id, request.Name, request.UserId);
            return tag.ToTagResponse();
        }

        public async Task<TagResponse?> GetTag(int tagId)
        {
            _logger.LogDebug("GetTag called for TagId {TagId}", tagId);
            Tag? tag = await _tagGetRepository.GetTag(tagId);
            if (tag is null)
            {
                _logger.LogWarning("GetTag: Tag {TagId} not found", tagId);
                return null;
            }

            _logger.LogInformation("Tag {TagId} retrieved successfully", tagId);
            return tag.ToTagResponse();
        }

        public async Task<TagResponse?> GetTag(string name)
        {
            _logger.LogDebug("GetTag called for Name '{Name}'", name);
            Tag? tag = await _tagGetRepository.GetTag(name);
            if (tag is null)
            {
                _logger.LogWarning("GetTag: Tag with Name '{Name}' not found", name);
                return null;
            }

            _logger.LogInformation("Tag '{Name}' retrieved successfully", name);
            return tag.ToTagResponse();
        }

        public async Task<TagResponse?> UpdateTag(int tagId, TagUpdateRequest request)
        {
            _logger.LogDebug("UpdateTag called for TagId {TagId}", tagId);
            Tag tag = request.ToTag();
            tag.Id = tagId;
            Tag? updated = await _tagUpdateRepository.UpdateTag(tag);
            if (updated is null)
            {
                _logger.LogWarning("UpdateTag: Tag {TagId} not found or update failed", tagId);
                return null;
            }

            _logger.LogInformation("Tag {TagId} updated successfully", tagId);
            return updated.ToTagResponse();
        }

        public async Task<bool> DeleteTag(int tagId)
        {
            _logger.LogDebug("DeleteTag called for TagId {TagId}", tagId);
            bool deleted = await _tagDeleteRepository.DeleteTag(tagId);
            if (!deleted)
                _logger.LogWarning("DeleteTag: Tag {TagId} not found or delete failed", tagId);
            else
                _logger.LogInformation("Tag {TagId} deleted successfully", tagId);

            return deleted;
        }

        public async Task<IEnumerable<TagResponse>?> GetTags(string name)
        {
            _logger.LogDebug("GetTags called with Name filter '{Name}'", name);
            IEnumerable<Tag>? tags = await _tagGetRepository.GetTags(t => t.Name.Contains(name));
            if (tags is null)
            {
                _logger.LogWarning("GetTags: no tags found for Name filter '{Name}'", name);
                return null;
            }

            var result = tags.Select(t => t.ToTagResponse()).ToList();
            _logger.LogInformation("GetTags returned {Count} tag(s) for Name filter '{Name}'", result.Count, name);
            return result;
        }

        public async Task<IEnumerable<TagResponse>?> GetUserTags(Guid userId)
        {
            _logger.LogDebug("GetUserTags called for UserId {UserId}", userId);
            IEnumerable<Tag>? tags = await _tagGetRepository.GetTags(t => t.UserId == userId);
            if (tags is null)
            {
                _logger.LogWarning("GetUserTags: no tags found for UserId {UserId}", userId);
                return null;
            }

            var result = tags.Select(t => t.ToTagResponse()).ToList();
            _logger.LogInformation("GetUserTags returned {Count} tag(s) for UserId {UserId}", result.Count, userId);
            return result;
        }
    }
}
