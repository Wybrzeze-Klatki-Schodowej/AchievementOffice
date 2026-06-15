using AchievementOffice.Entities;
using AchievementOffice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("api/ranking")]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUserRanking()
        {
            var users = await _rankingService.GetUserRanking();
            if (!users.IsSuccess)
                return BadRequest(new { message = users.ErrorMessage });

            return Ok(users.Value);
        }

        [Authorize]
        [HttpGet("groups")]
        public async Task<IActionResult> GetGroupRanking()
        {
            var groups = await _rankingService.GetGroupRanking();
            if (!groups.IsSuccess)
                return BadRequest(new { message = groups.ErrorMessage });

            return Ok(groups.Value);
        }
    }
}
