'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     The common interface of the camm Web-Manager
    ''' </summary>
    Public Interface IWebManager

        ''' <summary>
        ''' Existing debug levels in camm Web-Manager
        ''' </summary>
        ''' <remarks></remarks>
        Enum DebugLevels As Byte
            ''' <summary>
            '''     No debugging
            ''' </summary>
            ''' <remarks>
            '''     This feature is set to disabled.
            ''' </remarks>
            NoDebug = 0 'kein Debug
            ''' <summary>
            '''     Access error warnings only
            ''' </summary>
            ''' <remarks>
            '''     Warning messages will be sent to the developer contact configured in your config files.
            ''' </remarks>
            Low_WarningMessagesOnAccessError = 1 'Warn-Messages über Access Errors an Developer Contact
            ''' <summary>
            '''     More access error warnings
            ''' </summary>
            ''' <remarks>
            '''     Additional warning messages will be sent to the developer contact configured in your config files.
            ''' </remarks>
            Low_WarningMessagesOnAccessError_AdditionalDetails = 2 'Protokollierung von zusätzlichen Informationen
            ''' <summary>
            '''     Actively collect data for debugging
            ''' </summary>
            ''' <remarks>
            '''     Even more additional warning messages will be sent to the developer contact configured in your config files.
            ''' </remarks>
            Medium_LoggingOfDebugInformation = 3 'Protokollierung von zusätzlichen Debug-Informationen
            ''' <summary>
            '''     Send all e-mails to developer account - never use in production environments!
            ''' </summary>
            ''' <remarks>
            '''     <para>ATTENTION: Never use this mode in production environments!</para>
            '''     <para>All e-mails will be redirected to the developer; no e-mail will be sent to the origin receipient. This is optimal for project development and testing environments.</para>
            '''		<para>The messages will be sent to the developer contact configured in your config files.</para>
            ''' </remarks>
            Medium_LoggingAndRedirectAllEMailsToDeveloper = 4 'e-mail-Umleitung an Developer Contact
            ''' <summary>
            '''     Maximum debug level - never use in production environments!
            ''' </summary>
            ''' <remarks>
            '''     <para>ATTENTION: Never use this mode in production environments!</para>
            '''     <para>All e-mails will be redirected to the developer; no e-mail will be sent to the origin receipient. This is optimal for setting up the project.</para>
            '''		<para>The messages will be sent to the developer contact configured in your config files.</para>
            '''     <para>Automatic redirects have to be manually executed. This is ideally for solving redirection bugs or when loosing session paths in cookieless scenarios.</para>
            ''' </remarks>
            Max_RedirectAllEMailsToDeveloper = 5 'e-mail-Umleitung an Developer Contact + manual page redirections
        End Enum

        ''' <summary>
        ''' Special users/groups pre-defined by camm Web-Manager
        ''' </summary>
        ''' <remarks>
        '''     Values &gt; 0 are really existing users;
        '''     Values &lt; 0 are pseudonyms
        '''     Values = 0 are invalid
        ''' </remarks>
        Enum SpecialUsers As Long
            AnonymousUser = -1
            UpdateProcessor = -43
            [Public] = -2
            RegisteredUser = -2
            Code = -33
            InvalidUser = 0
        End Enum

        Property ConnectionString() As String
        ReadOnly Property UI() As CompuMaster.camm.WebManager.UserInterface
        ReadOnly Property PerformanceMethods() As CompuMaster.camm.WebManager.PerformanceMethods
#If ImplementedSubClassesWithIWebManagerInterface Then
        ReadOnly Property MessagingEMails() As Messaging.EMails
        ReadOnly Property IsSupported() As WMCapabilities
#End If

        ''' <summary>
        ''' camm Web-Manager assembly/library version
        ''' </summary>
        Function VersionAssembly() As Version
        ''' <summary>
        ''' camm Web-Manager database version
        ''' </summary>
        Function VersionDatabase() As Version
        ''' <summary>
        ''' camm Web-Manager database version
        ''' </summary>
        ''' <param name="allowCaching">True to allow reading from cache</param>
        Function VersionDatabase(allowCaching As Boolean) As Version

    End Interface

End Namespace