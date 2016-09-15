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
        '''     Server group information
        ''' </summary>
        Public Class ServerGroupInformation
            Implements IServerGroupInformation

            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _Title As String
            Dim _NavTitle As String
            Dim _MasterServer As ServerInformation
            Dim _MasterServerID As Integer
            Dim _AdminServer As ServerInformation
            Dim _AdminServerID As Integer
            Dim _AccessLevelDefaultID As Integer
            Dim _AccessLevelDefault As AccessLevelInformation
            Dim _SecurityContactName As String
            Dim _SecurityContactAddress As String
            Dim _DevelopmentContactName As String
            Dim _DevelopmentContactAddress As String
            Dim _ContentManagementContactName As String
            Dim _ContentManagementContactAddress As String
            Dim _UnspecifiedContactName As String
            Dim _UnspecifiedContactAddress As String
            Dim _OfficialCompanyWebSiteTitle As String
            Dim _OfficialCompanyWebSiteURL As String
            Dim _CompanyTitle As String
            Dim _CompanyFormerTitle As String
            Dim _AllowImpersonation As Boolean
            Dim _GroupAnonymousID As Integer
            Dim _GroupPublicID As Integer
            Dim _GroupAnonymous As GroupInformation
            Dim _GroupPublic As GroupInformation
            Dim _Servers As ServerInformation()
            Dim _CopyrightSinceYear As Integer
            Dim _ImageUrlBig As String
            Dim _ImageUrlSmall As String

            Friend Sub New(ByVal ServerGroupID As Integer, ByVal Title As String, ByVal NavTitle As String, ByVal OfficialCompanyWebSiteTitle As String, ByVal OfficialCompanyWebSiteURL As String, ByVal CompanyTitle As String, ByVal CompanyFormerTitle As String, ByVal AccessLevelDefaultID As Integer, ByVal MasterServerID As Integer, ByVal AdminServerID As Integer, ByVal GroupAnonymousID As Integer, ByVal GroupPublicID As Integer,
                SecurityContactName As String, SecurityContactAddress As String, DevelopmentContactName As String, DevelopmentContractAddress As String, ContentManagementContactName As String, ContentManagementContactAddress As String, UnspecifiedContactName As String, UnspecifiedContactAddress As String, AllowImpersonation As Boolean, copyrightSinceYear As Integer, imageUrlBig As String, imageUrlSmall As String, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                _ID = ServerGroupID
                _Title = Title
                _NavTitle = NavTitle
                _OfficialCompanyWebSiteTitle = OfficialCompanyWebSiteTitle
                _OfficialCompanyWebSiteURL = OfficialCompanyWebSiteURL
                _CompanyTitle = CompanyTitle
                _CompanyFormerTitle = CompanyFormerTitle
                _AccessLevelDefaultID = AccessLevelDefaultID
                _AdminServerID = AdminServerID
                _MasterServerID = MasterServerID
                _GroupAnonymousID = GroupAnonymousID
                _GroupPublicID = GroupPublicID
                _SecurityContactName = SecurityContactName
                _SecurityContactAddress = SecurityContactAddress
                _DevelopmentContactAddress = DevelopmentContractAddress
                _DevelopmentContactName = DevelopmentContactName
                _ContentManagementContactAddress = ContentManagementContactAddress
                _ContentManagementContactName = ContentManagementContactName
                _UnspecifiedContactName = UnspecifiedContactName
                _UnspecifiedContactAddress = UnspecifiedContactAddress
                _AllowImpersonation = AllowImpersonation
                _CopyrightSinceYear = copyrightSinceYear
                _ImageUrlBig = imageUrlBig
                _ImageUrlSmall = _ImageUrlSmall
            End Sub
            Public Sub New(ByVal ServerGroupID As Integer, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                Dim MyConn As New SqlConnection(WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("select * from system_servergroups where id = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServerGroupID
                Dim MyReader As SqlDataReader = Nothing
                Try
                    MyConn.Open()
                    MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    If MyReader.Read Then
                        _ID = CType(MyReader("ID"), Integer)
                        _Title = Utils.Nz(MyReader("ServerGroup"), CType(Nothing, String))
                        _NavTitle = Utils.Nz(MyReader("AreaNavTitle"), CType(Nothing, String))
                        _OfficialCompanyWebSiteTitle = Utils.Nz(MyReader("AreaCompanyWebSiteTitle"), CType(Nothing, String))
                        _OfficialCompanyWebSiteURL = Utils.Nz(MyReader("AreaCompanyWebSiteURL"), CType(Nothing, String))
                        _CompanyTitle = Utils.Nz(MyReader("AreaCompanyTitle"), CType(Nothing, String))
                        _CompanyFormerTitle = Utils.Nz(MyReader("AreaCompanyFormerTitle"), CType(Nothing, String))
                        _AccessLevelDefaultID = Utils.Nz(MyReader("AccessLevel_Default"), 0)
                        _AdminServerID = Utils.Nz(MyReader("UserAdminServer"), 0)
                        _MasterServerID = Utils.Nz(MyReader("MasterServer"), 0)
                        _GroupAnonymousID = Utils.Nz(MyReader("ID_Group_Anonymous"), 0)
                        _GroupPublicID = Utils.Nz(MyReader("ID_Group_Public"), 0)
                        _SecurityContactAddress = Utils.Nz(MyReader("AreaSecurityContactEMail"), CType(Nothing, String))
                        _SecurityContactName = Utils.Nz(MyReader("AreaSecurityContactTitle"), CType(Nothing, String))
                        _DevelopmentContactName = Utils.Nz(MyReader("AreaDevelopmentContactTitle"), CType(Nothing, String))
                        _DevelopmentContactAddress = Utils.Nz(MyReader("AreaDevelopmentContactEMail"), CType(Nothing, String))
                        _ContentManagementContactName = Utils.Nz(MyReader("AreaContentManagementContactTitle"), CType(Nothing, String))
                        _ContentManagementContactAddress = Utils.Nz(MyReader("AreaContentManagementContactEMail"), CType(Nothing, String))
                        _UnspecifiedContactName = Utils.Nz(MyReader("AreaUnspecifiedContactTitle"), CType(Nothing, String))
                        _UnspecifiedContactAddress = Utils.Nz(MyReader("AreaUnspecifiedContactEMail"), CType(Nothing, String))
                        _CopyrightSinceYear = CType(MyReader("AreaCopyRightSinceYear"), Integer)
                        _ImageUrlBig = Utils.Nz(MyReader("AreaImage"), "")
                        _ImageUrlSmall = Utils.Nz(MyReader("AreaButton"), "")
                        If Tools.Data.DataQuery.DataReaderUtils.ContainsColumn(MyReader, "AllowImpersonation") Then
                            _AllowImpersonation = CType(MyReader("AllowImpersonation"), Boolean)
                        Else
                            _AllowImpersonation = False
                        End If
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

#If NOTIMPLEMENTED Then
            Public Enum InheritionType As Byte
                All = 0
                InheritedAuthorizations = 1
                NonInheritedAuthorizations = 2
            End Enum
            Public Enum DeveloperType As Byte
                All = 0
                Developers = 1
                NonDevelopers = 2
            End Enum
            Public ReadOnly Property AuthorizedUserIDs(ByVal inheritionState As InheritionType, ByVal developerState As DeveloperType) As Long
                Get
                    'TODO: Implementation
                End Get
            End Property
#End If

            ''' <summary>
            '''     The ID value of this server group
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property

            ''' <summary>
            '''     The common title of this server group
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
            '''     The title of this server group in a shorter name, often used for the navigation bars
            ''' </summary>
            ''' <value></value>
            Public Property NavTitle() As String
                Get
                    If _NavTitle <> "" Then
                        Return _NavTitle
                    Else
                        Return _Title
                    End If
                End Get
                Set(ByVal Value As String)
                    _NavTitle = Value
                End Set
            End Property

            ''' <summary>
            '''     The official website title of the company, typically used for the link/logo from the extranet to the internet website
            ''' </summary>
            ''' <value></value>
            Public Property OfficialCompanyWebSiteTitle() As String
                Get
                    Return _OfficialCompanyWebSiteTitle
                End Get
                Set(ByVal Value As String)
                    _OfficialCompanyWebSiteTitle = Value
                End Set
            End Property

            ''' <summary>
            '''     The official website address of the company, typically used for the link/logo from the extranet to the internet website
            ''' </summary>
            ''' <value></value>
            Public Property OfficialCompanyWebSiteURL() As String
                Get
                    Return _OfficialCompanyWebSiteURL
                End Get
                Set(ByVal Value As String)
                    _OfficialCompanyWebSiteURL = Value
                End Set
            End Property

            ''' <summary>
            '''     The company title, e. g. 'YourCompany'
            ''' </summary>
            ''' <value></value>
            Public Property CompanyTitle() As String
                Get
                    Return _CompanyTitle
                End Get
                Set(ByVal Value As String)
                    _CompanyTitle = Value
                End Set
            End Property

            ''' <summary>
            '''     The official company title, e. g. 'YourCompany Ltd.'
            ''' </summary>
            ''' <value></value>
            Public Property CompanyFormerTitle() As String
                Get
                    Return _CompanyFormerTitle
                End Get
                Set(ByVal Value As String)
                    _CompanyFormerTitle = Value
                End Set
            End Property

            ''' <summary>
            '''     The ID value for the group of registered users
            ''' </summary>
            ''' <value></value>
            Public Property GroupPublic() As CompuMaster.camm.WebManager.WMSystem.GroupInformation
                Get
                    If _GroupPublic Is Nothing Then
                        _GroupPublic = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(_GroupPublicID, _WebManager)
                    End If
                    Return _GroupPublic
                End Get
                Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.GroupInformation)
                    _GroupPublic = Value
                    _GroupPublicID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     The ID value for the group of unregistered users
            ''' </summary>
            ''' <value></value>
            Public Property GroupAnonymous() As CompuMaster.camm.WebManager.WMSystem.GroupInformation
                Get
                    If _GroupAnonymous Is Nothing Then
                        _GroupAnonymous = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(_GroupAnonymousID, _WebManager)
                    End If
                    Return _GroupAnonymous
                End Get
                Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.GroupInformation)
                    _GroupAnonymous = Value
                    _GroupAnonymousID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     The master server which is the primary handler for all login requests
            ''' </summary>
            ''' <value></value>
            Public Property MasterServer() As ServerInformation
                Get
                    If _MasterServer Is Nothing Then
                        _MasterServer = New ServerInformation(_MasterServerID, _WebManager)
                    End If
                    Return _MasterServer
                End Get
                Set(ByVal Value As ServerInformation)
                    _MasterServer = Value
                    _MasterServerID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     A reference to an administration server
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     This administration server can be part of another servergroup. This allows you to remove any administration possibilities from your untrusted extranet and to only allow user administration on a server in your intranet.
            ''' </remarks>
            Public Property AdminServer() As ServerInformation
                Get
                    If _AdminServer Is Nothing Then
                        _AdminServer = New ServerInformation(_AdminServerID, _WebManager)
                    End If
                    Return _AdminServer
                End Get
                Set(ByVal Value As ServerInformation)
                    _AdminServer = Value
                    _AdminServerID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     The default access level role for all users who register themselves in this server group
            ''' </summary>
            ''' <value></value>
            Public Property AccessLevelDefault() As AccessLevelInformation
                Get
                    If _AccessLevelDefault Is Nothing Then
                        _AccessLevelDefault = New AccessLevelInformation(_AccessLevelDefaultID, _WebManager)
                    End If
                    Return _AccessLevelDefault
                End Get
                Set(ByVal Value As AccessLevelInformation)
                    _AccessLevelDefault = Value
                    _AccessLevelDefaultID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     The access level roles which are allowed to access this server group
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccessLevels() As AccessLevelInformation()
                Get
                    Static _AccessLevels As AccessLevelInformation()
                    If _AccessLevels Is Nothing Then
                        _AccessLevels = Me._WebManager.System_GetAccessLevelInfos(Me._ID)
                    End If
                    Return _AccessLevels
                End Get
            End Property

            ''' <summary>
            '''     A list of attached servers to this server group
            ''' </summary>
            ''' <value></value>
            Public Property Servers() As ServerInformation()
                Get
                    If _Servers Is Nothing Then
                        _Servers = _WebManager.System_GetServersInfo(_ID)
                    End If
                    Return _Servers
                End Get
                Set(ByVal Value As ServerInformation())
                    _Servers = Value
                End Set
            End Property

            Public Property DevelopmentContactAddress As String
                Get
                    Return _DevelopmentContactAddress
                End Get
                Set(value As String)
                    _DevelopmentContactAddress = value
                End Set
            End Property

            Public Property SecurityContactName As String
                Get
                    Return _SecurityContactName
                End Get
                Set(value As String)
                    _SecurityContactName = value
                End Set
            End Property

            Public Property SecurityContactAddress As String
                Get
                    Return _SecurityContactAddress
                End Get
                Set(value As String)
                    _SecurityContactAddress = value
                End Set
            End Property

            Public Property DevelopmentContactName As String
                Get
                    Return _DevelopmentContactName
                End Get
                Set(value As String)
                    _DevelopmentContactName = value
                End Set
            End Property

            Public Property ContentManagementContactName As String
                Get
                    Return _ContentManagementContactName
                End Get
                Set(value As String)
                    _ContentManagementContactName = value
                End Set
            End Property

            Public Property ContentManagementContactAddress As String
                Get
                    Return _ContentManagementContactAddress
                End Get
                Set(value As String)
                    _ContentManagementContactAddress = value
                End Set
            End Property

            Public Property UnspecifiedContactName As String
                Get
                    Return _UnspecifiedContactName
                End Get
                Set(value As String)
                    _UnspecifiedContactName = value
                End Set
            End Property

            Public Property UnspecifiedContactAddress As String
                Get
                    Return _UnspecifiedContactAddress
                End Get
                Set(value As String)
                    _UnspecifiedContactAddress = value
                End Set
            End Property

            Public Property AllowImpersonation As Boolean
                Get
                    Return _AllowImpersonation
                End Get
                Set(value As Boolean)
                    _AllowImpersonation = value
                End Set
            End Property

            Public Property CopyrightSinceYear As Integer
                Get
                    Return _CopyrightSinceYear
                End Get
                Set(value As Integer)
                    _CopyrightSinceYear = value
                End Set
            End Property

            Public Property ImageUrlBig As String
                Get
                    Return _ImageUrlBig
                End Get
                Set(value As String)
                    _ImageUrlBig = value
                End Set
            End Property

            Public Property ImageUrlSmall As String
                Get
                    Return _ImageUrlSmall
                End Get
                Set(value As String)
                    _ImageUrlSmall = value
                End Set
            End Property

        End Class

    End Class

End Namespace