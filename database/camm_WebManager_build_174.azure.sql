/* Überprüfen Sie das Skript gründlich, bevor Sie es außerhalb des Datenbank-Designer-Kontexts ausführen, um potenzielle Datenverluste zu vermeiden.*/
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
declare @table_name nvarchar(256)
declare @col_name nvarchar(256)

set @table_name = N'dbo.Log_Users'
set @col_name = N'ModifiedOn'

DECLARE @ObjectName NVARCHAR(100)
SELECT @ObjectName = OBJECT_NAME([default_object_id]) FROM SYS.COLUMNS
WHERE [object_id] = OBJECT_ID(@table_name) AND [name] = @col_name;
EXEC('ALTER TABLE '+@table_name+' DROP CONSTRAINT ' + @ObjectName)
GO
CREATE TABLE dbo.Tmp_Log_Users
	(
	ID int NOT NULL IDENTITY (1, 1)  PRIMARY KEY,
	ID_User int NOT NULL,
	Type nvarchar(80) NOT NULL,
	Value nvarchar(255) NULL,
	ModifiedOn datetime NOT NULL,
	ModifiedBy int NULL
	)  
GO

ALTER TABLE dbo.Tmp_Log_Users ADD CONSTRAINT
	DF__Log_Users__Modif__2E1BDC42 DEFAULT (getdate()) FOR ModifiedOn
GO
SET IDENTITY_INSERT dbo.Tmp_Log_Users ON;
INSERT INTO dbo.Tmp_Log_Users (ID, ID_User, Type, Value, ModifiedOn)
SELECT ID, ID_User, CONVERT(nvarchar(80), Type), Value, ModifiedOn 
FROM dbo.Log_Users 
 
;SET IDENTITY_INSERT dbo.Tmp_Log_Users OFF
GO
DROP TABLE dbo.Log_Users
GO
EXECUTE sp_rename N'dbo.Tmp_Log_Users', N'Log_Users', 'OBJECT' 
GO
--WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 

GO
CREATE NONCLUSTERED INDEX IX_Log_Users ON dbo.Log_Users
	(
	ID_User
	) --WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
GO
CREATE NONCLUSTERED INDEX IX_Log_Users_1 ON dbo.Log_Users
	(
	Type
	) --WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
GO
CREATE NONCLUSTERED INDEX IX_Log_Users_2 ON dbo.Log_Users
	(
	ID_User,
	Type
	) --WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
GO


----------------------------------------------------
-- dbo.Public_SetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_SetUserDetailData
	(
		@IDUser int,
		@Type varchar(50),
		@Value nvarchar(255),
		@DoNotLogSuccess bit = 0,
		@ModifiedBy int = 0
	)
WITH ENCRYPTION
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
		INSERT INTO dbo.Log_Users (ID_User, Type, Value, ModifiedBy) VALUES (@IDUser, @Type, @Value, @ModifiedBy) 
	-- Logging
	if @DoNotLogSuccess = 0 
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@IDUser, GetDate(), '0.0.0.0', '0.0.0.0', -9, 'User attributes modified by user id ' + CAST(@ModifiedBy as varchar(20)))
	-- Exit
	set nocount off
	SELECT -1
	return
GO


----------------------------------------------------
-- dbo.Int_UpdateUserDetailDataWithProfileData
----------------------------------------------------
ALTER Procedure Int_UpdateUserDetailDataWithProfileData
	(
		@IDUser int,
		@ModifiedBy int = 0
	)
WITH ENCRYPTION
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
	Exec Public_SetUserDetailData @IDUser, 'CompleteName', @CompleteName, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'CompleteNameInclAddresses', @CompleteNameInclAddresses, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'Sex', @Sex, 1, @ModifiedBy
	-- e-mail address
	Exec Public_SetUserDetailData @IDUser, 'email', @eMail, 1, @ModifiedBy
	-- Other details
	Exec Public_SetUserDetailData @IDUser, 'CustomerNo', @CustomerNo, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'SupplierNo', @SupplierNo, 1, @ModifiedBy
	Exec Public_SetUserDetailData @IDUser, 'Company', @Company, 1, @ModifiedBy

-- Exit

	return 
GO



----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateUserDetails 
(
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
	@SupplierNo nvarchar(50) = Null,
	@DoNotLogSuccess bit = 0,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)
WITH ENCRYPTION
AS

SET NOCOUNT ON
	-- Profile update
	UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, Land = @Land, State = @State, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, AccountAccessability = @AccountAccessability, LoginDisabled = @LoginDisabled, LoginLockedTill = @LoginLockedTill, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE ID = @CurUserID
	-- Logging
	IF IsNull(@DoNotLogSuccess, 0) = 0
	BEGIN
		If IsNull(@IsUserChange, 0) = 0 
			-- admin
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 4, 'Admin with user id ' + CAST(@ModifiedBy as varchar(20)) + ' has modified profile')
		Else
			-- user change
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', -4, 'User has modified profile')
	END
	-- Write UserDetails
	Exec Int_UpdateUserDetailDataWithProfileData @CurUserID, @ModifiedBy

-- Rückgabewert
SET NOCOUNT OFF
SELECT Result = -1

GO



----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_UpdateUserPW 
(
	@Username nvarchar(20),
	@NewPasscode varchar(4096),
	@DoNotLogSuccess bit = 0,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Password update
		UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate(), LoginLockedTill = Null, LoginFailures = 0 WHERE LoginName = @Username
		-- Logging
		IF IsNull(@DoNotLogSuccess, 0) = 0
		BEGIN
			If IsNull(@IsUserChange, 0) = 0 
				-- admin
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription)
 				values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 5, 'Admin with user id ' + CAST(@ModifiedBy as varchar(20)) + ' has modified password')
			Else
				-- user change
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
				values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 6, 'User has modified password')
		END	END
Else
	-- Rückgabewert
	SELECT Result = 0
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
	@SupplierNo nvarchar(50) = Null,
	@IsUserChange bit = 0,
	@ModifiedBy int = 0
)
WITH ENCRYPTION
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
		If IsNull(@IsUserChange, 0) = 0 
			-- admin
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, '0.0.0.0', @WebAppID, 3, 'Account ' + @Username + ' created by admin with user id ' + CAST(@ModifiedBy as varchar(20)))
		Else
			-- user change
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, '0.0.0.0', @WebAppID, 1, 'Account ' + @Username + ' created by user itself')
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
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID, @ModifiedBy
GO
