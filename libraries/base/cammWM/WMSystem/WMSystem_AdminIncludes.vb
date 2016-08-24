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

    'PLEASE NOTE REGARDING THESE CLASSES: 
    'AdminIncludes for cammWM.Admin

    Partial Public Class WMSystem

        '/ Cache objects
        Private _AdminSystem_CachedUserInfo As UserInformation
        Private _AdminSystem_CachedAccessLevelInfo As AccessLevelInformation
        Private _AdminSystem_CachedServerInfo As ServerInformation
        Private _AdminSystem_CachedServerGroupInfo As ServerGroupInformation
        Private _AdminSystem_CachedLanguageInfo As LanguageInformation

        '/ Create links to navigation preview URLs for each available server group of a user
        <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_WriteNavPreviewNav_TR2TR_2Cols(ByVal UserID As Integer, ByVal UserFullName As String, Optional ByVal WriteToCurrentContext As Boolean = False) As String
            Return System_WriteNavPreviewNav_TR2TR_2Cols(CLng(UserID), UserFullName, WriteToCurrentContext)
        End Function
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_WriteNavPreviewNav_TR2TR_2Cols(ByVal UserID As Long, ByVal UserFullName As String, Optional ByVal WriteToCurrentContext As Boolean = False) As String

            Dim Result As String
            Result = "<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & UserFullName & ":</b></FONT></P></TD></TR><TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">"

            Dim UserAccessableServerGroups As ServerGroupInformation()
            Dim AvailableLanguages As LanguageInformation()
            If UserID > 0 Then
                Dim UserInfo As UserInformation
                UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, Me)
                UserAccessableServerGroups = UserInfo.AccessLevel.ServerGroups
            ElseIf UserID = SpecialUsers.User_Public Then
                UserAccessableServerGroups = System_GetServerGroupsInfo()
            ElseIf UserID = SpecialUsers.User_Anonymous Then
                UserAccessableServerGroups = System_GetServerGroupsInfo()
            Else
                Throw New Exception("Invalid user information requested")
            End If
            AvailableLanguages = System_GetLanguagesInfo()

            For Each MySGInfo As ServerGroupInformation In UserAccessableServerGroups
                For Each MyLangInfo As LanguageInformation In AvailableLanguages
                    Result &= "<a href=""#"" onClick=""OpenNavDemo(" & MyLangInfo.ID & ", '" & System.Web.HttpUtility.UrlEncode(MySGInfo.MasterServer.IPAddressOrHostHeader) & "', '" & UserID & "');"">" & _
                        MySGInfo.Title & _
                        ", " & _
                        MyLangInfo.LanguageName_English & _
                        "</a><br>"
                Next
            Next

            Result &= "</FONT></TD></TR>"

            If WriteToCurrentContext Then
                HttpContext.Current.Response.Write(Result)
                Return Nothing
            Else
                Return Result
            End If

        End Function

        '/ Admin area info methods
        <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserLoginName(ByVal UserID As Integer) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.ID <> UserID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(UserID), Me)
            End If
            Return _AdminSystem_CachedUserInfo.LoginName
        End Function
        <Obsolete("Use UserInformation.LoginName instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserLoginName(ByVal UserID As Long) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.IDLong <> UserID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, Me)
            End If
            Return _AdminSystem_CachedUserInfo.LoginName
        End Function
        Private Function UserLoginName(ByVal userID As Long) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.IDLong <> userID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(userID, Me)
            End If
            Return _AdminSystem_CachedUserInfo.LoginName
        End Function
        <Obsolete("Use UserInformation.FullName instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserFullName(ByVal UserID As Integer) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.ID <> UserID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(UserID), Me)
            End If
            Return _AdminSystem_CachedUserInfo.FullName
        End Function
        <Obsolete("Use UserInformation.FullName instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserFullName(ByVal UserID As Long) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.IDLong <> UserID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, Me)
            End If
            Return _AdminSystem_CachedUserInfo.FullName
        End Function
        <Obsolete("Use UserInformation.EMailAddress instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserEMailAddress(ByVal UserID As Integer) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.ID <> UserID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(UserID), Me)
            End If
            Return _AdminSystem_CachedUserInfo.EMailAddress
        End Function
        <Obsolete("Use UserInformation.EMailAddress instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetUserEMailAddress(ByVal UserID As Long) As String
            If _AdminSystem_CachedUserInfo Is Nothing OrElse _AdminSystem_CachedUserInfo.IDLong <> UserID Then
                _AdminSystem_CachedUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, Me)
            End If
            Return _AdminSystem_CachedUserInfo.EMailAddress
        End Function
        <Obsolete("Use ServerInformation.Description instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetServerTitle(ByVal ServerID As Integer) As String
            If _AdminSystem_CachedServerInfo Is Nothing OrElse _AdminSystem_CachedServerInfo.ID <> ServerID Then
                _AdminSystem_CachedServerInfo = New ServerInformation(ServerID, Me)
            End If
            Return _AdminSystem_CachedServerInfo.Description
        End Function
        <Obsolete("Use ServerGroupInformation.Title instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetServerGroupTitle(ByVal ServerGroupID As Integer) As String
            If _AdminSystem_CachedServerGroupInfo Is Nothing OrElse _AdminSystem_CachedServerGroupInfo.ID <> ServerGroupID Then
                _AdminSystem_CachedServerGroupInfo = New ServerGroupInformation(ServerGroupID, Me)
            End If
            Return _AdminSystem_CachedServerGroupInfo.Title
        End Function
        <Obsolete("Use LanguageInformation.Abbreviation instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetLanguageAbbrev(ByVal LangID As Integer) As String
            If _AdminSystem_CachedLanguageInfo Is Nothing OrElse _AdminSystem_CachedLanguageInfo.ID <> LangID Then
                _AdminSystem_CachedLanguageInfo = New LanguageInformation(LangID, Me)
            End If
            Return _AdminSystem_CachedLanguageInfo.Abbreviation
        End Function
        <Obsolete("Use AccessLevelInformation.Title instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetAccessLevelTitle(ByVal AccessLevelID As Integer) As String
            If _AdminSystem_CachedAccessLevelInfo Is Nothing OrElse _AdminSystem_CachedAccessLevelInfo.ID <> AccessLevelID Then
                _AdminSystem_CachedAccessLevelInfo = New AccessLevelInformation(AccessLevelID, Me)
            End If
            Return _AdminSystem_CachedAccessLevelInfo.Title
        End Function
        <Obsolete("Use AccessLevelInformation.Remarks instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_GetAccessLevelRemarks(ByVal AccessLevelID As Integer) As String
            If _AdminSystem_CachedAccessLevelInfo Is Nothing OrElse _AdminSystem_CachedAccessLevelInfo.ID <> AccessLevelID Then
                _AdminSystem_CachedAccessLevelInfo = New AccessLevelInformation(AccessLevelID, Me)
            End If
            Return _AdminSystem_CachedAccessLevelInfo.Remarks
        End Function

        '/ Admin area authorization info
        Private AdminPrivate_GetSubAuthorizationStatus_TableName As String
        Dim AdminPrivate_GetSubAuthorizationStatus_TablePrimID As Integer
        Dim AdminPrivate_GetSubAuthorizationStatus_UserID As Long
        Dim AdminPrivate_GetSubAuthorizationStatus_RequiredAuth As String
        Dim AdminPrivate_GetSubAuthorizationStatus_Result As Boolean

        ''' <summary>
        '''     Should be public but is not possible while it has to be overloaded with a function only differing by a long and an integer
        ''' </summary>
        ''' <param name="TableName"></param>
        ''' <param name="TablePrimID"></param>
        ''' <param name="UserID"></param>
        ''' <param name="RequiredAuth"></param>
        Public Function System_GetSubAuthorizationStatus(ByVal TableName As String, ByVal TablePrimID As Integer, ByVal UserID As Long, ByVal RequiredAuth As String) As Boolean

            If AdminPrivate_GetSubAuthorizationStatus_TableName = TableName And _
              AdminPrivate_GetSubAuthorizationStatus_TablePrimID = TablePrimID And _
              AdminPrivate_GetSubAuthorizationStatus_UserID = UserID And _
              AdminPrivate_GetSubAuthorizationStatus_RequiredAuth = RequiredAuth Then
                Return AdminPrivate_GetSubAuthorizationStatus_Result
            End If

            If System_IsSecurityMaster(TableName, UserID) = True Then
                AdminPrivate_GetSubAuthorizationStatus_Result = True
                AdminPrivate_GetSubAuthorizationStatus_TableName = TableName
                AdminPrivate_GetSubAuthorizationStatus_TablePrimID = TablePrimID
                AdminPrivate_GetSubAuthorizationStatus_UserID = UserID
                AdminPrivate_GetSubAuthorizationStatus_RequiredAuth = RequiredAuth
                Return True
            End If

            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = ConnectionString

            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT COUNT(*) FROM System_SubSecurityAdjustments Where (TablePrimaryIDValue = 0 AND UserID = " & CLng(UserID) & " AND AuthorizationType = N'SecurityMaster') OR ((TablePrimaryIDValue = 0 OR TablePrimaryIDValue = " & CLng(TablePrimID) & ") AND TableName = N'" & Replace(TableName, "'", "''") & "' AND UserID = @UserID AND AuthorizationType = N'" & Replace(RequiredAuth, "'", "''") & "')"
                    .CommandType = CommandType.Text

                    .Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(UserID)

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn

                MyRecSet = MyCmd.ExecuteReader()
                MyRecSet.Read()
                If CType(MyRecSet(0), Integer) = 0 Then
                    System_GetSubAuthorizationStatus = False
                    AdminPrivate_GetSubAuthorizationStatus_Result = False
                    AdminPrivate_GetSubAuthorizationStatus_TableName = TableName
                    AdminPrivate_GetSubAuthorizationStatus_TablePrimID = TablePrimID
                    AdminPrivate_GetSubAuthorizationStatus_UserID = UserID
                    AdminPrivate_GetSubAuthorizationStatus_RequiredAuth = RequiredAuth
                Else
                    System_GetSubAuthorizationStatus = True
                    AdminPrivate_GetSubAuthorizationStatus_Result = True
                    AdminPrivate_GetSubAuthorizationStatus_TableName = TableName
                    AdminPrivate_GetSubAuthorizationStatus_TablePrimID = TablePrimID
                    AdminPrivate_GetSubAuthorizationStatus_UserID = UserID
                    AdminPrivate_GetSubAuthorizationStatus_RequiredAuth = RequiredAuth
                End If
            Catch
                System_GetSubAuthorizationStatus = False
                AdminPrivate_GetSubAuthorizationStatus_Result = False
                AdminPrivate_GetSubAuthorizationStatus_TableName = TableName
                AdminPrivate_GetSubAuthorizationStatus_TablePrimID = TablePrimID
                AdminPrivate_GetSubAuthorizationStatus_UserID = UserID
                AdminPrivate_GetSubAuthorizationStatus_RequiredAuth = RequiredAuth
            Finally
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                    MyRecSet.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyDBConn Is Nothing Then
                    If MyDBConn.State <> ConnectionState.Closed Then
                        MyDBConn.Close()
                    End If
                    MyDBConn.Dispose()
                End If
            End Try

        End Function

        '/ User's admin status
        Dim AdminPrivate_IsSecurityMaster_TableName As String
        Dim AdminPrivate_IsSecurityMaster_UserID As Long
        Dim AdminPrivate_IsSecurityMaster_Result As Integer

        ''' <summary>
        '''     Is an user a security master?
        ''' </summary>
        ''' <param name="TableName">Either 'Groups' or 'Applications'</param>
        ''' <param name="UserID">The user ID</param>
        <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_IsSecurityMaster(ByVal TableName As String, ByVal UserID As Integer) As Boolean
            Return System_IsSecurityMaster(TableName, CLng(UserID))
        End Function

        ''' <summary>
        '''     Is an user a security master?
        ''' </summary>
        ''' <param name="TableName">Either 'Groups' or 'Applications'</param>
        ''' <param name="UserID">The user ID</param>
        Public Function System_IsSecurityMaster(ByVal TableName As String, ByVal UserID As Long) As Boolean
            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand

            If AdminPrivate_IsSecurityMaster_TableName = TableName And _
              AdminPrivate_IsSecurityMaster_UserID = UserID And _
              AdminPrivate_IsSecurityMaster_Result = 1 Then
                System_IsSecurityMaster = True
                Exit Function
            ElseIf AdminPrivate_IsSecurityMaster_TableName = TableName And _
              AdminPrivate_IsSecurityMaster_UserID = UserID And _
              AdminPrivate_IsSecurityMaster_Result = 2 Then
                System_IsSecurityMaster = False
                Exit Function
            End If

            If System_IsSuperVisor(UserID) = True Then
                System_IsSecurityMaster = True
                AdminPrivate_IsSecurityMaster_Result = 1
                AdminPrivate_IsSecurityMaster_TableName = TableName
                AdminPrivate_IsSecurityMaster_UserID = UserID
                Exit Function
            End If

            'Create connection
            MyDBConn.ConnectionString = ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd

                    .CommandText = "SELECT COUNT(*) FROM System_SubSecurityAdjustments Where (TablePrimaryIDValue = 0 AND UserID = @UserID AND AuthorizationType = N'SecurityMaster' AND TableName = @TableName)"
                    .CommandType = CommandType.Text

                    .Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(UserID)
                    .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = TableName

                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn

                Dim ReturnValue As Integer
                ReturnValue = CType(MyCmd.ExecuteScalar(), Integer)
                If ReturnValue > 0 Then
                    System_IsSecurityMaster = True
                    AdminPrivate_IsSecurityMaster_Result = 1
                    AdminPrivate_IsSecurityMaster_TableName = TableName
                    AdminPrivate_IsSecurityMaster_UserID = UserID
                Else
                    System_IsSecurityMaster = False
                    AdminPrivate_IsSecurityMaster_Result = 2
                    AdminPrivate_IsSecurityMaster_TableName = TableName
                    AdminPrivate_IsSecurityMaster_UserID = UserID
                End If
            Catch
                System_IsSecurityMaster = False
                AdminPrivate_IsSecurityMaster_Result = 2
                AdminPrivate_IsSecurityMaster_TableName = TableName
                AdminPrivate_IsSecurityMaster_UserID = UserID
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyDBConn Is Nothing Then
                    If MyDBConn.State <> ConnectionState.Closed Then
                        MyDBConn.Close()
                    End If
                    MyDBConn.Dispose()
                End If
            End Try

        End Function

        Dim AdminPrivate_IsSuperVisor_UserID As Long
        Dim AdminPrivate_IsSuperVisor_Result As Integer

        ''' <summary>
        '''     Is an user a supervisor?
        ''' </summary>
        ''' <param name="UserID"></param>
        <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_IsSuperVisor(ByVal UserID As Integer) As Boolean
            Return System_IsSuperVisor(CLng(UserID))
        End Function

        ''' <summary>
        '''     Is an user a supervisor?
        ''' </summary>
        ''' <param name="UserID"></param>
        Public Function System_IsSuperVisor(ByVal UserID As Long) As Boolean

            If AdminPrivate_IsSuperVisor_UserID = UserID And AdminPrivate_IsSuperVisor_Result = 1 Then
                Return True
            ElseIf AdminPrivate_IsSuperVisor_UserID = UserID And AdminPrivate_IsSuperVisor_Result = 2 Then
                Return False
            End If

            'Get parameter value and append parameter
            Dim MyCmd As SqlCommand
            If Setup.DatabaseUtils.Version(Me, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd = New SqlCommand("SELECT ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade WHERE ID_Group = 6 AND ID_User = @UserID", New SqlConnection(Me.ConnectionString))
            Else
                MyCmd = New SqlCommand("SELECT ID_User FROM Memberships WHERE ID_Group = 6 AND ID_User = @UserID", New SqlConnection(Me.ConnectionString))
            End If
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(UserID)
            Dim MyScalarResult As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(MyScalarResult) OrElse CType(MyScalarResult, Integer) <> UserID Then
                AdminPrivate_IsSuperVisor_Result = 2
                AdminPrivate_IsSuperVisor_UserID = UserID
                Return False
            Else
                AdminPrivate_IsSuperVisor_Result = 1
                AdminPrivate_IsSuperVisor_UserID = UserID
                Return True
            End If

        End Function

        ''' <summary>
        '''     Is an user a member of a group (by effective rule)?
        ''' </summary>
        ''' <param name="userID">The ID of the user account which shall be analyzed</param>
        ''' <param name="groupName">The name of the group where the user shall be a member</param>
        ''' <returns>True if the user is a member, otherwise false</returns>
        Public Function System_IsMember(ByVal userID As Long, ByVal groupName As String) As Boolean
            If groupName = Nothing Then
                Throw New ArgumentNullException("groupName")
            End If

            Dim MyScalarResult As Object
            Dim MyCmd As SqlCommand = Nothing
            Dim Result As Boolean

            Try
                If Setup.DatabaseUtils.Version(Me, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd = New SqlCommand("SELECT ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade INNER JOIN Gruppen ON Memberships_EffectiveRulesWithClonesNthGrade.ID_Group = Gruppen.ID WHERE Gruppen.Name = @GroupName AND ID_User= @UserID", New SqlConnection(ConnectionString))
                Else
                    MyCmd = New SqlCommand("SELECT ID_User FROM Memberships INNER JOIN Gruppen ON Memberships.ID_Group = Gruppen.ID WHERE Gruppen.Name = @GroupName AND ID_User= @UserID", New SqlConnection(ConnectionString))
                End If
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(userID)
                MyCmd.Parameters.Add("@GroupName", SqlDbType.NVarChar).Value = groupName

                'Create recordset by executing the command
                MyScalarResult = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                If IsDBNull(MyScalarResult) OrElse CType(MyScalarResult, Integer) <> userID Then
                    Result = False
                Else
                    Result = True
                End If
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
            End Try

            Return Result

        End Function

        Dim AdminPrivate_IsSecurityOperator_UserID As Long
        Dim AdminPrivate_IsSecurityOperator_Result As Integer
        ''' <summary>
        '''     Is an user a security operator which has got some administration priviledges?
        ''' </summary>
        ''' <param name="UserID"></param>
        ''' <remarks>
        '''     The difference to a security master is that an operator has no priviledges, first. And a security master has got all priviledges to do all things regarding his master role.
        ''' </remarks>
        <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function System_IsSecurityOperator(ByVal UserID As Integer) As Boolean
            Return System_IsSecurityOperator(CLng(UserID))
        End Function

        ''' <summary>
        '''     Is an user a security operator which has got some administration priviledges?
        ''' </summary>
        ''' <param name="UserID"></param>
        ''' <returns>True if the user is a security operator or a supervisor</returns>
        ''' <remarks>
        '''     <para>The difference to a security master is that an operator has no priviledges, first. And a security master has got all priviledges to do all things regarding his master role.</para>
        ''' </remarks>
        Public Function System_IsSecurityOperator(ByVal UserID As Long) As Boolean

            If AdminPrivate_IsSecurityOperator_UserID = UserID And AdminPrivate_IsSecurityOperator_Result = 1 Then
                Return True
            ElseIf AdminPrivate_IsSecurityOperator_UserID = UserID And AdminPrivate_IsSecurityOperator_Result = 2 Then
                Return False
            End If

            'Get parameter value and append parameter
            Dim MyCmd As SqlCommand
            If Setup.DatabaseUtils.Version(Me, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd = New SqlCommand("SELECT ID_User FROM Memberships_EffectiveRulesWithClonesNthGrade WHERE ID_Group IN (6, 7) AND ID_User = @UserID", New SqlConnection(Me.ConnectionString))
            Else
                MyCmd = New SqlCommand("SELECT ID_User FROM Memberships WHERE ID_Group IN (6, 7) AND ID_User = @UserID", New SqlConnection(Me.ConnectionString))
            End If
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(UserID)
            Dim MyScalarResult As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(MyScalarResult) OrElse CType(MyScalarResult, Integer) <> UserID Then
                AdminPrivate_IsSecurityOperator_Result = 2
                AdminPrivate_IsSecurityOperator_UserID = UserID
                Return False
            Else
                AdminPrivate_IsSecurityOperator_Result = 1
                AdminPrivate_IsSecurityOperator_UserID = UserID
                Return True
            End If

        End Function

        ''' <summary>
        '''     Set a new password for an user account and sends required notification messages
        ''' </summary>
        ''' <param name="userInfo">The user information object which shall get a new password</param>
        ''' <param name="newPassword">A new password</param>
        ''' <param name="notificationProvider">An instance of a notification class which handles the distribution of all required mails</param>
        <Obsolete("Use userInfo.SetPassword instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Sub System_SetUserPassword(ByVal userInfo As UserInformation, ByVal newPassword As String, Optional ByVal notificationProvider As WMNotifications = Nothing)
            userInfo.SetPassword(newPassword, CType(notificationProvider, Notifications.INotifications))
        End Sub

        ''' <summary>
        '''     Set a new password for an user account and sends required notification messages
        ''' </summary>
        ''' <param name="userInfo">The user information object which shall get a new password</param>
        ''' <param name="newPassword">A new password</param>
        ''' <param name="notificationProvider">An instance of a NotificationProvider class which handles the distribution of all required mails</param>
        <Obsolete("Use userInfo.SetPassword instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Sub System_SetUserPassword(ByVal userInfo As UserInformation, ByVal newPassword As String, ByVal notificationProvider As Notifications.INotifications)
            userInfo.SetPassword(newPassword, notificationProvider)
        End Sub

    End Class

End Namespace
