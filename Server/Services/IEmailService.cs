using Server.Models;

namespace Server.Services
{
    public interface IEmailService
    {
        Task<bool> SendInterviewNotificationAsync(Interview interview);
        Task<bool> SendInterviewCancellationAsync(Interview interview);
    }
}