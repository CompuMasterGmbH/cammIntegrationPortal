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
SELECT @AppID_Applications = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations','',getdate(),@ModifiedBy,'Web Administration','User Administration','Authorizations',NULL,NULL,NULL,'[ADMINURL]apprights.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Authorizations = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups','',getdate(),@ModifiedBy,'Web Administration','User Administration','Groups',NULL,NULL,NULL,'[ADMINURL]groups.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Groups = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships','',getdate(),@ModifiedBy,'Web Administration','User Administration','Group memberships',NULL,NULL,NULL,'[ADMINURL]memberships.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Memberships = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users','',getdate(),@ModifiedBy,'Web Administration','User Administration','Users',NULL,NULL,NULL,'[ADMINURL]users.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Users = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','Reset user password',NULL,NULL,NULL,'[ADMINURL]users_resetpw.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_ResetPassword = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview','',getdate(),@ModifiedBy,'Web Administration','Navigation preview',NULL,NULL,NULL,NULL,'[ADMINURL]users_navbar_preview.aspx','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_NavPreview = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','User hotline support',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.aspx','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Trouble= @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Server administration',NULL,NULL,NULL,'[ADMINURL]servers.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_ServerSetup = @@IDENTITY
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','About Web-Manager',NULL,NULL,NULL,'[ADMINURL]about.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Access levels',NULL,NULL,NULL,'[ADMINURL]accesslevels.aspx',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_AccessLevels = @@IDENTITY

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
SELECT @AppID_Logs = @@IDENTITY
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
SELECT @AppID_Redirections = @@IDENTITY
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
SELECT @AppID_Markets = @@IDENTITY
-- Authorizations
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Markets ,6,getdate(),@ModifiedBy)

-- Mail queue monitor - English: not a system app to allow modification of authorizations
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Mail Queue Monitor','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','Mail queue monitor',NULL,NULL,NULL,'[ADMINURL]mailqueue_monitor.aspx','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,3)
SELECT @AppID_QueueMonitor = @@IDENTITY
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
SELECT @AppID_TextModules = @@IDENTITY
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

---------------------------------------------------------------------
-- Refresh all navigation items for all administration servers
---------------------------------------------------------------------

DECLARE @AdminServer int
DECLARE AdminServersCursor CURSOR FOR
	SELECT UserAdminServer 
	FROM System_ServerGroups
	GROUP BY UserAdminServer

OPEN AdminServersCursor

-- Perform the first fetch and store the values in variables.
-- Note: The variables are in the same order as the columns
-- in the SELECT statement. 

FETCH NEXT FROM AdminServersCursor
INTO @AdminServer

-- Check @@FETCH_STATUS to see if there are any more rows to fetch.
WHILE @@FETCH_STATUS = 0
BEGIN

   -- Concatenate and display the current values in the variables.
   EXEC dbo.AdminPrivate_CreateAdminServerNavPoints @AdminServer, 0, -43, 1

   -- This is executed as long as the previous fetch succeeds.
   FETCH NEXT FROM AdminServersCursor
   INTO @AdminServer
END

CLOSE AdminServersCursor
DEALLOCATE AdminServersCursor
GO
