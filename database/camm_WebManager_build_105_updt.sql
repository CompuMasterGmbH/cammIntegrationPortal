/******************************************************
  SystemModule: Minor fixes
	- Add Supervisors to the group of Security Admins if there are no members in Security Admins
	- dbo.Public_UserIsAuthorizedForApp / Speedtuning
******************************************************/
-- Add Supervisors to the group of Security Admins if there are no members in Security Admins
declare @securityadmincount int
select @securityadmincount = count(id_user) from memberships where id_group = 7

if @securityadmincount = 0
	insert into memberships (id_group, id_user, releasedon, releasedby)
	select 7, id_user, getdate(), -43
	from memberships
	where id_group = 6
GO

------------------------------------------------------------------------------------------------------------------------
-- Ergänzende Datenkorrekturmaßnahmen
------------------------------------------------------------------------------------------------------------------------
update dbo.Log 
set ConflictType = 31
where ConflictDescription Like 'Now inher%' And ConflictType = 1
GO


----------------------------------------------------
-- dbo.Public_UserIsAuthorizedForApp
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UserIsAuthorizedForApp
(
	@Username nvarchar(20),
	@WebApplication varchar(255),
	@ServerIP nvarchar(32)
)

AS 

DECLARE @CurUserID int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByAnonymousGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @WebAppID int
DECLARE @RequestedServerID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID)))


------------------------------
-- UserLoginValidierung --
------------------------------

		If NullIf(@WebApplication, '') = 'Public'
			Return 1 -- Zugriff gewährt
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT TOP 1 @bufferUserIDByAnonymousGroup = ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID)
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByPublicGroup = ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @PublicGroupID)
		If NullIf(@bufferUserIDByPublicGroup, -1) <> -1 
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByUser = ID_User FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_User = @CurUserID)
		If NullIf(@bufferUserIDByUser, -1) <> -1 
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByGroup = Memberships.ID_User FROM Memberships INNER JOIN view_ApplicationRights ON Memberships.ID_Group = view_ApplicationRights.ID_Group WHERE (view_ApplicationRights.Title = @WebApplication) AND (view_ApplicationRights.LocationID = @RequestedServerID) AND (Memberships.ID_User = @CurUserID)
		If NullIf(@bufferUserIDByGroup, -1) <> -1
			Return 1 -- Zugriff gewährt
		SELECT TOP 1 @bufferUserIDByAdmin = ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6)
		If NullIf(@bufferUserIDByAdmin, -1) <> -1 
			Return 1 -- Zugriff gewährt
		Else
			Return 0 -- kein Zugriff auf aktuelles Dokument

GO


/*************************************************************
**                                                          **
** Update users tables for support of more nvarchar fields  **
** instead of varchar and also bugfixing some SPs using old **
** field sizes                                              **
**                                                          **
**************************************************************/

BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
GO
CREATE TABLE dbo.Tmp_Benutzer
	(
	ID int NOT NULL IDENTITY (1, 1),
	Loginname nvarchar(50) NOT NULL,
	LoginPW varchar(4096) NOT NULL,
	Company nvarchar(100) NULL,
	Anrede varchar(20) NOT NULL,
	Titel nvarchar(40) NULL,
	Vorname nvarchar(60) NOT NULL,
	Nachname nvarchar(60) NOT NULL,
	Namenszusatz nvarchar(40) NULL,
	[E-MAIL] nvarchar(50) NOT NULL,
	Strasse nvarchar(60) NULL,
	PLZ nvarchar(20) NULL,
	Ort nvarchar(100) NULL,
	State nvarchar(60) NULL,
	Land nvarchar(60) NULL,
	LoginCount decimal(18, 0) NOT NULL,
	LoginFailures int NOT NULL,
	LoginLockedTill datetime NULL,
	LoginDisabled bit NOT NULL,
	AccountAccessability int NOT NULL,
	CreatedOn datetime NOT NULL,
	ModifiedOn datetime NOT NULL,
	LastLoginOn datetime NULL,
	LastLoginViaRemoteIP nvarchar(50) NULL,
	CurrentLoginViaRemoteIP nvarchar(50) NULL,
	[1stPreferredLanguage] int NULL,
	[2ndPreferredLanguage] int NULL,
	[3rdPreferredLanguage] int NULL,
	CustomerNo nvarchar(100) NULL,
	SupplierNo nvarchar(100) NULL,
	System_SessionID int NULL
	)  ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_Benutzer ON;
IF EXISTS(SELECT * FROM dbo.Benutzer)
	 EXEC('INSERT INTO dbo.Tmp_Benutzer (ID, Loginname, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [E-MAIL], Strasse, PLZ, Ort, State, Land, LoginCount, LoginFailures, LoginLockedTill, LoginDisabled, AccountAccessability, CreatedOn, ModifiedOn, LastLoginOn, LastLoginViaRemoteIP, CurrentLoginViaRemoteIP, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], CustomerNo, SupplierNo, System_SessionID)
		SELECT ID, Loginname, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [E-MAIL], Strasse, PLZ, Ort, State, Land, LoginCount, CONVERT(int, LoginFailures), LoginLockedTill, LoginDisabled, AccountAccessability, CreatedOn, ModifiedOn, LastLoginOn, LastLoginViaRemoteIP, CurrentLoginViaRemoteIP, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], CustomerNo, SupplierNo, System_SessionID FROM dbo.Benutzer TABLOCKX')
GO
SET IDENTITY_INSERT dbo.Tmp_Benutzer OFF
GO
DROP TABLE dbo.Benutzer
GO
ALTER TABLE dbo.Tmp_Benutzer ADD CONSTRAINT
	DF__Benutzer__LoginC_105_37CE DEFAULT (0) FOR LoginCount
GO
ALTER TABLE dbo.Tmp_Benutzer ADD CONSTRAINT
	DF__Benutzer__LoginF_105_3C07 DEFAULT (0) FOR LoginFailures
GO
ALTER TABLE dbo.Tmp_Benutzer ADD CONSTRAINT
	DF__Benutzer__LoginD_105_3040 DEFAULT (0) FOR LoginDisabled
GO
ALTER TABLE dbo.Tmp_Benutzer ADD CONSTRAINT
	DF__Benutzer__Create_105_3479 DEFAULT (getdate()) FOR CreatedOn
GO
ALTER TABLE dbo.Tmp_Benutzer ADD CONSTRAINT
	DF__Benutzer__Modifi_105_38B2 DEFAULT (getdate()) FOR ModifiedOn
GO
EXECUTE sp_rename N'dbo.Tmp_Benutzer', N'Benutzer', 'OBJECT'
GO
ALTER TABLE dbo.Benutzer ADD CONSTRAINT
	PK_Benutzer PRIMARY KEY CLUSTERED 
	(
	ID
	) WITH FILLFACTOR = 90 ON [PRIMARY]

GO
ALTER TABLE dbo.Benutzer ADD CONSTRAINT
	IX_Benutzer UNIQUE NONCLUSTERED 
	(
	Loginname
	) WITH FILLFACTOR = 90 ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_Benutzer_1 ON dbo.Benutzer
	(
	System_SessionID
	) WITH FILLFACTOR = 90 ON [PRIMARY]
GO

----------------------------------------------------
-- dbo.Public_ValidateGUIDLogin
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateGUIDLogin
(
	@Username nvarchar(20),
	@GUID int,
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512)
)

AS

DECLARE @CurUserID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

If @CurUserID Is Null
	-- User does not exist
	Return 1

UPDATE dbo.System_WebAreasAuthorizedForSession SET ScriptEngine_SessionID = @ScriptEngine_SessionID, LastSessionStateRefresh = GetDate()
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     
                      (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID = @GUID) AND (Benutzer.Loginname = @Username) AND 
                      (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND (System_Servers.IP = @ServerIP)

IF @@ROWCOUNT = 1 
	insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
	values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 97, 'Prepare GUID login - Script Engine #' + Cast(@ScriptEngine_ID As NVarchar(20)))
Else
	-- User has no such ScriptEngine_SessionID
	Return 2

SET NOCOUNT OFF

-- Erfolgsmitteilung
Select @UserName
Return -1
GO
----------------------------------------------------
-- dbo.Public_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UpdateUserPW 
(
	@Username nvarchar(20),
	@OldPasscode varchar(4096),
	@NewPasscode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication varchar(4096)
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @OldPasscode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnt die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@OldPasscode)
			BEGIN
				IF ASCII(SUBSTRING(@OldPasscode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate() WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 6, 'User password changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.Public_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UpdateUserDetails 
(
	@Username nvarchar(20),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication varchar(1024),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures int
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures int
DECLARE @CurUserAccountAccessability int
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @Passcode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@Passcode)
			BEGIN
				IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, State = @State, Land = @Land, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -4, 'User profile changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO

----------------------------------------------------
-- dbo.Public_SetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_SetUserDetailData
	(
		@IDUser int,
		@Type varchar(50),
		@Value nvarchar(255),
		@DoNotLogSuccess bit = 0
	)

AS
DECLARE @CountOfValuesInTable int

	set nocount on
	-- How many rows exist?
	SET @CountOfValuesInTable = (SELECT COUNT(ID) FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type)
	
	If @CountOfValuesInTable > 0 
		-- Remove old settings first
		DELETE FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
	-- Append value to table
	If @Type Is Not Null And @Value Is Not Null 
		INSERT INTO dbo.Log_Users (ID_User, Type, Value) VALUES (@IDUser, @Type, @Value) 
	-- Logging
	if @DoNotLogSuccess = 0 
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@IDUser, GetDate(), '0.0.0.0', '0.0.0.0', -9, 'User attributes modified')
	-- Exit
	set nocount off
	SELECT -1
	return
GO

----------------------------------------------------
-- [dbo].[Public_ServerDebug]
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ServerDebug
(
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32)
)

AS

-- Deklaration Variablen/Konstanten
DECLARE @WebApplication varchar(1024)
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime

DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Registered_ScriptEngine_SessionID varchar(512)
DECLARE @RequestedServerID int
DECLARE @WebAppID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @WebApplication = 'Public'

SET NOCOUNT ON

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return

	END
----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID)))

------------------------------
-- UserLoginValidierung --
------------------------------

-- ReAuthentifizierung?
	-- Does the user has got authorization?
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ApplicationsRightsByGroup.ID_GroupOrPerson FROM Memberships RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID) AND (ApplicationsRightsByGroup.ID_GroupOrPerson = @PublicGroupID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships.ID_User FROM Memberships RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (((Memberships.ID_User = @CurUserID) AND (Applications.Title = @WebApplication))) AND Applications.LocationID = @LocationID)
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6))
		If NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
			SET @MyResult = 1 -- Zugriff gewährt
		Else
			SET @MyResult = 2 -- kein Zugriff auf aktuelles Dokument

IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		SET NOCOUNT ON
	END
Else -- @MyResult = 0 Or @MyResult = 2
	-- Login failed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2	
		SET NOCOUNT ON
	END
GO

----------------------------------------------------
-- dbo.Public_RestorePassword
----------------------------------------------------
ALTER Procedure dbo.Public_RestorePassword
(
		@Username nvarchar(20),
		@eMail nvarchar(50)
)

AS
	SELECT Result = (SELECT SUBSTRING(LoginPW, 1, len(LoginPW)) FROM dbo.Benutzer WHERE Loginname = @Username And [e-mail] = @eMail)
GO

----------------------------------------------------
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
(
	@Username nvarchar(20),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32)
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END

-------------
-- Logout --
-------------
-- Rückgabewert
SELECT Result = -1
-- CurUserCurrentLoginViaRemoteIP und SessionIDs zurücksetzen
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null WHERE LoginName = @Username
-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 99, 'Logout')
GO

----------------------------------------------------
-- dbo.Public_GetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserDetailData
	(
		@IDUser int,
		@Type varchar(50)
	)

As

If @Type = 'Sex'
	SELECT CASE WHEN Anrede = 'Mr.' THEN 'm' Else 'w' END As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LoginName'
	SELECT LoginName As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'Addresses'
	SELECT Anrede As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LastName'
	SELECT Nachname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'FirstName'
	SELECT Vorname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '1stPreferredLanguage'
	SELECT [1stPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '2ndPreferredLanguage'
	SELECT [2ndPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '3rdPreferredLanguage'
	SELECT [3rdPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'AccessLevel'
	SELECT [AccountAccessability] As Result FROM Benutzer WHERE ID = @IDUser
Else
	SELECT Value As Result FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
GO
----------------------------------------------------
-- dbo.Public_GetToDoLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetToDoLogonList
	(
	@Username nvarchar(20),
	@ScriptEngine_SessionID nvarchar(512),
	@ScriptEngine_ID int
	)

AS

-- GUIDs alter Sessions zurücksetzen
SET NOCOUNT ON
UPDATE    System_WebAreasAuthorizedForSession
SET              Inactive = 1
WHERE     (LastSessionStateRefresh < DATEADD(hh, - 12, GETDATE()))

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
             
         System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     (System_WebAreasAuthorizedForSession.ScriptEngine_SessionID IS NULL) AND (System_Servers.ID > 0) AND 
		(System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND 
		(Benutzer.Loginname = @Username)
	 OR
                (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND (System_Servers.ID > 0) AND 
		(Benutzer.Loginname = @Username) AND 
-- following line might not make sense
		-- (System_WebAreasAuthorizedForSession.ScriptEngine_SessionID = @ScriptEngine_SessionID) AND (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND
-- therefore here is a replacement
		(System_WebAreasAuthorizedForSession.ScriptEngine_ID <> @ScriptEngine_ID) AND
-- end following non sense block
                      (System_WebAreasAuthorizedForSession.LastSessionStateRefresh < DATEADD(minute, - 3, GETDATE()))
GO

----------------------------------------------------
-- dbo.Public_GetServerConfig
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetServerConfig
(
@ServerIP nvarchar(32)
)

AS SELECT     dbo.System_ServerGroups.ServerGroup AS ServerGroupDescription, dbo.System_ServerGroups.ID_Group_Public, 
                      System_Servers_1.ServerProtocol AS MasterServerProtocol, System_Servers_1.ServerName AS MasterServerName, 
                      System_Servers_1.ServerPort AS MasterServerPort, System_Servers_2.ServerProtocol AS UserAdminServerProtocol, 
                      System_Servers_2.ServerName AS UserAdminServerName, System_Servers_2.ServerPort AS UserAdminServerPort, 
                      dbo.System_ServerGroups.UserAdminServer, System_Servers_3.*, dbo.System_ServerGroups.AreaImage AS ServerGroupImageBig, 
                      dbo.System_ServerGroups.AreaButton AS ServerGroupImageSmall, COALESCE (dbo.System_ServerGroups.AreaNavTitle, 
                      dbo.System_ServerGroups.ServerGroup) AS ServerGroupTitle_Navigation, dbo.System_ServerGroups.AreaCompanyFormerTitle, 
                      dbo.System_ServerGroups.AreaCompanyTitle, dbo.System_ServerGroups.AreaSecurityContactEMail, 
                      dbo.System_ServerGroups.AreaSecurityContactTitle, dbo.System_ServerGroups.AreaDevelopmentContactEMail, 
                      dbo.System_ServerGroups.AreaDevelopmentContactTitle, dbo.System_ServerGroups.AreaContentManagementContactEMail, 
                      dbo.System_ServerGroups.AreaContentManagementContactTitle, dbo.System_ServerGroups.AreaUnspecifiedContactEMail, 
                      dbo.System_ServerGroups.AreaUnspecifiedContactTitle, dbo.System_ServerGroups.AreaCopyRightSinceYear, 
                      dbo.System_ServerGroups.AreaCompanyWebSiteURL, dbo.System_ServerGroups.AreaCompanyWebSiteTitle, 
                      dbo.System_ServerGroups.ID AS ID_ServerGroup, dbo.System_ServerGroups.AccessLevel_Default
FROM         dbo.System_ServerGroups INNER JOIN
                      dbo.System_Servers System_Servers_2 ON dbo.System_ServerGroups.UserAdminServer = System_Servers_2.ID INNER JOIN
                      dbo.System_Servers System_Servers_1 ON dbo.System_ServerGroups.MasterServer = System_Servers_1.ID INNER JOIN
                      dbo.System_Servers System_Servers_3 ON dbo.System_ServerGroups.ID = System_Servers_3.ServerGroup

WHERE     (System_Servers_3.IP = @ServerIP)
GO

----------------------------------------------------
-- dbo.Public_GetLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetLogonList
	(
	@Username nvarchar(20)
	)

AS

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
                      System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID, 
                      System_WebAreasAuthorizedForSession.LastSessionStateRefresh
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND (Benutzer.Loginname = @Username) AND (System_Servers.ID > 0)
GO

----------------------------------------------------
-- dbo.Public_GetCurServerLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetCurServerLogonList
(
@ServerIP nvarchar(32)
)

AS 

DECLARE @LocationID int
----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup

FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Abbruch
		Return
	END

---------------------------------------
-- Anmeldestellen zurückliefern --
---------------------------------------
SELECT     NULL AS ID, NULL AS SessionID, System_Servers.IP, System_Servers.ServerDescription, System_Servers.ServerProtocol, 
                      System_Servers.ServerName, System_Servers.ServerPort, NULL AS ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, NULL AS ScriptEngine_SessionID, System_WebAreaScriptEnginesAuthorization.ID AS OrderID1, 
                      System_Servers.ID AS OrderID2, System_ScriptEngines.ID AS OrderID3
FROM         System_Servers INNER JOIN
                      System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server INNER JOIN
                      System_ScriptEngines ON System_WebAreaScriptEnginesAuthorization.ScriptEngine = System_ScriptEngines.ID
WHERE     (System_Servers.Enabled <> 0) AND (System_Servers.ServerGroup = @LocationID) AND (System_Servers.ID > 0)
ORDER BY System_WebAreaScriptEnginesAuthorization.ID, System_Servers.ID, System_ScriptEngines.ID
GO

----------------------------------------------------
-- dbo.Public_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.Public_CreateUserAccount
(
	@Username nvarchar(20),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int = 0,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @LocationID int
DECLARE @CurUserStatus_InternalSessionID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0

	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, account will be created now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Create account
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, CreatedOn, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], LoginCount, CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, 1, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = @@IDENTITY --(select ID from dbo.Benutzer where LoginName = @Username)
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = @@IDENTITY
		UPDATE dbo.Benutzer SET System_SessionID = @CurUserStatus_InternalSessionID WHERE LoginName = @UserName
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		INSERT INTO dbo.System_WebAreasAuthorizedForSession
		                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
		SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
		                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
		FROM         dbo.System_Servers INNER JOIN
		                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
		WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @LocationID)
		SET NOCOUNT OFF
		-- Logging
		SET NOCOUNT ON
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 1, 'Account ' + @Username + ' created')
		SET NOCOUNT OFF
	END
Else
	-- Rückgabewert
	SELECT Result = 1
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO
----------------------------------------------------
-- dbo.Int_UpdateUserDetailDataWithProfileData
----------------------------------------------------
ALTER Procedure Int_UpdateUserDetailDataWithProfileData
	(
		@IDUser int
	)

As
DECLARE @LoginName nvarchar(20)
	-- Result and Initializing
	SELECT -1
	set nocount on
-- GetCompleteName (a little bit modified)
	DECLARE @Addresses nvarchar (20)
	DECLARE @Titel nvarchar (20)
	DECLARE @Vorname nvarchar(60)
	DECLARE @Nachname nvarchar(60)
	DECLARE @Namenszusatz nvarchar(40)
	DECLARE @CompleteName nvarchar (255)
	DECLARE @CompleteNameInclAddresses nvarchar (255)
	DECLARE @CustomerNo nvarchar(100) 
	DECLARE @SupplierNo nvarchar(100)
	DECLARE @Company nvarchar(100)
	DECLARE @eMail nvarchar (50)
	DECLARE @Sex varchar (1)
	
	SELECT @Sex = Case Anrede When 'Mr.' Then 'm' When 'Ms.' Then 'w' Else Null End, @eMail = [E-MAIL], @Vorname = Vorname, @Nachname = Nachname, @Nachname = Nachname, @Namenszusatz = Namenszusatz, @CustomerNo = CustomerNo, @SupplierNo = SupplierNo, @Company = Company, @Titel = Titel, @Addresses = Anrede FROM dbo.Benutzer WHERE ID = @IDUser

	-- Namenszusatz könnte Null sein
	If substring(@Namenszusatz,1,20) <> '' --Is Not Null
		SET @Namenszusatz = ' ' + @Namenszusatz
	Else
		SET @Namenszusatz = ''
	SET @CompleteName = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	SET @CompleteNameInclAddresses  = LTrim(RTrim(@Addresses + @Titel)) + ' ' + LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	Exec Public_SetUserDetailData @IDUser, 'CompleteName', @CompleteName, 1
	Exec Public_SetUserDetailData @IDUser, 'CompleteNameInclAddresses', @CompleteNameInclAddresses, 1
	Exec Public_SetUserDetailData @IDUser, 'Sex', @Sex, 1
	-- e-mail address
	Exec Public_SetUserDetailData @IDUser, 'email', @eMail, 1
	-- Other details
	Exec Public_SetUserDetailData @IDUser, 'CustomerNo', @CustomerNo, 1
	Exec Public_SetUserDetailData @IDUser, 'SupplierNo', @SupplierNo, 1
	Exec Public_SetUserDetailData @IDUser, 'Company', @Company, 1

-- Exit

	return 
GO

----------------------------------------------------
-- dbo.Int_LogAuthChanges
----------------------------------------------------
ALTER PROCEDURE dbo.Int_LogAuthChanges
(
@UserID int = Null,
@GroupID int = Null,
@AppID int
)

AS 

If @GroupID Is Not Null
	begin
		-- log indirect changes on users
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		select id_user, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -7, Null
		from view_Memberships_CummulatedWithAnonymous
		where id_group = @GroupID
		-- log group auth change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -8, @GroupID)
	end
Else
	If @UserID Is Not Null
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -6, Null)
GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServer
	(
		@ServerID int
	)

AS

-- Script engines of connected servers will be UNREGISTERED. 
DELETE System_WebAreaScriptEnginesAuthorization
FROM System_Servers INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_Servers.ID = @ServerID 

-- Related logs will be DELETED permanently. 
DELETE Log
FROM System_Servers INNER JOIN Log ON System_Servers.IP = Log.ServerIP
WHERE System_Servers.ID = @ServerID 
DELETE System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
FROM System_Servers INNER JOIN System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes ON System_Servers.ID = System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.Server
WHERE System_Servers.ID = @ServerID 

-- Script engine relations must be erased as well
DELETE 
FROM System_WebAreaScriptEnginesAuthorization
WHERE Server = @ServerID

-- DELETE the server itself
DELETE 
FROM dbo.System_Servers
WHERE ID = @ServerID
GO
----------------------------------------------------
-- dbo.Public_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UpdateUserDetails 
(
	@Username nvarchar(20),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication varchar(1024), -- dummy
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @Passcode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@Passcode)
			BEGIN
				IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, State = @State, Land = @Land, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -4, 'User profile changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO
----------------------------------------------------
-- dbo.Public_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UpdateUserPW 
(
	@Username nvarchar(20),
	@OldPasscode varchar(4096),
	@NewPasscode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication varchar(4096) -- dummy / reserved
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @OldPasscode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnt die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@OldPasscode)
			BEGIN
				IF ASCII(SUBSTRING(@OldPasscode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate() WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 6, 'User password changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
----------------------------------------------------
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
	@Username nvarchar(20),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @System_SessionID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = ID, @System_SessionID = System_SessionID from dbo.Benutzer where LoginName = @Username
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END

-------------
-- Logout --
-------------
-- Rückgabewert
SELECT Result = -1
-- CurUserCurrentLoginViaRemoteIP und SessionIDs zurücksetzen
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null WHERE LoginName = @Username
-- Session schließen
UPDATE dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
SET Inactive = 1
WHERE SessionID = @System_SessionID
-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 99, 'Logout')
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

alter table dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
alter column ScriptEngine_SessionID nvarchar(128) NULL
GO

if exists (select * from sys.indexes where name = N'IX_System_WebAreasAuthorizedForSession_2' and object_id = object_id(N'[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]'))
DROP INDEX [IX_System_WebAreasAuthorizedForSession_2] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
GO

 CREATE  INDEX [IX_System_WebAreasAuthorizedForSession_2] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]([ScriptEngine_SessionID]) WITH  FILLFACTOR = 90 ON [PRIMARY]
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetUserNameForScriptEngineSessionID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_GetUserNameForScriptEngineSessionID]
GO

CREATE PROCEDURE dbo.Public_GetUserNameForScriptEngineSessionID
(
	@UserName nvarchar(20) output,
	@ScriptEngine_SessionID nvarchar(128),
	@ScriptEngine_ID int,
	@ServerID int
)

AS
select @UserName = dbo.Benutzer.LoginName
from dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes 
	left join dbo.System_UserSessions ON dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.SessionID = dbo.System_UserSessions.ID_Session
	left join dbo.Benutzer ON dbo.System_UserSessions.ID_User = dbo.Benutzer.ID
where scriptengine_sessionID = @ScriptEngine_SessionID 
	and server=@ServerID
	and scriptengine_ID=@ScriptEngine_ID
	and inactive=0
order by sessionid desc

GO


----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateUserDetails 
	@CurUserID int,
	@WebApplication varchar(1024),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int,
	@LoginDisabled bit = 0,
	@LoginLockedTill datetime,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null

AS

SET NOCOUNT ON
-- Profile update
	-- Password update
	UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, Land = @Land, State = @State, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, AccountAccessability = @AccountAccessability, LoginDisabled = @LoginDisabled, LoginLockedTill = @LoginLockedTill, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE ID = @CurUserID
	-- Logging
	insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 4, 'Admin has modified profile')
	-- Write UserDetails
	Exec Int_UpdateUserDetailDataWithProfileData @CurUserID

-- Rückgabewert
SET NOCOUNT OFF
SELECT Result = -1

GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateUserAccount
(
	@Username nvarchar(20),
	@Passcode varchar(4096),
	@WebApplication varchar(1024),
	@ServerIP nvarchar(32),
	@Company nvarchar(100),
	@Anrede varchar(20),
	@Titel nvarchar(40),
	@Vorname nvarchar(60),
	@Nachname nvarchar(60),
	@Namenszusatz nvarchar(40),
	@eMail nvarchar(50),
	@Strasse nvarchar(60),
	@PLZ nvarchar(20),
	@Ort nvarchar(100),
	@State nvarchar(60),
	@Land nvarchar(60),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability tinyint,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @LocationID int
DECLARE @WebAppID int

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID)))


-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', @WebAppID, 3, 'Account ' + @Username + ' created')
		SET NOCOUNT OFF
		-- Rückgabewert
		If @CurUserID Is Not Null 
			SELECT Result = -1 
		Else
			SELECT Result = -2
	END

Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO
