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
    public class PollsController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var polls = await _pollService.GetAllPollsAsync(search, sort, page, pageSize);
            return Ok(polls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var poll = await _pollService.GetPollByIdAsync(id);
            if (poll == null) return NotFound();
            return Ok(poll);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePollDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var poll = await _pollService.CreatePollAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = poll.Id }, poll);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePollDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");
            var poll = await _pollService.UpdatePollAsync(id, dto, userId, isAdmin);
            return Ok(poll);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");
            await _pollService.DeletePollAsync(id, userId, isAdmin);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/toggle-publish")]
        public async Task<IActionResult> TogglePublish(Guid id, [FromQuery] bool publish)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");
            await _pollService.TogglePublishResultsAsync(id, publish, userId, isAdmin);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/close")]
        public async Task<IActionResult> ClosePoll(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");
            await _pollService.ClosePollAsync(id, userId, isAdmin);
            return NoContent();
        }
    }
}
