using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JudjodApi.Data;

namespace JudjodApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : BaseApiController
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db, IConfiguration config) : base(config) => _db = db;

    // GET /api/projects
    // GET /api/projects?status=InProgress&priority=High
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] string? priority)
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var q = _db.Projects.Include(p => p.Owners).ThenInclude(o => o.User).Include(p => p.CreatedByUser).AsQueryable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(p => p.Status == status);
        if (!string.IsNullOrEmpty(priority)) q = q.Where(p => p.Priority == priority);

        var result = await q.OrderBy(p => p.SortOrder).Select(p => new
        {
            p.Id, p.Name, p.Description, p.Status, p.Priority, p.Progress,
            p.StartDate, p.EndDate, p.CreatedAt, p.UpdatedAt,
            CreatedBy = p.CreatedByUser == null ? null : p.CreatedByUser.Username,
            Owners = p.Owners.Select(o => new
            {
                o.UserId,
                Username = o.User == null ? null : o.User.Username,
                FullName = o.User == null ? null : $"{o.User.FirstName} {o.User.LastName}".Trim()
            })
        }).ToListAsync();

        return Ok(result);
    }

    // GET /api/projects/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var p = await _db.Projects
            .Include(p => p.Owners).ThenInclude(o => o.User)
            .Include(p => p.CreatedByUser)
            .Include(p => p.ActivityLogs).ThenInclude(a => a.User)
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id, p.Name, p.Description, p.Status, p.Priority, p.Progress,
                p.Issues, p.StartDate, p.EndDate, p.CreatedAt, p.UpdatedAt,
                CreatedBy = p.CreatedByUser == null ? null : p.CreatedByUser.Username,
                Owners = p.Owners.Select(o => new
                {
                    o.UserId,
                    Username = o.User == null ? null : o.User.Username,
                    FullName = o.User == null ? null : $"{o.User.FirstName} {o.User.LastName}".Trim()
                }),
                RecentActivity = p.ActivityLogs.OrderByDescending(a => a.CreatedAt).Take(10).Select(a => new
                {
                    a.ActionType, a.Description, a.CreatedAt,
                    By = a.User == null ? null : a.User.Username
                })
            }).FirstOrDefaultAsync();

        if (p == null) return NotFound(new { error = $"Project {id} not found." });
        return Ok(p);
    }

    // GET /api/projects/archive
    [HttpGet("archive")]
    public async Task<IActionResult> GetArchive()
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var result = await _db.Projects
            .Include(p => p.Owners).ThenInclude(o => o.User)
            .Include(p => p.CreatedByUser)
            .Where(p => p.Status == "Closed")
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .Select(p => new
            {
                p.Id, p.Name, p.Description, p.Priority, p.Progress,
                p.StartDate, p.EndDate, p.CreatedAt, ClosedAt = p.UpdatedAt,
                CreatedBy = p.CreatedByUser == null ? null : p.CreatedByUser.Username,
                Owners = p.Owners.Select(o => new
                {
                    o.UserId,
                    Username = o.User == null ? null : o.User.Username,
                    FullName = o.User == null ? null : $"{o.User.FirstName} {o.User.LastName}".Trim()
                })
            }).ToListAsync();

        return Ok(result);
    }
}
