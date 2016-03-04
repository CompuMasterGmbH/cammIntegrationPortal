'Copyright 2014-2016 CompuMaster GmbH, http://www.compumaster.de
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