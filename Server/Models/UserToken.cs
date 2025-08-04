using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
    public class UserToken
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}