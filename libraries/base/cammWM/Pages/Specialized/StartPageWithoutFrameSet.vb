'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie k�nnen es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder sp�teren ver�ffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es n�tzlich sein wird, aber OHNE JEDE GEW�HRLEISTUNG, bereitgestellt; sogar ohne die implizite Gew�hrleistung der MARKTF�HIGKEIT oder EIGNUNG F�R EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License f�r weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden f�r Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_ErrorPageForwarderIDs

Namespace CompuMaster.camm.WebManager.Pages.Specialized

    ''' <summary>
    '''     Handles referrer requests by session content or query string arguments
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class StartPageWithoutFrameSet
        Inherits Page

        Protected FrameContentURL As String = ""

        Sub PageInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

            'Referer
            If Request.QueryString("forceref") <> "" AndAlso Request.QueryString("forceref").IndexOf("/"c) = 0 Then
                FrameContentURL = Request.QueryString("forceref")
            ElseIf LCase(CType(Session("System_Referer"), String)) = LCase(Request.ServerVariables("SCRIPT_NAME")) Then
                'Falls Referer = dieses FrameSet
                FrameContentURL = Response.ApplyAppPathModifier("/sysdata/frames/frame_main.aspx?Lang=" & cammWebManager.UI.MarketID)
            ElseIf CType(Session("System_Referer"), String) <> "" Then
                'Falls anderer Referer
                FrameContentURL = CType(Session("System_Referer"), String)
            ElseIf Request.QueryString("referer") <> "" Then 'used in old defaults of account_register.aspx for launching userjustcreated.aspx
                'Falls anderer Referer
                FrameContentURL = Request.QueryString("referer")
            ElseIf Request.QueryString("ref") <> "" Then
                'Falls anderer Referer
                FrameContentURL = Request.QueryString("ref")
            Else
                'Wenn �berhaupt kein Referer
                FrameContentURL = ""
            End If
            Session("System_Referer") = ""
            If cammWebManager.IsLoggedOn Then
                cammWebManager.System_SetSessionValue("System_Referer", "")
            End If

            'Redirect to destination page if required (redirect with 301 permenantly not allowed because client must redirect sometimes to another location as it already remembers by a previous request, so the client is not allowed to cache the redirection)
            If FrameContentURL <> Nothing Then
                If cammWebManager Is Nothing Then
                    Utils.RedirectTemporary(HttpContext.Current, FrameContentURL)
                Else
                    cammWebManager.RedirectTo(FrameContentURL)
                End If
            End If

        End Sub
    End Class

End Namespace