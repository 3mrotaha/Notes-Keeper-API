using Microsoft.Extensions.Logging;
using Moq;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.TagRepositoryContracts;
using NotesKeeper.Core.DTOs.TagDTOs;
using NotesKeeper.Core.Services;

namespace NotesKeeper.Tests.Unit;

public class TagServiceTests
{
    private readonly Mock<ITagAddRepository> _addRepo = new();
    private readonly Mock<ITagGetRepository> _getRepo = new();
    private readonly Mock<ITagUpdateRepository> _updateRepo = new();
    private readonly Mock<ITagDeleteRepository> _deleteRepo = new();
    private readonly Mock<ILogger<TagService>> _logger = new();
    private readonly TagService _sut;

    public TagServiceTests()
    {
        _sut = new TagService(
            _addRepo.Object,
            _getRepo.Object,
            _updateRepo.Object,
            _deleteRepo.Object,
            _logger.Object);
    }

    // ── AddTag ────────────────────────────────────────────────

    [Fact]
    public async Task AddTag_ValidRequest_ReturnsTagResponse()
    {
        var userId = Guid.NewGuid();
        _addRepo.Setup(r => r.AddTag(It.IsAny<Tag>())).ReturnsAsync(7);

        var result = await _sut.AddTag(new TagAddRequest { Name = "Work", UserId = userId });

        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        Assert.Equal("Work", result.Name);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task AddTag_RepositoryReturnsMinusOne_ReturnsNull()
    {
        _addRepo.Setup(r => r.AddTag(It.IsAny<Tag>())).ReturnsAsync(-1);

        var result = await _sut.AddTag(new TagAddRequest { Name = "Work", UserId = Guid.NewGuid() });

        Assert.Null(result);
    }

    // ── GetTag by id ──────────────────────────────────────────

    [Fact]
    public async Task GetTag_ById_Found_ReturnsTagResponse()
    {
        var tag = new Tag { Id = 3, Name = "Study", UserId = Guid.NewGuid() };
        _getRepo.Setup(r => r.GetTag(3)).ReturnsAsync(tag);

        var result = await _sut.GetTag(3);

        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Study", result.Name);
    }

    [Fact]
    public async Task GetTag_ById_NotFound_ReturnsNull()
    {
        _getRepo.Setup(r => r.GetTag(99)).ReturnsAsync((Tag?)null);

        var result = await _sut.GetTag(99);

        Assert.Null(result);
    }

    // ── GetTag by name ────────────────────────────────────────

    [Fact]
    public async Task GetTag_ByName_Found_ReturnsTagResponse()
    {
        var tag = new Tag { Id = 4, Name = "Health", UserId = Guid.NewGuid() };
        _getRepo.Setup(r => r.GetTag("Health")).ReturnsAsync(tag);

        var result = await _sut.GetTag("Health");

        Assert.NotNull(result);
        Assert.Equal("Health", result.Name);
    }

    [Fact]
    public async Task GetTag_ByName_NotFound_ReturnsNull()
    {
        _getRepo.Setup(r => r.GetTag("Ghost")).ReturnsAsync((Tag?)null);

        var result = await _sut.GetTag("Ghost");

        Assert.Null(result);
    }

    // ── UpdateTag ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateTag_ValidRequest_ReturnsUpdatedResponse()
    {
        var userId = Guid.NewGuid();
        var updated = new Tag { Id = 2, Name = "Finance", UserId = userId };
        _updateRepo.Setup(r => r.UpdateTag(It.IsAny<Tag>())).ReturnsAsync(updated);

        var result = await _sut.UpdateTag(2, new TagUpdateRequest { Name = "Finance", UserId = userId });

        Assert.NotNull(result);
        Assert.Equal("Finance", result.Name);
    }

    [Fact]
    public async Task UpdateTag_NotFound_ReturnsNull()
    {
        _updateRepo.Setup(r => r.UpdateTag(It.IsAny<Tag>())).ReturnsAsync((Tag?)null);

        var result = await _sut.UpdateTag(99, new TagUpdateRequest { Name = "X", UserId = Guid.NewGuid() });

        Assert.Null(result);
    }

    // ── DeleteTag ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteTag_Existing_ReturnsTrue()
    {
        _deleteRepo.Setup(r => r.DeleteTag(1)).ReturnsAsync(true);

        var result = await _sut.DeleteTag(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTag_NotFound_ReturnsFalse()
    {
        _deleteRepo.Setup(r => r.DeleteTag(99)).ReturnsAsync(false);

        var result = await _sut.DeleteTag(99);

        Assert.False(result);
    }
}
