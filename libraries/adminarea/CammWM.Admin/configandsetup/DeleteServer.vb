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
    '''     A page to delete server
    ''' </summary>
    Public Class DeleteServer
        Inherits Page

#Region "Variable Declaration"
        Protected lblServer, lblIP, lblDescription, lblAddress, lblErrMsg As Label
        Protected hypDelete As HyperLink
#End Region

#Region "Page Events"
        Private Sub DeleteServer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ServerIP As String
            Dim Field_ServerDescription As String
            Dim Field_ServerAddress As String
            If Request.QueryString("ID") <> "" And Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                    'Older / without distributed deletion of foreign key table rows
                    lblErrMsg.Text = "Server group erasing failed! (database build too old, please update first)"
                Else
                    'Newer / with distributed deletion of foreign key table rows
                    Dim sqlParam As SqlParameter() = {New SqlParameter("@ServerID", CLng(Request.QueryString("ID")))}
                    Try
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteServer", CommandType.StoredProcedure, sqlParam, Automations.AutoOpenAndCloseAndDisposeConnection)
                        Response.Redirect("servers.aspx")
                    Catch
                        lblErrMsg.Text = "Removing of server failed! "
                    End Try
                End If
            Else
                Dim MyServerInfo As New CompuMaster.camm.WebManager.WMSystem.ServerInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                Field_ServerIP = MyServerInfo.IPAddressOrHostHeader
                Field_ServerDescription = MyServerInfo.Description
                Field_ServerAddress = MyServerInfo.ServerURL
                lblServer.Text = Server.HtmlEncode(Utils.Nz(Request.QueryString("ID"), 0).ToString)
                lblIP.Text = Server.HtmlEncode(Utils.Nz(Field_ServerIP, String.Empty))
                lblDescription.Text = Server.HtmlEncode(Utils.Nz(Field_ServerDescription, String.Empty))
                lblAddress.Text = Server.HtmlEncode(Utils.Nz(Field_ServerAddress, String.Empty))
                hypDelete.Text = "Yes, delete it!"
                hypDelete.NavigateUrl = "servers_delete_server.aspx?ID=" + Server.HtmlEncode(Request.QueryString("ID")) + "&DEL=NOW&token=" & Session.SessionID
            End If
        End Sub
#End Region

    End Class

End Namespace