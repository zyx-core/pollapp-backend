using System;
using System.Collections.Generic;

namespace PollApp.Backend.Models
{
    public class Poll
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool VotingEnabled { get; set; } = true;
        public bool CommentsEnabled { get; set; } = true;
        public bool ResultsPublished { get; set; } = false;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Active"; // "Active" or "Closed"
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? Creator { get; set; }
        public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
