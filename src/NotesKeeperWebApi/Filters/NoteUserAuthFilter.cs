using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NotesKeeper.Core.ServiceContracts.NoteServiceContracts;

public class NoteUserAuthFilter : IAsyncAuthorizationFilter
{
    private readonly INoteGetService _noteGetService;
    private readonly ILogger<NoteUserAuthFilter> _logger;
    public NoteUserAuthFilter(INoteGetService noteGetService, ILogger<NoteUserAuthFilter> logger)
    {
        _noteGetService = noteGetService;
        _logger = logger;
    }


    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        Guid userId = Guid.Parse(context.HttpContext.Request.Headers["userId"].ToString());
        int? noteId = Convert.ToInt32(context.HttpContext.Request.RouteValues["noteId"]?.ToString());

        _logger.LogInformation("NoteUserAuthFilter: Authorizing userId={UserId} for noteId={NoteId}", userId, noteId);

        if (noteId is null)
        {
            _logger.LogWarning("NoteUserAuthFilter: noteId is missing from route values. Returning 400.");
            context.Result = new BadRequestObjectResult("no route of noteId");
            return;
        }

        var note = await _noteGetService.GetNote(noteId.Value);

        if (note?.UserId != userId)
        {
            _logger.LogWarning("NoteUserAuthFilter: userId={UserId} is not the owner of noteId={NoteId}. Returning 401.", userId, noteId);
            context.Result = new UnauthorizedObjectResult($"userId={userId} is not the owner of noteId={noteId}");
            return;
        }

        _logger.LogInformation("NoteUserAuthFilter: Authorization succeeded for userId={UserId} on noteId={NoteId}", userId, noteId);
    }
}