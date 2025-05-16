using InternshipAPI.Data;
using InternshipAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternshipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HR")]
    public class HrController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HrController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all interns
        [HttpGet("interns")]
        public async Task<ActionResult<IEnumerable<Intern>>> GetInterns()
        {
            var interns = await _context.Interns.ToListAsync();
            return Ok(interns);
        }

        // Get intern by id
        [HttpGet("interns/{id}")]
        public async Task<ActionResult<Intern>> GetIntern(int id)
        {
            var intern = await _context.Interns.FindAsync(id);

            if (intern == null)
            {
                return NotFound(new { message = "Intern not found" });
            }

            return Ok(intern);
        }

        // Create a new intern
        [HttpPost("interns")]
        public async Task<ActionResult<Intern>> CreateIntern(Intern intern)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists
            if (await _context.Interns.AnyAsync(i => i.Email == intern.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            _context.Interns.Add(intern);
            await _context.SaveChangesAsync();

            // If UserId is not provided, create a user account for the intern
            if (intern.UserId == null)
            {
                var user = new User
                {
                    Username = intern.Email.Split('@')[0], // Use part of email as username
                    Password = BCrypt.Net.BCrypt.HashPassword("Intern@123"), // Default password
                    Email = intern.Email,
                    Role = "Intern",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Update the intern with the user id
                intern.UserId = user.Id;
                _context.Entry(intern).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetIntern), new { id = intern.Id }, intern);
        }

        // Update an intern
        [HttpPut("interns/{id}")]
        public async Task<IActionResult> UpdateIntern(int id, Intern intern)
        {
            if (id != intern.Id)
            {
                return BadRequest(new { message = "Id mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if email already exists for another intern
            if (await _context.Interns.AnyAsync(i => i.Email == intern.Email && i.Id != id))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Get the existing intern to check if we need to update the User entity
            var existingIntern = await _context.Interns.FindAsync(id);
            if (existingIntern == null)
            {
                return NotFound(new { message = "Intern not found" });
            }

            // Update the existing intern properties instead of tracking a new entity
            existingIntern.FirstName = intern.FirstName;
            existingIntern.LastName = intern.LastName;
            existingIntern.Email = intern.Email;
            existingIntern.Phone = intern.Phone;
            existingIntern.JoiningDate = intern.JoiningDate;
            existingIntern.Department = intern.Department;
            existingIntern.Project = intern.Project;
            // Don't update UserId as it should remain the same

            // If the intern has a UserId, also update the corresponding User entity
            if (existingIntern.UserId.HasValue)
            {
                var user = await _context.Users.FindAsync(existingIntern.UserId.Value);
                if (user != null)
                {
                    // Update user properties that should match the intern
                    user.Email = existingIntern.Email;
                    // Update the role if needed
                    user.Role = "Intern"; // Set the role to Intern
                    Console.WriteLine("User entity updated with role: " + user.Role);
                }
            }

            try
            {
                Console.WriteLine("Saving changes !!!");
                await _context.SaveChangesAsync();
                Console.WriteLine("changes saved !!!");
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Catch block opens !!!");
                if (!await InternExists(id))
                {
                    return NotFound(new { message = "Intern not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete an intern
        [HttpDelete("interns/{id}")]
        public async Task<IActionResult> DeleteIntern(int id)
        {
            var intern = await _context.Interns.FindAsync(id);
            if (intern == null)
            {
                return NotFound(new { message = "Intern not found" });
            }

            _context.Interns.Remove(intern);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> InternExists(int id)
        {
            return await _context.Interns.AnyAsync(e => e.Id == id);
        }
    }
}
