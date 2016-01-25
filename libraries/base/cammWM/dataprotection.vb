Option Explicit On
Option Strict On

Namespace CompuMaster.camm.WebManager
    Public Class DataProtectionSettings


        Private LogTypeDeletionList As New ArrayList

        Const PropertyName_AnonymizeIPs As String = "AnonymizeIPsAfterDays"
        Const PropertyName_DeleteUsersAfterDays As String = "DeleteUsersAfterDays"
        Const PropertyName_DeleteMailsAfterDays As String = "DeleteMailsAfterDays"
        Const PropertyName_LogTypeDeletion As String = "LogTypeDeletionSetting"

        Private _anonymizeIPsAfterDays As Integer
        Private _deleteDeactivatedUsersAfterDays As Integer
        Private _deleteMailsAfterDays As Integer



        Public Property AnonymizeIPsAfterDays As Integer
            Get
                Return _anonymizeIPsAfterDays
            End Get
            Set(value As Integer)
                _anonymizeIPsAfterDays = value
            End Set
        End Property

        Public Property DeleteDeactivatedUsersAfterDays As Integer
            Get
                Return _deleteDeactivatedUsersAfterDays
            End Get
            Set(value As Integer)
                _deleteDeactivatedUsersAfterDays = value
            End Set
        End Property

        Public Property DeleteMailsAfterDays As Integer
            Get
                Return _deleteMailsAfterDays
            End Get
            Set(value As Integer)
                _deleteMailsAfterDays = value
            End Set
        End Property


        Private ConnectionString As String

        Public Sub New(ByVal connectionString As String)
            Me.ConnectionString = connectionString
            LoadSettings()
        End Sub

        Private Sub LoadSettings()
            Dim cmd As New SqlClient.SqlCommand("SELECT PropertyName, ValueInt FROM System_GlobalProperties WHERE PropertyName IN ( '" & PropertyName_AnonymizeIPs & "', '" & PropertyName_DeleteUsersAfterDays & "', '" & PropertyName_DeleteMailsAfterDays & "'  )")
            cmd.Connection = New SqlClient.SqlConnection(Me.ConnectionString)
            Dim resultHashTable As Hashtable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)


            Dim anonymizeIpsDays As Object = resultHashTable(PropertyName_AnonymizeIPs)
            If Not anonymizeIpsDays Is Nothing AndAlso Not IsDBNull(anonymizeIpsDays) Then
                Me.AnonymizeIPsAfterDays = CType(anonymizeIpsDays, Integer)

            End If

            Dim deleteDeactivatedDays As Object = resultHashTable(PropertyName_DeleteUsersAfterDays)
            If Not deleteDeactivatedDays Is Nothing AndAlso Not IsDBNull(deleteDeactivatedDays) Then
                Me.DeleteDeactivatedUsersAfterDays = CType(deleteDeactivatedDays, Integer)
            End If

            Dim deleteMailsDays As Object = resultHashTable(PropertyName_DeleteMailsAfterDays)
            If Not deleteMailsDays Is Nothing AndAlso Not IsDBNull(deleteMailsDays) Then
                Me.DeleteMailsAfterDays = CType(deleteMailsDays, Integer)
            End If

        End Sub

        Private Sub ExecuteParameterizedQuery(ByVal cmd As SqlClient.SqlCommand, ByVal key As String, ByVal value As Object)
            cmd.Parameters("@key").Value = key
            cmd.Parameters("@value").Value = value
            cmd.ExecuteNonQuery()
        End Sub

        Public Sub AddLogTypeDeletionSetting(ByVal key As String, ByVal delete As Boolean)
            Dim newSetting As New ArrayList
            newSetting.Add(key)
            newSetting.Add(delete)
            LogTypeDeletionList.Add(newSetting)
        End Sub

        'TODO: maybe this doesn't really belong here
        'TODO: replace DISTINCT by GROUP BY construct
        Public Function GetLogTypes() As ArrayList
            Dim connection As New SqlClient.SqlConnection(Me.ConnectionString)
            Dim commandText As String = "SELECT DISTINCT Type, COALESCE((SELECT 1 FROM [dbo].System_GlobalProperties WHERE ValueNVarChar=dbo.Log_users.Type And ValueBoolean = 1 AND PropertyName='" & PropertyName_LogTypeDeletion & "'), 0) FROM dbo.Log_users"
            Dim cmd As New SqlClient.SqlCommand(commandText, connection)

            Dim reader As IDataReader = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Dim result As New ArrayList

            While reader.Read()
                Dim entry As New ArrayList
                entry.Add(reader(0))
                entry.Add(reader(1))
                result.Add(entry)
            End While
            Return result
        End Function





        ''' <summary>
        ''' Creates an sql command with an open connection and opened transaction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateSqlTransactionCommand() As SqlClient.SqlCommand
            Dim result As New SqlClient.SqlCommand
            result.Connection = New SqlClient.SqlConnection(Me.ConnectionString)
            result.Connection.Open()
            result.Transaction = result.Connection.BeginTransaction()
            Return result
        End Function


        Public Sub SaveSettings()
            Dim cmd As SqlClient.SqlCommand = Nothing
            Try
                cmd = CreateSqlTransactionCommand()
                cmd.CommandText = "UPDATE System_GlobalProperties SET ValueInt = @value WHERE PropertyName = @key " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO [dbo].[System_GlobalProperties] (PropertyName, ValueInt) VALUES (@key, @value)"
                cmd.Parameters.Add("@value", SqlDbType.Int)
                cmd.Parameters.Add("@key", SqlDbType.VarChar)

                ExecuteParameterizedQuery(cmd, PropertyName_AnonymizeIPs, Me.AnonymizeIPsAfterDays)
                ExecuteParameterizedQuery(cmd, PropertyName_DeleteUsersAfterDays, Me.DeleteDeactivatedUsersAfterDays)
                ExecuteParameterizedQuery(cmd, PropertyName_DeleteMailsAfterDays, Me.DeleteMailsAfterDays)

                cmd.CommandText = "UPDATE [dbo].[System_GlobalProperties] SET ValueBoolean = @value WHERE ValueNVarChar = @key AND PropertyName= '" & PropertyName_LogTypeDeletion & "'  " & _
                  "IF @@ROWCOUNT = 0 " & _
                  "INSERT INTO [dbo].[System_GlobalProperties] (PropertyName, ValueNVarChar, ValueBoolean) VALUES ('" & PropertyName_LogTypeDeletion & "', @key, @value)"
                cmd.Parameters.Remove(cmd.Parameters("@value"))
                cmd.Parameters.Add("@value", SqlDbType.Bit)
                For Each logtype As ArrayList In LogTypeDeletionList
                    ExecuteParameterizedQuery(cmd, CType(logtype(0), String), CType(logtype(1), Boolean))
                Next

                cmd.Transaction.Commit()
            Catch
                Try
                    If Not cmd Is Nothing Then
                        If Not cmd.Transaction Is Nothing Then
                            cmd.Transaction.Rollback()
                            cmd.Transaction.Dispose()
                            cmd.Transaction = Nothing
                        End If
                    End If
                Catch rollbackException As Exception
                    Throw New Exception("Failed to rollback transaction", rollbackException)
                End Try
            Finally
                If Not cmd Is Nothing Then
                    If Not cmd.Transaction Is Nothing Then
                        cmd.Transaction.Dispose()
                        cmd.Transaction = Nothing
                    End If
                    If Not cmd.Connection Is Nothing Then
                        Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(cmd.Connection)
                    End If
                End If
            End Try

        End Sub





    End Class
End Namespace