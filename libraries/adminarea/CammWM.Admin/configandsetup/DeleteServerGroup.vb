﻿'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
    '''     A page to delete a server group
    ''' </summary>
    Public Class DeleteServerGroup
        Inherits Page

#Region "Variable Declaration"
        Protected lblServerGroupName, lblServerGroupId, lblErrMsg As Label
#End Region

#Region "Page Events"
        Private Sub DeleteServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                    'Older / without distributed deletion of foreign key table rows
                    lblErrMsg.Text = "Server group erasing failed! (database build too old, please update first)"
                Else
                    'Newer / with distributed deletion of foreign key table rows
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID_ServerGroup", CLng(Request.QueryString("ID")))}
                    Try
                        CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteServerGroup", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                        Response.Redirect("servers.aspx")
                    Catch ex As Exception
                        lblErrMsg.Text = "Server group erasing failed! (" & Server.HtmlEncode(ex.Message) & ")"
                    End Try
                End If
            Else
                Dim MyServerGroupInfo As New CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                lblServerGroupId.Text = Server.HtmlEncode(Request.QueryString("ID"))
                lblServerGroupName.Text = Server.HtmlEncode(MyServerGroupInfo.Title)
            End If
        End Sub
#End Region

    End Class

End Namespace