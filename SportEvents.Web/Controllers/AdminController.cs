using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Admin;
using SportEvents.Web.Models.Db;
using System.Security.Claims;

namespace SportEvents.Web.Controllers;

[Authorize(Roles = AppRoles.Administrator)]
public class AdminController : Controller
{
    private readonly SportsCompetitionsDbContext db;
    private readonly PasswordHasher<object> passwordHasher = new();

    public AdminController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var roles = await db.Roles
            .AsNoTracking()
            .Include(item => item.Users)
            .OrderBy(item => item.title)
            .ToListAsync();

        var users = await db.Users
            .AsNoTracking()
            .Include(item => item.idContactNavigation)
            .Include(item => item.idRoleNavigation)
            .OrderBy(item => item.idRoleNavigation!.title)
            .ThenBy(item => item.idContactNavigation!.lastname)
            .ThenBy(item => item.idContactNavigation!.firstname)
            .ToListAsync();

        var currentUserId = GetCurrentUserId();

        var model = new AdminUsersViewModel
        {
            StatusMessage = TempData["StatusMessage"] as string,
            StatusType = TempData["StatusType"] as string ?? "info",
            Roles = roles.Select(role => new RoleManagementViewModel
            {
                RoleId = role.id,
                Title = role.title,
                UserCount = role.Users.Count,
                IsSystemRole = IsSystemRole(role.title)
            }).ToList(),
            CreateRole = new RoleCreateViewModel(),
            Users = users.Select(user => new UserRoleViewModel
            {
                UserId = user.id,
                FullName = BuildFullName(user.idContactNavigation!.lastname, user.idContactNavigation.firstname, user.idContactNavigation.middlename),
                Email = user.idContactNavigation.email,
                PhotoUrl = MediaCatalog.UserPhotoOrDefault(user.idContactNavigation.photoUrl),
                RoleTitle = user.idRoleNavigation?.title ?? AppRoles.User,
                SelectedRoleId = user.idRole ?? 0,
                IsCurrentUser = currentUserId == user.id,
                AvailableRoles = BuildRoleOptions(roles, user.idRole)
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await db.Users
            .Include(item => item.idContactNavigation)
                .ThenInclude(item => item!.Participant)
            .Include(item => item.idRoleNavigation)
            .FirstOrDefaultAsync(item => item.id == id);

        if (user?.idContactNavigation is null)
        {
            return NotFound();
        }

        var contact = user.idContactNavigation;
        var model = new AdminUserEditViewModel
        {
            UserId = user.id,
            FirstName = contact.firstname,
            LastName = contact.lastname,
            MiddleName = contact.middlename,
            BirthDate = contact.birthDate,
            Sex = contact.sex,
            Phone = contact.phone ?? "+",
            Email = contact.email,
            PhotoUrl = contact.photoUrl,
            RoleId = user.idRole
        };

        await PopulateRoleOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(AdminUserEditViewModel model)
    {
        var user = await db.Users
            .Include(item => item.idContactNavigation)
                .ThenInclude(item => item!.Participant)
            .Include(item => item.idRoleNavigation)
            .FirstOrDefaultAsync(item => item.id == model.UserId);

        if (user?.idContactNavigation is null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await ValidateUserEditAsync(model, user.idContactNavigation.id);
        }

        if (!ModelState.IsValid)
        {
            await PopulateRoleOptionsAsync(model);
            return View(model);
        }

        var role = await db.Roles.FirstAsync(item => item.id == model.RoleId!.Value);
        var contact = user.idContactNavigation;

        contact.firstname = model.FirstName.Trim();
        contact.lastname = model.LastName.Trim();
        contact.middlename = string.IsNullOrWhiteSpace(model.MiddleName) ? null : model.MiddleName.Trim();
        contact.birthDate = model.BirthDate;
        contact.sex = model.Sex;
        contact.phone = model.Phone.Trim();
        contact.email = model.Email.Trim();
        contact.photoUrl = NormalizePhotoUrl(model.PhotoUrl);

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            contact.passwordHash = passwordHasher.HashPassword(null!, model.NewPassword);
        }

        user.idRole = role.id;
        user.idRoleNavigation = role;

        if (string.Equals(role.title, AppRoles.Participant, StringComparison.OrdinalIgnoreCase)
            && contact.Participant is null)
        {
            db.Participants.Add(new Participant
            {
                idContact = contact.id
            });
        }

        await db.SaveChangesAsync();

        if (IsCurrentUser(user.id))
        {
            return await SignOutWithStatusAsync("Данные вашей учётной записи обновлены. Выполните вход снова.");
        }

        SetStatus("Данные пользователя обновлены.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(int userId, int roleId)
    {
        var user = await db.Users
            .Include(item => item.idContactNavigation)
                .ThenInclude(item => item!.Participant)
            .Include(item => item.idRoleNavigation)
            .FirstOrDefaultAsync(item => item.id == userId);

        if (user is null)
        {
            return NotFound();
        }

        var role = await db.Roles.FirstOrDefaultAsync(item => item.id == roleId);
        if (role is null)
        {
            return BadRequest();
        }

        var roleChanged = user.idRole != role.id;
        user.idRole = role.id;

        if (string.Equals(role.title, AppRoles.Participant, StringComparison.OrdinalIgnoreCase)
            && user.idContactNavigation is not null
            && user.idContactNavigation.Participant is null)
        {
            db.Participants.Add(new Participant
            {
                idContact = user.idContactNavigation.id
            });
        }

        await db.SaveChangesAsync();

        if (roleChanged && IsCurrentUser(user.id))
        {
            return await SignOutWithStatusAsync("Ваша роль обновлена. Выполните вход снова.");
        }

        SetStatus("Роль пользователя обновлена.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole([Bind(Prefix = "CreateRole")] RoleCreateViewModel model)
    {
        var normalizedTitle = NormalizeRoleTitle(model.Title);
        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            SetStatus("Введите название роли.", "danger");
            return RedirectToAction(nameof(Index));
        }

        var exists = await db.Roles.AnyAsync(item => item.title.ToLower() == normalizedTitle.ToLower());
        if (exists)
        {
            SetStatus("Роль с таким названием уже существует.", "danger");
            return RedirectToAction(nameof(Index));
        }

        db.Roles.Add(new Role { title = normalizedTitle });
        await db.SaveChangesAsync();

        SetStatus("Новая роль создана.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int roleId, string title)
    {
        var role = await db.Roles.FirstOrDefaultAsync(item => item.id == roleId);
        if (role is null)
        {
            return NotFound();
        }

        if (IsSystemRole(role.title))
        {
            SetStatus("Системные роли нельзя переименовывать.", "danger");
            return RedirectToAction(nameof(Index));
        }

        var normalizedTitle = NormalizeRoleTitle(title);
        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            SetStatus("Введите название роли.", "danger");
            return RedirectToAction(nameof(Index));
        }

        var duplicateExists = await db.Roles.AnyAsync(item =>
            item.id != roleId && item.title.ToLower() == normalizedTitle.ToLower());

        if (duplicateExists)
        {
            SetStatus("Роль с таким названием уже существует.", "danger");
            return RedirectToAction(nameof(Index));
        }

        role.title = normalizedTitle;
        await db.SaveChangesAsync();

        SetStatus("Название роли обновлено.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(int roleId)
    {
        var role = await db.Roles
            .Include(item => item.Users)
            .FirstOrDefaultAsync(item => item.id == roleId);

        if (role is null)
        {
            return NotFound();
        }

        if (IsSystemRole(role.title))
        {
            SetStatus("Системные роли нельзя удалять.", "danger");
            return RedirectToAction(nameof(Index));
        }

        if (role.Users.Count > 0)
        {
            SetStatus("Нельзя удалить роль, пока она назначена пользователям.", "danger");
            return RedirectToAction(nameof(Index));
        }

        db.Roles.Remove(role);
        await db.SaveChangesAsync();

        SetStatus("Роль удалена.", "success");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetDatabase()
    {
        try
        {
            await db.Database.EnsureDeletedAsync();
            db.ChangeTracker.Clear();
            await DbInitializer.InitializeAsync(db);

            TempData["StatusMessage"] = "База данных сброшена и заново заполнена тестовыми данными. Выполните вход снова.";
            TempData["StatusType"] = "success";

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }
        catch (Exception exception)
        {
            TempData["StatusMessage"] = $"Не удалось сбросить БД: {exception.Message}";
            TempData["StatusType"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task ValidateUserEditAsync(AdminUserEditViewModel model, int contactId)
    {
        if (model.RoleId is null || !await db.Roles.AnyAsync(item => item.id == model.RoleId.Value))
        {
            ModelState.AddModelError(nameof(AdminUserEditViewModel.RoleId), "Выберите корректную роль.");
        }

        if (string.IsNullOrWhiteSpace(model.Phone) || !IsValidPhone(model.Phone))
        {
            ModelState.AddModelError(nameof(AdminUserEditViewModel.Phone), "Неверный номер телефона.");
        }

        if (!string.IsNullOrWhiteSpace(model.NewPassword) && model.NewPassword.Length < 8)
        {
            ModelState.AddModelError(nameof(AdminUserEditViewModel.NewPassword), "Пароль должен содержать не менее 8 символов.");
        }

        var normalizedEmail = model.Email.Trim();
        var emailExists = await db.Contacts.AnyAsync(item =>
            item.id != contactId && item.email.ToLower() == normalizedEmail.ToLower());

        if (emailExists)
        {
            ModelState.AddModelError(nameof(AdminUserEditViewModel.Email), "Пользователь с таким email уже существует.");
        }
    }

    private async Task PopulateRoleOptionsAsync(AdminUserEditViewModel model)
    {
        var roles = await db.Roles
            .AsNoTracking()
            .OrderBy(item => item.title)
            .ToListAsync();

        model.AvailableRoles = BuildRoleOptions(roles, model.RoleId);
    }

    private static IReadOnlyList<SelectListItem> BuildRoleOptions(IEnumerable<Role> roles, int? selectedRoleId)
    {
        return roles
            .Select(role => new SelectListItem
            {
                Value = role.id.ToString(),
                Text = role.title,
                Selected = selectedRoleId == role.id
            })
            .ToList();
    }

    private static bool IsSystemRole(string roleTitle)
    {
        return AppRoles.All.Contains(roleTitle, StringComparer.OrdinalIgnoreCase);
    }

    private static string NormalizeRoleTitle(string? title)
    {
        return title?.Trim() ?? string.Empty;
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

    private int? GetCurrentUserId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;
    }

    private bool IsCurrentUser(int userId)
    {
        return GetCurrentUserId() == userId;
    }

    private void SetStatus(string message, string type)
    {
        TempData["StatusMessage"] = message;
        TempData["StatusType"] = type;
    }

    private static string? NormalizePhotoUrl(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl.Trim();
    }

    private async Task<IActionResult> SignOutWithStatusAsync(string message)
    {
        SetStatus(message, "success");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "User");
    }

    private static string BuildFullName(string lastName, string firstName, string? middleName)
    {
        return string.Join(" ", new[] { lastName, firstName, middleName }.Where(item => !string.IsNullOrWhiteSpace(item)));
    }
}
