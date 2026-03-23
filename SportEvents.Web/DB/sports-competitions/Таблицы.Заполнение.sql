
-- #region НАЗВАНИЕ_ОБЛАСТИ_КОДА

--#endregion

-- #region Переменные
DECLARE @sportId INT;
DECLARE @typeId INT;
declare @idSchool int;
--#endregion

-- #region SPORT: Лёгкая атлетика

INSERT INTO Sport
    (title)
VALUES
    (N'Лёгкая атлетика');
SET @sportId = SCOPE_IDENTITY();

------------------------------------------------------------
-- TYPE: Бег (стадион)
INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Бег (стадион)');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'100 м'),
    (@typeId, N'200 м'),
    (@typeId, N'400 м'),
    (@typeId, N'800 м'),
    (@typeId, N'1500 м'),
    (@typeId, N'5000 м'),
    (@typeId, N'10000 м'),
    (@typeId, N'110 м с барьерами'),
    (@typeId, N'100 м с барьерами'),
    (@typeId, N'400 м с барьерами'),
    (@typeId, N'3000 м с препятствиями'),
    (@typeId, N'Эстафета 4×100 м'),
    (@typeId, N'Эстафета 4×400 м');

------------------------------------------------------------
-- TYPE: Прыжки
INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Прыжки');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Прыжок в высоту'),
    (@typeId, N'Прыжок с шестом'),
    (@typeId, N'Прыжок в длину'),
    (@typeId, N'Тройной прыжок');

------------------------------------------------------------
-- TYPE: Метания
INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Метания');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Толкание ядра'),
    (@typeId, N'Метание диска'),
    (@typeId, N'Метание молота'),
    (@typeId, N'Метание копья');

------------------------------------------------------------
-- TYPE: Многоборья
INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Многоборья');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Десятиборье'),
    (@typeId, N'Семиборье');

------------------------------------------------------------
-- TYPE: Шоссе / ходьба
INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Шоссе и спортивная ходьба');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Марафон'),
    (@typeId, N'Спортивная ходьба 20 км'),
    (@typeId, N'Спортивная ходьба 35 км');
-- #endregion

-- #region SPORT:Баскетбол
INSERT INTO Sport
    (title)
VALUES
    (N'Баскетбол');
SET @sportId = SCOPE_IDENTITY();

INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Классический 5x5');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Мужчины'),
    (@typeId, N'Женщины');

INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'3x3');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Стритбол'),
    (@typeId, N'Олимпийский формат');
--#endregion

-- #region SPORT:Футбол

-- Sport
INSERT INTO Sport
    (title)
VALUES
    (N'Футбол');
SET @sportId = SCOPE_IDENTITY();

-- Типы
INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Классический');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Мужчины'),
    (@typeId, N'Женщины'),
    (@typeId, N'Юниоры');

INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Мини-футбол (футзал)');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Любительский'),
    (@typeId, N'Профессиональный');

INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Пляжный футбол');
--#endregion

-- #region SPORT:Волейбол

INSERT INTO Sport
    (title)
VALUES
    (N'Волейбол');
SET @sportId = SCOPE_IDENTITY();

INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Классический');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Мужчины'),
    (@typeId, N'Женщины'),
    (@typeId, N'Студенческий');

INSERT INTO SportType
    (idSport, title)
VALUES
    (@sportId, N'Пляжный');
SET @typeId = SCOPE_IDENTITY();

INSERT INTO SportSubType
    (idSportType, title)
VALUES
    (@typeId, N'Пары мужчины'),
    (@typeId, N'Пары женщины'),
    (@typeId, N'Смешанные пары');
--#endregion

-- #region Школы(SChool)

-- #region ДЮСШ №1
insert into School
    (title, [description])
values
    (N'ДЮСШ №1', N'ДЮСШ №1 - лучшая спортивная школа по лёгкой атлетике для детей всех возрастов');
set @idSchool = scope_identity();
-- #region ДИсциплины
INSERT INTO SchoolsSportsSubTypes
    (idSportSubType,idSchool)
SELECT SportSubTypeId, @idSchool
FROM vSportSubTypesFull v
WHERE v.SportTitle = N'Лёгкая атлетика'
    AND v.SportTypeTitle IN (N'Бег (стадион)',N'Прыжки');
--#endregion

--#endregion

-- #region "Движение - это жизнь": Бег и Шоссе/ходьба
insert into School
    (title, [description])
values
    (N'Движение - это жизнь', N'Школа лёгкой атлетики "Движение - это жизнь" - лучшее место для взрослых, которые желают заниматься лёгкой атлетикой');
set @idSchool = scope_identity();
-- #region ДИсциплины
INSERT INTO SchoolsSportsSubTypes
    (idSportSubType,idSchool)
SELECT SportSubTypeId, @idSchool
FROM vSportSubTypesFull v
WHERE v.SportTitle = N'Лёгкая атлетика'
    AND v.SportTypeTitle IN (N'Бег (стадион)',N'Шоссе и спортивная ходьба');
--#endregion

--#endregion

--#endregion

-- #region События(Event)

BEGIN TRY
    BEGIN TRANSACTION;
    
        DECLARE @eventId INT;

-- #region 1) EVENT: Олимпийские игры
INSERT INTO [Event]
    (title, [description], photoUrl, dateStart, dateEnd)
VALUES
    (
        N'Олимпийские игры — Париж 2024',
        N'Комплексное международное спортивное событие.',
        N'/images/demo/events/city-cup.svg',
        '2024-07-26T00:00:00',
        '2024-08-11T00:00:00'
);
   SET @eventId = SCOPE_IDENTITY();

-- #region Соревнования(Competition)
DECLARE @toInsert TABLE(
    SportSubTypeTitle NVARCHAR(255) NOT NULL,
    CompetitionTitle NVARCHAR(255) NOT NULL,
    CompetitionDesc NVARCHAR(255) NOT NULL,
    PhotoUrl NVARCHAR(512) NULL,
    DateStart DATETIME NULL,
    DateEnd DATETIME NULL
);


INSERT INTO @toInsert
    (SportSubTypeTitle, CompetitionTitle, CompetitionDesc, PhotoUrl, DateStart, DateEnd)
VALUES
    (N'100 м', N'Лёгкая атлетика — 100 м', N'Предварительные забеги, полуфиналы и финал.', N'/images/demo/competitions/sprint.svg', '2024-08-03T10:00:00', '2024-08-04T22:00:00'),
    (N'200 м', N'Лёгкая атлетика — 200 м', N'Соревнования на спринтерской дистанции.', N'/images/demo/competitions/run-200.svg', '2024-08-05T10:00:00', '2024-08-06T22:00:00'),
    (N'400 м', N'Лёгкая атлетика — 400 м', N'Квалификация и финальные забеги.', N'/images/placeholders/competition-default.svg', '2024-08-07T10:00:00', '2024-08-08T22:00:00'),
    (N'1500 м', N'Лёгкая атлетика — 1500 м', N'Тактическая средняя дистанция.', N'/images/placeholders/competition-default.svg', '2024-08-06T10:00:00', '2024-08-07T22:00:00'),
    (N'Эстафета 4×100 м', N'Лёгкая атлетика — эстафета 4×100 м', N'Командный спринт.', N'/images/placeholders/competition-default.svg', '2024-08-08T18:00:00', '2024-08-09T22:00:00'),
    (N'Прыжок в длину', N'Лёгкая атлетика — прыжок в длину', N'Квалификация и финал.', N'/images/demo/competitions/long-jump.svg', '2024-08-02T10:00:00', '2024-08-03T22:00:00'),
    (N'Прыжок в высоту', N'Лёгкая атлетика — прыжок в высоту', N'Квалификация и финал.', N'/images/placeholders/competition-default.svg', '2024-08-04T10:00:00', '2024-08-05T22:00:00'),
    (N'Толкание ядра', N'Лёгкая атлетика — толкание ядра', N'Квалификация и финал.', N'/images/placeholders/competition-default.svg', '2024-08-01T10:00:00', '2024-08-02T22:00:00'),
    (N'Метание копья', N'Лёгкая атлетика — метание копья', N'Квалификация и финал.', N'/images/placeholders/competition-default.svg', '2024-08-09T10:00:00', '2024-08-10T22:00:00'),
    (N'Марафон', N'Лёгкая атлетика — марафон', N'Дистанция 42.195 км.', N'/images/placeholders/competition-default.svg', '2024-08-10T08:00:00', '2024-08-10T14:00:00');

--#region Вставка Competition и привязка событий к соревнованиям(EventsCompetitions)
DECLARE @newCompetitions TABLE (competitionId INT NOT NULL);

INSERT INTO Competition
    (idSportSubType, title, [description], photoUrl, dateStart, dateEnd)
OUTPUT INSERTED.id INTO @newCompetitions(competitionId)
SELECT
    sst.id,
    i.CompetitionTitle,
    i.CompetitionDesc,
    i.PhotoUrl,
    i.DateStart,
    i.DateEnd
FROM @toInsert i
    JOIN SportSubType sst ON sst.title = i.SportSubTypeTitle;

INSERT INTO EventsCompetitions
    (idEvent, idCompetition)
SELECT @eventId, nc.competitionId
FROM @newCompetitions nc;
   

--#endregion 

--#endregion 

--#endregion 

    COMMIT;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT> 0 ROLLBACK;

    -- Вывод ошибки
    SELECT
    ERROR_NUMBER()   AS ErrorNumber,
    ERROR_SEVERITY() AS ErrorSeverity,
    ERROR_STATE()    AS ErrorState,
    ERROR_LINE()     AS ErrorLine,
    ERROR_MESSAGE()  AS ErrorMessage;
END CATCH

--#endregion

-- #region Контакты(Contacts)

INSERT INTO Contact
    (firstname, lastname, middlename, birthDate, sex, phone, email, photoUrl)
VALUES
    (N'Алексей', N'Смирнов', N'Андреевич', '1994-12-05', 1, '+7 999 101-01-01', 'alexey.smirnov@gmail.com', N'/images/demo/users/administrator.svg'),
    (N'Дмитрий', N'Кузнецов', N'Сергеевич', '1996-03-18', 1, '+7 999 202-02-02', 'd.kuznetsov@mail.ru', N'/images/demo/users/administrator.svg'),
    (N'Максим', N'Попов', N'Игоревич', '1989-07-11', 1, '+7 999 303-03-03', 'max.popov@yandex.ru', N'/images/demo/users/organizer.svg'),
    (N'Сергей', N'Васильев', N'Олегович', '1983-09-21', 1, '+7 999 404-04-04', 'sergey.vasiliev@gmail.com', N'/images/demo/users/organizer.svg'),
    (N'Андрей', N'Новиков', N'Дмитриевич', '1997-02-06', 1, '+7 999 505-05-05', 'andrey.novikov@mail.ru', N'/images/demo/users/organizer.svg'),
    (N'Роман', N'Фёдоров', N'Алексеевич', '1991-04-14', 1, '+7 999 606-06-06', 'roman.fedorov@yandex.ru', N'/images/demo/users/participant.svg'),
    (N'Владимир', N'Морозов', N'Павлович', '1979-12-30', 1, '+7 999 707-07-07', 'v.morozov@gmail.com', N'/images/demo/users/participant.svg'),
    (N'Евгений', N'Волков', N'Николаевич', '1986-09-07', 1, '+7 999 808-08-08', 'evgeny.volkov@mail.ru', N'/images/demo/users/participant.svg'),
    (N'Татьяна', N'Соколова', N'Игоревна', '1998-02-25', 0, '+7 999 909-09-09', 't.sokolova@gmail.com', N'/images/demo/users/participant.svg'),
    (N'Елена', N'Лебедева', N'Андреевна', '1993-11-10', 0, '+7 999 111-11-22', 'elena.lebedeva@mail.ru', N'/images/demo/users/participant.svg'),
    (N'Мария', N'Козлова', N'Сергеевна', '2000-03-08', 0, '+7 999 222-22-33', 'maria.kozlova@yandex.ru', N'/images/demo/users/participant.svg'),
    (N'Наталья', N'Павлова', N'Владимировна', '1988-01-19', 0, '+7 999 333-33-44', 'n.pavlova@gmail.com', N'/images/demo/users/participant.svg'),
    (N'Ирина', N'Семёнова', N'Александровна', '1995-05-27', 0, '+7 999 444-44-55', 'irina.semenova@mail.ru', N'/images/demo/users/participant.svg'),
    (N'Ксения', N'Голубева', N'Романовна', '2001-09-16', 0, '+7 999 555-55-66', 'ksenia.golubeva@yandex.ru', N'/images/demo/users/participant.svg'),
    (N'Юлия', N'Виноградова', N'Олеговна', '1990-05-03', 0, '+7 999 666-66-77', 'y.vinogradova@gmail.com', N'/images/demo/users/participant.svg'),
    (N'Олег', N'Богданов', N'Максимович', '1985-11-28', 1, '+7 999 777-77-88', 'oleg.bogdanov@mail.ru', N'/images/demo/users/viewer.svg'),
    (N'Никита', N'Орлов', N'Денисович', '2002-06-13', 1, '+7 999 888-88-99', 'nikita.orlov@yandex.ru', N'/images/demo/users/viewer.svg'),
    (N'Артур', N'Захаров', N'Михайлович', '1987-01-04', 1, '+7 999 999-99-00', 'arthur.zakharov@gmail.com', N'/images/demo/users/viewer.svg'),
    (N'Виктория', N'Макарова', N'Евгеньевна', '1996-12-22', 0, '+7 999 121-21-21', 'v.makarova@mail.ru', N'/images/demo/users/viewer.svg'),
    (N'Дарья', N'Тарасова', N'Константиновна', '1997-08-07', 0, '+7 999 131-31-31', 'daria.tarasova@yandex.ru', N'/images/demo/users/viewer.svg');

-- #endregion

-- #region Роли (Role)
INSERT into Role
    (title)
VALUES
    (N'Администратор'),
    (N'Организатор'),
    (N'Участник'),
    (N'Пользователь');
--#endregion

-- #region Пользователи (User)
INSERT INTO [User]
    (idContact, idRole)
SELECT c.id,
    CASE 
            WHEN c.id BETWEEN 1 AND 2 THEN 1  -- Администратор
            WHEN c.id BETWEEN 3 AND 5 THEN 2  -- Организатор
            WHEN c.id BETWEEN 6 AND 15 THEN 3 -- Участник
            ELSE 4                            -- Пользователь
    END
FROM Contact c;
--#endregion

-- #region Участники (Participants) //пока случайная раскидка
;WITH
    ParticipantsToInsert
    AS
    (
        SELECT
            c.id AS ContactId,
            ROW_NUMBER() OVER (ORDER BY c.id) AS rn
        FROM Contact c
            JOIN [User] u ON u.idContact = c.id
            JOIN [Role] r ON r.id = u.idRole
        WHERE r.title = N'Участник'
    ),

    SchoolsNumbered
    AS
    (
        SELECT
            id AS SchoolId,
            ROW_NUMBER() OVER (ORDER BY id) AS rn
        FROM School
    ),

    SchoolCount
    AS
    (
        SELECT COUNT(*) AS cnt
        FROM School
    )

INSERT INTO Participant
    (idSchool, idContact)
SELECT
    s.SchoolId,
    p.ContactId
FROM ParticipantsToInsert p
CROSS JOIN SchoolsNumbered s
CROSS JOIN SchoolCount sc
WHERE (p.rn - 1) % sc.cnt + 1 = s.rn
    AND NOT EXISTS (
    SELECT 1
    FROM Participant part
    WHERE part.idContact = p.ContactId
);
-- #endregion

-- #region Привязка участников к соревнованиям (ParticipantsCompetitions)
INSERT INTO ParticipantsCompetitions
    (idCompetition, idParticipant)
SELECT
    comp.id,
    p.id
FROM Participant p
    JOIN SchoolsSportsSubTypes sss
    ON sss.idSchool = p.idSchool
    JOIN Competition comp
    ON comp.idSportSubType = sss.idSportSubType
WHERE NOT EXISTS (
        SELECT 1
FROM ParticipantsCompetitions pc
WHERE pc.idCompetition = comp.id
    AND pc.idParticipant = p.id
);
-- #endregion
