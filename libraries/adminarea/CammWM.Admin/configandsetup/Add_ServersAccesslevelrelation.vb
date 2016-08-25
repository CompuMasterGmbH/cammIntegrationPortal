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
    '''     A page to create a Server Level relation.
    ''' </summary>
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

End Namespace