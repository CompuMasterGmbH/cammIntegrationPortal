if exists (select * from sys.objects where object_id = object_id(N'[dbo].[NavItems]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[NavItems]
GO
CREATE TABLE [dbo].[NavItems] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Level1Title] [nvarchar] (512) NULL ,
	[Level2Title] [nvarchar] (512) NULL ,
	[Level3Title] [nvarchar] (512) NULL ,
	[Level4Title] [nvarchar] (512) NULL ,
	[Level5Title] [nvarchar] (512) NULL ,
	[Level6Title] [nvarchar] (512) NULL ,
	[Level1TitleIsHTMLCoded] [bit] NOT NULL ,
	[Level2TitleIsHTMLCoded] [bit] NOT NULL ,
	[Level3TitleIsHTMLCoded] [bit] NOT NULL ,
	[Level4TitleIsHTMLCoded] [bit] NOT NULL ,
	[Level5TitleIsHTMLCoded] [bit] NOT NULL ,
	[Level6TitleIsHTMLCoded] [bit] NOT NULL ,
	[IsNew] [bit] NOT NULL ,
	[IsUpdated] [bit] NOT NULL ,
	[ResetIsNewUpdatedStatusOn] [datetime] NULL ,
	[NavURL] [varchar] (512) NULL ,
	[AddLanguageID2URL] [bit] NOT NULL ,
	[NavFrame] [varchar] (50) NULL ,
	[NavTooltipText] [ntext] NULL ,
	[OnMouseOver] [ntext] NULL ,
	[OnMouseOut] [ntext] NULL ,
	[OnClick] [ntext] NULL ,
	[ServerID] [int] NOT NULL ,
	[LanguageID] [int] NOT NULL ,
	[SystemAppType] [int] NULL ,
	[ModifiedOn] [datetime] NOT NULL CONSTRAINT [DF_NavItems_ModifiedOn] DEFAULT (getdate()),
	[ModifiedBy] [int] NULL ,
	[ReleasedOn] [datetime] NOT NULL CONSTRAINT [DF_NavItems_ReleasedOn] DEFAULT (getdate()),
	[ReleasedBy] [int] NOT NULL ,
	[Sort] [int] NULL CONSTRAINT [DF_NavItems_Sort] DEFAULT (1000000),
	[Disabled] [bit] NOT NULL CONSTRAINT [DF_NavItems_Disabled] DEFAULT (1),
	[Remarks] [ntext] NULL ,
	CONSTRAINT [PK_NavItems] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)   
)  TEXTIMAGE_ON [PRIMARY]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_vs_NavItems]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[SecurityObjects_vs_NavItems]
GO
CREATE TABLE [dbo].[SecurityObjects_vs_NavItems] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[ID_SecurityObject] [int] NOT NULL ,
	[ID_NavItem] [int] NOT NULL ,
	CONSTRAINT [PK_SecurityObjects_vs_NavItems] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)   
) 
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_CurrentAndInactiveOnes]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[SecurityObjects_CurrentAndInactiveOnes]
GO
CREATE TABLE [dbo].[SecurityObjects_CurrentAndInactiveOnes] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Title] [varchar] (255) NULL ,
	[TitleAdminArea] [nvarchar] (255) NULL ,
	[ReleasedOn] [datetime] NOT NULL CONSTRAINT [DF_SecurityObjects_ReleasedOn] DEFAULT (getdate()),
	[ReleasedBy] [int] NOT NULL ,
	[ModifiedOn] [datetime] NULL CONSTRAINT [DF_SecurityObjects_ModifiedOn] DEFAULT (getdate()),
	[ModifiedBy] [int] NULL ,
	[Disabled] [bit] NOT NULL CONSTRAINT [DF_SecurityObjects_Disabled] DEFAULT (1),
	[AuthsAsSecObjID] [int] NULL ,
	[Deleted] [bit] NOT NULL CONSTRAINT [DF_SecurityObjects_Deleted] DEFAULT (0),
	[SystemAppType] [int] NULL ,
	[Remarks] [ntext] NULL ,
	CONSTRAINT [PK_SecurityObjects] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)   ,
	CONSTRAINT [IX_SecurityObjects] UNIQUE  NONCLUSTERED 
	(
		[Title]
	)   
)  TEXTIMAGE_ON [PRIMARY]
GO

CREATE  INDEX [IX_NavItems] ON [dbo].[NavItems]([ID], [LanguageID])  
CREATE  INDEX [IX_NavItems_1] ON [dbo].[NavItems]([ID], [ServerID])  
CREATE  INDEX [IX_SecurityObjects_vs_NavItems] ON [dbo].[SecurityObjects_vs_NavItems]([ID_SecurityObject])  
CREATE  INDEX [IX_SecurityObjects_vs_NavItems_1] ON [dbo].[SecurityObjects_vs_NavItems]([ID_NavItem])  
CREATE  INDEX [IX_SecurityObjects_1] ON [dbo].[SecurityObjects_CurrentAndInactiveOnes]([AuthsAsSecObjID])  
CREATE  INDEX [IX_SecurityObjects_2] ON [dbo].[SecurityObjects_CurrentAndInactiveOnes]([SystemAppType])  
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_AuthsByGroup]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[SecurityObjects_AuthsByGroup]
GO
CREATE TABLE [dbo].[SecurityObjects_AuthsByGroup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[ReleasedOn] [datetime] NOT NULL,
	[ReleasedBy] [int] NOT NULL,
 CONSTRAINT [PK_SecurityObjects_AuthsByGroup] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) 
GO
ALTER TABLE [dbo].[SecurityObjects_AuthsByGroup] ADD DEFAULT (getdate()) FOR [ReleasedOn]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_AuthsByUser]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[SecurityObjects_AuthsByUser]
GO
CREATE TABLE [dbo].[SecurityObjects_AuthsByUser](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[ReleasedOn] [datetime] NOT NULL,
	[ReleasedBy] [int] NOT NULL,
	[DevelopmentTeamMember] [bit] NOT NULL,
 CONSTRAINT [PK_SecurityObjects_AuthsByUser] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) 
GO
ALTER TABLE [dbo].[SecurityObjects_AuthsByUser] ADD  DEFAULT (getdate()) FOR [ReleasedOn]
GO
ALTER TABLE [dbo].[SecurityObjects_AuthsByUser] ADD  DEFAULT (0) FOR [DevelopmentTeamMember]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Apps2SecObj_SyncWarnLog]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Apps2SecObj_SyncWarnLog]
GO
CREATE TABLE [dbo].[Apps2SecObj_SyncWarnLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ConflictDescription] [ntext] NOT NULL,
 CONSTRAINT [PK_Apps2SecObj_SyncWarnLog] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) 

GO
-- New VIEWS --
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

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[RefillSplittedSecObjAndNavPointsTables]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.RefillSplittedSecObjAndNavPointsTables
GO
CREATE PROCEDURE dbo.RefillSplittedSecObjAndNavPointsTables
AS
BEGIN

TRUNCATE TABLE dbo.SecurityObjects_CurrentAndInactiveOnes
TRUNCATE TABLE dbo.NavItems
TRUNCATE TABLE dbo.SecurityObjects_vs_NavItems
TRUNCATE TABLE dbo.SecurityObjects_AuthsByGroup
TRUNCATE TABLE dbo.SecurityObjects_AuthsByUser
TRUNCATE TABLE dbo.Apps2SecObj_SyncWarnLog

-- AppSecurityObjects übernehmen
INSERT INTO dbo.SecurityObjects_CurrentAndInactiveOnes (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Remarks, SystemAppType, Deleted, AuthsAsSecObjID, [Disabled], ModifiedBy, ModifiedOn)
SELECT     Title, MAX(TitleAdminArea) AS Expr1, MAX(ReleasedOn) AS Expr2, MIN(ReleasedBy) AS Expr3, MAX(Remarks) AS Expr4, MAX(SystemAppType) 
                      AS Expr5, MIN(Case when AppDeleted <> 0 then 1 else 0 end) AS Expr7, NULL AS Expr8, MIN(case when AppDisabled <> 0 then 1 else 0 end) AS Expr9, MIN(ModifiedBy) AS Expr11, 
                      IsNull(MAX(ModifiedOn), MAX(ReleasedOn)) AS Expr10
FROM         dbo.Applications
GROUP BY Title

-- AppNavItems übernehmen
INSERT INTO dbo.NavItems
                      (Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, ServerID, 
                      LanguageID, SystemAppType, Remarks, ReleasedOn, ReleasedBy, ModifiedOn, ModifiedBy, Disabled, Sort, ResetIsNewUpdatedStatusOn,
                       OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, 
                      Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded)
SELECT     Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, LocationID, 
                      LanguageID, SystemAppType, Remarks, ReleasedOn, ReleasedBy, IsNull(ModifiedOn, ReleasedOn), ModifiedBy, AppDisabled, Sort, 
                      ResetIsNewUpdatedStatusOn, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded
FROM         dbo.Applications
WHERE     (AppDeleted = 0)

-- Verknüpfungen zwischen SecurityObjects und NavItems aufbauen
INSERT INTO dbo.SecurityObjects_vs_NavItems (ID_SecurityObject, ID_NavItem)
SELECT dbo.SecurityObjects_CurrentAndInactiveOnes.ID as SecID, dbo.NavItems.ID as NavID
FROM dbo.Applications
	LEFT JOIN dbo.SecurityObjects_CurrentAndInactiveOnes ON dbo.Applications.Title = dbo.SecurityObjects_CurrentAndInactiveOnes.Title
	LEFT JOIN dbo.NavItems ON
		ISNULL(dbo.Applications.Level1Title, '') = ISNULL(dbo.NavItems.Level1Title, '') AND 
		ISNULL(dbo.Applications.Level2Title, '') = ISNULL(dbo.NavItems.Level2Title, '') AND 
		ISNULL(dbo.Applications.Level3Title, '') = ISNULL(dbo.NavItems.Level3Title, '') AND 
		ISNULL(dbo.Applications.Level4Title, '') = ISNULL(dbo.NavItems.Level4Title, '') AND 
		ISNULL(dbo.Applications.Level5Title, '') = ISNULL(dbo.NavItems.Level5Title, '') AND 
		ISNULL(dbo.Applications.Level6Title, '') = ISNULL(dbo.NavItems.Level6Title, '') AND 
		dbo.Applications.NavURL = dbo.NavItems.NavURL AND
		dbo.Applications.LanguageID = dbo.NavItems.LanguageID AND
		dbo.Applications.LocationID = dbo.NavItems.ServerID
WHERE dbo.NavItems.ID Is Not NULL
GROUP BY dbo.SecurityObjects_CurrentAndInactiveOnes.ID, dbo.NavItems.ID

-- Zusammenstellung der Applikationsvererbungen, welche über die Title-Grenzen hinweg gehen
CREATE TABLE #AuthInheritionsBetweenDifferentApps
(
        InheritingID int,
        InheritingTitle varchar(255),
        InheritedFromID int,
        InheritedFromTitle varchar(255)
)
;
INSERT INTO #AuthInheritionsBetweenDifferentApps
(
        InheritingID,
        InheritingTitle,
        InheritedFromID,
        InheritedFromTitle
)
SELECT     dbo.Applications.ID AS InheritingID, dbo.Applications.Title AS InheritingTitle, 
                      ApplicationsInheritFrom.ID AS InheritedFromID, ApplicationsInheritFrom.Title AS InheritedFromTitle
FROM         dbo.Applications INNER JOIN
                      dbo.Applications ApplicationsInheritFrom ON 
                      dbo.Applications.AuthsAsAppID = ApplicationsInheritFrom.ID AND 
                      dbo.Applications.Title <> ApplicationsInheritFrom.Title

-- Berechtigungsvererbung zwischen Anwendungen unterschiedlicher ApplicationTitles wiederherstellen
UPDATE dbo.SecurityObjects
SET dbo.SecurityObjects.AuthsAsSecObjID = secobj_by_authsasSecObjid.ID
FROM dbo.SecurityObjects 
	INNER JOIN dbo.Applications ON dbo.SecurityObjects.Title = dbo.Applications.Title
	INNER JOIN dbo.Applications apps_by_authsasappid ON Applications.AuthsAsAppID = apps_by_authsasappid.ID
	INNER JOIN dbo.SecurityObjects secobj_by_authsasSecObjid ON apps_by_authsasappid.Title = secobj_by_authsasSecObjid.Title
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Gruppenberechtigungen neu aufbauen - Apps ohne Vererbung
INSERT INTO SecurityObjects_AuthsByGroup (ID_SecurityObject, ID_Group, ReleasedOn, ReleasedBy, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByGroup.ID_GroupOrPerson, dbo.ApplicationsRightsByGroup.ReleasedOn, dbo.ApplicationsRightsByGroup.ReleasedBy,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Null

-- Gruppenberechtigungen neu aufbauen - Apps mit Vererbung - Gleicher AppTitle
INSERT INTO SecurityObjects_AuthsByGroup (ID_SecurityObject, ID_Group, ReleasedOn, ReleasedBy, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByGroup.ID_GroupOrPerson, dbo.ApplicationsRightsByGroup.ReleasedOn, dbo.ApplicationsRightsByGroup.ReleasedBy,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID Not IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Gruppenberechtigungen neu aufbauen - Apps mit Vererbung - Unterschiedlicher AppTitle
INSERT INTO SecurityObjects_AuthsByGroup (ID_SecurityObject, ID_Group, ReleasedOn, ReleasedBy, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByGroup.ID_GroupOrPerson, dbo.ApplicationsRightsByGroup.ReleasedOn, dbo.ApplicationsRightsByGroup.ReleasedBy,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.Applications apps_by_authsasappid ON Applications.AuthsAsAppID = apps_by_authsasappid.ID
	INNER JOIN dbo.SecurityObjects ON apps_by_authsasappid.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Benutzerberechtigungen neu aufbauen - Apps ohne Vererbung
INSERT INTO SecurityObjects_AuthsByUser (ID_SecurityObject, ID_User, ReleasedOn, ReleasedBy, DevelopmentTeamMember, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByUser.ID_GroupOrPerson, 
	dbo.ApplicationsRightsByUser.ReleasedOn, 
	dbo.ApplicationsRightsByUser.ReleasedBy, 
	dbo.ApplicationsRightsByUser.DevelopmentTeamMember,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByUser ON dbo.Applications.ID = dbo.ApplicationsRightsByUser.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Null

-- Benutzerberechtigungen neu aufbauen - Apps mit Vererbung - Gleicher AppTitle
INSERT INTO SecurityObjects_AuthsByUser (ID_SecurityObject, ID_User, ReleasedOn, ReleasedBy, DevelopmentTeamMember, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByUser.ID_GroupOrPerson, dbo.ApplicationsRightsByUser.ReleasedOn, dbo.ApplicationsRightsByUser.ReleasedBy, dbo.ApplicationsRightsByUser.DevelopmentTeamMember,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.SecurityObjects ON dbo.Applications.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByUser ON dbo.Applications.ID = dbo.ApplicationsRightsByUser.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID Not IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

-- Benutzerberechtigungen neu aufbauen - Apps mit Vererbung - Unterschiedlicher AppTitle
INSERT INTO SecurityObjects_AuthsByUser (ID_SecurityObject, ID_User, ReleasedOn, ReleasedBy, DevelopmentTeamMember, ID_ServerGroup)
SELECT dbo.SecurityObjects.ID As ID_Application, 
	dbo.ApplicationsRightsByUser.ID_GroupOrPerson, dbo.ApplicationsRightsByUser.ReleasedOn, dbo.ApplicationsRightsByUser.ReleasedBy, dbo.ApplicationsRightsByUser.DevelopmentTeamMember,
	dbo.System_Servers.ServerGroup
FROM dbo.Applications 
	INNER JOIN dbo.Applications apps_by_authsasappid ON Applications.AuthsAsAppID = apps_by_authsasappid.ID
	INNER JOIN dbo.SecurityObjects ON apps_by_authsasappid.Title = dbo.SecurityObjects.Title
	INNER JOIN dbo.ApplicationsRightsByUser ON dbo.Applications.ID = dbo.ApplicationsRightsByUser.ID_Application
	INNER JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
WHERE dbo.Applications.AuthsAsAppID Is Not Null AND dbo.Applications.ID IN (SELECT InheritingID FROM #AuthInheritionsBetweenDifferentApps)

/*
-- Warnungen protokollieren bei Auftreten von mehreren gleichen AppTitles mit unterschiedlichen Vererbungsinformationen
INSERT INTO dbo.Apps2SecObj_SyncWarnLog (ConflictDescription)
SELECT 'Authorizations of application "' + InheritingTitle + '" (ID ' + Cast(InheritingID as varchar(30)) + ') have been transferred to application "' + InheritedFromTitle + '" (ID ' + Cast(InheritedFromID as varchar(30)) + ') because application "' + InheritingTitle + '" (ID ' + Cast(InheritingID as varchar(30)) + ') inherits all authorizations from application "' + InheritedFromTitle + '" (ID ' + Cast(InheritedFromID as varchar(30)) + '). Please check the current inheritions between these two applications.'
FROM #AuthInheritionsBetweenDifferentApps

INSERT INTO dbo.Apps2SecObj_SyncWarnLog (ConflictDescription)
SELECT 'The old application ID ' + cast(dbo.Applications.ID as varchar(20)) + ' and title "' + dbo.Applications.Title + '" had set up different applications to inherit from. One of these is application ID ' + cast(isnull(apps_auths_as_appid.ID,0) as varchar(20)) + ' and title "' + IsNull(apps_auths_as_appid.Title, '(no application)') + '". Please check the current inheritions between these two applications.'
FROM #AuthInheritionsBetweenDifferentApps 
	INNER JOIN dbo.Applications ON #AuthInheritionsBetweenDifferentApps.InheritingTitle = dbo.Applications.Title
	LEFT JOIN dbo.Applications apps_auths_as_appid ON dbo.Applications.AuthsAsAppID = apps_auths_as_appid.ID

INSERT INTO dbo.Apps2SecObj_SyncWarnLog (ConflictDescription)
VALUES ('Authorizations of different applications with the same application title have been collected and summarized into one application security object.')
*/

-- Aufräumarbeiten
DROP TABLE #AuthInheritionsBetweenDifferentApps

END
GO
EXEC RefillSplittedSecObjAndNavPointsTables

GO
-- New VIEWS --
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

-- New VIEWS --
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
