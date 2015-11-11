/******************************************************
  Module: camm WebManager Services (for SAP, etc.)
******************************************************/

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_EventsProcessed]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[Log_EventsProcessed]
GO

CREATE TABLE [dbo].[Log_EventsProcessed] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Type] [varchar] (6) NOT NULL ,
	[ValueInt] [int] NULL ,
	CONSTRAINT [PK_Log_EventsProcessed] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	) 
) 
GO

CREATE NONCLUSTERED INDEX [IX_Log_EventsProcessed] ON [dbo].[Log_EventsProcessed](Type )
GO
CREATE INDEX [IX_Log_EventsProcessed2] ON [dbo].[Log_EventsProcessed]([ValueInt], [Type]) 
GO
