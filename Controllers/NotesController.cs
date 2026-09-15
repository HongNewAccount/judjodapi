using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JudjodApi.Data;

namespace JudjodApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : BaseApiController
{
    private readonly AppDbContext _db;
    public NotesController(AppDbContext db, IConfiguration config) : base(config) => _db = db;

    // GET /api/notes
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var now = DateTime.UtcNow;
        var raw = await _db.StickyNotes
            .Include(n => n.User)
            .Where(n => n.ExpiresAt >= now)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        var notes = raw.Select(n => new
        {
            n.Id,
            n.Title,
            n.Content,
            n.Color,
            n.CreatedAt,
            n.ExpiresAt,
            DaysLeft   = Math.Max(0, (int)(n.ExpiresAt.Date - now.Date).TotalDays),
            CreatedBy  = n.User?.Username,
            AuthorName = n.User == null ? null : $"{n.User.FirstName} {n.User.LastName}".Trim()
        });

        return Ok(notes);
    }

    // GET /api/notes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var now  = DateTime.UtcNow;
        var note = await _db.StickyNotes
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null) return NotFound(new { error = $"Note {id} not found." });

        return Ok(new
        {
            note.Id,
            note.Title,
            note.Content,
            note.Color,
            note.CreatedAt,
            note.ExpiresAt,
            DaysLeft   = Math.Max(0, (int)(note.ExpiresAt.Date - now.Date).TotalDays),
            CreatedBy  = note.User?.Username,
            AuthorName = note.User == null ? null : $"{note.User.FirstName} {note.User.LastName}".Trim()
        });
    }
}
