GO

IF NOT EXISTS (
    SELECT
        name
    FROM
        sys.databases
    WHERE
        name = N'sports-competitions'
)
BEGIN
    CREATE DATABASE [sports-competitions]
END

USE [sports-competitions]