<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" />

<xsl:template match="root">
<xsl:text disable-output-escaping="yes">&lt;%
</xsl:text>
<xsl:apply-templates select="declarationsblock"/>
<xsl:apply-templates select="browseridsblock"/>
<xsl:apply-templates select="languagesblock"/>
<xsl:apply-templates select="markets2languagesblock"/>
<xsl:apply-templates select="datetimeblock"/>
<xsl:apply-templates select="languagesblock" mode="supportedlanguagesblock"/>
<xsl:text disable-output-escaping="yes">
%&gt;</xsl:text>
</xsl:template>

<xsl:template match="declarationsblock">
<xsl:for-each select="dataroot">
<xsl:for-each select="declarations">
<xsl:sort select="SortID" data-type="number" order="ascending" />
<xsl:text>dim </xsl:text><xsl:value-of select="What2Setup" /><xsl:if test="string-length(Specials) != 0"><xsl:text> </xsl:text><xsl:value-of select="Specials"/></xsl:if><xsl:text>
</xsl:text>
</xsl:for-each>
</xsl:for-each>
</xsl:template>

<xsl:template match="browseridsblock">
<xsl:text>
Function GetLanguageIDOfBrowserSetting(SettingValue)

	Dim Result 
	Select Case LCase(SettingValue)
</xsl:text>
<xsl:for-each select="dataroot">
<xsl:for-each select="BrowserIDs">
<xsl:sort select="PropertyValue_Text" data-type="text" order="ascending" />
<xsl:text>		Case "</xsl:text><xsl:value-of select="PropertyValue_Text" /><xsl:text>": Result = </xsl:text><xsl:value-of select="ID"/><xsl:text>
</xsl:text>
</xsl:for-each>
</xsl:for-each>
<xsl:text>	End Select

	GetLanguageIDOfBrowserSetting = Result

End Function
</xsl:text>
</xsl:template>

<xsl:template match="markets2languagesblock">
<xsl:text>
Function GetAlternativelySupportedLanguageID (marketID)
	Dim Result 

            Select Case marketID
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:sort select="languageid" data-type="number" order="descending" />
<xsl:text>                Case </xsl:text>
<xsl:for-each select="markets">
<xsl:sort select="ID" data-type="number" order="ascending" />
<xsl:for-each select="ID">
<xsl:if test="position()!=1"><xsl:text>, </xsl:text></xsl:if>
<xsl:value-of select="." />
</xsl:for-each>
</xsl:for-each>
<xsl:text>
                    Result = </xsl:text><xsl:value-of select="languageid" /><xsl:text>
</xsl:text>
</xsl:for-each>
<xsl:text>                Case Else
                    Result = marketID 'The language ID is the market ID
            End Select

	GetAlternativelySupportedLanguageID = Result

End Function
</xsl:text>
</xsl:template>

<xsl:template match="languagesblock">
<xsl:text>
Private CurrentlyLoadedLanguagesStrings
Sub LoadLanguageStrings (IDLanguage)

	If CurrentlyLoadedLanguagesStrings &lt;&gt; 0 And CurrentlyLoadedLanguagesStrings = IDLanguage Then
		'Language data is already loaded
		Exit Sub
	End If

	'Try to get a supported, alternative LanguageID if IDLanguage isn't supported directly
	Dim MyIDLanguage
	If IsSupportedLanguage (IDLanguage) Then
		MyIDLanguage = IDLanguage
	Else
		MyIDLanguage = GetAlternativelySupportedLanguageID (IDLanguage)
	End If

	Select case MyIDLanguage
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:sort select="LangID" data-type="number" order="descending" />
<xsl:if test="languageid!=1"><xsl:text>		Case </xsl:text><xsl:value-of select="languageid" /><xsl:text>:
</xsl:text></xsl:if>
<xsl:if test="languageid=1"><xsl:text>		Case Else:
</xsl:text></xsl:if>
<xsl:apply-templates select="languagesetups"/>
</xsl:for-each>
<xsl:text>
	End Select
	
	LoadCustomizedLanguageStrings IDLanguage

	CurrentlyLoadedLanguagesStrings = IDLanguage

End Sub 
</xsl:text>
</xsl:template>

<xsl:template match="languagesetups">
<xsl:for-each select="languagesetup">
<xsl:sort select="SortID" data-type="number" order="ascending" />
<xsl:text>				</xsl:text><xsl:value-of select="What2Setup" /><xsl:text> = </xsl:text><xsl:choose><xsl:when test="string-length(Specials) != 0"><xsl:value-of select="Specials"/></xsl:when><xsl:otherwise><xsl:value-of select="Value2Setup" /></xsl:otherwise></xsl:choose><xsl:text>
</xsl:text>
</xsl:for-each>
</xsl:template>

<xsl:template match="datetimeblock">
<xsl:text>
Function GetCurLongDate(MyLanguage)
Dim LongDate 
Dim WeekdayDescr
Dim MonthDescr

	Select case MyLanguage
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:sort select="LangID" data-type="number" order="descending" />
<xsl:if test="languageid!=1"><xsl:text>		Case </xsl:text><xsl:value-of select="languageid" /><xsl:text>:
</xsl:text></xsl:if>
<xsl:if test="languageid=1"><xsl:text>		Case Else:
</xsl:text></xsl:if>
<xsl:apply-templates select="languagesetups" mode="datetimeblock"/>
</xsl:for-each>
<xsl:text>	End Select
	GetCurLongDate = LongDate

	LoadCustomizedLanguageStrings IDLanguage

End Function
</xsl:text>
</xsl:template>

<xsl:template match="languagesetups" mode="datetimeblock">
<xsl:for-each select="languagesetup">
<xsl:sort select="SortID" data-type="number" order="ascending" />
<xsl:choose><xsl:when test="string-length(Specials) != 0"><xsl:value-of select="Specials"/></xsl:when><xsl:otherwise><xsl:value-of select="Value2Setup" /></xsl:otherwise></xsl:choose><xsl:text>
</xsl:text>
</xsl:for-each>
</xsl:template>

<xsl:template match="languagesblock" mode="supportedlanguagesblock">
<xsl:text>
        Function IsSupportedLanguage (ByVal LanguageID)

            'LanguageID = Me.GetAlternativelySupportedLanguageID(LanguageID)

            Select Case LanguageID
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:text>                Case </xsl:text><xsl:value-of select="languageid" /><xsl:text>
                    IsSupportedLanguage = True
</xsl:text>
</xsl:for-each>
<xsl:text>                Case Else
                    IsSupportedLanguage = False
            End Select
        End Function
</xsl:text>
</xsl:template>

</xsl:stylesheet>
