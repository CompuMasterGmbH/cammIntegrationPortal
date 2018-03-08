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
                Dim enumvalue As Integer = CType([Enum].Parse(GetType(camm.WebManager.WMSystem.Logging_ConflictTypes), conflictname), Integer)
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
                    If control Is Nothing Then control = Me.Page.FindControl("txtBox" + val.ToString().Replace("-"c, "_"c)) 'Workaround for .NET 4 behaviour
                    If control Is Nothing Then Throw New Exception("Control ""txtBox" & val.ToString() & """ expected but not found")
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

End Namespace
