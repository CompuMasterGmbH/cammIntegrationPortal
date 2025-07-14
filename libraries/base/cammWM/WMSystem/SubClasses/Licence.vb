﻿'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

    Public Class Environment

        Private _WebManager As WMSystem

        Friend Sub New(ByVal webmanager As WMSystem)
            _WebManager = webmanager
        End Sub

        ''' <summary>
        '''     Licence details of the camm Web-Manager instance
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property Licence() As Licence
            Get
                Return New Licence(_WebManager)
            End Get
        End Property

        Public Const CacheKeyProductName As String = "CwmProductName"
        Public Property CachedProductName As String
            Get
                Return "camm WebManager Community Edition"
            End Get
            Set(value As String)
            End Set
        End Property

        ''' <summary>
        ''' Product name of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Product name could be for example "camm Enterprise WebManager"</remarks>
        Public ReadOnly Property ProductName() As String
            Get
                Return "camm WebManager Community Edition"
            End Get
        End Property

        ''' <summary>
        ''' Shortened licence hash code for camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>This shortened license code is not intended for licence identification, but for project discussions to clarify the instance in discussion</remarks>
        <Obsolete("FEATURE DISABLED")>
        Public ReadOnly Property LicenceKeyShortened As String
            Get
                Return ""
            End Get
        End Property

        ''' <summary>
        ''' Licence hash code for camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        <Obsolete("FEATURE DISABLED")>
        Public ReadOnly Property LicenceKey() As String
            Get
                Return ""
            End Get
        End Property
    End Class

    Public Class Licence

        Friend Sub New(ByVal cammWebManager As WMSystem)
        End Sub

        ''' <summary>
        '''     The licencee, the name or an identifier of the organization 
        ''' </summary>
        ''' <value></value>
        Public Property Licencee() As String
            Get
                Return Nothing
            End Get
            Set(ByVal Value As String)
            End Set
        End Property

    End Class

End Namespace
