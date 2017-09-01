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
------------------------------------------------------------------------------------------------------------
-- Force first table filling (or full update on repetitions)
------------------------------------------------------------------------------------------------------------
	UPDATE [dbo].[Applications_CurrentAndInactiveOnes] 
	SET TitleAdminArea = TitleAdminArea
GO