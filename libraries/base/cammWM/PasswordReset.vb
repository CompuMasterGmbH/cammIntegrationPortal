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
