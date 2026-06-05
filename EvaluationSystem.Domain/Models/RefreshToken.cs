using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
    public class RefreshToken:BaseEntity
    {
        public string Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn == null && !IsExpired;

        // Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
