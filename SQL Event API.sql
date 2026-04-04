-- USE EventManagement
-- GO
-- CREATE TABLE Event.Users(
--     UserId INT PRIMARY KEY IDENTITY(1,1),
--     FirstName NVARCHAR(50) NOT NULL,
--     LastName NVARCHAR(50) NOT NULL,
--     Email NVARCHAR(100) NOT NULL UNIQUE,
--     PasswordHash NVARCHAR(255) NOT NULL,
--     IsAdmin BIT NOT NULL DEFAULT 0,
--     CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
-- )
-- GO
-- CREATE TABLE Event.Categories(
--     CategoryId INT PRIMARY KEY IDENTITY(1,1),
--     NameCategory NVARCHAR(100) NOT NULL UNIQUE
-- )
-- GO
-- CREATE TABLE Event.Events(
--     EventId INT PRIMARY KEY IDENTITY(1,1),
--     CategoryId INT NOT NULL,
--     MaxAttendees INT NOT NULL CHECK (MaxAttendees > 0),
--     Title NVARCHAR(200) NOT NULL,
--     Description NVARCHAR(MAX) NOT NULL,
--     Location NVARCHAR(200) NOT NULL,
--     StartDate DATETIME NOT NULL,
--     EndDate DATETIME NOT NULL,
--     CreatedBy INT NOT NULL,
--     CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--     FOREIGN KEY (CategoryId) REFERENCES Event.Categories(CategoryId),
--     FOREIGN KEY (CreatedBy) REFERENCES Event.Users(UserId)
-- )
-- GO
-- CREATE TABLE Event.Bookings(
--     BookingId INT PRIMARY KEY IDENTITY(1,1),
--     EventId INT NOT NULL,
--     UserId INT NOT NULL,
--     BookingDate DATETIME NOT NULL DEFAULT GETDATE(),
--     Status TINYINT NOT NULL DEFAULT 1, -- 1: Confirmed, 2: Cancelled, 3: Attended
--     FOREIGN KEY (EventId) REFERENCES Event.Events(EventId),
--     FOREIGN KEY (UserId) REFERENCES Event.Users(UserId)
-- )
GO
INSERT INTO Event.Users
    (
    FirstName ,
    LastName ,
    Email ,
    PasswordHash ,
    IsAdmin ,
    CreatedAt )
VALUES(
        'Admin', 'User', 'admin@example.com', 'hashed_password', 1, GETDATE()
    )
GO
SELECT * FROM Event.Users
GO
SELECT [EventId],
    [CategoryId],
    [MaxAttendees],
    [Title],
    [Description],
    [Location],
    [StartDate],
    [EndDate],
    [CreatedBy],
    [CreatedAt]
FROM Event.Events
GO
SELECT [CategoryId],
       [NameCategory]
FROM Event.Categories
GO
SELECT e.*,c.NameCategory FROM Event.Events AS e JOIN Event.Categories AS c ON e.CategoryId = c.CategoryId
GO
INSERT INTO Event.Categories (NameCategory) 
VALUES ('Technology'), ('Music'), ('Sports'), ('Education');
GO
