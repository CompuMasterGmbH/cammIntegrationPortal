-- drop table because of dropped concept
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Apps2SecObj_SyncWarnLog]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Apps2SecObj_SyncWarnLog]
GO

-- add security access everything group (but without supervisor priviledges to do aministration)
IF NOT EXISTS (SELECT ID FROM dbo.Gruppen WHERE ID = -6)
BEGIN
	SET IDENTITY_INSERT dbo.Gruppen ON;
	INSERT INTO Gruppen (ID, Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy) 
	VALUES(-6, 'Security Access Everything', 'System group: allowed to access all applications', GETDATE(), 1, 1, GETDATE(), 1);
	SET IDENTITY_INSERT dbo.Gruppen OFF;
END
GO

-- fix the log items in the history
UPDATE dbo.Log
SET ConflictType = 31
WHERE ConflictType = 1 AND ApplicationID IS NOT NULL
GO

<%IGNORE_ERRORS%>
-- Add column ServerGroupID for later improvements to track additional data for security analysts
ALTER TABLE [dbo].[Log]
ADD [ByUserID] int NULL
GO

<%IGNORE_ERRORS%>
-- Add column for extended data for later improvements to track additional data for technicians
ALTER TABLE [dbo].[Log]
ADD [Data] ntext NULL
GO

<%IGNORE_ERRORS%>
-- Add column ServerGroupID for later feature for SplitAppsIntoNav+SecObj milestone
ALTER TABLE [dbo].[Log]
ADD ServerGroupID int NULL
GO

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
ALTER TABLE [dbo].[ApplicationsRightsByGroup]
ADD IsSupervisorAutoAccessRule bit NOT NULL DEFAULT (0)
GO

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
-- Add column IsSystemRuleOfServerGroupsAndTheirUserAccessLevelsID required to identify anonymous+public group memberships owned by this relation
ALTER TABLE [dbo].[Memberships]
ADD IsSystemRuleOfServerGroupsAndTheirUserAccessLevelsID int NULL
GO

<%IGNORE_ERRORS%>
-- Add column IsSystemRuleOfServerGroupID required to identify anonymous user groups owned by ServerGroup
ALTER TABLE [dbo].[Memberships]
ADD IsSystemRuleOfServerGroupID int NULL
GO

-- Reset previously filled data since it's waste
TRUNCATE TABLE [dbo].SecurityObjects_vs_NavItems

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_CurrentAndInactiveOnes]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].SecurityObjects_CurrentAndInactiveOnes
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_AuthsByUser]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].SecurityObjects_AuthsByUser
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[SecurityObjects_AuthsByGroup]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].SecurityObjects_AuthsByGroup
GO
<%IGNORE_ERRORS%>
-- Add columns for tracking admin users
ALTER TABLE [dbo].SecurityObjects_vs_NavItems
ADD
	[ReleasedOn] [datetime] NOT NULL CONSTRAINT [DF_SecurityObjects_vs_NavItems_ReleasedOn] DEFAULT (getdate()),
	[ReleasedBy] [int] NOT NULL ,
	[ModifiedOn] [datetime] NULL CONSTRAINT [DF_SecurityObjects_vs_NavItems_ModifiedOn] DEFAULT (getdate()),
	[ModifiedBy] [int] NULL
GO

-- App/SecObj Authorizations into 
IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRights_Inheriting]'))
DROP VIEW [dbo].[ApplicationsRights_Inheriting]
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRights_Inheriting]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
drop table [dbo].[ApplicationsRights_Inheriting]
GO
CREATE TABLE [dbo].[ApplicationsRights_Inheriting] (
	[ID] [int] IDENTITY (1, 1) NOT NULL ,
	[ID_Inheriting] [int] NOT NULL ,
	[ID_Source] [int] NOT NULL ,
	[RuleSourceApplicationID] [int] NULL ,
	[ReleasedOn] [datetime] NOT NULL CONSTRAINT [DF_ApplicationsRights_Inheriting_ReleasedOn] DEFAULT (getdate()),
	[ReleasedBy] [int] NOT NULL ,
	[ModifiedOn] [datetime] NULL CONSTRAINT [DF_ApplicationsRights_Inheriting_ModifiedOn] DEFAULT (getdate()),
	[ModifiedBy] [int] NULL
	CONSTRAINT [PK_ApplicationsRights_Inheriting] PRIMARY KEY  CLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO

-- Pre-Staging table for pre-translation of GROUP-AUTH rows with ID_ServerGroup = 0 being multiplied to all available server groups
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup]
GO
CREATE TABLE [dbo].[ApplicationsRightsByGroup_PreStaging1ForRealServerGroup](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[IsServerGroup0Rule] [bit] NOT NULL,
	[IsDenyRule] [bit] NOT NULL,
	[IsDevRule] [bit] NOT NULL,
	[DerivedFromAppRightsID] int NOT NULL
 CONSTRAINT [PK_ApplicationsRightsByGroup_PreStaging1ForRealServerGroup] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) ON [PRIMARY]
GO

-- Pre-Staging table for pre-translation of USER-AUTH rows with ID_ServerGroup = 0 being multiplied to all available server groups
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ApplicationsRightsByUser_PreStaging1ForRealServerGroup]
GO
CREATE TABLE [dbo].[ApplicationsRightsByUser_PreStaging1ForRealServerGroup](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[IsServerGroup0Rule] [bit] NOT NULL,
	[IsDenyRule] [bit] NOT NULL,
	[IsDevRule] [bit] NOT NULL,
	[DerivedFromAppRightsID] int NOT NULL
 CONSTRAINT [PK_ApplicationsRightsByUser_PreStaging1ForRealServerGroup] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) ON [PRIMARY]
GO

-- Pre-Staging table for pre-translation of GROUP-AUTH rows for auth inheritions
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByGroup_PreStaging2Inheritions]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ApplicationsRightsByGroup_PreStaging2Inheritions]
GO
CREATE TABLE [dbo].[ApplicationsRightsByGroup_PreStaging2Inheritions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[IsDenyRule] [bit] NOT NULL,
	[IsDevRule] [bit] NOT NULL,
	[DerivedFromAppRightsID] int NOT NULL,
	[DerivedFromInheritedSecurityObjectRelationID] int NULL,
	[DerivedFromPreStaging1ID] bigint NOT NULL
 CONSTRAINT [PK_ApplicationsRightsByGroup_PreStaging2Inheritions] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) ON [PRIMARY]
GO

-- Pre-Staging table for pre-translation of USER-AUTH rows for auth inheritions
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByUser_PreStaging2Inheritions]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ApplicationsRightsByUser_PreStaging2Inheritions]
GO
CREATE TABLE [dbo].[ApplicationsRightsByUser_PreStaging2Inheritions](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[IsDenyRule] [bit] NOT NULL,
	[IsDevRule] [bit] NOT NULL,
	[DerivedFromAppRightsID] int NOT NULL,
	[DerivedFromInheritedSecurityObjectRelationID] int NULL,
	[DerivedFromPreStaging1ID] bigint NOT NULL
 CONSTRAINT [PK_ApplicationsRightsByUser_PreStaging2Inheritions] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) ON [PRIMARY]
GO

-- Pre-Staging table for pre-translation of USER-AUTH rows for group auth resolvings
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByUser_PreStaging3GroupsResolved]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ApplicationsRightsByUser_PreStaging3GroupsResolved]
GO
CREATE TABLE [dbo].[ApplicationsRightsByUser_PreStaging3GroupsResolved](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[IsDevRule] [bit] NOT NULL,
	[IsDenyRule] [bit] NOT NULL,
	[DerivedFromAppRightsID] bigint NOT NULL,
	[DerivedFromPreStaging2_Groups_RealServerGroupID] int NULL,
	[DerivedFromPreStaging2_Groups_ID] bigint NULL,
	[DerivedFromPreStaging2_Users_ID] bigint NULL
 CONSTRAINT [PK_ApplicationsRightsByUser_PreStaging3GroupsResolved] PRIMARY KEY CLUSTERED 
 (
	[ID] ASC
 )
) ON [PRIMARY]
GO

-- Pre-Staging table for pre-translation of GROUP-AUTH rows for allow/deny rules
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules]
GO
CREATE TABLE [dbo].[ApplicationsRightsByUser_PreStaging4AllowDenyRules](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_SecurityObject] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_ServerGroup] [int] NOT NULL,
	[IsDevRule] [bit] NOT NULL,
	[DerivedFromAppRightsID] int NOT NULL,
	[DerivedFromPreStaging3ID] bigint NOT NULL,
	[PK_UniqueRowData] varchar(250) NOT NULL,
	[UniqueAuthObject] varchar(120) NOT NULL
 CONSTRAINT [PK_ApplicationsRightsByUser_PreStaging4AllowDenyRules] PRIMARY KEY CLUSTERED 
 (
	[PK_UniqueRowData] ASC
 )
) ON [PRIMARY]
GO

--Pre-Staging tables for memberships
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Memberships_DenyRules]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Memberships_DenyRules]
GO
CREATE TABLE [dbo].[Memberships_DenyRules](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_Memberships] [int] NOT NULL,
 CONSTRAINT [PK_Memberships_DenyRules] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
) ON [PRIMARY]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Memberships_EffectiveRules]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Memberships_EffectiveRules]
GO
CREATE TABLE [dbo].[Memberships_EffectiveRules](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_Memberships_ByAllowRule] [int] NOT NULL,
 CONSTRAINT [PK_Memberships_EffectiveRules] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
) ON [PRIMARY]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Memberships_EffectiveRulesWithClones1stGrade]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Memberships_EffectiveRulesWithClones1stGrade]
GO
CREATE TABLE [dbo].[Memberships_EffectiveRulesWithClones1stGrade](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_Memberships_ByAllowRule] [int] NOT NULL,
	[ID_Memberships_EffectiveRules] [int] NOT NULL,
	[ID_MembershipsClones] [int] NULL,
 CONSTRAINT [PK_Memberships_EffectiveRulesWithClones1stGrade] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
) ON [PRIMARY]
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Memberships_EffectiveRulesWithClonesNthGrade]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1)
DROP TABLE [dbo].[Memberships_EffectiveRulesWithClonesNthGrade]
GO
CREATE TABLE [dbo].[Memberships_EffectiveRulesWithClonesNthGrade](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Group] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_Memberships_ByAllowRule] [int] NOT NULL,
	[ID_Memberships_EffectiveRulesWithClones1stGrade] [int] NOT NULL,
	[ID_MembershipsClones] [int] NULL,
 CONSTRAINT [PK_Memberships_EffectiveRulesWithClonesNthGrade] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)
) ON [PRIMARY]
GO

-- DROP MODULE Log analysis 
UPDATE dbo.Applications
SET AppDeleted = 1
WHERE SystemApp = 1 And SystemAppType = 3 
	AND Title = 'System - User Administration - LogAnalysis'

