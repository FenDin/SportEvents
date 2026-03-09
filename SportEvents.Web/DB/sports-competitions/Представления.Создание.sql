-- #region vSportSubTypesFull:Вид спорта → Раздел вида спорта → Дисциплина

CREATE OR ALTER VIEW vSportSubTypesFull
AS
    SELECT
        s.id   AS SportId,
        s.title AS SportTitle,
        st.id  AS SportTypeId,
        st.title AS SportTypeTitle,
        sst.id AS SportSubTypeId,
        sst.title AS SportSubTypeTitle
    FROM Sport s
        JOIN SportType st ON st.idSport = s.id
        JOIN SportSubType sst ON sst.idSportType = st.id;

GO

-- #endregion

-- #region vCompetitionsFull:Соревнования + полный путь дисциплины

CREATE OR ALTER VIEW vCompetitionsFull
AS
    SELECT
        c.id            AS CompetitionId,
        c.title         AS CompetitionTitle,
        c.[description] AS CompetitionDescription,
        c.dateStart     AS CompetitionDateStart,
        c.dateEnd       AS CompetitionDateEnd,

        v.*

    FROM Competition c
        JOIN vSportSubTypesFull v
        ON v.SportSubTypeId = c.idSportSubType

GO
-- #endregion

-- #region vEventsCompetitionsFull:События + соревнования (для страницы события и календаря)

CREATE OR ALTER VIEW vEventsCompetitionsFull
AS
    SELECT
        e.id        AS EventId,
        e.title     AS EventTitle,
        e.dateStart AS EventDateStart,
        e.dateEnd   AS EventDateEnd,

        c.id        AS CompetitionId,
        c.title     AS CompetitionTitle,
        c.dateStart AS CompetitionDateStart,
        c.dateEnd   AS CompetitionDateEnd,

        v.*

    FROM Event e
        JOIN EventsCompetitions ec ON ec.idEvent = e.id
        JOIN Competition c ON ec.idCompetition = c.id
        JOIN vSportSubTypesFull v ON v.SportSubTypeId = c.idSportSubType

GO

-- #endregion

-- #region vSchoolSportSubTypes:Школа + перечень дисциплин (список “что преподаёт школа”)

CREATE OR ALTER VIEW vSchoolSportSubTypes
AS
    SELECT
        sch.id      AS SchoolId,
        sch.title   AS SchoolTitle,
        sch.[description] AS SchoolDescription,

        v.*

    FROM School sch
        JOIN SchoolsSportsSubTypes ssst ON ssst.idSchool = sch.id
        JOIN vSportSubTypesFull v ON v.SportSubTypeId = ssst.idSportSubType

GO

-- #endregion

-- #region vParticipantsFull:Участник (Participant) + контакт + школа

CREATE OR ALTER VIEW vParticipantsFull
AS
    SELECT
        p.id AS ParticipantId,

        sch.id AS SchoolId,
        sch.title AS SchoolTitle,
        sch.description AS SchoolDescription,

        c.id         AS ContactId,
        c.lastname   AS LastName,
        c.firstname  AS FirstName,
        c.middlename AS MiddleName,
        c.age        AS Age,
        c.sex        AS Sex,
        c.phone      AS Phone,
        c.email      AS Email

    FROM Participant p
        JOIN Contact c ON c.id = p.idContact
        JOIN School sch ON sch.id = p.idSchool

GO

-- #endregion

-- #region vCompetitionParticipants:Участники соревнований (для списка участников конкретного соревнования)

CREATE OR ALTER VIEW vCompetitionParticipants
AS
    SELECT
        vC.*,
        vP.*

    FROM ParticipantsCompetitions pc
        inner join vCompetitionsFull vC ON pc.idCompetition = vC.CompetitionId
        inner join vParticipantsFull vP ON pc.idParticipant = vP.ParticipantId

GO

-- #endregion

-- #region vUsersFull:Пользователи + роль + контакт (для админки и авторизации)

CREATE OR ALTER VIEW vUsersFull
AS
    SELECT
        u.id       AS UserId,
        r.id       AS RoleId,
        r.title    AS RoleTitle,
        c.id       AS ContactId,
        c.email    AS Email,
        c.phone    AS Phone,
        c.lastname AS LastName,
        c.firstname AS FirstName,
        c.middlename AS MiddleName,
        c.passwordHash AS PasswordHash

    FROM [User] u
        JOIN [Role] r ON r.id = u.idRole
        JOIN Contact c ON c.id = u.idContact

GO        

-- #endregion   