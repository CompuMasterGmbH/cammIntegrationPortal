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

Option Explicit On
Option Strict On

Namespace CompuMaster.camm.WebManager.Modules.WebEdit.Controls

    ''' <summary>
    '''     The smart and built-in content management system of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    '''     This page contains a web editor which saves/load the content to/from the CWM database. The editor will only be visible for those people with appropriate authorization. All other people will only see the content, but nothing to modify it.
    '''     The content may be different for languages or markets. In all cases, there will be a version history.
    '''     When there is no content for an URL in the database - or it hasn't been released - the page request will lead to an HTTP 404 error code.
    ''' </remarks>
    <Obsolete("Better use one of the cammWM.SmartEditor controls instead")> Public Class SmartWcms3
        Inherits SmartPlainHtmlEditor

        Property CachedData_WarningAlreadySent As Boolean
            Get
                Return CType(Me.Page.Cache("CachedData_WarningAlreadySent|" & Me.Page.Request.Url.AbsolutePath), Boolean)
            End Get
            Set(value As Boolean)
                Me.Page.Cache("CachedData_WarningAlreadySent|" & Me.Page.Request.Url.AbsolutePath) = value
            End Set
        End Property

        Private Sub SendWarningMail(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If CachedData_WarningAlreadySent = False Then
                CachedData_WarningAlreadySent = True
                Dim PlainTextBody As String = "Please replace control SmartWcms3 and SmartWcms (both obsolete, both provided by cammWM.dll) by another SmartEditor control (see e.g. cammWM.SmartEditor.dll)"
                Dim HtmlBody As String = System.Web.HttpUtility.HtmlEncode(PlainTextBody)
                Me.cammWebManager.Log.ReportWarningViaEMail(PlainTextBody, HtmlBody, "WARNING: obsolete SmartEditor to be replaced")
            End If
        End Sub

#Region "For compatibility only: binary interface stays untouched"
        Private _Docs As String
        ''' <summary>
        '''     Contains the control specific upload folder for documents
        ''' </summary>
        ''' <value></value>
        Public Property Docs() As String
            Get
                Return _Docs
            End Get
            Set(ByVal Value As String)
                _Docs = Value
                If Me.DocsReadOnly Is Nothing OrElse Me.DocsReadOnly.Length = 0 Then
                    Me.DocsReadOnly = New String() {Value}
                End If
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
        Private _DocsUploadSizeMax As Integer = 512000
        ''' <summary>
        '''     Max. upload size for documents in Bytes
        ''' </summary>
        ''' <value></value>
        Public Property DocsUploadSizeMax() As Integer
            Get
                Return _DocsUploadSizeMax
            End Get
            Set(ByVal Value As Integer)
                _DocsUploadSizeMax = Value
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property

        Private _BaseDocsUploadFilter As String() = New String() {"*.rtf", "*.csv", "*.xml", "*.ppt", "*.pptm", "*.pptx", "*.pps", "*.ppsx", "*.pdf", "*.txt", "*.doc", "*.docm", "*.docx", "*.xls", "*.xlsx", "*.xlsm", "*.xlsb", "*.xlt", "*.xltx", "*.xltm", "*.pot", "*.potx", "*.potm", "*.dot", "*.dotx", "*.dotm", "*.xps", "*.odt", "*.ott", "*.odp", "*.otp", "*.ods", "*.ots", "*.odg"}
        Private _DocsUploadFilter As String() = New String() {}
        ''' <summary>
        '''     Contains the control specific upload filter for documents
        ''' </summary>
        ''' <value></value>
        ''' <history>
        '''     [link]		30.08.2007  Fixed
        '''     [zeutzheim] 10.05.2007  Fixed
        ''' 	[swiercz]	06.12.2005	Created
        ''' </history>
        <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Property DocsUploadFilter() As String()
            Get
                'Here we also add the upper and lower case of the given filters, otherwise you are not allowed
                'to upload e.g. test.TXT
                'This workarround is necessary, because we do not have the source code of rad-editor to disable
                'case sensitive handling of the filters


                Dim savecount As Integer = UBound(_BaseDocsUploadFilter)
                ReDim _DocsUploadFilter((2 * _BaseDocsUploadFilter.Length) - 1)

                For i As Integer = 0 To savecount
                    _DocsUploadFilter(i + savecount + 1) = _BaseDocsUploadFilter(i).ToUpper
                    _DocsUploadFilter(i) = _BaseDocsUploadFilter(i).ToLower
                Next

                Return _DocsUploadFilter
            End Get
            Set(ByVal Value As String())
                _BaseDocsUploadFilter = Value

                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
        Private _DocsReadOnly As String() = New String() {}
        ''' <summary>
        '''     Contains the control specific readonly folders for documents
        ''' </summary>
        ''' <value></value>
        <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Overloads Property DocsReadOnly() As String()
            Get
                Return _DocsReadOnly
            End Get
            Set(ByVal Value As String())
                _DocsReadOnly = Value
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property

        Private _Media As String
        ''' <summary>
        '''     Contains the control specific upload folder for media files
        ''' </summary>
        ''' <value></value>
        Public Property Media() As String
            Get
                Return _Media
            End Get
            Set(ByVal Value As String)
                _Media = Value
                If Me.MediaReadOnly Is Nothing OrElse Me.MediaReadOnly.Length = 0 Then
                    Me.MediaReadOnly = New String() {Value}
                End If
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
        Private _MediaUploadSizeMax As Integer = 512000
        ''' <summary>
        '''     Max. upload size for media files in Bytes
        ''' </summary>
        ''' <value></value>
        Public Property MediaUploadSizeMax() As Integer
            Get
                Return _MediaUploadSizeMax
            End Get
            Set(ByVal Value As Integer)
                _MediaUploadSizeMax = Value
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
        Private _MediaReadOnly As String() = New String() {}
        ''' <summary>
        '''     Contains the control specific readonly folders for media files
        ''' </summary>
        ''' <value></value>
        <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Overloads Property MediaReadOnly() As String()
            Get
                Return _MediaReadOnly
            End Get
            Set(ByVal Value As String())
                _MediaReadOnly = Value
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property

        Private _Images As String
        ''' <summary>
        '''     Contains the control specific upload folder for images
        ''' </summary>
        ''' <value></value>
        Public Property Images() As String
            Get
                Return _Images
            End Get
            Set(ByVal Value As String)
                _Images = Value
                If Me.ImagesReadOnly Is Nothing OrElse Me.ImagesReadOnly.Length = 0 Then
                    Me.ImagesReadOnly = New String() {Value}
                End If
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
        Private _ImagesUploadSizeMax As Integer = 512000
        ''' <summary>
        '''     Max. upload size for images in Bytes
        ''' </summary>
        ''' <value></value>
        Public Property ImagesUploadSizeMax() As Integer
            Get
                Return _ImagesUploadSizeMax
            End Get
            Set(ByVal Value As Integer)
                _ImagesUploadSizeMax = Value
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
        Private _ImagesReadOnly As String() = New String() {}
        ''' <summary>
        '''     Contains the control specific readonly folders for images
        ''' </summary>
        ''' <value></value>
        <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Overloads Property ImagesReadOnly() As String()
            Get
                Return _ImagesReadOnly
            End Get
            Set(ByVal Value As String())
                _ImagesReadOnly = Value
                If Me.ChildControlsCreated Then
                    'AssignPropertiesToChildEditor()
                End If
            End Set
        End Property
#End Region

    End Class

End Namespace