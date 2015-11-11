----------- index improvement --------------
if exists (select * from dbo.sysindexes where name = N'IX_Log_Users_2' and id = object_id(N'[dbo].[Log_Users]'))
drop index [dbo].[Log_Users].[IX_Log_Users_2]
GO

 CREATE  INDEX [IX_Log_Users_2] ON [dbo].[Log_Users]([ID_User], [Type]) WITH  FILLFACTOR = 90 ON [PRIMARY]
GO


-------------------------------------------------
-- Mail tables
-------------------------------------------------

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Log_eMail_Attachments]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[Log_eMail_Attachments]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Log_eMailMessages]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
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
	)WITH  FILLFACTOR = 90  ON [PRIMARY] 
) ON [PRIMARY]
GO

if exists (select * from dbo.sysindexes where name = N'IX_Log_eMailMessages' and id = object_id(N'[dbo].[Log_eMailMessages]'))
drop index [dbo].[Log_eMailMessages].[IX_Log_eMailMessages]
GO

 CREATE  INDEX [IX_Log_eMailMessages] ON [dbo].[Log_eMailMessages]([State]) WITH  FILLFACTOR = 90 ON [PRIMARY]
GO

if exists (select * from dbo.sysindexes where name = N'IX_Log_eMailMessages2' and id = object_id(N'[dbo].[Log_eMailMessages]'))
drop index [dbo].[Log_eMailMessages].[IX_Log_eMailMessages2]
GO

if exists (select * from dbo.sysindexes where name = N'IX_Log_eMailMessages3' and id = object_id(N'[dbo].[Log_eMailMessages]'))
drop index [dbo].[Log_eMailMessages].[IX_Log_eMailMessages3]
GO

 CREATE  INDEX [IX_Log_eMailMessages3] ON [dbo].[Log_eMailMessages]([State], [DateTime], [ID]) WITH  FILLFACTOR = 90 ON [PRIMARY]
GO

