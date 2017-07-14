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

Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager.Pages.Specialized

    Public Class WebCron
        Inherits CompuMaster.camm.WebManager.Pages.Page

        ''' <summary>
        ''' The HTTP response status code will be 500 if the webcron request fails for some reason if set to False (default), but will always be 200 OK if set to True
        ''' </summary>
        ''' <returns></returns>
        Public Property HttpStatusCodeAlways200OK As Boolean = False

        ''' <summary>
        ''' In case of errors, the exception details will be logged into the database (if set to True) or will just be displayed as message in response body (if set to false)
        ''' </summary>
        ''' <returns></returns>
        Public Property LogErrorsIntoDatabase As Boolean = False

        Private Sub WebCron_Load(sender As Object, e As EventArgs) Handles Me.Load
            Try
                CompuMaster.camm.WebManager.WebServices.CoreWebCronJobRunner.ExecuteNextWebCronJob(Me.Context, Me.cammWebManager)
                Response.Clear()
                Response.Write("200 OK")
            Catch ex As Exception
                Response.Clear()
                If Me.HttpStatusCodeAlways200OK = False Then
                    Response.StatusCode = 500
                End If
                If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Response.Write("500 Internal Error: " & ex.ToString)
                Else
                    Response.Write("500 Internal Error: " & ex.Message)
                End If
                If Me.LogErrorsIntoDatabase Then
                    Try
                        Me.cammWebManager.Log.Exception(ex, False)
                    Catch
                        'ignore exceptions here
                    End Try
                End If
            End Try
            Response.End()
        End Sub

    End Class

End Namespace