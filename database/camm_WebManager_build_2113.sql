--fix missing effective memberships
IF OBJECT_ID ('dbo.U_Memberships_DenyRules', 'TR') IS NOT NULL
   DROP TRIGGER dbo.U_Memberships_DenyRules;
GO
update Memberships
set id_user=id_user