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

Imports CommandLine
Imports CommandLine.Text

'<Assembly: CommandLine.Text.AssemblyUsage("DbUpdateTool.exe {options}")>

Friend Class CommandlineOptions

    Public Enum DatabaseServerTypes As Byte
        MsSqlServer = 1
        MsAzureSql = 2
    End Enum

    <CommandLine.Option("d"c, "DatabaseServerType", HelpText:="MsSqlServer or MsAzureSql", MutuallyExclusiveSet:="Setup/Update")>
    Public Property DatabaseServerType As DatabaseServerTypes

    <CommandLine.Option("n"c, "CreateNewDatabaseInstance", HelpText:="Cleanup SQL database and create a new camm Web-Manager database instance", MutuallyExclusiveSet:="Setup/Update")>
    Public Property CreateNewDatabaseInstance As Boolean

    <CommandLine.Option("u"c, "UpdateDatabaseInstance", HelpText:="Update an existing camm Web-Manager database instance", MutuallyExclusiveSet:="Setup/Update")>
    Public Property UpdateDatabaseInstance As Boolean

    <CommandLine.Option("c"c, "ConnectionString", HelpText:="The connectionstring to create/update the database schema in an existing SQL database", MutuallyExclusiveSet:="Setup/Update")>
    Public Property ConnectionString As String

    <CommandLine.Option("ConnectionStringForSqlDatabaseCreation", HelpText:="The connectionstring to the SQL server to initially create an (empty) SQL database", MutuallyExclusiveSet:="Setup/Update")>
    Public Property ConnectionStringForSqlDatabaseCreation As String

    <CommandLine.Option("DatabaseNameForSqlDatabaseCreation", HelpText:="The new SQL database name which shall be created", MutuallyExclusiveSet:="Setup/Update")>
    Public Property DatabaseNameForSqlDatabaseCreation As String

    <CommandLine.Option("i"c, "ServerID", HelpText:="Server ID As (to be) referenced in web.config (e. g. Server IP / Host Header Name)", MutuallyExclusiveSet:="Setup/Update")>
    Public Property ServerID As String

    Private _ServerProtocol As String
    <CommandLine.Option("s"c, "ServerProtocol", DefaultValue:="https", HelpText:="HTTPS or HTTP", MutuallyExclusiveSet:="Setup/Update")>
    Public Property ServerProtocol As String
        Get
            Return _ServerProtocol
        End Get
        Set(value As String)
            Select Case value.ToLowerInvariant
                Case "https", "http", ""
                    _ServerProtocol = value.ToLowerInvariant
                Case Else
                    Throw New Exception("Invalid protocol value")
            End Select
        End Set
    End Property

    <CommandLine.Option("p"c, "ServerPort", HelpText:="Port (keep empty for Default)", MutuallyExclusiveSet:="Setup/Update")>
    Public Property Port As Integer
    Friend Function PortAsText() As String
        If Port = 0 Then
            Return ""
        Else
            Return Port.ToString
        End If
    End Function

    <CommandLine.Option("o"c, "ServerOfficialDnsName", HelpText:="WebServer name for browser clients", MutuallyExclusiveSet:="Setup/Update")>
    Public Property ServerOfficialDnsName As String

    <HelpOption()>
    Public Function GetUsage() As String
        Return HelpText.AutoBuild(Me, Sub(current As HelpText) HelpText.DefaultParsingErrorsHandler(Me, current))
        '    var Help = New HelpText {
        '  Heading = New HeadingInfo("<<app title>>", "<<app version>>"),
        '  Copyright = New CopyrightInfo("<<app author>>", 2014),
        '  AdditionalNewLineAfterOption = True,
        '  AddDashesToOption = True
        '};
        'Help.AddPreOptionsLine("<<license details here.>>");
        'Help.AddPreOptionsLine("Usage: app -p Someone");
        'Help.AddOptions(this);
        'Return Help;
    End Function

    <CommandLine.Option("t"c, "FirstServerGroupTitle", HelpText:="First server group title in administration area, e.g. ""YourCompany Extranet""", MutuallyExclusiveSet:="Setup/Update")>
    Public Property FirstServerGroupTitle As String

    <CommandLine.Option("f"c, "FirstServerGroupTitleInNav", HelpText:="First server group title in navigation, e.g. ""Extranet""", MutuallyExclusiveSet:="Setup/Update")>
    Public Property FirstServerGroupTitleInNav As String

    <CommandLine.Option("e"c, "StandardEMailContact", HelpText:="Standard contact e-mail address, e.g. ""webmaster@yourcompany.com""", MutuallyExclusiveSet:="Setup/Update")>
    Public Property StandardEMailContact As String

    <CommandLine.Option("m"c, "CompanyName", HelpText:="Your company name, e.g. ""YourCompany""", MutuallyExclusiveSet:="Setup/Update")>
    Public Property CompanyName As String

    <CommandLine.Option("x"c, "FormerCompanyName", HelpText:="Extended, former company name, e.g. ""YourCompany Ltd.""", MutuallyExclusiveSet:="Setup/Update")>
    Public Property FormerCompanyName As String

    <CommandLine.Option("w"c, "GeneralWebsiteUrl", HelpText:="The official URL of the regular website, e.g. ""http://www.yourcompany.com/""", MutuallyExclusiveSet:="Setup/Update")>
    Public Property GeneralWebsiteUrl As String

    <CommandLine.Option("l"c, "LimitUpdatesToDbBuild", HelpText:="Limit the update process to stop at a specific database build no., e.g. to update the datebase till build no. 206. Without this option, updates are always processed up to the latest available version", MutuallyExclusiveSet:="Setup/Update")>
    Public Property LimitUpdatesToDbBuild As Integer

    <CommandLine.Option("ListAvailablePatches", HelpText:="List the patches that are provided by the current version of this update tool")>
    Public Property ListAvailablePatches As Boolean

    <CommandLine.Option("LatestAvailablePatchVersion", HelpText:="Show the latest update version which can be installed by the current version of this update tool")>
    Public Property LatestAvailableDbPatchVersion As Boolean

    <CommandLine.Option("CurrentDbVersion", HelpText:="Show the current version of the database (requires -c)")>
    Public Property CurrentDbVersion As Boolean

    '<CommandLine.Option("DebugSession", HelpText:="")>
    Public Property DebugSession As Boolean

End Class
