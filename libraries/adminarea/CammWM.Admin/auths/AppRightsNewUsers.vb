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
    '''     A page to add any user to a perticular application
    ''' </summary>
    Public Class AppRightsNewUsers
        Inherits Page

#Region "Variable Declaration"
        Dim CurUserID As Long
        Dim ErrMSg As String
        Dim ErrMSg2_HTML As String
        Protected lblErr, lblMsg, lblNoRecMsg, lblID, lblLoginName As Label
        Protected WithEvents CheckBoxTop50Results As CheckBox
        Protected WithEvents btnOK, searchUserBtn As Button
        Protected SearchUsersTextBox As TextBox
        Protected WithEvents drp_App As DropDownList
        Protected WithEvents rptUserList As Repeater
        Protected MyDt As DataTable
        Protected WithEvents chk_deny As CheckBox
        Protected WithEvents chk_devteam As CheckBox
#End Region

#Region "Page Events"
        Private Sub AppRightsNewUsers_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErr.Text = ""
            lblMsg.Text = ""
            If Not IsPostBack Then
                ListApps()
                If Not drp_App.SelectedValue = "" Then
                    ListOfUsers(CInt(drp_App.SelectedValue))
                End If
            End If
        End Sub
#End Region

#Region "User-Defined Methods"

        Protected Sub ListOfUsers(ByVal AppID As Integer)
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
                sb.Append("SELECT " & TopClause & " System_AccessLevels.Title As AccessLevel_Title, Benutzer.*, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS UserNameComplete FROM [Benutzer] LEFT JOIN System_AccessLevels ON Benutzer.AccountAccessability = System_AccessLevels.ID" & vbNewLine)
                sb.Append(WhereClause & " ORDER BY Nachname, Vorname" & vbNewLine)

                Dim SqlQuery As String = sb.ToString

                Dim cmd As New System.Data.SqlClient.SqlCommand(SqlQuery, New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@SearchWords", SqlDbType.NVarChar).Value = SearchWords
                cmd.Parameters.Add("@AppID", SqlDbType.Int).Value = AppID
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
        ''' <summary>
        ''' List all available applications and bind them on the DropDownList. PreSelect application where user comes from.
        ''' </summary>
        Private Sub ListApps()
            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
            Dim WhereClause As String = "WHERE (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('UpdateRelations','Owner')))) "
            Dim sql As String = "SELECT Applications.ID, Applications.Title, Applications.AppDisabled, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID " & WhereClause & " ORDER BY Title"

            Dim AppTable As DataTable = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sql, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            For Each row As DataRow In AppTable.Rows
                row("Title") = Utils.Nz(row("Title"), String.Empty) & " (ID " & Utils.Nz(row("ID"), String.Empty) & Utils.Nz(IIf(Utils.Nz(row("AppDisabled"), False) = True, " (D)", ""), String.Empty) & " / " & Utils.Nz(row("ServerDescription"), String.Empty) & " / " & Utils.Nz(row("Description"), String.Empty) & ")"
            Next

            Me.drp_App.DataSource = AppTable
            Me.drp_App.DataTextField = "Title"
            Me.drp_App.DataValueField = "ID"
            Me.drp_App.DataBind()

            If Not Request.QueryString("ID") = "" Then
                Me.drp_App.SelectedValue = Request.QueryString("ID")
            End If

        End Sub

#End Region

#Region "Control Events"
        ''' <summary>
        ''' Is a checkbox checked or unchecked/missing?
        ''' </summary>
        ''' <param name="control"></param>
        Private Function IsChecked(control As CheckBox) As Boolean
            If control Is Nothing Then
                Return False
            Else
                Return control.Checked
            End If
        End Function

        Private Sub searchUserBtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles searchUserBtn.Click
            ListOfUsers(CInt(drp_App.SelectedValue))
        End Sub

        Private Sub top50ChkCheckChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBoxTop50Results.CheckedChanged
            ListOfUsers(CInt(drp_App.SelectedValue))
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
                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Company"), String.Empty))
                End With
            End If
        End Sub
        ''' <summary>
        ''' Load new user data if selected application changes
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub drpAppsSelectedAppChanged(ByVal sender As Object, ByVal e As EventArgs) Handles drp_App.SelectedIndexChanged
            ListOfUsers(CInt(CType(sender, DropDownList).SelectedValue))
        End Sub

        Public Function IsUserAlreadyAuthorized(ByVal userID As Long, ByVal applicationID As Integer) As Boolean
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                Throw New NotSupportedException("DbVersion.Build >= " & WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule.ToString & " requires calling an overloaded version of this method")
            End If
            Dim commandText As String = "SELECT count([ID]) FROM [dbo].[ApplicationsRightsByUser] WHERE [ID_GroupOrPerson] = @UserID  AND [ID_Application] =  @AppID"
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = userID
            MyCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = applicationID
            Dim RecordCount As Integer = CType(ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Return CInt(RecordCount) > 0
        End Function

        Public Function IsUserAlreadyAuthorized(ByVal userID As Long, ByVal applicationID As Integer, isDev As Boolean, isDenyRule As Boolean) As Boolean
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then 'Older
                Throw New NotSupportedException("DbVersion.Build < " & WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule.ToString & " requires calling an overloaded version of this method")
            End If
            Dim commandText As String = "SELECT count([ID]) FROM [dbo].[ApplicationsRightsByUser] WHERE [ID_GroupOrPerson] = @UserID  AND [ID_Application] =  @AppID AND IsNull(IsDenyRule, 0) = @IsDenyRule AND IsNull(DevelopmentTeamMember, 0) = @IsDev"
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@UserID", SqlDbType.BigInt).Value = userID
            MyCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = applicationID
            MyCmd.Parameters.Add("@IsDev", SqlDbType.Bit).Value = isDev
            MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
            Dim RecordCount As Integer = CType(ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Return CInt(RecordCount) > 0
        End Function

        Public Function GetRequiredUserFlagsForApplication(ByVal applicationID As Integer) As String()
            Dim commandParams As SqlParameter() = {New SqlParameter("@AppID", applicationID)}
            Dim commandText As String = "Select RequiredUserProfileFlags  From Applications_CurrentAndInactiveOnes Where ID = @AppID" '& dropAppID
            Dim RequiredUserFlags As String = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToString()

            If RequiredUserFlags = String.Empty Then
                Return New String() {}
            End If

            Return RequiredUserFlags.Split(CChar(","))

        End Function

        Private Sub btnOKClick(ByVal sender As Object, ByVal args As EventArgs) Handles btnOK.Click

            Dim dropAppID As Integer = CInt(drp_App.SelectedValue)
            Dim dropAppText As String = drp_App.SelectedItem.Text

            Dim arrRequiredUserFlags() As String = GetRequiredUserFlagsForApplication(dropAppID)
            Dim flagsUpdateLinks As String = String.Empty

            For Each MyListItem As UI.Control In rptUserList.Controls
                For Each MyControl As UI.Control In MyListItem.Controls
                    Dim authorize As Boolean = True
                    If MyControl.GetType Is GetType(System.Web.UI.WebControls.CheckBox) AndAlso (MyControl.ID = "chk_user" Or MyControl.ID = "chk_devteam" Or MyControl.ID = "chk_deny") Then
                        If MyControl.ID <> "chk_user" Then
                            'only create authorization once and never again on next controls in For-Each-Loop
                            authorize = False
                        End If

                        If authorize Then

                            Dim chk As CheckBox = CType(MyControl, CheckBox)
                            If chk.Checked OrElse IsChecked(CType(MyListItem.FindControl("chk_deny"), CheckBox)) = True Then
                                Dim dropUserText As String = CType(MyListItem.FindControl("lblLoginName"), UI.HtmlControls.HtmlAnchor).InnerText
                                Dim dropUserID As Integer = CInt(CType(MyListItem.FindControl("lblID"), Label).Text)

                                Dim userInfo As WMSystem.UserInformation = cammWebManager.System_GetUserInfo(CType(dropUserID, Long))

                                'Show or hide controls for MilestoneDBVersion_AuthsWithSupportForDenyRule depending on environment support
                                'If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                                'Hint: what is:
                                ' - If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then 'Older
                                ' - If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Equal OR Newer
                                If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 AndAlso IsUserAlreadyAuthorized(dropUserID, dropAppID) Then
                                    lblErr.Text &= "User " + dropUserText.ToString() + " is already authorized for application " + dropAppText.Trim & "<br />"
                                    authorize = False
                                ElseIf Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 AndAlso MyListItem.FindControl("chk_deny") IsNot Nothing AndAlso IsUserAlreadyAuthorized(dropUserID, dropAppID, IsChecked(CType(MyListItem.FindControl("chk_devteam"), CheckBox)), IsChecked(CType(MyListItem.FindControl("chk_deny"), CheckBox))) Then
                                    lblErr.Text &= "User " + dropUserText.ToString() + " (development access: " & IsChecked(CType(MyListItem.FindControl("chk_devteam"), CheckBox)).ToString.ToLower & ", deny rule: " & IsChecked(CType(MyListItem.FindControl("chk_deny"), CheckBox)).ToString.ToLower & ") is already authorized for application " + dropAppText.Trim & "<br />"
                                    authorize = False
                                ElseIf Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 AndAlso IsUserAlreadyAuthorized(dropUserID, dropAppID, IsChecked(CType(MyListItem.FindControl("chk_devteam"), CheckBox)), False) Then
                                    lblErr.Text &= "User " + dropUserText.ToString() + " is already authorized for application " + dropAppText.Trim & "<br />"
                                    authorize = False
                                ElseIf Not ((cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                                    Response.Write("No authorization To administrate this application.")
                                    Response.End()
                                ElseIf dropAppText.ToString.Trim() <> "" And dropUserText.ToString.Trim() <> "" Then
                                    If IsChecked(CType(MyListItem.FindControl("chk_deny"), CheckBox)) = False AndAlso Me.CurrentDbVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then 'Newer
                                        'Allow-rule requires flag-check
                                        If Not userInfo Is Nothing Then
                                            Dim validationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(userInfo, arrRequiredUserFlags, True)
                                            flagsUpdateLinks &= CompuMaster.camm.WebManager.Administration.Utils.FormatLinksToFlagUpdatePages(validationResults, userInfo)
                                        End If
                                    End If

                                    If flagsUpdateLinks <> String.Empty Then
                                        authorize = False
                                    End If

                                    If authorize Then
                                        Try
                                            'Use implemented workflow of camm WebManager to send Authorization-Mail if user is authorized the first time
                                            'HINT: SapFlag checks will be checked continually, but no information on missing flag name is given here any more
                                            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then 'Older
                                                userInfo.AddAuthorization(dropAppID, CType(Nothing, Integer), CType(MyListItem.FindControl("chk_devteam"), CheckBox).Checked, False, CType(Nothing, Notifications.INotifications))
                                            Else
                                                userInfo.AddAuthorization(dropAppID, CType(Nothing, Integer), CType(MyListItem.FindControl("chk_devteam"), CheckBox).Checked, IsChecked(CType(MyListItem.FindControl("chk_deny"), CheckBox)), CType(Nothing, Notifications.INotifications))
                                            End If

                                            lblMsg.Text &= dropUserText + " has been authorized for application " + dropAppText & "<br />"
                                        Catch ex As CompuMaster.camm.WebManager.FlagValidation.RequiredFlagException
                                            For MyCounter As Integer = 0 To ex.ValidationResults.Length - 1
                                                ErrMSg &= "Authorization creation failed for user ID " & ex.ValidationResults(MyCounter).UserID & "! (" & [Enum].GetName(GetType(CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode), ex.ValidationResults(MyCounter).ValidationResult) & ": " & ex.ValidationResults(MyCounter).Flag & ")<br />"
                                            Next
                                        Catch ex As Exception
                                            ErrMSg &= "Authorization creation failed for user ID " & dropUserID & "! (" & ex.Message & ")<br />"
                                        End Try
                                    End If
                                Else
                                    ErrMSg &= "Please specify an application and a user to proceed!<br />"
                                End If

                                If ErrMSg <> "" Then lblErr.Text = Utils.Nz(ErrMSg, String.Empty)

                            End If
                        End If
                    End If
                Next
            Next

            If flagsUpdateLinks <> String.Empty Then
                lblErr.Text = "Couldn't update the authorization due to problems with the following additional flags:<br />" & flagsUpdateLinks
                lblErr.Text &= "<br>Please make sure the users have the required additional flags and that the value is correct for the specific type of the flag."
            End If

        End Sub
#End Region

    End Class

End Namespace