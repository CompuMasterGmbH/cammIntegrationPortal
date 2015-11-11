IF EXISTS (select * from sys.indexes where object_id = object_id('dbo.Log_eMailMessages') and name = 'IX_Log_eMailMessages_1')
DROP INDEX IX_Log_eMailMessages_1 ON dbo.Log_eMailMessages
GO
CREATE NONCLUSTERED INDEX IX_Log_eMailMessages_1 ON dbo.Log_eMailMessages
	(
	DateTime
	) 
GO
IF EXISTS (select * from sys.indexes where object_id = object_id('dbo.Log_eMailMessages') and name = 'IX_Log_eMailMessages_2')
DROP INDEX IX_Log_eMailMessages_2 ON dbo.Log_eMailMessages
GO
CREATE NONCLUSTERED INDEX IX_Log_eMailMessages_2 ON dbo.Log_eMailMessages
	(
	UserID
	) 
GO