using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollApp.Backend.DTOs;
using PollApp.Backend.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PollApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("poll/{pollId}")]
        public async Task<IActionResult> GetByPoll(Guid pollId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var comments = await _commentService.GetCommentsForPollAsync(pollId, page, pageSize);
            return Ok(comments);
        }

        [Authorize]
        [HttpPost("poll/{pollId}")]
        public async Task<IActionResult> Create(Guid pollId, [FromBody] CreateCommentDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var comment = await _commentService.AddCommentAsync(pollId, dto, userId);
            return Ok(comment);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var comment = await _commentService.UpdateCommentAsync(id, dto, userId);
            return Ok(comment);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");
            await _commentService.DeleteCommentAsync(id, userId, isAdmin);
            return NoContent();
        }
    }
}
