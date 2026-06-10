using System;

namespace PollApp.Backend.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class CreateCommentDto
    {
        public string CommentText { get; set; } = string.Empty;
    }

    public class UpdateCommentDto
    {
        public string CommentText { get; set; } = string.Empty;
    }

    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalPolls { get; set; }
        public int TotalVotes { get; set; }
        public int TotalComments { get; set; }
        public int ActivePolls { get; set; }
        public int ClosedPolls { get; set; }
    }
}
