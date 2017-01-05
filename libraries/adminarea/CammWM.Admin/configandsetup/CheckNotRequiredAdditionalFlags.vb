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

Imports System.Web.UI.WebControls

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

End Namespace
