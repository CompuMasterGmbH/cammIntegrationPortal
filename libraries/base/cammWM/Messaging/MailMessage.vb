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
    '''     The content of an e-mail with all required data
    ''' </summary>
    ''' <remarks>
    '''     ToDo: some fields are not yet use in queueing and queue send method, implement them everywhere
    ''' </remarks>
    Public Class MailMessage

        Sub New(ByVal xml As String, ByVal webManager As WMSystem)
            Dim MailData As DataSet = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertXmlToDataset(xml)
            Dim MessageData As DataTable = MailData.Tables("message")
            Dim Attachments As DataTable = MailData.Tables("attachments")
            Dim Headers As DataTable = MailData.Tables("headers")

            'Restore header information
            Dim additionalHeaders As New Specialized.NameValueCollection
            For MyCounter As Integer = 0 To Headers.Rows.Count - 1
                additionalHeaders.Add(CType(Headers.Rows(MyCounter)("key"), String), CType(Headers.Rows(MyCounter)("value"), String))
            Next

            'Restore attachments
            Dim mailAttachments As New ArrayList
            For MyCounter As Integer = 0 To Attachments.Rows.Count - 1
                Dim EMailAttachment As New CompuMaster.camm.WebManager.Messaging.EMailAttachment
                EMailAttachment.PlaceholderInMhtmlToBeReplacedByContentID = Utils.Nz(Attachments.Rows(MyCounter)("Placeholder"), CType(Nothing, String))
                EMailAttachment.RawData = CType(Utils.Nz(Attachments.Rows(MyCounter)("FileData")), Byte())
                EMailAttachment.RawDataFilename = Utils.Nz(Attachments.Rows(MyCounter)("FileName"), CType(Nothing, String))
                mailAttachments.Add(EMailAttachment)
            Next

            'Restore message data
            Dim FromName As String = Nothing, FromAddress As String = Nothing, RcptTo As String = Nothing, RcptCc As String = Nothing, RcptBcc As String = Nothing
            Dim Subject As String = Nothing, Charset As String = Nothing, BodyText As String = Nothing, BodyHtml As String = Nothing
            For MyCounter As Integer = 0 To MessageData.Rows.Count - 1
                Select Case LCase(CType(MessageData.Rows(MyCounter)("key"), String))
                    Case "fromname"
                        FromName = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "fromaddress"
                        FromAddress = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "to"
                        RcptTo = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "cc"
                        RcptCc = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "bcc"
                        RcptBcc = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "subject"
                        Subject = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "charset"
                        Charset = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "textbody"
                        BodyText = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case "htmlbody"
                        BodyHtml = Utils.Nz(MessageData.Rows(MyCounter)("value"), CType(Nothing, String))
                    Case Else
                        'Invalid
                        'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                        Dim WorkaroundEx As New Exception("")
                        Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                        Try
                            WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                        Catch
                        End Try
                        webManager.Log.RuntimeWarning("Case for """ & LCase(CType(MessageData.Rows(MyCounter)("key"), String)) & """ doesn't exist", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)
                End Select
            Next

            'Now, send the e-mail and return
            Dim EMailAttachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = CType(mailAttachments.ToArray(GetType(CompuMaster.camm.WebManager.Messaging.EMailAttachment)), Messaging.EMailAttachment())

            'Now, fill the class instance
            Me.To = RcptTo
            Me.Cc = RcptCc
            Me.Bcc = RcptBcc
            Me.Subject = Subject
            Me.BodyPlainText = BodyText
            Me.BodyHtml = BodyHtml
            Me.FromName = FromName
            Me.FromAddress = FromAddress
            Me.EMailAttachments = EMailAttachments
            Me.AdditionalHeaders = additionalHeaders
            Me.Charset = Charset

        End Sub

        Private _Charset As String
        Public Property Charset() As String
            Get
                Return Me._Charset
            End Get
            Set(ByVal Value As String)
                Me._Charset = Value
            End Set
        End Property

        Private _RequestReadingConfirmation As Boolean
        Public Property RequestReadingConfirmation() As Boolean
            Get
                Return Me._RequestReadingConfirmation
            End Get
            Set(ByVal Value As Boolean)
                Me._RequestReadingConfirmation = Value
            End Set
        End Property

        Private _RequestTransmissionConfirmation As Boolean
        Public Property RequestTransmissionConfirmation() As Boolean
            Get
                Return Me._RequestTransmissionConfirmation
            End Get
            Set(ByVal Value As Boolean)
                Me._RequestTransmissionConfirmation = Value
            End Set
        End Property

        Private _AdditionalHeaders As System.Collections.Specialized.NameValueCollection
        Public Property AdditionalHeaders() As System.Collections.Specialized.NameValueCollection
            Get
                Return Me._AdditionalHeaders
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                Me._AdditionalHeaders = Value
            End Set
        End Property

        Private _Sensitivity As Messaging.EMails.Sensitivity
        Public Property Sensitivity() As Messaging.EMails.Sensitivity
            Get
                Return Me._Sensitivity
            End Get
            Set(ByVal Value As Messaging.EMails.Sensitivity)
                Me._Sensitivity = Value
            End Set
        End Property

        Private _Priority As Messaging.EMails.Priority
        Public Property Priority() As Messaging.EMails.Priority
            Get
                Return Me._Priority
            End Get
            Set(ByVal Value As Messaging.EMails.Priority)
                Me._Priority = Value
            End Set
        End Property

        Private _FromAddress As String
        Public Property FromAddress() As String
            Get
                Return Me._FromAddress
            End Get
            Set(ByVal Value As String)
                Me._FromAddress = Value
            End Set
        End Property

        Private _FromName As String
        Public Property FromName() As String
            Get
                Return Me._FromName
            End Get
            Set(ByVal Value As String)
                Me._FromName = Value
            End Set
        End Property

        Private _To As String
        Public Property [To]() As String
            Get
                Return Me._To
            End Get
            Set(ByVal Value As String)
                Me._To = Value
            End Set
        End Property

        Private _Cc As String
        Public Property Cc() As String
            Get
                Return Me._Cc
            End Get
            Set(ByVal Value As String)
                Me._Cc = Value
            End Set
        End Property

        Private _Bcc As String
        Public Property Bcc() As String
            Get
                Return _Bcc
            End Get
            Set(ByVal Value As String)
                _Bcc = Value
            End Set
        End Property

        Private _Subject As String
        Public Property Subject() As String
            Get
                Return _Subject
            End Get
            Set(ByVal Value As String)
                _Subject = Value
            End Set
        End Property

        Private _BodyPlainText As String
        Public Property BodyPlainText() As String
            Get
                Return _BodyPlainText
            End Get
            Set(ByVal Value As String)
                _BodyPlainText = Value
            End Set
        End Property

        Private _BodyHtml As String
        Public Property BodyHtml() As String
            Get
                Return _BodyHtml
            End Get
            Set(ByVal Value As String)
                _BodyHtml = Value
            End Set
        End Property

        Private _Attachments As CompuMaster.camm.WebManager.WMSystem.EMailAttachement()
        <Obsolete("Use EMailAttachments instead"), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property Attachments() As CompuMaster.camm.WebManager.WMSystem.EMailAttachement()
            Get
                If _Attachments Is Nothing Then
                    _Attachments = New CompuMaster.camm.WebManager.WMSystem.EMailAttachement() {}
                End If
                Return _Attachments
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.EMailAttachement())
                _Attachments = Value
            End Set
        End Property

        Private _EMailAttachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment()
        Public Property EMailAttachments() As CompuMaster.camm.WebManager.Messaging.EMailAttachment()
            Get
                If _EMailAttachments Is Nothing Then
                    _EMailAttachments = New CompuMaster.camm.WebManager.Messaging.EMailAttachment() {}
                End If
                Return _EMailAttachments
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.Messaging.EMailAttachment())
                _EMailAttachments = Value
            End Set
        End Property

    End Class

End Namespace