'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Setup.Pages

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("use Webservice to install the camm Web-Manager database", True)>
    Public Class Install
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Private WithEvents DBSetup As New CompuMaster.camm.WebManager.Setup.DatabaseSetup("WebManager", "Web-Manager")
        Protected btnUpdateDB As System.Web.UI.WebControls.Button

        ' Important Changes 2004-11-10 by Markus Kottenhahn
        ' The two checkboxes CheckBoxKeepExistingData and CheckBoxNewDB were replaced by two RadioButtons
        ' due to the current Windows Installer app. The checkboxes appeared to make the thing much more
        ' complicated, as they only made sense if they were both checked (for making a new install) or if
        ' CheckBoxNewDB was not checked, the CheckBoxExistingData did not matter.
        ' If any app is running into problems with these changes they can either be undone or easily 
        ' integrated into the "new" interpretation.
        Protected LabelShowDebugLevel As System.Web.UI.WebControls.Label
        Protected LabelShowAppVersion As System.Web.UI.WebControls.Label
        Protected LabelShowDBVersion As System.Web.UI.WebControls.Label
        Protected OptionInstallNewDB As System.Web.UI.WebControls.RadioButton
        Protected OptionUpdateExistingDB As System.Web.UI.WebControls.RadioButton
        Protected CheckUseExistingButEmptyDatabase As System.Web.UI.WebControls.CheckBox
        Protected TextBoxDBCatalog As System.Web.UI.WebControls.TextBox
        Protected TextBoxDBServer As System.Web.UI.WebControls.TextBox
        Protected TextBoxAuthUser As System.Web.UI.WebControls.TextBox
        Protected TextBoxAuthPassword As System.Web.UI.WebControls.TextBox
        Protected TextBoxAuthUserAdmin As System.Web.UI.WebControls.TextBox
        Protected TextBoxAuthPasswordAdmin As System.Web.UI.WebControls.TextBox
        Protected TextBoxProtocol As System.Web.UI.WebControls.TextBox
        Protected TextBoxServerName As System.Web.UI.WebControls.TextBox
        Protected TextBoxPort As System.Web.UI.WebControls.TextBox
        Protected TextBoxServerIP As System.Web.UI.WebControls.TextBox
        Protected TextBoxSGroupTitle As System.Web.UI.WebControls.TextBox
        Protected TextBoxSGroupNavTitle As System.Web.UI.WebControls.TextBox
        Protected TextBoxCompanyURL As System.Web.UI.WebControls.TextBox
        Protected TextBoxSGroupContact As System.Web.UI.WebControls.TextBox
        Protected TextBoxCompanyName As System.Web.UI.WebControls.TextBox
        Protected TextBoxCompanyFormerName As System.Web.UI.WebControls.TextBox

        Private Function AnyDataEntered() As Boolean
            ' This simple function tells us, if any data has been entered into the form. Sometimes this
            ' information could be really useful, if you don't want to get stuck with validation controls
            Return _
                 TextBoxDBServer.Text <> "" Or TextBoxDBCatalog.Text <> "" Or
                 TextBoxAuthUser.Text <> "" Or TextBoxAuthPassword.Text <> "" Or
                 TextBoxServerIP.Text <> "" Or TextBoxProtocol.Text <> "" Or
                 TextBoxServerName.Text <> "" Or TextBoxPort.Text <> "" Or
                 TextBoxServerIP.Text <> "" Or TextBoxSGroupTitle.Text <> "" Or
                 TextBoxSGroupNavTitle.Text <> "" Or TextBoxCompanyURL.Text <> "" Or
                 TextBoxSGroupContact.Text <> "" Or TextBoxCompanyName.Text <> "" Or
                 TextBoxCompanyFormerName.Text <> ""
        End Function

        Public Function IsDatabaseServerAccessible() As Boolean
            ' This function checks if the given database server exists and the user is allowed to open the 
            ' standard db, i.e. the function returns, if the db server is accessible anyway. This does not
            ' tell us anything about the (probably not yet existing) database.
            Dim MyResult As Boolean = True
            Dim conn As New SqlConnection
            conn.ConnectionString = ConnectionStringServerAdministration()
            Try
                conn.Open()
            Catch
                MyResult = False
            Finally
                If conn.State <> ConnectionState.Closed Then conn.Close()
                If Not conn Is Nothing Then conn.Dispose()
            End Try
            Return MyResult
        End Function

        Public Function DatabaseExists() As Boolean
            ' This function checks if the given database (i.e. the files, not only the server) exists And _
            ' the user is allowed to open this db. The technical difference ist only, that the name of the
            ' database is given in the ConnectionString
            Dim MyResult As Boolean = True
            Dim conn As New SqlConnection
            conn.ConnectionString = ConnectionString()
            Try
                conn.Open()
            Catch
                MyResult = False
            Finally
                If conn.State <> ConnectionState.Closed Then conn.Close()
                If Not conn Is Nothing Then conn.Dispose()
            End Try
            Return MyResult
        End Function

        Private Const SecurityObjectName As String = "System - User Administration - ServerSetup"

        Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Server.ScriptTimeout = 900 '15 minutes
            DBSetup.UpdatesOnly = False
        End Sub

        Private Sub PageOnPreRender(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            Response.Write(DBSetup.GetLogData.Replace(vbNewLine, "<br>" & vbNewLine))
        End Sub

        Private Function Setup_IsUserAuthorized(ByVal SecurityObjectName As String) As Boolean
            ' This function is some kind of w?rgaround in fact that System_IsUserauthorizedForApp cannot
            ' give a proper result when the database has not yet been installed
            Dim MyResult As Boolean
            Dim MyBuffer As Integer
            Dim MyDBConn As New SqlConnection
            Dim MyCmd As New SqlCommand

            Dim ServerName As String = cammWebManager.CurrentServerIdentString

            'Create connection
            MyDBConn.ConnectionString = ConnectionString()
            Try
                MyDBConn.Open()
                'Get parameter value and append parameter
                MyCmd.CommandText = "Public_UserIsAuthorizedForApp"
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ReturnValue", SqlDbType.Int)
                MyCmd.Parameters("@ReturnValue").Direction = ParameterDirection.ReturnValue
                If Not cammWebManager.IsLoggedOn Then
                    MyCmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = DBNull.Value
                Else
                    MyCmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = cammWebManager.CurrentUserLoginName
                End If
                MyCmd.Parameters.Add("@WebApplication", SqlDbType.NVarChar).Value = SecurityObjectName
                MyCmd.Parameters.Add("@ServerIP", SqlDbType.VarChar).Value = ServerName
                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                MyCmd.ExecuteNonQuery()
                MyBuffer = CType(MyCmd.Parameters("@ReturnValue").Value, Integer)
                MyResult = (MyBuffer = 1)
            Catch ex As SqlException
                Select Case ex.Number
                    Case 4060
                        ' If the database is not yet created, error 4060 will be raised. But this exception
                        ' will(also) be raised if the given user has just no authorization for accessing the
                        ' database (if it exists). So we have to make sure that the server itself is
                        ' accessible AND the database does not yet exist.
                        MyResult = (IsDatabaseServerAccessible() And Not DatabaseExists())
                    Case 2812
                        ' This means that the database exists but the stored procedure  was not found. So 
                        ' Authorization can be Ok, as the logical database is not yet installed.
                        MyResult = True
                    Case Else
                        ' In any other case it might be better to deny authorization anyway...
                        MyResult = False
                End Select
            Finally
                If Not MyCmd Is Nothing Then MyCmd.Dispose()
                If Not MyDBConn Is Nothing Then
                    If MyDBConn.State <> ConnectionState.Closed Then MyDBConn.Close()
                    MyDBConn.Dispose()
                End If
            End Try
            Return MyResult
        End Function

        Private Sub PageOnLoad(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            ' Show the debug level if it makes sense
            If DBSetup.DebugLevel > WMSystem.DebugLevels.NoDebug Then
                LabelShowDebugLevel.Visible = True
                LabelShowDebugLevel.Text = "<br>DebugLevel: " & DBSetup.DebugLevel
            End If

            Dim ExecutionAllowed As Boolean
            Try
                ' In any case we need to find out if the user is allowed to
                ' access the Web-Manager anyway, if it is already installed.
                Dim IsAuthorized As Boolean = Setup_IsUserAuthorized(SecurityObjectName)

                If IsAuthorized Then
                    ExecutionAllowed = True
                Else
                    Try
                        If cammWebManager.System_GetServerID(cammWebManager.CurrentServerIdentString) = 0 Then
                            Throw New Exception("Server is misconfigured")
                        End If
                    Catch ex As Exception
                        ExecutionAllowed = True
                        Trace.Warn("Setup: PageOnLoad", "camm Web-Manager hasn't been configured correctly, we have to allow a reset 5A")
                    End Try
                End If
                If Not ExecutionAllowed Then
                    Dim RequestDetails As New Collections.Specialized.NameValueCollection
                    RequestDetails("SecurityObjectName") = SecurityObjectName
                    cammWebManager.Log.Warn("Setup", CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization & vbNewLine & "Security object: " & SecurityObjectName & vbNewLine & "This is the camm Web-Manager database update script and that's why this is a very security related item.")
                End If
            Catch
                Trace.Warn("Setup: PageOnLoad", CType(IIf(AnyDataEntered, "Access denied", "Some data missing"), String))
            End Try
        End Sub

        Private Function GetConnectionString_ServerAdministration() As Object
            Return ConnectionStringServerAdministration()
        End Function

        Private Function ConnectionStringServerAdministration() As String
            Return (
                 "SERVER=" & Me.TextBoxDBServer.Text & ";" &
                 "PWD=" & Me.TextBoxAuthPassword.Text & ";" &
                 "UID=" & Me.TextBoxAuthUser.Text & ";" &
                 "Pooling=false;")
        End Function

        Public Function GetConnectionString_ServerAdministration_sa() As Object
            Return ConnectionStringServerAdministrationSA()
        End Function

        Private Function ConnectionStringServerAdministrationSA() As String
            If TextBoxAuthUserAdmin.Text <> "" And TextBoxAuthPasswordAdmin.Text <> "" Then
                Return (
                     "SERVER=" & Me.TextBoxDBServer.Text & ";" &
                     "PWD=" & Me.TextBoxAuthPasswordAdmin.Text & ";" &
                     "UID=" & Me.TextBoxAuthUserAdmin.Text & ";" &
                     "Pooling=false;")
            Else
                Return ""
            End If

        End Function

        Public Function GetConnectionString() As Object
            Return ConnectionString()
        End Function

        Private Function ConnectionString() As String
            Return (
                 "SERVER=" & Me.TextBoxDBServer.Text & ";" &
                 "PWD=" & Me.TextBoxAuthPassword.Text & ";" &
                 "UID=" & Me.TextBoxAuthUser.Text & ";" &
                 "DATABASE=" & Me.TextBoxDBCatalog.Text & ";" &
                 "Pooling=false;")
        End Function

        Sub btnCheckDBVersion_Click(ByVal sender As Object, ByVal e As EventArgs)
            LabelShowDBVersion.Visible = True
            LabelShowDBVersion.Text = "<br>"
            Try
                LabelShowDBVersion.Text += Setup.DatabaseUtils.Version(cammWebManager, False).ToString()
            Catch
                LabelShowDBVersion.Text += "No version information available or maybe there is a bug in cammWebManager.System_DBVersion_Ex"
            End Try
        End Sub

        Sub btnUpdateDB_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim DatabaseOk As Boolean

            If OptionInstallNewDB.Checked Then
                ' In this case we will (re-)create the database anyway. Here we have to be sure that we're
                ' allowed to call a  DROP and a CREATE DATABASE, and if this fails with primary given user, _
                ' we may try it again using the user sa or someone with equal authorizations. In this case 
                ' we should also take care to grant the original user as dbo  of the new database. 
                Try
                    DatabaseOk = DBSetup.CreateDatabase(ConnectionString(), ConnectionStringServerAdministration(), TextBoxDBCatalog.Text, CheckUseExistingButEmptyDatabase.Checked, False, ConnectionStringServerAdministrationSA(), TextBoxAuthUser.Text, TextBoxAuthPassword.Text, TextBoxAuthUserAdmin.Text, TextBoxAuthPasswordAdmin.Text)
                Catch ex As Exception
                    ' What could happen if Creation process failed?
                    DatabaseOk = False
                    Trace.Warn("Setup: btnUpdate_Click:", "Unhandled exception in CreateDatabase call: " & ex.Message)
                End Try
            Else
                'Existing database - reinitialize it!
                Try
                    DatabaseOk = DBSetup.InitDatabase(ConnectionString())
                Catch ex As Exception
                    ' What could happen if Creation process failed?
                    DatabaseOk = False
                    Trace.Warn("Setup: btnUpdate_Click:", "Unhandled exception in CreateDatabase call: " & ex.Message)
                End Try
            End If

            ' Now update the existing db
            If DatabaseOk Then
                ' If the db access does not appear to be Ok, it makes no sense trying to update it...
                Dim Replacements As System.Collections.Specialized.NameValueCollection
                Replacements = DBSetup.GetWebManagerReplacements(TextBoxServerIP.Text, TextBoxProtocol.Text, TextBoxServerName.Text, TextBoxPort.Text, TextBoxSGroupTitle.Text, TextBoxSGroupNavTitle.Text, TextBoxSGroupContact.Text, TextBoxCompanyName.Text, TextBoxCompanyFormerName.Text, TextBoxCompanyURL.Text)
                Try
                    DBSetup.DoUpdates(ConnectionString, Replacements)
                Catch ex As Exception
                    Trace.Warn("Setup: Update-Error: ", "Unhandled exception in DoUpdates call: " & ex.ToString)
                End Try
            End If
        End Sub

        Public Sub DisplayWarning() Handles DBSetup.WarningsQueueChanged
            DBSetup.WriteToLog("<span class=""red""><b>Install Warning:</b> " & DBSetup.Warnings.Replace(vbNewLine, "<br>") & "</span>")
            DBSetup.Warnings = Nothing
        End Sub

        Public Sub DisplayNewStatus_ProgressTask() Handles DBSetup.ProgressTaskStatusChanged
            Dim Msg As String = "Progress Task: " & DBSetup.ProgressOfTasks.CurrentStepTitle
            Msg += " (Step " & DBSetup.ProgressOfTasks.CurrentStepNumber.ToString
            Msg += " of " & DBSetup.ProgressOfTasks.StepsTotal.ToString & ")"
            DBSetup.WriteToLog(Msg & vbNewLine)
        End Sub

        Public Sub DisplayNewStatus_Step() Handles DBSetup.StepStatusChanged
            DBSetup.WriteToLog("Step: " & DBSetup.CurrentStepTitle & vbNewLine)
        End Sub

        Public Sub DisplayNewStatus_ProgressStep() Handles DBSetup.ProgressStepStatusChanged
            DBSetup.WriteToLog("Progress Step: " & DBSetup.ProgressOfSteps.CurrentStepTitle & " (Step " & DBSetup.ProgressOfSteps.CurrentStepNumber & " of " & DBSetup.ProgressOfSteps.StepsTotal & ")")
        End Sub

    End Class

End Namespace