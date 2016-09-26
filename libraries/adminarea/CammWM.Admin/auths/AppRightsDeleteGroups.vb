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
    Public Class AppRightsDeleteGroups
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblField_ID, lblField_AppTitle, lblField_GroupID, lblField_Groupname, lblField_Description As Label
        Protected ancDelete, ancDontDelete As Web.UI.HtmlControls.HtmlAnchor
#End Region

#Region "Page Events"
        Private Sub AppRightsDeleteGroups_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ID, Field_AppID, Field_GroupID As Integer
            Dim Field_AppTitle As String = ""
            lblErrMsg.Text = ""

            If False Then
            ElseIf Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Dim sqlParams As SqlParameter() = {New SqlParameter("@AuthID", CLng(Request.QueryString("ID"))), New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}

                Try
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteApplicationRightsByGroup", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("apprights.aspx?Application=" & CInt(Request.QueryString("APPID")) & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID"))
                Catch ex As Exception
                    lblErrMsg.Text = "Authorization erasing failed! (" & ex.Message & ")"
                End Try
            Else
                Dim sqlParams1 As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                Dim MySQLString As String = "SELECT * FROM dbo.view_ApplicationRights WHERE ID_Group Is Not Null And ThisAuthIsFromAppID Is Null And ID_AppRight=@ID"
                Dim MyDt As New DataTable

                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), MySQLString, CommandType.Text, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If MyDt Is Nothing OrElse MyDt.Rows.Count = 0 Then
                    lblErrMsg.Text = "Authorization not found!<br>Try to go back and refresh the content of the previous page."
                    Response.Redirect(cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrCode=" & Server.UrlEncode(lblErrMsg.Text))
                End If

                With MyDt.Rows(0)
                    Field_ID = Utils.Nz(.Item("ID_AppRight"), 0)
                    Field_AppTitle = Utils.Nz(.Item("Title"), String.Empty)
                    Field_AppID = Utils.Nz(.Item("ID_Application"), 0)
                    Field_GroupID = Utils.Nz(.Item("ID_Group"), 0)
                End With
                MyDt.Dispose()
            End If

            lblField_ID.Text = Server.HtmlEncode(Utils.Nz(Field_ID, 0).ToString)
            lblField_AppTitle.Text = Server.HtmlEncode(Utils.Nz(Field_AppTitle, String.Empty))

            If True Then
                Dim Field_Groupname, Field_Description As String
                Dim MyDt As New DataTable
                Field_Groupname = ""
                Field_Description = ""

                Dim sqlParams2 As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Field_GroupID & "")))}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString),
                                    "SELECT * FROM dbo.Gruppen Where ID=@ID", CommandType.Text, sqlParams2, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    With MyDt.Rows(0)
                        Field_GroupID = Utils.Nz(.Item("ID"), 0)
                        Field_Groupname = Utils.Nz(.Item("Name"), String.Empty)
                        Field_Description = Utils.Nz(.Item("Description"), String.Empty)
                    End With
                End If

                lblField_Description.Text = Server.HtmlEncode(Utils.Nz(Field_Description, String.Empty))
                lblField_Groupname.Text = Server.HtmlEncode(Utils.Nz(Field_Groupname, String.Empty))
                lblField_GroupID.Text = Server.HtmlEncode(Utils.Nz(Field_GroupID, String.Empty))
                ancDelete.HRef = "apprights_delete_groups.aspx?ID=" & Request.QueryString("ID").ToString & "&DEL=NOW&APPID=" & Field_AppID & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID").ToString & "&token=" & Session.SessionID
                ancDontDelete.HRef = "apprights.aspx?Application=" & Field_AppID & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID").ToString
            End If
        End Sub
#End Region

    End Class

End Namespace