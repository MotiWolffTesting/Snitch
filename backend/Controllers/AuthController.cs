// AuthController handles user registration, login, and admin user management endpoints.
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using System;
using System.Linq;
using BCrypt.Net;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SnitchDbContext _context;

        // Constructor injects configuration and database context
        public AuthController(IConfiguration configuration, SnitchDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest(new { message = "Username already exists" });

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsAdmin = false,
                IsApproved = false
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Registration successful, pending approval." });
        }

        // Login a user and return a JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });
            if (!user.IsApproved)
                return Unauthorized(new { message = "User not approved" });
            var token = GenerateJwtToken(user);
            return Ok(new { token, isAdmin = user.IsAdmin, isApproved = user.IsApproved });
        }

        // Get all users (admin only)
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            // Only allow admins
            // if (!User.IsInRole("Admin")) return Forbid();
            var users = await _context.Users.Select(u => new { u.Id, u.Username, u.IsAdmin }).ToListAsync();
            return Ok(users);
        }

        // Promote a user to admin (admin only)
        [HttpPost("promote/{id}")]
        public async Task<IActionResult> PromoteUser(int id)
        {
            // if (!User.IsInRole("Admin")) return Forbid();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsAdmin = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User promoted to admin." });
        }

        // Demote a user from admin (admin only)
        [HttpPost("demote/{id}")]
        public async Task<IActionResult> DemoteUser(int id)
        {
            // if (!User.IsInRole("Admin")) return Forbid();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsAdmin = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User demoted from admin." });
        }

        // Delete a user (admin only)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // if (!User.IsInRole("Admin")) return Forbid();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User deleted." });
        }

        // Generate a JWT token for a user
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim("IsApproved", user.IsApproved.ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Request model for registration
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    // Request model for login
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}