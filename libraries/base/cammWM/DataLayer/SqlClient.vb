'Copyright 2014-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
    ''' A database layer for MS SQL Server
    ''' </summary>
    Friend Class DataLayerSqlClient
        Implements IDataLayer

        Public Sub SaveLastServiceExecutionDate(webmanager As IWebManager, dbConnection As IDbConnection, triggerServiceVersion As String) Implements IDataLayer.SaveLastServiceExecutionDate
            Dim MyConn As SqlClient.SqlConnection = Nothing
            Try
                'Prepare the connection for the several SQL commands
                If dbConnection Is Nothing Then
                    'only when we created our connection ourself
                    MyConn = New SqlClient.SqlConnection(webmanager.ConnectionString)
                    MyConn.Open()
                Else
                    'use given connection
                    MyConn = CType(dbConnection, SqlClient.SqlConnection)
                End If

                Dim sql As String = "UPDATE [dbo].[System_GlobalProperties] SET ValueDateTime=GetDate(), ValueNText = @VersionString WHERE PropertyName = 'LastWebServiceExecutionDate' AND ValueNVarChar = N'camm WebManager' " & vbNewLine &
                    "IF @@ROWCOUNT = 0 " & vbNewLine &
                    "INSERT INTO [dbo].[System_GlobalProperties] (PropertyName, ValueNVarChar, ValueNText, ValueDateTime) VALUES ('LastWebServiceExecutionDate', 'camm WebManager', @VersionString, GetDate()) "
                Dim cmd As New System.Data.SqlClient.SqlCommand(sql, MyConn)
                cmd.CommandType = CommandType.Text
                cmd.Parameters.Add("@VersionString", SqlDbType.NText).Value = Utils.StringNotEmptyOrAlternativeValue(triggerServiceVersion, "0.0.0.0")
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Finally
                'Close connection if created/opened by above code
                If dbConnection Is Nothing Then
                    'executes only when we created our connection by ourself
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                End If
            End Try
        End Sub

        Public Function QueryLastServiceExecutionDate(webmanager As IWebManager, dbConnection As IDbConnection) As Date Implements IDataLayer.QueryLastServiceExecutionDate
            Dim MyConn As SqlClient.SqlConnection = Nothing
            Try
                'Prepare the connection for the several SQL commands
                If dbConnection Is Nothing Then
                    'only when we created our connection ourself
                    MyConn = New SqlClient.SqlConnection(webmanager.ConnectionString)
                    MyConn.Open()
                Else
                    'use given connection
                    MyConn = CType(dbConnection, SqlClient.SqlConnection)
                End If

                Dim cmd As System.Data.SqlClient.SqlCommand
                cmd = New SqlClient.SqlCommand("SELECT ValueDateTime FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LastWebServiceExecutionDate' AND ValueNVarChar = N'camm WebManager'", MyConn)
                Dim result As Object = Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If result Is Nothing OrElse CompuMaster.camm.WebManager.Utils.Nz(result) Is Nothing Then
                    Return Nothing
                Else
                    Return CType(result, Date)
                End If
            Finally
                'Close connection if created/opened by above code
                If dbConnection Is Nothing Then
                    'executes only when we created our connection by ourself
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                End If
            End Try
        End Function

        ''' <summary>
        ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
        ''' </summary>
        ''' <param name="securityObjectID">The security object ID</param>
        ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
        ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
        ''' <param name="isDenyRule">True for a deny rule or False for a grant access rule</param>
        ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
        ''' <remarks>This action will be done immediately without the need for saving</remarks>
        Public Sub AddUserAuthorization(webmanager As WMSystem, dbConnection As IDbConnection, ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, userInfo As WMSystem.UserInformation, userID As Long, ByVal developerAuthorization As Boolean, isDenyRule As Boolean, modifyingUserID As Long, Optional ByVal notifications As Notifications.INotifications = Nothing) Implements IDataLayer.AddUserAuthorization
            'If serverGroupID <> 0 AndAlso Setup.DatabaseUtils.Version(WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) < 0 Then 'Older
            '    Throw New NotSupportedException("MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects required")
            'End If

            If userID = Nothing Then
                Throw New Exception("User has to be created, first, before you can modify the list of authorizations")
            End If

            Dim MyConn As SqlClient.SqlConnection = Nothing
            Try
                'Prepare the connection for the several SQL commands
                If dbConnection Is Nothing Then
                    'only when we created our connection ourself
                    MyConn = New SqlClient.SqlConnection(webmanager.ConnectionString)
                    MyConn.Open()
                Else
                    'use given connection
                    MyConn = CType(dbConnection, SqlClient.SqlConnection)
                End If
                Dim MyCmd As New SqlClient.SqlCommand("AdminPrivate_CreateApplicationRightsByUser", MyConn)
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = modifyingUserID
                MyCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = securityObjectID
                MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
                MyCmd.Parameters.Add("@IsDevelopmentTeamMember", SqlDbType.Bit).Value = developerAuthorization
                Dim _DBVersion As Version = Setup.DatabaseUtils.Version(webmanager, True) 'Environment check
                If _DBVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.Parameters.Add("@ServerGroupID", SqlDbType.Int).Value = serverGroupID
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
                ElseIf isDenyRule Then
                    Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
                End If
                Dim Result As Object
                Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If IsDBNull(Result) OrElse Result Is Nothing Then
                    Throw New Exception("Authorization creation failed")
                ElseIf CType(Result, Integer) = -1 Then
                    'Success
                ElseIf CType(Result, Integer) = 0 Then
                    Throw New Exception("Authorization creation failed (invalid release-by-user)")
                ElseIf CType(Result, Integer) = 3 Then
                    Throw New Exception("Authorization creation failed (invalid application ID)")
                ElseIf CType(Result, Integer) = 2 Then
                    Throw New Exception("Authorization creation failed (special SAP application requires a flag)")
                ElseIf CType(Result, Integer) = 5 Then
                    Throw New Exception("Authorization creation failed (invalid user ID)")
                Else
                    Throw New Exception("Authorization creation failed unexpectedly")
                End If
            Finally
                'Close connection if created/opened by above code
                If dbConnection Is Nothing Then
                    'executes only when we created our connection by ourself
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                End If
            End Try

            Dim InitOfAuthorizationsDone As Boolean
            If userInfo Is Nothing Then
                InitOfAuthorizationsDone = WMSystem.UserInformation.ReadInitAuthorizationsDoneValue(MyConn, userID)
            Else
                InitOfAuthorizationsDone = userInfo.AccountAuthorizationsAlreadySet
            End If
            If InitOfAuthorizationsDone = False Then
                'send e-mail when first authorization has been set up 
                InitOfAuthorizationsDone = True 'save this value locally in this class instance
                'Check wether InitAuthorizationsDone flag has been set
                If DataLayer.Current.SetUserDetail(webmanager, Nothing, userID, "InitAuthorizationsDone", "1", True) Then
                    If userInfo Is Nothing Then userInfo = New WMSystem.UserInformation(userID, webmanager, False)
                    Try
                        If notifications Is Nothing Then
                            webmanager.Notifications.NotificationForUser_AuthorizationsSet(userInfo)
                        Else
                            notifications.NotificationForUser_AuthorizationsSet(userInfo)
                        End If
                    Catch
                    End Try
                End If
            End If
        End Sub

        Public Function SaveSecurityObject(webmanager As IWebManager, dbConnection As IDbConnection, securityObject As WMSystem.SecurityObjectInformation, modifyingUserID As Long) As Integer Implements IDataLayer.SaveSecurityObject
            Dim MyCmd As SqlClient.SqlCommand = Nothing
            Dim MyConn As SqlClient.SqlConnection = Nothing

            Try
                'Prepare the connection for the several SQL commands
                If dbConnection Is Nothing Then
                    'only when we created our connection ourself
                    MyConn = New SqlClient.SqlConnection(webmanager.ConnectionString)
                    MyConn.Open()
                Else
                    'use given connection
                    MyConn = CType(dbConnection, SqlClient.SqlConnection)
                End If

                'Create new security object if required
                If securityObject.ID = 0 Then
                    MyCmd = New SqlClient.SqlCommand("AdminPrivate_CreateApplication", MyConn)
                    MyCmd.CommandType = CommandType.StoredProcedure
                    MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = modifyingUserID
                    MyCmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = securityObject.Name
                    'Save it and recieve the new ID
                    Dim ResultOfCreation As Integer = Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), 0)
                    If ResultOfCreation = 0 Then
                        Throw New Exception("Application creation failed!")
                    Else
                        securityObject.SetIDInternal(ResultOfCreation)
                    End If
                End If

                'Update application/security object
                MyCmd = New SqlClient.SqlCommand("AdminPrivate_UpdateApp", MyConn)
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@Title", SqlDbType.VarChar).Value = securityObject.Name
                MyCmd.Parameters.Add("@TitleAdminArea", SqlDbType.NVarChar).Value = securityObject.DisplayName
                MyCmd.Parameters.Add("@Level1Title", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).Level1Title
                MyCmd.Parameters.Add("@Level2Title", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).Level2Title
                MyCmd.Parameters.Add("@Level3Title", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).Level3Title
                MyCmd.Parameters.Add("@Level4Title", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).Level4Title
                MyCmd.Parameters.Add("@Level5Title", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).Level5Title
                MyCmd.Parameters.Add("@Level6Title", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).Level6Title
                MyCmd.Parameters.Add("@Level1TitleIsHTMLCoded", SqlDbType.Bit).Value = securityObject.NavigationItems(0).Level1TitleIsHtmlCoded
                MyCmd.Parameters.Add("@Level2TitleIsHTMLCoded", SqlDbType.Bit).Value = securityObject.NavigationItems(0).Level2TitleIsHtmlCoded
                MyCmd.Parameters.Add("@Level3TitleIsHTMLCoded", SqlDbType.Bit).Value = securityObject.NavigationItems(0).Level3TitleIsHtmlCoded
                MyCmd.Parameters.Add("@Level4TitleIsHTMLCoded", SqlDbType.Bit).Value = securityObject.NavigationItems(0).Level4TitleIsHtmlCoded
                MyCmd.Parameters.Add("@Level5TitleIsHTMLCoded", SqlDbType.Bit).Value = securityObject.NavigationItems(0).Level5TitleIsHtmlCoded
                MyCmd.Parameters.Add("@Level6TitleIsHTMLCoded", SqlDbType.Bit).Value = securityObject.NavigationItems(0).Level6TitleIsHtmlCoded
                MyCmd.Parameters.Add("@NavURL", SqlDbType.VarChar).Value = securityObject.NavigationItems(0).NavUrl
                MyCmd.Parameters.Add("@NavFrame", SqlDbType.VarChar).Value = securityObject.NavigationItems(0).NavFrame
                MyCmd.Parameters.Add("@NavTooltipText", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).NavTooltipText
                MyCmd.Parameters.Add("@IsNew", SqlDbType.Bit).Value = securityObject.NavigationItems(0).IsNew
                MyCmd.Parameters.Add("@IsUpdated", SqlDbType.Bit).Value = securityObject.NavigationItems(0).IsUpdated
                MyCmd.Parameters.Add("@LocationID", SqlDbType.Int).Value = securityObject.NavigationItems(0).ServerID
                MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = securityObject.NavigationItems(0).MarketID
                MyCmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = modifyingUserID
                MyCmd.Parameters.Add("@AppDisabled", SqlDbType.Bit).Value = securityObject.Disabled
                MyCmd.Parameters.Add("@Sort", SqlDbType.Int).Value = securityObject.NavigationItems(0).Sort
                MyCmd.Parameters.Add("@ResetIsNewUpdatedStatusOn", SqlDbType.DateTime).Value = IIf(securityObject.NavigationItems(0).ResetIsNewUpdatedStatusOn = DateTime.MinValue, DBNull.Value, securityObject.NavigationItems(0).ResetIsNewUpdatedStatusOn)
                MyCmd.Parameters.Add("@OnMouseOver", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).OnMouseOver
                MyCmd.Parameters.Add("@OnMouseOut", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).OnMouseOut
                MyCmd.Parameters.Add("@OnClick", SqlDbType.NVarChar).Value = securityObject.NavigationItems(0).OnClick
                MyCmd.Parameters.Add("@AddLanguageID2URL", SqlDbType.Bit).Value = securityObject.NavigationItems(0).AddMarketIDToUrl
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = securityObject.ID
                Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)

                'Update extended fields
                MyCmd = New SqlClient.SqlCommand()
                MyCmd.Connection = MyConn
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = securityObject.ID
                MyCmd.Parameters.Add("@GeneralRemarks", SqlDbType.NVarChar).Value = securityObject.Remarks
                MyCmd.Parameters.Add("@RequiredFlags", SqlDbType.NVarChar).Value = securityObject.RequiredFlags
                If webmanager.VersionDatabase(True).Build >= 185 Then
                    MyCmd.CommandText = "Update Applications_CurrentAndInactiveOnes SET Remarks = @GeneralRemarks, RequiredUserProfileFlags = @RequiredFlags, RequiredUserProfileFlagsRemarks = @UserProfileFlagsRemarks Where ID = @ID"
                    MyCmd.Parameters.Add("@UserProfileFlagsRemarks", SqlDbType.NVarChar).Value = securityObject.RequiredFlagsRemarks
                Else
                    MyCmd.CommandText = "Update Applications_CurrentAndInactiveOnes SET Remarks = @GeneralRemarks, RequiredUserProfileFlags = @RequiredFlags Where ID = @ID"
                End If
                Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)

                'Delete security object if requested
                If securityObject.Deleted = True Then
                    'Delete application
                    Dim sqlParamsDelApp As SqlClient.SqlParameter() = {New SqlClient.SqlParameter("@ID", securityObject.ID)}
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlClient.SqlConnection(webmanager.ConnectionString), "Update dbo.Applications Set AppDeleted=1 WHERE ID=@ID", CommandType.Text, sqlParamsDelApp, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'Delete Authorizations: users
                    Dim sqlParamsDelUser As SqlClient.SqlParameter() = {New SqlClient.SqlParameter("@ID_Application", securityObject.ID)}
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlClient.SqlConnection(webmanager.ConnectionString), "DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_Application=@ID_Application", CommandType.Text, sqlParamsDelUser, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'Delete Authorizations: groups
                    Dim sqlParamsDelGroup As SqlClient.SqlParameter() = {New SqlClient.SqlParameter("@ID_Application", securityObject.ID)}
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlClient.SqlConnection(webmanager.ConnectionString), "DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_Application=@ID_Application", CommandType.Text, sqlParamsDelGroup, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'Remove any inheritions from that application
                    Dim sqlParamsDelAppInher As SqlClient.SqlParameter() = {New SqlClient.SqlParameter("@ID", securityObject.ID)}
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlClient.SqlConnection(webmanager.ConnectionString), "UPDATE [dbo].[Applications_CurrentAndInactiveOnes] SET [AuthsAsAppID]=Null WHERE [AuthsAsAppID]=@ID", CommandType.Text, sqlParamsDelAppInher, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'TODO: delete related navigation items as soon as SplittedNav feature is supported
                End If
            Finally
                'Close connection if created/opened by above code
                If dbConnection Is Nothing Then
                    'executes only when we created our connection by ourself
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                End If
            End Try

            Return securityObject.ID

        End Function

        ''' <summary>
        '''     Set a user profile setting
        ''' </summary>
        ''' <param name="webManager">A valid instance of camm Web-Manager</param>
        ''' <param name="dbConnection">An open connection which shall be used or nothing if a new one shall be created independently and on the fly</param>
        ''' <param name="userID">The ID of the user who shall receive the updated value</param>
        ''' <param name="propertyName">The key name of the flag</param>
        ''' <param name="value">The new value of the flag</param>
        ''' <param name="doNotLogSuccess">False will lead to an informational log entry in the database after the value has been saved; in case of True there won't be created a log entry</param>
        Public Function SetUserDetail(ByVal webManager As IWebManager, ByVal dbConnection As IDbConnection, ByVal userID As Long, ByVal propertyName As String, ByVal value As String, Optional ByVal doNotLogSuccess As Boolean = False) As Boolean Implements IDataLayer.SetUserDetail

            'User ID cannot be nothing
            If userID = Nothing Then
                CType(webManager, WMSystem).Log.RuntimeException(New ArgumentException("UserID has been 0", "UserID"), False, False)
            ElseIf propertyName = Nothing Then
                Throw New ArgumentNullException("propertyName")
            ElseIf propertyName.Length > 80 Then
                Throw New ArgumentException("Property names with length of more than 80 characters are not supported", "propertyName")
            End If
            value = Mid(Trim(Utils.StringNotNothingOrEmpty(value)), 1, 255)
            If value = "," Then
                Throw New Exception("Invalid value shall be saved for flag """ & propertyName & """: """ & value & """")
            ElseIf value.Split(","c).Length >= 2 AndAlso value.Split(","c)(0) = value.Split(","c)(1) Then
                Throw New Exception("Invalid value shall be saved for flag """ & propertyName & """: """ & value & """")
            End If

            'Never change virtual system users
            If WMSystem.IsSystemUser(userID) Then
                Throw New Exception("Can't set user details for system users")
            End If

            'Get parameter value and append parameter
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.CommandText = "Public_SetUserDetailData"
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@IDUser", SqlDbType.Int).Value = userID
            MyCmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Trim(propertyName)
            If value = Nothing Then
                MyCmd.Parameters.Add("@Value", SqlDbType.NVarChar).Value = DBNull.Value
            Else
                MyCmd.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value
            End If
            MyCmd.Parameters.Add("@DoNotLogSuccess", SqlDbType.Bit).Value = doNotLogSuccess

            Try
                If dbConnection Is Nothing Then
                    'only when we created our connection ourself
                    MyCmd.Connection = New SqlClient.SqlConnection(webManager.ConnectionString)
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Else
                    'use given connection
                    MyCmd.Connection = CType(dbConnection, SqlClient.SqlConnection)
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                End If
                SetUserDetail = True
            Catch
                SetUserDetail = False
            End Try

        End Function

        ''' <summary>
        '''     Reads a single user detail value from the database
        ''' </summary>
        ''' <param name="webManager">A reference to a camm Web-Manager instance</param>
        ''' <param name="userID">The user ID</param>
        ''' <param name="propertyName">The requested property name</param>
        ''' <returns>The resulting value as a String</returns>
        Public Function GetUserDetail(ByVal webManager As IWebManager, ByVal userID As Long, ByVal propertyName As String) As String Implements IDataLayer.GetUserDetail
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(webManager.ConnectionString)
            MyCmd.CommandText = "Public_GetUserDetailData"
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@IDUser", SqlDbType.Int).Value = userID
            MyCmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Trim(propertyName)
            Return Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
        End Function
        ''' <summary>
        '''     Get a string with all logon servers for a user 
        ''' </summary>
        ''' <param name="webManager">An instance of camm Web-Manager</param>
        ''' <param name="userID">A user ID</param>
        ''' <returns>A string with all relative server groups; every server group is placed in a new line.</returns>
        ''' <remarks>
        '''     If there is only 1 server group available, the returned string contains only the simply URL of the master server of this server group.
        '''     Are there 2 or more server groups available then each URL of the corresponding master server is followed by the server group title in parenthesis.
        ''' </remarks>
        Public Function GetUserLogonServers(ByVal webManager As IWebManager, ByVal userID As Long) As String Implements IDataLayer.GetUserLogonServers
            Dim MyUserID As Long = CType(userID, Long)
            Dim MyDBConn As New SqlClient.SqlConnection
            Dim MyRecSet As SqlClient.SqlDataReader = Nothing
            Dim MyCmd As New SqlClient.SqlCommand
            Dim MyResult As String = Nothing
            Dim AllServersAreDisabled As Boolean
            Dim CurRecordIsRead As Boolean

            'Create connection
            MyDBConn.ConnectionString = webManager.ConnectionString
            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd
                    .CommandText = "select dbo.System_ServerGroups.ServerGroup, dbo.System_Servers.ServerProtocol, dbo.System_Servers.ServerName, dbo.System_Servers.ServerPort from (((dbo.System_AccessLevels inner join dbo.Benutzer on dbo.System_AccessLevels.ID = dbo.Benutzer.AccountAccessability) inner join dbo.System_ServerGroupsAndTheirUserAccessLevels on dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel) inner join dbo.System_ServerGroups on dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID) inner join dbo.System_Servers on dbo.System_ServerGroups.MasterServer = dbo.System_Servers.ID where dbo.Benutzer.id = " & MyUserID & " and dbo.System_Servers.enabled = 1"
                    .CommandType = CommandType.Text
                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn

                MyRecSet = MyCmd.ExecuteReader()
                CurRecordIsRead = MyRecSet.Read
                If Not CurRecordIsRead Then
                    MyRecSet.Close()
                    MyCmd.Dispose()

                    MyCmd = New SqlClient.SqlCommand

                    'Get parameter value and append parameter
                    With MyCmd
                        .CommandText = "select dbo.System_ServerGroups.ServerGroup, dbo.System_Servers.ServerProtocol, dbo.System_Servers.ServerName, dbo.System_Servers.ServerPort from (((dbo.System_AccessLevels inner join dbo.Benutzer on dbo.System_AccessLevels.ID = dbo.Benutzer.AccountAccessability) inner join dbo.System_ServerGroupsAndTheirUserAccessLevels on dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel) inner join dbo.System_ServerGroups on dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID) inner join dbo.System_Servers on dbo.System_ServerGroups.MasterServer = dbo.System_Servers.ID where dbo.Benutzer.id = " & MyUserID
                        .CommandType = CommandType.Text
                    End With

                    'Create recordset by executing the command
                    MyCmd.Connection = MyDBConn

                    MyRecSet = MyCmd.ExecuteReader()
                    CurRecordIsRead = MyRecSet.Read
                    AllServersAreDisabled = True
                Else
                    AllServersAreDisabled = False
                End If

                Dim MyServerURL As String
                Dim MyServerGroupTitle As String
                Dim MyRowCounter As Integer

                Do While CurRecordIsRead
                    CurRecordIsRead = False

                    MyRowCounter += 1
                    MyServerGroupTitle = CType(MyRecSet("ServerGroup"), String)
                    MyServerURL = CType(MyRecSet("ServerProtocol"), String) & "://" & CType(MyRecSet("ServerName"), String)
                    If Not IsDBNull(MyRecSet("ServerPort")) Then
                        If Not CType(MyRecSet("ServerName"), String) = "" Then
                            If LCase(CType(MyRecSet("ServerProtocol"), String)) = "http" AndAlso CType(MyRecSet("ServerPort"), Integer) = 80 Then
                                'don't add default port for selected protocol
                            ElseIf LCase(CType(MyRecSet("ServerProtocol"), String)) = "https" AndAlso CType(MyRecSet("ServerPort"), Integer) = 443 Then
                                'don't add default port for selected protocol
                            Else
                                MyServerURL = MyServerURL & ":" & CType(MyRecSet("ServerPort"), Integer).ToString
                            End If
                        End If
                    End If
                    MyServerURL = MyServerURL & "/"

                    CurRecordIsRead = MyRecSet.Read

                    If MyRowCounter = 1 And Not CurRecordIsRead Then
                        '1 server only
                        MyResult = MyServerURL
                    ElseIf MyRowCounter = 1 And CurRecordIsRead Then
                        'several servers found, this is the 1st server
                        MyResult = MyServerURL & " (" & MyServerGroupTitle & ")"
                    Else
                        MyResult = MyResult & ControlChars.CrLf & MyServerURL & " (" & MyServerGroupTitle & ")"
                        'several servers found, this is one of them (not the first)
                    End If
                    If AllServersAreDisabled Then
                        MyResult = MyResult & ControlChars.CrLf & CType(webManager, WMSystem).Internationalization.UserManagementMasterServerAvailableInNearFuture & ControlChars.CrLf
                    End If
                Loop

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

            GetUserLogonServers = MyResult

        End Function

        ''' <summary>
        ''' Get a list of all userIDs by additional flag
        ''' </summary>
        ''' <param name="flagName"></param>
        ''' <param name="webmanager"></param>
        ''' <returns>All userIDs by additional flag</returns>
        ''' <remarks></remarks>
        Public Function ListOfUsersByAdditionalFlag(ByVal flagName As String, ByVal webmanager As IWebManager) As Long() Implements IDataLayer.ListOfUsersByAdditionalFlag
            Dim sqlStr As String = "SELECT [ID_User] FROM [dbo].[Log_Users] INNER JOIN Benutzer ON Benutzer.ID = dbo.Log_Users.ID_User where Type = @FlagName"
            Dim cmd As New SqlClient.SqlCommand(sqlStr, New SqlClient.SqlConnection(webmanager.ConnectionString))
            cmd.Parameters.Add("@FlagName", SqlDbType.NVarChar).Value = flagName
            Dim userAL As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Dim Result As New ArrayList(userAL.Capacity)
            For MyCounter As Integer = 0 To userAL.Count - 1
                Result.Add(CType(userAL(MyCounter), Long))
            Next
            Return CType(Result.ToArray(GetType(Long)), Long())
        End Function
        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>All flag names which are used in the user profiles</returns>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        '''     [zeutzheim] 03.07.2009 Modified
        ''' </history>
        Public Function ListOfAdditionalFlagsInUseByUserProfiles(ByVal webmanager As IWebManager) As String() Implements IDataLayer.ListOfAdditionalFlagsInUseByUserProfiles
            Const sql As String = "SELECT [Type] FROM [dbo].[Log_Users] INNER JOIN Benutzer ON Benutzer.ID = dbo.Log_Users.ID_User GROUP BY [Type] ORDER BY [Type]"
            Dim list As ArrayList
            'Retrieve all fields stored in table [Log_Users]
            list = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlClient.SqlConnection(webmanager.ConnectionString), sql, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            'remove all not-additional-flag-fields!
            For removeCounter As Integer = list.Count - 1 To 0 Step -1
                For myCounter As Integer = CompuMaster.camm.WebManager.WMSystem.UserInformation.ReservedFlagNames.Length - 1 To 0 Step -1
                    If CType(list(removeCounter), String).ToLower(System.Globalization.CultureInfo.InvariantCulture) = CompuMaster.camm.WebManager.WMSystem.UserInformation.ReservedFlagNames(myCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                        list.RemoveAt(removeCounter)
                        Exit For
                    End If
                Next
            Next

            Return CType(list.ToArray(GetType(String)), String())
        End Function
        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashlist with the flag name as key and the count of occurances as the value</returns>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        '''     [zeutzheim] 03.07.2009 Modified
        ''' </history>
        Public Function ListOfAdditionalFlagsInUseByUserProfilesWithCount(ByVal webmanager As IWebManager) As Hashtable Implements IDataLayer.ListOfAdditionalFlagsInUseByUserProfilesWithCount
            Const sql As String = "SELECT [Type], Count(*) As [Count] FROM [dbo].[Log_Users] INNER JOIN Benutzer ON Benutzer.ID = dbo.Log_Users.ID_User GROUP BY [Type] ORDER BY [Type]"

            'Retrieve all fields stored in table [Log_Users]
            Dim list As Hashtable
            list = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(New SqlClient.SqlConnection(webmanager.ConnectionString), sql, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            'remove all not-additional-flag-fields!
            Dim itemsToRemove As New ArrayList
            For Each item As DictionaryEntry In list
                For myCounter As Integer = 0 To CompuMaster.camm.WebManager.WMSystem.UserInformation.ReservedFlagNames.Length - 1
                    If CType(item.Key, String).ToLower(System.Globalization.CultureInfo.InvariantCulture) = CType(CompuMaster.camm.WebManager.WMSystem.UserInformation.ReservedFlagNames(myCounter), String).ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                        itemsToRemove.Add(item.Key)
                        Exit For
                    End If
                Next
            Next
            For removeCounter As Integer = itemsToRemove.Count - 1 To 0 Step -1
                list.Remove(itemsToRemove(removeCounter))
            Next

            Return list
        End Function
        ''' <summary>
        ''' Get the list of additional flags which are required by the security objects
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <history>
        ''' 	[wezel]	    07.05.2008	Created
        '''     [zeutzheim] 02.07.2009 Modified
        '''     [zeutzheim] 09.07.2009 Modified
        ''' </history>
        Public Function ListOfAdditionalFlagsRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String() Implements IDataLayer.ListOfAddtionalFlagsRequiredBySecurityObjects
            Const sql As String = "SELECT [RequiredUserProfileFlags] FROM [dbo].[Applications_CurrentAndInactiveOnes] where not [RequiredUserProfileFlags] is null AND not [RequiredUserProfileFlags] = '' Group By [RequiredUserProfileFlags] order by [RequiredUserProfileFlags]"
            Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(webmanager.ConnectionString))
            Dim AL As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Dim sourceFlags As String() = CType(AL.ToArray(GetType(String)), String())
            Dim FlagList As String()

            Dim addToResult As Boolean = True

            For myUserCounter As Integer = 0 To sourceFlags.Length - 1

                'Do not return system/ reserved flags
                For mySystemFlagCounter As Integer = 0 To CompuMaster.camm.WebManager.WMSystem.UserInformation.ReservedFlagNames.Length - 1
                    If sourceFlags(myUserCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture) = CompuMaster.camm.WebManager.WMSystem.UserInformation.ReservedFlagNames(mySystemFlagCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                        addToResult = False
                        myUserCounter = sourceFlags.Length - 1
                        Exit For
                    End If
                Next
                If addToResult Then
                    For myCounter As Integer = 0 To AL.Count - 1
                        If CStr(AL(myCounter)).IndexOf(",") >= 0 Then
                            Dim MoreFlags As String() = CStr(AL(myCounter)).Split(New [Char]() {","c})
                            For my2ndcounter As Integer = 1 To MoreFlags.Length - 1
                                If Trim(MoreFlags(my2ndcounter)) <> Nothing Then AL.Add(Trim(MoreFlags(my2ndcounter)))
                            Next
                            AL(myCounter) = Trim(CStr(AL(myCounter)).Substring(0, CStr(AL(myCounter)).IndexOf(",")))
                        End If
                    Next
                End If
            Next

            If AL.Count < 0 Then
                ReDim FlagList(0)
                FlagList(0) = ""
            Else
                ReDim FlagList(AL.Count - 1)
                AL.CopyTo(FlagList)

                'Remove Duplicates
                Dim ALRD As New ArrayList
                For DPCounter As Integer = 0 To FlagList.Length - 1
                    If Not ALRD.Contains(FlagList(DPCounter)) Then
                        ALRD.Add(Trim(FlagList(DPCounter)))
                    End If
                Next
                FlagList = Nothing
                ReDim FlagList(ALRD.Count - 1)
                ALRD.CopyTo(FlagList)
            End If

            Return FlagList

        End Function
        ''' <summary>
        ''' Get the list of additional flags which are not required by the security objects
        ''' </summary>
        ''' <param name="webmanager"></param>
        Public Function ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String() Implements IDataLayer.ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects
            Dim flagListByUserProfiles As String() = Me.ListOfAdditionalFlagsInUseByUserProfiles(webmanager)
            Dim flagListBySecurityObjects As String() = Me.ListOfAdditionalFlagsRequiredBySecurityObjects(webmanager)
            Dim addToResult As Boolean = True
            Dim resultAL As New ArrayList
            For myUserCounter As Integer = 0 To flagListByUserProfiles.Length - 1
                For mySecObjCounter As Integer = 0 To flagListBySecurityObjects.Length - 1
                    If flagListByUserProfiles(myUserCounter) = flagListBySecurityObjects(mySecObjCounter) Then
                        addToResult = False
                    End If
                Next
                If addToResult Then
                    resultAL.Add(flagListByUserProfiles(myUserCounter))
                End If
                addToResult = True
            Next
            Return CType(resultAL.ToArray(GetType(System.String)), String())
        End Function
        ''' <summary>
        ''' Copy authorizations from one security object to another one (without creating duplicates if they already exist)
        ''' </summary>
        ''' <param name="webmanager">A valid web-manager instance</param>
        ''' <param name="sourceSecurityObjectID">The security object ID which shall be the source</param>
        ''' <param name="destinationSecurityObjectID">The ID of the security ojbect which shall receive the additional authorizations</param>
        ''' <remarks>
        ''' Only missing authorizations will be copied to the destination security object.
        ''' </remarks>
        Public Sub CopyAuthorizations(ByVal webmanager As IWebManager, ByVal sourceSecurityObjectID As Integer, ByVal destinationSecurityObjectID As Integer) Implements IDataLayer.CopyAuthorizations
            'Environment check
            Dim _DBVersion As Version = Setup.DatabaseUtils.Version(webmanager, True)
            If _DBVersion.CompareTo(WMSystem.MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                Throw New NotImplementedException("Support for database version " & _DBVersion.ToString & " is currently not supported. Please update the camm WebManager software, first!")
            End If

            Const sqlTillDbBuild_4_11 As String = _
                "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                "-- Copy missing user authorizations --" & vbNewLine & _
                "INSERT INTO [dbo].[ApplicationsRightsByUser]" & vbNewLine & _
                "           ([ID_Application]" & vbNewLine & _
                "           ,[ID_GroupOrPerson]" & vbNewLine & _
                "           ,[ReleasedOn]" & vbNewLine & _
                "           ,[ReleasedBy]" & vbNewLine & _
                "           ,[DevelopmentTeamMember])" & vbNewLine & _
                "SELECT @DestinationSecObjID" & vbNewLine & _
                "      ,[ID_GroupOrPerson]" & vbNewLine & _
                "      ,[ReleasedOn]" & vbNewLine & _
                "      ,[ReleasedBy]" & vbNewLine & _
                "      ,[DevelopmentTeamMember]" & vbNewLine & _
                "  FROM [dbo].[ApplicationsRightsByUser]" & vbNewLine & _
                "where [ID_Application] = @SourceSecObjID AND ID_GroupOrPerson NOT IN " & vbNewLine & _
                "	(" & vbNewLine & _
                "	SELECT [ID_GroupOrPerson]" & vbNewLine & _
                "	FROM [dbo].[ApplicationsRightsByUser]" & vbNewLine & _
                "	WHERE ID_Application = @DestinationSecObjID" & vbNewLine & _
                "	)" & vbNewLine & _
                "" & vbNewLine & _
                "-- Copy missing group authorizations --" & vbNewLine & _
                "INSERT INTO [dbo].[ApplicationsRightsByGroup]" & vbNewLine & _
                "           ([ID_Application]" & vbNewLine & _
                "           ,[ID_GroupOrPerson]" & vbNewLine & _
                "           ,[ReleasedOn]" & vbNewLine & _
                "           ,[ReleasedBy])" & vbNewLine & _
                "SELECT @DestinationSecObjID" & vbNewLine & _
                "      ,[ID_GroupOrPerson]" & vbNewLine & _
                "      ,[ReleasedOn]" & vbNewLine & _
                "      ,[ReleasedBy]" & vbNewLine & _
                "  FROM [dbo].[ApplicationsRightsByGroup]" & vbNewLine & _
                "where [ID_Application] = @SourceSecObjID AND ID_GroupOrPerson NOT IN " & vbNewLine & _
                "	(" & vbNewLine & _
                "	SELECT [ID_GroupOrPerson]" & vbNewLine & _
                "	FROM [dbo].[ApplicationsRightsByGroup]" & vbNewLine & _
                "	WHERE ID_Application = @DestinationSecObjID" & vbNewLine & _
                "	)"

            Const sqlSinceDbBuild_4_12 As String = _
                "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                "-- Copy missing user authorizations --" & vbNewLine & _
                "INSERT INTO [dbo].[ApplicationsRightsByUser]" & vbNewLine & _
                "           ([ID_Application]" & vbNewLine & _
                "           ,[ID_GroupOrPerson]" & vbNewLine & _
                "           ,[ID_ServerGroup]" & vbNewLine & _
                "           ,[ReleasedOn]" & vbNewLine & _
                "           ,[ReleasedBy]" & vbNewLine & _
                "           ,[DevelopmentTeamMember]" & vbNewLine & _
                "           ,[IsDenyRule])" & vbNewLine & _
                "SELECT @DestinationSecObjID" & vbNewLine & _
                "      ,NewAuths.[ID_GroupOrPerson]" & vbNewLine & _
                "      ,NewAuths.[ID_ServerGroup]" & vbNewLine & _
                "      ,NewAuths.[ReleasedOn]" & vbNewLine & _
                "      ,NewAuths.[ReleasedBy]" & vbNewLine & _
                "      ,NewAuths.[DevelopmentTeamMember]" & vbNewLine & _
                "      ,NewAuths.[IsDenyRule]" & vbNewLine & _
                "FROM [dbo].[ApplicationsRightsByUser] AS NewAuths" & vbNewLine & _
                "    LEFT JOIN [dbo].[ApplicationsRightsByUser] AS ExistingAuths" & vbNewLine & _
                "        ON ExistingAuths.ID_Application = @DestinationSecObjID" & vbNewLine & _
                "            AND ExistingAuths.ID_GroupOrPerson = NewAuths.ID_GroupOrPerson" & vbNewLine & _
                "            AND ExistingAuths.ID_ServerGroup = NewAuths.ID_ServerGroup" & vbNewLine & _
                "            AND ExistingAuths.IsDenyRule = NewAuths.IsDenyRule" & vbNewLine & _
                "            AND ExistingAuths.DevelopmentTeamMember = NewAuths.DevelopmentTeamMember" & vbNewLine & _
                "WHERE NewAuths.[ID_Application] = @SourceSecObjID " & vbNewLine & _
                "    AND ExistingAuths.ID IS NULL" & vbNewLine & _
                "    " & vbNewLine & _
                "-- Copy missing group authorizations --" & vbNewLine & _
                "INSERT INTO [dbo].[ApplicationsRightsByGroup]" & vbNewLine & _
                "           ([ID_Application]" & vbNewLine & _
                "           ,[ID_GroupOrPerson]" & vbNewLine & _
                "           ,[ID_ServerGroup]" & vbNewLine & _
                "           ,[ReleasedOn]" & vbNewLine & _
                "           ,[ReleasedBy]" & vbNewLine & _
                "           ,[DevelopmentTeamMember]" & vbNewLine & _
                "           ,[IsDenyRule])" & vbNewLine & _
                "SELECT @DestinationSecObjID" & vbNewLine & _
                "      ,NewAuths.[ID_GroupOrPerson]" & vbNewLine & _
                "      ,NewAuths.[ID_ServerGroup]" & vbNewLine & _
                "      ,NewAuths.[ReleasedOn]" & vbNewLine & _
                "      ,NewAuths.[ReleasedBy]" & vbNewLine & _
                "      ,NewAuths.[DevelopmentTeamMember]" & vbNewLine & _
                "      ,NewAuths.[IsDenyRule]" & vbNewLine & _
                "FROM [dbo].[ApplicationsRightsByGroup] AS NewAuths" & vbNewLine & _
                "    LEFT JOIN [dbo].[ApplicationsRightsByGroup] AS ExistingAuths" & vbNewLine & _
                "        ON ExistingAuths.ID_Application = @DestinationSecObjID" & vbNewLine & _
                "            AND ExistingAuths.ID_GroupOrPerson = NewAuths.ID_GroupOrPerson" & vbNewLine & _
                "            AND ExistingAuths.ID_ServerGroup = NewAuths.ID_ServerGroup" & vbNewLine & _
                "            AND ExistingAuths.IsDenyRule = NewAuths.IsDenyRule" & vbNewLine & _
                "            AND ExistingAuths.DevelopmentTeamMember = NewAuths.DevelopmentTeamMember" & vbNewLine & _
                "WHERE NewAuths.[ID_Application] = @SourceSecObjID " & vbNewLine & _
                "    AND NewAuths.[IsSupervisorAutoAccessRule] = 0" & vbNewLine & _
                "    AND ExistingAuths.ID IS NULL"

            Dim sql As String
            If _DBVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                sql = sqlSinceDbBuild_4_12
            Else
                sql = sqlTillDbBuild_4_11
            End If

            Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(webmanager.ConnectionString))
            cmd.Parameters.Add("@SourceSecObjID", SqlDbType.Int).Value = sourceSecurityObjectID
            cmd.Parameters.Add("@DestinationSecObjID", SqlDbType.Int).Value = destinationSecurityObjectID
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        ''' <summary>
        ''' Create an appropriate log entry for an external, not-yet-assigned user account
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        ''' <param name="fullUserName">The complete name of the user, e. g. &quot;Dr. Bill Wilson&quot;</param>
        Public Sub AddMissingExternalAccountAssignment(ByVal webmanager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String, ByVal fullUserName As String, ByVal emailAddress As String, ByVal errorDetails As String) Implements IDataLayer.AddMissingExternalAccountAssignment
            If Setup.DatabaseUtils.Version(webmanager, True).Build >= 162 Then
                Dim MyCmd As New SqlClient.SqlCommand("LogMissingExternalUserAssignment", New SqlClient.SqlConnection(webmanager.ConnectionString))
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ExternalAccountSystem", SqlDbType.NVarChar, 50).Value = externalAccountSystemName
                MyCmd.Parameters.Add("@LogonName", SqlDbType.NVarChar, 100).Value = fullUserLogonName
                MyCmd.Parameters.Add("@FullUserName", SqlDbType.NVarChar, 150).Value = Utils.StringNotNothingOrDBNull(fullUserName)
                MyCmd.Parameters.Add("@EMailAddress", SqlDbType.NVarChar, 250).Value = Utils.StringNotNothingOrDBNull(emailAddress)
                MyCmd.Parameters.Add("@Error", SqlDbType.NText).Value = Utils.StringNotNothingOrDBNull(errorDetails)
                MyCmd.Parameters.Add("@Remove", SqlDbType.Bit).Value = False
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        ''' <summary>
        ''' Remove an existing log entry of an external account which is successfully assigned, now
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        Public Sub RemoveMissingExternalAccountAssignment(ByVal webmanager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String) Implements IDataLayer.RemoveMissingExternalAccountAssignment
            If Setup.DatabaseUtils.Version(webmanager, True).Build >= 162 Then
                Dim MyCmd As New SqlClient.SqlCommand("LogMissingExternalUserAssignment", New SqlClient.SqlConnection(webmanager.ConnectionString))
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ExternalAccountSystem", SqlDbType.NVarChar, 50).Value = externalAccountSystemName
                MyCmd.Parameters.Add("@LogonName", SqlDbType.NVarChar, 100).Value = fullUserLogonName
                MyCmd.Parameters.Add("@FullUserName", SqlDbType.NVarChar, 150).Value = DBNull.Value
                MyCmd.Parameters.Add("@EMailAddress", SqlDbType.NVarChar, 250).Value = DBNull.Value
                MyCmd.Parameters.Add("@Error", SqlDbType.NText).Value = DBNull.Value
                MyCmd.Parameters.Add("@Remove", SqlDbType.Bit).Value = True
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        ''' <summary>
        ''' Query a list of existing user IDs
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        Public Function ActiveUsers(ByVal webmanager As IWebManager) As Long() Implements IDataLayer.ActiveUsers
            Const sql As String = "SELECT ID FROM Benutzer"
            Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(webmanager.ConnectionString))

            'New code to fix InvalidCastException
            Dim Al As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Dim result As Long()
            ReDim result(Al.Count - 1)
            For myCounter As Integer = 0 To Al.Count - 1
                result(myCounter) = CType(Al(myCounter), Long)
            Next

            Return result

        End Function

        ''' <summary>
        ''' Query a list of user IDs from existing plus deleted users
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashtable containing the user ID as key field (Int64) and the status &quot;Deleted&quot; as a boolean value in the hashtable's value field (true indicates a deleted user)</returns>
        Public Function ActiveAndDeletedUsers(ByVal webmanager As IWebManager) As Hashtable Implements IDataLayer.ActiveAndDeletedUsers
            Const sql As String = "SELECT IsNull(AllUsers.ID_User, Benutzer.ID) AS ID, CASE WHEN Benutzer.ID IS NULL THEN 1 ELSE 0 END AS Deleted FROM Benutzer full join (SELECT ID_User FROM Log_Users GROUP BY ID_User) as AllUsers on Benutzer.ID = AllUsers.ID_User"
            Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(webmanager.ConnectionString))
            Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        Public Sub AddGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean) Implements IDataLayer.AddGroupAuthorization
            If groupID = Nothing Then
                Dim Message As String = "Group has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf securityObjectID = Nothing Then
                Dim Message As String = "Security object has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            End If

            Dim MyConn As New SqlClient.SqlConnection(webmanager.ConnectionString)
            Dim MyCmd As New SqlClient.SqlCommand("AdminPrivate_CreateApplicationRightsByGroup", MyConn)
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = CType(webmanager, WMSystem).CurrentUserID(WMSystem.SpecialUsers.User_Code)
            MyCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = securityObjectID
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
            If Setup.DatabaseUtils.Version(webmanager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd.Parameters.Add("@ServerGroupID", SqlDbType.Int).Value = serverGroupID
                MyCmd.Parameters.Add("@IsDevelopmentTeamMember", SqlDbType.Bit).Value = isDeveloperAuthorization
                MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
            End If

            Dim Result As Object
            Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(Result) OrElse Result Is Nothing Then
                Dim Message As String = "Authorization creation failed"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf CType(Result, Integer) = -1 Then
                'Success
            ElseIf CType(Result, Integer) = 0 Then
                Dim Message As String = "Authorization creation failed (invalid release-by-user)"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf CType(Result, Integer) = 3 Then
                Dim Message As String = "Authorization creation failed (invalid application ID)"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf CType(Result, Integer) = 1 Then
                Dim Message As String = "Authorization creation failed (this special SAP application can't be authorized for groups)"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            Else
                Dim Message As String = "Authorization creation failed unexpectedly"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            End If
        End Sub

        Public Sub RemoveGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer) Implements IDataLayer.RemoveGroupAuthorization
            RemoveGroupAuthorization(webmanager, securityObjectID, groupID, serverGroupID, False, False)
        End Sub

        Public Sub RemoveGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean) Implements IDataLayer.RemoveGroupAuthorization
            If groupID = Nothing Then
                Dim Message As String = "Group has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf securityObjectID = Nothing Then
                Dim Message As String = "Security object has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            End If

            Dim MyCmd As SqlClient.SqlCommand
            'Find the auth ID
            MyCmd = New SqlClient.SqlCommand("", New SqlClient.SqlConnection(webmanager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            If Setup.DatabaseUtils.Version(webmanager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd.CommandText = "select top 1 id as AuthID from dbo.ApplicationsRightsByGroup where id_grouporperson = @groupid and id_application = @SecurityObjectID AND IsNull(ID_ServerGroup, 0) = @ServerGroupID and [DevelopmentTeamMember] = @IsDeveloperAuthorization and [IsDenyRule] = @IsDenyRule"
            Else
                MyCmd.CommandText = "select top 1 id as AuthID from dbo.ApplicationsRightsByGroup where id_grouporperson = @groupid and id_application = @SecurityObjectID"
            End If
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
            MyCmd.Parameters.Add("@SecurityObjectID", SqlDbType.Int).Value = securityObjectID
            If Setup.DatabaseUtils.Version(webmanager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd.Parameters.Add("@ServerGroupID", SqlDbType.Int).Value = serverGroupID
                MyCmd.Parameters.Add("@IsDeveloperAuthorization", SqlDbType.Bit).Value = isDeveloperAuthorization
                MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
            End If
            Dim AuthID As Integer = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)

            'Remove the auth line
            If AuthID <> Nothing Then
                MyCmd = New SqlClient.SqlCommand("AdminPrivate_DeleteApplicationRightsByGroup", New SqlClient.SqlConnection(webmanager.ConnectionString))
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@AuthID", SqlDbType.Int).Value = AuthID
                MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = CType(webmanager, WMSystem).CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                'Requery the list of authorization next time it's required
            End If

        End Sub

        Public Sub RemoveUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer) Implements IDataLayer.RemoveUserAuthorization
            RemoveUserAuthorization(webmanager, securityObjectID, userID, serverGroupID, False, False)
        End Sub

        Public Sub RemoveUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean) Implements IDataLayer.RemoveUserAuthorization
            If userID = Nothing Then
                Dim Message As String = "User has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf securityObjectID = Nothing Then
                Dim Message As String = "Security object has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            End If

            Dim MyCmd As SqlClient.SqlCommand
            'Find the auth ID
            MyCmd = New SqlClient.SqlCommand("", New SqlClient.SqlConnection(webmanager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            If Setup.DatabaseUtils.Version(webmanager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd.CommandText = "select top 1 id as AuthID from dbo.ApplicationsRightsByUser where id_grouporperson = @userid and id_application = @SecurityObjectID AND IsNull(ID_ServerGroup, 0) = @ServerGroupID and [DevelopmentTeamMember] = @IsDeveloperAuthorization and [IsDenyRule] = @IsDenyRule"
            Else
                MyCmd.CommandText = "select top 1 id as AuthID from dbo.ApplicationsRightsByUser where id_grouporperson = @userid and id_application = @SecurityObjectID"
            End If
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
            MyCmd.Parameters.Add("@SecurityObjectID", SqlDbType.Int).Value = securityObjectID
            If Setup.DatabaseUtils.Version(webmanager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd.Parameters.Add("@ServerGroupID", SqlDbType.Int).Value = serverGroupID
                MyCmd.Parameters.Add("@IsDeveloperAuthorization", SqlDbType.Bit).Value = isDeveloperAuthorization
                MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
            ElseIf isDenyRule Then
                Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
            End If
            Dim AuthID As Integer = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)

            'Remove the auth line
            If AuthID <> Nothing Then
                MyCmd = New SqlClient.SqlCommand("AdminPrivate_DeleteApplicationRightsByUser", New SqlClient.SqlConnection(webmanager.ConnectionString))
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@AuthID", SqlDbType.Int).Value = AuthID
                MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = CType(webmanager, WMSystem).CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

    End Class

End Namespace