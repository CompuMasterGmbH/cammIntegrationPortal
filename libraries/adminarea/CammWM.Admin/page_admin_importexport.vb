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
    ''' Class	 : camm.WebManager.Pages.Administration.ImportBase
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Import
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	26.08.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ImportBase
        Inherits Page

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Available action types
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	19.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Enum ImportActions
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Insert users which haven't existed yet as well as update users which already exist
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	02.09.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            InsertOrUpdate = 4
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Only insert items which haven't existed
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	02.09.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            InsertOnly = 2
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Only update users which already exist
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	02.09.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            UpdateOnly = 3
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Remove all items specified in the import file
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	02.09.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Remove = 1
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Memberships/authorization shall be set exactly as defined in the import file
            ''' </summary>
            ''' <remarks>
            '''     Items will be inserted or removed as needed to fit the requirements
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	02.09.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            FitExact = 5
        End Enum

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The selected import action
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property ImportAction() As ImportActions
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportAction"), ImportActions)
            End Get
            Set(ByVal Value As ImportActions)
                Session("WebManager.Administration.Import.UserList.ImportAction") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The selected import action for memberships
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property ImportActionMemberships() As ImportActions
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportActionMemberships"), ImportActions)
            End Get
            Set(ByVal Value As ImportActions)
                If Value <> Nothing And Value <> ImportActions.FitExact And Value <> ImportActions.InsertOnly Then
                    Throw New ArgumentException("Invalid value", "ImportActionMemberships")
                End If
                Session("WebManager.Administration.Import.UserList.ImportActionMemberships") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The selected import action for authorizations
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property ImportActionAuthorizations() As ImportActions
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportActionAuthorizations"), ImportActions)
            End Get
            Set(ByVal Value As ImportActions)
                If Value <> Nothing And Value <> ImportActions.FitExact And Value <> ImportActions.InsertOnly Then
                    Throw New ArgumentException("Invalid value", "ImportActionAuthorizations")
                End If
                Session("WebManager.Administration.Import.UserList.ImportActionAuthorizations") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Messages logged by the import process
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property MessagesLog() As String
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.MessagesLog"), String)
            End Get
            Set(ByVal Value As String)
                Session("WebManager.Administration.Import.UserList.MessagesLog") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The import table with the user information
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	30.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property ImportTable() As DataTable
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.DataTable"), DataTable)
            End Get
            Set(ByVal Value As DataTable)
                Session("WebManager.Administration.Import.UserList.DataTable") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The import table with the user information
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	30.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property SuppressNotificationMails() As Boolean
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.SuppressNotificationMails"), Boolean)
            End Get
            Set(ByVal Value As Boolean)
                Session("WebManager.Administration.Import.UserList.SuppressNotificationMails") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The culture of the import file (required to correctly convert all strings back to their origin data type, e. g. date values)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property ImportFileCulture() As System.Globalization.CultureInfo
            Get
                If Session("WebManager.Administration.Import.UserList.SelectedCulture") Is Nothing Then
                    Return System.Globalization.CultureInfo.CurrentCulture
                Else
                    Return CType(Session("WebManager.Administration.Import.UserList.SelectedCulture"), Globalization.CultureInfo)
                End If
            End Get
            Set(ByVal Value As System.Globalization.CultureInfo)
                Session("WebManager.Administration.Import.UserList.SelectedCulture") = Value
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.ImportUsers
    ''' 
    ''' -----------------------------------------------------------------------------
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
    ''' <history>
    ''' 	[AdminSupport]	26.08.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
        Protected WithEvents RadioStep3ActionMembershipsFitExact As RadioButton
        Protected WithEvents RadioStep3ActionMembershipsInsertOnly As RadioButton
        Protected WithEvents RadioStep3ActionAuthorizationsFitExact As RadioButton
        Protected WithEvents RadioStep3ActionAuthorizationsInsertOnly As RadioButton
        Protected WithEvents RadioStep3ActionAdditionalFlagsFitExact As RadioButton
        Protected WithEvents RadioStep3ActionAdditionalFlagsDefinedKeysOnly As RadioButton
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Switch the form to the desired step
        ''' </summary>
        ''' <param name="stepNumber"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	26.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The current step number
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property CurrentStepNumber() As Integer
            Get
                Return Utils.Nz(viewstate("StepNumber"), 0)
            End Get
            Set(ByVal Value As Integer)
                viewstate("StepNumber") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The temporary filename of the location of the uploaded file
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
                Me.ImportTable = CompuMaster.camm.WebManager.Administration.Tools.Data.Csv.ReadDataTableFromCsvFile(ImportFile, True, Me.TextboxStep2Charset.Text, , , , True)
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
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Memberships", GetType(Integer()), False, False, False, True, True)
                Result = Result And PrepareStep4ValidateImportColumn(CheckResult, Me.ImportTable, ImportFileCulture, "User_Authorizations", GetType(Integer()), False, False, False, True, True)
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
                'Show import action radio buttons for memberships or authtorizations when required
                If Me.ImportTable.Columns.Contains("User_Memberships") Then
                    Me.PanelStep3MembershipsImportType.Visible = True
                Else
                    Me.PanelStep3MembershipsImportType.Visible = False
                End If
                If Me.ImportTable.Columns.Contains("User_Authorizations") Then
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
        ''' -----------------------------------------------------------------------------
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
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	29.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.ImportUsersProcessing
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A helper page which processes the import in fact
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	31.08.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ImportUsersProcessing
        Inherits ImportBase

        Protected WithEvents LiteralStep4ProcessingMessageLog As Label

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     How many user accounts shall be imported at once?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Web requests are regulary limited to 30 seconds. So we're not allowed to import all user accounts in one request. We have to split the processing of the whole list into multiple requests if we don't want the request to stop unexpectedly.
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	30.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable ReadOnly Property NumberOfUsersToImportInOneRoundTrip() As Integer
            Get
                Return 5
            End Get
        End Property

        Private Sub PageOnPreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            ExecuteImport()
        End Sub

        Private _TotalRecords As Integer
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The number of total records in the import table
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property TotalRecords() As Integer
            Get
                Return _TotalRecords
            End Get
        End Property

        Private _ProgressState As Integer
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The number of already processed records from the import table
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ProgressState() As Integer
            Get
                Return _ProgressState
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculate the values for the output to the user
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	30.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Manage the import of several user accounts (while this page request)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Import some user details defined by the given datarow of user data
        ''' </summary>
        ''' <param name="userData">A datarow from the import table</param>
        ''' <param name="rowID">An ID to identify the row by the user in case of errors</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
                        MyUser.PhoneNumber = Utils.Nz(userData("User_PhoneNumber"), "")
                    End If
                    If userData.Table.Columns.Contains("User_MobileNumber") Then
                        MyUser.MobileNumber = Utils.Nz(userData("User_MobileNumber"), "")
                    End If
                    If userData.Table.Columns.Contains("User_FaxNumber") Then
                        MyUser.FaxNumber = Utils.Nz(userData("User_FaxNumber"), "")
                    End If
                    If userData.Table.Columns.Contains("User_Position") Then
                        MyUser.Position = Utils.Nz(userData("User_Position"), "")
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
                            If MyUser.ID <> Nothing Then
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
                    Me.MessagesLog &= "<font color=""red"">Row #" & rowID & ": " & Server.HtmlDecode(ex.ToString).Replace(vbNewLine, "<br>") & "</font>" & vbNewLine
                Else
                    Me.MessagesLog &= "<font color=""red"">Row #" & rowID & ": " & Server.HtmlDecode(ex.Message) & "</font>" & vbNewLine
                End If
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Assign the user profile information from the datarow to the user object
        ''' </summary>
        ''' <param name="user">The user information object which shall be updated</param>
        ''' <param name="userData">The import data record</param>
        ''' <param name="culture">The culture of the import data (when a string has to be converted to a datetime, etc.)</param>
        ''' <remarks>
        '''     All profile information will be copied here except loginname and password
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub ApplyUserProfileData(ByRef user As WMSystem.UserInformation, ByVal userData As DataRow, ByVal culture As System.Globalization.CultureInfo)

            'User account data
            If userData.Table.Columns.Contains("User_EMailAddress") Then
                user.EMailAddress = Trim(Utils.Nz(userData("User_EMailAddress"), ""))
            End If
            If userData.Table.Columns.Contains("User_Gender") Then
                If Utils.Nz(userData("User_Gender"), "") = "" Then
                    user.Gender = WMSystem.Sex.Undefined
                Else
                    user.Gender = CType(System.Enum.Parse(GetType(CompuMaster.camm.WebManager.WMSystem.Sex), Utils.Nz(userData("User_Gender"), "")), WMSystem.Sex)
                End If
            End If
            If userData.Table.Columns.Contains("User_AcademicTitle") Then
                user.AcademicTitle = Trim(Utils.Nz(userData("User_AcademicTitle"), ""))
            End If
            If userData.Table.Columns.Contains("User_FirstName") Then
                user.FirstName = Trim(Utils.Nz(userData("User_FirstName"), ""))
            End If
            If userData.Table.Columns.Contains("User_LastName") Then
                user.LastName = Trim(Utils.Nz(userData("User_LastName"), ""))
            End If
            If userData.Table.Columns.Contains("User_NameAddition") Then
                user.NameAddition = Trim(Utils.Nz(userData("User_NameAddition"), ""))
            End If
            If userData.Table.Columns.Contains("User_Company") Then
                user.Company = Trim(Utils.Nz(userData("User_Company"), ""))
            End If
            If userData.Table.Columns.Contains("User_Position") Then
                user.Position = Trim(Utils.Nz(userData("User_Position"), ""))
            End If
            If userData.Table.Columns.Contains("User_Street") Then
                user.Street = Trim(Utils.Nz(userData("User_Street"), ""))
            End If
            If userData.Table.Columns.Contains("User_ZipCode") Then
                user.ZipCode = Trim(Utils.Nz(userData("User_ZipCode"), ""))
            End If
            If userData.Table.Columns.Contains("User_Location") Then
                user.Location = Trim(Utils.Nz(userData("User_Location"), ""))
            End If
            If userData.Table.Columns.Contains("User_State") Then
                user.State = Trim(Utils.Nz(userData("User_State"), ""))
            End If
            If userData.Table.Columns.Contains("User_Country") Then
                user.Country = Trim(Utils.Nz(userData("User_Country"), ""))
            End If
            If userData.Table.Columns.Contains("User_PhoneNumber") Then
                user.PhoneNumber = Trim(Utils.Nz(userData("User_PhoneNumber"), ""))
            End If
            If userData.Table.Columns.Contains("User_MobileNumber") Then
                user.MobileNumber = Trim(Utils.Nz(userData("User_MobileNumber"), ""))
            End If
            If userData.Table.Columns.Contains("User_FaxNumber") Then
                user.FaxNumber = Trim(Utils.Nz(userData("User_FaxNumber"), ""))
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Assign the memberships and authorizations to a user's account
        ''' </summary>
        ''' <param name="user">The user information object which shall be updated</param>
        ''' <param name="userData">The import data record</param>
        ''' <param name="culture">The culture of the import data (when a string has to be converted to a datetime, etc.)</param>
        ''' <param name="importActionMemberships">The type of the import</param>
        ''' <param name="importActionAuthorizations">The type of the import</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	12.09.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub ApplyMembershipsAndAuthorizations(ByVal user As WMSystem.UserInformation, ByVal userData As DataRow, ByVal culture As System.Globalization.CultureInfo, ByVal importActionMemberships As ImportActions, ByVal importActionAuthorizations As ImportActions)

            If Not (importActionMemberships = 0 OrElse importActionMemberships = ImportBase.ImportActions.InsertOnly OrElse importActionMemberships = ImportBase.ImportActions.FitExact) Then
                Throw New ArgumentException("Invalid import action", "importActionMemberships")
            End If
            If Not (importActionAuthorizations = 0 OrElse importActionAuthorizations = ImportBase.ImportActions.InsertOnly OrElse importActionAuthorizations = ImportBase.ImportActions.FitExact) Then
                Throw New ArgumentException("Invalid import action", "importActionAuthorizations")
            End If

            'Memberships
            If userData.Table.Columns.Contains("User_Memberships") And importActionMemberships <> Nothing Then
                'Collect group IDs
                Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(userData("User_Memberships"), ""))
                Dim values As String() = Value.Split(New Char() {","c})
                Dim requiredGroups As New ArrayList
                For MyConversionTestCounter As Integer = 0 To values.Length - 1
                    If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                        requiredGroups.Add(Integer.Parse(values(MyConversionTestCounter), culture))
                    End If
                Next
                'Prepare correct notifications class
                Dim MyNotifications As WebManager.Notifications.INotifications
                If Me.SuppressNotificationMails = True Then
                    MyNotifications = New WebManager.Notifications.NoNotifications(cammWebManager)
                Else
                    MyNotifications = cammWebManager.Notifications()
                End If
                'Remove unwanted memberships
                Dim CurrentMemberships As WMSystem.GroupInformation() = user.Memberships
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
                            user.RemoveMembership(CurrentMemberships(MyCounterIsCurrently).id)
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
                        user.AddMembership(CType(requiredGroups(MyCounterShallBe), Integer), MyNotifications)
                    End If
                Next
            End If
            'Authorizations
            If userData.Table.Columns.Contains("User_Authorizations") And importActionAuthorizations <> Nothing Then
                'Collect security object IDs
                Dim value As String = Utils.StringNotNothingOrEmpty(Utils.Nz(userData("User_Authorizations"), ""))
                Dim values As String() = CType(Value, String).Split(New Char() {","c})
                Dim requiredSecurityObjectsList As New ArrayList
                For MyConversionTestCounter As Integer = 0 To values.Length - 1
                    If Not Trim(values(MyConversionTestCounter)) = Nothing Then
                        requiredSecurityObjectsList.Add(Integer.Parse(values(MyConversionTestCounter), culture))
                    End If
                Next
                Dim RequiredSecurityObjects As Integer() = CType(requiredSecurityObjectsList.ToArray(GetType(Integer)), Integer())

                'Prepare correct notifications class
                Dim MyNotifications As WebManager.Notifications.INotifications
                If Me.SuppressNotificationMails = True Then
                    MyNotifications = New WebManager.Notifications.NoNotifications(cammWebManager)
                Else
                    MyNotifications = cammWebManager.Notifications()
                End If
                'Remove unwanted authorizations
                Dim CurrentAuthorizations As WMSystem.SecurityObjectAuthorizationForUser() = user.Authorizations
                If importActionMemberships = ImportBase.ImportActions.FitExact Then
                    For MyCounterIsCurrently As Integer = 0 To CurrentAuthorizations.Length - 1
                        'Evaluate if wanted authorization
                        Dim ShallBeThere As Boolean = False
                        For MyCounterShallBe As Integer = 0 To RequiredSecurityObjects.Length - 1
                            If RequiredSecurityObjects(MyCounterShallBe) = CurrentAuthorizations(MyCounterIsCurrently).SecurityObjectID Then
                                ShallBeThere = True
                            End If
                        Next
                        'Remove unwanted authorization
                        If ShallBeThere = False Then
                            user.RemoveAuthorization(CurrentAuthorizations(MyCounterIsCurrently).SecurityObjectID, CurrentAuthorizations(MyCounterIsCurrently).ServerGroupID)
                        End If
                    Next
                End If
                'Add missing authorizations
                For MyCounterShallBe As Integer = 0 To RequiredSecurityObjects.Length - 1
                    'Evaluate if missing authorization
                    Dim AlreadyExist As Boolean = False
                    For MyCounterIsCurrently As Integer = 0 To CurrentAuthorizations.Length - 1
                        If RequiredSecurityObjects(MyCounterShallBe) = CurrentAuthorizations(MyCounterIsCurrently).SecurityObjectID Then
                            AlreadyExist = True
                        End If
                    Next
                    'Add missing authorization
                    If AlreadyExist = False Then
                        user.AddAuthorization(RequiredSecurityObjects(MyCounterShallBe), cammWebManager.CurrentServerInfo.ParentServerGroupID, MyNotifications)
                    End If
                Next
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Error handler for unexpected page errors
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     Show the error message on the page output in the IFrame
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	31.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnError(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Error
            Response.Clear()
            Response.Write(Server.GetLastError.ToString.Replace(vbNewLine, "<br>"))
            Server.ClearError()
            Response.End()
        End Sub

    End Class

End Namespace