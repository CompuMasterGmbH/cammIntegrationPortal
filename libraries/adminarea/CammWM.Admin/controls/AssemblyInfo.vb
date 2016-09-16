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

Imports System.Reflection

Namespace CompuMaster.camm.WebManager.Controls.Administration

    Public Class AssemblyInfo
        ''' <summary>
        ''' Assembly (title and version) information
        ''' </summary>
        Public Function GetAssemblyInfo() As String
            Dim curAsm As System.Reflection.Assembly
            Dim attrTitle As AssemblyTitleAttribute
            'get the currently executing assembly instance
            curAsm = System.Reflection.Assembly.GetExecutingAssembly()
            'get the custom attributes in the assembly
            Dim attrs() As Object
            Dim result As String = ""

            attrs = curAsm.GetCustomAttributes(GetType(AssemblyTitleAttribute), False)
            If attrs.Length > 0 Then
                attrTitle = CType(attrs(0), Reflection.AssemblyTitleAttribute)
                result = attrTitle.Title.ToString
                If curAsm.GetName.Version.ToString.Trim <> "" Then result += " v." & curAsm.GetName.Version.ToString.Trim & ""
            End If

            Return result
        End Function
    End Class

End Namespace
