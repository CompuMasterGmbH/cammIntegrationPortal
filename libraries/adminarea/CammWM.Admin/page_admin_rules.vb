'Copyright 2008-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
    '''     A page to select user-field and display a list of users per field
    ''' </summary>
    Public Class RulesUserFields
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblTitle As Label
        Protected hypListRules As HyperLink
        Protected WithEvents rptFieldList As Repeater
        Protected WithEvents btnApplyChanges As Button
        Protected WithEvents dropFieldSelection As DropDownList
#End Region

#Region "Page Events"
        Private Sub UserFieldDetails_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = String.Empty

            If Not IsPostBack Then
                Try
                    FillFieldDropDownList()
                    ListOfUserFieldDetails()
                    SetLabelsBySelectedField()
                Catch ex As Exception
                    Throw
                End Try
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        Private Sub SetLabelsBySelectedField()
            hypListRules.NavigateUrl = "listallrules.aspx?field=" & Trim(dropFieldSelection.SelectedValue)
            lblTitle.Text = "Administration - Users Per " & Server.HtmlEncode(Trim(dropFieldSelection.SelectedItem.Text))
        End Sub

        Private Sub FillFieldDropDownList()
            If dropFieldSelection.Items.Count > 0 Then dropFieldSelection.Items.Clear()

            dropFieldSelection.Items.Add(New ListItem("Academic Title", "title"))
            dropFieldSelection.Items.Add(New ListItem("Company", "company"))
            dropFieldSelection.Items.Add(New ListItem("Country", "country"))
            dropFieldSelection.Items.Add(New ListItem("State", "state"))

            'if redirected from replace page then it will select that field again in dropdownlist
            If Trim(Request.QueryString("field")) <> String.Empty Then dropFieldSelection.SelectedValue = Trim(Request.QueryString("field"))
        End Sub

        Private Function GetListOfUsersByField(ByVal strSearchValue As String) As DataTable
            Dim sqlParams As SqlParameter() = {New SqlParameter("@FieldName", Trim(dropFieldSelection.SelectedValue)), New SqlParameter("@searchvalue", strSearchValue.Trim)}
            Dim sqlQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select lu.id_user from log_users lu inner join benutzer b on b.id=lu.id_user where lu.type=@FieldName and lu.value=@searchvalue"
            Return FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
        End Function

        Private Sub ListOfUserFieldDetails()
            If rptFieldList.Visible = False Then rptFieldList.Visible = True
            If btnApplyChanges.Enabled = False Then btnApplyChanges.Enabled = True
            Dim sqlParams As SqlParameter() = {New SqlParameter("@fieldid", Trim(dropFieldSelection.SelectedValue))}
            Dim sqlQuery As String = String.Empty

            If Trim(dropFieldSelection.SelectedValue) = "title" Then
                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select isnull(titel,'') as value,count(id) counts from benutzer group by isnull(titel,'') order by value"
            ElseIf Trim(dropFieldSelection.SelectedValue) = "country" Then
                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select isnull(land,'') as value,count(id) counts from benutzer group by isnull(land,'') order by value"
            Else
                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select isnull(" & Trim(dropFieldSelection.SelectedValue) & ",'') as value,count(id) counts from benutzer group by isnull(" & Trim(dropFieldSelection.SelectedValue) & ",'') order by value"
                'sqlQuery = "select isnull(value,'') value,count(lu.id) counts from log_users lu inner join benutzer b on lu.id_user=b.id where type=@fieldid group by isnull(value,'') order by value"
            End If

            rptFieldList.DataSource = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            rptFieldList.DataBind()

            If rptFieldList.Items.Count = 0 Then
                btnApplyChanges.Enabled = False
                rptFieldList.Visible = False
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptFieldList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptFieldList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)
                With drv
                    CType(e.Item.FindControl("lblFieldValue"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("value"), String.Empty))
                    CType(e.Item.FindControl("lblUserCount"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("counts"), String.Empty))
                    CType(e.Item.FindControl("lnkReplace"), HyperLink).NavigateUrl = "replacefieldinfo.aspx?field=" + dropFieldSelection.SelectedValue + "&value=" + Utils.Nz(.Item("value"), String.Empty).Trim
                    CType(e.Item.FindControl("lnkUserDetails"), HyperLink).NavigateUrl = "fielduserlist.aspx?field=" + dropFieldSelection.SelectedValue + "&value=" + Utils.Nz(.Item("value"), String.Empty).Trim
                End With
            End If
        End Sub

        Private Sub btnApplyChanges_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApplyChanges.Click
            Try
                'Replace all records according to existing rules
                Dim sqlParams As SqlParameter() = {New SqlParameter("@FieldName", Trim(dropFieldSelection.SelectedValue)), New SqlParameter("@RuleName", "userfields"), New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@RemoteIP", cammWebManager.CurrentServerIdentString), New SqlParameter("@ServerIP", cammWebManager.CurrentRemoteClientAddress), New SqlParameter("@SecurityObject", cammWebManager.SecurityObject.ToString), New SqlParameter("@LocationID", cammWebManager.CurrentServerInfo.ID), New SqlParameter("@LangID", cammWebManager.UIMarketInfo.AlternativeLanguageInfo.ID)}
                ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_RulesUserFieldsApplyAll", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                ListOfUserFieldDetails()
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub dropFieldSelection_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dropFieldSelection.SelectedIndexChanged
            ListOfUserFieldDetails()
            SetLabelsBySelectedField()
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to view a list of users as per selected user-field
    ''' </summary>
    Public Class FieldUserList
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblTitle, lblTotalUsers As Label
        Protected WithEvents rptUserList As Repeater
#End Region

#Region "Page Events"
        Private Sub FieldUserList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not IsPostBack Then
                    BindUserList()
                    cammWebManagerAdminMenu.HRef = "rules_user_fields.aspx?field=" & Trim(Request.QueryString("field"))
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

#Region "User-Defined Functions"
        'bind user-list according to passed field type 
        Private Sub BindUserList()
            If rptUserList.Visible = False Then rptUserList.Visible = True
            Dim sqlQuery As String = String.Empty
            Dim sqlParams As SqlParameter() = {New SqlParameter("@FieldName", Trim(Request.QueryString("field"))), New SqlParameter("@FieldValue", Trim(Request.QueryString("value")))}

            If Trim(Request.QueryString("field")) = "title" Then
                If Trim(Request.QueryString("value")) = String.Empty Then
                    lblTitle.Text = "Administration - List of users for academic title <i>empty/blank</i>"
                Else
                    lblTitle.Text = "Administration - List of users for academic title <i>""" & Trim(Request.QueryString("value")) & """</i>"
                End If

                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select vorname,nachname,id,(isnull(vorname,'')+' '+isnull(nachname,'')) fullname,isnull(loginname,'') username,isnull(state,'') state,isnull(land,'') land from benutzer where isnull(titel,'')=@FieldValue order by fullname"
            Else
                If Trim(Request.QueryString("value")) = String.Empty Then
                    lblTitle.Text = "Administration - List of users for " & Trim(Request.QueryString("field")) & " <i>empty/blank</i>"
                Else
                    lblTitle.Text = "Administration - List of users for " & Trim(Request.QueryString("field")) & " <i>""" & Trim(Request.QueryString("value")) & """</i>"
                End If

                sqlQuery = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select vorname,nachname,isnull((select [value] from log_users where id_user=b.id and type='position'),'') position,b.id,(isnull(vorname,'')+' '+isnull(nachname,'')) fullname,isnull(loginname,'') username,isnull(state,'') state,isnull(ort,'') land from log_users lu inner join benutzer b on lu.id_user=b.id where [type]=@FieldName and isnull([value],'')=@FieldValue order by fullname"
            End If

            rptUserList.DataSource = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            rptUserList.DataBind()

            If rptUserList.Items.Count = 0 Then
                rptUserList.Visible = False
                lblErrMsg.Text = "No user(s) found matching your selection criteria."
            Else
                lblTotalUsers.Text = "Total: " & rptUserList.Items.Count & " user(s)"
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptUserList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUserList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)
                With drv
                    If Trim(Request.QueryString("field")).ToString.ToLower = "company" Then
                        CType(e.Item.FindControl("lblField1"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("position"), String.Empty))
                    ElseIf Trim(Request.QueryString("field")).ToString.ToLower = "title" Then
                        CType(e.Item.FindControl("lblField1"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Vorname"), String.Empty))
                        CType(e.Item.FindControl("lblField2"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Nachname"), String.Empty))
                    Else
                        CType(e.Item.FindControl("lblField1"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("state"), String.Empty))
                        CType(e.Item.FindControl("lblField2"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("land"), String.Empty))
                    End If

                    CType(e.Item.FindControl("lblUserName"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("username"), String.Empty))
                    CType(e.Item.FindControl("hypFullName"), HyperLink).Text = Server.HtmlEncode(Utils.Nz(.Item("fullname"), String.Empty))
                    CType(e.Item.FindControl("hypFullName"), HyperLink).NavigateUrl = "users_update.aspx?id=" & Utils.Nz(.Item("id"), 0)
                    CType(e.Item.FindControl("hypEditProfile"), HyperLink).NavigateUrl = "users_update.aspx?id=" & Utils.Nz(.Item("id"), 0)
                End With
            ElseIf e.Item.ItemType = ListItemType.Header Then
                If Trim(Request.QueryString("field")).ToString.ToLower = "company" Then
                    CType(e.Item.FindControl("lblField1"), Label).Text = "Job Position"
                ElseIf Trim(Request.QueryString("field")).ToString.ToLower = "title" Then
                    CType(e.Item.FindControl("lblField1"), Label).Text = "First name"
                    CType(e.Item.FindControl("lblField2"), Label).Text = "Last name"
                Else
                    CType(e.Item.FindControl("lblField1"), Label).Text = "State"
                    CType(e.Item.FindControl("lblField2"), Label).Text = "City"
                End If
            End If

            If e.Item.ItemType <> ListItemType.Footer Then
                If Trim(Request.QueryString("field")).ToString.ToLower = "company" Then
                    Dim rptCell As HtmlTableCell = CType(e.Item.FindControl("tdCity"), HtmlTableCell)
                    If Not rptCell Is Nothing Then rptCell.Visible = False
                End If
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to replace perticular field value with new value
    ''' </summary>
    Public Class ReplaceFieldInfo
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblTitle, lblOLDFieldValue, lblNewFieldValue, lblSaveRule, lblJobTitle As Label
        Protected txtOLDFieldValue, txtNewFieldValue As TextBox
        Protected chkSaveRule, chkJobTitle As CheckBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub ReplaceFieldInfo_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not IsPostBack Then
                    cammWebManagerAdminMenu.HRef = "rules_user_fields.aspx?field=" & Trim(Request.QueryString("field"))
                    If Trim(Request.QueryString("value")) <> String.Empty Then txtOLDFieldValue.Text = Trim(Request.QueryString("value"))
                    If Trim(Request.QueryString("field")) <> "title" Then
                        lblJobTitle.Visible = False
                        chkJobTitle.Visible = False
                        If Trim(Request.QueryString("value")) = String.Empty Then
                            lblTitle.Text = "Administration - Replace " & Trim(Request.QueryString("field")) & " name <i>empty/blank</i>"
                        Else
                            lblTitle.Text = "Administration - Replace " & Trim(Request.QueryString("field")) & " name <i>""" & Trim(Request.QueryString("value")) & """</i>"
                        End If
                    Else
                        If Trim(Request.QueryString("value")) = String.Empty Then
                            lblTitle.Text = "Administration - Replace academic title <i>empty/blank</i>"
                        Else
                            lblTitle.Text = "Administration - Replace academic title <i>""" & Trim(Request.QueryString("value")) & """</i>"
                        End If
                    End If

                    Dim dtRule As DataTable = LoadRuleValuesByFieldAndValue()
                    If Not dtRule Is Nothing AndAlso dtRule.Rows.Count > 0 Then
                        chkSaveRule.Checked = True
                        txtNewFieldValue.Text = Utils.Nz(dtRule.Rows(0)("replacementvalue"), String.Empty)
                    End If
                    If Not dtRule Is Nothing Then dtRule.Dispose()

                    If Utils.Nz(Request.QueryString("rule"), 0) = 1 Then
                        lblJobTitle.Visible = False
                        chkJobTitle.Visible = False
                        chkSaveRule.Enabled = False
                        btnSubmit.Text = "Update"
                    Else
                        txtOLDFieldValue.ReadOnly = True
                    End If
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

#Region "User-Defined Functions"
        Private Function LoadRuleValuesByFieldAndValue() As DataTable
            Dim sqlParams As SqlParameter() = {New SqlParameter("@FieldName", Trim(Request.QueryString("field"))), New SqlParameter("@RuleName", "userfields"), New SqlParameter("@searchvalue", Request.QueryString("value"))}
            Dim sqlQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select isnull(ReplacementValue,'') ReplacementValue,id from ReplaceRulesUserProperties where FieldName=@FieldName and rulename=@RuleName and SearchValue=@searchvalue"
            Return FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
        End Function
#End Region

#Region "Control Events"
        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            Try
                If CInt(Val(Request.QueryString("rule") & "")) = 1 Then
                    'update rule 
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("id") & ""))), New SqlParameter("@SearchValue", txtOLDFieldValue.Text.ToString.Trim), New SqlParameter("@ReplaceValue", txtNewFieldValue.Text.ToString.Trim)}
                    Dim sqlQuery As String = "update ReplaceRulesUserProperties set SearchValue=@SearchValue,ReplacementValue=@ReplaceValue where id=@ID"
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("listallrules.aspx?field=" & Trim(Request.QueryString("field")))
                Else
                    'Update rule and replace all old field values with new values in Log_Users table
                    Dim blnJobTitle As Boolean = False
                    If chkJobTitle.Visible = True AndAlso chkJobTitle.Checked = True Then blnJobTitle = True
                    Dim sqlParams1 As SqlParameter() = {New SqlParameter("@FieldName", Trim(Request.QueryString("field"))), New SqlParameter("@RuleName", "userfields"), New SqlParameter("@SaveRule", chkSaveRule.Checked), New SqlParameter("@SearchValue", txtOLDFieldValue.Text.ToString.Trim), New SqlParameter("@ReplaceValue", txtNewFieldValue.Text.ToString.Trim), New SqlParameter("@ReplaceJobTitle", blnJobTitle), New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@RemoteIP", cammWebManager.CurrentServerIdentString), New SqlParameter("@ServerIP", cammWebManager.CurrentRemoteClientAddress), New SqlParameter("@SecurityObject", cammWebManager.SecurityObject.ToString), New SqlParameter("@LocationID", cammWebManager.CurrentServerInfo.ID), New SqlParameter("@LangID", cammWebManager.UIMarketInfo.AlternativeLanguageInfo.ID), New SqlParameter("@RuleID", 0.0)}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_RulesUserFields", CommandType.StoredProcedure, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("rules_user_fields.aspx?field=" & Trim(Request.QueryString("field")))
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to view the list of rules, also rule can be edited or applied
    ''' </summary>
    Public Class ListAllRules
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblTitle As Label
        Protected WithEvents rptFieldList As Repeater
        Protected WithEvents btnApplyChanges As Button
        Protected WithEvents dropFieldSelection As DropDownList
#End Region

#Region "Page Events"
        Private Sub UserFieldDetails_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = String.Empty

            If Not IsPostBack Then
                Try
                    FillFieldDropDownList()
                    DeleteRule()
                    ApplyPerticularRule()
                    ListOfRuleDetails()
                    lblTitle.Text = "Administration - Rules Per " & Trim(dropFieldSelection.SelectedItem.Text)
                    cammWebManagerAdminMenu.HRef = "rules_user_fields.aspx?field=" & Trim(dropFieldSelection.SelectedValue)
                Catch ex As Exception
                    Throw
                End Try
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        Private Sub ApplyPerticularRule()
            'Apply single rule which is requested
            If CInt(Val(Request.QueryString("apply") & "")) = 1 Then
                Dim sqlParams1 As SqlParameter() = {New SqlParameter("@FieldName", Trim(Request.QueryString("field"))), New SqlParameter("@RuleName", "userfields"), New SqlParameter("@SaveRule", False), New SqlParameter("@SearchValue", String.Empty), New SqlParameter("@ReplaceValue", String.Empty), New SqlParameter("@ReplaceJobTitle", False), New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@RemoteIP", cammWebManager.CurrentServerIdentString), New SqlParameter("@ServerIP", cammWebManager.CurrentRemoteClientAddress), New SqlParameter("@SecurityObject", cammWebManager.SecurityObject.ToString), New SqlParameter("@LocationID", cammWebManager.CurrentServerInfo.ID), New SqlParameter("@LangID", cammWebManager.UIMarketInfo.AlternativeLanguageInfo.ID), New SqlParameter("@RuleID", Trim(Request.QueryString("id")))}
                ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_RulesUserFields", CommandType.StoredProcedure, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Response.Redirect("rules_user_fields.aspx?field=" & Trim(Request.QueryString("field")))
            End If
        End Sub

        Private Sub DeleteRule()
            'Delete single rule which is requested
            If CInt(Val(Request.QueryString("delete") & "")) = 1 Then
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", Trim(Request.QueryString("id")))}
                Dim sqlQuery As String = "delete ReplaceRulesUserProperties where id=@ID"
                ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        Private Sub FillFieldDropDownList()
            If dropFieldSelection.Items.Count > 0 Then dropFieldSelection.Items.Clear()

            dropFieldSelection.Items.Add(New ListItem("Academic Title", "title"))
            dropFieldSelection.Items.Add(New ListItem("Company", "company"))
            dropFieldSelection.Items.Add(New ListItem("Country", "country"))
            dropFieldSelection.Items.Add(New ListItem("State", "state"))

            'if redirected from replace page then it will select that field again in dropdownlist
            If Trim(Request.QueryString("field")) <> String.Empty Then dropFieldSelection.SelectedValue = Trim(Request.QueryString("field"))
        End Sub

        Private Function GetListOfRulesByField() As DataTable
            Dim sqlParams As SqlParameter() = {New SqlParameter("@FieldName", Trim(dropFieldSelection.SelectedValue)), New SqlParameter("@RuleName", "UserFields")}
            Dim sqlQuery As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select ID,RuleName,FieldName,SearchValue,ReplacementValue from ReplaceRulesUserProperties where FieldName=@FieldName and rulename=@RuleName"
            Return FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sqlQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
        End Function

        Private Sub ListOfRuleDetails()
            If rptFieldList.Visible = False Then rptFieldList.Visible = True
            If btnApplyChanges.Enabled = False Then btnApplyChanges.Enabled = True

            rptFieldList.DataSource = GetListOfRulesByField()
            rptFieldList.DataBind()

            If rptFieldList.Items.Count = 0 Then
                btnApplyChanges.Enabled = False
                rptFieldList.Visible = False
                lblErrMsg.Text = "No rule(s) found for selected field """ & dropFieldSelection.SelectedValue.ToString & """"
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptFieldList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptFieldList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim drv As DataRowView = CType(e.Item.DataItem, DataRowView)
                With drv
                    CType(e.Item.FindControl("lblSearchValue"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("SearchValue"), String.Empty))
                    CType(e.Item.FindControl("lblReplaceValue"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ReplacementValue"), String.Empty))
                    CType(e.Item.FindControl("lnkEdit"), HyperLink).NavigateUrl = "replacefieldinfo.aspx?field=" & Trim(dropFieldSelection.SelectedValue) & "&value=" & Utils.Nz(.Item("SearchValue"), String.Empty).Trim & "&rule=1&id=" & Utils.Nz(.Item("id"), 0)
                    CType(e.Item.FindControl("lnkDelete"), HyperLink).NavigateUrl = "listallrules.aspx?field=" & Trim(dropFieldSelection.SelectedValue) & "&delete=1" & "&id=" & Utils.Nz(.Item("id"), 0)
                    CType(e.Item.FindControl("lnkApply"), HyperLink).NavigateUrl = "listallrules.aspx?field=" & Trim(dropFieldSelection.SelectedValue) & "&id=" & Utils.Nz(.Item("id"), 0) & "&apply=1"
                End With
            End If
        End Sub

        Private Sub btnApplyChanges_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApplyChanges.Click
            Try
                'Replace all records according to existing rules
                Dim sqlParams As SqlParameter() = {New SqlParameter("@FieldName", Trim(dropFieldSelection.SelectedValue)), New SqlParameter("@RuleName", "userfields"), New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@RemoteIP", cammWebManager.CurrentServerIdentString), New SqlParameter("@ServerIP", cammWebManager.CurrentRemoteClientAddress), New SqlParameter("@SecurityObject", cammWebManager.SecurityObject.ToString), New SqlParameter("@LocationID", cammWebManager.CurrentServerInfo.ID), New SqlParameter("@LangID", cammWebManager.UIMarketInfo.AlternativeLanguageInfo.ID)}
                ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_RulesUserFieldsApplyAll", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Response.Redirect("rules_user_fields.aspx?field=" & Trim(dropFieldSelection.SelectedValue))
            Catch ex As Exception
                Throw
            End Try
        End Sub

        Private Sub dropFieldSelection_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dropFieldSelection.SelectedIndexChanged
            Try
                ListOfRuleDetails()
                cammWebManagerAdminMenu.HRef = "rules_user_fields.aspx?field=" & Trim(dropFieldSelection.SelectedValue)
                lblTitle.Text = "Administration - Rules Per " & Trim(dropFieldSelection.SelectedItem.Text)
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

    End Class

End Namespace


