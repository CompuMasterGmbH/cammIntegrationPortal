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
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    Public Class ApplicationBasePage
        Inherits Page

        Friend Sub New()
        End Sub

        ''' <summary>
        ''' The authorizations overview block for an application as HTML
        ''' </summary>
        ''' <param name="securityObjectID"></param>
        ''' <returns></returns>
        Public Function RenderAuthorizations(securityObjectID As Integer) As String
            Dim strblr As New System.Text.StringBuilder
            strblr.Append("<table cellSpacing=""0"" cellPadding=""0"" border=""0"">")
            Dim sql As String
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                sql = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine &
                              "SELECT ItemType, ID_Group, Name, ID_User, LoginDisabled, LoginName, DevelopmentTeamMember, IsDenyRule FROM view_ApplicationRights WHERE ID_Application = 6185 AND ID_AppRight Is NOT Null ORDER BY ItemType, Name, DevelopmentTeamMember, IsDenyRule"
            Else
                sql = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                              "SELECT ItemType, ID_Group, Name, ID_User, LoginDisabled, LoginName, DevelopmentTeamMember, 0 AS IsDenyRule FROM view_ApplicationRights WHERE ID_Application = @ID AND ID_AppRight Is NOT Null ORDER BY ItemType, Name, DevelopmentTeamMember"
            End If
            Dim cmd2 As New SqlClient.SqlCommand(sql, New SqlConnection(cammWebManager.ConnectionString))
            cmd2.Parameters.Add("@ID", SqlDbType.Int).Value = CType(Trim(Request.QueryString("ID")), Integer)
            Dim dtUpdate As DataTable = FillDataTable(cmd2, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If Not dtUpdate Is Nothing Then
                For Each dr As DataRow In dtUpdate.Rows
                    strblr.Append("<TR><TD VAlign=""Top"" WIDTH=""160""><P><FONT face=""Arial"" size=""2"">")
                    If Utils.Nz(dr("IsDenyRule"), False) Then
                        strblr.Append("DENY")
                    Else
                        strblr.Append("ALLOW")
                    End If
                    If CInt(dr("ItemType")) = 1 Then
                        'Groups
                        strblr.Append(" Group ")
                        If Utils.Nz(dr("DevelopmentTeamMember"), False) Then
                            strblr.Append("<strong title=""Authorization For test And development purposes And For inactive security objects""> {Dev} </strong>")
                        End If
                        strblr.Append("</FONT></P></TD><TD VAlign=""Top"" Width=""240""><P><FONT face=""Arial"" size=""2"">")
                        strblr.Append("<a href=""groups_update.aspx?ID=" & Utils.Nz(dr("ID_Group"), 0) & """>")
                        Dim GroupName As String = Utils.Nz(dr("Name"), String.Empty)
                        If Trim(GroupName) = "" Then GroupName = "{Group ID " & Utils.Nz(dr("ID_Group"), 0) & "}"
                        strblr.Append(Server.HtmlEncode(GroupName))
                        strblr.Append("</a>")
                    Else
                        'Users
                        strblr.Append(" User ")
                        If Utils.Nz(dr("DevelopmentTeamMember"), False) Then
                            strblr.Append("<strong title=""Authorization For test And development purposes And For inactive security objects""> {Dev} </strong>")
                        End If
                        strblr.Append("</FONT></P></TD><TD VAlign=""Top"" Width=""240""><P><FONT face=""Arial"" size=""2"">")
                        strblr.Append("<a href=""users_update.aspx?ID=" & Utils.Nz(dr("ID_User"), 0).ToString & """>")
                        strblr.Append(Utils.Nz(Server.HtmlEncode(Me.SafeLookupUserFullName(Utils.Nz(dr("ID_User"), -1L))), String.Empty) & " (" & Server.HtmlEncode(Utils.Nz(dr("LoginName"), String.Empty)) & ")")
                        strblr.Append("</a>")
                        strblr.Append(Utils.Nz(IIf(Utils.Nz(dr("LoginDisabled"), False) <> False, "&nbsp;<em><font color= ""#D1D1D1"">(Disabled)</font></em>", ""), String.Empty))
                    End If
                    strblr.Append("</FONT></P></TD></TR>")
                Next
            End If
            strblr.Append("</table>")
            Return strblr.ToString
        End Function

    End Class

End Namespace