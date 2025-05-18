using AllTheBeans.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AllTheBeans.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] Login request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            try
            {
                if (request.Username == "testuser" && request.Password == "password")
                {
                    var token = GenerateJwtToken(request.Username);
                    _logger.LogInformation("User {Username} authenticated successfully.", request.Username);
                    return Ok(new { token });
                }
                _logger.LogWarning("Authentication failed for user: {Username}", request.Username);
                return Unauthorized("Invalid credentials");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during login for user: {Username}", request.Username);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        private string GenerateJwtToken(string username)
        {
            try
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Tester")
                };

                var key = _configuration["Jwt:Key"];
                if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
                    throw new SecurityTokenException("JWT key is missing or too short. Must be at least 256 bits (32 bytes).");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user: {Username}", username);
                throw;
            }

        }
    }
}
