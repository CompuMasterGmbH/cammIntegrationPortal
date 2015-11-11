Option Strict On
Option Explicit On 

Namespace CompuMaster.camm.WebManager

    'Legacy interface kept for compatibility reasons.
	Public Interface ICrypt
		Function CryptString(ByVal text As String) As String
		Function DeCryptString(ByVal text As String) As String
		Function CryptString(ByVal text As String, ByVal key As String) As String
		Function DeCryptString(ByVal text As String, ByVal key As String) As String
    End Interface

    Public Interface IWMPasswordTransformation
        ''' <summary>
        ''' Generates a nonce (number used once or number once) which the algorithm needs to work (properly) with, e. g. IV or Salt
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function GenerateAlgorithmNonce() As Byte()

        ''' <summary>
        ''' Encryption/Hashing with plaintext and IV/salt. 
        ''' </summary>
        ''' <param name="plaintext"></param>
        ''' <param name="noncevalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Transform(ByVal plainText As Byte(), ByVal noncevalue As Byte()) As Byte()

        ''' <summary>
        ''' Encryption/Hashing with plaintext and IV/salt, returning ciphertext as a string (usually base64 encoded)
        ''' </summary>
        ''' <param name="plainText"></param>
        ''' <param name="noncevalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function TransformString(ByVal plainText As String, ByVal noncevalue As Byte()) As String

        ''' <summary>
        ''' Encrypts/Hashes the string. Will generate IV/salt.
        ''' </summary>
        ''' <param name="plainText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function TransformString(ByVal plainText As String) As CryptoTransformationResult

    End Interface


    Public Interface IWMPasswordTransformationBackwards
        ''' <summary>
        ''' Decrypt with plaintext and IV/salt. 
        ''' </summary>
        ''' <param name="plaintext"></param>
        ''' <param name="noncevalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function TransformBack(ByVal plaintext As Byte(), ByVal noncevalue As Byte()) As Byte()

        ''' <summary>
        ''' Decrypt with plaintext and IV/salt, returning ciphertext as a string (usually base64 encoded)
        ''' </summary>
        ''' <param name="encryptedString"></param>
        ''' <param name="noncevalue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function TransformStringBack(ByVal encryptedString As String, ByVal noncevalue As Byte()) As String
    End Interface


    Public Enum PasswordAlgorithm
        EncDecModSimple = 0
        TripleDES = 1
#If NetFramework <> "1_1" Then
        AES256 = 2
        PBKDF2 = 3
#End If
    End Enum


    Public Structure CryptoTransformationResult
        ''' <summary>
        ''' Algorithm used for the transformation
        ''' </summary>
        ''' <remarks></remarks>
        Dim Algorithm As PasswordAlgorithm

        ''' <summary>
        ''' The transformed (encrypted/hashed) text
        ''' </summary>
        ''' <remarks></remarks>
        Dim TransformedText As String
        ''' <summary>
        ''' The nonce for the crypto algorithm used for the transformation. This is usually the IV or Salt
        ''' </summary>
        ''' <remarks></remarks>
        Dim Noncevalue As Byte()
    End Structure

    Public Class PasswordTransformerFactory
        ''' <summary>
        ''' Produces the proper algorithm
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ProduceCryptographicTransformer(ByVal algorithm As PasswordAlgorithm) As IWMPasswordTransformation
            Dim result As IWMPasswordTransformation
            Select Case algorithm

                Case PasswordAlgorithm.EncDecModSimple
                    result = New EncDecModSimple()
                Case PasswordAlgorithm.TripleDES
                    result = New TripleDesBase64Encryption()
#If NetFramework <> "1_1" Then
                Case PasswordAlgorithm.AES256
                    result = New AES256()
                Case PasswordAlgorithm.PBKDF2
                    result = New PBKDF2()
#End If
                Case Else
                    Throw New ArgumentException("Invalid value for parameter algorithm", "algorithm")
            End Select

            Return result
        End Function

        Public Shared Function ProduceDecryptor(ByVal algorithm As PasswordAlgorithm) As IWMPasswordTransformationBackwards
            Dim result As IWMPasswordTransformationBackwards = Nothing
            Select Case algorithm
                Case PasswordAlgorithm.EncDecModSimple
                    result = New EncDecModSimple()
                Case PasswordAlgorithm.TripleDES
                    result = New TripleDesBase64Encryption()
#If NetFrameWork <> "1_1" Then
                Case PasswordAlgorithm.AES256
                    result = New AES256()
#End If
                Case Else
                    Throw New ArgumentException("Invalid algorithm value: not supported or algorithm cannot be used for decryption", "algorithm")
            End Select
            Return result
        End Function

    End Class

    Public Class AlgorithmInfo

        Public Shared Function IsWeak(ByVal algorithm As PasswordAlgorithm) As Boolean
            Return algorithm = PasswordAlgorithm.EncDecModSimple OrElse algorithm = PasswordAlgorithm.TripleDES
        End Function

        Public Shared Function CanDecrypt(ByVal algorithm As PasswordAlgorithm) As Boolean
#If NetFramework = "1_1" Then
            Return algorithm = PasswordAlgorithm.EncDecModSimple OrElse algorithm = PasswordAlgorithm.TripleDES
#Else
            Return algorithm = PasswordAlgorithm.AES256 OrElse algorithm = PasswordAlgorithm.EncDecModSimple OrElse algorithm = PasswordAlgorithm.TripleDES
#End If
        End Function

        Public Shared Function CanDecrypt(ByVal transformer As IWMPasswordTransformation) As Boolean
            Return TypeOf transformer Is IWMPasswordTransformationBackwards
        End Function



    End Class


End Namespace