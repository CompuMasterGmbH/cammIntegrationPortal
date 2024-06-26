<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" />

<xsl:template match="root">
<xsl:text disable-output-escaping="yes">&lt;script language="vb" runat="server"&gt;
	
	
Class CustomSettingsAndData
	Inherits CompuMaster.camm.WebManager.WMSettingsAndData
</xsl:text>
<xsl:apply-templates select="languagesblock"/>
<xsl:apply-templates select="markets2languagesblock"/>
<xsl:text disable-output-escaping="yes">
End Class

&lt;System.Obsolete ("Subject of removal in one of the next versions")&gt; Public Sub LoadCustomizedLanguageStrings(LanguageID As Integer)
	'Do not use any more!
	'This method is subject of remval in one of the next versions
End Sub

&lt;/script&gt;</xsl:text>
</xsl:template>

<xsl:template match="markets2languagesblock">
<xsl:text>
	Public Function CustomLookupOfLanguageID (marketID as Integer) As Integer

		Return marketID 'do not process following lines for now

		'Some market languages are equal or similar to other languages.
		'Examples: 
		'    English (US) can use same vocabulary as English (UK), both use English langage
		'    German (Austria) can use same vocabulary as German (Germany), both use German language 

		'Returns that language ID which is related to the given market (maybe the market ID itself)
		Dim Result as Integer

		Select Case marketID
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:sort select="languageid" data-type="number" order="descending" />
<xsl:text>			Case </xsl:text>
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
<xsl:text>			Case Else
				Result = marketID 'The language ID is the market ID
		End Select

		Return Result

	End Function
</xsl:text>
</xsl:template>

<xsl:template match="languagesblock">
<xsl:text>
	Public Overrides Sub LoadLanguageStrings(ByVal marketID As Integer)

		'Load the defaults, first
		MyBase.LoadLanguageStrings (MarketID)

		EXIT SUB 'do not process following lines for now

		'Try to get a supported, alternative LanguageID if marketID isn't supported directly
		Dim languageID As Integer = CustomLookupOfLanguageID (marketID)

		'Override here with own definitions; if required, add your own public variables, too
		'Hint: override only those variables which you really want to override, this will allow you to get the latest text versions from the most up-to-date camm Web-Manager version
		Select case languageID
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:sort select="LangID" data-type="number" order="descending" />
<xsl:if test="languageid!=1"><xsl:text>			Case </xsl:text><xsl:value-of select="languageid" /><xsl:text>:
</xsl:text></xsl:if>
<xsl:if test="languageid=1"><xsl:text>			Case Else:
</xsl:text></xsl:if>
<xsl:apply-templates select="languagesetups"/>
</xsl:for-each>
<xsl:text>
		End Select

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

</xsl:stylesheet>
