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
    public class VotesController : ControllerBase
    {
        private readonly IPollService _pollService;

        public VotesController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [Authorize]
        [HttpPost("{pollId}")]
        public async Task<IActionResult> CastVote(Guid pollId, [FromBody] VoteDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _pollService.CastVoteAsync(pollId, dto, userId);
            return Ok(new { message = "Vote cast successfully" });
        }
    }
}
