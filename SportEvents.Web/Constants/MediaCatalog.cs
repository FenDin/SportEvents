namespace SportEvents.Web.Constants;

public static class MediaCatalog
{
    public static class Defaults
    {
        public const string EventPhoto = "/images/placeholders/event-default.svg";
        public const string CompetitionPhoto = "/images/placeholders/competition-default.svg";
        public const string UserPhoto = "/images/placeholders/user-default.svg";
    }

    public static class Seeded
    {
        public const string EventCityCup = "/images/demo/events/city-cup.svg";
        public const string EventUniversity = "/images/demo/events/university-championship.svg";

        public const string CompetitionSprint = "/images/demo/competitions/sprint.svg";
        public const string CompetitionLongJump = "/images/demo/competitions/long-jump.svg";
        public const string CompetitionVolleyball = "/images/demo/competitions/volleyball.svg";
        public const string CompetitionRun200 = "/images/demo/competitions/run-200.svg";
        public const string CompetitionSwimming = "/images/demo/competitions/swimming.svg";
        public const string CompetitionBasketball = "/images/demo/competitions/basketball-3x3.svg";

        public const string UserAdministrator = "/images/demo/users/administrator.svg";
        public const string UserOrganizer = "/images/demo/users/organizer.svg";
        public const string UserParticipant = "/images/demo/users/participant.svg";
        public const string UserViewer = "/images/demo/users/viewer.svg";
    }

    public static string EventPhotoOrDefault(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? Defaults.EventPhoto : photoUrl;
    }

    public static string CompetitionPhotoOrDefault(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? Defaults.CompetitionPhoto : photoUrl;
    }

    public static string UserPhotoOrDefault(string? photoUrl)
    {
        return string.IsNullOrWhiteSpace(photoUrl) ? Defaults.UserPhoto : photoUrl;
    }
}
