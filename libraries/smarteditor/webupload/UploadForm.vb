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
    Public Class UploadForm
        Inherits CompuMaster.camm.SmartWebEditor.ProtectedUploadPage

        Protected WithEvents TableOptions As System.Web.UI.WebControls.Table
        Protected WithEvents LabelImageUploadFolder As System.Web.UI.WebControls.Label
        Protected WithEvents LabelImageUploadFolderValue As System.Web.UI.WebControls.Label

        Private Configuration As New ConfigurationUploadSettings

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

            'Initializes the object security
            InitializeSecurityObject()
            cammWebManager.AuthorizeDocumentAccess(cammWebManager.SecurityObject)

            'Ensure that upload folders are available - if possible and write permission is given
            CreateUploadFolders()

            'Initialize the label who are describing the upload locations targets
            Me.LabelImageUploadFolderValue.Text = Me.ImageUploadFolder

        End Sub

#Region " Protected Variables "

        Protected LabelSelectImageToUpload As System.Web.UI.WebControls.Label
        Protected InputFileUploadImage As System.Web.UI.HtmlControls.HtmlInputFile
        Protected CheckBoxImageReduction As System.Web.UI.WebControls.CheckBox
        Protected LabelUploadedImageNames As System.Web.UI.WebControls.Label
        Protected LabelWarning As System.Web.UI.WebControls.Label
        Protected WithEvents ButtonUploadImage As System.Web.UI.WebControls.Button
        Protected LabelProcessingTips As System.Web.UI.WebControls.Label

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



#End Region

#Region "Properties"


        ''' <summary>
        '''     The upload folder for images
        ''' </summary>
        Private ReadOnly Property ImageUploadFolder() As String
            Get
                Static _ImageUploadFolder As String
                If _ImageUploadFolder Is Nothing AndAlso Request.QueryString("imageupload") <> Nothing Then
                    Dim folder As String = Me.DecryptUrlParameters(Request.QueryString("imageupload"))
                    If folder.StartsWith("/") OrElse folder.StartsWith("~/") Then
                        _ImageUploadFolder = UploadTools.FullyInterpretedVirtualPath(folder)
                    Else
                        _ImageUploadFolder = UploadTools.CombineUnixPaths(GetReferencePath, folder)
                    End If
                End If
                Return _ImageUploadFolder
            End Get
        End Property


        ''' <summary>
        '''     The upload folder for documents
        ''' </summary>
        Private ReadOnly Property DocumentUploadFolder() As String
            Get
                Static _DocumentUploadFolder As String
                If _DocumentUploadFolder Is Nothing AndAlso Request.QueryString("docupload") <> Nothing Then
                    Dim folder As String = Me.DecryptUrlParameters(Request.QueryString("docupload"))
                    If folder.StartsWith("/") OrElse folder.StartsWith("~/") Then
                        _DocumentUploadFolder = UploadTools.FullyInterpretedVirtualPath(folder)
                    Else
                        _DocumentUploadFolder = UploadTools.CombineUnixPaths(GetReferencePath, folder)
                    End If
                End If
                Return _DocumentUploadFolder
            End Get
        End Property


        ''' <summary>
        '''     The upload folder for media
        ''' </summary>
        Private ReadOnly Property MediaUploadFolder() As String
            Get
                Static _MediaUploadFolder As String
                If _MediaUploadFolder Is Nothing AndAlso Request.QueryString("mediaupload") <> Nothing Then
                    Dim folder As String = Me.DecryptUrlParameters(Request.QueryString("mediaupload"))
                    If folder.StartsWith("/") OrElse folder.StartsWith("~/") Then
                        _MediaUploadFolder = UploadTools.FullyInterpretedVirtualPath(folder)
                    Else
                        _MediaUploadFolder = UploadTools.CombineUnixPaths(GetReferencePath, folder)
                    End If
                End If
                Return _MediaUploadFolder
            End Get
        End Property

#End Region

        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Initialize text
            Me.InitializeText()

            'Reset warnings
            Me.LabelWarning.Text = Nothing

            'Show configuration values for max. width, height, etc. in form
            Me.InitializeImageDataManagementSettings()

        End Sub

#Region " Initialize "

        ''' <summary>
        '''     Initialize form text
        ''' </summary>
        Private Sub InitializeText()

            'ToDo: localize following strings
            'Localizations
            Me.LabelImageUploadFolder.Text = "Bild-Ablageort:"

            Me.LabelSelectImageToUpload.Text = "Wählen Sie eine Bilddatei zum Hochladen"
            Me.ButtonUploadImage.Text = "Hochladen"
            Me.LabelProcessingTips.Text = "<b>Bearbeitungshinweise:</b><br><br>" &
                                        "1. Es können nur folgende Datenformate auf den Server geladen werden: JPEGs, GIFs, PNGs und BMPs <br><br>" &
                                        "2. Je nach Ihrer Internetanbindung ist die max. Dateigröße sowie die max. Übertragungsdauer limitiert. Falls der " &
                                        "Ladevorgang mehrmals automatisch abgebrochen wird, reduzieren Sie die Dateigröße der Datei und versuchen Sie es erneut. <br><br>" &
                                        "3. Bitte beachten Sie, dass die Pixelanzahl der Bilddatei, welche hochgeladen wird, stets größer ist als die " &
                                        "gewünschte Pixelgröße der Normalansicht. Ist dies nicht der Fall, werder die Dateien unverändert auf den Server hochgeladen. <br><br>" &
                                        ""


            Me.LabelImageDimensionQuestion.Text = "<b>Welche Dimensionen benötigen Sie?</b><br>" &
                                                "Beim Resampling bleibt das Seitenverhältnis erhalten"

            Me.LabelMiniatureView.Text = "Miniaturansicht"
            Me.LabelNormalView.Text = "Normalansicht"
            Me.LabelMaxWidth.Text = "Max. Breite"
            Me.LabelMaxHeight.Text = "Max. Höhe"

            If HttpContext.Current.Session("ImageDataManagement.FileUploaded") Is Nothing Then
                Me.LabelUploadedImageNames.Text = ""
            Else
                Me.LabelUploadedImageNames.Text = "Das Bild wurde gespeichert unter "
                Dim files As ArrayList = CType(HttpContext.Current.Session("ImageDataManagement.FileUploaded"), ArrayList)
                For Each name As String In files
                    Me.LabelUploadedImageNames.Text &= "<br>" & name
                Next

            End If

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

        Private _GetReferencePath As String

        ''' <summary>
        '''     Contains the ref var passed by url
        ''' </summary>
        Private Property GetReferencePath() As String
            Get
                If _GetReferencePath Is Nothing Then
                    _GetReferencePath = Utils.RemoveFilenameInUnixPath(DecryptUrlParameters(Request.QueryString("ref")))
                End If
                If _GetReferencePath <> Nothing AndAlso Not _GetReferencePath.EndsWith("/") Then
                    _GetReferencePath &= "/"
                End If
                If Trim(_GetReferencePath) = Nothing Then
                    Throw New Exception("Missing path reference")
                End If
                Return _GetReferencePath
            End Get
            Set(ByVal Value As String)
                _GetReferencePath = Value
            End Set
        End Property


        ''' <summary>
        '''     Initializes the property and also checks for invalid parameters
        ''' </summary>
        Private Sub InitializeSecurityObject()
            Dim result As Boolean = False
            Dim mySecurityObject As String = DecryptUrlParameters(Request.QueryString("securityobject"))
            'Check for invalid application names, passed by url
            If Trim(mySecurityObject) = Nothing OrElse LCase(mySecurityObject) = "@@anonymous" OrElse LCase(mySecurityObject) = "anonymous" Then
                Throw New Exception("Access denied in sWcms upload form because of non given/valid security object.")
            Else
                SecurityObject = mySecurityObject
            End If
        End Sub

#End Region

#Region "Events and functions"

        ''' <summary>
        '''     Handles the upload buttons click event
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub ButtonUploadImage_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonUploadImage.Click

            'Reinitialize the upload folder for pictures
            If Not Me.InputFileUploadImage.PostedFile.ContentLength > 0 AndAlso Me.InputFileUploadImage.PostedFile.FileName = "" Then
                Exit Sub
            End If
            Dim ext As String = System.IO.Path.GetExtension(Me.InputFileUploadImage.PostedFile.FileName).ToLower
            Select Case ext
                Case ".jpg", ".jpeg", ".gif", ".png", ".bmp"

                Case Else
                    Exit Sub
            End Select

            Dim fs As System.IO.Stream = Me.InputFileUploadImage.PostedFile.InputStream
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

                Dim fileName As String = System.IO.Path.GetFileNameWithoutExtension(Me.InputFileUploadImage.PostedFile.FileName) & "_" & miniatureViewWidth & "x" & miniatureViewHeight & "_" & Now.ToString("yyyyMMddHHmmss") & System.IO.Path.GetExtension(Me.InputFileUploadImage.PostedFile.FileName)
                Dim fPath As String = HttpContext.Current.Server.MapPath(Me.ImageUploadFolder) & System.IO.Path.DirectorySeparatorChar & fileName
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

                fileName = System.IO.Path.GetFileNameWithoutExtension(Me.InputFileUploadImage.PostedFile.FileName) & "_" & normalViewWidth & "x" & normalViewHeight & "_" & Now.ToString("yyyyMMddHHmmss") & System.IO.Path.GetExtension(Me.InputFileUploadImage.PostedFile.FileName)
                fPath = HttpContext.Current.Server.MapPath(Me.ImageUploadFolder) & System.IO.Path.DirectorySeparatorChar & fileName
                If Me.CheckBoxNormalView.Checked Then
                    resizer.Resize(normalViewWidth, normalViewHeight)
                    Try
                        resizer.Save(fPath, System.Drawing.Imaging.ImageFormat.Jpeg)
                    Catch ex As Exception
                        Throw New Exception("Image can not be saved, missing write permission?.", ex)
                    End Try
                    files.Add(System.IO.Path.GetFileName(fPath))
                End If
            Else
                Dim fileName As String = System.IO.Path.GetFileNameWithoutExtension(Me.InputFileUploadImage.PostedFile.FileName) & "_" & Now.ToString("yyyyMMddHHmmss") & System.IO.Path.GetExtension(Me.InputFileUploadImage.PostedFile.FileName)
                Dim fPath As String = HttpContext.Current.Server.MapPath(Me.ImageUploadFolder) & System.IO.Path.DirectorySeparatorChar & fileName

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
            'Dim files As ArrayList = CType(HttpContext.Current.Session("ImageDataManagement.FileUploaded"), ArrayList)
            For Each name As String In files
                Me.LabelUploadedImageNames.Text &= "<br>" & name
            Next

            'HttpContext.Current.Session("ImageDataManagement.FileUploaded") = files

        End Sub


        ''' <summary>
        '''     Decrypts a string passed as url parameter
        ''' </summary>
        ''' <param name="encryptedString"></param>
        Private Function DecryptUrlParameters(ByVal encryptedString As String) As String
            Dim result As String
            Try
                Try
                    'try to decode base64 string
                    Dim myCryptingEngine As CompuMaster.camm.WebManager.ICrypt = New CompuMaster.camm.WebManager.TripleDesBase64Encryption
                    result = myCryptingEngine.DeCryptString(encryptedString)
                Catch
                    'try to decode old and buggy encrypted string (not base64 Encoded)
                    Dim myCryptingEngine As CompuMaster.camm.WebManager.ICrypt = New CompuMaster.camm.WebManager.TripleDesEncryptionBase
                    result = myCryptingEngine.DeCryptString(encryptedString)
                End Try
            Catch ex As Exception
                Throw New Exception("Error while decrypting Url parameter. '" & ex.Message & "'")
            End Try
            Return result
        End Function    'DecryptUrlParameters(ByVal encryptedString As String) As String


        ''' <summary>
        '''     Creates several upload folders, based on the formula which has redirected to the form
        ''' </summary>
        Private Sub CreateUploadFolders()
            Dim result As Boolean = False

            'Create the file systems full path for folder creation
            Try
                'Create the image upload folder if necessary
                Dim myDirectoryInfo As System.IO.DirectoryInfo
                If ImageUploadFolder <> Nothing Then
                    myDirectoryInfo = New System.IO.DirectoryInfo(Server.MapPath(Me.ImageUploadFolder))
                    If Not myDirectoryInfo.Exists Then
                        myDirectoryInfo.Create()
                    End If
                End If

                If DocumentUploadFolder <> Nothing Then
                    myDirectoryInfo = New System.IO.DirectoryInfo(Server.MapPath(Me.DocumentUploadFolder))
                    If Not myDirectoryInfo.Exists Then
                        myDirectoryInfo.Create()
                    End If
                End If

                If MediaUploadFolder <> Nothing Then
                    myDirectoryInfo = New System.IO.DirectoryInfo(Server.MapPath(Me.MediaUploadFolder))
                    If Not myDirectoryInfo.Exists Then
                        myDirectoryInfo.Create()
                    End If
                End If

                result = True
            Catch ex As Exception
                Me.cammWebManager.Log.Write("WCMSWebCtrl - Create upload folders" & vbNewLine & ex.ToString)
            End Try
        End Sub

#End Region

    End Class

End Namespace