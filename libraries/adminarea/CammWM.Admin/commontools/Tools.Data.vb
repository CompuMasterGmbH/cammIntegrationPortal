'Copyright 2004-2008,2015,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

'Entkoppelt von Ursprungs-Version durch geänderten Namespace
'Entschlackt von Code-Bestandteilen ohne Verwendung

Imports System.IO

Namespace CompuMaster.camm.WebManager.Administration.Tools.Data

    ''' <summary>
    '''     CompuMaster common tools and utilities
    ''' </summary>
    Friend Class NamespaceDoc
    End Class

    Namespace DataQuery

        ''' <summary>
        '''     Common routines to query data from any data provider
        ''' </summary>
        Friend Class AnyIDataProvider
            ''' <summary>
            '''     Create a new database connection by reflection of a type name
            ''' </summary>
            ''' <param name="assemblyName">The assembly which implements the desired connection type</param>
            ''' <param name="connectionTypeName">The case-insensitive type name of the connection class, e. g. System.Data.SqlClient.SqlConnection</param>
            ''' <returns>The created connection object as an IDbConnection</returns>
            ''' <remarks>
            '''     Errors will be thrown in case of unresolvable parameter values or if the created type can't be casted into an IDbConnection.
            ''' </remarks>
            Public Shared Function CreateConnection(ByVal assemblyName As String, ByVal connectionTypeName As String) As IDbConnection
                Dim connectionType As Type = Nothing
                Dim runningAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly
                Dim referencedAssemblies As System.Reflection.AssemblyName() = runningAssembly.GetReferencedAssemblies

                For Each currentAssemblyName As System.Reflection.AssemblyName In referencedAssemblies
                    If currentAssemblyName.Name = assemblyName Then
                        Dim referencedAssembly As System.Reflection.Assembly = System.Reflection.Assembly.Load(currentAssemblyName.FullName)
                        connectionType = referencedAssembly.GetType(connectionTypeName, True, True)
                        Exit For
                    End If
                Next

                If Not connectionType Is Nothing Then
                    Return CType(Activator.CreateInstance(connectionType), IDbConnection)
                Else
                    Throw New ArgumentException("Assembly not found in the list of referenced assemblies", "assemblyName")
                End If

            End Function
            ''' <summary>
            '''     Create a new database connection by reflection of a type name
            ''' </summary>
            ''' <param name="assemblyName">The assembly which implements the desired connection type</param>
            ''' <param name="connectionTypeName">The case-insensitive type name of the connection class, e. g. System.Data.SqlClient.SqlConnection</param>
            ''' <param name="connectionString">A connection string to be used for this connection</param>
            ''' <returns>The created connection object as an IDbConnection</returns>
            ''' <remarks>
            '''     Errors will be thrown in case of unresolvable parameter values or if the created type can't be casted into an IDbConnection.
            ''' </remarks>
            Public Shared Function CreateConnection(ByVal assemblyName As String, ByVal connectionTypeName As String, ByVal connectionString As String) As IDbConnection
                Dim Result As IDbConnection = CreateConnection(assemblyName, connectionTypeName)
                Result.ConnectionString = connectionString
                Return Result
            End Function
            ''' <summary>
            '''     Automations for the connection in charge
            ''' </summary>
            Friend Enum Automations As Integer
                None = 0
                AutoOpenConnection = 1
                AutoCloseAndDisposeConnection = 2
                AutoOpenAndCloseAndDisposeConnection = 3
            End Enum
            ''' <summary>
            '''     Executes a command without returning any data
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <param name="commandTimeout">A timeout value in seconds for the command object (negative values will be ignored and leave the timeout value on default)</param>
            Friend Shared Sub ExecuteNonQuery(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations, ByVal commandTimeout As Integer)
                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not commandTimeout < 0 Then
                    MyCmd.CommandTimeout = commandTimeout
                End If
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Dim Result As Object
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    Result = MyCmd.ExecuteNonQuery
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
            End Sub
            ''' <summary>
            '''     Executes a command without returning any data
            ''' </summary>
            ''' <param name="dbCommand">The command with an assigned connection property value</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Sub ExecuteNonQuery(ByVal dbCommand As IDbCommand, ByVal automations As Automations)
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyConn As IDbConnection = MyCmd.Connection
                Dim Result As Integer
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    Result = MyCmd.ExecuteNonQuery
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
            End Sub
            ''' <summary>
            '''     Executes a command without returning any data
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Sub ExecuteNonQuery(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations)
                ExecuteNonQuery(dbConnection, commandText, commandType, sqlParameters, automations, -1)
            End Sub
            ''' <summary>
            '''     Executes a command without returning any data
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            Friend Shared Sub ExecuteNonQuery(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter())
                ExecuteNonQuery(dbConnection, commandText, commandType, sqlParameters, Automations.AutoOpenAndCloseAndDisposeConnection)
            End Sub
            ''' <summary>
            '''     Executes a command scalar and returns the value
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function ExecuteScalar(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations) As Object
                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Dim Result As Object
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    Result = MyCmd.ExecuteScalar
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Executes a command scalar and returns the value
            ''' </summary>
            ''' <param name="dbCommand">The command with an assigned connection property value</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function ExecuteScalar(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As Object
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyConn As IDbConnection = MyCmd.Connection
                Dim Result As Object
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    Result = MyCmd.ExecuteScalar
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function

            ''' <summary>
            '''     Data execution exceptions with details on the executed IDbCommand
            ''' </summary>
            Friend Class DataException
                Inherits System.Exception

                Private _commandText As String

                Friend Sub New(ByVal command As IDbCommand, ByVal innerException As Exception)
                    MyBase.New("Data layer exception", innerException)
#If DEBUG Then
                    _commandText = "CommandType: " & command.CommandType.ToString & vbNewLine
                    _commandText &= "CommandText:" & vbNewLine & command.CommandText
                    _commandText &= vbNewLine & vbNewLine
                    If command.Parameters.Count > 0 Then
                        _commandText &= "Parameters:" & vbNewLine & ConvertParameterCollectionToString(command.Parameters) & vbNewLine
                    Else
                        _commandText &= "Parameters:" & vbNewLine & "The parameters collection is empty" & vbNewLine
                    End If
#End If
                End Sub

#If DEBUG Then
                ''' <summary>
                '''     Convert the collection with all the parameters to a plain text string
                ''' </summary>
                ''' <param name="parameters">An IDataParameterCollection of a IDbCommand</param>
                Private Function ConvertParameterCollectionToString(ByVal parameters As System.Data.IDataParameterCollection) As String
                    Dim Result As String = Nothing
                    For MyCounter As Integer = 0 To parameters.Count - 1
                        Result &= "Parameter " & MyCounter & ": "
                        Try
                            Result &= CType(parameters(MyCounter), IDataParameter).ParameterName & ": "
                            Try
                                If CType(parameters(MyCounter), IDataParameter).Value Is Nothing Then
                                    Result &= "{null}"
                                ElseIf IsDBNull(CType(parameters(MyCounter), IDataParameter).Value) Then
                                    Result &= "{DBNull.Value}"
                                Else
                                    Result &= CType(parameters(MyCounter), IDataParameter).Value.ToString
                                End If
                            Catch
                                Result &= "{" & CType(parameters(MyCounter), IDataParameter).Value.GetType.ToString & "}"
                            End Try
                        Catch
                            Result &= "{" & parameters(MyCounter).GetType.ToString & "}"
                        End Try
                        Result &= vbNewLine
                    Next
                    Return Result
                End Function
                ''' <summary>
                '''     The complete and detailed exception information inclusive the command text
                ''' </summary>
                Public Overrides Function ToString() As String
                    Return MyBase.ToString & vbNewLine & vbNewLine & _commandText
                End Function
#End If
            End Class
            ''' <summary>
            '''     Executes a command scalar and returns the value
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            Friend Shared Function ExecuteScalar(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter()) As Object
                Return ExecuteScalar(dbConnection, commandText, commandType, sqlParameters, Automations.AutoOpenAndCloseAndDisposeConnection)
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first column
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function ExecuteReaderAndPutFirstColumnIntoArrayList(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations) As ArrayList
                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Return ExecuteReaderAndPutFirstColumnIntoArrayList(MyCmd, automations)
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first column
            ''' </summary>
            ''' <param name="dbCommand">The command object which shall be executed</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function ExecuteReaderAndPutFirstColumnIntoArrayList(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As ArrayList
                Dim MyConn As IDbConnection = dbCommand.Connection
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyReader As IDataReader = Nothing
                Dim Result As New ArrayList
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Result.Add(MyReader(0))
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first column
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            Friend Shared Function ExecuteReaderAndPutFirstColumnIntoArrayList(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter()) As ArrayList
                Return ExecuteReaderAndPutFirstColumnIntoArrayList(dbConnection, commandText, commandType, sqlParameters, Automations.AutoOpenAndCloseAndDisposeConnection)
            End Function
            Friend Shared Function ExecuteReaderAndPutFirstColumnIntoGenericList(Of TValue)(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As System.Collections.Generic.List(Of TValue)
                Dim MyConn As IDbConnection = dbCommand.Connection
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyReader As IDataReader = Nothing
                Dim Result As New System.Collections.Generic.List(Of TValue)
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If Not MyConn Is Nothing AndAlso MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        If IsDBNull(MyReader(0)) Then
                            Result.Add(Nothing)
                        Else
                            Result.Add(CType(MyReader(0), TValue))
                        End If
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function

            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbCommand">The command object which shall be executed</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>A list of KeyValuePairs with the values of the first column in the key field and the second column values in the value field, NULL values are initialized with null (Nothing in VisualBasic)</returns>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoGenericKeyValuePairs(Of TKey, TValue)(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As System.Collections.Generic.List(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue))
                Dim Result As New System.Collections.Generic.List(Of System.Collections.Generic.KeyValuePair(Of TKey, TValue))
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyReader As IDataReader = Nothing
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyCmd.Connection.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Dim key As TKey, value As TValue
                        If IsDBNull(MyReader(0)) Then
                            key = Nothing
                        Else
                            key = CType(MyReader(0), TKey)
                        End If
                        If IsDBNull(MyReader(1)) Then
                            value = Nothing
                        Else
                            value = CType(MyReader(1), TValue)
                        End If
                        Result.Add(New System.Collections.Generic.KeyValuePair(Of TKey, TValue)(key, value))
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyCmd.Connection Is Nothing Then
                            If MyCmd.Connection.State <> ConnectionState.Closed Then
                                MyCmd.Connection.Close()
                            End If
                            MyCmd.Connection.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function

            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbCommand">The command object which shall be executed</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>A dictionary of KeyValuePairs with the values of the first column in the key field and the second column values in the value field, NULL values are initialized with null (Nothing in VisualBasic)</returns>
            ''' <remarks>
            ''' ATTENTION: Please note that multiple but equal values from the first column will result in 1 key/value pair since hashtables use a unique key and override the value with the last assignment. Alternatively you may want to receive a List of KeyValuePairs.
            ''' </remarks>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoGenericDictionary(Of TKey, TValue)(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As System.Collections.Generic.Dictionary(Of TKey, TValue)
                Dim Result As New System.Collections.Generic.Dictionary(Of TKey, TValue)
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyReader As IDataReader = Nothing
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyCmd.Connection.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Dim key As TKey, value As TValue
                        If IsDBNull(MyReader(0)) Then
                            key = Nothing
                        Else
                            key = CType(MyReader(0), TKey)
                        End If
                        If IsDBNull(MyReader(1)) Then
                            value = Nothing
                        Else
                            value = CType(MyReader(1), TValue)
                        End If
                        If Result.ContainsKey(key) Then
                            Result(key) = value
                        Else
                            Result.Add(key, value)
                        End If
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyCmd.Connection Is Nothing Then
                            If MyCmd.Connection.State <> ConnectionState.Closed Then
                                MyCmd.Connection.Close()
                            End If
                            MyCmd.Connection.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function

            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first column
            ''' </summary>
            ''' <param name="dbCommand">The command object which shall be executed</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>A hashtable with the values of the first column in the hashtable's key field and the second column values in the hashtable's value field</returns>
            ''' <remarks>
            ''' ATTENTION: Please note that multiple but equal values from the first column will result in 1 key/value pair since hashtables use a unique key and override the value with the last assignment. Alternatively you may want to receive an array of DictionaryEntry.
            ''' </remarks>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As Hashtable
                Dim Result As New Hashtable
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyReader As IDataReader = Nothing
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyCmd.Connection.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Result.Add(MyReader(0), MyReader(1))
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyCmd.Connection Is Nothing Then
                            If MyCmd.Connection.State <> ConnectionState.Closed Then
                                MyCmd.Connection.Close()
                            End If
                            MyCmd.Connection.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>A hashtable with the values of the first column in the hashtable's key field and the second column values in the hashtable's value field</returns>
            ''' <remarks>
            ''' ATTENTION: Please note that multiple but equal values from the first column will result in 1 key/value pair since hashtables use a unique key and override the value with the last assignment. Alternatively you may want to receive an array of DictionaryEntry.
            ''' </remarks>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations) As Hashtable
                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Dim MyReader As IDataReader = Nothing
                Dim Result As New Hashtable
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Result.Add(MyReader(0), MyReader(1))
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbCommand">The prepared command to the database</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>An array of DictionaryEntry with the values of the first column as the key element and the second column values in the value element of the DictionaryEntry</returns>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As DictionaryEntry()
                Dim Result As New ArrayList
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyReader As IDataReader = Nothing
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyCmd.Connection.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Result.Add(New DictionaryEntry(MyReader(0), MyReader(1)))
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyCmd.Connection Is Nothing Then
                            If MyCmd.Connection.State <> ConnectionState.Closed Then
                                MyCmd.Connection.Close()
                            End If
                            MyCmd.Connection.Dispose()
                        End If
                    End If
                End Try
                Return CType(Result.ToArray(GetType(DictionaryEntry)), DictionaryEntry())
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>An array of DictionaryEntry with the values of the first column as the key element and the second column values in the value element of the DictionaryEntry</returns>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations) As DictionaryEntry()
                Dim Result As New ArrayList
                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Dim MyReader As IDataReader = Nothing
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    MyReader = MyCmd.ExecuteReader
                    While MyReader.Read
                        Result.Add(New DictionaryEntry(MyReader(0), MyReader(1)))
                    End While
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End If
                End Try
                Return CType(Result.ToArray(GetType(DictionaryEntry)), DictionaryEntry())
            End Function
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <returns>A hashtable with the values of the first column in the hashtable's key field and the second column values in the hashtable's value field</returns>
            ''' <remarks>
            ''' ATTENTION: Please note that multiple but equal values from the first column will result in 1 key/value pair since hashtables use a unique key and override the value with the last assignment. Alternatively you may want to receive an array of DictionaryEntry.
            ''' </remarks>
            Friend Shared Function ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter()) As Hashtable
                Return ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(dbConnection, commandText, commandType, sqlParameters, Automations.AutoOpenAndCloseAndDisposeConnection)
            End Function
            ''' <summary>
            '''     Executes a command and return the data reader object for it
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <param name="commandTimeout">A timeout value in seconds for the command object (negative values will be ignored and leave the timeout value on default)</param>
            ''' <remarks>
            '''     Automations can only open a connection, but never close. This is because you have to close the connection by yourself AFTER you walked through the data reader.
            ''' </remarks>
            Friend Shared Function ExecuteReader(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations, ByVal commandTimeout As Integer) As IDataReader
                If automations = Automations.AutoCloseAndDisposeConnection OrElse automations = Automations.AutoOpenAndCloseAndDisposeConnection Then
                    Throw New Exception("Can't close a data reader automatically since data has to be read first")
                End If

                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not commandTimeout < 0 Then
                    MyCmd.CommandTimeout = commandTimeout
                End If
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Dim Result As IDataReader
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    Result = MyCmd.ExecuteReader
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Executes a command and return the data reader object for it
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            Friend Shared Function ExecuteReader(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations) As IDataReader

                Dim MyConn As IDbConnection = dbConnection
                Dim MyCmd As IDbCommand = MyConn.CreateCommand
                MyCmd.CommandText = commandText
                MyCmd.CommandType = commandType
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Dim Result As IDataReader
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyConn.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    If automations = Automations.AutoCloseAndDisposeConnection OrElse automations = Automations.AutoOpenAndCloseAndDisposeConnection Then
                        Result = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    Else
                        Result = MyCmd.ExecuteReader()
                    End If
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Executes a command and return the data reader object for it
            ''' </summary>
            ''' <param name="dbCommand">The command with an assigned connection property value</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function ExecuteReader(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As IDataReader
                Dim MyCmd As IDbCommand = dbCommand
                Dim MyConn As IDbConnection = MyCmd.Connection
                Dim Result As IDataReader
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If MyCmd.Connection.State <> ConnectionState.Open Then
                            MyConn.Open()
                        End If
                    End If
                    If automations = Automations.AutoCloseAndDisposeConnection OrElse automations = Automations.AutoOpenAndCloseAndDisposeConnection Then
                        Result = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    Else
                        Result = MyCmd.ExecuteReader()
                    End If
                Catch ex As Exception
                    Throw New DataException(MyCmd, ex)
                Finally
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Fill a new data table with the result of a command
            ''' </summary>
            ''' <param name="dbCommand">The command object</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <param name="tableName">The name for the new table</param>
            Friend Shared Function FillDataTable(ByVal dbCommand As IDbCommand, ByVal automations As Automations, ByVal tableName As String) As System.Data.DataTable
                Dim MyReader As IDataReader = Nothing
                Dim Result As New System.Data.DataTable
                Dim dbConnection As IDbConnection = dbCommand.Connection
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If dbConnection.State <> ConnectionState.Open Then
                            dbConnection.Open()
                        End If
                    End If
                    'Attention: ExecuteReader doesn't allow auto-close of the connection
                    Dim Automation As Automations
                    If automations = Automations.AutoCloseAndDisposeConnection Then
                        Automation = Automations.None
                    ElseIf automations = Automations.AutoOpenAndCloseAndDisposeConnection Then
                        Automation = Automations.AutoOpenConnection
                    End If
                    'Execute the reader
                    MyReader = Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(dbCommand, Automation)
                    'Convert the reader to a data table
                    Result = Tools.Data.DataTables.ConvertDataReaderToDataTable(MyReader, tableName)
                Catch ex As Exception
                    Throw New DataException(dbCommand, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso MyReader.IsClosed = False Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        CloseAndDisposeConnection(dbConnection)
                    End If
                End Try
                Return Result
            End Function
            ''' <summary>
            '''     Fill a new data table with the result of a command
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <param name="tableName">The name for the new table</param>
            ''' <param name="commandTimeout">A timeout value in seconds for the command object (negative values will be ignored and leave the timeout value on default)</param>
            Friend Shared Function FillDataTable(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations, ByVal tableName As String, ByVal commandTimeout As Integer) As System.Data.DataTable
                Dim MyCmd As IDbCommand = dbConnection.CreateCommand
                MyCmd.CommandType = commandType
                If commandTimeout >= 0 Then 'never assign a -1 value
                    MyCmd.CommandTimeout = commandTimeout
                End If
                MyCmd.CommandText = commandText
                If Not sqlParameters Is Nothing Then
                    For Each MySqlParam As IDataParameter In sqlParameters
                        MyCmd.Parameters.Add(MySqlParam)
                    Next
                End If
                Return FillDataTable(MyCmd, automations, tableName)
            End Function
            ''' <summary>
            '''     Fill a new data table with the result of a command
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <param name="tableName">The name for the new table</param>
            Friend Shared Function FillDataTable(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations, ByVal tableName As String) As System.Data.DataTable
                Return FillDataTable(dbConnection, commandText, commandType, sqlParameters, automations, tableName, -1)
            End Function
            ''' <summary>
            '''     Fill a new data table with the result of a command
            ''' </summary>
            ''' <param name="dbConnection">The connection to the database</param>
            ''' <param name="commandText">The command text</param>
            ''' <param name="commandType">The command type</param>
            ''' <param name="sqlParameters">An optional list of SqlParameters</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function FillDataTable(ByVal dbConnection As IDbConnection, ByVal commandText As String, ByVal commandType As System.Data.CommandType, ByVal sqlParameters As IDataParameter(), ByVal automations As Automations) As System.Data.DataTable
                Return FillDataTable(dbConnection, commandText, commandType, sqlParameters, automations, Nothing)
            End Function
            ''' <summary>
            '''     Fill a new data table with the result of a command
            ''' </summary>
            ''' <param name="dbCommand">The command object</param>
            ''' <param name="automations">Automation options for the connection</param>
            Friend Shared Function FillDataTables(ByVal dbCommand As IDbCommand, ByVal automations As Automations) As System.Data.DataTable()
                Dim MyReader As IDataReader = Nothing
                Dim Results As New ArrayList
                Dim dbConnection As IDbConnection = dbCommand.Connection
                Try
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoOpenConnection Then
                        If dbConnection.State <> ConnectionState.Open Then
                            dbConnection.Open()
                        End If
                    End If
                    'Attention: ExecuteReader doesn't allow auto-close of the connection
                    Dim Automation As Automations
                    If automations = Automations.AutoCloseAndDisposeConnection Then
                        Automation = Automations.None
                    ElseIf automations = Automations.AutoOpenAndCloseAndDisposeConnection Then
                        Automation = Automations.AutoOpenConnection
                    End If
                    'Execute the reader
                    MyReader = Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(dbCommand, Automation)
                    'Convert the reader to data tables
                    Dim Result As New System.Data.DataSet
                    Result = Tools.Data.DataTables.ConvertDataReaderToDataSet(MyReader)
                    For MyCounter As Integer = 0 To Result.Tables.Count - 1
                        Results.Add(Result.Tables(MyCounter))
                    Next
                Catch ex As Exception
                    Throw New DataException(dbCommand, ex)
                Finally
                    If Not MyReader Is Nothing AndAlso MyReader.IsClosed = False Then
                        MyReader.Close()
                    End If
                    If automations = Automations.AutoOpenAndCloseAndDisposeConnection OrElse automations = Automations.AutoCloseAndDisposeConnection Then
                        CloseAndDisposeConnection(dbConnection)
                    End If
                End Try
                Return CType(Results.ToArray(GetType(DataTable)), DataTable())
            End Function
            ''' <summary>
            '''     Securely close and dispose a database connection
            ''' </summary>
            ''' <param name="connection">The connection to close and dispose</param>
            Friend Shared Sub CloseAndDisposeConnection(ByVal connection As IDbConnection)
                Dim MyConn As IDbConnection = connection
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Sub
            ''' <summary>
            '''     Securely close a database connection
            ''' </summary>
            ''' <param name="connection">The connection to close</param>
            Friend Shared Sub CloseConnection(ByVal connection As IDbConnection)
                Dim MyConn As IDbConnection = connection
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Sub
            ''' <summary>
            '''     Open a database connection if it is not already opened
            ''' </summary>
            ''' <param name="connection">The connection to open</param>
            Friend Shared Sub OpenConnection(ByVal connection As IDbConnection)
                If connection Is Nothing Then
                    Throw New ArgumentNullException("connection")
                End If
                Dim MyConn As IDbConnection = connection
                If MyConn.State <> ConnectionState.Open Then
                    MyConn.Open()
                End If
            End Sub

        End Class

    End Namespace

    ''' <summary>
    '''     Provides simplified access to CSV files
    ''' </summary>
    Friend Class Csv

#Region "Read data"

#Region "Fixed columns"
        ''' <summary>
        '''     Read from a CSV table
        ''' </summary>
        ''' <param name="reader">A stream reader targetting CSV data</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="columnWidths">An array of column widths in their order</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Private Shared Function ReadDataTableFromCsvReader(ByVal reader As StreamReader, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, ByVal columnWidths As Integer(), Optional ByVal convertEmptyStringsToDBNull As Boolean = False) As DataTable

            If cultureFormatProvider Is Nothing Then
                cultureFormatProvider = System.Globalization.CultureInfo.InvariantCulture
            End If
            If columnWidths Is Nothing Then
                columnWidths = New Integer() {Integer.MaxValue}
            End If

            Dim Result As New DataTable
            Dim rdStr As String
            Dim RowCounter As Integer

            'Read file content
            rdStr = reader.ReadToEnd
            If rdStr = Nothing Then
                'simply return the empty table when there is no input data
                Return Result
            End If

            'Read the file char by char and add row by row
            Dim CharPosition As Integer = 0
            While CharPosition < rdStr.Length

                'Read the next csv row
                Dim ColValues As New ArrayList
                SplitFixedCsvLineIntoCellValues(rdStr, ColValues, CharPosition, columnWidths)

                'Add it as a new data row (respectively add the columns definition)
                RowCounter += 1
                If RowCounter = 1 AndAlso includesColumnHeaders Then
                    'Read first line as column names
                    For ColCounter As Integer = 0 To ColValues.Count - 1
                        Dim colName As String = Trim(CType(ColValues(ColCounter), String))
                        If Result.Columns.Contains(colName) Then
                            colName = String.Empty
                        End If
                        Result.Columns.Add(New DataColumn(colName, GetType(String)))
                    Next
                Else
                    'Read line as data and automatically add required additional columns on the fly
                    Dim MyRow As DataRow = Result.NewRow
                    For ColCounter As Integer = 0 To ColValues.Count - 1
                        Dim colValue As String = Trim(CType(ColValues(ColCounter), String))
                        If Result.Columns.Count <= ColCounter Then
                            Result.Columns.Add(New DataColumn(Nothing, GetType(String)))
                        End If
                        MyRow(ColCounter) = colValue
                    Next
                    Result.Rows.Add(MyRow)
                End If

            End While

            If convertEmptyStringsToDBNull Then
                ConvertEmptyStringsToDBNullValue(Result)
            Else
                ConvertDBNullValuesToEmptyStrings(Result)
            End If

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV file
        ''' </summary>
        ''' <param name="path">The path of the file</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="columnWidths">An array of column widths in their order</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvFile(path As String, includesColumnHeaders As Boolean, columnWidths As Integer(), encoding As String, convertEmptyStringsToDBNull As Boolean) As DataTable

            Dim Result As New DataTable

            If File.Exists(path) Then
            ElseIf path.ToLower.StartsWith("http://") OrElse path.ToLower.StartsWith("https://") Then
                Dim LocalCopyOfFileContentFromRemoteUri As String = Utils.ReadStringDataFromUri(path, encoding)
                Result = ReadDataTableFromCsvString(LocalCopyOfFileContentFromRemoteUri, includesColumnHeaders, columnWidths, convertEmptyStringsToDBNull)
                Result.TableName = System.IO.Path.GetFileNameWithoutExtension(path)
                Return Result
            Else
                Throw New System.IO.FileNotFoundException("File not found", path)
            End If

            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(path, System.Text.Encoding.GetEncoding(encoding))
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, System.Globalization.CultureInfo.CurrentCulture, columnWidths, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV file
        ''' </summary>
        ''' <param name="path">The path of the file</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="columnWidths">An array of column widths in their order</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="cultureFormatProvider"></param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvFile(path As String, includesColumnHeaders As Boolean, columnWidths As Integer(), encoding As System.Text.Encoding, cultureFormatProvider As System.Globalization.CultureInfo, convertEmptyStringsToDBNull As Boolean) As DataTable

            Dim Result As New DataTable

            If File.Exists(path) Then
            ElseIf path.ToLower.StartsWith("http://") OrElse path.ToLower.StartsWith("https://") Then
                Dim LocalCopyOfFileContentFromRemoteUri As String = Utils.ReadStringDataFromUri(path, encoding.WebName)
                Result = ReadDataTableFromCsvString(LocalCopyOfFileContentFromRemoteUri, includesColumnHeaders, columnWidths, convertEmptyStringsToDBNull)
                Result.TableName = System.IO.Path.GetFileNameWithoutExtension(path)
                Return Result
            Else
                Throw New System.IO.FileNotFoundException("File not found", path)
            End If

            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(path, encoding)
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, cultureFormatProvider, columnWidths, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV table
        ''' </summary>
        ''' <param name="data">The content of a CSV file</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="columnWidths">An array of column widths in their order</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvString(ByVal data As String, ByVal includesColumnHeaders As Boolean, ByVal columnWidths As Integer(), Optional ByVal convertEmptyStringsToDBNull As Boolean = False) As DataTable

            Dim Result As New DataTable
            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(New MemoryStream(System.Text.Encoding.Unicode.GetBytes(data)), System.Text.Encoding.Unicode, False)
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, System.Globalization.CultureInfo.CurrentCulture, columnWidths, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV table
        ''' </summary>
        ''' <param name="data">The content of a CSV file</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="cultureFormatProvider"></param>
        ''' <param name="columnWidths">An array of column widths in their order</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvString(ByVal data As String, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, ByVal columnWidths As Integer(), Optional ByVal convertEmptyStringsToDBNull As Boolean = False) As DataTable

            Dim Result As New DataTable
            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(New MemoryStream(System.Text.Encoding.Unicode.GetBytes(data)), System.Text.Encoding.Unicode, False)
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, cultureFormatProvider, columnWidths, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Split a line content into separate column values and add them to the output list
        ''' </summary>
        ''' <param name="lineContent">The line content as it has been read from the CSV file</param>
        ''' <param name="outputList">An array list which shall hold the separated column values</param>
        ''' <param name="startPosition">The start position to which the columnWidhts are related to</param>
        ''' <param name="columnWidths">An array of column widths in their order</param>
        Private Shared Sub SplitFixedCsvLineIntoCellValues(ByRef lineContent As String, ByVal outputList As ArrayList, ByRef startposition As Integer, ByVal columnWidths As Integer())

            Dim CurrentColumnValue As System.Text.StringBuilder = Nothing
            Dim CharPositionCounter As Integer = 0

            For CharPositionCounter = startposition To lineContent.Length - 1
                If CharPositionCounter = startposition Then
                    'Prepare the new value for the first column
                    CurrentColumnValue = New System.Text.StringBuilder
                ElseIf SplitFixedCsvLineIntoCellValuesIsNewColumnPosition(CharPositionCounter, startposition, columnWidths) Then
                    'A new column has been found
                    'Save the previous column value 
                    outputList.Add(CurrentColumnValue.ToString)
                    'Prepare the new value for  the next column
                    CurrentColumnValue = New System.Text.StringBuilder
                End If
                Select Case lineContent.Chars(CharPositionCounter)
                    Case ControlChars.Lf
                        'now it's a line separator
                        Exit For
                    Case ControlChars.Cr
                        'now it's a line separator
                        If CharPositionCounter + 1 < lineContent.Length AndAlso lineContent.Chars(CharPositionCounter + 1) = ControlChars.Lf Then
                            'Found a CrLf occurance; handle it as one line break!
                            CharPositionCounter += 1
                        End If
                        Exit For
                    Case Else
                        'just add the character as it is because it's inside of a cell text
                        CurrentColumnValue.Append(lineContent.Chars(CharPositionCounter))
                End Select
            Next

            'Add the last column value to the collection
            If Not CurrentColumnValue Is Nothing AndAlso CurrentColumnValue.Length <> 0 Then
                outputList.Add(CurrentColumnValue.ToString)
            End If

            'Next start position is the next char after the last read one
            startposition = CharPositionCounter + 1

        End Sub
        ''' <summary>
        '''     Calculate if the current position is the first position of a new column
        ''' </summary>
        ''' <param name="currentPosition">The current position in the whole document</param>
        ''' <param name="startPosition">The start position to which the columnWidhts are related to</param>
        ''' <param name="columnWidths">An array containing the definitions of the column widths</param>
        ''' <returns>True if the current position identifies a new column value, otherwise False</returns>
        Private Shared Function SplitFixedCsvLineIntoCellValuesIsNewColumnPosition(ByVal currentPosition As Integer, ByVal startPosition As Integer, ByVal columnWidths As Integer()) As Boolean
            Dim positionDifference As Integer = currentPosition - startPosition
            For MyCounter As Integer = 0 To columnWidths.Length - 1
                Dim ColumnStartPosition As Integer
                ColumnStartPosition += columnWidths(MyCounter)
                If positionDifference = ColumnStartPosition Then
                    Return True
                End If
            Next
            Return False
        End Function

        Private Shared Function SumOfIntegerValues(ByVal array As Integer(), ByVal sumUpToElementIndex As Integer) As Integer
            Dim Result As Integer
            For MyCounter As Integer = 0 To sumUpToElementIndex
                Result += array(MyCounter)
            Next
            Return Result
        End Function
#End Region

#Region "Separator separation"
        ''' <summary>
        '''     Read from a CSV file
        ''' </summary>
        ''' <param name="path">The path of the file</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="recognizeMultipleColumnSeparatorCharsAsOne">Currently without purpose</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvFile(path As String, includesColumnHeaders As Boolean, encoding As String, columnSeparator As Char, recognizeTextBy As Char, recognizeMultipleColumnSeparatorCharsAsOne As Boolean, convertEmptyStringsToDBNull As Boolean) As DataTable

            Dim Result As New DataTable

            If File.Exists(path) Then
            ElseIf path.ToLower.StartsWith("http://") OrElse path.ToLower.StartsWith("https://") Then
                Dim LocalCopyOfFileContentFromRemoteUri As String = Utils.ReadStringDataFromUri(path, encoding)
                Result = ReadDataTableFromCsvString(LocalCopyOfFileContentFromRemoteUri, includesColumnHeaders, columnSeparator, recognizeTextBy, recognizeMultipleColumnSeparatorCharsAsOne, convertEmptyStringsToDBNull)
                Result.TableName = System.IO.Path.GetFileNameWithoutExtension(path)
                Return Result
            Else
                Throw New System.IO.FileNotFoundException("File not found", path)
            End If

            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(path, System.Text.Encoding.GetEncoding(encoding))
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, System.Globalization.CultureInfo.CurrentCulture, columnSeparator, recognizeTextBy, recognizeMultipleColumnSeparatorCharsAsOne, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV file
        ''' </summary>
        ''' <param name="Path">The path of the file</param>
        ''' <param name="IncludesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="Encoding">The text encoding of the file</param>
        ''' <param name="cultureFormatProvider"></param>
        ''' <param name="RecognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="recognizeMultipleColumnSeparatorCharsAsOne">Currently without purpose</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvFile(path As String, includesColumnHeaders As Boolean, encoding As System.Text.Encoding, cultureFormatProvider As System.Globalization.CultureInfo, recognizeTextBy As Char, recognizeMultipleColumnSeparatorCharsAsOne As Boolean, convertEmptyStringsToDBNull As Boolean) As DataTable

            Dim Result As New DataTable

            If File.Exists(path) Then
            ElseIf path.ToLower.StartsWith("http://") OrElse path.ToLower.StartsWith("https://") Then
                Dim LocalCopyOfFileContentFromRemoteUri As String = Utils.ReadStringDataFromUri(path, encoding.WebName)
                Result = ReadDataTableFromCsvString(LocalCopyOfFileContentFromRemoteUri, includesColumnHeaders, cultureFormatProvider, recognizeTextBy, recognizeMultipleColumnSeparatorCharsAsOne, convertEmptyStringsToDBNull)
                Result.TableName = System.IO.Path.GetFileNameWithoutExtension(path)
                Return Result
            Else
                Throw New System.IO.FileNotFoundException("File not found", path)
            End If

            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(path, encoding)
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, cultureFormatProvider, Nothing, recognizeTextBy, recognizeMultipleColumnSeparatorCharsAsOne, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV table
        ''' </summary>
        ''' <param name="reader">A stream reader targetting CSV data</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="recognizeDoubledColumnSeparatorCharAsOne">Currently without purpose</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Private Shared Function ReadDataTableFromCsvReader(ByVal reader As StreamReader, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal columnSeparator As Char = Nothing, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal recognizeDoubledColumnSeparatorCharAsOne As Boolean = True, Optional ByVal convertEmptyStringsToDBNull As Boolean = False) As DataTable

            If cultureFormatProvider Is Nothing Then
                cultureFormatProvider = System.Globalization.CultureInfo.InvariantCulture
            End If

            If columnSeparator = Nothing Then
                'Attention: list separator is a string, but columnSeparator is implemented as char! Might be a bug in some specal cultures
                If cultureFormatProvider.TextInfo.ListSeparator.Length > 1 Then
                    Throw New NotSupportedException("No column separator has been defined and the current culture declares a list separator with more than 1 character. Column separators with more than 1 characters are currenlty not supported.")
                End If
                columnSeparator = cultureFormatProvider.TextInfo.ListSeparator.Chars(0)
            End If

            Dim Result As New DataTable
            Dim rdStr As String
            Dim RowCounter As Integer

            'Read file content
            rdStr = reader.ReadToEnd
            If rdStr = Nothing Then
                'simply return the empty table when there is no input data
                Return Result
            End If

            'Read the file char by char and add row by row
            Dim CharPosition As Integer = 0
            While CharPosition < rdStr.Length

                'Read the next csv row
                Dim ColValues As New ArrayList
                SplitCsvLineIntoCellValues(rdStr, ColValues, CharPosition, columnSeparator, recognizeTextBy, recognizeDoubledColumnSeparatorCharAsOne)

                'Add it as a new data row (respectively add the columns definition)
                RowCounter += 1
                If RowCounter = 1 AndAlso includesColumnHeaders Then
                    'Read first line as column names
                    For ColCounter As Integer = 0 To ColValues.Count - 1
                        Dim colName As String = Trim(CType(ColValues(ColCounter), String))
                        Result.Columns.Add(New DataColumn(colName, GetType(String)))
                    Next
                Else
                    'Read line as data and automatically add required additional columns on the fly
                    Dim MyRow As DataRow = Result.NewRow
                    For ColCounter As Integer = 0 To ColValues.Count - 1
                        Dim colValue As String = Trim(CType(ColValues(ColCounter), String))
                        If Result.Columns.Count <= ColCounter Then
                            Result.Columns.Add(New DataColumn(Nothing, GetType(String)))
                        End If
                        MyRow(ColCounter) = colValue
                    Next
                    Result.Rows.Add(MyRow)
                End If

            End While

            If convertEmptyStringsToDBNull Then
                ConvertEmptyStringsToDBNullValue(Result)
            Else
                ConvertDBNullValuesToEmptyStrings(Result)
            End If

            Return Result

        End Function
        ''' <summary>
        '''     Split a line content into separate column values and add them to the output list
        ''' </summary>
        ''' <param name="lineContent">The line content as it has been read from the CSV file</param>
        ''' <param name="outputList">An array list which shall hold the separated column values</param>
        ''' <param name="startposition"></param>
        ''' <param name="columnSeparator"></param>
        ''' <param name="recognizeTextBy"></param>
        ''' <param name="recognizeDoubledColumnSeparatorCharAsOne"></param>
        Private Shared Sub SplitCsvLineIntoCellValues(ByRef lineContent As String, ByVal outputList As ArrayList, ByRef startposition As Integer, ByVal columnSeparator As Char, ByVal recognizeTextBy As Char, ByVal recognizeDoubledColumnSeparatorCharAsOne As Boolean)

            Dim CurrentColumnValue As New System.Text.StringBuilder
            Dim InQuotationMarks As Boolean
            Dim CharPositionCounter As Integer

            For CharPositionCounter = startposition To lineContent.Length - 1
                Select Case lineContent.Chars(CharPositionCounter)
                    Case columnSeparator
                        If InQuotationMarks Then
                            'just add the character as it is because it's inside of a cell text
                            CurrentColumnValue.Append(lineContent.Chars(CharPositionCounter))
                        Else
                            'now it's a column separator
                            ''ToDo: implement the handling of recognizeDoubledColumnSeparatorCharAsOne as Excel does (double means multiple?)
                            'If recognizeDoubledColumnSeparatorCharAsOne = True Then
                            '    'undefined behaviour, currently adding them as 2 chars again
                            '    CurrentColumnValue.Append("""""")
                            '    'fix the position to be now after the second quotation marks
                            '    CharPositionCounter += 1
                            'End If
                            'Add previously collected data as column value
                            outputList.Add(CurrentColumnValue.ToString)
                            CurrentColumnValue = New System.Text.StringBuilder
                        End If
                    Case recognizeTextBy
                        If InQuotationMarks = False Then
                            InQuotationMarks = Not InQuotationMarks
                        Else
                            'Switch between state of in- our out-of quotation marks
                            If CharPositionCounter + 1 < lineContent.Length AndAlso lineContent.Chars(CharPositionCounter + 1) = recognizeTextBy Then
                                'doubled quotation marks lead to one single quotation mark
                                CurrentColumnValue.Append("""")
                                'fix the position to be now after the second quotation marks
                                CharPositionCounter += 1
                            Else
                                InQuotationMarks = Not InQuotationMarks
                            End If
                        End If
                    Case ControlChars.Lf
                        If InQuotationMarks Then
                            'just add the line-break because it's inside of a cell text
                            'but add the line break in the format of the curren platform
                            CurrentColumnValue.Append(System.Environment.NewLine)
                        Else
                            'now it's a line separator
                            'Add previously collected data as column value
                            outputList.Add(CurrentColumnValue.ToString)
                            CurrentColumnValue = New System.Text.StringBuilder
                            'Leave this method because the reading of one csv row has been completed
                            Exit For
                        End If
                    Case ControlChars.Cr
                        If InQuotationMarks Then
                            'just add the character as it is because it's inside of a cell text
                            CurrentColumnValue.Append(lineContent.Chars(CharPositionCounter))
                        Else
                            'now it's a line separator
                            If CharPositionCounter + 1 < lineContent.Length AndAlso lineContent.Chars(CharPositionCounter + 1) = ControlChars.Lf Then
                                'Found a CrLf occurance; handle it as one line break!
                                CharPositionCounter += 1
                            End If
                            'Add previously collected data as column value
                            outputList.Add(CurrentColumnValue.ToString)
                            CurrentColumnValue = New System.Text.StringBuilder
                            'Leave this method because the reading of one csv row has been completed
                            Exit For
                        End If
                    Case Else
                        'just add the character as it is because it's inside of a cell text
                        CurrentColumnValue.Append(lineContent.Chars(CharPositionCounter))
                End Select
            Next

            'Add the last column value to the collection
            If CurrentColumnValue.Length <> 0 Then
                outputList.Add(CurrentColumnValue.ToString)
            End If

            'Next start position is the next char after the last read one
            startposition = CharPositionCounter + 1

        End Sub
        ''' <summary>
        '''     Read from a CSV table
        ''' </summary>
        ''' <param name="data">The content of a CSV file</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="recognizeDoubledColumnSeparatorCharAsOne">Currently without purpose</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvString(ByVal data As String, ByVal includesColumnHeaders As Boolean, Optional ByVal columnSeparator As Char = Nothing, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal recognizeDoubledColumnSeparatorCharAsOne As Boolean = True, Optional ByVal convertEmptyStringsToDBNull As Boolean = False) As DataTable

            Dim Result As New DataTable
            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(New MemoryStream(System.Text.Encoding.Unicode.GetBytes(data)), System.Text.Encoding.Unicode, False)
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, System.Globalization.CultureInfo.CurrentCulture, columnSeparator, recognizeTextBy, recognizeDoubledColumnSeparatorCharAsOne, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Read from a CSV table
        ''' </summary>
        ''' <param name="data">The content of a CSV file</param>
        ''' <param name="IncludesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="cultureFormatProvider"></param>
        ''' <param name="RecognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="recognizeDoubledColumnSeparatorCharAsOne">Currently without purpose</param>
        ''' <param name="convertEmptyStringsToDBNull">Convert values with empty strings automatically to DbNull</param>
        Friend Shared Function ReadDataTableFromCsvString(ByVal data As String, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal recognizeDoubledColumnSeparatorCharAsOne As Boolean = True, Optional ByVal convertEmptyStringsToDBNull As Boolean = False) As DataTable

            Dim Result As New DataTable
            Dim reader As StreamReader = Nothing
            Try
                reader = New StreamReader(New MemoryStream(System.Text.Encoding.Unicode.GetBytes(data)), System.Text.Encoding.Unicode, False)
                Result = ReadDataTableFromCsvReader(reader, includesColumnHeaders, cultureFormatProvider, Nothing, recognizeTextBy, recognizeDoubledColumnSeparatorCharAsOne, convertEmptyStringsToDBNull)
            Finally
                If Not reader Is Nothing Then
                    reader.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Convert DBNull values to empty strings
        ''' </summary>
        ''' <param name="data">The data which might contain DBNull values</param>
        Private Shared Sub ConvertDBNullValuesToEmptyStrings(ByVal data As DataTable)

            'Parameter validation
            If data Is Nothing Then
                Throw New ArgumentNullException("data")
            End If

            'Ensure that only string columns are here
            For ColCounter As Integer = 0 To data.Columns.Count - 1
                If Not data.Columns(ColCounter).DataType Is GetType(String) Then
                    Throw New Exception("All columns must be of data type System.String")
                End If
            Next

            'Update content
            For RowCounter As Integer = 0 To data.Rows.Count - 1
                Dim MyRow As DataRow = data.Rows(RowCounter)
                For ColCounter As Integer = 0 To data.Columns.Count - 1
                    If MyRow(ColCounter).GetType Is GetType(DBNull) Then
                        MyRow(ColCounter) = ""
                    End If
                Next
            Next

        End Sub
        ''' <summary>
        '''     Convert empty string values to DBNull
        ''' </summary>
        ''' <param name="data">The data which might contain empty strings</param>
        Private Shared Sub ConvertEmptyStringsToDBNullValue(ByVal data As DataTable)

            'Parameter validation
            If data Is Nothing Then
                Throw New ArgumentNullException("data")
            End If

            'Ensure that only string columns are here
            For ColCounter As Integer = 0 To data.Columns.Count - 1
                If Not data.Columns(ColCounter).DataType Is GetType(String) Then
                    Throw New Exception("All columns must be of data type System.String")
                End If
            Next

            'Update content
            For RowCounter As Integer = 0 To data.Rows.Count - 1
                Dim MyRow As DataRow = data.Rows(RowCounter)
                For ColCounter As Integer = 0 To data.Columns.Count - 1
                    Try
                        If MyRow(ColCounter).GetType Is GetType(String) AndAlso CType(MyRow(ColCounter), String) = "" Then
                            MyRow(ColCounter) = DBNull.Value
                        End If
                    Catch
                        'Ignore any conversion errors since we only want to change string columns
                    End Try
                Next
            Next

        End Sub
#End Region

#End Region

#Region "Write data"
        Friend Shared Sub WriteDataTableToCsvFile(ByVal path As String, ByVal dataTable As System.Data.DataTable)
            WriteDataTableToCsvFile(path, dataTable, True, System.Globalization.CultureInfo.InvariantCulture)
        End Sub

        Friend Shared Sub WriteDataTableToCsvFile(ByVal path As String, ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, ByVal columnWidths As Integer(), ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal encoding As String = "UTF-8")

            'Create stream writer
            Dim writer As StreamWriter = Nothing
            Try
                writer = New StreamWriter(path, False, System.Text.Encoding.GetEncoding(encoding))
                writer.Write(ConvertDataTableToCsv(dataTable, includesColumnHeaders, cultureFormatProvider, columnWidths))
            Finally
                If Not writer Is Nothing Then
                    writer.Close()
                End If
            End Try

        End Sub

        Friend Shared Sub WriteDataTableToCsvFile(ByVal path As String, ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c)

            'Create stream writer
            Dim writer As StreamWriter = Nothing
            Try
                writer = New StreamWriter(path, False, System.Text.Encoding.GetEncoding(encoding))
                writer.Write(ConvertDataTableToCsv(dataTable, includesColumnHeaders, cultureFormatProvider, columnSeparator, recognizeTextBy))
            Finally
                If Not writer Is Nothing Then
                    writer.Close()
                End If
            End Try

        End Sub
        ''' <summary>
        '''     Trims a string to exactly the required fix size
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="fixedLengthSize"></param>
        ''' <param name="alignedRight">Add additionally required spaces on the left (True) or on the right (False)</param>
        Private Shared Function FixedLengthText(ByVal text As String, ByVal fixedLengthSize As Integer, ByVal alignedRight As Boolean) As String
            Dim Result As String = Mid(text, 1, fixedLengthSize)
            If Result.Length < fixedLengthSize Then
                'Add some spaces to the string
                If alignedRight = False Then
                    Result &= Strings.Space(fixedLengthSize - Result.Length)
                Else
                    Result = Strings.Space(fixedLengthSize - Result.Length) & Result
                End If
            End If
            Return Result
        End Function
        ''' <summary>
        '''     Convert the datatable to a string based, comma-separated format
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders"></param>
        ''' <param name="cultureFormatProvider"></param>
        ''' <param name="columnWidths"></param>
        Friend Shared Function ConvertDataTableToCsv(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, ByVal columnWidths As Integer()) As String

            If cultureFormatProvider Is Nothing Then
                cultureFormatProvider = System.Globalization.CultureInfo.InvariantCulture
            End If

            Dim writer As New System.Text.StringBuilder

            'Column headers
            If includesColumnHeaders Then
                For ColCounter As Integer = 0 To System.Math.Min(columnWidths.Length, dataTable.Columns.Count) - 1
                    writer.Append(FixedLengthText(dataTable.Columns(ColCounter).ColumnName, columnWidths(ColCounter), False))
                Next
                writer.Append(vbNewLine)
            End If

            'Data values
            For RowCounter As Integer = 0 To dataTable.Rows.Count - 1
                For ColCounter As Integer = 0 To System.Math.Min(columnWidths.Length, dataTable.Columns.Count) - 1
                    If dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                        writer.Append(FixedLengthText(String.Empty, columnWidths(ColCounter), False))
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(String) Then
                        'Strings
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            writer.Append(FixedLengthText(CType(dataTable.Rows(RowCounter)(ColCounter), String), columnWidths(ColCounter), False))
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Double) Then
                        'Doubles
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(FixedLengthText(CType(dataTable.Rows(RowCounter)(ColCounter), Double).ToString(cultureFormatProvider), columnWidths(ColCounter), True))
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Decimal) Then
                        'Decimals
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(FixedLengthText(CType(dataTable.Rows(RowCounter)(ColCounter), Decimal).ToString(cultureFormatProvider), columnWidths(ColCounter), True))
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.DateTime) Then
                        'Datetime
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(FixedLengthText(CType(dataTable.Rows(RowCounter)(ColCounter), DateTime).ToString(cultureFormatProvider), columnWidths(ColCounter), False))
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Int16) OrElse dataTable.Columns(ColCounter).DataType Is GetType(System.Int32) OrElse dataTable.Columns(ColCounter).DataType Is GetType(System.Int64) Then
                        'Datetime
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(FixedLengthText(CType(dataTable.Rows(RowCounter)(ColCounter), System.Int64).ToString(cultureFormatProvider), columnWidths(ColCounter), True))
                        End If
                    Else
                        'Other data types
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(FixedLengthText(CType(dataTable.Rows(RowCounter)(ColCounter), String), columnWidths(ColCounter), False))
                        End If
                    End If
                Next
                writer.Append(vbNewLine)
            Next
            Return writer.ToString

        End Function
        ''' <summary>
        '''     Convert the datatable to a string based, comma-separated format
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders"></param>
        ''' <param name="cultureFormatProvider"></param>
        ''' <param name="columnSeparator"></param>
        ''' <param name="recognizeTextBy"></param>
        Friend Shared Function ConvertDataTableToCsv(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c) As String

            If cultureFormatProvider Is Nothing Then
                cultureFormatProvider = System.Globalization.CultureInfo.InvariantCulture
            End If

            If columnSeparator = Nothing Then
                columnSeparator = cultureFormatProvider.TextInfo.ListSeparator
            End If

            Dim writer As New System.Text.StringBuilder

            'Column headers
            If includesColumnHeaders Then
                For ColCounter As Integer = 0 To dataTable.Columns.Count - 1
                    If ColCounter <> 0 Then
                        writer.Append(columnSeparator)
                    End If
                    writer.Append(recognizeTextBy & dataTable.Columns(ColCounter).ColumnName & recognizeTextBy)
                Next
                writer.Append(vbNewLine)
            End If

            'Data values
            For RowCounter As Integer = 0 To dataTable.Rows.Count - 1
                For ColCounter As Integer = 0 To dataTable.Columns.Count - 1
                    If ColCounter <> 0 Then
                        writer.Append(columnSeparator)
                    End If
                    If dataTable.Columns(ColCounter).DataType Is GetType(String) Then
                        'Strings
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            writer.Append(recognizeTextBy & CType(dataTable.Rows(RowCounter)(ColCounter), String).Replace(recognizeTextBy, recognizeTextBy & recognizeTextBy) & recognizeTextBy)
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Double) Then
                        'Doubles
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(CType(dataTable.Rows(RowCounter)(ColCounter), Double).ToString(cultureFormatProvider))
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Decimal) Then
                        'Decimals
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(CType(dataTable.Rows(RowCounter)(ColCounter), Decimal).ToString(cultureFormatProvider))
                        End If
                    ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.DateTime) Then
                        'Datetime
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(CType(dataTable.Rows(RowCounter)(ColCounter), DateTime).ToString(cultureFormatProvider))
                        End If
                    Else
                        'Other data types
                        If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                            'Other data types which do not require textual handling
                            writer.Append(CType(dataTable.Rows(RowCounter)(ColCounter), String))
                        End If
                    End If
                Next
                writer.Append(vbNewLine)
            Next
            Return writer.ToString

        End Function
        ''' <summary>
        '''     Write to a CSV file
        ''' </summary>
        ''' <param name="path">The path of the file</param>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="decimalSeparator">A character indicating the decimal separator in the text string</param>
        Friend Shared Sub WriteDataTableToCsvFile(ByVal path As String, ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal decimalSeparator As Char = "."c)

            Dim cultureFormatProvider As New System.Globalization.CultureInfo("")
            cultureFormatProvider.NumberFormat.CurrencyDecimalSeparator = decimalSeparator
            cultureFormatProvider.NumberFormat.NumberDecimalSeparator = decimalSeparator
            cultureFormatProvider.NumberFormat.PercentDecimalSeparator = decimalSeparator

            'Create stream writer
            Dim writer As StreamWriter = Nothing
            Try
                writer = New StreamWriter(path, False, System.Text.Encoding.GetEncoding(encoding))

                'Column headers
                If includesColumnHeaders Then
                    For ColCounter As Integer = 0 To dataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        writer.Write(recognizeTextBy & dataTable.Columns(ColCounter).ColumnName & recognizeTextBy)
                    Next
                    writer.WriteLine()
                End If

                'Data values
                For RowCounter As Integer = 0 To dataTable.Rows.Count - 1
                    For ColCounter As Integer = 0 To dataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        If dataTable.Columns(ColCounter).DataType Is GetType(String) Then
                            'Strings
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                writer.Write(recognizeTextBy & CType(dataTable.Rows(RowCounter)(ColCounter), String).Replace(recognizeTextBy, recognizeTextBy & recognizeTextBy) & recognizeTextBy)
                            End If
                        ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Double) Then
                            'Doubles
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), Double).ToString(cultureFormatProvider))
                            End If
                        ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Decimal) Then
                            'Decimals
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), Decimal).ToString(cultureFormatProvider))
                            End If
                        ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.DateTime) Then
                            'Datetime
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), DateTime).ToString(cultureFormatProvider))
                            End If
                        Else
                            'Other data types
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), String))
                            End If
                        End If
                    Next
                    writer.WriteLine()
                Next

            Finally
                If Not writer Is Nothing Then
                    writer.Close()
                End If
            End Try

        End Sub
        ''' <summary>
        '''     Create a CSV table
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="decimalSeparator"></param>
        ''' <returns>A string containing the CSV table</returns>
        Friend Shared Function WriteDataTableToCsvString(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As Char = ","c, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal decimalSeparator As Char = "."c) As String
            Dim MyStream As MemoryStream = WriteDataTableToCsvMemoryStream(dataTable, includesColumnHeaders, System.Text.Encoding.Unicode.EncodingName, columnSeparator, recognizeTextBy, decimalSeparator)
            Return System.Text.Encoding.Unicode.GetString(MyStream.ToArray)
        End Function
        ''' <summary>
        '''     Create a CSV table
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="decimalSeparator"></param>
        ''' <returns>A string containing the CSV table</returns>
        Friend Shared Function WriteDataTableToCsvBytes(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As Char = ","c, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal decimalSeparator As Char = "."c) As Byte()
            Dim MyStream As MemoryStream = WriteDataTableToCsvMemoryStream(dataTable, includesColumnHeaders, encoding, columnSeparator, recognizeTextBy, decimalSeparator)
            Return MyStream.ToArray
        End Function
        ''' <summary>
        '''     Create a CSV table
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="cultureFormatProvider">A globalization information object for the conversion of all data to strings</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <returns>A string containing the CSV table</returns>
        Friend Shared Function WriteDataTableToCsvBytes(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, ByVal encoding As System.Text.Encoding, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal columnSeparator As Char = ","c, Optional ByVal recognizeTextBy As Char = """"c) As Byte()
            Dim MyStream As MemoryStream = WriteDataTableToCsvMemoryStream(dataTable, includesColumnHeaders, encoding, cultureFormatProvider, columnSeparator, recognizeTextBy)
            Return MyStream.ToArray
        End Function
        ''' <summary>
        '''     Create a CSV table
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <param name="decimalSeparator"></param>
        ''' <returns>A memory stream containing all texts as bytes in Unicode format</returns>
        Friend Shared Function WriteDataTableToCsvMemoryStream(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal decimalSeparator As Char = "."c) As System.IO.MemoryStream
            Dim cultureFormatProvider As System.Globalization.CultureInfo = CType(System.Globalization.CultureInfo.InvariantCulture.Clone, System.Globalization.CultureInfo)
            cultureFormatProvider.NumberFormat.CurrencyDecimalSeparator = decimalSeparator
            cultureFormatProvider.NumberFormat.NumberDecimalSeparator = decimalSeparator
            cultureFormatProvider.NumberFormat.PercentDecimalSeparator = decimalSeparator
            Return WriteDataTableToCsvMemoryStream(dataTable, includesColumnHeaders, System.Text.Encoding.GetEncoding(encoding), cultureFormatProvider, columnSeparator, recognizeTextBy)
        End Function
        ''' <summary>
        '''     Create a CSV table
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="cultureFormatProvider">A globalization information object for the conversion of all data to strings</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        ''' <returns>A memory stream containing all texts as bytes in Unicode format</returns>
        Friend Shared Function WriteDataTableToCsvMemoryStream(ByVal dataTable As System.Data.DataTable, ByVal includesColumnHeaders As Boolean, ByVal encoding As System.Text.Encoding, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c) As System.IO.MemoryStream

            If cultureFormatProvider Is Nothing Then
                cultureFormatProvider = System.Globalization.CultureInfo.InvariantCulture
            End If

            If columnSeparator = Nothing Then
                columnSeparator = cultureFormatProvider.TextInfo.ListSeparator
            End If

            'Create stream writer
            Dim Result As New MemoryStream
            Dim writer As StreamWriter = Nothing
            Try
                writer = New StreamWriter(Result, encoding)

                'Column headers
                If includesColumnHeaders Then
                    For ColCounter As Integer = 0 To dataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        writer.Write(recognizeTextBy & CsvEncode(dataTable.Columns(ColCounter).ColumnName, recognizeTextBy) & recognizeTextBy)
                    Next
                    writer.WriteLine()
                End If

                'Data values
                For RowCounter As Integer = 0 To dataTable.Rows.Count - 1
                    For ColCounter As Integer = 0 To dataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        If dataTable.Columns(ColCounter).DataType Is GetType(String) Then
                            'Strings
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                writer.Write(recognizeTextBy & CsvEncode(CType(dataTable.Rows(RowCounter)(ColCounter), String), recognizeTextBy) & recognizeTextBy)
                            End If
                        ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Double) Then
                            'Doubles
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), Double).ToString(cultureFormatProvider))
                            End If
                        ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.Decimal) Then
                            'Decimals
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), Decimal).ToString(cultureFormatProvider))
                            End If
                        ElseIf dataTable.Columns(ColCounter).DataType Is GetType(System.DateTime) Then
                            'Datetime
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), DateTime).ToString(cultureFormatProvider))
                            End If
                        Else
                            'Other data types
                            If Not dataTable.Rows(RowCounter)(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataTable.Rows(RowCounter)(ColCounter), String))
                            End If
                        End If
                    Next
                    writer.WriteLine()
                Next

            Finally
                If Not writer Is Nothing Then
                    writer.Close()
                End If
            End Try

            Return Result

        End Function
        ''' <summary>
        '''     Encode a string into CSV encoding
        ''' </summary>
        ''' <param name="value">The unencoded text</param>
        ''' <param name="recognizeTextBy">The character to identify a string in the CSV file</param>
        ''' <returns>The encoded writing style of the given text</returns>
        Private Shared Function CsvEncode(ByVal value As String, ByVal recognizeTextBy As Char) As String
            Dim Result As String
            Result = Replace(value, recognizeTextBy, recognizeTextBy & recognizeTextBy)
            Result = Replace(value, ControlChars.CrLf, ControlChars.Lf)
            Result = Replace(value, ControlChars.Cr, ControlChars.Lf)
            Return Result
        End Function

        Friend Shared Sub WriteDataViewToCsvFile(ByVal path As String, ByVal dataview As System.Data.DataView)
            WriteDataViewToCsvFile(path, dataview, True, System.Globalization.CultureInfo.InvariantCulture)
        End Sub

        Friend Shared Sub WriteDataViewToCsvFile(ByVal path As String, ByVal dataView As System.Data.DataView, ByVal includesColumnHeaders As Boolean, ByVal cultureFormatProvider As System.Globalization.CultureInfo, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c)

            Dim DataTable As System.Data.DataTable = dataView.Table

            If cultureFormatProvider Is Nothing Then
                cultureFormatProvider = System.Globalization.CultureInfo.InvariantCulture
            End If

            If columnSeparator = Nothing Then
                columnSeparator = cultureFormatProvider.TextInfo.ListSeparator
            End If

            'Create stream writer
            Dim writer As StreamWriter = Nothing
            Try
                writer = New StreamWriter(path, False, System.Text.Encoding.GetEncoding(encoding))

                'Column headers
                If includesColumnHeaders Then
                    For ColCounter As Integer = 0 To DataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        writer.Write(recognizeTextBy & DataTable.Columns(ColCounter).ColumnName & recognizeTextBy)
                    Next
                    writer.WriteLine()
                End If

                'Data values
                For RowCounter As Integer = 0 To dataView.Count - 1
                    For ColCounter As Integer = 0 To DataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        If DataTable.Columns(ColCounter).DataType Is GetType(String) Then
                            'Strings
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                writer.Write(recognizeTextBy & CType(dataView.Item(RowCounter).Row(ColCounter), String).Replace(recognizeTextBy, recognizeTextBy & recognizeTextBy) & recognizeTextBy)
                            End If
                        ElseIf DataTable.Columns(ColCounter).DataType Is GetType(System.Double) Then
                            'Doubles
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), Double).ToString(cultureFormatProvider))
                            End If
                        ElseIf DataTable.Columns(ColCounter).DataType Is GetType(System.Decimal) Then
                            'Decimals
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), Decimal).ToString(cultureFormatProvider))
                            End If
                        ElseIf DataTable.Columns(ColCounter).DataType Is GetType(System.DateTime) Then
                            'Datetime
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), DateTime).ToString(cultureFormatProvider))
                            End If
                        Else
                            'Other data types
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), String))
                            End If
                        End If
                    Next
                Next

            Finally
                If Not writer Is Nothing Then
                    writer.Close()
                End If
            End Try

        End Sub
        ''' <summary>
        '''     Write to a CSV file
        ''' </summary>
        ''' <param name="path">The path of the file</param>
        ''' <param name="dataView">A dataview object with the desired rows</param>
        ''' <param name="includesColumnHeaders">Indicates wether column headers are present</param>
        ''' <param name="encoding">The text encoding of the file</param>
        ''' <param name="columnSeparator">Choose the required character for splitting the columns. Set to null (Nothing in VisualBasic) to enable fixed column widths mode</param>
        ''' <param name="recognizeTextBy">A character indicating the start and end of text strings</param>
        Friend Shared Sub WriteDataViewToCsvFile(ByVal path As String, ByVal dataView As System.Data.DataView, ByVal includesColumnHeaders As Boolean, Optional ByVal encoding As String = "UTF-8", Optional ByVal columnSeparator As String = ","c, Optional ByVal recognizeTextBy As Char = """"c, Optional ByVal decimalSeparator As Char = "."c)

            Dim DataTable As System.Data.DataTable = dataView.Table

            Dim cultureFormatProvider As New System.Globalization.CultureInfo("")
            cultureFormatProvider.NumberFormat.CurrencyDecimalSeparator = decimalSeparator
            cultureFormatProvider.NumberFormat.NumberDecimalSeparator = decimalSeparator
            cultureFormatProvider.NumberFormat.PercentDecimalSeparator = decimalSeparator

            'Create stream writer
            Dim writer As StreamWriter = Nothing
            Try
                writer = New StreamWriter(path, False, System.Text.Encoding.GetEncoding(encoding))

                'Column headers
                If includesColumnHeaders Then
                    For ColCounter As Integer = 0 To DataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        writer.Write(recognizeTextBy & DataTable.Columns(ColCounter).ColumnName & recognizeTextBy)
                    Next
                    writer.WriteLine()
                End If

                'Data values
                For RowCounter As Integer = 0 To dataView.Count - 1
                    For ColCounter As Integer = 0 To DataTable.Columns.Count - 1
                        If ColCounter <> 0 Then
                            writer.Write(columnSeparator)
                        End If
                        If DataTable.Columns(ColCounter).DataType Is GetType(String) Then
                            'Strings
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                writer.Write(recognizeTextBy & CType(dataView.Item(RowCounter).Row(ColCounter), String).Replace(recognizeTextBy, recognizeTextBy & recognizeTextBy) & recognizeTextBy)
                            End If
                        ElseIf DataTable.Columns(ColCounter).DataType Is GetType(System.Double) Then
                            'Doubles
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), Double).ToString(cultureFormatProvider))
                            End If
                        ElseIf DataTable.Columns(ColCounter).DataType Is GetType(System.Decimal) Then
                            'Decimals
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), Decimal).ToString(cultureFormatProvider))
                            End If
                        ElseIf DataTable.Columns(ColCounter).DataType Is GetType(System.DateTime) Then
                            'Datetime
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), DateTime).ToString(cultureFormatProvider))
                            End If
                        Else
                            'Other data types
                            If Not dataView.Item(RowCounter).Row(ColCounter) Is DBNull.Value Then
                                'Other data types which do not require textual handling
                                writer.Write(CType(dataView.Item(RowCounter).Row(ColCounter), String))
                            End If
                        End If
                    Next
                    writer.WriteLine()
                Next

            Finally
                If Not writer Is Nothing Then
                    writer.Close()
                End If
            End Try

        End Sub

#End Region

    End Class

    ''' <summary>
    '''     Common DataTable operations
    ''' </summary>
    Friend Class DataTables

        ''' <summary>
        ''' Copy the values of a data column into an arraylist
        ''' </summary>
        ''' <param name="column">The column which contains the data</param>
        ''' <returns>An array containing data with type of the column's datatype OR with type of DBNull</returns>
        ''' <remarks></remarks>
        Public Shared Function ConvertColumnValuesIntoList(Of T)(ByVal column As DataColumn) As Generic.List(Of T)
            Return ConvertDataTableToList(Of T)(column.Table, column.Ordinal)
        End Function

        ''' <summary>
        '''     Convert a data table column to a generic list (except DBNull values)
        ''' </summary>
        ''' <param name="column">The column which shall be used to fill the arraylist</param>
        ''' <returns>An array containing data with type of the column's datatype OR with type of DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Function ConvertDataTableToList(Of T)(ByVal column As DataColumn) As Generic.List(Of T)
            Return ConvertDataTableToList(Of T)(column.Table, column.Ordinal)
        End Function

        ''' <summary>
        '''     Convert a data table column to a generic list (except DBNull values)
        ''' </summary>
        ''' <param name="data">The first column of this data table will be used</param>
        ''' <returns>An array containing data with type of the column's datatype OR with type of DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Function ConvertDataTableToList(Of T)(ByVal data As DataTable) As Generic.List(Of T)
            Return ConvertDataTableToList(Of T)(data, 0)
        End Function

        ''' <summary>
        '''     Convert a data table column to a generic list (except DBNull values)
        ''' </summary>
        ''' <param name="data">The data table with the content</param>
        ''' <param name="selectedColumnIndex">The column which shall be used to fill the arraylist</param>
        ''' <returns>An array containing data with type of the column's datatype OR with type of DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        Public Shared Function ConvertDataTableToList(Of T)(ByVal data As DataTable, ByVal selectedColumnIndex As Integer) As Generic.List(Of T)
            Dim Result As New System.Collections.Generic.List(Of T)
            For MyCounter As Integer = 0 To data.Rows.Count - 1
                If Not IsDBNull(data.Rows(MyCounter)(selectedColumnIndex)) Then
                    Result.Add(CType(data.Rows(MyCounter)(selectedColumnIndex), T))
                End If
            Next
            Return Result
        End Function

        ''' <summary>
        ''' Remove rows with duplicate values in a given column
        ''' </summary>
        ''' <param name="dataTable">A datatable with duplicate values</param>
        ''' <param name="columnName">Column name of the datatable which contains the duplicate values</param>
        ''' <returns>A datatable with unique records in the specified column</returns>
        Friend Shared Function RemoveDuplicates(ByVal dataTable As DataTable, ByVal columnName As String) As DataTable
            Dim hTable As New Hashtable
            Dim duplicateList As New ArrayList

            'Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            'And add duplicate item value in arraylist.
            Dim drow As DataRow
            For Each drow In dataTable.Rows
                If hTable.Contains(drow(columnName)) Then
                    duplicateList.Add(drow)
                Else
                    hTable.Add(drow(columnName), String.Empty)
                End If
            Next drow
            'Removing a list of duplicate items from datatable.
            Dim daRow As DataRow
            For Each daRow In duplicateList
                dataTable.Rows.Remove(daRow)
            Next daRow
            'Datatable which contains unique records will be return as output.
            Return dataTable
        End Function 'RemoveDuplicateRows
        ''' <summary>
        '''     Drop all columns except the required ones
        ''' </summary>
        ''' <param name="table">A data table containing some columns</param>
        ''' <param name="remainingColumns">A list of column names which shall not be removed</param>
        ''' <remarks>
        '''     If the list of the remaining columns contains some column names which are not existing, then those column names will be ignored. There will be no exception in this case.
        '''     The names of the columns are handled case-insensitive.
        ''' </remarks>
        Friend Shared Sub KeepColumnsAndRemoveAllOthers(ByVal table As DataTable, ByVal remainingColumns As String())
            Dim KeepColFlags(table.Columns.Count - 1) As Boolean
            'Identify unwanted columns
            For MyKeepColCounter As Integer = 0 To remainingColumns.Length - 1
                If remainingColumns(MyKeepColCounter) <> Nothing Then
                    For MyColCounter As Integer = 0 To table.Columns.Count - 1
                        If table.Columns(remainingColumns(MyKeepColCounter)) Is table.Columns(MyColCounter) Then
                            KeepColFlags(MyColCounter) = True
                        End If
                    Next
                End If
            Next
            'Remove unwanted columns
            For MyCounter As Integer = KeepColFlags.Length - 1 To 0 Step -1
                If KeepColFlags(MyCounter) = False Then
                    table.Columns.RemoveAt(MyCounter)
                End If
            Next
        End Sub
        ''' <summary>
        '''     Lookup the row index for a data row in a data table
        ''' </summary>
        ''' <param name="dataRow">The data row whose index number is required</param>
        ''' <returns>An index number for the given data row</returns>
        Friend Shared Function RowIndex(ByVal dataRow As DataRow) As Integer
            If dataRow.Table Is Nothing Then
                Throw New Exception("Datarow must be part of a table to retrieve its row index")
            End If
            For MyCounter As Integer = 0 To dataRow.Table.Rows.Count - 1
                If dataRow.Table.Rows(MyCounter) Is dataRow Then
                    Return MyCounter
                End If
            Next
            Throw New Exception("Unexpected error: provided data row can't be identified in its data table. Please contact your software vendor.")
        End Function
        ''' <summary>
        '''     Find duplicate values in a given row and calculate the number of occurances of each value in the table
        ''' </summary>
        ''' <param name="column">A column of a datatable</param>
        ''' <returns>A hashtable containing the origin column value as key and the number of occurances as value</returns>
        Friend Shared Function FindDuplicates(ByVal column As DataColumn) As Hashtable
            Return FindDuplicates(column, 2)
        End Function
        ''' <summary>
        '''     Find duplicate values in a given row and calculate the number of occurances of each value in the table
        ''' </summary>
        ''' <param name="column">A column of a datatable</param>
        ''' <param name="minOccurances">Only values with occurances equal or more than this number will be returned</param>
        ''' <returns>A hashtable containing the origin column value as key and the number of occurances as value</returns>
        Friend Shared Function FindDuplicates(ByVal column As DataColumn, ByVal minOccurances As Integer) As Hashtable

            Dim Table As DataTable = column.Table
            Dim Result As New Hashtable

            'Find all elements and count their duplicates number
            For MyCounter As Integer = 0 To Table.Rows.Count - 1
                Dim key As Object = Table.Rows(MyCounter)(column)
                If Result.ContainsKey(key) Then
                    'Increase counter for this existing value by 1
                    Result.Item(key) = CType(Result.Item(key), Integer) + 1
                Else
                    'Add new element
                    Result.Add(key, 1)
                End If
            Next

            'Remove all elements with occurances lesser than the required number
            Dim removeTheseKeys As New ArrayList
            For Each MyKey As DictionaryEntry In Result
                If CType(MyKey.Value, Integer) < minOccurances Then
                    removeTheseKeys.Add(MyKey.Key)
                End If
            Next
            For MyCounter As Integer = 0 To removeTheseKeys.Count - 1
                Result.Remove(removeTheseKeys(MyCounter))
            Next

            Return Result

        End Function
        ''' <summary>
        '''     Convert the first two columns into objects which can be consumed by the ListControl objects in the System.Windows.Forms namespaces
        ''' </summary>
        ''' <param name="datatable">The datatable which contains a key column and a value column for the list control</param>
        ''' <returns>An array of WinFormsListControlItem</returns>
        Friend Shared Function ConvertDataTableToWinFormsListControlItem(ByVal datatable As DataTable) As WinFormsListControlItem()
            If datatable Is Nothing Then
                Return Nothing
            ElseIf datatable.Rows.Count = 0 Then
                Return New WinFormsListControlItem() {}
            Else
                Dim Result As WinFormsListControlItem()
                ReDim Result(datatable.Rows.Count - 1)
                For MyCounter As Integer = 0 To datatable.Rows.Count - 1
                    Result(MyCounter) = New WinFormsListControlItem
                    Result(MyCounter).Key = datatable.Rows(MyCounter)(0)
                    Result(MyCounter).Value = datatable.Rows(MyCounter)(1)
                Next
                Return Result
            End If
        End Function

        Friend Class WinFormsListControlItem

            Private _Key As Object
            Public Property Key() As Object
                Get
                    Return _Key
                End Get
                Set(ByVal Value As Object)
                    _Key = Value
                End Set
            End Property

            Public Overrides Function ToString() As String
                If Value Is Nothing Then
                    Return String.Empty
                Else
                    Return Value.ToString
                End If
            End Function

            Private _Value As Object
            Public Property Value() As Object
                Get
                    Return _Value
                End Get
                Set(ByVal Value As Object)
                    _Value = Value
                End Set
            End Property
        End Class
        ''' <summary>
        '''     Convert a dataset to an xml string with data and schema information
        ''' </summary>
        ''' <param name="dataset"></param>
        Friend Shared Function ConvertDatasetToXml(ByVal dataset As DataSet) As String
            Dim sbuilder As New System.Text.StringBuilder
            Dim xmlSW As System.IO.StringWriter = New System.IO.StringWriter(sbuilder)
            dataset.WriteXml(xmlSW, XmlWriteMode.WriteSchema)
            xmlSW.Close()
            Return sbuilder.ToString
        End Function
        ''' <summary>
        '''     Convert an xml string to a dataset
        ''' </summary>
        ''' <param name="xml"></param>
        Friend Shared Function ConvertXmlToDataset(ByVal xml As String) As DataSet
            Dim reader As System.IO.StringReader = New System.IO.StringReader(xml)
            Dim DataSet As New DataSet
            DataSet.ReadXml(reader, XmlReadMode.Auto)
            reader.Close()
            Return DataSet
        End Function
        ''' <summary>
        '''     Create a new data table clone with only some first rows
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="NumberOfRows">The number of rows to be copied</param>
        ''' <returns>The new clone of the datatable</returns>
        Friend Shared Function GetDataTableWithSubsetOfRows(ByVal SourceTable As DataTable, ByVal NumberOfRows As Integer) As DataTable
            Return GetDataTableWithSubsetOfRows(SourceTable, 0, NumberOfRows)
        End Function
        ''' <summary>
        '''     Create a new data table clone with only some first rows
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="StartAtRow">The position where to start the copy process, the first row is at 0</param>
        ''' <param name="NumberOfRows">The number of rows to be copied</param>
        ''' <returns>The new clone of the datatable</returns>
        Friend Shared Function GetDataTableWithSubsetOfRows(ByVal SourceTable As DataTable, ByVal StartAtRow As Integer, ByVal NumberOfRows As Integer) As DataTable
            Dim Result As DataTable = SourceTable.Clone
            Dim MyRows As DataRowCollection = SourceTable.Rows
            Dim LastRowIndex As Integer

            If NumberOfRows = Integer.MaxValue Then
                'Read to end
                LastRowIndex = MyRows.Count - 1
            Else
                'Read only the given number of rows
                LastRowIndex = StartAtRow + NumberOfRows - 1
                'Verify that we're not going to read more rows than existant
                If LastRowIndex >= MyRows.Count Then
                    LastRowIndex = MyRows.Count - 1
                End If
            End If

            If Not MyRows Is Nothing AndAlso MyRows.Count > 0 Then
                For MyRowCounter As Integer = StartAtRow To LastRowIndex
                    Dim MyNewRow As DataRow = Result.NewRow
                    MyNewRow.ItemArray = MyRows(MyRowCounter).ItemArray
                    Result.Rows.Add(MyNewRow)
                Next
            End If

            Return Result

        End Function
        ''' <summary>
        '''     Remove those rows in the source column which haven't got the same value in the compare table
        ''' </summary>
        ''' <param name="sourceColumn">This is the column of the master table where all operations shall be executed</param>
        ''' <param name="valuesMustExistInThisColumnToKeepTheSourceRow">This is the comparison value against the source table's column</param>
        ''' <returns>An arraylist of removed keys</returns>
        ''' <remarks>
        '''     Strings will be compared case-insensitive, DBNull values in the source table will always be removed
        ''' </remarks>
        Friend Shared Function RemoveRowsWithNoCorrespondingValueInComparisonTable(ByVal sourceColumn As DataColumn, ByVal valuesMustExistInThisColumnToKeepTheSourceRow As DataColumn) As ArrayList
            Return RemoveRowsWithNoCorrespondingValueInComparisonTable(sourceColumn, valuesMustExistInThisColumnToKeepTheSourceRow, True, True)
        End Function
        ''' <summary>
        '''     Remove those rows in the source column which haven't got the same value in the compare table
        ''' </summary>
        ''' <param name="sourceColumn">This is the column of the master table where all operations shall be executed</param>
        ''' <param name="valuesMustExistInThisColumnToKeepTheSourceRow">This is the comparison value against the source table's column</param>
        ''' <param name="ignoreCaseInStrings">Strings will be compared case-insensitive</param>
        ''' <param name="alwaysRemoveDBNullValues">Always remove the source row when it contains a DBNull value</param>
        ''' <returns>An arraylist of removed keys</returns>
        Friend Shared Function RemoveRowsWithNoCorrespondingValueInComparisonTable(ByVal sourceColumn As DataColumn, ByVal valuesMustExistInThisColumnToKeepTheSourceRow As DataColumn, ByVal ignoreCaseInStrings As Boolean, ByVal alwaysRemoveDBNullValues As Boolean) As ArrayList

            'parameters validation
            If sourceColumn Is Nothing Then
                Throw New ArgumentNullException("sourceColumn")
            ElseIf valuesMustExistInThisColumnToKeepTheSourceRow Is Nothing Then
                Throw New ArgumentNullException("valuesMustExistInThisColumnToKeepTheSourceRow")
            ElseIf Not sourceColumn.DataType Is valuesMustExistInThisColumnToKeepTheSourceRow.DataType Then
                Throw New Exception("Data type mismatch")
            End If

            'Prepare local variables
            Dim Result As New ArrayList 'Contains all keys which have been removed
            Dim sourceTable As DataTable = sourceColumn.Table
            Dim comparisonTable As DataTable = valuesMustExistInThisColumnToKeepTheSourceRow.Table

            'Loop through the source table and try to find matches in the comparison table
            For MyCounter As Integer = sourceTable.Rows.Count - 1 To 0 Step -1
                Dim MatchFound As Boolean = False
                If sourceColumn.DataType Is GetType(String) Then
                    'Compare strings
                    For MyCompCounter As Integer = 0 To comparisonTable.Rows.Count - 1
                        If IsDBNull(sourceTable.Rows(MyCounter)(sourceColumn)) Then
                            If alwaysRemoveDBNullValues Then
                                'Remove this line from source table because it contains a DBNull and those rows shall be removed
                                MatchFound = False
                                Exit For
                            Else
                                If IsDBNull(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                                    'This is a match, keep that row!
                                    MatchFound = True
                                    Exit For
                                Else
                                    'Not identical, continue search
                                End If
                            End If
                        ElseIf IsDBNull(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                            'Not identical, continue search
                        ElseIf ignoreCaseInStrings = True AndAlso String.Compare(CType(sourceTable.Rows(MyCounter)(sourceColumn), String), CType(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow), String), True, System.Globalization.CultureInfo.InvariantCulture) = 0 Then
                            'Case insensitive comparison resulted to successful match
                            MatchFound = True
                            Exit For
                        ElseIf ignoreCaseInStrings = False AndAlso String.Compare(CType(sourceTable.Rows(MyCounter)(sourceColumn), String), CType(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow), String), False, System.Globalization.CultureInfo.InvariantCulture) = 0 Then
                            'Case sensitive comparison resulted to successful match
                            MatchFound = True
                            Exit For
                        Else
                            'Not identical, continue search
                        End If
                    Next
                ElseIf sourceColumn.DataType.IsValueType Then
                    'Compare value types
                    For MyCompCounter As Integer = 0 To comparisonTable.Rows.Count - 1
                        If IsDBNull(sourceTable.Rows(MyCounter)(sourceColumn)) Then
                            If alwaysRemoveDBNullValues Then
                                'Remove this line from source table because it contains a DBNull and those rows shall be removed
                                MatchFound = False
                                Exit For
                            Else
                                If IsDBNull(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                                    'This is a match, keep that row!
                                    MatchFound = True
                                    Exit For
                                Else
                                    'Not identical, continue search
                                End If
                            End If
                        ElseIf IsDBNull(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                            'Not identical, continue search
                        ElseIf CType(sourceTable.Rows(MyCounter)(sourceColumn), ValueType).Equals(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                            'Values are equal
                            MatchFound = True
                            Exit For
                        Else
                            'Not identical, continue search
                        End If
                    Next
                ElseIf sourceColumn.DataType.IsValueType = False Then
                    'Compare objects
                    For MyCompCounter As Integer = 0 To comparisonTable.Rows.Count - 1
                        If IsDBNull(sourceTable.Rows(MyCounter)(sourceColumn)) Then
                            If alwaysRemoveDBNullValues Then
                                'Remove this line from source table because it contains a DBNull and those rows shall be removed
                                MatchFound = False
                                Exit For
                            Else
                                If IsDBNull(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                                    'This is a match, keep that row!
                                    MatchFound = True
                                    Exit For
                                Else
                                    'Not identical, continue search
                                End If
                            End If
                        ElseIf IsDBNull(comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow)) Then
                            'Not identical, continue search
                        ElseIf sourceTable.Rows(MyCounter)(sourceColumn) Is comparisonTable.Rows(MyCompCounter)(valuesMustExistInThisColumnToKeepTheSourceRow) Then
                            'Objects are the same
                            MatchFound = True
                            Exit For
                        Else
                            'Not identical, continue search
                        End If
                    Next
                End If
                If MatchFound = False Then
                    'Add the key of the row to the result list
                    Result.Add(sourceTable.Rows(MyCounter)(sourceColumn))
                    'No match found leads to removal of the row in the source table
                    sourceTable.Rows.RemoveAt(MyCounter)
                End If
            Next
            Return Result
        End Function

#If notyetimplemented Then
        ''' <summary>
        '''     Remove those rows in the source column which haven't got the same value in the compare table
        ''' </summary>
        ''' <param name="sourceColumns">These are the columns of the master table where all operations shall be executed</param>
        ''' <param name="valuesMustExistInTheseColumnsToKeepTheSourceRow">These are the comparison values against the source table's columns</param>
        ''' <param name="ignoreCaseInStrings">Strings will be compared case-insensitive</param>
        ''' <param name="alwaysRemoveDBNullValues">Always remove the source row when it contains a DBNull value</param>
        ''' <returns>An arraylist with object arrays containing all key values of a row in the order of the source columns</returns>
        Friend Shared Function RemoveRowsWithNoCorrespondingValueInComparisonTable(ByVal sourceColumns As DataColumn(), ByVal valuesMustExistInTheseColumnsToKeepTheSourceRow As DataColumn(), ByVal ignoreCaseInStrings As Boolean, ByVal alwaysRemoveDBNullValues As Boolean) As ArrayList

            'parameter validation
            If sourceColumns Is Nothing Then
                Throw New ArgumentNullException("sourceColumns")
            ElseIf valuesMustExistInTheseColumnsToKeepTheSourceRow Is Nothing Then
                Throw New ArgumentNullException("valuesMustExistInTheseColumnsToKeepTheSourceRow")
            ElseIf sourceColumns.Length <> valuesMustExistInTheseColumnsToKeepTheSourceRow.Length Then
                Throw New ArgumentException("Key definition of both tables must contain the same number of keys")
            Else
                'ToDo: additional testings
                '- Are table references of all source columns the same?
                '- Are table references of all comparison columns the same?
                '- Are all keys in the source table matching the same datatype as in the comparison table?
                '- Additional checks see already implemented functions
            End If

            'Attention: result of this function is not an arraylist containing keys!
            '           result of this funciton is an arraylist containing object arrays of keys!

        End Function
#End If
        ''' <summary>
        '''     Creates a complete clone of a DataRow with structure as well as data
        ''' </summary>
        ''' <param name="sourceRow">The source row to be copied</param>
        ''' <returns>The new clone of the DataRow</returns>
        ''' <remarks>
        '''     The resulting DataRow has got the schema from the sourceRow's DataTable, but it hasn't been added to the table yet.
        ''' </remarks>
        Public Shared Function CreateDataRowClone(ByVal sourceRow As DataRow) As DataRow
            If sourceRow Is Nothing Then Throw New ArgumentNullException("sourceRow")
            Dim Result As DataRow = sourceRow.Table.NewRow
            Result.ItemArray = sourceRow.ItemArray
            Return Result
        End Function
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <returns>The new clone of the datatable</returns>
        Friend Shared Function GetDataTableClone(ByVal SourceTable As DataTable) As DataTable
            Return GetDataTableClone(SourceTable, Nothing, Nothing)
        End Function
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="RowFilter">An additional row filter, for all rows set it to null (Nothing in VisualBasic)</param>
        ''' <returns>The new clone of the datatable</returns>
        Friend Shared Function GetDataTableClone(ByVal SourceTable As DataTable, ByVal RowFilter As String) As DataTable
            Return GetDataTableClone(SourceTable, RowFilter, Nothing)
        End Function
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="RowFilter">An additional row filter, for all rows set it to null (Nothing in VisualBasic)</param>
        ''' <param name="Sort">An additional sort command</param>
        ''' <returns>The new clone of the datatable</returns>
        Friend Shared Function GetDataTableClone(ByVal SourceTable As DataTable, ByVal RowFilter As String, ByVal Sort As String) As DataTable
            Return GetDataTableClone(SourceTable, RowFilter, Sort, Nothing)
        End Function
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="RowFilter">An additional row filter, for all rows set it to null (Nothing in VisualBasic)</param>
        ''' <param name="Sort">An additional sort command</param>
        ''' <param name="topRows">How many rows from top shall be returned as maximum?</param>
        ''' <returns>The new clone of the datatable</returns>
        Friend Shared Function GetDataTableClone(ByVal SourceTable As DataTable, ByVal RowFilter As String, ByVal Sort As String, ByVal topRows As Integer) As DataTable
            Dim Result As DataTable = SourceTable.Clone
            Dim MyRows As DataRow() = SourceTable.Select(RowFilter, Sort)

            If topRows = Nothing Then
                'All rows
                topRows = Integer.MaxValue
            End If

            If Not MyRows Is Nothing Then
                For MyCounter As Integer = 1 To MyRows.Length
                    If MyCounter > topRows Then
                        Exit For
                    Else
                        Dim MyNewRow As DataRow = Result.NewRow
                        MyNewRow.ItemArray = MyRows(MyCounter - 1).ItemArray
                        Result.Rows.Add(MyNewRow)
                    End If
                Next
            End If

            Return Result

        End Function
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="sourceTable">The source table to be copied</param>
        ''' <param name="destinationTable">The destination of all operations; the destination table will be a clone of the source table at the end</param>
        ''' <param name="rowFilter">An additional row filter, for all rows set it to null (Nothing in VisualBasic)</param>
        ''' <param name="sort">An additional sort command</param>
        ''' <param name="topRows">How many rows from top shall be returned as maximum?</param>
        ''' <param name="overwritePropertiesOfExistingColumns">Shall the data type or any other settings of an existing table be modified to match the source column's definition?</param>
        ''' <param name="dropExistingRowsInDestinationTable">Remove the existing rows of the destination table, first</param>
        ''' <param name="removeUnusedColumnsFromDestinationTable">Remove the existing columns of the destination table which are not present in the source table</param>
        Friend Shared Sub GetDataTableClone(ByVal sourceTable As DataTable, ByVal destinationTable As DataTable, ByVal rowFilter As String, ByVal sort As String, ByVal topRows As Integer, ByVal overwritePropertiesOfExistingColumns As Boolean, ByVal dropExistingRowsInDestinationTable As Boolean, ByVal removeUnusedColumnsFromDestinationTable As Boolean)

            If dropExistingRowsInDestinationTable Then
                'Drop existing rows
                For MyRowCounter As Integer = destinationTable.Rows.Count - 1 To 0 Step -1
                    destinationTable.Rows(MyRowCounter).Delete()
                Next
            End If

            'Define column set of destination table to column set of source table
            If removeUnusedColumnsFromDestinationTable Then
                '1. Remove columns not more required
                For MyDestTableCounter As Integer = destinationTable.Columns.Count - 1 To 0 Step -1
                    Dim columnExistsInSource As Boolean = False
                    For MySourceTableCounter As Integer = 0 To sourceTable.Columns.Count - 1
                        If destinationTable.Columns(MyDestTableCounter).ColumnName = sourceTable.Columns(MySourceTableCounter).ColumnName Then
                            columnExistsInSource = True
                        End If
                    Next
                    If columnExistsInSource = False Then
                        destinationTable.Columns.RemoveAt(MyDestTableCounter)
                    End If
                Next
            End If
            '2. Update existing, matching columns to be of the same data type
            If overwritePropertiesOfExistingColumns = True Then
                For MyDestTableCounter As Integer = 0 To destinationTable.Columns.Count - 1
                    For MySourceTableCounter As Integer = 0 To sourceTable.Columns.Count - 1
                        If destinationTable.Columns(MyDestTableCounter).ColumnName = sourceTable.Columns(MySourceTableCounter).ColumnName Then
                            destinationTable.Columns(MyDestTableCounter).AllowDBNull = sourceTable.Columns(MySourceTableCounter).AllowDBNull
                            destinationTable.Columns(MyDestTableCounter).AutoIncrement = sourceTable.Columns(MySourceTableCounter).AutoIncrement
                            destinationTable.Columns(MyDestTableCounter).AutoIncrementSeed = sourceTable.Columns(MySourceTableCounter).AutoIncrementSeed
                            destinationTable.Columns(MyDestTableCounter).AutoIncrementStep = sourceTable.Columns(MySourceTableCounter).AutoIncrementStep
                            destinationTable.Columns(MyDestTableCounter).Caption = sourceTable.Columns(MySourceTableCounter).Caption
                            destinationTable.Columns(MyDestTableCounter).ColumnMapping = sourceTable.Columns(MySourceTableCounter).ColumnMapping
                            destinationTable.Columns(MyDestTableCounter).DataType = sourceTable.Columns(MySourceTableCounter).DataType
                            destinationTable.Columns(MyDestTableCounter).DefaultValue = sourceTable.Columns(MySourceTableCounter).DefaultValue
                            destinationTable.Columns(MyDestTableCounter).Expression = sourceTable.Columns(MySourceTableCounter).Expression
                            destinationTable.Columns(MyDestTableCounter).ExtendedProperties.Clear()
                            For Each key As Object In sourceTable.Columns(MySourceTableCounter).ExtendedProperties
                                destinationTable.Columns(MyDestTableCounter).ExtendedProperties.Add(key, sourceTable.Columns(MySourceTableCounter).ExtendedProperties(key))
                            Next
                            destinationTable.Columns(MyDestTableCounter).MaxLength = sourceTable.Columns(MySourceTableCounter).MaxLength
                            destinationTable.Columns(MyDestTableCounter).Namespace = sourceTable.Columns(MySourceTableCounter).Namespace
                            destinationTable.Columns(MyDestTableCounter).Prefix = sourceTable.Columns(MySourceTableCounter).Prefix
                            destinationTable.Columns(MyDestTableCounter).ReadOnly = sourceTable.Columns(MySourceTableCounter).ReadOnly
                            destinationTable.Columns(MyDestTableCounter).Unique = sourceTable.Columns(MySourceTableCounter).Unique
                        End If
                    Next
                Next
            End If
            '3. Add missing columns
            For MySourceTableCounter As Integer = 0 To sourceTable.Columns.Count - 1
                Dim columnExistsInDestination As Boolean = False
                For MyDestTableCounter As Integer = 0 To destinationTable.Columns.Count - 1
                    If destinationTable.Columns(MyDestTableCounter).ColumnName = sourceTable.Columns(MySourceTableCounter).ColumnName Then
                        columnExistsInDestination = True
                    End If
                Next
                If columnExistsInDestination = False Then
                    'Add missing column
                    Dim MyDestTableCounter As Integer 'for the new column index
                    MyDestTableCounter = destinationTable.Columns.Add(sourceTable.Columns(MySourceTableCounter).ColumnName).Ordinal
                    destinationTable.Columns(MyDestTableCounter).AllowDBNull = sourceTable.Columns(MySourceTableCounter).AllowDBNull
                    destinationTable.Columns(MyDestTableCounter).AutoIncrement = sourceTable.Columns(MySourceTableCounter).AutoIncrement
                    destinationTable.Columns(MyDestTableCounter).AutoIncrementSeed = sourceTable.Columns(MySourceTableCounter).AutoIncrementSeed
                    destinationTable.Columns(MyDestTableCounter).AutoIncrementStep = sourceTable.Columns(MySourceTableCounter).AutoIncrementStep
                    destinationTable.Columns(MyDestTableCounter).Caption = sourceTable.Columns(MySourceTableCounter).Caption
                    destinationTable.Columns(MyDestTableCounter).ColumnMapping = sourceTable.Columns(MySourceTableCounter).ColumnMapping
                    destinationTable.Columns(MyDestTableCounter).DataType = sourceTable.Columns(MySourceTableCounter).DataType
                    destinationTable.Columns(MyDestTableCounter).DefaultValue = sourceTable.Columns(MySourceTableCounter).DefaultValue
                    destinationTable.Columns(MyDestTableCounter).Expression = sourceTable.Columns(MySourceTableCounter).Expression
                    destinationTable.Columns(MyDestTableCounter).ExtendedProperties.Clear()
                    For Each key As Object In sourceTable.Columns(MySourceTableCounter).ExtendedProperties
                        destinationTable.Columns(MyDestTableCounter).ExtendedProperties.Add(key, sourceTable.Columns(MySourceTableCounter).ExtendedProperties(key))
                    Next
                    destinationTable.Columns(MyDestTableCounter).MaxLength = sourceTable.Columns(MySourceTableCounter).MaxLength
                    destinationTable.Columns(MyDestTableCounter).Namespace = sourceTable.Columns(MySourceTableCounter).Namespace
                    destinationTable.Columns(MyDestTableCounter).Prefix = sourceTable.Columns(MySourceTableCounter).Prefix
                    destinationTable.Columns(MyDestTableCounter).ReadOnly = sourceTable.Columns(MySourceTableCounter).ReadOnly
                    destinationTable.Columns(MyDestTableCounter).Unique = sourceTable.Columns(MySourceTableCounter).Unique
                End If
            Next

            'Copy related rows from source table to destination table row by row and column by column
            Dim MyRows As DataRow() = sourceTable.Select(rowFilter, sort)

            If topRows = Nothing Then
                'All rows
                topRows = Integer.MaxValue
            End If

            'Copy rows
            If Not MyRows Is Nothing Then
                'Copy row by row
                For MyRowCounter As Integer = 1 To MyRows.Length
                    If MyRowCounter > topRows Then
                        Exit For
                    Else
                        Dim MyNewRow As DataRow = destinationTable.NewRow
                        'Copy column by column
                        For MyColCounter As Integer = 0 To sourceTable.Columns.Count - 1
                            Dim colName As String = sourceTable.Columns(MyColCounter).ColumnName
                            MyNewRow(colName) = sourceTable.Rows(MyRowCounter - 1)(MyColCounter)
                        Next
                        destinationTable.Rows.Add(MyNewRow)
                    End If
                Next
            End If

        End Sub
        ''' <summary>
        '''     Creates a clone of a dataview but as a new data table
        ''' </summary>
        ''' <param name="data">The data view to create the data table from</param>
        Friend Shared Function ConvertDataViewToDataTable(ByVal data As DataView) As System.Data.DataTable
            Dim Result As DataTable = data.Table.Clone
            'Dim MyRows As DataRowView() = data.Item

            If data.Count > 0 Then
                For MyCounter As Integer = 1 To data.Count
                    Dim MyNewRow As DataRow = Result.NewRow
                    MyNewRow.ItemArray = data.Item(MyCounter - 1).Row.ItemArray
                    Result.Rows.Add(MyNewRow)
                Next
            End If

            Return Result

        End Function
        ''' <summary>
        '''     Convert an ArrayList to a datatable
        ''' </summary>
        ''' <param name="arrayList">An ArrayList with some content</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        <Obsolete("use ConvertICollectionToDataTable instead", False)> Friend Shared Function ConvertArrayListToDataTable(ByVal arrayList As ArrayList) As DataTable
            Return ConvertICollectionToDataTable(arrayList)
        End Function
        ''' <summary>
        '''     Convert a data table to an arraylist
        ''' </summary>
        ''' <param name="column">The column which shall be used to fill the arraylist</param>
        Friend Shared Function ConvertDataTableToArrayList(ByVal column As DataColumn) As ArrayList
            Return ConvertDataTableToArrayList(column.Table, column.Ordinal)
        End Function
        ''' <summary>
        '''     Convert a data table to an arraylist
        ''' </summary>
        ''' <param name="data">The first column of this data table will be used</param>
        Friend Shared Function ConvertDataTableToArrayList(ByVal data As DataTable) As ArrayList
            Return ConvertDataTableToArrayList(data, 0)
        End Function
        ''' <summary>
        '''     Convert a data table to an arraylist
        ''' </summary>
        ''' <param name="data">The data table with the content</param>
        ''' <param name="selectedColumnIndex">The column which shall be used to fill the arraylist</param>
        Friend Shared Function ConvertDataTableToArrayList(ByVal data As DataTable, ByVal selectedColumnIndex As Integer) As ArrayList
            Dim Result As New ArrayList
            For MyCounter As Integer = 0 To data.Rows.Count - 1
                Result.Add(data.Rows(MyCounter)(selectedColumnIndex))
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Convert a data table to a hash table
        ''' </summary>
        ''' <param name="keyColumn">This is the key column from the data table and MUST BE UNIQUE</param>
        ''' <param name="valueColumn">A column which contains the values</param>
        ''' <remarks>
        ''' ATTENTION: the very first column is used as key column and must be unique therefore
        ''' </remarks>
        Friend Shared Function ConvertDataTableToHashtable(ByVal keyColumn As DataColumn, ByVal valueColumn As DataColumn) As Hashtable
            If Not keyColumn.Table Is valueColumn.Table Then
                Throw New Exception("Key column and value column must be from the same table")
            End If
            Return ConvertDataTableToHashtable(keyColumn.Table, keyColumn.Ordinal, valueColumn.Ordinal)
        End Function
        ''' <summary>
        '''     Convert a data table to a hash table
        ''' </summary>
        ''' <param name="data">The first two columns of this data table will be used</param>
        ''' <remarks>
        '''     ATTENTION: the very first column is used as key column and must be unique therefore
        ''' </remarks>
        Friend Shared Function ConvertDataTableToHashtable(ByVal data As DataTable) As Hashtable
            Return ConvertDataTableToHashtable(data, 0, 1)
        End Function
        ''' <summary>
        '''     Convert a data table to a hash table
        ''' </summary>
        ''' <param name="data">The data table with the content</param>
        ''' <param name="keyColumnIndex">This is the key column from the data table and MUST BE UNIQUE (make it unique, first!)</param>
        ''' <param name="valueColumnIndex">A column which contains the values</param>
        ''' <remarks>
        '''     ATTENTION: the very first column is used as key column and must be unique therefore
        ''' </remarks>
        Friend Shared Function ConvertDataTableToHashtable(ByVal data As DataTable, ByVal keyColumnIndex As Integer, ByVal valueColumnIndex As Integer) As Hashtable
            If data.Columns(keyColumnIndex).Unique = False Then
                Throw New Exception("The hashtable requires your key column to be a unique column - make it a unique column, first!")
            End If
            Dim Result As New Hashtable
            For MyCounter As Integer = 0 To data.Rows.Count - 1
                Result.Add(data.Rows(MyCounter)(keyColumnIndex), data.Rows(MyCounter)(valueColumnIndex))
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Convert a data table to an array of dictionary entries
        ''' </summary>
        ''' <param name="data">The first two columns of this data table will be used</param>
        ''' <remarks>
        '''     The very first column is used as key column, the second one as the value column
        ''' </remarks>
        Friend Shared Function ConvertDataTableToDictionaryEntryArray(ByVal data As DataTable) As DictionaryEntry()
            Return ConvertDataTableToDictionaryEntryArray(data, 0, 1)
        End Function
        ''' <summary>
        '''     Convert a data table to an array of dictionary entries
        ''' </summary>
        ''' <param name="keyColumn">This is the key column from the data table</param>
        ''' <param name="valueColumn">A column which contains the values</param>
        Friend Shared Function ConvertDataTableToDictionaryEntryArray(ByVal keyColumn As DataColumn, ByVal valueColumn As DataColumn) As DictionaryEntry()
            If Not keyColumn.Table Is valueColumn.Table Then
                Throw New Exception("Key column and value column must be from the same table")
            End If
            Return ConvertDataTableToDictionaryEntryArray(keyColumn.Table, keyColumn.Ordinal, valueColumn.Ordinal)
        End Function
        ''' <summary>
        '''     Convert a data table to an array of dictionary entries
        ''' </summary>
        ''' <param name="data">The data table with the content</param>
        ''' <param name="keyColumnIndex">This is the key column from the data table</param>
        ''' <param name="valueColumnIndex">A column which contains the values</param>
        Friend Shared Function ConvertDataTableToDictionaryEntryArray(ByVal data As DataTable, ByVal keyColumnIndex As Integer, ByVal valueColumnIndex As Integer) As DictionaryEntry()
            Dim Result As DictionaryEntry()
            ReDim Result(data.Rows.Count - 1)
            For MyCounter As Integer = 0 To data.Rows.Count - 1
                Result(MyCounter) = New DictionaryEntry(data.Rows(MyCounter)(keyColumnIndex), data.Rows(MyCounter)(valueColumnIndex))
            Next
            Return Result
        End Function
        ''' <summary>
        '''     Convert a hashtable to a datatable
        ''' </summary>
        ''' <param name="hashtable">A hashtable with some content</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        <Obsolete("use ConvertIDictionaryToDataTable instead and pay attention to parameter keyIsUnique", False)> Friend Shared Function ConvertHashtableToDataTable(ByVal hashtable As Hashtable) As DataTable
            Return ConvertIDictionaryToDataTable(hashtable, True)
        End Function
        ''' <summary>
        '''     Convert an ICollection to a datatable
        ''' </summary>
        ''' <param name="collection">An ICollection with some content</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        Friend Shared Function ConvertICollectionToDataTable(ByVal collection As ICollection) As DataTable
            Dim Result As New DataTable
            Result.Columns.Add(New DataColumn("value"))

            For Each MyKey As Object In collection
                Dim MyRow As DataRow = Result.NewRow
                MyRow(0) = MyKey
                Result.Rows.Add(MyRow)
            Next

            Return Result
        End Function
        ''' <summary>
        '''     Convert an IDictionary to a datatable
        ''' </summary>
        ''' <param name="dictionary">An IDictionary with some content</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        Friend Shared Function ConvertIDictionaryToDataTable(ByVal dictionary As IDictionary) As DataTable
            Return ConvertIDictionaryToDataTable(dictionary, False)
        End Function
        ''' <summary>
        '''     Convert an IDictionary to a datatable
        ''' </summary>
        ''' <param name="dictionary">An IDictionary with some content</param>
        ''' <param name="keyIsUnique">If true, the key column in the data table will be marked as unique</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        Friend Shared Function ConvertIDictionaryToDataTable(ByVal dictionary As IDictionary, ByVal keyIsUnique As Boolean) As DataTable
            Dim Result As New DataTable
            Result.Columns.Add(New DataColumn("key"))
            Result.Columns("key").Unique = True
            Result.Columns.Add(New DataColumn("value"))

            For Each MyKey As Object In dictionary.Keys
                Dim MyRow As DataRow = Result.NewRow
                MyRow(0) = MyKey
                MyRow(1) = dictionary(MyKey)
                Result.Rows.Add(MyRow)
            Next

            Return Result
        End Function
        ''' <summary>
        '''     Convert a NameValueCollection to a datatable
        ''' </summary>
        ''' <param name="nameValueCollection">An IDictionary with some content</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        Friend Shared Function ConvertNameValueCollectionToDataTable(ByVal nameValueCollection As Specialized.NameValueCollection) As DataTable
            Return ConvertNameValueCollectionToDataTable(nameValueCollection, False)
        End Function
        ''' <summary>
        '''     Convert a NameValueCollection to a datatable
        ''' </summary>
        ''' <param name="nameValueCollection">An IDictionary with some content</param>
        ''' <param name="keyIsUnique">If true, the key column in the data table will be marked as unique</param>
        ''' <returns>Datatable with column &quot;key&quot; and &quot;value&quot;</returns>
        Friend Shared Function ConvertNameValueCollectionToDataTable(ByVal nameValueCollection As Specialized.NameValueCollection, ByVal keyIsUnique As Boolean) As DataTable
            Dim Result As New DataTable
            Result.Columns.Add(New DataColumn("key"))
            Result.Columns("key").Unique = True
            Result.Columns.Add(New DataColumn("value"))

            For Each MyKey As String In nameValueCollection.Keys
                Dim MyRow As DataRow = Result.NewRow
                MyRow(0) = MyKey
                MyRow(1) = nameValueCollection(MyKey)
                Result.Rows.Add(MyRow)
            Next

            Return Result
        End Function
        ''' <summary>
        '''     Simplified creation of a DataTable by definition of a SQL statement and a connection string
        ''' </summary>
        ''' <param name="strSQL">The SQL statement to retrieve the data</param>
        ''' <param name="ConnectionString">The connection string to the data source</param>
        ''' <param name="NameOfNewDataTable">The name of the new DataTable</param>
        ''' <returns>A filled DataTable</returns>
        Friend Shared Function GetDataTableViaODBC(ByVal strSQL As String, ByVal ConnectionString As String, ByVal NameOfNewDataTable As String) As DataTable

            Dim MyConn As New Odbc.OdbcConnection(ConnectionString)
            Dim MyDataTable As New DataTable(NameOfNewDataTable)
            Dim MyCmd As New Odbc.OdbcCommand(strSQL, MyConn)
            Dim MyDA As New Odbc.OdbcDataAdapter(MyCmd)
            Try
                MyConn.Open()
                MyDA.Fill(MyDataTable)
            Finally
                If Not MyDA Is Nothing Then
                    MyDA.Dispose()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            Return MyDataTable

        End Function
        ''' <summary>
        '''     Simplified creation of a DataTable by definition of a SQL statement and a connection string
        ''' </summary>
        ''' <param name="strSQL">The SQL statement to retrieve the data</param>
        ''' <param name="ConnectionString">The connection string to the data source</param>
        ''' <param name="NameOfNewDataTable">The name of the new DataTable</param>
        ''' <returns>A filled DataTable</returns>
        Friend Shared Function GetDataTableViaSqlClient(ByVal strSQL As String, ByVal ConnectionString As String, ByVal NameOfNewDataTable As String) As DataTable

            Dim MyConn As New System.Data.SqlClient.SqlConnection(ConnectionString)
            Dim MyDataTable As New DataTable(NameOfNewDataTable)
            Dim MyCmd As New System.Data.SqlClient.SqlCommand(strSQL, MyConn)
            Dim MyDA As New System.Data.SqlClient.SqlDataAdapter(MyCmd)

            Try
                MyConn.Open()
                MyDA.Fill(MyDataTable)
            Finally
                If Not MyDA Is Nothing Then
                    MyDA.Dispose()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            Return MyDataTable

        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="dataTable">The datatable to retrieve the content from</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Public Shared Function ConvertToHtmlTable(ByVal dataTable As DataTable) As String
            Return Tools.Data.DataTables.ConvertToHtmlTable(dataTable, "<H1>", "</H1>", Nothing)
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Public Shared Function ConvertToHtmlTable(ByVal rows As DataRowCollection, ByVal label As String) As String
            Return Tools.Data.DataTables.ConvertToHtmlTable(rows, label, "<H1>", "</H1>", Nothing)
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Public Shared Function ConvertToHtmlTable(ByVal rows() As DataRow, ByVal label As String) As String
            Return Tools.Data.DataTables.ConvertToHtmlTable(rows, label, "<H1>", "</H1>", Nothing)
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="dataTable">The datatable to retrieve the content from</param>
        ''' <param name="titleTagOpener">The opening tag in front of the table's title</param>
        ''' <param name="titleTagEnd">The closing tag after the table title</param>
        ''' <param name="additionalTableAttributes">Additional attributes for the rendered table</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Friend Shared Function ConvertToHtmlTable(ByVal dataTable As DataTable, ByVal titleTagOpener As String, ByVal titleTagEnd As String, ByVal additionalTableAttributes As String) As String
            Return ConvertToHtmlTable(dataTable.Rows, dataTable.TableName, titleTagOpener, titleTagEnd, additionalTableAttributes)
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <param name="titleTagOpener">The opening tag in front of the table's title</param>
        ''' <param name="titleTagEnd">The closing tag after the table title</param>
        ''' <param name="additionalTableAttributes">Additional attributes for the rendered table</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Friend Shared Function ConvertToHtmlTable(ByVal rows As DataRowCollection, ByVal label As String, ByVal titleTagOpener As String, ByVal titleTagEnd As String, ByVal additionalTableAttributes As String) As String
            Return ConvertToHtmlTable(rows, label, titleTagOpener, titleTagEnd, additionalTableAttributes, False)
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <param name="titleTagOpener">The opening tag in front of the table's title</param>
        ''' <param name="titleTagEnd">The closing tag after the table title</param>
        ''' <param name="additionalTableAttributes">Additional attributes for the rendered table</param>
        ''' <param name="htmlEncodeCellContentAndLineBreaks">Encode all output to valid HTML</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Friend Shared Function ConvertToHtmlTable(ByVal rows As DataRowCollection, ByVal label As String, ByVal titleTagOpener As String, ByVal titleTagEnd As String, ByVal additionalTableAttributes As String, ByVal htmlEncodeCellContentAndLineBreaks As Boolean) As String
            Dim Result As New System.Text.StringBuilder
            If label <> "" Then
                Result.Append(titleTagOpener)
                If htmlEncodeCellContentAndLineBreaks Then
                    Result.Append(HtmlEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(String.Format("{0}", label))))
                Else
                    Result.Append(String.Format("{0}", label))
                End If
                Result.Append(titleTagEnd & System.Environment.NewLine)
            End If
            If rows.Count <= 0 Then
                Return Nothing
            End If
            Result.Append("<TABLE ")
            Result.Append(additionalTableAttributes)
            Result.Append("><TR>")
            For Each column As DataColumn In rows(0).Table.Columns
                Result.Append("<TH>")
                If column.Caption <> Nothing Then
                    If htmlEncodeCellContentAndLineBreaks Then
                        Result.Append(HtmlEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(String.Format("{0}", column.Caption))))
                    Else
                        Result.Append(String.Format("{0}", column.Caption))
                    End If
                Else
                    If htmlEncodeCellContentAndLineBreaks Then
                        Result.Append(HtmlEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(String.Format("{0}", column.ColumnName))))
                    Else
                        Result.Append(String.Format("{0}", column.ColumnName))
                    End If
                End If
                Result.Append("</TH>")
            Next
            Result.Append("</TR>")
            Result.Append(System.Environment.NewLine)
            For Each row As DataRow In rows
                Result.Append("<TR>")
                For Each column As DataColumn In row.Table.Columns
                    Result.Append("<TD>")
                    If htmlEncodeCellContentAndLineBreaks Then
                        Result.Append(HtmlEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(String.Format("{0}", row(column)))))
                    Else
                        Result.Append(String.Format("{0}", row(column)))
                    End If
                    Result.Append("</TD>")
                Next
                Result.Append("</TR>")
                Result.Append(System.Environment.NewLine)
            Next
            Result.Append("</TABLE>")
            Return Result.ToString
        End Function
        ''' <summary>
        '''     Converts all line breaks into HTML line breaks (&quot;&lt;br&gt;&quot;)
        ''' </summary>
        ''' <param name="Text"></param>
        ''' <remarks>
        '''     Supported line breaks are linebreaks of Windows, MacOS as well as Linux/Unix.
        ''' </remarks>
        Private Shared Function HtmlEncodeLineBreaks(ByVal Text As String) As String
            Return Text.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Cr, "<br>").Replace(ControlChars.Lf, "<br>")
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows as an html table
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <param name="titleTagOpener">The opening tag in front of the table's title</param>
        ''' <param name="titleTagEnd">The closing tag after the table title</param>
        ''' <param name="additionalTableAttributes">Additional attributes for the rendered table</param>
        ''' <returns>If no rows have been processed, the return string is nothing</returns>
        Friend Shared Function ConvertToHtmlTable(ByVal rows() As DataRow, ByVal label As String, ByVal titleTagOpener As String, ByVal titleTagEnd As String, ByVal additionalTableAttributes As String) As String
            Dim Result As New System.Text.StringBuilder
            If label <> "" Then
                Result.Append(titleTagOpener)
                Result.Append(String.Format("{0}", label) & System.Environment.NewLine)
                Result.Append(titleTagEnd)
            End If
            If rows.Length <= 0 Then
                Return Nothing
            End If
            Result.Append("<TABLE ")
            Result.Append(additionalTableAttributes)
            Result.Append("><TR>")
            For Each c As DataColumn In rows(0).Table.Columns
                Result.Append(String.Format("{0}", c.ColumnName))
            Next
            Result.Append("</TR>")
            Result.Append(System.Environment.NewLine)
            For Each row As DataRow In rows
                Result.Append("<TR>")
                For Each column As DataColumn In row.Table.Columns
                    Result.Append("<TD>")
                    Result.Append(String.Format("{0}", row(column)))
                    Result.Append("</TD>")
                Next
                Result.Append("</TR>")
                Result.Append(System.Environment.NewLine)
            Next
            Result.Append("</TABLE>")
            Return Result.ToString
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows, helpfull for debugging purposes
        ''' </summary>
        ''' <param name="dataTable">The datatable to retrieve the content from</param>
        ''' <returns>All rows are tab separated. If no rows have been processed, the user will get notified about this fact</returns>
        Friend Shared Function ConvertToPlainTextTable(ByVal dataTable As DataTable) As String
            Return _ConvertToPlainTextTable(dataTable.Rows, dataTable.TableName)
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows, helpfull for debugging purposes
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <returns>All rows are tab separated. If no rows have been processed, the user will get notified about this fact</returns>
        Friend Shared Function ConvertToPlainTextTable(ByVal rows() As DataRow, ByVal label As String) As String
            Const separator As Char = ControlChars.Tab
            Dim Result As New System.Text.StringBuilder
            If label <> "" Then
                Result.Append(String.Format("{0}", label) & System.Environment.NewLine)
            End If
            If rows.Length <= 0 Then
                Result.Append("no rows found" & System.Environment.NewLine)
                Return Result.ToString
            End If
            For Each column As DataColumn In rows(0).Table.Columns
                If column.Ordinal <> 0 Then Result.Append(separator)
                If column.Caption <> Nothing Then
                    Result.Append(String.Format("{0}", column.Caption))
                Else
                    Result.Append(String.Format("{0}", column.ColumnName))
                End If
            Next
            Result.Append(System.Environment.NewLine)
            For Each row As DataRow In rows
                For Each column As DataColumn In row.Table.Columns
                    If column.Ordinal <> 0 Then Result.Append(separator)
                    Result.Append(String.Format("{0}", row(column)))
                Next
                Result.Append(System.Environment.NewLine)
            Next
            Return Result.ToString
        End Function
        ''' <summary>
        '''     Return a string with all columns and rows, helpfull for debugging purposes
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <returns>All rows are tab separated. If no rows have been processed, the user will get notified about this fact</returns>
        Private Shared Function _ConvertToPlainTextTable(ByVal rows As DataRowCollection, ByVal label As String) As String
            Const separator As Char = ControlChars.Tab
            Dim Result As New System.Text.StringBuilder
            If label <> "" Then
                Result.Append(String.Format("{0}", label) & System.Environment.NewLine)
            End If
            If rows.Count <= 0 Then
                Result.Append("no rows found" & System.Environment.NewLine)
                Return Result.ToString
            End If
            For Each column As DataColumn In rows(0).Table.Columns
                If column.Ordinal <> 0 Then Result.Append(separator)
                If column.Caption <> Nothing Then
                    Result.Append(String.Format("{0}", column.Caption))
                Else
                    Result.Append(String.Format("{0}", column.ColumnName))
                End If
            Next
            Result.Append(System.Environment.NewLine)
            For Each row As DataRow In rows
                For Each column As DataColumn In row.Table.Columns
                    If column.Ordinal <> 0 Then Result.Append(separator)
                    Result.Append(String.Format("{0}", row(column)))
                Next
                Result.Append(System.Environment.NewLine)
            Next
            Return Result.ToString
        End Function
        ''' <summary>
        '''     Remove the specified columns if they exist
        ''' </summary>
        ''' <param name="datatable">A datatable where the operations shall be made</param>
        ''' <param name="columnNames">The names of the columns which shall be removed</param>
        ''' <remarks>
        '''     The columns will only be removed if they exist. If a column name doesn't exist, it will be ignored.
        ''' </remarks>
        Public Shared Sub RemoveColumns(ByVal datatable As System.Data.DataTable, ByVal columnNames As String())
            If Not columnNames Is Nothing Then
                For MyRemoveCounter As Integer = 0 To columnNames.Length - 1
                    For MyColumnsCounter As Integer = datatable.Columns.Count - 1 To 0 Step -1
                        If datatable.Columns(MyColumnsCounter).ColumnName = columnNames(MyRemoveCounter) Then
                            datatable.Columns.RemoveAt(MyColumnsCounter)
                        End If
                    Next
                Next
            End If
        End Sub
        ''' <summary>
        '''     Return a string with all columns and rows, helpfull for debugging purposes
        ''' </summary>
        ''' <param name="rows">The rows to be processed</param>
        ''' <param name="label">An optional title of the rows</param>
        ''' <returns>All rows are tab separated. If no rows have been processed, the user will get notified about this fact</returns>
        Friend Shared Function ConvertToPlainTextTable(ByVal rows As DataRowCollection, ByVal label As String) As String
            Return _ConvertToPlainTextTable(rows, label)
        End Function
        ''' <summary>
        '''     Convert any opened datareader into a dataset
        ''' </summary>
        ''' <param name="dataReader">An already opened dataReader</param>
        ''' <returns>A dataset containing all datatables the dataReader was able to read</returns>
        Friend Shared Function ConvertDataReaderToDataSet(ByVal datareader As IDataReader) As DataSet
            Dim Result As New DataSet
            Dim DRA As New DataReaderAdapter
            DRA.FillFromReader(Result, datareader)
            Return Result
        End Function
        ''' <summary>
        '''     Convert any opened datareader into a data table
        ''' </summary>
        ''' <param name="dataReader">An already opened dataReader</param>
        ''' <returns>A data table containing all data the dataReader was able to read</returns>
        Friend Shared Function ConvertDataReaderToDataTable(ByVal dataReader As IDataReader) As DataTable
            Return ConvertDataReaderToDataTable(dataReader, Nothing)
        End Function
        ''' <summary>
        '''     Convert any opened datareader into a data table
        ''' </summary>
        ''' <param name="dataReader">An already opened dataReader</param>
        ''' <param name="tableName">The name for the new table</param>
        ''' <returns>A data table containing all data the dataReader was able to read</returns>
        Friend Shared Function ConvertDataReaderToDataTable(ByVal dataReader As IDataReader, ByVal tableName As String) As DataTable

            Dim Result As DataTable
            Dim DRA As New DataReaderAdapter
            If tableName Is Nothing Then
                Result = New DataTable
            Else
                Result = New DataTable(tableName)
            End If
            DRA.FillFromReader(Result, dataReader)
            Return Result

        End Function

        ''' <summary>
        '''     A data adapter for data readers making the real conversion
        ''' </summary>
        Private Class DataReaderAdapter
            Inherits System.Data.Common.DbDataAdapter

            Friend Function FillFromReader(ByVal dataTable As DataTable, ByVal dataReader As IDataReader) As Integer
                Return Me.Fill(dataTable, dataReader)
            End Function

            Friend Function FillFromReader(ByVal dataSet As DataSet, ByVal dataReader As IDataReader) As Integer
                Return Me.Fill(dataSet, "Table", dataReader, 0, 0)
            End Function

            Protected Overrides Function CreateRowUpdatedEvent(ByVal dataRow As System.Data.DataRow, ByVal command As System.Data.IDbCommand, ByVal statementType As System.Data.StatementType, ByVal tableMapping As System.Data.Common.DataTableMapping) As System.Data.Common.RowUpdatedEventArgs
                Return Nothing
            End Function

            Protected Overrides Function CreateRowUpdatingEvent(ByVal dataRow As System.Data.DataRow, ByVal command As System.Data.IDbCommand, ByVal statementType As System.Data.StatementType, ByVal tableMapping As System.Data.Common.DataTableMapping) As System.Data.Common.RowUpdatingEventArgs
                Return Nothing
            End Function

            Protected Overrides Sub OnRowUpdated(ByVal value As System.Data.Common.RowUpdatedEventArgs)
            End Sub

            Protected Overrides Sub OnRowUpdating(ByVal value As System.Data.Common.RowUpdatingEventArgs)
            End Sub

        End Class
        ''' <summary>
        '''     Table join types
        ''' </summary>
        Friend Enum JoinTypes As Integer
            ''' <summary>
            '''     The result contains only those rows which exist in both tables
            ''' </summary>
            Inner = 0
            ''' <summary>
            '''     The result contains all rows of the left, parent table and only those rows of the other table which are related the rows of the left table
            ''' </summary>
            Left = 1
            '''' -----------------------------------------------------------------------------
            '''' <summary>
            ''''     The result contains all rows of the left, parent table and all rows of the right, child table. Missing values on the other side will be of value DBNull 
            '''' </summary>
            '''' <remarks>
            '''' </remarks>
            '''' <history>
            '''' 	[adminwezel]	17.01.2006	Created
            '''' </history>
            '''' -----------------------------------------------------------------------------
            'Full = 2
        End Enum
        ''' <summary>
        '''     Execute a table join on two tables of the same dataset based on the first relation found
        ''' </summary>
        ''' <param name="leftParentTable"></param>
        ''' <param name="rightChildTable"></param>
        ''' <param name="joinType">Inner or left join</param>
        Friend Shared Function JoinTables(ByVal leftParentTable As DataTable, ByVal rightChildTable As DataTable, ByVal joinType As JoinTypes) As DataTable

            'Find the appropriate relation information
            Dim ActiveRelation As DataRelation = Nothing
            For MyRelCounter As Integer = 0 To leftParentTable.ChildRelations.Count - 1
                If leftParentTable.ChildRelations(MyRelCounter).ChildTable Is rightChildTable Then
                    ActiveRelation = leftParentTable.ChildRelations(MyRelCounter)
                    Exit For
                End If
            Next

            Return JoinTables(leftParentTable, rightChildTable, ActiveRelation, joinType)

        End Function
        ''' <summary>
        '''     Execute a table join on two tables of the same dataset which have got a defined relation
        ''' </summary>
        ''' <param name="leftParentTable">The left or parent table</param>
        ''' <param name="rightChildTable">The right or child table</param>
        ''' <param name="relation">A data table relation which shall be used for the joining</param>
        ''' <param name="joinType">Inner or left join</param>
        ''' <remarks>
        '''     The selected columns are: 
        '''     <list>
        '''         <item>all columns from the left parent table</item>
        '''         <item>INNER JOIN: those columns from the right child table which are not member of the relation in charge</item>
        '''         <item>LEFT JOIN: all columns from the right child table</item>
        '''     </list>
        ''' </remarks>
        Friend Shared Function JoinTables(ByVal leftParentTable As DataTable, ByVal rightChildTable As DataTable, ByVal relation As DataRelation, ByVal joinType As JoinTypes) As DataTable

            'Verify parameters
            If leftParentTable Is Nothing Or rightChildTable Is Nothing Then
                Throw New Exception("One or both table references are null")
            End If
            If leftParentTable.DataSet Is Nothing OrElse Not leftParentTable.DataSet Is rightChildTable.DataSet Then
                Throw New Exception("Both tables must be member of the same dataset")
            End If
            If relation Is Nothing Then
                Throw New Exception("No relation defined between the two tables")
            End If

            Dim rightColumns As Integer() = Nothing
            If joinType = JoinTypes.Inner Then
                Dim ChildColumnsToAdd As New ArrayList 'Contains the column index values of the child table which shall be copied. These columns are regulary all columns (LEFT JOIN) except those required for the relation (for INNER JOIN; they would lead to duplicate columns)
                'Collect the data columns which shall be added from the client table to the parent table
                Dim RelationColumns As DataColumn() = relation.ChildColumns
                For MyColCounter As Integer = 0 To rightChildTable.Columns.Count - 1
                    Dim IsRelationColumn As Boolean = False
                    'Verify that we don't add relation columns which would lead to duplicate data since those columns must also be in the parent table
                    For MyRelColCounter As Integer = 0 To RelationColumns.Length - 1
                        If rightChildTable.Columns(MyColCounter) Is RelationColumns(MyRelColCounter) Then
                            IsRelationColumn = True
                        End If
                    Next
                    If IsRelationColumn = False Then
                        'Add the column index to the list of ToAdd-columns
                        ChildColumnsToAdd.Add(MyColCounter)
                    End If
                Next
                rightColumns = CType(ChildColumnsToAdd.ToArray(GetType(Integer)), Integer())
            End If

            'Return the results
            Return JoinTables(leftParentTable, Nothing, rightChildTable, rightColumns, relation, joinType)

        End Function
        ''' <summary>
        '''     Execute a table join on two tables of the same dataset which have got a defined relation
        ''' </summary>
        ''' <param name="leftParentTable">The left or parent table</param>
        ''' <param name="leftTableColumnsToCopy">An array of columns to copy from the left table</param>
        ''' <param name="rightChildTable">The right or child table</param>
        ''' <param name="rightTableColumnsToCopy">An array of columns to copy from the right table</param>
        ''' <param name="joinType">Inner or left join</param>
        Friend Shared Function JoinTables(ByVal leftParentTable As DataTable, ByVal leftTableColumnsToCopy As DataColumn(), ByVal rightChildTable As DataTable, ByVal rightTableColumnsToCopy As DataColumn(), ByVal joinType As JoinTypes) As DataTable

            'Find the appropriate relation information
            Dim ActiveRelation As DataRelation = Nothing
            For MyRelCounter As Integer = 0 To leftParentTable.ChildRelations.Count - 1
                If leftParentTable.ChildRelations(MyRelCounter).ChildTable Is rightChildTable Then
                    ActiveRelation = leftParentTable.ChildRelations(MyRelCounter)
                    Exit For
                End If
            Next

            'Find required column indexes
            Dim LeftColumns As Integer() = Nothing
            Dim RightColumns As Integer() = Nothing
            If Not leftTableColumnsToCopy Is Nothing Then
                Dim indexesOfLeftTableColumnsToCopy As New ArrayList
                For MyCounter As Integer = 0 To leftTableColumnsToCopy.Length - 1
                    indexesOfLeftTableColumnsToCopy.Add(leftTableColumnsToCopy(MyCounter).Ordinal)
                Next
                LeftColumns = CType(indexesOfLeftTableColumnsToCopy.ToArray(GetType(Integer)), Integer())
            End If
            If Not leftTableColumnsToCopy Is Nothing Then
                Dim indexesOfRightTableColumnsToCopy As New ArrayList
                For MyCounter As Integer = 0 To leftTableColumnsToCopy.Length - 1
                    indexesOfRightTableColumnsToCopy.Add(leftTableColumnsToCopy(MyCounter).Ordinal)
                Next
                RightColumns = CType(indexesOfRightTableColumnsToCopy.ToArray(GetType(Integer)), Integer())
            End If

            'Return the results
            Return JoinTables(leftParentTable, LeftColumns, rightChildTable, RightColumns, ActiveRelation, joinType)

        End Function
        ''' <summary>
        '''     Execute a table join on two tables of the same dataset which have got a defined relation
        ''' </summary>
        ''' <param name="leftParentTable">The left or parent table</param>
        ''' <param name="indexesOfLeftTableColumnsToCopy">An array of column indexes to copy from the left table</param>
        ''' <param name="rightChildTable">The right or child table</param>
        ''' <param name="indexesOfRightTableColumnsToCopy">An array of column indexes to copy from the right table</param>
        ''' <param name="joinType">Inner or left join</param>
        Friend Shared Function JoinTables(ByVal leftParentTable As DataTable, ByVal indexesOfLeftTableColumnsToCopy As Integer(), ByVal rightChildTable As DataTable, ByVal indexesOfRightTableColumnsToCopy As Integer(), ByVal joinType As JoinTypes) As DataTable

            'Find the appropriate relation information
            Dim ActiveRelation As DataRelation = Nothing
            For MyRelCounter As Integer = 0 To leftParentTable.ChildRelations.Count - 1
                If leftParentTable.ChildRelations(MyRelCounter).ChildTable Is rightChildTable Then
                    ActiveRelation = leftParentTable.ChildRelations(MyRelCounter)
                    Exit For
                End If
            Next

            Return JoinTables(leftParentTable, indexesOfLeftTableColumnsToCopy, rightChildTable, indexesOfRightTableColumnsToCopy, ActiveRelation, joinType)

        End Function
        ''' <summary>
        '''     Execute a table join on two tables of the same dataset which have got a defined relation
        ''' </summary>
        ''' <param name="leftParentTable">The left or parent table</param>
        ''' <param name="indexesOfLeftTableColumnsToCopy">An array of column indexes to copy from the left table</param>
        ''' <param name="rightChildTable">The right or child table</param>
        ''' <param name="indexesOfRightTableColumnsToCopy">An array of column indexes to copy from the right table</param>
        ''' <param name="relation">A data table relation which shall be used for the joining</param>
        ''' <param name="joinType">Inner or left join</param>
        Friend Shared Function JoinTables(ByVal leftParentTable As DataTable, ByVal indexesOfLeftTableColumnsToCopy As Integer(), ByVal rightChildTable As DataTable, ByVal indexesOfRightTableColumnsToCopy As Integer(), ByVal relation As DataRelation, ByVal joinType As JoinTypes) As DataTable

            'Verify parameters
            If leftParentTable Is Nothing Or rightChildTable Is Nothing Then
                Throw New Exception("One or both table references are null")
            End If
            If leftParentTable.DataSet Is Nothing OrElse Not leftParentTable.DataSet Is rightChildTable.DataSet Then
                Throw New Exception("Both tables must be member of the same dataset")
            End If
            If relation Is Nothing Then
                Throw New Exception("No relation defined between the two tables")
            End If

            'Prepare column wrap table
            Dim LeftTableColumnWraps As Integer()
            If indexesOfLeftTableColumnsToCopy Is Nothing Then
                Dim LeftColumnsToCopy As New ArrayList
                'Add all columns from left table
                For ColCounter As Integer = 0 To leftParentTable.Columns.Count - 1
                    LeftColumnsToCopy.Add(ColCounter)
                Next
                LeftTableColumnWraps = CType(LeftColumnsToCopy.ToArray(GetType(Integer)), Integer())
            Else
                'Add all columns as defined by indexesOfLeftTableColumnsToCopy
                Dim colWraps As New ArrayList
                For ColCounter As Integer = 0 To indexesOfLeftTableColumnsToCopy.Length - 1
                    Try
                        colWraps.Add(leftParentTable.Columns.Item(indexesOfLeftTableColumnsToCopy(ColCounter)).Ordinal)
                    Catch
                        Throw New Exception("Column index can't be found in source table's column collection: " & indexesOfLeftTableColumnsToCopy(ColCounter))
                    End Try
                Next
                LeftTableColumnWraps = CType(colWraps.ToArray(GetType(Integer)), Integer())
            End If

            'Prepare the result table by copying the parent table
            Dim Result As DataTable = leftParentTable.Clone
            Result.TableName = "JoinedTable"

            'Remove left table columns which are not required any more
            For MyCounter As Integer = Result.Columns.Count - 1 To 0 Step -1
                Dim KeepThisColumn As Boolean = False
                For MyColCounter As Integer = 0 To LeftTableColumnWraps.Length - 1
                    If LeftTableColumnWraps(MyColCounter) = MyCounter Then
                        KeepThisColumn = True
                        Exit For
                    End If
                Next
                If KeepThisColumn = False Then
                    If Result.Columns(MyCounter).Unique = True Then
                        Result.Columns(MyCounter).Unique = False
                    End If
                    Result.Columns.Remove(Result.Columns(MyCounter))
                End If
            Next

            'Add the right columns
            Dim RightTableColumnWraps As Integer()
            If indexesOfRightTableColumnsToCopy Is Nothing Then
                Dim RightColumnsToCopy As New ArrayList
                For MyCounter As Integer = 0 To rightChildTable.Columns.Count - 1
                    RightColumnsToCopy.Add(MyCounter)
                Next
                RightTableColumnWraps = CType(RightColumnsToCopy.ToArray(GetType(Integer)), Integer())
            Else
                RightTableColumnWraps = CType(indexesOfRightTableColumnsToCopy.Clone, Integer())
            End If
            For MyCounter As Integer = 0 To RightTableColumnWraps.Length - 1
                Dim MyColumn As DataColumn = rightChildTable.Columns(RightTableColumnWraps(MyCounter))
                Dim UniqueColumnName As String = LookupUnqiueColumnName(Result, MyColumn.ColumnName)
                Dim ColumnCaption As String = MyColumn.Caption
                Dim ColumnType As System.Type = MyColumn.DataType
                Result.Columns.Add(UniqueColumnName, ColumnType).Caption = ColumnCaption
            Next

            'Fill the rows now with the missing data
            For MyLeftTableRowCounter As Integer = 0 To leftParentTable.Rows.Count - 1
                Dim MyLeftRow As DataRow = leftParentTable.Rows(MyLeftTableRowCounter)
                Dim MyRightRows As DataRow() = MyLeftRow.GetChildRows(relation)

                If MyRightRows.Length = 0 Then
                    'Data only on left hand (parent) side
                    If joinType = JoinTypes.Left Then
                        Dim NewRow As DataRow = Result.NewRow
                        'Copy only data from parent table
                        For MyColCounter As Integer = 0 To LeftTableColumnWraps.Length - 1
                            NewRow(MyColCounter) = MyLeftRow(LeftTableColumnWraps(MyColCounter))
                        Next
                        'Add the new row, now
                        Result.Rows.Add(NewRow)
                    End If
                Else
                    'Data found on both sides
                    For RowInserts As Integer = 0 To MyRightRows.Length - 1
                        Dim NewRow As DataRow = Result.NewRow
                        'Copy data from parent table row
                        For MyColCounter As Integer = 0 To LeftTableColumnWraps.Length - 1
                            NewRow(MyColCounter) = MyLeftRow(LeftTableColumnWraps(MyColCounter))
                        Next
                        'Copy data from this child row
                        Dim MyRightRowsChild As DataRow = MyRightRows(RowInserts)
                        For MyColCounter As Integer = 0 To RightTableColumnWraps.Length - 1
                            NewRow(LeftTableColumnWraps.Length + MyColCounter) = MyRightRowsChild(RightTableColumnWraps(MyColCounter))
                        Next
                        'Add the new row, now
                        Result.Rows.Add(NewRow)
                    Next
                End If

            Next

            Return Result

        End Function
        ''' <summary>
        '''     Cross join of two tables
        ''' </summary>
        ''' <param name="leftTable">A first datatable</param>
        ''' <param name="indexesOfLeftTableColumnsToCopy">An array of column indexes to copy from the left table</param>
        ''' <param name="rightTable">A second datatable</param>
        ''' <param name="indexesOfRightTableColumnsToCopy">An array of column indexes to copy from the right table</param>
        Friend Shared Function CrossJoinTables(ByVal leftTable As DataTable, ByVal indexesOfLeftTableColumnsToCopy As Integer(), ByVal rightTable As DataTable, ByVal indexesOfRightTableColumnsToCopy As Integer()) As DataTable

            'Verify parameters
            If leftTable Is Nothing Or rightTable Is Nothing Then
                Throw New Exception("One or both table references are null")
            End If

            'Prepare column wrap table
            Dim LeftTableColumnWraps As Integer()
            If indexesOfLeftTableColumnsToCopy Is Nothing Then
                Dim LeftColumnsToCopy As New ArrayList
                'Add all columns from left table
                For ColCounter As Integer = 0 To leftTable.Columns.Count - 1
                    LeftColumnsToCopy.Add(ColCounter)
                Next
                LeftTableColumnWraps = CType(LeftColumnsToCopy.ToArray(GetType(Integer)), Integer())
            Else
                'Add all columns as defined by indexesOfLeftTableColumnsToCopy
                Dim colWraps As New ArrayList
                For ColCounter As Integer = 0 To indexesOfLeftTableColumnsToCopy.Length - 1
                    Try
                        colWraps.Add(leftTable.Columns.Item(indexesOfLeftTableColumnsToCopy(ColCounter)).Ordinal)
                    Catch
                        Throw New Exception("Column index can't be found in source table's column collection: " & indexesOfLeftTableColumnsToCopy(ColCounter))
                    End Try
                Next
                LeftTableColumnWraps = CType(colWraps.ToArray(GetType(Integer)), Integer())
            End If

            'Prepare the result table by copying the parent table
            Dim Result As DataTable = leftTable.Clone
            Result.TableName = "JoinedTable"

            'Remove left table columns which are not required any more
            For MyCounter As Integer = Result.Columns.Count - 1 To 0 Step -1
                Dim KeepThisColumn As Boolean = False
                For MyColCounter As Integer = 0 To LeftTableColumnWraps.Length - 1
                    If LeftTableColumnWraps(MyColCounter) = MyCounter Then
                        KeepThisColumn = True
                        Exit For
                    End If
                Next
                If KeepThisColumn = False Then
                    If Result.Columns(MyCounter).Unique = True Then
                        Result.Columns(MyCounter).Unique = False
                    End If
                    Result.Columns.Remove(Result.Columns(MyCounter))
                End If
            Next

            'Add the right columns
            Dim RightTableColumnWraps As Integer()
            If indexesOfRightTableColumnsToCopy Is Nothing Then
                Dim RightColumnsToCopy As New ArrayList
                For MyCounter As Integer = 0 To rightTable.Columns.Count - 1
                    RightColumnsToCopy.Add(MyCounter)
                Next
                RightTableColumnWraps = CType(RightColumnsToCopy.ToArray(GetType(Integer)), Integer())
            Else
                RightTableColumnWraps = CType(indexesOfRightTableColumnsToCopy.Clone, Integer())
            End If
            For MyCounter As Integer = 0 To RightTableColumnWraps.Length - 1
                Dim MyColumn As DataColumn = rightTable.Columns(RightTableColumnWraps(MyCounter))
                Dim UniqueColumnName As String = LookupUnqiueColumnName(Result, MyColumn.ColumnName)
                Dim ColumnCaption As String = MyColumn.Caption
                Dim ColumnType As System.Type = MyColumn.DataType
                Result.Columns.Add(UniqueColumnName, ColumnType).Caption = ColumnCaption
            Next

            'Fill the rows now with the missing data
            For MyLeftTableRowCounter As Integer = 0 To leftTable.Rows.Count - 1
                For MyRightTableRowCounter As Integer = 0 To rightTable.Rows.Count - 1
                    Dim MyLeftRow As DataRow = leftTable.Rows(MyLeftTableRowCounter)
                    Dim MyRightRow As DataRow = rightTable.Rows(MyRightTableRowCounter)

                    'Data found on both sides
                    Dim NewRow As DataRow = Result.NewRow

                    'Copy data from parent table row
                    For MyColCounter As Integer = 0 To LeftTableColumnWraps.Length - 1
                        NewRow(MyColCounter) = MyLeftRow(LeftTableColumnWraps(MyColCounter))
                    Next

                    'Copy data from this child row
                    For MyColCounter As Integer = 0 To RightTableColumnWraps.Length - 1
                        NewRow(LeftTableColumnWraps.Length + MyColCounter) = MyRightRow(RightTableColumnWraps(MyColCounter))
                    Next

                    'Add the new row, now
                    Result.Rows.Add(NewRow)

                Next
            Next

            Return Result

        End Function
        ''' <summary>
        '''     Add a prefix to the names of the columns
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="columnIndexes"></param>
        ''' <param name="prefix">e. g. "orders."</param>
        Friend Shared Sub AddPrefixesToColumnNames(ByVal dataTable As DataTable, ByVal columnIndexes As Integer(), ByVal prefix As String)

            'all columns if nothing is given
            If columnIndexes Is Nothing Then
                ReDim columnIndexes(dataTable.Columns.Count - 1)
                For MyCounter As Integer = 0 To dataTable.Columns.Count - 1
                    columnIndexes(MyCounter) = MyCounter
                Next
            End If

            For MyCounter As Integer = 0 To columnIndexes.Length - 1
                dataTable.Columns(columnIndexes(MyCounter)).ColumnName = prefix & dataTable.Columns(columnIndexes(MyCounter)).ColumnName
            Next

        End Sub
        ''' <summary>
        '''     Add a suffix to the names of the columns
        ''' </summary>
        ''' <param name="dataTable"></param>
        ''' <param name="columnIndexes"></param>
        ''' <param name="suffix">e. g. "-orders"</param>
        Friend Shared Sub AddSuffixesToColumnNames(ByVal dataTable As DataTable, ByVal columnIndexes As Integer(), ByVal suffix As String)

            'all columns if nothing is given
            If columnIndexes Is Nothing Then
                ReDim columnIndexes(dataTable.Columns.Count - 1)
                For MyCounter As Integer = 0 To dataTable.Columns.Count - 1
                    columnIndexes(MyCounter) = MyCounter
                Next
            End If

            For MyCounter As Integer = 0 To columnIndexes.Length - 1
                dataTable.Columns(columnIndexes(MyCounter)).ColumnName = dataTable.Columns(columnIndexes(MyCounter)).ColumnName & suffix
            Next

        End Sub
        ''' <summary>
        '''     Lookup a new unique column name for a data table
        ''' </summary>
        ''' <param name="dataTable">The data table which shall get a new data column</param>
        ''' <param name="suggestedColumnName">A column name suggestion</param>
        ''' <returns>The suggested column name as it is or modified column name to be unique</returns>
        Friend Shared Function LookupUnqiueColumnName(ByVal dataTable As DataTable, ByVal suggestedColumnName As String) As String

            Dim ColumnNameAlreadyExistant As Boolean = False
            For MyCounter As Integer = 0 To dataTable.Columns.Count - 1
                If String.Compare(suggestedColumnName, dataTable.Columns(MyCounter).ColumnName, True) = 0 Then
                    ColumnNameAlreadyExistant = True
                End If
            Next

            If ColumnNameAlreadyExistant = False Then
                'Exit function
                Return suggestedColumnName
            Else
                'Add prefix "ClientTable_" or add/increase a counter at the end
                If suggestedColumnName.StartsWith("ClientTable_") Then
                    'Find the position range of an already existing counter at the end of the string - if there is a number
                    Dim NumberPosition As Integer
                    For NumberPartCounter As Integer = suggestedColumnName.Length - 1 To 0 Step -1
                        If Char.IsNumber(suggestedColumnName.Chars(NumberPartCounter)) = False Then
                            NumberPosition = NumberPartCounter + 1
                            Exit For
                        End If
                    Next
                    'Read out the value of the counter
                    Dim NumberCounterValue As Integer
                    If NumberPosition > suggestedColumnName.Length Then
                        NumberCounterValue = 0
                    Else
                        NumberCounterValue = CType(Mid(suggestedColumnName, NumberPosition), Integer)
                    End If
                    'Attach/update the counter value
                    suggestedColumnName = Mid(suggestedColumnName, 1, NumberPosition) & NumberCounterValue.ToString
                Else
                    'Add new prefix
                    suggestedColumnName = "ClientTable_" & suggestedColumnName
                End If
                'Revalidate uniqueness by running recursively
                suggestedColumnName = LookupUnqiueColumnName(dataTable, suggestedColumnName)
            End If

            Return suggestedColumnName

        End Function

#Region "ReArrangeDataColumns"

        ''' <summary>
        '''     An exception which gets thrown when converting data in the ReArrangeDataColumns methods
        ''' </summary>
        Friend Class ReArrangeDataColumnsException
            Inherits Exception

            Public Sub New(ByVal rowIndex As Integer, ByVal columnIndex As Integer, ByVal sourceColumnType As Type, ByVal targetColumnType As Type, ByVal problematicValue As Object, ByVal innerException As Exception)
                MyBase.New("Data conversion exception", innerException)
                _RowIndex = rowIndex
                _ColumnIndex = columnIndex
                _sourceColumnType = sourceColumnType
                _targetColumnType = targetColumnType
                _problematicValue = problematicValue
            End Sub

            Private _sourceColumnType As Type
            Public ReadOnly Property SourceColumnType() As Type
                Get
                    Return _sourceColumnType
                End Get
            End Property

            Private _targetColumnType As Type
            Public ReadOnly Property TargetColumnType() As Type
                Get
                    Return _targetColumnType
                End Get
            End Property

            Private _problematicValue As Object
            Public ReadOnly Property ProblematicValue() As Object
                Get
                    Return _problematicValue
                End Get
            End Property

            Private _RowIndex As Integer
            Public ReadOnly Property RowIndex() As Integer
                Get
                    Return _RowIndex
                End Get
            End Property

            Private _ColumnIndex As Integer
            Public ReadOnly Property ColumnIndex() As Integer
                Get
                    Return _ColumnIndex
                End Get
            End Property

            Public Overrides ReadOnly Property Message() As String
                Get
                    Dim Result As String
                    Result = "Conversion exception in row index " & RowIndex & " and column index " & ColumnIndex & "." & System.Environment.NewLine
                    Result &= "Data type in source table: " & SourceColumnType.ToString & System.Environment.NewLine
                    Result &= "Data type in destination table: " & TargetColumnType.ToString & System.Environment.NewLine
                    If ProblematicValue.GetType Is GetType(String) Then
                        Result &= "The problematic value is: " & CType(ProblematicValue, String) & System.Environment.NewLine
                    End If
                    If Not Me.InnerException Is Nothing Then
                        Result &= System.Environment.NewLine & "This is the inner exception message: " & System.Environment.NewLine & Me.InnerException.Message
                    End If
                    Return Result
                End Get
            End Property

        End Class
        ''' <summary>
        '''     Rearrange columns
        ''' </summary>
        ''' <param name="source">The source table with data</param>
        ''' <param name="columnsToCopy">An array of column names which shall be copied in the specified order from the source table</param>
        ''' <returns>A new and independent data table with copied data</returns>
        Friend Shared Function ReArrangeDataColumns(ByVal source As DataTable, ByVal columnsToCopy As String()) As DataTable
            Dim columns As New ArrayList
            For MyCounter As Integer = 0 To columnsToCopy.Length - 1
                columns.Add(New DataColumn(columnsToCopy(MyCounter), source.Columns(columnsToCopy(MyCounter)).DataType))
            Next
            Return ReArrangeDataColumns(source, CType(columns.ToArray(GetType(System.Data.DataColumn)), System.Data.DataColumn()))
        End Function
        ''' <summary>
        '''     Rearrange columns and also change their data types
        ''' </summary>
        ''' <param name="source">The source table with data</param>
        ''' <param name="destinationColumnSet">An array of columns as they shall be inserted into the result</param>
        ''' <returns>A new and independent data table with copied data</returns>
        ''' <remarks>
        '''     The copy process requires that the names of the destination columns can be found in the columns collection of the source table. 
        ''' </remarks>
        ''' <example>
        '''     <code language="vb">
        '''         ReArrangeDataColumns(source, New System.Data.DataColumn() {New DataColumn("column1Name", GetType(String)), New DataColumn("column2Name", GetType(Integer))})
        '''     </code>
        ''' </example>
        Public Shared Function ReArrangeDataColumns(ByVal source As DataTable, ByVal destinationColumnSet As DataColumn()) As DataTable
            Return ReArrangeDataColumns(source, destinationColumnSet, Nothing)
        End Function
        ''' <summary>
        '''     Rearrange columns and also change their data types
        ''' </summary>
        ''' <param name="source">The source table with data</param>
        ''' <param name="destinationColumnSet">An array of columns as they shall be inserted into the result</param>
        ''' <param name="ignoreConversionExceptionAndLogThemHere">In case of data conversion exceptions, log them here instead of throwing them immediately</param>
        ''' <returns>A new and independent data table with copied data</returns>
        ''' <remarks>
        '''     The copy process requires that the names of the destination columns can be found in the columns collection of the source table. 
        ''' </remarks>
        ''' <example>
        '''     <code language="vb">
        '''         ReArrangeDataColumns(source, New System.Data.DataColumn() {New DataColumn("column1Name", GetType(String)), New DataColumn("column2Name", GetType(Integer))})
        '''     </code>
        ''' </example>
        Public Shared Function ReArrangeDataColumns(ByVal source As DataTable, ByVal destinationColumnSet As DataColumn(), ByVal ignoreConversionExceptionAndLogThemHere As ArrayList) As DataTable

            'Parameter validation
            If source Is Nothing Then
                Throw New ArgumentNullException("source")
            End If
            If destinationColumnSet Is Nothing Then
                Throw New ArgumentNullException("destinationColumnSet")
            ElseIf destinationColumnSet.Length = 0 Then
                Throw New ArgumentException("empty array not allowed", "destinationColumnSet")
            End If

            'Prepare new datatable
            Dim Result As New DataTable(source.TableName)
            For MyCounter As Integer = 0 To destinationColumnSet.Length - 1
                Result.Columns.Add(destinationColumnSet(MyCounter))
            Next

            'Prepare column wrap table
            Dim colWraps As New ArrayList
            For ColCounter As Integer = 0 To destinationColumnSet.Length - 1
                Try
                    colWraps.Add(source.Columns.Item(destinationColumnSet(ColCounter).ColumnName).Ordinal)
                Catch
                    Throw New Exception("Column name can't be found in source table's column collection: " & destinationColumnSet(ColCounter).ColumnName)
                End Try
            Next
            Dim ColumnWraps As Integer() = CType(colWraps.ToArray(GetType(Integer)), Integer())

            'Copy content
            For RowCounter As Integer = 0 To source.Rows.Count - 1

                Dim MyNewRow As DataRow = Result.NewRow

                For ColCounter As Integer = 0 To destinationColumnSet.Length - 1
                    Dim sourceData As Object = source.Rows(RowCounter)(ColumnWraps(ColCounter))
                    If sourceData Is Nothing OrElse IsDBNull(sourceData) = True Then
                        MyNewRow(ColCounter) = sourceData
                    Else
                        Try
                            MyNewRow(ColCounter) = sourceData
                        Catch ex As Exception
                            Dim conversionException As ReArrangeDataColumnsException
                            conversionException = New ReArrangeDataColumnsException(RowCounter, ColCounter, source.Columns(ColumnWraps(ColCounter)).DataType, Result.Columns(ColCounter).DataType, source.Rows(RowCounter)(ColumnWraps(ColCounter)), ex)
                            If ignoreConversionExceptionAndLogThemHere Is Nothing Then
                                Throw conversionException
                            Else
                                ignoreConversionExceptionAndLogThemHere.Add(conversionException)
                            End If
                        End Try
                    End If
                Next

                Result.Rows.Add(MyNewRow)

            Next

            'Return new datatable
            Return Result

        End Function

#End Region

    End Class

End Namespace