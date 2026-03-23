using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models;
using SportEvents.Web.Models.Db;
using System.Security.Claims;

namespace SportEvents.Web.Controllers;

public class UserController : Controller
{
    private readonly SportsCompetitionsDbContext db;
    private readonly PasswordHasher<object> passwordHasher = new();

    public UserController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.DemoAccounts = DemoAccountCatalog.All;
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.DemoAccounts = DemoAccountCatalog.All;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await db.Users
            .Include(item => item.idContactNavigation)
            .Include(item => item.idRoleNavigation)
            .FirstOrDefaultAsync(item => item.idContactNavigation != null && item.idContactNavigation.email == model.Email);

        var contact = user?.idContactNavigation;
        if (user is null || contact is null || string.IsNullOrWhiteSpace(contact.passwordHash))
        {
            ModelState.AddModelError("", "Неверный email или пароль");
            return View(model);
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(null!, contact.passwordHash, model.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Неверный email или пароль");
            return View(model);
        }

        await SignInAsync(user, contact, model.RememberMe);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("StartPage", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!IsValidPhone(model.Phone))
        {
            ModelState.AddModelError(nameof(model.Phone), "Неверный номер телефона.");
            return View(model);
        }

        if (await db.Contacts.AnyAsync(item => item.email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Пользователь с таким email уже существует.");
            return View(model);
        }

        var contact = new Contact
        {
            firstname = model.FirstName,
            lastname = model.LastName,
            middlename = model.MiddleName,
            birthDate = model.BirthDate,
            sex = model.Sex,
            phone = model.Phone,
            email = model.Email,
            photoUrl = NormalizePhotoUrl(model.PhotoUrl),
            passwordHash = passwordHasher.HashPassword(null!, model.Password)
        };

        db.Contacts.Add(contact);
        await db.SaveChangesAsync();

        var role = await GetRoleAsync(AppRoles.Participant);
        var user = new User
        {
            idContact = contact.id,
            idRole = role.id,
            idRoleNavigation = role
        };

        db.Users.Add(user);
        db.Participants.Add(new Participant { idContact = contact.id });
        await db.SaveChangesAsync();

        await SignInAsync(user, contact, false);
        return RedirectToAction("StartPage", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private bool IsValidPhone(string phone)
    {
        var phoneUtil = PhoneNumberUtil.GetInstance();

        try
        {
            var number = phoneUtil.Parse(phone, "RU");
            return phoneUtil.IsValidNumber(number);
        }
        catch
        {
            return false;
        }
    }

    private async Task<Role> GetRoleAsync(string roleTitle)
    {
        var role = await db.Roles.FirstOrDefaultAsync(item => item.title == roleTitle);
        if (role is null)
        {
            throw new InvalidOperationException($"Роль '{roleTitle}' не найдена.");
        }

        return role;
    }

    private static string? NormalizePhotoUrl(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl.Trim();
    }

    private async Task SignInAsync(User user, Contact contact, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.id.ToString()),
            new(ClaimTypes.Name, contact.email),
            new(ClaimTypes.Email, contact.email),
            new(ClaimTypes.GivenName, contact.firstname),
            new(ClaimTypes.Surname, contact.lastname),
            new("contactId", contact.id.ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.idRoleNavigation?.title))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.idRoleNavigation.title));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = isPersistent });
    }
}
