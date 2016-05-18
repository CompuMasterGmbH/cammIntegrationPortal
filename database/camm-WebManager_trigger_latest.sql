-- dropped concept -> dropped trigger
IF OBJECT_ID ('dbo.IUD_Apps2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_Apps2SecObjAndNavItems;
GO
------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[Applications_CurrentAndInactiveOnes] - TRIGGER dbo.IUD_Apps2SecObjAndNavItems
------------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.IUD_Applications', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_Applications;
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRights_Inheriting]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [dbo].[IUD_Applications] 
   ON  [dbo].[Applications_CurrentAndInactiveOnes] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	-- Update table dbo.[ApplicationsRights_Inheriting]
	DELETE dbo.[ApplicationsRights_Inheriting]
	FROM dbo.[ApplicationsRights_Inheriting]
		INNER JOIN deleted
			ON dbo.[ApplicationsRights_Inheriting].ID_Inheriting = deleted.ID
				AND dbo.[ApplicationsRights_Inheriting].ID_Source = deleted.AuthsAsAppID;
	INSERT INTO dbo.[ApplicationsRights_Inheriting] (ID_Inheriting, ID_Source, ReleasedBy, ModifiedBy)
	SELECT ID, AuthsAsAppID, ModifiedBy, ModifiedBy
	FROM inserted
	WHERE AppDeleted = 0 AND AuthsAsAppID IS NOT NULL;
	-- Update table dbo.ApplicationsRightsByGroup to always allow Supervisors
	DELETE dbo.[ApplicationsRightsByGroup]
	FROM dbo.[ApplicationsRightsByGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup].ID_Application = deleted.ID
				AND dbo.[ApplicationsRightsByGroup].ID_ServerGroup = 0
				AND dbo.[ApplicationsRightsByGroup].ID_GroupOrPerson = 6
				AND dbo.[ApplicationsRightsByGroup].DevelopmentTeamMember = 1
				AND dbo.[ApplicationsRightsByGroup].IsDenyRule = 0;
	INSERT INTO dbo.[ApplicationsRightsByGroup] (ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, ReleasedBy)
	SELECT ID, 0, 6, 1, 0, IsNull(inserted.ModifiedBy, inserted.ReleasedBy)
	FROM inserted
	WHERE AppDeleted = 0
END' 
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRights_Inheriting]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
-- Force first table filling (or full update on repetitions)
UPDATE [dbo].[Applications_CurrentAndInactiveOnes] 
SET TitleAdminArea = TitleAdminArea
GO

------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[ApplicationsRightsByGroup] - TRIGGER dbo.IUD_AuthsGroups2PreStagingForServerGroup
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsGroups2PreStagingForServerGroup]'))
DROP TRIGGER [dbo].[IUD_AuthsGroups2PreStagingForServerGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsGroups2PreStagingForServerGroup]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[IUD_AuthsGroups2PreStagingForServerGroup] 
   ON  [dbo].[ApplicationsRightsByGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup = 0
	DELETE dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].ID_SecurityObject = deleted.ID_Application
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].ID_Group = deleted.ID_GroupOrPerson
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsDevRule = deleted.DevelopmentTeamMember
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsDenyRule = deleted.IsDenyRule
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsServerGroup0Rule <> 0
	WHERE deleted.ID_ServerGroup = 0;
	-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup <> 0
	DELETE dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].ID_SecurityObject = deleted.ID_Application
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].ID_ServerGroup = deleted.ID_ServerGroup
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].ID_Group = deleted.ID_GroupOrPerson
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsDevRule = deleted.DevelopmentTeamMember
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsDenyRule = deleted.IsDenyRule
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsServerGroup0Rule = 0
	WHERE deleted.ID_ServerGroup <> 0;
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup = 0
	INSERT INTO dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule)
	SELECT ID_Application, dbo.System_ServerGroups.ID AS ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 1
	FROM inserted
        CROSS JOIN dbo.System_ServerGroups
	WHERE inserted.ID_ServerGroup = 0
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup <> 0
	INSERT INTO dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule)
	SELECT ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 0
	FROM inserted
	WHERE inserted.ID_ServerGroup <> 0
END
' 
GO
-- Force first table filling (or full update on repetitions)
  update [ApplicationsRightsByGroup]
  set IsDenyRule = IsDenyRule
GO

------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[ApplicationsRightsByUser] - TRIGGER dbo.IUD_AuthsUsers2PreStagingForServerGroup
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsUsers2PreStagingForServerGroup]'))
DROP TRIGGER [dbo].[IUD_AuthsUsers2PreStagingForServerGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsUsers2PreStagingForServerGroup]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[IUD_AuthsUsers2PreStagingForServerGroup] 
   ON  [dbo].[ApplicationsRightsByUser] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup = 0
	DELETE dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_SecurityObject = deleted.ID_Application
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_User = deleted.ID_GroupOrPerson
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsDevRule = deleted.DevelopmentTeamMember
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsDenyRule = deleted.IsDenyRule
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsServerGroup0Rule <> 0
	WHERE deleted.ID_ServerGroup = 0;
	-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup <> 0
	DELETE dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_SecurityObject = deleted.ID_Application
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_ServerGroup = deleted.ID_ServerGroup
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_User = deleted.ID_GroupOrPerson
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsDevRule = deleted.DevelopmentTeamMember
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsDenyRule = deleted.IsDenyRule
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsServerGroup0Rule = 0
	WHERE deleted.ID_ServerGroup <> 0;
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup = 0
	INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule)
	SELECT ID_Application, dbo.System_ServerGroups.ID AS ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 1
	FROM inserted
        CROSS JOIN dbo.System_ServerGroups
	WHERE inserted.ID_ServerGroup = 0
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup <> 0
	INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule)
	SELECT ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 0
	FROM inserted
	WHERE inserted.ID_ServerGroup <> 0
END
' 
GO
-- Force first table filling (or full update on repetitions)
  update [ApplicationsRightsByUser]
  set IsDenyRule = IsDenyRule
GO

------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[System_ServerGroups] - TRIGGER dbo.IUD_ServerGroupAuths2PreStagingForServerGroup
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup]'))
DROP TRIGGER [dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup]'))
EXEC dbo.sp_executesql @statement = N'
CREATE TRIGGER [dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup] 
   ON  [dbo].[System_ServerGroups] 
   FOR INSERT,DELETE
AS 
BEGIN
DECLARE @AFirstServerGroupID int
SELECT TOP 1 @AFirstServerGroupID = ID FROM [dbo].[System_ServerGroups] WHERE ID NOT IN (SELECT ID FROM inserted)

	SET NOCOUNT ON;
	-- Drop all related app-rights: USERS
	DELETE dbo.[ApplicationsRightsByUser]
	FROM dbo.[ApplicationsRightsByUser]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByUser].ID_ServerGroup = deleted.ID
	-- Drop all related app-rights: GROUPS
	DELETE dbo.[ApplicationsRightsByGroup]
	FROM dbo.[ApplicationsRightsByGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup].ID_ServerGroup = deleted.ID
	-- Insert required pre-staging data for inserted ServerGroup to complete pre-stage-data for CROSS JOIN for auths with ID_ServerGroup = 0
	IF @AFirstServerGroupID IS NOT NULL
		BEGIN
			-- clone group auths from a first, existing server group
			INSERT INTO dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule)
			SELECT ID_SecurityObject, inserted.ID, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule
			FROM dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
				CROSS JOIN inserted
			WHERE ID_ServerGroup = @AFirstServerGroupID AND IsServerGroup0Rule <> 0
			-- clone user auths from a first, existing server group
			INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule)
			SELECT ID_SecurityObject, inserted.ID, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule
			FROM dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
				CROSS JOIN inserted
			WHERE ID_ServerGroup = @AFirstServerGroupID AND IsServerGroup0Rule <> 0
		END 
END
' 
GO

------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup] - TRIGGER dbo.IUD_AuthsByGroup_PreStagingForRealServerGroup_SyncAnonymousUser
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsByGroup_PreStagingForRealServerGroup_SyncAnonymousUser]'))
DROP TRIGGER [dbo].[IUD_AuthsByGroup_PreStagingForRealServerGroup_SyncAnonymousUser]
GO
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsByGroup_PreStagingForRealServerGroup_SyncAnonymousUser]'))
EXEC dbo.sp_executesql @statement = N'
-- Forward ALL auth changes (DROP+INSERT) for anonymous group to anonymous user
CREATE TRIGGER [dbo].[IUD_AuthsByGroup_PreStagingForRealServerGroup_SyncAnonymousUser] 
   ON  [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	-- Forward-Drop all anonymous app-rights
	DELETE [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]
	FROM [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_ServerGroup = deleted.ID_ServerGroup
				AND [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_User = -1
				AND [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_SecurityObject = deleted.ID_SecurityObject
				AND [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsServerGroup0Rule = deleted.IsServerGroup0Rule
				AND [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsDenyRule = deleted.IsDenyRule
				AND [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsDevRule = deleted.IsDevRule
	WHERE deleted.ID_Group IN (SELECT dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups)
	-- Forward-Insert required pre-staging data for anonymous app-rights
	INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule)
	SELECT ID_SecurityObject, ID_ServerGroup, -1, IsDevRule, IsDenyRule, IsServerGroup0Rule
	FROM inserted
		INNER JOIN dbo.System_ServerGroups 
			ON inserted.ID_Group = dbo.System_ServerGroups.ID_Group_Anonymous
END
'
GO
------------------------------------------------------------------------------------------------------------

IF OBJECT_ID ('dbo.IUD_AuthsUsers2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_AuthsUsers2SecObjAndNavItems;
GO
CREATE TRIGGER [dbo].[IUD_AuthsUsers2SecObjAndNavItems] 
   ON  [dbo].[ApplicationsRightsByUser] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
    -- DEACTIVATED BY JOCHEN WEZEL ON 2013/2014 BECAUSE OF TOO MANY DEADLOCKS ERRORS - TO BE REACTIVATED WHEN STABLE: EXEC RefillSplittedSecObjAndNavPointsTables
END
GO

IF OBJECT_ID ('dbo.IUD_AuthsGroups2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_AuthsGroups2SecObjAndNavItems;
GO

IF OBJECT_ID ('dbo.Benutzer_PostUserDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Benutzer_PostUserDeletionTrigger;
GO
CREATE TRIGGER dbo.Benutzer_PostUserDeletionTrigger
   ON [dbo].Benutzer
   AFTER DELETE
AS 
BEGIN
	--Remove references of deleted users
	DELETE FROM dbo.Memberships WHERE ID_User IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.System_UserSessions WHERE ID_USER IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.System_SubSecurityAdjustments WHERE UserID IN ( SELECT ID FROM deleted ) 

	--Log the fact that user has been deleted.
	INSERT INTO dbo.Log_Users 
	(ID_USER, Type, VALUE, ModifiedOn)
	SELECT ID, 'DeletedOn', GetDate(), GETDATE() FROM deleted
END
GO
IF OBJECT_ID ('dbo.Gruppen_PostUserDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Gruppen_PostUserDeletionTrigger;
GO
CREATE TRIGGER dbo.Gruppen_PostUserDeletionTrigger
   ON [dbo].Gruppen
   AFTER DELETE
AS 
BEGIN
	--Remove references of deleted users
	DELETE FROM dbo.Memberships WHERE ID_Group IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_GroupOrPerson IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.System_SubSecurityAdjustments WHERE TableName = 'Groups' AND TablePrimaryIDValue IN ( SELECT ID FROM deleted ) 
END
GO

IF OBJECT_ID ('dbo.Applications_PostAppDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Applications_PostAppDeletionTrigger;
GO
CREATE TRIGGER dbo.Applications_PostAppDeletionTrigger
   ON [dbo].Applications_CurrentAndInactiveOnes
   AFTER DELETE
AS 
BEGIN
	--Remove references of deleted users
	DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_Application IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_Application IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.System_SubSecurityAdjustments WHERE TableName = 'Applications' AND TablePrimaryIDValue IN ( SELECT ID FROM deleted ) 
END