using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Competitions;
using SportEvents.Web.Models.Db;
using System.Security.Claims;

namespace SportEvents.Web.Controllers;

public class CompetitionsController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public CompetitionsController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery, string status = "all")
    {
        var competitions = await LoadCompetitionsQuery()
            .OrderBy(item => item.dateStart ?? DateTime.MaxValue)
            .AsSplitQuery()
            .ToListAsync();

        var items = competitions
            .Where(item => MatchesSearch(item, searchQuery))
            .Where(item => MatchesStatus(item.dateStart, item.dateEnd, status))
            .Select(item => new CompetitionListItemViewModel
            {
                Id = item.id,
                Title = item.title,
                Description = item.description,
                PhotoUrl = MediaCatalog.CompetitionPhotoOrDefault(item.photoUrl),
                DateStart = item.dateStart,
                DateEnd = item.dateEnd,
                SportTitle = item.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title,
                SportTypeTitle = item.idSportSubTypeNavigation.idSportTypeNavigation.title,
                SportSubTypeTitle = item.idSportSubTypeNavigation.title,
                ParticipantCount = item.ParticipantsCompetitions.Count,
                Events = item.EventsCompetitions
                    .Select(link => link.idEventNavigation.title)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(title => title)
                    .ToList()
            })
            .ToList();

        return View(new CompetitionCatalogViewModel
        {
            SearchQuery = searchQuery,
            Status = status,
            CanManage = CanManageCatalog(),
            Items = items
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var competition = await LoadCompetitionsQuery()
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        var participant = await GetCurrentParticipantAsync();

        return View(new CompetitionDetailsViewModel
        {
            Id = competition.id,
            Title = competition.title,
            Description = competition.description,
            PhotoUrl = MediaCatalog.CompetitionPhotoOrDefault(competition.photoUrl),
            DateStart = competition.dateStart,
            DateEnd = competition.dateEnd,
            SportTitle = competition.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title,
            SportTypeTitle = competition.idSportSubTypeNavigation.idSportTypeNavigation.title,
            SportSubTypeTitle = competition.idSportSubTypeNavigation.title,
            ParticipantCount = competition.ParticipantsCompetitions.Count,
            CanManage = CanManageCatalog(),
            CanEnroll = User.IsInRole(AppRoles.Participant) && (!competition.dateEnd.HasValue || competition.dateEnd.Value.Date >= DateTime.Today),
            IsUserEnrolled = participant is not null && competition.ParticipantsCompetitions.Any(item => item.idParticipant == participant.id),
            Events = competition.EventsCompetitions
                .Select(link => new CompetitionEventLinkViewModel
                {
                    Id = link.idEventNavigation.id,
                    Title = link.idEventNavigation.title,
                    PhotoUrl = MediaCatalog.EventPhotoOrDefault(link.idEventNavigation.photoUrl),
                    DateStart = link.idEventNavigation.dateStart,
                    DateEnd = link.idEventNavigation.dateEnd
                })
                .OrderBy(item => item.DateStart ?? DateTime.MaxValue)
                .ToList()
        });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CompetitionEditViewModel
        {
            DateStart = DateTime.Today.AddDays(7).AddHours(10),
            DateEnd = DateTime.Today.AddDays(7).AddHours(14)
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompetitionEditViewModel model)
    {
        ValidateCompetition(model);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var competition = new Competition
        {
            title = model.Title,
            description = model.Description,
            dateStart = model.DateStart,
            dateEnd = model.DateEnd,
            photoUrl = NormalizePhotoUrl(model.PhotoUrl),
            idSportSubType = model.SportSubTypeId!.Value
        };

        db.Competitions.Add(competition);
        await db.SaveChangesAsync();

        db.EventsCompetitions.Add(new EventsCompetition
        {
            idCompetition = competition.id,
            idEvent = model.EventId!.Value
        });

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = competition.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var competition = await db.Competitions
            .AsNoTracking()
            .Include(item => item.EventsCompetitions)
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        var model = new CompetitionEditViewModel
        {
            Id = competition.id,
            Title = competition.title,
            Description = competition.description,
            DateStart = competition.dateStart,
            DateEnd = competition.dateEnd,
            PhotoUrl = competition.photoUrl,
            SportSubTypeId = competition.idSportSubType,
            EventId = competition.EventsCompetitions.OrderBy(link => link.id).Select(link => (int?)link.idEvent).FirstOrDefault()
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CompetitionEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        ValidateCompetition(model);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var competition = await db.Competitions
            .Include(item => item.EventsCompetitions)
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        competition.title = model.Title;
        competition.description = model.Description;
        competition.dateStart = model.DateStart;
        competition.dateEnd = model.DateEnd;
        competition.photoUrl = NormalizePhotoUrl(model.PhotoUrl);
        competition.idSportSubType = model.SportSubTypeId!.Value;

        db.EventsCompetitions.RemoveRange(competition.EventsCompetitions.Where(link => link.idEvent != model.EventId));
        if (!competition.EventsCompetitions.Any(link => link.idEvent == model.EventId))
        {
            db.EventsCompetitions.Add(new EventsCompetition
            {
                idCompetition = competition.id,
                idEvent = model.EventId!.Value
            });
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = competition.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var competition = await LoadCompetitionsQuery()
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        return View(new CompetitionDetailsViewModel
        {
            Id = competition.id,
            Title = competition.title,
            Description = competition.description,
            PhotoUrl = MediaCatalog.CompetitionPhotoOrDefault(competition.photoUrl),
            DateStart = competition.dateStart,
            DateEnd = competition.dateEnd,
            SportTitle = competition.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title,
            SportTypeTitle = competition.idSportSubTypeNavigation.idSportTypeNavigation.title,
            SportSubTypeTitle = competition.idSportSubTypeNavigation.title,
            ParticipantCount = competition.ParticipantsCompetitions.Count,
            Events = competition.EventsCompetitions
                .Select(link => new CompetitionEventLinkViewModel
                {
                    Id = link.idEventNavigation.id,
                    Title = link.idEventNavigation.title,
                    PhotoUrl = MediaCatalog.EventPhotoOrDefault(link.idEventNavigation.photoUrl),
                    DateStart = link.idEventNavigation.dateStart,
                    DateEnd = link.idEventNavigation.dateEnd
                })
                .ToList()
        });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var competition = await db.Competitions
            .Include(item => item.EventsCompetitions)
            .Include(item => item.ParticipantsCompetitions)
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        db.ParticipantsCompetitions.RemoveRange(competition.ParticipantsCompetitions);
        db.EventsCompetitions.RemoveRange(competition.EventsCompetitions);
        db.Competitions.Remove(competition);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = AppRoles.Participant)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int id)
    {
        var participant = await GetCurrentParticipantAsync();
        if (participant is null)
        {
            return Forbid();
        }

        var competition = await db.Competitions
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        if (!IsCompetitionActual(competition.dateEnd))
        {
            return Forbid();
        }

        var alreadyEnrolled = await db.ParticipantsCompetitions.AnyAsync(item =>
            item.idCompetition == id && item.idParticipant == participant.id);

        if (!alreadyEnrolled)
        {
            db.ParticipantsCompetitions.Add(new ParticipantsCompetition
            {
                idCompetition = id,
                idParticipant = participant.id
            });

            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = AppRoles.Participant)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(int id)
    {
        var participant = await GetCurrentParticipantAsync();
        if (participant is null)
        {
            return Forbid();
        }

        var competition = await db.Competitions
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.id == id);

        if (competition is null)
        {
            return NotFound();
        }

        if (!IsCompetitionActual(competition.dateEnd))
        {
            return Forbid();
        }

        var enrollment = await db.ParticipantsCompetitions.FirstOrDefaultAsync(item =>
            item.idCompetition == id && item.idParticipant == participant.id);

        if (enrollment is not null)
        {
            db.ParticipantsCompetitions.Remove(enrollment);
            await db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private IQueryable<Competition> LoadCompetitionsQuery()
    {
        return db.Competitions
            .AsNoTracking()
            .Include(item => item.idSportSubTypeNavigation)
                .ThenInclude(item => item.idSportTypeNavigation)
                    .ThenInclude(item => item.idSportNavigation)
            .Include(item => item.EventsCompetitions)
                .ThenInclude(item => item.idEventNavigation)
            .Include(item => item.ParticipantsCompetitions);
    }

    private bool CanManageCatalog()
    {
        return User.IsInRole(AppRoles.Administrator) || User.IsInRole(AppRoles.Organizer);
    }

    private async Task PopulateOptionsAsync(CompetitionEditViewModel model)
    {
        var events = await db.Events
            .AsNoTracking()
            .OrderBy(item => item.dateStart ?? DateTime.MaxValue)
            .ToListAsync();

        model.EventOptions = events
            .Select(item => new SelectListItem
            {
                Value = item.id.ToString(),
                Text = $"{item.title} ({FormatDate(item.dateStart)} - {FormatDate(item.dateEnd)})",
                Selected = model.EventId == item.id
            })
            .ToList();

        var sportSubTypes = await db.SportSubtypes
            .AsNoTracking()
            .Include(item => item.idSportTypeNavigation)
                .ThenInclude(item => item.idSportNavigation)
            .OrderBy(item => item.idSportTypeNavigation.idSportNavigation.title)
            .ThenBy(item => item.idSportTypeNavigation.title)
            .ThenBy(item => item.title)
            .ToListAsync();

        model.SportSubtypeOptions = sportSubTypes
            .Select(item => new SelectListItem
            {
                Value = item.id.ToString(),
                Text = $"{item.idSportTypeNavigation.idSportNavigation.title} / {item.idSportTypeNavigation.title} / {item.title}",
                Selected = model.SportSubTypeId == item.id
            })
            .ToList();
    }

    private async Task<Participant?> GetCurrentParticipantAsync()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return null;
        }

        var user = await db.Users
            .Include(item => item.idContactNavigation)
                .ThenInclude(item => item!.Participant)
            .FirstOrDefaultAsync(item => item.id == userId);

        if (user?.idContactNavigation is null)
        {
            return null;
        }

        if (user.idContactNavigation.Participant is not null)
        {
            return user.idContactNavigation.Participant;
        }

        if (!User.IsInRole(AppRoles.Participant))
        {
            return null;
        }

        var participant = new Participant
        {
            idContact = user.idContactNavigation.id
        };

        db.Participants.Add(participant);
        await db.SaveChangesAsync();
        return participant;
    }

    private static bool MatchesSearch(Competition competition, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return true;
        }

        return competition.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(competition.description)
                && competition.description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            || competition.idSportSubTypeNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || competition.idSportSubTypeNavigation.idSportTypeNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || competition.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || competition.EventsCompetitions.Any(link => link.idEventNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
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

    private static bool IsCompetitionActual(DateTime? dateEnd)
    {
        return !dateEnd.HasValue || dateEnd.Value.Date >= DateTime.Today;
    }

    private void ValidateCompetition(CompetitionEditViewModel model)
    {
        if (model.DateStart.HasValue && model.DateEnd.HasValue && model.DateEnd < model.DateStart)
        {
            ModelState.AddModelError(nameof(CompetitionEditViewModel.DateEnd), "Дата окончания не может быть раньше даты начала.");
        }
    }

    private static string FormatDate(DateTime? date)
    {
        return date?.ToString("dd.MM.yyyy") ?? "без даты";
    }

    private static string? NormalizePhotoUrl(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? null : photoUrl.Trim();
    }
}
