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
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration


    ''' <summary>
    '''     A page to delete a membership
    ''' </summary>
    Public Class Membership_Delete
        Inherits Page

#Region "Variable Declaration"
        Dim Redirect2URL, ErrMsg, Field_Name, Field_Descr, Field_CompleteName As String
        Dim Field_ID, Field_GroupID As Integer
        Dim Field_UserID As Long
        Dim Field_IsDenyRule As Boolean
        Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
        Dim dt As New DataTable
        Protected lblMembershipID, lblRule, lblGroupName, lblGroupDescription, lblLoginName, lblCompleteName, lblErrMsg As Label
        Protected ancDelete, ancTouch As HtmlAnchor
#End Region

#Region "Page Event"
        Private Sub Membership_Delete_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Dim CurUserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                Try
                    Dim sqlParams1 As SqlParameter() = {New SqlParameter("@ID", Request.QueryString("ID")),
                        New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)),
                        New SqlParameter("@compare", CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))))}
                    Dim mySqlQuery As String
                    mySqlQuery = "DELETE FROM dbo.Memberships WHERE ID=@ID and (0 <> @compare OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid=@UserID AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','Owner')))"
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), mySqlQuery, CommandType.Text, sqlParams1, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Redirect2URL = "memberships.aspx?GROUPID=" & Request.QueryString("GROUPID")
                Catch ex As Exception
                    If cammWebManager.System_DebugLevel >= 3 Then
                        ErrMsg = "Membership erasing failed! (" & ex.Message & ex.StackTrace & ")"
                    Else
                        ErrMsg = "Membership erasing failed!"
                    End If
                End Try
                If Redirect2URL <> "" Then Response.Redirect(Redirect2URL)
            End If

            Dim sqlParams As SqlParameter() = {New SqlParameter("@ID_Membership", Request.QueryString("ID"))}
            dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SELECT * FROM dbo.view_Memberships WHERE ID_Membership=@ID_Membership and (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','Owner')))", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Redirect2URL = "memberships.aspx"
            Else
                With dt.Rows(0)
                    Field_ID = Utils.Nz(.Item("ID_Membership"), 0)
                    Field_GroupID = Utils.Nz(.Item("ID_Group"), 0)
                    If dt.Columns.Contains("IsDenyRule") Then
                        Field_IsDenyRule = Utils.Nz(.Item("IsDenyRule"), False)
                    Else
                        Field_IsDenyRule = False
                    End If
                    Field_UserID = Utils.Nz(.Item("ID_User"), 0)
                    Field_Name = Utils.Nz(.Item("Name"), String.Empty)
                    Field_Descr = Utils.Nz(.Item("description"), String.Empty)
                    Field_CompleteName = Utils.Nz(.Item("Vorname"), String.Empty)
                End With
            End If

            If Redirect2URL <> "" Then Response.Redirect(Redirect2URL)
            If ErrMsg <> "" Then
                lblErrMsg.Text = ErrMsg
            End If

            MyUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(Field_UserID), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
            lblMembershipID.Text = Server.HtmlEncode(Utils.Nz(Field_ID, 0).ToString)
            lblGroupName.Text = Server.HtmlEncode(Utils.Nz(Field_Name, String.Empty))
            lblGroupDescription.Text = Server.HtmlEncode(Utils.Nz(Field_Descr, String.Empty))
            lblLoginName.Text = Server.HtmlEncode(Utils.Nz(MyUserInfo.LoginName, String.Empty))
            lblCompleteName.Text = Server.HtmlEncode(Utils.Nz(Field_CompleteName, String.Empty))
            If lblRule Is Nothing Then
                'compatibility mode: admin pages haven't been updated yet and don't provide the lblRule tag - but since this is a DENY rule, there MUST be a warning!
                If Field_IsDenyRule = True Then
                    If lblErrMsg.Text <> Nothing Then
                        lblErrMsg.Text &= "<br />"
                    End If
                    lblErrMsg.Text &= "WARNING: this membership is declared as a DENY rule!"
                End If
            Else
                If Field_IsDenyRule = True Then
                    lblRule.Text = "DENY"
                Else
                    lblRule.Text = "GRANT"
                End If
            End If

            ancDelete.HRef = "memberships_delete.aspx?ID=" & Request.QueryString("ID") & "&DEL=NOW&GROUPID=" & Field_GroupID & "&token=" & Session.SessionID
            ancTouch.HRef = "memberships.aspx?GROUPID=" & Field_GroupID
        End Sub
#End Region

    End Class

End Namespace