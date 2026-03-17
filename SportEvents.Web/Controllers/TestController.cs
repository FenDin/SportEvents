using Microsoft.AspNetCore.Mvc;

namespace SportEvents.Web.Controllers
{
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Controls()
        {
            return View();
        }


    }
}