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
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration


    ''' <summary>
    '''     Clone user
    ''' </summary>
    Public Class UsersClone
        Inherits Page

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

        Protected ReadOnly Property UserID() As Long
            Get
                If Request.QueryString("ID") = "" Then
                    Return 0
                Else
                    Return CType(Request.QueryString("ID"), Long)
                End If
            End Get
        End Property

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

        ''' <summary>
        ''' Checks whether username is already in use
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="args"></param>
        Public Sub ValidatorNewUserLoginName_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs)
            If args.Value.Length > 50 Then
                args.IsValid = False
                CType(source, CustomValidator).ErrorMessage = "Loginname is too long. Max. 50 characters."
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
        ''' <summary>
        ''' Initialize Datatable. Contains data that was not copied e.g. protected flags
        ''' Does NOT contain values that are manually unchecked
        ''' </summary>
        Private Sub initNotCopiedDataDatatable()
            If Session("cwmCloneUserNotCopiedDataDt") Is Nothing Then
                notCopiedData = New DataTable("NotCopiedData")
                notCopiedData.Columns.Add("Key")
                notCopiedData.Columns.Add("Value")
                Session("cwmCloneUserNotCopiedDataDt") = notCopiedData
            End If
        End Sub
        ''' <summary>
        ''' Enumeration of data types which automatically cannot be copied
        ''' </summary>
        Enum notCopiedDataEnum As Integer
            AdditionalFlag = 1
            Membership = 2
            Authorization = 3
        End Enum
        ''' <summary>
        ''' Add a new entry that automatically cannot be copied
        ''' </summary>
        ''' <param name="dataType">Type of data, e.g. AdditionalFlag</param>
        ''' <param name="value">The value, e.g. value of additional flag or name of membership</param>
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
        ''' <summary>
        ''' Removes an entry, e.g. authorization has required flags, but its a protected flag
        ''' Handles check or uncheck the authorization-checkbox
        ''' </summary>
        ''' <param name="dataType"></param>
        ''' <param name="value"></param>
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
        ''' <summary>
        '''     ID and (English) title of all available languages (markets) + the currently selected language (when it is different)
        ''' </summary>
        ''' <param name="alwaysIncludeThisLanguage">The currently selected language should always appear</param>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        Private Function AvailableLanguages(ByVal alwaysIncludeThisLanguage As Integer) As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Description_English FROM System_Languages WHERE (IsActive = 1 AND NOT ID = 10000) OR ID = " & alwaysIncludeThisLanguage & " ORDER BY Description_English", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function
        ''' <summary>
        '''     ID and title of the available access levels
        ''' </summary>
        ''' <remarks>Intended for the preferred languages dropdown boxes
        ''' </remarks>
        Private Function AvailableAccessLevels() As DictionaryEntry()
            Return ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(New SqlClient.SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Title FROM System_AccessLevels ORDER BY Title", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

#Region " Assign data to form "
        ''' <summary>
        ''' Assigns the source user information to the webform
        ''' </summary>
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
        ''' <summary>
        ''' Assigns the additional flags information of the source user to the webform
        ''' </summary>
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
        ''' <summary>
        ''' Check whether it's allowed to copy the additional flag
        ''' Customizing in /sysdata/users_clone.aspx
        ''' </summary>
        ''' <param name="flagName"></param>
        Public Overridable Function AdditionalFlagAllowCopy(ByVal flagName As String) As Boolean
            Return True
        End Function
        ''' <summary>
        ''' Assigns the membership information of the source user to the webform
        ''' </summary>
        Private Sub AssignMembershipsToPnl()
            Dim MyGroupInfosAllowRule As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = UserInfo.MembershipsByRule().AllowRule
            Dim MyGroupInfosDenyRule As CompuMaster.camm.WebManager.WMSystem.GroupInformation() = UserInfo.MembershipsByRule().DenyRule

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
                If MyGroupInfo.IsSystemGroupByServerGroup = False Then
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
                End If
            Next
        End Sub
        ''' <summary>
        ''' Assigns the authorization information of the source user to the webform
        ''' </summary>
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
                    Dim SecObjID As Integer = 0
                    Try
                        SecObjID = MyUserAuthInfo.SecurityObjectInfo.ID
                    Catch
                        cammWebManager.Log.Warn("Missing security object with ID " & MyUserAuthInfo.SecurityObjectID & " in authorizations for user ID " & UserInfo.IDLong)
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
                    If SecObjID <> 0 Then
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
                    End If
                Next
            End If
        End Sub
        ''' <summary>
        ''' Handles the checkbox check changed event to control required flags
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
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

        ''' <summary>
        ''' Get required flags for given securityobject
        ''' </summary>
        ''' <param name="SecurityObjectID"></param>
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

        Sub BtnSubmitClick(ByVal sender As Object, ByVal e As EventArgs) Handles Button_Submit.Click
            notCopiedData = CType(Session("cwmCloneUserNotCopiedDataDt"), DataTable)
            Me.Page.Validate()
            If Page.IsValid Then
                Dim ClonedUser As CompuMaster.camm.WebManager.WMSystem.UserInformation = Clone(New_Field_LoginName.Text, CShort(IIf(cmbAnrede.SelectedValue = Nothing, 0, cmbAnrede.SelectedValue)), New_Field_Titel.Text, New_Field_Vorname.Text, New_Field_Namenszusatz.Text, New_Field_Nachname.Text, New_Field_e_mail.Text, New_Field_Password.Text)
                If Not ClonedUser Is Nothing Then
                    lblStatusMsg.ForeColor = Drawing.Color.Green
                    lblStatusMsg.Text = "Cloning was successful! New userID: " & ClonedUser.IDLong & ". <a href=""users_update.aspx?ID=" & ClonedUser.IDLong & """>>>Update UserProfile</a><br />"
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

    End Class

End Namespace