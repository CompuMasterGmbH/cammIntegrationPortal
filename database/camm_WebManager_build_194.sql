--default 4 years for existing installations (set by JW)
IF NOT EXISTS (SELECT 1 FROM [dbo].System_GlobalProperties WHERE PropertyName = 'MaxRetentionDays' AND ValueNVarchar = 'camm WebManager')
INSERT INTO [dbo].System_GlobalProperties (PropertyName, ValueNVarChar, ValueInt) VALUES ('MaxRetentionDays', 'camm WebManager', 365 * 4 )  
GO