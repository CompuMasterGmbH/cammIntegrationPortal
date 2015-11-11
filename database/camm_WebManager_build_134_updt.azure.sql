-----------------------------------
-- Smart WebEditor Content table --
-----------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[WebManager_WebEditor]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[WebManager_WebEditor]
GO

CREATE TABLE [dbo].[WebManager_WebEditor] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[ServerID] [int] NOT NULL ,
	[LanguageID] [int] NOT NULL ,
	[IsActive] [bit] NOT NULL ,
	[URL] [nvarchar] (340) NOT NULL ,
	[EditorID] [nvarchar] (100) NULL ,
	[Content] [ntext] NOT NULL ,
	[ModifiedOn] [datetime] NOT NULL ,
	[ModifiedByUser] [int] NOT NULL ,
	[ReleasedOn] [datetime] NOT NULL ,
	[ReleasedByUser] [int] NOT NULL ,
	[Version] [int] NOT NULL ,
	CONSTRAINT [PK__WebManager_WebEd__284DF453] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)   ,
	CONSTRAINT [IX_WebManager_WebEditor] UNIQUE  NONCLUSTERED 
	(
		[ServerID],
		[LanguageID],
		[Version],
		[URL],
		[EditorID]
	)   
) 
GO

----------------------------------------------------
-- dbo.Int_LogAuthChanges
----------------------------------------------------
ALTER PROCEDURE dbo.Int_LogAuthChanges
(
@UserID int = Null,
@GroupID int = Null,
@AppID int
)
WITH ENCRYPTION
AS 

If @GroupID Is Not Null
	begin
		-- log indirect changes on users
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		select id_user, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -7, Null
		from view_Memberships_CummulatedWithAnonymous
		where id_group = @GroupID
		-- log group auth change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -8, cast(@GroupID as nvarchar(50)))
	end
Else
	If @UserID Is Not Null
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -6, Null)
GO
