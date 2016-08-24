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

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A helper page which processes the import in fact
    ''' </summary>
    Public Class ImportUsersProcessing
        Inherits ImportBase

        Protected WithEvents LiteralStep4ProcessingMessageLog As Label
        ''' <summary>
        '''     How many user accounts shall be imported at once?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Web requests are regulary limited to 30 seconds. So we're not allowed to import all user accounts in one request. We have to split the processing of the whole list into multiple requests if we don't want the request to stop unexpectedly.
        ''' </remarks>
        Protected Overridable ReadOnly Property NumberOfUsersToImportInOneRoundTrip() As Integer
            Get
                Return 5
            End Get
        End Property

        Private Sub PageOnPreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            ExecuteImport()
        End Sub

        Private _TotalRecords As Integer
        ''' <summary>
        '''     The number of total records in the import table
        ''' </summary>
        ''' <value></value>
        Protected ReadOnly Property TotalRecords() As Integer
            Get
                Return _TotalRecords
            End Get
        End Property

        Private _ProgressState As Integer
        ''' <summary>
        '''     The number of already processed records from the import table
        ''' </summary>
        ''' <value></value>
        Protected ReadOnly Property ProgressState() As Integer
            Get
                Return _ProgressState
            End Get
        End Property
        ''' <summary>
        '''     Calculate the values for the output to the user
        ''' </summary>
        Protected Sub CalculateProgressState()

            _TotalRecords = ImportTable.Rows.Count

            _ProgressState = 0
            Dim ImportDoneColumnIndex As Integer = ImportTable.Columns("User_ImportDone").Ordinal
            For MyCounter As Integer = 0 To ImportTable.Rows.Count - 1
                If CType(ImportTable.Rows(MyCounter)(ImportDoneColumnIndex), Boolean) = True Then
                    _ProgressState += 1
                End If
            Next

        End Sub
        ''' <summary>
        '''     Manage the import of several user accounts (while this page request)
        ''' </summary>
        Protected Overridable Sub ExecuteImport()
            Try
                If ImportTable Is Nothing Then
                    Throw New Exception("Import table missing, but it should do")
                End If

                'Search rows which need to be processed
                Dim rowsNotYetProcessed As DataRow()
                rowsNotYetProcessed = ImportTable.Select("User_ImportDone = 0")

                If rowsNotYetProcessed Is Nothing Then
                    Throw New Exception("Nothing to do")
                End If

                'Process those top 5 rows
                For MyCounter As Integer = 0 To Math.Min(Me.NumberOfUsersToImportInOneRoundTrip - 1, rowsNotYetProcessed.Length - 1)
                    Dim MyRow As DataRow = rowsNotYetProcessed(MyCounter)
                    ImportUser(MyRow, CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.RowIndex(MyRow) + 1)
                    rowsNotYetProcessed(MyCounter)("User_ImportDone") = True
                Next

                'Calculate new output values for UI
                CalculateProgressState()

            Catch ex As Exception
                If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Me.LiteralStep4ProcessingMessageLog.Text = "<font color=""red"">" & Server.HtmlDecode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>"
                Else
                    Me.LiteralStep4ProcessingMessageLog.Text = "<font color=""red"">" & Server.HtmlDecode(ex.Message) & "</font>"
                End If
            End Try

        End Sub
        ''' <summary>
        '''     Import some user details defined by the given datarow of user data
        ''' </summary>
        ''' <param name="userData">A datarow from the import table</param>
        ''' <param name="rowID">An ID to identify the row by the user in case of errors</param>
        Protected Sub ImportUser(ByVal userData As DataRow, ByVal rowID As Integer)

            Try
                Dim userLoginName As String = Utils.Nz(userData("User_LoginName"), "")

                If Me.ImportAction = ImportActions.InsertOrUpdate Or Me.ImportAction = ImportActions.InsertOnly Or Me.ImportAction = ImportActions.UpdateOnly Then
                    'Find logins already existing/not existing
                    Dim userIDToUpdate As Long = CLng(cammWebManager.System_GetUserID(userLoginName, True))
                    Dim MyUser As CompuMaster.camm.WebManager.WMSystem.UserInformation
                    If userIDToUpdate = -1 And (Me.ImportAction = ImportBase.ImportActions.InsertOnly Or Me.ImportAction = ImportBase.ImportActions.InsertOrUpdate) Then
                        'User account not found and one of the insert actions selected
                        MyUser = New WMSystem.UserInformation(Nothing, userLoginName, "", False, "", WMSystem.Sex.Undefined, "", "", "", "", "", "", "", "", "", 1, 0, 0, False, False, False, 0, cammWebManager, "", Nothing)
                    ElseIf userIDToUpdate = -1 And Me.ImportAction = ImportBase.ImportActions.UpdateOnly Then
                        'Import action is UpdateOnly, but user account doesn't exist
                        Throw New Exception("User account doesn't exist: " & userLoginName)
                    ElseIf userIDToUpdate <> -1 And (Me.ImportAction = ImportBase.ImportActions.UpdateOnly Or Me.ImportAction = ImportBase.ImportActions.InsertOrUpdate) Then
                        'User account found and one of the update actions selected
                        MyUser = cammWebManager.System_GetUserInfo(userIDToUpdate)
                        MyUser.ReloadFullUserData()
                    ElseIf userIDToUpdate <> -1 And Me.ImportAction = ImportBase.ImportActions.InsertOnly Then
                        'Import action is InsertOnly, but user account already exists
                        Throw New Exception("User account already exist: " & userLoginName)
                    Else
                        'should never go here
                        Throw New Exception("Invalid operation")
                    End If

                    'Assign user profile values 
                    ApplyUserProfileData(MyUser, userData, Me.ImportFileCulture)
                    If userData.Table.Columns.Contains("User_PhoneNumber") Then
                        ApplyUserProfileFlagConditionally_Internal(MyUser.PhoneNumber, Trim(Utils.Nz(userData("User_PhoneNumber"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
                    End If
                    If userData.Table.Columns.Contains("User_MobileNumber") Then
                        ApplyUserProfileFlagConditionally_Internal(MyUser.MobileNumber, Trim(Utils.Nz(userData("User_MobileNumber"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
                    End If
                    If userData.Table.Columns.Contains("User_FaxNumber") Then
                        ApplyUserProfileFlagConditionally_Internal(MyUser.FaxNumber, Trim(Utils.Nz(userData("User_FaxNumber"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
                    End If
                    If userData.Table.Columns.Contains("User_Position") Then
                        ApplyUserProfileFlagConditionally_Internal(MyUser.Position, Trim(Utils.Nz(userData("User_Position"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
                    End If
                    'And save all changes (as well as the password)
                    If userIDToUpdate = -1 Then
                        'New account - and set up the password
                        Dim userPassword As String
                        If userData.Table.Columns.Contains("User_Password") Then
                            userPassword = Utils.Nz(userData("User_Password"), "")
                        Else
                            userPassword = Nothing
                        End If
                        Try
                            If userPassword = "" Then
                                If Me.SuppressNotificationMails = True Then
                                    Throw New Exception("Must create a new password when suppressing all notifications to user") 'so, DO FAIL completely!
                                Else
                                    Throw New CompuMaster.camm.WebManager.PasswordTooWeakException("Force creating a new password while notification of user is enabled")
                                End If
                            End If
                            MyUser.Save(userPassword, Me.SuppressNotificationMails)
                        Catch ex As CompuMaster.camm.WebManager.PasswordTooWeakException
                            'Password too weak - use a random password now
                            Dim userAccesslevel As Integer = CType(userData("User_AccessLevel"), Integer)
                            Dim newPW As String = cammWebManager.PasswordSecurity.InspectionSeverities(userAccesslevel).CreateRandomSecurePassword
                            If MyUser.IDLong <> Nothing Then
                                'User account has already been created in the try block
                                MyUser.SetPassword(newPW, SuppressNotificationMails)
                            Else
                                MyUser.Save(newPW, SuppressNotificationMails)
                            End If
                            Dim passwordMessage As String
                            If Me.SuppressNotificationMails Then
                                'usually not called code blocksince throwing a regular exception in try-block above doesn't end up in this code block any more
                                'this behaviour is desired by redesign of import tool by JW on 2016-01-08
                                passwordMessage = "Password too weak; it originally was """ & userPassword & """ for login name """ & userLoginName & """, the new password is now """ & newPW & """."
                            Else
                                passwordMessage = "Password too weak; it originally was """ & userPassword & """ for login name """ & userLoginName & """, the new password is now a random password."
                            End If
                            Me.MessagesLog &= "<font color=""#A04444"">Row #" & rowID & ": " & passwordMessage & "</font>" & vbNewLine
                        End Try
                    Else
                        'Existing account - never change the password
                        MyUser.Save(Me.SuppressNotificationMails)
                    End If

                    'Assign the memberships and authorizations as required (changes are made directly in the database, so no saving again required)
                    ApplyMembershipsAndAuthorizations(MyUser, userData, Me.ImportFileCulture, Me.ImportActionMemberships, Me.ImportActionAuthorizations)

                ElseIf Me.ImportAction = ImportActions.Remove Then
                    'Find logins already existing
                    Dim userIDToRemove As Long = CLng(cammWebManager.System_GetUserID(userLoginName, True))
                    If userIDToRemove = -1 Then
                        'Account not found
                        Me.MessagesLog &= "<font color=""red"">Row #" & rowID & ": account """ & userLoginName & """ not found</font>" & vbNewLine
                    Else
                        'Remove
                        Dim userToRemove As CompuMaster.camm.WebManager.WMSystem.UserInformation
                        userToRemove = cammWebManager.System_GetUserInfo(userIDToRemove)
                        userToRemove.LoginDeleted = True
                        userToRemove.Save(SuppressNotificationMails)
                        Me.MessagesLog &= "Account """ & userLoginName & """ removed." & vbNewLine
                    End If

                Else
                    Throw New Exception("Invalid import action " & Me.ImportAction.ToString)
                End If

            Catch ex As Exception
                If True Then 'Debugging without stacktrace is very difficult-->always true! 'cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Me.MessagesLog &= "<font color=""red"">Row #" & rowID & ": " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>" & vbNewLine
                Else
                    Me.MessagesLog &= "<font color=""red"">Row #" & rowID & ": " & Server.HtmlEncode(ex.Message) & "</font>" & vbNewLine
                End If
            End Try

        End Sub
        ''' <summary>
        '''     Assign the user profile information from the datarow to the user object
        ''' </summary>
        ''' <param name="user">The user information object which shall be updated</param>
        ''' <param name="userData">The import data record</param>
        ''' <param name="culture">The culture of the import data (when a string has to be converted to a datetime, etc.)</param>
        ''' <remarks>
        '''     All profile information will be copied here except loginname and password
        ''' </remarks>
        Protected Overridable Sub ApplyUserProfileData(ByRef user As WMSystem.UserInformation, ByVal userData As DataRow, ByVal culture As System.Globalization.CultureInfo)

            'User account data
            If userData.Table.Columns.Contains("User_EMailAddress") Then
                ApplyUserProfileFlagConditionally_Internal(user.EMailAddress, Trim(Utils.Nz(userData("User_EMailAddress"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_Gender") Then
                If Utils.Nz(userData("User_Gender"), "") = "" Then
                    user.Gender = WMSystem.Sex.Undefined
                Else
                    user.Gender = CType(System.Enum.Parse(GetType(CompuMaster.camm.WebManager.WMSystem.Sex), Utils.Nz(userData("User_Gender"), "")), WMSystem.Sex)
                End If
            End If
            If userData.Table.Columns.Contains("User_AcademicTitle") Then
                ApplyUserProfileFlagConditionally_Internal(user.AcademicTitle, Trim(Utils.Nz(userData("User_AcademicTitle"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_FirstName") Then
                ApplyUserProfileFlagConditionally_Internal(user.FirstName, Trim(Utils.Nz(userData("User_FirstName"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_LastName") Then
                ApplyUserProfileFlagConditionally_Internal(user.LastName, Trim(Utils.Nz(userData("User_LastName"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_NameAddition") Then
                ApplyUserProfileFlagConditionally_Internal(user.NameAddition, Trim(Utils.Nz(userData("User_NameAddition"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_Company") Then
                ApplyUserProfileFlagConditionally_Internal(user.Company, Trim(Utils.Nz(userData("User_Company"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_Position") Then
                ApplyUserProfileFlagConditionally_Internal(user.Position, Trim(Utils.Nz(userData("User_Position"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_Street") Then
                ApplyUserProfileFlagConditionally_Internal(user.Street, Trim(Utils.Nz(userData("User_Street"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_ZipCode") Then
                ApplyUserProfileFlagConditionally_Internal(user.ZipCode, Trim(Utils.Nz(userData("User_ZipCode"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_Location") Then
                ApplyUserProfileFlagConditionally_Internal(user.Location, Trim(Utils.Nz(userData("User_Location"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_State") Then
                ApplyUserProfileFlagConditionally_Internal(user.State, Trim(Utils.Nz(userData("User_State"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_Country") Then
                ApplyUserProfileFlagConditionally_Internal(user.Country, Trim(Utils.Nz(userData("User_Country"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_PhoneNumber") Then
                ApplyUserProfileFlagConditionally_Internal(user.PhoneNumber, Trim(Utils.Nz(userData("User_PhoneNumber"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_MobileNumber") Then
                ApplyUserProfileFlagConditionally_Internal(user.MobileNumber, Trim(Utils.Nz(userData("User_MobileNumber"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_FaxNumber") Then
                ApplyUserProfileFlagConditionally_Internal(user.FaxNumber, Trim(Utils.Nz(userData("User_FaxNumber"), "")), Not Me.ImportOverwriteWithEmptyCellValues)
            End If
            If userData.Table.Columns.Contains("User_PreferredLanguage1") Then
                user.PreferredLanguage1 = New WMSystem.LanguageInformation(CType(userData("User_PreferredLanguage1"), Integer), cammWebManager)
            End If
            If userData.Table.Columns.Contains("User_PreferredLanguage2") Then
                Dim langID As Integer = CType(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_PreferredLanguage2"), "")), Integer)
                user.PreferredLanguage2 = New WMSystem.LanguageInformation(langID, cammWebManager)
            End If
            If userData.Table.Columns.Contains("User_PreferredLanguage3") Then
                Dim langID As Integer = CType(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_PreferredLanguage3"), "")), Integer)
                user.PreferredLanguage3 = New WMSystem.LanguageInformation(langID, cammWebManager)
            End If
            If userData.Table.Columns.Contains("User_LoginDisabled") Then
                Dim value As Boolean = Boolean.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_LoginDisabled"), "")))
                user.LoginDisabled = value
            End If
            If userData.Table.Columns.Contains("User_LoginDeleted") Then
                Dim value As Boolean = Boolean.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_LoginDeleted"), "")))
                user.LoginDeleted = value
            End If
            If userData.Table.Columns.Contains("User_LoginLockedTemporary") Then
                Dim value As Boolean = Boolean.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_LoginLockedTemporary"), "")))
                user.LoginLockedTemporary = value
            End If
            If userData.Table.Columns.Contains("User_ExternalAccount") Then
                user.ExternalAccount = Trim(Utils.Nz(userData("User_ExternalAccount"), ""))
            End If
            If userData.Table.Columns.Contains("User_AccessLevel") Then
                Dim value As Integer = Integer.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_AccessLevel"), "")), culture)
                user.AccessLevel = New WMSystem.AccessLevelInformation(value, cammWebManager)
            End If
            If userData.Table.Columns.Contains("User_AccountAuthorizationsAlreadySet") Then
                Dim value As Boolean = Boolean.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_AccountAuthorizationsAlreadySet"), "")))
                user.AccountAuthorizationsAlreadySet = value
            End If
            If userData.Table.Columns.Contains("User_AccountProfileValidatedByEMailTest") Then
                Dim value As Boolean = Boolean.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_AccountProfileValidatedByEMailTest"), "")))
                user.AccountProfileValidatedByEMailTest = value
            End If
            If userData.Table.Columns.Contains("User_AutomaticLogonAllowedByMachineToMachineCommunication") Then
                Dim value As Boolean = Boolean.Parse(Utils.StringNotEmptyOrNothing(Utils.Nz(userData("User_AutomaticLogonAllowedByMachineToMachineCommunication"), "")))
                user.AutomaticLogonAllowedByMachineToMachineCommunication = value
            End If
            If userData.Table.Columns.Contains("User_AdditionalFlags") Then
                Dim value As String = Utils.Nz(userData("User_AdditionalFlags"), "")
                'Just update the existing AdditionalFlags collection with newer/updated/removed values (removal = assignment of empty string)
                Utils.ReFillNameValueCollection(user.AdditionalFlags, value)
            End If

        End Sub

        Protected Sub ApplyUserProfileFlagConditionally_Internal(ByRef userInfoField As String, newValue As String, skipIfEmptyValue As Boolean)
            If skipIfEmptyValue = True And newValue = Nothing Then
                'don't update
            Else
                userInfoField = newValue
            End If
        End Sub
        ''' <summary>
        '''     Assign the memberships and authorizations to a user's account
        ''' </summary>
        ''' <param name="user">The user information object which shall be updated</param>
        ''' <param name="userData">The import data record</param>
        ''' <param name="culture">The culture of the import data (when a string has to be converted to a datetime, etc.)</param>
        ''' <param name="importActionMemberships">The type of the import</param>
        ''' <param name="importActionAuthorizations">The type of the import</param>
        Protected Overridable Sub ApplyMembershipsAndAuthorizations(ByVal user As WMSystem.UserInformation, ByVal userData As DataRow, ByVal culture As System.Globalization.CultureInfo, ByVal importActionMemberships As ImportActions, ByVal importActionAuthorizations As ImportActions)

            If Not (importActionMemberships = 0 OrElse importActionMemberships = ImportBase.ImportActions.InsertOnly OrElse importActionMemberships = ImportBase.ImportActions.FitExact) Then
                Throw New ArgumentException("Invalid import action", "importActionMemberships")
            End If
            If Not (importActionAuthorizations = 0 OrElse importActionAuthorizations = ImportBase.ImportActions.InsertOnly OrElse importActionAuthorizations = ImportBase.ImportActions.FitExact) Then
                Throw New ArgumentException("Invalid import action", "importActionAuthorizations")
            End If

            'Prepare correct notifications class
            Dim MyNotifications As WebManager.Notifications.INotifications
            If Me.SuppressNotificationMails = True Then
                MyNotifications = New WebManager.Notifications.NoNotifications(cammWebManager)
            Else
                MyNotifications = cammWebManager.Notifications()
            End If

            'Memberships
            If userData.Table.Columns.Contains("User_Memberships_AllowRule_GroupIDs") And importActionMemberships <> Nothing Then
                'Collect group IDs
                Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(userData("User_Memberships_AllowRule_GroupIDs"), ""))
                Dim values As String() = value.Split(New Char() {","c})
                Dim requiredGroups As New ArrayList
                For MyConversionTestCounter As Integer = 0 To values.Length - 1
                    If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                        requiredGroups.Add(Integer.Parse(values(MyConversionTestCounter), culture))
                    End If
                Next
                'Remove unwanted memberships
                Dim CurrentMemberships As WMSystem.GroupInformation() = user.MembershipsByRule().AllowRule
                If importActionMemberships = ImportBase.ImportActions.FitExact Then
                    For MyCounterIsCurrently As Integer = 0 To CurrentMemberships.Length - 1
                        'Evaluate if wanted membership
                        Dim ShallBeThere As Boolean = False
                        For MyCounterShallBe As Integer = 0 To requiredGroups.Count - 1
                            If CType(requiredGroups(MyCounterShallBe), Integer) = CurrentMemberships(MyCounterIsCurrently).ID Then
                                ShallBeThere = True
                            End If
                        Next
                        'Remove unwanted membership
                        If ShallBeThere = False Then
                            user.RemoveMembership(CurrentMemberships(MyCounterIsCurrently).ID, False)
                        End If
                    Next
                End If
                'Add missing memberships
                For MyCounterShallBe As Integer = 0 To requiredGroups.Count - 1
                    'Evaluate if missing membership
                    Dim AlreadyExist As Boolean = False
                    For MyCounterIsCurrently As Integer = 0 To CurrentMemberships.Length - 1
                        If CType(requiredGroups(MyCounterShallBe), Integer) = CurrentMemberships(MyCounterIsCurrently).ID Then
                            AlreadyExist = True
                        End If
                    Next
                    'Add missing membership
                    If AlreadyExist = False Then
                        user.AddMembership(CType(requiredGroups(MyCounterShallBe), Integer), False, MyNotifications)
                    End If
                Next
            End If
            If userData.Table.Columns.Contains("User_Memberships_DenyRule_GroupIDs") And importActionMemberships <> Nothing Then
                'Collect group IDs
                Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(userData("User_Memberships_DenyRule_GroupIDs"), ""))
                Dim values As String() = value.Split(New Char() {","c})
                Dim requiredGroups As New ArrayList
                For MyConversionTestCounter As Integer = 0 To values.Length - 1
                    If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                        requiredGroups.Add(Integer.Parse(values(MyConversionTestCounter), culture))
                    End If
                Next
                'Remove unwanted memberships
                Dim CurrentMemberships As WMSystem.GroupInformation() = user.MembershipsByRule().DenyRule
                If importActionMemberships = ImportBase.ImportActions.FitExact Then
                    For MyCounterIsCurrently As Integer = 0 To CurrentMemberships.Length - 1
                        'Evaluate if wanted membership
                        Dim ShallBeThere As Boolean = False
                        For MyCounterShallBe As Integer = 0 To requiredGroups.Count - 1
                            If CType(requiredGroups(MyCounterShallBe), Integer) = CurrentMemberships(MyCounterIsCurrently).ID Then
                                ShallBeThere = True
                            End If
                        Next
                        'Remove unwanted membership
                        If ShallBeThere = False Then
                            user.RemoveMembership(CurrentMemberships(MyCounterIsCurrently).ID, True)
                        End If
                    Next
                End If
                'Add missing memberships
                For MyCounterShallBe As Integer = 0 To requiredGroups.Count - 1
                    'Evaluate if missing membership
                    Dim AlreadyExist As Boolean = False
                    For MyCounterIsCurrently As Integer = 0 To CurrentMemberships.Length - 1
                        If CType(requiredGroups(MyCounterShallBe), Integer) = CurrentMemberships(MyCounterIsCurrently).ID Then
                            AlreadyExist = True
                        End If
                    Next
                    'Add missing membership
                    If AlreadyExist = False Then
                        user.AddMembership(CType(requiredGroups(MyCounterShallBe), Integer), True, MyNotifications)
                    End If
                Next
            End If
            'Authorizations
            If userData.Table.Columns.Contains("User_Authorizations_AllowRule_AppIDs") And importActionAuthorizations <> Nothing Then
                'Collect security object IDs and other required infos for authorizations
                Dim RequiredSecurityObjects As Integer() = ApplyMembershipsAndAuthorizations_LoadAppIDs(userData("User_Authorizations_AllowRule_AppIDs"), culture)
                Dim RequiredSecurityObjects_IsDev As Boolean() = ApplyMembershipsAndAuthorizations_LoadIsDevRules(userData("User_Authorizations_AllowRule_IsDevRule"), RequiredSecurityObjects)
                Dim RequiredSecurityObjects_SrvGroupIDs As Integer() = ApplyMembershipsAndAuthorizations_LoadSrvGroupIDs(userData("User_Authorizations_AllowRule_SrvGroupID"), RequiredSecurityObjects, culture)
                'Apply all required auth changes
                ApplyMembershipsAndAuthorizations_ApplyAuths(importActionAuthorizations, user, False, False, RequiredSecurityObjects, RequiredSecurityObjects_SrvGroupIDs, RequiredSecurityObjects_IsDev, MyNotifications)
                ApplyMembershipsAndAuthorizations_ApplyAuths(importActionAuthorizations, user, True, False, RequiredSecurityObjects, RequiredSecurityObjects_SrvGroupIDs, RequiredSecurityObjects_IsDev, MyNotifications)
            End If
            If userData.Table.Columns.Contains("User_Authorizations_DenyRule_AppIDs") And importActionAuthorizations <> Nothing Then
                'Collect security object IDs and other required infos for authorizations
                Dim RequiredSecurityObjects As Integer() = ApplyMembershipsAndAuthorizations_LoadAppIDs(userData("User_Authorizations_DenyRule_AppIDs"), culture)
                Dim RequiredSecurityObjects_IsDev As Boolean() = ApplyMembershipsAndAuthorizations_LoadIsDevRules(userData("User_Authorizations_DenyRule_IsDevRule"), RequiredSecurityObjects)
                Dim RequiredSecurityObjects_SrvGroupIDs As Integer() = ApplyMembershipsAndAuthorizations_LoadSrvGroupIDs(userData("User_Authorizations_DenyRule_SrvGroupID"), RequiredSecurityObjects, culture)
                'Apply all required auth changes
                ApplyMembershipsAndAuthorizations_ApplyAuths(importActionAuthorizations, user, False, True, RequiredSecurityObjects, RequiredSecurityObjects_SrvGroupIDs, RequiredSecurityObjects_IsDev, MyNotifications)
                ApplyMembershipsAndAuthorizations_ApplyAuths(importActionAuthorizations, user, True, True, RequiredSecurityObjects, RequiredSecurityObjects_SrvGroupIDs, RequiredSecurityObjects_IsDev, MyNotifications)
            End If
        End Sub

        Friend Shared Function ApplyMembershipsAndAuthorizations_LoadAppIDs(valueCell As Object, culture As System.Globalization.CultureInfo) As Integer()
            Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(valueCell, ""))
            Dim values As String() = value.Split(New Char() {","c})
            Dim requiredSecurityObjectsList As New ArrayList
            For MyConversionTestCounter As Integer = 0 To values.Length - 1
                If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                    requiredSecurityObjectsList.Add(Integer.Parse(values(MyConversionTestCounter), culture))
                End If
            Next
            Return CType(requiredSecurityObjectsList.ToArray(GetType(Integer)), Integer())
        End Function

        Private Function ApplyMembershipsAndAuthorizations_LoadIsDevRules(valueCell As Object, RequiredSecurityObjects As Integer()) As Boolean()
            Dim Result As Boolean()
            Dim values As String() = Utils.StringNotNothingOrEmpty(Utils.Nz(valueCell, "")).Split(New Char() {","c})
            If values.Length = 0 Then
                'JIT-create initialized array with values False
                ReDim Result(RequiredSecurityObjects.Length)
            Else
                Dim requiredSecurityObjectsList_IsDev As New ArrayList
                For MyConversionTestCounter As Integer = 0 To values.Length - 1
                    If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                        requiredSecurityObjectsList_IsDev.Add(Boolean.Parse(values(MyConversionTestCounter)))
                    Else
                        requiredSecurityObjectsList_IsDev.Add(False)
                    End If
                Next
                Result = CType(requiredSecurityObjectsList_IsDev.ToArray(GetType(Boolean)), Boolean())
            End If
            If Result.Length <> RequiredSecurityObjects.Length Then
                Throw New Exception("IsDev array must be empty or same size as AppIDs array")
            End If
            Return Result
        End Function

        Private Function ApplyMembershipsAndAuthorizations_LoadSrvGroupIDs(valueCell As Object, RequiredSecurityObjects As Integer(), culture As System.Globalization.CultureInfo) As Integer()
            Dim Result As Integer()
            Dim values As String() = Utils.StringNotNothingOrEmpty(Utils.Nz(valueCell, "")).Split(New Char() {","c})
            If values.Length = 0 Then
                'JIT-create initialized array with values False
                ReDim Result(RequiredSecurityObjects.Length)
            Else
                Dim requiredSecurityObjectsList_SrvGroupID As New ArrayList
                For MyConversionTestCounter As Integer = 0 To values.Length - 1
                    If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                        requiredSecurityObjectsList_SrvGroupID.Add(Integer.Parse(values(MyConversionTestCounter), culture))
                    Else
                        requiredSecurityObjectsList_SrvGroupID.Add(0)
                    End If
                Next
                Result = CType(requiredSecurityObjectsList_SrvGroupID.ToArray(GetType(Integer)), Integer())
            End If
            If Result.Length <> RequiredSecurityObjects.Length Then
                Throw New Exception("SrvGroupIDs array must be empty or same size as AppIDs array")
            End If
            Return Result
        End Function

        ''' <summary>
        ''' Apply all required auth changes
        ''' </summary>
        ''' <param name="importActionAuthorizations"></param>
        ''' <param name="user"></param>
        ''' <param name="isDevRule">Chosen calling mode of this method will only change this rule type</param>
        ''' <param name="isDenyRule">Chosen calling mode of this method will only change this rule type</param>
        ''' <param name="RequiredSecurityObjects"></param>
        ''' <param name="RequiredSecurityObjects_SrvGroupIDs"></param>
        ''' <param name="RequiredSecurityObjects_IsDevRule"></param>
        ''' <param name="MyNotifications"></param>
        Private Sub ApplyMembershipsAndAuthorizations_ApplyAuths(importActionAuthorizations As ImportBase.ImportActions, user As WMSystem.UserInformation, isDevRule As Boolean, isDenyRule As Boolean, RequiredSecurityObjects As Integer(), RequiredSecurityObjects_SrvGroupIDs As Integer(), RequiredSecurityObjects_IsDevRule As Boolean(), MyNotifications As WebManager.Notifications.INotifications)
            'Remove unwanted authorizations
            Dim CurrentAuthorizations As WMSystem.SecurityObjectAuthorizationForUser()
            If isDenyRule = False And isDevRule = False Then
                CurrentAuthorizations = user.AuthorizationsByRule.AllowRuleStandard
            ElseIf isDenyRule = False And isDevRule = True Then
                CurrentAuthorizations = user.AuthorizationsByRule.AllowRuleDevelopers
            ElseIf isDenyRule = True And isDevRule = False Then
                CurrentAuthorizations = user.AuthorizationsByRule.DenyRuleStandard
            Else 'If isDenyRule = True And isDevRule = True Then
                CurrentAuthorizations = user.AuthorizationsByRule.DenyRuleDevelopers
            End If
            If importActionAuthorizations = ImportBase.ImportActions.FitExact Then
                ApplyMembershipsAndAuthorizations_DropAuths(user, CurrentAuthorizations, False, True, RequiredSecurityObjects, RequiredSecurityObjects_SrvGroupIDs, RequiredSecurityObjects_IsDevRule)
            End If
            'Add missing authorizations
            ApplyMembershipsAndAuthorizations_AddMissingAuths(user, CurrentAuthorizations, False, True, RequiredSecurityObjects, RequiredSecurityObjects_SrvGroupIDs, RequiredSecurityObjects_IsDevRule, MyNotifications)
        End Sub

        ''' <summary>
        ''' Add missing authorizations
        ''' </summary>
        Private Sub ApplyMembershipsAndAuthorizations_AddMissingAuths(user As WMSystem.UserInformation, CurrentAuthorizations As WMSystem.SecurityObjectAuthorizationForUser(), isDevRule As Boolean, isDenyRule As Boolean, RequiredSecurityObjects As Integer(), RequiredSecurityObjects_SrvGroupIDs As Integer(), RequiredSecurityObjects_IsDevRule As Boolean(), MyNotifications As WebManager.Notifications.INotifications)
            For MyCounterShallBe As Integer = 0 To RequiredSecurityObjects.Length - 1
                If RequiredSecurityObjects_IsDevRule(MyCounterShallBe) = isDevRule Then
                    Dim RequiredSrvGroupID As Integer = RequiredSecurityObjects_SrvGroupIDs(MyCounterShallBe)
                    Dim RequiredSecObjID As Integer = RequiredSecurityObjects(MyCounterShallBe)
                    'Evaluate if missing authorization
                    Dim AlreadyExist As Boolean = False
                    For MyCounterIsCurrently As Integer = 0 To CurrentAuthorizations.Length - 1
                        If RequiredSecObjID = CurrentAuthorizations(MyCounterIsCurrently).SecurityObjectID AndAlso _
                                    RequiredSrvGroupID = CurrentAuthorizations(MyCounterIsCurrently).ServerGroupID Then
                            AlreadyExist = True
                        End If
                    Next
                    'Add missing authorization
                    If AlreadyExist = False Then
                        user.AddAuthorization(RequiredSecObjID, RequiredSrvGroupID, isDevRule, isDenyRule, MyNotifications)
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Remove unwanted authorizations
        ''' </summary>
        ''' <param name="user"></param>
        ''' <param name="CurrentAuthorizations"></param>
        ''' <param name="isDevRule"></param>
        ''' <param name="isDenyRule"></param>
        ''' <param name="RequiredSecurityObjects"></param>
        ''' <param name="RequiredSecurityObjects_SrvGroupIDs"></param>
        Private Sub ApplyMembershipsAndAuthorizations_DropAuths(user As WMSystem.UserInformation, CurrentAuthorizations As WMSystem.SecurityObjectAuthorizationForUser(), isDevRule As Boolean, isDenyRule As Boolean, RequiredSecurityObjects As Integer(), RequiredSecurityObjects_SrvGroupIDs As Integer(), RequiredSecurityObjects_IsDevRule As Boolean())
            For MyCounterIsCurrently As Integer = 0 To CurrentAuthorizations.Length - 1
                If CurrentAuthorizations(MyCounterIsCurrently).IsDeveloperAuthorization = isDevRule Then
                    'Evaluate if wanted authorization
                    Dim SrvGroupID As Integer = CurrentAuthorizations(MyCounterIsCurrently).ServerGroupID
                    Dim ShallBeThere As Boolean = False
                    For MyCounterShallBe As Integer = 0 To RequiredSecurityObjects.Length - 1
                        If RequiredSecurityObjects(MyCounterShallBe) = CurrentAuthorizations(MyCounterIsCurrently).SecurityObjectID AndAlso _
                                    RequiredSecurityObjects_SrvGroupIDs(MyCounterShallBe) = SrvGroupID AndAlso _
                                    RequiredSecurityObjects_IsDevRule(MyCounterShallBe) = isDevRule Then
                            ShallBeThere = True
                        End If
                    Next
                    'Remove unwanted authorization
                    If ShallBeThere = False Then
                        user.RemoveAuthorization(CurrentAuthorizations(MyCounterIsCurrently).SecurityObjectID, SrvGroupID, isDevRule, isDenyRule)
                    End If
                End If
            Next
        End Sub
        ''' <summary>
        '''     Error handler for unexpected page errors
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     Show the error message on the page output in the IFrame
        ''' </remarks>
        Private Sub PageOnError(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Error
            Response.Clear()
            Response.Write(Server.GetLastError.ToString.Replace(vbNewLine, "<br>"))
            Server.ClearError()
            Response.End()
        End Sub

    End Class

End Namespace