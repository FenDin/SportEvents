using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Core.Sports_competitions.Models;

namespace MVC.Core.Sports_competitions.Controllers
{
    public class UserController : Controller
    {
        #region Поля
        private readonly SportsCompetitionsDbContext db;

        public UserController(SportsCompetitionsDbContext db)
        {
            this.db = db;
        }
        #endregion

        #region Вход (Login)

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(RegisterViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1) найти контакт по email
            var contact = await db.Contacts.FirstOrDefaultAsync(c => c.email == model.Email);
            if (contact == null || string.IsNullOrWhiteSpace(contact.passwordHash))
            {

            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Регистрация (Register)

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            



            return RedirectToAction("Login");
        }

        #endregion

        #region CRUD
        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        #endregion
    }
}
