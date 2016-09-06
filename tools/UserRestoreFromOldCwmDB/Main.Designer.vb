<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.UsersGrid = New System.Windows.Forms.DataGridView()
        Me.TextBoxLoginName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ButtonGo = New System.Windows.Forms.Button()
        Me.ButtonRestore = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxConnectionStringSourceDB = New System.Windows.Forms.TextBox()
        Me.TextBoxSql = New System.Windows.Forms.TextBox()
        Me.TextBoxConnectionStringCurrentDB = New System.Windows.Forms.TextBox()
        Me.lblCount = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.UsersGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'UsersGrid
        '
        Me.UsersGrid.AllowUserToOrderColumns = True
        Me.UsersGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UsersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.UsersGrid.Location = New System.Drawing.Point(12, 181)
        Me.UsersGrid.Name = "UsersGrid"
        Me.UsersGrid.Size = New System.Drawing.Size(657, 234)
        Me.UsersGrid.TabIndex = 0
        '
        'TextBoxLoginName
        '
        Me.TextBoxLoginName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxLoginName.Location = New System.Drawing.Point(277, 128)
        Me.TextBoxLoginName.MaxLength = 99999
        Me.TextBoxLoginName.Multiline = True
        Me.TextBoxLoginName.Name = "TextBoxLoginName"
        Me.TextBoxLoginName.Size = New System.Drawing.Size(313, 20)
        Me.TextBoxLoginName.TabIndex = 1
        Me.TextBoxLoginName.Text = "admin*"
        Me.ToolTip1.SetToolTip(Me.TextBoxLoginName, "e.g. ""admin*"" or ""admin|administrator|root""")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 131)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(262, 39)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Deleted single user loginname which shall be restored " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(supports wildchar ""*"") o" &
    "r list of loginnames separated " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "by pipe char ""|"" (exact name, no wildchar suppo" &
    "rt)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'ButtonGo
        '
        Me.ButtonGo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonGo.Location = New System.Drawing.Point(596, 128)
        Me.ButtonGo.Name = "ButtonGo"
        Me.ButtonGo.Size = New System.Drawing.Size(73, 20)
        Me.ButtonGo.TabIndex = 3
        Me.ButtonGo.Text = "Search user"
        Me.ButtonGo.UseVisualStyleBackColor = True
        '
        'ButtonRestore
        '
        Me.ButtonRestore.Location = New System.Drawing.Point(277, 154)
        Me.ButtonRestore.Name = "ButtonRestore"
        Me.ButtonRestore.Size = New System.Drawing.Size(170, 19)
        Me.ButtonRestore.TabIndex = 4
        Me.ButtonRestore.Text = "Restore"
        Me.ButtonRestore.UseVisualStyleBackColor = True
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
        Me.Label4.Size = New System.Drawing.Size(233, 26)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Connection string to SQL server for old/backup " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "database containing the restore " &
    "data:"
        '
        'TextBoxConnectionStringSourceDB
        '
        Me.TextBoxConnectionStringSourceDB.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxConnectionStringSourceDB.Location = New System.Drawing.Point(277, 38)
        Me.TextBoxConnectionStringSourceDB.Name = "TextBoxConnectionStringSourceDB"
        Me.TextBoxConnectionStringSourceDB.Size = New System.Drawing.Size(392, 20)
        Me.TextBoxConnectionStringSourceDB.TabIndex = 9
        Me.TextBoxConnectionStringSourceDB.Text = Global.My.MySettings.Default.ConnectionStringSourceDB
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
        'lblCount
        '
        Me.lblCount.AutoSize = True
        Me.lblCount.Location = New System.Drawing.Point(13, 131)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(0, 13)
        Me.lblCount.TabIndex = 12
        '
        'Panel1
        '
        Me.Panel1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.PictureBox1)
        Me.Panel1.Location = New System.Drawing.Point(74, 78)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(544, 44)
        Me.Panel1.TabIndex = 15
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(40, 1)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(490, 26)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Restoration of user accounts includes profile data, memberships, authorization" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "N" &
    "ot (yet) supported is the restoration of: user password, sub security adjustment" &
    "s for administration area" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(2, 2)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(32, 29)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 15
        Me.PictureBox1.TabStop = False
        '
        'Main
        '
        Me.AcceptButton = Me.ButtonGo
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(681, 427)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.lblCount)
        Me.Controls.Add(Me.TextBoxSql)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxConnectionStringSourceDB)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxConnectionStringCurrentDB)
        Me.Controls.Add(Me.ButtonRestore)
        Me.Controls.Add(Me.ButtonGo)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TextBoxLoginName)
        Me.Controls.Add(Me.UsersGrid)
        Me.MinimumSize = New System.Drawing.Size(689, 454)
        Me.Name = "Main"
        Me.Text = "User Restore from old CWM database"
        CType(Me.UsersGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents UsersGrid As System.Windows.Forms.DataGridView
    Friend WithEvents TextBoxLoginName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ButtonGo As System.Windows.Forms.Button
    Friend WithEvents ButtonRestore As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TextBoxConnectionStringCurrentDB As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBoxConnectionStringSourceDB As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxSql As System.Windows.Forms.TextBox
    Friend WithEvents lblCount As Label
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents PictureBox1 As PictureBox
End Class
