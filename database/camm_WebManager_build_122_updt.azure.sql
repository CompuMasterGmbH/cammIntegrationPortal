----------- index improvement --------------
if exists (select * from sys.indexes where name = N'IX_Log_Users_2' and object_id = object_id(N'[dbo].[Log_Users]'))
DROP INDEX [IX_Log_Users_2] ON [dbo].[Log_Users]
GO

 CREATE  INDEX [IX_Log_Users_2] ON [dbo].[Log_Users]([ID_User], [Type])  
GO


-------------------------------------------------
-- Mail tables
-------------------------------------------------

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_eMail_Attachments]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[Log_eMail_Attachments]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_eMailMessages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[Log_eMailMessages]
GO

CREATE TABLE [dbo].[Log_eMailMessages] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[UserID] [int] NOT NULL ,
	[data] [ntext],
	[State] [tinyint] NOT NULL,
	[DateTime] [datetime] NOT NULL 
	CONSTRAINT [PK_Log_eMailMessages] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)   
) 
GO

if exists (select * from sys.indexes where name = N'IX_Log_eMailMessages' and object_id = object_id(N'[dbo].[Log_eMailMessages]'))
DROP INDEX [IX_Log_eMailMessages] ON [dbo].[Log_eMailMessages]
GO

 CREATE  INDEX [IX_Log_eMailMessages] ON [dbo].[Log_eMailMessages]([State])  
GO

if exists (select * from sys.indexes where name = N'IX_Log_eMailMessages2' and object_id = object_id(N'[dbo].[Log_eMailMessages]'))
DROP INDEX [IX_Log_eMailMessages2] ON [dbo].[Log_eMailMessages]
GO

if exists (select * from sys.indexes where name = N'IX_Log_eMailMessages3' and object_id = object_id(N'[dbo].[Log_eMailMessages]'))
DROP INDEX [IX_Log_eMailMessages3] ON [dbo].[Log_eMailMessages]
GO

 CREATE  INDEX [IX_Log_eMailMessages3] ON [dbo].[Log_eMailMessages]([State], [DateTime], [ID])  
GO

