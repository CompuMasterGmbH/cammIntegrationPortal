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
    '''     A page to list authorized users for an application with missing RequiredUserFlags
    ''' </summary>
    Public Class AppCheckUsersForMissingFlags
        Inherits Page

#Region "Variable Declaration"
        Protected AppID As Integer
        Protected UsersWithMissingFlagsGrid As DataGrid
        Protected ltrInfo As Literal
#End Region

        Private Sub AppCheckUsersForMissingFlags_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            AppID = CType(Request.QueryString("ID"), Integer)
            Dim resultDt As New DataTable("result")
            resultDt.Columns.Add("UserName", GetType(String))
            resultDt.Columns.Add("MissingFlags", GetType(String))

            'Get all users of the application
            Dim Users As DataTable = GetUsersByAppID(AppID)
            'Get all required flags of the application
            Dim AppFlags As String() = GetRequiredFlagsByAppID(AppID)

            'Step through all users
            For myUserCounter As Integer = 0 To Users.Rows.Count - 1
                'Contains the missing flags of the user in this application
                Dim LinksToMissingFlags As String = Nothing

                'Step through all required flags of the application
                For myFlagCounter As Integer = 0 To AppFlags.Length - 1
                    If IsFlagMissing(CLng(Users.Rows(myUserCounter)("ID_User")), AppFlags(myFlagCounter)) Then
                        'Add the missing flag as a link to the flag-update page to the list
                        Dim seperator As String = Nothing
                        If LinksToMissingFlags <> "" Then
                            seperator = ",&nbsp;"
                        End If
                        LinksToMissingFlags &= seperator & "<a target=""_blank"" href=""users_update_flag.aspx?ID=" & CLng(Utils.Nz(Users.Rows(myUserCounter)("ID_User"))) & "&Type=" & Server.UrlEncode(Trim(AppFlags(myFlagCounter))) & """>" & Server.HtmlEncode(Trim(AppFlags(myFlagCounter))) & "</a>"
                    End If
                Next
                If LinksToMissingFlags <> Nothing Then
                    'Add a new row with all information to the result table
                    Dim row As DataRow = resultDt.NewRow
                    'Provide username as a link to the user-update page
                    row("UserName") = "<a target=""_blank"" href=""users_update.aspx?ID=" & CLng(Utils.Nz(Users.Rows(myUserCounter)("ID_User"))) & """>" & Me.SafeLookupUserFullName(Utils.Nz(Users.Rows(myUserCounter)("ID_User"), 0L), True) & "</a>"
                    row("MissingFlags") = LinksToMissingFlags
                    resultDt.Rows.Add(row)
                End If
            Next

            If resultDt.Rows.Count > 0 Then
                'Bind the data to the datagrid
                UsersWithMissingFlagsGrid.DataSource = resultDt
                UsersWithMissingFlagsGrid.DataBind()
            Else
                ltrInfo.Text = "No users with missing flagvalues."
            End If

        End Sub

        Private Function IsFlagMissing(ByVal userID As Long, ByVal flagName As String) As Boolean
            If Me.cammWebManager.System_GetUserInfo(userID).AdditionalFlags(Trim(flagName)) Is Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function IsFlagFilled(ByVal userID As Long, ByVal flagName As String) As Boolean
            If IsFlagMissing(userID, flagName) = False Then
                If CStr(Me.cammWebManager.System_GetUserInfo(userID).AdditionalFlags(Trim(flagName))) <> Nothing Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        Public Function GetRequiredFlagsByAppID(ByVal ID As Integer) As String()
            Dim cmd3 As New SqlCommand("Select RequiredUserProfileFlags From Applications_CurrentAndInactiveOnes Where ID = @ID", New SqlConnection(cammWebManager.ConnectionString))
            cmd3.Parameters.Add("@ID", SqlDbType.Int).Value = ID
            Dim al As ArrayList = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd3, Automations.AutoOpenAndCloseAndDisposeConnection)
            If al.Item(0) Is DBNull.Value Then
                Return New String() {}
            Else
                Return CStr(al.Item(0)).Split(","c)
            End If
        End Function

        Private Function GetUsersByAppID(ByVal ID As Integer) As DataTable
            Dim cmd As System.Data.SqlClient.SqlCommand
            If Setup.DatabaseUtils.Version(Me.cammWebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                cmd = New System.Data.SqlClient.SqlCommand("Select * From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID union select ID_User from ApplicationsRightsByUser_RulesCumulativeWithInherition where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group  is not null and ID_Application = @ID)) as a Group by ID_User", New SqlConnection(cammWebManager.ConnectionString))
            Else
                cmd = New System.Data.SqlClient.SqlCommand("Select * From (select ID_User from view_ApplicationRights where ID_User is not null and ID_Application = @ID union select ID_User from Memberships where ID_Group in (Select ID_Group from view_ApplicationRights where ID_Group  is not null and ID_Application = @ID)) as a Group by ID_User", New SqlConnection(cammWebManager.ConnectionString))
            End If
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
            Return FillDataTable(cmd, Automations.AutoOpenAndCloseAndDisposeConnection, "usertable")
        End Function

    End Class

End Namespace