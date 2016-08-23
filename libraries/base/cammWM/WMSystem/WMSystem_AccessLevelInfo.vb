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

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Partial Public Class WMSystem

        ''' <summary>
        '''     Access level information
        ''' </summary>
        ''' <remarks>
        '''     Access levels are user roles defining the availability of the existant server groups for the user
        ''' </remarks>
        Public Class AccessLevelInformation
            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _Title As String
            Dim _Remarks As String
            Dim _ServerGroups As ServerGroupInformation()
            Dim _Users As UserInformation()
            Public Sub New(ByVal ID As Integer, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                Dim MyConn As New SqlConnection(WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("Select * From system_accesslevels Where ID = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
                Dim MyReader As SqlDataReader = Nothing
                Try
                    MyConn.Open()
                    MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    If MyReader.Read Then
                        _ID = CType(MyReader("ID"), Integer)
                        _Title = Utils.Nz(MyReader("Title"), CType(Nothing, String))
                        _Remarks = Utils.Nz(MyReader("Remarks"), CType(Nothing, String))
                    End If
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyConn Is Nothing Then
                        If MyConn.State <> ConnectionState.Closed Then
                            MyConn.Close()
                        End If
                        MyConn.Dispose()
                    End If
                End Try
            End Sub
            ''' <summary>
            '''     The ID value for this access level role
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            ''' <summary>
            '''     The title for this access level role
            ''' </summary>
            ''' <value></value>
            Public Property Title() As String
                Get
                    Return _Title
                End Get
                Set(ByVal Value As String)
                    _Title = Value
                End Set
            End Property
            ''' <summary>
            '''     Some optional remarks on this role
            ''' </summary>
            ''' <value></value>
            Public Property Remarks() As String
                Get
                    Return _Remarks
                End Get
                Set(ByVal Value As String)
                    _Remarks = Value
                End Set
            End Property
            ''' <summary>
            '''     A list of server groups which are accessable by this role
            ''' </summary>
            ''' <value></value>
            Public Property ServerGroups() As ServerGroupInformation()
                Get
                    If _ServerGroups Is Nothing Then
                        _ServerGroups = _WebManager.System_GetServerGroupsInfo(_ID)
                    End If
                    Return _ServerGroups
                End Get
                Set(ByVal Value As ServerGroupInformation())
                    _ServerGroups = Value
                End Set
            End Property

            ''' <summary>
            '''     A list of users which are assigned to this role
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property UserIDs() As Long()
                Get
                    Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                    Dim MyCmd As New SqlCommand("Select benutzer.id From benutzer Where benutzer.AccountAccessability = @ID Order By [1stPreferredLanguage]", MyConn)
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                    Dim MyUsers As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Dim Result As Long()
                    ReDim Result(MyUsers.Count - 1)
                    For MyCounter As Integer = 0 To MyUsers.Count - 1
                        Result(MyCounter) = CType(MyUsers(MyCounter), Long)
                    Next
                    Return Result
                End Get
            End Property

            ''' <summary>
            '''     A list of users which are assigned to this role
            ''' </summary>
            ''' <value></value>
            Public Property Users() As UserInformation()
                Get
                    If _Users Is Nothing Then
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("Select * From benutzer Where benutzer.AccountAccessability = @ID Order By [1stPreferredLanguage]", MyConn)
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim MyUsers As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "Users")
                        If MyUsers.Rows.Count > 0 Then
                            ReDim Preserve _Users(MyUsers.Rows.Count - 1)
                            Dim MyCounter As Integer = 0
                            For Each MyDataRow As DataRow In MyUsers.Rows
                                _Users(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.UserInformation(MyDataRow, _WebManager)
                                If _Users(MyCounter).Gender = Sex.Undefined AndAlso (_Users(MyCounter).FirstName = Nothing OrElse _Users(MyCounter).LastName = Nothing) Then
                                    'Regard it as a group of persons without a specific name
                                    _Users(MyCounter).Gender = Sex.MissingNameOrGroupOfPersons
                                End If
                                MyCounter += 1
                            Next
                        Else
                            _Users = Nothing
                        End If
                    End If
                    Return _Users
                End Get
                Set(ByVal Value As UserInformation())
                    _Users = Value
                End Set
            End Property
        End Class

    End Class

End Namespace