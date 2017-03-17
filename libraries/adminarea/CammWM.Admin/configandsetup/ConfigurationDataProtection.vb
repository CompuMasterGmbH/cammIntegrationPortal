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

        'Those should never be under control of the user, because they are used internally by the application
        'TODO: maybe specify in config file?
        Private ignoredTypeNames As String() = {"deletedon", "isdeleteduser"}

        Private Sub CreateCheckBoxes()
            For Each logTypeName As DictionaryEntry In dataProtection.GetLogTypes()
                Dim name As String = CStr(logTypeName.Key)
                Dim keep As Boolean = CBool(logTypeName.Value)

                Dim enabled As Boolean = Not (Array.IndexOf(ignoredTypeNames, name.ToLower()) > -1)
                Dim checkBox As New CheckBox
                checkBox.ID = nameCheckBoxTypes & name
                checkBox.Checked = keep
                If enabled Then
                    checkBox.Text = name
                Else
                    checkBox.Text = name & " <i>(internal system flag, setting can't be modified)</i>"
                End If

                checkBox.Enabled = enabled

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
            CreateCheckBoxes()
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
                            If checkBox.Enabled Then
                                dataProtection.AddLogTypeDeletionSetting(checkBox.Text, keepAlive)
                            End If
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

End Namespace
