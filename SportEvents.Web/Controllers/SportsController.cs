using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Db;
using SportEvents.Web.Models.Sports;

namespace SportEvents.Web.Controllers;

public class SportsController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public SportsController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery)
    {
        var sports = await LoadSportsQuery()
            .OrderBy(item => item.title)
            .ToListAsync();

        var items = sports
            .Where(item => MatchesSearch(item, searchQuery))
            .Select(MapSportListItem)
            .ToList();

        return View(new SportCatalogViewModel
        {
            SearchQuery = searchQuery,
            CanManage = CanManageCatalog(),
            Items = items
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var sport = await LoadSportsQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sport is null)
        {
            return NotFound();
        }

        return View(MapSportDetails(sport, CanManageCatalog()));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new SportEditViewModel());
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SportEditViewModel model)
    {
        await ValidateSportAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var sport = new Sport
        {
            title = model.Title.Trim()
        };

        db.Sports.Add(sport);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = sport.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var sport = await db.Sports
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sport is null)
        {
            return NotFound();
        }

        return View(new SportEditViewModel
        {
            Id = sport.id,
            Title = sport.title
        });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SportEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateSportAsync(model, id);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var sport = await db.Sports.FirstOrDefaultAsync(item => item.id == id);
        if (sport is null)
        {
            return NotFound();
        }

        sport.title = model.Title.Trim();
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = sport.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var sport = await LoadSportsQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (sport is null)
        {
            return NotFound();
        }

        return View(MapSportDetails(sport, false));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sport = await db.Sports
            .Include(item => item.SportTypes)
            .FirstOrDefaultAsync(item => item.id == id);

        if (sport is null)
        {
            return NotFound();
        }

        if (sport.SportTypes.Count > 0)
        {
            var model = await BuildSportDetailsViewModelAsync(id, false);
            ModelState.AddModelError(string.Empty, model.DeleteBlockedReason ?? "Нельзя удалить вид спорта, пока у него есть дочерние типы.");
            return View(model);
        }

        db.Sports.Remove(sport);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<Sport> LoadSportsQuery()
    {
        return db.Sports
            .AsNoTracking()
            .Include(item => item.SportTypes)
                .ThenInclude(item => item.SportSubtypes);
    }

    private async Task ValidateSportAsync(SportEditViewModel model, int? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            return;
        }

        var title = model.Title.Trim();
        var duplicateExists = await db.Sports.AnyAsync(item =>
            item.id != currentId && item.title == title);

        if (duplicateExists)
        {
            ModelState.AddModelError(nameof(SportEditViewModel.Title), "Вид спорта с таким названием уже существует.");
        }
    }

    private bool CanManageCatalog()
    {
        return User.IsInRole(AppRoles.Administrator) || User.IsInRole(AppRoles.Organizer);
    }

    private static bool MatchesSearch(Sport sport, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return true;
        }

        return sport.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || sport.SportTypes.Any(item =>
                item.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                || item.SportSubtypes.Any(subtype => subtype.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)));
    }

    private static SportListItemViewModel MapSportListItem(Sport sport)
    {
        return new SportListItemViewModel
        {
            Id = sport.id,
            Title = sport.title,
            SportTypeCount = sport.SportTypes.Count,
            SportSubtypeCount = sport.SportTypes.Sum(item => item.SportSubtypes.Count),
            SportTypeTitles = sport.SportTypes
                .OrderBy(item => item.title)
                .Select(item => item.title)
                .Take(4)
                .ToList()
        };
    }

    private static SportDetailsViewModel MapSportDetails(Sport sport, bool canManage)
    {
        return new SportDetailsViewModel
        {
            Id = sport.id,
            Title = sport.title,
            SportTypeCount = sport.SportTypes.Count,
            SportSubtypeCount = sport.SportTypes.Sum(item => item.SportSubtypes.Count),
            CanManage = canManage,
            CanDelete = sport.SportTypes.Count == 0,
            DeleteBlockedReason = sport.SportTypes.Count == 0
                ? null
                : "Сначала удалите или перенесите все типы спорта, которые относятся к этому виду.",
            SportTypes = sport.SportTypes
                .OrderBy(item => item.title)
                .Select(item => new SportTypeSummaryViewModel
                {
                    Id = item.id,
                    Title = item.title,
                    SportSubtypeCount = item.SportSubtypes.Count,
                    SportSubtypeTitles = item.SportSubtypes
                        .OrderBy(subtype => subtype.title)
                        .Select(subtype => subtype.title)
                        .Take(5)
                        .ToList()
                })
                .ToList()
        };
    }

    private async Task<SportDetailsViewModel> BuildSportDetailsViewModelAsync(int id, bool canManage)
    {
        var sport = await LoadSportsQuery()
            .FirstAsync(item => item.id == id);

        return MapSportDetails(sport, canManage);
    }
}
