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

Namespace CompuMaster.camm.SmartWebEditor.Pages.Upload

    Public Class GeneralUploadForm
        Inherits CompuMaster.camm.SmartWebEditor.ProtectedUploadPage

        Protected WithEvents LabelFileUploadFolder As System.Web.UI.WebControls.Label
        Protected WithEvents LabelFileUploadFolderValue As System.Web.UI.WebControls.Label

        Private Configuration As New ConfigurationUploadSettings

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            InitializeSecurityObject()
            cammWebManager.AuthorizeDocumentAccess(cammWebManager.SecurityObject)
            CreateUploadFolders()

            Me.LabelFileUploadFolderValue.Text = Me.FileUploadFolder

        End Sub

#Region " Protected Variables "

        Protected LabelSelectFileToUpload As System.Web.UI.WebControls.Label
        Protected InputFileUpload As System.Web.UI.HtmlControls.HtmlInputFile
        Protected LabelUploadFilesList As System.Web.UI.WebControls.Label
        Protected LabelWarning As System.Web.UI.WebControls.Label
        Protected WithEvents ButtonUploadFile As System.Web.UI.WebControls.Button
        Protected LabelProcessingTips As System.Web.UI.WebControls.Label




#End Region

#Region "Properties"


        Private _FileUploadFolder As String
        ''' <summary>
        '''     The upload folder for files
        ''' </summary>
        Protected ReadOnly Property FileUploadFolder() As String
            Get
                If _FileUploadFolder Is Nothing AndAlso Request.QueryString("uploadpath") <> Nothing Then
                    Dim folder As String = Me.DecryptUrlParameters(Request.QueryString("uploadpath"))

                    If folder.StartsWith("/") OrElse folder.StartsWith("~/") Then
                        _FileUploadFolder = UploadTools.FullyInterpretedVirtualPath(folder)
                    Else
                        _FileUploadFolder = UploadTools.CombineUnixPaths(GetReferencePath, folder)
                    End If
                End If
                Return _FileUploadFolder
            End Get
        End Property

        Protected ReadOnly Property MaxFileUploadSize() As Integer
            Get
                Dim key As String = "maxfileuploadsize"
                If Request.QueryString(key) <> Nothing Then
                    Return Integer.Parse(Me.DecryptUrlParameters(Request.QueryString(key)))
                End If
                Return 0
            End Get
        End Property

        Protected ReadOnly Property ReadOnlyDirectories() As String()
            Get
                Dim key As String = "readonlydirs"
                If Request.QueryString(key) <> Nothing Then
                    Dim decryptedParam As String = Me.DecryptUrlParameters(Request.QueryString(key))
                    Return decryptedParam.Split(";"c)
                End If
                Return Nothing
            End Get
        End Property


        Public ReadOnly Property ParentWindowCallbackFunction As String
            Get
                Dim key As String = "callbackfunction"
                If Request.QueryString(key) <> Nothing Then
                    Dim decryptedParam As String = Me.DecryptUrlParameters(Request.QueryString(key))
                    Return decryptedParam
                End If
                Return Nothing
            End Get
        End Property


#End Region
        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.InitializeText()
            Me.LabelWarning.Text = Nothing
        End Sub

#Region " Initialize "

        ''' <summary>
        '''     Initialize form text
        ''' </summary>
        Protected Overridable Sub InitializeText()
            'ToDo: localize following strings
            Me.LabelFileUploadFolder.Text = "Upload location:"
            Me.LabelSelectFileToUpload.Text = "Choose a file to upload"
            Me.ButtonUploadFile.Text = "Upload"
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

        Private _AllowedFileExtensions As String()
        Protected Property AllowedFileExtensions As String()
            Get
                Return _AllowedFileExtensions
            End Get
            Set(value As String())
                _AllowedFileExtensions = value
            End Set
        End Property

#End Region

#Region "Events and functions"

        Protected Function IsAllowedExtension(ByVal extension As String) As Boolean
            Return Array.IndexOf(_AllowedFileExtensions, extension) > -1
        End Function

        ''' <summary>
        '''     Decrypts a string passed as url parameter
        ''' </summary>
        ''' <param name="encryptedString"></param>
        Protected Function DecryptUrlParameters(ByVal encryptedString As String) As String
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
        Protected Sub CreateUploadFolders()
            Try
                If Me.FileUploadFolder <> Nothing Then
                    Dim myDirectoryInfo As New System.IO.DirectoryInfo(Server.MapPath(Me.FileUploadFolder))
                    If Not myDirectoryInfo.Exists Then
                        myDirectoryInfo.Create()
                    End If
                End If
            Catch ex As Exception
                Me.cammWebManager.Log.Write("WCMSWebCtrl - Create upload folders" & vbNewLine & ex.ToString)
            End Try
        End Sub

#End Region

    End Class

End Namespace