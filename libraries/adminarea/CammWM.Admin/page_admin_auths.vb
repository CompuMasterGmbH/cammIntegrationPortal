Option Strict On
Option Explicit On 

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider
Imports CompuMaster.camm.WebManager.Administration
Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AuthorizationsOverview
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     The security objects overview page
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	27.08.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AuthorizationsOverview
        Inherits Page

        Protected Sub ActionExport(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender

            If Request.QueryString("action") = "export" Then
                If Request.QueryString("securityobjectid") <> Nothing Then
                    Dim SecurityObjectID As Integer = Utils.Nz(Request.QueryString("securityobjectid"), 0)
                    CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.AuthorizedGroupsAndPersons(cammWebManager, SecurityObjectID, True))
                ElseIf Request.QueryString("groupid") <> Nothing Then
                    Dim GroupID As Integer = Utils.Nz(Request.QueryString("groupid"), 0)
                    CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.AuthorizationsOfGroup(cammWebManager, GroupID))
                ElseIf Request.QueryString("userid") <> Nothing Then
                    Dim UserID As Long = Utils.Nz(Request.QueryString("userid"), 0)
                    CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.AuthorizationsOfUser(cammWebManager, UserID))
                End If
            End If

        End Sub

    End Class

    '''-----------------------------------------------------------------------------
    '''Project	 : camm WebManager
    '''Class	 : camm.WebManager.Pages.Administration.AppRightsList
    '''-----------------------------------------------------------------------------
    '''<summary>
    '''    A page to view list of application for authorization
    '''</summary>
    '''<remarks>
    '''</remarks>
    '''<history>
    '''	[I-link]	24.09.2007	Created
    '''</history>
    '''-----------------------------------------------------------------------------
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

            strQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
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
        Private Sub rptAppListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptAppList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                With dtApps.Rows(e.Item.ItemIndex)
                    NewAppID = Utils.Nz(.Item("ID_Application"), 0)

                    'If FirstAppLine Then FirstAppLine = False Else CType(e.Item.FindControl("trAddBlank"), HtmlTableRow).Style.Add("display", "")
                    If Not IsDBNull(.Item("AppDisabled")) Then If Utils.Nz(.Item("AppDisabled"), False) = True Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).Style.Add("display", "")
                    If Trim(Request.QueryString("Application")) = "" Then CType(e.Item.FindControl("ancAuthsAsAppID"), HtmlAnchor).HRef = "apprights.aspx?Application=" & Utils.Nz(.Item("ID_Application"), String.Empty) & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0) & ""

                    If IsDBNull(.Item("TitleAdminArea")) Then
                        CType(e.Item.FindControl("hypTitle"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Title"), String.Empty)) & " (" & Server.HtmlEncode(Utils.Nz(.Item("Abbreviation"), String.Empty)) & ")" & "<br>" & IIf(IsDBNull(.Item("ServerDescription")), "", Server.HtmlEncode(Utils.Nz(.Item("ServerDescription"), String.Empty))).ToString
                    ElseIf Utils.Nz(.Item("TitleAdminArea"), String.Empty) = "" Then
                        CType(e.Item.FindControl("hypTitle"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Title"), String.Empty)) & " (" & Server.HtmlEncode(Utils.Nz(.Item("Abbreviation"), String.Empty)) & ")" & "<br>" & IIf(IsDBNull(.Item("ServerDescription")), "", Server.HtmlEncode(.Item("ServerDescription").ToString)).ToString
                    Else
                        CType(e.Item.FindControl("hypTitle"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("TitleAdminArea"), String.Empty)) & " (" & Server.HtmlEncode(Utils.Nz(.Item("Abbreviation"), String.Empty)) & ")" & "<br>" & IIf(IsDBNull(.Item("ServerDescription")), "", Server.HtmlEncode(.Item("ServerDescription").ToString)).ToString
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
                        Dim sqlstr As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT count([RequiredUserProfileFlags]) FROM [dbo].[Applications_CurrentAndInactiveOnes] where id = @AppID"
                        Dim cmd As New SqlCommand(sqlstr, New SqlConnection(cammWebManager.ConnectionString))
                        cmd.Parameters.Add("@AppID", SqlDbType.Int).Value = .Item("ID_Application").ToString
                        If Val(ExecuteScalar(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)) > 0 Then
                            CType(e.Item.FindControl("ancBatchUserFlagEditor"), HyperLink).NavigateUrl = "users_batchuserflageditor.aspx?AppID=" & .Item("ID_Application").ToString
                            CType(e.Item.FindControl("ancBatchUserFlagEditor"), HyperLink).Visible = True
                        End If
                    End If

                    If Trim(Request.QueryString("Showall")) <> "" OrElse Trim(Request.QueryString("Application")) <> "" OrElse CurUserIsSecurityOperator Then
                        If Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Applications, AuthorizationTypeEffective.UpdateRelations, CType(.Item("ID_Application"), Integer)) Then
                            CType(e.Item.FindControl("ancAddGroupShowDetails"), HtmlAnchor).HRef = "apprights_new_groups.aspx?ID=" & Utils.Nz(.Item("ID_Application"), 0).ToString & ""
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
                        dt = New DataTable
                        tempStr = New Text.StringBuilder

                        Dim sqlParams As SqlParameter() = {New SqlParameter("@AppID", NewAppID)}
                        strQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select LoginDisabled,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_Group,DevelopmentTeamMember,Name,Vorname,Description from [view_ApplicationRights] where id_application=@AppID and isnull(ID_Group,0)<>0 order by Name"
                        dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                            'strDeleteLink = String.Empty
                            'CType(e.Item.FindControl("rptShowGroups"), Repeater).DataSource = MyDt
                            'CType(e.Item.FindControl("rptShowGroups"), Repeater).DataBind()

                            tempStr.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"">")
                            tempStr.Append("<TR>")
                            tempStr.Append("<TD WIDTH=""30px""> &nbsp;</TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1"" WIDTH=""60""><P class=""boldFont"">Group ID</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Name</b>&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Description</b>&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Action&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                            If Trim(Request.QueryString("Application") & "") <> "" Then
                                If Utils.Nz(Request.QueryString("ShowAllUsers"), 0) = 1 Then tempStr.Append("<a href=""apprights.aspx?&Application=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(dt.Rows(0)("AuthsAsAppID"), 0).ToString & """>Hide all users</a>") Else tempStr.Append("<a href=""apprights.aspx?&ShowAllUsers=1&Application=" & Utils.Nz(.Item("ID_Application"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(dt.Rows(0)("AuthsAsAppID"), 0).ToString & """>Show all users</a>")
                            End If
                            tempStr.Append("</P></TD></TR>")

                            For j As Integer = 0 To dt.Rows.Count - 1
                                tempStr.Append("<TR>")
                                tempStr.Append("<TD>&nbsp;</TD>")
                                tempStr.Append("<TD><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(dt.Rows(j)("ID_Group"), 0).ToString) & IIf(Utils.Nz(dt.Rows(j)("DevelopmentTeamMember"), False), "<b>{Dev}</b>", "").ToString & "&nbsp;</P></TD>")
                                tempStr.Append("<TD WIDTH=""170""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(dt.Rows(j)("Name"), String.Empty)) & "</P></TD>")
                                tempStr.Append("<TD WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(dt.Rows(j)("Description"), String.Empty)) & "&nbsp;</P></TD>")
                                tempStr.Append("<TD><P class=""normalFont"">")
                                If IsDBNull(dt.Rows(j)("ThisAuthIsFromAppID")) Then
                                    If Not IsDBNull(dt.Rows(j)("ID_AppRight")) Then tempStr.Append("<a href=""apprights_delete_groups.aspx?ID=" & Utils.Nz(dt.Rows(j)("ID_AppRight"), 0).ToString & "&AuthsAsAppID=" & Utils.Nz(dt.Rows(j)("AuthsAsAppID"), 0).ToString & """>Delete</a>")
                                Else
                                    tempStr.Append("<em>Inherited</em>")
                                End If
                                tempStr.Append("&nbsp;</P></TD>")
                                tempStr.Append("</TR>")

                                If Not Request.QueryString("ShowAllUsers") Is Nothing AndAlso Utils.Nz(Request.QueryString("ShowAllUsers"), 0) = 1 Then
                                    MyDt = New DataTable
                                    strQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                                "SELECT * FROM [view_Memberships] WHERE ID_Group = " & Utils.Nz(dt.Rows(j)("ID_Group"), 0).ToString & " And ID_Group not in (Select id_group_public from system_servergroups) and ID_Group not in (Select id_group_anonymous from system_servergroups) and (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments Where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Applications' AND AuthorizationType In ('SecurityMaster','ViewAllRelations')) OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Applications' AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner'))) ORDER BY Nachname, Name, ID_Group, Vorname"
                                    MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                    If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                                        'CType(e.Item.FindControl("rptShowGroupUsers"), Repeater).DataSource = MyDt
                                        'CType(e.Item.FindControl("rptShowGroupUsers"), Repeater).DataBind()
                                        tempStr.Append("<TR><TD>&nbsp;</TD><TD colspan=""4""><TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"">")
                                        tempStr.Append("<TR><TD WIDTH=""50"">&nbsp;</TD><TD class=""boldFontHeader"" WIDTH=""60""><P class=""boldFont"">User ID</P></TD><TD class=""boldFontHeader""><P class=""boldFont"">Name&nbsp;</P></TD><TD class=""boldFontHeader""><P class=""boldFont"">Login&nbsp;</P></TD><TD class=""boldFontHeader""><P class=""boldFont"">Company&nbsp;</P></TD></TR>")

                                        For i As Integer = 0 To MyDt.Rows.Count - 1
                                            tempStr.Append("<TR><TD>&nbsp;</TD><TD><P class=""normalFont"">" & Utils.Nz(MyDt.Rows(i)("ID_User"), 0) & "&nbsp;</P></TD><TD WIDTH=""170""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Nachname"), String.Empty)) & ", " & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Vorname"), String.Empty)) & "&nbsp;</P></TD><TD WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("LoginName"), String.Empty)) & "&nbsp;</P></TD><TD><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Company"), String.Empty)) & "&nbsp;</P></TD></TR>")
                                        Next

                                        tempStr.Append("</TABLE></TD></TR>")
                                    Else
                                        tempStr.Append("<TR><TD WIDTH=""50"">&nbsp;</TD><TD VAlign=""Top"" colspan=""4""><P><FONT face=""Arial"" size=""1"">No users are member of this group</FONT></P></TD></TR>")
                                    End If
                                End If
                            Next

                            tempStr.Append("</TABLE>")
                            CType(e.Item.FindControl("tdAddUserGroupDetails"), HtmlTableCell).InnerHtml = tempStr.ToString
                        End If

                        MyDt = New DataTable
                        tempStr = New Text.StringBuilder
                        Dim sqlParams1 As SqlParameter() = {New SqlParameter("@AppID", NewAppID)}
                        strQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select name1,LoginDisabled,DevelopmentTeamMember,AuthsAsAppID,ThisAuthIsFromAppID,ID_AppRight,ID_User,Nachname,Vorname,LoginName from [view_ApplicationRights] where id_application=@AppID and isnull(ID_User,0)<>0 order by Nachname"
                        MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                        If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                            'strDeleteUserLink = String.Empty
                            'Server.HtmlEncode(Utils.Nz(.Item("ID_User"), 0).ToString) & IIf(Utils.Nz(.Item("DevelopmentTeamMember"), False), "<b>{Dev}</b>", "").ToString
                            'If IsDBNull(.Item("ThisAuthIsFromAppID")) Then
                            '    If Not IsDBNull(.Item("ID_AppRight")) Then strDeleteUserLink = ("<a href=""apprights_delete_users.aspx?ID=" & Utils.Nz(.Item("ID_AppRight"), 0) & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0).ToString & """>Delete</a>")
                            'Else
                            '    strDeleteUserLink = ("<em>Inherited</em>")
                            'End If

                            'CType(e.Item.FindControl("rptShowUsers"), Repeater).DataSource = MyDt
                            'CType(e.Item.FindControl("rptShowUsers"), Repeater).DataBind()

                            tempStr.Append("<TABLE WIDTH=""100%"" CELLSPACING=""0"" CELLPADDING=""3"" border=""0"" bordercolor=""#FFFFFF"">")
                            tempStr.Append("<TR>")
                            tempStr.Append("<TD WIDTH=""30"">&nbsp;</TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1"" WIDTH=""60""><P class=""boldFont"">User ID</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Name&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Login&nbsp;</P></TD>")
                            tempStr.Append("<TD BGCOLOR=""#E1E1E1""><P class=""boldFont"">Action")

                            Dim tempLit As New Label
                            If cammWebManager.CurrentUserInfo.IsMember(6) Then
                                tempStr.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                                tempStr.Append("<a href=""apprights_cleanup.aspx?ID=" & NewAppID & "&AuthsAsAppID=" & CInt(Request.QueryString("AuthsAsAppID")) & """>Cleanup user authorization</a>")
                            End If
                            tempStr.Append("</P></TD></TR>")

                            For i As Integer = 0 To MyDt.Rows.Count - 1
                                tempStr.Append("<TR>")
                                tempStr.Append("<TD>&nbsp;</TD>")
                                tempStr.Append("<TD><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("ID_User"), 0).ToString) & IIf(Utils.Nz(MyDt.Rows(i)("DevelopmentTeamMember"), False), "<b>{Dev}</b>", "").ToString)
                                If Not IsDBNull(MyDt.Rows(i)("LoginDisabled")) Then If Utils.Nz(MyDt.Rows(i)("LoginDisabled"), True) = True Then tempStr.Append("<span title="""" Disabled"""">(D)</span>&nbsp;</P></TD>")
                                tempStr.Append("<TD WIDTH=""170""><P class=""normalFont"">")

                                Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex
                                If MyDBVersion.Build >= 147 Then tempStr.Append(Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("Name1"), String.Empty))) Else tempStr.Append(Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("VorName"), String.Empty)) + " " + Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("NachName"), String.Empty)))

                                tempStr.Append("&nbsp;</P></TD>")
                                tempStr.Append("<TD WIDTH=""200""><P class=""normalFont"">" & Server.HtmlEncode(Utils.Nz(MyDt.Rows(i)("LoginName"), String.Empty)) & "&nbsp;</P></TD>")
                                tempStr.Append("<TD><P class=""normalFont"">")
                                If IsDBNull(MyDt.Rows(i)("ThisAuthIsFromAppID")) Then
                                    If Not IsDBNull(MyDt.Rows(i)("ID_AppRight")) Then tempStr.Append("<a href=""apprights_delete_users.aspx?ID=" & Utils.Nz(MyDt.Rows(i)("ID_AppRight"), 0) & "&AuthsAsAppID=" & Utils.Nz(MyDt.Rows(i)("AuthsAsAppID"), 0).ToString & """>Delete</a>")
                                Else
                                    tempStr.Append("<em>Inherited</em>")
                                End If

                                tempStr.Append("&nbsp;</P></TD>")
                                tempStr.Append("</TR>")
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
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AppRightsNewUsers
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to add any user to a perticular application
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[zeutzheim]	24.02.2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
                sb.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT " & TopClause & " System_AccessLevels.Title As AccessLevel_Title, Benutzer.*, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS UserNameComplete FROM [Benutzer] LEFT JOIN System_AccessLevels ON Benutzer.AccountAccessability = System_AccessLevels.ID" & vbNewLine)
                sb.Append(WhereClause & " ORDER BY Nachname, Vorname" & vbNewLine)

                Dim SqlQuery As String = sb.ToString

                Dim cmd As New SqlClient.SqlCommand(SqlQuery, New SqlConnection(cammWebManager.ConnectionString))
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' List all available applications and bind them on the DropDownList. PreSelect application where user comes from.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	25.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ListApps()
            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
            Dim WhereClause As String = "WHERE (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('UpdateRelations','Owner')))) "
            Dim sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT Applications.ID, Applications.Title, Applications.AppDisabled, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID " & WhereClause & " ORDER BY Title"

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

        Private Sub searchUserBtnClick(ByVal sender As Object, ByVal e As EventArgs) Handles searchUserBtn.Click
            ListOfUsers(CInt(drp_App.SelectedValue))
        End Sub

        Private Sub top50ChkCheckChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CheckBoxTop50Results.CheckedChanged
            ListOfUsers(CInt(drp_App.SelectedValue))
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
        ''' 	[zeutzheim]	25.02.2010	Created
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
                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Company"), String.Empty))
                End With
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Load new user data if selected application changes
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	25.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub drpAppsSelectedAppChanged(ByVal sender As Object, ByVal e As EventArgs) Handles drp_App.SelectedIndexChanged
            ListOfUsers(CInt(CType(sender, DropDownList).SelectedValue))
        End Sub

        Public Function IsUserAlreadyAuthorized(ByVal userId As Integer, ByVal applicationID As Integer) As Boolean
            Dim commandText As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "SELECT count([ID]) FROM [dbo].[ApplicationsRightsByUser] WHERE [ID_GroupOrPerson] = @UserID  AND [ID_Application] =  @AppID"
            Dim commandParams As SqlParameter() = {New SqlParameter("@UserID", userId), New SqlParameter("@AppID", applicationID)}
            Dim RecordCount As Object = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            Return CInt(RecordCount) > 0
        End Function

        Public Function GetRequiredUserFlagsForApplication(ByVal applicationID As Integer) As String()
            Dim commandParams As SqlParameter() = {New SqlParameter("@AppID", applicationID)}
            Dim commandText As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select RequiredUserProfileFlags  From Applications_CurrentAndInactiveOnes Where ID = @AppID" '& dropAppID
            Dim RequiredUserFlags As String = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToString()

            If RequiredUserFlags = String.Empty Then
                Return New String() {}
            End If

            Return RequiredUserFlags.Split(CChar(","))

        End Function

        Private Sub btnOKClick(ByVal sender As Object, ByVal args As EventArgs) Handles btnOK.Click
            Dim authorize As Boolean = True

            Dim dropAppID As Integer = CInt(drp_App.SelectedValue)
            Dim dropAppText As String = drp_App.SelectedItem.Text

            Dim arrRequiredUserFlags() As String = GetRequiredUserFlagsForApplication(dropAppID)
            Dim flagsUpdateLinks As String = String.Empty

            For Each MyListItem As UI.Control In rptUserList.Controls
                For Each MyControl As UI.Control In MyListItem.Controls
                    If MyControl.GetType Is GetType(System.Web.UI.WebControls.CheckBox) AndAlso (MyControl.ID = "chk_user" Or MyControl.ID = "chk_devteam") Then
                        If CType(MyListItem.FindControl("chk_user"), CheckBox).Checked AndAlso CType(MyListItem.FindControl("chk_devteam"), CheckBox).Checked Then
                            If MyControl.ID = "chk_user" Then
                                authorize = False
                            End If
                        End If

                        If authorize Then

                            Dim chk As CheckBox = CType(MyControl, CheckBox)
                            If chk.Checked Then
                                Dim dropUserText As String = CType(MyListItem.FindControl("lblLoginName"), UI.HtmlControls.HtmlAnchor).InnerText
                                Dim dropUserID As Integer = CInt(CType(MyListItem.FindControl("lblID"), Label).Text)

                                Dim userInfo As WMSystem.UserInformation = cammWebManager.System_GetUserInfo(CType(dropUserID, Long))

                                If IsUserAlreadyAuthorized(dropUserID, dropAppID) Then
                                    lblErr.Text &= "User " + dropUserText.ToString() + " is already authorized for application " + dropAppText.Trim & "<br />"
                                    authorize = False
                                ElseIf Not ((cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                                    Response.Write("No authorization to administrate this application.")
                                    Response.End()
                                ElseIf dropAppText.ToString.Trim() <> "" And dropUserText.ToString.Trim() <> "" Then
                                    Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex

                                    If MyDBVersion.Build >= 147 Then 'Newer
                                        If Not userInfo Is Nothing Then
                                            Dim validationResults As FlagValidation.FlagValidationResult() = FlagValidation.GetFlagValidationResults(userInfo, arrRequiredUserFlags)
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
                                            userInfo.AddAuthorization(dropAppID, CType(Nothing, Integer), CType(MyListItem.FindControl("chk_devteam"), CheckBox).Checked, Nothing)

                                            lblMsg.Text &= dropUserText + " has been authorized for application " + dropAppText & "<br />"
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
                    authorize = True
                Next
            Next

            If flagsUpdateLinks <> String.Empty Then
                lblErr.Text = "Couldn't update the authorization due to problems with the following additional flags:<br />" & flagsUpdateLinks
                lblErr.Text &= "<br>Please make sure the users have the required additional flags and that the value is correct for the specific type of the flag."
            End If



        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AppRightsNewGroups
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to add any group to a perticular application
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[zeutzheim]	15.03.2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AppRightsNewGroups
        Inherits Page

#Region "Variable Declaration"
        Dim CurUserID As Long
        Dim ErrMSg As String
        Protected lblErr As Label
        Protected lblMsg As Label
        Protected WithEvents btnOK As Button
        Protected WithEvents drp_apps, drp_groups As DropDownList
#End Region

#Region "Page Events"
        Private Sub AppRightsNewGroups_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErr.Text = ""
            lblMsg.Text = ""
            If Not IsPostBack Then
                ListApps()
                LoadGroups()
            End If
        End Sub

#End Region

#Region "User-Defined Methods"
        Private Sub LoadGroups()
            Dim GrpTable As New DataTable

            If HttpContext.Current.Cache("CammWM.Admin.Table.AllGroups") Is Nothing Then
                GrpTable = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM Gruppen ORDER BY Name", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                HttpContext.Current.Cache.Insert("CammWM.Admin.Table.AllGroups", GrpTable, Nothing, Now.AddMinutes(20), Caching.Cache.NoSlidingExpiration)
            Else
                GrpTable = CType(HttpContext.Current.Cache("CammWM.Admin.Table.AllGroups"), DataTable)
            End If

            Me.drp_groups.DataSource = GrpTable
            Me.drp_groups.DataTextField = "Name"
            Me.drp_groups.DataValueField = "ID"
            Me.drp_groups.DataBind()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' List all available applications and bind them on the DropDownList. PreSelect application where user comes from.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	25.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ListApps()
            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
            Dim WhereClause As String = "WHERE (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('UpdateRelations','Owner')))) "
            Dim sql As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT Applications.ID, Applications.Title, Applications.AppDisabled, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID " & WhereClause & " ORDER BY Title"
            Dim AppTable As DataTable = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sql, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            For Each row As DataRow In AppTable.Rows
                row("Title") = Utils.Nz(row("Title"), String.Empty) & " (ID " & Utils.Nz(row("ID"), String.Empty) & Utils.Nz(IIf(Utils.Nz(row("AppDisabled"), False) = True, " (D)", ""), String.Empty) & " / " & Utils.Nz(row("ServerDescription"), String.Empty) & " / " & Utils.Nz(row("Description"), String.Empty) & ")"
            Next

            Me.drp_apps.DataSource = AppTable
            Me.drp_apps.DataTextField = "Title"
            Me.drp_apps.DataValueField = "ID"
            Me.drp_apps.DataBind()

            If Not Request.QueryString("ID") = "" Then
                Me.drp_apps.SelectedValue = Request.QueryString("ID")
            End If

        End Sub
#End Region


        ''' <summary>
        ''' Checks whether the given group is authorization for the application
        ''' </summary>
        ''' <param name="groupId"></param>
        ''' <param name="applicationId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function IsGroupAuthorizedForApplication(ByVal groupId As Integer, ByVal applicationId As Integer) As Boolean
            Dim commandText As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT count([ID]) FROM [dbo].[ApplicationsRightsByGroup] WHERE [ID_GroupOrPerson] = @GroupId AND [ID_Application] =  @AppID"
            Dim commandParameters As SqlParameter() = {New SqlParameter("@GroupId", groupId), New SqlParameter("@AppID", applicationId)}
            Dim RecordCount As Integer = CType(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParameters, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Return RecordCount > 0
        End Function

        Public Function GetRequiredUserFlagsForApplication(ByVal applicationID As Integer) As String()
            Dim commandParams As SqlParameter() = {New SqlParameter("@AppID", applicationID)}
            Dim commandText As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select RequiredUserProfileFlags  From Applications_CurrentAndInactiveOnes Where ID = @AppID" '& dropAppID
            Dim RequiredUserFlags As String = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToString().Trim()

            If RequiredUserFlags = String.Empty Then
                Return New String() {}
            End If

            Return RequiredUserFlags.Split(CChar(","))

        End Function


#Region "Control Events"

        Private Sub btnOKClick(ByVal sender As Object, ByVal args As EventArgs) Handles btnOK.Click
            Dim GroupTable As New DataTable
            Dim AppTable As New DataTable
            Dim dropGroupText, dropAppText As String
            Dim dropGroupID, dropAppID As Integer

            dropAppText = Utils.Nz(drp_apps.SelectedItem.Text, String.Empty)
            dropAppID = CInt(Utils.Nz(drp_apps.SelectedValue, String.Empty))


            dropGroupText = Utils.Nz(drp_groups.SelectedItem.Text, String.Empty)
            dropGroupID = CInt(Utils.Nz(drp_groups.SelectedValue, String.Empty))

            If IsGroupAuthorizedForApplication(dropGroupID, dropAppID) Then
                lblErr.Text = "Group " + dropGroupText.ToString.Trim + " is already authorized for application " + dropAppText.Trim
            ElseIf Not ((cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                Response.Write("No authorization to administrate this application.")
                Response.End()
            ElseIf dropAppText.Trim <> "" And dropGroupText.ToString.Trim <> "" Then
                CurUserID = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)

                Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex
                If MyDBVersion.Build >= 147 Then

                    Dim requiredFlags() As String = GetRequiredUserFlagsForApplication(dropAppID)

                    Dim commandText As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT A.ID_USER [ID], (Select   ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname FROM Benutzer Where ID = A.ID_USER) AS [Name] From Memberships A Where ID_Group =  @GroupID" '+ CStr(id_grouporperson.SelectedItem.Id)
                    Dim commandParameter As SqlParameter() = {New SqlParameter("@GroupID", dropGroupID)}
                    Dim DTUser As DataTable = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParameter, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    Dim flagsUpdateLinks As String = String.Empty
                    If Not DTUser Is Nothing AndAlso DTUser.Rows.Count > 0 Then
                        For Each dRowUser As DataRow In DTUser.Rows
                            Dim userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = cammWebManager.System_GetUserInfo(CType(dRowUser("ID"), Long))
                            If Not userInfo Is Nothing Then
                                Dim validationResults As FlagValidation.FlagValidationResult() = FlagValidation.GetFlagValidationResults(userInfo, requiredFlags)
                                flagsUpdateLinks &= CompuMaster.camm.WebManager.Administration.Utils.FormatLinksToFlagUpdatePages(validationResults, userInfo)
                            End If
                        Next
                    End If

                    If flagsUpdateLinks.Length > 0 Then
                        lblErr.Text = "Required flags are not set or type of value is not correct:<br>" + flagsUpdateLinks + "<br>Failed to create authorization for group!"
                        Exit Sub
                    End If

                End If

                Try
                    'Use implemented workflow of camm WebManager to send Authorization-Mail if user is authorized the first time
                    'HINT: SapFlag checks will be checked continually, but no information on missing flag name is given here any more
                    Dim grpInfo As New WebManager.WMSystem.GroupInformation(dropGroupID, cammWebManager)
                    grpInfo.AddAuthorization(dropAppID, CType(Nothing, Integer))
                    lblErr.Text = ""
                    lblMsg.Text = dropGroupText.Trim + " has been authorized for application " + dropAppText
                Catch ex As Exception
                    cammWebManager.Log.ReportErrorViaEMail(ex, "cammWebManager: Group authorization creation failed!")
                    ErrMSg = "Group authorization creation failed! (" & ex.Message & ")"
                End Try

            ElseIf dropAppText.Trim = "" And dropGroupText.ToString.Trim = "" Then
                'do nothing
            Else
                ErrMSg = "Please specify an application and a group to proceed!"
            End If

            If ErrMSg <> "" Then lblErr.Text = Utils.Nz(ErrMSg, String.Empty)
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AppRightsDeleteGroups
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete any group from any application
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	21.09.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
                Dim MySQLString As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_ApplicationRights WHERE ID_Group Is Not Null And ThisAuthIsFromAppID Is Null And ID_AppRight=@ID"
                Dim MyDt As New DataTable

                mydt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), MySQLString, CommandType.Text, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

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
                mydt.Dispose()
            End If

            lblField_ID.Text = Server.HtmlEncode(Utils.Nz(Field_ID, 0).ToString)
            lblField_AppTitle.Text = Server.HtmlEncode(Utils.Nz(Field_AppTitle, String.Empty))

            If True Then
                Dim Field_Groupname, Field_Description As String
                Dim MyDt As New DataTable
                Field_Groupname = ""
                Field_Description = ""

                Dim sqlParams2 As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Field_GroupID & "")))}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
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

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AppRightsDeleteGroups
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete any group from any application
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	21.09.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
                Dim MySQLString As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_ApplicationRights WHERE ID_User Is Not Null And ThisAuthIsFromAppID Is Null And ID_AppRight=@AuthID"
                Dim MyDt As New DataTable

                mydt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), MySQLString, CommandType.Text, sqlparams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

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
                mydt.Dispose()
            End If

            lblField_ID.Text = Server.HtmlEncode(Utils.Nz(Field_ID, 0).ToString)
            lblField_AppTitle.Text = Server.HtmlEncode(Utils.Nz(Field_AppTitle, String.Empty))

            If True Then
                Dim Field_loginname As String = ""
                Dim Field_username As String = ""
                If Field_UserID = Nothing Then Field_UserID = Utils.Nz(Request.QueryString("ID"), 0)
                Dim MyDt As New DataTable

                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", Field_UserID)}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_UserList WHERE ID = @ID", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not mydt Is Nothing AndAlso mydt.Rows.Count > 0 Then
                    With mydt.Rows(0)
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
                mydt.Dispose()

            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.Administration.Apprights_New_Transmession
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     The Apprights_New_Transmession page 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	16.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Apprights_New_Transmession
        Inherits Page

#Region "Variable Declaration"
        Dim CurUserID As Long
        Dim ErrMsg As String
        Dim dt As New DataTable
        Protected lblErrMsg As Label
        Protected cmbApplicationID, cmbInheritsApplicationID As DropDownList
        Protected WithEvents btnCreateTransmission As Button
#End Region

#Region "Page Events"
        Private Sub Apprights_New_Transmession_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not IsPostBack Then FillDropDownList()
        End Sub
#End Region

#Region "User-Defined Methods"
        Sub FillDropDownList()
            Dim MyCount As Integer
            'Fill DropDwonList cmbApplicationID
            If cmbApplicationID.SelectedValue = "" Then
                cmbApplicationID.Items.Add("Please select!")
                cmbApplicationID.SelectedIndex = 0
            End If

            dt = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT Applications.*, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID ORDER BY Title", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            Dim MyAppID As Integer
            If cmbApplicationID.SelectedValue <> "" AndAlso cmbApplicationID.SelectedValue <> "Please select!" Then
                MyAppID = cmbApplicationID.SelectedIndex
            Else
                MyAppID = Utils.Nz(Request.QueryString("ID"), 0)
            End If
            Dim temp As String
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    temp = dt.Columns.Item(MyCount).ToString
                    cmbApplicationID.Items.Add(New ListItem(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")", Utils.Nz(dr("ID"), 0).ToString))
                    cmbApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("Title", Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")")
                    cmbApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("onmouseover", "this.title=this.options[this.selectedIndex].title")
                    If MyAppID = Utils.Nz(dr("ID"), 0) Then
                        cmbApplicationID.SelectedValue = Utils.Nz(dr("ID"), 0).ToString
                    End If
                Next
            End If

            'Fill DropDwonList cmbInheritsApplicationID
            If cmbInheritsApplicationID.SelectedValue = "" Then
                cmbInheritsApplicationID.Items.Add("Please Select!")
                cmbInheritsApplicationID.SelectedIndex = 0
            End If

            Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", MyAppID)}
            dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT Applications.*, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID WHERE Applications.ID <> @ID ORDER BY Title", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    temp = dt.Columns.Item(MyCount).ToString
                    cmbInheritsApplicationID.Items.Add(New ListItem(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")", Utils.Nz(dr("ID"), 0).ToString))
                    cmbInheritsApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("Title", Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")")
                    cmbInheritsApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("onmouseover", "this.title=this.options[this.selectedIndex].title")
                    If (Trim(cmbInheritsApplicationID.SelectedValue) = Trim(Utils.Nz(dr("ID"), 0).ToString)) Then
                        cmbInheritsApplicationID.SelectedValue = Utils.Nz(dr("ID"), 0).ToString
                    End If
                Next
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnCreateTransmission_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateTransmission.Click
            If Not ((cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                ErrMsg = "No authorization to administrate this application."
            ElseIf cmbApplicationID.SelectedValue <> String.Empty And cmbInheritsApplicationID.SelectedValue <> String.Empty Then
                Dim SqlParams As SqlParameter() = {New SqlParameter("@ReleasedByUserID", CurUserID), New SqlParameter("@IDApp", cmbApplicationID.SelectedValue), New SqlParameter("@InheritsFrom", cmbInheritsApplicationID.SelectedValue)}
                Dim Result As Object
                'dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetAuthorizationInherition", CommandType.StoredProcedure, SqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)

                Try
                    Result = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetAuthorizationInherition", CommandType.StoredProcedure, SqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    If Result Is Nothing OrElse IsDBNull(Result) Then
                        ErrMsg = "Undefined error detected!"
                    ElseIf CInt(Result) = -1 Then
                        Dim temp As String
                        temp = cmbApplicationID.SelectedValue
                        Response.Redirect("apprights.aspx?Application=" & cmbApplicationID.SelectedValue & "&AuthsAsAppID=" & cmbInheritsApplicationID.SelectedValue)
                    Else
                        ErrMsg = "Transmission creation failed!"
                    End If
                Catch ex As Exception
                    ErrMsg = "Authorization creation failed! (" & ex.Message & ")"
                End Try

            ElseIf cmbApplicationID.SelectedValue = "" And cmbInheritsApplicationID.SelectedValue = "" Then
            Else
                ErrMsg = "Transmission details:"
            End If

            If ErrMsg <> "" Then
                lblErrMsg.Text = ErrMsg
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AppRightsDeleteTransmission
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete AppRights Transmission
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>  
    ''' 	[I-link]	16.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AppRightsDeleteTransmission
        Inherits Page

#Region "Variable Declaration"
        Protected gcAddHtml As HtmlGenericControl
        Protected lblErrMsg As Label
        Protected hypNoDelete, hypDelete As HyperLink
#End Region

#Region "Page Events"
        Private Sub AppRightsDeleteTransmission_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim CurUserID As Long
            If CLng(Request.QueryString("ID")) <> 0 AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                CurUserID = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                Dim sqlParams As SqlParameter() = {
                                                    New SqlParameter("@ReleasedByUserID", CurUserID), _
                                                    New SqlParameter("@IDApp", CLng(Request.QueryString("ID"))), _
                                                    New SqlParameter("@InheritsFrom", DBNull.Value)
                                                }
                Try
                    Dim ResulT As Object
                    ResulT = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetAuthorizationInherition", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    If ResulT Is Nothing OrElse IsDBNull(ResulT) Then
                        lblErrMsg.Text = "Undefined error detected!"
                    ElseIf CInt(ResulT) = -1 Then
                        Response.Redirect("apprights.aspx?Application=" & CInt(Request.QueryString("ID")))
                    Else
                        lblErrMsg.Text = "Erasing of transmission failed!"
                    End If
                Catch
                    lblErrMsg.Text = "Erasing of transmission failed!"
                End Try
            Else
                Dim DisplAppID As Integer
                Dim LanguageDescription As String
                Dim ServerDescription As String
                Dim strBilder As New Text.StringBuilder
                Dim dtResult As DataTable
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                dtResult = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "SELECT Applications.*, Languages.Description FROM Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID WHERE Applications.ID = @ID ORDER BY Applications.Title", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                Dim iCount As Integer = 0
                While iCount < dtResult.Rows.Count
                    If Utils.Nz(Request.QueryString("ID"), 0L) = Utils.Nz(dtResult.Rows(iCount)("ID"), 0L) Then
                        strBilder.Append("ID " & CLng(Request.QueryString("ID")) & ", " & Server.HtmlEncode(Utils.Nz(dtResult.Rows(iCount)("Title"), String.Empty)))
                        DisplAppID = Utils.Nz(dtResult.Rows(iCount)("ID"), 0)
                        LanguageDescription = CompuMaster.camm.WebManager.Utils.Nz(dtResult.Rows(iCount)("Description"), String.Empty)
                        ServerDescription = Utils.Nz(New camm.WebManager.WMSystem.ServerInformation(CInt(Val(dtResult.Rows(iCount)("LocationID"))), cammWebManager).Description, String.Empty)
                        strBilder.Append(" (" & Server.HtmlEncode(ServerDescription) & " / " & Server.HtmlEncode(LanguageDescription))
                        strBilder.Append(")<input type=""hidden"" name=""app_id"" value=""")
                        strBilder.Append(DisplAppID & """")
                    Else
                        strBilder.Append("<font color=""red"">Application not found!</font>")
                    End If
                    iCount += 1
                End While
                gcAddHtml.InnerHtml = strBilder.ToString
                hypDelete.NavigateUrl = "apprights_delete_transmission.aspx?ID=" + Request.QueryString("ID").ToString + "&DEL=NOW&APPID=" + Request.QueryString("ID").ToString & "&token=" & Session.SessionID
                hypDelete.Text = "Yes, delete it!"
                hypNoDelete.NavigateUrl = "apprights.aspx?Application=" + Request.QueryString("ID").ToString
                hypNoDelete.Text = "No! Don't touch it!"
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWM.Admin
    ''' Class	 : camm.WebManager.Pages.Administration.CleanUpUserAuthorization
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Removes authorization of single users that are already authorized by a group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[zeutzheim]	16.02.2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CleanUpUserAuthorization
        Inherits Page

#Region " Variables "
        Protected pnlAuthCleanUp As Panel
        Protected InfoLbl, lblErr As Label
        Protected WithEvents cleanUpBtn As Button
#End Region

#Region " Page Events "

        Private ReadOnly Property SqlQuery() As String
            Get
                Dim sb As New System.Text.StringBuilder
                sb.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine)
                sb.Append("SELECT UserAuths.UserID, UserAuths.AuthID" & vbNewLine)
                sb.Append("FROM" & vbNewLine)
                sb.Append("      (" & vbNewLine)
                sb.Append("            SELECT ID_GroupOrPerson AS UserID, ID AS AuthID" & vbNewLine)
                sb.Append("            FROM dbo.ApplicationsRightsByUser" & vbNewLine)
                sb.Append("            WHERE ID_Application = @AppID AND DevelopmentTeamMember <> 'True'" & vbNewLine)
                sb.Append("      ) AS UserAuths" & vbNewLine)
                sb.Append("      INNER JOIN" & vbNewLine)
                sb.Append("      (" & vbNewLine)
                sb.Append("            SELECT dbo.Memberships.ID_User AS UserID" & vbNewLine)
                sb.Append("            FROM dbo.ApplicationsRightsByGroup" & vbNewLine)
                sb.Append("                  INNER JOIN dbo.Memberships ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Memberships.ID_Group" & vbNewLine)
                sb.Append("            WHERE ID_Application = @AppID" & vbNewLine)
                sb.Append("      ) AS GroupAuths " & vbNewLine)
                sb.Append("      ON UserAuths.UserID = GroupAuths.UserID" & vbNewLine)
                Return sb.ToString
            End Get
        End Property

        Private Sub OnPageInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            If Not Page.IsPostBack Then
                'List users
                Dim sqlParams1 As SqlParameter() = {New SqlParameter("@AppID", CInt(Request.QueryString("ID")))}
                Dim userAl As ArrayList = ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlConnection(cammWebManager.ConnectionString), SqlQuery, CommandType.Text, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                Dim sb As New Text.StringBuilder
                If userAl.Count <= 0 Then
                    sb.Append("No users are already authorized by group")
                    cleanUpBtn.Enabled = False
                Else
                    sb.Append("The following users are duplicate authorized users:<br /><br/>")
                    sb.Append("<lu>")

                    For mycounter As Integer = 0 To userAl.Count - 1
                        userAl(mycounter) = CLng(userAl(mycounter))
                    Next

                    Dim Users As CompuMaster.camm.WebManager.WMSystem.UserInformation() = cammWebManager.System_GetUserInfos(CType(userAl.ToArray(GetType(Long)), Long()))


                    For Each UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation In Users
                        sb.Append("<li>")
                        sb.Append(UserInfo.IDLong)
                        sb.Append(" - ")
                        sb.Append(UserInfo.FullName)
                        sb.Append("</li>")
                    Next


                    sb.Append("</lu>")
                    sb.Append("<br /><br />Do you realy want to cleanup the user authorizations? Group authorizations will NOT be modified!<br /><br />")
                End If
                InfoLbl.Text = sb.ToString
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Check whether user is supervisor
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	16.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub OnPageLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Not cammWebManager.CurrentUserInfo.IsMember(6) Then
                Response.Write("Access denied.")
                Response.End()
                Exit Sub
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Cleanup authorized users that are already authorized by group
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	16.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CleanUpAuthorization(ByVal sender As Object, ByVal e As EventArgs) Handles cleanUpBtn.Click
            Dim NewAppID As Integer = CInt(Request.QueryString("ID"))

            Dim sb As New System.Text.StringBuilder
            sb.Append("-- Declare the variables to store the values returned by FETCH." & vbNewLine)
            sb.Append("DECLARE @UserID int" & vbNewLine)
            sb.Append("DECLARE @AuthID int" & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append(" DECLARE userauth_cursor CURSOR FOR " & vbNewLine)
            sb.Append(SqlQuery & vbNewLine) 'embed the base query sql 
            sb.Append(vbNewLine)
            sb.Append(" OPEN userauth_cursor " & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append("-- Perform the first fetch and store the values in variables." & vbNewLine)
            sb.Append("-- Note: The variables are in the same order as the columns" & vbNewLine)
            sb.Append("-- in the SELECT statement. " & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append(" FETCH NEXT FROM userauth_cursor " & vbNewLine)
            sb.Append("INTO @UserID, @AuthID" & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append("-- Check @@FETCH_STATUS to see if there are any more rows to fetch." & vbNewLine)
            sb.Append("WHILE @@FETCH_STATUS = 0" & vbNewLine)
            sb.Append("BEGIN" & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append("   -- Concatenate and display the current values in the variables." & vbNewLine)
            sb.Append("   EXEC AdminPrivate_DeleteApplicationRightsByUser @AuthID" & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append("   -- This is executed as long as the previous fetch succeeds." & vbNewLine)
            sb.Append("   FETCH NEXT FROM userauth_cursor " & vbNewLine)
            sb.Append("   INTO @UserID, @AuthID" & vbNewLine)
            sb.Append("END" & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append(" CLOSE userauth_cursor " & vbNewLine)
            sb.Append(" DEALLOCATE userauth_cursor " & vbNewLine)
            sb.Append(vbNewLine)
            sb.Append(SqlQuery & vbNewLine) 'embed the base query sql 

            'remove authorization for single user
            'logging of deleting authorization is handled by the stored procedure AdminPrivate_DeleteApplicationRightsByUser
            Dim sqlParams_del As SqlParameter() = {New SqlParameter("@AppID", NewAppID)}
            Dim testAl As New ArrayList
            Try
                testAl = ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlConnection(cammWebManager.ConnectionString), sb.ToString, CommandType.Text, sqlParams_del, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Catch ex As Exception
                lblErr.Text &= ex.ToString
            End Try

            If testAl.Count <= 0 AndAlso lblErr.Text = "" Then
                InfoLbl.Text = "Cleanup successfull!"
                InfoLbl.ForeColor = Drawing.Color.Green
            Else
                InfoLbl.Text = "Was not able to cleanup all authorizations!"
                InfoLbl.ForeColor = Drawing.Color.Red
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get all users that are authorized by a group
        ''' </summary>
        ''' <param name="NewAppId"></param>
        ''' <returns>ArrayList of users that are authorized by group (userID)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	16.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetAuthorizedUsersByGroup(ByVal NewAppId As Integer) As ArrayList
            'Get authorized groups
            Dim groupAL As ArrayList
            Dim sqlParams As SqlParameter() = {New SqlParameter("@AppID", NewAppId)}
            Dim strQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select ID_Group from [view_ApplicationRights] where id_application=@AppID and isnull(ID_Group,0)<>0 order by ID_Group"
            groupAL = ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            'Get users of authorized groups
            Dim userInGroupAl As New ArrayList
            For myGroupCounter As Integer = 0 To groupAL.Count - 1
                Dim tmpAl As ArrayList
                strQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "SELECT ID_User FROM [view_Memberships] WHERE ID_Group = " & groupAL(myGroupCounter).ToString & " And ID_Group not in (Select id_group_public from system_servergroups) and ID_Group not in (Select id_group_anonymous from system_servergroups) and (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments Where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Groups' AND AuthorizationType In ('SecurityMaster','ViewAllRelations')) OR id_group in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " AND TableName = 'Groups' AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner'))) ORDER BY ID_User"
                tmpAl = ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                For myTmpCounter As Integer = 0 To tmpAl.Count - 1
                    If Not userInGroupAl.Contains(tmpAl(myTmpCounter)) Then userInGroupAl.Add(tmpAl(myTmpCounter))
                Next
            Next
            Return userInGroupAl
        End Function
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWM.Admin
    ''' Class	 : camm.WebManager.Pages.Administration.CheckUsersForMissingFlags
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to list authorized users for an application with missing RequiredUserFlags
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[zeutzheim]	13.08.2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AppCheckUsersForMissingFlags
        Inherits Page

#Region "Variable Declaration"
        Protected AppID As Integer
        Protected UsersWithMissingFlagsGrid As DataGrid
        Protected ltrInfo As Literal
#End Region

        Private Sub AppCheckUsersForMissingFlags_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            AppID = CType(Request.QueryString("ID"), Integer)
            Dim resultDt As New DataTable("result")
            resultDt.Columns.Add("UserName", GetType(String))
            resultDt.Columns.Add("MissingFlags", GetType(String))

            'Get all users of the application
            Dim Users As DataTable = GetUsersByAppID(AppID)
            'Get all required flags of the application
            Dim AppFlags As String() = GetRequiredFlagsByAppID(AppID)

            'Step through all users
            For myUserCounter As Integer = 0 To Users.Rows.Count - 1
                'Contains the missing flags of the user in this application
                Dim LinksToMissingFlags As String = Nothing

                'Step through all required flags of the application
                For myFlagCounter As Integer = 0 To AppFlags.Length - 1
                    If IsFlagMissing(CLng(Users.Rows(myUserCounter)("ID_User")), AppFlags(myFlagCounter)) Then
                        'Add the missing flag as a link to the flag-update page to the list
                        Dim seperator As String = Nothing
                        If LinksToMissingFlags <> "" Then
                            seperator = ",&nbsp;"
                        End If
                        LinksToMissingFlags &= seperator & "<a target=""_blank"" href=""users_update_flag.aspx?ID=" & CLng(Utils.Nz(Users.Rows(myUserCounter)("ID_User"))) & "&Type=" & Server.UrlEncode(Trim(AppFlags(myFlagCounter))) & """>" & Server.HtmlEncode(Trim(AppFlags(myFlagCounter))) & "</a>"
                    End If
                Next
                If LinksToMissingFlags <> Nothing Then
                    'Add a new row with all information to the result table
                    Dim row As DataRow = resultDt.NewRow
                    'Provide username as a link to the user-update page
                    row("UserName") = "<a target=""_blank"" href=""users_update.aspx?ID=" & CLng(Utils.Nz(Users.Rows(myUserCounter)("ID_User"))) & """>" & cammWebManager.System_GetUserInfo(CLng(Utils.Nz(Users.Rows(myUserCounter)("ID_User")))).FullName & " (" & cammWebManager.System_GetUserInfo(CLng(Utils.Nz(Users.Rows(myUserCounter)("ID_User")))).LoginName & ")" & "</a>"
                    row("MissingFlags") = LinksToMissingFlags
                    resultDt.Rows.Add(row)
                End If
            Next

            If resultDt.Rows.Count > 0 Then
                'Bind the data to the datagrid
                UsersWithMissingFlagsGrid.DataSource = resultDt
                UsersWithMissingFlagsGrid.DataBind()
            Else
                ltrInfo.Text = "No users with missing flagvalues."
            End If

        End Sub

        Private Function IsFlagMissing(ByVal userID As Long, ByVal flagName As String) As Boolean
            If Me.cammWebManager.System_GetUserInfo(userID).AdditionalFlags(Trim(flagName)) Is Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function IsFlagFilled(ByVal userID As Long, ByVal flagName As String) As Boolean
            If IsFlagMissing(userID, flagName) = False Then
                If CStr(Me.cammWebManager.System_GetUserInfo(userID).AdditionalFlags(Trim(flagName))) <> Nothing Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        Public Function GetRequiredFlagsByAppID(ByVal ID As Integer) As String()
            Dim cmd3 As New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select RequiredUserProfileFlags From Applications_CurrentAndInactiveOnes Where ID = @ID", New SqlConnection(cammWebManager.ConnectionString))
            cmd3.Parameters.Add("@ID", SqlDbType.Int).Value = ID
            Dim al As ArrayList = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd3, Automations.AutoOpenAndCloseAndDisposeConnection)
            If al.Item(0) Is DBNull.Value Then
                Return New String() {}
            Else
                Return CStr(al.Item(0)).Split(","c)
            End If
        End Function

        Private Function GetUsersByAppID(ByVal ID As Integer) As DataTable
            Dim cmd As New SqlClient.SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select * From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID union select ID_USer from Memberships where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group  is not null and ID_Application = @ID)) as a Group by ID_User", New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
            Return FillDataTable(cmd, Automations.AutoOpenAndCloseAndDisposeConnection, "usertable")
        End Function

    End Class

End Namespace


