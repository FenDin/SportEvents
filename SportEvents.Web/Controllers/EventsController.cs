using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Db;
using SportEvents.Web.Models.Events;

namespace SportEvents.Web.Controllers;

public class EventsController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public EventsController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery, string status = "all")
    {
        var events = await db.Events
            .AsNoTracking()
            .Include(item => item.EventsCompetitions)
                .ThenInclude(item => item.idCompetitionNavigation)
                    .ThenInclude(item => item.idSportSubTypeNavigation)
                        .ThenInclude(item => item.idSportTypeNavigation)
                            .ThenInclude(item => item.idSportNavigation)
            .AsSplitQuery()
            .OrderBy(item => item.dateStart ?? DateTime.MaxValue)
            .ToListAsync();

        var filteredEvents = events
            .Where(item => MatchesSearch(item, searchQuery))
            .Where(item => MatchesStatus(item.dateStart, item.dateEnd, status))
            .Select(MapEventListItem)
            .ToList();

        return View(new EventCatalogViewModel
        {
            SearchQuery = searchQuery,
            Status = status,
            CanManage = CanManageCatalog(),
            Items = filteredEvents
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var eventEntity = await db.Events
            .AsNoTracking()
            .Include(item => item.EventsCompetitions)
                .ThenInclude(item => item.idCompetitionNavigation)
                    .ThenInclude(item => item.idSportSubTypeNavigation)
                        .ThenInclude(item => item.idSportTypeNavigation)
                            .ThenInclude(item => item.idSportNavigation)
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (eventEntity is null)
        {
            return NotFound();
        }

        var model = new EventDetailsViewModel
        {
            Id = eventEntity.id,
            Title = eventEntity.title,
            Description = eventEntity.description,
            PhotoUrl = MediaCatalog.EventPhotoOrDefault(eventEntity.photoUrl),
            DateStart = eventEntity.dateStart,
            DateEnd = eventEntity.dateEnd,
            CanManage = CanManageCatalog(),
            Competitions = eventEntity.EventsCompetitions
                .OrderBy(item => item.idCompetitionNavigation.dateStart ?? DateTime.MaxValue)
                .Select(item => new EventCompetitionViewModel
                {
                    Id = item.idCompetitionNavigation.id,
                    Title = item.idCompetitionNavigation.title,
                    Description = item.idCompetitionNavigation.description,
                    PhotoUrl = MediaCatalog.CompetitionPhotoOrDefault(item.idCompetitionNavigation.photoUrl),
                    DateStart = item.idCompetitionNavigation.dateStart,
                    DateEnd = item.idCompetitionNavigation.dateEnd,
                    SportTitle = item.idCompetitionNavigation.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title,
                    SportTypeTitle = item.idCompetitionNavigation.idSportSubTypeNavigation.idSportTypeNavigation.title,
                    SportSubTypeTitle = item.idCompetitionNavigation.idSportSubTypeNavigation.title
                })
                .ToList()
        };

        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new EventEditViewModel
        {
            DateStart = DateTime.Today.AddDays(7).AddHours(10),
            DateEnd = DateTime.Today.AddDays(7).AddHours(18)
        });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventEditViewModel model)
    {
        ValidateDateRange(model.DateStart, model.DateEnd);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        db.Events.Add(new Event
        {
            title = model.Title,
            description = model.Description,
            dateStart = model.DateStart,
            dateEnd = model.DateEnd,
            photoUrl = NormalizePhotoUrl(model.PhotoUrl)
        });

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var eventEntity = await db.Events.FindAsync(id);
        if (eventEntity is null)
        {
            return NotFound();
        }

        return View(new EventEditViewModel
        {
            Id = eventEntity.id,
            Title = eventEntity.title,
            Description = eventEntity.description,
            DateStart = eventEntity.dateStart,
            DateEnd = eventEntity.dateEnd,
            PhotoUrl = eventEntity.photoUrl
        });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ValidateDateRange(model.DateStart, model.DateEnd);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var eventEntity = await db.Events.FindAsync(id);
        if (eventEntity is null)
        {
            return NotFound();
        }

        eventEntity.title = model.Title;
        eventEntity.description = model.Description;
        eventEntity.dateStart = model.DateStart;
        eventEntity.dateEnd = model.DateEnd;
        eventEntity.photoUrl = NormalizePhotoUrl(model.PhotoUrl);

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var eventEntity = await db.Events
            .AsNoTracking()
            .Include(item => item.EventsCompetitions)
            .FirstOrDefaultAsync(item => item.id == id);

        if (eventEntity is null)
        {
            return NotFound();
        }

        return View(new EventDetailsViewModel
        {
            Id = eventEntity.id,
            Title = eventEntity.title,
            Description = eventEntity.description,
            PhotoUrl = MediaCatalog.EventPhotoOrDefault(eventEntity.photoUrl),
            DateStart = eventEntity.dateStart,
            DateEnd = eventEntity.dateEnd,
            Competitions = eventEntity.EventsCompetitions
                .Select(item => new EventCompetitionViewModel { Id = item.idCompetition })
                .ToList()
        });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eventEntity = await db.Events
            .Include(item => item.EventsCompetitions)
            .FirstOrDefaultAsync(item => item.id == id);

        if (eventEntity is null)
        {
            return NotFound();
        }

        db.EventsCompetitions.RemoveRange(eventEntity.EventsCompetitions);
        db.Events.Remove(eventEntity);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private bool CanManageCatalog()
    {
        return User.IsInRole(AppRoles.Administrator) || User.IsInRole(AppRoles.Organizer);
    }

    private static bool MatchesSearch(Event eventEntity, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return true;
        }

        return eventEntity.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(eventEntity.description)
                && eventEntity.description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    private static bool MatchesStatus(DateTime? dateStart, DateTime? dateEnd, string status)
    {
        var today = DateTime.Today;
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "all" : status.ToLowerInvariant();

        return normalizedStatus switch
        {
            "upcoming" => !dateEnd.HasValue || dateEnd.Value.Date >= today,
            "past" => dateEnd.HasValue && dateEnd.Value.Date < today,
            _ => true
        };
    }

    private static EventListItemViewModel MapEventListItem(Event eventEntity)
    {
        return new EventListItemViewModel
        {
            Id = eventEntity.id,
            Title = eventEntity.title,
            Description = eventEntity.description,
            PhotoUrl = MediaCatalog.EventPhotoOrDefault(eventEntity.photoUrl),
            DateStart = eventEntity.dateStart,
            DateEnd = eventEntity.dateEnd,
            CompetitionCount = eventEntity.EventsCompetitions.Count,
            Sports = eventEntity.EventsCompetitions
                .Select(item => item.idCompetitionNavigation.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList()
        };
    }

    private void ValidateDateRange(DateTime? dateStart, DateTime? dateEnd)
    {
        if (dateStart.HasValue && dateEnd.HasValue && dateEnd < dateStart)
        {
            ModelState.AddModelError(nameof(EventEditViewModel.DateEnd), "Дата окончания не может быть раньше даты начала.");
        }
    }

    private static string? NormalizePhotoUrl(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl.Trim();
    }
}
