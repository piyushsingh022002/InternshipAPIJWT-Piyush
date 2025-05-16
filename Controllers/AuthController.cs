using InternshipAPI.Data;
using InternshipAPI.Models;
using InternshipAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace InternshipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            
            if (user == null)
                return BadRequest(new { message = "User not found", username = model.Username });

            bool passwordMatches = false;
            bool isValidHash = true;
            
            try
            {
                // Try to verify the password
                passwordMatches = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // If we get here, the hash is invalid
                isValidHash = false;
            }
            
            // If the hash is invalid or password doesn't match, update the password hash
            if (!isValidHash || !passwordMatches)
            {
                // Create a new hash and update the user's password
                string newHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
                user.Password = newHash;
                await _context.SaveChangesAsync();
                
                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                
                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role,
                    UserId = user.Id,
                    Expiration = DateTime.UtcNow.AddMinutes(120),
                    Message = isValidHash ? "Login successful" : "Password hash updated and login successful"
                });
            }
            
            // Password matches and hash is valid, generate JWT token
            var normalToken = _jwtService.GenerateToken(user);
            
            return Ok(new LoginResponse
            {
                Token = normalToken,
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id,
                Expiration = DateTime.UtcNow.AddMinutes(120) // Assuming 120 minutes expiry
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                return BadRequest(new { message = "Username already exists" });

            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest(new { message = "Email already exists" });

            // Create new user
            var user = new User
            {
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Email = model.Email,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin()
        {
            // Check if admin already exists
            if (await _context.Users.AnyAsync(u => u.Username == "admin"))
                return BadRequest(new { message = "Admin user already exists" });

            // Create admin user with fixed password
            var admin = new User
            {
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@example.com",
                Role = "HR",
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin user created successfully", username = "admin", password = "admin123" });
        }
    }
}
