using System;
using System.Collections.Generic;

namespace PollApp.Backend.DTOs
{
    public class PollDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool VotingEnabled { get; set; }
        public bool CommentsEnabled { get; set; }
        public bool ResultsPublished { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public int TotalComments { get; set; }
        public UserDto Creator { get; set; } = null!;
        public List<PollOptionDto> Options { get; set; } = new List<PollOptionDto>();
    }

    public class CreatePollDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool EnableVoting { get; set; } = true;
        public bool EnableComments { get; set; } = true;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }

    public class UpdatePollDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool EnableVoting { get; set; }
        public bool EnableComments { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }

    public class PollOptionDto
    {
        public Guid Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int VotesCount { get; set; }
    }

    public class VoteDto
    {
        public Guid PollOptionId { get; set; }
    }
}
