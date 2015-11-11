Option Explicit On 
Option Strict On

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Public Class Statistics
        Private _WebManager As WMSystem
        Sub New(ByVal WebManager As WMSystem)
            _WebManager = WebManager
        End Sub

        ''' <param name="SessionTimeout">A value in minutes how long a user session is regarded as active when no logout and no further action have been performed</param>
        Public Function NumberOfUsersCurrentlyOnline(Optional ByVal SessionTimeout As Integer = -30) As Integer
            Dim User_Auth_Validation_DBConn As New SqlConnection
            Dim User_Auth_Validation_RecSet As SqlDataReader = Nothing
            Dim User_Auth_Validation_Cmd As New SqlCommand
            Dim Result As Integer

            'Create connection
            User_Auth_Validation_DBConn.ConnectionString = _WebManager.ConnectionString
            Try
                User_Auth_Validation_DBConn.Open()

                'Get parameter value and append parameter
                With User_Auth_Validation_Cmd
                    .CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "select count (id_user) as NumberOfUsersCurrentlyOnline" & vbNewLine & _
                                    "from(system_usersessions)" & vbNewLine & _
                                    "where system_usersessions.id_session in " & vbNewLine & _
                                    "	(" & vbNewLine & _
                                    "	select case sessionid" & vbNewLine & _
                                    "	from dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes" & vbNewLine & _
                                    "	where dbo.System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes.inactive = 0" & vbNewLine & _
                                    "		and lastsessionstaterefresh > dateadd(minute, @SessionTimeout, getdate())" & vbNewLine & _
                                    "	)"
                    .CommandType = CommandType.Text

                    .Parameters.Add("@SessionTimeout", SqlDbType.Int).Value = SessionTimeout
                End With

                'Create recordset by executing the command
                User_Auth_Validation_Cmd.Connection = User_Auth_Validation_DBConn
                User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.ExecuteReader()

                User_Auth_Validation_RecSet.Read()
                Result = CType(User_Auth_Validation_RecSet(0), Integer)

            Finally
                If Not User_Auth_Validation_RecSet Is Nothing AndAlso Not User_Auth_Validation_RecSet.IsClosed Then
                    User_Auth_Validation_RecSet.Close()
                End If
                If Not User_Auth_Validation_Cmd Is Nothing Then
                    User_Auth_Validation_Cmd.Dispose()
                End If
                If Not User_Auth_Validation_DBConn Is Nothing Then
                    If User_Auth_Validation_DBConn.State <> ConnectionState.Closed Then
                        User_Auth_Validation_DBConn.Close()
                    End If
                    User_Auth_Validation_DBConn.Dispose()
                End If
            End Try

            Return Result

        End Function
    End Class

End Namespace