using System;
using System.Text.Json.Serialization;

namespace PollApp.Backend.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PollId { get; set; }
        public Guid UserId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Poll? Poll { get; set; }
        
        public User? User { get; set; }
    }
}
