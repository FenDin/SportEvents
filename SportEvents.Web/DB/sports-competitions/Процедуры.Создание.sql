CREATE OR ALTER PROCEDURE dbo.CheckDbObjectExists
    @objectType NVARCHAR(50),
    -- CHECK, DEFAULT, INDEX, FK
    @ObjectName SYSNAME,
    @TableName SYSNAME = NULL,
	@exists BIT OUTPUT
-- для INDEX можно указать таблицу
AS
BEGIN
    SET NOCOUNT ON;



    IF @objectType = 'CHECK'
    BEGIN
        IF EXISTS(
        SELECT 1
        FROM sys.check_constraints
        WHERE name = @ObjectName
    )
    SET @exists = 1;
    END


    ELSE IF @ObjectType = 'DEFAULT'
    BEGIN
        IF EXISTS (SELECT 1
        FROM sys.default_constraints
        WHERE name = @ObjectName)
            SET @exists = 1;
    END

        ELSE IF @ObjectType = 'INDEX'
    BEGIN
        IF EXISTS (
            SELECT 1
        FROM sys.indexes i
        WHERE i.name = @ObjectName
            AND (@TableName IS NULL OR i.object_id = OBJECT_ID(@TableName))
        )
            SET @exists = 1;
    END

        ELSE IF @ObjectType = 'FK'
    BEGIN
        IF EXISTS (SELECT 1
        FROM sys.foreign_keys
        WHERE name = @ObjectName)
            SET @exists = 1;
    END


END
GO