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

Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to add a new server group
    ''' </summary>
    Public Class NewServerGroup
        Inherits Page

#Region "Variable Declaration Section"
        Protected txtGroupname, txtEmail As TextBox
        Protected lblServerGroupName, lblServerGroupId, lblErrMsg As Label
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub NewServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not (Page.IsPostBack) Then
                Dim CurUserID As Long
                CurUserID = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                txtGroupname.Text = ""
                txtEmail.Text = cammWebManager.CurrentUserInfo.EMailAddress
            End If
        End Sub
#End Region

#Region "Control Events"
        Protected Sub btnSubmitClcik(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If txtGroupname.Text.Trim <> "" And txtEmail.Text.Trim <> "" Then
                Dim Cmd As New SqlClient.SqlCommand("AdminPrivate_CreateServerGroup", New SqlConnection(cammWebManager.ConnectionString))
                Cmd.CommandType = CommandType.StoredProcedure
                Cmd.Parameters.Add("@GroupName", SqlDbType.NVarChar).Value = Mid(Trim(txtGroupname.Text), 1, 255)
                Cmd.Parameters.Add("@email_Developer", SqlDbType.NVarChar).Value = Mid(Trim(txtEmail.Text), 1, 255)
                Cmd.Parameters.Add("@UserID_Creator", SqlDbType.Int).Value = CType(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), Integer) 'TODO: SP input value still int instead of bigint!

                Dim Redirect2URL As String = ""
                Try
                    Dim obj As Object = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(Cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    If obj Is Nothing Then
                        lblErrMsg.Text = "Undefined error detected!"
                    ElseIf CLng(obj) <> 0 Then
                        Redirect2URL = "servers.aspx#ServerGroup" & Utils.Nz(obj, String.Empty)
                    Else
                        lblErrMsg.Text = "Server group creation failed!"
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = "Server group creation failed! (" & ex.InnerException.Message & ")"
                End Try

                If Redirect2URL.Trim <> "" Then Response.Redirect(Redirect2URL)
            Else
                lblErrMsg.Text = "Please specify a name for the server group and a general e-mail address for this new server group."
            End If
        End Sub
#End Region

    End Class

End Namespace