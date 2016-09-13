'Copyright 2015-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports CompuMaster.camm.WebManager

Friend Module CommandLineApp

    Sub ShowCurrentDbBuildNo(options As CommandlineOptions)
        Dim MyDBVersion As Setup.DatabaseSetup.DBServerVersion
        MyDBVersion = DBSetup.GetSQLServerVersion(options.ConnectionString)

        ' Test connection for MS SQL Servers
        If MyDBVersion.ErrorsFound = True Then
            If MyDBVersion.Exception.InnerException IsNot Nothing AndAlso MyDBVersion.Exception.Message = "Data layer exception" Then
                'show inner exception for CM DataLayer Exception
                QuitWithError("CRITICAL ERROR: Error connecting to database server: " & MyDBVersion.Exception.InnerException.Message)
            Else
                QuitWithError("CRITICAL ERROR: Error connecting to database server: " & MyDBVersion.Exception.Message)
            End If
        Else
            Console.WriteLine("Database build no. currently installed: " & DBSetup.GetCurrentDBBuildNo(options.ConnectionString).ToString)
        End If
    End Sub

    Sub ShowLatestAvailableDbPatchVersion()
        Console.WriteLine("Database updates are available up to build no. " & CompuMaster.camm.WebManager.Setup.DatabaseSetup.LastBuildVersionInSetupFiles.ToString)
    End Sub

    Sub ListAvailablePatches()
        'Get builds overview
        Dim ds_build_index As New Data.DataSet
        Dim indexdata As New Data.DataView
        Dim Stream As System.IO.TextReader = Nothing
        Try
            Dim BuildIndex As String = CompuMaster.camm.WebManager.Setup.DatabaseSetup.ResourceStringDatabaseSetup("build_index.xml")
            If BuildIndex = Nothing Then
                Throw New Exception("Resources for update not available; these are the error details: " & CompuMaster.camm.WebManager.Setup.DatabaseSetup.ValidateResourceAccessability())
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
        For MyCounter As Integer = 0 To indexdata.Count - 1
            Dim MyRow As Data.DataRowView = indexdata.Item(MyCounter)
            Dim BuildMetaData As String = CompuMaster.camm.WebManager.Setup.DatabaseSetup.ResourceStringDatabaseSetup(CType(MyRow("file"), String))
            If BuildMetaData <> "" Then
                Try
                    Stream = New System.IO.StringReader(BuildMetaData)
                    ds_data.ReadXml(Stream)
                    Dim MyDataRow As Data.DataRow = ds_data.Tables("version").Rows(ds_data.Tables("version").Rows.Count - 1)
                    Console.WriteLine(New Version(CType(MyDataRow("major"), Integer), CType(MyDataRow("minor"), Integer), CType(MyDataRow("build"), Integer)))
                Catch ex As Exception
                    Throw New Exception("Error reading meta data in " & CType(MyRow("file"), String), ex)
                Finally
                    If Not Stream Is Nothing Then Stream.Close()
                End Try
            End If
        Next
    End Sub

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
            Catch ex As CompuMaster.camm.WebManager.Setup.DatabaseSetup.DatabasePatchCommandException
                QuitWithError("CRITICAL ERROR: " & ex.ToString & vbNewLine & "DB Patch Command: " & ex.AdditionalErrorDetails)
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
        Catch ex As CompuMaster.camm.WebManager.Setup.DatabaseSetup.DatabasePatchCommandException
            QuitWithError("CRITICAL ERROR: " & ex.ToString & vbNewLine & "DB Patch Command: " & ex.AdditionalErrorDetails)
        Catch ex As Exception
            QuitWithError("CRITICAL ERROR: " & ex.ToString)
        End Try
    End Sub

End Module
