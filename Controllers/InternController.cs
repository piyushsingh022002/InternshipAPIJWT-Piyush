using InternshipAPI.Data;
using InternshipAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InternshipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Intern")]
    public class InternController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InternController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get welcome message
        [HttpGet("welcome")]
        public IActionResult GetWelcomeMessage()
        {
            return Ok(new { message = "Welcome to the Internship Portal! We're glad to have you on board." });
        }

        // Get current intern details
        [HttpGet("profile")]
        public async Task<ActionResult<Intern>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return BadRequest(new { message = "Invalid user" });
            }

            var intern = await _context.Interns.FirstOrDefaultAsync(i => i.UserId == id);
            if (intern == null)
            {
                return NotFound(new { message = "Intern profile not found" });
            }

            return Ok(intern);
        }

        // Get list of all interns (limited information)
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<object>>> GetInternList()
        {
            var interns = await _context.Interns
                .Select(i => new
                {
                    i.Id,
                    i.FirstName,
                    i.LastName,
                    i.Department,
                    i.Project
                })
                .ToListAsync();

            return Ok(interns);
        }

        // Update own profile (limited fields)
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] Intern internUpdate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int id))
            {
                return BadRequest(new { message = "Invalid user" });
            }

            var intern = await _context.Interns.FirstOrDefaultAsync(i => i.UserId == id);
            if (intern == null)
            {
                return NotFound(new { message = "Intern profile not found" });
            }

            // Only allow updating certain fields
            intern.Phone = internUpdate.Phone;
            
            try
            {
                await _context.SaveChangesAsync();
                return Ok(intern);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "An error occurred while updating the profile" });
            }
        }
    }
}
