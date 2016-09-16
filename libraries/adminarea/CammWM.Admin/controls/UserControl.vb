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

Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager.Controls.Administration

    Public Class UserControl
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        ''' <summary>
        ''' Safely lookup the name of a group
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Protected Function SafeLookupGroupName(id As Integer) As String
            Try
                If id = -1 Then
                    Return "{Anonymous}"
                Else
                    Return New CompuMaster.camm.WebManager.WMSystem.GroupInformation(id, CType(Me.cammWebManager, WMSystem)).Name
                End If
            Catch ex As Exception
                Return "[?:" & id & "] (" & ex.Message & ")"
            End Try
        End Function

        ''' <summary>
        ''' Safely lookup the name of a user
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Protected Function SafeLookupUserFullName(id As Long) As String
            Return Me.SafeLookupUserFullName(id, False)
        End Function

        ''' <summary>
        ''' Safely lookup the name of a user
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="additionallyWithLoginName">True to enable additional output of login name, e.g. &quot;User Full Name (Login name)&quot;</param>
        ''' <returns></returns>
        Protected Function SafeLookupUserFullName(id As Long, additionallyWithLoginName As Boolean) As String
            Return CompuMaster.camm.WebManager.Administration.Utils.FormatUserNameSafely(CType(Me.cammWebManager, WMSystem), id, additionallyWithLoginName)
        End Function

    End Class

End Namespace
