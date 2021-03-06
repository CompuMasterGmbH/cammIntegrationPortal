----------------------------------------------------
-- dbo.AdminPrivate_SetAuthorizationInherition
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetAuthorizationInherition 
(
@ReleasedByUserID int, 
@IDApp int, 
@InheritsFrom int
)

AS 
SET NOCOUNT ON
UPDATE    dbo.Applications
SET              AuthsAsAppID = @InheritsFrom, ModifiedBy = @ReleasedByUserID, ModifiedOn = getdate()
WHERE     (ID = @IDApp)
-- Logging
If (@InheritsFrom Is Null) 
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 31, 'Application now inhertis from nothing')
	End
Else
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 31, 'Application now inhertis from ID ' + Convert(varchar(50), @InheritsFrom))
	End
SET NOCOUNT ON
SELECT Result = -1

GO

-- fix the log items in the history
UPDATE dbo.Log
SET ConflictType = 31
WHERE ConflictType = 1 AND ApplicationID IS NOT NULL