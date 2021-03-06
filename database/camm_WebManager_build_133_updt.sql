--------------------------------------------
-- Extend the log table for unicode and long conflict descriptors
--------------------------------------------

if exists (select * from sys.objects where object_id = object_id(N'[Tmp_Log]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [Tmp_Log]
GO
CREATE TABLE dbo.Tmp_Log
	(
	ID int NOT NULL IDENTITY (1, 1),
	UserID int NOT NULL,
	LoginDate datetime NOT NULL,
	RemoteIP nvarchar(32) NOT NULL,
	ServerIP nvarchar(32) NOT NULL,
	ApplicationID int NULL,
	URL nvarchar(1024) NULL,
	ConflictType int NOT NULL,
	ConflictDescription ntext NULL,
	ReviewedAndClosed bit NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_Log ON
GO
IF EXISTS(SELECT * FROM dbo.[Log])
	 EXEC('INSERT INTO dbo.Tmp_Log (ID, UserID, LoginDate, RemoteIP, ServerIP, ApplicationID, URL, ConflictType, ConflictDescription, ReviewedAndClosed)
		SELECT ID, UserID, LoginDate, RemoteIP, ServerIP, ApplicationID, URL, ConflictType, CONVERT(ntext, ConflictDescription), IsNull(ReviewedAndClosed,0) FROM dbo.[Log] (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_Log OFF
GO
DROP TABLE dbo.[Log]
GO
ALTER TABLE dbo.Tmp_Log ADD CONSTRAINT
	DF__log__reviewedand__5B0E7E4A DEFAULT (0) FOR ReviewedAndClosed
GO
EXECUTE sp_rename N'dbo.Tmp_Log', N'Log', 'OBJECT'
GO
ALTER TABLE dbo.[Log] ADD CONSTRAINT
	PK_Log PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH FILLFACTOR = 90 ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_Log ON dbo.[Log]
	(
	UserID
	) WITH FILLFACTOR = 90 ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_Log_1 ON dbo.[Log]
	(
	LoginDate
	) WITH FILLFACTOR = 90 ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_Log_2 ON dbo.[Log]
	(
	ApplicationID
	) WITH FILLFACTOR = 90 ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_Log_3 ON dbo.[Log]
	(
	ConflictType
	) WITH FILLFACTOR = 90 ON [PRIMARY]
GO
