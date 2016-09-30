-- =========================================================================================================
-- ===== DROP AUTO-AUTHS OF OLD AUTH-DESIGN (WILL BE REINSERTED ON NEXT DB PATCH)
-- =========================================================================================================
-- delete all previous supervisor auths for system apps (will be re-inserted with IsSupervisorAutoAccessRule on next db patch
DELETE [dbo].[ApplicationsRightsByGroup]
FROM [dbo].[ApplicationsRightsByGroup]
WHERE ID_GroupOrPerson IN (-6,6) 
	AND [IsSupervisorAutoAccessRule] = 0
	AND [IsDenyRule] = 0
	AND ID_Application IN 
	(
		SELECT [ID]
		FROM [dbo].[Applications_CurrentAndInactiveOnes]
		where systemapptype in (2,3)
)
GO

-- =========================================================================================================
-- ===== AUTHORIZATIONS TABLES - PRE-STAGED/PRE-CALCULATED, EFFECTIVE
-- =========================================================================================================
---------------------------------------------------------------------------------------------------
-- CLEANUP OF ALL PRE-EXISTING DATA IN PRE-STAGING-TABLES
---------------------------------------------------------------------------------------------------
	TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging4AllowDenyRules
	TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging3GroupsResolved
	TRUNCATE TABLE dbo.ApplicationsRightsByGroup_PreStaging2Inheritions
	TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging2Inheritions
	TRUNCATE TABLE dbo.ApplicationsRightsByGroup_PreStaging1ForRealServerGroup
	TRUNCATE TABLE dbo.ApplicationsRightsByUser_PreStaging1ForRealServerGroup
GO
---------------------------------------------------------------------------------------------------
-- PRE-FILL ALL PRE-STAGING-LEVELS
---------------------------------------------------------------------------------------------------
	-- Force first table filling (or full update on repetitions)
	UPDATE [ApplicationsRightsByGroup]
	SET IsDenyRule = IsDenyRule
GO
	-- Force first table filling (or full update on repetitions)
	UPDATE [ApplicationsRightsByUser]
	SET IsDenyRule = IsDenyRule
GO
-- =========================================================================================================
-- ===== SUPERVISOR'S ALWAYS ACCESS RULE
-- =========================================================================================================
------------------------------------------------------------------------------------------------------------
-- Cleanup of old supervisor authorizations for system apps (in same transactions as following rebuild command)
------------------------------------------------------------------------------------------------------------
	DELETE dbo.[ApplicationsRightsByGroup]
	FROM applications
		INNER JOIN [dbo].[ApplicationsRightsByGroup] 
			ON applications.id = [ApplicationsRightsByGroup].ID_Application
	WHERE IsSupervisorAutoAccessRule = 0
		AND id_grouporperson = 6
		AND SystemApp <> 0;
------------------------------------------------------------------------------------------------------------
-- Force first table filling (or full update on repetitions)
------------------------------------------------------------------------------------------------------------
	UPDATE [dbo].[Applications_CurrentAndInactiveOnes] 
	SET TitleAdminArea = TitleAdminArea
GO
-- =========================================================================================================
-- ===== MEMBERSHIPS SYSTEM RULES
-- =========================================================================================================
-----------------------------------------------------------------------
-- RE-CREATE ALL MEMBERSHIPS SYSTEM RULES FOR ANONYMOUS USERS
-----------------------------------------------------------------------
	-- reset all system rules (=rules for anonymous+public groups)
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRuleOfServerGroupID IS NOT NULL;
	-- insert anonymous groups memberships to user with ID -1 (for every server group)
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRuleOfServerGroupID)
	SELECT ID_Group_Anonymous, -1, GETDATE(), -43, 0, ID
	FROM [dbo].[System_ServerGroups]
GO
-----------------------------------------------------------------------
-- RE-CREATE ALL MEMBERSHIPS SYSTEM RULES FOR REAL USERS
-----------------------------------------------------------------------
	-- reset all system rules (=rules for anonymous+public groups)
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRuleOfServerGroupsAndTheirUserAccessLevelsID IS NOT NULL;
	-- insert public groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRuleOfServerGroupsAndTheirUserAccessLevelsID)
	SELECT [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, dbo.System_ServerGroupsAndTheirUserAccessLevels.ID
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	-- insert anonymous groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRuleOfServerGroupsAndTheirUserAccessLevelsID)
	SELECT [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, dbo.System_ServerGroupsAndTheirUserAccessLevels.ID
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
GO
-- =========================================================================================================
-- ===== CLEANUP OF OLD USER SESSION VALUES
-- =========================================================================================================
DELETE 
FROM [dbo].[System_SessionValues] 
WHERE [dbo].[System_SessionValues].SessionID NOT IN 
	(
		SELECT [ID_Session] FROM [dbo].[System_UserSessions]
	)
   