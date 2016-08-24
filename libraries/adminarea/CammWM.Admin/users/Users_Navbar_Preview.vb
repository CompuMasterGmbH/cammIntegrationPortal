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
    '''     The Users_Navbar_Preview page 
    ''' </summary>
    Public Class Users_Navbar_Preview
        Inherits Page

#Region "Variable Declaration"
        Dim gc, gc1 As New HtmlGenericControl
        Protected tdAnonymous, tdPublic As HtmlTableCell
        Protected lblHeading As Label
#End Region

#Region "Page Events"
        Private Sub Users_Navbar_Preview_PageLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Added if condition for showing Navigation Preview for all groups.
            If (Not Request.QueryString("GroupName") Is Nothing) Then 'AndAlso Not Request.QueryString("UserView") Is Nothing) Then
                lblHeading.Text = "Administration - Navigation preview of " + Request.QueryString("GroupName").ToString
                gc.InnerHtml += GetNavigationLinksApplication()  ' cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CType(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous, Int64), "Anonymous")
                tdAnonymous.Controls.Add(gc)
            Else
                lblHeading.Text = "Administration - Navigation preview of special users"
                gc.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CType(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous, Int64), "Anonymous")
                tdAnonymous.Controls.Add(gc)
                gc1.InnerHtml = cammWebManager.System_WriteNavPreviewNav_TR2TR_2Cols(CType(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Public, Int64), "Public")

                tdPublic.Controls.Add(gc1)
            End If
        End Sub
#End Region

#Region "User-Defined Functions"
        'This function will return navigation links for a specific user in the selected group.
        Private Function GetNavigationLinks(ByVal UserID As Long, ByVal Username As String) As String
            Dim serverGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
            Dim s As String = ("<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & Username & ":</b></FONT></P></TD></TR><TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">")
            Dim GroupId As String = Request.QueryString("GroupId").ToString
            Dim selectQuery As String = "select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where id_group = @GroupID Union select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where ID_Application in (select ID_Application from dbo.view_ApplicationRights where id_group =" + GroupId + "Group By ID_Application) and Id_User is not null and AppDisabled = 1"
            Dim cmd As New SqlCommand(selectQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupId
            Dim MyDt As DataTable = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If (UserID > 0) Then
                Dim webManager As WMSystem = cammWebManager
                Dim information As New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, (webManager), False)
                serverGroups = information.AccessLevel.ServerGroups
            ElseIf (UserID = -2) Then
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            Else
                If (UserID <> -1) Then
                    Throw New Exception("Invalid user information requested")
                End If
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            End If
            Dim informationArray As CompuMaster.camm.WebManager.WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)
            Dim information2 As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation
            For Each information2 In serverGroups
                If (IsAccessible(information2.MasterServer.ID, MyDt)) Then
                    Dim information3 As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                    For Each information3 In informationArray
                        If (IsAccessibleToLang(information3.ID, MyDt, UserID)) Then
                            s = String.Concat(New String() {s, "<a href=""#"" onClick=""OpenNavDemo(", information3.ID.ToString, ", '", HttpUtility.UrlEncode(information2.MasterServer.IPAddressOrHostHeader), "', '", UserID.ToString, "');"">", information2.Title, ", ", information3.LanguageName_English, "</a><br>"})
                        End If
                    Next
                End If
            Next
            s = (s & "</FONT></TD></TR><TR><TD>&nbsp;</TD></TR>")
            Return s
        End Function

        'This function will check accessibility of a servergroup from a particular location.
        Public Function DefaultNavPreviewLinks(ByVal UserID As Long, ByVal UserFullName As String, Optional ByVal WriteToCurrentContext As Boolean = False) As String

            'Temp--------------
            Dim GroupId As String = Request.QueryString("GroupId").ToString
            Dim selectQuery As String = "select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where id_group = @GroupID Union select ID_Application,Title,LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where ID_Application in (select ID_Application from dbo.view_ApplicationRights where id_group =" + GroupId + "Group By ID_Application) and Id_User is not null and AppDisabled = 1"
            Dim cmd As New SqlCommand(selectQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = CType(GroupId, Integer)
            Dim MyDt As DataTable = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            '------------------
            Dim serverGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()

            Dim s As String = ("<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & UserFullName & ":</b></FONT></P></TD></TR><TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">")
            serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            Dim informationArray As CompuMaster.camm.WebManager.WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)
            Dim information2 As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation
            For Each information2 In serverGroups
                If (IsAccessible(information2.MasterServer.ID, MyDt)) Then
                    Dim information3 As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                    For Each information3 In informationArray
                        If (IsAccessibleToLang(information3.ID, MyDt, UserID)) Then
                            s = String.Concat(New String() {s, "<a href=""#"" onClick=""OpenNavDemo(", information3.ID.ToString, ", '", HttpUtility.UrlEncode(information2.MasterServer.IPAddressOrHostHeader), "', '", UserID.ToString, "');"">", information2.Title, ", ", information3.LanguageName_English, "</a><br>"})
                        End If
                    Next
                End If
            Next
            s = (s & "</FONT></TD></TR><TR><TD>&nbsp;</TD></TR>")
            Return s
        End Function

        Private Function IsAccessible(ByVal ServerId As Integer, ByVal Mydt As DataTable) As Boolean
            For iCount As Integer = 0 To Mydt.Rows.Count - 1
                If (CInt(Mydt.Rows(iCount)("LocationId")) = ServerId) Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' <summary>
        ''' Check accessibility of a servergroup application for a particular language and check the disabled application
        ''' </summary>
        ''' <param name="LangId"></param>
        ''' <param name="Mydt"></param>
        ''' <param name="userid"></param>
        Private Function IsAccessibleToLang(ByVal LangId As Integer, ByVal Mydt As DataTable, ByVal userid As Long) As Boolean
            For iCount As Integer = 0 To Mydt.Rows.Count - 1
                If (CInt(Mydt.Rows(iCount)("LanguageId")) = LangId AndAlso Not Mydt.Rows(iCount)("AppDisabled") Is DBNull.Value AndAlso CInt(Mydt.Rows(iCount)("AppDisabled")) = 0) Then
                    Return True
                Else
                    If (Not Mydt.Rows(iCount)("AppDisabled") Is DBNull.Value AndAlso CInt(Mydt.Rows(iCount)("AppDisabled")) = 1 AndAlso CLng(Mydt.Rows(iCount)("ID_User")) = userid) Then
                        Return True
                    End If
                End If
            Next
            Return False
        End Function

        Private Function GetNavigationLinksApplication() As String
            Dim serverGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
            Dim s As String = Nothing '= ("<TR><TD BGCOLOR=""#C1C1C1"" ColSpan=""2""><P><FONT face=""Arial"" size=""2""><b>" & "" & ":</b></FONT></P></TD></TR>")
            s += "<TR><TD ColSpan=""2"" VAlign=""Top""><FONT face=""Arial"" size=""2"">"
            Dim GroupId As String = Request.QueryString("GroupId").ToString
            Dim selectQuery As String = "select LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where id_group = @GroupID Union select LocationId,LanguageId,ID_User,AppDisabled from dbo.view_ApplicationRights where ID_Application in (select ID_Application from dbo.view_ApplicationRights where id_group = @GroupID Group By ID_Application) and Id_User is not null and AppDisabled = 1"

            Dim cmd As New SqlCommand(selectQuery, New SqlConnection(cammWebManager.ConnectionString))
            cmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = CType(GroupId, Integer)
            Dim MyDt As DataTable = FillDataTable(cmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            Dim UserID As Long = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)

            If (UserID > 0) Then
                Dim webManager As WMSystem = cammWebManager
                Dim information As New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, (webManager), False)
                serverGroups = information.AccessLevel.ServerGroups
            ElseIf (UserID = -2) Then
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            Else
                If (UserID <> -1) Then
                    Throw New Exception("Invalid user information requested")
                End If
                serverGroups = cammWebManager.System_GetServerGroupsInfo(-1)
            End If

            'Added to apply new logic to show all server groups and markets
            Dim informationArray As CompuMaster.camm.WebManager.WMSystem.LanguageInformation() = cammWebManager.System_GetLanguagesInfo(False)
            Dim information2 As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation
            For Each information2 In serverGroups
                Dim information3 As CompuMaster.camm.WebManager.WMSystem.LanguageInformation
                For Each information3 In informationArray
                    s = String.Concat(New String() {s, "<a href=""#"" onClick=""OpenNavDemo(", information3.ID.ToString, ", '", HttpUtility.UrlEncode(information2.MasterServer.IPAddressOrHostHeader), "', '", UserID.ToString, "'," & Utils.Nz(Request.QueryString("GroupId"), 0) & ");"">", Utils.Nz(information2.Title, String.Empty), ", ", information3.LanguageName_English, "</a><br>"})
                Next
            Next

            s = (s & "</FONT></TD></TR><TR><TD>&nbsp;</TD></TR>")
            Return s
        End Function
#End Region

    End Class

End Namespace