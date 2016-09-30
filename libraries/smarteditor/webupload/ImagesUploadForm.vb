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
    ''' Image upload form (page is centrally positioned in /system/... folder)
    ''' </summary>
    Public Class ImagesUploadForm
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

        Protected LabelImageDimensionQuestion As System.Web.UI.WebControls.Label
        Protected LabelMiniatureView As System.Web.UI.WebControls.Label
        Protected CheckBoxMiniatureView As System.Web.UI.WebControls.CheckBox
        Protected LabelNormalView As System.Web.UI.WebControls.Label
        Protected CheckBoxNormalView As System.Web.UI.WebControls.CheckBox
        Protected LabelMaxWidth As System.Web.UI.WebControls.Label
        Protected TextBoxMiniatureMaxWidth As System.Web.UI.WebControls.TextBox
        Protected TextBoxNormalMaxWidth As System.Web.UI.WebControls.TextBox
        Protected LabelMaxHeight As System.Web.UI.WebControls.Label
        Protected TextBoxMiniatureMaxHeight As System.Web.UI.WebControls.TextBox
        Protected TextBoxNormalMaxHeight As System.Web.UI.WebControls.TextBox

        Protected LabelOriginalView As System.Web.UI.WebControls.Label
        Protected CheckBoxOriginalView As System.Web.UI.WebControls.CheckBox

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

            'Show configuration values for max. width, height, etc. in form
            Me.InitializeImageDataManagementSettings()

            Me.InitializeControls()

        End Sub

#Region "Initialize "

        Protected NoImageChosenJavascriptMessageText As String
        Protected OnlyFollowingExtensionsAreAllowed As String

        ''' <summary>
        '''     Initialize form text
        ''' </summary>
        Protected Overrides Sub InitializeText()

            'Localizations
            Me.LabelFileUploadFolder.Text = My.Resources.Label_FileUploadFolder
            Me.LabelSelectFileToUpload.Text = My.Resources.Label_SelectImageFileToUpload
            Me.ButtonUploadFile.Text = My.Resources.Label_UploadFile
            Me.LabelProcessingTips.Text = String.Format(My.Resources.Label_ImageProcessingTips, String.Join(", ", Me.UploadParamters.AllowedFileExtensions))
            Me.LabelImageDimensionQuestion.Text = My.Resources.Label_ImageDimensionQuestion

            Me.LabelMiniatureView.Text = My.Resources.Label_MiniatureView
            Me.LabelNormalView.Text = My.Resources.Label_NormalView
            Me.LabelMaxWidth.Text = My.Resources.Label_MaxWidth
            Me.LabelMaxHeight.Text = My.Resources.Label_MaxHeight
            Me.NoImageChosenJavascriptMessageText = My.Resources.Label_NoImageChosenJavascriptMessageText
            Me.OnlyFollowingExtensionsAreAllowed = My.Resources.Label_OnlyFollowingExtensionsAreAllowed
            Me.ltrlInsertSectionHeadline.Text = My.Resources.Label_InsertSectionHeadline

        End Sub

        ''' <summary>
        '''     Initialize form text fields
        ''' </summary>
        Private Sub InitializeImageDataManagementSettings()

            If Not Me.IsPostBack Then
                Me.TextBoxMiniatureMaxWidth.Text = CStr(Configuration.MiniatureViewMaxWidth)
                Me.TextBoxMiniatureMaxHeight.Text = CStr(Configuration.MiniatureViewMaxHeight)
                Me.TextBoxNormalMaxWidth.Text = CStr(Configuration.NormalViewMaxWidth)
                Me.TextBoxNormalMaxHeight.Text = CStr(Configuration.NormalViewMaxHeight)

                Me.CheckBoxMiniatureView.Checked = Configuration.MiniatureView
                Me.CheckBoxNormalView.Checked = Configuration.NormalView

            End If

        End Sub


#End Region


#Region "Events and functions"

        ''' <summary>
        '''     Handles the upload buttons click event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub ButtonUploadImage_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonUploadFile.Click
            'Reinitialize the upload folder for pictures
            If Not Me.InputFileUpload.PostedFile.ContentLength > 0 AndAlso Me.InputFileUpload.PostedFile.FileName = "" Then
                Exit Sub
            End If

            Dim ext As String = System.IO.Path.GetExtension(Me.InputFileUpload.PostedFile.FileName).ToLower
            If Not Me.IsAllowedExtension(ext) Then
                Me.LabelWarning.Text = "Invalid file extension"
                Exit Sub
            End If

            If Me.UploadParamters.MaxUploadSize > 0 Then
                Dim fileSize As Integer = Me.InputFileUpload.PostedFile.ContentLength
                If fileSize > Me.UploadParamters.MaxUploadSize Then
                    LabelWarning.Text = "Uploaded file is too large."
                    Exit Sub
                End If
            End If

            Dim fs As System.IO.Stream = Me.InputFileUpload.PostedFile.InputStream
            Dim data(CType(fs.Length, Integer)) As Byte
            fs.Read(data, 0, CType(fs.Length, Integer))
            fs.Close()

            Dim files As New ArrayList
            If Me.CheckBoxMiniatureView.Checked OrElse Me.CheckBoxNormalView.Checked Then
                Dim miniatureViewWidth As Integer = Utils.TryCInt(Trim(HttpContext.Current.Request.Form(Me.TextBoxMiniatureMaxWidth.ID)))
                Dim miniatureViewHeight As Integer = Utils.TryCInt(Trim(HttpContext.Current.Request.Form(Me.TextBoxMiniatureMaxHeight.ID)))
                Dim normalViewWidth As Integer = Utils.TryCInt(Trim(HttpContext.Current.Request.Form(Me.TextBoxNormalMaxWidth.ID)))
                Dim normalViewHeight As Integer = Utils.TryCInt(Trim(HttpContext.Current.Request.Form(Me.TextBoxNormalMaxHeight.ID)))

                Dim resizer As New CompuMaster.Drawing.Imaging.ImageScaling
                Try
                    resizer.Read(data)
                Catch
                    LabelWarning.Text = "Uploaded file doesn't seem to be a valid image file. Read operation aborted."
                    Exit Sub
                End Try

                Dim fileName As String = System.IO.Path.GetFileNameWithoutExtension(Me.InputFileUpload.PostedFile.FileName) & "_" & miniatureViewWidth & "x" & miniatureViewHeight & "_" & Now.ToString("yyyyMMddHHmmss") & System.IO.Path.GetExtension(Me.InputFileUpload.PostedFile.FileName)
                Dim fPath As String = HttpContext.Current.Server.MapPath(Me.UploadParamters.UploadPath) & System.IO.Path.DirectorySeparatorChar & fileName
                If Me.CheckBoxMiniatureView.Checked Then
                    Try
                        resizer.Resize(miniatureViewWidth, miniatureViewHeight)
                    Catch
                        LabelWarning.Text = "Uploaded file doesn't seem to be a valid image file. Resize operation aborted."
                        Exit Sub
                    End Try
                    Try
                        resizer.Save(fPath, System.Drawing.Imaging.ImageFormat.Jpeg)
                    Catch ex As Exception
                        If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                            Throw New Exception("Image """ & fPath & """ can not be saved, missing write permission?", ex)
                        Else
                            Throw New Exception("Image can not be saved, missing write permission?", ex)
                        End If
                        Throw New Exception("Image can not be saved, missing write permission?", ex)
                    End Try
                    files.Add(System.IO.Path.GetFileName(fPath))
                End If

                fileName = System.IO.Path.GetFileNameWithoutExtension(Me.InputFileUpload.PostedFile.FileName) & "_" & normalViewWidth & "x" & normalViewHeight & "_" & Now.ToString("yyyyMMddHHmmss") & System.IO.Path.GetExtension(Me.InputFileUpload.PostedFile.FileName)
                fPath = HttpContext.Current.Server.MapPath(Me.UploadParamters.UploadPath) & System.IO.Path.DirectorySeparatorChar & fileName
                If Me.CheckBoxNormalView.Checked Then
                    resizer.Resize(normalViewWidth, normalViewHeight)
                    Try
                        resizer.Save(fPath, System.Drawing.Imaging.ImageFormat.Jpeg)
                    Catch ex As Exception
                        Throw New Exception("Image can not be saved, missing write permission?.", ex)
                    End Try
                    files.Add(System.IO.Path.GetFileName(fPath))
                End If
            End If
            If Me.CheckBoxOriginalView.Checked Then
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
            End If

            'ToDo: localize following string
            Me.LabelUploadedImageNames.Text = "Das Bild wurde gespeichert unter "
            For Each name As String In files
                Me.LabelUploadedImageNames.Text &= "<br>" & name
            Next
        End Sub



#End Region

    End Class

End Namespace