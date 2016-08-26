'Copyright 2005,2007,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.SmartWebEditor

    Public Class UploadTools

        ''' <summary>
        '''     Combine a unix path with another one
        ''' </summary>
        ''' <param name="path1">A first path</param>
        ''' <param name="path2">A second path which shall be appended to the first path</param>
        ''' <returns>The combined path</returns>
        ''' <remarks>
        ''' If path2 starts with &quot;/&quot;, it is considered as root folder and will be the only return value.
        ''' </remarks>
        Friend Shared Function CombineUnixPaths(ByVal path1 As String, ByVal path2 As String) As String
            If path1 = Nothing OrElse (path2 <> Nothing AndAlso path2.StartsWith("/")) Then
                Return path2
            ElseIf path2 = Nothing Then
                Return path1
            Else
                'path2.StartsWith("/") can never happen since it has already been evaluated above
                If path1.EndsWith("/") Then
                    Return path1 & path2
                Else
                    Return path1 & "/" & path2
                End If
            End If
        End Function

        ''' <summary>
        '''     Return the full virtual path based on the given string
        ''' </summary>
        ''' <param name="virtualPath">A path like ~/images or images/styles or /images/</param>
        Friend Shared Function FullyInterpretedVirtualPath(ByVal virtualPath As String) As String
            If virtualPath Is Nothing Then
                Throw New ArgumentNullException("virtualPath")
            End If
            If virtualPath.StartsWith("~/") Then
                virtualPath = Replace(virtualPath, "~/", "")
                Dim myApplicationPath As String = System.Web.HttpContext.Current.Request.ApplicationPath()
                If Not myApplicationPath.EndsWith("/") Then
                    myApplicationPath = myApplicationPath & "/"
                End If
                virtualPath = myApplicationPath & virtualPath
            ElseIf virtualPath.StartsWith("/") Then
                'Do nothing, because it is already the servers rootpath
            Else
                Dim currentVirtualPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
                If Not currentVirtualPath.EndsWith("/") Then
                    currentVirtualPath = currentVirtualPath.Substring(0, currentVirtualPath.LastIndexOf("/") + 1)
                End If
                virtualPath = currentVirtualPath & virtualPath
            End If
            Return virtualPath
        End Function

    End Class

End Namespace