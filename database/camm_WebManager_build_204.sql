-- Remove all obsolete data from user table
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null