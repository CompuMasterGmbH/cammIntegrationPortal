#If NetFramework <> "1_1" Then
Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager

    Public Class AES256
        Inherits WMSymmetricCrypt

        Protected Overrides ReadOnly Property AlgorithmKey As Byte()
            Get
                Dim config As String = Configuration.AES256Key
                If Not config Is Nothing Then
                    Return System.Text.Encoding.ASCII.GetBytes(config)
                End If
                Dim result As Byte() = {161, 71, 69, 149, 9, 136, 30, 162, 181, 172, 201, 1, 219, 138, 104, 137, 186, 212, 1, 73, 112, 144, 102, 250, 117, 101, 232, 5, 117, 77, 29, 209} 'mainly for backwards compatibility, of course this is not ideal
                Return result
            End Get
        End Property

        Protected Overrides ReadOnly Property ImplementedAlgorithm As PasswordAlgorithm
            Get
                Return PasswordAlgorithm.AES256
            End Get
        End Property

        Public Sub New()
            MyBase.New(New System.Security.Cryptography.RijndaelManaged, 128, 256)
        End Sub
    End Class

End Namespace
#End If