using SportEvents.Web.Constants;

namespace SportEvents.Web.Models.Home;

public sealed class HomeOverviewViewModel
{
    public int EventsCount { get; init; }
    public int CompetitionsCount { get; init; }
    public int ParticipantCount { get; init; }
    public string? CurrentRole { get; init; }
    public string? UserDisplayName { get; init; }
    public string CurrentUserPhotoUrl { get; init; } = string.Empty;
    public IReadOnlyList<FeaturedEventViewModel> UpcomingEvents { get; init; } = Array.Empty<FeaturedEventViewModel>();
    public IReadOnlyList<DemoAccountDefinition> DemoAccounts { get; init; } = Array.Empty<DemoAccountDefinition>();
}

public sealed class FeaturedEventViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string PhotoUrl { get; init; } = string.Empty;
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public int CompetitionCount { get; init; }
}
