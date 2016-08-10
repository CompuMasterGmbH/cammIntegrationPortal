Option Explicit On
Option Strict On

Public Class Main

    Private Sub ButtonGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonGo.Click
        Try
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
            Dim SourceUsers As DataTable = CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(CreateSqlCommandSourceDB("select * from dbo.benutzer where loginname like N'" & Me.TextBoxLoginName.Text.Replace("'", "''").Replace("*", "%") & "'"), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "users")
            Me.UsersGrid.DataSource = SourceUsers
            Dim SourceUsersCount As Integer = SourceUsers.Rows.Count
            If SourceUsersCount < 1 Then
                MsgBox("User not found!", MsgBoxStyle.Exclamation)
                MakeProceedingAvailable(False)
            ElseIf SourceUsersCount > 1 Then
                MsgBox("Multiple users with same/similar loginname", MsgBoxStyle.Exclamation)
                MakeProceedingAvailable(False)
            Else
                'we can proceed
                Dim SourceUserID As Long = CType(SourceUsers.Rows(0)("ID"), Long)
                Dim SourceUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Me.SourceCwmInstance.System_GetUserInfo(SourceUserID)
                If Not IsNothing(CompuMaster.Data.DataQuery.AnyIDataProvider.ExecuteScalar(Me.CreateSqlCommandCurrentDB("SELECT ID FROM dbo.benutzer WHERE loginname = N'" & Me.TextBoxLoginName.Text.Replace("'", "''") & "'"), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)) Then
                    MakeProceedingAvailable(False)
                    MsgBox("User with this loginname already exists in current database", MsgBoxStyle.Exclamation)
                ElseIf Not IsNothing(CompuMaster.Data.DataQuery.AnyIDataProvider.ExecuteScalar(Me.CreateSqlCommandCurrentDB("SELECT ID FROM dbo.benutzer WHERE ID = " & SourceUserID), CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)) Then
                    MakeProceedingAvailable(False)
                    MsgBox("User with this ID already exists in current database", MsgBoxStyle.Exclamation)
                Else
                    'destination table says okay
                    Me.ButtonRestore.Tag = SourceUserID
                    MakeProceedingAvailable(True)
                    MsgBox("User account is ready to be restored", MsgBoxStyle.Information)
                End If
            End If
        Catch ex As Exception
            MakeProceedingAvailable(False)
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub

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
            Dim DestinationUserID As Long = CType(Me.ButtonRestore.Tag, Long)
            If DestinationUserID = 0L Then Throw New InvalidOperationException("UserID must be not zero")
            MakeProceedingAvailable(False)
            RestoreUser(DestinationUserID, Me.SourceCwmInstance, Me.DestinationCwmInstance)
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

    Private Sub RestoreUser(ByVal sourceUserID As Long, ByVal sourceCwm As CompuMaster.camm.WebManager.WMSystem, ByVal destCwm As CompuMaster.camm.WebManager.WMSystem)
        Try
            Dim SourceUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = sourceCwm.System_GetUserInfo(sourceUserID)
            Dim DestinationUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = destCwm.System_GetUserInfo(sourceUserID)
            If DestinationUserInfo IsNot Nothing Then
                Throw New Exception("User already exists in current database")
            End If

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
                MsgBox("User restore finished successfully", MsgBoxStyle.Information)
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
End Class
