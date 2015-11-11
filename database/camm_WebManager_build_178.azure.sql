IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_RenameLoginName]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.AdminPrivate_RenameLoginName
GO
CREATE PROCEDURE dbo.AdminPrivate_RenameLoginName
(
	@UserID int,
	@LogonName nvarchar(50)
)
WITH ENCRYPTION
AS
DECLARE @CurrentLoginName nvarchar(50)
SELECT @CurrentLoginName = LoginName FROM Benutzer WHERE ID = @UserID

IF @CurrentLoginName Is Not Null
	If @CurrentLoginName <> @LogonName 
		-- Rename only if different logon names
		UPDATE Benutzer SET LoginName = @LogonName WHERE ID = @UserID

GO