using Asp.Versioning;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Identity.Client;
using NotesKeeper.Core.Domain.RepositoryContracts.NoteRepositoryContracts;
using NotesKeeper.Core.DTOs.TagDTOs;
using NotesKeeper.Core.ServiceContracts.NoteServiceContracts;
using NotesKeeper.UI.Controllers;

namespace MyApp.Namespace
{
    [ApiVersion("2.0")]
    public class FileController : CustomControllerBase
    {

        private readonly INoteGetService _noteGetService;
        private readonly IConverter _pdfService;
        public FileController(INoteGetService noteGetService, IConverter pdfService)
        {
            _noteGetService = noteGetService;
            _pdfService = pdfService;
        }

        [HttpGet]
        [Route("export/note/{noteId:int}")]
        [TypeFilter(typeof(NoteUserAuthFilter))]
        [EnableRateLimiting("concurrent")] // this api endpoint takes time and heavy cpu load, so control the concurrent requests on the server side.
        public async Task<IActionResult> ExportNoteAsPdf(int noteId)
        {
            var note = await _noteGetService.GetNote(noteId);

            if (note is null)
                return Problem(detail: $"Can't find the note of id={noteId}", statusCode: StatusCodes.Status400BadRequest);


            var pdfDocument = await generatePdfFile(note.Title, note.NoteBody, note.CreatedAt, note.Tags);

            
            return File(pdfDocument, "application/pdf", $"Note-{note.UserId}-{note.Id}.pdf");
        }

        private Task<byte[]> generatePdfFile(string? title, string? body, DateTime createdAt, List<TagResponse?>? tags)
        {
            string htmlTemplate = @"
                <html>
                <head>
                    <meta charset='utf-8'>
                    <title>{{NOTE_TITLE}}</title>
                    <style>
                        body { font-family: Arial, sans-serif; margin: 20px; line-height: 1.5; }
                        h1 { color: #2c3e50; font-size: 24px; margin-bottom: 5px; }
                        .date { font-size: 12px; color: #7f8c8d; margin-bottom: 15px; }
                        p { font-size: 14px; color: #34495e; white-space: pre-wrap; }
                        .tags { margin-top: 15px; }
                        .tag { display: inline-block; background-color: #3498db; color: white; 
                            padding: 3px 8px; border-radius: 4px; margin-right: 5px; font-size: 12px; }
                        hr { margin: 20px 0; border: 0; border-top: 1px solid #ddd; }
                    </style>
                </head>
                <body>
                    <h1>{{NOTE_TITLE}}</h1>
                    <div class='date'>Created at: {{CREATED_AT}}</div>
                    <hr>
                    <p>{{NOTE_BODY}}</p>
                    <div class='tags'>
                        {{TAGS}}
                    </div>
                </body>
                </html>";

            string tagsHtml = "";
            if (tags is not null)
                tagsHtml = string.Join(" ", tags.Select(t => $"<span class='tag'>{t.Name}</span>"));
            else
                tagsHtml = "<span class='tag'>No Tags</span>";

            // Replace placeholders
            string htmlContent = htmlTemplate
                .Replace("{{NOTE_TITLE}}", title)
                .Replace("{{NOTE_BODY}}", body)
                .Replace("{{TAGS}}", tagsHtml)
                .Replace("{{CREATED_AT}}", createdAt.ToString("yyyy-MM-dd HH:mm"));

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Margins = new MarginSettings { Top = 10, Bottom = 10 }
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        HtmlContent = htmlContent
                    }
                }
            };

            return Task.FromResult(_pdfService.Convert(doc));
        }
    }
}

