ALTER VIEW [dbo].[Languages]
AS
SELECT     ID, Abbreviation, Description_OwnLang, Description_English AS Description, IsActive, BrowserLanguageID, AlternativeLanguage, DirectionOfLetters
FROM         dbo.System_Languages
GO
