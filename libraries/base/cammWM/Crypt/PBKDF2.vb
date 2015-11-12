﻿#If NetFramework <> "1_1" Then
Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager
    Public Class PBKDF2
        Implements IWMPasswordTransformation

        Const SubKeySize As Integer = 32 '256 bit
        Const SaltSize As Integer = 16 ' 128 bit...

        Private ReadOnly Property IterationCount As Integer
            Get
                Return CompuMaster.camm.WebManager.WMSystem.Configuration.PBKDF2Rounds
            End Get
        End Property

        Private Function GenerateSalt() As Byte()
            Dim result As Byte() = New Byte(SaltSize - 1) {}
            Dim rng As New System.Security.Cryptography.RNGCryptoServiceProvider()
            rng.GetBytes(result)
            Return result
        End Function


        ''' <summary>
        ''' Generates the nonce. Format: [rounds (PBKDF2 iteraetions) as ascii bytes]-[salt bytes]
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Not exactly only the nonce in this case, but we must prepend it with the nr. of rounds.</remarks>
        Public Function GenerateAlgorithmNonce() As Byte() Implements IWMPasswordTransformation.GenerateAlgorithmNonce
            Dim salt As Byte() = GenerateSalt()
            Dim rounds As Byte() = System.Text.Encoding.ASCII.GetBytes(IterationCount.ToString() + "-")

            Dim result As Byte() = New Byte(salt.Length + rounds.Length - 1) {}
            Array.Copy(rounds, result, rounds.Length)
            Array.Copy(salt, 0, result, rounds.Length, salt.Length)
            Return result
        End Function

        ''' <summary>
        ''' Verifies that we got a correct parameter for our operation. Must have the format: [rounds (PBKDF2 iteraetions) as ascii bytes]-[salt bytes]
        ''' </summary>
        ''' <param name="roundsSaltParam"></param>
        ''' <remarks></remarks>
        Private Sub VerifyRoundsSaltParam(ByVal roundsSaltParam As Byte())
            Dim foundDash As Boolean = False
            Dim iterationCountEndIndex As Integer = 0
            For Each c As Byte In roundsSaltParam
                Dim character As Char = Convert.ToChar(c)
                If character = "-" Then
                    iterationCountEndIndex -= 1 'we found the dash which seperates the nr of iteration and the actual nonce/salt. We don't need it, so substract 1.
                    foundDash = True
                    Exit For
                End If
                iterationCountEndIndex += 1
            Next
            If iterationCountEndIndex < 0 OrElse Not foundDash Then
                Throw New ArgumentException("Supplied param is invalid. Must have format: [rounds (PBKDF2 iterations) as ascii bytes]-[salt bytes]")
            End If

        End Sub

        ''' <summary>
        ''' Extracts the iteration count
        ''' </summary>
        ''' <param name="roundsSaltParam">[rounds (PBKDF2 iteraetions) as ascii bytes]-[salt bytes]</param>
        ''' <param name="dashPosition">0-based index of the dash which seperates rounds and salt</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ExtractRoundsFromParam(ByVal roundsSaltParam As Byte(), ByVal dashPosition As Integer) As Integer
            Dim roundsByte As Byte() = New Byte(dashPosition - 1) {}
            Array.Copy(roundsSaltParam, roundsByte, roundsByte.Length)
            Dim roundsString As String = System.Text.Encoding.ASCII.GetString(roundsByte)

            Return Convert.ToInt32(roundsString)
        End Function

        ''' <summary>
        ''' Extracts the salt
        ''' </summary>
        ''' <param name="roundsSaltParam">[rounds (PBKDF2 iteraetions) as ascii bytes]-[salt bytes]</param>
        ''' <param name="dashPosition">0-based index of the dash which seperates rounds and salt</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ExtractSaltFromParam(ByVal roundsSaltParam As Byte(), ByVal dashPosition As Integer) As Byte()
            Dim salt As Byte() = New Byte(SaltSize - 1) {}
            Array.Copy(roundsSaltParam, dashPosition + 1, salt, 0, SaltSize)
            Return salt
        End Function



        Public Function Transform(plaintext() As Byte, noncevalue() As Byte) As Byte() Implements IWMPasswordTransformation.Transform
            VerifyRoundsSaltParam(noncevalue)

            Dim dashPosition As Integer = -1
            For i As Integer = 0 To noncevalue.Length - 1
                Dim c As Char = Convert.ToChar(noncevalue(i))
                If c = "-"c Then
                    dashPosition = i
                    Exit For
                End If
            Next
          
            Dim rounds As Integer = ExtractRoundsFromParam(noncevalue, dashPosition)
            Dim salt As Byte() = ExtractSaltFromParam(noncevalue, dashPosition)

            Dim deriver As New System.Security.Cryptography.Rfc2898DeriveBytes(plaintext, salt, rounds)
            Return deriver.GetBytes(SubKeySize)
        End Function

        Public Function TransformString(str As String, noncevalue() As Byte) As String Implements IWMPasswordTransformation.TransformString
            Dim bytes As Byte() = System.Text.Encoding.UTF8.GetBytes(str)
            Return Convert.ToBase64String(Transform(bytes, noncevalue))
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
            result.Algorithm = PasswordAlgorithm.PBKDF2
            Return result
        End Function


    End Class

End Namespace
#End If