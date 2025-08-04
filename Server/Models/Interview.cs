using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class Interview
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CandidateName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string CandidateEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string InterviewerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string InterviewerEmail { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? GoogleMeetLink { get; set; }
        public string? CalendarEventId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    }

    public enum InterviewStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}