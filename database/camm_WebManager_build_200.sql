IF NOT EXISTS (SELECT ID FROM Gruppen WHERE ID = -5)
BEGIN
INSERT INTO Gruppen (ID, Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy) 
VALUES(-5, 'Data Protection Coordinators', 'System group: manages data protection related matters', GETDATE(), 1, 1, GETDATE(), 1) 
END