namespace SportEvents.Web.Constants;

public sealed record DemoAccountDefinition(
    string Role,
    string Email,
    string Password,
    string PhotoUrl,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateTime BirthDate,
    bool Sex,
    string Phone,
    string Description);

public static class DemoAccountCatalog
{
    public static readonly IReadOnlyList<DemoAccountDefinition> All = new[]
    {
        new DemoAccountDefinition(
            AppRoles.Administrator,
            "admin@sportevents.local",
            "Admin123!",
            MediaCatalog.Seeded.UserAdministrator,
            "Алексей",
            "Админов",
            "Сергеевич",
            new DateTime(1988, 4, 12),
            true,
            "+79990000001",
            "Полный доступ: управление каталогом, ролями и демо-данными."),
        new DemoAccountDefinition(
            AppRoles.Organizer,
            "organizer@sportevents.local",
            "Organizer123!",
            MediaCatalog.Seeded.UserOrganizer,
            "Ольга",
            "Организаторова",
            "Игоревна",
            new DateTime(1992, 8, 6),
            false,
            "+79990000002",
            "Создание, редактирование и удаление спортивных событий."),
        new DemoAccountDefinition(
            AppRoles.Participant,
            "participant@sportevents.local",
            "Participant123!",
            MediaCatalog.Seeded.UserParticipant,
            "Павел",
            "Участников",
            "Андреевич",
            new DateTime(2001, 2, 18),
            true,
            "+79990000003",
            "Просмотр каталога и запись на соревнования."),
        new DemoAccountDefinition(
            AppRoles.Participant,
            "participant2@sportevents.local",
            "Participant234!",
            MediaCatalog.Seeded.UserViewer,
            "Марина",
            "Финишерова",
            "Олеговна",
            new DateTime(2002, 7, 4),
            false,
            "+79990000005",
            "Демо-участник для тестирования привязки к школам и соревнованиям."),
        new DemoAccountDefinition(
            AppRoles.Participant,
            "participant3@sportevents.local",
            "Participant345!",
            MediaCatalog.Seeded.UserParticipant,
            "Илья",
            "Эстафетов",
            "Максимович",
            new DateTime(2000, 9, 14),
            true,
            "+79990000006",
            "Демо-участник для наполнения таблиц участников."),
        new DemoAccountDefinition(
            AppRoles.User,
            "user@sportevents.local",
            "User12345!",
            MediaCatalog.Seeded.UserViewer,
            "Юлия",
            "Пользовательская",
            "Олеговна",
            new DateTime(1999, 11, 22),
            false,
            "+79990000004",
            "Базовый просмотр без административных прав.")
    };
}
