-- make Dutch language correct
update [dbo].[System_Languages] set BrowserLanguageID = 'nl' where ID = 311
delete from [dbo].[System_Languages] where ID = 553
GO