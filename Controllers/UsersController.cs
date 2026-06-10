using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollApp.Backend.DbContexts;
using System.Linq;
using System.Threading.Tasks;

namespace PollApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly PollDbContext _context;

        public UsersController(PollDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Role,
                    u.TeamName,
                    u.IsActive,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(System.Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            
            if (user.Role == "Admin") return BadRequest(new { error = "Cannot deactivate an Admin." });

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User status updated." });
        }
    }
}
