using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JudjodApi.Data;

namespace JudjodApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseApiController
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db, IConfiguration config) : base(config) => _db = db;

    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var users = await _db.Users.OrderBy(u => u.Username).Select(u => new
        {
            u.Id, u.Username, u.FirstName, u.LastName,
            FullName = $"{u.FirstName} {u.LastName}".Trim(),
            u.Email, u.Phone, u.Role, u.WorkLocation, u.IsActive, u.CreatedAt
        }).ToListAsync();

        return Ok(users);
    }

    // GET /api/users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!IsAuthorized()) return Unauthorized(new { error = "Invalid or missing API key." });

        var u = await _db.Users.Where(u => u.Id == id).Select(u => new
        {
            u.Id, u.Username, u.FirstName, u.LastName,
            FullName = $"{u.FirstName} {u.LastName}".Trim(),
            u.Email, u.Phone, u.Role, u.WorkLocation, u.IsActive, u.CreatedAt
        }).FirstOrDefaultAsync();

        if (u == null) return NotFound(new { error = $"User {id} not found." });
        return Ok(u);
    }
}
