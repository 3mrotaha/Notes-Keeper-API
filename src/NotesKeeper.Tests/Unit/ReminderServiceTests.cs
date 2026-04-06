using Microsoft.Extensions.Logging;
using Moq;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.ReminderRepositoryContracts;
using NotesKeeper.Core.DTOs.ReminderDTOs;
using NotesKeeper.Core.Services;

namespace NotesKeeper.Tests.Unit;

public class ReminderServiceTests
{
    private readonly Mock<IReminderAddRepository> _addRepo = new();
    private readonly Mock<IReminderGetRepository> _getRepo = new();
    private readonly Mock<IReminderUpdateRepository> _updateRepo = new();
    private readonly Mock<IReminderDeleteRepository> _deleteRepo = new();
    private readonly Mock<ILogger<ReminderService>> _logger = new();
    private readonly ReminderService _sut;

    public ReminderServiceTests()
    {
        _sut = new ReminderService(
            _addRepo.Object,
            _getRepo.Object,
            _updateRepo.Object,
            _deleteRepo.Object,
            _logger.Object);
    }

    // ── AddReminder ───────────────────────────────────────────

    [Fact]
    public async Task AddReminder_ValidRequest_ReturnsReminderResponse()
    {
        _addRepo.Setup(r => r.AddReminder(It.IsAny<Reminder>())).ReturnsAsync(4);

        var result = await _sut.AddReminder(new ReminderAddRequest
        {
            NoteId = 1,
            Message = "Buy milk",
            DateTime = DateTime.UtcNow.AddHours(1)
        });

        Assert.NotNull(result);
        Assert.Equal(4, result.Id);
        Assert.Equal("Buy milk", result.Message);
    }

    [Fact]
    public async Task AddReminder_RepositoryReturnsMinusOne_ReturnsNull()
    {
        _addRepo.Setup(r => r.AddReminder(It.IsAny<Reminder>())).ReturnsAsync(-1);

        var result = await _sut.AddReminder(new ReminderAddRequest
        {
            NoteId = 1,
            DateTime = DateTime.UtcNow.AddHours(1)
        });

        Assert.Null(result);
    }

    // ── GetReminder ───────────────────────────────────────────

    [Fact]
    public async Task GetReminder_Found_ReturnsReminderResponse()
    {
        var reminder = new Reminder { Id = 2, NoteId = 1, Message = "Call dentist", DateTime = DateTime.UtcNow.AddDays(1) };
        _getRepo.Setup(r => r.GetReminder(2)).ReturnsAsync(reminder);

        var result = await _sut.GetReminder(2);

        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Call dentist", result.Message);
    }

    [Fact]
    public async Task GetReminder_NotFound_ReturnsNull()
    {
        _getRepo.Setup(r => r.GetReminder(99)).ReturnsAsync((Reminder?)null);

        var result = await _sut.GetReminder(99);

        Assert.Null(result);
    }

    // ── UpdateReminder ────────────────────────────────────────

    [Fact]
    public async Task UpdateReminder_ValidRequest_ReturnsUpdatedResponse()
    {
        var dt = DateTime.UtcNow.AddHours(2);
        var updated = new Reminder { Id = 3, NoteId = 1, Message = "Updated msg", DateTime = dt };
        _updateRepo.Setup(r => r.UpdateReminder(It.IsAny<Reminder>())).ReturnsAsync(updated);

        var result = await _sut.UpdateReminder(3, new ReminderUpdateRequest
        {
            NoteId = 1,
            Message = "Updated msg",
            DateTime = dt
        });

        Assert.NotNull(result);
        Assert.Equal("Updated msg", result.Message);
    }

    [Fact]
    public async Task UpdateReminder_NotFound_ReturnsNull()
    {
        _updateRepo.Setup(r => r.UpdateReminder(It.IsAny<Reminder>())).ReturnsAsync((Reminder?)null);

        var result = await _sut.UpdateReminder(99, new ReminderUpdateRequest
        {
            NoteId = 1,
            DateTime = DateTime.UtcNow
        });

        Assert.Null(result);
    }

    // ── DeleteReminder ────────────────────────────────────────

    [Fact]
    public async Task DeleteReminder_Existing_ReturnsTrue()
    {
        _deleteRepo.Setup(r => r.DeleteReminder(1)).ReturnsAsync(true);

        var result = await _sut.DeleteReminder(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteReminder_NotFound_ReturnsFalse()
    {
        _deleteRepo.Setup(r => r.DeleteReminder(99)).ReturnsAsync(false);

        var result = await _sut.DeleteReminder(99);

        Assert.False(result);
    }
}
