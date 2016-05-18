/****** Object:  Index [IX_Applications]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[ResetIsNewUpdatedStatusOn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Applications_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications_1' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications_1] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[LocationID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Applications_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications_2' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications_2] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[Sort] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Applications_CurrentAndInactiveOnes]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications_CurrentAndInactiveOnes' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[ReleasedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Applications_CurrentAndInactiveOnes_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications_CurrentAndInactiveOnes_1' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes_1] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[LocationID] ASC,
	[Title] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Applications_CurrentAndInactiveOnes_12]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications_CurrentAndInactiveOnes_12' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes_12] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Applications_CurrentAndInactiveOnes_13]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Applications_CurrentAndInactiveOnes_13' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_Applications_CurrentAndInactiveOnes_13] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[AuthsAsAppID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByGroup]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByGroup' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByGroup]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByGroup] ON [dbo].[ApplicationsRightsByGroup]
(
	[ID_Application] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByGroup_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByGroup_1' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByGroup]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByGroup_1] ON [dbo].[ApplicationsRightsByGroup]
(
	[ID_GroupOrPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByGroup_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByGroup_2' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByGroup]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByGroup_2] ON [dbo].[ApplicationsRightsByGroup]
(
	[ID_Application] ASC,
	[ID_GroupOrPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByGroup_3]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByGroup_3' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByGroup]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByGroup_3] ON [dbo].[ApplicationsRightsByGroup]
(
	[ID_GroupOrPerson] ASC,
	[ID_Application] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByUser]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByUser' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByUser]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByUser] ON [dbo].[ApplicationsRightsByUser]
(
	[ID_Application] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByUser_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByUser_1' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByUser]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByUser_1] ON [dbo].[ApplicationsRightsByUser]
(
	[ID_GroupOrPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByUser_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByUser_2' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByUser]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByUser_2] ON [dbo].[ApplicationsRightsByUser]
(
	[ID_Application] ASC,
	[ID_GroupOrPerson] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_ApplicationsRightsByUser_3]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ApplicationsRightsByUser_3' AND object_id = OBJECT_ID('[dbo].[ApplicationsRightsByUser]')) 
CREATE NONCLUSTERED INDEX [IX_ApplicationsRightsByUser_3] ON [dbo].[ApplicationsRightsByUser]
(
	[ID_GroupOrPerson] ASC,
	[ID_Application] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Benutzer_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Benutzer_1' AND object_id = OBJECT_ID('[dbo].[Benutzer]')) 
CREATE NONCLUSTERED INDEX [IX_Benutzer_1] ON [dbo].[Benutzer]
(
	[System_SessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log' AND object_id = OBJECT_ID('[dbo].[Log]')) 
CREATE NONCLUSTERED INDEX [IX_Log] ON [dbo].[Log]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='_dta_index_Log_5_1188199283__K8_1_3' AND object_id = OBJECT_ID('[dbo].[Log]')) 
CREATE NONCLUSTERED INDEX [_dta_index_Log_5_1188199283__K8_1_3] ON [dbo].[Log]
(
	[ConflictType] ASC
)
INCLUDE ( 	[ID],
	[LoginDate]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_1' AND object_id = OBJECT_ID('[dbo].[Log]')) 
CREATE NONCLUSTERED INDEX [IX_Log_1] ON [dbo].[Log]
(
	[LoginDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_2' AND object_id = OBJECT_ID('[dbo].[Log]')) 
CREATE NONCLUSTERED INDEX [IX_Log_2] ON [dbo].[Log]
(
	[ApplicationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_3]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_3' AND object_id = OBJECT_ID('[dbo].[Log]')) 
CREATE NONCLUSTERED INDEX [IX_Log_3] ON [dbo].[Log]
(
	[ConflictType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_eMailMessages]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_eMailMessages' AND object_id = OBJECT_ID('[dbo].[Log_eMailMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_eMailMessages] ON [dbo].[Log_eMailMessages]
(
	[State] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_eMailMessages_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_eMailMessages_1' AND object_id = OBJECT_ID('[dbo].[Log_eMailMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_eMailMessages_1] ON [dbo].[Log_eMailMessages]
(
	[DateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_eMailMessages_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_eMailMessages_2' AND object_id = OBJECT_ID('[dbo].[Log_eMailMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_eMailMessages_2] ON [dbo].[Log_eMailMessages]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_eMailMessages3]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_eMailMessages3' AND object_id = OBJECT_ID('[dbo].[Log_eMailMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_eMailMessages3] ON [dbo].[Log_eMailMessages]
(
	[State] ASC,
	[DateTime] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Log_EventsProcessed]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_EventsProcessed' AND object_id = OBJECT_ID('[dbo].[Log_EventsProcessed]')) 
CREATE NONCLUSTERED INDEX [IX_Log_EventsProcessed] ON [dbo].[Log_EventsProcessed]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Log_EventsProcessed2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_EventsProcessed2' AND object_id = OBJECT_ID('[dbo].[Log_EventsProcessed]')) 
CREATE NONCLUSTERED INDEX [IX_Log_EventsProcessed2] ON [dbo].[Log_EventsProcessed]
(
	[ValueInt] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Log_MissingAssignmentsOfExternalAccounts]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_MissingAssignmentsOfExternalAccounts' AND object_id = OBJECT_ID('[dbo].[Log_MissingAssignmentsOfExternalAccounts]')) 
CREATE UNIQUE NONCLUSTERED INDEX [IX_Log_MissingAssignmentsOfExternalAccounts] ON [dbo].[Log_MissingAssignmentsOfExternalAccounts]
(
	[ExternalAccountSystem] ASC,
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Log_TextMessages]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_TextMessages' AND object_id = OBJECT_ID('[dbo].[Log_TextMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_TextMessages] ON [dbo].[Log_TextMessages]
(
	[MessageCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_TextMessages_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_TextMessages_1' AND object_id = OBJECT_ID('[dbo].[Log_TextMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_TextMessages_1] ON [dbo].[Log_TextMessages]
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_TextMessages_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_TextMessages_2' AND object_id = OBJECT_ID('[dbo].[Log_TextMessages]')) 
CREATE NONCLUSTERED INDEX [IX_Log_TextMessages_2] ON [dbo].[Log_TextMessages]
(
	[GroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Log_Users]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_Users' AND object_id = OBJECT_ID('[dbo].[Log_Users]')) 
CREATE NONCLUSTERED INDEX [IX_Log_Users] ON [dbo].[Log_Users]
(
	[ID_User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Log_Users_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_Users_1' AND object_id = OBJECT_ID('[dbo].[Log_Users]')) 
CREATE NONCLUSTERED INDEX [IX_Log_Users_1] ON [dbo].[Log_Users]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Log_Users_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Log_Users_2' AND object_id = OBJECT_ID('[dbo].[Log_Users]')) 
CREATE NONCLUSTERED INDEX [IX_Log_Users_2] ON [dbo].[Log_Users]
(
	[ID_User] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Memberships]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Memberships' AND object_id = OBJECT_ID('[dbo].[Memberships]')) 
CREATE NONCLUSTERED INDEX [IX_Memberships] ON [dbo].[Memberships]
(
	[ID_Group] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Memberships_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Memberships_1' AND object_id = OBJECT_ID('[dbo].[Memberships]')) 
CREATE NONCLUSTERED INDEX [IX_Memberships_1] ON [dbo].[Memberships]
(
	[ID_User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Memberships_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Memberships_2' AND object_id = OBJECT_ID('[dbo].[Memberships]')) 
CREATE NONCLUSTERED INDEX [IX_Memberships_2] ON [dbo].[Memberships]
(
	[ID_Group] ASC,
	[ID_User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_Memberships_3]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Memberships_3' AND object_id = OBJECT_ID('[dbo].[Memberships]')) 
CREATE NONCLUSTERED INDEX [IX_Memberships_3] ON [dbo].[Memberships]
(
	[ID_User] ASC,
	[ID_Group] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_NavItems]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_NavItems' AND object_id = OBJECT_ID('[dbo].[NavItems]')) 
CREATE NONCLUSTERED INDEX [IX_NavItems] ON [dbo].[NavItems]
(
	[ID] ASC,
	[LanguageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_NavItems_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_NavItems_1' AND object_id = OBJECT_ID('[dbo].[NavItems]')) 
CREATE NONCLUSTERED INDEX [IX_NavItems_1] ON [dbo].[NavItems]
(
	[ID] ASC,
	[ServerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_SecurityObjects_vs_NavItems]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_SecurityObjects_vs_NavItems' AND object_id = OBJECT_ID('[dbo].[SecurityObjects_vs_NavItems]')) 
CREATE NONCLUSTERED INDEX [IX_SecurityObjects_vs_NavItems] ON [dbo].[SecurityObjects_vs_NavItems]
(
	[ID_SecurityObject] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_SecurityObjects_vs_NavItems_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_SecurityObjects_vs_NavItems_1' AND object_id = OBJECT_ID('[dbo].[SecurityObjects_vs_NavItems]')) 
CREATE NONCLUSTERED INDEX [IX_SecurityObjects_vs_NavItems_1] ON [dbo].[SecurityObjects_vs_NavItems]
(
	[ID_NavItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_System_GlobalProperties]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_GlobalProperties' AND object_id = OBJECT_ID('[dbo].[System_GlobalProperties]')) 
CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties] ON [dbo].[System_GlobalProperties]
(
	[PropertyName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_System_GlobalProperties_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_GlobalProperties_1' AND object_id = OBJECT_ID('[dbo].[System_GlobalProperties]')) 
CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_1] ON [dbo].[System_GlobalProperties]
(
	[ValueNVarChar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_System_GlobalProperties_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_GlobalProperties_2' AND object_id = OBJECT_ID('[dbo].[System_GlobalProperties]')) 
CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_2] ON [dbo].[System_GlobalProperties]
(
	[ValueInt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_System_GlobalProperties_3]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_GlobalProperties_3' AND object_id = OBJECT_ID('[dbo].[System_GlobalProperties]')) 
CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_3] ON [dbo].[System_GlobalProperties]
(
	[ValueDecimal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_System_GlobalProperties_4]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_GlobalProperties_4' AND object_id = OBJECT_ID('[dbo].[System_GlobalProperties]')) 
CREATE NONCLUSTERED INDEX [IX_System_GlobalProperties_4] ON [dbo].[System_GlobalProperties]
(
	[ValueDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_System_Servers_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_Servers_1' AND object_id = OBJECT_ID('[dbo].[System_Servers]')) 
CREATE NONCLUSTERED INDEX [IX_System_Servers_1] ON [dbo].[System_Servers]
(
	[IP] ASC,
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_System_WebAreasAuthorizedForSession]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_WebAreasAuthorizedForSession' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_System_WebAreasAuthorizedForSession] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
(
	[ScriptEngine_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_System_WebAreasAuthorizedForSession_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_WebAreasAuthorizedForSession_1' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_System_WebAreasAuthorizedForSession_1] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
(
	[LastSessionStateRefresh] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_System_WebAreasAuthorizedForSession_2]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_System_WebAreasAuthorizedForSession_2' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE NONCLUSTERED INDEX [IX_System_WebAreasAuthorizedForSession_2] ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]
(
	[ScriptEngine_SessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TextModules_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_TextModules_1' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE NONCLUSTERED INDEX [IX_TextModules_1] ON [dbo].[TextModules]
(
	[ServerGroupID] ASC,
	[MarketID] ASC,
	[WebsiteAreaID] ASC,
	[Key] ASC,
	[TypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** Object:  Index [IX_WebManager_DownloadHandler_Files_1]    Script Date: 02.07.2015 15:06:08 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_WebManager_DownloadHandler_Files_1' AND object_id = OBJECT_ID('[dbo].[WebManager_DownloadHandler_Files]')) 
CREATE NONCLUSTERED INDEX [IX_WebManager_DownloadHandler_Files_1] ON [dbo].[WebManager_DownloadHandler_Files]
(
	[ServerID] ASC,
	[TimeOfRemoval] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
/****** SEVERAL INDEXES BUILD IN YEAR 2010 ******/
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_2852' AND object_id = OBJECT_ID('[dbo].[Log_Users]')) 
CREATE INDEX missing_index_2852 ON [dbo].[Log_Users] ([Type], [Value], [ID_User])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_11' AND object_id = OBJECT_ID('[dbo].[Log_Users]')) 
CREATE INDEX missing_index_11 ON [dbo].[Log_Users] ([Value])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_15' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_15 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID], [ScriptEngine_ID], [Inactive])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_7' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_7 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID], [Inactive], [Server], [ScriptEngine_LogonGUID]) INCLUDE ([ID], [ScriptEngine_SessionID], [ScriptEngine_ID], [LastSessionStateRefresh])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_25' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_25 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID], [Server], [ScriptEngine_ID], [Inactive])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_21' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_21 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID], [ScriptEngine_ID]) INCLUDE ([ScriptEngine_SessionID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_1' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_1 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID], [ScriptEngine_LogonGUID], [ScriptEngine_ID], [Inactive]) INCLUDE ([ID], [Server], [ScriptEngine_SessionID], [LastSessionStateRefresh])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_3' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_3 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([ScriptEngine_LogonGUID], [ScriptEngine_ID], [Inactive])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_9' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_9 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([Inactive], [Server], [ScriptEngine_LogonGUID]) INCLUDE ([ID], [SessionID], [ScriptEngine_SessionID], [ScriptEngine_ID], [LastSessionStateRefresh])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_5' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_5 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([Inactive], [LastSessionStateRefresh]) INCLUDE ([ID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_17' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_17 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID]) INCLUDE ([ID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_23' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_23 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([ScriptEngine_ID]) INCLUDE ([SessionID], [ScriptEngine_SessionID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_19' AND object_id = OBJECT_ID('[dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_19 ON [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes] ([SessionID])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_48160' AND object_id = OBJECT_ID('[dbo].[TextModulesCache]')) 
CREATE INDEX missing_index_48160 ON [dbo].[TextModulesCache] ([MarketID], [WebsiteAreaID]) INCLUDE ([PrimaryID])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_39301' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_39301 ON [dbo].[TextModules] ([MarketID], [WebsiteAreaID]) INCLUDE ([ServerGroupID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_49131' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_49131 ON [dbo].[TextModules] ([Version]) INCLUDE ([MarketID], [ServerGroupID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_49508' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_49508 ON [dbo].[TextModules] ([WebsiteAreaID], [ServerGroupID], [Released])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_39299' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_39299 ON [dbo].[TextModules] ([WebsiteAreaID]) INCLUDE ([MarketID])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_50' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_50 ON [dbo].[Applications_CurrentAndInactiveOnes] ([AppDeleted]) INCLUDE ([Level4Title], [IsNew], [IsUpdated])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_48' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_48 ON [dbo].[Applications_CurrentAndInactiveOnes] ([AppDeleted]) INCLUDE ([Level3Title], [IsNew], [IsUpdated])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_44' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_44 ON [dbo].[Applications_CurrentAndInactiveOnes] ([AppDeleted]) INCLUDE ([Level1Title], [IsNew], [IsUpdated])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_46' AND object_id = OBJECT_ID('[dbo].[Applications_CurrentAndInactiveOnes]')) 
CREATE INDEX missing_index_46 ON [dbo].[Applications_CurrentAndInactiveOnes] ([AppDeleted]) INCLUDE ([Level2Title], [IsNew], [IsUpdated])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_40' AND object_id = OBJECT_ID('[dbo].[System_SessionValues]')) 
CREATE INDEX missing_index_40 ON [dbo].[System_SessionValues] ([SessionID])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_21307' AND object_id = OBJECT_ID('[dbo].[WebManager_WebEditor]')) 
CREATE INDEX missing_index_21307 ON [dbo].[WebManager_WebEditor] ([ServerID], [IsActive], [URL], [EditorID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_61' AND object_id = OBJECT_ID('[dbo].[WebManager_WebEditor]')) 
CREATE INDEX missing_index_61 ON [dbo].[WebManager_WebEditor] ([ServerID], [URL], [EditorID])

GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41748' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41748 ON [dbo].[TextModules] ([Key]) INCLUDE ([PrimaryID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41268' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41268 ON [dbo].[TextModules] ([Version]) INCLUDE ([MarketID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41423' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41423 ON [dbo].[TextModules] ([MarketID], [Released]) INCLUDE ([Key], [Version])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41280' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41280 ON [dbo].[TextModules] ([Version]) INCLUDE ([MarketID], [WebsiteAreaID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41777' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41777 ON [dbo].[TextModules] ([Version]) INCLUDE ([MarketID], [WebsiteAreaID], [ServerGroupID])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41488' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41488 ON [dbo].[TextModules] ([MarketID]) INCLUDE ([ServerGroupID], [Key], [Version], [Released])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41441' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41441 ON [dbo].[TextModules] ([MarketID], [Released]) INCLUDE ([ServerGroupID], [Key], [Version])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_42862' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_42862 ON [dbo].[TextModules] ([MarketID], [WebsiteAreaID], [ServerGroupID], [Released]) INCLUDE ([Key], [Version])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41476' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41476 ON [dbo].[TextModules] ([MarketID]) INCLUDE ([ServerGroupID], [Key], [Version])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_42843' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_42843 ON [dbo].[TextModules] ([MarketID], [ServerGroupID], [Released]) INCLUDE ([Key], [Version])
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='missing_index_41512' AND object_id = OBJECT_ID('[dbo].[TextModules]')) 
CREATE INDEX missing_index_41512 ON [dbo].[TextModules] ([MarketID], [ServerGroupID], [Key]) INCLUDE ([Version])


/****** Object:  Index [IX_Memberships]    Script Date: 07.04.2016 10:22:52 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MembershipsClones]') AND name = N'IX_MembershipsClones')
CREATE NONCLUSTERED INDEX [IX_MembershipsClones] ON [dbo].[MembershipsClones]
(
	[ID_Group] ASC
) ON [PRIMARY]
GO

/****** Object:  Index [IX_Memberships_1]    Script Date: 07.04.2016 10:22:52 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MembershipsClones]') AND name = N'IX_MembershipsClones_1')
CREATE NONCLUSTERED INDEX [IX_MembershipsClones_1] ON [dbo].[MembershipsClones]
(
	[ID_ClonedGroup] ASC
) ON [PRIMARY]
GO

/****** Object:  Index [IX_Memberships_2]    Script Date: 07.04.2016 10:22:52 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MembershipsClones]') AND name = N'IX_MembershipsClones_2')
CREATE NONCLUSTERED INDEX [IX_MembershipsClones_2] ON [dbo].[MembershipsClones]
(
	[ID_Group] ASC,
	[ID_ClonedGroup] ASC,
	IsDenyRule
) ON [PRIMARY]
GO

/****** Object:  Index [IX_Memberships_3]    Script Date: 07.04.2016 10:22:52 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MembershipsClones]') AND name = N'IX_MembershipsClones_3')
CREATE NONCLUSTERED INDEX [IX_MembershipsClones_3] ON [dbo].[MembershipsClones]
(
	[ID_ClonedGroup] ASC,
	[ID_Group] ASC,
	IsDenyRule
) ON [PRIMARY]
GO

-----------------------------------------------------------------------
-- ADDED DenyRule COLUMN TO INDEXES FOR [MembershipsClones]
-----------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Memberships]') AND name = N'IX_Memberships_3')
DROP INDEX IX_Memberships_3 ON dbo.Memberships
GO
CREATE NONCLUSTERED INDEX IX_Memberships_3 ON dbo.Memberships
	(
	ID_User,
	ID_Group,
	IsDenyRule
	) ON [PRIMARY]
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Memberships]') AND name = N'IX_Memberships_2')
DROP INDEX IX_Memberships_2 ON dbo.Memberships
GO
CREATE NONCLUSTERED INDEX IX_Memberships_2 ON dbo.Memberships
	(
	ID_Group,
	ID_User,
	IsDenyRule
	) ON [PRIMARY]
GO

-----------------------------------------------------------------------
-- ADDED DenyRule + ID_ServerGroup COLUMNs TO INDEXES FOR [ApplicationsRightsByUser]
-----------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser]') AND name = N'IX_ApplicationsRightsByUser_2')
DROP INDEX IX_ApplicationsRightsByUser_2 ON dbo.ApplicationsRightsByUser
GO
CREATE NONCLUSTERED INDEX IX_ApplicationsRightsByUser_2 ON dbo.ApplicationsRightsByUser
	(
	ID_Application,
	ID_GroupOrPerson,
	ID_ServerGroup,
	IsDenyRule
	) ON [PRIMARY]
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser]') AND name = N'IX_ApplicationsRightsByUser_3')
DROP INDEX IX_ApplicationsRightsByUser_3 ON dbo.ApplicationsRightsByUser
GO
CREATE NONCLUSTERED INDEX IX_ApplicationsRightsByUser_3 ON dbo.ApplicationsRightsByUser
	(
	ID_GroupOrPerson,
	ID_Application,
	ID_ServerGroup,
	IsDenyRule
	) ON [PRIMARY]
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser]') AND name = N'IX_ApplicationsRightsByUser_4')
DROP INDEX IX_ApplicationsRightsByUser_4 ON dbo.ApplicationsRightsByUser
GO
CREATE NONCLUSTERED INDEX IX_ApplicationsRightsByUser_4 ON dbo.ApplicationsRightsByUser
	(
	ID_ServerGroup
	) ON [PRIMARY]
GO

-----------------------------------------------------------------------
-- ADDED DenyRule + ID_ServerGroup COLUMNs TO INDEXES FOR [ApplicationsRightsByGroup]
-----------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup]') AND name = N'IX_ApplicationsRightsByGroup_2')
DROP INDEX IX_ApplicationsRightsByGroup_2 ON dbo.ApplicationsRightsByGroup
GO
CREATE NONCLUSTERED INDEX IX_ApplicationsRightsByGroup_2 ON dbo.ApplicationsRightsByGroup
	(
	ID_Application,
	ID_GroupOrPerson,
	ID_ServerGroup,
	IsDenyRule
	) ON [PRIMARY]
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup]') AND name = N'IX_ApplicationsRightsByGroup_3')
DROP INDEX IX_ApplicationsRightsByGroup_3 ON dbo.ApplicationsRightsByGroup
GO
CREATE NONCLUSTERED INDEX IX_ApplicationsRightsByGroup_3 ON dbo.ApplicationsRightsByGroup
	(
	ID_GroupOrPerson,
	ID_Application,
	ID_ServerGroup,
	IsDenyRule
	) ON [PRIMARY]
GO
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup]') AND name = N'IX_ApplicationsRightsByGroup_4')
DROP INDEX IX_ApplicationsRightsByGroup_4 ON dbo.ApplicationsRightsByGroup
GO
CREATE NONCLUSTERED INDEX IX_ApplicationsRightsByGroup_4 ON dbo.ApplicationsRightsByGroup
	(
	ID_ServerGroup
	) ON [PRIMARY]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND name = N'_dta_index_Log_5_1188199283__K4')
DROP INDEX _dta_index_Log_5_1188199283__K4 ON dbo.Log
go
CREATE NONCLUSTERED INDEX [_dta_index_Log_5_1188199283__K4] ON [dbo].[Log]
(
	[RemoteIP] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND name = N'_dta_index_Log_5_1188199283__K1')
DROP INDEX _dta_index_Log_5_1188199283__K1 ON dbo.Log
go
CREATE NONCLUSTERED INDEX [_dta_index_Log_5_1188199283__K1] ON [dbo].[Log]
(
	[ID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND name = N'_dta_stat_1188199283_1_8_2_3')
DROP STATISTICS [dbo].[Log]._dta_stat_1188199283_1_8_2_3 
GO
CREATE STATISTICS [_dta_stat_1188199283_1_8_2_3] ON [dbo].[Log]([ID], [ConflictType], [UserID], [LoginDate])
GO

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND name = N'_dta_stat_1188199283_2_1_3_6_8')
DROP STATISTICS [dbo].[Log]._dta_stat_1188199283_2_1_3_6_8 
GO
CREATE STATISTICS [_dta_stat_1188199283_2_1_3_6_8] ON [dbo].[Log]([UserID], [ID], [LoginDate], [ApplicationID], [ConflictType])
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Log_Users]') AND name = N'_dta_index_Log_Users_5_1272391602__K2_K1_3_4_5_6')
DROP INDEX _dta_index_Log_Users_5_1272391602__K2_K1_3_4_5_6 ON dbo.Log_Users
go
CREATE NONCLUSTERED INDEX [_dta_index_Log_Users_5_1272391602__K2_K1_3_4_5_6] ON [dbo].[Log_Users]
(
	[ID_User] ASC,
	[ID] ASC
)
INCLUDE ( 	[Type],
	[Value],
	[ModifiedOn],
	[ModifiedBy]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Log_Users]') AND name = N'_dta_stat_1272391602_1_2_3')
DROP STATISTICS [dbo].[Log_Users]._dta_stat_1272391602_1_2_3 
GO
CREATE STATISTICS [_dta_stat_1272391602_1_2_3] ON [dbo].[Log_Users]([ID], [ID_User], [Type])
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K2_K3_K7_K4_K6')
DROP INDEX _dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K2_K3_K7_K4_K6 ON dbo.ApplicationsRightsByGroup_PreStagingForRealServerGroup
go
CREATE NONCLUSTERED INDEX [_dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K2_K3_K7_K4_K6] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
(
	[ID_SecurityObject] ASC,
	[ID_Group] ASC,
	[IsDevRule] ASC,
	[ID_ServerGroup] ASC,
	[IsDenyRule] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K4_K6_K3_K2_K7')
DROP INDEX _dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K4_K6_K3_K2_K7 ON dbo.ApplicationsRightsByGroup_PreStagingForRealServerGroup
go
CREATE NONCLUSTERED INDEX [_dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K4_K6_K3_K2_K7] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
(
	[ID_ServerGroup] ASC,
	[IsDenyRule] ASC,
	[ID_Group] ASC,
	[ID_SecurityObject] ASC,
	[IsDevRule] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K2_K4_K6_K3_K7')
DROP INDEX _dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K2_K4_K6_K3_K7 ON dbo.ApplicationsRightsByGroup_PreStagingForRealServerGroup
go
CREATE NONCLUSTERED INDEX [_dta_index_ApplicationsRightsByGroup_PreSta_5_1674489044__K2_K4_K6_K3_K7] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]
(
	[ID_SecurityObject] ASC,
	[ID_ServerGroup] ASC,
	[IsDenyRule] ASC,
	[ID_Group] ASC,
	[IsDevRule] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1674489044_6_2')
DROP STATISTICS [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]._dta_stat_1674489044_6_2 
GO
CREATE STATISTICS [_dta_stat_1674489044_6_2] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]([IsDenyRule], [ID_SecurityObject])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1674489044_3_4')
DROP STATISTICS [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]._dta_stat_1674489044_3_4 
GO
CREATE STATISTICS [_dta_stat_1674489044_3_4] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]([ID_Group], [ID_ServerGroup])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1674489044_6_4_3')
DROP STATISTICS [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]._dta_stat_1674489044_6_4_3 
GO
CREATE STATISTICS [_dta_stat_1674489044_6_4_3] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]([IsDenyRule], [ID_ServerGroup], [ID_Group])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1674489044_2_3_4_6_7')
DROP STATISTICS [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]._dta_stat_1674489044_2_3_4_6_7 
GO
CREATE STATISTICS [_dta_stat_1674489044_2_3_4_6_7] ON [dbo].[ApplicationsRightsByGroup_PreStagingForRealServerGroup]([ID_SecurityObject], [ID_Group], [ID_ServerGroup], [IsDenyRule], [IsDevRule])
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_index_Applications_CurrentAndInactiveO_5_21575115__K1_K26_K22')
DROP INDEX _dta_index_Applications_CurrentAndInactiveO_5_21575115__K1_K26_K22 ON dbo.Applications_CurrentAndInactiveOnes
go
CREATE NONCLUSTERED INDEX [_dta_index_Applications_CurrentAndInactiveO_5_21575115__K1_K26_K22] ON [dbo].[Applications_CurrentAndInactiveOnes]
(
	[ID] ASC,
	[AppDeleted] ASC,
	[AppDisabled] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_1_25')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_1_25 
GO
CREATE STATISTICS [_dta_stat_21575115_1_25] ON [dbo].[Applications_CurrentAndInactiveOnes]([ID], [ResetIsNewUpdatedStatusOn])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_15_26')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_15_26 
GO
CREATE STATISTICS [_dta_stat_21575115_15_26] ON [dbo].[Applications_CurrentAndInactiveOnes]([IsNew], [AppDeleted])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_1_26_17')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_1_26_17 
GO
CREATE STATISTICS [_dta_stat_21575115_1_26_17] ON [dbo].[Applications_CurrentAndInactiveOnes]([ID], [AppDeleted], [LocationID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_1_17_18')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_1_17_18 
GO
CREATE STATISTICS [_dta_stat_21575115_1_17_18] ON [dbo].[Applications_CurrentAndInactiveOnes]([ID], [LocationID], [LanguageID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_26_1_16')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_26_1_16 
GO
CREATE STATISTICS [_dta_stat_21575115_26_1_16] ON [dbo].[Applications_CurrentAndInactiveOnes]([AppDeleted], [ID], [IsUpdated])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_18_1_26_17')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_18_1_26_17 
GO
CREATE STATISTICS [_dta_stat_21575115_18_1_26_17] ON [dbo].[Applications_CurrentAndInactiveOnes]([LanguageID], [ID], [AppDeleted], [LocationID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_16_26_15_1_17')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_16_26_15_1_17 
GO
CREATE STATISTICS [_dta_stat_21575115_16_26_15_1_17] ON [dbo].[Applications_CurrentAndInactiveOnes]([IsUpdated], [AppDeleted], [IsNew], [ID], [LocationID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_17_18_1_26_16')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_17_18_1_26_16 
GO
CREATE STATISTICS [_dta_stat_21575115_17_18_1_26_16] ON [dbo].[Applications_CurrentAndInactiveOnes]([LocationID], [LanguageID], [ID], [AppDeleted], [IsUpdated])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_1_26_22_17_18')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_1_26_22_17_18 
GO
CREATE STATISTICS [_dta_stat_21575115_1_26_22_17_18] ON [dbo].[Applications_CurrentAndInactiveOnes]([ID], [AppDeleted], [AppDisabled], [LocationID], [LanguageID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_26_16_15_17_18_22')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_26_16_15_17_18_22 
GO
CREATE STATISTICS [_dta_stat_21575115_26_16_15_17_18_22] ON [dbo].[Applications_CurrentAndInactiveOnes]([AppDeleted], [IsUpdated], [IsNew], [LocationID], [LanguageID], [AppDisabled])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_18_26_16_15_1_17')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_18_26_16_15_1_17 
GO
CREATE STATISTICS [_dta_stat_21575115_18_26_16_15_1_17] ON [dbo].[Applications_CurrentAndInactiveOnes]([LanguageID], [AppDeleted], [IsUpdated], [IsNew], [ID], [LocationID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Applications_CurrentAndInactiveOnes]') AND name = N'_dta_stat_21575115_22_26_1_16_15_17_18')
DROP STATISTICS [dbo].[Applications_CurrentAndInactiveOnes]._dta_stat_21575115_22_26_1_16_15_17_18 
GO
CREATE STATISTICS [_dta_stat_21575115_22_26_1_16_15_17_18] ON [dbo].[Applications_CurrentAndInactiveOnes]([AppDisabled], [AppDeleted], [ID], [IsUpdated], [IsNew], [LocationID], [LanguageID])
go

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]') AND name = N'_dta_index_ApplicationsRightsByUser_PreStag_5_1706489158__K3_K4_K6_K2_K7')
DROP INDEX _dta_index_ApplicationsRightsByUser_PreStag_5_1706489158__K3_K4_K6_K2_K7 ON dbo.ApplicationsRightsByUser_PreStagingForRealServerGroup
go
CREATE NONCLUSTERED INDEX [_dta_index_ApplicationsRightsByUser_PreStag_5_1706489158__K3_K4_K6_K2_K7] ON [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]
(
	[ID_User] ASC,
	[ID_ServerGroup] ASC,
	[IsDenyRule] ASC,
	[ID_SecurityObject] ASC,
	[IsDevRule] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1706489158_6_4')
DROP STATISTICS [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]._dta_stat_1706489158_6_4 
GO
CREATE STATISTICS [_dta_stat_1706489158_6_4] ON [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]([IsDenyRule], [ID_ServerGroup])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1706489158_4_3_6_2')
DROP STATISTICS [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]._dta_stat_1706489158_4_3_6_2 
GO
CREATE STATISTICS [_dta_stat_1706489158_4_3_6_2] ON [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]([ID_ServerGroup], [ID_User], [IsDenyRule], [ID_SecurityObject])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]') AND name = N'_dta_stat_1706489158_2_7_3_4_6')
DROP STATISTICS [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]._dta_stat_1706489158_2_7_3_4_6 
GO
CREATE STATISTICS [_dta_stat_1706489158_2_7_3_4_6] ON [dbo].[ApplicationsRightsByUser_PreStagingForRealServerGroup]([ID_SecurityObject], [IsDevRule], [ID_User], [ID_ServerGroup], [IsDenyRule])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND name = N'_dta_stat_1188199283_3_1')
DROP STATISTICS [dbo].[Log]._dta_stat_1188199283_3_1 
GO
CREATE STATISTICS [_dta_stat_1188199283_3_1] ON [dbo].[Log]([LoginDate], [ID])
go

IF EXISTS (SELECT * FROM sys.stats WHERE object_id = OBJECT_ID(N'[dbo].[Log]') AND name = N'_dta_stat_1188199283_1_8_2_3_6_4')
DROP STATISTICS [dbo].[Log]._dta_stat_1188199283_1_8_2_3_6_4 
GO
CREATE STATISTICS [_dta_stat_1188199283_1_8_2_3_6_4] ON [dbo].[Log]([ID], [ConflictType], [UserID], [LoginDate], [ApplicationID], [RemoteIP])
go

