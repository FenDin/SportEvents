using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Admin;
using SportEvents.Web.Models.Db;

namespace SportEvents.Web.Controllers;

[Authorize(Roles = AppRoles.Administrator)]
public class AdminController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public AdminController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var roles = await db.Roles
            .AsNoTracking()
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

        var model = new AdminUsersViewModel
        {
            Users = users.Select(user => new UserRoleViewModel
            {
                UserId = user.id,
                FullName = BuildFullName(user.idContactNavigation!.lastname, user.idContactNavigation.firstname, user.idContactNavigation.middlename),
                Email = user.idContactNavigation.email,
                RoleTitle = user.idRoleNavigation?.title ?? AppRoles.User,
                SelectedRoleId = user.idRole ?? 0,
                AvailableRoles = BuildRoleOptions(roles, user.idRole)
            }).ToList()
        };

        return View(model);
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
        return RedirectToAction(nameof(Index));
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

    private static string BuildFullName(string lastName, string firstName, string? middleName)
    {
        return string.Join(" ", new[] { lastName, firstName, middleName }.Where(item => !string.IsNullOrWhiteSpace(item)));
    }
}
