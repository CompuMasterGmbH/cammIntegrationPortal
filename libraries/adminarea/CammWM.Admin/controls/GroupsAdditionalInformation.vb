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

Namespace CompuMaster.camm.WebManager.Controls.Administration

    Public Class GroupsAdditionalInformation
        Inherits UserControl

        Public MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation

        Public Function SortAuthorizations(ByVal left As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForGroup, ByVal right As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForGroup) As Integer
            Dim DisplayNameLeft As String, DisplayNameRight As String, IsDevRuleLeft As Boolean, IsAllowRuleLeft As Boolean, IsDevRuleRight As Boolean, IsAllowRuleRight As Boolean
            Try
                'following statement might fail on invalid references to security objects (e.g. security object deleted in table, but authorization entry never deleted for some reasons)
                DisplayNameLeft = left.SecurityObjectInfo.DisplayName
                IsAllowRuleLeft = Not left.IsDenyRule
                IsDevRuleLeft = left.IsDevRule
            Catch
                DisplayNameLeft = "{ERROR: Invalid ID " & left.SecurityObjectID & "}"
            End Try
            Try
                'following statement might fail on invalid references to security objects (e.g. security object deleted in table, but authorization entry never deleted for some reasons)
                DisplayNameRight = right.SecurityObjectInfo.DisplayName
                IsAllowRuleRight = Not right.IsDenyRule
                IsDevRuleRight = right.IsDevRule
            Catch
                DisplayNameRight = "{ERROR: Invalid ID " & right.SecurityObjectID & "}"
            End Try
            If IsAllowRuleLeft.CompareTo(IsAllowRuleRight) <> 0 Then
                Return IsAllowRuleRight.CompareTo(IsAllowRuleLeft)
            ElseIf IsDevRuleLeft.CompareTo(IsDevRuleRight) <> 0 Then
                Return IsDevRuleLeft.CompareTo(IsDevRuleRight)
            Else
                Return DisplayNameLeft.CompareTo(DisplayNameRight)
            End If
        End Function

        Protected Function CombineAuthorizations(group As CompuMaster.camm.WebManager.WMSystem.GroupInformation) As CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForGroup()
            Dim Result As New System.Collections.Generic.List(Of CompuMaster.camm.WebManager.WMSystem.SecurityObjectAuthorizationForGroup)
            Result.AddRange(group.AuthorizationsByRule.AllowRuleStandard)
            Result.AddRange(group.AuthorizationsByRule.AllowRuleDevelopers)
            Result.AddRange(group.AuthorizationsByRule.DenyRuleStandard)
            Result.AddRange(group.AuthorizationsByRule.DenyRuleDevelopers)
            Return Result.ToArray
        End Function

    End Class

End Namespace
