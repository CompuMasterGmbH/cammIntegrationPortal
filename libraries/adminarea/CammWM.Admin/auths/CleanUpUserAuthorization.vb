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

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    ''' Removes authorization of single users that are already authorized by a group
    ''' </summary>
    Public Class CleanUpUserAuthorization
        Inherits Page

#Region " Variables "
        Protected pnlAuthCleanUp As Panel
        Protected InfoLbl, lblErr As Label
        Protected WithEvents cleanUpBtn As Button
#End Region

#Region " Page Events "

        Private ReadOnly Property SqlQuery() As String
            Get
                Dim sb As New System.Text.StringBuilder
                sb.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine)
                sb.Append("SELECT UserAuths.UserID, UserAuths.AuthID" & vbNewLine)
                sb.Append("FROM" & vbNewLine)
                sb.Append("      (" & vbNewLine)
                sb.Append("            SELECT ID_GroupOrPerson AS UserID, ID AS AuthID" & vbNewLine)
                sb.Append("            FROM dbo.ApplicationsRightsByUser" & vbNewLine)
                sb.Append("            WHERE ID_Application = @AppID AND DevelopmentTeamMember <> 'True'" & vbNewLine)
                sb.Append("      ) AS UserAuths" & vbNewLine)
                sb.Append("      INNER JOIN" & vbNewLine)
                sb.Append("      (" & vbNewLine)
                sb.Append("            SELECT dbo.Memberships.ID_User AS UserID" & vbNewLine)
                sb.Append("            FROM dbo.ApplicationsRightsByGroup" & vbNewLine)
                sb.Append("                  INNER JOIN dbo.Memberships ON dbo.ApplicationsRightsByGroup.ID_GroupOrPerson = dbo.Memberships.ID_Group" & vbNewLine)
                sb.Append("            WHERE ID_Application = @AppID" & vbNewLine)
                sb.Append("      ) AS GroupAuths " & vbNewLine)
                sb.Append("      ON UserAuths.UserID = GroupAuths.UserID" & vbNewLine)
                Return sb.ToString
            End Get
        End Property

        Private Sub AddPotentialCleanupItemToResults(results As DataTable, userAuth As WMSystem.SecurityObjectAuthorizationForUser, foundInGroupAuth As WMSystem.SecurityObjectAuthorizationForGroup)
            Dim NewResultRow As DataRow = results.NewRow
            NewResultRow("UserAuth_UserID") = userAuth.UserInfo.IDLong
            NewResultRow("UserAuth_UserCompany") = userAuth.UserInfo.Company
            NewResultRow("UserAuth_UserFullName") = userAuth.UserInfo.FullName
            NewResultRow("UserAuth_UserLoginName") = userAuth.UserInfo.LoginName
            NewResultRow("UserAuth_ServerGroup") = userAuth.ServerGroupID
            If userAuth.IsDeveloperAuthorization Then
                NewResultRow("UserAuth_IsDev") = "Dev"
            Else
                NewResultRow("UserAuth_IsDev") = "./."
            End If
            If userAuth.IsDenyRule Then
                NewResultRow("UserAuth_IsDenyRule") = "DENY"
            Else
                NewResultRow("UserAuth_IsDenyRule") = "GRANT"
            End If
            NewResultRow("GroupAuthWithEffectiveUserAuth_GroupID") = foundInGroupAuth.GroupInfo.ID
            NewResultRow("GroupAuthWithEffectiveUserAuth_GroupName") = foundInGroupAuth.GroupInfo.Name
            NewResultRow("GroupAuthWithEffectiveUserAuth_ServerGroup") = foundInGroupAuth.ServerGroupID
            If foundInGroupAuth.IsDevRule Then
                NewResultRow("GroupAuthWithEffectiveUserAuth_IsDev") = "Dev"
            Else
                NewResultRow("GroupAuthWithEffectiveUserAuth_IsDev") = "./."
            End If
            If foundInGroupAuth.IsDenyRule Then
                NewResultRow("GroupAuthWithEffectiveUserAuth_IsDenyRule") = "DENY"
            Else
                NewResultRow("GroupAuthWithEffectiveUserAuth_IsDenyRule") = "GRANT"
            End If
            'Final test
            If userAuth.IsDeveloperAuthorization <> foundInGroupAuth.IsDevRule OrElse userAuth.IsDenyRule <> foundInGroupAuth.IsDenyRule Then
                Throw New Exception("User's dev rule and deny rule must always match with the group's dev rule and deny rule")
            End If
            results.Rows.Add(NewResultRow)
        End Sub

        ''' <summary>
        ''' Check for unnecessary user authorizations and show results to user
        ''' </summary>
        Private Sub ShowPotentialCleanupItems()

            _ResultsOfCheckForPotentialCleanupItems = CheckForPotentialCleanupItems()

            If _ResultsOfCheckForPotentialCleanupItems.Rows.Count = 0 Then
                InfoLbl.Text = "No user entries to clean up."
                cleanUpBtn.Enabled = False
            Else
                InfoLbl.Text = CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.ConvertToHtmlTable(_ResultsOfCheckForPotentialCleanupItems.Rows, _ResultsOfCheckForPotentialCleanupItems.TableName, "<h4>", "</h4>", "style=""width: 100%; border: 0px solid black; text-align: left;""", True)
                cleanUpBtn.Enabled = True
            End If

        End Sub

        Private ReadOnly Property ReferencedSecurityObjectID As Integer
            Get
                Return CInt(Request.QueryString("ID"))
            End Get
        End Property

        Private ReadOnly Property ReferencedSecurityObjectInfo As WMSystem.SecurityObjectInformation
            Get
                Static _ReferencedSecurityObjectInfo As WMSystem.SecurityObjectInformation
                If _ReferencedSecurityObjectInfo Is Nothing Then
                    _ReferencedSecurityObjectInfo = New WMSystem.SecurityObjectInformation(Me.ReferencedSecurityObjectID, Me.cammWebManager, False)
                End If
                Return _ReferencedSecurityObjectInfo
            End Get
        End Property

        Private _ResultsOfCheckForPotentialCleanupItems As DataTable
        ''' <summary>
        ''' Check for unnecessary user authorizations and provide results as System.Data.DataTable
        ''' </summary>
        Private Function CheckForPotentialCleanupItems() As DataTable
            Dim SecObj As WMSystem.SecurityObjectInformation = ReferencedSecurityObjectInfo

            Dim Results As New DataTable
            Results.Columns.Add("UserAuth_UserID", GetType(Long)).Caption = "User ID"
            Results.Columns.Add("UserAuth_UserCompany", GetType(String)).Caption = "Company"
            Results.Columns.Add("UserAuth_UserFullName", GetType(String)).Caption = "User name"
            Results.Columns.Add("UserAuth_UserLoginName", GetType(String)).Caption = "Login name"
            Results.Columns.Add("UserAuth_ServerGroup", GetType(Integer)).Caption = "Server group"
            Results.Columns.Add("UserAuth_IsDev", GetType(String)).Caption = "User privilege Type"
            Results.Columns.Add("UserAuth_IsDenyRule", GetType(String)).Caption = "User rule"
            Results.Columns.Add("GroupAuthWithEffectiveUserAuth_GroupID", GetType(Integer)).Caption = "Group ID"
            Results.Columns.Add("GroupAuthWithEffectiveUserAuth_GroupName", GetType(String)).Caption = "Group name"
            Results.Columns.Add("GroupAuthWithEffectiveUserAuth_ServerGroup", GetType(Integer)).Caption = "Server group"
            Results.Columns.Add("GroupAuthWithEffectiveUserAuth_IsDev", GetType(String)).Caption = "Group Priviledge Type"
            Results.Columns.Add("GroupAuthWithEffectiveUserAuth_IsDenyRule", GetType(String)).Caption = "Group Rule"

            CheckForPotentialCleanupItems_AddItemsByRuleType(Results, SecObj.AuthorizationsForUsersByRule.AllowRuleStandard, SecObj.AuthorizationsForGroupsByRule.AllowRuleStandard)
            CheckForPotentialCleanupItems_AddItemsByRuleType(Results, SecObj.AuthorizationsForUsersByRule.AllowRuleDevelopers, SecObj.AuthorizationsForGroupsByRule.AllowRuleDevelopers)
            CheckForPotentialCleanupItems_AddItemsByRuleType(Results, SecObj.AuthorizationsForUsersByRule.DenyRuleStandard, SecObj.AuthorizationsForGroupsByRule.DenyRuleStandard)
            CheckForPotentialCleanupItems_AddItemsByRuleType(Results, SecObj.AuthorizationsForUsersByRule.DenyRuleDevelopers, SecObj.AuthorizationsForGroupsByRule.DenyRuleDevelopers)

            Return Results
        End Function

        Private Sub CheckForPotentialCleanupItems_AddItemsByRuleType(results As DataTable, userAuths As WMSystem.SecurityObjectAuthorizationForUser(), groupAuths As WMSystem.SecurityObjectAuthorizationForGroup())
            For MyUserCounter As Integer = 0 To userAuths.Length - 1
                Dim FoundInGroupAuth As WMSystem.SecurityObjectAuthorizationForGroup = Nothing
                For MyGroupCounter As Integer = 0 To groupAuths.Length - 1
                    For MyGroupMemberCounter As Integer = 0 To groupAuths(MyGroupCounter).GroupInfo.MembersByRule().Effective.Length - 1
                        If userAuths(MyUserCounter).UserID = groupAuths(MyGroupCounter).GroupInfo.MembersByRule().Effective(MyGroupMemberCounter).IDLong AndAlso (userAuths(MyUserCounter).ServerGroupID = groupAuths(MyGroupCounter).ServerGroupID OrElse groupAuths(MyGroupCounter).ServerGroupID = 0) Then
                            'UserID must be the same - but also the server group ID must be the same or at least the GroupAuth's server group ID must be 0 meaning "applying everywhere"
                            FoundInGroupAuth = groupAuths(MyGroupCounter)
                            Exit For
                        End If
                    Next
                    If Not FoundInGroupAuth Is Nothing Then Exit For
                Next
                If Not FoundInGroupAuth Is Nothing Then
                    AddPotentialCleanupItemToResults(results, userAuths(MyUserCounter), FoundInGroupAuth)
                End If
            Next
        End Sub

        Private Sub OnPageLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Not Me.CurrentAdminIsSupervisor Then
                Response.Write("Access denied.")
                Response.End()
                Exit Sub
            Else
                ShowPotentialCleanupItems()
            End If
        End Sub

        ''' <summary>
        ''' Cleanup authorized users that are already authorized by group
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub CleanUpAuthorization(ByVal sender As Object, ByVal e As EventArgs) Handles cleanUpBtn.Click

            Try
                Dim SecObj As WMSystem.SecurityObjectInformation = ReferencedSecurityObjectInfo
                For MyCounter As Integer = 0 To _ResultsOfCheckForPotentialCleanupItems.Rows.Count - 1
                    Dim UserID As Long = CType(_ResultsOfCheckForPotentialCleanupItems.Rows(MyCounter)("UserAuth_UserID"), Long)
                    Dim IsDev As Boolean
                    If UCase(CType(_ResultsOfCheckForPotentialCleanupItems.Rows(MyCounter)("UserAuth_IsDev"), String)) = "DEV" Then
                        IsDev = True
                    Else
                        IsDev = False
                    End If
                    Dim IsDenyRule As Boolean
                    If UCase(CType(_ResultsOfCheckForPotentialCleanupItems.Rows(MyCounter)("UserAuth_IsDenyRule"), String)) = "DENY" Then
                        IsDenyRule = True
                    Else
                        IsDenyRule = False
                    End If
                    Dim ServerGroupID As Integer = CType(_ResultsOfCheckForPotentialCleanupItems.Rows(MyCounter)("UserAuth_ServerGroup"), Integer)
                    SecObj.RemoveAuthorizationForUser(UserID, ServerGroupID, IsDev, IsDenyRule)
                Next
            Catch ex As Exception
                lblErr.Text &= ex.ToString
            End Try

            If lblErr.Text = "" Then
                InfoLbl.Text = "Cleanup successfull!"
                InfoLbl.ForeColor = Drawing.Color.Green
            Else
                InfoLbl.Text = "Failed: not able to cleanup all authorizations!"
                InfoLbl.ForeColor = Drawing.Color.Red
            End If
        End Sub

        '''' -----------------------------------------------------------------------------
        '''' <summary>
        '''' Get all users that are authorized by a group
        '''' </summary>
        '''' <param name="NewAppId"></param>
        '''' <returns>ArrayList of users that are authorized by group (userID)</returns>
        '''' <remarks>
        '''' </remarks>
        '''' <history>
        '''' 	[zeutzheim]	16.02.2010	Created
        '''' </history>
        '''' -----------------------------------------------------------------------------
        'Private Function GetAuthorizedUsersByGroup(ByVal NewAppId As Integer) As ArrayList
        '    'Get authorized groups
        '    Dim groupAL As ArrayList
        '    Dim sqlParams As SqlParameter() = {New SqlParameter("@AppID", NewAppId)}
        '    Dim strQuery As String = "select ID_Group from [view_ApplicationRights] where id_application=@AppID and isnull(ID_Group,0)<>0 order by ID_Group"
        '    groupAL = ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        '    'Get users of authorized groups
        '    Dim userInGroupAl As New ArrayList
        '    For myGroupCounter As Integer = 0 To groupAL.Count - 1
        '        Dim tmpAl As ArrayList
        '        strQuery = "SELECT ID_User " & vbNewLine & _
        '                            "FROM [view_Memberships] " & vbNewLine & _
        '                            "WHERE ID_Group = " & groupAL(myGroupCounter).ToString & " " & vbNewLine & _
        '                            "   And ID_Group not in " & vbNewLine & _
        '                            "       (" & vbNewLine & _
        '                            "           Select id_group_public " & vbNewLine & _
        '                            "           from system_servergroups" & vbNewLine & _
        '                            "       ) " & vbNewLine & _
        '                            "   and ID_Group not in " & vbNewLine & _
        '                            "       (" & vbNewLine & _
        '                            "           Select id_group_anonymous " & vbNewLine & _
        '                            "           from system_servergroups" & vbNewLine & _
        '                            "       ) " & vbNewLine & _
        '                            "   and (" & vbNewLine & _
        '                            "       0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " " & vbNewLine & _
        '                            "       OR 0 in " & vbNewLine & _
        '                            "           (" & vbNewLine & _
        '                            "               select tableprimaryidvalue " & vbNewLine & _
        '                            "               from System_SubSecurityAdjustments " & vbNewLine & _
        '                            "               Where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " " & vbNewLine & _
        '                            "                   AND TableName = 'Groups' " & vbNewLine & _
        '                            "                   AND AuthorizationType In ('SecurityMaster','ViewAllRelations')" & vbNewLine & _
        '                            "           ) " & vbNewLine & _
        '                            "       OR id_group in " & vbNewLine & _
        '                            "           (" & vbNewLine & _
        '                            "               select tableprimaryidvalue " & vbNewLine & _
        '                            "               from System_SubSecurityAdjustments " & vbNewLine & _
        '                            "               where userid = " & cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous) & " " & vbNewLine & _
        '                            "                   AND TableName = 'Groups' " & vbNewLine & _
        '                            "                   AND AuthorizationType In ('UpdateRelations','ViewRelations','Owner')" & vbNewLine & _
        '                            "           )" & vbNewLine & _
        '                            "       ) " & vbNewLine & _
        '                            "ORDER BY ID_User"
        '        tmpAl = ExecuteReaderAndPutFirstColumnIntoArrayList(New SqlConnection(cammWebManager.ConnectionString), strQuery, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        '        For myTmpCounter As Integer = 0 To tmpAl.Count - 1
        '            If Not userInGroupAl.Contains(tmpAl(myTmpCounter)) Then userInGroupAl.Add(tmpAl(myTmpCounter))
        '        Next
        '    Next
        '    Return userInGroupAl
        'End Function
#End Region

    End Class

End Namespace