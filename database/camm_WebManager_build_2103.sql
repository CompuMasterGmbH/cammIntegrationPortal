<%IGNORE_ERRORS%>
-- Add column ServerGroupID for later preparation of feature for SplitAppsIntoNav+SecObj milestone
-- Add column ID_ServerGroup for allowing assignment of  server group scope for an authorization item
ALTER TABLE [dbo].[ApplicationsRightsByGroup]
ADD ID_ServerGroup int NOT NULL DEFAULT (0)
GO

<%IGNORE_ERRORS%>
-- Add column ServerGroupID for later preparation of feature for SplitAppsIntoNav+SecObj milestone
-- Add column ID_ServerGroup for allowing assignment of  server group scope for an authorization item
ALTER TABLE [dbo].[ApplicationsRightsByUser]
ADD ID_ServerGroup int NOT NULL DEFAULT (0)
GO

-----------------------------------------------------------------------
-- ADDED TABLE FOR [MembershipsClones] for later feature development
-----------------------------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[MembershipsClones]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[MembershipsClones]
GO

CREATE TABLE [dbo].[MembershipsClones](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_ClonedGroup] [int] NOT NULL,
	[ReleasedOn] [datetime] NOT NULL DEFAULT (getdate()),
	[ReleasedBy] [int] NOT NULL,
	[IsDenyRule] [bit] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_MembershipsClones] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO
