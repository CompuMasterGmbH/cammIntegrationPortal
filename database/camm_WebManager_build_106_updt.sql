---------------------------------------------------------------------------
-- UserID 0 --> UserID 1  (ex admin ID had been 0, but has changed to 1) --
---------------------------------------------------------------------------

UPDATE [dbo].[Applications_CurrentAndInactiveOnes]
SET [ReleasedBy]=1
WHERE [ReleasedBy]=0
UPDATE [dbo].[Applications_CurrentAndInactiveOnes]
SET [ModifiedBy]=1 
WHERE [ModifiedBy]=0

UPDATE [dbo].[ApplicationsRightsByGroup]
SET [ReleasedBy]=1
WHERE [ReleasedBy]=0

UPDATE [dbo].[ApplicationsRightsByUser]
SET [ReleasedBy]=1
WHERE [ReleasedBy]=0

UPDATE [dbo].[Gruppen]
SET [ReleasedBy]=1
WHERE [ReleasedBy]=0
UPDATE [dbo].[Gruppen]
SET [ModifiedBy]=1
WHERE [ModifiedBy]=0

UPDATE [dbo].[Memberships]
SET [ReleasedBy]=1
WHERE [ReleasedBy]=0

UPDATE [System_AccessLevels]
SET [ReleasedBy]=1
WHERE [ReleasedBy]=0
UPDATE [System_AccessLevels]
SET [ModifiedBy]=1
WHERE [ModifiedBy]=0

UPDATE dbo.System_ServerGroups
SET [ModifiedBy]=1
WHERE [ModifiedBy]=0

-- Remove old flags instead of overwriting newer ones with the correct User ID value
DELETE FROM [dbo].[Log_Users]
WHERE ID_User = 0
GO

--------------------------------------------
-- Add missing owners of security objects --
--------------------------------------------

insert into dbo.System_SubSecurityAdjustments (userid, tablename, tableprimaryidvalue, authorizationtype)
select dbo.Applications_CurrentAndInactiveOnes.releasedby, 'Applications', dbo.Applications_CurrentAndInactiveOnes.id, 'Owner'
from dbo.Applications_CurrentAndInactiveOnes left join dbo.System_SubSecurityAdjustments
	on dbo.Applications_CurrentAndInactiveOnes.id = dbo.System_SubSecurityAdjustments.tableprimaryidvalue
	and dbo.System_SubSecurityAdjustments.tablename = 'Applications'
where dbo.System_SubSecurityAdjustments.userid is null
	and dbo.Applications_CurrentAndInactiveOnes.appdeleted = 0
order by dbo.Applications_CurrentAndInactiveOnes.id 
GO

----------------------------------
-- Add missing owners of groups --
----------------------------------

insert into dbo.System_SubSecurityAdjustments (userid, tablename, tableprimaryidvalue, authorizationtype)
select dbo.gruppen.releasedby, 'Groups', dbo.gruppen.id, 'Owner'
from dbo.gruppen left join dbo.System_SubSecurityAdjustments
	on dbo.gruppen.id = dbo.System_SubSecurityAdjustments.tableprimaryidvalue
	and dbo.System_SubSecurityAdjustments.tablename = 'Groups'
where dbo.System_SubSecurityAdjustments.userid is null
order by dbo.gruppen.id 