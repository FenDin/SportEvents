using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Db;
using SportEvents.Web.Models.Sports;

namespace SportEvents.Web.Controllers;

public class SportTypesController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public SportTypesController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery)
    {
        var sportTypes = await LoadSportTypesQuery()
            .OrderBy(item => item.idSportNavigation.title)
            .ThenBy(item => item.title)
            .ToListAsync();

        var items = sportTypes
            .Where(item => MatchesSearch(item, searchQuery))
            .Select(MapSportTypeListItem)
            .ToList();

        return View(new SportTypeCatalogViewModel
        {
            SearchQuery = searchQuery,
            CanManage = CanManageCatalog(),
            Items = items
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var sportType = await LoadSportTypesQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportType is null)
        {
            return NotFound();
        }

        return View(MapSportTypeDetails(sportType, CanManageCatalog()));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new SportTypeEditViewModel();
        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SportTypeEditViewModel model)
    {
        await ValidateSportTypeAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var sportType = new SportType
        {
            idSport = model.SportId!.Value,
            title = model.Title.Trim()
        };

        db.SportTypes.Add(sportType);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = sportType.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sportType = await db.SportTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportType is null)
        {
            return NotFound();
        }

        var model = new SportTypeEditViewModel
        {
            Id = sportType.id,
            SportId = sportType.idSport,
            Title = sportType.title
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SportTypeEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateSportTypeAsync(model, id);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var sportType = await db.SportTypes.FirstOrDefaultAsync(item => item.id == id);
        if (sportType is null)
        {
            return NotFound();
        }

        sportType.idSport = model.SportId!.Value;
        sportType.title = model.Title.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = sportType.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var sportType = await LoadSportTypesQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportType is null)
        {
            return NotFound();
        }

        return View(MapSportTypeDetails(sportType, false));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sportType = await db.SportTypes
            .Include(item => item.SportSubtypes)
            .Include(item => item.idSportNavigation)
            .FirstOrDefaultAsync(item => item.id == id);

        if (sportType is null)
        {
            return NotFound();
        }

        if (sportType.SportSubtypes.Count > 0)
        {
            var model = await BuildSportTypeDetailsViewModelAsync(id, false);
            ModelState.AddModelError(string.Empty, model.DeleteBlockedReason ?? "Нельзя удалить тип спорта, пока у него есть дисциплины.");
            return View(model);
        }

        db.SportTypes.Remove(sportType);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<SportType> LoadSportTypesQuery()
    {
        return db.SportTypes
            .AsNoTracking()
            .Include(item => item.idSportNavigation)
            .Include(item => item.SportSubtypes)
                .ThenInclude(item => item.Competitions)
            .Include(item => item.SportSubtypes)
                .ThenInclude(item => item.SchoolsSportsSubTypes);
    }

    private async Task PopulateOptionsAsync(SportTypeEditViewModel model)
    {
        var sports = await db.Sports
            .AsNoTracking()
            .OrderBy(item => item.title)
            .ToListAsync();

        model.SportOptions = sports
            .Select(item => new SelectListItem
            {
                Value = item.id.ToString(),
                Text = item.title,
                Selected = model.SportId == item.id
            })
            .ToList();
    }

    private async Task ValidateSportTypeAsync(SportTypeEditViewModel model, int? currentId = null)
    {
        if (!model.SportId.HasValue || string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        var title = model.Title.Trim();
        var duplicateExists = await db.SportTypes.AnyAsync(item =>
            item.id != currentId
            && item.idSport == model.SportId.Value
            && item.title == title);

        if (duplicateExists)
        {
            ModelState.AddModelError(nameof(SportTypeEditViewModel.Title), "В этом виде спорта уже есть тип с таким названием.");
        }
    }

    private bool CanManageCatalog()
    {
        return User.IsInRole(AppRoles.Administrator) || User.IsInRole(AppRoles.Organizer);
    }

    private static bool MatchesSearch(SportType sportType, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return true;
        }

        return sportType.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || sportType.idSportNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || sportType.SportSubtypes.Any(item => item.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    private static SportTypeListItemViewModel MapSportTypeListItem(SportType sportType)
    {
        return new SportTypeListItemViewModel
        {
            Id = sportType.id,
            Title = sportType.title,
            SportId = sportType.idSport,
            SportTitle = sportType.idSportNavigation.title,
            SportSubtypeCount = sportType.SportSubtypes.Count,
            SportSubtypeTitles = sportType.SportSubtypes
                .OrderBy(item => item.title)
                .Select(item => item.title)
                .Take(5)
                .ToList()
        };
    }

    private static SportTypeDetailsViewModel MapSportTypeDetails(SportType sportType, bool canManage)
    {
        return new SportTypeDetailsViewModel
        {
            Id = sportType.id,
            Title = sportType.title,
            SportId = sportType.idSport,
            SportTitle = sportType.idSportNavigation.title,
            SportSubtypeCount = sportType.SportSubtypes.Count,
            CanManage = canManage,
            CanDelete = sportType.SportSubtypes.Count == 0,
            DeleteBlockedReason = sportType.SportSubtypes.Count == 0
                ? null
                : "Сначала удалите или перенесите все дисциплины, которые относятся к этому типу.",
            SportSubtypes = sportType.SportSubtypes
                .OrderBy(item => item.title)
                .Select(item => new SportSubtypeSummaryViewModel
                {
                    Id = item.id,
                    Title = item.title,
                    CompetitionCount = item.Competitions.Count,
                    SchoolCount = item.SchoolsSportsSubTypes.Count
                })
                .ToList()
        };
    }

    private async Task<SportTypeDetailsViewModel> BuildSportTypeDetailsViewModelAsync(int id, bool canManage)
    {
        var sportType = await LoadSportTypesQuery()
            .FirstAsync(item => item.id == id);

        return MapSportTypeDetails(sportType, canManage);
    }
}
