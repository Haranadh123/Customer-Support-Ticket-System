IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CustomerSupportDb')
BEGIN
    CREATE DATABASE CustomerSupportDb;
END
GO

USE CustomerSupportDb;
GO

-- Users table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role INT NOT NULL -- 1: User, 2: Admin
);
END
GO

-- Tickets table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tickets]') AND type in (N'U'))
BEGIN
CREATE TABLE Tickets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketNumber NVARCHAR(20) NOT NULL UNIQUE,
    Subject NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Priority INT NOT NULL, -- 1: Low, 2: Medium, 3: High
    [Status] INT NOT NULL DEFAULT 1, -- 1: Open, 2: In Progress, 3: Closed
    CreatedDate DATETIME NOT NULL,
    CreatedByUserId INT NOT NULL,
    AssignedToAdminId INT NULL,
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id),
    FOREIGN KEY (AssignedToAdminId) REFERENCES Users(Id)
);
END
GO

-- TicketComments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TicketComments]') AND type in (N'U'))
BEGIN
CREATE TABLE TicketComments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL,
    CommentText NVARCHAR(MAX) NOT NULL,
    IsInternal BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL,
    CreatedByUserId INT NOT NULL,
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);
END
GO

-- TicketStatusHistory table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TicketStatusHistory]') AND type in (N'U'))
BEGIN
CREATE TABLE TicketStatusHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TicketId INT NOT NULL,
    OldStatus INT NOT NULL,
    NewStatus INT NOT NULL,
    ChangedByUserId INT NOT NULL,
    ChangedDate DATETIME NOT NULL,
    FOREIGN KEY (TicketId) REFERENCES Tickets(Id),
    FOREIGN KEY (ChangedByUserId) REFERENCES Users(Id)
);
END
GO

-- Seed initial users
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'user1')
    INSERT INTO Users (Username, Password, Role) VALUES ('user1', 'password123', 1);

IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'user2')
    INSERT INTO Users (Username, Password, Role) VALUES ('user2', 'password123', 1);

IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin1')
    INSERT INTO Users (Username, Password, Role) VALUES ('admin1', 'password123', 2);

IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin2')
    INSERT INTO Users (Username, Password, Role) VALUES ('admin2', 'password123', 2);
GO
