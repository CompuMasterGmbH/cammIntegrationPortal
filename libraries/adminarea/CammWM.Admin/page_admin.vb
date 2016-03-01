'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

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
    '''     The base page for all administration page of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
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
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
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
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
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

        Protected Friend Function CurrentDbVersion() As Version
            Static MyDBVersion As Version
            If MyDBVersion Is Nothing Then
                MyDBVersion = cammWebManager.System_DBVersion_Ex
            End If
            Return MyDBVersion
        End Function

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
        Protected ltrlUpdateHint As Literal
        Protected ltrlLicenceModel As Literal
        Protected ltrlLicenceType As Literal

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

            SetLastWebCronExecutionDate()

            Try
                LoadLicenceData()
                SetLabelsWithExpirationDates()
                SetLicenceDescription()
                SetLabelsLicenseDetailsAndUpdateHint()
            Catch ex As CompuMaster.camm.WebManager.Log.SystemException
                cammWebManager.Log.Exception(ex, False)
                If Not Me.lblErrMsg Is Nothing Then
                    If ex.InnerException Is Nothing Then
                        Me.lblErrMsg.Text &= "<p>ERROR ON LICENSE QUERY: " & Server.HtmlEncode(ex.Message) & "</p>"
                        'Me.lblErrMsg.Text = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ex.ToString))
                    ElseIf ex.InnerException.Message = "Data layer exception" AndAlso Not ex.InnerException.InnerException Is Nothing Then
                        Me.lblErrMsg.Text &= "<p>ERROR ON LICENSE QUERY: " & Server.HtmlEncode(ex.InnerException.InnerException.Message) & "</p>"
                        'Me.lblErrMsg.Text = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ex.ToString))
                    Else
                        'Me.lblErrMsg.Text &= "<p>ERROR ON LICENSE QUERY: " & Server.HtmlEncode(ex.InnerException.Message) & "</p>"
                        Me.lblErrMsg.Text &= "<p>ERROR ON LICENSE QUERY: " & Server.HtmlEncode(ex.ToString) & "</p>"
                        'Me.lblErrMsg.Text = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ex.ToString))
                    End If
                End If
            Catch ex As Exception
                cammWebManager.Log.Exception(ex, False)
                If Not Me.lblErrMsg Is Nothing Then
                    'Me.lblErrMsg.Text &= "<p>ERROR ON LICENSE QUERY: " & Server.HtmlEncode(ex.Message) & "</p>"
                    Me.lblErrMsg.Text &= "<p>ERROR ON LICENSE QUERY: " & Server.HtmlEncode(ex.ToString) & "</p>"
                    'Me.lblErrMsg.Text = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ex.ToString))
                End If
            End Try

        End Sub

        Private Sub About_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            Dim files As String() = FindFilesToBeRemoved()
            If files.Length > 0 Then
                trSecurity.Style.Add("display", "")
            Else
                trSecurity.Style.Add("display", "none")
            End If
        End Sub

        ''' <summary>
        ''' Load license data from database cache or from CompuMaster license/registration server
        ''' </summary>
        Private Sub LoadLicenceData()
            Dim productRegistration As New CompuMaster.camm.WebManager.Registration.ProductRegistration(Me.cammWebManager)

            Dim forceFetch As String = Request.Item("forceserverrefresh")
            Dim forceFetchFromServer As Boolean = forceFetch <> Nothing AndAlso CType(forceFetch, Boolean)
            If forceFetchFromServer Then
                productRegistration.RefreshValidationDataFromServer(True)
            End If
            instanceValidationData = productRegistration.GetCachedValidationResult()
            If instanceValidationData Is Nothing Then
                Throw New Exception("failed to fetch validation data")
            End If
        End Sub

        Private Sub SetLastWebCronExecutionDate()
            If Not Me.lblLastWebCronExecution Is Nothing Then
                Dim lastServiceExecution As Date = DataLayer.Current.QueryLastServiceExecutionDate(Me.cammWebManager, Nothing)
                If lastServiceExecution = Nothing Then
                    lblLastWebCronExecution.Text = "never"
                Else
                    lblLastWebCronExecution.Text = lastServiceExecution.ToString()
                End If
            End If
        End Sub

        Private Function CreateContractExpirationString(ByVal expirationDate As DateTime) As String
            Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()
            Const expirationAppendix As String = " - <b>expired!</b>"
            Dim result As String
            If expirationDate = Nothing Then
                result = "No contract/subscription ordered"
            Else
                result = expirationDate.ToString()
                If expirationDate < currentDate Then
                    result &= expirationAppendix
                End If
            End If
            Return result
        End Function

        Private Sub SetLabelsWithExpirationDates()
            If Not Me.instanceValidationData Is Nothing Then
                If Not Me.ltrlSupportContractExpirationDate Is Nothing Then Me.ltrlSupportContractExpirationDate.Text = CreateContractExpirationString(Me.instanceValidationData.SupportContractExpirationDate)
                If Not Me.ltrlUpdateContractExpirationDate Is Nothing Then Me.ltrlUpdateContractExpirationDate.Text = CreateContractExpirationString(Me.instanceValidationData.UpdateContractExpirationDate)
                If Not Me.ltrlLicenceExpirationDate Is Nothing Then Me.ltrlLicenceExpirationDate.Text = CreateContractExpirationString(Me.instanceValidationData.LicenceData.ExpirationDate)
            End If
        End Sub

        Private Sub SetLabelsLicenseDetailsAndUpdateHint()
            If Not Me.ltrlUpdateHint Is Nothing Then
                If Me.instanceValidationData Is Nothing Then 'No updates
                    Me.ltrlUpdateHint.Text = "" '"no service info from server available"
                ElseIf Not Me.instanceValidationData Is Nothing AndAlso Now.ToUniversalTime < Me.instanceValidationData.UpdateContractExpirationDate Then
                    If Me.instanceValidationData.SecurityUpdateAvailable = True Then
                        Me.ltrlUpdateHint.Text = "(security update available, please update as soon as possible)"
                    ElseIf Me.instanceValidationData.UpdateAvailable = True
                        Me.ltrlUpdateHint.Text = "(update available)"
                    Else 'current contract, but no updates
                        Me.ltrlUpdateHint.Text = "" '"update subscription active, but no updates available"
                    End If
                Else 'old contract, no updates
                    Me.ltrlUpdateHint.Text = "" '"service info available, but update subscription outdated"
                End If
            End If
            If Not Me.ltrlLicenceModel Is Nothing AndAlso Not Me.instanceValidationData Is Nothing AndAlso Not Me.instanceValidationData.LicenceData Is Nothing Then
                Me.ltrlLicenceModel.Text = [Enum].GetName(Me.instanceValidationData.LicenceData.Model.GetType(), Me.instanceValidationData.LicenceData.Model)
            End If
            If Not Me.ltrlLicenceType Is Nothing AndAlso Not Me.instanceValidationData Is Nothing AndAlso Not Me.instanceValidationData.LicenceData Is Nothing Then
                ltrlLicenceType.Text = [Enum].GetName(Me.instanceValidationData.LicenceData.Type.GetType(), Me.instanceValidationData.LicenceData.Type)
            End If
        End Sub

        Private Sub SetLicenceDescription()
            Dim text As String = ""
            If Not instanceValidationData Is Nothing Then
                Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()
                If CompuMaster.camm.WebManager.Registration.LicenceInfo.IsLicenceModelWithSupport(instanceValidationData.LicenceData.Model) Then
                    If instanceValidationData.UpdateContractExpirationDate = Nothing Then
                        text &= "without updates (contract missing)"
                    ElseIf currentDate < instanceValidationData.UpdateContractExpirationDate Then
                        text &= "with updates"
                    Else
                        text &= "without updates (contract expired)"
                    End If

                    If instanceValidationData.SupportContractExpirationDate = Nothing Then
                        text &= ", without support and maintenance service (contract missing) "
                    ElseIf currentDate < instanceValidationData.SupportContractExpirationDate Then
                        text &= ", with support and maintenance service "
                    Else
                        text &= ", without support and maintenance service (contract expired) "
                    End If
                Else
                    text = "Without updates, without support and maintenance service "
                End If
            End If
            If Not Me.lblLicenceDescription Is Nothing Then
                lblLicenceDescription.Text = text
            End If
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

    End Class

End Namespace