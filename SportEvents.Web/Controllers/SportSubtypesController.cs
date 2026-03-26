using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Db;
using SportEvents.Web.Models.Sports;

namespace SportEvents.Web.Controllers;

public class SportSubtypesController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public SportSubtypesController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery)
    {
        var sportSubtypes = await LoadSportSubtypesQuery()
            .OrderBy(item => item.idSportTypeNavigation.idSportNavigation.title)
            .ThenBy(item => item.idSportTypeNavigation.title)
            .ThenBy(item => item.title)
            .ToListAsync();

        var items = sportSubtypes
            .Where(item => MatchesSearch(item, searchQuery))
            .Select(MapSportSubtypeListItem)
            .ToList();

        return View(new SportSubtypeCatalogViewModel
        {
            SearchQuery = searchQuery,
            CanManage = CanManageCatalog(),
            Items = items
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var sportSubtype = await LoadSportSubtypesQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportSubtype is null)
        {
            return NotFound();
        }

        return View(MapSportSubtypeDetails(sportSubtype, CanManageCatalog()));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new SportSubtypeEditViewModel();
        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SportSubtypeEditViewModel model)
    {
        await ValidateSportSubtypeAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var sportSubtype = new SportSubtype
        {
            idSportType = model.SportTypeId!.Value,
            title = model.Title.Trim()
        };

        db.SportSubtypes.Add(sportSubtype);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = sportSubtype.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sportSubtype = await db.SportSubtypes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportSubtype is null)
        {
            return NotFound();
        }

        var model = new SportSubtypeEditViewModel
        {
            Id = sportSubtype.id,
            SportTypeId = sportSubtype.idSportType,
            Title = sportSubtype.title
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SportSubtypeEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateSportSubtypeAsync(model, id);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var sportSubtype = await db.SportSubtypes.FirstOrDefaultAsync(item => item.id == id);
        if (sportSubtype is null)
        {
            return NotFound();
        }

        sportSubtype.idSportType = model.SportTypeId!.Value;
        sportSubtype.title = model.Title.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = sportSubtype.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var sportSubtype = await LoadSportSubtypesQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportSubtype is null)
        {
            return NotFound();
        }

        return View(MapSportSubtypeDetails(sportSubtype, false));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sportSubtype = await db.SportSubtypes
            .Include(item => item.idSportTypeNavigation)
                .ThenInclude(item => item.idSportNavigation)
            .Include(item => item.Competitions)
            .Include(item => item.SchoolsSportsSubTypes)
                .ThenInclude(item => item.idSchoolNavigation)
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportSubtype is null)
        {
            return NotFound();
        }

        if (sportSubtype.Competitions.Count > 0 || sportSubtype.SchoolsSportsSubTypes.Count > 0)
        {
            var model = await BuildSportSubtypeDetailsViewModelAsync(id, false);
            ModelState.AddModelError(string.Empty, model.DeleteBlockedReason ?? "Нельзя удалить дисциплину, пока она используется в системе.");
            return View(model);
        }

        db.SportSubtypes.Remove(sportSubtype);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<SportSubtype> LoadSportSubtypesQuery()
    {
        return db.SportSubtypes
            .AsNoTracking()
            .Include(item => item.idSportTypeNavigation)
                .ThenInclude(item => item.idSportNavigation)
            .Include(item => item.Competitions)
            .Include(item => item.SchoolsSportsSubTypes)
                .ThenInclude(item => item.idSchoolNavigation);
    }

    private async Task PopulateOptionsAsync(SportSubtypeEditViewModel model)
    {
        var sportTypes = await db.SportTypes
            .AsNoTracking()
            .Include(item => item.idSportNavigation)
            .OrderBy(item => item.idSportNavigation.title)
            .ThenBy(item => item.title)
            .ToListAsync();

        model.SportTypeOptions = sportTypes
            .Select(item => new SelectListItem
            {
                Value = item.id.ToString(),
                Text = $"{item.idSportNavigation.title} / {item.title}",
                Selected = model.SportTypeId == item.id
            })
            .ToList();
    }

    private async Task ValidateSportSubtypeAsync(SportSubtypeEditViewModel model, int? currentId = null)
    {
        if (!model.SportTypeId.HasValue || string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        var title = model.Title.Trim();
        var duplicateExists = await db.SportSubtypes.AnyAsync(item =>
            item.id != currentId
            && item.idSportType == model.SportTypeId.Value
            && item.title == title);

        if (duplicateExists)
        {
            ModelState.AddModelError(nameof(SportSubtypeEditViewModel.Title), "У выбранного типа спорта уже есть дисциплина с таким названием.");
        }
    }

    private bool CanManageCatalog()
    {
        return User.IsInRole(AppRoles.Administrator) || User.IsInRole(AppRoles.Organizer);
    }

    private static bool MatchesSearch(SportSubtype sportSubtype, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return true;
        }

        return sportSubtype.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || sportSubtype.idSportTypeNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || sportSubtype.idSportTypeNavigation.idSportNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);
    }

    private static SportSubtypeListItemViewModel MapSportSubtypeListItem(SportSubtype sportSubtype)
    {
        return new SportSubtypeListItemViewModel
        {
            Id = sportSubtype.id,
            Title = sportSubtype.title,
            SportTypeId = sportSubtype.idSportType,
            SportTypeTitle = sportSubtype.idSportTypeNavigation.title,
            SportId = sportSubtype.idSportTypeNavigation.idSport,
            SportTitle = sportSubtype.idSportTypeNavigation.idSportNavigation.title,
            CompetitionCount = sportSubtype.Competitions.Count,
            SchoolCount = sportSubtype.SchoolsSportsSubTypes.Count
        };
    }

    private static SportSubtypeDetailsViewModel MapSportSubtypeDetails(SportSubtype sportSubtype, bool canManage)
    {
        return new SportSubtypeDetailsViewModel
        {
            Id = sportSubtype.id,
            Title = sportSubtype.title,
            SportTypeId = sportSubtype.idSportType,
            SportTypeTitle = sportSubtype.idSportTypeNavigation.title,
            SportId = sportSubtype.idSportTypeNavigation.idSport,
            SportTitle = sportSubtype.idSportTypeNavigation.idSportNavigation.title,
            CompetitionCount = sportSubtype.Competitions.Count,
            SchoolCount = sportSubtype.SchoolsSportsSubTypes.Count,
            CanManage = canManage,
            CanDelete = sportSubtype.Competitions.Count == 0 && sportSubtype.SchoolsSportsSubTypes.Count == 0,
            DeleteBlockedReason = sportSubtype.Competitions.Count == 0 && sportSubtype.SchoolsSportsSubTypes.Count == 0
                ? null
                : "Сначала отвяжите дисциплину от соревнований и школ, где она используется.",
            CompetitionTitles = sportSubtype.Competitions
                .OrderBy(item => item.title)
                .Select(item => item.title)
                .ToList(),
            SchoolTitles = sportSubtype.SchoolsSportsSubTypes
                .Select(item => item.idSchoolNavigation.title)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList()
        };
    }

    private async Task<SportSubtypeDetailsViewModel> BuildSportSubtypeDetailsViewModelAsync(int id, bool canManage)
    {
        var sportSubtype = await LoadSportSubtypesQuery()
            .FirstAsync(item => item.id == id);

        return MapSportSubtypeDetails(sportSubtype, canManage);
    }
}
