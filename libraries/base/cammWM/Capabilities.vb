Option Explicit On 
Option Strict On

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager

#Region "Capabilities"
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMCapabilities
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Capabilities of this configured camm Web-Manager instance
    ''' </summary>
    ''' <remarks>
    '''     Several possibilities to do specific things have to be defined to work. This allows for example to send e-mails with a queue instead of the immediate distribution of every single e-mail.
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
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
            _WebManager = WebManager

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

        Private Sub RequiredComponentsDetailedCheckForQuiksoftSmtp()
            Dim MyComponent As New Quiksoft.EasyMail.SMTP.SMTP
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfQuiksoftSmtp() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(Quiksoft.EasyMail.SMTP.SMTP)).GetName.Version.ToString
        End Function

        Private Sub RequiredComponentsDetailedCheckForComponentArtWebUI()
            Dim MyComponent As New ComponentArt.Web.UI.Menu
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfComponentArtWebUI() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(ComponentArt.Web.UI.Menu)).GetName.Version.ToString
        End Function

        Private Sub RequiredComponentsDetailedCheckForCyberaktAspNet()
            Dim MyComponent As New CYBERAKT.WebControls.Navigation.ASPnetMenu
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfCyberaktAspNet() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(CYBERAKT.WebControls.Navigation.ASPnetMenu)).GetName.Version.ToString
        End Function

        Private Sub RequiredComponentsDetailedCheckForTelerikRadChart()
            Dim MyComponent As New Telerik.WebControls.RadChart
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfTelerikRadChart() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(Telerik.WebControls.RadChart)).GetName.Version.ToString
        End Function

        Private Sub RequiredComponentsDetailedCheckForTelerikRadEditor()
            Dim MyComponent As New Telerik.WebControls.RadEditor
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfTelerikRadEditor() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(Telerik.WebControls.RadEditor)).GetName.Version.ToString
        End Function

        Private Sub RequiredComponentsDetailedCheckForCompuMasterImaging()
            Dim MyComponent As New CompuMaster.Drawing.Imaging.ImageScaling
        End Sub

        Private Function RequiredComponentsDetailedCheckVersionOfCompuMasterImaging() As String
            Return System.Reflection.Assembly.GetAssembly(GetType(CompuMaster.Drawing.Imaging.ImageScaling)).GetName.Version.ToString
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

            'Check SMTP component
            newRow = Result.NewRow
            newRow("ComponentName") = "SMTP.Net"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForQuiksoftSmtp()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfQuiksoftSmtp()
            Catch
                newRow("Version") = Nothing
            End Try
            Result.Rows.Add(newRow)

            'Check ComponentArt component
            newRow = Result.NewRow
            newRow("ComponentName") = "ComponentArt.Web.UI"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForComponentArtWebUI()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfComponentArtWebUI()
            Catch
                newRow("Version") = Nothing
            End Try
            Result.Rows.Add(newRow)

            'Check AspNetMenu component
            newRow = Result.NewRow
            newRow("ComponentName") = "ASPnetMenu"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForCyberaktAspNet()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfCyberaktAspNet()
            Catch
                newRow("Version") = Nothing
            End Try
            Result.Rows.Add(newRow)

            'Check RadChart component
            newRow = Result.NewRow
            newRow("ComponentName") = "RadChart"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForTelerikRadChart()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfTelerikRadChart()
            Catch
                newRow("Version") = Nothing
            End Try
            Result.Rows.Add(newRow)

            'Check RadEditor component
            newRow = Result.NewRow
            newRow("ComponentName") = "RadEditor"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForTelerikRadEditor()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfTelerikRadEditor()
            Catch
                newRow("Version") = Nothing
            End Try
            Result.Rows.Add(newRow)

            'Check CompuMaster.Imaging component
            newRow = Result.NewRow
            newRow("ComponentName") = "CompuMaster.Imaging"
            newRow("ErrorDetails") = Nothing
            Try
                RequiredComponentsDetailedCheckForCompuMasterImaging()
                newRow("Status") = "Working"
            Catch ex As Exception
                newRow("Status") = "Missing or failing"
                newRow("ErrorDetails") = ex.ToString
            End Try
            Try
                newRow("Version") = RequiredComponentsDetailedCheckVersionOfCompuMasterImaging()
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