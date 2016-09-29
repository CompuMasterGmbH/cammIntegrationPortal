<script language="vb" runat="server">
'================================================
'====                                        ====
'==== Which things must be customized?       ====
'====                                        ====
'==== 1. config.asp/aspx/etc.                ====
'==== 2. internationalization.aspx/aspx/etc. ====
'==== 3. editorial.asp/aspx/etc.             ====
'==== 4. help.asp/aspx/etc.                  ====
'==== 5. your logos and images in            ====
'====    /sysdata/images/*                   ====
'==== 6. server configuration (admin scripts)====
'================================================
'====                                        ====
'==== Which things may be customized?        ====
'====                                        ====
'==== 1. input e-mail address in global.asax ====
'====    if you want to get aspx errors	     ====
'====    e-mailed to you                     ====
'==== 2. createaccount.asp/aspx/etc, if you  ====
'====    want to collect some more data      ====
'==== 3. feedback.asp/aspx/etc, if you want  ====
'====    to collect some more/specific data  ====
'================================================
'====                                        ====
'==== Which things should be updated? When?  ====
'====                                        ====
'==== 1. disclaimer, when ever the law has   ====
'====    has changed; here is your template: ====
'====	 www.disclaimer.de/disclaimer.htm    ====
'================================================


Sub SetupAdditionalConfiguration()
	'================================================
	'==== Important contacts and                 ====
	'==== connection string to the database      ====
	'==== PLEASE MODIFY FOR YOUR NEEDS           ====
	'==== Note: the database user must a dbo     ====
	'====       (= database owner)               ====
	'================================================
	'Dim ConfigurationSettings As New System.Configuration.AppSettingsReader
	'Me.SMTPServerName = ConfigurationSettings.GetValue("WebManager.SMTPServerName", GetType(String))
	'Me.SMTPServerPort = ConfigurationSettings.GetValue("WebManager.SMTPServerPort", GetType(Integer))
	'Me.StandardEMailAccountName = ConfigurationSettings.GetValue("WebManager.StandardEMailAccountName", GetType(String))
	'Me.StandardEMailAccountAddress = ConfigurationSettings.GetValue("WebManager.StandardEMailAccountAddress", GetType(String))
	'Me.DevelopmentEMailAccountAddress = ConfigurationSettings.GetValue("WebManager.DevelopmentEMailAccountAddress", GetType(String))
	'Me.TechnicalServiceEMailAccountName = ConfigurationSettings.GetValue("WebManager.TechnicalServiceEMailAccountName", GetType(String))
	'Me.TechnicalServiceEMailAccountAddress = ConfigurationSettings.GetValue("WebManager.TechnicalServiceEMailAccountAddress", GetType(String))

	'Me.Internationalization = New CustomSettingsAndData
	'Me.System_DebugLevel = ConfigurationSettings.GetValue("WebManager.DebugLevel", GetType(Integer))
	'Me.ConnectionString = ConfigurationSettings.GetValue("WebManager.Connectionstring", GetType(String))

	'=======================================================================
	'== The usage of the IP address is recommended. The server name
	'== should only be used if you use several different virtual servers 
	'== with this extranet engine. In this case the table of servers 
	'== should contain the server name and not the IP address.
	'== IMPORTANT: This value is allowed to contain up to 32 characters.
	'==            Do not use larger server names, they would result in
	'==            errors.
	'=======================================================================
	'Me.CurrentServerIdentString = ConfigurationSettings.GetValue("WebManager.ServerIdentification", GetType(String))
	'Me.CurrentServerIdentString = Request.ServerVariables("LOCAL_ADDR")

	'================================================
	'==== Place for user defined variables,      ====
	'==== constants, functions                   ====
	'================================================
	'==== Changes at this place are allowed.     ====
	'==== This code is available globally        ====
	'================================================
	'== You are allowed to modify the location of the index file (=frameset definition, start page),
	'== but ensure to let it equal on all servers of your server group
	'Me.Internationalization.User_Auth_Config_Paths_UserAuthSystem = "/"
	'Me.Internationalization.User_Auth_Config_CurServerURL = Me.System_GetServerURL(Me.CurrentServerIdentString)
	'Me.Internationalization.User_Auth_Config_UserAuthMasterServer = Me.System_GetMasterServerURL(Me.CurrentServerIdentString)
	'Me.Internationalization.User_Auth_Validation_NoRefererURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_UserAuthSystem & "index.aspx"
	'Me.Internationalization.User_Auth_Validation_LogonScriptURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "logon.aspx"
	'Me.Internationalization.User_Auth_Validation_AfterLogoutURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "logon.aspx?ErrID=44"
	'Me.Internationalization.User_Auth_Validation_AccessErrorScriptURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "access_error.aspx"
	'Me.Internationalization.User_Auth_Validation_CreateUserAccountInternalURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_SystemData & "account_register.aspx"
	'Me.Internationalization.User_Auth_Validation_TerminateOldSessionScriptURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_Login & "forcelogin.aspx"
	'Me.Internationalization.User_Auth_Validation_CheckLoginURL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_Login & "checklogin.aspx"
	'Me.Internationalization.OfficialServerGroup_URL = Me.Internationalization.User_Auth_Config_UserAuthMasterServer & Me.Internationalization.User_Auth_Config_Paths_UserAuthSystem
	'Me.Internationalization.OfficialServerGroup_AdminURL= Me.System_GetUserAdminServer_SystemURL(Me.CurrentServerIdentString)
	'Me.Internationalization.OfficialServerGroup_Title = Me.System_GetServerGroupTitle(Me.CurrentServerIdentString)
	'Me.Internationalization.OfficialServerGroup_Company_FormerTitle = Me.System_GetServerConfig(Me.CurrentServerIdentString, "AreaCompanyFormerTitle")
	'Me.Internationalization.User_Auth_Config_Files_Administration_DefaultPageInAdminEMails = "memberships.aspx" 
End Sub

</script>
