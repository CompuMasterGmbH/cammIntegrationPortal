----------------------------------------------------
-- dbo.Int_LogAuthChanges
----------------------------------------------------
ALTER PROCEDURE dbo.Int_LogAuthChanges
(
@UserID int = Null,
@GroupID int = Null,
@AppID int,
@ReleasedByUserID int = Null
)
WITH ENCRYPTION
AS 

If @GroupID Is Not Null
	begin
		-- log indirect changes on users
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		select id_user, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -7, Null
		from view_Memberships_CummulatedWithAnonymous
		where id_group = @GroupID
		-- log group auth change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (IsNull(@UserID, 0), GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -8, cast(@GroupID as nvarchar(20)) + N' by user ' + cast(@ReleasedByUserID as nvarchar(20)))
	end
Else
	If @UserID Is Not Null
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -6, N'by user ' + cast(@ReleasedByUserID as nvarchar(20)))
GO

----------------------------------------------------
-- dbo.AdminPrivate_CreateMemberships
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMemberships 
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForMemberships 'UpdateRelations', @ReleasedByUserID, @GroupID
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
				-- log group membership change
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', NULL, -11, cast(@GroupID as nvarchar(50)))
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
WITH ENCRYPTION
AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application
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
	EXEC @CurUserID = dbo.IsAdministratorForAuthorizations 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Password validation and update
If @CurUserID Is Not Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application, ID_GroupOrPerson, ReleasedBy) VALUES (@AppID, @GroupID, @ReleasedByUserID)
		EXEC Int_LogAuthChanges NULL, @GroupID, @AppID, @ReleasedByUserID
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
WITH ENCRYPTION
AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @SAPFlagName varchar(50)
DECLARE @SAPFlagValue nvarchar(255)
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForAuthorizations 'UpdateRelations', @ReleasedByUserID, @AppID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Check if the given user really exists
SELECT @UserID = (select ID from dbo.Benutzer where id = @UserID)
If @UserID Is Null
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
	SELECT @SAPFlagValue = [Value] FROM Log_Users WHERE ID_User = @UserID AND Type = @SAPFlagName
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
		EXEC Int_LogAuthChanges @UserID, Null, @AppID, @ReleasedByUserID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		Return -1
	END
ELSE
	-- Releasing user is not authorized to administer
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END

GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByGroup]
(
	@AuthID int,
	@ReleasedByUserID int
)
WITH ENCRYPTION
AS 

declare @groupID int
declare @AppID int
select top 1 @groupid = id_grouporperson, @appid = id_application from dbo.ApplicationsRightsByGroup where id = @AuthID

EXEC Int_LogAuthChanges NULL, @GroupID, @AppID, @ReleasedByUserID 
DELETE FROM dbo.ApplicationsRightsByGroup WHERE     (ID_GroupOrPerson IS NOT NULL) AND ID=@AuthID

GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByUser]
(
	@AuthID int,
	@ReleasedByUserID int = NULL
)
WITH ENCRYPTION
AS
declare @UserID int
declare @AppID int
select top 1 @userid = id_grouporperson, @appid = id_application from dbo.ApplicationsRightsByUser where id = @AuthID

EXEC Int_LogAuthChanges @UserID, Null, @AppID, @ReleasedByUserID 
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson Is Not Null And ID=@AuthID
GO


IF  EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[AdminPrivate_DeleteMemberships]
GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteMemberships
----------------------------------------------------
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteMemberships]
(
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int
)
WITH ENCRYPTION
AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @MemberShipID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
If @ReleasedByUserID <> -33 And @ReleasedByUserID <> -43  And @ReleasedByUserID <> -1
	EXEC @CurUserID = dbo.IsAdministratorForMemberships 'UpdateRelations', @ReleasedByUserID, @GroupID
Else
	SELECT @CurUserID = @ReleasedByUserID

-- Is releasing user a valid user ID?
If @CurUserID Is Not Null
	-- Validation successfull, membership will be updated now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Record update
		DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID
		-- log group membership change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', NULL, -12, cast(@GroupID as nvarchar(50)))
	END
Else
	-- Rückgabewert
	SELECT Result = 0
GO