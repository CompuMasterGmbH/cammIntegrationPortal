---------------------------------------
-- add new column "ReviewedAndClosed" to table log
---------------------------------------
if not exists (select * from dbo.syscolumns where id = object_id('dbo.Log') and name = 'ReviewedAndClosed') 
ALTER TABLE dbo.[Log]
	ADD ReviewedAndClosed bit
GO
---------------------------------------
-- Update SP "Redirects_LogAndGetURL" 
---------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Redirects_LogAndGetURL]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Redirects_LogAndGetURL]
GO


CREATE PROCEDURE dbo.Redirects_LogAndGetURL
	(
		@IDRedirector int
	)
WITH ENCRYPTION
AS 

SET NOCOUNT ON

-- Log
INSERT INTO dbo.Redirects_Log
                      (ID_Redirector)
VALUES     (@IDRedirector)


-- Increase the ticker 
UPDATE dbo.Redirects_ToAddr
SET NumberOfRedirections = NumberOfRedirections + 1
WHERE ID = @IDRedirector


SET NOCOUNT OFF

-- Get URL
SELECT RedirectTo
FROM dbo.Redirects_ToAddr
WHERE ID = @IDRedirector

GO
-----------------------------------------------------------
--Insert data into security objects and authorizations: Redirection Admin Area
-----------------------------------------------------------
DECLARE @ModifiedBy int
SELECT @ModifiedBy = -43
DECLARE @AppID_Redirections int, @AdminServerID int
SELECT @AppID_Redirections = ID FROM dbo.Applications_CurrentAndInactiveOnes WHERE Title = 'System - Administration - Redirections'
IF @AppID_Redirections IS NULL
BEGIN
	-- Retrieve one server ID
	SELECT TOP 1 @AdminServerID = System_Servers.ID
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- 1st security object
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	VALUES('System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,@AdminServerID,1,1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
	SELECT @AppID_Redirections = @@IDENTITY
	-- Rest of new security objects - same languages (English)
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Redirections',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,System_Servers.ID,1,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	WHERE System_Servers.ID <> @AdminServerID
	GROUP BY System_Servers.ID
	-- Rest of new security objects - other languages
	INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
	SELECT 'System - Administration - Redirections',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Weiterleitungen',NULL,NULL,NULL,'[ADMINURL]redir/index.aspx',NULL,NULL,0,0,System_Servers.ID,2,1,getdate(),@ModifiedBy,0,@AppID_Redirections ,1000000,NULL,0,NULL,NULL,NULL,1,2
	FROM System_Servers INNER JOIN System_ServerGroups ON System_Servers.ID = System_ServerGroups.UserAdminServer
	GROUP BY System_Servers.ID
	-- Authorizations
	INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Redirections ,6,getdate(),@ModifiedBy)
END
GO