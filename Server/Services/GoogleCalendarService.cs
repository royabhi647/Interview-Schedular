using Server.Data;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly ServerDataContext _context;
        private readonly IConfiguration _configuration;

        public GoogleCalendarService(ServerDataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> CreateCalendarEventAsync(string accessToken, CreateInterviewDto interview)
        {
            // For now, return a placeholder Google Meet link
            await Task.Delay(100); // Simulate API call
            return $"https://meet.google.com/placeholder-{Guid.NewGuid().ToString("N")[..10]}";
        }

        public async Task<bool> DeleteCalendarEventAsync(string accessToken, string eventId)
        {
            await Task.Delay(100);
            return true;
        }

        public async Task<UserToken?> GetValidTokenAsync(string userId)
        {
            var token = await _context.UserTokens
                .FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);

            if (token == null)
                return null;

            // Check if token is expired
            if (token.ExpiresAt <= DateTime.UtcNow)
            {
                token.IsActive = false;
                await _context.SaveChangesAsync();
                return null;
            }

            return token;
        }

        public async Task<bool> RefreshTokenAsync(UserToken userToken)
        {
            await Task.Delay(100);
            userToken.IsActive = false;
            await _context.SaveChangesAsync();
            return false;
        }
    }
}