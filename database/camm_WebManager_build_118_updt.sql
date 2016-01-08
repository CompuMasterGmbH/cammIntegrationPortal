-- Remove wrong items from a bug which saved some data for user id 0
delete
FROM  [dbo].[Log_Users]
where id_user = 0
GO


-- Remove old, unused objects
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetCompleteUserInfo]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_GetCompleteUserInfo]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetCompleteAddresses]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_GetCompleteAddresses]
GO


------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
---------------------------- UPDATE OF VARCHARs TO NVARCHARs -----------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------


if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UpdateUserDetails]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Public_UpdateUserDetails]
GO

----------------------------------------------------
-- dbo.Public_UpdateUserDetails
----------------------------------------------------
CREATE PROCEDURE dbo.Public_UpdateUserDetails 
(
	@Username nvarchar(20),
	@Passcode varchar(4096),
	@ServerIP nvarchar(32),
	@RemoteIP nvarchar(32),
	@WebApplication nvarchar(1024),
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
WITH ENCRYPTION
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
DECLARE @bufferLastLoginRemoteIP nvarchar(32)
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
-- dbo.AdminPrivate_UpdateStatusLoginDisabled
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_UpdateStatusLoginDisabled 
(
	@Username nvarchar(20),
	@boolStatus bit
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
		UPDATE dbo.Benutzer SET LoginDisabled = @boolStatus, ModifiedOn = GetDate() WHERE LoginName = @Username
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO

----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_UpdateUserPW 
(
	@Username nvarchar(20),
	@NewPasscode varchar(4096)
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
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 5, 'Admin has modified password')
	END
Else
	-- Rückgabewert
	SELECT Result = 0


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


----------------------------------------------------
-- dbo.Public_GetCompleteName
----------------------------------------------------
ALTER Procedure Public_GetCompleteName
(
	@Username nvarchar(20)
)
WITH ENCRYPTION
As
DECLARE @Vorname nvarchar(30)
DECLARE @Nachname nvarchar(30)
DECLARE @Namenszusatz nvarchar(20)
SET @Vorname = (SELECT Vorname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Nachname = (SELECT Nachname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Namenszusatz = (SELECT Namenszusatz FROM dbo.Benutzer WHERE Loginname = @Username)
-- Namenszusatz könnte Null sein
If substring(@Namenszusatz,1,20) <> '' --Is Not Null
	SET @Namenszusatz = ' ' + @Namenszusatz
Else
	SET @Namenszusatz = ''
SELECT Result = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	/* set nocount on */
	return 
GO

----------------------------------------------------
-- dbo.Public_GetUserID
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserID
(
	@Username nvarchar(20)
)
WITH ENCRYPTION
As
declare @UserID int

set nocount on
SELECT TOP 1 @UserID = ID FROM dbo.Benutzer WHERE Loginname = @Username
set nocount off

SELECT Result = @UserID

Return @UserID
GO


----------------------------------------------------
-- dbo.Public_GetNavPointsOfUser
----------------------------------------------------
ALTER Procedure dbo.Public_GetNavPointsOfUser
(
	@UserID int,
	@ServerIP nvarchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0,
	@SearchForAlternativeLanguages bit = 1
)
WITH ENCRYPTION
As
DECLARE @IsSecurityAdmin bit
DECLARE @AllowedLocation int
DECLARE @buffer int
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @AlternativeLanguage int

SET NoCount ON

-- for example: LanguageID = 512 --> also search for the alternative language with ID 2
If @SearchForAlternativeLanguages = 1
	SELECT @AlternativeLanguage = AlternativeLanguage
	FROM System_Languages
	WHERE ID = @LanguageID
Else
	SELECT @AlternativeLanguage = @LanguageID
If @AlternativeLanguage IS NULL
	SELECT @AlternativeLanguage = @LanguageID

SET @buffer = (SELECT COUNT(ID_Group) FROM dbo.view_Memberships WHERE (ID_Group = 6) AND (ID_User = @UserID))
If @buffer = 0 
	SET @IsSecurityAdmin = 0
Else
	SET @IsSecurityAdmin = 1

-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Abbruch
		Return
	END

-- Application des Internet-Server
SELECT   @AllowedLocation = dbo.System_Servers.ServerGroup, @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @AllowedLocation Is Null 
	Return

--ResetIsNewUpdatedStatusOn
UPDATE dbo.Applications_CurrentAndInactiveOnes SET IsNew = 0, IsUpdated = 0, ResetIsNewUpdatedStatusOn = Null WHERE (ResetIsNewUpdatedStatusOn < GETDATE())

-- Recordset zurückgeben	
If (@IsSecurityAdmin = 0)	-- True would be = 1

	BEGIN

		SELECT distinct Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
		into #NavUpdatedItems_Filtered
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_User = @UserID) OR (dbo.Memberships.ID_User = @UserID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
				AND (IsUpdated <> 0 OR IsNew <> 0)

		SET NoCount OFF

		IF @AnonymousAccess = 0
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_User = @UserID) OR (dbo.Memberships.ID_User = @UserID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage)  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title
		Else
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1)
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered
	END

Else 
	BEGIN

		SELECT distinct Level1Title
		into #NavUpdatedItems_Level1Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level2Title
		into #NavUpdatedItems_Level2Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level3Title
		into #NavUpdatedItems_Level3Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level4Title
		into #NavUpdatedItems_Level4Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level5Title
		into #NavUpdatedItems_Level5Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))


		SELECT distinct Level6Title
		into #NavUpdatedItems_Level6Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		IF @AnonymousAccess = 0
			SET @AnonymousGroupID = 0 -- ungültige GroupID, damit das Ergebnis später nicht verfälscht wird

		SET NoCount OFF

		SELECT DISTINCT dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
			dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
			dbo.Applications.NavURL, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, dbo.Applications.NavToolTipText, dbo.Applications.Sort, Level1IsUpdated = Case When  #NavUpdatedItems_Level1Title.Level1Title Is Not Null Then -1 Else 0 End, Level2IsUpdated = Case When  #NavUpdatedItems_Level2Title.Level2Title Is Not Null Then -1 Else 0 End, 
			Level3IsUpdated = Case When  #NavUpdatedItems_Level3Title.Level3Title Is Not Null Then -1 Else 0 End, Level4IsUpdated = Case When  #NavUpdatedItems_Level4Title.Level4Title Is Not Null Then -1 Else 0 End, Level5IsUpdated = Case When  #NavUpdatedItems_Level5Title.Level5Title Is Not Null Then -1 Else 0 End, Level6IsUpdated = Case When  #NavUpdatedItems_Level6Title.Level6Title Is Not Null Then -1 Else 0 End, 
			dbo.Applications.AppDisabled, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL
			, Case When dbo.Applications.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End  As Level3TitleIsPresent, Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent,
			Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
		FROM ((((((dbo.Applications
			 left join #NavUpdatedItems_Level1Title on dbo.Applications.Level1Title = #NavUpdatedItems_Level1Title.Level1Title
			) left join #NavUpdatedItems_Level2Title on dbo.Applications.Level2Title = #NavUpdatedItems_Level2Title.Level2Title
			) left join #NavUpdatedItems_Level3Title on dbo.Applications.Level3Title = #NavUpdatedItems_Level3Title.Level3Title
			) left join #NavUpdatedItems_Level4Title on dbo.Applications.Level4Title = #NavUpdatedItems_Level4Title.Level4Title
			) left join #NavUpdatedItems_Level5Title on dbo.Applications.Level5Title = #NavUpdatedItems_Level5Title.Level5Title
			) left join #NavUpdatedItems_Level6Title on dbo.Applications.Level6Title = #NavUpdatedItems_Level6Title.Level6Title
			)  LEFT JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
		WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.Applications.LanguageID in (@LanguageID, @AlternativeLanguage) And dbo.Applications.Title <> 'System - Login'
		ORDER BY dbo.Applications.Sort, Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End, dbo.Applications.Level1Title, Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End, dbo.Applications.Level2Title, Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End, dbo.Applications.Level3Title, Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End, dbo.Applications.Level4Title, Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End, dbo.Applications.Level5Title, dbo.Applications.Level6Title

		DROP TABLE #NavUpdatedItems_Level1Title
		DROP TABLE #NavUpdatedItems_Level2Title
		DROP TABLE #NavUpdatedItems_Level3Title
		DROP TABLE #NavUpdatedItems_Level4Title
		DROP TABLE #NavUpdatedItems_Level5Title
		DROP TABLE #NavUpdatedItems_Level6Title
	END