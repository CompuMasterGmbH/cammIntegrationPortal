IF OBJECT_ID ('dbo.IUD_Apps2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_Apps2SecObjAndNavItems;
GO
CREATE TRIGGER [dbo].[IUD_Apps2SecObjAndNavItems] 
   ON  [dbo].[Applications_CurrentAndInactiveOnes] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
    -- DEACTIVATED BY JOCHEN WEZEL ON 2013/2014 BECAUSE OF TOO MANY DEADLOCKS ERRORS - TO BE REACTIVATED WHEN STABLE: EXEC RefillSplittedSecObjAndNavPointsTables
END
GO

IF OBJECT_ID ('dbo.IUD_AuthsUsers2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_AuthsUsers2SecObjAndNavItems;
GO
CREATE TRIGGER [dbo].[IUD_AuthsUsers2SecObjAndNavItems] 
   ON  [dbo].[ApplicationsRightsByUser] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
    -- DEACTIVATED BY JOCHEN WEZEL ON 2013/2014 BECAUSE OF TOO MANY DEADLOCKS ERRORS - TO BE REACTIVATED WHEN STABLE: EXEC RefillSplittedSecObjAndNavPointsTables
END
GO

IF OBJECT_ID ('dbo.IUD_AuthsGroups2SecObjAndNavItems', 'TR') IS NOT NULL
   DROP TRIGGER dbo.IUD_AuthsGroups2SecObjAndNavItems;
GO
CREATE TRIGGER [dbo].[IUD_AuthsGroups2SecObjAndNavItems] 
   ON  [dbo].[ApplicationsRightsByGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
    -- DEACTIVATED BY JOCHEN WEZEL ON 2013/2014 BECAUSE OF TOO MANY DEADLOCKS ERRORS - TO BE REACTIVATED WHEN STABLE: EXEC RefillSplittedSecObjAndNavPointsTables
END
GO

IF OBJECT_ID ('dbo.Benutzer_PostUserDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Benutzer_PostUserDeletionTrigger;
GO
CREATE TRIGGER dbo.Benutzer_PostUserDeletionTrigger
   ON [dbo].Benutzer
   AFTER DELETE
AS 
BEGIN
	--Remove references of deleted users
	DELETE FROM dbo.Memberships WHERE ID_User IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.SecurityObjects_AuthsByUser WHERE ID_USER IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.System_UserSessions WHERE ID_USER IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.System_SubSecurityAdjustments WHERE UserID IN ( SELECT ID FROM deleted ) 

	--Log the fact that user has been deleted.
	INSERT INTO dbo.Log_Users 
	(ID_USER, Type, VALUE, ModifiedOn)
	SELECT ID, 'DeletedOn', GetDate(), GETDATE() FROM deleted
END
GO
IF OBJECT_ID ('dbo.Gruppen_PostUserDeletionTrigger', 'TR') IS NOT NULL
   DROP TRIGGER dbo.Gruppen_PostUserDeletionTrigger;
GO
CREATE TRIGGER dbo.Gruppen_PostUserDeletionTrigger
   ON [dbo].Gruppen
   AFTER DELETE
AS 
BEGIN
	--Remove references of deleted users
	DELETE FROM dbo.Memberships WHERE ID_Group IN ( SELECT ID FROM deleted ) 
	DELETE FROM dbo.SecurityObjects_AuthsByGroup WHERE ID_Group IN ( SELECT ID FROM deleted ) 
END
GO
