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
            var shoutout = await _shoutoutService.CreateAsync(createDto);

            if (!shoutout.IsSuccess)
                return BadRequest(new { message = shoutout.ErrorMessage });

           /* return CreatedAtAction(
                nameof(GetShoutoutById),
                new { shoutoutId = shoutout.Value!.ShoutoutId },
                shoutout.Value
            );*/
            return Ok(shoutout.Value);
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
            var deleted = await _shoutoutService.DeleteAsync(shoutoutId);

            if (!deleted.IsSuccess)
                return deleted.ErrorMessage switch
                {
                    "Not found" => NotFound(),
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
            var result = await _shoutoutService.ReactAsync(shoutoutId, reaction);

            if (!result.IsSuccess)
                return result.ErrorMessage == "Not found" ? NotFound() : BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }

        [Authorize]
        [HttpDelete("{shoutoutId:guid}/react")]
        public async Task<ActionResult<ShoutoutResponse>> Unreact(Guid shoutoutId)
        {
            var result = await _shoutoutService.UnreactAsync(shoutoutId);

            if (!result.IsSuccess)
                return result.ErrorMessage == "Not found" ? NotFound() : BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Value);
        }
    }
}