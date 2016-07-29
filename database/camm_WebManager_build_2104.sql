-- Remove all obsolete data from user table
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null;
GO

-- Deactivate all active sessions for safety between user session handling phase 1 "single login" and phase 2 "multiple simultaneous logins"
UPDATE [dbo].[System_WebAreasAuthorizedForSession]
SET inactive = 1;
GO

-- Reset all app's SystemAppTypes with negative values since not required any more (app's LocationID has got the same information)
UPDATE [dbo].[Applications_CurrentAndInactiveOnes]
SET systemapptype = NULL
where isNull(systemapptype,0) < 0
GO
