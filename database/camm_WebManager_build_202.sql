IF OBJECT_ID('dbo.Log_TextMessages', 'U') IS NOT NULL
  DROP TABLE dbo.Log_TextMessages; 
  
--After how many days to delete mails
IF NOT EXISTS( SELECT 1 FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'DeleteMailsAfterDays' ) 
BEGIN
	INSERT INTO dbo.System_GlobalProperties (PropertyName, ValueInt) VALUES ('DeleteMailsAfterDays', 7)
END


--After how many days to delete deactivated users
IF NOT EXISTS( SELECT 1 FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'DeleteUsersAfterDays' ) 
BEGIN
	INSERT INTO dbo.System_GlobalProperties (PropertyName, ValueInt) VALUES ('DeleteUsersAfterDays', 0)
END

--After how many days to anonymize IP addresses
IF NOT EXISTS( SELECT 1 FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'AnonymizeIPsAfterDays' ) 
BEGIN
	INSERT INTO dbo.System_GlobalProperties (PropertyName, ValueInt) VALUES ('AnonymizeIPsAfterDays', 7)
END

