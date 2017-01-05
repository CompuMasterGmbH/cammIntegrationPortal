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
        ''' <remarks></remarks>
        Public Function CountConvertable() As Integer
            Dim MyCmd As New SqlClient.SqlCommand
            MyCmd.Connection = New SqlClient.SqlConnection(Me.CammWebManager.ConnectionString)
            MyCmd.CommandText = "SELECT COUNT(ID) FROM [dbo].Benutzer WHERE LoginPWAlgorithm IN " + GetDecryptableAlgosSQLString()
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