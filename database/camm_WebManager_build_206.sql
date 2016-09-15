---------------------------------------------------------------------------------------------------------------------
-- PREPARE DATA CHANGES BY FOLLOWING STATEMENTS BY TEMPORARY UPDATE OF TRIGGERS TO WORK AGAIN (FULL ROLL-OVER WILL FOLLOW IN LATER DB PATCHES)
---------------------------------------------------------------------------------------------------------------------
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
		END
END
GO
---------------------------------------------------------------------------------------------------------------------
-- SPLIT Anonymous GROUP 58 FOR ALL SERVER GROUPS INTO SEPARATE ANONYMOUS GROUPS
---------------------------------------------------------------------------------------------------------------------
DECLARE @ServerGroupID int
DECLARE @ServerGroupAnonymousGroupID int
DECLARE @Group_Anonymous_Name nvarchar(255)

-- Watch out for a server group with an obsolete anonymous group with ID 58
SELECT TOP 1 @ServerGroupID = ID, 
	@ServerGroupAnonymousGroupID = ID_Group_Anonymous,
	@Group_Anonymous_Name = 'Anonymous ' + SubString(ServerGroup, 1, 242)
FROM dbo.System_ServerGroups
WHERE ID_Group_Anonymous = 58

WHILE @ServerGroupID IS NOT NULL
	BEGIN
		-- Anonymous Group anlegen
		INSERT INTO dbo.Gruppen
					(Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy)
		SELECT     @Group_Anonymous_Name AS Name, 'System group: all anonymous users (without being logged on)' AS ServerDescription, 
					GETDATE() AS ReleasedOn, -43 AS ReleasedBy, 
					1 AS SystemGroup, GETDATE() AS ModifiedOn, -43 AS ModifiedBy
		SELECT @ServerGroupAnonymousGroupID = SCOPE_IDENTITY()

		-- Public Group Security Adjustments
		INSERT INTO [dbo].[System_SubSecurityAdjustments] 
			(UserID, TableName, TablePrimaryIDValue, AuthorizationType)
		SELECT UserID, TableName, @ServerGroupAnonymousGroupID, AuthorizationType
		FROM [dbo].[System_SubSecurityAdjustments]
		WHERE TableName = 'Groups'
			AND TablePrimaryIDValue = 58

		-- insert copy of group auths for new anonymous group based on auths for obsolete anonymous group
		INSERT INTO [dbo].[ApplicationsRightsByGroup]
			(
			   [ID_Application]
			  ,[ID_GroupOrPerson]
			  ,[ReleasedOn]
			  ,[ReleasedBy]
			  ,[DevelopmentTeamMember]
			  ,[IsDenyRule]
			  ,[ID_ServerGroup]
			)
		SELECT [ID_Application]
			  ,@ServerGroupAnonymousGroupID
			  ,[ReleasedOn]
			  ,[ReleasedBy]
			  ,[DevelopmentTeamMember]
			  ,[IsDenyRule]
			  ,[ID_ServerGroup]
		FROM [dbo].[ApplicationsRightsByGroup]
		WHERE ID_GroupOrPerson = 58

		-- update the server group to reference the newly created group
		UPDATE dbo.System_ServerGroups
		SET ID_Group_Anonymous = @ServerGroupAnonymousGroupID
		WHERE ID = @ServerGroupID

		SELECT @ServerGroupID,	@ServerGroupAnonymousGroupID, @Group_Anonymous_Name

		-- Watch out for a server group with an obsolete anonymous group with ID 58
		SELECT @ServerGroupID = NULL,
			@ServerGroupAnonymousGroupID = NULL,
			@Group_Anonymous_Name = NULL
		SELECT TOP 1 @ServerGroupID = ID, 
			@ServerGroupAnonymousGroupID = ID_Group_Anonymous,
			@Group_Anonymous_Name = 'Anonymous ' + SubString(ServerGroup, 1, 242)
		FROM dbo.System_ServerGroups
		WHERE ID_Group_Anonymous = 58
	END

DELETE
FROM [dbo].[ApplicationsRightsByGroup]
WHERE ID_GroupOrPerson = 58;
DELETE 
FROM dbo.Gruppen
WHERE ID = 58;
