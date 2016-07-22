-- dropped concept -> dropped trigger
IF OBJECT_ID ('dbo.IUD_Apps2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_Apps2SecObjAndNavItems;
GO
-- =========================================================================================================
-- ===== CIRCULAR REFERENCES CHECKS
-- =========================================================================================================
IF OBJECT_ID ('dbo.IU_MembershipsClones_CircularReferences', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IU_MembershipsClones_CircularReferences;
GO
CREATE TRIGGER [dbo].IU_MembershipsClones_CircularReferences 
   ON  [dbo].MembershipsClones 
   FOR INSERT,DELETE
AS 
BEGIN
	DECLARE @InsertedCircularReferences int
	DECLARE @InvalidSampleValue int
	DECLARE @ErrorMessage nvarchar(120)
	SELECT @InsertedCircularReferences = COUNT(*) FROM inserted WHERE [ID_Group] = [ID_ClonedGroup];
	IF IsNull(@InsertedCircularReferences, 0) > 0
		BEGIN
			SET NOCOUNT ON;
			SELECT TOP 1 @InvalidSampleValue = ID_Group FROM inserted
			SET @ErrorMessage = 'Cannot add circular references into table MembershipsClones: ID ' + CAST(@InvalidSampleValue AS nvarchar(20))
			RAISERROR (@ErrorMessage, 16, 1)
			ROLLBACK TRANSACTION 		
			RETURN	
		END
END
GO
IF OBJECT_ID ('dbo.IU_ApplicationsRights_Inheriting_CircularReferences', 'TR') IS NOT NULL
   DROP TRIGGER [dbo].[IU_ApplicationsRights_Inheriting_CircularReferences];
GO
CREATE TRIGGER [dbo].IU_ApplicationsRights_Inheriting_CircularReferences 
   ON  [dbo].ApplicationsRights_Inheriting 
   FOR INSERT,DELETE
AS 
BEGIN
	DECLARE @InsertedCircularReferences int
	DECLARE @InvalidSampleValue int
	DECLARE @ErrorMessage nvarchar(120)
	SELECT @InsertedCircularReferences = COUNT(*) FROM inserted WHERE [ID_Inheriting] = [ID_Source];
	IF IsNull(@InsertedCircularReferences, 0) > 0
		BEGIN
			SET NOCOUNT ON;
			SELECT TOP 1 @InvalidSampleValue = [ID_Source] FROM inserted
			SET @ErrorMessage = 'Cannot add circular references into table ApplicationsRights_Inheriting: ID ' + CAST(@InvalidSampleValue AS nvarchar(20))
			RAISERROR (@ErrorMessage, 16, 1)
			ROLLBACK TRANSACTION 		
			RETURN	
		END
END
GO
-- Execute checks on existing data to prevent stepping forward in case of critical errors
DECLARE @InsertedCircularReferences int
DECLARE @InvalidSampleValue int
DECLARE @ErrorMessage nvarchar(120)

SELECT @InsertedCircularReferences = COUNT(*) FROM dbo.MembershipsClones WHERE [ID_Group] = [ID_ClonedGroup];
IF IsNull(@InsertedCircularReferences, 0) > 0
	BEGIN
		SET NOCOUNT ON;
		SELECT TOP 1 @InvalidSampleValue = ID_Group FROM dbo.MembershipsClones
		SET @ErrorMessage = 'Cannot add circular references into table MembershipsClones: ID ' + CAST(@InvalidSampleValue AS nvarchar(20))
		RAISERROR (@ErrorMessage, 16, 1)
		ROLLBACK TRANSACTION 		
		RETURN	
	END
SELECT @InsertedCircularReferences = NULL, @InvalidSampleValue = @InvalidSampleValue
SELECT @InsertedCircularReferences = COUNT(*) FROM dbo.ApplicationsRights_Inheriting WHERE [ID_Inheriting] = [ID_Source];
IF IsNull(@InsertedCircularReferences, 0) > 0
	BEGIN
		SET NOCOUNT ON;
		SELECT TOP 1 @InvalidSampleValue = [ID_Source] FROM dbo.ApplicationsRights_Inheriting
		SET @ErrorMessage = 'Cannot add circular references into table ApplicationsRights_Inheriting: ID ' + CAST(@InvalidSampleValue AS nvarchar(20))
		RAISERROR (@ErrorMessage, 16, 1)
		ROLLBACK TRANSACTION 		
		RETURN	
	END
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
CREATE TRIGGER [dbo].[IUD_Applications] 
   ON  [dbo].[Applications_CurrentAndInactiveOnes] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;

	-- Update table dbo.[ApplicationsRights_Inheriting]
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			DELETE dbo.[ApplicationsRights_Inheriting]
			FROM dbo.[ApplicationsRights_Inheriting]
				INNER JOIN deleted
					ON dbo.[ApplicationsRights_Inheriting].[RuleSourceApplicationID] = deleted.ID
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			INSERT INTO dbo.[ApplicationsRights_Inheriting] (ID_Inheriting, ID_Source, ReleasedBy, ModifiedBy, RuleSourceApplicationID)
			SELECT ID, AuthsAsAppID, ModifiedBy, ModifiedBy, ID
			FROM inserted
			WHERE AppDeleted = 0 AND AuthsAsAppID IS NOT NULL;
		END

	-- Update table dbo.ApplicationsRightsByGroup to always allow Supervisors
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			DELETE dbo.[ApplicationsRightsByGroup]
			FROM dbo.[ApplicationsRightsByGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByGroup].ID_Application = deleted.ID
						AND dbo.[ApplicationsRightsByGroup].ID_ServerGroup = 0
						AND dbo.[ApplicationsRightsByGroup].ID_GroupOrPerson IN (-6, 6)
						AND dbo.[ApplicationsRightsByGroup].DevelopmentTeamMember = 1
						AND dbo.[ApplicationsRightsByGroup].IsDenyRule = 0;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			INSERT INTO dbo.[ApplicationsRightsByGroup] (ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, ReleasedBy, ReleasedOn, [IsSupervisorAutoAccessRule])
			SELECT ID, 0, 6, 1, 0, IsNull(inserted.ModifiedBy, inserted.ReleasedBy), IsNull(inserted.ModifiedOn, inserted.ReleasedOn), 1
			FROM inserted
			WHERE AppDeleted = 0;
			INSERT INTO dbo.[ApplicationsRightsByGroup] (ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, ReleasedBy, ReleasedOn, [IsSupervisorAutoAccessRule])
			SELECT ID, 0, -6, 1, 0, IsNull(inserted.ModifiedBy, inserted.ReleasedBy), IsNull(inserted.ModifiedOn, inserted.ReleasedOn), 1
			FROM inserted
			WHERE AppDeleted = 0;
		END
END 
GO
-- Force first table filling (or full update on repetitions)
UPDATE [dbo].[Applications_CurrentAndInactiveOnes] 
SET TitleAdminArea = TitleAdminArea
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

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowAffectedCount bigint
	SELECT @RowAffectedCount = IsNull(COUNT_BIG(*), 0) FROM @UsersWithModifiedAccessLevel;
	IF IsNull(@RowAffectedCount, 0) > 0
		BEGIN
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

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowAffectedCount bigint
	SELECT @RowAffectedCount = IsNull(COUNT_BIG(*), 0) FROM @Users;
	IF IsNull(@RowAffectedCount, 0) > 0
		BEGIN
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

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0 OR IsNull(@RowInsertsCount, 0) > 0
		BEGIN
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
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- (re-)insert anonymous groups memberships to user with ID -1
			INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
			SELECT ID_Group_Anonymous, -1, GETDATE(), -43, 0, 1
			FROM inserted
			GROUP BY ID_Group_Anonymous
		END
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

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowAffectedCount bigint
	SELECT @RowAffectedCount = IsNull(COUNT_BIG(*), 0) FROM @Users;
	IF IsNull(@RowAffectedCount, 0) > 0
		BEGIN
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
END
GO
------------------------------------------------------------------------------------------------------------
-- PRE-CALCULATION OF EFFECTIVE MEMBERSHIPS BASED ON ALLOW AND DENY RULES
------------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.U_Memberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.U_Memberships;
GO
CREATE TRIGGER [dbo].U_Memberships 
   ON  [dbo].Memberships 
   FOR UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	RAISERROR ('Memberships can''t be updated, only inserted or deleted', 16, 1)
	ROLLBACK TRANSACTION 		
	RETURN	
END
GO
IF OBJECT_ID ('dbo.ID_Memberships', 'TR') IS NOT NULL
   DROP TRIGGER dbo.ID_Memberships;
GO
CREATE TRIGGER [dbo].ID_Memberships 
   ON  [dbo].Memberships
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- 1st, forward all row deletions
			DELETE [dbo].[Memberships_EffectiveRules]
			FROM [dbo].[Memberships_EffectiveRules]
				INNER JOIN deleted
					ON [dbo].[Memberships_EffectiveRules].[ID_Memberships_ByAllowRule] = deleted.ID;
			DELETE [dbo].[Memberships_DenyRules]
			FROM [dbo].[Memberships_DenyRules]
				INNER JOIN deleted
					ON [dbo].[Memberships_DenyRules].[ID_Memberships] = deleted.ID;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- 2nd, forward all row inserts
			INSERT INTO dbo.[Memberships_EffectiveRules] (ID_Group, ID_User, ID_Memberships_ByAllowRule)
			SELECT inserted.ID_Group, inserted.ID_User, inserted.ID
			FROM inserted
				LEFT JOIN [dbo].[Memberships_DenyRules]
					ON inserted.ID_Group = [dbo].[Memberships_DenyRules].ID_Group
						AND inserted.ID_User = [dbo].[Memberships_DenyRules].ID_User
			WHERE IsDenyRule = 0
				AND [dbo].[Memberships_DenyRules].ID_Group IS NULL;
			INSERT INTO dbo.[Memberships_DenyRules] (ID_Group, ID_User, ID_Memberships)
			SELECT ID_Group, ID_User, ID
			FROM inserted
			WHERE IsDenyRule <> 0;
		END
END
GO
IF OBJECT_ID ('dbo.U_Memberships_DenyRules', 'TR') IS NOT NULL
   DROP TRIGGER dbo.U_Memberships_DenyRules;
GO
CREATE TRIGGER [dbo].U_Memberships_DenyRules 
   ON  [dbo].Memberships_DenyRules 
   FOR UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	RAISERROR ('Membership Deny Rules can''t be updated, only inserted or deleted', 16, 1)
	ROLLBACK TRANSACTION 		
	RETURN	
END
GO
IF OBJECT_ID ('dbo.ID_Memberships_DenyRules', 'TR') IS NOT NULL
   DROP TRIGGER dbo.ID_Memberships_DenyRules;
GO
CREATE TRIGGER [dbo].ID_Memberships_DenyRules 
   ON  [dbo].Memberships_DenyRules 
   FOR INSERT,DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- 1st, forward all insertions as row deletions
			DELETE [dbo].[Memberships_EffectiveRules]
			FROM [dbo].[Memberships_EffectiveRules]
				INNER JOIN inserted
					ON inserted.ID_Group = [dbo].[Memberships_EffectiveRules].ID_Group
						AND inserted.ID_User = [dbo].[Memberships_EffectiveRules].ID_User
		END
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- 2nd, forward all deletions as row inserts (if any allow rules start to apply)
			INSERT INTO dbo.[Memberships_EffectiveRules] (ID_Group, ID_User, ID_Memberships_ByAllowRule)
			SELECT [dbo].[Memberships].ID_Group, [dbo].[Memberships].ID_User, [dbo].[Memberships].ID
			FROM deleted
				INNER JOIN [dbo].[Memberships]
					ON deleted.ID_Group = [dbo].[Memberships].ID_Group
						AND deleted.ID_User = [dbo].[Memberships].ID_User
				LEFT JOIN [dbo].[Memberships_DenyRules]
					ON [dbo].[Memberships].ID_Group = [dbo].[Memberships_DenyRules].ID_Group
						AND [dbo].[Memberships].ID_User = [dbo].[Memberships_DenyRules].ID_User
				LEFT JOIN [dbo].[Memberships_EffectiveRules]
					ON [dbo].[Memberships].ID_Group = [dbo].[Memberships_EffectiveRules].ID_Group
						AND [dbo].[Memberships].ID_User = [dbo].[Memberships_EffectiveRules].ID_User
			WHERE IsDenyRule = 0 -- only add allow rules
				AND [dbo].[Memberships_DenyRules].ID_Group IS NULL -- only insert rows which haven't got a deny rule in place
				AND [dbo].[Memberships_EffectiveRules].ID_Group IS NULL -- only insert rows which haven't already existed (e.g. by another rule)
		END
END
GO
IF OBJECT_ID ('dbo.U_MembershipsClones', 'TR') IS NOT NULL
   DROP TRIGGER dbo.U_MembershipsClones;
GO
CREATE TRIGGER [dbo].U_MembershipsClones 
   ON  [dbo].MembershipsClones 
   FOR UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	RAISERROR ('Membership Clones Rules can''t be updated, only inserted or deleted', 16, 1)
	ROLLBACK TRANSACTION 		
	RETURN	
END
GO
IF OBJECT_ID ('dbo.ID_MembershipsClones', 'TR') IS NOT NULL
   DROP TRIGGER dbo.ID_MembershipsClones;
GO
CREATE TRIGGER [dbo].ID_MembershipsClones 
   ON  [dbo].MembershipsClones 
   FOR INSERT,DELETE
AS 
BEGIN
	DECLARE @InsertedDenyRules int
	SELECT @InsertedDenyRules = COUNT(*) FROM inserted WHERE IsDenyRule <> 0;
	IF IsNull(@InsertedDenyRules, 0) > 0
		BEGIN
			SET NOCOUNT ON;
			RAISERROR ('Membership Clones Rules for DenyRules not supported, yet', 16, 1)
			ROLLBACK TRANSACTION 		
			RETURN	
		END
END
GO
IF OBJECT_ID ('dbo.ID_Memberships_EffectiveRules', 'TR') IS NOT NULL
   DROP TRIGGER dbo.ID_Memberships_EffectiveRules
GO
CREATE TRIGGER [dbo].ID_Memberships_EffectiveRules
   ON  [dbo].Memberships_EffectiveRules
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- 1st, forward all row deletions
			DELETE [dbo].[Memberships_EffectiveRulesWithClones1stGrade]
			FROM [dbo].[Memberships_EffectiveRulesWithClones1stGrade]
				INNER JOIN deleted
					ON [dbo].[Memberships_EffectiveRulesWithClones1stGrade].[ID_Memberships_EffectiveRules] = deleted.ID;
				-- TODO on delete: what has to happen when Deny-Cloning-Rules exist? Recalculate AllowRule?!?
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- 2nd, forward all row inserts
			-- 2a. direct effective memberships
			INSERT INTO dbo.[Memberships_EffectiveRulesWithClones1stGrade] (ID_Group, ID_User, [ID_Memberships_EffectiveRules], [ID_Memberships_ByAllowRule], [ID_MembershipsClones])
			SELECT inserted.ID_Group, inserted.ID_User, inserted.ID, inserted.ID_Memberships_ByAllowRule, NULL
			FROM inserted;
			-- 2b. cloned effective memberships by Allow-Rule
			INSERT INTO dbo.[Memberships_EffectiveRulesWithClones1stGrade] (ID_Group, ID_User, [ID_Memberships_EffectiveRules], [ID_Memberships_ByAllowRule], [ID_MembershipsClones])
			SELECT [dbo].[MembershipsClones].ID_Group, inserted.ID_User, inserted.ID, inserted.ID_Memberships_ByAllowRule, [dbo].[MembershipsClones].ID
			FROM inserted
				INNER JOIN [dbo].[MembershipsClones] ON [dbo].[MembershipsClones].ID_ClonedGroup = inserted.ID_Group
				LEFT JOIN 
					(
					SELECT ID_Group, ID_ClonedGroup
					FROM [dbo].[MembershipsClones] 
					WHERE IsDenyRule <> 0
					) AS DenyCloningRules 
						ON DenyCloningRules.ID_ClonedGroup = [dbo].[MembershipsClones].ID_ClonedGroup
							AND DenyCloningRules.ID_Group = [dbo].[MembershipsClones].ID_Group
			WHERE [dbo].[MembershipsClones].IsDenyRule = 0 -- only Cloning-Allowed-Rules
				AND DenyCloningRules.ID_ClonedGroup IS NULL -- only on missing Cloning-Denied-Rules
			;
			-- 2c. cloned effective memberships by Deny-Rule
			-- TODO on insert: what has to happen when Deny-Cloning-Rules exist? Delete what from what? Restore which Allow-Rules?
		END
END
GO
IF OBJECT_ID ('dbo.ID_Memberships_EffectiveRulesWithClones1stGrade', 'TR') IS NOT NULL
   DROP TRIGGER dbo.ID_Memberships_EffectiveRulesWithClones1stGrade;
GO
CREATE TRIGGER [dbo].ID_Memberships_EffectiveRulesWithClones1stGrade 
   ON  [dbo].[Memberships_EffectiveRulesWithClones1stGrade] 
   FOR INSERT,DELETE
AS 
BEGIN

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- 1st, forward all row deletions
			DELETE [dbo].[Memberships_EffectiveRulesWithClonesNthGrade]
			FROM [dbo].[Memberships_EffectiveRulesWithClonesNthGrade]
				INNER JOIN deleted
					ON [dbo].[Memberships_EffectiveRulesWithClonesNthGrade].[ID_Memberships_EffectiveRulesWithClones1stGrade] = deleted.ID;
			-- TODO on delete: what has to happen when cloning of 1 of the many n cloning-levels has been revoked? Delete? Recalculate ?!?
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- 2nd, forward all row inserts
			INSERT INTO dbo.[Memberships_EffectiveRulesWithClonesNthGrade] (ID_Group, ID_User, [ID_Memberships_EffectiveRulesWithClones1stGrade], [ID_Memberships_ByAllowRule], [ID_MembershipsClones])
			SELECT inserted.ID_Group, inserted.ID_User, inserted.ID, inserted.ID_Memberships_ByAllowRule, [ID_MembershipsClones]
			FROM inserted;
			-- TODO on insert: what has to happen when cloning of has been added for a group which is 1 of the many n cloning-levels? Delete what from what? Insert what into what? Restore what?
		END
END
GO
IF OBJECT_ID ('dbo.ID_Memberships_EffectiveRulesWithClonesNthGrade', 'TR') IS NOT NULL
   DROP TRIGGER dbo.ID_Memberships_EffectiveRulesWithClonesNthGrade;
GO
CREATE TRIGGER [dbo].ID_Memberships_EffectiveRulesWithClonesNthGrade 
   ON  [dbo].[Memberships_EffectiveRulesWithClonesNthGrade] 
   FOR INSERT,DELETE
AS 
BEGIN

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- 1st, forward all row deletions
			DELETE dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
			FROM dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
				INNER JOIN deleted 
					ON dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved.DerivedFromPreStaging2_Groups_ID = deleted.ID_Group
						AND dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved.ID_User = deleted.ID_User;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- 2nd, forward all row inserts
			INSERT INTO dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[IsDenyRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging2_Groups_RealServerGroupID]
				   ,[DerivedFromPreStaging2_Groups_ID]
				   ,[DerivedFromPreStaging2_Users_ID])
			SELECT ApplicationsRightsByGroup_PreStaging2Inheritions.[ID_SecurityObject]
				   ,inserted.ID_User
				   ,ApplicationsRightsByGroup_PreStaging2Inheritions.[ID_ServerGroup]
				   ,ApplicationsRightsByGroup_PreStaging2Inheritions.[IsDevRule]
				   ,ApplicationsRightsByGroup_PreStaging2Inheritions.[IsDenyRule]
				   ,ApplicationsRightsByGroup_PreStaging2Inheritions.[DerivedFromAppRightsID]
				   ,ApplicationsRightsByGroup_PreStaging2Inheritions.ID_ServerGroup
				   ,ApplicationsRightsByGroup_PreStaging2Inheritions.ID
				   ,NULL
			FROM dbo.ApplicationsRightsByGroup_PreStaging2Inheritions
				INNER JOIN inserted
					ON ApplicationsRightsByGroup_PreStaging2Inheritions.ID_Group = inserted.ID_Group;
		END
END
GO
-- Initial reset/filling of pre-calculated effective memberships
ALTER TABLE dbo.Memberships DISABLE TRIGGER U_Memberships;
UPDATE dbo.Memberships
SET ReleasedBy = ReleasedBy;
ALTER TABLE dbo.Memberships ENABLE TRIGGER U_Memberships;
GO
-- =========================================================================================================
-- ===== AUTHORIZATIONS TABLES - PRE-STAGED/PRE-CALCULATED, EFFECTIVE
-- =========================================================================================================

---------------------------------------------------------------------------------------------------
--> PLEASE NOTE: anonymous user is a user with a group ID -1 and user ID -1!
---------------------------------------------------------------------------------------------------
--> ATTENTION GROUPS VERSION: 
--> users are already member of public group, but GROUPS are not automatically member of public group
--> consider this behaviour in all dependent queries/SPs like SP GetNavPoints*ByUser/ByGroup
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 1
---------------------------------------------------------------------------------------------------
-- pre-existing authorizations for all registered users/groups incl. anonymous and public auths
--> AND all-server-groups "0" rewritten to real server-group-IDs
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 2
---------------------------------------------------------------------------------------------------
-- pre-existing authorizations for all registered users/groups incl. anonymous and public auths
-- AND groups auths resolved to the several users auths
-- AND all-server-groups "0" rewritten to real server-group-IDs
--> AND added entries from inherited security objects/applications
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 3
---------------------------------------------------------------------------------------------------
-- cumulated for all registered users (incl. anonymous auths)
-- AND groups auths resolved to the several users auths
-- AND all-server-groups "0" rewritten to real server-group-IDs
-- AND added entries from inherited security objects/applications
--> AND groups auths resolved to the several users auths 
-->     respectively user id -1 with related server group ID for all anonymous groups
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 4
---------------------------------------------------------------------------------------------------
-- cumulated for all registered users (incl. anonymous auths)
-- AND groups auths resolved to the several users auths
-- AND all-server-groups "0" rewritten to real server-group-IDs
-- AND added entries from inherited security objects/applications
-- AND groups auths resolved to the several users auths 
--     respectively user id -1 with related server group ID for all anonymous groups
--> AND calculated remaining AllowRules after subtraction of DenyRules
---------------------------------------------------------------------------------------------------
GO
-- CLEANUP OF ALL PRE-EXISTING DATA IN PRE-STAGING-TABLES
TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules
TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
TRUNCATE TABLE dbo.ApplicationsRightsByGroup_PreStaging2Inheritions
TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging2Inheritions
TRUNCATE TABLE dbo.ApplicationsRightsByGroup_PreStaging1ForRealServerGroup
TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging1ForRealServerGroup
GO
---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 1
---------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[ApplicationsRightsByGroup] - TRIGGER dbo.IUD_AuthsGroups2PreStaging1ForServerGroup
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsGroups2PreStagingForServerGroup]'))
DROP TRIGGER [dbo].[IUD_AuthsGroups2PreStagingForServerGroup]
GO
CREATE TRIGGER [dbo].[IUD_AuthsGroups2PreStagingForServerGroup] 
   ON  [dbo].[ApplicationsRightsByGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup = 0
			DELETE dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
			FROM dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup].[DerivedFromAppRightsID] = deleted.ID
						AND dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup].IsServerGroup0Rule <> 0
			WHERE deleted.ID_ServerGroup = 0;
			-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup <> 0
			DELETE dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
			FROM dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup].DerivedFromAppRightsID = deleted.ID
						AND dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup].IsServerGroup0Rule = 0
			WHERE deleted.ID_ServerGroup <> 0;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup = 0
			INSERT INTO dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
			SELECT ID_Application, dbo.System_ServerGroups.ID AS ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 1, inserted.ID
			FROM inserted
				CROSS JOIN dbo.System_ServerGroups
			WHERE inserted.ID_ServerGroup = 0
			-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup <> 0
			INSERT INTO dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
			SELECT ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 0, inserted.ID
			FROM inserted
			WHERE inserted.ID_ServerGroup <> 0
		END
END
GO

------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[ApplicationsRightsByUser] - TRIGGER dbo.IUD_AuthsUsers2PreStagingForServerGroup
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_AuthsUsers2PreStagingForServerGroup]'))
DROP TRIGGER [dbo].[IUD_AuthsUsers2PreStagingForServerGroup]
GO
CREATE TRIGGER [dbo].[IUD_AuthsUsers2PreStagingForServerGroup] 
   ON  [dbo].[ApplicationsRightsByUser] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup = 0
			DELETE dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
			FROM dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup].DerivedFromAppRightsID = deleted.ID
						AND dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup].IsServerGroup0Rule <> 0
			WHERE deleted.ID_ServerGroup = 0;
			-- Drop all pre-staging data to old/deleted auth setup: ID_ServerGroup <> 0
			DELETE dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
			FROM dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup].DerivedFromAppRightsID = deleted.ID
						AND dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup].IsServerGroup0Rule = 0
			WHERE deleted.ID_ServerGroup <> 0;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup = 0
			INSERT INTO dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
			SELECT ID_Application, dbo.System_ServerGroups.ID AS ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 1, inserted.ID
			FROM inserted
				CROSS JOIN dbo.System_ServerGroups
			WHERE inserted.ID_ServerGroup = 0
			-- (Re-)insert required pre-staging data for new/inserted auth setup: ID_ServerGroup <> 0
			INSERT INTO dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
			SELECT ID_Application, ID_ServerGroup, ID_GroupOrPerson, DevelopmentTeamMember, IsDenyRule, 0, inserted.ID
			FROM inserted
			WHERE inserted.ID_ServerGroup <> 0
		END
END
GO

------------------------------------------------------------------------------------------------------------
-- TABLE [dbo].[System_ServerGroups] - TRIGGER dbo.IUD_ServerGroupAuths2PreStagingForServerGroup
------------------------------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup]'))
DROP TRIGGER [dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup]
GO
CREATE TRIGGER [dbo].[IUD_ServerGroupAuths2PreStagingForServerGroup] 
   ON  [dbo].[System_ServerGroups] 
   FOR INSERT,DELETE
   -- NOT FOR UPDATE since applications rights would be deleted 
AS 
BEGIN
DECLARE @AFirstServerGroupID int
SELECT TOP 1 @AFirstServerGroupID = ID FROM [dbo].[System_ServerGroups] WHERE ID NOT IN (SELECT ID FROM inserted)

	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- Drop of depending auths and data works automatically except for anonymous-group->anonymous-user translations
			DELETE dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved]
			FROM dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved]
				INNER JOIN deleted 
					ON dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved].DerivedFromPreStaging2_Groups_RealServerGroupID = deleted.ID
			-- All ServerRule-0-CrossJoin-Rows have to be 
			--   1. deleted for existing ServerGroup-0-Rules
			--   2. inserted(copied) for new server group for existing ServerGroup-0-Rules
			-- Drop all indirectly related app-rights (ApplicationsRightsByUser_PreStaging1ForRealServerGroup): USERS
			DELETE dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
			FROM dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup].ID_ServerGroup = deleted.ID
			-- Drop all indirectly related app-rights (ApplicationsRightsByGroup_PreStaging1ForRealServerGroup): GROUPS
			DELETE dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
			FROM dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
				INNER JOIN deleted
					ON dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup].ID_ServerGroup = deleted.ID
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- Insert required pre-staging data for inserted ServerGroup to complete pre-stage-data for CROSS JOIN for auths with ID_ServerGroup = 0
			IF @AFirstServerGroupID IS NOT NULL
				BEGIN
					-- clone group auths from a first, existing server group
					INSERT INTO dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
					SELECT ID_SecurityObject, inserted.ID, ID_Group, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID
					FROM dbo.[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
						CROSS JOIN inserted
					WHERE ID_ServerGroup = @AFirstServerGroupID AND IsServerGroup0Rule <> 0
					-- clone user auths from a first, existing server group
					INSERT INTO dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup] (ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID)
					SELECT ID_SecurityObject, inserted.ID, ID_User, IsDevRule, IsDenyRule, IsServerGroup0Rule, DerivedFromAppRightsID
					FROM dbo.[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
						CROSS JOIN inserted
					WHERE ID_ServerGroup = @AFirstServerGroupID AND IsServerGroup0Rule <> 0
				END 
		END
END
GO
---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 2
---------------------------------------------------------------------------------------------------
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ApplicationsRightsByGroup_PreStaging1ToPreStaging2]'))
DROP TRIGGER [dbo].[IUD_ApplicationsRightsByGroup_PreStaging1ToPreStaging2]
GO
CREATE TRIGGER [dbo].[IUD_ApplicationsRightsByGroup_PreStaging1ToPreStaging2] 
   ON  [dbo].[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			DELETE dbo.ApplicationsRightsByGroup_PreStaging2Inheritions
			FROM dbo.ApplicationsRightsByGroup_PreStaging2Inheritions
				INNER JOIN deleted 
					ON dbo.ApplicationsRightsByGroup_PreStaging2Inheritions.DerivedFromPreStaging1ID = deleted.ID;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			INSERT INTO [dbo].[ApplicationsRightsByGroup_PreStaging2Inheritions]
				   ([ID_SecurityObject]
				   ,[ID_Group]
				   ,[ID_ServerGroup]
				   ,[IsDenyRule]
				   ,[IsDevRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging1ID]
				   ,[DerivedFromInheritedSecurityObjectRelationID])
			SELECT ID_SecurityObject, ID_Group, ID_ServerGroup, IsDenyRule, IsDevRule, DerivedFromAppRightsID, inserted.ID, NULL
			FROM inserted
			UNION ALL
			SELECT dbo.ApplicationsRights_Inheriting.ID_Inheriting, ID_ServerGroup, ID_Group, IsDenyRule, IsDevRule, DerivedFromAppRightsID, inserted.ID, dbo.ApplicationsRights_Inheriting.ID
			FROM inserted
				INNER JOIN dbo.ApplicationsRights_Inheriting
					ON inserted.ID_SecurityObject = dbo.ApplicationsRights_Inheriting.ID_Source;
		END
END
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ApplicationsRightsByUser_PreStaging1ToPreStaging2]'))
DROP TRIGGER [dbo].[IUD_ApplicationsRightsByUser_PreStaging1ToPreStaging2]
GO
CREATE TRIGGER [dbo].[IUD_ApplicationsRightsByUser_PreStaging1ToPreStaging2] 
   ON  [dbo].[ApplicationsRightsByUser_PreStaging1ForRealServerGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			DELETE dbo.ApplicationsRightsByUser_PreStaging2Inheritions
			FROM dbo.ApplicationsRightsByUser_PreStaging2Inheritions
				INNER JOIN deleted 
					ON dbo.ApplicationsRightsByUser_PreStaging2Inheritions.DerivedFromPreStaging1ID = deleted.ID;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			INSERT INTO [dbo].[ApplicationsRightsByUser_PreStaging2Inheritions]
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDenyRule]
				   ,[IsDevRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging1ID]
				   ,[DerivedFromInheritedSecurityObjectRelationID])
			SELECT ID_SecurityObject, ID_User, ID_ServerGroup, IsDenyRule, IsDevRule, DerivedFromAppRightsID, inserted.ID, NULL
			FROM inserted
			UNION ALL
			SELECT dbo.ApplicationsRights_Inheriting.ID_Inheriting, ID_ServerGroup, ID_User, IsDenyRule, IsDevRule, DerivedFromAppRightsID, inserted.ID, dbo.ApplicationsRights_Inheriting.ID
			FROM inserted
				INNER JOIN dbo.ApplicationsRights_Inheriting
					ON inserted.ID_SecurityObject = dbo.ApplicationsRights_Inheriting.ID_Source;
		END
END
--TODO: DerivedFromInheritedSecurityObjectRelationID INSERT/DELETE ON dbo.ApplicationsRights_Inheriting
GO
---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 3
---------------------------------------------------------------------------------------------------
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ApplicationsRightsByGroup_PreStaging2ToPreStaging3]'))
DROP TRIGGER [dbo].[IUD_ApplicationsRightsByGroup_PreStaging2ToPreStaging3]
GO
CREATE TRIGGER [dbo].[IUD_ApplicationsRightsByGroup_PreStaging2ToPreStaging3] 
   ON  [dbo].[ApplicationsRightsByGroup_PreStaging2Inheritions] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- forward-drop all group auths
			DELETE dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
			FROM dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
				INNER JOIN deleted 
					ON dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved.DerivedFromPreStaging2_Groups_ID = deleted.ID;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- I. Forward ALL auth changes (INSERT) for anonymous group to anonymous user
			-- Forward-Insert required pre-staging data for anonymous app-rights
			INSERT INTO dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved] 
					(ID_SecurityObject
					, ID_ServerGroup
					, ID_User
					, IsDevRule
					, IsDenyRule
					, DerivedFromAppRightsID
					, DerivedFromPreStaging2_Users_ID
					, DerivedFromPreStaging2_Groups_ID
					, DerivedFromPreStaging2_Groups_RealServerGroupID)
			SELECT ID_SecurityObject, ID_ServerGroup, -1, IsDevRule, IsDenyRule, DerivedFromAppRightsID, NULL, inserted.ID, System_ServerGroups.ID
			FROM inserted
				INNER JOIN dbo.System_ServerGroups 
					ON inserted.ID_Group = dbo.System_ServerGroups.ID_Group_Anonymous;

			-- II. Forward ALL auth changes (INSERT) for standard group auths
			-- forward-insert all standard group auths
			INSERT INTO dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[IsDenyRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging2_Groups_RealServerGroupID]
				   ,[DerivedFromPreStaging2_Groups_ID]
				   ,[DerivedFromPreStaging2_Users_ID])
			SELECT inserted.[ID_SecurityObject]
				   ,Memberships_EffectiveRulesWithClonesNthGrade.[ID_User]
				   ,inserted.[ID_ServerGroup]
				   ,inserted.[IsDevRule]
				   ,inserted.[IsDenyRule]
				   ,inserted.[DerivedFromAppRightsID]
				   ,inserted.ID_ServerGroup
				   ,inserted.ID
				   ,NULL
			FROM inserted
				INNER JOIN dbo.Memberships_EffectiveRulesWithClonesNthGrade
					ON inserted.ID_Group = Memberships_EffectiveRulesWithClonesNthGrade.ID_Group;
		END
END
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ApplicationsRightsByUser_PreStaging2ToPreStaging3]'))
DROP TRIGGER [dbo].[IUD_ApplicationsRightsByUser_PreStaging2ToPreStaging3]
GO
CREATE TRIGGER [dbo].[IUD_ApplicationsRightsByUser_PreStaging2ToPreStaging3] 
   ON  [dbo].[ApplicationsRightsByUser_PreStaging2Inheritions] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- forward-drop all user auths
			DELETE dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
			FROM dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
				INNER JOIN deleted 
					ON dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved.DerivedFromPreStaging2_Users_ID = deleted.ID;
		END
	IF IsNull(@RowInsertsCount, 0) > 0
		BEGIN
			-- forward-insert all user auths
			INSERT INTO dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[IsDenyRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging2_Groups_RealServerGroupID]
				   ,[DerivedFromPreStaging2_Groups_ID]
				   ,[DerivedFromPreStaging2_Users_ID])
			SELECT [ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[IsDenyRule]
				   ,[DerivedFromAppRightsID]
				   ,NULL
				   ,NULL
				   ,ID
			FROM inserted
		END
END
GO
---------------------------------------------------------------------------------------------------
-- PRE-STAGING-LEVEL 4
---------------------------------------------------------------------------------------------------
GO
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[IUD_ApplicationsRightsByUser_PreStaging3ToPreStaging4]'))
DROP TRIGGER [dbo].[IUD_ApplicationsRightsByUser_PreStaging3ToPreStaging4]
GO
CREATE TRIGGER [dbo].[IUD_ApplicationsRightsByUser_PreStaging3ToPreStaging4] 
   ON  [dbo].[ApplicationsRightsByUser_PreStaging3GroupsResolved] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @Soll table (
		[ID_SecurityObject] [int] NOT NULL,
		ID_User [int] NOT NULL,
		[ID_ServerGroup] [int] NOT NULL,
		[IsDevRule] [bit] NOT NULL,
		[DerivedFromAppRightsID] [int] NOT NULL,
		[DerivedFromPreStaging3ID] [bigint] NOT NULL,
		[PK_UniqueRowData] varchar(250) NOT NULL,
		[UniqueAuthObject] varchar(120) NOT NULL)
	DECLARE @ChangedAuthObjects table (
		[PK] varchar(250) NOT NULL)
	DECLARE @RowInsertsCount bigint, @RowDeletesCount bigint, @CurrentRowsCount bigint
	SELECT @RowInsertsCount = IsNull(COUNT_BIG(*), 0) FROM inserted;
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	SELECT @CurrentRowsCount = IsNull(COUNT_BIG(*), 0) FROM dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules;
	IF IsNull(@RowInsertsCount, 0) + IsNull(@RowDeletesCount, 0) = 0
		BEGIN
			-- empty amount of rows to be updated - just update required rows
			-- PRINT 'empty amount of rows to be updated';
		END
	IF IsNull(@RowInsertsCount, 0) + IsNull(@RowDeletesCount, 0) >= IsNull(@CurrentRowsCount, 0)
		BEGIN
			-- too many rows to be updated - just re-create the whole table in full
			-- PRINT 'too many rows to be updated - just re-create the whole table in full';
			INSERT INTO @Soll
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging3ID]
				   ,[PK_UniqueRowData]
				   ,[UniqueAuthObject])
			SELECT AllowRules.ID_SecurityObject, AllowRules.ID_User, AllowRules.ID_ServerGroup, AllowRules.IsDevRule, DerivedFromAppRightsID, AllowRules.ID,
				CAST(AllowRules.ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_User AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_ServerGroup AS varchar(50)) + '|' + 
					CAST(AllowRules.IsDevRule AS varchar(1)) + '|' + 
					CAST(DerivedFromAppRightsID AS varchar(50)) + '|' + 
					CAST(AllowRules.ID AS varchar(50)) AS PK_UniqueRowData,
				CAST(AllowRules.ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_User AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_ServerGroup AS varchar(50)) + '|' AS UniqueAuthObject
			FROM dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved] AS AllowRules
				LEFT JOIN 
					(
						SELECT ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule
						FROM dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved] 
						WHERE IsDenyRule <> 0
					) AS DenyRules
					ON AllowRules.ID_SecurityObject = DenyRules.ID_SecurityObject 
						AND AllowRules.ID_ServerGroup = DenyRules.ID_ServerGroup 
						AND AllowRules.ID_User = DenyRules.ID_User
						AND AllowRules.IsDevRule = DenyRules.IsDevRule
				INNER JOIN 
					(
						SELECT     ID, AppDisabled
						FROM         dbo.Applications_CurrentAndInactiveOnes
						WHERE     (AppDeleted = 0)
					) AS Applications 
					ON AllowRules.ID_SecurityObject = Applications.ID
			WHERE AllowRules.IsDenyRule = 0
				AND DenyRules.ID_SecurityObject IS NULL
				AND 
				(
					(
						AllowRules.IsDevRule = 0 
						AND Applications.AppDisabled = 0
					)
					OR AllowRules.IsDevRule = 1
				)
			GROUP BY AllowRules.[ID_SecurityObject]
				   ,AllowRules.[ID_User]
				   ,AllowRules.[ID_ServerGroup]
				   ,AllowRules.[IsDevRule]
				   ,AllowRules.[DerivedFromAppRightsID]
				   ,AllowRules.ID;
			-- insert missing rows in IST table from SOLL definition table
			INSERT INTO [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging3ID]
				   ,[PK_UniqueRowData]
				   ,[UniqueAuthObject])
			SELECT Soll.[ID_SecurityObject]
				   ,Soll.[ID_User]
				   ,Soll.[ID_ServerGroup]
				   ,Soll.[IsDevRule]
				   ,Soll.[DerivedFromAppRightsID]
				   ,Soll.[DerivedFromPreStaging3ID]
				   ,Soll.[PK_UniqueRowData]
				   ,Soll.[UniqueAuthObject]
			FROM @Soll AS Soll
				LEFT JOIN [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
					ON Soll.[PK_UniqueRowData] = [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].[PK_UniqueRowData]
			WHERE [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].ID_SecurityObject IS NULL;
			-- drop rows in IST table which are not present in SOLL definition table any more
			DELETE [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
			FROM @Soll AS Soll
				RIGHT JOIN [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
					ON Soll.[PK_UniqueRowData] = [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].[PK_UniqueRowData]
			WHERE Soll.ID_SecurityObject IS NULL;
		END
	ELSE
		BEGIN
			-- smaller amount of rows to be updated - just update required rows
			-- PRINT 'smaller amount of rows to be updated - just update required rows';
			-- 1. lookup the changed auth objects
			INSERT INTO @ChangedAuthObjects (PK)
			SELECT CAST(ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(ID_User AS varchar(50)) + '|' + 
					CAST(ID_ServerGroup AS varchar(50)) + '|'
			FROM inserted
			UNION ALL
			SELECT CAST(ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(ID_User AS varchar(50)) + '|' + 
					CAST(ID_ServerGroup AS varchar(50)) + '|'
			FROM deleted;
			-- 2. update as full table statements above, but filter for rows of @ChangedAuthObjects
			INSERT INTO @Soll
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging3ID]
				   ,[PK_UniqueRowData]
				   ,[UniqueAuthObject])
			SELECT AllowRules.ID_SecurityObject, AllowRules.ID_User, AllowRules.ID_ServerGroup, AllowRules.IsDevRule, DerivedFromAppRightsID, AllowRules.ID,
				CAST(AllowRules.ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_User AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_ServerGroup AS varchar(50)) + '|' + 
					CAST(AllowRules.IsDevRule AS varchar(1)) + '|' + 
					CAST(DerivedFromAppRightsID AS varchar(50)) + '|' + 
					CAST(AllowRules.ID AS varchar(50)) AS PK_UniqueRowData,
				CAST(AllowRules.ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_User AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_ServerGroup AS varchar(50)) + '|' AS UniqueAuthObject
			FROM dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved] AS AllowRules
				LEFT JOIN 
					(
						SELECT ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule
						FROM dbo.[ApplicationsRightsByUser_PreStaging3GroupsResolved] 
						WHERE IsDenyRule <> 0
					) AS DenyRules
					ON AllowRules.ID_SecurityObject = DenyRules.ID_SecurityObject 
						AND AllowRules.ID_ServerGroup = DenyRules.ID_ServerGroup 
						AND AllowRules.ID_User = DenyRules.ID_User
						AND AllowRules.IsDevRule = DenyRules.IsDevRule
				INNER JOIN 
					(
						SELECT     ID, AppDisabled
						FROM         dbo.Applications_CurrentAndInactiveOnes
						WHERE     (AppDeleted = 0)
					) AS Applications 
					ON AllowRules.ID_SecurityObject = Applications.ID
			WHERE AllowRules.IsDenyRule = 0
				AND DenyRules.ID_SecurityObject IS NULL
				AND 
				(
					(
						AllowRules.IsDevRule = 0 
						AND Applications.AppDisabled = 0
					)
					OR AllowRules.IsDevRule = 1
				)
				AND CAST(AllowRules.ID_SecurityObject AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_User AS varchar(50)) + '|' + 
					CAST(AllowRules.ID_ServerGroup AS varchar(50)) + '|' IN (SELECT PK FROM @ChangedAuthObjects)
			GROUP BY AllowRules.[ID_SecurityObject]
				   ,AllowRules.[ID_User]
				   ,AllowRules.[ID_ServerGroup]
				   ,AllowRules.[IsDevRule]
				   ,AllowRules.[DerivedFromAppRightsID]
				   ,AllowRules.ID;
			-- insert missing rows in IST table from SOLL definition table
			INSERT INTO [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
				   ([ID_SecurityObject]
				   ,[ID_User]
				   ,[ID_ServerGroup]
				   ,[IsDevRule]
				   ,[DerivedFromAppRightsID]
				   ,[DerivedFromPreStaging3ID]
				   ,[PK_UniqueRowData]
				   ,[UniqueAuthObject])
			SELECT Soll.[ID_SecurityObject]
				   ,Soll.[ID_User]
				   ,Soll.[ID_ServerGroup]
				   ,Soll.[IsDevRule]
				   ,Soll.[DerivedFromAppRightsID]
				   ,Soll.[DerivedFromPreStaging3ID]
				   ,Soll.[PK_UniqueRowData]
				   ,Soll.[UniqueAuthObject]
			FROM @Soll AS Soll
				LEFT JOIN [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
					ON Soll.[PK_UniqueRowData] = [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].[PK_UniqueRowData]
			WHERE [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].ID_SecurityObject IS NULL;
			-- drop rows in IST table which are not present in SOLL definition table any more
			DELETE [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
			FROM @Soll AS Soll
				RIGHT JOIN [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
					ON Soll.[PK_UniqueRowData] = [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].[PK_UniqueRowData]
			WHERE Soll.ID_SecurityObject IS NULL
				AND [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules].[UniqueAuthObject] IN (SELECT PK FROM @ChangedAuthObjects);
		END
END
GO
---------------------------------------------------------------------------------------------------
-- PRE-FILL ALL PRE-STAGING-LEVELS
---------------------------------------------------------------------------------------------------
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

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowDeletesCount bigint
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- drop all relations to access levels
			DELETE dbo.System_ServerGroupsAndTheirUserAccessLevels
			FROM dbo.System_ServerGroupsAndTheirUserAccessLevels 
				INNER JOIN deleted ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = deleted.ID
			-- drop all related servers
			DELETE dbo.System_Servers
			FROM dbo.System_Servers 
				INNER JOIN deleted ON dbo.System_Servers.ServerGroup = deleted.ID
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
		END
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

	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowDeletesCount bigint
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			-- drop all related security objects/applications
			DELETE dbo.Applications_CurrentAndInactiveOnes
			FROM dbo.Applications_CurrentAndInactiveOnes 
				INNER JOIN deleted ON dbo.Applications_CurrentAndInactiveOnes.LocationID = deleted.ID
			-- drop all script engine relations 
			DELETE System_WebAreaScriptEnginesAuthorization
			FROM System_WebAreaScriptEnginesAuthorization 
				INNER JOIN deleted ON dbo.System_WebAreaScriptEnginesAuthorization.Server = deleted.ID
		END
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
	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowDeletesCount bigint
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
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
	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowDeletesCount bigint
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			--Remove references of deleted groups
			DELETE FROM dbo.Memberships WHERE ID_Group IN ( SELECT ID FROM deleted ) 
			DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_GroupOrPerson IN ( SELECT ID FROM deleted ) 
			DELETE FROM dbo.System_SubSecurityAdjustments WHERE TableName = 'Groups' AND TablePrimaryIDValue IN ( SELECT ID FROM deleted ) 
			--Break up membership inherition
			DELETE FROM [dbo].[MembershipsClones] WHERE ID_Group IN ( SELECT ID FROM deleted ) OR ID_ClonedGroup IN ( SELECT ID FROM deleted ) 
		END
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
	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowDeletesCount bigint
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			--Remove references of deleted apps
			DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_Application IN ( SELECT ID FROM deleted ) 
			DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_Application IN ( SELECT ID FROM deleted ) 
			DELETE FROM dbo.System_SubSecurityAdjustments WHERE TableName = 'Applications' AND TablePrimaryIDValue IN ( SELECT ID FROM deleted ) 
			DELETE FROM [dbo].[SecurityObjects_vs_NavItems] WHERE ID_SecurityObject IN ( SELECT ID FROM deleted )
			--Break up app-rights inherition
			UPDATE dbo.Applications_CurrentAndInactiveOnes SET AuthsAsAppID = NULL WHERE ID IN ( SELECT ID FROM deleted )
			DELETE [dbo].[ApplicationsRights_Inheriting] WHERE ID_Inheriting IN ( SELECT ID FROM deleted ) OR ID_Source IN ( SELECT ID FROM deleted )
		END
END

IF OBJECT_ID ('dbo.D_SecurityObjects_vs_NavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.D_SecurityObjects_vs_NavItems;
GO
CREATE TRIGGER dbo.D_SecurityObjects_vs_NavItems
   ON [dbo].SecurityObjects_vs_NavItems
   AFTER DELETE
AS 
BEGIN
	-- check for real changes - in case of 0 updated rows, don't forward 0-row-updates to sub-sequent triggers (would be a waste of time)
	DECLARE @RowDeletesCount bigint
	SELECT @RowDeletesCount = IsNull(COUNT_BIG(*), 0) FROM deleted;
	IF IsNull(@RowDeletesCount, 0) > 0
		BEGIN
			DELETE FROM dbo.NavItems WHERE ID IN ( SELECT ID_NavItem FROM deleted )
		END
END
