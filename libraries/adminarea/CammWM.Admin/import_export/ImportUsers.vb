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
    '''     Import a list of users - wizard
    ''' </summary>
    ''' <remarks>
    '''     Step 1: Upload the CSV file
    '''     Step 2: Choose the charset of the CSV file (default is UTF-8)
    '''     Step 3: Verify the table content (the set of columns) and select an action (insert, update, delete)
    '''     Step 4: Processing
    '''     Step 5: Finish - view the log
    ''' </remarks>
    Public Class ImportUsers
        Inherits ImportBase

#Region "Declarations"
        Protected WithEvents ButtonStep1Submit As Button
        Protected WithEvents ButtonStep2Submit As Button
        Protected WithEvents ButtonStep3Submit As Button
        Protected WithEvents ButtonStep4Submit As Button
        Protected WithEvents ButtonStep5Submit As Button
        Protected WithEvents ButtonStep2PreviousStep As Button
        Protected WithEvents ButtonStep3PreviousStep As Button
        Protected WithEvents ButtonStep4PreviousStep As Button
        Protected WithEvents ButtonStep5PreviousStep As Button
        Protected WithEvents PanelStep1 As Panel
        Protected WithEvents PanelStep2 As Panel
        Protected WithEvents PanelStep3 As Panel
        Protected WithEvents PanelStep4 As Panel
        Protected WithEvents PanelStep5 As Panel
        Protected WithEvents ButtonStep2PreviewData As Button
        Protected WithEvents TextboxStep2Charset As TextBox
        Protected WithEvents TextboxStep3Culture As TextBox
        Protected WithEvents DatagridStep2DataPreview As DataGrid
        Protected WithEvents DatagridStep3ColumnsCheck As DataGrid
        Protected WithEvents RadioStep3ActionInsertOnly As RadioButton
        Protected WithEvents RadioStep3ActionUpdateOnly As RadioButton
        Protected WithEvents RadioStep3ActionInsertUpdate As RadioButton
        Protected WithEvents RadioStep3ActionRemoveOnly As RadioButton
        Protected WithEvents CheckboxStep3SuppressAllNotificationMails As CheckBox
        Protected WithEvents PanelStep3MembershipsImportType As Panel
        Protected WithEvents PanelStep3AuthorizationsImportType As Panel
        Protected WithEvents PanelStep3AdditionalFlagsImportType As Panel
        Protected WithEvents PanelStep3OverrideWithEmptyValuesFromImportFile As Panel
        Protected WithEvents RadioStep3ActionMembershipsFitExact As RadioButton
        Protected WithEvents RadioStep3ActionMembershipsInsertOnly As RadioButton
        Protected WithEvents RadioStep3ActionAuthorizationsFitExact As RadioButton
        Protected WithEvents RadioStep3ActionAuthorizationsInsertOnly As RadioButton
        Protected WithEvents RadioStep3ActionAdditionalFlagsFitExact As RadioButton
        Protected WithEvents RadioStep3ActionAdditionalFlagsDefinedKeysOnly As RadioButton
        Protected WithEvents RadioStep3OverrideWithEmptyValuesFromImportFileYes As RadioButton
        Protected WithEvents RadioStep3OverrideWithEmptyValuesFromImportFileNo As RadioButton
        Protected LabelStep1Errors As Label
        Protected LabelStep2Errors As Label
        Protected LabelStep3Errors As Label
        Protected LabelStep4Errors As Label
        Protected LabelStep5Log As Label
        Protected IFrameStep4ProcessingWindow As UI.HtmlControls.HtmlGenericControl
#End Region

        Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init

            If Not Page.IsPostBack Then
                SwitchStep(1)
            End If
            DatagridStep2DataPreview.HeaderStyle.Font.Bold = True
            DatagridStep3ColumnsCheck.HeaderStyle.Font.Bold = True

        End Sub
        ''' <summary>
        '''     Switch the form to the desired step
        ''' </summary>
        ''' <param name="stepNumber"></param>
        Protected Sub SwitchStep(ByVal stepNumber As Integer)
            If stepNumber < 1 Or stepNumber > 5 Then
                Throw New ArgumentException("Invalid step number")
            End If

            CurrentStepNumber = stepNumber

            PanelStep1.Visible = (stepNumber = 1)
            PanelStep2.Visible = (stepNumber = 2)
            PanelStep3.Visible = (stepNumber = 3)
            PanelStep4.Visible = (stepNumber = 4)
            PanelStep5.Visible = (stepNumber = 5)

            If stepNumber = 4 Then
                'Start the import in the first page request
                'MessagesLog = Nothing
                StartImport()
            End If

            If stepNumber = 1 OrElse stepNumber = 4 Then
                Me.cammWebManagerAdminMenu.AnchorText = ""
                Me.cammWebManagerAdminMenu.AnchorTitle = ""
                Me.cammWebManagerAdminMenu.HRef = ""
            Else
                Me.cammWebManagerAdminMenu.AnchorText = "Restart"
                Me.cammWebManagerAdminMenu.AnchorTitle = "Reset and begin a new import"
                Me.cammWebManagerAdminMenu.HRef = "users_import.aspx"
            End If

        End Sub
        ''' <summary>
        '''     The current step number
        ''' </summary>
        ''' <value></value>
        Protected Property CurrentStepNumber() As Integer
            Get
                Return Utils.Nz(ViewState("StepNumber"), 0)
            End Get
            Set(ByVal Value As Integer)
                ViewState("StepNumber") = Value
            End Set
        End Property
        ''' <summary>
        '''     The temporary filename of the location of the uploaded file
        ''' </summary>
        ''' <value></value>
        Private Property ImportFile() As String
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.RawFileData"), String)
            End Get
            Set(ByVal Value As String)
                Session("WebManager.Administration.Import.UserList.RawFileData") = Value
            End Set
        End Property

        Protected Sub StartImport()

            'Hide navigation buttons until the import has been completed
            Me.ButtonStep4PreviousStep.Visible = False
            Me.ButtonStep4Submit.Visible = False

            'Add a progress state columns to the import table
            If ImportTable.Columns.Contains("User_ImportDone") = False Then
                'Add new column
                ImportTable.Columns.Add("User_ImportDone", GetType(Boolean))
                'Predefine values to False
                Dim ImportDoneColumnIndex As Integer = ImportTable.Columns("User_ImportDone").Ordinal
                For MyCounter As Integer = 0 To ImportTable.Rows.Count - 1
                    ImportTable.Rows(MyCounter)(ImportDoneColumnIndex) = False
                Next
            End If

        End Sub

#Region "Buttons GoNext"
        Private Sub ButtonStep1Submit_Click(ByVal Source As Object, ByVal e As EventArgs) Handles ButtonStep1Submit.Click
            If Me.PrepareStep2 = True Then
                Me.SwitchStep(2)
                'Reset to empty grid, again
                Me.DatagridStep2DataPreview.DataSource = Nothing
                Me.DatagridStep2DataPreview.DataBind()
            End If
        End Sub

        Private Sub ButtonStep2Submit_Click(ByVal Source As Object, ByVal e As EventArgs) Handles ButtonStep2Submit.Click
            If Me.PrepareStep3 = True Then
                Me.SwitchStep(3)
                PrepareStep4(True) 'Show first validation results in datagrid
            End If
        End Sub

        Private Sub ButtonStep3Submit_Click(ByVal Source As Object, ByVal e As EventArgs) Handles ButtonStep3Submit.Click
            If Me.PrepareStep4(False) = True Then
                Me.SwitchStep(4)
                Me.MessagesLog = Nothing
            End If
        End Sub

        Private Sub ButtonStep4Submit_Click(ByVal Source As Object, ByVal e As EventArgs) Handles ButtonStep4Submit.Click
            Me.SwitchStep(5)
        End Sub

        Private Sub ButtonStep5Submit_Click(ByVal Source As Object, ByVal e As EventArgs) Handles ButtonStep5Submit.Click
            'Go back to step 1
            Me.SwitchStep(1)
        End Sub

#End Region

        Private Sub TryToRemoveOldTempFile(ByVal tempfile As String)
            If tempfile <> Nothing Then
                Try
                    System.IO.File.Delete(tempfile)
                Catch
                End Try
            End If
        End Sub

#Region "Prepare next steps by processing the current step"
        Private Function PrepareStep2() As Boolean
            'Receive the file
            If Request.Files.Count = 1 AndAlso Request.Files(0).FileName <> Nothing AndAlso Request.Files(0).ContentLength <> Nothing Then
                If Request.Files(0).ContentLength > 10000000 Then
                    LabelStep1Errors.Text = "File size is too big (max. 10 MB)<br>"
                    Return False
                Else
                    Try
                        TryToRemoveOldTempFile(ImportFile)
                        'ImportFile = System.IO.Path.GetTempFileName 'doesn't run in .NET 2.x non-full-trusted environments
                        ImportFile = Server.MapPath(cammWebManager.DownloadHandler.CreatePlainDownloadLink(DownloadHandler.DownloadLocations.WebManagerUserSession, "cwm/admin/userimport", Guid.NewGuid.ToString("n") & ".csv"))
                        Dim tempFolder As String = System.IO.Path.GetDirectoryName(ImportFile)
                        If System.IO.Directory.Exists(tempFolder) = False Then System.IO.Directory.CreateDirectory(tempFolder)
                        Request.Files(0).SaveAs(ImportFile)
                        Me.LabelStep1Errors.Text = Nothing
                        Me.LabelStep2Errors.Text = Nothing
                        Return True
                    Catch ex As Exception
                        TryToRemoveOldTempFile(ImportFile)
                        If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                            LabelStep1Errors.Text = ex.ToString.Replace(vbNewLine, "<br>") & "<br>"
                        Else
                            LabelStep1Errors.Text = ex.Message & "<br>"
                        End If
                        Return False
                    End Try
                End If
            Else
                LabelStep1Errors.Text = "You must upload a file before you can continue<br>"
                Me.LabelStep2Errors.Text = Nothing
                Return False
            End If
        End Function

        Private Function PrepareStep3() As Boolean
            Try
                Me.ImportTable = CompuMaster.camm.WebManager.Administration.Tools.Data.Csv.ReadDataTableFromCsvFile(ImportFile, True, Me.TextboxStep2Charset.Text, ","c, """"c, False, True)
                LabelStep2Errors.Text = Nothing
                LabelStep3Errors.Text = Nothing
                Return True
            Catch ex As Exception
                Me.ImportTable = Nothing
                If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    LabelStep2Errors.Text = ex.ToString.Replace(vbNewLine, "<br>") & "<br>"
                Else
                    LabelStep2Errors.Text = ex.Message & "<br>"
                End If
                Return False
            End Try
        End Function

        Private Function PrepareStep4(ByVal preValidateOnly As Boolean) As Boolean
            Dim CheckResult As New DataTable
            CheckResult.Columns.Add("Required column", GetType(String))
            CheckResult.Columns.Add("Data type", GetType(String))
            CheckResult.Columns.Add("Errors", GetType(String))
            Try
                If ImportTable Is Nothing Then
                    Throw New Exception("No import data available, has the session been reset?")
                End If
                'Determine the correct culture
                If Trim(Me.TextboxStep3Culture.Text) = Nothing Or LCase(Trim(Me.TextboxStep3Culture.Text)) = "invariant" Then
                    ImportFileCulture = System.Globalization.CultureInfo.InvariantCulture
                Else
                    ImportFileCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Trim(Me.TextboxStep3Culture.Text))
                End If
            Catch ex As Exception
                If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    LabelStep3Errors.Text = "Error resolving the culture ID: " & ex.ToString.Replace(vbNewLine, "<br>") & "<br>"
                Else
                    LabelStep3Errors.Text = "Error resolving the culture ID: " & ex.Message & "<br>"
                End If
                Return False
            End Try
            Dim Result As Boolean = True
            Try
                'Reset errors tag
                LabelStep3Errors.Text = Nothing
                Me.LabelStep4Errors.Text = Nothing
                Me.MessagesLog = Nothing
                'Determine the selected import action type
                If Me.RadioStep3ActionInsertOnly.Checked = True Then
                    Me.ImportAction = ImportBase.ImportActions.InsertOnly
                ElseIf Me.RadioStep3ActionInsertUpdate.Checked = True Then
                    Me.ImportAction = ImportBase.ImportActions.InsertOrUpdate
                ElseIf Me.RadioStep3ActionRemoveOnly.Checked = True Then
                    Me.ImportAction = ImportBase.ImportActions.Remove
                ElseIf Me.RadioStep3ActionUpdateOnly.Checked = True Then
                    Me.ImportAction = ImportBase.ImportActions.UpdateOnly
                Else
                    If preValidateOnly = False Then
                        LabelStep3Errors.Text = "You must select an action before you continue<br>"
                        Result = False
                    End If
                End If
                If Me.RadioStep3ActionAuthorizationsFitExact.Checked = True Then
                    Me.ImportActionAuthorizations = ImportBase.ImportActions.FitExact
                Else
                    Me.ImportActionAuthorizations = ImportBase.ImportActions.InsertOnly
                End If
                If Me.RadioStep3ActionMembershipsFitExact.Checked = True Then
                    Me.ImportActionMemberships = ImportBase.ImportActions.FitExact
                Else
                    Me.ImportActionMemberships = ImportBase.ImportActions.InsertOnly
                End If
                If Me.RadioStep3OverrideWithEmptyValuesFromImportFileYes Is Nothing OrElse Me.RadioStep3OverrideWithEmptyValuesFromImportFileYes.Checked Then
                    Me.ImportOverwriteWithEmptyCellValues = True
                Else
                    Me.ImportOverwriteWithEmptyCellValues = False
                End If

                'Suppress notification mails?
                Me.SuppressNotificationMails = Me.CheckboxStep3SuppressAllNotificationMails.Checked
                'Check column by column for existance as well as correct type of content
                'Note on critical chars: 
                '- ChrW(160) = Alt + 0160 = WhiteSpace For Numerics -> won't be trimmed by standard TRIM methods -> handle with cause! = usually not used (but already seen in import files from FarEast/Asia)
                '- ChrW(65533) = Alt + 0160 saved/loaded in wrongly encoded ANSI/UTF-8 files might result in char no. 65533!
                '- ChrW(164) = leading character in front of unicode chars in UTF-8 files -> indicates a wrong encoding setup
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_LoginName", GetType(String), True, True, True, False, False, Nothing, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_EMailAddress", GetType(String), False, True, False, False, False, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Password", GetType(String), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Gender", GetType(CompuMaster.camm.WebManager.WMSystem.Sex), False, True, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_AcademicTitle", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_FirstName", GetType(String), False, True, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_LastName", GetType(String), False, True, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_NameAddition", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Company", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Position", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Street", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_ZipCode", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Location", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_State", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Country", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_PhoneNumber", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_MobileNumber", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_FaxNumber", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_PreferredLanguage1", GetType(Integer), False, True, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_PreferredLanguage2", GetType(Integer), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_PreferredLanguage3", GetType(Integer), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_LoginDisabled", GetType(Boolean), False, False, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_LoginDeleted", GetType(Boolean), False, False, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_LoginLockedTemporary", GetType(Boolean), False, False, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_ExternalAccount", GetType(String), False, False, False, True, True, New Char() {ChrW(65533), ChrW(164), ChrW(160)})
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_AccessLevel", GetType(Integer), False, True, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_AccountAuthorizationsAlreadySet", GetType(Boolean), False, False, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_AccountProfileValidatedByEMailTest", GetType(Boolean), False, False, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_AutomaticLogonAllowedByMachineToMachineCommunication", GetType(Boolean), False, False, False, False, False)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_AdditionalFlags", GetType(String), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Memberships_AllowRule_GroupIDs", GetType(Integer()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Memberships_DenyRule_GroupIDs", GetType(Integer()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_AllowRule_AppIDs", GetType(Integer()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_AllowRule_IsDevRule", GetType(Boolean()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_AllowRule_IsDenyRule", GetType(Boolean()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_AllowRule_SrvGroupID", GetType(Integer()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_DenyRule_AppIDs", GetType(Integer()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_DenyRule_IsDevRule", GetType(Boolean()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_DenyRule_IsDenyRule", GetType(Boolean()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations_DenyRule_SrvGroupID", GetType(Integer()), False, False, False, True, True)
                'Verify that column "User_LoginName" doesn't contain any duplicates
                If Result = True AndAlso Me.ImportTable.Columns.Contains("User_LoginName") Then
                    Dim duplicates As Hashtable = CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.FindDuplicates(Me.ImportTable.Columns("User_LoginName"))
                    If duplicates.Count > 0 Then
                        LabelStep3Errors.Text = "Error: duplicates found in column ""User_LoginName""<br>"
                        If preValidateOnly = False Then
                            Result = False
                        End If
                    End If
                End If
                'Verify that column "User_Authorizations_*Rule_IsDe*/SrvGroupID" has got array length or 0 (zero) or same length as User_Authorizations_*Rule_AppIDs array
                If Result = True AndAlso (Me.ImportTable.Columns.Contains("User_Authorizations_AllowRule_IsDevRule") OrElse
                                Me.ImportTable.Columns.Contains("User_Authorizations_AllowRule_IsDenyRule") OrElse
                                Me.ImportTable.Columns.Contains("User_Authorizations_AllowRule_SrvGroupID") OrElse
                                Me.ImportTable.Columns.Contains("User_Authorizations_DenyRule_IsDevRule") OrElse
                                Me.ImportTable.Columns.Contains("User_Authorizations_DenyRule_IsDenyRule") OrElse
                                Me.ImportTable.Columns.Contains("User_Authorizations_DenyRule_SrvGroupID")) Then
                    'TODO: further implementation required for import with columns User_Authorizations_*Rule_IsDe*/SrvGroupID
                    LabelStep3Errors.Text = "Error: column support not yet implemented for ""User_Authorizations_*Rule_IsDevRule"", ""User_Authorizations_*Rule_IsDenyRule"", ""User_Authorizations_*Rule_SrvGroupID""<br>"
                    If preValidateOnly = False Then
                        Result = False
                    End If
                End If
                If Result = True Then
                    Me.AddTestResultRowToTable(CheckResult, "User_Memberships_AllowRule_GroupIDs|RequiredFlags", CheckForRequiredFlagsForNewMemberships(Me.ImportTable))
                    Me.AddTestResultRowToTable(CheckResult, "User_Authorizations_AllowRule_AppIDs|RequiredFlags", CheckForRequiredFlagsForNewAuthorizations(Me.ImportTable))
                End If
                'Show import action radio buttons for memberships or authorizations when required
                If Me.ImportTable.Columns.Contains("User_Memberships_AllowRule_GroupIDs") OrElse Me.ImportTable.Columns.Contains("User_Memberships_DenyRule_GroupIDs") Then
                    Me.PanelStep3MembershipsImportType.Visible = True
                Else
                    Me.PanelStep3MembershipsImportType.Visible = False
                End If
                If Me.ImportTable.Columns.Contains("User_Authorizations_AllowRule_AppIDs") OrElse Me.ImportTable.Columns.Contains("User_Authorizations_DenyRule_AppIDs") Then
                    Me.PanelStep3AuthorizationsImportType.Visible = True
                Else
                    Me.PanelStep3AuthorizationsImportType.Visible = False
                End If
                If Me.ImportTable.Columns.Contains("User_AdditionalFlags") Then
                    Me.PanelStep3AdditionalFlagsImportType.Visible = True
                Else
                    Me.PanelStep3AdditionalFlagsImportType.Visible = False
                End If
            Catch ex As Exception
                If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    LabelStep3Errors.Text = "Error resolving the culture ID: " & ex.ToString.Replace(vbNewLine, "<br>") & "<br>"
                Else
                    LabelStep3Errors.Text = "Error resolving the culture ID: " & ex.Message & "<br>"
                End If
                Result = False
            End Try
            'Bind the verification results data
            Me.DatagridStep3ColumnsCheck.DataSource = CheckResult
            Me.DatagridStep3ColumnsCheck.DataBind()

            Return Result
        End Function

        Protected Overridable Function CheckForRequiredFlagsForNewMemberships(ByVal importData As DataTable) As String
            Dim Result As String = ""
            If importData.Columns.Contains("User_Memberships_AllowRule_GroupIDs") Then
                Dim ToAddGroupIDs As New Generic.List(Of Integer)
                'Collect group IDs
                For MyRowCounter As Integer = 0 To importData.Rows.Count - 1
                    Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(importData.Rows(MyRowCounter)("User_Memberships_AllowRule_GroupIDs"), ""))
                    Dim values As String() = value.Split(New Char() {","c})
                    For MyConversionTestCounter As Integer = 0 To values.Length - 1
                        If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                            Dim GroupID As Integer = Integer.Parse(values(MyConversionTestCounter), System.Globalization.CultureInfo.InvariantCulture)
                            If ToAddGroupIDs.Contains(GroupID) = False Then
                                ToAddGroupIDs.Add(GroupID)
                            End If
                        End If
                    Next
                Next
                'Combine all infos to a datatable
                Dim RequiredFlagsInfo As New DataTable
                RequiredFlagsInfo.Columns.Add("Required flag", GetType(String))
                RequiredFlagsInfo.Columns.Add("Required by group", GetType(String))
                Dim GroupInfos As WMSystem.GroupInformation() = Me.cammWebManager.System_GetGroupInfos(ToAddGroupIDs.ToArray)
                For MyGroupCounter As Integer = 0 To GroupInfos.Length - 1
                    Dim RequiredFlags As String() = GroupInfos(MyGroupCounter).RequiredAdditionalFlags
                    For MyCounter As Integer = 0 To RequiredFlags.Length - 1
                        Dim row As DataRow = RequiredFlagsInfo.NewRow
                        row(0) = RequiredFlags(MyCounter)
                        row(1) = GroupInfos(MyGroupCounter).ID & ": " & GroupInfos(MyGroupCounter).Name
                        RequiredFlagsInfo.Rows.Add(row)
                    Next
                Next
                'Convert info datatable to HTML
                Result = WebManager.Administration.Tools.Data.DataTables.ConvertToHtmlTable(RequiredFlagsInfo, "", "", "style=""border-width: 1px; border-style: solid; width: 100%;""")
            End If

            Return Result
        End Function

        Protected Overridable Function CheckForRequiredFlagsForNewAuthorizations(ByVal importData As DataTable) As String
            Dim Result As String = ""
            If importData.Columns.Contains("User_Authorizations_AllowRule_AppIDs") Then
                Dim ToAddSecObjIDs As New Generic.List(Of Integer)
                'Collect group IDs
                For MyRowCounter As Integer = 0 To importData.Rows.Count - 1
                    Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(importData.Rows(MyRowCounter)("User_Authorizations_AllowRule_AppIDs"), ""))
                    Dim values As String() = value.Split(New Char() {","c})
                    For MyConversionTestCounter As Integer = 0 To values.Length - 1
                        If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                            Dim SecObjID As Integer = Integer.Parse(values(MyConversionTestCounter), System.Globalization.CultureInfo.InvariantCulture)
                            If ToAddSecObjIDs.Contains(SecObjID) = False Then
                                ToAddSecObjIDs.Add(SecObjID)
                            End If
                        End If
                    Next
                Next
                'Combine all infos to a datatable
                Dim RequiredFlagsInfo As New DataTable
                RequiredFlagsInfo.Columns.Add("Required flag", GetType(String))
                RequiredFlagsInfo.Columns.Add("Required by security object", GetType(String))
                Dim SecObjInfos As WMSystem.SecurityObjectInformation() = Me.cammWebManager.System_GetSecurityObjectInformations(ToAddSecObjIDs.ToArray)
                For MyGroupCounter As Integer = 0 To SecObjInfos.Length - 1
                    Dim RequiredFlags As String() = SecObjInfos(MyGroupCounter).RequiredAdditionalFlags
                    For MyCounter As Integer = 0 To RequiredFlags.Length - 1
                        Dim row As DataRow = RequiredFlagsInfo.NewRow
                        row(0) = RequiredFlags(MyCounter)
                        row(1) = SecObjInfos(MyGroupCounter).ID & ": " & SecObjInfos(MyGroupCounter).Name
                        RequiredFlagsInfo.Rows.Add(row)
                    Next
                Next
                'Convert info datatable to HTML
                Result = WebManager.Administration.Tools.Data.DataTables.ConvertToHtmlTable(RequiredFlagsInfo, "", "", "style=""border-width: 1px; border-style: solid; width: 100%;""")
            End If

            Return Result
        End Function

        ''' <summary>
        ''' Add a test result record to both the testResults collection and also to the checkTable table
        ''' </summary>
        ''' <param name="testResultsWithErrors"></param>
        ''' <param name="testResultsWithWarnings"></param>
        ''' <param name="isError">True for error records, false for warning items</param>
        ''' <param name="checkTable"></param>
        ''' <param name="columnName"></param>
        ''' <param name="destinationType"></param>
        ''' <param name="testResultHtml"></param>
        ''' <return>True on exceeding limit, False while below limit</return>
        Private Function AddTestResultRowToTable(testResultsWithErrors As Generic.List(Of DataRow), testResultsWithWarnings As Generic.List(Of DataRow), isError As Boolean, checkTable As DataTable, columnName As String, destinationType As Type, testResultHtml As String) As Boolean
            Const RecordLimitPerColumn As Integer = 5
            If testResultsWithErrors.Count + testResultsWithWarnings.Count < RecordLimitPerColumn Then
                'Add provided data
                Dim MyRow As DataRow = checkTable.NewRow
                MyRow(0) = columnName
                MyRow(1) = destinationType.Name
                MyRow(2) = testResultHtml
                checkTable.Rows.Add(MyRow)
                If isError Then
                    testResultsWithErrors.Add(MyRow)
                Else
                    testResultsWithWarnings.Add(MyRow)
                End If
                Return False
            ElseIf testResultsWithErrors.Count + testResultsWithWarnings.Count = RecordLimitPerColumn Then
                'Number of test result records exceeds limit per column - add a warning that test records won't be shown any more
                Dim MyRow As DataRow = checkTable.NewRow
                MyRow(0) = columnName
                MyRow(1) = destinationType.Name
                If isError Then
                    MyRow(2) = "<font color=""red""><em>Additional errors found - number of errors exceeds limit of " & RecordLimitPerColumn & " warnings per column</em></font>"
                Else
                    MyRow(2) = "<font color=""orange""><em>Additional errors found - number of errors exceeds limit of " & RecordLimitPerColumn & " warnings per column</em></font>"
                End If
                checkTable.Rows.Add(MyRow)
                If isError Then
                    testResultsWithErrors.Add(MyRow)
                Else
                    testResultsWithWarnings.Add(MyRow)
                End If
                Return True
            Else 'If testresults.Count > RecordLimitPerColumn Then
                'don't add records any more!
                Return True
            End If
        End Function
        ''' <summary>
        ''' Add an additional notification result record to the checkTable table
        ''' </summary>
        Private Sub AddTestResultRowToTable(checkTable As DataTable, columnName As String, noteHtmlData As String)
            Dim MyRow As DataRow = checkTable.NewRow
            MyRow(0) = columnName
            MyRow(1) = "REQUIREMENT"
            MyRow(2) = noteHtmlData
            checkTable.Rows.Add(MyRow)
        End Sub
        ''' <summary>
        '''     Verify the existance and the datatype of a column with its requirements
        ''' </summary>
        ''' <param name="checkTable">Table with columns "Required column", "Data type", "Errors" containing check results</param>
        ''' <param name="importTable">Import data</param>
        ''' <param name="columnName">Column name</param>
        ''' <param name="destinationType">Data type of column</param>
        ''' <param name="requiredColumnForUpdate"></param>
        ''' <param name="requiredColumnForInsert"></param>
        ''' <param name="requiredColumnForRemove"></param>
        ''' <param name="allowDBNull">True if DbNull values are allowed, False if a value must be available</param>
        ''' <param name="allowEmptyString">True if empty string values are allowed, False if the string must contain at least 1 character</param>
        ''' <param name="forbiddenChars">An optional list of forbidden chars</param>
        ''' <param name="warningChars">An optional list of chars causing warnings</param>
        ''' <returns>True on validation success, False on errors</returns>
        Private Function PrepareStep4ValidateImportColumn(ByVal checkTable As DataTable, ByVal importTable As DataTable, ByVal culture As System.Globalization.CultureInfo, ByVal columnName As String, ByVal destinationType As Type, ByVal requiredColumnForUpdate As Boolean, ByVal requiredColumnForInsert As Boolean, ByVal requiredColumnForRemove As Boolean, ByVal allowDBNull As Boolean, ByVal allowEmptyString As Boolean, Optional forbiddenChars As Char() = Nothing, Optional warningChars As Char() = Nothing) As Boolean
            Dim TestResultsWithErrors As New Generic.List(Of DataRow)
            Dim TestResultsWithWarnings As New Generic.List(Of DataRow)
            Dim columnIndex As Integer
            If allowDBNull = True AndAlso allowEmptyString = False Then
                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">AllowDbNull/Empty mismatch</font>")
            End If

            'Check for existance of the column
            If importTable.Columns.Contains(columnName) = False OrElse importTable.Columns(columnName).Ordinal = -1 Then
                If Me.ImportAction = ImportBase.ImportActions.InsertOnly And requiredColumnForInsert = True Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Column must exist for selected action type</font>")
                ElseIf Me.ImportAction = ImportBase.ImportActions.UpdateOnly And requiredColumnForUpdate = True Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Column must exist for selected action type</font>")
                ElseIf Me.ImportAction = ImportBase.ImportActions.Remove And requiredColumnForRemove = True Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Column must exist for selected action type</font>")
                ElseIf Me.ImportAction = ImportBase.ImportActions.InsertOrUpdate And (requiredColumnForInsert = True Or requiredColumnForUpdate = True) Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Column must exist for selected action type</font>")
                ElseIf columnName.ToLowerInvariant = "user_password" AndAlso Me.CheckboxStep3SuppressAllNotificationMails.Checked = False Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""" & System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.DarkOliveGreen) & """>Column doesn't exist; password will be generated dynamically and user will get a notification e-mail</font>")
                    Return True
                ElseIf columnName.ToLowerInvariant = "user_password" AndAlso Me.CheckboxStep3SuppressAllNotificationMails.Checked = True Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""" & System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.DarkOrange) & """>Column doesn't exist; new accounts can't be created</font>")
                    Return True
                Else
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""" & System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.DarkOliveGreen) & """>Column doesn't exist; no data will be imported/updated for this field</font>")
                    Return True
                End If
                Return False
            End If
            columnIndex = importTable.Columns(columnName).Ordinal

            'Check datatype and content row by row
            For MyCounter As Integer = 0 To importTable.Rows.Count - 1
                Dim cellValue As Object = importTable.Rows(MyCounter)(columnIndex)
                If allowDBNull = False AndAlso IsDBNull(cellValue) Then
                    'allowDBNull
                    If AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Column contains an invalid NULL (DBNull) value</font>") Then
                        Return False
                    End If
                ElseIf allowEmptyString = False AndAlso (cellValue Is Nothing OrElse Utils.Nz(cellValue, "") = "") Then
                    'allowEmptyString
                    If AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Column contains an invalid empty value</font>") Then
                        Return False
                    End If
                End If
                'Data type
                If Not IsDBNull(cellValue) Then
                    'Make empty string convertable by redefining it as Null/Nothing
                    If Utils.Nz(cellValue, "") = "" Then cellValue = Nothing
                    If destinationType Is GetType(Long) Then
                        Try
                            Long.Parse(CType(cellValue, String), culture)
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Integer) Then
                        Try
                            Integer.Parse(CType(cellValue, String), culture)
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Integer()) Then
                        Try
                            Dim values As String() = CType(cellValue, String).Split(New Char() {","c})
                            For MyConversionTestCounter As Integer = 0 To values.Length - 1
                                Integer.Parse(values(MyConversionTestCounter), culture)
                            Next
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Byte) Then
                        Try
                            Byte.Parse(CType(cellValue, String), culture)
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Decimal) Then
                        Try
                            Decimal.Parse(CType(cellValue, String), culture)
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Double) Then
                        Try
                            Double.Parse(CType(cellValue, String), culture)
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Date) Then
                        Try
                            Date.Parse(CType(cellValue, String), culture)
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(Boolean) Then
                        Try
                            Boolean.Parse(CType(cellValue, String))
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(WMSystem.Sex) Then
                        Try
                            WMSystem.Sex.Parse(GetType(WMSystem.Sex), CType(cellValue, String))
                        Catch ex As Exception
                            Dim ColumnWarningsLimitExceeded As Boolean = False
                            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>")
                            Else
                                AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Conversion error: " & Server.HtmlEncode(ex.Message) & "</font>")
                            End If
                            If ColumnWarningsLimitExceeded Then Return False
                        End Try
                    ElseIf destinationType Is GetType(String) Then
                        'Is already a string
                        If Not forbiddenChars Is Nothing Then
                            For Each Ch As Char In forbiddenChars
                                If Utils.Nz(cellValue, "").Contains(Ch) Then
                                    If AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Invalid char found: """ & Server.HtmlEncode(Ch) & """ (char no. " & AscW(Ch) & ")</font>") Then
                                        Return False
                                    End If
                                End If
                            Next
                        End If
                        If Not warningChars Is Nothing Then
                            For Each Ch As Char In warningChars
                                If Utils.Nz(cellValue, "").Contains(Ch) Then
                                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, False, checkTable, columnName, destinationType, "<font color=""orange"">Row " & MyCounter + 1 & ": special char found: """ & Server.HtmlEncode(Ch) & """ (char no. " & AscW(Ch) & ")</font>")
                                End If
                            Next
                        End If
                        If allowEmptyString = False AndAlso CType(cellValue, String) = Nothing Then
                            If AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Column contains an invalid empty value</font>") Then
                                Return False
                            End If
                        End If
                    Else
                        AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""red"">Row " & MyCounter + 1 & ": Column shall be of type " & destinationType.Name & ", but this type hasn't been implemented yet</font>")
                        Return False
                    End If
                End If
            Next

            'Return success
            If TestResultsWithErrors.Count = 0 Then
                'at least we've to add a no-error-record to the resulting checkTable
                If columnName.ToLowerInvariant = "user_password" Then
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "<font color=""orange"">Please note: Update actions will never update a user's password.</font>")
                Else
                    AddTestResultRowToTable(TestResultsWithErrors, TestResultsWithWarnings, True, checkTable, columnName, destinationType, "")
                End If
                Return True
            Else
                Return False
            End If

        End Function

#End Region

#Region "Buttons GoBack"
        Private Sub ButtonStep2PreviousStep_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonStep2PreviousStep.Click
            Me.SwitchStep(1)
        End Sub

        Private Sub ButtonStep3PreviousStep_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonStep3PreviousStep.Click
            Me.SwitchStep(2)
        End Sub

        Private Sub ButtonStep4PreviousStep_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonStep4PreviousStep.Click
            Me.SwitchStep(3)
        End Sub
#End Region

        Private Sub ButtonStep2PreviewData_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonStep2PreviewData.Click
            If PrepareStep3() = True Then
                'Show the grid
                Me.DatagridStep2DataPreview.DataSource = Me.ImportTable
                Me.DatagridStep2DataPreview.DataBind()
            Else
                'Reset to empty grid, again
                Me.DatagridStep2DataPreview.DataSource = Nothing
                Me.DatagridStep2DataPreview.DataBind()
            End If
        End Sub

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Trim(Me.TextboxStep3Culture.Text) = Nothing Then
                Me.TextboxStep3Culture.Text = Me.ImportFileCulture.Name
            End If

            If Me.CurrentStepNumber = 0 Then
                'Viewstate has been lost
                Me.SwitchStep(1)
            End If

            If Me.IsPostBack AndAlso Me.CurrentStepNumber = 4 Then
                'In the second page request (a post back initiated by the iframed processing page), verify if all imports have been done
                Dim waitingRows As DataRow() = Nothing
                Dim ForceFinish As Boolean
                If Not ImportTable Is Nothing Then
                    waitingRows = ImportTable.Select("User_ImportDone = 0")
                    If ImportTable.Rows.Count = 0 Then
                        'No rows to import
                        MessagesLog &= "<font color=""red"">Warning: import table doesn't contain any rows</font>" & vbNewLine
                        ForceFinish = True
                    ElseIf waitingRows Is Nothing OrElse waitingRows.Length = 0 Then
                        'Rows are there, but now finished
                        MessagesLog &= ImportTable.Rows.Count & " rows processed" & vbNewLine
                    Else
                        'Processing still in progress
                    End If
                Else
                    MessagesLog &= "<font color=""red"">Warning: import table not found - has the web application been restarted?</font>" & vbNewLine
                    ForceFinish = True
                End If
                If ForceFinish = True OrElse waitingRows Is Nothing OrElse waitingRows.Length = 0 Then
                    LabelStep5Log.Text = Replace("Executed import action=" & Me.ImportAction.ToString & vbNewLine & Me.MessagesLog, vbNewLine, "<br>")
                    Me.SwitchStep(5)
                End If
            End If

        End Sub

        Private Sub OnRadioStep3Action_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioStep3ActionInsertOnly.CheckedChanged, RadioStep3ActionInsertUpdate.CheckedChanged, RadioStep3ActionRemoveOnly.CheckedChanged, RadioStep3ActionUpdateOnly.CheckedChanged
            Me.PrepareStep4(True)
        End Sub

        Private Sub CheckboxStep3SuppressAllNotificationMails_CheckedChanged(sender As Object, e As EventArgs) Handles CheckboxStep3SuppressAllNotificationMails.CheckedChanged
            Me.PrepareStep4(True)
        End Sub

    End Class

End Namespace