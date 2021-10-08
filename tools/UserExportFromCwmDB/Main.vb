Option Explicit On
Option Strict On

Public Class Main

    Private MaxExecDateTime As DateTime = New DateTime(2021, 10, 31)

    Private Sub ButtonSelectExportTarget_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonSelectExportTarget.Click
        Try
            Dim SaveAsDialog As New SaveFileDialog() With {
                .FileName = CompuMaster.Data.Utils.StringNotEmptyOrAlternativeValue(
                    Me.TextBoxExportTargetFile.Text,
                    System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "User export " & Now.ToString("yyyy-MM-dd") & ".xlsx")),
                .Filter = "Excel-Dateien (*.xlsx)|.xlsx",
                .AddExtension = True,
                .DefaultExt = ".xlsx",
                .OverwritePrompt = True,
                .Title = "Export data to file path",
                .AutoUpgradeEnabled = True,
                .DereferenceLinks = True,
                .InitialDirectory = System.IO.Path.GetDirectoryName(.FileName)
            }
            If SaveAsDialog.ShowDialog(Me) = DialogResult.OK Then
                Me.TextBoxExportTargetFile.Text = SaveAsDialog.FileName
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub Main_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text &= String.Format(" V{0}", Global.My.Application.Info.Version.ToString(3))
        Me.RefreshStatus()
    End Sub

    Private Sub ButtonExport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonExport.Click
        Try
            If Me.TextBoxExportTargetFile.Text = Nothing Then Throw New Exception("Missing export path")
            If Me.TextBoxConnectionStringCurrentDB.Text = Nothing Then Throw New Exception("Missing connection string")
            If Now > maxExecDateTime Then Throw New NotSupportedException("Ausführungszeit-Limit erreicht. Bitte konsultieren Sie den Hersteller dieses Tools.")

            Dim ExportPath As String = System.IO.Path.Combine(System.Environment.CurrentDirectory, Me.TextBoxExportTargetFile.Text)
            Dim cammWebManager As New CompuMaster.camm.WebManager.WMSystem(Me.TextBoxConnectionStringCurrentDB.Text)

            Me.StartedOn = Now
            Me.Timer1.Enabled = True
            Me.Timer1.Start()
            Me.Cursor = Cursors.WaitCursor
            Me.RefreshStatus()
            My.Application.DoEvents()
            Dim Result As DataTable = CompuMaster.camm.WebManager.Administration.Export.Users(cammWebManager, cammWebManager.System_GetUserInfos())
            CompuMaster.Data.XlsEpplus.WriteDataTableToXlsFileAndFirstSheet(ExportPath, Result)
            CompuMaster.Data.Csv.WriteDataTableToCsvFile(ExportPath.Replace(".xlsx", ".csv"), Result)
            Me.StartedOn = Nothing
            Me.RefreshStatus()

            Me.Cursor = Cursors.Default
            MsgBox("Export completed with " & Now.Subtract(Me.StartedOn).TotalSeconds & "s", MsgBoxStyle.Information)
        Catch ex As NotSupportedException
            Me.Cursor = Cursors.Default
            MsgBox(ex.Message, MsgBoxStyle.Critical)
            Me.StartedOn = Nothing
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MsgBox(ex.ToString, MsgBoxStyle.Critical)
            Me.StartedOn = Nothing
        End Try

        Me.Timer1.Stop()
        Me.Timer1.Enabled = False
        Me.RefreshStatus()

    End Sub

    Property StartedOn As DateTime = Nothing

    Private Sub RefreshStatus()
        If StartedOn = Nothing Then
            Me.ToolStripStatusLabel.Text = "Not started."
        Else
            Me.ToolStripStatusLabel.Text = "Started on " & Me.StartedOn.ToShortTimeString & ", running already for " & Now.Subtract(Me.StartedOn).TotalSeconds.ToString("#,##0.0") & "s"
        End If
        Me.Refresh()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Me.RefreshStatus()
    End Sub

End Class
