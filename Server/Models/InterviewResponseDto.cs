namespace Server.Models
{
    public class InterviewResponseDto
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public string InterviewerName { get; set; } = string.Empty;
        public string InterviewerEmail { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? GoogleMeetLink { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}