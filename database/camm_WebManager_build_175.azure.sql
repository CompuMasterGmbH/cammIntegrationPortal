IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[IsAdministratorForAuthorizations]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsAdministratorForAuthorizations]
GO
CREATE PROC [dbo].[IsAdministratorForAuthorizations]
(
	@AuthorizationType nvarchar(50),
	@AdminUserID int,
	@SecurityObjectID int
)
WITH ENCRYPTION
AS 
DECLARE @Result int
select top 1 @Result = ID 
from System_SubSecurityAdjustments 
where 
	(
		(
			(UserID=@AdminUserID and TableName='Applications' and AuthorizationType='Owner' and TablePrimaryIDValue = @SecurityObjectID)
			OR (UserID=@AdminUserID and TableName='Applications' and AuthorizationType=@AuthorizationType and TablePrimaryIDValue = @SecurityObjectID)
			OR (UserID=@AdminUserID and TableName='Applications' and AuthorizationType='SecurityMaster' and TablePrimaryIDValue = 0)
		)
		AND @AdminUserID IN (SELECT ID FROM Benutzer WHERE ID = @AdminUserID AND LoginDisabled = 0) -- user must still be valid
		AND @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 7 AND ID_User = @AdminUserID) -- user must still be security admin
	)
	OR @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 6 AND ID_User = @AdminUserID) -- ALTERNATIVELY user must be a supervisor
IF @Result IS NULL 
	RETURN 0
ELSE
	RETURN 1
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[IsAdministratorForMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[IsAdministratorForMemberships]
GO
CREATE PROC [dbo].[IsAdministratorForMemberships]
(
	@AuthorizationType nvarchar(50),
	@AdminUserID int,
	@GroupID int
)
WITH ENCRYPTION
AS 
DECLARE @Result int
select top 1 @Result = ID 
from System_SubSecurityAdjustments 
where 
	(
		(
			(UserID=@AdminUserID and TableName='Groups' and AuthorizationType='Owner' and TablePrimaryIDValue = @GroupID)
			OR (UserID=@AdminUserID and TableName='Groups' and AuthorizationType=@AuthorizationType and TablePrimaryIDValue = @GroupID)
			OR (UserID=@AdminUserID and TableName='Groups' and AuthorizationType='SecurityMaster' and TablePrimaryIDValue = 0)
		)
		AND @AdminUserID IN (SELECT ID FROM Benutzer WHERE ID = @AdminUserID AND LoginDisabled = 0) -- user must still be valid
		AND @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 7 AND ID_User = @AdminUserID) -- user must still be security admin
	)
	OR @AdminUserID IN (SELECT ID_User FROM Memberships WHERE ID_Group = 6 AND ID_User = @AdminUserID) -- ALTERNATIVELY user must be a supervisor
IF @Result IS NULL 
	RETURN 0
ELSE
	RETURN 1
GO