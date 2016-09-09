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

Namespace CompuMaster.camm.SmartWebEditor.Pages
    Public Class ImageBrowser
        Inherits System.Web.UI.UserControl

        Private _ImageUploadFolderPath As String
        Public Property ImageUploadFolderPath As String
            Get
                Return _ImageUploadFolderPath
            End Get
            Set(value As String)
                _ImageUploadFolderPath = value
            End Set
        End Property

        Private _EditorId As String
        Public Property EditorId As String
            Get
                Return _EditorId
            End Get
            Set(value As String)
                _EditorId = value
            End Set
        End Property



        Protected listBoxUploadedFiles As System.Web.UI.WebControls.ListBox
        Protected WithEvents btnDeleteImage As System.Web.UI.WebControls.Button
        Protected lblDeletionMessage As System.Web.UI.WebControls.Label
        Protected btnPasteToEditor As System.Web.UI.WebControls.Button

        Public Sub FillUploadedFilesListBox()
            Me.listBoxUploadedFiles.Items.Clear()
            Dim path As String = Server.MapPath(Me.ImageUploadFolderPath)
            If System.IO.Directory.Exists(path) Then
                Dim files As String() = System.IO.Directory.GetFiles(path)
                For Each filePath As String In files
                    Dim fileName As String = System.IO.Path.GetFileName(filePath)
                    Me.listBoxUploadedFiles.Items.Add(fileName)
                Next
            End If

        End Sub

        Private Sub btnDeleteImage_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeleteImage.Click
            Dim selectedImage As String = Me.listBoxUploadedFiles.SelectedValue
            If selectedImage <> Nothing Then
                selectedImage = System.IO.Path.GetFileName(selectedImage)
                Dim filePath As String = System.IO.Path.Combine(HttpContext.Current.Server.MapPath(Me.ImageUploadFolderPath), selectedImage)
                Try
                    System.IO.File.Delete(filePath)
                Catch ex As Exception
                    lblDeletionMessage.Text = "Error while trying to delete file"
                    lblDeletionMessage.ForeColor = System.Drawing.Color.Red
                    Return
                End Try
                lblDeletionMessage.Text = "File has been successfully removed"
                lblDeletionMessage.ForeColor = System.Drawing.Color.Green

            End If


        End Sub
        Private Sub PagePreRender(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            Me.FillUploadedFilesListBox()
            Me.listBoxUploadedFiles.Attributes.Add("onchange", "document.getElementById('imagePath').value = getPathOfSelectedImage();")
            If Me.EditorId = "" Then
                Me.btnPasteToEditor.Visible = False
            End If
        End Sub


    End Class

End Namespace