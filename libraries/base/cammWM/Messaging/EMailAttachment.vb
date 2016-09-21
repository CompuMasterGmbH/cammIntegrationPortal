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

Namespace CompuMaster.camm.WebManager.Messaging

    ''' <summary>
    '''     An e-mail attachment
    ''' </summary>
    Public Class EMailAttachment
        Public Sub New()
            'Do nothing
        End Sub
        Public Sub New(ByVal filepath As String)
            Me.FilePath = filepath
        End Sub
        Public Sub New(ByVal filepath As String, ByVal placeholderName As String)
            Me.FilePath = filepath
            Me.PlaceholderInMhtmlToBeReplacedByContentID = placeholderName
        End Sub
        Public Sub New(ByVal data As Byte(), ByVal filename As String)
            Me.RawData = data
            Me.RawDataFilename = filename
        End Sub
        Public Sub New(ByVal data As Byte(), ByVal filename As String, ByVal placeholderName As String)
            Me.RawData = data
            Me.RawDataFilename = filename
            Me.PlaceholderInMhtmlToBeReplacedByContentID = placeholderName
        End Sub

        Private _AttachmentData As Byte()
        ''' <summary>
        '''     Binary data for this attachment
        ''' </summary>
        Public Property RawData() As Byte()
            Get
                Return _AttachmentData
            End Get
            Set(ByVal Value As Byte())
                _AttachmentData = Value
            End Set
        End Property

        Private _AttachmentData_Filename As String
        ''' <summary>
        '''     The filename for the binary data
        ''' </summary>
        Public Property RawDataFilename() As String
            Get
                Return _AttachmentData_Filename
            End Get
            Set(ByVal Value As String)
                _AttachmentData_Filename = Value
            End Set
        End Property

        Private _AttachmentFile As String
        ''' <summary>
        '''     A path to a file which shall be included
        ''' </summary>
        Public Property FilePath() As String
            Get
                Return _AttachmentFile
            End Get
            Set(ByVal Value As String)
                _AttachmentFile = Value
            End Set
        End Property

        Private _PlaceholderInMHTML_ToReplaceWithCID As String ' <Obsolete("Use ContentID instead", False), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        ''' <summary>
        '''     A placeholder string (without prefix "cid:") in the HTML code of the message (there it must be with prefix "cid:") which shall be replaced by the CID code of the attachment
        ''' </summary>
        ''' <remarks>
        ''' <para>Define the placeholder which shall be replaced by the Content-ID for the contents of a file to the email. Emails formatted in HTML can include images with this information and internally reference the image through a "cid" hyperlink.</para>
        ''' </remarks>
        Public Property PlaceholderInMhtmlToBeReplacedByContentID() As String
            Get
                Return _PlaceholderInMHTML_ToReplaceWithCID
            End Get
            Set(ByVal Value As String)
                _PlaceholderInMHTML_ToReplaceWithCID = Value
            End Set
        End Property

    End Class

End Namespace