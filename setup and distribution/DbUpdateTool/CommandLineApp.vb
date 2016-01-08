Option Explicit On
Option Strict On

Imports CompuMaster.camm.WebManager

Friend Module CommandLineApp

    Sub TestConnection(options As CommandlineOptions)
        Dim MyDBVersion As Setup.DatabaseSetup.DBServerVersion
        If options.ConnectionStringForSqlDatabaseCreation <> "" Then
            MyDBVersion = DBSetup.GetSQLServerVersion(options.ConnectionStringForSqlDatabaseCreation)
        Else
            MyDBVersion = DBSetup.GetSQLServerVersion(options.ConnectionString)
        End If

        ' Test connection for MS SQL Servers
        If MyDBVersion.ErrorsFound = True Then
            If MyDBVersion.Exception.InnerException IsNot Nothing AndAlso MyDBVersion.Exception.Message = "Data layer exception" Then
                'show inner exception for CM DataLayer Exception
                QuitWithError("CRITICAL ERROR: Error connecting to database server: " & MyDBVersion.Exception.InnerException.Message)
            Else
                QuitWithError("CRITICAL ERROR: Error connecting to database server: " & MyDBVersion.Exception.Message)
            End If
        ElseIf options.DatabaseServerType = CommandlineOptions.DatabaseServerTypes.MsSqlServer AndAlso MyDBVersion.ProductName = "Microsoft SQL Server" AndAlso MyDBVersion.VersionMajor >= 9 Then 'if user selected SQL (classic) db, it requires SQL Server 2005 or higher
            'continue
        ElseIf options.DatabaseServerType = CommandlineOptions.DatabaseServerTypes.MsAzureSql AndAlso MyDBVersion.ProductName = "Microsoft SQL Azure" AndAlso MyDBVersion.VersionMajor >= 12 Then 'if user selected SQL Azure db, it requires SQL Azure Server (V12) or higher
            'continue
        Else
            QuitWithError("Connection succesful, but wrong type of database server (e.g. Microsoft SQL Azure expected, but connected to classical Microsoft SQL Server)")
        End If

        If options.ConnectionStringForSqlDatabaseCreation <> "" Then
            Select Case options.DatabaseServerType
                Case CommandlineOptions.DatabaseServerTypes.MsSqlServer
                    'continue
                Case CommandlineOptions.DatabaseServerTypes.MsAzureSql
                    QuitWithError("ERROR: SQL database creation not supported on Azure")
                Case Else
                    QuitWithError("ERROR: Unknown/invalid SQL server type for creating SQL database")
            End Select
        End If

    End Sub

    Sub CreateSqlDatabaseAndCwmInstance(options As CommandlineOptions)
        If options.FirstServerGroupTitle = "" Then
            QuitWithError("MISSING ARGUMENT: FirstServerGroupTitle")
        ElseIf options.FirstServerGroupTitleInNav = "" Then
            QuitWithError("MISSING ARGUMENT: FirstServerGroupTitleInNav")
        ElseIf options.StandardEMailContact = "" Then
            QuitWithError("MISSING ARGUMENT: StandardEMailContact")
        ElseIf options.GeneralWebsiteUrl = "" Then
            QuitWithError("MISSING ARGUMENT: GeneralWebsiteUrl")
        ElseIf options.CompanyName = "" Then
            QuitWithError("MISSING ARGUMENT: CompanyName")
        ElseIf options.FormerCompanyName = "" Then
            QuitWithError("MISSING ARGUMENT: FormerCompanyName")
        ElseIf DBSetup.CreateDatabase(options.ConnectionString, options.ConnectionStringForSqlDatabaseCreation, options.DatabaseNameForSqlDatabaseCreation, False) = False Then
            QuitWithError("CRITICAL ERROR OCCURED, PROCESS ABORTED.")
        Else
            Console.WriteLine("SQL DATABASE CREATED SUCCESSFULLY.")
        End If
    End Sub

    Sub CleanupSqlDatabaseAndCreateWebManagerInstance(options As CommandlineOptions)
        If options.FirstServerGroupTitle = "" Then
            QuitWithError("MISSING ARGUMENT: FirstServerGroupTitle")
        ElseIf options.FirstServerGroupTitleInNav = "" Then
            QuitWithError("MISSING ARGUMENT: FirstServerGroupTitleInNav")
        ElseIf options.StandardEMailContact = "" Then
            QuitWithError("MISSING ARGUMENT: StandardEMailContact")
        ElseIf options.GeneralWebsiteUrl = "" Then
            QuitWithError("MISSING ARGUMENT: GeneralWebsiteUrl")
        ElseIf options.CompanyName = "" Then
            QuitWithError("MISSING ARGUMENT: CompanyName")
        ElseIf options.FormerCompanyName = "" Then
            QuitWithError("MISSING ARGUMENT: FormerCompanyName")
        Else
            'Cleanup everything, first
            If DBSetup.ResetOldDatabase(options.ConnectionString) = False Then
                QuitWithError("CRITICAL ERROR OCCURED, PROCESS ABORTED.")
            End If
            '(Re-)create basic instance
            Try
                DBSetup.InitDatabase(options.ConnectionString)
                UpdateWebManagerInstance(options)
            Catch ex As Exception
                QuitWithError("CRITICAL ERROR: " & ex.ToString)
            End Try
        End If
    End Sub

    Sub UpdateWebManagerInstance(options As CommandlineOptions)
        Dim DatabaseServerType As String
        Select Case options.DatabaseServerType
            Case CommandlineOptions.DatabaseServerTypes.MsSqlServer
                DatabaseServerType = "MSSQL" & DBSetup.GetSQLServerVersion(options.ConnectionString).VersionMajor
            Case CommandlineOptions.DatabaseServerTypes.MsAzureSql
                DatabaseServerType = "MSSQLAzure" & DBSetup.GetSQLServerVersion(options.ConnectionString).VersionMajor
                'Case CommandlineOptions.DatabaseServerTypes.MySql
                'DatabaseServerType = "MySQL"
            Case Else
                QuitWithError("CRITICAL ERROR: Invalid database server type")
        End Select
        Try
            If options.LimitUpdatesToDbBuild <> 0 Then
                DBSetup.DoUpdates(options.ConnectionString, DBSetup.GetWebManagerReplacements(options.ServerID, options.ServerProtocol, options.ServerOfficialDnsName, options.PortAsText, options.FirstServerGroupTitle, options.FirstServerGroupTitleInNav, options.StandardEMailContact, options.CompanyName, options.FormerCompanyName, options.GeneralWebsiteUrl), options.LimitUpdatesToDbBuild)
            Else
                DBSetup.DoUpdates(options.ConnectionString, DBSetup.GetWebManagerReplacements(options.ServerID, options.ServerProtocol, options.ServerOfficialDnsName, options.PortAsText, options.FirstServerGroupTitle, options.FirstServerGroupTitleInNav, options.StandardEMailContact, options.CompanyName, options.FormerCompanyName, options.GeneralWebsiteUrl))
            End If
            Dim MyDbVersion As Setup.DatabaseSetup.DBServerVersion = DBSetup.GetSQLServerVersion(options.ConnectionString)
            Console.WriteLine(vbNewLine & "UPDATE SUCCESSFULL :-)")
            If options.LimitUpdatesToDbBuild <> 0 Then Console.WriteLine("Update has been limitied to database build no. " & options.LimitUpdatesToDbBuild.ToString)
            Console.WriteLine("Database updates are available up to build no. " & CompuMaster.camm.WebManager.Setup.DatabaseSetup.LastBuildVersionInSetupFiles.ToString)
            Console.WriteLine("Database updates build no. currently installed: " & DBSetup.GetCurrentDBBuildNo(options.ConnectionString).ToString)
        Catch ex As Exception
            QuitWithError("CRITICAL ERROR: " & ex.ToString)
        End Try
    End Sub

End Module
