if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[LookupUserNameByScriptEngineSessionID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1) drop procedure [dbo].[LookupUserNameByScriptEngineSessionID]
GO
CREATE PROC dbo.LookupUserNameByScriptEngineSessionID
	(
	@ServerID int,
	@ScriptEngineID int,
	@ScriptEngineSessionID nvarchar(128)
	)

AS
SELECT Benutzer.LoginName
FROM [System_WebAreasAuthorizedForSession] AS SSID
	LEFT JOIN System_UserSessions AS USID ON SSID.SessionID = USID.ID_Session
	LEFT JOIN Benutzer ON USID.ID_User = Benutzer.ID
WHERE SSID.Server = @ServerID
	AND SSID.ScriptEngine_ID = @ScriptEngineID
	AND SSID.ScriptEngine_SessionID = @ScriptEngineSessionID
