-- Clean up all SubSecurityAdjustments with missing Applications
DELETE
  FROM [dbo].[System_SubSecurityAdjustments]
WHERE TableName = 'Applications' AND
	TablePrimaryIDValue <> 0 AND
	TablePrimaryIDValue NOT IN (SELECT [ID] FROM [dbo].[Applications])
-- Clean up all SubSecurityAdjustments with missing Applications
DELETE
  FROM [dbo].[System_SubSecurityAdjustments]
WHERE TableName = 'Groups' AND
	TablePrimaryIDValue <> 0 AND
	TablePrimaryIDValue NOT IN (SELECT [ID] FROM [dbo].[Gruppen])
-- Clean up all SubSecurityAdjustments with missing security admins/operators/related contacts/supervisors
DELETE
  FROM [dbo].[System_SubSecurityAdjustments]
WHERE UserID NOT IN (SELECT [ID_User] FROM [dbo].[Memberships] WHERE ID_Group IN (-7, 6, 7))



-- DELETE inconsistent data caused by manual table edits: AppRights(Groups) with missing Application
DELETE [dbo].[ApplicationsRightsByGroup]
FROM [dbo].[ApplicationsRightsByGroup]
	LEFT JOIN dbo.Applications ON [dbo].[ApplicationsRightsByGroup].ID_Application = dbo.Applications.ID
WHERE dbo.Applications.ID IS NULL
GO
-- DELETE inconsitent data caused by manual table edits: AppRights(Users) with missing Application
DELETE [dbo].[ApplicationsRightsByUser]
FROM [dbo].[ApplicationsRightsByUser]
	LEFT JOIN dbo.Applications ON [dbo].[ApplicationsRightsByUser].ID_Application = dbo.Applications.ID
WHERE dbo.Applications.ID IS NULL
GO
-- DELETE inconsitent data caused by manual table edits: AppRights(Groups) with missing Group
DELETE [dbo].[ApplicationsRightsByGroup]
FROM [dbo].[ApplicationsRightsByGroup]
	LEFT JOIN dbo.Gruppen ON [dbo].[ApplicationsRightsByGroup].ID_GroupOrPerson = dbo.Gruppen.ID
WHERE dbo.Gruppen.ID IS NULL
GO
-- DELETE inconsitent data caused by manual table edits: AppRights(Users) with missing User
DELETE [dbo].[ApplicationsRightsByUser]
FROM [dbo].[ApplicationsRightsByUser]
	LEFT JOIN dbo.Benutzer ON [dbo].[ApplicationsRightsByUser].ID_GroupOrPerson = dbo.Benutzer.ID
WHERE dbo.Benutzer.ID IS NULL
GO
-- DELETE inconsitent data caused by manual table edits: Memberships with missing Group
DELETE [dbo].[Memberships]
FROM [dbo].[Memberships]
	LEFT JOIN dbo.Gruppen ON [dbo].[Memberships].ID_Group = dbo.Gruppen.ID
WHERE dbo.Gruppen.ID IS NULL
GO
-- DELETE inconsitent data caused by manual table edits: Memberships with missing User
DELETE [dbo].[Memberships]
FROM [dbo].[Memberships]
	LEFT JOIN dbo.Benutzer ON [dbo].[Memberships].ID_User = dbo.Benutzer.ID
WHERE dbo.Benutzer.ID IS NULL
