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
    Public Class FileBrowser
        Inherits System.Web.UI.UserControl

        Private _UploadFolderPath As String
        Public Property UploadFolderPath As String
            Get
                Return _UploadFolderPath
            End Get
            Set(value As String)
                _UploadFolderPath = value
            End Set
        End Property

        Private _ReadOnlyDirectories As String()
        Public Property ReadonlyDirectories As String()
            Get
                Return _ReadOnlyDirectories
            End Get
            Set(value As String())
                _ReadOnlyDirectories = value
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

        Private _ParentWindowCallbackFunction As String
        Public Property ParentWindowCallbackFunction As String
            Get
                Return _ParentWindowCallbackFunction
            End Get
            Set(value As String)
                _ParentWindowCallbackFunction = value
            End Set
        End Property


        Protected listBoxUploadedFiles As System.Web.UI.WebControls.ListBox
        Protected WithEvents btnDeleteFile As System.Web.UI.WebControls.Button
        Protected lblDeletionMessage As System.Web.UI.WebControls.Label
        Protected btnPasteToEditor As System.Web.UI.WebControls.Button
        Protected txtBoxFilePath As System.Web.UI.WebControls.TextBox

        ''' <summary>
        ''' Recursively reads all files from a directory and adds them to the listbox 
        ''' </summary>
        ''' <param name="physicalPath">The physical path to read recursively from </param>
        ''' <param name="virtualPath">The virtual path of the physical path</param>
        Private Sub AddFilesToListBoxRecursively(ByVal physicalPath As String, ByVal virtualPath As String)
            For Each file As String In System.IO.Directory.GetFiles(physicalPath, "*.*", IO.SearchOption.AllDirectories)
                Dim relativePath As String = file.Replace(physicalPath, String.Empty)
                Dim fileName As String = System.IO.Path.GetFileName(file)
                Dim listItem As New System.Web.UI.WebControls.ListItem
                listItem.Text = fileName
                listItem.Value = UploadTools.FullyInterpretedVirtualPath(System.IO.Path.Combine(virtualPath, relativePath).Replace("\"c, "/"c))
                Me.listBoxUploadedFiles.Items.Add(listItem)

            Next
        End Sub

        Public Sub AddReadOnlyDirectores()
            For Each directory As String In Me.ReadonlyDirectories
                Dim physicalPath As String = Server.MapPath(directory)
                If System.IO.Directory.Exists(physicalPath) Then
                    AddFilesToListBoxRecursively(physicalPath, directory)
                End If
            Next
        End Sub
        Public Sub FillUploadedFilesListBox()
            Me.listBoxUploadedFiles.Items.Clear()
            Dim physicalPath As String = Server.MapPath(Me.UploadFolderPath)
            If System.IO.Directory.Exists(physicalPath) Then
                AddFilesToListBoxRecursively(physicalPath, Me.UploadFolderPath)
            End If

            AddReadOnlyDirectores()

        End Sub

        Private Sub btnDeleteFile_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles btnDeleteFile.Click
            Dim selectedFilePath As String = Me.listBoxUploadedFiles.SelectedValue
            If selectedFilePath <> Nothing Then
                'viewstate mac should prevent traversal attacks...
                Dim filePath As String = Server.MapPath(selectedFilePath)
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
        Private Sub Control_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            If Not Me.Page.EnableViewStateMac Then
                Throw New Exception("ViewStateMac must be enabled")
            End If
        End Sub
        Private Sub Control_PreRender(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            Me.FillUploadedFilesListBox()
            Me.listBoxUploadedFiles.Attributes.Add("onchange", "document.getElementById('" & Me.txtBoxFilePath.ClientID & "').value = this.value")
            If Me.EditorId = "" Then
                Me.btnPasteToEditor.Visible = False
            End If

        End Sub


    End Class

End Namespace