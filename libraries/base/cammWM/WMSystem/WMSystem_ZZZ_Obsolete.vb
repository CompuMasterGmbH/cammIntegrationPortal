'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager

    'PLEASE NOTE REGARDING THESE CLASSES: 
    'Password security / Subject of removal in V3.xx"
    'Notifications / Subject of removal in V3.xx"

    Partial Public Class WMSystem

        Public Class WMPasswordSecurity 'Subject of removal in V3.xx
            Inherits CompuMaster.camm.WebManager.WMPasswordSecurity
            Public Sub New(ByRef WMSystem As WMSystem)
                MyBase.New(WMSystem)
            End Sub
        End Class

        Public Class WMPasswordSecurityInspectionSeverity 'Subject of removal in V3.xx
            Inherits CompuMaster.camm.WebManager.WMPasswordSecurityInspectionSeverity
            Public Sub New(ByRef WMSystem As WMSystem)
                MyBase.New(WMSystem)
            End Sub
            Public Sub New(ByRef WMSystem As WMSystem, ByVal RequiredPasswordLength As Integer, ByVal RequiredComplexityPoints As Integer)
                MyBase.New(WMSystem, RequiredPasswordLength, RequiredComplexityPoints)
            End Sub
        End Class

        '<Obsolete("Use CompuMaster.camm.WebManager.WMNotifications instead")> _
        Public Class WMNotifications 'Subject of removal in V3.xx
            Inherits CompuMaster.camm.WebManager.WMNotifications
            Public Sub New(ByRef WMSystem As WMSystem)
                MyBase.New(WMSystem)
            End Sub
        End Class

#Region "Compatiblity methods"
        ''' <summary>
        '''     Replaces placeholders in a string by defined values
        ''' </summary>
        ''' <param name="message">The string with the placeholders</param>
        ''' <param name="values">One or more values which should be used for replacement</param>
        ''' <returns>The new resulting string</returns>
        ''' <remarks>
        '''     <para>Supported special character combinations are <code>\t</code>, <code>\r</code>, <code>\n</code>, <code>\\</code>, <code>\[</code></para>
        '''     <para>Supported placeholders are <code>[*]</code>, <code>[n:1..9]</code></para>
        ''' </remarks>
        <Obsolete("Use Utils.sprintf instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function sprintf(ByVal message As String, ByVal ParamArray values() As Object) As String
            Return CompuMaster.camm.WebManager.Utils.sprintf(message, values)
        End Function
#End Region

    End Class

End Namespace
