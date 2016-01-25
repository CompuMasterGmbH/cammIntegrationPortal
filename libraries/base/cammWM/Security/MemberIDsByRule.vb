Option Explicit On
Option Strict On

Namespace CompuMaster.camm.WebManager.Security

    Public Class MemberIDsByRule
        Friend Sub New(allowRuleItems As Long(), denyRuleItems As Long())
            Me._AllowRule = allowRuleItems
            Me._DenyRule = denyRuleItems
        End Sub

        Private _AllowRule As Long()
        Private _DenyRule As Long()
        Private _Effective As Long()

        Public ReadOnly Property AllowRule As Long()
            Get
                Return Me._AllowRule
            End Get
        End Property
        Public ReadOnly Property DenyRule As Long()
            Get
                Return Me._DenyRule
            End Get
        End Property
        Public ReadOnly Property Effective As Long()
            Get
                If Me._Effective Is Nothing Then
                    Dim RuleResult As New ArrayList()
                    For AllowRuleCounter As Integer = 0 To Me._AllowRule.Length - 1
                        Dim IsDenied As Boolean = False
                        For DenyRuleCounter As Integer = 0 To Me._DenyRule.Length - 1
                            If Me._AllowRule(AllowRuleCounter) = Me._DenyRule(DenyRuleCounter) Then
                                IsDenied = True
                                Exit For
                            End If
                        Next
                        If IsDenied = False Then
                            RuleResult.Add(Me._AllowRule(AllowRuleCounter))
                        End If
                    Next
                    Me._Effective = CType(RuleResult.ToArray(GetType(Long)), Long())
                End If
                Return Me._Effective
            End Get
        End Property
    End Class

End Namespace