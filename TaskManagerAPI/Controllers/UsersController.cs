using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models.Domain;
using TaskManagerAPI.Models.DTO;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TaskItemDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UsersController(TaskItemDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] AddUserRequestDto addUserRequestDto)
        {
            if (_dbContext.Users.Any(u => u.Username == addUserRequestDto.Username))
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                Username = addUserRequestDto.Username,
                FullName = addUserRequestDto.FullName,
                PasswordHash = HashPassword(addUserRequestDto.Password)
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Register), new { username = user.Username }, new UserDto
            {
                Username = user.Username,
                FullName = user.FullName
            });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == loginRequestDto.Username);
            if (user == null || user.PasswordHash != HashPassword(loginRequestDto.Password))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
