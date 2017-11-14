'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web
Imports System.Web.Mail
Imports System.Text
Imports System.Web.SessionState

Namespace CompuMaster.camm.WebManager.Pages.Application

#Region "Base error/warning pages"
    ''' <summary>
    '''     The common error page when an exception raises and doesn't get handled otherwise
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class BaseErrorPage
        Inherits CompuMaster.camm.WebManager.Pages.Page
        ''' <summary>
        '''     The last exception which leaded to this error page
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property LastException() As Exception
            Get
                Return CType(Context.Items("firedexception"), System.Exception)
            End Get
        End Property
        ''' <summary>
        '''     The last exception Guid/Token which leaded to this error page
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property LastExceptionGuid() As String
            Get
                Return CType(Context.Items("firedexceptionguid"), System.String)
            End Get
        End Property
        ''' <summary>
        '''     An optional error code which can contain a more safe exception message
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This should be the preferred message string for the user since the regular message string might contain sensitive data
        ''' </remarks>
        Public ReadOnly Property OptionalErrorCode() As String
            Get
                Return CType(Context.Items("firedexceptioncode"), System.String)
            End Get
        End Property
        ''' <summary>
        '''     Log the error of this page only, but don't redirect to an error page as the default error handler does
        ''' </summary>
        ''' <param name="e"></param>
        Protected Overrides Sub OnError(ByVal e As System.EventArgs)
            Dim ex As Exception = Server.GetLastError()
            If ex Is Nothing Then ex = New Exception("No exception details found, Server.GetLastError() returned Nothing")
            Log.WriteEventLogTrace("OnError:ErrorData:" & ex.ToString)
            cammWebManager.Log.Exception(ex, False)
            Throw New Exception("Error processing request", ex)
        End Sub

        Friend Overrides ReadOnly Property CreationOnTheFlyAllowed() As Boolean
            Get
                Return True
            End Get
        End Property

    End Class

    ''' <summary>
    '''     The common error page when an exception raises and doesn't get handled otherwise
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class BaseWarningPage
        Inherits CompuMaster.camm.WebManager.Pages.Page
        ''' <summary>
        '''     The last warning which leaded to this warning page
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property LastWarning() As Exception
            Get
                Return CType(Context.Items("firedexception"), System.Exception)
            End Get
        End Property
        ''' <summary>
        '''     The last exception Guid/Token which leaded to this error page
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property LastWarningGuid() As String
            Get
                Return CType(Context.Items("firedexceptionguid"), System.String)
            End Get
        End Property

        Friend Overrides ReadOnly Property CreationOnTheFlyAllowed() As Boolean
            Get
                Return True
            End Get
        End Property

    End Class

#End Region

#Region "Customizable error page"
    ''' <summary>
    '''     The error page which displays a notification to the user
    ''' </summary>
    ''' <remarks>
    '''     This is the page class which should be the base for all customizations.
    '''     Please note: all links and resources in the HTML code of this page must be absolute and start with "/". This is since the rendered HTML code will be displayed in the folders of the scripts where the errors occured.
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class ErrorPage
        Inherits BaseErrorPage

        Protected WithEvents TimeStamp As System.Web.UI.WebControls.Label
        Protected WithEvents ErrorPageTitle As System.Web.UI.WebControls.Label
        Protected WithEvents ErrorDescription As System.Web.UI.WebControls.Label

        Protected TextTemplateErrorTitle As String = "An error has been detected"
        Protected TextTemplateErrorDescription As String = "<p>We're sorry for the inconvenience and apologize. The error has been logged for our development team and we'll take care of it.</p><p>This is the reported error:</p><ul><code><Error></code></ul>"
        Protected TextTemplateNoErrorDescription As String = "<p>We're sorry for the inconvenience and apologize.<br>The error has been logged for our development team and we'll take care of it.</p>"
        ''' <summary>
        '''     Retrieve the exception information and return them with the correct HTTP return code
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim ErrorDetails As String

            If Not Me.LastException Is Nothing Then
                ErrorDetails = Me.LastException.Message
                If Me.OptionalErrorCode <> Nothing Then
                    ErrorDetails = Me.OptionalErrorCode
                End If
                If CompuMaster.camm.WebManager.Configuration.DebugLevel <= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    'Rename all absolute physical paths of the virtual root directory to relative ones
                    ErrorDetails = ErrorDetails.Replace(Server.MapPath("~/"), "~\")
                    ErrorDetails = ErrorDetails.Replace(Server.MapPath("/"), "?\")
                End If
                If Configuration.DebugLevel < WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    ErrorDetails = ""
                End If
                'Http return code
                If LastException.GetType Is GetType(System.IO.FileNotFoundException) Then
                    If LastException.StackTrace.IndexOf("at System.Web.UI.TemplateParser.GetParserCacheItem()") >= 0 Then
                        Page.Response.StatusCode = 404
                    End If
                ElseIf LastException.GetType Is GetType(System.Web.HttpCompileException) Then
                    Page.Response.StatusCode = CType(LastException, System.Web.HttpCompileException).GetHttpCode
                ElseIf LastException.GetType Is GetType(System.Web.HttpParseException) Then
                    Page.Response.StatusCode = CType(LastException, System.Web.HttpParseException).GetHttpCode
                ElseIf LastException.GetType Is GetType(System.Web.HttpUnhandledException) Then
                    Page.Response.StatusCode = CType(LastException, System.Web.HttpUnhandledException).GetHttpCode
                ElseIf LastException.GetType Is GetType(System.Web.HttpException) Then
                    Page.Response.StatusCode = CType(LastException, System.Web.HttpException).GetHttpCode
                Else
                    Page.Response.StatusCode = 500
                End If
                Try
                    If Page.Response.StatusCode = 500 AndAlso System.Environment.Version.Major >= 3 Then TrySkipIisCustomErrors()
                Catch
                End Try
            Else
                ErrorDetails = "No information available to the current exception"
            End If

            'Fill the page controls if they exist
            If Not Me.ErrorPageTitle Is Nothing Then
                Me.ErrorPageTitle.Text = TextTemplateErrorTitle
            End If
            If Not Me.ErrorDescription Is Nothing Then
                If ErrorDetails <> Nothing Then
                    Me.ErrorDescription.Text = TextTemplateErrorDescription.Replace("<Error>", Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(ErrorDetails)))
                Else
                    Me.ErrorDescription.Text = TextTemplateNoErrorDescription
                End If
            End If
            If Not Me.TimeStamp Is Nothing Then
                Me.TimeStamp.Text = Now.ToString("yyyy-MM-dd HH:mm:ss")
            End If
            If Me.LastException.Message <> Nothing Then
                If Me.TimeStamp.Text <> "" Then Me.TimeStamp.Text &= " / "
                Me.TimeStamp.Text &= "Exception token: " & Me.LastExceptionGuid
            End If

        End Sub

        ''' <summary>
        ''' Prevent IIS taking over the error page output
        ''' </summary>
        ''' <remarks>IIS 7 automatically takes over with its own error page as soon as http status code is 500. Property TrySkipIisCustomErrors is available starting with .NET 2.0 SP1</remarks>
        Private Sub TrySkipIisCustomErrors()
#If NetFramework <> "1_1" Then
            Try
                Response.TrySkipIisCustomErrors = True
            Catch
            End Try
#End If
        End Sub

    End Class
#End Region

End Namespace

Namespace CompuMaster.camm.WebManager.Application

#Region "InternalBaseHttpApplication"
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), System.Runtime.InteropServices.ComVisible(False)> _
    Public MustInherit Class InternalBaseHttpApplication
        Inherits System.Web.HttpApplication

#Region "cammWebManager object instantiation"
        Private _WebManager As CompuMaster.camm.WebManager.Controls.cammWebManager
        ''' <summary>
        '''     The current instance of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        Public Property cammWebManager() As CompuMaster.camm.WebManager.Controls.cammWebManager
            Get
                If _WebManager Is Nothing Then
                    'Create an instance on the fly
                    _WebManager = OnWebManagerJustInTimeCreation()
                End If
                Return _WebManager
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.Controls.cammWebManager)
                _WebManager = Value
            End Set
        End Property
        ''' <summary>
        '''     Create a camm Web-Manager instance on the fly
        ''' </summary>
        Protected Overridable Function OnWebManagerJustInTimeCreation() As CompuMaster.camm.WebManager.Controls.cammWebManager
            Return New CompuMaster.camm.WebManager.Controls.cammWebManagerJIT(True)
        End Function
#End Region
        ''' <summary>
        '''     Initialize the camm Web-Manager and all related components
        ''' </summary>
        Public Overrides Sub Init()
            MyBase.Init()

            'Enable the app key to allow the customized ComponentArtWebUI component to run in redistributable mode
            If HttpContext.Current.Application("ComponentArtWebUI_AppKey") Is Nothing Then
                Application("ComponentArtWebUI_AppKey") = "This edition of ComponentArt Web.UI is licensed for CompuMaster/camm application only."
            End If

            'Identifies a flag that the cammWebManager has a running instance in the Application
            Dim RunningFirstInit As Boolean = False 'Attention: the Init method may be called multiple times - why ever
            If HttpContext.Current.Application("WebManager.Application.InitiatedByCwmHttpApplication") Is Nothing Then
                Application("WebManager.Application.InitiatedByCwmHttpApplication") = True
                RunningFirstInit = True
            End If

            If RunningFirstInit Then
                Try
                    'Log the application version for live-tracking
                    If Not Me.cammWebManager Is Nothing Then
                        Application("WebManager.Application.Version") = Me.cammWebManager.System_Version_Ex.ToString
                    End If
                    'Check for required components and report errors/warnings to the technical staff
                    If Me.cammWebManager.ConnectionString <> Nothing Then
                        Dim CheckRequiredComponents As System.Data.DataTable
                        CheckRequiredComponents = Me.cammWebManager.IsSupported.RequiredComponentsDetailedCheck()
                        Dim SuppressedComponentsSecurityExceptions As String() = Configuration.ApplicationIgnoreSecurityExceptionsForLoadingAssemblies
                        Dim SuppressedComponentsAllExceptions As String() = Configuration.ApplicationIgnoreAllExceptionsForLoadingAssemblies
                        For MyCounter As Integer = 0 To CheckRequiredComponents.Rows.Count - 1
                            If CompuMaster.camm.WebManager.Utils.StringArrayContainsValue(SuppressedComponentsSecurityExceptions, CompuMaster.camm.WebManager.Utils.Nz(CheckRequiredComponents.Rows(MyCounter)("ComponentName"), ""), True) <> Nothing AndAlso CompuMaster.camm.WebManager.Utils.Nz(CheckRequiredComponents.Rows(MyCounter)("ErrorDetails"), "").IndexOf("System.Security.SecurityException") > -1 Then
                                CheckRequiredComponents.Rows(MyCounter)("Status") = "Critical Warning"
                            ElseIf CompuMaster.camm.WebManager.Utils.StringArrayContainsValue(SuppressedComponentsAllExceptions, CompuMaster.camm.WebManager.Utils.Nz(CheckRequiredComponents.Rows(MyCounter)("ComponentName"), ""), True) <> Nothing Then
                                CheckRequiredComponents.Rows(MyCounter)("Status") = "Load failure ignored"
                            End If
                        Next
                        If CompuMaster.camm.WebManager.Tools.Data.DataTables.GetDataTableClone(CheckRequiredComponents, "Status <> 'Working' AND Status <> 'Critical Warning' AND Status <> 'Load failure ignored'").Rows.Count > 0 Then
                            ReportRequiredComponentsFailures(CheckRequiredComponents)
                        End If
                    End If
                Catch ex As Exception
                    Throw New Exception("An error occured in Application_Init of CWM", ex)
                End Try
            End If

        End Sub

        ''' <summary>
        ''' Report warnings based on failing components
        ''' </summary>
        ''' <param name="data">A data table with details on found/failing components</param>
        ''' <remarks></remarks>
        Protected MustOverride Sub ReportRequiredComponentsFailures(ByVal data As DataTable)

        Private Sub Page_EndRequest(sender As Object, e As EventArgs) Handles MyBase.EndRequest
            If Configuration.SuppressProductRegistrationServiceConnection = False Then
                Try
                    Dim registration As New Registration.ProductRegistration(Me.cammWebManager)
                    If registration.IsRefreshFromRemoteLicenseServerRequired(48) Then
                        Application.Lock()
                        Try
                            registration.CheckRegistration(False)
                        Finally
                            Application.UnLock()
                        End Try
                    End If
                Catch ex As Exception
                    Dim text As String = "HttpApplication_Page_EndRequest: ERROR: " & ex.ToString()
                    Me.cammWebManager.Log.Exception(ex, False)
                End Try
            End If
        End Sub

    End Class
#End Region

#Region "ExceptionResult"
    Friend Class ExceptionResult
        Public Sub New(exception As Exception, exceptionGuid As String)
            Me.ExceptionData = exception
            Me.ExceptionGuid = exceptionGuid
        End Sub

        Private _ExceptionData As Exception
        Public Property ExceptionData As Exception
            Get
                Return _ExceptionData
            End Get
            Set(value As Exception)
                _ExceptionData = value
            End Set
        End Property

        Private _ExceptionGuid As String
        Public Property ExceptionGuid As String
            Get
                Return _ExceptionGuid
            End Get
            Set(value As String)
                _ExceptionGuid = value
            End Set
        End Property
    End Class
#End Region

#Region "BaseHttpApplication"
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class BaseHttpApplication
        Inherits InternalBaseHttpApplication

#Region "Exception handling"
        ''' <summary>
        '''     Catch the last exception and log it respectively create a notification for the technicians
        ''' </summary>
        ''' <param name="reportException">True reports the exception, false doesn't (for SPAM reasons)</param>
        ''' <returns>The last exception</returns>
        Public Function CatchAndDistributeLastError(ByVal reportException As Boolean) As Exception
            Dim Result As ExceptionResult = CatchAndDistributeLastErrorDetails(reportException)
            If Result Is Nothing Then
                Return Nothing
            Else
                Return Result.ExceptionData
            End If
        End Function

        ''' <summary>
        '''     Catch the last exception and log it respectively create a notification for the technicians
        ''' </summary>
        ''' <param name="reportException">True reports the exception, false doesn't (for SPAM reasons)</param>
        ''' <returns>The last exception</returns>
        Friend MustOverride Function CatchAndDistributeLastErrorDetails(reportException As Boolean) As ExceptionResult

        ''' <summary>
        ''' Report warnings based on failing components
        ''' </summary>
        ''' <param name="data">A data table with details on found/failing components</param>
        ''' <remarks></remarks>
        Protected Overrides Sub ReportRequiredComponentsFailures(ByVal data As DataTable)
            Dim ResultHtml As String
            Dim ResultPlainText As String
            ResultHtml = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToHtmlTable(data.Rows, "").Replace("<TD>Missing or failing</TD>", "<TD style=""color: red;"">Missing or failing</TD>").Replace("<TD>Critical Warning</TD>", "<TD style=""color: darkorange;"">Critical Warning</TD>").Replace("<TD>Warning</TD>", "<TD style=""color: darkorange;"">Warning</TD>")
            ResultHtml = CompuMaster.camm.WebManager.Log.BuildHtmlMessage(ResultHtml, "Conflict warning - failing component associations", "", Nothing, Me.Context, Nothing, Me.cammWebManager, ItemsLoggedInTheLast10Minutes)
            ResultPlainText = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToPlainTextTable(data.Rows, "")
            ResultPlainText = CompuMaster.camm.WebManager.Log.BuildPlainMessage(ResultPlainText, "Conflict warning - failing component associations", "", Nothing, Me.Context, Nothing, Me.cammWebManager, ItemsLoggedInTheLast10Minutes)
            Me.cammWebManager.MessagingEMails.QueueEMail(Me.cammWebManager.TechnicalServiceEMailAccountName, Me.cammWebManager.TechnicalServiceEMailAccountAddress, "Conflict warning - failing component associations", ResultPlainText, ResultHtml, Me.cammWebManager.StandardEMailAccountName, Me.cammWebManager.StandardEMailAccountAddress)
        End Sub
        ''' <summary>
        '''     Convert a compiler error collection into a simple text string
        ''' </summary>
        ''' <param name="errorCollection"></param>
        Protected Function ConvertErrorCollectionToString(ByVal errorCollection As System.CodeDom.Compiler.CompilerErrorCollection) As String
            Dim Result As New StringBuilder
            For MyCounter As Integer = 0 To errorCollection.Count - 1
                Result.Append(CType(IIf(errorCollection(MyCounter).IsWarning, "Warning", "Error"), String) & ": " & errorCollection(MyCounter).ErrorText & vbNewLine)
            Next
            Return Result.ToString
        End Function


        'Private Function GetLoggedExceptionItems() As Integer

        'End Function

        'Private Sub SetLoggedExceptionItems()

        'End Sub

        Protected Friend ItemsLoggedInTheLast10Minutes As Integer = 0
        ''' <summary>
        '''     Handle all occurances of exceptions
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Sub OnApplication_Error(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Error
            Log.WriteEventLogTrace("OnApplication_Error:Begin")
            Dim LastExceptionDetails As CompuMaster.camm.WebManager.Application.ExceptionResult = Nothing
            Dim LastExceptionGuid As String = Nothing
            Dim LastException As Exception = Nothing
            If (CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.On OrElse CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.NoSourceCode OrElse CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.Developer) AndAlso Not Me.cammWebManager Is Nothing Then
                'Distribute notification of this exception
                Try
                    Application.Lock()
                    Try
                        ItemsLoggedInTheLast10Minutes = CType(Context.Cache.Item("WebManager.NotifyOnApplicationExceptions.LoggedItems"), Integer)
                        'Increase the number of found exceptions
                        If Context.Cache.Item("WebManager.NotifyOnApplicationExceptions.LoggedItems") Is Nothing Then
                            Context.Cache.Add("WebManager.NotifyOnApplicationExceptions.LoggedItems", 1, Nothing, Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 10, 0), Caching.CacheItemPriority.NotRemovable, Nothing)
                        Else
                            Context.Cache.Item("WebManager.NotifyOnApplicationExceptions.LoggedItems") = ItemsLoggedInTheLast10Minutes + 1
                        End If
                        Log.WriteEventLogTrace("OnApplication_Error:ExceptionCatchingCompleted:CounterIncreased")
                    Finally
                        Application.UnLock()
                    End Try
                    'Catch error...
                    If ItemsLoggedInTheLast10Minutes <= 10 Then
                        '...and send it to the technical/development contact
                        LastExceptionDetails = CatchAndDistributeLastErrorDetails(True)
                        LastException = LastExceptionDetails.ExceptionData
                        LastExceptionGuid = LastExceptionDetails.ExceptionGuid
                        If LastException Is Nothing Then
                            Throw New Exception("MissingException")
                        End If
                    Else
                        '...and DON'T send an e-mail to prevent unwanted mass mailings
                        LastException = CatchAndDistributeLastError(False)
                        If LastException Is Nothing Then
                            Throw New Exception("MissingException")
                        End If
                    End If
                    If GetType(IgnoreException).IsInstanceOfType(LastException) Then
                        'Return without catching this 404 error
                        Log.WriteEventLogTrace("OnApplication_Error:404Exit")
                        Return
                    End If
                    Log.WriteEventLogTrace("OnApplication_Error:ExceptionCatchingCompleted:1")
                Catch ex As Exception
                    'Log the exception which happened while sending the original exception
                    Try
                        Log.WriteEventLogTrace("OnApplication_Error:CatchedException(see following message)")
                        CompuMaster.camm.WebManager.Log.WriteEventLogTrace(ex.ToString, System.Diagnostics.EventLogEntryType.Error, True)
                    Catch criticalEx As Exception
                        Dim CurrentResponse As System.Web.HttpResponse
                        Try
                            CurrentResponse = Response
                        Catch
                            Try
                                CurrentResponse = HttpContext.Current.Response
                            Catch
                                CurrentResponse = Nothing
                                'Abort error handler here - standard handler needs to take over
                                Exit Sub
                            End Try
                        End Try
                        'Ignore errors
                        CurrentResponse.Clear()
                        CurrentResponse.ContentType = "text/plain"
                        CurrentResponse.Write("INTERNAL ERROR e4038 - PLEASE NOTIFY US ABOUT THIS ERROR" & vbNewLine & vbNewLine)
                        CurrentResponse.Write("Error of error reporting system" & vbNewLine)
                        CurrentResponse.Write("Ex=" & ex.ToString & vbNewLine)
                        CurrentResponse.Write(vbNewLine)
                        CurrentResponse.Write("Error of alternative error reporting system" & vbNewLine)
                        CurrentResponse.Write("CriticalEx=" & criticalEx.ToString)
                        Server.ClearError()
                        CurrentResponse.End()
                    End Try
                End Try
            ElseIf CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.Off Then
                'Return without doing anything
                Log.WriteEventLogTrace("OnApplication_Error:Aborted:NotificationLevelOnApplicationException=Off")
                Return
            Else
                'No exception notification elsewhere
                Try
                    LastException = Server.GetLastError()
                    If Not LastException.InnerException Is Nothing Then
                        'Regulary, the real exception in contained in the InnerException property
                        LastException = LastException.InnerException
                    End If
                Catch ex As Exception
                    'Log the exception which happened while sending the original exception
                    CompuMaster.camm.WebManager.Log.WriteEventLogTrace(ex.ToString, System.Diagnostics.EventLogEntryType.Error, True)
                End Try
            End If

            Try
                Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:Begin")

                'There must be at least one exception for further processing
                If LastException Is Nothing Then
                    LastException = New Exception("Unknown exception")
                End If
                Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:ExceptionFilled")

                'Forward to a common error page if this has been enabled
                If CompuMaster.camm.WebManager.Configuration.TransferRequestOnApplicationExceptionsTo <> Nothing AndAlso System.IO.File.Exists(Server.MapPath(CompuMaster.camm.WebManager.Configuration.TransferRequestOnApplicationExceptionsTo)) AndAlso Me.Request.Url.AbsolutePath <> CompuMaster.camm.WebManager.Configuration.TransferRequestOnApplicationExceptionsTo Then
                    Try
                        Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:1")
                        Response.Clear()
                        Context.Items("firedexception") = LastException
                        Context.Items("firedexceptioncode") = Me.ErrorCode
                        Context.Items("firedexceptionguid") = LastExceptionGuid
                        Server.ClearError()
                        Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:2")
                        Server.Transfer(CompuMaster.camm.WebManager.Configuration.TransferRequestOnApplicationExceptionsTo, False)
                    Catch ex As Exception
                        Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:Failed:Begin")
                        If CompuMaster.camm.WebManager.Configuration.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                            Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:Failed:PreResponseWrite:EndWithMediemDebugLevel")
                            Response.Write("<h3>Error in " & CompuMaster.camm.WebManager.Configuration.TransferRequestOnApplicationExceptionsTo & ": or while transferring to it</h3>")
                            Response.Write(Utils.HTMLEncodeLineBreaks(ex.ToString))
                            Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:Failed:EndWithMediemDebugLevel")
                            Response.End()
                        Else
                            If System.Environment.Version.Major >= 4 Then
                                'ASP.NET 4.x or higher doesn't allow Server.Transfer to /sysdata/error.aspx 
                                'and in case we would simply exit this method, it would step into a recursive loop
                                Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:Failed:PreResponseWrite")
                                Response.Write("<h3>Error in " & CompuMaster.camm.WebManager.Configuration.TransferRequestOnApplicationExceptionsTo & " or while transferring to it:</h3>")
                                Response.Write("<ul><li>" & Utils.HTMLEncodeLineBreaks(ex.Message) & "</li>")
                                Response.Write("<li>" & Utils.HTMLEncodeLineBreaks(LastException.Message) & "</li></ul>")
                                Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:PreServerTransfer:Failed:End")
                                Response.End()
                            Else
                                'ASP.NET <= 2
                                'Just exit - the default error handler takes over
                                Return
                            End If
                        End If
                    End Try
                End If
            Catch criticalEx As Exception
                Try
                    Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:Failed:Begin")
                Catch
                End Try
                Dim CurrentResponse As System.Web.HttpResponse
                Try
                    CurrentResponse = Response
                Catch
                    Try
                        CurrentResponse = HttpContext.Current.Response
                    Catch
                        CurrentResponse = Nothing
                        'Abort error handler here - standard handler needs to take over
                        Exit Sub
                    End Try
                End Try
                'Ignore errors
                CurrentResponse.Clear()
                CurrentResponse.ContentType = "text/plain"
                CurrentResponse.Write("INTERNAL ERROR e4039 - PLEASE NOTIFY US ABOUT THIS ERROR" & vbNewLine & vbNewLine)
                CurrentResponse.Write("Error of error reporting system" & vbNewLine)
                CurrentResponse.Write("CriticalEx=" & criticalEx.ToString)
                Server.ClearError()
                Try
                    Log.WriteEventLogTrace("OnApplication_Error:ExceptionResponse:Failed:End")
                Catch
                End Try
                CurrentResponse.End()
            End Try

        End Sub

        Private _ErrorCode As String
        ''' <summary>
        '''     The short HTTP error code text, e. g. "404 File not found"
        ''' </summary>
        ''' 
        Protected Property ErrorCode() As String
            Get
                Return _ErrorCode
            End Get
            Set(ByVal value As String)
                _ErrorCode = value
            End Set
        End Property

#Region "Render exception messages"
        ''' <summary>
        '''     Prepare the message text for the exception in plain text format
        ''' </summary>
        ''' <param name="exceptionDetails"></param>
        ''' <param name="exceptionIdentifier"></param>
        Public Overridable Function BuildPlainMessage(ByVal exceptionDetails As String, ByVal exceptionIdentifier As String, exceptionGuid As String) As String
            Dim CurrentRequest As System.Web.HttpRequest, CurrentContext As System.Web.HttpContext, CurrentUserPrincipal As System.Security.Principal.IPrincipal
            Try
                CurrentRequest = Request
                CurrentContext = Context
                CurrentUserPrincipal = User
            Catch
                Try
                    CurrentRequest = HttpContext.Current.Request
                    CurrentContext = HttpContext.Current
                    CurrentUserPrincipal = HttpContext.Current.User
                Catch
                    CurrentRequest = Nothing
                    CurrentContext = Nothing
                    CurrentUserPrincipal = Nothing
                End Try
            End Try
            Return CompuMaster.camm.WebManager.Log.BuildPlainMessage(exceptionDetails, exceptionIdentifier, exceptionGuid, CurrentRequest, CurrentContext, CurrentUserPrincipal, Me.cammWebManager, ItemsLoggedInTheLast10Minutes)
        End Function
        ''' <summary>
        '''     Prepare the message text for the exception in HTML output format
        ''' </summary>
        ''' <param name="exceptionDetailsAsHtml">Details on the exception in valid HTML code (and without body or script tags or similar)</param>
        ''' <param name="exceptionIdentifier"></param>
        Public Overridable Function BuildHtmlMessage(ByVal exceptionDetailsAsHtml As String, ByVal exceptionIdentifier As String, exceptionGuid As String) As String
            Dim CurrentRequest As System.Web.HttpRequest, CurrentContext As System.Web.HttpContext, CurrentUserPrincipal As System.Security.Principal.IPrincipal
            Try
                CurrentRequest = Request
                CurrentContext = Context
                CurrentUserPrincipal = User
            Catch
                Try
                    CurrentRequest = HttpContext.Current.Request
                    CurrentContext = HttpContext.Current
                    CurrentUserPrincipal = HttpContext.Current.User
                Catch
                    CurrentRequest = Nothing
                    CurrentContext = Nothing
                    CurrentUserPrincipal = Nothing
                End Try
            End Try
            Return CompuMaster.camm.WebManager.Log.BuildHtmlMessage(exceptionDetailsAsHtml, exceptionIdentifier, exceptionGuid, CurrentRequest, CurrentContext, CurrentUserPrincipal, Me.cammWebManager, ItemsLoggedInTheLast10Minutes)
        End Function
#End Region

#End Region
        ''' <summary>
        '''     Do some cleanup work
        ''' </summary>
        Public Overrides Sub Dispose()
            'ToDo: look for any uncommitted log items (hits to URLs without security) which need to be replicated
            MyBase.Dispose()
        End Sub

#Region "Authentication/Authorization"
#If NotImplemented Then
#Region "ValidateAccessToSecurityObject"
        ''' <summary>
        '''     Perform a document access check for the security object if it has been configured for the current path
        ''' </summary>
        Protected Overridable Sub ValidateAccessToSecurityObject()

            If cammWebManager.SecurityObject <> Nothing AndAlso cammWebManager.InitializationState >= WMSystem.InitializationStates.ServerCommunicationAvailable Then
                'Security object, database connection as well as current server ID are present - we are now able to perform the security check
                cammWebManager.AuthorizeDocumentAccess(cammWebManager.SecurityObject)
            End If

        End Sub

#End Region

        Private Sub HttpApplication_AuthenticateRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.AuthenticateRequest

        End Sub

        Private Sub HttpApplication_AuthorizeRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.AuthorizeRequest

            'Where is the correct place to retrieve the configured value in web.config of the current folder (not the application folder!)
            ''Perform a document access check for the security object if it has been configured for the current path
            'ValidateAccessToSecurityObject()

        End Sub
#End If
#End Region

    End Class

    Public Class IgnoreException
        Inherits Exception

    End Class

#End Region

#Region "Optional HttpApplication handler of camm Web-Manager (e. g. for handling of exceptions)"
    ''' <summary>
    '''     An HttpApplication handler of camm Web-Manager for the handling of thrown exceptions
    ''' </summary>
    ''' <remarks>
    '''     To use the camm Web-Manager in the application context, you should have configured the most important things in your web.config - even if you have configured all settings in your /sysdata/config.vb file.
    '''     camm Web-Manager will take all settings from the web.config file of your web application. The required ones or the important ones for this HttpApplication class are:
    '''     <list>
    '''         <item>WebManager.ConnectionString</item>
    '''         <item>WebManager.SMTPServerName</item>
    '''         <item>WebManager.StandardEMailAccountAddress</item>
    '''         <item>WebManager.DevelopmentEMailAccountAddress</item>
    '''         <item>WebManager.TechnicalServiceEMailAccountName</item>
    '''         <item>WebManager.TechnicalServiceEMailAccountAddress</item>
    '''         <item>WebManager.NotifyOnApplicationExceptions (only if you want to change the default)</item>
    '''     </list>
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class HttpApplication
        Inherits BaseHttpApplication
        ''' <summary>
        '''     Catch the last exception and log it respectively create a notification for the technicians
        ''' </summary>
        ''' <param name="reportException">True reports the exception, false doesn't (for SPAM reasons)</param>
        ''' <returns>The last exception</returns>
        Friend Overrides Function CatchAndDistributeLastErrorDetails(reportException As Boolean) As ExceptionResult
            Log.WriteEventLogTrace("CatchAndDistributeLastError:Begin")
            Dim LastException As Exception
            LastException = Server.GetLastError()
            If LastException Is Nothing Then
                LastException = New Exception("An exception has been cached by the global application, but Server.GetLastError returned no exception")
                Log.WriteEventLogTrace("CatchAndDistributeLastError:Aborted:NoExceptionFound")
                Return Nothing
            ElseIf Not LastException.InnerException Is Nothing Then
                'Regulary, the real exception in contained in the InnerException property
                LastException = LastException.InnerException
            End If
            Dim LastExceptionPlainString As String
            Dim LastExceptionHtmlString As String
            If LastException.GetType Is GetType(System.IO.FileNotFoundException) Then
                If Configuration.NotifyOnApplicationExceptions404 = False Then
                    'Ignore 404 errors
                    Return New ExceptionResult(New IgnoreException, "")
                ElseIf Configuration.NotifyOnApplicationExceptions404IgnoreCrawlers = True AndAlso Utils.IsRequestFromCrawlerAgent(Request) = True Then
                    'Ignore 404 errors by crawlers
                    Return New ExceptionResult(New IgnoreException, "")
                Else
                    LastExceptionPlainString = CType(LastException, System.IO.FileNotFoundException).ToString()
                    LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(LastExceptionPlainString))
                    If LastException.StackTrace.IndexOf(" System.Web.UI.TemplateParser.GetParserCacheItem(") >= 0 OrElse LastException.StackTrace.IndexOf(" System.Web.UI.Util.CheckVirtualFileExists(") >= 0 Then
                        ErrorCode = "404 File not found"
                    End If
                End If
            ElseIf LastException.GetType Is GetType(System.Web.HttpCompileException) Then
                Try
                    If CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions >= Configuration.NotificationLevelOnApplicationException.On Then
                        LastExceptionPlainString = CType(LastException, System.Web.HttpCompileException).ToString() & vbNewLine & vbNewLine & ConvertErrorCollectionToString(CType(LastException, System.Web.HttpCompileException).Results.Errors) & vbNewLine & CType(LastException, System.Web.HttpCompileException).SourceCode
                        LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(CType(LastException, System.Web.HttpCompileException).ToString()) & vbNewLine) & CType(LastException, System.Web.HttpCompileException).GetHtmlErrorMessage.Replace("html>", "innerhtml>").Replace("head>", "innerhead>").Replace("<script", "<innerscript").Replace("</script", "</innerscript").Replace("<body", "<innerbody").Replace("</body", "</innerbody").Replace("display: none;", "")
                    Else
                        LastExceptionPlainString = CType(LastException, System.Web.HttpCompileException).ToString() & vbNewLine & vbNewLine
                        LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(CType(LastException, System.Web.HttpCompileException).ToString()) & vbNewLine)
                    End If
                    ErrorCode = "500 Compilation failed"
                Catch criticalEx As Exception
                    LastExceptionPlainString = CType(LastException, System.Web.HttpCompileException).ToString() & vbNewLine & vbNewLine
                    LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(CType(LastException, System.Web.HttpCompileException).ToString()) & vbNewLine)
                    ErrorCode = "500 Compilation failed (no details because of internal exception: " & criticalEx.Message & ")"
                End Try
            ElseIf LastException.GetType Is GetType(System.Web.HttpParseException) Then
                LastExceptionPlainString = CType(LastException, System.Web.HttpParseException).ToString()
                LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(LastExceptionPlainString))
                ErrorCode = "500 Parsing file failed"
            ElseIf LastException.GetType Is GetType(System.Web.HttpUnhandledException) Then
                LastExceptionPlainString = CType(LastException, System.Web.HttpUnhandledException).ToString()
                LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(LastExceptionPlainString))
                ErrorCode = CType(LastException, System.Web.HttpUnhandledException).GetHttpCode & " " & CType(LastException, System.Web.HttpUnhandledException).Message
            ElseIf LastException.GetType Is GetType(System.Web.HttpException) Then
                If Not LastException.StackTrace Is Nothing AndAlso (LastException.StackTrace.IndexOf(" System.Web.UI.TemplateParser.GetParserCacheItem(") >= 0 OrElse LastException.StackTrace.IndexOf(" System.Web.UI.Util.CheckVirtualFileExists(") >= 0) Then
                    If Configuration.NotifyOnApplicationExceptions404 = False Then
                        'Ignore 404 errors
                        Return New ExceptionResult(New IgnoreException, "")
                    ElseIf Configuration.NotifyOnApplicationExceptions404IgnoreCrawlers = True AndAlso Utils.IsRequestFromCrawlerAgent(Request) = True Then
                        'Ignore 404 errors by crawlers
                        Return New ExceptionResult(New IgnoreException, "")
                    End If
                End If
                LastExceptionPlainString = CType(LastException, System.Web.HttpException).ToString()
                LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(LastExceptionPlainString))
                ErrorCode = CType(LastException, System.Web.HttpException).GetHttpCode & " " & CType(LastException, System.Web.HttpException).Message
            ElseIf LastException.GetType Is GetType(System.Data.SqlClient.SqlException) Then
                LastExceptionPlainString = CType(LastException, System.Data.SqlClient.SqlException).ToString()
                LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(LastExceptionPlainString))
            Else
                LastExceptionPlainString = LastException.ToString()
                LastExceptionHtmlString = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(LastExceptionPlainString))
            End If

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(LastException)
            If AdditionalExceptionDetails <> Nothing Then
                LastExceptionPlainString &= vbNewLine & AdditionalExceptionDetails
                LastExceptionHtmlString &= "<p><em>" & Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(AdditionalExceptionDetails)) & "</em></p>"
            End If

            'Send the e-mail
            Dim ExceptionGuid As String = Guid.NewGuid.ToString()
            If reportException Then
                Dim BodyHtmlText As String = Me.BuildHtmlMessage(LastExceptionHtmlString, ErrorCode, ExceptionGuid)
                Dim BodyPlainText As String = Me.BuildPlainMessage(LastExceptionPlainString, ErrorCode, ExceptionGuid)
                Dim RequestUrlHostName As String = RequestUrlHost(Request)

                If CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.TechnicalContactAndDeveloper Then
                    cammWebManager.MessagingEMails.SendEMail(Messaging.EMails.CreateReceipientString(cammWebManager.DevelopmentEMailAccountAddress, cammWebManager.DevelopmentEMailAccountAddress) & "," & cammWebManager.TechnicalServiceEMailAccountAddress, "", "", "Page error @ " & RequestUrlHostName, BodyPlainText, BodyHtmlText, cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                ElseIf CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions = Configuration.NotificationLevelOnApplicationException.Developer Then
                    cammWebManager.MessagingEMails.SendEMail(cammWebManager.DevelopmentEMailAccountAddress, cammWebManager.DevelopmentEMailAccountAddress, "Page error @ " & RequestUrlHostName, BodyPlainText, BodyHtmlText, cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                Else
                    cammWebManager.MessagingEMails.SendEMail(cammWebManager.TechnicalServiceEMailAccountName, cammWebManager.TechnicalServiceEMailAccountAddress, "Page error @ " & RequestUrlHostName, BodyPlainText, BodyHtmlText, cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress)
                End If
            End If

            Log.WriteEventLogTrace("CatchAndDistributeLastError:ErrorData:" & LastException.ToString & vbNewLine & "ExceptionGuid: " & ExceptionGuid)

            Log.WriteEventLogTrace("CatchAndDistributeLastError:End")
            Return New ExceptionResult(LastException, ExceptionGuid)

        End Function

        Friend Shared Function RequestUrlHost(Request As System.Web.HttpRequest) As String
            Try
                If Request IsNot Nothing Then
                    Return Request.Url.Host
                Else
                    Return "{Machine: " & System.Environment.MachineName & "}"
                End If
            Catch ex As Exception
                Return "{RequestHost not resolvable: " & ex.Message & "}"
            End Try
        End Function

    End Class

#End Region

#Region "ExceptionLogIntoWindowsEventLog"

    ''' <summary>
    '''     An HttpApplication handler of camm Web-Manager for the handling of thrown exceptions and logging them into the windows event log
    ''' </summary>
    ''' <remarks>
    '''     To use the camm Web-Manager in the application context, you should have configured the most important things in your web.config - even if you have configured all settings in your /sysdata/config.vb file.
    '''     camm Web-Manager will take all settings from the web.config file of your web application. The required ones or the important ones for this HttpApplication class are:
    '''     <list>
    '''         <item>WebManager.NotifyOnApplicationExceptions (only if you want to change the default)</item>
    '''     </list>
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class ExceptionLogIntoWindowsEventLog
        Inherits BaseHttpApplication
        ''' <summary>
        '''     Catch the last exception and log it respectively send a notification e-mail
        ''' </summary>
        ''' <returns>The last exception</returns>
        Friend Overrides Function CatchAndDistributeLastErrorDetails(reportException As Boolean) As ExceptionResult
            Log.WriteEventLogTrace("CatchAndDistributeLastError:Begin")
            Dim LastException As Exception
            LastException = Server.GetLastError()
            If LastException Is Nothing Then
                LastException = New Exception("An exception has been cached by the global application, but Server.GetLastError returned no exception")
            ElseIf Not LastException.InnerException Is Nothing Then
                'Regulary, the real exception in contained in the InnerException property
                LastException = LastException.InnerException
            End If
            Dim LastExceptionPlainString As String
            If LastException.GetType Is GetType(System.IO.FileNotFoundException) Then
                If Configuration.NotifyOnApplicationExceptions404 = False Then
                    'Ignore 404 errors
                    Return New ExceptionResult(New IgnoreException, "")
                ElseIf Configuration.NotifyOnApplicationExceptions404IgnoreCrawlers = True AndAlso Utils.IsRequestFromCrawlerAgent(Request) = True Then
                    'Ignore 404 errors by crawlers
                    Return New ExceptionResult(New IgnoreException, "")
                Else
                    LastExceptionPlainString = CType(LastException, System.IO.FileNotFoundException).ToString()
                    If LastException.StackTrace.IndexOf(" System.Web.UI.TemplateParser.GetParserCacheItem(") >= 0 OrElse LastException.StackTrace.IndexOf(" System.Web.UI.Util.CheckVirtualFileExists(") >= 0 Then
                        ErrorCode = "404 File not found"
                    End If
                End If
            ElseIf LastException.GetType Is GetType(System.Web.HttpCompileException) Then
                If CompuMaster.camm.WebManager.Configuration.NotifyOnApplicationExceptions >= Configuration.NotificationLevelOnApplicationException.On Then
                    LastExceptionPlainString = CType(LastException, System.Web.HttpCompileException).ToString() & vbNewLine & vbNewLine & ConvertErrorCollectionToString(CType(LastException, System.Web.HttpCompileException).Results.Errors) & vbNewLine & CType(LastException, System.Web.HttpCompileException).SourceCode
                Else
                    LastExceptionPlainString = CType(LastException, System.Web.HttpCompileException).ToString() & vbNewLine & vbNewLine
                End If
                ErrorCode = "500 Compilation failed"
            ElseIf LastException.GetType Is GetType(System.Web.HttpParseException) Then
                LastExceptionPlainString = CType(LastException, System.Web.HttpParseException).ToString()
                ErrorCode = "500 Parsing file failed"
            ElseIf LastException.GetType Is GetType(System.Web.HttpUnhandledException) Then
                LastExceptionPlainString = CType(LastException, System.Web.HttpUnhandledException).ToString()
                ErrorCode = CType(LastException, System.Web.HttpUnhandledException).GetHttpCode & " " & CType(LastException, System.Web.HttpUnhandledException).Message
            ElseIf LastException.GetType Is GetType(System.Web.HttpException) Then
                If LastException.StackTrace.IndexOf(" System.Web.UI.TemplateParser.GetParserCacheItem(") >= 0 OrElse LastException.StackTrace.IndexOf(" System.Web.UI.Util.CheckVirtualFileExists(") >= 0 Then
                    If Configuration.NotifyOnApplicationExceptions404 = False Then
                        'Ignore 404 errors
                        Return New ExceptionResult(New IgnoreException, "")
                    ElseIf Configuration.NotifyOnApplicationExceptions404IgnoreCrawlers = True AndAlso Utils.IsRequestFromCrawlerAgent(Request) = True Then
                        'Ignore 404 errors by crawlers
                        Return New ExceptionResult(New IgnoreException, "")
                    End If
                End If
                LastExceptionPlainString = CType(LastException, System.Web.HttpException).ToString()
                ErrorCode = CType(LastException, System.Web.HttpException).GetHttpCode & " " & CType(LastException, System.Web.HttpException).Message
            ElseIf LastException.GetType Is GetType(System.Data.SqlClient.SqlException) Then
                LastExceptionPlainString = CType(LastException, System.Data.SqlClient.SqlException).ToString()
            Else
                LastExceptionPlainString = LastException.ToString()
            End If

            'Additional exception details
            Dim AdditionalExceptionDetails As String = CompuMaster.camm.WebManager.Log.AdditionalDataOfException(LastException)
            If AdditionalExceptionDetails <> Nothing Then
                LastExceptionPlainString &= vbNewLine & AdditionalExceptionDetails
            End If

            'Create the windows event log entry
            Dim ExceptionGuid As String = Guid.NewGuid.ToString()
            Dim BodyPlainText As String = Me.BuildPlainMessage(LastExceptionPlainString, ErrorCode, ExceptionGuid)
            If reportException Then
                Dim RequestUrlHostName As String = HttpApplication.RequestUrlHost(Request)
                Log.WriteEventLogTrace("Page error @ " & RequestUrlHostName & vbNewLine & BodyPlainText, System.Diagnostics.EventLogEntryType.Error, True)
            End If

            Log.WriteEventLogTrace("CatchAndDistributeLastError:ErrorData:" & LastException.ToString & vbNewLine & "ExceptionGuid: " & ExceptionGuid)

            Log.WriteEventLogTrace("CatchAndDistributeLastError:End")
            Return New ExceptionResult(LastException, ExceptionGuid)

        End Function

        Protected Overrides Sub ReportRequiredComponentsFailures(ByVal data As DataTable)
            Dim ResultHtml As String
            Dim ResultPlainText As String
            ResultHtml = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToHtmlTable(data.Rows, "").Replace("<TD>Missing or failing</TD>", "<TD style=""color: red;"">Missing or failing</TD>").Replace("<TD>Critical Warning</TD>", "<TD style=""color: darkorange;"">Critical Warning</TD>").Replace("<TD>Warning</TD>", "<TD style=""color: darkorange;"">Warning</TD>")
            ResultHtml = CompuMaster.camm.WebManager.Log.BuildHtmlMessage(ResultHtml, "Conflict warning - failing component associations", "", Nothing, Me.Context, Nothing, Me.cammWebManager, ItemsLoggedInTheLast10Minutes)
            ResultPlainText = CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertToPlainTextTable(data.Rows, "")
            ResultPlainText = CompuMaster.camm.WebManager.Log.BuildPlainMessage(ResultPlainText, "Conflict warning - failing component associations", "", Nothing, Me.Context, Nothing, Me.cammWebManager, ItemsLoggedInTheLast10Minutes)
            Me.cammWebManager.MessagingEMails.QueueEMail(Me.cammWebManager.TechnicalServiceEMailAccountName, Me.cammWebManager.TechnicalServiceEMailAccountAddress, "Conflict warning - failing component associations", ResultPlainText, ResultHtml, Me.cammWebManager.StandardEMailAccountName, Me.cammWebManager.StandardEMailAccountAddress)
        End Sub

    End Class

#End Region

#Region "HttpApplication for Single-Sign-On"
    <System.Runtime.InteropServices.ComVisible(False)> Public Class SingleSignOnViaWindowsAuthentification
        Inherits HttpApplication
        ''' <summary>
        '''     Catch the 401 errors which can't be catched by web.config or Application_Error
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     Redirect to the error page for status code 401
        ''' </remarks>
        Private Sub Global_EndRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.EndRequest
            If Response.StatusCode = 401 AndAlso Request.IsAuthenticated = True Then
                cammWebManager.RedirectTo("notauthorized.aspx")
            ElseIf Response.StatusCode = 401 Then
                Response.ClearContent()
                Response.Write("<html>" & vbNewLine)
                Response.Write("<head>" & vbNewLine)
                Response.Write("<meta http-equiv=""refresh"" content=""0; url=notauthenticated.aspx"">" & vbNewLine)
                Response.Write("</head>" & vbNewLine)
                Response.Write("<body>" & vbNewLine)
                Response.Write("<script language=""javascript"">" & vbNewLine)
                Response.Write("    window.location = 'notauthenticated.aspx';" & vbNewLine)
                Response.Write("</script>" & vbNewLine)
                Response.Write("</body>" & vbNewLine)
                Response.Write("</html>")
                Response.End()
            End If
        End Sub

        'Private Sub Page_AuthenticateRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.AuthenticateRequest

        'End Sub

        'Private Sub Page_AuthorizeRequest(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.AuthorizeRequest

        'End Sub

    End Class
#End Region

End Namespace