-----------------------------------------------
-- Add indexes for increased performance 
-----------------------------------------------
if exists (select * from sys.indexes where name = N'IX_ApplicationsRightsByGroup_2' and object_id = object_id(N'[dbo].[ApplicationsRightsByGroup]'))
DROP INDEX [IX_ApplicationsRightsByGroup_2] ON [dbo].[ApplicationsRightsByGroup]
GO

 CREATE  INDEX [IX_ApplicationsRightsByGroup_2] ON [dbo].[ApplicationsRightsByGroup]([ID_Application], [ID_GroupOrPerson]) 
GO
if exists (select * from sys.indexes where name = N'IX_ApplicationsRightsByGroup_3' and object_id = object_id(N'[dbo].[ApplicationsRightsByGroup]'))
DROP INDEX [IX_ApplicationsRightsByGroup_3] ON [dbo].[ApplicationsRightsByGroup]
GO

 CREATE  INDEX [IX_ApplicationsRightsByGroup_3] ON [dbo].[ApplicationsRightsByGroup]([ID_GroupOrPerson], [ID_Application]) 
GO
if exists (select * from sys.indexes where name = N'IX_Applications_CurrentAndInactiveOnes_1' and object_id = object_id(N'[dbo].[Applications_CurrentAndInactiveOnes]'))
DROP INDEX [IX_Applications_CurrentAndInactiveOnes_1] ON [dbo].[Applications_CurrentAndInactiveOnes]
GO
 CREATE  INDEX [IX_Applications_CurrentAndInactiveOnes_1] ON [dbo].[Applications_CurrentAndInactiveOnes]([LocationID], [Title], [ID]) 
GO
if exists (select * from sys.indexes where name = N'IX_ApplicationsRightsByUser_2' and object_id = object_id(N'[dbo].[ApplicationsRightsByUser]'))
DROP INDEX [IX_ApplicationsRightsByUser_2] ON [dbo].[ApplicationsRightsByUser]
GO

 CREATE  INDEX [IX_ApplicationsRightsByUser_2] ON [dbo].[ApplicationsRightsByUser]([ID_Application], [ID_GroupOrPerson]) 
GO
if exists (select * from sys.indexes where name = N'IX_ApplicationsRightsByUser_3' and object_id = object_id(N'[dbo].[ApplicationsRightsByUser]'))
DROP INDEX [IX_ApplicationsRightsByUser_3] ON [dbo].[ApplicationsRightsByUser]
GO

 CREATE  INDEX [IX_ApplicationsRightsByUser_3] ON [dbo].[ApplicationsRightsByUser]([ID_GroupOrPerson], [ID_Application]) 
GO
if exists (select * from sys.indexes where name = N'IX_Memberships_2' and object_id = object_id(N'[dbo].[Memberships]'))
DROP INDEX [IX_Memberships_2] ON [dbo].[Memberships]
GO

 CREATE  INDEX [IX_Memberships_2] ON [dbo].[Memberships]([ID_Group], [ID_User]) 
GO
if exists (select * from sys.indexes where name = N'IX_Memberships_3' and object_id = object_id(N'[dbo].[Memberships]'))
DROP INDEX [IX_Memberships_3] ON [dbo].[Memberships]
GO

 CREATE  INDEX [IX_Memberships_3] ON [dbo].[Memberships]([ID_User], [ID_Group]) 
GO
if exists (select * from sys.indexes where name = N'IX_System_Servers_1' and object_id = object_id(N'[dbo].[System_Servers]'))
DROP INDEX [IX_System_Servers_1] ON [dbo].[System_Servers]
GO

 CREATE  INDEX [IX_System_Servers_1] ON [dbo].[System_Servers]([IP], [ID]) 
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

-- GUIDs alter Sessions zurücksetzen
DELETE FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
WHERE(Inactive = 1 And (LastSessionStateRefresh < DateAdd(Day, -3, GETDATE())))

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


-----------------------------------------------
-- ReCreate download handler table
-----------------------------------------------

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[WebManager_DownloadHandler_Files]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[WebManager_DownloadHandler_Files]
GO

CREATE TABLE [dbo].[WebManager_DownloadHandler_Files] (
	[UniqueCrypticID] [char] (36) NOT NULL ,
	[VirtualDownloadLocation] [nvarchar] (3800) NOT NULL ,
	[ServerID] [int] NOT NULL ,
	[TimeOfRemoval] [datetime] NOT NULL ,
	CONSTRAINT [PK_WebManager_DownloadHandler_Files] PRIMARY KEY  CLUSTERED 
	(
		[UniqueCrypticID]
	)    ,
) 
GO
CREATE NONCLUSTERED INDEX IX_WebManager_DownloadHandler_Files_1 ON dbo.WebManager_DownloadHandler_Files
	(
	ServerID,
	TimeOfRemoval
	) 
GO

----------------------------------------------
-- Added field Company in memberships list
----------------------------------------------
ALTER VIEW dbo.view_Memberships
AS
SELECT     dbo.Memberships.ID AS ID_Membership, dbo.Gruppen.ID AS ID_Group, dbo.Gruppen.Name, dbo.Gruppen.Description, dbo.Memberships.ReleasedOn,
Benutzer1.ID AS ID_ReleasedBy, Benutzer1.Vorname AS ReleasedByFirstName, Benutzer1.Nachname AS ReleasedByLastName,
dbo.Benutzer.ID AS ID_User, dbo.Benutzer.Loginname, dbo.Benutzer.Vorname, dbo.Benutzer.Nachname, dbo.Benutzer.LoginDisabled, dbo.Benutzer.Company 
FROM         dbo.Memberships LEFT OUTER JOIN
dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer1 ON dbo.Memberships.ReleasedBy = Benutzer1.ID RIGHT OUTER JOIN
dbo.Gruppen ON dbo.Memberships.ID_Group = dbo.Gruppen.ID
