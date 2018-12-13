'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Setup

    ''' <summary>
    ''' camm Web-Manager database setup and update
    ''' </summary>
    <CLSCompliant(False)> Public Class DatabaseSetup
        Inherits SetupBase
        Public Event StepStatusChanged()
        Public Event ProgressTaskStatusChanged()
        Public Event ProgressStepStatusChanged()

        Private _SetupPackageName As String
        Private _ProductName As String
        Private _ProductTitle As String

        Public Sub New(ByVal ProductName As String, ByVal ProductTitle As String)
            MyBase.New(ProductName)
            If Mid(ProductName, 1, 5) = "camm " Then
                Throw New Exception("Product name mustn't contain a ""camm""")
            End If
            _ProductName = "camm " & ProductName
            _ProductTitle = "camm " & ProductTitle
            _SetupPackageName = ProductName
        End Sub

#Region "Public interfaces and event raising"
        Private _CurrentStep As String
        Public ReadOnly Property CurrentStepTitle() As String
            Get
                Return _CurrentStep
            End Get
        End Property
        Private Sub SwitchToStep(ByVal Text As String)
            _CurrentStep = Text
            RaiseEvent StepStatusChanged()
        End Sub

        Public Structure ProgressStatus
            Dim CurrentStepTitle As String
            Dim StepsTotal As Integer
            Dim CurrentStepNumber As Integer
        End Structure

        Dim _ProgressOfTasks As ProgressStatus
        Dim _ProgressOfSteps As ProgressStatus
        ''' <summary>Main steps like database creation, each separate database update (build)</summary>
        Public ReadOnly Property ProgressOfTasks() As ProgressStatus
            Get
                Return _ProgressOfTasks
            End Get
        End Property

        ''' <summary>Sub steps of a database update (build)</summary>
        Public ReadOnly Property ProgressOfSteps() As ProgressStatus
            Get
                Return _ProgressOfSteps
            End Get
        End Property

        Private Sub UpdateProgressOfTasks(ByVal CurrentTaskTitle As String, ByVal CurrentStepNumber As Integer, ByVal TotalSteps As Integer)
            _ProgressOfTasks = New ProgressStatus
            _ProgressOfTasks.CurrentStepTitle = CurrentTaskTitle
            _ProgressOfTasks.CurrentStepNumber = CurrentStepNumber
            _ProgressOfTasks.StepsTotal = System.Math.Max(TotalSteps, CurrentStepNumber)
            _ProgressOfSteps = Nothing
            RaiseEvent ProgressTaskStatusChanged()
        End Sub

        Private Sub UpdateProgressOfSteps(ByVal CurrentStepTitle As String, ByVal CurrentStepNumber As Integer, ByVal TotalSteps As Integer)
            _ProgressOfSteps = New ProgressStatus
            _ProgressOfTasks.CurrentStepTitle = CurrentStepTitle
            _ProgressOfSteps.CurrentStepNumber = CurrentStepNumber
            _ProgressOfSteps.StepsTotal = System.Math.Max(TotalSteps, CurrentStepNumber)
            RaiseEvent ProgressStepStatusChanged()
        End Sub
#End Region

#Region "Main subs"
        Public Shared Function ResourceDataDatabaseSetup(ByVal name As String) As Byte()
#If UseLocalSQLs = True Then 'For debug/dev mode in DB Update wizard solution
            Return System.IO.File.ReadAllBytes(name)
#End If
            Dim Result As Byte()

            Dim ResMngr As Resources.ResourceManager = Nothing
            Try
                'Try using de-bzipped data, first
                ResMngr = New Resources.ResourceManager("dbsetup", System.Reflection.Assembly.GetAssembly(GetType(DatabaseSetup)))
                ResMngr.IgnoreCase = True
                Try
                    Result = Utils.Compression.DeCompress(CType(ResMngr.GetObject(name & "_bzip2"), Byte()), Utils.Compression.CompressionType.BZip2)
                Catch
                    Result = Utils.Compression.DeCompress(CType(ResMngr.GetObject(name), Byte()), Utils.Compression.CompressionType.BZip2)
                End Try
            Catch
                'UnBZipping hasn't worked, use raw data as last alternative
                If ResMngr Is Nothing Then
                    ResMngr = New Resources.ResourceManager("dbsetup", System.Reflection.Assembly.GetAssembly(GetType(DatabaseSetup)))
                End If
                ResMngr.IgnoreCase = True
                Result = CType(ResMngr.GetObject(name), Byte())
            Finally
                If Not ResMngr Is Nothing Then ResMngr.ReleaseAllResources()
            End Try

            Return Result

        End Function

        Public Shared Function ValidateResourceAccessability() As String
#If UseLocalSQLs = True Then 'For debug/dev mode in DB Update wizard solution
            Return "Running in debug/development mode for using local SQL files"
#End If
            Dim ErrorDetails As String = Nothing
            Dim name As String = "build_index.xml"
            Dim Result As String = Nothing
            Dim ResMngr As Resources.ResourceManager = Nothing
            Try
                'Try using de-bzipped data, first
                ResMngr = New Resources.ResourceManager("dbsetup", System.Reflection.Assembly.GetAssembly(GetType(DatabaseSetup)))
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(name & "_bzip2")
                If Result <> Nothing Then
                    ErrorDetails &= vbNewLine & "Resource key """ & name & "_bzip2"" is existing and contains some data --> OK"
                Else
                    ErrorDetails &= vbNewLine & "Resource key """ & name & "_bzip2"" is existing BUT is missing data --> FAILURE"
                End If
                Try
                    Result = Utils.Compression.DeCompress(Result, Utils.Compression.CompressionType.BZip2)
                    ErrorDetails &= vbNewLine & "Resource key """ & name & "_bzip2"" decompressing successful --> OK"
                Catch ex As Exception
                    ErrorDetails &= vbNewLine & "Resource key """ & name & "_bzip2"" can't be decompressed (" & ex.Message & ") --> FAILURE"
                End Try
            Catch ex As Exception
                ErrorDetails &= vbNewLine & "Exception has been thrown (" & ex.Message & ") --> FAILURE"

                'UnBZipping hasn't worked, use raw data as last alternative
                If ResMngr Is Nothing Then
                    ResMngr = New Resources.ResourceManager("dbsetup", System.Reflection.Assembly.GetAssembly(GetType(DatabaseSetup)))
                End If
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(name)
                If Result <> Nothing Then
                    ErrorDetails &= vbNewLine & "Resource key """ & name & """ is existing and contains some data --> OK"
                Else
                    ErrorDetails &= vbNewLine & "Resource key """ & name & """ is existing BUT is missing data --> FAILURE"
                End If
            Finally
                If Not ResMngr Is Nothing Then ResMngr.ReleaseAllResources()
            End Try

            Return Mid(ErrorDetails, 3)
        End Function

        Public Shared Function ResourceStringDatabaseSetup(ByVal name As String) As String
#If UseLocalSQLs = True Then 'For debug/dev mode in DB Update wizard solution
            Return System.IO.File.ReadAllText(name)
#End If
            Dim Result As String = Nothing

            Dim ResMngr As Resources.ResourceManager = Nothing
            Try
                'Try using de-bzipped data, first
                ResMngr = New Resources.ResourceManager("dbsetup", System.Reflection.Assembly.GetAssembly(GetType(DatabaseSetup)))
                ResMngr.IgnoreCase = True
                Result = Utils.Compression.DeCompress(ResMngr.GetString(name & "_bzip2"), Utils.Compression.CompressionType.BZip2)
            Catch
                'UnBZipping hasn't worked, use raw data as last alternative
                If ResMngr Is Nothing Then
                    ResMngr = New Resources.ResourceManager("dbsetup", System.Reflection.Assembly.GetAssembly(GetType(DatabaseSetup)))
                End If
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(name)
            Finally
                If Not ResMngr Is Nothing Then ResMngr.ReleaseAllResources()
            End Try

            Return Result

        End Function

        Public Function ResourceSqlDBSetup(ByVal name As String) As String
            Dim Result As String = Nothing
            Try
                Result = ResourceStringDatabaseSetup(name)
            Catch
                RaiseWarning("Embedded string resource """ & name & """ can't be found, search for file, now")
            End Try
            Return Result
        End Function

        Public Function ResourceSqlDBSetupBinary(ByVal name As String) As Byte()
            Dim Result As Byte() = Nothing
            Try
                Result = ResourceDataDatabaseSetup(name)
            Catch
                RaiseWarning("Embedded binary resource """ & name & """ can't be found, search for file, now")
            End Try
            Return Result
        End Function

        '================================================================================== _
        'TODO: Korrekten DB-Server-Provider ansprechen bzw. Installation/Update verweigern! 
        '================================================================================== _

        Private _UpdatesOnly As Boolean = False
        Private _UpdatesOnlyDirty As Boolean = False
        ''' <summary>
        '''     Are only updates or also creation of the databases allowed 
        ''' </summary>
        ''' <value></value>
        Public Property UpdatesOnly() As Boolean
            Get
                If _UpdatesOnlyDirty = False AndAlso Not System.Web.HttpContext.Current Is Nothing Then
                    'for Web scenarios, update-only is the default; windows applications may create
                    _UpdatesOnly = True
                End If
                Return _UpdatesOnly
            End Get
            Set(ByVal Value As Boolean)
                _UpdatesOnly = Value
                _UpdatesOnlyDirty = True
            End Set
        End Property

        Public Function GetWebManagerReplacements(ByVal ServerIP As String, ByVal ServerProtocol As String, ByVal ServerName As String, ByVal ServerPort As String, ByVal SGroupTitle As String, ByVal SGroupNavTitle As String, ByVal SGroupContact As String, ByVal CompanyName As String, ByVal CompanyFormerName As String, ByVal CompanyURL As String) As System.Collections.Specialized.NameValueCollection
            Dim ReplacementsInInitSQL As New System.Collections.Specialized.NameValueCollection
            If ServerIP <> "" Then
                ReplacementsInInitSQL.Add("<%Server_IP%>", GetSQLFormatting_StringValue(ServerIP))
            Else
                ReplacementsInInitSQL.Add("<%Server_IP%>", GetSQLFormatting_StringValue("localhost"))
            End If
            If ServerProtocol <> "" Then
                ReplacementsInInitSQL.Add("<%Server_Protocol%>", GetSQLFormatting_StringValue(ServerProtocol))
            Else
                ReplacementsInInitSQL.Add("<%Server_Protocol%>", GetSQLFormatting_StringValue("https"))
            End If
            If ServerName <> "" Then
                ReplacementsInInitSQL.Add("<%Server_Name%>", GetSQLFormatting_StringValue(ServerName))
            Else
                ReplacementsInInitSQL.Add("<%Server_Name%>", GetSQLFormatting_StringValue("localhost"))
            End If
            If ServerPort <> "" Then
                ReplacementsInInitSQL.Add("<%Server_Port%>", GetSQLFormatting_StringValue(ServerPort))
            Else
                ReplacementsInInitSQL.Add("<%Server_Port%>", "NULL")
            End If
            If SGroupTitle <> "" Then
                ReplacementsInInitSQL.Add("<%SGroup_Title%>", GetSQLFormatting_StringValue(SGroupTitle))
            Else
                ReplacementsInInitSQL.Add("<%SGroup_Title%>", GetSQLFormatting_StringValue("Your Company"))
            End If
            If SGroupNavTitle <> "" Then
                ReplacementsInInitSQL.Add("<%SGroup_NavTitle%>", GetSQLFormatting_StringValue(SGroupNavTitle))
            Else
                ReplacementsInInitSQL.Add("<%SGroup_NavTitle%>", GetSQLFormatting_StringValue("Your Company"))
            End If
            If SGroupContact <> "" Then
                ReplacementsInInitSQL.Add("<%SGroup_Contact%>", GetSQLFormatting_StringValue(SGroupContact))
            Else
                ReplacementsInInitSQL.Add("<%SGroup_Contact%>", GetSQLFormatting_StringValue("info@yourcompany.com"))
            End If
            If CompanyName <> "" Then
                ReplacementsInInitSQL.Add("<%Company_Name%>", GetSQLFormatting_StringValue(CompanyName))
            Else
                ReplacementsInInitSQL.Add("<%Company_Name%>", GetSQLFormatting_StringValue("Your Company"))
            End If
            If CompanyFormerName <> "" Then
                ReplacementsInInitSQL.Add("<%Company_FormerName%>", GetSQLFormatting_StringValue(CompanyFormerName))
            Else
                ReplacementsInInitSQL.Add("<%Company_FormerName%>", GetSQLFormatting_StringValue("Your Company GmbH"))
            End If
            If CompanyURL <> "" Then
                ReplacementsInInitSQL.Add("<%Company_URL%>", GetSQLFormatting_StringValue(CompanyURL))
            Else
                ReplacementsInInitSQL.Add("<%Company_URL%>", GetSQLFormatting_StringValue("http://www.YourCompany.com/"))
            End If
            Return ReplacementsInInitSQL
        End Function

        Public Sub WriteErrorMessage(ByVal ErrMsg As String)
            Dim Msg As String = Nothing
            Msg += "==============================================" & vbNewLine
            Msg += Now() & vbNewLine
            Msg += "==============================================" & vbNewLine
            Msg += ErrMsg & vbNewLine
            WriteToLog(Msg)
        End Sub

        Public Sub WriteErrorMessage(ByVal ex As SqlException, ByVal MySql As String, ByVal ErrorIndex As String)
            Dim i As Integer
            Dim Msg As String = ""
            For i = 0 To ex.Errors.Count - 1
                Msg += "ErrorIndex #" & i.ToString() & vbNewLine
                Msg += "Message: " & ex.Errors(i).Message & vbNewLine
                Msg += "LineNumber: " & ex.Errors(i).LineNumber & vbNewLine
                Msg += "Source: " & ex.Errors(i).Source & vbNewLine
                Msg += "Procedure: " & ex.Errors(i).Procedure & vbNewLine
                If DebugLevel >= 3 Then Msg += "SQL BEGIN ==>" & vbNewLine & MySql & vbNewLine & "<== SQL END" & vbNewLine
            Next i
            WriteErrorMessage(Msg)
            If Me._LogFileEnabled Then
                RaiseWarning("Error " & ErrorIndex & " found - please check the log file " & _LogFile & "!" & vbNewLine & ex.Errors(0).Message)
            Else
                RaiseWarning("Error " & ErrorIndex & " found!" & vbNewLine & ex.Errors(0).Message)
            End If
        End Sub

        Public Function DatabaseConnect(ByVal ConnectionString As String) As Boolean
            Dim DBconn As New SqlConnection
            If ConnectionString = "" Then Return False
            Dim MyResult As Boolean = True
            DBconn.ConnectionString = ConnectionString
            Try
                DBconn.Open()
            Catch ex As Exception
                If DebugLevel > 0 Then WriteToLog("Error DatabaseConnect:" & ex.GetBaseException.Message)
                MyResult = False
            Finally
                If DBconn.State <> ConnectionState.Closed Then DBconn.Close()
                If Not DBconn Is Nothing Then DBconn.Dispose()
            End Try
            Return MyResult
        End Function

        Public Function ResetOldDatabase(connectionString As String) As Boolean
            If connectionString = "" Then Return False

            ' We try to put a simple drop statement to the database server
            Dim DropConn As New SqlConnection
            DropConn.ConnectionString = connectionString
            Dim DBCmd As New SqlCommand
            Dim MyResult As Boolean = True
            Dim MyCurSQLCmdText4ResettingDB As String = ResourceSqlDBSetup("reset-sql-azure-db.sql")
            DBCmd = New SqlClient.SqlCommand
            Try
                DropConn.Open()
                DBCmd.Connection = DropConn
                DBCmd.CommandText = MyCurSQLCmdText4ResettingDB
                DBCmd.CommandType = CommandType.Text
                DBCmd.CommandTimeout = 300 '5 minutes
                DBCmd.ExecuteNonQuery()
            Catch ex As System.Data.SqlClient.SqlException
                ' Maybe it doesn't really matter if there was an error, as it only means that we could not
                ' drop the existing db, but this does not mean, we could not install anything on it
                WriteErrorMessage(ex, MyCurSQLCmdText4ResettingDB, "#78reset")
                MyResult = False
            Finally
                DBCmd.Dispose()
                If Not DBCmd Is Nothing Then DBCmd.Dispose()
                If DropConn.State <> ConnectionState.Closed Then DropConn.Close()
                DropConn.Dispose()
            End Try
            Return MyResult
        End Function

        Public Function DropOldDatabase(ByVal DatabaseName As String, ByVal ConnectionString As String) As Boolean
            If ConnectionString = "" Or DatabaseName = "" Then Return False

            ' We try to put a simple drop statement to the database server
            Dim DropConn As New SqlConnection
            DropConn.ConnectionString = ConnectionString
            Dim DBCmd As New SqlCommand
            Dim MyResult As Boolean = True
            Dim MyCurSQLCmdText4DroppingDB As String = "IF EXISTS (SELECT name FROM master.dbo.sysdatabases " & "WHERE name = N'" & DatabaseName.Replace("'", "''") & "') " & "DROP DATABASE [" & DatabaseName & "]"
            DBCmd = New SqlClient.SqlCommand
            Try
                DropConn.Open()
                DBCmd.Connection = DropConn
                DBCmd.CommandText = MyCurSQLCmdText4DroppingDB
                DBCmd.CommandType = CommandType.Text
                DBCmd.CommandTimeout = 300 '5 minutes
                DBCmd.ExecuteNonQuery()
            Catch ex As System.Data.SqlClient.SqlException
                ' Maybe it doesn't really matter if there was an error, as it only means that we could not
                ' drop the existing db, but this does not mean, we could not install anything on it
                WriteErrorMessage(ex, MyCurSQLCmdText4DroppingDB, "#78")
                MyResult = False
            Finally
                DBCmd.Dispose()
                If Not DBCmd Is Nothing Then DBCmd.Dispose()
                If DropConn.State <> ConnectionState.Closed Then DropConn.Close()
                DropConn.Dispose()
            End Try
            Return MyResult
        End Function

        Public Function CreateNewDatabase(ByVal ConnectionString As String, ByVal DatabaseName As String) As Boolean
            If ConnectionString = "" Then Return False

            'Create Database
            Dim MyDBCreationSQLCmdText As String = "IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'" & DatabaseName.Replace("'", "''") & "') " & "CREATE DATABASE [" & DatabaseName & "]"
            Dim DBconn As New SqlConnection
            Dim DBCmd As New SqlClient.SqlCommand
            Dim MyResult As Boolean = True
            DBconn.ConnectionString = ConnectionString
            Try
                DBconn.Open()
                DBCmd.Connection = DBconn
                DBCmd.CommandText = MyDBCreationSQLCmdText
                DBCmd.CommandType = CommandType.Text
                DBCmd.CommandTimeout = 300 '5 minutes
                DBCmd.ExecuteNonQuery()
            Catch ex As SqlException
                MyResult = False
                WriteErrorMessage(ex, MyDBCreationSQLCmdText, "#79a")
            Finally
                If Not DBCmd Is Nothing Then DBCmd.Dispose()
                If DBconn.State <> ConnectionState.Closed Then DBconn.Close()
                DBconn.Dispose()
            End Try
            Return MyResult
        End Function

        <Obsolete("Parameter databaseNamePublic outdated")> Function InitDatabase(ByVal DatabaseName As String, ByVal ConnectionString As String) As Boolean
            Return InitDatabase(ConnectionString)
        End Function
        Public Function InitDatabase(ByVal ConnectionString As String) As Boolean

            If UpdatesOnly Then Throw New Exception("Initialization of database forbidden; only maintenance updates allowed")

            If ConnectionString = "" Then Return False

            Dim MyResult As Boolean = True
            'Initialize Database
            Dim MyCurSQLCmdText As String
            Dim MySQLCmdTextReader As System.IO.TextReader
#If UseLocalSQLs Then
            Dim MySQLCmdText As String = ResourceSqlDBSetup("camm_WebManager_InitDatabase.sql")
#Else
            Dim MySQLCmdText As String = ResourceSqlDBSetup("InitDatabase.sql")
#End If
            If MySQLCmdText = "" Then
                Try
                    MySQLCmdTextReader = New System.IO.StreamReader(_CurWorkDir & "camm_" & _SetupPackageName & "_InitDatabase.sql")
                    MySQLCmdText = MySQLCmdTextReader.ReadToEnd
                    MySQLCmdTextReader.Close()
                Catch ex As Exception
                    WriteErrorMessage("Message: " & ex.GetBaseException.ToString)
                    If Me._LogFileEnabled Then
                        RaiseWarning("Error #81 found - please check the log file " & _LogFile & "!")
                    Else
                        RaiseWarning("Error #81 found!")
                    End If
                    MyResult = False

                End Try
            End If

            ' If connection Ok and Data available we can initialize the db
            If MyResult Then
                Dim DBConn As New SqlConnection
                DBConn.ConnectionString = ConnectionString
                Dim MyCurSQLCmdCollection As Collection = SplitSQLCmdByGOCommand(MySQLCmdText)
                Dim MyStepCounter As Integer = 0
                Dim MyStepCount As Integer = MyCurSQLCmdCollection.Count
                Dim DBCmd As New SqlClient.SqlCommand
                DBCmd.Connection = DBConn
                For Each MyCurSQLCmdText In MyCurSQLCmdCollection
                    'Status field update
                    MyStepCounter += 1
                    SwitchToStep("Initializing database..." & vbNewLine & "Current step: " & MyStepCounter & " / " & MyStepCount)
                    UpdateProgressOfSteps("Current step:", MyStepCounter, MyStepCount)
                    Dim MyCmdExtProps As TSpecialUpdateBlockProperties = GetSpecialUpdateBlockProperties(MyCurSQLCmdText)
                    'Execute SQL 
                    Try
                        DBConn.Open()
                        DBCmd.Connection = DBConn
                        DBCmd.CommandText = MyCmdExtProps.CommandText
                        DBCmd.CommandType = CommandType.Text
                        DBCmd.CommandTimeout = 300 '5 minutes
                        DBCmd.ExecuteNonQuery()
                        DBCmd.Dispose()
                    Catch ex As SqlException
                        Dim Msg As String = ""
                        Dim i As Integer
                        If Not MyCmdExtProps.IgnoreSQLErrors Then
                            For i = 0 To ex.Errors.Count - 1
                                Msg += "ErrorIndex #" & i.ToString() & vbNewLine
                                Msg += "Message: " & ex.Errors(i).Message & vbNewLine
                                Msg += "LineNumber: " & ex.Errors(i).LineNumber & vbNewLine
                                Msg += "Source: " & ex.Errors(i).Source & vbNewLine
                                Msg += "Procedure: " & ex.Errors(i).Procedure & vbNewLine
                                Msg += "Current step: " & MyStepCounter & " / " & MyCurSQLCmdCollection.Count & vbNewLine
                                If DebugLevel >= 3 Then Msg += "SQL BEGIN ==>" & vbNewLine & MyCurSQLCmdText & vbNewLine & "<== SQL END" & vbNewLine
                            Next i
                            WriteErrorMessage(Msg)
                            If _LogFileEnabled Then
                                RaiseWarning("Error #79b found - please check the log file " & _LogFile & "!")
                            Else
                                RaiseWarning("Error #79b found!")
                            End If
                        End If
                        MyResult = False
                    Finally
                        If DBConn.State <> ConnectionState.Closed Then DBConn.Close()
                    End Try
                Next
                If Not DBConn Is Nothing Then DBConn.Dispose()
                If Not DBCmd Is Nothing Then DBCmd.Dispose()
            End If
            Return MyResult
        End Function

        Public Function TruncateLog(ByVal ConnectionString As String, ByVal DatabaseName As String) As Boolean
            ' Truncating database transaction log - this makes sense 
            ' if the db has already existed before (re-) installing it.
            SwitchToStep("Connecting database server...")
            UpdateProgressOfSteps("Connecting database server...", 1, 2)
            Dim DBConn As New SqlConnection
            DBConn.ConnectionString = ConnectionString
            Dim MyResult As Boolean = True
            ' First we try to look if the server exists
            Try
                DBConn.Open()
                'Drop old transaction log
                Dim MyCurSQLCmdText4DroppingDB As String = "BACKUP LOG [" & DatabaseName & "] WITH TRUNCATE_ONLY"
                SwitchToStep("Truncating transaction log...")
                UpdateProgressOfSteps("Truncating transaction log...", 2, 2)
                Dim DBCmd As New SqlClient.SqlCommand
                Try
                    DBCmd.Connection = DBConn
                    DBCmd.CommandText = MyCurSQLCmdText4DroppingDB
                    DBCmd.CommandType = CommandType.Text
                    DBCmd.CommandTimeout = 300 '5 minutes
                    DBCmd.ExecuteNonQuery()
                    DBCmd.Dispose()
                Catch ex As SqlException
                    WriteErrorMessage(ex, MyCurSQLCmdText4DroppingDB, "#78b")
                    MyResult = False
                Finally
                    If Not DBCmd Is Nothing Then DBCmd.Dispose()
                End Try
            Catch e As Exception
                Dim ErrMsg As String = "Error connecting database server!" & vbNewLine & e.GetBaseException.ToString
                WriteErrorMessage(ErrMsg)
                If System.Web.HttpContext.Current Is Nothing Then RaiseWarning(ErrMsg)
                MyResult = False
            Finally
                If DBConn.State <> ConnectionState.Closed Then
                    DBConn.Close()
                    DBConn.Dispose()
                End If
            End Try
            Return MyResult
        End Function

        Public Function CreateDatabase(ByVal ConnectionStringToDatabase As String, ByVal ConnectionStringServerAdministration As String, ByVal DatabaseName As String, Optional ByVal doNotDropAndReCreateSqlDatabase As Boolean = False) As Boolean
            Return CreateDatabase(ConnectionStringToDatabase, ConnectionStringServerAdministration, DatabaseName, doNotDropAndReCreateSqlDatabase, False)
        End Function

        Public Function AddUser(ByVal ConnectionString As String, ByVal DatabaseName As String, ByVal UserName As String, ByVal UserPassword As String) As Boolean
            If ConnectionString = "" Then Return False
            Dim Myresult As Boolean = True
            Dim AddConn As New SqlConnection
            AddConn.ConnectionString = ConnectionString
            Dim AddCmd As New SqlCommand
            Try
                AddConn.Open()
                AddCmd.Connection = AddConn
                'USE "customized.test.cammwebmanager"
                'EXEC sp_addlogin 'kottenhahn', 'test', 'customized.test.cammwebmanager'
                'EXEC sp_grantdbaccess 'kottenhahn', 'kottenhahn'
                'EXEC sp_addrolemember 'db_owner', 'kottenhahn'
                AddCmd.CommandText = "USE [" & DatabaseName & "]" & vbNewLine
                AddCmd.CommandText += "EXEC sp_addlogin '" & UserName & "', '" & UserPassword & "', '" & DatabaseName & "'" & vbNewLine
                AddCmd.CommandText += "EXEC sp_grantdbaccess 'kottenhahn', 'kottenhahn'" & vbNewLine
                AddCmd.CommandText += "EXEC sp_addrolemember 'db_owner', '" & UserName & "'"
                AddCmd.ExecuteNonQuery()
            Catch ex As SqlException
                ' Hey! What went wrong?
                WriteErrorMessage("Error when adding User: " & ex.Number.ToString & ": " & ex.Message)
                Myresult = False
            Finally
                If Not AddCmd Is Nothing Then AddCmd.Dispose()
                If AddConn.State <> ConnectionState.Closed Then AddConn.Close()
                If Not AddConn Is Nothing Then AddConn.Dispose()
            End Try
            Return Myresult
        End Function

        Public Function CreateDatabase( _
         ByVal ConnectionStringToDatabase As String, _
         ByVal ConnectionStringServerAdministration As String, _
         ByVal DatabaseName As String, _
         ByVal doNotDropAndReCreateSqlDatabase As Boolean, _
         ByVal TruncateDatabaseTransactionLog As Boolean, _
         Optional ByVal ConnectionStringServerAdministration_sa As String = "", _
         Optional ByVal UserName As String = "", _
         Optional ByVal UserPassword As String = "", _
         Optional ByVal AdminUserName As String = "", _
         Optional ByVal AdminPassword As String = "") As Boolean

            If UpdatesOnly Then Throw New Exception("Creation of databases forbidden; only maintenance updates allowed")

            Dim MyResult As Boolean = True

            If Not TruncateDatabaseTransactionLog Then

                'Create db new
                Dim MaxTasks As Integer = CType(IIf(doNotDropAndReCreateSqlDatabase, 1, 2), Integer)
                Dim CurTaskNo As Integer = 1
                Dim MaxSteps As Integer = CType(IIf(AdminUserName <> "" And AdminPassword <> "", 4, 3), Integer)
                Dim CurStepNo As Integer = 1

                UpdateProgressOfTasks("Creation of database...", CurTaskNo, MaxTasks)

                SwitchToStep("Connecting database server...")
                UpdateProgressOfSteps("Connecting database server...", 1, MaxSteps)
                If Not (DatabaseConnect(ConnectionStringServerAdministration) OrElse DatabaseConnect(ConnectionStringServerAdministration_sa)) Then
                    MyResult = False
                Else

                    If AdminUserName <> "" And AdminPassword <> "" Then
                        SwitchToStep("Adding user (if needed)...")
                        CurStepNo += 1
                        UpdateProgressOfSteps("Add User ", CurStepNo, MaxSteps)
                        AddUser(ConnectionStringServerAdministration_sa, DatabaseName, UserName, UserPassword)
                    End If

                    If Not doNotDropAndReCreateSqlDatabase Then
                        'Drop old database
                        SwitchToStep("Dropping old database (if one exists)...")
                        CurStepNo += 1
                        UpdateProgressOfSteps("Dropping old database (if one exists)...", CurStepNo, MaxSteps)
                        If DropOldDatabase(DatabaseName, ConnectionStringServerAdministration) Then
                            CurStepNo += 1
                            SwitchToStep("Create new database...")
                            UpdateProgressOfSteps("Create database", CurStepNo, MaxSteps)
                            'Create
                            MyResult = CreateNewDatabase(ConnectionStringServerAdministration, DatabaseName)
                        End If
                    End If

                    If MyResult Then
                        ' Init db
                        CurTaskNo += 1
                        UpdateProgressOfTasks("Initializing database...", CurTaskNo, MaxTasks)
                        Try
                            MyResult = InitDatabase(ConnectionStringToDatabase)
                        Catch ex As Exception
                            WriteErrorMessage("Unhandled exception in InitDatabase" & ex.Message)
                            MyResult = False
                        End Try
                    End If
                End If
            Else
                ' We will only truncate the db log - if it makes sense?
                MyResult = TruncateLog(ConnectionStringServerAdministration, DatabaseName)
            End If
            Return MyResult
        End Function

        Public Shared Function LastBuildVersionInSetupFiles() As Version
            'Get builds overview
            Dim ds_build_index As New Data.DataSet
            Dim indexdata As New Data.DataView
            Dim Stream As System.IO.TextReader = Nothing
            Try
                Dim BuildIndex As String = ResourceStringDatabaseSetup("build_index.xml")
                If BuildIndex = Nothing Then
                    Throw New Exception("Resources for update not available; these are the error details: " & ValidateResourceAccessability())
                End If
                Stream = New System.IO.StringReader(BuildIndex)
                ds_build_index.ReadXml(Stream)
            Finally
                If Not Stream Is Nothing Then Stream.Close()
            End Try
            indexdata = ds_build_index.Tables("files").DefaultView
            indexdata.Sort = "ID"

            'Enumerate build infos
            Dim ds_data As New Data.DataSet
            Dim Result As Version = Nothing
            Dim MyRow As Data.DataRowView = indexdata.Item(indexdata.Count - 1)
            Dim BuildMetaData As String = ResourceStringDatabaseSetup(CType(MyRow("file"), String))
            If BuildMetaData <> "" Then
                Try
                    Stream = New System.IO.StringReader(BuildMetaData)
                    ds_data.ReadXml(Stream)
                    Dim MyDataRow As Data.DataRow = ds_data.Tables("version").Rows(ds_data.Tables("version").Rows.Count - 1)
                    Result = New Version(CType(MyDataRow("major"), Integer), CType(MyDataRow("minor"), Integer), CType(MyDataRow("build"), Integer))
                Catch ex As Exception
                    Throw New Exception("Error reading meta data in " & CType(MyRow("file"), String), ex)
                Finally
                    If Not Stream Is Nothing Then Stream.Close()
                End Try
            End If
            Return Result

        End Function

        Public Sub DoUpdates(ByVal ConnectionString As String, ByVal ReplacementsInInitSQL As System.Collections.Specialized.NameValueCollection)
            DoUpdates(ConnectionString, ReplacementsInInitSQL, Integer.MaxValue)
        End Sub
        Public Sub DoUpdates(ByVal ConnectionString As String, ByVal ReplacementsInInitSQL As System.Collections.Specialized.NameValueCollection, ByVal upToBuildNo As Integer)
            Dim DBConn As New SqlConnection
            Dim DBCmd As SqlCommand = Nothing
            Dim ListOfAppliedPatches As String = Nothing

            DBConn.ConnectionString = ConnectionString
            UpdateProgressOfTasks("Searching for patches...", 0, 0)

            'DB-Connect
            SwitchToStep("Connecting database...")
            Try
                DBConn.Open()
            Catch e As Exception
                RaiseWarning("Error connecting database!" & ControlChars.NewLine & ControlChars.NewLine & e.GetBaseException.ToString)
                If DBConn.State <> ConnectionState.Closed Then
                    DBConn.Close()
                End If
                Throw New Exception("Warnings/Errors have been logged")
                Exit Sub
            End Try

            'Step through all update files
            Dim ds_build_index As New Data.DataSet
            Dim indexdata As New Data.DataView
            SwitchToStep("Searching for updates...")
            Try
#If UseLocalSQLs Then
                Dim BuildIndex As String = ResourceSqlDBSetup("camm_WebManager_build_index.xml")
#Else
                Dim BuildIndex As String = ResourceSqlDBSetup("build_index.xml")
#End If
                If BuildIndex <> "" Then
                    Dim Stream As System.IO.TextReader = New System.IO.StringReader(BuildIndex)
                    ds_build_index.ReadXml(Stream)
                    Stream.Close()
                Else
                    ds_build_index.ReadXml(_CurWorkDir & "camm_" & _SetupPackageName & "_build_index.xml")
                End If
            Catch
                RaiseWarning("Error #83 found - patch index file not found!")
                If DBConn.State <> ConnectionState.Closed Then DBConn.Close()
                Throw New Exception("Warnings/Errors have been logged")
                Exit Sub
            End Try
            indexdata = ds_build_index.Tables("files").DefaultView
            indexdata.Sort = "ID"
            Dim MyTaskCounter As Integer = 0
            For MyCounter As Integer = 0 To indexdata.Count - 1
                Dim MyRow As Data.DataRowView = indexdata.Item(MyCounter)
                Dim DBBuildNo As Long = 0
                DBBuildNo = GetCurrentDBBuildNo(DBConn, DebugLevel)

                'XML-DBBuildNo
                Dim ds_data As New Data.DataSet
                Dim SQLCodeMajorVer As Long = 0
                Dim SQLCodeMinorVer As Long = 0
                Dim SQLCodeBuildNo As Long = 0
                Dim RequiredDBBuildNo As Long = -1
                Dim RequirementsMatched As Boolean = False

                Dim BuildMetaData As String = ResourceSqlDBSetup(CType(MyRow("file"), String))
                If BuildMetaData <> "" Then
                    Dim Stream As System.IO.TextReader = New System.IO.StringReader(BuildMetaData)
                    ds_data.ReadXml(Stream)
                    Stream.Close()
                Else
                    ds_data.ReadXml(_CurWorkDir & CType(MyRow("file"), String))
                End If

                Dim SqlServerVersion As DBServerVersion = GetSQLServerVersion(ConnectionString)
                Dim MyDataRow As Data.DataRow
                For Each MyDataRow In ds_data.Tables("version").Rows
                    SQLCodeMajorVer = CType(MyDataRow("major"), Integer)
                    SQLCodeMinorVer = CType(MyDataRow("minor"), Integer)
                    SQLCodeBuildNo = CType(MyDataRow("build"), Integer)
                Next
                For Each MyDataRow In ds_data.Tables("updatedetails").Rows
                    Try
                        RequiredDBBuildNo = CType(MyDataRow("BuildFor"), Integer)
                    Catch
                        Throw New Exception("Found datatype for field [BuildFor]: " & MyDataRow("BuildFor").GetType.ToString)
                    End Try
                    If DBBuildNo = RequiredDBBuildNo Then
                        RequirementsMatched = True
                        Exit For
                    End If
                Next

                MyTaskCounter += 1
                Dim SqlExecuted As Boolean = False
                For Each MyDataRow In ds_data.Tables("requires").Rows
                    Dim fileSQLCommands As String = ""
                    Dim fileExecuteSQLCommand As String = ""
                    If Utils.Nz(MyDataRow("product"), "") = "MSSQLServer" AndAlso SqlServerVersion.ProductName = "Microsoft SQL Server" AndAlso CType(MyDataRow("version_major"), Integer) = SqlServerVersion.VersionMajor Then
                        fileSQLCommands = CType(MyDataRow("file"), String)
                        fileExecuteSQLCommand = CType(MyDataRow("execfile"), String)
                    End If
                    If Utils.Nz(MyDataRow("product"), "") = "MSSQLServerAzure" AndAlso SqlServerVersion.ProductName = "Microsoft SQL Azure" AndAlso CType(MyDataRow("version_major"), Integer) = SqlServerVersion.VersionMajor Then
                        fileSQLCommands = CType(MyDataRow("file"), String)
                        fileExecuteSQLCommand = CType(MyDataRow("execfile"), String)
                    End If

                    'If SQL file is present, run it
                    If fileSQLCommands <> Nothing OrElse fileExecuteSQLCommand <> Nothing Then
                        SqlExecuted = True
                        If SQLCodeBuildNo <= upToBuildNo Then
                            UpdateProgressOfTasks("Applying patch " & fileSQLCommands & "...", MyTaskCounter, indexdata.Table.Rows.Count)

                            'Apply patch
                            If RequirementsMatched And fileSQLCommands <> "" Then
                                SwitchToStep("Applying patch " & fileSQLCommands & "...")

                                'Get SQL
                                Dim MySQLCmdText As String = ResourceSqlDBSetup(fileSQLCommands)
                                If MySQLCmdText = "" Then
                                    Dim MySQLCmdTextReader As System.IO.TextReader
                                    MySQLCmdTextReader = New System.IO.StreamReader(_CurWorkDir & fileSQLCommands)
                                    MySQLCmdText = MySQLCmdTextReader.ReadToEnd
                                    MySQLCmdTextReader.Close()
                                End If

                                'MySQLCmdText = Utils.ReplaceString(MySQLCmdText, "<%IGNORE_ERRORS%>", "", Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase) 'SQL Azure uses another table name than classic SQL server

                                'If SqlServerVersion.ProductName = "Microsoft SQL Azure" Then
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "[\t ]ON\s*\[PRIMARY]", " ") 'SQL Azure usually doesn't support "ON [PRIMARY]" attributes -  just remove them
                                'MySQLCmdText = Utils.ReplaceString(MySQLCmdText, "dbo.sysindexes", "sys.indexes", Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase) 'SQL Azure uses another table name than classic SQL server
                                'MySQLCmdText = Utils.ReplaceString(MySQLCmdText, "dbo.sysobjects", "sys.objects", Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase) 'SQL Azure uses another table name than classic SQL server
                                'MySQLCmdText = Utils.ReplaceString(MySQLCmdText, " id = object_id(", " object_id = object_id(", Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase) 'SQL Azure uses column name "object_id" instead of "id" (used by classic SQL server)
                                'MySQLCmdText = Utils.ReplaceString(MySQLCmdText, "OBJECTPROPERTY(id,", "OBJECTPROPERTY(object_id,", Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase) 'SQL Azure uses column name "object_id" instead of "id" (used by classic SQL server)
                                'MySQLCmdText = Utils.ReplaceString(MySQLCmdText, "WITH (HOLDLOCK TABLOCKX)", " ", Utils.ReplaceComparisonTypes.InvariantCultureIgnoreCase) 'SQL Azure uses column name "object_id" instead of "id" (used by classic SQL server)
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "(?-s:)drop index (?<schema>.*)(?:\.)(?<table>.*)(?:\.)(?<index>.*)\r\n", "DROP INDEX ${index} ON ${schema}.${table}" & vbNewLine) 'SQL Azure doesn't support the old syntax any more --> change into new syntax
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "set identity_insert (?<table>.*) ON\r\nGO\r\n", "SET IDENTITY_INSERT ${table} ON;" & vbNewLine) 'IDENTITY_INSERT statements must not be separated with a GO statements from the INSERT statement
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "GO\r\nset identity_insert (?<table>.*) OFF\r\n", ";SET IDENTITY_INSERT ${table} OFF" & vbNewLine) 'IDENTITY_INSERT statements must not be separated with a GO statements from the INSERT statement
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "(?<begin>CREATE TABLE[\w\s\.,\(\)[\]]*)IDENTITY \(1,?\s1\)(?<options>[\w\s]*),(?![\w\s\.,\-\(\)[\]]*CONSTRAINT[\w\s\.,\(\)[\]]*PRIMARY[\w\s\.,\([\]]*\))", "${begin}IDENTITY (1, 1) ${options} PRIMARY KEY,") 'SQL Azure requires IDENTITY columns always to be clustered/primary key - but just add this primary kex for CREATE TABLE statements without an immediate primary key creation
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "(?<exec>EXEC\w* sp_rename[\w\t ',\.]*\r\nGO\r\n)ALTER TABLE [\w\][\.]* (with nocheck )?ADD CONSTRAINT\s*\w* PRIMARY KEY \w*\s*\(\s*ID\s*\)[ =\w[\]]*", "${exec}") 'since above statement already creates primary keys, we don't need it here any more
                                'MySQLCmdText = Utils.ReplaceByRegExIgnoringCase(MySQLCmdText, "WITH *FILLFACTOR = \d*", "") 'SQL Azure usually doesn't support "WITH FILLFACTOR" attributes -  just remove them
                                'End If

                                'Execute SQL
                                Dim MyCurSQLCmdText As String
                                Dim MyCurSQLCmdCollection As Collection = SplitSQLCmdByGOCommand(MySQLCmdText)
                                Dim MyStepCounter As Integer = 0
                                For Each MyCurSQLCmdText In MyCurSQLCmdCollection

                                    'Status field update
                                    MyStepCounter += 1
                                    SwitchToStep("Applying patch " & fileSQLCommands & "..." & ControlChars.NewLine & _
                                      "Current step: " & MyStepCounter & " / " & MyCurSQLCmdCollection.Count)
                                    UpdateProgressOfSteps("Current step:", MyStepCounter, MyCurSQLCmdCollection.Count)

                                    Dim MyCmdExtProps As TSpecialUpdateBlockProperties = GetSpecialUpdateBlockProperties(MyCurSQLCmdText)

                                    '-------------------------------------------------
                                    'Execute SQL --- SETUP of all database objects
                                    '-------------------------------------------------
                                    Try
                                        If MyCmdExtProps.FileUpload Then
                                            'Do a file upload
                                            UploadFileIntoDBField(MyCmdExtProps.CommandText, DBConn, MyCmdExtProps.FileUpload_Path)
                                        Else
                                            'simply execute the given sql command
                                            DBCmd = New SqlClient.SqlCommand
                                            DBCmd.Connection = DBConn
                                            If SqlServerVersion.ProductName = "Microsoft SQL Azure" Then
                                                MyCmdExtProps.CommandText = MyCmdExtProps.CommandText.Replace("WITH ENCRYPTION,", "WITH ")
                                                MyCmdExtProps.CommandText = MyCmdExtProps.CommandText.Replace("WITH ENCRYPTION", "")
                                            Else
#If DEBUG Then
                                                'Do not encrypt the stored procedures in debug mode
                                                MyCmdExtProps.CommandText = MyCmdExtProps.CommandText.Replace("WITH ENCRYPTION,", "WITH ")
                                                MyCmdExtProps.CommandText = MyCmdExtProps.CommandText.Replace("WITH ENCRYPTION", "")
#End If
                                            End If
                                            DBCmd.CommandText = MyCmdExtProps.CommandText
                                            DBCmd.CommandType = CommandType.Text
                                            DBCmd.CommandTimeout = 300 '5 minutes
                                            DBCmd.ExecuteNonQuery()
                                            DBCmd.Dispose()
                                        End If
                                    Catch e As SqlException
                                        Dim errorMessages As String = Nothing
                                        Dim i As Integer

                                        If MyCmdExtProps.IgnoreSQLErrors = False Then
                                            For i = 0 To e.Errors.Count - 1
                                                errorMessages += "ErrorIndex #" & i.ToString() & ControlChars.NewLine _
                                                 & "Message: " & e.Errors(i).Message & ControlChars.NewLine _
                                                 & "LineNumber: " & e.Errors(i).LineNumber & ControlChars.NewLine _
                                                 & "Source: " & e.Errors(i).Source & ControlChars.NewLine _
                                                 & "Procedure: " & e.Errors(i).Procedure & ControlChars.NewLine _
                                                 & "Current step: " & MyStepCounter & " / " & MyCurSQLCmdCollection.Count & ControlChars.NewLine _
                                                 & "Source file: " & fileSQLCommands & ControlChars.NewLine _
                                                 & CType(IIf(DebugLevel >= 3, "SQL BEGIN ==>" & ControlChars.NewLine & MyCurSQLCmdText & ControlChars.NewLine & ControlChars.NewLine & "<== SQL END" & ControlChars.NewLine, ""), String) _
                                                 & ControlChars.NewLine
                                            Next i
                                            WriteToLog("==============================================" & ControlChars.NewLine & _
                                             Now() & ControlChars.NewLine & _
                                             "==============================================" & ControlChars.NewLine & _
                                             ControlChars.NewLine & _
                                             errorMessages)
                                            If SqlServerVersion.ProductName = "" Then
                                                WriteToLog(vbNewLine & vbNewLine & "==============================================" & vbNewLine & "FULL SQL AZURE STATEMENTS" & "========================================" & vbNewLine & MySQLCmdText & vbNewLine & vbNewLine)
                                            End If
                                            If _LogFileEnabled Then
                                                RaiseWarning("Error #79c found - please check the log file " & _LogFile & "!")
                                            Else
                                                RaiseWarning("Error #79c found!")
                                            End If

                                            If DBConn.State <> ConnectionState.Closed Then
                                                DBConn.Close()
                                            End If
                                            Throw New DatabasePatchCommandException("Warnings/Errors have been logged", New Tools.Data.DataQuery.AnyIDataProvider.DataException(DBCmd, e), errorMessages)
                                            Exit Sub
                                        End If
                                    Finally
                                        If Not DBCmd Is Nothing Then DBCmd.Dispose()
                                    End Try

                                Next

                                'Execute additional file
                                If fileExecuteSQLCommand <> "" Then
                                    Dim MySQLExecCmdText As String = ResourceSqlDBSetup(fileExecuteSQLCommand)
                                    If MySQLExecCmdText = "" Then
                                        Try
                                            Dim MySQLExecCmdTextReader As System.IO.TextReader
                                            MySQLExecCmdTextReader = New System.IO.StreamReader(_CurWorkDir & fileExecuteSQLCommand)
                                            MySQLExecCmdText = MySQLExecCmdTextReader.ReadToEnd
                                            MySQLExecCmdTextReader.Close()
                                        Catch e As SqlException
                                            Dim errorMessages As String = Nothing
                                            Dim i As Integer

                                            For i = 0 To e.Errors.Count - 1
                                                errorMessages += "Message: " & e.GetBaseException.ToString _
                                                 & ControlChars.NewLine
                                            Next i
                                            WriteToLog("==============================================" & ControlChars.NewLine & _
                                             Now() & ControlChars.NewLine & _
                                             "==============================================" & ControlChars.NewLine & _
                                             ControlChars.NewLine & _
                                             errorMessages)
                                            If Me._LogFileEnabled Then
                                                RaiseWarning("Error #68 found - please check the log file " & _LogFile & "!")
                                            Else
                                                RaiseWarning("Error #68 found!")
                                            End If
                                            If DBConn.State <> ConnectionState.Closed Then
                                                DBConn.Close()
                                            End If
                                            Throw New Exception("Warnings/Errors have been logged")
                                            Exit Sub
                                        Finally
                                            If Not DBCmd Is Nothing Then DBCmd.Dispose()
                                        End Try
                                    End If
                                    If SqlServerVersion.ProductName = "Microsoft SQL Azure" Then
                                        'SQL Azure usually doesn't support "ON [PRIMARY]" attributes -  just remove them
                                        MySQLExecCmdText = Replace(MySQLExecCmdText, "ON [PRIMARY]", "")
                                    End If
                                    'Replace placeholders
                                    If Not ReplacementsInInitSQL Is Nothing Then
                                        For Each MyKey As String In ReplacementsInInitSQL
                                            MySQLExecCmdText = MySQLExecCmdText.Replace(MyKey, ReplacementsInInitSQL(MyKey))
                                        Next
                                    End If

                                    'Execute commands
                                    Dim MyCurSQLExecCmdCollection As Collection = SplitSQLCmdByGOCommand(MySQLExecCmdText)
                                    Dim MyExecStepCounter As Integer = 0
                                    For Each MySQLExecCmdText In MyCurSQLExecCmdCollection
                                        'Status field update
                                        MyExecStepCounter += 1
                                        SwitchToStep("Configure database..." & ControlChars.NewLine & _
                                          "Current step: " & MyExecStepCounter & " / " & MyCurSQLExecCmdCollection.Count)
                                        UpdateProgressOfSteps("Configure database...", MyExecStepCounter, MyCurSQLExecCmdCollection.Count)

                                        'Execute SQL
                                        Try
                                            DBCmd = New SqlClient.SqlCommand
                                            DBCmd.Connection = DBConn
                                            DBCmd.CommandText = MySQLExecCmdText
                                            DBCmd.CommandType = CommandType.Text
                                            DBCmd.CommandTimeout = 300 '5 minutes
                                            DBCmd.ExecuteNonQuery()
                                            DBCmd.Dispose()
                                        Catch e As SqlException
                                            Dim errorMessages As String = Nothing
                                            Dim i As Integer

                                            For i = 0 To e.Errors.Count - 1
                                                errorMessages += "ErrorIndex #" & i.ToString() & ControlChars.NewLine _
                                                 & "Message: " & e.Errors(i).Message & ControlChars.NewLine _
                                                 & "LineNumber: " & e.Errors(i).LineNumber & ControlChars.NewLine _
                                                 & "Source: " & e.Errors(i).Source & ControlChars.NewLine _
                                                 & "Procedure: " & e.Errors(i).Procedure & ControlChars.NewLine _
                                                 & "Current step: " & MyStepCounter & " / " & MyCurSQLCmdCollection.Count _
                                                 & CType(IIf(DebugLevel >= 3, "SQL BEGIN ==>" & ControlChars.NewLine & MySQLExecCmdText & ControlChars.NewLine & ControlChars.NewLine & "<== SQL END" & ControlChars.NewLine, ""), String) _
                                                 & ControlChars.NewLine
                                            Next i
                                            '"Current task: " & Me.ProgressOfTasks.CurrentStepTitle & vbNewLine & _
                                            '"Current sub task: " & Me.ProgressOfSteps.CurrentStepTitle & vbNewLine & _
                                            '"Current step description: " & Me.CurrentStepTitle & vbNewLine & _
                                            WriteToLog("==============================================" & ControlChars.NewLine & _
                                             Now() & ControlChars.NewLine & _
                                             "==============================================" & ControlChars.NewLine & _
                                             ControlChars.NewLine & _
                                             errorMessages)
                                            If Me._LogFileEnabled Then
                                                RaiseWarning("Error #69 found - please check the log file " & _LogFile & "!")
                                            Else
                                                RaiseWarning("Error #69 found!")
                                            End If
                                            If DBConn.State <> ConnectionState.Closed Then
                                                DBConn.Close()
                                            End If
                                            Throw New Exception("Warnings/Errors have been logged")
                                            Exit Sub
                                        Finally
                                            If Not DBCmd Is Nothing Then DBCmd.Dispose()
                                        End Try
                                    Next
                                End If


                                'Update DB build status
                                Dim FinishDBUpdateStepSQLCmdText As String
                                FinishDBUpdateStepSQLCmdText = "-----------------------------------------------------------" & vbNewLine & _
                                 "-- Update table content to match new definition " & vbNewLine & _
                                 "-- (to allow multiple camm products in one database)" & vbNewLine & _
                                 "-----------------------------------------------------------" & vbNewLine & _
                                 "UPDATE [dbo].[System_GlobalProperties]" & vbNewLine & _
                                 "SET [ValueNVarChar] = (SELECT TOP 1 ValueNVarChar FROM [dbo].[System_GlobalProperties] WHERE PropertyName = N'DBProductName')" & vbNewLine & _
                                 "WHERE ID in (1, 2, 3) AND ValueNVarChar Is Null And PropertyName like N'DBVersion_%'" & vbNewLine & _
                                 vbNewLine & _
                                 "-----------------------------------------------------------" & vbNewLine & _
                                 "-- Update data in dbo.System_GlobalProperties" & vbNewLine & _
                                 "-----------------------------------------------------------" & vbNewLine & _
                                 "UPDATE [dbo].[System_GlobalProperties] SET [ValueInt] = " & SQLCodeMajorVer & " WHERE PropertyName = N'DBVersion_Major' AND ValueNVarChar = N'camm " & _SetupPackageName.Replace("'", "''") & "'" & vbNewLine & _
                                 "UPDATE [dbo].[System_GlobalProperties] SET [ValueInt] = " & SQLCodeMinorVer & " WHERE PropertyName = N'DBVersion_Minor' AND ValueNVarChar = N'camm " & _SetupPackageName.Replace("'", "''") & "'" & vbNewLine & _
                                 "UPDATE [dbo].[System_GlobalProperties] SET [ValueInt] = " & SQLCodeBuildNo & " WHERE PropertyName = N'DBVersion_Build' AND ValueNVarChar = N'camm " & _SetupPackageName.Replace("'", "''") & "'" & vbNewLine
                                Try
                                    DBCmd = New SqlClient.SqlCommand
                                    DBCmd.Connection = DBConn
                                    DBCmd.CommandText = FinishDBUpdateStepSQLCmdText
                                    DBCmd.CommandType = CommandType.Text
                                    DBCmd.CommandTimeout = 300 '5 minutes
                                    DBCmd.ExecuteNonQuery()
                                    DBCmd.Dispose()
                                Catch e As SqlException
                                    Dim errorMessages As String = Nothing
                                    Dim i As Integer

                                    For i = 0 To e.Errors.Count - 1
                                        errorMessages += "ErrorIndex #" & i.ToString() & ControlChars.NewLine _
                                         & "Message: " & e.Errors(i).Message & ControlChars.NewLine _
                                         & "LineNumber: " & e.Errors(i).LineNumber & ControlChars.NewLine _
                                         & "Source: " & e.Errors(i).Source & ControlChars.NewLine _
                                         & "Procedure: " & e.Errors(i).Procedure & ControlChars.NewLine _
                                         & "Current step: " & MyStepCounter & " / " & MyCurSQLCmdCollection.Count _
                                         & "Source file: " & fileSQLCommands _
                                         & CType(IIf(DebugLevel >= 3, "SQL BEGIN ==>" & ControlChars.NewLine & FinishDBUpdateStepSQLCmdText & ControlChars.NewLine & ControlChars.NewLine & "<== SQL END" & ControlChars.NewLine, ""), String) _
                                         & ControlChars.NewLine
                                    Next i
                                    WriteToLog("==============================================" & ControlChars.NewLine & _
                                     Now() & ControlChars.NewLine & _
                                     "==============================================" & ControlChars.NewLine & _
                                     ControlChars.NewLine & _
                                     errorMessages)
                                    If Me._LogFileEnabled Then
                                        RaiseWarning("Error #67 found - please check the log file " & _LogFile & "!")
                                    Else
                                        RaiseWarning("Error #67 found!")
                                    End If
                                    If DBConn.State <> ConnectionState.Closed Then
                                        DBConn.Close()
                                    End If
                                    Throw New Exception("Warnings/Errors have been logged")
                                    Exit Sub
                                Finally
                                    If Not DBCmd Is Nothing Then DBCmd.Dispose()
                                End Try

                                ListOfAppliedPatches += CType(MyRow("file"), String) & ControlChars.NewLine
                            ElseIf fileSQLCommands = "" Then
                                RaiseWarning("Error #97 found - patch file """ & _CurWorkDir & CType(MyRow("file"), String) & """ corrupt or with missing data!")
                                If DBConn.State <> ConnectionState.Closed Then
                                    DBConn.Close()
                                End If
                                Throw New Exception("Warnings/Errors have been logged")
                                Exit Sub
                            Else
                                'Requirement don't match because the current database is newer than this update script
                            End If
                        Else
                            UpdateProgressOfTasks("Ignore patch " & fileSQLCommands & "...", MyTaskCounter, indexdata.Table.Rows.Count)
                        End If
                    End If


                Next

                'No SQL has been run - this might happen if no patch files were found, typically if a new sql server version is available, but XMLs haven't been updated yet to support the new sql server version
                If SqlExecuted = False Then
                    Me.WriteToLog("Missing support for current database server """ & SqlServerVersion.ProductName & """ V" & SqlServerVersion.VersionMajor.ToString() & " at patch file for CWM DB build " & SQLCodeBuildNo.ToString())
                    Throw New Exception("Missing support for current SQL server """ & SqlServerVersion.ProductName & """ V" & SqlServerVersion.VersionMajor.ToString() & " at patch file for CWM DB build " & SQLCodeBuildNo.ToString())
                End If

            Next
            WriteToLog("<p>==============================================<br>")
            WriteToLog(Now() & "<br>")
            WriteToLog("==============================================<br><br>")
            WriteToLog("Applied patches:<br>")
            If ListOfAppliedPatches = "" Then
                WriteToLog("There are no further updates for this database." & vbNewLine)
            Else
                WriteToLog(ListOfAppliedPatches.Replace(vbNewLine, "<br>"))
            End If

            WriteToLog("</p><p>Current database build no. is " & GetCurrentDBBuildNo(DBConn, DebugLevel).ToString() & "</p>")

            DBConn.Dispose()

        End Sub

        ''' <summary>
        ''' An exception that represents an error on executing a single SQL command on the database
        ''' </summary>
        Public Class DatabasePatchCommandException
            Inherits Exception

            Public Sub New(message As String, innerException As Exception, additionalErrorDetails As String)
                MyBase.New(message, innerException)
                Me._AdditionalErrorDetails = additionalErrorDetails
            End Sub

            Private _AdditionalErrorDetails As String
            Public ReadOnly Property AdditionalErrorDetails As String
                Get
                    Return _AdditionalErrorDetails
                End Get
            End Property

        End Class

#End Region

#Region "Internal functions"

        Private Function SplitSQLCmdByGOCommand(ByVal SQLComplete As String) As Collection
            Dim MyCmdCollection As New Collection
            Dim MyCurPos As Integer
            Dim MyCurBlockBegin As Integer = 0

            For MyCurPos = 0 To SQLComplete.Length
                Dim MyCurPosLineBreakCr As Integer
                Dim MyCurPosLineBreakLf As Integer
                Dim MyCurPosLineBreakCrLf As Integer
                Dim LineBegin As Integer
                Dim LineEnd As Integer
                Dim LineContent As String

                'Reposition cursor after line break
                MyCurPosLineBreakCr = SQLComplete.IndexOf(ControlChars.Cr, MyCurPos)
                MyCurPosLineBreakLf = SQLComplete.IndexOf(ControlChars.Lf, MyCurPos)
                MyCurPosLineBreakCrLf = SQLComplete.IndexOf(ControlChars.CrLf, MyCurPos)
                If MyCurPosLineBreakCr = -1 Then MyCurPosLineBreakCr = SQLComplete.Length
                If MyCurPosLineBreakLf = -1 Then MyCurPosLineBreakLf = SQLComplete.Length
                If MyCurPosLineBreakCrLf = -1 Then MyCurPosLineBreakCrLf = SQLComplete.Length
                LineBegin = MinValue(MyCurPosLineBreakCr, MinValue(MyCurPosLineBreakLf, MyCurPosLineBreakCrLf))
                If Mid(SQLComplete, LineBegin + 1, 2) = ControlChars.CrLf Then
                    LineBegin += 2
                Else
                    LineBegin += 1
                End If
                LineBegin = MinValue(LineBegin, SQLComplete.Length)

                'Find end of line
                MyCurPosLineBreakCr = SQLComplete.IndexOf(ControlChars.Cr, LineBegin)
                MyCurPosLineBreakLf = SQLComplete.IndexOf(ControlChars.Lf, LineBegin)
                MyCurPosLineBreakCrLf = SQLComplete.IndexOf(ControlChars.CrLf, LineBegin)
                If MyCurPosLineBreakCr = -1 Then MyCurPosLineBreakCr = SQLComplete.Length
                If MyCurPosLineBreakLf = -1 Then MyCurPosLineBreakLf = SQLComplete.Length
                If MyCurPosLineBreakCrLf = -1 Then MyCurPosLineBreakCrLf = SQLComplete.Length
                LineEnd = MinValue(MyCurPosLineBreakCr, MinValue(MyCurPosLineBreakLf, MyCurPosLineBreakCrLf))

                'Add array element
                LineContent = Mid(SQLComplete, LineBegin + 1, LineEnd - LineBegin).Trim(" "c)
                If LineContent.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "go" Or LineBegin >= SQLComplete.Length Then
                    MyCmdCollection.Add(Mid(SQLComplete, MyCurBlockBegin + 1, (LineBegin) - (MyCurBlockBegin)))
                    MyCurBlockBegin = LineEnd
                End If
                MyCurPos = LineEnd
            Next

            Return MyCmdCollection

        End Function

        Private Structure TSpecialUpdateBlockProperties
            Dim IgnoreSQLErrors As Boolean
            Dim FileUpload As Boolean
            Dim FileUpload_Path As String
            Dim CommandText As String
        End Structure

        Private Function GetSpecialUpdateBlockProperties(ByVal SQLCommandBlock As String) As TSpecialUpdateBlockProperties

            Dim Result As New TSpecialUpdateBlockProperties

            'Standard values
            Result.IgnoreSQLErrors = False

            'analyse and modify command text and extended command block properties
            If InStr(SQLCommandBlock, "<%IGNORE_ERRORS%>") > 0 Then
                SQLCommandBlock = SQLCommandBlock.Replace("<%IGNORE_ERRORS%>", "")
                Result.IgnoreSQLErrors = True
            End If
            If InStr(SQLCommandBlock, "<%FILE_UPLOAD%>") > 0 AndAlso InStr(SQLCommandBlock, "<%/FILE_UPLOAD%>") > InStr(SQLCommandBlock, "<%FILE_UPLOAD%>") + "<%FILE_UPLOAD%>".Length Then
                Result.FileUpload_Path = SQLCommandBlock.Substring(InStr(SQLCommandBlock, "<%FILE_UPLOAD%>") + "<%FILE_UPLOAD%>".Length - 1, InStr(SQLCommandBlock, "<%/FILE_UPLOAD%>") - (InStr(SQLCommandBlock, "<%FILE_UPLOAD%>") + "<%FILE_UPLOAD%>".Length))
                SQLCommandBlock = SQLCommandBlock.Remove(InStr(SQLCommandBlock, "<%FILE_UPLOAD%>") - 1, InStr(SQLCommandBlock, "<%/FILE_UPLOAD%>") + "<%/FILE_UPLOAD%>".Length - InStr(SQLCommandBlock, "<%FILE_UPLOAD%>"))
                Result.FileUpload = True
            End If

            'return updated sql command string
            Result.CommandText = SQLCommandBlock
            Return Result

        End Function

        Private Function MinValue(ByVal Value1 As Integer, ByVal Value2 As Integer) As Integer
            Return CType(IIf(Value1 < Value2, Value1, Value2), Integer)
        End Function

        ''' <summary>
        ''' camm Web-Manager database build no.
        ''' </summary>
        ''' <param name="ConnectionString"></param>
        ''' <remarks></remarks>
        Public Function GetCurrentDBBuildNo(ByVal ConnectionString As String) As Integer
            Dim Result As Integer
            Dim MySqlConn As New SqlConnection(ConnectionString)
            Try
                MySqlConn.Open()
                Result = GetCurrentDBBuildNo(MySqlConn, DebugLevel)
            Finally
                MySqlConn.Close()
                MySqlConn.Dispose()
            End Try
            Return Result
        End Function

        ''' <summary>
        ''' camm Web-Manager database build no.
        ''' </summary>
        ''' <param name="DBConnection"></param>
        ''' <param name="DebugLevel"></param>
        ''' <remarks></remarks>
        Private Function GetCurrentDBBuildNo(ByVal DBConnection As SqlClient.SqlConnection, ByVal DebugLevel As Integer) As Integer
            Dim sqlGetDBVersion As String = "declare @dbversion_major int" & vbNewLine & _
             "declare @dbversion_minor int" & vbNewLine & _
             "declare @dbversion_build int" & vbNewLine & _
             "declare @dbproductname nvarchar(256)" & vbNewLine & _
             vbNewLine & _
             "SET NOCOUNT ON" & vbNewLine & _
             vbNewLine & _
             "select @dbversion_major = valueint from system_globalproperties where propertyname = N'DBVersion_Major' and valuenvarchar = N'camm " & _SetupPackageName.Replace("'", "''") & "'" & vbNewLine & _
             "if @dbversion_major is null " & vbNewLine & _
             "   select @dbversion_major = valueint from system_globalproperties where propertyname = N'DBVersion_Major'" & _
             vbNewLine & _
             "select @dbversion_minor = valueint from system_globalproperties where propertyname = N'DBVersion_Minor' and valuenvarchar = N'camm " & _SetupPackageName.Replace("'", "''") & "'" & vbNewLine & _
             "if @dbversion_minor is null " & vbNewLine & _
             "   select @dbversion_minor = valueint from system_globalproperties where propertyname = N'DBVersion_Minor'" & _
             vbNewLine & _
             "select @dbversion_build = valueint from system_globalproperties where propertyname = N'DBVersion_Build' and valuenvarchar = N'camm " & _SetupPackageName.Replace("'", "''") & "'" & vbNewLine & _
             "if @dbversion_build is null " & vbNewLine & _
             "   select @dbversion_build = valueint from system_globalproperties where propertyname = N'DBVersion_Build'" & _
             vbNewLine & _
             "select @dbversion_build = valueint from system_globalproperties where propertyname = N'DBVersion_Build'" & vbNewLine & _
             vbNewLine & _
             "select @dbproductname = valuenvarchar from system_globalproperties where propertyname = N'DBProductName'" & vbNewLine & _
             vbNewLine & _
             "SET NOCOUNT OFF" & vbNewLine & _
             vbNewLine & _
             "SELECT DBProductName = @dbproductname, " & vbNewLine & _
             "   DBVersion_Major = @dbversion_major, " & vbNewLine & _
             "	DBVersion_Minor = @dbversion_minor, " & vbNewLine & _
             "	DBVersion_Build = @dbversion_build"
            Dim DBCmd As SqlCommand = Nothing
            Dim MyRecSet As SqlClient.SqlDataReader = Nothing
            Dim DBBuildNo As Object

            'Current DBBuildNo
            Try
                DBCmd = New SqlClient.SqlCommand
                DBCmd.Connection = DBConnection
                DBCmd.CommandText = sqlGetDBVersion
                DBCmd.CommandType = CommandType.Text
                DBCmd.CommandTimeout = 300 '5 minutes
                MyRecSet = DBCmd.ExecuteReader
                MyRecSet.Read()
                DBBuildNo = MyRecSet("DBVersion_Build")
                If Utils.Nz(MyRecSet("DBProductName"), "") = "camm " & _SetupPackageName Then
                Else
                    RaiseWarning("Error #66 found - wrong database selected in connection string. You wanted to install ""camm " & _SetupPackageName & """ into a database of """ & Utils.Nz(MyRecSet("DBProductName"), "") & """")
                End If
                DBCmd.Dispose()
                MyRecSet.Close()
            Catch e As SqlException
                Dim errorMessages As String = Nothing
                Dim i As Integer

                For i = 0 To e.Errors.Count - 1
                    errorMessages += "ErrorIndex #" & i.ToString() & ControlChars.NewLine _
                       & "Message: " & e.Errors(i).Message & ControlChars.NewLine _
                       & "LineNumber: " & e.Errors(i).LineNumber & ControlChars.NewLine _
                       & "Source: " & e.Errors(i).Source & ControlChars.NewLine _
                       & "Procedure: " & e.Errors(i).Procedure & ControlChars.NewLine _
                       & CType(IIf(DebugLevel >= 3, "SQL BEGIN ==>" & ControlChars.NewLine & sqlGetDBVersion & ControlChars.NewLine & ControlChars.NewLine & "<== SQL END" & ControlChars.NewLine, ""), String) _
                       & ControlChars.NewLine
                Next i
                WriteToLog("==============================================" & ControlChars.NewLine & _
                 Now() & ControlChars.NewLine & _
                 "==============================================" & ControlChars.NewLine & _
                 ControlChars.NewLine & _
                 errorMessages)
                Throw New Exception("Error #34 found - please check the log file " & _LogFile & "!")
            Finally
                If Not DBCmd Is Nothing Then DBCmd.Dispose()
                If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then MyRecSet.Close()
            End Try

            Return CType(DBBuildNo, Integer)

        End Function

        Private Function GetSQLFormatting_StringValue(ByVal TextValue As String, Optional ByVal KeepEmptyStrings As Boolean = False) As String
            If TextValue = "" Then
                If KeepEmptyStrings Then
                    Return ("''")
                Else
                    Return ("NULL")
                End If
            Else
                Return ("N'" & TextValue.Replace("'", "''") & "'")
            End If
        End Function

        Public Structure DBServerVersion
            Dim ProductName As String
            Dim ProcessorCount As String
            Dim PhysicalMemory As String
            Dim WindowsVersion As String
            Dim Platform As String
            Dim VersionMajor As Long
            Dim VersionMinor As Long
            Dim VersionBuild As Long
            Dim ErrorsFound As Boolean
            Dim Exception As Exception
        End Structure

        ''' <summary>
        ''' Evaluate the version of the connected sql server instance
        ''' </summary>
        ''' <param name="DBConnectionString"></param>
        ''' <remarks></remarks>
        Public Function GetSQLServerVersion(ByVal DBConnectionString As String) As DBServerVersion
            Dim DBConn As New SqlConnection(DBConnectionString & ";DATABASE=;")
            Dim DBCmd As SqlCommand
            Dim MyRecSet As SqlClient.SqlDataReader
            Dim buf As String
            Dim result As New DBServerVersion

            Try
                DBConn.Open()
                DBCmd = New SqlClient.SqlCommand
                DBCmd.Connection = DBConn
                DBCmd.CommandText = "master.dbo.xp_msver"
                DBCmd.CommandType = CommandType.StoredProcedure
                DBCmd.CommandTimeout = 300 '5 minutes
                MyRecSet = DBCmd.ExecuteReader

                While MyRecSet.Read
                    If Strings.LCase(Utils.Nz(MyRecSet("Name"), "")) = "productname" Then result.ProductName = Trim(Utils.Nz(MyRecSet("Character_Value"), ""))
                    If Strings.LCase(Utils.Nz(MyRecSet("Name"), "")) = "processorcount" Then result.ProcessorCount = Trim(Utils.Nz(MyRecSet("Character_Value"), ""))
                    If Strings.LCase(Utils.Nz(MyRecSet("Name"), "")) = "physicalmemory" Then result.PhysicalMemory = Trim(Utils.Nz(MyRecSet("Character_Value"), ""))
                    If Strings.LCase(Utils.Nz(MyRecSet("Name"), "")) = "windowsnersion" Then result.WindowsVersion = Trim(Utils.Nz(MyRecSet("Character_Value"), ""))
                    If Strings.LCase(Utils.Nz(MyRecSet("Name"), "")) = "platform" Then result.Platform = Trim(Utils.Nz(MyRecSet("Character_Value"), ""))
                    If Strings.LCase(Utils.Nz(MyRecSet("Name"), "")) = "productversion" Then
                        buf = Trim(Utils.Nz(MyRecSet("Character_Value"), ""))
                        result.VersionMajor = CLng(Mid$(buf, 1, InStr(buf, ".") - 1))
                        buf = Mid$(buf, InStr(buf, ".") + 1)
                        result.VersionMinor = CLng(Mid$(buf, 1, InStr(buf, ".") - 1))
                        buf = Mid$(buf, InStr(buf, ".") + 1)
                        result.VersionBuild = CLng(buf)
                    End If
                End While

                MyRecSet.Close()
                DBCmd.Dispose()

            Catch
                Try
                    'Statement above fails on MS SQL Azure - try to identify the version manually
                    'Microsoft SQL Azure (RTM) - 10.25.9501.0 	Nov  3 2010 13:04:51 	Copyright (c) Microsoft Corporation
                    'Alternative values might be:
                    'Microsoft SQL Server  2000 - 8.00.760 (Intel X86) Dec 17 2002 14:22:05 Copyright (c) 1988-2003 Microsoft Corporation Desktop Engine on Windows NT 5.1 (Build 2600: Service Pack 3)
                    Dim sp As String = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(DBConn, "SELECT @@Version", CommandType.Text, Nothing, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), "")
                    Dim matches As System.Text.RegularExpressions.Match
                    matches = System.Text.RegularExpressions.Regex.Match(sp, "(?<ServerProduct>.*) - (?<MajorVersion>\d*)\.(?<MinorVersion>\d*)\.(?<Build>\d*).*", System.Text.RegularExpressions.RegexOptions.CultureInvariant Or System.Text.RegularExpressions.RegexOptions.Singleline)
                    Console.WriteLine(matches.Captures(0).Value)
                    If matches.Groups("ServerProduct").Value.StartsWith("Microsoft SQL Server") Then
                        result.ProductName = "Microsoft SQL Server"
                    ElseIf matches.Groups("ServerProduct").Value.StartsWith("Microsoft SQL Azure") Then
                        result.ProductName = "Microsoft SQL Azure"
                    Else
                        result.ProductName = matches.Groups("ServerProduct").Value
                    End If
                    result.VersionMajor = Integer.Parse(matches.Groups("MajorVersion").Value)
                    result.VersionMinor = Integer.Parse(matches.Groups("MinorVersion").Value)
                    result.VersionBuild = Integer.Parse(matches.Groups("Build").Value)
                Catch ex As Exception
                    result.Exception = ex
                    result.ErrorsFound = True
                End Try
            Finally
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(DBConn)
            End Try

            Return result

        End Function

        Private Sub UploadFileIntoDBField(ByVal sqlSELECT As String, ByVal DBConnection As SqlClient.SqlConnection, ByVal File As String)

            Const myDataTableName As String = "data"
            Dim intImageSize As Integer
            Dim ImageStream As System.IO.Stream

            ' Gets the Size of the Image
            Dim ImageContent() As Byte
            Dim BinaryData As Byte() = ResourceSqlDBSetupBinary(File)
            If BinaryData Is Nothing Then
                ImageContent = BinaryData
            Else
                ImageStream = New System.IO.FileStream(File, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                intImageSize = CType(ImageStream.Length, Integer)
                ReDim ImageContent(intImageSize)
                Dim intStatus As Integer
                intStatus = ImageStream.Read(ImageContent, 0, intImageSize)
                ImageStream.Close()
            End If

            'Open database field, update and write back
            Dim myDataAdapter As New SqlDataAdapter
            'Dim DBConnection As New Data.SqlClient.SqlConnection(DBConnectionString)
            myDataAdapter.SelectCommand = New SqlCommand(sqlSELECT, DBConnection)
            Dim cb As SqlCommandBuilder = New SqlCommandBuilder(myDataAdapter)
            myDataAdapter.UpdateCommand.CommandTimeout = 300 '5 minutes
            'DBConnection.Open()
            Dim ds As DataSet = New DataSet
            myDataAdapter.Fill(ds, myDataTableName)

            ' Code to modify data in DataSet here 
            Dim myDataTable As DataTable = ds.Tables(myDataTableName)
            myDataTable.Rows(0)(myDataTable.Columns.Count - 1) = ImageContent

            myDataAdapter.Update(ds, myDataTableName)
            'DBConnection.Close()
            'DBConnection.Dispose()
            myDataAdapter.Dispose()
            cb.Dispose()
            myDataTable.Dispose()
            ds.Dispose()

        End Sub
#End Region

    End Class

End Namespace