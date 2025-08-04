using Server.Models;

namespace Server.Services
{
    public interface IGoogleCalendarService
    {
        Task<string> CreateCalendarEventAsync(string accessToken, CreateInterviewDto interview);
        Task<bool> DeleteCalendarEventAsync(string accessToken, string eventId);
        Task<UserToken?> GetValidTokenAsync(string userId);
        Task<bool> RefreshTokenAsync(UserToken userToken);
    }
}