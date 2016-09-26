'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden für Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Strict On
Option Explicit On

Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to add any group to a perticular application
    ''' </summary>
    Public Class AppRightsNewGroups
        Inherits Page

#Region "Variable Declaration"
        Dim CurUserID As Long
        Dim ErrMSg As String
        Protected lblErr As Label
        Protected lblMsg As Label
        Protected WithEvents btnOK As Button
        Protected WithEvents drp_apps, drp_groups, drp_servergroups As DropDownList
        Protected WithEvents chk_deny As CheckBox
        Protected WithEvents chk_devteam As CheckBox
#End Region

#Region "Page Events"
        Private Sub AppRightsNewGroups_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErr.Text = ""
            lblMsg.Text = ""
            If Not IsPostBack Then
                ListApps()
                ListServerGroups()
                LoadGroups()
            End If

            If Me.IsPostBack = False AndAlso Me.chk_deny IsNot Nothing AndAlso Me.chk_devteam IsNot Nothing Then
                'Show or hide controls for MilestoneDBVersion_AuthsWithSupportForDenyRule depending on environment support
                If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    Me.chk_deny.Visible = True
                    Me.chk_devteam.Visible = True
                Else
                    Me.chk_deny.Visible = False
                    Me.chk_devteam.Visible = False
                End If
            End If
        End Sub

#End Region

#Region "User-Defined Methods"
        Private Sub LoadGroups()
            Dim GrpTable As New DataTable

            If HttpContext.Current.Cache("CammWM.Admin.Table.AllGroups") Is Nothing Then
                GrpTable = FillDataTable(New SqlCommand("Select * FROM Gruppen ORDER BY Name", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                HttpContext.Current.Cache.Insert("CammWM.Admin.Table.AllGroups", GrpTable, Nothing, Now.AddMinutes(20), Caching.Cache.NoSlidingExpiration)
            Else
                GrpTable = CType(HttpContext.Current.Cache("CammWM.Admin.Table.AllGroups"), DataTable)
            End If

            Me.drp_groups.DataSource = GrpTable
            Me.drp_groups.DataTextField = "Name"
            Me.drp_groups.DataValueField = "ID"
            Me.drp_groups.DataBind()

        End Sub
        ''' <summary>
        ''' List all available applications and bind them on the DropDownList. PreSelect application where user comes from.
        ''' </summary>
        Private Sub ListApps()
            Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
            Dim WhereClause As String = "WHERE (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " Or Applications.ID In (Select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID And TableName = 'Applications' AND Applications.ID in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Applications' AND AuthorizationType In ('UpdateRelations','Owner')))) "
            Dim sql As String = "SELECT Applications.ID, Applications.Title, Applications.AppDisabled, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID " & WhereClause & " ORDER BY Title"
            Dim AppTable As DataTable = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), sql, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            For Each row As DataRow In AppTable.Rows
                row("Title") = Utils.Nz(row("Title"), String.Empty) & " (ID " & Utils.Nz(row("ID"), String.Empty) & Utils.Nz(IIf(Utils.Nz(row("AppDisabled"), False) = True, " (D)", ""), String.Empty) & " / " & Utils.Nz(row("ServerDescription"), String.Empty) & " / " & Utils.Nz(row("Description"), String.Empty) & ")"
            Next

            Me.drp_apps.DataSource = AppTable
            Me.drp_apps.DataTextField = "Title"
            Me.drp_apps.DataValueField = "ID"
            Me.drp_apps.DataBind()

            If Not Request.QueryString("ID") = "" Then
                Me.drp_apps.SelectedValue = Request.QueryString("ID")
            End If

        End Sub

        ''' <summary>
        ''' List all available servergroups and bind them on the DropDownList. PreSelect servergroup 0 (=all-item)
        ''' </summary>
        Private Sub ListServerGroups()
            Me.drp_servergroups.Items.Clear()
            Me.drp_servergroups.Items.Add(New ListItem("(All server groups)", "0"))
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                For Each SGroup As WMSystem.ServerGroupInformation In Me.cammWebManager.System_GetServerGroupsInfo
                    Me.drp_servergroups.Items.Add(New ListItem(SGroup.Title, SGroup.ID.ToString))
                Next
            End If
            If Not Request.QueryString("ID") = "" Then
                Me.drp_servergroups.SelectedValue = "0"
            End If
        End Sub

#End Region

        ''' <summary>
        ''' Checks whether the given group is authorization for the application
        ''' </summary>
        ''' <param name="groupId"></param>
        ''' <param name="applicationId"></param>
        ''' <remarks></remarks>
        Private Function IsGroupAuthorizedForApplication(ByVal groupId As Integer, ByVal applicationId As Integer) As Boolean
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                Throw New NotSupportedException("DbVersion.Build >= " & WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule.ToString & " requires calling an overloaded version of this method")
            End If
            Dim commandText As String = "SELECT count([ID]) FROM [dbo].[ApplicationsRightsByGroup] WHERE [ID_GroupOrPerson] = @GroupId AND [ID_Application] =  @AppID"
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupId
            MyCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = applicationId
            Dim RecordCount As Integer = CType(ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Return RecordCount > 0
        End Function

        ''' <summary>
        ''' Checks whether the given group is authorization for the application
        ''' </summary>
        ''' <param name="groupId"></param>
        ''' <param name="applicationId"></param>
        ''' <remarks></remarks>
        Private Function IsGroupAuthorizedForApplication(ByVal groupID As Integer, ByVal applicationID As Integer, serverGroupID As Integer, isDev As Boolean, isDenyRule As Boolean) As Boolean
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then 'Older
                Throw New NotSupportedException("DbVersion.Build < " & WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule.ToString & " requires calling an overloaded version of this method")
            End If
            Dim commandText As String = "SELECT count([ID]) FROM [dbo].[ApplicationsRightsByGroup] WHERE [ID_GroupOrPerson] = @GroupId AND [ID_Application] =  @AppID AND [ID_ServerGroup] =  @ServerGroupID AND IsNull(IsDenyRule, 0) = @IsDenyRule AND IsNull(DevelopmentTeamMember, 0) = @IsDev"
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
            MyCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = applicationID
            MyCmd.Parameters.Add("@ServerGroupID", SqlDbType.Int).Value = serverGroupID
            MyCmd.Parameters.Add("@IsDev", SqlDbType.Bit).Value = isDev
            MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
            Dim RecordCount As Integer = CType(ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Return RecordCount > 0
        End Function

        Public Function GetRequiredUserFlagsForApplication(ByVal applicationID As Integer) As String()
            Dim commandParams As SqlParameter() = {New SqlParameter("@AppID", applicationID)}
            Dim commandText As String = "Select RequiredUserProfileFlags FROM Applications_CurrentAndInactiveOnes Where ID = @AppID" '& dropAppID
            Dim RequiredUserFlags As String = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), commandText, CommandType.Text, commandParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToString().Trim()

            If RequiredUserFlags = String.Empty Then
                Return New String() {}
            End If

            Return RequiredUserFlags.Split(CChar(","))

        End Function


#Region "Control Events"
        ''' <summary>
        ''' Is a checkbox checked or unchecked/missing?
        ''' </summary>
        ''' <param name="control"></param>
        Private Function IsChecked(control As CheckBox) As Boolean
            If control Is Nothing Then
                Return False
            Else
                Return control.Checked
            End If
        End Function

        Private Sub btnOKClick(ByVal sender As Object, ByVal args As EventArgs) Handles btnOK.Click
            Dim GroupTable As New DataTable
            Dim AppTable As New DataTable
            Dim dropGroupText, dropAppText As String
            Dim dropGroupID, dropAppID As Integer
            Dim dropServerGroupID As Integer = CInt(drp_servergroups.SelectedValue)

            dropAppText = Utils.Nz(drp_apps.SelectedItem.Text, String.Empty)
            dropAppID = CInt(Utils.Nz(drp_apps.SelectedValue, String.Empty))


            dropGroupText = Utils.Nz(drp_groups.SelectedItem.Text, String.Empty)
            dropGroupID = CInt(Utils.Nz(drp_groups.SelectedValue, String.Empty))

            'Hint: what is:
            ' - If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then 'Older
            ' - If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Equal OR Newer
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 AndAlso IsGroupAuthorizedForApplication(dropGroupID, dropAppID) Then
                lblErr.Text = "Group " + dropGroupText.ToString.Trim + " is already authorized for application " + dropAppText.Trim
            ElseIf Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 AndAlso Me.chk_devteam IsNot Nothing AndAlso Me.chk_deny IsNot Nothing AndAlso IsGroupAuthorizedForApplication(dropGroupID, dropAppID, dropServerGroupID, Me.chk_devteam.Checked, Me.chk_deny.Checked) Then
                lblErr.Text = "Group " & dropGroupText.ToString.Trim & " (development access: " & IsChecked(Me.chk_devteam).ToString.ToLower & ", deny rule: " & IsChecked(Me.chk_deny).ToString.ToLower & ") is already authorized for application " & dropAppText.Trim
            ElseIf Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 AndAlso Me.chk_devteam Is Nothing AndAlso Me.chk_deny Is Nothing AndAlso IsGroupAuthorizedForApplication(dropGroupID, dropAppID, dropServerGroupID, False, False) Then
                lblErr.Text = "Group " + dropGroupText.ToString.Trim + " is already authorized for application " + dropAppText.Trim
            ElseIf Not ((cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                Response.Write("No authorization to administrate this application.")
                Response.End()
            ElseIf dropAppText.Trim <> "" And dropGroupText.ToString.Trim <> "" Then
                CurUserID = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)

                If IsChecked(Me.chk_deny) = False AndAlso Me.CurrentDbVersion.Build >= WMSystem.MilestoneDBBuildNumber_Build147 Then
                    'Allow-rules require flags-check
                    Dim requiredFlags() As String = GetRequiredUserFlagsForApplication(dropAppID)

                    Dim commandText As String
                    If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                        commandText = "SELECT A.ID_USER [ID], (Select   ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname FROM Benutzer Where ID = A.ID_USER) AS [Name] From [Memberships_EffectiveRulesWithClonesNthGrade] AS A Where ID_Group = @GroupID" '+ CStr(id_grouporperson.SelectedItem.Id)
                    Else
                        commandText = "SELECT A.ID_USER [ID], (Select   ISNULL(Benutzer.Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Benutzer.Namenszusatz, ''), 1, 1))}) + Benutzer.Nachname + ', ' + Benutzer.Vorname FROM Benutzer Where ID = A.ID_USER) AS [Name] From Memberships A Where ID_Group =  @GroupID" '+ CStr(id_grouporperson.SelectedItem.Id)
                    End If
                    Dim MyCmd As New System.Data.SqlClient.SqlCommand(commandText, New SqlConnection(cammWebManager.ConnectionString))
                    MyCmd.CommandType = CommandType.Text
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = dropGroupID
                    Dim DTUser As DataTable = FillDataTable(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    Dim flagsUpdateLinks As String = String.Empty
                    If Not DTUser Is Nothing AndAlso DTUser.Rows.Count > 0 Then
                        For Each dRowUser As DataRow In DTUser.Rows
                            Dim userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = cammWebManager.System_GetUserInfo(CType(dRowUser("ID"), Long))
                            If Not userInfo Is Nothing Then
                                Dim validationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(userInfo, requiredFlags, True)
                                flagsUpdateLinks &= CompuMaster.camm.WebManager.Administration.Utils.FormatLinksToFlagUpdatePages(validationResults, userInfo)
                            End If
                        Next
                    End If

                    If flagsUpdateLinks.Length > 0 Then
                        lblErr.Text = "Required flags are not set or type of value is not correct:<br>" + flagsUpdateLinks + "<br>Failed to create authorization for group!"
                        Exit Sub
                    End If

                End If

                Try
                    'Use implemented workflow of camm WebManager to send Authorization-Mail if user is authorized the first time
                    'HINT: SapFlag checks will be checked continually, but no information on missing flag name is given here any more
                    Dim grpInfo As New WebManager.WMSystem.GroupInformation(dropGroupID, cammWebManager)
                    grpInfo.AddAuthorization(dropAppID, dropServerGroupID, IsChecked(Me.chk_devteam), IsChecked(Me.chk_deny))
                    lblErr.Text = ""
                    lblMsg.Text = dropGroupText.Trim + " has been authorized for application " + dropAppText
                Catch ex As Exception
                    cammWebManager.Log.ReportErrorByEMail(ex, "cammWebManager: Group authorization creation failed!")
                    ErrMSg = "Group authorization creation failed! (" & ex.Message & ")"
                End Try

            ElseIf dropAppText.Trim = "" And dropGroupText.ToString.Trim = "" Then
                'do nothing
            Else
                ErrMSg = "Please specify an application and a group to proceed!"
            End If

            If ErrMSg <> "" Then lblErr.Text = Utils.Nz(ErrMSg, String.Empty)
        End Sub
#End Region

    End Class

End Namespace