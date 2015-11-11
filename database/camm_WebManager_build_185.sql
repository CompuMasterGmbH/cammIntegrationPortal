---------------------------------------------
-- Add new column RequiredUserProfileFlags --
---------------------------------------------
if not exists (select * from dbo.syscolumns where id = object_id('dbo.Applications_CurrentAndInactiveOnes') and name = 'RequiredUserProfileFlagsRemarks') 
ALTER TABLE dbo.Applications_CurrentAndInactiveOnes ADD
	RequiredUserProfileFlagsRemarks nvarchar(4000) NULL
GO

---------------------------------------------
-- Refresh schema of view by recreating it --
---------------------------------------------
	
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[Applications]') and OBJECTPROPERTY(id, N'IsView') = 1) drop view [dbo].[Applications]
GO
CREATE VIEW dbo.Applications
AS
SELECT     dbo.Applications_CurrentAndInactiveOnes.*
FROM         dbo.Applications_CurrentAndInactiveOnes
WHERE     (AppDeleted = 0)
GO


----------------------------------------------------
-- dbo.AdminPrivate_CloneApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CloneApplication 
	@ReleasedByUserID int,
	@AppID int,
	@CloneType int,
	@CopyDelegates int
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
				                      LocationID, LanguageID, SystemApp, SystemAppType, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, @ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, 0, NULL, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags
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
				                      LocationID, LanguageID, SystemApp, SystemAppType, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, 
						@ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, 0, NULL, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, @AppID As AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL, RequiredUserProfileFlags
				FROM         dbo.Applications
				WHERE    (ID = @AppID)

				SELECT @NewAppID = @@IDENTITY
			END

		If @CopyDelegates = 1
			BEGIN
				INSERT INTO [dbo].[System_SubSecurityAdjustments]([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
				SELECT [UserID], [TableName], @NewAppID, [AuthorizationType]
				FROM [dbo].[System_SubSecurityAdjustments]
				WHERE TableName = 'Applications' AND TablePrimaryIDValue = @AppID
			END
		Else
			BEGIN
				INSERT INTO [dbo].[System_SubSecurityAdjustments]([UserID], [TableName], [TablePrimaryIDValue], [AuthorizationType])
				SELECT [UserID], [TableName], @NewAppID, [AuthorizationType]
				FROM [dbo].[System_SubSecurityAdjustments]
				WHERE TableName = 'Applications' AND TablePrimaryIDValue = @AppID AND NOT ([AuthorizationType] = 'Owner' OR [AuthorizationType] = 'Update' OR [AuthorizationType] = 'UpdateRelations' OR [AuthorizationType] = 'Delete' OR [AuthorizationType] = 'View' OR [AuthorizationType] = 'ViewLogs')
			END
		
		SET NOCOUNT OFF
	
		SELECT Result = @NewAppID
		
	END
Else
	
	SELECT Result = 0

GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[view_ApplicationRights]') and OBJECTPROPERTY(id, N'IsView') = 1)
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
                      SystemApp AS SystemApp, SystemAppType AS SystemAppType, 0 AS DevelopmentTeamMember
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
                      SystemAppType, 0 AS DevelopmentTeamMember
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
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0)
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
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0)
FROM         dbo.Benutzer RIGHT OUTER JOIN
                      dbo.ApplicationsRightsByUser ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson RIGHT OUTER JOIN
                      dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.AuthsAsAppID LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID


GO

