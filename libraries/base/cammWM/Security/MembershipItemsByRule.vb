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

Namespace CompuMaster.camm.WebManager.Security

    Public Class MembershipItemsByRule
        Friend Sub New(allowRuleItems As GroupInformation(), denyRuleItems As GroupInformation())
            Me._AllowRule = allowRuleItems
            Me._DenyRule = denyRuleItems
        End Sub

        Private _AllowRule As GroupInformation()
        Private _DenyRule As GroupInformation()
        Private _Effective As GroupInformation()

        Public ReadOnly Property AllowRule As GroupInformation()
            Get
                Return Me._AllowRule
            End Get
        End Property
        Public ReadOnly Property DenyRule As GroupInformation()
            Get
                Return Me._DenyRule
            End Get
        End Property
        Public ReadOnly Property Effective As GroupInformation()
            Get
                If Me._Effective Is Nothing Then
                    Dim RuleResult As New ArrayList()
                    For AllowRuleCounter As Integer = 0 To Me._AllowRule.Length - 1
                        Dim IsDenied As Boolean = False
                        For DenyRuleCounter As Integer = 0 To Me._DenyRule.Length - 1
                            If Me._AllowRule(AllowRuleCounter).ID = Me._DenyRule(DenyRuleCounter).ID Then
                                IsDenied = True
                                Exit For
                            End If
                        Next
                        If IsDenied = False Then
                            RuleResult.Add(Me._AllowRule(AllowRuleCounter))
                        End If
                    Next
                    Me._Effective = CType(RuleResult.ToArray(GetType(GroupInformation)), GroupInformation())
                End If
                Return Me._Effective
            End Get
        End Property
    End Class

End Namespace