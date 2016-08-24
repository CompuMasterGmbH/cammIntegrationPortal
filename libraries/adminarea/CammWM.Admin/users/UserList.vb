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
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to view a list of users
    ''' </summary>
    Public Class UserList
        Inherits UsersOverview

#Region "Variable Declaration"
        Protected lblErrMsg, lblNoRecMsg, lblID, lblCompany, lblLoginName, lblAccessLevelTitle, lblLand, lblState, lblLastLoginOn, lblLastLoginViaRemoteIP, lblCreatedOn, lblModifiedOn As Label
        Protected ancUserNameComplete, ancUpdate, ancDelete, ancClone As HtmlAnchor
        Protected gcDisabled As HtmlGenericControl
        Protected WithEvents rptUserList As Repeater
        Protected SearchUsersTextBox As TextBox
        Protected WithEvents CheckBoxTop50Results As CheckBox
        Dim MyDt As New DataTable
#End Region

#Region "Page Events"
        Private Sub UserList_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            lblErrMsg.Text = ""
            lblNoRecMsg.Text = ""
        End Sub

        Private Sub UserList_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            ListOfUsers()
            'SearchUsersTextBox.Attributes.Add("onblur", "return demoMatchClick('" & SearchUsersTextBox.ClientID & "');")
        End Sub
#End Region

#Region "User-Defined Methods"
        Protected Sub ListOfUsers()
            Dim WhereClause As String

            Dim SearchWords As String = SearchUsersTextBox.Text.Replace("'", "''").Replace("*", "%").Trim.Replace("  ", " ")
            SearchWords = "%" & SearchWords & "%"
            SearchWords = SearchWords.Replace(" ", "% %")
            Dim searchItems As String() = SearchWords.Split(" "c)
            Dim SingleNameItems As String = String.Empty
            Dim IsSingelName As Boolean = False


            'Search for the hole searchword in every column
            WhereClause = "Where (Loginname LIKE @SearchWords Or Namenszusatz LIKE @SearchWords Or company LIKE @SearchWords or Nachname LIKE @SearchWords Or Vorname LIKE @SearchWords Or [E-Mail] LIKE @SearchWords)"

            'Search for every single word (space-seperated) in searchword
            If searchItems.Length > 1 Then
                WhereClause &= " OR ("
                For myWordCounter As Integer = 0 To searchItems.Length - 1
                    If myWordCounter <> 0 Then WhereClause &= " AND "
                    WhereClause &= " (Loginname LIKE @SearchItem" & myWordCounter & " Or Namenszusatz LIKE @SearchItem" & myWordCounter & " Or company LIKE @SearchItem" & myWordCounter & " or Nachname LIKE @SearchItem" & myWordCounter & " Or Vorname LIKE @SearchItem" & myWordCounter & " Or [E-Mail] LIKE @SearchItem" & myWordCounter & ")"
                Next
                WhereClause &= " )"
            End If

            Dim TopClause As String = ""
            If CheckBoxTop50Results.Checked = True Then TopClause = "TOP 50" Else TopClause = ""

            Try
                If SearchWords = String.Empty Then WhereClause = String.Empty
                Dim SqlQuery As String = "SELECT " & TopClause & " System_AccessLevels.Title As AccessLevel_Title, Benutzer.*, ISNULL(Namenszusatz, '') + SPACE({ fn LENGTH(SUBSTRING(ISNULL(Namenszusatz, ''), 1, 1)) }) + Nachname + ', ' + Vorname AS UserNameComplete FROM [Benutzer] LEFT JOIN System_AccessLevels ON Benutzer.AccountAccessability = System_AccessLevels.ID " & WhereClause & " ORDER BY Nachname, Vorname"

                Dim cmd As New SqlClient.SqlCommand(SqlQuery, New SqlConnection(cammWebManager.ConnectionString))
                cmd.Parameters.Add("@SearchWords", SqlDbType.NVarChar).Value = SearchWords
                For myWordCounter2 As Integer = 0 To searchItems.Length - 1
                    cmd.Parameters.Add("@SearchItem" & myWordCounter2, SqlDbType.NVarChar).Value = searchItems(myWordCounter2)
                Next
                MyDt = FillDataTable(cmd, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                If Not MyDt Is Nothing AndAlso MyDt.Rows.Count > 0 Then
                    rptUserList.DataSource = MyDt
                    rptUserList.DataBind()
                Else
                    Dim drow As DataRow
                    drow = MyDt.NewRow
                    MyDt.Rows.Add(drow)
                    rptUserList.DataSource = MyDt
                    rptUserList.DataBind()
                    rptUserList.Items(0).Visible = False
                    lblNoRecMsg.Text = "No records found matching your search request."
                End If
            Catch ex As Exception
                Throw
            Finally
                MyDt.Dispose()
            End Try
        End Sub

#End Region

#Region "Control Events"
        Private Sub rptUserListItemBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptUserList.ItemDataBound
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                With MyDt.Rows(e.Item.ItemIndex)
                    CType(e.Item.FindControl("lblID"), Label).Text = Utils.Nz(.Item("ID"), 0).ToString
                    If Not IsDBNull(.Item("LoginDisabled")) Then If Utils.Nz(.Item("LoginDisabled"), False) = True Then CType(e.Item.FindControl("gcDisabled"), HtmlGenericControl).InnerHtml = "<nobr title=""Disabled"">(D)</nobr>"
                    CType(e.Item.FindControl("ancUserNameComplete"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancUserNameComplete"), HtmlAnchor).InnerHtml = Server.HtmlEncode(CompuMaster.camm.WebManager.Administration.Utils.FormatUserName(.Item("Vorname"), .Item("Nachname")))
                    CType(e.Item.FindControl("lblCompany"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Company"), String.Empty))
                    CType(e.Item.FindControl("lblLoginName"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("lblLoginName"), HtmlAnchor).InnerHtml = Server.HtmlEncode(Utils.Nz(.Item("LoginName"), String.Empty))
                    CType(e.Item.FindControl("lblAccessLevelTitle"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("AccessLevel_Title"), String.Empty))
                    CType(e.Item.FindControl("lblLand"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("Land"), String.Empty))
                    CType(e.Item.FindControl("lblState"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("State"), String.Empty))
                    CType(e.Item.FindControl("lblLastLoginOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("LastLoginOn"), String.Empty))
                    CType(e.Item.FindControl("lblLastLoginViaRemoteIP"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("LastLoginViaRemoteIP"), String.Empty))
                    CType(e.Item.FindControl("lblCreatedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("CreatedOn"), String.Empty))
                    CType(e.Item.FindControl("lblModifiedOn"), Label).Text = Server.HtmlEncode(Utils.Nz(.Item("ModifiedOn"), String.Empty))
                    CType(e.Item.FindControl("ancUpdate"), HtmlAnchor).HRef = "users_update.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancDelete"), HtmlAnchor).HRef = "users_delete.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                    CType(e.Item.FindControl("ancClone"), HtmlAnchor).HRef = "users_clone.aspx?ID=" & Utils.Nz(.Item("ID"), 0).ToString
                End With
            End If
        End Sub

#End Region

    End Class

End Namespace