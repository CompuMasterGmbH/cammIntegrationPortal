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

Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

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
                    MyUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(LookupUserID), Me.cammWebManager)
                    Dim gCtl As New HtmlGenericControl
                    gCtl.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CLng(LookupUserID), CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(MyUserInfo))
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

End Namespace