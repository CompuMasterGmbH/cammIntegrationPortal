ALTER view [dbo].[view_Memberships_CummulatedWithAnonymous]
AS
------ OBSOLETE: view doesn't reflect the public (registered) user membership ------
SELECT     ID_Group, ID_User
FROM         dbo.Memberships
UNION
SELECT     58, id
FROM         dbo.benutzer

GO
ALTER VIEW [dbo].[view_ApplicationRights_CommulatedPerUser]
AS
------ OBSOLETE: view is very slow and doesn't reflect the public (registered) user membership ------
SELECT     dbo.view_ApplicationRights.Title, dbo.view_ApplicationRights.ID_Application, ISNULL(dbo.view_ApplicationRights.ID_User, 
                      dbo.view_Memberships_CummulatedWithAnonymous.ID_User) AS ID_User
FROM         dbo.view_ApplicationRights LEFT OUTER JOIN
                      dbo.view_Memberships_CummulatedWithAnonymous ON 
                      dbo.view_ApplicationRights.ID_Group = dbo.view_Memberships_CummulatedWithAnonymous.ID_Group
WHERE     (ISNULL(dbo.view_ApplicationRights.ID_User, dbo.view_Memberships_CummulatedWithAnonymous.ID_User) IS NOT NULL)
UNION
SELECT     Applications.Title, Applications.ID AS ID_Application, dbo.Memberships.ID_User
FROM         dbo.Applications CROSS JOIN
                      dbo.Memberships
WHERE     dbo.Memberships.ID_Group = 6

GO