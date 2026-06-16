using AchievementOffice.Services;
using Microsoft.AspNetCore.Mvc;

namespace AchievementOffice.Controllers
{
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }
    }
}
