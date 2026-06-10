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
    public interface ICommentService
    {
        Task<List<CommentDto>> GetCommentsForPollAsync(Guid pollId, int page, int pageSize);
        Task<CommentDto> AddCommentAsync(Guid pollId, CreateCommentDto dto, Guid userId);
        Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto dto, Guid userId);
        Task DeleteCommentAsync(Guid id, Guid userId, bool isAdmin);
    }

    public class CommentService : ICommentService
    {
        private readonly PollDbContext _context;
        private readonly IMapper _mapper;

        public CommentService(PollDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CommentDto>> GetCommentsForPollAsync(Guid pollId, int page, int pageSize)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PollId == pollId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<CommentDto> AddCommentAsync(Guid pollId, CreateCommentDto dto, Guid userId)
        {
            var poll = await _context.Polls.FindAsync(pollId);
            if (poll == null) throw new Exception("Poll not found.");
            if (!poll.CommentsEnabled) throw new Exception("Comments are disabled for this poll.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                PollId = pollId,
                UserId = userId,
                CommentText = dto.CommentText,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Load user for DTO mapping
            comment.User = await _context.Users.FindAsync(userId);

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto dto, Guid userId)
        {
            var comment = await _context.Comments.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            if (comment == null) throw new Exception("Comment not found.");
            if (comment.UserId != userId) throw new Exception("Unauthorized. You can only edit your own comments.");

            comment.CommentText = dto.CommentText;
            comment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task DeleteCommentAsync(Guid id, Guid userId, bool isAdmin)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) throw new Exception("Comment not found.");
            
            // Admin can delete any comment, user can only delete their own
            if (comment.UserId != userId && !isAdmin) throw new Exception("Unauthorized. You cannot delete this comment.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
