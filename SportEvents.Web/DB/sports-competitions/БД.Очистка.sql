-- Удаление всех внешних ключей
DECLARE @constraintName NVARCHAR(128)
DECLARE @tableName NVARCHAR(128)
DECLARE @sql NVARCHAR(MAX)

DECLARE constraints_cursor CURSOR FOR
SELECT 
    fk.name AS constraintName,
    t.name AS tableName
FROM 
    sys.foreign_keys AS fk
INNER JOIN 
    sys.tables AS t ON fk.parent_object_id = t.object_id

OPEN constraints_cursor

FETCH NEXT FROM constraints_cursor INTO @constraintName, @tableName
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = 'ALTER TABLE [' + @tableName + '] DROP CONSTRAINT [' + @constraintName + ']'
    EXEC sp_executesql @sql
    PRINT 'Dropped constraint ' + @constraintName + ' from table ' + @tableName
    FETCH NEXT FROM constraints_cursor INTO @constraintName, @tableName
END

CLOSE constraints_cursor
DEALLOCATE constraints_cursor

-- Удаление всех таблиц
DECLARE @dropTableSql NVARCHAR(MAX)

DECLARE tables_cursor CURSOR FOR
SELECT 
    name 
FROM 
    sys.tables

OPEN tables_cursor

FETCH NEXT FROM tables_cursor INTO @tableName
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @dropTableSql = 'DROP TABLE [' + @tableName + ']'
    EXEC sp_executesql @dropTableSql
    PRINT 'Dropped table ' + @tableName
    FETCH NEXT FROM tables_cursor INTO @tableName
END

CLOSE tables_cursor
DEALLOCATE tables_cursor

-- Удаление всех представлений
DECLARE @viewName NVARCHAR(128)

DECLARE views_cursor CURSOR FOR
SELECT 
    name 
FROM 
    sys.views

OPEN views_cursor

FETCH NEXT FROM views_cursor INTO @viewName
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = 'DROP VIEW [' + @viewName + ']'
    EXEC sp_executesql @sql
    PRINT 'Dropped view ' + @viewName
    FETCH NEXT FROM views_cursor INTO @viewName
END

CLOSE views_cursor
DEALLOCATE views_cursor
