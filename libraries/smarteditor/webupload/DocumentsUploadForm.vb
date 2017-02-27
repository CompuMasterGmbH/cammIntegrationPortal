'Copyright 2005,2007,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web
Imports CompuMaster.camm.WebManager

Namespace CompuMaster.camm.SmartWebEditor.Pages

    ''' <summary>
    ''' Image upload form (page is often centrally positioned in /system/... folder)
    ''' </summary>
    Public Class DocumentsUploadForm
        Inherits CompuMaster.camm.SmartWebEditor.Pages.Upload.GeneralUploadForm

        Private Configuration As New ConfigurationUploadSettings

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            'Initializes the object security
            InitializeSecurityObject()
            cammWebManager.AuthorizeDocumentAccess(cammWebManager.SecurityObject)

            'Ensure that upload folders are available - if possible and write permission is given
            CreateUploadFolders()

            'Initialize the label who are describing the upload locations targets
            Me.LabelFileUploadFolderValue.Text = Me.UploadParamters.UploadPath

        End Sub

        Protected CheckBoxImageReduction As System.Web.UI.WebControls.CheckBox
        Protected LabelUploadedImageNames As System.Web.UI.WebControls.Label

        Protected NoImageChosenJavascriptMessageText As String
        Protected OnlyFollowingExtensionsAreAllowed As String

        Private Sub InitializeControls()
            Dim fileBrowser As FileBrowser = CType(Me.Page.FindControl("FileBrowserControl"), FileBrowser)
            If Not fileBrowser Is Nothing Then
                fileBrowser.UploadFolderPath = Me.UploadParamters.UploadPath
                fileBrowser.ReadonlyDirectories = Me.UploadParamters.ReadOnlyDirectories
                fileBrowser.ParentWindowCallbackFunction = Me.UploadParamters.JavaScriptCallBackFunctionName
                fileBrowser.EditorId = Me.UploadParamters.EditorClientId
                fileBrowser.CloseWindowAfterInsertion = True
                fileBrowser.UILanguage = Me.cammWebManager.UI.LanguageID
            End If
        End Sub

        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Initialize text
            Me.InitializeText()

            'Reset warnings
            Me.LabelWarning.Text = Nothing

            Me.InitializeControls()

        End Sub

#Region "Initialize "

        ''' <summary>
        '''     Initialize form text
        ''' </summary>
        Protected Overrides Sub InitializeText()

            'Localizations
            Me.LabelSelectFileToUpload.Text = My.Resources.Label_SelectDocFileToUpload
            Me.ButtonUploadFile.Text = My.Resources.Label_UploadFile
            Me.LabelProcessingTips.Text = String.Format(My.Resources.Label_DocProcessingTips, String.Join(", ", Me.UploadParamters.AllowedFileExtensions))
            Me.LabelFileUploadFolder.Text = My.Resources.Label_FileUploadFolder
            Me.NoImageChosenJavascriptMessageText = My.Resources.Label_NoImageChosenJavascriptMessageText
            Me.OnlyFollowingExtensionsAreAllowed = My.Resources.Label_OnlyFollowingExtensionsAreAllowed
            Me.ltrlInsertSectionHeadline.Text = My.Resources.Label_InsertDocument

        End Sub

#End Region


#Region "Events and functions"

        ''' <summary>
        '''     Handles the upload buttons click event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub ButtonUploadFile_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonUploadFile.Click
            'Reinitialize the upload folder for 
            If Not Me.InputFileUpload.PostedFile.ContentLength > 0 AndAlso Me.InputFileUpload.PostedFile.FileName = "" Then
                Exit Sub
            End If

            Dim ext As String = System.IO.Path.GetExtension(Me.InputFileUpload.PostedFile.FileName).ToLower
            If Not Me.IsAllowedExtension(ext) Then
                Me.LabelWarning.Text = My.Resources.Label_InvalidFileExtension
                Exit Sub
            End If

            If Me.UploadParamters.MaxUploadSize > 0 Then
                Dim fileSize As Integer = Me.InputFileUpload.PostedFile.ContentLength
                If fileSize > Me.UploadParamters.MaxUploadSize Then
                    LabelWarning.Text = My.Resources.Label_UploadedFileTooLarge
                    Exit Sub
                End If
            End If

            Dim fs As System.IO.Stream = Me.InputFileUpload.PostedFile.InputStream
            Dim data(CType(fs.Length, Integer)) As Byte
            fs.Read(data, 0, CType(fs.Length, Integer))
            fs.Close()

            Dim files As New ArrayList


            Dim fileName As String = System.IO.Path.GetFileNameWithoutExtension(Me.InputFileUpload.PostedFile.FileName) & "_" & Now.ToString("yyyyMMddHHmmss") & System.IO.Path.GetExtension(Me.InputFileUpload.PostedFile.FileName)
            Dim fPath As String = HttpContext.Current.Server.MapPath(Me.UploadParamters.UploadPath) & System.IO.Path.DirectorySeparatorChar & fileName

            Dim fi As New System.IO.FileInfo(fPath)
            Dim writer As System.IO.Stream = fi.Create
            Try
                writer.Write(data, 0, data.Length)
                writer.Flush()
            Finally
                writer.Close()
            End Try
            files.Add(System.IO.Path.GetFileName(fPath))


            'ToDo: localize following string
            Me.LabelUploadedImageNames.Text = "File has been saved as "
            For Each name As String In files
                Me.LabelUploadedImageNames.Text &= "<br>" & name
            Next
        End Sub



#End Region

    End Class

End Namespace