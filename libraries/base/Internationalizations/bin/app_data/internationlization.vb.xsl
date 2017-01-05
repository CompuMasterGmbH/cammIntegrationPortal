<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="text" />

<xsl:template match="root">
<xsl:text disable-output-escaping="yes">Option Explicit On 
Option Strict On

NameSpace CompuMaster.camm.WebManager
Public Class WMSettingsAndData

        Private Function GetLocalizedString(ByVal Name As String) As String
            Dim Result As String
            Dim ResMngr As Resources.ResourceManager

            Dim MyCachedLanguageData As System.Collections.Specialized.NameValueCollection
            If Not System.Web.HttpContext.Current Is Nothing AndAlso Not System.Web.HttpContext.Current.Application("WebManager.LocalizationInfos") Is Nothing Then
                MyCachedLanguageData = CType(System.Web.HttpContext.Current.Application("WebManager.LocalizationInfos"), System.Collections.Specialized.NameValueCollection)
                Result = MyCachedLanguageData(Name)
            Else
                ResMngr = New Resources.ResourceManager("cammWM.StringRes", System.Reflection.Assembly.GetExecutingAssembly)
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(Name)
                ResMngr.ReleaseAllResources()
            End If

            Return (Result)

        End Function

#Region "WebManager Essentials"
        Public Event LanguageDataLoaded(ByVal LanguageID As Integer)

        Public User_Auth_Config_Paths_UserAuthSystem As String = "/"
        Public User_Auth_Config_Paths_Login As String = "/sysdata/login/"
        Public User_Auth_Config_Paths_Administration As String = "/sysdata/admin/" 
        Public User_Auth_Config_Paths_StandardIncludes As String = "/system/"
        Public User_Auth_Config_Paths_SystemData As String = "/sysdata/"

        Public User_Auth_Config_Files_Administration_DefaultPageInAdminEMails As String = "memberships.aspx"
        Public User_Auth_Validation_NoRefererURL as string
        Public User_Auth_Validation_LogonScriptURL as string
        Public User_Auth_Validation_AfterLogoutURL as string
        Public User_Auth_Validation_AccessErrorScriptURL as string
        Public User_Auth_Validation_CreateUserAccountInternalURL as string
        Public User_Auth_Validation_TerminateOldSessionScriptURL as string
        Public User_Auth_Validation_CheckLoginURL as string
        Public User_Auth_Config_UserAuthMasterServer as string
        Public User_Auth_Config_CurServerURL as string
        Public OfficialServerGroup_URL As String
        Public OfficialServerGroup_AdminURL As String 'default value, for navigation URLs and for redirections
        Public OfficialServerGroup_AdminURL_SecurityAdminNotifications As String 'for e-mails for security administrators
        Public OfficialServerGroup_Title As String
        Public OfficialServerGroup_Company_FormerTitle As String
#End Region

</xsl:text>
<xsl:apply-templates select="declarationsblock"/>
<xsl:apply-templates select="browseridsblock"/>
<xsl:apply-templates select="languagesblock"/>
<xsl:apply-templates select="markets2languagesblock"/>
<xsl:apply-templates select="datetimeblock"/>
<xsl:apply-templates select="languagesblock" mode="supportedlanguagesblock"/>
<xsl:text disable-output-escaping="yes">
End Class
End NameSpace</xsl:text>
</xsl:template>

<xsl:template match="declarationsblock">
<xsl:text>#Region "Declarations"
</xsl:text>
<xsl:for-each select="dataroot">
<xsl:for-each select="declarations">
<xsl:sort select="SortID" data-type="number" order="ascending" />
<xsl:if test="Obsolete = 'true'"><xsl:text>&lt;Obsolete ("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)&gt; </xsl:text></xsl:if>
<xsl:text>Public </xsl:text><xsl:value-of select="What2Setup" /><xsl:if test="string-length(Specials) != 0"><xsl:text> </xsl:text><xsl:value-of select="Specials"/></xsl:if><xsl:text>
</xsl:text>
</xsl:for-each>
</xsl:for-each>
<xsl:text>#End Region
</xsl:text>
</xsl:template>

<xsl:template match="browseridsblock">
<xsl:text>
Public Overridable Function GetLanguageIDOfCultureName(ByVal CultureName As String) As Integer
	Return GetLanguageIDOfBrowserSetting(CultureName)
End Function

Friend Overridable Function GetLanguageIDOfBrowserSetting(SettingValue as string) As Integer
	If SettingValue = Nothing Then
		Return Nothing
	End If

	Dim Result As Integer
	Select Case SettingValue.ToLower(System.Globalization.CultureInfo.InvariantCulture)
</xsl:text>
<xsl:for-each select="dataroot">
<xsl:for-each select="BrowserIDs">
<xsl:sort select="PropertyValue_Text" data-type="text" order="ascending" />
<xsl:text>		Case "</xsl:text><xsl:value-of select="PropertyValue_Text" /><xsl:text>"
			 Result = </xsl:text><xsl:value-of select="ID"/><xsl:text>
</xsl:text>
</xsl:for-each>
</xsl:for-each>
<xsl:text>	End Select

	Return Result

End Function
</xsl:text>
</xsl:template>

<xsl:template match="markets2languagesblock">
<xsl:text>
Public Overridable Function GetAlternativelySupportedLanguageID (marketID as Integer) As Integer
	Dim Result as Integer

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

	Return Result

End Function
</xsl:text>
</xsl:template>

<xsl:template match="languagesblock">
<xsl:text>
Private CurrentlyLoadedLanguagesStrings as Integer
Public Overridable Sub LoadLanguageStrings (IDLanguage As Integer)

	If CurrentlyLoadedLanguagesStrings &lt;&gt; Nothing AndAlso CurrentlyLoadedLanguagesStrings = IDLanguage Then
		'Language data is already loaded
		Exit Sub
	End If

	'Try to get a supported, alternative LanguageID if IDLanguage isn't supported directly
	Dim MyIDLanguage As Integer
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

	RaiseEvent LanguageDataLoaded (IDLanguage)

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
Public Overridable Function GetCurLongDate(MyLanguage As Integer) As String
Dim LongDate as String
Dim WeekdayDescr as String = Nothing
Dim MonthDescr as String = Nothing

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
        Public Overridable Function IsSupportedLanguage (ByVal LanguageID As Integer) As Boolean

            'Returns True if the language is INTERNALLY supported
            'The alternative languages don't play a role, here!

            Select Case LanguageID
</xsl:text>
<xsl:for-each select="languagedata">
<xsl:text>                Case </xsl:text><xsl:value-of select="languageid" /><xsl:text>
                    Return True
</xsl:text>
</xsl:for-each>
<xsl:text>                Case Else
                    Return False
            End Select
        End Function
</xsl:text>
</xsl:template>

</xsl:stylesheet>