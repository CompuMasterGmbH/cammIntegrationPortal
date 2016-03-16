'Copyright 2003-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
