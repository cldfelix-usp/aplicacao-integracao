
namespace Server.Models;

public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => RevokedDate == null && !IsExpired;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
