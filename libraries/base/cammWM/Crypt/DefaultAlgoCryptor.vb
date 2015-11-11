Option Strict On
Option Explicit On


Namespace CompuMaster.camm.WebManager
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DefaultAlgoCryptor

        Private webmanager As WMSystem
        Private Algorithm As PasswordAlgorithm
        Private currentTransformer As IWMPasswordTransformation



        Public Sub New(ByVal wm As WMSystem)
            Me.webmanager = wm
            Me.Algorithm = Me.webmanager.System_GetDefaultPasswordAlgorithm()
            Me.currentTransformer = PasswordTransformerFactory.ProduceCryptographicTransformer(Me.Algorithm)
        End Sub

        ''' <summary>
        ''' Encrypts/Hashes the plaintext.
        ''' </summary>
        ''' <param name="plaintext">Plaintext to be hashed or crypted</param>
        ''' <remarks></remarks>
        Public Function TransformPlaintext(ByVal plaintext As String) As CryptoTransformationResult
            Dim param As Byte() = Me.currentTransformer.GenerateAlgorithmNonce()
            Dim result As New CryptoTransformationResult
            result.TransformedText = Me.currentTransformer.TransformString(plaintext, param)
            result.Noncevalue = param
            result.Algorithm = Me.Algorithm
            Return result
        End Function

    End Class

End Namespace