'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Explicit On
Option Strict On

Imports System.Data
Imports System.Data.SqlClient
Imports System.Web

Namespace CompuMaster.camm.WebManager.Messaging

    ''' <summary>
    '''     Messaging classes for e-mail, SMS, etc.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Friend Class NamespaceDoc
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Messaging.EMails
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Mail delivery methods
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	04.05.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class EMails

        Friend _MailSendingSystem As MailSendingSystem = MailSendingSystem.Auto
        Public Enum MailSendingSystem As Integer
            [Auto] = -1
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> ChilkatActiveX = 0
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> ChilkatNet = 1
            Queue = 2
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> NetFramework1 = -3
            NetFramework = 3
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> EasyMail = 4
        End Enum

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The preferred system for sending e-mails
        ''' </summary>
        ''' <value>The new favorite</value>
        ''' <remarks>
        '''     Please note: if the mail system in unavailable, camm Web-Manager tries to send the e-mail with other systems automatically.
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property MailSystem() As MailSendingSystem
            Get
                Return _MailSendingSystem
            End Get
            Set(ByVal Value As MailSendingSystem)
                _MailSendingSystem = Value
            End Set
        End Property

        Public Enum Priority As Integer
            High = 1
            Normal = 3
            Low = 5
        End Enum

        Public Enum Sensitivity As Integer
            Status_Normal = 1
            Status_Personal = 2
            Status_Private = 3
            Status_CompanyConfidential = 4
        End Enum

#Region "Common methods"

#If ImplementedSubClassesWithIWebManagerInterface Then
        Private _WebManager As IWebManager
#End If
        Private _WebManager As WMSystem
        Friend Sub New(ByVal cammWebManager As WMSystem)
            _WebManager = cammWebManager
        End Sub

#Region "SendEMail"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Send an e-mail
        ''' </summary>
        ''' <param name="rcptName">The name of the receipient</param>
        ''' <param name="rcptAddress">The e-mail address of the receipient</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal rcptName As String, ByVal rcptAddress As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String) As Boolean
            Dim attachment() As CompuMaster.camm.WebManager.Messaging.EMailAttachment = Nothing
            Return SendEMail(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, attachment, Messaging.EMails.Priority.Normal, Messaging.EMails.Sensitivity.Status_Normal, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Send an e-mail
        ''' </summary>
        ''' <param name="rcptName">The name of the receipient</param>
        ''' <param name="rcptAddress">The e-mail address of the receipient</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal rcptName As String, ByVal rcptAddress As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String) As Boolean
            Return SendEMail(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, replyToName, replyToAddress, CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function SendEMail(ByVal rcptName As String, ByVal rcptAddress As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal attachments As CompuMaster.camm.WebManager.WMSystem.EMailAttachement(), Optional ByVal priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal requestTransmissionConfirmation As Boolean = False, Optional ByVal requestReadingConfirmation As Boolean = False, Optional ByVal additionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal msgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            Return SendEMail(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(attachments), CType(priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders, msgCharset, bufErrorDetails)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail
        ''' </summary>
        ''' <param name="rcptName">The name of the receipient</param>
        ''' <param name="rcptAddress">The e-mail address of the receipient</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal rcptName As String, ByVal rcptAddress As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment(), Optional ByVal priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal requestTransmissionConfirmation As Boolean = False, Optional ByVal requestReadingConfirmation As Boolean = False, Optional ByVal additionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal msgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            Log.WriteEventLogTrace("SendEMail:Begin")
            Dim Result As Boolean

            'Validate and auto-complete receipient information
            If rcptAddress = Nothing Then
                Try
                    Throw New ArgumentNullException("rcptAddress")
                Catch ex As Exception
                    bufErrorDetails = ex.ToString
                    Log.WriteEventLogTrace("SendEMail:Aborted:RcptAddress")
                    Return False
                End Try
            End If
            If rcptName = Nothing Then
                rcptName = rcptAddress
            End If

            'Auto-complete sender information
            If senderAddress = "" Then
                senderAddress = _WebManager.StandardEMailAccountAddress
                senderName = _WebManager.StandardEMailAccountName
            ElseIf senderName = "" Then
                senderName = senderAddress
            End If

            If msgCharset = "" Then
                msgCharset = "UTF-8"
            End If


            If Me._MailSendingSystem = Messaging.EMails.MailSendingSystem.Queue OrElse (_MailSendingSystem = Messaging.EMails.MailSendingSystem.Auto AndAlso _WebManager.IsSupported.Messaging.eMailQueue) Then

                'Store the e-mail into the mail queue
                Dim QueueResult As Boolean = SendViaQueue(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, msgCharset, Nothing, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                Log.WriteEventLogTrace("SendEMail:End:SentByQueue:" & QueueResult.ToString)
                Return QueueResult

            ElseIf _WebManager.IsSupported.Messaging.eMail Then

                'Auto detection analyses the environment and updates the default mail sending system
                If _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.Auto Then
                    _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.NetFramework Then
                    'do nothing, because it is supported
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.NetFramework1 Then
                    'do nothing, because it is supported
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.ChilkatActiveX Then
                    _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.ChilkatNet Then
                    _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.EasyMail Then
                    _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework
                Else
                    Log.WriteEventLogTrace("SendEMail:Aborted:NotImplementedMailComponent:1")
                    Throw New NotImplementedException("Not yet implemented: unknown mail component """ & _MailSendingSystem & """")
                End If

                'Now, delegate the call to the corresponding mail method
                If _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework Then
                    Result = System_SendEMailEx_NetFramework(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                ElseIf _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework1 Then
                    Result = System_SendEMailEx_NetFramework1(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                Else
                    Log.WriteEventLogTrace("SendEMail:Aborted:NotImplementedMailComponent:2")
                    Throw New NotImplementedException("Not yet implemented: unknown mail component """ & _MailSendingSystem & """")
                End If
            Else
                _WebManager.Log.RuntimeException("Mail system hasn't been activated", True)
            End If

            Log.WriteEventLogTrace("SendEMail:End")
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail
        ''' </summary>
        ''' <param name="rcptName">The name of the receipient</param>
        ''' <param name="rcptAddress">The e-mail address of the receipient</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function SendEMail(ByVal rcptName As String, ByVal rcptAddress As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal attachments As WMSystem.EMailAttachement(), Optional ByVal priority As WMSystem.MailImportance = Nothing, Optional ByVal sensitivity As WMSystem.MailSensitivity = Nothing, Optional ByVal requestTransmissionConfirmation As Boolean = False, Optional ByVal requestReadingConfirmation As Boolean = False, Optional ByVal additionalHeaders As System.Collections.Specialized.NameValueCollection = Nothing, Optional ByVal msgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            Return SendEMail(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, replyToName, replyToAddress, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(attachments), CType(priority, Messaging.EMails.Priority), CType(sensitivity, Messaging.EMails.Sensitivity), requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders, msgCharset, bufErrorDetails)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail
        ''' </summary>
        ''' <param name="rcptName">The name of the receipient</param>
        ''' <param name="rcptAddress">The e-mail address of the receipient</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal rcptName As String, ByVal rcptAddress As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal attachments As Messaging.EMailAttachment(), Optional ByVal priority As Messaging.EMails.Priority = Nothing, Optional ByVal sensitivity As Messaging.EMails.Sensitivity = Nothing, Optional ByVal requestTransmissionConfirmation As Boolean = False, Optional ByVal requestReadingConfirmation As Boolean = False, Optional ByVal additionalHeaders As System.Collections.Specialized.NameValueCollection = Nothing, Optional ByVal msgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            If additionalHeaders Is Nothing Then additionalHeaders = New System.Collections.Specialized.NameValueCollection
            additionalHeaders("Reply-To") = EMails.CreateReceipientString(replyToName, replyToAddress)
            Return SendEMail(rcptName, rcptAddress, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders, msgCharset, bufErrorDetails)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Cc">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Bcc">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal rcptAddresses_To As String, ByVal rcptAddresses_Cc As String, ByVal RcptAddresses_Bcc As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String) As Boolean
            Return SendEMail(rcptAddresses_To, rcptAddresses_Cc, RcptAddresses_Bcc, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, CType(Nothing, Messaging.EMailAttachment()), Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Cc">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Bcc">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal rcptAddresses_To As String, ByVal rcptAddresses_Cc As String, ByVal RcptAddresses_Bcc As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHtmlBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String) As Boolean
            Return SendEMail(rcptAddresses_To, rcptAddresses_Cc, RcptAddresses_Bcc, msgSubject, msgTextBody, msgHtmlBody, senderName, senderAddress, replyToName, replyToAddress, Nothing, Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Cc">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Bcc">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal Attachments As Messaging.EMailAttachment(), Optional ByVal Priority As Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            Return SendEMail(True, RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders, MsgCharset, bufErrorDetails)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Cc">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Bcc">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function SendEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal Attachments As WMSystem.EMailAttachement(), Optional ByVal Priority As WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            Return SendEMail(True, RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders, MsgCharset, bufErrorDetails)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Cc">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Bcc">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SendEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal Attachments As Messaging.EMailAttachment(), Optional ByVal Priority As Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            If AdditionalHeaders Is Nothing Then
                AdditionalHeaders = New System.Collections.Specialized.NameValueCollection
            End If
            If replyToAddress <> Nothing Then
                AdditionalHeaders("Reply-To") = EMails.CreateReceipientString(replyToName, replyToAddress)
            ElseIf replyToName <> Nothing Then
                'Error situation since name exists but no address
                Throw New ArgumentException("replyToAddress must be set when replyToName is set")
            Else
                'No information regarding a reply-to --> do nothing
            End If
            Return SendEMail(True, RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders, MsgCharset, bufErrorDetails)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients
        ''' </summary>
        ''' <param name="allowQueuing">Is the queuing mechanism allowed (a queue item should never get queued again)</param>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Cc">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_Bcc">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="msgSubject">A subject for the new e-mail</param>
        ''' <param name="msgTextBody">The message text as plain text</param>
        ''' <param name="msgHtmlBody">The message text as HTML code</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="attachments">An array of optional attachments</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="requestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="additionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="msgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function SendEMail(ByVal allowQueuing As Boolean, ByVal rcptAddresses_To As String, ByVal rcptAddresses_Cc As String, ByVal rcptAddresses_Bcc As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal attachments As Messaging.EMailAttachment(), Optional ByVal priority As Messaging.EMails.Priority = Nothing, Optional ByVal sensitivity As Messaging.EMails.Sensitivity = Nothing, Optional ByVal requestTransmissionConfirmation As Boolean = False, Optional ByVal requestReadingConfirmation As Boolean = False, Optional ByVal additionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal msgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing) As Boolean
            Log.WriteEventLogTrace("SendEMail:Begin")
            Dim Result As Boolean

            If rcptAddresses_To = Nothing AndAlso rcptAddresses_Cc = Nothing AndAlso rcptAddresses_Bcc = Nothing Then
                Try
                    Throw New ArgumentNullException("rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc")
                Catch ex As Exception
                    bufErrorDetails = ex.ToString
                    Return False
                End Try
            End If

            'Auto-complete sender information
            If SenderAddress = "" Then
                SenderAddress = _WebManager.StandardEMailAccountAddress
                SenderName = _WebManager.StandardEMailAccountName
            ElseIf SenderName = "" Then
                SenderName = SenderAddress
            End If

            If msgCharset = "" Then
                msgCharset = "UTF-8"
            End If

            If allowQueuing AndAlso Configuration.ProcessMailQueue <> WMSystem.TripleState.False AndAlso (_MailSendingSystem = Messaging.EMails.MailSendingSystem.Queue OrElse (_MailSendingSystem = Messaging.EMails.MailSendingSystem.Auto AndAlso _WebManager.IsSupported.Messaging.eMailQueue)) Then

                'Store the e-mail into the mail queue
                Return SendViaQueue(rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, msgCharset, Nothing, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)

            ElseIf _WebManager.IsSupported.Messaging.eMail Then

                'Auto detection analyses the environment and updates the default mail sending system
                If _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.Auto Then
                    _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.NetFramework
                End If

                'Now, delegate the call to the corresponding mail method
                If _MailSendingSystem = CompuMaster.camm.WebManager.Messaging.EMails.MailSendingSystem.EasyMail Then
                    Result = System_SendEMail_MultipleRcpts_NetFramework(rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.NetFramework Then
                    Result = System_SendEMail_MultipleRcpts_NetFramework(rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.NetFramework1 Then
                    Result = System_SendEMail_MultipleRcpts_NetFramework1(rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.ChilkatActiveX Then
                    'Delegate to System.Net.Mail
                    Result = System_SendEMail_MultipleRcpts_NetFramework(rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                ElseIf _MailSendingSystem = WMSystem.MailSendingSystem.ChilkatNet Then
                    'Delegate to System.Net.Mail
                    Result = System_SendEMail_MultipleRcpts_NetFramework(rcptAddresses_To, rcptAddresses_Cc, rcptAddresses_Bcc, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, msgCharset, bufErrorDetails, attachments, priority, sensitivity, requestTransmissionConfirmation, requestReadingConfirmation, additionalHeaders)
                Else
                    Throw New NotImplementedException("Not yet implemented: unknown mail component """ & _MailSendingSystem & """")
                End If
            Else
                _WebManager.Log.RuntimeException("Mail system hasn't been activated", True)
            End If
            Log.WriteEventLogTrace("SendEMail:End")
            Return Result
        End Function

#Region "QueueEMail"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String) As Boolean
            Return QueueEMail(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String) As Boolean
            Return QueueEMail(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, replyToName, replyToAddress, CType(Nothing, Messaging.EMailAttachment()), Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function QueueEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.WMSystem.EMailAttachement(), Optional ByVal Priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean

            'Auto-complete sender information
            If SenderAddress = "" Then
                SenderAddress = _WebManager.StandardEMailAccountAddress
                SenderName = _WebManager.StandardEMailAccountName
            ElseIf SenderName = "" Then
                SenderName = SenderAddress
            End If

            If MsgCharset = "" Then
                MsgCharset = "UTF-8"
            End If

            'Store the e-mail into the mail queue
            Return SendViaQueue(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(Sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)

        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	17.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment(), Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean
            'Auto-complete sender information
            If SenderAddress = "" Then
                SenderAddress = _WebManager.StandardEMailAccountAddress
                SenderName = _WebManager.StandardEMailAccountName
            ElseIf SenderName = "" Then
                SenderName = SenderAddress
            End If

            If MsgCharset = "" Then
                MsgCharset = "UTF-8"
            End If

            'Store the e-mail into the mail queue
            Return SendViaQueue(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete(), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function QueueEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.WMSystem.EMailAttachement(), Optional ByVal Priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean
            If AdditionalHeaders Is Nothing Then
                AdditionalHeaders = New System.Collections.Specialized.NameValueCollection
            End If
            If replyToAddress <> Nothing Then
                AdditionalHeaders("Reply-To") = EMails.CreateReceipientString(replyToName, replyToAddress)
            ElseIf replyToName <> Nothing Then
                'Error situation since name exists but no address
                Throw New ArgumentException("replyToAddress must be set when replyToName is set")
            Else
                'No information regarding a reply-to --> do nothing
            End If
            'Store the e-mail into the mail queue
            Return SendViaQueue(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(Sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">The name of the receipient</param>
        ''' <param name="RcptAddress">The e-mail address of the receipient</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	17.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment(), Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean
            If AdditionalHeaders Is Nothing Then
                AdditionalHeaders = New System.Collections.Specialized.NameValueCollection
            End If
            If replyToAddress <> Nothing Then
                AdditionalHeaders("Reply-To") = EMails.CreateReceipientString(replyToName, replyToAddress)
            ElseIf replyToName <> Nothing Then
                'Error situation since name exists but no address
                Throw New ArgumentException("replyToAddress must be set when replyToName is set")
            Else
                'No information regarding a reply-to --> do nothing
            End If
            'Store the e-mail into the mail queue
            Return SendViaQueue(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String) As Boolean
            Dim attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing
            Return QueueEMail(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, attachments, Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String) As Boolean
            Dim attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing
            Return QueueEMail(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, replyToName, replyToAddress, attachments, Nothing, Nothing, False, False, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <param name="connection">An open connection to the camm Web-Manager database which shall be used</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.WMSystem.EMailAttachement(), Optional ByVal Priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean

            'Auto-complete sender information
            If SenderAddress = "" Then
                SenderAddress = _WebManager.StandardEMailAccountAddress
                SenderName = _WebManager.StandardEMailAccountName
            ElseIf SenderName = "" Then
                SenderName = SenderAddress
            End If

            If MsgCharset = "" Then
                MsgCharset = "UTF-8"
            End If

            'Store the e-mail into the mail queue
            Return SendViaQueue(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(Sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <param name="connection">An open connection to the camm Web-Manager database which shall be used</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment(), Optional ByVal Priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean

            'Auto-complete sender information
            If SenderAddress = "" Then
                SenderAddress = _WebManager.StandardEMailAccountAddress
                SenderName = _WebManager.StandardEMailAccountName
            ElseIf SenderName = "" Then
                SenderName = SenderAddress
            End If

            If MsgCharset = "" Then
                MsgCharset = "UTF-8"
            End If

            'Store the e-mail into the mail queue
            Return SendViaQueue(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, Attachments, CType(Priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(Sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <param name="connection">An open connection to the camm Web-Manager database which shall be used</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.WMSystem.EMailAttachement(), Optional ByVal Priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean
            If AdditionalHeaders Is Nothing Then
                AdditionalHeaders = New System.Collections.Specialized.NameValueCollection
            End If
            If replyToAddress <> Nothing Then
                AdditionalHeaders("Reply-To") = EMails.CreateReceipientString(replyToName, replyToAddress)
            ElseIf replyToName <> Nothing Then
                'Error situation since name exists but no address
                Throw New ArgumentException("replyToAddress must be set when replyToName is set")
            Else
                'No information regarding a reply-to --> do nothing
            End If
            Return SendViaQueue(RcptAddresses_To, RcptAddresses_CC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(Attachments), CType(Priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(Sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="RcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="RcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">A subject for the new e-mail</param>
        ''' <param name="MsgTextBody">The message text as plain text</param>
        ''' <param name="MsgHTMLBody">The message text as HTML code</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="replyToName">The name of the person who should receive the reply</param>
        ''' <param name="replyToAddress">The e-mail address of the person who should receive the reply</param>
        ''' <param name="Attachments">An array of optional attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Ask for a confirmation notice for the successfull transmission</param>
        ''' <param name="RequestReadingConfirmation">Ask for a confirmation notice when the message has been read by the receipient</param>
        ''' <param name="AdditionalHeaders">A collection of optinally additional e-mail headers</param>
        ''' <param name="MsgCharset">IF empty it's UTF-8, there shouldn't be a need for changing this</param>
        ''' <param name="bufErrorDetails">A pointer to a variable where to write additional error details to</param>
        ''' <param name="connection">An open connection to the camm Web-Manager database which shall be used</param>
        ''' <returns>True if the transmission of the e-mail was successfull</returns>
        ''' <remarks>
        '''     A result of true means that there haven't been detected any errors while sending. But this doesn't mean that there are no errors while transmission (SMTP server can't route e-mails, receipient's address doesn't exist, etc.)
        ''' </remarks>
        ''' <seealso>CreateReceipientString</seealso>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function QueueEMail(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal replyToName As String, ByVal replyToAddress As String, ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment(), Optional ByVal Priority As CompuMaster.camm.WebManager.WMSystem.MailImportance = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.WMSystem.MailSensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal connection As IDbConnection = Nothing) As Boolean
            If AdditionalHeaders Is Nothing Then
                AdditionalHeaders = New System.Collections.Specialized.NameValueCollection
            End If
            If replyToAddress <> Nothing Then
                AdditionalHeaders("Reply-To") = EMails.CreateReceipientString(replyToName, replyToAddress)
            ElseIf replyToName <> Nothing Then
                'Error situation since name exists but no address
                Throw New ArgumentException("replyToAddress must be set when replyToName is set")
            Else
                'No information regarding a reply-to --> do nothing
            End If
            Return SendViaQueue(RcptAddresses_To, RcptAddresses_CC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, Attachments, CType(Priority, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CType(Sensitivity, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

#End Region

#End Region
#End Region

#Region "Utils"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create a valid receipient string for the address lists parameters of method SendEMail
        ''' </summary>
        ''' <param name="name">The name of the receipient</param>
        ''' <param name="address">The e-mail address of the receipient</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     <para>RFC-821 standard describes as following:</para>
        '''	    <para>&lt;special&gt; ::= "&lt;" | "&gt;" | "(" | ")" | "[" | "]" | "\" | "."
        '''	              | "," | ";" | ":" | "@"  """ | the control
        '''	              characters (ASCII codes 0 through 31 inclusive and
        '''	              127)</para>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateReceipientString(ByVal name As String, ByVal address As String) As String
            If name <> "" Then
                name = name.Replace("\", "\\")
                name = name.Replace("<", "\<").Replace(">", "\>").Replace("(", "\(").Replace(")", "\)").Replace("[", "\[").Replace("]", "\]")
                name = name.Replace(".", "\.").Replace(",", "\,").Replace(":", "\:").Replace(";", "\;")
                name = name.Replace("@", "\@")
                name = name.Replace("""", "'") 'quotations marks lead to bugs in Chilkat (or at least there)
                'Replace control characters
                For MyCounter As Integer = 0 To 31
                    name = name.Replace(ChrW(MyCounter), " "c)
                Next
                name = name.Replace(ChrW(127), " "c)
            End If
            Return CType(IIf(name <> "", name & " ", ""), String) & "<" & address & ">"
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create a valid receipient string for the address lists parameters of method SendEMail
        ''' </summary>
        ''' <param name="address">The e-mail address of the receipient</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateReceipientString(ByVal address As String) As String
            Return CreateReceipientString(Nothing, address)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extract e-mail addresses from a receipients list constructed by method CreateReceipientString or manually
        ''' </summary>
        ''' <param name="receipientsCommaSeparated"></param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Valid parameter values are e. g. 
        ''' <list>
        ''' <item>l?nc\,\.\:\;\@\[\]\(\)ma9\'\\\&lt;\&gt;kljkj &lt;compumaster@web.de&gt;, her\,ma9\'\\\&lt;\&gt;kljkj &lt;jwezel@compumaster.de&gt;</item>
        ''' <item>&lt;compumaster@web.de&gt;,&lt;jwezel@compumaster.de&gt;</item>
        ''' <item>compumaster@web.de,jwezel@compumaster.de</item>
        ''' </list>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' ----------------------------------------------------------------------------- 
        Private Shared Function SplitEMailAddressesFromReceipientsList(ByVal receipientsCommaSeparated As String) As String()
            Dim Result As String() = Utils.SplitString(receipientsCommaSeparated, ","c, "\"c)
            For MyCounter As Integer = 0 To Result.Length - 1
                'Now we got strings like
                '"l?nc\,\.\:\;\@\[\]\(\)ma9\'\\\<\>kljkj <compumaster@web.de>"
                '" her\,ma9\'\\\<\>kljkj <jwezel@compumaster.de>"
                '" ""Jochen"" <jwezel@compumaster.de>" <------------TOCHECK!
                '" <compumaster@web.de>"
                '"<jwezel@compumaster.de>" 
                '"compumaster@web.de"
                '"jwezel@compumaster.de"
                Dim AddressStart As Integer = Result(MyCounter).LastIndexOf("<")
                If AddressStart > 0 Then
                    Result(MyCounter) = Mid(Result(MyCounter), AddressStart)
                End If
                Result(MyCounter) = Trim(Result(MyCounter))
                If InStr(Result(MyCounter), "@") < 2 OrElse InStr(Result(MyCounter), "@") > Result(MyCounter).Length - 1 Then
                    Result(MyCounter) = ""
                End If
                'Now we got strings like
                '"<compumaster@web.de>"
                '"<jwezel@compumaster.de>"
                '"<compumaster@web.de>"
                '"<jwezel@compumaster.de>" 
                '"compumaster@web.de"
                '"jwezel@compumaster.de"
                If Left(Result(MyCounter), 1) = "<" AndAlso Right(Result(MyCounter), 1) = ">" Then
                    Result(MyCounter) = Mid(Result(MyCounter), 2, Len(Result(MyCounter)) - 2)
                End If
            Next
            Return Result
        End Function

        ''' <summary>
        ''' Split an e-mail address in typical encoding for SMTP protocol into the two parts e-mail address and receipient name
        ''' </summary>
        ''' <param name="emailAddressInSmtpFormat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function SplitEMailAddressesIntoAddressParts(ByVal emailAddressInSmtpFormat As String) As EMailReceipient
            Dim Receipient As New EMailReceipient

            If Not emailAddressInSmtpFormat.LastIndexOf("<") = -1 Then
                Receipient.Address = emailAddressInSmtpFormat.Substring(emailAddressInSmtpFormat.LastIndexOf("<"), emailAddressInSmtpFormat.Length - emailAddressInSmtpFormat.LastIndexOf("<"))
                Receipient.Address = Receipient.Address.Replace("<", "").Replace(">", "")
            Else
                Receipient.Address = emailAddressInSmtpFormat
            End If

            If emailAddressInSmtpFormat.LastIndexOf(" ") > 0 AndAlso emailAddressInSmtpFormat.LastIndexOf(" ") = emailAddressInSmtpFormat.LastIndexOf("<") - 1 Then
                Receipient.Name = emailAddressInSmtpFormat.Substring(0, emailAddressInSmtpFormat.LastIndexOf(" "))
                Receipient.Name = Receipient.Name.Replace("<", "").Replace(">", "")
            Else
                Receipient.Name = String.Empty
            End If

            Return Receipient
        End Function

        ''' <summary>
        ''' Split an e-mail address list in typical encoding for SMTP protocol into the two parts e-mail address and receipient name
        ''' </summary>
        ''' <param name="emailAddressesInSmtpFormat"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function SplitEMailAddressesIntoEMailRecipientsFromReceipientsList(ByVal emailAddressesInSmtpFormat As String) As EMailReceipient()
            Dim Result As String() = Utils.SplitString(emailAddressesInSmtpFormat, ","c, "\"c)
            Dim EmailReceipient(Result.Length - 1) As EMailReceipient
            For MyCounter As Integer = 0 To Result.Length - 1
                EmailReceipient(MyCounter) = SplitEMailAddressesIntoAddressParts(Result(MyCounter))
            Next
            Return EmailReceipient
        End Function

        Friend Shared Function ConvertWMSystemEMailAttachmentToMessagingEMailAttachment(ByVal attachments As WMSystem.EMailAttachement()) As Messaging.EMailAttachment()
            If attachments Is Nothing Then
                Return Nothing
            Else
                Dim Result As New ArrayList
                For MyCounter As Integer = 0 To attachments.Length - 1
                    Dim Attachment As New Messaging.EMailAttachment
                    Attachment.FilePath = attachments(MyCounter).AttachmentFile
                    Attachment.PlaceholderInMhtmlToBeReplacedByContentID = attachments(MyCounter).PlaceholderInMHTML_ToReplaceWithCID
                    Attachment.RawData = attachments(MyCounter).AttachmentData
                    Attachment.RawDataFilename = attachments(MyCounter).AttachmentData_Filename
                    Result.Add(Attachment)
                Next
                Return CType(Result.ToArray(GetType(Messaging.EMailAttachment)), Messaging.EMailAttachment())
            End If
        End Function

        Private Class EMailReceipient
            Public Sub New()
            End Sub
            Public Sub New(ByVal name As String, ByVal address As String)
                If address Is Nothing Then
                    Throw New ArgumentNullException("address")
                End If
                Me.Name = name
                Me.Address = address
            End Sub

            Private _Name As String
            Private _Address As String

            Public Property Address() As String
                Get
                    Return _Address
                End Get
                Set(ByVal Value As String)
                    If Value Is Nothing Then
                        Throw New ArgumentNullException("Address")
                    End If
                    _Address = Value
                End Set
            End Property
            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property
        End Class
#End Region

        ''' <summary>
        ''' Replace all Content-IDs by encoding-independent Content-IDs
        ''' </summary>
        ''' <param name="htmlBody">The e-mail HTML body</param>
        ''' <param name="attachments">The attachments of the e-mail</param>
        Private Sub FixHtmlContentIDs(ByRef htmlBody As String, ByRef attachments As EMailAttachment())
            If Not attachments Is Nothing Then
                For MyCounter As Integer = 0 To attachments.Length - 1
                    If attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID <> Nothing Then
                        'Ensure working content-ID in all encodings
                        'If suggested Content-ID is not possible for usage without a working encoding (at least it's unknown how to do it, currently), then replace the Content-ID by another value and also do this replacement in the html message part
                        Dim newContentIDName As String = Guid.NewGuid.ToString
                        'Replace exact matches
                        htmlBody = Utils.ReplaceString(htmlBody, attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID, newContentIDName, Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase)
                        'Replace html- and url-encoded matches
                        htmlBody = Utils.ReplaceString(htmlBody, System.Web.HttpUtility.UrlEncode(attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID), newContentIDName, Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase)
                        htmlBody = Utils.ReplaceString(htmlBody, System.Web.HttpUtility.HtmlEncode(attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID), newContentIDName, Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase)
                        htmlBody = Utils.ReplaceString(htmlBody, System.Web.HttpUtility.HtmlEncode(System.Web.HttpUtility.UrlEncode(attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID)), newContentIDName, Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase)
                        attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID = newContentIDName
                    End If
                Next

                'remove attachments with missing references
                Dim atts As New ArrayList(attachments)
                For MyCounter As Integer = atts.Count - 1 To 0 Step -1
                    If CType(atts(MyCounter), EMailAttachment).PlaceholderInMhtmlToBeReplacedByContentID <> Nothing AndAlso htmlBody.IndexOf(CType(atts(MyCounter), EMailAttachment).PlaceholderInMhtmlToBeReplacedByContentID) = -1 Then
                        'html doesn't contain any reference to the embedded object - removing attachment
                        atts.RemoveAt(MyCounter)
                    End If
                Next
                attachments = CType(atts.ToArray(GetType(EMailAttachment)), EMailAttachment())
            End If
        End Sub

#Region "Mail Queue"

        Private QueueTransactionData As ArrayList

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Start a transaction for queuing mails
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub BeginQueueTransaction()
            If Not QueueTransactionData Is Nothing Then
                RollbackQueueTransaction()
            End If
            QueueTransactionData = New ArrayList
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Commit all changes in the transaction
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CommitQueueTransaction()
            CommitQueueTransaction(Nothing)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Commit all changes in the transaction
        ''' </summary>
        ''' <param name="connection">An open connection to the camm Web-Manager database</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CommitQueueTransaction(ByVal connection As IDbConnection)

            Dim ReUseDBConnection As Boolean = True
            If connection Is Nothing Then
                ReUseDBConnection = False
                connection = New SqlConnection(_WebManager.ConnectionString)
            End If

            Dim MyCmd As New SqlCommand
            MyCmd.Connection = CType(connection, SqlConnection)
            MyCmd.CommandText = _
                    "CREATE TABLE #mqueuetransids (ID int)" & vbNewLine & _
                    Me.QueueTransactionSqlRelatedIDs & vbNewLine & _
                    "UPDATE dbo.Log_eMailMessages SET State = " & CType(QueueMonitoring.QueueStates.Queued, Byte).ToString & " FROM dbo.Log_eMailMessages INNER JOIN #mqueuetransids ON dbo.Log_eMailMessages.ID = #mqueuetransids.ID" & vbNewLine & _
                    "DROP TABLE #mqueuetransids"
            MyCmd.CommandType = CommandType.Text

            If ReUseDBConnection = True Then
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
            Else
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Roll back all changes in the transaction
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RollbackQueueTransaction()
            RollbackQueueTransaction(Nothing)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Roll back all changes in the transaction
        ''' </summary>
        ''' <param name="connection">An open connection to the camm Web-Manager database</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RollbackQueueTransaction(ByVal connection As IDbConnection)

            Dim ReUseDBConnection As Boolean = True
            If connection Is Nothing Then
                ReUseDBConnection = False
                connection = New SqlConnection(_WebManager.ConnectionString)
            End If

            Dim MyCmd As New SqlCommand
            MyCmd.Connection = CType(connection, SqlConnection)
            MyCmd.CommandText = _
                    "CREATE TABLE #mqueuetransids (ID int)" & vbNewLine & _
                    Me.QueueTransactionSqlRelatedIDs & vbNewLine & _
                    "DELETE dbo.Log_eMailMessages FROM dbo.Log_eMailMessages INNER JOIN #mqueuetransids ON dbo.Log_eMailMessages.ID = #mqueuetransids.ID" & vbNewLine & _
                    "DROP TABLE #mqueuetransids"
            MyCmd.CommandType = CommandType.Text
            If ReUseDBConnection = True Then
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
            Else
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return a piece of sql code for accessing the related IDs to be used in commit or rollback commands
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function QueueTransactionSqlRelatedIDs() As String
            Dim Result As New System.Text.StringBuilder
            For MyCounter As Integer = 0 To Me.QueueTransactionData.Count - 1
                Result.Append("INSERT INTO #mqueuetransids (ID) VALUES (")
                Result.Append(CType(Me.QueueTransactionData(MyCounter), Integer).ToString)
                Result.Append(")")
                Result.Append(vbNewLine)
            Next
            Return Result.ToString
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Send an e-mail to the mail queue
        ''' </summary>
        ''' <param name="RcptName">Receipient's name</param>
        ''' <param name="RcptAddress">Receipient's e-mail address</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="connection">An open connection to the camm Web-Manager database which shall be used</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SendViaQueue(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, ByVal MsgCharset As String, ByVal connection As IDbConnection, ByRef bufErrorDetails As String, ByVal Attachments() As CompuMaster.camm.WebManager.Messaging.EMailAttachment, ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority, ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity, ByVal RequestTransmissionConfirmation As Boolean, ByVal RequestReadingConfirmation As Boolean, ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection) As Boolean
            Return SendViaQueue(CreateReceipientString(RcptName, RcptAddress), Nothing, Nothing, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, connection, bufErrorDetails, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends an e-mail to multiple receipients via the mail queue
        ''' </summary>
        ''' <param name="rcptAddresses_To">The receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_CC">The copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="rcptAddresses_BCC">The blind copy receipients comma-separated with format "Complete Name &lt;somebody@yourcompany.com&gt;" or only a simple e-mail address in the format "somebody@yourcompany.com"</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="msgTextBody">Message plain text body</param>
        ''' <param name="msgHTMLBody">Message html text body</param>
        ''' <param name="senderName">The name of the sender</param>
        ''' <param name="senderAddress">The e-mail address of the sender</param>
        ''' <param name="msgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="connection">An open connection to the camm Web-Manager database which shall be used</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="attachments">Array of e-mail attachments</param>
        ''' <param name="priority">The priority of the e-mail</param>
        ''' <param name="sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="requestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="requestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="additionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SendViaQueue(ByVal rcptAddresses_To As String, ByVal rcptAddresses_CC As String, ByVal rcptAddresses_BCC As String, ByVal msgSubject As String, ByVal msgTextBody As String, ByVal msgHTMLBody As String, ByVal senderName As String, ByVal senderAddress As String, ByVal msgCharset As String, ByVal connection As IDbConnection, ByRef bufErrorDetails As String, ByVal attachments() As Messaging.EMailAttachment, ByVal priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority, ByVal sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity, ByVal requestTransmissionConfirmation As Boolean, ByVal requestReadingConfirmation As Boolean, ByVal additionalHeaders As Collections.Specialized.NameValueCollection) As Boolean
            Try
                FixHtmlContentIDs(msgHTMLBody, attachments)

                'Setup additional headers
                If additionalHeaders Is Nothing Then
                    additionalHeaders = New System.Collections.Specialized.NameValueCollection
                End If

                'Setup mail priority
                If Not priority = Nothing Then
                    Select Case priority
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low
                        Case CType(4, CompuMaster.camm.WebManager.Messaging.EMails.Priority) 'Between Normal and Low
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.High
                        Case CType(2, CompuMaster.camm.WebManager.Messaging.EMails.Priority) 'Between Normal and High
                        Case Else 'CompuMaster.camm.WebManager.WMSystem.MailImportance.Normal
                            priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.Normal
                    End Select
                    additionalHeaders("X-Priority") = CByte(priority).ToString()
                    If priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.High OrElse priority = CompuMaster.camm.WebManager.WMSystem.MailImportance.Normal OrElse priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low Then
                        additionalHeaders("X-MSMail-Priority") = priority.ToString()
                        additionalHeaders("Importance") = priority.ToString()
                    End If
                End If

                'Setup mail sensitivity
                If Not sensitivity = Nothing Then
                    Select Case sensitivity
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_CompanyConfidential
                            additionalHeaders("Sensitivity") = "Company-Confidential"
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal
                            additionalHeaders("Sensitivity") = "Personal"
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Private
                            additionalHeaders("Sensitivity") = "Private"
                    End Select
                End If

                'Setup mail receipt confirmations
                If requestReadingConfirmation Then
                    additionalHeaders("Disposition-Notification-To") = senderAddress
                End If
                If requestTransmissionConfirmation Then
                    additionalHeaders("Return-Receipt-To") = """" & senderName & """ <" & senderAddress & ">"
                End If

                'Collect mail data
                Dim MailData As New DataSet("root")

                'Version information
                Dim VersionData As DataTable
                Dim VersionDataHashtable As New Hashtable
                VersionDataHashtable.Add("NetLibrary", _WebManager.System_Version_Ex.ToString)
                VersionData = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertIDictionaryToDataTable(VersionDataHashtable)
                VersionData.TableName = "version"
                MailData.Tables.Add(VersionData)

                'Headers
                Dim Headers As DataTable
                Headers = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertNameValueCollectionToDataTable(additionalHeaders)
                Headers.TableName = "headers"
                MailData.Tables.Add(Headers)

                'Attachments
                Dim AttachedFiles As New DataTable("attachments")
                AttachedFiles.Columns.Add("Placeholder", GetType(String))
                AttachedFiles.Columns.Add("FileName", GetType(String))
                AttachedFiles.Columns.Add("FileData", GetType(Byte()))
                AttachedFiles.Columns.Add("OriginFileNameBeforePlaceholderValue", GetType(String)) 'to allow an attachment with only content-id in filename to be saved to disc with correct file type again (e.g. for mail queue monitor e-mail preview dialog)
                If Not attachments Is Nothing Then
                    For MyCounter As Integer = 0 To attachments.Length - 1
                        Dim MyAttachment As CompuMaster.camm.WebManager.Messaging.EMailAttachment = Nothing

                        If attachments(MyCounter).FilePath <> "" AndAlso Not System.IO.File.Exists(attachments(MyCounter).FilePath) Then
                            bufErrorDetails = "Attachment file not found: " & attachments(MyCounter).FilePath
                            Return False
                        ElseIf (attachments(MyCounter).RawData Is Nothing OrElse attachments(MyCounter).RawDataFilename = Nothing) AndAlso attachments(MyCounter).FilePath <> "" AndAlso System.IO.File.Exists(attachments(MyCounter).FilePath) Then
                            'Load file system data into memory as raw binary data
                            Dim fs As System.IO.FileStream = New System.IO.FileStream(attachments(MyCounter).FilePath, IO.FileMode.Open)
                            Try
                                Dim fi As IO.FileInfo = New IO.FileInfo(attachments(MyCounter).FilePath)
                                Dim byteArr(CType(fi.Length, Integer) - 1) As Byte
                                fs.Read(byteArr, 0, CType(fi.Length, Integer))
                                attachments(MyCounter).RawData = byteArr
                                attachments(MyCounter).RawDataFilename = IO.Path.GetFileName(attachments(MyCounter).FilePath)
                                attachments(MyCounter).FilePath = Nothing
                            Catch ex As Exception
                                bufErrorDetails = ex.ToString
                                Return False
                            Finally
                                fs.Close()
                            End Try
                        ElseIf Not attachments(MyCounter).RawData Is Nothing AndAlso attachments(MyCounter).RawDataFilename <> Nothing Then
                            'Just add the binary data
                        Else
                            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                            Dim WorkaroundEx As New Exception("")
                            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                            Try
                                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                            Catch
                            End Try
                            _WebManager.Log.RuntimeWarning("Empty or invalid email attachment", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)
                            bufErrorDetails = "Empty or invalid email attachment, see camm Web-Manager warnings log for full stack trace"
                            Return False
                        End If

                        'Add the binary data
                        MyAttachment = attachments(MyCounter)
                        Dim MyRow As DataRow = AttachedFiles.NewRow
                        MyRow("FileData") = Utils.ObjectNotNothingOrDBNull(MyAttachment.RawData)
                        MyRow("FileName") = Utils.StringNotNothingOrDBNull(MyAttachment.RawDataFilename)
                        MyRow("Placeholder") = Utils.StringNotNothingOrDBNull(MyAttachment.PlaceholderInMhtmlToBeReplacedByContentID)
                        If MyAttachment.PlaceholderInMhtmlToBeReplacedByContentID <> Nothing Then
                            'Required to drop the origin filename if we want to use a Content-ID
                            MyRow("OriginFileNameBeforePlaceholderValue") = MyRow("FileName")
                            MyRow("FileName") = Utils.StringNotNothingOrDBNull(MyAttachment.PlaceholderInMhtmlToBeReplacedByContentID)
                        End If
                        AttachedFiles.Rows.Add(MyRow)
                    Next
                End If
                MailData.Tables.Add(AttachedFiles)

                'Basic message data
                If _WebManager.System_DebugLevel >= WMSystem.DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper Then
                    rcptAddresses_To = EMails.CreateReceipientString(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress)
                    rcptAddresses_CC = Nothing
                    rcptAddresses_BCC = Nothing
                    If msgTextBody <> "" Then 'text body present
                        msgTextBody = "The server currently runs in debug mode. All e-mails will be sent to your address only." & ControlChars.CrLf & ControlChars.CrLf & ControlChars.CrLf & msgTextBody
                    End If
                    If msgHTMLBody <> "" Then 'HTML body present
                        msgHTMLBody = "<h4><font face=""Arial"" color=""red""><b><em>The server currently runs in debug mode. All e-mails will be sent to your address only.</em></b></font></h4>" & msgHTMLBody
                    End If
                End If
                Dim MessageData As DataTable
                Dim MessageDataCollection As New Collections.Specialized.NameValueCollection
                MessageDataCollection.Add("FromAddress", senderAddress)
                MessageDataCollection.Add("FromName", senderName)
                MessageDataCollection.Add("To", rcptAddresses_To)
                MessageDataCollection.Add("Cc", rcptAddresses_CC)
                MessageDataCollection.Add("Bcc", rcptAddresses_BCC)
                MessageDataCollection.Add("Subject", msgSubject)
                MessageDataCollection.Add("Charset", msgCharset)
                MessageDataCollection.Add("TextBody", msgTextBody)
                MessageDataCollection.Add("HtmlBody", msgHTMLBody)
                MessageData = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertNameValueCollectionToDataTable(MessageDataCollection)
                MessageData.TableName = "message"
                MailData.Tables.Add(MessageData)

                'Add the mail to the queue
                Dim MyConn As IDbConnection
                Dim ReUseExistingConnection As Boolean
                If connection Is Nothing Then
                    MyConn = New SqlConnection(_WebManager.ConnectionString)
                    ReUseExistingConnection = False
                Else
                    MyConn = connection
                    ReUseExistingConnection = True
                End If
                Dim MyCmd As New SqlCommand("INSERT INTO [dbo].[Log_eMailMessages]([UserID], [data], [State], [DateTime]) VALUES (@UserID, @Data, @State, GETDATE())" & vbNewLine & "SELECT @ID = @@IDENTITY", CType(MyConn, SqlConnection))
                MyCmd.Parameters.Add("@ID", SqlDbType.Int)
                MyCmd.Parameters("@ID").Direction = ParameterDirection.Output
                If System.Web.HttpContext.Current Is Nothing OrElse System.Web.HttpContext.Current.Session Is Nothing Then
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(WMSystem.SpecialUsers.User_Code)
                Else
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                End If
                MyCmd.Parameters.Add("@Data", SqlDbType.NText).Value = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertDatasetToXml(MailData)
                If Me.QueueTransactionData Is Nothing Then
                    'Send to current queue immediately
                    MyCmd.Parameters.Add("@State", SqlDbType.TinyInt).Value = CompuMaster.camm.WebManager.Messaging.QueueMonitoring.QueueStates.Queued
                Else
                    'Add to queue table, but wait for a commit before real queueing starts
                    MyCmd.Parameters.Add("@State", SqlDbType.TinyInt).Value = CompuMaster.camm.WebManager.Messaging.QueueMonitoring.QueueStates.WaitingForReleaseBeforeQueuing
                End If

                'Execute the command
                If ReUseExistingConnection Then
                    'Use open connection, never close it
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                Else
                    'Open and close connection as required
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                End If
                'Add ID to collection for transaction handling
                If Not Me.QueueTransactionData Is Nothing Then
                    Me.QueueTransactionData.Add(Utils.Nz(MyCmd.Parameters("@ID").Value, 0%))
                End If
                Return True
            Catch ex As Exception
                _WebManager.Log.RuntimeWarning("Error sending email via queue", ex.ToString, WMSystem.DebugLevels.NoDebug, True, False)
#If DEBUG Then
                bufErrorDetails = ex.ToString
#Else
                If _WebManager.DebugLevel >= _WebManager.DebugLevels.Medium_LoggingOfDebugInformation Then
                    bufErrorDetails = ex.ToString
                Else
                    bufErrorDetails = ex.Message
                End If
#End If
                Return False
            End Try

        End Function

#End Region

#Region ".NET 2 or higher framework standard methods"
#If NetFramework <> "1_1" Then
        ''' <summary>
        '''     Send an e-mail via the default e-mail handler of the .NET Framework
        ''' </summary>
        ''' <param name="RcptName">Receipient's name</param>
        ''' <param name="RcptAddress">Receipient's e-mail address</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        '''     Requires the existance of the CDO.Message object on MS platforms, otherwise this method will fail
        '''     This component only allows to send the e-mail either in plain text or html code format
        ''' </remarks>
        Private Function System_SendEMailEx_NetFramework2(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing, Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
            Return System_SendEMail_MultipleRcpts_NetFramework2(EMails.CreateReceipientString(RcptName, RcptAddress), Nothing, Nothing, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, bufErrorDetails, Attachments, CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

        ''' <summary>
        '''     Send an e-mail to multiple receipents via the default e-mail handler of the .NET Framework
        ''' </summary>
        ''' <param name="RcptAddresses_To">TO receipients</param>
        ''' <param name="RcptAddresses_CC">CC receipients</param>
        ''' <param name="RcptAddresses_BCC">BCC receipients</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        ''' </remarks>
        Private Function System_SendEMail_MultipleRcpts_NetFramework2(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing, Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
            Dim Result As Boolean
            Dim MyMail As New System.Net.Mail.MailMessage()
            Dim ErrorFound As String = Nothing
            Dim smtp As New System.Net.Mail.SmtpClient()
            FixHtmlContentIDs(MsgHTMLBody, Attachments)

            'Prepare main mail properties
            smtp.Host = _WebManager.SMTPServerName
            If _WebManager.SMTPServerPort <> 25 Then
                smtp.Port = _WebManager.SMTPServerPort
            End If

            Select Case _WebManager.SMTPAuthType
                Case "LOGIN", "PLAIN"
                    If Me._WebManager.SMTPUserName = Nothing Then Throw New ArgumentNullException("WebManager.SmtpUserName")
                    If Me._WebManager.SMTPPassword = Nothing Then Throw New ArgumentNullException("WebManager.SmtpPassword")
                    smtp.Credentials = New System.Net.NetworkCredential(_WebManager.SMTPUserName, _WebManager.SMTPPassword)
                    smtp.EnableSsl = False
                    smtp.Timeout = 10 * 1000 'in ms
                Case "LOGIN-SSL"
                    If Me._WebManager.SMTPUserName = Nothing Then Throw New ArgumentNullException("WebManager.SmtpUserName")
                    If Me._WebManager.SMTPPassword = Nothing Then Throw New ArgumentNullException("WebManager.SmtpPassword")
                    smtp.UseDefaultCredentials = False
                    smtp.Credentials = New System.Net.NetworkCredential(_WebManager.SMTPUserName, _WebManager.SMTPPassword)
                    smtp.EnableSsl = True
                    smtp.Timeout = 10 * 1000 'in ms
                Case "CRAM-MD5"
                    bufErrorDetails = (New NotSupportedException("CRAM-MD5 is not supported")).ToString
                    Return False
                Case "NTLM"
                    bufErrorDetails = (New NotSupportedException("NTLM is not supported")).ToString
                    Return False
                Case Else
                    'SmtpAuthMethod = "NONE"
            End Select

            Try
                'Add all recepients
                If RcptAddresses_To <> Nothing Then
                    For Each rcpt As EMailReceipient In EMails.SplitEMailAddressesIntoEMailRecipientsFromReceipientsList(RcptAddresses_To)
                        MyMail.To.Add(New System.Net.Mail.MailAddress(rcpt.Address, rcpt.Name))
                    Next
                End If
                If RcptAddresses_CC <> Nothing Then
                    For Each rcpt As EMailReceipient In EMails.SplitEMailAddressesIntoEMailRecipientsFromReceipientsList(RcptAddresses_CC)
                        MyMail.CC.Add(New System.Net.Mail.MailAddress(rcpt.Address, rcpt.Name))
                    Next
                End If
                If RcptAddresses_BCC <> Nothing Then
                    For Each rcpt As EMailReceipient In EMails.SplitEMailAddressesIntoEMailRecipientsFromReceipientsList(RcptAddresses_BCC)
                        MyMail.Bcc.Add(New System.Net.Mail.MailAddress(rcpt.Address, rcpt.Name))
                    Next
                End If
            Catch ex As Exception
                bufErrorDetails = ex.ToString
                Return False
            End Try

            MyMail.Subject = MsgSubject

            Try
                MyMail.SubjectEncoding = System.Text.Encoding.GetEncoding(MsgCharset)
                MyMail.BodyEncoding = System.Text.Encoding.GetEncoding(MsgCharset)
            Catch
                Try
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    _WebManager.Log.RuntimeWarning("Not supported by System.Web.Mail: encoding cannot retrieved from string """ & MsgCharset & """", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)
                Catch
                End Try
            End Try

            Dim plainView As System.Net.Mail.AlternateView = Nothing
            Dim htmlView As System.Net.Mail.AlternateView = Nothing
            If MsgTextBody <> "" Then
                plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(MsgTextBody, Nothing, "text/plain")
            End If
            If MsgHTMLBody <> "" Then
                htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(MsgHTMLBody, Nothing, "text/html")
            End If

            MyMail.From = New System.Net.Mail.MailAddress(SenderAddress, SenderName)

            'Add attachments / related/embedded objects
            Dim TempFiles As New Collections.Generic.List(Of String)
            If Not Attachments Is Nothing Then
                For MyCounter As Integer = 0 To Attachments.Length - 1
                    Dim MyAttachment As System.Net.Mail.Attachment = Nothing
                    Dim MyEmbeddedImg As System.Net.Mail.LinkedResource = Nothing

                    If Not Attachments(MyCounter).RawData Is Nothing OrElse Attachments(MyCounter).RawDataFilename <> Nothing Then
                        Try
                            'Create a temporary file, store the data there and add that file as attachment before removing again
                            Dim tempFile As String = System.IO.Path.GetTempFileName()
                            System.IO.File.Delete(tempFile)
                            TempFiles.Add(tempFile)
                            System.IO.Directory.CreateDirectory(tempFile)
                            tempFile &= System.IO.Path.DirectorySeparatorChar & Attachments(MyCounter).RawDataFilename
                            Dim tempFileStream As System.IO.FileStream = System.IO.File.Create(tempFile)
                            tempFileStream.Write(Attachments(MyCounter).RawData, 0, Attachments(MyCounter).RawData.Length)
                            tempFileStream.Flush()
                            tempFileStream.Close()
                            'TempFiles.Add(tempFile)
                            If Attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID <> Nothing Then
                                MyEmbeddedImg = New System.Net.Mail.LinkedResource(tempFile)
                                MyEmbeddedImg.ContentId = Attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID
                            Else
                                MyAttachment = New System.Net.Mail.Attachment(tempFile)
                            End If
                        Catch ex As Exception
                            ErrorFound = ex.ToString & vbNewLine
                        End Try
                    ElseIf Attachments(MyCounter).FilePath <> "" Then
                        Dim fi As New IO.FileInfo(Attachments(MyCounter).FilePath)
                        If fi.Exists Then
                            Try
                                If Attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID <> Nothing Then
                                    MyEmbeddedImg = New System.Net.Mail.LinkedResource(Attachments(MyCounter).FilePath)
                                    MyEmbeddedImg.ContentId = Attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID
                                Else
                                    MyAttachment = New System.Net.Mail.Attachment(Attachments(MyCounter).FilePath)
                                End If
                            Catch ex As Exception
                                _WebManager.Log.RuntimeException(ex, True, False, WMSystem.DebugLevels.NoDebug)
                                bufErrorDetails = ex.ToString
                                Return False
                            End Try
                        Else
                            bufErrorDetails = "Attachment not found: " & Attachments(MyCounter).FilePath
                            Return False
                        End If
                    Else
                        Try
                            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                            Dim WorkaroundEx As New Exception("")
                            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                            Try
                                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                            Catch
                            End Try
                            _WebManager.Log.RuntimeWarning("Empty or invalid email attachment", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)
                        Catch
                        End Try
                    End If
                    If Not MyEmbeddedImg Is Nothing Then
                        'add the LinkedResource to the appropriate view
                        htmlView.LinkedResources.Add(MyEmbeddedImg)
                    End If
                    If Not MyAttachment Is Nothing Then
                        MyMail.Attachments.Add(MyAttachment)
                    End If
                Next
            End If

            Try
                If plainView IsNot Nothing Then
                    MyMail.AlternateViews.Add(plainView)
                End If
                If htmlView IsNot Nothing Then
                    MyMail.AlternateViews.Add(htmlView)
                End If

                'Setup mail priority
                If Not Priority = Nothing Then
                    Select Case Priority
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low
                        Case CType(4, CompuMaster.camm.WebManager.Messaging.EMails.Priority) 'Between Normal and Low
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.High
                        Case CType(2, CompuMaster.camm.WebManager.Messaging.EMails.Priority) 'Between Normal and High
                        Case Else 'CompuMaster.camm.WebManager.WMSystem.MailImportance.Normal
                            Priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.Normal
                    End Select
                    MyMail.Headers.Add("X-Priority", CByte(Priority).ToString())
                    If Priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.High OrElse Priority = CompuMaster.camm.WebManager.WMSystem.MailImportance.Normal OrElse Priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low Then
                        MyMail.Headers.Add("Importance", Priority.ToString())
                        MyMail.Headers.Add("Priority", Priority.ToString())
                        MyMail.Headers.Add("X-MSMail-Priority", Priority.ToString())
                    End If
                    Select Case Priority
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.High, CType(2, CompuMaster.camm.WebManager.Messaging.EMails.Priority)
                            MyMail.Priority = System.Net.Mail.MailPriority.High
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low, CType(4, CompuMaster.camm.WebManager.Messaging.EMails.Priority)
                            MyMail.Priority = System.Net.Mail.MailPriority.Low
                        Case Else
                            MyMail.Priority = System.Net.Mail.MailPriority.Normal
                    End Select
                End If

                'Setup mail sensitivity
                If Not Sensitivity = Nothing Then
                    Select Case Sensitivity
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_CompanyConfidential
                            MyMail.Headers.Add("Sensitivity", "Company-Confidential")
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal
                            MyMail.Headers.Add("Sensitivity", "Personal")
                        Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Private
                            MyMail.Headers.Add("Sensitivity", "Private")
                    End Select
                End If

                'Setup mail receipt confirmations
                If RequestReadingConfirmation Then
                    'Disposition-Notification-To: "Jochen Wezel" <compumaster@web.de> 'Lesebest?tigung
                    MyMail.Headers.Add("Disposition-Notification-To", SenderAddress)
                End If
                'MIGHT BE FIXED OR IS STILL TODO: The bug is that nothing happens :( _
                If RequestTransmissionConfirmation Then
                    'Return-Receipt-To: "Jochen Wezel" <compumaster@web.de> '?bermittlungsbest?tigung
                    'MyMail.Headers.Add("Return-Receipt-To", SenderAddress)
                    MyMail.DeliveryNotificationOptions = Net.Mail.DeliveryNotificationOptions.OnSuccess
                End If

                'Setup additional headers
                If Not AdditionalHeaders Is Nothing Then
                    For Each MyKey As String In AdditionalHeaders
                        If MyKey.ToLower() = "reply-to" Then
                            Dim rcpt As EMailReceipient = SplitEMailAddressesIntoAddressParts(AdditionalHeaders(MyKey))
                            MyMail.ReplyTo = New System.Net.Mail.MailAddress(rcpt.Address, rcpt.Name)
                        Else
                            MyMail.Headers.Add(MyKey, AdditionalHeaders(MyKey))
                        End If
                    Next
                End If

                'Send the mail and return the success/error status
                If ErrorFound = Nothing Then
                    smtp.Send(MyMail)
                    Result = True
                Else
                    bufErrorDetails = ErrorFound
                    Result = False
                End If
            Catch ex As Exception
                Result = False
#If DEBUG Then
                bufErrorDetails = ex.ToString
#Else
                    If _WebManager.DebugLevel >= _WebManager.DebugLevels.Medium_LoggingOfDebugInformation Then
                        bufErrorDetails = ex.ToString
                    Else
                        bufErrorDetails = ex.Message
                    End If
#End If
            End Try

            'Clean up all temporary files
            For MyCounter As Integer = 0 To TempFiles.Count - 1
                Try
                    System.IO.Directory.Delete(CType(TempFiles(MyCounter), String), True)
                Catch
                End Try
            Next

            Return Result

        End Function
#End If
#End Region

#Region ".NET framework standard methods"
        ''' <summary>
        '''     Send an e-mail via the default e-mail handler of the .NET Framework
        ''' </summary>
        ''' <param name="RcptName">Receipient's name</param>
        ''' <param name="RcptAddress">Receipient's e-mail address</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        '''     Requires the existance of the CDO.Message object on MS platforms, otherwise this method will fail
        '''     This component only allows to send the e-mail either in plain text or html code format
        ''' </remarks>
        Private Function System_SendEMailEx_NetFramework(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing, Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
#If NetFramework <> "1_1" Then
            Return System_SendEMail_MultipleRcpts_NetFramework2(EMails.CreateReceipientString(RcptName, RcptAddress), Nothing, Nothing, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, bufErrorDetails, Attachments, CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
#Else
            Return System_SendEMail_MultipleRcpts_NetFramework1(EMails.CreateReceipientString(RcptName, RcptAddress), Nothing, Nothing, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, bufErrorDetails, Attachments, CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
#End If
        End Function

        ''' <summary>
        '''     Send an e-mail to multiple receipents via the default e-mail handler of the .NET Framework
        ''' </summary>
        ''' <param name="RcptAddresses_To">TO receipients</param>
        ''' <param name="RcptAddresses_CC">CC receipients</param>
        ''' <param name="RcptAddresses_BCC">BCC receipients</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        ''' </remarks>
        Private Function System_SendEMail_MultipleRcpts_NetFramework(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing, Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
#If NetFramework <> "1_1" Then
            Return System_SendEMail_MultipleRcpts_NetFramework2(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, bufErrorDetails, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
#Else
            Return System_SendEMail_MultipleRcpts_NetFramework1(RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, bufErrorDetails, Attachments, Priority, Sensitivity, RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
#End If
        End Function

#End Region
#Region ".NET 1.x framework standard methods"
#If VS2015OrHigher = True Then
#Disable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Send an e-mail via the default e-mail handler of the .NET Framework
        ''' </summary>
        ''' <param name="RcptName">Receipient's name</param>
        ''' <param name="RcptAddress">Receipient's e-mail address</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        '''     Requires the existance of the CDO.Message object on MS platforms, otherwise this method will fail
        '''     This component only allows to send the e-mail either in plain text or html code format
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function System_SendEMailEx_NetFramework1(ByVal RcptName As String, ByVal RcptAddress As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing, Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
            Return System_SendEMail_MultipleRcpts_NetFramework1(EMails.CreateReceipientString(RcptName, RcptAddress), Nothing, Nothing, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset, bufErrorDetails, Attachments, CType(Priority, Messaging.EMails.Priority), CType(Sensitivity, Messaging.EMails.Sensitivity), RequestTransmissionConfirmation, RequestReadingConfirmation, AdditionalHeaders)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Send an e-mail to multiple receipents via the default e-mail handler of the .NET Framework
        ''' </summary>
        ''' <param name="RcptAddresses_To">TO receipients</param>
        ''' <param name="RcptAddresses_CC">CC receipients</param>
        ''' <param name="RcptAddresses_BCC">BCC receipients</param>
        ''' <param name="MsgSubject">Message subject</param>
        ''' <param name="MsgTextBody">Message plain text body</param>
        ''' <param name="MsgHTMLBody">Message html text body</param>
        ''' <param name="SenderName">The name of the sender</param>
        ''' <param name="SenderAddress">The e-mail address of the sender</param>
        ''' <param name="MsgCharset">The charset of the message, default is UTF-8</param>
        ''' <param name="bufErrorDetails">A reference to a string variable which shall contain error information when an exception gets fired</param>
        ''' <param name="Attachments">Array of e-mail attachments</param>
        ''' <param name="Priority">The priority of the e-mail</param>
        ''' <param name="Sensitivity">The sensitivity of the e-mail</param>
        ''' <param name="RequestTransmissionConfirmation">Request a transmission confirmation</param>
        ''' <param name="RequestReadingConfirmation">Request a reading confirmation</param>
        ''' <param name="AdditionalHeaders">Additional headers for the e-mail</param>
        ''' <returns>True if successfull, false for failures (also see bufErrorDetails)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function System_SendEMail_MultipleRcpts_NetFramework1(ByVal RcptAddresses_To As String, ByVal RcptAddresses_CC As String, ByVal RcptAddresses_BCC As String, ByVal MsgSubject As String, ByVal MsgTextBody As String, ByVal MsgHTMLBody As String, ByVal SenderName As String, ByVal SenderAddress As String, Optional ByVal MsgCharset As String = Nothing, Optional ByRef bufErrorDetails As String = Nothing, Optional ByVal Attachments As CompuMaster.camm.WebManager.Messaging.EMailAttachment() = Nothing, Optional ByVal Priority As CompuMaster.camm.WebManager.Messaging.EMails.Priority = Nothing, Optional ByVal Sensitivity As CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity = Nothing, Optional ByVal RequestTransmissionConfirmation As Boolean = False, Optional ByVal RequestReadingConfirmation As Boolean = False, Optional ByVal AdditionalHeaders As Collections.Specialized.NameValueCollection = Nothing) As Boolean
            Dim MyMail As New System.Web.Mail.MailMessage
            Dim ErrorFound As String = Nothing
            FixHtmlContentIDs(MsgHTMLBody, Attachments)

            'Prepare main mail properties
            Mail.SmtpMail.SmtpServer = _WebManager.SMTPServerName & CType(IIf(_WebManager.SMTPServerPort <> 25, ":" & _WebManager.SMTPServerPort, ""), String)

            'sendusing cdoSendUsingPort, value 2, for sending the message using 
            'the network.
            '
            'smtpauthenticate Specifies the mechanism used when authenticating 
            'to an SMTP 
            'service over the network. Possible values are:
            '- cdoAnonymous, value 0. Do Not authenticate.
            '- cdoBasic, value 1. Use basic clear-text authentication. 
            'When using this option you have to provide the user name And password 
            'through the sendusername And sendpassword fields.
            '- cdoNTLM, value 2. The current process security context Is used to 
            ' authenticate with the service.

            Select Case _WebManager.SMTPAuthType
                Case "LOGIN", "PLAIN"
                    If Me._WebManager.SMTPUserName = Nothing Then Throw New ArgumentNullException("WebManager.SmtpUserName")
                    If Me._WebManager.SMTPPassword = Nothing Then Throw New ArgumentNullException("WebManager.SmtpPassword")
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1) 'basic auth
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", _WebManager.SMTPUserName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", _WebManager.SMTPPassword)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", 2) 'send using network
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", _WebManager.SMTPServerName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout", 10)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", _WebManager.SMTPServerPort)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", False)
                Case "LOGIN-SSL"
                    If Me._WebManager.SMTPUserName = Nothing Then Throw New ArgumentNullException("WebManager.SmtpUserName")
                    If Me._WebManager.SMTPPassword = Nothing Then Throw New ArgumentNullException("WebManager.SmtpPassword")
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1) 'basic auth
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", _WebManager.SMTPUserName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", _WebManager.SMTPPassword)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", 2) 'send using network
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", _WebManager.SMTPServerName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout", 10)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", _WebManager.SMTPServerPort)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", True)
                Case "CRAM-MD5"
                    bufErrorDetails = (New NotSupportedException("CRAM-MD5 is not supported")).ToString
                    Return False
                Case "NTLM"
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 2) 'NTLM auth
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", _WebManager.SMTPUserName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", _WebManager.SMTPPassword)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", 2) 'send using network
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", _WebManager.SMTPServerName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout", 10)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", _WebManager.SMTPServerPort)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", False)
                Case "NONE-SSL"
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 0) 'send anonymous
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", 2) 'send using network
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", _WebManager.SMTPServerName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout", 10)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", _WebManager.SMTPServerPort)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", True)
                Case Else
                    'SmtpAuthMethod = "NONE"
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 0) 'send anonymous
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusing", 2) 'send using network
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserver", _WebManager.SMTPServerName)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpconnectiontimeout", 10)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", _WebManager.SMTPServerPort)
                    MyMail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", False)
            End Select

            'Add all recepients
            If RcptAddresses_To <> Nothing Then
                'TODO 4 TZ: why this workaround here and not at BCC too?
                'MyMail.To = Strings.Join(Me.ExtractEMailAddressesFromReceipientsList(RcptAddresses_To), ";")
                'MyMail.Headers.Add("To", RcptAddresses_To)
                'MyMail.To = Strings.Join(EMails.SplitEMailAddressesFromReceipientsList(RcptAddresses_To), ",")
                MyMail.Headers.Add("To", Strings.Join(EMails.SplitEMailAddressesFromReceipientsList(RcptAddresses_To), ","))
            End If
            If RcptAddresses_CC <> Nothing Then
                'TODO 4 TZ: why this workaround here and not at BCC too?
                'MyMail.Cc = Strings.Join(Me.ExtractEMailAddressesFromReceipientsList(RcptAddresses_CC), ";")
                'MyMail.Headers.Add("Cc", RcptAddresses_CC)
                'MyMail.Cc = Strings.Join(EMails.SplitEMailAddressesFromReceipientsList(RcptAddresses_CC), ",")
                MyMail.Headers.Add("Cc", Strings.Join(EMails.SplitEMailAddressesFromReceipientsList(RcptAddresses_CC), ","))
            End If
            If RcptAddresses_BCC <> Nothing Then
                'TODO 4 TZ: why no workaround here but at TO and CC?
                MyMail.Bcc = Strings.Join(EMails.SplitEMailAddressesFromReceipientsList(RcptAddresses_BCC), ";")
            End If

            MyMail.Subject = MsgSubject

            Try
                MyMail.BodyEncoding = System.Text.Encoding.GetEncoding(MsgCharset)
            Catch
                Try
                    'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                    Dim WorkaroundEx As New Exception("")
                    Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                    Try
                        WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                    Catch
                    End Try
                    _WebManager.Log.RuntimeWarning("Not supported by System.Web.Mail: encoding cannot retrieved from string """ & MsgCharset & """", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)
                Catch
                End Try
            End Try

            If MsgHTMLBody = "" Then
                MyMail.BodyFormat = System.Web.Mail.MailFormat.Text
                MyMail.Body = MsgTextBody
            Else
                MyMail.BodyFormat = System.Web.Mail.MailFormat.Html
                MyMail.Body = MsgHTMLBody
            End If

            MyMail.From = SenderAddress
            MyMail.Headers.Add("Sender", EMails.CreateReceipientString(SenderName, SenderAddress))

            'Add attachments / related/embedded objects
            Dim TempFiles As New ArrayList
            If Not Attachments Is Nothing Then
                For MyCounter As Integer = 0 To Attachments.Length - 1
                    Dim MyAttachment As System.Web.Mail.MailAttachment = Nothing
                    If Attachments(MyCounter).PlaceholderInMhtmlToBeReplacedByContentID <> Nothing Then
                        Try
                            'Required attachment header (exemplary) for reference to src="cid:mycidvalue"
                            'Content-Type: image/jpeg; name="image001.jpg"
                            'Content-Description: image001.jpg
                            'Content-Disposition: inline; filename="image001.jpg"; size=2177;
                            'Content-ID: <mycidvalue>

                            'Supported @ .Net 2.0 - BUT NOT WITH .NET 1.x using CDO mechanisms!
                            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                            Dim WorkaroundEx As New Exception("")
                            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                            Try
                                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                            Catch
                            End Try
                            _WebManager.Log.RuntimeWarning("Not supported by System.Web.Mail: placeholders in MHTML (CIDs)", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)

                            'CRITICAL ERROR: EMBEDDED ATTACHMENTS NOT SUPPORTED HERE
                            bufErrorDetails = (New NotSupportedException("Embedded attachments not supported in MailSendingSystem .Net 1.x/CDO")).ToString
                            Return False
                        Catch
                        End Try
                    End If
                    If Not Attachments(MyCounter).RawData Is Nothing OrElse Attachments(MyCounter).RawDataFilename <> Nothing Then
                        Try
                            'Create a temporary file, store the data there and add that file as attachment before removing again
                            Dim tempFile As String = System.IO.Path.GetTempFileName()
                            System.IO.File.Delete(tempFile)
                            TempFiles.Add(tempFile)
                            System.IO.Directory.CreateDirectory(tempFile)
                            tempFile &= System.IO.Path.DirectorySeparatorChar & Attachments(MyCounter).RawDataFilename
                            Dim tempFileStream As System.IO.FileStream = System.IO.File.Create(tempFile)
                            tempFileStream.Write(Attachments(MyCounter).RawData, 0, Attachments(MyCounter).RawData.Length)
                            tempFileStream.Flush()
                            tempFileStream.Close()
                            'TempFiles.Add(tempFile)
                            MyAttachment = New System.Web.Mail.MailAttachment(tempFile)
                        Catch ex As Exception
                            ErrorFound = ex.ToString & vbNewLine
                        End Try
                    ElseIf Attachments(MyCounter).FilePath <> "" Then
                        Dim fi As New IO.FileInfo(Attachments(MyCounter).FilePath)
                        If fi.Exists Then
                            Try
                                MyAttachment = New System.Web.Mail.MailAttachment(Attachments(MyCounter).FilePath)
                            Catch ex As Exception
                                _WebManager.Log.RuntimeException(ex, True, False, WMSystem.DebugLevels.NoDebug)
                                bufErrorDetails = ex.ToString
                                Return False
                            End Try
                        Else
                            bufErrorDetails = "Attachment not found: " & Attachments(MyCounter).FilePath
                            Return False
                        End If
                    Else
                        Try
                            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                            Dim WorkaroundEx As New Exception("")
                            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                            Try
                                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                            Catch
                            End Try
                            _WebManager.Log.RuntimeWarning("Empty or invalid email attachment", WorkaroundStackTrace, WMSystem.DebugLevels.NoDebug, True, False)
                        Catch
                        End Try
                    End If
                    If Not MyAttachment Is Nothing Then
                        MyMail.Attachments.Add(MyAttachment)
                    End If
                Next
            End If

            'Setup mail priority
            If Not Priority = Nothing Then
                Select Case Priority
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low
                    Case CType(4, CompuMaster.camm.WebManager.Messaging.EMails.Priority) 'Between Normal and Low
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.High
                    Case CType(2, CompuMaster.camm.WebManager.Messaging.EMails.Priority) 'Between Normal and High
                    Case Else 'CompuMaster.camm.WebManager.WMSystem.MailImportance.Normal
                        Priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.Normal
                End Select
                MyMail.Headers.Add("X-Priority", CByte(Priority).ToString())
                If Priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.High OrElse Priority = CompuMaster.camm.WebManager.WMSystem.MailImportance.Normal OrElse Priority = CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low Then
                    MyMail.Headers.Add("Importance", Priority.ToString())
                    MyMail.Headers.Add("Priority", Priority.ToString())
                    MyMail.Headers.Add("X-MSMail-Priority", Priority.ToString())
                End If
                Select Case Priority
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.High, CType(2, CompuMaster.camm.WebManager.Messaging.EMails.Priority)
                        MyMail.Priority = System.Web.Mail.MailPriority.High
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Priority.Low, CType(4, CompuMaster.camm.WebManager.Messaging.EMails.Priority)
                        MyMail.Priority = System.Web.Mail.MailPriority.Low
                    Case Else
                        MyMail.Priority = System.Web.Mail.MailPriority.Normal
                End Select
            End If

            'Setup mail sensitivity
            If Not Sensitivity = Nothing Then
                Select Case Sensitivity
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_CompanyConfidential
                        MyMail.Headers.Add("Sensitivity", "Company-Confidential")
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal
                        MyMail.Headers.Add("Sensitivity", "Personal")
                    Case CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Private
                        MyMail.Headers.Add("Sensitivity", "Private")
                End Select
            End If

            'Setup mail receipt confirmations
            If RequestReadingConfirmation Then
                'Disposition-Notification-To: "Jochen Wezel" <compumaster@web.de> 'Lesebest?tigung
                MyMail.Headers.Add("Disposition-Notification-To", SenderAddress)
            End If
            'TODO: The bug is that nothing happens :( _
            If RequestTransmissionConfirmation Then
                'Return-Receipt-To: "Jochen Wezel" <compumaster@web.de> '?bermittlungsbest?tigung
                MyMail.Headers.Add("Return-Receipt-To", SenderAddress)
            End If

            'Setup additional headers
            If Not AdditionalHeaders Is Nothing Then
                For Each MyKey As String In AdditionalHeaders
                    MyMail.Headers.Add(MyKey, AdditionalHeaders(MyKey))
                Next
            End If

            'Send the mail and return the success/error status
            Dim Result As Boolean
            If ErrorFound = Nothing Then
                Try
                    System.Web.Mail.SmtpMail.Send(MyMail)
                    Result = True
                Catch ex As Exception
                    Result = False
#If DEBUG Then
                    bufErrorDetails = ex.ToString
#Else
                    If _WebManager.DebugLevel >= _WebManager.DebugLevels.Medium_LoggingOfDebugInformation Then
                        bufErrorDetails = ex.ToString
                    Else
                        bufErrorDetails = ex.Message
                    End If
#End If
                End Try
            Else
                bufErrorDetails = ErrorFound
            End If

            'Clean up all temporary files
            For MyCounter As Integer = 0 To TempFiles.Count - 1
                Try
                    System.IO.Directory.Delete(CType(TempFiles(MyCounter), String), True)
                Catch
                End Try
            Next

            Return Result

        End Function
#End Region
#If VS2015OrHigher = True Then
#Enable Warning BC40000 'obsolete warnings
#End If
    End Class

#Region " Friend Class Mail "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Messaging.Mail
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     The content of an e-mail with all required data
    ''' </summary>
    ''' <remarks>
    '''     ToDo: some fields are not yet use in queueing and queue send method, implement them everywhere
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	24.11.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
#End Region

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     An e-mail attachment
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	04.05.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Binary data for this attachment
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property RawData() As Byte()
            Get
                Return _AttachmentData
            End Get
            Set(ByVal Value As Byte())
                _AttachmentData = Value
            End Set
        End Property

        Private _AttachmentData_Filename As String
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The filename for the binary data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property RawDataFilename() As String
            Get
                Return _AttachmentData_Filename
            End Get
            Set(ByVal Value As String)
                _AttachmentData_Filename = Value
            End Set
        End Property

        Private _AttachmentFile As String
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A path to a file which shall be included
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FilePath() As String
            Get
                Return _AttachmentFile
            End Get
            Set(ByVal Value As String)
                _AttachmentFile = Value
            End Set
        End Property

        Private _PlaceholderInMHTML_ToReplaceWithCID As String ' <Obsolete("Use ContentID instead", False), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A placeholder string (without prefix "cid:") in the HTML code of the message (there it must be with prefix "cid:") which shall be replaced by the CID code of the attachment
        ''' </summary>
        ''' <remarks>
        ''' <para>Define the placeholder which shall be replaced by the Content-ID for the contents of a file to the email. Emails formatted in HTML can include images with this information and internally reference the image through a "cid" hyperlink.</para>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PlaceholderInMhtmlToBeReplacedByContentID() As String
            Get
                Return _PlaceholderInMHTML_ToReplaceWithCID
            End Get
            Set(ByVal Value As String)
                _PlaceholderInMHTML_ToReplaceWithCID = Value
            End Set
        End Property

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Messaging.QueueMonitoring
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Methods for the mail queue manager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	04.05.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class QueueMonitoring

        Private _WebManager As WMSystem
        Friend Sub New(ByVal cammWebManager As WMSystem)
            _WebManager = cammWebManager
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Number of mails in the mail queue
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function MailsInQueue() As Integer
            Dim Result As Object
            Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(_WebManager.ConnectionString), _
                            "SELECT COUNT(*) AS NumberOfItems FROM [dbo].[Log_eMailMessages] WHERE State = " & CType(QueueStates.Queued, Byte).ToString & " OR (State = " & CType(QueueStates.FailureAfter1Trial, Byte).ToString & " AND DATEDIFF (mi,[DateTime],GetDate()) > 15) OR (State IN (" & CType(QueueStates.FailureAfter2Trials, Byte) & "," & CType(QueueStates.Sending, Byte) & ") AND DATEDIFF (d,[DateTime],GetDate()) > 1)", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            SetLastActivityDate(New SqlConnection(_WebManager.ConnectionString), Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Return Utils.Nz(Result, 0)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Process first mail in queue
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ProcessOneMail()

            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)

            'Load queued elements or those ones which have [failed 1st time 15 minutes or more ago] or have [failed 2nd time 1 day or more ago] or which have [kept on status "Sending" for more than 1 day]
            'To the same time, switch it to state "sending"
            Dim MyReader As IDataReader = Nothing
            Dim MyMailID As Integer, MyMailData As String, MyMailState As QueueStates, MyMailDateTime As DateTime
            Try
                MyReader = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader( _
                        MyConn, _
                        "declare @ID int, @OriginState int" & vbNewLine & _
                            "SELECT TOP 1 @ID = [ID], @OriginState = State FROM [dbo].[Log_eMailMessages] WHERE State = " & CType(QueueStates.Queued, Byte).ToString & " OR (State = " & CType(QueueStates.FailureAfter1Trial, Byte).ToString & " AND DATEDIFF (mi,[DateTime],GetDate()) > 15) OR (State IN (" & CType(QueueStates.FailureAfter2Trials, Byte) & "," & CType(QueueStates.Sending, Byte) & ") AND DATEDIFF (d,[DateTime],GetDate()) > 1) ORDER BY State" & vbNewLine & _
                            "UPDATE [dbo].[Log_eMailMessages] SET State = " & CType(QueueStates.Sending, Byte).ToString & ", [DateTime] = GETDATE() WHERE ID = @ID" & vbNewLine & _
                            "SELECT [ID], [UserID], [data], [State], [DateTime], @OriginState As OriginState FROM [dbo].[Log_eMailMessages] WHERE ID = @ID", _
                        CommandType.Text, _
                        Nothing, _
                        Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                'Read one data row
                If Not MyReader.Read Then
                    'Nothing to do; return immediately
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                    Return
                Else
                    MyMailID = CType(MyReader("ID"), Integer)
                    MyMailData = CType(MyReader("Data"), String)
                    MyMailState = CType(MyReader("OriginState"), CompuMaster.camm.WebManager.Messaging.QueueMonitoring.QueueStates)
                    MyMailDateTime = CType(MyReader("DateTime"), DateTime)
                End If
            Catch
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                Throw
            Finally
                MyReader.Close()
            End Try

            'Send Mail
            Dim Success As Boolean
            Try
                Success = SendMail(MyMailData, MyMailID, MyConn)
            Catch ex As Exception
                SetLastErrorDetails(ex.ToString, MyMailID, MyConn, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                If Setup.DatabaseUtils.Version(Me._WebManager, True).Build < 155 Then 'exception hasn't been logged in queue, so log it in event log
                    Me._WebManager.Log.RuntimeException(ex, False, False, WMSystem.DebugLevels.NoDebug)
                End If
                Success = False
            End Try

            'Switch to next state
            Dim NewState As QueueStates
            If Success Then
                NewState = QueueStates.Successfull
            Else
                Select Case MyMailState
                    Case QueueStates.Queued
                        NewState = QueueStates.FailureAfter1Trial
                    Case QueueStates.FailureAfter1Trial
                        NewState = QueueStates.FailureAfter2Trials
                    Case Else 'QueueStates.FailureAfter2Trials, QueueStates.Sending
                        NewState = QueueStates.FailureAfterLastTrial
                End Select
            End If
            Try
                'Update this single mail
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyConn, "UPDATE [dbo].[Log_eMailMessages] SET State = " & CType(NewState, Byte).ToString & ", [DateTime] = GETDATE() WHERE ID = " & MyMailID.ToString, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                'Cleanup all queued elements never commited or cancelled and cancel them after 1 day
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyConn, "UPDATE [dbo].[Log_eMailMessages] SET State = " & CType(QueueStates.Cancelled, Byte).ToString & ", [DateTime] = GETDATE() WHERE State = " & CType(QueueStates.WaitingForReleaseBeforeQueuing, Byte).ToString & " AND DateAdd(dd, 1, [DateTime]) < GETDATE()", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
            Catch
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                Throw
            End Try

            'Trigger activity and queue truncation
            Try
                SetLastActivityDate(MyConn, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                CleanUpOldQueueElementsWhenNotDoneTooLongTime(MyConn)
            Finally
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Send the mail which has been serialized as XML data
        ''' </summary>
        ''' <param name="xml">The XML representation of the mail data</param>
        ''' <param name="mailQueueID">The ID of the item in the mail queue, required for logging purposes only</param>
        ''' <returns>True if successfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SendMail(ByVal xml As String, ByVal mailQueueID As Integer, ByVal dbConnection As SqlClient.SqlConnection) As Boolean

            Dim MailData As New WebManager.Messaging.MailMessage(xml, _WebManager)
            Dim Errors As String = String.Empty
            Dim Result As Boolean = _WebManager.MessagingEMails.SendEMail(False, MailData.To, MailData.Cc, MailData.Bcc, MailData.Subject, MailData.BodyPlainText, MailData.BodyHtml, MailData.FromName, MailData.FromAddress, MailData.EMailAttachments, MailData.Priority, MailData.Sensitivity, MailData.RequestTransmissionConfirmation, MailData.RequestReadingConfirmation, MailData.AdditionalHeaders, MailData.Charset, Errors)
            If Errors <> Nothing Then
                SetLastErrorDetails(Errors, mailQueueID, dbConnection, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                _WebManager.Log.RuntimeWarning("Error while sending mail ID " & mailQueueID & " from " & MailData.FromAddress & " via mail queue: " & vbNewLine & Errors & vbNewLine, WorkaroundStackTrace)
                Result = False
            End If
            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Update the state of a queued item
        ''' </summary>
        ''' <param name="queuedItemID">ID of Log_emailMessage record</param>
        ''' <param name="state">State of an email</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateQueueState(ByVal queuedItemID As Integer, ByVal state As Messaging.QueueMonitoring.QueueStates)
            Dim dbConn As New System.Data.SqlClient.SqlConnection(Me._WebManager.ConnectionString)
            dbConn.Open()

            Dim cmd As New SqlClient.SqlCommand
            Try
                With cmd
                    .CommandText = "Update [dbo].[Log_eMailMessages] Set [State] = " & state & " where [ID] = " & queuedItemID
                    .CommandType = CommandType.Text
                    .Connection = dbConn

                    .ExecuteNonQuery()
                End With

            Finally
                If Not cmd Is Nothing Then
                    cmd.Dispose()
                End If
                If Not dbConn Is Nothing Then
                    If dbConn.State <> ConnectionState.Closed Then
                        dbConn.Close()
                    End If
                    dbConn.Dispose()
                End If
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Log_eMailData for emailID
        ''' </summary>
        ''' <param name="queuedItemID">unique id of Log_eMailMessage record</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function LoadMailMessage(ByVal queuedItemID As Integer) As WebManager.Messaging.MailMessage
            Dim result As String

            Dim dbConn As New System.Data.SqlClient.SqlConnection(Me._WebManager.ConnectionString)
            dbConn.Open()

            Dim obj As Object
            Dim cmd As New SqlClient.SqlCommand
            Try
                With cmd
                    .CommandText = "SELECT [Data] FROM [dbo].[Log_eMailMessages] where [ID] = " & queuedItemID
                    .CommandType = CommandType.Text
                    .Connection = dbConn

                    obj = .ExecuteScalar
                End With

            Finally
                If Not cmd Is Nothing Then
                    cmd.Dispose()
                End If
                If Not dbConn Is Nothing Then
                    If dbConn.State <> ConnectionState.Closed Then
                        dbConn.Close()
                    End If
                    dbConn.Dispose()
                End If
            End Try
            result = Utils.Nz(obj, CType(Nothing, String))

            Return New MailMessage(result, _WebManager)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Available states in the mail queue table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Enum QueueStates As Byte
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The mail has been stored into the queue, but hasn't been allowed to be sent yet
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            WaitingForReleaseBeforeQueuing = 0
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Mail has been queued, sending will follow asap.
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Queued = 1
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The queue mailing system is currently engaged to send this mail
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Sending = 2
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Mail transmission has been cancelled by the sending user or the sending application
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Cancelled = 3
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Mail has been delivered without errors
            ''' </summary>
            ''' <remarks>
            '''     This doesn't say that (all of) the receipients have got the mail, it only says that there was no error while sending it
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Successfull = 4
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Delivery of the mail has been failed one time
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            FailureAfter1Trial = 5
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Delivery of the mail has been failed two times
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            FailureAfter2Trials = 6
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The transfer will not be repeated, the delivery of the mail has been failed three times
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[AdminSupport]	04.05.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            FailureAfterLastTrial = 7
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The three time failure is accepted by moderator
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	24.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            FailureAccepted = 8
        End Enum

#Region "Trigger/register the availability of the queuing mechanism"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Set the last error information by the mail queue processor
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Zeutzheim]	06.12.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetLastErrorDetails(ByVal errorDetails As String, ByVal mailQueueID As Integer, ByVal connection As SqlConnection, ByVal automations As CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations)
            Dim _DBBuildNo As Integer = Setup.DatabaseUtils.Version(Me._WebManager, True).Build
            If _DBBuildNo >= 155 Then
                Try
                    Dim MyCmd As New SqlCommand("UPDATE [dbo].[Log_eMailMessages] SET [ErrorDetails] = @ErrDetails WHERE ID = @ID", connection)
                    If errorDetails = Nothing Then
                        MyCmd.Parameters.Add("@ErrDetails", SqlDbType.NText).Value = DBNull.Value
                    Else
                        MyCmd.Parameters.Add("@ErrDetails", SqlDbType.NText).Value = errorDetails
                    End If
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = mailQueueID
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, automations)
                Catch
                End Try
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Set the last activity information by the mail queue processor
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	04.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetLastActivityDate(ByVal connection As SqlConnection, ByVal automations As CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations)
            Try
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery( _
                        connection, _
                        "DECLARE @RowNumber int" & vbNewLine & _
                            "SELECT @RowNumber = COUNT(*)" & vbNewLine & _
                            "FROM [dbo].[System_GlobalProperties]" & vbNewLine & _
                            "WHERE VALUENVarChar = N'camm WebManager' AND PropertyName = N'LastMailQueueProcessing'" & vbNewLine & _
                            "SELECT @RowNumber" & vbNewLine & _
                            vbNewLine & _
                            "IF @RowNumber = 0 " & vbNewLine & _
                            "	INSERT INTO [dbo].[System_GlobalProperties]" & vbNewLine & _
                            "		(ValueNVarChar, PropertyName, ValueDateTime)" & vbNewLine & _
                            "	VALUES (N'camm WebManager', N'LastMailQueueProcessing', GetDate())" & vbNewLine & _
                            "ELSE" & vbNewLine & _
                            "	UPDATE [dbo].[System_GlobalProperties]" & vbNewLine & _
                            "	SET ValueDateTime = GetDate()" & vbNewLine & _
                            "	WHERE ValueNVarChar = N'camm WebManager' AND PropertyName = N'LastMailQueueProcessing'" & vbNewLine, _
                        CommandType.Text, _
                        Nothing, _
                        automations)
            Catch
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Has the monitor been active in the last 30 minutes?
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	05.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function MonitorIsActive() As Boolean

            Dim Result As Integer
            Try
                Result = CType(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar( _
                        New SqlConnection(_WebManager.ConnectionString), _
                            "SELECT COUNT(*)" & vbNewLine & _
                            "FROM [dbo].[System_GlobalProperties]" & vbNewLine & _
                            "WHERE VALUENVarChar = N'camm WebManager' AND PropertyName = N'LastMailQueueProcessing' AND DATEDIFF (mi,[ValueDateTime],GetDate()) < 30", _
                        CommandType.Text, _
                        Nothing, _
                        Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Catch
                Result = 0
            End Try
            'Return true if there was one row found
            Return (Result = 1)

        End Function

#End Region

#Region "Shorten mail queue table when there are too old rows"

        Private Sub CleanUpOldQueueElementsWhenNotDoneTooLongTime(ByVal connection As SqlConnection)

            'Detect the last execution of the cleanup method
            Static LastCleanUpCached As DateTime
            Dim LastCleanUp As DateTime
            If HttpContext.Current Is Nothing Then
                LastCleanUp = LastCleanUpCached
            Else
                Try
                    LastCleanUp = CType(HttpContext.Current.Application("WebManager.Messaging.Queue.LastCleanUp"), DateTime)
                Catch
                End Try
            End If
            'Detect if it has to be run again
            Dim PerformCleanup As Boolean
            If LastCleanUp = Nothing Then
                PerformCleanup = True
            ElseIf Now.Subtract(LastCleanUp).TotalHours > 4 Then
                PerformCleanup = True
            End If
            If PerformCleanup = True Then
                Me.CleanUpOldQueueElements(connection)
                If HttpContext.Current Is Nothing Then
                    LastCleanUpCached = Now
                Else
                    HttpContext.Current.Application("WebManager.Messaging.Queue.LastCleanUp") = Now
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Remove old lines from the mail queue which are older than 14 days
        ''' </summary>
        ''' <remarks>
        '''     An additional new log entry will be created to log the truncation
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	05.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CleanUpOldQueueElements(ByVal connection As SqlConnection)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery( _
                        connection, _
                        "DELETE [dbo].[Log_eMailMessages] FROM (SELECT ID FROM [dbo].[Log_eMailMessages] WHERE State IN (" & CType(QueueStates.Cancelled, Byte) & "," & CType(QueueStates.Successfull, Byte) & "," & CType(QueueStates.FailureAfterLastTrial, Byte) & "," & CType(QueueStates.FailureAccepted, Byte) & ") AND DATEDIFF (d,[DateTime],GetDate()) > 90) AS toremove WHERE dbo.Log_eMailMessages.ID = toremove.ID", _
                        CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.None, 300)
        End Sub
#End Region

    End Class

End Namespace