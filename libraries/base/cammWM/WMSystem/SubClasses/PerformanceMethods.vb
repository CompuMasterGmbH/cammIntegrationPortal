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

    Public Class PerformanceMethods
        Private _WebManager As WMSystem
        Sub New(ByVal WebManager As WMSystem)
            _WebManager = WebManager
        End Sub

        ''' <summary>
        ''' Is a login name existing?
        ''' </summary>
        ''' <param name="loginName">A login name</param>
        ''' <returns>True if it exists, otherwise false</returns>
        Public Function IsUserExisting(ByVal loginName As String) As Boolean
            Return IsUserExisting(_WebManager, loginName)
        End Function

        Friend Shared Function IsUserExisting(ByVal webManager As WMSystem, ByVal loginName As String) As Boolean
            If CType(webManager.System_GetUserID(loginName, True), Long) = -1& Then
                'User doesn't exist
                Return False
            Else
                'User exists
                Return True
            End If
        End Function

        ''' <summary>
        ''' Query a list of existing user IDs
        ''' </summary>
        Function ActiveUsers() As Long()
            Return WebManager.DataLayer.Current.ActiveUsers(_WebManager)
        End Function

        ''' <summary>
        ''' Query a list of user IDs from existing plus deleted users
        ''' </summary>
        ''' <returns>A hashtable containing the user ID as key field (Int64) and the status &quot;Deleted&quot; as a boolean value in the hashtable's value field (true indicates a deleted user)</returns>
        Function ActiveAndDeletedUsers() As Hashtable
            Return CompuMaster.camm.WebManager.DataLayer.Current.ActiveAndDeletedUsers(_WebManager)
        End Function

    End Class

End Namespace