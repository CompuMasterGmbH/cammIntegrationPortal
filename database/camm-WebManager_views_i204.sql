/* Contains all views cumulated in their latest version up to build 187  */

----------------------------------------------------
-- [dbo].[AdminPrivate_ServerGroupAccessLevels]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ServerGroupAccessLevels]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[AdminPrivate_ServerGroupAccessLevels]
GO
CREATE VIEW dbo.AdminPrivate_ServerGroupAccessLevels
AS
SELECT     dbo.System_ServerGroupsAndTheirUserAccessLevels.ID, dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup,
dbo.System_ServerGroupsAndTheirUserAccessLevels.Remarks, dbo.System_AccessLevels.ID AS AccessLevels_ID,
dbo.System_AccessLevels.Title AS AccessLevels_Title, dbo.System_AccessLevels.Remarks AS AccessLevels_Remarks
FROM         dbo.System_AccessLevels INNER JOIN
dbo.System_ServerGroupsAndTheirUserAccessLevels ON
dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
----------------------------------------------------
-- [dbo].[AdminPrivate_ServerRelations]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ServerRelations]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[AdminPrivate_ServerRelations]
GO
CREATE VIEW dbo.AdminPrivate_ServerRelations
AS
SELECT     dbo.System_ServerGroups.*, Gruppen_1.ID AS Group_Public_ID, Gruppen_1.Name AS Group_Public_Name, Gruppen_2.ID AS Group_Anonymous_ID,
Gruppen_2.Name AS Group_Anonymous_Name, System_Servers_1.ID AS UserAdminServer_ID,
System_Servers_1.Enabled AS UserAdminServer_Enabled, System_Servers_1.IP AS UserAdminServer_IP,
System_Servers_1.ServerDescription AS UserAdminServer_ServerDescription,
System_Servers_1.ServerProtocol AS UserAdminServer_ServerProtocol, System_Servers_1.ServerName AS UserAdminServer_ServerName,
System_Servers_1.ServerPort AS UserAdminServer_ServerPort, System_Servers_3.ID AS MasterServer_ID,
System_Servers_3.Enabled AS MasterServer_Enabled, System_Servers_3.IP AS MasterServer_IP,
System_Servers_3.ServerDescription AS MasterServer_ServerDescription, System_Servers_3.ServerProtocol AS MasterServer_ServerProtocol,
System_Servers_3.ServerName AS MasterServer_ServerName, System_Servers_3.ServerPort AS MasterServer_ServerPort,
System_Servers_2.ID AS MemberServer_ID, System_Servers_2.Enabled AS MemberServer_Enabled, System_Servers_2.IP AS MemberServer_IP,
System_Servers_2.ServerDescription AS MemberServer_ServerDescription, System_Servers_2.ServerProtocol AS MemberServer_ServerProtocol,
System_Servers_2.ServerName AS MemberServer_ServerName, System_Servers_2.ServerPort AS MemberServer_ServerPort
FROM         dbo.System_ServerGroups INNER JOIN
dbo.System_Servers System_Servers_1 ON dbo.System_ServerGroups.UserAdminServer = System_Servers_1.ID INNER JOIN
dbo.Gruppen Gruppen_1 ON dbo.System_ServerGroups.ID_Group_Public = Gruppen_1.ID INNER JOIN
dbo.Gruppen Gruppen_2 ON dbo.System_ServerGroups.ID_Group_Anonymous = Gruppen_2.ID INNER JOIN
dbo.System_Servers System_Servers_3 ON dbo.System_ServerGroups.MasterServer = System_Servers_3.ID INNER JOIN
dbo.System_Servers System_Servers_2 ON dbo.System_ServerGroups.ID = System_Servers_2.ServerGroup
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
----------------------------------------------------
-- [dbo].[Applications]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[Applications]
GO
CREATE VIEW dbo.Applications
AS
SELECT     dbo.Applications_CurrentAndInactiveOnes.*
FROM         dbo.Applications_CurrentAndInactiveOnes
WHERE     (AppDeleted = 0)
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[SecurityObjects]
GO
CREATE VIEW [dbo].[SecurityObjects]
WITH ENCRYPTION
AS
SELECT * 
FROM dbo.SecurityObjects_CurrentAndInactiveOnes
WHERE IsNull(Deleted, 0) = 0
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_CumulatedAuthsPerUser]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[SecurityObjects_CumulatedAuthsPerUser]
GO
CREATE VIEW [dbo].[SecurityObjects_CumulatedAuthsPerUser]
WITH ENCRYPTION
AS
-- secobj 2 user
select securityobjects.id AS ID_SecurityObject, dbo.SecurityObjects_AuthsByUser.ID_ServerGroup, dbo.SecurityObjects_AuthsByUser.ID_User
from dbo.securityobjects 
	inner join dbo.SecurityObjects_AuthsByUser on dbo.securityobjects.id = dbo.SecurityObjects_AuthsByUser.id_securityobject
UNION ALL
-- secobj 2 group 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup, dbo.Memberships.ID_User
from dbo.securityobjects
	inner join dbo.SecurityObjects_AuthsByGroup on dbo.securityobjects.id = dbo.SecurityObjects_AuthsByGroup.id_securityobject
	inner join dbo.Memberships on dbo.SecurityObjects_AuthsByGroup.ID_Group = dbo.Memberships.ID_Group
UNION ALL
-- secobj 2 inheritedSecObj 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByUser.ID_ServerGroup, dbo.SecurityObjects_AuthsByUser.ID_User
from dbo.securityobjects 
	inner join dbo.SecurityObjects_AuthsByUser on dbo.securityobjects.authsasSecObjid = dbo.SecurityObjects_AuthsByUser.id_securityobject
UNION ALL
-- secobj 2 inheritedSecObj 2 group 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup, dbo.Memberships.ID_User
from dbo.securityobjects
	inner join dbo.SecurityObjects_AuthsByGroup on dbo.securityobjects.authsasSecObjid = dbo.SecurityObjects_AuthsByGroup.id_securityobject
	inner join dbo.Memberships on dbo.SecurityObjects_AuthsByGroup.ID_Group = dbo.Memberships.ID_Group
UNION ALL
-- all secobjs 2 supervisors-group 2 user
select securityobjects.id, null, dbo.Memberships.ID_User
from dbo.securityobjects
	cross join dbo.Memberships
where dbo.Memberships.ID_Group = 6
UNION ALL
-- secobj 2 anonymous-group 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup, -1
from dbo.securityobjects
	inner join dbo.SecurityObjects_AuthsByGroup on dbo.securityobjects.id = dbo.SecurityObjects_AuthsByGroup.id_securityobject
	inner join dbo.System_ServerGroups on dbo.SecurityObjects_AuthsByGroup.ID_Group = dbo.System_ServerGroups.ID_Group_Anonymous
where dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup = dbo.System_ServerGroups.ID
UNION ALL
-- secobj 2 inheritedSecObj 2 anonymous-group 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup, -1
from dbo.securityobjects
	inner join dbo.SecurityObjects_AuthsByGroup on dbo.securityobjects.authsasSecObjid = dbo.SecurityObjects_AuthsByGroup.id_securityobject
	inner join dbo.System_ServerGroups on dbo.SecurityObjects_AuthsByGroup.ID_Group = dbo.System_ServerGroups.ID_Group_Anonymous
where dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup = dbo.System_ServerGroups.ID
UNION ALL
-- secobj 2 public-group 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup, -2
from dbo.securityobjects
	inner join dbo.SecurityObjects_AuthsByGroup on dbo.securityobjects.id = dbo.SecurityObjects_AuthsByGroup.id_securityobject
	inner join dbo.System_ServerGroups on dbo.SecurityObjects_AuthsByGroup.ID_Group = dbo.System_ServerGroups.ID_Group_Public
where dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup = dbo.System_ServerGroups.ID
UNION ALL
-- secobj 2 inheritedSecObj 2 public-group 2 user
select securityobjects.id, dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup, -2
from dbo.securityobjects
	inner join dbo.SecurityObjects_AuthsByGroup on dbo.securityobjects.authsasSecObjID = dbo.SecurityObjects_AuthsByGroup.id_securityobject
	inner join dbo.System_ServerGroups on dbo.SecurityObjects_AuthsByGroup.ID_Group = dbo.System_ServerGroups.ID_Group_Public
where dbo.SecurityObjects_AuthsByGroup.ID_ServerGroup = dbo.System_ServerGroups.ID
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[debug_SecurityObjects_CumulatedAuthsPerUser_woDuplicates]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[debug_SecurityObjects_CumulatedAuthsPerUser_woDuplicates]
GO
CREATE VIEW [dbo].[debug_SecurityObjects_CumulatedAuthsPerUser_woDuplicates]
AS
select id_securityobject, id_servergroup, id_user
from [SecurityObjects_CumulatedAuthsPerUser]
group by id_securityobject, id_servergroup, id_user
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[debug_CumulatedAuths_OldSchema]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[debug_CumulatedAuths_OldSchema]
GO
CREATE VIEW [dbo].[debug_CumulatedAuths_OldSchema]
AS
-- User auths cumulated - old schema
select dbo.system_servers.ServerGroup as ServerGroupID, Applications.LocationID, applications.LanguageID, applications.title, 
	Applications.Level1Title, Applications.Level2Title, Applications.Level3Title, Applications.Level4Title, Applications.Level5Title, Applications.Level6Title,
	Applications.NavURL, Applications.AppDisabled
	,Benutzer.ID as ID_User
from applications 
	inner join dbo.view_ApplicationRights_CommulatedPerUser as auths on Applications.ID = auths.ID_Application
	inner join Benutzer on auths.ID_User = Benutzer.ID
	inner join dbo.System_Servers on Applications.LocationID = dbo.System_Servers.ID
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[debug_CumulatedAuths_NewSchema]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[debug_CumulatedAuths_NewSchema]
GO
CREATE VIEW [dbo].[debug_CumulatedAuths_NewSchema]
AS
-- User auths cumulated - new schema
select auths.ID_ServerGroup, NavItems.ServerID, NavItems.LanguageID, securityobjects.title, 
	NavItems.Level1Title, NavItems.Level2Title, NavItems.Level3Title, NavItems.Level4Title, NavItems.Level5Title, NavItems.Level6Title,
	NavItems.NavURL, NavItems.[Disabled]
	,Benutzer.ID as ID_User
from navitems 
	inner join dbo.System_Servers on navitems.ServerID = dbo.System_Servers.ID
	inner join dbo.SecurityObjects_vs_NavItems as secobjvsnavitems on secobjvsnavitems.id_navitem = navitems.id
	inner join securityobjects on secobjvsnavitems.id_securityobject = securityobjects.id
	inner join [debug_SecurityObjects_CumulatedAuthsPerUser_woDuplicates] as auths on securityobjects.ID = auths.ID_SecurityObject
	inner join Benutzer on auths.ID_User = Benutzer.ID
where dbo.System_Servers.ServerGroup = auths.ID_ServerGroup
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser_FAST]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[view_ApplicationRights_CommulatedPerUser_FAST]
GO
CREATE view [dbo].[view_ApplicationRights_CommulatedPerUser_FAST]
as
select ID_Application, ID_User
from
(
-- secobj 2 user
select Applications.id AS ID_Application, dbo.ApplicationsRightsByUser.ID_GroupOrPerson as ID_User
from dbo.Applications 
	inner join dbo.ApplicationsRightsByUser on dbo.Applications.id = dbo.ApplicationsRightsByUser.ID_Application
UNION ALL
-- secobj 2 group 2 user
select Applications.id, dbo.Memberships.ID_User
from dbo.Applications
	inner join dbo.ApplicationsRightsByGroup on dbo.Applications.id = dbo.ApplicationsRightsByGroup.ID_Application
	inner join dbo.Memberships on dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Memberships.ID_Group
UNION ALL
-- secobj 2 inheritedSecObj 2 user
select Applications.id, dbo.ApplicationsRightsByUser.ID_GroupOrPerson
from dbo.Applications 
	inner join dbo.ApplicationsRightsByUser on dbo.Applications.authsasappid = dbo.ApplicationsRightsByUser.ID_Application
UNION ALL
-- secobj 2 inheritedSecObj 2 group 2 user
select Applications.id, dbo.Memberships.ID_User
from dbo.Applications
	inner join dbo.ApplicationsRightsByGroup on dbo.Applications.authsasappid = dbo.ApplicationsRightsByGroup.ID_Application
	inner join dbo.Memberships on dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Memberships.ID_Group
UNION ALL
-- all secobjs 2 supervisors-group 2 user
select Applications.id, dbo.Memberships.ID_User
from dbo.Applications
	cross join dbo.Memberships
where dbo.Memberships.ID_Group = 6
UNION ALL
-- secobj 2 anonymous-group 2 user
select Applications.id, -1
from dbo.Applications
	inner join dbo.ApplicationsRightsByGroup on dbo.Applications.id = dbo.ApplicationsRightsByGroup.ID_Application
	inner join dbo.System_ServerGroups on dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.System_ServerGroups.ID_Group_Anonymous
UNION ALL
-- secobj 2 inheritedSecObj 2 anonymous-group 2 user
select Applications.id, -1
from dbo.Applications
	inner join dbo.ApplicationsRightsByGroup on dbo.Applications.authsasappid = dbo.ApplicationsRightsByGroup.ID_Application
	inner join dbo.System_ServerGroups on dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.System_ServerGroups.ID_Group_Anonymous
UNION ALL
-- secobj 2 public-group 2 user
select Applications.id, -2
from dbo.Applications
	inner join dbo.ApplicationsRightsByGroup on dbo.Applications.id = dbo.ApplicationsRightsByGroup.ID_Application
	inner join dbo.System_ServerGroups on dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.System_ServerGroups.ID_Group_Public
UNION ALL
-- secobj 2 inheritedSecObj 2 public-group 2 user
select Applications.id, -2
from dbo.Applications
	inner join dbo.ApplicationsRightsByGroup on dbo.Applications.authsasappID = dbo.ApplicationsRightsByGroup.ID_Application
	inner join dbo.System_ServerGroups on dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.System_ServerGroups.ID_Group_Public
) as AppAuths
group by ID_Application, ID_User
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
----------------------------------------------------
-- [dbo].[Applications]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[Applications]
GO
CREATE VIEW dbo.Applications
AS
SELECT     dbo.Applications_CurrentAndInactiveOnes.*
FROM         dbo.Applications_CurrentAndInactiveOnes
WHERE     (AppDeleted = 0)
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
drop view [dbo].[view_ApplicationRights]
GO

CREATE  VIEW dbo.view_ApplicationRights
AS
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END AS TitleAdminAreaDisplay, dbo.Applications.Level1Title, 
                      dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID AS ID_Application, 
                      Benutzer1.ID AS AppReleasedByID, Benutzer1.Vorname AS AppReleasedByVorname, Benutzer1.Nachname AS AppReleasedByNachname, 
                      Applications.ReleasedOn AS AppReleasedOn, ApplicationsRightsByGroup.ID AS ID_AppRight, NULL AS ID_User, NULL AS LoginDisabled, NULL 
                      AS Loginname, NULL AS Vorname, NULL AS Nachname,null AS [Name1],
		      Gruppen.ID AS ID_Group, Gruppen.Name, Gruppen.Description, 1 AS ItemType, 
                      Applications.AppDisabled AS AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, NULL AS ThisAuthIsFromAppID, 
                      dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL, 
                      SystemApp AS SystemApp, SystemAppType AS SystemAppType, IsNull(ApplicationsRightsByGroup.DevelopmentTeamMember, 0) AS DevelopmentTeamMember, IsNull(ApplicationsRightsByGroup.IsDenyRule, 0) AS IsDenyRule, Benutzer1.Company as CompanyName
FROM         dbo.Applications LEFT OUTER JOIN
                      dbo.ApplicationsRightsByGroup LEFT OUTER JOIN
                      dbo.Gruppen ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Gruppen.ID ON 
                      dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
UNION
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID AS ID_Application, 
                      Benutzer1.ID AS AppReleasedByID, Benutzer1.Vorname AS AppReleasedByVorname, Benutzer1.Nachname AS AppReleasedByNachname, 
                      Applications.ReleasedOn AS AppReleasedOn, ApplicationsRightsByGroup.ID AS ID_AppRight, NULL AS ID_User, NULL AS LoginDisabled, NULL 
                      AS Loginname, NULL AS Vorname, NULL AS Nachname, null AS [Name1], 
		      Gruppen.ID AS ID_Group, Gruppen.Name, Gruppen.Description, 1 AS ItemType, 
                      Applications.AppDisabled AS AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, Applications.AuthsAsAppID AS ThisAuthIsFromAppID, 
                      dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL, SystemApp, 
                      SystemAppType, IsNull(ApplicationsRightsByGroup.DevelopmentTeamMember, 0), IsNull(ApplicationsRightsByGroup.IsDenyRule, 0), Benutzer1.Company 
FROM         dbo.Applications LEFT OUTER JOIN
                      dbo.ApplicationsRightsByGroup LEFT OUTER JOIN
                      dbo.Gruppen ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Gruppen.ID ON 
                      dbo.Applications.AuthsAsAppID = dbo.ApplicationsRightsByGroup.ID_Application LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
UNION
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID, Benutzer1.ID, Benutzer1.Vorname, 
                      Benutzer1.Nachname, Applications.ReleasedOn, ApplicationsRightsByUser.ID, Benutzer.ID, Benutzer.LoginDisabled, Benutzer.Loginname, 
                      Benutzer.Vorname, Benutzer.Nachname,  ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname AS [Name1],
		  NULL, NULL, NULL, 2, Applications.AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, NULL 
                      AS ThisAuthIsFromAppID, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0), IsNull(ApplicationsRightsByUser.IsDenyRule, 0), Benutzer1.Company 
FROM         dbo.Benutzer RIGHT OUTER JOIN
                      dbo.ApplicationsRightsByUser ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson RIGHT OUTER JOIN
                      dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.ID LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
UNION
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID, Benutzer1.ID, Benutzer1.Vorname, 
                      Benutzer1.Nachname, Applications.ReleasedOn, ApplicationsRightsByUser.ID, Benutzer.ID, Benutzer.LoginDisabled, Benutzer.Loginname, 
                      Benutzer.Vorname, Benutzer.Nachname, ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname AS [Name1],
		      NULL, NULL, NULL, 2, Applications.AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, 
                      Applications.AuthsAsAppID AS ThisAuthIsFromAppID, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0), IsNull(ApplicationsRightsByUser.IsDenyRule, 0), Benutzer1.Company 
FROM         dbo.Benutzer RIGHT OUTER JOIN
                      dbo.ApplicationsRightsByUser ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson RIGHT OUTER JOIN
                      dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.AuthsAsAppID LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID


GO

----------------------------------------------------
-- dbo.[view_ApplicationRights_CommulatedPerUser]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_ApplicationRights_CommulatedPerUser]
GO
create view dbo.[view_ApplicationRights_CommulatedPerUser]
as
SELECT     dbo.view_ApplicationRights.Title, dbo.view_ApplicationRights.ID_Application, ISNULL(dbo.view_ApplicationRights.ID_User, 
                      dbo.view_Memberships_CummulatedWithAnonymous.ID_User) AS ID_User
FROM         dbo.view_ApplicationRights LEFT OUTER JOIN
                      dbo.view_Memberships_CummulatedWithAnonymous ON 
                      dbo.view_ApplicationRights.ID_Group = dbo.view_Memberships_CummulatedWithAnonymous.ID_Group
WHERE     (ISNULL(dbo.view_ApplicationRights.ID_User, dbo.view_Memberships_CummulatedWithAnonymous.ID_User) IS NOT NULL)
UNION
SELECT     Applications.Title, Applications.ID AS ID_Application, dbo.Memberships.ID_User
FROM         dbo.Applications CROSS JOIN
                      dbo.Memberships
WHERE     dbo.Memberships.ID_Group = 6
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser_woDups]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[view_ApplicationRights_CommulatedPerUser_woDups]
GO
CREATE view [dbo].[view_ApplicationRights_CommulatedPerUser_woDups]
as
select ID_Application, ID_User
from
(
SELECT     dbo.view_ApplicationRights.Title, dbo.view_ApplicationRights.ID_Application, ISNULL(dbo.view_ApplicationRights.ID_User, 
                      dbo.view_Memberships_CummulatedWithAnonymous.ID_User) AS ID_User
FROM         dbo.view_ApplicationRights LEFT OUTER JOIN
                      dbo.view_Memberships_CummulatedWithAnonymous ON 
                      dbo.view_ApplicationRights.ID_Group = dbo.view_Memberships_CummulatedWithAnonymous.ID_Group
WHERE     (ISNULL(dbo.view_ApplicationRights.ID_User, dbo.view_Memberships_CummulatedWithAnonymous.ID_User) IS NOT NULL)
UNION
SELECT     Applications.Title, Applications.ID AS ID_Application, dbo.Memberships.ID_User
FROM         dbo.Applications CROSS JOIN
                      dbo.Memberships
WHERE     dbo.Memberships.ID_Group = 6
) as AppAuths
group by ID_Application, ID_User

GO

----------------------------------------------------
-- [dbo].[view_Applications]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Applications]
GO
CREATE VIEW dbo.view_Applications
AS
SELECT     dbo.Applications.ID, dbo.Applications.Title, dbo.Applications.ReleasedOn, dbo.Applications.ReleasedBy AS ReleasedByID,
Benutzer_2.Loginname AS ReleasedByLoginname, Benutzer_2.Vorname AS ReleasedByFirstName, Benutzer_2.Nachname AS ReleasedByLastName,
dbo.Applications.LocationID, dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.NavURL,
dbo.Applications.NavFrame, dbo.Applications.LanguageID, dbo.Applications.SystemApp, dbo.Applications.ModifiedOn,
dbo.Applications.ModifiedBy AS ModifiedByID, Benutzer_1.Loginname AS ModifiedByLoginname, Benutzer_1.Vorname AS ModifiedByFirstName,
Benutzer_1.Nachname AS ModifiedByLastName, dbo.Applications.AppDisabled, dbo.Applications.AuthsAsAppID, dbo.Applications.NavTooltipText,
dbo.Applications.IsNew, dbo.Applications.IsUpdated, dbo.Applications.ResetIsNewUpdatedStatusOn, dbo.Applications.Sort,
dbo.Applications.TitleAdminArea, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick,
dbo.Applications.AddLanguageID2URL, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title,
dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded,
dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded,
dbo.Applications.SystemAppType, dbo.Applications.Remarks
FROM         dbo.Applications LEFT OUTER JOIN
dbo.Benutzer Benutzer_1 ON dbo.Applications.ModifiedBy = Benutzer_1.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer_2 ON dbo.Applications.ReleasedBy = Benutzer_2.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_eMailAccounts_of_Groups]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_eMailAccounts_of_Groups]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_eMailAccounts_of_Groups]
GO
CREATE VIEW dbo.view_eMailAccounts_of_Groups
AS
SELECT DISTINCT dbo.Memberships.ID_Group, dbo.Benutzer.[E-MAIL]
FROM         dbo.Memberships LEFT OUTER JOIN
dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Groups]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Groups]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Groups]
GO
CREATE VIEW dbo.view_Groups
AS
SELECT     dbo.Gruppen.ID, dbo.Gruppen.Name, dbo.Gruppen.Description, dbo.Gruppen.ReleasedOn, dbo.Gruppen.ReleasedBy AS ReleasedByID,
Benutzer_1.Loginname AS ReleasedByLoginname, Benutzer_1.Vorname AS ReleasedByFirstName, Benutzer_1.Nachname AS ReleasedByLastName,
dbo.Gruppen.SystemGroup, dbo.Gruppen.ModifiedOn, dbo.Gruppen.ModifiedBy AS ModifiedByID, Benutzer_1.Loginname AS ModifiedByLoginname,
Benutzer_1.Vorname AS ModifiedByFirstName, Benutzer_1.Nachname AS ModifiedByLastName
FROM         dbo.Gruppen LEFT OUTER JOIN
dbo.Benutzer Benutzer_1 ON dbo.Gruppen.ModifiedBy = Benutzer_1.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer_2 ON dbo.Gruppen.ReleasedBy = Benutzer_2.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Languages]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Languages]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Languages]
GO
CREATE VIEW dbo.view_Languages
AS
SELECT     dbo.Languages.*
FROM         dbo.Languages
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_AccessStatistics_Complete - Pre1]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_AccessStatistics_Complete - Pre1]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_AccessStatistics_Complete - Pre1]
GO
CREATE VIEW dbo.[view_Log_AccessStatistics_Complete - Pre1]
AS
SELECT     ServerGroup, Application, COUNT(1) AS [Count], LoginDate, UserID, YEAR(LoginDate) AS LoginYear, MONTH(LoginDate) AS LoginMonth, DAY(LoginDate) 
                      AS LoginDay
FROM         dbo.view_Log_Base4Analysis
WHERE     (ConflictType = 0)
GROUP BY Application, ServerGroup, LoginDate, UserID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_AccessStatistics_Complete - Pre2]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_AccessStatistics_Complete - Pre2]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_AccessStatistics_Complete - Pre2]
GO
CREATE VIEW dbo.[view_Log_AccessStatistics_Complete - Pre2]
AS
SELECT     Min(dbo.system_ServerGroups.ServerGroup) as ServerGroup_Title, Application, SUM([Count]) AS Hits, CAST(LoginYear AS varchar(4)) 
                      + '-' + CASE WHEN LoginMonth < 10 THEN '0' ELSE '' END + CAST(LoginMonth AS varchar(4)) AS LoginDate
FROM         dbo.[view_Log_AccessStatistics_Complete - Pre1] left join dbo.system_servergroups on dbo.[view_Log_AccessStatistics_Complete - Pre1].servergroup = dbo.system_servergroups.id
GROUP BY dbo.[view_Log_AccessStatistics_Complete - Pre1].ServerGroup, Application, CAST(LoginYear AS varchar(4)) + '-' + CASE WHEN LoginMonth < 10 THEN '0' ELSE '' END + CAST(LoginMonth AS varchar(4))
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Base4Analysis]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Base4Analysis]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Base4Analysis]
GO
CREATE VIEW dbo.view_Log_Base4Analysis
AS
SELECT     dbo.[Log].*, CASE WHEN dbo.Applications.TitleAdminArea IS NULL OR
                      dbo.Applications.TitleAdminArea = '' THEN dbo.Applications.Title ELSE dbo.Applications.TitleAdminArea END AS Application, dbo.System_Servers.ServerGroup
FROM         (dbo.[Log] LEFT OUTER JOIN
                      dbo.Applications ON dbo.[Log].ApplicationID = dbo.Applications.ID) LEFT OUTER JOIN System_Servers ON Log.ServerIP = System_Servers.IP
WHERE     (NOT (dbo.Applications.SystemApp = 1)) AND (dbo.[Log].UserID NOT IN
                          (SELECT     id_user
                            FROM          memberships
                            WHERE      dbo.Memberships.ID_Group = 6)) AND (NOT (dbo.[Log].ApplicationID IN
                          (SELECT     id
                            FROM          applications
                            WHERE      systemapp = 1)))
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_Application_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_Application_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_Application_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_Application_Complete
AS
SELECT     ApplicationID, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis
WHERE     (ConflictType = 0)
GROUP BY ApplicationID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_Application_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_Application_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_Application_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_Application_CurrentMonth
AS
SELECT     ApplicationID, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis
WHERE     (ConflictType = 0) AND (LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY ApplicationID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerApplication_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerApplication_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerApplication_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerApplication_Complete
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, Application, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis 
WHERE     (ConflictType = 0)
GROUP BY Application, dbo.view_Log_Base4Analysis.ServerGroup
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerApplication_CurrentMonth
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, Application, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis 
WHERE     (ConflictType = 0) AND (LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY Application, dbo.view_Log_Base4Analysis.ServerGroup
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_User_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_User_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_User_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_User_Complete
AS
SELECT     dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname AS Name,
dbo.Benutzer.[E-MAIL], COUNT(1) AS [Count], dbo.Benutzer.Company
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
dbo.Benutzer ON dbo.view_Log_Base4Analysis.UserID = dbo.Benutzer.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0)
GROUP BY dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname,
dbo.Benutzer.[E-MAIL], dbo.Benutzer.Company
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_User_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_User_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_User_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_User_CurrentMonth
AS
SELECT     dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname AS Name,
dbo.Benutzer.[E-MAIL], COUNT(1) AS [Count], dbo.Benutzer.Company
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
dbo.Benutzer ON dbo.view_Log_Base4Analysis.UserID = dbo.Benutzer.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0) AND (dbo.view_Log_Base4Analysis.LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname,
dbo.Benutzer.[E-MAIL], dbo.Benutzer.Company
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
----------------------------------------------------
-- [dbo].[view_UserList]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_UserList]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_UserList]
GO
CREATE VIEW dbo.view_UserList
AS
SELECT     ID, Loginname, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS Name,
[E-MAIL]
FROM         dbo.Benutzer
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerUserApplication_Complete
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, dbo.view_Log_Base4Analysis.UserID, dbo.view_UserList.Name, dbo.view_Log_Base4Analysis.Application, 
                      COUNT(1) AS [Count], MAX(dbo.view_Log_Base4Analysis.LoginDate) AS LastAccessDate
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
                      dbo.view_UserList ON dbo.view_Log_Base4Analysis.UserID = dbo.view_UserList.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0)
GROUP BY dbo.view_Log_Base4Analysis.ServerGroup, dbo.view_Log_Base4Analysis.Application, dbo.view_Log_Base4Analysis.UserID, dbo.view_UserList.Name
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerUserApplication_CurrentMonth
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, dbo.view_Log_Base4Analysis.UserID, dbo.view_UserList.Name, dbo.view_Log_Base4Analysis.Application, 
                      COUNT(1) AS [Count], MAX(dbo.view_Log_Base4Analysis.LoginDate) AS LastAccessDate
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
                      dbo.view_UserList ON dbo.view_Log_Base4Analysis.UserID = dbo.view_UserList.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0) AND (dbo.view_Log_Base4Analysis.LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY dbo.view_Log_Base4Analysis.ServerGroup, dbo.view_Log_Base4Analysis.Application, dbo.view_UserList.Name, dbo.view_Log_Base4Analysis.UserID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_User_Statistics_LastLogonDates]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_User_Statistics_LastLogonDates]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_User_Statistics_LastLogonDates]
GO
CREATE VIEW dbo.view_User_Statistics_LastLogonDates
AS
SELECT     dbo.Benutzer.LastLoginOn, dbo.view_UserList.ID, dbo.view_UserList.Loginname, dbo.view_UserList.Name, dbo.view_UserList.[E-MAIL]
FROM         dbo.view_UserList INNER JOIN
dbo.Benutzer ON dbo.view_UserList.ID = dbo.Benutzer.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

ALTER view [dbo].[view_Memberships_CummulatedWithAnonymous]
AS
------ OBSOLETE: view doesn't reflect the public (registered) user membership ------
SELECT     ID_Group, ID_User
FROM         dbo.Memberships
UNION
SELECT     58, id
FROM         dbo.benutzer

GO

ALTER VIEW dbo.view_Memberships
AS
SELECT     dbo.Memberships.ID AS ID_Membership, dbo.Gruppen.ID AS ID_Group, dbo.Gruppen.Name, dbo.Gruppen.Description, dbo.Memberships.ReleasedOn,
Benutzer1.ID AS ID_ReleasedBy, Benutzer1.Vorname AS ReleasedByFirstName, Benutzer1.Nachname AS ReleasedByLastName,
dbo.Benutzer.ID AS ID_User, dbo.Benutzer.Loginname, dbo.Benutzer.Vorname, dbo.Benutzer.Nachname, dbo.Benutzer.LoginDisabled, dbo.Benutzer.Company 
FROM         dbo.Memberships LEFT OUTER JOIN
dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer1 ON dbo.Memberships.ReleasedBy = Benutzer1.ID RIGHT OUTER JOIN
dbo.Gruppen ON dbo.Memberships.ID_Group = dbo.Gruppen.ID
