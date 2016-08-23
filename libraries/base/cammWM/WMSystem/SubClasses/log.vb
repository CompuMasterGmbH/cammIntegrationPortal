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

Imports CompuMaster.camm.WebManager.WMSystem
Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     Event log methods of camm Web-Manager
    ''' </summary>
    Public Class Log
        Private _WebManager As WMSystem
        Sub New(ByVal webManager As WMSystem)
            _WebManager = webManager
        End Sub

#Region "EventLog"
        ''' <summary>
        ''' Write a log entry into the sytem's event log in case tracing is enabled in configuration
        ''' </summary>
        ''' <param name="data">The event details</param>
        ''' <param name="type">Default type is Information</param>
        ''' <param name="writeAlwaysIndependentlyFromConfig">Write this entry always independently from the configured web.config setting WebManager.EventLogTrace</param>
        Friend Shared Sub WriteEventLogTrace(ByVal data As String, Optional ByVal type As System.Diagnostics.EventLogEntryType = Diagnostics.EventLogEntryType.Information, Optional ByVal writeAlwaysIndependentlyFromConfig As Boolean = False)
            Static _ConfigSettingEventLogTrace As TripleState
            If _ConfigSettingEventLogTrace = TripleState.Undefined Then
                _ConfigSettingEventLogTrace = Utils.BooleanToWMTriplestate(Configuration.EventLogTrace)
            End If
            If writeAlwaysIndependentlyFromConfig OrElse _ConfigSettingEventLogTrace = TripleState.True Then
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                Utils.WriteToEventLog(type, data & vbNewLine & WorkaroundStackTrace)
            End If
        End Sub
#End Region

#Region "FileLog in AppData"
#If NetFramework <> "1_1" Then
        ''' <summary>
        ''' Collect and write error data into the error log file on webserver disk (see app_data directory)
        ''' </summary>
        <Obsolete("The preferred log mechanism should be to database or e-mail")> _
        Friend Shared Sub LogToFileError(exception As Exception)
            Dim filePath As String = System.Web.HttpContext.Current.Server.MapPath("~/app_data/error.log")
            Dim basePath As String = System.IO.Path.GetDirectoryName(filePath)
            If System.IO.Directory.Exists(basePath) = False Then System.IO.Directory.CreateDirectory(basePath)
            Dim LogData As String = exception.ToString
            LogData &= vbNewLine & "REQUEST QUERY ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.QueryString
                LogData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.QueryString(key)
            Next
            LogData &= vbNewLine & "REQUEST FORM ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.Form
                LogData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.Form(key)
            Next
            LogData &= vbNewLine & "REQUEST SERVER ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.ServerVariables
                If key.StartsWith("ALL_") = False AndAlso System.Web.HttpContext.Current.Request.ServerVariables(key) <> Nothing Then
                    LogData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.ServerVariables(key)
                End If
            Next
            Try
                System.Web.HttpContext.Current.Application.Lock()
                System.IO.File.AppendAllText(filePath, vbNewLine & vbNewLine & "ERROR ON " & Now.ToString("yyyy-MM-dd HH:mm:ss") & vbNewLine & LogData)
            Finally
                System.Web.HttpContext.Current.Application.UnLock()
            End Try
        End Sub

        ''' <summary>
        ''' Collect and write warning data into the warning log file on webserver disk (see app_data directory)
        ''' </summary>
        <Obsolete("The preferred log mechanism should be to database or e-mail")> _
        Friend Shared Sub LogToFileWarning(exception As Exception)
            Dim filePath As String = System.Web.HttpContext.Current.Server.MapPath("~/app_data/warning.log")
            Dim basePath As String = System.IO.Path.GetDirectoryName(filePath)
            If System.IO.Directory.Exists(basePath) = False Then System.IO.Directory.CreateDirectory(basePath)
            Dim LogData As String = exception.ToString
            LogData &= vbNewLine & "REQUEST QUERY ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.QueryString
                LogData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.QueryString(key)
            Next
            LogData &= vbNewLine & "REQUEST FORM ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.Form
                LogData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.Form(key)
            Next
            LogData &= vbNewLine & "REQUEST SERVER ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.ServerVariables
                If key.StartsWith("ALL_") = False AndAlso System.Web.HttpContext.Current.Request.ServerVariables(key) <> Nothing Then
                    LogData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.ServerVariables(key)
                End If
            Next
            Try
                System.Web.HttpContext.Current.Application.Lock()
                System.IO.File.AppendAllText(filePath, vbNewLine & vbNewLine & "WARNING ON " & Now.ToString("yyyy-MM-dd HH:mm:ss") & vbNewLine & LogData)
            Finally
                System.Web.HttpContext.Current.Application.UnLock()
            End Try
        End Sub

        ''' <summary>
        ''' Collect and write debug data into the debug log file on webserver disk (see app_data directory)
        ''' </summary>
        <Obsolete("The preferred log mechanism should be to database or e-mail")> _
        Friend Shared Sub LogToFileDebugInfo(logData As String)
            Dim filePath As String = System.Web.HttpContext.Current.Server.MapPath("~/app_data/debug.log")
            Dim basePath As String = System.IO.Path.GetDirectoryName(filePath)
            If System.IO.Directory.Exists(basePath) = False Then System.IO.Directory.CreateDirectory(basePath)
            logData &= vbNewLine & "REQUEST QUERY ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.QueryString
                logData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.QueryString(key)
            Next
            logData &= vbNewLine & "REQUEST FORM ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.Form
                logData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.Form(key)
            Next
            logData &= vbNewLine & "REQUEST SERVER ITEMS"
            For Each key As String In System.Web.HttpContext.Current.Request.ServerVariables
                If key.StartsWith("ALL_") = False AndAlso System.Web.HttpContext.Current.Request.ServerVariables(key) <> Nothing Then
                    logData &= vbNewLine & "  - " & key & "=" & System.Web.HttpContext.Current.Request.ServerVariables(key)
                End If
            Next
            Try
                System.Web.HttpContext.Current.Application.Lock()
                System.IO.File.AppendAllText(filePath, vbNewLine & vbNewLine & "DEBUG INFO ON " & Now.ToString("yyyy-MM-dd HH:mm:ss") & vbNewLine & logData)
            Finally
                System.Web.HttpContext.Current.Application.UnLock()
            End Try
        End Sub
#End If
#End Region

#Region "Send error e-mail"

        ''' <summary>
        ''' Send an exception notification to the responsible contact
        ''' </summary>
        ''' <param name="exception">A warning message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use ReportWarningByEMail instead")> _
        Public Sub ReportWarningViaEMail(ByVal exception As Exception, ByVal messageSubject As String)
            ReportWarningByEMail(exception, messageSubject)
        End Sub

        ''' <summary>
        ''' Send an exception notification to the responsible contact
        ''' </summary>
        ''' <param name="exception">A warning message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <returns>True if the e-mail could be sent using a configured e-mail send system, False if no configured e-mail send system has been available or if another error occured</returns>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        Public Function ReportWarningByEMail(ByVal exception As Exception, ByVal messageSubject As String) As Boolean
            Log.WriteEventLogTrace("ReportWarningByEMail1:Begin")
            Dim BodyHtmlText As String = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(exception.ToString))
            Dim BodyPlainText As String = exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(exception)
            If AdditionalExceptionDetails <> Nothing Then
                BodyPlainText &= vbNewLine & AdditionalExceptionDetails
                BodyHtmlText &= "<p><em>" & Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(AdditionalExceptionDetails)) & "</em></p>"
            End If

            Dim Result As Boolean
            Result = ReportWarningByEMail(BodyPlainText, BodyHtmlText, messageSubject)
            Log.WriteEventLogTrace("ReportWarningByEMail1:End")
            Return Result
        End Function

        ''' <summary>
        ''' Send a warning notification to the responsible contact
        ''' </summary>
        ''' <param name="plainText">A plain text warning message which shall be reported to the developer/technical contact</param>
        ''' <param name="htmlText">An HTML warning message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use ReportWarningByEMail instead")> _
        Public Sub ReportWarningViaEMail(ByVal plainText As String, ByVal htmlText As String, ByVal messageSubject As String)
            ReportWarningByEMail(plainText, htmlText, messageSubject)
        End Sub

        ''' <summary>
        ''' Send a warning notification to the responsible contact
        ''' </summary>
        ''' <param name="plainText">A plain text warning message which shall be reported to the developer/technical contact</param>
        ''' <param name="htmlText">An HTML warning message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <returns>True if the e-mail could be sent using a configured e-mail send system, False if no configured e-mail send system has been available or if another error occured</returns>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        Public Function ReportWarningByEMail(ByVal plainText As String, ByVal htmlText As String, ByVal messageSubject As String) As Boolean
            Log.WriteEventLogTrace("ReportWarningByEMail2:Begin")
            If Not HttpContext.Current Is Nothing Then
                'Web application
                If messageSubject = Nothing Then messageSubject = "Page warning @ " & Me._WebManager.Page.Request.Url.Host
            Else
                'Console/windows application
                If messageSubject = Nothing Then messageSubject = "Warning @ " & System.Environment.MachineName
            End If
            Log.WriteEventLogTrace("ReportWarningByEMail2:End")
            Return ReportErrorByEMail(plainText, htmlText, messageSubject)
        End Function

        ''' <summary>
        ''' Send an exception notification to the responsible contact
        ''' </summary>
        ''' <param name="exception">An error which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use ReportErrorByEMail instead")> _
        Public Sub ReportErrorViaEMail(ByVal exception As Exception, ByVal messageSubject As String)
            ReportErrorByEMail(exception, messageSubject)
        End Sub

        ''' <summary>
        ''' Send an exception notification to the responsible contact
        ''' </summary>
        ''' <param name="exception">An error which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <returns>True if the e-mail could be sent using a configured e-mail send system, False if no configured e-mail send system has been available or if another error occured</returns>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        Public Function ReportErrorByEMail(ByVal exception As Exception, ByVal messageSubject As String) As Boolean
            Dim BodyHtmlText As String = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(exception.ToString))
            Dim BodyPlainText As String = exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(exception)
            If AdditionalExceptionDetails <> Nothing Then
                BodyPlainText &= vbNewLine & AdditionalExceptionDetails
                BodyHtmlText &= "<p><em>" & Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(AdditionalExceptionDetails)) & "</em></p>"
            End If

            Return ReportErrorByEMail(BodyPlainText, BodyHtmlText, messageSubject)
        End Function

        ''' <summary>
        ''' Step through the exception tree and collection additional data
        ''' </summary>
        ''' <param name="ex"></param>
        ''' <remarks></remarks>
        Friend Shared Function AdditionalDataOfException(ex As Exception) As String
            Dim Result As String = Nothing
#If NetFramework <> "1_1" Then
            If ex IsNot Nothing Then
                Try
                    For Each key As String In ex.Data.Keys
                        If Result <> Nothing Then Result &= vbNewLine
                        Result &= "    " & ex.GetType.ToString & " detail data: " & key & "=" & CType(ex.Data(key), String)
                    Next
                    If ex.InnerException IsNot Nothing Then
                        Result &= AdditionalDataOfException(ex.InnerException)
                    End If
                Catch lookupEx As Exception
                    If Result <> Nothing Then Result &= vbNewLine
                    Result &= "    WARNING: not able to lookup exception details: " & lookupEx.Message
                End Try
            End If
#End If
            Return Result
        End Function

        ''' <summary>
        ''' Send an error notification to the responsible contact
        ''' </summary>
        ''' <param name="plainText">A plain text error message which shall be reported to the developer/technical contact</param>
        ''' <param name="htmlText">An HTML error message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use ReportErrorByEMail instead")> _
        Public Sub ReportErrorViaEMail(ByVal plainText As String, ByVal htmlText As String, ByVal messageSubject As String)
            ReportErrorByEMail(plainText, htmlText, messageSubject)
        End Sub

        ''' <summary>
        ''' Send an error notification to the responsible contact
        ''' </summary>
        ''' <param name="plainText">A plain text error message which shall be reported to the developer/technical contact</param>
        ''' <param name="htmlText">An HTML error message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <returns>True if the e-mail could be sent using a configured e-mail send system, False if no configured e-mail send system has been available or if another error occured</returns>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        Public Function ReportErrorByEMail(ByVal plainText As String, ByVal htmlText As String, ByVal messageSubject As String) As Boolean
            Log.WriteEventLogTrace("ReportErrorByEMail1:Begin")
            If plainText = Nothing Then
                Throw New ArgumentNullException("plainText")
            End If
            If htmlText = Nothing AndAlso plainText <> Nothing Then
                htmlText = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(plainText))
            ElseIf htmlText = Nothing Then
                Throw New ArgumentNullException("htmlText")
            End If

            Dim user As System.Security.Principal.IPrincipal, request As System.Web.HttpRequest, context As System.Web.HttpContext
            If Not _WebManager.Page Is Nothing Then
                user = _WebManager.Page.User
                request = _WebManager.Page.Request
                context = HttpContext.Current
            ElseIf Not HttpContext.Current Is Nothing Then
                user = HttpContext.Current.User
                request = HttpContext.Current.Request
                context = HttpContext.Current
            Else
                user = New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent)
                request = Nothing
                context = Nothing
            End If

            Dim BodyHtmlText As String = Log.BuildHtmlMessage(htmlText, messageSubject, "", request, context, user, _WebManager, -1)
            Dim BodyPlainText As String = Log.BuildPlainMessage(plainText, messageSubject, "", request, context, user, _WebManager, -1)
            Dim Result As Boolean
            Result = ReportErrorByEMail(BodyPlainText, BodyHtmlText, messageSubject, user, request, context)
            Log.WriteEventLogTrace("ReportErrorByEMail1:End")
            Return Result
        End Function

        ''' <summary>
        ''' The location of the executed code/assembly, typically somewhere in the parent or grand parent directory
        ''' </summary>
        Private Shared Function GetCodeLocation() As String
            Dim result As String = ""
            Try
                result = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(New Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath)) & "|" & System.IO.Path.GetDirectoryName(New Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath)
            Catch ex As Exception
                result = "(couldn't get code location)"
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Send an exception notification to the responsible contact
        ''' </summary>
        ''' <param name="plainText">A plain text error message which shall be reported to the developer/technical contact</param>
        ''' <param name="htmlText">An HTML error message which shall be reported to the developer/technical contact</param>
        ''' <param name="messageSubject">The e-mail subject</param>
        ''' <param name="user">The current user principal</param>
        ''' <param name="request">The current request</param>
        ''' <param name="context">The current context of the request</param>
        ''' <returns>True if the e-mail could be sent using a configured e-mail send system, False if no configured e-mail send system has been available or if another error occured</returns>
        ''' <remarks>
        ''' Requires an active e-mail system. Errors will be ignored.
        ''' </remarks>
        Friend Function ReportErrorByEMail(ByVal plainText As String, ByVal htmlText As String, ByVal messageSubject As String, ByVal user As System.Security.Principal.IPrincipal, ByVal request As System.Web.HttpRequest, ByVal context As System.Web.HttpContext) As Boolean
            Log.WriteEventLogTrace("ReportErrorByEMail2:Begin")
            Dim Result As Boolean = True
            Try
                If Not HttpContext.Current Is Nothing Then
                    'Web application
                    If messageSubject = Nothing Then messageSubject = "Page error @ " & Me._WebManager.Page.Request.Url.Host
                    If CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.TechnicalContactAndDeveloper Then
                        _WebManager.MessagingEMails.SendEMail(Messaging.EMails.CreateReceipientString(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress) & "," & _WebManager.TechnicalServiceEMailAccountAddress, "", "", messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                    ElseIf CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.Developer Then
                        _WebManager.MessagingEMails.SendEMail(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress, messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                    Else
                        _WebManager.MessagingEMails.SendEMail(_WebManager.TechnicalServiceEMailAccountName, _WebManager.TechnicalServiceEMailAccountAddress, messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                    End If
                Else
                    'Console/windows application
                    If messageSubject = Nothing Then messageSubject = "Error @ " & System.Environment.MachineName
                    If _WebManager.IsSupported.Messaging.eMailQueue Then 'try first using the queue because the client workstation is not trusted to be able to send e-mails directly in all situations/firewall configurations
                        If CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.TechnicalContactAndDeveloper Then
                            _WebManager.MessagingEMails.QueueEMail(Messaging.EMails.CreateReceipientString(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress) & "," & _WebManager.TechnicalServiceEMailAccountAddress, "", "", messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                        ElseIf CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.Developer Then
                            _WebManager.MessagingEMails.QueueEMail(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress, messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                        Else
                            _WebManager.MessagingEMails.QueueEMail(_WebManager.TechnicalServiceEMailAccountName, _WebManager.TechnicalServiceEMailAccountAddress, messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                        End If
                    ElseIf _WebManager.IsSupported.Messaging.eMail Then
                        If CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.TechnicalContactAndDeveloper Then
                            _WebManager.MessagingEMails.SendEMail(Messaging.EMails.CreateReceipientString(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress) & "," & _WebManager.TechnicalServiceEMailAccountAddress, "", "", messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                        ElseIf CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.Developer Then
                            _WebManager.MessagingEMails.SendEMail(_WebManager.DevelopmentEMailAccountAddress, _WebManager.DevelopmentEMailAccountAddress, messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                        Else
                            _WebManager.MessagingEMails.SendEMail(_WebManager.TechnicalServiceEMailAccountName, _WebManager.TechnicalServiceEMailAccountAddress, messageSubject, plainText, htmlText, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress)
                        End If
                    Else
                        'No mailing system available
                        Result = False
                    End If
                End If
            Catch
                Result = False
            End Try
            Log.WriteEventLogTrace("ReportErrorByEMail2:End")
            Return Result
        End Function

        ''' <summary>
        '''     Prepare the message text for the exception in plain text format
        ''' </summary>
        ''' <param name="exceptionDetails"></param>
        ''' <param name="exceptionIdentifier"></param>
        Friend Shared Function BuildPlainMessage(ByVal exceptionDetails As String, ByVal exceptionIdentifier As String, exceptionGuid As String, ByVal request As System.Web.HttpRequest, ByVal context As System.Web.HttpContext, ByVal user As System.Security.Principal.IPrincipal, ByVal webManager As WebManager.WMSystem, errorCounter As Integer) As String

            Dim ContextRequest As HttpRequest = Nothing
            If Not context Is Nothing Then
                Try
                    ContextRequest = context.Request
                Catch ex As HttpException
                    'Integrated mode of IIS 7 throw an HttpException in Application_Init event because the request/response objects haven't been available, yet
                    ContextRequest = Nothing
                End Try
            End If

            'For safety
            If request Is Nothing AndAlso Not context Is Nothing Then
                request = ContextRequest
            End If

            If exceptionIdentifier = "" Then
                If Not context Is Nothing Then
                    exceptionIdentifier = "Page Error"
                Else
                    exceptionIdentifier = "Error in CWM client application"
                End If
            End If

            Dim strMessage As New Text.StringBuilder
            strMessage.Append(exceptionIdentifier & vbNewLine)
            strMessage.Append(StrDup(exceptionIdentifier.Length, "="c) & vbNewLine)
            strMessage.Append(vbNewLine)
            If Not request Is Nothing Then
                strMessage.Append("Page URL: ")
                strMessage.Append(request.Url.AbsoluteUri & vbNewLine)

                'HTTP_Referer
                strMessage.Append("Referer: " & request.Headers("REFERER") & vbNewLine)
            Else
                strMessage.Append("Code location: ")
                strMessage.Append(Log.GetCodeLocation())
            End If

            'CWM / CurrentServerIdentString
            If Not webManager Is Nothing Then
                strMessage.Append("ServerIdentString: ")
                strMessage.Append(Utils.StringNotNothingOrAlternativeValue(webManager.CurrentServerIdentString, "{not assigned yet}"))
            End If

            'CWM / ExceptionGuid
            If True Then
                strMessage.Append("ExceptionToken: ")
                strMessage.Append(Utils.StringNotNothingOrAlternativeValue(exceptionGuid, "{N/A}"))
                strMessage.Append("ExceptionCounter (within last max. 10 minutes): ")
                strMessage.Append(errorCounter)
            End If

            'CWM / Authenticated user
            If Not context Is Nothing AndAlso Not context.Session Is Nothing AndAlso CType(context.Session("System_Username"), String) <> Nothing Then
                strMessage.Append("CWM user: " & CType(context.Session("System_Username"), String) & ControlChars.CrLf)
            ElseIf Not context Is Nothing AndAlso Not webManager Is Nothing AndAlso webManager.CurrentServerIdentString <> Nothing Then
                'Try to identify the user by his cookie data
                Try
                    Dim username As String
                    username = webManager.LookupUserNameByScriptEngineSessionID(webManager.CurrentServerInfo.ID, WMSystem.ScriptEngines.ASPNet, webManager.CurrentScriptEngineSessionID)
                    strMessage.Append("CWM user (reported by auth. cookie): " & username & ControlChars.CrLf)
                Catch
                    'Ignore all errors
                End Try
            ElseIf context Is Nothing AndAlso Not webManager Is Nothing AndAlso webManager.CurrentUserLoginName <> Nothing Then 'non-web-environment (no httpcontext)
                strMessage.Append("CWM user: " & webManager.CurrentUserLoginName & ControlChars.CrLf)
            ElseIf Not webManager Is Nothing AndAlso webManager.DebugLevel >= DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                strMessage.Append("CWM user: {not avilable in this context: HC:" & (Not context Is Nothing) & "|WM:" & (Not webManager Is Nothing) & "|SID:" & webManager.CurrentServerIdentStringNoAutoLookup & "}" & ControlChars.CrLf)
            Else
                strMessage.Append("CWM user: {not avilable in this context: HC:" & (Not context Is Nothing) & "|WM:" & (Not webManager Is Nothing) & "}" & ControlChars.CrLf)
            End If

            'IIS/ASP.Net / Authenticated user
            If Not user Is Nothing AndAlso Not user.Identity Is Nothing AndAlso user.Identity.IsAuthenticated Then
                strMessage.Append("Authenticated user: " & user.Identity.Name & ControlChars.CrLf)
            End If

            'Client/request details
            If Not request Is Nothing Then
                strMessage.Append("Remote IP Address: ")
                strMessage.Append(request.UserHostAddress & vbNewLine)
                Try
                    If request.UserHostAddress <> Utils.LookupRealRemoteClientIPOfHttpContext(context) Then
                        strMessage.Append("Remote IP Address behind the reverse proxy: ")
                        strMessage.Append(Utils.LookupRealRemoteClientIPOfHttpContext(context) & vbNewLine)
                    End If
                Catch ex As Exception
                    strMessage.Append("Remote IP Address behind the reverse proxy: ")
                    strMessage.Append(ex.Message & vbNewLine)
                End Try
                strMessage.Append("User Agent: ")
                strMessage.Append(request.UserAgent & vbNewLine)
                strMessage.Append("User Agent classification: ")
                strMessage.Append(UserAgentClassification(request) & vbNewLine)
            Else
                strMessage.Append("Server machine Name: ")
                strMessage.Append(System.Environment.MachineName() & vbNewLine)
                strMessage.Append("Server OS Version: ")
                strMessage.Append(System.Environment.OSVersion.ToString & vbNewLine)
            End If
            strMessage.Append("Time: ")
            strMessage.Append(System.DateTime.Now & vbNewLine)

            'The exception itself
            strMessage.Append("Details: " & vbNewLine)
            strMessage.Append(exceptionDetails)
            strMessage.Append(ControlChars.CrLf)
            strMessage.Append(ControlChars.CrLf)

            If Not context Is Nothing AndAlso Not request Is Nothing Then
                ' Gathering QueryString information 
                strMessage.Append("QueryString Data:" + ControlChars.CrLf + "----------" + ControlChars.CrLf)
                If request.QueryString.Count = 0 Then
                    strMessage.Append("{Empty request querystring collection}" + ControlChars.CrLf)
                Else
                    For i As Integer = 0 To request.QueryString.Count - 1
                        strMessage.Append(request.QueryString.Keys(i) + "=" + request.QueryString(i) + ControlChars.CrLf)
                    Next
                End If
                strMessage.Append(ControlChars.CrLf)

                ' Gathering Post Data information 
                strMessage.Append("Post Data:" + ControlChars.CrLf + "----------" + ControlChars.CrLf)
                If request.Form.Count = 0 Then
                    strMessage.Append("{Empty form collection}" + ControlChars.CrLf)
                Else
                    For i As Integer = 0 To request.Form.Count - 1
                        If request.Form.Keys(i).ToUpper = "__VIEWSTATE" Then
                            strMessage.Append(request.Form.Keys(i) + "=")
                            If request.Form(i) Is Nothing Then
                                strMessage.Append("{Length=0}")
                            Else
                                strMessage.Append("{Length=" & request.Form(i).Length & "}")
                            End If
                            strMessage.Append(ControlChars.CrLf)
                        ElseIf request.Form(i) Is Nothing Then
                            strMessage.Append(request.Form.Keys(i) + "=")
                            strMessage.Append("{null/Nothing}")
                            strMessage.Append(ControlChars.CrLf)
                        ElseIf request.Form(i) = Nothing Then
                            strMessage.Append(request.Form.Keys(i) + "=")
                            strMessage.Append("{Length=0}")
                            strMessage.Append(ControlChars.CrLf)
                        ElseIf Not request.Form(i) Is Nothing AndAlso request.Form(i).Length > 10000 Then
                            'Fields with more than 10K characters are considered as too long
                            strMessage.Append(request.Form.Keys(i) + "=")
                            strMessage.Append("{Length=" & request.Form(i).Length & "}")
                            strMessage.Append(ControlChars.CrLf)
                        Else
                            strMessage.Append(request.Form.Keys(i) + "=" + request.Form(i) + ControlChars.CrLf)
                        End If
                    Next
                End If
                strMessage.Append(ControlChars.CrLf)

                ' Gathering Server Variables information 
                strMessage.Append("Server Variables:" + ControlChars.CrLf + "-----------------" + ControlChars.CrLf)
                For i As Integer = 0 To request.ServerVariables.Count - 1
                    If (request.ServerVariables(i) = Nothing OrElse request.ServerVariables.Keys(i).StartsWith("ALL_")) AndAlso CompuMaster.camm.WebManager.Controls.cammWebManager.Configuration.DebugLevel < WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        'do not show empty lines
                    Else
                        'show content / show empty lines with higher debug levels
                        strMessage.Append(request.ServerVariables.Keys(i) + "=" + request.ServerVariables(i) + ControlChars.CrLf)
                    End If
                Next
                strMessage.Append(ControlChars.CrLf)

                ' Gathering HttpCache Variables information 
                strMessage.Append("HttpCache Variables:" + ControlChars.CrLf + "--------------------" + ControlChars.CrLf)
                Dim strHttpCacheVariables As New Text.StringBuilder
                If Not context.Cache Is Nothing Then
                    If context.Cache.Count = 0 Then
                        strHttpCacheVariables.Append("{Empty cache items collection}" + ControlChars.CrLf)
                    Else
                        Try
                            For Each key As System.Collections.DictionaryEntry In context.Cache
                                strHttpCacheVariables.Append(System.Web.HttpUtility.HtmlEncode(CType(key.Key, String)) + "=" + System.Web.HttpUtility.HtmlEncode(Mid(Utils.ObjectNotNothingOrEmptyString(key.Value).ToString, 1, 200)) & ControlChars.CrLf)
                            Next
                        Catch ex As Exception
                            strHttpCacheVariables.Append(ex.ToString)
                        End Try
                    End If
                Else
                    strHttpCacheVariables.Append("{Cache object is null (Nothing in VisualBasic)}")
                End If
                strMessage.Append(ControlChars.CrLf)

                ' Gathering Session Variables information 
                strMessage.Append("Session Variables:" + ControlChars.CrLf + "------------------" + ControlChars.CrLf)
                Dim strSessionVariables As New Text.StringBuilder
                If Not context.Session Is Nothing Then
                    If context.Session.Count = 0 Then
                        strSessionVariables.Append("{Empty session items collection}" + ControlChars.CrLf)
                    Else
                        Dim key As String = Nothing
                        Try
                            For Each key In context.Session
                                strSessionVariables.Append(key + "=" + Utils.ObjectNotNothingOrEmptyString(context.Session.Item(key)).ToString & ControlChars.CrLf)
                            Next
                        Catch ex As Exception
                            strSessionVariables.Append(key & "={Error}" & ex.ToString)
                        End Try
                    End If
                Else
                    strSessionVariables.Append("{Session object is null (Nothing in VisualBasic)}")
                End If
                strMessage.Append(ControlChars.CrLf)

                ' Gathering request header information 
                strMessage.Append("Request headers:" + ControlChars.CrLf + "----------------" + ControlChars.CrLf)
                Dim strRequestHeaders As New Text.StringBuilder
                If Not request Is Nothing AndAlso Not request.Headers Is Nothing Then
                    If request.Headers.Count = 0 Then
                        strRequestHeaders.Append("{Empty request headers collection}" + ControlChars.CrLf)
                    Else
                        Dim key As String = Nothing
                        Try
                            For Each key In request.Headers
                                strRequestHeaders.Append(key + "=" + Utils.ObjectNotNothingOrEmptyString(request.Headers.Item(key)).ToString & ControlChars.CrLf)
                            Next
                        Catch ex As Exception
                            strRequestHeaders.Append(ex.ToString)
                        End Try
                    End If
                Else
                    strRequestHeaders.Append("{Request headers object is null (Nothing in VisualBasic)}")
                End If
                strMessage.Append(ControlChars.CrLf)
            End If

            If Not context Is Nothing Then
                'Server environment
                strMessage.Append("Server name: ")
                strMessage.Append(System.Environment.MachineName & vbNewLine)
                If CompuMaster.camm.WebManager.Utils.GetWorkstationID <> System.Environment.MachineName Then
                    strMessage.Append("Server network IP: ")
                    strMessage.Append(CompuMaster.camm.WebManager.Utils.GetWorkstationID & vbNewLine)
                End If
            Else
                'Workstation environment
                strMessage.Append("Workstation name: ")
                strMessage.Append(System.Environment.MachineName & vbNewLine)
                If CompuMaster.camm.WebManager.Utils.GetWorkstationID <> System.Environment.MachineName Then
                    strMessage.Append("Workstation network IP: ")
                    strMessage.Append(CompuMaster.camm.WebManager.Utils.GetWorkstationID & vbNewLine)
                End If
            End If

            'CWM environment
            strMessage.Append("System.Environment.Version: ")
            strMessage.Append(System.Environment.Version.ToString & vbNewLine)
            strMessage.Append("camm Web-Manager Version: ")
            strMessage.Append(CompuMaster.camm.WebManager.Setup.ApplicationUtils.Version.ToString & vbNewLine)


            Return strMessage.ToString

        End Function

        ''' <summary>
        '''     Prepare the message text for the exception in HTML output format
        ''' </summary>
        ''' <param name="exceptionDetailsAsHtml">Details on the exception in valid HTML code (and without body or script tags or similar)</param>
        ''' <param name="exceptionIdentifier"></param>
        Friend Shared Function BuildHtmlMessage(ByVal exceptionDetailsAsHtml As String, ByVal exceptionIdentifier As String, exceptionGuid As String, ByVal request As System.Web.HttpRequest, ByVal context As System.Web.HttpContext, ByVal user As System.Security.Principal.IPrincipal, ByVal webManager As WebManager.WMSystem, errorCounter As Integer) As String

            Dim ContextRequest As HttpRequest = Nothing
            If Not context Is Nothing Then
                Try
                    ContextRequest = context.Request
                Catch ex As HttpException
                    'Integrated mode of IIS 7 throw an HttpException in Application_Init event because the request/response objects haven't been available, yet
                    ContextRequest = Nothing
                End Try
            End If

            'For safety
            If request Is Nothing AndAlso Not context Is Nothing Then
                request = ContextRequest
            End If

            If exceptionIdentifier = "" Then
                If Not context Is Nothing Then
                    exceptionIdentifier = "Page Error"
                Else
                    exceptionIdentifier = "Error in CWM client application"
                End If
            End If

            Dim strMessage As New Text.StringBuilder
            strMessage.Append("<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">")
            strMessage.Append("<html>")
            strMessage.Append("<head>")
            strMessage.Append("<title>" & System.Web.HttpUtility.HtmlEncode(exceptionIdentifier) & "</title>")
            strMessage.Append("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">")
            strMessage.Append("<style type=""text/css"">")
            strMessage.Append("<!--")
            strMessage.Append(".basix {")
            strMessage.Append("font-family: Verdana, Arial, Helvetica, sans-serif;")
            strMessage.Append("font-size: 12px;")
            strMessage.Append("}")
            strMessage.Append(".header1 {")
            strMessage.Append("font-family: Verdana, Arial, Helvetica, sans-serif;")
            strMessage.Append("font-size: 12px;")
            strMessage.Append("font-weight: bold;")
            strMessage.Append("color: #000099;")
            strMessage.Append("}")
            strMessage.Append(".tlbbkground1 {")
            strMessage.Append("background-color:  #000099;")
            strMessage.Append("}")
            strMessage.Append("-->")
            strMessage.Append("</style>")
            strMessage.Append("</head>")
            strMessage.Append("<body>")
            strMessage.Append("<table width=""85%"" border=""0"" align=""center"" cellpadding=""5"" cellspacing=""1"" class=""tlbbkground1"">")
            strMessage.Append("<tr bgcolor=""#eeeeee"">")
            strMessage.Append("<td valign=""top"" colspan=""2"" class=""header1""><h3>" & System.Web.HttpUtility.HtmlEncode(exceptionIdentifier) & "</h3></td>")
            strMessage.Append("</tr>")
            If Not request Is Nothing Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Page</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(request.Url.AbsoluteUri) & "</td>")
                strMessage.Append("</tr>")

                'HTTP_Referer
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Referer</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(request.Headers("REFERER")) & "</td>")
                strMessage.Append("</tr>")
            Else
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Code Location</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & Log.GetCodeLocation() & "</td>")
                strMessage.Append("</tr>")
            End If

            'CWM / CurrentServerIdentString
            If Not webManager Is Nothing Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>ServerIdentString</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(Utils.StringNotNothingOrAlternativeValue(webManager.CurrentServerIdentString, "{not assigned yet}")) & "</td>")
                strMessage.Append("</tr>")
            End If

            'CWM / ExceptionGuid
            If True Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>ExceptionToken</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(Utils.StringNotNothingOrAlternativeValue(exceptionGuid, "{N/A}")) & "</td>")
                strMessage.Append("</tr>")
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>ExceptionCounter (within last max. 10 minutes)</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(errorCounter.ToString) & "</td>")
                strMessage.Append("</tr>")
            End If

            'CWM / Authenticated user
            If Not context Is Nothing AndAlso Not context.Session Is Nothing AndAlso CType(context.Session("System_Username"), String) <> Nothing Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(CType(context.Session("System_Username"), String)) & "</td>")
                strMessage.Append("</tr>")
            ElseIf Not context Is Nothing AndAlso Not webManager Is Nothing AndAlso webManager.CurrentServerIdentString <> Nothing Then
                'Try to identify the user by his cookie data
                Try
                    Dim username As String
                    username = webManager.LookupUserNameByScriptEngineSessionID(webManager.CurrentServerInfo.ID, WMSystem.ScriptEngines.ASPNet, webManager.CurrentScriptEngineSessionID)
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user (reported by auth. cookie)</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(username) & "</td>")
                    strMessage.Append("</tr>")
                Catch ex As Exception
                    'Ignore all errors
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode("{" & ex.Message & "}") & "</td>")
                    strMessage.Append("</tr>")
                End Try
            ElseIf Not context Is Nothing AndAlso Not webManager Is Nothing Then
                'Try to identify the user by his cookie data - lesser safe lookup without server ID
                Try
                    Dim username As String
                    username = webManager.LookupUserNameByScriptEngineSessionID(WMSystem.ScriptEngines.ASPNet, webManager.CurrentScriptEngineSessionID)
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user (reported by auth. cookie, semi-trusted)</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(username) & "</td>")
                    strMessage.Append("</tr>")
                Catch ex As Exception
                    'Ignore all errors
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode("{" & ex.Message & "}") & "</td>")
                    strMessage.Append("</tr>")
                End Try
            ElseIf context Is Nothing AndAlso Not webManager Is Nothing AndAlso webManager.CurrentUserLoginName <> Nothing Then 'non-web-environment (no httpcontext)
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(webManager.CurrentUserLoginName) & "</td>")
                strMessage.Append("</tr>")
            ElseIf Not webManager Is Nothing AndAlso webManager.DebugLevel >= DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix""><em>{not available in this context: HC:" & (Not context Is Nothing) & "|WM:" & (Not webManager Is Nothing) & "|SID:" & webManager.CurrentServerIdentStringNoAutoLookup & "}</em></td>")
                strMessage.Append("</tr>")
            Else
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>CWM user</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix""><em>{not available in this context: HC:" & (Not context Is Nothing) & "|WM:" & (Not webManager Is Nothing) & "}</em></td>")
                strMessage.Append("</tr>")
            End If

            'IIS/ASP.Net / Authenticated user
            If Not user Is Nothing AndAlso Not user.Identity Is Nothing AndAlso user.Identity.IsAuthenticated Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Authenticated user</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(user.Identity.Name) & "</td>")
                strMessage.Append("</tr>")
            End If

            'Client/request details
            If Not request Is Nothing Then
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Remote IP Address</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(request.UserHostAddress) & "</td>")
                strMessage.Append("</tr>")
                Try
                    If request.UserHostAddress <> Utils.LookupRealRemoteClientIPOfHttpContext(context) Then
                        strMessage.Append("<tr>")
                        strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Remote IP Address behind the reverse proxy</td>")
                        strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(Utils.LookupRealRemoteClientIPOfHttpContext(context)) & "</td>")
                        strMessage.Append("</tr>")
                    End If
                Catch ex As Exception
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Remote IP Address behind the reverse proxy</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(ex.Message) & "</td>")
                    strMessage.Append("</tr>")
                End Try
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>User Agent</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(request.UserAgent) & "</td>")
                strMessage.Append("</tr>")
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>User Agent classification</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(UserAgentClassification(request)) & "</td>")
                strMessage.Append("</tr>")
            Else
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Server machine name</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(System.Environment.MachineName) & "</td>")
                strMessage.Append("</tr>")
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Server OS Version</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(System.Environment.OSVersion.ToString) & "</td>")
                strMessage.Append("</tr>")
            End If
            strMessage.Append("<tr>")
            strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Time</td>")
            strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(System.DateTime.Now.ToString) & "</td>")
            strMessage.Append("</tr>")

            'The exception itself
            strMessage.Append("<tr>")
            strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Details</td>")
            strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & exceptionDetailsAsHtml & "</td>")
            strMessage.Append("</tr>")

            If Not context Is Nothing AndAlso Not request Is Nothing Then
                ' Gathering QueryString information 
                Dim htmlQueryStrings As New Text.StringBuilder
                If request.QueryString.Count = 0 Then
                    htmlQueryStrings.Append("<em>Empty request querystring collection</em><br>")
                Else
                    For i As Integer = 0 To request.QueryString.Count - 1
                        htmlQueryStrings.Append(System.Web.HttpUtility.HtmlEncode(request.QueryString.Keys(i)) & "=" & System.Web.HttpUtility.HtmlEncode(request.QueryString(i)) & "<br>")
                    Next
                End If
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>QueryString Data</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & htmlQueryStrings.ToString & "</td>")
                strMessage.Append("</tr>")

                ' Gathering Post Data information 
                Dim htmlPostData As New Text.StringBuilder
                If request.Form.Count = 0 Then
                    htmlPostData.Append("<em>Empty form collection</em><br>")
                Else
                    For i As Integer = 0 To request.Form.Count - 1
                        If request.Form.Keys(i).ToUpper = "__VIEWSTATE" Then
                            htmlPostData.Append(System.Web.HttpUtility.HtmlEncode(request.Form.Keys(i)) & "=")
                            If request.Form(i) Is Nothing Then
                                htmlPostData.Append("{Length=0}")
                            Else
                                htmlPostData.Append("{Length=" & request.Form(i).Length & "}")
                            End If
                            htmlPostData.Append("<br>")
                        ElseIf request.Form(i) Is Nothing Then
                            htmlPostData.Append(System.Web.HttpUtility.HtmlEncode(request.Form.Keys(i)) & "=")
                            htmlPostData.Append("{null/Nothing}")
                            htmlPostData.Append("<br>")
                        ElseIf request.Form(i) = Nothing Then
                            htmlPostData.Append(System.Web.HttpUtility.HtmlEncode(request.Form.Keys(i)) & "=")
                            htmlPostData.Append("{Length=0}")
                            htmlPostData.Append("<br>")
                        ElseIf Not request.Form(i) Is Nothing AndAlso request.Form(i).Length > 10000 Then
                            'Fields with more than 10K characters are considered as too long
                            htmlPostData.Append(System.Web.HttpUtility.HtmlEncode(request.Form.Keys(i)) & "=")
                            htmlPostData.Append("{Length=" & request.Form(i).Length & "}")
                            htmlPostData.Append("<br>")
                        Else
                            htmlPostData.Append(System.Web.HttpUtility.HtmlEncode(request.Form.Keys(i)) & "=" & System.Web.HttpUtility.HtmlEncode(request.Form(i)) & "<br>")
                        End If
                    Next
                End If
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Post Data</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & htmlPostData.ToString & "</td>")
                strMessage.Append("</tr>")

                ' Gathering Server Variables information 
                Dim strServerVariables As New Text.StringBuilder
                For i As Integer = 0 To request.ServerVariables.Count - 1
                    If (request.ServerVariables(i) = Nothing OrElse request.ServerVariables.Keys(i).StartsWith("ALL_")) AndAlso CompuMaster.camm.WebManager.Controls.cammWebManager.Configuration.DebugLevel < WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        'do not show empty lines
                    Else
                        'show content / show empty lines with higher debug levels
                        strServerVariables.Append(System.Web.HttpUtility.HtmlEncode(request.ServerVariables.Keys(i)) & "=" & System.Web.HttpUtility.HtmlEncode(request.ServerVariables(i)) & "<br>")
                    End If
                Next
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Server Variables</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & strServerVariables.ToString & "</td>")
                strMessage.Append("</tr>")

                ' Gathering HttpCache Variables information 
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>HttpCache Variables</td>")
                Dim strHttpCacheVariables As New Text.StringBuilder
                If Not context.Cache Is Nothing Then
                    If context.Cache.Count = 0 Then
                        strHttpCacheVariables.Append("<em>Empty cache items collection</em><br>")
                    Else
                        Try
                            For Each key As System.Collections.DictionaryEntry In context.Cache
                                strHttpCacheVariables.Append(System.Web.HttpUtility.HtmlEncode(CType(key.Key, String)) & "=" & System.Web.HttpUtility.HtmlEncode(Mid(Utils.ObjectNotNothingOrEmptyString(key.Value).ToString, 1, 200)) & "<br>")
                            Next
                        Catch ex As Exception
                            strHttpCacheVariables.Append(ex.ToString)
                        End Try
                    End If
                Else
                    strHttpCacheVariables.Append("<em>Cache object is null (Nothing in VisualBasic)</em>")
                End If
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & strHttpCacheVariables.ToString & "</td>")
                strMessage.Append("</tr>")

                ' Gathering Session Variables information 
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Session Variables</td>")
                Dim strSessionVariables As New Text.StringBuilder
                If Not context.Session Is Nothing Then
                    If context.Session.Count = 0 Then
                        strSessionVariables.Append("<em>Empty session items collection</em><br>")
                    Else
                        Dim key As String = Nothing
                        Try
                            For Each key In context.Session
                                strSessionVariables.Append(key & "=" & Utils.ObjectNotNothingOrEmptyString(context.Session.Item(key)).ToString & "<br>")
                            Next
                        Catch ex As Exception
                            strSessionVariables.Append(key & "={Error}" & ex.ToString)
                        End Try
                    End If
                Else
                    strSessionVariables.Append("<em>Session object is null (Nothing in VisualBasic)</em>")
                End If
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & strSessionVariables.ToString & "</td>")
                strMessage.Append("</tr>")

                ' Gathering request headers information 
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Request headers</td>")
                Dim strRequestHeaders As New Text.StringBuilder
                If Not request Is Nothing AndAlso Not request.Headers Is Nothing Then
                    If request.Headers.Count = 0 Then
                        strRequestHeaders.Append("<em>Empty request headers collection</em><br>")
                    Else
                        Try
                            For Each key As String In request.Headers
                                strRequestHeaders.Append(System.Web.HttpUtility.HtmlEncode(key) & "=" & System.Web.HttpUtility.HtmlEncode(Utils.ObjectNotNothingOrEmptyString(request.Headers.Item(key)).ToString) & "<br>")
                            Next
                        Catch ex As Exception
                            strRequestHeaders.Append(ex.ToString)
                        End Try
                    End If
                Else
                    strRequestHeaders.Append("<em>Request object is null (Nothing in VisualBasic)</em>")
                End If
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & strRequestHeaders.ToString & "</td>")
                strMessage.Append("</tr>")
            End If

            If Not context Is Nothing Then
                'Server environment
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Server name</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(System.Environment.MachineName) & "</td>")
                strMessage.Append("</tr>")
                If CompuMaster.camm.WebManager.Utils.GetWorkstationID <> System.Environment.MachineName Then
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Server network IP</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(CompuMaster.camm.WebManager.Utils.GetWorkstationID) & "</td>")
                    strMessage.Append("</tr>")
                End If
            Else
                'Workstation environment
                strMessage.Append("<tr>")
                strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Workstation name</td>")
                strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(System.Environment.MachineName) & "</td>")
                strMessage.Append("</tr>")
                If CompuMaster.camm.WebManager.Utils.GetWorkstationID <> System.Environment.MachineName Then
                    strMessage.Append("<tr>")
                    strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>Workstation network IP</td>")
                    strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(CompuMaster.camm.WebManager.Utils.GetWorkstationID) & "</td>")
                    strMessage.Append("</tr>")
                End If
            End If

            'CWM environment
            strMessage.Append("<tr>")
            strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>System.Environment.Version</td>")
            strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(System.Environment.Version.ToString) & "</td>")
            strMessage.Append("</tr>")
            strMessage.Append("<tr>")
            strMessage.Append("<td valign=""top"" width=""200"" align=""right"" bgcolor=""#eeeeee"" class=""header1"" nowrap>camm Web-Manager Version</td>")
            strMessage.Append("<td valign=""top"" bgcolor=""#FFFFFF"" class=""basix"">" & System.Web.HttpUtility.HtmlEncode(CompuMaster.camm.WebManager.Setup.ApplicationUtils.Version.ToString) & "</td>")
            strMessage.Append("</tr>")

            strMessage.Append("</table>")
            strMessage.Append("</body>")
            strMessage.Append("</html>")
            Return strMessage.ToString
        End Function

        ''' <summary>
        ''' Classifies the user agent types based on crawler/potential machine agent information
        ''' </summary>
        ''' <param name="request"></param>
        ''' <remarks></remarks>
        Private Shared Function UserAgentClassification(ByVal request As HttpRequest) As String
            Return UserAgentClassification(Utils.IsRequestFromCrawlerAgent(request), Utils.IsRequestFromCrawlerOrPotentialMachineAgent(request))
        End Function

        ''' <summary>
        ''' Classifies the user agent types based on crawler/potential machine agent information
        ''' </summary>
        ''' <param name="isCrawler"></param>
        ''' <param name="potentiallyIsCrawler"></param>
        ''' <remarks></remarks>
        Private Shared Function UserAgentClassification(ByVal isCrawler As Boolean, ByVal potentiallyIsCrawler As Boolean) As String
            If isCrawler Then
                Return "Crawler/robot"
            ElseIf potentiallyIsCrawler Then
                Return "Machine/application agent (potentially)"
            Else
                Return "Request by user"
            End If
        End Function

#End Region

#Region "Methods for applications"
        ''' <summary>
        '''     Create an information log entry in camm Web-Manager's event log
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="requiredDebugLevel">The required debug level before this message gets logged</param>
        Public Sub Write(ByVal message As String, Optional ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels = DebugLevels.NoDebug)
            WriteLogItem(message, Logging_ConflictTypes.ApplicationInformation, requiredDebugLevel)
        End Sub
        ''' <summary>
        '''     Create a warning log entry in camm Web-Manager's event log
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="requiredDebugLevel">The required debug level before this message gets logged</param>
        Public Sub Warn(ByVal message As String, Optional ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels = DebugLevels.NoDebug)
            WriteLogItem(message, Logging_ConflictTypes.ApplicationWarning, requiredDebugLevel)
        End Sub
        ''' <summary>
        '''     Create a warning log entry in camm Web-Manager's event log and do not throw the exception
        ''' </summary>
        ''' <param name="exception"></param>
        Public Sub Warn(ByVal exception As Exception)
            Dim ExDetails As String = exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(exception)
            If AdditionalExceptionDetails <> Nothing Then
                ExDetails &= vbNewLine & AdditionalExceptionDetails
            End If

            WriteLogItem(ExDetails, Logging_ConflictTypes.ApplicationException, DebugLevels.NoDebug, False)
        End Sub
        ''' <summary>
        '''     Create a warning log entry in camm Web-Manager's event log and do not throw the exception
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="stackTrace">A stacktrace for tracking the error or Nothing for auto-generation of StackTrace</param>
        Public Sub Warn(ByVal message As String, ByVal stackTrace As String)
            Me.Warn(message, stackTrace, DebugLevels.NoDebug, False, False)
        End Sub
        ''' <summary>
        '''     Create a warning log entry in camm Web-Manager's event log and do not throw the exception
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="stackTrace">A stacktrace for tracking the error or Nothing for auto-generation of StackTrace</param>
        ''' <param name="neverSendWarningMails"></param>
        ''' <param name="abortRequest"></param>
        Friend Sub Warn(ByVal message As String, ByVal stackTrace As String, ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels, ByVal neverSendWarningMails As Boolean, ByVal abortRequest As Boolean)
            If stackTrace Is Nothing Then
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                stackTrace = WorkaroundStackTrace
            End If
            WriteLogItem(message & vbNewLine & stackTrace, Logging_ConflictTypes.ApplicationWarning, requiredDebugLevel, neverSendWarningMails)
            If abortRequest = True Then
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    'Console, windows and webservice applications
                    If message = Nothing Then
                        Throw New SystemException
                    Else
                        Throw New SystemException(message)
                    End If
                Else
                    _WebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
            End If
        End Sub
        ''' <summary>
        '''     Create an error log entry in camm Web-Manager's event log and optionally throw it
        ''' </summary>
        ''' <param name="exception">An exception which shall be logged</param>
        ''' <param name="throwException">Throw the exception or ignore it</param>
        Public Sub Exception(ByVal exception As Exception, Optional ByVal throwException As Boolean = True)
            Dim ExDetails As String = exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(exception)
            If AdditionalExceptionDetails <> Nothing Then
                ExDetails &= vbNewLine & AdditionalExceptionDetails
            End If

            WriteLogItem(ExDetails, Logging_ConflictTypes.ApplicationException, DebugLevels.NoDebug, False)
            If throwException = True Then
                Throw New SystemException("Exception logged by camm Web-Manager", exception)
            End If
        End Sub
        ''' <summary>
        '''     Create an error log entry in camm Web-Manager's event log and optionally throw it
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="throwException">Throw the exception or ignore it</param>
        Public Sub Exception(ByVal message As String, Optional ByVal throwException As Boolean = True)
            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
            Dim WorkaroundEx As New Exception("")
            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
            Try
                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
            Catch
            End Try
            WriteLogItem(message & vbNewLine & WorkaroundStackTrace.ToString, Logging_ConflictTypes.ApplicationException, DebugLevels.NoDebug)
            If throwException = True Then
                If message = Nothing Then
                    Throw New SystemException
                Else
                    Throw New SystemException(message)
                End If
            End If
        End Sub
#End Region

#Region "Runtime methods"
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and do not throw it
        ''' </summary>
        ''' <param name="message">The message which has to be logged</param>
        ''' <param name="requiredDebugLevel">Logging will happen if this debug level is set.</param>
        ''' <param name="neverSendWarningMails">If set to false, there will be an e-mail notification to the TechnicalService contact</param>
        Friend Sub RuntimeInformation(ByVal message As String, Optional ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels = DebugLevels.NoDebug, Optional ByVal neverSendWarningMails As Boolean = False)
            WriteLogItem(message, Logging_ConflictTypes.RuntimeInformation, requiredDebugLevel, neverSendWarningMails)
        End Sub
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and do not throw it
        ''' </summary>
        ''' <param name="Message">The message which has to be logged</param>
        ''' <param name="stackTrace">The exception StackTrace or System.Environment.StackTrace</param>
        Friend Sub RuntimeWarning(ByVal message As String, ByVal stackTrace As String)
            Me.RuntimeWarning(message, stackTrace, DebugLevels.NoDebug, False, False)
        End Sub
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and throw it
        ''' </summary>
        ''' <param name="message">The message which has to be logged</param>
        ''' <param name="stackTrace">The exception StackTrace or System.Environment.StackTrace</param>
        ''' <param name="requiredDebugLevel">Logging will happen if this debug level is set.</param>
        ''' <param name="neverSendWarningMails">If set to false, there will be an e-mail notification to the TechnicalService contact</param>
        ''' <param name="abortRequest"></param>
        Friend Sub RuntimeWarning(ByVal message As String, ByVal stackTrace As String, ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels, ByVal neverSendWarningMails As Boolean, ByVal abortRequest As Boolean)
            If stackTrace Is Nothing Then
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                stackTrace = WorkaroundStackTrace
            End If
            WriteLogItem(message & vbNewLine & stackTrace, Logging_ConflictTypes.RuntimeWarning, requiredDebugLevel, neverSendWarningMails)
            If abortRequest = True Then
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    'Console, windows and webservice applications
                    If message = Nothing Then
                        Throw New Exception
                    Else
                        Throw New Exception(message)
                    End If
                Else
                    _WebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
            End If
        End Sub
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and do not throw it
        ''' </summary>
        ''' <param name="Exception"></param>
        ''' <param name="neverSendWarningMails"></param>
        Friend Sub RuntimeWarning(ByVal Exception As Exception, Optional ByVal neverSendWarningMails As Boolean = False)
            Dim ExDetails As String = Exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(Exception)
            If AdditionalExceptionDetails <> Nothing Then
                ExDetails &= vbNewLine & AdditionalExceptionDetails
            End If

            WriteLogItem(ExDetails, Logging_ConflictTypes.RuntimeWarning, DebugLevels.NoDebug, neverSendWarningMails)
        End Sub
        ''' <summary>
        '''     Create an error log entry in camm Web-Manager's event log and do not throw it
        ''' </summary>
        ''' <param name="exception">An exception which shall be logged</param>
        ''' <param name="neverSendWarningMails">If set to false, there will be an e-mail notification to the TechnicalService contact</param>
        ''' <param name="throwException">True will throw the exception after logging, with false it will return to the calling method</param>
        ''' <param name="requiredDebugLevelBeforeLogging">Logging will happen if this debug level is set. (But throwing the exception will always happen)</param>
        Friend Sub RuntimeException(ByVal exception As Exception, ByVal neverSendWarningMails As Boolean, ByVal throwException As Boolean, ByVal requiredDebugLevelBeforeLogging As DebugLevels)
            Dim ExDetails As String = exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(exception)
            If AdditionalExceptionDetails <> Nothing Then
                ExDetails &= vbNewLine & AdditionalExceptionDetails
            End If

            WriteLogItem(ExDetails, Logging_ConflictTypes.ApplicationException, requiredDebugLevelBeforeLogging, neverSendWarningMails)
            If throwException Then
                Throw exception
            End If
        End Sub
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and throw it
        ''' </summary>
        ''' <param name="exception">An exception which shall be logged</param>
        ''' <param name="neverSendWarningMails">If set to false, there will be an e-mail notification to the TechnicalService contact</param>
        ''' <param name="abortRequestAndRedirectToAnErrorPage">Redirect the user to an error page</param>
        Friend Sub RuntimeException(ByVal exception As Exception, ByVal neverSendWarningMails As Boolean, ByVal abortRequestAndRedirectToAnErrorPage As Boolean)
            Dim ExDetails As String = exception.ToString

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(exception)
            If AdditionalExceptionDetails <> Nothing Then
                ExDetails &= vbNewLine & AdditionalExceptionDetails
            End If

            WriteLogItem(ExDetails, Logging_ConflictTypes.RuntimeException, DebugLevels.NoDebug, neverSendWarningMails)
            If abortRequestAndRedirectToAnErrorPage = True Then
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    'Console, windows and webservice applications
                    Throw New Exception("Runtime exception", exception)
                Else
                    _WebManager.RedirectToErrorPage(exception.Message, Nothing, Nothing)
                End If
            Else
                Throw exception
            End If
        End Sub
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and throw it
        ''' </summary>
        ''' <param name="message">A message which shall be logged</param>
        ''' <param name="neverSendWarningMails">If set to false, there will be an e-mail notification to the TechnicalService contact</param>
        ''' <param name="abortRequestAndRedirectToAnErrorPage">Redirect the user to an error page</param>
        Friend Sub RuntimeException(ByVal message As String, Optional ByVal neverSendWarningMails As Boolean = False, Optional ByVal abortRequestAndRedirectToAnErrorPage As Boolean = False)
            WriteLogItem(message, Logging_ConflictTypes.RuntimeException, DebugLevels.NoDebug, neverSendWarningMails)
            If abortRequestAndRedirectToAnErrorPage = True Then
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    'Console, windows and webservice applications
                    Throw New Exception(message)
                Else
                    'Web applications
                    _WebManager.RedirectToErrorPage(message, Nothing, Nothing)
                End If
            Else
                If message = Nothing Then
                    Throw New Exception
                Else
                    Throw New Exception(message)
                End If
            End If
        End Sub
        ''' <summary>
        ''' Create an error log entry in camm Web-Manager's event log and throw it
        ''' </summary>
        ''' <param name="displayMessage">A message which shall be displayed</param>
        ''' <param name="logMessage">A message which shall be logged</param>
        ''' <param name="neverSendWarningMails">If set to false, there will be an e-mail notification to the TechnicalService contact</param>
        ''' <param name="abortRequestAndRedirectToAnErrorPage"></param>
        Friend Sub RuntimeException(ByVal displayMessage As String, ByVal logMessage As String, Optional ByVal neverSendWarningMails As Boolean = False, Optional ByVal abortRequestAndRedirectToAnErrorPage As Boolean = False)
            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
            Dim WorkaroundEx As New Exception("")
            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
            Try
                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
            Catch
            End Try
            WriteLogItem(logMessage & vbNewLine & WorkaroundStackTrace.ToString, Logging_ConflictTypes.RuntimeException, DebugLevels.NoDebug, neverSendWarningMails)
            If displayMessage = Nothing Then
                displayMessage = logMessage
            End If
            If abortRequestAndRedirectToAnErrorPage = True Then
                If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then
                    'Console, windows and webservice applications
                    Throw New Exception(displayMessage)
                Else
                    _WebManager.RedirectToErrorPage(displayMessage, Nothing, Nothing)
                End If
            Else
                If displayMessage = Nothing Then
                    Throw New Exception
                Else
                    Throw New Exception(displayMessage)
                End If
            End If
        End Sub
#End Region

#Region "Write the log item"
        Private WrittenLogsSinceCwmInstanceStart As Integer = 0
        ''' <summary>
        '''     Write a log entry to the database
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="conflictType">The type of the message to get protocolled</param>
        ''' <param name="requiredDebugLevel">The required debug level before this message gets logged</param>
        ''' <param name="neverSendWarningMails">Never send warning mails even if an exception gets thrown</param>
        Friend Sub WriteLogItem(ByVal message As String, ByVal conflictType As CompuMaster.camm.WebManager.WMSystem.Logging_ConflictTypes, ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels, Optional ByVal neverSendWarningMails As Boolean = False)
            Dim Url As String = Nothing
            Try
                If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Request Is Nothing Then
                    Url = HttpContext.Current.Request.RawUrl
                End If
            Catch ex As Exception
                Throw New Exception(message, ex)
            End Try
            Me.WriteLogItem(message, conflictType, requiredDebugLevel, neverSendWarningMails, Url)
        End Sub
        ''' <summary>
        '''     Write a log entry to the database
        ''' </summary>
        ''' <param name="message">An message which shall be logged</param>
        ''' <param name="conflictType">The type of the message to get protocolled</param>
        ''' <param name="requiredDebugLevel">The required debug level before this message gets logged</param>
        ''' <param name="neverSendWarningMails">Never send warning mails even if an exception gets thrown</param>
        ''' <param name="address">The address/URL which is related to this log item</param>
        Friend Sub WriteLogItem(ByVal message As String, ByVal conflictType As CompuMaster.camm.WebManager.WMSystem.Logging_ConflictTypes, ByVal requiredDebugLevel As CompuMaster.camm.WebManager.WMSystem.DebugLevels, ByVal neverSendWarningMails As Boolean, ByVal address As String)
            Log.WriteEventLogTrace("WriteLogItem:MessageData:" & vbNewLine & message)
            Try
                If MaxRowsInLogTable > 0 Then
                    If _WebManager.DebugLevel >= requiredDebugLevel Then
                        If _WebManager.ConnectionString <> "" Then
                            Dim MyDBConn As New SqlConnection
                            Dim MyCmd As New SqlCommand
                            Dim _Message As Object
                            If message = "" Then
                                _Message = DBNull.Value
                            Else
                                _Message = message
                            End If
                            Try
                                'Increase the inmemory value
                                RowsInLogTable += 1
                                'Shorten log table if required
                                If Me.WrittenLogsSinceCwmInstanceStart > 0 AndAlso Me.WrittenLogsSinceCwmInstanceStart Mod 100 = 0 AndAlso Now.Subtract(DataLayer.Current.QueryLastServiceExecutionDate(_WebManager, Nothing)).TotalDays > 15 Then 'every 100 log-writes starting with the very first one (value = 0)
                                    Try
                                        CleanUpLogTable()
                                    Catch
                                        'Ignore any errors regarding cleanup, here
                                    End Try
                                End If

                                'Create connection
                                MyDBConn.ConnectionString = _WebManager.ConnectionString
                                MyDBConn.Open()

                                'Get parameter value and append parameter
                                With MyCmd
                                    .CommandText = "INSERT INTO [dbo].[Log] ([UserID], [LoginDate], [RemoteIP], [ServerIP], [ApplicationID], [URL], [ConflictType], [ConflictDescription]) " & _
                                        "VALUES (@UserID, GetDate(), @RemoteIP, @ServerIP, @ApplicationID, @URL, @ConflictType, @ConflictDescription)"
                                    .CommandType = CommandType.Text

                                    Dim UserID As Long
                                    Dim RemoteIP As String
                                    Dim ServerIP As String
                                    Dim AppID As Object
                                    Dim Url As Object
                                    If Not HttpContext.Current Is Nothing Then
                                        'Web application
                                        UserID = _WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                                        RemoteIP = HttpContext.Current.Request.UserHostAddress
                                        If _WebManager.CurrentServerIdentString = Nothing Then
                                            ServerIP = "0.0.0.0"
                                        Else
                                            ServerIP = _WebManager.CurrentServerIdentString
                                        End If
                                        If _WebManager.SecurityObject = Nothing Then
                                            AppID = DBNull.Value
                                        Else
                                            Dim MySecObjects As SecurityObjectInformation() = _WebManager.System_GetSecurityObjectInformations(_WebManager.SecurityObject)
                                            If Not MySecObjects Is Nothing AndAlso MySecObjects.Length > 0 Then
                                                AppID = MySecObjects(0).ID
                                            Else
                                                AppID = DBNull.Value
                                            End If
                                        End If
                                        Url = Utils.StringNotNothingOrEmpty(address)
                                    Else
                                        'Windows/console application
                                        UserID = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                                        RemoteIP = _WebManager.CurrentRemoteClientAddress
                                        ServerIP = _WebManager.CurrentServerIdentString
                                        AppID = DBNull.Value
                                        Url = DBNull.Value
                                    End If

                                    If Setup.DatabaseUtils.Version(_WebManager, False).Build < 133 Then
                                        .Parameters.Add("@UserID", SqlDbType.Int).Value = UserID
                                        .Parameters.Add("@RemoteIP", SqlDbType.VarChar).Value = RemoteIP
                                        .Parameters.Add("@ServerIP", SqlDbType.VarChar).Value = ServerIP
                                        .Parameters.Add("@ApplicationID", SqlDbType.Int).Value = AppID
                                        .Parameters.Add("@URL", SqlDbType.VarChar, 1024).Value = Url
                                        .Parameters.Add("@ConflictType", SqlDbType.Int).Value = conflictType
                                        .Parameters.Add("@ConflictDescription", SqlDbType.VarChar, 1024).Value = _Message
                                    Else
                                        .Parameters.Add("@UserID", SqlDbType.Int).Value = UserID
                                        .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = RemoteIP
                                        .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = ServerIP
                                        .Parameters.Add("@ApplicationID", SqlDbType.Int).Value = AppID
                                        .Parameters.Add("@URL", SqlDbType.NVarChar, 1024).Value = Url
                                        .Parameters.Add("@ConflictType", SqlDbType.Int).Value = conflictType
                                        .Parameters.Add("@ConflictDescription", SqlDbType.NText).Value = _Message
                                    End If

                                End With

                                'Create recordset by executing the command
                                MyCmd.Connection = MyDBConn
                                MyCmd.ExecuteNonQuery()

                            Catch ex As Exception
                                Log.WriteEventLogTrace("WriteLogItem:ErrorData:" & ex.ToString)
                                If neverSendWarningMails = False AndAlso _WebManager.TechnicalServiceEMailAccountAddress <> "" AndAlso _WebManager.SMTPServerName <> "" Then
                                    Try
                                        Log.WriteEventLogTrace("WriteLogItem:SendEMail:Begin")
                                        Dim RequestEnvironmentUrl As String = ""
                                        If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Request Is Nothing Then RequestEnvironmentUrl = HttpContext.Current.Request.RawUrl
                                        _WebManager.MessagingEMails.SendEMail(_WebManager.TechnicalServiceEMailAccountName, _WebManager.TechnicalServiceEMailAccountAddress, "camm WebManager Warning", "A log entry should be created, but there was an error while creation:" & vbNewLine & ex.ToString & vbNewLine & vbNewLine & "The original message is:" & vbNewLine & message & vbNewLine & vbNewLine & "Reported on " & Now.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss") & " UTC at " & _WebManager.CurrentServerIdentString & vbNewLine & RequestEnvironmentUrl, Nothing, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress, CType(Nothing, Messaging.EMailAttachment()), Messaging.EMails.Priority.High)
                                        Log.WriteEventLogTrace("WriteLogItem:SendEMail:End")
                                    Catch innerex As Exception
                                        Log.WriteEventLogTrace("WriteLogItem:ErrorData:" & innerex.ToString)
                                    End Try
                                End If
                            Finally
                                MyCmd.Dispose()
                                If Not MyDBConn Is Nothing Then
                                    If MyDBConn.State <> ConnectionState.Closed Then
                                        MyDBConn.Close()
                                    End If
                                    MyDBConn.Dispose()
                                End If
                            End Try
                        ElseIf neverSendWarningMails = False AndAlso _WebManager.SMTPServerName <> "" AndAlso _WebManager.TechnicalServiceEMailAccountAddress <> "" Then
                            Try
                                Log.WriteEventLogTrace("WriteLogItem:SendEMail:Begin")
                                _WebManager.MessagingEMails.SendEMail(_WebManager.TechnicalServiceEMailAccountName, _WebManager.TechnicalServiceEMailAccountAddress, "camm WebManager Warning", "The original message is:" & vbNewLine & message & vbNewLine & vbNewLine & "Reported on " & Now.ToUniversalTime.ToString("yyyy-MM-dd HH:mm:ss") & " UTC at " & _WebManager.CurrentServerIdentString, Nothing, _WebManager.StandardEMailAccountName, _WebManager.StandardEMailAccountAddress, CType(Nothing, Messaging.EMailAttachment()), Messaging.EMails.Priority.High)
                                Log.WriteEventLogTrace("WriteLogItem:SendEMail:End")
                            Catch ex As Exception
                                Log.WriteEventLogTrace("WriteLogItem:ErrorData:" & ex.ToString)
                            End Try
                        End If
                    End If
                    WrittenLogsSinceCwmInstanceStart += 1
                End If
            Catch ex As Exception
                'Console.WriteLine(ex.ToString)
            End Try
        End Sub
#End Region

#Region "Shorten log table when demanded"
        Private _RowsInLogTable As Long
        Private _RetentionDays As Integer
        Private _MaxRowsInLogTable As Integer

        Private Const CacheKeyMaxRetentionDays As String = "WebManager.Log.MaxRetentionDays"
        Private Const CacheKeyMaxLogItems As String = "WebManager.Log.RowsMax"
        Private Const CacheKeyRowsInLogTable As String = "WebManager.Log.RowsInTable"
        Private Const CacheKeyConflictTypesLifeTimes As String = "WebManager.Log.ConflictTypesLifeTimes"

        Private Const SqlPropertyNameMaxRetentionDays As String = "MaxRetentionDays"
        Private Const SqlPropertyNameMaxLogItems As String = "MaxLogItems"
        Private Const SqlPropertyNameConflictTypesLifeTimes As String = "ConflictTypeAge"
        ''' <summary>
        '''     The current approximate number of log entries in the event log
        ''' </summary>
        ''' <value></value>
        Public Property RowsInLogTable() As Long
            Get
                Dim cache As Object = GetCacheItem(CacheKeyRowsInLogTable)
                If Not cache Is Nothing Then
                    Return CType(cache, Integer)
                End If
                If _RowsInLogTable = Nothing Then
                    _RowsInLogTable = Me.CountRowsInLogTable()
                    SetCacheItem(CacheKeyRowsInLogTable, _RowsInLogTable)
                End If
                Return _RowsInLogTable
            End Get
            Set(value As Long)
                SetCacheItem(CacheKeyRowsInLogTable, value)
                _RowsInLogTable = value
            End Set
        End Property

        ''' <summary>
        ''' Requery the very current count of records in log table and refresh the RowsInLogTable property and return the current count
        ''' </summary>
        ''' <returns>The very current amount of log entries in database</returns>
        ''' <remarks></remarks>
        Public Function RefreshedRowsInLogTable() As Long
            Dim Result As Long = Me.CountRowsInLogTable()
            Me.RowsInLogTable = Result
            Return Result
        End Function

        ''' <summary>
        ''' Amount of days logs should be retained
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property MaxRetentionDays() As Integer
            Get
                If _RetentionDays = Nothing Then
                    Dim cache As Object = GetCacheItem(CacheKeyMaxRetentionDays)
                    If Not cache Is Nothing Then
                        Return CType(cache, Integer)
                    End If

                    _RetentionDays = GetIntegerConfigEntry(SqlPropertyNameMaxRetentionDays)
                    If _RetentionDays = Nothing Then
                        _RetentionDays = 365 * 9 + 366 * 3 '12 years
                    End If
                End If
                Return _RetentionDays
            End Get
            Set(ByVal Value As Integer)
                SetCacheItem(CacheKeyMaxRetentionDays, Value)
                SetIntegerConfigEntry(SqlPropertyNameMaxRetentionDays, Value)
                _RetentionDays = Value
            End Set
        End Property
        ''' <summary>
        '''     The number of maximum rows in the event log before older logs get truncated
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     A zero value leads to infinite log entries.
        ''' </remarks>
        Public Property MaxRowsInLogTable() As Integer
            Get
                If _MaxRowsInLogTable = Nothing Then
                    Dim cache As Object = GetCacheItem(CacheKeyMaxLogItems)
                    If Not cache Is Nothing Then
                        Return CType(cache, Integer)
                    End If

                    Dim Result As Integer = Me.GetIntegerConfigEntry(SqlPropertyNameMaxLogItems)
                    If Result = Nothing Then
                        _MaxRowsInLogTable = Integer.MaxValue
                    Else
                        _MaxRowsInLogTable = Result
                    End If
                End If
                Return _MaxRowsInLogTable
            End Get
            Set(ByVal Value As Integer)
                SetCacheItem(CacheKeyMaxLogItems, Value)
                SetIntegerConfigEntry(SqlPropertyNameMaxLogItems, Value)
                _MaxRowsInLogTable = Value
            End Set
        End Property

        Private _ConflictTypeLifeTime As Hashtable
        Public Property ConflictTypeLifeTime() As Hashtable
            Get
                Dim cache As Object = GetCacheItem(CacheKeyConflictTypesLifeTimes)
                If Not cache Is Nothing Then
                    Return CType(cache, Hashtable)
                End If
                If _ConflictTypeLifeTime Is Nothing Then
                    _ConflictTypeLifeTime = GetConflictTypeLifetimes()
                End If
                Return _ConflictTypeLifeTime
            End Get
            Set(ByVal Value As Hashtable)
                SetCacheItem(CacheKeyConflictTypesLifeTimes, Value)
                SetConflictTypesLifetime(Value)
                _ConflictTypeLifeTime = Value
            End Set
        End Property


        ''' <summary>
        ''' Returns an item from the cache
        ''' </summary>
        ''' <param name="key"></param>
        Private Function GetCacheItem(ByVal key As String) As Object
            If Not HttpContext.Current Is Nothing Then
                Return HttpContext.Current.Cache(key)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Adds or updates a cache entry
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        Private Sub SetCacheItem(ByVal key As String, ByVal value As Object)
            If Not HttpContext.Current Is Nothing Then
                HttpContext.Current.Cache.Remove(key)
                HttpContext.Current.Cache.Add(key, value, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 60, 0), Caching.CacheItemPriority.Normal, Nothing)
            End If
        End Sub

        ''' <summary>
        ''' Saves an integer into the  [System]_[GlobalProperties] table or updates it
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        Private Sub SetIntegerConfigEntry(ByVal key As String, ByVal value As Integer)
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand()
            MyCmd.Connection = MyConn
            MyCmd.CommandText = "DECLARE @RowNumber int" & vbNewLine & _
                        "SELECT @RowNumber = COUNT(*)" & vbNewLine & _
                        "FROM [dbo].[System_GlobalProperties]" & vbNewLine & _
                        "WHERE VALUENVarChar = N'camm WebManager' AND PropertyName = @key" & vbNewLine & _
                        "SELECT @RowNumber" & vbNewLine & _
                        vbNewLine & _
                        "IF @RowNumber = 0 " & vbNewLine & _
                        "	INSERT INTO [dbo].[System_GlobalProperties]" & vbNewLine & _
                        "		(ValueNVarChar, PropertyName, ValueInt)" & vbNewLine & _
                        "	VALUES (N'camm WebManager', @key, @ValueInt)" & vbNewLine & _
                        "ELSE" & vbNewLine & _
                        "	UPDATE [dbo].[System_GlobalProperties]" & vbNewLine & _
                        "	SET ValueInt = @ValueInt" & vbNewLine & _
                        "	WHERE ValueNVarChar = N'camm WebManager' AND PropertyName = @key" & vbNewLine
            MyCmd.Parameters.Add(New SqlParameter("@key", SqlDbType.VarChar)).Value = key
            MyCmd.Parameters.Add(New SqlParameter("@ValueInt", SqlDbType.Int)).Value = value
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        ''' <summary>
        ''' Returns a hashtable containing the lifetime of a conflict type in days
        ''' </summary>
        Private Function GetConflictTypeLifetimes() As Hashtable
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim cmd As New SqlCommand("SELECT ValueInt, ValueDecimal FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = N'camm WebManager' AND PropertyName= @propertyname")
            cmd.Parameters.Add("@propertyname", SqlDbType.VarChar).Value = SqlPropertyNameConflictTypesLifeTimes
            cmd.Connection = MyConn

            Dim tbl As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "lifetime")

            Dim rowsCount As Integer = tbl.Rows.Count
            If rowsCount < 1 Then
                Return Nothing
            End If
            Dim result As New Hashtable(rowsCount - 1)
            For Each row As DataRow In tbl.Rows
                result.Add(row(0), row(1))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Stores a hashtable containing the lifetime of a conflict type in days
        ''' </summary>
        ''' <param name="hashTable"></param>
        Public Sub SetConflictTypesLifetime(ByVal hashTable As Hashtable)
            Dim MyCmd As SqlCommand = Nothing
            Dim MyConn As SqlConnection = Nothing
            Dim message As String = "UPDATE [dbo].[System_GlobalProperties] SET ValueDecimal = @value WHERE ValueNVarChar = N'camm WebManager' AND PropertyName= @propertyname AND ValueInt = @key " & _
                "IF @@ROWCOUNT = 0 " & _
                "INSERT INTO [dbo].[System_GlobalProperties] (ValueNVarChar, PropertyName, ValueInt, ValueDecimal) VALUES ('camm WebManager', @propertyname, @key, @value)"
            Try
                MyCmd = New SqlCommand(message)
                MyConn = New SqlConnection(_WebManager.ConnectionString)
                MyCmd.Connection = MyConn
                MyCmd.Connection.Open()
                MyCmd.Parameters.Add("@propertyname", SqlDbType.VarChar).Value = SqlPropertyNameConflictTypesLifeTimes
                MyCmd.Parameters.Add("@key", SqlDbType.Int)
                MyCmd.Parameters.Add("@value", SqlDbType.Int)
                For Each entry As DictionaryEntry In hashTable
                    MyCmd.Parameters("@key").Value = CType(entry.Key, Integer)
                    MyCmd.Parameters("@value").Value = CType(entry.Value, Integer)
                    MyCmd.ExecuteNonQuery()
                Next
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

        End Sub

        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Private Function GetIntegerConfigEntry(ByVal key As String) As Integer
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT [ValueInt] FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = N'camm WebManager' AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            MyCmd.Connection = MyConn
            Dim Result As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Result = CompuMaster.camm.WebManager.Utils.Nz(Result)
            Return CType(Result, Integer)
        End Function

        ''' <summary>
        ''' Query the current row number of logs from database
        ''' </summary>
        ''' <remarks></remarks>
        Private Function CountRowsInLogTable() As Long
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("select count(*) as RowsNumber from log", MyConn)
            MyCmd.Connection = MyConn
            Return CType(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Long)
        End Function

        ''' <summary>
        ''' Log a cleanup action
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks></remarks>
        Friend Sub LogCleanupAction(ByVal msg As String)
            Dim cmd As SqlCommand = Nothing
            Dim connection As SqlConnection = Nothing
            Try
                connection = New SqlConnection(_WebManager.ConnectionString)
                cmd = New SqlCommand("INSERT INTO [dbo].[Log] ([UserID], [LoginDate], [RemoteIP], [ServerIP], [ApplicationID], [URL], [ConflictType], [ConflictDescription]) " & _
                        "VALUES (@UserID, GetDate(), @RemoteIP, @ServerIP, @ApplicationID, @URL, @ConflictType, @ConflictDescription)", connection)

                With cmd
                    .CommandType = CommandType.Text
                    .Parameters.Add("@UserID", SqlDbType.Int).Value = SpecialUsers.User_Code
                    .Parameters.Add("@RemoteIP", SqlDbType.NVarChar).Value = _WebManager.CurrentRemoteClientAddress
                    .Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = _WebManager.CurrentServerIdentString
                    .Parameters.Add("@ApplicationID", SqlDbType.Int).Value = DBNull.Value
                    .Parameters.Add("@URL", SqlDbType.NVarChar, 1024).Value = DBNull.Value
                    .Parameters.Add("@ConflictType", SqlDbType.Int).Value = Logging_ConflictTypes.RuntimeInformation
                    .Parameters.Add("@ConflictDescription", SqlDbType.NVarChar, 1024).Value = "Cleanup: " & msg
                End With
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Refresh local cache counters
                Me.RowsInLogTable += 1
                Me.WrittenLogsSinceCwmInstanceStart += 1
            Catch
                'Ignore all errors
            Finally
                If Not cmd Is Nothing Then
                    cmd.Dispose()
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Enforces that we don't have more rows than allowed
        ''' </summary>
        ''' <return>Number of deleted rows from log table</return>
        ''' <remarks></remarks>
        Private Function ShrinkTableToMaxRows(maxNumberOfDeletedRows As Integer) As Integer
            If Me.RowsInLogTable > Me.MaxRowsInLogTable Then
                Dim RowsToRemove As Long = System.Math.Max(0, System.Math.Min(maxNumberOfDeletedRows, Me.RefreshedRowsInLogTable - Me.MaxRowsInLogTable - 100)) 'never use a negative value in case that cache-RowsInTable differs from refreshed-RowsInTable + because we immediately add a new log for truncation info, provide a buffer of 100 (so that not every time the truncation must happen and prevent looping this code) 
                If RowsToRemove > 0 Then
                    Dim connection As New SqlConnection(_WebManager.ConnectionString)
                    Dim cmd As New SqlCommand("DELETE FROM dbo.Log WHERE ID IN (SELECT TOP " & RowsToRemove & " ID FROM [dbo].[Log] ORDER BY ID ASC); SELECT @@ROWCOUNT", connection)
                    cmd.CommandType = CommandType.Text
                    Dim Result As Integer = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
                    LogCleanupAction("Log truncated by " & Result & " rows to keep compliant with the limit of " & MaxRowsInLogTable & " rows; currently present approx. " & Me.RowsInLogTable & " rows")
                    Return Result
                End If
            End If
            Return 0
        End Function

        ''' <summary>
        ''' Removes expired log entries
        ''' </summary>
        ''' <remarks></remarks>
        Private Function DeleteExpiredEntries(maxNumberOfDeletedRows As Integer) As Integer
            Dim connection As New SqlConnection(_WebManager.ConnectionString)
            Dim Sql As String = "DELETE FROM [dbo].[Log] WHERE ID IN " & vbNewLine & _
                "    (" & vbNewLine & _
                "        SELECT TOP " & maxNumberOfDeletedRows & " ID " & vbNewLine & _
                "        FROM dbo.Log" & vbNewLine & _
                "            INNER JOIN (SELECT ValueInt as ConflictTypeID, ValueDecimal as RetentionDays FROM dbo.System_GlobalProperties WHERE PropertyName='ConflictTypeAge') AS RetentionConfig" & vbNewLine & _
                "                ON Log.ConflictType = RetentionConfig.ConflictTypeID" & vbNewLine & _
                "        WHERE LoginDate < DateAdd(dd, -COALESCE(RetentionDays, @DefaultRetentionDays), GETDATE())" & vbNewLine & _
                "    )" & vbNewLine & _
                "SELECT @@ROWCOUNT"
            Dim cmd As New SqlCommand(Sql, connection)
            cmd.CommandType = CommandType.Text
            cmd.Parameters.Add("@DefaultRetentionDays", SqlDbType.Int).Value = Me.MaxRetentionDays
            Dim Result As Integer = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
            LogCleanupAction("Removed " & Result & " expired log entries (delete max. " & maxNumberOfDeletedRows & " rows; max. retention days: " & Me.MaxRetentionDays & "; max. total rows in log table: " & Me.MaxRowsInLogTable & ")")
            Return Result
        End Function

        Private Shared locker As Object = New Object()
        Private Shared IsCleanUpLogTableRunning As Boolean
        ''' <summary>
        '''     Remove old lines from the log to save memory in database
        ''' </summary>
        Public Sub CleanUpLogTable()
            CleanUpLogTableInternal()
        End Sub
        ''' <summary>
        '''     Remove old lines from the log to save memory in database
        ''' </summary>
        ''' <return>Number of deleted rows from log table</return>
        Friend Function CleanUpLogTableInternal() As Integer
            'web access
            SyncLock locker
                If IsCleanUpLogTableRunning Then
                    Return 0
                End If
                IsCleanUpLogTableRunning = True
            End SyncLock

            Dim DeletedRecords As Integer = 0
            Try
                DeletedRecords = DeleteExpiredEntries(500)
                If DeletedRecords < 500 Then 'lesser than 500 rows deleted, yet - so it was pretty fast - still more time to delete a few more rows
                    DeletedRecords = DeletedRecords + ShrinkTableToMaxRows(500)
                End If
                RowsInLogTable = Nothing
            Finally
                IsCleanUpLogTableRunning = False
            End Try
            Return DeletedRecords
        End Function
#End Region

        ''' <summary>
        ''' An exception of the internal system
        ''' </summary>
        Public Class SystemException
            Inherits Exception

            Friend Sub New()
                MyBase.New
            End Sub

            Friend Sub New(message As String)
                MyBase.New(message)
            End Sub

            Friend Sub New(message As String, innerException As Exception)
                MyBase.New(message, innerException)
            End Sub
        End Class

    End Class

End Namespace