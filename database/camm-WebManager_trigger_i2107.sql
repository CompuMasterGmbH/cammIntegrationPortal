-- dropped concept -> dropped trigger
IF OBJECT_ID ('dbo.IUD_Apps2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_Apps2SecObjAndNavItems;
GO
-- =========================================================================================================
-- ===== SUPERVISOR'S ALWAYS ACCESS RULE
-- =========================================================================================================
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
	INSERT INTO dbo.[ApplicationsRightsByGroup] (ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, ReleasedBy, [IsSupervisorAutoAccessRule])
	SELECT ID, 0, 6, 1, 0, IsNull(inserted.ModifiedBy, inserted.ReleasedBy), 1
	FROM inserted
	WHERE AppDeleted = 0
END' 
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRights_Inheriting]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
-- Force first table filling (or full update on repetitions)
UPDATE [dbo].[Applications_CurrentAndInactiveOnes] 
SET TitleAdminArea = TitleAdminArea
GO

-- =========================================================================================================
-- ===== AUTHORIZATIONS TABLES - PRE-STAGED/PRE-CALCULATED, EFFECTIVE
-- =========================================================================================================
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
			ON dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].[DerivedFromAppRightsID] = deleted.ID
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsServerGroup0Rule <> 0
	WHERE deleted.ID_ServerGroup = 0;
	-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup <> 0
	DELETE dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].DerivedFromAppRightsID = deleted.ID
				AND dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].IsServerGroup0Rule = 0
	WHERE deleted.ID_ServerGroup <> 0;
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup = 0
	INSERT INTO dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
	SELECT ID_Application, dbo.System_ServerGroups.ID AS ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 1, inserted.ID
	FROM inserted
        CROSS JOIN dbo.System_ServerGroups
	WHERE inserted.ID_ServerGroup = 0
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup <> 0
	INSERT INTO dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
	SELECT ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 0, inserted.ID
	FROM inserted
	WHERE inserted.ID_ServerGroup <> 0
END
' 
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
			ON dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].DerivedFromUserAppRightsID = deleted.ID
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsServerGroup0Rule <> 0
	WHERE deleted.ID_ServerGroup = 0;
	-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup <> 0
	DELETE dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].DerivedFromUserAppRightsID = deleted.ID
				AND dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].IsServerGroup0Rule = 0
	WHERE deleted.ID_ServerGroup <> 0;
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup = 0
	INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromUserAppRightsID, DerivedFromGroupAppRightsID, [DerivedFromGroupAppRightsPreStagingForRealServerGroup_ID])
	SELECT ID_Application, dbo.System_ServerGroups.ID AS ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 1, inserted.ID, NULL, NULL
	FROM inserted
        CROSS JOIN dbo.System_ServerGroups
	WHERE inserted.ID_ServerGroup = 0
	-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup <> 0
	INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromUserAppRightsID, DerivedFromGroupAppRightsID, [DerivedFromGroupAppRightsPreStagingForRealServerGroup_ID])
	SELECT ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 0, inserted.ID, NULL, NULL
	FROM inserted
	WHERE inserted.ID_ServerGroup <> 0
END
' 
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
   -- NOT FOR UPDATE since applications rights would be deleted 
AS 
BEGIN
DECLARE @AFirstServerGroupID int
SELECT TOP 1 @AFirstServerGroupID = ID FROM [dbo].[System_ServerGroups] WHERE ID NOT IN (SELECT ID FROM inserted)

	SET NOCOUNT ON;
	-- Drop all directly related app-rights: USERS
	DELETE dbo.[ApplicationsRightsByUser]
	FROM dbo.[ApplicationsRightsByUser]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByUser].ID_ServerGroup = deleted.ID
	-- Drop all directly related app-rights: GROUPS
	DELETE dbo.[ApplicationsRightsByGroup]
	FROM dbo.[ApplicationsRightsByGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup].ID_ServerGroup = deleted.ID
	-- Drop all indirectly related app-rights (ApplicationsRightsByUser_PreStagingForRealServerGroup): USERS
	DELETE dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_ServerGroup = deleted.ID
	-- Drop all indirectly related app-rights (ApplicationsRightsByGroup_PreStagingForRealServerGroup): GROUPS
	DELETE dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
	FROM dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
		INNER JOIN deleted
			ON dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup].ID_ServerGroup = deleted.ID
	-- Drop public groups
	DELETE dbo.[Gruppen]
	FROM dbo.[Gruppen]
		INNER JOIN deleted
			ON dbo.[Gruppen].ID = deleted.ID_Group_Public
	-- Drop anonymous groups
	DELETE dbo.[Gruppen]
	FROM dbo.[Gruppen]
		INNER JOIN deleted
			ON dbo.[Gruppen].ID = deleted.ID_Group_Anonymous
	-- Insert required pre-staging data for inserted ServerGroup to complete pre-stage-data for CROSS JOIN for auths with ID_ServerGroup = 0
	IF @AFirstServerGroupID IS NOT NULL
		BEGIN
			-- clone group auths from a first, existing server group
			INSERT INTO dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
			SELECT ID_SecurityObject, inserted.ID, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID
			FROM dbo.[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
				CROSS JOIN inserted
			WHERE ID_ServerGroup = @AFirstServerGroupID AND IsServerGroup0Rule <> 0
			-- clone user auths from a first, existing server group
			INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromUserAppRightsID, DerivedFromGroupAppRightsID, [DerivedFromGroupAppRightsPreStagingForRealServerGroup_ID])
			SELECT ID_SecurityObject, inserted.ID, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromUserAppRightsID, DerivedFromGroupAppRightsID, [DerivedFromGroupAppRightsPreStagingForRealServerGroup_ID]
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
			ON [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].[DerivedFromGroupAppRightsPreStagingForRealServerGroup_ID] = deleted.ID
				AND [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup].ID_User = -1
	WHERE deleted.ID_Group IN (SELECT dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups)
	-- Forward-Insert required pre-staging data for anonymous app-rights
	INSERT INTO dbo.[ApplicationsRightsByUser_PreStagingForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromUserAppRightsID, DerivedFromGroupAppRightsID, [DerivedFromGroupAppRightsPreStagingForRealServerGroup_ID])
	SELECT ID_SecurityObject, ID_ServerGroup, -1, IsDevRule, IsDenyRule, IsServerGroup0Rule, NULL, DerivedFromAppRightsID, inserted.ID
	FROM inserted
		INNER JOIN dbo.System_ServerGroups 
			ON inserted.ID_Group = dbo.System_ServerGroups.ID_Group_Anonymous
END
'
GO
-- Force first table filling (or full update on repetitions)
  update [ApplicationsRightsByGroup]
  set IsDenyRule = IsDenyRule
GO
-- Force first table filling (or full update on repetitions)
  update [ApplicationsRightsByUser]
  set IsDenyRule = IsDenyRule
GO

-- =========================================================================================================
-- ===== MEMBERSHIPS TABLES - PRE-STAGED/PRE-CALCULATED, EFFECTIVE
-- =========================================================================================================
------------------------------------------------------------------------------------------------------------
-- RE-CREATION OF DEFAULT MEMBERSHIPS BASED ON USER ACCESS LEVEL AND PUBLIC+ANONYMOUS GROUPS OF RELATED SERVER GROUPS
------------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.D_BenutzerDefaultMemberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.D_BenutzerDefaultMemberships;
   -- job is already done at [Benutzer_PostUserDeletionTrigger]
GO
IF OBJECT_ID ('dbo.IU_BenutzerDefaultMemberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IU_BenutzerDefaultMemberships;
GO
CREATE TRIGGER [dbo].[IU_BenutzerDefaultMemberships] 
   ON  [dbo].[Benutzer] 
   FOR INSERT,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @UsersWithModifiedAccessLevel table (ID int); --means updated users with changed access level OR inserted users
	INSERT INTO @UsersWithModifiedAccessLevel (ID)
	SELECT inserted.ID
	FROM inserted
		LEFT JOIN deleted ON inserted.ID = deleted.ID
	WHERE inserted.AccountAccessability <> deleted.AccountAccessability
	-- reset all system rules (=rules for anonymous+public groups)
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRule <> 0
		AND ID_User IN (SELECT ID FROM @UsersWithModifiedAccessLevel)
	-- insert public groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	WHERE [dbo].[Benutzer].ID IN (SELECT ID FROM @UsersWithModifiedAccessLevel)
	GROUP BY [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID
	-- insert anonymous groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	WHERE [dbo].[Benutzer].ID IN (SELECT ID FROM @UsersWithModifiedAccessLevel)
	GROUP BY [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID
END
GO
IF OBJECT_ID ('dbo.IUD_System_ServerGroupsAndTheirUserAccessLevels_ForDefaultMemberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_System_ServerGroupsAndTheirUserAccessLevels_ForDefaultMemberships;
GO
CREATE TRIGGER [dbo].[IUD_System_ServerGroupsAndTheirUserAccessLevels_ForDefaultMemberships] 
   ON  [dbo].[System_ServerGroupsAndTheirUserAccessLevels] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	-- on delete: reset all system rules (=rules for anonymous+public groups) of related users and re-create them
	DECLARE @Users table (ID int);
	INSERT INTO @Users (ID)
	SELECT dbo.Benutzer.ID
	FROM dbo.Benutzer
	WHERE dbo.Benutzer.AccountAccessability IN 
		(
			SELECT ID 
			FROM deleted
			UNION ALL 
			SELECT ID 
			FROM inserted -- only applies on row update (real new inserts haven't got any relations to users, yet)
		)
	-- reset all system rules (=rules for anonymous+public groups)
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRule <> 0
		AND ID_User IN (SELECT ID FROM @Users)
	-- insert public groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	WHERE [dbo].[Benutzer].ID IN (SELECT ID FROM @Users)
	GROUP BY [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID
	-- insert anonymous groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	WHERE [dbo].[Benutzer].ID IN (SELECT ID FROM @Users)
	GROUP BY [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID
END
GO
IF OBJECT_ID ('dbo.IUD_System_ServerGroups_ForAnonymousDefaultMemberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_System_ServerGroups_ForAnonymousDefaultMemberships;
GO
CREATE TRIGGER [dbo].[IUD_System_ServerGroups_ForAnonymousDefaultMemberships] 
   ON  [dbo].[System_ServerGroups] 
   FOR INSERT,UPDATE,DELETE
AS 
BEGIN
	SET NOCOUNT ON;
	-- reset all system rules for anonymous group of changed server groups
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRule <> 0
		AND ID_User = -1
		AND 
			(
				ID_Group IN (SELECT ID_Group_Anonymous FROM inserted)
				OR ID_Group IN (SELECT ID_Group_Anonymous FROM deleted)
			)
	-- insert anonymous groups memberships to user with ID -1
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT ID_Group_Anonymous, -1, GETDATE(), -43, 0, 1
	FROM inserted
	GROUP BY ID_Group_Anonymous
END
GO
IF OBJECT_ID ('dbo.U_System_ServerGroups_ForDefaultMemberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.U_System_ServerGroups_ForDefaultMemberships;
GO
CREATE TRIGGER [dbo].[U_System_ServerGroups_ForDefaultMemberships] 
   ON  [dbo].[System_ServerGroups] 
   FOR UPDATE
   -- FOR INSERT: NOT REQUIRED since there haven't been any related users, yet
   -- FOR DELETE: NOT REQUIRED since user memberships update is fired by nested trigger at table [System_ServerGroupsAndTheirUserAccessLevels]
AS 
BEGIN
	SET NOCOUNT ON;
	-- reset all system rules for users with access (=rules for anonymous+public groups)
	DECLARE @Users table (ID int);
	INSERT INTO @Users (ID)
	SELECT dbo.Benutzer.ID
	FROM dbo.Benutzer
		INNER JOIN dbo.System_ServerGroupsAndTheirUserAccessLevels ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
	WHERE dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup IN 
		(
			-- select ServerGroupIDs where a change happened at public or anonymous group ID
			SELECT deleted.ID 
			FROM deleted
				INNER JOIN inserted ON deleted.ID = inserted.ID
			WHERE deleted.ID_Group_Public <> inserted.ID_Group_Public
				OR deleted.ID_Group_Anonymous <> inserted.ID_Group_Anonymous
		)
	-- reset all system rules (=rules for anonymous+public groups)
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRule <> 0
		AND ID_User IN (SELECT ID FROM @Users)
	-- insert public groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	WHERE [dbo].[Benutzer].ID IN (SELECT ID FROM @Users)
	GROUP BY [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID
	-- insert anonymous groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	WHERE [dbo].[Benutzer].ID IN (SELECT ID FROM @Users)
	GROUP BY [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID
END
GO
-- =========================================================================================================
-- ===== CONTINUEOUS DATA INTEGRITY: on DELETE, do the required cleanup on depending foreign key rows
-- =========================================================================================================
------------------------------------------------------------------------------------------------------------
-- CLEAN UP TABLE System_ServerGroupsAndTheirUserAccessLevels ON DELETED FOREIGN KEYS
------------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.D_System_AccessLevels_FK_Check', 'TR') IS NOT NULL
   DROP TRIGGER dbo.D_System_AccessLevels_FK_Check;
GO
CREATE TRIGGER [dbo].[D_System_AccessLevels_FK_Check] 
   ON  [dbo].[System_AccessLevels] 
   FOR DELETE
   -- FOR INSERT: NOT REQUIRED since there haven't been any relations, yet
   -- FOR UPDATE: NOT REQUIRED since relations don't change in ID value
AS 
BEGIN
	SET NOCOUNT ON;
	-- removal prohibited as long as AcccessLevel is in use
	DECLARE @FirstRelationEntry int;
	SELECT TOP 1 @FirstRelationEntry  = ID
	FROM dbo.[System_ServerGroups]
	WHERE dbo.[System_ServerGroups].[AccessLevel_Default] IN (SELECT ID FROM deleted);
	IF @FirstRelationEntry IS NOT NULL
		BEGIN
			RAISERROR ('Access level still referenced as default access level by 1 or more server groups', 16, 1)
			ROLLBACK TRANSACTION 		
			RETURN	
		END	
	SELECT TOP 1 @FirstRelationEntry  = NULL
	SELECT TOP 1 @FirstRelationEntry  = ID
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
	WHERE dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel IN (SELECT ID FROM deleted);
	IF @FirstRelationEntry IS NOT NULL
		BEGIN
			RAISERROR ('Access level still referenced by 1 or more server groups', 16, 2)
			ROLLBACK TRANSACTION 		
			RETURN	
		END	
	SELECT TOP 1 @FirstRelationEntry  = NULL
	SELECT TOP 1 @FirstRelationEntry 
	FROM dbo.Benutzer
	WHERE dbo.Benutzer.AccountAccessability IN (SELECT ID FROM deleted);
	IF @FirstRelationEntry IS NOT NULL
		BEGIN
			RAISERROR ('Access level still referenced by 1 or more users', 16, 3)
			ROLLBACK TRANSACTION 		
			RETURN	
		END	
END
GO

IF OBJECT_ID ('dbo.D_System_ServerGroups_FK_ServerGroupsAndTheirUserAccessLevels', 'TR') IS NOT NULL
   DROP TRIGGER dbo.D_System_ServerGroups_FK_ServerGroupsAndTheirUserAccessLevels;
GO
IF OBJECT_ID ('dbo.D_System_ServerGroups_FKs', 'TR') IS NOT NULL
   DROP TRIGGER dbo.D_System_ServerGroups_FKs;
GO
CREATE TRIGGER [dbo].[D_System_ServerGroups_FKs] 
   ON  [dbo].[System_ServerGroups] 
   FOR DELETE
   -- FOR INSERT: NOT REQUIRED since there haven't been any relations, yet
   -- FOR UPDATE: NOT REQUIRED since relations don't change in ID value
AS 
BEGIN
	SET NOCOUNT ON;
	-- drop all relations to access levels
	DELETE dbo.System_ServerGroupsAndTheirUserAccessLevels
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels 
		INNER JOIN deleted ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = deleted.ID
	-- drop all related servers
	DELETE dbo.System_Servers
	FROM dbo.System_Servers 
		INNER JOIN deleted ON dbo.System_Servers.ServerGroup = deleted.ID
END
GO

IF OBJECT_ID ('dbo.D_System_Servers_FKs', 'TR') IS NOT NULL
   DROP TRIGGER dbo.D_System_Servers_FKs;
GO
CREATE TRIGGER [dbo].[D_System_Servers_FKs] 
   ON  [dbo].[System_Servers] 
   FOR DELETE
   -- FOR INSERT: NOT REQUIRED since there haven't been any relations, yet
   -- FOR UPDATE: NOT REQUIRED since relations don't change in ID value
AS 
BEGIN
	SET NOCOUNT ON;
	-- drop all related security objects/applications
	DELETE dbo.Applications_CurrentAndInactiveOnes
	FROM dbo.Applications_CurrentAndInactiveOnes 
		INNER JOIN deleted ON dbo.Applications_CurrentAndInactiveOnes.LocationID = deleted.ID
	-- drop all script engine relations 
	DELETE System_WebAreaScriptEnginesAuthorization
	FROM System_WebAreaScriptEnginesAuthorization 
		INNER JOIN deleted ON dbo.System_WebAreaScriptEnginesAuthorization.Server = deleted.ID
END
GO

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

	-- remove all extended user details ON USER REMOVAL where the detail type (field propertyname of table log_users) is declared as DELETE IMMEDIATELY ON USER REMOVAL
	DELETE FROM dbo.Log_Users WHERE ID_User IN ( SELECT ID FROM deleted ) AND [Type] IN (SELECT ValueNVarChar FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LogTypeDeletionSetting' And ValueBoolean = 1)

	--Log the fact that user has been deleted.
	INSERT INTO dbo.Log_Users (ID_USER, Type, VALUE, ModifiedOn)
	SELECT ID, 'DeletedOn', GetDate(), GETDATE() FROM deleted
END
GO
IF OBJECT_ID ('dbo.Gruppen_PostUserDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Gruppen_PostUserDeletionTrigger;
GO
IF OBJECT_ID ('dbo.Gruppen_PostGroupDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Gruppen_PostGroupDeletionTrigger;
GO
CREATE TRIGGER dbo.Gruppen_PostGroupDeletionTrigger
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