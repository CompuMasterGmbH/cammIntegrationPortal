IF EXISTS (SELECT * FROM sys.objects WHERE object_id = object_id(N'[dbo].[Public_GetNavPointsOfGroup]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
DROP PROCEDURE [Public_GetNavPointsOfGroup]
GO
----------------------------------------------------
-- dbo.Public_GetNavPointsOfGroup
----------------------------------------------------
CREATE Procedure [dbo].[Public_GetNavPointsOfGroup]
	@GroupID int,
	@ServerIP nvarchar(32),
	@LanguageID int,
	@AnonymousAccess bit = 0,
	@SearchForAlternativeLanguages bit = 1
WITH ENCRYPTION
As
DECLARE @IsSecurityAdmin bit
DECLARE @AllowedLocation int
DECLARE @buffer int
DECLARE @PublicGroupID int
DECLARE @AnonymousGroupID int
DECLARE @AlternativeLanguage int

SET NoCount ON

-- for example: LanguageID = 512 --> also search for the alternative language with ID 2
If @SearchForAlternativeLanguages = 1
	SELECT @AlternativeLanguage = AlternativeLanguage
	FROM System_Languages
	WHERE ID = @LanguageID
Else
	SELECT @AlternativeLanguage = @LanguageID
If @AlternativeLanguage IS NULL
	SELECT @AlternativeLanguage = @LanguageID

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
		SELECT distinct Level1Title, Level2Title, Level3Title, Level4Title, Level5Title, Level6Title
		into #NavUpdatedItems_Filtered
			FROM dbo.view_ApplicationRights LEFT OUTER JOIN dbo.Memberships ON dbo.view_ApplicationRights.ID_Group = dbo.Memberships.ID_Group LEFT JOIN dbo.System_Servers ON dbo.view_ApplicationRights.LocationID = dbo.System_Servers.ID
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_Group = @GroupID) OR (dbo.Memberships.ID_Group = @GroupID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
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
			WHERE (dbo.System_Servers.ServerGroup = @AllowedLocation And ((dbo.view_ApplicationRights.ID_Group = @GroupID) OR (dbo.Memberships.ID_Group = @GroupID) OR (dbo.view_ApplicationRights.ID_Group = @PublicGroupID) OR (dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID)))  And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage)  And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1) 
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
			WHERE dbo.System_Servers.ServerGroup = @AllowedLocation And dbo.view_ApplicationRights.ID_Group = @AnonymousGroupID And dbo.view_ApplicationRights.LanguageID in (@LanguageID, @AlternativeLanguage) And (dbo.view_ApplicationRights.AppDisabled = 0 Or dbo.view_ApplicationRights.DevelopmentTeamMember = 1)
			ORDER BY dbo.view_ApplicationRights.Sort, Case When dbo.view_ApplicationRights.Level2Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level1Title, Case When dbo.view_ApplicationRights.Level3Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level2Title, Case When dbo.view_ApplicationRights.Level4Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level3Title, Case When dbo.view_ApplicationRights.Level5Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level4Title, Case When dbo.view_ApplicationRights.Level6Title Is Null Then 0 Else 1 End, dbo.view_ApplicationRights.Level5Title, dbo.view_ApplicationRights.Level6Title

		DROP TABLE #NavUpdatedItems_Filtered
