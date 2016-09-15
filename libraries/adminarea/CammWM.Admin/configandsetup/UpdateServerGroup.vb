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
    '''     A page to Update a server group
    ''' </summary>
    Public Class UpdateServerGroup
        Inherits Page

        Protected lblErrMsg, lblFieldID, lblGroupInfo As Label
        Protected _
                txtFieldServerGroup, txtAreaNavTitle, txtAreaCompanyFormerTitle, txtAreaButton, txtAreaImage, _
                txtAreaCompanyTitle, txtAreaCopyRightSinceYear, txtAreaCompanyWebSiteURL, _
                txtAreaCompanyWebSiteTitle, txtAreaSecurityContactEMail, txtAreaSecurityContactTitle, _
                txtAreaContentManagementContactEMail, txtAreaContentManagementContactTitle, txtAreaUnspecifiedContactEMail, _
                txtAreaDevelopmentContactEMail, txtAreaDevelopmentContactTitle, txtAreaUnspecifiedContactTitle _
                As TextBox
        Protected hiddenTxt_ID_Group_Public, hiddenTxt_GroupAnonymous As HtmlInputHidden
        Protected cmbUserAdminServer, cmbMasterServer, cmbAccessLevelDefault As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected hypIdGroupPublic As HyperLink
        Protected hypIdGroupAnonymous As HyperLink
        Protected CheckboxAllowImpersonationUsers As CheckBox

        Private _ServerGroup As WMSystem.ServerGroupInformation
        Protected ReadOnly Property ServerGroup As WMSystem.ServerGroupInformation
            Get
                If _ServerGroup Is Nothing Then
                    _ServerGroup = New WMSystem.ServerGroupInformation(CInt(Request.QueryString("ID")), Me.cammWebManager)
                End If
                Return _ServerGroup
            End Get
        End Property

        Private Sub UpdateServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Not Page.IsPostBack Then
                FillDropDownLists(Utils.Nz(Request.QueryString("ID"), 0))

                lblFieldID.Text = ServerGroup.ID.ToString
                txtFieldServerGroup.Text = ServerGroup.Title
                hiddenTxt_GroupAnonymous.Value = ServerGroup.GroupAnonymous.ID.ToString
                If Not lblGroupInfo Is Nothing Then lblGroupInfo.Text = Server.HtmlEncode(ServerGroup.GroupAnonymous.Name)
                If Not hypIdGroupAnonymous Is Nothing Then
                    hypIdGroupAnonymous.Text = Server.HtmlEncode(ServerGroup.GroupAnonymous.Name)
                    hypIdGroupAnonymous.NavigateUrl = "groups_update.aspx?ID=" + ServerGroup.GroupAnonymous.ID.ToString
                End If

                hiddenTxt_ID_Group_Public.Value = ServerGroup.GroupPublic.ID.ToString
                hypIdGroupPublic.Text = Server.HtmlEncode(ServerGroup.GroupPublic.Name)
                hypIdGroupPublic.NavigateUrl = "groups_update.aspx?ID=" + ServerGroup.GroupPublic.ID.ToString

                cmbMasterServer.SelectedValue = ServerGroup.MasterServer.ID.ToString
                cmbUserAdminServer.SelectedValue = ServerGroup.AdminServer.ID.ToString
                txtAreaButton.Text = ServerGroup.ImageUrlSmall
                txtAreaImage.Text = ServerGroup.ImageUrlBig
                txtAreaNavTitle.Text = ServerGroup.NavTitle
                txtAreaCompanyFormerTitle.Text = ServerGroup.CompanyFormerTitle
                txtAreaCompanyTitle.Text = ServerGroup.CompanyTitle
                txtAreaSecurityContactEMail.Text = ServerGroup.SecurityContactAddress
                txtAreaSecurityContactTitle.Text = ServerGroup.SecurityContactName
                txtAreaDevelopmentContactEMail.Text = ServerGroup.DevelopmentContactAddress
                txtAreaDevelopmentContactTitle.Text = ServerGroup.DevelopmentContactName
                txtAreaContentManagementContactEMail.Text = ServerGroup.ContentManagementContactAddress
                txtAreaContentManagementContactTitle.Text = ServerGroup.ContentManagementContactName
                txtAreaUnspecifiedContactEMail.Text = ServerGroup.UnspecifiedContactAddress
                txtAreaUnspecifiedContactTitle.Text = ServerGroup.UnspecifiedContactName
                txtAreaCopyRightSinceYear.Text = ServerGroup.CopyrightSinceYear.ToString
                txtAreaCompanyWebSiteURL.Text = ServerGroup.OfficialCompanyWebSiteURL
                txtAreaCompanyWebSiteTitle.Text = ServerGroup.OfficialCompanyWebSiteTitle
                cmbAccessLevelDefault.SelectedValue = ServerGroup.AccessLevelDefault.ID.ToString
                If Me.CheckboxAllowImpersonationUsers Is Nothing Then
                    'do nothing - just ignore this situation until lib+webscripts are both updated to minimum required version
                Else
                    Me.CheckboxAllowImpersonationUsers.Checked = ServerGroup.AllowImpersonation
                End If
            End If
        End Sub

        Private Sub FillDropDownLists(ByVal Field_ID As Integer)
            Try
                Dim dtAccess As DataTable
                dtAccess = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlCommand("SELECT [dbo].[System_AccessLevels].id, [dbo].[System_AccessLevels].title FROM [dbo].[System_AccessLevels]", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtAccess Is Nothing AndAlso dtAccess.Rows.Count > 0 Then
                    For Each dr As DataRow In dtAccess.Rows
                        cmbAccessLevelDefault.Items.Add(New ListItem(Utils.Nz(dr("Title"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If

                dtAccess.Dispose()

                Dim dtAccessLevel As DataTable
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ServerGroup", Field_ID)}
                dtAccessLevel = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SELECT ID,ServerDescription, IP  FROM System_Servers WHERE ServerGroup = @ServerGroup", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data1")

                If Not dtAccessLevel Is Nothing AndAlso dtAccessLevel.Rows.Count > 0 Then
                    For Each dr As DataRow In dtAccessLevel.Rows
                        cmbMasterServer.Items.Add(New ListItem(Utils.Nz(dr("ServerDescription"), String.Empty) + " (" + Utils.Nz(dr("IP"), String.Empty) + ")", Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If

                dtAccessLevel.Dispose()

                Dim dtSystem As DataTable
                dtSystem = FillDataTable(New SqlCommand("SELECT ID,ServerDescription, IP  FROM System_Servers WHERE Enabled <> 0", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data2")
                dtSystem.Dispose()

                If Not dtSystem Is Nothing AndAlso dtSystem.Rows.Count > 0 Then
                    For Each dr As DataRow In dtSystem.Rows
                        cmbUserAdminServer.Items.Add(New ListItem(Utils.Nz(dr("ServerDescription"), String.Empty) + " (" + Utils.Nz(dr("IP"), String.Empty) + ")", Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If lblFieldID.Text <> Nothing AndAlso
             txtFieldServerGroup.Text.Trim <> "" AndAlso
             hiddenTxt_ID_Group_Public.Value.Trim <> "" AndAlso
             hiddenTxt_GroupAnonymous.Value.Trim <> "" AndAlso
             cmbMasterServer.SelectedValue <> "" AndAlso
             cmbUserAdminServer.SelectedValue <> "" AndAlso
             txtAreaButton.Text.Trim <> "" AndAlso
             txtAreaImage.Text.Trim <> "" AndAlso
             txtAreaCompanyFormerTitle.Text.Trim <> "" AndAlso
             txtAreaCompanyTitle.Text.Trim <> "" AndAlso
             txtAreaSecurityContactEMail.Text.Trim <> "" AndAlso
             txtAreaSecurityContactTitle.Text.Trim <> "" AndAlso
             txtAreaDevelopmentContactEMail.Text.Trim <> "" AndAlso
             txtAreaDevelopmentContactTitle.Text.Trim <> "" AndAlso
             txtAreaContentManagementContactEMail.Text.Trim <> "" AndAlso
             txtAreaContentManagementContactTitle.Text.Trim <> "" AndAlso
             txtAreaUnspecifiedContactEMail.Text.Trim <> "" AndAlso
             txtAreaUnspecifiedContactTitle.Text.Trim <> "" AndAlso
             txtAreaCopyRightSinceYear.Text.Trim <> "" AndAlso
             txtAreaCompanyWebSiteURL.Text.Trim <> "" AndAlso
             txtAreaCompanyWebSiteTitle.Text.Trim <> "" AndAlso
             cmbAccessLevelDefault.SelectedValue <> "" Then

                Dim MyCmd As New SqlCommand("AdminPrivate_UpdateServerGroup", New SqlConnection(cammWebManager.ConnectionString))
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = lblFieldID.Text
                MyCmd.Parameters.Add("@ServerGroup", SqlDbType.NVarChar).Value = Mid(Trim(txtFieldServerGroup.Text.Trim), 1, 255)
                MyCmd.Parameters.Add("@ID_Group_Public", SqlDbType.Int).Value = CInt(hiddenTxt_ID_Group_Public.Value.Trim)
                MyCmd.Parameters.Add("@ID_Group_Anonymous", SqlDbType.Int).Value = CInt(hiddenTxt_GroupAnonymous.Value.Trim)
                MyCmd.Parameters.Add("@MasterServer", SqlDbType.Int).Value = CInt(cmbMasterServer.SelectedValue)
                MyCmd.Parameters.Add("@UserAdminServer", SqlDbType.Int).Value = CInt(cmbUserAdminServer.SelectedValue)
                MyCmd.Parameters.Add("@AreaImage", SqlDbType.NVarChar).Value = Trim(txtAreaImage.Text)
                MyCmd.Parameters.Add("@AreaButton", SqlDbType.NVarChar).Value = Trim(txtAreaButton.Text)
                MyCmd.Parameters.Add("@AreaNavTitle", SqlDbType.NVarChar).Value = Trim(txtAreaNavTitle.Text)
                MyCmd.Parameters.Add("@AreaCompanyFormerTitle", SqlDbType.NVarChar).Value = Trim(txtAreaCompanyFormerTitle.Text)
                MyCmd.Parameters.Add("@AreaCompanyTitle", SqlDbType.NVarChar).Value = Trim(txtAreaCompanyTitle.Text)
                MyCmd.Parameters.Add("@AreaSecurityContactEMail", SqlDbType.NVarChar).Value = Trim(txtAreaSecurityContactEMail.Text)
                MyCmd.Parameters.Add("@AreaSecurityContactTitle", SqlDbType.NVarChar).Value = Trim(txtAreaSecurityContactTitle.Text)
                MyCmd.Parameters.Add("@AreaDevelopmentContactEMail", SqlDbType.NVarChar).Value = Trim(txtAreaDevelopmentContactEMail.Text)
                MyCmd.Parameters.Add("@AreaDevelopmentContactTitle", SqlDbType.NVarChar).Value = Trim(txtAreaDevelopmentContactTitle.Text)
                MyCmd.Parameters.Add("@AreaContentManagementContactEMail", SqlDbType.NVarChar).Value = Trim(txtAreaContentManagementContactEMail.Text)
                MyCmd.Parameters.Add("@AreaContentManagementContactTitle", SqlDbType.NVarChar).Value = Trim(txtAreaContentManagementContactTitle.Text)
                MyCmd.Parameters.Add("@AreaUnspecifiedContactEMail", SqlDbType.NVarChar).Value = Trim(txtAreaUnspecifiedContactEMail.Text)
                MyCmd.Parameters.Add("@AreaUnspecifiedContactTitle", SqlDbType.NVarChar).Value = Trim(txtAreaUnspecifiedContactTitle.Text)
                MyCmd.Parameters.Add("@AreaCopyRightSinceYear", SqlDbType.Int).Value = CInt(txtAreaCopyRightSinceYear.Text)
                MyCmd.Parameters.Add("@AreaCompanyWebSiteURL", SqlDbType.NVarChar).Value = Trim(txtAreaCompanyWebSiteURL.Text)
                MyCmd.Parameters.Add("@AreaCompanyWebSiteTitle", SqlDbType.NVarChar).Value = Trim(txtAreaCompanyWebSiteTitle.Text)
                MyCmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                MyCmd.Parameters.Add("@AccessLevel_Default", SqlDbType.Int).Value = CInt(cmbAccessLevelDefault.SelectedValue)
                If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer (support introduced with this db build)
                    If Me.CheckboxAllowImpersonationUsers Is Nothing Then
                        MyCmd.Parameters.Add("@AllowImpersonationUsers", SqlDbType.Bit).Value = DBNull.Value
                    Else
                        MyCmd.Parameters.Add("@AllowImpersonationUsers", SqlDbType.Bit).Value = Me.CheckboxAllowImpersonationUsers.Checked
                    End If
                End If

                Try
                    ExecuteNonQuery(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch
                    lblErrMsg.Text = "Server group update failed!"
                End Try
            Else
                lblErrMsg.Text = "Please specify all relevant server group details to proceed."
            End If
        End Sub

    End Class

End Namespace