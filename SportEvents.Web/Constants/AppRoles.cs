namespace SportEvents.Web.Constants;

public static class AppRoles
{
    public const string Administrator = "Администратор";
    public const string Organizer = "Организатор";
    public const string Participant = "Участник";
    public const string User = "Пользователь";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Administrator,
        Organizer,
        Participant,
        User
    };
}
