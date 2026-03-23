using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportEvents.Web.Constants;
using SportEvents.Web.Models.Db;

namespace SportEvents.Web.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(SportsCompetitionsDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        await EnsurePhotoColumnsAsync(db);

        await SeedRolesAsync(db);
        await SeedSportsCatalogAsync(db);
        await SeedEventsCatalogAsync(db);
        await SeedDemoUsersAsync(db);
    }

    private static async Task SeedRolesAsync(SportsCompetitionsDbContext db)
    {
        var existingRoles = await db.Roles
            .AsNoTracking()
            .Select(item => item.title)
            .ToListAsync();

        var missingRoles = AppRoles.All
            .Except(existingRoles, StringComparer.OrdinalIgnoreCase)
            .Select(title => new Role { title = title })
            .ToList();

        if (missingRoles.Count == 0)
        {
            return;
        }

        await db.Roles.AddRangeAsync(missingRoles);
        await db.SaveChangesAsync();
    }

    private static async Task SeedSportsCatalogAsync(SportsCompetitionsDbContext db)
    {
        var sports = new[]
        {
            new SportSeed(
                "Лёгкая атлетика",
                new[]
                {
                    new SportTypeSeed("Бег", new[] { "100 м", "200 м", "Марафон" }),
                    new SportTypeSeed("Прыжки", new[] { "Прыжок в длину" })
                }),
            new SportSeed(
                "Плавание",
                new[]
                {
                    new SportTypeSeed("Вольный стиль", new[] { "50 м вольным стилем", "100 м вольным стилем" })
                }),
            new SportSeed(
                "Командные виды спорта",
                new[]
                {
                    new SportTypeSeed("Волейбол", new[] { "Классический волейбол" }),
                    new SportTypeSeed("Баскетбол", new[] { "Баскетбол 3x3" })
                })
        };

        foreach (var sportSeed in sports)
        {
            var sport = await db.Sports.FirstOrDefaultAsync(item => item.title == sportSeed.Title);
            if (sport is null)
            {
                sport = new Sport { title = sportSeed.Title };
                db.Sports.Add(sport);
                await db.SaveChangesAsync();
            }

            foreach (var typeSeed in sportSeed.Types)
            {
                var sportType = await db.SportTypes.FirstOrDefaultAsync(item =>
                    item.title == typeSeed.Title && item.idSport == sport.id);

                if (sportType is null)
                {
                    sportType = new SportType
                    {
                        idSport = sport.id,
                        title = typeSeed.Title
                    };

                    db.SportTypes.Add(sportType);
                    await db.SaveChangesAsync();
                }

                foreach (var subTypeTitle in typeSeed.SubTypes)
                {
                    var exists = await db.SportSubtypes.AnyAsync(item =>
                        item.title == subTypeTitle && item.idSportType == sportType.id);

                    if (exists)
                    {
                        continue;
                    }

                    db.SportSubtypes.Add(new SportSubtype
                    {
                        idSportType = sportType.id,
                        title = subTypeTitle
                    });

                    await db.SaveChangesAsync();
                }
            }
        }
    }

    private static async Task SeedEventsCatalogAsync(SportsCompetitionsDbContext db)
    {
        var eventSeeds = new[]
        {
            new EventSeed(
                "Кубок города по летним видам спорта 2026",
                "Открытая серия стартов для школьных и студенческих команд.",
                new DateTime(2026, 5, 14, 9, 0, 0),
                new DateTime(2026, 5, 16, 19, 0, 0),
                MediaCatalog.Seeded.EventCityCup,
                new[]
                {
                    new CompetitionSeed("100 м", "Спринт 100 м", "Личный старт на 100 метров.", new DateTime(2026, 5, 14, 10, 0, 0), new DateTime(2026, 5, 14, 13, 0, 0), MediaCatalog.Seeded.CompetitionSprint),
                    new CompetitionSeed("Прыжок в длину", "Прыжок в длину", "Квалификация и финал по прыжкам в длину.", new DateTime(2026, 5, 15, 11, 0, 0), new DateTime(2026, 5, 15, 16, 0, 0), MediaCatalog.Seeded.CompetitionLongJump),
                    new CompetitionSeed("Классический волейбол", "Турнир по волейболу", "Групповой этап и плей-офф для команд.", new DateTime(2026, 5, 16, 9, 0, 0), new DateTime(2026, 5, 16, 18, 0, 0), MediaCatalog.Seeded.CompetitionVolleyball)
                }),
            new EventSeed(
                "Открытый чемпионат университетов 2026",
                "Межвузовская программа по плаванию, бегу и игровым видам спорта.",
                new DateTime(2026, 9, 10, 10, 0, 0),
                new DateTime(2026, 9, 12, 20, 0, 0),
                MediaCatalog.Seeded.EventUniversity,
                new[]
                {
                    new CompetitionSeed("200 м", "Университетский забег 200 м", "Финальный забег между командами университетов.", new DateTime(2026, 9, 10, 11, 0, 0), new DateTime(2026, 9, 10, 14, 0, 0), MediaCatalog.Seeded.CompetitionRun200),
                    new CompetitionSeed("100 м вольным стилем", "Плавание 100 м вольным стилем", "Индивидуальный старт на короткой воде.", new DateTime(2026, 9, 11, 10, 0, 0), new DateTime(2026, 9, 11, 13, 0, 0), MediaCatalog.Seeded.CompetitionSwimming),
                    new CompetitionSeed("Баскетбол 3x3", "Кубок по баскетболу 3x3", "Быстрый турнир для студенческих сборных.", new DateTime(2026, 9, 12, 12, 0, 0), new DateTime(2026, 9, 12, 18, 0, 0), MediaCatalog.Seeded.CompetitionBasketball)
                })
        };

        foreach (var eventSeed in eventSeeds)
        {
            var eventEntity = await db.Events.FirstOrDefaultAsync(item => item.title == eventSeed.Title);
            if (eventEntity is null)
            {
                eventEntity = new Event
                {
                    title = eventSeed.Title,
                    description = eventSeed.Description,
                    dateStart = eventSeed.DateStart,
                    dateEnd = eventSeed.DateEnd,
                    photoUrl = eventSeed.PhotoUrl
                };

                db.Events.Add(eventEntity);
                await db.SaveChangesAsync();
            }

            eventEntity.description = eventSeed.Description;
            eventEntity.dateStart = eventSeed.DateStart;
            eventEntity.dateEnd = eventSeed.DateEnd;
            eventEntity.photoUrl = eventSeed.PhotoUrl;
            await db.SaveChangesAsync();

            foreach (var competitionSeed in eventSeed.Competitions)
            {
                var sportSubType = await db.SportSubtypes.FirstOrDefaultAsync(item => item.title == competitionSeed.SportSubTypeTitle);
                if (sportSubType is null)
                {
                    continue;
                }

                var competition = await db.Competitions.FirstOrDefaultAsync(item => item.title == competitionSeed.Title);
                if (competition is null)
                {
                    competition = new Competition
                    {
                        idSportSubType = sportSubType.id,
                        title = competitionSeed.Title,
                        description = competitionSeed.Description,
                        dateStart = competitionSeed.DateStart,
                        dateEnd = competitionSeed.DateEnd,
                        photoUrl = competitionSeed.PhotoUrl
                    };

                    db.Competitions.Add(competition);
                    await db.SaveChangesAsync();
                }

                competition.description = competitionSeed.Description;
                competition.dateStart = competitionSeed.DateStart;
                competition.dateEnd = competitionSeed.DateEnd;
                competition.photoUrl = competitionSeed.PhotoUrl;
                await db.SaveChangesAsync();

                var linkExists = await db.EventsCompetitions.AnyAsync(item =>
                    item.idEvent == eventEntity.id && item.idCompetition == competition.id);

                if (linkExists)
                {
                    continue;
                }

                db.EventsCompetitions.Add(new EventsCompetition
                {
                    idEvent = eventEntity.id,
                    idCompetition = competition.id
                });

                await db.SaveChangesAsync();
            }
        }
    }

    private static async Task SeedDemoUsersAsync(SportsCompetitionsDbContext db)
    {
        var passwordHasher = new PasswordHasher<object>();
        var roles = await db.Roles.ToDictionaryAsync(item => item.title, StringComparer.OrdinalIgnoreCase);

        foreach (var account in DemoAccountCatalog.All)
        {
            if (!roles.TryGetValue(account.Role, out var role))
            {
                continue;
            }

            var contact = await db.Contacts
                .Include(item => item.User)
                .Include(item => item.Participant)
                .FirstOrDefaultAsync(item => item.email == account.Email);

            if (contact is null)
            {
                contact = new Contact
                {
                    firstname = account.FirstName,
                    lastname = account.LastName,
                    middlename = account.MiddleName,
                    birthDate = account.BirthDate,
                    sex = account.Sex,
                    phone = account.Phone,
                    email = account.Email
                };

                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
            }

            contact.firstname = account.FirstName;
            contact.lastname = account.LastName;
            contact.middlename = account.MiddleName;
            contact.birthDate = account.BirthDate;
            contact.sex = account.Sex;
            contact.phone = account.Phone;
            contact.email = account.Email;
            contact.photoUrl = account.PhotoUrl;
            contact.passwordHash = passwordHasher.HashPassword(null!, account.Password);

            if (contact.User is null)
            {
                db.Users.Add(new User
                {
                    idContact = contact.id,
                    idRole = role.id
                });
            }
            else
            {
                contact.User.idRole = role.id;
            }

            if (string.Equals(account.Role, AppRoles.Participant, StringComparison.OrdinalIgnoreCase)
                && contact.Participant is null)
            {
                db.Participants.Add(new Participant { idContact = contact.id });
            }

            await db.SaveChangesAsync();
        }
    }

    private static async Task EnsurePhotoColumnsAsync(SportsCompetitionsDbContext db)
    {
        const string sql = """
            IF COL_LENGTH('dbo.Contact', 'photoUrl') IS NULL
                ALTER TABLE [dbo].[Contact] ADD [photoUrl] nvarchar(512) NULL;

            IF COL_LENGTH('dbo.Event', 'photoUrl') IS NULL
                ALTER TABLE [dbo].[Event] ADD [photoUrl] nvarchar(512) NULL;

            IF COL_LENGTH('dbo.Competition', 'photoUrl') IS NULL
                ALTER TABLE [dbo].[Competition] ADD [photoUrl] nvarchar(512) NULL;
            """;

        await db.Database.ExecuteSqlRawAsync(sql);
    }

    private sealed record SportSeed(string Title, IReadOnlyList<SportTypeSeed> Types);
    private sealed record SportTypeSeed(string Title, IReadOnlyList<string> SubTypes);
    private sealed record EventSeed(
        string Title,
        string Description,
        DateTime DateStart,
        DateTime DateEnd,
        string PhotoUrl,
        IReadOnlyList<CompetitionSeed> Competitions);
    private sealed record CompetitionSeed(
        string SportSubTypeTitle,
        string Title,
        string Description,
        DateTime DateStart,
        DateTime DateEnd,
        string PhotoUrl);
}
