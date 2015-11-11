----------------------------------------------------------------------------------------------------------------------------------------------
-- New module "TextModules" --
----------------------------------------------------------------------------------------------------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[TextModules]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[TextModules]
GO

CREATE TABLE [dbo].[TextModules] (
	[PrimaryID] [int] IDENTITY (1, 1) NOT NULL ,
	[MarketID] [int] NOT NULL ,
	[WebsiteAreaID] [nvarchar] (50) NOT NULL ,
	[ServerGroupID] [int] NOT NULL ,
	[Key] [nvarchar] (100) NOT NULL ,
	[Value] [ntext] NOT NULL ,
	[Version] [int] NOT NULL ,
	[TypeID] [int] NOT NULL ,
	[Released] [bit] NOT NULL ,
	[PublishedOn] [datetime] NULL ,
	[Title] [nvarchar] (1024) NULL ,   -- NOT YET REQUIRED --
	[IsDeleted] [bit] NULL ,
	CONSTRAINT [PK_TextModules] PRIMARY KEY  CLUSTERED 
	(
		[PrimaryID]
	)    ,
	CONSTRAINT [IX_TextModules] UNIQUE  NONCLUSTERED 
	(
		[ServerGroupID],
		[MarketID],
		[Version],
		[WebsiteAreaID],
		[Key]
	)   
)  TEXTIMAGE_ON [PRIMARY]
GO

----------------------------------------------------------------------------------------------------------------------------------------------
-- Additional indexes
----------------------------------------------------------------------------------------------------------------------------------------------

if exists (select * from sys.indexes where name = N'IX_TextModules_1' and object_id = object_id(N'[dbo].[TextModules]'))
DROP INDEX [IX_TextModules_1] ON [dbo].[TextModules]
GO
 CREATE  INDEX [IX_TextModules_1] ON [dbo].[TextModules]([ServerGroupID], [MarketID], [WebsiteAreaID], [Key], [TypeID]) 
GO

----------------------------------------------------------------------------------------------------------------------------------------------
-- New navigation items and security objects
----------------------------------------------------------------------------------------------------------------------------------------------

-------------------------------------------------------------
-- Fix of system app settings of English security objects
-------------------------------------------------------------
-- Update system app state for existing, english applications (other ones have already been correct)
UPDATE dbo.Applications
SET SystemApp = 1, SystemAppType = 3
WHERE Title IN 
	(
		'System - Administration - Redirections', 
		'System - TextModules', 
		'System - Mail Queue Monitor',
		'System - User Administration - LogAnalysis'
	)
	AND LanguageID = 1

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
	@Username nvarchar(20),
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
