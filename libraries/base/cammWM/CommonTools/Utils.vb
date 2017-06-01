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

Imports System.Text
Imports System.IO

Namespace CompuMaster.camm.WebManager

    Public Class Utils

#Region "Crawler detection"
        ''' <summary>
        ''' Evaluate by the given request headers if a browser is a search robot
        ''' </summary>
        ''' <param name="request">The current HTTP request</param>
        ''' <remarks>
        ''' <seealso cref="IsRequestFromCrawlerOrPotentialMachineAgent" />
        ''' <seealso cref="IsRequestFromPotentialMachineAgent" />
        ''' </remarks>
        Public Shared Function IsRequestFromCrawlerAgent(ByVal request As Web.HttpRequest) As Boolean
            Dim UserAgentLowered As String = Utils.StringNotNothingOrEmpty(request.Headers("User-Agent")).ToLower
            If request.Browser.Crawler Then
                Return True
            ElseIf Utils.StringNotNothingOrEmpty(request.Headers("From")).ToLower.IndexOf("googlebot") > -1 Then
                Return True
            ElseIf UserAgentLowered.IndexOf("googlebot") > -1 Then
                'Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)
                Return True
            ElseIf UserAgentLowered.IndexOf("slurp") > -1 Then
                'Mozilla/5.0 (compatible; Yahoo! Slurp; http://help.yahoo.com/help/us/ysearch/slurp)
                Return True
            ElseIf UserAgentLowered.IndexOf("msnbot/") > -1 Then
                'msnbot/0.9 (+http://search.msn.com/msnbot.htm)
                Return True
            ElseIf UserAgentLowered.IndexOf("crawler") > -1 Then
                'some other type of crawler
                Return True
            ElseIf UserAgentLowered.IndexOf("spider") > -1 Then
                'some other type of crawler
                Return True
            ElseIf UserAgentLowered.IndexOf("robot") > -1 Then
                'some other type of crawler
                Return True
            ElseIf UserAgentLowered = "ibot" Then
                'some other type of crawler
                Return True
            ElseIf UserAgentLowered.IndexOf(" bot") > -1 OrElse UserAgentLowered.IndexOf("/bot") > -1 OrElse UserAgentLowered.IndexOf("bot/") > -1 OrElse UserAgentLowered.IndexOf("bot ") > -1 OrElse UserAgentLowered.IndexOf("bot") = 0 Then
                'some other type of crawler
                Return True
            Else
                'Normal browser (or another spider/crawler which is not identified as such one)
                Return False
            End If
        End Function

        ''' <summary>
        ''' Evaluate by the given request headers if the browser is potentially a crawler agent, a machine or a mis-configured browser agent indicating that the client is not a typical browser with a user interace
        ''' </summary>
        ''' <param name="request">The current HTTP request</param>
        ''' <remarks>
        ''' Following requests will be evaluated to be a potential crawler/to be not a typical browser with user interface
        ''' <list>
        ''' <item>Java/{version number}</item>
        ''' <item>libwww*</item>
        ''' <item>All other clients which have been identified as a regular crawler</item>
        ''' </list>
        ''' <seealso cref="IsRequestFromPotentialMachineAgent" />
        ''' <seealso cref="IsRequestFromCrawlerAgent" />
        ''' </remarks>
        Public Shared Function IsRequestFromCrawlerOrPotentialMachineAgent(ByVal request As Web.HttpRequest) As Boolean
            If IsRequestFromCrawlerAgent(request) Then
                'A regular crawler
                Return True
            ElseIf IsRegExMatch("Java/d*.d*.d*", Utils.StringNotNothingOrEmpty(request.Headers("User-Agent"))) Then
                'Java/1.6.0
                Return True
            ElseIf IsRegExMatch("Java/d*.d*.d*_d*", Utils.StringNotNothingOrEmpty(request.Headers("User-Agent"))) Then
                'Java/1.6.0_14
                Return True
            ElseIf IsRegExMatch("libwww.*", Utils.StringNotNothingOrEmpty(request.Headers("User-Agent"))) Then
                'libwww-perl/5.834
                Return True
            Else
                'Normal browser (or another spider/crawler which is not identified as such one)
                Return False
            End If
        End Function

        ''' <summary>
        ''' Evaluate by the given request headers if the browser is potentially an application, a machine or a mis-configured browser agent indicating that the client is not a typical browser with a user interace
        ''' </summary>
        ''' <param name="request">The current HTTP request</param>
        ''' <remarks>
        ''' A crawler request will be evaluated with a False result.
        ''' Following requests will be evaluated to be a potential crawler/to be not a typical browser with user interface
        ''' <list>
        ''' <item>Java/{version number}</item>
        ''' <item>libwww*</item>
        ''' </list>
        ''' <seealso cref="IsRequestFromCrawlerOrPotentialMachineAgent" />
        ''' <seealso cref="IsRequestFromCrawlerAgent" />
        ''' </remarks>
        Public Shared Function IsRequestFromPotentialMachineAgent(ByVal request As Web.HttpRequest) As Boolean
            If IsRequestFromCrawlerAgent(request) Then
                'A regularly identified crawler
                Return False
            Else
                'All other cases
                Return IsRequestFromCrawlerOrPotentialMachineAgent(request)
            End If
        End Function

#End Region

#Region "Support for reverse proxy environments"
        ''' <summary>
        '''     Lookup the real IP address of the remote client in a current HttpContext
        ''' </summary>
        ''' <returns>A remote IP address of the client behind a reverse proxy</returns>
        ''' <remarks>
        '''     <para>In case that there is a reverse proxy which forwards all incoming requests to your web-server, the regularily reported remote IP address is the one of the reverse proxy. But sometimes you'll need the IP address of the real client.</para>
        '''     <para>This method analyzes the request headers and if the reverse proxy reports the client IP, it will be returned instead of normal remote IP.</para>
        '''     <para>Sometimes if there are multiple cascaded proxies or reverse proxies, then you may get a value in headers like 'client-ip, proxy-ip, reverse-proxy-1-ip, reverse-proxy-2-ip'. In this situation, this function will return the IP address of the partner of the latest reverse proxy, in this example reverse-proxy-2-ip.</para>
        '''     <para>ATTENTION: Because request headers can be manipulated, the general requesting of this value in unsecure. It's only a trustable value if you really run a reverse proxy environment. That's why you need to activate the interpretation of the request header 'X-Forwarded-For' in your web.config by configuring the setting WebManager.InterpreteRequestHeaderXForwardedFor.</para>
        ''' </remarks>
        Public Shared Function LookupRealRemoteClientIPOfHttpContext() As String
            Return LookupRealRemoteClientIPOfHttpContext(System.Web.HttpContext.Current)
        End Function
        ''' <summary>
        '''     Lookup the real IP address of the remote client in a current HttpContext
        ''' </summary>
        ''' <param name="httpContext">An HttpContext object</param>
        ''' <returns>A remote IP address of the client behind a reverse proxy</returns>
        ''' <remarks>
        '''     <para>In case that there is a reverse proxy which forwards all incoming requests to your web-server, the regularily reported remote IP address is the one of the reverse proxy. But sometimes you'll need the IP address of the real client.</para>
        '''     <para>This method analyzes the request headers and if the reverse proxy reports the client IP, it will be returned instead of normal remote IP.</para>
        '''     <para>Sometimes if there are multiple cascaded proxies or reverse proxies, then you may get a value in headers like 'client-ip, proxy-ip, reverse-proxy-1-ip, reverse-proxy-2-ip'. In this situation, this function will return the IP address of the partner of the latest reverse proxy, in this example reverse-proxy-2-ip.</para>
        '''     <para>ATTENTION: Because request headers can be manipulated, the general requesting of this value in unsecure. It's only a trustable value if you really run a reverse proxy environment. That's why you need to activate the interpretation of the request header 'X-Forwarded-For' in your web.config by configuring the setting WebManager.InterpreteRequestHeaderXForwardedFor.</para>
        ''' </remarks>
        Public Shared Function LookupRealRemoteClientIPOfHttpContext(ByVal httpContext As System.Web.HttpContext) As String
            If httpContext Is Nothing OrElse HttpContext.Current Is Nothing Then
                Throw New ArgumentNullException("httpContext")
            End If
            If Configuration.InterpreteRequestHeaderXForwardedFor AndAlso Trim(httpContext.Request.Headers("X-Forwarded-For")) <> "" Then
                'Read client IP reported by Apache/Squid (Reverse) Proxy (Attention: also regular proxies may report this header, not only reverse proxies!)
                Dim HeaderValue As String = Trim(httpContext.Request.Headers("X-Forwarded-For")) 'e.g. "145.254.12.4" or "192.168.0.5, unknown, 145.65.75.14, 124.16.2.18"
                If HeaderValue.LastIndexOf(","c) > -1 Then
                    HeaderValue = Trim(HeaderValue.Substring(HeaderValue.LastIndexOf(","c) + 1))
                End If
                Return HeaderValue
            Else
                Return httpContext.Request.UserHostAddress
            End If
        End Function
#End Region

#Region "HttpCaching"
        ''' <summary>
        ''' Set an HttpCache item to a new value respectively remove it if the value is Nothing
        ''' </summary>
        ''' <param name="key">The key name for the cache item</param>
        ''' <param name="value">The new value</param>
        ''' <param name="priority">The cache priority</param>
        ''' <exception cref="System.SystemException">
        ''' If HttpContext.Current is not available (if it is not a web application), there will be thrown a normal System.Exception
        ''' </exception>
        Friend Shared Sub SetHttpCacheValue(ByVal key As String, ByVal value As Object, ByVal priority As System.Web.Caching.CacheItemPriority)
            SetHttpCacheValue(key, value, System.Web.Caching.Cache.NoSlidingExpiration, priority)
        End Sub
        ''' <summary>
        ''' Set an HttpCache item to a new value respectively remove it if the value is Nothing
        ''' </summary>
        ''' <param name="key">The key name for the cache item</param>
        ''' <param name="value">The new value</param>
        ''' <param name="slidingTimespan">A timespan with a sliding expiration date</param>
        ''' <param name="priority">The cache priority</param>
        ''' <exception cref="System.SystemException">
        ''' If HttpContext.Current is not available (if it is not a web application), there will be thrown a normal System.Exception
        ''' </exception>
        Friend Shared Sub SetHttpCacheValue(ByVal key As String, ByVal value As Object, ByVal slidingTimespan As TimeSpan, ByVal priority As System.Web.Caching.CacheItemPriority)
            If key = Nothing Then
                Throw New ArgumentNullException("key")
            End If
            If Not System.Web.HttpContext.Current Is Nothing Then
                If value Is Nothing Then
                    'Remove item
                    System.Web.HttpContext.Current.Cache.Remove(key)
                ElseIf System.Web.HttpContext.Current.Cache(key) Is Nothing Then
                    'Insert new item
                    System.Web.HttpContext.Current.Cache.Add(key, value, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingTimespan, priority, Nothing)
                Else
                    'Update existing item
                    System.Web.HttpContext.Current.Cache(key) = value
                End If
            Else
                Throw New Exception("HttpContext.Current not available - caching impossible")
            End If
        End Sub
#End Region

#Region "Redirection with 301"

        ''' <summary>
        ''' Redirect a browser with an HTTP 301 return code permanently to the new address
        ''' </summary>
        ''' <param name="context">The context of the current request</param>
        ''' <param name="url">The new destination address</param>
        ''' <remarks>A regular Response.Redirect does a HTTP 302 temporary redirection which has a decrease in search engine optimization efforts regarding the link priority. You'll get a more strong link priority if you redirect e. g. from your root URL to your start page with a permanent link.</remarks>
        Public Shared Sub RedirectPermanently(ByVal context As System.Web.HttpContext, ByVal url As String)

            context.Response.Clear()
            'context.Response.ClearHeaders() 'would strip SetCookie headers (e.g. required for ASP.NET cookies) away --> ClearHeaders is not allowed!
            context.Response.Status = "301 Moved Permanently"
            context.Response.AddHeader("Location", url)
            context.Response.End()

        End Sub

#End Region

#Region "Redirection with 302"

        ''' <summary>
        ''' Redirect a browser with an HTTP 302 return code temporary to the new address
        ''' </summary>
        ''' <param name="context">The context of the current request</param>
        ''' <param name="url">The new destination address</param>
        ''' <remarks>MS .NET Framework has got an error in ASP.NET 2.x at Response.Redirect which leads to invalidly encoded URLs in the body document of the redirect response. Search engines might use this wrong link to follow and index, but it may produce errors at the destination URL. Especially, the ampersand character gets encoded twice, in this case the destination page can't lookup the query parameters correctly any more.</remarks>
        Public Shared Sub RedirectTemporary(ByVal context As System.Web.HttpContext, ByVal url As String)

            context.Response.Clear()
            'context.Response.ClearHeaders() 'would strip SetCookie headers (e.g. required for ASP.NET cookies) away --> ClearHeaders is not allowed!
            context.Response.ContentType = "text/html"
            context.Response.CacheControl = "private"
            context.Response.Status = "302 Found"
            context.Response.AddHeader("Location", url)
            context.Response.Write("<html><head><title>Object moved</title></head><body>")
            context.Response.Write("<h2>Object moved to <a href=""" & System.Web.HttpUtility.HtmlEncode(url) & """>here</a>.</h2>")
            context.Response.Write("</body></html>")
            context.Response.End()

        End Sub

#End Region

#Region "Redirect of POST requests"
        ''' <summary>
        '''     Redirect to another url by using regular html forms to post data
        ''' </summary>
        ''' <param name="context">The context of the current request</param>
        ''' <param name="url">The new destination address</param>
        ''' <param name="postData">A collection of key/value pairs for the destination page</param>
        ''' <remarks>
        ''' <para>This method creates a client HTML form with hidden data fields and a JavaScript auto-post-back to send the data with POST method to a destination address.</para>
        ''' <para>Use this redirection method if the recieving page only understands form data and doesn't understand query parameters. Also use this method if you have to post a huge amount of data so that it extends the critical browser-limits of the request GET parameters.</para>
        ''' </remarks>
        Public Shared Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection)
            RedirectWithPostData(context, url, postData, "POST")
        End Sub
        ''' <summary>
        '''     Redirect to another url by using regular html forms to post data
        ''' </summary>
        ''' <param name="context">The context of the current request</param>
        ''' <param name="url">The new destination address</param>
        ''' <param name="postData">A collection of key/value pairs for the destination page</param>
        ''' <param name="httpMethod">POST (default) or GET</param>
        ''' <remarks>
        ''' <para>This method creates a client HTML form with hidden data fields and a JavaScript auto-post-back to send the data with POST method to a destination address.</para>
        ''' <para>Use this redirection method if the recieving page only understands form data and doesn't understand query parameters. Also use this method if you have to post a huge amount of data so that it extends the critical browser-limits of the request GET parameters.</para>
        ''' </remarks>
        Public Shared Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal httpMethod As String)
            RedirectWithPostData(context, url, postData, httpMethod, Nothing)
        End Sub

        ''' <summary>
        '''     Redirect to another url by using regular html forms to post data
        ''' </summary>
        ''' <param name="context">The context of the current request</param>
        ''' <param name="url">The new destination address</param>
        ''' <param name="postData">A collection of key/value pairs for the destination page</param>
        ''' <param name="httpMethod">POST (default) or GET</param>
        ''' <param name="additionalFormAttributes">Additional attributes which shall be embedded into the HTML FORM tag</param>
        ''' <remarks>
        ''' <para>This method creates a client HTML form with hidden data fields and a JavaScript auto-post-back to send the data with POST method to a destination address.</para>
        ''' <para>Use this redirection method if the recieving page only understands form data and doesn't understand query parameters. Also use this method if you have to post a huge amount of data so that it extends the critical browser-limits of the request GET parameters.</para>
        ''' </remarks>
        Public Shared Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal httpMethod As String, ByVal additionalFormAttributes As String)
            RedirectWithPostData(context, url, postData, httpMethod, additionalFormAttributes, "", "")
        End Sub

        ''' <summary>
        '''     Redirect to another url by using regular html forms to post data
        ''' </summary>
        ''' <param name="context">The context of the current request</param>
        ''' <param name="url">The new destination address</param>
        ''' <param name="postData">A collection of key/value pairs for the destination page</param>
        ''' <param name="httpMethod">POST (default) or GET</param>
        ''' <param name="additionalFormAttributes">Additional attributes which shall be embedded into the HTML FORM tag</param>
        ''' <param name="additionalHeaderTags">Some optional header tags like SCRIPT tags (hint: reference document.forms[0] in javascript code to access form objects)</param>
        ''' <param name="jsClientCodeOnSubmit">Some optional javascript statements to be executed immediately before submitting the form (hint: reference document.forms[0] to access form objects)</param>
        ''' <remarks>
        ''' <para>This method creates a client HTML form with hidden data fields and a JavaScript auto-post-back to send the data with POST method to a destination address.</para>
        ''' <para>Use this redirection method if the recieving page only understands form data and doesn't understand query parameters. Also use this method if you have to post a huge amount of data so that it extends the critical browser-limits of the request GET parameters.</para>
        ''' </remarks>
        Public Shared Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal httpMethod As String, ByVal additionalFormAttributes As String, additionalHeaderTags As String, jsClientCodeOnSubmit As String)

            context.Response.Clear()
            'context.Response.ClearHeaders() 'would strip SetCookie headers (e.g. required for ASP.NET cookies) away --> ClearHeaders is not allowed!
            context.Response.ContentType = "text/html"
            context.Response.CacheControl = "private"
            context.Response.Write("<html>")
            context.Response.Write("<head>")
            context.Response.Write("<title>Redirection</title>")
            If additionalHeaderTags <> Nothing Then
                context.Response.Write(additionalHeaderTags)
            End If
            context.Response.Write("</head>")
            context.Response.Write("<body>")
            context.Response.Write(RedirectWithPostDataCreateFormScript(url, postData, httpMethod, additionalFormAttributes, True, jsClientCodeOnSubmit))
            context.Response.Write("</body>")
            context.Response.Write("</html>")
            context.Response.End()

        End Sub




        ''' <summary>
        ''' Create an HTML form containing all relevant data and a client script for AutoPostBack
        ''' </summary>
        ''' <param name="url">The new destination address</param>
        ''' <param name="postData">A collection of key/value pairs for the destination page</param>
        ''' <param name="httpMethod">POST (default) or GET</param>
        ''' <param name="additionalFormAttributes">Additional attributes which shall be embedded into the HTML FORM tag</param>
        ''' <param name="autoClientPostBack">Shall a JavaScript autommatically commit the form or shall the user has to do it manually?</param>
        ''' <param name="jsClientCodeOnSubmit">Some optional javascript statements to be executed immediately before submitting the form (hint: reference document.forms[0] to access form objects)</param>
        ''' <returns>The complete HTML FORM tag which can be embedded into an HTML BODY</returns>
        ''' <remarks>
        ''' <para>This method creates a client HTML form with hidden data fields and a JavaScript auto-post-back to send the data with POST method to a destination address.</para>
        ''' <para>Use this redirection method if the recieving page only understands form data and doesn't understand query parameters. Also use this method if you have to post a huge amount of data so that it extends the critical browser-limits of the request GET parameters.</para>
        ''' </remarks>
        Friend Shared Function RedirectWithPostDataCreateFormScript(ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal httpMethod As String, ByVal additionalFormAttributes As String, ByVal autoClientPostBack As Boolean, jsClientCodeOnSubmit As String) As String
            Dim Result As New StringBuilder
            If httpMethod = Nothing Then httpMethod = "POST"
            Result.Append("<form action=""" & System.Web.HttpUtility.HtmlAttributeEncode(url) & """ name=""redirform"" method=""" & httpMethod & """ " & additionalFormAttributes & ">")

            If Not postData Is Nothing Then
                For Each keyName As String In postData
                    Dim value As String = postData(keyName)
                    Result.Append("<input type=""hidden"" id=""" & System.Web.HttpUtility.HtmlAttributeEncode(keyName) & """ name=""" & System.Web.HttpUtility.HtmlAttributeEncode(keyName) & """ value=""" & System.Web.HttpUtility.HtmlAttributeEncode(value) & """>")
                Next
            End If

            If Not autoClientPostBack Then
                Result.Append("<button type=""submit"" name=""submit_button"">Submit</button>")
            End If
            Result.Append("<noscript>Warning: There is no javascript enabled in your browser. But this is required here to automatically redirect to another address.</noscript>")
            Result.Append("</form>")



            If autoClientPostBack Then
                Result.Append("<script language=""javascript"">")
                Result.Append("<!--" & vbNewLine)
                If jsClientCodeOnSubmit <> Nothing Then
                    Result.Append("    " & jsClientCodeOnSubmit & vbNewLine)
                End If
                Result.Append("    document.forms[0].submit();" & vbNewLine)
                Result.Append("-->" & vbNewLine)
                Result.Append("</script>")
            End If

            Return Result.ToString()
        End Function

#End Region

#Region "ReadString/ByteDataFromUri"

        Public Shared Function ReadByteDataFromUri(ByVal uri As String) As Byte()
            Dim client As New System.Net.WebClient
            Return client.DownloadData(uri)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal uri As String, ByVal encodingName As String) As String
            Return ReadStringDataFromUri(CType(Nothing, System.Net.WebClient), uri, encodingName)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal uri As String, ByVal encodingName As String, ByVal ignoreSslValidationExceptions As Boolean) As String
            Return ReadStringDataFromUri(CType(Nothing, System.Net.WebClient), uri, encodingName, False)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal client As System.Net.WebClient, ByVal uri As String, ByVal encodingName As String) As String
            Return ReadStringDataFromUri(client, uri, encodingName, False)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal client As System.Net.WebClient, ByVal uri As String, ByVal encodingName As String, ByVal ignoreSslValidationExceptions As Boolean) As String
            Return ReadStringDataFromUri(client, uri, encodingName, False, CType(Nothing, String))
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal client As System.Net.WebClient, ByVal uri As String, ByVal encodingName As String, ByVal ignoreSslValidationExceptions As Boolean, ByVal postData As String) As String
            If client Is Nothing Then client = New System.Net.WebClient
            'https://compumaster.dyndns.biz/.....asmx without trusted certificate
#If Not NET_1_1 Then
            Dim CurrentValidationCallback As System.Net.Security.RemoteCertificateValidationCallback = System.Net.ServicePointManager.ServerCertificateValidationCallback
            Try
                If ignoreSslValidationExceptions Then System.Net.ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf OnValidationCallback)
#End If
                If encodingName <> Nothing Then
                    Dim bytes As Byte()
                    If postData Is Nothing Then
                        bytes = client.DownloadData(uri)
                    Else
                        bytes = client.UploadData(uri, System.Text.Encoding.GetEncoding(encodingName).GetBytes(postData))
                    End If
                    Return System.Text.Encoding.GetEncoding(encodingName).GetString(bytes)
                Else
#If NET_1_1 Then
                Dim encoding As System.Text.Encoding
                Try
                    Dim encName As String = client.ResponseHeaders("Content-Type")
                    If encName <> "" And encName.IndexOf("charset=") > -1 Then
                        encName = encName.Substring(encName.IndexOf("charset=") + "charset=".Length)
                        encoding = System.Text.Encoding.GetEncoding(encName)
                    Else
                        encoding = System.Text.Encoding.Default
                    End If
                Catch
                    encoding = System.Text.Encoding.Default
                End Try
                Dim bytes As Byte()
                If postData Is Nothing Then
                    bytes = client.DownloadData(uri)
                Else
                    bytes = client.UploadData(uri, encoding.GetBytes(postData))
                End If
                Return encoding.GetString(bytes)
#Else
                    If postData Is Nothing Then
                        Return client.DownloadString(uri)
                    Else
                        Return client.UploadString(uri, postData)
                    End If
#End If
                End If
#If Not NET_1_1 Then
            Finally
                System.Net.ServicePointManager.ServerCertificateValidationCallback = CurrentValidationCallback
            End Try
#End If
        End Function

#If Not NET_1_1 Then
        ''' <summary>
        ''' Suppress all SSL certification requirements - just use the webservice SSL URL
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="cert"></param>
        ''' <param name="chain"></param>
        ''' <param name="errors"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function OnValidationCallback(ByVal sender As Object, ByVal cert As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal errors As System.Net.Security.SslPolicyErrors) As Boolean
            Return True
        End Function
#End If

#End Region

#Region "Network host information"
        ''' <summary>
        '''     Get the first value of the IP address list or the workstation name if there are no IP addresses
        ''' </summary>
        Public Shared Function GetWorkstationID() As String
#If VS2015OrHigher = True Then
#Disable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If

            Dim host As System.Net.IPHostEntry = System.Net.Dns.Resolve(System.Net.Dns.GetHostName)

            ' Loop on the AddressList
            Dim curAdd As System.Net.IPAddress
            For Each curAdd In host.AddressList

                ' Display the server IP address in the standard format. In 
                ' IPv4 the format will be dotted-quad notation, in IPv6 it will be
                ' in in colon-hexadecimal notation.
                Return curAdd.ToString()

            Next

            'If not already returned with a found IP, return with the host name
            Return System.Net.Dns.GetHostName

#If VS2015OrHigher = True Then
#Enable Warning BC40000 'obsolete warnings
#End If
        End Function
        ''' <summary>
        '''     Is the remote client connecting from a private network?
        ''' </summary>
        ''' <returns>True for private networks, false for localhost and public networks</returns>
        Public Shared Function IsRemoteClientFromLanOrPrivateNetwork() As Boolean

            If System.Web.HttpContext.Current Is Nothing Then
                Throw New Exception("Not running in a web application")
            End If

            Return IsHostFromLanOrPrivateNetwork(System.Web.HttpContext.Current.Request.UserHostAddress)

        End Function
        ''' <summary>
        '''     Is the real remote client (possibly from behind a reverse proxy server) connecting from a private network?
        ''' </summary>
        ''' <returns>True for private networks, false for localhost and public networks</returns>
        Public Shared Function IsRealRemoteClientIPFromLanOrPrivateNetwork() As Boolean

            If System.Web.HttpContext.Current Is Nothing Then
                Throw New Exception("Not running in a web application")
            End If

            Return IsHostFromLanOrPrivateNetwork(LookupRealRemoteClientIPOfHttpContext())

        End Function
        ''' <summary>
        '''     Is the IPv4 address a host of a specified address range?
        ''' </summary>
        ''' <param name="IPv4Address">An IPv4 host address</param>
        ''' <param name="AddressRange">An array of strings defining address ranges, e. g. 192.168.*</param>
        ''' <remarks>
        '''     This method compares the string content and not the byte content, that's why you should not forget the leading dot in front of the star in your IP address range.
        ''' </remarks>
        ''' <example>
        '''     <para>These address ranges would be fine:</para>
        '''     <list>
        '''         <item>10.*</item>
        '''         <item>192.168.*</item>
        '''     </list>
        '''     <para>Those address ranges could get misinterpreted:</para>
        '''     <list>
        '''         <item>10* <em>(would also accept 101.*, 102.*, etc. and invalid IP addresses like 10:ZZ:*)</em></item>
        '''         <item>192.168.*.* <em>(all letters after the first star will be ignored)</em></item>
        '''     </list>
        ''' </example>
        Public Shared Function IsHostOfAddressRange(ByVal IPv4Address As String, ByVal AddressRange As String()) As Boolean

            If IPv4Address = "" Then
                Return False
            ElseIf InStr(IPv4Address, "*") = 1 Then
                Return False 'Invalid filter, return false even if it would be true, regulary
            ElseIf AddressRange Is Nothing OrElse AddressRange.Length = 0 Then
                Return False
            Else
                'First look says: it might be possible; go on
                For Each RangeElement As String In AddressRange
                    Dim RangeStartPos As Integer = InStr(RangeElement, "*") - 1
                    Dim RangeStart As String
                    If RangeStartPos > 0 Then
                        'Found a "*", do a like matching
                        RangeStart = Mid(RangeElement, 1, RangeStartPos)
                        If IPv4Address.ToUpper(System.Globalization.CultureInfo.InvariantCulture).StartsWith(RangeStart.ToUpper(System.Globalization.CultureInfo.InvariantCulture)) Then
                            Return True
                        End If
                    Else
                        'No jokers, exact match required
                        RangeStart = RangeElement
                        If IPv4Address.ToUpper(System.Globalization.CultureInfo.InvariantCulture) = (RangeStart.ToUpper(System.Globalization.CultureInfo.InvariantCulture)) Then
                            Return True
                        End If
                    End If
                Next
            End If

        End Function
        ''' <summary>
        '''     Is the host address a private network node (defined by RFC-1597)?
        ''' </summary>
        ''' <param name="IPAddress">The IP address to validate</param>
        ''' <returns>True for private or LAN networks, otherwise false</returns>
        Public Shared Function IsHostFromLanOrPrivateNetwork(ByVal IPAddress As String) As Boolean

            Dim IP As System.Net.IPAddress
            IP = System.Net.IPAddress.Parse(IPAddress)

            Console.WriteLine(IP.AddressFamily.ToString)

            If IP.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                'RFC 1918
                'private ranges are
                '10.x.x.x
                '172.16.x.x - 172.31.x.x
                '192.168.x.x
                If Left(IPAddress, 3) = "10." OrElse Left(IPAddress, 8) = "192.168." Then
                    Return True
                ElseIf Left(IPAddress, 4) = "172." Then
                    Select Case Mid(IPAddress, 5, 3)
                        Case "16.", "17.", "18.", "19.", "20.", "21.", "22.", "23.", "24.", "25.", "26.", "27.", "28.", "29.", "30.", "31."
                            Return True
                        Case Else
                            Return False
                    End Select
                Else
                    Return False
                End If
            ElseIf IP.AddressFamily = System.Net.Sockets.AddressFamily.InterNetworkV6 Then
                'http://www.zytrax.com/tech/protocols/ipv6.html#types
                'Scope is local LAN only; can't be routed outside of local LAN
                'private ranges (link local) are beginning with 
                '- fe8x (most commonly used)
                '- fe9x
                '- feax
                '- febx
                'Scope is local private network; can't be routed outside over Internet
                'private ranges (site local) are beginning with 
                '- fecx (most commonly used)
                '- fedx
                '- feex
                '- fefx
                Dim IPBytes As Byte() = IP.GetAddressBytes
                If IPBytes(0) = 254 Then 'FE 
                    If IPBytes(1) >= 192 Then '(C0-FF) 
                        'private network
                        Return True
                    ElseIf IPBytes(1) >= 128 Then '(80-BF) 
                        'LAN
                        Return True
                    Else
                        'Global unicast or others
                        Return False '(00-7F)
                    End If
                Else
                    Return False
                End If
            Else
                'network type not detectable            
                Return False
            End If

        End Function
        ''' <summary>
        '''     Is the host a localhost?
        ''' </summary>
        ''' <param name="IPAddress">The IP address to validate</param>
        Public Shared Function IsLoopBackDevice(ByVal IPAddress As String) As Boolean
            Return System.Net.IPAddress.IsLoopback(System.Net.IPAddress.Parse(IPAddress))
        End Function

#End Region

#Region "Nz"

        ''' <summary>
        ''' Try to access a column value if it exists, otherwise DBNull.Value
        ''' </summary>
        ''' <param name="row"></param>
        ''' <param name="columnName"></param>
        Friend Shared Function CellValueIfColumnExists(row As System.Data.DataRow, columnName As String) As Object
            If row.Table.Columns.Contains(columnName) Then
                Return row(columnName)
            Else
                Return DBNull.Value
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function IfNull(ByVal CheckValueIfDBNull As Object, Optional ByVal ReplaceWithThis As Object = Nothing) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns null (Nothing in VisualBasic) in that case
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return Nothing
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Object) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Integer) As Integer
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Integer)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Long) As Long
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Long)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Boolean) As Boolean
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Boolean)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As DateTime) As DateTime
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, DateTime)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As String) As String
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, String)
            End If
        End Function

        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Nullable(Of Integer)) As Nullable(Of Integer)
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Nullable(Of Integer))
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Nullable(Of Long)) As Nullable(Of Long)
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Nullable(Of Long))
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Nullable(Of Boolean)) As Nullable(Of Boolean)
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Nullable(Of Boolean))
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Nullable(Of DateTime)) As Nullable(Of DateTime)
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Nullable(Of DateTime))
            End If
        End Function

        ''' <summary>
        '''     Checks for DBNull and returns the null (Nothing in VisualBasic) alternatively
        ''' </summary>
        ''' <param name="checkValueIfDBNull">The value to be checked</param>
        ''' <returns>A value which is not DBNull, otherwise null (Nothing in VisualBasic)</returns>
        <DebuggerHidden()> Public Shared Function Nz(Of T)(ByVal checkValueIfDBNull As Object) As T
            If IsDBNull(checkValueIfDBNull) Then
                Return Nothing
            ElseIf checkValueIfDBNull Is Nothing Then
                Return CType(Nothing, T)
            ElseIf Nullable.GetUnderlyingType(GetType(T)) IsNot Nothing AndAlso checkValueIfDBNull.GetType.IsValueType AndAlso Nullable.GetUnderlyingType(GetType(T)) IsNot checkValueIfDBNull.GetType Then
                Dim UnderlyingType As Type = Nullable.GetUnderlyingType(GetType(T))
                Dim Result As T = CType(Activator.CreateInstance(GetType(T), checkValueIfDBNull), T)
                Return Result
            Else
                Return CType(checkValueIfDBNull, T)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="checkValueIfDBNull">The value to be checked</param>
        ''' <param name="replaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <DebuggerHidden()> Public Shared Function Nz(Of T)(ByVal checkValueIfDBNull As Object, ByVal replaceWithThis As T) As T
            If IsDBNull(checkValueIfDBNull) Then
                Return (replaceWithThis)
            Else
                Return CType(checkValueIfDBNull, T)
            End If
        End Function

#End Region

#Region "InlineIf"
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As String, ByVal falseValue As String) As String
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Integer, ByVal falseValue As Integer) As Integer
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Date, ByVal falseValue As Date) As Date
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Long, ByVal falseValue As Long) As Long
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Double, ByVal falseValue As Double) As Double
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Boolean, ByVal falseValue As Boolean) As Boolean
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
#End Region

#Region "Type conversions"
        ''' <summary>
        ''' Convert a boolean value into the corresponding value of a TriState
        ''' </summary>
        ''' <param name="value">A boolean value</param>
        ''' <returns>A value of type TriState with either TriState.True or TriState.False</returns>
        Friend Shared Function BooleanToWMTriplestate(ByVal value As Boolean) As WMSystem.TripleState
            If value Then
                Return WMSystem.TripleState.True
            Else
                Return WMSystem.TripleState.False
            End If
        End Function
        ''' <summary>
        ''' Convert a boolean value into the corresponding value of a TriState
        ''' </summary>
        ''' <param name="value">A boolean value</param>
        ''' <returns>A value of type TriState with either TriState.True or TriState.False</returns>
        Friend Shared Function BooleanToTristate(ByVal value As Boolean) As TriState
            If value Then
                Return TriState.True
            Else
                Return TriState.False
            End If
        End Function
        ''' <summary>
        ''' Convert a TriState value into a corresponding boolean value
        ''' </summary>
        ''' <param name="value">A value of type TriState with either TriState.True or TriState.False</param>
        ''' <returns>A boolean value with either True or False</returns>
        ''' <remarks>
        '''     If the input value is TriState.Default, there will be thrown an ArgumentException
        ''' </remarks>
        Friend Shared Function TristateToBoolean(ByVal value As TriState) As Boolean
            If value = TriState.True Then
                Return True
            ElseIf value = TriState.False Then
                Return False
            Else
                Throw New ArgumentException("value must be TriState.True or TriState.False", "value")
            End If
        End Function
        ''' <summary>
        ''' Convert a TriState value into a corresponding boolean value
        ''' </summary>
        ''' <param name="value">A value of type TriState with either TriState.True or TriState.False</param>
        ''' <returns>A boolean value with either True or False</returns>
        ''' <remarks>
        '''     If the input value is TriState.Default, there will be thrown an ArgumentException
        ''' </remarks>
        Friend Shared Function WMTriplestateToBoolean(ByVal value As WMSystem.TripleState) As Boolean
            If value = WMSystem.TripleState.True Then
                Return True
            ElseIf value = WMSystem.TripleState.False Then
                Return False
            Else
                Throw New ArgumentException("value must be TripleState.True or TripleState.False", "value")
            End If
        End Function
        ''' <summary>
        ''' Convert a WMSystem.TripleState value into a corresponding Tristate value
        ''' </summary>
        ''' <param name="value">A value of type WMSystem.TripleState</param>
        ''' <returns>A Tristate value</returns>
        Friend Shared Function WMTriplestateToTristate(ByVal value As WMSystem.TripleState) As TriState
            If value = WMSystem.TripleState.True Then
                Return TriState.True
            ElseIf value = WMSystem.TripleState.False Then
                Return TriState.False
            Else
                Return TriState.UseDefault
            End If
        End Function
        ''' <summary>
        ''' Convert a Tristate value into a corresponding WMSystem.TripleState value
        ''' </summary>
        ''' <param name="value">A Tristate value</param>
        ''' <returns>A value of type WMSystem.TripleState</returns>
        Friend Shared Function TristateToWMTriplestate(ByVal value As TriState) As WMSystem.TripleState
            If value = TriState.True Then
                Return WMSystem.TripleState.True
            ElseIf value = TriState.False Then
                Return WMSystem.TripleState.False
            Else
                Return WMSystem.TripleState.Undefined
            End If
        End Function
        ''' <summary>
        ''' Check the expression and return a strongly typed value
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="expression">The expression for the check</param>
        ''' <param name="trueValue">The return value if the expression is true</param>
        ''' <param name="falseValue">The return value if the expression is false</param>
        ''' <returns>A strongly typed return value</returns>
        Public Shared Function IIf(Of T)(expression As Boolean, trueValue As T, falseValue As T) As T
            If expression Then Return trueValue Else Return falseValue
        End Function
        ''' <summary>
        '''     Return the string which is not nothing or else String.Empty
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        Public Shared Function StringNotEmptyOrNothing(ByVal value As String) As String
            If value = Nothing Then
                Return Nothing
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not nothing or else String.Empty
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        Public Shared Function StringNotNothingOrEmpty(ByVal value As String) As String
            If value Is Nothing Then
                Return String.Empty
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not nothing or else the alternative value
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <param name="alternativeValue">An alternative value if the first value is nothing</param>
        Public Shared Function StringNotNothingOrAlternativeValue(ByVal value As String, ByVal alternativeValue As String) As String
            If value Is Nothing Then
                Return alternativeValue
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not empty or else the alternative value
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <param name="alternativeValue">An alternative value if the first value is empty</param>
        Public Shared Function StringNotEmptyOrAlternativeValue(ByVal value As String, ByVal alternativeValue As String) As String
            If value = Nothing Then
                Return alternativeValue
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not empty or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        Public Shared Function StringNotEmptyOrDBNull(ByVal value As String) As Object
            If value = Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the datetime value which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The datetime value to be validated</param>
        Public Shared Function DateTimeNotNothingOrDBNull(ByVal value As DateTime) As Object
            If value = Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the object which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The object to be validated</param>
        Public Shared Function ObjectNotNothingOrEmptyString(ByVal value As Object) As Object
            If value Is Nothing Then
                Return String.Empty
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the object which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The object to be validated</param>
        Public Shared Function ObjectNotNothingOrDBNull(ByVal value As Object) As Object
            If value Is Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the object which is not an empty string or otherwise return Nothing
        ''' </summary>
        ''' <param name="value">The object to be validated</param>
        ''' <returns>A string with length > 0 (the value) or nothing</returns>
        Public Shared Function ObjectNotEmptyStringOrNothing(ByVal value As Object) As Object
            If value Is Nothing Then
                Return Nothing
            ElseIf value.GetType Is GetType(String) AndAlso CType(value, String) = "" Then
                Return Nothing
            Else
                Return value
            End If
        End Function

        ''' <summary>
        '''     Return the value if there is a value or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The nullable type value to be validated</param>
        Public Shared Function NullableTypeWithItsValueOrDBNull(Of T As Structure)(ByVal value As Nullable(Of T)) As Object
            If value.HasValue = False Then
                Return DBNull.Value
            Else
                Return value.Value
            End If
        End Function

        ''' <summary>
        '''     Return the array which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="values">The array to be validated</param>
        Public Shared Function ArrayNotNothingOrDBNull(ByVal values As Array) As Object
            If values Is Nothing Then
                Return DBNull.Value
            Else
                Return values
            End If
        End Function

        ''' <summary>
        '''     Return the array with at least 1 element or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="values">The array to be validated</param>
        Public Shared Function ArrayNotEmptyOrDBNull(ByVal values As Array) As Object
            If values Is Nothing OrElse values.Length = 0 Then
                Return DBNull.Value
            Else
                Return values
            End If
        End Function
        ''' <summary>
        '''     Return the array with at least 1 element or otherwise return Nothing
        ''' </summary>
        ''' <param name="values">The array to be validated</param>
        Public Shared Function ArrayNotEmptyOrNothing(Of T)(ByVal values As T()) As T()
            If values Is Nothing OrElse values.Length = 0 Then
                Return Nothing
            Else
                Return values
            End If
        End Function
        ''' <summary>
        '''     Return the array with at least 0 elements in case it's Nothing
        ''' </summary>
        ''' <param name="values">The array to be validated</param>
        Public Shared Function ArrayNotNothingOrEmpty(Of T)(ByVal values As T()) As T()
            If values Is Nothing Then
                Return New T() {}
            Else
                Return values
            End If
        End Function

        ''' <summary>
        '''     Return the string which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        Public Shared Function StringNotNothingOrDBNull(ByVal value As String) As Object
            If value Is Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted long value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        Public Shared Function TryCLng(ByVal Expression As Object) As Long
            Return TryCLng(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted long value or the alternative value if the conversion was unsuccessfull</returns>
        Public Shared Function TryCLng(ByVal Expression As Object, ByVal AlternativeValue As Long) As Long
            Try
                Return CLng(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a integer value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted integer value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        Public Shared Function TryCInt(ByVal Expression As Object) As Integer
            Return TryCInt(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a integer value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted integer value or the alternative value if the conversion was unsuccessfull</returns>
        Public Shared Function TryCInt(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Integer
            Try
                Return CInt(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a double value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted double value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        Public Shared Function TryCDbl(ByVal Expression As Object) As Double
            Return TryCDbl(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a double value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted double value or the alternative value if the conversion was unsuccessfull</returns>
        Public Shared Function TryCDbl(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Double
            Try
                Return CDbl(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a decimal value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted decimal value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        Public Shared Function TryCDec(ByVal Expression As Object) As Decimal
            Return TryCDec(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a decimal value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted decimal value or the alternative value if the conversion was unsuccessfull</returns>
        Public Shared Function TryCDec(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Decimal
            Try
                Return CDec(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function

#End Region

#Region "Hashing"

        Friend Shared Function ComputeHash(ByVal value As String) As String
            'create Encrypted value for parameter 'value' and return it as string
            Dim result As Byte()
            Dim hashprovider As New System.Security.Cryptography.MD5CryptoServiceProvider

            hashprovider.ComputeHash(ConvertStringToBytes(value))
            result = hashprovider.Hash
            hashprovider.Clear()

            'Return the new string with file system compatible naming standard (windows, linux, mac)
            Return ConvertBytesToBase64String(result)

        End Function
#End Region

#Region "String iterations"
        Friend Shared Function StringArrayContainsValue(ByVal stringArray As String(), ByVal searchFor As String, ByVal ignoreCase As Boolean) As String
            If stringArray Is Nothing Then Throw New ArgumentNullException("stringArray")
            For MyCounter As Integer = 0 To stringArray.Length - 1
                If ignoreCase Then
                    If StringNotNothingOrEmpty(stringArray(MyCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture)) = StringNotNothingOrEmpty(searchFor.ToLower(System.Globalization.CultureInfo.InvariantCulture)) Then Return stringArray(MyCounter)
                Else
                    If stringArray(MyCounter) = searchFor Then Return stringArray(MyCounter)
                End If
            Next
            Return Nothing
        End Function
#End Region

#Region "String encoding Base64"
        ''' <summary>
        ''' Convert a normal string into base64 string
        ''' </summary>
        ''' <param name="text">A normal string</param>
        ''' <returns>The base64 string </returns>
        ''' <remarks>
        ''' The byte encoding is based on UTF-8 encoding
        ''' </remarks>
        Public Shared Function ConvertStringToBase64String(ByVal text As String) As String
            If text Is Nothing Then
                Return Nothing
            ElseIf text = Nothing Then
                Return ""
            Else
                Return ConvertBytesToBase64String(ConvertStringToBytes(text))
            End If
        End Function
        ''' <summary>
        ''' Convert a normal string into base64 string
        ''' </summary>
        ''' <param name="text">A normal string</param>
        ''' <returns>The base64 string </returns>
        ''' <remarks>
        ''' The byte encoding is based on UTF-8 encoding
        ''' </remarks>
        Public Shared Function ConvertStringToBase64String(ByVal text As String, insertLineBreaks As Boolean) As String
            Return ConvertStringToBase64String(text, 70)
        End Function
        ''' <summary>
        ''' Convert a normal string into base64 string
        ''' </summary>
        ''' <param name="text">A normal string</param>
        ''' <returns>The base64 string </returns>
        ''' <remarks>
        ''' The byte encoding is based on UTF-8 encoding
        ''' </remarks>
        Public Shared Function ConvertStringToBase64String(ByVal text As String, insertLineBreaksAfterNumberOfChars As Integer) As String
            If text Is Nothing Then
                Return Nothing
            Else
                Dim Result As New System.Text.StringBuilder(ConvertStringToBase64String(text))
                Dim MyCounter As Integer = insertLineBreaksAfterNumberOfChars
                Do While MyCounter <= Result.Length - 1
                    Result.Insert(MyCounter, vbNewLine)
                    MyCounter += insertLineBreaksAfterNumberOfChars + 2 '+2 because of vbCrLf are 2 chars!
                Loop
                Return Result.ToString
            End If
        End Function
        ''' <summary>
        ''' Convert a base64 string into a normal string
        ''' </summary>
        ''' <param name="base64String">A base64 string</param>
        ''' <returns>A normal string</returns>
        ''' <remarks>
        ''' The byte encoding is based on UTF-8 encoding
        ''' </remarks>
        Public Shared Function ConvertBase64StringToString(ByVal base64String As String) As String
            If base64String Is Nothing Then
                Return Nothing
            Else
                Return ConvertBytesToString(ConvertBase64StringToBytes(base64String))
            End If
        End Function
        ''' <summary>
        ''' Convert a base64 string into a normal string
        ''' </summary>
        ''' <param name="base64String">A base64 string</param>
        ''' <returns>A normal string</returns>
        ''' <remarks>
        ''' The byte encoding is based on UTF-8 encoding
        ''' </remarks>
        Public Shared Function ConvertBase64StringToString(ByVal base64String As String, autoRemoveLineBreaks As Boolean) As String
            If base64String Is Nothing Then
                Return Nothing
            ElseIf base64String = Nothing Then
                Return ""
            ElseIf autoRemoveLineBreaks Then
                Return ConvertBytesToString(ConvertBase64StringToBytes(Replace(Replace(Replace(Replace(base64String, " ", ""), ChrW(9), ""), ControlChars.Cr, ""), ControlChars.Lf, "")))
            Else
                Return ConvertBytesToString(ConvertBase64StringToBytes(base64String))
            End If
        End Function
        ''' <summary>
        ''' Convert a base64 string into a normal string
        ''' </summary>
        ''' <param name="base64String">A base64 string</param>
        ''' <returns>A normal string</returns>
        ''' <remarks>
        ''' </remarks>
        Private Shared Function ConvertBase64StringToBytes(ByVal base64String As String) As Byte()
            If base64String Is Nothing Then
                Return Nothing
            ElseIf base64String = Nothing Then
                Return New Byte() {}
            Else
                Return System.Convert.FromBase64String(base64String)
            End If
        End Function
        Private Shared Function ConvertBytesToString(ByVal data As Byte()) As String
            If data Is Nothing Then
                Return Nothing
            ElseIf data.Length = 0 Then
                Return ""
            Else
                ' Declare a UTF8Encoding object so we may use the GetByte 
                ' method to transform the plainText into a Byte array. 
                Dim utf8encoder As New System.Text.UTF8Encoding
                Return utf8encoder.GetString(data, 0, data.Length)
            End If
        End Function
        Private Shared Function ConvertBytesToBase64String(ByVal byteData As Byte()) As String
            If byteData Is Nothing Then
                Return Nothing
            ElseIf byteData.Length = 0 Then
                Return ""
            Else
                Return System.Convert.ToBase64String(byteData)
            End If
        End Function
        Private Shared Function ConvertStringToBytes(ByVal text As String) As Byte()
            If text Is Nothing Then
                Return Nothing
            ElseIf text = Nothing Then
                Return New Byte() {}
            Else
                ' Declare a UTF8Encoding object so we may use the GetByte 
                ' method to transform the plainText into a Byte array. 
                Dim utf8encoder As New System.Text.UTF8Encoding
                Return utf8encoder.GetBytes(text)
            End If
        End Function
#End Region

#Region "String manipulation and HTML conversions"
        ''' <summary>
        '''     Combine a unix path with another one
        ''' </summary>
        ''' <param name="path1">A first path</param>
        ''' <param name="path2">A second path which shall be appended to the first path</param>
        ''' <returns>The combined path</returns>
        ''' <remarks>
        ''' If path2 starts with &quot;/&quot;, it is considered as root folder and will be the only return value.
        ''' </remarks>
        Friend Shared Function CombineUnixPaths(ByVal path1 As String, ByVal path2 As String) As String
            If path1 = Nothing OrElse (path2 <> Nothing AndAlso path2.StartsWith("/")) Then
                Return path2
            ElseIf path2 = Nothing Then
                Return path1
            Else
                'path2.StartsWith("/") can never happen since it has already been evaluated above
                If path1.EndsWith("/") Then
                    Return path1 & path2
                Else
                    Return path1 & "/" & path2
                End If
            End If
        End Function
        ''' <summary>
        '''     Get the complete query string of the current request in a form usable for recreating this query string for a following request
        ''' </summary>
        ''' <param name="removeParameters">Remove all values with this name form the query string</param>
        ''' <returns>A new string with all query string information without the starting questionmark character</returns>
        Public Shared Function QueryStringWithoutSpecifiedParameters(ByVal removeParameters As String()) As String
            Return NameValueCollectionWithoutSpecifiedKeys(System.Web.HttpContext.Current.Request.QueryString, removeParameters)
        End Function
        ''' <summary>
        '''     Get the complete string of a collection in a form usable for recreating a query string for a following request
        ''' </summary>
        ''' <param name="collection">A NameValueCollection, e. g. Request.QueryString</param>
        ''' <param name="removeKeys">Names of keys which shall not be in the output</param>
        ''' <returns>A string of the collection data which can be appended to any URL (with url encoding)</returns>
        Public Shared Function NameValueCollectionWithoutSpecifiedKeys(ByVal collection As System.Collections.Specialized.NameValueCollection, ByVal removeKeys As String()) As String
            Dim RedirectionParams As String = ""
            For Each ParamItem As String In collection
                Dim RemoveThisParameter As Boolean = False
                If ParamItem = "" Then
                    RemoveThisParameter = True
                ElseIf Not removeKeys Is Nothing Then
                    Dim MyParamItem As String = ParamItem.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    For Each My2RemoveParameter As String In removeKeys
                        If MyParamItem = My2RemoveParameter.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                            RemoveThisParameter = True
                        End If
                    Next
                End If
                If RemoveThisParameter = False Then
                    'do not collect empty items (ex. the item between "?" and "&" in "index.aspx?&Lang=2")
                    RedirectionParams = RedirectionParams & "&" & System.Web.HttpUtility.UrlEncode(ParamItem) & "=" & System.Web.HttpUtility.UrlEncode(collection(ParamItem))
                End If
            Next
            Return Mid(RedirectionParams, 2)
        End Function
        ''' <summary>
        '''     Split a string by a separator if there is not a special leading character
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="separator"></param>
        ''' <param name="exceptLeadingCharacter"></param>
        Public Shared Function SplitString(ByVal text As String, ByVal separator As Char, ByVal exceptLeadingCharacter As Char) As String()
            If text = Nothing Then
                Return New String() {}
            End If
            Dim Result As New ArrayList
            'Go through every char of the string
            For MyCounter As Integer = 0 To text.Length - 1
                Dim SplitHere As Boolean
                Dim StartPosition As Integer
                'Find split points
                If text.Chars(MyCounter) = separator Then
                    If MyCounter = 0 Then
                        SplitHere = True
                    ElseIf text.Chars(MyCounter - 1) <> exceptLeadingCharacter Then
                        SplitHere = True
                    End If
                End If
                'Add partial string
                If SplitHere OrElse MyCounter = text.Length - 1 Then
                    Result.Add(text.Substring(StartPosition, CType(IIf(SplitHere = False, 1, 0), Integer) + MyCounter - StartPosition)) 'If Split=False then this if-block was caused by the end of the text; in this case we have to simulate to be after the last character position to ensure correct extraction of the last text element
                    SplitHere = False 'Reset status
                    StartPosition = MyCounter + 1 'Next string starts after the current char
                End If
            Next
            Return CType(Result.ToArray(GetType(String)), String())
        End Function

        ''' <summary>
        ''' Split a string into an array of integers
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="separator"></param>
        ''' <remarks></remarks>
        Public Shared Function SplitStringToInteger(ByVal text As String, ByVal separator As Char) As Integer()
            If text Is Nothing Then
                Return New Integer() {}
            End If
            Dim Result As New ArrayList
            Dim SplittedText As String() = text.Split(separator)
            For MyCounter As Integer = 0 To SplittedText.Length - 1
                Result.Add(Integer.Parse(SplittedText(MyCounter)))
            Next
            Return CType(Result.ToArray(GetType(Integer)), Integer())
        End Function
        ''' <summary>
        '''     Converts all line breaks into HTML line breaks (&quot;&lt;br&gt;&quot;)
        ''' </summary>
        ''' <param name="text">A text string which might contain line breaks of any platform type</param>
        ''' <returns>The text string with encoded line breaks to &quot;&lt;br&gt;&quot;</returns>
        ''' <remarks>
        '''     Supported line breaks are linebreaks of Windows, MacOS as well as Linux/Unix.
        ''' </remarks>
        Public Shared Function HTMLEncodeLineBreaks(ByVal text As String) As String
            If text = Nothing Then
                Return text
            Else
                Return text.Replace(ControlChars.CrLf, "<br />").Replace(ControlChars.Cr, "<br />").Replace(ControlChars.Lf, "<br />")
            End If
        End Function
        ''' <summary>
        '''     Search for a string in another string and return the number of matches
        ''' </summary>
        ''' <param name="source">The string where to search in</param>
        ''' <param name="searchFor">The searched string (binary comparison)</param>
        Public Shared Function CountOfOccurances(ByVal source As String, ByVal searchFor As String) As Integer
            Return CountOfOccurances(source, searchFor, CompareMethod.Binary)
        End Function
        ''' <summary>
        '''     Search for a string in another string and return the number of matches
        ''' </summary>
        ''' <param name="source">The string where to search in</param>
        ''' <param name="searchFor">The searched string</param>
        ''' <param name="compareMethod">Binary or text search</param>
        Public Shared Function CountOfOccurances(ByVal source As String, ByVal searchFor As String, ByVal compareMethod As Microsoft.VisualBasic.CompareMethod) As Integer

            If searchFor = "" Then
                Throw New ArgumentNullException("searchFor")
            End If

            Dim Result As Integer

            'Initial values
            Dim Position As Integer = 0
            Dim NextMatch As Integer = InStr(Position + 1, source, searchFor, compareMethod)

            'and now loop through the source string
            While NextMatch > 0
                Result += 1
                If NextMatch <= Position Then
                    Throw New Exception("NextMatch=" & NextMatch & ", OldPosition=" & Position & ", searchFor=""" & searchFor & """, source=""" & source & """")
                End If
                Position = NextMatch

                'Search for next value
                NextMatch = InStr(Position + 1, source, searchFor, compareMethod)
            End While

            Return Result

        End Function
        ''' <summary>
        '''     Replaces placeholders in a string by defined values
        ''' </summary>
        ''' <param name="message">The string with the placeholders</param>
        ''' <param name="values">One or more values which should be used for replacement</param>
        ''' <returns>The new resulting string</returns>
        ''' <remarks>
        '''     <para>Supported special character combinations are <code>\t</code>, <code>\r</code>, <code>\n</code>, <code>\\</code>, <code>\[</code></para>
        '''     <para>Supported placeholders are <code>[*]</code>, <code>[n:1..9]</code></para>
        ''' </remarks>
        Public Shared Function sprintf(ByVal message As String, ByVal ParamArray values() As Object) As String
            Const errpfNoClosingBracket As Integer = vbObjectError + 1
            Const errpfMissingValue As Integer = vbObjectError + 2
            '*** Special chars ***
            '   \t = TAB
            '   \r = CR
            '   \n = CRLF
            '   \[ = [
            '   \\ = \
            '*** Placeholder ***
            '   []		= value of parameter list will be skipped
            '   [*]		= value will be inserted without any special format
            '   [###]	= "###" stands for a format string (further details in your SDK help for command "Format()")
            '   [n:...]	= n as 1..9: uses the n-th parameter; the internal parameter index will not be increased!
            '             There can be a format string behind the ":"

            Dim iv%, orig_iv%, iob%, icb%
            Dim formatString As String

            If message Is Nothing Then message = ""
            message = message.Replace("\\", "[\]")
            message = message.Replace("\t", vbTab)
            message = message.Replace("\r", vbCr)
            message = message.Replace("\n", vbCrLf)
            message = message.Replace("\[", "[(]")

            iob = 1
            Do
                iob = InStr(iob, message, "[")
                If iob = 0 Then Exit Do

                icb = InStr(iob + 1, message, "]")
                If icb = 0 Then
                    Err.Raise(errpfNoClosingBracket, "printf", "Missing ']' after '[' at position " & iob & "!")
                End If

                formatString = Mid$(message, iob + 1, icb - iob - 1)

                If InStr("123456789", Mid$(formatString, 1, 1)) > 0 And Mid$(formatString, 2, 1) = ":" Then
                    orig_iv = iv

                    iv = CInt(Mid$(formatString, 1, 1)) - 1
                    If iv > UBound(values) Then iv = UBound(values)

                    formatString = Mid$(formatString, 3)
                Else
                    orig_iv = -1
                End If


                Select Case formatString
                    Case ""
                        formatString = ""
                    Case "("
                        formatString = "["
                        iv = iv - 1
                    Case "\"
                        formatString = "\"
                        iv = iv - 1
                    Case "*"
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = CType(values(iv), String)
                    Case Else 'with user specified format string
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = CType(values(iv), String) 'Format(values(iv), formatString)
                End Select

                message = Left$(message, iob - 1) & formatString & Mid$(message, icb + 1)
                iob = iob + Len(formatString) + CType(IIf(orig_iv >= 0, 2, 0), Integer)

                If orig_iv >= 0 Then
                    iv = orig_iv
                Else
                    iv = iv + 1
                End If
            Loop

            sprintf = message

        End Function
        ''' <summary>
        ''' Join the elements of an integer array to a single string
        ''' </summary>
        ''' <param name="values">An array of values</param>
        ''' <param name="delimiter">A delimiter which shall separate the values in the string</param>
        Public Shared Function JoinArrayToString(ByVal values As Integer(), ByVal delimiter As String) As String
            Dim result As New System.Text.StringBuilder
            If values Is Nothing Then
                Throw New ArgumentNullException("values")
            End If
            For MyCounter As Integer = 0 To values.Length - 1
                If MyCounter <> 0 Then result.Append(delimiter)
                result.Append(values(MyCounter))
            Next
            Return result.ToString
        End Function
        ''' <summary>
        ''' Join the elements of an integer array to a single string
        ''' </summary>
        ''' <param name="values">An array of values</param>
        ''' <param name="delimiter">A delimiter which shall separate the values in the string</param>
        Friend Shared Function _JoinArrayToString(ByVal values As Long(), ByVal delimiter As String) As String
            Dim result As New System.Text.StringBuilder
            If values Is Nothing Then
                Throw New ArgumentNullException("values")
            End If
            For MyCounter As Integer = 0 To values.Length - 1
                If MyCounter <> 0 Then result.Append(delimiter)
                result.Append(values(MyCounter))
            Next
            Return result.ToString
        End Function
        ''' <summary>
        '''     Join all items of a NameValueCollection (for example Request.QueryString) to a simple string
        ''' </summary>
        ''' <param name="NameValueCollectionToString">A collection like Request.Form or Request.QueryString</param>
        ''' <param name="BeginningOfItem">A string in front of a key</param>
        ''' <param name="KeyValueSeparator">The string between key and value</param>
        ''' <param name="EndOfItem">The string to be placed at the end of a value</param>
        ''' <returns>A string containing all elements of the collection</returns>
        Public Shared Function JoinNameValueCollectionToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection, ByVal BeginningOfItem As String, ByVal KeyValueSeparator As String, ByVal EndOfItem As String) As String
            Dim Result As String = Nothing
            For Each ParamItem As String In NameValueCollectionToString
                Result &= BeginningOfItem & ParamItem & KeyValueSeparator & NameValueCollectionToString(ParamItem) & EndOfItem
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Join all items of a NameValueCollection (for example Request.QueryString) to a simple string
        ''' </summary>
        ''' <param name="NameValueCollectionToString">A collection like Request.Form or Request.QueryString</param>
        ''' <returns>A string containing all elements of the collection which can be appended to any internet address</returns>
        ''' <remarks>
        '''     If you need to read the values directly from the returned string, pay attention that all names and values might be UrlEncoded and you have to decode them, first.
        ''' </remarks>
        ''' <see also="FillNameValueCollectionWith" />
        Public Shared Function JoinNameValueCollectionWithUrlEncodingToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection) As String
            Dim Result As String = Nothing
            For Each ParamItem As String In NameValueCollectionToString
                If Result <> Nothing Then
                    Result &= "&"
                End If
                Result &= System.Web.HttpUtility.UrlEncode(ParamItem) & "=" & System.Web.HttpUtility.UrlEncode(NameValueCollectionToString(ParamItem))
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Restore a NameValueCollection's content which has been previously converted to a string
        ''' </summary>
        ''' <param name="nameValueCollection">An already existing NameValueCollection which shall receive the new values</param>
        ''' <param name="nameValueCollectionWithUrlEncoding">A string containing the UrlEncoded writing style of a NameValueCollection</param>
        ''' <remarks>
        '''     Please note: existing values in the collection won't be appended, they'll be overridden
        ''' </remarks>
        ''' <see also="JoinNameValueCollectionWithUrlEncodingToString" />
        Public Shared Sub ReFillNameValueCollection(ByVal nameValueCollection As System.Collections.Specialized.NameValueCollection, ByVal nameValueCollectionWithUrlEncoding As String)

            If nameValueCollection Is Nothing Then
                Throw New ArgumentNullException("nameValueCollection")
            End If
            If nameValueCollectionWithUrlEncoding = Nothing Then
                'Nothing to do
                Return
            End If

            'Split at "&"
            Dim parameters As String() = nameValueCollectionWithUrlEncoding.Split(New Char() {"&"c})
            For MyCounter As Integer = 0 To parameters.Length - 1
                Dim KeyValuePair As String() = parameters(MyCounter).Split(New Char() {"="c})
                If KeyValuePair.Length = 0 Then
                    'empty - nothing to do
                ElseIf KeyValuePair.Length = 1 Then
                    'key name only
                    nameValueCollection(System.Web.HttpUtility.UrlDecode(KeyValuePair(0))) = Nothing
                ElseIf KeyValuePair.Length = 2 Then
                    'key/value pair
                    nameValueCollection(System.Web.HttpUtility.UrlDecode(KeyValuePair(0))) = System.Web.HttpUtility.UrlDecode(KeyValuePair(1))
                Else
                    'invalid data
                    Throw New ArgumentException("Invalid data - more than one equals signs (""="") found", "nameValueCollectionWithUrlEncoding")
                End If
            Next

        End Sub
        ''' <summary>
        '''     Converts HTML messages to simple text
        ''' </summary>
        ''' <param name="HTML">A string with HTML code</param>
        ''' <returns>The rendered output as plain text</returns>
        Public Shared Function ConvertHTMLToText(ByVal HTML As String) As String
            'TODO: 1. remove of all other HTML tags
            '      2. search case insensitive
            '      3. truncate content between <head> and </head>, <script> and </script>, <!-- and -->
            '      Please note: there is already a camm HTML file filter which might get reused here
            Dim Result As String = HTML
            Result = ReplaceString(Result, vbNewLine, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, vbCr, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, vbLf, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&lt;", "<", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&gt;", "<", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&amp;", "&", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&quot;", """", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&micro;", "µ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&sect;", "§", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&copy;", "©", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&reg;", "®", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&trade;", "™", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&bull;", "• ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&euro;", "€", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&mdash;", " – ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&mdash;", " — ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<p>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<br>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<br/>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<br />", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<p.*?>", vbNewLine & vbNewLine)
            Result = ReplaceString(Result, "</p>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<li.*?>", "- ")
            Result = ReplaceString(Result, "</li>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<ol.*?>", "- ")
            Result = ReplaceString(Result, "</ol>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<ul.*?>", "")
            Result = ReplaceString(Result, "</ul>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<i.*?>", "")
            Result = ReplaceString(Result, "</i>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<b.*?>", "")
            Result = ReplaceString(Result, "</b>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<em.*?>", "")
            Result = ReplaceString(Result, "</em>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<strong.*?>", "")
            Result = ReplaceString(Result, "</strong>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<small>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "</small>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<html.*?>", "")
            Result = ReplaceString(Result, "</html>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<body.*?>", "")
            Result = ReplaceString(Result, "</body>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<span.*?>", "")
            Result = ReplaceString(Result, "</span>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<a.*?>", "")
            Result = ReplaceString(Result, "</a>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<font.*?>", "")
            Result = ReplaceString(Result, "</font>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<div.*?>", vbNewLine)
            Result = ReplaceString(Result, "</div>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<input.*?>", vbNewLine)
            Result = ReplaceString(Result, "</input>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<label.*?>", vbNewLine)
            Result = ReplaceString(Result, "</label>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<fieldset.*?>", vbNewLine)
            Result = ReplaceString(Result, "</fieldset>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<option.*?>", vbNewLine)
            Result = ReplaceString(Result, "</option>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<select.*?>", vbNewLine)
            Result = ReplaceString(Result, "</select>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<form.*?>", vbNewLine)
            Result = ReplaceString(Result, "</form>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<optgroup.*?>", vbNewLine)
            Result = ReplaceString(Result, "</optgroup>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<!DOCTYPE.*?>", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<table.*?>", vbNewLine)
            Result = ReplaceString(Result, "</table>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<tr.*?>", vbNewLine)
            Result = ReplaceString(Result, "</tr>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<th.*?>", "")
            Result = ReplaceString(Result, "</th>", vbTab, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<td.*?>", "")
            Result = ReplaceString(Result, "</td>", vbTab, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<thead.*?>", "")
            Result = ReplaceString(Result, "</thead>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<tbody.*?>", "")
            Result = ReplaceString(Result, "</tbody>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<tfoot.*?>", "")
            Result = ReplaceString(Result, "</tfoot>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<caption.*?>", "")
            Result = ReplaceString(Result, "</caption>", vbTab, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<!--.*-->", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<script.*</script?>", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<head?>.*</head?>", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<img.*?>", "")
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreaksBeforeAndAfter(Result, "<hl.*?>", vbNewLine & "-------------------------------------------------------" & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreaksBeforeAndAfter(Result, "<hr.*?>", vbNewLine & "-------------------------------------------------------" & vbNewLine)
            'Result = ReplaceByRegExIgnoringCase(Result, "<h1>.*?</h1>", vbNewLine & Result.ToUpperInvariant & vbnewline)
            'Result = ReplaceByRegExIgnoringCase(Result, "<h2>.*?</h2>", vbNewLine & Result.ToUpperInvariant & vbnewline)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h1.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h1?>", vbNewLine & "=======================================================" & vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h2.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h2?>", vbNewLine & "-------------------------------------------------------" & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h3.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h3?>", vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h4.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h4?>", vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h5.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h5?>", vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h6.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h6?>", vbNewLine)
            Result = ReplaceString(Result, vbTab, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Dim TextLength As Integer
            Do
                'Loop as long as replacements takes effect
                TextLength = Result.Length
                Result = Result.Replace("  ", " ")
            Loop Until TextLength = Result.Length
            Result = ReplaceString(Result, " &nbsp;", " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&nbsp; ", " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&nbsp;", " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Return Result
        End Function

        Private Shared Function ReplaceByRegExIgnoringCaseEnforcingLineBreaksBeforeAndAfter(html As String, searchExpression As String, replaceValueWithLineBreaksBeforeAndAfter As String) As String
            Dim Result As String = html
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*" & searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreaksBeforeAndAfter)
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*" & searchExpression & "p{Zs}*", replaceValueWithLineBreaksBeforeAndAfter)
            Result = ReplaceByRegExIgnoringCase(Result, "\p{Zs}*" & searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreaksBeforeAndAfter)
            Result = ReplaceByRegExIgnoringCase(Result, "\p{Zs}*" & searchExpression & "\p{Zs}*", replaceValueWithLineBreaksBeforeAndAfter)
            Return Result
        End Function

        Private Shared Function ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(html As String, searchExpression As String, replaceValueWithLineBreaksBefore As String) As String
            Dim Result As String = html
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*(?:\r\n|\r|\n)\p{Zs}*" & searchExpression, replaceValueWithLineBreaksBefore)
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*" & searchExpression, replaceValueWithLineBreaksBefore)
            Result = ReplaceByRegExIgnoringCase(Result, searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreaksBefore)
            Return Result
        End Function

        Private Shared Function ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(html As String, searchExpression As String, replaceValueWithLineBreakAfter As String) As String
            Dim Result As String = html
            Result = ReplaceByRegExIgnoringCase(Result, searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreakAfter)
            Result = ReplaceByRegExIgnoringCase(Result, searchExpression, replaceValueWithLineBreakAfter)
            Return Result
        End Function

        Friend Shared Function ReplaceByRegExIgnoringCase(ByVal text As String, ByVal searchForRegExpression As String, ByVal replaceBy As String) As String
            Return ReplaceByRegExIgnoringCase(text, searchForRegExpression, replaceBy, System.Text.RegularExpressions.RegexOptions.IgnoreCase Or System.Text.RegularExpressions.RegexOptions.CultureInvariant Or System.Text.RegularExpressions.RegexOptions.Multiline)
        End Function

        Friend Shared Function ReplaceByRegExIgnoringCase(ByVal text As String, ByVal searchForRegExpression As String, ByVal replaceBy As String, ByVal options As System.Text.RegularExpressions.RegexOptions) As String
            Return System.Text.RegularExpressions.Regex.Replace(text, searchForRegExpression, replaceBy, options)
        End Function

        ''' <summary>
        ''' String comparison types for ReplaceString method
        ''' </summary>
        ''' <remarks></remarks>
        Friend Enum ReplaceComparisonTypes As Byte
            ''' <summary>
            ''' Compare 2 strings with case sensitivity
            ''' </summary>
            ''' <remarks></remarks>
            CaseSensitive = 0
            ''' <summary>
            ''' Compare 2 strings by lowering their case based on the current culture
            ''' </summary>
            ''' <remarks></remarks>
            CurrentCultureIgnoreCase = 1
            ''' <summary>
            ''' Compare 2 strings by lowering their case following invariant culture rules
            ''' </summary>
            ''' <remarks></remarks>
            InvariantCultureIgnoreCase = 2
        End Enum

        ''' <summary>
        ''' Replace a string in another string based on a defined StringComparison type
        ''' </summary>
        ''' <param name="original">The original string</param>
        ''' <param name="pattern">The search expression</param>
        ''' <param name="replacement">The string which shall be inserted instead of the pattern</param>
        ''' <param name="comparisonType">The comparison type for searching for the pattern</param>
        ''' <returns>A new string with all replacements</returns>
        ''' <remarks></remarks>
        Friend Shared Function ReplaceString(ByVal original As String, ByVal pattern As String, ByVal replacement As String, ByVal comparisonType As ReplaceComparisonTypes) As String
            If original = Nothing OrElse pattern = Nothing Then
                Return original
            End If
            Dim lenPattern As Integer = pattern.Length
            Dim idxPattern As Integer = -1
            Dim idxLast As Integer = 0
            Dim result As New System.Text.StringBuilder
            Select Case comparisonType
                Case ReplaceComparisonTypes.CaseSensitive
                    While True
                        idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType)
                        If idxPattern < 0 Then
                            result.Append(original, idxLast, original.Length - idxLast)
                            Exit While
                        End If
                        result.Append(original, idxLast, idxPattern - idxLast)
                        result.Append(replacement)
                        idxLast = idxPattern + lenPattern
                    End While
                Case ReplaceComparisonTypes.CurrentCultureIgnoreCase
                    While True
                        Dim comparisonStringOriginal As String, comparisonStringPattern As String
                        comparisonStringOriginal = original.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        comparisonStringPattern = pattern.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        idxPattern = comparisonStringOriginal.IndexOf(comparisonStringPattern, idxPattern + 1)
                        If idxPattern < 0 Then
                            result.Append(original, idxLast, original.Length - idxLast)
                            Exit While
                        End If
                        result.Append(original, idxLast, idxPattern - idxLast)
                        result.Append(replacement)
                        idxLast = idxPattern + lenPattern
                    End While
                Case ReplaceComparisonTypes.InvariantCultureIgnoreCase
                    While True
                        Dim comparisonStringOriginal As String, comparisonStringPattern As String
                        comparisonStringOriginal = original.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        comparisonStringPattern = pattern.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        idxPattern = comparisonStringOriginal.IndexOf(comparisonStringPattern, idxPattern + 1)
                        If idxPattern < 0 Then
                            result.Append(original, idxLast, original.Length - idxLast)
                            Exit While
                        End If
                        result.Append(original, idxLast, idxPattern - idxLast)
                        result.Append(replacement)
                        idxLast = idxPattern + lenPattern
                    End While
                Case Else
                    Throw New ArgumentOutOfRangeException("comparisonType", "Invalid value")
            End Select
            Return result.ToString()
        End Function

#End Region

#Region "LinkHighlighting"
        ''' <summary>
        '''     Converts addresses or URLs in a string into HTML hyperlinks
        ''' </summary>
        ''' <param name="LinkInitiator">Search for this start string of a word</param>
        ''' <param name="Msg">Search inside this string</param>
        ''' <param name="AdditionalProtocolInitiator">An additionally required prefix</param>
        ''' <returns>HTML with hyperlinks</returns>
        ''' <example language="vb">
        '''     Dim HTMLResult As String = ConvertProtocolAddressIntoHyperLink ("www.", Text, "http://")
        ''' </example>
        Private Shared Function ConvertProtocolAddressIntoHyperLink(ByVal LinkInitiator As String, ByVal Msg As String, ByVal AdditionalProtocolInitiator As String) As String
            Dim OldEndPoint As Integer
            Dim LinkStartPos As Integer
            Dim LinkEndPos As Integer
            Dim LinkEndPosBySpaceChar As Integer
            Dim LinkEndPosByReturnChar As Integer

            'String nach umwandelbaren Zeichenfolgen durchforsten und abändern
            OldEndPoint = 0
            Do
                LinkStartPos = InStr(OldEndPoint + 1, LCase(Msg), LinkInitiator)
                If LinkStartPos <> 0 Then
                    LinkEndPosBySpaceChar = InStr(LinkStartPos, LCase(Msg), " ") - 1
                    If LinkEndPosBySpaceChar = -1 Then LinkEndPosBySpaceChar = Len(Msg)
                    LinkEndPosByReturnChar = InStr(LinkStartPos, LCase(Msg), Chr(13)) - 1
                    If LinkEndPosByReturnChar = -1 Then LinkEndPosByReturnChar = Len(Msg)
                    LinkEndPos = CType(IIf(LinkEndPosBySpaceChar < LinkEndPosByReturnChar, LinkEndPosBySpaceChar, LinkEndPosByReturnChar), Integer)
                    'Exclude Satzzeichen
                    If Mid(Msg, LinkEndPos, 1) = "." Or _
                            Mid(Msg, LinkEndPos, 1) = "!" Or _
                            Mid(Msg, LinkEndPos, 1) = "?" Or _
                            Mid(Msg, LinkEndPos, 1) = "," Or _
                            Mid(Msg, LinkEndPos, 1) = ";" Or _
                            Mid(Msg, LinkEndPos, 1) = ":" Then
                        LinkEndPos = LinkEndPos - 1
                    End If
                    If Len(LinkInitiator) <> LinkEndPos - LinkStartPos + 1 Then
                        Msg = Mid(Msg, 1, LinkStartPos - 1) & "<a href=""" & AdditionalProtocolInitiator & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & "</a>" & Mid(Msg, LinkEndPos + 1)
                        OldEndPoint = LinkEndPos + Len("<a href=""" & AdditionalProtocolInitiator & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & "</a>")
                    Else
                        OldEndPoint = LinkEndPos
                    End If
                End If
            Loop Until LinkStartPos = 0
            ConvertProtocolAddressIntoHyperLink = Msg

        End Function
        ''' <summary>
        '''     Convert e-mail addresses into hyperlinks
        ''' </summary>
        ''' <param name="Msg">The string where to search in</param>
        ''' <returns>HTML with hyperlinks</returns>
        Private Shared Function ConvertEMailAddressIntoHyperLink(ByVal Msg As String) As String
            Dim OldEndPoint As Integer
            Dim LinkStartPos As Integer
            Dim LinkEndPos As Integer
            Dim LinkEndPosBySpaceChar As Integer
            Dim LinkEndPosByReturnChar As Integer
            Dim LefterChar As String

            'String nach umwandelbaren Zeichenfolgen durchforsten und abändern
            OldEndPoint = 0
            Do
                LinkStartPos = InStr(OldEndPoint + 1, LCase(Msg), "@")
                Do
                    'Anfang der e-mail-Adresse suchen
                    If LinkStartPos > 1 Then
                        LefterChar = Mid(Msg, LinkStartPos - 1, 1)
                    Else
                        LefterChar = " "
                    End If
                    If LefterChar = " " Or _
                            LefterChar = Chr(13) Or _
                            LefterChar = Chr(10) Or _
                            LefterChar = "!" Or _
                            LefterChar = "?" Or _
                            LefterChar = "," Or _
                            LefterChar = ";" Or _
                            LefterChar = ":" Then
                        Exit Do
                    Else
                        LinkStartPos = LinkStartPos - 1
                    End If
                Loop

                If LinkStartPos <> 0 Then
                    LinkEndPosBySpaceChar = InStr(LinkStartPos, LCase(Msg), " ") - 1
                    If LinkEndPosBySpaceChar = -1 Then LinkEndPosBySpaceChar = Len(Msg)
                    LinkEndPosByReturnChar = InStr(LinkStartPos, LCase(Msg), Chr(13)) - 1
                    If LinkEndPosByReturnChar = -1 Then LinkEndPosByReturnChar = Len(Msg)
                    LinkEndPos = CType(IIf(LinkEndPosBySpaceChar < LinkEndPosByReturnChar, LinkEndPosBySpaceChar, LinkEndPosByReturnChar), Integer)
                    'Exclude Satzzeichen
                    If Mid(Msg, LinkEndPos, 1) = "." Or _
                            Mid(Msg, LinkEndPos, 1) = "!" Or _
                            Mid(Msg, LinkEndPos, 1) = "?" Or _
                            Mid(Msg, LinkEndPos, 1) = "," Or _
                            Mid(Msg, LinkEndPos, 1) = ";" Or _
                            Mid(Msg, LinkEndPos, 1) = ":" Then
                        LinkEndPos = LinkEndPos - 1
                    End If
                    If 6 < LinkEndPos - LinkStartPos + 1 Then
                        Msg = Mid(Msg, 1, LinkStartPos - 1) & "<a href=""mailto:" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & "</a>" & Mid(Msg, LinkEndPos + 1)
                        OldEndPoint = LinkEndPos + Len("<a href=""mailto:" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & "</a>")
                    Else
                        OldEndPoint = LinkEndPos
                    End If
                End If
            Loop Until LinkStartPos = 0
            ConvertEMailAddressIntoHyperLink = Msg

        End Function
        Private Shared Function ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink(ByVal SearchForTypicalString As String, ByVal Msg As String, ByVal ProtocolInitiator As String) As String
            Dim OldEndPoint As Integer
            Dim LinkAreaStartPos As Integer
            Dim LinkAreaStartPosByAnchorWithSpace As Integer
            Dim LinkAreaStartPosByAnchorWithReturn As Integer
            Dim ProbeArea As String

            'String nach umwandelbaren Adressen in nicht-Link-Bereichen aufspüren
            OldEndPoint = 0
            Do
                LinkAreaStartPosByAnchorWithSpace = InStr(OldEndPoint + 1, LCase(Msg), "<a ")
                If LinkAreaStartPosByAnchorWithSpace = 0 Then LinkAreaStartPosByAnchorWithSpace = Len(Msg) + 1
                LinkAreaStartPosByAnchorWithReturn = InStr(OldEndPoint + 1, LCase(Msg), "<a" & Chr(13))
                If LinkAreaStartPosByAnchorWithReturn = 0 Then LinkAreaStartPosByAnchorWithReturn = Len(Msg) + 1
                LinkAreaStartPos = CType(IIf(LinkAreaStartPosByAnchorWithSpace < LinkAreaStartPosByAnchorWithReturn, LinkAreaStartPosByAnchorWithSpace, LinkAreaStartPosByAnchorWithReturn), Integer)
                ProbeArea = Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1)
                ProbeArea = ConvertProtocolAddressIntoHyperLink(SearchForTypicalString, ProbeArea, ProtocolInitiator)
                Msg = Mid(Msg, 1, OldEndPoint) & ProbeArea & Mid(Msg, LinkAreaStartPos)
                LinkAreaStartPos = LinkAreaStartPos + Len(ProbeArea) - Len(Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1))
                OldEndPoint = InStr(LinkAreaStartPos, LCase(Msg), "</a>") + 3
            Loop Until LinkAreaStartPos = Len(Msg) Or OldEndPoint = Len(Msg) Or OldEndPoint = 3
            ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink = Msg

        End Function
        Private Shared Function ConvertEMailAddressCurrentlyWOHyperLinkToHyperLink(ByVal Msg As String) As String
            Dim OldEndPoint As Integer
            Dim LinkAreaStartPos As Integer
            Dim LinkAreaStartPosByAnchorWithSpace As Integer
            Dim LinkAreaStartPosByAnchorWithReturn As Integer
            Dim ProbeArea As String

            'String nach umwandelbaren e-mail-Adressen in nicht-Link-Bereichen aufspüren
            OldEndPoint = 0
            Do
                LinkAreaStartPosByAnchorWithSpace = InStr(OldEndPoint + 1, LCase(Msg), "<a ")
                If LinkAreaStartPosByAnchorWithSpace = 0 Then LinkAreaStartPosByAnchorWithSpace = Len(Msg) + 1
                LinkAreaStartPosByAnchorWithReturn = InStr(OldEndPoint + 1, LCase(Msg), "<a" & Chr(13))
                If LinkAreaStartPosByAnchorWithReturn = 0 Then LinkAreaStartPosByAnchorWithReturn = Len(Msg) + 1
                LinkAreaStartPos = CType(IIf(LinkAreaStartPosByAnchorWithSpace < LinkAreaStartPosByAnchorWithReturn, LinkAreaStartPosByAnchorWithSpace, LinkAreaStartPosByAnchorWithReturn), Integer)
                ProbeArea = Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1)
                ProbeArea = ConvertEMailAddressIntoHyperLink(ProbeArea)
                Msg = Mid(Msg, 1, OldEndPoint) & ProbeArea & Mid(Msg, LinkAreaStartPos)
                LinkAreaStartPos = LinkAreaStartPos + Len(ProbeArea) - Len(Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1))
                OldEndPoint = InStr(LinkAreaStartPos, LCase(Msg), "</a>") + 3
            Loop Until LinkAreaStartPos = Len(Msg) Or OldEndPoint = Len(Msg) Or OldEndPoint = 3
            ConvertEMailAddressCurrentlyWOHyperLinkToHyperLink = Msg

        End Function
        ''' <summary>
        '''     Converts URLs and e-mail addresses in a string into HTML hyperlinks
        ''' </summary>
        ''' <param name="Text">The standard text without any HTML</param>
        ''' <returns>HTML with hyperlinks</returns>
        Public Shared Function HighlightLinksInMessage(ByVal Text As String) As String
            Dim HTMLMsg As String = Text

            HTMLMsg = ConvertProtocolAddressIntoHyperLink("http://", HTMLMsg, "")
            HTMLMsg = ConvertProtocolAddressIntoHyperLink("https://", HTMLMsg, "")
            HTMLMsg = ConvertProtocolAddressIntoHyperLink("ftp://", HTMLMsg, "")
            HTMLMsg = ConvertProtocolAddressIntoHyperLink("mailto:", HTMLMsg, "")

            HTMLMsg = ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink("www.", HTMLMsg, "http://")
            HTMLMsg = ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink("ftp.", HTMLMsg, "ftp://")

            HTMLMsg = ConvertEMailAddressCurrentlyWOHyperLinkToHyperLink(HTMLMsg)

            Return HTMLMsg

        End Function
#End Region

#Region "Paths and URIs"
        ''' <summary>
        ''' Remove a possibly trailing slash from an URL
        ''' </summary>
        ''' <param name="url">An URL address</param>
        Friend Shared Function RemoveTrailingSlash(ByVal url As String) As String
            If url.Length > 0 AndAlso Right(url, 1) = "/" Then
                Return Mid(url, 1, url.Length - 1)
            Else
                Return url
            End If
        End Function
        ''' <summary>
        '''     If the path is a unix path with a filename at the end, the file name will be removed. The resulting path always ends with a "/".
        ''' </summary>
        ''' <param name="path">A unix path with or without a filename</param>
        ''' <remarks>
        ''' All letters behind the last slash will be removed, so a path ending with a slash will never be modified.
        ''' </remarks>
        Public Shared Function RemoveFilenameInUnixPath(ByVal path As String) As String
            If path Is Nothing Then
                Return Nothing
            ElseIf path.EndsWith("/") Then
                Return path
            ElseIf path.IndexOf("/"c) < 0 Then
                Return path
            Else
                Return path.Substring(0, path.LastIndexOf("/"c) + 1)
            End If
        End Function
        ''' <summary>
        '''     Return the full virtual path based on the given string
        ''' </summary>
        ''' <param name="virtualPath">A path like ~/images or images/styles or /images/</param>
        Friend Shared Function FullyInterpretedVirtualPath(ByVal virtualPath As String) As String
            If virtualPath Is Nothing Then
                Throw New ArgumentNullException("virtualPath")
            End If
            If virtualPath.StartsWith("~/") Then
                virtualPath = Replace(virtualPath, "~/", "")
                Dim myApplicationPath As String = System.Web.HttpContext.Current.Request.ApplicationPath()
                If Not myApplicationPath.EndsWith("/") Then
                    myApplicationPath = myApplicationPath & "/"
                End If
                virtualPath = myApplicationPath & virtualPath
            ElseIf virtualPath.StartsWith("/") Then
                'Do nothing, because it is already the servers rootpath
            Else
                Dim currentVirtualPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
                If Not currentVirtualPath.EndsWith("/") Then
                    currentVirtualPath = currentVirtualPath.Substring(0, currentVirtualPath.LastIndexOf("/") + 1)
                End If
                virtualPath = currentVirtualPath & virtualPath
            End If
            Return virtualPath
        End Function
        ''' <summary>
        '''     Return the full physical path based on the given string
        ''' </summary>
        ''' <param name="virtualPath">A path like ~/images or images/styles or /images/</param>
        ''' <remarks>
        '''     Requires execution on a web server (because HttpContext must be there)
        ''' </remarks>
        Friend Shared Function FullyInterpredetPhysicalPath(ByVal virtualPath As String) As String
            Return System.Web.HttpContext.Current.Server.MapPath(virtualPath)
        End Function
        ''' <summary>
        '''     The script name without path and query information, only the file name itself
        ''' </summary>
        ''' <returns>E. g. "index.aspx"</returns>
        Public Shared Function ScriptNameWithoutPath() As String
            Dim result As String

            If System.Environment.Version.Major >= 4 AndAlso System.Web.HttpContext.Current.Request.RawUrl.EndsWith("/") Then
                'Beginning with .NET 4, RawUrl contains the URL as requested by the client, so the script name after a folder might be missing; e.g. /test/ is given, but required is /test/default.aspx later on
                result = System.Web.HttpContext.Current.Request.Url.AbsolutePath
            Else
                '.NET 1 + 2: RawUrl contains the URL as requested by the client + the request script name, so the script name after a folder is present; e.g. /test/ is given, RawUrl returns the expected /test/default.aspx
                result = System.Web.HttpContext.Current.Request.RawUrl
            End If

            If result.IndexOf("?"c) > -1 Then
                result = result.Substring(0, result.IndexOf("?"c))
            End If
            If result.LastIndexOf("/"c) > -1 Then
                result = result.Substring(result.LastIndexOf("/"c) + 1)
            End If

            Return result
        End Function
        ''' <summary>
        '''     Check validity of an email address
        ''' </summary>
        ''' <param name="emailAddress">email address to be validated</param>
        ''' <returns>True if email address is syntactically valid else false</returns>
        Public Shared Function ValidateEmailAddress(ByVal emailAddress As String) As Boolean
            If Trim(emailAddress) = Nothing Then
                Return False
            ElseIf emailAddress.IndexOf(" ") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf(",") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf(";") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf(":") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf("+") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf("!") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf("?") >= 0 Then
                Return False
            ElseIf emailAddress.IndexOf("*") >= 0 Then
                Return False
            Else
                Dim regEx As New System.Text.RegularExpressions.Regex("\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
                Return regEx.IsMatch(Trim(emailAddress))
            End If
        End Function
        ''' <summary>
        '''     Check validity of an URL
        ''' </summary>
        ''' <param name="url">URL to be validated</param>
        ''' <returns>True if URL is syntactically valid else false</returns>
        Public Shared Function ValidateInternetUrl(ByVal url As String) As Boolean
            If Trim(url) = Nothing Then
                Return False
            Else
                If IsRegExMatch("http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", url) Then
                    Return True
                ElseIf IsRegExMatch("https://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", url) Then
                    Return True
                ElseIf IsRegExMatch("ftp://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", url) Then
                    Return True
                ElseIf IsRegExMatch("mailto:([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", url) Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function
#End Region

#Region "IO"

        ''' <summary>
        ''' Write all bytes to a binary file
        ''' </summary>
        ''' <param name="path">The file path for the output</param>
        ''' <param name="bytes">File output data</param>
        ''' <remarks>An existing file will be overwritten</remarks>
        Friend Shared Sub WriteAllBytes(ByVal path As String, ByVal bytes As Byte())
            If (bytes Is Nothing) Then
                Throw New ArgumentNullException("bytes")
            End If
            Dim stream As System.IO.FileStream = New System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read)
            stream.Write(bytes, 0, bytes.Length)
            stream.Close()
        End Sub

#End Region

#Region "Assembly & Resources"

        ''' <summary>
        ''' Create an instance of an object which is defined by assembly name and class name
        ''' </summary>
        ''' <param name="assemblyName">The name of an assembly (e.g. "System.Data")</param>
        ''' <param name="className">A class name (e.g. "System.Data.DataTable")</param>
        ''' <returns>The created instance of the requested</returns>
        ''' <remarks>The loading procedure is in following order:
        ''' <list>
        ''' <item>Assemblies already loaded into the current application domain</item>
        ''' <item>Assembly loaded from disc (with standard search order in application folder, GAC, etc.)</item>
        ''' </list>
        ''' <para></para>
        ''' </remarks>
        ''' <exception cref="Exception">If the object can't be created or the assembly can't be loaded, there will be a System.Exception</exception>
        Friend Function CreateObject(ByVal assemblyName As String, ByVal className As String) As Object
            Return CreateObject(assemblyName, className, New Object() {})
        End Function
        ''' <summary>
        ''' Create an instance of an object which is defined by assembly name and class name
        ''' </summary>
        ''' <param name="assemblyName">An assembly name which is already loaded into the current application domain (e.g. "System.Data")</param>
        ''' <param name="className">A class name (e.g. "System.Data.DataTable")</param>
        ''' <param name="parameters">Parameters for the constructor of the new class instance</param>
        ''' <returns>The created instance of the requested</returns>
        ''' <remarks>The loading procedure is in following order:
        ''' <list>
        ''' <item>Assemblies already loaded into the current application domain</item>
        ''' <item>Assembly loaded from disc (with standard search order in application folder, GAC, etc.)</item>
        ''' </list>
        ''' <para></para>
        ''' </remarks>
        ''' <exception cref="Exception">If the object can't be created or the assembly can't be loaded, there will be a System.Exception</exception>
        Friend Function CreateObject(ByVal assemblyName As String, ByVal className As String, ByVal parameters As Object()) As Object
            Dim Result As Object = Nothing
            For Each MyAssembly As System.Reflection.Assembly In System.AppDomain.CurrentDomain.GetAssemblies
                If MyAssembly.GetName.Name.ToLower = assemblyName.ToLower Then
                    Result = MyAssembly.CreateInstance(className, True, Reflection.BindingFlags.Default, Nothing, parameters, Nothing, Nothing)
                    If Result Is Nothing Then
                        Throw New Exception("Object class could not be created")
                    End If
                    Exit For
                End If
            Next
            If Result Is Nothing Then
                Dim MyAssembly As System.Reflection.Assembly
                MyAssembly = System.Reflection.Assembly.Load(assemblyName)
                Result = MyAssembly.CreateInstance(className, True, Reflection.BindingFlags.Default, Nothing, parameters, Nothing, Nothing)
                If Result Is Nothing Then
                    Throw New Exception("Object class could not be created")
                End If
            End If
            If Result Is Nothing Then
                Throw New Exception("Assembly name not loaded in current application domain and not loadable on request")
            End If
            Return Result
        End Function
        ''' <summary>
        '''     Is the assembly already loaded into the current application domain?
        ''' </summary>
        ''' <param name="assemblyName">The name of an assembly</param>
        ''' <returns>True if the assembly is loaded, False if not</returns>
        Friend Function IsAssemblyAlreadyLoaded(ByVal assemblyName As String) As Boolean
            For Each MyAssembly As System.Reflection.Assembly In System.AppDomain.CurrentDomain.GetAssemblies
                If MyAssembly.GetName.Name.ToLower = assemblyName.ToLower Then
                    Return True
                End If
            Next
            Return False
        End Function
        ''' <summary>
        '''     Load a value from an embedded resource file
        ''' </summary>
        ''' <param name="resourceFileWithoutExtension">The name of the .resx file</param>
        ''' <param name="key">The key of the requested value</param>
        Friend Shared Function ResourceValue(ByVal resourceFileWithoutExtension As String, ByVal key As String) As String
            Dim Result As String = Nothing

            Dim ResMngr As Resources.ResourceManager = Nothing
            Try
                ResMngr = New Resources.ResourceManager(resourceFileWithoutExtension, System.Reflection.Assembly.GetExecutingAssembly)
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(key)
            Catch ex As Exception
                Throw New Exception("Embedded resource """ & key & """ in """ & resourceFileWithoutExtension & """ can't be found", ex)
            Finally
                If Not ResMngr Is Nothing Then ResMngr.ReleaseAllResources()
            End Try

            Return Result

        End Function

        ''' <summary>
        ''' Read an embedded, binary resource file
        ''' </summary>
        ''' <param name="embeddedFileName">The name of the resouces</param>
        ''' <remarks></remarks>
        Friend Shared Function ResourceBinaryValue(ByVal embeddedFileName As String) As Byte()
            Dim stream As System.IO.Stream = Nothing
            Dim buffer As Byte()
            Try
                Try
                    stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedFileName)
                Catch ex As Exception
                    Throw New Exception("Failure while loading resource name """ & embeddedFileName & """" & vbNewLine & "Available resource names are: " & String.Join(",", System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames), ex)
                End Try
                ReDim buffer(CInt(stream.Length) - 1)
                stream.Read(buffer, 0, CInt(stream.Length))
            Finally
                If Not stream Is Nothing Then stream.Close()
            End Try
            Return buffer
        End Function

#End Region

#Region "Regular Expressions"
        ''' <summary>
        '''     Check if a value matches to the regular expression
        ''' </summary>
        ''' <param name="regularExpression"></param>
        ''' <param name="value"></param>
        Private Shared Function IsRegExMatch(ByVal regularExpression As String, ByVal value As String) As Boolean
            Dim regEx As New System.Text.RegularExpressions.Regex(regularExpression)
            Return regEx.IsMatch(value)
        End Function
#End Region

#Region "Internationalization & Localization"
        Public Shared Function IsRightToLeftCulture(ByVal culture As System.Globalization.CultureInfo) As Boolean
            Return IsRightToLeftCulture(culture.Name)
        End Function

        Public Shared Function IsRightToLeftCulture(ByVal cultureName As String) As Boolean
            'Only look at the parent culture.   
            Dim myCulture As String = cultureName.Split("-"c)(0)
            Select Case myCulture
                Case "ar"
                    'Arabic
                    Return True
                Case "fa"
                    'Farsi
                    Return True
                Case "div"
                    'Divehi
                    Return True
                Case "syr"
                    'Syriac
                    Return True
                Case "he"
                    'Hebrew
                    Return True
                Case "ur"
                    'Urdu
                    Return True
                Case Else
                    Return False
            End Select
        End Function

#End Region

#Region "Lookup of server form"
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <param name="control"></param>
        ''' <returns>The control of the server form if it's existant</returns>
        Friend Shared Function LookupParentServerForm(ByVal control As System.Web.UI.Control) As System.Web.UI.HtmlControls.HtmlForm
            If control Is Nothing Then
                Throw New ArgumentNullException("control")
            End If
            If control.Parent Is Nothing Then
                Return Nothing
            Else
                Dim Result As System.Web.UI.HtmlControls.HtmlForm = Nothing
                'Is the parent already the server form?
                If GetType(System.Web.UI.HtmlControls.HtmlForm).IsInstanceOfType(control.Parent) Then
                    Result = CType(control.Parent, System.Web.UI.HtmlControls.HtmlForm)
                End If
                If Result Is Nothing Then
                    'Retry search in parent control
                    Return LookupParentServerForm(control.Parent)
                End If
                Return Result
            End If
        End Function
        ''' <summary>
        '''     Lookup the server form which resides on the page
        ''' </summary>
        ''' <param name="page"></param>
        ''' <returns>The control of the server form if it's existant</returns>
        Friend Shared Function LookupServerForm(ByVal page As System.Web.UI.Page) As System.Web.UI.HtmlControls.HtmlForm
            Return LookupServerForm(page.Controls, True)
        End Function
        ''' <summary>
        '''     Search in the controls collection for a server form
        ''' </summary>
        ''' <param name="controls"></param>
        ''' <param name="searchInChildren">True to execute the search in the controls collection recursively, False to not search in any children controls</param>
        ''' <returns>The control of the server form if it's existant</returns>
        Private Shared Function LookupServerForm(ByVal controls As System.Web.UI.ControlCollection, ByVal searchInChildren As Boolean) As System.Web.UI.HtmlControls.HtmlForm
            Dim Result As System.Web.UI.HtmlControls.HtmlForm = Nothing
            'Search in controls
            For MyCounter As Integer = 0 To controls.Count - 1
                If GetType(System.Web.UI.HtmlControls.HtmlForm).IsInstanceOfType(controls(MyCounter)) Then
                    Result = CType(controls(MyCounter), System.Web.UI.HtmlControls.HtmlForm)
                End If
            Next
            'Search in children of controls
            If Result Is Nothing Then
                For MyCounter As Integer = 0 To controls.Count - 1
                    If controls(MyCounter).HasControls = True Then
                        Result = LookupServerForm(controls(MyCounter).Controls, True)
                    End If
                Next
            End If
            'Return the result
            Return Result
        End Function
#End Region

#Region "Performing external requests"
        ''' <summary>
        '''     Perform a request to a remote server and read the HTML/text from there
        ''' </summary>
        ''' <param name="uri">A URI which shall be read</param>
        ''' <returns>A string containing the remote HTML code</returns>
        ''' <remarks>
        ''' Redirections will be executed automatically.
        ''' A defined charset is not guaranteed. The result will be in this charset as the remote server sends the document.
        ''' </remarks>
        ''' <history>
        ''' </history>
        Friend Shared Function GetHtmlFromUri(ByVal uri As String) As String
            Dim response As System.Net.HttpWebResponse = Nothing
            Dim dataStream As System.IO.Stream = Nothing
            Dim reader As System.IO.StreamReader = Nothing
            Dim result As String
            Try
                Dim request As System.Net.HttpWebRequest
                request = CType(System.Net.HttpWebRequest.Create(uri), System.Net.HttpWebRequest)
                request.AllowAutoRedirect = True
                response = CType(request.GetResponse(), System.Net.HttpWebResponse)
                ' Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream()
                ' Open the stream using a StreamReader for easy access.
                reader = New System.IO.StreamReader(dataStream, True)
                ' Read the content.
                result = reader.ReadToEnd()
            Finally
                ' Cleanup the streams and the response.
                If Not reader Is Nothing Then reader.Close()
                If Not dataStream Is Nothing Then dataStream.Close()
                If Not response Is Nothing Then response.Close()
            End Try
            Return result
        End Function
        ''' <summary>
        '''     Perform a request to a remote server and read the HTML/text from there
        ''' </summary>
        ''' <param name="uri">A URI which shall be read</param>
        ''' <returns>A string containing the remote HTML code</returns>
        ''' <remarks>
        ''' Redirections will be executed automatically.
        ''' A defined charset is not guaranteed. The result will be in this charset as the remote server sends the document.
        ''' </remarks>
        ''' <history>
        ''' </history>
        Friend Shared Function GetHtmlFromUri(ByVal uri As String, ByVal method As String, ByVal postData As System.Collections.Specialized.NameValueCollection) As String
            Return GetHtmlFromUri(uri, method, Utils.JoinNameValueCollectionWithUrlEncodingToString(postData))
        End Function
        ''' <summary>
        '''     Perform a request to a remote server and read the HTML/text from there
        ''' </summary>
        ''' <param name="uri">A URI which shall be read</param>
        ''' <returns>A string containing the remote HTML code</returns>
        ''' <remarks>
        ''' Redirections will be executed automatically.
        ''' A defined charset is not guaranteed. The result will be in this charset as the remote server sends the document.
        ''' </remarks>
        ''' <history>
        ''' </history>
        Friend Shared Function GetHtmlFromUri(ByVal uri As String, ByVal method As String, ByVal postData As System.Collections.Specialized.NameValueCollection, ByVal requestContentType As String, ByVal requestEncoding As System.Text.Encoding, ByVal responseEncoding As System.Text.Encoding) As String
            Return GetHtmlFromUri(uri, method, Utils.JoinNameValueCollectionWithUrlEncodingToString(postData), String.Empty, System.Text.Encoding.UTF8, System.Text.Encoding.UTF8)
        End Function
        ''' <summary>
        '''     Perform a request to a remote server and read the HTML/text from there
        ''' </summary>
        ''' <param name="uri">A URI which shall be read</param>
        ''' <returns>A string containing the remote HTML code</returns>
        ''' <remarks>
        ''' Redirections will be executed automatically.
        ''' A defined charset is not guaranteed. The result will be in this charset as the remote server sends the document.
        ''' </remarks>
        ''' <history>
        ''' </history>
        Friend Shared Function GetHtmlFromUri(ByVal uri As String, ByVal method As String, ByVal postData As String) As String
            Return GetHtmlFromUri(uri, method, postData, String.Empty, System.Text.Encoding.UTF8, System.Text.Encoding.UTF8)
        End Function
        ''' <summary>
        '''     Perform a request to a remote server and read the HTML/text from there
        ''' </summary>
        ''' <param name="uri">A URI which shall be read</param>
        ''' <returns>A string containing the remote HTML code</returns>
        ''' <remarks>
        ''' Redirections will be executed automatically.
        ''' A defined charset is not guaranteed. The result will be in this charset as the remote server sends the document.
        ''' </remarks>
        ''' <history>
        ''' </history>
        Friend Shared Function GetHtmlFromUri(ByVal uri As String, ByVal method As String, ByVal postData As String, ByVal requestContentType As String, ByVal requestEncoding As System.Text.Encoding, ByVal responseEncoding As System.Text.Encoding) As String

            If method = Nothing Then
                method = "GET"
            End If
            If requestContentType = Nothing Then
                requestContentType = "text/html" 'use text/xml for webservice requests
            End If

            Dim Result As System.Net.WebResponse = Nothing
            Dim ResponseText As String
            Dim RequestStream As Stream = Nothing
            Dim ReceiveStream As Stream = Nothing
            Dim sr As StreamReader = Nothing

            Try
                Dim req As System.Net.WebRequest
                req = System.Net.WebRequest.Create(uri)
                If method <> "POST" Then
                    req.Method = method
                Else
                    req.Method = "POST"
                    req.ContentType = requestContentType
                    If postData <> Nothing Then
                        Dim SomeBytes() As Byte
                        SomeBytes = requestEncoding.GetBytes(postData)
                        req.ContentLength = SomeBytes.Length
                        RequestStream = req.GetRequestStream()
                        RequestStream.Write(SomeBytes, 0, SomeBytes.Length)
                        RequestStream.Close()
                    Else
                        req.ContentLength = 0
                    End If
                End If
                Result = req.GetResponse()
                'TODO: review result.Headers("Content-Type"), META content-type, Unicode-Signature
                ReceiveStream = Result.GetResponseStream()
                sr = New StreamReader(ReceiveStream, responseEncoding)
                ResponseText = sr.ReadToEnd()
            Finally
                If Not Result Is Nothing Then
                    Result.Close()
                End If
                If Not sr Is Nothing Then
                    sr.Close()
                End If
                If Not RequestStream Is Nothing Then
                    RequestStream.Close()
                End If
                If Not ReceiveStream Is Nothing Then
                    ReceiveStream.Close()
                End If
            End Try

            Return ResponseText

        End Function

#End Region

#Region "Windows application event log"
        ''' <summary>
        ''' Write entry to event log 
        ''' </summary>
        ''' <param name="type">The entry type, e. g. Warning, Information</param>
        ''' <param name="data">The event details</param>
        ''' <returns>True if successful, false if not</returns>
        Friend Shared Function WriteToEventLog(ByVal type As System.Diagnostics.EventLogEntryType, ByVal data As String) As Boolean
            Return WriteToEventLog(data, "camm Web-Manager", type, "Application")
        End Function
        ''' <summary>
        ''' Write entry to event log 
        ''' </summary>
        ''' <param name="entry">Value to Write</param>
        ''' <param name="appName">Name of Client Application. Needed because before writing to event log, you must have a named EventLog source</param>
        ''' <param name="eventType">Entry Type, from EventLogEntryType Structure e.g., EventLogEntryType.Warning, EventLogEntryType.Error</param>
        ''' <param name="logName">Name of Log (System, Application; Security is read-only) If you specify a non-existent log, the log will be created</param>
        ''' <returns>True if successful, false if not</returns>
        ''' <remarks>
        '''EXAMPLES: 
        '''1.  Simple Example, Accepting All Defaults
        '''    WriteToEventLog "Hello Event Log"
        '''2.  Specify EventSource, EventType, and LogName
        '''    WriteToEventLog("Danger, Danger, Danger", "MyVbApp", EventLogEntryType.Warning, "System")
        '''
        '''NOTE:     EventSources are tightly tied to their log. 
        '''          So don't use the same source name for different 
        '''          logs, and vice versa
        ''' </remarks>
        Private Shared Function WriteToEventLog(ByVal entry As String, ByVal appName As String, Optional ByVal eventType As System.Diagnostics.EventLogEntryType = System.Diagnostics.EventLogEntryType.Information, Optional ByVal logName As String = "Application") As Boolean

            Dim objEventLog As New System.Diagnostics.EventLog

            Try
                If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Request Is Nothing Then
                    Dim Url As String = HttpContext.Current.Request.Url.ToString
                    entry = "URL=" & Url & ":" & vbNewLine & entry
                End If
                Try
                    'Register the App as an Event Source
                    If Not System.Diagnostics.EventLog.SourceExists(appName) Then
                        System.Diagnostics.EventLog.CreateEventSource(appName, logName)
                    End If
                    objEventLog.Source = appName
                    objEventLog.WriteEntry(entry, eventType)
                Catch
                    objEventLog.Source = "Application"
                    objEventLog.WriteEntry(appName & ": " & vbNewLine & entry, eventType)
                End Try
                Return True
            Catch
                Return False
            End Try

        End Function
#End Region

#Region "Compression"
        Public Class Compression

            Public Enum CompressionType
                GZip
                BZip2
                Zip
            End Enum

            Private Shared Function OutputStream(ByVal InputStream As Stream, ByVal CompressionProvider As CompressionType) As Stream

                Select Case CompressionProvider
                    Case CompressionType.BZip2
                        Return New ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(InputStream)

                    Case CompressionType.GZip
                        Return New ICSharpCode.SharpZipLib.GZip.GZipOutputStream(InputStream)

                    Case CompressionType.Zip
                        Return New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(InputStream)

                    Case Else
                        Return New ICSharpCode.SharpZipLib.GZip.GZipOutputStream(InputStream)

                End Select
            End Function

            Private Shared Function InputStream(ByVal InStream As Stream, ByVal CompressionProvider As CompressionType) As Stream

                Select Case CompressionProvider
                    Case CompressionType.BZip2
                        Return New ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(InStream)

                    Case CompressionType.GZip
                        Return New ICSharpCode.SharpZipLib.GZip.GZipInputStream(InStream)

                    Case CompressionType.Zip
                        Return New ICSharpCode.SharpZipLib.Zip.ZipInputStream(InStream)

                    Case Else
                        Return New ICSharpCode.SharpZipLib.GZip.GZipInputStream(InStream)

                End Select

            End Function

            Public Shared Function Compress(ByVal bytesToCompress As Byte(), ByVal CompressionProvider As CompressionType) As Byte()
                Dim ms As MemoryStream = New MemoryStream
                Dim s As Stream = OutputStream(ms, CompressionProvider)
                s.Write(bytesToCompress, 0, bytesToCompress.Length)
                s.Close()
                Return ms.ToArray()
            End Function

            Public Shared Function Compress(ByVal stringToCompress As String, ByVal CompressionProvider As CompressionType) As String
                Dim compressedData As Byte() = CompressToByte(stringToCompress, CompressionProvider)
                Dim strOut As String = Convert.ToBase64String(compressedData)
                Return strOut
            End Function

            Private Shared Function CompressToByte(ByVal stringToCompress As String, ByVal CompressionProvider As CompressionType) As Byte()
                Dim bytData As Byte() = Encoding.Unicode.GetBytes(stringToCompress)
                Return Compress(bytData, CompressionProvider)
            End Function

            Public Shared Function DeCompress(ByVal stringToDecompress As String, ByVal CompressionProvider As CompressionType) As String
                Dim outString As String = String.Empty

                If stringToDecompress = "" Then
                    Throw New ArgumentNullException("stringToDecompress", "You tried to use an empty string")
                End If
                Try
                    Dim inArr As Byte() = Convert.FromBase64String(stringToDecompress.Trim())
                    outString = System.Text.Encoding.Unicode.GetString(DeCompress(inArr, CompressionProvider))

                Catch nEx As NullReferenceException
                    Throw 'Return nEx.Message
                End Try
                Return outString

            End Function

            Public Shared Function DeCompress(ByVal bytesToDecompress As Byte(), ByVal CompressionProvider As CompressionType) As Byte()
                Dim writeData(4096) As Byte
                Dim s2 As Stream = InputStream(New MemoryStream(bytesToDecompress), CompressionProvider)
                Dim outStream As MemoryStream = New MemoryStream
                While True
                    Dim size As Integer = s2.Read(writeData, 0, writeData.Length)
                    If (size > 0) Then
                        outStream.Write(writeData, 0, size)
                    Else
                        Exit While
                    End If
                End While
                s2.Close()

                Dim outArr As Byte() = outStream.ToArray()
                outStream.Close()
                Return outArr

            End Function

        End Class
#End Region

#Region "Zip archives"
        Public Class Zip

            Private Interface ZipFileItem
                Property Name() As String
                Property LastChange() As DateTime
                Property Size() As Long
                Property CompressedSize() As Long
                Property IsDirectory() As Boolean
            End Interface

            Public Class ZipContentListItem
                Implements ZipFileItem

                Private _CompressedSize As Long
                Public Property CompressedSize() As Long Implements ZipFileItem.CompressedSize
                    Get
                        Return _CompressedSize
                    End Get
                    Set(ByVal Value As Long)
                        _CompressedSize = Value
                    End Set
                End Property

                Private _IsDirectory As Boolean
                Public Property IsDirectory() As Boolean Implements ZipFileItem.IsDirectory
                    Get
                        Return _IsDirectory
                    End Get
                    Set(ByVal Value As Boolean)
                        _IsDirectory = Value
                    End Set
                End Property

                Private _LastChange As Date
                Public Property LastChange() As Date Implements ZipFileItem.LastChange
                    Get
                        Return _LastChange
                    End Get
                    Set(ByVal Value As Date)
                        _LastChange = Value
                    End Set
                End Property

                Private _Name As String
                Public Property Name() As String Implements ZipFileItem.Name
                    Get
                        Return _Name
                    End Get
                    Set(ByVal Value As String)
                        _Name = Value
                    End Set
                End Property

                Private _Size As Long
                Public Property Size() As Long Implements ZipFileItem.Size
                    Get
                        Return _Size
                    End Get
                    Set(ByVal Value As Long)
                        _Size = Value
                    End Set
                End Property
            End Class

            Private Class ZipExtractedItem
                Inherits ZipContentListItem
                Private _ExtractedLocation As String
                Public Property ExtractedLocation() As String
                    Get
                        Return _ExtractedLocation
                    End Get
                    Set(ByVal Value As String)
                        _ExtractedLocation = Value
                    End Set
                End Property
            End Class

            ''' <summary>
            ''' Extract a single file from a ZIP archive
            ''' </summary>
            ''' <param name="zipFile">The ZIP archive</param>
            ''' <param name="extractionPath">The path where the ZIP file shall be extracted to</param>
            ''' <remarks></remarks>
            Public Shared Sub Extract(ByVal zipFile As String, ByVal extractionPath As String)
                If File.Exists(zipFile) = False Then Throw New FileNotFoundException("ZIP archive doesn't exist", zipFile)

                Dim zipFileStream As FileStream = Nothing
                Dim strmZipInputStream As ICSharpCode.SharpZipLib.Zip.ZipInputStream = Nothing
                Dim Result As New ArrayList
                Try
                    zipFileStream = File.OpenRead(zipFile)
                    strmZipInputStream = New ICSharpCode.SharpZipLib.Zip.ZipInputStream(zipFileStream)

                    Dim zipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry
                    zipEntry = strmZipInputStream.GetNextEntry()
                    While IsNothing(zipEntry) = False
                        Dim item As ZipExtractedItem = CType(ConvertZipEntryToZipContentListItem(zipEntry, New ZipExtractedItem), ZipExtractedItem)

                        If item.IsDirectory Then
                            'Directory extraction
                            'Create sub directory if necessary
                            Dim dirName As String
                            dirName = System.IO.Path.Combine(extractionPath, item.Name.Replace("/", System.IO.Path.DirectorySeparatorChar))
                            If System.IO.Directory.Exists(dirName) = False Then
                                System.IO.Directory.CreateDirectory(dirName)
                            End If
                        Else
                            'File extraction
                            Dim StreamWriter As FileStream = Nothing
                            Try
                                Dim ExtractionFilePath As String
                                ExtractionFilePath = System.IO.Path.Combine(extractionPath, item.Name.Replace("/", System.IO.Path.DirectorySeparatorChar))
                                StreamWriter = File.Create(ExtractionFilePath)

                                Dim Size As Integer = 2048
                                Dim data(2048) As Byte
                                While (True)
                                    Size = strmZipInputStream.Read(data, 0, data.Length)
                                    If (Size > 0) Then
                                        StreamWriter.Write(data, 0, Size)
                                    Else
                                        Exit While
                                    End If
                                End While
                            Finally
                                If Not StreamWriter Is Nothing Then StreamWriter.Close()
                            End Try
                        End If
                        zipEntry = strmZipInputStream.GetNextEntry()
                    End While
                Finally
                    If Not strmZipInputStream Is Nothing Then strmZipInputStream.Close()
                    If Not zipFileStream Is Nothing Then zipFileStream.Close()
                End Try
            End Sub
            ''' <summary>
            ''' Extract a single file from a ZIP archive
            ''' </summary>
            ''' <param name="zipFile">The ZIP archive</param>
            ''' <param name="zipItem">A file entry representing the desired file of the ZIP archive</param>
            ''' <param name="fullExtractionFilePath">The full path of the file which shall be written to disc</param>
            ''' <remarks>
            ''' The extraction path must be absolute. There won't be any modifications to reflect a matching folder structure from the ZIP archive with the one on disc
            ''' </remarks>
            Public Shared Sub Extract(ByVal zipFile As String, ByVal zipItem As ZipContentListItem, ByVal fullExtractionFilePath As String)
                If File.Exists(zipFile) = False Then Throw New FileNotFoundException("ZIP archive doesn't exist", zipFile)

                Dim zipFileStream As FileStream = Nothing
                Dim strmZipInputStream As ICSharpCode.SharpZipLib.Zip.ZipInputStream = Nothing
                Dim Result As New ArrayList
                Try
                    zipFileStream = File.OpenRead(zipFile)
                    strmZipInputStream = New ICSharpCode.SharpZipLib.Zip.ZipInputStream(zipFileStream)

                    Dim theEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry
                    theEntry = strmZipInputStream.GetNextEntry()
                    While IsNothing(theEntry) = False
                        Dim item As ZipExtractedItem = CType(ConvertZipEntryToZipContentListItem(theEntry, New ZipExtractedItem), ZipExtractedItem)

                        If item.Name = zipItem.Name AndAlso item.IsDirectory = False Then

                            'Create output directory if required
                            Dim OutputFolder As String = System.IO.Path.GetDirectoryName(fullExtractionFilePath)
                            If System.IO.Directory.Exists(OutputFolder) = False Then
                                System.IO.Directory.CreateDirectory(OutputFolder)
                            End If

                            Dim StreamWriter As FileStream = Nothing
                            Try
                                Dim ExtractionFilePath As String
                                ExtractionFilePath = fullExtractionFilePath
                                StreamWriter = File.Create(ExtractionFilePath)

                                Dim Size As Integer = 2048
                                Dim data(2048) As Byte
                                While (True)
                                    Size = strmZipInputStream.Read(data, 0, data.Length)
                                    If (Size > 0) Then
                                        StreamWriter.Write(data, 0, Size)
                                    Else
                                        Exit While
                                    End If
                                End While
                            Finally
                                If Not StreamWriter Is Nothing Then StreamWriter.Close()
                            End Try
                        End If

                        theEntry = strmZipInputStream.GetNextEntry()
                    End While
                Finally
                    If Not strmZipInputStream Is Nothing Then strmZipInputStream.Close()
                    If Not zipFileStream Is Nothing Then zipFileStream.Close()
                End Try

            End Sub

            Private Shared Function ConvertZipEntryToZipContentListItem(ByVal zipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry, ByVal newItem As ZipContentListItem) As ZipFileItem
                newItem.Name = zipEntry.Name.ToString
                newItem.Size = zipEntry.Size
                newItem.LastChange = zipEntry.DateTime
                newItem.CompressedSize = zipEntry.CompressedSize
                newItem.IsDirectory = zipEntry.IsDirectory
                Return newItem
            End Function
            ''' <summary>
            ''' Load the list of content of a zip archive
            ''' </summary>
            ''' <param name="zipFile">The ZIP archive</param>
            ''' <returns>An array of ZipContentListItems</returns>
            Public Shared Function LoadListOfContent(ByVal zipFile As String) As ZipContentListItem()
                If File.Exists(zipFile) = False Then Throw New FileNotFoundException("ZIP archive doesn't exist", zipFile)
                Dim zipFileStream As FileStream = Nothing
                Dim strmZipInputStream As ICSharpCode.SharpZipLib.Zip.ZipInputStream = Nothing
                Dim Result As New ArrayList
                Try
                    zipFileStream = File.OpenRead(zipFile)
                    strmZipInputStream = New ICSharpCode.SharpZipLib.Zip.ZipInputStream(zipFileStream)
                    Dim objEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry
                    objEntry = strmZipInputStream.GetNextEntry()
                    While IsNothing(objEntry) = False
                        Result.Add(ConvertZipEntryToZipContentListItem(objEntry, New ZipContentListItem))
                        objEntry = strmZipInputStream.GetNextEntry()
                    End While
                Finally
                    If Not strmZipInputStream Is Nothing Then strmZipInputStream.Close()
                    If Not zipFileStream Is Nothing Then zipFileStream.Close()
                End Try
                Return CType(Result.ToArray(GetType(ZipContentListItem)), ZipContentListItem())
            End Function

        End Class
#End Region

#Region "ConnectionString without sensitive data"
        ''' <summary>
        ''' Prepare a connection string for transmission to users without sensitive password information
        ''' </summary>
        ''' <param name="fullConnectionString">The regular ConnectionString</param>
        ''' <returns>The first part of the ConnectionString till the password position</returns>
        ''' <remarks>
        ''' All information after the password position will be removed, too. So, you can hide the user name by positioning it after the password (UID=user;PWD=xxxx vs. PWD=xxxx;UID=user).
        ''' </remarks>
        Public Shared Function ConnectionStringWithoutPasswords(ByVal fullConnectionString As String) As String
            Dim PWDPos As Integer
            PWDPos = InStr(UCase(fullConnectionString), "PWD=")
            If PWDPos > 0 Then
                fullConnectionString = Mid(fullConnectionString, 1, PWDPos + 3) & "..."
            Else
                fullConnectionString = fullConnectionString
            End If
            PWDPos = InStr(UCase(fullConnectionString), "PASSWORD=")
            If PWDPos > 0 Then
                fullConnectionString = Mid(fullConnectionString, 1, PWDPos + 8) & "..."
            Else
                fullConnectionString = fullConnectionString
            End If
            Return fullConnectionString
        End Function
#End Region

#Region "Mono framework detection"
        '''<summary>
        ''' Checks whether the framework used is Mono.
        '''</summary>
        '''<returns>True (using Mono) or false (not using Mono).</returns> 
        Public Shared Function IsMonoFramework() As Boolean
            Return Not System.Type.GetType("Mono.Runtime", False) Is Nothing
        End Function
#End Region

    End Class

#Region "Compatibility module"
    <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
    Public Module UtilsCompatibility
        ''' <summary>
        '''     Redirect to another url by using regular html forms to post data
        ''' </summary>
        ''' <param name="url"></param>
        ''' <param name="postData"></param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection)
            Utils.RedirectWithPostData(context, url, postData)
        End Sub

        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal httpMethod As String)
            Utils.RedirectWithPostData(context, url, postData, httpMethod)
        End Sub

        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Sub RedirectWithPostData(ByVal context As System.Web.HttpContext, ByVal url As String, ByVal postData As Collections.Specialized.NameValueCollection, ByVal httpMethod As String, ByVal additionalFormAttributes As String)
            Utils.RedirectWithPostData(context, url, postData, httpMethod, additionalFormAttributes)
        End Sub


#Region "Network host information"
        ''' <summary>
        '''     Get the first value of the IP address list or the workstation name if there are no IP addresses
        ''' </summary>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function GetWorkstationID() As String

            Dim host As System.Net.IPHostEntry = System.Net.Dns.Resolve(System.Net.Dns.GetHostName)

            ' Loop on the AddressList
            Dim curAdd As System.Net.IPAddress
            For Each curAdd In host.AddressList

                ' Display the server IP address in the standard format. In 
                ' IPv4 the format will be dotted-quad notation, in IPv6 it will be
                ' in in colon-hexadecimal notation.
                Return curAdd.ToString()

            Next

            'If not already returned with a found IP, return with the host name
            Return System.Net.Dns.GetHostName

        End Function
        ''' <summary>
        '''     Is the remote client connecting from a private network?
        ''' </summary>
        ''' <returns>True for private networks, false for localhost and public networks</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function IsRemoteClientFromLanOrPrivateNetwork() As Boolean

            If System.Web.HttpContext.Current Is Nothing Then
                Throw New Exception("Not running in a web application")
            End If

            Return Utils.IsHostFromLanOrPrivateNetwork(System.Web.HttpContext.Current.Request.UserHostAddress)

        End Function
        ''' <summary>
        '''     Is the IPv4 address a host of a specified address range?
        ''' </summary>
        ''' <param name="IPv4Address">An IPv4 host address</param>
        ''' <param name="AddressRange">An array of strings defining address ranges, e. g. 192.168.*</param>
        ''' <remarks>
        '''     This method compares the string content and not the byte content, that's why you should not forget the leading dot in front of the star in your IP address range.
        ''' </remarks>
        ''' <example>
        '''     <para>These address ranges would be fine:</para>
        '''     <list>
        '''         <item>10.*</item>
        '''         <item>192.168.*</item>
        '''     </list>
        '''     <para>Those address ranges could get misinterpreted:</para>
        '''     <list>
        '''         <item>10* <em>(would also accept 101.*, 102.*, etc. and invalid IP addresses like 10:ZZ:*)</em></item>
        '''         <item>192.168.*.* <em>(all letters after the first star will be ignored)</em></item>
        '''     </list>
        ''' </example>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function IsHostOfAddressRange(ByVal IPv4Address As String, ByVal AddressRange As String()) As Boolean

            If IPv4Address = "" Then
                Return False
            ElseIf InStr(IPv4Address, "*") = 1 Then
                Return False 'Invalid filter, return false even if it would be true, regulary
            ElseIf AddressRange Is Nothing OrElse AddressRange.Length = 0 Then
                Return False
            Else
                'First look says: it might be possible; go on
                For Each RangeElement As String In AddressRange
                    Dim RangeStartPos As Integer = InStr(RangeElement, "*") - 1
                    Dim RangeStart As String
                    If RangeStartPos > 0 Then
                        'Found a "*", do a like matching
                        RangeStart = Mid(RangeElement, 1, RangeStartPos)
                        If IPv4Address.ToUpper(System.Globalization.CultureInfo.InvariantCulture).StartsWith(RangeStart.ToUpper(System.Globalization.CultureInfo.InvariantCulture)) Then
                            Return True
                        End If
                    Else
                        'No jokers, exact match required
                        RangeStart = RangeElement
                        If IPv4Address.ToUpper(System.Globalization.CultureInfo.InvariantCulture) = (RangeStart.ToUpper(System.Globalization.CultureInfo.InvariantCulture)) Then
                            Return True
                        End If
                    End If
                Next
            End If

        End Function
        ''' <summary>
        '''     Is the host address a private network node (defined by RFC-1597)?
        ''' </summary>
        ''' <param name="IPAddress">The IP address to validate</param>
        ''' <returns>True for private or LAN networks, otherwise false</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function IsHostFromLanOrPrivateNetwork(ByVal IPAddress As String) As Boolean

            Dim IP As System.Net.IPAddress
            IP = System.Net.IPAddress.Parse(IPAddress)

            Console.WriteLine(IP.AddressFamily.ToString)

            If IP.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                'RFC 1918
                'private ranges are
                '10.x.x.x
                '172.16.x.x - 172.31.x.x
                '192.168.x.x
                If Left(IPAddress, 3) = "10." OrElse Left(IPAddress, 8) = "192.168." Then
                    Return True
                ElseIf Left(IPAddress, 4) = "172." Then
                    Select Case Mid(IPAddress, 5, 3)
                        Case "16.", "17.", "18.", "19.", "20.", "21.", "22.", "23.", "24.", "25.", "26.", "27.", "28.", "29.", "30.", "31."
                            Return True
                        Case Else
                            Return False
                    End Select
                Else
                    Return False
                End If
            ElseIf IP.AddressFamily = System.Net.Sockets.AddressFamily.InterNetworkV6 Then
                'http://www.zytrax.com/tech/protocols/ipv6.html#types
                'Scope is local LAN only; can't be routed outside of local LAN
                'private ranges (link local) are beginning with 
                '- fe8x (most commonly used)
                '- fe9x
                '- feax
                '- febx
                'Scope is local private network; can't be routed outside over Internet
                'private ranges (site local) are beginning with 
                '- fecx (most commonly used)
                '- fedx
                '- feex
                '- fefx
                Dim IPBytes As Byte() = IP.GetAddressBytes
                If IPBytes(0) = 254 Then 'FE 
                    If IPBytes(1) >= 192 Then '(C0-FF) 
                        'private network
                        Return True
                    ElseIf IPBytes(1) >= 128 Then '(80-BF) 
                        'LAN
                        Return True
                    Else
                        'Global unicast or others
                        Return False '(00-7F)
                    End If
                Else
                    Return False
                End If
            Else
                'network type not detectable            
                Return False
            End If

        End Function
        ''' <summary>
        '''     Is the host a localhost?
        ''' </summary>
        ''' <param name="IPAddress">The IP address to validate</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function IsLoopBackDevice(ByVal IPAddress As String) As Boolean
            Return System.Net.IPAddress.IsLoopback(System.Net.IPAddress.Parse(IPAddress))
        End Function

#End Region

#Region "Nz"
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function IfNull(ByVal CheckValueIfDBNull As Object, Optional ByVal ReplaceWithThis As Object = Nothing) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns null (Nothing in VisualBasic) in that case
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return Nothing
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Object) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Integer) As Integer
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Integer)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Long) As Long
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Long)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Boolean) As Boolean
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Boolean)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As DateTime) As DateTime
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, DateTime)
            End If
        End Function
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As String) As String
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, String)
            End If
        End Function
#End Region

#Region "Type conversions"
        ''' <summary>
        '''     Return the string which is not nothing or else String.Empty
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function StringNotEmptyOrNothing(ByVal value As String) As String
            If value = Nothing Then
                Return Nothing
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not nothing or else String.Empty
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function StringNotNothingOrEmpty(ByVal value As String) As String
            If value Is Nothing Then
                Return String.Empty
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not empty or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function StringNotEmptyOrDBNull(ByVal value As String) As Object
            If value = Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the object which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function ObjectNotNothingOrEmptyString(ByVal value As Object) As Object
            If value Is Nothing Then
                Return String.Empty
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the object which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function ObjectNotNothingOrDBNull(ByVal value As Object) As Object
            If value Is Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the object which is not an empty string or otherwise return Nothing
        ''' </summary>
        ''' <param name="value">The object to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function ObjectNotEmptyStringOrNothing(ByVal value As Object) As Object
            If value.GetType Is GetType(String) AndAlso CType(value, String) = "" Then
                Return Nothing
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Return the string which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function StringNotNothingOrDBNull(ByVal value As String) As Object
            If value Is Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted long value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCLng(ByVal Expression As Object) As Long
            Return Utils.TryCLng(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted long value or the alternative value if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCLng(ByVal Expression As Object, ByVal AlternativeValue As Long) As Long
            Try
                Return CLng(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a integer value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted integer value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCInt(ByVal Expression As Object) As Integer
            Return Utils.TryCInt(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a integer value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted integer value or the alternative value if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCInt(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Integer
            Try
                Return CInt(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a double value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted double value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCDbl(ByVal Expression As Object) As Double
            Return Utils.TryCDbl(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a double value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted double value or the alternative value if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCDbl(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Double
            Try
                Return CDbl(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a decimal value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted decimal value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCDec(ByVal Expression As Object) As Decimal
            Return Utils.TryCDec(Expression, Nothing)
        End Function
        ''' <summary>
        '''     Tries to convert the expression into a decimal value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted decimal value or the alternative value if the conversion was unsuccessfull</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function TryCDec(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Decimal
            Try
                Return CDec(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function

#End Region

#Region "String manipulation and HTML conversions"
        ''' <summary>
        '''     Get the complete query string of the current request in a form usable for recreating this query string for a following request
        ''' </summary>
        ''' <param name="removeParameters">Remove all values with this name form the query string</param>
        ''' <returns>A new string with all query string information without the starting questionmark character</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function QueryStringWithoutSpecifiedParameters(ByVal removeParameters As String()) As String
            Return Utils.NameValueCollectionWithoutSpecifiedKeys(System.Web.HttpContext.Current.Request.QueryString, removeParameters)
        End Function
        ''' <summary>
        '''     Get the complete string of a collection in a form usable for recreating a query string for a following request
        ''' </summary>
        ''' <param name="collection">A NameValueCollection, e. g. Request.QueryString</param>
        ''' <param name="removeKeys">Names of keys which shall not be in the output</param>
        ''' <returns>A string of the collection data which can be appended to any URL (with url encoding)</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function NameValueCollectionWithoutSpecifiedKeys(ByVal collection As System.Collections.Specialized.NameValueCollection, ByVal removeKeys As String()) As String
            Dim RedirectionParams As String = ""
            For Each ParamItem As String In collection
                Dim RemoveThisParameter As Boolean = False
                If ParamItem = "" Then
                    RemoveThisParameter = True
                ElseIf Not removeKeys Is Nothing Then
                    Dim MyParamItem As String = ParamItem.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    For Each My2RemoveParameter As String In removeKeys
                        If MyParamItem = My2RemoveParameter.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                            RemoveThisParameter = True
                        End If
                    Next
                End If
                If RemoveThisParameter = False Then
                    'do not collect empty items (ex. the item between "?" and "&" in "index.aspx?&Lang=2")
                    RedirectionParams = RedirectionParams & "&" & System.Web.HttpUtility.UrlEncode(ParamItem) & "=" & System.Web.HttpUtility.UrlEncode(collection(ParamItem))
                End If
            Next
            Return Mid(RedirectionParams, 2)
        End Function
        ''' <summary>
        '''     Split a string by a separator if there is not a special leading character
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="separator"></param>
        ''' <param name="exceptLeadingCharacter"></param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function SplitString(ByVal text As String, ByVal separator As Char, ByVal exceptLeadingCharacter As Char) As String()
            Dim Result As New ArrayList
            'Go through every char of the string
            For MyCounter As Integer = 0 To text.Length - 1
                Dim SplitHere As Boolean
                Dim StartPosition As Integer
                'Find split points
                If text.Chars(MyCounter) = separator Then
                    If MyCounter = 0 Then
                        SplitHere = True
                    ElseIf text.Chars(MyCounter - 1) <> exceptLeadingCharacter Then
                        SplitHere = True
                    End If
                End If
                'Add partial string
                If SplitHere OrElse MyCounter = text.Length - 1 Then
                    Result.Add(text.Substring(StartPosition, CType(IIf(SplitHere = False, 1, 0), Integer) + MyCounter - StartPosition)) 'If Split=False then this if-block was caused by the end of the text; in this case we have to simulate to be after the last character position to ensure correct extraction of the last text element
                    SplitHere = False 'Reset status
                    StartPosition = MyCounter + 1 'Next string starts after the current char
                End If
            Next
            Return CType(Result.ToArray(GetType(String)), String())
        End Function
        ''' <summary>
        '''     Converts all line breaks into HTML line breaks (&quot;&lt;br&gt;&quot;)
        ''' </summary>
        ''' <param name="Text"></param>
        ''' <remarks>
        '''     Supported line breaks are linebreaks of Windows, MacOS as well as Linux/Unix.
        ''' </remarks>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function HTMLEncodeLineBreaks(ByVal Text As String) As String
            Return Text.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Cr, "<br>").Replace(ControlChars.Lf, "<br>")
        End Function
        ''' <summary>
        '''     Search for a string in another string and return the number of matches
        ''' </summary>
        ''' <param name="source">The string where to search in</param>
        ''' <param name="searchFor">The searched string (binary comparison)</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function CountOfOccurances(ByVal source As String, ByVal searchFor As String) As Integer
            Return Utils.CountOfOccurances(source, searchFor, CompareMethod.Binary)
        End Function
        ''' <summary>
        '''     Search for a string in another string and return the number of matches
        ''' </summary>
        ''' <param name="source">The string where to search in</param>
        ''' <param name="searchFor">The searched string</param>
        ''' <param name="compareMethod">Binary or text search</param>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function CountOfOccurances(ByVal source As String, ByVal searchFor As String, ByVal compareMethod As Microsoft.VisualBasic.CompareMethod) As Integer

            If searchFor = "" Then
                Throw New ArgumentNullException("searchFor")
            End If

            Dim Result As Integer

            'Initial values
            Dim Position As Integer = 0
            Dim NextMatch As Integer = InStr(Position + 1, source, searchFor, compareMethod)

            'and now loop through the source string
            While NextMatch > 0
                Result += 1
                If NextMatch <= Position Then
                    Throw New Exception("NextMatch=" & NextMatch & ", OldPosition=" & Position & ", searchFor=""" & searchFor & """, source=""" & source & """")
                End If
                Position = NextMatch

                'Search for next value
                NextMatch = InStr(Position + 1, source, searchFor, compareMethod)
            End While

            Return Result

        End Function
        ''' <summary>
        '''     Replaces placeholders in a string by defined values
        ''' </summary>
        ''' <param name="message">The string with the placeholders</param>
        ''' <param name="values">One or more values which should be used for replacement</param>
        ''' <returns>The new resulting string</returns>
        ''' <remarks>
        '''     <para>Supported special character combinations are <code>\t</code>, <code>\r</code>, <code>\n</code>, <code>\\</code>, <code>\[</code></para>
        '''     <para>Supported placeholders are <code>[*]</code>, <code>[n:1..9]</code></para>
        ''' </remarks>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function sprintf(ByVal message As String, ByVal ParamArray values() As Object) As String
            Const errpfNoClosingBracket As Integer = vbObjectError + 1
            Const errpfMissingValue As Integer = vbObjectError + 2
            '*** Special chars ***
            '   \t = TAB
            '   \r = CR
            '   \n = CRLF
            '   \[ = [
            '   \\ = \
            '*** Placeholder ***
            '   []		= value of parameter list will be skipped
            '   [*]		= value will be inserted without any special format
            '   [###]	= "###" stands for a format string (further details in your SDK help for command "Format()")
            '   [n:...]	= n as 1..9: uses the n-th parameter; the internal parameter index will not be increased!
            '             There can be a format string behind the ":"

            Dim iv%, orig_iv%, iob%, icb%
            Dim formatString As String

            If message Is Nothing Then message = ""
            message = message.Replace("\\", "[\]")
            message = message.Replace("\t", vbTab)
            message = message.Replace("\r", vbCr)
            message = message.Replace("\n", vbCrLf)
            message = message.Replace("\[", "[(]")

            iob = 1
            Do
                iob = InStr(iob, message, "[")
                If iob = 0 Then Exit Do

                icb = InStr(iob + 1, message, "]")
                If icb = 0 Then
                    Err.Raise(errpfNoClosingBracket, "printf", "Missing ']' after '[' at position " & iob & "!")
                End If

                formatString = Mid$(message, iob + 1, icb - iob - 1)

                If InStr("123456789", Mid$(formatString, 1, 1)) > 0 And Mid$(formatString, 2, 1) = ":" Then
                    orig_iv = iv

                    iv = CInt(Mid$(formatString, 1, 1)) - 1
                    If iv > UBound(values) Then iv = UBound(values)

                    formatString = Mid$(formatString, 3)
                Else
                    orig_iv = -1
                End If


                Select Case formatString
                    Case ""
                        formatString = ""
                    Case "("
                        formatString = "["
                        iv = iv - 1
                    Case "\"
                        formatString = "\"
                        iv = iv - 1
                    Case "*"
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = CType(values(iv), String)
                    Case Else 'with user specified format string
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = CType(values(iv), String) 'Format(values(iv), formatString)
                End Select

                message = Left$(message, iob - 1) & formatString & Mid$(message, icb + 1)
                iob = iob + Len(formatString) + CType(IIf(orig_iv >= 0, 2, 0), Integer)

                If orig_iv >= 0 Then
                    iv = orig_iv
                Else
                    iv = iv + 1
                End If
            Loop

            sprintf = message

        End Function
        ''' <summary>
        '''     Join all items of a NameValueCollection (for example Request.QueryString) to a simple string
        ''' </summary>
        ''' <param name="NameValueCollectionToString">A collection like Request.Form or Request.QueryString</param>
        ''' <param name="BeginningOfItem">A string in front of a key</param>
        ''' <param name="KeyValueSeparator">The string between key and value</param>
        ''' <param name="EndOfItem">The string to be placed at the end of a value</param>
        ''' <returns>A string containing all elements of the collection</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function JoinNameValueCollectionToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection, ByVal BeginningOfItem As String, ByVal KeyValueSeparator As String, ByVal EndOfItem As String) As String
            Dim Result As String = Nothing
            For Each ParamItem As String In NameValueCollectionToString
                Result &= BeginningOfItem & ParamItem & KeyValueSeparator & NameValueCollectionToString(ParamItem) & EndOfItem
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Join all items of a NameValueCollection (for example Request.QueryString) to a simple string
        ''' </summary>
        ''' <param name="NameValueCollectionToString">A collection like Request.Form or Request.QueryString</param>
        ''' <returns>A string containing all elements of the collection which can be appended to any internet address</returns>
        ''' <remarks>
        '''     If you need to read the values directly from the returned string, pay attention that all names and values might be UrlEncoded and you have to decode them, first.
        ''' </remarks>
        ''' <see also="FillNameValueCollectionWith" />
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function JoinNameValueCollectionWithUrlEncodingToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection) As String
            Dim Result As String = Nothing
            For Each ParamItem As String In NameValueCollectionToString
                If Result <> Nothing Then
                    Result &= "&"
                End If
                Result &= System.Web.HttpUtility.UrlEncode(ParamItem) & "=" & System.Web.HttpUtility.UrlEncode(NameValueCollectionToString(ParamItem))
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Restore a NameValueCollection's content which has been previously converted to a string
        ''' </summary>
        ''' <param name="nameValueCollection">An already existing NameValueCollection which shall receive the new values</param>
        ''' <param name="nameValueCollectionWithUrlEncoding">A string containing the UrlEncoded writing style of a NameValueCollection</param>
        ''' <remarks>
        '''     Please note: existing values in the collection won't be appended, they'll be overridden
        ''' </remarks>
        ''' <see also="JoinNameValueCollectionWithUrlEncodingToString" />
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Sub ReFillNameValueCollection(ByVal nameValueCollection As System.Collections.Specialized.NameValueCollection, ByVal nameValueCollectionWithUrlEncoding As String)

            If nameValueCollection Is Nothing Then
                Throw New ArgumentNullException("nameValueCollection")
            End If
            If nameValueCollectionWithUrlEncoding = Nothing Then
                'Nothing to do
                Return
            End If

            'Split at "&"
            Dim parameters As String() = nameValueCollectionWithUrlEncoding.Split(New Char() {"&"c})
            For MyCounter As Integer = 0 To parameters.Length - 1
                Dim KeyValuePair As String() = parameters(MyCounter).Split(New Char() {"="c})
                If KeyValuePair.Length = 0 Then
                    'empty - nothing to do
                ElseIf KeyValuePair.Length = 1 Then
                    'key name only
                    nameValueCollection(System.Web.HttpUtility.UrlDecode(KeyValuePair(0))) = Nothing
                ElseIf KeyValuePair.Length = 2 Then
                    'key/value pair
                    nameValueCollection(System.Web.HttpUtility.UrlDecode(KeyValuePair(0))) = System.Web.HttpUtility.UrlDecode(KeyValuePair(1))
                Else
                    'invalid data
                    Throw New ArgumentException("Invalid data - more than one equals signs (""="") found", "nameValueCollectionWithUrlEncoding")
                End If
            Next

        End Sub
        ''' <summary>
        '''     Converts HTML messages to simple text
        ''' </summary>
        ''' <param name="HTML">A string with HTML code</param>
        ''' <returns>The rendered output as plain text</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function ConvertHTMLToText(ByVal HTML As String) As String
            'TODO: 1. remove of all other HTML tags
            '      2. search case insensitive
            '      3. truncate content between <head> and </head>, <script> and </script>, <!-- and -->
            '      Please note: there is already a camm HTML file filter which might get reused here
            Dim Result As String = HTML
            Result = Result.Replace("<br>", vbNewLine)
            Result = Result.Replace("<p>", vbNewLine & vbNewLine)
            Result = Result.Replace("</p>", "")
            Result = Result.Replace("<li>", "- ")
            Result = Result.Replace("</li>", vbNewLine)
            Result = Result.Replace("<ul>", "    ")
            Result = Result.Replace("</ul>", vbNewLine)
            Result = Result.Replace("<b>", "")
            Result = Result.Replace("</b>", "")
            Result = Result.Replace("<strong>", "")
            Result = Result.Replace("</strong>", "")
            Result = Result.Replace("<small>", "")
            Result = Result.Replace("</small>", "")
            Result = Result.Replace("<html>", "")
            Result = Result.Replace("</html>", "")
            Result = Result.Replace("<body>", "")
            Result = Result.Replace("</body>", "")
            Result = Result.Replace("<font face=""Arial"">", "")
            Result = Result.Replace("<font face=""Arial"" color=""red"">", "")
            Result = Result.Replace("</font>", "")
            Result = Result.Replace("<hl>", "-------------------------------------------------------")
            Return Result
        End Function
#End Region

#Region "LinkHighlighting"
        ''' <summary>
        '''     Converts URLs and e-mail addresses in a string into HTML hyperlinks
        ''' </summary>
        ''' <param name="Text">The standard text without any HTML</param>
        ''' <returns>HTML with hyperlinks</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function HighlightLinksInMessage(ByVal Text As String) As String
            Return Utils.HighlightLinksInMessage(Text)
        End Function
#End Region

#Region "Paths and URIs"
        ''' <summary>
        '''     Check validity of an email address
        ''' </summary>
        ''' <param name="emailAddress">email address to be validated</param>
        ''' <returns>True if email address is syntactically valid else false</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function ValidateEmailAddress(ByVal emailAddress As String) As Boolean
            Return Utils.ValidateEmailAddress(emailAddress)
        End Function
        ''' <summary>
        '''     Check validity of an URL
        ''' </summary>
        ''' <param name="url">URL to be validated</param>
        ''' <returns>True if URL is syntactically valid else false</returns>
        <Obsolete("Use static methods in Utils instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function ValidateInternetUrl(ByVal url As String) As Boolean
            Return Utils.ValidateInternetUrl(url)
        End Function
#End Region

    End Module
#End Region

#Region "StringArray conversion"
    ''' <summary>
    '''     Allow a string array property to be filled by a comma separated string
    ''' </summary>
    Friend Class StringArrayConverter
        Inherits System.ComponentModel.CollectionConverter

        Public Sub New()
        End Sub

        Public Overloads Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            If (sourceType Is GetType(String)) Then
                Return True
            End If
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overloads Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            If (destinationType Is GetType(String())) Then
                Return True
            End If
            Return MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overloads Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal sourceObj As Object) As Object
            If TypeOf sourceObj Is String Then
                Dim chArray1 As Char() = New Char() {","c}
                Return CType(sourceObj, String).Split(chArray1)
            End If
            Return MyBase.ConvertFrom(context, culture, sourceObj)
        End Function

        Public Overloads Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal destinationObj As Object, ByVal destinationType As Type) As Object
            If TypeOf destinationObj Is String() Then
                Dim text1 As String = String.Join(",", CType(destinationObj, String()))
                If (destinationType Is GetType(System.ComponentModel.Design.Serialization.InstanceDescriptor)) Then
                    Dim typeArray1 As Type() = New Type() {GetType(String)}
                    Dim info1 As System.Reflection.ConstructorInfo = GetType(String).GetConstructor(typeArray1)
                    If (info1 Is Nothing) Then
                        GoTo Label_007B
                    End If
                    Dim objArray1 As Object() = New Object() {text1}
                    Return New System.ComponentModel.Design.Serialization.InstanceDescriptor(info1, objArray1)
                End If
                If (destinationType Is GetType(String)) Then
                    Return text1
                End If
            End If
Label_007B:
            Return MyBase.ConvertTo(context, culture, destinationObj, destinationType)
        End Function
    End Class

    ''' <summary>
    '''     Allow an integer array property to be filled by a comma separated string
    ''' </summary>
    Friend Class IntegerArrayConverter
        Inherits System.ComponentModel.CollectionConverter

        Public Sub New()
        End Sub

        Public Overloads Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            If (sourceType Is GetType(String)) Then
                Return True
            End If
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overloads Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            If (destinationType Is GetType(Integer())) Then
                Return True
            End If
            Return MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overloads Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal sourceObj As Object) As Object
            If TypeOf sourceObj Is String Then
                Dim chArray1 As Char() = New Char() {","c}
                Return ConvertStringArrayToIntegerArray(CType(sourceObj, String).Split(chArray1))
            End If
            Return MyBase.ConvertFrom(context, culture, sourceObj)
        End Function

        Private Function ConvertStringArrayToIntegerArray(values As String()) As Integer()
            Dim Result As New ArrayList
            For Each value As String In values
                If value = "" Then
                    Result.Add(0)
                Else
                    Result.Add(Integer.Parse(value))
                End If
            Next
            Return CType(Result.ToArray(GetType(Integer)), Integer())
        End Function

        Public Overloads Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal destinationObj As Object, ByVal destinationType As Type) As Object
            If TypeOf destinationObj Is Integer() Then
                Dim text1 As String = String.Join(",", CType(destinationObj, String()))
                If (destinationType Is GetType(System.ComponentModel.Design.Serialization.InstanceDescriptor)) Then
                    Dim typeArray1 As Type() = New Type() {GetType(Integer)}
                    Dim info1 As System.Reflection.ConstructorInfo = GetType(Integer).GetConstructor(typeArray1)
                    If (info1 Is Nothing) Then
                        GoTo Label_007B
                    End If
                    Dim objArray1 As Object() = New Object() {text1}
                    Return New System.ComponentModel.Design.Serialization.InstanceDescriptor(info1, objArray1)
                End If
                If (destinationType Is GetType(Integer)) Then
                    Return text1
                End If
            End If
Label_007B:
            Return MyBase.ConvertTo(context, culture, destinationObj, destinationType)
        End Function
    End Class

#End Region

#Region "DummyValidatorForAppearanceInValidationSummary"
    ''' <summary>
    ''' The validation summary control works by iterating over the Page.Validators
    ''' collection and displaying the ErrorMessage property of each validator
    ''' that return false for the IsValid() property.  This class will act 
    ''' like all the other validators except it always is invalid and thus the 
    ''' ErrorMessage property will always be displayed.
    ''' </summary>
    Friend Class DummyValidatorForAppearanceInValidationSummary
        Implements Web.UI.IValidator

        Private errorMsg As String

        Public Sub New(ByVal message As String)
            errorMsg = message
        End Sub

        Public Overridable Property ErrorMessage() As String Implements Web.UI.IValidator.ErrorMessage
            Get
                Return errorMsg
            End Get
            Set(ByVal value As String)
                errorMsg = value
            End Set
        End Property

        Public Property IsValid() As Boolean Implements Web.UI.IValidator.IsValid
            Get
                Return False
            End Get
            Set(ByVal value As Boolean)
            End Set
        End Property

        Public Sub Validate() Implements Web.UI.IValidator.Validate
        End Sub
    End Class

#End Region

End Namespace