/******************************************************
  Module: Redirects
******************************************************/

/******************************************************
  Begin
******************************************************/

----------------------------------------------------
-- [dbo].[Redirects_Log]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Redirects_Log]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[Redirects_Log]
GO

CREATE TABLE [dbo].[Redirects_Log] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[ID_Redirector] [int] NOT NULL ,
	[AccessDateTime] [datetime] NOT NULL ,
	CONSTRAINT [PK_Redirects_Log] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)
)  
GO

ALTER TABLE [dbo].[Redirects_Log] WITH NOCHECK ADD 
	CONSTRAINT [DF_Redirects_Log_AccessDateTime] DEFAULT (getdate()) FOR [AccessDateTime]
GO

----------------------------------------------------
-- [dbo].[Redirects_ToAddr]
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Redirects_ToAddr]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[Redirects_ToAddr]
GO

CREATE TABLE [dbo].[Redirects_ToAddr] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[Description] [nvarchar] (255) NOT NULL ,
	[RedirectTo] [nvarchar] (255) NOT NULL ,
	[NumberOfRedirections] [int] NOT NULL ,
	CONSTRAINT [PK_Redirects_ToAddr] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)
) 
GO

ALTER TABLE [dbo].[Redirects_ToAddr] WITH NOCHECK ADD 
	CONSTRAINT [DF_Redirects_ToAddr_NumberOfRedirections] DEFAULT (0) FOR [NumberOfRedirections]
GO

----------------------------------------------------
-- [dbo].[Redirects_LogAndGetURL]
----------------------------------------------------

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Redirects_LogAndGetURL]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
drop procedure [dbo].[Redirects_LogAndGetURL]
GO

CREATE PROCEDURE dbo.Redirects_LogAndGetURL
	(
		@IDRedirector nvarchar(50)
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

/******************************************************
  End
******************************************************/
