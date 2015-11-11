IF EXISTS (select * from dbo.sysindexes where id = object_id('dbo.Log_eMailMessages') and name = 'IX_Log_eMailMessages_1')
DROP INDEX dbo.Log_eMailMessages.IX_Log_eMailMessages_1
GO
CREATE NONCLUSTERED INDEX IX_Log_eMailMessages_1 ON dbo.Log_eMailMessages
	(
	DateTime
	) 
GO
IF EXISTS (select * from dbo.sysindexes where id = object_id('dbo.Log_eMailMessages') and name = 'IX_Log_eMailMessages_2')
DROP INDEX dbo.Log_eMailMessages.IX_Log_eMailMessages_2
GO
CREATE NONCLUSTERED INDEX IX_Log_eMailMessages_2 ON dbo.Log_eMailMessages
	(
	UserID
	) 
GO