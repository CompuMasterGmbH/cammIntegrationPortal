ALTER procedure [dbo].[AdminPrivate_UpdateSubSecurityAdjustment]
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