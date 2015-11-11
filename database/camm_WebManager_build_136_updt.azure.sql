-----------------------------------------------
-- Update SmartWcms tables: NULLable columns --
-----------------------------------------------
CREATE TABLE dbo.Tmp_WebManager_WebEditor
	(
	ID int NOT NULL IDENTITY (1, 1)  PRIMARY KEY,
	ServerID int NOT NULL,
	LanguageID int NOT NULL,
	IsActive bit NOT NULL,
	URL nvarchar(340) NOT NULL,
	EditorID nvarchar(100) NULL,
	Content ntext NOT NULL,
	ModifiedOn datetime NOT NULL,
	ModifiedByUser int NOT NULL,
	ReleasedOn datetime NULL,
	ReleasedByUser int NULL,
	Version int NOT NULL
	)  
	 TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_WebManager_WebEditor ON;
IF EXISTS(SELECT * FROM dbo.WebManager_WebEditor)
	 EXEC('INSERT INTO dbo.Tmp_WebManager_WebEditor (ID, ServerID, LanguageID, IsActive, URL, EditorID, Content, ModifiedOn, ModifiedByUser, ReleasedOn, ReleasedByUser, Version)
		SELECT ID, ServerID, LanguageID, IsActive, URL, EditorID, Content, ModifiedOn, ModifiedByUser, ReleasedOn, ReleasedByUser, Version FROM dbo.WebManager_WebEditor  ')
;SET IDENTITY_INSERT dbo.Tmp_WebManager_WebEditor OFF
GO
DROP TABLE dbo.WebManager_WebEditor
GO
EXECUTE sp_rename N'dbo.Tmp_WebManager_WebEditor', N'WebManager_WebEditor', 'OBJECT' 
GO


GO
ALTER TABLE dbo.WebManager_WebEditor ADD CONSTRAINT
	IX_WebManager_WebEditor UNIQUE NONCLUSTERED 
	(
	ServerID,
	LanguageID,
	Version,
	URL,
	EditorID
	)  
GO
