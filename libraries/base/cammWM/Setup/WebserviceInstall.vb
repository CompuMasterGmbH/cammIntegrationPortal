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

Option Explicit On
Option Strict On

Imports System.Web.Services

Namespace CompuMaster.camm.WebManager.Setup.WebServices

    <System.Runtime.InteropServices.ComVisible(False)> Public Class Install
        Inherits WebService

        Private Const SecurityObjectName As String = "System - User Administration - ServerSetup"

#Region "Internal variables for the properties"
        Private _DebugLevel As Integer = 0
        Private _InstallNewDB As Boolean = False
        Private _UseExistingButEmptyDB As Boolean = False
        Private _TextBoxDBCatalog As String = ""
        Private _TextBoxDBServer As String = ""
        Private _TextBoxAuthUser As String = ""
        Private _TextBoxAuthPassword As String = ""
        Private _TextBoxAuthUserAdmin As String = ""
        Private _TextBoxAuthPasswordAdmin As String = ""
        Private _TextBoxProtocol As String = ""
        Private _TextBoxServerName As String = ""
        Private _TextBoxPort As String = ""
        Private _TextBoxServerIP As String = ""
        Private _TextBoxSGroupTitle As String = ""
        Private _TextBoxSGroupNavTitle As String = ""
        Private _TextBoxCompanyURL As String = ""
        Private _TextBoxSGroupContact As String = ""
        Private _TextBoxCompanyName As String = ""
        Private _TextBoxCompanyFormerName As String = ""
#End Region

        Public ReadOnly Property DebugLevel() As Integer
            Get
                ' This property shows the currently used Debug Level stored in web.conig
                If _DebugLevel = 0 Then
                    ' TODO: Read the debug level from Web.config and store
                    ' it in _DebugLevel
                    _DebugLevel = 1
                End If
                Return _DebugLevel
            End Get
        End Property

        Public Property InstallNewDB() As Boolean
            Get
                Return _InstallNewDB
            End Get
            Set(ByVal Value As Boolean)
                _InstallNewDB = Value
            End Set
        End Property

        Public Property UseExistingButEmptyDB() As Boolean
            ' If this property is set to true, then the Installer expects
            ' that the database has to exist and does not even try to
            ' create a new one - this even means that all data in this
            ' database will be overwritten without any prompt for
            ' confirmation!!!
            Get
                Return _UseExistingButEmptyDB
            End Get
            Set(ByVal Value As Boolean)
                _UseExistingButEmptyDB = Value
            End Set
        End Property

        ' TODO: Add properties for storage of user defined data, due to the 
        ' Web Controls in .Pages.Install

        <WebMethod()> Public Function IsDatabaseServerAccessible(ByVal ConnectionStringServerAdministration As String) As Boolean
            ' TODO: This function verifies, that the Database server is accessible
            Return False
        End Function

        <WebMethod()> Public Function DatabaseExists(ByVal ConnectionString As String) As Boolean
            ' TODO: This function verifies that the Database itself is 
            ' accessible to the user defined in the connection string
            Return False
        End Function

        <WebMethod()> Public Function GetConnectionString() As String
            ' TODO: Returns a usable connection string to Database 
            Return _
                 "SERVER=" & ";" &
                 "DATABASE=" & ";" &
                 "UID=" & ";" &
                 "PWD=" & ";" &
                 "Pooling=false;"
        End Function

        <WebMethod()> Public Function GetConnectionString_ServerAdministration() As String
            ' TODO: Returns a usable connection string to Server 
            Return _
                 "SERVER=" & ";" &
                 "UID=" & ";" &
                 "PWD=" & ";" &
                 "Pooling=false;"
        End Function

        <WebMethod()> Public Function GetConnectionString_ServerAdministration_sa() As String
            ' TODO: Returns a usable connection string to Server for Admin
            ' purposes. returns a blank string if no admin account data available
            If True Then
                Return _
                    "SERVER=" & ";" &
                    "DATABASE=" & ";" &
                    "UID=" & ";" &
                    "PWD=" & ";" &
                    "Pooling=false"
            Else
                Return ""
            End If
        End Function

        <WebMethod()> Public Function GetDataset() As DataSet
            Return New DataSet("root")
        End Function

        Public Class Update
            ' Even much more TODO: Fill this class corresponding to
            ' .pages.Update and have a look, how the update.aspx in
            ' /system/admin/install/ works
        End Class
    End Class

End Namespace