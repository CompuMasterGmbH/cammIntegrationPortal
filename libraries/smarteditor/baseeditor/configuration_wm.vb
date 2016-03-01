'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Explicit On
Option Strict On

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     Configuration settings read from web.config/app.config
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Friend Class Configuration

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The connection string to connect to the camm Web-Manager database
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property ConnectionString() As String
            Get
                Dim Result As String = Nothing
                Try
                    Result = WebManagerSettings.Item("WebManager.ConnectionString")
                Catch
                End Try
#If NetFramework <> "1_1" Then
                If Result = Nothing Then
                    Try
                        Return System.Configuration.ConfigurationManager.ConnectionStrings("WebManagerDatabase").ConnectionString
                    Catch
                    End Try
                End If
                If Result = Nothing Then
                    Try
                        Return System.Configuration.ConfigurationManager.ConnectionStrings("WebManager").ConnectionString
                    Catch
                    End Try
                End If
#End If
                If Result = Nothing Then Result = AdditionalConfiguration("WebManager.ConnectionString")
                If Result = Nothing Then Result = AdditionalConfiguration("WebManagerDatabase")
                Return Result
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The SecurityObject which must be authorized for the current user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     You can configure this value in your web.config to ensure that e. g. a whole directory structure uses this value and is protected by this way
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property SecurityObject() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.SecurityObject")
                Catch
                    Return Nothing
                End Try
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The language which shall be used when it can't be detected by any other way
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	12.07.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property GlobalizationDefaultMarket() As Integer
            Get
                'Return the forced language if there is one
                If GlobalizationForcedMarket <> Nothing Then
                    Return GlobalizationForcedMarket
                End If
                'Retrieve a configured default value
                'Otherwise it's english (= 1)
                Return Configuration.LoadIntegerSetting("WebManager.Globalization.DefaultMarket", Configuration.LoadIntegerSetting("WebManager.Languages.Default", 1, True), True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A comma separated list of market IDs which are supported by the current web application and which are available for the users
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' Web applications are provided by independent developers and manufacturers. Often the application is provided for 1 or 2 markets only. In this case, many other activated markets and languages of the portal would be in danger to show up translated partially because of some text blocks translated and provided by the application logic, some other blocks handled by the portal itself and fully available in all activated languages (e.g. footer information like a link to the data protection rules). In this case, it is better to show up the application GUI in only the supported language of the application itself.
        ''' <example>
        ''' Your portal is setup to provide markets English, English (USA), English (Canada) and Spanish. Your application has been developed for markets English (USA) and English (Canada). So, the application can appear fully translated in USA and Canada. English users from other countries or Spanish users are not supported by the application. If a user comes to the application with e. g. English (without market information), the application won't show up in standard English, but it will appear in the application's <see cref="CompuMaster.camm.WebManager.Configuration.GlobalizationDefaultMarket">default market</see>.
        ''' </example>
        ''' <para>If not defined, the general list of activated markets will be used.</para>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	19.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property GlobalizationSupportedMarkets() As Integer()
            Get
                Try
                    'Return the forced language if there is one
                    If GlobalizationForcedMarket <> Nothing Then
                        Return New Integer() {GlobalizationForcedMarket}
                    End If
                    'Return the configured value
                    Dim Result As String
                    Result = Configuration.LoadStringSetting("WebManager.Globalization.SupportedMarkets", Nothing)
                    If Result = Nothing Then
                        Result = Configuration.LoadStringSetting("WebManager.ActiveMarkets", Nothing)
                    End If
                    Return Utils.SplitStringToInteger(Result, ","c)
                Catch
                    Return New Integer() {}
                End Try
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The language which shall be forced for the GUI
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	12.07.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property GlobalizationForcedMarket() As Integer
            Get
                Return Configuration.LoadIntegerSetting("WebManager.Globalization.ForcedMarket", Configuration.LoadIntegerSetting("WebManager.Languages.ForcedLanguage", Nothing, True), True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Every WebEditor content is related to a server; this property overrides the server ID value where to read from/save to
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' 0 = currently used server; other values = forced server ID
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	14.01.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared ReadOnly Property WebEditorContentOfServerID() As Integer
            Get
                Dim Result As Integer
                Try
                    Result = CType(WebManagerSettings("WebManager.WebEditor.ContentOfServerID"), Integer)
                Catch
                End Try
                Return Result
            End Get
        End Property

#Region "Load configuration setting helper methods"

        Friend Shared ReadOnly Property WebManagerSettings() As System.Collections.Specialized.NameValueCollection
            Get
                Return System.Configuration.ConfigurationSettings.AppSettings
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load a boolean value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	12.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function LoadBooleanSetting(ByVal appSettingName As String, ByVal defaultValue As Boolean, ByVal suppressExceptions As Boolean) As Boolean
            Dim Result As Boolean = defaultValue
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If Not value Is Nothing Then
                    value = value.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                Else
                    value = ""
                End If
                Select Case value
                    Case "0", "off", "false"
                        Result = False
                    Case "1", "-1", "on", "true"
                        Result = True
                    Case ""
                        'keep default
                    Case Else
                        Result = Boolean.Parse(value)
                End Select
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose ""true"" or ""false"", please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load an integer value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	12.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function LoadIntegerSetting(ByVal appSettingName As String, ByVal defaultValue As Integer, ByVal suppressExceptions As Boolean) As Integer
            Dim Result As Integer = defaultValue
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If value = Nothing Then
                    Result = defaultValue
                Else
                    Result = Integer.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
                End If
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose a valid integer number, please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load an Int64 value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' -----------------------------------------------------------------------------
        Private Shared Function LoadLongSetting(ByVal appSettingName As String, ByVal defaultValue As Long, ByVal suppressExceptions As Boolean) As Long
            Dim Result As Long = defaultValue
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If value = Nothing Then
                    Result = defaultValue
                Else
                    Result = Integer.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
                End If
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose a valid integer number, please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load a string value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	12.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function LoadStringSetting(ByVal appSettingName As String, ByVal defaultValue As String) As String
            Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
            If value = Nothing Then value = AdditionalConfiguration(appSettingName)
            If value = Nothing Then
                Return defaultValue
            Else
                Return value
            End If
        End Function
#End Region

    End Class

End Namespace