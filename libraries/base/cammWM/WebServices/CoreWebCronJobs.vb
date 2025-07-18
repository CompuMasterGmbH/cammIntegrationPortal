﻿'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager.WebServices

    Friend Class CoreWebCronJobRunner

        ''' <summary>
        ''' Execute all pending processes for mail queue as well as other general todo items
        ''' </summary>
        ''' <param name="httpContext"></param>
        ''' <param name="webManager"></param>
        Public Shared Sub ExecuteNextWebCronJob(httpContext As HttpContext, webManager As WMSystem)

            'Never execute this method in parallel
            Static IsAlreadyInExecution As Boolean
            If IsAlreadyInExecution Then
                Return
            Else
                IsAlreadyInExecution = True
            End If

            Static _CoreWebCronJobRunner As CompuMaster.camm.WebManager.WebServices.CoreWebCronJobRunner
            If _CoreWebCronJobRunner Is Nothing Then
                _CoreWebCronJobRunner = New CompuMaster.camm.WebManager.WebServices.CoreWebCronJobRunner(httpContext, webManager)
            End If

            Try
                Dim StartTime As DateTime = Now
                Dim MaxTotalSecondsTimeout As Integer = CType(httpContext.Server.ScriptTimeout / 2.3, Integer)
                'System.Web.Configuration.WebConfigurationManager.GetSection("httpRuntime")

                'RANDOMIZED EXEC ORDER
                If RandomBoolean() Then
                    _CoreWebCronJobRunner.ProcessMailQueueItems(StartTime, MaxTotalSecondsTimeout)
                    _CoreWebCronJobRunner.ExecutePendingProcesses(StartTime, MaxTotalSecondsTimeout)
                Else
                    _CoreWebCronJobRunner.ExecutePendingProcesses(StartTime, MaxTotalSecondsTimeout)
                    _CoreWebCronJobRunner.ProcessMailQueueItems(StartTime, MaxTotalSecondsTimeout)
                End If
            Finally
                'Allow the next timer event to run this method again
                IsAlreadyInExecution = False
            End Try

        End Sub

        ''' <summary>
        ''' A random boolean value
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>WARNING: Might return the very same result value if called repetitive in loops within a short time frame (e.g. within a single web request)</remarks>
        Private Shared Function RandomBoolean() As Boolean
            Dim RandGen As New Random
            Return CType(RandGen.Next(0, 2), Boolean)
        End Function

        Friend Sub New(httpContext As HttpContext, webManager As WMSystem)
            Me.cammWebManager = webManager
            Me.Context = httpContext
        End Sub

        Private cammWebManager As WMSystem
        Private Context As HttpContext

        ''' <summary>
        ''' Send 1 mail from mail queue
        ''' </summary>
        Private Sub MailQueueProcessOneItem()
            cammWebManager.MessagingQueueMonitoring.ProcessOneMail()
        End Sub

        ''' <summary>
        ''' The number of items in mail queue
        ''' </summary>
        ''' <returns></returns>
        Private Function MailQueueNumberOfItems() As Integer
            Return cammWebManager.MessagingQueueMonitoring.MailsInQueue()
        End Function

        ''' <summary>
        ''' Process all items in mail queue as long as there are more items and the timeout hasn't been reached
        ''' </summary>
        ''' <param name="requestStartTime"></param>
        ''' <param name="maxTotalSecondsTimeout"></param>
        Public Sub ProcessMailQueueItems(requestStartTime As Date, maxTotalSecondsTimeout As Integer)
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return 'if request already consumed to much time, return here and run this step on one of the next requests (when previous steps processed more fast because of nothing/lesser to do there)
            Dim MaxNumberOfQueuedItems As Integer = Me.MailQueueNumberOfItems
            For MyItemCounter As Integer = 1 To MaxNumberOfQueuedItems
                Me.MailQueueProcessOneItem()
                If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return 'if request already consumed to much time, return here and run this step on one of the next requests (when previous steps processed more fast because of nothing/lesser to do there)
            Next
        End Sub

        ''' <summary>
        ''' Execute several processes which are timed asynchronously
        ''' </summary>
        Public Sub ExecutePendingProcesses(requestStartTime As Date, maxTotalSecondsTimeout As Integer)
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return 'if request already consumed to much time, return here and run this step on one of the next requests (when previous steps processed more fast because of nothing/lesser to do there)

            Dim FoundException As Exception = Nothing

            Dim dataProtectionSettings As CompuMaster.camm.WebManager.DataProtectionSettings = Nothing
            Try
                dataProtectionSettings = New CompuMaster.camm.WebManager.DataProtectionSettings(Me.cammWebManager.ConnectionString)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try

            Try
                'Store the current datetime stamp for review in CWM about page
                DataLayer.Current.SaveLastServiceExecutionDate(Me.cammWebManager, Nothing, System.Web.HttpContext.Current.Request.Headers("X-CWM-TriggerServiceVersion"))
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex 'nur neuen Fehler schreiben, wenn nicht bereits ein Fehler auftrat
            End Try
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return 'if request already consumed to much time, return here and run this step on one of the next requests (when previous steps processed more fast because of nothing/lesser to do there)

            Try
                AnonymizeOldIPAddresses(dataProtectionSettings.AnonymizeIPsAfterDays)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return

            Try
                DeleteExpiredUserSessions(500)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return

            Try
                RunCleanups()
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return

            Try
                CleanupUnregisteredFiles()
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(requestStartTime).TotalSeconds > maxTotalSecondsTimeout Then Return

            Try
                DeleteDeactivatedUsers(dataProtectionSettings.DeleteDeactivatedUsersAfterDays)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex 'nur neuen Fehler schreiben, wenn nicht bereits ein Fehler auftrat
            End Try

            Try
                DeleteMails(dataProtectionSettings.DeleteMailsAfterDays)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex 'nur neuen Fehler schreiben, wenn nicht bereits ein Fehler auftrat
            End Try

            'Not activated because on servers with 2 virtual webservers in 1 folder structure, it would always remove all "unregistered" files which are registered for another server instance!
            ''Cleanup unregistered files (typically they shouldn't really exist...) - may take several minutes to proceed!
            'If Me.LastDownloadHandlerCleanUpUnregisteredFiles < Now.Subtract(New TimeSpan(8, 0, 0)) Then
            '    cammWebManager.DownloadHandler.CleanUpUnregisteredFiles()
            '    Me.LastDownloadHandlerCleanUpUnregisteredFiles = Now
            'End If

            If Not FoundException Is Nothing Then
                Try
                    Me.cammWebManager.Log.Exception(FoundException)
                Catch
                End Try
                Throw New Exception("Unexpected exception", FoundException)
            End If
        End Sub

        Private Property LastDownloadHandlerCleanUp() As Date
            Get
                Return Utils.Nz(CType(Me.Context.Cache("LastDownloadHandlerCleanUp"), Date), Date.MinValue)
            End Get
            Set(ByVal Value As Date)
                Me.Context.Cache("LastDownloadHandlerCleanUp") = Value
            End Set
        End Property

        Private Property LastDownloadHandlerCleanUpUnregisteredFiles() As Date
            Get
                Return Utils.Nz(CType(Me.Context.Cache("LastDownloadHandlerCleanUp"), Date), Date.MinValue)
            End Get
            Set(ByVal Value As Date)
                Me.Context.Cache("LastDownloadHandlerCleanUp") = Value
            End Set
        End Property

        ''' <summary>
        ''' Anonymizes old IP Addresses
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AnonymizeOldIPAddresses(ByVal anonymizeAfterDays As Integer)
            If anonymizeAfterDays > 0 Then
                Dim sql As String = "UPDATE log SET RemoteIP = 'anonymized' WHERE LoginDate < DateAdd(dd, @days, GetDate()) and isnull(RemoteIP, '') <> 'anonymized' "
                Dim cmd As New SqlClient.SqlCommand
                cmd.Parameters.Add("@days", SqlDbType.Int).Value = anonymizeAfterDays * -1
                cmd.CommandText = sql
                cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)

                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        ''' <summary>
        ''' Remove inactive and outdated user sessions
        ''' </summary>
        ''' <param name="maxNumberOfDeletedRows"></param>
        Private Sub DeleteExpiredUserSessions(maxNumberOfDeletedRows As Integer)
            Dim Sql As String = "DELETE FROM [dbo].[System_UserSessions]" & vbNewLine & _
                "WHERE ID_Session IN (" & vbNewLine & _
                "  SELECT TOP " & maxNumberOfDeletedRows & " [ID_Session]" & vbNewLine & _
                "  FROM [dbo].[System_UserSessions]" & vbNewLine & _
                "  where ID_Session not in (select SessionID from [dbo].[System_WebAreasAuthorizedForSession_CurrentAndInactiveOnes])" & vbNewLine & _
                "  )"
            Dim connection As New System.Data.SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
            Dim cmd As New System.Data.SqlClient.SqlCommand(Sql, connection)
            cmd.CommandType = CommandType.Text
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

            Me.cammWebManager.Log.LogCleanupAction("Removed expired user sessions")
        End Sub

        Private Sub CleanupUnregisteredFiles()
            'Cleanup unregistered files (typically they shouldn't really exist...)
            If Me.LastDownloadHandlerCleanUp < Now.Subtract(New TimeSpan(0, 30, 0)) Then
                cammWebManager.DownloadHandler.CleanUp()
                Me.LastDownloadHandlerCleanUp = Now
            End If
        End Sub

        Private Sub RunCleanups()
            cammWebManager.Log.CleanUpLogTable()
            DeleteExpiredUserSessions(1000)
        End Sub

        ''' <summary>
        ''' Delete deactivated users
        ''' </summary>
        ''' <param name="deleteAfterDays">0 for never delete any users, 1 or higher to delete inactive user accounts with no change/update within specified amount of days</param>
        Private Sub DeleteDeactivatedUsers(ByVal deleteAfterDays As Integer)
            If deleteAfterDays > 0 Then
                Dim connection As New System.Data.SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
                Dim sql As String = "SELECT ID FROM dbo.Benutzer WHERE LoginDisabled = 1 AND DateAdd(dd, @days, ModifiedOn) < GETDATE() AND ( LastLoginOn IS NULL OR DateAdd(dd, @days, LastLoginOn) < GETDATE() )"
                Dim cmd As New System.Data.SqlClient.SqlCommand(sql, connection)
                cmd.CommandType = CommandType.Text
                cmd.Parameters.Add("@days", SqlDbType.Int).Value = deleteAfterDays

                Dim result As ArrayList = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Dim deleterconnection As System.Data.SqlClient.SqlConnection = Nothing
                Try
                    deleterconnection = New System.Data.SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
                    deleterconnection.Open()
                    For Each userId As Integer In result
                        Dim deleteCommand As New System.Data.SqlClient.SqlCommand("AdminPrivate_DeleteUser")
                        deleteCommand.Connection = deleterconnection
                        deleteCommand.CommandType = CommandType.StoredProcedure
                        deleteCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userId
                        CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(deleteCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                    Next
                Finally
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(deleterconnection)
                End Try

                Me.cammWebManager.Log.LogCleanupAction("Deleted deactivated users")
            End If
        End Sub

        ''' <summary>
        ''' Delete e-mails in one of the mail queue states Cancelled, Successful, FailureAfterLastTrial or FailureAccepted
        ''' </summary>
        ''' <param name="deleteAfterDays"></param>
        Private Sub DeleteMails(ByVal deleteAfterDays As Integer)
            If deleteAfterDays > 0 Then
                Dim sql As String = "DELETE FROM [dbo].[Log_eMailMessages] WHERE State IN (" & CType(Messaging.QueueMonitoring.QueueStates.Cancelled, Integer) & "," & CType(Messaging.QueueMonitoring.QueueStates.Successfull, Integer) & "," & CType(Messaging.QueueMonitoring.QueueStates.FailureAfterLastTrial, Integer) & "," & CType(Messaging.QueueMonitoring.QueueStates.FailureAccepted, Integer) & ") AND DateAdd(dd, @days, DateTime) < GETDATE()"
                Dim connection As New System.Data.SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
                Dim cmd As New System.Data.SqlClient.SqlCommand(sql, connection)
                cmd.Parameters.Add("@days", SqlDbType.Int).Value = deleteAfterDays
                cmd.CommandType = CommandType.Text
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

    End Class

End Namespace