<!--#include virtual="/system/definitions.asp"-->
<%
Sub System_CheckForAccessAuthorization(ApplicationName, intReserved)
'***************************************************
'*** Überprüft die Berechtigung für das aktuelle ***
'*** Dokument, welches diese Prozedur aufruft    ***
'***************************************************
dim strServerIP
dim strRemoteIP
dim User_Auth_Validation_DBConn
dim User_Auth_Validation_RecSet
dim User_Auth_Validation_Cmd
dim strWebURL
'dim MyReferer

	System_Init 'If not yet done...

	Application.Lock
	on error resume next

	If User_Auth_Validation_AccessErrorScriptURL = "" Then
		User_Auth_Validation_AccessErrorScriptURL = "/system/access_error.asp"
	End If

	If LCase(ApplicationName) = "@@anonymous" Or (LCase(ApplicationName) = "@@public" And Session("System_Username") <> "") Then
		Exit Sub
	ElseIf (LCase(ApplicationName) = "@@public" And Session("System_Username") = "") Then
		ApplicationName = "Public"
	End If
	If Trim(ApplicationName) = "" Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(134) & "&URL=" & Server.URLEncode(Request.ServerVariables("SCRIPT_NAME"))
	End If

	'Response.ExpiresAbsolute = Now() -1
	'Response.AddHeader "pragma", "no-cache"
	'Response.AddHeader "cache-control", "private, no-cache, must-revalidate"

	strServerIP = GetCurrentServerIdentString
	strRemoteIP = Request.ServerVariables("REMOTE_ADDR")

	If Request.QueryString = "" Then
		strWebURL = System_GetServerURL (CStr(strServerIP)) & Request.ServerVariables("SCRIPT_NAME")
	Else
		strWebURL = System_GetServerURL (CStr(strServerIP)) & Request.ServerVariables("SCRIPT_NAME") & "?" & Request.QueryString
	End If

	'Create connection
	Set User_Auth_Validation_DBConn = Server.CreateObject("ADODB.Connection")
	User_Auth_Validation_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set User_Auth_Validation_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
    With User_Auth_Validation_Cmd

		.CommandText = "Public_ValidateDocument"
		.CommandType = adCmdStoredProc

        If Session("System_Username") = "" Then
	        .Parameters.Append .CreateParameter("@Username", adVarWChar, adParamInput, 1, Null)
	    Else
	        .Parameters.Append .CreateParameter("@Username", adVarWChar, adParamInput, len((Session("System_Username"))), (Session("System_Username")))
	    End If
        .Parameters.Append .CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, CStr(strServerIP))
        .Parameters.Append .CreateParameter("@RemoteIP", adVarWChar, adParamInput, 32, CStr(strRemoteIP))
        .Parameters.Append .CreateParameter("@WebApplication", adVarWChar, adParamInput, 1024, CStr(Mid(ApplicationName,1,1024)))
        .Parameters.Append .CreateParameter("@WebURL", adVarWChar, adParamInput, 1024, CStr(Mid(strWebURL,1,1024)))
		.Parameters.Append .CreateParameter("@ScriptEngine_ID", adInteger, adParamInput, 4, 1)
		.Parameters.Append .CreateParameter("@ScriptEngine_SessionID", adVarWChar, adParamInput, 512, Session.SessionID)
		If Not IsNull(intReserved) Then
			'Logging success/standard hit yes/no
			.Parameters.Append .CreateParameter("@Reserved", adInteger, adParamInput, 4, CLng(intReserved))
		End If
    End With

	'Create recordset by executing the command
	Set User_Auth_Validation_Cmd.ActiveConnection = User_Auth_Validation_DBConn
	Set User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.Execute

	If err<>0 then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(24) & "&ErrCode=" & Server.URLEncode(Err.Description) ' Unknown
	ElseIf User_Auth_Validation_RecSet.State = 0 Then 'adStateClosed
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(24) & "&ErrCode=" & Server.URLEncode("No ReturnValue because RecordSet closed") ' Unknown
	ElseIf (User_Auth_Validation_RecSet.BOF And User_Auth_Validation_RecSet.EOF) Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(24) & "&ErrCode=" & Server.URLEncode("No ReturnValue because RecordSet empty") ' Unknown
	ElseIf IsNull(User_Auth_Validation_RecSet.Fields(0)) = True Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(24) & "&ErrCode=" & Server.URLEncode("ReturnValue = Null") ' Unknown
	ElseIf User_Auth_Validation_RecSet.Fields(0) = 58 Then
		Response.Redirect User_Auth_Validation_LogonScriptURL & "?ref=" & Server.URLEncode(strWebURL)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = 57 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(30)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -10 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(16)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -9 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(16)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -5 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?DisplayFrameSet=No&ErrID=" & Server.URLEncode(27) & "&ErrCode=" & Server.URLEncode("Application name=""" & ApplicationName & """; URL=""" & strWebURL & """")
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -4 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(29)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -3 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(27)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -2 Then
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(34)
	ElseIf User_Auth_Validation_RecSet.Fields(0) = -1 Then
		'Access granted! - Extraline because of unknown but existing values which otherwise result in problems
	ElseIf User_Auth_Validation_RecSet.Fields(0) = 0 Or User_Auth_Validation_RecSet.Fields(0) = 43 Or User_Auth_Validation_RecSet.Fields(0) = 44 Then
		Response.Redirect User_Auth_Validation_LogonScriptURL & "?ErrID=" & Server.URLEncode(4)
	Else
		Response.Redirect User_Auth_Validation_AccessErrorScriptURL & "?ErrID=" & Server.URLEncode(24) & "&ErrCode=" & Server.URLEncode("ReturnValue = UnknownValue") 'Unknown
	End If

	dim RecResultedUser
	If IsNull(User_Auth_Validation_RecSet.Fields(1).value) Then
		RecResultedUser = ""
	Else
		RecResultedUser = User_Auth_Validation_RecSet.Fields(1).value
	End If
	If Not lcase(Session("System_Username")) = Lcase(RecResultedUser) Then
		Response.Redirect User_Auth_Validation_LogonScriptURL & "?su=" & server.urlencode(session("System_Username")) & "&ru=" & server.urlencode(RecResultedUser)
	End If

	Set User_Auth_Validation_Cmd.ActiveConnection = Nothing

	'Default-Sprache setzen
	If Request.QueryString("Lang") <> "" Then
		Session("CurLanguage") = CLng(Request.QueryString("Lang"))
	ElseIf Session("CurLanguage") = "" Or Session("CurLanguage") = 0 Then
		Session("CurLanguage") = 1
	End If
	CurLanguage = Session("CurLanguage")

	'Fehlerüberprüfung wieder abschalten
	On Error Goto 0
	Application.UnLock

	Session.Timeout = 240 '4 h
	IsLoggedOn = True

End Sub
%>