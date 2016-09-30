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
        Protected ltrlInsertSectionHeadline As System.Web.UI.WebControls.Literal

        Private Configuration As New ConfigurationUploadSettings

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            SetUploadParamters()
            InitializeSecurityObject()
            cammWebManager.AuthorizeDocumentAccess(cammWebManager.SecurityObject)
            CreateUploadFolders()

            Me.LabelFileUploadFolderValue.Text = Me.UploadParamters.UploadPath

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

        Protected UploadParamters As CompuMaster.camm.SmartWebEditor.UploadFormParameters

        Private Sub SetUploadParamters()
            Dim guid As String = Request.QueryString("key")
            If guid Is Nothing Then
                Throw New Exception("Missing uid paramater")
            End If
            Dim base64Data As Object = Utils.Nz(Me.cammWebManager.System_GetSessionValue(guid))
            If base64Data Is Nothing Then
                Throw New Exception("Error while trying to retrieve upload paramaters")
            End If
            Dim serializedBytes As Byte() = Convert.FromBase64String(CType(base64Data, String))
            Dim memStreams As New System.IO.MemoryStream(serializedBytes)
            Dim formatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

            Me.UploadParamters = CType(formatter.Deserialize(memStreams), UploadFormParameters)
        End Sub

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


        ''' <summary>
        '''     Initializes the property and also checks for invalid parameters
        ''' </summary>
        Protected Sub InitializeSecurityObject()
            Dim result As Boolean = False
            Dim mySecurityObject As String = UploadParamters.SecurityObject
            'Check for invalid application names, passed by url
            If Trim(mySecurityObject) = Nothing OrElse LCase(mySecurityObject) = "@@anonymous" OrElse LCase(mySecurityObject) = "anonymous" Then
                Throw New Exception("Access denied in sWcms upload form because of non given/valid security object.")
            Else
                SecurityObject = mySecurityObject
            End If
        End Sub

#End Region

#Region "Events and functions"

        Protected Function IsAllowedExtension(ByVal extension As String) As Boolean
            Return Array.IndexOf(Me.UploadParamters.AllowedFileExtensions, extension) > -1
        End Function

        ''' <summary>
        '''     Creates several upload folders, based on the formula which has redirected to the form
        ''' </summary>
        Protected Sub CreateUploadFolders()
            Try
                If Me.UploadParamters.UploadPath <> Nothing Then
                    Dim myDirectoryInfo As New System.IO.DirectoryInfo(Server.MapPath(Me.UploadParamters.UploadPath))
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