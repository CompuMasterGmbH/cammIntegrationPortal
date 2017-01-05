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

Namespace CompuMaster.camm.WebManager.Setup.Pages

    ''' <summary>
    '''     Database update page (accessable via About page)
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class Update
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Private WithEvents DBSetup As New CompuMaster.camm.WebManager.Setup.DatabaseSetup("WebManager", "Web-Manager")
        Protected btnUpdateDB As System.Web.UI.WebControls.Button
        Protected LabelShowAppVersion As System.Web.UI.WebControls.Label
        Protected LabelShowDBVersionPatchManager As System.Web.UI.WebControls.Label
        Protected LabelShowDBVersionWMSystem As System.Web.UI.WebControls.Label
        Protected GridInstanceBuildNos As System.Web.UI.WebControls.DataGrid
        Protected TextboxUpdateToDBVersion As System.Web.UI.WebControls.TextBox

        Private Const SecurityObjectName As String = "System - User Administration - ServerSetup"

        Private Sub PageOnLoad(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            LabelShowAppVersion.Visible = True
            LabelShowAppVersion.Text = cammWebManager.System_Version_Ex.ToString()
            LabelShowDBVersionPatchManager.Visible = True
            LabelShowDBVersionPatchManager.Text = DBSetup.GetCurrentDBBuildNo(cammWebManager.ConnectionString).ToString
            Dim CurrentDBBuild As Version = Setup.DatabaseUtils.Version(cammWebManager, False)
            LabelShowDBVersionWMSystem.Visible = True
            LabelShowDBVersionWMSystem.Text = CurrentDBBuild.ToString()

            cammWebManager.PageTitle = "Administration - Software update"
            Dim RedirectToURL As String = Nothing
            Dim ExecutionAllowed As Boolean
            If cammWebManager.System_CheckForAccessAuthorization_NoRedirect(SecurityObjectName, RedirectToURL) = CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_DBResults.AccessGranted Then
                ExecutionAllowed = True
            Else
                Try
                    If cammWebManager.System_GetServerID(cammWebManager.CurrentServerIdentString) = 0 Then
                        Throw New Exception("Server is misconfigured")
                    End If
                Catch
                    'camm Web-Manager hasn't been configured correctly, we have to allow a reset
                    ExecutionAllowed = True
                End Try
            End If
            If ExecutionAllowed = False Then
                Dim RedirectionCause As String = "This has been because the access for webmanager database updater page hasn't been granted"
                Dim RequestDetails As New Collections.Specialized.NameValueCollection
                RequestDetails("SecurityObjectName") = SecurityObjectName
                cammWebManager.Log.Warn(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization & vbNewLine & "Security object: " & SecurityObjectName & vbNewLine & "This is the camm Web-Manager database update script and that's why this is a very security related item.")
                Response.Clear()
                Response.Write(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization)
                Response.End()
            End If

            Dim CwmInstances As DataTable = LoadCwmInstancesList()

            'Load instances list from table and show in datagrid
            If Not GridInstanceBuildNos Is Nothing Then
                If CwmInstances.Rows.Count = 0 Then
                    Dim row As DataRow = CwmInstances.NewRow
                    row("Instance location") = "No information available - instances report their build no. beginning with build 152."
                    row("Assembly Build No") = Setup.ApplicationUtils.Version.Build
                    row("Application compatible with build no") = Setup.ApplicationUtils.Version.Build
                    row("Reported on") = Now
                    CwmInstances.Rows.Add(row)
                End If
                GridInstanceBuildNos.DataSource = CwmInstances
                GridInstanceBuildNos.DataBind()
                GridInstanceBuildNos.HeaderStyle.Font.Bold = True
            End If

            'Recommend max. build no for update
            If Not TextboxUpdateToDBVersion Is Nothing AndAlso Page.IsPostBack = False Then
                Dim LowestFoundBuild As Integer = Setup.DatabaseSetup.LastBuildVersionInSetupFiles.Build
                For MyCounter As Integer = 0 To CwmInstances.Rows.Count - 1
                    LowestFoundBuild = System.Math.Min(LowestFoundBuild, CType(CwmInstances.Rows(MyCounter)("Application compatible with build no"), Integer))
                Next
                'Show LastAvailablePatchBuild if higher than current db build else the current db build
                If LowestFoundBuild < CurrentDBBuild.Build Then
                    TextboxUpdateToDBVersion.Text = CurrentDBBuild.Build.ToString
                Else
                    TextboxUpdateToDBVersion.Text = LowestFoundBuild.ToString
                End If
            End If
        End Sub
        ''' <summary>
        ''' Load the list of reported camm Web-Manager instances accessing the database since the last update
        ''' </summary>
        Private Function LoadCwmInstancesList() As DataTable
            Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString), "SELECT ValueNText AS [Instance location], ValueInt As [Assembly Build No], Cast (ValueDecimal as int) As [Application compatible with build no], ValueDateTime As [Reported on] FROM System_GlobalProperties WHERE PropertyName LIKE 'AppInstance_%' ORDER BY ValueDecimal DESC", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function
        ''' <summary>
        ''' Reset the list of camm Web-Manager instances
        ''' </summary>
        Private Sub ResetCwmInstancesList()
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString), "DELETE FROM System_GlobalProperties WHERE PropertyName LIKE 'AppInstance_%'", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        Private Sub PageOnPreRender(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            Response.Write(DBSetup.GetLogData().Replace(vbNewLine, "<br>"))
        End Sub

        Public Function GetConnectionString() As Object
            Return ConnectionString()
        End Function

        Private Function ConnectionString() As String
            Return cammWebManager.ConnectionString & ";Pooling=false;" 'this was important, but why again? 2005-12-02_jw
        End Function

        Sub btnUpdateDB_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim Replacements As New System.Collections.Specialized.NameValueCollection
            Response.Buffer = False
            Response.Write("<html><body style=""font-family: Arial;"">")
            Try
                If TextboxUpdateToDBVersion Is Nothing Then
                    DBSetup.DoUpdates(ConnectionString, Replacements)
                Else
                    Dim UpdateToBuildNo As Integer
                    UpdateToBuildNo = Utils.TryCInt(TextboxUpdateToDBVersion.Text, Integer.MaxValue)
                    DBSetup.DoUpdates(ConnectionString, Replacements, UpdateToBuildNo)
                End If
                ResetCwmInstancesList()
                Response.Write("<p><strong>Setup: All required updates have been installed successfully</strong></p>")
            Catch ex As Exception
                Response.Write("Setup: Update-Error: Unhandled exception in DoUpdates call: " & Utils.HTMLEncodeLineBreaks(ex.ToString))
                Response.Write(Utils.HTMLEncodeLineBreaks("Warnings: " & vbNewLine & DBSetup.Warnings & vbNewLine & vbNewLine & DBSetup.GetLogData))
            End Try
            Response.Write("<p><strong>ATTENTION: all servers and web applications must be restarted to reset local caches (use e.g. iisreset.exe or re-save every web.config).</strong></p>")
            Response.Write("</font></body></html>")
            Response.End()

        End Sub

        Public Sub DisplayWarning() Handles DBSetup.WarningsQueueChanged
            Response.Write("<p style=""color:red""><b>Update Warning</b><br>" & DBSetup.Warnings.Replace(vbNewLine, "<br>") & "</p>")
            DBSetup.Warnings = Nothing
        End Sub

        Public Sub DisplayNewStatus_ProgressTask() Handles DBSetup.ProgressTaskStatusChanged
            Response.Write(("<p>Progress Task: " & DBSetup.ProgressOfTasks.CurrentStepTitle & " (Step " & DBSetup.ProgressOfTasks.CurrentStepNumber & " of " & DBSetup.ProgressOfTasks.StepsTotal & ")" & vbNewLine).Replace(vbNewLine, "<br>"))
        End Sub

        Public Sub DisplayNewStatus_Step() Handles DBSetup.StepStatusChanged
            Response.Write("Step: " & DBSetup.CurrentStepTitle.Replace(vbNewLine, "<br>") & "<br>")
        End Sub

        Public Sub DisplayNewStatus_ProgressStep() Handles DBSetup.ProgressStepStatusChanged
            Response.Write(("Progress Step: " & DBSetup.ProgressOfSteps.CurrentStepTitle & " (Step " & DBSetup.ProgressOfSteps.CurrentStepNumber & " of " & DBSetup.ProgressOfSteps.StepsTotal & ")" & vbNewLine).Replace(vbNewLine, "<br>"))
        End Sub

        Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
            Server.ScriptTimeout = 900 '15 minutes
        End Sub
    End Class

End Namespace