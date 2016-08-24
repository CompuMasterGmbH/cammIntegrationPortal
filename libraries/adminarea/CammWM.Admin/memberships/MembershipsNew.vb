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
    '''     A page to add new members to an existing group
    ''' </summary>
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
        ''' <summary>
        ''' Bind user data to the repeater control
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
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
        ''' Checks whether the given user is already a member of the given group
        ''' </summary>
        ''' <param name="groupID"></param>
        ''' <param name="userId"></param>
        ''' <remarks></remarks>
        Private Function IsAlreadyMember(ByVal groupId As Integer, ByVal userId As Long, denyRule As Boolean) As Boolean
            Dim commandText As String
            If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                commandText = "SELECT count([ID]) FROM [dbo].[Memberships] WHERE [ID_Group] = @GroupID AND [ID_User] = @UserID AND IsDenyRule = @DenyRule"
                Dim cmd As New SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId
                cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId
                cmd.Parameters.Add("@DenyRule", SqlDbType.Bit).Value = denyRule
                Dim RecordCount As Integer = CType(ExecuteScalar(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
                Return RecordCount > 0
            Else
                If denyRule = True Then Return False 'Deny rule not existant yet
                commandText = "SELECT count([ID]) FROM [dbo].[Memberships] WHERE [ID_Group] = @GroupID AND [ID_User] = @UserID"
                Dim cmd As New SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userId
                cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId
                Dim RecordCount As Integer = CType(ExecuteScalar(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
                Return RecordCount > 0
            End If
        End Function

        Private Sub btnOKClick(ByVal sender As Object, ByVal args As EventArgs) Handles btnOK.Click

            If drp_groups Is Nothing OrElse drp_groups.SelectedItem Is Nothing OrElse Trim(drp_groups.SelectedItem.Text) = Nothing Then
                'No group available - no membership adding possible
                lblMsg.Text &= "No group selected"
                Return
            End If
            Dim dropDownGroupText As String = Trim(drp_groups.SelectedItem.Text)
            Dim dropDownGroupID As Integer = CInt(drp_groups.SelectedValue)
            Dim dropDownGroupInfo As New WebManager.WMSystem.GroupInformation(dropDownGroupID, Me.cammWebManager)


            Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex()
            Dim requiredFlags As String() = Nothing
            If MyDBVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then
                requiredFlags = dropDownGroupInfo.RequiredAdditionalFlags
            End If

            For Each MyListItem As UI.Control In rptUserList.Controls
                For Each MyControl As UI.Control In MyListItem.Controls
                    If MyControl.GetType Is GetType(System.Web.UI.WebControls.CheckBox) AndAlso MyControl.ID = "chk_user" Then
                        AddMembershipRule(MyDBVersion, MyListItem, CType(MyControl, CheckBox), False, dropDownGroupText, dropDownGroupID, dropDownGroupInfo, requiredFlags)
                    ElseIf MyControl.GetType Is GetType(System.Web.UI.WebControls.CheckBox) AndAlso MyControl.ID = "chk_denyuser" Then
                        AddMembershipRule(MyDBVersion, MyListItem, CType(MyControl, CheckBox), True, dropDownGroupText, dropDownGroupID, dropDownGroupInfo, requiredFlags)
                    End If
                Next
            Next
        End Sub

        Private Sub AddMembershipRule(MyDBVersion As Version, MyListItem As UI.Control, checkboxControl As System.Web.UI.WebControls.CheckBox, isDenyRule As Boolean, dropDownGroupText As String, dropDownGroupID As Integer, dropDownGroupInfo As WebManager.WMSystem.GroupInformation, requiredFlags As String())
            Dim ErrorMessage As String = String.Empty
            Dim SuccessMessage As String = String.Empty

            If checkboxControl.Checked Then
                Dim createMembership As Boolean = True
                Dim loginName As String = CType(MyListItem.FindControl("lblLoginName"), UI.HtmlControls.HtmlAnchor).InnerText.Trim()
                Dim chosenUserId As Integer = CInt(CType(MyListItem.FindControl("lblID"), Label).Text)

                If IsAlreadyMember(dropDownGroupID, chosenUserId, isDenyRule) Then
                    ErrorMessage &= "User " + Server.HtmlEncode(loginName) + " is already  member of group " + Server.HtmlEncode(dropDownGroupText) & "<br />"
                    createMembership = False
                ElseIf Not ((cammWebManager.System_GetSubAuthorizationStatus("Groups", dropDownGroupID, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") OrElse cammWebManager.System_GetSubAuthorizationStatus("Groups", dropDownGroupID, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                    Response.Write("No authorization to administrate this group.")
                    Response.End()
                ElseIf loginName <> "" And dropDownGroupText <> "" Then
                    Dim userId As Long = CType(chosenUserId, Long)
                    Dim userInfo As WMSystem.UserInformation = cammWebManager.System_GetUserInfo(userId)

                    'Check for required flags
                    If isDenyRule = False AndAlso MyDBVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then
                        Dim flagsUpdateLinks As String = String.Empty
                        If Not userInfo Is Nothing Then
                            Dim validationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(userInfo, requiredFlags, True)
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
                            userInfo.AddMembership(dropDownGroupID, isDenyRule, CType(Nothing, WebManager.Notifications.INotifications))
                            If isDenyRule Then
                                SuccessMessage &= loginName + " has been denied for membership in group " + dropDownGroupText & "<br />"
                            Else
                                SuccessMessage &= loginName + " has been added as member to group " + dropDownGroupText & "<br />"
                            End If
                        Catch ex As Exception
                            If isDenyRule Then
                                ErrorMessage &= "User " & loginName & ": Membership deny-rule creation failed! (" & ex.Message & ")<br />"
                            Else
                                ErrorMessage &= "User " & loginName & ": Membership creation failed! (" & ex.Message & ")<br />"
                            End If
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

        End Sub
#End Region

    End Class

End Namespace


