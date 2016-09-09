Option Strict Off
Option Explicit On 

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager
Imports CompuMaster.Data
Imports CompuMaster.camm.WebManager.Controls.Administration
'Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

    Public Class User_WeddingAuthTransfer
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

#Region "Variable Declaration"
        Dim ShowUserDetailsComplete, ShowUnlockStatus, IsVisible, IsVisibleTrue As Boolean
        Dim dt As New DataTable
        Dim MyCount As Integer
        Dim StoreValue As String
        Protected lblID, lblCompany, lblSupplierNO, lblUpdateSupplierNO, lblUpdateCustomerNO, lblCustomerNO, lblAnrede, lblTitel, lblVorname, lblNachname, lblNamenszusatz As Label
        Protected lblStrasse, lblPLZ, lblORT, lblState, lblLand, lblLoginCount, lblLoginFailures, lblLoginLockedTill, lblLoginDisabled As Label
        Protected lblAccountAccessability, lblCreatedOn, lblModifiedOn, lblLastLoginOn, lblLastLoginViaremoteIP, lblFirstPreferredLanguage, lblSecondPreferredLanguage, lblThirdPreferredLanguage As Label
        Protected lblErrMsgPrevious, lblErrMsgFuture, lblTemporaryStatus, lblCustomerHadline, lblPermanentStatus, lblLoginAccessability, lblLoginAccountAccessability, lblLogonStatus, lblAdminBlockFooter As Label
        Protected WithEvents txtLoginNamePrevious As TextBox
        Protected WithEvents txtLoginNameFuture As TextBox
		Protected MsgSuccessLabel, MsgEMailTemplate as Label
        Protected ancUserListPrevious, ancUserListFuture, ancLoginName, ancEmail, ancPermanentStatus As HtmlAnchor
        Protected WithEvents btnStartTransfer, btnShowUserDetailsPrevious, btnShowUserDetailsFuture As Button
        Protected WithEvents rptUserShowPrevious As Repeater
        Protected WithEvents rptUserShowFuture As Repeater
        Protected cammWebManagerAdminUserInfoDetailsPrevious As UsersAdditionalInformation
        Protected cammWebManagerAdminUserInfoDetailsFuture As UsersAdditionalInformation
		protected cammWebManagerAdminUserInfoAdditionalFlagsPrevious, cammWebManagerAdminUserInfoAdditionalFlagsFuture as Object
        Protected block_customersupplierdata As Object
        Protected tdAddDataTable As HtmlTableCell
        Protected trErrMsgPrevious, trUserLinkPrevious As HtmlTableRow
        Protected trErrMsgFuture, trUserLinkFuture As HtmlTableRow
        Protected tableMemberships As HtmlControl
#End Region
		Protected UserInfoPrevious as CompuMaster.camm.WebManager.WMSystem.UserInformation
		Protected UserInfoFuture as CompuMaster.camm.WebManager.WMSystem.UserInformation

#Region "Page Event"
        Private Sub PageInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            cammWebManager.PageAdditionalBodyAttributes("onLoad") = "if (window['navPreview']) { document.forms['LookupUser'].LoginName.focus();};"
        End Sub

        Private Sub PageLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			MsgSuccessLabel.Text = Nothing
			MsgEMailTemplate.Visible = False
			VisibleControlPrevious(False, False)
			VisibleControlFuture(False, False)
			ErrorMsgPreviousVisible(False, False)
			ErrorMsgFutureVisible(False, False)
            If Not IsPostBack Then
		        txtLoginNamePrevious.Attributes.Add("onkeypress", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnShowUserDetailsPrevious.UniqueID + "').click();return false;}} else {return true}; ")
				txtLoginNameFuture.Attributes.Add("onkeypress", "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" + btnShowUserDetailsFuture.UniqueID + "').click();return false;}} else {return true}; ")
			Else
				ViewUserProfile(True)
			End If

        End Sub

        Private Sub PagePostLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
		End Sub

		Private Sub ShowErrorUserloginnamePrevious (loginName as string, errorInfo as string)
			If loginName <> Nothing Then
				lblErrMsgPrevious.Text = "Unknown login name """ & Server.HtmlEncode(loginName) & """! Please try to get the correct login via the normal"
			Else
				lblErrMsgPrevious.Text = "Error: " & Server.HtmlEncode(errorInfo) & "! Please try to get the correct login via the normal"
			End If
			ancUserListPrevious.HRef = "users.aspx"
			ancUserListPrevious.InnerHtml = "Users List"
			ErrorMsgPreviousVisible(True, True)
		End Sub
		Private Sub ShowSuccessMessage (message as string)
			MsgSuccessLabel.Text = "<h4 align=""center""><font color=""green"">" & Server.HtmlEncode(message) & " :-)</font></h4>"
			MsgEMailTemplate.Visible = True
		End Sub
		Private Sub ShowErrorUserloginnameFuture (loginName as string, errorInfo as string)
			If loginName <> Nothing Then
				lblErrMsgFuture.Text = "Unknown login name """ & Server.HtmlEncode(loginName) & """! Please try to get the correct login via the normal"
			Else
				lblErrMsgFuture.Text = "Error: " & Server.HtmlEncode(errorInfo) & "! Please try to get the correct login via the normal"
			End If
			ancUserListFuture.HRef = "users.aspx"
			ancUserListFuture.InnerHtml = "Users List"
			ErrorMsgFutureVisible(True, True)
		End Sub
#End Region

#Region "Control Events"
        Private Sub btnStartTransfer_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnStartTransfer.Click
			If UserInfoPrevious Is Nothing Then
				ShowErrorUserloginnamePrevious("", "Invalid user name")
				Return
			End If
			If UserInfoFuture Is Nothing Then
				ShowErrorUserloginnameFuture("", "Invalid user name")
				Return
			End If

			'Move additional flags
			Dim TransferFlags As New List (Of String)
			For Each AdditionalFlagKey as string in UserInfoPrevious.AdditionalFlags
				If UserInfoPrevious.AdditionalFlags(AdditionalFlagKey) <> Nothing Then
					TransferFlags.Add(AdditionalFlagKey)
				End If
			Next
			For Each AdditionalFlagKey as string in TransferFlags
				UserInfoFuture.AdditionalFlags(AdditionalFlagKey) = UserInfoPrevious.AdditionalFlags(AdditionalFlagKey)
			Next
			'Move group memberships, security administration priviledges as well as direct authorizations
			Dim Sql As String = "declare @NewUserID int, @PreviousUserID int, @AdminUserID int" & vbNewLine & _
				"select @NewUserID = 2, @PreviousUserID =90, @AdminUserID =9" & vbNewLine & _
				"" & vbNewLine & _
				"insert into memberships (id_group, id_user, releasedon, releasedby)" & vbNewLine & _
				"select id_group, @NewUserID, getdate(), @AdminUserID" & vbNewLine & _
				"from memberships" & vbNewLine & _
				"where id_user = @PreviousUserID " & vbNewLine & _
				"    and id_group not in" & vbNewLine & _
				"    (" & vbNewLine & _
				"        select id_group from memberships where id_user = @NewUserID" & vbNewLine & _
				"    )" & vbNewLine & _
				"" & vbNewLine & _
				"insert into [ApplicationsRightsByUser] (id_application, id_grouporperson, releasedon, releasedby, developmentteammember)" & vbNewLine & _
				"select id_application, @NewUserID, getdate(), @AdminUserID, developmentteammember" & vbNewLine & _
				"from [dbo].[ApplicationsRightsByUser]" & vbNewLine & _
				"where id_grouporperson = @PreviousUserID" & vbNewLine & _
				"    and isnull(developmentteammember, 0) = 0" & vbNewLine & _
				"    and id_application not in" & vbNewLine & _
				"    (" & vbNewLine & _
				"        select id_application " & vbNewLine & _
				"        from [dbo].[ApplicationsRightsByUser] " & vbNewLine & _
				"        where id_grouporperson = @NewUserID" & vbNewLine & _
				"            and isnull(developmentteammember, 0) = 0" & vbNewLine & _
				"    )" & vbNewLine & _
				"" & vbNewLine & _
				"insert into [ApplicationsRightsByUser] (id_application, id_grouporperson, releasedon, releasedby, developmentteammember)" & vbNewLine & _
				"select id_application, @NewUserID, getdate(), @AdminUserID, developmentteammember" & vbNewLine & _
				"from [dbo].[ApplicationsRightsByUser]" & vbNewLine & _
				"where id_grouporperson = @PreviousUserID" & vbNewLine & _
				"    and isnull(developmentteammember, 0) = 1" & vbNewLine & _
				"    and id_application not in" & vbNewLine & _
				"    (" & vbNewLine & _
				"        select id_application " & vbNewLine & _
				"        from [dbo].[ApplicationsRightsByUser] " & vbNewLine & _
				"        where id_grouporperson = @NewUserID" & vbNewLine & _
				"            and isnull(developmentteammember, 0) = 1" & vbNewLine & _
				"    )" & vbNewLine & _
				"" & vbNewLine & _
				"insert into [dbo].[System_SubSecurityAdjustments] (userid, tablename, tableprimaryidvalue, authorizationtype, releasedon, releasedby)" & vbNewLine & _
				"select @NewUserID as userid, Soll.tablename, Soll.tableprimaryidvalue, Soll.authorizationtype, getdate(), @AdminUserID" & vbNewLine & _
				"from" & vbNewLine & _
				"(" & vbNewLine & _
				"	select tablename, tableprimaryidvalue, authorizationtype" & vbNewLine & _
				"	from [dbo].[System_SubSecurityAdjustments]" & vbNewLine & _
				"	where userid = @PreviousUserID" & vbNewLine & _
				") as Soll" & vbNewLine & _
				"left join" & vbNewLine & _
				"(" & vbNewLine & _
				"	select id, tablename, tableprimaryidvalue, authorizationtype" & vbNewLine & _
				"	from [dbo].[System_SubSecurityAdjustments]" & vbNewLine & _
				"	where userid = @NewUserID" & vbNewLine & _
				") as Ist" & vbNewLine & _
				"on Soll.tablename = Ist.tablename AND Soll.tableprimaryidvalue = Ist.tableprimaryidvalue AND Soll.authorizationtype = Ist.authorizationtype" & vbNewLine & _
				"where Ist.ID is null"
			Dim MyCmd as new SqlCommand()
			MyCmd.Connection = New SqlConnection(cammWebManager.ConnectionString)
			MyCmd.CommandText = sql
			MyCmd.CommandType = CommandType.Text
			MyCmd.Parameters.Add ("@PreviousUserID", GetType(Long)).Value = UserInfoPrevious.IDLong
			MyCmd.Parameters.Add ("@NewUserID", GetType(Long)).Value = UserInfoFuture.IDLong
			DataQuery.ExecuteNonQuery(MyCmd, DataQuery.Automations.AutoOpenAndCloseAndDisposeConnection)
			'Finally create migration comment
			UserInfoFuture.AdditionalFlags("MigrationComment") = "User authorizations migrated by " & cammWebManager.CurrentUserInfo.FullName & " on " & Now.ToSTring("yyyy-MM-dd HH:mm:ss") & " from account """ & UserInfoPrevious.LoginName & """ (ID " & UserInfoPrevious.IDLong & ") to """ & UserInfoFuture.LoginName & """ (ID " & UserInfoFuture.IDLong & ")"
			UserInfoFuture.Save
			'UI update
			ViewUserProfile(True)
			cammWebManagerAdminUserInfoAdditionalFlagsPrevious.RefreshData
			cammWebManagerAdminUserInfoAdditionalFlagsFuture.RefreshData
			ShowSuccessMessage("Authorizations and priviledges transferred successfully - please inform the user about consequences, too:")
        End Sub

        Private Sub rptUserShow_ItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUserShowPrevious.ItemDataBound, rptUserShowFuture.ItemDataBound
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                If dt.Rows.Count > 0 Then
                    With dt.Rows(e.Item.ItemIndex)
                        For MyCount = 0 To dt.Columns.Count - 1
                            Select Case UCase(dt.Columns.Item(MyCount).ToString())
                                Case "ID"
                                    CType(e.Item.FindControl("lblID"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))
                                Case "LOGINNAME"
                                    CType(e.Item.FindControl("ancLoginName"), HtmlAnchor).HRef = "users_update.aspx?ID=" & CompuMaster.camm.WebManager.Utils.Nz(.Item("ID"), String.Empty)
                                    CType(e.Item.FindControl("ancLoginName"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINPW"
                                Case "CUSTOMERNO"
                                    CType(e.Item.FindControl("lblCustomerHadline"), Label).Text = cammWebManager.Internationalization.UpdateProfile_Descr_CustomerSupplierData
                                    CType(e.Item.FindControl("lblUpdateCustomerNO"), Label).Text = cammWebManager.Internationalization.UpdateProfile_Descr_CustomerNo
                                    CType(e.Item.FindControl("lblCustomerNO"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "SUPPLIERNO"
                                    CType(e.Item.FindControl("lblUpdateSupplierNO"), Label).Text = cammWebManager.Internationalization.UpdateProfile_Descr_SupplierNo
                                    CType(e.Item.FindControl("lblSupplierNO"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "COMPANY"
                                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ANREDE"
                                    CType(e.Item.FindControl("lblAnrede"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "TITEL"
                                    CType(e.Item.FindControl("lblTitel"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "VORNAME"
                                    CType(e.Item.FindControl("lblVorname"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "NACHNAME"
                                    CType(e.Item.FindControl("lblNachname"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "NAMENSZUSATZ"
                                    CType(e.Item.FindControl("lblNamenszusatz"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "E-MAIL"
                                    CType(e.Item.FindControl("ancEmail"), HtmlAnchor).HRef = "mailto:" & Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))
                                    CType(e.Item.FindControl("ancEmail"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))
                                Case "STRASSE"
                                    CType(e.Item.FindControl("lblStrasse"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "PLZ"
                                    CType(e.Item.FindControl("lblPLZ"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ORT"
                                    CType(e.Item.FindControl("lblORT"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "STATE"
                                    CType(e.Item.FindControl("lblState"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LAND"
                                    CType(e.Item.FindControl("lblLand"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINCOUNT"
                                    CType(e.Item.FindControl("lblLoginCount"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINFAILURES"
                                    CType(e.Item.FindControl("lblLoginFailures"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINLOCKEDTILL"
                                    CType(e.Item.FindControl("lblLoginLockedTill"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LOGINDISABLED"
                                    CType(e.Item.FindControl("lblLoginDisabled"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "ACCOUNTACCESSABILITY"
                                    Dim MyAccessLevel As New CompuMaster.camm.WebManager.WMSystem.AccessLevelInformation(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                                    CType(e.Item.FindControl("lblAccountAccessability"), Label).Text = Server.HtmlEncode(MyAccessLevel.Title)

                                Case "CREATEDON"
                                    CType(e.Item.FindControl("lblCreatedOn"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "MODIFIEDON"
                                    CType(e.Item.FindControl("lblModifiedOn"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LASTLOGINON"
                                    CType(e.Item.FindControl("lblLastLoginOn"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "LASTLOGINVIAREMOTEIP"
                                    CType(e.Item.FindControl("lblLastLoginViaremoteIP"), Label).Text = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), String.Empty))

                                Case "1STPREFERREDLANGUAGE"
                                    Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                    MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                                    CType(e.Item.FindControl("lblFirstPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)

                                Case "2NDPREFERREDLANGUAGE"
                                    If Not IsDBNull(.Item(MyCount)) Then
                                        Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                        MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                                        CType(e.Item.FindControl("lblSecondPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)
                                    End If

                                Case "3RDPREFERREDLANGUAGE"
                                    If Not IsDBNull(.Item(MyCount)) Then
                                        Dim MyLanguageInfo As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                                        MyLanguageInfo = New CompuMaster.camm.WebManager.WMSystem.LanguageInformation(CompuMaster.camm.WebManager.Utils.Nz(.Item(MyCount), 0), CType(cammWebManager, CompuMaster.camm.webmanager.WMSystem))
                                        CType(e.Item.FindControl("lblThirdPreferredLanguage"), Label).Text = Server.HtmlEncode(MyLanguageInfo.LanguageName_English)
                                    End If
                            End Select
                        Next
                    End With
                End If
            End If
        End Sub

#End Region

#Region "User-Defined Methods"
        Sub ViewUserProfile(ByVal ShowUserDetailsComplete As Boolean)
            If txtLoginNamePrevious.Text.Trim <> "" Then
                Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Nothing
				Dim LookupUserID As Long
                Try
                    LookupUserID = CLng(cammWebManager.System_GetUserID(txtLoginNamePrevious.Text))
                    MyUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(LookupUserID), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
					UserInfoPrevious = MyUserInfo
                    Dim gCtl As New HtmlGenericControl
                    gCtl.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CLng(LookupUserID), MyUserInfo.FullName)
                    tdAddDataTable.Controls.Add(gCtl)
                Catch
                    ShowErrorUserloginnamePrevious (txtLoginNamePrevious.Text, "")
                End Try

				If LookupUserID <> 0 Then
					cammWebManagerAdminUserInfoDetailsPrevious.MyUserInfo = MyUserInfo
					cammWebManagerAdminUserInfoAdditionalFlagsPrevious.cammWebManager = Me.cammWebManager
					cammWebManagerAdminUserInfoAdditionalFlagsPrevious.MyUserInfo = MyUserInfo

					If ShowUserDetailsComplete = True Then
						Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", LookupUserID)}
						dt = DataQuery.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SELECT * FROM [Benutzer] WHERE ID = @ID ORDER BY Nachname, Vorname", CommandType.Text, sqlParams, DataQuery.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
						rptUserShowPrevious.DataSource = dt
						rptUserShowPrevious.DataBind()
						VisibleControlPrevious(True, True)
						ErrorMsgPreviousVisible(False, False)
					End If
				End If
            ElseIf Me.IsPostBack = True Then
                ShowErrorUserloginnamePrevious ("", "missing login name")
			End If

            If txtLoginNameFuture.Text.Trim <> "" Then
                Dim MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Nothing
				Dim LookupUserID As Long
                Try
                    LookupUserID = CLng(cammWebManager.System_GetUserID(txtLoginNameFuture.Text.Trim))
                    MyUserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(LookupUserID), CType(cammWebManager, CompuMaster.camm.WebManager.WMSystem))
					UserInfoFuture = MyUserInfo
                    Dim gCtl As New HtmlGenericControl
                    gCtl.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CLng(LookupUserID), MyUserInfo.FullName)
                    tdAddDataTable.Controls.Add(gCtl)
                Catch
                    ShowErrorUserloginnameFuture (txtLoginNameFuture.Text, "")
                End Try

				If LookupUserID <> 0 Then
					cammWebManagerAdminUserInfoDetailsFuture.MyUserInfo = MyUserInfo
					cammWebManagerAdminUserInfoAdditionalFlagsFuture.cammWebManager = Me.cammWebManager
					cammWebManagerAdminUserInfoAdditionalFlagsFuture.MyUserInfo = MyUserInfo

					If ShowUserDetailsComplete = True Then
						Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", LookupUserID)}
						dt = DataQuery.FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "SELECT * FROM [Benutzer] WHERE ID = @ID ORDER BY Nachname, Vorname", CommandType.Text, sqlParams, DataQuery.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
						rptUserShowFuture.DataSource = dt
						rptUserShowFuture.DataBind()
						VisibleControlFuture(True, True)
						ErrorMsgFutureVisible(False, False)
					End If
				End If
            ElseIf Me.IsPostBack = True Then
                ShowErrorUserloginnameFuture ("", "missing login name")
            End If
	End Sub

        Sub VisibleControlPrevious(ByVal IsVisible As Boolean, ByVal IsVisibleTrue As Boolean)
            If IsVisibleTrue = False Then
                rptUserShowPrevious.Visible = False
            ElseIf IsVisible = True Then
                rptUserShowPrevious.Visible = True
            Else
                rptUserShowPrevious.Visible = False
            End If
        End Sub
        Sub VisibleControlFuture(ByVal IsVisible As Boolean, ByVal IsVisibleTrue As Boolean)
            If IsVisibleTrue = False Then
                rptUserShowFuture.Visible = False
            ElseIf IsVisible = True Then
                rptUserShowFuture.Visible = True
            Else
                rptUserShowFuture.Visible = False
            End If
        End Sub

        Sub ErrorMsgPreviousVisible(ByVal IsVisible As Boolean, ByVal IsVisibleTrue As Boolean)
            If IsVisibleTrue = False Then
                trErrMsgPrevious.Visible = False
            ElseIf IsVisible = True Then
                trErrMsgPrevious.Visible = True
                ancUserListPrevious.Visible = True
            Else
                trErrMsgPrevious.Visible = True
                ancUserListPrevious.Visible = False
            End If
        End Sub
        Sub ErrorMsgFutureVisible(ByVal IsVisible As Boolean, ByVal IsVisibleTrue As Boolean)
            If IsVisibleTrue = False Then
                trErrMsgFuture.Visible = False
            ElseIf IsVisible = True Then
                trErrMsgFuture.Visible = True
                ancUserListFuture.Visible = True
            Else
                trErrMsgFuture.Visible = True
                ancUserListFuture.Visible = False
            End If
        End Sub
#End Region

    End Class

	