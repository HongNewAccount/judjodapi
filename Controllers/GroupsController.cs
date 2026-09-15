using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JudjodApi.Data;

namespace JudjodApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : BaseApiController
{
    private readonly AppDbContext _db;
    public GroupsController(AppDbContext db, IConfiguration config) : base(config) => _db = db;

    // GET /api/groups
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var groups = await _db.ProjectGroups
            .Include(g => g.ProjectAssignments).ThenInclude(a => a.Project)
            .OrderBy(g => g.Name)
            .Select(g => new
            {
                g.Id, g.Name, g.Color, g.CreatedAt,
                Projects = g.ProjectAssignments.Select(a => new
                {
                    a.Project.Id, a.Project.Name, a.Project.Status, a.Project.Progress
                })
            }).ToListAsync();

        return Ok(groups);
    }
}
