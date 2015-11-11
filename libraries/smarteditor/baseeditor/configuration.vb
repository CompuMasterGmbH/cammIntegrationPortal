Option Explicit On
Option Strict On

Namespace CompuMaster.camm.SmartWebEditor
    Public Class Configuration

        Friend Sub New()
            'Creatable only assembly-internally
        End Sub

        Public ReadOnly Property ContentOfServerID() As Integer
            Get
                Return WebManager.Configuration.WebEditorContentOfServerID
            End Get
        End Property

    End Class

End Namespace