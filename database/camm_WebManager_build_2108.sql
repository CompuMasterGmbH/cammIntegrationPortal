-- =========================================================================================================
-- ===== AUTHORIZATIONS TABLES - PRE-STAGED/PRE-CALCULATED, EFFECTIVE
-- =========================================================================================================
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
-- ===== SUPERVISOR'S ALWAYS ACCESS RULE
-- =========================================================================================================
------------------------------------------------------------------------------------------------------------
-- Cleanup of old supervisor authorizations for system apps
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