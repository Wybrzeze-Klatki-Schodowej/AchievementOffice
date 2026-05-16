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

        // [HttpPost]
        // public async Task<IActionResult> CreateShoutout([FromBody] CreateShoutoutRequest request)
        // {
        //     var shoutout = await _shoutoutService.CreateShoutoutAsync(request);
        //     return CreatedAtAction(nameof(GetShoutoutById), new { id = shoutout.ShoutoutId }, shoutout);
        // }

        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetShoutoutById(Guid id)
        // {
        //     var shoutout = await _shoutoutService.GetShoutoutByIdAsync(id);
        //     if (shoutout == null)
        //         return NotFound();

        //     return Ok(shoutout);
        // }
    }
}