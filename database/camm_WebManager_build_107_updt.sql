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
-- dbo.AdminPrivate_CloneApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CloneApplication 
(
	@ReleasedByUserID int,
	@AppID int,
	@CloneType int
)

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
				                      LocationID, LanguageID, SystemApp, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, @ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, SystemApp, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL
				FROM         dbo.Applications
				WHERE     (ID = @AppID)

				-- Add Group Authorizations
				INSERT INTO dbo.ApplicationsRightsByGroup
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application)
				SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @AppID AS ID_Application
				FROM         dbo.ApplicationsRightsByGroup
				WHERE     (ID_Application = @AppID)

				-- Add User Authorizations
				INSERT INTO dbo.ApplicationsRightsByUser
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application)
				
SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @AppID AS ID_Application
				FROM         dbo.ApplicationsRightsByUser
				WHERE     (ID_Application = @AppID)

			END
		Else -- copy application and inherit authorizations from cloned application
			BEGIN
				INSERT INTO dbo.Applications
				                      (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, 
				                      LocationID, LanguageID, SystemApp, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, 
						@ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, SystemApp, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, @AppID As AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL
				FROM         dbo.Applications
				WHERE    (ID = @AppID)
			END

		SELECT @NewAppID = @@IDENTITY

		INSERT INTO [dbo].[System_SubSecurityAdjustments]([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
		SELECT [UserID], [TableName], @NewAppID, [AuthorizationType]
		FROM [dbo].[System_SubSecurityAdjustments]
		WHERE TableName = 'Applications' AND TablePrimaryIDValue = @AppID

		SET NOCOUNT OFF
	
		SELECT Result = @NewAppID
		
	END
Else
	
	SELECT Result = 0

GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServerGroup
(
@ID_ServerGroup int
)

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

-- DELETE the server group itself
DELETE 
FROM System_ServerGroups
WHERE System_ServerGroups.ID = @ID_ServerGroup


SET NOCOUNT OFF

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
SELECT @ID_Group_Public = @@IDENTITY

-- Public Group Security Adjustments
INSERT INTO [dbo].[System_SubSecurityAdjustments] (UserID, TableName, TablePrimaryIDValue, AuthorizationType)
VALUES (@UserID_Creator, 'Groups', @ID_Group_Public, 'Owner')

-- Neuen Server anlegen, welcher als MasterServer fungieren soll
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, 'secured.yourcompany.com' AS IP, 'Secured server' AS ServerDescription, 0 AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout
SELECT @ID_MasterServer = @@IDENTITY

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
SELECT @ID_ServerGroup = @@IDENTITY

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
-- dbo.AdminPrivate_CreateMemberships
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMemberships 
(
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43 
	SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
Else
	SELECT @CurUserID = @ReleasedByUserID
SELECT @MemberShipID = ID FROM dbo.Memberships WHERE ID_Group = @GroupID And ID_User = @UserID
If @MemberShipID Is Null
	BEGIN
		-- Password validation and update
		If @CurUserID Is Not Null
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Record update
				INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedBy) VALUES (@GroupID, @UserID, @ReleasedByUserID)
			END
		Else
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	SELECT Result = -1


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

AS

-- Removed functionality
-- Now in WebManager DLL

GO

---------------------------------------------------------
-- Remove any existing nav items of any master servers --
---------------------------------------------------------
DELETE dbo.ApplicationsRightsByGroup
FROM dbo.ApplicationsRightsByGroup INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
WHERE SystemApp <> 0 And SystemAppType = 1 

DELETE
FROM dbo.Applications_CurrentAndInactiveOnes 
WHERE SystemApp <> 0 And SystemAppType = 1 

GO
----------------------------------------------------
-- dbo.Public_ValidateUser
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateUser
(
	@Username nvarchar(20),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@ForceLogin bit
)

AS

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

-- Konstanten setzen
SET @MaxLoginFailures = 7

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
		-- LoginRemoteIP 
		update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), CurrentLoginViaRemoteIP = @RemoteIP where LoginName = @Username
		-- LoginCount hochzählen
		update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1 where LoginName = @Username
		-- LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 98, 'Validation of login data successfull')
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = @@IDENTITY
		UPDATE dbo.Benutzer SET System_SessionID = @CurUserStatus_InternalSessionID WHERE LoginName = @UserName
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		INSERT INTO dbo.System_WebAreasAuthorizedForSession
		                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
		SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
		                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
		FROM         dbo.System_Servers INNER JOIN
		                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
		WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @LocationID)
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, LoginName, System_SessionID FROM dbo.Benutzer WHERE LoginName = @Username
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
----------------------------------------------------
-- dbo.Public_ValidateDocument
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateDocument
	@Username nvarchar(20),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication nvarchar(1024),
	@WebURL nvarchar(1024),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@Reserved int = Null

AS

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
FROM dbo.Benutzer 
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
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID)))
If @WebAppID Is Null And @WebApplication Not Like 'Public'
	BEGIN
		SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
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
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1
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
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1 Or NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByUser, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
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


---------------------------------------------------------
-- Update any existing nav points of all admin servers --
-- to refer to the .aspx files instead of .asp         --
---------------------------------------------------------
UPDATE dbo.Applications_CurrentAndInactiveOnes 
SET NavURL = '[ADMINURL]users_navbar_preview.aspx'
WHERE SystemApp <> 0 And SystemAppType = 2 
	AND NavURL = '[MASTERSERVER]/sysdata/users_navbar_preview.asp'

UPDATE dbo.Applications_CurrentAndInactiveOnes 
SET NavURL = NavURL + 'x'
WHERE SystemApp <> 0 And SystemAppType = 2 
	AND NavURL like '%.asp'

GO


----------------------------------------------------
-- dbo.System_Languages
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) 
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) 
drop table [dbo].[System_Languages]
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) 
CREATE TABLE [dbo].[System_Languages]
(
[ID]                                INT IDENTITY(1,1) NOT NULL,
[Abbreviation]                      NVARCHAR(10),
[Description_OwnLang]               NVARCHAR(255) NOT NULL,
[Description_English]               NVARCHAR(255) NOT NULL,
[Description_German]                NVARCHAR(255),
[BrowserLanguageID]                 NVARCHAR(10),
[AlternativeLanguage]               INT,
[IsActive]                          BIT NOT NULL DEFAULT 0,
CONSTRAINT [PK_System_Languages] PRIMARY KEY CLUSTERED ( [ID] )
);

GO

/******************************************************
  Insert data   Begin
******************************************************/

-----------------------------------------------------------
--Insert data into dbo.Languages
-----------------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) 
BEGIN
	IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages ON
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('1','eng','English','English','en',NULL,'Englisch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('2','deu','Deutsch','German','de',NULL,'Deutsch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('3','fra','Francais','French','fr',NULL,'Französisch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('4','esp','Espanol','Spanish','es',NULL,'Spanisch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('5','aar','Afar','Afar',NULL,NULL,'Afar')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('6','abk','Abkhazian','Abkhazian',NULL,NULL,'Abkhazian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('7','ace','Achinese','Achinese',NULL,NULL,'Achinese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('8','ach','Acoli','Acoli',NULL,NULL,'Acoli')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('9','ada','Adangme','Adangme',NULL,NULL,'Adangme')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('10','afa','Afro-Asiatic (Other)','Afro-Asiatic (Other)',NULL,NULL,'Afro-Asiatic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('11','afh','Afrihili','Afrihili',NULL,NULL,'Afrihili')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('12','afr','Afrikaans','Afrikaans','af',NULL,'Afrikaans')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('13','aka','Akan','Akan',NULL,NULL,'Akan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('14','akk','Akkadian','Akkadian',NULL,NULL,'Akkadian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('16','ale','Aleut','Aleut',NULL,NULL,'Aleut')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('17','alg','Algonquian languages','Algonquian languages',NULL,NULL,'Algonquian languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('18','amh','Amharic','Amharic',NULL,NULL,'Amharic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('19','ang','English, Old (ca.450-1100)','English, Old (ca.450-1100)',NULL,NULL,'Englisch, Old (ca.450-1100)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('20','apa','Apache languages','Apache languages',NULL,NULL,'Apache languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('21','ara','Arabic','Arabic','ar',NULL,'Arabic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('22','arc','Aramaic','Aramaic',NULL,NULL,'Aramaic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('23','arm','Armenian','Armenian','hy',NULL,'Armenian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('24','arn','Araucanian','Araucanian',NULL,NULL,'Araucanian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('25','arp','Arapaho','Arapaho',NULL,NULL,'Arapaho')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('26','art','Artificial (Other)','Artificial (Other)',NULL,NULL,'Artificial (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('27','arw','Arawak','Arawak',NULL,NULL,'Arawak')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('28','asm','Assamese','Assamese','as',NULL,'Assamese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('29','ast','Asturian; Bable','Asturian; Bable',NULL,NULL,'Asturian; Bable')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('30','ath','Athapascan languages','Athapascan languages',NULL,NULL,'Athapascan languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('31','aus','Australian languages','Australian languages',NULL,NULL,'Australian languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('32','ava','Avaric','Avaric',NULL,NULL,'Avaric')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('33','ave','Avestan','Avestan',NULL,NULL,'Avestan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('34','awa','Awadhi','Awadhi',NULL,NULL,'Awadhi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('35','aym','Aymara','Aymara',NULL,NULL,'Aymara')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('36','aze','Azerbaijani','Azerbaijani','az',NULL,'Azerbaijani')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('37','bad','Banda','Banda',NULL,NULL,'Banda')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('38','bai','Bamileke languages','Bamileke languages',NULL,NULL,'Bamileke languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('39','bak','Bashkir','Bashkir',NULL,NULL,'Bashkir')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('40','bal','Baluchi','Baluchi',NULL,NULL,'Baluchi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('41','bam','Bambara','Bambara',NULL,NULL,'Bambara')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('42','ban','Balinese','Balinese',NULL,NULL,'Balinese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('43','baq','Basque','Basque','eu',NULL,'Basque')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('44','bas','Basa','Basa',NULL,NULL,'Basa')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('45','bat','Baltic (Other)','Baltic (Other)',NULL,NULL,'Baltic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('46','bej','Beja','Beja',NULL,NULL,'Beja')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('47','bel','Belarusian','Belarusian','be',NULL,'Belarusian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('48','bem','Bemba','Bemba',NULL,NULL,'Bemba')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('49','ben','Bengali','Bengali','bn',NULL,'Bengali')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('50','ber','Berber (Other)','Berber (Other)',NULL,NULL,'Berber (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('51','bho','Bhojpuri','Bhojpuri',NULL,NULL,'Bhojpuri')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('52','bih','Bihari','Bihari',NULL,NULL,'Bihari')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('53','bik','Bikol','Bikol',NULL,NULL,'Bikol')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('54','bin','Bini','Bini',NULL,NULL,'Bini')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('55','bis','Bislama','Bislama',NULL,NULL,'Bislama')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('56','bla','Siksika','Siksika',NULL,NULL,'Siksika')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('57','bnt','Bantu (Other)','Bantu (Other)',NULL,NULL,'Bantu (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('58','tib','Tibetan','Tibetan',NULL,NULL,'Tibetan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('59','bos','Bosnian','Bosnian',NULL,NULL,'Bosnian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('60','bra','Braj','Braj',NULL,NULL,'Braj')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('61','bre','Breton','Breton',NULL,NULL,'Breton')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('62','btk','Batak (Indonesia)','Batak (Indonesia)',NULL,NULL,'Batak (Indonesia)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('63','bua','Buriat','Buriat',NULL,NULL,'Buriat')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('64','bug','Buginese','Buginese',NULL,NULL,'Buginese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('65','bul','Bulgarian','Bulgarian','bg',NULL,'Bulgarian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('66','bur','Burmese','Burmese',NULL,NULL,'Burmese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('67','cad','Caddo','Caddo',NULL,NULL,'Caddo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('68','cai','Central American Indian (Other)','Central American Indian (Other)',NULL,NULL,'Central American Indian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('70','car','Carib','Carib',NULL,NULL,'Carib')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('71','cat','Catalan','Catalan','ca',NULL,'Catalan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('72','cau','Caucasian (Other)','Caucasian (Other)',NULL,NULL,'Caucasian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('73','ceb','Cebuano','Cebuano',NULL,NULL,'Cebuano')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('74','cel','Celtic (Other)','Celtic (Other)',NULL,NULL,'Celtic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('75','cze','Czech','Czech','cs',NULL,'Czech')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('76','cha','Chamorro','Chamorro',NULL,NULL,'Chamorro')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('77','chb','Chibcha','Chibcha',NULL,NULL,'Chibcha')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('78','che','Chechen','Chechen',NULL,NULL,'Chechen')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('79','chg','Chagatai','Chagatai',NULL,NULL,'Chagatai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('80','chi','Chinese','Chinese','zh',NULL,'Chinese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('81','chk','Chuukese','Chuukese',NULL,NULL,'Chuukese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('82','chm','Mari','Mari',NULL,NULL,'Mari')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('83','chn','Chinook jargon','Chinook jargon',NULL,NULL,'Chinook jargon')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('84','cho','Choctaw','Choctaw',NULL,NULL,'Choctaw')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('85','chp','Chipewyan','Chipewyan',NULL,NULL,'Chipewyan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('86','chr','Cherokee','Cherokee',NULL,NULL,'Cherokee')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('88','chv','Chuvash','Chuvash',NULL,NULL,'Chuvash')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('89','chy','Cheyenne','Cheyenne',NULL,NULL,'Cheyenne')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('90','cmc','Chamic languages','Chamic languages',NULL,NULL,'Chamic languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('91','cop','Coptic','Coptic',NULL,NULL,'Coptic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('92','cor','Cornish','Cornish',NULL,NULL,'Cornish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('93','cos','Corsican','Corsican',NULL,NULL,'Corsican')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('94','cpe','Creoles and pidgins, English based (Other)','Creoles and pidgins, English based (Other)',NULL,NULL,'Creoles and pidgins, English based (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('95','cpf','Creoles and pidgins, French-based (Other)','Creoles and pidgins, French-based (Other)',NULL,NULL,'Creoles and pidgins, French-based (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('96','cpp','Creoles and pidgins,','Creoles and pidgins,',NULL,NULL,'Creoles and pidgins,')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('98','cre','Cree','Cree',NULL,NULL,'Cree')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('99','crp','Creoles and pidgins (Other)','Creoles and pidgins (Other)',NULL,NULL,'Creoles and pidgins (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('100','cus','Cushitic (Other)','Cushitic (Other)',NULL,NULL,'Cushitic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('101','wel','Welsh','Welsh',NULL,NULL,'Welsh')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('103','dak','Dakota','Dakota',NULL,NULL,'Dakota')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('104','dan','Danish','Danish','da',NULL,'Danish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('105','day','Dayak','Dayak',NULL,NULL,'Dayak')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('106','del','Delaware','Delaware',NULL,NULL,'Delaware')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('107','den','Slave (Athapascan)','Slave (Athapascan)',NULL,NULL,'Slave (Athapascan)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('109','dgr','Dogrib','Dogrib',NULL,NULL,'Dogrib')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('110','din','Dinka','Dinka',NULL,NULL,'Dinka')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('111','div','Divehi','Divehi',NULL,NULL,'Divehi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('112','doi','Dogri','Dogri',NULL,NULL,'Dogri')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('113','dra','Dravidian (Other)','Dravidian (Other)',NULL,NULL,'Dravidian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('114','dua','Duala','Duala',NULL,NULL,'Duala')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('115','dum','Dutch, Middle (ca.1050-1350)','Dutch, Middle (ca.1050-1350)',NULL,NULL,'Dutch, Middle (ca.1050-1350)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('117','dyu','Dyula','Dyula',NULL,NULL,'Dyula')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('118','dzo','Dzongkha','Dzongkha',NULL,NULL,'Dzongkha')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('119','efi','Efik','Efik',NULL,NULL,'Efik')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('120','egy','Egyptian (Ancient)','Egyptian (Ancient)',NULL,NULL,'Egyptian (Ancient)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('121','eka','Ekajuk','Ekajuk',NULL,NULL,'Ekajuk')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('123','elx','Elamite','Elamite',NULL,NULL,'Elamite')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('125','enm','English, Middle (1100-1500)','English, Middle (1100-1500)',NULL,NULL,'Englisch, Middle (1100-1500)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('126','epo','Esperanto','Esperanto',NULL,NULL,'Esperanto')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('127','est','Estonian','Estonian','et',NULL,'Estonian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('129','ewe','Ewe','Ewe',NULL,NULL,'Ewe')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('130','ewo','Ewondo','Ewondo',NULL,NULL,'Ewondo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('131','fan','Fang','Fang',NULL,NULL,'Fang')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('132','fao','Faroese','Faroese',NULL,NULL,'Faroese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('134','fat','Fanti','Fanti',NULL,NULL,'Fanti')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('135','fij','Fijian','Fijian',NULL,NULL,'Fijian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('136','fin','Finnish','Finnish','fi',NULL,'Finnish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('137','fiu','Finno-Ugrian (Other)','Finno-Ugrian (Other)',NULL,NULL,'Finno-Ugrian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('138','fon','Fon','Fon',NULL,NULL,'Fon')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('140','frm','French, Middle (ca.1400-1800)','French, Middle (ca.1400-1800)',NULL,NULL,'Französisch, Middle (ca.1400-1800)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('141','fro','French, Old (842-ca.1400)','French, Old (842-ca.1400)',NULL,NULL,'Französisch, Old (842-ca.1400)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('142','fry','Frisian','Frisian',NULL,NULL,'Frisian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('143','ful','Fulah','Fulah',NULL,NULL,'Fulah')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('144','fur','Friulian','Friulian',NULL,NULL,'Friulian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('145','gaa','Ga','Ga',NULL,NULL,'Ga')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('146','gay','Gayo','Gayo',NULL,NULL,'Gayo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('147','gba','Gbaya','Gbaya',NULL,NULL,'Gbaya')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('148','gem','Germanic (Other)','Germanic (Other)',NULL,NULL,'Germanisch (Sonstige)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('151','gez','Geez','Geez',NULL,NULL,'Geez')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('152','gil','Gilbertese','Gilbertese',NULL,NULL,'Gilbertese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('153','gla','Gaelic; Scottish Gaelic','Gaelic; Scottish Gaelic','gd',NULL,'Gaelic; Scottish Gaelic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('154','gle','Irish','Irish',NULL,NULL,'Irish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('155','glg','Gallegan','Gallegan',NULL,NULL,'Gallegan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('156','glv','Manx','Manx',NULL,NULL,'Manx')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('157','gmh','German, Middle High (ca.1050-1500)','German, Middle High (ca.1050-1500)',NULL,NULL,'Deutsch, Middle High (ca.1050-1500)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('158','goh','German, Old High (ca.750-1050)','German, Old High (ca.750-1050)',NULL,NULL,'Deutsch, Old High (ca.750-1050)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('159','gon','Gondi','Gondi',NULL,NULL,'Gondi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('160','gor','Gorontalo','Gorontalo',NULL,NULL,'Gorontalo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('161','got','Gothic','Gothic',NULL,NULL,'Gothic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('162','grb','Grebo','Grebo',NULL,NULL,'Grebo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('163','grc','Greek, Ancient (to 1453)','Greek, Ancient (to 1453)',NULL,NULL,'Greek, Ancient (to 1453)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('164','gre','Greek, Modern (1453-)','Greek, Modern (1453-)','el',NULL,'Greek, Modern (1453-)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('165','grn','Guarani','Guarani',NULL,NULL,'Guarani')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('166','guj','Gujarati','Gujarati','gu',NULL,'Gujarati')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('167','gwi','Gwich´in','Gwich´in',NULL,NULL,'Gwich´in')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('168','hai','Haida','Haida',NULL,NULL,'Haida')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('169','hau','Hausa','Hausa',NULL,NULL,'Hausa')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('170','haw','Hawaiian','Hawaiian',NULL,NULL,'Hawaiian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('171','heb','Hebrew','Hebrew','he',NULL,'Hebrew')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('172','her','Herero','Herero',NULL,NULL,'Herero')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('173','hil','Hiligaynon','Hiligaynon',NULL,NULL,'Hiligaynon')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('174','him','Himachali','Himachali',NULL,NULL,'Himachali')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('175','hin','Hindi','Hindi','hi',NULL,'Hindi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('176','hit','Hittite','Hittite',NULL,NULL,'Hittite')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('177','hmn','Hmong','Hmong',NULL,NULL,'Hmong')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('178','hmo','Hiri Motu','Hiri Motu',NULL,NULL,'Hiri Motu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('179','scr','Croatian','Croatian','hr',NULL,'Croatian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('180','hun','Hungarian','Hungarian','hu',NULL,'Hungarian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('181','hup','Hupa','Hupa',NULL,NULL,'Hupa')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('183','iba','Iban','Iban',NULL,NULL,'Iban')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('184','ibo','Igbo','Igbo',NULL,NULL,'Igbo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('185','ice','Icelandic','Icelandic','is',NULL,'Icelandic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('186','ido','Ido','Ido',NULL,NULL,'Ido')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('187','ijo','Ijo','Ijo',NULL,NULL,'Ijo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('188','iku','Inuktitut','Inuktitut',NULL,NULL,'Inuktitut')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('189','ile','Interlingue','Interlingue',NULL,NULL,'Interlingue')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('190','ilo','Iloko','Iloko',NULL,NULL,'Iloko')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('191','ina','Interlingua (International Auxiliary','Interlingua (International Auxiliary',NULL,NULL,'Interlingua (International Auxiliary')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('193','inc','Indic (Other)','Indic (Other)',NULL,NULL,'Indic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('194','ind','Indonesian','Indonesian','id',NULL,'Indonesian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('195','ine','Indo-European (Other)','Indo-European (Other)',NULL,NULL,'Indo-European (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('196','ipk','Inupiaq','Inupiaq',NULL,NULL,'Inupiaq')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('197','ira','Iranian (Other)','Iranian (Other)',NULL,NULL,'Iranian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('198','iro','Iroquoian languages','Iroquoian languages',NULL,NULL,'Iroquoian languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('200','ita','Italian','Italian','it',NULL,'Italian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('201','jav','Javanese','Javanese',NULL,NULL,'Javanese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('202','jpn','Japanese','Japanese','ja',NULL,'Japanese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('203','jpr','Judeo-Persian','Judeo-Persian',NULL,NULL,'Judeo-Persian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('204','jrb','Judeo-Arabic','Judeo-Arabic',NULL,NULL,'Judeo-Arabic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('205','kaa','Kara-Kalpak','Kara-Kalpak',NULL,NULL,'Kara-Kalpak')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('206','kab','Kabyle','Kabyle',NULL,NULL,'Kabyle')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('207','kac','Kachin','Kachin',NULL,NULL,'Kachin')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('208','kal','Kalaallisut','Kalaallisut',NULL,NULL,'Kalaallisut')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('209','kam','Kamba','Kamba',NULL,NULL,'Kamba')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('210','kan','Kannada','Kannada','kn',NULL,'Kannada')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('211','kar','Karen','Karen',NULL,NULL,'Karen')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('212','kas','Kashmiri','Kashmiri',NULL,NULL,'Kashmiri')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('213','geo','Georgian','Georgian',NULL,NULL,'Georgian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('214','kau','Kanuri','Kanuri',NULL,NULL,'Kanuri')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('215','kaw','Kawi','Kawi',NULL,NULL,'Kawi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('216','kaz','Kazakh','Kazakh','kk',NULL,'Kazakh')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('217','kha','Khasi','Khasi',NULL,NULL,'Khasi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('218','khi','Khoisan (Other)','Khoisan (Other)',NULL,NULL,'Khoisan (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('219','khm','Khmer','Khmer',NULL,NULL,'Khmer')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('220','kho','Khotanese','Khotanese',NULL,NULL,'Khotanese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('221','kik','Kikuyu; Gikuyu','Kikuyu; Gikuyu',NULL,NULL,'Kikuyu; Gikuyu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('222','kin','Kinyarwanda','Kinyarwanda',NULL,NULL,'Kinyarwanda')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('223','kir','Kirghiz','Kirghiz','kz',NULL,'Kirghiz')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('224','kmb','Kimbundu','Kimbundu',NULL,NULL,'Kimbundu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('225','kok','Konkani','Konkani','kok',NULL,'Konkani')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('226','kom','Komi','Komi',NULL,NULL,'Komi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('227','kon','Kongo','Kongo',NULL,NULL,'Kongo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('228','kor','Korean','Korean','ko',NULL,'Korean')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('229','kos','Kosraean','Kosraean',NULL,NULL,'Kosraean')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('230','kpe','Kpelle','Kpelle',NULL,NULL,'Kpelle')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('231','kro','Kru','Kru',NULL,NULL,'Kru')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('232','kru','Kurukh','Kurukh',NULL,NULL,'Kurukh')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('233','kua','Kuanyama; Kwanyama','Kuanyama; Kwanyama',NULL,NULL,'Kuanyama; Kwanyama')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('234','kum','Kumyk','Kumyk',NULL,NULL,'Kumyk')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('235','kur','Kurdish','Kurdish',NULL,NULL,'Kurdish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('236','kut','Kutenai','Kutenai',NULL,NULL,'Kutenai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('237','lad','Ladino','Ladino',NULL,NULL,'Ladino')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('238','lah','Lahnda','Lahnda',NULL,NULL,'Lahnda')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('239','lam','Lamba','Lamba',NULL,NULL,'Lamba')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('240','lao','Lao','Lao',NULL,NULL,'Lao')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('241','lat','Latin','Latin',NULL,NULL,'Latin')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('242','lav','Latvian','Latvian','lv',NULL,'Latvian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('243','lez','Lezghian','Lezghian',NULL,NULL,'Lezghian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('244','lim','Limburgan; Limburger; Limburgish','Limburgan; Limburger; Limburgish',NULL,NULL,'Limburgan; Limburger; Limburgish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('245','lin','Lingala','Lingala',NULL,NULL,'Lingala')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('246','lit','Lithuanian','Lithuanian','lt',NULL,'Lithuanian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('247','lol','Mongo','Mongo',NULL,NULL,'Mongo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('248','loz','Lozi','Lozi',NULL,NULL,'Lozi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('249','ltz','Letzeburgesch','Luxembourgish',NULL,NULL,'Luxembourgish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('250','lua','Luba-Lulua','Luba-Lulua',NULL,NULL,'Luba-Lulua')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('251','lub','Luba-Katanga','Luba-Katanga',NULL,NULL,'Luba-Katanga')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('252','lug','Ganda','Ganda',NULL,NULL,'Ganda')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('253','lui','Luiseno','Luiseno',NULL,NULL,'Luiseno')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('254','lun','Lunda','Lunda',NULL,NULL,'Lunda')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('255','luo','Luo (Kenya and Tanzania)','Luo (Kenya and Tanzania)',NULL,NULL,'Luo (Kenya and Tanzania)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('256','lus','lushai','lushai',NULL,NULL,'lushai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('258','mad','Madurese','Madurese',NULL,NULL,'Madurese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('259','mag','Magahi','Magahi',NULL,NULL,'Magahi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('260','mah','Marshallese','Marshallese',NULL,NULL,'Marshallese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('261','mai','Maithili','Maithili',NULL,NULL,'Maithili')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('262','mak','Makasar','Makasar',NULL,NULL,'Makasar')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('263','mal','Malayalam','Malayalam','ml',NULL,'Malayalam')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('264','man','Mandingo','Mandingo',NULL,NULL,'Mandingo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('265','mao','Maori','Maori',NULL,NULL,'Maori')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('266','map','Austronesian (Other)','Austronesian (Other)',NULL,NULL,'Austronesian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('268','mar','Marathi','Marathi','mr',NULL,'Marathi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('269','mas','Masai','Masai',NULL,NULL,'Masai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('271','mdr','Mandar','Mandar',NULL,NULL,'Mandar')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('272','men','Mende','Mende',NULL,NULL,'Mende')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('273','mga','Irish, Middle (900-1200)','Irish, Middle (900-1200)',NULL,NULL,'Irish, Middle (900-1200)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('274','mic','Micmac','Micmac',NULL,NULL,'Micmac')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('275','min','Minangkabau','Minangkabau',NULL,NULL,'Minangkabau')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('277','mac','Macedonian','Macedonian','mk',NULL,'Macedonian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('278','mkh','Mon-Khmer (Other)','Mon-Khmer (Other)',NULL,NULL,'Mon-Khmer (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('279','mlg','Malagasy','Malagasy',NULL,NULL,'Malagasy')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('280','mlt','Maltese','Maltese','mt',NULL,'Maltese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('281','mnc','Manchu','Manchu',NULL,NULL,'Manchu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('282','mni','Manipuri','Manipuri',NULL,NULL,'Manipuri')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('283','mno','Manobo languages','Manobo languages',NULL,NULL,'Manobo languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('284','moh','Mohawk','Mohawk',NULL,NULL,'Mohawk')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('285','mol','Moldavian','Moldavian',NULL,NULL,'Moldavian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('286','mon','Mongolian','Mongolian','mn',NULL,'Mongolian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('287','mos','Mossi','Mossi',NULL,NULL,'Mossi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('289','may','Malay','Malay',NULL,NULL,'Malay')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('290','mul','Multiple languages','Multiple languages',NULL,NULL,'Multiple languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('291','mun','Munda languages','Munda languages',NULL,NULL,'Munda languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('292','mus','Creek','Creek',NULL,NULL,'Creek')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('293','mwr','Marwari','Marwari',NULL,NULL,'Marwari')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('295','myn','Mayan languages','Mayan languages',NULL,NULL,'Mayan languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('296','nah','Nahuatl','Nahuatl',NULL,NULL,'Nahuatl')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('297','nai','North American Indian','North American Indian',NULL,NULL,'North American Indian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('299','nap','Neapolitan','Neapolitan',NULL,NULL,'Neapolitan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('300','nau','Nauru','Nauru',NULL,NULL,'Nauru')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('301','nav','Navajo; Navaho','Navajo; Navaho',NULL,NULL,'Navajo; Navaho')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('302','nbl','Ndebele, South; South Ndebele','Ndebele, South; South Ndebele',NULL,NULL,'Ndebele, South; South Ndebele')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('303','nde','Ndebele, North; North Ndebele','Ndebele, North; North Ndebele',NULL,NULL,'Ndebele, North; North Ndebele')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('304','ndo','Ndonga','Ndonga',NULL,NULL,'Ndonga')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('305','nds','Low German; Low Saxon; German, Low; Saxon, Low','Low German; Low Saxon; German, Low; Saxon, Low',NULL,NULL,'Low German; Low Saxon; German, Low; Saxon, Low')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('306','nep','Nepali','Nepali','ne',NULL,'Nepali')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('307','new','Newari','Newari',NULL,NULL,'Newari')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('308','nia','Nias','Nias',NULL,NULL,'Nias')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('309','nic','Niger-Kordofanian (Other)','Niger-Kordofanian (Other)',NULL,NULL,'Niger-Kordofanian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('310','niu','Niuean','Niuean',NULL,NULL,'Niuean')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('311','dut','Dutch','Dutch',NULL,NULL,'Dutch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('312','non','Norse, Old','Norse, Old',NULL,NULL,'Norse, Old')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('313','nor','Norwegian','Norwegian','no',NULL,'Norwegian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('314','nno','Norwegian Nynorsk; Nynorsk, Norwegian','Norwegian Nynorsk; Nynorsk, Norwegian','nn-no','313','Norwegian Nynorsk; Nynorsk, Norwegian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('315','nob','Norwegian Bokmål; Bokmål, Norwegian','Norwegian Bokmål; Bokmål, Norwegian','nb-no','313','Norwegian Bokmål; Bokmål, Norwegian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('316','nso','Sotho, Northern','Sotho, Northern',NULL,NULL,'Sotho, Northern')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('317','nub','Nubian languages','Nubian languages',NULL,NULL,'Nubian languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('318','nya','Chichewa; Chewa; Nyanja','Chichewa; Chewa; Nyanja',NULL,NULL,'Chichewa; Chewa; Nyanja')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('319','nym','Nyamwezi','Nyamwezi',NULL,NULL,'Nyamwezi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('320','nyn','Nyankole','Nyankole',NULL,NULL,'Nyankole')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('321','nyo','Nyoro','Nyoro',NULL,NULL,'Nyoro')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('322','nzi','Nzima','Nzima',NULL,NULL,'Nzima')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('323','oci','Occitan (post 1500); Provençal','Occitan (post 1500); Provençal',NULL,NULL,'Occitan (post 1500); Provençal')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('324','oji','Ojibwa','Ojibwa',NULL,NULL,'Ojibwa')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('325','ori','Oriya','Oriya','or',NULL,'Oriya')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('326','orm','Oromo','Oromo',NULL,NULL,'Oromo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('327','osa','Osage','Osage',NULL,NULL,'Osage')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('328','oss','Ossetian; Ossetic','Ossetian; Ossetic',NULL,NULL,'Ossetian; Ossetic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('329','ota','Turkish, Ottoman (1500-1928)','Turkish, Ottoman (1500-1928)',NULL,NULL,'Turkish, Ottoman (1500-1928)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('330','oto','Otomian languages','Otomian languages',NULL,NULL,'Otomian languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('331','paa','Papuan (Other)','Papuan (Other)',NULL,NULL,'Papuan (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('332','pag','Pangasinan','Pangasinan',NULL,NULL,'Pangasinan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('333','pal','Pahlavi','Pahlavi',NULL,NULL,'Pahlavi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('334','pam','Pampanga','Pampanga',NULL,NULL,'Pampanga')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('335','pan','Panjabi','Panjabi','pa',NULL,'Panjabi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('336','pap','Papiamento','Papiamento',NULL,NULL,'Papiamento')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('337','pau','Palauan','Palauan',NULL,NULL,'Palauan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('338','peo','Persian, Old (ca.600-400 B.C.)','Persian, Old (ca.600-400 B.C.)',NULL,NULL,'Persian, Old (ca.600-400 B.C.)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('339','per','Persian','Persian',NULL,NULL,'Persian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('340','phi','Philippine (Other)','Philippine (Other)',NULL,NULL,'Philippine (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('341','phn','Phoenician','Phoenician',NULL,NULL,'Phoenician')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('342','pli','Pali','Pali',NULL,NULL,'Pali')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('343','pol','Polish','Polish','pl',NULL,'Polish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('344','pon','Pohnpeian','Pohnpeian',NULL,NULL,'Pohnpeian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('345','por','Portuguese','Portuguese','pt',NULL,'Portuguese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('346','pra','Prakrit languages','Prakrit languages',NULL,NULL,'Prakrit languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('347','pro','Provençal, Old (to 1500)','Provençal, Old (to 1500)',NULL,NULL,'Provençal, Old (to 1500)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('348','pus','Pushto','Pushto',NULL,NULL,'Pushto')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('350','que','Quechua','Quechua',NULL,NULL,'Quechua')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('351','raj','Rajasthani','Rajasthani',NULL,NULL,'Rajasthani')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('352','rap','Rapanui','Rapanui',NULL,NULL,'Rapanui')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('353','rar','Rarotongan','Rarotongan',NULL,NULL,'Rarotongan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('354','roa','Romance (Other)','Romance (Other)',NULL,NULL,'Romance (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('355','roh','Raeto-Romance','Raeto-Romance',NULL,NULL,'Raeto-Romance')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('356','rom','Romany','Romany','ro',NULL,'Romany')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('357','rum','Romanian','Romanian',NULL,NULL,'Romanian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('358','run','Rundi','Rundi',NULL,NULL,'Rundi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('359','rus','Russian','Russian','ru',NULL,'Russian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('360','sad','Sandawe','Sandawe',NULL,NULL,'Sandawe')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('361','sag','Sango','Sango',NULL,NULL,'Sango')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('362','sah','Yakut','Yakut',NULL,NULL,'Yakut')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('363','sai','South American Indian (Other)','South American Indian (Other)',NULL,NULL,'South American Indian (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('365','sal','Salishan languages','Salishan languages',NULL,NULL,'Salishan languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('366','sam','Samaritan Aramaic','Samaritan Aramaic',NULL,NULL,'Samaritan Aramaic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('367','san','Sanskrit','Sanskrit','sa',NULL,'Sanskrit')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('368','sas','Sasak','Sasak',NULL,NULL,'Sasak')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('369','sat','Santali','Santali',NULL,NULL,'Santali')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('371','sco','Scots','Scots',NULL,NULL,'Scots')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('373','sel','Selkup','Selkup',NULL,NULL,'Selkup')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('374','sem','Semitic (Other)','Semitic (Other)',NULL,NULL,'Semitic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('375','sga','Irish, Old (to 900)','Irish, Old (to 900)',NULL,NULL,'Irish, Old (to 900)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('376','sgn','Sign Languages','Sign Languages',NULL,NULL,'Sign Languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('377','shn','Shan','Shan',NULL,NULL,'Shan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('378','sid','Sidamo','Sidamo',NULL,NULL,'Sidamo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('379','sin','Sinhalese','Sinhalese',NULL,NULL,'Sinhalese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('380','sio','Siouan languages','Siouan languages',NULL,NULL,'Siouan languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('381','sit','Sino-Tibetan (Other)','Sino-Tibetan (Other)',NULL,NULL,'Sino-Tibetan (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('382','sla','Slavic (Other)','Slavic (Other)',NULL,NULL,'Slavic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('383','slo','Slovak','Slovak','sk',NULL,'Slovak')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('384','slv','Slovenian','Slovenian','sl',NULL,'Slovenian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('385','sma','Southern Sami','Southern Sami',NULL,NULL,'Southern Sami')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('386','sme','Northern Sami','Northern Sami',NULL,NULL,'Northern Sami')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('387','smi','Sami languages (Other)','Sami languages (Other)',NULL,NULL,'Sami languages (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('388','smj','Lule Sami','Lule Sami',NULL,NULL,'Lule Sami')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('389','smn','Inari Sami','Inari Sami',NULL,NULL,'Inari Sami')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('390','smo','Samoan','Samoan',NULL,NULL,'Samoan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('391','sms','Skolt Sami','Skolt Sami',NULL,NULL,'Skolt Sami')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('392','sna','Shona','Shona',NULL,NULL,'Shona')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('393','snd','Sindhi','Sindhi',NULL,NULL,'Sindhi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('394','snk','Soninke','Soninke',NULL,NULL,'Soninke')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('395','sog','Sogdian','Sogdian',NULL,NULL,'Sogdian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('396','som','Somali','Somali',NULL,NULL,'Somali')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('397','son','Songhai','Songhai',NULL,NULL,'Songhai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('398','sot','Sotho, Southern','Sotho, Southern',NULL,NULL,'Sotho, Southern')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('399','spa','Spanish; Castilian','Spanish; Castilian',NULL,NULL,'Spanish; Castilian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('400','alb','Albanian','Albanian','sq',NULL,'Albanian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('401','srd','Sardinian','Sardinian',NULL,NULL,'Sardinian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('402','scc','Serbian','Serbian','sb',NULL,'Serbian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('403','srr','Serer','Serer',NULL,NULL,'Serer')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('404','ssa','Nilo-Saharan (Other)','Nilo-Saharan (Other)',NULL,NULL,'Nilo-Saharan (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('405','ssw','Swati','Swati',NULL,NULL,'Swati')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('406','suk','Sukuma','Sukuma',NULL,NULL,'Sukuma')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('407','sun','Sundanese','Sundanese',NULL,NULL,'Sundanese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('408','sus','Susu','Susu',NULL,NULL,'Susu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('409','sux','Sumerian','Sumerian',NULL,NULL,'Sumerian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('410','swa','Swahili','Swahili','sw',NULL,'Swahili')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('411','swe','Swedish','Swedish','sv',NULL,'Swedish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('412','syr','Syriac','Syriac','syr',NULL,'Syriac')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('413','tah','Tahitian','Tahitian',NULL,NULL,'Tahitian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('414','tai','Tai (Other)','Tai (Other)',NULL,NULL,'Tai (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('415','tam','Tamil','Tamil','ta',NULL,'Tamil')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('416','tat','Tatar','Tatar','tt',NULL,'Tatar')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('417','tel','Telugu','Telugu','te',NULL,'Telugu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('418','tem','Timne','Timne',NULL,NULL,'Timne')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('419','ter','Tereno','Tereno',NULL,NULL,'Tereno')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('420','tet','Tetum','Tetum',NULL,NULL,'Tetum')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('421','tgk','Tajik','Tajik',NULL,NULL,'Tajik')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('422','tgl','Tagalog','Tagalog',NULL,NULL,'Tagalog')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('423','tha','Thai','Thai','th',NULL,'Thai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('425','tig','Tigre','Tigre',NULL,NULL,'Tigre')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('426','tir','Tigrinya','Tigrinya',NULL,NULL,'Tigrinya')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('427','tiv','Tiv','Tiv',NULL,NULL,'Tiv')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('428','tkl','Tokelau','Tokelau',NULL,NULL,'Tokelau')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('429','tli','Tlingit','Tlingit',NULL,NULL,'Tlingit')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('430','tmh','Tamashek','Tamashek',NULL,NULL,'Tamashek')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('431','tog','Tonga (Nyasa)','Tonga (Nyasa)',NULL,NULL,'Tonga (Nyasa)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('432','ton','Tonga (Tonga Islands)','Tonga (Tonga Islands)',NULL,NULL,'Tonga (Tonga Islands)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('433','tpi','Tok Pisin','Tok Pisin',NULL,NULL,'Tok Pisin')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('434','tsi','Tsimshian','Tsimshian',NULL,NULL,'Tsimshian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('435','tsn','Tswana','Tswana','tn',NULL,'Tswana')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('436','tso','Tsonga','Tsonga','ts',NULL,'Tsonga')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('437','tuk','Turkmen','Turkmen',NULL,NULL,'Turkmen')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('438','tum','Tumbuka','Tumbuka',NULL,NULL,'Tumbuka')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('439','tup','Tupi languages','Tupi languages',NULL,NULL,'Tupi languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('440','tur','Turkish','Turkish','tr',NULL,'Turkish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('441','tut','Altaic (Other)','Altaic (Other)',NULL,NULL,'Altaic (Other)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('442','tvl','Tuvalu','Tuvalu',NULL,NULL,'Tuvalu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('443','twi','Twi','Twi',NULL,NULL,'Twi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('444','tyv','Tuvinian','Tuvinian',NULL,NULL,'Tuvinian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('445','uga','Ugaritic','Ugaritic',NULL,NULL,'Ugaritic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('446','uig','Uighur','Uighur',NULL,NULL,'Uighur')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('447','ukr','Ukrainian','Ukrainian','uk',NULL,'Ukrainian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('448','umb','Umbundu','Umbundu',NULL,NULL,'Umbundu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('449','und','Undetermined','Undetermined',NULL,NULL,'Undetermined')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('450','urd','Urdu','Urdu','ur',NULL,'Urdu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('451','uzb','Uzbek','Uzbek','uz',NULL,'Uzbek')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('452','vai','Vai','Vai',NULL,NULL,'Vai')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('453','ven','Venda','Venda',NULL,NULL,'Venda')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('454','vie','Vietnamese','Vietnamese','vi',NULL,'Vietnamese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('455','vol','Volapük','Volapük',NULL,NULL,'Volapük')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('456','vot','Votic','Votic',NULL,NULL,'Votic')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('457','wak','Wakashan languages','Wakashan languages',NULL,NULL,'Wakashan languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('458','wal','Walamo','Walamo',NULL,NULL,'Walamo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('459','war','Waray','Waray',NULL,NULL,'Waray')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('460','was','Washo','Washo',NULL,NULL,'Washo')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('462','wen','Sorbian languages','Sorbian languages',NULL,NULL,'Sorbian languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('463','wln','Walloon','Walloon',NULL,NULL,'Walloon')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('464','wol','Wolof','Wolof',NULL,NULL,'Wolof')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('465','xho','Xhosa','Xhosa','xh',NULL,'Xhosa')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('466','yao','Yao','Yao',NULL,NULL,'Yao')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('467','yap','Yapese','Yapese',NULL,NULL,'Yapese')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('468','yid','Yiddish','Yiddish','yi',NULL,'Yiddish')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('469','yor','Yoruba','Yoruba',NULL,NULL,'Yoruba')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('470','ypk','Yupik languages','Yupik languages',NULL,NULL,'Yupik languages')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('471','zap','Zapotec','Zapotec',NULL,NULL,'Zapotec')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('472','zen','Zenaga','Zenaga',NULL,NULL,'Zenaga')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('473','zha','Zhuang; Chuang','Zhuang; Chuang',NULL,NULL,'Zhuang; Chuang')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('475','znd','Zande','Zande',NULL,NULL,'Zande')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('476','zul','Zulu','Zulu','zu',NULL,'Zulu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('477','zun','Zuni','Zuni',NULL,NULL,'Zuni')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('478','ENU','English (US)','English (US)','en-us','1','Englisch (US)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('479',NULL,'Arabic (Ägypten)','Arabic (Ägypten)','ar-eg','21','Arabic (Ägypten)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('480',NULL,'Arabic (Algerien)','Arabic (Algerien)','ar-dz','21','Arabic (Algerien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('481',NULL,'Arabic (Bahrain)','Arabic (Bahrain)','ar-bh','21','Arabic (Bahrain)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('482',NULL,'Arabic (Irak)','Arabic (Irak)','ar-iq','21','Arabic (Irak)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('483',NULL,'Arabic (Jemen)','Arabic (Jemen)','ar-ye','21','Arabic (Jemen)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('484',NULL,'Arabic (Jordanien)','Arabic (Jordanien)','ar-jo','21','Arabic (Jordanien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('485',NULL,'Arabic (Katar)','Arabic (Katar)','ar-qa','21','Arabic (Katar)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('486',NULL,'Arabic (Kuwait)','Arabic (Kuwait)','ar-kw','21','Arabic (Kuwait)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('487',NULL,'Arabic (Libanon)','Arabic (Libanon)','ar-lb','21','Arabic (Libanon)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('488',NULL,'Arabic (Libyen)','Arabic (Libyen)','ar-ly','21','Arabic (Libyen)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('489',NULL,'Arabic (Marokko)','Arabic (Marokko)','ar-ma','21','Arabic (Marokko)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('490',NULL,'Arabic (Oman)','Arabic (Oman)','ar-om','21','Arabic (Oman)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('491',NULL,'Arabic (Saudi-Arabien)','Arabic (Saudi-Arabien)','ar-sa','21','Arabic (Saudi-Arabien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('492',NULL,'Arabic (Syrien)','Arabic (Syrien)','ar-sy','21','Arabic (Syrien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('493',NULL,'Arabic (Tunesien)','Arabic (Tunesien)','ar-tn','21','Arabic (Tunesien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('494',NULL,'Arabic (Vereinigte Arabische)','Arabic (Vereinigte Arabische)','ar-ae','21','Arabic (Vereinigte Arabische)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('495',NULL,'Chinese (Hongkong S.A.R.)','Chinese (Hongkong S.A.R.)','zh-hk','80','Chinese (Hongkong S.A.R.)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('496',NULL,'Chinese (Macao S.A.R.)','Chinese (Macao S.A.R.)','zh-mo','80','Chinese (Macao S.A.R.)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('497',NULL,'Chinese (Singapur)','Chinese (Singapur)','zh-sg','80','Chinese (Singapur)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('498',NULL,'Chinese (Taiwan)','Chinese (Taiwan)','zh-tw','80','Chinese (Taiwan)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('499',NULL,'Chinese (VR China)','Chinese (VR China)','zh-cn','80','Chinese (VR China)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('500',NULL,'Francais (Belgien)','French (Belgien)','fr-be','3','Französisch (Belgien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('501',NULL,'Francais (Kanada)','French (Canada)','fr-ca','3','Französisch (Kanada)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('502',NULL,'Francais (Luxembourg)','French (Luxemburg)','fr-lu','3','Französisch (Luxemburg)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('503',NULL,'Francais (Monaco)','French (Monaco)','fr-mc','3','Französisch (Monaco)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('504',NULL,'Francais (Schweiz)','French (Schweiz)','fr-ch','3','Französisch (Schweiz)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('505',NULL,'Italian (Schweiz)','Italian (Schweiz)','it-ch','200','Italian (Schweiz)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('506',NULL,'Malayan','Malayan','ms',NULL,'Malayan')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('507',NULL,'Maledivian','Maledivian','div',NULL,'Maledivian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('508',NULL,'Portuguese (Brasilien)','Portuguese (Brasilien)','pt-br','345','Portuguese (Brasilien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('509',NULL,'Rhaetian','Rhaetian','rm',NULL,'Rhaetian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('510',NULL,'Deutsch (Liechtenstein)','German (Liechtenstein)','de-li','2','Deutsch (Liechtenstein)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('511',NULL,'Deutsch (Luxemburg)','German (Luxemburg)','de-lu','2','Deutsch (Luxemburg)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('512',NULL,'Deutsch (Österreich)','German (Austria)','de-at','2','Deutsch (Österreich)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('513',NULL,'Deutsch (Schweiz)','German (Switzerland)','de-ch','2','Deutsch (Schweiz)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('514',NULL,'English (Australien)','English (Australien)','en-au','1','Englisch (Australien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('515',NULL,'English (Belize)','English (Belize)','en-bz','1','Englisch (Belize)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('516',NULL,'English (GB)','English (GB)','en-gb','1','Englisch (GB)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('517',NULL,'English (Irland)','English (Irland)','en-ie','1','Englisch (Irland)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('518',NULL,'English (Jamaika)','English (Jamaika)','en-jm','1','Englisch (Jamaika)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('519',NULL,'English (Kanada)','English (Kanada)','en-ca','1','Englisch (Kanada)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('520',NULL,'English (Karibik)','English (Karibik)','en-???????','1','Englisch (Karibik)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('521',NULL,'English (Neuseeland)','English (Neuseeland)','en-nz','1','Englisch (Neuseeland)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('522',NULL,'English (Philippinen)','English (Philippinen)','en-ph','1','Englisch (Philippinen)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('523',NULL,'English (Südafrika)','English (Südafrika)','en-za','1','Englisch (Südafrika)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('524',NULL,'English (Trinidad)','English (Trinidad)','en-tt','1','Englisch (Trinidad)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('525',NULL,'English (Zimbabwe)','English (Zimbabwe)','en-zw','1','Englisch (Zimbabwe)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('526',NULL,'Romany (Moldau)','Romany (Moldau)','ro-md','356','Romany (Moldau)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('527',NULL,'Russian (Moldau)','Russian (Moldau)','ru-md','359','Russian (Moldau)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('528',NULL,'Swedish (Finnland)','Swedish (Finnland)','sv-fi','411','Swedish (Finnland)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('529',NULL,'Serbian (Kyrillisch,Lateinisch)','Serbian (Kyrillisch,Lateinisch)','sr',NULL,'Serbian (Kyrillisch,Lateinisch)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('530',NULL,'Espanol (Argentinien)','Spanish (Argentinien)','es-ar','4','Spanish (Argentinien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('531',NULL,'Espanol (Bolivien)','Spanish (Bolivien)','es-bo','4','Spanish (Bolivien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('532',NULL,'Espanol (Chile)','Spanish (Chile)','es-cl','4','Spanish (Chile)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('533',NULL,'Espanol (Costa-Rica)','Spanish (Costa-Rica)','es-cr','4','Spanish (Costa-Rica)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('534',NULL,'Espanol (Dominikan.Rep.)','Spanish (Dominikan.Rep.)','es-do','4','Spanish (Dominikan.Rep.)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('535',NULL,'Espanol (Ecuador)','Spanish (Ecuador)','es-ec','4','Spanish (Ecuador)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('536',NULL,'Espanol (El-Salvador)','Spanish (El-Salvador)','es-sv','4','Spanish (El-Salvador)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('537',NULL,'Espanol (Guatemala)','Spanish (Guatemala)','es-gt','4','Spanish (Guatemala)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('538',NULL,'Espanol (Honduras)','Spanish (Honduras)','es-hn','4','Spanish (Honduras)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('539',NULL,'Espanol (Internationale Sorti)','Spanish (Internationale Sorti)','es','4','Spanish (Internationale Sorti)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('540',NULL,'Espanol (Kolumbien)','Spanish (Kolumbien)','es-co','4','Spanish (Kolumbien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('541',NULL,'Espanol (Mexiko)','Spanish (Mexiko)','es-mx','4','Spanish (Mexiko)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('542',NULL,'Espanol (Nicaragua)','Spanish (Nicaragua)','es-ni','4','Spanish (Nicaragua)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('543',NULL,'Espanol (Panama)','Spanish (Panama)','es-pa','4','Spanish (Panama)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('544',NULL,'Espanol (Paraguay)','Spanish (Paraguay)','es-py','4','Spanish (Paraguay)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('545',NULL,'Espanol (Peru)','Spanish (Peru)','es-pe','4','Spanish (Peru)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('546',NULL,'Espanol (Puerto Rico)','Spanish (Puerto Rico)','es-pr','4','Spanish (Puerto Rico)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('547',NULL,'Espanol (Traditionelle Sortie)','Spanish (Traditionelle Sortie)','es','4','Spanish (Traditionelle Sortie)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('548',NULL,'Espanol (Uruguay)','Spanish (Uruguay)','es-uy','4','Spanish (Uruguay)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('549',NULL,'Espanol (US)','Spanish (US)','es-us','4','Spanish (US)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('550',NULL,'Espanol (Venezuela)','Spanish (Venezuela)','es-ve','4','Spanish (Venezuela)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('551',NULL,'Georgian','Georgian','ka',NULL,'Georgian')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('552',NULL,'Dutch (Belgien)','Dutch (Belgien)','nl-be','553','Dutch (Belgien)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('553',NULL,'Dutch','Dutch','nl',NULL,'Dutch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('554',NULL,'Färäisch','Färäisch','fo',NULL,'Färäisch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('555',NULL,'Farsi','Farsi','fa',NULL,'Farsi')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('556',NULL,'Galizisch','Galizisch','gl',NULL,'Galizisch')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('557',NULL,'Sutu','Sutu','sx',NULL,'Sutu')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('558',NULL,'Deutsch (Deutschland)','German (Germany)','de-de','2','Deutsch (Deutschland)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('559',NULL,'Francais (France)','French (France)','fr-fr','3','Französisch (Frankreich)')
	INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) VALUES('560',NULL,'Espanol (Espana)','Spanish (Spanish)','es-es','4','Spanish (Spanish)')
	IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages OFF
END
/******************************************************
  Insert data  End
******************************************************/

GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) 
UPDATE System_Languages
SET System_Languages.IsActive = Languages.IsActive
FROM System_Languages INNER JOIN Languages ON System_Languages.ID = Languages.ID

GO

----------------------------------------------------
-- dbo.Languages
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) 
drop table [dbo].[Languages]
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
drop view [dbo].[Languages]
GO

CREATE VIEW dbo.Languages
AS
SELECT     ID, Abbreviation, Description_OwnLang, Description_English AS Description, IsActive, BrowserLanguageID, AlternativeLanguage
FROM         dbo.System_Languages

GO

