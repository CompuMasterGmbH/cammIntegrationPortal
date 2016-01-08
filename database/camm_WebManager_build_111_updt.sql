if exists (select * from sys.objects where object_id = object_id(N'[dbo].[WebManager_DownloadHandler_Files]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table dbo.[WebManager_DownloadHandler_Files]
GO

CREATE TABLE dbo.[WebManager_DownloadHandler_Files] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[DownloadLocation] [nvarchar] (450) NOT NULL ,
	[TimeOfRemoval] [datetime] NOT NULL ,
	CONSTRAINT [PK_WebManager_DownloadHandler_Files] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
) ON [PRIMARY]
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
		-- LoginCount hochzählen
		update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1 where LoginName = @Username
		-- LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 98, 'Validation of login data successfull')
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = @@IDENTITY
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
				update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), CurrentLoginViaRemoteIP = @RemoteIP where LoginName = @Username
				UPDATE dbo.Benutzer SET System_SessionID = @CurUserStatus_InternalSessionID WHERE LoginName = @UserName
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
				where [System_UserSessions].ID_User = @CurUserID and sessionid <> @CurUserStatus_InternalSessionID
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
				where sessionid <> @CurUserStatus_InternalSessionID and sessionid in (
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
----------------------------------------------------
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
(
	@Username nvarchar(20),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int = NULL,
	@ScriptEngine_SessionID nvarchar(512) = NULL
)
WITH ENCRYPTION
AS

SET NOCOUNT ON

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
WHERE SessionID = @System_SessionID
/* 
-- Nicht grundsätzlich alle Sessions beenden, welche vom gleichen Benutzer stammen sonder nur diejenigen, welche auch von der aktuellen Session stammen
-- Ansonsten würde das Logoff von z. B. einer Standalone-Applikation zum Beenden auch der WebSessions führen
-- BEGIN DEACTIVATION 
-- Weitere Sessions des gleichen Benutzers schließen, welche evtl. an anderen Servern nicht ausgeloggt wurden
update [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
set inactive = 1
from [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] inner join [System_UserSessions]
	on [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].sessionid = [System_UserSessions].ID_Session
where [System_UserSessions].ID_User = @CurUserID
-- END DEACTIVATION 
*/
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
where sessionid in (
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
