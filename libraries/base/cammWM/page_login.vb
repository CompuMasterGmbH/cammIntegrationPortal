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

Imports System.Data
Imports System.Data.SqlClient
Imports System.Web

Namespace CompuMaster.camm.WebManager.Pages.Login

    ''' <summary>
    '''     Logon distribution
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class LDirect
        Inherits Page

        Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            '***************************************************
            '*** Autorisiert den aktuellen Benutzer an       ***
            '*** Server und ScriptEngine                     ***
            '***************************************************
            Dim RedirDetails As New System.Collections.Specialized.NameValueCollection
            RedirDetails("Enabled logging for ldirect") = "True"
            Dim BufferedcammWebManagerIsLoggedOn As Boolean = cammWebManager.IsLoggedOn
            RedirDetails("cammWebManager.IsLoggedOn") = BufferedcammWebManagerIsLoggedOn.ToString

            Dim UserLoginName2Set As String = Nothing
            Dim AddLangID2RedirectAddr As String
            AddLangID2RedirectAddr = "&lang=" & Request.QueryString("lang")
            If Request.QueryString("lang") <> "" Then
                Session("CurLanguage") = CLng(Request.QueryString("Lang"))
                Response.Cookies("CWM").Expires = Now.AddDays(183) 'Cookie expires after 6 months
                If UCase(Request.ServerVariables("HTTPS")) = "ON" Then
                    Response.Cookies("CWM").Secure = True
                Else
                    Response.Cookies("CWM").Secure = False
                End If
                Response.Cookies("CWM")("Lang") = CType(Session("CurLanguage"), String)
            End If

            Application.Lock()
            Try
                If Request.QueryString("User") <> "" Then

                    Dim User_Auth_Validation_Cmd As New SqlCommand

                    'Get parameter value and append parameter
                    With User_Auth_Validation_Cmd
                        .Connection = New SqlConnection(cammWebManager.ConnectionString)
                        .CommandText = "Public_ValidateGUIDLogin"
                        .CommandType = CommandType.StoredProcedure

                        .Parameters.Add("@Username", SqlDbType.NVarChar).Value = Request.QueryString("User").ToString
                        .Parameters.Add("@GUID", SqlDbType.Int).Value = Request.QueryString("GUID").ToString
                        .Parameters.Add("@ServerIP", SqlDbType.VarChar).Value = cammWebManager.CurrentServerIdentString
                        .Parameters.Add("@RemoteIP", SqlDbType.VarChar).Value = cammWebManager.CurrentRemoteClientAddress
                        .Parameters.Add("@ScriptEngine_ID", SqlDbType.Int).Value = CompuMaster.camm.WebManager.WMSystem.ScriptEngines.ASPNet
                        .Parameters.Add("@ScriptEngine_SessionID", SqlDbType.VarChar, 512).Value = Me.cammWebManager.CurrentScriptEngineSessionID

                    End With

                    UserLoginName2Set = WebManager.Utils.StringNotNothingOrEmpty(WebManager.Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(User_Auth_Validation_Cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), ""))
                    RedirDetails("UserLoginName2Set") = UserLoginName2Set

                End If
            Catch ex As Exception
                cammWebManager.Log.RuntimeException(ex, False, False)
                UserLoginName2Set = ""
            Finally
                Application.UnLock()
            End Try

            Session.Timeout = 240 '4 h
            If Not BufferedcammWebManagerIsLoggedOn Then
                'Keine Änderung erlauben durch simples überschreiben eines vorhandenen 
                'Wertes durch den mitgeteilten, jedoch Anmeldung eines Benutzers ausführen, _
                'wenn bisher noch niemand in dieser Browsersession angemeldet ist.
                If UserLoginName2Set <> "" Then
                    cammWebManager.SetUserLoginName(UserLoginName2Set)
                    RedirDetails("SetUserLoginName") = UserLoginName2Set
                End If
            ElseIf BufferedcammWebManagerIsLoggedOn And Request.QueryString("User") = "" Then
                'Abmeldung
                If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation AndAlso Not HttpContext.Current Is Nothing Then
                    Dim DebugInfo As String = "Session will be abandoned because current loginname is not empty (""" & cammWebManager.CurrentUserLoginName & """) but doesn't match with user in URL"
                    HttpContext.Current.Response.Write(DebugInfo)
                    HttpContext.Current.Response.Write("<script language=""javascript""><!--" & vbNewLine & "confirm('" & DebugInfo.Replace("'", "´") & "');" & vbNewLine & "//--></script>")
                End If
                cammWebManager.ResetUserLoginName()
                RedirDetails("ResetUserLoginName") = Me.cammWebManager.CurrentUserLoginName
                Session.Abandon()
            End If

            'Nächstes Login
            Dim System_RedirectURI As String = ""
            Dim MyRedirectTo As String 'The address when the ldirect-refresh-cycle has been completed
            MyRedirectTo = Request.QueryString("redirectto")
            If cammWebManager.IsLoggedOn AndAlso Request.QueryString("LogonID") = "" Then 'Request.QueryString("LogonID") <> "" AND IsLoggedOn might happen when ASP session can't be restored while ASP.NET session remains available
                System_RedirectURI = cammWebManager.System_GetNextLogonURI(cammWebManager.CurrentUserLoginName)
            Else
                System_RedirectURI = cammWebManager.System_GetNextLogonURIOfUserAnonymous()
            End If

            Dim MyResponseRedirect As String = ""
            If System_RedirectURI = "" Then
                'No LDirect
                If MyRedirectTo <> "" Then
                    'e.g. frame_sub.aspx
                    MyResponseRedirect = MyRedirectTo
                Else
                    'MyResponseRedirect = cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL
                    MyResponseRedirect = cammWebManager.CalculateUrl(cammWebManager.CurrentServerInfo.ParentServerGroup.MasterServer.ID, CompuMaster.camm.WebManager.WMSystem.ScriptEngines.ASPNet, "/sysdata/login/loginprocedurefinished.aspx")
                End If
            Else
                'Next LDirect
                MyResponseRedirect = System_RedirectURI & "&redirectto=" & System.Web.HttpUtility.UrlEncode(MyRedirectTo) & AddLangID2RedirectAddr
            End If
            cammWebManager.RedirectTo(MyResponseRedirect, "This redirect is caused by ldirect's script engine hand shake", RedirDetails)

        End Sub

    End Class

    ''' <summary>
    '''     After a successfull logon/logoff, the user must be redirected to either the originally demanded URL or to the normal start page.
    ''' </summary>
    ''' <remarks>
    '''     When the user went to a document which required a logon, the document's address might be saved in the user session for later redirecting again. When this Referer has been saved, the user shall be redirected back to this URL again, otherwise he shall be redirected to the address of the regular start page.
    '''     Please note: this page executes always on the master server of the server group which is responsable for starting and finishing the logon procedure.
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class LoginProcedureFinished
        Inherits Page
        ''' <summary>
        '''     Redirect either to the referer or the start address
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Request.QueryString("redirectto") <> Nothing Then
                'Referer has already been requested in the previous request
                'Now the page has been reloaded in the top frame and we only have to redirect now to the final address
                cammWebManager.RedirectTo(Request.QueryString("redirectto"), "Redirect initiated by LoginProcedureFinished", Nothing)
            Else
                'Request the referer
                Dim NewUrl As String
                NewUrl = Referer
                If NewUrl <> Nothing Then
                    RenderPage(NewUrl)
                Else
                    RenderPage(StartPageUrl)
                End If
            End If
        End Sub

        Protected Overridable Sub RenderPage(ByVal redirectToUrl As String)

            Response.Clear()
            Response.Write("<html>" & vbNewLine)
            Response.Write("<body>" & vbNewLine)
            Response.Write("<script language=""javascript"">" & vbNewLine)
            Response.Write("<!--" & vbNewLine)
            Response.Write("// Reload current page in the _TOP frame if we're in a frame, currently" & vbNewLine)
            Response.Write("if (window.parent.length != 0)" & vbNewLine)
            Response.Write("{" & vbNewLine)
            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                Response.Write("   confirm('Reload in top frame: ' + window.location + '?redirectto=" & Server.UrlEncode(redirectToUrl).Replace("\", "\\").Replace("'", "\'") & "');" & vbNewLine)
            End If
            Response.Write("   window.top.location = window.location + '?redirectto=" & Server.UrlEncode(redirectToUrl).Replace("\", "\\").Replace("'", "\'") & "';" & vbNewLine)
            Response.Write("}" & vbNewLine)
            Response.Write("else" & vbNewLine)
            Response.Write("{" & vbNewLine)
            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                Response.Write("   if (confirm('Redirect to destination URL: " & redirectToUrl.Replace("\", "\\").Replace("'", "\'") & "'))" & vbNewLine)
                Response.Write("   {" & vbNewLine)
            End If
            Response.Write("   window.location = '" & redirectToUrl.Replace("\", "\\").Replace("'", "\'") & "';" & vbNewLine)
            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                Response.Write("   };" & vbNewLine)
            End If
            Response.Write("};" & vbNewLine)
            Response.Write("//-->" & vbNewLine)
            Response.Write("</script>" & vbNewLine)
            Response.Write("</body>" & vbNewLine)
            Response.Write("</html>" & vbNewLine)
            Response.End()

        End Sub
        ''' <summary>
        '''     When no special referer has been set up, then the redirect will go to the normal start page
        ''' </summary>
        ''' <value></value>
        Protected Overridable ReadOnly Property StartPageUrl() As String
            Get
                Return cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL
            End Get
        End Property
        ''' <summary>
        '''     When the user went to a document which required a logon, the document's address might be saved in the user session for later redirecting again. This property returns this value and removes the referer item from the session objects (to prevent an additional "going back").
        ''' </summary>
        ''' <value></value>
        Protected Overridable ReadOnly Property Referer() As String
            Get
                Dim Result As String = Nothing
                'Retrieve the current referer value
                If CType(Session("System_Referer"), String) <> Nothing Then
                    Result = CType(Session("System_Referer"), String)
                Else
                    Try
                        Result = CompuMaster.camm.WebManager.Utils.Nz(cammWebManager.System_GetSessionValue("System_Referer"), "")
                    Catch
                    End Try
                End If
                'Reset any referers
                Session("System_Referer") = Nothing
                If cammWebManager.IsLoggedOn Then
                    cammWebManager.System_SetSessionValue("System_Referer", Nothing)
                End If
                'Return the result
                Return Result
            End Get
        End Property

        ''' <summary>Reload within frames after the login has completed</summary>
        ''' <value></value>
        Protected Overridable ReadOnly Property ReloadInTopFrameBeforeRedirect() As Boolean
            Get
                Return True
            End Get
        End Property

    End Class
    ''' <summary>
    '''     Login utils
    ''' </summary>
    Public Module Utils

        Friend Function CryptedPassword(ByVal password As String) As String

            Dim MyPasswordBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(password)
            Return Convert.ToBase64String(MyPasswordBytes)

        End Function

        Friend Function DecryptedPassword(ByVal encryptedPassword As String) As String

            Dim MyPasswordBytes As Byte() = Convert.FromBase64String(encryptedPassword)
            Return System.Text.Encoding.Unicode.GetString(MyPasswordBytes)

        End Function
        ''' <summary>
        '''     Does the configuration allows a general login possibility via the query string data or else is it specially allowed for the specified user?
        ''' </summary>
        ''' <param name="username">Does this user has got login authorization via the HTTP GET method?</param>
        ''' <param name="cammWebManager">A camm Web-Manager instance</param>
        Public Function IsLoginAllowedViaQueryStringData(ByVal username As String, ByVal cammWebManager As WMSystem) As Boolean

            Dim AutomaticLogonAllowedByMachineToMachineCommunication As Boolean
            Try
                Dim MyPotentialUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation
                MyPotentialUserInfo = cammWebManager.System_GetUserInfo(CType(cammWebManager.System_GetUserID(username), Long))
                AutomaticLogonAllowedByMachineToMachineCommunication = MyPotentialUserInfo.AutomaticLogonAllowedByMachineToMachineCommunication
            Catch
                AutomaticLogonAllowedByMachineToMachineCommunication = False
            End Try
            If Configuration.AllowLogonViaRequestTypeGet = True Then
                AutomaticLogonAllowedByMachineToMachineCommunication = True
            End If
            Return AutomaticLogonAllowedByMachineToMachineCommunication

        End Function
        ''' <summary>
        '''     To complete the logon process, all the other servers in the same server group have to know the user, too
        ''' </summary>
        ''' <param name="cammWebManager">A camm Web-Manager instance</param>
        ''' <remarks>
        '''     This method will only be triggered on the master server (=the logon server) of a server group
        ''' </remarks>
        Friend Sub LogonToAllOtherServers(ByVal cammWebManager As WMSystem, ByVal javaScriptCodeWhenLoginFailed As String)

            'WebAreas Login vervollständigen
            Dim System_RedirectURI As String
            System_RedirectURI = cammWebManager.System_GetNextLogonURI(cammWebManager.CurrentUserLoginName)
            If System_RedirectURI = "" Then
                System_RedirectURI = cammWebManager.CalculateUrl(cammWebManager.CurrentServerInfo.ParentServerGroup.MasterServer.ID, CompuMaster.camm.WebManager.WMSystem.ScriptEngines.ASPNet, "/sysdata/login/loginprocedurefinished.aspx")
            End If

            'Debug mode: manual redirects
            If cammWebManager.System_DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                If InStr(System_RedirectURI, "?") > 0 Then
                    System_RedirectURI &= "&debug=1"
                Else
                    System_RedirectURI &= "?debug=1"
                End If
            End If

            HttpContext.Current.Response.Clear()

            HttpContext.Current.Response.Write("<html>" & vbNewLine)
            HttpContext.Current.Response.Write("<head>" & vbNewLine)
            HttpContext.Current.Response.Write("<title>Check login</title>" & vbNewLine)
            HttpContext.Current.Response.Write("<link rel=""stylesheet"" type=""text/css"" href=""" & cammWebManager.Internationalization.User_Auth_Config_UserAuthMasterServer & cammWebManager.Internationalization.User_Auth_Config_Paths_SystemData & "style_standard.css"">" & vbNewLine)
            HttpContext.Current.Response.Write("</head>" & vbNewLine)
            HttpContext.Current.Response.Write("<body>" & vbNewLine)
            HttpContext.Current.Response.Write("<br><br><br><br><br>" & vbNewLine)
            HttpContext.Current.Response.Write("<center>" & vbNewLine)
            HttpContext.Current.Response.Write("" & vbNewLine)
            HttpContext.Current.Response.Write("<div id=""cont_progress""></div>" & vbNewLine)
            HttpContext.Current.Response.Write("<script language=""javascript"">" & vbNewLine)
            HttpContext.Current.Response.Write("<!--" & vbNewLine)
            HttpContext.Current.Response.Write("	var cont_progress_content = '';" & vbNewLine)
            HttpContext.Current.Response.Write("" & vbNewLine)
            HttpContext.Current.Response.Write("	function writetoLyr(id, newContent)" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		cont_progress_content += newContent;" & vbNewLine)
            HttpContext.Current.Response.Write("" & vbNewLine)
            HttpContext.Current.Response.Write("		if (document.layers) " & vbNewLine)
            HttpContext.Current.Response.Write("		{" & vbNewLine)
            HttpContext.Current.Response.Write("			document.layers[id].document.write(cont_progress_content);" & vbNewLine)
            HttpContext.Current.Response.Write("			document.layers[id].document.close();" & vbNewLine)
            HttpContext.Current.Response.Write("		}" & vbNewLine)
            HttpContext.Current.Response.Write("		else if (document.all) " & vbNewLine)
            HttpContext.Current.Response.Write("		{" & vbNewLine)
            HttpContext.Current.Response.Write("			eval(""document.all.""+id+"".innerHTML='""+cont_progress_content+""'"");" & vbNewLine)
            HttpContext.Current.Response.Write("		}" & vbNewLine)
            HttpContext.Current.Response.Write("		else" & vbNewLine)
            HttpContext.Current.Response.Write("		{" & vbNewLine)
            HttpContext.Current.Response.Write("			document.getElementById(id).innerHTML = cont_progress_content;" & vbNewLine)
            HttpContext.Current.Response.Write("		};" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)

            HttpContext.Current.Response.Write("	function TimeOver1()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<h3>" & cammWebManager.Internationalization.Logon_Connecting_InProgress & "</h3><img src=""/system/login/images/darkbluepixel.gif"" width=""350"" height=""2"" border=""0""><br><img src=""/system/login/images/space.gif"" width=""350"" height=""2"" border=""0""><br><img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver2()', 2000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver2()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver3()', 2000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver3()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver4()', 3000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver4()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver5()', 3000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver5()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver6()', 5000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver6()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver7()', 5000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver7()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver8()', 7000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver8()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver9()', 7500);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver9()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0"">');" & vbNewLine)
            HttpContext.Current.Response.Write("		window.setTimeout ('TimeOver10()', 10000);" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)
            HttpContext.Current.Response.Write("	function TimeOver10()" & vbNewLine)
            HttpContext.Current.Response.Write("	{" & vbNewLine)
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<img src=""/system/login/images/progres.gif"" width=35 height=10 border=""0""><br><img src=""/system/login/images/space.gif"" width=""350"" height=""2"" border=""0""><br><img src=""/system/login/images/darkbluepixel.gif"" width=""350"" height=""2"" border=""0""><br>');" & vbNewLine)
            If javaScriptCodeWhenLoginFailed <> Nothing Then
                HttpContext.Current.Response.Write(javaScriptCodeWhenLoginFailed & vbNewLine)
            End If
            HttpContext.Current.Response.Write("		writetoLyr ('cont_progress', '<h3>" & cammWebManager.Internationalization.Logon_Connecting_LoginTimeout & "</h3><p>" & CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.Logon_Connecting_RecommendationOnTimeout, "" & cammWebManager.TechnicalServiceEMailAccountAddress & "") & "</p>');" & vbNewLine)
            HttpContext.Current.Response.Write("	}" & vbNewLine)

            HttpContext.Current.Response.Write("	window.setTimeout ('TimeOver1()', 2000);" & vbNewLine)
            HttpContext.Current.Response.Write("//-->" & vbNewLine)
            HttpContext.Current.Response.Write("</script>" & vbNewLine)
            HttpContext.Current.Response.Write("<iframe width=""" & CType(IIf(cammWebManager.System_DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation, "400", "1"), String) & """ height=""" & CType(IIf(cammWebManager.System_DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation, "300", "1"), String) & """ frameborder=off scrolling=no src=""" & System_RedirectURI & """>" & vbNewLine)
            'Debug mode: manual redirects
            If cammWebManager.System_DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                HttpContext.Current.Response.Write("<script language=""javascript"">" & vbNewLine)
                HttpContext.Current.Response.Write("<!--" & vbNewLine)
                HttpContext.Current.Response.Write("	window.location='" & System_RedirectURI & "';" & vbNewLine)
                HttpContext.Current.Response.Write("//-->" & vbNewLine)
                HttpContext.Current.Response.Write("</script>" & vbNewLine)
            End If
            HttpContext.Current.Response.Write("<noscript>" & vbNewLine)
            HttpContext.Current.Response.Write("<h3><font face=""Arial"" color=""red"">Please activate JavaScript in your browser and try again!</font></h3>" & vbNewLine)
            HttpContext.Current.Response.Write("</noscript>" & vbNewLine)
            HttpContext.Current.Response.Write("</iframe>" & vbNewLine)

            HttpContext.Current.Response.Write("</center>" & vbNewLine)
            HttpContext.Current.Response.Write("</body>" & vbNewLine)
            HttpContext.Current.Response.Write("</html>" & vbNewLine)
            HttpContext.Current.Response.End()

        End Sub
        ''' <summary>
        '''     Execute the logon to the current master server
        ''' </summary>
        ''' <param name="cammWebManager"></param>
        ''' <param name="usercredentials"></param>
        ''' <returns>An URL where the browser should be redirected to</returns>
        Friend Function LogonToCurrentMasterServer(ByVal cammWebManager As WMSystem, ByVal userCredentials As LogonCredentials, ByVal redirectionTargets As System.Collections.Specialized.NameValueCollection) As String
            Dim Result As String = Nothing
            If userCredentials Is Nothing OrElse userCredentials.Username = Nothing Then
                Result = (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?ErrID=" & CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs.ErrorUserOrPasswordWrong) 'Empty username or password
                If Not redirectionTargets Is Nothing AndAlso redirectionTargets(CType(WMSystem.ReturnValues_UserValidation.UserOrPasswortMisstyped_OR_AccountLocked_OR_LoginDeniedAtThisServerGroup, Integer).ToString) <> Nothing Then
                    Result = redirectionTargets(CType(WMSystem.ReturnValues_UserValidation.UserOrPasswortMisstyped_OR_AccountLocked_OR_LoginDeniedAtThisServerGroup, Integer).ToString)
                End If
            Else
                'Prepare the login
                Dim strServerIP As String, strRemoteIP As String
                Dim strWebApplication As String = "", strWebApplicationParameters As String = ""
                strServerIP = cammWebManager.CurrentServerIdentString
                strRemoteIP = cammWebManager.CurrentRemoteClientAddress

                If strWebApplication = "" Then
                    strWebApplicationParameters = HttpContext.Current.Request.ServerVariables("SCRIPT_NAME")
                Else
                    strWebApplicationParameters = ""
                End If

                'Execute the login
                Dim ValidationResult As CompuMaster.camm.WebManager.WMSystem.ReturnValues_UserValidation
                If userCredentials.Password = "" Then
                    ValidationResult = cammWebManager.ExecuteLogin(userCredentials.Username, userCredentials.ForceLogin)
                Else
                    ValidationResult = cammWebManager.ExecuteLogin(userCredentials.Username, userCredentials.Password, userCredentials.ForceLogin)
                End If

                'Analyze the result value
                If ValidationResult = WMSystem.ReturnValues_UserValidation.ServerNotFound Then
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & System.Web.HttpUtility.UrlEncode(CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorWrongNetwork, Integer).ToString) & "&User=" & System.Web.HttpUtility.UrlEncode(userCredentials.Username))
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.NoLoginRightForThisServer Then
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & System.Web.HttpUtility.UrlEncode(CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorWrongNetwork, Integer).ToString) & "&User=" & System.Web.HttpUtility.UrlEncode(userCredentials.Username))
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.ValidationSuccessfull_ButNoAuthorizationForRequiredSecurityObject Then
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?DisplayFrameSet=No&ErrID=" & System.Web.HttpUtility.UrlEncode(CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization, Integer).ToString) & "&User=" & System.Web.HttpUtility.UrlEncode(userCredentials.Username))
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.AlreadyLoggedIn Then
                    HttpContext.Current.Session("System_Logon_Buffer_Username") = userCredentials.Username
                    HttpContext.Current.Session("System_Logon_Buffer_Passcode") = userCredentials.Password
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_TerminateOldSessionScriptURL)
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.AccessDenied Then
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & System.Web.HttpUtility.UrlEncode(CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorNoAuthorization, Integer).ToString) & "&User=" & System.Web.HttpUtility.UrlEncode(userCredentials.Username))
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.TooManyLoginFailures Then
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & System.Web.HttpUtility.UrlEncode(CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorLogonFailedTooOften, Integer).ToString) & "&User=" & System.Web.HttpUtility.UrlEncode(userCredentials.Username))
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.ValidationSuccessfull Then
                    'Access granted!
                    cammWebManager.SetUserLoginName(userCredentials.Username)
                    'Remove temporary login data!
                    HttpContext.Current.Session.Remove("System_Logon_Buffer_Username")
                    HttpContext.Current.Session.Remove("System_Logon_Buffer_Passcode")
                ElseIf ValidationResult = WMSystem.ReturnValues_UserValidation.UserOrPasswortMisstyped_OR_AccountLocked_OR_LoginDeniedAtThisServerGroup Or ValidationResult = WMSystem.ReturnValues_UserValidation.UserNotFound_BecauseOf_UserAccountAccessability Or ValidationResult = WMSystem.ReturnValues_UserValidation.UserAccountLocked Then
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?ErrID=" & CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs.ErrorUserOrPasswordWrong)
                Else
                    Result = (cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorUndefined & "&User=" & System.Web.HttpUtility.UrlEncode(userCredentials.Username))  'Unknown
                End If

                'Lookup for return value overwritings
                If Not redirectionTargets Is Nothing AndAlso redirectionTargets(CType(ValidationResult, Integer).ToString) <> Nothing Then
                    Result = redirectionTargets(CType(ValidationResult, Integer).ToString)
                End If
            End If


            Return Result
        End Function

        ''' <summary>
        '''     Logon data
        ''' </summary>
        Public Class LogonCredentials
            Public Username As String
            Public Password As String
            Public ForceLogin As Boolean
        End Class

    End Module

    ''' <summary>
    '''     Validate the authorization credentials and start the login process when possible
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class CheckLogin
        Inherits Page
        ''' <summary>
        '''     The login information which shall be used for the logon
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Please note: the return value must be null (Nothing in VisualBasic) when the login credentials are incomplete
        ''' </remarks>
        Protected Overridable ReadOnly Property LoginCredentials() As LogonCredentials
            Get
                Dim Result As New LogonCredentials
                If Request.Form("Username") <> "" AndAlso Request.Form("Passcode") <> "" Then
                    Result = New LogonCredentials
                    Result.Username = Request.Form("Username")
                    Result.Password = Request.Form("Passcode")
                    If Request.Form("ForceLogin") = "1" Then
                        Result.ForceLogin = True
                    Else
                        Result.ForceLogin = False
                    End If
                ElseIf Request.QueryString("Username") <> "" AndAlso (Request.QueryString("Passcode") <> "" OrElse Request.QueryString("PassECode") <> "") AndAlso IsLoginAllowedViaQueryStringData(Request.QueryString("Username"), Me.cammWebManager) Then
                    Result = New LogonCredentials
                    Result.Username = Request.QueryString("Username")
                    Result.Password = Request.QueryString("Passcode")
                    If Request.QueryString("PassECode") <> "" Then
                        Result.Password = DecryptedPassword(Request.QueryString("PassECode"))
                    End If
                    If Request.QueryString("ForceLogin") Is Nothing Then
                        'Default when not defined
                        Result.ForceLogin = True
                    ElseIf Request.QueryString("ForceLogin") = "1" Then
                        Result.ForceLogin = True
                    Else
                        Result.ForceLogin = False
                    End If
                End If
                Return Result
            End Get
        End Property

        Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Request.Form("TargetUrlAfterLogin") <> Nothing Then
                Session("System_Referer") = Request.Form("TargetUrlAfterLogin")
            End If
            ValidateUserCredentialsAndLogon()
        End Sub
        ''' <summary>
        '''     Are any login credentials available? In single-sign-on scenarios, the user might be logged on with an external user account or anonymously.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This property helps to find out why the LoginCredentials property was empty: either the external login information hasbn't been there or else the login information had been there but without an assigned, valid webmanager account
        ''' </remarks>
        Protected Overridable ReadOnly Property IsAuthenticated() As Boolean
            Get
                If LoginCredentials Is Nothing Then
                    Return False
                Else
                    Return LoginCredentials.Username <> Nothing
                End If
            End Get
        End Property
        ''' <summary>
        '''     Validate the login data from the login form and then login on the current (master) server
        ''' </summary>
        Public Overridable Sub ValidateUserCredentialsAndLogon()

            Session.Timeout = 240 '4 h

            Dim System_ErrorRedirectURI As String = Nothing
            Dim UserCredentials As LogonCredentials = Nothing
            If Request.QueryString("User") = "created" And cammWebManager.IsLoggedOn Then
                'User has just been created; Session("System_Username") has already been set
            Else
                'Retrieve user logon credentials
                UserCredentials = Me.LoginCredentials
                If UserCredentials Is Nothing OrElse (UserCredentials.Username = Nothing AndAlso UserCredentials.Password = Nothing) Then
                    'Lookup for return value overwritings
                    'If Not RedirectionTargets Is Nothing AndAlso RedirectionTargets(CType(WMSystem.ReturnValues_UserValidation.UserForDemandedExternalAccountNotFound, Integer).ToString) <> Nothing Then
                    '    System_ErrorRedirectURI = RedirectionTargets(CType(WMSystem.ReturnValues_UserValidation.UserForDemandedExternalAccountNotFound, Integer).ToString)
                    'Else
                    If Not IsAuthenticated Then
                        OnMissingAuthentication()
                        Exit Sub
                    Else
                        OnMissingAssignmentOfExternalAccount()
                        Exit Sub
                    End If
                Else
                    'User credentials checked
                    System_ErrorRedirectURI = Utils.LogonToCurrentMasterServer(Me.cammWebManager, UserCredentials, RedirectionTargets)
                End If
            End If
            If System_ErrorRedirectURI <> "" Then
                If UserCredentials Is Nothing Then
                    cammWebManager.RedirectTo(System_ErrorRedirectURI)
                Else
                    cammWebManager.RedirectTo(System_ErrorRedirectURI, "UserCredentials.Username=" & UserCredentials.Username, Nothing)
                End If
            End If

            'After logging on to the master server, we have to logon now to the other related servers
            LogonToAllOtherServers()

        End Sub
        ''' <summary>
        '''     The actions which shall be made if an external login has been detected but there is not user account in CWM for it
        ''' </summary>
        Protected Overridable Sub OnMissingAssignmentOfExternalAccount()
            Throw New Exception("External account not assigned to a camm Web-Manager user account, and no redirection URL defined")
        End Sub
        ''' <summary>
        '''     The actions which shall be made if an external login has been detected but there is not user account in CWM for it
        ''' </summary>
        Protected Overridable Sub OnMissingAuthentication()
            Me.cammWebManager.RedirectToLogonPage(WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs.ErrorUserOrPasswordWrong, "Correct user credentials are required", Nothing)
        End Sub
        ''' <summary>
        '''     To complete the logon process, all the other servers in the same server group have to know the user, too
        ''' </summary>
        Protected Overridable Sub LogonToAllOtherServers()
            Me.LogonToAllOtherServers(Nothing)
        End Sub
        ''' <summary>
        '''     To complete the logon process, all the other servers in the same server group have to know the user, too
        ''' </summary>
        Protected Overridable Sub LogonToAllOtherServers(ByVal javaScriptCodeWhenLoginFailed As String)
            Utils.LogonToAllOtherServers(Me.cammWebManager, javaScriptCodeWhenLoginFailed)
        End Sub
        ''' <summary>
        '''     Redirection target overwritings for the different authentication return values
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     The key string in this collection is the expected return value, the value is the URL where the browser shall be redirected to
        ''' </remarks>
        Protected Overridable ReadOnly Property RedirectionTargets() As System.Collections.Specialized.NameValueCollection
            Get
                Return Nothing
            End Get
        End Property

    End Class

    ''' <summary>
    '''     A page which asks to really process the pending login when there is already a login from another workstation
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class ForceLogin
        Inherits Page

        Public ReadOnly Property CheckLoginUrl() As String
            Get
                If Request.QueryString("CheckLoginUrl") <> Nothing Then
                    Return Request.QueryString("CheckLoginUrl")
                Else
                    Return cammWebManager.Internationalization.User_Auth_Validation_CheckLoginURL
                End If
            End Get
        End Property

    End Class

    ''' <summary>
    '''     The regular login page
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class LoginForm
        Inherits Page
        ''' <summary>
        '''     Create an URL to load the frameset and the original path in the main frame of the frameset
        ''' </summary>
        ''' <param name="path">The original URL of the referer page</param>
        Public Overridable Function ApplyRefererUrlToFramesetUrl(ByVal path As String) As String
            If path = Nothing Then
                'When there is no referer, keep it empty
                Return Nothing
            ElseIf path.ToLower.StartsWith(cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL.ToLower) Then
                'The referer is the frameset page - never re-apply the path to load in the frameset again
                Return path
            End If
            'Embed the referer URL in the frameset URL
            If InStr(cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL, "?") = 0 Then
                Return cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL & "?ref=" & Server.UrlEncode(path)
            Else
                Return cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL & "&ref=" & Server.UrlEncode(path)
            End If
        End Function
        ''' <summary>
        '''     Render a page which looks up wether the current page is running in a frame window or if it's running as the top window
        ''' </summary>
        Public Sub RenderPageLookupIfTheCurrentWindowIsTheTopWindow()

            Response.Clear()
            Response.Write("<html>" & vbNewLine)
            Response.Write("<body>" & vbNewLine)
            Response.Write("<script language=""javascript"">" & vbNewLine)
            Response.Write("<!--" & vbNewLine)
            Response.Write("// Reload current page in the _TOP frame if we're in a frame, currently" & vbNewLine)
            Response.Write("if (window.parent.length != 0)" & vbNewLine)
            Response.Write("{" & vbNewLine)
            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                Response.Write("   confirm('Login page is already in a child frame');" & vbNewLine)
            End If
            If InStr(Request.Url.ToString, "?") = 0 Then
                Response.Write("   window.location = window.location + '?window=checked';" & vbNewLine)
            Else
                Response.Write("   window.location = window.location + '&window=checked';" & vbNewLine)
            End If
            Response.Write("}" & vbNewLine)
            Response.Write("else" & vbNewLine)
            Response.Write("{" & vbNewLine)
            Dim newUrl As String
            Dim CurrentRequestUrlWithoutRefParameter As New UriBuilder(Request.Url)
            CurrentRequestUrlWithoutRefParameter.Query = WebManager.Utils.QueryStringWithoutSpecifiedParameters(New String() {"ref"})
            If InStr(CurrentRequestUrlWithoutRefParameter.ToString, "?") = 0 Then
                newUrl = ApplyRefererUrlToFramesetUrl(CurrentRequestUrlWithoutRefParameter.ToString & "?ref=" & Server.UrlEncode(ApplyRefererUrlToFramesetUrl(Request.QueryString("ref"))) & "&window=checked")
            Else
                newUrl = ApplyRefererUrlToFramesetUrl(CurrentRequestUrlWithoutRefParameter.ToString & "&ref=" & Server.UrlEncode(ApplyRefererUrlToFramesetUrl(Request.QueryString("ref"))) & "&window=checked")
            End If
            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                Response.Write("   if (confirm('Reload logon form page in frameset + change ref parameter to use frameset\n" & newUrl.Replace("\", "\\").Replace("'", "\'") & "'))" & vbNewLine)
                Response.Write("   {" & vbNewLine)
            End If
            Response.Write("   window.location = '" & newUrl.Replace("\", "\\").Replace("'", "\'") & "';" & vbNewLine)
            If cammWebManager.DebugLevel >= WMSystem.DebugLevels.Max_RedirectPageRequestsManually Then
                Response.Write("   };" & vbNewLine)
            End If
            Response.Write("};" & vbNewLine)
            Response.Write("//-->" & vbNewLine)
            Response.Write("</script>" & vbNewLine)
            Response.Write("</body>" & vbNewLine)
            Response.Write("</html>" & vbNewLine)
            Response.End()

        End Sub

        Private UserMessage As String
        Public Property ErrorMessageForUser() As String
            Get
                Return UserMessage
            End Get
            Set(ByVal Value As String)
                UserMessage = Value
            End Set
        End Property

        Public Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            If Request.QueryString("ref") <> "" Then
                'If there is a given referer argument, assign it
                If Configuration.UseFrameset = True AndAlso Request.QueryString("window") <> "checked" Then
                    RenderPageLookupIfTheCurrentWindowIsTheTopWindow()
                ElseIf Configuration.UseFrameset = True Then
                    'Embed the referer in the frameset URL
                    Session("System_Referer") = ApplyRefererUrlToFramesetUrl(Request.QueryString("ref"))
                Else
                    'Take the referer as it is for frameless environments
                    Session("System_Referer") = Request.QueryString("ref")
                End If
            End If

            If cammWebManager.IsLoggedOn Then 'LCase(Request.QueryString("Action")) = "logout" Or 
                cammWebManager.ExecuteLogout()
            End If

            'Cookies im Browser aktiviert?
            If CType(Session("System_CookiesActivated"), Boolean) <> True Then
                If Request.QueryString("TestCookie") <> "1" Then
                    Session("System_CookiesActivated") = True
                    Dim RedirectionParams As String = WebManager.Utils.QueryStringWithoutSpecifiedParameters(New String() {"lang", "TestCookie"})
                    cammWebManager.RedirectTo(cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?TestCookie=1&" & RedirectionParams, "Session(""System_CookiesActivated"") set to """ & CType(Session("System_CookiesActivated"), Boolean).ToString & """", Nothing)
                Else
                    cammWebManager.RedirectTo(cammWebManager.Internationalization.User_Auth_Validation_AccessErrorScriptURL & "?ErrID=17", Nothing, Nothing)
                End If
            End If

            If Request.QueryString("ErrCode") <> "" Then
                UserMessage = "<p><font face=""Arial"" color=""red"">" & Request.QueryString("ErrCode") & "</font></p>"
            End If
            Select Case Request.QueryString("ErrID")
                Case CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs.InfoUserLoggedOutSuccessfully, Integer).ToString
                    UserMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.InfoUserLoggedOutSuccessfully & "</font></p>"
                    'RedirectRequest (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?LoginMessage=" & System.Web.HttpUtility.Urlencode(UserMessage))
                Case CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs.ErrorUserOrPasswordWrong, Integer).ToString
                    UserMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorUserOrPasswordWrong & "</font></p>"
                    'RedirectRequest (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?LoginMessage=" & System.Web.HttpUtility.Urlencode(UserMessage))
                Case CType(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_LoginPageForwarderIDs.ErrorEmptyPassword, Integer).ToString
                    UserMessage = "<p><font face=""Arial"" color=""red"">" & cammWebManager.Internationalization.ErrorEmptyPassword & "</font></p>"
                    'RedirectRequest (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?LoginMessage=" & System.Web.HttpUtility.Urlencode(UserMessage))
            End Select

            If cammWebManager.Internationalization.OfficialServerGroup_Title = "" Then
                'Database connection error/server configuration error
                cammWebManager.RedirectToErrorPage(CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs.ErrorServerConfigurationError, "Bad configuration detected", Nothing)
            End If

        End Sub

        Private Sub LoginForm_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender

            'Ensure that the frameset setting has been handled
            If Configuration.UseFrameset = True AndAlso Request.QueryString("window") <> "checked" Then
                RenderPageLookupIfTheCurrentWindowIsTheTopWindow()
            End If

        End Sub

    End Class

End Namespace