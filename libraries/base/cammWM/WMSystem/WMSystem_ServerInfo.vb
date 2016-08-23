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
        '''     Server information
        ''' </summary>
        Public Class ServerInformation
            Implements IServerInformation

            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _IP_Or_HostHeader As String
            Dim _Description As String
            Dim _URL_Protocol As String
            Dim _URL_DomainName As String
            Dim _URL_Port As String
            Dim _Enabled As Boolean
            Dim _ParentServerGroupID As Integer
            Dim _ParentServerGroup As ServerGroupInformation
            Dim _ServerSessionTimeout As Integer
            Dim _ServerUserlockingsTimeout As Integer

            Friend Sub New(ByVal ServerID As Integer, ByVal IP_Or_HostHeader As String, ByVal Description As String, ByVal URL_Protocol As String, ByVal URL_DomainName As String, ByVal URL_Port As String, ByVal Enabled As Boolean, ByVal ParentServerGroupID As Integer, ByRef WebManager As WMSystem, Optional ByVal ServerSessionTimeout As Integer = 15, Optional ByVal ServerUserlockingsTimeout As Integer = 3)
                _WebManager = WebManager
                _ID = ServerID
                _IP_Or_HostHeader = IP_Or_HostHeader
                _Description = Description
                _ParentServerGroupID = ParentServerGroupID
                _URL_Protocol = URL_Protocol
                _URL_DomainName = URL_DomainName
                _URL_Port = URL_Port
                _Enabled = Enabled
                _ServerSessionTimeout = ServerSessionTimeout
                _ServerUserlockingsTimeout = ServerUserlockingsTimeout
            End Sub
            Public Sub New(ByVal ServerID As Integer, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                LoadServerInfoFromDatabase(ServerID)
            End Sub
            Public Sub New(ByVal ServerIP As String, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                Dim ServerID As Integer = _WebManager.System_GetServerID(ServerIP)
                LoadServerInfoFromDatabase(ServerID)
            End Sub

            ''' <summary>
            '''     Load server information from database
            ''' </summary>
            ''' <param name="ServerID">A server ID</param>
            Private Sub LoadServerInfoFromDatabase(ByVal ServerID As Integer)
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("select * from system_servers where id = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServerID
                Dim MyReader As SqlDataReader = Nothing
                Try
                    MyConn.Open()
                    MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    If MyReader.Read Then
                        _ID = CType(MyReader("ID"), Integer)
                        _IP_Or_HostHeader = Utils.Nz(MyReader("IP"), CType(Nothing, String))
                        _Description = Utils.Nz(MyReader("ServerDescription"), CType(Nothing, String))
                        _ParentServerGroupID = Utils.Nz(MyReader("ServerGroup"), 0)
                        _URL_Protocol = Utils.Nz(MyReader("ServerProtocol"), CType(Nothing, String))
                        _URL_DomainName = Utils.Nz(MyReader("ServerName"), _IP_Or_HostHeader)
                        _URL_Port = Utils.Nz(MyReader("ServerPort"), CType(Nothing, String))
                        _Enabled = CType(MyReader("Enabled"), Boolean)
                        _ServerSessionTimeout = CType(MyReader("WebSessionTimeout"), Integer)
                        _ServerUserlockingsTimeout = CType(MyReader("LockTimeout"), Integer)
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
            '''     The ID value of this server
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property

            ''' <summary>
            '''     The server identification string
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     Typically, this is either an IP address or a host header name. This value can hold any ID, you only have to ensure that the server tries to login with that server identification string again. This can be set up in the web.config file or in /sysdata/config.*
            ''' </remarks>
            Public Property IPAddressOrHostHeader() As String
                Get
                    Return _IP_Or_HostHeader
                End Get
                Set(ByVal Value As String)
                    _IP_Or_HostHeader = Value
                End Set
            End Property

            ''' <summary>
            '''     The protocol name for the server, http or https
            ''' </summary>
            ''' <value></value>
            Public Property URL_Protocol() As String
                Get
                    Return _URL_Protocol
                End Get
                Set(ByVal Value As String)
                    _URL_Protocol = Value
                End Set
            End Property

            ''' <summary>
            '''     The domain name this server is available at
            ''' </summary>
            ''' <value></value>
            Public Property URL_DomainName() As String
                Get
                    Return _URL_DomainName
                End Get
                Set(ByVal Value As String)
                    _URL_DomainName = Value
                End Set
            End Property

            ''' <summary>
            '''     An optional port information if it's not the default port
            ''' </summary>
            ''' <value></value>
            Public Property URL_Port() As String
                Get
                    Return _URL_Port
                End Get
                Set(ByVal Value As String)
                    _URL_Port = Value
                End Set
            End Property

            ''' <summary>
            '''     The server URL without trailing slash, e. g. http://www.yourcompany:8080
            ''' </summary>
            Public Function ServerURL() As String
                Dim Field_ServerAddress As String
                Field_ServerAddress = _URL_Protocol & "://" & _URL_DomainName
                If _URL_Port <> Nothing AndAlso Not ((_URL_Port = "80" AndAlso _URL_Protocol.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "http") OrElse (_URL_Port = "443" AndAlso _URL_Protocol.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "https")) Then
                    Field_ServerAddress = Field_ServerAddress & ":" & _URL_Port
                End If
                Return Field_ServerAddress
            End Function

            ''' <summary>
            '''     Is this server activated?
            ''' </summary>
            ''' <value></value>
            Public Property Enabled() As Boolean
                Get
                    Return _Enabled
                End Get
                Set(ByVal Value As Boolean)
                    _Enabled = Value
                End Set
            End Property

            ''' <summary>
            '''     An optional description for this server
            ''' </summary>
            ''' <value></value>
            Public Property Description() As String
                Get
                    Return _Description
                End Get
                Set(ByVal Value As String)
                    _Description = Value
                End Set
            End Property

            Public Property ParentServerGroupID() As Integer
                Get
                    Return _ParentServerGroupID
                End Get
                Set(ByVal Value As Integer)
                    _ParentServerGroupID = Value
                    _ParentServerGroup = Nothing 'leads to reload
                End Set
            End Property

            ''' <summary>
            '''     The parent server group where this server is assigned to
            ''' </summary>
            ''' <value></value>
            Public Property ParentServerGroup() As ServerGroupInformation
                Get
                    If _ParentServerGroup Is Nothing Then
                        _ParentServerGroup = New ServerGroupInformation(_ParentServerGroupID, _WebManager)
                    End If
                    Return _ParentServerGroup
                End Get
                Set(ByVal Value As ServerGroupInformation)
                    _ParentServerGroup = Value
                    _ParentServerGroupID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     A session timeout value
            ''' </summary>
            ''' <value></value>
            Public Property ServerSessionTimeout() As Integer
                Get
                    Return _ServerSessionTimeout
                End Get
                Set(ByVal Value As Integer)
                    _ServerSessionTimeout = Value
                End Set
            End Property

            ''' <summary>
            '''     A timeout value how fast temporary locked users can logon again
            ''' </summary>
            ''' <value></value>
            Public Property ServerUserlockingsTimeout() As Integer
                Get
                    Return _ServerUserlockingsTimeout
                End Get
                Set(ByVal Value As Integer)
                    _ServerUserlockingsTimeout = Value
                End Set
            End Property
        End Class

    End Class

End Namespace