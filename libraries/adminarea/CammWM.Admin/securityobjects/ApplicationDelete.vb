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
    '''     A page to delete an application
    ''' </summary>
    Public Class ApplicationDelete
        Inherits ApplicationBasePage

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
                dtDelete = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
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
                dtDelete = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                                    "SELECT * FROM view_Languages WHERE IsActive = 1 ORDER BY Description", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtDelete Is Nothing Then
                    For Each dr As DataRow In dtDelete.Rows
                        If Utils.Nz(Field_Language, 0) = Utils.Nz(dr("ID"), 0) Then lblField_Language.Text = Server.HtmlEncode(Utils.Nz(dr("Description"), String.Empty))
                    Next
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            End Try

            tdAddLinks.InnerHtml = Me.RenderAuthorizations(MySecurityObjectInfo.ID)
        End Sub
#End Region

    End Class

End Namespace