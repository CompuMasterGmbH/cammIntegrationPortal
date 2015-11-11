-- New Column for Algorithm the password is encrypted/hashed with
IF COL_LENGTH('[dbo].Benutzer', 'LoginPWAlgorithm') IS NULL 
BEGIN
	ALTER TABLE [dbo].Benutzer ADD LoginPWAlgorithm integer
END

IF COL_LENGTH('[dbo].Benutzer', 'LoginPWNonceValue') IS NULL
BEGIN
	ALTER TABLE [dbo].Benutzer ADD LoginPWNonceValue varbinary(4096) --Means IV or Salt.
END
GO
UPDATE [dbo].Benutzer SET LoginPwNonceValue = 0x00, LoginPwALgorithm = 0 WHERE LoginPwAlgorithm IS NULL And LoginPwNonceVALUE IS NULL
GO
IF NOT EXISTS( SELECT 1 FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LoginPWAlgorithm' ) 
BEGIN
	-- Set the algorithm that should be used
	INSERT INTO [dbo].[System_GlobalProperties]
			   ([PropertyName],
			   [ValueNVarChar],
			   [ValueNText],
			   [ValueInt],
			   [ValueBoolean],
			   [ValueImage],
			   [ValueDecimal],
			   [ValueDateTime])
		 VALUES (
			   'LoginPWAlgorithm',
			   'camm WebManager',
			   NULL,
			   0,
			   NULL,
			   NULL,	
			   NULL,
			   NULL
	)
END


IF NOT EXISTS( SELECT 1 FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'PasswordResetBehavior' ) 
BEGIN
	--Set the password reset behavior
	INSERT INTO [dbo].[System_GlobalProperties]
			   ([PropertyName]
			   ,[ValueNVarChar]
			   ,[ValueNText]
			   ,[ValueInt]
			   ,[ValueBoolean]
			   ,[ValueImage]
			   ,[ValueDecimal]
			   ,[ValueDateTime])
		 VALUES (
			   'PasswordResetBehavior'
			   ,'camm WebManager'
			   ,NULL
			   ,0
			   ,NULL
			   ,NULL	
			   ,NULL
			   ,NULL
	)
END