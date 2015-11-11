<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" />

<xsl:template match="root">
<xsl:text disable-output-escaping="yes">--------------------------------------
-- UPDATE of table System_Languages --
--------------------------------------

-- Create temp table
CREATE TABLE #LanguageUpdates (
	[ID]                                INT NOT NULL,
	[Abbreviation]                      NVARCHAR(10),
	[Description_OwnLang]               NVARCHAR(255) NOT NULL,
	[Description_English]               NVARCHAR(255) NOT NULL,
	[Description_German]                NVARCHAR(255),
	[BrowserLanguageID]                 NVARCHAR(10),
	[AlternativeLanguage]               INT,
	[DirectionOfLetters]                VARCHAR(3)
) ON [PRIMARY]
GO

-- Insert language rows
</xsl:text>
<xsl:apply-templates select="systemlanguagesblock"/>
<xsl:text disable-output-escaping="yes">
GO

&lt;%IGNORE_ERRORS%&gt;
---------------------------------------------
-- Add new column RequiredUserProfileFlags --
---------------------------------------------
ALTER TABLE System_Languages ADD
	DirectionOfLetters varchar(3) NULL
GO

-- Update existing language items
UPDATE System_Languages
SET 
	System_Languages.Abbreviation = #LanguageUpdates.Abbreviation,
	System_Languages.Description_OwnLang = #LanguageUpdates.Description_OwnLang,
	System_Languages.Description_English = #LanguageUpdates.Description_English,
	System_Languages.Description_German = #LanguageUpdates.Description_German,
	System_Languages.BrowserLanguageID = #LanguageUpdates.BrowserLanguageID,
	System_Languages.AlternativeLanguage = #LanguageUpdates.AlternativeLanguage,
	System_Languages.DirectionOfLetters = #LanguageUpdates.DirectionOfLetters
FROM System_Languages INNER JOIN #LanguageUpdates ON System_Languages.ID = #LanguageUpdates.ID

-- Add new language items
IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages ON
INSERT INTO [dbo].[System_Languages] ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German]) 
SELECT #LanguageUpdates.[ID], #LanguageUpdates.[Abbreviation], #LanguageUpdates.[Description_OwnLang], #LanguageUpdates.[Description_English], #LanguageUpdates.[BrowserLanguageID], #LanguageUpdates.[AlternativeLanguage], #LanguageUpdates.[Description_German]
FROM #LanguageUpdates LEFT JOIN [dbo].[System_Languages] ON #LanguageUpdates.ID = [dbo].[System_Languages].ID
WHERE [dbo].[System_Languages].ID IS NULL
IF(	IDENT_INCR( 'dbo.System_Languages' ) IS NOT NULL OR IDENT_SEED('dbo.System_Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_Languages OFF

-- Removal of languages won't happen by this script
GO

-- Clean up
DROP TABLE #LanguageUpdates
GO
</xsl:text>
</xsl:template>

<xsl:template match="systemlanguagesblock">
<xsl:for-each select="dataroot">
<xsl:for-each select="systemlanguages">
<xsl:sort select="ID" data-type="number" order="ascending" />
<xsl:text>INSERT INTO #LanguageUpdates ([ID],[Abbreviation],[Description_OwnLang],[Description_English],[BrowserLanguageID],[AlternativeLanguage],[Description_German],[DirectionOfLetters]) 
VALUES(</xsl:text><xsl:value-of select="ID" /><xsl:text>,</xsl:text><xsl:value-of select="Abbreviation" /><xsl:text>,N</xsl:text><xsl:value-of select="Description_OwnLang" /><xsl:text>,</xsl:text><xsl:value-of select="Description_English" /><xsl:text>,</xsl:text><xsl:value-of select="BrowserLanguageID" /><xsl:text>,</xsl:text><xsl:if test="string-length(AlternativeLanguage) = 0"><xsl:text>NULL</xsl:text></xsl:if><xsl:value-of select="AlternativeLanguage" /><xsl:text>,</xsl:text><xsl:value-of select="Description_German" /><xsl:text>,</xsl:text><xsl:value-of select="DirectionOfLetters" /><xsl:text>)
</xsl:text>
</xsl:for-each>
</xsl:for-each>
</xsl:template>

</xsl:stylesheet>