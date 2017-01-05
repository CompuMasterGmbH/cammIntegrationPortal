'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Option Explicit On
Option Strict On

Namespace CompuMaster.camm.Controls '.WebControls ?!?

    'Inactive controls
#If Implemented Then

    ''' <summary>
    ''' WebPropertyGrid (Reflect object data)
    ''' </summary>
    Public Class WebPropertyGrid
        Inherits Web.UI.WebControls.DataGrid

        Public Function GetBindableObject(ByVal source As Object) As DataView
            Dim dt As New DataTable
            dt.Columns.Add("name", GetType(String))
            dt.Columns.Add("value", GetType(String))

            Dim t As Type = source.GetType
            For Each fi As Reflection.FieldInfo In t.GetFields(BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance)
                dt.Rows.Add(New Object() {fi.Name, fi.GetValue(source)})
            Next
            Return dt.DefaultView
        End Function

        Public Sub GetDataFromBindableObject(ByVal target As Object, ByVal bindableObject As DataView)
            Dim t As Type = target.GetType

            For Each r As DataRowView In bindableObject
                Dim fi As Reflection.FieldInfo = t.GetField(r("name"), BindingFlags.NonPublic Or BindingFlags.Public Or BindingFlags.Instance)
                If fi.FieldType Is GetType(String) Then
                    fi.SetValue(target, r("value"))
                ElseIf fi.FieldType Is GetType(DateTime) Then
                    fi.SetValue(target, DateTime.Parse(r("value")))
                End If
            Next
        End Sub

        Protected Overrides Sub CreateControlHierarchy(ByVal useDataSource As Boolean)

        End Sub

        Protected Overrides Sub PrepareControlHierarchy()

        End Sub
    End Class

    Public Class PageHeader
        Inherits System.Web.UI.UserControl

#Region "Control logic"

        Protected Overrides Sub Render(ByVal Output As HtmlTextWriter)

        End Sub

#End Region

    End Class

#End If

End Namespace
