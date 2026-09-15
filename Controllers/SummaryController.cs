using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JudjodApi.Data;

namespace JudjodApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummaryController : BaseApiController
{
    private readonly AppDbContext _db;
    public SummaryController(AppDbContext db, IConfiguration config) : base(config) => _db = db;

    // GET /api/summary
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var total = await _db.Projects.CountAsync();
        var byStatus = await _db.Projects.GroupBy(p => p.Status).Select(g => new { Status = g.Key ?? "Unknown", Count = g.Count() }).ToListAsync();
        var byPriority = await _db.Projects.GroupBy(p => p.Priority).Select(g => new { Priority = g.Key ?? "None", Count = g.Count() }).ToListAsync();
        var avg = await _db.Projects.AverageAsync(p => (double)p.Progress);
        var users = await _db.Users.CountAsync(u => u.IsActive);

        return Ok(new
        {
            TotalProjects = total,
            TotalActiveUsers = users,
            AverageProgress = Math.Round(avg, 1),
            ByStatus = byStatus,
            ByPriority = byPriority
        });
    }
}
