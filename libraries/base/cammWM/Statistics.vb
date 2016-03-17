'Copyright 2004-2005,2015,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Public Class Statistics
        Private _WebManager As WMSystem
        Sub New(ByVal WebManager As WMSystem)
            _WebManager = WebManager
        End Sub

        ''' <param name="SessionTimeout">A value in minutes how long a user session is regarded as active when no logout and no further action have been performed</param>
        Public Function NumberOfUsersCurrentlyOnline(Optional ByVal SessionTimeout As Integer = -30) As Integer
            Dim Sql As String = "select count (id_user) as NumberOfUsersCurrentlyOnline" & vbNewLine & _
                "from system_usersessions" & vbNewLine & _
                "where system_usersessions.id_session in " & vbNewLine & _
                "	(" & vbNewLine & _
                "    	select sessionid" & vbNewLine & _
                "    	from dbo.System_WebAreasAuthorizedForSession" & vbNewLine & _
                "    	where lastsessionstaterefresh > dateadd(minute, -30, getdate())"
            Dim MyCmd As New SqlCommand(Sql, New SqlConnection(_WebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@SessionTimeout", SqlDbType.Int).Value = SessionTimeout
            Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
        End Function
    End Class

End Namespace