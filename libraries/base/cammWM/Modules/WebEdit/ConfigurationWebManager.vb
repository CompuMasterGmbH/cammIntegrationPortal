'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden für Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Explicit On
Option Strict On

Namespace CompuMaster.camm.WebManager.Modules.WebEdit.Controls

    Friend Class ConfigurationWebManager

        Public Class CwmConfigAccessor
            Inherits CompuMaster.camm.WebManager.Modules.WebEdit.Controls.SmartWcmsEditorBase

            Friend Sub New()
            End Sub

        End Class

        ''' <summary>
        ''' Every WebEditor content is related to a server; this property overrides the server ID value where to read from/save to
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' 0 = currently used server; other values = forced server ID
        ''' </remarks>
        Public Shared ReadOnly Property WebEditorContentOfServerID() As Integer
            Get
                'Dim WMConfigAccessor As New CwmConfigAccessor
                'Return WMConfigAccessor.Configuration.ContentOfServerID

                ''Maybe a better way for future - but still inactive due to backwards-compatibility with CWM v4.10.192
                ''Return LoadIntegerSetting("WebManager.WebEditor.ContentOfServerID", 0, False)
                Return CompuMaster.camm.WebManager.Configuration.LoadIntegerSetting("WebManager.WebEditor.ContentOfServerID", 0, False)
            End Get
        End Property

        '#Region "Load configuration setting helper methods"

        Friend Shared ReadOnly Property WebManagerSettings() As System.Collections.Specialized.NameValueCollection
            Get
#If VS2015OrHigher = True Then
#Disable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
                Return System.Configuration.ConfigurationSettings.AppSettings
#If VS2015OrHigher = True Then
#Enable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
            End Get
        End Property

        '        ''' <summary>
        '        '''     Load an integer value from the configuration file
        '        ''' </summary>
        '        ''' <param name="appSettingName">The name of the appSetting item</param>
        '        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        '        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        '        ''' <returns></returns>
        '        Private Shared Function LoadIntegerSetting(ByVal appSettingName As String, ByVal defaultValue As Integer, ByVal suppressExceptions As Boolean) As Integer
        '            Dim Result As Integer = defaultValue
        '            Try
        '                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
        '                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
        '                If value = Nothing Then
        '                    Result = defaultValue
        '                Else
        '                    Result = Integer.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
        '                End If
        '            Catch ex As Exception
        '                If suppressExceptions = False Then
        '                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose a valid integer number, please.", ex)
        '                End If
        '            End Try
        '            Return Result
        '        End Function
        '#End Region

    End Class

End Namespace