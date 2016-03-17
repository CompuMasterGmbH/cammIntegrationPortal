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

    ''' <summary>
    '''  GroupAuthorizationItemsByRule for usage in SecurityObjectInformation class
    ''' </summary>
    Public MustInherit Class BaseGroupAuthorizationItemsByRule
        Friend Sub New(currentContextServerGroupID As Integer, _
                               allowRuleItemsNonDev As SecurityObjectAuthorizationForGroup(), _
                               allowRuleItemsIsDev As SecurityObjectAuthorizationForGroup(), _
                               denyRuleItemsNonDev As SecurityObjectAuthorizationForGroup(), _
                               denyRuleItemsIsDev As SecurityObjectAuthorizationForGroup())
            Me._AllowRuleNonDev = allowRuleItemsNonDev
            Me._AllowRuleIsDev = allowRuleItemsIsDev
            Me._DenyRuleNonDev = denyRuleItemsNonDev
            Me._DenyRuleIsDev = denyRuleItemsIsDev
            Me._CurrentContextServerGroupID = currentContextServerGroupID
        End Sub

        Private _CurrentContextServerGroupID As Integer
        Private _AllowRuleIsDev As SecurityObjectAuthorizationForGroup()
        Private _AllowRuleNonDev As SecurityObjectAuthorizationForGroup()
        Private _DenyRuleIsDev As SecurityObjectAuthorizationForGroup()
        Private _DenyRuleNonDev As SecurityObjectAuthorizationForGroup()
        Private _EffectiveStandard As SecurityObjectAuthorizationForGroup()
        Private _EffectiveForDevelopment As SecurityObjectAuthorizationForGroup()

        Public ReadOnly Property AllowRuleStandard As SecurityObjectAuthorizationForGroup()
            Get
                Return Me._AllowRuleNonDev
            End Get
        End Property
        Public ReadOnly Property AllowRuleDevelopers As SecurityObjectAuthorizationForGroup()
            Get
                Return Me._AllowRuleIsDev
            End Get
        End Property
        Public ReadOnly Property DenyRuleStandard As SecurityObjectAuthorizationForGroup()
            Get
                Return Me._DenyRuleNonDev
            End Get
        End Property
        Public ReadOnly Property DenyRuleDevelopers As SecurityObjectAuthorizationForGroup()
            Get
                Return Me._DenyRuleIsDev
            End Get
        End Property

        ''' <summary>
        ''' Effective authorizations (in context of user's current server group environment) are the combination of the rules [{AllowDevelopment} - {DenyDevelopment} - {DenyStandard}] + [{AllowStandard} - {DenyStandard}]
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' <para>Authorizations for ServerGroup ID "0" always beat such ones for a ServerGroup ID &lt;&gt; 0</para>
        ''' <para>Group with different authorization setups are effective authorizations by standard rules as follows (0=false, 1=true)
        ''' <list type="table">
        ''' <listheader>
        ''' <AllowDevelopment>AllowDevelopment</AllowDevelopment>
        ''' <DenyDevelopment>DenyDevelopment</DenyDevelopment>
        ''' <AllowStandard>AllowStandard</AllowStandard>
        ''' <DenyStandard>DenyStandard</DenyStandard>
        ''' <Result>Result</Result>
        ''' </listheader>
        ''' <item>
        ''' <AllowDevelopment>0</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>0</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>0</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>0</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>1</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>0</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>1</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>1</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>1</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>1</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>1</DenyStandard>
        ''' <Result>0</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>0</AllowDevelopment>
        ''' <DenyDevelopment>1</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>1</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>1</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>1</DenyStandard>
        ''' <Result>0</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>1</DenyDevelopment>
        ''' <AllowStandard>0</AllowStandard>
        ''' <DenyStandard>0</DenyStandard>
        ''' <Result>0</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>0</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>1</AllowStandard>
        ''' <DenyStandard>1</DenyStandard>
        ''' <Result>0</Result>
        ''' </item>
        ''' <item>
        ''' <AllowDevelopment>1</AllowDevelopment>
        ''' <DenyDevelopment>0</DenyDevelopment>
        ''' <AllowStandard>0</AllowStandard>
        ''' <DenyStandard>1</DenyStandard>
        ''' <Result>0</Result>
        ''' </item>
        ''' </list>
        ''' </para>
        ''' </remarks>
        Friend ReadOnly Property EffectiveStandardInternal(filterType As EffectivityType) As SecurityObjectAuthorizationForGroup()
            Get
                If Me._EffectiveStandard Is Nothing Then
                    Dim RuleResult As New ArrayList()
                    For AllowRuleCounter As Integer = 0 To Me._AllowRuleIsDev.Length - 1
                        If Me._AllowRuleIsDev(AllowRuleCounter).ServerGroupID = 0 OrElse Me._AllowRuleIsDev(AllowRuleCounter).ServerGroupID = _CurrentContextServerGroupID Then
                            Dim IsDenied As Boolean = False
                            For DenyRuleCounter As Integer = 0 To Me._DenyRuleIsDev.Length - 1
                                If (Me._DenyRuleIsDev(DenyRuleCounter).ServerGroupID = 0 OrElse Me._DenyRuleIsDev(DenyRuleCounter).ServerGroupID = _CurrentContextServerGroupID) AndAlso EffictivityFilterComparisonValue(Me._AllowRuleIsDev(AllowRuleCounter), filterType) = EffictivityFilterComparisonValue(Me._DenyRuleIsDev(DenyRuleCounter), filterType) Then
                                    IsDenied = True
                                    Exit For
                                End If
                            Next
                            For DenyRuleCounter As Integer = 0 To Me._DenyRuleNonDev.Length - 1
                                If (Me._DenyRuleNonDev(DenyRuleCounter).ServerGroupID = 0 OrElse Me._DenyRuleNonDev(DenyRuleCounter).ServerGroupID = _CurrentContextServerGroupID) AndAlso EffictivityFilterComparisonValue(Me._AllowRuleIsDev(AllowRuleCounter), filterType) = EffictivityFilterComparisonValue(Me._DenyRuleNonDev(DenyRuleCounter), filterType) Then
                                    IsDenied = True
                                    Exit For
                                End If
                            Next
                            If IsDenied = False Then
                                RuleResult.Add(Me._AllowRuleIsDev(AllowRuleCounter))
                            End If
                        End If
                    Next
                    For AllowRuleCounter As Integer = 0 To Me._AllowRuleNonDev.Length - 1
                        If Me._AllowRuleNonDev(AllowRuleCounter).ServerGroupID = 0 OrElse Me._AllowRuleNonDev(AllowRuleCounter).ServerGroupID = _CurrentContextServerGroupID Then
                            Dim IsDenied As Boolean = False
                            For DenyRuleCounter As Integer = 0 To Me._DenyRuleNonDev.Length - 1
                                If (Me._DenyRuleNonDev(DenyRuleCounter).ServerGroupID = 0 OrElse Me._DenyRuleNonDev(DenyRuleCounter).ServerGroupID = _CurrentContextServerGroupID) AndAlso EffictivityFilterComparisonValue(Me._AllowRuleNonDev(AllowRuleCounter), filterType) = EffictivityFilterComparisonValue(Me._DenyRuleNonDev(DenyRuleCounter), filterType) Then
                                    IsDenied = True
                                    Exit For
                                End If
                            Next
                            If IsDenied = False Then
                                RuleResult.Add(Me._AllowRuleNonDev(AllowRuleCounter))
                            End If
                        End If
                    Next
                    Me._EffectiveStandard = CType(RuleResult.ToArray(GetType(SecurityObjectAuthorizationForGroup)), SecurityObjectAuthorizationForGroup())
                End If
                Return Me._EffectiveStandard
            End Get
        End Property

        Friend Enum EffectivityType As Byte
            SecurityObjectByUserGroup = 0
            GroupBySecurityObject = 1
        End Enum

        Private Function EffictivityFilterComparisonValue(item As SecurityObjectAuthorizationForGroup, filterType As EffectivityType) As Long
            If filterType = EffectivityType.SecurityObjectByUserGroup Then
                Return item.SecurityObjectID
            Else
                Return item.GroupID
            End If
        End Function

        ''' <summary>
        ''' Effective authorizations (in context of user's current server group environment) are the combination of the rules [{AllowIsDev} - {DenyIsDev}]
        ''' </summary>
        ''' <returns>
        ''' <para>Authorizations for ServerGroup ID "0" always beat such ones for a ServerGroup ID &lt;&gt; 0</para>
        ''' </returns>
        Friend ReadOnly Property EffectiveForDevelopmentInternal(filterType As EffectivityType) As SecurityObjectAuthorizationForGroup()
            Get
                If Me._EffectiveForDevelopment Is Nothing Then
                    Dim RuleResult As New ArrayList()
                    For AllowRuleCounter As Integer = 0 To Me._AllowRuleIsDev.Length - 1
                        If Me._AllowRuleIsDev(AllowRuleCounter).ServerGroupID = 0 OrElse Me._AllowRuleIsDev(AllowRuleCounter).ServerGroupID = _CurrentContextServerGroupID Then
                            Dim IsDenied As Boolean = False
                            For DenyRuleCounter As Integer = 0 To Me._DenyRuleIsDev.Length - 1
                                If (Me._DenyRuleIsDev(AllowRuleCounter).ServerGroupID = 0 OrElse Me._DenyRuleIsDev(AllowRuleCounter).ServerGroupID = _CurrentContextServerGroupID) AndAlso EffictivityFilterComparisonValue(Me._AllowRuleIsDev(AllowRuleCounter), filterType) = EffictivityFilterComparisonValue(Me._DenyRuleIsDev(DenyRuleCounter), filterType) Then
                                    IsDenied = True
                                End If
                            Next
                            If IsDenied = False Then
                                RuleResult.Add(Me._AllowRuleIsDev(AllowRuleCounter))
                            End If
                        End If
                    Next
                    Me._EffectiveForDevelopment = CType(RuleResult.ToArray(GetType(SecurityObjectAuthorizationForGroup)), SecurityObjectAuthorizationForGroup())
                End If
                Return Me._EffectiveForDevelopment
            End Get
        End Property
    End Class

End Namespace