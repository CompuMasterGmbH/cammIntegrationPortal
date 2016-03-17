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

Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager

    Public MustInherit Class WMSymmetricCrypt
        Inherits SymmetricCrypt
        Implements IWMPasswordTransformation
        Implements IWMPasswordTransformationBackwards

        ''' <summary>
        ''' Key for the symmetric algorithm
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Varies for different algorithms and must be overriden thus</remarks>
        Protected MustOverride ReadOnly Property AlgorithmKey As Byte()


        ''' <summary>
        ''' Specifies which algorithm the subclass implements
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride ReadOnly Property ImplementedAlgorithm As PasswordAlgorithm


        Public Sub New(ByVal algo As System.Security.Cryptography.SymmetricAlgorithm, blockSize As Integer, ByVal keySize As Integer)
            MyBase.New(algo, blockSize, keySize)
        End Sub


        Public Function GenerateAlgorithmNonce() As Byte() Implements IWMPasswordTransformation.GenerateAlgorithmNonce
            Return MyBase.CreateIV()
        End Function

        Public Function Transform(ByVal plaintext As Byte(), ByVal iv As Byte()) As Byte() Implements IWMPasswordTransformation.Transform
            Return MyBase.EncryptBytes(plaintext, Me.AlgorithmKey, iv)
        End Function

        Public Function TransformBack(plaintext() As Byte, nonceValue() As Byte) As Byte() Implements IWMPasswordTransformationBackwards.TransformBack
            Return MyBase.DecryptBytes(plaintext, Me.AlgorithmKey, nonceValue)
        End Function

        ''' <summary>
        ''' Encrypts/Hashes the plaintext.
        ''' </summary>
        ''' <param name="plaintext">Plaintext to be hashed or crypted</param>
        ''' <remarks></remarks>
        Public Function TransformString(ByVal plaintext As String) As CryptoTransformationResult Implements IWMPasswordTransformation.TransformString
            Dim noncevalue As Byte() = Me.GenerateAlgorithmNonce()
            Dim result As New CryptoTransformationResult
            result.TransformedText = Me.TransformString(plaintext, noncevalue)
            result.Noncevalue = noncevalue
            result.Algorithm = Me.ImplementedAlgorithm
            Return result
        End Function

        Public Function TransformString(ByVal plainPassword As String, ByVal iv As Byte()) As String Implements IWMPasswordTransformation.TransformString
            If plainPassword = Nothing Then
                Return ""
            Else
                Dim password As Byte() = System.Text.Encoding.Unicode.GetBytes(plainPassword)
                Dim result As Byte() = Transform(password, iv)
                Return System.Convert.ToBase64String(result)
            End If
        End Function

        Public Function TransformStringBack(base64text As String, nonceValue() As Byte) As String Implements IWMPasswordTransformationBackwards.TransformStringBack
            Dim encryptedBytes As Byte() = System.Convert.FromBase64String(base64text)
            Dim result As Byte() = TransformBack(encryptedBytes, nonceValue)
            Return System.Text.Encoding.Unicode.GetString(result)
        End Function

    End Class




    Public Class SymmetricCrypt
        Private algorithm As System.Security.Cryptography.SymmetricAlgorithm


        Public Sub New(ByVal algo As System.Security.Cryptography.SymmetricAlgorithm, blockSize As Integer, ByVal keySize As Integer)
            algorithm = algo
            algorithm.BlockSize = blockSize
            algorithm.KeySize = keySize
            algorithm.Mode = System.Security.Cryptography.CipherMode.CBC
            algorithm.Padding = System.Security.Cryptography.PaddingMode.PKCS7
        End Sub

        ''' <summary>
        ''' Throws an exception if key length is not suitable for this algorithm
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Private Sub VerifyKeyLength(ByVal key As Byte())
            If key.Length * 8 <> algorithm.KeySize Then
                Throw New System.Security.Cryptography.CryptographicException("supplied key does not match keysize of algorithm")
            End If
        End Sub

        ''' <summary>
        ''' Throws an exception if iv length is not suitable for this algorithm
        ''' </summary>
        ''' <param name="iv"></param>
        ''' <remarks></remarks>
        Private Sub VerifyIVLength(ByVal iv As Byte())
            If iv.Length <> Me.algorithm.IV.Length Then
                Throw New System.Security.Cryptography.CryptographicException("Supplied IV has wrong length. Length passed:" & iv.Length & ", Required length by algorithm: " & Me.algorithm.IV.Length)
            End If
        End Sub

        ''' <summary>
        ''' Generates and returns an IV suitable for the algorithm
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateIV() As Byte()
            algorithm.GenerateIV()
            Return algorithm.IV
        End Function

        ''' <summary>
        ''' Performs the cryptographic transformation (encryption/decryption)
        ''' </summary>
        ''' <param name="transformer">Transformer</param>
        ''' <param name="data">plaintext data, or encrypted data</param>
        ''' <returns>Transfromed data</returns>
        ''' <remarks></remarks>
        Private Function Transform(ByVal transformer As System.Security.Cryptography.ICryptoTransform, ByVal data As Byte()) As Byte()
            Dim memstream As New System.IO.MemoryStream()
            Dim cryptoStream As New System.Security.Cryptography.CryptoStream(memstream, transformer, System.Security.Cryptography.CryptoStreamMode.Write)
            cryptoStream.Write(data, 0, data.Length)
            cryptoStream.FlushFinalBlock()

            Dim result As Byte() = memstream.ToArray()

            memstream.Close()
            cryptoStream.Close()

            Return result
        End Function

        ''' <summary>
        ''' Encrypts the given plaintext with the given key
        ''' </summary>
        ''' <param name="plaintext">Plaintext</param>
        ''' <param name="key">Key suitable for the the key size of chosen algorithm</param>
        ''' <param name="iv">Initialization vector to be used in the encryption</param>
        ''' <returns>Encrypted text</returns>
        ''' <remarks></remarks>
        Public Function EncryptBytes(ByVal plaintext As Byte(), ByVal key As Byte(), ByVal iv As Byte()) As Byte()
            VerifyKeyLength(key)
            VerifyIVLength(iv)

            Dim transformer As System.Security.Cryptography.ICryptoTransform = algorithm.CreateEncryptor(key, iv)
            Dim encryptedBytes As Byte() = Transform(transformer, plaintext)
            Return encryptedBytes
        End Function

        ''' <summary>
        ''' Decrypts the given plaintext with the given key
        ''' </summary>
        ''' <param name="cryptedText">Plaintext</param>
        ''' <param name="key">Key suitable for the the key size of chosen algorithm</param>
        ''' <param name="iv">Initialization vector to be used in the encryption</param>
        ''' <returns>Decrypted textV</returns>
        ''' <remarks></remarks>
        Public Function DecryptBytes(ByVal cryptedText As Byte(), ByVal key As Byte(), ByVal iv As Byte()) As Byte()
            VerifyKeyLength(key)
            VerifyIVLength(iv)

            Dim transformer As System.Security.Cryptography.ICryptoTransform = algorithm.CreateDecryptor(key, iv)
            Dim decryptedBytes As Byte() = Transform(transformer, cryptedText)
            Return decryptedBytes
        End Function
    End Class

End Namespace