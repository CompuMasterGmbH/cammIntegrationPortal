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

Option Strict Off
Option Explicit On

Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.ui
Imports System.Web.ui.WebControls

Namespace CompuMaster.camm.WebManager.Modules.Redirector.Pages

    Namespace Administration

        Public Class Overview
            Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

            Protected WithEvents refresh As UI.WebControls.Button
            Protected DataTable As UI.WebControls.DataGrid


            Protected Overridable Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
                cammWebManager.AuthorizeDocumentAccess("System - Administration - Redirections")
                LoadData()
            End Sub

            Protected Overridable Sub LoadData()
                Dim selectSQL As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select * from Redirects_ToAddr ORDER BY ID"
                Dim MyConn As New SqlConnection(cammWebManager.ConnectionString)
                Dim MyCmd As New SqlCommand(selectSQL, MyConn)
                Dim adapter As New SqlDataAdapter(MyCmd)
                Dim ds As New DataSet
                Try
                    MyConn.Open()
                    adapter.Fill(ds, "Redirects_ToAddr")
                Catch ex As Exception
                    cammWebManager.Log.Exception(ex)
                Finally
                    If Not adapter Is Nothing Then
                        adapter.Dispose()
                    End If
                    If Not MyConn Is Nothing Then
                        If MyConn.State <> ConnectionState.Closed Then
                            MyConn.Close()
                        End If
                        MyConn.Dispose()
                    End If
                End Try
                DataTable.DataSource = ds.Tables("Redirects_ToAddr")
                DataTable.DataBind()
            End Sub

            Protected Sub delete_button_click(ByVal sender As Object, ByVal e As CommandEventArgs)
                Dim MyConn As New SqlConnection(cammWebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                Dim ReDirID As Integer = CType(CType(sender, UI.WebControls.LinkButton).CommandName, Integer)
                cammWebManager.Log.Warn("Removal of redirection ID " & ReDirID, WMSystem.DebugLevels.Medium_LoggingOfDebugInformation)
                MyCmd.Connection = MyConn
                MyCmd.CommandText = "Delete from Redirects_ToAddr where id=@ID"
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ReDirID
                Try
                    MyConn.Open()
                    MyCmd.ExecuteNonQuery()
                Catch ex As Exception
                    cammWebManager.Log.Exception(ex)
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyConn Is Nothing Then
                        If MyConn.State <> ConnectionState.Closed Then
                            MyConn.Close()
                        End If
                        MyConn.Dispose()
                    End If
                End Try
                LoadData()
            End Sub

            Protected Sub button_itemdatabound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
                If e.Item.ItemType = ListItemType.Header Then
                    CType(e.Item.Cells(4).Controls(1), LinkButton).Attributes.Add("onclick", "popup('new_modify.aspx?b=" & "new1&id=0');")
                End If
                If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                    CType(e.Item.Cells(4).Controls(1), LinkButton).Attributes.Add("onclick", "popup('new_modify.aspx?b=" & e.Item.Cells(4).Controls(1).ID & "&id=" & CType(e.Item.Cells(4).Controls(1), LinkButton).CommandName & "');")
                    CType(e.Item.Cells(4).Controls(3), LinkButton).Attributes.Add("onclick", "return confirm('Are you sure?');")
                    CType(e.Item.Cells(4).Controls(5), LinkButton).Attributes.Add("onclick", "popup2('create_link.aspx?id=" & CType(e.Item.Cells(4).Controls(5), LinkButton).CommandName & "');")
                End If
            End Sub

        End Class

        Public Class ShowRedirectorLinks
            Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

            Protected Table As WebControls.Table

            Protected Overridable Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

                cammWebManager.AuthorizeDocumentAccess("System - Administration - Redirections")

                Dim id As Integer
                id = Request.QueryString("id")
                Dim i As Integer
                Dim groupinfo As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
                For i = 0 To cammWebManager.System_GetServerGroupsInfo.Length - 1
                    Dim rowNew As New TableRow
                    Dim cellNew1 As New TableCell
                    Dim cellNew2 As New TableCell
                    groupinfo = cammWebManager.System_GetServerGroupsInfo
                    Dim lbl1 As New Label
                    Dim lbl2 As New HtmlControls.HtmlAnchor
                    lbl1.Text = groupinfo(i).Title
                    lbl2.InnerText = groupinfo(i).MasterServer.ServerURL & "/sysdata/modules/redir/index.aspx?R=" & id
                    lbl2.HRef = lbl2.InnerText
                    lbl2.Target = "_blank"
                    cellNew1.Controls.Add(lbl1)
                    cellNew2.Controls.Add(lbl2)
                    rowNew.Controls.Add(cellNew1)
                    rowNew.Controls.Add(cellNew2)
                    Table.Controls.Add(rowNew)
                Next
            End Sub

        End Class

        Public Class EditRedirection
            Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

            Protected WithEvents LabelID As Web.UI.WebControls.Label
            Protected WithEvents LabelTitle As Web.UI.WebControls.Label
            Protected WithEvents TextDesc As Web.UI.WebControls.TextBox
            Protected WithEvents TextRT As Web.UI.WebControls.TextBox
            Protected WithEvents TextNR As Web.UI.WebControls.TextBox
            Protected WithEvents button_send As Web.UI.WebControls.Button
            Protected WithEvents button_reset As Web.UI.WebControls.Button
            Protected WithEvents ActivateJScript As Web.UI.WebControls.Panel
            Protected RowID As HtmlControls.HtmlTableRow

            Private button As String
            Private ReDirID As Integer

            Protected Overridable Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

                cammWebManager.AuthorizeDocumentAccess("System - Administration - Redirections")

                button = Request.QueryString("b")
                ReDirID = Request.QueryString("id")
                If Not IsPostBack Then
                    ActivateJScript.Visible = False
                    If ReDirID <> 0 Then
                        LabelTitle.Text = "Modify"

                        Dim MyConn As New SqlConnection(cammWebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand
                        MyCmd.Connection = MyConn
                        MyCmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select * from Redirects_ToAddr where id=@ID"
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ReDirID

                        Dim reader As SqlDataReader = Nothing
                        Try
                            MyConn.Open()
                            reader = MyCmd.ExecuteReader()
                            reader.Read()
                            LabelID.Text = ReDirID
                            ViewState("desc") = reader(1)
                            ViewState("rt") = reader(2)
                            ViewState("nr") = reader(3)
                            TextDesc.Text = ViewState("desc")
                            TextRT.Text = ViewState("rt")
                            TextNR.Text = ViewState("nr")
                        Catch ex As Exception
                            cammWebManager.Log.Exception(ex)
                        Finally
                            If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                                reader.Close()
                            End If
                            If Not MyCmd Is Nothing Then
                                MyCmd.Dispose()
                            End If
                            If Not MyConn Is Nothing Then
                                If MyConn.State <> ConnectionState.Closed Then
                                    MyConn.Close()
                                End If
                                MyConn.Dispose()
                            End If
                        End Try
                    Else
                        LabelTitle.Text = "New"
                        RowID.Visible = False

                    End If
                End If

            End Sub

            Protected Sub button_send_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_send.Click

                Dim MyConn As New SqlConnection(cammWebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.Connection = MyConn

                If button = "new1" Then
                    LabelID.Text = ""
                    cammWebManager.Log.Write("Creating new redirection", WMSystem.DebugLevels.Medium_LoggingOfDebugInformation)
                    MyCmd.CommandText = "Insert into Redirects_ToAddr (description,redirectto,numberofredirections) " & _
                                        "values (@Description, @URL, @NumberOfRedirs)"
                    MyCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = TextDesc.Text
                    MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = TextRT.Text
                    MyCmd.Parameters.Add("@NumberOfRedirs", SqlDbType.Int).Value = Utils.TryCInt(TextNR.Text)
                Else
                    cammWebManager.Log.Write("Update of redirection ID " & ReDirID, WMSystem.DebugLevels.Medium_LoggingOfDebugInformation)
                    MyCmd.CommandText = "Update Redirects_ToAddr " & _
                                        "Set description=@Description,redirectto=@URL,numberofredirections=@NumberOfRedirs " & _
                                        "where id=@ID"
                    MyCmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = TextDesc.Text
                    MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = TextRT.Text
                    MyCmd.Parameters.Add("@NumberOfRedirs", SqlDbType.Int).Value = Utils.TryCInt(TextNR.Text)
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ReDirID
                End If
                Try
                    MyConn.Open()
                    MyCmd.ExecuteNonQuery()
                    ActivateJScript.Visible = True
                Catch ex As Exception
                    cammWebManager.Log.Exception(ex)
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyConn Is Nothing Then
                        If MyConn.State <> ConnectionState.Closed Then
                            MyConn.Close()
                        End If
                        MyConn.Dispose()
                    End If
                End Try
            End Sub

            Protected Sub button_reset_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button_reset.Click
                If ReDirID = 0 Then
                    LabelID.Text = ""
                    TextDesc.Text = ""
                    TextRT.Text = ""
                    TextNR.Text = ""
                Else
                    LabelID.Text = ReDirID
                    TextDesc.Text = ViewState("desc")
                    TextRT.Text = ViewState("rt")
                    TextNR.Text = ViewState("nr")
                End If
            End Sub

        End Class

    End Namespace

End Namespace