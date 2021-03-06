------------------------------------
-- New virtual markets (>= 10000) --
------------------------------------
DECLARE @MyLang int
IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages ON
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10001
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10001, NULL, 'Espanol (Latin America/Caribbean)', 'Spanish (Latin America/Caribbean)', 'Spanisch (Latein-Amerika/Karibik)', NULL, 4, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10002
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10002, NULL, 'English (Latin America/Caribbean)', 'English (Latin America/Caribbean)', 'Englisch (Latein-Amerika/Karibik)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10003
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10003, NULL, 'Arabic (Middle East/Africa)', 'Arabic (Middle East/Africa)', 'Arabic (Middle East/Africa)', NULL, 21, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10004
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10004, NULL, 'English (Europe/Middle East/Africa)', 'English (Middle East/Africa)', 'English (Middle East/Africa)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10005
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10005, NULL, 'English (Asia/Pacific)', 'English (Asia/Pacific)', 'English (Asia/Pacific)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10006
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10006, NULL, 'English (Americas)', 'English (Americas)', 'English (Americas)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10007
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10007, NULL, 'English (Asia)', 'English (Asia)', 'English (Asia)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10008
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10008, NULL, 'English (Pacific)', 'English (Pacific)', 'English (Pacific)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10009
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10009, NULL, 'Arabic (Middle East)', 'Arabic (Middle East)', 'Arabic (Middle East)', NULL, 21, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10010
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10010, NULL, 'English (Middle East)', 'English (Middle East)', 'English (Middle East)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10011
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10011, NULL, 'English (Africa)', 'English (Africa)', 'English (Africa)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10012
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10012, NULL, 'English (Europe)', 'English (Europe)', 'English (Europa)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10013
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10013, NULL, 'English (European Union)', 'English (European Union)', 'English (Europäische Union)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10014
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10014, NULL, 'English (Latin America)', 'English (Latin America)', 'English (Latein-Amerika)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10015
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10015, NULL, 'Espanol (Latin America)', 'Spanish (Latin America)', 'Spanisch (Latein-Amerika)', NULL, 4, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10016
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10016, NULL, 'Espanol (Caribbean)', 'Spanish (Caribbean)', 'Spanisch (Karibik)', NULL, 4, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10017
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10017, NULL, 'English (Caribbean)', 'English (Caribbean)', 'English (Karibik)', NULL, 1, 0)
	END
SELECT @MyLang = ID FROM [dbo].[System_Languages] WHERE ID = 10018
if @MyLang IS NULL
	BEGIN
		INSERT INTO [dbo].[System_Languages]([ID], [Abbreviation], [Description_OwnLang], [Description_English], [Description_German], [BrowserLanguageID], [AlternativeLanguage], [IsActive])
		VALUES(10018, NULL, 'English (Middle East/Africa)', 'English (Middle East/Africa)', 'Englisch (Mittlerer Osten/Afrika)', NULL, 1, 0)
	END
IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages OFF
GO