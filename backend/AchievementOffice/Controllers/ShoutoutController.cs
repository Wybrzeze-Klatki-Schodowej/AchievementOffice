using System.Security.Claims;
using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("api/shoutouts")]
    public class ShoutoutController : ControllerBase
    {
        private readonly IShoutoutService _shoutoutService;

        public ShoutoutController(IShoutoutService shoutoutService)
        {
            _shoutoutService = shoutoutService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ShoutoutResponseDto>> Create(CreateShoutoutDto createDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user ID in token." });

            var shoutout = await _shoutoutService.CreateAsync(createDto, userId);

            // return CreatedAtAction(
            //     nameof(GetShoutoutById),
            //     new { id = shoutout.ShoutoutId },
            //     shoutout
            // );

            return Ok(shoutout);
        }

        [HttpPut("{shoutoutId:guid}")]
        public async Task<ActionResult<ShoutoutResponseDto>> Update(Guid shoutoutId, UpdateShoutoutDto updateDto)
        {
            var shoutout = await _shoutoutService.UpdateAsync(shoutoutId, updateDto);

            if (shoutout == null)
                return NotFound();

            return Ok(shoutout);
        }


        [HttpDelete("{shoutoutId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid shoutoutId)
        {
            var deleted = await _shoutoutService.DeleteAsync(shoutoutId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{shoutoutId:guid}")]
        public async Task<ActionResult<ShoutoutResponseDto>> GetShoutoutById([FromRoute] Guid shoutoutId)
        {
            var shoutout = await _shoutoutService.GetShoutoutByIdAsync(shoutoutId);

            if (shoutout == null)
                return NotFound();

            return Ok(shoutout);
        }


        [HttpGet]
        public async Task<ActionResult<List<ShoutoutResponseDto>>> GetAllShoutouts()
        {
            var shoutouts = await _shoutoutService.GetAllShoutoutsAsync();

            return Ok(shoutouts);
        }
    }
}