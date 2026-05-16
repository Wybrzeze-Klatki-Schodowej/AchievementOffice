using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AchievementOffice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // 1. Endpoint PUBLICZNY - każdy ma dostęp (bez tokena)
        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            return Ok(new { Message = "To jest publiczny endpoint. Każdy to widzi." });
        }

        // 2. Endpoint ZABEZPIECZONY - wymaga jakiegokolwiek ważnego tokena
        [Authorize]
        [HttpGet("private")]
        public IActionResult GetPrivateData()
        {
            // TUTA JEST MAGIA: Wyciągamy dane prosto z tokena, bez pytania bazy danych!
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var login = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                Message = "Gratulacje! Jesteś zalogowany.",
                YourDataFromToken = new
                {
                    Id = userId,
                    Login = login,
                    Email = email,
                    Role = role
                }
            });
        }

        // 3. Endpoint dla konkretnej ROLI (Opcjonalnie, jeśli chcesz przetestować role)
        // Jeśli w ClaimTypes.Role masz np. "Admin", to tylko on tu wejdzie
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult GetAdminData()
        {
            return Ok(new { Message = "Witaj szefie. Ten endpoint jest tylko dla Adminów." });
        }
    }
}