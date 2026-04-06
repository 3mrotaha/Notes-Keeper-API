using Microsoft.AspNetCore.Mvc;
using NotesKeeper.Core.DTOs.TagDTOs;
using NotesKeeper.Core.ServiceContracts.TagServiceContracts;
using NotesKeeper.UI.Controllers;

namespace NotesKeeperWebApi.Controllers
{
    [TypeFilter(typeof(UserCheckFilter))]
    public class TagController : CustomControllerBase
    {
        private readonly ITagAddService _tagAddService;
        private readonly ITagDeleteService _tagDeleteService;
        private readonly ITagGetService _tagGetService;
        private readonly ITagUpdateService _tagUpdateService;
        private readonly ILogger<TagController> _logger;

        public TagController(
            ITagAddService tagAddService,
            ITagDeleteService tagDeleteService,
            ITagGetService tagGetService,
            ITagUpdateService tagUpdateService,
            ILogger<TagController> logger)
        {
            _tagAddService = tagAddService;
            _tagDeleteService = tagDeleteService;
            _tagGetService = tagGetService;
            _tagUpdateService = tagUpdateService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddTag(TagAddRequest tagRequest)
        {
            _logger.LogDebug("AddTag called with request: {@TagRequest}", tagRequest);

            TagResponse? response = await _tagAddService.AddTag(tagRequest);

            if (response == null)
            {
                _logger.LogWarning("AddTag failed — service returned null for request: {@TagRequest}", tagRequest);
                return Problem(detail: "Failed to add tag", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tag added successfully. TagId: {TagId}", response.Id);
            return Ok(response);
        }

        [HttpDelete("{tagId:int}")]
        public async Task<IActionResult> DeleteTag(int tagId)
        {
            _logger.LogDebug("DeleteTag called for TagId: {TagId}", tagId);

            bool isDeleted = await _tagDeleteService.DeleteTag(tagId);
            if (!isDeleted)
            {
                _logger.LogWarning("DeleteTag failed — tag not found or could not be deleted. TagId: {TagId}", tagId);
                return Problem(detail: "Failed to delete tag", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tag deleted successfully. TagId: {TagId}", tagId);
            return Ok(new { Message = "Tag deleted successfully" });
        }

        [HttpGet("{tagId:int}")]
        public async Task<IActionResult> GetTag(int tagId)
        {
            _logger.LogDebug("GetTag called for TagId: {TagId}", tagId);

            TagResponse? response = await _tagGetService.GetTag(tagId);
            if (response == null)
            {
                _logger.LogWarning("GetTag returned null. Tag not found. TagId: {TagId}", tagId);
                return Problem(detail: "Failed to get tag", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tag retrieved successfully. TagId: {TagId}", tagId);
            return Ok(response);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetTags(string name)
        {
            _logger.LogDebug("GetTags called for Name: {Name}", name);

            IEnumerable<TagResponse>? response = await _tagGetService.GetTags(name);
            if (response == null)
            {
                _logger.LogWarning("GetTags returned null for Name: {Name}", name);
                return Problem(detail: "Failed to get tags", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tags retrieved for Name: {Name}. Count: {Count}", name, response.Count());
            return Ok(response);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserTags(Guid userId)
        {
            _logger.LogDebug("GetUserTags called for UserId: {UserId}", userId);

            IEnumerable<TagResponse>? response = await _tagGetService.GetUserTags(userId);
            if (response == null)
            {
                _logger.LogWarning("GetUserTags returned null for UserId: {UserId}", userId);
                return Problem(detail: "Failed to get user tags", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("User tags retrieved for UserId: {UserId}. Count: {Count}", userId, response.Count());
            return Ok(response);
        }

        [HttpPut("{tagId:int}")]
        public async Task<IActionResult> UpdateTag(int tagId, TagUpdateRequest tagRequest)
        {
            _logger.LogDebug("UpdateTag called for TagId: {TagId}", tagId);

            TagResponse? response = await _tagUpdateService.UpdateTag(tagId, tagRequest);
            if (response == null)
            {
                _logger.LogWarning("UpdateTag returned null. Tag not found or update failed. TagId: {TagId}", tagId);
                return Problem(detail: "Failed to update tag", statusCode: StatusCodes.Status400BadRequest);
            }

            _logger.LogInformation("Tag updated successfully. TagId: {TagId}", tagId);
            return Ok(response);
        }
    }
}