/*  Contains all functions and stored procedures cumulated in their latest version up to build 203 */

----------------------------------------------------
-- dbo.AdminPrivate_CloneApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CloneApplication 
	@ReleasedByUserID int,
	@AppID int,
	@CloneType int,
	@CopyDelegates int
WITH ENCRYPTION
AS
DECLARE @CurUserID int
DECLARE @NewAppID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		SET NOCOUNT ON

		If @CloneType = 1 -- copy application and authorizations
			BEGIN
				-- Add new application
				INSERT INTO dbo.Applications
				                      (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, 
				                      LocationID, LanguageID, SystemApp, SystemAppType, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, @ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, 0, NULL, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags
				FROM         dbo.Applications
				WHERE     (ID = @AppID)

				SELECT @NewAppID = SCOPE_IDENTITY()

				-- Add Group Authorizations
				INSERT INTO dbo.ApplicationsRightsByGroup
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application)
				SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @NewAppID AS ID_Application
				FROM         dbo.ApplicationsRightsByGroup
				WHERE     (ID_Application = @AppID)

				-- Add User Authorizations
				INSERT INTO dbo.ApplicationsRightsByUser
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application)
				SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @NewAppID AS ID_Application
				FROM         dbo.ApplicationsRightsByUser
				WHERE     (ID_Application = @AppID)

			END
		Else -- copy application and inherit authorizations from cloned application
			BEGIN
				INSERT INTO dbo.Applications
				                      (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, 
				                      LocationID, LanguageID, SystemApp, SystemAppType, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, 
						@ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, 0, NULL, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, @AppID As AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags
				FROM         dbo.Applications
				WHERE    (ID = @AppID)

				SELECT @NewAppID = SCOPE_IDENTITY()
			END

		If @CopyDelegates = 1
			BEGIN
				INSERT INTO [dbo].[System_SubSecurityAdjustments]([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
				SELECT [UserID], [TableName], @NewAppID, [AuthorizationType]
				FROM [dbo].[System_SubSecurityAdjustments]
				WHERE TableName = 'Applications' AND TablePrimaryIDValue = @AppID
			END
		Else
			BEGIN
				INSERT INTO [dbo].[System_SubSecurityAdjustments]([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
				SELECT [UserID], [TableName], @NewAppID, [AuthorizationType]
				FROM [dbo].[System_SubSecurityAdjustments]
				WHERE TableName = 'Applications' AND TablePrimaryIDValue = @AppID AND [AuthorizationType] = 'ResponsibleContact'
			END
		
		SET NOCOUNT OFF
	
		SELECT Result = @NewAppID
		
	END
Else
	
	SELECT Result = 0

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateAccessLevel 
	@ReleasedByUserID int,
	@Title nvarchar(50)
WITH ENCRYPTION
AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
		BEGIN
		SET NOCOUNT ON
		INSERT INTO dbo.System_AccessLevels (Title, ReleasedBy, ModifiedBy) VALUES (@Title, @ReleasedByUserID, @ReleasedByUserID)
		SET NOCOUNT OFF

		SELECT Result = SCOPE_IDENTITY()
		
	END
Else
	
	SELECT Result = 0


GO

---------------------------------------------------
-- dbo.AdminPrivate_CreateAdminServerNavPoints
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateAdminServerNavPoints
	(
		@NewServerID int,
		@OldServerID int,
		@ModifiedBy int,
		@ForceRewrite bit = 0
	)
WITH ENCRYPTION
AS

If @NewServerID = @OldServerID AND @ForceRewrite = 0
	Return

IF @ForceRewrite = 1 AND IsNull(@OldServerID, 0) = 0
	SELECT @OldServerID = @NewServerID

---------------------------------
-- Admin server nav items --
---------------------------------
DECLARE @AppID_Applications int
DECLARE @AppID_Authorizations int
DECLARE @AppID_Groups int
DECLARE @AppID_Memberships int
DECLARE @AppID_Users int
DECLARE @AppID_ResetPassword int
DECLARE @AppID_ServerSetup int
DECLARE @AppID_AccessLevels int
DECLARE @AppID_Trouble int
DECLARE @AppID_QueueMonitor int
DECLARE @AppID_NavPreview int
DECLARE @AppID_Logs int
DECLARE @AppID_Redirections int
DECLARE @AppID_Markets int
DECLARE @AppID_TextModules int
DECLARE @OldServerIsFurthermoreAdminServer bit
DECLARE @OldAppID_QueueMonitor int
DECLARE @OldAppID_Redirections int
DECLARE @OldAppID_TextModules int
DECLARE @OldAppID_Logs int

SET NOCOUNT ON

-- An admin server could be admin server for several server groups, 
-- that's why we have to check if we are allowed 
-- to delete the nav point from the old server
SELECT @OldServerIsFurthermoreAdminServer = 
	(
	SELECT TOP 1 CASE WHEN ID Is Null Then 0 Else 1 END 
	FROM dbo.System_ServerGroups 
	WHERE UserAdminServer = @OldServerID
	)
If @OldServerIsFurthermoreAdminServer Is Null
	SELECT @OldServerIsFurthermoreAdminServer = 0


-- Redirections - Get the application ID from where we can move the authorizations/sub delegations (the English one is always the master entry)
SELECT TOP 1 @OldAppID_Redirections = ID
FROM dbo.Applications
WHERE SystemApp = 1 And SystemAppType = 3 AND LanguageID = 1
	AND Title = 'System - Administration - Redirections'
	AND LocationID = @OldServerID
-- Log analysis - Get the application ID from where we can move the authorizations/sub delegations (the English one is always the master entry)
SELECT TOP 1 @OldAppID_Logs = ID
FROM dbo.Applications
WHERE SystemApp = 1 And SystemAppType = 3 AND LanguageID = 1
	AND Title = 'System - User Administration - LogAnalysis'
	AND LocationID = @OldServerID
-- Mail queue monitor - Get the application ID from where we can move the authorizations/sub delegations (the English one is always the master entry)
SELECT TOP 1 @OldAppID_QueueMonitor = ID
FROM dbo.Applications
WHERE SystemApp = 1 And SystemAppType = 3 AND LanguageID = 1
	AND Title = 'System - Mail Queue Monitor'
	AND LocationID = @OldServerID
-- Text modules - Get the application ID from where we can move the authorizations/sub delegations (the English one is always the master entry)
SELECT TOP 1 @OldAppID_TextModules = ID
FROM dbo.Applications
WHERE SystemApp = 1 And SystemAppType = 3 AND LanguageID = 1
	AND Title = 'System - TextModules'
	AND LocationID = @OldServerID


-- Remove old, unneeded stuff
If @ForceRewrite = 1 OR @OldServerIsFurthermoreAdminServer = 0
	-- okay, we can delete any existing nav points for administration purposes
	BEGIN
	-- delete old authorization to free up space in DB and to prevent possible half-opened doors for hackers
	DELETE dbo.ApplicationsRightsByUser
		FROM dbo.ApplicationsRightsByUser INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
		WHERE Applications_CurrentAndInactiveOnes.SystemApp <> 0 And Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (Applications_CurrentAndInactiveOnes.LocationID = @OldServerID OR Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	DELETE dbo.ApplicationsRightsByGroup
		FROM dbo.ApplicationsRightsByGroup INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
		WHERE Applications_CurrentAndInactiveOnes.SystemApp <> 0 And Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (Applications_CurrentAndInactiveOnes.LocationID = @OldServerID OR Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	-- delete the system applications themselves
	UPDATE Applications_CurrentAndInactiveOnes
		SET AppDeleted = 1
		FROM dbo.Applications_CurrentAndInactiveOnes 
		WHERE dbo.Applications_CurrentAndInactiveOnes.SystemApp <> 0 And dbo.Applications_CurrentAndInactiveOnes.SystemAppType IN (2, 3) And (dbo.Applications_CurrentAndInactiveOnes.LocationID = @OldServerID OR dbo.Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	END
Else
	-- we have to keep the old nav items for another server group which already uses our old server as admin server
	BEGIN
	-- delete old authorization to free up space in DB and to prevent possible half-opened doors for hackers
	DELETE dbo.ApplicationsRightsByUser
		FROM dbo.ApplicationsRightsByUser INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
		WHERE Applications_CurrentAndInactiveOnes.SystemApp <> 0 And Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	DELETE dbo.ApplicationsRightsByGroup
		FROM dbo.ApplicationsRightsByGroup INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
		WHERE Applications_CurrentAndInactiveOnes.SystemApp <> 0 And Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	-- delete the system applications themselves
	UPDATE Applications_CurrentAndInactiveOnes
		SET AppDeleted = 1
		FROM dbo.Applications_CurrentAndInactiveOnes 
		WHERE dbo.Applications_CurrentAndInactiveOnes.SystemApp <> 0 And dbo.Applications_CurrentAndInactiveOnes.SystemAppType IN (2, 3) And (dbo.Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	END


-- Create base security objects
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications','',getdate(),@ModifiedBy,'Web Administration','Setup','Applications',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Applications = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations','',getdate(),@ModifiedBy,'Web Administration','User Administration','Authorizations',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Authorizations = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups','',getdate(),@ModifiedBy,'Web Administration','User Administration','Groups',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Groups = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships','',getdate(),@ModifiedBy,'Web Administration','User Administration','Group memberships',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Memberships = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users','',getdate(),@ModifiedBy,'Web Administration','User Administration','Users',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Users = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','Reset user password',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_ResetPassword = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview','',getdate(),@ModifiedBy,'Web Administration','Navigation preview',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_NavPreview = SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','User hotline support',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Trouble= SCOPE_IDENTITY()

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Server administration',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_ServerSetup = SCOPE_IDENTITY()
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','About Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Access levels',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_AccessLevels = SCOPE_IDENTITY()

-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Trouble,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Authorizations ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Groups ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Memberships ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_ResetPassword ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_NavPreview ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Users ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_AccessLevels ,'6',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_ServerSetup ,'6',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Applications ,'6',getdate(),@ModifiedBy)

-- Log analysis
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,'Web Administration','Log Analysis','Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,3)
SELECT @AppID_Logs = SCOPE_IDENTITY()
-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Logs ,7,getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Logs ,6,getdate(),@ModifiedBy)
-- Move old authorizations and old admin delegation settings to new security object
IF @OldAppID_Logs IS NOT NULL 
	BEGIN
	UPDATE dbo.ApplicationsRightsByGroup SET ID_Application = @AppID_Logs WHERE ID_Application = @OldAppID_Logs 
		AND ID_GroupOrPerson NOT IN (6,7) -- these ones have already been addded
	UPDATE dbo.ApplicationsRightsByUser SET ID_Application = @AppID_Logs WHERE ID_Application = @OldAppID_Logs 
	UPDATE [dbo].[System_SubSecurityAdjustments] SET [TablePrimaryIDValue] = @AppID_Logs WHERE [TableName] = 'Applications' AND [TablePrimaryIDValue] = @OldAppID_Logs 
	END

-- Redirections - English: not a system app to allow modification of authorizations
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,3)
SELECT @AppID_Redirections = SCOPE_IDENTITY()
-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Redirections ,6,getdate(),@ModifiedBy)
-- Move old authorizations and old admin delegation settings to new security object
IF @OldAppID_Redirections IS NOT NULL 
	BEGIN
	UPDATE dbo.ApplicationsRightsByGroup SET ID_Application = @AppID_Redirections WHERE ID_Application = @OldAppID_Redirections 
		AND ID_GroupOrPerson NOT IN (6) -- these ones have already been addded
	UPDATE dbo.ApplicationsRightsByUser SET ID_Application = @AppID_Redirections WHERE ID_Application = @OldAppID_Redirections 
	UPDATE [dbo].[System_SubSecurityAdjustments] SET [TablePrimaryIDValue] = @AppID_Redirections WHERE [TableName] = 'Applications' AND [TablePrimaryIDValue] = @OldAppID_Redirections 
	END

-- Markets
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Markets/Languages',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Markets = SCOPE_IDENTITY()
-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Markets ,6,getdate(),@ModifiedBy)

-- Mail queue monitor - English: not a system app to allow modification of authorizations
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','Mail queue monitor',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,3)
SELECT @AppID_QueueMonitor = SCOPE_IDENTITY()
-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_QueueMonitor,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_QueueMonitor,'6',getdate(),@ModifiedBy)
-- Move old authorizations and old admin delegation settings to new security object
IF @OldAppID_QueueMonitor IS NOT NULL 
	BEGIN
	UPDATE dbo.ApplicationsRightsByGroup SET ID_Application = @AppID_QueueMonitor WHERE ID_Application = @OldAppID_QueueMonitor 
		AND ID_GroupOrPerson NOT IN (6,7) -- these ones have already been addded
	UPDATE dbo.ApplicationsRightsByUser SET ID_Application = @AppID_QueueMonitor WHERE ID_Application = @OldAppID_QueueMonitor 
	UPDATE [dbo].[System_SubSecurityAdjustments] SET [TablePrimaryIDValue] = @AppID_QueueMonitor WHERE [TableName] = 'Applications' AND [TablePrimaryIDValue] = @OldAppID_QueueMonitor 
	END

-- Text modules - English: not a system app to allow modification of authorizations
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Text Modules',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,3)
SELECT @AppID_TextModules = SCOPE_IDENTITY()
-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_TextModules ,'6',getdate(),@ModifiedBy)
-- Move old authorizations and old admin delegation settings to new security object
IF @OldAppID_TextModules IS NOT NULL 
	BEGIN
	UPDATE dbo.ApplicationsRightsByGroup SET ID_Application = @AppID_TextModules WHERE ID_Application = @OldAppID_TextModules 
		AND ID_GroupOrPerson NOT IN (6) -- these ones have already been addded
	UPDATE dbo.ApplicationsRightsByUser SET ID_Application = @AppID_TextModules WHERE ID_Application = @OldAppID_TextModules 
	UPDATE [dbo].[System_SubSecurityAdjustments] SET [TablePrimaryIDValue] = @AppID_TextModules WHERE [TableName] = 'Applications' AND [TablePrimaryIDValue] = @OldAppID_TextModules 
	END



-- Copies in German language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Navigations-Vorschau',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Benutzer-Verwaltung',N'Benutzer',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Benutzer-Verwaltung',N'Berechtigungen',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Benutzer-Verwaltung',N'Gruppen',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Benutzer-Verwaltung',N'Mitgliedschaften',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Log Auswertungen',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,2,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Trouble Center',N'Benutzer Hotline Support',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Trouble Center',N'Mail-Queue Monitor',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Trouble Center',N'Passwörter zurücksetzen',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Anwendungen',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Märkte/Sprachen',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,2,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Server-Verwaltung',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Text-Module',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Über Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Weiterleitungen',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,2,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Web-Administration',N'Setup',N'Zugriffs-Levels',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'2',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in German language


-- Copies in Polish language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Podgląd nawigacji',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Zarządzanie użytkownikami',N'Użytkownik',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Zarządzanie użytkownikami',N'Uprawnienia',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Zarządzanie użytkownikami',N'Grupy',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Zarządzanie użytkownikami',N'Członkostwa',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Analizy logowań',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,343,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Centrum pomocy',N'Pomoc telefoniczna dla użytkowników',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Centrum pomocy',N'Monitorowanie zapytań mailowych',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Centrum pomocy',N'Resetowanie haseł',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Zastosowania',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Rynki, języki',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,343,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Zarządzanie serwerami',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Moduły tekstowe',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Informacje o aplikacji Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Odsyłacze',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,343,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Administracja Web',N'Setup',N'Poziomy dostępu',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'343',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in German language


-- Copies in Japanese language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Web管理',N'ユーザー管理',N'権限',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Web管理',N'ユーザー管理',N'グループ',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Web管理',N'ユーザー管理',N'メンバーシップ',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Web管理',N'ユーザー管理',N'ユーザー',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Web管理',N'トラブルセンター',N'ユーザーホットラインサポート',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Web管理',N'トラブルセンター',N'パスワードをリセットする',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Web管理',N'トラブルセンター',N'メールキューモニタ',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Web管理',N'Setup',N'サーバー管理',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Web管理',N'セットアップ',N'Web-Managerについて',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Web管理',N'セットアップ',N'アプリケーション',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Web管理',N'セットアップ',N'アクセスレベル',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Web管理',N'ログ評価',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,202,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Web管理',N'セットアップ',N'転送',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,202,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Web管理',N'セットアップ',N'市場/言語',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,202,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Web管理',N'ナビゲーションプレビュー',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Web管理',N'セットアップ',N'テキストモジュール',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'202',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in Japanese language


-- Copies in French language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Administration web',N'Administration utilisateur',N'Autorisations',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Administration web',N'Administration utilisateur',N'Groupes',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Administration web',N'Administration utilisateur',N'Membres',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administration web',N'Administration utilisateur',N'Utilisateurs',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administration web',N'Centre de dépannage',N'Assistance téléphonique utilisateur',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Administration web',N'Centre de dépannage',N'Réinitialiser les mots de passe',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Administration web',N'Centre de dépannage',N'Moniteur queue de mail',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'Administration du serveur',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'A propos du Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'Applications',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'Niveaux d’accès',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Administration web',N'Evaluations Log',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,3,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'Retransmissions',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,3,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'Marchés/langues',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,3,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Administration web',N'Aperçu navigation',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Administration web',N'Setup',N'Modules texte',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'3',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in French language


-- Copies in Chinese language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Web管理',N'用户管理',N'权限',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Web管理',N'用户管理',N'组别',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Web管理',N'用户管理',N'会员',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Web管理',N'用户管理',N'用户',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Web管理',N'疑难中心',N'用户支持热线',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Web管理',N'疑难中心',N'重置密码',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Web管理',N'疑难中心',N'邮件查询显示器',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'服务器管理',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'关于Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'应用',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'访问级别',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Web管理',N'日志分析',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,80,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'转发',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,80,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'市场/语言',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,80,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Web管理',N'导航预览',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Web管理',N'设置',N'文本模块',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'80',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in Chinese language


-- Copies in Russian language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Управление пользователями',N'Права',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Управление пользователями',N'Группы',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Управление пользователями',N'Членства',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Управление пользователями',N'Пользователь',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Центр помощи',N'Горячая линия помощи пользователям',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Центр помощи',N'Сброс паролей',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Центр помощи',N'Монитор очереди электронной почты',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'Управление сервером',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'О Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'Приложения',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'Уровни доступа',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Анализы регистрации',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,359,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'Передачи',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,359,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'Рынки/языки',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,359,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Предварительный просмотр навигации',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Веб-организация',N'Настройка',N'Текстовые модули',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'359',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in Russian language


-- Copies in Italian language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Gestione utenti',N'Autorizzazioni',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Gestione utenti',N'Gruppi',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Gestione utenti',N'Appartenenze',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Gestione utenti',N'Utente',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Trouble Center',N'Supporto hotline per utenti',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Trouble Center',N'Reset password',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Trouble Center',N'Mail-Queue Monitor',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Amministrazione server',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Riguardo Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Applicazioni',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Livelli di accesso',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Valutazioni log',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,200,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Trasmissioni',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,200,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Mercati/Lingue',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,200,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Anteprima di navigazione',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Amministrazione web',N'Setup',N'Moduli di testo',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'200',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in Italian language


-- Copies in Spanish language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Administración de usuarios',N'Autorizaciones',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Administración de usuarios',N'Grupos',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Administración de usuarios',N'Socio en',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Administración de usuarios',N'Usuario',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Centro de asistencia',N'Línea directa de asistencia al usuario',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Centro de asistencia',N'Restablecer contraseñas',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Centro de asistencia',N'Monitor del servidor de correo',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Administrador del servidor',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Sobre el Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Aplicaciones',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Niveles de acceso',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Análisis log',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,4,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Redirecciones',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,4,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Mercados/Idiomas',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,4,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Vista preliminar de navegación',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Administración del sitio',N'Configuración',N'Módulos de texto',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'4',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in Spanish language


-- Copies in Portuguese language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Visualização prévia da navegação',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx',N'',NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_QueueMonitor,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Gestão dos utilizadores',N'Utilizador',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Gestão dos utilizadores',N'Autorizações',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Gestão dos utilizadores',N'Grupos',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Gestão dos utilizadores',N'Qualidades de membro',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Análise de protocolo',N'Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@NewServerID,345,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Centro de falhas',N'Hotline assistênca ao utilizador',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx',N'',NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Centro de falhas',N'Monitor para mails em linha de espera',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx',N'',NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Centro de falhas',N'Repór as palavras-passe',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Aplicações',NULL,NULL,NULL,'[ADMINURL]apps.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Markets',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Mercados/linguas',NULL,NULL,NULL,'[ADMINURL]markets.aspx',NULL,NULL,0,0,@NewServerID,345,1,getdate(),@ModifiedBy,0,@AppID_Markets ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Gestão do servidor',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - TextModules',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Módulos de texto',NULL,NULL,NULL,'[ADMINURL]textmodules.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_TextModules,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Através do Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Transmissões',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@NewServerID,345,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,N'Administração Web',N'Configuração',N'Nível de acesso',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,N'345',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
-- END Copies in Portuguese language


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

		SELECT @NewAppID = SCOPE_IDENTITY()

		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Applications', @NewAppID, 'Owner', @ReleasedByUserID

		SET NOCOUNT OFF
		SELECT Result = @NewAppID
	END
Else
	
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByGroup 
	@ReleasedByUserID int,
	@AppID int,
	@GroupID int
WITH ENCRYPTION
AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END


SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 1
	RETURN 1
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForAuthorizations 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Password validation and update
If @CurUserID Is Not Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application, ID_GroupOrPerson, ReleasedBy) VALUES (@AppID, @GroupID, @ReleasedByUserID)
		EXEC Int_LogAuthChanges @CurUserID, @GroupID, @AppID, @ReleasedByUserID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
	END
Else
	-- Rückgabewert
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 0
	END

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByUser 
	@ReleasedByUserID int,
	@AppID int,
	@UserID int,
	@IsDevelopmentTeamMember bit
WITH ENCRYPTION
AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @SAPFlagName varchar(50)
DECLARE @SAPFlagValue nvarchar(255)
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForAuthorizations 'UpdateRelations', @ReleasedByUserID, @AppID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Check if the given user really exists
SELECT @UserID = (select ID from dbo.Benutzer where id = @UserID)
If @UserID Is Null
	-- Rückgabewert
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 5
		Return 5
	END

SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SELECT @SAPFlagName = substring(navurl, charindex('|2|&', NavUrl)+4, charindex('|3|', NavUrl) - (charindex('|2|&', NavUrl)+4)) FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
	SELECT @SAPFlagValue = [Value] FROM Log_Users WHERE ID_User = @UserID AND Type = @SAPFlagName
	If @SAPFlagValue Is Null
		BEGIN
		SET NOCOUNT OFF
		SELECT Result = 2, @SAPFlagName as SAPFlagName
		RETURN 2
		END
	END

-- User validation and update
If @CurUserID Is Not Null
	-- Validation successfull, authorization will be saved now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByUser 
			(ID_Application, ID_GroupOrPerson, ReleasedBy, DevelopmentTeamMember) 
		VALUES 
			(@AppID, @UserID, @ReleasedByUserID, @IsDevelopmentTeamMember)
		EXEC Int_LogAuthChanges @UserID, Null, @AppID, @ReleasedByUserID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		Return -1
	END
ELSE
	-- Releasing user is not authorized to administer
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END

GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByGroup]
(
	@AuthID int,
	@ReleasedByUserID int
)
WITH ENCRYPTION
AS 

declare @groupID int
declare @AppID int
select top 1 @groupid = id_grouporperson, @appid = id_application from dbo.ApplicationsRightsByGroup where id = @AuthID

EXEC Int_LogAuthChanges NULL, @GroupID, @AppID, @ReleasedByUserID 
DELETE FROM dbo.ApplicationsRightsByGroup WHERE     (ID_GroupOrPerson IS NOT NULL) AND ID=@AuthID

GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByUser]
(
	@AuthID int,
	@ReleasedByUserID int = NULL
)
WITH ENCRYPTION
AS
declare @UserID int
declare @AppID int
select top 1 @userid = id_grouporperson, @appid = id_application from dbo.ApplicationsRightsByUser where id = @AuthID

EXEC Int_LogAuthChanges @UserID, Null, @AppID, @ReleasedByUserID 
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson Is Not Null And ID=@AuthID
GO

IF  EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AdminPrivate_DeleteMemberships]
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteMemberships
----------------------------------------------------
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteMemberships]
(
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForMemberships 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Is releasing user a valid user ID?
If @CurUserID Is Not Null
	-- Validation successfull, membership will be updated now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Record update
		DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID
		-- log group membership change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', NULL, -12, cast(@GroupID as nvarchar(50)))
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateGroup
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_CreateGroup] 
(
	@ReleasedByUserID int,
	@Name nvarchar(100),
	@Description nvarchar(1024)
)
WITH ENCRYPTION
AS
DECLARE @CurUserID int
DECLARE @NewGroupID int
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	BEGIN
		
		SELECT Result = -1
		
		INSERT INTO dbo.Gruppen (Name, Description, ReleasedBy, ModifiedBy) VALUES (@Name, @Description, @ReleasedByUserID, @ReleasedByUserID)
		SELECT @NewGroupID = SCOPE_IDENTITY()
		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Groups', @NewGroupID, 'Owner', @ReleasedByUserID
	END
Else
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateMasterServerNavPoints
----------------------------------------------------

ALTER PROCEDURE dbo.AdminPrivate_CreateMasterServerNavPoints
	(
		@NewServerID int,
		@OldServerID int,
		@ModifiedBy int
	)
WITH ENCRYPTION
AS

-- Removed functionality
-- Now in WebManager DLL

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateMemberships
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMemberships 
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForMemberships 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID
SELECT @MemberShipID = ID FROM dbo.Memberships WHERE ID_Group = @GroupID And ID_User = @UserID
If @MemberShipID Is Null
	BEGIN
		-- Is releasing user a valid user ID?
		If @CurUserID Is Not Null
			-- Validation successfull, membership will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Record update
				INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedBy) VALUES (@GroupID, @UserID, @ReleasedByUserID)
				-- log group membership change
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', NULL, -11, cast(@GroupID as nvarchar(50)))
			END
		Else
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	SELECT Result = -1
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateServer
	(
		@ServerIP varchar(32),
		@ServerGroup int
	)
WITH ENCRYPTION
AS

declare @NewServerID int

SET NOCOUNT ON

-- Create server
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, @ServerIP AS IP, 'Secured server' AS ServerDescription, @ServerGroup AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout

-- Get new server ID
SELECT @NewServerID = SCOPE_IDENTITY()

-- Check script engines
EXEC AdminPrivate_SetScriptEngineActivation 0, @NewServerID, 0, 1

-- Return new server ID
SET NOCOUNT OFF
SELECT     @NewServerID
RETURN     @NewServerID

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateServerGroup
(
@GroupName nvarchar(255),
@email_Developer nvarchar(255),
@UserID_Creator int
)
WITH ENCRYPTION
AS 

DECLARE @ID_ServerGroup int
DECLARE @ID_AdminServer int
DECLARE @ID_MasterServer int
DECLARE @ID_Group_Public int
DECLARE @ID_Group_Anonymous int
DECLARE @Group_Public_Name nvarchar(255)

SET NOCOUNT ON

SELECT @Group_Public_Name = 'Public ' + SubString(@GroupName, 1, 245)
SELECT @ID_AdminServer = (SELECT TOP 1 UserAdminServer FROM System_ServerGroups)
SELECT @ID_ServerGroup = (SELECT ID FROM System_ServerGroups WHERE ServerGroup = @GroupName)
SELECT @ID_Group_Public = (SELECT ID FROM Gruppen WHERE Name = @Group_Public_Name)
SELECT @ID_Group_Anonymous = (SELECT ID FROM Gruppen WHERE Name = 'Anonymous')
SELECT @ID_MasterServer = (SELECT TOP 1 ID FROM System_Servers WHERE IP = 'secured.yourcompany.com')

-- Erstellbarkeit gewährleisten
IF @ID_ServerGroup Is Not Null 
	BEGIN
		RAISERROR ('Server group already exists', 16, 1)
		RETURN	
	END	
IF @ID_Group_Public Is Not Null
 
	BEGIN
		RAISERROR ('The server group cannot be created because the public group already exists.', 16, 2)
		RETURN	
	END	
IF @ID_Group_Anonymous Is Null 
	BEGIN
		RAISERROR ('Anonymous user cannot be found. There might be a misconfiguration.', 16, 3)
		RETURN	
	END	
IF @ID_MasterServer Is Not Null 
	BEGIN
		RAISERROR ('There is already a server called "secured.yourcompany.com". Please rename it first before creating new server groups.', 16, 3)
		RETURN	
	END	

BEGIN TRANSACTION

SELECT @ID_Group_Public = Null, @ID_MasterServer = Null, @ID_ServerGroup = Null

-- Public Group anlegen
INSERT INTO dbo.Gruppen
                      (Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy)
SELECT     @Group_Public_Name AS Name, 'System group: all users logged on' AS ServerDescription, GETDATE() AS ReleasedOn, @UserID_Creator AS ReleasedBy, 
                      1 AS SystemGroup, GETDATE() AS ModifiedOn, @UserID_Creator AS ModifiedBy
SELECT @ID_Group_Public = SCOPE_IDENTITY()

-- Public Group Security Adjustments
INSERT INTO [dbo].[System_SubSecurityAdjustments] (UserID, TableName, TablePrimaryIDValue, AuthorizationType)
VALUES (@UserID_Creator, 'Groups', @ID_Group_Public, 'Owner')

-- Neuen Server anlegen, welcher als MasterServer fungieren soll
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, 'secured.yourcompany.com' AS IP, 'Secured server' AS ServerDescription, 0 AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout
SELECT @ID_MasterServer = SCOPE_IDENTITY()

-- Check script engines
EXEC AdminPrivate_SetScriptEngineActivation 0, @ID_MasterServer, 0, 1

-- ServerGroup anlegen
INSERT INTO dbo.System_ServerGroups
                      (ServerGroup, ID_Group_Public, ID_Group_Anonymous, MasterServer, UserAdminServer, AreaImage, AreaButton, AreaNavTitle, 
                      AreaCompanyFormerTitle, AreaCompanyTitle, AreaSecurityContactEMail, AreaSecurityContactTitle, AreaDevelopmentContactEMail, 
                      AreaDevelopmentContactTitle, AreaContentManagementContactEMail, AreaContentManagementContactTitle, AreaUnspecifiedContactEMail, 
                      AreaUnspecifiedContactTitle, AreaCopyRightSinceYear, AreaCompanyWebSiteURL, AreaCompanyWebSiteTitle, ModifiedBy)
SELECT     @GroupName AS Expr21, @ID_Group_Public AS Expr19, @ID_Group_Anonymous AS Expr20, @ID_MasterServer AS Expr18, @ID_AdminServer AS Expr17, 
                      '/sysdata/images/global/logo_csa.jpg' AS Expr1, '/sysdata/images/global/button_csa.gif' AS Expr2, NULL AS Expr3, 'YourCompany Ltd.' AS Expr4, 'YourCompany' AS Expr5, @email_Developer AS Expr6, 
                      @email_Developer AS Expr7, @email_Developer AS Expr8, @email_Developer AS Expr9, @email_Developer AS Expr10, 
                      @email_Developer AS Expr11, @email_Developer AS Expr12, @email_Developer AS Expr13, DATEPART(yyyy, GETDATE()) AS Expr14, 
                      'http://www.yourcompany.com/' AS Expr15, 'YourCompany Homepage' AS Expr16, @UserID_Creator as ModifiedBy
SELECT @ID_ServerGroup = SCOPE_IDENTITY()

-- Master Server in die ServerGroup aufnehmen
UPDATE dbo.System_Servers SET ServerGroup = @ID_ServerGroup WHERE ID = @ID_MasterServer

IF @ID_Group_Public Is Null OR @ID_MasterServer Is Null OR @ID_ServerGroup Is Null
	ROLLBACK TRANSACTION
ELSE

	COMMIT TRANSACTION

-- Create master server navigation
EXEC AdminPrivate_CreateMasterServerNavPoints @ID_MasterServer, Null, @UserID_Creator

SET NOCOUNT OFF

SELECT @ID_ServerGroup
Return @ID_ServerGroup

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateUserAccount
(
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@WebApplication varchar(1024),
	@ServerIP nvarchar(32),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability tinyint,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @LocationID int
DECLARE @WebAppID int

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID)))


-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
		-- Logging
		If IsNull(@IsUserChange, 0) = 0 
			-- admin
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, '0.0.0.0', @WebAppID, 3, 'Account ' + @Username + ' created by admin with user id ' + CAST(@ModifiedBy as varchar(20)))
		Else
			-- user change
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, '0.0.0.0', @WebAppID, 1, 'Account ' + @Username + ' created by user itself')
		SET NOCOUNT OFF
		-- Rückgabewert
		If @CurUserID Is Not Null 
			SELECT Result = -1 
		Else
			SELECT Result = -2
	END

Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID, @ModifiedBy
GO


----------------------------------------------------
-- dbo.AdminPrivate_DeleteAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteAccessLevel 
	@ID int,
	@JustAnotherAccessLevel int = Null
WITH ENCRYPTION
AS

-- If no replacement ID is given then search for a random one
If @JustAnotherAccessLevel Is Null
	SELECT TOP 1 @JustAnotherAccessLevel = ID FROM dbo.System_AccessLevels WHERE ID <> @ID

DELETE FROM dbo.System_AccessLevels WHERE ID = @ID


UPDATE dbo.System_ServerGroups
SET AccessLevel_Default = @JustAnotherAccessLevel
WHERE AccessLevel_Default = @ID


UPDATE dbo.Benutzer
Set AccountAccessability = @JustAnotherAccessLevel
Where AccountAccessability = @ID
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServer
	(
		@ServerID int
	)
WITH ENCRYPTION
AS

-- Script engines of connected servers will be UNREGISTERED. 
DELETE System_WebAreaScriptEnginesAuthorization
FROM System_Servers INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_Servers.ID = @ServerID 

-- Related logs will be DELETED permanently. 
DELETE Log
FROM System_Servers INNER JOIN Log ON System_Servers.IP = Log.ServerIP
WHERE System_Servers.ID = @ServerID 
DELETE System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
FROM System_Servers INNER JOIN System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes ON System_Servers.ID = System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.Server
WHERE System_Servers.ID = @ServerID 

-- Script engine relations must be erased as well
DELETE 
FROM System_WebAreaScriptEnginesAuthorization
WHERE Server = @ServerID

-- DELETE the server itself
DELETE 
FROM dbo.System_Servers
WHERE ID = @ServerID
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServerGroup
(
@ID_ServerGroup int
)
WITH ENCRYPTION
AS 

-- The corresponding public user group will be DELETED. And its items in the security admjustments table, too.
DELETE [dbo].[System_SubSecurityAdjustments]
FROM System_ServerGroups INNER JOIN [dbo].[System_SubSecurityAdjustments]
	ON System_ServerGroups.ID_Group_Public = [dbo].[System_SubSecurityAdjustments].TablePrimaryIDValue
		AND [dbo].[System_SubSecurityAdjustments].TableName='Groups'
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE Gruppen 
FROM System_ServerGroups INNER JOIN Gruppen ON System_ServerGroups.ID_Group_Public = Gruppen.ID 
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Relations between access levels and the server group will be DELETED. 
DELETE 
FROM System_ServerGroupsAndTheirUserAccessLevels 
WHERE ID_ServerGroup = @ID_ServerGroup

-- Script engines of connected servers will be UNREGISTERED. 
DELETE System_WebAreaScriptEnginesAuthorization
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Related logs will be DELETED permanently. 
DELETE Log
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Log ON System_Servers.IP = Log.ServerIP
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes ON System_Servers.ID = System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.Server
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Related applications and their authorizations will be DELETED permanently.
DELETE ApplicationsRightsByGroup
FROM ((System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.
ID = Applications_CurrentAndInactiveOnes.LocationID) INNER JOIN ApplicationsRightsByGroup ON Applications_CurrentAndInactiveOnes.ID = ApplicationsRightsByGroup.ID_Application
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE ApplicationsRightsByUser
FROM ((System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID) INNER JOIN ApplicationsRightsByUser ON Applications_CurrentAndInactiveOnes.ID = ApplicationsRightsByUser.ID_Application
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE Applications_CurrentAndInactiveOnes
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- All currently connected servers will be DELETED permanently. 
DELETE System_Servers
FROM System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Script engine relations must be erased as well
DELETE System_WebAreaScriptEnginesAuthorization
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- WebEditor/WCMS content must be purged
DELETE 
FROM [dbo].[WebManager_WebEditor]
WHERE ServerID = @ID_ServerGroup

-- DELETE the server group itself
DELETE 
FROM System_ServerGroups
WHERE System_ServerGroups.ID = @ID_ServerGroup


SET NOCOUNT OFF

GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteUser
	(
		@UserID int,
		@AdminUserID int = null
	)
WITH ENCRYPTION
AS


DELETE FROM dbo.Benutzer WHERE ID=@UserID
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson=@UserID
DELETE FROM dbo.Memberships WHERE ID_User=@UserID

DELETE FROM dbo.Log_Users WHERE ID_User = @UserID  AND [Type] IN (SELECT ValueNVarChar FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LogTypeDeletionSetting' And ValueBoolean = 1)


-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', -31, 'User deleted by admin ' + Cast(IsNull(@AdminUserID, '') as nvarchar(20)))
GO

----------------------------------------------------
-- dbo.AdminPrivate_GetCompleteUserInfo
----------------------------------------------------
ALTER Procedure dbo.AdminPrivate_GetCompleteUserInfo
(
	@UserID int
)
WITH ENCRYPTION
As
SELECT * FROM dbo.Benutzer WHERE ID = @UserID
	/* set nocount on */
	return

GO

----------------------------------------------------
-- dbo.AdminPrivate_GetScriptEnginesOfServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_GetScriptEnginesOfServer
(
@ServerID int
)
WITH ENCRYPTION
AS 
SELECT     (SELECT     WebEngine.ScriptEngine
                       FROM          System_WebAreaScriptEnginesAuthorization AS WebEngine
                       WHERE      (WebEngine.Server = @ServerID OR
                                              WebEngine.Server IS NULL) AND System_ScriptEngines.ID = WebEngine.ScriptEngine) AS ID, EngineName, ID AS ScriptEngineID,
                          CASE WHEN (SELECT      WebEngine2.Server 
                            FROM          System_WebAreaScriptEnginesAuthorization AS WebEngine2
                            WHERE      (WebEngine2.Server = @ServerID OR
                                                   WebEngine2.Server IS NULL) AND System_ScriptEngines.ID = WebEngine2.ScriptEngine) IS NULL THEN 0 ELSE 1 END AS IsActivated
FROM         System_ScriptEngines
GO

----------------------------------------------------
-- dbo.AdminPrivate_ResetLoginLockedTill
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_ResetLoginLockedTill
	(
		@ID int
	)
WITH ENCRYPTION
AS
declare @AccountAccessability tinyint
declare @LoginDisabled bit
declare @LoginLockedTill datetime
declare @CurrentLoginViaRemoteIP varchar(32)
	SET NOCOUNT ON
	SELECT @AccountAccessability = AccountAccessability, @LoginDisabled = LoginDisabled, @LoginLockedTill =LoginLockedTill, @CurrentLoginViaRemoteIP = CurrentLoginViaRemoteIP FROM Benutzer WHERE ID = @ID
	If @LoginLockedTill Is Not Null 
	Begin
		UPDATE    dbo.Benutzer
		SET LoginLockedTill = NULL
		WHERE (ID = @ID)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, URL, ConflictType) values (@ID, GetDate(), '0.0.0.0', '0.0.0.0', 'Lock status reset by Administrator', -5)
	End
	If @CurrentLoginViaRemoteIP Is Not Null
	Begin	
		update dbo.Benutzer 
		set CurrentLoginViaRemoteIP = Null 
		WHERE (ID = @ID)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, URL, ConflictType) values (@ID, GetDate(), '0.0.0.0', '0.0.0.0', 'Logout by Administrator', 99)
	End
	SET NOCOUNT OFF
	SELECT @AccountAccessability As AccountAccessability, @LoginDisabled As LoginDisabled, @LoginLockedTill As LoginLockedTill, @CurrentLoginViaRemoteIP As CurrentLoginViaRemoteIP
	RETURN 

GO

----------------------------------------------------
-- dbo.AdminPrivate_SetAuthorizationInherition
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetAuthorizationInherition 
(
@ReleasedByUserID int, 
@IDApp int, 
@InheritsFrom int
)
WITH ENCRYPTION
AS 
SET NOCOUNT ON
UPDATE    dbo.Applications
SET              AuthsAsAppID = @InheritsFrom, ModifiedBy = @ReleasedByUserID, ModifiedOn = getdate()
WHERE     (ID = @IDApp)
-- Logging
If (@InheritsFrom Is Null) 
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 31, 'Application now inhertis from nothing')
	End
Else
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 31, 'Application now inhertis from ID ' + Convert(varchar(50), @InheritsFrom))
	End
SET NOCOUNT ON
SELECT Result = -1

GO

-- fix the log items in the history
UPDATE dbo.Log
SET ConflictType = 31
WHERE ConflictType = 1 AND ApplicationID IS NOT NULL
GO
----------------------------------------------------
-- dbo.AdminPrivate_SetScriptEngineActivation
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetScriptEngineActivation
(
@ScriptEngineID int,
@ServerID int,
@Enabled bit,
@CheckMinimalActivations bit = 0
)
WITH ENCRYPTION
AS 

declare @ID int

SET NOCOUNT ON

-- CheckMinimalActivations nie bei customized Servern (ServerID < 0)
If @ServerID < 0
	SELECT @CheckMinimalActivations = 0

IF @CheckMinimalActivations = 0
	
	BEGIN
		SELECT     @ID = dbo.System_WebAreaScriptEnginesAuthorization.ID
		FROM         dbo.System_WebAreaScriptEnginesAuthorization
		WHERE Server = @ServerID AND ScriptEngine = @ScriptEngineID 
	
		If @Enabled <> 0 
			-- Enabled
			BEGIN
				IF @ID Is Null
					INSERT INTO dbo.System_WebAreaScriptEnginesAuthorization (Server, ScriptEngine)
					VALUES (@ServerID, @ScriptEngineID)
			END			
		Else 
			-- Disabled
			IF @ID Is Not Null
				DELETE FROM dbo.System_WebAreaScriptEnginesAuthorization
				WHERE ID = @ID
	END

ELSE	
	BEGIN
		SELECT   TOP 1  @ID = dbo.System_WebAreaScriptEnginesAuthorization.ID
		FROM         dbo.System_WebAreaScriptEnginesAuthorization
		WHERE Server = @ServerID

		IF @ID Is Null
			-- Activate at least ASP.NET script
			INSERT INTO dbo.System_WebAreaScriptEnginesAuthorization (Server, ScriptEngine)
			VALUES (@ServerID, 2) -- 2 = ASP.NET
	END

SET NOCOUNT OFF

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateAccessLevel 
(
	@ID int,
	@ReleasedByUserID int,
	@Title nvarchar(50),
	@Remarks ntext
)
WITH ENCRYPTION
AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null	
	BEGIN
		SET NOCOUNT ON
		UPDATE dbo.System_AccessLevels 
		SET Title = @Title, ReleasedBy = @ReleasedByUserID, Remarks = @Remarks, ModifiedBy = @ReleasedByUserID, ModifiedOn = GetDate()
		WHERE ID = @ID
		SET NOCOUNT OFF
	END
Else
	
	SELECT Result = 0



GO

--------------------------------------------------
-- dbo.AdminPrivate_UpdateApp --
--------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateApp
(
@Title varchar(255),
@TitleAdminArea nvarchar(255),
@Level1Title nvarchar(512),
@Level2Title nvarchar(512),
@Level3Title nvarchar(512),
@Level4Title nvarchar(512),
@Level5Title nvarchar(512),
@Level6Title nvarchar(512),
@Level1TitleIsHTMLCoded bit,
@Level2TitleIsHTMLCoded bit,
@Level3TitleIsHTMLCoded bit,
@Level4TitleIsHTMLCoded bit,
@Level5TitleIsHTMLCoded bit,
@Level6TitleIsHTMLCoded bit,
@NavURL varchar(512),
@NavFrame varchar(50),
@NavTooltipText nvarchar(1024),
@IsNew bit,
@IsUpdated bit,
@LocationID int,
@LanguageID int,
@ModifiedBy int,
@AppDisabled bit,
@Sort int,
@ResetIsNewUpdatedStatusOn varchar(30),
@OnMouseOver nvarchar(512),
@OnMouseOut nvarchar(512),
@OnClick nvarchar(512),
@AddLanguageID2URL bit,
@ID int
)
WITH ENCRYPTION
AS 
IF @LocationID < 0 
	UPDATE    dbo.Applications
	SET              Title = @Title, TitleAdminArea = @TitleAdminArea, Level1Title = CASE WHEN @Level1Title = '' THEN NULL ELSE @Level1Title END, 
                      Level2Title = CASE WHEN @Level2Title = '' THEN NULL ELSE @Level2Title END, Level3Title = CASE WHEN @Level3Title = '' THEN NULL 
                      ELSE @Level3Title END, Level4Title = CASE WHEN @Level4Title = '' THEN NULL ELSE @Level4Title END, 
                      Level5Title = CASE WHEN @Level5Title = '' THEN NULL ELSE @Level5Title END, Level6Title = CASE WHEN @Level6Title = '' THEN NULL 
                      ELSE @Level6Title END, Level1TitleIsHTMLCoded = @Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded = @Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded = @Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded = @Level4TitleIsHTMLCoded, 
                      Level5TitleIsHTMLCoded = @Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded = @Level6TitleIsHTMLCoded, NavURL = @NavURL, 
                      NavFrame = @NavFrame, NavTooltipText = @NavTooltipText, IsNew = @IsNew, IsUpdated = @IsUpdated, LocationID = @LocationID, 
                      LanguageID = @LanguageID, ModifiedOn = GETDATE(), ModifiedBy = @ModifiedBy, AppDisabled = @AppDisabled, Sort = @Sort, 
                      ResetIsNewUpdatedStatusOn = CONVERT(datetime, @ResetIsNewUpdatedStatusOn, 121), OnMouseOver = @OnMouseOver, 
                      OnMouseOut = @OnMouseOut, OnClick = @OnClick, AddLanguageID2URL = @AddLanguageID2URL
		,SystemAppType = @LocationID
	WHERE     (ID = @ID)
Else
	UPDATE    dbo.Applications
	SET              Title = @Title, TitleAdminArea = @TitleAdminArea, Level1Title = CASE WHEN @Level1Title = '' THEN NULL ELSE @Level1Title END, 
                      Level2Title = CASE WHEN @Level2Title = '' THEN NULL ELSE @Level2Title END, Level3Title = CASE WHEN @Level3Title = '' THEN NULL 
                      ELSE @Level3Title END, Level4Title = CASE WHEN @Level4Title = '' THEN NULL ELSE @Level4Title END, 
                      Level5Title = CASE WHEN @Level5Title = '' THEN NULL ELSE @Level5Title END, Level6Title = CASE WHEN @Level6Title = '' THEN NULL 
                      ELSE @Level6Title END, Level1TitleIsHTMLCoded = @Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded = @Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded = @Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded = @Level4TitleIsHTMLCoded, 
                      Level5TitleIsHTMLCoded = @Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded = @Level6TitleIsHTMLCoded, NavURL = @NavURL, 
                      NavFrame = @NavFrame, NavTooltipText = @NavTooltipText, IsNew = @IsNew, IsUpdated = @IsUpdated, LocationID = @LocationID, 
                      LanguageID = @LanguageID, ModifiedOn = GETDATE(), ModifiedBy = @ModifiedBy, AppDisabled = @AppDisabled, Sort = @Sort, 
                      ResetIsNewUpdatedStatusOn = CONVERT(datetime, @ResetIsNewUpdatedStatusOn, 121), OnMouseOver = @OnMouseOver, 
                      OnMouseOut = @OnMouseOut, OnClick = @OnClick, AddLanguageID2URL = @AddLanguageID2URL
	WHERE     (ID = @ID)

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateServer
(
@Enabled bit,
@IP nvarchar(32),
@ServerDescription nvarchar(200),
@ServerGroup int,
@ServerProtocol nvarchar(50),
@ServerName nvarchar(200),
@ServerPort int,
@ID int
)
WITH ENCRYPTION
AS

DECLARE @CurServerIP nvarchar(32)

DECLARE @OldServerIP nvarchar(32)
DECLARE @OldServerGroup int

SELECT @OldServerIP = (SELECT IP FROM System_Servers WHERE ID = @ID)
SELECT @OldServerGroup = (SELECT ServerGroup FROM System_Servers WHERE ID = @ID)

-- Hat sich die IP geändert?
If @OldServerIP <> @IP 
	BEGIN
		-- Kann der neue IP-Wert gespeichert werden oder existiert er schon?
		SELECT @OldServerIP = (SELECT IP FROM System_Servers WHERE IP = @IP)
		If @OldServerIP Is Not Null
			BEGIN
				RAISERROR ('IP / Host Header already exists', 16, 1
)
				RETURN
			END

		-- Logs umschreiben
		UPDATE LOG
		SET Log.ServerIP = @IP
		WHERE     (Log.ServerIP = @OldServerIP)

	END

-- Server updaten
UPDATE    dbo.System_Servers
SET              Enabled = @Enabled, IP = @IP, ServerDescription = @ServerDescription, ServerGroup = @ServerGroup, ServerProtocol = @ServerProtocol, 
                      ServerName = @ServerName, ServerPort = @ServerPort
WHERE     (ID = @ID)
GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateServerGroup
(
@ID int,
@ServerGroup nvarchar(255),
@ID_Group_Public int,
@ID_Group_Anonymous int,
@MasterServer int,
@UserAdminServer int,
@AreaImage nvarchar(512),
@AreaButton nvarchar(512),
@AreaNavTitle nvarchar(512),
@AreaCompanyFormerTitle nvarchar(512),
@AreaCompanyTitle nvarchar(512),
@AreaSecurityContactEMail nvarchar(512),
@AreaSecurityContactTitle nvarchar(512),
@AreaDevelopmentContactEMail nvarchar(512),
@AreaDevelopmentContactTitle nvarchar(512),
@AreaContentManagementContactEMail nvarchar(512),
@AreaContentManagementContactTitle nvarchar(512),
@AreaUnspecifiedContactEMail nvarchar(512),
@AreaUnspecifiedContactTitle nvarchar(512),
@AreaCopyRightSinceYear int,
@AreaCompanyWebSiteURL nvarchar(512),
@AreaCompanyWebSiteTitle nvarchar(512),
@ModifiedBy int,
@AccessLevel_Default int
)
WITH ENCRYPTION
AS

DECLARE @OldAdminServer int
DECLARE @OldMasterServer int

SELECT @OldAdminServer = UserAdminServer, @OldMasterServer = MasterServer FROM dbo.System_ServerGroups WHERE ID = @ID

UPDATE    dbo.System_ServerGroups
SET              ServerGroup = @ServerGroup, ID_Group_Public = @ID_Group_Public, ID_Group_Anonymous = @ID_Group_Anonymous, 
                      MasterServer = @MasterServer, UserAdminServer = @UserAdminServer, AreaImage = @AreaImage, AreaButton = @AreaButton, 
                      AreaNavTitle = @AreaNavTitle, AreaCompanyFormerTitle = @AreaCompanyFormerTitle, AreaCompanyTitle = @AreaCompanyTitle, 
                      AreaSecurityContactEMail = @AreaSecurityContactEMail, AreaSecurityContactTitle = @AreaSecurityContactTitle, 
                      AreaDevelopmentContactEMail = @AreaDevelopmentContactEMail, AreaDevelopmentContactTitle = @AreaDevelopmentContactTitle, 
                      AreaContentManagementContactEMail = @AreaContentManagementContactEMail, 
                      AreaContentManagementContactTitle = @AreaContentManagementContactTitle, AreaUnspecifiedContactEMail = @AreaUnspecifiedContactEMail, 
                      AreaUnspecifiedContactTitle = @AreaUnspecifiedContactTitle, AreaCopyRightSinceYear = @AreaCopyRightSinceYear, 
                      AreaCompanyWebSiteURL = @AreaCompanyWebSiteURL, AreaCompanyWebSiteTitle = @AreaCompanyWebSiteTitle, ModifiedBy = @ModifiedBy, ModifiedOn = getdate(),
                      AccessLevel_Default = @AccessLevel_Default
WHERE     (ID = @ID)

If @OldAdminServer <> @UserAdminServer 
	EXEC AdminPrivate_CreateAdminServerNavPoints @UserAdminServer, @OldAdminServer, @ModifiedBy

If @OldMasterServer <> @MasterServer 
	EXEC AdminPrivate_CreateMasterServerNavPoints @MasterServer, @OldMasterServer, @ModifiedBy

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateStatusLoginDisabled
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_UpdateStatusLoginDisabled 
(
	@Username nvarchar(50),
	@boolStatus bit
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1

		-- Password update
		UPDATE dbo.Benutzer SET LoginDisabled = @boolStatus, ModifiedOn = GetDate() WHERE LoginName = @Username
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO


ALTER PROCEDURE [dbo].[AdminPrivate_UpdateSubSecurityAdjustment]
(
	@ActionTypeSave bit,
	@UserID int,
	@TableName nvarchar(255),
	@TablePrimaryIDValue int,
	@AuthorizationType nvarchar(50),
	@ReleasedBy int	
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
		INSERT INTO System_SubSecurityAdjustments (UserID, TableName, TablePrimaryIDValue, AuthorizationType, ReleasedOn, ReleasedBy)
		VALUES (@UserID, @TableName, @TablePrimaryIDValue, @AuthorizationType, GETDATE(), @ReleasedBy)
	END
ELSE
	-- Delete
	DELETE FROM System_SubSecurityAdjustments 
	WHERE UserID = @UserID 
		AND TableName = @TableName 
		AND TablePrimaryIDValue = @TablePrimaryIDValue
		AND AuthorizationType = @AuthorizationType
GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateUserDetails 
(
	@CurUserID int,
	@WebApplication varchar(1024),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int,
	@LoginDisabled bit = 0,
	@LoginLockedTill datetime,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null,
	@DoNotLogSuccess bit = 0,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)
WITH ENCRYPTION
AS

SET NOCOUNT ON
	-- Profile update
	UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, Land = @Land, State = @State, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, AccountAccessability = @AccountAccessability, LoginDisabled = @LoginDisabled, LoginLockedTill = @LoginLockedTill, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE ID = @CurUserID
	-- Logging
	IF IsNull(@DoNotLogSuccess, 0) = 0
	BEGIN
		If IsNull(@IsUserChange, 0) = 0 
			-- admin
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 4, 'Admin with user id ' + CAST(@ModifiedBy as varchar(20)) + ' has modified profile')
		Else
			-- user change
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', -4, 'User has modified profile')
	END
	-- Write UserDetails
	Exec Int_UpdateUserDetailDataWithProfileData @CurUserID, @ModifiedBy

-- Rückgabewert
SET NOCOUNT OFF
SELECT Result = -1

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_UpdateUserPW] 
(
	@Username nvarchar(50),
	@NewPasscode varchar(4096),
	@DoNotLogSuccess bit = 0,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0,
	@LoginPWAlgorithm int = 0,
	@LoginPWNonceValue varbinary(4096) = 0x00 
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Password update
		UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate(), LoginLockedTill = Null, LoginFailures = 0, LoginPwAlgorithm = @LoginPWAlgorithm, LoginPWNonceValue = @LoginPWNonceValue WHERE LoginName = @Username
		-- Logging
		IF IsNull(@DoNotLogSuccess, 0) = 0
		BEGIN
			If IsNull(@IsUserChange, 0) = 0 
				-- admin
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription)
 				values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 5, 'Admin with user id ' + CAST(@ModifiedBy as varchar(20)) + ' has modified password')
			Else
				-- user change
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
				values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 6, 'User has modified password')
		END	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.Int_LogAuthChanges
----------------------------------------------------
ALTER PROCEDURE dbo.Int_LogAuthChanges
(
@UserID int = Null,
@GroupID int = Null,
@AppID int,
@ReleasedByUserID int = Null
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
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (IsNull(@UserID, 0), GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -8, cast(@GroupID as nvarchar(20)) + N' by user ' + cast(@ReleasedByUserID as nvarchar(20)))
	end
Else
	If @UserID Is Not Null
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -6, N'by user ' + cast(@ReleasedByUserID as nvarchar(20)))
GO


----------------------------------------------------
-- dbo.Int_UpdateUserDetailDataWithProfileData
----------------------------------------------------
ALTER Procedure Int_UpdateUserDetailDataWithProfileData
	(
		@IDUser int,
		@ModifiedBy int = 0
	)
WITH ENCRYPTION
As
DECLARE @LoginName nvarchar(50)
	-- Result and Initializing
	SELECT -1
	set nocount on
-- GetCompleteName (a little bit modified)
	DECLARE @Addresses nvarchar (20)
	DECLARE @Titel nvarchar (40)
	DECLARE @Vorname nvarchar(60)
	DECLARE @Nachname nvarchar(60)
	DECLARE @Namenszusatz nvarchar(40)
	DECLARE @CompleteName nvarchar (255)
	DECLARE @CompleteNameInclAddresses nvarchar (255)
	DECLARE @CustomerNo nvarchar(100) 
	DECLARE @SupplierNo nvarchar(100)
	DECLARE @Company nvarchar(100)
	DECLARE @eMail nvarchar (50)
	DECLARE @Sex varchar (1)
	
	SELECT @Sex = Case Anrede When 'Mr.' Then 'm' When 'Ms.' Then 'w' Else Null End, @eMail = [E-MAIL], @Vorname = Vorname, @Nachname = Nachname, @Nachname = Nachname, @Namenszusatz = Namenszusatz, @CustomerNo = CustomerNo, @SupplierNo = SupplierNo, @Company = Company, @Titel = Titel, @Addresses = Anrede FROM dbo.Benutzer WHERE ID = @IDUser

	-- Namenszusatz könnte Null sein
	If substring(@Namenszusatz,1,20) <> '' --Is Not Null
		SET @Namenszusatz = ' ' + @Namenszusatz
	Else
		SET @Namenszusatz = ''
	SET @CompleteName = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	SET @CompleteNameInclAddresses  = LTrim(RTrim(@Addresses + @Titel)) + ' ' + LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	Exec Public_SetUserDetailData @IDUser, 'CompleteName', @CompleteName, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'CompleteNameInclAddresses', @CompleteNameInclAddresses, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'Sex', @Sex, 1, @ModifiedBy
	-- e-mail address
	Exec Public_SetUserDetailData @IDUser, 'email', @eMail, 1, @ModifiedBy
	-- Other details
	Exec Public_SetUserDetailData @IDUser, 'CustomerNo', @CustomerNo, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'SupplierNo', @SupplierNo, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'Company', @Company, 1, @ModifiedBy

-- Exit

	return 
GO


IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[IsAdministratorForAuthorizations]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsAdministratorForAuthorizations]
GO
CREATE PROC [dbo].[IsAdministratorForAuthorizations]
(
	@AuthorizationType nvarchar(50),
	@AdminUserID int,
	@SecurityObjectID int
)
WITH ENCRYPTION
AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @Result int
select top 1 @Result = ID 
from System_SubSecurityAdjustments 
where 
	(
		(
			(UserID=@AdminUserID and TableName='Applications' and AuthorizationType='Owner' and TablePrimaryIDValue = @SecurityObjectID)
			OR (UserID=@AdminUserID and TableName='Applications' and AuthorizationType=@AuthorizationType and TablePrimaryIDValue = @SecurityObjectID)
			OR (UserID=@AdminUserID and TableName='Applications' and AuthorizationType='SecurityMaster' and TablePrimaryIDValue = 0)
		)
		AND @AdminUserID IN (SELECT ID FROM Benutzer WHERE ID = @AdminUserID AND LoginDisabled = 0) -- user must still be valid
		AND @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 7 AND ID_User = @AdminUserID) -- user must still be security admin
	)
	OR @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 6 AND ID_User = @AdminUserID) -- ALTERNATIVELY user must be a supervisor
IF @Result IS NULL 
	RETURN 0
ELSE
	RETURN 1
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[IsAdministratorForMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsAdministratorForMemberships]
GO
CREATE PROC [dbo].[IsAdministratorForMemberships]
(
	@AuthorizationType nvarchar(50),
	@AdminUserID int,
	@GroupID int
)
WITH ENCRYPTION
AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @Result int
select top 1 @Result = ID 
from System_SubSecurityAdjustments 
where 
	(
		(
			(UserID=@AdminUserID and TableName='Groups' and AuthorizationType='Owner' and TablePrimaryIDValue = @GroupID)
			OR (UserID=@AdminUserID and TableName='Groups' and AuthorizationType=@AuthorizationType and TablePrimaryIDValue = @GroupID)
			OR (UserID=@AdminUserID and TableName='Groups' and AuthorizationType='SecurityMaster' and TablePrimaryIDValue = 0)
		)
		AND @AdminUserID IN (SELECT ID FROM Benutzer WHERE ID = @AdminUserID AND LoginDisabled = 0) -- user must still be valid
		AND @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 7 AND ID_User = @AdminUserID) -- user must still be security admin
	)
	OR @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 6 AND ID_User = @AdminUserID) -- ALTERNATIVELY user must be a supervisor
IF @Result IS NULL 
	RETURN 0
ELSE
	RETURN 1
GO

--------------------------------------------------------------------------------------------------------------------------------
-- FIX for previous build 162 (which has been already fixed, too): Remove SP which has got schema name of the current DB user --
--------------------------------------------------------------------------------------------------------------------------------
IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo_camm_WebManager].[LogMissingExternalUserAssignment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo_camm_WebManager.LogMissingExternalUserAssignment
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[LogMissingExternalUserAssignment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.LogMissingExternalUserAssignment
GO
CREATE PROCEDURE dbo.LogMissingExternalUserAssignment
(
	@ExternalAccountSystem nvarchar(50),
	@LogonName nvarchar(100),
	@FullUserName nvarchar(150),
	@EMailAddress nvarchar(250),
	@Error ntext,
	@Remove bit
)
WITH ENCRYPTION
AS
IF @Remove = 0
	BEGIN
		-- Do an ADD if required
		DECLARE @ID int
		SELECT @ID = ID
		FROM [Log_MissingAssignmentsOfExternalAccounts]
		WHERE [ExternalAccountSystem] = @ExternalAccountSystem 
			AND [UserName] = @LogonName
		IF @ID IS NULL
			INSERT INTO [Log_MissingAssignmentsOfExternalAccounts] ([ExternalAccountSystem], [UserName], [FullName], [EMailAddress], [Error])
			VALUES (@ExternalAccountSystem, @LogonName, @FullUserName, @EMailAddress, @Error) 
	END
ELSE
	BEGIN
		-- Do a REMOVE
		DELETE 
		FROM [Log_MissingAssignmentsOfExternalAccounts]
		WHERE [ExternalAccountSystem] = @ExternalAccountSystem 
			AND [UserName] = @LogonName
	END
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[LookupUserNameByScriptEngineSessionID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[LookupUserNameByScriptEngineSessionID]
GO
CREATE PROC dbo.LookupUserNameByScriptEngineSessionID
	(
	@ServerID int,
	@ScriptEngineID int,
	@ScriptEngineSessionID nvarchar(128)
	)
WITH ENCRYPTION
AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT Benutzer.LoginName
FROM [System_WebAreasAuthorizedForSession] AS SSID
	LEFT JOIN System_UserSessions AS USID ON SSID.SessionID = USID.ID_Session
	LEFT JOIN Benutzer ON USID.ID_User = Benutzer.ID
WHERE SSID.Server = @ServerID
	AND SSID.ScriptEngine_ID = @ScriptEngineID
	AND SSID.ScriptEngine_SessionID = @ScriptEngineSessionID
GO
----------------------------------------------------
-- dbo.Public_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.Public_CreateUserAccount
(
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int = 0,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @LocationID int
DECLARE @CurUserStatus_InternalSessionID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0

	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, account will be created now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Create account
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, CreatedOn, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], LoginCount, CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, 1, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = SCOPE_IDENTITY() --(select ID from dbo.Benutzer where LoginName = @Username)
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = SCOPE_IDENTITY()
		UPDATE dbo.Benutzer SET System_SessionID = @CurUserStatus_InternalSessionID WHERE LoginName = @UserName
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		INSERT INTO dbo.System_WebAreasAuthorizedForSession
		                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
		SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
		                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
		FROM         dbo.System_Servers INNER JOIN
		                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
		WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @LocationID)
		SET NOCOUNT OFF
		-- Logging
		SET NOCOUNT ON
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 1, 'Account ' + @Username + ' created')
		SET NOCOUNT OFF
	END
Else
	-- Rückgabewert
	SELECT Result = 1
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO

----------------------------------------------------
-- dbo.Public_GetCompleteName
----------------------------------------------------
ALTER Procedure Public_GetCompleteName
(
	@Username nvarchar(50)
)
WITH ENCRYPTION
As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @Vorname nvarchar(60)
DECLARE @Nachname nvarchar(60)
DECLARE @Namenszusatz nvarchar(40)
SET @Vorname = (SELECT Vorname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Nachname = (SELECT Nachname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Namenszusatz = (SELECT Namenszusatz FROM dbo.Benutzer WHERE Loginname = @Username)
-- Namenszusatz könnte Null sein
If substring(@Namenszusatz,1,20) <> '' --Is Not Null
	SET @Namenszusatz = ' ' + @Namenszusatz
Else
	SET @Namenszusatz = ''
SELECT Result = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	/* set nocount on */
	return 
GO

----------------------------------------------------
-- dbo.Public_GetCurServerLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetCurServerLogonList
(
@ServerIP nvarchar(32)
)
WITH ENCRYPTION
AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @LocationID int
----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup

FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Abbruch
		Return
	END

---------------------------------------
-- Anmeldestellen zurückliefern --
---------------------------------------
SELECT     NULL AS ID, NULL AS SessionID, System_Servers.IP, System_Servers.ServerDescription, System_Servers.ServerProtocol, 
                      System_Servers.ServerName, System_Servers.ServerPort, NULL AS ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, NULL AS ScriptEngine_SessionID, System_WebAreaScriptEnginesAuthorization.ID AS OrderID1, 
                      System_Servers.ID AS OrderID2, System_ScriptEngines.ID AS OrderID3
FROM         System_Servers INNER JOIN
                      System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server INNER JOIN
                      System_ScriptEngines ON System_WebAreaScriptEnginesAuthorization.ScriptEngine = System_ScriptEngines.ID
WHERE     (System_Servers.Enabled <> 0) AND (System_Servers.ServerGroup = @LocationID) AND (System_Servers.ID > 0)
ORDER BY System_WebAreaScriptEnginesAuthorization.ID, System_Servers.ID, System_ScriptEngines.ID
GO

----------------------------------------------------
-- dbo.Public_GetEMailAddressesOfAllSecurityAdmins
----------------------------------------------------
ALTER Procedure dbo.Public_GetEMailAddressesOfAllSecurityAdmins
WITH ENCRYPTION
As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT Benutzer.[E-MAIL], Benutzer.ID FROM dbo.Memberships LEFT OUTER JOIN dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID WHERE (dbo.Memberships.ID_Group = 7)
	return 



GO

----------------------------------------------------
-- dbo.Public_GetLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetLogonList
	(
	@Username nvarchar(50),
	@ScriptEngine_SessionID nvarchar(512) = NULL,
	@ScriptEngine_ID int = NULL,
	@ServerID int = NULL
	)
WITH ENCRYPTION
AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

IF NOT @ScriptEngine_ID IS NULL AND NOT @ScriptEngine_SessionID IS NULL AND NOT @ServerID IS NULL
BEGIN
	-- Test for current session ID
	DECLARE @CurSessionID int
	SELECT @CurSessionID = SessionID
	FROM [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
	WHERE Inactive = 0 
		AND ScriptEngine_SessionID = @ScriptEngine_SessionID 
		AND ScriptEngine_ID = @ScriptEngine_ID
		AND Server = @ServerID
	-- If session ID is empty, then abort this procedure
	IF @CurSessionID IS NULL 
		RETURN 
END 

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
                      System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID, 
                      System_WebAreasAuthorizedForSession.LastSessionStateRefresh
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND (Benutzer.Loginname = @Username) AND (System_Servers.ID > 0)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo].[Public_GetNavPointsOfGroup]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Public_GetNavPointsOfGroup]
GO
----------------------------------------------------
-- dbo.Public_GetNavPointsOfGroup
----------------------------------------------------
CREATE Procedure [dbo].[Public_GetNavPointsOfGroup]
	@GroupID int,
	@ServerIP nvarchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0,
	@SearchForAlternativeLanguages bit = 1
WITH ENCRYPTION
As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @IsSecurityAdmin bit
DECLARE @AllowedLocation int
DECLARE @buffer int
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @AlternativeLanguage int

SET NoCount ON

-- for example: LanguageID = 512 --> also search for the alternative language with ID 2
If @SearchForAlternativeLanguages = 1
	SELECT @AlternativeLanguage = AlternativeLanguage
	FROM System_Languages
	WHERE ID = @LanguageID
Else
	SELECT @AlternativeLanguage = @LanguageID
If @AlternativeLanguage IS NULL
	SELECT @AlternativeLanguage = @LanguageID

-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Abbruch
		Return
	END

-- Application des Internet-Server
SELECT   @AllowedLocation = dbo.System_Servers.ServerGroup, @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @AllowedLocation Is Null 
	Return

--ResetIsNewUpdatedStatusOn
UPDATE dbo.Applications_CurrentAndInactiveOnes SET IsNew = 0, IsUpdated = 0, ResetIsNewUpdatedStatusOn = Null WHERE (ResetIsNewUpdatedStatusOn < GETDATE())

-- Recordset zurückgeben	
		CREATE TABLE #NavUpdatedItems_Filtered (Level1Title nvarchar(255), Level2Title nvarchar(255), Level3Title nvarchar(255), Level4Title nvarchar(255), Level5Title nvarchar(255), Level6Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Filtered (Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title)
		SELECT distinct Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_Group = @GroupID) OR (dbo.Memberships.ID_Group = @GroupID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
				AND (IsUpdated <> 0 OR IsNew <> 0)

		SET NoCount OFF

		IF @AnonymousAccess = 0
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_Group = @GroupID) OR (dbo.Memberships.ID_Group = @GroupID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage)  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title
		Else
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1)
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered

GO
----------------------------------------------------
-- dbo.Public_GetNavPointsOfUser
----------------------------------------------------
ALTER Procedure dbo.Public_GetNavPointsOfUser
(
	@UserID int,
	@ServerIP nvarchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0,
	@SearchForAlternativeLanguages bit = 1
)
WITH ENCRYPTION
As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @IsSecurityAdmin bit
DECLARE @AllowedLocation int
DECLARE @buffer int
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @AlternativeLanguage int

SET NoCount ON

-- for example: LanguageID = 512 --> also search for the alternative language with ID 2
If @SearchForAlternativeLanguages = 1
	SELECT @AlternativeLanguage = AlternativeLanguage
	FROM System_Languages
	WHERE ID = @LanguageID
Else
	SELECT @AlternativeLanguage = @LanguageID
If @AlternativeLanguage IS NULL
	SELECT @AlternativeLanguage = @LanguageID

SET @buffer = (SELECT COUNT(ID_Group) FROM dbo.view_Memberships WHERE (ID_Group = 6) AND (ID_User = @UserID))
If @buffer = 0 
	SET @IsSecurityAdmin = 0
Else
	SET @IsSecurityAdmin = 1

-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Abbruch
		Return
	END

-- Application des Internet-Server
SELECT   @AllowedLocation = dbo.System_Servers.ServerGroup, @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @AllowedLocation Is Null 
	Return

--ResetIsNewUpdatedStatusOn
UPDATE dbo.Applications_CurrentAndInactiveOnes SET IsNew = 0, IsUpdated = 0, ResetIsNewUpdatedStatusOn = Null WHERE (ResetIsNewUpdatedStatusOn < GETDATE())

-- Recordset zurückgeben	
If (@IsSecurityAdmin = 0)	-- True would be = 1

	BEGIN

		CREATE TABLE #NavUpdatedItems_Filtered (Level1Title nvarchar(255), Level2Title nvarchar(255), Level3Title nvarchar(255), Level4Title nvarchar(255), Level5Title nvarchar(255), Level6Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Filtered (Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title)
		SELECT distinct Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_User = @UserID) OR (dbo.Memberships.ID_User = @UserID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
				AND (IsUpdated <> 0 OR IsNew <> 0)

		SET NoCount OFF

		IF @AnonymousAccess = 0
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_User = @UserID) OR (dbo.Memberships.ID_User = @UserID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage)  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title
		Else
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1)
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered
	END

Else 
	BEGIN

		CREATE TABLE #NavUpdatedItems_Level1Title (Level1Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Level1Title (Level1Title)
		SELECT distinct Level1Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		CREATE TABLE #NavUpdatedItems_Level2Title (Level2Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Level2Title (Level2Title)
		SELECT distinct Level2Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		CREATE TABLE #NavUpdatedItems_Level3Title (Level3Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Level3Title (Level3Title)
		SELECT distinct Level3Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		CREATE TABLE #NavUpdatedItems_Level4Title (Level4Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Level4Title (Level4Title)
		SELECT distinct Level4Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		CREATE TABLE #NavUpdatedItems_Level5Title (Level5Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Level5Title (Level5Title)
		SELECT distinct Level5Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		CREATE TABLE #NavUpdatedItems_Level6Title (Level6Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Level6Title (Level6Title)
		SELECT distinct Level6Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		IF @AnonymousAccess = 0
			SET @AnonymousGroupID = 0 -- ungültige GroupID, damit das Ergebnis später nicht verfälscht wird

		SET NoCount OFF

		SELECT DISTINCT dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
			dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
			dbo.Applications.NavURL, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, dbo.Applications.NavToolTipText, dbo.Applications.Sort, Level1IsUpdated = Case When  #NavUpdatedItems_Level1Title.Level1Title Is Not Null Then -1 Else 0 End, Level2IsUpdated = Case When  #NavUpdatedItems_Level2Title.Level2Title Is Not Null Then -1 Else 0 End, 
			Level3IsUpdated = Case When  #NavUpdatedItems_Level3Title.Level3Title Is Not Null Then -1 Else 0 End, Level4IsUpdated = Case When  #NavUpdatedItems_Level4Title.Level4Title Is Not Null Then -1 Else 0 End, Level5IsUpdated = Case When  #NavUpdatedItems_Level5Title.Level5Title Is Not Null Then -1 Else 0 End, Level6IsUpdated = Case When  #NavUpdatedItems_Level6Title.Level6Title Is Not Null Then -1 Else 0 End, 
			dbo.Applications.AppDisabled, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL
			, Case When dbo.Applications.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End  As Level3TitleIsPresent, Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent,
			Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
		FROM ((((((dbo.Applications
			 left join #NavUpdatedItems_Level1Title on dbo.Applications.Level1Title = #NavUpdatedItems_Level1Title.Level1Title
			) left join #NavUpdatedItems_Level2Title on dbo.Applications.Level2Title = #NavUpdatedItems_Level2Title.Level2Title
			) left join #NavUpdatedItems_Level3Title on dbo.Applications.Level3Title = #NavUpdatedItems_Level3Title.Level3Title
			) left join #NavUpdatedItems_Level4Title on dbo.Applications.Level4Title = #NavUpdatedItems_Level4Title.Level4Title
			) left join #NavUpdatedItems_Level5Title on dbo.Applications.Level5Title = #NavUpdatedItems_Level5Title.Level5Title
			) left join #NavUpdatedItems_Level6Title on dbo.Applications.Level6Title = #NavUpdatedItems_Level6Title.Level6Title
			)  LEFT JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
		WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.Applications.LanguageID in (@LanguageID, @AlternativeLanguage) And dbo.Applications.Title <> 'System - Login'
		ORDER BY dbo.Applications.Sort, Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End, dbo.Applications.Level1Title, Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End, dbo.Applications.Level2Title, Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End, dbo.Applications.Level3Title, Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End, dbo.Applications.Level4Title, Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End, dbo.Applications.Level5Title, dbo.Applications.Level6Title

		DROP TABLE #NavUpdatedItems_Level1Title
		DROP TABLE #NavUpdatedItems_Level2Title
		DROP TABLE #NavUpdatedItems_Level3Title
		DROP TABLE #NavUpdatedItems_Level4Title
		DROP TABLE #NavUpdatedItems_Level5Title
		DROP TABLE #NavUpdatedItems_Level6Title
	END
GO
----------------------------------------------------
-- dbo.Public_GetServerConfig
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetServerConfig
(
@ServerIP nvarchar(32)
)
WITH ENCRYPTION
AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT     dbo.System_ServerGroups.ServerGroup AS ServerGroupDescription, dbo.System_ServerGroups.ID_Group_Public, 
                      System_Servers_1.ServerProtocol AS MasterServerProtocol, System_Servers_1.ServerName AS MasterServerName, 
                      System_Servers_1.ServerPort AS MasterServerPort, System_Servers_2.ServerProtocol AS UserAdminServerProtocol, 
                      System_Servers_2.ServerName AS UserAdminServerName, System_Servers_2.ServerPort AS UserAdminServerPort, 
                      dbo.System_ServerGroups.UserAdminServer, System_Servers_3.*, dbo.System_ServerGroups.AreaImage AS ServerGroupImageBig, 
                      dbo.System_ServerGroups.AreaButton AS ServerGroupImageSmall, COALESCE (dbo.System_ServerGroups.AreaNavTitle, 
                      dbo.System_ServerGroups.ServerGroup) AS ServerGroupTitle_Navigation, dbo.System_ServerGroups.AreaCompanyFormerTitle, 
                      dbo.System_ServerGroups.AreaCompanyTitle, dbo.System_ServerGroups.AreaSecurityContactEMail, 
                      dbo.System_ServerGroups.AreaSecurityContactTitle, dbo.System_ServerGroups.AreaDevelopmentContactEMail, 
                      dbo.System_ServerGroups.AreaDevelopmentContactTitle, dbo.System_ServerGroups.AreaContentManagementContactEMail, 
                      dbo.System_ServerGroups.AreaContentManagementContactTitle, dbo.System_ServerGroups.AreaUnspecifiedContactEMail, 
                      dbo.System_ServerGroups.AreaUnspecifiedContactTitle, dbo.System_ServerGroups.AreaCopyRightSinceYear, 
                      dbo.System_ServerGroups.AreaCompanyWebSiteURL, dbo.System_ServerGroups.AreaCompanyWebSiteTitle, 
                      dbo.System_ServerGroups.ID AS ID_ServerGroup, dbo.System_ServerGroups.AccessLevel_Default
FROM         dbo.System_ServerGroups INNER JOIN
                      dbo.System_Servers System_Servers_2 ON dbo.System_ServerGroups.UserAdminServer = System_Servers_2.ID INNER JOIN
                      dbo.System_Servers System_Servers_1 ON dbo.System_ServerGroups.MasterServer = System_Servers_1.ID INNER JOIN
                      dbo.System_Servers System_Servers_3 ON dbo.System_ServerGroups.ID = System_Servers_3.ServerGroup

WHERE     (System_Servers_3.IP = @ServerIP)
GO

----------------------------------------------------
-- dbo.Public_GetToDoLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetToDoLogonList
	(
	@Username nvarchar(50),
	@ScriptEngine_SessionID nvarchar(512),
	@ScriptEngine_ID int,
	@ServerID int
	)
WITH ENCRYPTION
AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- GUIDs alter Sessions zurücksetzen
SET NOCOUNT ON
UPDATE    System_WebAreasAuthorizedForSession WITH (XLOCK, ROWLOCK)
SET              Inactive = 1
WHERE     (LastSessionStateRefresh < DATEADD(hh, - 12, GETDATE()))

-- GUIDs alter Sessions zurücksetzen
DELETE FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes WITH (XLOCK, ROWLOCK)
WHERE(Inactive = 1 And (LastSessionStateRefresh < DateAdd(Day, -3, GETDATE())))

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
             
         System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, System_WebAreasAuthorizedForSession.ScriptEngine_ID,
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
FROM         System_WebAreasAuthorizedForSession WITH (NOLOCK) INNER JOIN
                      System_Servers WITH (NOLOCK) ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines WITH (NOLOCK) ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer WITH (NOLOCK) ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     (System_WebAreasAuthorizedForSession.ScriptEngine_SessionID IS NULL) AND (System_Servers.ID > 0) AND 
		(System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND 
		(Benutzer.Loginname = @Username)
	 OR
                (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND (System_Servers.ID > 0) AND 
		(Benutzer.Loginname = @Username) 
 			-- never show the current script engine session
			AND NOT (System_WebAreasAuthorizedForSession.ScriptEngine_SessionID = @ScriptEngine_SessionID 
				AND System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID
				AND System_WebAreasAuthorizedForSession.Server = @ServerID)
			AND System_WebAreasAuthorizedForSession.LastSessionStateRefresh < DATEADD(minute, - 3, GETDATE())
GO

----------------------------------------------------
-- dbo.Public_GetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserDetailData
	(
		@IDUser int,
		@Type varchar(50)
	)
WITH ENCRYPTION
As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

If @Type = 'Sex'
	SELECT CASE WHEN Anrede = 'Mr.' THEN 'm' Else 'w' END As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LoginName'
	SELECT LoginName As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'Addresses'
	SELECT Anrede As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LastName'
	SELECT Nachname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'FirstName'
	SELECT Vorname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '1stPreferredLanguage'
	SELECT [1stPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '2ndPreferredLanguage'
	SELECT [2ndPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '3rdPreferredLanguage'
	SELECT [3rdPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'AccessLevel'
	SELECT [AccountAccessability] As Result FROM Benutzer WHERE ID = @IDUser
Else
	SELECT Value As Result FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
GO

----------------------------------------------------
-- dbo.Public_GetUserID
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserID
(
	@Username nvarchar(50)
)
WITH ENCRYPTION
As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

declare @UserID int

set nocount on
SELECT TOP 1 @UserID = ID FROM dbo.Benutzer WHERE Loginname = @Username
set nocount off

SELECT Result = @UserID

Return @UserID
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetUserNameForScriptEngineSessionID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_GetUserNameForScriptEngineSessionID]
GO

CREATE PROCEDURE dbo.Public_GetUserNameForScriptEngineSessionID
(
	@UserName nvarchar(50) output,
	@ScriptEngine_SessionID nvarchar(128),
	@ScriptEngine_ID int,
	@ServerID int
)
WITH ENCRYPTION
AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

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

----------------------------------------------------
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
(
	@Username nvarchar(50),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int = NULL,
	@ScriptEngine_SessionID nvarchar(512) = NULL
)
WITH ENCRYPTION
AS

SET NOCOUNT ON

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @ServerID int
DECLARE @System_SessionID int
DECLARE @CurPrimarySystem_SessionID int
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @ServerID = ID FROM System_Servers WHERE IP = @ServerIP
IF @ServerID IS NULL
	BEGIN
		-- Rückgabewert
		SELECT Result = -4
		-- Abbruch
		Return
	END

IF @ScriptEngine_ID IS NULL
	-- old style pre build 111
	SELECT @CurUserID = ID, @System_SessionID = System_SessionID from dbo.Benutzer where LoginName = @Username
ELSE
	BEGIN
		-- since build 111 we've got the script engine information data to retrieve the webmanager session ID
		SELECT @CurUserID = ID, @CurPrimarySystem_SessionID = System_SessionID from dbo.Benutzer where LoginName = @Username
		SELECT TOP 1 @System_SessionID = [SessionID]
		FROM [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
		where inactive = 0
			and server = @serverid
			and scriptengine_sessionID = @ScriptEngine_SessionID
			and scriptengine_id = @ScriptEngine_ID
	END

-- Falls kein Benutzer gefunden wurde, jetzt diese Prozedur verlassen
IF @CurUserID IS NULL
	BEGIN
		-- Rückgabewert
		SELECT Result = -9
		-- Abbruch
		Return
	END

-------------
-- Logout --
-------------
-- CurUserCurrentLoginViaRemoteIP und SessionIDs zurücksetzen
if @ScriptEngine_ID <> -1 -- not a Net client (stand-alone)
BEGIN
	IF @CurPrimarySystem_SessionID = @System_SessionID -- if the primary system session ID has been the current system session id
	BEGIN
		UPDATE dbo.Benutzer 
		SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null WHERE LoginName = @Username
	END
END

-- Session schließen
UPDATE dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
SET Inactive = 1
WHERE [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].inactive = 0 AND SessionID = @System_SessionID
-- Ggf. weitere Sessions schließen, welche noch offen sind und von der gleichen Browsersession geöffnet wurden
-- Konkreter Beispielfall: 
-- 2 Browserfenster wurden geöffnet, die Cookies und damit die Sessions sind die gleichen; 
-- in beiden Browsern wurde zeitnah eine Anmeldung unterschiedlicher Benutzer ausgeführt und damit 2 Sessions erstellt, 
-- wobei die zweite Session durch ein Logout i. d. R. geschlossen würde, die zweite Session aber geöffnet bleibt; 
-- dies würde zu einem Sicherheitsleck und zu Verwirrung im Programmablauf führen, da anhand der SessionID 
-- der aktuellen Scriptsprache doch wieder eine Session herausgefunden werden könnte und auch wieder 
-- aktiviert würde, auch wenn es ein anderer Benutzer wäre; man wäre dann mit dessen Identität angemeldet!!!
update [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
set inactive = 1
where [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].inactive = 0 AND sessionid in (
	select RowsByScriptEngines.sessionid
	from [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] as RowsBySession
		inner join [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] as RowsByScriptEngines
			on RowsBySession.servergroup = RowsByScriptEngines.servergroup
			and RowsBySession.server = RowsByScriptEngines.server
			and RowsBySession.scriptengine_sessionid = RowsByScriptEngines.scriptengine_sessionid
			and RowsBySession.scriptengine_id = RowsByScriptEngines.scriptengine_id
	where RowsBySession.sessionid = @System_SessionID
		and RowsByScriptEngines.Inactive = 0
	)
-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 99, 'Logout')

SET NOCOUNT OFF

-- Rückgabewert
SELECT Result = -1

GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_PreValidateUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_PreValidateUser]
GO
CREATE PROCEDURE [dbo].[Public_PreValidateUser]
AS 
GO
----------------------------------------------------
-- dbo.Public_PreValidateUser
----------------------------------------------------
ALTER PROCEDURE dbo.Public_PreValidateUser
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@MaxLoginFailures int = 7
WITH ENCRYPTION
AS
-- Validates the user credentials, but doesn't log in
-- BUT: invalid credentials increase the number of login failures

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
DECLARE @CurUserCurrentLoginViaRemoteIP nvarchar(32)
DECLARE @CurUserAccountAccessability int
DECLARE @LoginFailureDelayHours float
DECLARE @position int
DECLARE @MyResult int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
DECLARE @LocationID int		-- ServerGroup
DECLARE @ServerID int
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @PasswordAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Logged_ScriptEngine_SessionID nvarchar(512)

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SET NOCOUNT ON

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn, @bufferLastLoginRemoteIP = LastLoginViaRemoteIP
FROM dbo.Benutzer 
WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@Username,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 58
		-- Abbruch
		Return
	END
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '') Or (IsNull(@ScriptEngine_SessionID,'') = '') Or (IsNull(@ScriptEngine_ID,0) = 0)
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END
If @CurUserAccountAccessability Is Null
	-- Benutzer nicht gefunden
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 43
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup, @ServerID = ID
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

--------------------------------------------------
-- Server-Zugriff durch Benutzer erlaubt? --
--------------------------------------------------
SELECT     @ServerIsAccessable = COUNT(*)
	FROM         System_ServerGroupsAndTheirUserAccessLevels INNER JOIN
                      System_Servers ON System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = System_Servers.ServerGroup
	WHERE     (System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = @CurUserAccountAccessability) AND (System_Servers.IP = @ServerIP)
If @ServerIsAccessable Is Null 
	SELECT @ServerIsAccessable = 0
If @ServerIsAccessable = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -9
		-- Abbruch
		Return
	END

------------------------------------------------------------------------
-- Überprüfung, ob bereits (an anderer Station) angemeldet --
------------------------------------------------------------------------
SELECT @WebSessionTimeOut = dbo.System_Servers.WebSessionTimeOut, @ReAuthByIPPossible = dbo.System_Servers.ReAuthenticateByIP, @LoginFailureDelayHours = dbo.System_Servers.LockTimeout
	FROM dbo.System_Servers
	WHERE dbo.System_Servers.IP = @ServerIP
If dateadd(minute,  + @WebSessionTimeOut, @bufferLastLoginOn) > GetDate() 
	SELECT @CurrentlyLoggedOn = 1
If @CurrentlyLoggedOn = 1
	-- Anmeldung vorhanden, jedoch evtl. an der gleichen Station (vergleiche mit SessionID) und dann genehmigt
	BEGIN
		SELECT @CurUserStatus_InternalSessionID = System_SessionID FROM dbo.Benutzer WHERE LoginName = @UserName
		If @CurUserStatus_InternalSessionID Is Not Null 
			BEGIN
				-- Stimmt die übermittelte SessionID der ScriptSprache mit der protokollierten überein?
				select @Logged_ScriptEngine_SessionID = scriptengine_sessionid from System_WebAreasAuthorizedForSession where sessionid=@CurUserStatus_InternalSessionID and scriptengine_id = @ScriptEngine_ID -- and server=@ServerID 
				IF @Logged_ScriptEngine_SessionID Is Not Null 
					IF @Logged_ScriptEngine_SessionID = @ScriptEngine_SessionID 
						-- Anmeldung mit gleicher Session erlaubt
						SELECT @CurrentlyLoggedOn = 0
					Else
						-- Anmeldung bereits von anderer Session vorliegend
						SELECT @CurrentlyLoggedOn = 1 	--Standardlogin ohne ForceLogin - Login derzeit noch nicht gewährt
				Else
					-- Session-Anmeldungseintrag nicht (mehr) vorhanden
					SELECT @CurrentlyLoggedOn = 0
	
			END
		Else
			-- sollte eigentlich nicht vorkommen, falls aber doch...lass eine Übernahme der Anmeldung zu...
			SET @CurrentlyLoggedOn = 0
	END

------------------------------
-- UserLoginValidierung --
------------------------------
If (@CurUserLoginDisabled = 1)
	BEGIN
		-- Konto gesperrt - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 44
		Return
	END
If  @CurUserLoginLockedTill > GetDate()
	BEGIN
		-- LoginSperre aktiv - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------

-- Passwortvergleich starten
SET @PasswordAuthSuccessfull = 0
If @Passcode Is Null
	-- for single-sign-on scenarios
	SET @PasswordAuthSuccessfull = 1 
Else
Begin
	-- the standard case: validate username and password against the database 
	If (@CurUserPW = @Passcode)
		-- Passwörter bis auf Groß-/Kleinschreibung schon mal identisch
		BEGIN
			-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
			-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
			SET @position = 0
			WHILE @position <= DATALENGTH(@Passcode)
				BEGIN
					IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
						BEGIN
							SET @PasswordAuthSuccessfull = 0
						BREAK
						END
					ELSE
						SET @PasswordAuthSuccessfull = 1
					SET @position = @position + 1
				END
		END
End

-- Passwortvergleich erfolgreich?
If @PasswordAuthSuccessfull = 1
	SET @MyResult = 1 -- Zugriff gewährt
Else
	SET @MyResult = 0 -- Passwortvergleich ergab Unterschiede


IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, @CurrentlyLoggedOn AS CurrentlyLoggedOn FROM dbo.Benutzer WHERE LoginName = @Username
		SET NOCOUNT ON
	END
Else -- @MyResult = 0
	-- Login failed
	BEGIN
		IF @CurUserLoginFailures = @MaxLoginFailures - 1
			-- LoginFailure Maximum nun erreicht
			BEGIN	
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -2	
				SET NOCOUNT ON
				-- Zeitliche Loginsperre setzen
				update dbo.Benutzer set LoginLockedTill = dateadd(hour, @LoginFailureDelayHours, getdate()) where LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -95, 'Login disabled for ' + cast(@LoginFailureDelayHours as nvarchar(5)) + ' hours')
			END	
		Else
			BEGIN
				SET NOCOUNT OFF
				If @MyResult = 0 -- Weitere Logins möglich - Rückgabewert
					SELECT Result = 0
				Else
					SELECT Result = -5
				SET NOCOUNT ON
			END
		-- Wert LoginFailures erhöhen 
		update dbo.Benutzer set LoginFailures = @CurUserLoginFailures + 1 where LoginName = @Username
	END
GO


----------------------------------------------------
-- dbo.Public_RestorePassword
----------------------------------------------------
ALTER Procedure dbo.Public_RestorePassword
(
		@Username nvarchar(50),
		@eMail nvarchar(50)
)
WITH ENCRYPTION
AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT Result = (SELECT SUBSTRING(LoginPW, 1, len(LoginPW)) FROM dbo.Benutzer WHERE Loginname = @Username And [e-mail] = @eMail)
GO

----------------------------------------------------
-- [dbo].[Public_ServerDebug]
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ServerDebug
(
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32)
)
WITH ENCRYPTION
AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- Deklaration Variablen/Konstanten
DECLARE @WebApplication varchar(1024)
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime

DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Registered_ScriptEngine_SessionID varchar(512)
DECLARE @RequestedServerID int
DECLARE @WebAppID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @WebApplication = 'Public'

SET NOCOUNT ON

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return

	END
----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID)))

------------------------------
-- UserLoginValidierung --
------------------------------

-- ReAuthentifizierung?
	-- Does the user has got authorization?
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ApplicationsRightsByGroup.ID_GroupOrPerson FROM Memberships RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID) AND (ApplicationsRightsByGroup.ID_GroupOrPerson = @PublicGroupID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships.ID_User FROM Memberships RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (((Memberships.ID_User = @CurUserID) AND (Applications.Title = @WebApplication))) AND Applications.LocationID = @LocationID)
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6))
		If NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
			SET @MyResult = 1 -- Zugriff gewährt
		Else
			SET @MyResult = 2 -- kein Zugriff auf aktuelles Dokument

IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		SET NOCOUNT ON
	END
Else -- @MyResult = 0 Or @MyResult = 2
	-- Login failed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2	
		SET NOCOUNT ON
	END
GO

----------------------------------------------------
-- dbo.Public_SetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_SetUserDetailData
	(
		@IDUser int,
		@Type varchar(50),
		@Value nvarchar(255),
		@DoNotLogSuccess bit = 0,
		@ModifiedBy int = 0
	)
WITH ENCRYPTION
AS
DECLARE @CountOfValuesInTable int

	set nocount on
	-- How many rows exist?
	SET @CountOfValuesInTable = (SELECT COUNT(ID) FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type)
	
	If @CountOfValuesInTable > 0 
		-- Remove old settings first
		DELETE FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
	-- Append value to table
	If @Type Is Not Null And @Value Is Not Null 
		INSERT INTO dbo.Log_Users (ID_User, Type, Value, ModifiedBy) VALUES (@IDUser, @Type, @Value, @ModifiedBy) 
	-- Logging
	if @DoNotLogSuccess = 0 
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@IDUser, GetDate(), '0.0.0.0', '0.0.0.0', -9, 'User attributes modified by user id ' + CAST(@ModifiedBy as varchar(20)))
	-- Exit
	set nocount off
	SELECT -1
	return
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UpdateUserDetails]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_UpdateUserDetails]
GO

----------------------------------------------------
-- dbo.Public_UpdateUserDetails
----------------------------------------------------
CREATE PROCEDURE dbo.Public_UpdateUserDetails 
(
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication nvarchar(1024),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures int
DECLARE @CurUserAccountAccessability int
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @Passcode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@Passcode)
			BEGIN
				IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, State = @State, Land = @Land, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -4, 'User profile changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO

----------------------------------------------------
-- dbo.Public_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE [dbo].[Public_UpdateUserPW] 
(
	@Username nvarchar(50),
	@OldPasscode varchar(4096),
	@NewPasscode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication varchar(4096),
	@LoginPWAlgorithm int = 0,
	@LoginPWNonceValue varbinary(4096) = 0x00 
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @OldPasscode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnt die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@OldPasscode)
			BEGIN
				IF ASCII(SUBSTRING(@OldPasscode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate(), LoginPwAlgorithm = @LoginPWAlgorithm, LoginPWNonceValue = @LoginPWNonceValue WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 6, 'User password changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.Public_UserIsAuthorizedForApp
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UserIsAuthorizedForApp
(
	@Username nvarchar(50),
	@WebApplication varchar(255),
	@ServerIP nvarchar(32)
)
WITH ENCRYPTION
AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @CurUserID int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByAnonymousGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @RequestedServerID int
DECLARE @OneOfTheSeveralAppIDs int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

------------------------------
-- UserLoginValidierung --
------------------------------

		If IsNull(@WebApplication, '') = 'Public' And @CurUserID Is Not Null
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @OneOfTheSeveralAppIDs = ID FROM dbo.Applications WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID)
		If @OneOfTheSeveralAppIDs Is Null
			Return 0 -- Security object nicht vorhanden
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT TOP 1 @bufferUserIDByAnonymousGroup = ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID)
		If IsNull(@bufferUserIDByAnonymousGroup, -1) <> -1
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByPublicGroup = ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @PublicGroupID)
		If IsNull(@bufferUserIDByPublicGroup, -1) <> -1 And @CurUserID Is Not Null
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByUser = ID_User FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_User = @CurUserID)
		If IsNull(@bufferUserIDByUser, -1) <> -1 
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByGroup = Memberships.ID_User FROM Memberships INNER JOIN view_ApplicationRights ON Memberships.ID_Group = view_ApplicationRights.ID_Group WHERE (view_ApplicationRights.Title = @WebApplication) AND (view_ApplicationRights.LocationID = @RequestedServerID) AND (Memberships.ID_User = @CurUserID)
		If IsNull(@bufferUserIDByGroup, -1) <> -1
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByAdmin = ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6)
		If IsNull(@bufferUserIDByAdmin, -1) <> -1 
			Return 1 -- Zugriff gewährt
		Else
			Return 0 -- kein Zugriff auf aktuelles Dokument

GO

----------------------------------------------------
-- dbo.Public_ValidateDocument
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateDocument
(
	@Username nvarchar(50),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication nvarchar(1024),
	@WebURL nvarchar(1024),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@Reserved int = Null
)
WITH ENCRYPTION
AS

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime

DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
DECLARE @CurUserCurrentLoginViaRemoteIP nvarchar(32)
DECLARE @MaxLoginFailures int
DECLARE @CurUserAccountAccessability int
DECLARE @LoginFailureDelayHours float
DECLARE @position int
DECLARE @MyResult int
DECLARE @Dummy bit
DECLARE @bufferUserIDByAnonymousGroup int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Registered_ScriptEngine_SessionID nvarchar(512)
DECLARE @RequestedServerID int
DECLARE @WebAppID int
DECLARE @LoggingSuccess_Disabled bit

SET NOCOUNT ON

-- Konstanten setzen
SET @MaxLoginFailures = 3

-- Reserved-Parameter auswerten
IF @Reserved = 1
	SELECT @LoggingSuccess_Disabled = 1
ELSE
	SELECT @LoggingSuccess_Disabled = 0

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn
FROM dbo.Benutzer  WITH (XLOCK, ROWLOCK)
WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@WebApplication,'') = '')
	-- No application title --> anonymous access allowed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, Null As LoginName, Null As System_SessionID
		-- Abbruch
		Return
	END
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return

	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID)) ORDER BY AppDeleted ASC)
If @WebAppID Is Null And @WebApplication Not Like 'Public'
	BEGIN
		SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
		PRINT 'Error resolving security object ID for logging purposes'
		RETURN
	END

--------------------------------------------------
-- Anonyme Userberechtigungen checken --
--------------------------------------------------
If (IsNull(@Username,'') = '')
	BEGIN
		-- Is Application available for anonymous access?
		SELECT @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT @bufferUserIDByAnonymousGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID))
		If IsNull(@bufferUserIDByAnonymousGroup, -1) <> -1
			-- Zugriff gewährt
			BEGIN
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (-1, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, 0)
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -1, Null As LoginName, Null As System_SessionID
				-- Abbruch
				Return
			END
		Else
			-- Login required
			BEGIN
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (-1, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, -25)
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = 58
				-- Abbruch
				Return
			END
	END

--------------------------------------------------
-- Server-Zugriff durch Benutzer erlaubt? --
--------------------------------------------------
If @CurUserAccountAccessability Is Null
	-- Benutzer nicht gefunden
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 43
		-- Abbruch
		Return
	END
SELECT     @ServerIsAccessable = COUNT(*)
	FROM         System_ServerGroupsAndTheirUserAccessLevels INNER JOIN       System_Servers ON System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = System_Servers.ServerGroup
	WHERE     (System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = @CurUserAccountAccessability) AND (System_Servers.IP = @ServerIP)
If @ServerIsAccessable Is Null 
	SELECT @ServerIsAccessable = 0
If @ServerIsAccessable = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -9
		-- Abbruch
		Return
	END

------------------------------------------------------------------------
-- Überprüfung, ob bereits (an anderer Station) angemeldet --
------------------------------------------------------------------------
SELECT @WebSessionTimeOut = dbo.System_Servers.WebSessionTimeOut, @ReAuthByIPPossible = dbo.System_Servers.ReAuthenticateByIP, @LoginFailureDelayHours = dbo.System_Servers.LockTimeout
	FROM dbo.System_Servers
	WHERE dbo.System_Servers.IP = @ServerIP
If dateadd(minute,  + @WebSessionTimeOut, @bufferLastLoginOn) > GetDate() And (@CurUserStatus_InternalSessionID Is Not Null)
	SELECT @CurrentlyLoggedOn = 1

---------------------------------------------------------------------------------
-- Versuch der Reauthentifizierung durch die mitgelieferte SessionID --
---------------------------------------------------------------------------------
SELECT @ReAuthSuccessfull = 0 -- Variablen-Initialisierung
SELECT @bufferLastLoginRemoteIP = (select LastLoginViaRemoteIP from dbo.Benutzer where LoginName = @Username)
SELECT     @Registered_ScriptEngine_SessionID = System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
	FROM         Benutzer INNER JOIN
                      System_WebAreasAuthorizedForSession ON Benutzer.System_SessionID = System_WebAreasAuthorizedForSession.SessionID
	WHERE     (Benutzer.ID = @CurUserID) AND (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND ScriptEngine_SessionID = @ScriptEngine_SessionID
If @Registered_ScriptEngine_SessionID = @ScriptEngine_SessionID
	SELECT @ReAuthSuccessfull = 1
If @ReAuthByIPPossible <> 0 And @ReAuthSuccessfull = 0
	SELECT @ReAuthSuccessfull = 0
If @CurrentlyLoggedOn = 1 And @ReAuthSuccessfull = 0
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, -98, 'Currently logged in on host ' + @bufferLastLoginRemoteIP + ' or with a different session ID, CurrentlyLoggedOn = ' + Cast(@CurrentlyLoggedOn as varchar(30)) + ', ReAuthSuccessfull = ' + Cast(@ReAuthSuccessfull as varchar(30)) + ', Login denied')
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -4
		-- Abbruch
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------
If (@CurUserLoginDisabled = 1)
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, -28, 'Account disabled')
		-- Konto gesperrt - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 44
		Return
	END
If  @CurUserLoginLockedTill > GetDate()
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, -29, 'Account locked temporary')
		-- LoginSperre aktiv - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------

-- ReAuthentifizierung?
If @ReAuthSuccessfull = 1
	-- Does the user has got authorization?
	BEGIN
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT @bufferUserIDByAnonymousGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID))
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @PublicGroupID))
		SELECT @bufferUserIDByUser = (SELECT DISTINCT ID_User FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_User = @CurUserID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships.ID_User FROM Memberships INNER JOIN view_ApplicationRights ON Memberships.ID_Group = view_ApplicationRights.ID_Group WHERE (view_ApplicationRights.Title = @WebApplication) AND (view_ApplicationRights.LocationID = @RequestedServerID) AND (Memberships.ID_User = @CurUserID))
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6))
		If IsNull(@bufferUserIDByAnonymousGroup, -1) <> -1 Or IsNull(@bufferUserIDByPublicGroup, -1) <> -1 Or IsNull(@bufferUserIDByUser, -1) <> -1 Or IsNull(@bufferUserIDByGroup, -1) <> -1 Or IsNull(@bufferUserIDByAdmin, -1) <> -1 Or IsNull(@WebApplication, '') = 'Public'
			SET @MyResult = 1 -- Zugriff gewährt
		Else
			SET @MyResult = 2 -- kein Zugriff auf aktuelles Dokument
	END
Else
	SET @MyResult = 0 -- Reauthentifizierung schlug fehl - Neuanmeldung erforderlich

IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- LoginRemoteIP und SessionIDs setzen
		update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), CurrentLoginViaRemoteIP = @RemoteIP where LoginName = @Username --, SessionID_ASP = @CurUserStatus_SessionID_ASP, SessionID_ASPX = @CurUserStatus_SessionID_ASPX, SessionID_SAP = @CurUserStatus_SessionID_SAP 
		-- LoginCount hochzählen
		If @LoggingSuccess_Disabled = 0 
			update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1 where LoginName = @Username
		-- LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		If @LoggingSuccess_Disabled = 0 
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, 0)
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		SELECT @CurUserStatus_InternalSessionID = System_SessionID 
		FROM dbo.Benutzer 
		WHERE LoginName = @Username
		SELECT @CurrentlyLoggedOn = 0
		-- WebAreaSessionState aktualisieren
		update dbo.System_WebAreasAuthorizedForSession set LastSessionStateRefresh = getdate() where ScriptEngine_ID = @ScriptEngine_ID and SessionID = @CurUserStatus_InternalSessionID And Server = @RequestedServerID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, LoginName, @CurUserStatus_InternalSessionID As System_SessionID
		FROM dbo.Benutzer
		WHERE LoginName = @Username
		SET NOCOUNT ON
	END
Else -- @MyResult = 0 Or @MyResult = 2
	-- Login failed
	BEGIN
		IF @CurUserLoginFailures = @MaxLoginFailures - 1
			-- LoginFailure Maximum nun erreicht
			BEGIN	
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -2	
				SET NOCOUNT ON
				-- Zeitliche Loginsperre setzen
				update dbo.Benutzer set LoginLockedTill = getdate() + 1.0/24*@LoginFailureDelayHours where LoginName = @Username
			END	
		Else
			BEGIN
				SET NOCOUNT OFF
				If @MyResult = 0 -- Weitere Logins möglich - Rückgabewert
					SELECT Result = 57	 -- Reauthentifizierung schlug fehl - Neuanmeldung erforderlich
				Else
					SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
					PRINT 'No access to current document'
				SET NOCOUNT ON
			END
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, -27)
	END

-- Wenn @CurUserLoginLockedTill vorhanden und älter als aktuelles Systemdatum, LoginFailureFields zurücksetzen
If Not (@CurUserLoginLockedTill > GetDate())

	If @MyResult = 1 Or @MyResult = 2
		-- Reauth-check successful --> LoginFailures = 0
		-- (Access may be granted or not - the password check has been successfull and so there aren no really LoginFailures;
		-- if you would say it is one, then the user would try to access a locked modul 3 times and after this he would be locked by the system...)
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
GO

----------------------------------------------------
-- dbo.Public_ValidateGUIDLogin
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateGUIDLogin
(
	@Username nvarchar(50),
	@GUID int,
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512)
)
WITH ENCRYPTION
AS

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
DECLARE @CurUserID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

If @CurUserID Is Null
	-- User does not exist
	Return 1

UPDATE dbo.System_WebAreasAuthorizedForSession SET ScriptEngine_SessionID = @ScriptEngine_SessionID, LastSessionStateRefresh = GetDate()
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     
                      (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID = @GUID) AND (Benutzer.Loginname = @Username) AND 
                      (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND (System_Servers.IP = @ServerIP)

IF @@ROWCOUNT = 1 
	insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
	values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 97, 'Prepare GUID login - Script Engine #' + Cast(@ScriptEngine_ID As NVarchar(20)))
Else
	-- User has no such ScriptEngine_SessionID
	Return 2

SET NOCOUNT OFF

-- Erfolgsmitteilung
Select @UserName
Return -1
GO

----------------------------------------------------
-- dbo.Public_ValidateUser
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateUser
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@ForceLogin bit,
	@MaxLoginFailures int = 7
WITH ENCRYPTION
AS

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
DECLARE @CurUserCurrentLoginViaRemoteIP nvarchar(32)
DECLARE @CurUserAccountAccessability int
DECLARE @LoginFailureDelayHours float
DECLARE @position int
DECLARE @MyResult int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
DECLARE @LocationID int		-- ServerGroup
DECLARE @ServerID int
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @PasswordAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Logged_ScriptEngine_SessionID nvarchar(512)

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SET NOCOUNT ON

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn, @bufferLastLoginRemoteIP = LastLoginViaRemoteIP
FROM dbo.Benutzer  WITH (XLOCK, ROWLOCK)
WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@Username,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 58
		-- Abbruch
		Return
	END
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '') Or (IsNull(@ScriptEngine_SessionID,'') = '') Or (IsNull(@ScriptEngine_ID,0) = 0)
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END
If @CurUserAccountAccessability Is Null
	-- Benutzer nicht gefunden
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 43
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup, @ServerID = ID
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

--------------------------------------------------
-- Server-Zugriff durch Benutzer erlaubt? --
--------------------------------------------------
SELECT     @ServerIsAccessable = COUNT(*)
	FROM         System_ServerGroupsAndTheirUserAccessLevels INNER JOIN
                      System_Servers ON System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = System_Servers.ServerGroup
	WHERE     (System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = @CurUserAccountAccessability) AND (System_Servers.IP = @ServerIP)
If @ServerIsAccessable Is Null 
	SELECT @ServerIsAccessable = 0
If @ServerIsAccessable = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -9
		-- Abbruch
		Return
	END

------------------------------------------------------------------------
-- Überprüfung, ob bereits (an anderer Station) angemeldet --
------------------------------------------------------------------------
SELECT @WebSessionTimeOut = dbo.System_Servers.WebSessionTimeOut, @ReAuthByIPPossible = dbo.System_Servers.ReAuthenticateByIP, @LoginFailureDelayHours = dbo.System_Servers.LockTimeout
	FROM dbo.System_Servers
	WHERE dbo.System_Servers.IP = @ServerIP
If dateadd(minute,  + @WebSessionTimeOut, @bufferLastLoginOn) > GetDate() 
	SELECT @CurrentlyLoggedOn = 1
If @CurrentlyLoggedOn = 1
	-- Anmeldung vorhanden, jedoch evtl. an der gleichen Station (vergleiche mit SessionID) und dann genehmigt
	BEGIN
		SELECT @CurUserStatus_InternalSessionID = System_SessionID FROM dbo.Benutzer WHERE LoginName = @UserName
		If @CurUserStatus_InternalSessionID Is Not Null 
			BEGIN
				-- Stimmt die übermittelte SessionID der ScriptSprache mit der protokollierten überein?
				select @Logged_ScriptEngine_SessionID = scriptengine_sessionid from System_WebAreasAuthorizedForSession where sessionid=@CurUserStatus_InternalSessionID and scriptengine_id = @ScriptEngine_ID -- and server=@ServerID 
				IF @Logged_ScriptEngine_SessionID Is Not Null 
					IF @Logged_ScriptEngine_SessionID = @ScriptEngine_SessionID 
						-- Anmeldung mit gleicher Session erlaubt
						SELECT @CurrentlyLoggedOn = 0
					Else
						-- Anmeldung bereits von anderer Session vorliegend
						IF @ForceLogin <> 0 
							SELECT @CurrentlyLoggedOn = 0 	--Attribut ForceLogin wurde mitgegeben, Anmeldung erfolgt!
						Else
							SELECT @CurrentlyLoggedOn = 1 	--Standardlogin ohne ForceLogin - Login derzeit noch nicht gewährt
				Else
					-- Session-Anmeldungseintrag nicht (mehr) vorhanden
					SELECT @CurrentlyLoggedOn = 0
	
			END
		Else
			-- sollte eigentlich nicht vorkommen, falls aber doch...lass eine Übernahme der Anmeldung zu...
			SET @CurrentlyLoggedOn = 0
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -98, 'Already logged on (TimeDiff): CurUserStatus_InternalSessionID: ' +  cast(@CurUserStatus_InternalSessionID as nvarchar(10)))
	END
If @CurrentlyLoggedOn = 1
	-- Abbruch der Authentifizierung
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -98, 'Currently logged in on host ' + @bufferLastLoginRemoteIP + ' or with a different session ID, CurrentlyLoggedOn = ' + Cast(@CurrentlyLoggedOn as varchar(30)) + ', Login denied')
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -4, LastRemoteIP = @bufferLastLoginRemoteIP
		-- Abbruch
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------
If (@CurUserLoginDisabled = 1)
	BEGIN
		-- Konto gesperrt - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 44
		Return
	END
If  @CurUserLoginLockedTill > GetDate()
	BEGIN
		-- LoginSperre aktiv - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------

-- Passwortvergleich starten
SET @PasswordAuthSuccessfull = 0
If @Passcode Is Null
	-- for single-sign-on scenarios
	SET @PasswordAuthSuccessfull = 1 
Else
Begin
	-- the standard case: validate username and password against the database 
	If (@CurUserPW = @Passcode)
		-- Passwörter bis auf Groß-/Kleinschreibung schon mal identisch
		BEGIN
			-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
			-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
			SET @position = 0
			WHILE @position <= DATALENGTH(@Passcode)
				BEGIN
					IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
						BEGIN
							SET @PasswordAuthSuccessfull = 0
						BREAK
						END
					ELSE
						SET @PasswordAuthSuccessfull = 1
					SET @position = @position + 1
				END
		END
End

-- Passwortvergleich erfolgreich?
If @PasswordAuthSuccessfull = 1
	SET @MyResult = 1 -- Zugriff gewährt
Else
	SET @MyResult = 0 -- Passwortvergleich ergab Unterschiede


IF @MyResult = 1
	-- Login successfull
	BEGIN

		-- Wenn @CurUserLoginLockedTill vorhanden und älter als aktuelles Systemdatum, LoginFailureFields zurücksetzen
		If Not (@CurUserLoginLockedTill > GetDate())
			-- Password check successful --> LoginFailures = 0
			update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- LoginCount hochzählen, LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1, LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 98, 'Validation of login data successfull')
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = SCOPE_IDENTITY()
		-- following action if not a client access through a stand-alone application
		-- (offline applications should not disturb any online sessions)
		if @ScriptEngine_ID = -1 -- Net client (stand-alone)
			BEGIN
				-- An welchen Systemen ist noch eine Anmeldung erforderlich?
				INSERT INTO dbo.System_WebAreasAuthorizedForSession
				                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID, ScriptEngine_SessionID)
				SELECT     dbo.System_Servers.ServerGroup, dbo.System_servers.id, 
				                      -1, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID, @ScriptEngine_SessionID
				FROM         dbo.System_Servers 
				WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ID = @ServerID)
			END
		else -- Web clients
			BEGIN
				-- LoginRemoteIP 
				update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), CurrentLoginViaRemoteIP = @RemoteIP, System_SessionID = @CurUserStatus_InternalSessionID where LoginName = @Username
				-- An welchen Systemen ist noch eine Anmeldung erforderlich?
				INSERT INTO dbo.System_WebAreasAuthorizedForSession
				                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
				SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
				                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
				FROM         dbo.System_Servers INNER JOIN
				                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
				WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @LocationID)
				-- Weitere Sessions des gleichen Benutzers schließen, welche evtl. an anderen Servern nicht ausgeloggt wurden
				update [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
				set inactive = 1
				from [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] inner join [System_UserSessions]
					on [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].sessionid = [System_UserSessions].ID_Session
				where [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].inactive = 0 AND [System_UserSessions].ID_User = @CurUserID and sessionid <> @CurUserStatus_InternalSessionID
				-- Ggf. weitere Sessions schließen, welche noch offen sind und von der gleichen Browsersession geöffnet wurden
				-- Konkreter Beispielfall: 
				-- 2 Browserfenster wurden geöffnet, die Cookies und damit die Sessions sind die gleichen; 
				-- in beiden Browsern wurde zeitnah eine Anmeldung unterschiedlicher Benutzer ausgeführt und damit 2 Sessions erstellt, 
				-- wobei die zweite Session durch ein Logout i. d. R. geschlossen würde, die zweite Session aber geöffnet bleibt; 
				-- dies würde zu einem Sicherheitsleck und zu Verwirrung im Programmablauf führen, da anhand der SessionID 
				-- der aktuellen Scriptsprache doch wieder eine Session herausgefunden werden könnte und auch wieder 
				-- aktiviert würde, auch wenn es ein anderer Benutzer wäre; man wäre dann mit dessen Identität angemeldet!!!
				update [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
				set inactive = 1
				where [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].inactive = 0 AND sessionid <> @CurUserStatus_InternalSessionID and sessionid in (
					select RowsByScriptEngines.sessionid
					from [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] as RowsBySession
						inner join [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] as RowsByScriptEngines
							on RowsBySession.servergroup = RowsByScriptEngines.servergroup
							and RowsBySession.server = RowsByScriptEngines.server
							and RowsBySession.scriptengine_sessionid = RowsByScriptEngines.scriptengine_sessionid
							and RowsBySession.scriptengine_id = RowsByScriptEngines.scriptengine_id
					where RowsBySession.sessionid = @CurUserStatus_InternalSessionID
						and RowsByScriptEngines.Inactive = 0
					)
				DELETE 
				FROM System_SessionValues
				WHERE SessionID NOT IN 
					-- list of valid SessionIDs
					(
					SELECT SessionID 
					FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
					WHERE Inactive = 0
					)
			END
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, LoginName, @CurUserStatus_InternalSessionID FROM dbo.Benutzer WHERE LoginName = @Username
		SET NOCOUNT ON
	END
Else -- @MyResult = 0
	-- Login failed
	BEGIN
		IF @CurUserLoginFailures = @MaxLoginFailures - 1
			-- LoginFailure Maximum nun erreicht
			BEGIN	
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -2	
				SET NOCOUNT ON
				-- Zeitliche Loginsperre setzen
				update dbo.Benutzer set LoginLockedTill = dateadd(hour, @LoginFailureDelayHours, getdate()) where LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -95, 'Login disabled for ' + cast(@LoginFailureDelayHours as nvarchar(5)) + ' hours')
			END	
		Else
			BEGIN
				SET NOCOUNT OFF
				If @MyResult = 0 -- Weitere Logins möglich - Rückgabewert
					SELECT Result = 0
				Else
					SELECT Result = -5
				SET NOCOUNT ON
			END
		-- Wert LoginFailures erhöhen 
		update dbo.Benutzer set LoginFailures = @CurUserLoginFailures + 1 where LoginName = @Username

		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -26, 'No valid login data')
	END
GO


---------------------------------------
-- Update SP "Redirects_LogAndGetURL" 
---------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Redirects_LogAndGetURL]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Redirects_LogAndGetURL]
GO


CREATE PROCEDURE dbo.Redirects_LogAndGetURL
	(
		@IDRedirector int
	)
WITH ENCRYPTION
AS 

SET NOCOUNT ON

-- Log
INSERT INTO dbo.Redirects_Log
                      (ID_Redirector)
VALUES     (@IDRedirector)


-- Increase the ticker 
UPDATE dbo.Redirects_ToAddr
SET NumberOfRedirections = NumberOfRedirections + 1
WHERE ID = @IDRedirector


SET NOCOUNT OFF

-- Get URL
SELECT RedirectTo
FROM dbo.Redirects_ToAddr
WHERE ID = @IDRedirector

GO
-----------------------------------------------------------
--Insert data into security objects and authorizations: Redirection Admin Area
-----------------------------------------------------------
DECLARE @ModifiedBy int
SELECT @ModifiedBy = -43
DECLARE @AppID_Redirections int, @AdminServerID int
SELECT @AppID_Redirections = ID FROM dbo.Applications_CurrentAndInactiveOnes WHERE Title = 'System - Administration - Redirections'
IF @AppID_Redirections IS NULL
BEGIN
	-- Retrieve one server ID
	SELECT TOP 1 @AdminServerID = System_Servers.ID
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- 1st security object
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@AdminServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
	SELECT @AppID_Redirections = SCOPE_IDENTITY()
	-- Rest of new security objects - same languages (English)
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,System_Servers.ID,1,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	WHERE System_Servers.ID <> @AdminServerID
	GROUP BY System_Servers.ID
	-- Rest of new security objects - other languages
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Weiterleitungen',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,System_Servers.ID,2,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- Authorizations
	INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Redirections ,6,getdate(),@ModifiedBy)
END
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[RefillSplittedSecObjAndNavPointsTables]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RefillSplittedSecObjAndNavPointsTables]
GO
CREATE PROCEDURE dbo.RefillSplittedSecObjAndNavPointsTables
AS
BEGIN
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

TRUNCATE TABLE dbo.SecurityObjects_CurrentAndInactiveOnes
TRUNCATE TABLE dbo.NavItems
TRUNCATE TABLE dbo.SecurityObjects_vs_NavItems
TRUNCATE TABLE dbo.SecurityObjects_AuthsByGroup
TRUNCATE TABLE dbo.SecurityObjects_AuthsByUser
TRUNCATE TABLE dbo.Apps2SecObj_SyncWarnLog

-- AppSecurityObjects übernehmen
INSERT INTO dbo.SecurityObjects_CurrentAndInactiveOnes (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Remarks, SystemAppType, Deleted, AuthsAsSecObjID, [Disabled], ModifiedBy, ModifiedOn)
SELECT     Title, MAX(TitleAdminArea) AS Expr1, MAX(ReleasedOn) AS Expr2, MIN(ReleasedBy) AS Expr3, MAX(Remarks) AS Expr4, MAX(SystemAppType) 
                      AS Expr5, MIN(Case when AppDeleted <> 0 then 1 else 0 end) AS Expr7, NULL AS Expr8, MIN(case when AppDisabled <> 0 then 1 else 0 end) AS Expr9, MIN(ModifiedBy) AS Expr11, 
                      IsNull(MAX(ModifiedOn), MAX(ReleasedOn)) AS Expr10
FROM         dbo.Applications
GROUP BY Title

-- AppNavItems übernehmen
INSERT INTO dbo.NavItems
                      (Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, ServerID, 
                      LanguageID, SystemAppType, Remarks, ReleasedOn, ReleasedBy, ModifiedOn, ModifiedBy, Disabled, Sort, ResetIsNewUpdatedStatusOn,
                       OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, 
                      Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded)
SELECT     Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, LocationID, 
                      LanguageID, SystemAppType, Remarks, ReleasedOn, ReleasedBy, IsNull(ModifiedOn, ReleasedOn), ModifiedBy, AppDisabled, Sort, 
                      ResetIsNewUpdatedStatusOn, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded
FROM         dbo.Applications
WHERE     (AppDeleted = 0)

-- Verknüpfungen zwischen SecurityObjects und NavItems aufbauen
INSERT INTO dbo.SecurityObjects_vs_NavItems (ID_SecurityObject, ID_NavItem)
SELECT dbo.SecurityObjects_CurrentAndInactiveOnes.ID as SecID, dbo.NavItems.ID as NavID
FROM dbo.Applications
	LEFT JOIN dbo.SecurityObjects_CurrentAndInactiveOnes ON dbo.Applications.Title = dbo.SecurityObjects_CurrentAndInactiveOnes.Title
	LEFT JOIN dbo.NavItems ON
		ISNULL(dbo.Applications.Level1Title, '') = ISNULL(dbo.NavItems.Level1Title, '') AND 
		ISNULL(dbo.Applications.Level2Title, '') = ISNULL(dbo.NavItems.Level2Title, '') AND 
		ISNULL(dbo.Applications.Level3Title, '') = ISNULL(dbo.NavItems.Level3Title, '') AND 
		ISNULL(dbo.Applications.Level4Title, '') = ISNULL(dbo.NavItems.Level4Title, '') AND 
		ISNULL(dbo.Applications.Level5Title, '') = ISNULL(dbo.NavItems.Level5Title, '') AND 
		ISNULL(dbo.Applications.Level6Title, '') = ISNULL(dbo.NavItems.Level6Title, '') AND 
		dbo.Applications.NavURL = dbo.NavItems.NavURL AND
		dbo.Applications.LanguageID = dbo.NavItems.LanguageID AND
		dbo.Applications.LocationID = dbo.NavItems.ServerID
WHERE dbo.NavItems.ID Is Not NULL
GROUP BY dbo.SecurityObjects_CurrentAndInactiveOnes.ID, dbo.NavItems.ID

-- Zusammenstellung der Applikationsvererbungen, welche über die Title-Grenzen hinweg gehen
CREATE TABLE #AuthInheritionsBetweenDifferentApps
(
        InheritingID int,
        InheritingTitle varchar(255),
        InheritedFromID int,
        InheritedFromTitle varchar(255)
)
;
INSERT INTO #AuthInheritionsBetweenDifferentApps
(
        InheritingID,
        InheritingTitle,
        InheritedFromID,
        InheritedFromTitle
)
SELECT     dbo.Applications.ID AS InheritingID, dbo.Applications.Title AS InheritingTitle, 
                      ApplicationsInheritFrom.ID AS InheritedFromID, ApplicationsInheritFrom.Title AS InheritedFromTitle
FROM         dbo.Applications INNER JOIN
                      dbo.Applications ApplicationsInheritFrom ON 
                      dbo.Applications.AuthsAsAppID = ApplicationsInheritFrom.ID AND 
                      dbo.Applications.Title <> ApplicationsInheritFrom.Title

-- Berechtigungsvererbung zwischen Anwendungen unterschiedlicher ApplicationTitles wiederherstellen
UPDATE dbo.SecurityObjects
SET dbo.SecurityObjects.AuthsAsSecObjID = secobj_by_authsasSecObjid.ID
FROM dbo.SecurityObjects 
	INNER JOIN dbo.Applications ON dbo.SecurityObjects.Title = dbo.Applications.Title
	INNER JOIN dbo.Applications apps_by_authsasappid ON Applications.AuthsAsAppID = apps_by_authsasappid.ID
	INNER JOIN dbo.SecurityObjects secobj_by_authsasSecObjid ON apps_by_authsasappid.Title = secobj_by_authsasSecObjid.Title
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Gruppenberechtigungen neu aufbauen - Apps ohne Vererbung
INSERT INTO SecurityObjects_AuthsByGroup (ID_SecurityObject, ID_Group, ReleasedOn, ReleasedBy, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByGroup.ID_GroupOrPerson, dbo.ApplicationsRightsByGroup.ReleasedOn, dbo.ApplicationsRightsByGroup.ReleasedBy,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Null

-- Gruppenberechtigungen neu aufbauen - Apps mit Vererbung - Gleicher AppTitle
INSERT INTO SecurityObjects_AuthsByGroup (ID_SecurityObject, ID_Group, ReleasedOn, ReleasedBy, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByGroup.ID_GroupOrPerson, dbo.ApplicationsRightsByGroup.ReleasedOn, dbo.ApplicationsRightsByGroup.ReleasedBy,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID Not IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Gruppenberechtigungen neu aufbauen - Apps mit Vererbung - Unterschiedlicher AppTitle
INSERT INTO SecurityObjects_AuthsByGroup (ID_SecurityObject, ID_Group, ReleasedOn, ReleasedBy, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByGroup.ID_GroupOrPerson, dbo.ApplicationsRightsByGroup.ReleasedOn, dbo.ApplicationsRightsByGroup.ReleasedBy,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.Applications apps_by_authsasappid ON Applications.AuthsAsAppID = apps_by_authsasappid.ID
	INNER JOIN dbo.SecurityObjects ON apps_by_authsasappid.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Benutzerberechtigungen neu aufbauen - Apps ohne Vererbung
INSERT INTO SecurityObjects_AuthsByUser (ID_SecurityObject, ID_User, ReleasedOn, ReleasedBy, DevelopmentTeamMember, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByUser.ID_GroupOrPerson, 
	dbo.ApplicationsRightsByUser.ReleasedOn, 
	dbo.ApplicationsRightsByUser.ReleasedBy, 
	dbo.ApplicationsRightsByUser.DevelopmentTeamMember,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByUser ON dbo.Applications.ID = dbo.ApplicationsRightsByUser.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Null

-- Benutzerberechtigungen neu aufbauen - Apps mit Vererbung - Gleicher AppTitle
INSERT INTO SecurityObjects_AuthsByUser (ID_SecurityObject, ID_User, ReleasedOn, ReleasedBy, DevelopmentTeamMember, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByUser.ID_GroupOrPerson, dbo.ApplicationsRightsByUser.ReleasedOn, dbo.ApplicationsRightsByUser.ReleasedBy, dbo.ApplicationsRightsByUser.DevelopmentTeamMember,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByUser ON dbo.Applications.ID = dbo.ApplicationsRightsByUser.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID Not IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Benutzerberechtigungen neu aufbauen - Apps mit Vererbung - Unterschiedlicher AppTitle
INSERT INTO SecurityObjects_AuthsByUser (ID_SecurityObject, ID_User, ReleasedOn, ReleasedBy, DevelopmentTeamMember, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByUser.ID_GroupOrPerson, dbo.ApplicationsRightsByUser.ReleasedOn, dbo.ApplicationsRightsByUser.ReleasedBy, dbo.ApplicationsRightsByUser.DevelopmentTeamMember,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.Applications apps_by_authsasappid ON Applications.AuthsAsAppID = apps_by_authsasappid.ID
	INNER JOIN dbo.SecurityObjects ON apps_by_authsasappid.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByUser ON dbo.Applications.ID = dbo.ApplicationsRightsByUser.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

/*
-- Warnungen protokollieren bei Auftreten von mehreren gleichen AppTitles mit unterschiedlichen Vererbungsinformationen
INSERT INTO dbo.Apps2SecObj_SyncWarnLog (ConflictDescription)
SELECT N'Authorizations of application "' + InheritingTitle + '" (ID ' + Cast(InheritingID as nvarchar(30)) + ') have been transferred to application "' + InheritedFromTitle + '" (ID ' + Cast(InheritedFromID as nvarchar(30)) + ') because application "' + InheritingTitle + '" (ID ' + Cast(InheritingID as nvarchar(30)) + ') inherits all authorizations from application "' + InheritedFromTitle + '" (ID ' + Cast(InheritedFromID as nvarchar(30)) + '). Please check the current inheritions between these two applications.'
FROM #AuthInheritionsBetweenDifferentApps

INSERT INTO dbo.Apps2SecObj_SyncWarnLog (ConflictDescription)
SELECT N'The old application ID ' + cast(dbo.Applications.ID as nvarchar(20)) + ' and title "' + dbo.Applications.Title + '" had set up different applications to inherit from. One of these is application ID ' + cast(isnull(apps_auths_as_appid.ID,0) as nvarchar(20)) + ' and title "' + IsNull(apps_auths_as_appid.Title, '(no application)') + '". Please check the current inheritions between these two applications.'
FROM #AuthInheritionsBetweenDifferentApps 
	INNER JOIN dbo.Applications ON #AuthInheritionsBetweenDifferentApps.InheritingTitle = dbo.Applications.Title
	LEFT JOIN dbo.Applications apps_auths_as_appid ON dbo.Applications.AuthsAsAppID = apps_auths_as_appid.ID

INSERT INTO dbo.Apps2SecObj_SyncWarnLog (ConflictDescription)
VALUES ('Authorizations of different applications with the same application title have been collected and summarized into one application security object.')
*/

-- Aufräumarbeiten
DROP TABLE #AuthInheritionsBetweenDifferentApps

END
GO
EXEC RefillSplittedSecObjAndNavPointsTables

GO
IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationRights_CumulatedPerUserAndServerGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.ApplicationRights_CumulatedPerUserAndServerGroup
GO
CREATE PROCEDURE dbo.ApplicationRights_CumulatedPerUserAndServerGroup
(
	@UserID int,
	@ServerGroupID int,
	@AuthorizedAppsCursor AS CURSOR VARYING OUTPUT 
)
WITH ENCRYPTION
AS
 -- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

/*
 * @UserID NULL means: search for anonymous auths only
 */

DECLARE @PublicGroupID int, @AnonymousGroupID int
SELECT @PublicGroupID = id_group_public, @AnonymousGroupID = id_group_anonymous
FROM system_servergroups where id = @ServerGroupID
IF @AnonymousGroupID IS NULL SET @AnonymousGroupID = 58

SET @AuthorizedAppsCursor = CURSOR FOR 
	    
SELECT ID_Application 
FROM
(
    -- Direct authorizations
    SELECT     dbo.ApplicationsRightsByUser.ID_Application
    FROM dbo.ApplicationsRightsByUser
    WHERE     dbo.ApplicationsRightsByUser.ID_GroupOrPerson = @UserID
    UNION ALL
    -- Indirect authorizations caused by application inheritage
    SELECT     dbo.Applications.ID
    FROM dbo.ApplicationsRightsByUser 
    	INNER JOIN dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.AuthsAsAppID
    WHERE     dbo.ApplicationsRightsByUser.ID_GroupOrPerson = @UserID
    UNION ALL
    -- Indirect authorizations caused by memberships as well as public and anonymous user
    SELECT     dbo.ApplicationsRightsByGroup.ID_Application
    FROM dbo.ApplicationsRightsByGroup LEFT OUTER JOIN
        (
          SELECT     ID_Group, ID_User
          FROM         dbo.Memberships
          WHERE ID_User = @UserID
          UNION ALL
          SELECT     @AnonymousGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @AnonymousGroupID IS NOT NULL
          UNION ALL
          SELECT     @PublicGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @PublicGroupID IS NOT NULL
          UNION ALL
          SELECT     @AnonymousGroupID, NULL
         ) AS Memberships_CummulatedWithAnonymousAndPublic ON 
         dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = Memberships_CummulatedWithAnonymousAndPublic.ID_Group
    WHERE CASE 
        WHEN @UserID IS NOT NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_User = @UserID 
			THEN 1 -- Normal user membership search
        WHEN @UserID IS NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_Group = @AnonymousGroupID 
			THEN 1 -- No user search but search for authorizations of ANONYMOUS group-user
        ELSE 0
        END = 1
    UNION ALL
    -- Indirect authorizations caused by memberships as well as public and anonymous user
    SELECT     dbo.Applications.ID
    FROM dbo.ApplicationsRightsByGroup 
    	INNER JOIN dbo.Applications ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications.AuthsAsAppID
        LEFT OUTER JOIN
        (
          SELECT     ID_Group, ID_User
          FROM         dbo.Memberships
          WHERE ID_User = @UserID
          UNION ALL
          SELECT     @AnonymousGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @AnonymousGroupID IS NOT NULL
          UNION ALL
          SELECT     @PublicGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @PublicGroupID IS NOT NULL
          UNION ALL
          SELECT     @AnonymousGroupID, NULL
         ) AS Memberships_CummulatedWithAnonymousAndPublic ON 
         dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = Memberships_CummulatedWithAnonymousAndPublic.ID_Group
    WHERE CASE 
        WHEN @UserID IS NOT NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_User = @UserID 
			THEN 1 -- Normal user membership search
        WHEN @UserID IS NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_Group = @AnonymousGroupID 
			THEN 1 -- No user search but search for authorizations of ANONYMOUS group-user
        ELSE 0
        END = 1
    UNION ALL
    -- Add authorizations caused by supervisor membership
    SELECT     Applications.ID AS ID_Application
    FROM         dbo.Applications CROSS JOIN
                          dbo.Memberships
    WHERE     dbo.Memberships.ID_Group = 6 AND dbo.Memberships.ID_User = @UserID
) AS BaseTable
GROUP BY ID_Application

OPEN @AuthorizedAppsCursor

GO
