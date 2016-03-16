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

Option Strict On
Option Explicit On 

Namespace CompuMaster.camm.WebManager.WebServices

    ''' <summary>
    '''     The base web service which implements the cammWebManager property
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public MustInherit Class BaseWebService
        Inherits System.Web.Services.WebService

        Private WithEvents _WebManager As CompuMaster.camm.WebManager.WMSystem
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The current instance of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	20.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property cammWebManager() As CompuMaster.camm.WebManager.WMSystem
            Get
                If _WebManager Is Nothing Then
                    'Create an instance on the fly
                    _WebManager = OnWebManagerJustInTimeCreation()
                    _WebManager.PageOnInit(Nothing, Nothing)
                End If
                Return _WebManager
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem)
                _WebManager = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create a camm Web-Manager instance on the fly
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function OnWebManagerJustInTimeCreation() As WMSystem
            Dim Result As WMSystem
            Result = New WMSystem(Me.GetType)
            Return Result
        End Function

        Private Sub _WebManager_InitLoadConfiguration() Handles _WebManager.InitLoadConfiguration
            'Initialize configuration and environment
            cammWebManager.ConnectionString = Configuration.ConnectionString
            cammWebManager.CurrentServerIdentString = Configuration.WebServiceCurrentServerIdentification
        End Sub

    End Class



    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WebServices.Core
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     camm Web-Manager core web service
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[AdminSupport]	10.05.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <Web.Services.WebService(name:="camm Web-Manager core web services", _
                description:="Core camm Web-Manager service for event triggering of queued or scheduled tasks", _
                [namespace]:="http://www.camm.biz/webmanager/core/"), System.Runtime.InteropServices.ComVisible(False)> _
    Public Class Core
        Inherits BaseWebService

        <Web.Services.WebMethod()> Public Sub MailQueueProcessOneItem()
            cammWebManager.MessagingQueueMonitoring.ProcessOneMail()
        End Sub

        <Web.Services.WebMethod()> Public Function MailQueueNumberOfItems() As Integer
            Return cammWebManager.MessagingQueueMonitoring.MailsInQueue()
        End Function


        ''' <summary>
        ''' Execute several processes which are timed asynchronously
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        <Web.Services.WebMethod()> Public Sub ExecutePendingProcesses()

            Dim start As DateTime = Now
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

            Try
                VerifyProductRegistration()
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(start).TotalSeconds > 10 Then Return 'if request already consumed to much time, return here and run this step on one of the next requests (when previous steps processed more fast because of nothing/lesser to do there)

            Try
                AnonymizeOldIPAddresses(dataProtectionSettings.AnonymizeIPsAfterDays)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(start).TotalSeconds > 10 Then Return

            Try
                DeleteExpiredUserSessions(500)
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(start).TotalSeconds > 10 Then Return

            Try
                RunCleanups()
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(start).TotalSeconds > 10 Then Return

            Try
                CleanupUnregisteredFiles()
            Catch ex As Exception
                If FoundException Is Nothing Then FoundException = ex
            End Try
            If Now.Subtract(start).TotalSeconds > 10 Then Return

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

        Private Sub VerifyProductRegistration()
            Dim productRegistration As New Registration.ProductRegistration(Me.cammWebManager)
            productRegistration.CheckRegistration(False)
        End Sub

        Private Sub DeleteDeactivatedUsers(ByVal deleteAfterDays As Integer)
            If deleteAfterDays > 0 Then
                Dim connection As New System.Data.SqlClient.SqlConnection(Me.cammWebManager.ConnectionString)
                Dim sql As String = "SELECT ID FROM dbo.Benutzer WHERE LoginDisabled = 1 AND DateAdd(dd, @days, ModifiedON) < GETDATE()"
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