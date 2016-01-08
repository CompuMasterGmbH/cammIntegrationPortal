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
            Dim Sql As String = "select count (id_user) as NumberOfUsersCurrentlyOnline" & vbNewLine & _
                "from system_usersessions" & vbNewLine & _
                "where system_usersessions.id_session in " & vbNewLine & _
                "	(" & vbNewLine & _
                "    	select sessionid" & vbNewLine & _
                "    	from dbo.System_WebAreasAuthorizedForSession" & vbNewLine & _
                "    	where lastsessionstaterefresh > dateadd(minute, -30, getdate())"
            Dim MyCmd As New SqlCommand(Sql, New SqlConnection(_WebManager.ConnectionString))
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@SessionTimeout", SqlDbType.Int).Value = SessionTimeout
            Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
        End Function
    End Class

End Namespace