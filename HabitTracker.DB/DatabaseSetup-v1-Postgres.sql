-- Create Database if it doesn't exist 
-- (in PostgreSQL this typically needs to be done separately, but including equivalent)
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'habittracker') THEN
        PERFORM dblink_exec('dbname=' || current_database(), 'CREATE DATABASE habittracker');
    END IF;
END $$;

-- Connect to the database
\c habittracker;

-- Drop tables if they exist with proper order to handle dependencies
DROP TABLE IF EXISTS "Streaks";
DROP TABLE IF EXISTS "Habits";

-- Create Habits table
CREATE TABLE "Habits" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500),
    "CreatedDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create Streaks table
CREATE TABLE "Streaks" (
    "Id" SERIAL PRIMARY KEY,
    "HabitId" INTEGER NOT NULL,
    "HabitCompletedDate" TIMESTAMP NOT NULL,
    "CreatedDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_Streaks_Habits" FOREIGN KEY ("HabitId") REFERENCES "Habits"("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX "IX_Streaks_HabitCompletedDate" ON "Streaks"("HabitCompletedDate");
CREATE INDEX "IX_Streaks_HabitId" ON "Streaks"("HabitId");

-- -- Create view for habit streaks
-- CREATE OR REPLACE VIEW vw_habit_streaks AS
-- WITH consecutive_dates AS (
--     SELECT 
--         h.id AS habit_id,
--         h.name AS habit_name,
--         s.habit_completed_date,
--         s.habit_completed_date - LEAD(s.habit_completed_date) OVER (
--             PARTITION BY h.id ORDER BY s.habit_completed_date
--         ) AS days_difference
--     FROM habits h
--     LEFT JOIN streaks s ON h.id = s.habit_id
-- )
-- SELECT 
--     habit_id,
--     habit_name,
--     COUNT(CASE WHEN days_difference = INTERVAL '1 day' OR days_difference IS NULL THEN 1 END) AS current_streak
-- FROM consecutive_dates
-- WHERE habit_completed_date >= CURRENT_DATE - INTERVAL '30 days'
-- GROUP BY habit_id, habit_name;

-- -- Create function for getting habit streak (PostgreSQL uses functions instead of stored procedures)
-- CREATE OR REPLACE FUNCTION get_habit_streak(habit_id_param INTEGER)
-- RETURNS TABLE (current_streak BIGINT) AS $$
-- BEGIN
--     RETURN QUERY
--     WITH consecutive_dates AS (
--         SELECT 
--             habit_completed_date,
--             habit_completed_date - LEAD(habit_completed_date) OVER (
--                 ORDER BY habit_completed_date
--             ) AS days_difference
--         FROM streaks
--         WHERE habit_id = habit_id_param
--     )
--     SELECT 
--         COUNT(CASE WHEN days_difference = INTERVAL '1 day' OR days_difference IS NULL THEN 1 END)
--     FROM consecutive_dates
--     WHERE habit_completed_date >= CURRENT_DATE - INTERVAL '30 days';
-- END;
-- $$ LANGUAGE plpgsql;

-- -- Insert sample data only if tables are empty
-- DO $$
-- DECLARE
--     habit_id INTEGER;
--     counter INTEGER := 0;
-- BEGIN
--     IF NOT EXISTS (SELECT 1 FROM habits LIMIT 1) THEN
--         INSERT INTO habits (name, description)
--         VALUES 
--             ('Morning Yoga', '15 minutes of yoga every morning'),
--             ('Reading', 'Read for 30 minutes daily'),
--             ('Meditation', '10 minutes mindfulness practice');
        
--         -- Insert sample streak data for the first habit
--         SELECT id INTO habit_id FROM habits WHERE name = 'Morning Yoga' LIMIT 1;
        
--         WHILE counter < 5 LOOP
--             INSERT INTO streaks (habit_id, habit_completed_date)
--             VALUES (habit_id, (CURRENT_DATE - (counter * INTERVAL '1 day'))::TIMESTAMP);
            
--             counter := counter + 1;
--         END LOOP;
--     END IF;
-- END $$;