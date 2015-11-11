-- Add additional field ErrorDetails to Mail Queue
if not exists (select * from dbo.syscolumns where id = object_id('dbo.Log_eMailMessages') and name = 'ErrorDetails') 
ALTER TABLE dbo.Log_eMailMessages ADD
	ErrorDetails ntext NULL
GO