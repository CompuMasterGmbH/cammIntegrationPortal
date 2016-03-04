'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Strict Off
Option Explicit On

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    ''' Kept around for applications that always used the old "Crypt" class which implemented EncDecMod, hence we cannot rename it. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Crypt
        Inherits EncDecModSimple

        <Obsolete("Weak crypt technology, please use provided, modern crypt technology methods")> Public Function CryptStringByKey(ByRef text As String, ByRef key As String) As String
            Return EncDecMOD.Crypt(key, text)
        End Function
        <Obsolete("Weak crypt technology, please use provided, modern crypt technology methods")> Public Function DeCryptStringByKey(ByRef text As String, ByRef key As String) As String
            Return EncDecMOD.Decrypt(key, text)
        End Function

    End Class
    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Class EncDecModSimple '<Obsolete("Only supports ANSI characters, not Unicode")> _
        Implements IWMPasswordTransformation
        Implements IWMPasswordTransformationBackwards
        Implements ICrypt

        <Obsolete("Weak crypt technology, please use provided, modern crypt technology methods")> Public Function DeCryptString(ByRef text As String) As String
            Return EncDecMOD.Decrypt("4f" & ChrW(54) & ChrW(103) & "4v" & ChrW(218) & ChrW(23) & ChrW(107) & "mh3" & ChrW(182) & "6f" & ChrW(118) & ChrW(98), text)
        End Function

        Public Function GenerateAlgorithmParam() As Byte() Implements IWMPasswordTransformation.GenerateAlgorithmNonce
            Return New Byte() {0}
        End Function

        Public Function Transform(ByVal plaintext() As Byte, param() As Byte) As Byte() Implements IWMPasswordTransformation.Transform
            Throw New NotImplementedException
        End Function

        Public Function TransformBack(plaintext() As Byte, param() As Byte) As Byte() Implements IWMPasswordTransformationBackwards.TransformBack
            Throw New NotImplementedException
        End Function

        Public Function TransformString(ByVal str As String, param() As Byte) As String Implements IWMPasswordTransformation.TransformString
            Return EncDecMOD.Crypt("4f6g4v" & ChrW(218) & ChrW(23) & "kmh3" & ChrW(182) & "6fvb", str)
        End Function

        <Obsolete("Weak crypt technology, please use provided, modern crypt technology methods")> Public Function CryptString(ByRef str As String) As String
            Return EncDecMOD.Crypt("4f6g4v" & ChrW(218) & ChrW(23) & "kmh3" & ChrW(182) & "6fvb", str)
        End Function

        ''' <summary>
        ''' Encrypts/Hashes the plaintext.
        ''' </summary>
        ''' <param name="plaintext">Plaintext to be hashed or crypted</param>
        ''' <remarks></remarks>
        Public Function TransformString(ByVal plaintext As String) As CryptoTransformationResult Implements IWMPasswordTransformation.TransformString
            Dim param As Byte() = Me.GenerateAlgorithmParam()
            Dim result As New CryptoTransformationResult
            result.TransformedText = Me.TransformString(plaintext, param)
            result.Noncevalue = param
            result.Algorithm = PasswordAlgorithm.EncDecModSimple
            Return result
        End Function

        Public Function TransformStringBack(str As String, param() As Byte) As String Implements IWMPasswordTransformationBackwards.TransformStringBack
            Return EncDecMOD.Decrypt("4f" & ChrW(54) & ChrW(103) & "4v" & ChrW(218) & ChrW(23) & ChrW(107) & "mh3" & ChrW(182) & "6f" & ChrW(118) & ChrW(98), str)
        End Function

        Private Function ICrypt_CryptString(text As String) As String Implements ICrypt.CryptString
            Return EncDecMOD.Crypt("4f6g4v" & ChrW(218) & ChrW(23) & "kmh3" & ChrW(182) & "6fvb", text)
        End Function

        Public Function ICrypt_CryptString(text As String, key As String) As String Implements ICrypt.CryptString
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            Return Me.TransformString(text, keyBytes)
        End Function

        Public Function ICrypt_DeCryptString(text As String) As String Implements ICrypt.DeCryptString
            Return EncDecMOD.Decrypt("4f" & ChrW(54) & ChrW(103) & "4v" & ChrW(218) & ChrW(23) & ChrW(107) & "mh3" & ChrW(182) & "6f" & ChrW(118) & ChrW(98), text)
        End Function

        Public Function ICrypt_DeCryptString(text As String, key As String) As String Implements ICrypt.DeCryptString
            Dim keyBytes As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(key)
            Return Me.TransformStringBack(text, keyBytes)
        End Function
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.EncDecMOD
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Encoding methods
    ''' </summary>
    ''' <remarks>
    '''     ACHTUNG! Kompatibilität nur mit ANSI gegeben, nicht mit Unicode!
    '''              Unicode-Strings werden wohl auch gecrypted, jedoch ist eine Rückkonvertierung nicht mehr funktional
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	18.04.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class EncDecMOD
        Shared x1a0(9) As Integer
        Shared cle(17) As Integer
        Shared x1a2 As Integer

        Shared ax, inter, res, bx As Integer
        Shared si, cx, dx, tmp As Integer
        Shared i As Integer
        Shared c As Byte


        Private Shared Sub Assemble()

            x1a0(0) = ((cle(1) * 256) + cle(2)) Mod 65536
            Code()
            inter = res

            x1a0(1) = x1a0(0) Xor ((cle(3) * 256) + cle(4))
            Code()
            inter = inter Xor res


            x1a0(2) = x1a0(1) Xor ((cle(5) * 256) + cle(6))
            Code()
            inter = inter Xor res

            x1a0(3) = x1a0(2) Xor ((cle(7) * 256) + cle(8))
            Code()
            inter = inter Xor res

            x1a0(4) = x1a0(3) Xor ((cle(9) * 256) + cle(10))
            Code()
            inter = inter Xor res

            x1a0(5) = x1a0(4) Xor ((cle(11) * 256) + cle(12))
            Code()
            inter = inter Xor res

            x1a0(6) = x1a0(5) Xor ((cle(13) * 256) + cle(14))
            Code()
            inter = inter Xor res

            x1a0(7) = x1a0(6) Xor ((cle(15) * 256) + cle(16))
            Code()
            inter = inter Xor res

            i = 0

        End Sub

        Private Shared Sub Code()
            dx = (x1a2 + i) Mod 65536
            ax = x1a0(i)
            cx = &H15AS
            bx = &H4E35S

            tmp = ax
            ax = si
            si = tmp

            tmp = ax
            ax = dx
            dx = tmp

            If (ax <> 0) Then
                ax = (ax * bx) Mod 65536
            End If

            tmp = ax
            ax = cx
            cx = tmp

            If (ax <> 0) Then
                ax = (ax * si) Mod 65536
                cx = (ax + cx) Mod 65536
            End If

            tmp = ax
            ax = si
            si = tmp
            ax = (ax * bx) Mod 65536
            dx = (cx + dx) Mod 65536

            ax = ax + 1

            x1a2 = dx
            x1a0(i) = ax

            res = ax Xor dx
            i = i + 1

        End Sub

        Public Shared Function Crypt(ByVal Key As String, ByVal inp As String) As String
            Dim currentCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture
            System.Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.CreateSpecificCulture("en-US")
            System.Threading.Thread.CurrentThread.CurrentUICulture = Globalization.CultureInfo.CreateSpecificCulture("en-US")
            Dim e As Integer
            Dim d As Integer
            Dim compte As Integer
            Dim cfd As Integer
            Dim cfc As Integer
            Dim lngchamp1 As Integer
            Dim champ1 As String
            Dim fois As Integer
            Crypt = ""
            si = 0
            x1a2 = 0
            i = 0

            For fois = 1 To 16
                cle(fois) = 0
            Next fois

            champ1 = Key
            lngchamp1 = Len(champ1)

            For fois = 1 To lngchamp1
                cle(fois) = Asc(Mid(champ1, fois, 1))
            Next fois

            champ1 = inp
            lngchamp1 = Len(champ1)
            For fois = 1 To lngchamp1
                c = Asc(Mid(champ1, fois, 1))

                Assemble()

                cfc = (((inter / 256) * 256) - (inter Mod 256)) / 256
                cfd = inter Mod 256

                For compte = 1 To 16

                    cle(compte) = cle(compte) Xor c

                Next compte

                c = c Xor (cfc Xor cfd)

                d = (((c / 16) * 16) - (c Mod 16)) / 16
                e = c Mod 16

                Crypt = Crypt & ChrW(&H61S + d)              ' d+&h61 give one letter range from a to p for the 4 high bits of c
                Crypt = Crypt & ChrW(&H61S + e)              ' e+&h61 give one letter range from a to p for the 4 low bits of c


            Next fois

            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture

        End Function

        Public Shared Function Decrypt(ByVal Key As String, ByVal inp As String) As String
            Dim currentCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture
            System.Threading.Thread.CurrentThread.CurrentCulture = Globalization.CultureInfo.CreateSpecificCulture("en-US")
            Dim compte As Integer
            Dim cfd As Integer
            Dim cfc As Integer
            Dim e As Integer
            Dim d As Integer
            Dim lngchamp1 As Integer
            Dim champ1 As String
            Dim fois As Integer
            Decrypt = ""
            si = 0
            x1a2 = 0
            i = 0

            For fois = 1 To 16
                cle(fois) = 0
            Next fois

            champ1 = Key
            lngchamp1 = Len(champ1)

            For fois = 1 To lngchamp1
                cle(fois) = Asc(Mid(champ1, fois, 1))
            Next fois

            champ1 = inp
            lngchamp1 = Len(champ1)

            For fois = 1 To lngchamp1

                d = Asc(Mid(champ1, fois, 1))
                If (d - &H61S) >= 0 Then
                    d = d - &H61S       ' to transform the letter to the 4 high bits of c
                    If (d >= 0) And (d <= 15) Then
                        d = d * 16
                    End If
                End If
                If (fois <> lngchamp1) Then
                    fois = fois + 1
                End If
                e = Asc(Mid(champ1, fois, 1))
                If (e - &H61S) >= 0 Then
                    e = e - &H61S       ' to transform the letter to the 4 low bits of c
                    If (e >= 0) And (e <= 15) Then
                        c = d + e
                    End If
                End If

                Assemble()

                cfc = (((inter / 256) * 256) - (inter Mod 256)) / 256
                cfd = inter Mod 256

                c = c Xor (cfc Xor cfd)

                For compte = 1 To 16

                    cle(compte) = cle(compte) Xor c

                Next compte

                Decrypt = Decrypt & ChrW(c)

            Next fois

            System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture

        End Function
    End Class
End Namespace