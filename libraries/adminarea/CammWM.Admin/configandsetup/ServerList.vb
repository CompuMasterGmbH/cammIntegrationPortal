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

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to view the list of servers
    ''' </summary>
    Public Class ServerList
        Inherits Page

#Region "Variable Declaration"
        Protected lblGroupID, lblAreaNavTitle, lblMasterServerID, lblMemberServerID2 As Label
        Protected WithEvents rptServerList, rptServerSubList As Repeater
        Protected ancGroupID, ancServerGroup, ancAdminServer, ancMasterServer, ancDeleteServerGroup, ancAdd As HtmlAnchor
        Protected ancNew, ancDeleteServer, ancMemberServerDesc As HtmlAnchor
        Protected trShowDetails, trAddBlank, trShowMsg As HtmlTableRow
        Protected gcDisabled As HtmlGenericControl
        Dim MyDt As New DataTable
        Dim FirstServerLine, ServerIsDisabled As Boolean
        Dim OldServerGroupID, NewServerGroupID As Integer
        Dim TextColorOfLine As String
        Dim CurServerGroup As Object = Nothing
#End Region

#Region "Page Events"
        Private Sub ServerList_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            BindControls()
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub BindControls()
            Try
                CurServerGroup = cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "ID_ServerGroup")
                FirstServerLine = True
                MyDt = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT AdminPrivate_ServerRelations.* FROM [AdminPrivate_ServerRelations] ORDER BY ServerGroup, MemberServer_ServerDescription, MemberServer_IP", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptServerList.DataSource = MyDt
                    rptServerList.DataBind()
                End If
            Catch ex As Exception
                Throw
            Finally
                MyDt.Dispose()
            End Try
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptServerListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptServerList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                With MyDt.Rows(e.Item.ItemIndex)
                    OldServerGroupID = NewServerGroupID
                    NewServerGroupID = Utils.Nz(.Item("ID"), 0)

                    If NewServerGroupID <> OldServerGroupID Then
                        If FirstServerLine Then
                            FirstServerLine = False
                            CType(e.Item.FindControl("trAddBlank"), HtmlTableRow).Style.Add("display", "none")
                        End If

                        CType(e.Item.FindControl("ancGroupID"), HtmlAnchor).Name = "ServerGroup" & .Item("ID").ToString
                        CType(e.Item.FindControl("lblGroupID"), Label).Text = Utils.Nz(.Item("ID"), 0).ToString
                        CType(e.Item.FindControl("ancServerGroup"), HtmlAnchor).HRef = "servers_update_group.aspx?ID=" & .Item("ID").ToString
                        CType(e.Item.FindControl("ancServerGroup"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("ServerGroup"), String.Empty))
                        CType(e.Item.FindControl("lblAreaNavTitle"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("AreaNavTitle"), String.Empty))
                        CType(e.Item.FindControl("ancAdminServer"), HtmlAnchor).HRef = "servers_update_server.aspx?ID=" & .Item("UserAdminServer_ID").ToString
                        CType(e.Item.FindControl("ancAdminServer"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("UserAdminServer_ServerDescription"), String.Empty))
                        CType(e.Item.FindControl("ancMasterServer"), HtmlAnchor).HRef = "servers_update_server.aspx?ID=" & .Item("MasterServer_ID").ToString
                        CType(e.Item.FindControl("ancMasterServer"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("MasterServer_ServerDescription"), String.Empty))
                        If Not e.Item.FindControl("lblGroupPublicName") Is Nothing Then CType(e.Item.FindControl("lblGroupPublicName"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Group_Public_Name"), String.Empty))
                        If Not e.Item.FindControl("hypGroupPublicName") Is Nothing Then
                            CType(e.Item.FindControl("hypGroupPublicName"), HyperLink).NavigateUrl = "groups_update.aspx?ID=" & (Utils.Nz(.Item("Group_Public_ID"), String.Empty))
                            CType(e.Item.FindControl("hypGroupPublicName"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Group_Public_Name"), String.Empty))
                        End If
                        If Not e.Item.FindControl("hypGroupAnonymousName") Is Nothing Then
                            CType(e.Item.FindControl("hypGroupAnonymousName"), HyperLink).NavigateUrl = "groups_update.aspx?ID=" & (Utils.Nz(.Item("Group_Anonymous_ID"), String.Empty))
                            CType(e.Item.FindControl("hypGroupAnonymousName"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Group_Anonymous_Name"), String.Empty))
                        End If
                        CType(e.Item.FindControl("ancAdd"), HtmlAnchor).HRef = "servers_new_accesslevelrelation.aspx?ID=" & .Item("ID").ToString

                        If CLng(CurServerGroup.ToString) <> CLng(.Item("ID").ToString) Then
                            CType(e.Item.FindControl("ancDeleteServerGroup"), HtmlAnchor).HRef = "servers_delete_group.aspx?ID=" & CLng(.Item("ID").ToString)
                            CType(e.Item.FindControl("ancDeleteServerGroup"), HtmlAnchor).InnerHtml = "Delete Server Group"
                        End If

                        'bind inner repeater control
                        Dim dt As New DataTable
                        Dim RecsFound As Boolean

                        Try
                            Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(.Item("ID").ToString))}
                            dt = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM AdminPrivate_ServerGroupAccessLevels WHERE ID_ServerGroup = @ID ORDER BY AccessLevels_Title", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                                CType(e.Item.FindControl("rptServerSubList"), Repeater).DataSource = dt
                                CType(e.Item.FindControl("rptServerSubList"), Repeater).DataBind()
                                RecsFound = True
                            End If
                        Catch ex As Exception
                            Throw
                        Finally
                            dt.Dispose()
                        End Try

                        If RecsFound = False Then CType(e.Item.FindControl("trShowMsg"), HtmlTableRow).Style.Add("display", "")
                    Else
                        CType(e.Item.FindControl("trShowDetails"), HtmlTableRow).Style.Add("display", "none")
                    End If

                    ServerIsDisabled = False
                    If Not IsDBNull(.Item("MemberServer_Enabled")) Then If Utils.Nz(.Item("MemberServer_Enabled"), False) = False Then ServerIsDisabled = True
                    If ServerIsDisabled = True Then TextColorOfLine = "gray" Else TextColorOfLine = "black"

                    CType(e.Item.FindControl("lblMasterServerID"), Label).Style.Add("color", TextColorOfLine)
                    CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).Style.Add("color", TextColorOfLine)
                    CType(e.Item.FindControl("lblMemberServerID2"), Label).Style.Add("color", TextColorOfLine)

                    CType(e.Item.FindControl("ancNew"), HtmlAnchor).HRef = "servers_new_server.aspx?ID=" & .Item("ID").ToString
                    CType(e.Item.FindControl("lblMasterServerID"), Label).Text = .Item("MemberServer_ID").ToString
                    If ServerIsDisabled Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).InnerHtml = "<nobr title=""Disabled"">(D)</nobr>"

                    CType(e.Item.FindControl("ancMemberServerDesc"), HtmlAnchor).HRef = "servers_update_server.aspx?ID=" & .Item("MemberServer_ID").ToString
                    CType(e.Item.FindControl("ancMemberServerDesc"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("MemberServer_ServerDescription"), String.Empty))
                    CType(e.Item.FindControl("lblMemberServerID2"), Label).Text = .Item("MemberServer_IP").ToString

                    If .Item("MasterServer_ID").ToString <> .Item("MemberServer_ID").ToString And .Item("UserAdminServer_ID").ToString <> .Item("MemberServer_ID").ToString Then
                        CType(e.Item.FindControl("ancDeleteServer"), HtmlAnchor).HRef = "servers_delete_server.aspx?ID=" & .Item("MemberServer_ID").ToString
                        CType(e.Item.FindControl("ancDeleteServer"), HtmlAnchor).InnerHtml = "Delete Server"
                    End If
                End With
            End If
        End Sub
#End Region

    End Class

End Namespace