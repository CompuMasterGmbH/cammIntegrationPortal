/*  Contains all functions and stored procedures cumulated in their latest version */

----------------------------------------------------
-- dbo.AdminPrivate_CloneApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CloneApplication 
	@ReleasedByUserID int,
	@AppID int,
	@CloneType int,
	@CopyDelegates int

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

				SELECT @NewAppID = SCOPE_IDENTITY()

				-- Add Group Authorizations
				INSERT INTO dbo.ApplicationsRightsByGroup
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application, [DevelopmentTeamMember], [IsDenyRule])
				SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @NewAppID, [DevelopmentTeamMember], [IsDenyRule] AS ID_Application
				FROM         dbo.ApplicationsRightsByGroup
				WHERE     ID_Application = @AppID AND IsSupervisorAutoAccessRule = 0

				-- Add User Authorizations
				INSERT INTO dbo.ApplicationsRightsByUser
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application, [DevelopmentTeamMember], [IsDenyRule])
				SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @NewAppID, [DevelopmentTeamMember], [IsDenyRule] AS ID_Application
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

				SELECT @NewAppID = SCOPE_IDENTITY()
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
				WHERE TableName = 'Applications' AND TablePrimaryIDValue = @AppID AND [AuthorizationType] = 'ResponsibleContact'
			END
		
		SET NOCOUNT OFF
	
		SELECT Result = @NewAppID
		
	END
Else
	
	SELECT Result = 0

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateAccessLevel 
	@ReleasedByUserID int,
	@Title nvarchar(50)

AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
		BEGIN
		SET NOCOUNT ON
		INSERT INTO dbo.System_AccessLevels (Title, ReleasedBy, ModifiedBy) VALUES (@Title, @ReleasedByUserID, @ReleasedByUserID)
		SET NOCOUNT OFF

		SELECT Result = SCOPE_IDENTITY()
		
	END
Else
	
	SELECT Result = 0


GO

---------------------------------------------------
-- dbo.AdminPrivate_CreateAdminServerNavPoints
----------------------------------------------------
-- TAKES PLACE AT SEPARATE camm_WebManager_navitems*.sql: ALTER PROCEDURE dbo.AdminPrivate_CreateAdminServerNavPoints
GO


----------------------------------------------------
-- dbo.AdminPrivate_CreateApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplication 
(
	@ReleasedByUserID int,
	@Title varchar(255)
)

AS
DECLARE @CurUserID int
DECLARE @NewAppID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		SET NOCOUNT ON
		INSERT INTO dbo.Applications (Title, ReleasedBy, ModifiedBy, LanguageID, LocationID) VALUES (@Title, @ReleasedByUserID, @ReleasedByUserID, 0, 0)

		SELECT @NewAppID = SCOPE_IDENTITY()

		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Applications', @NewAppID, 'Owner', @ReleasedByUserID

		SET NOCOUNT OFF
		SELECT Result = @NewAppID
	END
Else
	
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByGroup 
	@ReleasedByUserID int,
	@AppID int,
	@GroupID int,
	@ServerGroupID int = 0,
	@IsDevelopmentTeamMember bit = 0,
	@IsDenyRule bit = 0

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END


SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 1
	RETURN 1
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForAuthorizations 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Password validation and update
If @CurUserID Is Not Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application, ID_GroupOrPerson, ReleasedBy, DevelopmentTeamMember, IsDenyRule, ID_ServerGroup) 
		VALUES (@AppID, @GroupID, @ReleasedByUserID, @IsDevelopmentTeamMember, @IsDenyRule, @ServerGroupID)
		EXEC Int_LogAuthChanges @CurUserID, @GroupID, @AppID, @ReleasedByUserID, @IsDevelopmentTeamMember, @IsDenyRule, @ServerGroupID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
	END
Else
	-- Rückgabewert
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 0
	END

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByUser 
	@ReleasedByUserID int,
	@AppID int,
	@UserID int,
	@ServerGroupID int = 0,
	@IsDevelopmentTeamMember bit = 0,
	@IsDenyRule bit = 0

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @SAPFlagName varchar(50)
DECLARE @SAPFlagValue nvarchar(255)
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForAuthorizations 'UpdateRelations', @ReleasedByUserID, @AppID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Check if the given user really exists
SELECT @UserID = (select ID from dbo.Benutzer where id = @UserID)
If @UserID Is Null
	-- Rückgabewert
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 5
		Return 5
	END

SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SELECT @SAPFlagName = substring(navurl, charindex('|2|&', NavUrl)+4, charindex('|3|', NavUrl) - (charindex('|2|&', NavUrl)+4)) FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
	SELECT @SAPFlagValue = [Value] FROM Log_Users WHERE ID_User = @UserID AND Type = @SAPFlagName
	If @SAPFlagValue Is Null
		BEGIN
		SET NOCOUNT OFF
		SELECT Result = 2, @SAPFlagName as SAPFlagName
		RETURN 2
		END
	END

-- User validation and update
If @CurUserID Is Not Null
	-- Validation successfull, authorization will be saved now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByUser 
			(ID_Application, ID_GroupOrPerson, ReleasedBy, DevelopmentTeamMember, IsDenyRule, ID_ServerGroup) 
		VALUES 
			(@AppID, @UserID, @ReleasedByUserID, @IsDevelopmentTeamMember, @IsDenyRule, @ServerGroupID)
		EXEC Int_LogAuthChanges @UserID, Null, @AppID, @ReleasedByUserID, @IsDevelopmentTeamMember, @IsDenyRule, @ServerGroupID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		Return -1
	END
ELSE
	-- Releasing user is not authorized to administer
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END

GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByGroup]
(
	@AuthID int,
	@ReleasedByUserID int
)

AS 

declare @groupID int
declare @AppID int
declare @IsDevelopmentTeamMember bit
declare @IsDenyRule bit
declare @ServerGroupID int
select top 1 @groupid = id_grouporperson, @appid = id_application, @IsDevelopmentTeamMember = DevelopmentTeamMember, @IsDenyRule = IsDenyRule, @ServerGroupID = ID_ServerGroup from dbo.ApplicationsRightsByGroup where id = @AuthID

EXEC Int_LogAuthChanges NULL, @GroupID, @AppID, @ReleasedByUserID, @IsDevelopmentTeamMember, @IsDenyRule, @ServerGroupID 
DELETE FROM dbo.ApplicationsRightsByGroup WHERE     (ID_GroupOrPerson IS NOT NULL) AND ID=@AuthID

GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByUser]
(
	@AuthID int,
	@ReleasedByUserID int = NULL
)

AS
declare @UserID int
declare @AppID int
declare @IsDevelopmentTeamMember bit
declare @IsDenyRule bit
declare @ServerGroupID int
select top 1 @userid = id_grouporperson, @appid = id_application, @IsDevelopmentTeamMember = DevelopmentTeamMember, @IsDenyRule = IsDenyRule, @ServerGroupID = ID_ServerGroup from dbo.ApplicationsRightsByUser where id = @AuthID

EXEC Int_LogAuthChanges @UserID, Null, @AppID, @ReleasedByUserID, @IsDevelopmentTeamMember, @IsDenyRule, @ServerGroupID 
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson Is Not Null And ID=@AuthID
GO

IF  EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AdminPrivate_DeleteMemberships]
GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteMemberships
----------------------------------------------------
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteMemberships]
(
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int,
	@IsDenyRule bit = 0
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForMemberships 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Is releasing user a valid user ID?
If @CurUserID Is Not Null
	-- Validation successfull, membership will be updated now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Record update
		DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID AND IsDenyRule = @IsDenyRule AND IsSystemRuleOfServerGroupsAndTheirUserAccessLevelsID IS NULL AND IsSystemRuleOfServerGroupID IS NULL
		-- log group membership change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', NULL, -12, cast(@GroupID as nvarchar(50)) + N'|' + cast(@IsDenyRule as nvarchar(5)))
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateGroup
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_CreateGroup] 
(
	@ReleasedByUserID int,
	@Name nvarchar(100),
	@Description nvarchar(1024)
)

AS
DECLARE @CurUserID int
DECLARE @NewGroupID int
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	BEGIN
		INSERT INTO dbo.Gruppen (Name, Description, ReleasedBy, ModifiedBy) VALUES (@Name, @Description, @ReleasedByUserID, @ReleasedByUserID)
		SELECT @NewGroupID = SCOPE_IDENTITY()
		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Groups', @NewGroupID, 'Owner', @ReleasedByUserID
		SELECT Result = @NewGroupID 
	END
Else
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateMasterServerNavPoints
----------------------------------------------------

ALTER PROCEDURE dbo.AdminPrivate_CreateMasterServerNavPoints
	(
		@NewServerID int,
		@OldServerID int,
		@ModifiedBy int
	)

AS

-- Removed functionality
-- Now in WebManager DLL

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateMemberships
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMemberships 
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int,
	@IsDenyRule bit = 0

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForMemberships 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID
SELECT @MemberShipID = ID FROM dbo.Memberships WHERE ID_Group = @GroupID And ID_User = @UserID AND IsDenyRule = @IsDenyRule
If @MemberShipID Is Null
	BEGIN
		-- Is releasing user a valid user ID?
		If @CurUserID Is Not Null
			-- Validation successfull, membership will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Record update
				INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedBy, IsDenyRule) VALUES (@GroupID, @UserID, @ReleasedByUserID, @IsDenyRule)
				-- log group membership change
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
				values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', NULL, -11, cast(@GroupID as nvarchar(50)) + N'|' + cast(@IsDenyRule as nvarchar(5)))
			END
		Else
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	SELECT Result = -1
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateServer
	(
		@ServerIP varchar(32),
		@ServerGroup int
	)

AS

declare @NewServerID int

SET NOCOUNT ON

-- Create server
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, @ServerIP AS IP, 'Secured server' AS ServerDescription, @ServerGroup AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout

-- Get new server ID
SELECT @NewServerID = SCOPE_IDENTITY()

-- Check script engines
EXEC AdminPrivate_SetScriptEngineActivation 0, @NewServerID, 0, 1

-- Return new server ID
SET NOCOUNT OFF
SELECT     @NewServerID
RETURN     @NewServerID

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateServerGroup
(
@GroupName nvarchar(255),
@email_Developer nvarchar(255),
@UserID_Creator int
)

AS 

DECLARE @ID_ServerGroup int
DECLARE @ID_AdminServer int
DECLARE @ID_MasterServer int
DECLARE @ID_Group_Public int
DECLARE @ID_Group_Anonymous int
DECLARE @Group_Public_Name nvarchar(255)
DECLARE @Group_Anonymous_Name nvarchar(255)

SET NOCOUNT ON

SELECT @Group_Public_Name = 'Public ' + SubString(@GroupName, 1, 245)
SELECT @Group_Anonymous_Name = 'Anonymous ' + SubString(@GroupName, 1, 242)
SELECT @ID_AdminServer = (SELECT TOP 1 UserAdminServer FROM System_ServerGroups)
SELECT @ID_ServerGroup = (SELECT ID FROM System_ServerGroups WHERE ServerGroup = @GroupName)
SELECT @ID_Group_Public = (SELECT ID FROM Gruppen WHERE Name = @Group_Public_Name)
SELECT @ID_Group_Anonymous = (SELECT ID FROM Gruppen WHERE Name = @Group_Anonymous_Name)
SELECT @ID_MasterServer = (SELECT TOP 1 ID FROM System_Servers WHERE IP = 'secured.yourcompany.com')

-- Erstellbarkeit gewährleisten
IF @ID_ServerGroup Is Not Null 
	BEGIN
		RAISERROR ('Server group already exists', 16, 1)
		RETURN	
	END	
IF @ID_Group_Public Is Not Null
	BEGIN
		RAISERROR ('The server group cannot be created because the public group already exists.', 16, 2)
		RETURN	
	END	
IF @ID_Group_Anonymous Is Not Null
	BEGIN
		RAISERROR ('The server group cannot be created because the anonymous group already exists.', 16, 2)
		RETURN	
	END	
IF @ID_MasterServer Is Not Null 
	BEGIN
		RAISERROR ('There is already a server called "secured.yourcompany.com". Please rename it first before creating new server groups.', 16, 3)
		RETURN	
	END	

BEGIN TRANSACTION

SELECT @ID_Group_Anonymous = Null, @ID_Group_Public = Null, @ID_MasterServer = Null, @ID_ServerGroup = Null

-- Public Group anlegen
INSERT INTO dbo.Gruppen
                      (Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy)
SELECT     @Group_Public_Name AS Name, 'System group: all users logged on' AS ServerDescription, GETDATE() AS ReleasedOn, @UserID_Creator AS ReleasedBy, 
                      1 AS SystemGroup, GETDATE() AS ModifiedOn, @UserID_Creator AS ModifiedBy
SELECT @ID_Group_Public = SCOPE_IDENTITY()

-- Anonymous Group anlegen
INSERT INTO dbo.Gruppen
                      (Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy)
SELECT     @Group_Anonymous_Name AS Name, 'System group: all anonymous users (without being logged on)' AS ServerDescription, GETDATE() AS ReleasedOn, @UserID_Creator AS ReleasedBy, 
                      1 AS SystemGroup, GETDATE() AS ModifiedOn, @UserID_Creator AS ModifiedBy
SELECT @ID_Group_Anonymous = SCOPE_IDENTITY()

-- Public Group Security Adjustments
INSERT INTO [dbo].[System_SubSecurityAdjustments] (UserID, TableName, TablePrimaryIDValue, AuthorizationType)
VALUES (@UserID_Creator, 'Groups', @ID_Group_Public, 'Owner')
INSERT INTO [dbo].[System_SubSecurityAdjustments] (UserID, TableName, TablePrimaryIDValue, AuthorizationType)
VALUES (@UserID_Creator, 'Groups', @ID_Group_Anonymous, 'Owner')

-- Neuen Server anlegen, welcher als MasterServer fungieren soll
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, 'secured.yourcompany.com' AS IP, 'Secured server' AS ServerDescription, 0 AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout
SELECT @ID_MasterServer = SCOPE_IDENTITY()

-- Check script engines
EXEC AdminPrivate_SetScriptEngineActivation 0, @ID_MasterServer, 0, 1

-- ServerGroup anlegen
INSERT INTO dbo.System_ServerGroups
                      (ServerGroup, ID_Group_Public, ID_Group_Anonymous, MasterServer, UserAdminServer, AreaImage, AreaButton, AreaNavTitle, 
                      AreaCompanyFormerTitle, AreaCompanyTitle, AreaSecurityContactEMail, AreaSecurityContactTitle, AreaDevelopmentContactEMail, 
                      AreaDevelopmentContactTitle, AreaContentManagementContactEMail, AreaContentManagementContactTitle, AreaUnspecifiedContactEMail, 
                      AreaUnspecifiedContactTitle, AreaCopyRightSinceYear, AreaCompanyWebSiteURL, AreaCompanyWebSiteTitle, ModifiedBy)
SELECT     @GroupName AS Expr21, @ID_Group_Public AS Expr19, @ID_Group_Anonymous AS Expr20, @ID_MasterServer AS Expr18, @ID_AdminServer AS Expr17, 
                      '/sysdata/images/global/logo_csa.jpg' AS Expr1, '/sysdata/images/global/button_csa.gif' AS Expr2, NULL AS Expr3, 'YourCompany Ltd.' AS Expr4, 'YourCompany' AS Expr5, @email_Developer AS Expr6, 
                      @email_Developer AS Expr7, @email_Developer AS Expr8, @email_Developer AS Expr9, @email_Developer AS Expr10, 
                      @email_Developer AS Expr11, @email_Developer AS Expr12, @email_Developer AS Expr13, DATEPART(yyyy, GETDATE()) AS Expr14, 
                      'http://www.yourcompany.com/' AS Expr15, 'YourCompany Homepage' AS Expr16, @UserID_Creator as ModifiedBy
SELECT @ID_ServerGroup = SCOPE_IDENTITY()

-- Master Server in die ServerGroup aufnehmen
UPDATE dbo.System_Servers SET ServerGroup = @ID_ServerGroup WHERE ID = @ID_MasterServer

IF @ID_Group_Anonymous Is Null OR @ID_Group_Public Is Null OR @ID_MasterServer Is Null OR @ID_ServerGroup Is Null
	ROLLBACK TRANSACTION
ELSE
	COMMIT TRANSACTION

-- Create master server navigation
EXEC AdminPrivate_CreateMasterServerNavPoints @ID_MasterServer, Null, @UserID_Creator

SET NOCOUNT OFF

SELECT @ID_ServerGroup
Return @ID_ServerGroup

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateUserAccount
(
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@WebApplication varchar(1024),
	@ServerIP nvarchar(32),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability tinyint,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @ServerGroupID int
DECLARE @ServerID int
DECLARE @WebAppID int

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @ServerGroupID = dbo.System_ServerGroups.ID,
	@ServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers 
	INNER JOIN dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
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
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @ServerID)))


-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
		-- Logging
		If IsNull(@IsUserChange, 0) = 0 
			-- admin
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, '0.0.0.0', @WebAppID, 3, 'Account ' + @Username + ' created by admin with user id ' + CAST(@ModifiedBy as varchar(20)))
		Else
			-- user change
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, '0.0.0.0', @WebAppID, 1, 'Account ' + @Username + ' created by user itself')
		SET NOCOUNT OFF
		-- Rückgabewert
		If @CurUserID Is Not Null 
			SELECT Result = -1 
		Else
			SELECT Result = -2
	END

Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID, @ModifiedBy
GO


----------------------------------------------------
-- dbo.AdminPrivate_DeleteAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteAccessLevel 
	@ID int,
	@JustAnotherAccessLevel int = Null

AS

-- If no replacement ID is given then search for a random one
If @JustAnotherAccessLevel Is Null
	SELECT TOP 1 @JustAnotherAccessLevel = ID FROM dbo.System_AccessLevels WHERE ID <> @ID

DELETE FROM dbo.System_AccessLevels WHERE ID = @ID


UPDATE dbo.System_ServerGroups
SET AccessLevel_Default = @JustAnotherAccessLevel
WHERE AccessLevel_Default = @ID


UPDATE dbo.Benutzer
Set AccountAccessability = @JustAnotherAccessLevel
Where AccountAccessability = @ID
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServer
	(
		@ServerID int
	)

AS

-- DELETE the server - all depending foreign key rows will be deleted by trigger
DELETE 
FROM dbo.System_Servers
WHERE ID = @ServerID
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServerGroup
(
@ID_ServerGroup int
)

AS 

-- DELETE the server group - all depending foreign key rows will be deleted by trigger
DELETE 
FROM System_ServerGroups
WHERE System_ServerGroups.ID = @ID_ServerGroup

GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteUser
	(
		@UserID int,
		@AdminUserID int = null
	)

AS

-- DELETE the user account 
--     --> all depending foreign key rows will be deleted by trigger
--     --> all data from log_users table will be deleted as defined by separate data protection rules
DELETE FROM dbo.Benutzer WHERE ID=@UserID

-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', -31, 'User deleted by admin ' + Cast(IsNull(@AdminUserID, '') as nvarchar(20)))
GO

----------------------------------------------------
-- dbo.AdminPrivate_GetCompleteUserInfo
----------------------------------------------------
ALTER Procedure dbo.AdminPrivate_GetCompleteUserInfo
(
	@UserID int
)

As
SELECT * FROM dbo.Benutzer WHERE ID = @UserID
	/* set nocount on */
	return

GO

----------------------------------------------------
-- dbo.AdminPrivate_GetScriptEnginesOfServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_GetScriptEnginesOfServer
(
@ServerID int
)

AS 
SELECT     (SELECT     WebEngine.ScriptEngine
                       FROM          System_WebAreaScriptEnginesAuthorization AS WebEngine
                       WHERE      (WebEngine.Server = @ServerID OR
                                              WebEngine.Server IS NULL) AND System_ScriptEngines.ID = WebEngine.ScriptEngine) AS ID, EngineName, ID AS ScriptEngineID,
                          CASE WHEN (SELECT      WebEngine2.Server 
                            FROM          System_WebAreaScriptEnginesAuthorization AS WebEngine2
                            WHERE      (WebEngine2.Server = @ServerID OR
                                                   WebEngine2.Server IS NULL) AND System_ScriptEngines.ID = WebEngine2.ScriptEngine) IS NULL THEN 0 ELSE 1 END AS IsActivated
FROM         System_ScriptEngines
GO

----------------------------------------------------
-- dbo.AdminPrivate_ResetLoginLockedTill
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_ResetLoginLockedTill
	(
		@ID int
	)

AS
declare @AccountAccessability tinyint
declare @LoginDisabled bit
declare @LoginLockedTill datetime
	SET NOCOUNT ON
	SELECT @AccountAccessability = AccountAccessability, @LoginDisabled = LoginDisabled, @LoginLockedTill =LoginLockedTill FROM Benutzer WHERE ID = @ID
	If @LoginLockedTill Is Not Null 
	Begin
		UPDATE    dbo.Benutzer
		SET LoginLockedTill = NULL
		WHERE (ID = @ID)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, URL, ConflictType) values (@ID, GetDate(), '0.0.0.0', '0.0.0.0', 'Lock status reset by Administrator', -5)
	End
	SET NOCOUNT OFF
	SELECT @AccountAccessability As AccountAccessability, @LoginDisabled As LoginDisabled, @LoginLockedTill As LoginLockedTill, NULL As CurrentLoginViaRemoteIP
	RETURN 

GO

----------------------------------------------------
-- dbo.AdminPrivate_SetAuthorizationInherition
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetAuthorizationInherition 
(
@ReleasedByUserID int, 
@IDApp int, 
@InheritsFrom int
)

AS 
SET NOCOUNT ON
UPDATE    dbo.Applications
SET              AuthsAsAppID = @InheritsFrom, ModifiedBy = @ReleasedByUserID, ModifiedOn = getdate()
WHERE     (ID = @IDApp)
-- Logging
If (@InheritsFrom Is Null) 
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 31, 'Application now inherits from nothing')
	End
Else
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 31, 'Application now inherits from ID ' + Convert(varchar(50), @InheritsFrom))
	End
SET NOCOUNT ON
SELECT Result = -1

GO

----------------------------------------------------
-- dbo.AdminPrivate_SetScriptEngineActivation
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetScriptEngineActivation
(
@ScriptEngineID int,
@ServerID int,
@Enabled bit,
@CheckMinimalActivations bit = 0
)

AS 

declare @ID int

SET NOCOUNT ON

-- CheckMinimalActivations nie bei customized Servern (ServerID < 0)
If @ServerID < 0
	SELECT @CheckMinimalActivations = 0

IF @CheckMinimalActivations = 0
	
	BEGIN
		SELECT     @ID = dbo.System_WebAreaScriptEnginesAuthorization.ID
		FROM         dbo.System_WebAreaScriptEnginesAuthorization
		WHERE Server = @ServerID AND ScriptEngine = @ScriptEngineID 
	
		If @Enabled <> 0 
			-- Enabled
			BEGIN
				IF @ID Is Null
					INSERT INTO dbo.System_WebAreaScriptEnginesAuthorization (Server, ScriptEngine)
					VALUES (@ServerID, @ScriptEngineID)
			END			
		Else 
			-- Disabled
			IF @ID Is Not Null
				DELETE FROM dbo.System_WebAreaScriptEnginesAuthorization
				WHERE ID = @ID
	END

ELSE	
	BEGIN
		SELECT   TOP 1  @ID = dbo.System_WebAreaScriptEnginesAuthorization.ID
		FROM         dbo.System_WebAreaScriptEnginesAuthorization
		WHERE Server = @ServerID

		IF @ID Is Null
			-- Activate at least ASP.NET script
			INSERT INTO dbo.System_WebAreaScriptEnginesAuthorization (Server, ScriptEngine)
			VALUES (@ServerID, 2) -- 2 = ASP.NET
	END

SET NOCOUNT OFF

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateAccessLevel 
(
	@ID int,
	@ReleasedByUserID int,
	@Title nvarchar(50),
	@Remarks ntext
)

AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null	
	BEGIN
		SET NOCOUNT ON
		UPDATE dbo.System_AccessLevels 
		SET Title = @Title, ReleasedBy = @ReleasedByUserID, Remarks = @Remarks, ModifiedBy = @ReleasedByUserID, ModifiedOn = GetDate()
		WHERE ID = @ID
		SET NOCOUNT OFF
	END
Else
	
	SELECT Result = 0



GO

--------------------------------------------------
-- dbo.AdminPrivate_UpdateApp --
--------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateApp
(
@Title varchar(255),
@TitleAdminArea nvarchar(255),
@Level1Title nvarchar(512),
@Level2Title nvarchar(512),
@Level3Title nvarchar(512),
@Level4Title nvarchar(512),
@Level5Title nvarchar(512),
@Level6Title nvarchar(512),
@Level1TitleIsHTMLCoded bit,
@Level2TitleIsHTMLCoded bit,
@Level3TitleIsHTMLCoded bit,
@Level4TitleIsHTMLCoded bit,
@Level5TitleIsHTMLCoded bit,
@Level6TitleIsHTMLCoded bit,
@NavURL varchar(512),
@NavFrame varchar(50),
@NavTooltipText nvarchar(1024),
@IsNew bit,
@IsUpdated bit,
@LocationID int,
@LanguageID int,
@ModifiedBy int,
@AppDisabled bit,
@Sort int,
@ResetIsNewUpdatedStatusOn varchar(30),
@OnMouseOver nvarchar(512),
@OnMouseOut nvarchar(512),
@OnClick nvarchar(512),
@AddLanguageID2URL bit,
@ID int
)

AS 
	-- SystemAppType remains untouched
	UPDATE    dbo.Applications
	SET              Title = @Title, TitleAdminArea = @TitleAdminArea, Level1Title = CASE WHEN @Level1Title = '' THEN NULL ELSE @Level1Title END, 
                      Level2Title = CASE WHEN @Level2Title = '' THEN NULL ELSE @Level2Title END, Level3Title = CASE WHEN @Level3Title = '' THEN NULL 
                      ELSE @Level3Title END, Level4Title = CASE WHEN @Level4Title = '' THEN NULL ELSE @Level4Title END, 
                      Level5Title = CASE WHEN @Level5Title = '' THEN NULL ELSE @Level5Title END, Level6Title = CASE WHEN @Level6Title = '' THEN NULL 
                      ELSE @Level6Title END, Level1TitleIsHTMLCoded = @Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded = @Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded = @Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded = @Level4TitleIsHTMLCoded, 
                      Level5TitleIsHTMLCoded = @Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded = @Level6TitleIsHTMLCoded, NavURL = @NavURL, 
                      NavFrame = @NavFrame, NavTooltipText = @NavTooltipText, IsNew = @IsNew, IsUpdated = @IsUpdated, LocationID = @LocationID, 
                      LanguageID = @LanguageID, ModifiedOn = GETDATE(), ModifiedBy = @ModifiedBy, AppDisabled = @AppDisabled, Sort = @Sort, 
                      ResetIsNewUpdatedStatusOn = CONVERT(datetime, @ResetIsNewUpdatedStatusOn, 121), OnMouseOver = @OnMouseOver, 
                      OnMouseOut = @OnMouseOut, OnClick = @OnClick, AddLanguageID2URL = @AddLanguageID2URL
	WHERE     (ID = @ID)

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateServer
(
@Enabled bit,
@IP nvarchar(32),
@ServerDescription nvarchar(200),
@ServerGroup int,
@ServerProtocol nvarchar(50),
@ServerName nvarchar(200),
@ServerPort int,
@ID int
)

AS

DECLARE @CurServerIP nvarchar(32)

DECLARE @OldServerIP nvarchar(32)
DECLARE @OldServerGroup int

SELECT @OldServerIP = (SELECT IP FROM System_Servers WHERE ID = @ID)
SELECT @OldServerGroup = (SELECT ServerGroup FROM System_Servers WHERE ID = @ID)

-- Hat sich die IP geändert?
If @OldServerIP <> @IP 
	BEGIN
		-- Kann der neue IP-Wert gespeichert werden oder existiert er schon?
		SELECT @OldServerIP = (SELECT IP FROM System_Servers WHERE IP = @IP)
		If @OldServerIP Is Not Null
			BEGIN
				RAISERROR ('IP / Host Header already exists', 16, 1
)
				RETURN
			END

		-- Logs umschreiben
		UPDATE LOG
		SET Log.ServerIP = @IP
		WHERE     (Log.ServerIP = @OldServerIP)

	END

-- Server updaten
UPDATE    dbo.System_Servers
SET              Enabled = @Enabled, IP = @IP, ServerDescription = @ServerDescription, ServerGroup = @ServerGroup, ServerProtocol = @ServerProtocol, 
                      ServerName = @ServerName, ServerPort = @ServerPort
WHERE     (ID = @ID)
GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateServerGroup
(
@ID int,
@ServerGroup nvarchar(255),
@ID_Group_Public int,
@ID_Group_Anonymous int,
@MasterServer int,
@UserAdminServer int,
@AreaImage nvarchar(512),
@AreaButton nvarchar(512),
@AreaNavTitle nvarchar(512),
@AreaCompanyFormerTitle nvarchar(512),
@AreaCompanyTitle nvarchar(512),
@AreaSecurityContactEMail nvarchar(512),
@AreaSecurityContactTitle nvarchar(512),
@AreaDevelopmentContactEMail nvarchar(512),
@AreaDevelopmentContactTitle nvarchar(512),
@AreaContentManagementContactEMail nvarchar(512),
@AreaContentManagementContactTitle nvarchar(512),
@AreaUnspecifiedContactEMail nvarchar(512),
@AreaUnspecifiedContactTitle nvarchar(512),
@AreaCopyRightSinceYear int,
@AreaCompanyWebSiteURL nvarchar(512),
@AreaCompanyWebSiteTitle nvarchar(512),
@ModifiedBy int,
@AccessLevel_Default int,
@AllowImpersonationUsers bit = NULL
)

AS

DECLARE @OldAdminServer int
DECLARE @OldMasterServer int
DECLARE @OldAllowImpersonationUsers bit
SELECT @OldAdminServer = UserAdminServer, @OldMasterServer = MasterServer, @OldAllowImpersonationUsers = [AllowImpersonation] FROM dbo.System_ServerGroups WHERE ID = @ID

UPDATE    dbo.System_ServerGroups
SET              ServerGroup = @ServerGroup, ID_Group_Public = @ID_Group_Public, ID_Group_Anonymous = @ID_Group_Anonymous, 
                      MasterServer = @MasterServer, UserAdminServer = @UserAdminServer, AreaImage = @AreaImage, AreaButton = @AreaButton, 
                      AreaNavTitle = @AreaNavTitle, AreaCompanyFormerTitle = @AreaCompanyFormerTitle, AreaCompanyTitle = @AreaCompanyTitle, 
                      AreaSecurityContactEMail = @AreaSecurityContactEMail, AreaSecurityContactTitle = @AreaSecurityContactTitle, 
                      AreaDevelopmentContactEMail = @AreaDevelopmentContactEMail, AreaDevelopmentContactTitle = @AreaDevelopmentContactTitle, 
                      AreaContentManagementContactEMail = @AreaContentManagementContactEMail, 
                      AreaContentManagementContactTitle = @AreaContentManagementContactTitle, AreaUnspecifiedContactEMail = @AreaUnspecifiedContactEMail, 
                      AreaUnspecifiedContactTitle = @AreaUnspecifiedContactTitle, AreaCopyRightSinceYear = @AreaCopyRightSinceYear, 
                      AreaCompanyWebSiteURL = @AreaCompanyWebSiteURL, AreaCompanyWebSiteTitle = @AreaCompanyWebSiteTitle, ModifiedBy = @ModifiedBy, ModifiedOn = getdate(),
                      AccessLevel_Default = @AccessLevel_Default, [AllowImpersonation] = IsNull(@AllowImpersonationUsers, @OldAllowImpersonationUsers)
WHERE     (ID = @ID)

If @OldAdminServer <> @UserAdminServer 
	EXEC AdminPrivate_CreateAdminServerNavPoints @UserAdminServer, @OldAdminServer, @ModifiedBy

If @OldMasterServer <> @MasterServer 
	EXEC AdminPrivate_CreateMasterServerNavPoints @MasterServer, @OldMasterServer, @ModifiedBy

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateStatusLoginDisabled
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_UpdateStatusLoginDisabled 
(
	@Username nvarchar(50),
	@boolStatus bit
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1

		-- Password update
		UPDATE dbo.Benutzer SET LoginDisabled = @boolStatus, ModifiedOn = GetDate() WHERE LoginName = @Username
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

ALTER PROCEDURE [dbo].[AdminPrivate_UpdateSubSecurityAdjustment]
(
	@ActionTypeSave bit,
	@UserID int,
	@TableName nvarchar(255),
	@TablePrimaryIDValue int,
	@AuthorizationType nvarchar(50),
	@ReleasedBy int	
)

AS
DECLARE @CurrentPrimID int

If @ActionTypeSave <> 0
	-- Update or Insert Where Update is never neccessary
	BEGIN
	SELECT @CurrentPrimID = ID 
	FROM System_SubSecurityAdjustments 
	WHERE UserID = @UserID 
		AND TableName = @TableName 
		AND TablePrimaryIDValue = @TablePrimaryIDValue
		AND AuthorizationType = @AuthorizationType
	IF @CurrentPrimID Is Null 
		-- Insert required
		INSERT INTO System_SubSecurityAdjustments (UserID, TableName, TablePrimaryIDValue, AuthorizationType, ReleasedOn, ReleasedBy)
		VALUES (@UserID, @TableName, @TablePrimaryIDValue, @AuthorizationType, GETDATE(), @ReleasedBy)
	END
ELSE
	-- Delete
	DELETE FROM System_SubSecurityAdjustments 
	WHERE UserID = @UserID 
		AND TableName = @TableName 
		AND TablePrimaryIDValue = @TablePrimaryIDValue
		AND AuthorizationType = @AuthorizationType
GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateUserDetails 
(
	@CurUserID int,
	@WebApplication varchar(1024),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int,
	@LoginDisabled bit = 0,
	@LoginLockedTill datetime,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null,
	@DoNotLogSuccess bit = 0,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)

AS

SET NOCOUNT ON
	-- Profile update
	UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, Land = @Land, State = @State, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, AccountAccessability = @AccountAccessability, LoginDisabled = @LoginDisabled, LoginLockedTill = @LoginLockedTill, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE ID = @CurUserID
	-- Logging
	IF IsNull(@DoNotLogSuccess, 0) = 0
	BEGIN
		If IsNull(@IsUserChange, 0) = 0 
			-- admin
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 4, 'Admin with user id ' + CAST(@ModifiedBy as varchar(20)) + ' has modified profile')
		Else
			-- user change
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', -4, 'User has modified profile')
	END
	-- Write UserDetails
	Exec Int_UpdateUserDetailDataWithProfileData @CurUserID, @ModifiedBy

-- Rückgabewert
SET NOCOUNT OFF
SELECT Result = -1

GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_UpdateUserPW] 
(
	@Username nvarchar(50),
	@NewPasscode varchar(4096),
	@DoNotLogSuccess bit = 0,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0,
	@LoginPWAlgorithm int = 0,
	@LoginPWNonceValue varbinary(4096) = 0x00 
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Password update
		UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate(), LoginLockedTill = Null, LoginFailures = 0, LoginPwAlgorithm = @LoginPWAlgorithm, LoginPWNonceValue = @LoginPWNonceValue WHERE LoginName = @Username
		-- Logging
		IF IsNull(@DoNotLogSuccess, 0) = 0
		BEGIN
			If IsNull(@IsUserChange, 0) = 0 
				-- admin
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription)
 				values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 5, 'Admin with user id ' + CAST(@ModifiedBy as varchar(20)) + ' has modified password')
			Else
				-- user change
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
				values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 6, 'User has modified password')
		END	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.Int_LogAuthChanges
----------------------------------------------------
ALTER PROCEDURE dbo.Int_LogAuthChanges
(
@UserID int = Null,
@GroupID int = Null,
@AppID int,
@ReleasedByUserID int = Null,
@IsDevelopmentTeamMember int = NULL, 
@IsDenyRule int = NULL, 
@ServerGroupID int = NULL
)

AS 

declare @FlagInfo nvarchar(20)
SELECT @FlagInfo = N''
IF IsNull(@IsDevelopmentTeamMember, 0) <> 0 AND IsNull(@IsDenyRule, 0) <> 0
	SELECT @FlagInfo = ' (Dev+DenyRule)'
ELSE IF IsNull(@IsDevelopmentTeamMember, 0) <> 0
	SELECT @FlagInfo = ' (Dev)'
ELSE IF IsNull(@IsDenyRule, 0) <> 0
	SELECT @FlagInfo = ' (DenyRule)'

If @GroupID Is Not Null
	begin
		-- log indirect changes on users
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription, ServerGroupID) 
		select id_user, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -7, Null, NULL
		from [dbo].[Memberships_EffectiveRulesWithClonesNthGrade] 
		where id_group = @GroupID
		-- log group auth change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription, ServerGroupID) 
		values (IsNull(@UserID, 0), GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -8, cast(@GroupID as nvarchar(20)) + N' by user ' + cast(@ReleasedByUserID as nvarchar(20)) + @FlagInfo, NULL)
	end
Else
	If @UserID Is Not Null
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription, ServerGroupID) 
		values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -6, N'by user ' + cast(@ReleasedByUserID as nvarchar(20)) + @FlagInfo, NULL)
GO


----------------------------------------------------
-- dbo.Int_UpdateUserDetailDataWithProfileData
----------------------------------------------------
ALTER Procedure Int_UpdateUserDetailDataWithProfileData
	(
		@IDUser int,
		@ModifiedBy int = 0
	)

As
DECLARE @LoginName nvarchar(50)
	-- Result and Initializing
	SELECT -1
	set nocount on
-- GetCompleteName (a little bit modified)
	DECLARE @Addresses nvarchar (20)
	DECLARE @Titel nvarchar (40)
	DECLARE @Vorname nvarchar(60)
	DECLARE @Nachname nvarchar(60)
	DECLARE @Namenszusatz nvarchar(40)
	DECLARE @CompleteName nvarchar (255)
	DECLARE @CompleteNameInclAddresses nvarchar (255)
	DECLARE @CustomerNo nvarchar(100) 
	DECLARE @SupplierNo nvarchar(100)
	DECLARE @Company nvarchar(100)
	DECLARE @eMail nvarchar (50)
	DECLARE @Sex varchar (1)
	
	SELECT @Sex = Case Anrede When 'Mr.' Then 'm' When 'Ms.' Then 'w' Else Null End, @eMail = [E-MAIL], @Vorname = Vorname, @Nachname = Nachname, @Nachname = Nachname, @Namenszusatz = Namenszusatz, @CustomerNo = CustomerNo, @SupplierNo = SupplierNo, @Company = Company, @Titel = Titel, @Addresses = Anrede FROM dbo.Benutzer WHERE ID = @IDUser

	-- Namenszusatz könnte Null sein
	If substring(@Namenszusatz,1,20) <> '' --Is Not Null
		SET @Namenszusatz = ' ' + @Namenszusatz
	Else
		SET @Namenszusatz = ''
	SET @CompleteName = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	SET @CompleteNameInclAddresses  = LTrim(RTrim(@Addresses + @Titel)) + ' ' + LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	Exec Public_SetUserDetailData @IDUser, 'CompleteName', @CompleteName, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'CompleteNameInclAddresses', @CompleteNameInclAddresses, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'Sex', @Sex, 1, @ModifiedBy
	-- e-mail address
	Exec Public_SetUserDetailData @IDUser, 'email', @eMail, 1, @ModifiedBy
	-- Other details
	Exec Public_SetUserDetailData @IDUser, 'CustomerNo', @CustomerNo, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'SupplierNo', @SupplierNo, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'Company', @Company, 1, @ModifiedBy

-- Exit

	return 
GO


IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[IsAdministratorForAuthorizations]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsAdministratorForAuthorizations]
GO
CREATE PROC [dbo].[IsAdministratorForAuthorizations]
(
	@AuthorizationType nvarchar(50),
	@AdminUserID int,
	@SecurityObjectID int
)

AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @Result int
select top 1 @Result = ID 
from System_SubSecurityAdjustments 
where 
	(
		(
			(
                    UserID=@AdminUserID 
                    and TableName='Applications' 
                    and AuthorizationType='Owner' 
                    and TablePrimaryIDValue = @SecurityObjectID
            )
			OR 
			(
			        UserID=@AdminUserID 
			        and TableName='Applications' 
			        and AuthorizationType=@AuthorizationType 
			        and TablePrimaryIDValue = @SecurityObjectID
			)
			OR 
			(
			        UserID=@AdminUserID 
			        and TableName='Applications' 
			        and AuthorizationType='SecurityMaster' 
			        and TablePrimaryIDValue = 0
			)
		)
		AND @AdminUserID IN 
		(
		        SELECT ID 
		        FROM Benutzer 
		        WHERE ID = @AdminUserID 
		            AND LoginDisabled = 0
		) -- user must still be valid
		AND @AdminUserID IN 
		(
		        SELECT ID_User 
		        FROM Memberships_EffectiveRulesWithClonesNthGrade 
		        WHERE ID_Group = 7 
		            AND ID_User = @AdminUserID
		) -- user must still be security admin
	)
	OR @AdminUserID IN 
        (
            SELECT ID_User 
            FROM Memberships_EffectiveRulesWithClonesNthGrade 
            WHERE ID_Group = 6 
                AND ID_User = @AdminUserID
        ) -- ALTERNATIVELY user must be a supervisor
IF @Result IS NULL 
	RETURN 0
ELSE
	RETURN 1
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[IsAdministratorForMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsAdministratorForMemberships]
GO
CREATE PROC [dbo].[IsAdministratorForMemberships]
(
	@AuthorizationType nvarchar(50),
	@AdminUserID int,
	@GroupID int
)

AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @Result int
select top 1 @Result = ID 
from System_SubSecurityAdjustments 
where 
	(
		(
			(UserID=@AdminUserID and TableName='Groups' and AuthorizationType='Owner' and TablePrimaryIDValue = @GroupID)
			OR (UserID=@AdminUserID and TableName='Groups' and AuthorizationType=@AuthorizationType and TablePrimaryIDValue = @GroupID)
			OR (UserID=@AdminUserID and TableName='Groups' and AuthorizationType='SecurityMaster' and TablePrimaryIDValue = 0)
		)
		AND @AdminUserID IN (SELECT ID FROM Benutzer WHERE ID = @AdminUserID AND LoginDisabled = 0) -- user must still be valid
		AND @AdminUserID IN (SELECT ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade WHERE ID_Group = 7 AND ID_User = @AdminUserID) -- user must still be security admin
	)
	OR @AdminUserID IN (SELECT ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade WHERE ID_Group = 6 AND ID_User = @AdminUserID) -- ALTERNATIVELY user must be a supervisor
IF @Result IS NULL 
	RETURN 0
ELSE
	RETURN 1
GO

--------------------------------------------------------------------------------------------------------------------------------
-- FIX for previous build 162 (which has been already fixed, too): Remove SP which has got schema name of the current DB user --
--------------------------------------------------------------------------------------------------------------------------------
IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo_camm_WebManager].[LogMissingExternalUserAssignment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo_camm_WebManager.LogMissingExternalUserAssignment
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[LogMissingExternalUserAssignment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.LogMissingExternalUserAssignment
GO
CREATE PROCEDURE dbo.LogMissingExternalUserAssignment
(
	@ExternalAccountSystem nvarchar(50),
	@LogonName nvarchar(100),
	@FullUserName nvarchar(150),
	@EMailAddress nvarchar(250),
	@Error ntext,
	@Remove bit
)

AS
IF @Remove = 0
	BEGIN
		-- Do an ADD if required
		DECLARE @ID int
		SELECT @ID = ID
		FROM [Log_MissingAssignmentsOfExternalAccounts]
		WHERE [ExternalAccountSystem] = @ExternalAccountSystem 
			AND [UserName] = @LogonName
		IF @ID IS NULL
			INSERT INTO [Log_MissingAssignmentsOfExternalAccounts] ([ExternalAccountSystem], [UserName], [FullName], [EMailAddress], [Error])
			VALUES (@ExternalAccountSystem, @LogonName, @FullUserName, @EMailAddress, @Error) 
	END
ELSE
	BEGIN
		-- Do a REMOVE
		DELETE 
		FROM [Log_MissingAssignmentsOfExternalAccounts]
		WHERE [ExternalAccountSystem] = @ExternalAccountSystem 
			AND [UserName] = @LogonName
	END
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[LookupUserNameByScriptEngineSessionID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[LookupUserNameByScriptEngineSessionID]
GO
CREATE PROC dbo.LookupUserNameByScriptEngineSessionID
	(
	@ServerID int,
	@ScriptEngineID int,
	@ScriptEngineSessionID nvarchar(128)
	)

AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT Benutzer.LoginName
FROM [System_WebAreasAuthorizedForSession] AS SSID
	LEFT JOIN System_UserSessions AS USID ON SSID.SessionID = USID.ID_Session
	LEFT JOIN Benutzer ON USID.ID_User = Benutzer.ID
WHERE SSID.Server = @ServerID
	AND SSID.ScriptEngine_ID = @ScriptEngineID
	AND SSID.ScriptEngine_SessionID = @ScriptEngineSessionID
GO
----------------------------------------------------
-- dbo.Public_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.Public_CreateUserAccount
(
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int = 0,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @ServerGroupID int
DECLARE @CurUserStatus_InternalSessionID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @ServerGroupID = dbo.System_Servers.ServerGroup
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0

	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, account will be created now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Create account
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, CreatedOn, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], LoginCount, CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, 1, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = SCOPE_IDENTITY() --(select ID from dbo.Benutzer where LoginName = @Username)
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = SCOPE_IDENTITY()
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		INSERT INTO dbo.System_WebAreasAuthorizedForSession
		                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
		SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
		                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
		FROM         dbo.System_Servers INNER JOIN
		                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
		WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @ServerGroupID)
		SET NOCOUNT OFF
		-- Logging
		SET NOCOUNT ON
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 1, 'Account ' + @Username + ' created')
		SET NOCOUNT OFF
	END
Else
	-- Rückgabewert
	SELECT Result = 1
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO

----------------------------------------------------
-- dbo.Public_GetCompleteName
----------------------------------------------------
ALTER Procedure Public_GetCompleteName
(
	@Username nvarchar(50)
)

As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @Vorname nvarchar(60)
DECLARE @Nachname nvarchar(60)
DECLARE @Namenszusatz nvarchar(40)
SET @Vorname = (SELECT Vorname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Nachname = (SELECT Nachname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Namenszusatz = (SELECT Namenszusatz FROM dbo.Benutzer WHERE Loginname = @Username)
-- Namenszusatz könnte Null sein
If substring(@Namenszusatz,1,20) <> '' --Is Not Null
	SET @Namenszusatz = ' ' + @Namenszusatz
Else
	SET @Namenszusatz = ''
SELECT Result = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	/* set nocount on */
	return 
GO

----------------------------------------------------
-- dbo.Public_GetCurServerLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetCurServerLogonList
(
@ServerIP nvarchar(32)
)

AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @ServerGroupID int
----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @ServerGroupID = dbo.System_Servers.ServerGroup

FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Abbruch
		Return
	END

---------------------------------------
-- Anmeldestellen zurückliefern --
---------------------------------------
SELECT     NULL AS ID, NULL AS SessionID, System_Servers.IP, System_Servers.ServerDescription, System_Servers.ServerProtocol, 
                      System_Servers.ServerName, System_Servers.ServerPort, NULL AS ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, NULL AS ScriptEngine_SessionID, System_WebAreaScriptEnginesAuthorization.ID AS OrderID1, 
                      System_Servers.ID AS OrderID2, System_ScriptEngines.ID AS OrderID3
FROM         System_Servers INNER JOIN
                      System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server INNER JOIN
                      System_ScriptEngines ON System_WebAreaScriptEnginesAuthorization.ScriptEngine = System_ScriptEngines.ID
WHERE     (System_Servers.Enabled <> 0) AND (System_Servers.ServerGroup = @ServerGroupID) AND (System_Servers.ID > 0)
ORDER BY System_WebAreaScriptEnginesAuthorization.ID, System_Servers.ID, System_ScriptEngines.ID
GO

----------------------------------------------------
-- dbo.Public_GetEMailAddressesOfAllSecurityAdmins
----------------------------------------------------
ALTER Procedure dbo.Public_GetEMailAddressesOfAllSecurityAdmins

AS

SELECT Benutzer.[E-MAIL], Benutzer.ID 
FROM dbo.Memberships_EffectiveRulesWithClonesNthGrade 
	LEFT OUTER JOIN dbo.Benutzer 
		ON dbo.Memberships_EffectiveRulesWithClonesNthGrade.ID_User = dbo.Benutzer.ID
WHERE dbo.Memberships_EffectiveRulesWithClonesNthGrade.ID_Group = 7
GROUP BY Benutzer.[E-MAIL], Benutzer.ID

return 

GO

----------------------------------------------------
-- dbo.Public_GetLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetLogonList
	(
	@Username nvarchar(50),
	@ScriptEngine_SessionID nvarchar(512) = NULL,
	@ScriptEngine_ID int = NULL,
	@ServerID int = NULL
	)

AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

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
FROM System_WebAreasAuthorizedForSession 
      INNER JOIN System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID 
      INNER JOIN System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID 
WHERE (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL)
		AND System_WebAreasAuthorizedForSession.SessionID = @CurSessionID
		AND (System_Servers.ID > 0)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo].[Public_GetNavPointsOfGroup]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Public_GetNavPointsOfGroup]
GO
----------------------------------------------------
-- dbo.Public_GetNavPointsOfGroup
----------------------------------------------------
CREATE Procedure [dbo].[Public_GetNavPointsOfGroup]
	@GroupID int,
	@ServerIP nvarchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0,
	@SearchForAlternativeLanguages bit = 1

As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @ServerGroupID int
DECLARE @AlternativeLanguage int
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int

SET NoCount ON

-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Abbruch
		Return
	END

-- for example: LanguageID = 512 --> also search for the alternative language with ID 2
If @SearchForAlternativeLanguages = 1
	SELECT @AlternativeLanguage = AlternativeLanguage
		FROM System_Languages WITH (NOLOCK)
		WHERE ID = @LanguageID
Else
	SELECT @AlternativeLanguage = @LanguageID
If @AlternativeLanguage IS NULL
	SELECT @AlternativeLanguage = @LanguageID

-- Lookup ServerGroupID of given serverIP
SELECT @ServerGroupID = dbo.System_Servers.ServerGroup, 
	@PublicGroupID = dbo.System_ServerGroups.ID_Group_Public,
	@AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous
FROM dbo.System_Servers WITH (NOLOCK) 
	INNER JOIN dbo.System_ServerGroups WITH (NOLOCK) 
		ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE dbo.System_Servers.IP = @ServerIP
IF @ServerGroupID Is Null 
	Return

IF @AnonymousAccess <> 0
	SELECT @GroupID = NULL, @PublicGroupID = NULL; 

--ResetIsNewUpdatedStatusOn
UPDATE dbo.Applications_CurrentAndInactiveOnes 
SET IsNew = 0, IsUpdated = 0, ResetIsNewUpdatedStatusOn = Null 
WHERE ResetIsNewUpdatedStatusOn < GETDATE()

-- Recordset zurückgeben	
		-- All NavItem Titles where the group is authorized for AND where it is marked as IsUpdated/IsNew
		CREATE TABLE #NavUpdatedItems_Filtered (Level1Title nvarchar(255), Level2Title nvarchar(255), Level3Title nvarchar(255), Level4Title nvarchar(255), Level5Title nvarchar(255), Level6Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Filtered (Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title)
		SELECT Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
		FROM dbo.Applications 
			INNER JOIN dbo.System_Servers 
				ON dbo.Applications.LocationID = dbo.System_Servers.ID
		WHERE 
			dbo.Applications.ID IN 
				(
					SELECT ID_SecurityObject
					FROM dbo.ApplicationsRightsByGroup_EffectiveCumulative
					WHERE ID_ServerGroup = @ServerGroupID 
						AND ID_Group IN (@GroupID, @PublicGroupID, @AnonymousGroupID) 
				)	
			AND dbo.System_Servers.ServerGroup = @ServerGroupID
			AND LanguageID in (@LanguageID, @AlternativeLanguage)  
			AND (IsUpdated <> 0 OR IsNew <> 0)
		GROUP BY Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title

		SET NoCount OFF

			SELECT DISTINCT 
				dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, 
				dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
				dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, 
				dbo.Applications.Level3TitleIsHTMLCoded, dbo.Applications.Level4TitleIsHTMLCoded, 
				dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
				dbo.Applications.NavURL, dbo.Applications.NavFrame, 
				dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
				dbo.Applications.NavToolTipText, dbo.Applications.Sort, dbo.Applications.AppDisabled, 
				dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
				dbo.Applications.AddLanguageID2URL,
				Case When dbo.Applications.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, 
				Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, 
				Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, 
				Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, 
				Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, 
				Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent,
				case when 
					(
						select top 1 Level1Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title
					) is null then 0 else 1 end as Level1IsUpdated,
				case when 
					(
						select top 1 Level2Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title
					) is null then 0 else 1 end as Level2IsUpdated,
				case when 
					(
						select top 1 Level3Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title
					) is null then 0 else 1 end as Level3IsUpdated,
				case when 
					(
						select top 1 Level4Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title 
							AND #NavUpdatedItems_Filtered.Level4Title = dbo.Applications.Level4Title
					) is null then 0 else 1 end as Level4IsUpdated,
				case when 
					(
						select top 1 Level5Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title 
							AND #NavUpdatedItems_Filtered.Level4Title = dbo.Applications.Level4Title 
							AND #NavUpdatedItems_Filtered.Level5Title = dbo.Applications.Level5Title
					) is null then 0 else 1 end as Level5IsUpdated,
				case when 
					(
						select top 1 Level6Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title 
							AND #NavUpdatedItems_Filtered.Level4Title = dbo.Applications.Level4Title 
							AND #NavUpdatedItems_Filtered.Level5Title = dbo.Applications.Level5Title 
							AND #NavUpdatedItems_Filtered.Level6Title = dbo.Applications.Level6Title
					) is null then 0 else 1 end as Level6IsUpdated,
				Case 
					When Substring(NavURL,1,1) = '/' 
						Then ServerProtocol + '://' 
								+ ServerName 
								+ Case 
										When ServerPort Is Not Null 
											Then ':' +Cast(ServerPort as Varchar(6)) 
										Else '' 
										End 
								+ NavURL 
					Else 
						NavURL 
					End As NavURLAutocompleted
			FROM dbo.Applications
				INNER JOIN dbo.System_Servers 
					ON dbo.Applications.LocationID = dbo.System_Servers.ID
			WHERE 
				dbo.Applications.ID IN 
					(
						SELECT ID_SecurityObject
						FROM dbo.ApplicationsRightsByGroup_EffectiveCumulative
						WHERE ID_ServerGroup = @ServerGroupID 
							AND ID_Group IN (@GroupID, @PublicGroupID, @AnonymousGroupID) 
					)	
				AND dbo.System_Servers.ServerGroup = @ServerGroupID
				AND LanguageID in (@LanguageID, @AlternativeLanguage)  
				AND (IsNull(Level1Title, '')<>'' OR IsNull(NavUrl, '')<>'')
			ORDER BY dbo.Applications.Sort, 
				Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level1Title, 
				Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level2Title, 
				Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level3Title, 
				Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level4Title, 
				Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level5Title, 
				dbo.Applications.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo].[Public_GetNavPointsOfUser]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Public_GetNavPointsOfUser]
GO
----------------------------------------------------
-- dbo.Public_GetNavPointsOfUser
----------------------------------------------------
CREATE Procedure dbo.Public_GetNavPointsOfUser
(
	@UserID int,
	@ServerIP nvarchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0,
	@SearchForAlternativeLanguages bit = 1
)

As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @ServerGroupID int
DECLARE @AlternativeLanguage int

SET NoCount ON

-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Abbruch
		Return
	END

-- for example: LanguageID = 512 --> also search for the alternative language with ID 2
If @SearchForAlternativeLanguages = 1
	SELECT @AlternativeLanguage = AlternativeLanguage
		FROM System_Languages WITH (NOLOCK)
		WHERE ID = @LanguageID
Else
	SELECT @AlternativeLanguage = @LanguageID
If @AlternativeLanguage IS NULL
	SELECT @AlternativeLanguage = @LanguageID

-- Lookup ServerGroupID of given serverIP
SELECT @ServerGroupID = dbo.System_Servers.ServerGroup
FROM dbo.System_Servers WITH (NOLOCK) 
	INNER JOIN dbo.System_ServerGroups WITH (NOLOCK) 
		ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE dbo.System_Servers.IP = @ServerIP
IF @ServerGroupID Is Null 
	Return

IF @AnonymousAccess <> 0
	SELECT @UserID = -1 -- ID of anonymous user

--ResetIsNewUpdatedStatusOn
UPDATE dbo.Applications_CurrentAndInactiveOnes 
SET IsNew = 0, IsUpdated = 0, ResetIsNewUpdatedStatusOn = Null 
WHERE ResetIsNewUpdatedStatusOn < GETDATE()

-- Recordset zurückgeben	

		-- All NavItem Titles where the user is authorized for AND where it is marked as IsUpdated/IsNew
		CREATE TABLE #NavUpdatedItems_Filtered (Level1Title nvarchar(255), Level2Title nvarchar(255), Level3Title nvarchar(255), Level4Title nvarchar(255), Level5Title nvarchar(255), Level6Title nvarchar(255));
		INSERT INTO #NavUpdatedItems_Filtered (Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title)
		SELECT Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
		FROM dbo.Applications 
			INNER JOIN dbo.System_Servers 
				ON dbo.Applications.LocationID = dbo.System_Servers.ID
		WHERE 
			dbo.Applications.ID IN 
				(
					SELECT ID_SecurityObject
					FROM dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules
					WHERE ID_ServerGroup = @ServerGroupID 
						AND ID_User = @UserID 
				)	
			AND dbo.System_Servers.ServerGroup = @ServerGroupID
			AND LanguageID in (@LanguageID, @AlternativeLanguage)  
			AND (IsUpdated <> 0 OR IsNew <> 0)
		GROUP BY Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title

		SET NoCount OFF

			SELECT DISTINCT 
				dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, 
				dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
				dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, 
				dbo.Applications.Level3TitleIsHTMLCoded, dbo.Applications.Level4TitleIsHTMLCoded, 
				dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
				dbo.Applications.NavURL, dbo.Applications.NavFrame, 
				dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
				dbo.Applications.NavToolTipText, dbo.Applications.Sort, dbo.Applications.AppDisabled, 
				dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
				dbo.Applications.AddLanguageID2URL,
				Case When dbo.Applications.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, 
				Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, 
				Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, 
				Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, 
				Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, 
				Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent,
				case when 
					(
						select top 1 Level1Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title
					) is null then 0 else 1 end as Level1IsUpdated,
				case when 
					(
						select top 1 Level2Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title
					) is null then 0 else 1 end as Level2IsUpdated,
				case when 
					(
						select top 1 Level3Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title
					) is null then 0 else 1 end as Level3IsUpdated,
				case when 
					(
						select top 1 Level4Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title 
							AND #NavUpdatedItems_Filtered.Level4Title = dbo.Applications.Level4Title
					) is null then 0 else 1 end as Level4IsUpdated,
				case when 
					(
						select top 1 Level5Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title 
							AND #NavUpdatedItems_Filtered.Level4Title = dbo.Applications.Level4Title 
							AND #NavUpdatedItems_Filtered.Level5Title = dbo.Applications.Level5Title
					) is null then 0 else 1 end as Level5IsUpdated,
				case when 
					(
						select top 1 Level6Title 
						from #NavUpdatedItems_Filtered 
						where #NavUpdatedItems_Filtered.Level1Title = dbo.Applications.Level1Title 
							AND #NavUpdatedItems_Filtered.Level2Title = dbo.Applications.Level2Title 
							AND #NavUpdatedItems_Filtered.Level3Title = dbo.Applications.Level3Title 
							AND #NavUpdatedItems_Filtered.Level4Title = dbo.Applications.Level4Title 
							AND #NavUpdatedItems_Filtered.Level5Title = dbo.Applications.Level5Title 
							AND #NavUpdatedItems_Filtered.Level6Title = dbo.Applications.Level6Title
					) is null then 0 else 1 end as Level6IsUpdated,
				Case 
					When Substring(NavURL,1,1) = '/' 
						Then ServerProtocol + '://' 
								+ ServerName 
								+ Case 
										When ServerPort Is Not Null 
											Then ':' +Cast(ServerPort as Varchar(6)) 
										Else '' 
										End 
								+ NavURL 
					Else 
						NavURL 
					End As NavURLAutocompleted
			FROM dbo.Applications
				INNER JOIN dbo.System_Servers 
					ON dbo.Applications.LocationID = dbo.System_Servers.ID
			WHERE 
				dbo.Applications.ID IN 
					(
					SELECT ID_SecurityObject
					FROM dbo.[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
						WHERE ID_ServerGroup = @ServerGroupID 
							AND ID_User = @UserID 
					)	
				AND dbo.System_Servers.ServerGroup = @ServerGroupID
				AND LanguageID in (@LanguageID, @AlternativeLanguage)  
				AND (IsNull(Level1Title, '')<>'' OR IsNull(NavUrl, '')<>'')
			ORDER BY dbo.Applications.Sort, 
				Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level1Title, 
				Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level2Title, 
				Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level3Title, 
				Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level4Title, 
				Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End, 
				dbo.Applications.Level5Title, 
				dbo.Applications.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered
GO

----------------------------------------------------
-- dbo.Public_GetServerConfig
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetServerConfig
(
@ServerIP nvarchar(32)
)

AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT     dbo.System_ServerGroups.ServerGroup AS ServerGroupDescription, dbo.System_ServerGroups.ID_Group_Public, 
                      System_Servers_1.ServerProtocol AS MasterServerProtocol, System_Servers_1.ServerName AS MasterServerName, 
                      System_Servers_1.ServerPort AS MasterServerPort, System_Servers_2.ServerProtocol AS UserAdminServerProtocol, 
                      System_Servers_2.ServerName AS UserAdminServerName, System_Servers_2.ServerPort AS UserAdminServerPort, 
                      dbo.System_ServerGroups.UserAdminServer, System_Servers_3.*, dbo.System_ServerGroups.AreaImage AS ServerGroupImageBig, 
                      dbo.System_ServerGroups.AreaButton AS ServerGroupImageSmall, COALESCE (dbo.System_ServerGroups.AreaNavTitle, 
                      dbo.System_ServerGroups.ServerGroup) AS ServerGroupTitle_Navigation, dbo.System_ServerGroups.AreaCompanyFormerTitle, 
                      dbo.System_ServerGroups.AreaCompanyTitle, dbo.System_ServerGroups.AreaSecurityContactEMail, 
                      dbo.System_ServerGroups.AreaSecurityContactTitle, dbo.System_ServerGroups.AreaDevelopmentContactEMail, 
                      dbo.System_ServerGroups.AreaDevelopmentContactTitle, dbo.System_ServerGroups.AreaContentManagementContactEMail, 
                      dbo.System_ServerGroups.AreaContentManagementContactTitle, dbo.System_ServerGroups.AreaUnspecifiedContactEMail, 
                      dbo.System_ServerGroups.AreaUnspecifiedContactTitle, dbo.System_ServerGroups.AreaCopyRightSinceYear, 
                      dbo.System_ServerGroups.AreaCompanyWebSiteURL, dbo.System_ServerGroups.AreaCompanyWebSiteTitle, 
                      dbo.System_ServerGroups.ID AS ID_ServerGroup, dbo.System_ServerGroups.AccessLevel_Default
FROM         dbo.System_ServerGroups INNER JOIN
                      dbo.System_Servers System_Servers_2 ON dbo.System_ServerGroups.UserAdminServer = System_Servers_2.ID INNER JOIN
                      dbo.System_Servers System_Servers_1 ON dbo.System_ServerGroups.MasterServer = System_Servers_1.ID INNER JOIN
                      dbo.System_Servers System_Servers_3 ON dbo.System_ServerGroups.ID = System_Servers_3.ServerGroup

WHERE     (System_Servers_3.IP = @ServerIP)
GO

----------------------------------------------------
-- dbo.Public_GetToDoLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetToDoLogonList
	(
	@Username nvarchar(50),
	@ScriptEngine_SessionID nvarchar(512),
	@ScriptEngine_ID int,
	@ServerID int
	)

AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
DECLARE @CurrentSessionID int

-- GUIDs alter Sessions zurücksetzen
SET NOCOUNT ON
UPDATE    System_WebAreasAuthorizedForSession WITH (XLOCK, ROWLOCK)
SET              Inactive = 1
WHERE     (LastSessionStateRefresh < DATEADD(hh, - 12, GETDATE()))

-- GUIDs alter Sessions zurücksetzen
DELETE FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes WITH (XLOCK, ROWLOCK)
WHERE(Inactive = 1 And (LastSessionStateRefresh < DateAdd(Day, -3, GETDATE())))

-- Lookup internal session ID
SELECT TOP 1 @CurrentSessionID = SessionID
FROM System_WebAreasAuthorizedForSession WITH (NOLOCK) 
WHERE ScriptEngine_SessionID = @ScriptEngine_SessionID 
	AND ScriptEngine_ID = @ScriptEngine_ID
	AND Server = @ServerID
	AND SessionID IN (SELECT ID_Session FROM [dbo].[System_UserSessions] WITH (NOLOCK) WHERE ID_User = (SELECT ID FROM dbo.Benutzer WITH (NOLOCK) WHERE LoginName = @Username))

IF @CurrentSessionID IS NOT NULL
	-- WebAreaSessionState aktualisieren
	update dbo.System_WebAreasAuthorizedForSession 
	set LastSessionStateRefresh = getdate() 
	where ScriptEngine_ID = @ScriptEngine_ID 
		AND SessionID = @CurrentSessionID 
		AND Server = @ServerID
		AND ScriptEngine_SessionID = @ScriptEngine_SessionID
		AND LastSessionStateRefresh < DATEADD(minute, - 3, GETDATE())
	
-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
	System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
	System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, System_WebAreasAuthorizedForSession.ScriptEngine_ID,
	System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
FROM System_WebAreasAuthorizedForSession WITH (NOLOCK) 
	INNER JOIN System_Servers WITH (NOLOCK) ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID 
	INNER JOIN System_ScriptEngines WITH (NOLOCK) ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID 
WHERE System_WebAreasAuthorizedForSession.ScriptEngine_SessionID IS NULL
		AND System_Servers.ID > 0
		AND System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL
		AND System_WebAreasAuthorizedForSession.SessionID = @CurrentSessionID
	OR System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL
		AND System_Servers.ID > 0
		AND System_WebAreasAuthorizedForSession.SessionID = @CurrentSessionID
		-- never show the current script engine session
		AND NOT (
			System_WebAreasAuthorizedForSession.ScriptEngine_SessionID = @ScriptEngine_SessionID 
			AND System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID
			AND System_WebAreasAuthorizedForSession.Server = @ServerID
			)
		AND System_WebAreasAuthorizedForSession.LastSessionStateRefresh < DATEADD(minute, - 3, GETDATE())
GO

----------------------------------------------------
-- dbo.Public_GetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserDetailData
	(
		@IDUser int,
		@Type varchar(50)
	)

As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

If @Type = 'Sex'
	SELECT CASE WHEN Anrede = 'Mr.' THEN 'm' Else 'w' END As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LoginName'
	SELECT LoginName As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'Addresses'
	SELECT Anrede As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LastName'
	SELECT Nachname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'FirstName'
	SELECT Vorname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '1stPreferredLanguage'
	SELECT [1stPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '2ndPreferredLanguage'
	SELECT [2ndPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '3rdPreferredLanguage'
	SELECT [3rdPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'AccessLevel'
	SELECT [AccountAccessability] As Result FROM Benutzer WHERE ID = @IDUser
Else
	SELECT Value As Result FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
GO

----------------------------------------------------
-- dbo.Public_GetUserID
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserID
(
	@Username nvarchar(50)
)

As
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

declare @UserID int

set nocount on
SELECT TOP 1 @UserID = ID FROM dbo.Benutzer WHERE Loginname = @Username
set nocount off

SELECT Result = @UserID

Return @UserID
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetUserNameForScriptEngineSessionID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_GetUserNameForScriptEngineSessionID]
GO

CREATE PROCEDURE dbo.Public_GetUserNameForScriptEngineSessionID
(
	@UserName nvarchar(50) output,
	@ScriptEngine_SessionID nvarchar(128),
	@ScriptEngine_ID int,
	@ServerID int
)

AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

select @UserName = dbo.Benutzer.LoginName
from dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes 
	left join dbo.System_UserSessions ON dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.SessionID = dbo.System_UserSessions.ID_Session
	left join dbo.Benutzer ON dbo.System_UserSessions.ID_User = dbo.Benutzer.ID
where scriptengine_sessionID = @ScriptEngine_SessionID 
	and server=@ServerID
	and scriptengine_ID=@ScriptEngine_ID
	and inactive=0
order by sessionid desc

GO

----------------------------------------------------
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
(
	@Username nvarchar(50),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int = NULL,
	@ScriptEngine_SessionID nvarchar(512) = NULL
)

AS

SET NOCOUNT ON

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @ServerID int
DECLARE @System_SessionID int
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
		SELECT @CurUserID = ID from dbo.Benutzer where LoginName = @Username
		SELECT TOP 1 @System_SessionID = [SessionID]
		FROM [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
		where inactive = 0
			and server = @serverid
			and scriptengine_sessionID = @ScriptEngine_SessionID
			and scriptengine_id = @ScriptEngine_ID
			AND SessionID IN (SELECT ID_Session FROM [dbo].[System_UserSessions] WITH (NOLOCK) WHERE ID_User = @CurUserID)
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
-- Session schließen
UPDATE dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
SET Inactive = 1
WHERE [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].inactive = 0 AND SessionID = @System_SessionID

-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 99, 'Logout')

SET NOCOUNT OFF

-- Rückgabewert
SELECT Result = -1

GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_PreValidateUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_PreValidateUser]
GO
CREATE PROCEDURE [dbo].[Public_PreValidateUser]
AS 
GO
----------------------------------------------------
-- dbo.Public_PreValidateUser
----------------------------------------------------
ALTER PROCEDURE dbo.Public_PreValidateUser
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@MaxLoginFailures int = 7

AS
-- Validates the user credentials, but doesn't log in
-- BUT: invalid credentials increase the number of login failures

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
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
DECLARE @ServerGroupID int		
DECLARE @ServerID int
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @ReAuthSuccessfull bit
DECLARE @PasswordAuthSuccessfull bit
DECLARE @Logged_ScriptEngine_SessionID nvarchar(512)

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SET NOCOUNT ON

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn
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
SELECT   @ServerGroupID = dbo.System_Servers.ServerGroup, @ServerID = ID
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0)
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
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
If @Passcode Is Null
	-- for single-sign-on scenarios
	SET @PasswordAuthSuccessfull = 1 
Else
Begin
	-- the standard case: validate username and password against the database 
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
End

-- Passwortvergleich erfolgreich?
If @PasswordAuthSuccessfull = 1
	SET @MyResult = 1 -- Zugriff gewährt
Else
	SET @MyResult = 0 -- Passwortvergleich ergab Unterschiede


IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, CAST (0 AS bit) AS CurrentlyLoggedOn FROM dbo.Benutzer WHERE LoginName = @Username
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
	END
GO


----------------------------------------------------
-- dbo.Public_RestorePassword
----------------------------------------------------
ALTER Procedure dbo.Public_RestorePassword
(
		@Username nvarchar(50),
		@eMail nvarchar(50)
)

AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	SELECT Result = (SELECT SUBSTRING(LoginPW, 1, len(LoginPW)) FROM dbo.Benutzer WHERE Loginname = @Username And [e-mail] = @eMail)
GO

----------------------------------------------------
-- [dbo].[Public_ServerDebug]
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ServerDebug
(
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32)
)

AS
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

-- Deklaration Variablen/Konstanten
DECLARE @WebApplication varchar(1024)
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime

DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @ServerGroupID int
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @ReAuthSuccessfull bit
DECLARE @RequestedServerID int
DECLARE @WebAppID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @WebApplication = 'Public'

SET NOCOUNT ON

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
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
SELECT   @ServerGroupID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
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
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID)))

------------------------------
-- UserLoginValidierung --
------------------------------

-- ReAuthentifizierung?
	-- Does the user has got authorization?
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ApplicationsRightsByGroup.ID_GroupOrPerson FROM Memberships_EffectiveRulesWithClonesNthGrade RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships_EffectiveRulesWithClonesNthGrade.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID) AND (ApplicationsRightsByGroup.ID_GroupOrPerson = @PublicGroupID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships_EffectiveRulesWithClonesNthGrade.ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships_EffectiveRulesWithClonesNthGrade.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (((Memberships_EffectiveRulesWithClonesNthGrade.ID_User = @CurUserID) AND (Applications.Title = @WebApplication))) AND Applications.LocationID = @RequestedServerID)
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade WHERE ID_User = @CurUserID AND ID_Group = 6)
		If NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
			SET @MyResult = 1 -- Zugriff gewährt
		Else
			SET @MyResult = 2 -- kein Zugriff auf aktuelles Dokument

IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		SET NOCOUNT ON
	END
Else -- @MyResult = 0 Or @MyResult = 2
	-- Login failed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2	
		SET NOCOUNT ON
	END
GO

----------------------------------------------------
-- dbo.Public_SetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_SetUserDetailData
	(
		@IDUser int,
		@Type varchar(50),
		@Value nvarchar(255),
		@DoNotLogSuccess bit = 0,
		@ModifiedBy int = 0
	)

AS
DECLARE @CountOfValuesInTable int

	set nocount on
	-- How many rows exist?
	SET @CountOfValuesInTable = (SELECT COUNT(ID) FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type)
	
	If @CountOfValuesInTable > 0 
		-- Remove old settings first
		DELETE FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
	-- Append value to table
	If @Type Is Not Null And @Value Is Not Null 
		INSERT INTO dbo.Log_Users (ID_User, Type, Value, ModifiedBy) VALUES (@IDUser, @Type, @Value, @ModifiedBy) 
	-- Logging
	if @DoNotLogSuccess = 0 
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@IDUser, GetDate(), '0.0.0.0', '0.0.0.0', -9, 'User attributes modified by user id ' + CAST(@ModifiedBy as varchar(20)))
	-- Exit
	set nocount off
	SELECT -1
	return
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UpdateUserDetails]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_UpdateUserDetails]
GO

----------------------------------------------------
-- dbo.Public_UpdateUserDetails
----------------------------------------------------
CREATE PROCEDURE dbo.Public_UpdateUserDetails 
(
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication nvarchar(1024),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures int
DECLARE @CurUserAccountAccessability int
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @Passcode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@Passcode)
			BEGIN
				IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, State = @State, Land = @Land, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -4, 'User profile changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO

----------------------------------------------------
-- dbo.Public_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE [dbo].[Public_UpdateUserPW] 
(
	@Username nvarchar(50),
	@OldPasscode varchar(4096),
	@NewPasscode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication varchar(4096),
	@LoginPWAlgorithm int = 0,
	@LoginPWNonceValue varbinary(4096) = 0x00 
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @OldPasscode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnt die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@OldPasscode)
			BEGIN
				IF ASCII(SUBSTRING(@OldPasscode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate(), LoginPwAlgorithm = @LoginPWAlgorithm, LoginPWNonceValue = @LoginPWNonceValue WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 6, 'User password changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

IF  EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UserIsAuthorized]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Public_UserIsAuthorized]
GO
----------------------------------------------------
-- dbo.Public_UserIsAuthorized
----------------------------------------------------
CREATE PROCEDURE dbo.Public_UserIsAuthorized
(
	@UserID bigint,
	@SecurityObjectID int,
	@SecurityObjectName varchar(255),
	@ServerGroupID int
)

AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SET NOCOUNT ON

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If IsNull(@UserID, 0) = 0 
	OR IsNull(@ServerGroupID, 0) = 0 
	OR 
		(
		IsNull(@SecurityObjectID, 0) = 0 
		AND IsNull(@SecurityObjectName, '') = ''
		)
	OR 
		(
		IsNull(@SecurityObjectID, 0) <> 0 
		AND IsNull(@SecurityObjectName, '') <> ''
		)
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

------------------------------
-- UserLoginValidierung --
------------------------------

If IsNull(@SecurityObjectName, '') = 'Anonymous' 
	Return 1 -- Zugriff gewährt
If IsNull(@SecurityObjectName, '') = 'Public' And @UserID <> -1
	Return 1 -- Zugriff gewährt
	
DECLARE @FoundAuthsCount int
IF @SecurityObjectID <> 0
	SELECT @FoundAuthsCount = COUNT(*)
		FROM dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules WITH (NOLOCK)
		WHERE ID_User = @UserID
			AND ID_ServerGroup = @ServerGroupID 
			AND ID_SecurityObject = @SecurityObjectID
ELSE
	SELECT @FoundAuthsCount = COUNT(*)
		FROM dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules WITH (NOLOCK)
		WHERE ID_User = @UserID
			AND ID_ServerGroup = @ServerGroupID 
			AND ID_SecurityObject IN (SELECT ID FROM dbo.Applications WITH (NOLOCK) WHERE Title = @SecurityObjectName)
If @FoundAuthsCount > 0
	Return 1 -- Zugriff gewährt
Else
	Return 0 -- kein Zugriff auf aktuelles Dokument

GO

IF  EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UserIsAuthorizedForApp]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Public_UserIsAuthorizedForApp]
GO
----------------------------------------------------
-- dbo.Public_UserIsAuthorizedForApp
----------------------------------------------------
CREATE PROCEDURE dbo.Public_UserIsAuthorizedForApp
(
	@Username nvarchar(50),
	@WebApplication varchar(255),
	@ServerIP nvarchar(32)
)

AS 
-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

DECLARE @CurUserID int
DECLARE @ServerGroupID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WITH (NOLOCK) WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT @ServerGroupID = dbo.System_ServerGroups.ID
FROM dbo.System_Servers WITH (NOLOCK)
	INNER JOIN dbo.System_ServerGroups WITH (NOLOCK)
		ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE dbo.System_Servers.IP = @ServerIP
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

------------------------------
-- UserLoginValidierung --
------------------------------

If IsNull(@WebApplication, '') = 'Anonymous' 
	Return 1 -- Zugriff gewährt
If IsNull(@WebApplication, '') = 'Public' And @CurUserID Is Not Null
	Return 1 -- Zugriff gewährt
	
DECLARE @FoundAuthsCount int
SELECT @FoundAuthsCount = COUNT(*)
	FROM dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules WITH (NOLOCK)
	WHERE ID_User = IsNull(@CurUserID, -1)
		AND ID_ServerGroup = @ServerGroupID 
		AND ID_SecurityObject IN (SELECT ID FROM dbo.Applications WITH (NOLOCK) WHERE Title = @WebApplication)
If @FoundAuthsCount > 0
	Return 1 -- Zugriff gewährt
Else
	Return 0 -- kein Zugriff auf aktuelles Dokument

GO

----------------------------------------------------
-- dbo.Public_ValidateDocument
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateDocument
(
	@Username nvarchar(50),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication nvarchar(1024),
	@WebURL nvarchar(1024),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@Reserved int = Null
)

AS

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginCount int
DECLARE @CurUserAccountAccessability int
DECLARE @ServerGroupID int
DECLARE @ServerIsAccessable int
DECLARE @RequestedServerID int
DECLARE @LoggingSuccess_Disabled bit
DECLARE @CurrentSessionID int
DECLARE @FoundFirstExistingIDApplication int

SET NOCOUNT ON

-- Reserved-Parameter auswerten
IF @Reserved = 1
	SELECT @LoggingSuccess_Disabled = 1
ELSE
	SELECT @LoggingSuccess_Disabled = 0

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If IsNull(@WebApplication,'') IN ('', 'Anonymous')
	-- No or the anonymous application title --> anonymous access allowed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, Null As LoginName, Null As System_SessionID -- access granted without logging
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
------------------------------------------------------------------------------------------
-- Anonymous user accessing a real application always required login, first --
------------------------------------------------------------------------------------------
IF @Username IS NULL AND @WebApplication IS NOT NULL
	-- Public application title --> access of any valid user is allowed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 58 -- login required
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT @ServerGroupID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
	FROM dbo.System_Servers WITH (NOLOCK)
		INNER JOIN dbo.System_ServerGroups WITH (NOLOCK)
			ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
	WHERE dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

--------------------------------------------------------------------
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
--------------------------------------------------------------------
SELECT @CurUserID = ID, 
		@CurUserLoginDisabled = LoginDisabled, 
		@CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginCount = LoginCount, 
		@CurUserAccountAccessability = AccountAccessability
	FROM dbo.Benutzer WITH (XLOCK, ROWLOCK)
	WHERE LoginName = @Username
If @Username IS NOT NULL AND @CurUserAccountAccessability Is Null
	-- Benutzer nicht gefunden
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 43
		-- Abbruch
		Return
	END

IF @CurUserID IS NOT NULL
BEGIN
	------------------------------
	-- UserLoginValidierung --
	------------------------------
	If (@CurUserLoginDisabled = 1)
		BEGIN
			-- Logging
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
				values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -28, 'Account disabled')
			-- Konto gesperrt - Rückgabewert
			SET NOCOUNT OFF
			SELECT Result = 44
			Return
		END
	If  @CurUserLoginLockedTill > GetDate()
		BEGIN
			-- Logging
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
				values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -29, 'Account locked temporary')
			-- LoginSperre aktiv - Rückgabewert
			SET NOCOUNT OFF
			SELECT Result = -2
			Return
		END
	--------------------------------------------------
	-- Server-Zugriff durch Benutzer erlaubt? --
	--------------------------------------------------
	SELECT @ServerIsAccessable = COUNT(*)
		FROM System_ServerGroupsAndTheirUserAccessLevels WITH (NOLOCK)
			INNER JOIN System_Servers WITH (NOLOCK)
				ON System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = System_Servers.ServerGroup
		WHERE System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = @CurUserAccountAccessability 
			AND System_Servers.IP = @ServerIP
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
	---------------------------------------------------------------------------------
	-- Versuch der Authentifizierung durch die mitgelieferte SessionID --
	---------------------------------------------------------------------------------
	-- Lookup internal session ID
	SELECT TOP 1 @CurrentSessionID = SessionID
		FROM System_WebAreasAuthorizedForSession WITH (NOLOCK) 
		WHERE ScriptEngine_SessionID = @ScriptEngine_SessionID 
			AND ScriptEngine_ID = @ScriptEngine_ID
			AND Server = @RequestedServerID
			AND SessionID IN (
								SELECT ID_Session 
								FROM [dbo].[System_UserSessions] WITH (NOLOCK) 
								WHERE ID_User = @CurUserID
							 )
	If @CurrentSessionID IS NULL
		BEGIN
			-- Logging
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
				values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -98, 'Browser/client session not found: ' + @ScriptEngine_SessionID + ', ReAuthSuccessfull = 0, Login denied')
			-- Rückgabewert
			SET NOCOUNT OFF
			SELECT Result = 42 -- session closed
			-- Abbruch
			Return
		END
	---------------------------------------------------------------------------------
	-- Abkürzung beim Zugriff auf Application 'Public' durch angemeldeten, validen Benutzer
	---------------------------------------------------------------------------------
	IF @WebApplication = 'Public'
		-- Public application title --> access of any valid user is allowed
		BEGIN
			-- Rückgabewert
			SET NOCOUNT OFF
			SELECT Result = -1, @Username As LoginName, @CurrentSessionID As System_SessionID -- access granted without logging
			-- Abbruch
			Return
		END
END

------------------------------
-- Test for a really exisiting app with specified name --
------------------------------
SELECT @FoundFirstExistingIDApplication = 
	(
	select top 1 ID 
	from Applications WITH (NOLOCK) 
	where Applications.Title = @WebApplication 
		AND Applications.LocationID = @RequestedServerID
	)
If @FoundFirstExistingIDApplication Is Null
	BEGIN
		SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
		PRINT 'Error resolving security object ID for logging purposes'
		RETURN
	END

------------------------------
-- UserLoginValidierung --
------------------------------
DECLARE @FoundFirstIDApplication int
DECLARE @IsDevUser bit
SELECT TOP 1 @FoundFirstIDApplication = ID_SecurityObject, @IsDevUser = IsDevRule
    FROM dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules WITH (NOLOCK)
	WHERE ID_User = IsNull(@CurUserID, -1)
		AND ID_ServerGroup = @ServerGroupID
		AND ID_SecurityObject IN (SELECT ID FROM dbo.Applications WITH (NOLOCK) WHERE Title = @WebApplication)
ORDER BY IsDevRule DESC          

----------------------------------------------------------------
-- Deactivate access logging for dev&test users --
----------------------------------------------------------------
IF IsNull(@IsDevUser, 0) <> 0
	SELECT @LoggingSuccess_Disabled = 1
    
----------------------------------------------------------------
-- Ergebnis-Rückmeldung und ordentliche Protokollierung
----------------------------------------------------------------
IF @FoundFirstIDApplication IS NOT NULL
	-- Authorization GRANTED within valid user session with a valid user
	IF @CurUserID IS NOT NULL
		-- User with login
		BEGIN
			-- LoginRemoteIP und SessionIDs setzen + LoginFailureFields zurücksetzen
			update dbo.Benutzer 
				set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), 
					LoginFailures = 0, LoginLockedTill = Null  
				where LoginName = @Username 
			If @LoggingSuccess_Disabled = 0 
				-- LoginCount hochzählen
				update dbo.Benutzer 
					set LoginCount = @CurUserLoginCount + 1 
					where LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) 
					values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @FoundFirstIDApplication, @WebURL, 0)
			-- WebAreaSessionState aktualisieren
			update dbo.System_WebAreasAuthorizedForSession 
				set LastSessionStateRefresh = getdate() 
				where ScriptEngine_ID = @ScriptEngine_ID 
					and SessionID = @CurrentSessionID 
					And Server = @RequestedServerID
			-- Rückgabewert
			SET NOCOUNT OFF
			SELECT Result = -1, LoginName, @CurrentSessionID As System_SessionID
				FROM dbo.Benutzer
				WHERE LoginName = @Username
			Return
		END	
	ELSE
		-- User without login / anonymous user
		BEGIN
			-- Rückgabewert
			SET NOCOUNT OFF
			SELECT Result = -1, Null As LoginName, Null As System_SessionID -- access granted without logging
			Return
		END
ELSE 
	-- Authorization DENIED
	BEGIN
		-- Logging - PLEASE NOTE: möglicherweise jedoch nicht exakt die angefragte/erlaubte AppID da nur Logging einer exemplarischen AppID mit gleichem/angefragtem AppName jedoch ohne entsprechende Rechte --
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) 
			values (IsNull(@CurUserID, -1), GetDate(), @ServerIP, @RemoteIP, @FoundFirstExistingIDApplication, @WebURL, -27)
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
		PRINT 'No access to current document'
		Return
	END
GO

----------------------------------------------------
-- dbo.Public_ValidateGUIDLogin
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateGUIDLogin
(
	@Username nvarchar(50),
	@GUID int,
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512)
)

AS

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
DECLARE @CurUserID int
DECLARE @CurrentSessionID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

If @CurUserID Is Null
	-- User does not exist
	Return 1

-- Lookup internal session ID
SELECT TOP 1 @CurrentSessionID = SessionID
FROM System_WebAreasAuthorizedForSession WITH (NOLOCK) 
WHERE ScriptEngine_LogonGUID = @GUID
    AND ScriptEngine_SessionID IS NULL
	AND ScriptEngine_ID = @ScriptEngine_ID
	AND Server IN (SELECT ID FROM System_Servers WHERE IP = @ServerIP AND Enabled <> 0)
	AND SessionID IN (SELECT ID_Session FROM [dbo].[System_UserSessions] WHERE ID_User = @CurUserID)
IF @CurrentSessionID IS NULL
	-- Session does not exist
	Return 2
	
UPDATE dbo.System_WebAreasAuthorizedForSession 
SET ScriptEngine_SessionID = @ScriptEngine_SessionID, LastSessionStateRefresh = GetDate()
FROM System_WebAreasAuthorizedForSession 
	INNER JOIN System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID 
	INNER JOIN System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID 
WHERE System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID = @GUID 
	AND System_WebAreasAuthorizedForSession.SessionID = @CurrentSessionID
	AND System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID
	AND System_Servers.IP = @ServerIP

IF @@ROWCOUNT = 1 
	insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
	values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 97, 'Prepare GUID login - Script Engine #' + Cast(@ScriptEngine_ID As NVarchar(20)))
Else
	-- User has no such ScriptEngine_SessionID
	Return 2

SET NOCOUNT OFF

-- Erfolgsmitteilung
Select @UserName
Return -1
GO

----------------------------------------------------
-- dbo.Public_ValidateUser
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateUser
	@Username nvarchar(50),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@ForceLogin bit,
	@MaxLoginFailures int = 7

AS

-- Keine Locks anderer Transactions berücksichtigen - immer mit dem letzten Stand arbeiten (egal ob committed oder nicht)
--SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
	
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
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
DECLARE @ServerGroupID int
DECLARE @ServerID int
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @ReAuthSuccessfull bit
DECLARE @PasswordAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Logged_ScriptEngine_SessionID nvarchar(512)
DECLARE @CurrentSessionID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SET NOCOUNT ON

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn
FROM dbo.Benutzer  WITH (XLOCK, ROWLOCK)
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
SELECT   @ServerGroupID = dbo.System_Servers.ServerGroup, @ServerID = ID
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0)
IF @ServerGroupID Is Null 
	SELECT @ServerGroupID = 0
If @ServerGroupID = 0
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
If @Passcode Is Null
	-- for single-sign-on scenarios
	SET @PasswordAuthSuccessfull = 1 
Else
Begin
	-- the standard case: validate username and password against the database 
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
End

-- Passwortvergleich erfolgreich?
If @PasswordAuthSuccessfull = 1
	SET @MyResult = 1 -- Zugriff gewährt
Else
	SET @MyResult = 0 -- Passwortvergleich ergab Unterschiede


IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Überprüfung auf bereits vorhandene User Session mit gleicher ScriptEngineSessionID
		-- Lookup internal session ID
		SELECT TOP 1 @CurrentSessionID = SessionID
		FROM System_WebAreasAuthorizedForSession WITH (NOLOCK) 
		WHERE ScriptEngine_SessionID = @ScriptEngine_SessionID 
			AND ScriptEngine_ID = @ScriptEngine_ID
			AND Server = @ServerID
			AND SessionID IN (SELECT ID_Session FROM [dbo].[System_UserSessions] WITH (NOLOCK) WHERE ID_User = @CurUserID)
		If @CurrentSessionID IS NOT NULL
			-- Abbruch der bisherigen User Session, da gleich neue User Session erstellt wird
			UPDATE dbo.System_WebAreasAuthorizedForSession
			SET Inactive = 1
			WHERE SessionID = @CurrentSessionID
		
		-- Wenn @CurUserLoginLockedTill vorhanden und älter als aktuelles Systemdatum, LoginFailureFields zurücksetzen
		If Not (@CurUserLoginLockedTill > GetDate())
			-- Password check successful --> LoginFailures = 0
			update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- LoginCount hochzählen, LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1, LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 98, 'Validation of login data successfull')
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = SCOPE_IDENTITY()
		-- following action if not a client access through a stand-alone application
		-- (offline applications should not disturb any online sessions)
		if @ScriptEngine_ID = -1 -- Net client (stand-alone)
			BEGIN
				-- An welchen Systemen ist noch eine Anmeldung erforderlich?
				INSERT INTO dbo.System_WebAreasAuthorizedForSession
				                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID, ScriptEngine_SessionID)
				SELECT     dbo.System_Servers.ServerGroup, dbo.System_servers.id, 
				                      -1, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID, @ScriptEngine_SessionID
				FROM         dbo.System_Servers 
				WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ID = @ServerID)
			END
		else -- Web clients
			BEGIN
				-- LoginRemoteIP 
				update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate() where LoginName = @Username
				-- An welchen Systemen ist noch eine Anmeldung erforderlich?
				INSERT INTO dbo.System_WebAreasAuthorizedForSession
				                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID, ScriptEngine_SessionID)
				SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
				                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
									  ,CASE WHEN dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine = @ScriptEngine_ID 
												AND dbo.System_WebAreaScriptEnginesAuthorization.Server = @ServerID THEN @ScriptEngine_SessionID ELSE NULL END
				FROM         dbo.System_Servers INNER JOIN
				                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
				WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @ServerGroupID)
				-- Ggf. weitere Sessions schließen, welche noch offen sind und von der gleichen Browsersession geöffnet wurden
				-- Konkreter Beispielfall: 
				-- 2 Browserfenster wurden geöffnet, die Cookies und damit die Sessions sind die gleichen; 
				-- in beiden Browsern wurde zeitnah eine Anmeldung unterschiedlicher Benutzer ausgeführt und damit 2 Sessions erstellt, 
				-- wobei die erste Session durch ein Logout NUN zu schließen ist, die zweite Session aber geöffnet bleibt
				update [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
				set inactive = 1
				where [System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes].inactive = 0 
					AND sessionid in 
					(
						SELECT SessionID
						FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes WITH (NOLOCK) 
						WHERE ScriptEngine_SessionID = @ScriptEngine_SessionID 
							AND ScriptEngine_ID = @ScriptEngine_ID
							AND Server = @ServerID
							AND Inactive = 0
							AND SessionID IN (SELECT ID_Session FROM [dbo].[System_UserSessions] WITH (NOLOCK) WHERE ID_User <> @CurUserID)
					)
				DELETE 
				FROM System_SessionValues
				WHERE SessionID NOT IN 
					-- list of valid SessionIDs
					(
					SELECT SessionID 
					FROM System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
					WHERE Inactive = 0
					)
			END
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, LoginName, @CurUserStatus_InternalSessionID FROM dbo.Benutzer WHERE LoginName = @Username
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


---------------------------------------
-- Update SP "Redirects_LogAndGetURL" 
---------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Redirects_LogAndGetURL]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Redirects_LogAndGetURL]
GO


CREATE PROCEDURE dbo.Redirects_LogAndGetURL
	(
		@IDRedirector int
	)

AS 

SET NOCOUNT ON

-- Log
INSERT INTO dbo.Redirects_Log
                      (ID_Redirector)
VALUES     (@IDRedirector)


-- Increase the ticker 
UPDATE dbo.Redirects_ToAddr
SET NumberOfRedirections = NumberOfRedirections + 1
WHERE ID = @IDRedirector


SET NOCOUNT OFF

-- Get URL
SELECT RedirectTo
FROM dbo.Redirects_ToAddr
WHERE ID = @IDRedirector

GO
-----------------------------------------------------------
--Insert data into security objects and authorizations: Redirection Admin Area
-----------------------------------------------------------
DECLARE @ModifiedBy int
SELECT @ModifiedBy = -43
DECLARE @AppID_Redirections int, @AdminServerID int
SELECT @AppID_Redirections = ID FROM dbo.Applications_CurrentAndInactiveOnes WHERE Title = 'System - Administration - Redirections'
IF @AppID_Redirections IS NULL
BEGIN
	-- Retrieve one server ID
	SELECT TOP 1 @AdminServerID = System_Servers.ID
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- 1st security object
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@AdminServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
	SELECT @AppID_Redirections = SCOPE_IDENTITY()
	-- Rest of new security objects - same languages (English)
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,System_Servers.ID,1,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	WHERE System_Servers.ID <> @AdminServerID
	GROUP BY System_Servers.ID
	-- Rest of new security objects - other languages
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Weiterleitungen',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,System_Servers.ID,2,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- Authorizations
	INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Redirections ,6,getdate(),@ModifiedBy)
END
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[RefillSplittedSecObjAndNavPointsTables]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[RefillSplittedSecObjAndNavPointsTables]
GO

GO
IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationRights_CumulatedPerUserAndServerGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.ApplicationRights_CumulatedPerUserAndServerGroup
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Public_EnsureSession]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[Public_EnsureSession]
GO
----------------------------------------------------
-- dbo.Public_EnsureSession
----------------------------------------------------
CREATE PROCEDURE [dbo].[Public_EnsureSession]
(
	@CurUserID int,
	@ServerID int,
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512)
)

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurrentSessionID int

SET NOCOUNT ON

	---------------------------------------------------------------------------------
	-- Versuch der Auffindung einer vorhandenen Session
	---------------------------------------------------------------------------------
	-- Lookup internal session ID
	SELECT TOP 1 @CurrentSessionID = SessionID
		FROM System_WebAreasAuthorizedForSession WITH (NOLOCK) 
		WHERE ScriptEngine_SessionID = @ScriptEngine_SessionID 
			AND ScriptEngine_ID = @ScriptEngine_ID
			AND Server = @ServerID
			AND SessionID IN (
								SELECT ID_Session 
								FROM [dbo].[System_UserSessions] WITH (NOLOCK) 
								WHERE ID_User = @CurUserID
							 )
	If @CurrentSessionID IS NULL
		BEGIN
			-- Interne SessionID erstellen
			INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
			SELECT @CurrentSessionID = SCOPE_IDENTITY()
			-- An welchen Systemen ist noch eine Anmeldung erforderlich?
			INSERT INTO dbo.System_WebAreasAuthorizedForSession
									(ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID, ScriptEngine_SessionID)
			SELECT     dbo.System_Servers.ServerGroup, dbo.System_servers.id, @ScriptEngine_ID, @CurrentSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID, @ScriptEngine_SessionID
			FROM         dbo.System_Servers 
			WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ID = @ServerID)
		END

-- return the current/new inernal session ID
SET NOCOUNT OFF
SELECT @CurrentSessionID;

GO