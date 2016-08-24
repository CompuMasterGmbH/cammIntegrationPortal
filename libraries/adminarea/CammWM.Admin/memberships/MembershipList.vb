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
    '''     A page to view the list of memberships
    ''' </summary>
    Public Class MembershipList
        Inherits MembershipsOverview

#Region "Variable Declaration"
        Protected WithEvents rptMembershipList As Repeater
        Protected SearchGroupTextBox, SearchUserTextBox As TextBox
        Protected SearchGrpLabel, SearchUserLabel, lblGroupID, lblDescription, lblReleasedOn, lblUserID, lblLoginName, lblCompany, lblErrMsg As Label
        Protected WithEvents btnSubmit As Button
        Protected gcDisabled, gcSearchGroupSpace As HtmlGenericControl
        Protected ancShowSysGroups, ancShowAll, ancHideAll, ancGroupIDName, ancGroupID, ancDelete As HtmlAnchor
        Protected ancExportCSV, ancUserName, ancReleasedBy, ancName, ancAddUserShowDetails As HtmlAnchor
        Protected trDetails As HtmlTableRow
        Protected tdDetails As HtmlTableCell
        Dim MyDt, UserData As DataTable
        Dim flag As Boolean
        Dim FirstAppLine, DisplayNewHeaderUsers, CurUserIsSecurityOperator, CurUserIsSecurityMaster, CurUserIsSupervisor As Boolean
        Dim OldGroupID, NewGroupID As Integer
        Dim InsertHTML As Text.StringBuilder
#End Region

#Region "Page Events"
        Private Sub MembershipList_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            If Trim(Request.QueryString("GROUPID")) = "" Then
                SearchGroupTextBox.Style.Add("display", "")
                SearchGrpLabel.Style.Add("display", "")
                gcSearchGroupSpace.Style.Add("display", "")
            Else
                SearchGroupTextBox.Style.Add("display", "none")
                SearchGrpLabel.Style.Add("display", "none")
                gcSearchGroupSpace.Style.Add("display", "none")
            End If

            lblErrMsg.Text = ""
            'btnSubmit.Attributes.Add("onclick", "return CheckBlankFields('" & SearchGroupTextBox.ClientID & "','" & SearchUserTextBox.ClientID & "');")
            CurUserIsSecurityOperator = cammWebManager.System_IsSecurityOperator(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            CurUserIsSecurityMaster = cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            If Not CurUserIsSecurityMaster Then CurUserIsSecurityMaster = cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            CurUserIsSupervisor = cammWebManager.System_IsSuperVisor(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            If CurUserIsSecurityOperator AndAlso (CurUserIsSecurityMaster Or CurUserIsSupervisor) Then CurUserIsSecurityOperator = False

            If Trim(Request.QueryString("GROUPID")) <> "" And Utils.Nz(Request.QueryString("ShowSystemGrp"), 0) <> 1 Then
                ancShowSysGroups.InnerHtml = "Display all"
            Else
                ancShowSysGroups.InnerHtml = ""
            End If

            If Not CurUserIsSecurityOperator AndAlso (Trim(Request.QueryString("GROUPID")) = "" And Utils.Nz(Request.QueryString("Showall"), 0) <> 1) Then
                ancShowAll.InnerHtml = "Show all users"
            Else
                ancShowAll.InnerHtml = ""
            End If

            If Not CurUserIsSecurityOperator AndAlso (Trim(Request.QueryString("GROUPID")) = "" And Utils.Nz(Request.QueryString("Showall"), 0) = 1) Then
                ancHideAll.InnerHtml = "Hide all users"
            Else
                ancHideAll.InnerHtml = ""
            End If

            ListOfRecords()
        End Sub
#End Region

#Region "User-Defined Methods"
        ''' <summary>
        ''' Bind the data for the group headlines to the control with the list of records (indirectly bind the list of users, too)
        ''' </summary>
        Protected Sub ListOfRecords()
            Dim WhereClause, TopClause As String
            TopClause = ""

            WhereClause = " "
            If Trim(Request.QueryString("GROUPID") & "") <> "" Then
                WhereClause += " ID_Group = @GroupID And "
            ElseIf Trim(SearchGroupTextBox.Text & "") <> "" Then
                WhereClause += " m.[Name] LIKE @Name And "
            End If

            Dim searchString As String = SearchUserTextBox.Text.Replace("'", "''").Replace("*", "%")
            Dim searchItems As String() = Nothing

            If Trim(searchString) <> "" Then
                If InStr(searchString, " ") > 0 Then
                    searchString = Text.RegularExpressions.Regex.Replace(searchString, "\s{2,}", " ")
                    searchItems = searchString.Split(" "c)
                    For mySearchCounter As Integer = 0 To searchItems.Length - 1
                        WhereClause &= " (Loginname LIKE @SearchItem" & mySearchCounter & " Or (Vorname  LIKE @SearchItem" & mySearchCounter & " And Nachname LIKE @SearchItem" & mySearchCounter & ")"
                        WhereClause &= " Or (Nachname LIKE @SearchItem" & mySearchCounter & " And Vorname LIKE @SearchItem" & mySearchCounter & ")) And  "
                    Next
                Else
                    WhereClause += " (Nachname LIKE @UName Or Vorname LIKE @UName Or Loginname LIKE @UName) And "
                End If
            End If

            WhereClause += " ID_Group not in (Select id_group_public from system_servergroups) and ID_Group not in (Select id_group_anonymous from system_servergroups) and (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Groups' AND AuthorizationType In ('SecurityMaster','ViewAllRelations')) OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner'))) "

            FirstAppLine = True

            If WhereClause <> String.Empty Then WhereClause &= " and "

            Dim SqlQuery As String =
                "select ID_Group, m.[Name], m.Description, g.ReleasedOn, g.ReleasedBy ID_ReleasedBy," & vbNewLine &
                "   (select b.nachname from benutzer b where b.id=g.ReleasedBy) ReleasedByLastName," & vbNewLine &
                "   (select b.vorname from benutzer b where b.id=g.ReleasedBy) ReleasedByFirstName " & vbNewLine &
                "from [view_Memberships] m " & vbNewLine &
                "   left outer join gruppen g on m.ID_Group=g.id " & vbNewLine &
                "where " & WhereClause & " g.id in (" & vbNewLine &
                "   SELECT ID_Group " & vbNewLine &
                "   FROM [view_Memberships] " & vbNewLine &
                "   WHERE  ID_Group not in " & vbNewLine &
                "       (" & vbNewLine &
                "           Select id_group_public " & vbNewLine &
                "           from system_servergroups" & vbNewLine &
                "       ) " & vbNewLine &
                "       and ID_Group not in " & vbNewLine &
                "       (" & vbNewLine &
                "           Select id_group_anonymous " & vbNewLine &
                "           from system_servergroups" & vbNewLine &
                "       ) " & vbNewLine &
                "       and " & vbNewLine &
                "       (" & vbNewLine &
                "           0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) &
                "           OR 0 in " & vbNewLine &
                "           (" & vbNewLine &
                "               select tableprimaryidvalue " & vbNewLine &
                "               from System_SubSecurityAdjustments " & vbNewLine &
                "               where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & "" & vbNewLine &
                "                   AND TableName = 'Groups' " & vbNewLine &
                "                   AND AuthorizationType In ('SecurityMaster','ViewAllRelations')" & vbNewLine &
                "           ) " & vbNewLine &
                "           OR id_group in " & vbNewLine &
                "           (" & vbNewLine &
                "               select tableprimaryidvalue " & vbNewLine &
                "               from System_SubSecurityAdjustments " & vbNewLine &
                "               where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & "" & vbNewLine &
                "                   AND TableName = 'Groups' " & vbNewLine &
                "                   AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner')" & vbNewLine &
                "           )" & vbNewLine &
                "       ) " & vbNewLine &
                "   ) " & vbNewLine &
                "group by ID_Group, m.[Name], m.Description, g.ReleasedOn, g.ReleasedBy " & vbNewLine &
                "ORDER BY m.[Name], ID_Group, ReleasedByLastName, ReleasedByFirstName"


            Dim cmd As New SqlCommand(SqlQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = CType(Request.QueryString("GROUPID"), Integer)
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = SearchGroupTextBox.Text.ToString.Trim.Replace("'", "''").Replace("*", "%") & "%"
            cmd.Parameters.Add("@UName", SqlDbType.NVarChar).Value = SearchUserTextBox.Text.ToString.Trim.Replace("'", "''").Replace("*", "%") & "%"
            cmd.Parameters.Add("@UserID", SqlDbType.NVarChar).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
            If Not searchItems Is Nothing Then
                For myCounter As Integer = 0 To searchItems.Length - 1
                    cmd.Parameters.Add("@SearchItem" & myCounter, SqlDbType.NVarChar).Value = searchItems(myCounter) & "%"
                Next
            End If
            MyDt = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            rptMembershipList.DataSource = MyDt
            rptMembershipList.DataBind()

            If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
            Else
                lblErrMsg.Text = "There are no groups available for administration."
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptMembershipListItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptMembershipList.ItemDataBound
            If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
                With MyDt.Rows(e.Item.ItemIndex)
                    NewGroupID = Utils.Nz(.Item("ID_Group"), 0)

                    CType(e.Item.FindControl("ancGroupIDName"), HtmlAnchor).Name = "Group" & Utils.Nz(.Item("ID_Group"), 0).ToString
                    CType(e.Item.FindControl("lblGroupID"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ID_Group"), 0).ToString)

                    If Trim(Request.QueryString("GROUPID") & "") = "" Then
                        CType(e.Item.FindControl("ancGroupID"), HtmlAnchor).HRef = "memberships.aspx?GroupID=" & Utils.Nz(.Item("ID_Group"), String.Empty)
                    End If

                    CType(e.Item.FindControl("ancName"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("Name"), String.Empty))

                    If Trim(Request.QueryString("GROUPID") & "") = "" Then
                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "memberships.aspx?GroupID=" & Utils.Nz(.Item("ID_Group"), String.Empty)
                    Else
                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = ""
                    End If

                    CType(e.Item.FindControl("lblDescription"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Description"), String.Empty))
                    CType(e.Item.FindControl("lblReleasedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ReleasedOn"), String.Empty))

                    If Not IsDBNull(.Item("ID_ReleasedBy")) Then
                        Try
                            If Not cammWebManager.System_GetUserInfo(Utils.Nz(.Item("ID_ReleasedBy"), 0&)) Is Nothing Then
                                CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID_ReleasedBy"), 0)
                                CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(.Item("ReleasedByFirstName"), .Item("ReleasedByLastName"), New WMSystem.UserInformation(CLng(Utils.Nz(.Item("ID_ReleasedBy"), 0)), Me.cammWebManager).LoginName, CLng(Utils.Nz(.Item("ID_ReleasedBy"), 0))))
                            Else
                                CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).HRef = "#"
                                CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).InnerHtml = "-NA-"
                            End If
                        Catch ex As Exception
                            cammWebManager.Log.Warn(ex)
                            CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).HRef = "#"
                            CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).InnerHtml = "-NA-"
                        End Try
                    End If

                    If Trim(Request.QueryString("Showall") & "") <> "" OrElse Trim(Request.QueryString("GROUPID") & "") <> "" OrElse CurUserIsSecurityOperator Then
                        If Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.UpdateRelations, CType(.Item("ID_Group"), Integer)) Then
                            CType(e.Item.FindControl("ancAddUserShowDetails"), HtmlAnchor).HRef = "memberships_new.aspx?ID=" & Utils.Nz(.Item("ID_Group"), 0).ToString
                            CType(e.Item.FindControl("ancAddUserShowDetails"), HtmlAnchor).InnerHtml = "Add User"
                        End If
                        CType(e.Item.FindControl("ancExportCSV"), HtmlAnchor).HRef = "memberships.aspx?action=export&GroupID=" & Utils.Nz(.Item("ID_Group"), String.Empty)
                        CType(e.Item.FindControl("ancExportCSV"), HtmlAnchor).InnerHtml = "Export as CSV"
                    Else
                        CType(e.Item.FindControl("ancAddUserShowDetails"), HtmlAnchor).HRef = "memberships.aspx?GroupID=" & Utils.Nz(.Item("ID_Group"), 0).ToString
                        CType(e.Item.FindControl("ancAddUserShowDetails"), HtmlAnchor).InnerHtml = "Show Details"
                    End If

                    If (Trim(Request.QueryString("GROUPID")) <> "" OrElse Utils.Nz(Request.QueryString("Showall"), 0) = 1 OrElse CurUserIsSecurityOperator OrElse Trim(SearchUserTextBox.Text & "") <> "") Then
                        UserData = New DataTable
                        InsertHTML = New Text.StringBuilder

                        Dim SqlQuery As String
                        If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                            'Older / without IsDenyRule column
                            SqlQuery = "select ID_User,LoginDisabled,Nachname,Vorname,LoginName,Company,ID_Membership,0 AS IsDenyRule, 0 AS IsCloneRule from [view_Memberships] where ID_Group=" & Utils.Nz(.Item("id_group"), 0) & " and id_user is not null order by Nachname, Vorname"
                        Else
                            'Newer / IsDenyRule column available
                            SqlQuery = "select ID_User,LoginDisabled,Nachname,Vorname,LoginName,Company,ID_Membership,IsDenyRule,IsCloneRule from [view_Memberships] where ID_Group=" & Utils.Nz(.Item("id_group"), 0) & " and id_user is not null order by Nachname, Vorname"
                        End If

                        UserData = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), SqlQuery, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                        If Not UserData Is Nothing AndAlso UserData.Rows.Count > 0 Then
                            InsertHTML.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"">")
                            InsertHTML.Append("<TR height=""20""><TD WIDTH=""30"">&nbsp;</TD><TD class=""boldFontHeader"">User ID</TD>")
                            InsertHTML.Append("<TD class=""boldFontHeader"">Rule&nbsp;</TD>")
                            InsertHTML.Append("<TD class=""boldFontHeader"">Name&nbsp;</TD><TD class=""boldFontHeader"">Login&nbsp;</TD>")
                            InsertHTML.Append("<TD class=""boldFontHeader"">Company&nbsp;</TD><TD class=""boldFontHeader"">Action</TD></TR>")

                            For i As Integer = 0 To UserData.Rows.Count - 1
                                InsertHTML.Append("<TR><TD WIDTH=""30"">&nbsp;</TD>")
                                InsertHTML.Append("<TD WIDTH=""60""><P class=""normalFont""><span>" & Utils.Nz(UserData.Rows(i).Item("ID_User"), 0).ToString & "</span><span id=""gcDisabled"">")
                                If Not IsDBNull(UserData.Rows(i).Item("LoginDisabled")) Then If Utils.Nz(UserData.Rows(i).Item("LoginDisabled"), False) = True Then InsertHTML.Append("<nobr title=""Disabled"">(D)</nobr>")
                                InsertHTML.Append("</span>&nbsp;</P></TD>")
                                Dim RuleTitle As String
                                If Utils.Nz(UserData.Rows(i).Item("IsDenyRule"), False) = True Then
                                    RuleTitle = "DENY"
                                Else
                                    RuleTitle = "GRANT"
                                End If
                                InsertHTML.Append("<TD WIDTH=""60""><P class=""normalFont"">" & Server.HtmlEncode(RuleTitle) & "&nbsp;</P></TD>")
                                InsertHTML.Append("<TD WIDTH=""170""><P class=""normalFont""><a id=""ancUserName"" href=""users_update.aspx?ID=" & Utils.Nz(UserData.Rows(i).Item("ID_User"), 0).ToString & """>" & Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(UserData.Rows(i).Item("Vorname"), UserData.Rows(i).Item("Nachname"))) & "</a>&nbsp;</P></TD>")
                                InsertHTML.Append("<TD WIDTH=""150""><P class=""normalFont""><a id=""ancLoginName"" href=""users_update.aspx?ID=" & Utils.Nz(UserData.Rows(i).Item("ID_User"), 0).ToString & """>" & Server.HtmlEncode(Utils.Nz(UserData.Rows(i).Item("LoginName"), String.Empty)) & "</a>&nbsp;</P></TD>")
                                InsertHTML.Append("<TD WIDTH=""200""><P class=""normalFont""><span id=""lblCompany"">" & Server.HtmlEncode(Utils.Nz(UserData.Rows(i).Item("Company"), String.Empty)) & "</span>&nbsp;</P></TD>")
                                If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_MembershipsWithSupportForSystemAndCloneRule) >= 0 AndAlso CType(UserData.Rows(i).Item("IsCloneRule"), Boolean) = True Then 'Newer db build supporting IsCloneRule and IsCloneRule=true
                                    InsertHTML.Append("<TD><P class=""normalFont""><em>Inherited</em></P></TD></TR>")
                                ElseIf Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.UpdateRelations, CType(.Item("ID_Group"), Integer)) Then
                                    InsertHTML.Append("<TD><P class=""normalFont""><a id=""ancDelete"" href=""memberships_delete.aspx?ID=" & Utils.Nz(UserData.Rows(i).Item("ID_Membership"), 0).ToString & """>")
                                    If Not IsDBNull(UserData.Rows(i).Item("ID_Membership")) Then InsertHTML.Append("Delete")
                                    InsertHTML.Append("</a>&nbsp;</P></TD></TR>")
                                Else
                                    InsertHTML.Append("<TD><P class=""normalFont"">&nbsp;</P></TD></TR>")
                                End If
                            Next

                            InsertHTML.Append("</TABLE>")
                            CType(e.Item.FindControl("tdDetails"), HtmlTableCell).InnerHtml = InsertHTML.ToString
                        End If
                    End If
                End With
            End If
        End Sub
#End Region

    End Class

End Namespace