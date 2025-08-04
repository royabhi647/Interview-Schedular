using Server.Data;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly ServerDataContext _context;
        private readonly IGoogleCalendarService _googleCalendarService;
        private readonly IEmailService _emailService;

        public InterviewService(
            ServerDataContext context,
            IGoogleCalendarService googleCalendarService,
            IEmailService emailService)
        {
            _context = context;
            _googleCalendarService = googleCalendarService;
            _emailService = emailService;
        }

        public async Task<InterviewResponseDto> CreateInterviewAsync(CreateInterviewDto dto, string userId)
        {
            // Validate interview time
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time");

            if (dto.StartTime <= DateTime.UtcNow)
                throw new ArgumentException("Interview must be scheduled for future time");

            // Get valid Google token
            var userToken = await _googleCalendarService.GetValidTokenAsync(userId);
            if (userToken == null)
                throw new UnauthorizedAccessException("Valid Google authentication required");

            // Create interview entity
            var interview = new Interview
            {
                JobTitle = dto.JobTitle,
                CandidateName = dto.CandidateName,
                CandidateEmail = dto.CandidateEmail,
                InterviewerName = dto.InterviewerName,
                InterviewerEmail = dto.InterviewerEmail,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = InterviewStatus.Scheduled
            };

            try
            {
                // Create Google Calendar event
                var meetLink = await _googleCalendarService.CreateCalendarEventAsync(userToken.AccessToken, dto);
                interview.GoogleMeetLink = meetLink;

                // Save to database
                _context.Interviews.Add(interview);
                await _context.SaveChangesAsync();

                // Send email notifications
                await _emailService.SendInterviewNotificationAsync(interview);

                return MapToResponseDto(interview);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create interview: {ex.Message}", ex);
            }
        }

        public async Task<InterviewResponseDto?> GetInterviewByIdAsync(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            return interview != null ? MapToResponseDto(interview) : null;
        }

        public async Task<List<InterviewResponseDto>> GetAllInterviewsAsync()
        {
            var interviews = await _context.Interviews
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return interviews.Select(MapToResponseDto).ToList();
        }

        public async Task<bool> UpdateInterviewStatusAsync(int id, InterviewStatus status)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                return false;

            interview.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInterviewAsync(int id)
        {
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
                return false;

            _context.Interviews.Remove(interview);
            await _context.SaveChangesAsync();
            return true;
        }

        private static InterviewResponseDto MapToResponseDto(Interview interview)
        {
            return new InterviewResponseDto
            {
                Id = interview.Id,
                JobTitle = interview.JobTitle,
                CandidateName = interview.CandidateName,
                CandidateEmail = interview.CandidateEmail,
                InterviewerName = interview.InterviewerName,
                InterviewerEmail = interview.InterviewerEmail,
                StartTime = interview.StartTime,
                EndTime = interview.EndTime,
                GoogleMeetLink = interview.GoogleMeetLink,
                Status = interview.Status.ToString(),
                CreatedAt = interview.CreatedAt
            };
        }
    }
}