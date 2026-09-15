using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JudjodApi.Data;

namespace JudjodApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : BaseApiController
{
    private readonly AppDbContext _db;
    public LogsController(AppDbContext db, IConfiguration config) : base(config) => _db = db;

    // GET /api/logs
    // GET /api/logs?limit=20
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var limitStr = Request.Query["limit"].ToString();
        int? limit = int.TryParse(limitStr, out var n) && n > 0 ? n : null;

        var query = _db.ActivityLogs.Include(a => a.User).Include(a => a.Project).OrderByDescending(a => a.CreatedAt);

        var logs = await (limit.HasValue ? query.Take(limit.Value) : query)
            .Select(a => new
            {
                a.Id, a.ActionType, a.Description, a.OldValue, a.NewValue, a.CreatedAt,
                By = a.User == null ? null : a.User.Username,
                Project = a.Project == null ? null : a.Project.Name
            }).ToListAsync();

        return Ok(logs);
    }
}
