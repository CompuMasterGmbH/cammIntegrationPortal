/******************************************************
	DISABLE THE ASP ENGINE PER SETUP DEFAULT
******************************************************/
DELETE FROM [dbo].[System_WebAreaScriptEnginesAuthorization]
WHERE ScriptEngine = 1
GO

/******************************************************
  SystemModule: Minor fixes
	- List of supported script engines fixed
	- Profile data of old admin user with ID 0/1 fixed
	- Missing indizes on table "Log"
******************************************************/
-- Erase script engines 'PHP' and 'SAP WebStudio'
DELETE FROM System_ScriptEngines 
WHERE ID = 3 OR ID = 4
GO
-- Fix für Profildaten von User ID 0 
EXEC Int_UpdateUserDetailDataWithProfileData 0
EXEC Int_UpdateUserDetailDataWithProfileData 1
GO
-- Missing indizes on table "Log"
if exists (select * from sys.indexes where name = N'IX_Log' and object_id = object_id(N'[dbo].[Log]'))
DROP INDEX [IX_Log] ON [dbo].[Log]
GO
CREATE  INDEX [IX_Log] ON [dbo].[Log]([UserID]) 
GO
if exists (select * from sys.indexes where name = N'IX_Log_1' and object_id = object_id(N'[dbo].[Log]'))
DROP INDEX [IX_Log_1] ON [dbo].[Log]
GO
CREATE  INDEX [IX_Log_1] ON [dbo].[Log]([LoginDate]) 
GO
if exists (select * from sys.indexes where name = N'IX_Log_2' and object_id = object_id(N'[dbo].[Log]'))
DROP INDEX [IX_Log_2] ON [dbo].[Log]
GO
CREATE  INDEX [IX_Log_2] ON [dbo].[Log]([ApplicationID]) 
GO
if exists (select * from sys.indexes where name = N'IX_Log_3' and object_id = object_id(N'[dbo].[Log]'))
DROP INDEX [IX_Log_3] ON [dbo].[Log]
GO
CREATE  INDEX [IX_Log_3] ON [dbo].[Log]([ConflictType]) 
GO


/******************************************************
  SystemModule: AdminArea: 
	- Delegates
******************************************************/
SET ANSI_PADDING OFF
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_SubSecurityAdjustments]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[System_SubSecurityAdjustments]
GO

CREATE TABLE [dbo].[System_SubSecurityAdjustments] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[UserID] [int] NOT NULL ,
	[TableName] [nvarchar] (255) NOT NULL ,
	[TablePrimaryIDValue] [int] NOT NULL ,
	[AuthorizationType] [nvarchar] (50) NOT NULL ,
	CONSTRAINT [PK_System_SubSecurityAdjustments] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)   
) 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateSubSecurityAdjustment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[AdminPrivate_UpdateSubSecurityAdjustment]
GO

create procedure dbo.AdminPrivate_UpdateSubSecurityAdjustment
(
	@ActionTypeSave bit,
	@UserID int,
	@TableName nvarchar(255),
	@TablePrimaryIDValue int,
	@AuthorizationType nvarchar(50)
)
WITH ENCRYPTION
AS
DECLARE @CurrentPrimID int

If @ActionTypeSave <> 0
	-- Update or Insert Where Update is never neccessary
	BEGIN
	SELECT @CurrentPrimID = ID 
	FROM System_SubSecurityAdjustments 
	WHERE UserID = @UserID 
		AND TableName = @TableName 
		AND TablePrimaryIDValue = @TablePrimaryIDValue
		AND AuthorizationType = @AuthorizationType
	IF @CurrentPrimID Is Null 
		-- Insert required
		INSERT INTO System_SubSecurityAdjustments (UserID, TableName, TablePrimaryIDValue, AuthorizationType)
		VALUES (@UserID, @TableName, @TablePrimaryIDValue, @AuthorizationType)
	END
ELSE
	-- Delete
	DELETE FROM System_SubSecurityAdjustments 
	WHERE UserID = @UserID 
		AND TableName = @TableName 
		AND TablePrimaryIDValue = @TablePrimaryIDValue
		AND AuthorizationType = @AuthorizationType
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

INSERT INTO [dbo].[System_SubSecurityAdjustments]
	([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
SELECT ID_User, 'Applications', 0, 'SecurityMaster'
FROM Memberships
WHERE ID_Group = 7 AND ID_User Is Not Null

INSERT INTO [dbo].[System_SubSecurityAdjustments]
	([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
SELECT ID_User, 'Groups', 0, 'SecurityMaster'
FROM Memberships
WHERE ID_Group = 7 AND ID_User Is Not Null
GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplication 
(
	@ReleasedByUserID int,
	@Title varchar(255)
)
WITH ENCRYPTION
AS
DECLARE @CurUserID int
DECLARE @NewAppID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		SET NOCOUNT ON
		INSERT INTO dbo.Applications (Title, ReleasedBy, ModifiedBy, LanguageID, LocationID) VALUES (@Title, @ReleasedByUserID, @ReleasedByUserID, 0, 0)

		SELECT @NewAppID = @@IDENTITY

		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Applications', @NewAppID, 'Owner'

		SET NOCOUNT OFF
		SELECT Result = @NewAppID
	END
Else
	
	SELECT Result = 0
GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateGroup 
(
	@ReleasedByUserID int,
	@Name nvarchar(50),
	@Description nvarchar(1024)
)
WITH ENCRYPTION
AS
DECLARE @CurUserID int
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		
		SELECT Result = -1
		
		INSERT INTO dbo.Gruppen (Name, Description, ReleasedBy, ModifiedBy) VALUES (@Name, @Description, @ReleasedByUserID, @ReleasedByUserID)

		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Groups', @@IDENTITY, 'Owner'
	END
Else
	
	SELECT Result = 0
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetUserNameForScriptEngineSessionID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_GetUserNameForScriptEngineSessionID]
GO

CREATE PROCEDURE dbo.Public_GetUserNameForScriptEngineSessionID
(
	@UserName varchar(20) output,
	@ScriptEngine_SessionID nvarchar(128),
	@ScriptEngine_ID int,
	@ServerID int
)
WITH ENCRYPTION
AS
select @UserName = dbo.Benutzer.LoginName
from dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes 
	left join dbo.System_UserSessions ON dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.SessionID = dbo.System_UserSessions.ID_Session
	left join dbo.Benutzer ON dbo.System_UserSessions.ID_User = dbo.Benutzer.ID
where scriptengine_sessionID = @ScriptEngine_SessionID 
	and server=@ServerID
	and scriptengine_ID=@ScriptEngine_ID
	and inactive=0
order by sessionid desc

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

