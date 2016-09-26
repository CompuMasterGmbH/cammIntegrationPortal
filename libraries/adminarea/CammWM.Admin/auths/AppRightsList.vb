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

    '''<summary>
    '''    A page to view list of application for authorization
    '''</summary>
    Public Class AppRightsList
        Inherits AuthorizationsOverview

#Region "Variable Declaration"
        Protected WithEvents rptAppList, rptShowUsers As Repeater
        Protected txtSearchApp, txtSearchUser As TextBox
        Protected btnSubmit As Button
        Protected hypTitle, ancAdditionalAuth, ancTransmission, ancExport As HyperLink
        Protected ancAuthsAsAppID, ancAddGroupShowDetails, ancAddUser, ancSecurity, ancReleasedBy As HtmlAnchor
        Protected tdAddUserGroupDetails, tdErrMsg, tdSearchApp, tdAddLinks, tdAddUserDetails As HtmlTableCell
        Protected trAddBlank, trSubUser, trHeaders, trNoUserFound, trMain, trAddUserGroupDetails, trAddUserDetails As HtmlTableRow
        Protected gcDisabled As HtmlGenericControl
        Public iFieldCount As Integer
        Dim MyDt, dt, dtApps As DataTable
        Dim DA As SqlDataAdapter
        Dim NewAppID, OldAppID As Integer
        Dim FirstAppLine, DisplayNewHeaderUsers, DisplayNewHeaderGroups, DisplayShowAllUserLink, DisplayNewHeaderGroupsUsers As Boolean
        Dim CurUserIsSecurityOperator, CurUserIsSecurityMaster, CurUserIsSupervisor As Boolean
        Dim strQuery As String
        Public strShowAllUsers, strDeleteLink, strDeleteUserLink As String
        Dim tempStr As Text.StringBuilder
#End Region

#Region "Page Events"
        Private Sub AppRightsListLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Server.ScriptTimeout = 300
            tdErrMsg.InnerHtml = ""
        End Sub

        Private Sub AppRightsListPrerender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            ListOfRecords()
            BindControls()
        End Sub
#End Region

#Region "User-Defined Methods"
        Protected Sub ListOfRecords()
            Dim WhereClause As New Text.StringBuilder
            Dim TopClause As String
            Dim CurUserIsSecurityOperator As Boolean
            Dim CurUserIsSecurityMaster As Boolean

            MyDt = New DataTable

            CurUserIsSecurityOperator = cammWebManager.System_IsSecurityOperator(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            CurUserIsSecurityMaster = cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))

            If CurUserIsSecurityMaster Then CurUserIsSecurityOperator = False
            WhereClause.Append(" WHERE ")

            If Utils.Nz(Request.QueryString("AuthsAsAppID"), 0) = 0 OrElse Request.QueryString("AuthsAsAppID") = "" Then
                If Not CurUserIsSecurityOperator And (Request.QueryString("ShowAllApp") = "" OrElse Utils.Nz(Request.QueryString("ShowAllApp"), 0) = 1) Then WhereClause.Append(" AuthsAsAppID Is Null And ")
            Else
                WhereClause.Append(" AuthsAsAppID = @AuthsAsAppID And ")
            End If

            If Request.QueryString("Application") <> "" Then
                WhereClause.Append(" ID_Application = @AppID And ")
            ElseIf Trim(txtSearchApp.Text.ToString & "") <> "" Then
                WhereClause.Append(" Title LIKE @Title And ")
                WhereClause = WhereClause.Replace(" AuthsAsAppID Is Null And ", "")
            ElseIf Utils.Nz(Request.QueryString("ShowSystemApps"), 0) = 1 Then
                WhereClause.Append(" (SystemApp <> 0 AND SystemAppType <> 3) And ")
            ElseIf Request.QueryString("DisplayAll") = "" OrElse Utils.Nz(Request.QueryString("DisplayAll"), 0) = 1 Then
                WhereClause.Append(" (SystemApp = 0 OR SystemAppType = 3) And ")
            End If

            Dim SearchString As String = txtSearchUser.Text.Trim.Replace("'", "''").Replace("*", "%")
            Dim searchItems As String() = SearchString.Split(" "c)

            If Trim(txtSearchUser.Text & "") <> "" Then
                'to apply search for user/group from all application
                WhereClause = WhereClause.Replace(" AuthsAsAppID Is Null And ", "")

                If InStr(txtSearchUser.Text.Trim, " ") > 0 Then
                    WhereClause.Append(" Loginname LIKE @searchstring Or (Name1  LIKE @searchstring or Nachname LIKE @searchstring")
                    WhereClause.Append(" Or Name1  LIKE @searchstring or Vorname LIKE @searchstring)")
                    For mySearchCounter As Integer = 0 To searchItems.Length - 1
                        WhereClause.Append(" Or (Loginname LIKE @searchitem" & mySearchCounter & ") Or (Name1  LIKE @searchitem" & mySearchCounter & " And Nachname LIKE @searchitem" & mySearchCounter & ")")
                        WhereClause.Append(" Or (Name1  LIKE @searchitem" & mySearchCounter & " And Vorname LIKE @searchitem" & mySearchCounter & ")")
                    Next
                    WhereClause.Append(" Or ")
                Else
                    WhereClause.Append(" (Nachname LIKE @UserName Or Vorname LIKE @UserName Or Loginname LIKE @UserName Or Name LIKE @UserName) And ")
                End If
            End If

            WhereClause.Append(" (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments Where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Applications' AND AuthorizationType In ('SecurityMaster','ViewAllRelations')) OR id_application in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND id_application in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner')))) ")

            TopClause = ""

            strQuery = _
                "select " & TopClause & " ID_Application,AppDisabled,AuthsAsAppID,TitleAdminArea,Title,AppReleasedByVorname,AppReleasedByID," & _
                "AppReleasedByNachname,AppReleasedOn,NavURL,(select top 1 Description from Languages l where l.ID=view_ApplicationRights.LanguageID) As Abbreviation,(select top 1 ServerDescription from System_Servers s where s.ID=view_ApplicationRights.LocationID) as ServerDescription " & _
                ",(SELECT top 1 AuthsAsAppID FROM Applications a WHERE a.ID = view_ApplicationRights.[AuthsAsAppID]) as NextAuthsAsAppID from [view_ApplicationRights] " & WhereClause.ToString & " group by ID_Application,AppDisabled,AuthsAsAppID,TitleAdminArea,Title,AppReleasedByVorname," & _
                "AppReleasedByID,AppReleasedByNachname,AppReleasedOn,NavURL,languageid,LocationID,TitleAdminAreaDisplay order by TitleAdminAreaDisplay,LocationID,id_application"

            Dim cmd As New SqlCommand(strQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@AuthsAsAppID", SqlDbType.Int).Value = CInt(Val(Request.QueryString("AuthsAsAppID")))
            cmd.Parameters.Add("@AppID", SqlDbType.Int).Value = CLng(Request.QueryString("Application"))
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = txtSearchApp.Text.ToString.Trim.Replace("'", "''").Replace("*", "%") & "%"
            cmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = txtSearchUser.Text.ToString.Trim.Replace("'", "''").Replace("*", "%") & "%"
            cmd.Parameters.Add("@UserID", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
            cmd.Parameters.Add("@SearchString", SqlDbType.NVarChar).Value = SearchString
            For myCounter As Integer = 0 To searchItems.Length - 1
                cmd.Parameters.Add("@SearchItem" & myCounter, SqlDbType.NVarChar).Value = searchItems(myCounter)
            Next
            dtApps = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            MyDt.Dispose()
            WhereClause = Nothing
        End Sub

        Private Sub BindControls()
            Dim FirstAppLine, CurUserIsSecurityOperator, CurUserIsSecurityMaster, CurUserIsSupervisor As Boolean

            CurUserIsSecurityOperator = cammWebManager.System_IsSecurityOperator(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            CurUserIsSecurityMaster = cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            CurUserIsSupervisor = cammWebManager.System_IsSuperVisor(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            If CurUserIsSecurityOperator AndAlso (CurUserIsSecurityMaster Or CurUserIsSupervisor) Then CurUserIsSecurityOperator = False

            iFieldCount = CInt(Val(MyDt.Columns.Count & ""))
            If Trim(Request.QueryString("Application")) = "" Then tdSearchApp.Visible = True Else tdSearchApp.Visible = False
            Dim strBlr As New Text.StringBuilder

            If (Trim(Request.QueryString("Application") & "") <> "" And Utils.Nz(Request.QueryString("DisplayAll"), 0) <> 1) OrElse Utils.Nz(Request.QueryString("ShowSystemApps"), 0) = 1 Then strBlr.Append("<font size=""2""><a href=""apprights.aspx?DisplayAll=1"">Display all</a></font>")
            If Utils.Nz(Request.QueryString("ShowSystemApps"), 0) <> 1 Then strBlr.Append("<font size=""2"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=""apprights.aspx?ShowSystemApps=1"">Display system applications</a></font>")
            If Not CurUserIsSecurityOperator AndAlso (Trim(Request.QueryString("Application")) = "" AndAlso (Request.QueryString("ShowallApp") = "" OrElse Utils.Nz(Request.QueryString("ShowallApp"), 0) = 1)) Then strBlr.Append("<font face=""Arial"" size=""2"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=""apprights.aspx?ShowallApp=2" & IIf(Utils.Nz(Request.QueryString("DisplayAll"), 0) = 1, "&DisplayAll=1", "").ToString & IIf(Utils.Nz(Request.QueryString("ShowSystemApps"), 0) = 1, "&ShowSystemApps=1", "").ToString & IIf(Utils.Nz(Request.QueryString("Showall"), 0) = 1, "&Showall=1", "").ToString & """>Show all applications</a></font>")
            If Not CurUserIsSecurityOperator AndAlso (Trim(Request.QueryString("Application")) = "" AndAlso Utils.Nz(Request.QueryString("ShowallApp"), 0) = 2) Then strBlr.Append("<font face=""Arial"" size=""2"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=""apprights.aspx?ShowallApp=1" & IIf(Utils.Nz(Request.QueryString("DisplayAll"), 0) = 1, "&DisplayAll=1", "").ToString & IIf(Utils.Nz(Request.QueryString("ShowSystemApps"), 0) = 1, "&ShowSystemApps=1", "").ToString & IIf(Utils.Nz(Request.QueryString("Showall"), 0) = 1, "&Showall=1", "").ToString & """>Hide inheriting applications</a></font>")
            If CurUserIsSecurityMaster Then strBlr.Append("<font face=""Arial"" size=""2"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=""adjust_delegates.aspx?ID=0&Type=Applications&Title=All+applications"" title=""Adjust administrative delegates"">Security</a></font>")
            If Not CurUserIsSecurityOperator AndAlso (Trim(Request.QueryString("Application")) = "" AndAlso Utils.Nz(Request.QueryString("Showall"), 0) <> 1) Then strBlr.Append("<font face=""Arial"" size=""2"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=""apprights.aspx?Showall=1" & IIf(Utils.Nz(Request.QueryString("DisplayAll"), 0) = 1, "&DisplayAll=1", "").ToString & IIf(Utils.Nz(Request.QueryString("ShowSystemApps"), 0) = 1, "&ShowSystemApps=1", "").ToString & IIf(Request.QueryString("ShowallApp") = "" Or Utils.Nz(Request.QueryString("ShowallApp"), 0) = 1, "&ShowallApp=1", "&ShowallApp=2").ToString & """>Show all groups/users</a></font>")
            If Not CurUserIsSecurityOperator AndAlso (Trim(Request.QueryString("Application")) = "" AndAlso Utils.Nz(Request.QueryString("Showall"), 0) = 1) Then strBlr.Append("<font face=""Arial"" size=""2"">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=""apprights.aspx?" & IIf(Utils.Nz(Request.QueryString("DisplayAll"), 0) = 1, "DisplayAll=1", "").ToString & IIf(Utils.Nz(Request.QueryString("ShowSystemApps"), 0) = 1, "&ShowSystemApps=1", "").ToString & IIf(Request.QueryString("ShowallApp") = "" Or Utils.Nz(Request.QueryString("ShowallApp"), 0) = 1, "&ShowallApp=1", "&ShowallApp=2").ToString & """>Hide all groups/users</a></font>")

            tdAddLinks.InnerHtml = strBlr.ToString
            strBlr = Nothing

            If True Then
                FirstAppLine = True

                If Not dtApps Is Nothing AndAlso dtApps.Rows.Count > 0 Then
                    rptAppList.DataSource = dtApps
                    rptAppList.DataBind()
                Else
                    tdErrMsg.InnerHtml = "There are no applications available for administration."
                End If
                MyDt.Dispose()
            End If
        End Sub
#End Region

#Region "Control Events"
        ''' <summary>
        ''' Bind the data of application/security object headers (and query depending users/groups JIT, too)
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub rptAppListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptAppList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                With dtApps.Rows(e.Item.ItemIndex)
                    NewAppID = Utils.Nz(.Item("ID_Application"), 0)

                    'If FirstAppLine Then FirstAppLine = False Else CType(e.Item.FindControl("trAddBlank"), HtmlTableRow).Style.Add("display", "")
                    If Not IsDBNull(.Item("AppDisabled")) Then If Utils.Nz(.Item("AppDisabled"), False) = True Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).Style.Add("display", "")
                    If Trim(Request.QueryString("Application")) = "" Then CType(e.Item.FindControl("ancAuthsAsAppID"), HtmlAnchor).HRef = "apprights.aspx?Application=" & Utils.Nz(.Item("ID_Application"), String.Empty) & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0) & ""

                    If Utils.Nz(.Item("TitleAdminArea"), String.Empty) = "" Then
                        CType(e.Item.FindControl("hypTitle"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Title"), String.Empty)) & " (" & Server.HtmlEncode(Utils.Nz(.Item("Abbreviation"), String.Empty)) & ")" & "<br>" & Server.HtmlEncode(Utils.Nz(.Item("ServerDescription"), ""))
                    Else
                        CType(e.Item.FindControl("hypTitle"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("TitleAdminArea"), String.Empty)) & " (" & Server.HtmlEncode(Utils.Nz(.Item("Abbreviation"), String.Empty)) & ")" & "<br>" & Server.HtmlEncode(Utils.Nz(.Item("ServerDescription"), ""))
                    End If

                    If Trim(Request.QueryString("Application")) = "" Then CType(e.Item.FindControl("hypTitle"), HyperLink).NavigateUrl = "apprights.aspx?Application=" & Utils.Nz(.Item("ID_Application"), String.Empty) & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0).ToString & ""
                    If Not IsDBNull(.Item("AuthsAsAppID")) Then
                        CType(e.Item.FindControl("ancAdditionalAuth"), HyperLink).NavigateUrl = "apprights.aspx?Application=" & .Item("AuthsAsAppID").ToString & "&AuthsAsAppID=" & Utils.Nz(.Item("NextAuthsAsAppID"), 0).ToString
                        CType(e.Item.FindControl("ancAdditionalAuth"), HyperLink).Text = "<br>Additional authorizations from application ID " & .Item("AuthsAsAppID").ToString
                    End If

                    If Trim(Request.QueryString("Application")) <> "" Then
                        If Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Applications, AuthorizationTypeEffective.UpdateRelations, CType(.Item("ID_Application"), Integer)) Then
                            If IsDBNull(.Item("AuthsAsAppID")) Then
                                CType(e.Item.FindControl("ancTransmission"), HyperLink).NavigateUrl = "apprights_new_transmission.aspx?ID=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0).ToString & ""
                                CType(e.Item.FindControl("ancTransmission"), HyperLink).Text = "<br>Add Transmission"
                            Else
                                CType(e.Item.FindControl("ancTransmission"), HyperLink).NavigateUrl = "apprights_delete_transmission.aspx?ID=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0).ToString & ""
                                CType(e.Item.FindControl("ancTransmission"), HyperLink).Text = "<br>Delete Transmission"
                            End If
                        End If
                    End If

                    If Not IsDBNull(.Item("AppReleasedByID")) Then
                        CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("AppReleasedByID"), 0)
                        CType(e.Item.FindControl("ancReleasedBy"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(.Item("AppReleasedByNachname"), .Item("AppReleasedByVorname"), CLng(Utils.Nz(.Item("AppReleasedByID"), 0))))
                    End If

                    If Trim(Request.QueryString("Showall")) <> "" OrElse Trim(Request.QueryString("Application")) <> "" Then
                        CType(e.Item.FindControl("ancExport"), HyperLink).NavigateUrl = "apprights.aspx?action=export&SecurityObjectID=" & .Item("ID_Application").ToString
                        CType(e.Item.FindControl("ancExport"), HyperLink).Text = "<br />Export as CSV"
                    End If

                    If Trim(Request.QueryString("Showall")) <> "" OrElse Trim(Request.QueryString("Application")) <> "" Then
                        'Check for required flags
                        Dim sqlstr As String = "SELECT count([RequiredUserProfileFlags]) FROM [dbo].[Applications_CurrentAndInactiveOnes] where id = @AppID"
                        Dim cmd As New SqlCommand(sqlstr, New SqlConnection(cammWebManager.ConnectionString))
                        cmd.Parameters.Add("@AppID", SqlDbType.Int).Value = .Item("ID_Application").ToString
                        If Val(ExecuteScalar(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)) > 0 Then
                            CType(e.Item.FindControl("ancBatchUserFlagEditor"), HyperLink).NavigateUrl = "users_batchuserflageditor.aspx?AppID=" & .Item("ID_Application").ToString
                            CType(e.Item.FindControl("ancBatchUserFlagEditor"), HyperLink).Visible = True
                        End If
                    End If

                    If Trim(Request.QueryString("Showall")) <> "" OrElse Trim(Request.QueryString("Application")) <> "" OrElse CurUserIsSecurityOperator Then
                        If Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Applications, AuthorizationTypeEffective.UpdateRelations, CType(.Item("ID_Application"), Integer)) Then
                            CType(e.Item.FindControl("ancAddGroupShowDetails"), HtmlAnchor).HRef = "apprights_new_groups.aspx?ID=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & CInt(Request.QueryString("AuthsAsAppID"))
                            CType(e.Item.FindControl("ancAddGroupShowDetails"), HtmlAnchor).InnerHtml = "Add Group"
                            CType(e.Item.FindControl("ancAddUser"), HtmlAnchor).HRef = "apprights_new_users.aspx?ID=" & .Item("ID_Application").ToString & "&AuthsAsAppID=" & CInt(Request.QueryString("AuthsAsAppID"))
                            CType(e.Item.FindControl("ancAddUser"), HtmlAnchor).InnerHtml = "<br />Add User"
                        End If
                    Else
                        CType(e.Item.FindControl("ancAddGroupShowDetails"), HtmlAnchor).HRef = "apprights.aspx?Application=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0).ToString & ""
                        CType(e.Item.FindControl("ancAddGroupShowDetails"), HtmlAnchor).InnerHtml = "Show Details"
                    End If

                    If CurUserIsSecurityMaster Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(.Item("ID_Application")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Then
                        If IsDBNull(.Item("TitleAdminArea")) Then
                            CType(e.Item.FindControl("ancSecurity"), HtmlAnchor).HRef = "adjust_delegates.aspx?ID=" & .Item("ID_Application").ToString & "&Type=Applications&Title=" & Server.UrlEncode(Utils.Nz(.Item("Title"), String.Empty))
                        ElseIf Utils.Nz(.Item("TitleAdminArea"), String.Empty) = "" Then
                            CType(e.Item.FindControl("ancSecurity"), HtmlAnchor).HRef = "adjust_delegates.aspx?ID=" & .Item("ID_Application").ToString & "&Type=Applications&Title=" & Server.UrlEncode(Utils.Nz(.Item("Title"), String.Empty))
                        Else
                            CType(e.Item.FindControl("ancSecurity"), HtmlAnchor).HRef = "adjust_delegates.aspx?ID=" & .Item("ID_Application").ToString & "&Type=Applications&Title=" & Server.UrlEncode(Utils.Nz(.Item("TitleAdminArea"), String.Empty))
                        End If

                        CType(e.Item.FindControl("ancSecurity"), HtmlAnchor).InnerHtml = "<br />Security"
                    End If


                    If (Trim(Request.QueryString("Application")) <> "" OrElse Utils.Nz(Request.QueryString("Showall"), 0) = 1 OrElse CurUserIsSecurityOperator OrElse Trim(txtSearchUser.Text & "") <> "") Then
                        'Show all groups
                        dt = New DataTable
                        tempStr = New Text.StringBuilder

                        Dim sqlParams As SqlParameter() = {New SqlParameter("@AppID", NewAppID)}
                        If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                            'Older / without IsDenyRule column
                            strQuery = "select LoginDisabled,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_Group,DevelopmentTeamMember,Name,Vorname,Description,CAST (0 AS bit) AS IsDenyRule,CAST(0 AS int) AS ID_ServerGroup from [view_ApplicationRights] where id_application=@AppID and isnull(ID_Group,0)<>0 order by Name"
                        Else
                            'Newer / IsDenyRule column available
                            strQuery = "select LoginDisabled,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_Group,DevelopmentTeamMember,Name,Vorname,Description,IsDenyRule,ID_ServerGroup,IsSupervisorAutoAccessRule from [view_ApplicationRights] where id_application=@AppID and isnull(ID_Group,0)<>0 order by Name, IsDenyRule"
                        End If

                        dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                            'strDeleteLink = String.Empty
                            'CType(e.Item.FindControl("rptShowGroups"), Repeater).DataSource = MyDt
                            'CType(e.Item.FindControl("rptShowGroups"), Repeater).DataBind()

                            tempStr.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"">")
                            tempStr.Append("<TR>")
                            tempStr.Append("<TD WIDTH=""30px""> &nbsp;</TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1"" WIDTH=""60""><P class=""boldFont"">Group ID</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Rule</b>&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Name</b>&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Description</b>&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Action&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                            If Trim(Request.QueryString("Application") & "") <> "" Then
                                If Utils.Nz(Request.QueryString("ShowAllUsers"), 0) = 1 Then tempStr.Append("<a href=""apprights.aspx?&Application=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(dt.Rows(0)("AuthsAsAppID"), 0).ToString & """>Hide all users</a>") Else tempStr.Append("<a href=""apprights.aspx?&ShowAllUsers=1&Application=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(dt.Rows(0)("AuthsAsAppID"), 0).ToString & """>Show all users</a>")
                            End If
                            tempStr.Append("</P></TD></TR>")

                            For j As Integer = 0 To dt.Rows.Count - 1
                                Dim RowSeparator As String = ""
                                If j > 0 Then RowSeparator = " style=""border-top: solid 1px; border-color: lightgray;"""
                                tempStr.Append("<TR>")
                                tempStr.Append("<TD>&nbsp;</TD>")
                                tempStr.Append("<TD" & RowSeparator & "><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(dt.Rows(j)("ID_Group"), 0).ToString) & IIf(Utils.Nz(dt.Rows(j)("DevelopmentTeamMember"), False), "<b title=""Authorization for test and development purposes and for inactive security objects"">{Dev}</b>", "").ToString & "&nbsp;</P></TD>")
                                Dim RuleTitleForGroup As String
                                If MyDt.Columns.Contains("IsDenyRule") = True AndAlso Utils.Nz(dt.Rows(j).Item("IsDenyRule"), False) = True Then
                                    RuleTitleForGroup = "DENY"
                                Else
                                    RuleTitleForGroup = "GRANT"
                                End If
                                tempStr.Append("<TD" & RowSeparator & "><P class=""normalFont"" title=""Authorization for this group is set up as " & Server.HtmlEncode(RuleTitleForGroup) & """>" & Server.HtmlEncode(RuleTitleForGroup) & "</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & " WIDTH=""170""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(dt.Rows(j)("Name"), String.Empty)) & "</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & " WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(dt.Rows(j)("Description"), String.Empty)) & "&nbsp;</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & "><P class=""normalFont"">")
                                If dt.Columns.Contains("IsSupervisorAutoAccessRule") AndAlso Utils.Nz(dt.Rows(j)("IsSupervisorAutoAccessRule"), False) = True OrElse (dt.Columns.Contains("IsSupervisorAutoAccessRule") = False AndAlso IsDBNull(dt.Rows(j)("ThisAuthIsFromAppID")) = True AndAlso Utils.Nz(dt.Rows(j)("ID_Group"), 0) = 6 AndAlso Utils.Nz(dt.Rows(j)("DevelopmentTeamMember"), False) = True AndAlso Utils.Nz(dt.Rows(j)("IsDenyRule"), False) = False) Then
                                    'supervisor-auth - can't be deleted
                                    tempStr.Append("<em>Auto-Rule</em>")
                                ElseIf IsDBNull(dt.Rows(j)("ThisAuthIsFromAppID")) Then
                                    'standard auth - can be deleted
                                    If Not IsDBNull(dt.Rows(j)("ID_AppRight")) Then tempStr.Append("<a href=""apprights_delete_groups.aspx?ID=" & Utils.Nz(dt.Rows(j)("ID_AppRight"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(dt.Rows(j)("AuthsAsAppID"), 0).ToString & """>Delete</a>")
                                Else
                                    'inherited auth - can't be deleted, but should be identified as inherited auth
                                    tempStr.Append("<em>Inherited</em>")
                                End If
                                tempStr.Append("&nbsp;</P></TD>")
                                tempStr.Append("</TR>")

                                'Additional info on single-servergroup-rules
                                If dt.Columns.Contains("ID_ServerGroup") = True AndAlso CType(dt.Rows(j).Item("ID_ServerGroup"), Integer) <> 0 Then
                                    tempStr.Append("<TR>")
                                    tempStr.Append("<TD COLSPAN=""3"">&nbsp;</TD>")
                                    tempStr.Append("<TD COLSPAN=""2""><P class=""normalFont"">")
                                    tempStr.Append("<em>Applies only to server group: " & ServerGroupTitle(CType(dt.Rows(j).Item("ID_ServerGroup"), Integer)) & "</em>")
                                    tempStr.Append("&nbsp;</P></TD>")
                                    tempStr.Append("<TD COLSPAN=""1"">&nbsp;</TD>")
                                    tempStr.Append("</TR>")
                                End If

                                'Show all member users of current group
                                If Not Request.QueryString("ShowAllUsers") Is Nothing AndAlso Utils.Nz(Request.QueryString("ShowAllUsers"), 0) = 1 Then
                                    MyDt = New DataTable
                                    strQuery = "SELECT * " & vbNewLine & _
                                        "FROM [view_Memberships] " & vbNewLine & _
                                        "WHERE ID_Group = " & Utils.Nz(dt.Rows(j)("ID_Group"), 0).ToString & " " & vbNewLine & _
                                        "   And ID_Group not in " & vbNewLine & _
                                        "       (" & vbNewLine & _
                                        "           Select id_group_public " & vbNewLine & _
                                        "           from system_servergroups" & vbNewLine & _
                                        "       ) " & vbNewLine & _
                                        "   and ID_Group not in " & vbNewLine & _
                                        "       (" & vbNewLine & _
                                        "           Select id_group_anonymous " & vbNewLine & _
                                        "           from system_servergroups" & vbNewLine & _
                                        "       ) " & vbNewLine & _
                                        "   and (" & vbNewLine & _
                                        "       0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " " & vbNewLine & _
                                        "       OR 0 in " & vbNewLine & _
                                        "           (" & vbNewLine & _
                                        "               select tableprimaryidvalue " & vbNewLine & _
                                        "               from System_SubSecurityAdjustments " & vbNewLine & _
                                        "               Where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " " & vbNewLine & _
                                        "                   AND TableName = 'Applications' " & vbNewLine & _
                                        "                   AND AuthorizationType In ('SecurityMaster','ViewAllRelations')" & vbNewLine & _
                                        "           ) " & vbNewLine & _
                                        "       OR id_group in " & vbNewLine & _
                                        "           (" & vbNewLine & _
                                        "               select tableprimaryidvalue " & vbNewLine & _
                                        "               from System_SubSecurityAdjustments " & vbNewLine & _
                                        "               where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " " & vbNewLine & _
                                        "                   AND TableName = 'Applications' " & vbNewLine & _
                                        "                   AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner')" & vbNewLine & _
                                        "           )" & vbNewLine & _
                                        "       ) " & vbNewLine & _
                                        "ORDER BY Nachname, Name, ID_Group, Vorname"
                                    MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                    If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 AndAlso Not (MyDt.Rows.Count = 1 AndAlso IsDBNull(MyDt.Rows(0)("ID_User"))) Then
                                        tempStr.Append("<TR><TD>&nbsp;</TD>")
                                        tempStr.Append("<TD colspan=""5"">")
                                        tempStr.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"">")
                                        tempStr.Append("<TR><TD WIDTH=""50"">&nbsp;</TD>")
                                        tempStr.Append("<TD class=""boldFontHeader"" WIDTH=""60""><P class=""boldFont"">User ID</P></TD>")
                                        tempStr.Append("<TD class=""boldFontHeader""><P class=""boldFont"">Rule&nbsp;</P></TD>")
                                        tempStr.Append("<TD class=""boldFontHeader""><P class=""boldFont"">Name&nbsp;</P></TD>")
                                        tempStr.Append("<TD class=""boldFontHeader""><P class=""boldFont"">Login&nbsp;</P></TD>")
                                        tempStr.Append("<TD class=""boldFontHeader""><P class=""boldFont"">Company&nbsp;</P></TD>")
                                        tempStr.Append("</TR>")

                                        For i As Integer = 0 To MyDt.Rows.Count - 1
                                            Dim MemberRowSeparator As String = ""
                                            If i > 0 Then MemberRowSeparator = " style=""border-top: solid 1px; border-color: lightgray;"""
                                            tempStr.Append("<TR>")
                                            tempStr.Append("<TD>&nbsp;</TD>")
                                            tempStr.Append("<TD" & MemberRowSeparator & "><P class=""normalFont"">" & Utils.Nz(MyDt.Rows(i)("ID_User"), 0) & "&nbsp;</P></TD>")
                                            Dim RuleTitle As String
                                            If Utils.Nz(MyDt.Rows(i).Item("IsDenyRule"), False) = True Then
                                                RuleTitle = "DENY"
                                            Else
                                                RuleTitle = "GRANT"
                                            End If
                                            tempStr.Append("<TD" & MemberRowSeparator & "><P class=""normalFont"" title=""Membership for this user to this group is set up as " & Server.HtmlEncode(RuleTitle) & """>" & Server.HtmlEncode(RuleTitle) & "&nbsp;</P></TD>")
                                            tempStr.Append("<TD" & MemberRowSeparator & " WIDTH=""170""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Nachname"), String.Empty)) & ", " & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Vorname"), String.Empty)) & "&nbsp;</P></TD>")
                                            tempStr.Append("<TD" & MemberRowSeparator & " WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("LoginName"), String.Empty)) & "&nbsp;</P></TD>")
                                            tempStr.Append("<TD" & MemberRowSeparator & "><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Company"), String.Empty)) & "&nbsp;</P></TD>")
                                            tempStr.Append("</TR>")
                                        Next

                                        tempStr.Append("</TABLE></TD></TR>")
                                    Else
                                        tempStr.Append("<TR>")
                                        tempStr.Append("<TD WIDTH=""50"">&nbsp;</TD>")
                                        tempStr.Append("<TD VAlign=""Top"" colspan=""5""><P><FONT face=""Arial"" size=""1"">No users are member of this group (or no listing available for anonymous group and public group)</FONT></P></TD>")
                                        tempStr.Append("</TR>")
                                    End If
                                    'when showing users, add empty lines for better optical structure
                                    tempStr.Append("<TR>")
                                    tempStr.Append("<TD WIDTH=""50"">&nbsp;</TD>")
                                    tempStr.Append("<TD VAlign=""Top"" colspan=""5""><P>&nbsp;</P></TD>")
                                    tempStr.Append("</TR>")
                                End If
                            Next

                            tempStr.Append("</TABLE>")
                            CType(e.Item.FindControl("tdAddUserGroupDetails"), HtmlTableCell).InnerHtml = tempStr.ToString
                        End If

                        'Show authorized users
                        MyDt = New DataTable
                        tempStr = New Text.StringBuilder
                        Dim sqlParams1 As SqlParameter() = {New SqlParameter("@AppID", NewAppID)}
                        If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsAdminViewWithCompanyField) < 0 Then
                            'Older / without IsDenyRule column
                            strQuery = "select '' as companyname,name1,LoginDisabled,DevelopmentTeamMember,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_User,Nachname,Vorname,LoginName, CAST (0 AS bit) AS IsDenyRule,CAST(0 AS int) AS ID_ServerGroup from [view_ApplicationRights] where id_application=@AppID and isnull(ID_User,0)<>0 order by Nachname, CompanyName, IsDenyRule"
                        ElseIf Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                            'Older / without IsDenyRule column
                            strQuery = "select companyname,name1,LoginDisabled,DevelopmentTeamMember,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_User,Nachname,Vorname,LoginName, IsDenyRule,CAST(0 AS int) AS ID_ServerGroup from [view_ApplicationRights] where id_application=@AppID and isnull(ID_User,0)<>0 order by Nachname, CompanyName, IsDenyRule"
                        Else
                            'Newer / IsDenyRule column available
                            strQuery = "select companyname,name1,LoginDisabled,DevelopmentTeamMember,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_User,Nachname,Vorname,LoginName, IsDenyRule,ID_ServerGroup,IsSupervisorAutoAccessRule from [view_ApplicationRights] where id_application=@AppID and isnull(ID_User,0)<>0 order by Nachname, CompanyName, IsDenyRule"
                        End If
                        MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                        If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                            tempStr.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"" bordercolor=""#FFFFFF"">")
                            tempStr.Append("<TR>")
                            tempStr.Append("<TD WIDTH=""30"">&nbsp;</TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1"" WIDTH=""60""><P class=""boldFont"">User ID</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Rule</b>&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Name&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Login&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Company&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Action")

                            Dim tempLit As New Label
                            If cammWebManager.CurrentUserInfo.IsMember(6) Then
                                tempStr.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                                tempStr.Append("<a href=""apprights_cleanup.aspx?ID=" & NewAppID & "&AuthsAsAppID=" & CInt(Request.QueryString("AuthsAsAppID")) & """>Cleanup user authorization</a>")
                            End If
                            tempStr.Append("</P></TD></TR>")

                            For RowCounterI As Integer = 0 To MyDt.Rows.Count - 1
                                Dim RowSeparator As String = ""
                                If RowCounterI > 0 Then RowSeparator = " style=""border-top: solid 1px; border-color: lightgray;"""
                                tempStr.Append("<TR>")
                                tempStr.Append("<TD>&nbsp;</TD>")
                                tempStr.Append("<TD" & RowSeparator & "><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(RowCounterI)("ID_User"), 0).ToString) & IIf(Utils.Nz(MyDt.Rows(RowCounterI)("DevelopmentTeamMember"), False), "<b title=""Authorization for test and development purposes and for inactive security objects"">{Dev}</b>", "").ToString)
                                If Utils.Nz(MyDt.Rows(RowCounterI)("LoginDisabled"), False) = True Then tempStr.Append("<nobr title=""Disabled user account"">(D)</nobr>")
                                tempStr.Append("&nbsp;</P></TD>")
                                Dim RuleTitle As String
                                If MyDt.Columns.Contains("IsDenyRule") = True AndAlso Utils.Nz(MyDt.Rows(RowCounterI).Item("IsDenyRule"), False) = True Then
                                    RuleTitle = "DENY"
                                Else
                                    RuleTitle = "GRANT"
                                End If
                                tempStr.Append("<TD" & RowSeparator & "><P class=""normalFont"" title=""Authorization for this user is set up as " & Server.HtmlEncode(RuleTitle) & """>" & Server.HtmlEncode(RuleTitle) & "</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & " WIDTH=""170""><P class=""normalFont"">")

                                If Me.CurrentDbVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then
                                    tempStr.Append(Server.HtmlEncode(Utils.Nz(MyDt.Rows(RowCounterI)("Name1"), String.Empty)))
                                Else
                                    tempStr.Append(Server.HtmlEncode(Utils.Nz(MyDt.Rows(RowCounterI)("VorName"), String.Empty)) + " " + Server.HtmlEncode(Utils.Nz(MyDt.Rows(RowCounterI)("NachName"), String.Empty)))
                                End If

                                tempStr.Append("&nbsp;</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & " WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(RowCounterI)("LoginName"), String.Empty)) & "&nbsp;</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & " WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(RowCounterI)("CompanyName"), String.Empty)) & "&nbsp;</P></TD>")
                                tempStr.Append("<TD" & RowSeparator & "><P class=""normalFont"">")
                                If IsDBNull(MyDt.Rows(RowCounterI)("ThisAuthIsFromAppID")) Then
                                    If Not IsDBNull(MyDt.Rows(RowCounterI)("ID_AppRight")) Then tempStr.Append("<a href=""apprights_delete_users.aspx?ID=" & Utils.Nz(MyDt.Rows(RowCounterI)("ID_AppRight"), 0) & "&AuthsAsAppID=" & Utils.Nz(MyDt.Rows(RowCounterI)("AuthsAsAppID"), 0).ToString & """>Delete</a>")
                                Else
                                    tempStr.Append("<em>Inherited</em>")
                                End If

                                tempStr.Append("&nbsp;</P></TD>")
                                tempStr.Append("</TR>")

                                'Additional info on single-servergroup-rules
                                If MyDt.Columns.Contains("ID_ServerGroup") = True AndAlso CType(MyDt.Rows(RowCounterI).Item("ID_ServerGroup"), Integer) <> 0 Then
                                    tempStr.Append("<TR>")
                                    tempStr.Append("<TD COLSPAN=""3"">&nbsp;</TD>")
                                    tempStr.Append("<TD COLSPAN=""3""><P class=""normalFont"">")
                                    tempStr.Append("<em>Applies only to server group: " & ServerGroupTitle(CType(MyDt.Rows(RowCounterI).Item("ID_ServerGroup"), Integer)) & "</em>")
                                    tempStr.Append("&nbsp;</P></TD>")
                                    tempStr.Append("<TD COLSPAN=""1"">&nbsp;</TD>")
                                    tempStr.Append("</TR>")
                                End If
                            Next
                            tempStr.Append("</TABLE>")
                            CType(e.Item.FindControl("tdAddUserDetails"), HtmlTableCell).InnerHtml = tempStr.ToString
                        End If
                    Else
                        'CType(e.Item.FindControl("trAddUserGroupDetails"), HtmlTableRow).Style.Add("display", "none")
                        'CType(e.Item.FindControl("trAddUserDetails"), HtmlTableRow).Style.Add("display", "none")
                    End If
                End With
            End If
        End Sub

        Private Function ServerGroupTitle(serverGroupID As Integer) As String
            For MyCounter As Integer = 0 To AllServerGroupsInfo.Length - 1
                If AllServerGroupsInfo(MyCounter).ID = serverGroupID Then
                    Return """" & AllServerGroupsInfo(MyCounter).Title & """"
                End If
            Next
            Return "{Invalid reference " & serverGroupID & "}"
        End Function

        Private ReadOnly Property AllServerGroupsInfo As WMSystem.ServerGroupInformation()
            Get
                Static _BufferedResult As WMSystem.ServerGroupInformation()
                If _BufferedResult Is Nothing Then
                    _BufferedResult = Me.cammWebManager.System_GetServerGroupsInfo()
                End If
                Return _BufferedResult
            End Get
        End Property
#End Region

    End Class

End Namespace