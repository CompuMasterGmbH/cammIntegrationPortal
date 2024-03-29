'Copyright 2008-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Modules.Redirector.Pages

    ''' <summary>
    ''' Redirect to an URL which is referenced by the R-parameter in the querystring
    ''' </summary>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class Redirector
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Protected Overridable Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            'RedirectID
            Dim UseTemporaryRedirection As Boolean = False
            Dim MyRedirect2Addr As String
            Dim MyRedirect2ID As Long
            If Request.QueryString("R") <> Nothing Then
                Try
                    MyRedirect2ID = CType(Request.QueryString("R"), Long)
                Catch
                    MyRedirect2ID = 0 'Unknown ID leads to NULL result
                    UseTemporaryRedirection = True
                End Try
            Else
                MyRedirect2ID = 0 'Unknown ID leads to NULL result
                UseTemporaryRedirection = True
            End If
            Try
                MyRedirect2Addr = Redirector_GetLink(MyRedirect2ID)
                If MyRedirect2Addr = Nothing Then
                    MyRedirect2Addr = "/"
                    UseTemporaryRedirection = True
                End If
            Catch ex As Exception
                ReportException(ex)
                UseTemporaryRedirection = True
                MyRedirect2Addr = "/"
            End Try
            Dim RedirQueryString As String = Utils.QueryStringWithoutSpecifiedParameters(New String() {"R"})
            If RedirQueryString <> "" Then
                If InStr(MyRedirect2Addr, "?") <> 0 Then
                    'Attach to URL with already present query string
                    MyRedirect2Addr &= "&" & RedirQueryString
                Else
                    'Attach to URL as new query string
                    MyRedirect2Addr &= "?" & RedirQueryString
                End If
            End If
            If UseTemporaryRedirection = False AndAlso Me.cammWebManager.DebugLevel = 0 Then
                Me.RedirectPermanently(MyRedirect2Addr) '301 permanent redirection
            Else
                cammWebManager.RedirectTo(MyRedirect2Addr) '301 temporary redirection
            End If
        End Sub

        ''' <summary>
        ''' Report exception by e-mail since there might be critical website errors if redirections doesn't work properly any more
        ''' </summary>
        ''' <param name="exception"></param>
        ''' <remarks></remarks>
        Protected Overridable Sub ReportException(ByVal exception As Exception)
            Try
                cammWebManager.Log.ReportErrorByEMail(exception, "Redirection link invalid")
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' Look up the redirection link and (usually) log/count the access
        ''' </summary>
        ''' <param name="RedirectID"></param>
        ''' <returns>The URL of the requested redirection link</returns>
        ''' <remarks></remarks>
        Protected Overridable Function Redirector_GetLink(ByVal RedirectID As Long) As String
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = New SqlConnection(cammWebManager.ConnectionString)

            If Utils.IsRequestFromCrawlerOrPotentialMachineAgent(Me.Request) = False Then
                'User agent in meaning of a real user - shall be logged and counted
                MyCmd.CommandText = "Redirects_LogAndGetURL"
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@IDRedirector", SqlDbType.Int).Value = RedirectID
            Else
                'Crawler/machine agent - shall not be logged as really executed user redirection
                MyCmd.CommandText = "SELECT RedirectTo FROM dbo.Redirects_ToAddr WHERE ID = @IDRedirector"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@IDRedirector", SqlDbType.Int).Value = RedirectID
            End If

            Return Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))

        End Function

    End Class

    ''' <summary>
    '''     Data handling for the module of redirections
    ''' </summary>
    Public Class Data
        ''' <summary>
        '''     Read the number of redirections
        ''' </summary>
        ''' <param name="id">The redirection ID as visible in the administration area</param>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        Public Shared Function NumberOfRedirections(ByVal id As Integer, ByVal webmanager As CompuMaster.camm.WebManager.IWebManager) As Integer
            Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(webmanager.ConnectionString), _
                                    "Select numberofredirections from Redirects_ToAddr where id = " & id.ToString, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
        End Function

    End Class

End Namespace