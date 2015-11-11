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
    ''' Class	 : camm.WebManager.Pages.Administration.Page
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     The base page for all administration page of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	06.05.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class Page
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Private _FloatingMenu As CompuMaster.camm.WebManager.Controls.Administration.FloatingMenu
        Public Property cammWebManagerAdminMenu() As CompuMaster.camm.WebManager.Controls.Administration.FloatingMenu
            Get
                Return _FloatingMenu
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.Controls.Administration.FloatingMenu)
                _FloatingMenu = Value
            End Set
        End Property

        Private _CurrentAdminIsSecurityOperator As TriState = TriState.UseDefault
        ''' <summary>
        ''' Is the current user a security operator?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Result is cached</remarks>
        Public Function CurrentAdminIsSecurityOperator() As Boolean
            If _CurrentAdminIsSecurityOperator = TriState.UseDefault Then
                If cammWebManager.System_IsSecurityOperator(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                    _CurrentAdminIsSecurityOperator = TriState.True
                Else
                    _CurrentAdminIsSecurityOperator = TriState.False
                End If
            End If
            If _CurrentAdminIsSecurityOperator = TriState.True Then
                Return True
            Else
                Return False
            End If
        End Function

        Private _CurrentAdminIsSupervisor As TriState = TriState.UseDefault
        ''' <summary>
        ''' Is the current user a supervisor?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Result is cached</remarks>
        Public Function CurrentAdminIsSupervisor() As Boolean
            If _CurrentAdminIsSupervisor = TriState.UseDefault Then
                If cammWebManager.System_IsSuperVisor(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                    _CurrentAdminIsSupervisor = TriState.True
                Else
                    _CurrentAdminIsSupervisor = TriState.False
                End If
            End If
            If _CurrentAdminIsSupervisor = TriState.True Then
                Return True
            Else
                Return False
            End If
        End Function

        Private _CurrentAdminIsSecurityMasterApplications As TriState = TriState.UseDefault
        Private _CurrentAdminIsSecurityMasterGroups As TriState = TriState.UseDefault
        Public Enum AdministrationItemType As Byte
            Groups = 1
            Applications = 2
        End Enum
        ''' <summary>
        ''' Is the current user a SecurityMaster?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Result is cached</remarks>
        Public Function CurrentAdminIsSecurityMaster(securityMasterType As AdministrationItemType) As Boolean
            Select Case securityMasterType
                Case AdministrationItemType.Applications
                    If _CurrentAdminIsSecurityMasterApplications = TriState.UseDefault Then
                        If cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                            _CurrentAdminIsSecurityMasterApplications = TriState.True
                        Else
                            _CurrentAdminIsSecurityMasterApplications = TriState.False
                        End If
                    End If
                    If _CurrentAdminIsSecurityMasterApplications = TriState.True Then
                        Return True
                    Else
                        Return False
                    End If
                Case AdministrationItemType.Groups
                    If _CurrentAdminIsSecurityMasterGroups = TriState.UseDefault Then
                        If cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                            _CurrentAdminIsSecurityMasterGroups = TriState.True
                        Else
                            _CurrentAdminIsSecurityMasterGroups = TriState.False
                        End If
                    End If
                    If _CurrentAdminIsSecurityMasterGroups = TriState.True Then
                        Return True
                    Else
                        Return False
                    End If
                Case Else
                    Throw New ArgumentException("Invalid value", "securityMasterType")
            End Select
        End Function

        Public Enum AuthorizationTypeItemIndependent As Byte
            SecurityMaster = 1
            [New] = 2
            ViewAllItems = 3
            ViewAllRelations = 4
        End Enum

        Public Enum AuthorizationTypeItemDependent As Byte
            View = 1
            Update = 2
            ViewRelations = 3
            UpdateRelations = 4
            ViewLogs = 5
            PrimaryContact = 6
            ResponsibleContact = 7
            Owner = 8
        End Enum

        Public Enum AuthorizationTypeEffective As Byte
            View = 1
            Update = 2
            ViewRelations = 3
            UpdateRelations = 4
            ViewLogs = 5
            PrimaryContact = 6
            ResponsibleContact = 7
            Owner = 8
        End Enum

        Private _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective As New System.Collections.Specialized.NameValueCollection
        ''' <summary>
        ''' Is the current admin priviledged for administration of this item?
        ''' </summary>
        ''' <param name="itemType">Applications or Groups</param>
        ''' <param name="itemID">The primary ID of the item in applications/groups table</param>
        ''' <param name="authorizationType">View, UpdateRelations, etc. in their effective meaning (is the user authorized by this single item or by an item-independent setting?)</param>
        ''' <returns>Supervisors and security masters are always granted, all others have to be checked in more details</returns>
        ''' <remarks></remarks>
        Public Function CurrentAdminIsPrivilegedForItemAdministration(itemType As AdministrationItemType, authorizationType As AuthorizationTypeEffective, itemID As Integer) As Boolean
            If Me.CurrentAdminIsSupervisor OrElse Me.CurrentAdminIsSecurityMaster(itemType) Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "1" Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "0" Then
                Return False
            Else
                'Check for general (item independent) privileges
                Select Case authorizationType
                    Case AuthorizationTypeEffective.Update, AuthorizationTypeEffective.UpdateRelations
                        'security masters and supervisors could have general priviledges for that authorization type - but is already checked in code before
                    Case AuthorizationTypeEffective.View
                        If CurrentAdminIsPrivilegedForItemAdministration(itemType, AuthorizationTypeItemIndependent.ViewAllItems) Then Return True
                    Case AuthorizationTypeEffective.ViewRelations
                        If CurrentAdminIsPrivilegedForItemAdministration(itemType, AuthorizationTypeItemIndependent.ViewAllRelations) Then Return True
                End Select
                'Query the database
                Dim MyCmd As New SqlCommand
                With MyCmd
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID FROM [dbo].[System_SubSecurityAdjustments] WHERE TableName = @TableName AND AuthorizationType = @AuthorizationType AND UserID = @UserID AND TablePrimaryIDValue = @TablePrimaryIDValue"
                    .CommandType = CommandType.Text
                    .Connection = New SqlConnection(cammWebManager.ConnectionString)
                    .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AdministrationItemType), itemType)
                    .Parameters.Add("@TablePrimaryIDValue", SqlDbType.Int).Value = itemID
                    .Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AuthorizationTypeEffective), authorizationType)
                    .Parameters.Add("@UserID", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                End With
                Dim DBResult As Integer = Utils.Nz(CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection), 0)
                If DBResult <> 0 Then
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "1"
                    Return True
                Else
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "0"
                    Return False
                End If
            End If
        End Function

        Private _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent As New System.Collections.Specialized.NameValueCollection
        ''' <summary>
        ''' Is the current admin priviledged for administration of this item in its effective meaning?
        ''' </summary>
        ''' <param name="itemType">Applications or Groups</param>
        ''' <param name="authorizationType">SecurityMaster, ViewAllRelations, New, etc.</param>
        ''' <returns>Supervisors and security masters are always granted, all others have to be checked in more details</returns>
        ''' <remarks></remarks>
        Public Function CurrentAdminIsPrivilegedForItemAdministration(itemType As AdministrationItemType, authorizationType As AuthorizationTypeItemIndependent) As Boolean
            If Me.CurrentAdminIsSupervisor OrElse Me.CurrentAdminIsSecurityMaster(itemType) Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "1" Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "0" Then
                Return False
            Else
                'Query the database
                Dim MyCmd As New SqlCommand
                With MyCmd
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID FROM [dbo].[System_SubSecurityAdjustments] WHERE TableName = @TableName AND AuthorizationType = @AuthorizationType AND UserID = @UserID AND TablePrimaryIDValue = 0"
                    .CommandType = CommandType.Text
                    .Connection = New SqlConnection(cammWebManager.ConnectionString)
                    .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AdministrationItemType), itemType)
                    .Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AuthorizationTypeItemIndependent), authorizationType)
                    .Parameters.Add("@UserID", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                End With
                Dim DBResult As Integer = Utils.Nz(CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection), 0)
                If DBResult <> 0 Then
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "1"
                    Return True
                Else
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "0"
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' Close the sql connection and the sql command safely
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks></remarks>
        Protected Friend Sub CloseAndDisposeDbConnectionAndDbCommand(command As SqlClient.SqlCommand)
            If Not command Is Nothing Then
                CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(command.Connection)
                command.Dispose()
            End If
        End Sub

        ''' <summary>
        ''' Close the sql connection and the sql command safely
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        Protected Friend Sub CloseAndDisposeDbConnection(connection As SqlClient.SqlConnection)
            CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(connection)
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.About
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	06.05.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class About
        Inherits Page

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The last version informaton on the update build which is available
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected FileName As String
        Protected lblInstallLinks As Label
        Protected WithEvents lnkBtn As LinkButton
        Protected lblErrMsg As Label
        Protected trSecurity As HtmlTableRow
        Protected hrefDbUpdate As HtmlAnchor
        Protected brDbUpdate As Literal
        Protected lblLastWebCronExecution As Label
        Protected lblWebCronServer As Label
        Protected lblLicenceDescription As Literal

        Protected ltrlUpdateContractExpirationDate As Literal
        Protected ltrlSupportContractExpirationDate As Literal
        Protected ltrlLicenceExpirationDate As Literal

        Protected instanceValidationData As Registration.InstanceValidationResult


        Public Function AvailableUpdatesUpToBuild() As Version
            Return Setup.DatabaseSetup.LastBuildVersionInSetupFiles
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The current build no. of the database
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CurrentDatabaseBuild() As Version
            Static Result As Version
            If Result Is Nothing Then
                Result = CompuMaster.camm.WebManager.Setup.DatabaseUtils.Version(cammWebManager, False)
            End If
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The version number of the camm Web-Manager library with major, minor and build no., but without revision no.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CurrentApplicationBuild() As Version
            Dim DllVersion As Version = CurrentApplicationVersion()
            Return New System.Version(DllVersion.Major, DllVersion.Minor, DllVersion.Build)
        End Function

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkBtn.Click
            DropFilesToBeRemoved()
            lblInstallLinks.Text = ""
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The current version of the camm Web-Manager library
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CurrentApplicationVersion() As Version
            Static DllVersion As Version
            If DllVersion Is Nothing Then
                DllVersion = cammWebManager.System_Version_Ex
            End If
            Return DllVersion
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The current version of the cammWM.Admin library
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	04.09.2009	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CurrentAdminAreaVersion() As Version
            Static DllVersion As Version
            If DllVersion Is Nothing Then
                DllVersion = New Version(camm.WebManager.Administration.AssemblyVersion.Version)
            End If
            Return DllVersion
        End Function

        ''' ----------------------------------------------------------------------------- 
        ''' <summary> 
        ''' Find obsolete files and directories which should better be removed 
        ''' </summary> 
        ''' <returns>An array of virtual paths found on the local web server</returns> 
        ''' <remarks> 
        ''' </remarks> 
        ''' <history> 
        '''         [wezel] 30.10.2007      Created 
        ''' </history> 
        ''' ----------------------------------------------------------------------------- 
        Public Function FindFilesToBeRemoved() As String()
            Dim Result As New ArrayList
            FileName = ""
            FindFilesToBeRemoved_CheckVirtualDirectory(Result, "/install/")
            FindFilesToBeRemoved_CheckVirtualDirectory(Result, "/setup_webdb/")
            'FindFilesToBeRemoved_CheckVirtualDirectory(Result, "/system/admin/install/")
            FindFilesToBeRemoved_CheckVirtualFiles(Result, "/install.aspx")
            FindFilesToBeRemoved_CheckVirtualFiles(Result, "/system/admin/install/install.aspx")
            'FindFilesToBeRemoved_CheckVirtualFiles(Result, "/install/install.aspx")
            lblInstallLinks.Text = FileName
            Return CType(Result.ToArray(GetType(String)), String())
        End Function

        ''' ----------------------------------------------------------------------------- 
        ''' <summary> 
        ''' Check the existance of an virtual directory and if true then add it to the resultList 
        ''' </summary> 
        ''' <param name="resultList">The results list</param> 
        ''' <param name="virtualPath">A path to check</param> 
        ''' <remarks> 
        ''' </remarks> 
        ''' <history> 
        '''         [wezel] 30.10.2007      Created 
        ''' </history> 
        ''' ----------------------------------------------------------------------------- 
        Public Sub FindFilesToBeRemoved_CheckVirtualDirectory(ByVal resultList As ArrayList, ByVal virtualPath As String)
            Try
                If System.IO.Directory.Exists(Server.MapPath(virtualPath)) Then
                    resultList.Add(virtualPath)
                    FileName += virtualPath + "<br>"
                End If
            Catch
                'Ignore 
            End Try
        End Sub
        Public Sub FindFilesToBeRemoved_CheckVirtualFiles(ByVal resultList As ArrayList, ByVal virtualPath As String)
            Try
                If System.IO.File.Exists(Server.MapPath(virtualPath)) Then
                    resultList.Add(virtualPath)
                    FileName += virtualPath + "<br>"
                End If
            Catch
                'Ignore 
            End Try
        End Sub

        Private Sub About_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""
            'If Not IsPostBack Then
            If CurrentDatabaseBuild.CompareTo(AvailableUpdatesUpToBuild) < 0 AndAlso System.IO.File.Exists(Server.MapPath("/system/admin/install/update.aspx")) Then
                'database version is older than assembly version, patches for the database might be available
                hrefDbUpdate.Visible = True
                brDbUpdate.Visible = True
            End If

            LoadLicenceData()
            SetLastWebCronExecutionDate()
            SetExpirationDates()
            SetLicenceDescription()

            'End If
        End Sub

        Private Sub About_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            Dim files As String() = FindFilesToBeRemoved()
            If files.Length > 0 Then
                trSecurity.Style.Add("display", "")
            Else
                trSecurity.Style.Add("display", "none")
            End If
        End Sub

        Private Sub LoadLicenceData()
            Dim productRegistration As New CompuMaster.camm.WebManager.Registration.ProductRegistration(Me.cammWebManager)

            Dim forceFetch As String = Request.Item("forceserverrefresh")
            Dim fetchFromServer As Boolean = forceFetch <> Nothing AndAlso CType(forceFetch, Boolean)
            If fetchFromServer Then
                productRegistration.RefreshValidationDataFromServer()
            End If
            instanceValidationData = productRegistration.GetCachedValidationResult()
            If instanceValidationData Is Nothing Then
                Throw New Exception("failed to fetch validation data")
            End If
        End Sub

        Private Sub SetLastWebCronExecutionDate()
            Dim lastServiceExecution As Date = GetLastServiceExecutionDate()
            If lastServiceExecution = Nothing Then
                lblLastWebCronExecution.Text = "never"
            Else
                lblLastWebCronExecution.Text = lastServiceExecution.ToString()
            End If
        End Sub

        Private Function CreateContractExpirationString(ByVal expirationDate As DateTime) As String
            Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()
            Const expirationAppendix As String = " - <b>expired!</b>"
            Dim result As String = expirationDate.ToString()
            If expirationDate < currentDate Then
                result &= expirationAppendix
            End If
            Return result
        End Function

        Private Sub SetExpirationDates()
            If Not Me.instanceValidationData Is Nothing Then
                ltrlSupportContractExpirationDate.Text = CreateContractExpirationString(Me.instanceValidationData.SupportContractExpirationDate)
                ltrlUpdateContractExpirationDate.Text = CreateContractExpirationString(Me.instanceValidationData.UpdateContractExpirationDate)
                ltrlLicenceExpirationDate.Text = CreateContractExpirationString(Me.instanceValidationData.LicenceData.ExpirationDate)

            End If
        End Sub
        Private Sub SetLicenceDescription()
            Dim text As String = ""

            If Not instanceValidationData Is Nothing Then

                Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()

                If CompuMaster.camm.WebManager.Registration.LicenceInfo.IsLicenceModelWithSupport(instanceValidationData.LicenceData.Model) Then
                    If currentDate < instanceValidationData.UpdateContractExpirationDate Then
                        text &= "with updates"
                    Else
                        text &= "without updates (contract expired)"
                    End If

                    If currentDate < instanceValidationData.SupportContractExpirationDate Then
                        text &= ", with support and maintenance service "
                    Else
                        text &= ", without support and maintenance service (contract expired) "
                    End If
                Else
                    text = "Without updates, without support and maintenance service "
                End If
            End If
            lblLicenceDescription.Text = text
        End Sub

        ''' ----------------------------------------------------------------------------- 
        ''' <summary> 
        ''' Delete the unwanted files and directories 
        ''' </summary> 
        ''' <returns>A NameValueCollection containing all occured errors where Key is the filename and Value is the exception message</returns>
        ''' <remarks> 
        ''' </remarks> 
        ''' <history> 
        '''         [wezel] 30.10.2007      Created 
        ''' </history> 
        ''' ----------------------------------------------------------------------------- 
        Public Function DropFilesToBeRemoved() As Collections.Specialized.NameValueCollection
            Dim files As String() = FindFilesToBeRemoved()
            Dim errors As New Collections.Specialized.NameValueCollection
            For MyCounter As Integer = 0 To files.Length - 1
                Try

                    Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex
                    ' If MyDBVersion.Build >= 147 Then
                    If (files(MyCounter).Trim().EndsWith("/")) Then
                        System.IO.Directory.Delete(Server.MapPath(files(MyCounter)), True)

                    Else
                        System.IO.File.Delete(Server.MapPath(files(MyCounter)))
                    End If
                    '  Else
                    '    lblErrMsg.Text = "Your build version is less than 147. So, temporary files can not be deleted."
                    '  End If
                Catch ex As Exception
                    errors(files(MyCounter)) = ex.Message
                    lblErrMsg.Text = ex.Message
                    Exit For
                End Try
            Next
            files = FindFilesToBeRemoved()
            If files.Length > 0 Then
                trSecurity.Style.Add("display", "")
            Else
                trSecurity.Style.Add("display", "none")
            End If
            FileName = ""
            lblInstallLinks.Text = ""
            Return errors
        End Function

        Public Function LicenseFile() As String
            Dim LicenceFileName As String
            Select Case cammWebManager.UI.LanguageID
                Case 2
                    LicenceFileName = "licence-de.rtf"
                Case Else
                    LicenceFileName = "licence.rtf"
            End Select
            Dim FileUrl As String = cammWebManager.DownloadHandler.CreatePlainDownloadLink(DownloadHandler.DownloadLocations.PublicCache, "camm/webmanager", LicenceFileName)
            If System.IO.File.Exists(Server.MapPath(FileUrl)) = True Then
                Return FileUrl
            Else
                Dim LicenseRtf As Byte()
                LicenseRtf = CompuMaster.camm.WebManager.Setup.DatabaseSetup.ResourceDataDatabaseSetup(LicenceFileName)
                If LicenseRtf Is Nothing Then
                    Throw New Exception("Missing resource for " & LicenceFileName)
                End If
                Dim rawData As New CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile
                rawData.Data = LicenseRtf
                rawData.Filename = LicenceFileName
                rawData.MimeType = CompuMaster.camm.WebManager.MimeTypes.MimeTypeByFileName(rawData.Filename)
                cammWebManager.DownloadHandler.Clear()
                cammWebManager.DownloadHandler.Add(rawData, "webmanager")
                Return cammWebManager.DownloadHandler.ProcessAndGetPlainDownloadLink(DownloadHandler.DownloadLocations.PublicCache, "camm")
            End If
        End Function

        Private Function GetLastServiceExecutionDate() As Date
            Dim connection As System.Data.SqlClient.SqlConnection = Nothing
            Dim cmd As System.Data.SqlClient.SqlCommand = Nothing
            Try
                connection = New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
                connection.Open()
                cmd = New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ValueDateTime FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LastWebServiceExecutionDate'", connection)

                Dim result As Object = cmd.ExecuteScalar()
                If result Is Nothing OrElse CompuMaster.camm.WebManager.Utils.Nz(result) Is Nothing Then
                    Return Nothing
                End If
                Return CType(cmd.ExecuteScalar(), Date)
            Finally
                If Not connection Is Nothing Then
                    If connection.State <> ConnectionState.Closed Then
                        connection.Close()
                        connection.Dispose()
                    End If
                End If

                If Not cmd Is Nothing Then
                    cmd.Dispose()
                End If
            End Try
        End Function



    End Class

End Namespace