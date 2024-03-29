------------------------------------
-- New CWM markets (>= 4000) --
------------------------------------
------------------------------------
-- New virtual markets (>= 10000) --
------------------------------------
DECLARE @MyLang int
IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages ON
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10019
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10019, NULL, 'Spanish (South America)', 'Spanish (South America)', 'Spanisch (Süd Amerika)', NULL, 4, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10020
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10020, NULL, 'Portuguese (South America)', 'Portuguese (South America)', 'Portugisisch (Süd Amerika)', NULL, 345, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 4001
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(4001, NULL, 'Portuguese (Portugal)', 'Portuguese (Portugal)', 'Portugisisch (Portugal)', NULL, 345, 0)
	END
IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages OFF
GO