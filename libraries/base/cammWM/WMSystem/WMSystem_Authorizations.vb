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
        '''     Authorizations
        ''' </summary>
        Public Class Authorizations
            Implements IAuthorizationInformation

            Private _WebManager As WMSystem
            Private _SecurityObjectID As Integer
            Private _ServerGroupID As Integer
            Private _UserID As Long
            Private _UserGroupID As Integer

            ''' <summary>
            '''     An authorization for an user group
            ''' </summary>
            Public Class GroupAuthorizationInformation
                Implements IGroupAuthorizationInformation

                Dim _WebManager As WMSystem
                Dim _ID As Integer
                Dim _SecurityObjectID As Integer
                Dim _SecurityObjectInfo As SecurityObjectInformation
                Dim _GroupID As Integer
                Dim _GroupInfo As GroupInformation
                Dim _ServerGroupID As Integer
                Dim _ServerGroupInfo As ServerGroupInformation
                Dim _IsDenyRule As Boolean
                Dim _IsDevRule As Boolean

                Friend Sub New(ByRef WebManager As WMSystem, ByVal ID As Integer, ByVal SecurityObjectID As Integer, ByVal GroupID As Integer, ByVal ServerGroupID As Integer, IsDevelopmentAuth As Boolean, ReleasedOn As DateTime, ReleasedByUserID As Long, IsDenyRule As Boolean)
                    _WebManager = WebManager
                    _ID = ID
                    _SecurityObjectID = SecurityObjectID
                    _GroupID = GroupID
                    _ServerGroupID = ServerGroupID
                    _ReleasedBy_UserID = ReleasedByUserID
                    _ReleasedOn = ReleasedOn
                    _IsDenyRule = IsDenyRule
                    _IsDevRule = IsDenyRule
                End Sub

                ''' <summary>
                '''     The ID value for this authorization item
                ''' </summary>
                ''' <value></value>
                Public Property ID() As Integer
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As Integer)
                        _ID = Value
                    End Set
                End Property

                ''' <summary>
                '''     The security object which is pointed by this authorization
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property SecurityObjectInfo() As SecurityObjectInformation
                    Get
                        If _SecurityObjectInfo Is Nothing Then
                            _SecurityObjectInfo = New SecurityObjectInformation(_SecurityObjectID, _WebManager)
                        End If
                        Return _SecurityObjectInfo
                    End Get
                End Property

                ''' <summary>
                '''     A user group which has been authorized
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property GroupInfo() As CompuMaster.camm.WebManager.WMSystem.GroupInformation
                    Get
                        If _GroupInfo Is Nothing Then
                            _GroupInfo = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(_GroupID, _WebManager)
                        End If
                        Return _GroupInfo
                    End Get
                End Property

                ''' <summary>
                '''     A server group where this authorization shall take effect
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property ServerGroupInfo() As ServerGroupInformation
                    Get
                        If _ServerGroupInfo Is Nothing Then
                            _ServerGroupInfo = New ServerGroupInformation(_ServerGroupID, _WebManager)
                        End If
                        Return _ServerGroupInfo
                    End Get
                End Property

                ''' <summary>
                '''     The ID value of the user group
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property GroupID() As Integer
                    Get
                        Return _GroupID
                    End Get
                End Property

                ''' <summary>
                '''     The ID value of the targetted security object
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property SecurityObjectID() As Integer
                    Get
                        Return _SecurityObjectID
                    End Get
                End Property

                ''' <summary>
                '''     The ID value of the effected server group
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property ServerGroupID() As Integer
                    Get
                        Return _ServerGroupID
                    End Get
                End Property
                Friend WriteOnly Property Friend_SecurityObjectInfo() As SecurityObjectInformation
                    Set(ByVal Value As SecurityObjectInformation)
                        _SecurityObjectInfo = Value
                    End Set
                End Property
                Friend WriteOnly Property Friend_GroupInfo() As GroupInformation
                    Set(ByVal Value As GroupInformation)
                        _GroupInfo = Value
                    End Set
                End Property
                Friend WriteOnly Property Friend_ServerGroupInfo() As ServerGroupInformation
                    Set(ByVal Value As ServerGroupInformation)
                        _ServerGroupInfo = Value
                    End Set
                End Property

                ''' <summary>
                ''' Allow-rules GRANT access, Deny-rules DENY the access
                ''' </summary>
                Public Property IsDenyRule() As Boolean
                    Get
                        Return _IsDenyRule
                    End Get
                    Set(ByVal Value As Boolean)
                        _IsDenyRule = Value
                    End Set
                End Property

                ''' <summary>
                ''' Allow-rules GRANT access, Deny-rules DENY the access
                ''' </summary>
                Public Property IsDevRule() As Boolean
                    Get
                        Return _IsDevRule
                    End Get
                    Set(ByVal Value As Boolean)
                        _IsDevRule = Value
                    End Set
                End Property

#Region "Modification/Release information"
                Dim _ReleasedBy_UserID As Long
                Dim _ReleasedBy_UserInfo As UserInformation
                Dim _ReleasedOn As DateTime
                ''' <summary>
                '''     The release has been done by this user
                ''' </summary>
                Public Property ReleasedBy_UserID() As Long
                    Get
                        Return CType(_ReleasedBy_UserID, Long)
                    End Get
                    Set(ByVal Value As Long)
                        _ReleasedBy_UserID = Value
                        _ReleasedBy_UserInfo = Nothing
                    End Set
                End Property
                ''' <summary>
                '''     The release has been done by this user
                ''' </summary>
                Public Property ReleasedBy_UserInfo() As UserInformation
                    Get
                        If _ReleasedBy_UserInfo Is Nothing Then
                            _ReleasedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ReleasedBy_UserID, _WebManager, True)
                        End If
                        Return _ReleasedBy_UserInfo
                    End Get
                    Set(ByVal Value As UserInformation)
                        _ReleasedBy_UserInfo = Value
                        _ReleasedBy_UserID = _ReleasedBy_UserInfo.IDLong
                    End Set
                End Property
                ''' <summary>
                '''     The release has been done on this date/time
                ''' </summary>
                Public Property ReleasedOn() As DateTime
                    Get
                        Return _ReleasedOn
                    End Get
                    Set(ByVal Value As DateTime)
                        _ReleasedOn = Value
                    End Set
                End Property
#End Region

            End Class

            ''' <summary>
            '''     An authorization for an user
            ''' </summary>
            Public Class UserAuthorizationInformation
                Implements IUserAuthorizationInformation

                Dim _WebManager As WMSystem
                Dim _ID As Integer
                Dim _SecurityObjectID As Integer
                Dim _SecurityObjectInfo As SecurityObjectInformation
                Dim _UserID As Long
                Dim _UserInfo As UserInformation
                Dim _ServerGroupID As Integer
                Dim _ServerGroupInfo As ServerGroupInformation
                Dim _AlsoVisibleIfDisabled As Boolean
                Dim _IsDenyRule As Boolean

                Friend Sub New(ByRef WebManager As WMSystem, ByVal ID As Integer, ByVal SecurityObjectID As Integer, ByVal UserID As Long, ByVal ServerGroupID As Integer, ByVal AlsoVisibleIfDisabled As Boolean, ReleasedOn As DateTime, ReleasedByUserID As Long, IsDenyRule As Boolean)
                    _WebManager = WebManager
                    _ID = ID
                    _SecurityObjectID = SecurityObjectID
                    _UserID = UserID
                    _ServerGroupID = ServerGroupID
                    _AlsoVisibleIfDisabled = AlsoVisibleIfDisabled
                    _ReleasedBy_UserID = ReleasedByUserID
                    _ReleasedOn = ReleasedOn
                    _IsDenyRule = IsDenyRule
                End Sub

                ''' <summary>
                '''     The ID value for this authorization item
                ''' </summary>
                ''' <value></value>
                Public Property ID() As Integer
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As Integer)
                        _ID = Value
                    End Set
                End Property

                ''' <summary>
                '''     Is the user allowed to see and access the link to this security object application even if the security object hasn't been activated?
                ''' </summary>
                ''' <value></value>
                ''' <remarks>
                '''     Often, developers need access to test their new applcations before they can go live
                ''' </remarks>
                Public Property AlsoVisibleIfDisabled() As Boolean
                    Get
                        Return _AlsoVisibleIfDisabled
                    End Get
                    Set(ByVal Value As Boolean)
                        _AlsoVisibleIfDisabled = Value
                    End Set
                End Property
                ''' <summary>
                ''' Allow-rules GRANT access, Deny-rules DENY the access
                ''' </summary>
                Public Property IsDenyRule() As Boolean
                    Get
                        Return _IsDenyRule
                    End Get
                    Set(ByVal Value As Boolean)
                        _IsDenyRule = Value
                    End Set
                End Property

                ''' <summary>
                '''     A security object which is pointed by this authorization 
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property SecurityObjectInfo() As SecurityObjectInformation
                    Get
                        If _SecurityObjectInfo Is Nothing Then
                            _SecurityObjectInfo = New SecurityObjectInformation(_SecurityObjectID, _WebManager)
                        End If
                        Return _SecurityObjectInfo
                    End Get
                End Property

                ''' <summary>
                '''     The user which has got the authorization
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property UserInfo() As UserInformation
                    Get
                        If _UserInfo Is Nothing Then
                            _UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_UserID, _WebManager)
                        End If
                        Return _UserInfo
                    End Get
                End Property

                ''' <summary>
                '''     The server group where this authorization shall take effect
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property ServerGroupInfo() As ServerGroupInformation
                    Get
                        If _ServerGroupInfo Is Nothing Then
                            _ServerGroupInfo = New ServerGroupInformation(_ServerGroupID, _WebManager)
                        End If
                        Return _ServerGroupInfo
                    End Get
                End Property

                ''' <summary>
                '''     The user which has got the authorization
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property UserID() As Integer
                    Get
                        Return CType(_UserID, Integer)
                    End Get
                End Property

                ''' <summary>
                '''     A security object which is pointed by this authorization 
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property SecurityObjectID() As Integer
                    Get
                        Return _SecurityObjectID
                    End Get
                End Property

                ''' <summary>
                '''     The server group where this authorization shall take effect
                ''' </summary>
                ''' <value></value>
                Public ReadOnly Property ServerGroupID() As Integer
                    Get
                        Return _ServerGroupID
                    End Get
                End Property
                Friend WriteOnly Property Friend_SecurityObjectInfo() As SecurityObjectInformation
                    Set(ByVal Value As SecurityObjectInformation)
                        _SecurityObjectInfo = Value
                    End Set
                End Property
                Friend WriteOnly Property Friend_UserInfo() As UserInformation
                    Set(ByVal Value As UserInformation)
                        _UserInfo = Value
                    End Set
                End Property
                Friend WriteOnly Property Friend_ServerGroupInfo() As ServerGroupInformation
                    Set(ByVal Value As ServerGroupInformation)
                        _ServerGroupInfo = Value
                    End Set
                End Property

#Region "Modification/Release information"
                Dim _ReleasedBy_UserID As Long
                Dim _ReleasedBy_UserInfo As UserInformation
                Dim _ReleasedOn As DateTime
                ''' <summary>
                '''     The release has been done by this user
                ''' </summary>
                Public Property ReleasedBy_UserID() As Long
                    Get
                        Return CType(_ReleasedBy_UserID, Long)
                    End Get
                    Set(ByVal Value As Long)
                        _ReleasedBy_UserID = Value
                        _ReleasedBy_UserInfo = Nothing
                    End Set
                End Property
                ''' <summary>
                '''     The release has been done by this user
                ''' </summary>
                Public Property ReleasedBy_UserInfo() As UserInformation
                    Get
                        If _ReleasedBy_UserInfo Is Nothing Then
                            _ReleasedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ReleasedBy_UserID, _WebManager, True)
                        End If
                        Return _ReleasedBy_UserInfo
                    End Get
                    Set(ByVal Value As UserInformation)
                        _ReleasedBy_UserInfo = Value
                        _ReleasedBy_UserID = _ReleasedBy_UserInfo.IDLong
                    End Set
                End Property
                ''' <summary>
                '''     The release has been done on this date/time
                ''' </summary>
                Public Property ReleasedOn() As DateTime
                    Get
                        Return _ReleasedOn
                    End Get
                    Set(ByVal Value As DateTime)
                        _ReleasedOn = Value
                    End Set
                End Property
#End Region

            End Class
            Dim _AuthorizedGroups As New Collection
            Dim _AuthorizedUsers As New Collection
            Dim _AuthorizedGroupInfos As GroupAuthorizationInformation()
            Dim _AuthorizedUserInfos As UserAuthorizationInformation()
            Dim _DBVersion As Version
            Dim _ReloadData As Boolean

            Public ReadOnly Property InheritedAuthorizations() As Authorizations
                Get
                    Static _InheritedAuthorizations As Authorizations
                    Static _InheritingFromSecurityObjectID As Integer

                    If _SecurityObjectID = Nothing Then
                        Throw New NotSupportedException("Searching for inherited authorizations only available when already filtering for one special security object")
                    End If

                    'Check if data has changed
                    If _ReloadData = True Then
                        ReloadData()
                        _InheritedAuthorizations = Nothing
                    End If

                    'Query the demanded data
                    If _InheritedAuthorizations Is Nothing Then
                        If _InheritingFromSecurityObjectID = Nothing Then
                            Dim iSecObj As SecurityObjectInformation = New SecurityObjectInformation(_SecurityObjectID, _WebManager, False)
                            If Not iSecObj Is Nothing Then
                                _InheritingFromSecurityObjectID = iSecObj.InheritFrom_SecurityObjectID
                            End If
                        End If
                        If _InheritingFromSecurityObjectID <> Nothing Then
                            _InheritedAuthorizations = New Authorizations(_InheritingFromSecurityObjectID, _WebManager, _ServerGroupID, _UserGroupID, _UserID)
                        End If
                    End If
                    Return _InheritedAuthorizations
                End Get
            End Property

            ''' <summary>
            '''     Load the list of assigned authorizations
            ''' </summary>
            ''' <param name="SecurityObjectID">When not null (Nothing in VisualBasic) then filter for this security object else don't filter for this value</param>
            ''' <param name="WebManager">The instance of a camm Web-Manager</param>
            ''' <param name="ServerGroupID">When not null (Nothing in VisualBasic) then filter for this server group else don't filter for this value</param>
            Public Sub New(ByVal SecurityObjectID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal ServerGroupID As Integer = Nothing)
                Me.New(SecurityObjectID, WebManager, ServerGroupID, Nothing, Nothing)
            End Sub

            ''' <summary>
            '''     Load the list of assigned authorizations
            ''' </summary>
            ''' <param name="webManager">The instance of a camm Web-Manager</param>
            ''' <param name="securityObjectID">When not null (Nothing in VisualBasic) then filter for this security object else don't filter for this value</param>
            ''' <param name="serverGroupID">When not null (Nothing in VisualBasic) then filter for this server group else don't filter for this value</param>
            ''' <param name="userGroupID">When not null (Nothing in VisualBasic) then filter for this user group else don't filter for this value</param>
            ''' <param name="userID">When not null (Nothing in VisualBasic) then filter for this user else don't filter for this value</param>
            Public Sub New(ByVal webManager As CompuMaster.camm.WebManager.WMSystem, ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, ByVal userGroupID As Integer, ByVal userID As Long)
                Me.New(securityObjectID, webManager, serverGroupID, userGroupID, userID)
            End Sub

            ''' <summary>
            '''     Load the list of assigned authorizations
            ''' </summary>
            ''' <param name="securityObjectID">When not null (Nothing in VisualBasic) then filter for this security object else don't filter for this value</param>
            ''' <param name="webManager">The instance of a camm Web-Manager</param>
            ''' <param name="serverGroupID">When not null (Nothing in VisualBasic) then filter for this server group else don't filter for this value</param>
            ''' <param name="userGroupID">When not null (Nothing in VisualBasic) then filter for this user group else don't filter for this value</param>
            ''' <param name="userID">When not null (Nothing in VisualBasic) then filter for this user else don't filter for this value</param>
            Public Sub New(ByVal securityObjectID As Integer, ByRef webManager As CompuMaster.camm.WebManager.WMSystem, ByVal serverGroupID As Integer, ByVal userGroupID As Integer, ByVal userID As Long)
                _WebManager = webManager
                _SecurityObjectID = securityObjectID
                _ServerGroupID = serverGroupID
                _UserID = userID
                _UserGroupID = userGroupID

                'Preparation
                If securityObjectID = Nothing And serverGroupID <> Nothing Then
                    Throw New Exception("Not yet supported: list of security objects of a specific server group")
                End If
                _DBVersion = Setup.DatabaseUtils.Version(_WebManager, True)
                If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                    Throw New NotImplementedException("Support for database version " & _DBVersion.ToString & " is currently not supported. Please update the camm WebManager software, first!")
                End If

                Dim MyConn As New SqlConnection(webManager.ConnectionString)
                Dim MyCmd As SqlCommand = Nothing
                Dim MyReader As SqlDataReader = Nothing
                Try
                    MyConn.Open()
                    Dim filter As String

                    'Fill the list of authorized users
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByUser] WHERE 1 = 1", MyConn)
                    filter = Nothing
                    If securityObjectID <> Nothing Then
                        filter &= vbNewLine & "AND ID_Application = @IDApplication"
                        MyCmd.Parameters.Add("@IDApplication", SqlDbType.Int).Value = securityObjectID
                    End If
                    If serverGroupID <> Nothing Then
                        filter &= vbNewLine & "AND ID_ServerGroup = @IDServerGroup"
                        MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = serverGroupID
                    End If
                    If userID <> Nothing Then
                        filter &= vbNewLine & "AND ID_GroupOrPerson = @IDGroupOrPerson"
                        MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = userID
                    End If
                    If userGroupID <> Nothing Then
                        'When we want to filter for a group, we can't get results for users
                        filter &= vbNewLine & "AND 1=0"
                    End If
                    MyCmd.CommandText &= filter
                    MyReader = MyCmd.ExecuteReader()
                    While MyReader.Read
                        Dim MyServerGroup As Integer
                        If MyReader.GetSchemaTable.Columns.Contains("ID_ServerGroup") Then
                            MyServerGroup = Utils.Nz(MyReader("ID_ServerGroup"), 0)
                        Else
                            MyServerGroup = Nothing
                        End If
                        Dim MyIsDenyRule As Boolean
                        If MyReader.GetSchemaTable.Columns.Contains("IsDenyRule") Then
                            MyIsDenyRule = Utils.Nz(MyReader("IsDenyRule"), False)
                        Else
                            MyIsDenyRule = Nothing
                        End If
                        _AuthorizedUsers.Add(New UserAuthorizationInformation(_WebManager, _
                            CType(MyReader("ID"), Integer), _
                            CType(MyReader("ID_Application"), Integer), _
                            CType(MyReader("ID_GroupOrPerson"), Long), _
                            MyServerGroup, _
                            Utils.Nz(MyReader("DevelopmentTeamMember"), False), _
                            CType(MyReader("ReleasedOn"), DateTime), _
                            CType(MyReader("ReleasedBy"), Long), _
                            MyIsDenyRule))
                    End While
                    MyReader.Close()

                    'Fill the list of authorized groups
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByGroup] WHERE 1 = 1", MyConn)
                    filter = Nothing
                    If securityObjectID <> Nothing Then
                        filter &= vbNewLine & "AND ID_Application = @IDApplication"
                        MyCmd.Parameters.Add("@IDApplication", SqlDbType.Int).Value = securityObjectID
                    End If
                    If serverGroupID <> Nothing Then
                        filter &= vbNewLine & "AND ID_ServerGroup = @IDServerGroup"
                        MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = serverGroupID
                    End If
                    If userGroupID <> Nothing Then
                        filter &= vbNewLine & "AND ID_GroupOrPerson = @IDGroupOrPerson"
                        MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = userGroupID
                    End If
                    If userID <> Nothing Then
                        'When we want to filter for a user, we can't get results for group
                        filter &= vbNewLine & "AND 1=0"
                    End If
                    MyCmd.CommandText &= filter
                    MyReader = MyCmd.ExecuteReader()
                    While MyReader.Read
                        Dim MyServerGroup As Integer
                        If MyReader.GetSchemaTable.Columns.Contains("ID_ServerGroup") Then
                            MyServerGroup = Utils.Nz(MyReader("ID_ServerGroup"), 0)
                        Else
                            MyServerGroup = Nothing
                        End If
                        Dim MyIsDenyRule As Boolean
                        If MyReader.GetSchemaTable.Columns.Contains("IsDenyRule") Then
                            MyIsDenyRule = Utils.Nz(MyReader("IsDenyRule"), False)
                        Else
                            MyIsDenyRule = Nothing
                        End If
                        Dim MyIsDev As Boolean
                        If MyReader.GetSchemaTable.Columns.Contains("DevelopmentTeamMember") Then
                            MyIsDev = Utils.Nz(MyReader("DevelopmentTeamMember"), False)
                        Else
                            MyIsDev = False
                        End If
                        _AuthorizedGroups.Add(New GroupAuthorizationInformation(_WebManager, _
                            CType(MyReader("ID"), Integer), _
                            CType(MyReader("ID_Application"), Integer), _
                            CType(MyReader("ID_GroupOrPerson"), Integer), _
                            MyServerGroup, _
                            MyIsDev, _
                            CType(MyReader("ReleasedOn"), DateTime), _
                            CType(MyReader("ReleasedBy"), Long), _
                            MyIsDenyRule))
                    End While
                    MyReader.Close()

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

                'Quick loads
                LoadUserAndGroupInformations()
            End Sub

            Private Sub LoadUserAndGroupInformations()

                'Use quick load mechanisms for each group information object
                If Me._AuthorizedGroups.Count > 0 Then
                    Dim NeededGroupIDs As New ArrayList
                    For Each MyGroupAuthInfo As GroupAuthorizationInformation In Me._AuthorizedGroups
                        If Not NeededGroupIDs.Contains(MyGroupAuthInfo.GroupID) Then
                            NeededGroupIDs.Add(MyGroupAuthInfo.GroupID)
                        End If
                    Next
                    Dim MyGroupInfos As GroupInformation() = _WebManager.System_GetGroupInfos(NeededGroupIDs)
                    If Not MyGroupInfos Is Nothing Then
                        For Each MyGroupInfo As GroupInformation In MyGroupInfos
                            For Each MyGroupAuthInfo As GroupAuthorizationInformation In _AuthorizedGroups
                                If MyGroupInfo.ID = MyGroupAuthInfo.GroupID Then
                                    MyGroupAuthInfo.Friend_GroupInfo = MyGroupInfo
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                End If

                'Use quick load mechanisms for each user information object
                If Me._AuthorizedUsers.Count > 0 Then
                    Dim NeededUserIDs As New ArrayList
                    For Each MyUserAuthInfo As UserAuthorizationInformation In Me._AuthorizedUsers
                        If Not NeededUserIDs.Contains(CType(MyUserAuthInfo.UserID, Long)) Then
                            NeededUserIDs.Add(CType(MyUserAuthInfo.UserID, Long))
                        End If
                    Next
                    Dim MyUserInfos As UserInformation() = _WebManager.System_GetUserInfos(CType(NeededUserIDs.ToArray(GetType(Long)), Long()))
                    If Not MyUserInfos Is Nothing Then
                        For Each MyUserInfo As UserInformation In MyUserInfos
                            For Each MyUserAuthInfo As UserAuthorizationInformation In _AuthorizedUsers
                                If MyUserInfo.IDLong = MyUserAuthInfo.UserID Then
                                    MyUserAuthInfo.Friend_UserInfo = MyUserInfo
                                    Exit For
                                End If
                            Next
                        Next
                    End If
                End If

            End Sub

            Private Sub ReloadData()
                Dim MyReloadedData As New CompuMaster.camm.WebManager.WMSystem.Authorizations(_SecurityObjectID, _WebManager, _ServerGroupID)
                Me._AuthorizedUsers = MyReloadedData.GetUserAuthorizationInformations
                Me._AuthorizedGroups = MyReloadedData.GetGroupAuthorizationInformations
                LoadUserAndGroupInformations()
            End Sub

            Public ReadOnly Property GroupAuthorizationInformations(Optional ByVal GroupID As Integer = Nothing) As GroupAuthorizationInformation()
                Get
                    'Check if data has changed
                    If _ReloadData = True Then
                        ReloadData()
                        _AuthorizedGroupInfos = Nothing
                    End If

                    If _AuthorizedGroups.Count = 0 Then
                        Return Nothing
                    ElseIf GroupID = Nothing Then
                        'Do the normal job
                        If _AuthorizedGroupInfos Is Nothing Then
                            ReDim _AuthorizedGroupInfos(_AuthorizedGroups.Count - 1)
                            For MyCounter As Integer = 0 To _AuthorizedGroups.Count - 1
                                _AuthorizedGroupInfos(MyCounter) = CType(_AuthorizedGroups(MyCounter + 1), GroupAuthorizationInformation)
                            Next
                            Return _AuthorizedGroupInfos
                        Else
                            Return _AuthorizedGroupInfos
                        End If
                    Else
                        'only return those results which matches the given user id
                        Dim MyAuthorizedGroups As New Collection
                        For Each MyAuthorizedGroupInfo As Authorizations.GroupAuthorizationInformation In _AuthorizedGroups
                            If MyAuthorizedGroupInfo.GroupID = GroupID Then
                                MyAuthorizedGroups.Add(MyAuthorizedGroupInfo)
                            End If
                        Next
                        If MyAuthorizedGroups.Count = 0 Then
                            Return Nothing
                        Else
                            Dim MyAuthorizedGroupInfos As Authorizations.GroupAuthorizationInformation()
                            ReDim MyAuthorizedGroupInfos(MyAuthorizedGroups.Count - 1)
                            For MyCounter As Integer = 0 To MyAuthorizedGroups.Count - 1
                                MyAuthorizedGroupInfos(MyCounter) = CType(MyAuthorizedGroups(MyCounter + 1), GroupAuthorizationInformation)
                            Next
                            Return MyAuthorizedGroupInfos
                        End If
                    End If
                End Get
            End Property
            Public ReadOnly Property GroupInformation(ByVal GroupID As Integer) As GroupAuthorizationInformation
                Get
                    'Check if data has changed
                    If _ReloadData = True Then
                        ReloadData()
                    End If

                    'Do the normal job
                    For MyCounter As Integer = 0 To _AuthorizedGroups.Count - 1
                        If CType(_AuthorizedGroups(MyCounter), GroupInformation).ID = GroupID Then
                            Return CType(_AuthorizedGroups(MyCounter), GroupAuthorizationInformation)
                        End If
                    Next
                    Return Nothing
                End Get
            End Property
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Public ReadOnly Property UserAuthorizationInformations(ByVal UserID As Integer) As UserAuthorizationInformation()
                Get
                    Return UserAuthorizationInformations(CLng(UserID))
                End Get
            End Property
            Public ReadOnly Property UserAuthorizationInformations() As UserAuthorizationInformation()
                Get
                    Return UserAuthorizationInformations(CType(Nothing, Long))
                End Get
            End Property
            Public ReadOnly Property UserAuthorizationInformations(ByVal UserID As Long) As UserAuthorizationInformation()
                Get
                    'Check if data has changed
                    If _ReloadData = True Then
                        ReloadData()
                        _AuthorizedUserInfos = Nothing
                    End If

                    If _AuthorizedUsers.Count = 0 Then
                        Return Nothing
                    ElseIf UserID = Nothing Then
                        'Do the normal job
                        If _AuthorizedUserInfos Is Nothing Then
                            ReDim _AuthorizedUserInfos(_AuthorizedUsers.Count - 1)
                            For MyCounter As Integer = 0 To _AuthorizedUsers.Count - 1
                                _AuthorizedUserInfos(MyCounter) = CType(_AuthorizedUsers(MyCounter + 1), UserAuthorizationInformation)
                            Next
                            Return _AuthorizedUserInfos
                        Else
                            Return _AuthorizedUserInfos
                        End If
                    Else
                        'only return those results which matches the given user id
                        Dim MyAuthorizedUsers As New Collection
                        For Each MyAuthorizedUserInfo As Authorizations.UserAuthorizationInformation In _AuthorizedUsers
                            If MyAuthorizedUserInfo.UserID = UserID Then
                                MyAuthorizedUsers.Add(MyAuthorizedUserInfo)
                            End If
                        Next
                        If MyAuthorizedUsers.Count = 0 Then
                            Return Nothing
                        Else
                            Dim MyAuthorizedUserInfos As Authorizations.UserAuthorizationInformation()
                            ReDim MyAuthorizedUserInfos(MyAuthorizedUsers.Count - 1)
                            For MyCounter As Integer = 0 To MyAuthorizedUsers.Count - 1
                                MyAuthorizedUserInfos(MyCounter) = CType(MyAuthorizedUsers(MyCounter + 1), UserAuthorizationInformation)
                            Next
                            Return MyAuthorizedUserInfos
                        End If
                    End If
                End Get
            End Property
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public ReadOnly Property UserInformation(ByVal UserID As Integer) As UserInformation
                Get
                    Return UserInformation(CLng(UserID))
                End Get
            End Property
            Public ReadOnly Property UserInformation(ByVal UserID As Long) As UserInformation
                Get
                    'Check if data has changed
                    If _ReloadData = True Then
                        ReloadData()
                    End If

                    'Do the normal job
                    For MyCounter As Integer = 0 To _AuthorizedUsers.Count - 1
                        If CType(_AuthorizedUsers(MyCounter), UserInformation).IDLong = UserID Then
                            Return CType(_AuthorizedUsers(MyCounter), UserInformation)
                        End If
                    Next
                    Return Nothing
                End Get
            End Property

            Protected Function GetGroupAuthorizationInformations() As Collection
                'Check if data has changed
                If _ReloadData = True Then
                    ReloadData()
                End If

                'Do the normal job
                Return Me._AuthorizedGroups
            End Function
            Protected Function GetUserAuthorizationInformations() As Collection
                'Check if data has changed
                If _ReloadData = True Then
                    ReloadData()
                End If

                'Do the normal job
                Return Me._AuthorizedUsers
            End Function

            <Obsolete("STRONGLY RECOMMENDED: Use overloaded alternative")> Public Sub AddGroupAuthorization(ByVal GroupID As Integer, ByVal ServerGroupID As Integer, Optional ByVal SecurityObjectID As Integer = Nothing)
                AddGroupAuthorization(GroupID, ServerGroupID, SecurityObjectID, False, False)
            End Sub

            Public Sub AddGroupAuthorization(ByVal GroupID As Integer, ByVal ServerGroupID As Integer, ByVal SecurityObjectID As Integer, IsDenyRule As Boolean, IsDevRule As Boolean)

                'Welche SecurityObjectID?
                Dim MySecurityObjectID As Integer
                If SecurityObjectID <> Nothing Then
                    MySecurityObjectID = SecurityObjectID
                Else
                    MySecurityObjectID = _SecurityObjectID
                End If

                'Alle Vorbedingungen erfüllt?
                If MySecurityObjectID = Nothing Then
                    Throw New Exception("Parameter 'SecurityObjectID' required")
                End If

                'Fill the list of authorized users
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn
                If _DBVersion.CompareTo(MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND IsNull(ID_ServerGroup, 0) = @IDServerGroup AND IsDenyRule = @IsDenyRule AND DevelopmentTeamMember = @IsDevRule" & vbNewLine & _
                                        "INSERT INTO [dbo].[ApplicationsRightsByGroup] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn, ID_ServerGroup, DevelopmentTeamMember, IsDenyRule) " & _
                                        "VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate(), @IDServerGroup, @IsDevRule, @IsDenyRule)"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                    MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = ServerGroupID
                    MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                    MyCmd.Parameters.Add("@IsDevRule", SqlDbType.Bit).Value = IsDevRule
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = IsDenyRule
                Else
                    If ServerGroupID <> Nothing Then
                        Throw New Exception("Parameter 'ServerGroupID' not supported by the currently used database version")
                    ElseIf IsDenyRule = True Then
                        Throw New Exception("Parameter 'IsDenyRule' not supported by the currently used database version")
                    ElseIf IsDevRule = True Then
                        Throw New Exception("Parameter 'IsDevRule' not supported by the currently used database version")
                    End If
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson" & vbNewLine & _
                                        "INSERT INTO [dbo].[ApplicationsRightsByGroup] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate())"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                    MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                End If

                Try
                    MyConn.Open()
                    MyCmd.ExecuteNonQuery()
                Finally
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

                'Internes Objekt aktualisieren
                If MySecurityObjectID = _SecurityObjectID Then
                    'internes Memory-Objekt muss ebenfalls aktualisiert werden
                    _ReloadData = True
                End If
            End Sub

            Private ReadOnly Property RequiredApplicationFlags As String()
                Get
                    Static _RequiredApplicationFlags As String()
                    If _RequiredApplicationFlags Is Nothing Then
                        _RequiredApplicationFlags = SecurityObjectInformation.RequiredAdditionalFlags(Me._SecurityObjectID, Me._WebManager)
                    End If
                    Return _RequiredApplicationFlags
                End Get
            End Property

            <Obsolete("STRONGLY RECOMMENDED: Use overloaded alternative")> Public Sub AddUserAuthorization(ByRef UserInfo As UserInformation, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)
                AddUserAuthorization(UserInfo, False, ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled, SecurityObjectID, Notifications)
            End Sub

            Public Sub AddUserAuthorization(ByRef UserInfo As UserInformation, IsDenyRule As Boolean, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)

                'mycmd.CommandText = "SELECT * FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDApplication AND ID_ServerGroup = @IDServerGroup"
                'Welches SecurityObjectID?
                Dim MySecurityObjectID As Integer
                If SecurityObjectID <> Nothing Then
                    MySecurityObjectID = SecurityObjectID
                Else
                    MySecurityObjectID = _SecurityObjectID
                End If

                'Alle Vorbedingungen erfüllt?
                If MySecurityObjectID = Nothing Then
                    Throw New Exception("Parameter 'SecurityObjectID' required")
                ElseIf IsDenyRule = False Then
                    Dim RequiredFlagsValidationResults As FlagValidation.FlagValidationResult() = FlagValidation.ValidateRequiredFlags(UserInfo, Me.RequiredApplicationFlags, True)
                    If RequiredFlagsValidationResults.Length <> 0 Then
                        Throw New FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                    End If
                End If

                'Fill the list of authorized users
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn
                If _DBVersion.CompareTo(MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND IsNull(ID_ServerGroup, 0) = @IDServerGroup AND DevelopmentTeamMember = @DevelopmentTeamMember AND IsDenyRule = @IsDenyRule" & vbNewLine & _
                                        "INSERT INTO [dbo].[ApplicationsRightsByUser] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn, ID_ServerGroup, DevelopmentTeamMember, IsDenyRule) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate(), @IDServerGroup, @DevelopmentTeamMember, @IsDenyRule)"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                    MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = ServerGroupID
                    MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = UserInfo.IDLong
                    MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = IsDenyRule
                Else
                    If ServerGroupID <> Nothing Then
                        Throw New Exception("Parameter 'ServerGroupID' not supported by the currently used database version")
                    ElseIf IsDenyRule = True Then
                        Throw New Exception("Parameter 'IsDenyRule' not supported by the currently used database version")
                    End If
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND DevelopmentTeamMember = @DevelopmentTeamMember" & vbNewLine & _
                                        "INSERT INTO [dbo].[ApplicationsRightsByUser] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn, DevelopmentTeamMember) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate(), @DevelopmentTeamMember)"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = UserInfo.IDLong
                    MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                    MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
                End If

                Try
                    MyConn.Open()
                    MyCmd.ExecuteNonQuery()
                Finally
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

                If UserInfo.AccountAuthorizationsAlreadySet = False Then
                    'send e-mail when first authorizations have been set up
                    UserInfo.AccountAuthorizationsAlreadySet = True
                    'Check wether InitAuthorizationsDone flag has been set
                    If DataLayer.Current.SetUserDetail(_WebManager, Nothing, UserInfo.IDLong, "InitAuthorizationsDone", "1", True) Then
                        Try
                            If Notifications Is Nothing Then
                                _WebManager.Notifications.NotificationForUser_AuthorizationsSet(UserInfo)
                            Else
                                Notifications.NotificationForUser_AuthorizationsSet(UserInfo)
                            End If
                        Catch
                        End Try
                    End If
                End If

                'Internes Objekt aktualisieren
                If MySecurityObjectID = _SecurityObjectID Then
                    'internes Memory-Objekt muss ebenfalls aktualisiert werden
                    _ReloadData = True
                End If

            End Sub
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub AddUserAuthorization(ByVal UserID As Integer, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)
                AddUserAuthorization(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(UserID), _WebManager), ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled, SecurityObjectID, Notifications)
            End Sub
            <Obsolete("STRONGLY RECOMMENDED: Use overloaded alternative")> Public Sub AddUserAuthorization(ByVal UserID As Long, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)
                AddUserAuthorization(New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, _WebManager), ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled, SecurityObjectID, Notifications)
            End Sub

            <Obsolete("STRONGLY RECOMMENDED: Use overloaded alternative")> Public Function RemoveGroupAuthorization(ByVal GroupID As Integer, ByVal ServerGroupID As Integer) As Object
                RemoveGroupAuthorization(GroupID, ServerGroupID, False, False)
                Return Nothing
            End Function

            Public Sub RemoveGroupAuthorization(ByVal GroupID As Integer, ByVal ServerGroupID As Integer, IsDevRule As Boolean, IsDenyRule As Boolean)

                'Fill the list of authorized users
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn
                If _DBVersion.CompareTo(MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND IsNull(ID_ServerGroup, 0) = @IDServerGroup AND DevelopmentTeamMember = @IsDevRule AND IsDenyRule = @IsDenyRule"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                    MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = ServerGroupID
                    MyCmd.Parameters.Add("@IsDevRule", SqlDbType.Bit).Value = IsDevRule
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = IsDenyRule
                Else
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                End If

                Dim Result As Integer
                Try
                    MyConn.Open()
                    Result = MyCmd.ExecuteNonQuery()
                Finally
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

                'Internes Objekt aktualisieren
                _ReloadData = True

                'Return Result
            End Sub


            Public Sub RemoveGroupAuthorization(ByVal AuthorizationID As Integer)

                'Fill the list of authorized users
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID = @ID"
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = AuthorizationID

                Dim Result As Integer
                Try
                    MyConn.Open()
                    Result = MyCmd.ExecuteNonQuery()
                Finally
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

                'Internes Objekt aktualisieren
                _ReloadData = True
            End Sub

            ' TODO: change to sub
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function RemoveUserAuthorization(ByVal UserID As Integer, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False) As Object
                Return RemoveUserAuthorization(CLng(UserID), ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled)
            End Function
            ' TODO: change to sub
            <Obsolete("STRONGLY RECOMMENDED: Use overloaded alternative")> Public Function RemoveUserAuthorization(ByVal UserID As Long, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False) As Object
                RemoveUserAuthorization(UserID, ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled, False)
                Return Nothing
            End Function

            Public Sub RemoveUserAuthorization(ByVal UserID As Long, ByVal ServerGroupID As Integer, ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean, IsDenyRule As Boolean)

                'Fill the list of authorized users
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn
                If _DBVersion.CompareTo(MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND IsNull(ID_ServerGroup, 0) = @IDServerGroup AND DevelopmentTeamMember = @DevelopmentTeamMember AND IsDenyRule = @IsDenyRule"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = UserID
                    MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = ServerGroupID
                    MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = IsDenyRule
                Else
                    MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND DevelopmentTeamMember = @DevelopmentTeamMember"
                    MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                    MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = UserID
                    MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
                End If

                Dim Result As Integer
                Try
                    MyConn.Open()
                    Result = MyCmd.ExecuteNonQuery()
                Finally
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

                'Internes Objekt aktualisieren
                _ReloadData = True

                'Return Result

            End Sub

            Public Sub RemoveUserAuthorization(ByVal AuthorizationID As Integer)

                'Fill the list of authorized users
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID = @ID"
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = AuthorizationID

                Dim Result As Integer
                Try
                    MyConn.Open()
                    Result = MyCmd.ExecuteNonQuery()
                Finally
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

                'Internes Objekt aktualisieren
                _ReloadData = True
            End Sub
        End Class

    End Class

End Namespace