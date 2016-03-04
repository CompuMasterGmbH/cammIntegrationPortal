'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Strict On
Option Explicit On 

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to view the list of Groups
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
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

#Region "User-Defined Function(s)"
        'Added by I-link on 07.08.2008 for setting Navigation Preview link.-----------
        Private Function IsPublicOrAnonymous(ByVal GroupId As Integer) As Boolean
            For iCount As Integer = 0 To dtPublic.Rows.Count - 1
                If (CInt(dtPublic.Rows(iCount)("id_group_public")) = GroupId OrElse CInt(dtPublic.Rows(iCount)("id_group_anonymous")) = GroupId) Then
                    Return True
                End If
            Next
            Return False
        End Function
#End Region

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
                            Dim userinfo As WMSystem.UserInformation
                            userinfo = New WebManager.WMSystem.UserInformation(CType(.Item("ReleasedByID"), Int64), cammWebManager, False)
                            CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(userinfo.FullName, String.Empty))
                        Else
                            CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("ReleasedByLastName"), String.Empty) & ", " & Utils.Nz(.Item("ReleasedByFirstName"), String.Empty))
                        End If
                    Else
                        CType(e.Item.FindControl("ancReleasedByID"), HtmlAnchor).InnerHtml = Server.HtmlEncode(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(.Item("ReleasedByID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem), True).FullName)
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

                    If cammWebManager.System_DBVersion_Ex.Build < 164 Then
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

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.GroupNew
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to create new group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	04.09.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GroupNew
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg As Label
        Protected textName As TextBox
        Protected textDescription As TextBox
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub GroupNew_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Groups", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "New")) Then
                Response.Write("No authorization to create new groups.")
                Response.End()
            End If
        End Sub
#End Region

#Region "Control Events"
        Sub SaveChanges(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            Try
                Dim Redirect2URL As String = ""
                If Trim(textName.Text) <> "" Then
                    Dim iResult As Object
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@Name", textName.Text.Trim), New SqlParameter("@Description", textDescription.Text.Trim)}
                    iResult = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_CreateGroup", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                    If CInt(iResult) = -1 Then
                        Redirect2URL = "groups.aspx"
                    Else
                        lblErrMsg.Text = "Group creation failed!"
                    End If
                End If
                If Redirect2URL <> "" Then
                    Response.Redirect(Redirect2URL)
                Else
                    lblErrMsg.Text = "Please specify the field ""Group name"" to proceed!"
                End If
            Catch ex As Exception
                lblErrMsg.Text = "Group creation failed!"
            End Try
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.GroupDelete
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete a group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	9.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GroupDelete
        Inherits Page

#Region "Variable Decalration"
        Protected lblGroup_ID, lblLastModificationOn, lblCreatedOn, lblDescription, lblGroupName, lblGroupID, lblErrMsg As Label
        Protected hypLastModificationBy, hypCreatedBy, hypDeleteConfirmation As HyperLink
        Protected cammWebManagerAdminGroupInfoDetails As CompuMaster.camm.WebManager.Controls.Administration.GroupsAdditionalInformation
#End Region

#Region "Page Events"
        Private Sub GroupDeleteLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim MyGroupInfo As New CompuMaster.camm.webmanager.WMSystem.GroupInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
            cammWebManagerAdminGroupInfoDetails.MyGroupInfo = MyGroupInfo

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Groups", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Groups", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Delete")) Then
                Response.Write("No authorization to administrate this group.")
                Response.End()
            ElseIf Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then

                Try
                    Dim SqlParam As SqlParameter() = {New SqlParameter("@GroupID", Request.QueryString("ID"))}
                    Dim SqlParam1 As SqlParameter() = {New SqlParameter("@GroupIDForMembership", Request.QueryString("ID"))}
                    Dim SqlParam2 As SqlParameter() = {New SqlParameter("@GroupID1ForApplication", Request.QueryString("ID"))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.Gruppen WHERE (((ID)=@GroupID))", CommandType.Text, SqlParam, Automations.AutoOpenConnection)
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.Memberships WHERE (((ID_Group)=@GroupIDForMembership))", CommandType.Text, SqlParam1, Automations.AutoOpenConnection)
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM dbo.ApplicationsRightsByGroup WHERE (((ID_GroupOrPerson)=@GroupID1ForApplication))", CommandType.Text, SqlParam2, Automations.AutoOpenConnection)
                    Response.Redirect("groups.aspx")
                Catch ex As Exception
                    If cammWebManager.System_DebugLevel >= 3 Then
                        lblErrMsg.Text = "Group erasing failed! (" & ex.Message & ex.StackTrace & ")"
                    Else
                        lblErrMsg.Text = "Group erasing failed!"
                    End If
                End Try
            End If

            If True Then
                Dim MyDt As DataTable
                Dim SqlParamForSelection As SqlParameter() = {New SqlParameter("@GroupIDForSelection", Request.QueryString("ID"))}
                MyDt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_Groups WHERE (((ID)=@GroupIDForSelection))", CommandType.Text, SqlParamForSelection, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If MyDt Is Nothing OrElse MyDt.Rows.Count = 0 Then
                    lblErrMsg.Text = "Group not found!"
                Else
                    With MyDt.Rows(0)
                        lblGroupID.Text = Utils.Nz(.Item("ID"), 0).ToString
                        lblGroupName.Text = Server.HtmlEncode(Utils.Nz(.Item("Name"), String.Empty))
                        lblDescription.Text = Server.HtmlEncode(Utils.Nz(.Item("description"), String.Empty))
                        lblCreatedOn.Text = Server.HtmlEncode(Utils.Nz(.Item("ReleasedOn"), String.Empty))
                        hypCreatedBy.NavigateUrl = Server.HtmlEncode("users_update.aspx?ID=" + Utils.Nz(.Item("ReleasedByID"), 0).ToString)
                        hypCreatedBy.Text = Server.HtmlEncode(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(.Item("ReleasedByID")), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem)).FullName)
                        lblLastModificationOn.Text = Server.HtmlEncode(Utils.Nz(.Item("ModifiedOn"), String.Empty))
                        hypLastModificationBy.NavigateUrl = "users_update.aspx?ID=" + .Item("ModifiedByID").ToString
                        hypLastModificationBy.Text = Server.HtmlEncode(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(.Item("ModifiedByID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem)).FullName)
                        hypDeleteConfirmation.NavigateUrl = "groups_delete.aspx?ID=" + Request.QueryString("ID") + "&DEL=NOW&token=" & Session.SessionID
                        hypDeleteConfirmation.Text = "Yes, delete it!"
                    End With
                End If
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.GroupUpdate
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to update a group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	10.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GroupUpdate
        Inherits Page

#Region "Variable Decalration"
        Protected lblCreationDate, lblModificationDate, lblErrMsg, lblGroupName As Label
        Protected txtGroupName, txtDescription, txtBccMail As TextBox
        Protected hypCreatedBy, hypModifiedBy As HyperLink
        Protected cammWebManagerAdminGroupInfoDetails As CompuMaster.camm.WebManager.Controls.Administration.GroupsAdditionalInformation
        Protected cammWebManagerAdminDelegates As CompuMaster.camm.WebManager.Controls.Administration.AdministrativeDelegates

        Protected trUpdateGroup, trMemberList, trMemberList2, trMemberList3 As System.Web.UI.HtmlControls.HtmlTableRow
        Protected WithEvents btnSubmit As Button

        Protected ViewOnlyMode As Boolean = False

#End Region

#Region "Page Events"
        Private Sub GroupUpdate_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim MyGroupInfo As New CompuMaster.camm.webmanager.WMSystem.GroupInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                cammWebManagerAdminGroupInfoDetails.MyGroupInfo = MyGroupInfo
                cammWebManagerAdminDelegates.GroupInfo = MyGroupInfo

                If CInt(Val(Request.QueryString("view") & "")) = 1 OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Update, CInt(Request.QueryString("ID"))) = False Then
                    btnSubmit.Enabled = False
                    btnSubmit.Visible = False
                End If

                'dtPublic = FilDataTable(New SqlCommand("select id_group_public,id_group_anonymous from dbo.System_ServerGroups", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                'Dim Auths As CompuMaster.camm.WebManager.WMSystem.Authorizations
                'Auths = New CompuMaster.camm.WebManager.WMSystem.Authorizations(Nothing, cammWebManager, Nothing, MyGroupInfo.ID, Nothing)
                'Dim MyGroupAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.GroupAuthorizationInformation()
                'MyGroupAuths = Auths.GroupAuthorizationInformations(MyGroupInfo.ID)
                'If Not MyGroupAuths Is Nothing Then
                '    For Each MyGroupAuthInfo As CompuMaster.camm.WebManager.WMSystem.Authorizations.GroupAuthorizationInformation In MyGroupAuths
                '        Try
                '            Dim SecObjID As String = MyGroupAuthInfo.SecurityObjectInfo.ID.ToString
                '            Dim SecObjName As String = MyGroupAuthInfo.SecurityObjectInfo.DisplayName
                '        Catch
                '            cammWebManager.Log.Warn("Missing security object with ID " & MyGroupAuthInfo.SecurityObjectID & " in authorizations for group ID " & MyGroupInfo.ID)
                '            Response.Write(MyGroupAuthInfo.SecurityObjectID)

                '        End Try
                '    Next
                'End If

                'Added by I-link on 07.08.2008 for setting Navigation Preview link.-----------
                If (Not Request.QueryString("IsAnonymousOrPublic") Is Nothing AndAlso CInt(Request.QueryString("IsAnonymousOrPublic")) = 0) Then
                    CType(Me.FindControl("cammWebManagerAdminGroupInfoDetails").FindControl("ancPreview"), HtmlAnchor).HRef = "users_navbar_preview.aspx?" + "GroupName=" + MyGroupInfo.Name + "&GroupId=" + MyGroupInfo.ID.ToString
                    CType(Me.FindControl("cammWebManagerAdminGroupInfoDetails").FindControl("ancPreview"), HtmlAnchor).InnerHtml = "Navigation Preview"
                End If
                '---------------------------------------


                If Not (Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Owner, CInt(Request("ID"))) OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Update, CInt(Request("ID")))) AndAlso Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.View, CInt(Request("ID"))) Then
                    ViewOnlyMode = True
                End If

                If Not (Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Owner, CInt(Request("ID"))) OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.Update, CInt(Request("ID"))) OrElse Me.CurrentAdminIsPrivilegedForItemAdministration(AdministrationItemType.Groups, AuthorizationTypeEffective.View, CInt(Request("ID")))) Then
                    Response.Clear()
                    Response.Write("No authorization to administrate this group.")
                    Response.End()
                ElseIf txtGroupName.Text.Trim = "" And txtDescription.Text.Trim = "" Then
                    Dim Mydt As DataTable
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                    Mydt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID, Name, Description FROM dbo.Gruppen WHERE ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                    If Mydt Is Nothing Then
                        lblErrMsg.Text = "Group not found!"
                    Else
                        txtGroupName.Text = Utils.Nz(Mydt.Rows(0)("Name"), String.Empty)
                        lblGroupName.Text = Server.HtmlEncode(Utils.Nz(Mydt.Rows(0)("Name"), String.Empty))
                        txtDescription.Text = Utils.Nz(Mydt.Rows(0)("description"), String.Empty)
                    End If
                End If

                If Not Page.IsPostBack Then
                    Dim dtGroupInfo As DataTable
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                    dtGroupInfo = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_groups WHERE ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    If dtGroupInfo Is Nothing Then
                        lblErrMsg.Text = "Group not found!"
                    Else
                        'if group is special like Supervisor etc. then groupname is not editable
                        Select Case CBool(dtGroupInfo.Rows(0)("systemgroup"))
                            Case True
                                txtGroupName.Visible = False
                                lblGroupName.Visible = True
                                Select Case CInt(Request.QueryString("id"))
                                    Case -7, 6, 7
                                    Case Else
                                        trMemberList.Style.Add("display", "none")
                                        trMemberList2.Style.Add("display", "none")
                                        trMemberList3.Style.Add("display", "none")
                                End Select
                            Case False
                                txtGroupName.Visible = True
                                lblGroupName.Visible = False
                        End Select

                        lblCreationDate.Text = Server.HtmlEncode(Utils.Nz(dtGroupInfo.Rows(0)("ReleasedOn"), String.Empty))
                        lblModificationDate.Text = Server.HtmlEncode(Utils.Nz(dtGroupInfo.Rows(0)("ModifiedOn"), String.Empty))
                        hypCreatedBy.NavigateUrl = "users_update.aspx?ID=" + Utils.Nz(dtGroupInfo.Rows(0)("ReleasedByID"), 0).ToString
                        hypCreatedBy.Text = Server.HtmlEncode(New CompuMaster.camm.webmanager.WMSystem.UserInformation(CLng(dtGroupInfo.Rows(0)("ReleasedByID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem), True).FullName)
                        hypModifiedBy.NavigateUrl = "users_update.aspx?ID=" + Utils.Nz(dtGroupInfo.Rows(0)("ModifiedByID"), 0).ToString
                        hypModifiedBy.Text = Server.HtmlEncode(New CompuMaster.camm.webmanager.WMSystem.UserInformation(CLng(dtGroupInfo.Rows(0)("ModifiedByID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem), True).FullName)
                    End If

                    Dim dtMembership As DataTable
                    Dim sqlParams1 As SqlParameter() = {New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                    dtMembership = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM dbo.view_eMailAccounts_of_Groups WHERE ID_Group=@ID", CommandType.Text, sqlParams1, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                    Dim MembersEMailList As String = ""
                    Dim iCount As Integer = 0
                    While iCount < dtMembership.Rows.Count
                        MembersEMailList = MembersEMailList & Server.HtmlDecode(Utils.Nz(dtMembership.Rows(iCount)("E-MAIL"), String.Empty)) & "; "
                        iCount += 1
                    End While
                    txtBccMail.Text = MembersEMailList
                    If ViewOnlyMode = False Then trUpdateGroup.Visible = True
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

#Region " Control Events "
        Protected Overridable Sub btnSubmitClcik(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            Try
                If (ViewOnlyMode = False And Request.QueryString("ID") <> "" And txtGroupName.Text.Trim <> "") Then
                    Dim CurUserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                    Try
                        Dim sqlParams As SqlParameter() = { _
                            New SqlParameter("@Name", Replace(txtGroupName.Text.Trim, "'", "''")), _
                            New SqlParameter("@Description", txtDescription.Text.Trim.Replace("'", "''")), _
                            New SqlParameter("@ModifiedBy", CurUserID), _
                            New SqlParameter("@ID", CInt(Val(Request.QueryString("ID") & "")))}
                        Dim mySqlQuery As String = "UPDATE dbo.Gruppen SET Name=@Name,Description=@Description,ModifiedOn=GetDate(),ModifiedBy=@ModifiedBy WHERE ID=@ID"
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), mySqlQuery, CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                        Response.Redirect("groups.aspx")
                    Catch ex As Exception
                        lblErrMsg.Text = "Group update failed!"
                    End Try
                Else
                    lblErrMsg.Text = "Please specify the field ""Group name"" to proceed!"
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

    End Class

End Namespace