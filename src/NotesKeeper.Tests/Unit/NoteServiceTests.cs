using Microsoft.Extensions.Logging;
using Moq;
using NotesKeeper.Core.Domain.Entities;
using NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts;
using NotesKeeper.Core.DTOs.NoteDTOs;
using NotesKeeper.Core.Services;

namespace NotesKeeper.Tests.Unit;

public class NoteServiceTests
{
    private readonly Mock<INoteAddRepository> _addRepo = new();
    private readonly Mock<INoteGetRepository> _getRepo = new();
    private readonly Mock<INoteUpdateRepository> _updateRepo = new();
    private readonly Mock<INoteDeleteRepository> _deleteRepo = new();
    private readonly Mock<ILogger<NoteService>> _logger = new();
    private readonly NoteService _sut;

    public NoteServiceTests()
    {
        _sut = new NoteService(
            _addRepo.Object,
            _getRepo.Object,
            _updateRepo.Object,
            _deleteRepo.Object,
            _logger.Object);
    }

    // ── AddNote ──────────────────────────────────────────────

    [Fact]
    public async Task AddNote_ValidRequest_ReturnsNoteResponse()
    {
        var userId = Guid.NewGuid();
        var request = new NoteAddRequest { UserId = userId, Title = "Test", NoteBody = "Body" };
        _addRepo.Setup(r => r.AddNote(It.IsAny<Note>())).ReturnsAsync(1);

        var result = await _sut.AddNote(request);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Title);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task AddNote_RepositoryReturnsMinusOne_ReturnsNull()
    {
        _addRepo.Setup(r => r.AddNote(It.IsAny<Note>())).ReturnsAsync(-1);

        var result = await _sut.AddNote(new NoteAddRequest { UserId = Guid.NewGuid(), Title = "T", NoteBody = "B" });

        Assert.Null(result);
    }

    // ── GetNote ───────────────────────────────────────────────

    [Fact]
    public async Task GetNote_ExistingId_ReturnsNoteResponse()
    {
        var note = new Note { Id = 5, UserId = Guid.NewGuid(), Title = "Hi", NoteBody = "There" };
        _getRepo.Setup(r => r.GetNote(5)).ReturnsAsync(note);

        var result = await _sut.GetNote(5);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("Hi", result.Title);
    }

    [Fact]
    public async Task GetNote_NotFound_ReturnsNull()
    {
        _getRepo.Setup(r => r.GetNote(99)).ReturnsAsync((Note?)null);

        var result = await _sut.GetNote(99);

        Assert.Null(result);
    }

    // ── UpdateNote ────────────────────────────────────────────

    [Fact]
    public async Task UpdateNote_ValidRequest_ReturnsUpdatedResponse()
    {
        var userId = Guid.NewGuid();
        var updated = new Note { Id = 3, UserId = userId, Title = "New", NoteBody = "New body" };
        _updateRepo.Setup(r => r.UpdateNoteContent(It.IsAny<Note>())).ReturnsAsync(updated);

        var result = await _sut.UpdateNote(3, new NoteUpdateRequest { UserId = userId, Title = "New", NoteBody = "New body" });

        Assert.NotNull(result);
        Assert.Equal("New", result.Title);
    }

    [Fact]
    public async Task UpdateNote_NotFound_ReturnsNull()
    {
        _updateRepo.Setup(r => r.UpdateNoteContent(It.IsAny<Note>())).ReturnsAsync((Note?)null);

        var result = await _sut.UpdateNote(99, new NoteUpdateRequest { UserId = Guid.NewGuid(), Title = "X", NoteBody = "Y" });

        Assert.Null(result);
    }

    // ── DeleteNote ────────────────────────────────────────────

    [Fact]
    public async Task DeleteNote_Existing_ReturnsTrue()
    {
        _deleteRepo.Setup(r => r.DeleteNote(1)).ReturnsAsync(true);

        var result = await _sut.DeleteNote(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteNote_NotFound_ReturnsFalse()
    {
        _deleteRepo.Setup(r => r.DeleteNote(99)).ReturnsAsync(false);

        var result = await _sut.DeleteNote(99);

        Assert.False(result);
    }

    // ── GetUserNotes ──────────────────────────────────────────

    [Fact]
    public async Task GetUserNotes_WithNotes_ReturnsList()
    {
        var userId = Guid.NewGuid();
        var notes = new List<Note>
        {
            new Note { Id = 1, UserId = userId, Title = "A", NoteBody = "A body" },
            new Note { Id = 2, UserId = userId, Title = "B", NoteBody = "B body" }
        };
        _getRepo.Setup(r => r.GetNotes(It.IsAny<System.Linq.Expressions.Expression<Predicate<Note>>>()))
                .ReturnsAsync(notes);

        var result = await _sut.GetUserNotes(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserNotes_NoNotes_ReturnsNull()
    {
        _getRepo.Setup(r => r.GetNotes(It.IsAny<System.Linq.Expressions.Expression<Predicate<Note>>>()))
                .ReturnsAsync((IEnumerable<Note>?)null);

        var result = await _sut.GetUserNotes(Guid.NewGuid());

        Assert.Null(result);
    }

    // ── AssignTagToNote ───────────────────────────────────────

    [Fact]
    public async Task AssignTagToNote_Valid_ReturnsNoteResponse()
    {
        var note = new Note { Id = 1, UserId = Guid.NewGuid(), Title = "N", NoteBody = "B" };
        _updateRepo.Setup(r => r.UpdateNoteTag(1, 10)).ReturnsAsync(note);

        var result = await _sut.AssignTagToNote(1, 10);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task AssignTagToNote_NotFound_ReturnsNull()
    {
        _updateRepo.Setup(r => r.UpdateNoteTag(1, 10)).ReturnsAsync((Note?)null);

        var result = await _sut.AssignTagToNote(1, 10);

        Assert.Null(result);
    }

    // ── RemoveTagFromNote ─────────────────────────────────────

    [Fact]
    public async Task RemoveTagFromNote_Valid_ReturnsNoteResponse()
    {
        var note = new Note { Id = 1, UserId = Guid.NewGuid(), Title = "N", NoteBody = "B" };
        _updateRepo.Setup(r => r.RemoveNoteTag(1, 10)).ReturnsAsync(note);

        var result = await _sut.RemoveTagFromNote(1, 10);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RemoveTagFromNote_NotFound_ReturnsNull()
    {
        _updateRepo.Setup(r => r.RemoveNoteTag(1, 10)).ReturnsAsync((Note?)null);

        var result = await _sut.RemoveTagFromNote(1, 10);

        Assert.Null(result);
    }

    // ── AssignReminderToNote / RemoveReminderFromNote ─────────

    [Fact]
    public async Task AssignReminderToNote_Valid_ReturnsNoteResponse()
    {
        var note = new Note { Id = 1, UserId = Guid.NewGuid(), Title = "N", NoteBody = "B" };
        _updateRepo.Setup(r => r.UpdateNoteReminder(1, 5)).ReturnsAsync(note);

        var result = await _sut.AssignReminderToNote(1, 5);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task RemoveReminderFromNote_Valid_ReturnsNoteResponse()
    {
        var note = new Note { Id = 1, UserId = Guid.NewGuid(), Title = "N", NoteBody = "B" };
        _updateRepo.Setup(r => r.RemoveNoteReminder(1, 5)).ReturnsAsync(note);

        var result = await _sut.RemoveReminderFromNote(1, 5);

        Assert.NotNull(result);
    }
}
