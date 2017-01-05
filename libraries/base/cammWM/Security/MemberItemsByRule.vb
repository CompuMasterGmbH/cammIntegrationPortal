'Copyright 2014-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports CompuMaster.camm.WebManager.WMSystem
Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager.Security

    Public Class MemberItemsByRule

        Friend Sub New(webManager As WMSystem, groupID As Integer)
            Me._WebManager = webManager
            Me._GroupID = groupID
        End Sub

        Private _WebManager As WMSystem
        Private _GroupID As Integer
        Private _AllowRule As UserInformation()
        Private _DenyRule As UserInformation()
        Private _Effective As UserInformation()

        Public ReadOnly Property AllowRule As UserInformation()
            Get
                If Me._DenyRule Is Nothing Then
                    Me.QueryAllowAndDenyRuleItems()
                End If
                Return Me._AllowRule
            End Get
        End Property
        Public ReadOnly Property DenyRule As UserInformation()
            Get
                If Me._DenyRule Is Nothing Then
                    Me.QueryAllowAndDenyRuleItems()
                End If
                Return Me._DenyRule
            End Get
        End Property
        Public ReadOnly Property Effective As UserInformation()
            Get
                If Me._Effective Is Nothing Then
                    Me.QueryEffectiveRuleItems()
                End If
                Return Me._Effective
            End Get
        End Property

        Private Sub QueryAllowAndDenyRuleItems()
            Dim AllowRuleMemberGroups As New List(Of UserInformation)
            Dim DenyRuleMemberGroups As New List(Of UserInformation)
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("", MyConn)
            MyCmd.CommandType = CommandType.Text
            If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                MyCmd.CommandText = "Select benutzer.*, memberships.IsDenyRule From memberships inner Join benutzer On memberships.id_user = benutzer.id Where memberships.id_group = @ID "
            Else
                MyCmd.CommandText = "Select benutzer.*, CAST(0 AS bit) AS IsDenyRule From memberships inner Join benutzer On memberships.id_user = benutzer.id Where memberships.id_group = @ID"
            End If
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _GroupID
            Dim MemberUsers As DataTable = Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "MemberUsers")
            For MyCounter As Integer = 0 To MemberUsers.Rows.Count - 1
                Dim MyDataRow As DataRow = MemberUsers.Rows(MyCounter)
                Dim usr As New CompuMaster.camm.WebManager.WMSystem.UserInformation(MyDataRow, _WebManager)
                If Utils.Nz(MyDataRow("IsDenyRule"), False) = False Then
                    AllowRuleMemberGroups.Add(usr)
                Else
                    DenyRuleMemberGroups.Add(usr)
                End If
            Next
            Me._AllowRule = AllowRuleMemberGroups.ToArray
            Me._DenyRule = DenyRuleMemberGroups.ToArray
        End Sub

        Private Sub QueryEffectiveRuleItems()
            Dim EffectiveRuleMemberGroups As List(Of UserInformation)
            If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                EffectiveRuleMemberGroups = New List(Of UserInformation)
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("", MyConn)
                MyCmd.CommandType = CommandType.Text
                MyCmd.CommandText = "Select benutzer.* From Memberships_EffectiveRulesWithClonesNthGrade inner Join benutzer On Memberships_EffectiveRulesWithClonesNthGrade.id_user = benutzer.id Where Memberships_EffectiveRulesWithClonesNthGrade.id_group = @ID "
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _GroupID
                Dim MemberUsers As DataTable = Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "MemberUsers")
                For MyCounter As Integer = 0 To MemberUsers.Rows.Count - 1
                    Dim MyDataRow As DataRow = MemberUsers.Rows(MyCounter)
                    Dim usr As New CompuMaster.camm.WebManager.WMSystem.UserInformation(MyDataRow, _WebManager)
                    EffectiveRuleMemberGroups.Add(usr)
                Next
                Me._Effective = EffectiveRuleMemberGroups.ToArray
            Else
                Me._Effective = AllowRule
            End If

        End Sub

    End Class

End Namespace