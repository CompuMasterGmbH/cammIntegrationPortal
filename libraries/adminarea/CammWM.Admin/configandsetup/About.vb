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
Imports System.Web.UI.HtmlControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    ''' About page
    ''' </summary>
    Public Class About
        Inherits Page

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


        ''' <summary>
        ''' The last version informaton on the update build which is available
        ''' </summary>
        Public Function AvailableUpdatesUpToBuild() As Version
            Return Setup.DatabaseSetup.LastBuildVersionInSetupFiles
        End Function

        ''' <summary>
        ''' The current build no. of the database
        ''' </summary>
        Public Function CurrentDatabaseBuild() As Version
            Static Result As Version
            If Result Is Nothing Then
                Result = CompuMaster.camm.WebManager.Setup.DatabaseUtils.Version(cammWebManager, False)
            End If
            Return Result
        End Function

        ''' <summary>
        ''' The version number of the camm Web-Manager library with major, minor and build no., but without revision no.
        ''' </summary>
        Public Function CurrentApplicationBuild() As Version
            Dim DllVersion As Version = CurrentApplicationVersion()
            Return New System.Version(DllVersion.Major, DllVersion.Minor, DllVersion.Build)
        End Function

        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkBtn.Click
            DropFilesToBeRemoved()
            lblInstallLinks.Text = ""
        End Sub

        ''' <summary>
        ''' The current version of the camm Web-Manager library
        ''' </summary>
        Public Function CurrentApplicationVersion() As Version
            Static DllVersion As Version
            If DllVersion Is Nothing Then
                DllVersion = cammWebManager.System_Version_Ex
            End If
            Return DllVersion
        End Function

        ''' <summary>
        ''' The current version of the cammWM.Admin library
        ''' </summary>
        Public Function CurrentAdminAreaVersion() As Version
            Static DllVersion As Version
            If DllVersion Is Nothing Then
                DllVersion = New Version(camm.WebManager.Administration.AssemblyVersion.Version)
            End If
            Return DllVersion
        End Function

        ''' <summary> 
        ''' Find obsolete files and directories which should better be removed 
        ''' </summary> 
        ''' <returns>An array of virtual paths found on the local web server</returns> 
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

        ''' <summary> 
        ''' Check the existance of an virtual directory and if true then add it to the resultList 
        ''' </summary> 
        ''' <param name="resultList">The results list</param> 
        ''' <param name="virtualPath">A path to check</param> 
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
                productRegistration.RefreshValidationDataFromRemoteLicenseServer(True)
            End If
            Me.instanceValidationData = productRegistration.GetCachedValidationResult()
            If Me.instanceValidationData Is Nothing Then
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
                    ElseIf Me.instanceValidationData.UpdateAvailable = True Then
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

        ''' <summary> 
        ''' Delete the unwanted files and directories 
        ''' </summary> 
        ''' <returns>A NameValueCollection containing all occured errors where Key is the filename and Value is the exception message</returns>
        Public Function DropFilesToBeRemoved() As Collections.Specialized.NameValueCollection
            Dim files As String() = FindFilesToBeRemoved()
            Dim errors As New Collections.Specialized.NameValueCollection
            For MyCounter As Integer = 0 To files.Length - 1
                Try
                    Dim MyDBVersion As Version = cammWebManager.System_DBVersion_Ex
                    ' If MyDBVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then
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