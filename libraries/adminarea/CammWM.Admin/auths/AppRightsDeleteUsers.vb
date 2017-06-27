'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to delete any group from any application
    ''' </summary>
    Public Class AppRightsDeleteUsers
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblField_ID, lblField_AppTitle, lblField_UserID, lblField_username, lblField_loginname As Label
        Protected ancDelete, ancDontDelete As Web.UI.HtmlControls.HtmlAnchor
#End Region

#Region "Page Events"
        Private Sub AppRightsDeleteUsers_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ID, Field_AppID As Integer
            Dim Field_AppTitle As String = ""
            Dim Field_UserID As Long
            lblErrMsg.Text = ""

            If False Then
            ElseIf Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Dim sqlParams As SqlParameter() = {New SqlParameter("@AuthID", CLng(Request.QueryString("ID")))}

                Try
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteApplicationRightsByUser", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("apprights.aspx?Application=" & CInt(Request.QueryString("APPID")) & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID"))
                Catch ex As Exception
                    lblErrMsg.Text = "Authorization erasing failed! (" & ex.Message & ")"
                End Try
            Else
                Dim sqlParams As SqlParameter() = {New SqlParameter("@AuthID", CLng(Request.QueryString("ID")))}
                Dim MySQLString As String = "SELECT * FROM dbo.view_ApplicationRights WHERE ID_User Is Not Null And ThisAuthIsFromAppID Is Null And ID_AppRight=@AuthID"
                Dim MyDt As New DataTable

                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), MySQLString, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If MyDt Is Nothing OrElse MyDt.Rows.Count = 0 Then
                    lblErrMsg.Text = "Authorization not found!<br>Try to go back and refresh the content of the previous page."
                    Response.Redirect(cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrCode=" & Server.UrlEncode(lblErrMsg.Text))
                End If

                With MyDt.Rows(0)
                    Field_ID = Utils.Nz(.Item("ID_AppRight"), 0)
                    Field_AppTitle = Utils.Nz(.Item("Title"), String.Empty)
                    Field_AppID = Utils.Nz(.Item("ID_Application"), 0)
                    Field_UserID = Utils.Nz(.Item("ID_User"), 0)
                End With
                MyDt.Dispose()
            End If

            lblField_ID.Text = Server.HtmlEncode(Utils.Nz(Field_ID, 0).ToString)
            lblField_AppTitle.Text = Server.HtmlEncode(Utils.Nz(Field_AppTitle, String.Empty))

            If True Then
                Dim Field_loginname As String = ""
                Dim Field_username As String = ""
                If Field_UserID = Nothing Then Field_UserID = Utils.Nz(Request.QueryString("ID"), 0)
                Dim MyDt As New DataTable

                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", Field_UserID)}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString),
                                    "SELECT ID, Loginname, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS Name,[E-MAIL] FROM dbo.Benutzer WHERE ID = @ID", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    With MyDt.Rows(0)
                        Field_ID = Utils.Nz(.Item("ID"), 0)
                        Field_loginname = Utils.Nz(.Item("LoginName"), String.Empty)
                        Field_username = Utils.Nz(.Item("Name"), String.Empty)
                    End With
                End If

                lblField_UserID.Text = Server.HtmlEncode(Utils.Nz(Field_UserID, 0).ToString)
                lblField_loginname.Text = Server.HtmlEncode(Utils.Nz(Field_loginname, String.Empty))
                lblField_username.Text = Server.HtmlEncode(Utils.Nz(Field_username, String.Empty))
                ancDelete.HRef = "apprights_delete_users.aspx?ID=" & Request.QueryString("ID").ToString & "&DEL=NOW&APPID=" & Field_AppID & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID").ToString & "&token=" & Session.SessionID
                ancDontDelete.HRef = "apprights.aspx?Application=" & Field_AppID & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID").ToString
                MyDt.Dispose()

            End If
        End Sub
#End Region

    End Class

End Namespace