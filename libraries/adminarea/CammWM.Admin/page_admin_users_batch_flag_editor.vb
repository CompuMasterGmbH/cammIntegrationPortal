'Copyright 2009-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration.BatchUserFlags

    Public Class Editor
        Inherits Page

#Region "Control Declaration"
        Protected WithEvents ddlCwmGroupnames, ddlItemsPerPage, ddlCwmApps As DropDownList
        Protected WithEvents btnDoFilter, btnSaveAllTop, btnSaveAllBottom, btnSearchAndReplace As Button
        Protected WithEvents tblUsers As Table
        Protected WithEvents rowGroup, rowAppID, rowAppSelect As TableRow
        Protected WithEvents txtSearch, txtReplace, txtFlagname As TextBox
        Protected WithEvents rblFilterFlagvalues As RadioButtonList
        Protected WithEvents lblApp As Label
        Protected WithEvents phPaginationLinks, phSearchAndReplace, phContent As PlaceHolder
        Protected WithEvents chkSearchAndReplace As CheckBox
#End Region

#Region "Page Events"
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            FillForm()
            If Me.Request.QueryString("AppID") <> Nothing Then
                Me.ddlCwmApps.SelectedValue = Me.Request.QueryString("AppID")
            End If
            If Me.Request.QueryString("Flag") <> Nothing Then
                Me.txtFlagname.Text = Me.Request.QueryString("Flag")
            End If
            If Me.Request.QueryString("EditMode") <> Nothing Then
                Me.rblFilterFlagvalues.SelectedIndex = CInt(Request.QueryString("EditMode"))
            End If
        End Sub

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            FillUsersTable(False)
        End Sub

        Private Sub PageOnPreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            PaginationLinks(True)
            FillUsersTable(True)
            FormView()
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnSaveAllTopClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAllTop.Click
            SaveAllFlags()
        End Sub

        Private Sub btnSaveAllBottomClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAllBottom.Click
            SaveAllFlags()
        End Sub

        Private Sub btnDoFilter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDoFilter.Click
            PaginationLinks(True)
            FillUsersTable(True)
        End Sub

        Private Sub btnSearchAndReplace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearchAndReplace.Click
            If Me.txtSearch.Text <> Nothing Then
                SearchAndReplaceInForm(Me.txtSearch.Text, Me.txtReplace.Text)
            Else
                AddUiMessage("You must insert a search text.", MessageType.Warning)
            End If
        End Sub

        Sub LinkButtonClick(ByVal sender As Object, ByVal e As EventArgs)
            Me.PageIndex = CInt(CType(sender, LinkButton).CommandArgument)
            AddUiMessage("LinkButtonClick")
            PaginationLinks(True)
            FillUsersTable(True)
        End Sub

        Private Sub chkSearchAndReplace_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSearchAndReplace.CheckedChanged
            Me.phSearchAndReplace.Visible = Me.chkSearchAndReplace.Checked
            If Me.chkSearchAndReplace.Checked = True Then
                Me.chkSearchAndReplace.Text = Nothing
            Else
                Me.chkSearchAndReplace.Text = "Search and Replace"
            End If
        End Sub
#End Region

#Region "User Inteface Generation"
        Sub PaginationLinks()
            PaginationLinks(False)
        End Sub

        Sub PaginationLinks(ByVal refresh As Boolean)
            If refresh Then
                phPaginationLinks.Controls.Clear()
            End If
            If Me.ddlItemsPerPage.SelectedValue <> Nothing AndAlso CInt(Me.ddlItemsPerPage.SelectedValue) <> Nothing Then
                Dim ItemsPerPage As Integer = CInt(Me.ddlItemsPerPage.SelectedValue)
                If Not UsersToEdit Is Nothing Then
                    Dim UserCount As Integer = UsersToEdit.Length
                    Dim NoOfParts As Integer
                    'get the integer no of parts
                    NoOfParts = CInt(UserCount / ItemsPerPage)

                    'ArticlesCount divided by the chunksize has a remainder then increase the number of parts (because we've got an additional, non complete part)
                    If UserCount Mod ItemsPerPage > 0 Then
                        NoOfParts += 1
                    End If

                    If NoOfParts > 1 Then
                        For i As Integer = 1 To NoOfParts
                            Dim link As New LinkButton
                            link.CommandArgument = CStr(i)
                            link.Text = CStr(i)
                            If i = Me.PageIndex Then
                                link.Style.Add("background-color", "#000099")
                                link.Style.Add("color", "#FFFFFF")
                            Else
                                link.Style.Add("background-color", "#FFFFFF")
                                link.Style.Add("color", "#000099")
                            End If
                            link.Style.Add("font-weight", "bold")
                            link.Style.Add("padding-right", "3px")
                            AddHandler link.Click, AddressOf LinkButtonClick
                            phPaginationLinks.Controls.Add(link)
                        Next
                    End If
                End If
            End If
        End Sub

        Sub FillForm()
            FillGroupList()
            FillAppList()
        End Sub

        Private Sub FillAppList()
            Me.ddlCwmApps.Items.AddRange(Apps(False))
        End Sub

        Private Sub FillGroupList()
            Me.ddlCwmGroupnames.Items.Add(New ListItem(Nothing, Nothing))
            For Each gi As CompuMaster.camm.WebManager.WMSystem.GroupInformation In Me.cammWebManager.System_GetGroupInfos()
                If gi.IsSystemGroupByServerGroup = False Then
                    Dim LI As New ListItem
                    LI.Value = gi.Name
                    LI.Text = gi.Name & " (" & gi.MembersByRule(True).AllowRule.Length & ")"
                    If gi.Description <> Nothing Then LI.Attributes.Add("title", gi.Description)
                    Me.ddlCwmGroupnames.Items.Add(LI)
                End If
            Next
        End Sub

#Region "Style"
        Private Function GetTableCell() As TableCell
            Return GetTableCell(Nothing)
        End Function

        Private Function GetTableCell(ByVal text As String) As TableCell
            Dim Result As New TableCell
            Result.Style.Add("padding", "3px")
            Result.Style.Add("border", "1px solid #cecece")
            If text <> Nothing Then Result.Text = text
            Return Result
        End Function
#End Region
#End Region

#Region "Helpers"
        Public Function SplitUserInformationCollection(ByVal users As CompuMaster.camm.WebManager.WMSystem.UserInformation(), ByVal pageIndex As Integer, ByVal itemsPerPage As Integer) As CompuMaster.camm.WebManager.WMSystem.UserInformation()
            Dim Result As New ArrayList

            If Not (users Is Nothing) Then
                If itemsPerPage = 0 Then
                    Result.AddRange(users)
                Else
                    Dim userCount As Integer = users.Length - 1

                    If userCount <= itemsPerPage Then
                        'if Articles count is smaller then desired chunksize then return all Articles we've got
                        Result.AddRange(users)
                    ElseIf userCount > itemsPerPage Then

                        Dim NoOfParts As Integer
                        'get the integer no of parts
                        NoOfParts = CInt(userCount / itemsPerPage)

                        'ArticlesCount divided by the chunksize has a remainder then increase the number of parts (because we've got an additional, non complete part)
                        If userCount Mod itemsPerPage > 0 Then
                            NoOfParts += 1
                        End If

                        'if the requested chunknumber is bigger then the number of parts then set requestedChunkNumber to the highest number ofparts
                        If pageIndex > NoOfParts Then
                            pageIndex = NoOfParts
                        End If

                        Dim StartIndex As Integer
                        Dim EndIndex As Integer

                        StartIndex = itemsPerPage * (pageIndex - 1)
                        EndIndex = (StartIndex + itemsPerPage) - 1

                        If EndIndex > (userCount - 1) Then
                            EndIndex = userCount - 1
                        End If

                        If StartIndex >= EndIndex Then
                            StartIndex = userCount - (userCount Mod itemsPerPage)
                        End If

                        For i As Integer = StartIndex To EndIndex
                            If (i > 0) And i < users.Length - 1 Then
                                Result.Add(users(i))
                            End If
                        Next
                    End If
                End If
            End If

            Return CType(Result.ToArray(GetType(CompuMaster.camm.WebManager.WMSystem.UserInformation)), CompuMaster.camm.WebManager.WMSystem.UserInformation())
        End Function

        Private Sub ResetForm()
            phContent.Controls.Clear()
        End Sub

        Private Sub FormView()
            If UsersToEdit.Length <= 20 Then
                Me.btnSaveAllTop.Visible = False
            Else
                Me.btnSaveAllTop.Visible = True
            End If
        End Sub

        Private Function GetUsersBySecurityObjectID(ByVal securityObjectID As Integer) As CompuMaster.camm.WebManager.WMSystem.UserInformation()
            Dim Sql As String
            If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                Sql = "SELECT ID_User " & vbNewLine & _
                    "FROM (" & vbNewLine & _
                    "    SELECT [ID_User] " & vbNewLine & _
                    "    FROM [dbo].[view_Memberships_Effective_with_PublicNAnonymous] " & vbNewLine & _
                    "    WHERE [ID_Group] IN " & vbNewLine & _
                    "        ( " & vbNewLine & _
                    "        SELECT ID_Group " & vbNewLine & _
                    "        FROM [dbo].[view_ApplicationRights] " & vbNewLine & _
                    "        WHERE ID_Group Is NOT Null " & vbNewLine & _
                    "            AND ID_Application = @AppID" & vbNewLine & _
                    "        )" & vbNewLine & _
                    "    UNION ALL" & vbNewLine & _
                    "    SELECT ID_User " & vbNewLine & _
                    "    FROM [dbo].[view_ApplicationRights] " & vbNewLine & _
                    "    WHERE ID_User IS NOT NULL " & vbNewLine & _
                    "        AND ID_Application = @AppID" & vbNewLine & _
                    "    ) AS UserList " & vbNewLine & _
                    "GROUP BY ID_User " & vbNewLine & _
                    "ORDER BY ID_User "
            Else
                Sql = "SELECT ID_User " & vbNewLine & _
                    "FROM (" & vbNewLine & _
                    "    SELECT [ID_User] " & vbNewLine & _
                    "    FROM [dbo].[view_Memberships_CummulatedWithAnonymous] " & vbNewLine & _
                    "    WHERE [ID_Group] IN " & vbNewLine & _
                    "        ( " & vbNewLine & _
                    "        SELECT ID_Group " & vbNewLine & _
                    "        FROM [dbo].[view_ApplicationRights] " & vbNewLine & _
                    "        WHERE ID_Group Is NOT Null " & vbNewLine & _
                    "            AND ID_Application = @AppID" & vbNewLine & _
                    "        )" & vbNewLine & _
                    "    UNION ALL" & vbNewLine & _
                    "    SELECT ID_User " & vbNewLine & _
                    "    FROM [dbo].[view_ApplicationRights] " & vbNewLine & _
                    "    WHERE ID_User IS NOT NULL " & vbNewLine & _
                    "        AND ID_Application = @AppID" & vbNewLine & _
                    "    ) AS UserList " & vbNewLine & _
                    "GROUP BY ID_User " & vbNewLine & _
                    "ORDER BY ID_User "
            End If

            Dim Result As New ArrayList

            Dim SqlCmd As New SqlCommand
            SqlCmd.Connection = New SqlConnection(Me.cammWebManager.ConnectionString)
            SqlCmd.CommandText = Sql
            SqlCmd.Parameters.Add("@AppID", SqlDbType.Int).Value = securityObjectID
            Dim ResultDT As DataTable = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(SqlCmd, Automations.AutoOpenAndCloseAndDisposeConnection, "UserIDs")

            For Each row As DataRow In ResultDT.Rows
                If Not row(0) Is DBNull.Value Then
                    Result.Add(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(row(0)), Me.cammWebManager))
                End If
            Next

            Return CType(Result.ToArray(GetType(WMSystem.UserInformation)), WMSystem.UserInformation())
        End Function

        Private Function SelectedEditMode() As EditMode
            Dim Result As EditMode
            Select Case CInt(Me.rblFilterFlagvalues.SelectedValue)
                Case 1
                    Result = EditMode.All
                Case 2
                    Result = EditMode.NonEmptyFlags
                Case 3
                    Result = EditMode.EmptyFlags
                Case 4
                    Result = EditMode.EmptyOrInvalidFlags
                Case Else
                    Throw New Exception("Unknown edit mode.")
            End Select
            Return Result
        End Function

        Private Enum EditMode As Short
            All = 1
            NonEmptyFlags = 2
            EmptyFlags = 3
            EmptyOrInvalidFlags = 4
        End Enum

        Public Overridable Function IsProtectedFlag(ByVal flagName As String) As Boolean
            Return False
        End Function

        Private Sub SaveAllFlags()
            'Check whether Me.txtFlagname.Text is a protected flag (e.g. SAP-flag)
            If IsProtectedFlag(Me.txtFlagname.Text) Then
                Me.AddUiMessage(Me.txtFlagname.Text & " is a protected flag!", MessageType.Warning, True)
            Else
                Try
                    For Each key As String In GetFlagValuesFromForm()
                        SetAdditionalFlag(CLng(key), GetFlagValuesFromForm().Item(key))
                    Next
                    AddUiMessage("Save process was successful.", MessageType.Succes)
                Catch ex As Exception
                    AddUiMessage("Cannot save at least one of the userflags. '" & ex.Message & "'.", MessageType.Failure)
                End Try
                FillUsersTable(True)
            End If
        End Sub

        Public Property PageIndex() As Integer
            Get
                If ViewState("PageIndex") Is Nothing Then
                    Return 1
                Else
                    Return CInt(ViewState("PageIndex"))
                End If
            End Get
            Set(ByVal Value As Integer)
                ViewState("PageIndex") = Value
            End Set
        End Property

        Private Function Apps() As ListItem()
            Return Apps(False)
        End Function

        Private Function Apps(ByVal includeSysApps As Boolean) As ListItem()
            Dim Result As New ArrayList

            Result.Add(New ListItem("", ""))
            Dim SqlCmd As New SqlCommand
            SqlCmd.Connection = New SqlConnection(cammWebManager.ConnectionString)
            SqlCmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                "SELECT ID" & _
                                ",[Title] " & _
                                ",[TitleAdminArea] " & _
                                ",[LocationID] " & _
                                ",(SELECT [ServerDescription] FROM System_Servers B WHERE A.LocationID = B.ID) ServerName " & _
                                ",(SELECT Description_English FROM System_Languages C WHERE A.[LanguageID] = C.ID) [Language] " & _
                                ",[SystemApp] " & _
                                ",[AppDisabled] " & _
                                ",[AuthsAsAppID] " & _
                                ",[AppDeleted] " & _
                                ",[RequiredUserProfileFlags] " & _
                                "FROM [Applications_CurrentAndInactiveOnes] A ORDER BY TITLE, LANGUAGE"

            Dim DT As DataTable = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(SqlCmd, Automations.AutoOpenAndCloseAndDisposeConnection, "Apps")

            For Each row As DataRow In DT.Rows
                If includeSysApps = True OrElse (includeSysApps = False AndAlso CBool(CompuMaster.camm.WebManager.Utils.Nz(row("SystemApp"))) = False) Then
                    Dim LI As New ListItem
                    LI.Text = CStr(CompuMaster.camm.WebManager.Utils.Nz(row("Title"))) & " [" & CStr(CompuMaster.camm.WebManager.Utils.Nz(row("ID"))) & " - " & CStr(CompuMaster.camm.WebManager.Utils.Nz(row("ServerName"))) & " - " & CStr(CompuMaster.camm.WebManager.Utils.Nz(row("Language"))) & "]"
                    LI.Attributes.Add("title", CStr(CompuMaster.camm.WebManager.Utils.Nz(row("ID"))) & " - " & CStr(CompuMaster.camm.WebManager.Utils.Nz(row("TitleAdminArea"))))
                    LI.Value = CStr(CompuMaster.camm.WebManager.Utils.Nz(row("ID")))
                    Result.Add(LI)
                End If
            Next

            Return CType(Result.ToArray(GetType(ListItem)), ListItem())
        End Function
#End Region

#Region "UI Messages"
        Private Sub AddUiMessage(ByVal text As String)
            AddUiMessage(Server.HtmlEncode(text), MessageType.Normal, False)
        End Sub

        Private Sub AddUiMessage(ByVal text As String, ByVal messageType As MessageType)
            AddUiMessage(Server.HtmlEncode(text), messageType, False)
        End Sub

        Private Sub AddUiMessage(ByVal text As String, ByVal messageType As MessageType, ByVal htmlEncode As Boolean)
            If text <> Nothing Then
                Dim lbl As New Label
                If htmlEncode = True Then
                    lbl.Text = Server.HtmlEncode(text)
                Else
                    lbl.Text = text
                End If
                lbl.Text &= "<br>"
                Select Case messageType
                    Case messageType.Failure
                        lbl.Style.Add("color", "#ff0000")
                    Case messageType.Warning
                        lbl.Style.Add("color", "#FFD401")
                    Case messageType.Succes
                        lbl.Style.Add("color", "#335F12")
                    Case messageType.Normal
                        lbl.Style.Add("color", "#000000")
                End Select
                lbl.Style.Add("font-weight", "bold")
                phContent.Controls.Add(lbl)
            End If
        End Sub

        Enum MessageType
            Failure
            Warning
            Succes
            Normal
        End Enum
#End Region

#Region "Userlist"
        Private Sub FillUsersTable()
            FillUsersTable(False)
        End Sub

        Private Sub ClearUsersTable()
            tblUsers.Rows.Clear()
        End Sub

        Private Property UsersToEdit() As CompuMaster.camm.WebManager.WMSystem.UserInformation()
            Get
                If Me.Session("CWM.Admin.BatchUserFlagEditor") Is Nothing Then
                    If Me.ddlCwmApps.SelectedValue <> Nothing Then
                        Me.Session("CWM.Admin.BatchUserFlagEditor") = Me.GetUsersBySecurityObjectID(CInt(Me.ddlCwmApps.SelectedValue))
                    ElseIf Me.ddlCwmGroupnames.SelectedValue <> Nothing Then
                        Me.Session("CWM.Admin.BatchUserFlagEditor") = Me.cammWebManager.System_GetGroupInfo(Me.ddlCwmGroupnames.SelectedValue).MembersByRule(True).AllowRule
                    Else
                        Me.Session("CWM.Admin.BatchUserFlagEditor") = Me.cammWebManager.System_GetUserInfos(Me.cammWebManager.System_GetUserIDs())
                    End If
                End If
                Return CType(Me.Session("CWM.Admin.BatchUserFlagEditor"), CompuMaster.camm.WebManager.WMSystem.UserInformation())
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.UserInformation())
                Me.Session("CWM.Admin.BatchUserFlagEditor") = Value
            End Set
        End Property

        Private Sub FillUsersTable(ByVal refresh As Boolean)
            If refresh = True Then
                UsersToEdit = Nothing
                tblUsers.Rows.Clear()
            End If
            If UsersToEdit.Length = 0 Then
                AddUiMessage("No users to edit.", MessageType.Failure)
            Else
                AddDataToUsersTable(SplitUserInformationCollection(UsersToEdit(), PageIndex(), CInt(Me.ddlItemsPerPage.SelectedValue)))
            End If
            If Not tblUsers Is Nothing AndAlso tblUsers.Rows.Count > 0 Then
                Me.btnSaveAllTop.Visible = True
                Me.btnSaveAllBottom.Visible = True
            Else
                Me.btnSaveAllTop.Visible = False
                Me.btnSaveAllBottom.Visible = False
            End If
        End Sub

        Private Sub AddDataToUsersTable(ByVal userInfos As CompuMaster.camm.WebManager.WMSystem.UserInformation())
            tblUsers.ID = "UsersTable"
            tblUsers.CellPadding = 0
            tblUsers.CellSpacing = 0

            Dim Row As TableRow

            Row = New TableRow
            Row.Cells.Add(GetTableCell("<b>UserID</b>"))
            Row.Cells.Add(GetTableCell("<b>Fullname</b>"))
            Row.Cells.Add(GetTableCell("<b>Flagvalue</b> (" & Me.txtFlagname.Text & ")"))

            Row.Style.Add("background-color", "#c1c1c1")
            tblUsers.Rows.Add(Row)
            Dim RowCount As Integer = 0
            Dim sortDt As New DataTable("tmpSort")
            sortDt.Columns.Add("UserInfo", GetType(CompuMaster.camm.WebManager.WMSystem.UserInformation))
            sortDt.Columns.Add("LastName")

            For Each ui As CompuMaster.camm.WebManager.WMSystem.UserInformation In userInfos
                If AddThisRow(ui) Then
                    Dim nRow As DataRow = sortDt.NewRow
                    nRow("UserInfo") = ui
                    nRow("LastName") = ui.LastName
                    sortDt.Rows.Add(nRow)
                End If
            Next

            'Sort Table
            sortDt.DefaultView.Sort = "LastName"
            For Each sRow As DataRow In sortDt.Rows
                tblUsers.Rows.Add(UserRow(CType(sRow("UserInfo"), CompuMaster.camm.WebManager.WMSystem.UserInformation)))
                If RowCount Mod 2 = 0 Then
                    tblUsers.Rows(tblUsers.Rows.Count - 1).Style.Add("background-color", "#ffffff")
                Else
                    tblUsers.Rows(tblUsers.Rows.Count - 1).Style.Add("background-color", "#e1e1e1")
                End If
                RowCount += 1
            Next
            sortDt.Dispose()
        End Sub

        Private Function AddThisRow(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As Boolean
            Dim Result As Boolean = False
            Dim editMode As EditMode = SelectedEditMode()

            If editMode = editMode.All Then
                Result = True
            Else
                If editMode = editMode.EmptyFlags Then
                    Result = Not IsFlagFilled(userInfo)
                ElseIf editMode = editMode.NonEmptyFlags Then
                    Result = IsFlagFilled(userInfo)
                ElseIf editMode = editMode.EmptyOrInvalidFlags Then
                    Result = Not IsFlagValid(userInfo)
                End If
            End If
            Return Result
        End Function

        Private Function UserRow(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As TableRow
            Dim Result As New TableRow
            Dim Cell As TableCell

            'userid
            Result.Cells.Add(GetTableCell(userInfo.ID.ToString))

            'user fullname
            Cell = GetTableCell()
            Dim hpl As New HyperLink
            hpl.Target = "_blank"
            hpl.NavigateUrl = "users_update.aspx?ID=" & userInfo.ID.ToString
            hpl.Text = Server.HtmlEncode(userInfo.LoginName & " (" & userInfo.FirstName & " " & userInfo.LastName & ")")
            hpl.ToolTip = Server.HtmlEncode(userInfo.Company & ", " & userInfo.FullName)
            Cell.Controls.Add(hpl)
            Result.Cells.Add(Cell)

            'user flagvalue
            Result.Cells.Add(UserEditCellReadWrite(userInfo))

            Return Result
        End Function

        Private Function UserEditCellReadOnly(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As TableCell
            Return GetTableCell(GetFlagValue(userInfo))
        End Function

        Private Function UserEditCellReadWrite(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As TableCell
            Dim Result As TableCell = GetTableCell()

            Dim txtFlagValue As New TextBox
            txtFlagValue.ID = "txtFlagValue#" & userInfo.ID
            txtFlagValue.Text = GetFlagValue(userInfo)
            txtFlagValue.Width = Unit.Pixel(350)
            txtFlagValue.Attributes.Add("onfocus", "this.select()")
            Result.Controls.Add(txtFlagValue)

            Return Result
        End Function
#End Region

#Region "FlagData Manipulation"
        Private Function GetFlagValue(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As String
            If userInfo.AdditionalFlags.Item(Me.txtFlagname.Text) Is Nothing OrElse userInfo.AdditionalFlags.Item(Me.txtFlagname.Text) = Nothing Then
                Return Nothing
            Else
                Return userInfo.AdditionalFlags.Item(Me.txtFlagname.Text)
            End If
        End Function

        Private Function IsFlagFilled(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As Boolean
            If userInfo.AdditionalFlags.Item(Me.txtFlagname.Text) Is Nothing OrElse userInfo.AdditionalFlags.Item(Me.txtFlagname.Text) = Nothing Then
                Return False
            Else
                Return True
            End If
        End Function

        Private Function IsFlagValid(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As Boolean
            Dim flag As String = Me.txtFlagname.Text
            Dim flagvalidation As New CompuMaster.camm.WebManager.FlagValidation(flag)
            Return flagvalidation.Validate(userInfo).ValidationResult = CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.Success
        End Function


        Private Function GetFlagValuesFromForm() As Collections.Specialized.NameValueCollection
            Dim Result As New Collections.Specialized.NameValueCollection
            If tblUsers Is Nothing Then Throw New NullReferenceException("The UsersTable is not present.")
            If tblUsers.Rows.Count = 0 Then Throw New System.Exception("The UsersTable contains no data.")

            'Jump over first row, contains no data
            For i As Integer = 1 To tblUsers.Rows.Count - 1
                Dim row As TableRow = tblUsers.Rows(i)
                Dim FlagValue As String = GetFlagValueFromCell(row.Cells(2))
                Result.Add(row.Cells(0).Text, FlagValue)
            Next

            Return Result
        End Function

        Private Function SearchAndReplaceInForm(ByVal search As String, ByVal replace As String) As Collections.Specialized.NameValueCollection
            Dim Result As New Collections.Specialized.NameValueCollection
            If tblUsers Is Nothing Then Throw New NullReferenceException("The UsersTable is not present.")
            If tblUsers.Rows.Count = 0 Then Throw New System.Exception("The UsersTable contains no data.")

            For Each row As TableRow In tblUsers.Rows
                Dim FlagValue As String = GetFlagValueFromCell(row.Cells(2))
                If FlagValue <> Nothing Then
                    SetFlagValueInCell(row.Cells(2), FlagValue.Replace(search, replace))
                End If
            Next

            Return Result
        End Function

        Private Sub SetFlagValueInCell(ByVal cell As TableCell, ByVal value As String)
            For Each ctrl As UI.Control In cell.Controls
                If ctrl.GetType Is GetType(TextBox) Then
                    If CType(ctrl, TextBox).ID.StartsWith("txtFlagValue") Then
                        CType(ctrl, TextBox).Text = value
                        Exit For
                    End If
                End If
            Next
        End Sub

        Private Function GetFlagValueFromCell(ByVal cell As TableCell) As String
            Dim txtBox As TextBox = Nothing
            For Each ctrl As UI.Control In cell.Controls
                If ctrl.GetType Is GetType(TextBox) Then
                    If CType(ctrl, TextBox).ID.StartsWith("txtFlagValue") Then
                        txtBox = CType(ctrl, TextBox)
                        Exit For
                    End If
                End If
            Next

            If txtBox Is Nothing Then
                Return Nothing
            Else
                Return txtBox.Text
            End If
        End Function

        Private Sub SetAdditionalFlag(ByVal userID As Long, ByVal value As String)
            Dim UI As New CompuMaster.camm.WebManager.WMSystem.UserInformation(userID, Me.cammWebManager)
            UI.AdditionalFlags.Set(Me.txtFlagname.Text, value)
            UI.Save()
        End Sub
#End Region
    End Class

End Namespace