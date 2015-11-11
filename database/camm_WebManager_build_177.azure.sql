--------------------------------------------------------------------------------------------------------------------------------
-- FIX for previous build 162 (which has been already fixed, too): Remove SP which has got schema name of the current DB user --
--------------------------------------------------------------------------------------------------------------------------------
IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo_camm_WebManager].[LogMissingExternalUserAssignment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo_camm_WebManager.LogMissingExternalUserAssignment
GO

IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[LogMissingExternalUserAssignment]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.LogMissingExternalUserAssignment
GO
CREATE PROCEDURE dbo.LogMissingExternalUserAssignment
(
	@ExternalAccountSystem nvarchar(50),
	@LogonName nvarchar(100),
	@FullUserName nvarchar(150),
	@EMailAddress nvarchar(250),
	@Error ntext,
	@Remove bit
)
WITH ENCRYPTION
AS
IF @Remove = 0
	BEGIN
		-- Do an ADD if required
		DECLARE @ID int
		SELECT @ID = ID
		FROM [Log_MissingAssignmentsOfExternalAccounts]
		WHERE [ExternalAccountSystem] = @ExternalAccountSystem 
			AND [UserName] = @LogonName
		IF @ID IS NULL
			INSERT INTO [Log_MissingAssignmentsOfExternalAccounts] ([ExternalAccountSystem], [UserName], [FullName], [EMailAddress], [Error])
			VALUES (@ExternalAccountSystem, @LogonName, @FullUserName, @EMailAddress, @Error) 
	END
ELSE
	BEGIN
		-- Do a REMOVE
		DELETE 
		FROM [Log_MissingAssignmentsOfExternalAccounts]
		WHERE [ExternalAccountSystem] = @ExternalAccountSystem 
			AND [UserName] = @LogonName
	END
GO