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
    '''     A page to update a group
    ''' </summary>
    Public Class GroupUpdate
        Inherits Page

#Region "Variable Decalration"
        Protected lblCreationDate, lblModificationDate, lblErrMsg, lblGroupName As Label
        Protected txtGroupName, txtDescription, txtBccMail As TextBox
        Protected hypCreatedBy, hypModifiedBy As HyperLink
        Protected cammWebManagerAdminGroupInfoDetails As CompuMaster.camm.WebManager.Controls.Administration.GroupsAdditionalInformation
        Protected cammWebManagerAdminDelegates As CompuMaster.camm.WebManager.Controls.Administration.AdministrativeDelegates

        Protected trUpdateGroup, trMemberList, trMemberList2, trMemberList3 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents btnSubmit As Button

        Protected ViewOnlyMode As Boolean = False

#End Region

#Region "Page Events"
        Private Sub GroupUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim MyGroupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                cammWebManagerAdminGroupInfoDetails.MyGroupInfo = MyGroupInfo
                cammWebManagerAdminDelegates.GroupInfo = MyGroupInfo

                If CInt(Val(Request.QueryString("view") & "")) = 1 OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Update, CInt(Request.QueryString("ID"))) = False Then
                    btnSubmit.Enabled = False
                    btnSubmit.Visible = False
                End If

                'Added for setting Navigation Preview link
                If (Not Request.QueryString("IsAnonymousOrPublic") Is Nothing AndAlso CInt(Request.QueryString("IsAnonymousOrPublic")) = 0) Then
                    CType(Me.FindControl("cammWebManagerAdminGroupInfoDetails").FindControl("ancPreview"), HtmlAnchor).HRef = "users_navbar_preview.aspx?" + "GroupName=" + MyGroupInfo.Name + "&GroupId=" + MyGroupInfo.ID.ToString
                    CType(Me.FindControl("cammWebManagerAdminGroupInfoDetails").FindControl("ancPreview"), HtmlAnchor).InnerHtml = "Navigation Preview"
                End If


                If Not (Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Owner, CInt(Request("ID"))) OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Update, CInt(Request("ID")))) AndAlso Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.View, CInt(Request("ID"))) Then
                    ViewOnlyMode = True
                End If

                If Not (Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Owner, CInt(Request("ID"))) OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Update, CInt(Request("ID"))) OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.View, CInt(Request("ID")))) Then
                    Response.Clear()
                    Response.Write("No authorization to administrate this group.")
                    Response.End()
                ElseIf txtGroupName.Text.Trim = "" And txtDescription.Text.Trim = "" Then
                    Dim Mydt As DataTable
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                    Mydt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Name, Description FROM dbo.Gruppen WHERE ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                    If Mydt Is Nothing Then
                        lblErrMsg.Text = "Group not found!"
                    Else
                        txtGroupName.Text = Utils.Nz(Mydt.Rows(0)("Name"), String.Empty)
                        lblGroupName.Text = Server.HtmlEncode(Utils.Nz(Mydt.Rows(0)("Name"), String.Empty))
                        txtDescription.Text = Utils.Nz(Mydt.Rows(0)("description"), String.Empty)
                    End If
                End If

                If Not Page.IsPostBack Then
                    Dim dtGroupInfo As DataTable
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                    dtGroupInfo = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_groups WHERE ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                    Dim grpInfo As New WMSystem.GroupInformation(CInt(Val(Request.QueryString("ID"))), Me.cammWebManager)
                    If dtGroupInfo Is Nothing OrElse grpInfo.ID = 0 Then
                        lblErrMsg.Text = "Group not found!"
                    Else
                        'if group is special like Supervisor etc. then groupname is not editable
                        If grpInfo.IsSystemGroupByServerGroup Then
                            trMemberList.Style.Add("display", "none")
                            trMemberList2.Style.Add("display", "none")
                            trMemberList3.Style.Add("display", "none")
                        End If
                        txtGroupName.Visible = True
                        lblGroupName.Visible = False

                        lblCreationDate.Text = Server.HtmlEncode(Utils.Nz(dtGroupInfo.Rows(0)("ReleasedOn"), String.Empty))
                        lblModificationDate.Text = Server.HtmlEncode(Utils.Nz(dtGroupInfo.Rows(0)("ModifiedOn"), String.Empty))
                        hypCreatedBy.NavigateUrl = "users_update.aspx?ID=" + Utils.Nz(dtGroupInfo.Rows(0)("ReleasedByID"), 0).ToString
                        hypCreatedBy.Text = Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(dtGroupInfo.Rows(0)("ReleasedByID"))))
                        hypModifiedBy.NavigateUrl = "users_update.aspx?ID=" + Utils.Nz(dtGroupInfo.Rows(0)("ModifiedByID"), 0).ToString
                        hypModifiedBy.Text = Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(dtGroupInfo.Rows(0)("ModifiedByID"))))
                    End If

                    Dim MyCmd As New SqlClient.SqlCommand("SELECT [E-MAIL] FROM dbo.view_eMailAccounts_of_Groups WHERE ID_Group=@ID", New SqlConnection(cammWebManager.ConnectionString))
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = CInt(Val(Request.QueryString("ID") & ""))

                    Dim EMailAddressesOfMembers As System.Collections.Generic.List(Of String)
                    EMailAddressesOfMembers = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoGenericList(Of String)(mycmd, Automations.AutoOpenAndCloseAndDisposeConnection)
                    txtBccMail.Text = Strings.Join(EMailAddressesOfMembers.ToArray, "; ")
                    If ViewOnlyMode = False Then trUpdateGroup.Visible = True
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

#Region " Control Events "
        Protected Overridable Sub btnSubmitClcik(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            Try
                If (ViewOnlyMode = False And Request.QueryString("ID") <> "" And txtGroupName.Text.Trim <> "") Then
                    Dim CurUserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                    Try
                        Dim sqlParams As SqlParameter() = { _
                            New SqlParameter("@Name", Replace(txtGroupName.Text.Trim, "'", "''")), _
                            New SqlParameter("@Description", txtDescription.Text.Trim.Replace("'", "''")), _
                            New SqlParameter("@ModifiedBy", CurUserID), _
                            New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                        Dim mySqlQuery As String = "UPDATE dbo.Gruppen SET Name=@Name,Description=@Description,ModifiedOn=GetDate(),ModifiedBy=@ModifiedBy WHERE ID=@ID"
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), mySqlQuery, CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                        Response.Redirect("groups.aspx")
                    Catch ex As Exception
                        lblErrMsg.Text = "Group update failed!"
                    End Try
                Else
                    lblErrMsg.Text = "Please specify the field ""Group name"" to proceed!"
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

    End Class

End Namespace