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

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     Configuration settings read from web.config/app.config
    ''' </summary>
    Friend Class Configuration

        ''' <summary>
        '''     The connection string to connect to the camm Web-Manager database
        ''' </summary>
        Public Shared ReadOnly Property ConnectionString() As String
            Get
                Dim Result As String = Nothing
                Try
                    Result = WebManagerSettings.Item("WebManager.ConnectionString")
                Catch
                End Try
#If NetFramework <> "1_1" Then
                If Result = Nothing Then
                    Try
                        Return System.Configuration.ConfigurationManager.ConnectionStrings("WebManagerDatabase").ConnectionString
                    Catch
                    End Try
                End If
                If Result = Nothing Then
                    Try
                        Return System.Configuration.ConfigurationManager.ConnectionStrings("WebManager").ConnectionString
                    Catch
                    End Try
                End If
#End If
                If Result = Nothing Then Result = AdditionalConfiguration("WebManager.ConnectionString")
                If Result = Nothing Then Result = AdditionalConfiguration("WebManagerDatabase")
                Return Result
            End Get
        End Property

        ''' <summary>
        '''     The SecurityObject which must be authorized for the current user
        ''' </summary>
        ''' <remarks>
        '''     You can configure this value in your web.config to ensure that e. g. a whole directory structure uses this value and is protected by this way
        ''' </remarks>
        Public Shared ReadOnly Property SecurityObject() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.SecurityObject")
                Catch
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        '''     The configured server identification string
        ''' </summary>
        ''' <value>If nothing has been defined in the web.config/app.config, it returns "1st server"</value>
        Public Shared ReadOnly Property CurrentServerIdentification() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.ServerIdentification")
                Catch
                    Return "1st server"
                End Try
            End Get
        End Property

        ''' <summary>
        '''     The configured server identification string
        ''' </summary>
        ''' <value>If nothing has been defined in the web.config/app.config, it returns "1st server"</value>
        Public Shared ReadOnly Property WebServiceCurrentServerIdentification() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.WebService.ServerIdentification")
                Catch
                    Return CurrentServerIdentification
                End Try
            End Get
        End Property

        ''' <summary>
        '''     In case there is no e-mail address data in the external account system, this default will be used
        ''' </summary>
        ''' <value>If nothing has been defined in the web.config/app.config, it doesn't return any value</value>
        Public Shared ReadOnly Property SingleSignOnDefaultEMailAddress() As String
            Get
                Return LoadStringSetting("WebManager.SingleSignOn.DefaultEMailAddress", Nothing)
            End Get
        End Property

        ''' <summary>
        ''' In case of any load errors of assemblies into the application because of security exceptions with component names in this comma-separated setting, consider those components as critical warning but not as errors
        ''' </summary>
        ''' <value></value>
        ''' <returns>A comma-separated list of component names</returns>
        ''' <remarks>The problem of SecurityExceptions while loading assemblies often happens when the camm Web-Manager application starts, a check will try to detect if there are all required components available. If errors are found, the technical contact will receive a notification e-mail on every application start with a report of the related components. The e-mail will be send only if there are errors reported; critical warnings for example won't lead to a notification e-mail.</remarks>
        Public Shared ReadOnly Property ApplicationIgnoreSecurityExceptionsForLoadingAssemblies() As String()
            Get
                Return LoadStringSetting("WebManager.Application.IgnoreSecurityExceptionsForLoadingAssemblies", "").Split(","c)
            End Get
        End Property

        ''' <summary>
        ''' In case of any load errors of assemblies into the application because of any exceptions with component names in this comma-separated setting, consider those components as critical warning but not as errors
        ''' </summary>
        ''' <value></value>
        ''' <returns>A comma-separated list of component names</returns>
        ''' <remarks>Ignoring all load exceptions makes sense sometimes when an application must replace a component by another version, or the assembly shall not be distributed, or other reasons.</remarks>
        Public Shared ReadOnly Property ApplicationIgnoreAllExceptionsForLoadingAssemblies() As String()
            Get
                Return LoadStringSetting("WebManager.Application.IgnoreAllExceptionsForLoadingAssemblies", "").Split(","c)
            End Get
        End Property

        ''' <summary>
        '''     Shall the user receive a notification e-mail if the user account has been created by the single-sign-on process?
        ''' </summary>
        ''' <value>If nothing has been defined in the web.config/app.config, it defaults to False</value>
        Public Shared ReadOnly Property SingleSignOnSuppressUserNotification() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.SingleSignOn.SuppressUserNotification", False, True)
            End Get
        End Property

        ''' <summary>
        ''' A user login name which shall be used for immediate login (requires additional server group setup)
        ''' </summary>
        ''' <value></value>
        Public Shared ReadOnly Property ImpersonationLoginName() As String
            Get
                Return LoadStringSetting("WebManager.ImpersonationLoginName", "")
            End Get
        End Property

        ''' <summary>
        '''     Is it allowed to create the cammWebManager object just on the fly?
        ''' </summary>
        ''' <value>Default value is "Off"</value>
        Public Shared ReadOnly Property CreationOnTheFlyAllowed() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.CreationOnTheFly", False, False)
            End Get
        End Property

        Public Enum NotificationLevelOnApplicationException As Byte
            ''' <summary>
            '''     No exception handling
            ''' </summary>
            [False] = 0
            ''' <summary>
            '''     No exception handling
            ''' </summary>
            Off = 0
            ''' <summary>
            '''     A notification to the technical contact person will be send, but only with the StackTrace of the exception, no source code of a page
            ''' </summary>
            ''' <remarks>
            '''     This is the most often used setting because it notifies the webmaster about existing problems but never sends senstive data like hard coded passwords nor any other source code.
            '''     A maximum of 10 notification e-mails per 10 minutes has been set.
            ''' </remarks>
            NoSourceCode = 1
            ''' <summary>
            '''     A notification to the technical contact person will be send inclusive source code which caused the exception
            ''' </summary>
            ''' <remarks>
            '''     This is ideal for fast error analysis and solving. Use this when the e-mails can never been read by hackers (e. g. the e-mail gets sent from an internal web server to your internal SMTP server to your internal workstation; so, external hackers can't read the plain text of an e-mail).
            '''     A maximum of 10 notification e-mails per 10 minutes has been set.
            ''' </remarks>
            [True] = 2
            ''' <summary>
            '''     A notification to the technical contact person will be send inclusive source code which caused the exception
            ''' </summary>
            ''' <remarks>
            '''     This is ideal for fast error analysis and solving. Use this when the e-mails can never been read by hackers (e. g. the e-mail gets sent from an internal web server to your internal SMTP server to your internal workstation; so, external hackers can't read the plain text of an e-mail).
            '''     A maximum of 10 notification e-mails per 10 minutes has been set.
            ''' </remarks>
            [On] = 2
            ''' <summary>
            '''     A notification to the technical contact person will be send inclusive source code which caused the exception
            ''' </summary>
            ''' <remarks>
            '''     This is ideal for fast error analysis and solving. Use this when the e-mails can never been read by hackers (e. g. the e-mail gets sent from an internal web server to your internal SMTP server to your internal workstation; so, external hackers can't read the plain text of an e-mail).
            '''     A maximum of 10 notification e-mails per 10 minutes has been set.
            ''' </remarks>
            TechnicalContact = 2
            ''' <summary>
            '''     No notifications will be sent to the technical contact, but on the web site the error page will be shown instead of the standard ASP.NET error page
            ''' </summary>
            NoNotifications = 3
            ''' <summary>
            '''     A notification to the development contact person will be send inclusive source code which caused the exception
            ''' </summary>
            ''' <remarks>
            '''     This is ideal for fast error analysis and solving. Use this when the e-mails can never been read by hackers (e. g. the e-mail gets sent from an internal web server to your internal SMTP server to your internal workstation; so, external hackers can't read the plain text of an e-mail).
            '''     A maximum of 10 notification e-mails per 10 minutes has been set.
            ''' </remarks>
            Developer = 4
            ''' <summary>
            '''     A notification to the technical contact as well as to the development contact person will be send inclusive source code which caused the exception
            ''' </summary>
            ''' <remarks>
            '''     This is ideal for fast error analysis and solving. Use this when the e-mails can never been read by hackers (e. g. the e-mail gets sent from an internal web server to your internal SMTP server to your internal workstation; so, external hackers can't read the plain text of an e-mail).
            '''     A maximum of 10 notification e-mails per 10 minutes has been set.
            ''' </remarks>
            TechnicalContactAndDeveloper = 5
        End Enum
        ''' <summary>
        '''     Exception handling of camm Web-Manager for the whole web application
        ''' </summary>
        ''' <remarks>When there are 404 File not found exceptions or 500 Script errors in your web application, do you want to get a notification e-mail as the technical contact person? With some source code included?</remarks>
        Public Shared ReadOnly Property NotifyOnApplicationExceptions() As NotificationLevelOnApplicationException
            Get
                Try
                    If WebManagerSettings.Item("WebManager.NotifyOnApplicationExceptions") Is Nothing Then
                        Return NotificationLevelOnApplicationException.NoSourceCode
                    Else
                        Return CType(WebManagerSettings.Item("WebManager.NotifyOnApplicationExceptions"), NotificationLevelOnApplicationException)
                    End If
                Catch
                    Return NotificationLevelOnApplicationException.NoSourceCode
                End Try
            End Get
        End Property

        ''' <summary>
        '''     Exception handling of camm Web-Manager for the whole web application in case of 404 errors (file not found)
        ''' </summary>
        ''' <remarks>When there are 404 (File not found) exceptions in your web application, do you want to get a notification e-mail?</remarks>
        Public Shared ReadOnly Property NotifyOnApplicationExceptions404() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.NotifyOnApplicationExceptions.404", True, True)
            End Get
        End Property

        ''' <summary>
        '''     Exception handling of camm Web-Manager for the whole web application in case of 404 errors (file not found) if caused by crawlers
        ''' </summary>
        ''' <remarks>When there are 404 (File not found) exceptions in your web application caused by crawler requests, do you want to get a notification e-mail?</remarks>
        Public Shared ReadOnly Property NotifyOnApplicationExceptions404IgnoreCrawlers() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.NotifyOnApplicationExceptions.404.IgnoreBots", False, False)
            End Get
        End Property

        ''' <summary>
        '''     Exception handling of camm Web-Manager may set the HTTP response status code to e.g. 404 or 500 
        ''' </summary>
        ''' <remarks>
        ''' <para>Exception handling of camm Web-Manager may set the HTTP response status code to e.g. 404 or 500 - causing IIS to replace the page result with its own error page. When disabled, error pages will use HTTP response status code 200.</para>
        ''' <para>Check the alternative to use following web.config setup to force IIS 7 (and higher) to display the error page as prepared by the ASP.NET web application<code>&lt;system.webServer&gt;
        ''' &lt;httpErrors errorMode = "Custom" existingResponse="PassThrough"/&gt;
	    ''' &lt;/system.webServer&gt;</code></para>
        ''' </remarks>
        Public Shared ReadOnly Property NotifyOnApplicationExceptionsReplaceResponseStatusCode() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.NotifyOnApplicationExceptions.ReplaceResponseStatusCode", True, True)
            End Get
        End Property

        ''' <summary>
        ''' camm Web-Manager can automatically bind all page controls in its Page_Load event if desired
        ''' </summary>
        Public Shared ReadOnly Property DataBindAutomaticallyWhilePageOnLoad() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.DataBindAutomaticallyWhilePageOnLoad", False, True)
            End Get
        End Property

        ''' <summary>
        '''     Exception handling of camm Web-Manager transfers to this URL (not a redirect!)
        ''' </summary>
        Public Shared ReadOnly Property TransferRequestOnApplicationExceptionsTo() As String
            Get
                Try
                    If WebManagerSettings.Item("WebManager.TransferRequestOnApplicationExceptionsTo") Is Nothing Then
                        Return "/sysdata/error.aspx"
                    Else
                        Dim Result As String
                        Result = CType(WebManagerSettings.Item("WebManager.TransferRequestOnApplicationExceptionsTo"), String)
                        If System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(Result)) Then
                            Return Result
                        Else
                            Return "/sysdata/error.aspx"
                        End If
                    End If
                Catch
                    Return "/sysdata/error.aspx"
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Debug level of camm Web-Manager
        ''' </summary>
        ''' <value>The new debug level</value>
        ''' <remarks>The logging of warnings, errors can be influenced by this property as well as redirection of e-mails and the functionality level of automatic redirects. This is very helpfull on first setup of camm Web-Manager if you experience any problems or you don't want e-mails to be sent to the origin receipients except you yourself.</remarks>
        Public Shared ReadOnly Property DebugLevel() As DebugLevels
            Get
                Try
                    Return CType(WebManagerSettings.Item("WebManager.DebugLevel"), DebugLevels)
                Catch
                    Return DebugLevels.NoDebug
                End Try
            End Get
        End Property

        ''' <summary>
        ''' The SMTP server name for sending e-mails
        ''' </summary>
        ''' <value></value>
        ''' <remarks>The camm Web-Manager is heavily using e-mails for information and workflow purposes.</remarks>
        Public Shared ReadOnly Property SmtpServerName() As String
            Get
                Dim Result As String = Nothing
                Try
                    Return WebManagerSettings.Item("WebManager.SMTPServerName")
                Catch
                End Try
                If Result <> Nothing Then
                    Return Result
                Else
                    Return "localhost"
                End If
            End Get
        End Property
        ''' <summary>
        ''' The SMTP server port for sending e-mails
        ''' </summary>
        ''' <value></value>
        ''' <remarks>The camm Web-Manager is heavily using e-mails for information and workflow purposes.</remarks>
        Public Shared ReadOnly Property SmtpServerPort() As Integer
            Get
                Dim Result As Integer = 25
                Try
                    Dim SettingValue As String = WebManagerSettings.Item("WebManager.SMTPServerPort")
                    If SettingValue Is Nothing OrElse SettingValue = "" Then
                        'Default Port
                    Else
                        Result = CType(SettingValue, Integer)
                    End If
                Catch
                End Try
                Return Result
            End Get
        End Property
        ''' <summary>
        ''' The common title of the 1st level support team
        ''' </summary>
        ''' <value>The public title for the 1st level support team contact address</value>
        ''' <remarks>The e-mail address is used for all common activities inside of the camm WebManager world, for example a general contact for feedback or support requests. Mostly, the security administrators get all those e-mail. Depending of the content, they can answer directly or redirect to a 2nd level support team.</remarks>
        Public Shared ReadOnly Property StandardEMailAccountName() As String
            Get
                Dim Result As String = Nothing
                Try
                    Result = WebManagerSettings.Item("WebManager.StandardEMailAccountName")
                Catch
                End Try
                If Result <> Nothing Then
                    Return Result
                Else
                    Return "admin@localhost"
                End If
            End Get
        End Property
        ''' <summary>
        ''' The common e-mail address of the 1st level support team
        ''' </summary>
        ''' <value>Valid e-mail address</value>
        ''' <remarks>The e-mail address is used for all common activities inside of the camm WebManager world, for example a general contact for feedback or support requests. Mostly, the security administrators get all those e-mail. Depending of the content, they can answer directly or redirect to a 2nd level support team.</remarks>
        Public Shared ReadOnly Property StandardEMailAccountAddress() As String
            Get
                Dim Result As String = Nothing
                Try
                    Result = WebManagerSettings.Item("WebManager.StandardEMailAccountAddress")
                Catch
                End Try
                If Result <> Nothing Then
                    Return Result
                Else
                    Return "admin@localhost"
                End If
            End Get
        End Property
        ''' <summary>
        ''' The e-mail address of the developer is used when errors shall be reported
        ''' </summary>
        ''' <value>Valid e-mail address</value>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property DevelopmentEMailAccountAddress() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.DevelopmentEMailAccountAddress")
                Catch
                    Return "admin@localhost"
                End Try
            End Get
        End Property
        ''' <summary>
        ''' The common e-mail address of the technical support team
        ''' </summary>
        ''' <value>The public title for the techical support team contact address</value>
        ''' <remarks>The e-mail address is used for all technical activities.</remarks>
        Public Shared ReadOnly Property TechnicalServiceEMailAccountName() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.TechnicalServiceEMailAccountName")
                Catch
                    Return "admin@localhost"
                End Try
            End Get
        End Property
        ''' <summary>
        ''' The common e-mail address of the technical support team
        ''' </summary>
        ''' <value>Valid e-mail address</value>
        ''' <remarks>The e-mail address is used for all technical activities.</remarks>
        Public Shared ReadOnly Property TechnicalServiceEMailAccountAddress() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.TechnicalServiceEMailAccountAddress")
                Catch
                    Return "admin@localhost"
                End Try
            End Get
        End Property
        ''' <summary>
        '''     Configures the authentication methods for the SMTP server
        ''' </summary>
        ''' <value>The authentification type, possible values are "" or "NONE", "LOGIN", "PLAIN", "CRAM-MD5", or "NTLM"</value>
        Public Shared ReadOnly Property SmtpAuthentificationMode() As String
            Get
                Try
                    Dim Result As String = WebManagerSettings.Item("WebManager.SMTPServerAuthentificationMode")
                    If Result Is Nothing Then
                        Result = ""
                    Else
                        Result = Result.ToUpper(System.Globalization.CultureInfo.InvariantCulture)
                    End If
                    Select Case Result
                        Case "", "NONE", "LOGIN", "PLAIN", "CRAM-MD5", "NTLM"
                        Case Else
                            Throw New ArgumentException("The authentification type, possible values are """" or ""NONE"", ""LOGIN"", ""PLAIN"", ""CRAM-MD5"", or ""NTLM""")
                    End Select
                    Return Result
                Catch
                    Return Nothing
                End Try
            End Get
        End Property
        ''' <summary>
        '''     Sets up an user name for the SMTP server
        ''' </summary>
        ''' <value>The user name</value>
        Public Shared ReadOnly Property SmtpUsername() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.SMTPUsername")
                Catch
                    Try
                        Return WebManagerSettings.Item("WebManager.SMTPServerUsername")
                    Catch
                        Return Nothing
                    End Try
                End Try
            End Get
        End Property
        ''' <summary>
        '''     Sets up a password for the SMTP server
        ''' </summary>
        ''' <value>The password</value>
        Public Shared ReadOnly Property SmtpPassword() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.SMTPPassword")
                Catch
                    Try
                        Return WebManagerSettings.Item("WebManager.SMTPServerPassword")
                    Catch
                        Return Nothing
                    End Try
                End Try
            End Get
        End Property

        ''' <summary>
        '''     A compact policy header to be sent to the client
        ''' </summary>
        Public Shared ReadOnly Property CompactPolicyHeader() As String
            Get
                Try
                    Return WebManagerSettings.Item("WebManager.CompactPolicy")
                Catch
                    Return Nothing
                End Try
            End Get
        End Property

        ''' <summary>
        '''     Does this site uses a frameset?
        ''' </summary>
        Public Shared ReadOnly Property UseFrameset() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.UseFrameset", False, False)
            End Get
        End Property

        ''' <summary>
        '''     Allow logon processing with GET and POST request methods (true) or only with POST data (false)
        ''' </summary>
        Public Shared ReadOnly Property AllowLogonViaRequestTypeGet() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.AllowLogonViaRequestTypeGet", False, False)
            End Get
        End Property

        ''' <summary>
        '''     Run camm Web-Manager in cookieless mode
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     When camm Web-Manager runs in cookieless mode, then 
        '''     <list>
        '''         <item>all internal links track the SessionID per default (overwrite CalculateUrl method to get a custom behaviour)</item>
        '''         <item>it caches navigation items with an additional SessionID value in the primary key field</item>
        '''     </list>
        ''' </remarks>
        Public Shared ReadOnly Property CookieLess() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.CookieLess", False, False)
            End Get
        End Property

        ''' <summary>
        '''     Use the integrated mail queue for sending e-mails
        ''' </summary>
        ''' <value>TripleState.Undefined is for auto-detection, use TripleState.False for direct sending without mail queue or TripleState.True for sending via mail queue</value>
        Public Shared ReadOnly Property ProcessMailQueue() As TripleState
            Get
                Return LoadTripleStateSetting("WebManager.ProcessMailQueue", True)
            End Get
        End Property

        ''' <summary>
        '''     Defines weather to put data in "Session" or in "Cache".
        ''' </summary>
        ''' <remarks>
        '''     Reads set value from web.config. "True" indicates to use "Session" for holding data in memory, else hold data in "Http Cache"
        ''' </remarks>
        Public Shared ReadOnly Property DownloadHandlerSeparateRequestPutDataInSession() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.DownloadHandlerSeparateRequestPutDataInSession", False, True)
            End Get
        End Property

        ''' <summary>
        ''' Maximum download size for a file collection (e. g. several single files delivered in a ZIP archive)
        ''' </summary>
        ''' <value>Defaults to 4 MB or at least the same value as DownloadHandlerMaxFileSize</value>
        ''' <remarks>
        ''' <para>The collection size has got a maximum value, too. This ensures that ZIP archives (which are created on the fly for delivering several single files in 1 download) aren't without size limits.</para>
        ''' <seealso cref="DownloadHandlerMaxFileSize" />
        ''' </remarks>
        Public Shared ReadOnly Property DownloadHandlerMaxFileCollectionSize() As Long
            Get
                Return System.Math.Max(DownloadHandlerMaxFileSize, LoadLongSetting("WebManager.DownloadHandlerMaxFileCollectionSize", 4000000, True)) '4 MB per default
            End Get
        End Property

        ''' <summary>
        ''' Maximum file download size 
        ''' </summary>
        ''' <value>Defaults to 4 MB</value>
        ''' <remarks>This value ensures that downloads aren't without any size limits. A typical value for most cases is 4 MB, but you can increase/decrease the value for your requirements.
        ''' <seealso cref="DownloadHandlerMaxFileCollectionSize" />
        ''' </remarks>
        Public Shared ReadOnly Property DownloadHandlerMaxFileSize() As Long
            Get
                Return LoadLongSetting("WebManager.DownloadHandlerMaxFileSize", 4000000, True) '4 MB per default
            End Get
        End Property

        ''' <summary>
        '''     Defines the time limit in minutes to hold download data in Cache
        ''' </summary>
        ''' <remarks>
        '''     The default time limit is "5" minits.
        ''' </remarks>
        Public Shared ReadOnly Property DownloadHandlerSeparateRequestCacheTimeLimit_InMinutes() As Integer
            Get
                Dim result As Integer = 5 ' 5 minutes is default value
                Try
                    Dim check As Integer = CType(WebManagerSettings("WebManager.DownloadHandlerSeparateRequestCacheTimeLimitInMinutes"), Integer)
                    If check <> 0 Then
                        result = CType(WebManagerSettings("WebManager.DownloadHandlerSeparateRequestCacheTimeLimitInMinutes"), Integer)
                    End If
                Catch
                End Try
                Return result
            End Get
        End Property
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use DownloadHandlerSeparateRequestCacheTimeLimit_InMinutes instead")> Public Shared ReadOnly Property DownloadHandlerSeparateRequestCacheTimeLimit_InMinites() As Integer
            Get
                Return DownloadHandlerSeparateRequestCacheTimeLimit_InMinutes
            End Get
        End Property

        ''' <summary>
        '''     The language which shall be used when it can't be detected by any other way
        ''' </summary>
        Public Shared ReadOnly Property GlobalizationDefaultMarket() As Integer
            Get
                'Return the forced language if there is one
                If GlobalizationForcedMarket <> Nothing Then
                    Return GlobalizationForcedMarket
                End If
                'Retrieve a configured default value
                'Otherwise it's english (= 1)
                Return Configuration.LoadIntegerSetting("WebManager.Globalization.DefaultMarket", Configuration.LoadIntegerSetting("WebManager.Languages.Default", 1, True), True)
            End Get
        End Property

        ''' <summary>
        '''     A comma separated list of market IDs which are supported by the current web application and which are available for the users
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' Web applications are provided by independent developers and manufacturers. Often the application is provided for 1 or 2 markets only. In this case, many other activated markets and languages of the portal would be in danger to show up translated partially because of some text blocks translated and provided by the application logic, some other blocks handled by the portal itself and fully available in all activated languages (e.g. footer information like a link to the data protection rules). In this case, it is better to show up the application GUI in only the supported language of the application itself.
        ''' <example>
        ''' Your portal is setup to provide markets English, English (USA), English (Canada) and Spanish. Your application has been developed for markets English (USA) and English (Canada). So, the application can appear fully translated in USA and Canada. English users from other countries or Spanish users are not supported by the application. If a user comes to the application with e. g. English (without market information), the application won't show up in standard English, but it will appear in the application's <see cref="CompuMaster.camm.WebManager.Configuration.GlobalizationDefaultMarket">default market</see>.
        ''' </example>
        ''' <para>If not defined, the general list of activated markets will be used.</para>
        ''' </remarks>
        Public Shared ReadOnly Property GlobalizationSupportedMarkets() As Integer()
            Get
                Try
                    'Return the forced language if there is one
                    If GlobalizationForcedMarket <> Nothing Then
                        Return New Integer() {GlobalizationForcedMarket}
                    End If
                    'Return the configured value
                    Dim Result As String
                    Result = Configuration.LoadStringSetting("WebManager.Globalization.SupportedMarkets", Nothing)
                    If Result = Nothing Then
                        Result = Configuration.LoadStringSetting("WebManager.ActiveMarkets", Nothing)
                    End If
                    Return Utils.SplitStringToInteger(Result, ","c)
                Catch
                    Return New Integer() {}
                End Try
            End Get
        End Property

        ''' <summary>
        '''     The language which shall be forced for the GUI
        ''' </summary>
        Public Shared ReadOnly Property GlobalizationForcedMarket() As Integer
            Get
                Return Configuration.LoadIntegerSetting("WebManager.Globalization.ForcedMarket", Configuration.LoadIntegerSetting("WebManager.Languages.ForcedLanguage", Nothing, True), True)
            End Get
        End Property

        ''' <summary>
        ''' Every WebEditor content is related to a server; this property overrides the server ID value where to read from/save to
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' 0 = currently used server; other values = forced server ID
        ''' </remarks>
        Public Shared ReadOnly Property WebEditorContentOfServerID() As Integer
            Get
                Dim Result As Integer
                Try
                    Result = CType(WebManagerSettings("WebManager.WebEditor.ContentOfServerID"), Integer)
                Catch
                End Try
                Return Result
            End Get
        End Property

        ''' <summary>
        ''' To support reverse proxy environments, you need to activate the interpretion of the additional request header X-Forwarded-For which is typically created by the reverse proxy. This allows you to receive the IP address of the client that is connecting to the reverse proxy. In those cases, this is the more correct IP address of the client and the possibly more interesting value for you.
        ''' </summary>
        Public Shared ReadOnly Property InterpreteRequestHeaderXForwardedFor() As Boolean
            Get
                Return Configuration.LoadBooleanSetting("WebManager.InterpreteRequestHeaderXForwardedFor", False, False)
            End Get
        End Property

        ''' <summary>
        '''     Always regard the crawlers as an anonymous user and do not test authorization with database lookups
        ''' </summary>
        ''' <remarks>
        '''     <para>Based on the browser identification string, all browsers and also the crawlers are identifying themselves.</para>
        '''     <para>If one of these browsers identifies itself as a crawler, it can mostly be seen as an anonymous user. This helps saving unneeded roundtrips to the database for asking user account information and authorization states.</para>
        '''     <para>To reduce heavy loads while a crawler is visiting the website, all security objects other than @@Anonymous are handled as forbidden by default, even if a normal SecurityObject has got authorization for anonymous user.</para>
        '''     <para>In case you run into crawler indexing problems, consider to set this value to False in your configuration or configure your pages and applications to ask for SecurityObject=&quot;@@Anonymous&quot;.</para>
        ''' </remarks>
        Public Shared ReadOnly Property SecurityRecognizeCrawlersAsAnonymous() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.Security.RecognizeCrawlersAsAnonymous", True, False)
            End Get
        End Property

        ''' <summary>
        ''' EventLog trace enables reporting of the current page request progress
        ''' </summary>
        ''' <remarks>
        ''' This trace feature is especially useful in StackTrace overflow situations and may give a hint to identify the code area causing the problem.
        ''' </remarks>
        Public Shared ReadOnly Property EventLogTrace() As Boolean
            Get
                Return LoadBooleanSetting("WebManager.EventLogTrace", False, True)
            End Get
        End Property

        ''' <summary>
        ''' The working folder of Download-Handler, defaults to /system/downloads/
        ''' </summary>
        Public Shared ReadOnly Property DownloadHandlerVirtualTempPath() As String
            Get
                Dim Result As String = WebManagerSettings("WebManager.DownloadHandlerVirtualTempPath")
                If Result = Nothing Then Result = AdditionalConfiguration("WebManager.DownloadHandlerVirtualTempPath")
                If Result = Nothing Then
                    Result = "/system/downloads/"
                End If
                Return Result
            End Get
        End Property

        ''' <summary>
        ''' Recognize all database builds up to this number to be compatible with the current assembly version
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' In some situations, you update the camm Web-Manager database but still want to keep running an older version of the assembly in combination with the newer database version. This is possible, but not recommended. Do it on your own risk.
        ''' </remarks>
        Public Shared ReadOnly Property CompatibilityWithDatabaseBuild() As Integer
            Get
                Return LoadIntegerSetting("WebManager.CompatibilityWithDatabaseBuild", Setup.ApplicationUtils.Version.Build, True)
            End Get
        End Property

#Region "TextModules"
        ''' <summary>
        '''     Default ServerGroupID for load/save of text modules
        ''' </summary>
        ''' <remarks>
        '''     The ServerGroupID is typically diffferent for intranet and extranet.
        '''     By this setting complete cammWM.TextModule library will use the configured ServerGroupID for all loading/saving in methods with undefined server group ID
        ''' </remarks>
        Public Shared ReadOnly Property TextModulesServerGroupIDDefault() As System.Int32
            Get
                Static result As System.Int32
                If result = Nothing AndAlso WebManagerSettings("WebManager.TextModules.ServerGroupID.Default") <> Nothing Then
                    Try
                        result = Convert.ToInt32(WebManagerSettings("WebManager.TextModules.ServerGroupID.Default"))
                    Catch
                    End Try
                End If
                Return result
            End Get
        End Property
        ''' <summary>
        '''     Default ServerGroupID for load/save of text modules
        ''' </summary>
        ''' <remarks>
        '''     The ServerGroupID is typically diffferent for intranet and extranet.
        '''     By this setting complete cammWM.TextModule library will always use the configured ServerGroupID for all loading/saving even if a parameter of a method defines a server group ID
        ''' </remarks>
        Public Shared ReadOnly Property TextModulesServerGroupIDForced() As System.Int32
            Get
                Static result As System.Int32
                If result = Nothing AndAlso WebManagerSettings("WebManager.TextModules.ServerGroupID.Forced") <> Nothing Then
                    Try
                        result = Convert.ToInt32(WebManagerSettings("WebManager.TextModules.ServerGroupID.Forced"))
                    Catch
                    End Try
                End If
                Return result
            End Get
        End Property
#End Region

        Public Shared ReadOnly Property PBKDF2Rounds() As Integer
            Get
                Return LoadIntegerSetting("WebManager.Crypto.PBKDF2.Rounds", 200000, True) 'for 2014 200000 is a recommended choice 
            End Get
        End Property

        Public Shared ReadOnly Property AES256Key() As String
            Get
                Return LoadStringSetting("WebManager.Crypto.AES256.Key_AsciiCharsOnly", Nothing)
            End Get
        End Property

        Friend Shared ReadOnly Property SuppressProductRegistrationServiceConnection As Boolean
            Get
                Return LoadBooleanSetting("WebManager.SuppressProductRegistrationServiceConnection", False, True)
            End Get
        End Property

        Public Shared ReadOnly Property PasswordTokenExpirationHours() As Integer
            Get
                Return LoadIntegerSetting("WebManager.PasswordToken.ExpirationHours", 24, True)
            End Get
        End Property

        ''' <summary>
        ''' In case of any load errors of assemblies into the application because of security exceptions with component names in this comma-separated setting, consider those components as critical warning but not as errors
        ''' </summary>
        ''' <value></value>
        ''' <returns>A comma-separated list of component names</returns>
        ''' <remarks>The problem of SecurityExceptions while loading assemblies often happens when the camm Web-Manager application starts, a check will try to detect if there are all required components available. If errors are found, the technical contact will receive a notification e-mail on every application start with a report of the related components. The e-mail will be send only if there are errors reported; critical warnings for example won't lead to a notification e-mail.</remarks>
        Public Shared ReadOnly Property UserCloneExludedAdditionalFlags() As String()
            Get
                Dim values As String() = LoadStringSetting("WebManager.User.CloneExludedAdditionalFlags", "").Split(","c)

#If NetFramework <> "1_1" Then
                Dim result As New List(Of String)
                For Each value As String In values
                    value = Trim(value)
                    If Not String.IsNullOrEmpty(value) AndAlso Not result.Contains(value) Then result.Add(value)
                Next
                Return result.ToArray()
#Else
                Dim result As New ArrayList()
                For Each value As String In values
                    value = Trim(value)
                    If Not value Is Nothing AndAlso Not value = "" AndAlso Not result.Contains(value) Then 
                        result.Add(value)
                    End If
                Next
                Return CType(result.ToArray(), String())
#End If
            End Get
        End Property

#Region "Load configuration setting helper methods"

        Friend Shared ReadOnly Property WebManagerSettings() As System.Collections.Specialized.NameValueCollection
            Get
#If VS2015OrHigher = True Then
#Disable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If
                Return System.Configuration.ConfigurationSettings.AppSettings
#If VS2015OrHigher = True Then
#Enable Warning BC40000 'obsolete warnings
#End If
            End Get
        End Property

        ''' <summary>
        '''     Load a boolean value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        Private Shared Function LoadTripleStateSetting(ByVal appSettingName As String, ByVal suppressExceptions As Boolean) As TripleState
            Dim Result As TripleState = TripleState.Undefined
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If Not value Is Nothing Then
                    value = value.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                Else
                    value = ""
                End If
                Select Case value
                    Case "0", "off", "false"
                        Result = TripleState.False
                    Case "1", "-1", "on", "true"
                        Result = TripleState.True
                    Case "", "auto"
                        'keep default
                    Case Else
                        Result = Utils.BooleanToWMTriplestate(Boolean.Parse(value))
                End Select
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose ""off"" or ""on"" or ""auto"", please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' <summary>
        '''     Load a boolean value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        Private Shared Function LoadBooleanSetting(ByVal appSettingName As String, ByVal defaultValue As Boolean, ByVal suppressExceptions As Boolean) As Boolean
            Dim Result As Boolean = defaultValue
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If Not value Is Nothing Then
                    value = value.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                Else
                    value = ""
                End If
                Select Case value
                    Case "0", "off", "false"
                        Result = False
                    Case "1", "-1", "on", "true"
                        Result = True
                    Case ""
                        'keep default
                    Case Else
                        Result = Boolean.Parse(value)
                End Select
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose ""true"" or ""false"", please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' <summary>
        '''     Load an integer value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        Friend Shared Function LoadIntegerSetting(ByVal appSettingName As String, ByVal defaultValue As Integer, ByVal suppressExceptions As Boolean) As Integer
            Dim Result As Integer = defaultValue
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If value = Nothing Then
                    Result = defaultValue
                Else
                    Result = Integer.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
                End If
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose a valid integer number, please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' <summary>
        '''     Load an Int64 value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
        Friend Shared Function LoadLongSetting(ByVal appSettingName As String, ByVal defaultValue As Long, ByVal suppressExceptions As Boolean) As Long
            Dim Result As Long = defaultValue
            Try
                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
                If value = Nothing Then
                    Result = defaultValue
                Else
                    Result = Integer.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
                End If
            Catch ex As Exception
                If suppressExceptions = False Then
                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose a valid integer number, please.", ex)
                End If
            End Try
            Return Result
        End Function

        ''' <summary>
        '''     Load a string value from the configuration file
        ''' </summary>
        ''' <param name="appSettingName">The name of the appSetting item</param>
        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
        Friend Shared Function LoadStringSetting(ByVal appSettingName As String, ByVal defaultValue As String) As String
            Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
            If value = Nothing Then value = AdditionalConfiguration(appSettingName)
            If value = Nothing Then
                Return defaultValue
            Else
                Return value
            End If
        End Function
#End Region

    End Class

End Namespace