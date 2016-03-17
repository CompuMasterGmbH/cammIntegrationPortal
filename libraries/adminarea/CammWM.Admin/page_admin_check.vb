'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie k�nnen es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder sp�teren ver�ffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es n�tzlich sein wird, aber OHNE JEDE GEW�HRLEISTUNG, bereitgestellt; sogar ohne die implizite Gew�hrleistung der MARKTF�HIGKEIT oder EIGNUNG F�R EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License f�r weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden f�r Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Strict Off
Imports System.Web.UI.WebControls
Imports System.IO

Namespace CompuMaster.camm.WebManager.Pages.Administration
    Public Class CheckNotRequiredAdditionalFlags
        Inherits Page
#Region "Variable Declaration"
        Protected NotRequiredFlags As DataGrid
#End Region

#Region "Page Events"
        Private Sub Page_OnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            'Setup result datatable
            Dim resultDt As New DataTable("result")
            resultDt.Columns.Add("FlagName", GetType(String))

            'Get all additional flags, which are not required by any security object
            Dim FlagList() As String = CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(cammWebManager)

            For Each flag As String In FlagList
                Dim row As DataRow = resultDt.NewRow
                row("FlagName") = flag
                resultDt.Rows.Add(row)
            Next
            NotRequiredFlags.DataSource = resultDt
            NotRequiredFlags.DataBind()
        End Sub

        Private Function GetFlags(ByVal layer As CompuMaster.camm.WebManager.IDataLayer) As String()
            Return layer.ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(cammWebManager)
        End Function

#End Region

    End Class

    Public Class SystemCheckup
        Inherits Page

#Region "Variable Declaration"
        Protected infolbl As Label
        Protected resultTable As UI.HtmlControls.HtmlTable
#End Region

#Region "Page_Events"

        Private Sub Page_Onload(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            Dim xmlStr As String = ResourceFileConsitency("fileList.xml_bzip2")
            Dim ds As DataSet = CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.ConvertXmlToDataset(xmlStr)
            Dim dt As DataTable = ds.Tables(0)

            Dim missingFiles As Boolean = False

            Dim resultDT As New DataTable("result")
            resultDT.Columns.Add("path")
            resultDT.Columns.Add("found")

            For myCounter As Integer = 0 To dt.Rows.Count - 1
                Dim row As DataRow = resultDT.NewRow
                row(0) = dt.Rows(myCounter)(0)
                row(1) = File.Exists(Server.MapPath(dt.Rows(myCounter)(0)))
                resultDT.Rows.Add(row)
            Next

            resultDT = CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.GetDataTableClone(resultDT, Nothing, "found asc")

            For myResultCounter As Integer = 0 To resultDT.Rows.Count - 1
                Dim row As New UI.HtmlControls.HtmlTableRow
                Dim cell As New UI.HtmlControls.HtmlTableCell
                cell.InnerText = resultDT.Rows(myResultCounter)(0)
                row.Cells.Add(cell)
                cell = New UI.HtmlControls.HtmlTableCell
                If CStr(resultDT.Rows(myResultCounter)(1)).ToLower = "true" Then
                    cell.InnerHtml = "<font color=""green"">True</font>"
                Else
                    missingFiles = True
                    cell.InnerHtml = "<font color=""red"">False</font>"
                End If
                row.Cells.Add(cell)
                resultTable.Rows.Add(row)
            Next

            If missingFiles Then
                infolbl.Text = "One or more files are missing!"
                infolbl.ForeColor = Drawing.Color.Red
            Else
                infolbl.Text = "No files are missing!"
                infolbl.ForeColor = Drawing.Color.Green
            End If

        End Sub

        Private Function ResourceFileConsitency(ByVal name As String) As String
            Dim Result As String = Nothing
            Try
                Result = ResourceStringFileConsitency(name)
            Catch
                Throw New Exception("Embedded string resource """ & name & """ can't be found, search for file, now")
            End Try
            Return Result
        End Function

        Private Function ResourceStringFileConsitency(ByVal name As String) As String
            Dim Result As String = Nothing

            Dim ResMngr As Resources.ResourceManager = Nothing
            Try
                'Try using de-bzipped data, first
                ResMngr = New Resources.ResourceManager("fileConsistencyList", System.Reflection.Assembly.GetAssembly(GetType(SystemCheckup)))
                ResMngr.IgnoreCase = True
                Result = Utils.Compression.DeCompress(ResMngr.GetString(name), Utils.Compression.CompressionType.BZip2)
            Catch
                'UnBZipping hasn't worked, use raw data as last alternative
                If ResMngr Is Nothing Then
                    ResMngr = New Resources.ResourceManager("fileConsistencyList", System.Reflection.Assembly.GetAssembly(GetType(SystemCheckup)))
                End If
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(name)
            Finally
                If Not ResMngr Is Nothing Then ResMngr.ReleaseAllResources()
            End Try

            Return Result

        End Function

#End Region

    End Class
End Namespace
