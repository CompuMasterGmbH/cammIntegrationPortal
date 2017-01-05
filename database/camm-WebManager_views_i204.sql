/* Contains all views cumulated in their latest version from build 204 up to latest */

-------------------------------------------------------------------------------------------------
-- CLEAN-UP OF VIEWS NOT IN USE ANY MORE
-------------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[view_Memberships_Effective_wo_PublicNAnonymous]'))
DROP VIEW [dbo].[view_Memberships_Effective_wo_PublicNAnonymous]
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[view_Memberships_Effective_with_PublicNAnonymous]'))
DROP VIEW [dbo].[view_Memberships_Effective_with_PublicNAnonymous]
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Memberships_DenyRulesOnly]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Memberships_DenyRulesOnly]
GO

-------------------------------------------------------------------------------------------------
-- AUTHS CALCULATION QUEUE - VIEWs-CONCEPT (outfading concept)
-------------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_RulesCumulative]'))
DROP VIEW [dbo].[ApplicationsRightsByUser_RulesCumulative]
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_RulesCumulativeWithInherition]'))
DROP VIEW [dbo].[ApplicationsRightsByUser_RulesCumulativeWithInherition]
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_EffectiveCumulative]'))
DROP VIEW [dbo].[ApplicationsRightsByUser_EffectiveCumulative]
GO
CREATE VIEW [dbo].[ApplicationsRightsByUser_EffectiveCumulative]
AS
  SELECT [ID_SecurityObject]
      ,[ID_ServerGroup]
      ,[ID_User]
      ,[IsDevRule]
  FROM [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_RulesCumulative]'))
DROP VIEW [dbo].[ApplicationsRightsByGroup_RulesCumulative]
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_RulesCumulativeWithInherition]'))
DROP VIEW [dbo].[ApplicationsRightsByGroup_RulesCumulativeWithInherition]
GO
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_EffectiveCumulative]'))
DROP VIEW [dbo].[ApplicationsRightsByGroup_EffectiveCumulative]
GO
CREATE VIEW [dbo].[ApplicationsRightsByGroup_EffectiveCumulative]
AS
---------------------------------------------------------------------------------------------------
-- cumulated for all registered users (incl. anonymous auths)
-- AND already added in pre-staging-data: all-server-groups rewritten to real server-group-IDs
-- AND added entries from inherited security objects/applications
--> AND calculated remaining AllowRules after subtraction of DenyRules
-- PLEASE NOTE: anonymous user is a user with a group ID -1 and user ID -1!
---------------------------------------------------------------------------------------------------
    SELECT AllowRules.ID_SecurityObject, AllowRules.ID_ServerGroup, AllowRules.ID_Group, AllowRules.IsDevRule
    FROM dbo.ApplicationsRightsByGroup_PreStaging2Inheritions AS AllowRules
        LEFT JOIN 
            (
                SELECT ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule
                FROM dbo.ApplicationsRightsByGroup_PreStaging2Inheritions 
                WHERE IsDenyRule <> 0
            ) AS DenyRules
            ON AllowRules.ID_SecurityObject = DenyRules.ID_SecurityObject 
                AND AllowRules.ID_ServerGroup = DenyRules.ID_ServerGroup 
                AND AllowRules.ID_Group = DenyRules.ID_Group
                AND AllowRules.IsDevRule = DenyRules.IsDevRule
		INNER JOIN dbo.Applications 
			ON AllowRules.ID_SecurityObject = dbo.Applications.ID
    WHERE AllowRules.IsDenyRule = 0
        AND DenyRules.ID_SecurityObject IS NULL
		AND 
		(
			(
				AllowRules.IsDevRule = 0 
				AND dbo.Applications.AppDisabled = 0
			)
			OR AllowRules.IsDevRule = 1
		)
GO

----------------------------------------------------
-- [dbo].[AdminPrivate_ServerGroupAccessLevels]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ServerGroupAccessLevels]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[AdminPrivate_ServerGroupAccessLevels]
GO
CREATE VIEW dbo.AdminPrivate_ServerGroupAccessLevels

AS
SELECT 	dbo.System_ServerGroupsAndTheirUserAccessLevels.ID, 
		dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup,
		dbo.System_ServerGroupsAndTheirUserAccessLevels.Remarks, 
		dbo.System_AccessLevels.ID AS AccessLevels_ID,
		dbo.System_AccessLevels.Title AS AccessLevels_Title, 
		dbo.System_AccessLevels.Remarks AS AccessLevels_Remarks
FROM dbo.System_AccessLevels 
	INNER JOIN dbo.System_ServerGroupsAndTheirUserAccessLevels 
		ON dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel
GO
----------------------------------------------------
-- [dbo].[AdminPrivate_ServerRelations]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ServerRelations]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[AdminPrivate_ServerRelations]
GO
CREATE VIEW dbo.AdminPrivate_ServerRelations

AS
SELECT     dbo.System_ServerGroups.*, 
	Gruppen_1.ID AS Group_Public_ID, Gruppen_1.Name AS Group_Public_Name, Gruppen_2.ID AS Group_Anonymous_ID,
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
FROM         dbo.System_ServerGroups 
	INNER JOIN dbo.System_Servers System_Servers_1 
			ON dbo.System_ServerGroups.UserAdminServer = System_Servers_1.ID 
	INNER JOIN dbo.Gruppen Gruppen_1 
		ON dbo.System_ServerGroups.ID_Group_Public = Gruppen_1.ID 
	INNER JOIN dbo.Gruppen Gruppen_2 
		ON dbo.System_ServerGroups.ID_Group_Anonymous = Gruppen_2.ID 
	INNER JOIN dbo.System_Servers System_Servers_3 
		ON dbo.System_ServerGroups.MasterServer = System_Servers_3.ID 
	INNER JOIN dbo.System_Servers System_Servers_2 
		ON dbo.System_ServerGroups.ID = System_Servers_2.ServerGroup
GO
----------------------------------------------------
-- [dbo].[Applications]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[Applications]
GO
CREATE VIEW dbo.Applications

AS
SELECT     dbo.Applications_CurrentAndInactiveOnes.*
FROM         dbo.Applications_CurrentAndInactiveOnes
WHERE     (AppDeleted = 0)
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[SecurityObjects]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_CumulatedAuthsPerUser]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[SecurityObjects_CumulatedAuthsPerUser]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[debug_SecurityObjects_CumulatedAuthsPerUser_woDuplicates]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[debug_SecurityObjects_CumulatedAuthsPerUser_woDuplicates]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[debug_CumulatedAuths_OldSchema]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[debug_CumulatedAuths_OldSchema]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[debug_CumulatedAuths_NewSchema]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[debug_CumulatedAuths_NewSchema]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser_FAST]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[view_ApplicationRights_CommulatedPerUser_FAST]
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

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
drop view [dbo].[view_ApplicationRights]
GO

CREATE  VIEW dbo.view_ApplicationRights
AS
-- ApplicationsRightsByGroup directly
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END AS TitleAdminAreaDisplay, dbo.Applications.Level1Title, 
                      dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID AS ID_Application, 
                      ReleaseBenutzer.ID AS AppReleasedByID, ReleaseBenutzer.Vorname AS AppReleasedByVorname, ReleaseBenutzer.Nachname AS AppReleasedByNachname, 
                      Applications.ReleasedOn AS AppReleasedOn, ApplicationsRightsByGroup.ID AS ID_AppRight, NULL AS ID_User, NULL AS LoginDisabled, NULL 
                      AS Loginname, NULL AS Vorname, NULL AS Nachname,null AS [Name1],
                      dbo.ApplicationsRightsByGroup.ID_GroupOrPerson AS ID_Group, Gruppen.Name, Gruppen.Description, 1 AS ItemType, 
                      Applications.AppDisabled AS AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, NULL AS ThisAuthIsFromAppID, 
                      dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL, 
                      SystemApp AS SystemApp, SystemAppType AS SystemAppType, IsNull(ApplicationsRightsByGroup.DevelopmentTeamMember, 0) AS DevelopmentTeamMember, 
					  IsNull(ApplicationsRightsByGroup.IsDenyRule, 0) AS IsDenyRule, ReleaseBenutzer.Company as CompanyName,
					  IsSupervisorAutoAccessRule, dbo.ApplicationsRightsByGroup.ID_ServerGroup
FROM         dbo.Applications 
                      LEFT OUTER JOIN dbo.ApplicationsRightsByGroup ON dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application 
                      LEFT OUTER JOIN dbo.Gruppen ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Gruppen.ID 
                      LEFT OUTER JOIN dbo.Benutzer ReleaseBenutzer ON dbo.Applications.ReleasedBy = ReleaseBenutzer.ID
UNION
-- ApplicationsRightsByGroup indirectly for inheriting apps 
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID AS ID_Application, 
                      ReleaseBenutzer.ID AS AppReleasedByID, ReleaseBenutzer.Vorname AS AppReleasedByVorname, ReleaseBenutzer.Nachname AS AppReleasedByNachname, 
                      Applications.ReleasedOn AS AppReleasedOn, ApplicationsRightsByGroup.ID AS ID_AppRight, NULL AS ID_User, NULL AS LoginDisabled, NULL 
                      AS Loginname, NULL AS Vorname, NULL AS Nachname, null AS [Name1], 
                      dbo.ApplicationsRightsByGroup.ID_GroupOrPerson AS ID_Group, Gruppen.Name, Gruppen.Description, 1 AS ItemType, 
                      Applications.AppDisabled AS AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, Applications.AuthsAsAppID AS ThisAuthIsFromAppID, 
                      dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL, SystemApp, 
                      SystemAppType, IsNull(ApplicationsRightsByGroup.DevelopmentTeamMember, 0), IsNull(ApplicationsRightsByGroup.IsDenyRule, 0), ReleaseBenutzer.Company,
					  IsSupervisorAutoAccessRule, dbo.ApplicationsRightsByGroup.ID_ServerGroup
FROM         dbo.Applications 
					  LEFT OUTER JOIN [dbo].[ApplicationsRights_Inheriting] ON dbo.Applications.ID = [dbo].[ApplicationsRights_Inheriting].ID_Inheriting
                      LEFT OUTER JOIN dbo.ApplicationsRightsByGroup ON [dbo].[ApplicationsRights_Inheriting].ID_Source = dbo.ApplicationsRightsByGroup.ID_Application 
                      LEFT OUTER JOIN dbo.Gruppen ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Gruppen.ID 
                      LEFT OUTER JOIN dbo.Benutzer ReleaseBenutzer ON dbo.Applications.ReleasedBy = ReleaseBenutzer.ID
UNION
-- ApplicationsRightsByUser directly
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID, ReleaseBenutzer.ID, ReleaseBenutzer.Vorname, 
                      ReleaseBenutzer.Nachname, Applications.ReleasedOn, ApplicationsRightsByUser.ID, dbo.ApplicationsRightsByUser.ID_GroupOrPerson, Benutzer.LoginDisabled, Benutzer.Loginname, 
                      Benutzer.Vorname, Benutzer.Nachname,  
					  ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname AS [Name1],
                      NULL, NULL, NULL, 2, Applications.AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, NULL 
                      AS ThisAuthIsFromAppID, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0), 
					  IsNull(ApplicationsRightsByUser.IsDenyRule, 0), ReleaseBenutzer.Company, 
					  0 AS IsSupervisorAutoAccessRule, dbo.ApplicationsRightsByUser.ID_ServerGroup
FROM         dbo.Applications 
                      LEFT OUTER JOIN dbo.ApplicationsRightsByUser ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.ID 
                      LEFT OUTER JOIN dbo.Benutzer ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson 
                      LEFT OUTER JOIN dbo.Benutzer ReleaseBenutzer ON dbo.Applications.ReleasedBy = ReleaseBenutzer.ID
UNION
-- ApplicationsRightsByUser indirectly for inheriting apps
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID, ReleaseBenutzer.ID, ReleaseBenutzer.Vorname, 
                      ReleaseBenutzer.Nachname, Applications.ReleasedOn, ApplicationsRightsByUser.ID, dbo.ApplicationsRightsByUser.ID_GroupOrPerson, Benutzer.LoginDisabled, Benutzer.Loginname, 
                      Benutzer.Vorname, Benutzer.Nachname, 
					  ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname AS [Name1],
                      NULL, NULL, NULL, 2, Applications.AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, 
                      Applications.AuthsAsAppID AS ThisAuthIsFromAppID, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0), 
					  IsNull(ApplicationsRightsByUser.IsDenyRule, 0), ReleaseBenutzer.Company, 
					  0 AS IsSupervisorAutoAccessRule, dbo.ApplicationsRightsByUser.ID_ServerGroup
FROM         dbo.Applications 
					  LEFT OUTER JOIN [dbo].[ApplicationsRights_Inheriting] ON dbo.Applications.ID = [dbo].[ApplicationsRights_Inheriting].ID_Inheriting
                      LEFT OUTER JOIN dbo.ApplicationsRightsByUser ON [dbo].[ApplicationsRights_Inheriting].ID_Source = dbo.ApplicationsRightsByUser.ID_Application 
                      LEFT OUTER JOIN dbo.Benutzer ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson 
                      LEFT OUTER JOIN dbo.Benutzer ReleaseBenutzer ON dbo.Applications.ReleasedBy = ReleaseBenutzer.ID

GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
drop view [dbo].[view_ApplicationRights_CommulatedPerUser]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser_woDups]') and OBJECTPROPERTY(object_id, N'IsView') = 1)
DROP VIEW [dbo].[view_ApplicationRights_CommulatedPerUser_woDups]
GO

----------------------------------------------------
-- [dbo].[view_Applications]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Applications]
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
FROM dbo.Applications 
	LEFT OUTER JOIN dbo.Benutzer Benutzer_1 
		ON dbo.Applications.ModifiedBy = Benutzer_1.ID 
	LEFT OUTER JOIN dbo.Benutzer Benutzer_2 
		ON dbo.Applications.ReleasedBy = Benutzer_2.ID
GO
----------------------------------------------------
-- [dbo].[view_eMailAccounts_of_Groups]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_eMailAccounts_of_Groups]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_eMailAccounts_of_Groups]
GO
CREATE VIEW dbo.view_eMailAccounts_of_Groups

AS
SELECT dbo.Memberships_EffectiveRulesWithClonesNthGrade.ID_Group, dbo.Benutzer.[E-MAIL]
FROM dbo.Memberships_EffectiveRulesWithClonesNthGrade 
	LEFT OUTER JOIN dbo.Benutzer 
		ON dbo.Memberships_EffectiveRulesWithClonesNthGrade.ID_User = dbo.Benutzer.ID
WHERE dbo.Benutzer.[E-MAIL] IS NOT NULL
GROUP BY dbo.Memberships_EffectiveRulesWithClonesNthGrade.ID_Group, dbo.Benutzer.[E-MAIL]
GO
----------------------------------------------------
-- [dbo].[view_Groups]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Groups]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Groups]
GO
CREATE VIEW dbo.view_Groups

AS
SELECT     dbo.Gruppen.ID, dbo.Gruppen.Name, dbo.Gruppen.Description, dbo.Gruppen.ReleasedOn, dbo.Gruppen.ReleasedBy AS ReleasedByID,
	Benutzer_1.Loginname AS ReleasedByLoginname, Benutzer_1.Vorname AS ReleasedByFirstName, Benutzer_1.Nachname AS ReleasedByLastName,
	dbo.Gruppen.SystemGroup, dbo.Gruppen.ModifiedOn, dbo.Gruppen.ModifiedBy AS ModifiedByID, Benutzer_1.Loginname AS ModifiedByLoginname,
	Benutzer_1.Vorname AS ModifiedByFirstName, Benutzer_1.Nachname AS ModifiedByLastName
FROM dbo.Gruppen 
	LEFT OUTER JOIN dbo.Benutzer Benutzer_1 
		ON dbo.Gruppen.ModifiedBy = Benutzer_1.ID 
	LEFT OUTER JOIN dbo.Benutzer Benutzer_2 
		ON dbo.Gruppen.ReleasedBy = Benutzer_2.ID
GO
----------------------------------------------------
-- [dbo].[view_Languages]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Languages]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Languages]
GO
CREATE VIEW dbo.view_Languages

AS
SELECT     dbo.Languages.*
FROM         dbo.Languages
GO
----------------------------------------------------
-- [dbo].[view_Log_AccessStatistics_Complete - Pre1]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_AccessStatistics_Complete - Pre1]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_AccessStatistics_Complete - Pre1]
GO
----------------------------------------------------
-- [dbo].[view_Log_AccessStatistics_Complete - Pre2]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_AccessStatistics_Complete - Pre2]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_AccessStatistics_Complete - Pre2]
GO
----------------------------------------------------
-- [dbo].[view_Log_Base4Analysis]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Base4Analysis]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Base4Analysis]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_Application_Complete]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_Application_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_Application_Complete]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_Application_CurrentMonth]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_Application_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_Application_CurrentMonth]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerApplication_Complete]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerApplication_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_ServerApplication_Complete]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_User_Complete]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_User_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_User_Complete]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_User_CurrentMonth]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_User_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_User_CurrentMonth]
GO
----------------------------------------------------
-- [dbo].[view_UserList]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_UserList]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_UserList]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]
GO
----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]
GO
----------------------------------------------------
-- [dbo].[view_User_Statistics_LastLogonDates]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_User_Statistics_LastLogonDates]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_User_Statistics_LastLogonDates]
GO


if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Memberships_CummulatedWithAnonymous]') and OBJECTPROPERTY(object_id, N'IsView') = 1) 
drop view [dbo].[view_Memberships_CummulatedWithAnonymous]
GO

ALTER VIEW dbo.view_Memberships

AS

-- SPECIAL VIEW FOR ADMINISTRATION PAGES/CONTROLS:
-- shows every group at least with its name an details + NULL values for all other table's columns

-- ATTENTION: NOT INTENDED FOR MODIFICATIONS OF AUTHORISATIONS/MEMBERSHIPS EXCEPT BY THE CWM ADMIN AREA PAGES/CONTROLS !!!

SELECT dbo.Memberships.ID AS ID_Membership, dbo.Memberships.IsDenyRule,
	dbo.Gruppen.ID AS ID_Group, dbo.Gruppen.Name, dbo.Gruppen.Description, 
	dbo.Memberships.ReleasedOn,
	ReleasedByBenutzer.ID AS ID_ReleasedBy, ReleasedByBenutzer.Vorname AS ReleasedByFirstName, ReleasedByBenutzer.Nachname AS ReleasedByLastName,
	dbo.Benutzer.ID AS ID_User, dbo.Benutzer.Loginname, dbo.Benutzer.Vorname, dbo.Benutzer.Nachname, dbo.Benutzer.LoginDisabled, dbo.Benutzer.Company ,
	CAST(0 AS bit) AS IsCloneRule
FROM dbo.Memberships 
	INNER JOIN dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID 
	LEFT OUTER JOIN dbo.Benutzer ReleasedByBenutzer ON dbo.Memberships.ReleasedBy = ReleasedByBenutzer.ID 
	INNER JOIN dbo.Gruppen ON dbo.Memberships.ID_Group = dbo.Gruppen.ID
GO
	