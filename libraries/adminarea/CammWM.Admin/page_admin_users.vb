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
    '''     Reset the user's password
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class ResetUserPassword
        Inherits Page

        Protected displayloginname As String
        Protected ErrMsg As String

        Sub PageLoad()
            displayloginname = Request.Form("loginname")

            If Request.Form("newpassword") <> "" Then

                Dim LoginIDOfUser As Long = Utils.Nz(cammWebManager.System_GetUserID(Request.Form("loginname")), -1&)
                If LoginIDOfUser = -1 Then
                    ErrMsg = "Unknown login name """ & Request.Form("loginname") & """" & cammWebManager.System_GetUserID(Request.Form("loginname")).ToString
                Else
                    Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = cammWebManager.System_GetUserInfo(LoginIDOfUser)

                    Select Case cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ValidatePasswordComplexity(Request.Form("newpassword"), MyUserInfo)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                            ErrMsg = cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ErrorMessageComplexityPoints(1)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                            ErrMsg = "The password requires to be not bigger than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredMaximumPasswordLength & " characters!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                            ErrMsg = "The password requires to be not smaller than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredPasswordLength & " characters!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                            ErrMsg = "The password shouldn't contain pieces of the user account profile, especially login name, first or last name!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_Unspecified
                            ErrMsg = "There are some unknown errors when validating with the security policy for passwords!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
                            cammWebManager.System_SetUserPassword(MyUserInfo, Request.Form("newpassword"))
                            displayloginname = ""

                            'Send e-mail with new password to user
                            ErrMsg = "Password successfully changed for user """ & MyUserInfo.LoginName & """ (" & MyUserInfo.FullName & ")!"
                    End Select
                End If

            ElseIf Request.Form("newpassword") = "" And Request.Form("loginname") <> "" Then
                ErrMsg = "Please enter a new password for the user!"
            End If

        End Sub

    End Class

    ''' <summary>
    '''     The users overview administration page
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	27.08.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UsersOverview
        Inherits Page

#Region "Page Events"
        Protected Sub ActionExport(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            If Request.QueryString("action") = "export" Then
                If Request.QueryString("userid") = Nothing Then
                    CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.Users(cammWebManager, cammWebManager.System_GetUserInfos()))
                ElseIf Request.QueryString("userid") <> Nothing Then
                    CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(Me.cammWebManager, CompuMaster.camm.WebManager.Administration.Export.Users(cammWebManager, cammWebManager.System_GetUserInfos(New Long() {Utils.TryCLng(Request.QueryString("userid"))})))
                End If
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to view a list of users
    ''' </summary>
    Public Class UserList
        Inherits UsersOverview

#Region "Variable Declaration"
        Protected lblErrMsg, lblNoRecMsg, lblID, lblCompany, lblLoginName, lblAccessLevelTitle, lblLand, lblState, lblLastLoginOn, lblLastLoginViaRemoteIP, lblCreatedOn, lblModifiedOn As Label
        Protected ancUserNameComplete, ancUpdate, ancDelete, ancClone As HtmlAnchor
        Protected gcDisabled As HtmlGenericControl
        Protected WithEvents rptUserList As Repeater
        Protected SearchUsersTextBox As TextBox
        Protected WithEvents CheckBoxTop50Results As CheckBox
        Dim MyDt As New DataTable
#End Region

#Region "Page Events"
        Private Sub UserList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""
            lblNoRecMsg.Text = ""
        End Sub

        Private Sub UserList_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            ListOfUsers()
            'SearchUsersTextBox.Attributes.Add("onblur", "return demoMatchClick('" & SearchUsersTextBox.ClientID & "');")
        End Sub
#End Region

#Region "User-Defined Methods"
        Protected Sub ListOfUsers()
            Dim WhereClause As String

            Dim SearchWords As String = SearchUsersTextBox.Text.Replace("'", "''").Replace("*", "%").Trim.Replace("  ", " ")
            SearchWords = "%" & SearchWords & "%"
            SearchWords = SearchWords.Replace(" ", "% %")
            Dim searchItems As String() = SearchWords.Split(" "c)
            Dim SingleNameItems As String = String.Empty
            Dim IsSingelName As Boolean = False


            'Search for the hole searchword in every column
            WhereClause = "Where (Loginname LIKE @SearchWords Or Namenszusatz LIKE @SearchWords Or company LIKE @SearchWords or Nachname LIKE @SearchWords Or Vorname LIKE @SearchWords Or [E-Mail] LIKE @SearchWords)"

            'Search for every single word (space-seperated) in searchword
            If searchItems.Length > 1 Then
                WhereClause &= " OR ("
                For myWordCounter As Integer = 0 To searchItems.Length - 1
                    If myWordCounter <> 0 Then WhereClause &= " AND "
                    WhereClause &= " (Loginname LIKE @SearchItem" & myWordCounter & " Or Namenszusatz LIKE @SearchItem" & myWordCounter & " Or company LIKE @SearchItem" & myWordCounter & " or Nachname LIKE @SearchItem" & myWordCounter & " Or Vorname LIKE @SearchItem" & myWordCounter & " Or [E-Mail] LIKE @SearchItem" & myWordCounter & ")"
                Next
                WhereClause &= " )"
            End If

            Dim TopClause As String = ""
            If CheckBoxTop50Results.Checked = True Then TopClause = "TOP 50" Else TopClause = ""

            Try
                If SearchWords = String.Empty Then WhereClause = String.Empty
                Dim SqlQuery As String = "SELECT " & TopClause & " System_AccessLevels.Title As AccessLevel_Title, Benutzer.*, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS UserNameComplete FROM [Benutzer] LEFT JOIN System_AccessLevels ON Benutzer.AccountAccessability = System_AccessLevels.ID " & WhereClause & " ORDER BY Nachname, Vorname"

                Dim cmd As New SqlClient.SqlCommand(SqlQuery, New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@SearchWords", SqlDbType.NVarChar).Value = SearchWords
                For myWordCounter2 As Integer = 0 To searchItems.Length - 1
                    cmd.Parameters.Add("@SearchItem" & myWordCounter2, SqlDbType.NVarChar).Value = searchItems(myWordCounter2)
                Next
                MyDt = FillDataTable(cmd, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptUserList.DataSource = MyDt
                    rptUserList.DataBind()
                Else
                    Dim drow As DataRow
                    drow = MyDt.NewRow
                    MyDt.Rows.Add(drow)
                    rptUserList.DataSource = MyDt
                    rptUserList.DataBind()
                    rptUserList.Items(0).Visible = False
                    lblNoRecMsg.Text = "No records found matching your search request."
                End If
            Catch ex As Exception
                Throw
            Finally
                MyDt.Dispose()
            End Try
        End Sub

#End Region

#Region "Control Events"
        Private Sub rptUserListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUserList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                With MyDt.Rows(e.Item.ItemIndex)
                    CType(e.Item.FindControl("lblID"), Label).Text = Utils.Nz(.Item("ID"), 0).ToString
                    If Not IsDBNull(.Item("LoginDisabled")) Then If Utils.Nz(.Item("LoginDisabled"), False) = True Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).InnerHtml = "<nobr title=""Disabled"">(D)</nobr>"
                    CType(e.Item.FindControl("ancUserNameComplete"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancUserNameComplete"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(.Item("Vorname"), .Item("Nachname")))
                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Company"), String.Empty))
                    CType(e.Item.FindControl("lblLoginName"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("lblLoginName"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("LoginName"), String.Empty))
                    CType(e.Item.FindControl("lblAccessLevelTitle"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("AccessLevel_Title"), String.Empty))
                    CType(e.Item.FindControl("lblLand"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Land"), String.Empty))
                    CType(e.Item.FindControl("lblState"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("State"), String.Empty))
                    CType(e.Item.FindControl("lblLastLoginOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("LastLoginOn"), String.Empty))
                    CType(e.Item.FindControl("lblLastLoginViaRemoteIP"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("LastLoginViaRemoteIP"), String.Empty))
                    CType(e.Item.FindControl("lblCreatedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("CreatedOn"), String.Empty))
                    CType(e.Item.FindControl("lblModifiedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ModifiedOn"), String.Empty))
                    CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancDelete"), HtmlAnchor).HRef = "users_delete.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancClone"), HtmlAnchor).HRef = "users_clone.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                End With
            End If
        End Sub

#End Region

    End Class

    ''' <summary>
    '''     A page to create a new user
    ''' </summary>
    Public Class New_Users
        Inherits ImportBase


#Region "Variable Declaration"
#Region "Control_Declaration"
        Protected lblMsg As Label
        Protected txtPhone, txtFax, txtMobile, txtPosition, txtLoginName, txtPassword1, txtPassword2, txtCompany As TextBox
        Protected cmbAnrede As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected txtTitle, txtVorname, txtNachname, txtNamenszusatz, txtemail, txtStrasse, txtPLZ, txtOrt, txtState, txtLand As TextBox
        Protected cmbFirstPreferredLanguage, cmbSecondPreferredLanguage, cmbThirdPreferredLanguage, cmbAccountAccessability As DropDownList
#End Region
#Region "Variable Declaration"
        Dim Field_Phone, Field_Fax, Field_Mobile, Field_Position, Field_LoginName, Field_Password1, Field_Password2 As String
        Dim Field_Company, Field_Anrede, Field_Titel, Field_Vorname, Field_Nachname, Field_Namenszusatz, Field_e_mail As String
        Dim Field_Strasse, Field_PLZ, Field_Ort, Field_State, Field_Land As String
        Dim Field_1stPreferredLanguage, Field_2ndPreferredLanguage, Field_3rdPreferredLanguage, Field_AccountAccessable As Integer
#End Region
#Region "Declaration For UpdateRecord"
        Dim MyCount As Object
        Dim ErrMsg As String
        Dim UpdateSuccessfull As Boolean
        Dim LoginIDOfUser As Integer
        Dim NewUserID As Long
        Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
        Dim MyUserInfoAdditionalFlags As New Collections.Specialized.NameValueCollection
        Dim MyUserInfoSex As CompuMaster.camm.WebManager.WMSystem.Sex
#End Region
#End Region

#Region "Page Event"
        Private Sub New_Users_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim temp1 As String
                If cmbAnrede.Items.Count > 0 Then
                    If cmbAnrede.SelectedItem.Text = "Ms." Then
                        MyUserInfoSex = CompuMaster.camm.WebManager.WMSystem.Sex.Feminine
                        temp1 = Utils.Nz(MyUserInfoSex, String.Empty)
                    ElseIf cmbAnrede.SelectedItem.Text = "Mr." Then
                        MyUserInfoSex = CompuMaster.camm.WebManager.WMSystem.Sex.Masculine
                        temp1 = Utils.Nz(MyUserInfoSex, String.Empty)
                    End If
                End If
            Catch ex As Exception
                ErrMsg = ex.Message
            End Try

            Update_Data(UpdateSuccessfull, ErrMsg)

            If ErrMsg <> "" Then lblMsg.Text = ErrMsg
            txtLoginName.Text = Utils.Nz(Field_LoginName, String.Empty)
            txtPassword1.Text = Utils.Nz(Field_Password1, String.Empty)
            txtPassword1.TextMode = TextBoxMode.Password
            txtPassword2.Text = Utils.Nz(Field_Password2, String.Empty)
            txtPassword2.TextMode = TextBoxMode.Password
            txtCompany.Text = Utils.Nz(Field_Company, String.Empty)
            txtTitle.Text = Utils.Nz(Field_Titel, String.Empty)
            txtVorname.Text = Utils.Nz(Field_Vorname, String.Empty)
            txtNachname.Text = Utils.Nz(Field_Nachname, String.Empty)
            txtNamenszusatz.Text = Utils.Nz(Field_Namenszusatz, String.Empty)
            txtemail.Text = Utils.Nz(Field_e_mail, String.Empty)
            txtStrasse.Text = Utils.Nz(Field_Strasse, String.Empty)
            txtPLZ.Text = Utils.Nz(Field_PLZ, String.Empty)
            txtOrt.Text = Utils.Nz(Field_Ort, String.Empty)
            txtState.Text = Utils.Nz(Field_State, String.Empty)
            txtLand.Text = Utils.Nz(Field_Land, String.Empty)
            txtPhone.Text = Utils.Nz(Field_Phone, String.Empty)
            txtFax.Text = Utils.Nz(Field_Fax, String.Empty)
            txtMobile.Text = Utils.Nz(Field_Mobile, String.Empty)
            txtPosition.Text = Utils.Nz(Field_Position, String.Empty)
            If Not IsPostBack Then FillDropDownList(Field_AccountAccessable.ToString, Field_1stPreferredLanguage.ToString, Field_2ndPreferredLanguage.ToString, Field_3rdPreferredLanguage.ToString)
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If txtLoginName.Text <> "" AndAlso _
               txtPassword1.Text <> "" AndAlso _
               txtPassword2.Text <> "" AndAlso _
               txtPassword1.Text = txtPassword2.Text AndAlso _
               txtemail.Text <> "" AndAlso _
               cmbFirstPreferredLanguage.SelectedItem.Text <> "Please Select!" AndAlso _
               cmbAccountAccessability.SelectedItem.Text <> "Please Select!" Then

                If Me.cmbAnrede.SelectedValue = "1" Then
                    MyUserInfoSex = WMSystem.Sex.Masculine
                ElseIf Me.cmbAnrede.SelectedValue = "2" Then
                    MyUserInfoSex = WMSystem.Sex.Feminine
                Else
                    MyUserInfoSex = WMSystem.Sex.Undefined
                End If

                MyUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(Nothing, (Trim(txtLoginName.Text & "")), (Trim(txtemail.Text & "")), False, Trim(txtCompany.Text & ""), MyUserInfoSex, Trim(txtNamenszusatz.Text & ""), Trim(txtVorname.Text & ""), Trim(txtNachname.Text & ""), Trim(txtTitle.Text & ""), Trim(txtStrasse.Text & ""), Trim(txtPLZ.Text & ""), Trim(txtOrt.Text & ""), Trim(txtState.Text & ""), Trim(txtLand.Text & ""), Utils.Nz(IIf(cmbFirstPreferredLanguage.SelectedValue = "", 0, cmbFirstPreferredLanguage.SelectedValue), 0), Utils.Nz(IIf(cmbSecondPreferredLanguage.SelectedValue = "", 0, cmbSecondPreferredLanguage.SelectedValue), 0), Utils.Nz(IIf(cmbThirdPreferredLanguage.SelectedValue = "", 0, cmbThirdPreferredLanguage.SelectedValue), 0), False, False, False, CInt(Val(cmbAccountAccessability.SelectedValue & "")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem), "", CType(MyUserInfoAdditionalFlags, Collections.Specialized.NameValueCollection))
                MyUserInfo.AdditionalFlags("ComesFrom") = "Account created by Admin """ + cammWebManager.CurrentUserInfo.LoginName + """ (" + cammWebManager.CurrentUserInfo.FullName + ")"
                MyUserInfo.AdditionalFlags("Motivation") = "Account created by Admin"

                'Update record
                Select Case cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ValidatePasswordComplexity(txtPassword1.Text, MyUserInfo)
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                        ErrMsg = "The password doesn't match the current security policy for passwords!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                        ErrMsg = "The password requires to be not bigger than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredMaximumPasswordLength & " characters!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                        ErrMsg = "The password requires to be not smaller than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredPasswordLength & " characters!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                        ErrMsg = "The password shouldn't contain pieces of the user account profile, especially login name, first or last name!"
                    Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
                        Try
                            Dim dt As DataTable
                            Dim sqlParams As SqlParameter() = {New SqlParameter("@Loginname", Trim(txtLoginName.Text))}
                            dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select count(*) from Benutzer where Loginname=@Loginname", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                            Dim sqlParamsNew As SqlParameter() = {New SqlParameter("@Loginname", Trim(txtLoginName.Text))}
                            MyCount = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select count(*) from Benutzer where Loginname=@Loginname", CommandType.Text, sqlParamsNew, Automations.AutoOpenAndCloseAndDisposeConnection)

                            If Utils.Nz(MyCount, 0) > 0 Then
                                ErrMsg = "User account already exists!"
                                UpdateSuccessfull = False
                                ErrMsg = "User account already exists!"
                            Else
                                Dim NewPassword As String = txtPassword1.Text
                                NewUserID = cammWebManager.System_SetUserInfo(MyUserInfo, NewPassword)
                                Dim tempstr As String

                                tempstr = ""
                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Phone', @Value = N'" & Trim(Request.Form("Phone")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams1 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Phone"), New SqlParameter("@Value", txtPhone.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Fax', @Value = N'" & Trim(Request.Form("Fax")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams2 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Fax"), New SqlParameter("@Value", txtFax.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams2, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Mobile', @Value = N'" & Trim(Request.Form("Mobile")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams3 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Mobile"), New SqlParameter("@Value", txtMobile.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams3, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                dt = FillDataTable(New SqlCommand("exec Public_SetUserDetailData @IDUser = " & NewUserID & ", @Type = N'Position', @Value = N'" & Trim(Request.Form("Position")) & "', @DoNotLogSuccess = 1", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                                Dim sqlParams4 As SqlParameter() = {New SqlParameter("@IDUser", NewUserID), New SqlParameter("@Type", "Position"), New SqlParameter("@Value", txtPosition.Text), New SqlParameter("@DoNotLogSuccess", 1)}
                                tempstr = Utils.Nz(ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "Public_SetUserDetailData", CommandType.StoredProcedure, sqlParams4, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String.Empty)

                                ErrMsg = "The profile has been created successfully!"
                                UpdateSuccessfull = True
                            End If
                        Catch ex As Exception
                            Throw
                        End Try
                    Case Else
                        ErrMsg = "There are some unknown errors when validating with the security policy for passwords!"
                End Select
            Else
                ErrMsg = "Please specify a unique logon name, the password inclusive the confirmation, the complete name and address and at least one language preference to proceed!"
            End If
            Update_Data(UpdateSuccessfull, ErrMsg)
            If ErrMsg <> "" Then
                lblMsg.Text = ErrMsg
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
        Sub Update_Data(ByVal Update_d As Boolean, ByVal ErrMsg As String)
            If Update_d = True Then
                Response.Redirect("users.aspx")
            Else
                Field_LoginName = txtLoginName.Text
                Field_Password1 = txtPassword1.Text
                Field_Password2 = txtPassword2.Text
                Field_Company = txtCompany.Text
                If cmbAnrede.Items.Count > 0 Then
                    Field_Anrede = cmbAnrede.SelectedValue
                End If
                Field_Titel = txtTitle.Text
                Field_Vorname = txtVorname.Text
                Field_Nachname = txtNachname.Text
                Field_Namenszusatz = txtNamenszusatz.Text
                Field_e_mail = txtemail.Text
                Field_Strasse = txtStrasse.Text
                Field_PLZ = txtPLZ.Text.ToString()
                Field_Ort = txtOrt.Text.ToString()
                Field_State = txtState.Text
                Field_Land = txtLand.Text
                Field_Phone = txtPhone.Text.ToString()
                Field_Fax = txtFax.Text.ToString()
                Field_Mobile = txtMobile.Text.ToString()
                Field_Position = txtPosition.Text

                If cmbFirstPreferredLanguage.Items.Count > 0 Then
                    Field_1stPreferredLanguage = Utils.Nz(cmbFirstPreferredLanguage.SelectedValue, 0)
                End If
                If cmbSecondPreferredLanguage.Items.Count > 0 Then
                    Field_2ndPreferredLanguage = Utils.Nz(cmbSecondPreferredLanguage.SelectedValue, 0)
                End If
                If cmbThirdPreferredLanguage.Items.Count > 0 Then
                    Field_3rdPreferredLanguage = Utils.Nz(cmbThirdPreferredLanguage.SelectedValue, 0)
                End If
                If cmbAccountAccessability.Items.Count > 0 Then
                    Field_AccountAccessable = Utils.Nz(cmbAccountAccessability.SelectedValue, 0)
                End If

                If Not txtLoginName.Text = "" AndAlso _
                  txtPassword1.Text = "" AndAlso _
                  txtPassword2.Text = "" AndAlso _
                  txtCompany.Text = "" AndAlso _
                  cmbAnrede.SelectedItem.Text = "" AndAlso _
                  txtTitle.Text = "" AndAlso _
                  txtVorname.Text = "" AndAlso _
                  txtNachname.Text = "" AndAlso _
                  txtNamenszusatz.Text = "" AndAlso _
                  txtemail.Text = "" AndAlso _
                  txtStrasse.Text = "" AndAlso _
                  txtPLZ.Text = "" AndAlso _
                  txtOrt.Text = "" AndAlso _
                  txtState.Text = "" AndAlso _
                  txtLand.Text = "" AndAlso _
                  cmbFirstPreferredLanguage.SelectedItem.Text = "" AndAlso _
                  cmbSecondPreferredLanguage.SelectedItem.Text = "" AndAlso _
                  cmbThirdPreferredLanguage.SelectedItem.Text = "" AndAlso _
                  cmbAccountAccessability.SelectedItem.Text = "" AndAlso _
                  txtPassword1.Text = "" AndAlso _
                  txtPassword2.Text = "" Then
                    If ErrMsg = "" Then ErrMsg = "Please specify a unique logon name, the password inclusive the confirmation, the complete name and address and at least one language preference to proceed!"
                End If
            End If
        End Sub

        Sub FillDropDownList(ByVal Field_AccountAccessable As String, ByVal Field_1stPreferredLanguage As String, ByVal Field_2ndPreferredLanguage As String, ByVal Field_3rdPreferredLanguage As String)
            'Fill Anrede DropDownList            
            cmbAnrede.Items.Clear()
            cmbAnrede.Items.Add(New ListItem(Nothing, Nothing))
            cmbAnrede.Items.Add("Mr.")
            cmbAnrede.Items.Add("Ms.")
            Dim temp As String
            If Field_Anrede = "" Then
                cmbAnrede.SelectedIndex = 0
                temp = cmbAnrede.SelectedItem.Text
            End If
            If Field_Anrede = "Mr." Then
                cmbAnrede.SelectedIndex = 1
                temp = cmbAnrede.SelectedItem.Text
            End If
            If Field_Anrede = "Ms." Then
                cmbAnrede.SelectedIndex = 2
                temp = cmbAnrede.SelectedItem.Text
            End If

            'Fill AccountAccessability DropDownList
            cmbAccountAccessability.Items.Clear()
            cmbAccountAccessability.Items.Add("Please Select!")
            If Field_AccountAccessable = Nothing Then
                cmbAccountAccessability.SelectedIndex = 2
            End If
            Dim ExistentAccessLevels As CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation()
            ExistentAccessLevels = cammWebManager.System_GetAccessLevelInfos()
            For Each ExistentAccessLevel As CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation In ExistentAccessLevels
                cmbAccountAccessability.Items.Add(New ListItem(Server.HtmlEncode(ExistentAccessLevel.Title), Utils.Nz(ExistentAccessLevel.ID, 0).ToString))
                If Utils.Nz(Field_AccountAccessable, 0) = ExistentAccessLevel.ID Then
                    cmbAccountAccessability.SelectedValue = Utils.Nz(ExistentAccessLevel.ID, 0).ToString
                End If
            Next

            'Fill Field_1stPreferredLanguage DropDownList
            cmbFirstPreferredLanguage.Items.Clear()
            cmbFirstPreferredLanguage.Items.Add(New ListItem("Please Select!", "-1"))
            If Field_1stPreferredLanguage = Nothing Then
                cmbFirstPreferredLanguage.SelectedIndex = 0
            End If
            Dim ActiveLanguages As CompuMaster.camm.WebManager.WMSystem.LanguageInformation()
            ActiveLanguages = cammWebManager.System_GetLanguagesInfo(False)
            For Each MyActiveLanguage As CompuMaster.camm.WebManager.WMSystem.LanguageInformation In ActiveLanguages
                cmbFirstPreferredLanguage.Items.Add(New ListItem(MyActiveLanguage.LanguageName_English, Utils.Nz(MyActiveLanguage.ID, 0).ToString))
                If Utils.Nz(Field_1stPreferredLanguage, 0) = MyActiveLanguage.ID Then
                    cmbFirstPreferredLanguage.SelectedValue = Utils.Nz(MyActiveLanguage.ID, 0).ToString
                End If
            Next

            'Fill Field_2ndPreferredLanguage DropDownList
            cmbSecondPreferredLanguage.Items.Clear()
            cmbSecondPreferredLanguage.Items.Add(New ListItem("", "-1"))
            cmbSecondPreferredLanguage.SelectedIndex = 0
            For Each MyActiveLanguage As CompuMaster.camm.WebManager.WMSystem.LanguageInformation In ActiveLanguages
                cmbSecondPreferredLanguage.Items.Add(New ListItem(MyActiveLanguage.LanguageName_English, Utils.Nz(MyActiveLanguage.ID, 0).ToString))
                If Utils.Nz(Field_2ndPreferredLanguage, 0) = MyActiveLanguage.ID Then
                    cmbSecondPreferredLanguage.SelectedValue = Utils.Nz(MyActiveLanguage.ID, String.Empty)
                End If
            Next

            'Fill Field_3rdPreferredLanguage DropDownList
            cmbThirdPreferredLanguage.Items.Clear()
            cmbThirdPreferredLanguage.Items.Add(New ListItem("", "-1"))
            cmbThirdPreferredLanguage.SelectedIndex = 0
            For Each MyActiveLanguage As CompuMaster.camm.WebManager.WMSystem.LanguageInformation In ActiveLanguages
                cmbThirdPreferredLanguage.Items.Add(New ListItem(MyActiveLanguage.LanguageName_English, Utils.Nz(MyActiveLanguage.ID, 0).ToString))
                If Utils.Nz(Field_3rdPreferredLanguage, 0) = MyActiveLanguage.ID Then
                    cmbThirdPreferredLanguage.SelectedValue = Utils.Nz(MyActiveLanguage.ID, 0).ToString
                End If
            Next
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to delete a user
    ''' </summary>
    Public Class User_Delete
        Inherits ImportBase

#Region "Variable Declaration"
        Dim ErrMsg As String
        Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
        Dim Dt As New DataTable
        Dim MyCount As Integer
        Dim Odd As Boolean
#End Region

#Region "Control Declaration"
        Protected lblID, lblLoginName, lblCompany, lblAnrede, lblTitel, lblVorname, lblNachname, lblNamenszusatz As Label
        Protected lblStrasse, lblPLZ, lblORT, lblState, lblLand, lblLoginCount, lblLoginFailures, lblLoginLockedTill, lblLoginDisabled As Label
        Protected lblAccountAccessability, lblCreatedOn, lblModifiedOn, lblLastLoginOn, lblLastLoginViaremoteIP, lblExtAccount, lblMsg As Label
        Protected lblFirstPreferredLanguage, lblSecondPreferredLanguage, lblThirdPreferredLanguage As Label
        Protected WithEvents rptUserDelete As Repeater
        Protected ancEmail, ancDelete, ancTouch As HtmlAnchor
        Protected cammWebManagerAdminUserInfoDetails As camm.WebManager.Controls.Administration.UsersAdditionalInformation
        Protected phConfirmDeletion As PlaceHolder
#End Region

#Region "Page Event"

        Private Function GetToken() As String
            Return Session.SessionID
        End Function

        Private Sub SetUserDoesNotExistErrorMessage()
            lblMsg.Text = "Error: This user doesn't exist"
            ancDelete.Visible = False
            ancTouch.Visible = False
            phConfirmDeletion.Visible = False
        End Sub

        Private Sub User_Delete_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim token As String = GetToken()
            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = token Then
                Try
                    MyUserInfo = cammWebManager.System_GetUserInfo(CType(Request.QueryString("ID"), Long))
                    If MyUserInfo Is Nothing Then
                        SetUserDoesNotExistErrorMessage()
                        Return
                    End If
                    MyUserInfo.LoginDeleted = True
                    cammWebManager.System_SetUserInfo(MyUserInfo)
                Catch ex As Exception
                    ErrMsg = "User erasing failed!"
                    If cammWebManager.System_DebugLevel >= 3 Then ErrMsg &= " (" & ex.Message & ")"
                End Try
                Response.Redirect("users.aspx")
            End If

            Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Request.QueryString("ID")))}
            Dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM [Benutzer] WHERE ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            MyUserInfo = cammWebManager.System_GetUserInfo(CType(Request.QueryString("ID"), Long))
            If MyUserInfo Is Nothing Then
                SetUserDoesNotExistErrorMessage()
                Return
            End If

            If ErrMsg <> "" Then
                lblMsg.Text = ErrMsg
            End If

            If True Then
                rptUserDelete.DataSource = Dt
                rptUserDelete.DataBind()
            End If

            cammWebManagerAdminUserInfoDetails.MyUserInfo = MyUserInfo

            ancDelete.HRef = "users_delete.aspx?ID=" & Request.QueryString("ID") & "&DEL=NOW&token=" & token
            ancTouch.HRef = "users.aspx"
        End Sub
#End Region

#Region "Control Event"
        Private Sub rptUserDelete_ItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUserDelete.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                If Dt.Rows.Count > 0 Then
                    With Dt.Rows(e.Item.ItemIndex)
                        For MyCount = 0 To Dt.Columns.Count - 1
                            Select Case UCase(Dt.Columns.Item(MyCount).ToString())
                                Case "ID"
                                    CType(e.Item.FindControl("lblID"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                Case "LOGINNAME"
                                    CType(e.Item.FindControl("lblLoginName"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINPW"
                                Case "CUSTOMERNO"
                                Case "SUPPLIERNO"

                                Case "COMPANY"
                                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ANREDE"
                                    CType(e.Item.FindControl("lblAnrede"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "TITEL"
                                    CType(e.Item.FindControl("lblTitel"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "VORNAME"
                                    CType(e.Item.FindControl("lblVorname"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "NACHNAME"
                                    CType(e.Item.FindControl("lblNachname"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "NAMENSZUSATZ"
                                    CType(e.Item.FindControl("lblNamenszusatz"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "E-MAIL"
                                    CType(e.Item.FindControl("ancEmail"), HtmlAnchor).HRef = "mailto:= " & Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                    CType(e.Item.FindControl("ancEmail"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                Case "STRASSE"
                                    CType(e.Item.FindControl("lblStrasse"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "PLZ"
                                    CType(e.Item.FindControl("lblPLZ"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ORT"
                                    CType(e.Item.FindControl("lblORT"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "STATE"
                                    CType(e.Item.FindControl("lblState"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LAND"
                                    CType(e.Item.FindControl("lblLand"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINCOUNT"
                                    CType(e.Item.FindControl("lblLoginCount"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINFAILURES"
                                    CType(e.Item.FindControl("lblLoginFailures"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINLOCKEDTILL"
                                    CType(e.Item.FindControl("lblLoginLockedTill"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINDISABLED"
                                    CType(e.Item.FindControl("lblLoginDisabled"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ACCOUNTACCESSABILITY"
                                    Dim MyAccessLevel As New CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                    CType(e.Item.FindControl("lblAccountAccessability"), Label).Text = Server.HtmlEncode(MyAccessLevel.Title)

                                Case "CREATEDON"
                                    CType(e.Item.FindControl("lblCreatedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "MODIFIEDON"
                                    CType(e.Item.FindControl("lblModifiedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LASTLOGINON"
                                    CType(e.Item.FindControl("lblLastLoginOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LASTLOGINVIAREMOTEIP"
                                    CType(e.Item.FindControl("lblLastLoginViaremoteIP"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                    CType(e.Item.FindControl("lblExtAccount"), Label).Text = Server.HtmlEncode(MyUserInfo.ExternalAccount)

                                Case "1STPREFERREDLANGUAGE"
                                    Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                    MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                    CType(e.Item.FindControl("lblFirstPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)

                                Case "2NDPREFERREDLANGUAGE"
                                    If Not IsDBNull(.Item(MyCount)) Then
                                        Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                        MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                        CType(e.Item.FindControl("lblSecondPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)
                                    End If

                                Case "3RDPREFERREDLANGUAGE"
                                    If Not IsDBNull(.Item(MyCount)) Then
                                        Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                        MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                        CType(e.Item.FindControl("lblThirdPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)
                                    End If
                            End Select
                        Next
                    End With
                End If
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     Clone user
    ''' </summary>
    Public Class UsersClone
        Inherits Page

#Region "Variable Declaration"
        Protected notCopiedData As DataTable
        Protected lblStatusMsg, lblErrMsg, lbl_ID, lbl_LoginName As Label
        Protected lbl_Company, lbl_Titel, lbl_Vorname, lbl_Nachname, lbl_Namenszusatz, lbl_e_mail As Label
        Protected lblAnrede As Label
        Protected WithEvents Button_Submit As Button
        Protected UserInfo As WMSystem.UserInformation
        Protected PnlAddFlags, PnlGroupsInformation, PnlAuth As Panel
        Protected New_Field_LoginName, New_Field_Password, New_Field_Company, New_Field_Titel, New_Field_Vorname, New_Field_Nachname, New_Field_Namenszusatz, New_Field_e_mail As TextBox
        Protected cmbAnrede As DropDownList
        Protected WithEvents ValidatorNewUserLoginName As CustomValidator

#End Region

#Region "Property Declaration"
        Protected ReadOnly Property UserID() As Long
            Get
                If Request.QueryString("ID") = "" Then
                    Return 0
                Else
                    Return CType(Request.QueryString("ID"), Long)
                End If
            End Get
        End Property
#End Region

#Region "Page Events"
        Private Sub UsersClone_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
            UserInfo = New WebManager.WMSystem.UserInformation(UserID, cammWebManager, False)
            initNotCopiedDataDatatable()
            AssignAdditionalFlagsToPnl()
            AssignAuthToPnl()
            AssignMembershipsToPnl()
            AssignUserInfoDataToForm()
        End Sub

        Private Sub UsersClone_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""
        End Sub


#End Region

#Region "User-Defined Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Checks whether username is already in use
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="args"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ValidatorNewUserLoginName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
            If args.Value.Length > 20 Then
                args.IsValid = False
                CType(source, CustomValidator).ErrorMessage = "Loginname is too long. Max. 20 characters."
            Else
                args.IsValid = True
            End If
            If CType(Me.cammWebManager.System_GetUserID(args.Value, True), Long) >= 0 Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End Sub

#Region " Collection of not copied data "
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialize Datatable. Contains data that was not copied e.g. protected flags
        ''' Does NOT contain values that are manually unchecked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub initNotCopiedDataDatatable()
            If Session("cwmCloneUserNotCopiedDataDt") Is Nothing Then
                notCopiedData = New DataTable("NotCopiedData")
                notCopiedData.Columns.Add("Key")
                notCopiedData.Columns.Add("Value")
                Session("cwmCloneUserNotCopiedDataDt") = notCopiedData
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Enumeration of data types which automatically cannot be copied
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Enum notCopiedDataEnum As Integer
            AdditionalFlag = 1
            Membership = 2
            Authorization = 3
        End Enum

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Add a new entry that automatically cannot be copied
        ''' </summary>
        ''' <param name="dataType">Type of data, e.g. AdditionalFlag</param>
        ''' <param name="value">The value, e.g. value of additional flag or name of membership</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AddNotCopiedData(ByVal dataType As notCopiedDataEnum, ByVal value As String)
            notCopiedData = CType(Session("cwmCloneUserNotCopiedDataDt"), DataTable)
            If notCopiedData.Select("Key = '" & dataType.ToString() & "' and Value = '" & value & "'").Length <= 0 Then
                Dim row As DataRow = notCopiedData.NewRow
                row("Key") = dataType.ToString
                row("Value") = value
                notCopiedData.Rows.Add(row)
            End If
            Session("cwmCloneUserNotCopiedDataDt") = notCopiedData
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Removes an entry, e.g. authorization has required flags, but its a protected flag
        ''' Handles check or uncheck the authorization-checkbox
        ''' </summary>
        ''' <param name="dataType"></param>
        ''' <param name="value"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub RemoveNotCopiedData(ByVal dataType As notCopiedDataEnum, ByVal value As String)
            notCopiedData = CType(Session("cwmCloneUserNotCopiedDataDt"), DataTable)
            If Not value = "" Then
                Dim rows() As DataRow = notCopiedData.Select("Key = '" & dataType.ToString & "' and Value = '" & value & "'")
                For Each row As DataRow In rows
                    notCopiedData.Rows.Remove(row)
                Next
            End If
            Session("cwmCloneUserNotCopiedDataDt") = notCopiedData
        End Sub

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     ID and (English) title of all available languages (markets) + the currently selected language (when it is different)
        ''' </summary>
        ''' <param name="alwaysIncludeThisLanguage">The currently selected language should always appear</param>
        ''' <returns></returns>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	06.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AvailableLanguages(ByVal alwaysIncludeThisLanguage As Integer) As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Description_English FROM System_Languages WHERE (IsActive = 1 AND NOT ID = 10000) OR ID = " & alwaysIncludeThisLanguage & " ORDER BY Description_English", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     ID and title of the available access levels
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	06.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AvailableAccessLevels() As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Title FROM System_AccessLevels ORDER BY Title", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

#Region " Assign data to form "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Assigns the source user information to the webform
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AssignUserInfoDataToForm()
            'Start with source user
            lbl_ID.Text = Server.HtmlEncode(UserInfo.IDLong.ToString)
            lbl_LoginName.Text = Server.HtmlEncode(Utils.Nz(UserInfo.LoginName, String.Empty))
            lbl_Company.Text = Server.HtmlEncode(Utils.Nz(UserInfo.Company.ToString, String.Empty))
            lbl_Titel.Text = Server.HtmlEncode(Utils.Nz(UserInfo.AcademicTitle.ToString, String.Empty))
            lbl_Vorname.Text = Server.HtmlEncode(Utils.Nz(UserInfo.FirstName.ToString, String.Empty))
            lbl_Nachname.Text = Server.HtmlEncode(Utils.Nz(UserInfo.LastName.ToString, String.Empty))
            lbl_Namenszusatz.Text = Server.HtmlEncode(Utils.Nz(UserInfo.NameAddition.ToString, String.Empty))
            lbl_e_mail.Text = Server.HtmlEncode(Utils.Nz(UserInfo.EMailAddress.ToString, String.Empty))

            If UserInfo.Gender = WMSystem.Sex.Masculine Then
                lblAnrede.Text = "Mr."
            ElseIf UserInfo.Gender = WMSystem.Sex.Feminine Then
                lblAnrede.Text = "Mrs."
            Else
                lblAnrede.Text = Nothing
            End If

            'Destination user
            New_Field_LoginName.Text = String.Empty
            New_Field_Company.Text = Utils.Nz(UserInfo.Company.ToString, String.Empty)
            New_Field_Titel.Text = Utils.Nz(UserInfo.AcademicTitle.ToString, String.Empty)
            New_Field_Vorname.Text = Utils.Nz(UserInfo.FirstName.ToString, String.Empty)
            New_Field_Nachname.Text = Utils.Nz(UserInfo.LastName.ToString, String.Empty)
            New_Field_Namenszusatz.Text = Utils.Nz(UserInfo.NameAddition.ToString, String.Empty)
            New_Field_e_mail.Text = Utils.Nz(UserInfo.EMailAddress.ToString, String.Empty)

            'bind Gender/Anrede dropdownlist
            cmbAnrede.Items.Clear()
            cmbAnrede.Items.Add(New ListItem(Nothing, Nothing))
            cmbAnrede.Items.Add(New ListItem("Mr.", CType(WMSystem.Sex.Masculine, String)))
            cmbAnrede.Items.Add(New ListItem("Ms.", CType(WMSystem.Sex.Feminine, String)))
            If UserInfo.Gender = WMSystem.Sex.Masculine Then
                cmbAnrede.SelectedIndex = 1
            ElseIf UserInfo.Gender = WMSystem.Sex.Feminine Then
                cmbAnrede.SelectedIndex = 2
            Else
                cmbAnrede.SelectedIndex = 0
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Assigns the additional flags information of the source user to the webform
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub AssignAdditionalFlagsToPnl()
            If UserInfo.AdditionalFlags.Count > 0 Then
                Dim lit As Literal
                Dim HtmlStr As System.Text.StringBuilder

                HtmlStr = New System.Text.StringBuilder
                HtmlStr.Append("<tr><td bgcolor=""#C1C1C1"" colspan=""2""><font face=""Arial"" size=""2""><b>Additional Flags:</b></font></td>" & vbNewLine)
                HtmlStr.Append("<td bgcolor=""#C1C1C1""><font face=""Arial"" size=""2"">Choose the user flags to copy</font></td></tr>" & vbNewLine)
                lit = New Literal
                lit.Text = HtmlStr.ToString
                PnlAddFlags.Controls.Add(lit)

                For MyCounter As Integer = 0 To UserInfo.AdditionalFlags.Count - 1
                    Dim MyKeyName As String = UserInfo.AdditionalFlags.Keys.Item(MyCounter)
                    Dim MyItemValue As String = UserInfo.AdditionalFlags.Item(MyCounter)

                    HtmlStr = New System.Text.StringBuilder
                    HtmlStr.Append("<tr><td valign=""Top"" width=""200""><font face=""Arial"" size=""2"">" & vbNewLine)
                    HtmlStr.Append(Server.HtmlEncode(MyKeyName))
                    HtmlStr.Append("</font></td><td valign=""Top"" width=""200""><font face=""Arial"" size=""2"">" & vbNewLine)
                    HtmlStr.Append(Server.HtmlEncode(MyItemValue))
                    HtmlStr.Append("</font></p></td><td valign=""top""><font face=""Arial"" size=""2"">" & vbNewLine)
                    lit = New Literal
                    lit.Text = HtmlStr.ToString
                    PnlAddFlags.Controls.Add(lit)

                    Dim Chk As New CheckBox
                    Chk.ID = "AddFlags_" & MyKeyName

                    'Check for protected additional flags
                    If AdditionalFlagAllowCopy(MyKeyName) = False Then
                        Chk.Text = Chk.Text & " (protected additional flag)"
                        Chk.Checked = False
                        Chk.Enabled = False
                        'AdditionalFlag automatically cannot be copied. So we add it to our list, so we can inform the user later in the status message
                        'A special case is when this protected flag is also a required flag of an authorization that should be copied
                        'In this special case we only have to list this flag if the belonging authorization is checked (--> copy)
                        'Pay attention to remove the flag from the list, if the user unchecks the belonging authorization 
                        AddNotCopiedData(notCopiedDataEnum.AdditionalFlag, MyKeyName)
                        PnlAddFlags.Controls.Add(Chk)
                    Else
                        Chk.Checked = True
                        Chk.Text = Server.HtmlEncode(MyKeyName)
                        'Edit value of flag
                        Dim txtBox As New TextBox
                        txtBox.ID = "EditFlags_" & MyKeyName
                        If Not IsPostBack Then
                            txtBox.Text = Server.HtmlEncode(MyItemValue).Replace("&#252;", "?").Replace("&#246;", "?").Replace("&#228;", "?").Replace("&#196;", "?").Replace("&#214;", "?").Replace("&#220;", "?")
                        End If
                        Dim tmpLbl As New Label
                        tmpLbl.Text = "<br />"
                        PnlAddFlags.Controls.Add(Chk)
                        PnlAddFlags.Controls.Add(tmpLbl)
                        PnlAddFlags.Controls.Add(txtBox)
                    End If

                    'Give ability to tell the user whether this flag is a required flag by a checked authorization
                    'This is controled by AssignAuthToPnl, because in this case the additional flags belong to the authorizations and we have to handle the checkbox postback event
                    Dim lblIsRequiredFlag As New Label
                    lblIsRequiredFlag.ID = "LabelFlag:" & MyKeyName
                    PnlAddFlags.Controls.Add(lblIsRequiredFlag)

                    HtmlStr = New System.Text.StringBuilder
                    HtmlStr.Append("</font></td></tr>" & vbNewLine)
                    lit = New Literal
                    lit.Text = HtmlStr.ToString
                    PnlAddFlags.Controls.Add(lit)
                Next
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Check whether it's allowed to copy the additional flag
        ''' Customizing in /sysdata/users_clone.aspx
        ''' </summary>
        ''' <param name="flagName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	08.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function AdditionalFlagAllowCopy(ByVal flagName As String) As Boolean
            Return True
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Assigns the membership information of the source user to the webform
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AssignMembershipsToPnl()
            Dim MyGroupInfosAllowRule As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = UserInfo.MembershipsByRule(False).AllowRule
            Dim MyGroupInfosDenyRule As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = UserInfo.MembershipsByRule(False).AllowRule

            If (Not MyGroupInfosAllowRule Is Nothing AndAlso MyGroupInfosAllowRule.Length > 0) OrElse (Not MyGroupInfosDenyRule Is Nothing AndAlso MyGroupInfosDenyRule.Length > 0) Then
                Dim lit As Literal
                Dim HtmlCode As System.Text.StringBuilder

                HtmlCode = New System.Text.StringBuilder
                HtmlCode.Append("<tr><td bgcolor=""#C1C1C1"" colspan=""2""><font face=""Arial"" size=""2""><b>Memberships:</b></font></td>" & vbNewLine)
                HtmlCode.Append("<td bgcolor=""#C1C1C1""><font face=""Arial"" size=""2"">Choose the memberships to copy</font></td></tr>" & vbNewLine)
                lit = New Literal
                lit.Text = HtmlCode.ToString
                PnlGroupsInformation.Controls.Add(lit)

                AssignMembershipsToPnl_RuleAdd(MyGroupInfosAllowRule, False)
                AssignMembershipsToPnl_RuleAdd(MyGroupInfosDenyRule, True)
            End If
        End Sub

        Private Sub AssignMembershipsToPnl_RuleAdd(myGroupInfos As CompuMaster.camm.WebManager.WMSystem.GroupInformation(), isDenyRule As Boolean)
            Dim lit As Literal
            Dim HtmlCode As System.Text.StringBuilder
            For Each MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation In myGroupInfos
                Dim DisplayName As String = Nothing
                Dim ID As Integer = Nothing
                Try
                    DisplayName = MyGroupInfo.Description
                    ID = MyGroupInfo.ID
                Catch
                    DisplayName = "<em>(error)</em>"
                    ID = Nothing
                End Try

                HtmlCode = New System.Text.StringBuilder
                HtmlCode.Append("<tr><td valign=""Top"" width=""200""><font face=""Arial"" size=""2"">")
                If isDenyRule Then
                    HtmlCode.Append("DENY: ")
                Else
                    HtmlCode.Append("GRANT: ")
                End If
                HtmlCode.Append("<a href=""groups_update.aspx?ID=" & ID & """>" & vbNewLine)
                HtmlCode.Append(Server.HtmlEncode(MyGroupInfo.Name))
                HtmlCode.Append("</a></font></td><td valign=""Top"" width=""200""><font face=""Arial"" size=""2"">" & vbNewLine)
                HtmlCode.Append(Server.HtmlEncode(DisplayName))
                HtmlCode.Append("</font></td><td><font face=""Arial"" size=""2"">" & vbNewLine)
                lit = New Literal
                lit.Text = HtmlCode.ToString
                PnlGroupsInformation.Controls.Add(lit)

                Dim Chk As New CheckBox
                If isDenyRule Then
                    Chk.ID = "ChkMembershipsDeny_" & MyGroupInfo.ID
                Else
                    Chk.ID = "ChkMemberships_" & MyGroupInfo.ID
                End If
                Chk.Text = Server.HtmlEncode(MyGroupInfo.Name)
                Chk.Checked = True
                PnlGroupsInformation.Controls.Add(Chk)

                HtmlCode = New System.Text.StringBuilder
                HtmlCode.Append("</font></td></tr>" & vbNewLine)
                lit = New Literal
                lit.Text = HtmlCode.ToString
                PnlGroupsInformation.Controls.Add(lit)
            Next
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Assigns the authorization information of the source user to the webform
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AssignAuthToPnl()
            Dim Auths As CompuMaster.camm.WebManager.WMSystem.Authorizations
            Auths = New CompuMaster.camm.WebManager.WMSystem.Authorizations(Nothing, cammWebManager, Nothing, Nothing, UserInfo.IDLong)
            Dim MyUserAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation()
            MyUserAuths = Auths.UserAuthorizationInformations(UserInfo.IDLong)

            If Not MyUserAuths Is Nothing AndAlso MyUserAuths.Length > 0 Then
                Dim lit As Literal
                Dim HtmlStr As System.Text.StringBuilder

                HtmlStr = New System.Text.StringBuilder
                HtmlStr.Append("<tr><td bgcolor=""#C1C1C1"" colspan=""2""><font face=""Arial"" size=""2""><b>Authorizations:</b></font></td>" & vbNewLine)
                HtmlStr.Append("<td bgcolor=""#C1C1C1""><font face=""Arial"" size=""2"">Choose the authorizations to copy</font></td></tr>" & vbNewLine)
                lit = New Literal
                lit.Text = HtmlStr.ToString
                PnlAuth.Controls.Add(lit)

                For Each MyUserAuthInfo As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation In MyUserAuths
                    Try
                        Dim SecObjID As Integer = MyUserAuthInfo.SecurityObjectInfo.ID
                        Dim SecObjName As String = MyUserAuthInfo.SecurityObjectInfo.DisplayName

                        HtmlStr = New System.Text.StringBuilder
                        HtmlStr.Append("<tr><td width=""160"" valign=""Top""><font face=""Arial"" size=""2"">ID" & vbNewLine)
                        HtmlStr.Append(SecObjID)
                        HtmlStr.Append("</font></td><td width=""240"" valign=""Top""><font face=""Arial"" size=""2"">" & vbNewLine)
                        If MyUserAuthInfo.IsDenyRule Then
                            HtmlStr.Append("DENY: ")
                        Else
                            HtmlStr.Append("GRANT: ")
                        End If
                        HtmlStr.Append(Server.HtmlEncode(SecObjName))
                        If MyUserAuthInfo.AlsoVisibleIfDisabled Then
                            HtmlStr.Append(" (Dev)")
                        End If
                        HtmlStr.Append("</font></td><td valign=""top""><font face=""Arial"" size=""2"">" & vbNewLine)

                        lit = New Literal
                        lit.Text = HtmlStr.ToString
                        PnlAuth.Controls.Add(lit)

                        Dim Chk As New CheckBox
                        If MyUserAuthInfo.IsDenyRule Then
                            Chk.ID = "ChkAuthDeny_" & SecObjID
                        Else
                            Chk.ID = "ChkAuth_" & SecObjID
                        End If
                        Chk.Checked = True

                        'Check whether authorization has required flags and then give an autopostback event to control required flags in AssignAdditionalFlagsToPnl
                        If Not getRequiredFlags(SecObjID) Is Nothing Then
                            Chk.AutoPostBack = True
                            AddHandler Chk.CheckedChanged, AddressOf OnCheckboxCheckChanged
                        End If

                        PnlAuth.Controls.Add(Chk)

                        'Inform user about required flags at the first time
                        If Not Page.IsPostBack Then
                            For Each control As UI.Control In PnlAddFlags.Controls
                                If Not control Is Nothing AndAlso Not control.ID Is Nothing Then
                                    If control.ID.StartsWith("LabelFlag:") Then 'See more information about the label in AssignAdditionalFlagsToPnl
                                        For Each flagName As String In getRequiredFlags(SecObjID)
                                            flagName = Trim(flagName)
                                            If control.ID.IndexOf(flagName) > 0 Then
                                                'Now we have the right literal to tell the user that the additional flag is also a required flag by this authorization
                                                CType(control, Label).Visible = True
                                                CType(control, Label).ForeColor = Drawing.Color.Red
                                                Dim SI As New camm.WebManager.WMSystem.SecurityObjectInformation(CType(Chk.ID.Substring(Chk.ID.IndexOf("_") + 1), Integer), cammWebManager)
                                                CType(control, Label).Text = "<br />* required flag by " & SI.DisplayName()
                                                'check whether the required flag is a protected flag
                                                If Not AdditionalFlagAllowCopy(flagName) Then
                                                    'Add to list to inform the user that this required flag cannot be copied
                                                    AddNotCopiedData(notCopiedDataEnum.AdditionalFlag, flagName)
                                                End If
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If

                        HtmlStr = New System.Text.StringBuilder
                        HtmlStr.Append("</font></td></tr>" & vbNewLine)
                        lit = New Literal
                        lit.Text = HtmlStr.ToString
                        PnlAuth.Controls.Add(lit)

                    Catch
                        cammWebManager.Log.Warn("Missing security object with ID " & MyUserAuthInfo.SecurityObjectID & " in authorizations for group ID " & UserInfo.ID)
                        HtmlStr = New System.Text.StringBuilder
                        HtmlStr.Append("<tr>" & vbNewLine)
                        HtmlStr.Append("    <td colspan=""2"">" & vbNewLine)
                        HtmlStr.Append("        <table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">" & vbNewLine)
                        HtmlStr.Append("            <tr>" & vbNewLine)
                        HtmlStr.Append("                <td width=""160"" valign=""Top"">" & vbNewLine)
                        HtmlStr.Append("                    <p>" & vbNewLine)
                        HtmlStr.Append("                        <font face=""Arial"" size=""2""><em>ID" & vbNewLine)
                        HtmlStr.Append(MyUserAuthInfo.SecurityObjectID & "</em></font></p>" & vbNewLine)
                        HtmlStr.Append("                </td>" & vbNewLine)
                        HtmlStr.Append("                <td width=""240"" valign=""Top"">" & vbNewLine)
                        HtmlStr.Append("                    <p>" & vbNewLine)
                        HtmlStr.Append("                        <font face=""Arial"" size=""2""><em>Missing security object</em></font></p>" & vbNewLine)
                        HtmlStr.Append("                </td>" & vbNewLine)
                        HtmlStr.Append("            </tr>" & vbNewLine)
                        HtmlStr.Append("        </table>" & vbNewLine)
                        HtmlStr.Append("    </td>" & vbNewLine)
                        HtmlStr.Append("</tr>" & vbNewLine)

                        lit = New Literal
                        lit.Text = HtmlStr.ToString
                        PnlAuth.Controls.Add(lit)

                    End Try
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Handles the checkbox check changed event to control required flags
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	09.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub OnCheckboxCheckChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim chk As CheckBox = CType(sender, CheckBox)
            'Get the required flags for the current authorization
            Dim requiredFlags() As String = getRequiredFlags(CType(chk.ID.Substring(chk.ID.IndexOf("_") + 1), Integer))
            For Each control As UI.Control In PnlAddFlags.Controls
                If Not control Is Nothing AndAlso Not control.ID Is Nothing Then
                    If control.ID.StartsWith("LabelFlag:") Then 'See more information about the label in AssignAdditionalFlagsToPnl
                        For Each flagName As String In requiredFlags
                            flagName = Trim(flagName)
                            If control.ID.IndexOf(flagName) > 0 Then
                                If chk.Checked Then
                                    'Now we have the right literal to tell the user that the additional flag is also a required flag by this authorization
                                    CType(control, Label).Visible = True
                                    CType(control, Label).ForeColor = Drawing.Color.Red
                                    Dim SI As New camm.WebManager.WMSystem.SecurityObjectInformation(CType(chk.ID.Substring(chk.ID.IndexOf("_") + 1), Integer), cammWebManager)
                                    CType(control, Label).Text = "<br />* required flag by " & SI.DisplayName()

                                    'check whether the required flag is a protected flag
                                    If Not AdditionalFlagAllowCopy(flagName) Then
                                        'Add to list to inform the user that this required flag cannot be copied
                                        AddNotCopiedData(notCopiedDataEnum.AdditionalFlag, flagName)
                                    End If

                                Else
                                    'Undo
                                    CType(control, Label).Visible = False
                                    'remove from list
                                    RemoveNotCopiedData(notCopiedDataEnum.AdditionalFlag, flagName)
                                End If
                            End If
                        Next
                    End If
                End If
            Next
        End Sub


#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get required flags for given securityobject
        ''' </summary>
        ''' <param name="SecurityObjectID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	08.02.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function getRequiredFlags(ByVal SecurityObjectID As Integer) As String()
            'Get required flags
            Dim cmd As New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select RequiredUserProfileFlags From Applications_CurrentAndInactiveOnes Where ID = @ID", New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = SecurityObjectID
            Dim al As ArrayList = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Automations.AutoOpenAndCloseAndDisposeConnection)

            If Not al.Item(0) Is DBNull.Value OrElse Not al.Item(0) Is Nothing Then
                Return CStr(al.Item(0)).Split(","c)
            End If
            Return Nothing
        End Function

#Region " Clone "

        Private Sub CloneMemberships(ByVal TemplateUser As CompuMaster.camm.WebManager.WMSystem.UserInformation, ByVal NewUser As CompuMaster.camm.WebManager.WMSystem.UserInformation)
            For Each control As Web.UI.Control In Page.FindControl("PnlGroupsInformation").Controls
                If control.GetType.ToString = "System.Web.UI.WebControls.CheckBox" AndAlso control.ID.IndexOf("ChkMemberships_") >= 0 Then
                    If CType(control, CheckBox).Checked Then
                        NewUser.AddMembership(CType(control.ID.Substring(control.ID.IndexOf("_") + 1), Integer), False)
                    End If
                End If
                If control.GetType.ToString = "System.Web.UI.WebControls.CheckBox" AndAlso control.ID.IndexOf("ChkMembershipsDeny_") >= 0 Then
                    If CType(control, CheckBox).Checked Then
                        NewUser.AddMembership(CType(control.ID.Substring(control.ID.IndexOf("_") + 1), Integer), True)
                    End If
                End If
            Next
        End Sub

        Private Sub CloneAuthorizations(ByVal TemplateUser As CompuMaster.camm.WebManager.WMSystem.UserInformation, ByVal NewUser As CompuMaster.camm.WebManager.WMSystem.UserInformation)
            For Each control As Web.UI.Control In Page.FindControl("PnlAuth").Controls
                If control.GetType.ToString = "System.Web.UI.WebControls.CheckBox" AndAlso control.ID.IndexOf("ChkAuth_") >= 0 Then
                    If CType(control, CheckBox).Checked Then
                        Dim ServerGroupID As Integer
                        Dim IsDev As Boolean
                        'CWM throws exception now, if ServerGroupID is specified - reactivate codeline below if CWM supports the use of ServerGroupID in a future version
                        'NewUser.AddAuthorization(CType(control.ID.Substring(control.ID.IndexOf("_") + 1), Integer), cammWebManager.CurrentServerInfo.ParentServerGroupID)
                        NewUser.AddAuthorization(CType(control.ID.Substring(control.ID.IndexOf("_") + 1), Integer), ServerGroupID, IsDev, False)
                    End If
                End If
                If control.GetType.ToString = "System.Web.UI.WebControls.CheckBox" AndAlso control.ID.IndexOf("ChkAuthDeny_") >= 0 Then
                    If CType(control, CheckBox).Checked Then
                        Dim ServerGroupID As Integer
                        Dim IsDev As Boolean
                        'CWM throws exception now, if ServerGroupID is specified - reactivate codeline below if CWM supports the use of ServerGroupID in a future version
                        'NewUser.AddAuthorization(CType(control.ID.Substring(control.ID.IndexOf("_") + 1), Integer), cammWebManager.CurrentServerInfo.ParentServerGroupID)
                        NewUser.AddAuthorization(CType(control.ID.Substring(control.ID.IndexOf("_") + 1), Integer), ServerGroupID, IsDev, True)
                    End If
                End If
            Next
        End Sub

        Private Sub CloneAdditionalFlags(ByVal TemplateAdditionalFlags As System.Collections.Specialized.NameValueCollection, ByVal newUser As CompuMaster.camm.WebManager.WMSystem.UserInformation)
            For Each control As Web.UI.Control In Page.FindControl("PnlAddFlags").Controls
                If control.GetType.ToString = "System.Web.UI.WebControls.CheckBox" Then
                    If CType(control, CheckBox).Checked Then
                        Dim MyKeyName As String = control.ID.Substring(control.ID.IndexOf("_") + 1)
                        If Not IsFlagExcludedFromCloning(MyKeyName) Then
                            Dim MyItemValue As String = TemplateAdditionalFlags.Item(control.ID.Substring(control.ID.IndexOf("_") + 1))
                            'Get value of textbox
                            If Not PnlAddFlags.FindControl("EditFlags_" & control.ID.Substring(control.ID.IndexOf("_") + 1)) Is Nothing Then
                                Dim editFlag As TextBox = CType(PnlAddFlags.FindControl("EditFlags_" & control.ID.Substring(control.ID.IndexOf("_") + 1)), TextBox)
                                If Not editFlag.Text = "" Then
                                    If newUser.AdditionalFlags(MyKeyName) = "" Then
                                        newUser.AdditionalFlags(MyKeyName) = editFlag.Text
                                    End If
                                End If
                            Else
                                If newUser.AdditionalFlags(MyKeyName) = "" Then
                                    newUser.AdditionalFlags(MyKeyName) = MyItemValue
                                End If
                            End If
                        End If
                    End If
                End If
            Next
        End Sub

        Private Function IsFlagExcludedFromCloning(flagName As String) As Boolean
            For Each flag As String In cammWebManager.UserCloneExludedAdditionalFlags
                If String.Compare(flag, flagName, True) = 0 Then Return True
            Next
            Return False
        End Function

        Private Sub SetStandardFlagValues(ByVal userInfo As WMSystem.UserInformation)
            userInfo.AdditionalFlags.Set("ComesFrom", "Account cloned by Admin """ & cammWebManager.CurrentUserLoginName & """ (" & cammWebManager.CurrentUserInfo.FirstName & " " & cammWebManager.CurrentUserInfo.LastName & ")")
            userInfo.AdditionalFlags.Set("Motivation", "Account cloned by Admin")
        End Sub

        Private Function Clone(ByVal newLoginName As String, ByVal genderID As Short, ByVal newAcademicTitle As String, ByVal newFirstName As String, ByVal newNameAddition As String, ByVal newLastName As String, ByVal newEmailAddress As String, ByVal newPassword As String) As CompuMaster.camm.WebManager.WMSystem.UserInformation
            Me.Page.Validate()
            If Page.IsValid Then
                If newLoginName.Length > 20 Then
                    lblErrMsg.Text = "Loginname exeeded the length of max. 20 characters."
                    Return Nothing
                End If
                Dim TemplateUser As New WebManager.WMSystem.UserInformation(UserID, cammWebManager, False)
                Dim NewUser As WebManager.WMSystem.UserInformation = Nothing
                Try
                    NewUser = New WebManager.WMSystem.UserInformation(Nothing, newLoginName, newEmailAddress, False, New_Field_Company.Text, CType(genderID, WMSystem.Sex), newNameAddition, newFirstName, newLastName, newAcademicTitle, TemplateUser.Street, TemplateUser.ZipCode, TemplateUser.Location, TemplateUser.State, TemplateUser.Country, TemplateUser.PreferredLanguage1.ID, TemplateUser.PreferredLanguage2.ID, TemplateUser.PreferredLanguage3.ID, TemplateUser.LoginDisabled, False, False, TemplateUser.AccessLevel.ID, cammWebManager, CType(Nothing, String))
                Catch ex As System.NotSupportedException
                    lblErrMsg.Text = ex.Message
                Catch ex2 As CompuMaster.camm.WebManager.PasswordTooWeakException
                    lblErrMsg.Text = ex2.Message
                End Try

                If Not NewUser Is Nothing Then
                    If newPassword <> Nothing Then
                        Select Case cammWebManager.PasswordSecurity.InspectionSeverities(NewUser.AccessLevel.ID).ValidatePasswordComplexity(newPassword, UserID)
                            Case PasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                                lblErrMsg.Text = "Password does not match the required complexity."
                            Case PasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                                lblErrMsg.Text = "Password is too long."
                            Case PasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                                lblErrMsg.Text = "Password is too short."
                            Case PasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                                lblErrMsg.Text = "Password must not contain parts of the username."
                            Case PasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_Unspecified
                                lblErrMsg.Text = "There is an unknown problem with the given password."
                            Case Else
                                lblErrMsg.Text = Nothing
                        End Select
                        If lblErrMsg.Text <> Nothing Then
                            Return Nothing
                        End If
                    End If

                    NewUser.AccountAuthorizationsAlreadySet = False
                    NewUser.AccountProfileValidatedByEMailTest = False
                    NewUser.AutomaticLogonAllowedByMachineToMachineCommunication = TemplateUser.AutomaticLogonAllowedByMachineToMachineCommunication
                    NewUser.FaxNumber = TemplateUser.FaxNumber
                    NewUser.MobileNumber = TemplateUser.MobileNumber
                    NewUser.PhoneNumber = TemplateUser.PhoneNumber
                    NewUser.Position = TemplateUser.Position

                    CloneAdditionalFlags(TemplateUser.AdditionalFlags, NewUser)
                    SetStandardFlagValues(NewUser)

                    If Trim(newPassword) = Nothing Then
                        NewUser.Save()
                    Else
                        Try
                            NewUser.Save(newPassword)
                        Catch ex As CompuMaster.camm.WebManager.PasswordTooWeakException
                            lblErrMsg.Text = ex.Message
                        End Try
                    End If

                    If lblErrMsg.Text <> Nothing Then
                        Return Nothing
                    End If

                    CloneMemberships(TemplateUser, NewUser)
                    CloneAuthorizations(TemplateUser, NewUser)
                End If

                Return NewUser
            Else
                Return Nothing
            End If


        End Function

#End Region

#End Region

#Region "Control Events"
        Sub BtnSubmitClick(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Submit.Click
            notCopiedData = CType(Session("cwmCloneUserNotCopiedDataDt"), DataTable)
            Me.Page.Validate()
            If Page.IsValid Then
                Dim ClonedUser As CompuMaster.camm.WebManager.WMSystem.UserInformation = Clone(New_Field_LoginName.Text, CShort(IIf(cmbAnrede.SelectedValue = Nothing, 0, cmbAnrede.SelectedValue)), New_Field_Titel.Text, New_Field_Vorname.Text, New_Field_Namenszusatz.Text, New_Field_Nachname.Text, New_Field_e_mail.Text, New_Field_Password.Text)
                If Not ClonedUser Is Nothing Then
                    lblStatusMsg.ForeColor = Drawing.Color.Green
                    lblStatusMsg.Text = "Cloning was successful! New userID: " & ClonedUser.IDLong & ". <a href=""users_update.aspx?ID=" & ClonedUser.ID & """>>>Update UserProfile</a><br />"
                    'Add list of user-details that could not be copied to status message
                    Dim sb As New Text.StringBuilder
                    If notCopiedData.Rows.Count > 0 Then
                        sb.Append("The following user details couldn't be copied:<br/>")
                        sb.Append("<lu>")
                        For Each row As DataRow In notCopiedData.Rows
                            sb.Append("<li>")
                            sb.Append(row("Key"))
                            sb.Append(": ")
                            sb.Append(row("Value"))
                            sb.Append("</li>")
                            sb.Append("<br/>")
                        Next
                        sb.Append("</lu>")
                    End If
                    lblStatusMsg.Text &= sb.ToString
                    lblStatusMsg.Text = "<hr />" & lblStatusMsg.Text & "<hr />"
                Else
                    'only throw exception if no error message is available
                    If lblErrMsg.Text Is Nothing OrElse lblErrMsg.Text = "" Then
                        Throw New ArgumentNullException("ClonedUser", "Wasn't able to clone user")
                    End If
                End If
            End If
            'Cleanup
            Session("cwmCloneUserNotCopiedDataDt") = Nothing

        End Sub
#End Region

    End Class

    ''' <summary>
    '''     Updates user details
    ''' </summary>
    Public Class UsersUpdate
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, Field_ID, Field_LoginName, Field_LoginCount, Field_LoginFailures As Label
        Protected Field_CreatedOn, Field_ModifiedOn, Field_LastLoginOn As Label
        Protected Field_Company, Field_Titel, Field_Vorname, Field_Nachname, Field_Namenszusatz, Field_e_mail As TextBox
        Protected Field_Strasse, Field_PLZ, Field_Ort, Field_State, Field_Land, Field_LoginLockedTill As TextBox
        Protected Field_ExternalAccount, Field_Phone, Field_Fax, Field_Mobile, Field_Position As TextBox
        Protected cmbAnrede, cmb1stPreferredLanguage, cmb2ndPreferredLanguage, cmb3rdPreferredLanguage, cmbLoginDisabled, cmbAccountAccessable As DropDownList
        Protected WithEvents Button_Submit As Button
        Protected UserInfo As WMSystem.UserInformation
        Protected pnlSpecialUsers As Panel
#End Region

#Region "Property Declaration"
        Protected ReadOnly Property UserID() As Long
            Get
                If Request.QueryString("ID") = "" Then
                    Return 0
                Else
                    Return Utils.Nz(Request.QueryString("ID"), 0L)
                End If
            End Get
        End Property
#End Region

#Region "Page Events"
        Private Sub UsersUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            UserInfo = New WebManager.WMSystem.UserInformation(UserID, cammWebManager, False)
            lblErrMsg.Text = ""

            If Not Page.IsPostBack Then
                If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CInt(Request.QueryString("ID")) Then
                    AssignUserInfoDataToForm()
                    pnlSpecialUsers.Visible = False
                Else
                    pnlSpecialUsers.Visible = True
                    AssignUserInfoDataToForm()
                    FillDropDownLists()
                End If
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     ID and (English) title of all available languages (markets) + the currently selected language (when it is different)
        ''' </summary>
        ''' <param name="alwaysIncludeThisLanguage">The currently selected language should always appear</param>
        ''' <returns></returns>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	06.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AvailableLanguages(ByVal alwaysIncludeThisLanguage As Integer) As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Description_English FROM System_Languages WHERE (IsActive = 1 AND NOT ID = 10000) OR ID = " & alwaysIncludeThisLanguage & " ORDER BY Description_English", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     ID and title of the available access levels
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	06.10.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AvailableAccessLevels() As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Title FROM System_AccessLevels ORDER BY Title", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        Protected Sub AssignUserInfoDataToForm()
            Field_ID.Text = Server.HtmlEncode(UserInfo.IDLong.ToString)
            Field_LoginName.Text = Server.HtmlEncode(Utils.Nz(UserInfo.LoginName, String.Empty))
            Field_Company.Text = Utils.Nz(UserInfo.Company.ToString, String.Empty)
            Field_Titel.Text = Utils.Nz(UserInfo.AcademicTitle.ToString, String.Empty)
            Field_Vorname.Text = Utils.Nz(UserInfo.FirstName.ToString, String.Empty)
            Field_Nachname.Text = Utils.Nz(UserInfo.LastName.ToString, String.Empty)
            Field_Namenszusatz.Text = Utils.Nz(UserInfo.NameAddition.ToString, String.Empty)
            Field_e_mail.Text = Utils.Nz(UserInfo.EMailAddress.ToString, String.Empty)
            Field_Strasse.Text = Utils.Nz(UserInfo.Street.ToString, String.Empty)
            Field_PLZ.Text = Utils.Nz(UserInfo.ZipCode.ToString, String.Empty)
            Field_Ort.Text = Utils.Nz(UserInfo.Location.ToString, String.Empty)
            Field_State.Text = Utils.Nz(UserInfo.State.ToString, String.Empty)
            Field_Land.Text = Utils.Nz(UserInfo.Country.ToString, String.Empty)

            If UserInfo.PhoneNumber = Nothing Then Field_Phone.Text = "" Else Field_Phone.Text = Utils.Nz(UserInfo.PhoneNumber, String.Empty)
            If UserInfo.FaxNumber = Nothing Then Field_Fax.Text = "" Else Field_Fax.Text = Utils.Nz(UserInfo.FaxNumber.ToString, String.Empty)
            If UserInfo.MobileNumber = Nothing Then Field_Mobile.Text = "" Else Field_Mobile.Text = Utils.Nz(UserInfo.MobileNumber.ToString, String.Empty)
            If UserInfo.Position = Nothing Then Field_Position.Text = "" Else Field_Position.Text = Utils.Nz(UserInfo.Position, String.Empty)

            Field_LoginCount.Text = Server.HtmlEncode(Utils.Nz(UserInfo.AccountSuccessfullLogins, 0).ToString)
            Field_LoginFailures.Text = Server.HtmlEncode(Utils.Nz(UserInfo.AccountLoginFailures, 0).ToString)
            If UserInfo.LoginLockedTemporaryTill = Nothing Then
                Field_LoginLockedTill.Text = String.Empty
            Else
                Field_LoginLockedTill.Text = UserInfo.LoginLockedTemporaryTill.ToString
            End If
            Field_CreatedOn.Text = Server.HtmlEncode(UserInfo.AccountCreatedOn.ToString)
            Field_ModifiedOn.Text = Server.HtmlEncode(UserInfo.AccountModifiedOn.ToString)
            If UserInfo.AccountLastLoginOn = Nothing Then
                Field_LastLoginOn.Text = Nothing
            Else
                Field_LastLoginOn.Text = Server.HtmlEncode(UserInfo.AccountLastLoginOn.ToString)
            End If
            'Field_LastLoginViaRemoteIP.Text = Server.HtmlEncode(UserInfo.AccountLastLoginFromAddress)
            If UserInfo.ExternalAccount = Nothing Then Field_ExternalAccount.Text = "" Else Field_ExternalAccount.Text = UserInfo.ExternalAccount.ToString
        End Sub

        Protected Sub AssignFormDataToUserInfo()
            UserInfo.Company = Trim(Mid(Me.Field_Company.Text, 1, 50))
            If cmbAnrede.SelectedValue <> Nothing Then
                If Integer.Parse(cmbAnrede.SelectedValue) = CType(IUserInformation.GenderType.Feminine, Integer) Then
                    UserInfo.Gender = CType(IUserInformation.GenderType.Feminine, WMSystem.Sex)
                ElseIf Integer.Parse(cmbAnrede.SelectedValue) = CType(IUserInformation.GenderType.Masculine, Integer) Then
                    UserInfo.Gender = CType(IUserInformation.GenderType.Masculine, WMSystem.Sex)
                End If
            Else
                UserInfo.Gender = WMSystem.Sex.Undefined
            End If
            UserInfo.AcademicTitle = Trim(Mid(Me.Field_Titel.Text, 1, 20))
            UserInfo.FirstName = Trim(Mid(Me.Field_Vorname.Text, 1, 30))
            UserInfo.LastName = Trim(Mid(Me.Field_Nachname.Text, 1, 30))
            UserInfo.NameAddition = Trim(Mid(Me.Field_Namenszusatz.Text, 1, 20))
            UserInfo.EMailAddress = Trim(Mid(Me.Field_e_mail.Text, 1, 50))
            UserInfo.Street = Trim(Mid(Me.Field_Strasse.Text, 1, 30))
            UserInfo.ZipCode = Trim(Mid(Me.Field_PLZ.Text, 1, 10))
            UserInfo.Location = Trim(Mid(Me.Field_Ort.Text, 1, 50))
            UserInfo.State = Trim(Mid(Me.Field_State.Text, 1, 30))
            UserInfo.Country = Trim(Mid(Me.Field_Land.Text, 1, 30))

            UserInfo.PhoneNumber = Trim(Mid(Me.Field_Phone.Text, 1, 30))
            UserInfo.FaxNumber = Trim(Mid(Me.Field_Fax.Text, 1, 50))
            UserInfo.MobileNumber = Trim(Mid(Me.Field_Mobile.Text, 1, 30))
            UserInfo.Position = Trim(Mid(Me.Field_Position.Text, 1, 30))

            UserInfo.PreferredLanguage1 = New WMSystem.LanguageInformation(CInt(cmb1stPreferredLanguage.SelectedValue), cammWebManager)
            If Trim(cmb2ndPreferredLanguage.SelectedValue) = "" Then
                UserInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(0, cammWebManager)
            Else
                UserInfo.PreferredLanguage2 = New WMSystem.LanguageInformation(CInt(cmb2ndPreferredLanguage.SelectedValue), cammWebManager)
            End If
            If Trim(cmb3rdPreferredLanguage.SelectedValue) = "" Then
                UserInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(0, cammWebManager)
            Else
                UserInfo.PreferredLanguage3 = New WMSystem.LanguageInformation(CInt(cmb3rdPreferredLanguage.SelectedValue), cammWebManager)
            End If
            UserInfo.AccessLevel = New WMSystem.AccessLevelInformation(Utils.Nz(cmbAccountAccessable.SelectedValue, 0), cammWebManager)
            UserInfo.LoginDisabled = CBool(CInt(cmbLoginDisabled.SelectedValue))
            If Trim(Me.Field_LoginLockedTill.Text) = "" Then
                UserInfo.LoginLockedTemporary = False
            Else
                UserInfo.LoginLockedTemporaryTill = DateTime.Parse(Trim(Me.Field_LoginLockedTill.Text))
            End If

            UserInfo.ExternalAccount = Trim(Field_ExternalAccount.Text)
        End Sub

        Protected Overridable Function UpdateAccount() As Boolean
            If UserInfo.IDLong = Nothing Then
                Throw New Exception("Invalid user, can't update")
            ElseIf UserInfo.LoginName = Nothing Then
                Throw New Exception("Missing login name")
            ElseIf UserInfo.EMailAddress = Nothing Then
                Throw New Exception("Missing e-mail address")
            ElseIf UserInfo.IDLong <> Nothing And UserInfo.LoginName <> String.Empty Then
                Try
                    UserInfo.Save()
                Catch ex As Exception
                    'unhandled exception.
                    If ex.Message.ToString.ToUpper = "UNIQUE KEY ERROR" Then
                        lblErrMsg.Text = "External Account already exists."
                    Else
                        Throw
                    End If
                    Return False
                End Try
            Else
                Throw New Exception("Please specify a user id an login name.!")
            End If

            Return True
        End Function

        Private Sub FillDropDownLists()
            'bind Gender/Anrede dropdownlist
            cmbAnrede.Items.Clear()
            cmbAnrede.Items.Add(New ListItem(Nothing, Nothing))
            cmbAnrede.Items.Add(New ListItem("Mr.", CType(WMSystem.Sex.Masculine, String)))
            cmbAnrede.Items.Add(New ListItem("Ms.", CType(WMSystem.Sex.Feminine, String)))
            If UserInfo.Gender = WMSystem.Sex.Masculine Then
                cmbAnrede.SelectedIndex = 1
            ElseIf UserInfo.Gender = WMSystem.Sex.Feminine Then
                cmbAnrede.SelectedIndex = 2
            Else
                cmbAnrede.SelectedIndex = 0
            End If
            'bind LoginDisabled dropdownlist
            cmbLoginDisabled.Items.Clear()
            cmbLoginDisabled.Items.Add(New ListItem("Yes", "1"))
            cmbLoginDisabled.Items.Add(New ListItem("No", "0"))
            If UserInfo.LoginDisabled = True Then cmbLoginDisabled.SelectedValue = "1" Else cmbLoginDisabled.SelectedValue = "0"
            'bind AvailableAccessLevels dropdownlist
            cmbAccountAccessable.Items.Clear()
            Dim AccessLevels As DictionaryEntry() = AvailableAccessLevels()
            For MyCounter As Integer = 0 To AccessLevels.Length - 1
                cmbAccountAccessable.Items.Add(New ListItem(Server.HtmlEncode(Utils.Nz(AccessLevels(MyCounter).Value, String.Empty)), Utils.Nz(AccessLevels(MyCounter).Key, String.Empty)))
                If UserInfo.AccessLevel.ID = Utils.Nz(AccessLevels(MyCounter).Key, 0) Then cmbAccountAccessable.SelectedIndex = MyCounter
            Next
            'bind 1stPreferredLanguage dropdownlist
            cmb1stPreferredLanguage.Items.Clear()
            Dim AvailableLanguageInfos As DictionaryEntry()
            AvailableLanguageInfos = AvailableLanguages(UserInfo.PreferredLanguage1.ID)
            For MyCounter As Integer = 0 To AvailableLanguageInfos.Length - 1
                cmb1stPreferredLanguage.Items.Add(New ListItem(Utils.Nz(AvailableLanguageInfos(MyCounter).Value, String.Empty), Utils.Nz(AvailableLanguageInfos(MyCounter).Key, String.Empty)))
                If UserInfo.PreferredLanguage1.ID = Utils.Nz(AvailableLanguageInfos(MyCounter).Key, 0) Then cmb1stPreferredLanguage.SelectedIndex = MyCounter
            Next
            'bind 2ndPreferredLanguage dropdownlist
            cmb2ndPreferredLanguage.Items.Clear()
            cmb2ndPreferredLanguage.Items.Add(New ListItem("", ""))
            If UserInfo.PreferredLanguage2 Is Nothing Then
                AvailableLanguageInfos = AvailableLanguages(Nothing)
            Else
                AvailableLanguageInfos = AvailableLanguages(UserInfo.PreferredLanguage2.ID)
            End If
            For MyCounter As Integer = 0 To AvailableLanguageInfos.Length - 1
                cmb2ndPreferredLanguage.Items.Add(New ListItem(Utils.Nz(AvailableLanguageInfos(MyCounter).Value, String.Empty), Utils.Nz(AvailableLanguageInfos(MyCounter).Key, String.Empty)))
                If Not UserInfo.PreferredLanguage2 Is Nothing AndAlso UserInfo.PreferredLanguage2.ID = Utils.Nz(AvailableLanguageInfos(MyCounter).Key, 0) Then cmb2ndPreferredLanguage.SelectedIndex = MyCounter + 1
            Next
            'bind 3rdPreferredLanguage dropdownlist
            cmb3rdPreferredLanguage.Items.Clear()
            cmb3rdPreferredLanguage.Items.Add(New ListItem("", ""))
            If UserInfo.PreferredLanguage3 Is Nothing Then
                AvailableLanguageInfos = AvailableLanguages(Nothing)
            Else
                AvailableLanguageInfos = AvailableLanguages(UserInfo.PreferredLanguage3.ID)
            End If
            For MyCounter As Integer = 0 To AvailableLanguageInfos.Length - 1
                cmb3rdPreferredLanguage.Items.Add(New ListItem(Utils.Nz(AvailableLanguageInfos(MyCounter).Value, String.Empty), Utils.Nz(AvailableLanguageInfos(MyCounter).Key, String.Empty)))
                If Not UserInfo.PreferredLanguage3 Is Nothing AndAlso UserInfo.PreferredLanguage3.ID = Utils.Nz(AvailableLanguageInfos(MyCounter).Key, 0) Then cmb3rdPreferredLanguage.SelectedIndex = MyCounter + 1
            Next
        End Sub
#End Region

#Region "Control Events"
        Sub SaveChanges(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Submit.Click
            Try
                AssignFormDataToUserInfo()
                If UpdateAccount() = True Then
                    If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CInt(Request.QueryString("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CInt(Request.QueryString("ID")) Then
                        AssignUserInfoDataToForm()
                    End If

                    lblErrMsg.Style.Add("color", "#009900")
                    lblErrMsg.Text = "The record has been updated successfully!"
                End If
            Catch ex As Exception
                Throw New Exception("Cannot save changes to the user profile.", ex)
            End Try
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     A page to update user flags
    ''' </summary>
    Public Class User_Update_Flag
        Inherits Page

#Region "Variable Declaration"
        Dim dt As New DataTable
        Dim Fieldtype, FieldValue, Field_ID, ErrMsg As String
        Protected lblMsg, lblFlagUser As Label
        Protected txtType, txtValue As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub ButtonSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If Me.IsPostBack Then
                SaveChanges()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This method saves the changes
        ''' </summary>
        ''' <remarks>This save method is called by the form's submit button</remarks>
        ''' <history>
        ''' 	[wezel]	29.01.2010	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub SaveChanges()
            Dim type As String = txtType.Text.Trim()

            If Request.QueryString("ID") <> "" And type <> "" Then
                'Update record
                Dim value As String = txtValue.Text.Trim()

                Dim validator As New CompuMaster.camm.WebManager.FlagValidation(type)
                If validator.IsCorrectValueForType(value) Then
                    Dim userinfo As New camm.WebManager.WMSystem.UserInformation(CLng(Request.QueryString("ID")), cammWebManager)
                    userinfo.AdditionalFlags.Set(type, value)
                    userinfo.Save()
                    lblMsg.Text = "Flag saved successfully"
                Else
                    lblMsg.Text = "Incorrect value for this type. Flag wasn't saved."
                End If
            End If
        End Sub

        Private Sub User_Update_Flag_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            FillFields()
        End Sub

        Protected Overridable Sub FillFields()
            Dim userinfo As New camm.WebManager.WMSystem.UserInformation(CLng(Request.QueryString("ID")), cammWebManager) 'Reload user info object to present the latest data
            lblFlagUser.Text = Server.HtmlEncode("Flag of user " & userinfo.FullName & "  (" & userinfo.LoginName & ") ")
            If Not Me.IsPostBack Then
                txtType.Text = Request.QueryString("Type")
                txtValue.Text = userinfo.AdditionalFlags(txtType.Text)
            End If
        End Sub

#End Region

    End Class

    ''' <summary>
    '''     A page to reset user password
    ''' </summary>
    Public Class User_Resetpw
        Inherits Page

#Region "Variable Declaration"
        Dim displayloginname, ErrMsg As String
        Protected lblMsg As Label
        Protected txtLoginName, txtNewPassword As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Event"
        Private Sub User_Resetpw_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            displayloginname = Request.Form("txtLoginName")
            txtLoginName.Text = Utils.Nz(displayloginname, String.Empty)
            txtNewPassword.TextMode = TextBoxMode.Password
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If Request.Form("txtNewPassword") <> "" Then

                Dim LoginIDOfUser As Integer = Utils.Nz(cammWebManager.System_GetUserID(Request.Form("txtLoginName")), -1)
                If LoginIDOfUser = -1 Then
                    ErrMsg = "Unknown login name """ & Request.Form("txtLoginName") & """" & Utils.Nz(cammWebManager.System_GetUserID(Request.Form("txtLoginName")), String.Empty)
                Else
                    Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = cammWebManager.System_GetUserInfo(CType(LoginIDOfUser, Int64))

                    Select Case cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ValidatePasswordComplexity(Request.Form("txtNewPassword"), MyUserInfo)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                            ErrMsg = cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).ErrorMessageComplexityPoints(1)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                            ErrMsg = "The password requires to be not bigger than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredMaximumPasswordLength & " characters!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                            ErrMsg = "The password requires to be not smaller than " & cammWebManager.PasswordSecurity(MyUserInfo.AccessLevel.ID).RequiredPasswordLength & " characters!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                            ErrMsg = "The password shouldn't contain pieces of the user account profile, especially login name, first or last name!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_Unspecified
                            ErrMsg = "There are some unknown errors when validating with the security policy for passwords!"
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
                            cammWebManager.System_SetUserPassword(MyUserInfo, Request.Form("txtNewPassword"))
                            displayloginname = ""
                            ErrMsg = "Password successfully changed for user """ & MyUserInfo.LoginName & """ (" & MyUserInfo.FullName & ")!"
                    End Select
                End If
            ElseIf Request.Form("txtNewPassword") = "" And Request.Form("txtLoginName") <> "" Then
                ErrMsg = "Please enter a new password for the user!"
            End If

            If ErrMsg <> "" Then
                lblMsg.Text = ErrMsg
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     The User_Hotline_Support page allows to enter user login name and if user exists then it shows user's profile and other feature
    ''' </summary>
    Public Class User_Hotline_Support
        Inherits Page

#Region "Variable Declaration"
        Dim LookupUserID As Long
        Dim ShowUserDetailsComplete, ShowUnlockStatus, IsVisible, IsVisibleTrue As Boolean
        Dim dt As New DataTable
        Dim MyCount As Integer
        Dim StoreValue As String
        Protected lblID, lblCompany, lblSupplierNO, lblUpdateSupplierNO, lblUpdateCustomerNO, lblCustomerNO, lblAnrede, lblTitel, lblVorname, lblNachname, lblNamenszusatz As Label
        Protected lblStrasse, lblPLZ, lblORT, lblState, lblLand, lblLoginCount, lblLoginFailures, lblLoginLockedTill, lblLoginDisabled As Label
        Protected lblAccountAccessability, lblCreatedOn, lblModifiedOn, lblLastLoginOn, lblLastLoginViaremoteIP, lblFirstPreferredLanguage, lblSecondPreferredLanguage, lblThirdPreferredLanguage As Label
        Protected lblErrMsg, lblTemporaryStatus, lblCustomerHadline, lblPermanentStatus, lblLoginAccessability, lblLoginAccountAccessability, lblLogonStatus, lblAdminBlockFooter As Label
        Protected WithEvents txtLoginName As TextBox
        Protected ancUserList, ancLoginName, ancEmail, ancPermanentStatus As HtmlAnchor
        Protected WithEvents btnSubmit, btnUnlockAccount, btnShowUserDetails As Button
        Protected WithEvents rptUserShow As Repeater
        Protected cammWebManagerAdminUserInfoDetails As camm.WebManager.Controls.Administration.UsersAdditionalInformation
        Protected block_customersupplierdata As Object
        Protected tdAddDataTable As HtmlTableCell
        Protected trErrMsg, trUserLink As HtmlTableRow
        Protected tableNavigationBarPreview, tableUserDetails, tableUserResetted, tableMemberships As HtmlTable
#End Region

#Region "Page Event"
        Private Sub PageInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            cammWebManager.PageAdditionalBodyAttributes("onLoad") = "if (window['navPreview']) { document.forms['LookupUser'].LoginName.focus();};"
        End Sub

        Private Sub PageLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not IsPostBack Then
                VisibleControl(False, False)
                ButtonVisible()
                ErrorMsgVisible(False, False)
                txtLoginName.Attributes.Add("onkeypress", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnShowUserDetails.UniqueID + "').click();return false;}} else {return true}; ")

                Select Case CLng(Request.QueryString("ErrID"))
                    Case 134
                        lblErrMsg.Text = "Unknown login name! Please try to get the correct login via the normal"
                        ancUserList.HRef = "users.aspx"
                        ancUserList.InnerHtml = "Users List"
                        ErrorMsgVisible(True, True)
                    Case 135
                        lblErrMsg.Text = "Unknown error! Please contact your administrator!"
                        ErrorMsgVisible(False, True)
                End Select
                If txtLoginName.Text = "" Then
                    txtLoginName.Text = Request.QueryString("LoginName")
                End If
            End If

        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            ViewUserProfile(True, False)
            ButtonVisible()
        End Sub

        Private Sub btnShowUserDetails_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnShowUserDetails.Click
            ViewUserProfile(True, False)
        End Sub

        Private Sub btnUnlockAccount_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUnlockAccount.Click
            ViewUserProfile(False, True)
        End Sub

        Private Sub rptUserShow_ItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUserShow.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                If dt.Rows.Count > 0 Then
                    With dt.Rows(e.Item.ItemIndex)
                        For MyCount = 0 To dt.Columns.Count - 1
                            Select Case UCase(dt.Columns.Item(MyCount).ToString())
                                Case "ID"
                                    CType(e.Item.FindControl("lblID"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                Case "LOGINNAME"
                                    CType(e.Item.FindControl("ancLoginName"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), String.Empty)
                                    CType(e.Item.FindControl("ancLoginName"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINPW"
                                Case "CUSTOMERNO"
                                    CType(e.Item.FindControl("lblCustomerHadline"), Label).Text = cammWebManager.Internationalization.UpdateProfile_Descr_CustomerSupplierData
                                    CType(e.Item.FindControl("lblUpdateCustomerNO"), Label).Text = cammWebManager.Internationalization.UpdateProfile_Descr_CustomerNo
                                    CType(e.Item.FindControl("lblCustomerNO"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "SUPPLIERNO"
                                    CType(e.Item.FindControl("lblUpdateSupplierNO"), Label).Text = cammWebManager.Internationalization.UpdateProfile_Descr_SupplierNo
                                    CType(e.Item.FindControl("lblSupplierNO"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "COMPANY"
                                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ANREDE"
                                    CType(e.Item.FindControl("lblAnrede"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "TITEL"
                                    CType(e.Item.FindControl("lblTitel"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "VORNAME"
                                    CType(e.Item.FindControl("lblVorname"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "NACHNAME"
                                    CType(e.Item.FindControl("lblNachname"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "NAMENSZUSATZ"
                                    CType(e.Item.FindControl("lblNamenszusatz"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "E-MAIL"
                                    CType(e.Item.FindControl("ancEmail"), HtmlAnchor).HRef = "mailto:" & Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                    CType(e.Item.FindControl("ancEmail"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))
                                Case "STRASSE"
                                    CType(e.Item.FindControl("lblStrasse"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "PLZ"
                                    CType(e.Item.FindControl("lblPLZ"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ORT"
                                    CType(e.Item.FindControl("lblORT"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "STATE"
                                    CType(e.Item.FindControl("lblState"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LAND"
                                    CType(e.Item.FindControl("lblLand"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINCOUNT"
                                    CType(e.Item.FindControl("lblLoginCount"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINFAILURES"
                                    CType(e.Item.FindControl("lblLoginFailures"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINLOCKEDTILL"
                                    CType(e.Item.FindControl("lblLoginLockedTill"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINDISABLED"
                                    CType(e.Item.FindControl("lblLoginDisabled"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ACCOUNTACCESSABILITY"
                                    Dim MyAccessLevel As New CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                    CType(e.Item.FindControl("lblAccountAccessability"), Label).Text = Server.HtmlEncode(MyAccessLevel.Title)

                                Case "CREATEDON"
                                    CType(e.Item.FindControl("lblCreatedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "MODIFIEDON"
                                    CType(e.Item.FindControl("lblModifiedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LASTLOGINON"
                                    CType(e.Item.FindControl("lblLastLoginOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LASTLOGINVIAREMOTEIP"
                                    CType(e.Item.FindControl("lblLastLoginViaremoteIP"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item(MyCount), String.Empty))

                                Case "1STPREFERREDLANGUAGE"
                                    Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                    MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                    CType(e.Item.FindControl("lblFirstPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)

                                Case "2NDPREFERREDLANGUAGE"
                                    If Not IsDBNull(.Item(MyCount)) Then
                                        Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                        MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                        CType(e.Item.FindControl("lblSecondPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)
                                    End If

                                Case "3RDPREFERREDLANGUAGE"
                                    If Not IsDBNull(.Item(MyCount)) Then
                                        Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                        MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                                        CType(e.Item.FindControl("lblThirdPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)
                                    End If
                            End Select
                        Next
                    End With
                End If
            End If
        End Sub

#End Region

#Region "User-Defined Methods"
        Sub ViewUserProfile(ByVal ShowUserDetailsComplete As Boolean, ByVal ShowUnlockStatus As Boolean)
            If txtLoginName.Text.Trim <> "" Then
                Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Nothing
                Try
                    LookupUserID = CLng(cammWebManager.System_GetUserID(txtLoginName.Text))
                    MyUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(LookupUserID), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                    Dim gCtl As New HtmlGenericControl
                    gCtl.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CLng(LookupUserID), MyUserInfo.FullName)
                    tdAddDataTable.Controls.Add(gCtl)
                Catch
                    Response.Redirect(Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) & "?ErrID=134&LoginName=" & Server.UrlEncode(txtLoginName.Text))
                End Try

                cammWebManagerAdminUserInfoDetails.MyUserInfo = MyUserInfo

                If ShowUserDetailsComplete = True Then
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", LookupUserID)}
                    dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SELECT * FROM [Benutzer] WHERE ID = @ID ORDER BY Nachname, Vorname", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                    rptUserShow.DataSource = dt
                    rptUserShow.DataBind()
                    VisibleControl(True, True)
                    ErrorMsgVisible(False, False)

                ElseIf ShowUnlockStatus = True Then
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", LookupUserID)}
                    dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_ResetLoginLockedTill", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                    With dt.Rows(0)
                        If IsDBNull(.Item("LoginLockedTill")) Then
                            lblTemporaryStatus.Text = ":) Login hasn't been locked temporarly."
                        Else
                            lblTemporaryStatus.Text = "!! Login had been locked temporarly till " & Server.HtmlEncode(Utils.Nz(.Item("LoginLockedTill"), String.Empty)) & "  <Font color=""green"">Lock status has been resetted.</font>"
                        End If

                        If IsDBNull(.Item("LoginDisabled")) Then
                            lblPermanentStatus.Text = "!! <Font color=""red"">Login has been disabled permanently. To reactivate it, you can </Font>"
                            ancPermanentStatus.HRef = "users_update.aspx?ID=" & LookupUserID
                            ancPermanentStatus.InnerHtml = "click here"
                        Else
                            lblPermanentStatus.Text = ":) Login is functional."
                        End If

                        lblLoginAccessability.Text = Server.HtmlEncode(Utils.Nz(New camm.WebManager.WMSystem.AccessLevelInformation(CInt(Val(.Item("AccountAccessability"))), cammWebManager).Title, String.Empty))
                        lblLoginAccessability.Text = Server.HtmlEncode(Utils.Nz(New camm.WebManager.WMSystem.AccessLevelInformation(CInt(Val(.Item("AccountAccessability"))), cammWebManager).Remarks, String.Empty))
                    End With
                    VisibleControl(False, True)
                End If
            End If
        End Sub

        Sub VisibleControl(ByVal IsVisible As Boolean, ByVal IsVisibleTrue As Boolean)
            If IsVisibleTrue = False Then
                tableNavigationBarPreview.Visible = False
                tableUserDetails.Visible = False
                tableUserResetted.Visible = False
            ElseIf IsVisible = True Then
                tableNavigationBarPreview.Visible = True
                tableUserDetails.Visible = True
                tableUserResetted.Visible = False
            Else
                tableNavigationBarPreview.Visible = True
                tableUserDetails.Visible = False
                tableUserResetted.Visible = True
            End If
        End Sub

        Sub ButtonVisible()
            If txtLoginName.Text.Trim = "" Then
                btnShowUserDetails.Visible = False
                btnUnlockAccount.Visible = False
                btnSubmit.Visible = True
            Else
                btnShowUserDetails.Visible = True
                btnUnlockAccount.Visible = True
                btnSubmit.Visible = False
            End If
        End Sub

        Sub ErrorMsgVisible(ByVal IsVisible As Boolean, ByVal IsVisibleTrue As Boolean)
            If IsVisibleTrue = False Then
                trErrMsg.Visible = False
            ElseIf IsVisible = True Then
                trErrMsg.Visible = True
                ancUserList.Visible = True
            Else
                trErrMsg.Visible = True
                ancUserList.Visible = False
            End If
        End Sub
#End Region

    End Class

    ''' <summary>
    '''     The Users_Navbar_Preview page 
    ''' </summary>
    Public Class Users_Navbar_Preview
        Inherits Page

#Region "Variable Declaration"
        Dim gc, gc1 As New HtmlGenericControl
        Protected tdAnonymous, tdPublic As HtmlTableCell
        Protected lblHeading As Label
#End Region

#Region "Page Events"
        Private Sub Users_Navbar_Preview_PageLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Added if condition for showing Navigation Preview for all groups.
            If (Not Request.QueryString("GroupName") Is Nothing) Then 'AndAlso Not Request.QueryString("UserView") Is Nothing) Then
                lblHeading.Text = "Administration - Navigation preview of " + Request.QueryString("GroupName").ToString
                gc.InnerHtml += GetNavigationLinksApplication()  ' cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CType(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous, Int64), "Anonymous")
                tdAnonymous.Controls.Add(gc)
            Else
                lblHeading.Text = "Administration - Navigation preview of special users"
                gc.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CType(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous, Int64), "Anonymous")
                tdAnonymous.Controls.Add(gc)
                gc1.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CType(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Public, Int64), "Public")

                tdPublic.Controls.Add(gc1)
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        'This function will return navigation links for a specific user in the selected group.
        Private Function GetNavigationLinks(ByVal UserID As Long, ByVal Username As String) As String
            Dim serverGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
            Dim s As String = ("<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & Username & ":</b></FONT></P></TD></TR><TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">")
            Dim GroupId As String = Request.QueryString("GroupId").ToString
            Dim selectQuery As String = "select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where id_group = @GroupID Union select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where ID_Application in (select ID_Application from dbo.view_ApplicationRights where id_group =" + GroupId + "Group By ID_Application) and Id_User is not null and AppDisabled = 1"
            Dim cmd As New SqlCommand(selectQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupId
            Dim MyDt As DataTable = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If (UserID > 0) Then
                Dim webManager As WMSystem = cammWebManager
                Dim information As New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, (webManager), False)
                serverGroups = information.AccessLevel.ServerGroups
            ElseIf (UserID = -2) Then
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            Else
                If (UserID <> -1) Then
                    Throw New Exception("Invalid user information requested")
                End If
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            End If
            Dim informationArray As CompuMaster.camm.WebManager.WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)
            Dim information2 As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation
            For Each information2 In serverGroups
                If (IsAccessible(information2.MasterServer.ID, MyDt)) Then
                    Dim information3 As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                    For Each information3 In informationArray
                        If (IsAccessibleToLang(information3.ID, MyDt, UserID)) Then
                            s = String.Concat(New String() {s, "<a href=""#"" onClick=""OpenNavDemo(", information3.ID.ToString, ", '", HttpUtility.UrlEncode(information2.MasterServer.IPAddressOrHostHeader), "', '", UserID.ToString, "');"">", information2.Title, ", ", information3.LanguageName_English, "</a><br>"})
                        End If
                    Next
                End If
            Next
            s = (s & "</FONT></TD></TR><TR><TD>&nbsp;</TD></TR>")
            Return s
        End Function

        'This function will check accessibility of a servergroup from a particular location.
        Public Function DefaultNavPreviewLinks(ByVal UserID As Long, ByVal UserFullName As String, Optional ByVal WriteToCurrentContext As Boolean = False) As String

            'Temp--------------
            Dim GroupId As String = Request.QueryString("GroupId").ToString
            Dim selectQuery As String = "select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where id_group = @GroupID Union select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where ID_Application in (select ID_Application from dbo.view_ApplicationRights where id_group =" + GroupId + "Group By ID_Application) and Id_User is not null and AppDisabled = 1"
            Dim cmd As New SqlCommand(selectQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = CType(GroupId, Integer)
            Dim MyDt As DataTable = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            '------------------
            Dim serverGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()

            Dim s As String = ("<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & UserFullName & ":</b></FONT></P></TD></TR><TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">")
            serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            Dim informationArray As CompuMaster.camm.WebManager.WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)
            Dim information2 As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation
            For Each information2 In serverGroups
                If (IsAccessible(information2.MasterServer.ID, MyDt)) Then
                    Dim information3 As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                    For Each information3 In informationArray
                        If (IsAccessibleToLang(information3.ID, MyDt, UserID)) Then
                            s = String.Concat(New String() {s, "<a href=""#"" onClick=""OpenNavDemo(", information3.ID.ToString, ", '", HttpUtility.UrlEncode(information2.MasterServer.IPAddressOrHostHeader), "', '", UserID.ToString, "');"">", information2.Title, ", ", information3.LanguageName_English, "</a><br>"})
                        End If
                    Next
                End If
            Next
            s = (s & "</FONT></TD></TR><TR><TD>&nbsp;</TD></TR>")
            Return s
        End Function

        Private Function IsAccessible(ByVal ServerId As Integer, ByVal Mydt As DataTable) As Boolean
            For iCount As Integer = 0 To Mydt.Rows.Count - 1
                If (CInt(Mydt.Rows(iCount)("LocationId")) = ServerId) Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary>
        ''' Check accessibility of a servergroup application for a particular language and check the disabled application
        ''' </summary>
        ''' <param name="LangId"></param>
        ''' <param name="Mydt"></param>
        ''' <param name="userid"></param>
        ''' <returns></returns>
        Private Function IsAccessibleToLang(ByVal LangId As Integer, ByVal Mydt As DataTable, ByVal userid As Long) As Boolean
            For iCount As Integer = 0 To Mydt.Rows.Count - 1
                If (CInt(Mydt.Rows(iCount)("LanguageId")) = LangId AndAlso Not Mydt.Rows(iCount)("AppDisabled") Is DBNull.Value AndAlso CInt(Mydt.Rows(iCount)("AppDisabled")) = 0) Then
                    Return True
                Else
                    If (Not Mydt.Rows(iCount)("AppDisabled") Is DBNull.Value AndAlso CInt(Mydt.Rows(iCount)("AppDisabled")) = 1 AndAlso CLng(Mydt.Rows(iCount)("ID_User")) = userid) Then
                        Return True
                    End If
                End If
            Next
            Return False
        End Function

        Private Function GetNavigationLinksApplication() As String
            Dim serverGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
            Dim s As String = Nothing '= ("<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & "" & ":</b></FONT></P></TD></TR>")
            s += "<TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">"
            Dim GroupId As String = Request.QueryString("GroupId").ToString
            Dim selectQuery As String = "select LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where id_group = @GroupID Union select LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where ID_Application in (select ID_Application from dbo.view_ApplicationRights where id_group = @GroupID Group By ID_Application) and Id_User is not null and AppDisabled = 1"

            Dim cmd As New SqlCommand(selectQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = CType(GroupId, Integer)
            Dim MyDt As DataTable = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            Dim UserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)

            If (UserID > 0) Then
                Dim webManager As WMSystem = cammWebManager
                Dim information As New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, (webManager), False)
                serverGroups = information.AccessLevel.ServerGroups
            ElseIf (UserID = -2) Then
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            Else
                If (UserID <> -1) Then
                    Throw New Exception("Invalid user information requested")
                End If
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            End If

            'Added to apply new logic to show all server groups and markets
            Dim informationArray As CompuMaster.camm.WebManager.WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)
            Dim information2 As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation
            For Each information2 In serverGroups
                Dim information3 As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                For Each information3 In informationArray
                    s = String.Concat(New String() {s, "<a href=""#"" onClick=""OpenNavDemo(", information3.ID.ToString, ", '", HttpUtility.UrlEncode(information2.MasterServer.IPAddressOrHostHeader), "', '", UserID.ToString, "'," & Utils.Nz(Request.QueryString("GroupId"), 0) & ");"">", Utils.Nz(information2.Title, String.Empty), ", ", information3.LanguageName_English, "</a><br>"})
                Next
            Next

            s = (s & "</FONT></TD></TR><TR><TD>&nbsp;</TD></TR>")
            Return s
        End Function
#End Region

    End Class

End Namespace