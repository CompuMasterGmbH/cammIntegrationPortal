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

Imports System.IO
Imports System.Data
Imports System.Collections

Namespace CompuMaster.camm.SmartWebEditor.Data

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

            ''' -----------------------------------------------------------------------------
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
            ''' <history>
            ''' 	[wezel]	14.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Shared Function CreateConnection(ByVal assemblyName As String, ByVal connectionTypeName As String, ByVal connectionString As String) As IDbConnection
                Dim Result As IDbConnection = CreateConnection(assemblyName, connectionTypeName)
                Result.ConnectionString = connectionString
                Return Result
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Automations for the connection in charge
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[wezel]	19.01.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Friend Enum Automations As Integer
                None = 0
                AutoOpenConnection = 1
                AutoCloseAndDisposeConnection = 2
                AutoOpenAndCloseAndDisposeConnection = 3
            End Enum

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Executes a command without returning any data
            ''' </summary>
            ''' <param name="dbCommand">The command with an assigned connection property value</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	05.04.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Executes a command scalar and returns the value
            ''' </summary>
            ''' <param name="dbCommand">The command with an assigned connection property value</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	05.04.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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

            ''' -----------------------------------------------------------------------------
            ''' Project	 : kvp
            ''' Class	 : Tools.Data.DataQuery.AnyIDataProvider.DataException
            ''' 
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Data execution exceptions with details on the executed IDbCommand
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	23.06.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Convert the collection with all the parameters to a plain text string
                ''' </summary>
                ''' <param name="parameters">An IDataParameterCollection of a IDbCommand</param>
                ''' <returns></returns>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminwezel]	23.06.2005	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
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

                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     The complete and detailed exception information inclusive the command text
                ''' </summary>
                ''' <returns></returns>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminwezel]	23.06.2005	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                Public Overrides Function ToString() As String
                    Return MyBase.ToString & vbNewLine & vbNewLine & _commandText
                End Function
#End If
            End Class

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first column
            ''' </summary>
            ''' <param name="dbCommand">The command object which shall be executed</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	03.12.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Executes a command with a data reader and returns the values of the first two columns
            ''' </summary>
            ''' <param name="dbCommand">The prepared command to the database</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <returns>An array of DictionaryEntry with the values of the first column as the key element and the second column values in the value element of the DictionaryEntry</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	03.12.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Executes a command and return the data reader object for it
            ''' </summary>
            ''' <param name="dbCommand">The command with an assigned connection property value</param>
            ''' <param name="automations">Automation options for the connection</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	05.04.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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
                    MyReader = Data.DataQuery.AnyIDataProvider.ExecuteReader(dbCommand, Automation)
                    'Convert the reader to a data table
                    Result = Data.DataTables.ConvertDataReaderToDataTable(MyReader, tableName)
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

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Securely close and dispose a database connection
            ''' </summary>
            ''' <param name="connection">The connection to close and dispose</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	20.01.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Friend Shared Sub CloseAndDisposeConnection(ByVal connection As IDbConnection)
                Dim MyConn As IDbConnection = connection
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Securely close a database connection
            ''' </summary>
            ''' <param name="connection">The connection to close</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	20.01.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Friend Shared Sub CloseConnection(ByVal connection As IDbConnection)
                Dim MyConn As IDbConnection = connection
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Open a database connection if it is not already opened
            ''' </summary>
            ''' <param name="connection">The connection to open</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	20.01.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
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

        ''' <summary>
        ''' Methods for simplifying the handling with data readers
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Friend Class DataReaderUtils

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Lookup if the reader contains a result column with the requested name
            ''' </summary>
            ''' <param name="reader">A data reader object</param>
            ''' <param name="columnName">The name of the column which shall be identified</param>
            ''' <returns>True if the column exist else False</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[wezel]	22.10.2009	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Shared Function ContainsColumn(ByVal reader As IDataReader, ByVal columnName As String) As Boolean
                If reader Is Nothing Then Throw New ArgumentNullException("reader", "Parameter reader is required")
                If columnName = Nothing Then Throw New ArgumentNullException("columnName", "Parameter columnName can't be an empty value")
                For MyCounter As Integer = 0 To reader.FieldCount - 1
                    If LCase(reader.GetName(MyCounter)) = LCase(columnName) Then Return True
                Next
                Return False
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Return the column names of a data reader as a String array
            ''' </summary>
            ''' <param name="reader">A data reader object</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[wezel]	22.10.2009	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Shared Function ColumnNames(ByVal reader As IDataReader) As String()
                If reader Is Nothing Then Return Nothing
                Dim Result As New ArrayList
                For MyCounter As Integer = 0 To reader.FieldCount - 1
                    Result.Add(reader.GetName(MyCounter))
                Next
                Return CType(Result.ToArray(GetType(String)), String())
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Return the column data types of a data reader as an array
            ''' </summary>
            ''' <param name="reader">A data reader object</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[wezel]	22.10.2009	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Shared Function DataTypes(ByVal reader As IDataReader) As Type()
                If reader Is Nothing Then Return Nothing
                Dim Result As New ArrayList
                For MyCounter As Integer = 0 To reader.FieldCount - 1
                    Result.Add(reader.GetFieldType(MyCounter))
                Next
                Return CType(Result.ToArray(GetType(Type)), Type())
            End Function

        End Class

    End Namespace

    ''' <summary>
    '''     Common DataTable operations
    ''' </summary>
    Friend Class DataTables

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create a new data table clone with only some first rows
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="NumberOfRows">The number of rows to be copied</param>
        ''' <returns>The new clone of the datatable</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	28.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function GetDataTableWithSubsetOfRows(ByVal SourceTable As DataTable, ByVal NumberOfRows As Integer) As DataTable
            Return GetDataTableWithSubsetOfRows(SourceTable, 0, NumberOfRows)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create a new data table clone with only some first rows
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="StartAtRow">The position where to start the copy process, the first row is at 0</param>
        ''' <param name="NumberOfRows">The number of rows to be copied</param>
        ''' <returns>The new clone of the datatable</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	28.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a complete clone of a DataRow with structure as well as data
        ''' </summary>
        ''' <param name="sourceRow">The source row to be copied</param>
        ''' <returns>The new clone of the DataRow</returns>
        ''' <remarks>
        '''     The resulting DataRow has got the schema from the sourceRow's DataTable, but it hasn't been added to the table yet.
        ''' </remarks>
        ''' <history>
        ''' 	[baldauf]	2005-07-02  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateDataRowClone(ByVal sourceRow As DataRow) As DataRow
            If sourceRow Is Nothing Then Throw New ArgumentNullException("sourceRow")
            Dim Result As DataRow = sourceRow.Table.NewRow
            Result.ItemArray = sourceRow.ItemArray
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <returns>The new clone of the datatable</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	03.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function GetDataTableClone(ByVal SourceTable As DataTable) As DataTable
            Return GetDataTableClone(SourceTable, Nothing, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="RowFilter">An additional row filter, for all rows set it to null (Nothing in VisualBasic)</param>
        ''' <returns>The new clone of the datatable</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	03.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function GetDataTableClone(ByVal SourceTable As DataTable, ByVal RowFilter As String) As DataTable
            Return GetDataTableClone(SourceTable, RowFilter, Nothing, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a complete clone of a DataTable with structure as well as data
        ''' </summary>
        ''' <param name="SourceTable">The source table to be copied</param>
        ''' <param name="RowFilter">An additional row filter, for all rows set it to null (Nothing in VisualBasic)</param>
        ''' <param name="Sort">An additional sort command</param>
        ''' <param name="topRows">How many rows from top shall be returned as maximum?</param>
        ''' <returns>The new clone of the datatable</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	03.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Convert any opened datareader into a dataset
        ''' </summary>
        ''' <param name="dataReader">An already opened dataReader</param>
        ''' <returns>A dataset containing all datatables the dataReader was able to read</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	13.01.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function ConvertDataReaderToDataSet(ByVal datareader As IDataReader) As DataSet
            Dim Result As New DataSet
            Dim DRA As New DataReaderAdapter
            DRA.FillFromReader(Result, datareader)
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Convert any opened datareader into a data table
        ''' </summary>
        ''' <param name="dataReader">An already opened dataReader</param>
        ''' <returns>A data table containing all data the dataReader was able to read</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	13.01.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function ConvertDataReaderToDataTable(ByVal dataReader As IDataReader) As DataTable
            Return ConvertDataReaderToDataTable(dataReader, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Convert any opened datareader into a data table
        ''' </summary>
        ''' <param name="dataReader">An already opened dataReader</param>
        ''' <param name="tableName">The name for the new table</param>
        ''' <returns>A data table containing all data the dataReader was able to read</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	13.01.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

    End Class

End Namespace