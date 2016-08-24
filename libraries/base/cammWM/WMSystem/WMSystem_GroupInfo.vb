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
        '''     Group information
        ''' </summary>
        Public Class GroupInformation
            Implements IGroupInformation

            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _Name As String
            Dim _Description As String
            Dim _IsSystemGroup As Boolean

            ''' <summary>
            ''' Create a new instance of group information
            ''' </summary>
            ''' <param name="GroupID"></param>
            ''' <param name="InternalName"></param>
            ''' <param name="Description"></param>
            ''' <param name="IsSystemGroup"></param>
            ''' <param name="WebManager"></param>
            Friend Sub New(ByVal GroupID As Integer, ByVal InternalName As String, ByVal Description As String, ByVal IsSystemGroup As Boolean, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem)
                _ID = GroupID
                _Name = InternalName
                _Description = Description
                _IsSystemGroup = IsSystemGroup
                _WebManager = WebManager
            End Sub

            ''' <summary>
            '''     Assign properties of a group from a table row of the system database
            ''' </summary>
            ''' <param name="dataRow">The row from the data table containing the full user data</param>
            ''' <param name="webManager">The current instance of camm Web-Manager</param>
            Friend Sub New(dataRow As DataRow, webManager As WMSystem)
                Me.New(CType(dataRow("ID"), Integer), CType(dataRow("Name"), String), Utils.Nz(dataRow("Description"), CType(Nothing, String)), CType(IIf(CType(dataRow("SystemGroup"), Integer) <> 0, True, False), Boolean), webManager)
            End Sub

            ''' <summary>
            '''     Constructor of a new group information object
            ''' </summary>
            ''' <param name="GroupID">The ID value of the group which shall be loaded</param>
            ''' <param name="WebManager">The instance of camm Web-Manager</param>
            ''' <remarks>
            '''     If the group ID doesn't exist in the database, you'll get an object but it's empty and invalid since the ID is a zero value.
            ''' </remarks>
            Public Sub New(ByVal GroupID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem)
                _WebManager = WebManager
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("Select * From gruppen Where id = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = GroupID
                Dim MyReader As SqlDataReader = Nothing
                Try
                    MyConn.Open()
                    MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    If MyReader.Read Then
                        _ID = CType(MyReader("ID"), Integer)
                        _Name = Utils.Nz(MyReader("Name"), CType(Nothing, String))
                        _Description = Utils.Nz(MyReader("Description"), CType(Nothing, String))
                        _IsSystemGroup = CType(IIf(CType(MyReader("SystemGroup"), Integer) = 0, False, True), Boolean)
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
            '''     The ID value for this group
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            <Obsolete("use Name instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property InternalName() As String 'to be subject of removal in v3.x
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property

            ''' <summary>
            '''     The title for this user group
            ''' </summary>
            ''' <value></value>
            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property

            ''' <summary>
            '''     An optional description 
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

            ''' <summary>
            '''     Indicates wether this group is a system group (e. g. Security Administration, Public Intranet, Anonymous Extranet)
            ''' </summary>
            ''' <value></value>
            Public Property IsSystemGroup() As Boolean
                Get
                    Return _IsSystemGroup
                End Get
                Set(ByVal Value As Boolean)
                    _IsSystemGroup = Value
                End Set
            End Property

            ''' <summary>
            ''' Indicate if it is a system group because it's a public or anonymous group of a server group
            ''' </summary>
            Public ReadOnly Property IsSystemGroupByServerGroup As Boolean
                Get
                    If _IsSystemGroup = False Then
                        Return False
                    Else
                        Return Not IsSystemGroupBySpecialUsersGroup
                    End If
                End Get
            End Property

            ''' <summary>
            ''' Indicate if it is a system group because it's one of the special groups for priviledged administration purposes
            ''' </summary>
            Public ReadOnly Property IsSystemGroupBySpecialUsersGroup As Boolean
                Get
                    If _IsSystemGroup = False Then
                        Return False
                    Else
                        Dim SpecialGroupsList As New ArrayList
                        Dim SpecialGroups As Array = [Enum].GetValues(GetType(CompuMaster.camm.WebManager.WMSystem.SpecialGroups))
                        For Each value As Object In SpecialGroups
                            SpecialGroupsList.Add(CType(value, Integer))
                        Next
                        Return SpecialGroupsList.Contains(Me._ID)
                    End If
                End Get
            End Property

            ''' <summary>
            '''     A list of user IDs of all members
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use MembersByRule instead")> Public ReadOnly Property MemberUserIDs() As Long()
                Get
                    Return MemberUserIDsByRule.Effective
                End Get
            End Property

            Private _MemberIDsByRule As Security.MemberIDsByRule
            ''' <summary>
            ''' Memberships of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property MemberUserIDsByRule As Security.MemberIDsByRule
                Get
                    If _MemberIDsByRule Is Nothing Then
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("", MyConn)
                        MyCmd.CommandType = CommandType.Text
                        If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                            MyCmd.CommandText = "Select benutzer.ID, memberships.IsDenyRule From memberships left Join benutzer On memberships.id_user = benutzer.id Where memberships.id_group = @ID And benutzer.id Is Not null"
                        Else
                            MyCmd.CommandText = "Select benutzer.ID, CAST(0 AS bit) AS IsDenyRule From memberships left Join benutzer On memberships.id_user = benutzer.id Where memberships.id_group = @ID And benutzer.id Is Not null"
                        End If
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim MemberUsers As DictionaryEntry() = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                        Dim AllowRuleMemberGroups As New ArrayList
                        Dim DenyRuleMemberGroups As New ArrayList
                        For MyCounter As Integer = 0 To MemberUsers.Length - 1
                            Dim usr As Long = CType(MemberUsers(MyCounter).Key, Long)
                            If Utils.Nz(MemberUsers(MyCounter).Value, False) = False Then
                                AllowRuleMemberGroups.Add(usr)
                            Else
                                DenyRuleMemberGroups.Add(usr)
                            End If
                        Next
                        _MemberIDsByRule = New Security.MemberIDsByRule(CType(AllowRuleMemberGroups.ToArray(GetType(Long)), Long()), CType(DenyRuleMemberGroups.ToArray(GetType(Long)), Long()))
                    End If
                    Return _MemberIDsByRule
                End Get
            End Property

            ''' <summary>
            '''     A list of members
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use MembersByRule instead")> Public ReadOnly Property Members() As CompuMaster.camm.WebManager.WMSystem.UserInformation()
                Get
                    Return MembersByRule().Effective
                End Get
            End Property

            Private _MembersByRule As Security.MemberItemsByRule
            ''' <summary>
            ''' Memberships of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property MembersByRule() As Security.MemberItemsByRule
                Get
                    If _MembersByRule Is Nothing Then
                        _MembersByRule = New Security.MemberItemsByRule(_WebManager, _ID)
                    End If
                    Return _MembersByRule
                End Get
            End Property

            ''' <summary>
            '''     Add a new user to the list of members
            ''' </summary>
            ''' <param name="UserInfo">The new user</param>
            ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub AddMember(ByRef UserInfo As UserInformation, Optional ByVal Notifications As WMNotifications = Nothing)
                AddMember(UserInfo, False, Notifications)
            End Sub

            ''' <summary>
            '''     Add a new user to the list of members
            ''' </summary>
            ''' <param name="UserInfo">The new user</param>
            ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
            Public Sub AddMember(ByRef UserInfo As UserInformation, IsDenyRule As Boolean, Optional ByVal Notifications As CompuMaster.camm.WebManager.Notifications.INotifications = Nothing)

                If UserInfo.IDLong = SpecialUsers.User_Anonymous OrElse UserInfo.IDLong = SpecialUsers.User_Public OrElse UserInfo.IDLong = SpecialUsers.User_UpdateProcessor OrElse UserInfo.IDLong = SpecialUsers.User_Code Then
                    Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf _ID = Nothing Then
                    Dim Message As String = "Group has to be created, first, before you can modify the list of members"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf IsDenyRule = False Then
                    Dim RequiredApplicationFlags As String() = Me.RequiredAdditionalFlags
                    Dim RequiredFlagsValidationResults As FlagValidation.FlagValidationResult() = FlagValidation.ValidateRequiredFlags(UserInfo, RequiredApplicationFlags, True)
                    If RequiredFlagsValidationResults.Length <> 0 Then
                        Throw New FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                    End If
                End If

                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("AdminPrivate_CreateMemberships", MyConn)
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = _ID
                MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserInfo.IDLong
                If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = IsDenyRule
                ElseIf IsDenyRule Then
                    Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
                End If
                Dim Result As Object = Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If IsDBNull(Result) OrElse Result Is Nothing Then
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf CType(Result, Integer) = -1 Then
                    'Success
                Else
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                End If

                If UserInfo.AccountAuthorizationsAlreadySet = False Then
                    'send e-mail when first membership has been set up
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

                UserInfo.ResetMembershipsCache()
                ResetMembershipsCache()
            End Sub

            ''' <summary>
            '''     Add a new user to the list of members
            ''' </summary>
            ''' <param name="UserID">The ID value of the new user</param>
            ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub AddMember(ByVal UserID As Integer, Optional ByVal Notifications As WMNotifications = Nothing)
                AddMember(CLng(UserID), Notifications)
            End Sub

            ''' <summary>
            '''     Add a new user to the list of members
            ''' </summary>
            ''' <param name="UserID">The ID value of the new user</param>
            ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub AddMember(ByVal UserID As Long, Optional ByVal Notifications As WMNotifications = Nothing)
                AddMember(UserID, False, Notifications)
            End Sub

            ''' <summary>
            '''     Add a new user to the list of members
            ''' </summary>
            ''' <param name="UserID">The ID value of the new user</param>
            ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
            Public Sub AddMember(ByVal UserID As Long, IsDenyRule As Boolean, Optional ByVal Notifications As CompuMaster.camm.WebManager.Notifications.INotifications = Nothing)
                If UserID = SpecialUsers.User_Anonymous OrElse UserID = SpecialUsers.User_Public OrElse UserID = SpecialUsers.User_Code OrElse UserID = SpecialUsers.User_UpdateProcessor Then
                    Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf _ID = Nothing Then 'Check here again before spending time on getting complete user infos when it's clear that our main method will fail
                    Dim Message As String = "Group has to be created, first, before you can modify the list of members"
                    _WebManager.Log.RuntimeException(Message)
                End If
                AddMember(New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, _WebManager), IsDenyRule, Notifications)
            End Sub

            ''' <summary>
            ''' Is the given user a member of the current group?
            ''' </summary>
            ''' <param name="userID">A user ID which shall be tested for membership</param>
            ''' <returns>True if it is a member, otherwise False</returns>
            Public Function HasMember(ByVal userID As Long) As Boolean
                For MyCounter As Integer = 0 To MemberUserIDsByRule.Effective.Length - 1
                    If MemberUserIDsByRule.Effective(MyCounter) = userID Then
                        Return True
                    End If
                Next
                Return False
            End Function

            ''' <summary>
            ''' Is the given user a member of the current group?
            ''' </summary>
            ''' <param name="userLoginName">A loginname which shall be tested for membership</param>
            ''' <returns>True if it is a member, otherwise False</returns>
            Public Function HasMember(ByVal userLoginName As String) As Boolean
                Dim userID As Long
                userID = CType(Me._WebManager.System_GetUserID(userLoginName, True), Long)
                If userID = -1 Then
                    Throw New Exception("User """ & userLoginName & """ doesn't exist")
                End If
                Return HasMember(userID)
            End Function

            ''' <summary>
            '''     Remove a user from the list of members
            ''' </summary>
            ''' <param name="UserID">The ID value of the user</param>
            <Obsolete("UserID should by of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub RemoveMember(ByVal UserID As Integer)
                RemoveMember(CLng(UserID))
            End Sub

            ''' <summary>
            '''     Remove a user from the list of members
            ''' </summary>
            ''' <param name="UserID">The ID value of the user</param>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub RemoveMember(ByVal UserID As Long)
                RemoveMember(UserID, False)
            End Sub

            ''' <summary>
            '''     Remove a user from the list of members
            ''' </summary>
            ''' <param name="userInfo">The ID value of the user</param>
            Public Sub RemoveMember(ByVal userInfo As UserInformation, isDenyRule As Boolean)
                RemoveMember(userInfo.IDLong, isDenyRule)
                userInfo.ResetMembershipsCache()
            End Sub

            ''' <summary>
            '''     Remove a user from the list of members
            ''' </summary>
            ''' <param name="userID">The ID value of the user</param>
            Public Sub RemoveMember(ByVal userID As Long, isDenyRule As Boolean)


                If userID = SpecialUsers.User_Anonymous OrElse userID = SpecialUsers.User_Public OrElse userID = SpecialUsers.User_Code OrElse userID = SpecialUsers.User_UpdateProcessor Then
                    Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf _ID = Nothing Then
                    Dim Message As String = "Group has to be created, first, before you can modify the list of members"
                    _WebManager.Log.RuntimeException(Message)
                End If

                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As SqlCommand
                If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd = New SqlCommand("AdminPrivate_DeleteMemberships", MyConn)
                    MyCmd.CommandType = CommandType.StoredProcedure
                    MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = _ID
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
                ElseIf Setup.DatabaseUtils.Version(_WebManager, True).Build >= 176 Then  'Newer - build 176 introduced SP [AdminPrivate_DeleteMemberships]
                    If isDenyRule Then Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
                    MyCmd = New SqlCommand("AdminPrivate_DeleteMemberships", MyConn)
                    MyCmd.CommandType = CommandType.StoredProcedure
                    MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = _ID
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                Else
                    If isDenyRule Then Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
                    MyCmd = New SqlCommand("DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID", MyConn)
                    MyCmd.CommandType = CommandType.Text
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = _ID
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

                ResetMembershipsCache()

            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDev/isDenyRule parameter")> Public Sub AddAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer)
                Me.AddAuthorization(securityObjectID, serverGroupID, False, False)
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="isDeveloperAuthorization">Group members will be considered for development access</param>
            ''' <param name="isDenyRule">True for a deny rule, false for an allow rule (default)</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
                If isDenyRule = False Then
                    Dim FirstUserIDWithMissingFlags As Long = SecurityObjectInformation.ValidateRequiredFlagsOnAllRelatedUsers(SecurityObjectInformation.RequiredAdditionalFlags(securityObjectID, Me._WebManager), securityObjectID, Me._ID, isDenyRule, Me._WebManager)
                    If FirstUserIDWithMissingFlags <> 0L Then
                        Dim FirstError As New FlagValidation.FlagValidationResult(FirstUserIDWithMissingFlags, FlagValidation.FlagValidationResultCode.Missing)
                        Throw New FlagValidation.RequiredFlagException(FirstError)
                    End If
                End If
                CompuMaster.camm.WebManager.DataLayer.Current.AddGroupAuthorization(Me._WebManager, securityObjectID, Me._ID, serverGroupID, isDeveloperAuthorization, isDenyRule)
                'Requery the list of authorization next time it's required
                _AuthorizationsByRule = Nothing
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectInfo">The security object ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="isDeveloperAuthorization">Group members will be considered for development access</param>
            ''' <param name="isDenyRule">True for a deny rule, false for an allow rule (default)</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorization(ByVal securityObjectInfo As SecurityObjectInformation, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
                If isDenyRule = False Then
                    Dim FirstUserIDWithMissingFlags As Long = SecurityObjectInformation.ValidateRequiredFlagsOnAllRelatedUsers(securityObjectInfo.RequiredAdditionalFlags, securityObjectInfo.ID, Me._ID, isDenyRule, Me._WebManager)
                    If FirstUserIDWithMissingFlags <> 0L Then
                        Dim FirstError As New FlagValidation.FlagValidationResult(FirstUserIDWithMissingFlags, FlagValidation.FlagValidationResultCode.Missing)
                        Throw New FlagValidation.RequiredFlagException(FirstError)
                    End If
                End If
                AddAuthorization(securityObjectInfo.ID, serverGroupID, isDeveloperAuthorization, isDenyRule)
                securityObjectInfo.ResetAuthorizationsCacheForGroups()
            End Sub

            ''' <summary>
            '''     Add an authorization to a security object for all server groups (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Use overloaded method with parameter serverGroupID"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub AddAuthorization(ByVal securityObjectID As Integer)
                Me.AddAuthorization(securityObjectID, 0)
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID the user shall not be authorized for any more</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDev/isDenyRule parameter")> Public Sub RemoveAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer)
                RemoveAuthorization(securityObjectID, serverGroupID, False, False)
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID the user shall not be authorized for any more</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <param name="isDeveloperAuthorization">Group members will be considered for development access</param>
            ''' <param name="isDenyRule">True for a deny rule, false for an allow rule (default)</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveGroupAuthorization(Me._WebManager, securityObjectID, Me._ID, serverGroupID, isDeveloperAuthorization, isDenyRule)
                _AuthorizationsByRule = Nothing
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectInfo">The security object the user shall not be authorized for any more</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <param name="isDeveloperAuthorization">Group members will be considered for development access</param>
            ''' <param name="isDenyRule">True for a deny rule, false for an allow rule (default)</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorization(ByVal securityObjectInfo As SecurityObjectInformation, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
                RemoveAuthorization(securityObjectInfo.ID, serverGroupID, isDeveloperAuthorization, isDenyRule)
                securityObjectInfo.ResetAuthorizationsCacheForGroups()
            End Sub

            ''' <summary>
            ''' Reset cached/calculated authorizations
            ''' </summary>
            Friend Sub ResetMembershipsCache()
                _MemberIDsByRule = Nothing
                _MembersByRule = Nothing
            End Sub

            ''' <summary>
            ''' Reset cached/calculated authorizations
            ''' </summary>
            Friend Sub ResetAuthorizationsCache()
                _AuthorizationsByRule = Nothing
            End Sub

            ''' <summary>
            '''     Remove an authorization with assignment to all server groups (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID the user shall not be authorized for any more</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Use overloaded method with parameter serverGroupID"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub RemoveAuthorization(ByVal securityObjectID As Integer)
                Me.RemoveAuthorization(securityObjectID, 0)
            End Sub

            ''' <summary>
            '''     The authorizations list where the group is authorized for
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use MembersByRule instead")> Public ReadOnly Property Authorizations() As SecurityObjectAuthorizationForGroup()
                Get
                    Return AuthorizationsByRule.EffectiveByDenyRuleStandard()
                End Get
            End Property

            Private _AuthorizationsByRule As Security.GroupAuthorizationItemsByRuleForGroups
            ''' <summary>
            ''' Authorizations of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property AuthorizationsByRule As Security.GroupAuthorizationItemsByRuleForGroups
                Get
                    If _AuthorizationsByRule Is Nothing OrElse _AuthorizationsByRule.CurrentContextServerGroupIDInitialized <> (_WebManager.CurrentServerInfo IsNot Nothing) Then 'no cache object available OR srv. group initialization context changed
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("", MyConn)
                        MyCmd.CommandType = CommandType.Text
                        If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                            MyCmd.CommandText = "select applicationsrightsbygroup.ID as AuthorizationID, applicationsrightsbygroup.ID_Application as AuthorizationSecurityObjectID, applicationsrightsbygroup.ID_GroupOrPerson as AuthorizationGroupID, applicationsrightsbygroup.ID_ServerGroup as AuthorizationServerGroupID, applicationsrightsbygroup.ReleasedOn as AuthorizationReleasedOn, applicationsrightsbygroup.ReleasedBy as AuthorizationReleasedBy, applicationsrightsbygroup.DevelopmentTeamMember As AuthorizationIsDeveloper, applicationsrightsbygroup.IsDenyRule, Applications_CurrentAndInactiveOnes.* from applicationsrightsbygroup inner join Applications_CurrentAndInactiveOnes on applicationsrightsbygroup.id_application = Applications_CurrentAndInactiveOnes.id where applicationsrightsbygroup.id_grouporperson = @ID and Applications_CurrentAndInactiveOnes.id is not null"
                        Else
                            MyCmd.CommandText = "select applicationsrightsbygroup.ID as AuthorizationID, applicationsrightsbygroup.ID_Application as AuthorizationSecurityObjectID, applicationsrightsbygroup.ID_GroupOrPerson as AuthorizationGroupID, NULL as AuthorizationServerGroupID, applicationsrightsbygroup.ReleasedOn as AuthorizationReleasedOn, applicationsrightsbygroup.ReleasedBy as AuthorizationReleasedBy, CAST(0 As bit) As AuthorizationIsDeveloper, CAST(0 As bit) As IsDenyRule, Applications_CurrentAndInactiveOnes.* from applicationsrightsbygroup inner join Applications_CurrentAndInactiveOnes on applicationsrightsbygroup.id_application = Applications_CurrentAndInactiveOnes.id where applicationsrightsbygroup.id_grouporperson = @ID and Applications_CurrentAndInactiveOnes.id is not null"
                        End If
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim SecObjects As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "SecurityObjects")
                        Dim AllowRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForGroup)
                        Dim AllowRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForGroup)
                        Dim DenyRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForGroup)
                        Dim DenyRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForGroup)
                        For MyCounter As Integer = 0 To SecObjects.Rows.Count - 1
                            Dim MyDataRow As DataRow = SecObjects.Rows(MyCounter)
                            Dim NavInfo As New Security.NavigationInformation( _
                                        0, _
                                        Nothing, _
                                        Utils.Nz(MyDataRow("Level1Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level2Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level3Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level4Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level5Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level6Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level1TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level2TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level3TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level4TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level5TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level6TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("NavURL"), String.Empty), _
                                        Utils.Nz(MyDataRow("NavFrame"), String.Empty), _
                                        Utils.Nz(MyDataRow("NavTooltipText"), String.Empty), _
                                        Utils.Nz(MyDataRow("AddLanguageID2URL"), False), _
                                        Utils.Nz(MyDataRow("LanguageID"), 0), _
                                        Utils.Nz(MyDataRow("LocationID"), 0), _
                                        Utils.Nz(MyDataRow("Sort"), 0), _
                                        Utils.Nz(MyDataRow("IsNew"), False), _
                                        Utils.Nz(MyDataRow("IsUpdated"), False), _
                                        Utils.Nz(MyDataRow("ResetIsNewUpdatedStatusOn"), DateTime.MinValue), _
                                        Utils.Nz(MyDataRow("OnMouseOver"), String.Empty), _
                                        Utils.Nz(MyDataRow("OnMouseOut"), String.Empty), _
                                        Utils.Nz(MyDataRow("OnClick"), String.Empty))
                            Dim secObjInfo As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(CType(MyDataRow("ID"), Integer), CType(MyDataRow("Title"), String), Utils.Nz(MyDataRow("TitleAdminArea"), CType(Nothing, String)), Utils.Nz(MyDataRow("Remarks"), CType(Nothing, String)), CType(MyDataRow("ModifiedBy"), Long), Utils.Nz(MyDataRow("ModifiedOn"), CType(Nothing, Date)), CType(MyDataRow("ReleasedBy"), Long), Utils.Nz(MyDataRow("ReleasedOn"), CType(Nothing, Date)), Utils.Nz(MyDataRow("AppDisabled"), False), Utils.Nz(MyDataRow("AppDeleted"), False), Utils.Nz(MyDataRow("AuthsAsAppID"), 0), Utils.Nz(MyDataRow("SystemAppType"), 0), Utils.Nz(Utils.CellValueIfColumnExists(MyDataRow, "RequiredUserProfileFlags"), ""), Utils.Nz(Utils.CellValueIfColumnExists(MyDataRow, "RequiredUserProfileFlagsRemarks"), ""), NavInfo, _WebManager)
                            Dim secObjAuth As New SecurityObjectAuthorizationForGroup(_WebManager, CType(MyDataRow("AuthorizationID"), Integer), CType(MyDataRow("AuthorizationGroupID"), Integer), CType(MyDataRow("AuthorizationSecurityObjectID"), Integer), Utils.Nz(MyDataRow("AuthorizationServerGroupID"), 0), Me, secObjInfo, Nothing, Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False), Utils.Nz(MyDataRow("IsDenyRule"), False), CType(MyDataRow("AuthorizationReleasedOn"), DateTime), CType(MyDataRow("AuthorizationReleasedBy"), Integer), False)
                            If Utils.Nz(MyDataRow("IsDenyRule"), False) = False Then
                                If Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False) = True Then
                                    AllowRuleAuthsIsDev.Add(secObjAuth)
                                Else
                                    AllowRuleAuthsNonDev.Add(secObjAuth)
                                End If
                            Else
                                If Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False) = True Then
                                    DenyRuleAuthsIsDev.Add(secObjAuth)
                                Else
                                    DenyRuleAuthsNonDev.Add(secObjAuth)
                                End If
                            End If
                        Next
                        If _WebManager.CurrentServerInfo Is Nothing Then
                            _AuthorizationsByRule = New Security.GroupAuthorizationItemsByRuleForGroups( _
                                Me._ID, _
                                0, _
                                AllowRuleAuthsNonDev.ToArray(), _
                                AllowRuleAuthsIsDev.ToArray(), _
                                DenyRuleAuthsNonDev.ToArray(), _
                                DenyRuleAuthsIsDev.ToArray(), _
                                Me._WebManager)
                        Else
                            _AuthorizationsByRule = New Security.GroupAuthorizationItemsByRuleForGroups( _
                                _WebManager.CurrentServerInfo.ParentServerGroupID, _
                                Me._ID, _
                                0, _
                                AllowRuleAuthsNonDev.ToArray(), _
                                AllowRuleAuthsIsDev.ToArray(), _
                                DenyRuleAuthsNonDev.ToArray(), _
                                DenyRuleAuthsIsDev.ToArray(), _
                                Me._WebManager)
                        End If
                    End If
                    Return _AuthorizationsByRule
                End Get
            End Property

            ''' <summary>
            ''' Based on current authorization of this group and their additional flags requirements, every member user account must provide the requested flag data
            ''' </summary>
            ''' <returns>Array of strings representing required flag names (with type information)</returns>
            Public Function RequiredAdditionalFlags() As String()
                Return RequiredAdditionalFlags(Me.ID, Me._WebManager)
            End Function

            Friend Shared Function RequiredAdditionalFlags(groupID As Integer, webManager As WMSystem) As String()
                Dim Sql As String
                If webManager.System_DBVersion_Ex(True).CompareTo(MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    Sql = "        SELECT Applications_CurrentAndInactiveOnes.RequiredUserProfileFlags" & vbNewLine & _
                            "        FROM [dbo].[ApplicationsRightsByGroup] " & vbNewLine & _
                            "            INNER JOIN dbo.Applications_CurrentAndInactiveOnes " & vbNewLine & _
                            "                ON Applications_CurrentAndInactiveOnes.ID = [dbo].[ApplicationsRightsByGroup].ID_Application" & vbNewLine & _
                            "        WHERE [dbo].[ApplicationsRightsByGroup].isdenyrule = 0" & vbNewLine & _
                            "            AND [dbo].[ApplicationsRightsByGroup].ID_GroupOrPerson = @GroupID" & vbNewLine & _
                            "            AND Applications_CurrentAndInactiveOnes.RequiredUserProfileFlags IS NOT NULL"
                Else
                    Sql = "        SELECT Applications_CurrentAndInactiveOnes.RequiredUserProfileFlags" & vbNewLine & _
                            "        FROM [dbo].[ApplicationsRightsByGroup] " & vbNewLine & _
                            "            INNER JOIN dbo.Applications_CurrentAndInactiveOnes " & vbNewLine & _
                            "                ON Applications_CurrentAndInactiveOnes.ID = [dbo].[ApplicationsRightsByGroup].ID_Application" & vbNewLine & _
                            "        WHERE [dbo].[ApplicationsRightsByGroup].ID_GroupOrPerson = @GroupID" & vbNewLine & _
                            "            AND Applications_CurrentAndInactiveOnes.RequiredUserProfileFlags IS NOT NULL"
                End If
                Dim command As New SqlCommand(Sql, New SqlConnection(webManager.ConnectionString))
                command.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
                Dim RequiredFlagsMultiCellData As ArrayList = Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(command, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection) '1 row for each app requiring flags - still must be joined into 1 string array
                Dim Result As New ArrayList
                For MyCounter As Integer = 0 To RequiredFlagsMultiCellData.Count - 1
                    Dim RequiredFlagFieldOf1App As String = CType(RequiredFlagsMultiCellData(MyCounter), String)
                    Dim RequiredFlagFieldOf1AppSplitted As String() = RequiredFlagFieldOf1App.Split(","c)
                    For MyInnerCounter As Integer = 0 To RequiredFlagFieldOf1AppSplitted.Length - 1
                        If Result.Contains(RequiredFlagFieldOf1AppSplitted(MyInnerCounter)) = False Then
                            Result.Add(RequiredFlagFieldOf1AppSplitted(MyInnerCounter))
                        End If
                    Next
                Next
                Return CType(Result.ToArray(GetType(String)), String())
            End Function

#Region "Modification/Release information"
            Dim _ModifiedBy_UserID As Long
            Dim _ModifiedBy_UserInfo As UserInformation
            Dim _ModifiedOn As DateTime
            Dim _ReleasedBy_UserID As Long
            Dim _ReleasedBy_UserInfo As UserInformation
            Dim _ReleasedOn As DateTime

            ''' <summary>
            '''     Last modification by this user
            ''' </summary>
            Public Property ModifiedBy_UserID() As Long
                Get
                    Return CType(_ModifiedBy_UserID, Long)
                End Get
                Set(ByVal Value As Long)
                    _ModifiedBy_UserID = Value
                    _ModifiedBy_UserInfo = Nothing
                End Set
            End Property
            ''' <summary>
            '''     Last modification by this user
            ''' </summary>
            Public Property ModifiedBy_UserInfo() As UserInformation
                Get
                    If _ModifiedBy_UserInfo Is Nothing Then
                        _ModifiedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ModifiedBy_UserID, _WebManager, True)
                    End If
                    Return _ModifiedBy_UserInfo
                End Get
                Set(ByVal Value As UserInformation)
                    _ModifiedBy_UserInfo = Value
                    _ModifiedBy_UserID = _ModifiedBy_UserInfo.IDLong
                End Set
            End Property
            ''' <summary>
            '''     The date and time of the last modification
            ''' </summary>
            Public Property ModifiedOn() As DateTime
                Get
                    Return _ModifiedOn
                End Get
                Set(ByVal Value As DateTime)
                    _ModifiedOn = Value
                End Set
            End Property
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

    End Class

End Namespace