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