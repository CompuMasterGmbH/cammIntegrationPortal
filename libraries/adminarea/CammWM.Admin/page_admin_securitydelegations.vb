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

    ''' <summary>
    '''     Adjustments of delegations of security adminisration tasks
    ''' </summary>
    Public Class AdjustDelegates
        Inherits Page

#Region "Variable Declaration"
        Protected WithEvents rptAdjust As Repeater
        Public MyTableName As String
        Public BackLink As String = ""
        Public GlobalMode As Boolean = False
        Public ShowCompleteListOfDelegations As Boolean = False
        Protected lblTableName As Label
        Protected hypDeligates As HyperLink
        Protected lblAction As Label
        Protected trAdd As HtmlTableRow
        Protected cmbUser, cmbAuthorizationType As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected lblTitleName As Label
        Protected LinkButtonAdministrator, LinkButtonDelegation, LinkButtonSecurityObject As LinkButton
        Protected hdColumnName, hdSortOrder As HtmlInputHidden
#End Region

#Region "Page Events"
        Private Sub AdjustDelegates_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            If Request.QueryString("Type") = "Groups" AndAlso Utils.Nz(Request.QueryString("ID"), 0) <> 0 Then
                cammWebManager.AuthorizeDocumentAccess("System - User Administration - Groups")
                lblTableName.Text = "Groups"
                If Not (cammWebManager.System_GetSubAuthorizationStatus("Groups", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") OrElse cammWebManager.System_GetSubAuthorizationStatus("Groups", CInt(Request.QueryString("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner")) Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "Not enough access authorizations."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
                BackLink = "groups.aspx"
            ElseIf Request.QueryString("Type") = "Applications" AndAlso Utils.Nz(Request.QueryString("ID"), 0) <> 0 Then
                cammWebManager.AuthorizeDocumentAccess("System - User Administration - Authorizations")
                lblTableName.Text = "Applications"
                If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") OrElse cammWebManager.System_GetSubAuthorizationStatus("Groups", CInt(Request.QueryString("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner")) Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "Not enough access authorizations."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
                BackLink = "apps.aspx"
            ElseIf Request.QueryString("Type") = "Groups" AndAlso Utils.Nz(Request.QueryString("ID"), 0) = 0 AndAlso Not Request.QueryString("Action") = "VIEWALL" Then
                cammWebManager.AuthorizeDocumentAccess("System - User Administration - Groups")
                lblTableName.Text = "Groups"
                GlobalMode = True
                If Not cammWebManager.System_GetSubAuthorizationStatus("Groups", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "Not enough access authorizations."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
                BackLink = "groups.aspx"
            ElseIf Request.QueryString("Type") = "Applications" And Utils.Nz(Request.QueryString("ID"), 0) = 0 AndAlso Not Request.QueryString("Action") = "VIEWALL" Then
                cammWebManager.AuthorizeDocumentAccess("System - User Administration - Authorizations")
                lblTableName.Text = "Applications"
                GlobalMode = True
                If Not cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "Not enough access authorizations."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
                BackLink = "apps.aspx"
            ElseIf Request.QueryString("Type") = "Groups" AndAlso Request.QueryString("Action") = "VIEWALL" Then
                cammWebManager.AuthorizeDocumentAccess("System - User Administration - Groups")
                lblTableName.Text = "Groups"
                GlobalMode = True
                ShowCompleteListOfDelegations = True
                If Not cammWebManager.System_GetSubAuthorizationStatus("Groups", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "Not enough access authorizations."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
                BackLink = "groups.aspx"
            ElseIf Request.QueryString("Type") = "Applications" And Request.QueryString("Action") = "VIEWALL" Then
                cammWebManager.AuthorizeDocumentAccess("System - User Administration - Authorizations")
                lblTableName.Text = "Applications"
                GlobalMode = True
                ShowCompleteListOfDelegations = True
                If Not cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "Not enough access authorizations."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
                BackLink = "apps.aspx"
            Else
                If Not CType(Request.QueryString("ID"), Integer) < -1 Then
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    Dim message As String = "URL cannot be delivered since it needs a valid administration session."
                    cammWebManager.Log.Warn(message, WorkaroundStackTrace)
                    cammWebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
            End If

            cammWebManagerAdminMenu.HRef = BackLink
            cammWebManager.PageTitle = "Administration - " & lblTableName.Text & " - Security delegates"

            If lblTableName.Text <> "" AndAlso Request.QueryString("Action") = "SAVE" And Request.QueryString("Type") <> "" And Request.QueryString("ID") <> "" And Request.Form("UserID") <> "" And Request.Form("AuthorizationType") <> "" Then
                Dim MyDBConn As New SqlConnection
                MyDBConn.ConnectionString = cammWebManager.ConnectionString
                Dim MyCmd As New SqlCommand
                Try
                    MyDBConn.Open()
                    With MyCmd
                        .CommandText = "AdminPrivate_UpdateSubSecurityAdjustment"
                        .CommandType = CommandType.StoredProcedure
                        .Parameters.Add("@ActionTypeSave", SqlDbType.Bit).Value = True
                        .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = lblTableName.Text
                        .Parameters.Add("@TablePrimaryIDValue", SqlDbType.Int).Value = CLng(Request.QueryString("ID"))
                        .Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = CType(Request.Form("AuthorizationType"), String)
                        .Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(Request.Form("UserID"))
                        If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_Build173 Then 'Newer
                            .Parameters.Add("@ReleasedBy", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                        End If
                    End With
                    MyCmd.Connection = MyDBConn
                    MyCmd.ExecuteNonQuery()
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyDBConn Is Nothing Then
                        If MyDBConn.State <> ConnectionState.Closed Then
                            MyDBConn.Close()
                        End If
                        MyDBConn.Dispose()
                    End If
                End Try
            ElseIf lblTableName.Text <> "" AndAlso Request.QueryString("Action") = "DEL" And Request.QueryString("Type") <> "" And Request.QueryString("ID") <> "" And Request.QueryString("UserID") <> "" And Request.QueryString("AuthorizationType") <> "" Then
                Try
                    Dim MyCmd As New SqlClient.SqlCommand("AdminPrivate_UpdateSubSecurityAdjustment", New SqlConnection(cammWebManager.ConnectionString))
                    MyCmd.CommandType = CommandType.StoredProcedure
                    MyCmd.Parameters.Add("@ActionTypeSave", SqlDbType.Bit).Value = False
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(Request.QueryString("UserID"))
                    MyCmd.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = lblTableName.Text
                    MyCmd.Parameters.Add("@TablePrimaryIDValue", SqlDbType.Int).Value = CLng(Request.QueryString("ID"))
                    MyCmd.Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = CType(Request.QueryString("AuthorizationType"), String)
                    If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_Build173 Then 'Newer
                        MyCmd.Parameters.Add("@ReleasedBy", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                    End If
                    ExecuteNonQuery(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Catch ex As Exception
                    Throw
                End Try
            End If
        End Sub

        Private Sub AdjustDelegates_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblTitleName.Text = Server.HtmlEncode(Request.QueryString("Title"))
            btnSubmit.Attributes.Add("onclick", "return ValidateForm();")
        End Sub

        Private Sub AdjustDelegates_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            If ShowCompleteListOfDelegations = False Then
                BindRepeater()
                If (GlobalMode) Then
                    hypDeligates.Text = "View all delegates"
                    hypDeligates.NavigateUrl = Request.ServerVariables("SCRIPT_NAME") & "?Type=" & lblTableName.Text & "&Action=VIEWALL"
                End If
                trAdd.Visible = True
                If Not rptAdjust.Items.Count Mod 2 = 0 Then trAdd.BgColor = "#E1E1E1"
                If Not Page.IsPostBack Then
                    FillUserCombo(cmbUser)
                    FillAuthorizationTypeCombo(cmbAuthorizationType)
                End If
            Else
                trAdd.Visible = False
                BindReadOnlyRepeater()
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
        Public Function GetMyDataset() As DataTable
            lblTableName.Text = Request.QueryString("Type")
            If lblTableName.Text = Nothing Then lblTableName.Text = ""
            Dim MyDt As New DataTable

            Try
                Dim sqlParams As SqlParameter() = {New SqlParameter("@PrimID", CLng(Request.QueryString("ID"))),
                    New SqlParameter("@TableName", lblTableName.Text)}
                Dim sqlQuery As String = Nothing
                If Me.cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_Build173 Then
                    'ReleasedBy and ReleasedOn only in Database Build 173 or above
                    sqlQuery = "SELECT id, userid, authorizationtype, releasedby, releasedon FROM System_SubSecurityAdjustments WHERE TableName = @TableName AND TablePrimaryIDValue = @PrimID ORDER BY authorizationtype"
                Else
                    sqlQuery = "SELECT id, userid, authorizationtype FROM System_SubSecurityAdjustments WHERE TableName = @TableName AND TablePrimaryIDValue = @PrimID ORDER BY authorizationtype"
                End If
                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & sqlQuery
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            Catch
                Throw
            Finally
                MyDt.Dispose()
            End Try

            Return MyDt
        End Function

        Public Function GetCompleteListDataset_Applications() As DataTable
            lblTableName.Text = Request.QueryString("Type")
            If lblTableName.Text = Nothing Then lblTableName.Text = ""
            Dim MyDt As New DataTable

            Try
                Dim sqlQuery As String
                Dim _DBVersion As Version = CompuMaster.camm.WebManager.Setup.DatabaseUtils.Version(cammWebManager, True)

                If _DBVersion.CompareTo(WMSystem.MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                    sqlQuery = "SELECT System_SubSecurityAdjustments.*, CASE When IsNull(dbo.Applications.TitleAdminArea,'') = N'' Then dbo.Applications.Title Else dbo.Applications.TitleAdminArea End As SecuredObject FROM System_SubSecurityAdjustments LEFT JOIN dbo.Applications ON System_SubSecurityAdjustments.TablePrimaryIDValue = dbo.Applications.ID" & vbNewLine
                    sqlQuery &= "WHERE TableName = @TableName AND (TablePrimaryIDValue = 0 OR TablePrimaryIDValue IN (SELECT ID FROM dbo.Sytem_SecurityObjects)) ORDER BY AuthorizationType"
                Else
                    sqlQuery = "SELECT System_SubSecurityAdjustments.*, CASE When IsNull(dbo.Applications.TitleAdminArea,'') = N'' Then dbo.Applications.Title Else dbo.Applications.TitleAdminArea End + ' (ID ' + Cast(System_SubSecurityAdjustments.TablePrimaryIDValue as nvarchar) + ')' As SecuredObject FROM System_SubSecurityAdjustments LEFT JOIN dbo.Applications ON System_SubSecurityAdjustments.TablePrimaryIDValue = dbo.Applications.ID" & vbNewLine
                    sqlQuery &= "WHERE TableName = @TableName AND (TablePrimaryIDValue = 0 OR TablePrimaryIDValue IN (SELECT ID FROM dbo.Applications_CurrentAndInactiveOnes)) ORDER BY AuthorizationType"
                End If
                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & sqlQuery

                Dim sqlParams As SqlParameter() = {New SqlParameter("@TableName", "Applications")}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            Catch
                Throw
            Finally
                MyDt.Dispose()
            End Try

            Return MyDt
        End Function

        Public Function GetCompleteListDataset_Groups() As DataTable
            lblTableName.Text = Server.HtmlEncode(Request.QueryString("Type"))
            If lblTableName.Text = Nothing Then lblTableName.Text = ""
            Dim MyDt As New DataTable

            Try
                Dim sqlParams As SqlParameter() = {New SqlParameter("@TableName", "Groups")}
                Dim sqlQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                                    "SELECT System_SubSecurityAdjustments.*, dbo.Gruppen.[Name] AS SecuredObject FROM System_SubSecurityAdjustments LEFT JOIN dbo.Gruppen ON System_SubSecurityAdjustments.TablePrimaryIDValue = dbo.Gruppen.ID WHERE TableName = @TableName  AND (TablePrimaryIDValue = 0 OR TablePrimaryIDValue IN (SELECT ID FROM dbo.Gruppen)) ORDER BY AuthorizationType"
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            Catch
                Throw
            Finally
                MyDt.Dispose()
            End Try

            Return MyDt
        End Function

        Public Function GetSecurityAdminsDataset() As DataTable
            lblTableName.Text = Server.HtmlEncode(Request.QueryString("Type"))
            If lblTableName.Text = Nothing Then lblTableName.Text = ""
            Dim MyDt As New DataTable

            Try
                Dim sqlQuery As String
                If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    sqlQuery = "SELECT ID_User From Memberships_EffectiveRulesWithClonesNthGrade WHERE ID_Group IN (6, 7, -7) group by ID_User"
                Else
                    sqlQuery = "SELECT ID_User From Memberships WHERE ID_Group IN (6, 7, -7) group by ID_User"
                End If
                MyDt = FillDataTable(New SqlCommand(sqlQuery, New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            Catch
                Throw
            Finally
                MyDt.Dispose()
            End Try

            Return MyDt
        End Function

        Private Sub FillUserCombo(ByRef cmbUser As DropDownList)
            Dim MyDT As DataTable = GetSecurityAdminsDataset()
            cmbUser.Items.Clear()
            MyDT.Columns.Add(New DataColumn("CompleteName", GetType(String)))

            For Each myDR As DataRow In MyDT.Rows
                Dim FormattedName As String = Nothing
                Try
                    FormattedName = CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(myDR("ID_User")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem)))
                Catch ex As Exception
                    FormattedName = "[?] (" & ex.Message & ")"
                End Try
                myDR("Completename") = FormattedName
            Next

            If Not MyDT Is Nothing AndAlso MyDT.Rows.Count > 0 Then
                Dim drBlank As DataRow = MyDT.NewRow
                drBlank("Completename") = ""
                drBlank("ID_User") = "-1"
                MyDT.Rows.InsertAt(drBlank, 0)

                For Each drow As DataRow In MyDT.Select("", "completename asc")
                    cmbUser.Items.Add(New ListItem(Utils.Nz(drow("Completename"), String.Empty), drow("ID_User").ToString))
                Next
            End If

            'cmbUser.DataSource = New DataView(MyDT, "", "completename asc", DataViewRowState.CurrentRows)
            'cmbUser.DataTextField = "Completename"
            'cmbUser.DataValueField = "ID_User"
            'cmbUser.DataBind()
        End Sub

        Private Sub FillAuthorizationTypeCombo(ByRef cmbAuthorizationType As DropDownList)
            If GlobalMode = True Then
                cmbAuthorizationType.Items.Add(New ListItem("", ""))
                cmbAuthorizationType.Items.Add(New ListItem("SecurityMaster", "SecurityMaster"))
                cmbAuthorizationType.Items.Add(New ListItem("New", "New"))
                If lblTableName.Text = "Groups" Then
                    cmbAuthorizationType.Items.Add(New ListItem("ViewAllItems", "ViewAllItems"))
                    cmbAuthorizationType.Items.Add(New ListItem("ViewAllRelations", "ViewAllRelations"))
                Else
                    'Not yet implemented for Applications
                End If
            Else
                cmbAuthorizationType.Items.Add(New ListItem("", ""))
                cmbAuthorizationType.Items.Add(New ListItem("Owner", "Owner"))
                cmbAuthorizationType.Items.Add(New ListItem("Update", "Update"))
                cmbAuthorizationType.Items.Add(New ListItem("UpdateRelations", "UpdateRelations"))
                cmbAuthorizationType.Items.Add(New ListItem("Delete", "Delete"))
                cmbAuthorizationType.Items.Add(New ListItem("View", "View"))
                If lblTableName.Text = "Groups" Then
                    cmbAuthorizationType.Items.Add(New ListItem("ViewRelations", "ViewRelations"))
                Else
                    'Not yet implemented for Applications
                End If
                cmbAuthorizationType.Items.Add(New ListItem("ViewLogs", "ViewLogs"))
                cmbAuthorizationType.Items.Add(New ListItem("PrimaryContact", "PrimaryContact"))
                cmbAuthorizationType.Items.Add(New ListItem("ResponsibleContact", "ResponsibleContact"))
            End If
        End Sub

        Private Sub BindRepeater()
            Dim MyDt As DataTable = GetMyDataset()
            Dim sortexp As String = ""
            sortexp = Convert.ToString(hdColumnName.Value) & " " & Convert.ToString(hdSortOrder.Value)
            If sortexp.ToString.Trim = "" Then
                sortexp = "AuthorizationType asc, CompleteName asc"
                hdColumnName.Value = "CompleteName"
                hdSortOrder.Value = "asc"
            Else
                sortexp = sortexp.ToString.Trim + ", AuthorizationType asc, CompleteName asc"
            End If
            MyDt.Columns.Add(New DataColumn("CompleteName", GetType(String)))
            Dim myDR As DataRow
            For Each myDR In MyDt.Rows
                Dim FormattedName As String = Nothing
                Try
                    FormattedName = CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(myDR("userid")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem)))
                Catch ex As Exception
                    FormattedName = "[?] (" & ex.Message & ")"
                End Try
                myDR("Completename") = CStr(myDR("userid")) & FormattedName
            Next
            rptAdjust.DataSource = New DataView(MyDt, "", sortexp, DataViewRowState.CurrentRows)
            rptAdjust.DataBind()
        End Sub

        Private Sub BindReadOnlyRepeater()
            Dim MyDT As DataTable

            If lblTableName.Text <> "Groups" Then
                MyDT = GetCompleteListDataset_Applications()
            Else
                MyDT = GetCompleteListDataset_Groups()
            End If
            'rptAdjust.DataSource = MyDT
            'rptAdjust.DataBind()
            Dim sortexp As String = ""
            sortexp = hdColumnName.Value.ToString() & " " & hdSortOrder.Value.ToString()
            If sortexp.ToString.Trim = "" Then
                sortexp = "SecuredObject asc, AuthorizationType asc, CompleteName asc"
                hdColumnName.Value = "CompleteName"
                hdSortOrder.Value = "asc"
            Else
                sortexp = Trim(sortexp) + ", SecuredObject asc, AuthorizationType asc, CompleteName asc"
            End If
            MyDT.Columns.Add(New DataColumn("CompleteName", GetType(String)))
            Dim myDR As DataRow
            For Each myDR In MyDT.Rows
                If Not cammWebManager.System_GetUserInfo(CType(Utils.Nz(CLng(myDR("userid"))), Int64)) Is Nothing Then
                    Dim FormattedName As String = Nothing
                    Try
                        FormattedName = CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(myDR("userid")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem)))
                    Catch ex As Exception
                        FormattedName = "[?] (" & ex.Message & ")"
                    End Try
                    myDR("Completename") = FormattedName
                End If
            Next
            rptAdjust.DataSource = New DataView(MyDT, "", sortexp, DataViewRowState.CurrentRows)
            rptAdjust.DataBind()
        End Sub

        Private Sub SaveRecord()
            Dim MyDBConn As New SqlConnection
            MyDBConn.ConnectionString = cammWebManager.ConnectionString
            Dim MyCmd As New SqlCommand
            Try
                MyDBConn.Open()
                With MyCmd
                    .CommandText = "AdminPrivate_UpdateSubSecurityAdjustment"
                    .CommandType = CommandType.StoredProcedure

                    .Parameters.Add("@ActionTypeSave", SqlDbType.Bit).Value = True
                    .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = lblTableName.Text
                    .Parameters.Add("@TablePrimaryIDValue", SqlDbType.Int).Value = CLng(Request.QueryString("ID"))
                    .Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = CType(cmbAuthorizationType.SelectedValue, String)
                    .Parameters.Add("@UserID", SqlDbType.Int).Value = CLng(cmbUser.SelectedValue)
                    If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_Build173 Then 'Newer
                        .Parameters.Add("@ReleasedBy", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                    End If
                End With
                MyCmd.Connection = MyDBConn
                MyCmd.ExecuteNonQuery()
                cmbUser.SelectedValue = CType(-1, String)
                cmbAuthorizationType.SelectedValue = ""
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyDBConn Is Nothing Then
                    If MyDBConn.State <> ConnectionState.Closed Then
                        MyDBConn.Close()
                    End If
                    MyDBConn.Dispose()
                End If
            End Try

        End Sub
#End Region

#Region "Control Events"
        Private Sub rptAdjust_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAdjust.ItemCreated
            If e.Item.ItemType = ListItemType.Header Then
                CType(e.Item.FindControl("LinkButtonDelegation"), LinkButton).Attributes.Add("onclick", "return SetHeaderColumn('AuthorizationType');")
                CType(e.Item.FindControl("LinkButtonAdministrator"), LinkButton).Attributes.Add("onclick", "return SetHeaderColumn('CompleteName');")
                CType(e.Item.FindControl("LinkButtonSecurityObject"), LinkButton).Attributes.Add("onclick", "return SetHeaderColumn('SecuredObject');")
            End If
        End Sub

        Private Sub rptAdjust_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptAdjust.ItemDataBound
            Select Case e.Item.ItemType
                Case ListItemType.Header
                    'Dim lblAction As HtmlAnchor = CType(e.Item.FindControl("lblAction"), HtmlAnchor)
                    If (ShowCompleteListOfDelegations = False) Then
                        'lblAction.InnerText = "Action"
                        CType(e.Item.FindControl("lblAction"), Label).Visible = True
                        CType(e.Item.FindControl("LinkButtonSecurityObject"), LinkButton).Visible = False
                    Else
                        'lblAction.InnerText = "Secured object"
                        CType(e.Item.FindControl("lblAction"), Label).Visible = False
                        CType(e.Item.FindControl("LinkButtonSecurityObject"), LinkButton).Visible = True
                    End If

                Case ListItemType.Footer

                Case ListItemType.Item, ListItemType.AlternatingItem
                    Dim lblSecurityAdmin As Label = CType(e.Item.FindControl("lblSecurityAdmin"), Label)
                    Dim lblReleasedBy As Label = CType(e.Item.FindControl("lblReleasedBy"), Label)
                    Dim lblReleasedOn As Label = CType(e.Item.FindControl("lblReleasedOn"), Label)
                    Dim drCurrent As DataRowView = CType(e.Item.DataItem, DataRowView)
                    Try
                        lblSecurityAdmin.Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(drCurrent("UserID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))))
                    Catch ex As Exception
                        lblSecurityAdmin.Text = Server.HtmlEncode("[?] (" & ex.Message & ")")
                    End Try
                    If Me.cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_Build173 AndAlso Not Utils.Nz(drCurrent("ReleasedBy")) Is Nothing Then
                        Try
                            Dim UI As New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(Utils.Nz(drCurrent("ReleasedBy"))), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                            lblReleasedBy.Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(UI.FirstName, UI.LastName, UI.LoginName, UI.IDLong))
                        Catch ex As Exception
                            lblReleasedBy.Text = Server.HtmlEncode("[?] (" & ex.Message & ")")
                        End Try
                    Else
                        lblReleasedBy.Text = Nothing
                    End If
                    If Me.cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_Build173 AndAlso Not Utils.Nz(drCurrent("ReleasedOn")) Is Nothing Then
                        lblReleasedOn.Text = Server.HtmlEncode(Utils.Nz(drCurrent("ReleasedOn"), String.Empty))
                    Else
                        lblReleasedOn.Text = Nothing
                    End If
                    If (ShowCompleteListOfDelegations = False) Then
                        If CLng(Val(drCurrent("UserID"))) = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) And Utils.Nz(drCurrent("AuthorizationType"), String.Empty) = "SecurityMaster" Then
                        Else
                            Dim hypScript As HyperLink = CType(e.Item.FindControl("hypScript"), HyperLink)
                            hypScript.NavigateUrl = Request.ServerVariables("SCRIPT_NAME") + "?Title=" + Server.UrlEncode(Request.QueryString("Title")) + "&ID=" + Request.QueryString("ID") + "&Type=" + Request.QueryString("Type") + "&UserID=" + drCurrent("UserID").ToString + "&AuthorizationType=" + Server.UrlEncode(Utils.Nz(drCurrent("AuthorizationType"), String.Empty)) + "&Action=DEL"
                            hypScript.Text = "Delete"
                        End If
                    Else
                        Dim lblSecurityObject As Label = CType(e.Item.FindControl("lblSecuredObject"), Label)
                        If IsDBNull(drCurrent("SecuredObject")) Then
                            lblSecurityObject.Text = "<em>{missing ID}</em>"
                        Else
                            lblSecurityObject.Text = Server.HtmlEncode(CType(drCurrent("SecuredObject"), String))
                        End If
                    End If
            End Select
        End Sub

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            SaveRecord()
        End Sub
#End Region

    End Class


End Namespace


