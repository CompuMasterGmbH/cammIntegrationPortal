'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Strict On
Option Explicit On 

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     Memberships overview
    ''' </summary>
    ''' <remarks>
    '''     The number and order of the colmns in the export files might be subject of change in future for additional fields.
    ''' </remarks>
    Public Class MembershipsOverview
        Inherits Page

#Region "Page Events"
        Protected Sub ActionExport(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            If Request.QueryString("action") = "export" Then
                If Request.QueryString("groupid") <> Nothing Then
                    Dim GroupID As Integer = Utils.Nz(Request.QueryString("groupid"), 0)
                    If Request.QueryString("includeInactive") = "false" Then
                        CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.MembersOfGroup(cammWebManager, GroupID, False))
                    Else
                        CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.MembersOfGroup(cammWebManager, GroupID))
                    End If
                ElseIf Request.QueryString("userid") <> Nothing Then
                    Dim UserID As Long = Utils.Nz(Request.QueryString("userid"), 0)
                    CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.MembershipsOfUser(cammWebManager, UserID))
                End If
            End If

        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.MembershipList
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to view the list of memberships
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	11.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
        Protected trDetails As HtmlTableRow 'trAddBlank, trDetails, trHeaderDeatails,trHeaders   'Commented by [I-link] on 29.01.2009 (Unused rows)
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

            Dim SqlQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                "select ID_Group,m.[Name],m.Description,g.ReleasedOn,g.ReleasedBy ID_ReleasedBy,(select b.nachname from benutzer b where b.id=g.ReleasedBy) ReleasedByLastName,(select b.vorname from benutzer b where b.id=g.ReleasedBy) ReleasedByFirstName " & _
                "from [view_Memberships] m " & _
                "left outer join gruppen g on m.ID_Group=g.id " & _
                "where " & WhereClause & " g.id in (" & _
                "SELECT ID_Group FROM [view_Memberships] WHERE  ID_Group not in (Select id_group_public from system_servergroups) and ID_Group not in (Select id_group_anonymous from system_servergroups) and (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Groups' AND AuthorizationType In ('SecurityMaster','ViewAllRelations')) OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner'))) GROUP BY ID_Group" & _
                ") group by ID_Group,m.[Name],m.Description,g.ReleasedOn,g.ReleasedBy " & _
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

                        Dim SqlQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select ID_User,LoginDisabled,Nachname,Vorname,LoginName,Company,ID_Membership from [view_Memberships] where ID_Group=" & Utils.Nz(.Item("id_group"), 0) & " and id_user is not null order by Nachname, Vorname"
                        UserData = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), SqlQuery, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                        If Not UserData Is Nothing AndAlso UserData.Rows.Count > 0 Then
                            InsertHTML.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"">")
                            InsertHTML.Append("<TR height=""20""><TD WIDTH=""30"">&nbsp;</TD><TD class=""boldFontHeader"">User ID</TD>")
                            InsertHTML.Append("<TD class=""boldFontHeader"">Name&nbsp;</TD><TD class=""boldFontHeader"">Login&nbsp;</TD>")
                            InsertHTML.Append("<TD class=""boldFontHeader"">Company&nbsp;</TD><TD class=""boldFontHeader"">Action</TD></TR>")

                            For i As Integer = 0 To UserData.Rows.Count - 1
                                InsertHTML.Append("<TR><TD WIDTH=""30"">&nbsp;</TD>")
                                InsertHTML.Append("<TD WIDTH=""60""><P class=""normalFont""><span>" & Utils.Nz(UserData.Rows(i).Item("ID_User"), 0).ToString & "</span><span id=""gcDisabled"">")
                                If Not IsDBNull(UserData.Rows(i).Item("LoginDisabled")) Then If Utils.Nz(UserData.Rows(i).Item("LoginDisabled"), False) = True Then InsertHTML.Append("<nobr title=""Disabled"">(D)</nobr>")
                                InsertHTML.Append("</span>&nbsp;</P></TD>")
                                InsertHTML.Append("<TD WIDTH=""170""><P class=""normalFont""><a id=""ancUserName"" href=""users_update.aspx?ID=" & Utils.Nz(UserData.Rows(i).Item("ID_User"), 0).ToString & """>" & Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(UserData.Rows(i).Item("Vorname"), UserData.Rows(i).Item("Nachname"))) & "</a>&nbsp;</P></TD>")
                                InsertHTML.Append("<TD WIDTH=""150""><P class=""normalFont""><a id=""ancLoginName"" href=""users_update.aspx?ID=" & Utils.Nz(UserData.Rows(i).Item("ID_User"), 0).ToString & """>" & Server.HtmlEncode(Utils.Nz(UserData.Rows(i).Item("LoginName"), String.Empty)) & "</a>&nbsp;</P></TD>")
                                InsertHTML.Append("<TD WIDTH=""200""><P class=""normalFont""><span id=""lblCompany"">" & Server.HtmlEncode(Utils.Nz(UserData.Rows(i).Item("Company"), String.Empty)) & "</span>&nbsp;</P></TD>")
                                If Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.UpdateRelations, CType(.Item("ID_Group"), Integer)) Then
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

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.MembershipsNew
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to add new members to an existing group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[zeutzheim]	15.03.2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class MembershipsNew
        Inherits Page

#Region "Variable Declaration"
        Protected lblErr, lblMsg, lblNoRecMsg As Label
        Protected WithEvents btnOK, searchUserBtn As Button
        Protected WithEvents drp_groups, drp_users As DropDownList
        Protected SearchUsersTextBox As TextBox
        Protected CheckBoxTop50Results As CheckBox
        Protected WithEvents rptUserList As Repeater
        Protected MyDt As DataTable
#End Region

#Region "Page Events"
        Private Sub MembershipsNew_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErr.Text = ""
            lblMsg.Text = ""
            If Not IsPostBack Then
                ListOfUsers()
                LoadGroups()
            End If
        End Sub

#End Region

#Region "User-Defined Methods"
        Private Sub LoadGroups()
            Dim GrpTable As DataTable

            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
            Dim sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM Gruppen WHERE [ID] Not In (Select id_group_public from system_servergroups) And  [ID] not in (Select id_group_anonymous from system_servergroups) And (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Groups' AND AuthorizationType In ('SecurityMaster')) OR gruppen.id in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','Owner'))) ORDER BY Name"
            GrpTable = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sql, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            drp_groups.DataSource = GrpTable
            drp_groups.DataTextField = "Name"
            drp_groups.DataValueField = "ID"
            drp_groups.DataBind()

            drp_groups.SelectedValue = Request.QueryString("ID")

        End Sub

        Protected Sub ListOfUsers()
            Dim WhereClause As String = ""

            Dim SearchWords As String = SearchUsersTextBox.Text.Replace("'", "''").Replace("*", "%")
            SearchWords = "%" & SearchWords & "%"
            SearchWords = SearchWords.Replace(" ", "% %")
            Dim searchItems As String() = SearchWords.Split(" "c)
            Dim SingleNameItems As String = String.Empty
            Dim IsSingelName As Boolean = False


            'Search for the whole searchword in every column
            If Not SearchUsersTextBox.Text = "" Then
                WhereClause = " WHERE Loginname LIKE @SearchWords Or Namenszusatz LIKE @SearchWords  Or company LIKE @SearchWords or Nachname LIKE @SearchWords Or Vorname LIKE @SearchWords"
            End If

            'Search for every single word (space-seperated) in searchword
            If searchItems.Length > 0 Then
                For myWordCounter As Integer = 0 To searchItems.Length - 1
                    WhereClause &= " or Loginname LIKE @SearchItem" & myWordCounter & " Or Namenszusatz LIKE @SearchItem" & myWordCounter & " Or company LIKE @SearchItem" & myWordCounter & " or Nachname LIKE @SearchItem" & myWordCounter & " Or Vorname LIKE @SearchItem" & myWordCounter & ""
                Next
            End If

            Dim TopClause As String = ""
            If CheckBoxTop50Results.Checked = True Then TopClause = "TOP 50" Else TopClause = ""

            Try
                If SearchWords.Replace("%", "") = String.Empty Then WhereClause = String.Empty

                Dim sb As New System.Text.StringBuilder
                sb.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT " & TopClause & " System_AccessLevels.Title As AccessLevel_Title, Benutzer.*, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS UserNameComplete FROM [Benutzer] LEFT JOIN System_AccessLevels ON Benutzer.AccountAccessability = System_AccessLevels.ID" & vbNewLine)
                sb.Append(WhereClause & " ORDER BY Nachname, Vorname" & vbNewLine)

                Dim SqlQuery As String = sb.ToString

                Dim cmd As New SqlClient.SqlCommand(SqlQuery, New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@SearchWords", SqlDbType.NVarChar).Value = SearchWords
                For myWordCounter2 As Integer = 0 To searchItems.Length - 1
                    cmd.Parameters.Add("@SearchItem" & myWordCounter2, SqlDbType.NVarChar).Value = searchItems(myWordCounter2)
                Next
                MyDt = FillDataTable(cmd, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptUserList.DataSource = MyDt
                    rptUserList.DataBind()
                Else
                    Dim drow As DataRow
                    drow = MyDt.NewRow
                    MyDt.Rows.Add(drow)
                    rptUserList.DataSource = MyDt
                    rptUserList.DataBind()
                    rptUserList.Items(0).Visible = False
                    lblNoRecMsg.Text = "No records found matching your search request."
                End If
            Catch ex As Exception
                Throw
            Finally
                MyDt.Dispose()
            End Try
        End Sub

#End Region

#Region "Control Events"

        Private Sub searchUserBtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles searchUserBtn.Click
            ListOfUsers()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Bind user data to the repeater control
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	15.03.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub rptUserListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUserList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                With MyDt.Rows(e.Item.ItemIndex)
                    CType(e.Item.FindControl("lblID"), Label).Text = Utils.Nz(.Item("ID"), 0).ToString
                    If Not IsDBNull(.Item("LoginDisabled")) Then If Utils.Nz(.Item("LoginDisabled"), False) = True Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).InnerHtml = "<nobr title=""Disabled"">(D)</nobr>"
                    CType(e.Item.FindControl("ancUserNameComplete"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancUserNameComplete"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(.Item("Vorname"), .Item("Nachname")))
                    CType(e.Item.FindControl("lblLoginName"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("lblLoginName"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("LoginName"), String.Empty))
                    If Not e.Item.FindControl("lblCompany") Is Nothing Then CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Company"), String.Empty))
                End With
            End If
        End Sub

        ''' <summary>
        ''' Fetch required flags from every application the group has access to
        ''' </summary>
        ''' <param name="groupId"></param>
        ''' <returns></returns>
        ''' <remarks>Based on TZ's code</remarks>
        Private Function GetRequiredApplicationFlags(ByVal groupId As Integer) As String()
            Dim commandText As String = "DECLARE @EmployeeList varchar(4000) SELECT @EmployeeList = COALESCE(@EmployeeList + ', ', '') + RequiredUserProfileFlags From Applications_CurrentAndInactiveOnes Where ID IN (Select ID_Application From ApplicationsRightsByGroup Where ID_GroupOrPerson = @GroupID) Select @EmployeeList"
            Dim command As New SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
            command.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId
            Return ExecuteScalar(command, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToString().Split(CChar(","))
        End Function

        ''' <summary>
        ''' Checks whether the given user is already a member of the given group
        ''' </summary>
        ''' <param name="groupID"></param>
        ''' <param name="userId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsAlreadyMember(ByVal groupId As Integer, ByVal userId As Long) As Boolean
            Dim commandText As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT count([ID]) FROM [dbo].[Memberships] WHERE [ID_Group] = @GroupID AND [ID_User] = @UserID"

            Dim cmd As New SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId

            Dim RecordCount As Integer = CType(ExecuteScalar(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Return RecordCount > 0

        End Function

        Private Sub btnOKClick(ByVal sender As Object, ByVal args As EventArgs) Handles btnOK.Click
            Dim createMembership As Boolean

            If drp_groups Is Nothing OrElse drp_groups.SelectedItem Is Nothing OrElse Trim(drp_groups.SelectedItem.Text) = Nothing Then
                'No group available - no membership adding possible
                lblMsg.Text &= "No group selected"
                Return
            End If
            Dim dropGroupText As String = Trim(drp_groups.SelectedItem.Text)
            Dim dropGroupID As Integer = CInt(drp_groups.SelectedValue)

            Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex()
            Dim requiredFlags As String() = Nothing
            If MyDBVersion.Build >= 147 Then
                requiredFlags = GetRequiredApplicationFlags(dropGroupID)
            End If

            For Each MyListItem As UI.Control In rptUserList.Controls
                For Each MyControl As UI.Control In MyListItem.Controls
                    If MyControl.GetType Is GetType(System.Web.UI.WebControls.CheckBox) AndAlso MyControl.ID = "chk_user" Then

                        Dim chk As CheckBox = CType(MyControl, CheckBox)

                        Dim ErrorMessage As String = String.Empty
                        Dim SuccessMessage As String = String.Empty

                        If chk.Checked Then
                            Dim loginName As String = CType(MyListItem.FindControl("lblLoginName"), UI.HtmlControls.HtmlAnchor).InnerText.Trim()
                            Dim chosenUserId As Integer = CInt(CType(MyListItem.FindControl("lblID"), Label).Text)

                            If IsAlreadyMember(dropGroupID, chosenUserId) Then
                                ErrorMessage &= "User " + Server.HtmlEncode(loginName) + " is already  member of group " + Server.HtmlEncode(dropGroupText) & "<br />"
                                createMembership = False
                            ElseIf Not ((cammWebManager.System_GetSubAuthorizationStatus("Groups", dropGroupID, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") OrElse cammWebManager.System_GetSubAuthorizationStatus("Groups", dropGroupID, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                                Response.Write("No authorization to administrate this group.")
                                Response.End()
                            ElseIf loginName <> "" And dropGroupText <> "" Then
                                Dim userId As Long = CType(chosenUserId, Long)
                                Dim userInfo As WMSystem.UserInformation = cammWebManager.System_GetUserInfo(userId)

                                'Check for required flags
                                If MyDBVersion.Build >= 147 Then
                                    Dim flagsUpdateLinks As String = String.Empty
                                    If Not userInfo Is Nothing Then
                                        Dim validationResults As FlagValidation.FlagValidationResult() = FlagValidation.GetFlagValidationResults(userInfo, requiredFlags)
                                        flagsUpdateLinks &= CompuMaster.camm.WebManager.Administration.Utils.FormatLinksToFlagUpdatePages(validationResults, userInfo)
                                    End If

                                    If flagsUpdateLinks <> String.Empty Then
                                        ErrorMessage &= "The following errors occurred while checking the flags for user " & loginName & ":<br>" & flagsUpdateLinks
                                        createMembership = False
                                    End If
                                End If


                                'Create Membership
                                If createMembership Then
                                    Try
                                        'Use implemented workflow of camm WebManager to send Authorization-Mail if user is authorized the first time
                                        'HINT: SapFlag checks will be checked continually, but no information on missing flag name is given here any more
                                        userInfo.AddMembership(dropGroupID, CType(Nothing, WebManager.Notifications.INotifications))
                                        SuccessMessage &= loginName + " has been authorized for membership in group " + dropGroupText & "<br />"
                                    Catch ex As Exception
                                        ErrorMessage &= "User " & loginName & ": Membership creation failed! (" & ex.Message & ")<br />"
                                    End Try
                                End If
                            Else
                                ErrorMessage &= "Please specify a group and a user to proceed!<br />"
                            End If

                        End If

                        If SuccessMessage <> "" Then
                            lblMsg.Text &= SuccessMessage
                        End If

                        If ErrorMessage <> "" Then
                            lblErr.Text &= ErrorMessage
                        End If

                    End If
                    createMembership = True
                Next
            Next
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.Membership_Delete
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete a membership
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	9.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Membership_Delete
        Inherits Page

#Region "Variable Declaration"
        Dim Redirect2URL, ErrMsg, Field_Name, Field_Descr, Field_CompleteName As String
        Dim Field_ID, Field_GroupID As Integer
        Dim Field_UserID As Long
        Dim MyUserInfo As CompuMaster.camm.webmanager.WMSystem.UserInformation
        Dim dt As New DataTable
        Protected lblMembershipID, lblGroupName, lblGroupDescription, lblLoginName, lblCompleteName, lblErrMsg As Label
        Protected ancDelete, ancTouch As HtmlAnchor
#End Region

#Region "Page Event"
        Private Sub Membership_Delete_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Dim CurUserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                Try
                    Dim sqlParams1 As SqlParameter() = {New SqlParameter("@ID", Request.QueryString("ID")), _
                        New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), _
                        New SqlParameter("@compare", CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))))}
                    Dim mySqlQuery As String = "DELETE FROM dbo.Memberships WHERE ID=@ID and (0 <> @compare OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid=@UserID AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','Owner')))"
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
            dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "SELECT * FROM dbo.view_Memberships WHERE ID_Membership=@ID_Membership and (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','Owner')))", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Redirect2URL = "memberships.aspx"
            Else
                With dt.Rows(0)
                    Field_ID = Utils.Nz(.Item("ID_Membership"), 0)
                    Field_GroupID = Utils.Nz(.Item("ID_Group"), 0)
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

            MyUserInfo = New CompuMaster.camm.webmanager.WMSystem.UserInformation(CLng(Field_UserID), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
            lblMembershipID.Text = Server.HtmlEncode(Utils.Nz(Field_ID, 0).ToString)
            lblGroupName.Text = Server.HtmlEncode(Utils.Nz(Field_Name, String.Empty))
            lblGroupDescription.Text = Server.HtmlEncode(Utils.Nz(Field_Descr, String.Empty))
            lblLoginName.Text = Server.HtmlEncode(Utils.Nz(MyUserInfo.LoginName, String.Empty))
            lblCompleteName.Text = Server.HtmlEncode(Utils.Nz(Field_CompleteName, String.Empty))

            ancDelete.HRef = "memberships_delete.aspx?ID=" & Request.QueryString("ID") & "&DEL=NOW&GROUPID=" & Field_GroupID & "&token=" & Session.SessionID
            ancTouch.HRef = "memberships.aspx?GROUPID=" & Field_GroupID
        End Sub
#End Region

    End Class

End Namespace


