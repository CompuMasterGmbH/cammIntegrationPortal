'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs

Namespace CompuMaster.camm.WebManager.Pages.Specialized

    ''' <summary>
    '''     Send a binary file from download handler to the browser
    ''' </summary>
    ''' <remarks>
    '''     Per default, all size limits will be ignored since the CPU time or file system space has already been involved and can't be saved any more. So, since the file has already been created, it should be send now without any hassles.
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class DownloadFileByDownloadHandler
        Inherits Page

        Private _IgnoreSizeLimits As Boolean = True
        ''' <summary>
        '''     Ignore file size limits
        ''' </summary>
        ''' <value></value>
        Public Property IgnoreSizeLimits() As Boolean
            Get
                Return _IgnoreSizeLimits
            End Get
            Set(ByVal Value As Boolean)
                _IgnoreSizeLimits = Value
            End Set
        End Property

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

            If _IgnoreSizeLimits Then
                cammWebManager.DownloadHandler.MaxDownloadCollectionSize = Long.MaxValue
                cammWebManager.DownloadHandler.MaxDownloadSize = Long.MaxValue
            End If

            If HttpContext.Current.Request.QueryString("fid") <> "" Then
                'Identify required file by file ID (requires a lookup in download handler's database)
                Log.WriteEventLogTrace("CWM DH: DownloadFileByID")
                cammWebManager.DownloadHandler.DownloadFileByID(HttpContext.Current.Request.QueryString("fid"))
            ElseIf HttpContext.Current.Request.QueryString("fpath") <> "" Then
                'Identify required file by its path (only available for files in cache (because for the cache are no security demands)
                Log.WriteEventLogTrace("CWM DH: DownloadFileByPath")
                cammWebManager.DownloadHandler.DownloadFileByPath(HttpContext.Current.Request.QueryString("fpath"))
            ElseIf HttpContext.Current.Request.QueryString("cat") <> "" AndAlso HttpContext.Current.Request.QueryString("dataid") <> "" Then
                Log.WriteEventLogTrace("CWM DH: DownloadFileFromCache")
                cammWebManager.DownloadHandler.DownloadFileFromCache(HttpContext.Current.Request.QueryString("cat"), HttpContext.Current.Request.QueryString("dataid"))
            End If

        End Sub

    End Class

End Namespace