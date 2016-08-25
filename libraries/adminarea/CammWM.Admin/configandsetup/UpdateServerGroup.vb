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

#Region "Variable Declaration"
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
#End Region

#Region "Page Events"
        Private Sub UpdateServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ID_Group_Public As Integer
            Dim Field_ID_Group_Anonymous As Integer

            If Not Page.IsPostBack Then
                FillDropDownLists(Utils.Nz(Request.QueryString("ID"), 0))
                Dim dtServerGroup As DataTable

                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    dtServerGroup = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
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
                Catch ex As Exception
                    Throw
                End Try
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
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
#End Region

#Region "Control Events"
        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If lblFieldID.Text <> Nothing AndAlso _
             txtFieldServerGroup.Text.Trim <> "" AndAlso _
             hiddenTxt_ID_Group_Public.Value.Trim <> "" AndAlso _
             hiddenTxt_GroupAnonymous.Value.Trim <> "" AndAlso _
             cmbMasterServer.SelectedValue <> "" AndAlso _
             cmbUserAdminServer.SelectedValue <> "" AndAlso _
             txtAreaButton.Text.Trim <> "" AndAlso _
             txtAreaImage.Text.Trim <> "" AndAlso _
              txtAreaCompanyFormerTitle.Text.Trim <> "" AndAlso _
             txtAreaCompanyTitle.Text.Trim <> "" AndAlso _
             txtAreaSecurityContactEMail.Text.Trim <> "" AndAlso _
             txtAreaSecurityContactTitle.Text.Trim <> "" AndAlso _
             txtAreaDevelopmentContactEMail.Text.Trim <> "" AndAlso _
             txtAreaDevelopmentContactTitle.Text.Trim <> "" AndAlso _
             txtAreaContentManagementContactEMail.Text.Trim <> "" AndAlso _
             txtAreaContentManagementContactTitle.Text.Trim <> "" AndAlso _
             txtAreaUnspecifiedContactEMail.Text.Trim <> "" AndAlso _
             txtAreaUnspecifiedContactTitle.Text.Trim <> "" AndAlso _
             txtAreaCopyRightSinceYear.Text.Trim <> "" AndAlso _
             txtAreaCompanyWebSiteURL.Text.Trim <> "" AndAlso _
             txtAreaCompanyWebSiteTitle.Text.Trim <> "" AndAlso _
             cmbAccessLevelDefault.SelectedValue <> "" Then

                Dim sqlParams As SqlParameter() = { _
                                                            New SqlParameter("@ID", lblFieldID.Text), _
                                                            New SqlParameter("@ServerGroup", Mid(Trim(txtFieldServerGroup.Text.Trim), 1, 255)), _
                                                            New SqlParameter("@ID_Group_Public", CInt(hiddenTxt_ID_Group_Public.Value.Trim)), _
                                                            New SqlParameter("@ID_Group_Anonymous", CInt(hiddenTxt_GroupAnonymous.Value.Trim)), _
                                                            New SqlParameter("@MasterServer", CInt(cmbMasterServer.SelectedValue)), _
                                                            New SqlParameter("@UserAdminServer", CInt(cmbUserAdminServer.SelectedValue)), _
                                                            New SqlParameter("@AreaImage", Mid(Trim(txtAreaImage.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaButton", Mid(Trim(txtAreaButton.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaNavTitle", Mid(Trim(txtAreaNavTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCompanyFormerTitle", Mid(Trim(txtAreaCompanyFormerTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCompanyTitle", Mid(Trim(txtAreaCompanyTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaSecurityContactEMail", Mid(Trim(txtAreaSecurityContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaSecurityContactTitle", Mid(Trim(txtAreaSecurityContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaDevelopmentContactEMail", Mid(Trim(txtAreaDevelopmentContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaDevelopmentContactTitle", Mid(Trim(txtAreaDevelopmentContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaContentManagementContactEMail", Mid(Trim(txtAreaContentManagementContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaContentManagementContactTitle", Mid(Trim(txtAreaContentManagementContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaUnspecifiedContactEMail", Mid(Trim(txtAreaUnspecifiedContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaUnspecifiedContactTitle", Mid(Trim(txtAreaUnspecifiedContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCopyRightSinceYear", CInt(txtAreaCopyRightSinceYear.Text.Trim)), _
                                                            New SqlParameter("@AreaCompanyWebSiteURL", Mid(Trim(txtAreaCompanyWebSiteURL.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCompanyWebSiteTitle", Mid(Trim(txtAreaCompanyWebSiteTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@ModifiedBy", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), _
                                                            New SqlParameter("@AccessLevel_Default", CInt(cmbAccessLevelDefault.SelectedValue))}

                Try
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateServerGroup", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch
                    lblErrMsg.Text = "Server group update failed!"
                End Try
            Else
                lblErrMsg.Text = "Please specify all relevant server group details to proceed."
            End If
        End Sub
#End Region

    End Class

End Namespace