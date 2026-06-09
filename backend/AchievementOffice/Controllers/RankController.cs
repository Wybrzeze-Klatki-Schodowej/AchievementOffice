using AchievementOffice.Models;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("/api/ranks")]
    [Authorize]
    public class RankController : ControllerBase
    {
        private readonly IRankService _rankService;

        public RankController(IRankService rankService)
        {
            _rankService = rankService;
        }

        [HttpGet]
        public async Task<IActionResult> getAllRanks()
        {
            var result = await _rankService.GetAllRanks();
            return Ok(result.Value);
        }

        [HttpGet("{rankId}")]
        public async Task<IActionResult> getRank(Guid rankId)
        {
            var result = await _rankService.GetRank(rankId);
            if (!result.IsSuccess) return NotFound(new { Message = result.ErrorMessage });
            return Ok(result.Value);
        }

        [HttpGet("{rankId}/users")]
        public async Task<IActionResult> getAllUsersWithRank(Guid rankId)
        {
            var result = await _rankService.GetAllUsersWithRank(rankId);
            if (!result.IsSuccess) return NotFound(new { Message = result.ErrorMessage });
            return Ok(result.Value);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> createRank([FromBody] CreateRankRequest dto)
        {
            var result = await _rankService.CreateRank(dto);

            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });
            return Ok(result.Value);
        }

        [HttpDelete("{rankId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteRank(Guid rankId)
        {
            var result = await _rankService.DeleteRank(rankId);
            if (!result.IsSuccess) return BadRequest(new { message = result.ErrorMessage });
            return Ok();
        }

        [HttpPut("{rankId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateRank(Guid rankId, RankRequest dto)
        {
            var result = await _rankService.UpdateRank(rankId, dto);
            if (!result.IsSuccess) return BadRequest(new {message = result.ErrorMessage});
            return Ok(result.Value);
        }
    }
}
