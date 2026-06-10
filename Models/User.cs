using System;

namespace PollApp.Backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? ProfileImage { get; set; }
        public string? TeamName { get; set; }
        public bool IsActive { get; set; } = true;
        public string Role { get; set; } = "User"; // "Admin" or "User"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Refresh Token properties
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
