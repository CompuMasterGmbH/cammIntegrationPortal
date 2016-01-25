<%IGNORE_ERRORS%>
-- Add missing DevelopmentTeamMember column for consequency between auths for users and groups
ALTER TABLE [dbo].[ApplicationsRightsByGroup]
ADD DevelopmentTeamMember bit NOT NULL DEFAULT (0)
GO

<%IGNORE_ERRORS%>
-- Add column IsDenyRule for allowing GRANT and DENY priviledges
ALTER TABLE [dbo].[ApplicationsRightsByGroup]
ADD IsDenyRule bit NOT NULL DEFAULT (0)
GO

<%IGNORE_ERRORS%>
-- Add column IsDenyRule for allowing GRANT and DENY priviledges
ALTER TABLE [dbo].[ApplicationsRightsByUser]
ADD IsDenyRule bit NOT NULL DEFAULT (0)
GO

<%IGNORE_ERRORS%>
-- Add column IsDenyRule for allowing GRANT and DENY priviledges
ALTER TABLE [dbo].[Memberships]
ADD IsDenyRule bit NOT NULL DEFAULT (0)
GO

<%IGNORE_ERRORS%>
-- Add column ServerGroupID for later feature for SplitAppsIntoNav+SecObj milestone
ALTER TABLE [dbo].[Log]
ADD ServerGroupID int NULL
