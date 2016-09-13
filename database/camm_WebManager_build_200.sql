IF NOT EXISTS (SELECT ID FROM dbo.Gruppen WHERE ID = -5)
BEGIN
	SET IDENTITY_INSERT dbo.Gruppen ON;
	INSERT INTO Gruppen (ID, Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy) 
	VALUES(-5, 'Data Protection Coordinators', 'System group: manages data protection related matters', GETDATE(), 1, 1, GETDATE(), 1);
	SET IDENTITY_INSERT dbo.Gruppen OFF;
END