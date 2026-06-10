using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PollApp.Backend.DbContexts;
using PollApp.Backend.DTOs;
using PollApp.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PollApp.Backend.Services
{
    public interface IPollService
    {
        Task<List<PollDto>> GetAllPollsAsync(string? search, string? sort, int page, int pageSize);
        Task<PollDto?> GetPollByIdAsync(Guid id);
        Task<PollDto> CreatePollAsync(CreatePollDto dto, Guid userId);
        Task<PollDto> UpdatePollAsync(Guid id, UpdatePollDto dto, Guid userId, bool isAdmin);
        Task DeletePollAsync(Guid id, Guid userId, bool isAdmin);
        Task TogglePublishResultsAsync(Guid id, bool publish, Guid userId, bool isAdmin);
        Task ClosePollAsync(Guid id, Guid userId, bool isAdmin);
        Task CastVoteAsync(Guid pollId, VoteDto dto, Guid userId);
    }

    public class PollService : IPollService
    {
        private readonly PollDbContext _context;
        private readonly IMapper _mapper;

        public PollService(PollDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PollDto>> GetAllPollsAsync(string? search, string? sort, int page, int pageSize)
        {
            var query = _context.Polls
                .Include(p => p.Creator)
                .Include(p => p.Options).ThenInclude(o => o.Votes)
                .Include(p => p.Comments)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
            }

            // Close polls dynamically if EndDate passed
            var now = DateTime.UtcNow;
            
            // Note: We're not updating the DB here for performance, just mapping Status dynamically 
            // but for a robust app, we might want a background worker. For now, we will return dynamic status in mapping.

            query = sort?.ToLower() switch
            {
                "oldest" => query.OrderBy(p => p.CreatedAt),
                "popular" => query.OrderByDescending(p => p.Votes.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var polls = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<PollDto>>(polls);
            foreach (var dto in dtos)
            {
                if (dto.EndDate <= now) dto.Status = "Closed";
                dto.TotalVotes = polls.First(p => p.Id == dto.Id).Votes.Count;
                dto.TotalComments = polls.First(p => p.Id == dto.Id).Comments.Count;
            }

            return dtos;
        }

        public async Task<PollDto?> GetPollByIdAsync(Guid id)
        {
            var poll = await _context.Polls
                .Include(p => p.Creator)
                .Include(p => p.Options).ThenInclude(o => o.Votes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (poll == null) return null;

            var dto = _mapper.Map<PollDto>(poll);
            if (dto.EndDate <= DateTime.UtcNow) dto.Status = "Closed";
            dto.TotalVotes = poll.Votes.Count;
            dto.TotalComments = poll.Comments.Count;
            
            return dto;
        }

        public async Task<PollDto> CreatePollAsync(CreatePollDto dto, Guid userId)
        {
            var poll = new Poll
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                VotingEnabled = dto.EnableVoting,
                CommentsEnabled = dto.EnableComments,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Active",
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                Options = dto.Options.Select(o => new PollOption
                {
                    Id = Guid.NewGuid(),
                    OptionText = o
                }).ToList()
            };

            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            return await GetPollByIdAsync(poll.Id) ?? throw new Exception("Failed to retrieve created poll.");
        }

        public async Task<PollDto> UpdatePollAsync(Guid id, UpdatePollDto dto, Guid userId, bool isAdmin)
        {
            var poll = await _context.Polls.Include(p => p.Options).Include(p => p.Votes).FirstOrDefaultAsync(p => p.Id == id);
            if (poll == null) throw new Exception("Poll not found.");
            
            if (poll.CreatedBy != userId && !isAdmin) throw new Exception("Unauthorized.");
            if (poll.Votes.Any()) throw new Exception("Cannot update options after voting has started.");

            poll.Title = dto.Title;
            poll.Description = dto.Description;
            poll.ImageUrl = dto.ImageUrl;
            poll.VotingEnabled = dto.EnableVoting;
            poll.CommentsEnabled = dto.EnableComments;
            poll.StartDate = dto.StartDate;
            poll.EndDate = dto.EndDate;

            // Simple option replacement logic since there are no votes yet
            _context.PollOptions.RemoveRange(poll.Options);
            poll.Options = dto.Options.Select(o => new PollOption
            {
                Id = Guid.NewGuid(),
                OptionText = o,
                PollId = poll.Id
            }).ToList();

            await _context.SaveChangesAsync();
            return await GetPollByIdAsync(poll.Id) ?? throw new Exception("Failed to retrieve updated poll.");
        }

        public async Task DeletePollAsync(Guid id, Guid userId, bool isAdmin)
        {
            var poll = await _context.Polls
                .Include(p => p.Votes)
                .Include(p => p.Comments)
                .Include(p => p.Options)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (poll == null) throw new Exception("Poll not found.");
            if (poll.CreatedBy != userId && !isAdmin) throw new Exception("Unauthorized.");

            // Manually delete related entities to avoid SQL Server cascade restriction issues
            _context.Votes.RemoveRange(poll.Votes);
            _context.Comments.RemoveRange(poll.Comments);
            _context.PollOptions.RemoveRange(poll.Options);
            
            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();
        }

        public async Task TogglePublishResultsAsync(Guid id, bool publish, Guid userId, bool isAdmin)
        {
            var poll = await _context.Polls.FindAsync(id);
            if (poll == null) throw new Exception("Poll not found.");
            if (poll.CreatedBy != userId && !isAdmin) throw new Exception("Unauthorized.");

            poll.ResultsPublished = publish;
            await _context.SaveChangesAsync();
        }

        public async Task ClosePollAsync(Guid id, Guid userId, bool isAdmin)
        {
            var poll = await _context.Polls.FindAsync(id);
            if (poll == null) throw new Exception("Poll not found.");
            if (poll.CreatedBy != userId && !isAdmin) throw new Exception("Unauthorized.");

            poll.Status = "Closed";
            poll.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task CastVoteAsync(Guid pollId, VoteDto dto, Guid userId)
        {
            var poll = await _context.Polls.FindAsync(pollId);
            if (poll == null) throw new Exception("Poll not found.");
            if (!poll.VotingEnabled || poll.Status == "Closed" || poll.EndDate <= DateTime.UtcNow) throw new Exception("Voting is closed for this poll.");

            var existingVote = await _context.Votes.FirstOrDefaultAsync(v => v.PollId == pollId && v.UserId == userId);
            if (existingVote != null) throw new Exception("User has already voted on this poll.");

            var vote = new Vote
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PollId = pollId,
                PollOptionId = dto.PollOptionId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();
        }
    }
}
