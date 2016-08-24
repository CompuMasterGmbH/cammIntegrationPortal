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
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to delete a group
    ''' </summary>
    Public Class GroupDelete
        Inherits Page

#Region "Variable Decalration"
        Protected lblGroup_ID, lblLastModificationOn, lblCreatedOn, lblDescription, lblGroupName, lblGroupID, lblErrMsg As Label
        Protected hypLastModificationBy, hypCreatedBy, hypDeleteConfirmation As HyperLink
        Protected cammWebManagerAdminGroupInfoDetails As CompuMaster.camm.WebManager.Controls.Administration.GroupsAdditionalInformation
#End Region

#Region "Page Events"
        Private Sub GroupDeleteLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim MyGroupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
            cammWebManagerAdminGroupInfoDetails.MyGroupInfo = MyGroupInfo

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Groups", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Groups", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Delete")) Then
                Response.Write("No authorization to administrate this group.")
                Response.End()
            ElseIf Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then

                Try
                    Dim SqlParam As SqlParameter() = {New SqlParameter("@GroupID", Request.QueryString("ID"))}
                    Dim SqlParam1 As SqlParameter() = {New SqlParameter("@GroupIDForMembership", Request.QueryString("ID"))}
                    Dim SqlParam2 As SqlParameter() = {New SqlParameter("@GroupID1ForApplication", Request.QueryString("ID"))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.Gruppen WHERE (((ID)=@GroupID))", CommandType.Text, SqlParam, Automations.AutoOpenConnection)
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.Memberships WHERE (((ID_Group)=@GroupIDForMembership))", CommandType.Text, SqlParam1, Automations.AutoOpenConnection)
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.ApplicationsRightsByGroup WHERE (((ID_GroupOrPerson)=@GroupID1ForApplication))", CommandType.Text, SqlParam2, Automations.AutoOpenConnection)
                    Response.Redirect("groups.aspx")
                Catch ex As Exception
                    If cammWebManager.System_DebugLevel >= 3 Then
                        lblErrMsg.Text = "Group erasing failed! (" & ex.Message & ex.StackTrace & ")"
                    Else
                        lblErrMsg.Text = "Group erasing failed!"
                    End If
                End Try
            End If

            If True Then
                Dim MyDt As DataTable
                Dim SqlParamForSelection As SqlParameter() = {New SqlParameter("@GroupIDForSelection", Request.QueryString("ID"))}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_Groups WHERE (((ID)=@GroupIDForSelection))", CommandType.Text, SqlParamForSelection, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If MyDt Is Nothing OrElse MyDt.Rows.Count = 0 Then
                    lblErrMsg.Text = "Group not found!"
                Else
                    With MyDt.Rows(0)
                        lblGroupID.Text = Utils.Nz(.Item("ID"), 0).ToString
                        lblGroupName.Text = Server.HtmlEncode(Utils.Nz(.Item("Name"), String.Empty))
                        lblDescription.Text = Server.HtmlEncode(Utils.Nz(.Item("description"), String.Empty))
                        lblCreatedOn.Text = Server.HtmlEncode(Utils.Nz(.Item("ReleasedOn"), String.Empty))
                        hypCreatedBy.NavigateUrl = Server.HtmlEncode("users_update.aspx?ID=" + Utils.Nz(.Item("ReleasedByID"), 0).ToString)
                        hypCreatedBy.Text = Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(.Item("ReleasedByID"))))
                        lblLastModificationOn.Text = Server.HtmlEncode(Utils.Nz(.Item("ModifiedOn"), String.Empty))
                        hypLastModificationBy.NavigateUrl = "users_update.aspx?ID=" + .Item("ModifiedByID").ToString
                        hypLastModificationBy.Text = Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(.Item("ModifiedByID"))))
                        hypDeleteConfirmation.NavigateUrl = "groups_delete.aspx?ID=" + Request.QueryString("ID") + "&DEL=NOW&token=" & Session.SessionID
                        hypDeleteConfirmation.Text = "Yes, delete it!"
                    End With
                End If
            End If
        End Sub
#End Region

    End Class

End Namespace