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
                Dim Success As Boolean = False
                Try
                    MyUserInfo = cammWebManager.System_GetUserInfo(CType(Request.QueryString("ID"), Long))
                    If MyUserInfo Is Nothing Then
                        SetUserDoesNotExistErrorMessage()
                        Return
                    End If
                    MyUserInfo.LoginDeleted = True
                    MyUserInfo.Save()
                    Success = True
                Catch ex As Exception
                    ErrMsg = "User erasing failed!"
                    If cammWebManager.System_DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        ErrMsg &= "<br />" & ex.ToString.Replace(System.Environment.NewLine, "<br />")
                    Else
                        ErrMsg &= " (" & ex.Message & ")"
                    End If
                End Try
                If Success Then Response.Redirect("users.aspx")
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

End Namespace