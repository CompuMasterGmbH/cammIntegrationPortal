Option Explicit On
Option Strict On

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager.Security

    ''' <summary>
    ''' GroupAuthorizationItemsByRule for usage in SecurityObjectInformation class
    ''' </summary>
    Public Class GroupAuthorizationItemsByRuleForSecurityObjects
        Inherits BaseGroupAuthorizationItemsByRule

        Friend Sub New(currentContextServerGroupID As Integer, _
                          allowRuleItemsNonDev As SecurityObjectAuthorizationForGroup(), _
                          allowRuleItemsIsDev As SecurityObjectAuthorizationForGroup(), _
                          denyRuleItemsNonDev As SecurityObjectAuthorizationForGroup(), _
                          denyRuleItemsIsDev As SecurityObjectAuthorizationForGroup())
            MyBase.New(currentContextServerGroupID, allowRuleItemsNonDev, allowRuleItemsIsDev, denyRuleItemsNonDev, denyRuleItemsIsDev)
        End Sub

        Friend ReadOnly Property EffectiveStandard As SecurityObjectAuthorizationForGroup()
            Get
                Return MyBase.EffectiveStandardInternal(EffectivityType.GroupBySecurityObject)
            End Get
        End Property

        Friend ReadOnly Property EffectiveForDevelopment As SecurityObjectAuthorizationForGroup()
            Get
                Return MyBase.EffectiveForDevelopmentInternal(EffectivityType.GroupBySecurityObject)
            End Get
        End Property

    End Class

End Namespace