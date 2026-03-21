namespace SportEvents.Web.Constants;

public static class AppRoles
{
    public const string Administrator = "Администратор";
    public const string Organizer = "Организатор";
    public const string Judge = "Судья";
    public const string Coach = "Тренер";
    public const string Participant = "Участник";
    public const string User = "Пользователь";
    public const string Guest = "Гость";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Administrator,
        Organizer,
        Judge,
        Coach,
        Participant,
        User,
        Guest
    };
}
