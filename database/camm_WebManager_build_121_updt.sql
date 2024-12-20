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

AS

-- GUIDs alter Sessions zurücksetzen
SET NOCOUNT ON
UPDATE    System_WebAreasAuthorizedForSession
SET              Inactive = 1
WHERE     (LastSessionStateRefresh < DATEADD(hh, - 12, GETDATE()))

-- GUIDs alter Sessions zurücksetzen
DELETE FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
WHERE(Inactive = 1 And (LastSessionStateRefresh < DateAdd(Day, -3, GETDATE())))

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
             
         System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, System_WebAreasAuthorizedForSession.ScriptEngine_ID,
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
