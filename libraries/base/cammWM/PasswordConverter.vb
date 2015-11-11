#If NetFramework <> "1_1" Then
Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager

    Public Class PasswordConverter

        Private CammWebManager As WMSystem
        Private newAlgorithm As PasswordAlgorithm
        Private newAlgorithmTransformer As IWMPasswordTransformation

        Private UpdateCommand As SqlClient.SqlCommand
        Private UpdateTransaction As SqlClient.SqlTransaction

        Private Decryptors As New System.Collections.Generic.Dictionary(Of PasswordAlgorithm, IWMPasswordTransformationBackwards)

        Public Sub New(ByVal cwm As WMSystem, newAlgorithm As PasswordAlgorithm)
            Me.CammWebManager = cwm
            Me.newAlgorithm = newAlgorithm
            Me.newAlgorithmTransformer = PasswordTransformerFactory.ProduceCryptographicTransformer(newAlgorithm)
            SetDecryptors()
            SetUpdateCommand()
        End Sub

        Private Sub SetUpdateCommand()
            Me.UpdateCommand = New SqlClient.SqlCommand()
            Me.UpdateCommand.CommandText = "UPDATE [dbo].Benutzer SET LoginPW = @password, LoginPWAlgorithm = @Algo, LoginPWNonceValue = @Param WHERE ID = @UserID"
            Me.UpdateCommand.CommandType = CommandType.Text
            Me.UpdateCommand.Parameters.Add("@password", SqlDbType.VarChar)
            Me.UpdateCommand.Parameters.Add("@Algo", SqlDbType.Int)
            Me.UpdateCommand.Parameters.Add("@Param", SqlDbType.VarBinary)
            Me.UpdateCommand.Parameters.Add("@UserID", SqlDbType.Int)
        End Sub

        Private Sub ExecuteUpdate(ByVal transformResult As CryptoTransformationResult, ByVal userid As Long)
            Me.UpdateCommand.Parameters("@password").Value = transformResult.TransformedText
            Me.UpdateCommand.Parameters("@Algo").Value = transformResult.Algorithm
            Me.UpdateCommand.Parameters("@Param").Value = transformResult.Noncevalue
            Me.UpdateCommand.Parameters("@UserID").Value = userid

            Me.UpdateCommand.ExecuteNonQuery()
        End Sub

        Private Sub BeginTransaction()
            Dim connection As SqlClient.SqlConnection = New SqlClient.SqlConnection(Me.CammWebManager.ConnectionString)
            connection.Open()
            UpdateTransaction = connection.BeginTransaction()
            Me.UpdateCommand.Connection = connection
            Me.UpdateCommand.Transaction = UpdateTransaction
        End Sub

        Private Sub CommitTransaction()
            UpdateTransaction.Commit()
        End Sub

        Private Sub RollbackTransaction()
            UpdateTransaction.Rollback()
        End Sub

        Private Sub SetDecryptors()
            For Each algo As PasswordAlgorithm In GetDecryptibleAlgorithms()
                Dim decryptor As IWMPasswordTransformationBackwards = PasswordTransformerFactory.ProduceDecryptor(algo)
                Decryptors.Add(algo, decryptor)
            Next
        End Sub

        Private Function GetAlgorithms() As PasswordAlgorithm()
            Dim algos As System.Array = [Enum].GetValues(GetType(PasswordAlgorithm))
            Return CType(algos, PasswordAlgorithm())
        End Function

        Private Function GetDecryptibleAlgorithms() As PasswordAlgorithm()
            Dim result As New System.Collections.Generic.List(Of PasswordAlgorithm)
            For Each algo As PasswordAlgorithm In GetAlgorithms()
                If AlgorithmInfo.CanDecrypt(algo) Then
                    result.Add(algo)
                End If
            Next
            Return result.ToArray()
        End Function

        Private Function GetDecryptableAlgosSQLString() As String
            Dim result As String = "( "
            For Each algorithm As PasswordAlgorithm In GetAlgorithms()
                If algorithm <> Me.newAlgorithm Then
                    If AlgorithmInfo.CanDecrypt(algorithm) Then
                        result += CInt(algorithm).ToString() + ","
                    End If
                End If
            Next
            result = result.Remove(result.Length - 1, 1)
            result += ")"
            Return result
        End Function

        ''' <summary>
        ''' Counts how many passwords can/must be converted to the new algorithm
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CountConvertable() As Integer
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.CammWebManager.ConnectionString)
            MyCmd.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT COUNT(ID) FROM [dbo].Benutzer WHERE LoginPWAlgorithm IN " + GetDecryptableAlgosSQLString()
            MyCmd.CommandType = CommandType.Text

            Return CType(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
        End Function

        Private Sub SaveBatch()
            Try
                Me.CommitTransaction()
            Catch e As Exception
                Try
                    Me.RollbackTransaction()
                Catch ex As Exception
                    Throw New Exception("Error while rolling back transaction.", ex)
                End Try
                Throw New Exception("Error while commiting transaction. Changes reverted.", e)
            Finally
                If Me.UpdateTransaction.Connection IsNot Nothing Then
                    If Me.UpdateTransaction.Connection.State <> ConnectionState.Closed Then
                        Me.UpdateTransaction.Connection.Close()
                        Me.UpdateTransaction.Connection.Dispose()
                    End If

                End If
                If Me.UpdateTransaction IsNot Nothing Then
                    Me.UpdateTransaction.Dispose()
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Converts (up to) the specified amount of passwords into the new algorithm
        ''' </summary>
        ''' <param name="amount"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ConvertPasswords(ByVal amount As Integer) As Integer
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.CammWebManager.ConnectionString)
            MyCmd.CommandText = "SELECT TOP " + amount.ToString() + " ID, LoginPW, LoginPWAlgorithm, LoginPWNonceValue FROM [dbo].Benutzer WHERE LoginPWAlgorithm IN " + GetDecryptableAlgosSQLString()
            MyCmd.CommandType = CommandType.Text

            Dim reader As IDataReader = Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            Dim converted As Integer = 0

            Me.BeginTransaction()
            While reader.Read()
                Dim userId As Long = CType(reader(0), Long)
                Dim password As String = CType(reader(1), String)
                Dim algorithm As PasswordAlgorithm = CType(reader(2), PasswordAlgorithm)
                Dim param As Byte() = CType(reader(3), Byte())

                Dim decryptor As IWMPasswordTransformationBackwards = Me.Decryptors(algorithm)
                Dim decryptedPassword As String = decryptor.TransformStringBack(password, param)

                Dim encryptedResult As CryptoTransformationResult = Me.newAlgorithmTransformer.TransformString(decryptedPassword)

                Me.ExecuteUpdate(encryptedResult, userId)

                converted += 1
            End While

            Me.SaveBatch()

            Return converted

        End Function

    End Class



End Namespace
#End If