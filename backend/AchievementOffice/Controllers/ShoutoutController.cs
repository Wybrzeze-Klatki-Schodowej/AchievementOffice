using System.Security.Claims;
using AchievementOffice.Entities;
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
        private readonly IRankingService _rankingService;

        public ShoutoutController(IShoutoutService shoutoutService, IRankingService rankingService)
        {
            _shoutoutService = shoutoutService;
            _rankingService = rankingService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ShoutoutResponse>> Create(CreateShoutoutRequest createDto)
        {
            var shoutout = await _shoutoutService.CreateAsync(createDto);

            if (!shoutout.IsSuccess)
                return BadRequest(new { message = shoutout.ErrorMessage });

            var shoutoutValue = shoutout.Value;

            await _rankingService.ApplyShoutOutPointsCreate(shoutoutValue!.SenderId, shoutoutValue!.ReceiverId, null, true);

            return CreatedAtAction(
                nameof(GetShoutoutById),
                new { shoutoutId = shoutout.Value!.ShoutoutId },
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
                    "Not found" => NotFound(),
                    "Forbidden" => Forbid(),
                    _ => BadRequest(new { message = updated.ErrorMessage })
                };

            return Ok(updated.Value);
        }

        [Authorize]
        [HttpDelete("{shoutoutId:guid}")]
        public async Task<ActionResult> Delete(Guid shoutoutId)
        {
            var shoutout = await _shoutoutService.GetShoutoutByIdAsync(shoutoutId);
            if (!shoutout.IsSuccess)
                return NotFound(new { message = shoutout.ErrorMessage });

            var deleted = await _shoutoutService.DeleteAsync(shoutoutId);

            if (!deleted.IsSuccess)
                return deleted.ErrorMessage switch
                {
                    "Not found" => NotFound(),
                    "Forbidden" => Forbid(),
                    _ => BadRequest(new { message = deleted.ErrorMessage })
                };

            await _rankingService.ApplyShoutOutPointsCreate(shoutout.Value!.SenderId, shoutout.Value!.ReceiverId, true, null);
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
        public async Task<ActionResult<List<ShoutoutResponse>>> GetAll()
        {
            var shoutouts = await _shoutoutService.GetAllShoutoutsAsync();

            if (!shoutouts.IsSuccess)
                return BadRequest(new {message = shoutouts.ErrorMessage});

            return Ok(shoutouts.Value);
        }

        [Authorize]
        [HttpPost("{shoutoutId:guid}/react")]
        public async Task<ActionResult<ShoutoutResponse>> React(Guid shoutoutId, [FromBody] AchievementOffice.Entities.ReactionType reaction)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message= "Invalid user ID in token." });

            var result = await _shoutoutService.ReactAsync(shoutoutId, reaction);

            if (!result.IsSuccess)
                return result.ErrorMessage == "Not found" ? NotFound() : BadRequest(new { message = result.ErrorMessage });

            bool? newState = result.Value!.CurrentUserReaction != null ? true : null;
            await _rankingService.ApplyShoutOutPoints(userId, result.Value!.ReceiverId, result.Value!.PrevReactionState, newState);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpDelete("{shoutoutId:guid}/react")]
        public async Task<ActionResult<ShoutoutResponse>> Unreact(Guid shoutoutId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid user ID in token." });

            var result = await _shoutoutService.UnreactAsync(shoutoutId);

            if (!result.IsSuccess)
                return result.ErrorMessage == "Not found" ? NotFound() : BadRequest(new { message = result.ErrorMessage });

            await _rankingService.ApplyShoutOutPoints(userId, result.Value!.ReceiverId, result.Value!.PrevReactionState, null);

            return Ok(result.Value);
        }

        [HttpGet("receiver/{userId:guid}")]
        public async Task<ActionResult<List<ShoutoutResponse>>> GetReceivedShoutouts(Guid userId)
        {
            var shoutouts = await _shoutoutService.GetReceivedShoutoutsAsync(userId);

            if (!shoutouts.IsSuccess)
            {
                return BadRequest(new
                {
                    message = shoutouts.ErrorMessage
                });
            }

            return Ok(shoutouts.Value);
        }
    }
}