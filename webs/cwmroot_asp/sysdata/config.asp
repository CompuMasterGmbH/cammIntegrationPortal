<%
'================================================
'==== Important contacts and                 ====
'==== connection string to the database      ====
'==== PLEASE MODIFY FOR YOUR NEEDS           ====
'==== Note: the database user must a dbo     ====
'====       (= database owner)               ====
'================================================
const User_Auth_Config_CurSMTPServer = "localhost"
const User_Auth_Config_CurSMTPServer_Port = 25 
const StandardEMailAccountName = "Secured Area Administration"
const StandardEMailAccountAddress = "saa@yourdomain.com"
const DevelopmentEMailAccountAddress = "developer@yourdomain.com"
const TechnicalServiceEMailAccountName = "OnlineService"
const TechnicalServiceEMailAccountAddress = "onlineservice@yourdomain.com"
const User_Auth_Config_CurSMTPServer = "localhost"

dim User_Auth_Validation_DSN
'Connection string to your SQL server
User_Auth_Validation_DSN = "Provider=SQLOLEDB;DRIVER={SQLServer};SERVER=192.168.10.xxx;DATABASE=camm WebManager;pwd=yourpassword;uid=sa"

dim User_Auth_Config_Paths_UserAuthSystem
'You are allowed to modify the location of the index file (=frameset definition, start page),
'but ensure to let it equal on all servers of your server group
User_Auth_Config_Paths_UserAuthSystem = "/"

'Returns either the server name or the local IP address.
Function GetCurrentServerIdentString()
	'The usage of the IP address is recommended. The server name
	'should only be used if you use several different virtual servers 
	'with this extranet engine. In this case the table of servers 
	'should contain the server name and not the IP address.
	'IMPORTANT: This value is allowed to contain up to 32 characters.
	'           Do not use larger server names, they would result in
	'		    errors.
	
	'GetCurrentServerIdentString = Mid(Request.ServerVariables("SERVER_NAME"), 1, 32)
	GetCurrentServerIdentString = Request.ServerVariables("LOCAL_ADDR")

End Function

'================================================
'==== Place for user defined variables,      ====
'==== constants, functions                   ====
'================================================
'==== Changes at this place are allowed.     ====
'==== This code is available globally        ====
'================================================
Sub SetupAdditionalConfiguration()
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
	OfficialServerGroup_AdminURL= System_GetUserAdminServer_SystemURL(GetCurrentServerIdentString)
	OfficialServerGroup_Title = System_GetServerGroupTitle(GetCurrentServerIdentString)
	OfficialServerGroup_Company_FormerTitle = System_GetServerConfig(GetCurrentServerIdentString, "AreaCompanyFormerTitle")
	User_Auth_Config_Files_Administration_DefaultPageInAdminEMails = "memberships.aspx" 

	System_DebugLevel = 0
End Sub
%>