CREATE TABLE [Sport]
(
  [id] int IDENTITY (1, 1) ,
  [title] nvarchar(255) NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [SportType]
(
  [id] int IDENTITY (1, 1) ,
  [idSport] int NOT NULL ,
  [title] nvarchar(255) NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [SportSubtype]
(
  [id] int IDENTITY (1, 1) ,
  [idSportType] int NOT NULL ,
  [title] nvarchar(255) NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [School]
(
  [id] int IDENTITY (1, 1) ,
  [title] nvarchar(255) NOT NULL ,
  [description] nvarchar(255) ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [SchoolsSportsSubTypes]
(
  [id] int IDENTITY (1, 1) ,
  [idSportSubType] int NOT NULL ,
  [idSchool] int NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [Competition]
(
  [id] int IDENTITY (1, 1) ,
  [idSportSubType] int NOT NULL ,
  [title] nvarchar(255) NOT NULL ,
  [description] nvarchar(255) ,
  [dateStart] datetime ,
  [dateEnd] datetime ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [Event]
(
  [id] int IDENTITY (1, 1) ,
  [title] nvarchar(255) NOT NULL ,
  [description] nvarchar(255) ,
  [dateStart] datetime ,
  [dateEnd] datetime ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [EventsCompetitions]
(
  [id] int IDENTITY (1, 1) ,
  [idEvent] int NOT NULL ,
  [idCompetition] int NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [Participant]
(
  [id] int IDENTITY (1, 1) ,
  [idSchool] int ,
  [idContact] int NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [ParticipantsCompetitions]
(
  [id] int NOT NULL IDENTITY (1, 1) ,
  [idCompetition] int NOT NULL ,
  [idParticipant] int NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [Contact]
(
  [id] int NOT NULL IDENTITY (1, 1) ,
  [firstname] nvarchar(255) NOT NULL ,
  [lastname] nvarchar(255) NOT NULL ,
  [middlename] nvarchar(255) ,
  [birthDate] datetime NOT NULL ,
  [sex] bit NOT NULL ,
  [phone] nvarchar(20) ,
  [email] nvarchar(255) NOT NULL ,
  [passwordHash] nvarchar(255) ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [User]
(
  [id] int NOT NULL IDENTITY (1, 1) ,
  [idContact] int ,
  [idRole] int ,
  [dateCreated] datetime NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [Role]
(
  [id] int NOT NULL IDENTITY (1, 1) ,
  [title] nvarchar(255) NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [Image]
(
  [id] int IDENTITY (1, 1) ,
  [url] nvarchar(255) ,
  [title] nvarchar(255) ,
  [description] nvarchar(255) ,
  [dateCreated] datetime NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [ImagesUsers]
(
  [id] int IDENTITY (1, 1) ,
  [idImage] int NOT NULL ,
  [idContact] int NOT NULL ,
  PRIMARY KEY ([id])
) ON [PRIMARY]
GO

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