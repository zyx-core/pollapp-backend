using Microsoft.EntityFrameworkCore;
using PollApp.Backend.DbContexts;
using PollApp.Backend.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PollApp.Backend.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly PollDbContext _context;

        public DashboardService(PollDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var now = DateTime.UtcNow;

            var totalUsers = await _context.Users.CountAsync();
            var totalPolls = await _context.Polls.CountAsync();
            var totalVotes = await _context.Votes.CountAsync();
            var totalComments = await _context.Comments.CountAsync();

            var activePolls = await _context.Polls.CountAsync(p => p.Status == "Active" && p.EndDate > now);
            var closedPolls = await _context.Polls.CountAsync(p => p.Status == "Closed" || p.EndDate <= now);

            return new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalPolls = totalPolls,
                TotalVotes = totalVotes,
                TotalComments = totalComments,
                ActivePolls = activePolls,
                ClosedPolls = closedPolls
            };
        }
    }
}
