---------------------------------------------------------------------------------------------------------------------------
-- FIX for previous build 162 (which is fixed herewith, now): Remove SP which has got schema name of the current DB user --
---------------------------------------------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo_camm_WebManager].[LogMissingExternalUserAssignment]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo_camm_WebManager.LogMissingExternalUserAssignment

------------------------------------------------------
-- TABLE [Log_MissingAssignmentsOfExternalAccounts] --
------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo].[Log_MissingAssignmentsOfExternalAccounts]') AND OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Log_MissingAssignmentsOfExternalAccounts]
GO

CREATE TABLE [dbo].[Log_MissingAssignmentsOfExternalAccounts]
(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ExternalAccountSystem] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[FullName] [nvarchar](150) NULL,
	[EMailAddress] [nvarchar](250) NULL,
	[Error] [ntext] NULL,
 CONSTRAINT [PK_Log_MissingAssignmentsOfExternalAccounts] PRIMARY KEY CLUSTERED 
 (
	[ID] 
 )
)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = object_id(N'[dbo].[Log_MissingAssignmentsOfExternalAccounts]') AND name = N'IX_Log_MissingAssignmentsOfExternalAccounts')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Log_MissingAssignmentsOfExternalAccounts] ON [dbo].[Log_MissingAssignmentsOfExternalAccounts] 
(
	[ExternalAccountSystem],
	[UserName] 
)
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo].[LogMissingExternalUserAssignment]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
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