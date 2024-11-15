using Microsoft.AspNetCore.Mvc;

namespace Knight_duel_game.Controllers
{
    public class SwordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
