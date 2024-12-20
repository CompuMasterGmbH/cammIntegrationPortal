-- ATTENTION: This is a fix for Build 167. Datacolumn type where wrong and the whole update process incomplete!!! --
----------------------------------------------------
-- dbo.System_SubSecurityAdjustments
----------------------------------------------------
if exists (select * from dbo.syscolumns where id = object_id('dbo.System_SubSecurityAdjustments') and name = 'ReleasedOn') 
ALTER TABLE [dbo].[System_SubSecurityAdjustments]
DROP COLUMN ReleasedOn
GO
if exists (select * from dbo.syscolumns where id = object_id('dbo.System_SubSecurityAdjustments') and name = 'ReleasedBy') 
ALTER TABLE [dbo].[System_SubSecurityAdjustments]
DROP COLUMN ReleasedBy;
GO
if not exists (select * from dbo.syscolumns where id = object_id('dbo.System_SubSecurityAdjustments') and name = 'ReleasedOn') 
ALTER TABLE [dbo].[System_SubSecurityAdjustments]
ADD ReleasedOn DATETIME NULL
GO
if not exists (select * from dbo.syscolumns where id = object_id('dbo.System_SubSecurityAdjustments') and name = 'ReleasedBy') 
ALTER TABLE [dbo].[System_SubSecurityAdjustments]
ADD ReleasedBy INT NULL;
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

		SELECT @NewAppID = @@IDENTITY

		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Applications', @NewAppID, 'Owner', @ReleasedByUserID

		SET NOCOUNT OFF
		SELECT Result = @NewAppID
	END
Else
	
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateGroup 
(
	@ReleasedByUserID int,
	@Name nvarchar(50),
	@Description nvarchar(1024)
)

AS
DECLARE @CurUserID int
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		
		SELECT Result = -1
		
		INSERT INTO dbo.Gruppen (Name, Description, ReleasedBy, ModifiedBy) VALUES (@Name, @Description, @ReleasedByUserID, @ReleasedByUserID)

		EXEC AdminPrivate_UpdateSubSecurityAdjustment 1, @ReleasedByUserID, 'Groups', @@IDENTITY, 'Owner', @ReleasedByUserID
	END
Else
	
	SELECT Result = 0
GO