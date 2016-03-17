'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager

#Region "Capabilities"
    ''' <summary>
    '''     Capabilities of this configured camm Web-Manager instance
    ''' </summary>
    ''' <remarks>
    '''     Several possibilities to do specific things have to be defined to work. This allows for example to send e-mails with a queue instead of the immediate distribution of every single e-mail.
    ''' </remarks>
    Public Class WMCapabilities
        Private _WebManager As WMSystem
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a new WMCapabilities class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub New(ByVal webManager As WMSystem)
            _WebManager = webManager

            Messaging = New WMCapabilitiesMessaging(_WebManager)
            Delivery = New WMCapabilitiesDelivery(_WebManager)
        End Sub

        Private _Messaging As WMCapabilitiesMessaging
        Private _Delivery As WMCapabilitiesDelivery
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Messaging capabilities
        ''' </summary>
        ''' <value>A new WMCapabilitiesMessaging</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Messaging() As WMCapabilitiesMessaging
            Get
                Return _Messaging
            End Get
            Set(ByVal Value As WMCapabilitiesMessaging)
                _Messaging = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The delivery capabilities
        ''' </summary>
        ''' <value>A new WMCapabilitiesDelivery</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Delivery() As WMCapabilitiesDelivery
            Get
                Return _Delivery
            End Get
            Set(ByVal Value As WMCapabilitiesDelivery)
                _Delivery = Value
            End Set
        End Property

        Public Function RequiredComponentsQuickCheck() As Boolean
            Dim DetailedCheckData As DataTable = RequiredComponentsDetailedCheck()
            For MyCounter As Integer = 0 To DetailedCheckData.Rows.Count - 1
                If CType(DetailedCheckData.Rows(MyCounter)("Status"), String) <> "Working" Then
                    Return False
                End If
            Next
            Return True
        End Function

        Private Sub RequiredComponentsDetailedCheckForICSharpCodeSharpZipLib()
            Dim MyComponent As New ICSharpCode.SharpZipLib.Zip.FastZip
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfICSharpCodeSharpZipLib() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(ICSharpCode.SharpZipLib.Zip.FastZip)).GetName.Version.ToString()
        End Function

        Public Function RequiredComponentsDetailedCheck() As DataTable
            Dim Result As New DataTable("root")
            Result.Columns.Add("ComponentName", GetType(String))
            Result.Columns.Add("Status", GetType(String))
            Result.Columns.Add("Version", GetType(String))
            Result.Columns.Add("ErrorDetails", GetType(String))
            Dim newRow As DataRow = Result.NewRow

            'Check ZIP component
            newRow = Result.NewRow
            newRow("ComponentName") = "ICSharpCode.SharpZipLib"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForICSharpCodeSharpZipLib()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfICSharpCodeSharpZipLib()
            Catch
                newRow("Version") = Nothing
            End Try
            Result.Rows.Add(newRow)

            Return Result
        End Function

        Public Function RequiredComponentsDetailedCheckHtmlResult() As String
            Return Tools.Data.DataTables.ConvertToHtmlTable(RequiredComponentsDetailedCheck.Rows, "").Replace("<TD>Missing or failing</TD>", "<TD style=""color: red;"">Missing or failing</TD>")
        End Function
        Public Function RequiredComponentsDetailedCheckTextResult() As String
            Return Tools.Data.DataTables.ConvertToPlainTextTable(RequiredComponentsDetailedCheck.Rows, "")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMCapabilities.WMCapabilitiesMessaging
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Messaging capabilities
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class WMCapabilitiesMessaging
            Private _WebManager As WMSystem
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Creates a new WMCapabilitiesMessaging class
            ''' </summary>
            ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Sub New(ByVal webManager As WMSystem)
                _WebManager = webManager
            End Sub
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of e-mails
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property eMail() As Boolean
                Get
                    If _WebManager.SMTPServerName <> "" Then
                        Return True
                    Else
                        Return False
                    End If
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of SMS
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property SMS() As Boolean
                Get
                    'TODO: Not yet implemented
                    Return False
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of MMS
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <Obsolete, System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public ReadOnly Property MMS() As Boolean
                Get
                    'TODO: Not yet implemented
                    Return False
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of e-mail queue
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property eMailQueue() As Boolean
                Get
                    Try
                        Return (Configuration.ProcessMailQueue = TripleState.True OrElse (Configuration.ProcessMailQueue = TripleState.Undefined AndAlso _WebManager.MessagingQueueMonitoring.MonitorIsActive())) AndAlso Setup.DatabaseUtils.Version(_WebManager, True).Build >= WMSystem.MilestoneDBBuildNumber_MailQueue
                    Catch ex As Exception
                        Try
                            Me._WebManager.Log.RuntimeException(ex, True, False)
                        Catch
                            'Ignore the thrown error
                        End Try
                        Return False
                    End Try
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of SMS queue
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property SMSQueue() As Boolean
                Get
                    'TODO: Not yet implemented
                    Return False
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of MMSQueue
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <Obsolete, System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public ReadOnly Property MMSQueue() As Boolean
                Get
                    'TODO: Not yet implemented
                    Return False
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of news archiving system
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property NewsArchiveSystem() As Boolean
                Get
                    'TODO: Not yet implemented
                    Return False
                End Get
            End Property
#If NotYetImplemented Then
                Public ReadOnly Property ICQChat() As Boolean
                    Get
                        Return False
                    End Get
                End Property
                Public ReadOnly Property MSNChat() As Boolean
                    Get
                        Return False
                    End Get
                End Property
                Public ReadOnly Property AIMChat() As Boolean
                    Get
                        Return False
                    End Get
                End Property
#End If
        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMCapabilities.WMCapabilitiesDelivery
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Delivery capabilities
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class WMCapabilitiesDelivery
            Private _WebManager As WMSystem
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Creates a new WMCapabilities class
            ''' </summary>
            ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Sub New(ByVal webManager As WMSystem)
                _WebManager = webManager
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Support of download handler
            ''' </summary>
            ''' <value>True if supported</value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	06.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property DownloadHandler() As Boolean
                Get
                    Return _WebManager.DownloadHandler.IsFullyFeatured
                End Get
            End Property

#If NotYetImplemented Then
                Public ReadOnly Property PrintService() As Boolean
                    Get
                        Return False
                    End Get
                End Property
#End If
        End Class

    End Class
#End Region

End Namespace