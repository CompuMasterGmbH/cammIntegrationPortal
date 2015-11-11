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
				                      LocationID, LanguageID, SystemApp, SystemAppType, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, @ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, 0, NULL, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, AuthsAsAppID, Sort, 
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
				                      LocationID, LanguageID, SystemApp, SystemAppType, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, 
						@ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, 0, NULL, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, @AppID As AuthsAsAppID, Sort, 
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
-----------------------------------------------------
-- Additional cache table for module "TextModules" --
-----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[TextModulesCache]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[TextModulesCache]
GO
CREATE TABLE [dbo].[TextModulesCache] (
	[PrimaryID] [int] IDENTITY (1, 1) NOT NULL ,
	[MarketID] [int] NOT NULL ,
	[WebsiteAreaID] [nvarchar] (50)  ,
	[ServerGroupID] [int] NOT NULL ,
	[Key] [nvarchar] (100) ,
	[Value] [ntext] ,
	[Title] [nvarchar] (1024) ,
	[TypeID] [int] NOT NULL ,
	CONSTRAINT [PK_TextModulesCache] PRIMARY KEY  CLUSTERED 
	(
		[PrimaryID]
	)    ,
	CONSTRAINT [IX_TextModulesCache] UNIQUE  NONCLUSTERED 
	(
		[ServerGroupID],
		[MarketID],
		[WebsiteAreaID],
		[Key]
	)    
)  
GO