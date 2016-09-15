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

        Private Sub UpdateServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ID_Group_Public As Integer
            Dim Field_ID_Group_Anonymous As Integer

            If Not Page.IsPostBack Then
                FillDropDownLists(Utils.Nz(Request.QueryString("ID"), 0))
                Dim dtServerGroup As DataTable

                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    dtServerGroup = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                                     "SELECT  top 1 * FROM dbo.System_ServerGroups WHERE ID = @ID", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    lblFieldID.Text = Utils.Nz(dtServerGroup.Rows(0)("ID"), 0).ToString
                    txtFieldServerGroup.Text = Utils.Nz(dtServerGroup.Rows(0)("ServerGroup"), String.Empty)
                    Field_ID_Group_Public = Utils.Nz(dtServerGroup.Rows(0)("ID_Group_Public"), 0)
                    Field_ID_Group_Anonymous = Utils.Nz(dtServerGroup.Rows(0)("ID_Group_Anonymous"), 0)
                    Dim MyAnonymousGroupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(Field_ID_Group_Anonymous, CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                    hiddenTxt_GroupAnonymous.Value = Utils.Nz(MyAnonymousGroupInfo.ID, 0).ToString
                    If Not lblGroupInfo Is Nothing Then lblGroupInfo.Text = Server.HtmlEncode(MyAnonymousGroupInfo.Name)
                    If Not hypIdGroupAnonymous Is Nothing Then
                        hypIdGroupAnonymous.Text = Server.HtmlEncode(MyAnonymousGroupInfo.Name)
                        hypIdGroupAnonymous.NavigateUrl = "groups_update.aspx?ID=" + MyAnonymousGroupInfo.ID.ToString
                    End If

                    Dim MyPublicGroupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(Field_ID_Group_Public, CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                    hiddenTxt_ID_Group_Public.Value = Utils.Nz(MyPublicGroupInfo.ID, 0).ToString
                    hypIdGroupPublic.Text = Server.HtmlEncode(MyPublicGroupInfo.Name)
                    hypIdGroupPublic.NavigateUrl = "groups_update.aspx?ID=" + MyPublicGroupInfo.ID.ToString

                    cmbMasterServer.SelectedValue = Utils.Nz(dtServerGroup.Rows(0)("MasterServer"), String.Empty)
                    cmbUserAdminServer.SelectedValue = Utils.Nz(dtServerGroup.Rows(0)("UserAdminServer"), String.Empty)
                    txtAreaButton.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaButton"), String.Empty)
                    txtAreaImage.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaImage"), String.Empty)
                    txtAreaNavTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaNavTitle"), String.Empty)
                    txtAreaCompanyFormerTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyFormerTitle"), String.Empty)
                    txtAreaCompanyTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyTitle"), String.Empty)
                    txtAreaSecurityContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaSecurityContactEMail"), String.Empty)
                    txtAreaSecurityContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaSecurityContactTitle"), String.Empty)
                    txtAreaDevelopmentContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaDevelopmentContactEMail"), String.Empty)
                    txtAreaDevelopmentContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaDevelopmentContactTitle"), String.Empty)
                    txtAreaContentManagementContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaContentManagementContactEMail"), String.Empty)
                    txtAreaContentManagementContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaContentManagementContactTitle"), String.Empty)
                    txtAreaUnspecifiedContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaUnspecifiedContactEMail"), String.Empty)
                    txtAreaUnspecifiedContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaUnspecifiedContactTitle"), String.Empty)
                    txtAreaCopyRightSinceYear.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCopyRightSinceYear"), String.Empty)
                    txtAreaCompanyWebSiteURL.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyWebSiteURL"), String.Empty)
                    txtAreaCompanyWebSiteTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyWebSiteTitle"), String.Empty)
                    cmbAccessLevelDefault.SelectedValue = Utils.Nz(dtServerGroup.Rows(0)("AccessLevel_Default"), String.Empty)
                    If Me.CheckboxAllowImpersonationUsers Is Nothing Then
                        'do nothing - just ignore this situation until lib+webscripts are both updated to minimum required version
                    ElseIf dtServerGroup.Columns.Contains("AllowImpersonation") Then
                        Me.CheckboxAllowImpersonationUsers.Checked = Utils.Nz(dtServerGroup.Rows(0)("AccessLevel_Default"), False)
                    Else
                        Me.CheckboxAllowImpersonationUsers.Checked = False
                    End If
                Catch ex As Exception
                    Throw
                End Try
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