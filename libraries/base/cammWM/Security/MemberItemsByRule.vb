Option Explicit On
Option Strict On

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager.Security

    Public Class MemberItemsByRule
        Friend Sub New(allowRuleItems As UserInformation(), denyRuleItems As UserInformation())
            Me._AllowRule = allowRuleItems
            Me._DenyRule = denyRuleItems
        End Sub

        Private _AllowRule As UserInformation()
        Private _DenyRule As UserInformation()
        Private _Effective As UserInformation()

        Public ReadOnly Property AllowRule As UserInformation()
            Get
                Return Me._AllowRule
            End Get
        End Property
        Public ReadOnly Property DenyRule As UserInformation()
            Get
                Return Me._DenyRule
            End Get
        End Property
        Public ReadOnly Property Effective As UserInformation()
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
                    Me._Effective = CType(RuleResult.ToArray(GetType(UserInformation)), UserInformation())
                End If
                Return Me._Effective
            End Get
        End Property
    End Class

End Namespace