using Microsoft.AspNetCore.Mvc;

namespace Knight_duel_game.Controllers
{
    public class HorseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
