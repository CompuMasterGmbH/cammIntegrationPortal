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
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to delete AppRights Transmission
    ''' </summary>
    Public Class AppRightsDeleteTransmission
        Inherits Page

#Region "Variable Declaration"
        Protected gcAddHtml As HtmlGenericControl
        Protected lblErrMsg As Label
        Protected hypNoDelete, hypDelete, hypDeleteButCopyAuths As HyperLink
#End Region

        Protected Sub CopyGroupAuthorizations(ByVal currentUser As Long, ByVal targetApp As Integer, ByVal sourceApp As Integer)
            Dim Sql As String = ""
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                Sql = "INSERT INTO dbo.ApplicationsRightsByGroup (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application )" & vbNewLine & _
                                "SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @TargetAppID AS ID_Application" & vbNewLine & _
                                "FROM         dbo.ApplicationsRightsByGroup" & vbNewLine & _
                                "WHERE     (ID_Application = @SourceAppID)"
            Else
                Sql = "INSERT INTO dbo.ApplicationsRightsByGroup (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application, [DevelopmentTeamMember], [IsDenyRule])" & vbNewLine & _
                                "SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @TargetAppID, [DevelopmentTeamMember], [IsDenyRule] AS ID_Application" & vbNewLine & _
                                "FROM         dbo.ApplicationsRightsByGroup" & vbNewLine & _
                                "WHERE     (ID_Application = @SourceAppID)"
            End If
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(Sql, New SqlConnection(cammWebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.BigInt).Value = currentUser
            MyCmd.Parameters.Add("@TargetAppID", SqlDbType.Int).Value = targetApp
            MyCmd.Parameters.Add("@SourceAppID", SqlDbType.Int).Value = sourceApp
            AnyIDataProvider.ExecuteNonQuery(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        Protected Sub CopyUserAuthorizations(ByVal currentUser As Long, ByVal targetApp As Integer, ByVal sourceApp As Integer)
            Dim Sql As String = ""
            If Me.CurrentDbVersion.CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then
                Sql = "INSERT INTO dbo.ApplicationsRightsByUser (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application )" & vbNewLine & _
                                "SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @TargetAppID AS ID_Application" & vbNewLine & _
                                "FROM         dbo.ApplicationsRightsByUser" & vbNewLine & _
                                "WHERE     (ID_Application = @SourceAppID)"
            Else
                Sql = "INSERT INTO dbo.ApplicationsRightsByUser (ID_GroupOrPerson, ReleasedOn, ReleasedBy, ID_Application, [DevelopmentTeamMember])" & vbNewLine & _
                                "SELECT     ID_GroupOrPerson, GETDATE() AS ReleasedOn, @ReleasedByUserID AS ReleasedBy, @TargetAppID, [DevelopmentTeamMember]" & vbNewLine & _
                                "FROM         dbo.ApplicationsRightsByUser" & vbNewLine & _
                                "WHERE     (ID_Application = @SourceAppID)"
            End If
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(Sql, New SqlConnection(cammWebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.BigInt).Value = currentUser
            MyCmd.Parameters.Add("@TargetAppID", SqlDbType.Int).Value = targetApp
            MyCmd.Parameters.Add("@SourceAppID", SqlDbType.Int).Value = sourceApp
            AnyIDataProvider.ExecuteNonQuery(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub
        Protected Sub CopyAuthorizations(ByVal currentUser As Long, ByVal targetApp As Integer, ByVal sourceApp As Integer)
            CopyGroupAuthorizations(currentUser, targetApp, sourceApp)
            CopyUserAuthorizations(currentUser, targetApp, sourceApp)
        End Sub

#Region "Page Events"
        Private Sub AppRightsDeleteTransmission_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim CurUserID As Long
            If CInt(Request.QueryString("AuthsAsAppID")) <> 0 AndAlso CLng(Request.QueryString("ID")) <> 0 AndAlso Request.QueryString("DEL") = "NOW" AndAlso Request.QueryString("token") = Session.SessionID Then
                CurUserID = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                Dim Success As Boolean = True
                Try
                    Dim MyCmd As New System.Data.SqlClient.SqlCommand("AdminPrivate_SetAuthorizationInherition", New SqlConnection(cammWebManager.ConnectionString))
                    MyCmd.CommandType = CommandType.StoredProcedure
                    MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.BigInt).Value = CurUserID
                    MyCmd.Parameters.Add("@IDApp", SqlDbType.Int).Value = CInt(Request.QueryString("ID"))
                    MyCmd.Parameters.Add("@InheritsFrom", SqlDbType.Int).Value = DBNull.Value
                    Dim ResulT As Object
                    ResulT = ExecuteScalar(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection)
                    If ResulT Is Nothing OrElse IsDBNull(ResulT) Then
                        lblErrMsg.Text = "Undefined error detected!"
                        Success = False
                    ElseIf CInt(ResulT) = -1 Then
                        'Success :-)
                    Else
                        lblErrMsg.Text = "Erasing of transmission failed!"
                        Success = False
                    End If
                Catch ex As Exception
                    lblErrMsg.Text = "Erasing of transmission failed (" & ex.Message & ")!"
                    Success = False
                End Try
                If Success AndAlso Request.QueryString("CopyAuths") = "1" Then
                    Try
                        CopyAuthorizations(CurUserID, CInt(Request.QueryString("ID")), CInt(Request.QueryString("AuthsAsAppID")))

                    Catch ex As Exception
                        If lblErrMsg.Text <> Nothing Then lblErrMsg.Text &= "<br />"
                        lblErrMsg.Text = "Copying authorizations from inherited security object failed (" & ex.Message & ")!"
                        Success = False
                    End Try
                End If
                If Success Then
                    Response.Redirect("apprights.aspx?Application=" & CInt(Request.QueryString("ID")))
                End If
            Else
                Dim DisplAppID As Integer
                Dim LanguageDescription As String
                Dim ServerDescription As String
                Dim strBuilder As New Text.StringBuilder
                Dim dtResult As DataTable
                Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", CLng(Request.QueryString("ID")))}
                dtResult = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), _
                                     "SELECT Applications.*, Languages.Description FROM Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID WHERE Applications.ID = @ID ORDER BY Applications.Title", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")
                Dim iCount As Integer = 0
                While iCount < dtResult.Rows.Count
                    If Utils.Nz(Request.QueryString("ID"), 0L) = Utils.Nz(dtResult.Rows(iCount)("ID"), 0L) Then
                        strBuilder.Append("ID " & CLng(Request.QueryString("ID")) & ", " & Server.HtmlEncode(Utils.Nz(dtResult.Rows(iCount)("Title"), String.Empty)))
                        DisplAppID = Utils.Nz(dtResult.Rows(iCount)("ID"), 0)
                        LanguageDescription = CompuMaster.camm.WebManager.Utils.Nz(dtResult.Rows(iCount)("Description"), String.Empty)
                        ServerDescription = Utils.Nz(New camm.WebManager.WMSystem.ServerInformation(CInt(Val(dtResult.Rows(iCount)("LocationID"))), cammWebManager).Description, String.Empty)
                        strBuilder.Append(" (" & Server.HtmlEncode(ServerDescription) & " / " & Server.HtmlEncode(LanguageDescription))
                        strBuilder.Append(")<input type=""hidden"" name=""app_id"" value=""")
                        strBuilder.Append(DisplAppID & """")
                    Else
                        strBuilder.Append("<font color=""red"">Application not found!</font>")
                    End If
                    iCount += 1
                End While
                gcAddHtml.InnerHtml = strBuilder.ToString
                hypDelete.NavigateUrl = "apprights_delete_transmission.aspx?ID=" & Request.QueryString("ID").ToString & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID") & "&DEL=NOW&APPID=" & Request.QueryString("ID").ToString & "&token=" & Session.SessionID
                hypDelete.Text = "Yes, delete it!"
                hypNoDelete.NavigateUrl = "apprights.aspx?Application=" & Request.QueryString("ID").ToString & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID")
                hypNoDelete.Text = "No! Don't touch it!"
                If Not hypDeleteButCopyAuths Is Nothing Then
                    'Rename first choice to be more appropriate
                    hypDelete.Text = "Yes, delete it and drop all inherited authorizations!"
                    'Provide 2nd delete button
                    hypDeleteButCopyAuths.Text = "Yes, delete it, but keep a copy of all inherited authorizations"
                    hypDeleteButCopyAuths.NavigateUrl = "apprights_delete_transmission.aspx?ID=" & Request.QueryString("ID").ToString & "&AuthsAsAppID=" & Request.QueryString("AuthsAsAppID") & "&DEL=NOW&APPID=" & Request.QueryString("ID").ToString & "&CopyAuths=1&token=" & Session.SessionID
                End If
            End If
        End Sub
#End Region

    End Class

End Namespace