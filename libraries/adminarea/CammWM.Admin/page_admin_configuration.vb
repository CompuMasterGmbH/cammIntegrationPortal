'Copyright 2012-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration
    Public Class ConfigurationOverview
        Inherits Page
#Region "Variable Declaration"

#End Region

#Region "Page Events"
        Private Sub Page_OnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        End Sub
#End Region

    End Class

    Public Class ConfigurationLogging
        Inherits Page

#Region "Variable Declaration"
        Protected lblMsg, lblRowsInLogTable As Label
        Protected txtMaxRowsInLogTable, txtMaxDaysOfLogEntries As TextBox
        Protected WithEvents btnSaveSettings, btnDeleteOldLogsNow, btnSaveConflictTypes As Button
        Protected tblConflictTypes As Table
#End Region


        Private Sub SetConflictTypeTableHeader()
            Dim header As New TableRow
            Dim headerCellConflictTypes As New TableHeaderCell
            headerCellConflictTypes.Text = "ConflictType"
            Dim headerCellDays As New TableHeaderCell
            headerCellDays.Text = "Keep for x days"
            header.Cells.Add(headerCellConflictTypes)
            header.Cells.Add(headerCellDays)
            tblConflictTypes.Rows.Add(header)
        End Sub

        Private Function CreateIntegerCompareValidator(ByVal textBox As TextBox) As System.Web.UI.WebControls.CompareValidator
            Dim validator As New System.Web.UI.WebControls.CompareValidator
            validator.ControlToValidate = textBox.ID
            validator.Operator = ValidationCompareOperator.DataTypeCheck
            validator.Type = ValidationDataType.Integer
            validator.ErrorMessage = "value must be a number"
            validator.Display = ValidatorDisplay.Dynamic
            Return validator
        End Function

        Private Sub BuildConflictTypesTable()
            Dim index As Integer = 0
            Dim conflictTypeValues As Integer() = CType([Enum].GetValues(GetType(camm.WebManager.WMSystem.Logging_ConflictTypes)), Integer())
            Dim conflictTypeNames As String() = CType([Enum].GetNames(GetType(camm.WebManager.WMSystem.Logging_ConflictTypes)), String())
#If NetFramework <> "1_1" Then
            System.Array.Sort(Of String)(conflictTypeNames)
#End If
            SetConflictTypeTableHeader()

            Dim defaultMaxAgeInDays As String = CStr(cammWebManager.Log.MaxRetentionDays)
            Dim lifeTimeTable As Hashtable = cammWebManager.Log.ConflictTypeLifeTime
            For Each conflictname As String In conflictTypeNames
                Dim row As New TableRow
                Dim cell As New TableCell
                Dim txtCell As New TableCell

                cell.Text = conflictname

                Dim daysTextBox As New TextBox
                Dim enumvalue As Integer = conflictTypeValues(index)
                Dim txtBoxValue As String
                If lifeTimeTable Is Nothing Then
                    txtBoxValue = defaultMaxAgeInDays
                Else
                    txtBoxValue = CStr(lifeTimeTable(enumvalue))
                End If
                daysTextBox.Text = txtBoxValue
                daysTextBox.ID = "txtBox" & enumvalue.ToString().Replace("-"c, "_"c)
                txtCell.Controls.Add(daysTextBox)

                Dim validator As System.Web.UI.WebControls.CompareValidator = CreateIntegerCompareValidator(daysTextBox)
                validator.ID = "validator" & enumvalue.ToString().Replace("-"c, "_"c)
                txtCell.Controls.Add(validator)

                row.Cells.Add(cell)
                row.Cells.Add(txtCell)
                tblConflictTypes.Rows.Add(row)
                index += 1
            Next


        End Sub
#Region "Page Events"
        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            BuildConflictTypesTable()
        End Sub
        Private Sub Page_Onload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Not Me.IsPostBack Then
                lblRowsInLogTable.Text = CStr(cammWebManager.Log.RefreshedRowsInLogTable)
                txtMaxRowsInLogTable.Text = CStr(cammWebManager.Log.MaxRowsInLogTable)
                txtMaxDaysOfLogEntries.Text = CStr(cammWebManager.Log.MaxRetentionDays)
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub SaveSettings(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveSettings.Click
            cammWebManager.Log.MaxRowsInLogTable = CInt(txtMaxRowsInLogTable.Text)
            cammWebManager.Log.MaxRetentionDays = CInt(txtMaxDaysOfLogEntries.Text)
            lblMsg.Text = "Settings saved successfully"
        End Sub

        Private Sub SaveConflictTypeLifeTimes(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveConflictTypes.Click
            Me.Page.Validate()
            If Me.Page.IsValid Then
                Dim values As Integer() = CType([Enum].GetValues(GetType(camm.WebManager.WMSystem.Logging_ConflictTypes)), Integer())
                Dim ht As New Hashtable(values.Length)
                For Each val As Integer In values
                    Dim control As Web.UI.Control = Me.Page.FindControl("txtBox" + val.ToString())
                    Dim txtBox As TextBox = CType(control, TextBox)
                    Dim txtBoxValue As String = Trim(txtBox.Text)
                    If txtBoxValue = String.Empty Then
                        ht.Add(val, CStr(cammWebManager.Log.MaxRetentionDays))
                    Else
                        ht.Add(val, CType(txtBoxValue, Integer))
                    End If
                Next
                cammWebManager.Log.SetConflictTypesLifetime(ht)
                lblMsg.Text = "ConflictType Lifetime saved successfully"
            End If
        End Sub

        Private Sub DeleteOldLogsNow(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeleteOldLogsNow.Click
            cammWebManager.Log.CleanUpLogTable()
            lblRowsInLogTable.Text = CStr(cammWebManager.Log.RowsInLogTable)
            lblMsg.Text = "Old logs successfully deleted!"
        End Sub

        Private Sub ConfigurationLogging_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
            If txtMaxDaysOfLogEntries.Text <> Nothing AndAlso Utils.TryCInt(txtMaxDaysOfLogEntries.Text, 0) <> 0 Then
                Dim CurrentDaysAmount As Integer = Utils.TryCInt(txtMaxDaysOfLogEntries.Text, 0)
                txtMaxDaysOfLogEntries.ToolTip = CurrentDaysAmount & " days ~ " & (CurrentDaysAmount / 30).ToString("0.00") & " months ~ " & (CurrentDaysAmount / 365).ToString("0.00") & " years"
            End If
        End Sub
#End Region
    End Class

    Public Class ConfigurationPasswordEncryption
        Inherits Page

        Protected lblMsg As Label
        Protected cmbAlgorithms As DropDownList
        Protected cmbResetBehavior As DropDownList
        Protected WithEvents btnSubmitAlgo As Button
        Protected WithEvents btnSubmitRecovery As Button
        Protected pnlPage As Panel

        Private Function CurrentAlgorithm() As CompuMaster.camm.WebManager.PasswordAlgorithm
            Return Me.cammWebManager.System_GetDefaultPasswordAlgorithm()
        End Function

        Private Function IsSupported() As Boolean
#If NetFramework = "1_1" Then
            Return False
#End If
            Return Me.cammWebManager.System_Version_Ex().Build >= WMSystem.MilestoneAssembly_Build193 AndAlso Me.cammWebManager.System_SupportsMultiplePasswordAlgorithms()
        End Function

        Private Sub SetAlgoComboBox()
            For Each algo As CompuMaster.camm.WebManager.PasswordAlgorithm In [Enum].GetValues(GetType(CompuMaster.camm.WebManager.PasswordAlgorithm))
                Dim text As String = " (password not decryptable) (recommended)"
                If CompuMaster.camm.WebManager.AlgorithmInfo.CanDecrypt(algo) Then
                    text = " (password can be decrypted)"
                    If CompuMaster.camm.WebManager.AlgorithmInfo.IsWeak(algo) Then
                        text &= " (weak)"
                    End If
                End If


                Dim itemText As String = [Enum].GetName(GetType(PasswordAlgorithm), algo) & text
                Dim item As New ListItem
                item.Text = itemText
                item.Value = algo.ToString()
                cmbAlgorithms.Items.Add(item)
            Next
            cmbAlgorithms.SelectedIndex = Me.CurrentAlgorithm()
        End Sub

        Private Sub SetBehaviorComboBox()
            cmbResetBehavior.Items.Add("If algorithm allows, decrypt password and send it to the user. If algorithm doesn't allow decryption, send reset link")
            cmbResetBehavior.Items.Add("Always send a reset link")
            Dim index As Integer = Me.cammWebManager.System_GetPasswordRecoveryBehavior()
            If index >= 0 AndAlso index <= cmbResetBehavior.Items.Count - 1 Then
                cmbResetBehavior.SelectedIndex = index
            End If
        End Sub

        Private Sub SetDropDowns()
            SetAlgoComboBox()
            SetBehaviorComboBox()
        End Sub

        Private Sub Save_AlgorithmChoice(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmitAlgo.Click
            Dim algo As PasswordAlgorithm = CType([Enum].Parse(GetType(PasswordAlgorithm), cmbAlgorithms.SelectedItem.Value), PasswordAlgorithm)
            Me.cammWebManager.System_SetDefaultPasswordAlgorithm(algo)
            Me.lblMsg.Text = "The new algorithm has been set successfully."
        End Sub

        Private Sub Save_RecoveryChoice(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmitRecovery.Click
            Dim behavior As PasswordRecoveryBehavior = CType(cmbResetBehavior.SelectedIndex, PasswordRecoveryBehavior)
            Me.cammWebManager.System_SetPasswordRecoveryBehavior(behavior)
            Me.lblMsg.Text = "The new recovery behavior has been set successfully."
        End Sub


        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            If Me.IsSupported() Then
                If Not Me.IsPostBack Then
                    SetDropDowns()
                End If
            Else
                lblMsg.Text = "This functionaly requires database and library build 193, and .NET framework >= 2.0"
                lblMsg.ForeColor = Drawing.Color.Red
                pnlPage.Visible = False
            End If

        End Sub

        Private Sub Page_Onload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        End Sub
    End Class


    Public Class ConfigurationDataProtection
        Inherits Page

        Protected lblMsg As Label
        Protected WithEvents btnSaveSettings As Button
        Protected WithEvents btnSaveAndCleanupSettings As Button
        Protected txtBoxDeleteAfterDays As TextBox
        Protected txtBoxDeleteMailsAfterDays As TextBox
        Protected txtBoxAnonymizeIPs As TextBox

        Protected ltrlTypeList As PlaceHolder
        'TODO: what about caching?
        Protected dataProtection As DataProtectionSettings

        Private Const nameCheckBoxTypes As String = "chckBxTypeName"


        Private Sub SetLiteralTypeList()
            For Each logTypeName As DictionaryEntry In dataProtection.GetLogTypes()
                Dim name As String = CStr(logTypeName.Key)
                Dim keep As Boolean = CBool(logTypeName.Value)

                Dim checkBox As New CheckBox
                checkBox.ID = "chckBxTypeName" & name
                checkBox.Checked = keep
                checkBox.Text = name

                Dim brLiteral As New Literal
                brLiteral.Text = "<br>"
                ltrlTypeList.Controls.Add(checkBox)
                ltrlTypeList.Controls.Add(brLiteral)
            Next
        End Sub

        Private Sub SetTextBoxes()
            Me.txtBoxAnonymizeIPs.Text = dataProtection.AnonymizeIPsAfterDays.ToString()
            Me.txtBoxDeleteAfterDays.Text = dataProtection.DeleteDeactivatedUsersAfterDays.ToString()
            Me.txtBoxDeleteMailsAfterDays.Text = dataProtection.DeleteMailsAfterDays.ToString()
        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            dataProtection = New CompuMaster.camm.WebManager.DataProtectionSettings(Me.cammWebManager.ConnectionString)
            SetLiteralTypeList()
        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Not Me.IsPostBack() Then
                SetTextBoxes()
            End If
        End Sub

        Private Sub SaveSettings(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveSettings.Click
            ValidateAndSaveSettings()
        End Sub

        Private Function ValidateAndSaveSettings() As Boolean
            Me.Page.Validate()
            If Me.Page.IsValid() Then
                dataProtection.AnonymizeIPsAfterDays = CType(Me.txtBoxAnonymizeIPs.Text, Integer)
                dataProtection.DeleteDeactivatedUsersAfterDays = CType(Me.txtBoxDeleteAfterDays.Text, Integer)
                dataProtection.DeleteMailsAfterDays = CType(Me.txtBoxDeleteMailsAfterDays.Text, Integer)
                For Each ctrl As System.Web.UI.Control In ltrlTypeList.Controls
                    If TypeOf ctrl Is CheckBox Then
                        If ctrl.ID.IndexOf(nameCheckBoxTypes) > -1 Then
                            Dim checkBox As CheckBox = CType(ctrl, CheckBox)
                            Dim keepAlive As Boolean = checkBox.Checked
                            dataProtection.AddLogTypeDeletionSetting(checkBox.Text, keepAlive)
                        End If
                    End If
                Next
                dataProtection.SaveSettings()
                Return True
            Else
                lblMsg.Text = "Please correct the errors."
                Return False
            End If
        End Function

        Private Sub btnSaveAndCleanupSettings_Click(sender As Object, e As EventArgs) Handles btnSaveAndCleanupSettings.Click
            If ValidateAndSaveSettings() Then
                dataProtection.CleanupSettings()
            End If
        End Sub

    End Class

    Public Class PasswordConvert
        Inherits Page


        Private Sub Page_Onload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            HttpContext.Current.Session("PasswordConverter") = Nothing
        End Sub


    End Class


    Public Class PasswordConvertAjax
        Inherits Page
#If NetFramework <> "1_1" Then
        Private sessionKey As String = "PasswordConverter"
        Private ReadOnly Property Converter As PasswordConverter
            Get
                Dim sessionKey As String = "PasswordConverter"
                If HttpContext.Current.Session(sessionKey) Is Nothing Then
                    Dim convert As PasswordConverter = New PasswordConverter(Me.cammWebManager, Me.cammWebManager.System_GetDefaultPasswordAlgorithm())
                    HttpContext.Current.Session(sessionKey) = convert
                    Return convert
                End If
                Return CType(HttpContext.Current.Session(sessionKey), PasswordConverter)
            End Get
        End Property


        Private Sub Page_Onload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim action As String = Request("action")
            If Not action Is Nothing Then
                If action = "count" Then
                    Dim counted As Integer = Me.Converter.CountConvertable()
                    Response.Write(counted.ToString())
                End If
                If action = "convert" Then
                    Dim converted As Integer = Me.Converter.ConvertPasswords(5)
                    Response.Write(converted.ToString())
                End If
                If action = "reset" Then
                    HttpContext.Current.Session(sessionKey) = Nothing
                    Response.Write("ok")
                End If

            End If
        End Sub
#End If
    End Class


End Namespace
