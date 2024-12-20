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
WHERE SessionID = @System_SessionID
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
