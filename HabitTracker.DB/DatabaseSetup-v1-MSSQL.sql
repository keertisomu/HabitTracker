-- Create Database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HabitTracker')
BEGIN
    CREATE DATABASE HabitTracker;
END
GO

USE HabitTracker;
GO

-- Drop existing foreign key if exists before dropping tables
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Streaks_Habits')
BEGIN
    ALTER TABLE Streaks DROP CONSTRAINT FK_Streaks_Habits;
END
GO

-- Create Habits table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Habits')
BEGIN
    CREATE TABLE Habits (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Habits table created.';
END
GO

-- Create Streaks table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Streaks')
BEGIN
    CREATE TABLE Streaks (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        HabitId INT NOT NULL,
        HabitCompletedDate DATETIME2 NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Streaks table created.';
END
GO

-- Add foreign key if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Streaks_Habits')
BEGIN
    ALTER TABLE Streaks
    ADD CONSTRAINT FK_Streaks_Habits 
    FOREIGN KEY (HabitId) REFERENCES Habits(Id) ON DELETE CASCADE;
    PRINT 'Foreign key constraint added.';
END
GO

-- Create or alter indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Streaks_HabitSatisfiedDate' AND object_id = OBJECT_ID('Streaks'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Streaks_HabitSatisfiedDate
    ON Streaks(HabitCompletedDate);
    PRINT 'Index IX_Streaks_HabitSatisfiedDate created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Streaks_HabitId' AND object_id = OBJECT_ID('Streaks'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Streaks_HabitId
    ON Streaks(HabitId);
    PRINT 'Index IX_Streaks_HabitId created.';
END
GO

-- Create or alter view for habit streaks
CREATE OR ALTER VIEW vw_HabitStreaks
AS
WITH ConsecutiveDates AS (
    SELECT 
        h.Id AS HabitId,
        h.Name AS HabitName,
        s.HabitCompletedDate,
        DATEDIFF(DAY, s.HabitCompletedDate, LEAD(s.HabitCompletedDate) 
            OVER (PARTITION BY h.Id ORDER BY s.HabitCompletedDate)) AS DaysDifference
    FROM Habits h
    LEFT JOIN Streaks s ON h.Id = s.HabitId
)
SELECT 
    HabitId,
    HabitName,
    COUNT(CASE WHEN DaysDifference = -1 OR DaysDifference IS NULL THEN 1 END) AS CurrentStreak
FROM ConsecutiveDates
WHERE HabitCompletedDate >= DATEADD(DAY, -30, GETUTCDATE())
GROUP BY HabitId, HabitName;
GO

-- Create or alter stored procedure for getting habit streak
CREATE OR ALTER PROCEDURE sp_GetHabitStreak
    @HabitId INT
AS
BEGIN
    SET NOCOUNT ON;

    WITH ConsecutiveDates AS (
        SELECT 
            HabitCompletedDate,
            DATEDIFF(DAY, HabitCompletedDate, LEAD(HabitCompletedDate) 
                OVER (ORDER BY HabitCompletedDate)) AS DaysDifference
        FROM Streaks
        WHERE HabitId = @HabitId
    )
    SELECT 
        COUNT(CASE WHEN DaysDifference = -1 OR DaysDifference IS NULL THEN 1 END) AS CurrentStreak
    FROM ConsecutiveDates
    WHERE HabitCompletedDate >= DATEADD(DAY, -30, GETUTCDATE());
END;
GO

-- Insert sample data only if tables are empty
IF NOT EXISTS (SELECT TOP 1 1 FROM Habits)
BEGIN
    INSERT INTO Habits (Name, Description)
    VALUES 
        ('Morning Yoga', '15 minutes of yoga every morning'),
        ('Reading', 'Read for 30 minutes daily'),
        ('Meditation', '10 minutes mindfulness practice');
    PRINT 'Sample habits inserted.';

    -- Insert sample streak data for the first habit
    DECLARE @HabitId INT = (SELECT TOP 1 Id FROM Habits WHERE Name = 'Morning Yoga');
    DECLARE @Counter INT = 0;

    WHILE @Counter < 5
    BEGIN
        INSERT INTO Streaks (HabitId, HabitCompletedDate)
        VALUES (@HabitId, DATEADD(DAY, -@Counter, CAST(GETUTCDATE() AS DATE)));
        
        SET @Counter = @Counter + 1;
    END
    PRINT 'Sample streak data inserted.';
END
GO

-- Print completion message
PRINT 'Database setup completed successfully.';
GO