--limit retention time for log type RuntimeInformation to max 30 days (for high load reasons because of regular webcron cleanup log entries increasing table size to too large values for typical shared hostings)
IF NOT EXISTS (SELECT 1 FROM [dbo].System_GlobalProperties WHERE PropertyName = 'ConflictTypeAge' AND ValueNVarchar = 'camm WebManager' AND ValueInt = -70)
INSERT INTO [dbo].System_GlobalProperties (PropertyName, ValueNVarChar, ValueInt, ValueDecimal) VALUES ('MaxRetentionDays', 'camm WebManager', -70, 30)  
GO
UPDATE [dbo].System_GlobalProperties 
SET ValueDecimal = 30
WHERE PropertyName = 'MaxRetentionDays', ValueNVarChar = 'camm WebManager', ValueInt = -70, ValueDecimal > 30  
