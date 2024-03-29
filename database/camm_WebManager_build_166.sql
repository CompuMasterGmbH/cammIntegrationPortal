----------------------------------------------------
-- dbo.AdminPrivate_CreateMemberships
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMemberships 
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
Else
	SELECT @CurUserID = @ReleasedByUserID
SELECT @MemberShipID = ID FROM dbo.Memberships WHERE ID_Group = @GroupID And ID_User = @UserID
If @MemberShipID Is Null
	BEGIN
		-- Is releasing user a valid user ID?
		If @CurUserID Is Not Null
			-- Validation successfull, membership will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Record update
				INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedBy) VALUES (@GroupID, @UserID, @ReleasedByUserID)
			END
		Else
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	SELECT Result = -1
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByGroup 
	@ReleasedByUserID int,
	@AppID int,
	@GroupID int

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application ID
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END


SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 1
	RETURN 1
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Password validation and update
If @CurUserID Is Not Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application, ID_GroupOrPerson, ReleasedBy) VALUES (@AppID, @GroupID, @ReleasedByUserID)
		EXEC Int_LogAuthChanges @ReleasedByUserID, @GroupID, @AppID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
	END
Else
	-- Rückgabewert
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 0
	END

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByUser 
	@ReleasedByUserID int,
	@AppID int,
	@UserID int,
	@IsDevelopmentTeamMember bit

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @SAPFlagName varchar(50)
DECLARE @SAPFlagValue nvarchar(255)
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application ID
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
Else
	SELECT @CurUserID = @ReleasedByUserID
If @CurUserID Is Null
	-- Rückgabewert
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @UserID)
If @CurUserID Is Null
	-- Rückgabewert
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END

SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SELECT @SAPFlagName = substring(navurl, charindex('|2|&', NavUrl)+4, charindex('|3|', NavUrl) - (charindex('|2|&', NavUrl)+4)) FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
	SELECT @SAPFlagValue = [Value] FROM Log_Users WHERE ID_User = @CurUserID AND Type = @SAPFlagName
	If @SAPFlagValue Is Null
		BEGIN
		SET NOCOUNT OFF
		SELECT Result = 2, @SAPFlagName as SAPFlagName
		RETURN 2
		END
	END

-- User validation and update
If @CurUserID Is Not Null
	-- Validation successfull, authorization will be saved now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByUser 
			(ID_Application, ID_GroupOrPerson, ReleasedBy, DevelopmentTeamMember) 
		VALUES 
			(@AppID, @UserID, @ReleasedByUserID, @IsDevelopmentTeamMember)
		EXEC Int_LogAuthChanges @UserID, Null, @AppID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		Return -1
	END

