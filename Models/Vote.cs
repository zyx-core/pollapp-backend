using System;
using System.Text.Json.Serialization;

namespace PollApp.Backend.Models
{
    public class Vote
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PollId { get; set; }
        public Guid PollOptionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        public User? User { get; set; }
        
        [JsonIgnore]
        public Poll? Poll { get; set; }
        
        [JsonIgnore]
        public PollOption? PollOption { get; set; }
    }
}
