using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC.Core.Sports_competitions.Models;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Db;
using BCrypt.Net;
using PhoneNumbers;

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

            // Проверка номера телефона
            if (!IsValidPhone(model.Phone))
            {
                ModelState.AddModelError(nameof(model.Phone), "Неверный номер телефона.");
                return View(model);
            }


            if (await db.Contacts.AnyAsync(c=> c.email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Пользователь с таким email уже существует.");
                return View(model);
            }
            
            // хэшируем пароль
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            //Bcrypt превращает пароль в безопасный хеш

            // 1) создаём Contact
            var contact = new Contact
            {
                firstname = model.FirstName,
                lastname = model.LastName,
                middlename = model.MiddleName,
                birthDate = model.BirthDate,
                sex = model.Sex,
                phone = model.Phone,
                email = model.Email,
                passwordHash = passwordHash
            };

            db.Contacts.Add(contact);
            await db.SaveChangesAsync();

            // 2) создаём User: роль можно поставить "Пользователь" (или "Участник" — как решишь)
            // ищем роль "Пользователь"
            var role = await db.Roles.FirstOrDefaultAsync(r => r.title == "Пользователь");

            var user = new User
            {
                idContact = contact.id,
                idRole = role.id
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            // пока просто редирект на вход
            return RedirectToAction("Login");
        }

        private bool IsValidPhone(string phone)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var number = phoneUtil.Parse(phone, null);
                return phoneUtil.IsValidNumber(number);
            }
            catch
            {
                return false;
            }
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
