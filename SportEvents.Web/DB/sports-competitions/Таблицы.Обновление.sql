-- #region НАЗВАНИЕ_ОБЛАСТИ_КОДА

--#endregion

--#region Значения по умолчанию

ALTER TABLE [Image]
ADD CONSTRAINT DF_ImageCreated DEFAULT (SYSDATETIME()) FOR dateCreated;

ALTER TABLE [User]
ADD CONSTRAINT DF_UserCreated DEFAULT (SYSDATETIME()) FOR dateCreated;

--#endregion

--#region Валидация

--#region SPORT 

ALTER TABLE dbo.Sport
    ADD CONSTRAINT CK_Sport_Title_NotEmpty
    CHECK(LEN(LTRIM(RTRIM(title)))>0);
CREATE UNIQUE INDEX UX_Sport_Title ON dbo.Sport(title);

--#endregion

--#region TYPES

-- Проверка не включена, поскольку подвиды спорта повторяются
-- Например: Повторяющееся значение ключа: (Классический).

-- ALTER TABLE dbo.SportType
--     ADD CONSTRAINT CK_SportType_Title_NotEmpty
--     CHECK(LEN(LTRIM(RTRIM(title)))>0);
-- CREATE UNIQUE INDEX UX_Sport_Title ON dbo.SportType(title);

--#endregion

--#region SUBTYPES

-- Проверка не включена, поскольку подвиды спорта повторяются
-- Например: Повторяющееся значение ключа: (Мужчины).

-- ALTER TABLE dbo.SportSubType
--     ADD CONSTRAINT CK_SportSubType_Title_NotEmpty
--     CHECK(LEN(LTRIM(RTRIM(title)))>0);
-- CREATE UNIQUE INDEX UX_Sport_Title ON dbo.SportSubType(title);

--#endregion

-- #region SCHOOL

-- Проверка на пустое  и уникальное название
ALTER TABLE dbo.School
ADD CONSTRAINT CK_School_Title_NotEmpty
CHECK (LEN(LTRIM(RTRIM(title)))>0);
CREATE UNIQUE INDEX UX_School_Title ON dbo.School(title);

--#endregion

-- #region COMPETITION

-- Проверка на пустое  название
ALTER TABLE dbo.Competition
ADD CONSTRAINT CK_Competition_Title_NotEmpty
CHECK (LEN(LTRIM(RTRIM(title)))>0);
CREATE UNIQUE INDEX UX_Competition_Title ON dbo.Competition(title)
-- Проверка дат начала и кончания
ALTER TABLE dbo.Competition
ADD CONSTRAINT CK_Competition_Date
CHECK (dateStart <= dateEnd)

--#endregion

-- #region EVENT

-- Проверка на пустое  название
ALTER TABLE dbo.Event
ADD CONSTRAINT CK_Event_Title_NotEmpty
CHECK (LEN(LTRIM(RTRIM(title)))>0);
CREATE UNIQUE INDEX UX_Competition_Title ON dbo.Event(title)
-- Проверка дат начала и кончания
ALTER TABLE dbo.Event
ADD CONSTRAINT CK_Event_Date
CHECK (dateStart <= dateEnd)

--#endregion

-- #region CONTACT

--Возраст
-- ALTER TABLE dbo.Contact
-- ADD CONSTRAINT CK_Contact_Age CHECK (age BETWEEN 0 AND 120);
--ФИО на пустые строки
ALTER TABLE dbo.Contact
ADD CONSTRAINT CK_Contact_FirstName_NotEmpty
CHECK (LEN(LTRIM(RTRIM(firstname)))>0);
ALTER TABLE dbo.Contact
ADD CONSTRAINT CK_Contact_LastName_NotEmpty
CHECK (LEN(LTRIM(RTRIM(lastname)))>0);
ALTER TABLE dbo.Contact
ADD CONSTRAINT CK_Contact_MiddleName_NotEmpty
CHECK (LEN(LTRIM(RTRIM(middlename)))>0);
--регулярные выражения на почту и телефон
ALTER TABLE dbo.[Contact]
ADD CONSTRAINT CK_Contact_Phone_NotEmpty
CHECK (LEN(LTRIM(RTRIM(phone))) >= 6);
ALTER TABLE dbo.[Contact]
ADD CONSTRAINT CK_Contact_Email_Format
CHECK (email IS NULL OR email LIKE '%_@_%._%');
--#endregion

-- #region ROLE

-- проверка названия роли
ALTER TABLE dbo.[Role]
ADD CONSTRAINT CK_Role_Title_NotEmpty
CHECK (LEN(LTRIM(RTRIM(title))) > 0);
-- проверка на дубликаты ролей
CREATE UNIQUE INDEX UX_Role_Title ON dbo.[Role](title);

--#endregion

-- #region USER
-- One user per contact (обычно так)
CREATE UNIQUE INDEX UX_User_Contact ON dbo.[User](idContact);
--#endregion

-- #region Participant
-- One participant per contact (обычно так)
CREATE UNIQUE INDEX UX_Participant_Contact ON dbo.Participant(idContact);
--#endregion

-- #region Промежуточные таблицы (anti-duplicates)

-- N'UX_SchoolsSportsSubtypes
CREATE UNIQUE INDEX UX_SchoolsSportsSubtypes ON dbo.SchoolsSportsSubtypes(idSportSubType, idSchool);
-- N'UX_ParticipantsCompetitions
CREATE UNIQUE INDEX UX_ParticipantsCompetitions ON dbo.ParticipantsCompetitions(idCompetition, idParticipant);
-- N'UX_EventsCompetitions
CREATE UNIQUE INDEX UX_EventsCompetitions ON dbo.EventsCompetitions(idEvent, idCompetition);
-- N'UX_ImagesUsers
CREATE UNIQUE INDEX UX_ImagesUsers ON dbo.ImagesUsers(idImage, idContact);

--#endregion



















-- ALTER TABLE [Contact]
-- ADD CONSTRAINT CK_Contact_Phone CHECK (
--     phone LIKE '+%' AND
--     LEN(phone) BETWEEN 11 AND 16 AND
--     phone NOT LIKE '%[^0-9-+]%'
-- );

-- ALTER TABLE [Contact]
-- ADD CONSTRAINT CK_Contact_Email CHECK (
--     email LIKE '%[a-zA-Z0-9_/|\-.]+@[a-zA-Z0-9_/|\-]+[.][a-z]%'
-- );

--#endregion

-- #region FK indexes (performance, optional but recommended)

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_SportType_idSport' AND object_id = OBJECT_ID(N'dbo.SportType'))
        CREATE INDEX IX_SportType_idSport ON dbo.SportType(idSport);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_SportSubType_idSportType' AND object_id = OBJECT_ID(N'dbo.SportSubType'))
        CREATE INDEX IX_SportSubType_idSportType ON dbo.SportSubType(idSportType);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_Competition_idSportSubType' AND object_id = OBJECT_ID(N'dbo.Competition'))
        CREATE INDEX IX_Competition_idSportSubType ON dbo.Competition(idSportSubType);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_Participant_idSchool' AND object_id = OBJECT_ID(N'dbo.Participant'))
        CREATE INDEX IX_Participant_idSchool ON dbo.Participant(idSchool);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_Participant_idContact' AND object_id = OBJECT_ID(N'dbo.Participant'))
        CREATE INDEX IX_Participant_idContact ON dbo.Participant(idContact);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_PC_idCompetition' AND object_id = OBJECT_ID(N'dbo.ParticipantsCompetitions'))
        CREATE INDEX IX_PC_idCompetition ON dbo.ParticipantsCompetitions(idCompetition);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_PC_idParticipant' AND object_id = OBJECT_ID(N'dbo.ParticipantsCompetitions'))
        CREATE INDEX IX_PC_idParticipant ON dbo.ParticipantsCompetitions(idParticipant);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_EC_idEvent' AND object_id = OBJECT_ID(N'dbo.EventsCompetitions'))
        CREATE INDEX IX_EC_idEvent ON dbo.EventsCompetitions(idEvent);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_EC_idCompetition' AND object_id = OBJECT_ID(N'dbo.EventsCompetitions'))
        CREATE INDEX IX_EC_idCompetition ON dbo.EventsCompetitions(idCompetition);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_SSS_idSchool' AND object_id = OBJECT_ID(N'dbo.SchoolsSportsSubtypes'))
        CREATE INDEX IX_SSS_idSchool ON dbo.SchoolsSportsSubtypes(idSchool);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_SSS_idSportSubType' AND object_id = OBJECT_ID(N'dbo.SchoolsSportsSubtypes'))
        CREATE INDEX IX_SSS_idSportSubType ON dbo.SchoolsSportsSubtypes(idSportSubType);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_ImagesUsers_idImage' AND object_id = OBJECT_ID(N'dbo.ImagesUsers'))
        CREATE INDEX IX_ImagesUsers_idImage ON dbo.ImagesUsers(idImage);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_ImagesUsers_idContact' AND object_id = OBJECT_ID(N'dbo.ImagesUsers'))
        CREATE INDEX IX_ImagesUsers_idContact ON dbo.ImagesUsers(idContact);

IF NOT EXISTS (SELECT 1
FROM sys.indexes
WHERE name = N'IX_User_idRole' AND object_id = OBJECT_ID(N'dbo.[User]'))
        CREATE INDEX IX_User_idRole ON dbo.[User](idRole);

--#endregion