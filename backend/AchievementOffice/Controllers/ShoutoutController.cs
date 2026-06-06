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
        public async Task<ActionResult<ShoutoutResponse>> Create(CreateShoutoutRequest createDto)
        {
            // var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // if(!Guid.TryParse(userIdClaim, out var userId))
            //     return Unauthorized(new { message = "Invalid user ID in token." });

            var shoutout = await _shoutoutService.CreateAsync(createDto);

            if (!shoutout.IsSuccess)
                return BadRequest(new { message = shoutout.ErrorMessage });

            return CreatedAtAction(
                nameof(GetShoutoutById),
                new { id = shoutout.Value!.ShoutoutId },
                shoutout.Value
            );
        }

        [Authorize]
        [HttpPut("{shoutoutId:guid}")]
        public async Task<ActionResult<ShoutoutResponse>> Update(Guid shoutoutId, UpdateShoutoutRequest updateDto)
        {
            var updated = await _shoutoutService.UpdateAsync(shoutoutId, updateDto);

            if (!updated.IsSuccess)
                return updated.ErrorMessage switch
                {
                    "Shoutout not found" => NotFound(),
                    "Forbidden" => Forbid(),
                    _ => BadRequest(new { message = updated.ErrorMessage })
                };

            return Ok(updated.Value);
        }

        [Authorize]
        [HttpDelete("{shoutoutId:guid}")]
        public async Task<ActionResult> Delete(Guid shoutoutId)
        {
            var deleted = await _shoutoutService.DeleteAsync(shoutoutId);

            if (!deleted.IsSuccess)
                return deleted.ErrorMessage switch
                {
                    "Shoutout not found" => NotFound(),
                    "Forbidden" => Forbid(),
                    _ => BadRequest(new { message = deleted.ErrorMessage })
                };

            return NoContent();
        }

        [HttpGet("{shoutoutId:guid}")]
        public async Task<ActionResult<ShoutoutResponse>> GetShoutoutById(Guid shoutoutId)
        {
            var shoutout = await _shoutoutService.GetShoutoutByIdAsync(shoutoutId);

            if (!shoutout.IsSuccess)
                return NotFound(new { message = shoutout.ErrorMessage });

            return Ok(shoutout.Value);
        }


        [HttpGet]
        public async Task<ActionResult<List<ShoutoutResponse>>> GetAllShoutouts()
        {
            var shoutouts = await _shoutoutService.GetAllShoutoutsAsync();

            if (!shoutouts.IsSuccess)
                return BadRequest(new {message = shoutouts.ErrorMessage});

            return Ok(shoutouts.Value);
        }
    }
}