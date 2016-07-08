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

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     The base page for all administration page of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public MustInherit Class Page
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Private _FloatingMenu As CompuMaster.camm.WebManager.Controls.Administration.FloatingMenu
        Public Property cammWebManagerAdminMenu() As CompuMaster.camm.WebManager.Controls.Administration.FloatingMenu
            Get
                Return _FloatingMenu
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.Controls.Administration.FloatingMenu)
                _FloatingMenu = Value
            End Set
        End Property

        Private _CurrentAdminIsSecurityOperator As TriState = TriState.UseDefault
        ''' <summary>
        ''' Is the current user a security operator?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Result is cached</remarks>
        Public Function CurrentAdminIsSecurityOperator() As Boolean
            If _CurrentAdminIsSecurityOperator = TriState.UseDefault Then
                If cammWebManager.System_IsSecurityOperator(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                    _CurrentAdminIsSecurityOperator = TriState.True
                Else
                    _CurrentAdminIsSecurityOperator = TriState.False
                End If
            End If
            If _CurrentAdminIsSecurityOperator = TriState.True Then
                Return True
            Else
                Return False
            End If
        End Function

        Private _CurrentAdminIsSupervisor As TriState = TriState.UseDefault
        ''' <summary>
        ''' Is the current user a supervisor?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Result is cached</remarks>
        Public Function CurrentAdminIsSupervisor() As Boolean
            If _CurrentAdminIsSupervisor = TriState.UseDefault Then
                If cammWebManager.System_IsSuperVisor(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                    _CurrentAdminIsSupervisor = TriState.True
                Else
                    _CurrentAdminIsSupervisor = TriState.False
                End If
            End If
            If _CurrentAdminIsSupervisor = TriState.True Then
                Return True
            Else
                Return False
            End If
        End Function

        Private _CurrentAdminIsSecurityMasterApplications As TriState = TriState.UseDefault
        Private _CurrentAdminIsSecurityMasterGroups As TriState = TriState.UseDefault
        Public Enum AdministrationItemType As Byte
            Groups = 1
            Applications = 2
        End Enum
        ''' <summary>
        ''' Is the current user a SecurityMaster?
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Result is cached</remarks>
        Public Function CurrentAdminIsSecurityMaster(securityMasterType As AdministrationItemType) As Boolean
            Select Case securityMasterType
                Case AdministrationItemType.Applications
                    If _CurrentAdminIsSecurityMasterApplications = TriState.UseDefault Then
                        If cammWebManager.System_IsSecurityMaster("Applications", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                            _CurrentAdminIsSecurityMasterApplications = TriState.True
                        Else
                            _CurrentAdminIsSecurityMasterApplications = TriState.False
                        End If
                    End If
                    If _CurrentAdminIsSecurityMasterApplications = TriState.True Then
                        Return True
                    Else
                        Return False
                    End If
                Case AdministrationItemType.Groups
                    If _CurrentAdminIsSecurityMasterGroups = TriState.UseDefault Then
                        If cammWebManager.System_IsSecurityMaster("Groups", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                            _CurrentAdminIsSecurityMasterGroups = TriState.True
                        Else
                            _CurrentAdminIsSecurityMasterGroups = TriState.False
                        End If
                    End If
                    If _CurrentAdminIsSecurityMasterGroups = TriState.True Then
                        Return True
                    Else
                        Return False
                    End If
                Case Else
                    Throw New ArgumentException("Invalid value", "securityMasterType")
            End Select
        End Function

        Public Enum AuthorizationTypeItemIndependent As Byte
            SecurityMaster = 1
            [New] = 2
            ViewAllItems = 3
            ViewAllRelations = 4
        End Enum

        Public Enum AuthorizationTypeItemDependent As Byte
            View = 1
            Update = 2
            ViewRelations = 3
            UpdateRelations = 4
            ViewLogs = 5
            PrimaryContact = 6
            ResponsibleContact = 7
            Owner = 8
        End Enum

        Public Enum AuthorizationTypeEffective As Byte
            View = 1
            Update = 2
            ViewRelations = 3
            UpdateRelations = 4
            ViewLogs = 5
            PrimaryContact = 6
            ResponsibleContact = 7
            Owner = 8
        End Enum

        Private _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective As New System.Collections.Specialized.NameValueCollection
        ''' <summary>
        ''' Is the current admin priviledged for administration of this item?
        ''' </summary>
        ''' <param name="itemType">Applications or Groups</param>
        ''' <param name="itemID">The primary ID of the item in applications/groups table</param>
        ''' <param name="authorizationType">View, UpdateRelations, etc. in their effective meaning (is the user authorized by this single item or by an item-independent setting?)</param>
        ''' <returns>Supervisors and security masters are always granted, all others have to be checked in more details</returns>
        ''' <remarks></remarks>
        Public Function CurrentAdminIsPrivilegedForItemAdministration(itemType As AdministrationItemType, authorizationType As AuthorizationTypeEffective, itemID As Integer) As Boolean
            If Me.CurrentAdminIsSupervisor OrElse Me.CurrentAdminIsSecurityMaster(itemType) Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "1" Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "0" Then
                Return False
            Else
                'Check for general (item independent) privileges
                Select Case authorizationType
                    Case AuthorizationTypeEffective.Update, AuthorizationTypeEffective.UpdateRelations
                        'security masters and supervisors could have general priviledges for that authorization type - but is already checked in code before
                    Case AuthorizationTypeEffective.View
                        If CurrentAdminIsPrivilegedForItemAdministration(itemType, AuthorizationTypeItemIndependent.ViewAllItems) Then Return True
                    Case AuthorizationTypeEffective.ViewRelations
                        If CurrentAdminIsPrivilegedForItemAdministration(itemType, AuthorizationTypeItemIndependent.ViewAllRelations) Then Return True
                End Select
                'Query the database
                Dim MyCmd As New SqlCommand
                With MyCmd
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID FROM [dbo].[System_SubSecurityAdjustments] WHERE TableName = @TableName AND AuthorizationType = @AuthorizationType AND UserID = @UserID AND TablePrimaryIDValue = @TablePrimaryIDValue"
                    .CommandType = CommandType.Text
                    .Connection = New SqlConnection(cammWebManager.ConnectionString)
                    .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AdministrationItemType), itemType)
                    .Parameters.Add("@TablePrimaryIDValue", SqlDbType.Int).Value = itemID
                    .Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AuthorizationTypeEffective), authorizationType)
                    .Parameters.Add("@UserID", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                End With
                Dim DBResult As Integer = Utils.Nz(CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection), 0)
                If DBResult <> 0 Then
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "1"
                    Return True
                Else
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeEffective(itemType & "|" & authorizationType & "|" & itemID) = "0"
                    Return False
                End If
            End If
        End Function

        Private _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent As New System.Collections.Specialized.NameValueCollection
        ''' <summary>
        ''' Is the current admin priviledged for administration of this item in its effective meaning?
        ''' </summary>
        ''' <param name="itemType">Applications or Groups</param>
        ''' <param name="authorizationType">SecurityMaster, ViewAllRelations, New, etc.</param>
        ''' <returns>Supervisors and security masters are always granted, all others have to be checked in more details</returns>
        ''' <remarks></remarks>
        Public Function CurrentAdminIsPrivilegedForItemAdministration(itemType As AdministrationItemType, authorizationType As AuthorizationTypeItemIndependent) As Boolean
            If Me.CurrentAdminIsSupervisor OrElse Me.CurrentAdminIsSecurityMaster(itemType) Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "1" Then
                Return True
            ElseIf _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "0" Then
                Return False
            Else
                'Query the database
                Dim MyCmd As New SqlCommand
                With MyCmd
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT ID FROM [dbo].[System_SubSecurityAdjustments] WHERE TableName = @TableName AND AuthorizationType = @AuthorizationType AND UserID = @UserID AND TablePrimaryIDValue = 0"
                    .CommandType = CommandType.Text
                    .Connection = New SqlConnection(cammWebManager.ConnectionString)
                    .Parameters.Add("@TableName", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AdministrationItemType), itemType)
                    .Parameters.Add("@AuthorizationType", SqlDbType.NVarChar).Value = [Enum].GetName(GetType(AuthorizationTypeItemIndependent), authorizationType)
                    .Parameters.Add("@UserID", SqlDbType.Int).Value = cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)
                End With
                Dim DBResult As Integer = Utils.Nz(CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Automations.AutoOpenAndCloseAndDisposeConnection), 0)
                If DBResult <> 0 Then
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "1"
                    Return True
                Else
                    _CurrentAdminIsPrivilegedForItemAdministration_AuthorizationTypeItemIndependent(itemType & "|" & authorizationType) = "0"
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' Close the sql connection and the sql command safely
        ''' </summary>
        ''' <param name="command"></param>
        ''' <remarks></remarks>
        Protected Friend Sub CloseAndDisposeDbConnectionAndDbCommand(command As SqlClient.SqlCommand)
            If Not command Is Nothing Then
                CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(command.Connection)
                command.Dispose()
            End If
        End Sub

        ''' <summary>
        ''' Close the sql connection and the sql command safely
        ''' </summary>
        ''' <param name="connection"></param>
        ''' <remarks></remarks>
        Protected Friend Sub CloseAndDisposeDbConnection(connection As SqlClient.SqlConnection)
            CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(connection)
        End Sub

        Protected Friend Function CurrentDbVersion() As Version
            Static MyDBVersion As Version
            If MyDBVersion Is Nothing Then
                MyDBVersion = cammWebManager.System_DBVersion_Ex
            End If
            Return MyDBVersion
        End Function

    End Class

End Namespace