/******************************************************
  Tables  Begin
******************************************************/

print 'TABLECREATION - BEGIN'


----------------------------------------------------
-- dbo.Applications_CurrentAndInactiveOnes
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Applications_CurrentAndInactiveOnes]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Applications_CurrentAndInactiveOnes]
GO
CREATE TABLE [dbo].[Applications_CurrentAndInactiveOnes]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
Title                               VARCHAR(255),
TitleAdminArea                      nvarchar(255),
ReleasedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ReleasedBy                          INT NOT NULL,
Level1Title                         NVARCHAR(512),
Level2Title                         NVARCHAR(512),
Level3Title                         NVARCHAR(512),
Level4Title                         NVARCHAR(512),
Level5Title                         NVARCHAR(512),
Level6Title                         NVARCHAR(512),
NavURL                              VARCHAR(512),
NavFrame                            VARCHAR(50),
NavTooltipText                      NVARCHAR(1024),
IsNew                               BIT DEFAULT (0) NOT NULL,
IsUpdated                           BIT DEFAULT (0) NOT NULL,
LocationID                          INT NOT NULL,
LanguageID                          INT NOT NULL,
SystemApp                           BIT DEFAULT (0) NOT NULL,
ModifiedOn                          DATETIME DEFAULT (getdate()),
ModifiedBy                          INT,
AppDisabled                         BIT DEFAULT (1) NOT NULL,
AuthsAsAppID                        INT,
Sort                                INT DEFAULT (1000000),
ResetIsNewUpdatedStatusOn           DATETIME,
AppDeleted                          BIT DEFAULT (0) NOT NULL,
OnMouseOver                         NVARCHAR(512),
OnMouseOut                          NVARCHAR(512),
OnClick                             NVARCHAR(512),
AddLanguageID2URL                   BIT DEFAULT (1) NOT NULL,
Level1TitleIsHTMLCoded              BIT DEFAULT (0) NOT NULL,
Level2TitleIsHTMLCoded              BIT DEFAULT (0) NOT NULL,
Level3TitleIsHTMLCoded              BIT DEFAULT (0) NOT NULL,
Level4TitleIsHTMLCoded              BIT DEFAULT (0) NOT NULL,
Level5TitleIsHTMLCoded              BIT DEFAULT (0) NOT NULL,
Level6TitleIsHTMLCoded              BIT DEFAULT (0) NOT NULL,
SystemAppType                       INT,
Remarks                             NVARCHAR(512),
CONSTRAINT PK_Applications PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.ApplicationsRightsByGroup
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByGroup]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[ApplicationsRightsByGroup]
GO
CREATE TABLE [dbo].[ApplicationsRightsByGroup]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
ID_Application                      INT NOT NULL,
ID_GroupOrPerson                    INT NOT NULL,
ReleasedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ReleasedBy                          INT NOT NULL,
CONSTRAINT PK_ApplicationsRightsByGroup PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.ApplicationsRightsByUser
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[ApplicationsRightsByUser]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[ApplicationsRightsByUser]
GO
CREATE TABLE [dbo].[ApplicationsRightsByUser]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
ID_Application                      INT NOT NULL,
ID_GroupOrPerson                    INT NOT NULL,
ReleasedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ReleasedBy                          INT NOT NULL,
DevelopmentTeamMember               BIT DEFAULT (0) NOT NULL,
CONSTRAINT PK_ApplicationsRightsByUser PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.Benutzer
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Benutzer]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Benutzer]
GO
CREATE TABLE [dbo].[Benutzer]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
Loginname                           VARCHAR(20) NOT NULL,
LoginPW                             VARCHAR(4096) NOT NULL,
Company                             NVARCHAR(100),
Anrede                              VARCHAR(20) NOT NULL,
Titel                               NVARCHAR(40),
Vorname                             NVARCHAR(60) NOT NULL,
Nachname                            NVARCHAR(60) NOT NULL,
Namenszusatz                        NVARCHAR(40),
[E-MAIL]                              VARCHAR(50) NOT NULL,
Strasse                             NVARCHAR(60),
PLZ                                 NVARCHAR(20),
Ort                                 NVARCHAR(100),
State                               NVARCHAR(60),
Land                                VARCHAR(30),
LoginCount                          DECIMAL(18, 0) DEFAULT (0) NOT NULL,
LoginFailures                       TINYINT DEFAULT (0) NOT NULL,
LoginLockedTill                     DATETIME,
LoginDisabled                       BIT DEFAULT (0) NOT NULL,
AccountAccessability                INT NOT NULL,
CreatedOn                           DATETIME DEFAULT (getdate()) NOT NULL,
ModifiedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
LastLoginOn                         DATETIME,
LastLoginViaRemoteIP                VARCHAR(32),
CurrentLoginViaRemoteIP             VARCHAR(32),
[1stPreferredLanguage]                INT,
[2ndPreferredLanguage]                INT,
[3rdPreferredLanguage]                INT,
CustomerNo                          NVARCHAR(100),
SupplierNo                          NVARCHAR(100),
System_SessionID                    INT,
CONSTRAINT PK_Benutzer PRIMARY KEY CLUSTERED (ID ),
CONSTRAINT IX_Benutzer UNIQUE NONCLUSTERED(Loginname )
);

GO

----------------------------------------------------
-- dbo.Gruppen
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Gruppen]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Gruppen]
GO
CREATE TABLE [dbo].[Gruppen]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
Name                                NVARCHAR(100) NOT NULL,
Description                         NVARCHAR(1024),
ReleasedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ReleasedBy                          INT NOT NULL,
SystemGroup                         BIT DEFAULT (0) NOT NULL,
ModifiedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ModifiedBy                          INT NOT NULL,
CONSTRAINT PK_Gruppen PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.Languages
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Languages]
GO
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Languages]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[Languages]
GO
CREATE TABLE [dbo].[Languages]
(
ID                                  INT NOT NULL,
Abbreviation                        VARCHAR(10) NOT NULL,
Description_OwnLang                 VARCHAR(50) NOT NULL,
Description                         VARCHAR(50) NOT NULL,
IsActive                            BIT NOT NULL,
BrowserLanguageID                   VARCHAR(10),
CONSTRAINT PK_Languages PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.Log
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Log]
GO
CREATE TABLE [dbo].[Log]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
UserID                              INT NOT NULL,
LoginDate                           DATETIME NOT NULL,
RemoteIP                            VARCHAR(32) NOT NULL,
ServerIP                            VARCHAR(32) NOT NULL,
ApplicationID                       INT,
URL                                 VARCHAR(1024),
ConflictType                        INT NOT NULL,
ConflictDescription                 VARCHAR(1024),
CONSTRAINT PK_Log PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.Log_eMail_Attachments
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_eMail_Attachments]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Log_eMail_Attachments]
GO
CREATE TABLE [dbo].[Log_eMail_Attachments]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
eMailMsgID                          INT NOT NULL,
BinaryData                          VARBINARY(50) NOT NULL,
ObjectName                          VARCHAR(50) NOT NULL
);

GO

----------------------------------------------------
-- dbo.Log_eMailMessages
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_eMailMessages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Log_eMailMessages]
GO
CREATE TABLE [dbo].[Log_eMailMessages]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
UserID                              INT NOT NULL,
[To]                                  VARCHAR(50) NOT NULL,
CC                                  VARCHAR(50),
BCC                                 VARCHAR(50),
[From]                                VARCHAR(50),
Subject                             VARCHAR(50),
Body                                VARCHAR(50),
DateTime                            DATETIME NOT NULL
);

GO

----------------------------------------------------
-- dbo.Log_TextMessages
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_TextMessages]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Log_TextMessages]
GO
CREATE TABLE [dbo].[Log_TextMessages]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
UserID                              INT,
GroupID                             INT,
MessageCategory                     VARCHAR(50),
MessageSubject                      VARCHAR(512) NOT NULL,
MessageText                         NTEXT NOT NULL,
MessageLink                         VARCHAR(50),
DateTime                            DATETIME NOT NULL,
AlreadyDisplayed                    BIT NOT NULL,
CONSTRAINT PK_Log_TextMessages PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.Log_Users
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Log_Users]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Log_Users]
GO
CREATE TABLE [dbo].[Log_Users]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
ID_User                             INT NOT NULL,
Type                                VARCHAR(50) NOT NULL,
Value                               nvarchar(255),
ModifiedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
CONSTRAINT PK_Log_Users PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.Memberships
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Memberships]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[Memberships]
GO
CREATE TABLE [dbo].[Memberships]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
ID_Group                            INT NOT NULL,
ID_User                             INT NOT NULL,
ReleasedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ReleasedBy                          INT NOT NULL,
CONSTRAINT PK_Memberships PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.System_AccessLevels
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_AccessLevels]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_AccessLevels]
GO
CREATE TABLE [dbo].[System_AccessLevels]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
Title                               NVARCHAR(100) NOT NULL,
Remarks                             NTEXT,
ReleasedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ReleasedBy                          INT NOT NULL,
ModifiedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ModifiedBy                          INT NOT NULL,
CONSTRAINT PK_System_AccessLevels PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.System_ScriptEngines
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_ScriptEngines]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_ScriptEngines]
GO
CREATE TABLE [dbo].[System_ScriptEngines]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
EngineName                          NVARCHAR(100) NOT NULL,
FileName_EngineLogin                VARCHAR(256) NOT NULL,
CONSTRAINT PK_System_ScriptEngines PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.System_ServerGroups
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_ServerGroups]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_ServerGroups]
GO
CREATE TABLE [dbo].[System_ServerGroups]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
ServerGroup                         nvarchar(255) NOT NULL,
ID_Group_Public                     INT NOT NULL,
ID_Group_Anonymous                  INT NOT NULL,
MasterServer                        INT,
UserAdminServer                     INT,
AreaImage                           nvarchar(255) NOT NULL,
AreaButton                          nvarchar(255) NOT NULL,
AreaNavTitle                        nvarchar(255),
AreaCompanyFormerTitle              nvarchar(255) NOT NULL,
AreaCompanyTitle                    nvarchar(255) NOT NULL,
AreaSecurityContactEMail            nvarchar(255) NOT NULL,
AreaSecurityContactTitle            nvarchar(255) NOT NULL,
AreaDevelopmentContactEMail         nvarchar(255) NOT NULL,
AreaDevelopmentContactTitle         nvarchar(255) NOT NULL,
AreaContentManagementContactEMail   nvarchar(255) NOT NULL,
AreaContentManagementContactTitle   nvarchar(255) NOT NULL,
AreaUnspecifiedContactEMail         nvarchar(255) NOT NULL,
AreaUnspecifiedContactTitle         nvarchar(255) NOT NULL,
AreaCopyRightSinceYear              INT NOT NULL,
AreaCompanyWebSiteURL               nvarchar(255) NOT NULL,
AreaCompanyWebSiteTitle             nvarchar(255) NOT NULL,
ModifiedOn                          DATETIME DEFAULT (getdate()) NOT NULL,
ModifiedBy                          INT NOT NULL,
[AccessLevel_Default]               INT DEFAULT (0) NOT NULL,
CONSTRAINT PK_System_ServerGroups PRIMARY KEY CLUSTERED (ID ),
CONSTRAINT IX_System_ServerGroups UNIQUE NONCLUSTERED(ServerGroup )
);

GO

----------------------------------------------------
-- dbo.System_ServerGroupsAndTheirUserAccessLevels
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_ServerGroupsAndTheirUserAccessLevels]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_ServerGroupsAndTheirUserAccessLevels]
GO
CREATE TABLE [dbo].[System_ServerGroupsAndTheirUserAccessLevels]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
ID_AccessLevel                      INT,
ID_ServerGroup                      INT,
Remarks                             NTEXT,
CONSTRAINT PK__Kopie_von_System__2CF2ADDF PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.System_Servers
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_Servers]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_Servers]
GO
CREATE TABLE [dbo].[System_Servers]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
Enabled                             BIT DEFAULT (0) NOT NULL,
IP                                  VARCHAR(32) NOT NULL,
ServerDescription                   NVARCHAR(400) NOT NULL,
ServerGroup                         INT NOT NULL,
ServerProtocol                      VARCHAR(50) NOT NULL,
ServerName                          NVARCHAR(400) NOT NULL,
ServerPort                          INT,
ReAuthenticateByIP                  BIT NOT NULL,
WebSessionTimeout                   INT NOT NULL,
LockTimeout                         INT NOT NULL,
CONSTRAINT PK_System_Servers PRIMARY KEY CLUSTERED (ID ),
CONSTRAINT IX_System_Servers UNIQUE NONCLUSTERED(IP )
);

GO

----------------------------------------------------
-- dbo.System_SessionValues
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_SessionValues]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_SessionValues]
GO
CREATE TABLE [dbo].[System_SessionValues]
(
ID                                  int IDENTITY(1,1) NOT NULL,
SessionID                           INT NOT NULL,
VarName                             VARCHAR(50) NOT NULL,
VarType                             INT NOT NULL,
ValueInt                            int,
ValueNText                          NTEXT,
ValueFloat                          FLOAT,
ValueDecimal                        DECIMAL(18, 0),
ValueDateTime                       DATETIME,
ValueImage                          IMAGE,
ValueBool                           BIT,
CONSTRAINT PK_SessionValues PRIMARY KEY CLUSTERED (ID ),
CONSTRAINT IX_System_SessionValues UNIQUE NONCLUSTERED(VarName,SessionID )
);

GO

----------------------------------------------------
-- dbo.System_UserSessions
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_UserSessions]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_UserSessions]
GO
CREATE TABLE [dbo].[System_UserSessions]
(
ID_Session                          INT IDENTITY(1,1) NOT NULL,
ID_User                             INT NOT NULL,
CreatedOn                           DATETIME DEFAULT (getdate()) NOT NULL,
CONSTRAINT PK_UserSessions PRIMARY KEY CLUSTERED (ID_Session )
);

GO

----------------------------------------------------
-- dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
GO
CREATE TABLE [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
(
ID                                  int IDENTITY(1,1) NOT NULL,
SessionID                           INT NOT NULL,
ServerGroup                         INT NOT NULL,
Server                              INT NOT NULL,
ScriptEngine_SessionID              NVARCHAR(1024),
ScriptEngine_LogonGUID              INT,
ScriptEngine_ID                     INT NOT NULL,
LastSessionStateRefresh             DATETIME DEFAULT (getdate()) NOT NULL,
Inactive                            BIT DEFAULT (0) NOT NULL,
CONSTRAINT PK__System_WebAreasA__31B762FC PRIMARY KEY CLUSTERED (ID )
);

GO

----------------------------------------------------
-- dbo.System_WebAreaScriptEnginesAuthorization
----------------------------------------------------
if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_WebAreaScriptEnginesAuthorization]') and OBJECTPROPERTY(object_id, N'IsUserTable') = 1) drop table [dbo].[System_WebAreaScriptEnginesAuthorization]
GO
CREATE TABLE [dbo].[System_WebAreaScriptEnginesAuthorization]
(
ID                                  INT IDENTITY(1,1) NOT NULL,
Server                              INT NOT NULL,
ScriptEngine                        INT NOT NULL,
CONSTRAINT PK__Kopie_von_System__395884C4 PRIMARY KEY CLUSTERED (ID ),
CONSTRAINT IX_System_WebAreaScriptEnginesAuthorization UNIQUE NONCLUSTERED(Server,ScriptEngine )
);

GO
print 'TABLECREATION - END'

/******************************************************
  Insert data   Begin
******************************************************/
print 'INSERTDATA - BEGIN'

-----------------------------------------------------------
--Insert data into dbo.Benutzer
-----------------------------------------------------------
print 'dbo.Benutzer'
IF(	IDENT_INCR( 'dbo.Benutzer' ) IS NOT NULL OR IDENT_SEED('dbo.Benutzer') IS NOT NULL ) SET IDENTITY_INSERT dbo.Benutzer ON
INSERT INTO dbo.Benutzer (ID,Loginname,LoginPW,Company,Anrede,Titel,Vorname,Nachname,Namenszusatz,[E-MAIL],Strasse,PLZ,Ort,State,Land,LoginCount,LoginFailures,LoginLockedTill,LoginDisabled,AccountAccessability,CreatedOn,ModifiedOn,LastLoginOn,LastLoginViaRemoteIP,CurrentLoginViaRemoteIP,[1stPreferredLanguage],[2ndPreferredLanguage],[3rdPreferredLanguage],CustomerNo,SupplierNo,System_SessionID) 
VALUES('1','admin','glbcamjjon','','Mr.','','camm WebManager','Administrator','','admin@localhost','','','','','',0,'0',NULL,0,'1',getdate(),getdate(),getdate(),'127.0.0.1','127.0.0.1','1',NULL,NULL,'','',NULL)
IF(	IDENT_INCR( 'dbo.Benutzer' ) IS NOT NULL OR IDENT_SEED('dbo.Benutzer') IS NOT NULL ) SET IDENTITY_INSERT dbo.Benutzer OFF
GO

-----------------------------------------------------------
--Insert data into dbo.Gruppen
-----------------------------------------------------------
print 'dbo.Gruppen'
IF(	IDENT_INCR( 'dbo.Gruppen' ) IS NOT NULL OR IDENT_SEED('dbo.Gruppen') IS NOT NULL ) SET IDENTITY_INSERT dbo.Gruppen ON
INSERT INTO dbo.Gruppen (ID,Name,Description,ReleasedOn,ReleasedBy,SystemGroup,ModifiedOn,ModifiedBy) VALUES('6','Supervisors','System group: global administrators of the whole secured area',getdate(),'1',1,getdate(),'1')
INSERT INTO dbo.Gruppen (ID,Name,Description,ReleasedOn,ReleasedBy,SystemGroup,ModifiedOn,ModifiedBy) VALUES('7','Security Administration','System group: creation and modification of user authorizations and other properties',getdate(),'1',1,getdate(),'1')
INSERT INTO dbo.Gruppen (ID,Name,Description,ReleasedOn,ReleasedBy,SystemGroup,ModifiedOn,ModifiedBy) VALUES('58','Anonymous','System group: all anonymous users',getdate(),'1',1,getdate(),'1')
IF(	IDENT_INCR( 'dbo.Gruppen' ) IS NOT NULL OR IDENT_SEED('dbo.Gruppen') IS NOT NULL ) SET IDENTITY_INSERT dbo.Gruppen OFF
GO

-----------------------------------------------------------
--Insert data into dbo.Languages
-----------------------------------------------------------
print 'dbo.Languages'
IF(	IDENT_INCR( 'dbo.Languages' ) IS NOT NULL OR IDENT_SEED('dbo.Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.Languages ON
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('1','ENG','English (BE)','English (BE)',1,'EN')
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('2','DEU','Deutsch','German',1,'DE-de')
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('3','FRA','Francais','French',1,'FR-fr')
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('4','ESP','Espanol','Spanish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('5','aar','Afar','Afar',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('6','abk','Abkhazian','Abkhazian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('7','ace','Achinese','Achinese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('8','ach','Acoli','Acoli',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('9','ada','Adangme','Adangme',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('10','afa','Afro-Asiatic (Other)','Afro-Asiatic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('11','afh','Afrihili','Afrihili',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('12','afr','Afrikaans','Afrikaans',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('13','aka','Akan','Akan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('14','akk','Akkadian','Akkadian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('16','ale','Aleut','Aleut',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('17','alg','Algonquian languages','Algonquian languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('18','amh','Amharic','Amharic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('19','ang','English, Old (ca.450-1100)','English, Old (ca.450-1100)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('20','apa','Apache languages','Apache languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('21','ara','Arabic','Arabic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('22','arc','Aramaic','Aramaic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('23','arm','Armenian','Armenian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('24','arn','Araucanian','Araucanian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('25','arp','Arapaho','Arapaho',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('26','art','Artificial (Other)','Artificial (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('27','arw','Arawak','Arawak',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('28','asm','Assamese','Assamese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('29','ast','Asturian; Bable','Asturian; Bable',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('30','ath','Athapascan languages','Athapascan languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('31','aus','Australian languages','Australian languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('32','ava','Avaric','Avaric',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('33','ave','Avestan','Avestan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('34','awa','Awadhi','Awadhi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('35','aym','Aymara','Aymara',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('36','aze','Azerbaijani','Azerbaijani',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('37','bad','Banda','Banda',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('38','bai','Bamileke languages','Bamileke languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('39','bak','Bashkir','Bashkir',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('40','bal','Baluchi','Baluchi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('41','bam','Bambara','Bambara',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('42','ban','Balinese','Balinese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('43','baq','Basque','Basque',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('44','bas','Basa','Basa',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('45','bat','Baltic (Other)','Baltic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('46','bej','Beja','Beja',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('47','bel','Belarusian','Belarusian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('48','bem','Bemba','Bemba',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('49','ben','Bengali','Bengali',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('50','ber','Berber (Other)','Berber (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('51','bho','Bhojpuri','Bhojpuri',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('52','bih','Bihari','Bihari',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('53','bik','Bikol','Bikol',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('54','bin','Bini','Bini',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('55','bis','Bislama','Bislama',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('56','bla','Siksika','Siksika',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('57','bnt','Bantu (Other)','Bantu (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('58','tib','Tibetan','Tibetan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('59','bos','Bosnian','Bosnian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('60','bra','Braj','Braj',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('61','bre','Breton','Breton',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('62','btk','Batak (Indonesia)','Batak (Indonesia)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('63','bua','Buriat','Buriat',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('64','bug','Buginese','Buginese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('65','bul','Bulgarian','Bulgarian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('66','bur','Burmese','Burmese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('67','cad','Caddo','Caddo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('68','cai','Central American Indian (Other)','Central American Indian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('70','car','Carib','Carib',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('71','cat','Catalan','Catalan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('72','cau','Caucasian (Other)','Caucasian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('73','ceb','Cebuano','Cebuano',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('74','cel','Celtic (Other)','Celtic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('75','cze','Czech','Czech',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('76','cha','Chamorro','Chamorro',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('77','chb','Chibcha','Chibcha',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('78','che','Chechen','Chechen',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('79','chg','Chagatai','Chagatai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('80','chi','Chinese','Chinese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('81','chk','Chuukese','Chuukese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('82','chm','Mari','Mari',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('83','chn','Chinook jargon','Chinook jargon',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('84','cho','Choctaw','Choctaw',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('85','chp','Chipewyan','Chipewyan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('86','chr','Cherokee','Cherokee',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('88','chv','Chuvash','Chuvash',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('89','chy','Cheyenne','Cheyenne',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('90','cmc','Chamic languages','Chamic languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('91','cop','Coptic','Coptic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('92','cor','Cornish','Cornish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('93','cos','Corsican','Corsican',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('94','cpe','Creoles and pidgins, English based (Other)','Creoles and pidgins, English based (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('95','cpf','Creoles and pidgins, French-based (Other)','Creoles and pidgins, French-based (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('96','cpp','Creoles and pidgins,','Creoles and pidgins,',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('98','cre','Cree','Cree',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('99','crp','Creoles and pidgins (Other)','Creoles and pidgins (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('100','cus','Cushitic (Other)','Cushitic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('101','wel','Welsh','Welsh',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('103','dak','Dakota','Dakota',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('104','dan','Danish','Danish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('105','day','Dayak','Dayak',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('106','del','Delaware','Delaware',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('107','den','Slave (Athapascan)','Slave (Athapascan)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('109','dgr','Dogrib','Dogrib',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('110','din','Dinka','Dinka',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('111','div','Divehi','Divehi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('112','doi','Dogri','Dogri',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('113','dra','Dravidian (Other)','Dravidian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('114','dua','Duala','Duala',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('115','dum','Dutch, Middle (ca.1050-1350)','Dutch, Middle (ca.1050-1350)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('117','dyu','Dyula','Dyula',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('118','dzo','Dzongkha','Dzongkha',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('119','efi','Efik','Efik',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('120','egy','Egyptian (Ancient)','Egyptian (Ancient)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('121','eka','Ekajuk','Ekajuk',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('123','elx','Elamite','Elamite',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('125','enm','English, Middle (1100-1500)','English, Middle (1100-1500)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('126','epo','Esperanto','Esperanto',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('127','est','Estonian','Estonian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('129','ewe','Ewe','Ewe',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('130','ewo','Ewondo','Ewondo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('131','fan','Fang','Fang',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('132','fao','Faroese','Faroese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('134','fat','Fanti','Fanti',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('135','fij','Fijian','Fijian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('136','fin','Finnish','Finnish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('137','fiu','Finno-Ugrian (Other)','Finno-Ugrian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('138','fon','Fon','Fon',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('140','frm','French, Middle (ca.1400-1800)','French, Middle (ca.1400-1800)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('141','fro','French, Old (842-ca.1400)','French, Old (842-ca.1400)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('142','fry','Frisian','Frisian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('143','ful','Fulah','Fulah',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('144','fur','Friulian','Friulian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('145','gaa','Ga','Ga',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('146','gay','Gayo','Gayo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('147','gba','Gbaya','Gbaya',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('148','gem','Germanic (Other)','Germanic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('151','gez','Geez','Geez',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('152','gil','Gilbertese','Gilbertese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('153','gla','Gaelic; Scottish Gaelic','Gaelic; Scottish Gaelic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('154','gle','Irish','Irish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('155','glg','Gallegan','Gallegan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('156','glv','Manx','Manx',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('157','gmh','German, Middle High (ca.1050-1500)','German, Middle High (ca.1050-1500)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('158','goh','German, Old High (ca.750-1050)','German, Old High (ca.750-1050)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('159','gon','Gondi','Gondi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('160','gor','Gorontalo','Gorontalo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('161','got','Gothic','Gothic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('162','grb','Grebo','Grebo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('163','grc','Greek, Ancient (to 1453)','Greek, Ancient (to 1453)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('164','gre','Greek, Modern (1453-)','Greek, Modern (1453-)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('165','grn','Guarani','Guarani',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('166','guj','Gujarati','Gujarati',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('167','gwi','Gwich´in','Gwich´in',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('168','hai','Haida','Haida',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('169','hau','Hausa','Hausa',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('170','haw','Hawaiian','Hawaiian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('171','heb','Hebrew','Hebrew',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('172','her','Herero','Herero',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('173','hil','Hiligaynon','Hiligaynon',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('174','him','Himachali','Himachali',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('175','hin','Hindi','Hindi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('176','hit','Hittite','Hittite',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('177','hmn','Hmong','Hmong',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('178','hmo','Hiri Motu','Hiri Motu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('179','scr','Croatian','Croatian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('180','hun','Hungarian','Hungarian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('181','hup','Hupa','Hupa',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('183','iba','Iban','Iban',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('184','ibo','Igbo','Igbo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('185','ice','Icelandic','Icelandic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('186','ido','Ido','Ido',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('187','ijo','Ijo','Ijo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('188','iku','Inuktitut','Inuktitut',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('189','ile','Interlingue','Interlingue',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('190','ilo','Iloko','Iloko',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('191','ina','Interlingua (International Auxiliary','Interlingua (International Auxiliary',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('193','inc','Indic (Other)','Indic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('194','ind','Indonesian','Indonesian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('195','ine','Indo-European (Other)','Indo-European (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('196','ipk','Inupiaq','Inupiaq',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('197','ira','Iranian (Other)','Iranian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('198','iro','Iroquoian languages','Iroquoian languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('200','ita','Italian','Italian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('201','jav','Javanese','Javanese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('202','jpn','Japanese','Japanese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('203','jpr','Judeo-Persian','Judeo-Persian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('204','jrb','Judeo-Arabic','Judeo-Arabic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('205','kaa','Kara-Kalpak','Kara-Kalpak',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('206','kab','Kabyle','Kabyle',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('207','kac','Kachin','Kachin',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('208','kal','Kalaallisut','Kalaallisut',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('209','kam','Kamba','Kamba',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('210','kan','Kannada','Kannada',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('211','kar','Karen','Karen',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('212','kas','Kashmiri','Kashmiri',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('213','geo','Georgian','Georgian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('214','kau','Kanuri','Kanuri',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('215','kaw','Kawi','Kawi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('216','kaz','Kazakh','Kazakh',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('217','kha','Khasi','Khasi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('218','khi','Khoisan (Other)','Khoisan (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('219','khm','Khmer','Khmer',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('220','kho','Khotanese','Khotanese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('221','kik','Kikuyu; Gikuyu','Kikuyu; Gikuyu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('222','kin','Kinyarwanda','Kinyarwanda',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('223','kir','Kirghiz','Kirghiz',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('224','kmb','Kimbundu','Kimbundu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('225','kok','Konkani','Konkani',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('226','kom','Komi','Komi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('227','kon','Kongo','Kongo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('228','kor','Korean','Korean',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('229','kos','Kosraean','Kosraean',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('230','kpe','Kpelle','Kpelle',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('231','kro','Kru','Kru',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('232','kru','Kurukh','Kurukh',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('233','kua','Kuanyama; Kwanyama','Kuanyama; Kwanyama',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('234','kum','Kumyk','Kumyk',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('235','kur','Kurdish','Kurdish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('236','kut','Kutenai','Kutenai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('237','lad','Ladino','Ladino',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('238','lah','Lahnda','Lahnda',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('239','lam','Lamba','Lamba',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('240','lao','Lao','Lao',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('241','lat','Latin','Latin',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('242','lav','Latvian','Latvian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('243','lez','Lezghian','Lezghian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('244','lim','Limburgan; Limburger; Limburgish','Limburgan; Limburger; Limburgish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('245','lin','Lingala','Lingala',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('246','lit','Lithuanian','Lithuanian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('247','lol','Mongo','Mongo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('248','loz','Lozi','Lozi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('249','ltz','Letzeburgesch','Luxembourgish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('250','lua','Luba-Lulua','Luba-Lulua',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('251','lub','Luba-Katanga','Luba-Katanga',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('252','lug','Ganda','Ganda',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('253','lui','Luiseno','Luiseno',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('254','lun','Lunda','Lunda',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('255','luo','Luo (Kenya and Tanzania)','Luo (Kenya and Tanzania)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('256','lus','lushai','lushai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('258','mad','Madurese','Madurese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('259','mag','Magahi','Magahi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('260','mah','Marshallese','Marshallese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('261','mai','Maithili','Maithili',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('262','mak','Makasar','Makasar',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('263','mal','Malayalam','Malayalam',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('264','man','Mandingo','Mandingo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('265','mao','Maori','Maori',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('266','map','Austronesian (Other)','Austronesian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('268','mar','Marathi','Marathi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('269','mas','Masai','Masai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('271','mdr','Mandar','Mandar',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('272','men','Mende','Mende',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('273','mga','Irish, Middle (900-1200)','Irish, Middle (900-1200)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('274','mic','Micmac','Micmac',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('275','min','Minangkabau','Minangkabau',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('277','mac','Macedonian','Macedonian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('278','mkh','Mon-Khmer (Other)','Mon-Khmer (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('279','mlg','Malagasy','Malagasy',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('280','mlt','Maltese','Maltese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('281','mnc','Manchu','Manchu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('282','mni','Manipuri','Manipuri',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('283','mno','Manobo languages','Manobo languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('284','moh','Mohawk','Mohawk',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('285','mol','Moldavian','Moldavian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('286','mon','Mongolian','Mongolian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('287','mos','Mossi','Mossi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('289','may','Malay','Malay',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('290','mul','Multiple languages','Multiple languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('291','mun','Munda languages','Munda languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('292','mus','Creek','Creek',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('293','mwr','Marwari','Marwari',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('295','myn','Mayan languages','Mayan languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('296','nah','Nahuatl','Nahuatl',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('297','nai','North American Indian','North American Indian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('299','nap','Neapolitan','Neapolitan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('300','nau','Nauru','Nauru',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('301','nav','Navajo; Navaho','Navajo; Navaho',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('302','nbl','Ndebele, South; South Ndebele','Ndebele, South; South Ndebele',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('303','nde','Ndebele, North; North Ndebele','Ndebele, North; North Ndebele',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('304','ndo','Ndonga','Ndonga',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('305','nds','Low German; Low Saxon; German, Low; Saxon, Low','Low German; Low Saxon; German, Low; Saxon, Low',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('306','nep','Nepali','Nepali',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('307','new','Newari','Newari',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('308','nia','Nias','Nias',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('309','nic','Niger-Kordofanian (Other)','Niger-Kordofanian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('310','niu','Niuean','Niuean',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('311','dut','Dutch','Dutch',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('312','non','Norse, Old','Norse, Old',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('313','nor','Norwegian','Norwegian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('314','nno','Norwegian Nynorsk; Nynorsk, Norwegian','Norwegian Nynorsk; Nynorsk, Norwegian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('315','nob','Norwegian Bokmål; Bokmål, Norwegian','Norwegian Bokmål; Bokmål, Norwegian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('316','nso','Sotho, Northern','Sotho, Northern',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('317','nub','Nubian languages','Nubian languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('318','nya','Chichewa; Chewa; Nyanja','Chichewa; Chewa; Nyanja',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('319','nym','Nyamwezi','Nyamwezi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('320','nyn','Nyankole','Nyankole',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('321','nyo','Nyoro','Nyoro',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('322','nzi','Nzima','Nzima',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('323','oci','Occitan (post 1500); Provençal','Occitan (post 1500); Provençal',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('324','oji','Ojibwa','Ojibwa',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('325','ori','Oriya','Oriya',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('326','orm','Oromo','Oromo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('327','osa','Osage','Osage',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('328','oss','Ossetian; Ossetic','Ossetian; Ossetic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('329','ota','Turkish, Ottoman (1500-1928)','Turkish, Ottoman (1500-1928)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('330','oto','Otomian languages','Otomian languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('331','paa','Papuan (Other)','Papuan (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('332','pag','Pangasinan','Pangasinan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('333','pal','Pahlavi','Pahlavi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('334','pam','Pampanga','Pampanga',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('335','pan','Panjabi','Panjabi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('336','pap','Papiamento','Papiamento',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('337','pau','Palauan','Palauan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('338','peo','Persian, Old (ca.600-400 B.C.)','Persian, Old (ca.600-400 B.C.)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('339','per','Persian','Persian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('340','phi','Philippine (Other)','Philippine (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('341','phn','Phoenician','Phoenician',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('342','pli','Pali','Pali',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('343','pol','Polish','Polish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('344','pon','Pohnpeian','Pohnpeian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('345','por','Portuguese','Portuguese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('346','pra','Prakrit languages','Prakrit languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('347','pro','Provençal, Old (to 1500)','Provençal, Old (to 1500)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('348','pus','Pushto','Pushto',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('350','que','Quechua','Quechua',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('351','raj','Rajasthani','Rajasthani',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('352','rap','Rapanui','Rapanui',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('353','rar','Rarotongan','Rarotongan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('354','roa','Romance (Other)','Romance (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('355','roh','Raeto-Romance','Raeto-Romance',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('356','rom','Romany','Romany',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('357','rum','Romanian','Romanian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('358','run','Rundi','Rundi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('359','rus','Russian','Russian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('360','sad','Sandawe','Sandawe',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('361','sag','Sango','Sango',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('362','sah','Yakut','Yakut',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('363','sai','South American Indian (Other)','South American Indian (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('365','sal','Salishan languages','Salishan languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('366','sam','Samaritan Aramaic','Samaritan Aramaic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('367','san','Sanskrit','Sanskrit',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('368','sas','Sasak','Sasak',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('369','sat','Santali','Santali',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('371','sco','Scots','Scots',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('373','sel','Selkup','Selkup',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('374','sem','Semitic (Other)','Semitic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('375','sga','Irish, Old (to 900)','Irish, Old (to 900)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('376','sgn','Sign Languages','Sign Languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('377','shn','Shan','Shan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('378','sid','Sidamo','Sidamo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('379','sin','Sinhalese','Sinhalese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('380','sio','Siouan languages','Siouan languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('381','sit','Sino-Tibetan (Other)','Sino-Tibetan (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('382','sla','Slavic (Other)','Slavic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('383','slo','Slovak','Slovak',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('384','slv','Slovenian','Slovenian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('385','sma','Southern Sami','Southern Sami',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('386','sme','Northern Sami','Northern Sami',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('387','smi','Sami languages (Other)','Sami languages (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('388','smj','Lule Sami','Lule Sami',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('389','smn','Inari Sami','Inari Sami',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('390','smo','Samoan','Samoan',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('391','sms','Skolt Sami','Skolt Sami',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('392','sna','Shona','Shona',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('393','snd','Sindhi','Sindhi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('394','snk','Soninke','Soninke',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('395','sog','Sogdian','Sogdian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('396','som','Somali','Somali',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('397','son','Songhai','Songhai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('398','sot','Sotho, Southern','Sotho, Southern',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('399','spa','Spanish; Castilian','Spanish; Castilian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('400','alb','Albanian','Albanian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('401','srd','Sardinian','Sardinian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('402','scc','Serbian','Serbian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('403','srr','Serer','Serer',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('404','ssa','Nilo-Saharan (Other)','Nilo-Saharan (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('405','ssw','Swati','Swati',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('406','suk','Sukuma','Sukuma',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('407','sun','Sundanese','Sundanese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('408','sus','Susu','Susu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('409','sux','Sumerian','Sumerian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('410','swa','Swahili','Swahili',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('411','swe','Swedish','Swedish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('412','syr','Syriac','Syriac',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('413','tah','Tahitian','Tahitian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('414','tai','Tai (Other)','Tai (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('415','tam','Tamil','Tamil',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('416','tat','Tatar','Tatar',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('417','tel','Telugu','Telugu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('418','tem','Timne','Timne',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('419','ter','Tereno','Tereno',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('420','tet','Tetum','Tetum',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('421','tgk','Tajik','Tajik',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('422','tgl','Tagalog','Tagalog',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('423','tha','Thai','Thai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('425','tig','Tigre','Tigre',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('426','tir','Tigrinya','Tigrinya',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('427','tiv','Tiv','Tiv',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('428','tkl','Tokelau','Tokelau',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('429','tli','Tlingit','Tlingit',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('430','tmh','Tamashek','Tamashek',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('431','tog','Tonga (Nyasa)','Tonga (Nyasa)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('432','ton','Tonga (Tonga Islands)','Tonga (Tonga Islands)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('433','tpi','Tok Pisin','Tok Pisin',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('434','tsi','Tsimshian','Tsimshian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('435','tsn','Tswana','Tswana',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('436','tso','Tsonga','Tsonga',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('437','tuk','Turkmen','Turkmen',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('438','tum','Tumbuka','Tumbuka',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('439','tup','Tupi languages','Tupi languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('440','tur','Turkish','Turkish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('441','tut','Altaic (Other)','Altaic (Other)',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('442','tvl','Tuvalu','Tuvalu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('443','twi','Twi','Twi',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('444','tyv','Tuvinian','Tuvinian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('445','uga','Ugaritic','Ugaritic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('446','uig','Uighur','Uighur',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('447','ukr','Ukrainian','Ukrainian',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('448','umb','Umbundu','Umbundu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('449','und','Undetermined','Undetermined',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('450','urd','Urdu','Urdu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('451','uzb','Uzbek','Uzbek',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('452','vai','Vai','Vai',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('453','ven','Venda','Venda',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('454','vie','Vietnamese','Vietnamese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('455','vol','Volapük','Volapük',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('456','vot','Votic','Votic',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('457','wak','Wakashan languages','Wakashan languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('458','wal','Walamo','Walamo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('459','war','Waray','Waray',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('460','was','Washo','Washo',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('462','wen','Sorbian languages','Sorbian languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('463','wln','Walloon','Walloon',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('464','wol','Wolof','Wolof',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('465','xho','Xhosa','Xhosa',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('466','yao','Yao','Yao',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('467','yap','Yapese','Yapese',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('468','yid','Yiddish','Yiddish',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('469','yor','Yoruba','Yoruba',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('470','ypk','Yupik languages','Yupik languages',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('471','zap','Zapotec','Zapotec',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('472','zen','Zenaga','Zenaga',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('473','zha','Zhuang; Chuang','Zhuang; Chuang',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('475','znd','Zande','Zande',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('476','zul','Zulu','Zulu',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('477','zun','Zuni','Zuni',0,NULL)
INSERT INTO dbo.Languages (ID,Abbreviation,Description_OwnLang,Description,IsActive,BrowserLanguageID) VALUES('478','ENU','English (US)','English (US)',0,'EN-us')
IF(	IDENT_INCR( 'dbo.Languages' ) IS NOT NULL OR IDENT_SEED('dbo.Languages') IS NOT NULL ) SET IDENTITY_INSERT dbo.Languages OFF
GO

-----------------------------------------------------------
--Insert data into dbo.Memberships
-----------------------------------------------------------
print 'dbo.Memberships'
IF(	IDENT_INCR( 'dbo.Memberships' ) IS NOT NULL OR IDENT_SEED('dbo.Memberships') IS NOT NULL ) SET IDENTITY_INSERT dbo.Memberships ON
INSERT INTO dbo.Memberships (ID,ID_Group,ID_User,ReleasedOn,ReleasedBy) VALUES('2','6','1',getdate(),'1')
IF(	IDENT_INCR( 'dbo.Memberships' ) IS NOT NULL OR IDENT_SEED('dbo.Memberships') IS NOT NULL ) SET IDENTITY_INSERT dbo.Memberships OFF
GO

-----------------------------------------------------------
--Insert data into dbo.System_AccessLevels
-----------------------------------------------------------
print 'dbo.AccessLevels'
IF(	IDENT_INCR( 'dbo.System_AccessLevels' ) IS NOT NULL OR IDENT_SEED('dbo.System_AccessLevels') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_AccessLevels ON
INSERT INTO dbo.System_AccessLevels (ID,Title,Remarks,ReleasedOn,ReleasedBy,ModifiedOn,ModifiedBy) VALUES('0','Extranet access only','Users which are only allowed to access servers in the extranet server group',getdate(),'1',getdate(),'1')
INSERT INTO dbo.System_AccessLevels (ID,Title,Remarks,ReleasedOn,ReleasedBy,ModifiedOn,ModifiedBy) VALUES('1','Extranet + Intranet','Users which are only allowed to access servers in the extranet server group and in the intranet server group',getdate(),'1',getdate(),'1')
INSERT INTO dbo.System_AccessLevels (ID,Title,Remarks,ReleasedOn,ReleasedBy,ModifiedOn,ModifiedBy) VALUES('2','Intranet access only','Users which are only allowed to access servers in the intranet server group',getdate(),'1',getdate(),'1')
IF(	IDENT_INCR( 'dbo.System_AccessLevels' ) IS NOT NULL OR IDENT_SEED('dbo.System_AccessLevels') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_AccessLevels OFF
GO

-----------------------------------------------------------
--Insert data into dbo.System_ScriptEngines
-----------------------------------------------------------
print 'dbo.System_ScriptEngines'
IF(	IDENT_INCR( 'dbo.System_ScriptEngines' ) IS NOT NULL OR IDENT_SEED('dbo.System_ScriptEngines') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_ScriptEngines ON
INSERT INTO dbo.System_ScriptEngines (ID,EngineName,FileName_EngineLogin) VALUES('1','ASP','ldirect.asp')
INSERT INTO dbo.System_ScriptEngines (ID,EngineName,FileName_EngineLogin) VALUES('2','ASPX','ldirect.aspx')
INSERT INTO dbo.System_ScriptEngines (ID,EngineName,FileName_EngineLogin) VALUES('3','SAP WebStudio','ldirect_sap.asp')
INSERT INTO dbo.System_ScriptEngines (ID,EngineName,FileName_EngineLogin) VALUES('4','PHP','ldirect.php')
IF(	IDENT_INCR( 'dbo.System_ScriptEngines' ) IS NOT NULL OR IDENT_SEED('dbo.System_ScriptEngines') IS NOT NULL ) SET IDENTITY_INSERT dbo.System_ScriptEngines OFF
GO

print 'INSERTDATA - END'
/******************************************************
  Insert data  End
******************************************************/


/******************************************************
  Tables  End
******************************************************/

/******************************************************
  Indexes  Begin
******************************************************/

CREATE NONCLUSTERED INDEX [IX_Applications] ON [dbo].[Applications_CurrentAndInactiveOnes](ResetIsNewUpdatedStatusOn )
GO
CREATE NONCLUSTERED INDEX [IX_Applications_1] ON [dbo].[Applications_CurrentAndInactiveOnes](LocationID,LanguageID )
GO
CREATE NONCLUSTERED INDEX [IX_Applications_2] ON [dbo].[Applications_CurrentAndInactiveOnes](Sort )
GO
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes] ON [dbo].[Applications_CurrentAndInactiveOnes] ( ReleasedBy  );
GO
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes_12] ON [dbo].[Applications_CurrentAndInactiveOnes](Title )
GO
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes_13] ON [dbo].[Applications_CurrentAndInactiveOnes](AuthsAsAppID )
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByGroup] ON [dbo].[ApplicationsRightsByGroup](ID_Application )
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByGroup_1] ON [dbo].[ApplicationsRightsByGroup](ID_GroupOrPerson )
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByUser] ON [dbo].[ApplicationsRightsByUser](ID_Application )
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByUser_1] ON [dbo].[ApplicationsRightsByUser](ID_GroupOrPerson )
GO
CREATE NONCLUSTERED INDEX [IX_Benutzer_1] ON [dbo].[Benutzer](System_SessionID )
GO
CREATE NONCLUSTERED INDEX [IX_Log_TextMessages] ON [dbo].[Log_TextMessages](MessageCategory )
GO
CREATE NONCLUSTERED INDEX [IX_Log_TextMessages_1] ON [dbo].[Log_TextMessages](UserID )
GO
CREATE NONCLUSTERED INDEX [IX_Log_TextMessages_2] ON [dbo].[Log_TextMessages](GroupID )
GO
CREATE NONCLUSTERED INDEX [IX_Log_Users] ON [dbo].[Log_Users](ID_User )
GO
CREATE NONCLUSTERED INDEX [IX_Log_Users_1] ON [dbo].[Log_Users](Type )
GO
CREATE NONCLUSTERED INDEX [IX_Memberships] ON [dbo].[Memberships](ID_Group )
GO
CREATE NONCLUSTERED INDEX [IX_Memberships_1] ON [dbo].[Memberships](ID_User )
GO
CREATE NONCLUSTERED INDEX [IX_System_WebAreasAuthorizedForSession] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes](ScriptEngine_ID )
GO
CREATE NONCLUSTERED INDEX [IX_System_WebAreasAuthorizedForSession_1] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes](LastSessionStateRefresh )
GO


/******************************************************
  Indexes  End
******************************************************/

/******************************************************
  Stored procedures  Begin
******************************************************/

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteApplicationRightsByGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_DeleteApplicationRightsByGroup]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByGroup] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteApplicationRightsByUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_DeleteApplicationRightsByUser]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteApplicationRightsByUser] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_ValidateUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_ValidateUser]
GO
CREATE PROCEDURE [dbo].[Public_ValidateUser] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO


SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_ServerDebug]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_ServerDebug]
GO
CREATE PROCEDURE [dbo].[Public_ServerDebug] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_ValidateGUIDLogin]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_ValidateGUIDLogin]
GO
CREATE PROCEDURE [dbo].[Public_ValidateGUIDLogin] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_ValidateDocument]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_ValidateDocument]
GO
CREATE PROCEDURE [dbo].[Public_ValidateDocument] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UserIsAuthorizedForApp]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_UserIsAuthorizedForApp]
GO
CREATE PROCEDURE [dbo].[Public_UserIsAuthorizedForApp] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UpdateUserPW]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_UpdateUserPW]
GO
CREATE PROCEDURE [dbo].[Public_UpdateUserPW] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_UpdateUserDetails]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_UpdateUserDetails]
GO
CREATE PROCEDURE [dbo].[Public_UpdateUserDetails] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_SetUserDetailData]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_SetUserDetailData]
GO
CREATE PROCEDURE [dbo].[Public_SetUserDetailData] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_RestorePassword]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_RestorePassword]
GO
CREATE PROCEDURE [dbo].[Public_RestorePassword] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_Logout]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_Logout]
GO
CREATE PROCEDURE [dbo].[Public_Logout] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetUserID]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetUserID]
GO
CREATE PROCEDURE [dbo].[Public_GetUserID] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetUserDetailData]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetUserDetailData]
GO
CREATE PROCEDURE [dbo].[Public_GetUserDetailData] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetToDoLogonList]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetToDoLogonList]
GO
CREATE PROCEDURE [dbo].[Public_GetToDoLogonList] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetServerConfig]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetServerConfig]
GO
CREATE PROCEDURE [dbo].[Public_GetServerConfig] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetNavPointsOfUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetNavPointsOfUser]
GO
CREATE PROCEDURE [dbo].[Public_GetNavPointsOfUser] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetLogonList]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetLogonList]
GO
CREATE PROCEDURE [dbo].[Public_GetLogonList] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetEMailAddressesOfAllSecurityAdmins]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetEMailAddressesOfAllSecurityAdmins]
GO
CREATE PROCEDURE [dbo].[Public_GetEMailAddressesOfAllSecurityAdmins] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetCurServerLogonList]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetCurServerLogonList]
GO
CREATE PROCEDURE [dbo].[Public_GetCurServerLogonList] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetCompleteUserInfo]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetCompleteUserInfo]
GO
CREATE PROCEDURE [dbo].[Public_GetCompleteUserInfo] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetCompleteName]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetCompleteName]
GO
CREATE PROCEDURE [dbo].[Public_GetCompleteName] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_GetCompleteAddresses]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_GetCompleteAddresses]
GO
CREATE PROCEDURE [dbo].[Public_GetCompleteAddresses] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Public_CreateUserAccount]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Public_CreateUserAccount]
GO
CREATE PROCEDURE [dbo].[Public_CreateUserAccount] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Int_LogAuthChanges]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Int_LogAuthChanges]
GO
CREATE PROCEDURE [dbo].[Int_LogAuthChanges] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Int_UpdateUserDetailDataWithProfileData]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[Int_UpdateUserDetailDataWithProfileData]
GO
CREATE PROCEDURE [dbo].[Int_UpdateUserDetailDataWithProfileData] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateUserPW]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateUserPW]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateUserPW] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateUserDetails]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateUserDetails]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateUserDetails] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateStatusLoginDisabled]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateStatusLoginDisabled]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateStatusLoginDisabled] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateServerGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateServerGroup]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateServerGroup] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateServer]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateServer]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateServer] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateApp]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateApp]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateApp] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_UpdateAccessLevel]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_UpdateAccessLevel]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_UpdateAccessLevel] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_SetScriptEngineActivation]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_SetScriptEngineActivation]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_SetScriptEngineActivation] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_SetAuthorizationInherition]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_SetAuthorizationInherition]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_SetAuthorizationInherition] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ResetLoginLockedTill]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_ResetLoginLockedTill]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_ResetLoginLockedTill] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_GetScriptEnginesOfServer]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_GetScriptEnginesOfServer]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_GetScriptEnginesOfServer] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_GetCompleteUserInfo]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_GetCompleteUserInfo]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_GetCompleteUserInfo] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteServerGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_DeleteServerGroup]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteServerGroup] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteServer]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_DeleteServer]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteServer] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteAccessLevel]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_DeleteAccessLevel]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteAccessLevel] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateUserAccount]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateUserAccount]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateUserAccount] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateServerGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateServerGroup]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateServerGroup] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateServer]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateServer]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateServer] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateMemberships]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateMemberships]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateMemberships] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateMasterServerNavPoints]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateMasterServerNavPoints]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateMasterServerNavPoints] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateGroup]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateGroup] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateApplicationRightsByUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateApplicationRightsByUser]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateApplicationRightsByUser] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateApplicationRightsByGroup]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateApplicationRightsByGroup]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateApplicationRightsByGroup] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateApplication]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateApplication]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateApplication] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateAdminServerNavPoints]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateAdminServerNavPoints]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateAdminServerNavPoints] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CreateAccessLevel]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CreateAccessLevel]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CreateAccessLevel] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_DeleteUser]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_DeleteUser]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_DeleteUser] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_CloneApplication]') and OBJECTPROPERTY(object_id, N'IsProcedure') = 1) drop procedure [dbo].[AdminPrivate_CloneApplication]
GO
CREATE PROCEDURE [dbo].[AdminPrivate_CloneApplication] 
AS 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO


/******************************************************
  Stored procedures  End
******************************************************/

/******************************************************
  Views  Begin
******************************************************/

----------------------------------------------------
-- [dbo].[Applications]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[Applications]
GO
CREATE VIEW dbo.Applications
AS
SELECT     dbo.Applications_CurrentAndInactiveOnes.*
FROM         dbo.Applications_CurrentAndInactiveOnes
WHERE     (AppDeleted = 0)
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO


----------------------------------------------------
-- [dbo].[System_WebAreasAuthorizedForSession]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[System_WebAreasAuthorizedForSession]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[System_WebAreasAuthorizedForSession]
GO
CREATE VIEW dbo.System_WebAreasAuthorizedForSession
AS
SELECT     dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.*
FROM         dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
WHERE     (Inactive = 0)
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Memberships]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Memberships]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Memberships]
GO
CREATE VIEW dbo.view_Memberships
AS
SELECT     dbo.Memberships.ID AS ID_Membership, dbo.Gruppen.ID AS ID_Group, dbo.Gruppen.Name, dbo.Gruppen.Description, dbo.Memberships.ReleasedOn,
Benutzer1.ID AS ID_ReleasedBy, Benutzer1.Vorname AS ReleasedByFirstName, Benutzer1.Nachname AS ReleasedByLastName,
dbo.Benutzer.ID AS ID_User, dbo.Benutzer.Loginname, dbo.Benutzer.Vorname, dbo.Benutzer.Nachname, dbo.Benutzer.LoginDisabled
FROM         dbo.Memberships LEFT OUTER JOIN
dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer1 ON dbo.Memberships.ReleasedBy = Benutzer1.ID RIGHT OUTER JOIN
dbo.Gruppen ON dbo.Memberships.ID_Group = dbo.Gruppen.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_ApplicationRights]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_ApplicationRights]
GO
CREATE VIEW dbo.view_ApplicationRights
AS
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END AS TitleAdminAreaDisplay, dbo.Applications.Level1Title, 
                      dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID AS ID_Application, 
                      Benutzer1.ID AS AppReleasedByID, Benutzer1.Vorname AS AppReleasedByVorname, Benutzer1.Nachname AS AppReleasedByNachname, 
                      Applications.ReleasedOn AS AppReleasedOn, ApplicationsRightsByGroup.ID AS ID_AppRight, NULL AS ID_User, NULL AS LoginDisabled, NULL 
                      AS Loginname, NULL AS Vorname, NULL AS Nachname, Gruppen.ID AS ID_Group, Gruppen.Name, Gruppen.Description, 1 AS ItemType, 
                      Applications.AppDisabled AS AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, NULL AS ThisAuthIsFromAppID, 
                      dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL, 
                      SystemApp AS SystemApp, SystemAppType AS SystemAppType, 0 AS DevelopmentTeamMember
FROM         dbo.Applications LEFT OUTER JOIN
                      dbo.ApplicationsRightsByGroup LEFT OUTER JOIN
                      dbo.Gruppen ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Gruppen.ID ON 
                      dbo.Applications.ID = dbo.ApplicationsRightsByGroup.ID_Application LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
UNION
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID AS ID_Application, 
                      Benutzer1.ID AS AppReleasedByID, Benutzer1.Vorname AS AppReleasedByVorname, Benutzer1.Nachname AS AppReleasedByNachname, 
                      Applications.ReleasedOn AS AppReleasedOn, ApplicationsRightsByGroup.ID AS ID_AppRight, NULL AS ID_User, NULL AS LoginDisabled, NULL 
                      AS Loginname, NULL AS Vorname, NULL AS Nachname, Gruppen.ID AS ID_Group, Gruppen.Name, Gruppen.Description, 1 AS ItemType, 
                      Applications.AppDisabled AS AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, Applications.AuthsAsAppID AS ThisAuthIsFromAppID, 
                      dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL, SystemApp, 
                      SystemAppType, 0 AS DevelopmentTeamMember
FROM         dbo.Applications LEFT OUTER JOIN
                      dbo.ApplicationsRightsByGroup LEFT OUTER JOIN
                      dbo.Gruppen ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Gruppen.ID ON 
                      dbo.Applications.AuthsAsAppID = dbo.ApplicationsRightsByGroup.ID_Application LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
UNION
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID, Benutzer1.ID, Benutzer1.Vorname, 
                      Benutzer1.Nachname, Applications.ReleasedOn, ApplicationsRightsByUser.ID, Benutzer.ID, Benutzer.LoginDisabled, Benutzer.Loginname, 
                      Benutzer.Vorname, Benutzer.Nachname, NULL, NULL, NULL, 2, Applications.AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, NULL 
                      AS ThisAuthIsFromAppID, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0)
FROM         dbo.Benutzer RIGHT OUTER JOIN
                      dbo.ApplicationsRightsByUser ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson RIGHT OUTER JOIN
                      dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.ID LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
UNION
SELECT     Applications.Title, Applications.TitleAdminArea, CASE WHEN IsNull(Applications.TitleAdminArea, '') 
                      = '' THEN Applications.Title ELSE Applications.TitleAdminArea END, dbo.Applications.Level1Title, dbo.Applications.Level2Title, 
                      dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
                      dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, 
                      dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
                      dbo.Applications.NavURL, dbo.Applications.Sort, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, 
                      dbo.Applications.NavToolTipText, Applications.LocationID, Applications.LanguageID, Applications.ID, Benutzer1.ID, Benutzer1.Vorname, 
                      Benutzer1.Nachname, Applications.ReleasedOn, ApplicationsRightsByUser.ID, Benutzer.ID, Benutzer.LoginDisabled, Benutzer.Loginname, 
                      Benutzer.Vorname, Benutzer.Nachname, NULL, NULL, NULL, 2, Applications.AppDisabled, Applications.AuthsAsAppID AS AuthsAsAppID, 
                      Applications.AuthsAsAppID AS ThisAuthIsFromAppID, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, 
                      dbo.Applications.AddLanguageID2URL, SystemApp, SystemAppType, IsNull(dbo.ApplicationsRightsByUser.DevelopmentTeamMember, 0)
FROM         dbo.Benutzer RIGHT OUTER JOIN
                      dbo.ApplicationsRightsByUser ON dbo.Benutzer.ID = dbo.ApplicationsRightsByUser.ID_GroupOrPerson RIGHT OUTER JOIN
                      dbo.Applications ON dbo.ApplicationsRightsByUser.ID_Application = dbo.Applications.AuthsAsAppID LEFT OUTER JOIN
                      dbo.Benutzer Benutzer1 ON dbo.Applications.ReleasedBy = Benutzer1.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Applications]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Applications]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Applications]
GO
CREATE VIEW dbo.view_Applications
AS
SELECT     dbo.Applications.ID, dbo.Applications.Title, dbo.Applications.ReleasedOn, dbo.Applications.ReleasedBy AS ReleasedByID,
Benutzer_2.Loginname AS ReleasedByLoginname, Benutzer_2.Vorname AS ReleasedByFirstName, Benutzer_2.Nachname AS ReleasedByLastName,
dbo.Applications.LocationID, dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.NavURL,
dbo.Applications.NavFrame, dbo.Applications.LanguageID, dbo.Applications.SystemApp, dbo.Applications.ModifiedOn,
dbo.Applications.ModifiedBy AS ModifiedByID, Benutzer_1.Loginname AS ModifiedByLoginname, Benutzer_1.Vorname AS ModifiedByFirstName,
Benutzer_1.Nachname AS ModifiedByLastName, dbo.Applications.AppDisabled, dbo.Applications.AuthsAsAppID, dbo.Applications.NavTooltipText,
dbo.Applications.IsNew, dbo.Applications.IsUpdated, dbo.Applications.ResetIsNewUpdatedStatusOn, dbo.Applications.Sort,
dbo.Applications.TitleAdminArea, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick,
dbo.Applications.AddLanguageID2URL, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title,
dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded,
dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded,
dbo.Applications.SystemAppType, dbo.Applications.Remarks
FROM         dbo.Applications LEFT OUTER JOIN
dbo.Benutzer Benutzer_1 ON dbo.Applications.ModifiedBy = Benutzer_1.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer_2 ON dbo.Applications.ReleasedBy = Benutzer_2.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_eMailAccounts_of_Groups]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_eMailAccounts_of_Groups]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_eMailAccounts_of_Groups]
GO
CREATE VIEW dbo.view_eMailAccounts_of_Groups
AS
SELECT DISTINCT dbo.Memberships.ID_Group, dbo.Benutzer.[E-MAIL]
FROM         dbo.Memberships LEFT OUTER JOIN
dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Groups]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Groups]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Groups]
GO
CREATE VIEW dbo.view_Groups
AS
SELECT     dbo.Gruppen.ID, dbo.Gruppen.Name, dbo.Gruppen.Description, dbo.Gruppen.ReleasedOn, dbo.Gruppen.ReleasedBy AS ReleasedByID,
Benutzer_1.Loginname AS ReleasedByLoginname, Benutzer_1.Vorname AS ReleasedByFirstName, Benutzer_1.Nachname AS ReleasedByLastName,
dbo.Gruppen.SystemGroup, dbo.Gruppen.ModifiedOn, dbo.Gruppen.ModifiedBy AS ModifiedByID, Benutzer_1.Loginname AS ModifiedByLoginname,
Benutzer_1.Vorname AS ModifiedByFirstName, Benutzer_1.Nachname AS ModifiedByLastName
FROM         dbo.Gruppen LEFT OUTER JOIN
dbo.Benutzer Benutzer_1 ON dbo.Gruppen.ModifiedBy = Benutzer_1.ID LEFT OUTER JOIN
dbo.Benutzer Benutzer_2 ON dbo.Gruppen.ReleasedBy = Benutzer_2.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Languages]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Languages]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Languages]
GO
CREATE VIEW dbo.view_Languages
AS
SELECT     dbo.Languages.*
FROM         dbo.Languages
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Base4Analysis]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Base4Analysis]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Base4Analysis]
GO
CREATE VIEW dbo.view_Log_Base4Analysis
AS
SELECT     dbo.[Log].*, CASE WHEN dbo.Applications.TitleAdminArea IS NULL OR
                      dbo.Applications.TitleAdminArea = '' THEN dbo.Applications.Title ELSE dbo.Applications.TitleAdminArea END AS Application, dbo.System_Servers.ServerGroup
FROM         (dbo.[Log] LEFT OUTER JOIN
                      dbo.Applications ON dbo.[Log].ApplicationID = dbo.Applications.ID) LEFT OUTER JOIN System_Servers ON Log.ServerIP = System_Servers.IP
WHERE     (NOT (dbo.Applications.SystemApp = 1)) AND (dbo.[Log].UserID NOT IN
                          (SELECT     id_user
                            FROM          memberships
                            WHERE      dbo.Memberships.ID_Group = 6)) AND (NOT (dbo.[Log].ApplicationID IN
                          (SELECT     id
                            FROM          applications
                            WHERE      systemapp = 1)))
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_AccessStatistics_Complete - Pre1]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_AccessStatistics_Complete - Pre1]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_AccessStatistics_Complete - Pre1]
GO
CREATE VIEW dbo.[view_Log_AccessStatistics_Complete - Pre1]
AS
SELECT     ServerGroup, Application, COUNT(1) AS [Count], LoginDate, UserID, YEAR(LoginDate) AS LoginYear, MONTH(LoginDate) AS LoginMonth, DAY(LoginDate) 
                      AS LoginDay
FROM         dbo.view_Log_Base4Analysis
WHERE     (ConflictType = 0)
GROUP BY Application, ServerGroup, LoginDate, UserID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_Application_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_Application_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_Application_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_Application_Complete
AS
SELECT     ApplicationID, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis
WHERE     (ConflictType = 0)
GROUP BY ApplicationID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_Application_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_Application_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_Application_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_Application_CurrentMonth
AS
SELECT     ApplicationID, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis
WHERE     (ConflictType = 0) AND (LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY ApplicationID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerApplication_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerApplication_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerApplication_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerApplication_Complete
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, Application, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis 
WHERE     (ConflictType = 0)
GROUP BY Application, dbo.view_Log_Base4Analysis.ServerGroup
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerApplication_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerApplication_CurrentMonth
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, Application, COUNT(1) AS [Count]
FROM         dbo.view_Log_Base4Analysis 
WHERE     (ConflictType = 0) AND (LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY Application, dbo.view_Log_Base4Analysis.ServerGroup
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_User_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_User_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_User_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_User_Complete
AS
SELECT     dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname AS Name,
dbo.Benutzer.[E-MAIL], COUNT(1) AS [Count], dbo.Benutzer.Company
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
dbo.Benutzer ON dbo.view_Log_Base4Analysis.UserID = dbo.Benutzer.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0)
GROUP BY dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname,
dbo.Benutzer.[E-MAIL], dbo.Benutzer.Company
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_User_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_User_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_User_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_User_CurrentMonth
AS
SELECT     dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname AS Name,
dbo.Benutzer.[E-MAIL], COUNT(1) AS [Count], dbo.Benutzer.Company
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
dbo.Benutzer ON dbo.view_Log_Base4Analysis.UserID = dbo.Benutzer.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0) AND (dbo.view_Log_Base4Analysis.LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY dbo.view_Log_Base4Analysis.UserID, ISNULL(dbo.Benutzer.Namenszusatz, '')
+ SPACE({ fn LENGTH(SUBSTRING(ISNULL(dbo.Benutzer.Namenszusatz, ''), 1, 1)) }) + dbo.Benutzer.Nachname + ', ' + dbo.Benutzer.Vorname,
dbo.Benutzer.[E-MAIL], dbo.Benutzer.Company
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_UserList]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_UserList]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_UserList]
GO
CREATE VIEW dbo.view_UserList
AS
SELECT     ID, Loginname, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS Name,
[E-MAIL]
FROM         dbo.Benutzer
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_AccessStatistics_Complete - Pre2]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_AccessStatistics_Complete - Pre2]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_AccessStatistics_Complete - Pre2]
GO
CREATE VIEW dbo.[view_Log_AccessStatistics_Complete - Pre2]
AS
SELECT     Min(dbo.system_ServerGroups.ServerGroup) as ServerGroup_Title, Application, SUM([Count]) AS Hits, CAST(LoginYear AS varchar(4)) 
                      + '-' + CASE WHEN LoginMonth < 10 THEN '0' ELSE '' END + CAST(LoginMonth AS varchar(4)) AS LoginDate
FROM         dbo.[view_Log_AccessStatistics_Complete - Pre1] left join dbo.system_servergroups on dbo.[view_Log_AccessStatistics_Complete - Pre1].servergroup = dbo.system_servergroups.id
GROUP BY dbo.[view_Log_AccessStatistics_Complete - Pre1].ServerGroup, Application, CAST(LoginYear AS varchar(4)) + '-' + CASE WHEN LoginMonth < 10 THEN '0' ELSE '' END + CAST(LoginMonth AS varchar(4))
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerUserApplication_Complete]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerUserApplication_Complete
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, dbo.view_Log_Base4Analysis.UserID, dbo.view_UserList.Name, dbo.view_Log_Base4Analysis.Application, 
                      COUNT(1) AS [Count], MAX(dbo.view_Log_Base4Analysis.LoginDate) AS LastAccessDate
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
                      dbo.view_UserList ON dbo.view_Log_Base4Analysis.UserID = dbo.view_UserList.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0)
GROUP BY dbo.view_Log_Base4Analysis.ServerGroup, dbo.view_Log_Base4Analysis.Application, dbo.view_Log_Base4Analysis.UserID, dbo.view_UserList.Name
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Log_Statistics_By_ServerUserApplication_CurrentMonth]
GO
CREATE VIEW dbo.view_Log_Statistics_By_ServerUserApplication_CurrentMonth
AS
SELECT     dbo.view_Log_Base4Analysis.ServerGroup As ServerGroupID, dbo.view_Log_Base4Analysis.UserID, dbo.view_UserList.Name, dbo.view_Log_Base4Analysis.Application, 
                      COUNT(1) AS [Count], MAX(dbo.view_Log_Base4Analysis.LoginDate) AS LastAccessDate
FROM         dbo.view_Log_Base4Analysis LEFT OUTER JOIN
                      dbo.view_UserList ON dbo.view_Log_Base4Analysis.UserID = dbo.view_UserList.ID
WHERE     (dbo.view_Log_Base4Analysis.ConflictType = 0) AND (dbo.view_Log_Base4Analysis.LoginDate > GETDATE() - DAY(GETDATE()))
GROUP BY dbo.view_Log_Base4Analysis.ServerGroup, dbo.view_Log_Base4Analysis.Application, dbo.view_UserList.Name, dbo.view_Log_Base4Analysis.UserID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_User_Statistics_LastLogonDates]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_User_Statistics_LastLogonDates]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_User_Statistics_LastLogonDates]
GO
CREATE VIEW dbo.view_User_Statistics_LastLogonDates
AS
SELECT     dbo.Benutzer.LastLoginOn, dbo.view_UserList.ID, dbo.view_UserList.Loginname, dbo.view_UserList.Name, dbo.view_UserList.[E-MAIL]
FROM         dbo.view_UserList INNER JOIN
dbo.Benutzer ON dbo.view_UserList.ID = dbo.Benutzer.ID
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO


----------------------------------------------------
-- [dbo].[AdminPrivate_ServerRelations]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ServerRelations]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[AdminPrivate_ServerRelations]
GO
CREATE VIEW dbo.AdminPrivate_ServerRelations
AS
SELECT     dbo.System_ServerGroups.*, Gruppen_1.ID AS Group_Public_ID, Gruppen_1.Name AS Group_Public_Name, Gruppen_2.ID AS Group_Anonymous_ID,
Gruppen_2.Name AS Group_Anonymous_Name, System_Servers_1.ID AS UserAdminServer_ID,
System_Servers_1.Enabled AS UserAdminServer_Enabled, System_Servers_1.IP AS UserAdminServer_IP,
System_Servers_1.ServerDescription AS UserAdminServer_ServerDescription,
System_Servers_1.ServerProtocol AS UserAdminServer_ServerProtocol, System_Servers_1.ServerName AS UserAdminServer_ServerName,
System_Servers_1.ServerPort AS UserAdminServer_ServerPort, System_Servers_3.ID AS MasterServer_ID,
System_Servers_3.Enabled AS MasterServer_Enabled, System_Servers_3.IP AS MasterServer_IP,
System_Servers_3.ServerDescription AS MasterServer_ServerDescription, System_Servers_3.ServerProtocol AS MasterServer_ServerProtocol,
System_Servers_3.ServerName AS MasterServer_ServerName, System_Servers_3.ServerPort AS MasterServer_ServerPort,
System_Servers_2.ID AS MemberServer_ID, System_Servers_2.Enabled AS MemberServer_Enabled, System_Servers_2.IP AS MemberServer_IP,
System_Servers_2.ServerDescription AS MemberServer_ServerDescription, System_Servers_2.ServerProtocol AS MemberServer_ServerProtocol,
System_Servers_2.ServerName AS MemberServer_ServerName, System_Servers_2.ServerPort AS MemberServer_ServerPort
FROM         dbo.System_ServerGroups INNER JOIN
dbo.System_Servers System_Servers_1 ON dbo.System_ServerGroups.UserAdminServer = System_Servers_1.ID INNER JOIN
dbo.Gruppen Gruppen_1 ON dbo.System_ServerGroups.ID_Group_Public = Gruppen_1.ID INNER JOIN
dbo.Gruppen Gruppen_2 ON dbo.System_ServerGroups.ID_Group_Anonymous = Gruppen_2.ID INNER JOIN
dbo.System_Servers System_Servers_3 ON dbo.System_ServerGroups.MasterServer = System_Servers_3.ID INNER JOIN
dbo.System_Servers System_Servers_2 ON dbo.System_ServerGroups.ID = System_Servers_2.ServerGroup
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[AdminPrivate_ServerGroupAccessLevels]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[AdminPrivate_ServerGroupAccessLevels]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[AdminPrivate_ServerGroupAccessLevels]
GO
CREATE VIEW dbo.AdminPrivate_ServerGroupAccessLevels
AS
SELECT     dbo.System_ServerGroupsAndTheirUserAccessLevels.ID, dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup,
dbo.System_ServerGroupsAndTheirUserAccessLevels.Remarks, dbo.System_AccessLevels.ID AS AccessLevels_ID,
dbo.System_AccessLevels.Title AS AccessLevels_Title, dbo.System_AccessLevels.Remarks AS AccessLevels_Remarks
FROM         dbo.System_AccessLevels INNER JOIN
dbo.System_ServerGroupsAndTheirUserAccessLevels ON
dbo.System_AccessLevels.ID = dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- [dbo].[view_Memberships_CummulatedWithAnonymous]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_Memberships_CummulatedWithAnonymous]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_Memberships_CummulatedWithAnonymous]
GO
create view dbo.[view_Memberships_CummulatedWithAnonymous]
as
SELECT     ID_Group, ID_User
FROM         dbo.Memberships
UNION
SELECT     58, id
FROM         dbo.benutzer
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

----------------------------------------------------
-- dbo.[view_ApplicationRights_CommulatedPerUser]
----------------------------------------------------
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from sys.objects where object_id = object_id(N'[dbo].[view_ApplicationRights_CommulatedPerUser]') and OBJECTPROPERTY(object_id, N'IsView') = 1) drop view [dbo].[view_ApplicationRights_CommulatedPerUser]
GO
create view dbo.[view_ApplicationRights_CommulatedPerUser]
as
SELECT     dbo.view_ApplicationRights.Title, dbo.view_ApplicationRights.ID_Application, ISNULL(dbo.view_ApplicationRights.ID_User, 
                      dbo.view_Memberships_CummulatedWithAnonymous.ID_User) AS ID_User
FROM         dbo.view_ApplicationRights LEFT OUTER JOIN
                      dbo.view_Memberships_CummulatedWithAnonymous ON 
                      dbo.view_ApplicationRights.ID_Group = dbo.view_Memberships_CummulatedWithAnonymous.ID_Group
WHERE     (ISNULL(dbo.view_ApplicationRights.ID_User, dbo.view_Memberships_CummulatedWithAnonymous.ID_User) IS NOT NULL)
UNION
SELECT     Applications.Title, Applications.ID AS ID_Application, dbo.Memberships.ID_User
FROM         dbo.Applications CROSS JOIN
                      dbo.Memberships
WHERE     dbo.Memberships.ID_Group = 6
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

/******************************************************
  Views  End
******************************************************/

/******************************************************
  Stored procedures  Begin
******************************************************/

----------------------------------------------------
-- [dbo].[Public_ServerDebug]
----------------------------------------------------

ALTER PROCEDURE dbo.Public_ServerDebug
(
	@ServerIP varchar(32),
	@RemoteIP varchar(32)
)

AS

-- Deklaration Variablen/Konstanten
DECLARE @WebApplication varchar(1024)
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime

DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Registered_ScriptEngine_SessionID varchar(512)
DECLARE @RequestedServerID int
DECLARE @WebAppID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @WebApplication = 'Public'

SET NOCOUNT ON

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return

	END
----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID)))

------------------------------
-- UserLoginValidierung --
------------------------------

-- ReAuthentifizierung?
	-- Does the user has got authorization?
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ApplicationsRightsByGroup.ID_GroupOrPerson FROM Memberships RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID) AND (ApplicationsRightsByGroup.ID_GroupOrPerson = @PublicGroupID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships.ID_User FROM Memberships RIGHT OUTER JOIN ApplicationsRightsByGroup ON Memberships.ID_Group = ApplicationsRightsByGroup.ID_GroupOrPerson RIGHT OUTER JOIN Applications ON ApplicationsRightsByGroup.ID_Application = Applications.ID WHERE (((Memberships.ID_User = @CurUserID) AND (Applications.Title = @WebApplication))) AND Applications.LocationID = @LocationID)
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6))
		If NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
			SET @MyResult = 1 -- Zugriff gewährt
		Else
			SET @MyResult = 2 -- kein Zugriff auf aktuelles Dokument

IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		SET NOCOUNT ON
	END
Else -- @MyResult = 0 Or @MyResult = 2
	-- Login failed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2	
		SET NOCOUNT ON
	END
GO

----------------------------------------------------
-- dbo.AdminPrivate_CloneApplication
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CloneApplication 
	@ReleasedByUserID int,
	@AppID int,
	@CloneType int

AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		SET NOCOUNT ON

		If @CloneType = 1 -- copy application and authorizations
			BEGIN
				-- Add new application
				INSERT INTO dbo.Applications
				                      (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, 
				                      LocationID, LanguageID, SystemApp, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, @ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, SystemApp, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL
				FROM         dbo.Applications
				WHERE     (ID = @AppID)

				-- Add Group Authorizations
				INSERT INTO dbo.ApplicationsRightsByGroup
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application)
				SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @AppID AS ID_Application
				FROM         dbo.ApplicationsRightsByGroup
				WHERE     (ID_Application = @AppID)

				-- Add User Authorizations
				INSERT INTO dbo.ApplicationsRightsByUser
				                      (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application)
				
SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @AppID AS ID_Application
				FROM         dbo.ApplicationsRightsByUser
				WHERE     (ID_Application = @AppID)

			END
		Else -- copy application and inherit authorizations from cloned application
			BEGIN
				INSERT INTO dbo.Applications
				                      (Title, TitleAdminArea, ReleasedOn, ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, IsNew, IsUpdated, 
				                      LocationID, LanguageID, SystemApp, ModifiedOn, ModifiedBy, AppDisabled, AuthsAsAppID, Sort, ResetIsNewUpdatedStatusOn, AppDeleted, 
				                      OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL)
				SELECT     Title, 'Disabled clone of ' + Case When IsNull(TitleAdminArea, '') = '' Then Title Else TitleAdminArea End, getdate() as ReleasedOn, 
						@ReleasedByUserID AS ReleasedBy, Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title, Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded, Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded, Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded, NavURL, NavFrame, NavTooltipText, 
				                      IsNew, IsUpdated, LocationID, LanguageID, SystemApp, getdate() as ModifiedOn, @ReleasedByUserID AS ModifiedBy, 1 as AppDisabled, @AppID As AuthsAsAppID, Sort, 
				                      ResetIsNewUpdatedStatusOn, AppDeleted, OnMouseOver, OnMouseOut, OnClick, AddLanguageID2URL
				FROM         dbo.Applications
				WHERE    (ID = @AppID)
			END
		SET NOCOUNT OFF

		SELECT Result = @@IDENTITY
		
	END
Else
	
	SELECT Result = 0

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateAccessLevel 
	@ReleasedByUserID int,
	@Title nvarchar(50)

AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
		BEGIN
		SET NOCOUNT ON
		INSERT INTO dbo.System_AccessLevels (Title, ReleasedBy, ModifiedBy) VALUES (@Title, @ReleasedByUserID, @ReleasedByUserID)
		SET NOCOUNT OFF

		SELECT Result = @@IDENTITY
		
	END
Else
	
	SELECT Result = 0


GO

---------------------------------------------------
-- dbo.AdminPrivate_CreateAdminServerNavPoints
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateAdminServerNavPoints
	(
		@NewServerID int,
		@OldServerID int,
		@ModifiedBy int
	)

AS

If @NewServerID = @OldServerID
	Return

---------------------------------
-- Admin server nav items --
---------------------------------
DECLARE @AppID_Applications int
DECLARE @AppID_Authorizations int
DECLARE @AppID_Groups int
DECLARE @AppID_Memberships int
DECLARE @AppID_Users int
DECLARE @AppID_ResetPassword int
DECLARE @AppID_ServerSetup int
DECLARE @AppID_AccessLevels int
DECLARE @AppID_Trouble int
DECLARE @AppID_NavPreview int
DECLARE @OldServerIsFurthermoreAdminServer bit

SET NOCOUNT ON

-- An admin server could be admin server for several server groups, 
-- that's why we have to check if we are allowed 
-- to delete the nav point from the old server
SELECT @OldServerIsFurthermoreAdminServer = (SELECT TOP 1 CASE WHEN ID Is Null Then 0 Else 1 END FROM dbo.System_ServerGroups WHERE UserAdminServer = @OldServerID)
If @OldServerIsFurthermoreAdminServer Is Null
	SELECT @OldServerIsFurthermoreAdminServer = 0


If @OldServerIsFurthermoreAdminServer = 0
	BEGIN
	-- okay, we can delete any existing nav points for administration purposes
	DELETE dbo.ApplicationsRightsByGroup
		FROM dbo.ApplicationsRightsByGroup INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
		WHERE Applications_CurrentAndInactiveOnes.SystemApp <> 0 And Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (Applications_CurrentAndInactiveOnes.LocationID = @OldServerID OR Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	UPDATE Applications_CurrentAndInactiveOnes
		SET AppDeleted = 1
		FROM dbo.Applications_CurrentAndInactiveOnes 
		WHERE dbo.Applications_CurrentAndInactiveOnes.SystemApp <> 0 And dbo.Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (dbo.Applications_CurrentAndInactiveOnes.LocationID = @OldServerID OR dbo.Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	END
Else
	BEGIN
	-- we have to keep the nav items for another server group which already uses our old server as admin server
	DELETE dbo.ApplicationsRightsByGroup
		FROM dbo.ApplicationsRightsByGroup INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
		WHERE Applications_CurrentAndInactiveOnes.SystemApp <> 0 And Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	UPDATE Applications_CurrentAndInactiveOnes
		SET AppDeleted = 1
		FROM dbo.Applications_CurrentAndInactiveOnes 
		WHERE dbo.Applications_CurrentAndInactiveOnes.SystemApp <> 0 And dbo.Applications_CurrentAndInactiveOnes.SystemAppType = 2 And (dbo.Applications_CurrentAndInactiveOnes.LocationID = @NewServerID)
	END


INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications','',getdate(),@ModifiedBy,'Web Administration','Setup','Applications',NULL,NULL,NULL,'[ADMINURL]apps.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Applications = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations','',getdate(),@ModifiedBy,'Web Administration','User Administration','Authorizations',NULL,NULL,NULL,'[ADMINURL]apprights.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Authorizations = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups','',getdate(),@ModifiedBy,'Web Administration','User Administration','Groups',NULL,NULL,NULL,'[ADMINURL]groups.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Groups = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships','',getdate(),@ModifiedBy,'Web Administration','User Administration','Group memberships',NULL,NULL,NULL,'[ADMINURL]memberships.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Memberships = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users','',getdate(),@ModifiedBy,'Web Administration','User Administration','Users',NULL,NULL,NULL,'[ADMINURL]users.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Users = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','Reset user password',NULL,NULL,NULL,'[ADMINURL]users_resetpw.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_ResetPassword = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview','',getdate(),@ModifiedBy,'Web Administration','Navigation preview',NULL,NULL,NULL,NULL,'[MASTERSERVER]/sysdata/users_navbar_preview.asp','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_NavPreview = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users','',getdate(),@ModifiedBy,'Web Administration','Trouble Center','User hotline support',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.asp','',NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_Trouble= @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Server administration',NULL,NULL,NULL,'[ADMINURL]servers.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_ServerSetup = @@IDENTITY
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','About WebManager',NULL,NULL,NULL,'[ADMINURL]about.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,'Web Administration','Setup','Access levels',NULL,NULL,NULL,'[ADMINURL]accesslevels.asp',NULL,NULL,0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,NULL,1000000,NULL,0,NULL,NULL,NULL,1,2)
SELECT @AppID_AccessLevels = @@IDENTITY

-- Copies in German language
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Authorizations',NULL,getdate(),@ModifiedBy,'Web-Administration','Benutzer-Verwaltung','Berechtigungen',NULL,NULL,NULL,'[ADMINURL]apprights.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Authorizations,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Groups',NULL,getdate(),@ModifiedBy,'Web-Administration','Benutzer-Verwaltung','Gruppen',NULL,NULL,NULL,'[ADMINURL]groups.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Groups,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Memberships',NULL,getdate(),@ModifiedBy,'Web-Administration','Benutzer-Verwaltung','Mitgliedschaften',NULL,NULL,NULL,'[ADMINURL]memberships.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Memberships,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,'Web-Administration','Benutzer-Verwaltung','Benutzer',NULL,NULL,NULL,'[ADMINURL]users.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users',NULL,getdate(),@ModifiedBy,'Web-Administration','Trouble Center','Benutzer Hotline Support',NULL,NULL,NULL,'[ADMINURL]users_hotline_support.asp','',NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Users,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Users - Reset password',NULL,getdate(),@ModifiedBy,'Web-Administration','Trouble Center','Passwörter zurücksetzen',NULL,NULL,NULL,'[ADMINURL]users_resetpw.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_ResetPassword,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - NavPreview',NULL,getdate(),@ModifiedBy,'Web-Administration','Navigations-Vorschau',NULL,NULL,NULL,NULL,'[MASTERSERVER]/sysdata/users_navbar_preview.asp','',NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_NavPreview,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Server-Verwaltung',NULL,NULL,NULL,'[ADMINURL]servers.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - ServerSetup',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Über WebManager',NULL,NULL,NULL,'[ADMINURL]about.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_ServerSetup ,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - Applications',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Anwendungen',NULL,NULL,NULL,'[ADMINURL]apps.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - User Administration - AccessLevels',NULL,getdate(),@ModifiedBy,'Web-Administration','Setup','Zugriffs-Levels',NULL,NULL,NULL,'[ADMINURL]accesslevels.asp',NULL,NULL,0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Applications,1000000,NULL,0,NULL,NULL,NULL,1,2)

INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Trouble,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Authorizations ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Groups ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Memberships ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_ResetPassword ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_NavPreview ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Users ,'7',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_AccessLevels ,'6',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_ServerSetup ,'6',getdate(),@ModifiedBy)
INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Applications ,'6',getdate(),@ModifiedBy)

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplication
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_CreateApplication 
	@ReleasedByUserID int,
	@Title varchar(255)

AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		SET NOCOUNT ON
		INSERT INTO dbo.Applications (Title, ReleasedBy, ModifiedBy, LanguageID, LocationID) VALUES (@Title, @ReleasedByUserID, @ReleasedByUserID, 0, 0)
		SET NOCOUNT OFF

		SELECT Result = @@IDENTITY
		
	END
Else
	
	SELECT Result = 0

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByGroup 
	@ReleasedByUserID int,
	@AppID int,
	@GroupID int

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application ID
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END


SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 1
	RETURN 1
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)

-- Password validation and update
If @CurUserID Is Not Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application, ID_GroupOrPerson, ReleasedBy) VALUES (@AppID, @GroupID, @ReleasedByUserID)
		EXEC Int_LogAuthChanges @ReleasedByUserID, @GroupID, @AppID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
	END
Else
	-- Rückgabewert
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 0
	END

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateApplicationRightsByUser 
	@ReleasedByUserID int,
	@AppID int,
	@UserID int,
	@IsDevelopmentTeamMember bit

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @AppIsOnSAP bit
DECLARE @SAPFlagName varchar(50)
DECLARE @SAPFlagValue nvarchar(255)
DECLARE @AppValidator int

SET NOCOUNT ON

SELECT @AppValidator = ID FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppValidator Is Null
	-- Rückgabewert: Invalid application ID
	BEGIN
	SET NOCOUNT OFF
	SELECT Result = 3
	RETURN 3
	END

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Null
	-- Rückgabewert
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @UserID)
If @CurUserID Is Null
	-- Rückgabewert
	BEGIN
		SET NOCOUNT OFF
		SELECT Result = 0
		Return 0
	END

SELECT @AppIsOnSAP = Case When NavURL like '[[]SAP%|2|&%' And LocationID < 0 Then 1 Else 0 End FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
If @AppIsOnSAP = 1
	-- Rückgabewert: Authorisation nur möglich, wenn benötigte Attribute für SAP-SSO-Anwendungen gepflegt sind
	BEGIN
	SELECT @SAPFlagName = substring(navurl, charindex('|2|&', NavUrl)+4, charindex('|3|', NavUrl) - (charindex('|2|&', NavUrl)+4)) FROM Applications_CurrentAndInactiveOnes WHERE ID = @AppID
	SELECT @SAPFlagValue = [Value] FROM Log_Users WHERE ID_User = @CurUserID AND Type = @SAPFlagName
	If @SAPFlagValue Is Null
		BEGIN
		SET NOCOUNT OFF
		SELECT Result = 2, @SAPFlagName as SAPFlagName
		RETURN 2
		END
	END

-- User validation and update
If @CurUserID Is Not Null
	-- Validation successfull, authorization will be saved now
	BEGIN
		-- Record update
		INSERT INTO dbo.ApplicationsRightsByUser 
			(ID_Application, ID_GroupOrPerson, ReleasedBy, DevelopmentTeamMember) 
		VALUES 
			(@AppID, @UserID, @ReleasedByUserID, @IsDevelopmentTeamMember)
		EXEC Int_LogAuthChanges @UserID, Null, @AppID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1
		Return -1
	END

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateGroup
----------------------------------------------------
ALTER PROCEDURE AdminPrivate_CreateGroup 
	@ReleasedByUserID int,
	@Name nvarchar(50),
	@Description nvarchar(1024)

AS
DECLARE @CurUserID int
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null
	
	BEGIN
		
		SELECT Result = -1
		
		INSERT INTO dbo.Gruppen (Name, Description, ReleasedBy, ModifiedBy) VALUES (@Name, @Description, @ReleasedByUserID, @ReleasedByUserID)
	END
Else
	
	SELECT Result = 0




GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateMasterServerNavPoints
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMasterServerNavPoints
	(
		@NewServerID int,
		@OldServerID int,
		@ModifiedBy int
	)

AS

If @NewServerID = @OldServerID
	Return

SET NOCOUNT ON
----------------------------------
-- Master server nav items --
----------------------------------
DECLARE @AppID_Login int

DELETE dbo.ApplicationsRightsByGroup
FROM dbo.ApplicationsRightsByGroup INNER JOIN dbo.Applications_CurrentAndInactiveOnes ON dbo.ApplicationsRightsByGroup.ID_Application = dbo.Applications_CurrentAndInactiveOnes.ID
WHERE SystemApp <> 0 And SystemAppType = 1 And (LocationID = @OldServerID OR LocationID = @NewServerID)

UPDATE dbo.Applications_CurrentAndInactiveOnes
SET AppDeleted = 1
FROM dbo.Applications_CurrentAndInactiveOnes 
WHERE SystemApp <> 0 And SystemAppType = 1 And (LocationID = @OldServerID OR LocationID = @NewServerID)

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Login','System - Login - LoginForm',getdate(),@ModifiedBy,'Anmeldung','Login',NULL,NULL,NULL,NULL,'[LOGONPAGE]',NULL,'Nach Anmeldung stehen Ihnen zusätzliche Möglichkeiten auf dieser WebSite zur Verfügung.',0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,NULL,9000000,NULL,0,NULL,NULL,NULL,1,1)
SELECT @AppID_Login = @@IDENTITY

INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Login','System - Login - PasswordRecovery',getdate(),@ModifiedBy,'Anmeldung','Passwort vergessen?',NULL,NULL,NULL,NULL,'[MASTERSERVER]/system/sendpw.asp',NULL,'Wenn Ihnen Ihr Passwort abhanden kommen sollte, sind Sie hier richtig!',0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Login,9000400,NULL,0,NULL,NULL,NULL,1,1)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Login','System - Login - NewUser',getdate(),@ModifiedBy,'Anmeldung','Neues Konto anlegen',NULL,NULL,NULL,NULL,'[MASTERSERVER]/sysdata/createaccount.asp',NULL,'Erstellen Sie hier Ihr Benutzerkonto, um zusätzliche Dienste und Leistungen in Anspruch nehmen zu können.',0,0,@NewServerID,'2',1,getdate(),@ModifiedBy,0,@AppID_Login,9000200,NULL,0,NULL,NULL,NULL,1,1)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Login','System - Login - LoginForm',getdate(),@ModifiedBy,'Login','Logon',NULL,NULL,NULL,NULL,'[LOGONPAGE]',NULL,'Additional features will be available after logon.',0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,@AppID_Login,9000000,NULL,0,NULL,NULL,NULL,1,1)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Login','System - Login - PasswordRecovery',getdate(),@ModifiedBy,'Login','Password forgotten?',NULL,NULL,NULL,NULL,'[MASTERSERVER]/system/sendpw.asp',NULL,'If you have lost your password you can recover it by e-mail.',0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,@AppID_Login,9000400,NULL,0,NULL,NULL,NULL,1,1)
INSERT INTO dbo.Applications_CurrentAndInactiveOnes (Title,TitleAdminArea,ReleasedOn,ReleasedBy,Level1Title,Level2Title,Level3Title,Level4Title,Level5Title,Level6Title,NavURL,NavFrame,NavTooltipText,IsNew,IsUpdated,LocationID,LanguageID,SystemApp,ModifiedOn,ModifiedBy,AppDisabled,AuthsAsAppID,Sort,ResetIsNewUpdatedStatusOn,AppDeleted,OnMouseOver,OnMouseOut,OnClick,AddLanguageID2URL,SystemAppType) 
VALUES('System - Login','System - Login - NewUser',getdate(),@ModifiedBy,'Login','Create new user account',NULL,NULL,NULL,NULL,'[MASTERSERVER]/sysdata/createaccount.asp',NULL,'You can create a new user account if you would like to receive additional services.',0,0,@NewServerID,'1',1,getdate(),@ModifiedBy,0,@AppID_Login,9000200,NULL,0,NULL,NULL,NULL,1,1)

INSERT INTO dbo.ApplicationsRightsByGroup (ID_Application,ID_GroupOrPerson,ReleasedOn,ReleasedBy) VALUES(@AppID_Login,'58',getdate(),@ModifiedBy)

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateMemberships
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateMemberships 
	@ReleasedByUserID int,
	@GroupID int,
	@UserID int

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
-- Password validation and update
If @CurUserID Is Not Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Record update
		INSERT INTO dbo.Memberships (ID_Group, ID_User, ReleasedBy) VALUES (@GroupID, @UserID, @ReleasedByUserID)
	END
Else
	-- Rückgabewert
	SELECT Result = 0



GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateServer
	(
		@ServerIP varchar(32),
		@ServerGroup int
	)

AS

declare @NewServerID int

SET NOCOUNT ON

-- Create server
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, @ServerIP AS IP, 'Secured server' AS ServerDescription, @ServerGroup AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout

-- Get new server ID
SELECT @NewServerID = @@IDENTITY

-- Check script engines
EXEC AdminPrivate_SetScriptEngineActivation 0, @NewServerID, 0, 1

-- Return new server ID
SET NOCOUNT OFF
SELECT     @NewServerID
RETURN     @NewServerID

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateServerGroup
(
@GroupName nvarchar(255),
@email_Developer nvarchar(255),
@UserID_Creator int
)

AS 

DECLARE @ID_ServerGroup int
DECLARE @ID_AdminServer int
DECLARE @ID_MasterServer int
DECLARE @ID_Group_Public int
DECLARE @ID_Group_Anonymous int
DECLARE @Group_Public_Name nvarchar(255)

SET NOCOUNT ON

SELECT @Group_Public_Name = 'Public ' + SubString(@GroupName, 1, 245)
SELECT @ID_AdminServer = (SELECT TOP 1 UserAdminServer FROM System_ServerGroups)
SELECT @ID_ServerGroup = (SELECT ID FROM System_ServerGroups WHERE ServerGroup = @GroupName)
SELECT @ID_Group_Public = (SELECT ID FROM Gruppen WHERE Name = @Group_Public_Name)
SELECT @ID_Group_Anonymous = (SELECT ID FROM Gruppen WHERE Name = 'Anonymous')
SELECT @ID_MasterServer = (SELECT TOP 1 ID FROM System_Servers WHERE IP = 'secured.yourcompany.com')

-- Erstellbarkeit gewährleisten
IF @ID_ServerGroup Is Not Null 
	BEGIN
		RAISERROR ('Server group already exists', 16, 1)
		RETURN	
	END	
IF @ID_Group_Public Is Not Null
 
	BEGIN
		RAISERROR ('The server group cannot be created because the public group already exists.', 16, 2)
		RETURN	
	END	
IF @ID_Group_Anonymous Is Null 
	BEGIN
		RAISERROR ('Anonymous user cannot be found. There might be a misconfiguration.', 16, 3)
		RETURN	
	END	
IF @ID_MasterServer Is Not Null 
	BEGIN
		RAISERROR ('There is already a server called "secured.yourcompany.com". Please rename it first before creating new server groups.', 16, 3)
		RETURN	
	END	

BEGIN TRANSACTION

SELECT @ID_Group_Public = Null, @ID_MasterServer = Null, @ID_ServerGroup = Null

-- Public Group anlegen
INSERT INTO dbo.Gruppen
                      (Name, Description, ReleasedOn, ReleasedBy, SystemGroup, ModifiedOn, ModifiedBy)
SELECT     @Group_Public_Name AS Name, 'System group: all users logged on' AS ServerDescription, GETDATE() AS ReleasedOn, @UserID_Creator AS ReleasedBy, 
                      1 AS SystemGroup, GETDATE() AS ModifiedOn, @UserID_Creator AS ModifiedBy
SELECT @ID_Group_Public = @@IDENTITY

-- Neuen Server anlegen, welcher als MasterServer fungieren soll
INSERT INTO dbo.System_Servers
                      (Enabled, IP, ServerDescription, ServerGroup, ServerProtocol, ServerName, ServerPort, ReAuthenticateByIP, WebSessionTimeout, LockTimeout)
SELECT     0 AS Enabled, 'secured.yourcompany.com' AS IP, 'Secured server' AS ServerDescription, 0 AS ServerGroup, 'https' AS ServerProtocol, 
                      'secured.yourcompany.com' AS ServerName, NULL AS ServerPort, 0 AS ReAuthenticateByIP, 15 AS WebSessionTimeout, 3 AS LockTimeout
SELECT @ID_MasterServer = @@IDENTITY

-- Check script engines
EXEC AdminPrivate_SetScriptEngineActivation 0, @ID_MasterServer, 0, 1

-- ServerGroup anlegen
INSERT INTO dbo.System_ServerGroups
                      (ServerGroup, ID_Group_Public, ID_Group_Anonymous, MasterServer, UserAdminServer, AreaImage, AreaButton, AreaNavTitle, 
                      AreaCompanyFormerTitle, AreaCompanyTitle, AreaSecurityContactEMail, AreaSecurityContactTitle, AreaDevelopmentContactEMail, 
                      AreaDevelopmentContactTitle, AreaContentManagementContactEMail, AreaContentManagementContactTitle, AreaUnspecifiedContactEMail, 
                      AreaUnspecifiedContactTitle, AreaCopyRightSinceYear, AreaCompanyWebSiteURL, AreaCompanyWebSiteTitle, ModifiedBy)
SELECT     @GroupName AS Expr21, @ID_Group_Public AS Expr19, @ID_Group_Anonymous AS Expr20, @ID_MasterServer AS Expr18, @ID_AdminServer AS Expr17, 
                      '/sysdata/images/global/logo_csa.jpg' AS Expr1, '/sysdata/images/global/button_csa.gif' AS Expr2, NULL AS Expr3, 'YourCompany Ltd.' AS Expr4, 'YourCompany' AS Expr5, @email_Developer AS Expr6, 
                      @email_Developer AS Expr7, @email_Developer AS Expr8, @email_Developer AS Expr9, @email_Developer AS Expr10, 
                      @email_Developer AS Expr11, @email_Developer AS Expr12, @email_Developer AS Expr13, DATEPART(yyyy, GETDATE()) AS Expr14, 
                      'http://www.yourcompany.com/' AS Expr15, 'YourCompany Homepage' AS Expr16, @UserID_Creator as ModifiedBy
SELECT @ID_ServerGroup = @@IDENTITY

-- Master Server in die ServerGroup aufnehmen
UPDATE dbo.System_Servers SET ServerGroup = @ID_ServerGroup WHERE ID = @ID_MasterServer

IF @ID_Group_Public Is Null OR @ID_MasterServer Is Null OR @ID_ServerGroup Is Null
	ROLLBACK TRANSACTION
ELSE

	COMMIT TRANSACTION

-- Create master server navigation
EXEC AdminPrivate_CreateMasterServerNavPoints @ID_MasterServer, Null, @UserID_Creator

SET NOCOUNT OFF

SELECT @ID_ServerGroup
Return @ID_ServerGroup

GO
----------------------------------------------------
-- dbo.AdminPrivate_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_CreateUserAccount
	@Username varchar(20),
	@Passcode varchar(4096),
	@WebApplication varchar(1024),
	@ServerIP varchar(32),
	@Company nvarchar(50),
	@Anrede nvarchar(20),
	@Titel nvarchar(20),
	@Vorname nvarchar(30),
	@Nachname nvarchar(30),
	@Namenszusatz nvarchar(20),
	@eMail varchar(50),
	@Strasse nvarchar(30),
	@PLZ nvarchar(10),
	@Ort nvarchar(20),
	@State nvarchar(30),
	@Land varchar(30),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability tinyint,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @LocationID int
DECLARE @WebAppID int

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @LocationID)))


-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, password will be updated now
	BEGIN
		-- Record update
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', @WebAppID, 3, 'Account ' + @Username + ' created')
		SET NOCOUNT OFF
		-- Rückgabewert
		If @CurUserID Is Not Null 
			SELECT Result = -1 
		Else
			SELECT Result = -2
	END

Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID
GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteAccessLevel 
	@ID int,
	@JustAnotherAccessLevel int = Null

AS

-- If no replacement ID is given then search for a random one
If @JustAnotherAccessLevel Is Null
	SELECT TOP 1 @JustAnotherAccessLevel = ID FROM dbo.System_AccessLevels WHERE ID <> @ID

DELETE FROM dbo.System_AccessLevels WHERE ID = @ID


UPDATE dbo.System_ServerGroups
SET AccessLevel_Default = @JustAnotherAccessLevel
WHERE AccessLevel_Default = @ID


UPDATE dbo.Benutzer
Set AccountAccessability = @JustAnotherAccessLevel
Where AccountAccessability = @ID
GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServer
	(
		@ServerID int
	)

AS

-- Script engines of connected servers will be UNREGISTERED. 
DELETE System_WebAreaScriptEnginesAuthorization
FROM System_Servers INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_Servers.ID = @ServerID 

-- Related logs will be DELETED permanently. 
DELETE Log
FROM System_Servers INNER JOIN Log ON System_Servers.IP = Log.ServerIP
WHERE System_Servers.ID = @ServerID 
DELETE System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
FROM System_Servers INNER JOIN System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes ON System_Servers.ID = System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.Server
WHERE System_Servers.ID = @ServerID 

-- Related applications and their authorizations will be DELETED permanently.
DELETE ApplicationsRightsByGroup
FROM (System_Servers INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID) INNER JOIN ApplicationsRightsByGroup ON Applications_CurrentAndInactiveOnes.ID = ApplicationsRightsByGroup.ID_Application
WHERE System_Servers.ID = @ServerID 
DELETE ApplicationsRightsByUser
FROM (System_Servers INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID) INNER JOIN ApplicationsRightsByUser ON Applications_CurrentAndInactiveOnes.ID = ApplicationsRightsByUser.ID_Application
WHERE System_Servers.ID = @ServerID 
DELETE Applications_CurrentAndInactiveOnes
FROM System_Servers INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID
WHERE System_Servers.ID = @ServerID 

-- Script engine relations must be erased as well
DELETE 
FROM System_WebAreaScriptEnginesAuthorization
WHERE Server = @ServerID

-- DELETE the server itself
DELETE 
FROM dbo.System_Servers
WHERE ID = @ServerID

GO
----------------------------------------------------
-- dbo.AdminPrivate_DeleteServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteServerGroup
(
@ID_ServerGroup int
)

AS 

-- The corresponding public user group will be DELETED. 
DELETE Gruppen 
FROM System_ServerGroups INNER JOIN Gruppen ON System_ServerGroups.ID_Group_Public = Gruppen.ID 
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Relations between access levels and the server group will be DELETED. 
DELETE 
FROM System_ServerGroupsAndTheirUserAccessLevels 
WHERE ID_ServerGroup = @ID_ServerGroup

-- Script engines of connected servers will be UNREGISTERED. 
DELETE System_WebAreaScriptEnginesAuthorization
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Related logs will be DELETED permanently. 
DELETE Log
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Log ON System_Servers.IP = Log.ServerIP
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes ON System_Servers.ID = System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.Server
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Related applications and their authorizations will be DELETED permanently.
DELETE ApplicationsRightsByGroup
FROM ((System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.
ID = Applications_CurrentAndInactiveOnes.LocationID) INNER JOIN ApplicationsRightsByGroup ON Applications_CurrentAndInactiveOnes.ID = ApplicationsRightsByGroup.ID_Application
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE ApplicationsRightsByUser
FROM ((System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID) INNER JOIN ApplicationsRightsByUser ON Applications_CurrentAndInactiveOnes.ID = ApplicationsRightsByUser.ID_Application
WHERE System_ServerGroups.ID = @ID_ServerGroup
DELETE Applications_CurrentAndInactiveOnes
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN Applications_CurrentAndInactiveOnes ON System_Servers.ID = Applications_CurrentAndInactiveOnes.LocationID
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- All currently connected servers will be DELETED permanently. 
DELETE System_Servers
FROM System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- Script engine relations must be erased as well
DELETE System_WebAreaScriptEnginesAuthorization
FROM (System_ServerGroups INNER JOIN System_Servers ON System_ServerGroups.ID = System_Servers.ServerGroup) INNER JOIN System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server
WHERE System_ServerGroups.ID = @ID_ServerGroup

-- DELETE the server group itself
DELETE 
FROM System_ServerGroups
WHERE System_ServerGroups.ID = @ID_ServerGroup


SET NOCOUNT OFF

GO
----------------------------------------------------
-- dbo.AdminPrivate_GetCompleteUserInfo
----------------------------------------------------
ALTER Procedure dbo.AdminPrivate_GetCompleteUserInfo
(
	@UserID int
)

As
SELECT * FROM dbo.Benutzer WHERE ID = @UserID
	/* set nocount on */
	return

GO
----------------------------------------------------
-- dbo.AdminPrivate_GetScriptEnginesOfServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_GetScriptEnginesOfServer
(
@ServerID int
)

AS 
SELECT     (SELECT     WebEngine.ScriptEngine
                       FROM          System_WebAreaScriptEnginesAuthorization AS WebEngine
                       WHERE      (WebEngine.Server = @ServerID OR
                                              WebEngine.Server IS NULL) AND System_ScriptEngines.ID = WebEngine.ScriptEngine) AS ID, EngineName, ID AS ScriptEngineID,
                          CASE WHEN (SELECT      WebEngine2.Server 
                            FROM          System_WebAreaScriptEnginesAuthorization AS WebEngine2
                            WHERE      (WebEngine2.Server = @ServerID OR
                                                   WebEngine2.Server IS NULL) AND System_ScriptEngines.ID = WebEngine2.ScriptEngine) IS NULL THEN 0 ELSE 1 END AS IsActivated
FROM         System_ScriptEngines
GO
----------------------------------------------------
-- dbo.AdminPrivate_ResetLoginLockedTill
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_ResetLoginLockedTill
	(
		@ID int
	)

AS
declare @AccountAccessability tinyint
declare @LoginDisabled bit
declare @LoginLockedTill datetime
declare @CurrentLoginViaRemoteIP varchar(32)
	SET NOCOUNT ON
	SELECT @AccountAccessability = AccountAccessability, @LoginDisabled = LoginDisabled, @LoginLockedTill =LoginLockedTill, @CurrentLoginViaRemoteIP = CurrentLoginViaRemoteIP FROM Benutzer WHERE ID = @ID
	If @LoginLockedTill Is Not Null 
	Begin
		UPDATE    dbo.Benutzer
		SET LoginLockedTill = NULL
		WHERE (ID = @ID)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, URL, ConflictType) values (@ID, GetDate(), '0.0.0.0', '0.0.0.0', 'Lock status reset by Administrator', -5)
	End
	If @CurrentLoginViaRemoteIP Is Not Null
	Begin	
		update dbo.Benutzer 
		set CurrentLoginViaRemoteIP = Null 
		WHERE (ID = @ID)
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, URL, ConflictType) values (@ID, GetDate(), '0.0.0.0', '0.0.0.0', 'Logout by Administrator', 99)
	End
	SET NOCOUNT OFF
	SELECT @AccountAccessability As AccountAccessability, @LoginDisabled As LoginDisabled, @LoginLockedTill As LoginLockedTill, @CurrentLoginViaRemoteIP As CurrentLoginViaRemoteIP
	RETURN 

GO
----------------------------------------------------
-- dbo.AdminPrivate_SetAuthorizationInherition
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetAuthorizationInherition 
(
@ReleasedByUserID int, 
@IDApp int, 
@InheritsFrom int
)

AS 
SET NOCOUNT ON
UPDATE    dbo.Applications
SET              AuthsAsAppID = @InheritsFrom, ModifiedBy = @ReleasedByUserID, ModifiedOn = getdate()
WHERE     (ID = @IDApp)
-- Logging
If (@InheritsFrom Is Null) 
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 1, 'Now inhertis from nothing')
	End
Else
	Begin
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		values (@ReleasedByUserID, GetDate(), '0.0.0.0', '0.0.0.0', @IDApp, 1, 'Now inhertis from ID ' + Convert(varchar(50), @InheritsFrom))
	End
SET NOCOUNT ON
SELECT Result = -1

GO
----------------------------------------------------
-- dbo.AdminPrivate_SetScriptEngineActivation
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_SetScriptEngineActivation
(
@ScriptEngineID int,
@ServerID int,
@Enabled bit,
@CheckMinimalActivations bit = 0
)

AS 

declare @ID int

SET NOCOUNT ON

-- CheckMinimalActivations nie bei customized Servern (ServerID < 0)
If @ServerID < 0
	SELECT @CheckMinimalActivations = 0

IF @CheckMinimalActivations = 0
	
	BEGIN
		SELECT     @ID = dbo.System_WebAreaScriptEnginesAuthorization.ID
		FROM         dbo.System_WebAreaScriptEnginesAuthorization
		WHERE Server = @ServerID AND ScriptEngine = @ScriptEngineID 
	
		If @Enabled <> 0 
			-- Enabled
			BEGIN
				IF @ID Is Null
					INSERT INTO dbo.System_WebAreaScriptEnginesAuthorization (Server, ScriptEngine)
					VALUES (@ServerID, @ScriptEngineID)
			END			
		Else 
			-- Disabled
			IF @ID Is Not Null
				DELETE FROM dbo.System_WebAreaScriptEnginesAuthorization
				WHERE ID = @ID
	END

ELSE	
	BEGIN
		SELECT   TOP 1  @ID = dbo.System_WebAreaScriptEnginesAuthorization.ID
		FROM         dbo.System_WebAreaScriptEnginesAuthorization
		WHERE Server = @ServerID

		IF @ID Is Null
			-- Activate at least ASP script
			INSERT INTO dbo.System_WebAreaScriptEnginesAuthorization (Server, ScriptEngine)
			VALUES (@ServerID, 1) -- 1 = ASP
	END

SET NOCOUNT OFF

GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateAccessLevel
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateAccessLevel 
(
	@ID int,
	@ReleasedByUserID int,
	@Title nvarchar(50),
	@Remarks ntext
)

AS
DECLARE @CurUserID int
SET @CurUserID = (select ID from dbo.Benutzer where id = @ReleasedByUserID)
If @CurUserID Is Not Null	
	BEGIN
		SET NOCOUNT ON
		UPDATE dbo.System_AccessLevels 
		SET Title = @Title, ReleasedBy = @ReleasedByUserID, Remarks = @Remarks, ModifiedBy = @ReleasedByUserID, ModifiedOn = GetDate()
		WHERE ID = @ID
		SET NOCOUNT OFF
	END
Else
	
	SELECT Result = 0



GO
--------------------------------------------------
-- dbo.AdminPrivate_UpdateApp --
--------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateApp
(
@Title varchar(255),
@TitleAdminArea nvarchar(255),
@Level1Title nvarchar(512),
@Level2Title nvarchar(512),
@Level3Title nvarchar(512),
@Level4Title nvarchar(512),
@Level5Title nvarchar(512),
@Level6Title nvarchar(512),
@Level1TitleIsHTMLCoded bit,
@Level2TitleIsHTMLCoded bit,
@Level3TitleIsHTMLCoded bit,
@Level4TitleIsHTMLCoded bit,
@Level5TitleIsHTMLCoded bit,
@Level6TitleIsHTMLCoded bit,
@NavURL varchar(512),
@NavFrame varchar(50),
@NavTooltipText nvarchar(1024),
@IsNew bit,
@IsUpdated bit,
@LocationID int,
@LanguageID int,
@ModifiedBy int,
@AppDisabled bit,
@Sort int,
@ResetIsNewUpdatedStatusOn varchar(30),
@OnMouseOver nvarchar(512),
@OnMouseOut nvarchar(512),
@OnClick nvarchar(512),
@AddLanguageID2URL bit,
@ID int
)

AS 
IF @LocationID < 0 
	UPDATE    dbo.Applications
	SET              Title = @Title, TitleAdminArea = @TitleAdminArea, Level1Title = CASE WHEN @Level1Title = '' THEN NULL ELSE @Level1Title END, 
                      Level2Title = CASE WHEN @Level2Title = '' THEN NULL ELSE @Level2Title END, Level3Title = CASE WHEN @Level3Title = '' THEN NULL 
                      ELSE @Level3Title END, Level4Title = CASE WHEN @Level4Title = '' THEN NULL ELSE @Level4Title END, 
                      Level5Title = CASE WHEN @Level5Title = '' THEN NULL ELSE @Level5Title END, Level6Title = CASE WHEN @Level6Title = '' THEN NULL 
                      ELSE @Level6Title END, Level1TitleIsHTMLCoded = @Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded = @Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded = @Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded = @Level4TitleIsHTMLCoded, 
                      Level5TitleIsHTMLCoded = @Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded = @Level6TitleIsHTMLCoded, NavURL = @NavURL, 
                      NavFrame = @NavFrame, NavTooltipText = @NavTooltipText, IsNew = @IsNew, IsUpdated = @IsUpdated, LocationID = @LocationID, 
                      LanguageID = @LanguageID, ModifiedOn = GETDATE(), ModifiedBy = @ModifiedBy, AppDisabled = @AppDisabled, Sort = @Sort, 
                      ResetIsNewUpdatedStatusOn = CONVERT(datetime, @ResetIsNewUpdatedStatusOn, 121), OnMouseOver = @OnMouseOver, 
                      OnMouseOut = @OnMouseOut, OnClick = @OnClick, AddLanguageID2URL = @AddLanguageID2URL
		,SystemAppType = @LocationID
	WHERE     (ID = @ID)
Else
	UPDATE    dbo.Applications
	SET              Title = @Title, TitleAdminArea = @TitleAdminArea, Level1Title = CASE WHEN @Level1Title = '' THEN NULL ELSE @Level1Title END, 
                      Level2Title = CASE WHEN @Level2Title = '' THEN NULL ELSE @Level2Title END, Level3Title = CASE WHEN @Level3Title = '' THEN NULL 
                      ELSE @Level3Title END, Level4Title = CASE WHEN @Level4Title = '' THEN NULL ELSE @Level4Title END, 
                      Level5Title = CASE WHEN @Level5Title = '' THEN NULL ELSE @Level5Title END, Level6Title = CASE WHEN @Level6Title = '' THEN NULL 
                      ELSE @Level6Title END, Level1TitleIsHTMLCoded = @Level1TitleIsHTMLCoded, Level2TitleIsHTMLCoded = @Level2TitleIsHTMLCoded, 
                      Level3TitleIsHTMLCoded = @Level3TitleIsHTMLCoded, Level4TitleIsHTMLCoded = @Level4TitleIsHTMLCoded, 
                      Level5TitleIsHTMLCoded = @Level5TitleIsHTMLCoded, Level6TitleIsHTMLCoded = @Level6TitleIsHTMLCoded, NavURL = @NavURL, 
                      NavFrame = @NavFrame, NavTooltipText = @NavTooltipText, IsNew = @IsNew, IsUpdated = @IsUpdated, LocationID = @LocationID, 
                      LanguageID = @LanguageID, ModifiedOn = GETDATE(), ModifiedBy = @ModifiedBy, AppDisabled = @AppDisabled, Sort = @Sort, 
                      ResetIsNewUpdatedStatusOn = CONVERT(datetime, @ResetIsNewUpdatedStatusOn, 121), OnMouseOver = @OnMouseOver, 
                      OnMouseOut = @OnMouseOut, OnClick = @OnClick, AddLanguageID2URL = @AddLanguageID2URL
	WHERE     (ID = @ID)

GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateServer
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateServer
(
@Enabled bit,
@IP nvarchar(32),
@ServerDescription nvarchar(200),
@ServerGroup int,
@ServerProtocol nvarchar(50),
@ServerName nvarchar(200),
@ServerPort int,
@ID int
)

AS

DECLARE @CurServerIP nvarchar(32)

DECLARE @OldServerIP nvarchar(32)
DECLARE @OldServerGroup int

SELECT @OldServerIP = (SELECT IP FROM System_Servers WHERE ID = @ID)
SELECT @OldServerGroup = (SELECT ServerGroup FROM System_Servers WHERE ID = @ID)

-- Hat sich die IP geändert?
If @OldServerIP <> @IP 
	BEGIN
		-- Kann der neue IP-Wert gespeichert werden oder existiert er schon?
		SELECT @OldServerIP = (SELECT IP FROM System_Servers WHERE IP = @IP)
		If @OldServerIP Is Not Null
			BEGIN
				RAISERROR ('IP / Host Header already exists', 16, 1
)
				RETURN
			END

		-- Logs umschreiben
		UPDATE LOG
		SET Log.ServerIP = @IP
		WHERE     (Log.ServerIP = @OldServerIP)

	END

-- Server updaten
UPDATE    dbo.System_Servers
SET              Enabled = @Enabled, IP = @IP, ServerDescription = @ServerDescription, ServerGroup = @ServerGroup, ServerProtocol = @ServerProtocol, 
                      ServerName = @ServerName, ServerPort = @ServerPort
WHERE     (ID = @ID)
GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateServerGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateServerGroup
(
@ID int,
@ServerGroup nvarchar(255),
@ID_Group_Public int,
@ID_Group_Anonymous int,
@MasterServer int,
@UserAdminServer int,
@AreaImage nvarchar(512),
@AreaButton nvarchar(512),
@AreaNavTitle nvarchar(512),
@AreaCompanyFormerTitle nvarchar(512),
@AreaCompanyTitle nvarchar(512),
@AreaSecurityContactEMail nvarchar(512),
@AreaSecurityContactTitle nvarchar(512),
@AreaDevelopmentContactEMail nvarchar(512),
@AreaDevelopmentContactTitle nvarchar(512),
@AreaContentManagementContactEMail nvarchar(512),
@AreaContentManagementContactTitle nvarchar(512),
@AreaUnspecifiedContactEMail nvarchar(512),
@AreaUnspecifiedContactTitle nvarchar(512),
@AreaCopyRightSinceYear int,
@AreaCompanyWebSiteURL nvarchar(512),
@AreaCompanyWebSiteTitle nvarchar(512),
@ModifiedBy int,
@AccessLevel_Default int
)

AS

DECLARE @OldAdminServer int
DECLARE @OldMasterServer int

SELECT @OldAdminServer = UserAdminServer, @OldMasterServer = MasterServer FROM dbo.System_ServerGroups WHERE ID = @ID

UPDATE    dbo.System_ServerGroups
SET              ServerGroup = @ServerGroup, ID_Group_Public = @ID_Group_Public, ID_Group_Anonymous = @ID_Group_Anonymous, 
                      MasterServer = @MasterServer, UserAdminServer = @UserAdminServer, AreaImage = @AreaImage, AreaButton = @AreaButton, 
                      AreaNavTitle = @AreaNavTitle, AreaCompanyFormerTitle = @AreaCompanyFormerTitle, AreaCompanyTitle = @AreaCompanyTitle, 
                      AreaSecurityContactEMail = @AreaSecurityContactEMail, AreaSecurityContactTitle = @AreaSecurityContactTitle, 
                      AreaDevelopmentContactEMail = @AreaDevelopmentContactEMail, AreaDevelopmentContactTitle = @AreaDevelopmentContactTitle, 
                      AreaContentManagementContactEMail = @AreaContentManagementContactEMail, 
                      AreaContentManagementContactTitle = @AreaContentManagementContactTitle, AreaUnspecifiedContactEMail = @AreaUnspecifiedContactEMail, 
                      AreaUnspecifiedContactTitle = @AreaUnspecifiedContactTitle, AreaCopyRightSinceYear = @AreaCopyRightSinceYear, 
                      AreaCompanyWebSiteURL = @AreaCompanyWebSiteURL, AreaCompanyWebSiteTitle = @AreaCompanyWebSiteTitle, ModifiedBy = @ModifiedBy, ModifiedOn = getdate(),
                      AccessLevel_Default = @AccessLevel_Default
WHERE     (ID = @ID)

If @OldAdminServer <> @UserAdminServer 
	EXEC AdminPrivate_CreateAdminServerNavPoints @UserAdminServer, @OldAdminServer, @ModifiedBy

If @OldMasterServer <> @MasterServer 
	EXEC AdminPrivate_CreateMasterServerNavPoints @MasterServer, @OldMasterServer, @ModifiedBy

GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateStatusLoginDisabled
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateStatusLoginDisabled 
	@Username varchar(20),
	@boolStatus bit

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1

		-- Password update
		UPDATE dbo.Benutzer SET LoginDisabled = @boolStatus, ModifiedOn = GetDate() WHERE LoginName = @Username
	END
Else
	-- Rückgabewert
	SELECT Result = 0

GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateUserDetails 
(
	@CurUserID int,
	@WebApplication varchar(1024),
	@Company nvarchar(50),
	@Anrede nvarchar(20),
	@Titel nvarchar(20),
	@Vorname nvarchar(30),
	@Nachname nvarchar(30),
	@Namenszusatz nvarchar(20),
	@eMail varchar(50),
	@Strasse nvarchar(30),
	@PLZ nvarchar(10),
	@Ort nvarchar(50),
	@State nvarchar(30),
	@Land varchar(30),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability tinyint,
	@LoginDisabled bit = 0,
	@LoginLockedTill datetime,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS

SET NOCOUNT ON
-- Profile update
	-- Password update
	UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, Land = @Land, State = @State, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, AccountAccessability = @AccountAccessability, LoginDisabled = @LoginDisabled, LoginLockedTill = @LoginLockedTill, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE ID = @CurUserID
	-- Logging
	insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 4, 'Admin has modified profile')
	-- Write UserDetails
	Exec Int_UpdateUserDetailDataWithProfileData @CurUserID

-- Rückgabewert
SET NOCOUNT OFF
SELECT Result = -1

GO
----------------------------------------------------
-- dbo.AdminPrivate_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_UpdateUserPW 
(
	@Username varchar(20),
	@NewPasscode varchar(4096)
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Password update
If @CurUserID >= 0
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Password update
		UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate(), LoginLockedTill = Null, LoginFailures = 0 WHERE LoginName = @Username
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), '0.0.0.0', '0.0.0.0', 5, 'Admin has modified password')
	END
Else
	-- Rückgabewert
	SELECT Result = 0

GO

----------------------------------------------------
-- dbo.Int_LogAuthChanges
----------------------------------------------------
ALTER PROCEDURE dbo.Int_LogAuthChanges
(
@UserID int = Null,
@GroupID int = Null,
@AppID int
)

AS 

If @GroupID Is Not Null
	begin
		-- log indirect changes on users
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) 
		select id_user, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -7, Null
		from view_Memberships_CummulatedWithAnonymous
		where id_group = @GroupID
		-- log group auth change
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -8, @GroupID)
	end
Else
	If @UserID Is Not Null
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', @AppID, -6, Null)
GO
----------------------------------------------------
-- dbo.Int_UpdateUserDetailDataWithProfileData
----------------------------------------------------
ALTER Procedure dbo.Int_UpdateUserDetailDataWithProfileData
	(
		@IDUser int
	)

As
DECLARE @LoginName varchar
	-- Result and Initializing
	SELECT -1
	set nocount on
-- GetCompleteName (a little bit modified)
	DECLARE @Addresses nvarchar (20)
	DECLARE @Titel nvarchar (20)
	DECLARE @Vorname nvarchar(30)
	DECLARE @Nachname nvarchar(30)
	DECLARE @Namenszusatz nvarchar(20)
	DECLARE @CompleteName nvarchar (255)
	DECLARE @CompleteNameInclAddresses nvarchar (255)
	DECLARE @CustomerNo nvarchar(50)
	DECLARE @SupplierNo nvarchar(50)
	DECLARE @Company nvarchar(100)
	DECLARE @eMail varchar (255)
	DECLARE @Sex varchar (1)
	
	SELECT @Sex = Case Anrede When 'Mr.' Then 'm' When 'Ms.' Then 'w' Else Null End, @eMail = [E-MAIL], @Vorname = Vorname, @Nachname = Nachname, @Nachname = Nachname, @Namenszusatz = Namenszusatz, @CustomerNo = CustomerNo, @SupplierNo = SupplierNo, @Company = Company, @Titel = Titel, @Addresses = Anrede FROM dbo.Benutzer WHERE ID = @IDUser

	-- Namenszusatz könnte Null sein
	If substring(@Namenszusatz,1,20) <> '' --Is Not Null
		SET @Namenszusatz = ' ' + @Namenszusatz
	Else
		SET @Namenszusatz = ''
	SET @CompleteName = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	SET @CompleteNameInclAddresses  = LTrim(RTrim(@Addresses + @Titel)) + ' ' + LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	Exec Public_SetUserDetailData @IDUser, 'CompleteName', @CompleteName, 1
	Exec Public_SetUserDetailData @IDUser, 'CompleteNameInclAddresses', @CompleteNameInclAddresses, 1
	Exec Public_SetUserDetailData @IDUser, 'Sex', @Sex, 1
	-- e-mail address
	Exec Public_SetUserDetailData @IDUser, 'email', @eMail, 1
	-- Other details
	Exec Public_SetUserDetailData @IDUser, 'CustomerNo', @CustomerNo, 1
	Exec Public_SetUserDetailData @IDUser, 'SupplierNo', @SupplierNo, 1
	Exec Public_SetUserDetailData @IDUser, 'Company', @Company, 1

-- Exit

	return 
GO
----------------------------------------------------
-- dbo.Public_CreateUserAccount
----------------------------------------------------
ALTER PROCEDURE dbo.Public_CreateUserAccount
(
	@Username varchar(20),
	@Passcode varchar(4096),
	@ServerIP varchar(32),
	@RemoteIP varchar(32),
	@Company nvarchar(50),
	@Anrede nvarchar(20),
	@Titel nvarchar(20),
	@Vorname nvarchar(30),
	@Nachname nvarchar(30),
	@Namenszusatz nvarchar(20),
	@eMail varchar(30),
	@Strasse nvarchar(30),
	@PLZ nvarchar(10),
	@Ort nvarchar(20),
	@State nvarchar(30),
	@Land varchar(30),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@AccountAccessability int = 0,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @LocationID int
DECLARE @CurUserStatus_InternalSessionID int

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SET @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0

	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

-- Password validation and update
If @CurUserID Is Null
	-- Validation successfull, account will be created now
	BEGIN
		-- Rückgabewert
		SELECT Result = -1
		-- Create account
		SET NOCOUNT ON
		INSERT INTO dbo.Benutzer (LoginName, LoginPW, Company, Anrede, Titel, Vorname, Nachname, Namenszusatz, [e-mail], Strasse, PLZ, Ort, State, Land, CreatedOn, ModifiedOn, AccountAccessability, [1stPreferredLanguage], [2ndPreferredLanguage], [3rdPreferredLanguage], LoginCount, CustomerNo, SupplierNo) VALUES (@Username, @Passcode, @Company, @Anrede, @Titel, @Vorname, @Nachname, @Namenszusatz, @eMail, @Strasse, @PLZ, @Ort, @State, @Land, GetDate(), GetDate(), @AccountAccessability, @1stPreferredLanguage, @2ndPreferredLanguage, @3rdPreferredLanguage, 1, @CustomerNo, @SupplierNo)
		-- Aktualisierung Variable: UserID
		SET @CurUserID = @@IDENTITY --(select ID from dbo.Benutzer where LoginName = @Username)
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = @@IDENTITY
		UPDATE dbo.Benutzer SET System_SessionID = @CurUserStatus_InternalSessionID WHERE LoginName = @UserName
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		INSERT INTO dbo.System_WebAreasAuthorizedForSession
		                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
		SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
		                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
		FROM         dbo.System_Servers INNER JOIN
		                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
		WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @LocationID)
		SET NOCOUNT OFF
		-- Logging
		SET NOCOUNT ON
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 1, 'Account ' + @Username + ' created')
		SET NOCOUNT OFF
	END
Else
	-- Rückgabewert
	SELECT Result = 1
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID

GO
----------------------------------------------------
-- dbo.Public_GetCompleteAddresses
----------------------------------------------------
ALTER Procedure dbo.Public_GetCompleteAddresses
(
	@Username varchar(20)
)

As
DECLARE @Anrede nvarchar(20)
DECLARE @Titel nvarchar(20)
DECLARE @Vorname nvarchar(30)
DECLARE @Nachname nvarchar(30)
DECLARE @Namenszusatz nvarchar(20)
SET @Anrede = (SELECT Anrede FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Titel = (SELECT Titel FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Vorname = (SELECT Vorname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Nachname = (SELECT Nachname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Namenszusatz = (SELECT Namenszusatz FROM dbo.Benutzer WHERE Loginname = @Username)
-- Titel könnte Null sein
If substring(@Titel ,1,20) <> '' --Is Not Null
	SET @Titel = ' ' + @Titel 
Else
	SET @Titel = ''
-- Namenszusatz könnte Null sein
If substring(@Namenszusatz,1,20) <> '' --Is Not Null
	SET @Namenszusatz = ' ' + @Namenszusatz
Else
	SET @Namenszusatz = ''
SELECT Result = LTrim(RTrim(@Anrede + @Titel)) + ' ' + LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	/* set nocount on */
	return

GO
----------------------------------------------------
-- dbo.Public_GetCompleteName
----------------------------------------------------
ALTER Procedure dbo.Public_GetCompleteName
(
	@Username varchar(20)
)

As
DECLARE @Vorname nvarchar(30)
DECLARE @Nachname nvarchar(30)
DECLARE @Namenszusatz nvarchar(20)
SET @Vorname = (SELECT Vorname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Nachname = (SELECT Nachname FROM dbo.Benutzer WHERE Loginname = @Username)
SET @Namenszusatz = (SELECT Namenszusatz FROM dbo.Benutzer WHERE Loginname = @Username)
-- Namenszusatz könnte Null sein
If substring(@Namenszusatz,1,20) <> '' --Is Not Null
	SET @Namenszusatz = ' ' + @Namenszusatz
Else
	SET @Namenszusatz = ''
SELECT Result = LTrim(RTrim(@Vorname + @Namenszusatz + ' ' +  @Nachname))
	/* set nocount on */
	return 

GO
----------------------------------------------------
-- dbo.Public_GetCompleteUserInfo
----------------------------------------------------
ALTER Procedure dbo.Public_GetCompleteUserInfo
(
		@Username varchar(20)
)

As
SELECT * FROM dbo.Benutzer WHERE loginname = @Username
	/* set nocount on */
	return
GO
----------------------------------------------------
-- dbo.Public_GetCurServerLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetCurServerLogonList
(
@ServerIP varchar(32)
)

AS 

DECLARE @LocationID int
----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup

FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Abbruch
		Return
	END

---------------------------------------
-- Anmeldestellen zurückliefern --
---------------------------------------
SELECT     NULL AS ID, NULL AS SessionID, System_Servers.IP, System_Servers.ServerDescription, System_Servers.ServerProtocol, 
                      System_Servers.ServerName, System_Servers.ServerPort, NULL AS ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, NULL AS ScriptEngine_SessionID, System_WebAreaScriptEnginesAuthorization.ID AS OrderID1, 
                      System_Servers.ID AS OrderID2, System_ScriptEngines.ID AS OrderID3
FROM         System_Servers INNER JOIN
                      System_WebAreaScriptEnginesAuthorization ON System_Servers.ID = System_WebAreaScriptEnginesAuthorization.Server INNER JOIN
                      System_ScriptEngines ON System_WebAreaScriptEnginesAuthorization.ScriptEngine = System_ScriptEngines.ID
WHERE     (System_Servers.Enabled <> 0) AND (System_Servers.ServerGroup = @LocationID) AND (System_Servers.ID > 0)
ORDER BY System_WebAreaScriptEnginesAuthorization.ID, System_Servers.ID, System_ScriptEngines.ID

GO
----------------------------------------------------
-- dbo.Public_GetEMailAddressesOfAllSecurityAdmins
----------------------------------------------------
ALTER Procedure dbo.Public_GetEMailAddressesOfAllSecurityAdmins

As
	SELECT Benutzer.[E-MAIL], Benutzer.ID FROM dbo.Memberships LEFT OUTER JOIN dbo.Benutzer ON dbo.Memberships.ID_User = dbo.Benutzer.ID WHERE (dbo.Memberships.ID_Group = 7)
	return 



GO
----------------------------------------------------
-- dbo.Public_GetLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetLogonList
	(
	@Username varchar(20)
	)

AS

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
                      System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID, 
                      System_WebAreasAuthorizedForSession.LastSessionStateRefresh
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND (Benutzer.Loginname = @Username) AND (System_Servers.ID > 0)

GO
----------------------------------------------------
-- dbo.Public_GetNavPointsOfUser
----------------------------------------------------
ALTER Procedure dbo.Public_GetNavPointsOfUser
(
	@UserID int,
	@ServerIP varchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0
)

As
DECLARE @IsSecurityAdmin bit
DECLARE @AllowedLocation int
DECLARE @buffer int
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int

SET NoCount ON

SET @buffer = (SELECT COUNT(ID_Group) FROM dbo.view_Memberships WHERE (ID_Group = 6) AND (ID_User = @UserID))
If @buffer = 0 
	SET @IsSecurityAdmin = 0
Else
	SET @IsSecurityAdmin = 1

-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Abbruch
		Return
	END

-- Application des Internet-Server
SELECT   @AllowedLocation = dbo.System_Servers.ServerGroup, @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public, @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @AllowedLocation Is Null 
	Return

--ResetIsNewUpdatedStatusOn
UPDATE dbo.Applications_CurrentAndInactiveOnes SET IsNew = 0, IsUpdated = 0, ResetIsNewUpdatedStatusOn = Null WHERE (ResetIsNewUpdatedStatusOn < GETDATE())

-- Recordset zurückgeben	
If (@IsSecurityAdmin = 0)	-- True would be = 1

	BEGIN

		SELECT distinct Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
		into #NavUpdatedItems_Filtered
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_User = @UserID) OR (dbo.Memberships.ID_User = @UserID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID = @LanguageID  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
				AND (IsUpdated <> 0 OR IsNew <> 0)

		SET NoCount OFF

		IF @AnonymousAccess = 0
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_User = @UserID) OR (dbo.Memberships.ID_User = @UserID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID = @LanguageID  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
				AND dbo.view_ApplicationRights.Title <> 'System - Login'
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title
		Else
			SELECT DISTINCT dbo.view_ApplicationRights.Level1Title, dbo.view_ApplicationRights.Level2Title, dbo.view_ApplicationRights.Level3Title, dbo.view_ApplicationRights.Level4Title, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title, 
				dbo.view_ApplicationRights.Level1TitleIsHTMLCoded, dbo.view_ApplicationRights.Level2TitleIsHTMLCoded, dbo.view_ApplicationRights.Level3TitleIsHTMLCoded, dbo.view_ApplicationRights.Level4TitleIsHTMLCoded, dbo.view_ApplicationRights.Level5TitleIsHTMLCoded, dbo.view_ApplicationRights.Level6TitleIsHTMLCoded, 
				dbo.view_ApplicationRights.NavURL, dbo.view_ApplicationRights.NavFrame, dbo.view_ApplicationRights.IsNew, dbo.view_ApplicationRights.IsUpdated, dbo.view_ApplicationRights.NavToolTipText, dbo.view_ApplicationRights.Sort, 
				dbo.view_ApplicationRights.AppDisabled, dbo.view_ApplicationRights.OnMouseOver, dbo.view_ApplicationRights.OnMouseOut, dbo.view_ApplicationRights.OnClick, dbo.view_ApplicationRights.AddLanguageID2URL
				, Case When dbo.view_ApplicationRights.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End As Level3TitleIsPresent, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent
, case when (select top 1 Level1Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level1Title = dbo.view_applicationrights.Level1Title) is null then 0 else 1 end as Level1IsUpdated
, case when (select top 1 Level2Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level2Title = dbo.view_applicationrights.Level2Title) is null then 0 else 1 end as Level2IsUpdated
, case when (select top 1 Level3Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level3Title = dbo.view_applicationrights.Level3Title) is null then 0 else 1 end as Level3IsUpdated
, case when (select top 1 Level4Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level4Title = dbo.view_applicationrights.Level4Title) is null then 0 else 1 end as Level4IsUpdated
, case when (select top 1 Level5Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level5Title = dbo.view_applicationrights.Level5Title) is null then 0 else 1 end as Level5IsUpdated
, case when (select top 1 Level6Title from #NavUpdatedItems_Filtered where #NavUpdatedItems_Filtered.Level6Title = dbo.view_applicationrights.Level6Title) is null then 0 else 1 end as Level6IsUpdated,
Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group  LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID And dbo.view_ApplicationRights.LanguageID = @LanguageID  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1)
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered
	END

Else 
	BEGIN

		SELECT distinct Level1Title
		into #NavUpdatedItems_Level1Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level2Title
		into #NavUpdatedItems_Level2Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level3Title
		into #NavUpdatedItems_Level3Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level4Title
		into #NavUpdatedItems_Level4Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level5Title
		into #NavUpdatedItems_Level5Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		SELECT distinct Level6Title
		into #NavUpdatedItems_Level6Title
		FROM dbo.Applications
		WHERE ((IsUpdated <> 0) OR (IsNew <> 0))

		IF @AnonymousAccess = 0
			SET @AnonymousGroupID = 0 -- ungültige GroupID, damit das Ergebnis später nicht verfälscht wird

		SET NoCount OFF

		SELECT DISTINCT dbo.Applications.Level1Title, dbo.Applications.Level2Title, dbo.Applications.Level3Title, dbo.Applications.Level4Title, dbo.Applications.Level5Title, dbo.Applications.Level6Title, 
			dbo.Applications.Level1TitleIsHTMLCoded, dbo.Applications.Level2TitleIsHTMLCoded, dbo.Applications.Level3TitleIsHTMLCoded, dbo.Applications.Level4TitleIsHTMLCoded, dbo.Applications.Level5TitleIsHTMLCoded, dbo.Applications.Level6TitleIsHTMLCoded, 
			dbo.Applications.NavURL, dbo.Applications.NavFrame, dbo.Applications.IsNew, dbo.Applications.IsUpdated, dbo.Applications.NavToolTipText, dbo.Applications.Sort, Level1IsUpdated = Case When  #NavUpdatedItems_Level1Title.Level1Title Is Not Null Then -1 Else 0 End, Level2IsUpdated = Case When  #NavUpdatedItems_Level2Title.Level2Title Is Not Null Then -1 Else 0 End, 
			Level3IsUpdated = Case When  #NavUpdatedItems_Level3Title.Level3Title Is Not Null Then -1 Else 0 End, Level4IsUpdated = Case When  #NavUpdatedItems_Level4Title.Level4Title Is Not Null Then -1 Else 0 End, Level5IsUpdated = Case When  #NavUpdatedItems_Level5Title.Level5Title Is Not Null Then -1 Else 0 End, Level6IsUpdated = Case When  #NavUpdatedItems_Level6Title.Level6Title Is Not Null Then -1 Else 0 End, 
			dbo.Applications.AppDisabled, dbo.Applications.OnMouseOver, dbo.Applications.OnMouseOut, dbo.Applications.OnClick, dbo.Applications.AddLanguageID2URL
			, Case When dbo.Applications.Level1Title Is Null Then 0 Else 1 End As Level1TitleIsPresent, Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End As Level2TitleIsPresent, Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End  As Level3TitleIsPresent, Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End As Level4TitleIsPresent, Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End As Level5TitleIsPresent, Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End As Level6TitleIsPresent,
			Case When Substring(NavURL,1,1) = '/' Then ServerProtocol + '://' + ServerName + Case When ServerPort Is Not Null Then ':' +Cast(ServerPort as Varchar(6)) Else '' End + NavURL Else NavURL End As NavURLAutocompleted
		FROM ((((((dbo.Applications
			 left join #NavUpdatedItems_Level1Title on dbo.Applications.Level1Title = #NavUpdatedItems_Level1Title.Level1Title
			) left join #NavUpdatedItems_Level2Title on dbo.Applications.Level2Title = #NavUpdatedItems_Level2Title.Level2Title
			) left join #NavUpdatedItems_Level3Title on dbo.Applications.Level3Title = #NavUpdatedItems_Level3Title.Level3Title
			) left join #NavUpdatedItems_Level4Title on dbo.Applications.Level4Title = #NavUpdatedItems_Level4Title.Level4Title
			) left join #NavUpdatedItems_Level5Title on dbo.Applications.Level5Title = #NavUpdatedItems_Level5Title.Level5Title
			) left join #NavUpdatedItems_Level6Title on dbo.Applications.Level6Title = #NavUpdatedItems_Level6Title.Level6Title
			)  LEFT JOIN dbo.System_Servers ON dbo.Applications.LocationID = dbo.System_Servers.ID
		WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.Applications.LanguageID = @LanguageID And dbo.Applications.Title <> 'System - Login'
		ORDER BY dbo.Applications.Sort, Case When dbo.Applications.Level2Title Is Null Then 0 Else 1 End, dbo.Applications.Level1Title, Case When dbo.Applications.Level3Title Is Null Then 0 Else 1 End, dbo.Applications.Level2Title, Case When dbo.Applications.Level4Title Is Null Then 0 Else 1 End, dbo.Applications.Level3Title, Case When dbo.Applications.Level5Title Is Null Then 0 Else 1 End, dbo.Applications.Level4Title, Case When dbo.Applications.Level6Title Is Null Then 0 Else 1 End, dbo.Applications.Level5Title, dbo.Applications.Level6Title

		DROP TABLE #NavUpdatedItems_Level1Title
		DROP TABLE #NavUpdatedItems_Level2Title
		DROP TABLE #NavUpdatedItems_Level3Title
		DROP TABLE #NavUpdatedItems_Level4Title
		DROP TABLE #NavUpdatedItems_Level5Title
		DROP TABLE #NavUpdatedItems_Level6Title
	END
GO
----------------------------------------------------
-- dbo.Public_GetServerConfig
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetServerConfig
(
@ServerIP varchar(32)
)

AS

SELECT     dbo.System_ServerGroups.ServerGroup AS ServerGroupDescription, dbo.System_ServerGroups.ID_Group_Public, 
                      System_Servers_1.ServerProtocol AS MasterServerProtocol, System_Servers_1.ServerName AS MasterServerName, 
                      System_Servers_1.ServerPort AS MasterServerPort, System_Servers_2.ServerProtocol AS UserAdminServerProtocol, 
                      System_Servers_2.ServerName AS UserAdminServerName, System_Servers_2.ServerPort AS UserAdminServerPort, 
                      dbo.System_ServerGroups.UserAdminServer, System_Servers_3.*, dbo.System_ServerGroups.AreaImage AS ServerGroupImageBig, 
                      dbo.System_ServerGroups.AreaButton AS ServerGroupImageSmall, COALESCE (dbo.System_ServerGroups.AreaNavTitle, 
                      dbo.System_ServerGroups.ServerGroup) AS ServerGroupTitle_Navigation, dbo.System_ServerGroups.AreaCompanyFormerTitle, 
                      dbo.System_ServerGroups.AreaCompanyTitle, dbo.System_ServerGroups.AreaSecurityContactEMail, 
                      dbo.System_ServerGroups.AreaSecurityContactTitle, dbo.System_ServerGroups.AreaDevelopmentContactEMail, 
                      dbo.System_ServerGroups.AreaDevelopmentContactTitle, dbo.System_ServerGroups.AreaContentManagementContactEMail, 
                      dbo.System_ServerGroups.AreaContentManagementContactTitle, dbo.System_ServerGroups.AreaUnspecifiedContactEMail, 
                      dbo.System_ServerGroups.AreaUnspecifiedContactTitle, dbo.System_ServerGroups.AreaCopyRightSinceYear, 
                      dbo.System_ServerGroups.AreaCompanyWebSiteURL, dbo.System_ServerGroups.AreaCompanyWebSiteTitle, 
                      dbo.System_ServerGroups.ID AS ID_ServerGroup, dbo.System_ServerGroups.AccessLevel_Default
FROM         dbo.System_ServerGroups INNER JOIN
                      dbo.System_Servers System_Servers_2 ON dbo.System_ServerGroups.UserAdminServer = System_Servers_2.ID INNER JOIN
                      dbo.System_Servers System_Servers_1 ON dbo.System_ServerGroups.MasterServer = System_Servers_1.ID INNER JOIN
                      dbo.System_Servers System_Servers_3 ON dbo.System_ServerGroups.ID = System_Servers_3.ServerGroup

WHERE     (System_Servers_3.IP = @ServerIP)
GO
----------------------------------------------------
-- dbo.Public_GetToDoLogonList
----------------------------------------------------
ALTER PROCEDURE dbo.Public_GetToDoLogonList
	(
	@Username varchar(20),
	@ScriptEngine_SessionID nvarchar(512),
	@ScriptEngine_ID int
	)

AS

-- GUIDs alter Sessions zurücksetzen
SET NOCOUNT ON
UPDATE    dbo.System_WebAreasAuthorizedForSession
SET              Inactive = 1
WHERE     (LastSessionStateRefresh < DATEADD(hh, - 12, GETDATE()))

-- Logon-ToDo-Liste übergeben
SET NOCOUNT OFF
SELECT     System_WebAreasAuthorizedForSession.ID, System_WebAreasAuthorizedForSession.SessionID, System_Servers.IP, 
             
         System_Servers.ServerDescription, System_Servers.ServerProtocol, System_Servers.ServerName, System_Servers.ServerPort, 
                      System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID, System_ScriptEngines.EngineName, 
                      System_ScriptEngines.FileName_EngineLogin, System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     (System_WebAreasAuthorizedForSession.ScriptEngine_SessionID IS NULL) AND (System_Servers.ID > 0) AND 
		(System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND 
		(Benutzer.Loginname = @Username)
	 OR
                (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID IS NOT NULL) AND (System_Servers.ID > 0) AND 
		(Benutzer.Loginname = @Username) AND 
-- following line might not make sense
		-- (System_WebAreasAuthorizedForSession.ScriptEngine_SessionID = @ScriptEngine_SessionID) AND (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND
-- therefore here is a replacement
		(System_WebAreasAuthorizedForSession.ScriptEngine_ID <> @ScriptEngine_ID) AND
-- end following non sense block
                      (System_WebAreasAuthorizedForSession.LastSessionStateRefresh < DATEADD(minute, - 3, GETDATE()))
GO
----------------------------------------------------
-- dbo.Public_GetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserDetailData
	(
		@IDUser int,
		@Type varchar(50)
	)

As

If @Type = 'Sex'
	SELECT CASE WHEN Anrede = 'Mr.' THEN 'm' Else 'w' END As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LoginName'
	SELECT LoginName As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'Addresses'
	SELECT Anrede As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'LastName'
	SELECT Nachname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'FirstName'
	SELECT Vorname As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '1stPreferredLanguage'
	SELECT [1stPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '2ndPreferredLanguage'
	SELECT [2ndPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = '3rdPreferredLanguage'
	SELECT [3rdPreferredLanguage] As Result FROM Benutzer WHERE ID = @IDUser
Else If @Type = 'AccessLevel'
	SELECT [AccountAccessability] As Result FROM Benutzer WHERE ID = @IDUser
Else
	SELECT Value As Result FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type

GO
----------------------------------------------------
-- dbo.Public_GetUserID
----------------------------------------------------
ALTER Procedure dbo.Public_GetUserID
(
		@Username varchar(20)
)

As
declare @UserID int

set nocount on
SELECT TOP 1 @UserID = ID FROM dbo.Benutzer WHERE Loginname = @Username
set nocount off

SELECT Result = @UserID

Return @UserID
GO
----------------------------------------------------
-- dbo.Public_Logout
----------------------------------------------------
ALTER PROCEDURE dbo.Public_Logout 
(
	@Username varchar(20),
	@ServerIP varchar(32),
	@RemoteIP varchar(32)
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END

-------------
-- Logout --
-------------
-- Rückgabewert
SELECT Result = -1
-- CurUserCurrentLoginViaRemoteIP und SessionIDs zurücksetzen
UPDATE dbo.Benutzer 
SET CurrentLoginViaRemoteIP = Null, System_SessionID = Null WHERE LoginName = @Username
-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 99, 'Logout')


GO
----------------------------------------------------
-- dbo.Public_RestorePassword
----------------------------------------------------
ALTER Procedure dbo.Public_RestorePassword
(
		@Username varchar(20),
		@eMail varchar(50)
)

As
SELECT Result = (SELECT SUBSTRING(LoginPW, 1, len(LoginPW)) FROM dbo.Benutzer WHERE Loginname = @Username And [e-mail] = @eMail)
	/* set nocount on */
	return 


GO
----------------------------------------------------
-- dbo.Public_SetUserDetailData
----------------------------------------------------
ALTER Procedure dbo.Public_SetUserDetailData
	(
		@IDUser int,
		@Type varchar(50),
		@Value nvarchar(255),
		@DoNotLogSuccess bit = 0
	)

As
DECLARE @CountOfValuesInTable int

	set nocount on
	-- How many rows exist?
	SET @CountOfValuesInTable = (SELECT COUNT(ID) FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type)
	
	If @CountOfValuesInTable > 0 
		-- Remove old settings first
		DELETE FROM dbo.Log_Users WHERE ID_User = @IDUser AND Type = @Type
	-- Append value to table
	If @Type Is Not Null And @Value Is Not Null 
		INSERT INTO dbo.Log_Users (ID_User, Type, Value) VALUES (@IDUser, @Type, @Value) 
	-- Logging
	if @DoNotLogSuccess = 0 
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@IDUser, GetDate(), '0.0.0.0', '0.0.0.0', -9, 'User attributes modified')
	-- Exit
	set nocount off
	SELECT -1
	return
GO
----------------------------------------------------
-- dbo.Public_UpdateUserDetails
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UpdateUserDetails 
(
	@Username varchar(20),
	@Passcode varchar(4096),
	@ServerIP varchar(32),
	@RemoteIP varchar(32),
	@WebApplication varchar(1024),
	@Company nvarchar(50),
	@Anrede varchar(20),
	@Titel nvarchar(20),
	@Vorname nvarchar(30),
	@Nachname nvarchar(30),
	@Namenszusatz nvarchar(20),
	@eMail varchar(30),
	@Strasse nvarchar(30),
	@PLZ nvarchar(10),
	@Ort nvarchar(50),
	@State nvarchar(30),
	@Land varchar(30),
	@1stPreferredLanguage int,
	@2ndPreferredLanguage int,
	@3rdPreferredLanguage int,
	@CustomerNo nvarchar(50) = Null,
	@SupplierNo nvarchar(50) = Null
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @Passcode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@Passcode)
			BEGIN
				IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET Company = @Company, Anrede = @Anrede, Titel = @Titel, Vorname = @Vorname, Nachname = @Nachname, Namenszusatz = @Namenszusatz, [e-mail] = @eMail, Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort, State = @State, Land = @Land, ModifiedOn = GetDate(), [1stPreferredLanguage] = @1stPreferredLanguage, [2ndPreferredLanguage] = @2ndPreferredLanguage, [3rdPreferredLanguage] = @3rdPreferredLanguage, CustomerNo = @CustomerNo, SupplierNo = @SupplierNo WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -4, 'User profile changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0
-- Write UserDetails
Exec Int_UpdateUserDetailDataWithProfileData @CurUserID

GO
----------------------------------------------------
-- dbo.Public_UpdateUserPW
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UpdateUserPW 
(
	@Username varchar(20),
	@OldPasscode varchar(4096),
	@NewPasscode varchar(4096),
	@ServerIP varchar(32),
	@RemoteIP varchar(32),
	@WebApplication varchar(4096)
)

AS
-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- minutes
DECLARE @bufferLastLoginOn as datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
-- Konstanten setzen
SET @MaxLoginFailures = 3
SET @LoginFailureDelayHours = 3
SET @WebSessionTimeOut = 15 
-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen
SELECT @CurUserID = (select ID from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserPW = (select LoginPW from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginDisabled = (select LoginDisabled from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginLockedTill = (select LoginLockedTill from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginFailures = (select LoginFailures from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserLoginCount = (select LoginCount from dbo.Benutzer where LoginName = @Username)
SELECT @CurUserAccountAccessability = (select AccountAccessability from dbo.Benutzer where LoginName = @Username)
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SELECT Result = -3
		-- Abbruch
		Return
	END
-- Password validation and update
If @CurUserPW = @OldPasscode 
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnt die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@OldPasscode)
			BEGIN
				IF ASCII(SUBSTRING(@OldPasscode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @MyResult = 0
						BREAK
					END
				ELSE
					SET @MyResult = 1
				SET @position = @position + 1
			END
		IF @MyResult = 1
			-- Validation successfull, password will be updated now
			BEGIN
				-- Rückgabewert
				SELECT Result = -1
				-- Password update
				UPDATE dbo.Benutzer SET LoginPW = @NewPasscode, ModifiedOn = GetDate() WHERE LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 6, 'User password changed by user himself')
			END
		ELSE
			-- Rückgabewert
			SELECT Result = 0
	END
Else
	-- Rückgabewert
	SELECT Result = 0

GO
----------------------------------------------------
-- dbo.Public_UserIsAuthorizedForApp
----------------------------------------------------
ALTER PROCEDURE dbo.Public_UserIsAuthorizedForApp
(
	@Username varchar(20),
	@WebApplication varchar(255),
	@ServerIP varchar(32)
)

AS 

DECLARE @CurUserID int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByAnonymousGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @WebAppID int
DECLARE @RequestedServerID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@ServerIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		-- Abbruch
		Return 0
	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID)))


------------------------------
-- UserLoginValidierung --
------------------------------

		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT @bufferUserIDByAnonymousGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID))
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @PublicGroupID))
		SELECT @bufferUserIDByUser = (SELECT DISTINCT ID_User FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_User = @CurUserID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships.ID_User FROM Memberships INNER JOIN view_ApplicationRights ON Memberships.ID_Group = view_ApplicationRights.ID_Group WHERE (view_ApplicationRights.Title = @WebApplication) AND (view_ApplicationRights.LocationID = @RequestedServerID) AND (Memberships.ID_User = @CurUserID))
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6))
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1 Or NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByUser, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
			Return 1 -- Zugriff gewährt
		Else
			Return 0 -- kein Zugriff auf aktuelles Dokument
GO
----------------------------------------------------
-- dbo.Public_ValidateDocument
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateDocument
	@Username varchar(20),
	@ServerIP varchar(32),
	@RemoteIP varchar(32),
	@WebApplication varchar(1024),
	@WebURL varchar(1024),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@Reserved int = Null

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime

DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @CurUserCurrentLoginViaRemoteIP varchar(32)
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @Dummy bit
DECLARE @bufferUserIDByAnonymousGroup int
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
DECLARE @LocationID int 	-- ServerGroup
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Registered_ScriptEngine_SessionID varchar(512)
DECLARE @RequestedServerID int
DECLARE @WebAppID int
DECLARE @LoggingSuccess_Disabled bit

SET NOCOUNT ON

-- Konstanten setzen
SET @MaxLoginFailures = 3

-- Reserved-Parameter auswerten
IF @Reserved = 1
	SELECT @LoggingSuccess_Disabled = 1
ELSE
	SELECT @LoggingSuccess_Disabled = 0

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn
FROM dbo.Benutzer 
WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@WebApplication,'') = '')
	-- No application title --> anonymous access allowed
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, Null As LoginName, Null As System_SessionID
		-- Abbruch
		Return
	END
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_ServerGroups.ID, @RequestedServerID = dbo.System_Servers.ID
FROM         dbo.System_Servers INNER JOIN
                      dbo.System_ServerGroups ON dbo.System_Servers.ServerGroup = dbo.System_ServerGroups.ID
WHERE     (dbo.System_Servers.IP = @ServerIP AND dbo.System_Servers.Enabled <> 0)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return

	END

----------------------------------------------------------------
-- WebAppID ermitteln für ordentliche Protokollierung --
----------------------------------------------------------------
SELECT @WebAppID = (select top 1 ID from Applications where ((Applications.Title = @WebApplication) AND (Applications.LocationID = @RequestedServerID)))
If @WebAppID Is Null And @WebApplication Not Like 'Public'
	BEGIN
		SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
		RETURN
	END

--------------------------------------------------
-- Anonyme Userberechtigungen checken --
--------------------------------------------------
If (IsNull(@Username,'') = '')
	BEGIN
		-- Is Application available for anonymous access?
		SELECT @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT @bufferUserIDByAnonymousGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID))
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1
			-- Zugriff gewährt
			BEGIN
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (-1, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, 0)
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -1, Null As LoginName, Null As System_SessionID
				-- Abbruch
				Return
			END
		Else
			-- Login required
			BEGIN
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (-1, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, -25)
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = 58
				-- Abbruch
				Return
			END
	END

--------------------------------------------------
-- Server-Zugriff durch Benutzer erlaubt? --
--------------------------------------------------
If @CurUserAccountAccessability Is Null
	-- Benutzer nicht gefunden
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 43
		-- Abbruch
		Return
	END
SELECT     @ServerIsAccessable = COUNT(*)
	FROM         System_ServerGroupsAndTheirUserAccessLevels INNER JOIN       System_Servers ON System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = System_Servers.ServerGroup
	WHERE     (System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = @CurUserAccountAccessability) AND (System_Servers.IP = @ServerIP)
If @ServerIsAccessable Is Null 
	SELECT @ServerIsAccessable = 0
If @ServerIsAccessable = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -9
		-- Abbruch
		Return
	END

------------------------------------------------------------------------
-- Überprüfung, ob bereits (an anderer Station) angemeldet --
------------------------------------------------------------------------
SELECT @WebSessionTimeOut = dbo.System_Servers.WebSessionTimeOut, @ReAuthByIPPossible = dbo.System_Servers.ReAuthenticateByIP, @LoginFailureDelayHours = dbo.System_Servers.LockTimeout
	FROM dbo.System_Servers
	WHERE dbo.System_Servers.IP = @ServerIP
If dateadd(minute,  + @WebSessionTimeOut, @bufferLastLoginOn) > GetDate() And (@CurUserStatus_InternalSessionID Is Not Null)
	SELECT @CurrentlyLoggedOn = 1

---------------------------------------------------------------------------------
-- Versuch der Reauthentifizierung durch die mitgelieferte SessionID --
---------------------------------------------------------------------------------
SELECT @ReAuthSuccessfull = 0 -- Variablen-Initialisierung
SELECT @bufferLastLoginRemoteIP = (select LastLoginViaRemoteIP from dbo.Benutzer where LoginName = @Username)
SELECT     @Registered_ScriptEngine_SessionID = System_WebAreasAuthorizedForSession.ScriptEngine_SessionID
	FROM         Benutzer INNER JOIN
                      System_WebAreasAuthorizedForSession ON Benutzer.System_SessionID = System_WebAreasAuthorizedForSession.SessionID
	WHERE     (Benutzer.ID = @CurUserID) AND (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND ScriptEngine_SessionID = @ScriptEngine_SessionID
If @Registered_ScriptEngine_SessionID = @ScriptEngine_SessionID
	SELECT @ReAuthSuccessfull = 1
If @ReAuthByIPPossible <> 0 And @ReAuthSuccessfull = 0
	SELECT @ReAuthSuccessfull = 0
If @CurrentlyLoggedOn = 1 And @ReAuthSuccessfull = 0
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, -98, 'Currently logged in on host ' + @bufferLastLoginRemoteIP + ' or with a different session ID, CurrentlyLoggedOn = ' + Cast(@CurrentlyLoggedOn as varchar(30)) + ', ReAuthSuccessfull = ' + Cast(@ReAuthSuccessfull as varchar(30)) + ', Login denied')
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -4
		-- Abbruch
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------
If (@CurUserLoginDisabled = 1)
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, -28, 'Account disabled')
		-- Konto gesperrt - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 44
		Return
	END
If  @CurUserLoginLockedTill > GetDate()
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, -29, 'Account locked temporary')
		-- LoginSperre aktiv - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------

-- ReAuthentifizierung?
If @ReAuthSuccessfull = 1
	-- Does the user has got authorization?
	BEGIN
		SELECT @PublicGroupID = dbo.System_ServerGroups.ID_Group_Public FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @PublicGroupID Is Null 
			SELECT @PublicGroupID = 0
		SELECT @AnonymousGroupID = dbo.System_ServerGroups.ID_Group_Anonymous FROM dbo.System_ServerGroups INNER JOIN dbo.System_Servers ON dbo.System_ServerGroups.ID = dbo.System_Servers.ServerGroup WHERE system_servers.ip = @ServerIP
		If @AnonymousGroupID Is Null 
			SELECT @AnonymousGroupID = 0
		SELECT @bufferUserIDByAnonymousGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @AnonymousGroupID))
		SELECT @bufferUserIDByPublicGroup = (SELECT DISTINCT ID_Group FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_Group = @PublicGroupID))
		SELECT @bufferUserIDByUser = (SELECT DISTINCT ID_User FROM view_ApplicationRights WHERE (Title = @WebApplication) AND (LocationID = @RequestedServerID) AND (ID_User = @CurUserID))
		SELECT @bufferUserIDByGroup = (SELECT DISTINCT Memberships.ID_User FROM Memberships INNER JOIN view_ApplicationRights ON Memberships.ID_Group = view_ApplicationRights.ID_Group WHERE (view_ApplicationRights.Title = @WebApplication) AND (view_ApplicationRights.LocationID = @RequestedServerID) AND (Memberships.ID_User = @CurUserID))
		SELECT @bufferUserIDByAdmin = (SELECT DISTINCT ID_User FROM Memberships WHERE (ID_User = @CurUserID) AND (ID_Group = 6))
		If NullIf(@bufferUserIDByAnonymousGroup, -1) <> -1 Or NullIf(@bufferUserIDByPublicGroup, -1) <> -1 Or NullIf(@bufferUserIDByUser, -1) <> -1 Or NullIf(@bufferUserIDByGroup, -1) <> -1 Or NullIf(@bufferUserIDByAdmin, -1) <> -1 Or NullIf(@WebApplication, '') = 'Public'
			SET @MyResult = 1 -- Zugriff gewährt
		Else
			SET @MyResult = 2 -- kein Zugriff auf aktuelles Dokument
	END
Else
	SET @MyResult = 0 -- Reauthentifizierung schlug fehl - Neuanmeldung erforderlich

IF @MyResult = 1
	-- Login successfull
	BEGIN
		-- LoginRemoteIP und SessionIDs setzen
		update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), CurrentLoginViaRemoteIP = @RemoteIP where LoginName = @Username --, SessionID_ASP = @CurUserStatus_SessionID_ASP, SessionID_ASPX = @CurUserStatus_SessionID_ASPX, SessionID_SAP = @CurUserStatus_SessionID_SAP 
		-- LoginCount hochzählen
		If @LoggingSuccess_Disabled = 0 
			update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1 where LoginName = @Username
		-- LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		If @LoggingSuccess_Disabled = 0 
			insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, 0)
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		SELECT @CurUserStatus_InternalSessionID = System_SessionID 
		FROM dbo.Benutzer 
		WHERE LoginName = @Username
		SELECT @CurrentlyLoggedOn = 0
		-- WebAreaSessionState aktualisieren
		update dbo.System_WebAreasAuthorizedForSession set LastSessionStateRefresh = getdate() where ScriptEngine_ID = @ScriptEngine_ID and SessionID = @CurUserStatus_InternalSessionID And Server = @RequestedServerID
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, LoginName, @CurUserStatus_InternalSessionID As System_SessionID
		FROM dbo.Benutzer
		WHERE LoginName = @Username
		SET NOCOUNT ON
	END
Else -- @MyResult = 0 Or @MyResult = 2
	-- Login failed
	BEGIN
		IF @CurUserLoginFailures = @MaxLoginFailures - 1
			-- LoginFailure Maximum nun erreicht
			BEGIN	
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -2	
				SET NOCOUNT ON
				-- Zeitliche Loginsperre setzen
				update dbo.Benutzer set LoginLockedTill = getdate() + 1.0/24*@LoginFailureDelayHours where LoginName = @Username
			END	
		Else
			BEGIN
				SET NOCOUNT OFF
				If @MyResult = 0 -- Weitere Logins möglich - Rückgabewert
					SELECT Result = 57	 -- Reauthentifizierung schlug fehl - Neuanmeldung erforderlich
				Else
					SELECT Result = -5	 -- kein Zugriff auf aktuelles Dokument
				SET NOCOUNT ON
			END
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ApplicationID, URL, ConflictType) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, @WebAppID, @WebURL, -27)
	END

-- Wenn @CurUserLoginLockedTill vorhanden und älter als aktuelles Systemdatum, LoginFailureFields zurücksetzen
If Not (@CurUserLoginLockedTill > GetDate())

	If @MyResult = 1 Or @MyResult = 2
		-- Reauth-check successful --> LoginFailures = 0
		-- (Access may be granted or not - the password check has been successfull and so there aren no really LoginFailures;
		-- if you would say it is one, then the user would try to access a locked modul 3 times and after this he would be locked by the system...)
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
GO
----------------------------------------------------
-- dbo.Public_ValidateGUIDLogin
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateGUIDLogin
(
	@Username varchar(20),
	@GUID int,
	@ServerIP varchar(32),
	@RemoteIP varchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512)
)

AS

DECLARE @CurUserID int

SET NOCOUNT ON

SELECT @CurUserID = ID FROM dbo.Benutzer WHERE LoginName = @Username

If @CurUserID Is Null
	-- User does not exist
	Return 1

UPDATE System_WebAreasAuthorizedForSession SET ScriptEngine_SessionID = @ScriptEngine_SessionID, LastSessionStateRefresh = GetDate()
FROM         System_WebAreasAuthorizedForSession INNER JOIN
                      System_Servers ON System_WebAreasAuthorizedForSession.Server = System_Servers.ID INNER JOIN
                      System_ScriptEngines ON System_WebAreasAuthorizedForSession.ScriptEngine_ID = System_ScriptEngines.ID INNER JOIN
                      Benutzer ON System_WebAreasAuthorizedForSession.SessionID = Benutzer.System_SessionID
WHERE     
                      (System_WebAreasAuthorizedForSession.ScriptEngine_LogonGUID = @GUID) AND (Benutzer.Loginname = @Username) AND 
                      (System_WebAreasAuthorizedForSession.ScriptEngine_ID = @ScriptEngine_ID) AND (System_Servers.IP = @ServerIP)

IF @@ROWCOUNT = 1 
	insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
	values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 97, 'Prepare GUID login - Script Engine #' + Cast(@ScriptEngine_ID As Varchar(20)))
Else
	-- User has no such ScriptEngine_SessionID
	Return 2

SET NOCOUNT OFF

-- Erfolgsmitteilung
Select @UserName
Return -1
GO
----------------------------------------------------
-- dbo.Public_ValidateUser
----------------------------------------------------
ALTER PROCEDURE dbo.Public_ValidateUser
(
	@Username varchar(20),
	@Passcode varchar(4096),
	@ServerIP varchar(32),
	@RemoteIP varchar(32),
	@ScriptEngine_ID int,
	@ScriptEngine_SessionID nvarchar(512),
	@ForceLogin bit
)

AS

-- Deklaration Variablen/Konstanten
DECLARE @CurUserID int
DECLARE @CurUserPW varchar(4096)
DECLARE @CurUserLoginDisabled bit
DECLARE @CurUserLoginLockedTill datetime
DECLARE @CurUserLoginFailures tinyint
DECLARE @CurUserLoginCount int
DECLARE @CurUserCurrentLoginViaRemoteIP varchar(32)
DECLARE @MaxLoginFailures tinyint
DECLARE @CurUserAccountAccessability tinyint
DECLARE @LoginFailureDelayHours float
DECLARE @position smallint
DECLARE @MyResult smallint
DECLARE @bufferUserIDByPublicGroup int
DECLARE @bufferUserIDByUser int
DECLARE @bufferUserIDByGroup int
DECLARE @bufferUserIDByAdmin int
DECLARE @WebSessionTimeOut int -- in minutes
DECLARE @bufferLastLoginOn datetime
DECLARE @bufferLastLoginRemoteIP varchar(32)
DECLARE @LocationID int		-- ServerGroup
DECLARE @ServerID int
DECLARE @PublicGroupID int
DECLARE @ServerIsAccessable int
DECLARE @CurrentlyLoggedOn bit
DECLARE @ReAuthByIPPossible bit
DECLARE @ReAuthSuccessfull bit
DECLARE @PasswordAuthSuccessfull bit
DECLARE @CurUserStatus_InternalSessionID int
DECLARE @Logged_ScriptEngine_SessionID varchar(512)

-- Konstanten setzen
SET @MaxLoginFailures = 7

-- Wertzuweisungen Benutzereigenschaften --> lokale Variablen

SET NOCOUNT ON

SELECT @CurUserID = ID, @CurUserPW = LoginPW, @CurUserLoginDisabled = LoginDisabled, @CurUserLoginLockedTill = LoginLockedTill, 
		@CurUserLoginFailures = LoginFailures, @CurUserLoginCount = LoginCount, @CurUserAccountAccessability = AccountAccessability,
		@bufferLastLoginOn = LastLoginOn, @bufferLastLoginRemoteIP = LastLoginViaRemoteIP
FROM dbo.Benutzer 
WHERE LoginName = @Username

-------------------------------------------------------------------------------------------------------------------------
-- Überprüfung auf Vollständigkeit übergebener Parameter, die sonst nicht mehr weiter geprüft werden --
-------------------------------------------------------------------------------------------------------------------------
If (IsNull(@Username,'') = '')
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 58
		-- Abbruch
		Return
	END
If (IsNull(@ServerIP,'') = '') Or (IsNull(@RemoteIP,'') = '') Or (IsNull(@ScriptEngine_SessionID,'') = '') Or (IsNull(@ScriptEngine_ID,0) = 0)
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -3
		-- Abbruch
		Return
	END
If @CurUserAccountAccessability Is Null
	-- Benutzer nicht gefunden
	BEGIN
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 43
		-- Abbruch
		Return
	END

----------------------------------
-- ServerGroup bestimmen --
----------------------------------
SELECT   @LocationID = dbo.System_Servers.ServerGroup, @ServerID = ID
FROM         dbo.System_Servers
WHERE     (dbo.System_Servers.IP = @ServerIP)
IF @LocationID Is Null 
	SELECT @LocationID = 0
If @LocationID = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -10
		-- Abbruch
		Return
	END

--------------------------------------------------
-- Server-Zugriff durch Benutzer erlaubt? --
--------------------------------------------------
SELECT     @ServerIsAccessable = COUNT(*)
	FROM         System_ServerGroupsAndTheirUserAccessLevels INNER JOIN
                      System_Servers ON System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = System_Servers.ServerGroup
	WHERE     (System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = @CurUserAccountAccessability) AND (System_Servers.IP = @ServerIP)
If @ServerIsAccessable Is Null 
	SELECT @ServerIsAccessable = 0
If @ServerIsAccessable = 0
	-- Nicht authentifizierter Zugang über den aktuell gewählten Server
	BEGIN
		-- Rückgabewert setzen
		SET NOCOUNT OFF
		SELECT Result = -9
		-- Abbruch
		Return
	END

------------------------------------------------------------------------
-- Überprüfung, ob bereits (an anderer Station) angemeldet --
------------------------------------------------------------------------
SELECT @WebSessionTimeOut = dbo.System_Servers.WebSessionTimeOut, @ReAuthByIPPossible = dbo.System_Servers.ReAuthenticateByIP, @LoginFailureDelayHours = dbo.System_Servers.LockTimeout
	FROM dbo.System_Servers
	WHERE dbo.System_Servers.IP = @ServerIP
If dateadd(minute,  + @WebSessionTimeOut, @bufferLastLoginOn) > GetDate() 
	SELECT @CurrentlyLoggedOn = 1
If @CurrentlyLoggedOn = 1
	-- Anmeldung vorhanden, jedoch evtl. an der gleichen Station (vergleiche mit SessionID) und dann genehmigt
	BEGIN
		SELECT @CurUserStatus_InternalSessionID = System_SessionID FROM dbo.Benutzer WHERE LoginName = @UserName
		If @CurUserStatus_InternalSessionID Is Not Null 
			BEGIN
				-- Stimmt die übermittelte SessionID der ScriptSprache mit der protokollierten überein?
				select @Logged_ScriptEngine_SessionID = scriptengine_sessionid from System_WebAreasAuthorizedForSession where sessionid=@CurUserStatus_InternalSessionID and scriptengine_id = @ScriptEngine_ID -- and server=@ServerID 
				IF @Logged_ScriptEngine_SessionID Is Not Null 
					IF @Logged_ScriptEngine_SessionID = @ScriptEngine_SessionID 
						-- Anmeldung mit gleicher Session erlaubt
						SELECT @CurrentlyLoggedOn = 0
					Else
						-- Anmeldung bereits von anderer Session vorliegend
						IF @ForceLogin <> 0 
							SELECT @CurrentlyLoggedOn = 0 	--Attribut ForceLogin wurde mitgegeben, Anmeldung erfolgt!
						Else
							SELECT @CurrentlyLoggedOn = 1 	--Standardlogin ohne ForceLogin - Login derzeit noch nicht gewährt
				Else
					-- Session-Anmeldungseintrag nicht (mehr) vorhanden
					SELECT @CurrentlyLoggedOn = 0
	
			END
		Else
			-- sollte eigentlich nicht vorkommen, falls aber doch...lass eine Übernahme der Anmeldung zu...
			SET @CurrentlyLoggedOn = 0
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) 
			values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -98, 'Already logged on (TimeDiff): CurUserStatus_InternalSessionID: ' +  cast(@CurUserStatus_InternalSessionID as nvarchar(10)))
	END
If @CurrentlyLoggedOn = 1
	-- Abbruch der Authentifizierung
	BEGIN
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -98, 'Currently logged in on host ' + @bufferLastLoginRemoteIP + ' or with a different session ID, CurrentlyLoggedOn = ' + Cast(@CurrentlyLoggedOn as varchar(30)) + ', Login denied')
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -4, LastRemoteIP = @bufferLastLoginRemoteIP
		-- Abbruch
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------
If (@CurUserLoginDisabled = 1)
	BEGIN
		-- Konto gesperrt - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = 44
		Return
	END
If  @CurUserLoginLockedTill > GetDate()
	BEGIN
		-- LoginSperre aktiv - Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -2
		Return
	END

------------------------------
-- UserLoginValidierung --
------------------------------

-- Passwortvergleich starten
SET @PasswordAuthSuccessfull = 0
If (@CurUserPW = @Passcode)
	-- Passwörter bis auf Groß-/Kleinschreibung schon mal identisch
	BEGIN
		-- Okay, jetzt sind die Strings schon mal gleich lang und die einzelnen Buchstaben grundsätzlich gleich,
		-- jedoch könnte die Groß- und Kleinschreibung derzeit noch unterschiedlich sein
		SET @position = 0
		WHILE @position <= DATALENGTH(@Passcode)
			BEGIN
				IF ASCII(SUBSTRING(@Passcode, @position+1, 1)) <> ASCII(SUBSTRING(@CurUserPW, @position+1, 1)) 
					BEGIN
						SET @PasswordAuthSuccessfull = 0
					BREAK
					END
				ELSE
					SET @PasswordAuthSuccessfull = 1
				SET @position = @position + 1
			END
	END

-- Passwortvergleich erfolgreich?
If @PasswordAuthSuccessfull = 1
	SET @MyResult = 1 -- Zugriff gewährt
Else
	SET @MyResult = 0 -- Passwortvergleich ergab Unterschiede


IF @MyResult = 1
	-- Login successfull
	BEGIN

		-- Wenn @CurUserLoginLockedTill vorhanden und älter als aktuelles Systemdatum, LoginFailureFields zurücksetzen
		If Not (@CurUserLoginLockedTill > GetDate())
			-- Password check successful --> LoginFailures = 0
			update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- LoginRemoteIP 
		update dbo.Benutzer set LastLoginViaRemoteIP = @RemoteIP, LastLoginOn = GetDate(), CurrentLoginViaRemoteIP = @RemoteIP where LoginName = @Username
		-- LoginCount hochzählen
		update dbo.Benutzer set LoginCount = @CurUserLoginCount + 1 where LoginName = @Username
		-- LoginFailureFields zurücksetzen
		update dbo.Benutzer set LoginFailures = 0, LoginLockedTill = Null where LoginName = @Username
		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, 98, 'Validation of login data successfull')
		-- Interne SessionID erstellen
		INSERT INTO System_UserSessions (ID_User) VALUES (@CurUserID)
		SELECT @CurUserStatus_InternalSessionID = @@IDENTITY
		UPDATE dbo.Benutzer SET System_SessionID = @CurUserStatus_InternalSessionID WHERE LoginName = @UserName
		-- An welchen Systemen ist noch eine Anmeldung erforderlich?
		INSERT INTO dbo.System_WebAreasAuthorizedForSession
		                      (ServerGroup, Server, ScriptEngine_ID, SessionID, ScriptEngine_LogonGUID)
		SELECT     dbo.System_Servers.ServerGroup, dbo.System_WebAreaScriptEnginesAuthorization.Server, 
		                      dbo.System_WebAreaScriptEnginesAuthorization.ScriptEngine, @CurUserStatus_InternalSessionID AS InternalSessionID, cast(rand() * 1000000000 as int) AS RandomGUID
		FROM         dbo.System_Servers INNER JOIN
		                      dbo.System_WebAreaScriptEnginesAuthorization ON dbo.System_Servers.ID = dbo.System_WebAreaScriptEnginesAuthorization.Server
		WHERE     (dbo.System_Servers.Enabled <> 0) AND (dbo.System_Servers.ServerGroup = @LocationID)
		-- Rückgabewert
		SET NOCOUNT OFF
		SELECT Result = -1, LoginName, System_SessionID FROM dbo.Benutzer WHERE LoginName = @Username
		SET NOCOUNT ON
	END
Else -- @MyResult = 0
	-- Login failed
	BEGIN
		IF @CurUserLoginFailures = @MaxLoginFailures - 1
			-- LoginFailure Maximum nun erreicht
			BEGIN	
				-- Rückgabewert
				SET NOCOUNT OFF
				SELECT Result = -2	
				SET NOCOUNT ON
				-- Zeitliche Loginsperre setzen
				update dbo.Benutzer set LoginLockedTill = dateadd(hour, @LoginFailureDelayHours, getdate()) where LoginName = @Username
				-- Logging
				insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -95, 'Login disabled for ' + cast(@LoginFailureDelayHours as nvarchar(5)) + ' hours')
			END	
		Else
			BEGIN
				SET NOCOUNT OFF
				If @MyResult = 0 -- Weitere Logins möglich - Rückgabewert
					SELECT Result = 0
				Else
					SELECT Result = -5
				SET NOCOUNT ON
			END
		-- Wert LoginFailures erhöhen 
		update dbo.Benutzer set LoginFailures = @CurUserLoginFailures + 1 where LoginName = @Username

		-- Logging
		insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@CurUserID, GetDate(), @ServerIP, @RemoteIP, -26, 'No valid login data')
	END
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteUser
	(
		@UserID int
	)

AS


DELETE FROM dbo.Benutzer WHERE ID=@UserID
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson=@UserID
DELETE FROM dbo.Memberships WHERE ID_User=@UserID

-- Logging
insert into dbo.Log (UserID, LoginDate, ServerIP, RemoteIP, ConflictType, ConflictDescription) values (@UserID, GetDate(), '0.0.0.0', '0.0.0.0', -31, 'User deleted by admin')
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByGroup
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteApplicationRightsByGroup
(
@AuthID int,
@ReleasedByUserID int
)

AS 

declare @groupID int
declare @AppID int
select top 1 @groupid = id_grouporperson, @appid = id_application from dbo.ApplicationsRightsByGroup where id = @AuthID

EXEC Int_LogAuthChanges @ReleasedByUserID, @GroupID, @AppID
DELETE FROM dbo.ApplicationsRightsByGroup WHERE     (ID_GroupOrPerson IS NOT NULL) AND ID=@AuthID
GO

----------------------------------------------------
-- dbo.AdminPrivate_DeleteApplicationRightsByUser
----------------------------------------------------
ALTER PROCEDURE dbo.AdminPrivate_DeleteApplicationRightsByUser
	(
		@AuthID int
	)

AS
declare @UserID int
declare @AppID int
select top 1 @userid = id_grouporperson, @appid = id_application from dbo.ApplicationsRightsByUser where id = @AuthID

EXEC Int_LogAuthChanges @UserID, Null, @AppID
DELETE FROM dbo.ApplicationsRightsByUser WHERE ID_GroupOrPerson Is Not Null And ID=@AuthID
GO

/******************************************************
  Stored procedures  End
******************************************************/