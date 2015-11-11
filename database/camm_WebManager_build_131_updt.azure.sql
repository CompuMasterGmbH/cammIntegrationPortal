---------------------
-- Web-Editor WCMS --
---------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[WebManager_WebEditor]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[WebManager_WebEditor]
GO

CREATE TABLE [dbo].[WebManager_WebEditor] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[ServerID] [int] NOT NULL ,
	[LanguageID] [int] NOT NULL ,
	[IsActive] [bit] NOT NULL ,
	[URL] [nvarchar] (440) NOT NULL ,
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
		[URL]
	)   
)  TEXTIMAGE_ON [PRIMARY]
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
-- dbo.Public_UserIsAuthorizedForApp
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UserIsAuthorizedForApp
(
	@Username nvarchar(20),
	@WebApplication varchar(255),
	@ServerIP nvarchar(32)
)
WITH ENCRYPTION
AS 

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

		If NullIf(@WebApplication, '') = 'Public' And @CurUserID Is Not Null
			Return 1 -- Zugriff gewährt
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT TOP 1 @bufferUserIDByAnonymousGroup = ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID)
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByPublicGroup = ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @PublicGroupID)
		If NullIf(@bufferUserIDByPublicGroup, -1) <> -1 And @CurUserID Is Not Null
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByUser = ID_User FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_User = @CurUserID)
		If NullIf(@bufferUserIDByUser, -1) <> -1 
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByGroup = Memberships.ID_User FROM Memberships INNER JOIN view_ApplicationRights ON Memberships.ID_Group = view_ApplicationRights.ID_Group WHERE (view_ApplicationRights.Title = @WebApplication) AND (view_ApplicationRights.LocationID = @RequestedServerID) AND (Memberships.ID_User = @CurUserID)
		If NullIf(@bufferUserIDByGroup, -1) <> -1
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByAdmin = ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6)
		If NullIf(@bufferUserIDByAdmin, -1) <> -1 
			Return 1 -- Zugriff gewährt
		Else
			Return 0 -- kein Zugriff auf aktuelles Dokument

GO
