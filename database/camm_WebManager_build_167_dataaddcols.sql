if not exists (select * from dbo.syscolumns where id = object_id('dbo.System_SubSecurityAdjustments') and name = 'ReleasedOn') 
ALTER TABLE [dbo].[System_SubSecurityAdjustments]
ADD ReleasedOn INT NULL
GO
if not exists (select * from dbo.syscolumns where id = object_id('dbo.System_SubSecurityAdjustments') and name = 'ReleasedBy') 
ALTER TABLE [dbo].[System_SubSecurityAdjustments]
ADD ReleasedBy DATETIME NULL;
