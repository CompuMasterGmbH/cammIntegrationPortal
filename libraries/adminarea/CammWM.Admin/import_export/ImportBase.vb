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

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     Import
    ''' </summary>
    Public Class ImportBase
        Inherits Page
        ''' <summary>
        '''     Available action types
        ''' </summary>
        Public Enum ImportActions
            ''' <summary>
            '''     Insert users which haven't existed yet as well as update users which already exist
            ''' </summary>
            InsertOrUpdate = 4
            ''' <summary>
            '''     Only insert items which haven't existed
            ''' </summary>
            InsertOnly = 2
            ''' <summary>
            '''     Only update users which already exist
            ''' </summary>
            UpdateOnly = 3
            ''' <summary>
            '''     Remove all items specified in the import file
            ''' </summary>
            Remove = 1
            ''' <summary>
            '''     Memberships/authorization shall be set exactly as defined in the import file
            ''' </summary>
            ''' <remarks>
            '''     Items will be inserted or removed as needed to fit the requirements
            ''' </remarks>
            FitExact = 5
        End Enum
        ''' <summary>
        '''     The selected import action
        ''' </summary>
        ''' <value></value>
        Protected Property ImportAction() As ImportActions
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportAction"), ImportActions)
            End Get
            Set(ByVal Value As ImportActions)
                Session("WebManager.Administration.Import.UserList.ImportAction") = Value
            End Set
        End Property
        ''' <summary>
        '''     The selected import action for memberships
        ''' </summary>
        ''' <value></value>
        Protected Property ImportActionMemberships() As ImportActions
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportActionMemberships"), ImportActions)
            End Get
            Set(ByVal Value As ImportActions)
                If Value <> Nothing And Value <> ImportActions.FitExact And Value <> ImportActions.InsertOnly Then
                    Throw New ArgumentException("Invalid value", "ImportActionMemberships")
                End If
                Session("WebManager.Administration.Import.UserList.ImportActionMemberships") = Value
            End Set
        End Property
        ''' <summary>
        '''     The selected import action for authorizations
        ''' </summary>
        ''' <value></value>
        Protected Property ImportActionAuthorizations() As ImportActions
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportActionAuthorizations"), ImportActions)
            End Get
            Set(ByVal Value As ImportActions)
                If Value <> Nothing And Value <> ImportActions.FitExact And Value <> ImportActions.InsertOnly Then
                    Throw New ArgumentException("Invalid value", "ImportActionAuthorizations")
                End If
                Session("WebManager.Administration.Import.UserList.ImportActionAuthorizations") = Value
            End Set
        End Property

        ''' <summary>
        ''' Overwrite user profiles with empty values when the import file provides empty cells OR overwrite user profiles only with existing values from import files while ignoring overwriting of filled user profile fields with empty values from import file
        ''' </summary>
        Protected Property ImportOverwriteWithEmptyCellValues() As Boolean
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.ImportOverwriteWithEmptyCellValues"), Boolean)
            End Get
            Set(ByVal Value As Boolean)
                Session("WebManager.Administration.Import.UserList.ImportOverwriteWithEmptyCellValues") = Value
            End Set
        End Property
        ''' <summary>
        '''     Messages logged by the import process
        ''' </summary>
        ''' <value></value>
        Protected Property MessagesLog() As String
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.MessagesLog"), String)
            End Get
            Set(ByVal Value As String)
                Session("WebManager.Administration.Import.UserList.MessagesLog") = Value
            End Set
        End Property
        ''' <summary>
        '''     The import table with the user information
        ''' </summary>
        ''' <value></value>
        Protected Property ImportTable() As DataTable
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.DataTable"), DataTable)
            End Get
            Set(ByVal Value As DataTable)
                Session("WebManager.Administration.Import.UserList.DataTable") = Value
            End Set
        End Property
        ''' <summary>
        '''     The import table with the user information
        ''' </summary>
        ''' <value></value>
        Protected Property SuppressNotificationMails() As Boolean
            Get
                Return CType(Session("WebManager.Administration.Import.UserList.SuppressNotificationMails"), Boolean)
            End Get
            Set(ByVal Value As Boolean)
                Session("WebManager.Administration.Import.UserList.SuppressNotificationMails") = Value
            End Set
        End Property
        ''' <summary>
        '''     The culture of the import file (required to correctly convert all strings back to their origin data type, e. g. date values)
        ''' </summary>
        ''' <value></value>
        Protected Property ImportFileCulture() As System.Globalization.CultureInfo
            Get
                If Session("WebManager.Administration.Import.UserList.SelectedCulture") Is Nothing Then
                    Return System.Globalization.CultureInfo.CurrentCulture
                Else
                    Return CType(Session("WebManager.Administration.Import.UserList.SelectedCulture"), Globalization.CultureInfo)
                End If
            End Get
            Set(ByVal Value As System.Globalization.CultureInfo)
                Session("WebManager.Administration.Import.UserList.SelectedCulture") = Value
            End Set
        End Property

    End Class

End Namespace