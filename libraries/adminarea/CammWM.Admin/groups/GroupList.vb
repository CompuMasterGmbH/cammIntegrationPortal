'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to view the list of Groups
    ''' </summary>
    Public Class GroupList
        Inherits Page

#Region "Variable Declaration"
        Protected lblID, lblDescription, lblReleasedOn, lblErrMsg As Label
        Protected ancNavPreview, ancNew, ancSecurity, ancName, ancUpdate, ancDelete, ancReleasedByID, ancCheckMembership As HtmlAnchor
        Protected WithEvents rptGroupList As Repeater
        Dim MyDt, dtPublic, dtAuth As New DataTable
        Dim strAuthType As String = ""
        Dim blnMembership, blnDelete As Boolean
        Dim CurUserIsSecurityMaster, CurUserIsSupervisor, CurUserIsSecurityMasterGrp, CurUserIsGrantedViewAll As Boolean
#End Region

#Region "Page Events"
        Private Sub GroupList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            CurUserIsSecurityMaster = cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
            CurUserIsSupervisor = cammWebManager.System_IsSuperVisor(CLng(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)))

            Try
                Dim sqlParams1 As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
                dtAuth = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select TablePrimaryIDValue, AuthorizationType from System_SubSecurityAdjustments where userid=@UserID and TableName='Groups'", CommandType.Text, sqlParams1, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If Not dtAuth Is Nothing AndAlso dtAuth.Rows.Count > 0 AndAlso dtAuth.Select("AuthorizationType='SecurityMaster'").Length > 0 Then CurUserIsSecurityMasterGrp = True
                If Not dtAuth Is Nothing AndAlso dtAuth.Rows.Count > 0 AndAlso dtAuth.Select("AuthorizationType='ViewAllItems'").Length > 0 Then CurUserIsGrantedViewAll = True

                Dim sqlParams As SqlParameter() = {New SqlParameter("@UserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT view_Groups.* , Isnull((select tableprimaryidvalue from System_SubSecurityAdjustments where view_Groups.id = System_SubSecurityAdjustments.TablePrimaryIDValue and userid = @UserID AND TableName = 'Groups' AND AuthorizationType = 'UpdateRelations' group by tableprimaryidvalue),-1) as AuthTypeID FROM [view_Groups] WHERE (0 <> " & CLng(cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))) & " OR 0 in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Groups' AND AuthorizationType In ('SecurityMaster','ViewAllItems')) OR id in (select tableprimaryidvalue from System_SubSecurityAdjustments where userid = @UserID AND TableName = 'Groups' AND AuthorizationType In ('Update','Owner','View','UpdateRelations','ViewRelations','Delete'))) ORDER BY Name", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                dtPublic = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select id_group_public,id_group_anonymous from dbo.System_ServerGroups", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptGroupList.DataSource = MyDt
                    rptGroupList.DataBind()
                Else
                    lblErrMsg.Text = "There are no groups available for administration."
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

        'Added for setting Navigation Preview link
        Private Function IsPublicOrAnonymous(ByVal GroupId As Integer) As Boolean
            For iCount As Integer = 0 To dtPublic.Rows.Count - 1
                If (CInt(dtPublic.Rows(iCount)("id_group_public")) = GroupId OrElse CInt(dtPublic.Rows(iCount)("id_group_anonymous")) = GroupId) Then
                    Return True
                End If
            Next
            Return False
        End Function

#Region "Control Events"
        Private Sub rptGroupListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptGroupList.ItemDataBound
            If e.Item.ItemType = ListItemType.Header Then
                If cammWebManager.System_GetSubAuthorizationStatus("Groups", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "New") Then
                    CType(e.Item.FindControl("ancNew"), HtmlAnchor).Style.Add("display", "")
                Else
                    CType(e.Item.FindControl("ancNew"), HtmlAnchor).Style.Add("display", "none")
                End If

                If cammWebManager.System_GetSubAuthorizationStatus("Groups", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "SecurityMaster") Then
                    CType(e.Item.FindControl("ancSecurity"), HtmlAnchor).Style.Add("display", "")
                Else
                    CType(e.Item.FindControl("ancSecurity"), HtmlAnchor).Style.Add("display", "none")
                End If
            End If

            If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
                With MyDt.Rows(e.Item.ItemIndex)
                    blnMembership = False
                    blnDelete = False
                    CType(e.Item.FindControl("lblID"), Label).Text = Utils.Nz(.Item("ID"), 0).ToString
                    'If .Item("SystemGroup") = 0 Then
                    CType(e.Item.FindControl("lblDescription"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Description"), String.Empty))

                    If Not IsDBNull(.Item("ReleasedByLastName")) Then
                        CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ReleasedByID"), 0).ToString

                        If camm.WebManager.WMSystem.SpecialUsers.User_Anonymous = CInt(.Item("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Code = CInt(.Item("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Invalid = CInt(.Item("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_Public = CInt(.Item("ID")) OrElse camm.WebManager.WMSystem.SpecialUsers.User_UpdateProcessor = CInt(.Item("ID")) Then
                            CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(Me.SafeLookupUserFullName(CType(.Item("ReleasedByID"), Int64)), String.Empty))
                        Else
                            CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("ReleasedByLastName"), String.Empty) & ", " & Utils.Nz(.Item("ReleasedByFirstName"), String.Empty))
                        End If
                    Else
                        CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Me.SafeLookupUserFullName(CLng(.Item("ReleasedByID"))))
                    End If

                    CType(e.Item.FindControl("lblReleasedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ReleasedOn"), String.Empty))

                    'Select Case cint(.Item("id"))
                    '    Case -7, 6, 7
                    '        CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & .Item("ID")
                    '        CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).InnerText = "Update"
                    'End Select

                    Select Case CBool(.Item("SystemGroup"))
                        Case True
                            If IsPublicOrAnonymous(Utils.Nz(.Item("ID"), 0)) Then
                                CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=1"
                                CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).InnerHtml = "Update"
                                CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=1"
                                CType(e.Item.FindControl("ancName"), HtmlAnchor).InnerHtml = Server.HtmlDecode(Utils.Nz(.Item("Name"), String.Empty))
                            Else
                                CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0"
                                CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).InnerHtml = "Update"
                                CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0"
                                CType(e.Item.FindControl("ancName"), HtmlAnchor).InnerHtml = Server.HtmlDecode(Utils.Nz(.Item("Name"), String.Empty))
                            End If
                        Case False
                            CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0"
                            CType(e.Item.FindControl("ancName"), HtmlAnchor).InnerHtml = Server.HtmlDecode(Utils.Nz(.Item("Name"), String.Empty))
                    End Select

                    If CInt(.Item("SystemGroup")) = 0 Then
                        If IsPublicOrAnonymous(Utils.Nz(.Item("ID"), 0)) Then
                            CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=1"
                            CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).InnerHtml = "Update"
                        Else
                            CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0"
                            CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).InnerHtml = "Update"
                        End If
                        CType(e.Item.FindControl("ancDelete"), HtmlAnchor).HRef = "groups_delete.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                        CType(e.Item.FindControl("ancDelete"), HtmlAnchor).InnerHtml = "Delete<br>"
                        blnDelete = True
                    End If

                    If CurUserIsSupervisor OrElse CurUserIsSecurityMaster OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.ViewRelations, CType(.Item("ID"), Integer)) OrElse Utils.Nz(.Item("AuthTypeID"), 0) > 0 Then
                        CType(e.Item.FindControl("ancCheckMembership"), HtmlAnchor).HRef = Server.HtmlEncode("memberships.aspx?lang=" & Request.QueryString("lang") & "&GROUPID=" & Utils.Nz(.Item("ID"), 0).ToString)
                        CType(e.Item.FindControl("ancCheckMembership"), HtmlAnchor).InnerHtml = "Check Memberships<br>"
                        blnMembership = True
                    End If


                    CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).HRef = "users_navbar_preview.aspx?GroupName=" & .Item("Name").ToString + "&GroupId=" + .Item("ID").ToString
                    'If blnDelete = True And blnMembership = False Then
                    '    CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).InnerHtml = "Nav. Preview"
                    'ElseIf CurUserIsSupervisor AndAlso CurUserIsSecurityMasterGrp Then
                    '    CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).InnerHtml = "Nav. Preview"
                    'Else
                    CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).InnerHtml = "Nav. Preview"
                    'End If

                    If cammWebManager.System_DBVersion_Ex.Build < WMSystem.MilestoneDBBuildNumber_Build164 Then
                        CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).Disabled = True
                        CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).HRef = "#"
                        CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).Title = "Available with CWM DB-Build 164 or higher - please update your database"
                    End If

                    If Not CurUserIsSupervisor AndAlso Not CurUserIsSecurityMasterGrp AndAlso (CurUserIsSecurityMaster OrElse (Not dtAuth Is Nothing AndAlso dtAuth.Rows.Count > 0)) Then
                        'if not a supervisor and not a security master for groups, check for every single priviledge (IS THIS COMMENT CORRECT? DON'T KNOW WHY AndAlso (CurUserIsSecurityMaster OrElse (Not dtAuth Is Nothing AndAlso dtAuth.Rows.Count > 0)...
                        If Not Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.View, CType(.Item("ID"), Integer)) Then
                            CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = ""
                        End If
                        CType(e.Item.FindControl("ancDelete"), HtmlAnchor).Visible = False
                        CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).Visible = False
                        If Not Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.ViewRelations, CType(.Item("ID"), Integer)) Then
                            CType(e.Item.FindControl("ancCheckMembership"), HtmlAnchor).Visible = False
                        End If
                        Dim arrdtRows() As DataRow
                        arrdtRows = dtAuth.Select("TablePrimaryIDValue=" & Utils.Nz(.Item("ID"), 0))
                        For Each drow As DataRow In arrdtRows
                            Select Case Utils.Nz(drow("AuthorizationType"), String.Empty).ToLower
                                Case "delete"
                                    CType(e.Item.FindControl("ancDelete"), HtmlAnchor).Visible = True

                                Case "update"
                                    CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).Visible = True
                                    If Utils.Nz(.Item("SystemGroup"), False) = True AndAlso IsPublicOrAnonymous(Utils.Nz(.Item("ID"), 0)) Then
                                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=1"
                                    Else
                                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0"
                                    End If

                                Case "updaterelations", "viewrelations", "viewallrelations"
                                    CType(e.Item.FindControl("ancCheckMembership"), HtmlAnchor).Visible = True
                                    If dtAuth.Select("TablePrimaryIDValue=" & Utils.Nz(.Item("ID"), 0) & " and AuthorizationType in ('Delete','Update')").Length > 0 Then
                                        CType(e.Item.FindControl("ancCheckMembership"), HtmlAnchor).InnerHtml = "Check Memberships<br>"
                                    End If

                                Case "owner"
                                    CType(e.Item.FindControl("ancDelete"), HtmlAnchor).Visible = True
                                    CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).Visible = True
                                    CType(e.Item.FindControl("ancCheckMembership"), HtmlAnchor).Visible = True
                                    If Utils.Nz(.Item("SystemGroup"), False) = True AndAlso IsPublicOrAnonymous(Utils.Nz(.Item("ID"), 0)) Then
                                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=1"
                                    Else
                                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0"
                                    End If
                                Case "view", "viewallitems"
                                    If Utils.Nz(.Item("SystemGroup"), False) = True AndAlso IsPublicOrAnonymous(Utils.Nz(.Item("ID"), 0)) Then
                                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=1&view=1"
                                    Else
                                        CType(e.Item.FindControl("ancName"), HtmlAnchor).HRef = "groups_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString + "&IsAnonymousOrPublic=0&view=1"
                                    End If
                            End Select
                            'If dtAuth.Select("TablePrimaryIDValue=" & Utils.Nz(.Item("ID"), 0) & " and AuthorizationType in ('Delete','Update','UpdateRelations')").Length > 0 Then
                            '    CType(e.Item.FindControl("ancNavPreview"), HtmlAnchor).InnerHtml = "Nav. Preview"
                            'End If
                        Next
                    End If
                End With
            End If
        End Sub
#End Region

    End Class

End Namespace