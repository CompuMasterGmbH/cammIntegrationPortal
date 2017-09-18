INSERT INTO dbo.Log_Users (ID_USER, Type, VALUE) 
SELECT ID, 'IsDeletedUser', '0' 
FROM dbo.Benutzer 
WHERE ID NOT IN (SELECT ID_USER FROM dbo.Log_Users WHERE Type = 'IsDeletedUser')
