using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SolarPlantAPI.Data;
using SolarPlantAPI.Models;
using BCrypt.Net;
using SolarPlantAPI.Services;

namespace SolarPlantAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(DatabaseContext databaseContext, JwtService jwtService, ILogger<AuthenticationController> logger)
        {
            _databaseContext = databaseContext;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            _logger.LogInformation("New user registration attempt for username: {Username}", user.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration data provided.");
                return BadRequest(ModelState);
            }
            try
            {
                if (await _databaseContext.Users.AnyAsync(u => u.Username == user.Username))
                    return BadRequest("Username already exists.");
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                if (user.Role != "Admin" && user.Role != "User")
                {
                    _logger.LogWarning("User role set to 'User'.");
                    user.Role = "User";
                }

                _databaseContext.Add(user);
                await _databaseContext.SaveChangesAsync();

                _logger.LogInformation("User '{Username}' registered successfully.", user.Username);
                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering user '{Username}'.", user.Username);
                return StatusCode(500, "An error occurred while registering the user.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            _logger.LogInformation("Login attempt for username: {Username}", user.Username);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login data provided.");
                return BadRequest(ModelState);
            }
            try
            {
                var userFromDb = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

                if (userFromDb == null || !BCrypt.Net.BCrypt.Verify(user.PasswordHash, userFromDb.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", user.Username);
                    return Unauthorized(new { message = "Credentials are invalid." });
                }

                var token = _jwtService.GenerateJwtToken(userFromDb);

                _logger.LogInformation("User '{Username}' logged in successfully.", user.Username);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login for user '{Username}'.", user.Username);
                return StatusCode(500, "An error occurred while processing login.");
            }
        }
    }
}
