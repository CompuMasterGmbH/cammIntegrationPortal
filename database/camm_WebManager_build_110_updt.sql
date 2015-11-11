----------------------------------------------------
-- dbo.AdminPrivate_CloneApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CloneApplication 
(
	@ReleasedByUserID int,
	@AppID int,
	@CloneType int
)
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
				                      LocationID, LanguageID, SystemApp, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, @ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, SystemApp, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL
				FROM         dbo.Applications
				WHERE     (ID = @AppID)

				SELECT @NewAppID = @@IDENTITY

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
				                      LocationID, LanguageID, SystemApp, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, 
						@ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, SystemApp, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, @AppID As AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL
				FROM         dbo.Applications
				WHERE    (ID = @AppID)

				SELECT @NewAppID = @@IDENTITY
			END

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
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
(
	@Username nvarchar(20),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32)
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @System_SessionID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = ID, @System_SessionID = System_SessionID from dbo.Benutzer where LoginName = @Username
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END

-------------
-- Logout --
-------------
-- Rückgabewert
SELECT Result = -1
-- CurUserCurrentLoginViaRemoteIP und SessionIDs zurücksetzen
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null WHERE LoginName = @Username
-- Session schließen
UPDATE dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
SET Inactive = 1
WHERE SessionID = @System_SessionID
-- Weitere Sessions des gleichen Benutzers schließen, welche evtl. an anderen Servern nicht ausgeloggt wurden
update [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
set inactive = 1
from [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] inner join [System_UserSessions]
	on [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].sessionid = [System_UserSessions].ID_Session
where [System_UserSessions].ID_User = @CurUserID
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
-- dbo.Public_GetLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetLogonList
	(
	@Username nvarchar(20),
	@ScriptEngine_SessionID nvarchar(512) = NULL,
	@ScriptEngine_ID int = NULL,
	@ServerID int = NULL
	)
WITH ENCRYPTION
AS

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
----------------------------------------------------
-- dbo.Public_GetToDoLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetToDoLogonList
	(
	@Username nvarchar(20),
	@ScriptEngine_SessionID nvarchar(512),
	@ScriptEngine_ID int,
	@ServerID int
	)
WITH ENCRYPTION
AS

-- GUIDs alter Sessions zurücksetzen
SET NOCOUNT ON
UPDATE    System_WebAreasAuthorizedForSession
SET              Inactive = 1
WHERE     (LastSessionStateRefresh < DATEADD(hh, - 12, GETDATE()))

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
             
         System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
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
-----------------------------------------------------------
--Insert data into dbo.Gruppen: Security Related Contacts
-----------------------------------------------------------
DECLARE @ModifiedBy int
SELECT @ModifiedBy = -43
DECLARE @MyGroupID int
SELECT @MyGroupID = ID FROM Gruppen WHERE ID = -7
IF @MyGroupID IS NULL
BEGIN
	IF(	IDENT_INCR( 'dbo.Gruppen' ) IS NOT NULL OR IDENT_SEED('dbo.Gruppen') IS NOT NULL ) SET IDENTITY_INSERT dbo.Gruppen ON
	INSERT INTO dbo.Gruppen (ID,Name,Description,ReleasedOn,ReleasedBy,SystemGroup,ModifiedOn,ModifiedBy) VALUES('-7','Security Related Contacts','System group: special users with (for example) responsibility for a camm Web-Manager security object or access to log analysis',getdate(),@ModifiedBy,1,getdate(),@ModifiedBy)
	IF(	IDENT_INCR( 'dbo.Gruppen' ) IS NOT NULL OR IDENT_SEED('dbo.Gruppen') IS NOT NULL ) SET IDENTITY_INSERT dbo.Gruppen OFF
END
GO
-----------------------------------------------------------
--Insert data into security objects and authorizations: Logs
-----------------------------------------------------------
DECLARE @ModifiedBy int
SELECT @ModifiedBy = -43
DECLARE @AppID_Logs int, @AdminServerID int
SELECT @AppID_Logs = ID FROM dbo.Applications_CurrentAndInactiveOnes WHERE Title = 'System - User Administration - LogAnalysis'
IF @AppID_Logs IS NULL
BEGIN
	-- Retrieve one server ID
	SELECT TOP 1 @AdminServerID = System_Servers.ID
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- 1st security object
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	VALUES('System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,'Web Administration','Log Analysis','Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,@AdminServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
	SELECT @AppID_Logs = @@IDENTITY
	-- Rest of new security objects - same languages (English)
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,'Web-Administration','Log Analysis','Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,System_Servers.ID,1,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	WHERE System_Servers.ID <> @AdminServerID
	GROUP BY System_Servers.ID
	-- Rest of new security objects - other languages
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - User Administration - LogAnalysis',NULL,getdate(),@ModifiedBy,'Web-Administration','Log Auswertungen','Web-Manager',NULL,NULL,NULL,'[ADMINURL]logs/index.aspx',NULL,NULL,0,0,System_Servers.ID,2,1,getdate(),@ModifiedBy,0,@AppID_Logs ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- Authorizations
	INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Logs ,7,getdate(),@ModifiedBy)
	INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Logs ,6,getdate(),@ModifiedBy)
END
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
