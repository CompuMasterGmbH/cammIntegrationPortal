-----------------------------------------------------------------------
-- RE-CREATE ALL MEMBERSHIPS SYSTEM RULES FOR ANONYMOUS USERS
-----------------------------------------------------------------------
	update System_ServerGroups
	set AccessLevel_Default = AccessLevel_Default
	from System_ServerGroups
GO
-----------------------------------------------------------------------
-- RE-CREATE ALL MEMBERSHIPS SYSTEM RULES FOR REAL USERS
-----------------------------------------------------------------------
	-- reset all system rules (=rules for anonymous+public groups)
	DELETE dbo.Memberships 
	FROM dbo.Memberships 
	WHERE dbo.Memberships.IsSystemRule <> 0
		AND ID_User <> -1
	-- insert public groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Public, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
	-- insert anonymous groups memberships
	INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedOn, ReleasedBy, IsDenyRule, IsSystemRule)
	SELECT [dbo].[System_ServerGroups].ID_Group_Anonymous, [dbo].[Benutzer].ID AS ID_User, GETDATE(), -43, 0, 1
	FROM dbo.System_ServerGroupsAndTheirUserAccessLevels
		INNER JOIN [dbo].[Benutzer] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = [dbo].[Benutzer].AccountAccessability
		INNER JOIN [dbo].[System_ServerGroups] ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = [dbo].[System_ServerGroups].ID
GO