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

    '' <summary>
    ''     A page to update an application
    '' </summary>
    Public Class ApplicationUpdate
        Inherits ApplicationBasePage

#Region "Variable Declaration"
        Protected lblField_ID, lblField_ReleasedOn, lblField_ModifiedOn, lblErrMsg As Label
        Protected txtField_Level1Title, txtField_Level2Title, txtField_Level3Title As TextBox
        Protected txtField_Level4Title, txtField_Level5Title, txtField_Level6Title As TextBox
        Protected txtField_Title, txtField_TitleAdminArea, txtField_RequiredUserFlags, txtField_RequiredUserFlagsRemarks As TextBox
        Protected txtField_ResetIsNewUpdatedStatusOn, txtField_Sort, txtField_NavJSOnClick As TextBox
        Protected txtField_NavURL, txtField_NavFrame, txtField_NavTooltip, txtField_NavJSOnMOver, txtField_NavJSOnMOut As TextBox
        Protected txtField_GeneralRemarks As TextBox
        Protected rbNew, rbUpdate, rbStandard As RadioButton
        Protected rbLevel1TitleText, rbLevel2TitleText, rbLevel3TitleText As RadioButton
        Protected rbLevel4TitleText, rbLevel5TitleText, rbLevel6TitleText As RadioButton
        Protected rbLevel1TitleHTML, rbLevel2TitleHTML, rbLevel3TitleHTML As RadioButton
        Protected rbLevel4TitleHTML, rbLevel5TitleHTML, rbLevel6TitleHTML As RadioButton
        Protected cmbLocation, cmbLanguage, cmbAppDisabled, cmbAddLanguageID2URL As DropDownList
        Protected hypLastModifiedBy, hypCreatedBy, hypUsersMissingFlags As HyperLink
        Protected WithEvents btnSubmit As Button
        Protected tdAddLinks As Web.UI.HtmlControls.HtmlTableCell
        Protected cammWebManagerAdminDelegates As CompuMaster.camm.WebManager.Controls.Administration.AdministrativeDelegates
        Dim Field_LocationID, Field_Language, Field_AppDisabled, Field_AddLanguageID2URL As Integer
#End Region

#Region "Page Events"
        Private Sub ApplicationUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Update")) Then
                Response.Write("No authorization to administrate this application.")
                Response.End()
            Else
                If Not IsPostBack Then
                    SetControlValues()
                    FillDropDownLists()
                End If
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        Private Sub SetControlValues()
            Dim dtUpdate As New DataTable
            Dim dr As DataRow

            hypUsersMissingFlags.NavigateUrl = "apps_users_missing_flags.aspx?ID=" & CType(Request.QueryString("ID"), Integer)

            Try
                lblField_ID.Text = Server.HtmlEncode(Trim(Request.QueryString("ID")))
                Dim MySecurityObjectInfo As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(CInt(Trim(Request.QueryString("ID"))), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
                Dim cmd As New SqlClient.SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.Applications WHERE ID=@ID", New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CType(Request.QueryString("ID"), Integer)
                dtUpdate = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                cammWebManagerAdminDelegates.SecurityObjectInfo = MySecurityObjectInfo

                If Not dtUpdate Is Nothing AndAlso dtUpdate.Rows.Count > 0 Then
                    For Each dr In dtUpdate.Rows
                        txtField_Title.Text = MySecurityObjectInfo.Name
                        txtField_TitleAdminArea.Text = MySecurityObjectInfo.DisplayName
                        txtField_Level1Title.Text = Utils.Nz(dr("Level1Title"), String.Empty)
                        txtField_Level2Title.Text = Utils.Nz(dr("Level2Title"), String.Empty)
                        txtField_Level3Title.Text = Utils.Nz(dr("Level3Title"), String.Empty)
                        txtField_Level4Title.Text = Utils.Nz(dr("Level4Title"), String.Empty)
                        txtField_Level5Title.Text = Utils.Nz(dr("Level5Title"), String.Empty)
                        txtField_Level6Title.Text = Utils.Nz(dr("Level6Title"), String.Empty)

                        If CBool(dr("Level1TitleIsHTMLCoded")) = True Then rbLevel1TitleHTML.Checked = True Else rbLevel1TitleText.Checked = True
                        If CBool(dr("Level2TitleIsHTMLCoded")) = True Then rbLevel2TitleHTML.Checked = True Else rbLevel2TitleText.Checked = True
                        If CBool(dr("Level3TitleIsHTMLCoded")) = True Then rbLevel3TitleHTML.Checked = True Else rbLevel3TitleText.Checked = True
                        If CBool(dr("Level4TitleIsHTMLCoded")) = True Then rbLevel4TitleHTML.Checked = True Else rbLevel4TitleText.Checked = True
                        If CBool(dr("Level5TitleIsHTMLCoded")) = True Then rbLevel5TitleHTML.Checked = True Else rbLevel5TitleText.Checked = True
                        If CBool(dr("Level6TitleIsHTMLCoded")) = True Then rbLevel6TitleHTML.Checked = True Else rbLevel6TitleText.Checked = True

                        txtField_NavURL.Text = Utils.Nz(dr("NavURL"), String.Empty)
                        txtField_NavFrame.Text = Utils.Nz(dr("NavFrame"), String.Empty)
                        Field_LocationID = CInt(dr("LocationID"))
                        Field_Language = CInt(dr("LanguageID"))
                        Field_AppDisabled = CInt(IIf(CType(dr("AppDisabled"), Boolean) = True, 1, 0)) + 1
                        txtField_NavTooltip.Text = Utils.Nz(dr("NavTooltipText"), String.Empty)
                        txtField_NavJSOnMOver.Text = Utils.Nz(dr("OnMouseOver"), String.Empty)
                        txtField_NavJSOnMOut.Text = Utils.Nz(dr("OnMouseOut"), String.Empty)
                        txtField_NavJSOnClick.Text = Utils.Nz(dr("OnClick"), String.Empty)
                        txtField_Sort.Text = Utils.Nz(dr("Sort"), String.Empty)
                        txtField_RequiredUserFlags.Text = Utils.Nz(dr("RequiredUserProfileFlags"), String.Empty)
                        txtField_GeneralRemarks.Text = Utils.Nz(dr("Remarks"), String.Empty)
                        If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                            txtField_RequiredUserFlagsRemarks.Text = Utils.Nz(dr("RequiredUserProfileFlagsRemarks"), String.Empty)
                        End If

                        If CBool(dr("IsNew")) Then
                            rbNew.Checked = True
                        ElseIf CBool(dr("IsUpdated")) Then
                            rbUpdate.Checked = True
                        Else
                            rbStandard.Checked = True
                        End If

                        If Not IsDBNull(dr("ResetIsNewUpdatedStatusOn")) Then txtField_ResetIsNewUpdatedStatusOn.Text = Year(CDate(dr("ResetIsNewUpdatedStatusOn"))) & "-" & Month(CDate(dr("ResetIsNewUpdatedStatusOn"))) & "-" & Day(CType(dr("ResetIsNewUpdatedStatusOn"), Date)) & " " & Hour(CType(dr("ResetIsNewUpdatedStatusOn"), DateTime)) & ":" & Minute(CType(dr("ResetIsNewUpdatedStatusOn"), DateTime)) & ":" & Second(CType(dr("ResetIsNewUpdatedStatusOn"), Date)) Else txtField_ResetIsNewUpdatedStatusOn.Text = ""
                        If CBool(dr("AddLanguageID2URL")) Then Field_AddLanguageID2URL = 1 Else Field_AddLanguageID2URL = 0
                        txtField_RequiredUserFlags.Text = Utils.Nz(dr("RequiredUserProfileFlags"), String.Empty)
                    Next
                Else
                    lblErrMsg.Text = "Application not found!"
                End If

                Try
                    If MySecurityObjectInfo.ReleasedBy_UserID <> 0 Then
                        lblField_ReleasedOn.Text = Utils.Nz(MySecurityObjectInfo.ReleasedOn, String.Empty)
                        Dim Field_ReleasedByID As String = MySecurityObjectInfo.ReleasedBy_UserID.ToString
                        Dim Field_ReleasedByName As String = Me.SafeLookupUserFullName(MySecurityObjectInfo.ReleasedBy_UserID)
                        hypCreatedBy.NavigateUrl = "users_update.aspx?ID=" & Field_ReleasedByID
                        hypCreatedBy.Text = Server.HtmlEncode(Utils.Nz(Field_ReleasedByName, String.Empty))
                    End If
                Catch ex As ArgumentNullException
                    hypCreatedBy.Text = "{User ID 0}"
                Catch ex As Exception
                    Throw New Exception("Unexpected exception", ex)
                End Try

                lblField_ModifiedOn.Text = Utils.Nz(MySecurityObjectInfo.ModifiedOn, String.Empty)
                Try
                    Dim Field_ModifiedByID As String = ""
                    Field_ModifiedByID = MySecurityObjectInfo.ModifiedBy_UserID.ToString
                    Dim Field_ModifiedByName As String = Me.SafeLookupUserFullName(MySecurityObjectInfo.ModifiedBy_UserID)
                    hypLastModifiedBy.NavigateUrl = "users_update.aspx?ID=" & Field_ModifiedByID
                    hypLastModifiedBy.Text = Server.HtmlEncode(Utils.Nz(Field_ModifiedByName, String.Empty))
                Catch ex As ArgumentNullException
                    hypLastModifiedBy.Text = "{User ID 0}"
                Catch ex As Exception
                    Throw New Exception("Unexpected exception", ex)
                End Try

                tdAddLinks.InnerHtml = Me.RenderAuthorizations(MySecurityObjectInfo.ID)
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            Finally
                dtUpdate.Dispose()
                dr = Nothing
            End Try
        End Sub

        Private Sub FillDropDownLists()
            Dim dtUpdate As New DataTable

            Try
                'bind location dropdownlist
                cmbLocation.Items.Clear()
                If Field_LocationID = Nothing Then cmbLocation.Items.Add(New ListItem("(Please select!)", ""))
                dtUpdate = FillDataTable(New SqlCommand("Set TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select * FROM System_Servers WHERE Enabled = 1 ORDER BY ServerDescription", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtUpdate Is Nothing Then
                    For Each dr As DataRow In dtUpdate.Rows
                        cmbLocation.Items.Add(New ListItem(Utils.Nz(dr("ServerDescription"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next

                    If cmbLocation.Items.Count > 0 AndAlso Val(Field_LocationID & "") <> 0 Then cmbLocation.SelectedValue = Utils.Nz(Field_LocationID, 0).ToString
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            End Try

            Try
                'bind language dropdownlist
                cmbLanguage.Items.Clear()
                If Field_Language = Nothing Then cmbLanguage.Items.Add(New ListItem("(Please select!)", ""))
                dtUpdate = FillDataTable(New SqlCommand("Set TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select * FROM view_Languages WHERE IsActive = 1 ORDER BY Description", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtUpdate Is Nothing Then
                    For Each dr As DataRow In dtUpdate.Rows
                        cmbLanguage.Items.Add(New ListItem(Utils.Nz(dr("Description"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next

                    If cmbLanguage.Items.Count > 0 AndAlso Val(Field_Language & "") <> 0 Then cmbLanguage.SelectedValue = Utils.Nz(Field_Language, 0).ToString
                End If
            Catch ex As Exception
                Throw New Exception("Unexpected exception", ex)
            End Try

            'bind AppDisabled dropdownlist
            cmbAppDisabled.Items.Clear()
            If Field_AppDisabled = Nothing Then cmbAppDisabled.Items.Add(New ListItem("(Please select!)", ""))
            cmbAppDisabled.Items.Add(New ListItem("Yes", "2"))
            cmbAppDisabled.Items.Add(New ListItem("No", "1"))
            If Utils.Nz(Field_AppDisabled, 0) = 2 Then cmbAppDisabled.SelectedValue = "2"
            If Utils.Nz(Field_AppDisabled, 0) = 1 Then cmbAppDisabled.SelectedValue = "1"

            'bind addlanguageurl dropdownlist
            cmbAddLanguageID2URL.Items.Clear()
            If Field_AddLanguageID2URL = Nothing Then cmbAppDisabled.Items.Add(New ListItem("(Please select!)", ""))
            cmbAddLanguageID2URL.Items.Add(New ListItem("Yes", "2"))
            cmbAddLanguageID2URL.Items.Add(New ListItem("No", "1"))
            If Utils.Nz(Field_AddLanguageID2URL, 0) = 1 Then cmbAddLanguageID2URL.SelectedValue = "2"
            If Utils.Nz(Field_AddLanguageID2URL, 0) = 0 Then cmbAddLanguageID2URL.SelectedValue = "1"
        End Sub




#End Region

#Region "Control Events"


        Private Sub SetSubmitErrorText(ByVal requiredFlags As ArrayList)

            lblErrMsg.Text = Server.HtmlDecode("<hr><p><b>You changed the Required user flags,</b><br>but there are some users authorized For this app which Do Not have one Or more Of the required user flags, Or the value Of a flag Is invalid For its type. You need To <b>add these flags</b> To all listed users <b>before</b> you can update this application.</p>")
            lblErrMsg.Text &= "<p>The following flags are affected:<br>"
            Dim HtmlCode As New System.Text.StringBuilder
            For Each rFlag As String In requiredFlags
                HtmlCode.Append("<a target=""_blank"" href=""users_batchuserflageditor.aspx?AppID=" & Request.QueryString("ID") & "&Flag=" & Server.UrlEncode(Trim(rFlag)) & "&EditMode=3"">Edit Flag '" & Server.HtmlEncode(Trim(rFlag)) & "'</a><br>")
            Next
            HtmlCode.Append("</p><hr>")
            lblErrMsg.Text &= HtmlCode.ToString()
        End Sub

        ''' <summary>
        ''' Updates the application using the data entered on the page
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub UpdateApplication()
            Dim sqlParams As SqlParameter() = {New SqlParameter("@Title", Trim(txtField_Title.Text)), New SqlParameter("@TitleAdminArea", Trim(txtField_TitleAdminArea.Text)), _
                   New SqlParameter("@Level1Title", Trim(txtField_Level1Title.Text)), New SqlParameter("@Level2Title", Trim(txtField_Level2Title.Text)), New SqlParameter("@Level3Title", Trim(txtField_Level3Title.Text)), New SqlParameter("@Level4Title", Trim(txtField_Level4Title.Text)), New SqlParameter("@Level5Title", Trim(txtField_Level5Title.Text)), New SqlParameter("@Level6Title", Trim(txtField_Level6Title.Text)), _
                   New SqlParameter("@Level1TitleIsHTMLCoded", IIf(rbLevel1TitleHTML.Checked = True, True, False)), New SqlParameter("@Level2TitleIsHTMLCoded", IIf(rbLevel2TitleHTML.Checked = True, True, False)), New SqlParameter("@Level3TitleIsHTMLCoded", IIf(rbLevel3TitleHTML.Checked = True, True, False)), New SqlParameter("@Level4TitleIsHTMLCoded", IIf(rbLevel4TitleHTML.Checked = True, True, False)), New SqlParameter("@Level5TitleIsHTMLCoded", IIf(rbLevel5TitleHTML.Checked = True, True, False)), New SqlParameter("@Level6TitleIsHTMLCoded", IIf(rbLevel6TitleHTML.Checked = True, True, False)), _
                   New SqlParameter("@NavURL", Mid(Trim(txtField_NavURL.Text), 1, 512)), New SqlParameter("@NavFrame", Mid(Trim(txtField_NavFrame.Text), 1, 50)), New SqlParameter("@NavTooltipText", Mid(Trim(txtField_NavTooltip.Text), 1, 1024)), _
                   New SqlParameter("@IsNew", IIf(rbNew.Checked = True, True, False)), New SqlParameter("@IsUpdated", IIf(rbUpdate.Checked = True, True, False)), New SqlParameter("@LocationID", cmbLocation.SelectedValue), New SqlParameter("@LanguageID", cmbLanguage.SelectedValue), New SqlParameter("@ModifiedBy", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), _
                   New SqlParameter("@AppDisabled", IIf(CLng(cmbAppDisabled.SelectedValue) - 1 = 0, False, True)), New SqlParameter("@Sort", IIf(Trim(txtField_Sort.Text) = "", DBNull.Value, Trim(txtField_Sort.Text))), New SqlParameter("@ResetIsNewUpdatedStatusOn", IIf(Trim(txtField_ResetIsNewUpdatedStatusOn.Text) = "", DBNull.Value, Mid(Trim(txtField_ResetIsNewUpdatedStatusOn.Text), 1, 30))), New SqlParameter("@OnMouseOver", Mid(Trim(txtField_NavJSOnMOver.Text), 1, 512)), New SqlParameter("@OnMouseOut", Mid(Trim(txtField_NavJSOnMOut.Text), 1, 512)), _
                   New SqlParameter("@OnClick", Mid(Trim(txtField_NavJSOnClick.Text), 1, 512)), New SqlParameter("@AddLanguageID2URL", IIf(Utils.Nz(cmbAddLanguageID2URL.SelectedValue, 0) <> 1, True, False)), New SqlParameter("@ID", CInt(Request.QueryString("ID")))}

            ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateApp", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            Dim remarks As String = ""
            If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                remarks = ", RequiredUserProfileFlagsRemarks = @UserProfileFlagsRemarks"
            End If

            Dim cmd3 As New SqlCommand("Update Applications_CurrentAndInactiveOnes SET RequiredUserProfileFlags = @RequiredFlags, Remarks = @GeneralRemarks" & remarks & " Where ID = @ID", New SqlConnection(cammWebManager.ConnectionString))
            cmd3.Parameters.Add("@ID", SqlDbType.Int).Value = CType(lblField_ID.Text, Integer)
            cmd3.Parameters.Add("@RequiredFlags", SqlDbType.NVarChar).Value = Trim(txtField_RequiredUserFlags.Text)
            cmd3.Parameters.Add("@GeneralRemarks", SqlDbType.NVarChar).Value = txtField_GeneralRemarks.Text.Trim()
            If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                cmd3.Parameters.Add("@UserProfileFlagsRemarks", SqlDbType.NVarChar).Value = txtField_RequiredUserFlagsRemarks.Text.Trim()
            End If
            ExecuteNonQuery(cmd3, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        Private Sub btnSubmitClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If CInt(Val(Request.QueryString("ID") & "")) <> 0 AndAlso cmbLocation.SelectedValue <> String.Empty AndAlso cmbAppDisabled.SelectedItem.Text <> String.Empty Then
                lblErrMsg.Text = ""
                Dim invalidFlags As New ArrayList

                Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex()
                If MyDBVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 AndAlso txtField_RequiredUserFlags IsNot Nothing Then
                    Dim requiredApplicationFlags() As String = txtField_RequiredUserFlags.Text.Split(CChar(","))
                    If requiredApplicationFlags.Length > 0 Then
                        Dim cmd As SqlClient.SqlCommand
                        If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                            cmd = New SqlClient.SqlCommand("Select ID_User From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID AND IsNull(IsDenyRule, 0) = 0 UNION select ID_User from Memberships_EffectiveRulesWithClonesNthGrade where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group is not null and ID_Application = @ID AND IsNull(IsDenyRule, 0) = 0)) as a GROUP BY ID_User", New SqlConnection(cammWebManager.ConnectionString))
                        Else
                            cmd = New SqlClient.SqlCommand("Select ID_User From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID union select ID_User from Memberships where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group  is not null and ID_Application = @ID)) as a GROUP BY ID_User", New SqlConnection(cammWebManager.ConnectionString))
                        End If
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = CType(Trim(lblField_ID.Text), Integer)

                        Dim tbleUsersAllowedForApplication As System.Collections.Generic.List(Of Long) = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoGenericList(Of Long)(cmd, Automations.AutoOpenAndCloseAndDisposeConnection)
                        Dim UserAccountsToValidate As WMSystem.UserInformation() = cammWebManager.System_GetUserInfos(tbleUsersAllowedForApplication.ToArray)

                        For Each userInfo As WMSystem.UserInformation In UserAccountsToValidate
                            Dim validationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(userInfo, requiredApplicationFlags, True)
                            For Each validationResult As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult In validationResults
                                If validationResult.ValidationResult <> CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.Success Then
                                    If Not invalidFlags.Contains(validationResult.Flag) Then
                                        invalidFlags.Add(validationResult.Flag)
                                    End If
                                End If
                            Next
                        Next

                        If invalidFlags.Count > 0 Then
                            SetSubmitErrorText(invalidFlags)
                            Exit Sub
                        End If
                    End If
                End If
                Try
                    UpdateApplication()
                    If lblErrMsg.Text = "" Then
                        Utils.RedirectTemporary(HttpContext.Current, "apps.aspx#ID" & Request.QueryString("ID"))
                    End If
                Catch ex As Exception
                    If cammWebManager.System_DebugLevel >= 3 Then
                        lblErrMsg.Text = "Application update failed! (" & ex.Message & ex.StackTrace & ")"
                    ElseIf cammWebManager.System_DebugLevel = 5 Then
                        lblErrMsg.Text = "Application update failed! (" & ex.ToString & ")"
                    Else
                        lblErrMsg.Text = "Application update failed!"
                    End If
                End Try
            Else
                lblErrMsg.Text = "Please specify the applicable location and language and the navigation information."
            End If
        End Sub
#End Region

    End Class

End Namespace