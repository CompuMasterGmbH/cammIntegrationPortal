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
    '''     A page to delete a Server Lveel relation.
    ''' </summary>
    Public Class Delete_ServersAccesslevelrelation
        Inherits Page

#Region "Variable Declaration"
        Protected hypDelete As HyperLink
        Protected lblAccessLevel, lblServerGroup, lblRelationId, lblErrMsg As Label
#End Region

#Region "Page Events"
        Private Sub Delete_ServersAccesslevelrelation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM System_ServerGroupsAndTheirUserAccessLevels WHERE ID = @ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch
                    lblErrMsg.Text = "Removing of access level relation failed! "
                End Try
            Else
                Dim dtServerDetail As DataTable = Nothing

                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    dtServerDetail = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT  dbo.System_ServerGroupsAndTheirUserAccessLevels.ID, dbo.System_AccessLevels.Title, dbo.System_ServerGroups.ServerGroup FROM dbo.System_ServerGroupsAndTheirUserAccessLevels LEFT OUTER JOIN dbo.System_AccessLevels ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = dbo.System_AccessLevels.ID LEFT OUTER JOIN dbo.System_ServerGroups ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID WHERE dbo.System_ServerGroupsAndTheirUserAccessLevels.ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    If dtServerDetail Is Nothing Then
                        lblErrMsg.Text = "Access level relation not found!"
                    Else
                        lblRelationId.Text = Request.QueryString("ID")
                        lblAccessLevel.Text = Server.HtmlEncode(Utils.Nz(dtServerDetail.Rows(0)("Title"), String.Empty))
                        lblServerGroup.Text = Server.HtmlEncode(Utils.Nz(dtServerDetail.Rows(0)("ServerGroup"), String.Empty))
                        hypDelete.NavigateUrl = "servers_delete_accesslevelrelation.aspx?ID=" + Request.QueryString("ID") + "&DEL=NOW&token=" & Session.SessionID
                        hypDelete.Text = "Yes, delete it!"
                    End If
                Catch ex As Exception
                    Throw
                Finally
                    dtServerDetail.Dispose()
                End Try
            End If
        End Sub
#End Region

    End Class

End Namespace