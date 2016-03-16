'Copyright 2014-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager
    Public Class PassswordReset

        Private cammWebManager As WMSystem
        Private UserInfo As IUserInformation

        Private token As String

        Public Sub New(cwm As WMSystem, userinfo As IUserInformation)
            Me.cammWebManager = cwm
            Me.UserInfo = userinfo
        End Sub

        Private Function ConstructUrl() As String
            Return Me.cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL + "?forceref=" + System.Web.HttpUtility.UrlEncode("/sysdata/account_resetpassword.aspx?user=" + UserInfo.ID.ToString() + "&token=" + System.Web.HttpUtility.UrlEncode(token))
        End Function

        Private Sub GenerateToken()
            Dim rng As New System.Security.Cryptography.RNGCryptoServiceProvider
            Dim randomBytes As Byte() = New Byte(32) {}
            rng.GetBytes(randomBytes)
            token = System.Convert.ToBase64String(randomBytes)
        End Sub

        Private Sub SaveToken()
            CompuMaster.camm.WebManager.DataLayer.Current.SetUserDetail(cammWebManager, Nothing, Me.UserInfo.ID, "PasswordResetToken", token)
        End Sub

        ''' <summary>
        ''' Returns a URL the user can open in order to reset his password. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateResetUrl() As String
            GenerateToken()
            SaveToken()
            Return ConstructUrl()
        End Function

        Private Function GetTokenCreationDate(ByVal token As String) As DateTime
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
            MyCmd.CommandText = "SELECT ModifiedOn FROM [dbo].Log_Users WHERE Type = 'PasswordResetToken' AND ID_User = @userid AND Value = @token"
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@userid", SqlDbType.Int).Value = Me.UserInfo.ID
            MyCmd.Parameters.Add("@token", SqlDbType.NVarChar, 255).Value = token

            Dim creationDate As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If creationDate Is Nothing OrElse IsDBNull(creationDate) Then
                Return Nothing
            End If
            Return CType(creationDate, DateTime)
        End Function

        Private Function TokenHasExpired(ByVal tokenDate As DateTime) As Boolean
            Return DateTime.Now.Subtract(tokenDate).TotalHours > Configuration.PasswordTokenExpirationHours
        End Function

        ''' <summary>
        ''' Checks whether the passed stoken is valid for the user
        ''' </summary>
        ''' <param name="token"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TokenIsValid(ByVal token As String) As Boolean
            Dim tokenDate As DateTime = GetTokenCreationDate(token)
            If tokenDate <> Nothing Then
                Return Not TokenHasExpired(tokenDate)
            End If
            Return False
        End Function

        Public Sub DeleteStoredToken()
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
            MyCmd.CommandText = "DELETE FROM [dbo].Log_Users WHERE Type = 'PasswordResetToken' AND ID_User = @userid"
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@userid", SqlDbType.Int).Value = Me.UserInfo.ID
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub


    End Class


End Namespace
