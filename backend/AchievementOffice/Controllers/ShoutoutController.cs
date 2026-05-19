using AchievementOffice.Models;
using AchievementOffice.Services;
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

        [HttpPost]
        public async Task<ActionResult<ShoutoutResponseDto>> Create(CreateShoutoutDto createDto)
        {
            var shoutout = await _shoutoutService.CreateAsync(createDto);

            return CreatedAtAction(
                nameof(GetShoutoutById),
                new { id = shoutout.ShoutoutId },
                shoutout
            );
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