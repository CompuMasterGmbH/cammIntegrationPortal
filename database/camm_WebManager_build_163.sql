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
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @ReleasedByUserID <> -33 AND @CurUserID Is Null --if not UserByCode and no other valid user ID
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
		SELECT Result = 5
		Return 5
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

GO
