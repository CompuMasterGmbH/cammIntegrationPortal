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
                dt = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ServerGroup,id FROM System_ServerGroups ORDER BY ServerGroup", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                cmbServerGroup.Items.Clear()
                cmbServerGroup.Items.Insert(0, New ListItem("", ""))
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        cmbServerGroup.Items.Add(New ListItem(Utils.Nz(dr("ServerGroup"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If

                'for Market
                dt = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
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

            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), _
                New SqlParameter("@TitleAdminArea", txtApplication.Text.ToString.Trim.Replace(") '", "''").Replace("*", "%") & "%"), _
                New SqlParameter("@ServerGroup", cmbServerGroup.SelectedValue), _
                New SqlParameter("@LanguageID", cmbMarket.SelectedValue), _
                New SqlParameter("@ApplicationText", "%" & txtApplication.Text.Trim.Replace("'", "''").Replace("*", "%") & "%"), _
                New SqlParameter("@NavUrl", "%" & txtApplication.Text.Trim.Replace("'", "''").Replace("*", "%") & "%")}
            strWHERE.Append(" (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('SecurityMaster')) OR view_applications.id in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('Update','Owner')))")
            Dim strQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
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
                    If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CLng(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CLng(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CLng(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CLng(.Item("ReleasedByID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CLng(.Item("ReleasedByID")) Then
                        CType(e.Item.FindControl("hlnReleasedByLastName"), HyperLink).Text = Server.HtmlEncode(Me.SafeLookupUserFullName(CType(.Item("ReleasedByID"), Int64)))
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

End Namespace