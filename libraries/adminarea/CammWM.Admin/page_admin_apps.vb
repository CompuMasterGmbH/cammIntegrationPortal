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
    '''     A page to view list of applications
    ''' </summary>
    Public Class ApplicationList
        Inherits Page

#Region "Variable Declaration"
        Protected cmbMarket, cmbServerGroup As DropDownList
        Protected txtApplication As TextBox
        Protected lblErrMsg, lblNavURL, lblTitle, lblAbbreviation, lblReleasedOn, lblServerDescription, lblDescription As Label
        Protected hlnNew, hlnSecurity, hlnAnchorID, hlnID, hlnTitleAdminArea, hlnDescription, hlnReleasedByLastName As HyperLink
        Protected hlnUpdate, hlnDelete, hlnClone As HyperLink
        Protected WithEvents rptAppList As Repeater
        Protected gc As Web.UI.HtmlControls.HtmlGenericControl
        Protected chkTop50Only As CheckBox
        Protected WithEvents btnsubmit As Button
        Dim Odd As Boolean
        Dim MyRecCounter As Integer
        Dim MyDt As New DataTable
#End Region

#Region "Page Events"
        Private Sub ApplicationList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""
        End Sub

        Private Sub ApplicationList_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            Try
                Dim serverindex As Integer = cmbServerGroup.SelectedIndex
                Dim marketindex As Integer = cmbMarket.SelectedIndex
                FillDropDownLists()
                cmbServerGroup.SelectedIndex = serverindex
                cmbMarket.SelectedIndex = marketindex
                ListOfApps()

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptAppList.DataSource = MyDt
                    rptAppList.DataBind()
                Else
                    lblErrMsg.Text = "No records found matching your search request."
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            Finally
                MyDt.Dispose()
            End Try
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub FillDropDownLists()
            Dim dt As New DataTable

            Try
                dt = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                                    "SELECT ServerGroup,id FROM System_ServerGroups ORDER BY ServerGroup", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                cmbServerGroup.Items.Clear()
                cmbServerGroup.Items.Insert(0, New ListItem("", ""))
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        cmbServerGroup.Items.Add(New ListItem(Utils.Nz(dr("ServerGroup"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If

                'for Market
                dt = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                                    "SELECT Description,id FROM view_Languages WHERE IsActive = 1 ORDER BY Description", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                cmbMarket.Items.Clear()
                cmbMarket.Items.Insert(0, New ListItem("", ""))
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        cmbMarket.Items.Add(New ListItem(Utils.Nz(dr("Description"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            Finally
                dt.Dispose()
            End Try
        End Sub

        Private Sub ListOfApps()

            Dim Top50Constraint As String = ""
            If chkTop50Only.Checked Then
                Top50Constraint = "Top 50"
            End If

            Dim strWHERE As New Text.StringBuilder
            strWHERE.Append("WHERE ")

            If Trim(txtApplication.Text) <> "" Then
                strWHERE.Append(" (TitleAdminArea Like @TitleAdminArea Or Title Like @ApplicationText Or NavUrl Like @NavUrl) And")
            End If

            If Val(cmbMarket.SelectedIndex & "") > 0 Then
                strWHERE.Append(" LanguageID = @LanguageID And")
                cmbMarket.SelectedIndex = cmbMarket.Items.IndexOf(cmbMarket.Items.FindByValue(cmbMarket.SelectedValue))
            End If

            If Val(cmbServerGroup.SelectedIndex & "") > 0 Then
                strWHERE.Append(" LocationID in (Select ID from System_Servers Where ServerGroup=@ServerGroup) And")
                cmbServerGroup.SelectedIndex = cmbServerGroup.Items.IndexOf(cmbServerGroup.Items.FindByValue(cmbServerGroup.SelectedValue))
            End If

            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)),
                New SqlParameter("@TitleAdminArea", txtApplication.Text.ToString.Trim.Replace(") '", "''").Replace("*", "%") & "%"),
                New SqlParameter("@ServerGroup", cmbServerGroup.SelectedValue),
                New SqlParameter("@LanguageID", cmbMarket.SelectedValue),
                New SqlParameter("@ApplicationText", "%" & txtApplication.Text.Trim.Replace("'", "''").Replace("*", "%") & "%"),
                New SqlParameter("@NavUrl", "%" & txtApplication.Text.Trim.Replace("'", "''").Replace("*", "%") & "%")}
            strWHERE.Append(" (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('SecurityMaster')) OR view_applications.id in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('Update','Owner')))")
            Dim strQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                                    "SELECT " & Top50Constraint & " view_applications.*, system_servers.serverdescription, view_Languages.Description As Abbreviation, view_Languages.Description FROM ([view_Applications] left join System_Servers on view_applications.Locationid = system_servers.id) left join view_Languages on view_applications.languageid = view_Languages.id " & strWHERE.ToString & " ORDER BY Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, Level1Title, Level2Title, Level3Title, NavURL"

            Try
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            Finally
                MyDt.Dispose()
            End Try
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptAppListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptAppList.ItemDataBound
            If e.Item.ItemType = ListItemType.Header Then
                Dim CurUserIsAllowedToAddNewItems, CurUserIsSecurityMaster As Boolean

                CurUserIsAllowedToAddNewItems = cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "New")
                CurUserIsSecurityMaster = cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))

                If CurUserIsAllowedToAddNewItems Then
                    CType(e.Item.FindControl("hlnNew"), HyperLink).NavigateUrl = "apps_new.aspx" & ""
                    CType(e.Item.FindControl("hlnNew"), HyperLink).Text = "New"
                End If

                If CurUserIsSecurityMaster Then
                    CType(e.Item.FindControl("hlnSecurity"), HyperLink).NavigateUrl = "adjust_delegates.aspx?ID=0&Type=Applications&Title=All+applications"
                    CType(e.Item.FindControl("hlnSecurity"), HyperLink).Text = "Security"
                End If
            End If

            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                With MyDt.Rows(e.Item.ItemIndex)
                    MyRecCounter = MyRecCounter + 1

                    If Not IsDBNull(.Item("AppDisabled")) Then
                        If CBool(.Item("AppDisabled")) = True Then CType(e.Item.FindControl("hlnID"), HyperLink).Style.Add("forecolor", "gray") Else CType(e.Item.FindControl("hlnID"), HyperLink).Style.Add("forecolor", "black")
                    Else
                        CType(e.Item.FindControl("hlnID"), HyperLink).Style.Add("forecolor", "black")
                    End If

                    CType(e.Item.FindControl("hlnID"), HyperLink).Text = .Item("ID").ToString
                    CType(e.Item.FindControl("hlnAnchorID"), HyperLink).Attributes.Add("name", "ID" & .Item("ID").ToString)
                    If Not IsDBNull(.Item("AppDisabled")) Then If CBool(.Item("AppDisabled")) = True Then CType(e.Item.FindControl("gc"), Web.UI.HtmlControls.HtmlGenericControl).InnerHtml = "<br><nobr title=""Disabled"">(D)</nobr>"
                    If Not IsDBNull(.Item("NavURL")) Then CType(e.Item.FindControl("lblNavURL"), Label).Text = .Item("NavURL").ToString
                    CType(e.Item.FindControl("lblTitle"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Title").ToString, String.Empty))
                    CType(e.Item.FindControl("lblServerDescription"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ServerDescription"), ""))
                    CType(e.Item.FindControl("lblDescription"), Label).ToolTip = Server.HtmlEncode(Utils.Nz(.Item("Description"), String.Empty))
                    CType(e.Item.FindControl("lblAbbreviation"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Abbreviation"), String.Empty))

                    If CBool(.Item("SystemApp")) = False OrElse Utils.Nz(.Item("SystemAppType"), 0) = 3 Then
                        CType(e.Item.FindControl("hlnDescription"), HyperLink).NavigateUrl = "apprights.aspx?Application=" & CInt(.Item("ID")) & "&AuthsAsAppID=" & Utils.Nz(.Item("AuthsAsAppID"), 0)
                        CType(e.Item.FindControl("hlnDescription"), HyperLink).Text = "Check Auths."
                    End If

                    CType(e.Item.FindControl("hlnTitleAdminArea"), HyperLink).NavigateUrl = "apps_update.aspx?ID=" & CInt(.Item("ID"))
                    CType(e.Item.FindControl("hlnTitleAdminArea"), HyperLink).Text = ""

                    If Utils.Nz(.Item("TitleAdminArea"), "") <> "" Then CType(e.Item.FindControl("hlnTitleAdminArea"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("TitleAdminArea"), "")) Else CType(e.Item.FindControl("hlnTitleAdminArea"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Title"), String.Empty))
                    Dim userinfo As WMSystem.UserInformation
                    If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CInt(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CInt(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CInt(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CInt(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CInt(.Item("ReleasedByID")) Then
                        userinfo = New WebManager.WMSystem.UserInformation(CType(.Item("ReleasedByID"), Int64), cammWebManager, False)
                        CType(e.Item.FindControl("hlnReleasedByLastName"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(userinfo.FullName, ""))
                        CType(e.Item.FindControl("hlnReleasedByLastName"), HyperLink).NavigateUrl = ""
                    Else
                        CType(e.Item.FindControl("hlnReleasedByLastName"), HyperLink).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(.Item("ReleasedByFirstName"), .Item("ReleasedByLastName"), CLng(Utils.Nz(.Item("ReleasedByID"), 0))))
                        If Trim(CompuMaster.camm.WebManager.Utils.Nz(.Item("ReleasedByLoginName"), String.Empty)) = "" Then
                            'user already deleted
                            CType(e.Item.FindControl("hlnReleasedByLastName"), HyperLink).NavigateUrl = ""
                        Else
                            'existing user account
                            CType(e.Item.FindControl("hlnReleasedByLastName"), HyperLink).NavigateUrl = "users_update.aspx?ID=" & CInt(.Item("ReleasedByID"))
                        End If
                    End If
                    CType(e.Item.FindControl("lblReleasedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ReleasedOn"), String.Empty))

                    If Utils.Nz(.Item("SystemApp"), 0) = 0 OrElse Utils.Nz(.Item("SystemAppType"), 0) = 3 Then
                        CType(e.Item.FindControl("hlnUpdate"), HyperLink).NavigateUrl = "apps_update.aspx?ID=" & CInt(.Item("ID"))
                        CType(e.Item.FindControl("hlnUpdate"), HyperLink).Text = "Update"
                        CType(e.Item.FindControl("hlnDelete"), HyperLink).NavigateUrl = "apps_delete.aspx?ID=" & CInt(.Item("ID"))
                        CType(e.Item.FindControl("hlnDelete"), HyperLink).Text = "Delete"
                        CType(e.Item.FindControl("hlnClone"), HyperLink).NavigateUrl = "apps_clone.aspx?ID=" & CInt(.Item("ID"))
                        CType(e.Item.FindControl("hlnClone"), HyperLink).Text = "Clone"
                    End If
                End With
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to create new application
    ''' </summary>
    Public Class ApplicationNew
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg As Label
        Protected txtTitle As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub ApplicationNew_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "New")) Then
                Response.Write("No authorization to create new applications.")
                Response.End()
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If Trim(txtTitle.Text) <> "" Then
                Dim iResult As Object
                Dim Redirect2URL As String = ""

                Dim sqlParams As SqlParameter() = {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@Title", Trim(txtTitle.Text))}
                iResult = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_CreateApplication", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If Utils.Nz(iResult, 0L) <> 0 Then
                    Redirect2URL = "apps_update.aspx?ID=" & iResult.ToString
                Else
                    lblErrMsg.Text = "Application creation failed!"
                End If

                If Redirect2URL <> "" Then Utils.RedirectTemporary(HttpContext.Current, Redirect2URL)
            Else
                lblErrMsg.Text = "Please specify an application title."
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to clone an application
    ''' </summary>
    Public Class ApplicationClone
        Inherits Page

#Region "Page Events"
        Private Sub ApplicationClone_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim ErrMsg, Redirect2URL As String
            Dim iResult As Object = Nothing
            ErrMsg = ""
            Redirect2URL = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "New")) Then
                Response.Write("No authorization to create new applications.")
                Response.End()
            ElseIf Request.QueryString("ID") <> "" And Request.Form("CloneType") <> "" Then
                Dim sqlParams As SqlParameter()
                If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                    'Clone AdminSecurityDelegates - supported since DB-Build 185
                    sqlParams = New SqlParameter() {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@AppID", CLng(Request.QueryString("ID"))), New SqlParameter("@CloneType", CLng(Request.Form("CloneType"))), New SqlParameter("@CopyDelegates", CLng(Utils.Nz(Request.Form("CopyAdminSecurityDelegates"), 0)))}
                Else
                    sqlParams = New SqlParameter() {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@AppID", CLng(Request.QueryString("ID"))), New SqlParameter("@CloneType", CLng(Request.Form("CloneType")))}
                End If

                iResult = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_CloneApplication", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                If Utils.Nz(iResult, 0L) <> 0 Then
                    Redirect2URL = "apps_update.aspx?ID=" & iResult.ToString
                Else
                    ErrMsg = "Application creation failed!"
                End If

                If Redirect2URL <> "" Then Utils.RedirectTemporary(HttpContext.Current, Redirect2URL)
            ElseIf Request.Form("submit") <> "" Then
                ErrMsg = "Application cloning failed."
            End If

            If ErrMsg <> "" Then Response.Write("<p><font face=""Arial"" size=""2"" color=""red"">" & Utils.Nz(ErrMsg, String.Empty) & "</font></p>")
        End Sub
#End Region

    End Class

    '' <summary>
    ''     A page to update an application
    '' </summary>
    Public Class ApplicationUpdate
        Inherits Page

#Region "Variable Declaration"
        Protected lblField_ID, lblField_ReleasedOn, lblField_ModifiedOn, lblErrMsg As Label
        Protected txtField_Level1Title, txtField_Level2Title, txtField_Level3Title As TextBox
        Protected txtField_Level4Title, txtField_Level5Title, txtField_Level6Title As TextBox
        Protected txtField_Title, txtField_TitleAdminArea, txtField_RequiredUserFlags, txtField_RequiredUserFlagsRemarks As TextBox
        Protected txtField_ResetIsNewUpdatedStatusOn, txtField_Sort, txtField_NavJSOnClick As TextBox
        Protected txtField_NavURL, txtField_NavFrame, txtField_NavTooltip, txtField_NavJSOnMOver, txtField_NavJSOnMOut As TextBox
        Protected txtField_GeneralRemarks As TextBox
        Protected rbNew, rbUpdate, rbStandard As RadioButton
        Protected rbLevel1TitleText, rbLevel2TitleText, rbLevel3TitleText As RadioButton
        Protected rbLevel4TitleText, rbLevel5TitleText, rbLevel6TitleText As RadioButton
        Protected rbLevel1TitleHTML, rbLevel2TitleHTML, rbLevel3TitleHTML As RadioButton
        Protected rbLevel4TitleHTML, rbLevel5TitleHTML, rbLevel6TitleHTML As RadioButton
        Protected cmbLocation, cmbLanguage, cmbAppDisabled, cmbAddLanguageID2URL As DropDownList
        Protected hypLastModifiedBy, hypCreatedBy, hypUsersMissingFlags As HyperLink
        Protected WithEvents btnSubmit As Button
        Protected tdAddLinks As Web.UI.HtmlControls.HtmlTableCell
        Protected cammWebManagerAdminDelegates As CompuMaster.camm.WebManager.Controls.Administration.AdministrativeDelegates
        Dim Field_LocationID, Field_Language, Field_AppDisabled, Field_AddLanguageID2URL As Integer
#End Region

#Region "Page Events"
        Private Sub ApplicationUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Update")) Then
                Response.Write("No authorization to administrate this application.")
                Response.End()
            Else
                If Not IsPostBack Then
                    SetControlValues()
                    FillDropDownLists()
                End If
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        Private Sub SetControlValues()
            Dim dtUpdate As New DataTable
            Dim dr As DataRow
            Dim strBlr As New Text.StringBuilder

            hypUsersMissingFlags.NavigateUrl = "apps_users_missing_flags.aspx?ID=" & CType(Request.QueryString("ID"), Integer)

            Try
                lblField_ID.Text = Server.HtmlEncode(Trim(Request.QueryString("ID")))
                Dim MySecurityObjectInfo As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(CInt(Trim(Request.QueryString("ID"))), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                Dim cmd As New SqlClient.SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.Applications WHERE ID=@ID", New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CType(Request.QueryString("ID"), Integer)
                dtUpdate = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                cammWebManagerAdminDelegates.SecurityObjectInfo = MySecurityObjectInfo

                If Not dtUpdate Is Nothing AndAlso dtUpdate.Rows.Count > 0 Then
                    For Each dr In dtUpdate.Rows
                        txtField_Title.Text = MySecurityObjectInfo.Name
                        txtField_TitleAdminArea.Text = MySecurityObjectInfo.DisplayName
                        txtField_Level1Title.Text = Utils.Nz(dr("Level1Title"), String.Empty)
                        txtField_Level2Title.Text = Utils.Nz(dr("Level2Title"), String.Empty)
                        txtField_Level3Title.Text = Utils.Nz(dr("Level3Title"), String.Empty)
                        txtField_Level4Title.Text = Utils.Nz(dr("Level4Title"), String.Empty)
                        txtField_Level5Title.Text = Utils.Nz(dr("Level5Title"), String.Empty)
                        txtField_Level6Title.Text = Utils.Nz(dr("Level6Title"), String.Empty)

                        If CBool(dr("Level1TitleIsHTMLCoded")) = True Then rbLevel1TitleHTML.Checked = True Else rbLevel1TitleText.Checked = True
                        If CBool(dr("Level2TitleIsHTMLCoded")) = True Then rbLevel2TitleHTML.Checked = True Else rbLevel2TitleText.Checked = True
                        If CBool(dr("Level3TitleIsHTMLCoded")) = True Then rbLevel3TitleHTML.Checked = True Else rbLevel3TitleText.Checked = True
                        If CBool(dr("Level4TitleIsHTMLCoded")) = True Then rbLevel4TitleHTML.Checked = True Else rbLevel4TitleText.Checked = True
                        If CBool(dr("Level5TitleIsHTMLCoded")) = True Then rbLevel5TitleHTML.Checked = True Else rbLevel5TitleText.Checked = True
                        If CBool(dr("Level6TitleIsHTMLCoded")) = True Then rbLevel6TitleHTML.Checked = True Else rbLevel6TitleText.Checked = True

                        txtField_NavURL.Text = Utils.Nz(dr("NavURL"), String.Empty)
                        txtField_NavFrame.Text = Utils.Nz(dr("NavFrame"), String.Empty)
                        Field_LocationID = CInt(dr("LocationID"))
                        Field_Language = CInt(dr("LanguageID"))
                        Field_AppDisabled = CInt(IIf(CType(dr("AppDisabled"), Boolean) = True, 1, 0)) + 1
                        txtField_NavTooltip.Text = Utils.Nz(dr("NavTooltipText"), String.Empty)
                        txtField_NavJSOnMOver.Text = Utils.Nz(dr("OnMouseOver"), String.Empty)
                        txtField_NavJSOnMOut.Text = Utils.Nz(dr("OnMouseOut"), String.Empty)
                        txtField_NavJSOnClick.Text = Utils.Nz(dr("OnClick"), String.Empty)
                        txtField_Sort.Text = Utils.Nz(dr("Sort"), String.Empty)
                        txtField_RequiredUserFlags.Text = Utils.Nz(dr("RequiredUserProfileFlags"), String.Empty)
                        txtField_GeneralRemarks.Text = Utils.Nz(dr("Remarks"), String.Empty)
                        If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                            txtField_RequiredUserFlagsRemarks.Text = Utils.Nz(dr("RequiredUserProfileFlagsRemarks"), String.Empty)
                        End If

                        If CBool(dr("IsNew")) Then
                            rbNew.Checked = True
                        ElseIf CBool(dr("IsUpdated")) Then
                            rbUpdate.Checked = True
                        Else
                            rbStandard.Checked = True
                        End If

                        If Not IsDBNull(dr("ResetIsNewUpdatedStatusOn")) Then txtField_ResetIsNewUpdatedStatusOn.Text = Year(CDate(dr("ResetIsNewUpdatedStatusOn"))) & "-" & Month(CDate(dr("ResetIsNewUpdatedStatusOn"))) & "-" & Day(CType(dr("ResetIsNewUpdatedStatusOn"), Date)) & " " & Hour(CType(dr("ResetIsNewUpdatedStatusOn"), DateTime)) & ":" & Minute(CType(dr("ResetIsNewUpdatedStatusOn"), DateTime)) & ":" & Second(CType(dr("ResetIsNewUpdatedStatusOn"), Date)) Else txtField_ResetIsNewUpdatedStatusOn.Text = ""
                        If CBool(dr("AddLanguageID2URL")) Then Field_AddLanguageID2URL = 1 Else Field_AddLanguageID2URL = 0
                        txtField_RequiredUserFlags.Text = Utils.Nz(dr("RequiredUserProfileFlags"), String.Empty)
                    Next
                Else
                    lblErrMsg.Text = "Application not found!"
                End If

                Try
                    If Not MySecurityObjectInfo.ReleasedBy_UserInfo Is Nothing Then
                        lblField_ReleasedOn.Text = Utils.Nz(MySecurityObjectInfo.ReleasedOn, String.Empty)
                        Dim Field_ReleasedByID As String = Utils.Nz(MySecurityObjectInfo.ReleasedBy_UserInfo.ID, String.Empty)
                        Dim Field_ReleasedByName As String = MySecurityObjectInfo.ReleasedBy_UserInfo.FullName
                        hypCreatedBy.NavigateUrl = "users_update.aspx?ID=" & Field_ReleasedByID
                        hypCreatedBy.Text = Server.HtmlEncode(Utils.Nz(Field_ReleasedByName, String.Empty))
                    End If
                Catch ex As ArgumentNullException
                    hypCreatedBy.Text = "{User ID 0}"
                Catch ex As Exception
                    Throw New Exception("Unexpected exception", ex)
                End Try

                lblField_ModifiedOn.Text = Utils.Nz(MySecurityObjectInfo.ModifiedOn, String.Empty)
                Try
                    Dim Field_ModifiedByID As String = ""
                    If Val(MySecurityObjectInfo.ModifiedBy_UserID & "") <> 0 Then
                        Field_ModifiedByID = Utils.Nz(MySecurityObjectInfo.ModifiedBy_UserInfo.ID, String.Empty)
                        Dim Field_ModifiedByName As String = MySecurityObjectInfo.ModifiedBy_UserInfo.FullName
                        hypLastModifiedBy.NavigateUrl = "users_update.aspx?ID=" & Field_ModifiedByID
                        hypLastModifiedBy.Text = Server.HtmlEncode(Utils.Nz(Field_ModifiedByName, String.Empty))
                    Else
                        Dim userinfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(CType(MySecurityObjectInfo.ModifiedBy_UserID, Int64), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem), True)
                        Field_ModifiedByID = Utils.Nz(MySecurityObjectInfo.ModifiedBy_UserID, String.Empty)
                        Dim Field_ModifiedByName As String = userinfo.FullName
                        hypLastModifiedBy.NavigateUrl = "users_update.aspx?ID=" & Field_ModifiedByID
                        hypLastModifiedBy.Text = Server.HtmlEncode(Utils.Nz(Field_ModifiedByName, "{User ID 0}"))
                    End If
                Catch ex As ArgumentNullException
                    hypLastModifiedBy.Text = "{User ID 0}"
                Catch ex As Exception
                    Throw New Exception("Unexpected exception", ex)
                End Try

                Try
                    strBlr.Append("<table cellSpacing=""0"" cellPadding=""0"" border=""0"">")
                    Dim cmd2 As New SqlClient.SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ItemType, ID_Group, Name, ID_User, LoginDisabled, LoginName, DevelopmentTeamMember FROM view_ApplicationRights WHERE ID_Application = @ID AND ID_AppRight Is NOT Null ORDER BY ItemType, Name", New SqlConnection(cammWebManager.ConnectionString))
                    cmd2.Parameters.Add("@ID", SqlDbType.Int).Value = CType(Trim(Request.QueryString("ID")), Integer)
                    dtUpdate = FillDataTable(cmd2, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    If Not dtUpdate Is Nothing Then
                        For Each dr In dtUpdate.Rows
                            If CInt(dr("ItemType")) = 1 Then
                                strBlr.Append("<TR><TD VAlign=""Top"" WIDTH=""160""><P><FONT face=""Arial"" size=""2"">Group</FONT></P></TD><TD VAlign=""Top"" Width=""240""><P><FONT face=""Arial"" size=""2""><a href=""groups_update.aspx?ID=" & Utils.Nz(dr("ID_Group"), 0) & """>" & Server.HtmlEncode(Utils.Nz(dr("Name"), String.Empty)) & "</a></FONT></P></TD></TR>")
                            Else
                                strBlr.Append("<TR><TD VAlign=""Top"" WIDTH=""160""><P><FONT face=""Arial"" size=""2"">User " + Utils.Nz(IIf(Utils.Nz(dr("DevelopmentTeamMember"), False), "<B title=""Authorization for test and development purposes and for inactive security objects""> {Dev} </B>", ""), String.Empty) + "</FONT></P></TD><TD VAlign=""Top"" Width=""240""><P><FONT face=""Arial"" size=""2""><a href=""users_update.aspx?ID=" & Utils.Nz(dr("ID_User"), 0).ToString & """>" & Utils.Nz(Server.HtmlEncode(Me.SafeLookupUserFullName(Utils.Nz(dr("ID_User"), -1L))), String.Empty) & " (" & Server.HtmlEncode(Utils.Nz(dr("LoginName"), String.Empty)) & ")</a>" & Utils.Nz(IIf(Utils.Nz(dr("LoginDisabled"), False) <> False, "&nbsp;<em><font color=""#D1D1D1"">(Disabled)</font></em>", ""), String.Empty) & "</FONT></P></TD></TR>")
                            End If
                        Next
                    End If
                Catch ex As Exception
                    Throw New Exception("Unexpected exception", ex)
                End Try

                strBlr.Append("</table>")
                tdAddLinks.InnerHtml = strBlr.ToString
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            Finally
                dtUpdate.Dispose()
                dr = Nothing
                strBlr = Nothing
            End Try
        End Sub

        Private Sub FillDropDownLists()
            Dim dtUpdate As New DataTable

            Try
                'bind location dropdownlist
                cmbLocation.Items.Clear()
                If Field_LocationID = Nothing Then cmbLocation.Items.Add(New ListItem("(Please select!)", ""))
                dtUpdate = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM System_Servers WHERE Enabled = 1 ORDER BY ServerDescription", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtUpdate Is Nothing Then
                    For Each dr As DataRow In dtUpdate.Rows
                        cmbLocation.Items.Add(New ListItem(Utils.Nz(dr("ServerDescription"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next

                    If cmbLocation.Items.Count > 0 AndAlso Val(Field_LocationID & "") <> 0 Then cmbLocation.SelectedValue = Utils.Nz(Field_LocationID, 0).ToString
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            End Try

            Try
                'bind language dropdownlist
                cmbLanguage.Items.Clear()
                If Field_Language = Nothing Then cmbLanguage.Items.Add(New ListItem("(Please select!)", ""))
                dtUpdate = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM view_Languages WHERE IsActive = 1 ORDER BY Description", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtUpdate Is Nothing Then
                    For Each dr As DataRow In dtUpdate.Rows
                        cmbLanguage.Items.Add(New ListItem(Utils.Nz(dr("Description"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next

                    If cmbLanguage.Items.Count > 0 AndAlso Val(Field_Language & "") <> 0 Then cmbLanguage.SelectedValue = Utils.Nz(Field_Language, 0).ToString
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            End Try

            'bind AppDisabled dropdownlist
            cmbAppDisabled.Items.Clear()
            If Field_AppDisabled = Nothing Then cmbAppDisabled.Items.Add(New ListItem("(Please select!)", ""))
            cmbAppDisabled.Items.Add(New ListItem("Yes", "2"))
            cmbAppDisabled.Items.Add(New ListItem("No", "1"))
            If Utils.Nz(Field_AppDisabled, 0) = 2 Then cmbAppDisabled.SelectedValue = "2"
            If Utils.Nz(Field_AppDisabled, 0) = 1 Then cmbAppDisabled.SelectedValue = "1"

            'bind addlanguageurl dropdownlist
            cmbAddLanguageID2URL.Items.Clear()
            If Field_AddLanguageID2URL = Nothing Then cmbAppDisabled.Items.Add(New ListItem("(Please select!)", ""))
            cmbAddLanguageID2URL.Items.Add(New ListItem("Yes", "2"))
            cmbAddLanguageID2URL.Items.Add(New ListItem("No", "1"))
            If Utils.Nz(Field_AddLanguageID2URL, 0) = 1 Then cmbAddLanguageID2URL.SelectedValue = "2"
            If Utils.Nz(Field_AddLanguageID2URL, 0) = 0 Then cmbAddLanguageID2URL.SelectedValue = "1"
        End Sub




#End Region

#Region "Control Events"


        Private Sub SetSubmitErrorText(ByVal requiredFlags As ArrayList)

            lblErrMsg.Text = Server.HtmlDecode("<hr><p><b>You changed the Required user flags,</b><br>but there are some users authorized for this app which do not have one or more of the required user flags, or the value of a flag is invalid for its type. You need to <b>add these flags</b> to all listed users <b>before</b> you can update this application.</p>")
            lblErrMsg.Text &= "<p>The following flags are affected:<br>"
            Dim HtmlCode As New System.Text.StringBuilder
            For Each rFlag As String In requiredFlags
                HtmlCode.Append("<a target=""_blank"" href=""users_batchuserflageditor.aspx?AppID=" & Request.QueryString("ID") & "&Flag=" & Server.UrlEncode(Trim(rFlag)) & "&EditMode=3"">Edit Flag '" & Server.HtmlEncode(Trim(rFlag)) & "'</a><br>")
            Next
            HtmlCode.Append("</p><hr>")
            lblErrMsg.Text &= HtmlCode.ToString()
        End Sub

        ''' <summary>
        ''' Updates the application using the data entered on the page
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub UpdateApplication()
            Dim sqlParams As SqlParameter() = {New SqlParameter("@Title", Trim(txtField_Title.Text)), New SqlParameter("@TitleAdminArea", Trim(txtField_TitleAdminArea.Text)), _
                   New SqlParameter("@Level1Title", Trim(txtField_Level1Title.Text)), New SqlParameter("@Level2Title", Trim(txtField_Level2Title.Text)), New SqlParameter("@Level3Title", Trim(txtField_Level3Title.Text)), New SqlParameter("@Level4Title", Trim(txtField_Level4Title.Text)), New SqlParameter("@Level5Title", Trim(txtField_Level5Title.Text)), New SqlParameter("@Level6Title", Trim(txtField_Level6Title.Text)), _
                   New SqlParameter("@Level1TitleIsHTMLCoded", IIf(rbLevel1TitleHTML.Checked = True, True, False)), New SqlParameter("@Level2TitleIsHTMLCoded", IIf(rbLevel2TitleHTML.Checked = True, True, False)), New SqlParameter("@Level3TitleIsHTMLCoded", IIf(rbLevel3TitleHTML.Checked = True, True, False)), New SqlParameter("@Level4TitleIsHTMLCoded", IIf(rbLevel4TitleHTML.Checked = True, True, False)), New SqlParameter("@Level5TitleIsHTMLCoded", IIf(rbLevel5TitleHTML.Checked = True, True, False)), New SqlParameter("@Level6TitleIsHTMLCoded", IIf(rbLevel6TitleHTML.Checked = True, True, False)), _
                   New SqlParameter("@NavURL", Mid(Trim(txtField_NavURL.Text), 1, 512)), New SqlParameter("@NavFrame", Mid(Trim(txtField_NavFrame.Text), 1, 50)), New SqlParameter("@NavTooltipText", Mid(Trim(txtField_NavTooltip.Text), 1, 1024)), _
                   New SqlParameter("@IsNew", IIf(rbNew.Checked = True, True, False)), New SqlParameter("@IsUpdated", IIf(rbUpdate.Checked = True, True, False)), New SqlParameter("@LocationID", cmbLocation.SelectedValue), New SqlParameter("@LanguageID", cmbLanguage.SelectedValue), New SqlParameter("@ModifiedBy", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), _
                   New SqlParameter("@AppDisabled", IIf(CLng(cmbAppDisabled.SelectedValue) - 1 = 0, False, True)), New SqlParameter("@Sort", IIf(Trim(txtField_Sort.Text) = "", DBNull.Value, Trim(txtField_Sort.Text))), New SqlParameter("@ResetIsNewUpdatedStatusOn", IIf(Trim(txtField_ResetIsNewUpdatedStatusOn.Text) = "", DBNull.Value, Mid(Trim(txtField_ResetIsNewUpdatedStatusOn.Text), 1, 30))), New SqlParameter("@OnMouseOver", Mid(Trim(txtField_NavJSOnMOver.Text), 1, 512)), New SqlParameter("@OnMouseOut", Mid(Trim(txtField_NavJSOnMOut.Text), 1, 512)), _
                   New SqlParameter("@OnClick", Mid(Trim(txtField_NavJSOnClick.Text), 1, 512)), New SqlParameter("@AddLanguageID2URL", IIf(Utils.Nz(cmbAddLanguageID2URL.SelectedValue, 0) <> 1, True, False)), New SqlParameter("@ID", CInt(Request.QueryString("ID")))}

            ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateApp", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            Dim remarks As String = ""
            If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                remarks = ", RequiredUserProfileFlagsRemarks = @UserProfileFlagsRemarks"
            End If

            Dim cmd3 As New SqlCommand("Update Applications_CurrentAndInactiveOnes SET RequiredUserProfileFlags = @RequiredFlags, Remarks = @GeneralRemarks" & remarks & " Where ID = @ID", New SqlConnection(cammWebManager.ConnectionString))
            cmd3.Parameters.Add("@ID", SqlDbType.Int).Value = CType(lblField_ID.Text, Integer)
            cmd3.Parameters.Add("@RequiredFlags", SqlDbType.NVarChar).Value = Trim(txtField_RequiredUserFlags.Text)
            cmd3.Parameters.Add("@GeneralRemarks", SqlDbType.NVarChar).Value = txtField_GeneralRemarks.Text.Trim()
            If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                cmd3.Parameters.Add("@UserProfileFlagsRemarks", SqlDbType.NVarChar).Value = txtField_RequiredUserFlagsRemarks.Text.Trim()
            End If
            ExecuteNonQuery(cmd3, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If CInt(Val(Request.QueryString("ID") & "")) <> 0 AndAlso cmbLocation.SelectedValue <> String.Empty AndAlso cmbAppDisabled.SelectedItem.Text <> String.Empty Then
                lblErrMsg.Text = ""
                Dim invalidFlags As New ArrayList

                Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex()
                If MyDBVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then
                    Dim requiredApplicationFlags() As String = txtField_RequiredUserFlags.Text.Split(CChar(","))

                    Dim cmd As SqlClient.SqlCommand
                    If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                        cmd = New SqlClient.SqlCommand("Select * From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID union select ID_User from Memberships_EffectiveRulesWithClonesNthGrade where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group  is not null and ID_Application = @ID)) as a GROUP BY ID_User", New SqlConnection(cammWebManager.ConnectionString))
                    Else
                        cmd = New SqlClient.SqlCommand("Select * From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID union select ID_User from Memberships where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group  is not null and ID_Application = @ID)) as a GROUP BY ID_User", New SqlConnection(cammWebManager.ConnectionString))
                    End If
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CType(Trim(lblField_ID.Text), Integer)

                    Dim tbleUsersAllowedForApplication As DataTable = FillDataTable(cmd, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    For Each row As DataRow In tbleUsersAllowedForApplication.Rows
                        Dim userId As Long = CType(Utils.Nz(row.Item("ID_User")), Long)
                        Dim userInfo As WMSystem.UserInformation = cammWebManager.System_GetUserInfo(userId)
                        If Not userInfo Is Nothing Then
                            Dim validationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(userInfo, requiredApplicationFlags, True)
                            For Each validationResult As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult In validationResults
                                If validationResult.ValidationResult <> CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.Success Then
                                    If Not invalidFlags.Contains(validationResult.Flag) Then
                                        invalidFlags.Add(validationResult.Flag)
                                    End If
                                End If
                            Next
                        End If
                    Next

                    If invalidFlags.Count > 0 Then
                        SetSubmitErrorText(invalidFlags)
                        Exit Sub
                    End If

                End If
                Try
                    UpdateApplication()
                    If lblErrMsg.Text = "" Then
                        Utils.RedirectTemporary(HttpContext.Current, "apps.aspx#ID" & Request.QueryString("ID"))
                    End If
                Catch ex As Exception
                    If cammWebManager.System_DebugLevel >= 3 Then
                        lblErrMsg.Text = "Application update failed! (" & ex.Message & ex.StackTrace & ")"
                    ElseIf cammWebManager.System_DebugLevel = 5 Then
                        lblErrMsg.Text = "Application update failed! (" & ex.ToString & ")"
                    Else
                        lblErrMsg.Text = "Application update failed!"
                    End If
                End Try
            Else
                lblErrMsg.Text = "Please specify the applicable location and language and the navigation information."
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to delete an application
    ''' </summary>
    Public Class ApplicationDelete
        Inherits Page

#Region "Variable Declaration"
        Protected lblField_ID, lblField_Name, lblField_Title, lblField_LocationID, lblErrMsg As Label
        Protected lblField_Level1Title, lblField_Level2Title, lblField_Level3Title, lblField_NavURL As Label
        Protected lblField_NavFrame, lblCreatedOn, lblField_ModifiedOn, lblField_Language As Label
        Protected hypCreatedBy, hypModifiedBy, hypDelete, hypDontDelete As HyperLink
        Protected tdAddLinks As Web.UI.HtmlControls.HtmlTableCell
#End Region

#Region "Page Events"
        Private Sub ApplicationDelete_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Delete")) Then
                Response.Write("No authorization to administrate this application.")
                Response.End()
            Else
                If Not IsPostBack Then
                    DeleteApplication()
                End If
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        Private Sub DeleteApplication()
            Dim Field_Language As Integer
            Dim dtDelete As New DataTable
            Dim strBlr As New Text.StringBuilder

            lblField_ID.Text = Request.QueryString("ID")
            Dim MySecurityObjectInfo As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(Utils.Nz(lblField_ID.Text, 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))

            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Try
                    'Delete application
                    Dim sqlParamsDelApp As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("ID")))}
                    'ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.Applications WHERE ID=@ID", CommandType.Text, sqlParamsDelApp, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "Update dbo.Applications Set AppDeleted=1 WHERE ID=@ID", CommandType.Text, sqlParamsDelApp, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'Delete Authorizations: users
                    Dim sqlParamsDelUser As SqlParameter() = {New SqlParameter("@ID_Application", CInt(Request.QueryString("ID")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_Application=@ID_Application", CommandType.Text, sqlParamsDelUser, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'Delete Authorizations: groups
                    Dim sqlParamsDelGroup As SqlParameter() = {New SqlParameter("@ID_Application", CInt(Request.QueryString("ID")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.ApplicationsRightsByGroup WHERE ID_Application=@ID_Application", CommandType.Text, sqlParamsDelGroup, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    'Remove any inheritions from that application
                    Dim sqlParamsDelAppInher As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("ID")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "UPDATE [dbo].[Applications_CurrentAndInactiveOnes] SET [AuthsAsAppID]=Null WHERE [AuthsAsAppID]=@ID", CommandType.Text, sqlParamsDelAppInher, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                    Utils.RedirectTemporary(HttpContext.Current, "apps.aspx#ID" & Request.QueryString("ID"))
                Catch
                    lblErrMsg.Text = "Application erasing failed!"
                End Try
            Else
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("ID")))}
                dtDelete = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.Applications WHERE ID=@ID", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtDelete Is Nothing Then
                    For Each dr As DataRow In dtDelete.Rows
                        lblField_Name.Text = Server.HtmlEncode(MySecurityObjectInfo.Name)
                        lblField_Title.Text = Server.HtmlEncode(MySecurityObjectInfo.DisplayName)
                        lblField_Level1Title.Text = Server.HtmlEncode(Utils.Nz(dr("Level1Title"), String.Empty))
                        lblField_Level2Title.Text = Server.HtmlEncode(Utils.Nz(dr("Level2Title"), String.Empty))
                        lblField_Level3Title.Text = Server.HtmlEncode(Utils.Nz(dr("Level3Title"), String.Empty))
                        lblField_NavURL.Text = Server.HtmlEncode(Utils.Nz(dr("NavURL"), String.Empty))
                        lblField_NavFrame.Text = Server.HtmlEncode(Utils.Nz(dr("NavFrame"), String.Empty))
                        lblField_LocationID.Text = Server.HtmlEncode(Utils.Nz(New camm.WebManager.WMSystem.ServerInformation(CInt(Val(dr("LocationID"))), cammWebManager).Description, String.Empty))
                        Field_Language = Utils.Nz(dr("LanguageID"), 0)
                        hypCreatedBy.NavigateUrl = ""
                        hypCreatedBy.Text = ""
                        hypDelete.NavigateUrl = "apps_delete.aspx?ID=" & Request.QueryString("ID") & "&DEL=NOW&token=" & Session.SessionID
                        hypDontDelete.NavigateUrl = "apps.aspx#ID" & Request.QueryString("ID")
                    Next
                Else
                    lblErrMsg.Text = "Application not found!"
                End If
            End If

            Try
                dtDelete = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM view_Languages WHERE IsActive = 1 ORDER BY Description", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtDelete Is Nothing Then
                    For Each dr As DataRow In dtDelete.Rows
                        If Utils.Nz(Field_Language, 0) = Utils.Nz(dr("ID"), 0) Then lblField_Language.Text = Server.HtmlEncode(Utils.Nz(dr("Description"), String.Empty))
                    Next
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            End Try

            Try
                strBlr.Append("<table cellSpacing=""0"" cellPadding=""0"" border=""0"">")
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID_Application", lblField_ID.Text)}
                dtDelete = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ItemType, ID_Group, Name, ID_User, LoginDisabled, LoginName FROM view_ApplicationRights WHERE ID_Application = @ID_Application AND ID_AppRight Is NOT Null ORDER BY ItemType, Title", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtDelete Is Nothing Then
                    For Each dr As DataRow In dtDelete.Rows
                        If Utils.Nz(dr("ItemType"), 0) = 1 Then
                            strBlr.Append("<TR><TD VAlign=""Top"" WIDTH=""160""><P><FONT face=""Arial"" size=""2"">Group</FONT></P></TD><TD VAlign=""Top"" Width=""240""><P><FONT face=""Arial"" size=""2""><a href=""groups_update.aspx?ID=" & Utils.Nz(dr("ID_Group"), 0).ToString & """>" & Server.HtmlEncode(Utils.Nz(dr("Name"), String.Empty)) & "</a></FONT></P></TD></TR>")
                        Else
#If VS2015OrHigher = True Then
#Disable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
                            strBlr.Append("<TR><TD VAlign=""Top"" WIDTH=""160""><P><FONT face=""Arial"" size=""2"">User</FONT></P></TD><TD VAlign=""Top"" Width=""240""><P><FONT face=""Arial"" size=""2""><a href=""users_update.aspx?ID=" & Utils.Nz(dr("ID_User"), 0).ToString & """>" & Server.HtmlEncode(Utils.Nz(cammWebManager.System_GetUserDetail(dr("ID_User"), "CompleteName"), String.Empty)) & " (" & Server.HtmlEncode(Utils.Nz(dr("LoginName"), String.Empty)) & ")</a>" & Utils.Nz(IIf(Utils.Nz(dr("LoginDisabled"), False), "&nbsp;<em><font color= ""#D1D1D1"">(Disabled)</font></em>", ""), String.Empty) & "</FONT></P></TD></TR>")
#If VS2015OrHigher = True Then
#Enable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
                        End If
                    Next
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            Finally
                dtDelete.Dispose()
            End Try

            strBlr.Append("</table>")
            tdAddLinks.InnerHtml = strBlr.ToString
        End Sub
#End Region

    End Class

End Namespace


