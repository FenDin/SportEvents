using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Data;
using SportEvents.Web.Models;
using SportEvents.Web.Models.Home;
using System.Diagnostics;
using System.Security.Claims;

namespace SportEvents.Web.Controllers;

public class HomeController : Controller
{
    private readonly SportsCompetitionsDbContext db;

    public HomeController(SportsCompetitionsDbContext db)
    {
        this.db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await BuildOverviewModelAsync());
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> StartPage()
    {
        return View(await BuildOverviewModelAsync());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<HomeOverviewViewModel> BuildOverviewModelAsync()
    {
        var today = DateTime.Today;

        var upcomingEvents = await db.Events
            .AsNoTracking()
            .Where(item => !item.dateEnd.HasValue || item.dateEnd >= today)
            .OrderBy(item => item.dateStart ?? DateTime.MaxValue)
            .Select(item => new FeaturedEventViewModel
            {
                Id = item.id,
                Title = item.title,
                Description = item.description,
                DateStart = item.dateStart,
                DateEnd = item.dateEnd,
                CompetitionCount = item.EventsCompetitions.Count
            })
            .Take(3)
            .ToListAsync();

        if (upcomingEvents.Count == 0)
        {
            upcomingEvents = await db.Events
                .AsNoTracking()
                .OrderBy(item => item.dateStart ?? DateTime.MaxValue)
                .Select(item => new FeaturedEventViewModel
                {
                    Id = item.id,
                    Title = item.title,
                    Description = item.description,
                    DateStart = item.dateStart,
                    DateEnd = item.dateEnd,
                    CompetitionCount = item.EventsCompetitions.Count
                })
                .Take(3)
                .ToListAsync();
        }

        return new HomeOverviewViewModel
        {
            EventsCount = await db.Events.CountAsync(),
            CompetitionsCount = await db.Competitions.CountAsync(),
            ParticipantCount = await db.Participants.CountAsync(),
            UpcomingEvents = upcomingEvents,
            DemoAccounts = DemoAccountCatalog.All,
            CurrentRole = User.FindFirstValue(ClaimTypes.Role),
            UserDisplayName = GetDisplayName()
        };
    }

    private string? GetDisplayName()
    {
        var displayName = string.Join(
            " ",
            new[]
            {
                User.FindFirstValue(ClaimTypes.GivenName),
                User.FindFirstValue(ClaimTypes.Surname)
            }.Where(item => !string.IsNullOrWhiteSpace(item)));

        return string.IsNullOrWhiteSpace(displayName) ? User.Identity?.Name : displayName;
    }
}
