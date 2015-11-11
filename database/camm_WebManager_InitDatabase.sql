----------------------------------------------------
-- dbo.System_GlobalProperties
----------------------------------------------------
if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[System_GlobalProperties]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) 
BEGIN
	CREATE TABLE [dbo].[System_GlobalProperties]
	(
	[ID]                                INT IDENTITY(1,1) NOT NULL,
	[PropertyName]                      NVARCHAR(128) NOT NULL,
	[ValueNVarChar]                     NVARCHAR(256),
	[ValueNText]                        NTEXT,
	[ValueInt]                          INT,
	[ValueBoolean]                      BIT,
	[ValueImage]                        IMAGE,
	[ValueDecimal]                      DECIMAL(18, 0),
	[ValueDateTime]                     DATETIME,
	CONSTRAINT [PK__System_GlobalPro__467D75B8] PRIMARY KEY CLUSTERED ( [ID] )
	);

	CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties] ON [dbo].[System_GlobalProperties](PropertyName )
	CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_1] ON [dbo].[System_GlobalProperties](ValueNVarChar )
	CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_2] ON [dbo].[System_GlobalProperties](ValueInt )
	CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_3] ON [dbo].[System_GlobalProperties](ValueDecimal )
	CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_4] ON [dbo].[System_GlobalProperties](ValueDateTime )
END
GO

-----------------------------------------------------------
--Insert data into dbo.System_GlobalProperties
-----------------------------------------------------------
DELETE FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = 'camm WebManager'
INSERT INTO [dbo].[System_GlobalProperties] ([PropertyName],[ValueNVarChar],[ValueNText],[ValueInt],[ValueBoolean],[ValueImage],[ValueDecimal],[ValueDateTime]) VALUES('DBVersion_Major','camm WebManager','','0',NULL,'',NULL,NULL)
INSERT INTO [dbo].[System_GlobalProperties] ([PropertyName],[ValueNVarChar],[ValueNText],[ValueInt],[ValueBoolean],[ValueImage],[ValueDecimal],[ValueDateTime]) VALUES('DBVersion_Minor','camm WebManager','','0',NULL,'',NULL,NULL)
INSERT INTO [dbo].[System_GlobalProperties] ([PropertyName],[ValueNVarChar],[ValueNText],[ValueInt],[ValueBoolean],[ValueImage],[ValueDecimal],[ValueDateTime]) VALUES('DBVersion_Build','camm WebManager','','0',NULL,'',NULL,NULL)
INSERT INTO [dbo].[System_GlobalProperties] ([PropertyName],[ValueNVarChar],[ValueNText],[ValueInt],[ValueBoolean],[ValueImage],[ValueDecimal],[ValueDateTime]) VALUES('DBProductName','camm WebManager','',NULL,NULL,'',NULL,NULL)
--Keep logs for 7 days (new installations). 
INSERT INTO [dbo].[System_GlobalProperties] ([PropertyName],[ValueNVarchar],[ValueInt]) VALUES('MaxRetentionDays', 'camm WebManager', 7) 


GO
