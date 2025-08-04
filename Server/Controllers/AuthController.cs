using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ServerDataContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ServerDataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                var clientId = _configuration["Google:ClientId"];
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/Auth/google-callback";

                var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                             $"client_id={clientId}&" +
                             $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                             $"response_type=code&" +
                             $"scope={Uri.EscapeDataString("openid profile email https://www.googleapis.com/auth/calendar")}&" +
                             $"access_type=offline";

                return Ok(new { authUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to initiate Google authentication", detail = ex.Message });
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string? state = null)
        {
            var frontendUrl = _configuration["Frontend:Url"] ?? "http://localhost:3000";

            try
            {
                // For testing, create a fake token
                var existingTokens = _context.UserTokens.Where(t => t.UserId == "default-user");
                _context.UserTokens.RemoveRange(existingTokens);

                var userToken = new UserToken
                {
                    UserId = "default-user",
                    AccessToken = "fake-access-token-" + Guid.NewGuid().ToString("N")[..16],
                    RefreshToken = "fake-refresh-token-" + Guid.NewGuid().ToString("N")[..16],
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsActive = true
                };

                _context.UserTokens.Add(userToken);
                await _context.SaveChangesAsync();

                return Redirect($"{frontendUrl}?auth=success");
            }
            catch (Exception ex)
            {
                return Redirect($"{frontendUrl}?auth=error&message={Uri.EscapeDataString(ex.Message)}");
            }
        }

        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            try
            {
                var token = _context.UserTokens
                    .Where(t => t.UserId == "default-user" && t.IsActive)
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefault();

                var isAuthenticated = token != null && token.ExpiresAt > DateTime.UtcNow;

                return Ok(new
                {
                    isAuthenticated,
                    expiresAt = token?.ExpiresAt,
                    hasToken = token != null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to check authentication status", detail = ex.Message });
            }
        }

        [HttpPost("fake-login")]
        public async Task<IActionResult> FakeLogin()
        {
            try
            {
                // Remove existing tokens
                var existingTokens = _context.UserTokens.Where(t => t.UserId == "default-user");
                _context.UserTokens.RemoveRange(existingTokens);

                // Create fake token for testing
                var userToken = new UserToken
                {
                    UserId = "default-user",
                    AccessToken = "fake-access-token-" + Guid.NewGuid().ToString("N")[..16],
                    RefreshToken = "fake-refresh-token-" + Guid.NewGuid().ToString("N")[..16],
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsActive = true
                };

                _context.UserTokens.Add(userToken);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Fake authentication successful", token = userToken.AccessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create fake authentication", detail = ex.Message });
            }
        }
    }
}