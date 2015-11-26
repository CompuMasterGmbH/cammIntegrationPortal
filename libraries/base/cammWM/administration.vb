Option Explicit On 
Option Strict On

Namespace CompuMaster.camm.WebManager

    Public Interface IDataLayer

        Sub RemoveUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer)
        Sub RemoveGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer)
        Sub AddGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer)

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Set a user profile setting
        ''' </summary>
        ''' <param name="webManager">A valid instance of camm Web-Manager</param>
        ''' <param name="dbConnection">An open connection which shall be used or nothing if a new one shall be created independently and on the fly</param>
        ''' <param name="userID">The ID of the user who shall receive the updated value</param>
        ''' <param name="propertyName">The key name of the flag</param>
        ''' <param name="value">The new value of the flag</param>
        ''' <param name="doNotLogSuccess">False will lead to an informational log entry in the database after the value has been saved; in case of True there won't be created a log entry</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	04.04.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function SetUserDetail(ByVal webManager As IWebManager, ByVal dbConnection As IDbConnection, ByVal userID As Long, ByVal propertyName As String, ByVal value As String, Optional ByVal doNotLogSuccess As Boolean = False) As Boolean

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Reads a single user detail value from the database
        ''' </summary>
        ''' <param name="webManager">A reference to a camm Web-Manager instance</param>
        ''' <param name="userID">The user ID</param>
        ''' <param name="propertyName">The requested property name</param>
        ''' <returns>The resulting value as a String</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	15.08.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function GetUserDetail(ByVal webManager As IWebManager, ByVal userID As Long, ByVal propertyName As String) As String

        ''' -----------------------------------------------------------------------------
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
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function GetUserLogonServers(ByVal webManager As IWebManager, ByVal userID As Long) As String

        ''' <summary>
        ''' Get a list of all userIDs by additional flag
        ''' </summary>
        ''' <param name="FlagName"></param>
        ''' <param name="webmanager"></param>
        ''' <returns>All userIDs by additional flag</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     [zeutzheim] 17.08.2009 Created
        ''' </history>
        Function ListOfUsersByAdditionalFlag(ByVal FlagName As String, ByVal webmanager As IWebManager) As Long()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>All flag names which are used in the user profiles</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function ListOfAdditionalFlagsInUseByUserProfiles(ByVal webmanager As IWebManager) As String()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashlist with the flag name as key and the count of occurances as the value</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function ListOfAdditionalFlagsInUseByUserProfilesWithCount(ByVal webmanager As IWebManager) As Hashtable

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are required by the security objects
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function ListOfAddtionalFlagsRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are not required by the security objects
        ''' </summary>
        ''' <param name="webmanager"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.07.2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Copy authorizations from one security object to another one (without creating duplicates if they already exist)
        ''' </summary>
        ''' <param name="webmanager">A valid web-manager instance</param>
        ''' <param name="sourceSecurityObjectID">The security object ID which shall be the source</param>
        ''' <param name="destinationSecurityObjectID">The ID of the security ojbect which shall receive the additional authorizations</param>
        ''' <remarks>
        ''' Only missing authorizations will be copied to the destination security object.
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	27.05.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub CopyAuthorizations(ByVal webmanager As IWebManager, ByVal sourceSecurityObjectID As Integer, ByVal destinationSecurityObjectID As Integer)

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create an appropriate log entry for an external, not-yet-assigned user account
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        ''' <param name="fullUserName">The complete name of the user, e. g. &quot;Dr. Bill Wilson&quot;</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.09.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AddMissingExternalAccountAssignment(ByVal webmanager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String, ByVal fullUserName As String, ByVal emailAddress As String, ByVal errorDetails As String)

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Remove an existing log entry of an external account which is successfully assigned, now
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.09.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub RemoveMissingExternalAccountAssignment(ByVal webmanager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String)

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Query a list of existing user IDs
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	23.10.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function ActiveUsers(ByVal webmanager As IWebManager) As Long()

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Query a list of user IDs from existing plus deleted users
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashtable containing the user ID as key field (Int64) and the status &quot;Deleted&quot; as a boolean value in the hashtable's value field (true indicates a deleted user)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	23.10.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function ActiveAndDeletedUsers(ByVal webmanager As IWebManager) As Hashtable

    End Interface

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.DataLayer
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Provides access to the configured database layer (e. g. for MS SQL Server)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[wezel]	27.05.2008	Created
    '''     [wezel]	27.05.2008	Changed access modifier from Friend to Public because several other components (e.g. a webservice implementing interfaces to external systems) need direct access because of high performance requirements
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DataLayer

        Private Shared _DataLayer As IDataLayer
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The data layer which contains all sql commands which shall run against the required database engine
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	27.05.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Property Current() As IDataLayer
            Get
                If _DataLayer Is Nothing Then
                    _DataLayer = New DataLayerSqlClient
                End If
                Return _DataLayer
            End Get
            Set(ByVal Value As IDataLayer)
                _DataLayer = Value
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.DataLayerSqlClient
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' A database layer for MS SQL Server
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[wezel]	27.05.2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class DataLayerSqlClient
        Implements IDataLayer

        ''' <summary>
        '''     Set a user profile setting
        ''' </summary>
        ''' <param name="webManager">A valid instance of camm Web-Manager</param>
        ''' <param name="dbConnection">An open connection which shall be used or nothing if a new one shall be created independently and on the fly</param>
        ''' <param name="userID">The ID of the user who shall receive the updated value</param>
        ''' <param name="propertyName">The key name of the flag</param>
        ''' <param name="value">The new value of the flag</param>
        ''' <param name="doNotLogSuccess">False will lead to an informational log entry in the database after the value has been saved; in case of True there won't be created a log entry</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
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
        ''' <remarks>
        ''' </remarks>
        Public Function GetUserDetail(ByVal webManager As IWebManager, ByVal userID As Long, ByVal propertyName As String) As String Implements IDataLayer.GetUserDetail
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(webManager.ConnectionString)
            MyCmd.CommandText = "Public_GetUserDetailData"
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@IDUser", SqlDbType.Int).Value = userID
            MyCmd.Parameters.Add("@Type", SqlDbType.NVarChar).Value = Trim(propertyName)
            Return Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
        End Function

        ''' -----------------------------------------------------------------------------
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
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select dbo.System_ServerGroups.ServerGroup, dbo.System_Servers.ServerProtocol, dbo.System_Servers.ServerName, dbo.System_Servers.ServerPort from (((dbo.System_AccessLevels inner join dbo.Benutzer on dbo.System_AccessLevels.ID = dbo.Benutzer.AccountAccessability) inner join dbo.System_ServerGroupsAndTheirUserAccessLevels on dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel) inner join dbo.System_ServerGroups on dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID) inner join dbo.System_Servers on dbo.System_ServerGroups.MasterServer = dbo.System_Servers.ID where dbo.Benutzer.id = " & MyUserID & " and dbo.System_Servers.enabled = 1"
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
                        .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select dbo.System_ServerGroups.ServerGroup, dbo.System_Servers.ServerProtocol, dbo.System_Servers.ServerName, dbo.System_Servers.ServerPort from (((dbo.System_AccessLevels inner join dbo.Benutzer on dbo.System_AccessLevels.ID = dbo.Benutzer.AccountAccessability) inner join dbo.System_ServerGroupsAndTheirUserAccessLevels on dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel) inner join dbo.System_ServerGroups on dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID) inner join dbo.System_Servers on dbo.System_ServerGroups.MasterServer = dbo.System_Servers.ID where dbo.Benutzer.id = " & MyUserID
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
        ''' <history>
        '''     [zeutzheim] 17.08.2009 Created
        ''' </history>
        Public Function ListOfUsersByAdditionalFlag(ByVal flagName As String, ByVal webmanager As IWebManager) As Long() Implements IDataLayer.ListOfUsersByAdditionalFlag
            Dim sqlStr As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [ID_User] FROM [dbo].[Log_Users] INNER JOIN Benutzer ON Benutzer.ID = dbo.Log_Users.ID_User where Type = @FlagName"
            Dim cmd As New SqlClient.SqlCommand(sqlStr, New SqlClient.SqlConnection(webmanager.ConnectionString))
            cmd.Parameters.Add("@FlagName", SqlDbType.NVarChar).Value = flagName
            Dim userAL As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Dim Result As New ArrayList(userAL.Capacity)
            For MyCounter As Integer = 0 To userAL.Count - 1
                Result.Add(CType(userAL(MyCounter), Long))
            Next
            Return CType(Result.ToArray(GetType(Long)), Long())
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>All flag names which are used in the user profiles</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        '''     [zeutzheim] 03.07.2009 Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ListOfAdditionalFlagsInUseByUserProfiles(ByVal webmanager As IWebManager) As String() Implements IDataLayer.ListOfAdditionalFlagsInUseByUserProfiles
            Const sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [Type] FROM [dbo].[Log_Users] INNER JOIN Benutzer ON Benutzer.ID = dbo.Log_Users.ID_User GROUP BY [Type] ORDER BY [Type]"
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashlist with the flag name as key and the count of occurances as the value</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	07.05.2008	Created
        '''     [zeutzheim] 03.07.2009 Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ListOfAdditionalFlagsInUseByUserProfilesWithCount(ByVal webmanager As IWebManager) As Hashtable Implements IDataLayer.ListOfAdditionalFlagsInUseByUserProfilesWithCount
            Const sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [Type], Count(*) As [Count] FROM [dbo].[Log_Users] INNER JOIN Benutzer ON Benutzer.ID = dbo.Log_Users.ID_User GROUP BY [Type] ORDER BY [Type]"

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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are required by the security objects
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	    07.05.2008	Created
        '''     [zeutzheim] 02.07.2009 Modified
        '''     [zeutzheim] 09.07.2009 Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ListOfAdditionalFlagsRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String() Implements IDataLayer.ListOfAddtionalFlagsRequiredBySecurityObjects
            Const sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [RequiredUserProfileFlags] FROM [dbo].[Applications_CurrentAndInactiveOnes] where not [RequiredUserProfileFlags] is null AND not [RequiredUserProfileFlags] = '' Group By [RequiredUserProfileFlags] order by [RequiredUserProfileFlags]"
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get the list of additional flags which are not required by the security objects
        ''' </summary>
        ''' <param name="webmanager"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.07.2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Copy authorizations from one security object to another one (without creating duplicates if they already exist)
        ''' </summary>
        ''' <param name="webmanager">A valid web-manager instance</param>
        ''' <param name="sourceSecurityObjectID">The security object ID which shall be the source</param>
        ''' <param name="destinationSecurityObjectID">The ID of the security ojbect which shall receive the additional authorizations</param>
        ''' <remarks>
        ''' Only missing authorizations will be copied to the destination security object.
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	27.05.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CopyAuthorizations(ByVal webmanager As IWebManager, ByVal sourceSecurityObjectID As Integer, ByVal destinationSecurityObjectID As Integer) Implements IDataLayer.CopyAuthorizations
            'Environment check
            Dim _DBVersion As Version = Setup.DatabaseUtils.Version(webmanager, True)
            If _DBVersion.CompareTo(WMSystem.MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                Throw New NotImplementedException("Support for database version " & _DBVersion.ToString & " is currently not supported. Please update the camm WebManager software, first!")
            End If

            Const sql As String = _
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
            Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(webmanager.ConnectionString))
            cmd.Parameters.Add("@SourceSecObjID", SqlDbType.Int).Value = sourceSecurityObjectID
            cmd.Parameters.Add("@DestinationSecObjID", SqlDbType.Int).Value = destinationSecurityObjectID
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Create an appropriate log entry for an external, not-yet-assigned user account
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        ''' <param name="fullUserName">The complete name of the user, e. g. &quot;Dr. Bill Wilson&quot;</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.09.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Remove an existing log entry of an external account which is successfully assigned, now
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	12.09.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Query a list of existing user IDs
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	23.10.2008	Created
        '''     [zeutzheim] 03.07.2009 Modified
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ActiveUsers(ByVal webmanager As IWebManager) As Long() Implements IDataLayer.ActiveUsers
            Const sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID FROM Benutzer"
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Query a list of user IDs from existing plus deleted users
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashtable containing the user ID as key field (Int64) and the status &quot;Deleted&quot; as a boolean value in the hashtable's value field (true indicates a deleted user)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	23.10.2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ActiveAndDeletedUsers(ByVal webmanager As IWebManager) As Hashtable Implements IDataLayer.ActiveAndDeletedUsers
            Const sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT IsNull(AllUsers.ID_User, Benutzer.ID) AS ID, CASE WHEN Benutzer.ID IS NULL THEN 1 ELSE 0 END AS Deleted FROM Benutzer full join (SELECT ID_User FROM Log_Users GROUP BY ID_User) as AllUsers on Benutzer.ID = AllUsers.ID_User"
            Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(webmanager.ConnectionString))
            Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        Public Sub AddGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer) Implements IDataLayer.AddGroupAuthorization
            If serverGroupID <> Nothing Then Throw New NotImplementedException("Specifying a server group ID is not yet supported in this release of camm Web-Manager")

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
            If serverGroupID <> Nothing Then Throw New NotImplementedException("Specifying a server group ID is not yet supported in this release of camm Web-Manager")

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
            MyCmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select top 1 id as AuthID from dbo.ApplicationsRightsByGroup where id_grouporperson = @groupid and id_application = @SecurityObjectID"
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
            MyCmd.Parameters.Add("@SecurityObjectID", SqlDbType.Int).Value = securityObjectID
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
            If serverGroupID <> Nothing Then Throw New NotImplementedException("Specifying a server group ID is not yet supported in this release of camm Web-Manager")

            If userID = Nothing Then
                Dim Message As String = "User has to be created, first, before you can modify the list of authorizations"
                CType(webmanager, WMSystem).Log.RuntimeException(Message)
            ElseIf securityObjectID = Nothing Then
                Dim Message As String = "Security object has to be created, first, before you can modify the list of authorizations"
                CType(WebManager, WMSystem).Log.RuntimeException(Message)
            End If

            Dim MyCmd As SqlClient.SqlCommand
            'Find the auth ID
            MyCmd = New SqlClient.SqlCommand("", New SqlClient.SqlConnection(webmanager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select top 1 id as AuthID from dbo.ApplicationsRightsByUser where id_grouporperson = @userid and id_application = @SecurityObjectID"
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userID
            MyCmd.Parameters.Add("@SecurityObjectID", SqlDbType.Int).Value = securityObjectID
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

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.TestLayer
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Required for NUnit tests only to make requried methods accessable
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	15.08.2006	Created
    '''     [zeutzheim]     02.07.2009  Modified
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class TestLayer
        Public Shared Function ActiveAndDeletedUsers(ByVal WebManager As IWebManager) As Hashtable
            Return CompuMaster.camm.WebManager.DataLayer.Current.ActiveAndDeletedUsers(WebManager)
        End Function
        Public Shared Function ActiveUsers(ByVal WebManager As IWebManager) As Long()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ActiveUsers(WebManager)
        End Function
        Public Shared Sub AddMissingExternalAccountAssignment(ByVal WebManager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String, ByVal fullUserName As String, ByVal emailAdress As String, ByVal errorDetails As String)
            DataLayer.Current.AddMissingExternalAccountAssignment(WebManager, externalAccountSystemName, fullUserLogonName, fullUserName, emailAdress, errorDetails)
        End Sub
        Public Shared Sub CopyAuthorizations(ByVal WebManager As IWebManager, ByVal sourceSecurityObjectID As Integer, ByVal destinationSecurityObjectID As Integer)
            DataLayer.Current.CopyAuthorizations(WebManager, sourceSecurityObjectID, destinationSecurityObjectID)
        End Sub
        Public Shared Function GetUserDetail(ByVal WebManager As IWebManager, ByVal userID As Long, ByVal propertyName As String) As String
            Return CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(WebManager, userID, propertyName)
        End Function
        Public Shared Function GetUserLogonServers(ByVal WebManager As IWebManager, ByVal userID As Long) As String
            Return CompuMaster.camm.WebManager.DataLayer.Current.GetUserLogonServers(WebManager, userID)
        End Function
        Public Shared Function ListOfUsersByAdditionalFlag(ByVal FlagName As String, ByVal WebManager As IWebManager) As Long()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfUsersByAdditionalFlag(FlagName, WebManager)
        End Function
        Public Shared Function ListOfAdditionalFlagsInUseByUserProfiles(ByVal WebManager As IWebManager) As String()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfiles(WebManager)
        End Function
        Public Shared Function ListOfAdditionalFlagsInUseByUserProfilesWithCount(ByVal WebManager As IWebManager) As Hashtable
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfilesWithCount(WebManager)
        End Function
        Public Shared Function ListOfAddtionalFlagsRequiredBySecurityObjects(ByVal WebManager As IWebManager) As String()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAddtionalFlagsRequiredBySecurityObjects(WebManager)
        End Function
        Public Shared Function ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(ByVal WebManager As IWebManager) As String()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(WebManager)
        End Function
        Public Shared Sub RemoveMissingExternalAccountAssignment(ByVal WebManager As IWebManager, ByVal externalaccountsystemname As String, ByVal fullUserLogonName As String)
            CompuMaster.camm.WebManager.DataLayer.Current.RemoveMissingExternalAccountAssignment(WebManager, externalaccountsystemname, fullUserLogonName)
        End Sub
        Public Shared Function SetUserDetail(ByVal WebManager As IWebManager, ByVal dbConnection As IDbConnection, ByVal userID As Long, ByVal propertyName As String, ByVal value As String, Optional ByVal doNotLogSuccess As Boolean = False) As Boolean
            Return CompuMaster.camm.WebManager.DataLayer.Current.SetUserDetail(WebManager, dbConnection, userID, propertyName, value, doNotLogSuccess)
        End Function
    End Class

End Namespace