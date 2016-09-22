#ChangeLog for camm Integration Portal / camm Web-Manager

##Note on updates
* camm Web-Manager is build up to provide most compatibility with old version. But there may be some modifications which need to be tested on your current instance.
* We strongly recommend a small test environment with same content in /sysdata as in your production environment (except the config.vb and web.config which may differ).
* Changes in /sysdata do not automatically apply for already installed camm Web-Manager instances. This folder contains project dependent content and has to be updated manually.
* If there are breaking changes in the database, never update the database first. First, you always have to update the cammWM.dll in your projects plus the script files.
* All updates might contain additional minor changes not stated here explicitly
* Legend
  * :heavy_plus_sign:	Added features
  * :heavy_minus_sign:	Removed items
  * :arrows_clockwise:	Changed
  * :warning:	Breaking change (or to be treated carefully)
* :construction: Known issues
  * With .Net2 you can setup the security/trust level from Full down to Medium (=Microsoft’s recommendation). This will break functionality for following modules: DownloadHandler, WebEditor, and all related components

##Changes while developing in following builds

###Build 2104 (DB Engine)
* :heavy_minus_sign:	remove all obsolete data from user table
* :heavy_minus_sign::warning:	deactivate all active sessions for safety between user session handling phase 1 "single login" and phase 2 "multiple simultaneous logins"

###Build 178 → 206 (ASP Engine)
* :heavy_plus_sign:	Splitted OfficialServerGroup_AdminURL to support separate value for e-mail notifications (OfficialServerGroup_AdminURL_SecurityAdminNotifications)

###Build 205 → 206 (ASP.NET Engine)
* :arrows_clockwise::warning: security fix: public server check only under special circumstances
* :heavy_plus_sign:	product registration service: added auth statistics
* :heavy_plus_sign:	update DB build to a customizable DB version
* :heavy_plus_sign:	authorization transmissions: support for copying auths while deleting authorization transmission
* :heavy_plus_sign:	authorization administration: backend support for IsDenyRule, front-end
* :heavy_plus_sign:	authorization administration: display users in list with company name
* :heavy_plus_sign:	added .NET 2 standard mail system System.Net.Mail (.NET 1 still uses System.Web.Mail with CDO technic)
* :heavy_plus_sign:	added/changed strong-typing of additional flags of user profiles with following possible types:
  *	FlagName	String
  *	FlagName{String}	String
  *	FlagName{Text}	String
  *	FlagName{Int}	Integer 32 bit
  *	FlagName{Int32}	Integer 32 bit
  *	FlagName{Number}	Integer 32 bit
  *	FlagName{Long}	Integer 64 bit
  *	FlagName{Int64}	Integer 64 bit
  *	FlagName{Bit}	Boolean/bit value (store values “1” or “0”)
  *	FlagName{Bool}	Boolean/bit value (store values “true” or “false”)
  *	FlagName{Boolean}	Boolean/bit value (store values “true” or “false”)
  *	FlagName{Date}	String with a date/time in international ISO format, e.g. with format “yyyy-MM-dd HH:mm:ss” (independent of current culture of user)
  *	FlagName{Date/ISO}	String with a date/time in international ISO format, e.g. with format “yyyy-MM-dd HH:mm:ss” (independent of current culture of user)
* :arrows_clockwise:	user administration: import UI notes on additional flags to default to replace only defined keys and not to always cleanup the whole collection
* :arrows_clockwise:	fixed several errors in mail sub systems, especially regarding embedded attachments and general stability
* :arrows_clockwise:	several fixes in administration area
* :arrows_clockwise:	several fixes in general
* :heavy_minus_sign:	removed QuikSoft.EasyMail component – replaced by stable System.Net.Mail mail sub system (new mail sub system will be used automatically without any changes required)
* :heavy_minus_sign:	removed Telerik.Rad components – replaced by stable SmartPlainTextEditor with always-warning to technician asking for replacing by another SmartEditor control
* :heavy_minus_sign:	removed obsolete assembly dependency to CompuMaster.Imaging

###Build 205 → 2109 (DB Engine)
* :warning:	requires restart of all web applications (e.g. by iisreset or re-saving/re-upload of web.config on all connected servers and web applications)
* :arrows_clockwise::warning:	splitted anonymous group ID 58 into separate ones for each server group
* :heavy_plus_sign:	prepared memberships inherition
* :heavy_plus_sign:	added membership deny rules
* :heavy_plus_sign:	added pre-staging/pre-calculations of memberships and authorizations
* :heavy_plus_sign:	added user impersonation feature for test and development users and server groups
* :arrows_clockwise:	distributed delete on tables with foreign key relationship by triggers
* :heavy_minus_sign:	view/sp/function encryption
* :heavy_minus_sign:	separated/removed module for log analysis from standard for reduced dependencies

###Build 202 → 205 (ASP.NET Engine)
* :warning: the server for processing the asynchronous tasks (running the "core" webservice) must be able to connect directly to the internet (to www.camm.biz by https and http) for the introduced product registration service (without proxy support)
* :heavy_plus_sign:	product registration service
* :heavy_plus_sign:	support for simultaneous login
* :heavy_plus_sign:	user administration overview: added support for searching for e-mail addresses
* :heavy_plus_sign:	user administration overview: changed to logical AND search (instead of logical OR/ANY previously)
* :arrows_clockwise:	fixed issue in mail queue with empty reply-to header
* :arrows_clockwise:	fixed several minor bugs 
* :heavy_minus_sign:	separated/removed module for log analysis from standard for reduced dependencies
* :heavy_minus_sign:	separated/removed navigation tools for 3rd party components ComponentArt and CyberAkt
* :heavy_minus_sign:	removed ComponentArt dependency (except AppKey setup) 
* :heavy_minus_sign:	removed integraded upload for internal smart editors (system\modules\smartwcms\upload.aspx) which is a separate module, now
* :heavy_minus_sign:	removed CyberAkt dependency
* :heavy_minus_sign:	removed RadChart dependency

###Build 204 → 205 (DB Engine)
* :heavy_plus_sign:	support for simultaneous login
* :heavy_plus_sign:	added support for Azure SQL V12
* :arrows_clockwise:	migrated azure v11 improvements of sql statements into standard sql files
* :heavy_minus_sign:	removed support for SQL Server 7 and 2000 in complete
* :heavy_minus_sign:	removed support for Azure SQL V11

###Build 200 → 204 (DB Engine)
* :heavy_plus_sign:	anonymization rules
* :heavy_plus_sign:	added columns for AuthDenyRules and AuthDevGroup
  *	IsAllowedStandard = (AllowIsDev – DenyIsDev) + (AllowStandard – DenyStandard)
  *	IsAllowedForDevelopment = AllowIsDev - DenyIsDev
* :heavy_plus_sign:	added basic support IsDenyRule for memberships
  *	IsEffectiveMember = Allow - Deny

###Build 200 → 202 (ASP.NET Engine)
* :heavy_plus_sign:	improved exception handling with exception token for reference purposes
* :heavy_plus_sign:	IIS sometimes replaced error pages by its own custom page – so it replaced the prepared error page for the user of the error handling system by an IIS error page. Additional configuration can be set up in web.config using the app setting WebManager.NotifyOnApplicationExceptions.ReplaceResponseStatusCode to False or – more recommended if it fits for your environment - 
  ```
  <system.webServer>
  	<httpErrors errorMode="Custom" existingResponse="PassThrough"/>
  </system.webServer>
  ```
* :arrows_clockwise:	fixed several deadlock situations

###Build 199 → 200 (ASP.NET Engine)
* :warning:	redesigned service interfaces (requires update of trigger service application)
* :heavy_plus_sign:	added support for strong typed additional flags for optional application requirements 
* :arrows_clockwise:	fixed several deadlock situations

###Build 199 → 200 (DB Engine)
* :arrows_clockwise:	fixed several deadlock situations

###Build 197 → 199 (ASP.NET Engine)
* :heavy_plus_sign:	added basic support for community edition and online license verification (unstable)
* :arrows_clockwise:	some minor internal changes and fixes

###Build 197 → 199 (DB Engine)
* :heavy_plus_sign:	(re)created indexes and views for improved performance

###Build 194 → 197 (ASP.NET Engine)
* :heavy_plus_sign:	added support for enhanced password security
* :heavy_plus_sign: 	CreateUser/UpdateUser/-PW also logs current admin user
* :heavy_plus_sign:	added support for administration priviledges ViewRelations, ViewAllItems and ViewAllRelations for Group objects
* :heavy_plus_sign:	Page classes: lookup CWM instance in master page by searching in all its (sub)controls
* :arrows_clockwise:	removed unnecessary size information on SQL parameters - often with an old, smaller size than currently available in table (e.g. usernames limited to 20 chars, but 50 chars available; similar behaviour at some code locations for location, state, country, street, title and a few more fields)
* :arrows_clockwise: 	fixed missing saving of system group flag in System_UpdateGroup
* :arrows_clockwise: 	required DB build no. for System_SupportsMultiplePasswordAlgorithms corrected to 195 (instead of 193)
* :arrows_clockwise:	fixed some minor bugs in administration area

###Build 193 → 197 (DB Engine)
* :heavy_plus_sign:	added support for enhanced password security
* :heavy_plus_sign:	added basic support for data protection module
* :heavy_plus_sign:	on create group, automatically setup creator as group owner
* :heavy_plus_sign:	Added trigger for deleting related data of user/group when deleting a user/group
* :arrows_clockwise:	widened size information on SP parameters - often with an old, smaller size than currently available in table (e.g. usernames limited to 20 chars, but 50 chars available; similar behaviour at some code locations for location, state, country, street, title and a few more fields)
* :arrows_clockwise: 	recreation of all function+sp+trigger
* :arrows_clockwise: 	fixed group name field size from 50 to 100 chars at SP CreateGroup

###Build 193 → 194 (ASP.NET Engine)
* :heavy_plus_sign: 	UpdateUserDetails and UpdateUserPW now save the admin user ID into the log

###Build 192 → 193 (ASP.NET Engine)
* :warning: 	An additional e-mail template has been added for sending password reset links (instead of password itself). Add your customized template to your /sysdata/custom_internationalization.vb if required.
* :heavy_plus_sign: 	Added support for multiple password algorithms (requires .NET 2.0 or higher at all of your root web applications and ADS-SSO web applications). You can chose at Web Administration >> Setup >> About Web-Manager >> Advanced configuration between old EncDecMod, TripleDES, AES256, and PBKDF2 (recommended)
* :heavy_plus_sign: 	Added password reset form.
* :heavy_plus_sign: 	Added converter so you can convert old passwords to the new algorithm you have chosen.
* :heavy_plus_sign: 	Added forceref= parameter. You can use this to load a page in the frameset. Works like ref=, but it ignores session variables. Will only work with server local pages starting with /.
* :heavy_plus_sign: 	Added password recovery behaviours. Either try to decrypt password if algorithm allows it and send it to the user, or always send a link to the reset form.
* :arrows_clockwise: 	index_frameset.aspx has been changed to fix a potential XSS.

###Build 190 → 193 (DB Engine)
* :heavy_plus_sign: 	Added two columns to users table, storing password algorithm and nonce (IV/Salt)
* :heavy_plus_sign: 	Default values for password rest behaviour and to be used algorithm.
* :arrows_clockwise: 	Changed procedures so they work together with these two columns

###Build 189 → 190 (DB Engine)
* :heavy_plus_sign:	Added support for MS SQL-Server 2014
* :heavy_plus_sign:	Added support for MS SQL-Azure (V11)

###Build 188 → 189 (DB Engine)
* :heavy_plus_sign:	Added support for MS SQL-Server 2012
* :heavy_plus_sign:	Enabled error page also for ASP.NET/IIS with integrated mode (requires ASP.Net >= 3)
* :arrows_clockwise:	Workaround: Reset all existing user account to “InitAuthorizationsDone” (haven’t been correctly updated in past)

###Build 179 → 192 (ASP.NET Engine) (unstable)
* :warning:	Update account_updateprofile.aspx with its base class to allow easy modification of available motivation reasons as well (like the register form)
* :heavy_plus_sign:	Added partial support for MS SQL Azure as database server
* :heavy_plus_sign:	Added support and base classes for supporting the Master page concept
* :heavy_plus_sign:	Added support for custom script code in Utils.RedirectWithPostData
* :heavy_plus_sign:	Automatic warning e-mail on failed hard link creation
* :arrows_clockwise:	Compatibility to DB updates till 192 inclusive
* :arrows_clockwise:	Fixed bug to insert many unnecessary rows in 2 tables which might lead to exceeded space limits for the database
* :arrows_clockwise:	Fixed caching bug at several code places (the cache never kept the data for next request)
* :arrows_clockwise:	Fixed .Net 1.1 compatibility of system/apps.aspx. (Update cammWM.Admin.dll together with system/apps.aspx)
* :arrows_clockwise:	Fixed download handler file name compatibility issue with invalid control chars like tab, null-char, etc.
* :arrows_clockwise:	Improved exception details on failed hard link creation

###Build 186 → 188 (DB Engine)
* :arrows_clockwise:	Fixed issues caused by changes in build 180 to 184 regarding returned ID values of new rows
* :arrows_clockwise:	Full rewrite of all views and procedures

###Build 185 → 186 (DB Engine)
* :heavy_plus_sign:	Increased performance on message queue table by adding additional column indexes

###Build 184 → 185 (DB Engine)
* :heavy_plus_sign:	Extended data on feature user profile flag
* :arrows_clockwise:	Application cloning and application rights improved/fixed

###Build 179 → 184 (DB Engine)
* :warning:	Support for SQL Server 7 ends with build 179; beginning with build 180, support is not available anymore (but productive usage is still possible with build 179 without problems)!
* :heavy_plus_sign:	Introduction of additional tables and columns for splitted management of navigation items and security objects inclusive automatic filling possibility by triggers
* :heavy_plus_sign:	Option to select security delegates while copying an application
* :arrows_clockwise:	Marked a few SPs as obsolete
* :arrows_clockwise:	Several small fixes in admin area
* :arrows_clockwise:	Show only first 50 apps in application list in default view

###Build 177 → 179 (DB Engine)
* :heavy_plus_sign:	Introduction of fast SP for cumulated authorizations of a user
* :heavy_plus_sign:	Introduction of new SP for renaming login names

###Build 131 → 178 (ASP Engine)
* :heavy_plus_sign:	Added MailQueue technology
* :arrows_clockwise:	Redirected SMTP e-mail methods to MailQueue methods to support 64 bit platforms (the existing 32-bit Chilkat ActiveX component (which caused the problem because it’s not available for 64 bit) is not required any more)

###Build 177 → 178 (ASP.NET Engine) (unstable – in process)
* :warning:	Changed property type in UserInformation class for Authorizations from SecurityObjectInformation array to SecurityObjectAuthorizationForUser array
* :heavy_plus_sign:	Handling protected flags in clone users
* :heavy_plus_sign:	Some minor improvements in admin area
* :heavy_plus_sign:	Cleanup user authorizations
* :heavy_plus_sign:	Download handler file and collection sizes now configurable in web.config
* :heavy_plus_sign:	Added properties in information classes for Authorizations
* :heavy_plus_sign:	Added missing properties and methods to access and modify the authorizations in SecurityObjectInformation class
* :heavy_plus_sign:	Updated CompuMaster.Imaging component to version 2010.8; it’s now .NET platform dependent
* :arrows_clockwise:	Removed ComponentArt ComboBox-Controls and replaced them with DropDownLists and new user-selection worklflow in admin area
* :arrows_clockwise:	Minor changes in BatchUserFlagEditor and reactivating links in about-page
* :arrows_clockwise:	Fixed .NET 4 breaking change regarding RawUrl affecting WebEditor’s address and content lookup
* :arrows_clockwise:	Several bugfixes in admin area
* :arrows_clockwise:	Fixed random password generation which had got chr(0)-characters causing problems
* :arrows_clockwise:	Fixed global catching of requests with compilation exceptions which may fail because of security exceptions
* :arrows_clockwise:	Fixed global catching of request with 404 errors for framework versions >= 2
* :heavy_minus_sign:	Removed write properties for authorizations and memberships in GroupInformation and UserInformation classes
* :arrows_clockwise:	Fixed download handler error with file handles still open by the operating system
* :arrows_clockwise:	Fixed handling of loading user infos for invalid user IDs (exception will now be thrown)
* :arrows_clockwise:	Added machine network info into global application error messages for better identifying the origin server reporting the error
* :arrows_clockwise:	Moved functionality from User/GroupInformation classes to DataLayer regarding add/remove authorizations
* :arrows_clockwise:	Some internal changes in download handler module – may fix a few errors
* :arrows_clockwise:	Fixed DownloadHandler redirection to use own redirection implementation and to not use ASP.NET 2.x's integrated Response.Redirect which uses an incorrectly encoded redirection body (usually not considered, but may be considered by some simple search robots) 

###Build 174 → 177 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Add/RemoveMemberships/Authorizations now ensures the current admin user to be authorized to administer the relationships (by UpdateReleations, Owner, SecurityMaster or Supervisor rights)

###Build 169 → 177 (DB Engine)
* :heavy_plus_sign:	Introduction of SPs IsAdministratorForAuthorizations and IsAdministratorForMemberships and altered SPs like AdminPrivate_CreateMemberships, dbo.AdminPrivate_CreateApplicationRightsByGroup to ensure the current admin user to be authorized to administer the relationships (by UpdateReleations, Owner, SecurityMaster or Supervisor rights)
* :arrows_clockwise:	Fixed SP [AdminPrivate_UpdateSubSecurityAdjustment] for correct parameter data types
* :arrows_clockwise:	LogMissingExternalUserAssignment now ensured with schema name [dbo]

###Build 173 → 174 (ASP.NET Engine) (unstable – in process)
* :arrows_clockwise:	caching of encrypted ServerIdentstring in Downloadhandler class. This was necessary because the old caching logic did not support the changing of ServerIdenstring in environments where the ServerIdentstring can be changed during application lifetime.

###Build 172 → 173 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	With ASP.NET 2 or higher, there might be some assembly load exceptions for security reasons (the webserver is configured to run e. g. with medium trust instead of full trust). Those errors lead to a notification mail to the technical contact. By using the config setting “WebManager.Application.IgnoreSecurityExceptionsForLoadingAssemblies”, it is possible to ignore those security exceptions for defined components if they’re not required.

###Build 169 → 172 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Introduced gender field to be one of the following values: UndefinedGender (if a user has got a first and family name, but the gender is unknown) and MissingNameOrGroupOfPersons (if the gender is unknown as well as there is first name or last name missing; those accounts are treated as group account and those users are welcomed e. g. with “Hello together”)
* :heavy_plus_sign:	Introduces writing direction information in market/language info object

###Build 166 → 169 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Several improvements in Admin Area
* :heavy_plus_sign: Added method for doing a prevalidation of user credentials for usage in custom login forms

###Build 166 → 169 (DB Engine) (unstable – in process)
* :heavy_plus_sign:	Added procedure for PreValidation of user credentials

###Build 164 → 166 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Added HttpApplication ExceptionLogIntoWindowsEventLog

###Build 164 → 166 (DB Engine) (unstable – in process)
* :heavy_plus_sign:	Added navigation points
* :heavy_plus_sign:	Allowed authorization and membership creation also for the anonymous user (e.g. to be used in user creation forms where the user shall be created and authorized in one step)

###Build 163 → 164 (DB Engine) (unstable – in process)
* :heavy_plus_sign:	Added stored procedure to return navigation items by group

###Build 163 → 164 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Added group navigation preview in admin area (requires /sysdata/nav/index.aspx to use the new data structure as in delivered example)
* :arrows_clockwise:	Fixed compilation errors with the update profile form
* :arrows_clockwise:-	Several minor changes in DownloadHandler
* :arrows_clockwise:	Implemented new version of ComponentArt.Web.UI (2009.2) and fixed some bugs while filtering data in the combobox control (Admin-Area)
* :arrows_clockwise:	Fixed notification, when a new account was created by the user himself and then he creates another account by browsing back to the CreateUser form.
* :heavy_plus_sign:	Added option configuration in about-page
* :heavy_plus_sign:	Added check for common issues in about page
* :arrows_clockwise:	Fixed encoding issues while creating a new user authorization (html-injection possible)
* :heavy_plus_sign:	Added CloneUsers functionality and page form
* :heavy_plus_sign:	Added link to send an e-mail to the technical contact when login timed out
* :arrows_clockwise:	Some improvements in custom notification class, e.g. support for e-mail signature (config.vb)
* :arrows_clockwise:	Some improvements and fixes in log analysis
* :arrows_clockwise:	Fixed many remote vulnerabilities, e.g. sql-injection, xssBuild 162 → 163 (ASP.NET Engine) (unstable – in process)
* :warning:	Control sWcms has been splitted internally into a base class and a UI class. Caused by this change there is the need to recompile assemblies which use the sWcms functionality (if you don’t do, you would receive e.g. property/method doesn’t exist exceptions)
* :warning:	Update of component ICSharpCodeZipLib to version 0.85.5 and now in versions for .NET 1.1 and 2.0 to support medium trust level environments. Consider carefully this point if you use this library in your own solutions, too: the public key token of this library has changed and requires a recompilation of your solution referencing this DLL.
* :heavy_plus_sign:	Added some methods for cloning user accounts
* :heavy_plus_sign:	Added configuration setting for WebManager.EventLogTrace which enables reporting of the current page request progress, especially usefull for debugging StackOverflow situations
* :heavy_plus_sign:	Added logging/tracing feature to write to windows event log (needs to be activated on-demand)
* :heavy_plus_sign:	Added notification e-mail to technical contect in case of components with assembly loading problems (requires the global.asax file to be in place)
* :arrows_clockwise:	Several minor fixes in admin area
* :arrows_clockwise:	Fixed issue with multiple CWM-Lang-Cookies for different paths (in this case, the market ID didn’t change any more)
* :arrows_clockwise:	Fixed some issues in exception catching and logging

###Build 162 → 163 (DB Engine)
* :arrows_clockwise:	Support adding of authorizations by code-user (-33)

###Build 161 → 162 (DB Engine)
* :heavy_plus_sign:	Added support for SQL Server 2008
* :arrows_clockwise:	Fixed SQL setup/update statements for SQL Server 7
* :arrows_clockwise:	Strip down of very early (and for newer releases unnecessary) update statements to reduce file size of cammWM.dll

###Build 161 → 162 (ASP.NET Engine) (unstable – in process)
* :warning:	Control sWcms has been splitted internally into a base class and a UI class. Caused by this change there is the need to recompile assemblies which use the sWcms functionality (if you don’t do, you would receive e.g. property/method doesn’t exist exceptions)
* :warning:	Update of component ICSharpCodeZipLib to version 0.85.5 and now in versions for .NET 1.1 and 2.0 to support medium trust level environments. Consider carefully this point if you use this library in your own solutions, too: the public key token of this library has changed and requires a recompilation of your solution referencing this DLL.
* :heavy_plus_sign:	Support for language hungary
* :heavy_plus_sign:	Additional information and warnings in Single-Sign-On module in case of debug levels 1 or 2
* :heavy_plus_sign:	Single-Sign-On now also uses information from ADS regarding department and manager when creating a new user account
* :heavy_plus_sign:	Introduced salutation formulas which can be localized from language to language
* :heavy_plus_sign:	ComponentArt library in versions for .NET 1.1 and 2.0
* :heavy_plus_sign:	Ensuring Content-ID to be the new file name of an attachment in queue e-mails
* :heavy_plus_sign:	Improved database update/setup wizard using CWM embedded resources as well as adding port 1433 automatically to the database connection string if port hasn’t been supplied
* :heavy_plus_sign:	Show database update error details in case of errors
* :heavy_plus_sign:	Implemented SMTP login ability for .Net-Framework-internal SMTP provider
* :arrows_clockwise:	Fixed issue with missing attachments in queued e-mails
* :arrows_clockwise:	Restored binary compatibility with previous versions for SmartWcms control
* :arrows_clockwise:	Fixed: JIT-user-creation by ADS-SSO module now takes title field of ADS which is position and places it into position field (previously it was placed into academic title field)
* :arrows_clockwise:	Single-Sign-On puts ADS-Jobtitle into correct field “Position” now
* :arrows_clockwise:	Internal reorganization of method logic for automatic error emails
* :arrows_clockwise:	Upgraded EasyMail component to 3.0.1.12
* :construction: Known issues
  *	With .Net2 you can setup the security/trust level from Full down to Medium (=Microsoft’s recommendation). This will break functionality for following modules: DownloadHandler, WebEditor, and all related components

###Build 157 → 161 (DB Engine)
* :arrows_clockwise:	Removed unnecessary and inconsistent data

###Build 160 → 161 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Separated builds for .NET 1.1 and 2.0
* :arrows_clockwise:	Smart WebEditor uses the webmanager configuration value “WebManager. WebEditorContentOfServerID” for loading content as a defined server ID (so you can load content from another server ID instead the current server)
* :arrows_clockwise:	Some minor bug fixes
* :arrows_clockwise:	Some work for strong typing
* :construction: Known issues
  * With .Net2 you can setup the security/trust level from Full down to Medium (=Microsoft’s recommendation). This will break functionality for following modules: DownloadHandler, WebEditor, and all related components

###Build 159 → 160 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Changed Configuration.ProcessMailQueue to TripleState and switched Auto or blank config value to auto-detect and false to be really false
* :heavy_plus_sign:	Introduction of exchangebale data layer (to be configured by app.config/web.config in future releases)
* :heavy_plus_sign:	Introduced new property AutoSecurityCheckLogsPageAccess
* :heavy_plus_sign:	Introduced config setting WebManager.NotifyOnApplicationExceptions.404.IgnoreBots
* :construction: Known issues
  * With .Net2 you can setup the security/trust level from Full down to Medium (=Microsoft’s recommendation). This will break functionality for following modules: DownloadHandler, WebEditor, and all related components

###Build 158 → 159 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Improved handling of POST data in exception notification e-mails sent by the global.asax handler by limiting all fields to a maximum of 10,000 characters
* :arrows_clockwise:	Fixed issue with sending e-mails with the EasyMail component
* :construction: Known issues
  * With .Net2 you can setup the security/trust level from Full down to Medium (=Microsoft’s recommendation). This will break functionality for following modules: DownloadHandler, WebEditor, and all related components

###Build 157 → 158 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Added configuration option “WebManager.WebEditor.ContentOfServerID" to force a server ID for WebEditor where to read/save the content from/to (without specifying, it will always be the current server ID)
* :arrows_clockwise:	Fixed missing/wrongly configured member for overloaded method System_SendEmail_Ex2

###Build 155 → 157 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Added Polish language
* :heavy_plus_sign:	Support for Apache reverse proxy scenarios with non-standard port number
* :heavy_plus_sign:	Updated library ComponentArt Web.UI to V2007.2.1272
* :heavy_plus_sign:	Exception notification emails don’t contain whole viewstate any more
* :heavy_plus_sign:	Added support for notification e-mails with writing direction right-to-left
* :arrows_clockwise:	Notification mails now correctly encode dynamic fields in HTML
* :arrows_clockwise:	Minor corrections in Portuguese language
* :arrows_clockwise:	Fixed SmartWcms Editor functionality to not add the local server name (to remove local server name while saving)
* :arrows_clockwise:	Reegineered account_register.aspx
* :arrows_clockwise:	Fixed issue with missing e-mail notifications for administrators when another security admin created a new user account

###Build 153 → 155 (DB Engine)
* :heavy_plus_sign:	Added error details field in mail queue table for storing last error in case of errors

###Build 154 → 155 (ASP.NET Engine) (unstable – in process)
* :heavy_plus_sign:	Added function for SalutationUnformalWithFirstName in user information object
* :heavy_plus_sign:	Fixed user data search: SearchMethod.All will lead to all records instead to those records which have got an entry (in technical words: there will be a left join instead of an inner join)

###Build 153 → 154 (ASP.NET Engine) (unstable – in process)
* :warning:	Additional overloads of Messaging.SendMail might break down running application with method calls in dynamically compiled sources (e.g. if used in ASPX pages)
* :warning:	Added boolean configuration parameter “WebManager.DataBindAutomaticallyWhilePageOnLoad” defaults to False if not present (in past it had been an internal default to True)
* :heavy_plus_sign:	Added global.asax exception notification for developers (instead of the technical contact of the whole website) or for both developer and technical contact
* :heavy_plus_sign:	Improved global.asax exception handling to not catch 404 errors (so that customError redirections for 404 errors continue to work as set up) by introducing appSetting “WebManager.NotifyOnApplicationExceptions.404”
* :heavy_plus_sign:	Several improvements and bugfixes in admin area
* :heavy_plus_sign:	Introduced MailSendingSystem EasyMail
* :heavy_plus_sign:	Improved e-mail capabilities regarding MailSendingSystem NetFramework
* :heavy_plus_sign:	Improved e-mail queue capabilities to support attachments and embedded files
* :heavy_plus_sign:	Improved support for 64bit environments, code security and Mono’s .NET Framework implementation by removing all relations to Chilkat component which contains some unmanaged code 
* :heavy_minus_sign:	Removed library reference to chilkat e-mail component
* :arrows_clockwise:	Replaced default e-mail transport from Chilkat to Quiksoft-EasyMail component
* :arrows_clockwise:	Corrected, safe HTML output in exception notification e-mails sent by global.asax
* :arrows_clockwise:	SmartWcmsControl: Automatically remove the server URL part from links if it is the current server

###Build 152 → 153 (DB Engine)
* :heavy_plus_sign:	Drop old session values (batch command)
* :heavy_plus_sign:	Drop old session values (periodically)

###Build 152 → 153 (ASP.NET Engine)
* :heavy_plus_sign:	Completed localizations with remaining languages Chinese (Mandarin) and Italian
* :arrows_clockwise:	Optimizations and bug fixing in CWM admin area
* :arrows_clockwise:	Crypt result made independent from current thread culture

###Build 150 → 152 (DB Engine)
* :heavy_plus_sign:	Added navigation items for administration area for languages Spanish, Portuguese , Russian, Japanese, Chinese (Mandarin), Italian

###Build 144 → 152 (ASP.NET Engine)
* :heavy_plus_sign:	Added localizations for languages Spanish, Portuguese , Russian, Japanese, Chinese (Mandarin), Italian (some languages still need further translations)
* :heavy_plus_sign:	The known bug documented in build 144 has been fixed
* :heavy_plus_sign:	The admin area has improved for basic handling of required user flags (still missing some features)
* :heavy_plus_sign:	The cammWM.dll has been splitted into 2 dlls: all classes related to the admin area have been out-sourced into the new cammWM.Admin.dll
* :heavy_plus_sign:	TextModules admin area now supports creation of new website area IDs
* :heavy_plus_sign:	TextModules admin area allows copying of a website area in a single market into another one
* :heavy_plus_sign:	Improvements of UI in TextModules admin area
* :heavy_plus_sign:	Data language in TextModules admin area changeable independently from current UI market selection
* :heavy_plus_sign:	New configuration setting WebManager.CompatibilityWithDatabaseBuild which allows usage of older assemblies with later database versions
* :heavy_plus_sign:	Some changed text elements in all languages for file account_sendpassword.aspx
* :heavy_plus_sign:	Application version logging for reference when updating database
* :heavy_plus_sign:	Improved defaults for security
* :heavy_plus_sign:	Improved debug level outputs for improved security
* :heavy_plus_sign:	Database update page shows current instances and their versions in future (requires each instance to run build >= 152 to report its version into the database periodically)
* :heavy_plus_sign:	Database updates can be executed up to a specified version; this allows to update to an intermediate release and you don’t have to update to the very latest version
* :construction: Known issues
  * With .Net2 you can setup the security/trust level from Full down to Medium (=Microsoft’s recommendation). This will break functionality for following modules: DownloadHandler, WebEditor, Chilkat MailComponent and all related components

###Build 144 → 150 (DB Engine)
* :heavy_plus_sign:	Updated market/language descriptions
* :heavy_plus_sign:	Added Spanish navigation items for administration area
* :heavy_plus_sign:	Update of markets/languages list with additional virtual markets
* :heavy_plus_sign:	Added field RequiredUserProfileFlags in applications table

###Build 143 → 144 (ASP.NET Engine)
* :heavy_plus_sign:	Improved caching of list of LanguageInfos + DBVersion
* :heavy_plus_sign:	Improved performance in some SQL commands
* :heavy_plus_sign:	Improved admin area (especially the drop down boxes in add/authorize user/group forms)
* :heavy_plus_sign:	Updated 3rd pary control ComponentArt.Web.UI from 3.0 to version 2700.1.1590
* :heavy_plus_sign:	Customizable download handler working path in web.config
* :warning:	HtmlEncoded menu items or tree items in ComponentArt.Web.UI solutions are not supported any more by the newest control version; plain text elements are still fine
* :warning:	KNOWN ISSUE: Authorize user form and add membership form: drop down control has got a scrolling bug currently; clear the name field to stop the scroll-back-jump bug or type in the name manually
* :arrows_clockwise:	About-page updated, some smaller, internal changes in update mechanisms
* :arrows_clockwise:	Some code refactoring in configuration class
* :arrows_clockwise:	Changed System_SendEMail* methods to be obsolete, use MessagingEMails.SendEMail instead

###Build 141 → 143 (ASP.NET Engine)
* :warning:	Web-Explorer project has been splitted from camm Web-Manager; the old classes in CWM’s assembly are marked as obsolete. The new implementation can be found in separate project “camm Web-Explorer.Base” 
* :arrows_clockwise:	Fixed issues with cammWM.TextModules: preview area has thrown compilation error
* :arrows_clockwise:	Fixed admin account ID after database setup
* :arrows_clockwise:	Fixed another issue with auto-account-registration of Single-Sign-On application which hadn’t been fixed in build 139
* :heavy_plus_sign:	Added IsMember methods in UserInformation and HasMember methods in GroupInformation
* :heavy_plus_sign:	Security Object can be defined in web.config, as following: <configuration><appSettings><add key="WebManager.Securityobject" value="@@Public" /></appSettings></configuration>
* :heavy_plus_sign:	New additional markets, which have got a more “virtual” character like “Latin America/Caribbean”

###Build 140 → 141 (ASP.NET Engine)
* :heavy_plus_sign:	Authorization checks with prefixed “@@” check against the group memberships instead of authorization for a security object, see also “@@Public” and “@@Anonymous”
* :arrows_clockwise:	Admin account now initialized with accesslevel “Extranet + Intranet” instead of “Extranet only”
* :arrows_clockwise:	Some internal code improvements

###Build 139 → 140 (ASP.NET Engine)
* :arrows_clockwise:	Fixed auto-login-feature of Single-Sign-On application which broke down by changes in Build 139
* :arrows_clockwise:	Fixed auto-account-registration of Single-Sign-On application which was caused by an earlier build
* :arrows_clockwise:	some smaller changes for better logging
* :arrows_clockwise:	Fixed removal of users (since build 138)

###Build 138 → 139 (ASP.NET Engine)
* :heavy_plus_sign:	Support for IIS-sub-applications below of the IIS-root-application
* :heavy_plus_sign:	Support for mixed .NET environments on one virtual webserver by using separate IIS-applications with separate application pools for each version of the used .NET frameworks

###Build 137 → 138 (ASP.NET Engine)
* :heavy_plus_sign:	Fixes in WebEditor for better support of .NET Framework version 2
* :heavy_plus_sign:	Changed DefaultNotifications class to provide overridable subs
* :heavy_plus_sign:	Mail methods now provide additional overloads for simplified usage of reply-to addresses
* :heavy_plus_sign:	Many improvements in  administration area for faster working with many thousands of users
* :arrows_clockwise:	Some internal changes/marked many old System_* methods as obsolete
* :arrows_clockwise:	Fixed bug saving a user with a filled LockedTemporaryTill property value
* :arrows_clockwise:	Moved code from aspx to library for sending password to user via e-mail
* :arrows_clockwise:	Some minor bug fixes in administration area in user creation form

###Build 136 → 137 (ASP.NET Engine)
* :arrows_clockwise:	Cloning of applications now resets the entries for system application
* :arrows_clockwise:	Setup page for market activation now always show current (instead of cached) data

###Build 135 → 136 (ASP.NET Engine)
* :heavy_plus_sign:	Web Editor XXL (rc1 code)

###Build 134 → 135 (ASP.NET Engine)
* :heavy_plus_sign:	Web Editor XXL (beta2 code)
* :arrows_clockwise:	Minor changes in security checks on database: IsUserAuthorized-Check now returns a false for Supervisors, too, if the security object doesn’t exist
* :heavy_plus_sign:	Text Modules (alpha code)
* :heavy_plus_sign:	Login form always opens in frameset when frameset is enabled
* :heavy_plus_sign:	Setup mechanism now knows system applications with custom authorizations (e. g. Mail Queue Monitor) and is able to handle those. The administration forms are certainly able to handle this new system application type, too.

###Build 133 → 134 (ASP.NET Engine)
* :heavy_plus_sign:	Web Editor XXL (beta code)
* :heavy_plus_sign:	ComponentArt will be unlocked by the camm Web-Manager’s HttpApplication implementation when the application starts (before, it had been unlocked in the first camm-WebManager enabled page access)
* :heavy_plus_sign:	New method to read the number of redirections; allows the creation of e. g. self-written page hit counters
* :arrows_clockwise:	Fixed internal logging
* :arrows_clockwise:	Fixed some path issues in Download Handler module with RawData/RawSingleData

###Build 132 → 133 (ASP.NET Engine)
* :heavy_plus_sign:	Logging now with more details (URL) and unlimited error descriptions
* :heavy_plus_sign:	Update mechanism is now able to execute longer than only for 30 seconds (up to 15 minutes); by this feature, update support for heavily filled databases is available
* :heavy_plus_sign:	Support for pages which require server forms by the include files for top and bottom html code; in /sysdata/includes/ you’ll find a few additional files for the new form versions
* :warning:	All system files are now depending on the new include file versions for “server form” or “without form”; those include files may require an update for showing the correct styles & Co. in both page types (with and without server forms)
* :arrows_clockwise:	Log analysis: event log errors fixed
* :arrows_clockwise:	Mail queue monitor updates (beta code)
* :arrows_clockwise:	Web Editor CMS (beta code)
* :arrows_clockwise:	Several minor bugs fixed

###Build 131 → 132 (ASP.NET Engine)
* :heavy_plus_sign:	Mail queue monitor (alpha code)

###Build 130 → 131 (Database server)
* :heavy_plus_sign:	Support for MS SQL Server 2005 editions

###Build 130 → 131 (ASP Engine)
* :arrows_clockwise:	Stack overflow bug in internationalization.asp fixed
* :arrows_clockwise:	ldirect.asp now redirects to /sysdata/login/loginprocedurefinished.aspx instead of the server group start URL
* :arrows_clockwise:	Fixed some Unicode issues
* :arrows_clockwise:	Fixed components check

###Build 130 → 131 (ASP.NET Engine)
* :warning:	New file required: /sysdata/login/loginprocedurefinished.aspx
* :warning:	When your project is a frameset project, you have to add a new item to your web.config: 
   ```
   <add key="WebManager.UseFrameset" value="true" />
   ```
* :arrows_clockwise:	Single-sign-on now redirects to the custom URL when a user is already logged on and the logon hasn’t been forced.
* :arrows_clockwise:	Fixed bugs in user import
* :arrows_clockwise:	Navigation items table contained columns for CssClass, CssClassDown, CssClassOver and ClientSideOnClick, but they were booleans instead of strings. Now, they’re strings.
* :arrows_clockwise:	When the document security check fails, the redirection link to the logon form now contains the complete URL of the origin document (previously, the protocol and server name weren’t there)
* :arrows_clockwise:	Fixed SetUserInfo: e-mail value now doesn’t override the ComesFrom flag any more
* :arrows_clockwise:	Some minor changes in BaseExplorer and e-mail test page (better exception documentation)
* :arrows_clockwise:	Auto-detection of current market must return one of the activated markets (defined in CWM database or in configuration file)
* :heavy_plus_sign:	Web Editor CMS (alpha code)
* :heavy_plus_sign:	Web.UIMenu fill method now supports HoverCssClass and SelectedCssClass
* :heavy_plus_sign:	New file /sysdata/login/loginprocedurefinished.aspx will be called by the last ldirect.asp(x); the feature of this file is to reload walk to the _top frame again and then to redirect to the origin referrer (that caused the logon form to appear) respectively the normal start page of the server group
* :heavy_plus_sign:	Support for going back to the originally requested page when a user wanted to access a protected document and had to log on, first
* :heavy_plus_sign:	New property WebManager.UseFrameset in web.config specifys wether you want to use frames or not (otherwise, when a user goes to a page which requires a logon, the originally requested page will load in the top window (frame) instead of the main window inside of the frameset)
* :heavy_plus_sign:	ComponentArt Web.UI Suite 3.0 included
* :arrows_clockwise:	Improved version of IsUserAuthorized function
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Web setup procedure undocumented/maybe buggy or incomplete
  * Documentation is missing some items

###Build 129 → 130
* :arrows_clockwise:	Send forgotten password page required case sensitive spelling of username and e-mail address as it had been saved in the database; now it’s case in-sensitive
* :arrows_clockwise:	Fixed error (object can’t be converted to WMNotifications) when sending mail with forgotten password
* :heavy_plus_sign:	Export to CSV file of single users or the complete users list
* :arrows_clockwise:	Export now requires logged on and authorized user (not possible for anonymous users if they know the deep link)
* :heavy_plus_sign:	Import of user accounts via a CSV file (if desired: inclusive memberships and authorizations update)
* :arrows_clockwise:	Some minor fixes to prevent caching problems in user information objects in very special situations
* :heavy_plus_sign:	UserInformation object now provides access to the authorizations as well as change possibilities
* :arrows_clockwise:	System_IsUserAuthorized function in ASP engine has been fixed and is in working state, now
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Web setup procedure undocumented/maybe buggy or incomplete
  * Documentation is missing some items

###Build 128 → 129
* :arrows_clockwise:	Fixed login via URL: permission check for login with method GET now returns the correct value (was always false)
* :heavy_plus_sign:	Export feature to CSV file in administration of memberships as well as authorizations
* :arrows_clockwise:	Security objects inherition state change had been wrongly shown in the log analysis as user-self-creation
* :arrows_clockwise:	The logs now show the new users correctly as self-created or created by admin
* :arrows_clockwise:	Autostart menu of CD now contains the correct phone and fax contacts
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 127 → 128
* :heavy_plus_sign:	Automatic and single sign on with ADS now available (but still limited to one single logon simultaneously)
* :arrows_clockwise:	Fixed security issue: when loading the list of authorization for a group, no user authorizations shall be loaded (and vice versa)
* :arrows_clockwise:	Fixed bug when no user login credentials were supplied in the login form (bug was created in build 127)
* :arrows_clockwise:	Fixed logging when ano server identification string is present (often the case in HttpApplication)
* :arrows_clockwise:	Some internationalization strings fixed
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 126 → 127
* :arrows_clockwise:	Update server group failed because on CreateAdminNavigationPoints procedure; now fixed
* :arrows_clockwise:	Some internal changes in update profile form to use the UserInformation object for data update instead of old sql commands
* :heavy_plus_sign:	Update profile form now allows maintenance of fields phone number, fax, mobile and also position
* :heavy_plus_sign:	New config settings for Languages.Default as well as Languages.ForcedLanguage
* :warning:	All pages inheriting from CompuMaster.camm.WebManager.Pages.Page can now only take an instance of CompuMaster.camm.WebManager.Controls.cammWebManager. To assign a CompuMaster.camm.WebManager.WMSystem property is now strictly forbidden (even if the property type keeps at CompuMaster.camm.WebManager.WMSystem for compatibility reasons).
* :heavy_plus_sign:	Support for fast authorization check against the supervisor group via the pseudo application name “@@Supervisor”
* :heavy_plus_sign:	Upgrade of RAD Editor from version 3.0 to 3.1
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 125a → 126
* :heavy_plus_sign:	In administration forms, the application authorizations dialog for adding new users, the dialog for adding new groups as well as the dialog to add new memberships now contain only the list of applications, for which the current user is authorized to update the relations
* :heavy_plus_sign:	All base controls of camm Web-Manager now implement the interface CompuMaster.camm.WebManager.Controls.IControl; this interface can be used to detect whether an object implementd the cammWebManager property or not. In this case, the initialization process of the CompuMaster.camm.WebManager.Pages.Page (the base page for all other pages) inherits its cammWebManager property to all sub controls.
* :heavy_plus_sign:	The new CompuMaster.camm.WebManager.Pages.IPage interface provides a common interface to access the camm WebManager property from outside of the page class
* :heavy_plus_sign:	Some speed tuning when assigning/looking up the camm WebManager to all sub controls on a page or control
* :heavy_plus_sign:	Resize controls have got some fixes regarding several issues, e. g. they use the download handler now for caching; they’re now considered as stable and fast
* :arrows_clockwise:	Function System_IsSecurityMaster now returns the correctly cached value (instead of a cache mixture with the supervisor flag)
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 125 → 125a
* :heavy_plus_sign:	camm RadEditor and RadSpell now choose the optimal GUI and dictionary language based on the best match between the current culture and the available data files
* :heavy_plus_sign:	New image resize controls in namespaces CompuMaster.camm.Controls.WebControls and CompuMaster.camm.Controls.WebControls.StyleTemplates for automatic, just-in-time, on-demand resizing of images (=cached and advanced by download handler)
* :heavy_plus_sign:	New supported query string parameter “PLang”: In comparison to the “Lang” parameter which switches the current market/language for the current page request and all following page requests, the “PLang” parameter changes the current market/language only for the current page request.
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 124 → 125
* :heavy_plus_sign:	New CompuMaster.camm.WebManager.Application.HttpApplication class for inherition by the global.asax file; this HttpApplication handles all exceptions thrown by a web application and sends e-mail messages to the technical contact – up to a limit of 10 e-mails per 10 minutes. All other error mails will be dropped to prevent a DoS of the SMTP server
* :arrows_clockwise:	Link to preview page for user navigation now corrected
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 123 → 124
* :arrows_clockwise:	SetUserDetail method now saves the value correctly (it saved the DoNotLogSuccess parameter accidentially)
* :arrows_clockwise:	System_SetUserDetails does not log anymore if DoNotLogSuccess parameter is true
* :heavy_plus_sign:	Mails in queue now get repeatedly tried to be send up to 3 times when there are send errors (e. g. SMTP server down)
* :heavy_plus_sign:	Speed increase for update or removal of user accounts and groups
* :heavy_plus_sign:	Notifications now in separate namespace; additional NoNotifications class, additional INotifications interface
* :heavy_plus_sign:	User self registrations and profile updates and password changes will now be logged as those (they were previously logged as a change by the administrator)
* :heavy_plus_sign:	Removed unnecessary log items when creating a new user account or updating it
* :arrows_clockwise:	One of the overloaded IsUserAuthorized Method have been executed with wrong parameters list
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 122 → 123
* :heavy_plus_sign:	Support for the navigation filling of ComponentArt’s Web.UI.SiteMap
* :heavy_plus_sign:	Automatic replacement if navigation URLs of string “[NAVID]” by the ID value of the navigation item
* :heavy_plus_sign:	Added new configuration setting “CookieLess” which leads to changed caching behaviour in cookie-less environments as well as changed default paths values for links or link elements (don’t forget to add the required session ID value into the navigation URLs in camm Web-Manager administration and to track this session ID from and to everywhere in your whole website; otherwise you would loose all session content and your logon status)
* :heavy_plus_sign:	Account register, update, password change and recovery pages now render the content table with transparent colour instead of white background
* :heavy_plus_sign:	Login and Logout methods now redirect to the login/logout page in webapplications (behaviour for non-web applications remains unchanged)
* :heavy_plus_sign:	Administration form for activation of markets/languages added
* :arrows_clockwise:	Fixed missing administration links for redirections and log analysis whenever the administration server gets created or updated (those additional navigation items have been already created once, but that was only for the state of the administration server when the first database update had been made; so those links have been freezed)
* :arrows_clockwise:	Navigation data: missing hierarchy levels will now be added even if there are more than 1 levels missing
* :arrows_clockwise:	DBSetup of build 118 fixed for databases with names other than “camm WebManager”
* :arrows_clockwise:	Fixed reinitializing when not necessary which had overridden customized paths with default values again (was a problem for cookieles scenarios)
* :arrows_clockwise:	Log table gets reduced now when maximum size exceeded
* :heavy_minus_sign:	EMailAttachment’s member for charset now obsolete and subject of removal, it hasn’t made sense
* :arrows_clockwise:	GroupInformation: Members property now read-only
* :arrows_clockwise:	Mailing functionality has moved to CompuMaster.camm.WebManager.Messaging
* :heavy_plus_sign:	GroupInformation: new read-only property MemberUserIDs
* :heavy_plus_sign:	New mail queue system for collecting mails in a queue first (fast) before they get send (slow process). This allows you to send thousands of mails in one page request without getting timeouts. You’ve also got the possibility to use a transaction which ensures that either all or no e-mail will be sent. The queuing service requires you to install the camm Web-Manager Service on one machine which will trigger the queue processing by a web service. This again says that you only need a connection to the http or https port of the web server, which is always allowed by every firewall, in principle.
* :heavy_plus_sign:	UserInfo Save methods now allows to suppress any mail messages which would be regulary sent to the users
* :heavy_plus_sign:	camm Web-Manager Service and camm Web-Manager QueueMonitor watch for items in the queues and process them. Both to the same via the web service interface, but one run as service, the other one run as window application.
* :arrows_clockwise:	Workaround for AspNetMenu: navigation fill methods now ensure that empty targets (not nothing!) will be handled as they would be nothing. By this way, navigation elements won’t open in a new window any more
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 120 → 122
* :heavy_plus_sign:	Web editor now defaults to use session instead of cookies to avoid dropping of the ASP.NET session ID when the cookie gets too long
* :heavy_plus_sign:	LabelLine control now also supports other child controls than LiteralControls
* :heavy_plus_sign:	LabelLine control now supports a variable number of linebreaks before and after the text label (default keeps at one line break after the label content)
* :heavy_plus_sign:	IsLoggedOn property now read only and now correctly reflects the log on state of a user
* :heavy_plus_sign:	Support for the navigation filling of ComponentArt’s Web.UI.Menu and Web.UI.TreeView
* :arrows_clockwise:	Fixed: update procedure via /system/admin/install/update.aspx
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 119 → 120
* :heavy_plus_sign:	New utility function IsHostFromLanOrPrivateNetwork to e. g. allow simple detection of the remote client trust state (intranet users might see more than extranet users)
* :heavy_plus_sign:	Performance tunings in SQL statements/database
* :arrows_clockwise:	Download handler (beta) table structure on database changed
* :arrows_clockwise:	Download handler (beta) fixes and changes
* :heavy_plus_sign:	New check which throws an exception if the database build number is newer than the DLL version number. This is to prevent old applications running with newer databases and corrupting data.
* :heavy_plus_sign:	New MimeTypes class with methods for general use, using first internal wrapping (for speed) and then asking the windows registry (if available) for unresolvable MIME types
* :heavy_plus_sign:	DownloadHandler module released
* :warning:	Updated UserInformation constructor with additional parameter “ExternalAccount” (string) might break down custom implementations/scripts for user creation
* :heavy_plus_sign:	Marked lots of old methods as obsolete
* :heavy_plus_sign:	Fixed “BOMAG Dealer” string in account_register.aspx form
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 118 → 119
* :heavy_plus_sign:	BaseControl now automatically accesses the cammWebManager object of the page; a manual assignment is not more needed
* :heavy_plus_sign:	Setup: all the database update scripts are now included in the DLL in compressed form to save size
* :arrows_clockwise:	Several bug fixes and improvements in logs module
* :heavy_plus_sign:	In BaseControl in CompuMaster.camm.WebManager.Controls, do automatically lookup for the cammWebManager instance of the page; this control is ideal for your individual controls which require access to camm WebManager, simply inherit from it
* :heavy_plus_sign:	Added additional base controls to CompuMaster.camm.WebManager.Controls making you more powerfull creating your own controls: Control, UserControl, TemplateControl, WebControl
* :heavy_plus_sign:	Support for centralized compact policy distribution by the camm Web-Manager control
* :heavy_plus_sign:	New system fields in user information: PhoneNumber, FaxNumber, MobileNumber, Position
* :heavy_plus_sign:	Automatic reading of settings in web.config/app.config
* :heavy_plus_sign:	Automatic creation of the cammWebManager object in CompuMaster.camm.WebManager.Pages.Page to allow simplified usage of camm Web-Manager; now you can use a simple 
* :heavy_plus_sign:	Added new salutation properties in UserInformation class (and marked old ones as obsolete)
* :arrows_clockwise:	System_SetUserDetail wrote all properties with user ID 0 (zero) instead of that one it got via the parameters
* :arrows_clockwise:	IsSecurityMaster now returns the correct value (was always true)
* :arrows_clockwise:	Fixed some Unicode issues
* :arrows_clockwise:	Fixed administration form to create/modify redirection to save the settings always
* :arrows_clockwise:	Refreshed data protection pages
* :arrows_clockwise:	Fixed bug with involvement of the correct mail sending system
* :arrows_clockwise:	Fixed bug with chilkat mail version checks and their logging
* :heavy_plus_sign:	Introduced UIMarket and UILanguage functions, making CurLanguage and CurLanguageInfo obsolete
* Known Bugs
  * Logfile analysis run into errors if you have configured more than 11 server groups or servers in a server group
  * Setup procedure undocumented/maybe buggy or incomplete

###Build 117 → 118
* :arrows_clockwise:	Logfile application enabled per default
* :arrows_clockwise:	Pages/Controls in sysdata: internal structure of custom_internationalization.vb changed completely
* :arrows_clockwise:	DLL: WMSettingsAndData methods now overridable to allow inherition from that class
* :arrows_clockwise:	Fixed compilation errors in /system/feedback.aspx and /system/help.aspx
* :arrows_clockwise:	Setup: separate /system/admin/install/update.aspx for patch management for the database (no possibility to accidentially overwrite/recreate an existent camm Web-Manager database)

###Build 116a → 117
* :arrows_clockwise:	System_IsUserAuthorizedForApp: Bug fixed with public users
* :heavy_plus_sign:	Logfile analysis

###Build 116 → 116a
* :heavy_plus_sign:	Configurable Redirections
* :heavy_plus_sign:	Additional field “ReviewedAndClosed” in table “Log”
* :heavy_plus_sign:	Support for Chilkat components version 6

###Build 115 → 116
* :heavy_plus_sign:	Authentification on SMTP server now possible

#Build no. up to 111

##Legend
* :heavy_plus_sign: Added feature
* :heavy_minus_sign: Removed
* :arrows_clockwise: Changed
* :warning: Breaking changes; pay attention!

##Changes while developing in following builds

###Build 111
* Database structure/content
  * :heavy_plus_sign: Added table for new module "Download Handler"
  * :heavy_plus_sign: Added new navigation item for new module "Logfiles"
  * :arrows_clockwise::warning: Additional parameters for stored procedures dbo.Public_Logout
* Library
  * :heavy_plus_sign: New controls for easy web editing in namespace CompuMaster.camm.WebManager.Modules.WebEdit.Controls
  * :heavy_plus_sign: Automatic data binding for page on load of cammWebManager control
  * :arrows_clockwise: Some internal changes
* Pages/Controls
  * :heavy_plus_sign: Editorial in /sysdata/editorial.aspx can now be edited by the supervisor with an easy to use web editor
  * :arrows_clockwise::warning: Include files (default templates in /sysdata) are using controls, now. If you've got pages with the same include for several times, you'll get compilation errors because of the existance of duplicated web controls with same name
  * :arrows_clockwise::warning: Internal Structure of /sysdata/editorial.aspx and /sysdata/disclaimer.aspx changed to handle with the new control based logic

###Build 110
* Database
  * :arrows_clockwise::warning: Additional parameters for stored procedures dbo.Public_GetToDoLogonList and dbo.Public_GetLogonList
  * :arrows_clockwise: Some bug fixes
* Library
  * :arrows_clockwise::warning: Some corrections of type of UserID: integers should be long integers; that's why some methods have been overloaded. This may raise a problem if you call that overloaded methods with lazy parameter types
  * :arrows_clockwise: Some internal changes
* Pages/Controls
  * :heavy_plus_sign: New chart generator for easy purposes available in /system/modules/charts/chartgenerator.aspx, a sample on how to use is in /modules/charts/index.aspx

###Build 109
* Database
  * Some bug fixes
* Library
  * Some bug fixes
* Pages/Controls

###Build 108
* First stable release
