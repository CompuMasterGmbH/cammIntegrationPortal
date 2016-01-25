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
