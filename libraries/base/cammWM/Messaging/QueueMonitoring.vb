'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Messaging

    ''' <summary>
    '''     Methods for the mail queue manager
    ''' </summary>
    Public Class QueueMonitoring

        Private _WebManager As WMSystem
        Friend Sub New(ByVal cammWebManager As WMSystem)
            _WebManager = cammWebManager
        End Sub
        ''' <summary>
        '''     Number of mails in the mail queue
        ''' </summary>
        Public Function MailsInQueue() As Integer
            Dim Result As Object
            Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(_WebManager.ConnectionString),
                            "SELECT COUNT(*) AS NumberOfItems FROM [dbo].[Log_eMailMessages] WHERE State = " & CType(QueueStates.Queued, Byte).ToString & " OR (State = " & CType(QueueStates.FailureAfter1Trial, Byte).ToString & " AND DATEDIFF (mi,[DateTime],GetDate()) > 15) OR (State IN (" & CType(QueueStates.FailureAfter2Trials, Byte) & "," & CType(QueueStates.Sending, Byte) & ") AND DATEDIFF (d,[DateTime],GetDate()) > 1)", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            SetLastActivityDate(New SqlConnection(_WebManager.ConnectionString), Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Return Utils.Nz(Result, 0)
        End Function
        ''' <summary>
        '''     Process first mail in queue
        ''' </summary>
        Public Sub ProcessOneMail()

            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)

            'Load queued elements or those ones which have [failed 1st time 15 minutes or more ago] or have [failed 2nd time 1 day or more ago] or which have [kept on status "Sending" for more than 1 day]
            'To the same time, switch it to state "sending"
            Dim MyReader As IDataReader = Nothing
            Dim MyMailID As Integer, MyMailData As String, MyMailState As QueueStates, MyMailDateTime As DateTime
            Try
                MyReader = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(
                        MyConn,
                        "declare @ID int, @OriginState int" & vbNewLine &
                            "SELECT TOP 1 @ID = [ID], @OriginState = State FROM [dbo].[Log_eMailMessages] WHERE State = " & CType(QueueStates.Queued, Byte).ToString & " OR (State = " & CType(QueueStates.FailureAfter1Trial, Byte).ToString & " AND DATEDIFF (mi,[DateTime],GetDate()) > 15) OR (State IN (" & CType(QueueStates.FailureAfter2Trials, Byte) & "," & CType(QueueStates.Sending, Byte) & ") AND DATEDIFF (d,[DateTime],GetDate()) > 1) ORDER BY State" & vbNewLine &
                            "UPDATE [dbo].[Log_eMailMessages] SET State = " & CType(QueueStates.Sending, Byte).ToString & ", [DateTime] = GETDATE() WHERE ID = @ID" & vbNewLine &
                            "SELECT [ID], [UserID], [data], [State], [DateTime], @OriginState As OriginState FROM [dbo].[Log_eMailMessages] WHERE ID = @ID",
                        CommandType.Text,
                        Nothing,
                        Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                'Read one data row
                If Not MyReader.Read Then
                    'Nothing to do; return immediately
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                    Return
                Else
                    MyMailID = CType(MyReader("ID"), Integer)
                    MyMailData = CType(MyReader("Data"), String)
                    MyMailState = CType(MyReader("OriginState"), CompuMaster.camm.WebManager.Messaging.QueueMonitoring.QueueStates)
                    MyMailDateTime = CType(MyReader("DateTime"), DateTime)
                End If
            Catch
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                Throw
            Finally
                MyReader.Close()
            End Try

            'Send Mail
            Dim Success As Boolean
            Try
                Success = SendMail(MyMailData, MyMailID, MyConn)
            Catch ex As Exception
                SetLastErrorDetails(ex.ToString, MyMailID, MyConn, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                If Setup.DatabaseUtils.Version(Me._WebManager, True).Build < 155 Then 'exception hasn't been logged in queue, so log it in event log
                    Me._WebManager.Log.RuntimeException(ex, False, False, WMSystem.DebugLevels.NoDebug)
                End If
                Success = False
            End Try

            'Switch to next state
            Dim NewState As QueueStates
            If Success Then
                NewState = QueueStates.Successfull
            Else
                Select Case MyMailState
                    Case QueueStates.Queued
                        NewState = QueueStates.FailureAfter1Trial
                    Case QueueStates.FailureAfter1Trial
                        NewState = QueueStates.FailureAfter2Trials
                    Case Else 'QueueStates.FailureAfter2Trials, QueueStates.Sending
                        NewState = QueueStates.FailureAfterLastTrial
                End Select
            End If
            Try
                'Update this single mail
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyConn, "UPDATE [dbo].[Log_eMailMessages] SET State = " & CType(NewState, Byte).ToString & ", [DateTime] = GETDATE() WHERE ID = " & MyMailID.ToString, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                'Cleanup all queued elements never commited or cancelled and cancel them after 1 day
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyConn, "UPDATE [dbo].[Log_eMailMessages] SET State = " & CType(QueueStates.Cancelled, Byte).ToString & ", [DateTime] = GETDATE() WHERE State = " & CType(QueueStates.WaitingForReleaseBeforeQueuing, Byte).ToString & " AND DateAdd(dd, 1, [DateTime]) < GETDATE()", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
            Catch
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                Throw
            End Try

            'Trigger activity and queue truncation
            Try
                SetLastActivityDate(MyConn, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                CleanUpOldQueueElementsWhenNotDoneTooLongTime(MyConn)
            Finally
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
            End Try

        End Sub
        ''' <summary>
        '''     Send the mail which has been serialized as XML data
        ''' </summary>
        ''' <param name="xml">The XML representation of the mail data</param>
        ''' <param name="mailQueueID">The ID of the item in the mail queue, required for logging purposes only</param>
        ''' <returns>True if successfull</returns>
        Private Function SendMail(ByVal xml As String, ByVal mailQueueID As Integer, ByVal dbConnection As SqlClient.SqlConnection) As Boolean

            Dim MailData As New WebManager.Messaging.MailMessage(xml, _WebManager)
            Dim Errors As String = String.Empty
            Dim Result As Boolean = _WebManager.MessagingEMails.SendEMail(False, MailData.To, MailData.Cc, MailData.Bcc, MailData.Subject, MailData.BodyPlainText, MailData.BodyHtml, MailData.FromName, MailData.FromAddress, MailData.EMailAttachments, MailData.Priority, MailData.Sensitivity, MailData.RequestTransmissionConfirmation, MailData.RequestReadingConfirmation, MailData.AdditionalHeaders, MailData.Charset, Errors)
            If Errors <> Nothing Then
                SetLastErrorDetails(Errors, mailQueueID, dbConnection, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                Dim WorkaroundEx As New Exception("")
                Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                Try
                    WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                Catch
                End Try
                _WebManager.Log.RuntimeWarning("Error while sending mail ID " & mailQueueID & " from " & MailData.FromAddress & " via mail queue: " & vbNewLine & Errors & vbNewLine, WorkaroundStackTrace)
                Result = False
            End If
            Return Result

        End Function
        ''' <summary>
        '''     Update the state of a queued item
        ''' </summary>
        ''' <param name="queuedItemID">ID of Log_emailMessage record</param>
        ''' <param name="state">State of an email</param>
        Public Sub UpdateQueueState(ByVal queuedItemID As Integer, ByVal state As Messaging.QueueMonitoring.QueueStates)
            Dim dbConn As New System.Data.SqlClient.SqlConnection(Me._WebManager.ConnectionString)
            dbConn.Open()

            Dim cmd As New SqlClient.SqlCommand
            Try
                With cmd
                    .CommandText = "Update [dbo].[Log_eMailMessages] Set [State] = " & state & " where [ID] = " & queuedItemID
                    .CommandType = CommandType.Text
                    .Connection = dbConn

                    .ExecuteNonQuery()
                End With

            Finally
                If Not cmd Is Nothing Then
                    cmd.Dispose()
                End If
                If Not dbConn Is Nothing Then
                    If dbConn.State <> ConnectionState.Closed Then
                        dbConn.Close()
                    End If
                    dbConn.Dispose()
                End If
            End Try
        End Sub
        ''' <summary>
        '''     Log_eMailData for emailID
        ''' </summary>
        ''' <param name="queuedItemID">unique id of Log_eMailMessage record</param>
        Public Function LoadMailMessage(ByVal queuedItemID As Integer) As WebManager.Messaging.MailMessage
            Dim result As String

            Dim dbConn As New System.Data.SqlClient.SqlConnection(Me._WebManager.ConnectionString)
            dbConn.Open()

            Dim obj As Object
            Dim cmd As New SqlClient.SqlCommand
            Try
                With cmd
                    .CommandText = "SELECT [Data] FROM [dbo].[Log_eMailMessages] where [ID] = " & queuedItemID
                    .CommandType = CommandType.Text
                    .Connection = dbConn

                    obj = .ExecuteScalar
                End With

            Finally
                If Not cmd Is Nothing Then
                    cmd.Dispose()
                End If
                If Not dbConn Is Nothing Then
                    If dbConn.State <> ConnectionState.Closed Then
                        dbConn.Close()
                    End If
                    dbConn.Dispose()
                End If
            End Try
            result = Utils.Nz(obj, CType(Nothing, String))

            Return New MailMessage(result, _WebManager)
        End Function
        ''' <summary>
        '''     Available states in the mail queue table
        ''' </summary>
        Public Enum QueueStates As Byte
            ''' <summary>
            '''     The mail has been stored into the queue, but hasn't been allowed to be sent yet
            ''' </summary>
            WaitingForReleaseBeforeQueuing = 0
            ''' <summary>
            '''     Mail has been queued, sending will follow asap.
            ''' </summary>
            Queued = 1
            ''' <summary>
            '''     The queue mailing system is currently engaged to send this mail
            ''' </summary>
            Sending = 2
            ''' <summary>
            '''     Mail transmission has been cancelled by the sending user or the sending application
            ''' </summary>
            Cancelled = 3
            ''' <summary>
            '''     Mail has been delivered without errors
            ''' </summary>
            ''' <remarks>
            '''     This doesn't say that (all of) the receipients have got the mail, it only says that there was no error while sending it
            ''' </remarks>
            Successfull = 4
            ''' <summary>
            '''     Delivery of the mail has been failed one time
            ''' </summary>
            FailureAfter1Trial = 5
            ''' <summary>
            '''     Delivery of the mail has been failed two times
            ''' </summary>
            FailureAfter2Trials = 6
            ''' <summary>
            '''     The transfer will not be repeated, the delivery of the mail has been failed three times
            ''' </summary>
            FailureAfterLastTrial = 7
            ''' <summary>
            '''     The three time failure is accepted by moderator
            ''' </summary>
            FailureAccepted = 8
        End Enum

#Region "Trigger/register the availability of the queuing mechanism"
        ''' <summary>
        '''     Set the last error information by the mail queue processor
        ''' </summary>
        Private Sub SetLastErrorDetails(ByVal errorDetails As String, ByVal mailQueueID As Integer, ByVal connection As SqlConnection, ByVal automations As CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations)
            Dim _DBBuildNo As Integer = Setup.DatabaseUtils.Version(Me._WebManager, True).Build
            If _DBBuildNo >= 155 Then
                Try
                    Dim MyCmd As New SqlCommand("UPDATE [dbo].[Log_eMailMessages] SET [ErrorDetails] = @ErrDetails WHERE ID = @ID", connection)
                    If errorDetails = Nothing Then
                        MyCmd.Parameters.Add("@ErrDetails", SqlDbType.NText).Value = DBNull.Value
                    Else
                        MyCmd.Parameters.Add("@ErrDetails", SqlDbType.NText).Value = errorDetails
                    End If
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = mailQueueID
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, automations)
                Catch
                End Try
            End If
        End Sub
        ''' <summary>
        '''     Set the last activity information by the mail queue processor
        ''' </summary>
        Private Sub SetLastActivityDate(ByVal connection As SqlConnection, ByVal automations As CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations)
            Try
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(
                        connection,
                        "DECLARE @RowNumber int" & vbNewLine &
                            "SELECT @RowNumber = COUNT(*)" & vbNewLine &
                            "FROM [dbo].[System_GlobalProperties]" & vbNewLine &
                            "WHERE VALUENVarChar = N'camm WebManager' AND PropertyName = N'LastMailQueueProcessing'" & vbNewLine &
                            "SELECT @RowNumber" & vbNewLine &
                            vbNewLine &
                            "IF @RowNumber = 0 " & vbNewLine &
                            "	INSERT INTO [dbo].[System_GlobalProperties]" & vbNewLine &
                            "		(ValueNVarChar, PropertyName, ValueDateTime)" & vbNewLine &
                            "	VALUES (N'camm WebManager', N'LastMailQueueProcessing', GetDate())" & vbNewLine &
                            "ELSE" & vbNewLine &
                            "	UPDATE [dbo].[System_GlobalProperties]" & vbNewLine &
                            "	SET ValueDateTime = GetDate()" & vbNewLine &
                            "	WHERE ValueNVarChar = N'camm WebManager' AND PropertyName = N'LastMailQueueProcessing'" & vbNewLine,
                        CommandType.Text,
                        Nothing,
                        automations)
            Catch
            End Try
        End Sub
        ''' <summary>
        '''     Has the monitor been active in the last 30 minutes?
        ''' </summary>
        Friend Function MonitorIsActive() As Boolean

            Dim Result As Integer
            Try
                Result = CType(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(
                        New SqlConnection(_WebManager.ConnectionString),
                            "SELECT COUNT(*)" & vbNewLine &
                            "FROM [dbo].[System_GlobalProperties]" & vbNewLine &
                            "WHERE VALUENVarChar = N'camm WebManager' AND PropertyName = N'LastMailQueueProcessing' AND DATEDIFF (mi,[ValueDateTime],GetDate()) < 30",
                        CommandType.Text,
                        Nothing,
                        Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), Integer)
            Catch
                Result = 0
            End Try
            'Return true if there was one row found
            Return (Result = 1)

        End Function

#End Region

#Region "Shorten mail queue table when there are too old rows"

        Private Sub CleanUpOldQueueElementsWhenNotDoneTooLongTime(ByVal connection As SqlConnection)

            'Detect the last execution of the cleanup method
            Static LastCleanUpCached As DateTime
            Dim LastCleanUp As DateTime
            If HttpContext.Current Is Nothing Then
                LastCleanUp = LastCleanUpCached
            Else
                Try
                    LastCleanUp = CType(HttpContext.Current.Application("WebManager.Messaging.Queue.LastCleanUp"), DateTime)
                Catch
                End Try
            End If
            'Detect if it has to be run again
            Dim PerformCleanup As Boolean
            If LastCleanUp = Nothing Then
                PerformCleanup = True
            ElseIf Now.Subtract(LastCleanUp).TotalHours > 4 Then
                PerformCleanup = True
            End If
            If PerformCleanup = True Then
                Me.CleanUpOldQueueElements(connection)
                If HttpContext.Current Is Nothing Then
                    LastCleanUpCached = Now
                Else
                    HttpContext.Current.Application("WebManager.Messaging.Queue.LastCleanUp") = Now
                End If
            End If

        End Sub
        ''' <summary>
        '''     Remove old lines from the mail queue which are older than 14 days
        ''' </summary>
        ''' <remarks>
        '''     An additional new log entry will be created to log the truncation
        ''' </remarks>
        Private Sub CleanUpOldQueueElements(ByVal connection As SqlConnection)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(
                        connection,
                        "DELETE [dbo].[Log_eMailMessages] FROM (SELECT ID FROM [dbo].[Log_eMailMessages] WHERE State IN (" & CType(QueueStates.Cancelled, Byte) & "," & CType(QueueStates.Successfull, Byte) & "," & CType(QueueStates.FailureAfterLastTrial, Byte) & "," & CType(QueueStates.FailureAccepted, Byte) & ") AND DATEDIFF (d,[DateTime],GetDate()) > 90) AS toremove WHERE dbo.Log_eMailMessages.ID = toremove.ID",
                        CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.None, 300)
        End Sub
#End Region

    End Class

End Namespace