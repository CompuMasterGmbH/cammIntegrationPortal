<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ButtonSelectExportTarget = New System.Windows.Forms.Button()
        Me.ButtonExport = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxExportTargetFile = New System.Windows.Forms.TextBox()
        Me.TextBoxSql = New System.Windows.Forms.TextBox()
        Me.TextBoxConnectionStringCurrentDB = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButtonSelectExportTarget
        '
        Me.ButtonSelectExportTarget.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonSelectExportTarget.Location = New System.Drawing.Point(638, 38)
        Me.ButtonSelectExportTarget.Name = "ButtonSelectExportTarget"
        Me.ButtonSelectExportTarget.Size = New System.Drawing.Size(31, 20)
        Me.ButtonSelectExportTarget.TabIndex = 3
        Me.ButtonSelectExportTarget.Text = "..."
        Me.ButtonSelectExportTarget.UseVisualStyleBackColor = True
        '
        'ButtonExport
        '
        Me.ButtonExport.Location = New System.Drawing.Point(499, 64)
        Me.ButtonExport.Name = "ButtonExport"
        Me.ButtonExport.Size = New System.Drawing.Size(170, 31)
        Me.ButtonExport.TabIndex = 4
        Me.ButtonExport.Text = "&Execute export"
        Me.ButtonExport.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(13, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(258, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Connection string to SQL server for current database:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(13, 41)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(80, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Export file path:"
        '
        'TextBoxExportTargetFile
        '
        Me.TextBoxExportTargetFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxExportTargetFile.Location = New System.Drawing.Point(277, 38)
        Me.TextBoxExportTargetFile.Name = "TextBoxExportTargetFile"
        Me.TextBoxExportTargetFile.Size = New System.Drawing.Size(355, 20)
        Me.TextBoxExportTargetFile.TabIndex = 9
        Me.TextBoxExportTargetFile.Text = Global.My.MySettings.Default.ExportPath
        '
        'TextBoxSql
        '
        Me.TextBoxSql.Location = New System.Drawing.Point(782, 502)
        Me.TextBoxSql.Multiline = True
        Me.TextBoxSql.Name = "TextBoxSql"
        Me.TextBoxSql.Size = New System.Drawing.Size(430, 262)
        Me.TextBoxSql.TabIndex = 11
        Me.TextBoxSql.Visible = False
        '
        'TextBoxConnectionStringCurrentDB
        '
        Me.TextBoxConnectionStringCurrentDB.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxConnectionStringCurrentDB.Location = New System.Drawing.Point(277, 12)
        Me.TextBoxConnectionStringCurrentDB.Name = "TextBoxConnectionStringCurrentDB"
        Me.TextBoxConnectionStringCurrentDB.Size = New System.Drawing.Size(392, 20)
        Me.TextBoxConnectionStringCurrentDB.TabIndex = 5
        Me.TextBoxConnectionStringCurrentDB.Text = Global.My.MySettings.Default.ConnectionStringDestinationDB
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 98)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(681, 22)
        Me.StatusStrip1.TabIndex = 12
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel
        '
        Me.ToolStripStatusLabel.Name = "ToolStripStatusLabel"
        Me.ToolStripStatusLabel.Size = New System.Drawing.Size(69, 17)
        Me.ToolStripStatusLabel.Text = "Not started."
        '
        'Timer1
        '
        Me.Timer1.Interval = 250
        '
        'Main
        '
        Me.AcceptButton = Me.ButtonSelectExportTarget
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(681, 120)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TextBoxSql)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxExportTargetFile)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxConnectionStringCurrentDB)
        Me.Controls.Add(Me.ButtonExport)
        Me.Controls.Add(Me.ButtonSelectExportTarget)
        Me.MinimumSize = New System.Drawing.Size(697, 159)
        Me.Name = "Main"
        Me.Text = "User Export from CWM database"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ButtonSelectExportTarget As System.Windows.Forms.Button
    Friend WithEvents ButtonExport As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxConnectionStringCurrentDB As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBoxExportTargetFile As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxSql As System.Windows.Forms.TextBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel As ToolStripStatusLabel
    Friend WithEvents Timer1 As Timer
End Class
