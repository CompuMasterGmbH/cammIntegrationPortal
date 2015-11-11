INSERT INTO [dbo].[Log_Users] ([ID_User], [Type], [Value], [ModifiedOn], [ModifiedBy])
SELECT ID, 'InitAuthorizationsDone', '1', getdate(), -44
FROM Benutzer 
WHERE ID NOT IN 
(
	SELECT [ID_User] 
	FROM [dbo].[Log_Users] 
	where [Type] = 'InitAuthorizationsDone' AND [Value] = '1'
)
AND ID IN
(
	Select id_user 
	From dbo.Memberships
	UNION
	SELECT [ID_GroupOrPerson]
	FROM [dbo].[ApplicationsRightsByUser]
)
