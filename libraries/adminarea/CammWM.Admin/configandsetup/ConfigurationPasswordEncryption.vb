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

End Namespace
