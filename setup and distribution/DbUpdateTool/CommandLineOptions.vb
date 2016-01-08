Imports CommandLine
Imports CommandLine.Text

Friend Class CommandlineOptions

    <CommandLine.Option("a"c, "LicenseAccepted", HelpText:="Confirmation of accepted license is required", Required:=True)>
    Public Property LicenseAccepted As Boolean

    Public Enum DatabaseServerTypes As Byte
        MsSqlServer = 1
        MsAzureSql = 2
    End Enum
    <CommandLine.Option("d"c, "DatabaseServerType", HelpText:="MsSqlServer or MsAzureSql", Required:=True)>
    Public Property DatabaseServerType As DatabaseServerTypes

    <CommandLine.Option("n"c, "CreateNewDatabaseInstance", HelpText:="Cleanup SQL database and create a new camm Web-Manager database instance")>
    Public Property CreateNewDatabaseInstance As Boolean

    <CommandLine.Option("u"c, "UpdateDatabaseInstance", HelpText:="Update an existing camm Web-Manager database instance")>
    Public Property UpdateDatabaseInstance As Boolean

    <CommandLine.Option("c"c, "ConnectionString", HelpText:="The connectionstring to create/update the database schema in an existing SQL database", Required:=True)>
    Public Property ConnectionString As String

    <CommandLine.Option("ConnectionStringForSqlDatabaseCreation", HelpText:="The connectionstring to the SQL server to initially create an (empty) SQL database")>
    Public Property ConnectionStringForSqlDatabaseCreation As String

    <CommandLine.Option("DatabaseNameForSqlDatabaseCreation", HelpText:="The new SQL database name which shall be created")>
    Public Property DatabaseNameForSqlDatabaseCreation As String

    <CommandLine.Option("i"c, "ServerID", HelpText:="Server ID As (to be) referenced in web.config (e. g. Server IP / Host Header Name)")>
    Public Property ServerID As String

    Private _ServerProtocol As String
    <CommandLine.Option("s"c, "ServerProtocol", DefaultValue:="https", HelpText:="HTTPS or HTTP")>
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

    <CommandLine.Option("p"c, "ServerPort", HelpText:="Port (keep empty for Default)")>
    Public Property Port As Integer
    Friend Function PortAsText() As String
        If Port = 0 Then
            Return ""
        Else
            Return Port.ToString
        End If
    End Function

    <CommandLine.Option("o"c, "ServerOfficialDnsName", HelpText:="WebServer name for browser clients")>
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

    <CommandLine.Option("t"c, "FirstServerGroupTitle", HelpText:="First server group title in administration area, e.g. ""YourCompany Extranet""")>
    Public Property FirstServerGroupTitle As String

    <CommandLine.Option("f"c, "FirstServerGroupTitleInNav", HelpText:="First server group title in navigation, e.g. ""Extranet""")>
    Public Property FirstServerGroupTitleInNav As String

    <CommandLine.Option("e"c, "StandardEMailContact", HelpText:="Standard contact e-mail address, e.g. ""webmaster@yourcompany.com""")>
    Public Property StandardEMailContact As String

    <CommandLine.Option("m"c, "CompanyName", HelpText:="Your company name, e.g. ""YourCompany""")>
    Public Property CompanyName As String

    <CommandLine.Option("x"c, "FormerCompanyName", HelpText:="Extended, former company name, e.g. ""YourCompany Ltd.""")>
    Public Property FormerCompanyName As String

    <CommandLine.Option("w"c, "GeneralWebsiteUrl", HelpText:="The official URL of the regular website, e.g. ""http://www.yourcompany.com/""")>
    Public Property GeneralWebsiteUrl As String

    <CommandLine.Option("l"c, "LimitUpdatesToDbBuild", HelpText:="Limit the update process to stop at a specific database build no., e.g. to update the datebase till build no. 206. Without this option, updates are always processed up to the latest available version")>
    Public Property LimitUpdatesToDbBuild As Integer

End Class
