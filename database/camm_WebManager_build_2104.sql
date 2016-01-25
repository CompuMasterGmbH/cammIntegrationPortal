-- Remove all obsolete data from user table
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null;
GO

-- Deactivate all active sessions for safety between user session handling phase 1 "single login" and phase 2 "multiple simultaneous logins"
UPDATE [dbo].[System_WebAreasAuthorizedForSession]
SET inactive = 1;