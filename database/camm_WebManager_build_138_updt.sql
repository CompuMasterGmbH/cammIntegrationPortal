----------------------------------------------------
-- dbo.AdminPrivate_DeleteUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteUser
	(
		@UserID int,
		@AdminUserID int = null
	)

AS


DELETE FROM dbo.Benutzer WHERE ID=@UserID
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson=@UserID
DELETE FROM dbo.Memberships WHERE ID_User=@UserID

-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', -31, 'User deleted by admin ' + Cast(IsNull(@AdminUserID, '') as nvarchar(20)))
GO
