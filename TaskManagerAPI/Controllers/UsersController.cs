using Microsoft.AspNetCore.Mvc;  // Import necessary namespaces for ASP.NET Core MVC features.
using Microsoft.IdentityModel.Tokens;  // For working with JWT tokens.
using System.IdentityModel.Tokens.Jwt;  // For creating JWT tokens.
using System.Security.Claims;  // For handling claims inside the JWT.
using System.Security.Cryptography;  // To use cryptographic functions like SHA256 hashing.
using System.Text;  // To handle string encoding, specifically UTF8 for hashing.
using TaskManagerAPI.Data;  // Accessing the database context.
using TaskManagerAPI.Models.Domain;  // Domain models like User, TaskItem, etc.
using TaskManagerAPI.Models.DTO;  // Data transfer objects used for exchanging data.

namespace TaskManagerAPI.Controllers  // Namespace where this controller resides.
{
    [Route("api/[controller]")]  // Defines the route for the API controller, dynamic controller name based on class name (Users).
    [ApiController]  // Marks the class as an API controller that follows conventions.
    public class UsersController : ControllerBase  // Controller class for managing user actions.
    {
        private readonly TaskItemDbContext _dbContext;  // Database context for interacting with the database.
        private readonly IConfiguration _configuration;  // To access application settings, like JWT settings.

        // Constructor to inject dependencies (dbContext and configuration).
        public UsersController(TaskItemDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // POST: api/users/register - This endpoint allows users to register.
        [HttpPost("register")]
        public IActionResult Register([FromBody] AddUserRequestDto addUserRequestDto)
        {
            // Check if the username already exists in the database.
            if (_dbContext.Users.Any(u => u.Username == addUserRequestDto.Username))
            {
                return BadRequest("Username already exists.");  // Return a bad request if username is taken.
            }

            // Create a new user object with provided data.
            var user = new User
            {
                Username = addUserRequestDto.Username,
                FullName = addUserRequestDto.FullName,
                PasswordHash = HashPassword(addUserRequestDto.Password)  // Hash the password before saving.
            };

            // Save the new user to the database.
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Return the newly created user as a response (without the password).
            return CreatedAtAction(nameof(Register), new { username = user.Username }, new UserDto
            {
                Username = user.Username,
                FullName = user.FullName
            });
        }

        // POST: api/users/login - Endpoint for users to log in and receive a JWT token.
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequestDto)
        {
            // Find the user by username in the database.
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == loginRequestDto.Username);

            // If user not found or password doesn't match, return unauthorized.
            if (user == null || user.PasswordHash != HashPassword(loginRequestDto.Password))
            {
                return Unauthorized("Invalid credentials.");  // Return unauthorized if invalid login.
            }

            // Generate a JWT token for the authenticated user.
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });  // Return the token to the client.
        }

        // Method to hash the password using SHA256 hashing algorithm.
        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())  // Using SHA256 hashing algorithm.
            {
                var bytes = Encoding.UTF8.GetBytes(password);  // Convert the password to bytes (UTF8).
                var hash = sha.ComputeHash(bytes);  // Compute the hash.
                return Convert.ToBase64String(hash);  // Return the hashed password as a base64 string.
            }
        }

        // Method to generate a JWT token for the user.
        private string GenerateJwtToken(User user)
        {
            // Get the JWT settings from the configuration file (appsettings.json).
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));  // Get the JWT secret key.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);  // Set signing credentials with the key and algorithm.

            // Claims include the user's unique ID and username.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // Subject claim with user ID.
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)  // Unique name claim with username.
            };

            // Create the JWT token with the claims, issuer, audience, and expiry time.
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],  // Issuer from configuration.
                audience: jwtSettings["Audience"],  // Audience from configuration.
                claims: claims,  // Add claims to the token.
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),  // Set expiry time.
                signingCredentials: creds  // Set signing credentials.
            );

            // Return the token as a string.
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
