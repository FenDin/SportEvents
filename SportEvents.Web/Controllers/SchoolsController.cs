using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models.Db;
using SportEvents.Web.Models.Schools;

namespace SportEvents.Web.Controllers;

public class SchoolsController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public SchoolsController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? searchQuery)
    {
        var schools = await LoadSchoolsQuery()
            .AsSplitQuery()
            .OrderBy(item => item.title)
            .ToListAsync();

        var items = schools
            .Where(item => MatchesSearch(item, searchQuery))
            .Select(MapSchoolListItem)
            .ToList();

        return View(new SchoolCatalogViewModel
        {
            SearchQuery = searchQuery,
            CanManage = CanManageCatalog(),
            Items = items
        });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var school = await LoadSchoolsQuery()
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (school is null)
        {
            return NotFound();
        }

        return View(MapSchoolDetails(school, CanManageCatalog()));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new SchoolEditViewModel();
        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SchoolEditViewModel model)
    {
        await ValidateSchoolAsync(model);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var school = new School
        {
            title = model.Title,
            description = model.Description
        };

        db.Schools.Add(school);
        await db.SaveChangesAsync();

        foreach (var sportSubtypeId in model.SelectedSportSubtypeIds.Distinct())
        {
            db.SchoolsSportsSubTypes.Add(new SchoolsSportsSubType
            {
                idSchool = school.id,
                idSportSubType = sportSubtypeId
            });
        }

        if (model.SelectedParticipantIds.Count > 0)
        {
            var participants = await db.Participants
                .Where(item => model.SelectedParticipantIds.Contains(item.id))
                .ToListAsync();

            foreach (var participant in participants)
            {
                participant.idSchool = school.id;
            }
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = school.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var school = await db.Schools
            .AsNoTracking()
            .Include(item => item.SchoolsSportsSubTypes)
            .Include(item => item.Participants)
            .FirstOrDefaultAsync(item => item.id == id);

        if (school is null)
        {
            return NotFound();
        }

        var model = new SchoolEditViewModel
        {
            Id = school.id,
            Title = school.title,
            Description = school.description,
            SelectedSportSubtypeIds = school.SchoolsSportsSubTypes
                .Select(item => item.idSportSubType)
                .ToList(),
            SelectedParticipantIds = school.Participants
                .Select(item => item.id)
                .ToList()
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SchoolEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateSchoolAsync(model, id);

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var school = await db.Schools
            .Include(item => item.SchoolsSportsSubTypes)
            .Include(item => item.Participants)
            .FirstOrDefaultAsync(item => item.id == id);

        if (school is null)
        {
            return NotFound();
        }

        school.title = model.Title;
        school.description = model.Description;

        var selectedSportSubtypeIds = model.SelectedSportSubtypeIds
            .Distinct()
            .ToHashSet();

        db.SchoolsSportsSubTypes.RemoveRange(
            school.SchoolsSportsSubTypes.Where(item => !selectedSportSubtypeIds.Contains(item.idSportSubType)));

        var existingSportSubtypeIds = school.SchoolsSportsSubTypes
            .Select(item => item.idSportSubType)
            .ToHashSet();

        foreach (var sportSubtypeId in selectedSportSubtypeIds)
        {
            if (existingSportSubtypeIds.Contains(sportSubtypeId))
            {
                continue;
            }

            db.SchoolsSportsSubTypes.Add(new SchoolsSportsSubType
            {
                idSchool = school.id,
                idSportSubType = sportSubtypeId
            });
        }

        var selectedParticipantIds = model.SelectedParticipantIds
            .Distinct()
            .ToHashSet();

        foreach (var participant in school.Participants.Where(item => !selectedParticipantIds.Contains(item.id)))
        {
            participant.idSchool = null;
        }

        if (selectedParticipantIds.Count > 0)
        {
            var participantsToAssign = await db.Participants
                .Where(item => selectedParticipantIds.Contains(item.id))
                .ToListAsync();

            foreach (var participant in participantsToAssign)
            {
                participant.idSchool = school.id;
            }
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = school.id });
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var school = await LoadSchoolsQuery()
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.id == id);

        if (school is null)
        {
            return NotFound();
        }

        return View(MapSchoolDetails(school, false));
    }

    [Authorize(Policy = AppPolicies.ManageCatalog)]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var school = await db.Schools
            .Include(item => item.Participants)
            .Include(item => item.SchoolsSportsSubTypes)
            .FirstOrDefaultAsync(item => item.id == id);

        if (school is null)
        {
            return NotFound();
        }

        foreach (var participant in school.Participants)
        {
            participant.idSchool = null;
        }

        db.SchoolsSportsSubTypes.RemoveRange(school.SchoolsSportsSubTypes);
        db.Schools.Remove(school);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private IQueryable<School> LoadSchoolsQuery()
    {
        return db.Schools
            .AsNoTracking()
            .Include(item => item.Participants)
                .ThenInclude(item => item.idContactNavigation)
            .Include(item => item.SchoolsSportsSubTypes)
                .ThenInclude(item => item.idSportSubTypeNavigation)
                    .ThenInclude(item => item.idSportTypeNavigation)
                        .ThenInclude(item => item.idSportNavigation);
    }

    private async Task PopulateOptionsAsync(SchoolEditViewModel model)
    {
        var selectedSportSubtypeIds = model.SelectedSportSubtypeIds.ToHashSet();
        var selectedParticipantIds = model.SelectedParticipantIds.ToHashSet();
        var sportSubtypes = await db.SportSubtypes
            .AsNoTracking()
            .Include(item => item.idSportTypeNavigation)
                .ThenInclude(item => item.idSportNavigation)
            .OrderBy(item => item.idSportTypeNavigation.idSportNavigation.title)
            .ThenBy(item => item.idSportTypeNavigation.title)
            .ThenBy(item => item.title)
            .ToListAsync();

        model.SportSubtypeOptions = sportSubtypes
            .Select(item => new SelectListItem
            {
                Value = item.id.ToString(),
                Text = $"{item.idSportTypeNavigation.idSportNavigation.title} / {item.idSportTypeNavigation.title} / {item.title}",
                Selected = selectedSportSubtypeIds.Contains(item.id)
            })
            .ToList();

        var participants = await db.Participants
            .AsNoTracking()
            .Include(item => item.idContactNavigation)
            .Include(item => item.idSchoolNavigation)
            .OrderBy(item => item.idContactNavigation.lastname)
            .ThenBy(item => item.idContactNavigation.firstname)
            .ToListAsync();

        model.ParticipantOptions = participants
            .Select(item => new SelectListItem
            {
                Value = item.id.ToString(),
                Text = BuildParticipantOptionLabel(item),
                Selected = selectedParticipantIds.Contains(item.id)
            })
            .ToList();
    }

    private async Task ValidateSchoolAsync(SchoolEditViewModel model, int? currentSchoolId = null)
    {
        var duplicateExists = await db.Schools.AnyAsync(item =>
            item.title == model.Title && item.id != currentSchoolId);

        if (duplicateExists)
        {
            ModelState.AddModelError(nameof(SchoolEditViewModel.Title), "Школа с таким названием уже существует.");
        }
    }

    private bool CanManageCatalog()
    {
        return User.IsInRole(AppRoles.Administrator) || User.IsInRole(AppRoles.Organizer);
    }

    private static bool MatchesSearch(School school, string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return true;
        }

        return school.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(school.description)
                && school.description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
            || school.SchoolsSportsSubTypes.Any(item =>
                item.idSportSubTypeNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                || item.idSportSubTypeNavigation.idSportTypeNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                || item.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    private static SchoolListItemViewModel MapSchoolListItem(School school)
    {
        return new SchoolListItemViewModel
        {
            Id = school.id,
            Title = school.title,
            Description = school.description,
            ParticipantCount = school.Participants.Count,
            SportSubTypes = school.SchoolsSportsSubTypes
                .Select(FormatSportSubtype)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList(),
            ParticipantNames = school.Participants
                .OrderBy(item => item.idContactNavigation.lastname)
                .ThenBy(item => item.idContactNavigation.firstname)
                .Select(item => FormatFullName(
                    item.idContactNavigation.lastname,
                    item.idContactNavigation.firstname,
                    item.idContactNavigation.middlename))
                .Take(3)
                .ToList()
        };
    }

    private static SchoolDetailsViewModel MapSchoolDetails(School school, bool canManage)
    {
        return new SchoolDetailsViewModel
        {
            Id = school.id,
            Title = school.title,
            Description = school.description,
            ParticipantCount = school.Participants.Count,
            CanManage = canManage,
            SportSubTypes = school.SchoolsSportsSubTypes
                .Select(FormatSportSubtype)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(item => item)
                .ToList(),
            Participants = school.Participants
                .OrderBy(item => item.idContactNavigation.lastname)
                .ThenBy(item => item.idContactNavigation.firstname)
                .Select(item => new SchoolParticipantViewModel
                {
                    FullName = FormatFullName(
                        item.idContactNavigation.lastname,
                        item.idContactNavigation.firstname,
                        item.idContactNavigation.middlename),
                    Email = item.idContactNavigation.email,
                    Phone = item.idContactNavigation.phone,
                    PhotoUrl = MediaCatalog.UserPhotoOrDefault(item.idContactNavigation.photoUrl)
                })
                .ToList()
        };
    }

    private static string FormatSportSubtype(SchoolsSportsSubType schoolSportSubtype)
    {
        return $"{schoolSportSubtype.idSportSubTypeNavigation.idSportTypeNavigation.idSportNavigation.title} / {schoolSportSubtype.idSportSubTypeNavigation.idSportTypeNavigation.title} / {schoolSportSubtype.idSportSubTypeNavigation.title}";
    }

    private static string FormatFullName(string lastName, string firstName, string? middleName)
    {
        return string.Join(
            " ",
            new[] { lastName, firstName, middleName }
                .Where(item => !string.IsNullOrWhiteSpace(item)));
    }

    private static string BuildParticipantOptionLabel(Participant participant)
    {
        var fullName = FormatFullName(
            participant.idContactNavigation.lastname,
            participant.idContactNavigation.firstname,
            participant.idContactNavigation.middlename);

        if (participant.idSchoolNavigation is null)
        {
            return $"{fullName} ({participant.idContactNavigation.email})";
        }

        return $"{fullName} ({participant.idContactNavigation.email}) - {participant.idSchoolNavigation.title}";
    }
}
