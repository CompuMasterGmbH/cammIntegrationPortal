Module MainCode

    Public Const SetupPackageName As String = "WebManager"
    Public Const SetupPackageTitle As String = "Web-Manager"
    Friend WithEvents DBSetup As New CompuMaster.camm.WebManager.Setup.DatabaseSetup(SetupPackageName, SetupPackageTitle)

    Public Sub Main()

        Dim options As New CommandlineOptions()
        If Not CommandLine.Parser.Default.ParseArguments(System.Environment.GetCommandLineArgs, options) Then
            Environment.Exit(CommandLine.Parser.DefaultExitCodeFail)
        ElseIf options.ConnectionStringForSqlDatabaseCreation <> "" Xor options.CreateNewDatabaseInstance Xor options.UpdateDatabaseInstance = False Then
            Console.WriteLine("Either -CreateNewDatabaseInstance or -UpdateDatabaseInstance or -ConnectionStringForSqlDatabaseCreation must be used")
            Environment.Exit(CommandLine.Parser.DefaultExitCodeFail)
        ElseIf options.ConnectionStringForSqlDatabaseCreation <> "" AndAlso options.DatabaseNameForSqlDatabaseCreation <> "" Then
            CommandLineApp.TestConnection(options)
            CommandLineApp.CreateSqlDatabaseAndCwmInstance(options)
            CommandLineApp.UpdateWebManagerInstance(options)
        ElseIf options.CreateNewDatabaseInstance Then
            CommandLineApp.TestConnection(options)
            CommandLineApp.CleanupSqlDatabaseAndCreateWebManagerInstance(options)
            CommandLineApp.UpdateWebManagerInstance(options)
        ElseIf options.UpdateDatabaseInstance Then
            CommandLineApp.TestConnection(options)
            CommandLineApp.UpdateWebManagerInstance(options)
        Else
            QuitWithError("Unexpected operation flow")
        End If
    End Sub

    Private Sub DBSetup_ProgressStepStatusChanged() Handles DBSetup.ProgressStepStatusChanged
        'Do Nothing or TODO: ProgressBar 2
    End Sub

    Private Sub DBSetup_ProgressTaskStatusChanged() Handles DBSetup.ProgressTaskStatusChanged
        Console.WriteLine(vbNewLine & DBSetup.ProgressOfTasks.CurrentStepTitle & vbNewLine & Strings.StrDup(DBSetup.ProgressOfTasks.CurrentStepTitle.Length, "=") & vbNewLine)
        'Me.ProgressBarInstallation.Minimum = 0
        'Me.ProgressBarInstallation.Maximum = DBSetup.ProgressOfTasks.StepsTotal
        'Me.ProgressBarInstallation.Value = DBSetup.ProgressOfTasks.CurrentStepNumber
        'Me.ProgressBarInstallation.Text = DBSetup.ProgressOfTasks.CurrentStepTitle
    End Sub

    Private Sub DBSetup_StepStatusChanged() Handles DBSetup.StepStatusChanged
        Console.WriteLine(DBSetup.CurrentStepTitle & vbNewLine)
    End Sub

    Private Sub DBSetup_WarningsQueueChanged() Handles DBSetup.WarningsQueueChanged
        Console.WriteLine("WARNING: " & DBSetup.Warnings)
    End Sub

    Friend Sub QuitWithError(errorInfo As String)
        Console.WriteLine(errorInfo)
        Environment.Exit(2)
    End Sub

End Module
