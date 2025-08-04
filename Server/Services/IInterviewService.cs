using Server.Models;

namespace Server.Services
{
    public interface IInterviewService
    {
        Task<InterviewResponseDto> CreateInterviewAsync(CreateInterviewDto dto, string userId);
        Task<InterviewResponseDto?> GetInterviewByIdAsync(int id);
        Task<List<InterviewResponseDto>> GetAllInterviewsAsync();
        Task<bool> UpdateInterviewStatusAsync(int id, InterviewStatus status);
        Task<bool> DeleteInterviewAsync(int id);
    }
}