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
    '''     The Apprights_New_Transmession page 
    ''' </summary>
    Public Class Apprights_New_Transmession
        Inherits Page

#Region "Variable Declaration"
        Dim CurUserID As Long
        Dim ErrMsg As String
        Dim dt As New DataTable
        Protected lblErrMsg As Label
        Protected cmbApplicationID, cmbInheritsApplicationID As DropDownList
        Protected WithEvents btnCreateTransmission As Button
#End Region

#Region "Page Events"
        Private Sub Apprights_New_Transmession_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not IsPostBack Then FillDropDownList()
        End Sub
#End Region

#Region "User-Defined Methods"
        Sub FillDropDownList()
            Dim MyCount As Integer
            'Fill DropDwonList cmbApplicationID
            If cmbApplicationID.SelectedValue = "" Then
                cmbApplicationID.Items.Add("Please select!")
                cmbApplicationID.SelectedIndex = 0
            End If

            dt = FillDataTable(New SqlCommand("SELECT Applications.*, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID ORDER BY Title", New SqlConnection(cammWebManager.ConnectionString)), Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            Dim MyAppID As Integer
            If cmbApplicationID.SelectedValue <> "" AndAlso cmbApplicationID.SelectedValue <> "Please select!" Then
                MyAppID = cmbApplicationID.SelectedIndex
            Else
                MyAppID = Utils.Nz(Request.QueryString("ID"), 0)
            End If
            Dim temp As String
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    temp = dt.Columns.Item(MyCount).ToString
                    cmbApplicationID.Items.Add(New ListItem(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")", Utils.Nz(dr("ID"), 0).ToString))
                    cmbApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("Title", Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")")
                    cmbApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("onmouseover", "this.title=this.options[this.selectedIndex].title")
                    If MyAppID = Utils.Nz(dr("ID"), 0) Then
                        cmbApplicationID.SelectedValue = Utils.Nz(dr("ID"), 0).ToString
                    End If
                Next
            End If

            'Fill DropDwonList cmbInheritsApplicationID
            If cmbInheritsApplicationID.SelectedValue = "" Then
                cmbInheritsApplicationID.Items.Add("Please Select!")
                cmbInheritsApplicationID.SelectedIndex = 0
            End If

            Dim sqlParams As SqlParameter() = {New SqlParameter("@ID", MyAppID)}
            dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), _
                                    "SELECT Applications.*, System_Servers.ServerDescription, Languages.Description FROM (Applications LEFT JOIN Languages ON Applications.LanguageID = Languages.ID) LEFT JOIN System_Servers ON Applications.LocationID = System_Servers.ID WHERE Applications.ID <> @ID ORDER BY Title", CommandType.Text, sqlParams, Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    temp = dt.Columns.Item(MyCount).ToString
                    cmbInheritsApplicationID.Items.Add(New ListItem(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")", Utils.Nz(dr("ID"), 0).ToString))
                    cmbInheritsApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("Title", Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")")
                    cmbInheritsApplicationID.Items.FindByText(Utils.Nz(dr("Title"), String.Empty) & " (ID " & Utils.Nz(dr("ID"), 0).ToString & " / " & Utils.Nz(dr("ServerDescription"), String.Empty) & " / " & Utils.Nz(dr("Description"), String.Empty) & ")").Attributes.Add("onmouseover", "this.title=this.options[this.selectedIndex].title")
                    If (Trim(cmbInheritsApplicationID.SelectedValue) = Trim(Utils.Nz(dr("ID"), 0).ToString)) Then
                        cmbInheritsApplicationID.SelectedValue = Utils.Nz(dr("ID"), 0).ToString
                    End If
                Next
            End If
        End Sub
#End Region

#Region "Control Events"
        Private Sub btnCreateTransmission_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateTransmission.Click
            If Not ((cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "Owner") Or cammWebManager.System_GetSubAuthorizationStatus("Applications", CInt(Request("ID")), cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "UpdateRelations"))) Then
                ErrMsg = "No authorization to administrate this application."
            ElseIf cmbApplicationID.SelectedValue <> String.Empty And cmbInheritsApplicationID.SelectedValue <> String.Empty Then
                Dim SqlParams As SqlParameter() = {New SqlParameter("@ReleasedByUserID", CurUserID), New SqlParameter("@IDApp", cmbApplicationID.SelectedValue), New SqlParameter("@InheritsFrom", cmbInheritsApplicationID.SelectedValue)}
                Dim Result As Object
                'dt = FillDataTable(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetAuthorizationInherition", CommandType.StoredProcedure, SqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)

                Try
                    Result = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_SetAuthorizationInherition", CommandType.StoredProcedure, SqlParams, Automations.AutoOpenAndCloseAndDisposeConnection)
                    If Result Is Nothing OrElse IsDBNull(Result) Then
                        ErrMsg = "Undefined error detected!"
                    ElseIf CInt(Result) = -1 Then
                        Dim temp As String
                        temp = cmbApplicationID.SelectedValue
                        Response.Redirect("apprights.aspx?Application=" & cmbApplicationID.SelectedValue & "&AuthsAsAppID=" & cmbInheritsApplicationID.SelectedValue)
                    Else
                        ErrMsg = "Transmission creation failed!"
                    End If
                Catch ex As Exception
                    ErrMsg = "Authorization creation failed! (" & ex.Message & ")"
                End Try

            ElseIf cmbApplicationID.SelectedValue = "" And cmbInheritsApplicationID.SelectedValue = "" Then
            Else
                ErrMsg = "Transmission details:"
            End If

            If ErrMsg <> "" Then
                lblErrMsg.Text = ErrMsg
            End If
        End Sub
#End Region

    End Class

End Namespace