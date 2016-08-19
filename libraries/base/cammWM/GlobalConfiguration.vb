'Copyright 2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Public Class GlobalConfiguration

        Friend Sub New(webManager As IWebManager)
            If webManager Is Nothing Then Throw New ArgumentNullException("webManager")
            Me._WebManager = webManager
            Me._ProductKey = "camm WebManager"
        End Sub

        Protected Sub New(webManager As IWebManager, productKey As String)
            If webManager Is Nothing Then Throw New ArgumentNullException("webManager")
            If Trim(productKey) = "" Then Throw New ArgumentNullException("productKey")
            If productKey.ToLowerInvariant() = "camm webmanager" OrElse productKey.ToLowerInvariant() = "camm integrationportal" Then Throw New ArgumentException("Forbidden reserved product key """ & productKey & """", "productKey")
            Me._WebManager = webManager
            Me._ProductKey = productKey
        End Sub

        Private _WebManager As IWebManager
        Private _ProductKey As String

        Friend Class ConfigRecord

            Friend Sub New(key As String)
                If key = "" Then
                    Throw New ArgumentNullException("key")
                ElseIf key.Length > 128 Then
                    Throw New ArgumentOutOfRangeException("key", "Length of key is limited to max. 128 characters")
                Else
                    Me.Key = key
                End If
            End Sub

            Sub New(key As String, value As Long)
                Me.New(key)
                Me.Int64Value = value
            End Sub
            Sub New(key As String, value As DateTime)
                Me.New(key)
                Me.DateTimeValue = value
            End Sub
            Sub New(key As String, value As String)
                Me.New(key)
                Me.StringValue = value
            End Sub
            Sub New(key As String, value As Boolean)
                Me.New(key)
                Me.BooleanValue = value
            End Sub
            Sub New(key As String, value As Decimal)
                Me.New(key)
                Me.DecimalValue = value
            End Sub
            Sub New(key As String, value As Byte())
                Me.New(key)
                Me.ByteArrayValue = value
            End Sub

            ReadOnly Property Key As String
            Property Int64Value As Long?
            Property DateTimeValue As DateTime?
            Property StringValue As String
            Property BooleanValue As Boolean?
            Property DecimalValue As Decimal?
            Property ByteArrayValue As Byte()

            Friend ReadOnly Property IsWithoutAnyConfigData As Boolean
                Get
                    If Me.Int64Value.HasValue = False And
                        Me.DateTimeValue.HasValue = False And
                        Me.StringValue = Nothing And
                        (Me.ByteArrayValue Is Nothing OrElse Me.ByteArrayValue.Length = 0) And
                        Me.BooleanValue.HasValue = False And
                        Me.DecimalValue.HasValue = False And
                        Me.DateTimeValue.HasValue = False Then
                        Return True
                    Else
                        Return False
                    End If
                End Get
            End Property

        End Class

        ''' <summary>
        ''' Read configuration set based on 1 record per key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryConfigRecord(ByVal key As String) As ConfigRecord
            Dim ResultRecords As List(Of ConfigRecord) = Me.QueryConfigRecords(key)
            Select Case ResultRecords.Count
                Case 0
                    Return New ConfigRecord(key)
                Case 1
                    Return ResultRecords(0)
                Case Else 'multiple rows
                    Throw New Exception("More than 1 config record present, ConfigQueryEntries method required")
            End Select
        End Function

        ''' <summary>
        ''' Read configuration set based on 0 up to multiple records per key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryConfigRecords(ByVal key As String) As List(Of ConfigRecord)
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT PropertyName AS KeyName, ValueInt, ValueNText, ValueBoolean, ValueImage, ValueDecimal, ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Dim ValueSet As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "ConfigValueSet")
            Dim ResultRecords As New List(Of ConfigRecord)
            For MyCounter As Integer = 0 To ValueSet.Rows.Count - 1
                Dim ResultRecord As New ConfigRecord(CompuMaster.camm.WebManager.Utils.Nz(Of String)(ValueSet.Rows(0)("KeyName")))
                ResultRecord.Int64Value = CompuMaster.camm.WebManager.Utils.Nz(Of Long?)(ValueSet.Rows(0)("ValueInt"))
                ResultRecord.StringValue = CompuMaster.camm.WebManager.Utils.Nz(Of String)(ValueSet.Rows(0)("ValueNText"))
                ResultRecord.BooleanValue = CompuMaster.camm.WebManager.Utils.Nz(Of Boolean?)(ValueSet.Rows(0)("ValueBoolean"))
                ResultRecord.ByteArrayValue = CompuMaster.camm.WebManager.Utils.Nz(Of Byte())(ValueSet.Rows(0)("ValueImage"))
                ResultRecord.DecimalValue = CompuMaster.camm.WebManager.Utils.Nz(Of Decimal?)(ValueSet.Rows(0)("ValueDecimal"))
                ResultRecord.DateTimeValue = CompuMaster.camm.WebManager.Utils.Nz(Of DateTime?)(ValueSet.Rows(0)("ValueDateTime"))
                ResultRecords.Add(ResultRecord)
            Next
            Return ResultRecords
        End Function

        ''' <summary>
        ''' Read configuration set based on 0 up to multiple records per key
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryConfigRecordsWithSubKeys(ByVal key As String) As List(Of ConfigRecord)
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT PropertyName AS KeyName, ValueInt, ValueNText, ValueBoolean, ValueImage, ValueDecimal, ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName LIKE CAST(@key + N'%' AS nvarchar(128))", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key.Replace("[", "[[]").Replace("_", "[_]").Replace("%", "[%]")
            MyCmd.Connection = MyConn
            Dim ValueSet As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "ConfigValueSet")
            Dim ResultRecords As New List(Of ConfigRecord)
            For MyCounter As Integer = 0 To ValueSet.Rows.Count - 1
                Dim ResultRecord As New ConfigRecord(CompuMaster.camm.WebManager.Utils.Nz(Of String)(ValueSet.Rows(0)("KeyName")))
                ResultRecord.Int64Value = CompuMaster.camm.WebManager.Utils.Nz(Of Long?)(ValueSet.Rows(0)("ValueInt"))
                ResultRecord.StringValue = CompuMaster.camm.WebManager.Utils.Nz(Of String)(ValueSet.Rows(0)("ValueNText"))
                ResultRecord.BooleanValue = CompuMaster.camm.WebManager.Utils.Nz(Of Boolean?)(ValueSet.Rows(0)("ValueBoolean"))
                ResultRecord.ByteArrayValue = CompuMaster.camm.WebManager.Utils.Nz(Of Byte())(ValueSet.Rows(0)("ValueImage"))
                ResultRecord.DecimalValue = CompuMaster.camm.WebManager.Utils.Nz(Of Decimal?)(ValueSet.Rows(0)("ValueDecimal"))
                ResultRecord.DateTimeValue = CompuMaster.camm.WebManager.Utils.Nz(Of DateTime?)(ValueSet.Rows(0)("ValueDateTime"))
                ResultRecords.Add(ResultRecord)
            Next
            Return ResultRecords
        End Function

        ''' <summary>
        ''' Delete a configuration set based on 1 record per key
        ''' </summary>
        ''' <param name="key"></param>
        Friend Sub DeleteConfigRecord(ByVal key As String)
            Me.WriteConfigRecord(New ConfigRecord(key))
        End Sub

        ''' <summary>
        ''' Delete a configuration set based on 0 up to multiple records per key
        ''' </summary>
        ''' <param name="key"></param>
        Friend Sub DeleteConfigRecords(ByVal key As String)
            Me.WriteConfigRecords(key, New List(Of ConfigRecord))
        End Sub

        ''' <summary>
        ''' Save a configuration set based on 1 record per key to the central database
        ''' </summary>
        ''' <param name="data"></param>
        Friend Sub WriteConfigRecord(ByVal data As ConfigRecord)
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand()
            MyCmd.Connection = MyConn
            MyCmd.CommandText = "DECLARE @RowNumber int" & vbNewLine &
                        "SELECT @RowNumber = COUNT(*)" & vbNewLine &
                        "FROM [dbo].[System_GlobalProperties]" & vbNewLine &
                        "WHERE VALUENVarChar = @ProductName AND PropertyName = @key" & vbNewLine &
                        "SELECT @RowNumber" & vbNewLine &
                        vbNewLine &
                        "IF @RemoveOnly = 0 AND IsNull(@RowNumber,0) = 0 " & vbNewLine &
                        "	INSERT INTO [dbo].[System_GlobalProperties]" & vbNewLine &
                        "		(ValueNVarChar, PropertyName, ValueInt, ValueNText, ValueBoolean, ValueImage, ValueDecimal, ValueDateTime)" & vbNewLine &
                        "	VALUES (@ProductName, @key, @ValueInt, @ValueNText, @ValueBoolean, @ValueImage, @ValueDecimal, @ValueDateTime)" & vbNewLine &
                        "ELSE IF @RemoveOnly = 0 AND IsNull(@RowNumber,0) = 1" & vbNewLine &
                        "	UPDATE [dbo].[System_GlobalProperties]" & vbNewLine &
                        "	SET ValueInt = @ValueInt, ValueNText = @ValueNText, ValueBoolean = @ValueBoolean, ValueImage = @ValueImage, ValueDecimal = @ValueDecimal, ValueDateTime = @ValueDateTime" & vbNewLine &
                        "	WHERE ValueNVarChar = @ProductName AND PropertyName = @key" & vbNewLine &
                        "ELSE IF @RemoveOnly <> 0 AND IsNull(@RowNumber,0) = 1" & vbNewLine &
                        "	DELETE FROM [dbo].[System_GlobalProperties]" & vbNewLine &
                        "	WHERE ValueNVarChar = @ProductName AND PropertyName = @key" & vbNewLine &
                        "SELECT @RowNumber AS ExistingRowsCount" & vbNewLine
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@RemoveOnly", SqlDbType.Bit).Value = data.IsWithoutAnyConfigData
            MyCmd.Parameters.Add("@key", SqlDbType.VarChar).Value = data.Key
            MyCmd.Parameters.Add("@ValueInt", SqlDbType.Int).Value = Utils.NullableTypeWithItsValueOrDBNull(data.Int64Value)
            MyCmd.Parameters.Add("@ValueNText", SqlDbType.NText).Value = Utils.StringNotNothingOrDBNull(data.StringValue)
            MyCmd.Parameters.Add("@ValueBoolean", SqlDbType.Bit).Value = Utils.NullableTypeWithItsValueOrDBNull(data.BooleanValue)
            MyCmd.Parameters.Add("@ValueImage", SqlDbType.Image).Value = Utils.ArrayNotNothingOrDBNull(data.ByteArrayValue)
            MyCmd.Parameters.Add("@ValueDecimal", SqlDbType.Decimal).Value = Utils.NullableTypeWithItsValueOrDBNull(data.DecimalValue)
            MyCmd.Parameters.Add("@ValueDateTime", SqlDbType.DateTime).Value = Utils.NullableTypeWithItsValueOrDBNull(data.DateTimeValue)
            Dim ExistingRowNumbers As Long = CompuMaster.camm.WebManager.Utils.Nz(Of Long)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
            If ExistingRowNumbers > 1 Then
                'no data has been changed by SQL command above because of IF ELSE logic
                Throw New Exception("Multiple config recordy already exist with the given key """ & data.Key & """, this method ConfigWriteEntry can't be used for saving multi-record configuration data")
            End If
        End Sub

        ''' <summary>
        ''' Save a configuration set based on 0 up to multiple records per key to the central database
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="dataSet"></param>
        Friend Sub WriteConfigRecords(ByVal key As String, ByVal dataSet As List(Of ConfigRecord))
            If key = Nothing Then
                Throw New ArgumentNullException("key")
            ElseIf key.Length > 128 Then
                Throw New ArgumentOutOfRangeException("key", "Length of key is limited to max. 128 characters")
            Else
                For MyCounter As Integer = 0 To dataSet.Count - 1
                    If dataSet(MyCounter).Key <> key Then
                        Throw New ArgumentException("All keys in dataSet must equal to key parameter """ & key & """", "key")
                    End If
                Next
            End If
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand()
            MyCmd.Connection = MyConn
            Dim sql As New System.Text.StringBuilder
            sql.AppendLine("DELETE" & vbNewLine &
                        "FROM [dbo].[System_GlobalProperties]" & vbNewLine &
                        "WHERE VALUENVarChar = @ProductName AND PropertyName = @key")
            For MyCounter As Integer = 0 To dataSet.Count - 1
                If dataSet(MyCounter).IsWithoutAnyConfigData = False Then
                    sql.AppendLine("INSERT INTO [dbo].[System_GlobalProperties]" & vbNewLine &
                                "	(ValueNVarChar, PropertyName, ValueInt, ValueNText, ValueBoolean, ValueImage, ValueDecimal, ValueDateTime)" & vbNewLine &
                                "VALUES (@ProductName, @key, @Value" & MyCounter & "Int, @Value" & MyCounter & "NText, @Value" & MyCounter & "Boolean, @Value" & MyCounter & "Image, @Value" & MyCounter & "Decimal, @Value" & MyCounter & "DateTime)")
                    Dim data As ConfigRecord = dataSet(MyCounter)
                MyCmd.Parameters.Add(New SqlParameter("@Value" & MyCounter & "Int", SqlDbType.Int)).Value = Utils.NullableTypeWithItsValueOrDBNull(data.Int64Value)
                MyCmd.Parameters.Add(New SqlParameter("@Value" & MyCounter & "NText", SqlDbType.NText)).Value = Utils.StringNotNothingOrDBNull(data.StringValue)
                MyCmd.Parameters.Add(New SqlParameter("@Value" & MyCounter & "Boolean", SqlDbType.Bit)).Value = Utils.NullableTypeWithItsValueOrDBNull(data.BooleanValue)
                MyCmd.Parameters.Add(New SqlParameter("@Value" & MyCounter & "Image", SqlDbType.Image)).Value = Utils.ArrayNotNothingOrDBNull(data.ByteArrayValue)
                MyCmd.Parameters.Add(New SqlParameter("@Value" & MyCounter & "Decimal", SqlDbType.Decimal)).Value = Utils.NullableTypeWithItsValueOrDBNull(data.DecimalValue)
                    MyCmd.Parameters.Add(New SqlParameter("@Value" & MyCounter & "DateTime", SqlDbType.DateTime)).Value = Utils.NullableTypeWithItsValueOrDBNull(data.DateTimeValue)
                End If
            Next
            MyCmd.CommandText = sql.ToString
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryInt64ConfigEntry(ByVal key As String) As Long?
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueInt FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of Long?)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function

        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryStringConfigEntry(ByVal key As String) As String
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueNText FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of String)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function
        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryBooleanConfigEntry(ByVal key As String) As Boolean?
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueBoolean FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of Boolean?)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function
        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryByteArrayConfigEntry(ByVal key As String) As Byte()
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueImage FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of Byte())(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function
        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryDoubleConfigEntry(ByVal key As String) As Double?
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueDecimal FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of Double?)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function
        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryDecimalConfigEntry(ByVal key As String) As Decimal?
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueDecimal FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of Decimal?)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function
        ''' <summary>
        ''' Read configuration value
        ''' </summary>
        ''' <param name="key"></param>
        ''' <remarks></remarks>
        Friend Function QueryDateTimeConfigEntry(ByVal key As String) As DateTime?
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As SqlCommand = New SqlCommand("SELECT ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE ValueNVarChar = @ProductName AND PropertyName = @key", MyConn)
            MyCmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = Me._ProductKey
            MyCmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            MyCmd.Connection = MyConn
            Return CompuMaster.camm.WebManager.Utils.Nz(Of DateTime?)(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
        End Function
    End Class

End Namespace