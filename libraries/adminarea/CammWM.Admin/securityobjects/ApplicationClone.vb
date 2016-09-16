'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Data.SqlClient
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     A page to clone an application
    ''' </summary>
    Public Class ApplicationClone
        Inherits Page

#Region "Page Events"
        Private Sub ApplicationClone_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim ErrMsg, Redirect2URL As String
            Dim iResult As Object = Nothing
            ErrMsg = ""
            Redirect2URL = ""

            If Not (cammWebManager.System_GetSubAuthorizationStatus("Applications", 0, cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous), "New")) Then
                Response.Write("No authorization to create new applications.")
                Response.End()
            ElseIf Request.QueryString("ID") <> "" And Request.Form("CloneType") <> "" Then
                Dim sqlParams As SqlParameter()
                If cammWebManager.System_DBVersion_Ex.Build >= WMSystem.MilestoneDBBuildNumber_AdminSecurityDelegates Then
                    'Clone AdminSecurityDelegates - supported since DB-Build 185
                    sqlParams = New SqlParameter() {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@AppID", CLng(Request.QueryString("ID"))), New SqlParameter("@CloneType", CLng(Request.Form("CloneType"))), New SqlParameter("@CopyDelegates", CLng(Utils.Nz(Request.Form("CopyAdminSecurityDelegates"), 0)))}
                Else
                    sqlParams = New SqlParameter() {New SqlParameter("@ReleasedByUserID", cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)), New SqlParameter("@AppID", CLng(Request.QueryString("ID"))), New SqlParameter("@CloneType", CLng(Request.Form("CloneType")))}
                End If

                iResult = ExecuteScalar(New SqlConnection(cammWebManager.ConnectionString), "AdminPrivate_CloneApplication", CommandType.StoredProcedure, sqlParams, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                If Utils.Nz(iResult, 0L) <> 0 Then
                    Redirect2URL = "apps_update.aspx?ID=" & iResult.ToString
                Else
                    ErrMsg = "Application creation failed!"
                End If

                If Redirect2URL <> "" Then Utils.RedirectTemporary(HttpContext.Current, Redirect2URL)
            ElseIf Request.Form("submit") <> "" Then
                ErrMsg = "Application cloning failed."
            End If

            If ErrMsg <> "" Then Response.Write("<p><font face=""Arial"" size=""2"" color=""red"">" & Utils.Nz(ErrMsg, String.Empty) & "</font></p>")
        End Sub
#End Region

    End Class

End Namespace