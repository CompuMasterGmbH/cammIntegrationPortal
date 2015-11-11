Option Strict Off
Option Explicit On

Namespace CompuMaster.camm.WebManager

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use TripleDesBase64Encryption")> _
    Public Class TripleDesEncryption
        Inherits TripleDesEncryptionBase

    End Class

    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
    Public Class TripleDesEncryptionBase
        Implements ICrypt

        Private DefaultKey() As Byte = {63, 249, 187, 13, 196, 160, 65, 7, 8, 47, 79, 18, 89, 71, 249, 173, 246, 231, 56, 28, 228, 136, 210, 117}
        Private DefaultInitializationVector() As Byte = {31, 201, 99, 7, 8, 47, 79, 18, 89, 92, 147, 212, 181, 145, 231, 254, 222}

        Function TripleDESEncrypt(ByVal inputInBytes() As Byte, ByVal key As Byte(), ByVal iv As Byte()) As Byte()
            ' Create a new TripleDES service provider 
            Dim tdesProvider As System.Security.Cryptography.TripleDESCryptoServiceProvider = New System.Security.Cryptography.TripleDESCryptoServiceProvider
            ' The ICryptTransform interface uses the TripleDES crypt provider along with encryption key and init vector information 
            Dim cryptoTransform As System.Security.Cryptography.ICryptoTransform = tdesProvider.CreateEncryptor(key, iv)
            ' All cryptographic functions need a stream to output the encrypted information. Here we declare a memory stream for this purpose. 
            Dim encryptedStream As System.IO.MemoryStream = New System.IO.MemoryStream
            Dim cryptStream As System.Security.Cryptography.CryptoStream = New System.Security.Cryptography.CryptoStream(encryptedStream, _
            cryptoTransform, System.Security.Cryptography.CryptoStreamMode.Write)
            ' Write the encrypted information to the stream. Flush the information when done to ensure everything is out of the buffer. 
            cryptStream.Write(inputInBytes, 0, inputInBytes.Length)
            cryptStream.FlushFinalBlock()
            encryptedStream.Position = 0
            ' Read the stream back into a Byte array and return it to the calling method
            Dim result(encryptedStream.Length - 1) As Byte
            encryptedStream.Read(result, 0, encryptedStream.Length)
            cryptStream.Close()

            Return result
        End Function

        Function TripleDESDecrypt(ByVal inputInBytes() As Byte, ByVal key As Byte(), ByVal iv As Byte()) As Byte()
            Dim tdesProvider As System.Security.Cryptography.TripleDESCryptoServiceProvider = New System.Security.Cryptography.TripleDESCryptoServiceProvider
            ' As before we must provide the encryption/decryption key along with  the init vector. 
            Dim cryptoTransform As System.Security.Cryptography.ICryptoTransform = tdesProvider.CreateDecryptor(key, iv)
            ' Provide a memory stream to decrypt information into 
            Dim decryptedStream As System.IO.MemoryStream = New System.IO.MemoryStream
            Dim cryptStream As System.Security.Cryptography.CryptoStream = New System.Security.Cryptography.CryptoStream(decryptedStream, _
            cryptoTransform, System.Security.Cryptography.CryptoStreamMode.Write)
            cryptStream.Write(inputInBytes, 0, inputInBytes.Length)
            cryptStream.FlushFinalBlock()
            decryptedStream.Position = 0
            'ead the memory stream and convert it back into a string 
            Dim result(decryptedStream.Length - 1) As Byte
            decryptedStream.Read(result, 0, decryptedStream.Length)
            cryptStream.Close()
            Return result
        End Function

#Region "Implementation of interfaces"
#Region "Unicode Encryption"
        Public Overloads Function CryptString(ByVal text As String) As String Implements ICrypt.CryptString
            Return System.Text.Encoding.Unicode.GetString(TripleDESEncrypt(System.Text.Encoding.Unicode.GetBytes(text), DefaultKey, DefaultInitializationVector))
        End Function

        Public Overloads Function CryptString(ByVal text As String, ByVal key As String) As String Implements ICrypt.CryptString
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            Try
                Return System.Text.Encoding.Unicode.GetString(TripleDESEncrypt(System.Text.UnicodeEncoding.Unicode.GetBytes(text), keyBytes, DefaultInitializationVector))
            Catch ex As Exception
                'Prepare a valid key based on the default key and bytes of the given key
                Dim safeKey As Byte() = DefaultKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return System.Text.Encoding.Unicode.GetString(TripleDESEncrypt(System.Text.UnicodeEncoding.Unicode.GetBytes(text), safeKey, DefaultInitializationVector))
            End Try
        End Function

        Public Overloads Function DeCryptString(ByVal text As String) As String Implements ICrypt.DeCryptString
            Return System.Text.Encoding.Unicode.GetString(TripleDESDecrypt(System.Text.Encoding.Unicode.GetBytes(text), DefaultKey, DefaultInitializationVector))
        End Function

        Public Overloads Function DeCryptString(ByVal text As String, ByVal key As String) As String Implements ICrypt.DeCryptString
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            Try
                Return System.Text.UnicodeEncoding.Unicode.GetString(TripleDESDecrypt(System.Text.UnicodeEncoding.Unicode.GetBytes(text), keyBytes, DefaultInitializationVector))
            Catch ex As Exception
                'Prepare a valid key based on the default key and bytes of the given key
                Dim safeKey() As Byte = DefaultKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return System.Text.UnicodeEncoding.Unicode.GetString(TripleDESDecrypt(System.Text.UnicodeEncoding.Unicode.GetBytes(text), safeKey, DefaultInitializationVector))
            End Try
        End Function
#End Region
#End Region

        Public Function GetValidEncryptionKeyString(ByVal key As String) As String
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            If keyBytes.Length > DefaultKey.Length Then
                Dim safeKey As Byte() = DefaultKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return System.Text.Encoding.Unicode.GetString(safeKey)
            Else
                Return key
            End If
        End Function

        Public Function GetValidEncryptionKeyBytes(ByVal key As String) As Byte()
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            If keyBytes.Length > DefaultKey.Length Then
                Dim safeKey As Byte() = DefaultKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return safeKey
            Else
                Return keyBytes
            End If
        End Function
    End Class

    Public Class TripleDesBase64Encryption
        Inherits WMSymmetricCrypt
        Implements ICrypt  'Retained for backwards compatiblity

        Protected Overrides ReadOnly Property AlgorithmKey As Byte()
            Get
                Dim result As Byte() = {63, 249, 187, 13, 196, 160, 65, 7, 8, 47, 79, 18, 89, 71, 249, 173, 246, 231, 56, 28, 228, 136, 210, 117}
                Return result
            End Get
        End Property

        Protected Overrides ReadOnly Property ImplementedAlgorithm As PasswordAlgorithm
            Get
                Return PasswordAlgorithm.TripleDES
            End Get
        End Property

        Private DefaultInitializationVector() As Byte = {31, 201, 99, 7, 8, 47, 79, 18}

        Public Sub New()
            MyBase.New(New System.Security.Cryptography.TripleDESCryptoServiceProvider, 64, 192)
        End Sub

        Function TripleDESEncrypt(ByVal inputInBytes() As Byte, ByVal key As Byte(), ByVal iv As Byte()) As Byte()
            Return Me.EncryptBytes(inputInBytes, key, iv)
        End Function

        Function TripleDESDecrypt(ByVal inputInBytes() As Byte, ByVal key As Byte(), ByVal iv As Byte()) As Byte()
            Return Me.DecryptBytes(inputInBytes, key, iv)
        End Function

#Region "Implementation of interfaces"

#Region "Base64 Encryption"
        Public Overloads Function CryptBase64String(ByVal text As String) As String Implements ICrypt.CryptString
            Return Me.TransformString(text, DefaultInitializationVector)
        End Function

        'TODO: replace function body with base class
        Public Overloads Function CryptBase64String(ByVal text As String, ByVal key As String) As String Implements ICrypt.CryptString
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            Try
                Return Convert.ToBase64String(TripleDESEncrypt(System.Text.Encoding.Unicode.GetBytes(text), keyBytes, DefaultInitializationVector))
            Catch ex As Exception
                'Prepare a valid key based on the default key and bytes of the given key
                Dim safeKey As Byte() = AlgorithmKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return Convert.ToBase64String(TripleDESEncrypt(System.Text.Encoding.Unicode.GetBytes(text), safeKey, DefaultInitializationVector))
            End Try
        End Function

        Public Overloads Function DeCryptBase64String(ByVal text As String) As String Implements ICrypt.DeCryptString
            Return Me.TransformStringBack(text, DefaultInitializationVector)
        End Function

        'TODO: replace function body with base class
        Public Overloads Function DeCryptBase64String(ByVal text As String, ByVal key As String) As String Implements ICrypt.DeCryptString
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            Try
                Return System.Text.Encoding.Unicode.GetString(TripleDESDecrypt(Convert.FromBase64String(text), keyBytes, DefaultInitializationVector))
            Catch ex As Exception
                'Prepare a valid key based on the default key and bytes of the given key
                Dim safeKey() As Byte = AlgorithmKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return System.Text.Encoding.Unicode.GetString(TripleDESDecrypt(Convert.FromBase64String(text), safeKey, DefaultInitializationVector))
            End Try
        End Function
#End Region

#End Region

        'TODO: can we drop these functions? 
        Public Function GetValidEncryptionKeyString(ByVal key As String) As String
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            If keyBytes.Length > AlgorithmKey.Length Then
                Dim safeKey As Byte() = AlgorithmKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return System.Text.Encoding.Unicode.GetString(safeKey)
            Else
                Return key
            End If
        End Function

        Public Function GetValidEncryptionKeyBytes(ByVal key As String) As Byte()
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            If keyBytes.Length > AlgorithmKey.Length Then
                Dim safeKey As Byte() = AlgorithmKey.Clone
                For MyCounter As Integer = 0 To System.Math.Min(safeKey.Length, keyBytes.Length) - 1
                    safeKey(MyCounter) = keyBytes(MyCounter)
                Next
                Return safeKey
            Else
                Return keyBytes
            End If
        End Function
    End Class
End Namespace