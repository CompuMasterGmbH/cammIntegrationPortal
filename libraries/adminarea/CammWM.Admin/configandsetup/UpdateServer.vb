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
    '''     A page to update a server
    ''' </summary>
    Public Class UpdateServer
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblGroupID As Label
        Protected txtHostHeader, txtDescription, txtPortNumber, txtServerName, txtProtocal As TextBox
        Protected cmbServerGroup, cmbEnabled As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected trEnableCombo, trEnabledMsg, trNotMasterServer, trMasterServer As HtmlTableRow
        Protected WithEvents rptEngine As Repeater
        Protected hypServerURl As HyperLink
        Private srtEngineDetail As SortedList
#End Region

#Region "Page Events"
        Private Sub UpdateServer_Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim i As Integer = rptEngine.Items.Count

            If (srtEngineDetail Is Nothing) Then
                srtEngineDetail = New SortedList
                For Each item As RepeaterItem In rptEngine.Items
                    Select Case item.ItemType
                        Case ListItemType.AlternatingItem, ListItemType.Item
                            Dim strEngineId As String = CType(item.FindControl("EngineId"), HtmlInputHidden).Value
                            Dim cmbEngine As DropDownList = CType(item.FindControl("cmbEngine"), DropDownList)
                            srtEngineDetail.Add(Server.HtmlEncode(strEngineId), cmbEngine.SelectedValue)
                    End Select
                Next
            End If

            If Not Page.IsPostBack Then
                Dim strWebURL As String = Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) & "?" & Utils.QueryStringWithoutSpecifiedParameters(Nothing)

                Dim Field_ID As Integer
                Dim Field_IsAdminServer As Boolean
                Dim Field_IsMasterServer As Boolean
                Dim Field_AddrName As String

                Dim dtServer As DataTable
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                dtServer = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT top 1 dbo.System_Servers.*, Case When System_ServerGroups_1.MasterServer Is Not Null Then 1 Else 0 End As IsMasterServer, Case When dbo.System_ServerGroups.UserAdminServer Is Not Null Then 1 Else 0 End As IsAdminServer FROM dbo.System_Servers LEFT OUTER JOIN dbo.System_ServerGroups ON dbo.System_Servers.ID = dbo.System_ServerGroups.UserAdminServer LEFT OUTER JOIN dbo.System_ServerGroups System_ServerGroups_1 ON dbo.System_Servers.ID = System_ServerGroups_1.MasterServer WHERE dbo.System_Servers.ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                If Not dtServer Is Nothing Then
                    Field_ID = Utils.Nz(Request.QueryString("ID"), 0)
                    lblGroupID.Text = Utils.Nz(Field_ID, 0).ToString
                    Field_IsMasterServer = Utils.Nz(dtServer.Rows(0)("IsMasterServer"), False)

                    If (Utils.Nz(Field_IsMasterServer, False) = False) Then
                        trMasterServer.Visible = True
                        trNotMasterServer.Visible = False
                        FillServerGroupCombo()
                        cmbServerGroup.SelectedValue = Utils.Nz(dtServer.Rows(0)("ServerGroup"), String.Empty)
                    Else
                        trMasterServer.Visible = False
                        trNotMasterServer.Visible = True
                        FillServerGroupCombo()
                        cmbServerGroup.SelectedValue = Utils.Nz(dtServer.Rows(0)("ServerGroup"), String.Empty)
                    End If

                    Field_IsAdminServer = Utils.Nz(dtServer.Rows(0)("IsAdminServer"), False)
                    FillEnabledCombo(cmbEnabled)

                    If CLng(cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "ID")) = CLng(Field_ID) And Utils.Nz(Field_IsAdminServer, False) <> False Then
                        trEnabledMsg.Visible = True
                        trEnableCombo.Visible = False
                        cmbEnabled.SelectedValue = "1"
                    Else
                        trEnableCombo.Visible = True
                        trEnabledMsg.Visible = False

                        If Utils.Nz(dtServer.Rows(0)("Enabled"), False) Then
                            cmbEnabled.SelectedValue = "1"
                        Else
                            cmbEnabled.SelectedValue = "0"
                        End If
                    End If

                    txtHostHeader.Text = Utils.Nz(dtServer.Rows(0)("IP"), String.Empty)
                    txtDescription.Text = Utils.Nz(dtServer.Rows(0)("ServerDescription"), String.Empty)
                    txtProtocal.Text = Utils.Nz(dtServer.Rows(0)("ServerProtocol"), String.Empty)
                    Field_AddrName = Utils.Nz(dtServer.Rows(0)("ServerName"), String.Empty)
                    txtServerName.Text = Utils.Nz(dtServer.Rows(0)("ServerName"), String.Empty)
                    txtPortNumber.Text = Utils.Nz(dtServer.Rows(0)("ServerPort"), String.Empty)
                    hypServerURl.Text = "event log"
                    hypServerURl.NavigateUrl = GetServerURL(Field_ID) + "/sysdata/servereventlog.aspx"
                    Me.GlobalConfig.WriteConfigRecord(New WebManager.Administration.GlobalConfiguration.ConfigRecord("ServerCheck_TimeStamp", Me.CurrentDatabaseDateTime))
                Else
                    lblErrMsg.Text = "Server not found"
                End If
            End If
        End Sub

        Private Sub UpdateServer_Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            BindDataList()
        End Sub
#End Region

#Region "User-Defined functions"
        Private Sub FillEnabledCombo(ByRef cmbEngine As DropDownList)
            cmbEngine.Items.Add(New ListItem("Yes", "1"))
            cmbEngine.Items.Add(New ListItem("No", "0"))
        End Sub

        Private Sub FillServerGroupCombo()
            Dim dtServerGroup As DataTable
            cmbServerGroup.Items.Clear()
            dtServerGroup = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM System_ServerGroups", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If Not dtServerGroup Is Nothing AndAlso dtServerGroup.Rows.Count > 0 Then
                For Each dr As DataRow In dtServerGroup.Rows
                    cmbServerGroup.Items.Add(New ListItem(Utils.Nz(dr("ServerGroup"), String.Empty), dr("ID").ToString))
                Next
            End If

            'cmbServerGroup.DataSource = dtServerGroup
            'cmbServerGroup.DataTextField = "ServerGroup"
            'cmbServerGroup.DataValueField = "ID"
        End Sub

        Private Function GetServerURL(ByVal ServerID As Integer) As String
            Dim ServerInfo As New CompuMaster.camm.WebManager.WMSystem.ServerInformation(ServerID, CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
            Return ServerInfo.ServerURL
        End Function

        Private Sub BindDataList()
            Dim sqlParamsScript As SqlParameter() = {New SqlParameter("@ServerID", CLng(Request.QueryString("ID")))}
            rptEngine.DataSource = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_GetScriptEnginesOfServer", CommandType.StoredProcedure, sqlParamsScript, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            rptEngine.DataBind()
        End Sub
#End Region

#Region "Control Events"
        Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            lblErrMsg.Text = ""
            If Request.QueryString("ID") <> Nothing And txtHostHeader.Text.Trim <> "" And txtDescription.Text.Trim <> "" And cmbServerGroup.SelectedValue <> Nothing And txtProtocal.Text.Trim <> "" And txtServerName.Text.Trim <> "" Then

                Dim atLeastOneScriptEngineActivated As Boolean = False
                For myScriptEngineCounter As Integer = 0 To srtEngineDetail.Count - 1
                    If CType(srtEngineDetail.GetByIndex(myScriptEngineCounter), Integer) = 1 Then
                        atLeastOneScriptEngineActivated = True
                        Exit For
                    End If
                Next
                If Not atLeastOneScriptEngineActivated Then
                    lblErrMsg.Text = "No script engine selected. Standard script engine was activated now."
                End If

                Dim MyRec As Object

                Try
                    If (txtPortNumber.Text.Trim <> "") Then
                        If Not IsNumeric(txtPortNumber.Text.Trim) Then
                            lblErrMsg.Text = "Port Number should be numeric"
                            Exit Sub
                        End If
                    End If
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@Enabled", CBool(cmbEnabled.SelectedValue)), _
                                                   New SqlParameter("@IP", Mid(Trim(txtHostHeader.Text), 1, 256)), _
                                                   New SqlParameter("@ServerDescription", Mid(Trim(txtDescription.Text), 1, 200)), _
                                                   New SqlParameter("@ServerGroup", CLng(cmbServerGroup.SelectedValue)), _
                                                   New SqlParameter("@ServerProtocol", Mid(Trim(txtProtocal.Text), 1, 200)), _
                                                   New SqlParameter("@ServerName", Mid(Trim(txtServerName.Text), 1, 200)), _
                                                   New SqlParameter("@ServerPort", IIf(txtPortNumber.Text.Trim = "", DBNull.Value, txtPortNumber.Text.Trim)), _
                                                   New SqlParameter("@ID", Request.QueryString("ID"))}

                    MyRec = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateServer", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)

                    Dim i As Integer
                    For i = 0 To srtEngineDetail.Count - 1
                        Dim sqlParam As SqlParameter() = { _
                                                           New SqlParameter("@ScriptEngineID", srtEngineDetail.GetKey(i)), _
                                                           New SqlParameter("@ServerID", Request.QueryString("ID")), _
                                                           New SqlParameter("@Enabled", srtEngineDetail.GetByIndex(i))}
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetScriptEngineActivation", CommandType.StoredProcedure, sqlParam, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Next


                    srtEngineDetail = Nothing
                    Dim sqlParamsScript As SqlParameter() = { _
                                                   New SqlParameter("@ScriptEngineID", "0"), _
                                                   New SqlParameter("@ServerID", Request.QueryString("ID")), _
                                                   New SqlParameter("@Enabled", False), _
                                                   New SqlParameter("@CheckMinimalActivations", True)}

                    Try
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetScriptEngineActivation", CommandType.StoredProcedure, sqlParamsScript, Automations.AutoOpenAndCloseAndDisposeConnection)
                        If lblErrMsg.Text = "" Then
                            Response.Redirect("servers.aspx")
                        End If
                    Catch ex As Exception
                        If cammWebManager.System_DebugLevel >= 3 Then
                            lblErrMsg.Text = "Server update failed! (" & ex.Message & ex.StackTrace & ")"
                        Else
                            lblErrMsg.Text = "Server update failed!"
                        End If
                    End Try
                Catch ex As CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.DataException
                    If ex.ToString.IndexOf("IP / Host Header already exists") > 0 Then
                        lblErrMsg.Text = "IP / Host Header already exists"
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = ex.Message
                End Try
            End If
        End Sub

        Private Sub rptEngine_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptEngine.ItemDataBound
            Select Case e.Item.ItemType
                Case ListItemType.AlternatingItem, ListItemType.Item
                    Dim cmbEngine As DropDownList = CType(e.Item.FindControl("cmbEngine"), DropDownList)
                    FillEnabledCombo(cmbEngine)
                    Dim drCurrent As DataRowView = CType(e.Item.DataItem, DataRowView)
                    If Utils.Nz(drCurrent("IsActivated"), False) = False Then
                        cmbEngine.SelectedValue = "0"
                    Else
                        cmbEngine.SelectedValue = "1"
                    End If
            End Select
        End Sub
#End Region

    End Class

End Namespace