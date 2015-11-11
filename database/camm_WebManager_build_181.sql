IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE type = 'TR' and id = OBJECT_ID(N'[dbo].[IUD_Apps2SecObjAndNavItems]'))
DROP TRIGGER [dbo].[IUD_Apps2SecObjAndNavItems]
GO
CREATE TRIGGER [dbo].[IUD_Apps2SecObjAndNavItems] 
   ON  [dbo].[Applications_CurrentAndInactiveOnes] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE type = 'TR' and id = OBJECT_ID(N'[dbo].[IUD_AuthsUsers2SecObjAndNavItems]'))
DROP TRIGGER [dbo].[IUD_AuthsUsers2SecObjAndNavItems]
GO
CREATE TRIGGER [dbo].[IUD_AuthsUsers2SecObjAndNavItems] 
   ON  [dbo].[ApplicationsRightsByUser] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE type = 'TR' and id = OBJECT_ID(N'[dbo].[IUD_AuthsGroups2SecObjAndNavItems]'))
DROP TRIGGER [dbo].[IUD_AuthsGroups2SecObjAndNavItems]
GO
CREATE TRIGGER [dbo].[IUD_AuthsGroups2SecObjAndNavItems] 
   ON  [dbo].[ApplicationsRightsByGroup] 
   FOR INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
END
GO