Option Explicit On
Option Strict On

Public Class Main

    Private Sub ButtonGo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonGo.Click
        Try
            ResetCwmState()
            If Me.TextBoxConnectionStringSourceDB.Text = Nothing Then
                MsgBox("Required name for old database which contains deleted user", MsgBoxStyle.Exclamation)
                Exit Sub
            End If
            If Me.TextBoxConnectionStringSourceDB.Text = Nothing Then
                MsgBox("Required name for current database where the user shall be reimported", MsgBoxStyle.Exclamation)
                Exit Sub
            End If
            If Me.TextBoxLoginName.Text = Nothing Then
                MsgBox("Required loginname for user", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            Dim SourceUsers As DataTable = Nothing
            If IsPipedListOfUsernames() Then
                SourceUsers = SearchUsersByPipeSeperadtedList()
            Else
                SourceUsers = SearchUserByLike()
            End If
            SourceUsers.Columns.Add("CanBeRestored", GetType(Boolean))
            SourceUsers.Columns.Add("Message", GetType(String))

            If SourceUsers.Rows.Count > 0 Then Me.lblCount.Text = String.Format("{0} User found.", SourceUsers.Rows.Count)

            Me.UsersGrid.DataSource = SourceUsers
            Me.UsersGrid.EditMode = DataGridViewEditMode.EditProgrammatically
            Me.UsersGrid.AllowUserToAddRows = False

            If IsPipedListOfUsernames() Then UsersGrid.SelectAll()

            Dim SourceUsersCount As Integer = SourceUsers.Rows.Count
            If SourceUsersCount < 1 Then
                MsgBox("User not found!", MsgBoxStyle.Exclamation)
                MakeProceedingAvailable(False)
            End If
        Catch ex As Exception
            MakeProceedingAvailable(False)
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub



    Private Function IsPipedListOfUsernames() As Boolean
        Return Not String.IsNullOrEmpty(Me.TextBoxLoginName.Text) AndAlso Me.TextBoxLoginName.Text.Contains("|")
    End Function

    Private Function SearchUserByLike() As DataTable
        Return CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(CreateSqlCommandSourceDB("select * from dbo.benutzer where loginname like N'" & Me.TextBoxLoginName.Text.Replace("'", "''").Replace("*", "%") & "'"), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "users")
    End Function

    Private Function SearchUsersByPipeSeperadtedList() As DataTable
        Return CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(CreateSqlCommandSourceDB("select * from dbo.benutzer where loginname in (" & GetPipeSeperatedListAsCsvFprSqlCommand(Me.TextBoxLoginName.Text) & ")"), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "users")
    End Function

    Private Function GetPipeSeperatedListAsCsvFprSqlCommand(pipedList As String) As String
        Dim l As List(Of String) = GetListOfStringFromPipeSeperatetString(pipedList)
        l = EncodeListOfStringForSqlCommand(l)
        Return MakeCommaSeperatedListForSqlCommand(l)
    End Function

    Private Function MakeCommaSeperatedListForSqlCommand(list As List(Of String)) As String
        Dim r As String = String.Empty
        If list.Count > 0 Then
            Dim temp As New Text.StringBuilder()
            For Each str As String In list
                temp.AppendFormat("N'{0}',", str)
            Next
            r = temp.ToString()
            If r.EndsWith(",") Then r = r.Remove(r.Length - 1)
        End If
        Return r
    End Function

    Private Function EncodeListOfStringForSqlCommand(list As List(Of String)) As List(Of String)
        Dim r As New List(Of String)
        For Each str As String In list
            r.Add(EncodeForSqlCommand(str))
        Next
        Return r
    End Function

    Private Function EncodeForSqlCommand(value As String) As String
        If String.IsNullOrEmpty(value) Then
            Return String.Empty
        Else
            Return value.Replace("'", "''")
        End If
    End Function

    Private Function GetListOfStringFromPipeSeperatetString(pipedList As String) As List(Of String)
        Dim r As New List(Of String)
        If Not String.IsNullOrEmpty(pipedList) AndAlso pipedList.Contains("|") Then
            For Each str As String In pipedList.Split("|"c)
                If Not r.Contains(str) Then r.Add(str)
            Next
        End If
        Return r
    End Function

    Sub MakeProceedingAvailable(ByVal visible As Boolean)
        Me.ButtonRestore.Enabled = visible
    End Sub

    Private Sub Main_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = String.Format("User Restore from old CWM database V{0}", Global.My.Application.Info.Version.ToString(3))
        MakeProceedingAvailable(False)
    End Sub

    Private Sub TextBoxLoginName_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBoxLoginName.TextChanged
        MakeProceedingAvailable(False)
    End Sub

    Private Function CreateSqlCommandCurrentDB(ByVal sql As String) As SqlClient.SqlCommand
        Dim Result As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(Me.TextBoxConnectionStringCurrentDB.Text))
        Return Result
    End Function

    Private Function CreateSqlCommandSourceDB(ByVal sql As String) As SqlClient.SqlCommand
        Dim Result As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(Me.TextBoxConnectionStringSourceDB.Text))
        Return Result
    End Function

    Private Sub ButtonRestore_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRestore.Click
        Try
            MakeProceedingAvailable(False)
            RestoreUsers(Me.SourceCwmInstance, Me.DestinationCwmInstance)
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Function SourceCwmInstance() As CompuMaster.camm.WebManager.WMSystem
        Return New CompuMaster.camm.WebManager.WMSystem(Me.TextBoxConnectionStringSourceDB.Text)
    End Function

    Private Function DestinationCwmInstance() As CompuMaster.camm.WebManager.WMSystem
        Return New CompuMaster.camm.WebManager.WMSystem(Me.TextBoxConnectionStringCurrentDB.Text)
    End Function

    Private Sub RestoreUsers(sourceCwm As CompuMaster.camm.WebManager.WMSystem, destCwm As CompuMaster.camm.WebManager.WMSystem)
        For Each r As DataGridViewRow In UsersGrid.SelectedRows
            RestoreUser(r, sourceCwm, destCwm)
        Next
        ResetCwmState()
        CheckIfUsersCanBeRestored()
    End Sub

    Private Sub RestoreUser(row As DataGridViewRow, sourceCwm As CompuMaster.camm.WebManager.WMSystem, destCwm As CompuMaster.camm.WebManager.WMSystem)
        Try
            If row Is Nothing Then Throw New ArgumentNullException("row")
            Dim sourceUserID As Long = CLng(row.Cells("ID").Value)
            Dim SourceUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = sourceCwm.System_GetUserInfo(sourceUserID)
            Dim DestinationUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = destCwm.System_GetUserInfo(sourceUserID)
            If DestinationUserInfo IsNot Nothing Then Throw New Exception("User already exists in current database")

            Try 'with auto-cleanup/rollback on failure
                'Step 1: restore user ID with old ID
                Dim RestoreUserIDSql As String = "BEGIN TRANSACTION" & vbNewLine &
                "IF(	IDENT_INCR( 'dbo.Benutzer' ) IS NOT NULL OR IDENT_SEED('dbo.Benutzer') IS NOT NULL ) SET IDENTITY_INSERT dbo.Benutzer ON;" & vbNewLine &
                "INSERT INTO [dbo].[Benutzer] ([ID],[Loginname],[LoginPW],[LoginDisabled],[AccountAccessability],[CreatedOn],[Anrede],[Vorname],[Nachname],[E-MAIL])" & vbNewLine &
                "VALUES (@UserID,@UserLoginName,'',1,-1,GETDATE(),'u','','','');" & vbNewLine &
                "IF(	IDENT_INCR( 'dbo.Benutzer' ) IS NOT NULL OR IDENT_SEED('dbo.Benutzer') IS NOT NULL ) SET IDENTITY_INSERT dbo.Benutzer OFF;" & vbNewLine &
                "COMMIT"
                Dim RestoreCmd As SqlClient.SqlCommand = Me.CreateSqlCommandCurrentDB(RestoreUserIDSql)
                RestoreCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = sourceUserID
                RestoreCmd.Parameters.Add("@UserLoginName", SqlDbType.NVarChar).Value = SourceUserInfo.LoginName
                CompuMaster.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(RestoreCmd, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Step 2: restore user profile
                DestinationUserInfo = destCwm.System_GetUserInfo(sourceUserID)
                DestinationUserInfo.LoginDeleted = False
                DestinationUserInfo.AdditionalFlags("!RestoreNote") = "User has been restored/recreated on " & Now.ToString("yyyy-MM-dd HH:mm:ss")
                DestinationUserInfo.AcademicTitle = SourceUserInfo.AcademicTitle
                DestinationUserInfo.AccessLevel = SourceUserInfo.AccessLevel
                DestinationUserInfo.AccountAuthorizationsAlreadySet = SourceUserInfo.AccountAuthorizationsAlreadySet
                DestinationUserInfo.AccountProfileValidatedByEMailTest = SourceUserInfo.AccountProfileValidatedByEMailTest
                DestinationUserInfo.AutomaticLogonAllowedByMachineToMachineCommunication = SourceUserInfo.AutomaticLogonAllowedByMachineToMachineCommunication
                DestinationUserInfo.Company = SourceUserInfo.Company
                DestinationUserInfo.Country = SourceUserInfo.Country
                DestinationUserInfo.EMailAddress = SourceUserInfo.EMailAddress
                DestinationUserInfo.ExternalAccount = SourceUserInfo.ExternalAccount
                DestinationUserInfo.FaxNumber = SourceUserInfo.FaxNumber
                DestinationUserInfo.FirstName = SourceUserInfo.FirstName
                DestinationUserInfo.Gender = SourceUserInfo.Gender
                DestinationUserInfo.LastName = SourceUserInfo.LastName
                DestinationUserInfo.Location = SourceUserInfo.Location
                DestinationUserInfo.LoginDisabled = SourceUserInfo.LoginDisabled
                DestinationUserInfo.LoginLockedTemporary = SourceUserInfo.LoginLockedTemporary
                DestinationUserInfo.LoginName = SourceUserInfo.LoginName
                DestinationUserInfo.MobileNumber = SourceUserInfo.MobileNumber
                DestinationUserInfo.NameAddition = SourceUserInfo.NameAddition
                DestinationUserInfo.State = SourceUserInfo.State
                DestinationUserInfo.Street = SourceUserInfo.Street
                DestinationUserInfo.ZipCode = SourceUserInfo.ZipCode
                For Each additionalFlagName As String In SourceUserInfo.AdditionalFlags
                    DestinationUserInfo.AdditionalFlags(additionalFlagName) = SourceUserInfo.AdditionalFlags(additionalFlagName)
                Next
                DestinationUserInfo.Save(True)
                'Step 3: restore user memberships
                'Step 3a: restore user memberships - allow rules
                For Each group As CompuMaster.camm.WebManager.WMSystem.GroupInformation In SourceUserInfo.MembershipsByRule.AllowRule
                    DestinationUserInfo.AddMembership(group, False, New CompuMaster.camm.WebManager.Notifications.NoNotifications(destCwm))
                Next
                'Step 3b: restore user memberships - deny rules
                For Each group As CompuMaster.camm.WebManager.WMSystem.GroupInformation In SourceUserInfo.MembershipsByRule.DenyRule
                    DestinationUserInfo.AddMembership(group, True, New CompuMaster.camm.WebManager.Notifications.NoNotifications(destCwm))
                Next
                'Step 4: restore user authorizations
                'Step 4a: restore user authorizations - allow rules
                For Each userAuth As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForUser In SourceUserInfo.AuthorizationsByRule.AllowRuleStandard
                    DestinationUserInfo.AddAuthorization(userAuth.SecurityObjectID, userAuth.ServerGroupID, False, False, New CompuMaster.camm.WebManager.Notifications.NoNotifications(destCwm))
                Next
                For Each userAuth As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForUser In SourceUserInfo.AuthorizationsByRule.AllowRuleDevelopers
                    DestinationUserInfo.AddAuthorization(userAuth.SecurityObjectID, userAuth.ServerGroupID, True, False, New CompuMaster.camm.WebManager.Notifications.NoNotifications(destCwm))
                Next
                'Step 4b: restore user authorizations - deny rules
                For Each userAuth As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForUser In SourceUserInfo.AuthorizationsByRule.DenyRuleStandard
                    DestinationUserInfo.AddAuthorization(userAuth.SecurityObjectID, userAuth.ServerGroupID, False, True, New CompuMaster.camm.WebManager.Notifications.NoNotifications(destCwm))
                Next
                For Each userAuth As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForUser In SourceUserInfo.AuthorizationsByRule.DenyRuleDevelopers
                    DestinationUserInfo.AddAuthorization(userAuth.SecurityObjectID, userAuth.ServerGroupID, True, True, New CompuMaster.camm.WebManager.Notifications.NoNotifications(destCwm))
                Next
                'TODO: Step 5: restore password of user
                'TODO: Step 6: restore sub-security-adjustments in administration area
            Catch ex As Exception
                'Silent rollback
                If DestinationUserInfo IsNot Nothing Then
                    DestinationUserInfo.LoginDeleted = True
                    DestinationUserInfo.Save(True)
                End If
                're-throw
                Throw New Exception("Exception on restore of user account; temporary partial user account data has been cleaned up (=removed) again)", ex)
            End Try
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub TextBoxConnectionString_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBoxConnectionStringCurrentDB.TextChanged
        MakeProceedingAvailable(False)
    End Sub

    Private Sub TextBoxCurrentDB_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        MakeProceedingAvailable(False)
    End Sub

    Private Sub TextBoxOldDB_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBoxConnectionStringSourceDB.TextChanged
        MakeProceedingAvailable(False)
    End Sub


    Private UnselectUsersWhochCannotBeRestoredProcessRunning As Boolean
    Private Sub UsersGrid_SelectionChanged(sender As Object, e As EventArgs) Handles UsersGrid.SelectionChanged
        If Not UnselectUsersWhochCannotBeRestoredProcessRunning Then CheckIfUsersCanBeRestored()
    End Sub

    Private Sub UnselectUsersWhochCannotBeRestored()
        UnselectUsersWhochCannotBeRestoredProcessRunning = True
        For Each r As DataGridViewRow In UsersGrid.SelectedRows
            If Not CBool(r.Cells("CanBeRestored").Value) Then r.Selected = False
        Next
        UnselectUsersWhochCannotBeRestoredProcessRunning = False
    End Sub

    Private Sub CheckIfUsersCanBeRestored()
        For Each r As DataGridViewRow In UsersGrid.SelectedRows
            Dim ID As Long = CLng(r.Cells("ID").Value)
            Dim success As CanBeRestored = CheckIfUserCanBeRestored(ID, CStr(r.Cells("Loginname").Value))
            If ID <> Nothing Then r.Cells("CanBeRestored").Value = success.CanBeRestored
            If Not success.CanBeRestored Then
                r.Cells("Message").Value = success.ErrorMessage
                r.DefaultCellStyle.BackColor = Color.Red
            End If
        Next

        If UsersGrid.SelectedRows.Count > 0 Then UnselectUsersWhochCannotBeRestored()
        If UsersGrid.SelectedRows.Count > 0 Then
            MakeProceedingAvailable(True)
            Me.ButtonRestore.Text = String.Format("Restore {0} user account(s)", UsersGrid.SelectedRows.Count)
        Else
            MakeProceedingAvailable(False)
        End If
    End Sub

    Private Function CheckIfUserCanBeRestored(userID As Long, username As String) As CanBeRestored
        Dim r As New CanBeRestored()
        Dim SourceUserID As Long = userID
        If CurrentCWMLoginNames.Contains(username) Then
            r.CanBeRestored = False
            r.ErrorMessage = "User with this loginname already exists in current database."
        ElseIf CurrentCWMUserIDs.Contains(userID) Then
            r.CanBeRestored = False
            r.ErrorMessage = "User with this ID already exists in current database."
        Else
            'destination table says okay
            Me.ButtonRestore.Tag = SourceUserID
            r.CanBeRestored = True
            r.ErrorMessage = String.Empty
        End If
        Return r
    End Function


    Private _CurrentCWMLoginNames As List(Of String)
    Public ReadOnly Property CurrentCWMLoginNames As List(Of String)
        Get
            If _CurrentCWMLoginNames Is Nothing Then _CurrentCWMLoginNames = GetListOfStringFromDataTable(CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(Me.CreateSqlCommandCurrentDB("SELECT loginname FROM dbo.benutzer"), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "Result"))
            Return _CurrentCWMLoginNames
        End Get
    End Property

    Private _CurrentCWMUserIDs As List(Of Long)
    Public ReadOnly Property CurrentCWMUserIDs As List(Of Long)
        Get
            If _CurrentCWMUserIDs Is Nothing Then _CurrentCWMUserIDs = GetListOfLongFromDataTable(CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(Me.CreateSqlCommandCurrentDB("SELECT ID FROM dbo.benutzer"), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "Result"))
            Return _CurrentCWMUserIDs
        End Get
    End Property

    Private Sub ResetCwmState()
        _CurrentCWMLoginNames = Nothing
        _CurrentCWMUserIDs = Nothing
    End Sub

    Private Function GetListOfStringFromDataTable(dt As DataTable) As List(Of String)
        Dim r As New List(Of String)
        For Each row As DataRow In dt.Rows
            r.Add(CStr(row(0)))
        Next
        Return r
    End Function

    Private Function GetListOfLongFromDataTable(dt As DataTable) As List(Of Long)
        Dim r As New List(Of Long)
        For Each row As DataRow In dt.Rows
            r.Add(CLng(row(0)))
        Next
        Return r
    End Function


    Private Class CanBeRestored
        Public CanBeRestored As Boolean
        Public ErrorMessage As String
    End Class
End Class
