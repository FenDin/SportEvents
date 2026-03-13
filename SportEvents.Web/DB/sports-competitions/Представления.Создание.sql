GO
-- #region vContact (вся инфа о контактах)

CREATE OR ALTER VIEW vContact
AS
    SELECT
        c.id       AS ContactId,
        c.lastname AS LastName,
        c.firstname AS FirstName,
        c.middlename AS MiddleName,
        c.birthDate AS BirthDate,
        c.sex AS Sex,
        c.phone    AS Phone,
        c.email    AS Email,
        c.passwordHash AS PasswordHash
    FROM [Contact] c

-- #endregion
GO

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

        vC.*

    FROM Participant p
        JOIN vContact vC ON vC.ContactId = p.idContact
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
        u.dateCreated AS UserDateCreated,
        r.id AS RoleId,
        r.title    AS RoleTitle,

        vC.*

    FROM [User] u
        JOIN [Role] r ON r.id = u.idRole
        JOIN vContact vC ON vC.ContactId = u.idContact

GO        

-- #endregion   