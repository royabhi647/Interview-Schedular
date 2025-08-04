using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class CreateInterviewDto
    {
        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string CandidateName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CandidateEmail { get; set; } = string.Empty;

        [Required]
        public string InterviewerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string InterviewerEmail { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}