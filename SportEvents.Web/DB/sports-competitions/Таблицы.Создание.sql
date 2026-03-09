use "sports-competitions"
--Вид спорта
create table
    Sport (
        id Int IDENTITY (1, 1),
        title NVARCHAR (255) NOT NULL,
        PRIMARY KEY (id)
    );
--Раздел внутри вида спорта
create table
    SportType (
        id Int IDENTITY (1, 1),
        idSport Int NOT NULL,
        title NVARCHAR (255) NOT NULL,
        PRIMARY KEY (id)
    )
--Дисциплина
create table
    SportSubType (
        id Int IDENTITY (1, 1),
        idSportType INT NOT NULL,
        title NVARCHAR (255) NOT NULL,
        PRIMARY KEY (id)
    )
create table
    SchoolsSportsSubtypes (
        id int IDENTITY (1, 1),
        idSportSubType int NOT NULL,
        idSchool int NOT NULL,
        PRIMARY KEY (id)
    )
create table
    School (
        id int IDENTITY (1, 1),
        title NVARCHAR (255) NOT NULL,
        [description] NVARCHAR (255),
        PRIMARY KEY (id)
    )
create table
    Participant (
        id int IDENTITY (1, 1),
        idSchool int NOT NULL,
        idContact int NOT NULL,
        PRIMARY KEY (id)
    )
create table
    ParticipantsCompetitions (
        id int IDENTITY (1, 1),
        idCompetition int NOT NULL,
        idParticipant int NOT NULL,
        PRIMARY KEY (id)
    )
create table
    Competition (
        id int IDENTITY (1, 1),
        idSportSubType int NOT NULL,
        title NVARCHAR (255) NOT NULL,
        [description] NVARCHAR (255),
        dateStart datetime,
        dateEnd datetime,
        PRIMARY KEY (id)
    )
create table
    EventsCompetitions (
        id int IDENTITY (1, 1),
        idEvent int,
        idCompetition int,
        PRIMARY KEY (id)
    )
create table
    [Event] (
        id int IDENTITY (1, 1),
        title NVARCHAR (255) NOT NULL,
        [description] NVARCHAR (255),
        dateStart DATETIME,
        dateEnd DATETIME,
        PRIMARY KEY (id)
    )
create table
    [Image] (
        id int IDENTITY (1, 1),
        url NVARCHAR (255) NOT NULL,
        title NVARCHAR (255) NOT NULL,
        [description] NVARCHAR (255),
        dateCreated datetime NOT NULL,
        PRIMARY KEY (id)
    )
create table
    [ImagesUsers] (
        id int IDENTITY (1, 1),
        idImage int NOT NULL,
        idContact int NOT NULL,
        PRIMARY KEY (id)
    )
create table
    [Contact] (
        id int IDENTITY (1, 1),
        firstname NVARCHAR (255) NOT NULL,
        lastname NVARCHAR (255) NOT NULL,
        middlename NVARCHAR (255),
        age int NOT NULL,
        sex bit NOT NULL,
        phone NVARCHAR (20) NOT NULL,
        email NVARCHAR (255),
        passwordHash NVARCHAR (255),
        PRIMARY KEY (id)
    )
create table
    [User] (
        id int IDENTITY (1, 1),
        idContact int NOT NULL,
        idRole int NOT NULL,
        PRIMARY KEY (id)
    )
create table
    [Role] (
        id int IDENTITY (1, 1),
        title NVARCHAR (255) NOT NULL,
        PRIMARY KEY (id)
    )

ALTER TABLE [SportType] ADD FOREIGN KEY (idSport) REFERENCES [Sport] ([id]);
				
ALTER TABLE [SportSubtype] ADD FOREIGN KEY (idSportType) REFERENCES [SportType] ([id]);
				
ALTER TABLE [SchoolsSportsSubTypes] ADD FOREIGN KEY (idSportSubType) REFERENCES [SportSubtype] ([id]);
				
ALTER TABLE [SchoolsSportsSubTypes] ADD FOREIGN KEY (idSchool) REFERENCES [School] ([id]);
				
ALTER TABLE [Competition] ADD FOREIGN KEY (idSportSubType) REFERENCES [SportSubtype] ([id]);
				
ALTER TABLE [EventsCompetitions] ADD FOREIGN KEY (idEvent) REFERENCES [Event] ([id]);
				
ALTER TABLE [EventsCompetitions] ADD FOREIGN KEY (idCompetition) REFERENCES [Competition] ([id]);
				
ALTER TABLE [Participant] ADD FOREIGN KEY (idSchool) REFERENCES [School] ([id]);
				
ALTER TABLE [Participant] ADD FOREIGN KEY (idContact) REFERENCES [Contact] ([id]);
				
ALTER TABLE [ParticipantsCompetitions] ADD FOREIGN KEY (idCompetition) REFERENCES [Competition] ([id]);
				
ALTER TABLE [ParticipantsCompetitions] ADD FOREIGN KEY (idParticipant) REFERENCES [Participant] ([id]);
				
ALTER TABLE [User] ADD FOREIGN KEY (idContact) REFERENCES [Contact] ([id]);
				
ALTER TABLE [User] ADD FOREIGN KEY (idRole) REFERENCES [Role] ([id]);
				
ALTER TABLE [ImagesUsers] ADD FOREIGN KEY (idImage) REFERENCES [Image] ([id]);
				
ALTER TABLE [ImagesUsers] ADD FOREIGN KEY (idContact) REFERENCES [Contact] ([id]);
