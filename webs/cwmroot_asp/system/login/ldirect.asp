<%@ CodePage=65001 %>
<!--#include virtual="/system/definitions.asp"-->
<%
'***************************************************
'*** Autorisiert den aktuellen Benutzer an       ***
'*** Server und ScriptEngine                     ***
'***************************************************

	dim AddLangID2RedirectAddr
	AddLangID2RedirectAddr = "&lang=" & Request.QueryString("lang")
	If Request.QueryString("lang") <> "" Then
		Session("CurLanguage") = CLng(Request.QueryString("Lang"))
		Response.Cookies("CWM").Expires = Now + 183 'Cookie läuft ab in 6 Monaten
		If UCase(Request.ServerVariables("HTTPS")) = "ON" Then
			Response.Cookies("CWM").Secure = True
		Else
			Response.Cookies("CWM").Secure = False
		End If
		Response.Cookies("CWM")("Lang") = Session("CurLanguage")
	End If

	Application.Lock
	on error goto 0

	dim MyRedirectTo
	dim strServerIP
	dim UserLoginName2Set
	strServerIP = GetCurrentServerIdentString
	MyRedirectTo = Request.QueryString("redirectto")

	If Request.QueryString("User") <> "" Then

		'Create connection
		Set User_Auth_Validation_DBConn = Server.CreateObject("ADODB.Connection")
		Set User_Auth_Validation_RecSet = Server.CreateObject("ADODB.Recordset")
		User_Auth_Validation_DBConn.Open(User_Auth_Validation_DSN)

		'Open command object with one parameter
		Set User_Auth_Validation_Cmd = Server.CreateObject("ADODB.Command")

		'Get parameter value and append parameter
		User_Auth_Validation_Cmd.CommandText = "Public_ValidateGUIDLogin"
		User_Auth_Validation_Cmd.CommandType = adCmdStoredProc

		User_Auth_Validation_Cmd.Parameters.Append User_Auth_Validation_Cmd.CreateParameter("@Username", adVarWChar, adParamInput, len(Request.QueryString("User")), Request.QueryString("User"))
		User_Auth_Validation_Cmd.Parameters.Append User_Auth_Validation_Cmd.CreateParameter("@GUID", adInteger, adParamInput, 4, CLng(Request.QueryString("GUID")))
		User_Auth_Validation_Cmd.Parameters.Append User_Auth_Validation_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, CStr(strServerIP))
		User_Auth_Validation_Cmd.Parameters.Append User_Auth_Validation_Cmd.CreateParameter("@RemoteIP", adVarWChar, adParamInput, 32, Request.ServerVariables("REMOTE_ADDR"))
		User_Auth_Validation_Cmd.Parameters.Append User_Auth_Validation_Cmd.CreateParameter("@ScriptEngine_ID", adInteger, adParamInput, 4, 1)
		User_Auth_Validation_Cmd.Parameters.Append User_Auth_Validation_Cmd.CreateParameter("@ScriptEngine_SessionID", adVarWChar, adParamInput, 512, Session.SessionID)

		'Create recordset by executing the command
		Set User_Auth_Validation_Cmd.ActiveConnection = User_Auth_Validation_DBConn
		Set User_Auth_Validation_RecSet  = User_Auth_Validation_Cmd.Execute
		If User_Auth_Validation_RecSet.State = 0 Then 'adStateClosed
			UserLoginName2Set = ""
		ElseIf User_Auth_Validation_RecSet.Eof Or User_Auth_Validation_RecSet.Bof Then
			UserLoginName2Set = ""
		Else
			UserLoginName2Set = User_Auth_Validation_RecSet.Fields(0)
		End If
		Set User_Auth_Validation_Cmd.ActiveConnection = Nothing

	End If

	Application.UnLock

	Session.Timeout = 240 '4 h
	If Session("System_Username") = "" Then
		'Keine Änderung erlauben durch simples überschreiben eines vorhandenen
		'Wertes durch den mitgeteilten, jedoch Anmeldung eines Benutzers ausführen,
		'wenn bisher noch niemand in dieser Browsersession angemeldet ist.
		Session("System_Username") = UserLoginName2Set
	ElseIf Session("System_Username") <> "" And Request.QueryString("User") = "" Then
		'Abmeldung
		Session("System_Username") = ""
	End If

	'Nächstes Login
	dim System_RedirectURI
	If Session("System_Username") <> "" Then
		System_RedirectURI = System_GetNextLogonURI (Session("System_Username"))
	Else
		System_RedirectURI = System_GetNextLogonURIOfUserAnonymous ()
	End If

	dim MyResponseRedirect
	If System_RedirectURI = "" Then
		If MyRedirectTo <> "" Then
			'e.g. frame_sub.asp
			MyResponseRedirect = MyRedirectTo
		Else
			'MyResponseRedirect = User_Auth_Validation_NoRefererURL
			MyResponseRedirect = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_SystemData & "login/loginprocedurefinished.aspx"
		End If
	Else
		MyResponseRedirect = System_RedirectURI & "&redirectto=" & server.URLEncode(MyRedirectTo) & AddLangID2RedirectAddr
	End If
	If System_DebugLevel >= 5 Then
		Response.Write "<a href=""" & MyResponseRedirect & """>" & MyResponseRedirect & "</a>"
	Else
		Response.Redirect MyResponseRedirect
	End If
%>