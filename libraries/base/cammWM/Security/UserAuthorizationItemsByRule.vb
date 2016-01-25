Option Explicit On
Option Strict On

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager.Security

    ''' <summary>
    ''' UserAuthorizationItemsByRule for usage in SecurityObjectInformation class
    ''' </summary>
    Public Class UserAuthorizationItemsByRuleForSecurityObjects
        Inherits BaseUserAuthorizationItemsByRule

        Friend Sub New(currentContextServerGroupID As Integer, _
                          allowRuleItemsNonDev As SecurityObjectAuthorizationForUser(), _
                          allowRuleItemsIsDev As SecurityObjectAuthorizationForUser(), _
                          denyRuleItemsNonDev As SecurityObjectAuthorizationForUser(), _
                          denyRuleItemsIsDev As SecurityObjectAuthorizationForUser())
            MyBase.New(currentContextServerGroupID, allowRuleItemsNonDev, allowRuleItemsIsDev, denyRuleItemsNonDev, denyRuleItemsIsDev)
        End Sub

        Friend ReadOnly Property EffectiveStandard As SecurityObjectAuthorizationForUser()
            Get
                Return MyBase.EffectiveStandardInternal(EffectivityType.UserBySecurityObject)
            End Get
        End Property

        Friend ReadOnly Property EffectiveForDevelopment As SecurityObjectAuthorizationForUser()
            Get
                Return MyBase.EffectiveForDevelopmentInternal(EffectivityType.UserBySecurityObject)
            End Get
        End Property

    End Class

    ''' <summary>
    ''' UserAuthorizationItemsByRule for usage in UserInformation class
    ''' </summary>
    Public Class UserAuthorizationItemsByRuleForUsers
        Inherits BaseUserAuthorizationItemsByRule

        Friend Sub New(currentContextServerGroupID As Integer, _
                              allowRuleItemsNonDev As SecurityObjectAuthorizationForUser(), _
                              allowRuleItemsIsDev As SecurityObjectAuthorizationForUser(), _
                              denyRuleItemsNonDev As SecurityObjectAuthorizationForUser(), _
                              denyRuleItemsIsDev As SecurityObjectAuthorizationForUser())
            MyBase.New(currentContextServerGroupID, allowRuleItemsNonDev, allowRuleItemsIsDev, denyRuleItemsNonDev, denyRuleItemsIsDev)
        End Sub

        Friend ReadOnly Property EffectiveStandard As SecurityObjectAuthorizationForUser()
            Get
                Return MyBase.EffectiveStandardInternal(EffectivityType.SecurityObjectByUser)
            End Get
        End Property

        Friend ReadOnly Property EffectiveForDevelopment As SecurityObjectAuthorizationForUser()
            Get
                Return MyBase.EffectiveForDevelopmentInternal(EffectivityType.SecurityObjectByUser)
            End Get
        End Property

    End Class

    Public Class BaseUserAuthorizationItemsByRule
        Friend Sub New(currentContextServerGroupID As Integer, _
                           allowRuleItemsNonDev As SecurityObjectAuthorizationForUser(), _
                           allowRuleItemsIsDev As SecurityObjectAuthorizationForUser(), _
                           denyRuleItemsNonDev As SecurityObjectAuthorizationForUser(), _
                           denyRuleItemsIsDev As SecurityObjectAuthorizationForUser())
            Me._AllowRuleNonDev = allowRuleItemsNonDev
            Me._AllowRuleIsDev = allowRuleItemsIsDev
            Me._DenyRuleNonDev = denyRuleItemsNonDev
            Me._DenyRuleIsDev = denyRuleItemsIsDev
            Me._CurrentContextServerGroupID = currentContextServerGroupID
        End Sub

        Private _CurrentContextServerGroupID As Integer
        Private _AllowRuleIsDev As SecurityObjectAuthorizationForUser()
        Private _AllowRuleNonDev As SecurityObjectAuthorizationForUser()
        Private _DenyRuleIsDev As SecurityObjectAuthorizationForUser()
        Private _DenyRuleNonDev As SecurityObjectAuthorizationForUser()
        Private _EffectiveStandard As SecurityObjectAuthorizationForUser()
        Private _EffectiveForDevelopment As SecurityObjectAuthorizationForUser()

        Public ReadOnly Property AllowRuleStandard As SecurityObjectAuthorizationForUser()
            Get
                Return Me._AllowRuleNonDev
            End Get
        End Property
        Public ReadOnly Property AllowRuleDevelopers As SecurityObjectAuthorizationForUser()
            Get
                Return Me._AllowRuleIsDev
            End Get
        End Property
        Public ReadOnly Property DenyRuleStandard As SecurityObjectAuthorizationForUser()
            Get
                Return Me._DenyRuleNonDev
            End Get
        End Property
        Public ReadOnly Property DenyRuleDevelopers As SecurityObjectAuthorizationForUser()
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
        ''' <para>User with different authorization setups are effective authorizations by standard rules as follows (0=false, 1=true)
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
        Friend ReadOnly Property EffectiveStandardInternal(filterType As EffectivityType) As SecurityObjectAuthorizationForUser()
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
                    Me._EffectiveStandard = CType(RuleResult.ToArray(GetType(SecurityObjectAuthorizationForUser)), SecurityObjectAuthorizationForUser())
                End If
                Return Me._EffectiveStandard
            End Get
        End Property

        Friend Enum EffectivityType As Byte
            SecurityObjectByUser = 0
            UserBySecurityObject = 1
        End Enum

        Private Function EffictivityFilterComparisonValue(item As SecurityObjectAuthorizationForUser, filterType As EffectivityType) As Long
            If filterType = EffectivityType.SecurityObjectByUser Then
                Return item.SecurityObjectID
            Else
                Return item.UserID
            End If
        End Function

        ''' <summary>
        ''' Effective authorizations (in context of user's current server group environment) are the combination of the rules [{AllowIsDev} - {DenyIsDev}]
        ''' </summary>
        ''' <returns></returns>
        Friend ReadOnly Property EffectiveForDevelopmentInternal(filterType As EffectivityType) As SecurityObjectAuthorizationForUser()
            Get
                If Me._EffectiveForDevelopment Is Nothing Then
                    Dim RuleResult As New ArrayList()
                    For AllowRuleCounter As Integer = 0 To Me._AllowRuleIsDev.Length - 1
                        If Me._AllowRuleIsDev(AllowRuleCounter).ServerGroupID = 0 OrElse Me._AllowRuleIsDev(AllowRuleCounter).ServerGroupID = _CurrentContextServerGroupID Then
                            Dim IsDenied As Boolean = False
                            For DenyRuleCounter As Integer = 0 To Me._DenyRuleIsDev.Length - 1
                                If (Me._DenyRuleIsDev(DenyRuleCounter).ServerGroupID = 0 OrElse Me._DenyRuleIsDev(DenyRuleCounter).ServerGroupID = _CurrentContextServerGroupID) AndAlso EffictivityFilterComparisonValue(Me._AllowRuleIsDev(AllowRuleCounter), filterType) = EffictivityFilterComparisonValue(Me._DenyRuleIsDev(DenyRuleCounter), filterType) Then
                                    IsDenied = True
                                End If
                            Next
                            If IsDenied = False Then
                                RuleResult.Add(Me._AllowRuleIsDev(AllowRuleCounter))
                            End If
                        End If
                    Next
                    Me._EffectiveForDevelopment = CType(RuleResult.ToArray(GetType(SecurityObjectAuthorizationForUser)), SecurityObjectAuthorizationForUser())
                End If
                Return Me._EffectiveForDevelopment
            End Get
        End Property
    End Class

End Namespace