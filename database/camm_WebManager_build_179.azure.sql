IF EXISTS (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationRights_CumulatedPerUserAndServerGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE dbo.ApplicationRights_CumulatedPerUserAndServerGroup
GO
CREATE PROCEDURE dbo.ApplicationRights_CumulatedPerUserAndServerGroup
(
	@UserID int,
	@ServerGroupID int,
	@AuthorizedAppsCursor AS CURSOR VARYING OUTPUT 
)
WITH ENCRYPTION
AS
 
/*
 * @UserID NULL means: search for anonymous auths only
 */

DECLARE @PublicGroupID int, @AnonymousGroupID int
SELECT @PublicGroupID = id_group_public, @AnonymousGroupID = id_group_anonymous
FROM system_servergroups where id = @ServerGroupID
IF @AnonymousGroupID IS NULL SET @AnonymousGroupID = 58

SET @AuthorizedAppsCursor = CURSOR FOR 
	    
SELECT ID_Application 
FROM
(
    -- Direct authorizations
    SELECT     dbo.ApplicationsRightsByUser.ID_Application
    FROM dbo.ApplicationsRightsByUser
    WHERE     dbo.ApplicationsRightsByUser.ID_GroupOrPerson = @UserID
    UNION ALL
    -- Indirect authorizations caused by application inheritage
    SELECT     dbo.Applications.ID
    FROM dbo.ApplicationsRightsByUser 
    	INNER JOIN dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.AuthsAsAppID
    WHERE     dbo.ApplicationsRightsByUser.ID_GroupOrPerson = @UserID
    UNION ALL
    -- Indirect authorizations caused by memberships as well as public and anonymous user
    SELECT     dbo.ApplicationsRightsByGroup.ID_Application
    FROM dbo.ApplicationsRightsByGroup LEFT OUTER JOIN
        (
          SELECT     ID_Group, ID_User
          FROM         dbo.Memberships
          WHERE ID_User = @UserID
          UNION ALL
          SELECT     @AnonymousGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @AnonymousGroupID IS NOT NULL
          UNION ALL
          SELECT     @PublicGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @PublicGroupID IS NOT NULL
          UNION ALL
          SELECT     @AnonymousGroupID, NULL
         ) AS Memberships_CummulatedWithAnonymousAndPublic ON 
         dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = Memberships_CummulatedWithAnonymousAndPublic.ID_Group
    WHERE CASE 
        WHEN @UserID IS NOT NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_User = @UserID 
			THEN 1 -- Normal user membership search
        WHEN @UserID IS NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_Group = @AnonymousGroupID 
			THEN 1 -- No user search but search for authorizations of ANONYMOUS group-user
        ELSE 0
        END = 1
    UNION ALL
    -- Indirect authorizations caused by memberships as well as public and anonymous user
    SELECT     dbo.Applications.ID
    FROM dbo.ApplicationsRightsByGroup 
    	INNER JOIN dbo.Applications ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications.AuthsAsAppID
        LEFT OUTER JOIN
        (
          SELECT     ID_Group, ID_User
          FROM         dbo.Memberships
          WHERE ID_User = @UserID
          UNION ALL
          SELECT     @AnonymousGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @AnonymousGroupID IS NOT NULL
          UNION ALL
          SELECT     @PublicGroupID, id
          FROM         dbo.benutzer
          WHERE ID = @UserID AND @PublicGroupID IS NOT NULL
          UNION ALL
          SELECT     @AnonymousGroupID, NULL
         ) AS Memberships_CummulatedWithAnonymousAndPublic ON 
         dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = Memberships_CummulatedWithAnonymousAndPublic.ID_Group
    WHERE CASE 
        WHEN @UserID IS NOT NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_User = @UserID 
			THEN 1 -- Normal user membership search
        WHEN @UserID IS NULL AND Memberships_CummulatedWithAnonymousAndPublic.ID_Group = @AnonymousGroupID 
			THEN 1 -- No user search but search for authorizations of ANONYMOUS group-user
        ELSE 0
        END = 1
    UNION ALL
    -- Add authorizations caused by supervisor membership
    SELECT     Applications.ID AS ID_Application
    FROM         dbo.Applications CROSS JOIN
                          dbo.Memberships
    WHERE     dbo.Memberships.ID_Group = 6 AND dbo.Memberships.ID_User = @UserID
) AS BaseTable
GROUP BY ID_Application

OPEN @AuthorizedAppsCursor

GO
