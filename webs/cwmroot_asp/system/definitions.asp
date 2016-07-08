<!--#include virtual="/sysdata/config.asp"-->
<!--#include virtual="/system/internationalization.asp"-->
<!--#include virtual="/sysdata/custom_internationalization.asp"-->
<%

'================================================
'==== System variables, constants, functions ====
'================================================
'==== Please do NOT CHANGE!!                 ====
'================================================

const System_ProductName = "camm WebManager Enterprise"
const System_Version = "4.10"
const System_Build = "206"
const System_Licence = "ah3dkjf7JHSLIeuzw2ah94kEMAq"

const User_Auth_Config_Paths_Login = "/sysdata/login/"
const User_Auth_Config_Paths_Administration = "/sysdata/admin/"
const User_Auth_Config_Paths_StandardIncludes = "/system/"
const User_Auth_Config_Paths_SystemData = "/sysdata/"

'General definitions
dim System_DebugLevel
dim CurLanguage
dim IsLoggedOn
dim User_Auth_Validation_NoRefererURL, User_Auth_Validation_LogonScriptURL, User_Auth_Validation_AfterLogoutURL, User_Auth_Validation_AccessErrorScriptURL, User_Auth_Validation_CreateUserAccountInternalURL, User_Auth_Validation_TerminateOldSessionScriptURL
dim User_Auth_Config_UserAuthMasterServer
dim User_Auth_Config_CurServerURL
dim User_Auth_Validation_CheckLoginURL
dim OfficialServerGroup_URL
dim OfficialServerGroup_AdminURL
dim OfficialServerGroup_AdminURL_SecurityAdminNotifications
dim OfficialServerGroup_Title
dim OfficialServerGroup_Company_FormerTitle
dim PageTitle
dim User_Auth_Config_Files_Administration_DefaultPageInAdminEMails

const adModeRead = 1
const adModeWrite = 2
const adModeReadWrite = 3
const adParamReturnValue = 4
const adParamInput = 1
const adParamOutput = 2
const adCmdStoredProc = 4
const adCmdTable = 2
const adCmdText = 1
const adOpenDynamic = 2
const adOpenForwardOnly = 0
const adOpenStatic = 3
const adOpenKeyset = 1
const adLockReadOnly = 1
const adLockOptimistic = 3
const adExecuteNoRecords = 128
'adArray 0x2000 Combine with another data type to indicate that the other data type is an array
const adBigInt = 20 '8-byte signed integer
const adBinary = 128 'Binary
const adBoolean = 11 'True or false Boolean
const adBSTR = 8 'Null-terminated character string
const adChapter = 136 '4-byte chapter value for a child recordset
const adChar = 129 'String
const adCurrency = 6 'Currency format
const adDate = 7 'Number of days since 12/30/1899
const adDBDate = 133 'YYYYMMDD date format
const adDBFileTime = 137 'Database file time
const adDBTime = 134 'HHMMSS time format
const adDBTimeStamp = 135 'YYYYMMDDHHMMSS date/time format
const adDecimal = 14 'Number with fixed precision and scale
const adDouble = 5 'Double precision floating-point
const adEmpty = 0 'no value
const adError = 10 '32-bit error code
const adFileTime = 64 'Number of 100-nanosecond intervals since 1/1/1601
const adGUID = 72 'Globally unique identifier
const adIDispatch = 9 'Currently not supported by ADO
const adInteger = 3 '4-byte signed integer
const adIUnknown = 13 'Currently not supported by ADO
const adLongVarBinary = 205 'Long binary value
const adLongVarChar = 201 'Long string value
const adLongVarWChar = 203 'Long Null-terminates string value
const adNumeric = 131 'Number with fixed precision and scale
const adPropVariant = 138 'PROPVARIANT automation
const adSingle = 4 'Single-precision floating-point value
const adSmallInt = 2 '2-byte signed integer
const adTinyInt = 16 '1-byte signed integer
const adUnsignedBigInt = 21 '8-byte unsigned integer
const adUnsignedInt = 19 '4-byte unsigned integer
const adUnsignedSmallInt = 18 '2-byte unsigned integer
const adUnsignedTinyInt = 17 '1-byte unsigned integer
const adUserDefined = 132 'User-defined variable
const adVarBinary = 204 'Binary value
const adVarChar = 200 'String
const adVariant = 12 'Automation variant
const adVarNumeric = 139 'Variable width exact numeric with signed scale
const adVarWChar = 202 'Null-terminated Unicode character string
const adWChar = 130 'Null-terminated Unicode character string

	'Will be removed in one of the next versions! Please use config.* instead!
	User_Auth_Config_CurServerURL = System_GetServerURL(GetCurrentServerIdentString)
	User_Auth_Config_UserAuthMasterServer = System_GetMasterServerURL(GetCurrentServerIdentString)
	User_Auth_Validation_NoRefererURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_UserAuthSystem & "index.aspx"
	User_Auth_Validation_LogonScriptURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_SystemData & "logon.aspx"
	User_Auth_Validation_AfterLogoutURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_SystemData & "logon.aspx?ErrID=44"
	User_Auth_Validation_AccessErrorScriptURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "access_error.aspx"
	User_Auth_Validation_CreateUserAccountInternalURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_SystemData & "createaccount.aspx"
	User_Auth_Validation_TerminateOldSessionScriptURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_Login & "forcelogin.aspx"
	User_Auth_Validation_CheckLoginURL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_Login & "checklogin.aspx"
	OfficialServerGroup_URL = User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_UserAuthSystem
	OfficialServerGroup_AdminURL = System_GetUserAdminServer_SystemURL(GetCurrentServerIdentString)
	OfficialServerGroup_AdminURL_SecurityAdminNotifications = OfficialServerGroup_AdminURL
	OfficialServerGroup_Title = System_GetServerGroupTitle(GetCurrentServerIdentString)
	OfficialServerGroup_Company_FormerTitle = System_GetServerConfig(GetCurrentServerIdentString, "AreaCompanyFormerTitle")
	User_Auth_Config_Files_Administration_DefaultPageInAdminEMails = "memberships.aspx"

Dim PasswordSecurity
dim PasswordComplexityValidationResult
dim IsAlreadyInitialized
System_Init()
%>
<script language="VBScript" runat="server">
Sub System_Init()
	If IsAlreadyInitialized = False Then

		'Check current language
		cammWebManager_Initialize

		'Setup password policies
		If Not IsObject(PasswordSecurity) Then
			Set PasswordSecurity = New WMPasswordSecurity
		End If
		PasswordSecurity.Init
		Set PasswordComplexityValidationResult = New CPasswordComplexityValidationResult

		'Setup customizations
		SetupAdditionalConfiguration()

		'Finish Initialization
		Response.CharSet = "UTF-8"
		LoadLanguageStrings CurLanguage
		IsAlreadyInitialized = True
	End If
End Sub


Function System_GetNextLogonURI(LoginNameOfUser)
dim NextLoginURI
dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd

	If LoginNameOfUser = "" Then Exit Function

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetToDoLogonList"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@Username", adVarWChar, adParamInput, len((LoginNameOfUser)), (LoginNameOfUser))
	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ScriptEngine_SessionID", adVarWChar, adParamInput, len(Session.SessionID), Session.SessionID)
	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ScriptEngine_ID", adInteger, adParamInput, 4, 1)
	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerID", adInteger, adParamInput, 4, System_GetCurrentServerID)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	NextLoginURI = ""

	If Not GetNextLogonURI_RecSet.EOF Then

		NextLoginURI = GetNextLogonURI_RecSet.Fields("ServerProtocol") & "://" & GetNextLogonURI_RecSet.Fields("ServerName")
		If Not IsNull(GetNextLogonURI_RecSet.Fields("ServerPort")) Then
			NextLoginURI = NextLoginURI & ":" & GetNextLogonURI_RecSet.Fields("ServerPort")
		End If
		NextLoginURI = NextLoginURI & User_Auth_Config_Paths_Login & GetNextLogonURI_RecSet.Fields("FileName_EngineLogin") & "?GUID=" & Server.URLEncode(GetNextLogonURI_RecSet.Fields("ScriptEngine_LogonGUID")) & "&User=" & Server.URLEncode(LoginNameOfUser) & "&Dat=" & Hour(Now) & Minute(Now) & Second(Now)
	End If

	System_GetNextLogonURI = NextLoginURI

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

End Function

Function System_GetCurrentServerID()
dim NextLoginURI
dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim MyCounter
dim Jump2RecordNo

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "SELECT ID FROM System_Servers WHERE IP = ?"
	GetNextLogonURI_Cmd.CommandType = adCmdText

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, GetCurrentServerIdentString)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	NextLoginURI = ""

	If Not GetNextLogonURI_RecSet.EOF Then
		System_GetCurrentServerID = CInt(GetNextLogonURI_RecSet.Fields("ID"))
	Else
		Err.Raise 10000, "", "Server configuration mismatch"
	End If

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

End Function

Function System_GetNextLogonURIOfUserAnonymous()
dim NextLoginURI
dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim MyCounter
dim Jump2RecordNo

	If Request.QueryString("LogonID") <> "" Then
		Jump2RecordNo = Request.QueryString("LogonID")
	Else
		Jump2RecordNo = 1
	End If

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetCurServerLogonList"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, GetCurrentServerIdentString)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	NextLoginURI = ""

	Do Until GetNextLogonURI_RecSet.EOF
		MyCounter = MyCounter + 1
		If CLng(MyCounter) >= CLng(Jump2RecordNo) Then
			NextLoginURI = GetNextLogonURI_RecSet.Fields("ServerProtocol") & "://" & GetNextLogonURI_RecSet.Fields("ServerName")
			If Not IsNull(GetNextLogonURI_RecSet.Fields("ServerPort")) Then
				NextLoginURI = NextLoginURI & ":" & GetNextLogonURI_RecSet.Fields("ServerPort")
			End If
			NextLoginURI = NextLoginURI & User_Auth_Config_Paths_Login & GetNextLogonURI_RecSet.Fields("FileName_EngineLogin") & "?LogonID=" & MyCounter + 1
			Exit Do
		End If
		GetNextLogonURI_RecSet.MoveNext
	Loop

	System_GetNextLogonURIOfUserAnonymous = NextLoginURI

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

End Function

Function System_GetServerURL(ServerIP)

'Use cached data if possible
if Application("CWM_ServerURL_ServerIP") = ServerIP Then
	System_GetServerURL = CStr(Application("CWM_ServerURL_Result"))
	exit function
end if

dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim ServerURL

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServerConfiG"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	ServerURL = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		ServerURL = GetNextLogonURI_RecSet.Fields("ServerProtocol") & "://" & GetNextLogonURI_RecSet.Fields("ServerName")
		If Not IsNull(GetNextLogonURI_RecSet.Fields("ServerPort")) Then
			ServerURL = ServerURL & ":" & GetNextLogonURI_RecSet.Fields("ServerPort")
		End If
	End If
	System_GetServerURL = ServerURL

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

	'Cache for next query
	Application("CWM_ServerURL_ServerIP") = ServerIP
	Application("CWM_ServerURL_Result") = ServerURL

End Function

dim System_GetMasterServerURL_Cache_Result
dim System_GetMasterServerURL_Cache_ServerIP
Function System_GetMasterServerURL(ServerIP)

'Use cached data if possible
if Application("CWM_MasterServerURL_ServerIP") = ServerIP Then
	System_GetMasterServerURL = CStr(Application("CWM_MasterServerURL_Result"))
	exit function
end if

dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim ServerURL

	If System_GetMasterServerURL_Cache_Result <> "" And System_GetMasterServerURL_Cache_ServerIP = ServerIP Then
		System_GetMasterServerURL = System_GetMasterServerURL_Cache_Result
		Exit Function
	End If

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServerConfIg"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	ServerURL = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		ServerURL = GetNextLogonURI_RecSet.Fields("MasterServerProtocol") & "://" & GetNextLogonURI_RecSet.Fields("MasterServerName")
		If Not IsNull(GetNextLogonURI_RecSet.Fields("MasterServerPort")) Then
			ServerURL = ServerURL & ":" & GetNextLogonURI_RecSet.Fields("MasterServerPort")
		End If
	End If
	System_GetMasterServerURL = ServerURL
	System_GetMasterServerURL_Cache_Result = ServerURL
	System_GetMasterServerURL_Cache_ServerIP = ServerIP

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

	'Cache for next query
	Application("CWM_MasterServerURL_ServerIP") = ServerIP
	Application("CWM_MasterServerURL_Result") = ServerURL

End Function

Function System_GetUserAdminServer_SystemURL(ServerIP)

'Use cached data if possible
if Application("CWM_AdminServerURL_ServerIP") = ServerIP Then
	System_GetUserAdminServer_SystemURL = CStr(Application("CWM_AdminServerURL_Result"))
	exit function
end if

dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim ServerURL

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServerConFig"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	ServerURL = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		ServerURL = GetNextLogonURI_RecSet.Fields("UserAdminServerProtocol") & "://" & GetNextLogonURI_RecSet.Fields("UserAdminServerName")
		If Not IsNull(GetNextLogonURI_RecSet.Fields("UserAdminServerPort")) Then
			ServerURL = ServerURL & ":" & GetNextLogonURI_RecSet.Fields("UserAdminServerPort")
		End If
		ServerURL = ServerURL & User_Auth_Config_Paths_Administration
	End If
	System_GetUserAdminServer_SystemURL = ServerURL

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

	'Cache for next query
	Application("CWM_AdminServerURL_ServerIP") = ServerIP
	Application("CWM_AdminServerURL_Result") = ServerURL

End Function

Function System_GetServerGroupTitle(ServerIP)

'Use cached data if possible
if Application("CWM_ServerGroupTitle_ServerIP") = ServerIP Then
	System_GetServerGroupTitle = CStr(Application("CWM_ServerGroupTitle_Result"))
	exit function
end if

dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim ServerURL

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServerCoNfig"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	ServerURL = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		ServerURL = GetNextLogonURI_RecSet.Fields("ServerGroupDescription")
	End If
	System_GetServerGroupTitle = ServerURL

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

	'Cache for next query
	Application("CWM_ServerGroupTitle_ServerIP") = ServerIP
	Application("CWM_ServerGroupTitle_Result") = ServerURL

End Function

Function System_GetServerGroupImageSmallAddr(ServerIP)
dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim MyBuffer

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServerCOnfig"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	MyBuffer = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		MyBuffer = GetNextLogonURI_RecSet.Fields("ServerGroupImageSmall")
	End If
	System_GetServerGroupImageSmallAddr = MyBuffer

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

End Function

Function System_GetServerConfig(ServerIP, PropertyName)

'Use cached data if possible
if IsNull(Application("CWM_ServerConfig_" & PropertyName & "_.:_" & ServerIP)) Then
	System_GetServerConfig = Null
	exit function
else
	if Application("CWM_ServerConfig_" & PropertyName & "_.:_" & ServerIP) <> "" Then
		System_GetServerConfig = Application("CWM_ServerConfig_" & PropertyName & "_.:_" & ServerIP)
		exit function
	end if
end if

dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim MyBuffer

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServeRConfig"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	MyBuffer = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		MyBuffer = GetNextLogonURI_RecSet.Fields(PropertyName)
	End If
	System_GetServerConfig = MyBuffer

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

	'Cache for next query
	Application("CWM_ServerConfig_" & PropertyName & "_.:_" & ServerIP) = MyBuffer

End Function

Function System_GetServerGroupImageBigAddr(ServerIP)
dim GetNextLogonURI_DBConn
dim GetNextLogonURI_RecSet
dim GetNextLogonURI_Cmd
dim MyBuffer

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	Set GetNextLogonURI_RecSet = Server.CreateObject("ADODB.Recordset")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_GetServerConfig"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, ServerIP)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	Set GetNextLogonURI_RecSet = GetNextLogonURI_Cmd.Execute

	MyBuffer = ""
	If Not GetNextLogonURI_RecSet.EOF Then
		MyBuffer = GetNextLogonURI_RecSet.Fields("ServerGroupImageBig")
	End If
	System_GetServerGroupImageBigAddr = MyBuffer

	GetNextLogonURI_RecSet.Close
	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_RecSet = Nothing
	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

End Function

Function System_IsUserAuthorizedForApp(AppTitle)
dim MyBuffer
dim GetNextLogonURI_DBConn
dim GetNextLogonURI_Cmd
dim ServerURL

	If AppTitle = "" Then
		System_IsUserAuthorizedForApp = False
		Exit Function
	End If
	If LCase(AppTitle) = "@@anonymous" Or (LCase(AppTitle) = "@@public" And Session("System_Username") <> "") Then
		System_IsUserAuthorizedForApp = True
		Exit Function
	End If

	'Create connection
	Set GetNextLogonURI_DBConn = Server.CreateObject("ADODB.Connection")
	GetNextLogonURI_DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set GetNextLogonURI_Cmd = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	GetNextLogonURI_Cmd.CommandText = "Public_UserIsAuthorizedForApp"
	GetNextLogonURI_Cmd.CommandType = adCmdStoredProc

	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ReturnValue", adInteger, adParamReturnValue, 32, 0)
	If Session("System_Username") = "" Then
		GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@Username", adVarWChar, adParamInput, 50, Null)
	Else
		GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@Username", adVarWChar, adParamInput, 50, Session("System_Username"))
	End If
	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@WebApplication", adVarWChar, adParamInput, 255, AppTitle)
	GetNextLogonURI_Cmd.Parameters.Append GetNextLogonURI_Cmd.CreateParameter("@ServerIP", adVarWChar, adParamInput, 32, GetCurrentServerIdentString)

	'Create recordset by executing the command
	Set GetNextLogonURI_Cmd.ActiveConnection = GetNextLogonURI_DBConn
	GetNextLogonURI_Cmd.Execute ,,adExecuteNoRecords

	MyBuffer = GetNextLogonURI_Cmd.Parameters("@ReturnValue").Value
	If MyBuffer <> 1 Then
		System_IsUserAuthorizedForApp = False
	Else
		System_IsUserAuthorizedForApp = True
	End If

	GetNextLogonURI_DBConn.Close

	Set GetNextLogonURI_Cmd = Nothing
	Set GetNextLogonURI_DBConn = Nothing

End Function

Function System_SendEMail(RcptName, RcptAddress, MsgSubject, MsgBody, SenderName, SenderAddress)
	System_SendEMail = System_SendEMailEx(RcptName, RcptAddress, MsgSubject, MsgBody, "", SenderName, SenderAddress, "")
End Function

Function System_QueueEMailEx (RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
    dim rcpt
    if RcptName = "" Then
        rcpt = rcptaddress
    else
        rcpt = RcptName & " <" & RcptAddress & ">"
    end if
    System_QueueEMailEx = System_SendEMail_MultipleRcpts (rcpt, "", "", MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
End Function

Function System_QueueEMail_MultipleRcpts (RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
If MsgCharset = "" Then MsgCharset = "UTF-8"
dim XmlOpener
dim XmlCloser
XmlOpener = "<root>" & vbNewLine & _
	"  <xs:schema id=""root"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">" & vbNewLine & _
	"    <xs:element name=""root"" msdata:IsDataSet=""true"" msdata:UseCurrentLocale=""true"">" & vbNewLine & _
	"      <xs:complexType>" & vbNewLine & _
	"        <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">" & vbNewLine & _
	"          <xs:element name=""version"">" & vbNewLine & _
	"            <xs:complexType>" & vbNewLine & _
	"              <xs:sequence>" & vbNewLine & _
	"                <xs:element name=""key"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"                <xs:element name=""value"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"              </xs:sequence>" & vbNewLine & _
	"            </xs:complexType>" & vbNewLine & _
	"          </xs:element>" & vbNewLine & _
	"          <xs:element name=""headers"">" & vbNewLine & _
	"            <xs:complexType>" & vbNewLine & _
	"              <xs:sequence>" & vbNewLine & _
	"                <xs:element name=""key"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"                <xs:element name=""value"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"              </xs:sequence>" & vbNewLine & _
	"            </xs:complexType>" & vbNewLine & _
	"          </xs:element>" & vbNewLine & _
	"          <xs:element name=""attachments"">" & vbNewLine & _
	"            <xs:complexType>" & vbNewLine & _
	"              <xs:sequence>" & vbNewLine & _
	"                <xs:element name=""Placeholder"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"                <xs:element name=""FileName"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"                <xs:element name=""FileData"" type=""xs:base64Binary"" minOccurs=""0"" />" & vbNewLine & _
	"                <xs:element name=""OriginFileNameBeforePlaceholderValue"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"              </xs:sequence>" & vbNewLine & _
	"            </xs:complexType>" & vbNewLine & _
	"          </xs:element>" & vbNewLine & _
	"          <xs:element name=""message"">" & vbNewLine & _
	"            <xs:complexType>" & vbNewLine & _
	"              <xs:sequence>" & vbNewLine & _
	"                <xs:element name=""key"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"                <xs:element name=""value"" type=""xs:string"" minOccurs=""0"" />" & vbNewLine & _
	"              </xs:sequence>" & vbNewLine & _
	"            </xs:complexType>" & vbNewLine & _
	"          </xs:element>" & vbNewLine & _
	"        </xs:choice>" & vbNewLine & _
	"      </xs:complexType>" & vbNewLine & _
	"      <xs:unique name=""Constraint1"">" & vbNewLine & _
	"        <xs:selector xpath="".//version"" />" & vbNewLine & _
	"        <xs:field xpath=""key"" />" & vbNewLine & _
	"      </xs:unique>" & vbNewLine & _
	"      <xs:unique name=""headers_Constraint1"" msdata:ConstraintName=""Constraint1"">" & vbNewLine & _
	"        <xs:selector xpath="".//headers"" />" & vbNewLine & _
	"        <xs:field xpath=""key"" />" & vbNewLine & _
	"      </xs:unique>" & vbNewLine & _
	"      <xs:unique name=""message_Constraint1"" msdata:ConstraintName=""Constraint1"">" & vbNewLine & _
	"        <xs:selector xpath="".//message"" />" & vbNewLine & _
	"        <xs:field xpath=""key"" />" & vbNewLine & _
	"      </xs:unique>" & vbNewLine & _
	"    </xs:element>" & vbNewLine & _
	"  </xs:schema>" & vbNewLine & _
	"  <version>" & vbNewLine & _
	"    <key>NetLibrary</key>" & vbNewLine & _
	"    <value>4.10.164.1011</value>" & vbNewLine & _
	"  </version>" & vbNewLine & _
	"  <headers>" & vbNewLine & _
	"    <key>X-Priority</key>" & vbNewLine & _
	"    <value>3</value>" & vbNewLine & _
	"  </headers>" & vbNewLine & _
	"  <headers>" & vbNewLine & _
	"    <key>X-MSMail-Priority</key>" & vbNewLine & _
	"    <value>Normal</value>" & vbNewLine & _
	"  </headers>" & vbNewLine & _
	"  <headers>" & vbNewLine & _
	"    <key>Importance</key>" & vbNewLine & _
	"    <value>Normal</value>" & vbNewLine & _
	"  </headers>" & vbNewLine & _
	""
XmlCloser = "</root>"

Dim MessageXml
MessageXml = XmlOpener & _
	"  <message>" & vbNewLine & _
	"    <key>FromAddress</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(SenderName, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>FromName</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(SenderName, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>To</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(RcptAddresses_To, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>Cc</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(RcptAddresses_CC, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>Bcc</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(RcptAddresses_BCC, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>Subject</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(MsgSubject, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>Charset</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(MsgCharset, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>TextBody</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(MsgTextBody, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"  <message>" & vbNewLine & _
	"    <key>HtmlBody</key>" & vbNewLine & _
	"    <value>" & XmlEncodeValue(MsgHTMLBody, False) & "</value>" & vbNewLine & _
	"  </message>" & vbNewLine & _
	"" & _
    XmlCloser

dim Sql
Sql = "DECLARE @Username nvarchar(50)" & vbnewline & _
    "SELECT @Username = ?" & vbnewline & _
    "DECLARE @UserID int" & vbnewline & _
    "SELECT @UserID = ID" & vbnewline & _
    "FROM Benutzer" & vbnewline & _
    "WHERE LoginName = @Username" & vbnewline & _
    "INSERT INTO Log_eMailMessages (UserID, data, State, [DateTime])" & vbnewline & _
    "VALUES (IsNull(@UserID, -1), @Data, 1, GETDATE())"

dim DBConn
dim Cmd
dim MyBuffer

on error resume next

	'Create connection
	Set DBConn = Server.CreateObject("ADODB.Connection")
	Set RecSet = Server.CreateObject("ADODB.Recordset")
	DBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set Cmd = Server.CreateObject("ADODB.Command")

    'Embed sql param into sql command because local ntext variables are not allowed and ADO doesn't provide another fast solution to develop
    sql = replace(sql, "@Data", "N'" & Replace(MessageXml, "'", "''") & "'")

	'Get parameter value and append parameter
	Cmd.CommandText = sql
	Cmd.CommandType = adCmdText

    dim user
    user = CStr(Trim(Mid(Session("System_Username"),1,50)))
    if user = "" Then user = Null
	Cmd.Parameters.Append Cmd.CreateParameter("@Username", adVarWChar, adParamInput, 50, user)
	'Cmd.Parameters.Append Cmd.CreateParameter("@Data", adLongVarWChar, adParamInput, , MessageXml)
        
	'Create recordset by executing the command
	Set Cmd.ActiveConnection = DBConn
	Cmd.Execute,,adExecuteNoRecords

	DBConn.Close

	Set Cmd = Nothing
	Set DBConn = Nothing

if err <> 0 then
    System_QueueEMail_MultipleRcpts = False
else 
    System_QueueEMail_MultipleRcpts = True
end if
err.Clear
on error goto 0

End Function

Function XmlEncodeValue (text, inAttribute)
    Dim Result
    Dim length
    length = Len(text)
    Dim counter 
    counter = 0
    Dim ch
    Do
        counter = counter + 1
        If (counter > length) Then
            XmlEncodeValue = Result
            Exit Function
        End If
        ch = mid([text], counter, 1)
        Select Case ch
            Case ChrW(9)
                Result = Result & ch
            Case ChrW(10), ChrW(13)
                If inAttribute Then
                    Result = Result & "&#" & AscW(ch) & ";"
                Else
                    Result = Result & ch
                End If
            Case """"
                Result = Result & "&quot;"
            Case "&"
                Result = Result & "&amp;"
            Case "'"
                Result = Result & "&apos;"
            Case "<"
                Result = Result & "&lt;"
            Case ">"
                Result = Result & "&gt;"
            Case Else
                If AscW(ch) >= &H20 And AscW(ch) < 128 Then
                    Result = Result & ch
                Else
                    Result = Result & "&#" & AscW(ch) & ";"
                End If
        End Select
    Loop
End Function

Function System_SendEMailEx(RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
	System_SendEMailEx = System_QueueEMailEx (RcptName, RcptAddress, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
End Function

Function System_SendEMail_MultipleRcpts (RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
	System_SendEMail_MultipleRcpts = System_QueueEMail_MultipleRcpts (RcptAddresses_To, RcptAddresses_CC, RcptAddresses_BCC, MsgSubject, MsgTextBody, MsgHTMLBody, SenderName, SenderAddress, MsgCharset)
End Function

Function sprintf(ByVal message, value1, value2, value3, value4, value5, value6, value7, value8, value9, value10, value11, value12)
'************************************************************************
'***  ATTENTION: ASP does not provide any string format functionality ***
'***             To use string format capabilities please use ASP.NET ***
'************************************************************************
Const errpfNoClosingBracket = -2147221503 ' + 1
Const errpfMissingValue = -2147221502 '+ 2
Const errpfVBScriptNotSupported = -2147221501 ' + 3
'*** Special chars ***
'   \t = TAB
'   \r = CR
'   \n = CRLF
'   \[ = [
'   \\ = \
'*** Placeholder ***
'   []      = value of parameter list will be skipped
'   [*]     = value will be inserted without any special format
'   [###]   = "###" stands for a format string (further details in your SDK help for command "Format()")
'   [n:...] = n as 1..9: uses the n-th parameter; the internal parameter index will not be increased!
'             There can be a format string behind the ":"

    'Because of compatibility issues there is a need of a workaround for "paramarray Values()"
    Dim Values()
    Dim ValueCount
    If Not IsObject(value12) Then
        ReDim Values(11)
        ValueCount = 12
    ElseIf Not IsObject(value11) Then
        ReDim Values(10)
        ValueCount = 11
    ElseIf Not IsObject(value10) Then
        ReDim Values(9)
        ValueCount = 10
    ElseIf Not IsObject(value9) Then
        ReDim Values(8)
        ValueCount = 9
    ElseIf Not IsObject(value8) Then
        ReDim Values(7)
        ValueCount = 8
    ElseIf Not IsObject(value7) Then
        ReDim Values(6)
        ValueCount = 7
    ElseIf Not IsObject(value6) Then
        ReDim Values(5)
        ValueCount = 6
    ElseIf Not IsObject(value5) Then
        ReDim Values(4)
        ValueCount = 5
    ElseIf Not IsObject(value4) Then
        ReDim Values(3)
        ValueCount = 4
    ElseIf Not IsObject(value3) Then
        ReDim Values(2)
        ValueCount = 3
    ElseIf Not IsObject(value2) Then
        ReDim Values(1)
        ValueCount = 2
    ElseIf Not IsObject(value1) Then
        ReDim Values(0)
        ValueCount = 1
    Else
        ReDim Values(0)
        ValueCount = 0
    End If
    If ValueCount >= 1 Then Values(0) = value1
    If ValueCount >= 2 Then Values(1) = value2
    If ValueCount >= 3 Then Values(2) = value3
    If ValueCount >= 4 Then Values(3) = value4
    If ValueCount >= 5 Then Values(4) = value5
    If ValueCount >= 6 Then Values(5) = value6
    If ValueCount >= 7 Then Values(6) = value7
    If ValueCount >= 8 Then Values(7) = value8
    If ValueCount >= 9 Then Values(8) = value9
    If ValueCount >= 10 Then Values(9) = value10
    If ValueCount >= 11 Then Values(10) = value11
    If ValueCount >= 12 Then Values(11) = value12

    Dim i, iv, orig_iv, iob, icb
    i = 0: iv = 0: orig_iv = 0: iob = 0: icb = 0
    Dim messageParts(), part, formatString

    message = Replace(message, "\\", "[\]")
    message = Replace(message, "\t", vbTab)
    message = Replace(message, "\r", vbCr)
    message = Replace(message, "\n", vbCrLf)
    message = Replace(message, "\[", "[(]")

    iob = 1
    Do
        iob = InStr(iob, message, "[")
        If iob = 0 Then Exit Do

        icb = InStr(iob + 1, message, "]")
        If icb = 0 Then
            Err.Raise errpfNoClosingBracket, "printf", "Missing ']' after '[' at position " & iob & "!"
        End If

        formatString = Mid(message, iob + 1, icb - iob - 1)

        If InStr("123456789", Mid(formatString, 1, 1)) > 0 And Mid(formatString, 2, 1) = ":" Then
            orig_iv = iv

            iv = CInt(Mid(formatString, 1, 1)) - 1
            If iv > UBound(Values) Then iv = UBound(Values)

            formatString = Mid(formatString, 3)
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
                If iv > UBound(Values) Then Err.Raise errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!"
                formatString = Values(iv)
            Case Else 'with user specified format string
                If iv > UBound(Values) Then Err.Raise errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!"
                formatString = Values(iv) 'Format(values(iv), formatString)
        End Select

        message = Left(message, iob - 1) & formatString & Mid(message, icb + 1)
        If orig_iv >= 0 Then
            iob = iob + Len(formatString) + 2
        Else
            iob = iob + Len(formatString) + 0
        End If

        If orig_iv >= 0 Then
            iv = orig_iv
        Else
            iv = iv + 1
        End If
    Loop

    sprintf = message

End Function

Function System_IsValidLanguageID (LanguageID)
Dim MyDBConn
Dim MyCmdObj
Dim MyRecSet

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	MyCmdObj.CommandText = "SELECT ID FROM Languages WHERE ID = " & CLng(LanguageID)
	MyCmdObj.CommandType = adCmdText

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	Set MyRecSet = MyCmdObj.Execute
	If MyRecSet.EOF Then
		System_IsValidLanguageID = False
	Else
		System_IsValidLanguageID = True
	End If

	MyRecSet.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

End Function

Function System_GetSessionValue (SettingName)
Dim MyDBConn
Dim MyCmdObj
Dim MyRecSet

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	MyCmdObj.CommandText = "SELECT * FROM [System_SessionValues] WHERE SessionID IN (SELECT System_SessionID FROM Benutzer WHERE LoginName = N'" & system_getsqltextvalue (Username) & "') AND VarName = N'" & system_getsqltextvalue (SettingName) & "'"
	MyCmdObj.CommandType = adCmdText

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	Set MyRecSet = MyCmdObj.Execute

	If MyRecSet.State = 0 Then 'closed recordset
		System_GetSessionValue = Null
	ElseIf MyRecSet.EOF Then
		System_GetSessionValue = Null
	ElseIf MyRecset.Fields("VarType") = 1 Then
		System_GetSessionValue = MyRecSet.Fields("ValueInt")
	ElseIf MyRecset.Fields("VarType") = 2 Then
		System_GetSessionValue = MyRecSet.Fields("ValueNText")
	ElseIf MyRecset.Fields("VarType") = 3 Then
		System_GetSessionValue = MyRecSet.Fields("ValueFloat")
	ElseIf MyRecset.Fields("VarType") = 4 Then
		System_GetSessionValue = MyRecSet.Fields("ValueDecimal")
	ElseIf MyRecset.Fields("VarType") = 5 Then
		System_GetSessionValue = MyRecSet.Fields("ValueDateTime")
	ElseIf MyRecset.Fields("VarType") = 6 Then
		System_GetSessionValue = MyRecSet.Fields("ValueImage")
	ElseIf MyRecset.Fields("VarType") = 7 Then
		System_GetSessionValue = MyRecSet.Fields("ValueBool")
	End If

	MyRecSet.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

End Function

private function System_GetSQLTextValue(Value)
	system_getsqltextvalue = replace(value, "'", "''")
end function

Function System_SetSessionValue (SettingName, SettingValue)
Dim MyDBConn
Dim MyRecSet
Dim MyRecSet2
Dim CurUserSessionID

	System_SetSessionValue = True

	If Session("System_Username") = "" Then
		Err.Raise 10010,"System_SetSessionValue","No valid logon in this user session yet."
		Exit Function
	End If

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyRecSet = Server.CreateObject("ADODB.RecordSet")
	Set MyRecSet2 = Server.CreateObject("ADODB.RecordSet")

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	MyCmdObj.CommandText = "SELECT System_SessionID FROM Benutzer WHERE LoginName = N'" & system_getsqltextvalue (Session("System_Username")) & "'"
	MyCmdObj.CommandType = adCmdText

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	Set MyRecSet2 = MyCmdObj.Execute

	'Get Last System_SessionID of user
	CurUserSessionID = MyRecSet2.Fields("System_SessionID")
	If IsNull(CurUserSessionID) Then
		System_SetSessionValue = False
		Exit Function
	End If


	'Create recordset by executing the command
	MyRecSet.Open "SELECT * FROM [System_SessionValues] WHERE SessionID = " & CurUserSessionID & " AND VarName = N'" & system_getsqltextvalue (SettingName) & "'", MyDBConn,adOpenKeyset,adLockOptimistic

	If MyRecSet.EOF then
		'Is it a new record?
		MyRecSet.AddNew
		MyRecSet.Fields("VarName") = SettingName
		MyRecSet.Fields("SessionID") = CurUserSessionID
	End If

	If VarType(SettingValue) = VBBoolean Then
		MyRecSet.Fields("ValueBool") = SettingValue
		MyRecSet.Fields("VarType") = 7
	ElseIf VarType(SettingValue) = VBString Then
		MyRecSet.Fields("ValueNText") = SettingValue
		MyRecSet.Fields("VarType") = 2
	ElseIf VarType(SettingValue) = VBDate Then
		MyRecSet.Fields("ValueDateTime") = SettingValue
		MyRecSet.Fields("VarType") = 5
	ElseIf VarType(SettingValue) = VBCurrency Or VarType(SettingValue) = VBDecimal Then
		MyRecSet.Fields("ValueDecimal") = SettingValue
		MyRecSet.Fields("VarType") = 4
	ElseIf VarType(SettingValue) = VBDouble Then
		MyRecSet.Fields("ValueFloat") = CDbl(SettingValue)
		MyRecSet.Fields("VarType") = 3
	ElseIf VarType(SettingValue) = VBNull Then
		MyRecSet.Fields("ValueImage") = SettingValue
		MyRecSet.Fields("VarType") = 6
	ElseIf VarType(SettingValue) = VBByte Or VarType(SettingValue) = VBLong Or VarType(SettingValue) = VBInteger Then
		MyRecSet.Fields("ValueInt") = SettingValue
		MyRecSet.Fields("VarType") = 1
	Else 'e.g. VarType(SettingValue) = VBSingle [because of small differences between origin and saved values]
		MyRecSet.Cancel
		System_SetSessionValue = False
		Err.Raise 10009,"System_SetSessionValue","Saving of given variant type (VarType-ID: " & VarType(SettingValue) & ") not supported"
	End If

	'Save changes
	MyRecSet.Update

	MyRecSet.Close
	MyRecSet2.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyRecSet2 = Nothing
	Set MyDBConn = Nothing

End Function

Function System_GetUserLogonServers(UserID)
Dim MyDBConn
Dim MyCmdObj
Dim MyRecSet
dim MyResult
dim AllServersAreDisabled

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
	MyCmdObj.CommandText = "select dbo.System_ServerGroups.ServerGroup, dbo.System_Servers.ServerProtocol, dbo.System_Servers.ServerName, dbo.System_Servers.ServerPort from (((dbo.System_AccessLevels inner join dbo.Benutzer on dbo.System_AccessLevels.ID = dbo.Benutzer.AccountAccessability) inner join dbo.System_ServerGroupsAndTheirUserAccessLevels on dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel) inner join dbo.System_ServerGroups on dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID) inner join dbo.System_Servers on dbo.System_ServerGroups.MasterServer = dbo.System_Servers.ID where dbo.Benutzer.id = " & clng(UserID) & " and dbo.System_Servers.enabled = 1"
	MyCmdObj.CommandType = adCmdText

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	Set MyRecSet = MyCmdObj.Execute

	If MyRecSet.EOF Then
		MyRecSet.Close

		Set MyCmdObj = Server.CreateObject("ADODB.Command")

		'Get parameter value and append parameter
	MyCmdObj.CommandText = "select dbo.System_ServerGroups.ServerGroup, dbo.System_Servers.ServerProtocol, dbo.System_Servers.ServerName, dbo.System_Servers.ServerPort from (((dbo.System_AccessLevels inner join dbo.Benutzer on dbo.System_AccessLevels.ID = dbo.Benutzer.AccountAccessability) inner join dbo.System_ServerGroupsAndTheirUserAccessLevels on dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel) inner join dbo.System_ServerGroups on dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID) inner join dbo.System_Servers on dbo.System_ServerGroups.MasterServer = dbo.System_Servers.ID where dbo.Benutzer.id = " & clng(UserID)
	MyCmdObj.CommandType = adCmdText

		'Create recordset by executing the command
		Set MyCmdObj.ActiveConnection = MyDBConn
		Set MyRecSet = MyCmdObj.Execute
		AllServersAreDisabled = True
	Else
		AllServersAreDisabled = False
	End If

dim MyServerURL
dim MyServerGroupTitle
dim MyRowCounter

	Do Until MyRecSet.EOF
		MyRowCounter = MyRowCounter + 1
		MyServerGroupTitle = MyRecSet.Fields("ServerGroup")
		MyServerURL = MyRecSet.Fields("ServerProtocol") & "://" & MyRecSet.Fields("ServerName")
		If Not IsNull(MyRecSet.Fields("ServerPort")) Then
			If Not MyRecSet.Fields("ServerName") = "" Then
				MyServerURL = MyServerURL & ":" & MyRecSet.Fields("ServerPort")
			End If
		End If
		MyServerURL = MyServerURL & "/"

		MyRecSet.MoveNext

		If MyRowCounter = 1 And MyRecSet.EOF Then
			'1 server only
			MyResult = MyServerURL
		ElseIf MyRowCounter = 1 And Not MyRecSet.EOF Then
			'several servers found, this is the 1st server
			MyResult = MyServerURL & " (" & MyServerGroupTitle & ")"
		Else
			MyResult = MyResult & Chr(13) & Chr(10) & MyServerURL & " (" & MyServerGroupTitle & ")"
			'several servers found, this is one of them (not the first)
		End If
		If AllServersAreDisabled Then
			MyResult = MyResult & chr(13) & chr(10) & UserManagementMasterServerAvailableInNearFuture & chr(13) & chr(10)
		End If
	Loop

	MyRecSet.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

	System_GetUserLogonServers = MyResult

End Function

Function System_Debug_GetComponentsAndVersions()
	System_Debug_GetComponentsAndVersions = ""
End Function


class CPasswordComplexityValidationResult
		public property get Success
			Success = -1
		end property
		public property get Failure_Unspecified
			Failure_Unspecified = 0
		end property
		public property get Failure_LengthMinimum
			Failure_LengthMinimum = 1
		end property
		public property get Failure_LengthMaximum
			Failure_LengthMaximum = 2
		end property
		public property get Failure_HigherPasswordComplexityRequired
			Failure_HigherPasswordComplexityRequired = 3
		end property
		public property get Failure_NotAllowed_PartOfProfileInformation
			Failure_NotAllowed_PartOfProfileInformation  = 4
		end property
End class

Class WMPasswordSecurity
            Public Sub Init()
            End Sub
            Dim MyInspectionSeverities()
            Public Property Get InspectionSeverities(ByVal AccessLevelID)
					on error resume next
					dim curubound
					curubound = ubound(MyInspectionSeverities)
					if err<>0 then curubound = -1
					on error goto 0
                If curubound = -1 Then
                    ReDim Preserve MyInspectionSeverities(AccessLevelID)
                    Set MyInspectionSeverities(AccessLevelID) = New WMPasswordSecurityInspectionSeverity
                    MyInspectionSeverities(AccessLevelID).Init
                Else
                    If UBound(MyInspectionSeverities) < AccessLevelID Then
                        ReDim Preserve MyInspectionSeverities(AccessLevelID)
                    End If
                    If Not IsObject(MyInspectionSeverities(AccessLevelID)) Then
                        Set MyInspectionSeverities(AccessLevelID) = New WMPasswordSecurityInspectionSeverity
                        MyInspectionSeverities(AccessLevelID).Init
                    End If
                End If
                Set InspectionSeverities = MyInspectionSeverities(AccessLevelID)
            End Property
            Public Property Set InspectionSeverities(AccessLevelID, ByVal Value)
					on error resume next
					dim curubound
					curubound = ubound(MyInspectionSeverities)
					if err<>0 then curubound = -1
					on error goto 0
                If curubound = -1 Then
                    ReDim MyInspectionSeverities(AccessLevelID + 1)
                    Set MyInspectionSeverities(AccessLevelID) = Value
                Else
                    If UBound(MyInspectionSeverities) < AccessLevelID Then
                        ReDim Preserve MyInspectionSeverities(AccessLevelID + 1)
                        Set MyInspectionSeverities(AccessLevelID) = Value
                    Else
                        Set MyInspectionSeverities(AccessLevelID) = Value
                    End If
                End If
            End Property
End Class

class WMPasswordSecurityInspectionSeverity

            Dim MyRequiredComplexityPoints
            Dim MyRequiredPasswordLength
            Dim MyRequiredMaximumPasswordLength
            Dim MyRecommendedPasswordLength

            Public Property Get RequiredPasswordLength()
                RequiredPasswordLength = MyRequiredPasswordLength
            End Property
            Public Property Let RequiredPasswordLength(Value)
                MyRequiredPasswordLength = Value
            End Property
            Public Property Get RequiredMaximumPasswordLength()
                RequiredMaximumPasswordLength = MyRequiredMaximumPasswordLength
            End Property
            Public Property Let RequiredMaximumPasswordLength(Value)
                MyRequiredMaximumPasswordLength = Value
            End Property
            Public Property Get RequiredComplexityPoints()
                RequiredComplexityPoints = MyRequiredComplexityPoints
            End Property
            Public Property Let RequiredComplexityPoints(Value)
                MyRequiredComplexityPoints = Value
            End Property
            Public Property Get RecommendedPasswordLength()
                RecommendedPasswordLength = MyRecommendedPasswordLength
            End Property
            Public Property Let RecommendedPasswordLength(Value)
                MyRecommendedPasswordLength = Value
            End Property

				Public Sub Init()
					MyRequiredComplexityPoints = 1
            	MyRequiredPasswordLength = 3
            	MyRequiredMaximumPasswordLength = 20
            	MyRecommendedPasswordLength = 8
				End Sub

            Function ValidatePasswordComplexity(ByVal Password, ByVal LoginName, ByVal FirstName, ByVal LastName)
                Dim Result
                Const specchars = "@#$?!,.+~%""=<>;:()_-\/*&"
                Const specchars_increasingsecurity = " äöüÄÖÜß§{}[]´`^°éóíúáàòìùèêîôûâ|'µ€²³"
                Const SmallLetters = "abcdefghijklmnopqrstuvwxyz"
                Const LargeLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                Const Numbers = "0123456789"
                Const CharsEqualOrGreaterThanAscW = 128

                'Password length
                If Len(Password) < MyRequiredPasswordLength Then
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_LengthMinimum
                    Exit Function
                ElseIf Len(Password) > MyRequiredMaximumPasswordLength Then
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_LengthMaximum
                    Exit Function
                End If

                'Analyse the complexity
                Dim CollectedComplexityPoints
                Dim MyCounter
                Dim CollectedComplexityPoints_Numbers
                Dim CollectedComplexityPoints_CharsEqualOrGreaterThanAscW
                Dim CollectedComplexityPoints_LargeLetters
                Dim CollectedComplexityPoints_SmallLetters
                Dim CollectedComplexityPoints_SpecChars
                Dim CollectedComplexityPoints_IncreaseingSecurity
                Dim CollectedComplexityPoints_Oth
                Dim CollectedComplexityPoints_Other
                Dim CollectedComplexityPoints_NotAllowed_PartOfProfileInformation
                CollectedComplexityPoints_Numbers = 0
                CollectedComplexityPoints_CharsEqualOrGreaterThanAscW = 0
                CollectedComplexityPoints_LargeLetters = 0
                CollectedComplexityPoints_SmallLetters = 0
                CollectedComplexityPoints_SpecChars = 0
                CollectedComplexityPoints_IncreaseingSecurity = 0
                CollectedComplexityPoints_Other = 0
                CollectedComplexityPoints_NotAllowed_PartOfProfileInformation = 0
                CollectedComplexityPoints = 0
                For MyCounter = 1 To Len(Password)
                    Dim MyChar
                    MyChar = Mid(Password, MyCounter, 1)
                    If InStr(specchars, MyChar) > 0 Then
                        CollectedComplexityPoints_SpecChars = 1
                    ElseIf InStr(specchars_increasingsecurity, MyChar) > 0 Then
                        CollectedComplexityPoints_IncreaseingSecurity = 1
                    ElseIf InStr(SmallLetters, MyChar) > 0 Then
                        CollectedComplexityPoints_SmallLetters = 1
                    ElseIf InStr(LargeLetters, MyChar) > 0 Then
                        CollectedComplexityPoints_LargeLetters = 1
                    ElseIf InStr(Numbers, MyChar) > 0 Then
                        CollectedComplexityPoints_Numbers = 1
                    ElseIf InStr(CharsEqualOrGreaterThanAscW, MyChar) > 0 Then
                        CollectedComplexityPoints_CharsEqualOrGreaterThanAscW = 1
                    Else
                        CollectedComplexityPoints = 1
                    End If
                Next

                If InStr(LCase(Password), Mid(LCase(LoginName), 1, 4)) > 0 Then
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                    Exit Function
                ElseIf InStr(LCase(Password), Mid(LCase(FirstName), 1, 4)) > 0 Then
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                    Exit Function
                ElseIf InStr(LCase(Password), Mid(LCase(LastName), 1, 4)) > 0 Then
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                    Exit Function
                End If

                'Summarize the complexity points
                CollectedComplexityPoints = CollectedComplexityPoints_Numbers + _
                    CollectedComplexityPoints_CharsEqualOrGreaterThanAscW + _
                    CollectedComplexityPoints_LargeLetters + _
                    CollectedComplexityPoints_SmallLetters + _
                    CollectedComplexityPoints_SpecChars + _
                    CollectedComplexityPoints_IncreaseingSecurity + CollectedComplexityPoints

                'Result of complexity points comparison
                If CollectedComplexityPoints >= MyRequiredComplexityPoints Then
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Success
                    Exit Function
                Else
                    ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                    Exit Function
                End If

                'In all other cases
                ValidatePasswordComplexity = PasswordComplexityValidationResult.Failure_Unspecified

            End Function

            Public Function CreateRandomSecurePassword(ByVal Length)
                Dim Result
                Dim Counter
                Dim ExitNow
                If Length = 0 Then Length = MyRecommendedPasswordLength
                Do
                    'If the customizing has been made faulty, here could be an endless loop --> detect this situation and abort
                    Counter = Counter + 1
                    If Counter > 100 Then
                        err.raise 10023, "Result=" & ValidatePasswordComplexity(Result, "", "", ""), "CreateRandomSecurePassword isn't able to create a valid password"
                    End If
                    'create a new random password
                    Result = CreateRandomPassword(Length)

                    If ValidatePasswordComplexity(Result, "pass", "user", "account") = PasswordComplexityValidationResult.Success Then
                        ExitNow = True
                    End If
                Loop Until ExitNow = True
                CreateRandomSecurePassword = Result
            End Function

            Function CreateRandomPassword(ByVal Length)
                Const specchars = "@#$?!.+~%""=<>;:()_-\/*&"
                Const alphabetnumbers = _
                    "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
                Dim max, loc, i, a, tmp
                Dim wordarray()
                If IsNumeric(Length) Then
                    max = 62 : loc = alphabetnumbers
                Else
                    CreateRandomPassword = Nothing
                    Exit Function
                End If
                tmp = ""
                For i = 1 To CInt(Length - 1)
                    Randomize()
                    a = CInt(Rnd() * max) + 1
                    Randomize()
                    a = CInt(Rnd() * a) + 1
                    If a > max Or a < 1 Then a = 3
                    tmp = tmp & Mid(loc, a, 1)
                Next
                tmp = StrReverse(tmp)
                Randomize()
                a = CInt(Rnd() * Len(specchars)) + 1
                If a > Len(specchars) Then a = 1
                loc = specchars
                tmp = tmp & Mid(loc, a, 1)
                ReDim wordarray(Length)
                For i = 1 To Len(tmp)
                    wordarray(i - 1) = Mid(tmp, i, 1)
                Next
                tmp = ""
                For i = 0 To UBound(wordarray) Step 2
                    If i > UBound(wordarray) Then Exit For
                    tmp = tmp & wordarray(i)
                Next
                For i = 1 To UBound(wordarray) Step 2
                    If i > UBound(wordarray) Then Exit For
                    tmp = tmp & wordarray(i)
                Next
                CreateRandomPassword = CStr(StrReverse(tmp))
            End Function

end class

Function cammWebManager_GetBrowserPreferredLanguage()

            Dim PreferedLanguage
            PreferedLanguage = cammWebManager_GetCulturesFromString(Request.Servervariables("HTTP_ACCEPT_LANGUAGE"))

            Dim Result
            Dim MyPreferedLanguage
            if not isempty(PreferedLanguage) then
	            If ubound(PreferedLanguage) > 0 Then
	                For Each MyPreferedLanguage In PreferedLanguage
	                    Dim MyLanguageID
	                    MyLanguageID = GetLanguageIDOfBrowserSetting(LCase(Trim(MyPreferedLanguage)))
	                    If IsSupportedLanguage(MyLanguageID) Then
	                        Result = MyLanguageID
	                        Exit For
	                    ElseIf IsSupportedLanguage(GetAlternativelySupportedLanguageID(MyLanguageID)) Then
	                        Result = MyLanguageID
	                        Exit For
	                    End If
	                Next
	            End If
            End If
            If Result = 0 Then
                Result = 1 'default
            End If

            cammWebManager_GetBrowserPreferredLanguage = Result

End Function

Sub cammWebManager_Initialize ()
	'Default-Sprache setzen
	on error Resume next
	dim CookieSavedValue_Language
	If Session("CurLanguage") = "" Then Session("CurLanguage") = cammWebManager_GetBrowserPreferredLanguage
	CookieSavedValue_Language = CLng(Request.Cookies("CWM")("Lang"))
	If CookieSavedValue_Language > 0 Then Session("CurLanguage") = CookieSavedValue_Language
	If Request.QueryString("Lang") <> "" Then
		Session("CurLanguage") = CLng(Request.QueryString("Lang"))
		Response.Cookies("CWM").Expires = Now + 183 'Cookie läuft ab in 6 Monaten
		If UCase(Request.ServerVariables("HTTPS")) = "ON" Then
			Response.Cookies("CWM").Secure = True
		Else
			Response.Cookies("CWM").Secure = False
		End If
		Response.Cookies("CWM")("Lang") = Session("CurLanguage")
	End If
	If Session("CurLanguage") < 1 Then
		Session("CurLanguage") = 1
	End If
	CurLanguage = Session("CurLanguage")
	dim ParamItem
	For Each ParamItem In Request.QueryString
		If Not lcase(ParamItem) = "lang" Then RedirectionParams = RedirectionParams & "&" & ParamItem & "=" & Server.URLEncode(Request.QueryString(ParamItem))
	Next
	RedirectionParams = Mid(RedirectionParams,2)
	on error goto 0
End Sub

Public Function cammWebManager_GetCulturesFromString(CultureString)
	CultureString = LCase(CultureString)

	Rem prüfen ob String leer ist
	If Len(CultureString) = 0 Then
	    bool = 0
	Else
	    bool = 1
	End If

	If bool = 1 Then

	   ArrayOfCultures = Split(CultureString, ",")

	   CountArrayLength = 0
	   For Counter = 0 To UBound(ArrayOfCultures)
	      Rem prüfen ob Substring leer ist
	      If Len(ArrayOfCultures(Counter)) <> 0 Then
	         Rem prüfen ob Substring mit einenm Zeichen zwischen a und z beginnt
	         If Asc(ArrayOfCultures(Counter)) >= 97 and Asc(ArrayOfCultures(Counter)) =< 122 Then
	            MyArray = Split(ArrayOfCultures(Counter), ";", 2)
	            ArrayOfCultures(Counter) = MyArray(0)
	            CountArrayLength = CountArrayLength + 1
	         Else
	            Rem Substring der nicht Regelkonform ist mit "####" überschreiben
	            ArrayOfCultures(Counter) = "####"
	         End If
	      Else
	         Rem Substring der nicht Regelkonform ist mit "####" überschreiben
	         ArrayOfCultures(Counter) = "####"
	      End If
	   Next

	   Redim MyClearedArray(CountArrayLength)

	   Rem CulturInfo in bereinigter Form in einen Array schreiben
	   CountCurrentPosition = 0
	   For Counter = 0 to UBound(ArrayOfCultures)
	      If strcomp(ArrayOfCultures(Counter), "####") = 0 then
	      Else
	         MyClearedArray(CountCurrentPosition) = ArrayOfCultures(Counter)
	         CountCurrentPosition = CountCurrentPosition + 1
	      End If
	   Next

	   cammWebManager_GetCulturesFromString = MyClearedArray

	End If

End Function

</script>