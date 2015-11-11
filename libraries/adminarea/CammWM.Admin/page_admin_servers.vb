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

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.ServerList
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to view the list of servers
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	12.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ServerList
        Inherits Page

#Region "Variable Declaration"
        Protected lblGroupID, lblAreaNavTitle, lblMasterServerID, lblMemberServerID2 As Label
        Protected WithEvents rptServerList, rptServerSubList As Repeater
        Protected ancGroupID, ancServerGroup, ancAdminServer, ancMasterServer, lblGroupPublicName, ancDeleteServerGroup, ancAdd As HtmlAnchor
        Protected ancNew, ancDeleteServer, ancMemberServerDesc As HtmlAnchor
        Protected trShowDetails, trAddBlank, trShowMsg As HtmlTableRow
        Protected gcDisabled As HtmlGenericControl
        Dim MyDt As New DataTable
        Dim FirstServerLine, ServerIsDisabled As Boolean
        Dim OldServerGroupID, NewServerGroupID As Integer
        Dim TextColorOfLine As String
        Dim CurServerGroup As Object = Nothing
#End Region

#Region "Page Events"
        Private Sub ServerList_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            BindControls()
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub BindControls()
            Try
                CurServerGroup = cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "ID_ServerGroup")
                FirstServerLine = True
                MyDt = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT AdminPrivate_ServerRelations.* FROM [AdminPrivate_ServerRelations] ORDER BY ServerGroup, MemberServer_ServerDescription, MemberServer_IP", New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptServerList.DataSource = MyDt
                    rptServerList.DataBind()
                End If
            Catch ex As Exception
                Throw
            Finally
                MyDt.Dispose()
            End Try
        End Sub
#End Region

#Region "Control Events"
        Private Sub rptServerListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptServerList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                With MyDt.Rows(e.Item.ItemIndex)
                    OldServerGroupID = NewServerGroupID
                    NewServerGroupID = Utils.Nz(.Item("ID"), 0)

                    If NewServerGroupID <> OldServerGroupID Then
                        If FirstServerLine Then
                            FirstServerLine = False
                            CType(e.Item.FindControl("trAddBlank"), HtmlTableRow).Style.Add("display", "none")
                        End If

                        CType(e.Item.FindControl("ancGroupID"), HtmlAnchor).Name = "ServerGroup" & .Item("ID").ToString
                        CType(e.Item.FindControl("lblGroupID"), Label).Text = Utils.Nz(.Item("ID"), 0).ToString
                        CType(e.Item.FindControl("ancServerGroup"), HtmlAnchor).HRef = "servers_update_group.aspx?ID=" & .Item("ID").ToString
                        CType(e.Item.FindControl("ancServerGroup"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("ServerGroup"), String.Empty))
                        CType(e.Item.FindControl("lblAreaNavTitle"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("AreaNavTitle"), String.Empty))
                        CType(e.Item.FindControl("ancAdminServer"), HtmlAnchor).HRef = "servers_update_server.aspx?ID=" & .Item("UserAdminServer_ID").ToString
                        CType(e.Item.FindControl("ancAdminServer"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("UserAdminServer_ServerDescription"), String.Empty))
                        CType(e.Item.FindControl("ancMasterServer"), HtmlAnchor).HRef = "servers_update_server.aspx?ID=" & .Item("MasterServer_ID").ToString
                        CType(e.Item.FindControl("ancMasterServer"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("MasterServer_ServerDescription"), String.Empty))
                        CType(e.Item.FindControl("lblGroupPublicName"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Group_Public_Name"), String.Empty))
                        CType(e.Item.FindControl("ancAdd"), HtmlAnchor).HRef = "servers_new_accesslevelrelation.aspx?ID=" & .Item("ID").ToString

                        If CLng(CurServerGroup.ToString) <> CLng(.Item("ID").ToString) Then
                            CType(e.Item.FindControl("ancDeleteServerGroup"), HtmlAnchor).HRef = "servers_delete_group.aspx?ID=" & CLng(.Item("ID").ToString)
                            CType(e.Item.FindControl("ancDeleteServerGroup"), HtmlAnchor).InnerHtml = "Delete Server Group"
                        End If

                        'bind inner repeater control
                        Dim dt As New DataTable
                        Dim RecsFound As Boolean

                        Try
                            Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(.Item("ID").ToString))}
                            dt = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM AdminPrivate_ServerGroupAccessLevels WHERE ID_ServerGroup = @ID ORDER BY AccessLevels_Title", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                                CType(e.Item.FindControl("rptServerSubList"), Repeater).DataSource = dt
                                CType(e.Item.FindControl("rptServerSubList"), Repeater).DataBind()
                                RecsFound = True
                            End If
                        Catch ex As Exception
                            Throw
                        Finally
                            dt.Dispose()
                        End Try

                        If RecsFound = False Then CType(e.Item.FindControl("trShowMsg"), HtmlTableRow).Style.Add("display", "")
                    Else
                        CType(e.Item.FindControl("trShowDetails"), HtmlTableRow).Style.Add("display", "none")
                    End If

                    ServerIsDisabled = False
                    If Not IsDBNull(.Item("MemberServer_Enabled")) Then If Utils.Nz(.Item("MemberServer_Enabled"), False) = False Then ServerIsDisabled = True
                    If ServerIsDisabled = True Then TextColorOfLine = "gray" Else TextColorOfLine = "black"

                    CType(e.Item.FindControl("lblMasterServerID"), Label).Style.Add("color", TextColorOfLine)
                    CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).Style.Add("color", TextColorOfLine)
                    CType(e.Item.FindControl("lblMemberServerID2"), Label).Style.Add("color", TextColorOfLine)

                    CType(e.Item.FindControl("ancNew"), HtmlAnchor).HRef = "servers_new_server.aspx?ID=" & .Item("ID").ToString
                    CType(e.Item.FindControl("lblMasterServerID"), Label).Text = .Item("MemberServer_ID").ToString
                    If ServerIsDisabled Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).InnerHtml = "<nobr title=""Disabled"">(D)</nobr>"

                    CType(e.Item.FindControl("ancMemberServerDesc"), HtmlAnchor).HRef = "servers_update_server.aspx?ID=" & .Item("MemberServer_ID").ToString
                    CType(e.Item.FindControl("ancMemberServerDesc"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("MemberServer_ServerDescription"), String.Empty))
                    CType(e.Item.FindControl("lblMemberServerID2"), Label).Text = .Item("MemberServer_IP").ToString

                    If .Item("MasterServer_ID").ToString <> .Item("MemberServer_ID").ToString And .Item("UserAdminServer_ID").ToString <> .Item("MemberServer_ID").ToString Then
                        CType(e.Item.FindControl("ancDeleteServer"), HtmlAnchor).HRef = "servers_delete_server.aspx?ID=" & .Item("MemberServer_ID").ToString
                        CType(e.Item.FindControl("ancDeleteServer"), HtmlAnchor).InnerHtml = "Delete Server"
                    End If
                End With
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.NewServerGroup
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to add a new server group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	11.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NewServerGroup
        Inherits Page

#Region "Variable Declaration Section"
        Protected txtGroupname, txtEmail As TextBox
        Protected lblServerGroupName, lblServerGroupId, lblErrMsg As Label
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub NewServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not (Page.IsPostBack) Then
                Dim CurUserID As Long
                CurUserID = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                txtGroupname.Text = ""
                txtEmail.Text = cammWebManager.CurrentUserInfo.EMailAddress
            End If
        End Sub
#End Region

#Region "Control Events"
        Protected Sub btnSubmitClcik(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If txtGroupname.Text.Trim <> "" And txtEmail.Text.Trim <> "" Then
                Dim sqlParams As SqlParameter() = { _
                                    New SqlParameter("@GroupName", Mid(Trim(txtGroupname.Text), 1, 255)), _
                                    New SqlParameter("@email_Developer", Mid(Trim(txtEmail.Text), 1, 255)), _
                                    New SqlParameter("@UserID_Creator", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) _
                                                  }

                Dim Redirect2URL As String = ""

                Try
                    Dim obj As Object = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar( _
                                                        New SqlConnection(cammWebManager.ConnectionString), _
                                                        "AdminPrivate_CreateServerGroup", _
                                                        CommandType.StoredProcedure, _
                                                        sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection _
                                                      )

                    If obj Is Nothing Then
                        lblErrMsg.Text = "Undefined error detected!"
                    ElseIf CLng(obj) <> 0 Then
                        Redirect2URL = "servers.aspx#ServerGroup" & Utils.Nz(obj, String.Empty)
                    Else
                        lblErrMsg.Text = "Server group creation failed!"
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = "Server group creation failed! (" & ex.InnerException.Message & ")"
                End Try

                If Redirect2URL.Trim <> "" Then Response.Redirect(Redirect2URL)
            Else
                lblErrMsg.Text = "Please specify a name for the server group and a general e-mail address for this new server group."
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.DeleteServerGroup
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete a server group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	10.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DeleteServerGroup
        Inherits Page

#Region "Variable Declaration"
        Protected lblServerGroupName, lblServerGroupId, lblErrMsg As Label
#End Region

#Region "Page Events"
        Private Sub DeleteServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID_ServerGroup", CLng(Request.QueryString("ID")))}

                Try
                    CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteServerGroup", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch ex As Exception
                    lblErrMsg.Text = "Server group erasing failed! (" & Server.HtmlEncode(ex.Message) & ")"
                End Try
            Else
                Dim MyServerGroupInfo As New CompuMaster.camm.webmanager.WMSystem.ServerGroupInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                lblServerGroupId.Text = Server.HtmlEncode(Request.QueryString("ID"))
                lblServerGroupName.Text = Server.HtmlEncode(MyServerGroupInfo.Title)
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.UpdateServerGroup
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to Update a server group
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	19.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UpdateServerGroup
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblFieldID, lblGroupInfo As Label
        Protected _
                txtFieldServerGroup, txtAreaNavTitle, txtAreaCompanyFormerTitle, txtAreaButton, txtAreaImage, _
                txtAreaCompanyTitle, txtAreaCopyRightSinceYear, txtAreaCompanyWebSiteURL, _
                txtAreaCompanyWebSiteTitle, txtAreaSecurityContactEMail, txtAreaSecurityContactTitle, _
                txtAreaContentManagementContactEMail, txtAreaContentManagementContactTitle, txtAreaUnspecifiedContactEMail, _
                txtAreaDevelopmentContactEMail, txtAreaDevelopmentContactTitle, txtAreaUnspecifiedContactTitle _
                As TextBox
        Protected hiddenTxt_ID_Group_Public, hiddenTxt_GroupAnonymous As HtmlInputHidden
        Protected cmbUserAdminServer, cmbMasterServer, cmbAccessLevelDefault As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected hypIdGroupPublic As HyperLink
#End Region

#Region "Page Events"
        Private Sub UpdateServerGroup_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ID_Group_Public As Integer
            Dim Field_ID_Group_Anonymous As Integer

            If Not Page.IsPostBack Then
                FillDropDownLists(Utils.Nz(Request.QueryString("ID"), 0))
                Dim dtServerGroup As DataTable

                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    dtServerGroup = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "SELECT  top 1 * FROM dbo.System_ServerGroups WHERE ID = @ID", CommandType.Text, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    lblFieldID.Text = Utils.Nz(dtServerGroup.Rows(0)("ID"), 0).ToString
                    txtFieldServerGroup.Text = Utils.Nz(dtServerGroup.Rows(0)("ServerGroup"), String.Empty)
                    Field_ID_Group_Public = Utils.Nz(dtServerGroup.Rows(0)("ID_Group_Public"), 0)
                    Field_ID_Group_Anonymous = Utils.Nz(dtServerGroup.Rows(0)("ID_Group_Anonymous"), 0)
                    Dim MyGroupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(Field_ID_Group_Anonymous, CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                    hiddenTxt_GroupAnonymous.Value = Utils.Nz(MyGroupInfo.ID, 0).ToString
                    lblGroupInfo.Text = Server.HtmlEncode(MyGroupInfo.Name)
                    MyGroupInfo = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(Field_ID_Group_Public, CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                    hiddenTxt_ID_Group_Public.Value = Utils.Nz(MyGroupInfo.ID, 0).ToString
                    hypIdGroupPublic.Text = Server.HtmlEncode(MyGroupInfo.Name)
                    hypIdGroupPublic.NavigateUrl = "groups_update.aspx?ID=" + MyGroupInfo.ID.ToString

                    cmbMasterServer.SelectedValue = Utils.Nz(dtServerGroup.Rows(0)("MasterServer"), String.Empty)
                    cmbUserAdminServer.SelectedValue = Utils.Nz(dtServerGroup.Rows(0)("UserAdminServer"), String.Empty)
                    txtAreaButton.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaButton"), String.Empty)
                    txtAreaImage.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaImage"), String.Empty)
                    txtAreaNavTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaNavTitle"), String.Empty)
                    txtAreaCompanyFormerTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyFormerTitle"), String.Empty)
                    txtAreaCompanyTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyTitle"), String.Empty)
                    txtAreaSecurityContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaSecurityContactEMail"), String.Empty)
                    txtAreaSecurityContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaSecurityContactTitle"), String.Empty)
                    txtAreaDevelopmentContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaDevelopmentContactEMail"), String.Empty)
                    txtAreaDevelopmentContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaDevelopmentContactTitle"), String.Empty)
                    txtAreaContentManagementContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaContentManagementContactEMail"), String.Empty)
                    txtAreaContentManagementContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaContentManagementContactTitle"), String.Empty)
                    txtAreaUnspecifiedContactEMail.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaUnspecifiedContactEMail"), String.Empty)
                    txtAreaUnspecifiedContactTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaUnspecifiedContactTitle"), String.Empty)
                    txtAreaCopyRightSinceYear.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCopyRightSinceYear"), String.Empty)
                    txtAreaCompanyWebSiteURL.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyWebSiteURL"), String.Empty)
                    txtAreaCompanyWebSiteTitle.Text = Utils.Nz(dtServerGroup.Rows(0)("AreaCompanyWebSiteTitle"), String.Empty)
                    cmbAccessLevelDefault.SelectedValue = Utils.Nz(dtServerGroup.Rows(0)("AccessLevel_Default"), String.Empty)
                Catch ex As Exception
                    Throw
                End Try
            End If
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub FillDropDownLists(ByVal Field_ID As Integer)
            Try
                Dim dtAccess As DataTable
                dtAccess = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [dbo].[System_AccessLevels].id, [dbo].[System_AccessLevels].title FROM [dbo].[System_AccessLevels]", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not dtAccess Is Nothing AndAlso dtAccess.Rows.Count > 0 Then
                    For Each dr As DataRow In dtAccess.Rows
                        cmbAccessLevelDefault.Items.Add(New ListItem(Utils.Nz(dr("Title"), String.Empty), Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If

                dtAccess.Dispose()

                Dim dtAccessLevel As DataTable
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ServerGroup", Field_ID)}
                dtAccessLevel = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "SELECT ID,ServerDescription, IP  FROM System_Servers WHERE ServerGroup = @ServerGroup", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data1")

                If Not dtAccessLevel Is Nothing AndAlso dtAccessLevel.Rows.Count > 0 Then
                    For Each dr As DataRow In dtAccessLevel.Rows
                        cmbMasterServer.Items.Add(New ListItem(Utils.Nz(dr("ServerDescription"), String.Empty) + " (" + Utils.Nz(dr("IP"), String.Empty) + ")", Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If

                dtAccessLevel.Dispose()

                Dim dtSystem As DataTable
                dtSystem = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID,ServerDescription, IP  FROM System_Servers WHERE Enabled <> 0", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data2")
                dtSystem.Dispose()

                If Not dtSystem Is Nothing AndAlso dtSystem.Rows.Count > 0 Then
                    For Each dr As DataRow In dtSystem.Rows
                        cmbUserAdminServer.Items.Add(New ListItem(Utils.Nz(dr("ServerDescription"), String.Empty) + " (" + Utils.Nz(dr("IP"), String.Empty) + ")", Utils.Nz(dr("ID"), 0).ToString))
                    Next
                End If
            Catch ex As Exception
                Throw
            End Try
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If lblFieldID.Text <> Nothing And _
             txtFieldServerGroup.Text.Trim <> "" And _
             hiddenTxt_ID_Group_Public.Value.Trim <> "" And _
             hiddenTxt_GroupAnonymous.Value.Trim <> "" And _
             cmbMasterServer.SelectedValue <> "" And _
             cmbUserAdminServer.SelectedValue <> "" And _
             txtAreaButton.Text.Trim <> "" And _
             txtAreaImage.Text.Trim <> "" And _
              txtAreaCompanyFormerTitle.Text.Trim <> "" And _
             txtAreaCompanyTitle.Text.Trim <> "" And _
             txtAreaSecurityContactEMail.Text.Trim <> "" And _
             txtAreaSecurityContactTitle.Text.Trim <> "" And _
             txtAreaDevelopmentContactEMail.Text.Trim <> "" And _
             txtAreaDevelopmentContactTitle.Text.Trim <> "" And _
             txtAreaContentManagementContactEMail.Text.Trim <> "" And _
             txtAreaContentManagementContactTitle.Text.Trim <> "" And _
             txtAreaUnspecifiedContactEMail.Text.Trim <> "" And _
             txtAreaUnspecifiedContactTitle.Text.Trim <> "" And _
             txtAreaCopyRightSinceYear.Text.Trim <> "" And _
             txtAreaCompanyWebSiteURL.Text.Trim <> "" And _
             txtAreaCompanyWebSiteTitle.Text.Trim <> "" And _
             cmbAccessLevelDefault.SelectedValue <> "" Then

                Dim sqlParams As SqlParameter() = { _
                                                            New SqlParameter("@ID", lblFieldID.Text), _
                                                            New SqlParameter("@ServerGroup", Mid(Trim(txtFieldServerGroup.Text.Trim), 1, 255)), _
                                                            New SqlParameter("@ID_Group_Public", CInt(hiddenTxt_ID_Group_Public.Value.Trim)), _
                                                            New SqlParameter("@ID_Group_Anonymous", CInt(hiddenTxt_GroupAnonymous.Value.Trim)), _
                                                            New SqlParameter("@MasterServer", CInt(cmbMasterServer.SelectedValue)), _
                                                            New SqlParameter("@UserAdminServer", CInt(cmbUserAdminServer.SelectedValue)), _
                                                            New SqlParameter("@AreaImage", Mid(Trim(txtAreaImage.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaButton", Mid(Trim(txtAreaButton.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaNavTitle", Mid(Trim(txtAreaNavTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCompanyFormerTitle", Mid(Trim(txtAreaCompanyFormerTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCompanyTitle", Mid(Trim(txtAreaCompanyTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaSecurityContactEMail", Mid(Trim(txtAreaSecurityContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaSecurityContactTitle", Mid(Trim(txtAreaSecurityContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaDevelopmentContactEMail", Mid(Trim(txtAreaDevelopmentContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaDevelopmentContactTitle", Mid(Trim(txtAreaDevelopmentContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaContentManagementContactEMail", Mid(Trim(txtAreaContentManagementContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaContentManagementContactTitle", Mid(Trim(txtAreaContentManagementContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaUnspecifiedContactEMail", Mid(Trim(txtAreaUnspecifiedContactEMail.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaUnspecifiedContactTitle", Mid(Trim(txtAreaUnspecifiedContactTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCopyRightSinceYear", CInt(txtAreaCopyRightSinceYear.Text.Trim)), _
                                                            New SqlParameter("@AreaCompanyWebSiteURL", Mid(Trim(txtAreaCompanyWebSiteURL.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@AreaCompanyWebSiteTitle", Mid(Trim(txtAreaCompanyWebSiteTitle.Text.Trim), 1, 512)), _
                                                            New SqlParameter("@ModifiedBy", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), _
                                                            New SqlParameter("@AccessLevel_Default", CInt(cmbAccessLevelDefault.SelectedValue)) _
                                                        }

                Try
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateServerGroup", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch
                    lblErrMsg.Text = "Server group update failed!"
                End Try
            Else
                lblErrMsg.Text = "Please specify all relevant server group details to proceed."
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.Delete_ServersAccesslevelrelation
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete a Server Lveel relation.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	10.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Delete_ServersAccesslevelrelation
        Inherits Page

#Region "Variable Declaration"
        Protected hypDelete As HyperLink
        Protected lblAccessLevel, lblServerGroup, lblRelationId, lblErrMsg As Label
#End Region

#Region "Page Events"
        Private Sub Delete_ServersAccesslevelrelation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            If Request.QueryString("ID") <> "" AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "DELETE FROM System_ServerGroupsAndTheirUserAccessLevels WHERE ID = @ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch
                    lblErrMsg.Text = "Removing of access level relation failed! "
                End Try
            Else
                Dim dtServerDetail As DataTable = Nothing

                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                    dtServerDetail = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT  dbo.System_ServerGroupsAndTheirUserAccessLevels.ID, dbo.System_AccessLevels.Title, dbo.System_ServerGroups.ServerGroup FROM dbo.System_ServerGroupsAndTheirUserAccessLevels LEFT OUTER JOIN dbo.System_AccessLevels ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_AccessLevel = dbo.System_AccessLevels.ID LEFT OUTER JOIN dbo.System_ServerGroups ON dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = dbo.System_ServerGroups.ID WHERE dbo.System_ServerGroupsAndTheirUserAccessLevels.ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                    If dtServerDetail Is Nothing Then
                        lblErrMsg.Text = "Access level relation not found!"
                    Else
                        lblRelationId.Text = Request.QueryString("ID")
                        lblAccessLevel.Text = Server.HtmlEncode(Utils.Nz(dtServerDetail.Rows(0)("Title"), String.Empty))
                        lblServerGroup.Text = Server.HtmlEncode(Utils.Nz(dtServerDetail.Rows(0)("ServerGroup"), String.Empty))
                        hypDelete.NavigateUrl = "servers_delete_accesslevelrelation.aspx?ID=" + Request.QueryString("ID") + "&DEL=NOW&token=" & Session.SessionID
                        hypDelete.Text = "Yes, delete it!"
                    End If
                Catch ex As Exception
                    Throw
                Finally
                    dtServerDetail.Dispose()
                End Try
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.Add_ServersAccesslevelrelation
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to create a Server Level relation.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	15.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Add_ServersAccesslevelrelation
        Inherits Page

#Region "Variable Declaration"
        Protected cmbAccessLevel As DropDownList
        Protected lblErrMsg As Label
        Protected WithEvents btnSubmit As Button
#End Region

#Region "Page Events"
        Private Sub Add_ServersAccesslevelrelation_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not Page.IsPostBack Then FillDropDownList()
        End Sub
#End Region

#Region "User-Defined Methods"
        Private Sub FillDropDownList()
            Dim dtAccessLevel As DataTable = Nothing

            Try
                Trace.Warn("Start retrive data")
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Val(Request.QueryString("ID") & "")))}
                dtAccessLevel = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID,Title FROM dbo.System_AccessLevels WHERE (ID NOT IN (SELECT ID_AccessLevel FROM dbo.System_ServerGroupsAndTheirUserAccessLevels WHERE  dbo.System_ServerGroupsAndTheirUserAccessLevels.ID_ServerGroup = @ID))", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                Trace.Warn("End retrive data")
                cmbAccessLevel.Items.Clear()

                If Not dtAccessLevel Is Nothing AndAlso dtAccessLevel.Rows.Count > 0 Then
                    Dim drBlank As DataRow = dtAccessLevel.NewRow()
                    drBlank("ID") = -1
                    drBlank("Title") = "Please select!"
                    dtAccessLevel.Rows.InsertAt(drBlank, 0)

                    For Each drow As DataRow In dtAccessLevel.Rows
                        cmbAccessLevel.Items.Add(New ListItem(drow("Title").ToString, drow("ID").ToString))
                    Next
                End If

                'cmbAccessLevel.DataSource = dtAccessLevel
                'cmbAccessLevel.DataTextField = "Title"
                'cmbAccessLevel.DataValueField = "ID"
            Catch ex As Exception
                Throw
            Finally
                dtAccessLevel.Dispose()
            End Try
        End Sub
#End Region

#Region "Control Events"
        Protected Sub btnSubmit_click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            If cmbAccessLevel.Items.Count > 0 AndAlso Utils.Nz(cmbAccessLevel.SelectedValue, 0) <> -1 AndAlso Request.QueryString("ID") <> "" Then
                Try
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(cmbAccessLevel.SelectedValue)), _
                        New SqlParameter("@ID_ServerGroup", CLng(Request.QueryString("ID")))}
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "INSERT INTO System_ServerGroupsAndTheirUserAccessLevels (ID_AccessLevel, ID_ServerGroup) SELECT @ID AS ID_AccessLevel, @ID_ServerGroup AS ID_ServerGroup", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx#ServerGroup" & Request.QueryString("ID"))
                Catch
                    lblErrMsg.Text = "Adding of access level failed!"
                End Try
            Else
                lblErrMsg.Text = "Please specify a name for the server group and a general e-mail address for this new server group."
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.AddNewServer
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to add new server
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	16.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AddNewServer
        Inherits Page

#Region "Variable Declaration"
        Protected txtServerIP As TextBox
        Protected WithEvents btnSubmit As Button
        Protected lblErrMsg As Label
#End Region

#Region "Control Events"
        Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
            If txtServerIP.Text.Trim <> "" And Request.QueryString("ID") <> "" Then
                Dim CurUserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ServerIP", Mid(Trim(txtServerIP.Text.Trim), 1, 256)), New SqlParameter("@ServerGroup", CLng(Request.QueryString("ID")))}
                Try
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_CreateServer", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx#ServerGroup" & Request.Form("servergroupid"))
                Catch ex As CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.DataException
                    If ex.ToString.IndexOf("UNIQUE KEY") > 0 Then
                        lblErrMsg.Text = "IP / Host Header already exists"
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = ex.Message
                End Try
            Else
                lblErrMsg.Text = "Please specify a name for the server group and a general e-mail address for this new server group."
            End If
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.UpdateServer
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to update a server
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link]	11.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UpdateServer
        Inherits Page

#Region "Variable Declaration"
        Protected lblErrMsg, lblGroupID As Label
        Protected txtHostHeader, txtDescription, txtPortNumber, txtServerName, txtProtocal As TextBox
        Protected cmbServerGroup, cmbEnabled As DropDownList
        Protected WithEvents btnSubmit As Button
        Protected trEnableCombo, trEnabledMsg, trNotMasterServer, trMasterServer As HtmlTableRow
        Protected WithEvents rptEngine As Repeater
        Protected hypServerURl As HyperLink
        Private srtEngineDetail As SortedList
#End Region

#Region "Page Events"
        Private Sub UpdateServer_Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim i As Integer = rptEngine.Items.Count

            If (srtEngineDetail Is Nothing) Then
                srtEngineDetail = New SortedList
                For Each item As RepeaterItem In rptEngine.Items
                    Select Case item.ItemType
                        Case ListItemType.AlternatingItem, ListItemType.Item
                            Dim strEngineId As String = CType(item.FindControl("EngineId"), HtmlInputHidden).Value
                            Dim cmbEngine As DropDownList = CType(item.FindControl("cmbEngine"), DropDownList)
                            srtEngineDetail.Add(Server.HtmlEncode(strEngineId), cmbEngine.SelectedValue)
                    End Select
                Next
            End If

            If Not Page.IsPostBack Then
                Dim strWebURL As String = Response.ApplyAppPathModifier(Request.ServerVariables("SCRIPT_NAME")) & "?" & Utils.QueryStringWithoutSpecifiedParameters(Nothing)

                Dim Field_ID As Integer
                Dim Field_IsAdminServer As Boolean
                Dim Field_IsMasterServer As Boolean
                Dim Field_AddrName As String

                Dim dtServer As DataTable
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                dtServer = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT top 1 dbo.System_Servers.*, Case When System_ServerGroups_1.MasterServer Is Not Null Then 1 Else 0 End As IsMasterServer, Case When dbo.System_ServerGroups.UserAdminServer Is Not Null Then 1 Else 0 End As IsAdminServer FROM dbo.System_Servers LEFT OUTER JOIN dbo.System_ServerGroups ON dbo.System_Servers.ID = dbo.System_ServerGroups.UserAdminServer LEFT OUTER JOIN dbo.System_ServerGroups System_ServerGroups_1 ON dbo.System_Servers.ID = System_ServerGroups_1.MasterServer WHERE dbo.System_Servers.ID=@ID", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                If Not dtServer Is Nothing Then
                    Field_ID = Utils.Nz(Request.QueryString("ID"), 0)
                    lblGroupID.Text = Utils.Nz(Field_ID, 0).ToString
                    Field_IsMasterServer = Utils.Nz(dtServer.Rows(0)("IsMasterServer"), False)

                    If (Utils.Nz(Field_IsMasterServer, False) = False) Then
                        trMasterServer.Visible = True
                        trNotMasterServer.Visible = False
                        FillServerGroupCombo()
                        cmbServerGroup.SelectedValue = Utils.Nz(dtServer.Rows(0)("ServerGroup"), String.Empty)
                    Else
                        trMasterServer.Visible = False
                        trNotMasterServer.Visible = True
                        FillServerGroupCombo()
                        cmbServerGroup.SelectedValue = Utils.Nz(dtServer.Rows(0)("ServerGroup"), String.Empty)
                    End If

                    Field_IsAdminServer = Utils.Nz(dtServer.Rows(0)("IsAdminServer"), False)
                    FillEnabledCombo(cmbEnabled)

                    If CLng(cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "ID")) = CLng(Field_ID) And Utils.Nz(Field_IsAdminServer, False) <> False Then
                        trEnabledMsg.Visible = True
                        trEnableCombo.Visible = False
                        cmbEnabled.SelectedValue = "1"
                    Else
                        trEnableCombo.Visible = True
                        trEnabledMsg.Visible = False

                        If Utils.Nz(dtServer.Rows(0)("Enabled"), False) Then
                            cmbEnabled.SelectedValue = "1"
                        Else
                            cmbEnabled.SelectedValue = "0"
                        End If
                    End If

                    txtHostHeader.Text = Utils.Nz(dtServer.Rows(0)("IP"), String.Empty)
                    txtDescription.Text = Utils.Nz(dtServer.Rows(0)("ServerDescription"), String.Empty)
                    txtProtocal.Text = Utils.Nz(dtServer.Rows(0)("ServerProtocol"), String.Empty)
                    Field_AddrName = Utils.Nz(dtServer.Rows(0)("ServerName"), String.Empty)
                    txtServerName.Text = Utils.Nz(dtServer.Rows(0)("ServerName"), String.Empty)
                    txtPortNumber.Text = Utils.Nz(dtServer.Rows(0)("ServerPort"), String.Empty)
                    hypServerURl.Text = "event log"
                    hypServerURl.NavigateUrl = GetServerURL(Field_ID) + "/sysdata/servereventlog.aspx"
                Else
                    lblErrMsg.Text = "Server not found"
                End If
            End If
        End Sub

        Private Sub UpdateServer_Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            BindDataList()
        End Sub
#End Region

#Region "User-Defined functions"
        Private Sub FillEnabledCombo(ByRef cmbEngine As DropDownList)
            cmbEngine.Items.Add(New ListItem("Yes", "1"))
            cmbEngine.Items.Add(New ListItem("No", "0"))
        End Sub

        Private Sub FillServerGroupCombo()
            Dim dtServerGroup As DataTable
            cmbServerGroup.Items.Clear()
            dtServerGroup = FillDataTable(New SqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT * FROM System_ServerGroups", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If Not dtServerGroup Is Nothing AndAlso dtServerGroup.Rows.Count > 0 Then
                For Each dr As DataRow In dtServerGroup.Rows
                    cmbServerGroup.Items.Add(New ListItem(Utils.Nz(dr("ServerGroup"), String.Empty), dr("ID").ToString))
                Next
            End If

            'cmbServerGroup.DataSource = dtServerGroup
            'cmbServerGroup.DataTextField = "ServerGroup"
            'cmbServerGroup.DataValueField = "ID"
        End Sub

        Private Function GetServerURL(ByVal ServerID As Integer) As String
            Dim ServerInfo As New CompuMaster.camm.WebManager.WMSystem.ServerInformation(ServerID, CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
            Return ServerInfo.ServerURL
        End Function

        Private Sub BindDataList()
            Dim sqlParamsScript As SqlParameter() = {New SqlParameter("@ServerID", CLng(Request.QueryString("ID")))}
            rptEngine.DataSource = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_GetScriptEnginesOfServer", CommandType.StoredProcedure, sqlParamsScript, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            rptEngine.DataBind()
        End Sub
#End Region

#Region "Control Events"
        Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
            lblErrMsg.Text = ""
            If Request.QueryString("ID") <> Nothing And txtHostHeader.Text.Trim <> "" And txtDescription.Text.Trim <> "" And cmbServerGroup.SelectedValue <> Nothing And txtProtocal.Text.Trim <> "" And txtServerName.Text.Trim <> "" Then

                Dim atLeastOneScriptEngineActivated As Boolean = False
                For myScriptEngineCounter As Integer = 0 To srtEngineDetail.Count - 1
                    If CType(srtEngineDetail.GetByIndex(myScriptEngineCounter), Integer) = 1 Then
                        atLeastOneScriptEngineActivated = True
                        Exit For
                    End If
                Next
                If Not atLeastOneScriptEngineActivated Then
                    lblErrMsg.Text = "No script engine selected. Standard script engine was activated now."
                End If

                Dim MyRec As Object

                Try
                    If (txtPortNumber.Text.Trim <> "") Then
                        If Not IsNumeric(txtPortNumber.Text.Trim) Then
                            lblErrMsg.Text = "Port Number should be numeric"
                            Exit Sub
                        End If
                    End If
                    Dim sqlParams As SqlParameter() = {New SqlParameter("@Enabled", CBool(cmbEnabled.SelectedValue)), _
                                                   New SqlParameter("@IP", Mid(Trim(txtHostHeader.Text), 1, 256)), _
                                                   New SqlParameter("@ServerDescription", Mid(Trim(txtDescription.Text), 1, 200)), _
                                                   New SqlParameter("@ServerGroup", CLng(cmbServerGroup.SelectedValue)), _
                                                   New SqlParameter("@ServerProtocol", Mid(Trim(txtProtocal.Text), 1, 200)), _
                                                   New SqlParameter("@ServerName", Mid(Trim(txtServerName.Text), 1, 200)), _
                                                   New SqlParameter("@ServerPort", IIf(txtPortNumber.Text.Trim = "", DBNull.Value, txtPortNumber.Text.Trim)), _
                                                   New SqlParameter("@ID", Request.QueryString("ID"))}

                    MyRec = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_UpdateServer", CommandType.StoredProcedure, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)

                    Dim i As Integer
                    For i = 0 To srtEngineDetail.Count - 1
                        Dim sqlParam As SqlParameter() = { _
                                                           New SqlParameter("@ScriptEngineID", srtEngineDetail.GetKey(i)), _
                                                           New SqlParameter("@ServerID", Request.QueryString("ID")), _
                                                           New SqlParameter("@Enabled", srtEngineDetail.GetByIndex(i)) _
                                                       }
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetScriptEngineActivation", CommandType.StoredProcedure, sqlParam, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Next


                    srtEngineDetail = Nothing
                    Dim sqlParamsScript As SqlParameter() = { _
                                                   New SqlParameter("@ScriptEngineID", "0"), _
                                                   New SqlParameter("@ServerID", Request.QueryString("ID")), _
                                                   New SqlParameter("@Enabled", False), _
                                                   New SqlParameter("@CheckMinimalActivations", True) _
                                                 }

                    Try
                        ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetScriptEngineActivation", CommandType.StoredProcedure, sqlParamsScript, Automations.AutoOpenAndCloseAndDisposeConnection)
                        If lblErrMsg.Text = "" Then
                            Response.Redirect("servers.aspx")
                        End If
                    Catch ex As Exception
                        If cammWebManager.System_DebugLevel >= 3 Then
                            lblErrMsg.Text = "Server update failed! (" & ex.message & ex.stacktrace & ")"
                        Else
                            lblErrMsg.Text = "Server update failed!"
                        End If
                    End Try
                Catch ex As CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.DataException
                    If ex.ToString.IndexOf("IP / Host Header already exists") > 0 Then
                        lblErrMsg.Text = "IP / Host Header already exists"
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = ex.message
                End Try
            End If
        End Sub

        Private Sub rptEngine_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptEngine.ItemDataBound
            Select Case e.Item.ItemType
                Case ListItemType.AlternatingItem, ListItemType.Item
                    Dim cmbEngine As DropDownList = CType(e.Item.FindControl("cmbEngine"), DropDownList)
                    FillEnabledCombo(cmbEngine)
                    Dim drCurrent As DataRowView = CType(e.Item.DataItem, DataRowView)
                    If Utils.Nz(drCurrent("IsActivated"), False) = False Then
                        cmbEngine.SelectedValue = "0"
                    Else
                        cmbEngine.SelectedValue = "1"
                    End If
            End Select
        End Sub
#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.DeleteServer
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A page to delete server
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[I-link] 16.10.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DeleteServer
        Inherits Page

#Region "Variable Declaration"
        Protected lblServer, lblIP, lblDescription, lblAddress, lblErrMsg As Label
        Protected hypDelete As HyperLink
#End Region

#Region "Page Events"
        Private Sub DeleteServer_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim Field_ServerIP As String
            Dim Field_ServerDescription As String
            Dim Field_ServerAddress As String
            If Request.QueryString("ID") <> "" And Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                Dim sqlParam As SqlParameter() = {New SqlParameter("@ServerID", CLng(Request.QueryString("ID")))}
                Try
                    ExecuteNonQuery(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_DeleteServer", CommandType.StoredProcedure, sqlParam, Automations.AutoOpenAndCloseAndDisposeConnection)
                    Response.Redirect("servers.aspx")
                Catch
                    lblErrMsg.Text = "Removing of server failed! "
                End Try
            Else
                Dim MyServerInfo As New CompuMaster.camm.webmanager.WMSystem.ServerInformation(CInt(Request.QueryString("ID")), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                Field_ServerIP = MyServerInfo.IPAddressOrHostHeader
                Field_ServerDescription = MyServerInfo.Description
                Field_ServerAddress = MyServerInfo.ServerURL
                lblServer.Text = Server.HtmlEncode(Utils.Nz(Request.QueryString("ID"), 0).ToString)
                lblIP.Text = Server.HtmlEncode(Utils.Nz(Field_ServerIP, String.Empty))
                lblDescription.Text = Server.HtmlEncode(Utils.Nz(Field_ServerDescription, String.Empty))
                lblAddress.Text = Server.HtmlEncode(Utils.Nz(Field_ServerAddress, String.Empty))
                hypDelete.Text = "Yes, delete it!"
                hypDelete.NavigateUrl = "servers_delete_server.aspx?ID=" + Server.HtmlEncode(Request.QueryString("ID")) + "&DEL=NOW&token=" & Session.SessionID
            End If
        End Sub
#End Region

    End Class

End Namespace