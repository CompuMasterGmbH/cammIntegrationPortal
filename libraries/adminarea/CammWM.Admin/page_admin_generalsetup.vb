'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration


    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.MarketActivations
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Activation of markets
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	29.04.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class MarketActivations
        Inherits Page

        Protected Markets As DataGrid

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The market which shall be changed
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	29.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Market() As Integer
            Get
                If Request.QueryString("Market") <> "" Then
                    Return CType(Request.QueryString("Market"), Integer)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Shall the market be activated?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	29.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property MarketActivated() As Boolean
            Get
                If Request.QueryString("MarketActivated") <> "" Then
                    Return CType(Request.QueryString("MarketActivated"), Boolean)
                Else
                    Throw New ArgumentNullException("MarketActivated")
                End If
            End Get
        End Property

        Private Sub MarketActivations_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Execute update if requested
            If Market <> Nothing Then
                cammWebManager.System_SetLanguageState(Market, MarketActivated)
            End If

            'Load data and fill the datagrid
            Dim AllMarkets As CompuMaster.camm.WebManager.WMSystem.LanguageInformation()
            AllMarkets = cammWebManager.System_GetLanguagesInfo(Nothing, True, False)

            Dim MarketsTable As New DataTable("Markets")
            MarketsTable.Columns.Add("ID", GetType(Integer))
            MarketsTable.Columns.Add("Name", GetType(String))
            MarketsTable.Columns.Add("Activated", GetType(Boolean))

            For MyCounter As Integer = 0 To AllMarkets.Length - 1
                Dim MyRow As DataRow = MarketsTable.NewRow
                MyRow(0) = AllMarkets(MyCounter).ID
                MyRow(1) = AllMarkets(MyCounter).LanguageName_English
                MyRow(2) = AllMarkets(MyCounter).IsActive
                MarketsTable.Rows.Add(MyRow)
            Next

            Markets.AutoGenerateColumns = False
            Markets.DataSource = MarketsTable
            Markets.Columns(0).HeaderText = "ID"
            Markets.Columns(1).HeaderText = "Name"
            Markets.Columns(2).HeaderText = "Activated"
            Markets.DataBind()
        End Sub

    End Class

#Region " Access Levels "
    ''' <summary>
    '''     A page to view accesslevels
    ''' </summary>
    Public Class AccessLevelsList
        Inherits Page

#Region "Variable Declaration"
        Protected WithEvents rptAccessRights As Repeater
        Protected lblID, lblRemarks, lblErrMsg As Label
        Protected hypTitle, hypDelete, hypUpdate As HyperLink
        Dim dt As New DataTable
#End Region

#Region "Page Events"
        ''' <summary>
        '''     Page Load event - shows the list of access levels
        ''' </summary>
        Private Sub AccessLevelsList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            lblErrMsg.Text = ""

            Try
                dt = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM System_AccessLevels ORDER BY Title", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    rptAccessRights.DataSource = dt
                    rptAccessRights.DataBind()
                End If
            Catch ex As Exception
                Throw
            Finally
                dt.Dispose()
            End Try
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptAccessRightsItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptAccessRights.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                With dt.Rows(e.Item.ItemIndex)
                    CType(e.Item.FindControl("lblID"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ID"), String.Empty))
                    CType(e.Item.FindControl("lblRemarks"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Remarks"), String.Empty))
                    CType(e.Item.FindControl("hypTitle"), HyperLink).NavigateUrl = "accesslevels_update.aspx?ID=" & Utils.Nz(.Item("ID"), String.Empty)
                    CType(e.Item.FindControl("hypTitle"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("Title"), String.Empty))
                    CType(e.Item.FindControl("hypUpdate"), HyperLink).NavigateUrl = "accesslevels_update.aspx?ID=" & Utils.Nz(.Item("ID"), String.Empty)
                    CType(e.Item.FindControl("hypUpdate"), HyperLink).Text = "Update"
                    CType(e.Item.FindControl("hypDelete"), HyperLink).NavigateUrl = "accesslevels_delete.aspx?ID=" & Utils.Nz(.Item("ID"), String.Empty)
                    CType(e.Item.FindControl("hypDelete"), HyperLink).Text = "Delete"
                End With
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to create accesslevel
    ''' </summary>
    Public Class AccessLevelsNew
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg As Label
        Protected txtTitle As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub AccessLevelsNew_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""
        End Sub
#End Region

#Region "Control Events"
        Private Sub CreateAccessLevel(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If Trim(txtTitle.Text) <> "" Then
                Try
                    Dim iResult As Object
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@Title", Mid(Trim(txtTitle.Text), 1, 50))}
                    iResult = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_CreateAccessLevel", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                    If iResult Is Nothing OrElse IsDBNull(iResult) Then
                        lblErrMsg.Text = Utils.Nz("Undefined error detected!", String.Empty)
                    ElseIf CLng(iResult) > 0 Then
                        Response.Redirect("accesslevels.aspx")
                    Else
                        lblErrMsg.Text = Utils.Nz("Access level creation failed!", String.Empty)
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = Utils.Nz("Undefined error detected!", String.Empty)
                End Try
            Else
                lblErrMsg.Text = Utils.Nz("Please specify a title to proceed!", String.Empty)
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to update accesslevel
    ''' </summary>
    Public Class AccessLevelsUpdate
        Inherits Page

#Region "Variable Declaration"
        Protected lblID, lblErrMsg As Label
        Protected txtTitle, txtRemarks As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub AccessLevelsUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""

            If Not IsPostBack Then
                AssignControlLabels()
                SetDatabaseValuesToControls()
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub AssignControlLabels()
            lblID.Text = Server.HtmlEncode(Utils.Nz(Request.QueryString("ID"), String.Empty))
            btnSubmit.Text = "Update access level"
        End Sub

        Private Sub SetDatabaseValuesToControls()
            Dim MySQLString As String
            Dim dt As New DataTable

            Try
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("ID")))}
                MySQLString = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Title, Remarks FROM dbo.System_AccessLevels WHERE ID=@ID"
                dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), MySQLString, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If dt Is Nothing AndAlso dt.Rows.Count = 0 Then
                    lblErrMsg.Text = Utils.Nz("Access level not found!", String.Empty)
                Else
                    txtTitle.Text = Trim(Utils.Nz(dt.Rows(0)("title"), String.Empty))
                    txtRemarks.Text = Trim(Utils.Nz(dt.Rows(0)("remarks"), String.Empty))
                End If
            Catch ex As Exception
                lblErrMsg.Text = "Undefined error detected!"
            Finally
                dt.Dispose()
            End Try
        End Sub
#End Region

#Region "Control Events"
        Private Sub CreateAccessLevel(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If Trim(txtTitle.Text) <> "" Then
                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID"))), New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@Title", Mid(Trim(txtTitle.Text), 1, 50)), CType(IIf(Trim(CStr(txtRemarks.Text)) <> "", New SqlParameter("@Remarks", Trim(CStr(txtRemarks.Text))), DBNull.Value), SqlParameter)}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateAccessLevel", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("accesslevels.aspx")
                Catch
                    lblErrMsg.Text = "Access level update failed!"
                End Try
            Else
                txtTitle.Text = Request.Form("title")
                txtRemarks.Text = Request.Form("remarks")
                lblErrMsg.Text = Utils.Nz("Please specify the field ""Title"" to proceed!", String.Empty)
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to delete accesslevel
    ''' </summary>
    Public Class AccessLevelsDelete
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblFieldID, lblFieldTitle, lblFieldRemarks As Label
#End Region

#Region "Page Events"
        Private Sub AccessLevelsDelete_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            DeleteAccessLevel()
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub DeleteAccessLevel()
            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" Then
                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("id")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteAccessLevel", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("accesslevels.aspx")
                Catch
                    lblErrMsg.Text = Utils.Nz("Access level erasing failed!", String.Empty)
                End Try
            Else
                Dim MySqlString As String
                Dim dt As New DataTable

                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("ID")))}
                    MySqlString = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.System_AccessLevels WHERE ID=@ID"
                    dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), MySqlString, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    If dt Is Nothing AndAlso dt.Rows.Count = 0 Then
                        lblErrMsg.Text = "Access level not found!"
                    Else
                        lblFieldID.Text = Utils.Nz(dt.Rows(0)("ID"), 0).ToString
                        lblFieldTitle.Text = Server.HtmlEncode(Utils.Nz(dt.Rows(0)("Title"), String.Empty))
                        lblFieldRemarks.Text = Server.HtmlEncode(Utils.Nz(dt.Rows(0)("Remarks"), String.Empty))
                    End If
                Catch ex As Exception
                    Throw
                Finally
                    dt.Dispose()
                End Try
            End If
        End Sub
#End Region

    End Class

#End Region

End Namespace