Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager.Registration

    ''' <summary>
    ''' Data sent to the webservice, used for validation purposes
    ''' </summary>
    Public Class CwmInstallationInfo
        Public instanceId As String
        Public version As Version
        Public databaseVersion As Version
        Public appInstancesCount As Integer
        Public serverGroupsCount As Integer
        Public serversCount As Integer
        Public enginesCount As Integer
        Public securityObjectsCount As Long
        Public groupsCount As Long
        Public membershipAssignmentsCount As Long
        Public usersCountTotal As Long
        Public usersCountPerServerGroup As ArrayList
        Public supervisorsCount As Integer
        Public securityAdministratorsCount As Integer
        Public securityRelatedContactsCount As Integer
        Public dataProtectionCoordinatorsCount As Integer
        Public RequestTime As DateTime
        Public ValidationHash As Byte()
    End Class

    Public Class CwmInstallationInfoFactory

        Private cammWebManger As WMSystem
        Public Sub New(cwm As WMSystem)
            Me.cammWebManger = cwm
        End Sub

        Private Function GenerateTokenForInstallationInfo(ByVal info As CwmInstallationInfo) As Byte()
            Dim hmac As New System.Security.Cryptography.HMACSHA1(New Byte() {62, 2, 42, 75, 222, 54, 200, 199, 20, 12})
            Dim memoryStream As New System.IO.MemoryStream
            Dim binaryWriter As New System.IO.BinaryWriter(memoryStream)
            binaryWriter.Write(info.appInstancesCount)
            binaryWriter.Write(info.databaseVersion.ToString())
            binaryWriter.Write(info.dataProtectionCoordinatorsCount)
            binaryWriter.Write(info.enginesCount)
            binaryWriter.Write(info.groupsCount)
            binaryWriter.Write(info.instanceId)
            binaryWriter.Write(info.membershipAssignmentsCount)
            binaryWriter.Write(info.securityAdministratorsCount)
            binaryWriter.Write(info.securityObjectsCount)
            binaryWriter.Write(info.securityRelatedContactsCount)
            binaryWriter.Write(info.serverGroupsCount)
            binaryWriter.Write(info.serversCount)
            binaryWriter.Write(info.supervisorsCount)
            binaryWriter.Write(info.usersCountTotal)
            binaryWriter.Write(info.version.ToString())
            binaryWriter.Write(info.RequestTime.Ticks)
            Dim result As Byte() = hmac.ComputeHash(memoryStream)
            binaryWriter.Close()
            memoryStream.Close()
            Return result
        End Function

        Private Function GetCountPerServerGroup(ByVal serversGroupsCount As Integer) As ArrayList
            Dim result As New ArrayList

            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT System_ServerGroups.ID, COUNT(Benutzer.ID) FROM Benutzer INNER JOIN System_Servers ON Benutzer.AccountAccessability = System_Servers.ID INNER JOIN System_ServerGroups ON System_ServerGroups.ID = System_Servers.ServerGroup GROUP BY System_ServerGroups.ID"
            cmd.CommandType = CommandType.Text
            Dim reader As System.Data.IDataReader = Nothing
            Try
                Dim connection As New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
                cmd.Connection = connection
                reader = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                While reader.Read()
                    'TODO: it didn't work with a two-dimensional array, which would be the more reasonable solution
                    Dim serverGroup As Integer = CType(Utils.Nz(reader(0)), Integer)
                    Dim userCount As Integer = CType(Utils.Nz(reader(1)), Integer)
                    result.Add(serverGroup)
                    result.Add(userCount)
                End While
            Finally
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(cmd.Connection)
                If Not reader Is Nothing Then
                    reader.Close()
                    reader.Dispose()
                End If

            End Try

            Return result
        End Function


        Private Function GetIntegerResult(ByVal cmd As IDbCommand) As Integer
            Return CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), Integer)
        End Function

        Private Function GetLongResult(ByVal cmd As IDbCommand) As Long
            Return CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), Long)
        End Function

        Public Function CollectInstallationInfo() As CwmInstallationInfo
            Dim result As New CwmInstallationInfo
            result.instanceId = Me.cammWebManger.Environment.LicenceKey
            result.version = Me.cammWebManger.System_Version_Ex()
            result.databaseVersion = Me.cammWebManger.System_DBVersion_Ex()

            Dim cmd As New SqlClient.SqlCommand
            Try
                cmd.CommandType = CommandType.Text
                cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
                cmd.Connection.Open()

                'Dunno whether really necessary, but kept since it was there before
                cmd.CommandText = "Set TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;"
                Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)

                cmd.CommandText = "Select count(*) As AppInstancesCount From system_globalproperties WHERE propertyname Like 'AppInstance_%' and valuenvarchar = 'camm WebManager' and valuedatetime > dateadd(mm, -1, getdate())"
                result.appInstancesCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT count(ID) FROM System_ServerGroups"
                result.serverGroupsCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM System_Servers"
                result.serversCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM System_WebAreaScriptEnginesAuthorization" 'TODO
                result.enginesCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM dbo.SecurityObjects_CurrentAndInactiveOnes"
                result.securityObjectsCount = GetLongResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM Gruppen"
                result.groupsCount = GetLongResult(cmd)

                cmd.CommandText = "SELECT count(*) as MembershipAssignmentsCount from memberships"
                result.membershipAssignmentsCount = GetLongResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM Benutzer"
                result.usersCountTotal = GetLongResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM dbo.Memberships WHERE ID_GROUP = " & CompuMaster.camm.WebManager.WMSystem.SpecialGroups.Group_SecurityAdministrators
                result.securityAdministratorsCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM dbo.Memberships WHERE ID_GROUP = " & CompuMaster.camm.WebManager.WMSystem.SpecialGroups.Group_Supervisors
                result.supervisorsCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM dbo.Memberships WHERE ID_GROUP = " & CompuMaster.camm.WebManager.WMSystem.SpecialGroups.Group_SecurityRelatedContacts
                result.securityRelatedContactsCount = GetIntegerResult(cmd)

                cmd.CommandText = "Select COUNT(ID) FROM dbo.Memberships WHERE ID_GROUP = " & CompuMaster.camm.WebManager.WMSystem.SpecialGroups.Group_DataProtectionCoordinators
                result.dataProtectionCoordinatorsCount = GetIntegerResult(cmd)

                result.usersCountPerServerGroup = GetCountPerServerGroup(result.serverGroupsCount)
                result.RequestTime = DateTime.Now.ToUniversalTime()
                result.ValidationHash = GenerateTokenForInstallationInfo(result)
                Return result

            Finally
                If Not cmd Is Nothing Then
                    If Not cmd.Connection Is Nothing Then
                        Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(cmd.Connection)
                    End If

                End If
            End Try


        End Function

    End Class


    Public Class LicenceData
        Public Enum LicenseModel As Byte
            Community = 7
            Enterprise = 6
            Professional = 5
            Standard = 4
            Light = 3
            Trial = 2
            Demo = 1
            None = 0
        End Enum

        Public Enum LicenceType As Byte
            None = 0
            PurchaseLicense = 1
            SubscriptionLicense = 2
            IndividualLicense = 3
        End Enum

        Public Model As LicenseModel
        Public Type As LicenceType
        Public ExpirationDate As DateTime

    End Class

    Public Class LicenceInfo
        ''' <summary>
        ''' Returns whether support is provided for the passed licence model
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Public Shared Function IsLicenceModelWithSupport(ByVal model As LicenceData.LicenseModel) As Boolean
            Return Not (model = LicenceData.LicenseModel.Community OrElse model = LicenceData.LicenseModel.Demo OrElse model = LicenceData.LicenseModel.Trial)
        End Function

        ''' <summary>
        ''' Returns whether the licence model is one that can expire
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        Public Shared Function IsExpiringLicenceModel(ByVal model As LicenceData.LicenseModel) As Boolean
            Return Not (model = LicenceData.LicenseModel.Demo OrElse model = LicenceData.LicenseModel.Community OrElse model = LicenceData.LicenseModel.None)
        End Function
    End Class


    Public Class InstanceValidationResult
        ''' <summary>
        ''' Date when the the support and maintenance contract expires
        ''' </summary>
        Public SupportContractExpirationDate As DateTime
        Public UpdateContractExpirationDate As DateTime
        Public LicenceData As LicenceData
        Public ResponseTime As DateTime
        Public ValidationHash As Byte()

        Friend Sub New()
            Me.LicenceData = New LicenceData()
            Me.LicenceData.Model = LicenceData.LicenseModel.Community
        End Sub

        Friend Sub New(licenseModel As LicenceData.LicenseModel)
            Me.LicenceData = New LicenceData()
            Me.LicenceData.Model = licenseModel
        End Sub
    End Class



    ''' <summary>
    ''' The client which contact's the server to ask whether licence etc. are still valid
    ''' </summary>
    <System.Web.Services.WebServiceBindingAttribute(Name:="CimRegistrationServiceClient", _
                [Namespace]:="http://www.camm.biz/support/cim/services/")> _
    Public Class ProductRegistrationClient
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol

        Private info As CwmInstallationInfo

        Public Sub New(ByVal installationInfo As CwmInstallationInfo)
            MyBase.New()
            Me.info = installationInfo
        End Sub



        ''' <summary>
        ''' Computes HMAC for passed InstanceValidationResult class, so we can check the data wasn't manipulated 
        ''' </summary>
        ''' <param name="info"></param>
        ''' <returns></returns>
        Private Function GenerateTokenForValidationResult(ByVal info As InstanceValidationResult) As Byte()
            Dim hmac As New System.Security.Cryptography.HMACSHA1(New Byte() {62, 2, 42, 75, 222, 54, 200, 199, 20, 12})
            Dim memoryStream As New System.IO.MemoryStream
            Dim binaryWriter As New System.IO.BinaryWriter(memoryStream)
            binaryWriter.Write(info.ResponseTime.Ticks)

            If Not info.SupportContractExpirationDate = Nothing Then
                binaryWriter.Write(info.SupportContractExpirationDate.Ticks)
            End If
            If Not info.UpdateContractExpirationDate = Nothing Then
                binaryWriter.Write(info.UpdateContractExpirationDate.Ticks)
            End If
            If Not info.LicenceData.ExpirationDate = Nothing Then
                binaryWriter.Write(info.LicenceData.ExpirationDate.Ticks)
            End If


            binaryWriter.Write(info.LicenceData.Model)
            binaryWriter.Write(info.LicenceData.Type)

            Dim result As Byte() = hmac.ComputeHash(memoryStream)

            binaryWriter.Close()
            memoryStream.Close()

            Return result
        End Function


        ''' <summary>
        ''' Checks whether received data has been tampered with or not
        ''' </summary>
        ''' <param name="info"></param>
        ''' <returns></returns>
        Private Function VerifyValidationResult(ByVal info As InstanceValidationResult) As Boolean
            Dim hash As Byte() = GenerateTokenForValidationResult(info)
            Dim isValid As Boolean = BitConverter.ToString(hash) = BitConverter.ToString(info.ValidationHash)
            If isValid Then 'attempt to mitigate replay attacks
                Return DateTime.Now.ToUniversalTime().Subtract(info.ResponseTime).TotalSeconds <= 300
            End If
        End Function

        <System.Diagnostics.DebuggerStepThrough(), System.Web.Services.Protocols.SoapDocumentMethodAttribute( _
            "http://www.camm.biz/support/cim/services/ServiceVersion", _
            RequestNamespace:="http://www.camm.biz/support/cim/services/", _
            ResponseNamespace:="http://www.camm.biz/support/cim/services/", _
            Use:=System.Web.Services.Description.SoapBindingUse.Literal, _
            ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Default)> _
        Public Function ServiceVersion() As Integer
            Dim result() As Object = Me.Invoke("ServiceVersion", New Object() {})
            Return CType(result(0), Integer)
        End Function

        Protected Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest
            Dim original As System.Net.WebRequest = MyBase.GetWebRequest(uri)
            original.Headers.Add("X-CWM-InstanceID", Me.info.instanceId)
            Return original
        End Function


        <System.Web.Services.Protocols.SoapDocumentMethodAttribute( _
            "http://www.camm.biz/support/cim/services/ValidateInstallation", _
            RequestNamespace:="http://www.camm.biz/support/cim/services/", _
            ResponseNamespace:="http://www.camm.biz/support/cim/services/", _
            Use:=System.Web.Services.Description.SoapBindingUse.Literal, _
            ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Default)> _
        Public Function ValidateInstallation() As InstanceValidationResult
            Dim result() As Object = Nothing
            Try
                Me.Url = "https://www.camm.biz/support/cim/services/registration.asmx"
                result = Me.Invoke("ValidateInstallation", New Object() {info})
            Catch
                Me.Url = "http://www.camm.biz/support/cim/services/registration.asmx"

                result = Me.Invoke("ValidateInstallation", New Object() {info})
            End Try

            Dim contractInfo As InstanceValidationResult = CType(result(0), InstanceValidationResult)
            If VerifyValidationResult(contractInfo) Then
                Return contractInfo
            End If
            Throw New Exception("Response manipulated during transfer or response took too long")
        End Function

    End Class

    Public Enum ContractExpirationNotificationTypes As Integer
        UpdateContract = 0
        SupportAndMaintananceContract = 1
        Licence = 2
    End Enum

    Public Structure ContractExpirationRecipient
        Public fullName As String
        Public email As String
    End Structure

    Public Class ContractExpirationNotifier


        Public Delegate Sub RecpientNotifcationFunction(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date)

        Private notificationFunction As RecpientNotifcationFunction
        Private recipients As New ArrayList

        Private expirationDate As DateTime


        Private cammwebmanager As WMSystem
        Public Sub New(ByVal cwm As WMSystem, ByVal expirationDate As DateTime, ByVal notificationType As ContractExpirationNotificationTypes)
            Me.cammwebmanager = cwm
            Me.expirationDate = expirationDate

            SetDelegate(notificationType)

        End Sub

        Private Sub SetDelegate(ByVal notificationType As ContractExpirationNotificationTypes)
            Select Case notificationType
                Case ContractExpirationNotificationTypes.Licence
                    notificationFunction = New RecpientNotifcationFunction(AddressOf Me.cammwebmanager.Notifications.SendLicenceHasExpiredMessage)
                Case ContractExpirationNotificationTypes.SupportAndMaintananceContract
                    notificationFunction = New RecpientNotifcationFunction(AddressOf Me.cammwebmanager.Notifications.SendSupportContractHasExpiredMessage)
                Case ContractExpirationNotificationTypes.UpdateContract
                    notificationFunction = New RecpientNotifcationFunction(AddressOf Me.cammwebmanager.Notifications.SendUpdateContractHasExpiredMessage)
            End Select
        End Sub
        Public Sub AddRecipient(ByVal recipient As ContractExpirationRecipient)
            Me.recipients.Add(recipient)
        End Sub

        Public Sub AddRecipient(ByVal name As String, ByVal email As String)
            Dim recipient As New ContractExpirationRecipient
            recipient.fullName = name
            recipient.email = email
            AddRecipient(recipient)
        End Sub


        Public Sub AddRecipient(ByVal user As WMSystem.UserInformation)
            AddRecipient(user.FullName, user.EMailAddress)
        End Sub
        Public Sub AddRecipient(ByVal group As WMSystem.SpecialGroups)
            Dim groupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(group, cammwebmanager)
            If Not groupInfo Is Nothing AndAlso Not groupInfo.Members Is Nothing Then
                For Each user As WMSystem.UserInformation In groupInfo.Members
                    AddRecipient(user)
                Next
            End If
        End Sub

        Public Sub Notify()
            Dim sentAlready As New ArrayList
            For Each recipient As ContractExpirationRecipient In recipients
                Dim email As String = recipient.email.Trim()
                Dim name As String = recipient.fullName.Trim()
                If Not sentAlready.Contains(email) Then
                    notificationFunction(name, email, Me.expirationDate)
                    sentAlready.Add(email)
                End If
            Next
        End Sub


    End Class

    Public Class ProductRegistration

        Public Const CacheKeyLastRegistrationUpdate As String = "LastFetchFromServer"
        Public Const CacheKeyServerIsDown As String = "RegistrationServerOffline"
        Private cammWebManger As WMSystem

        Private UpdateRegistrationDataFromServerFailed As Boolean = False



        ''' <summary>
        ''' Contacts our "licence server" and fetches expiration data etc.
        ''' </summary>
        Private Function FetchValidationDataFromServer() As InstanceValidationResult
            Dim factory As New CwmInstallationInfoFactory(Me.cammWebManger)

            Dim client As New ProductRegistrationClient(factory.CollectInstallationInfo())
            Dim installationInfo As InstanceValidationResult = Nothing
            Try
                installationInfo = client.ValidateInstallation()
                UpdateRegistrationDataFromServerFailed = False
            Catch ex As Exception
                UpdateRegistrationDataFromServerFailed = True
                Me.cammWebManger.Log.Exception(ex, False)
            End Try

            Return installationInfo
        End Function

        Private Sub UpdateExpirationMailSendingDate(ByVal key As String, ByVal dateSent As DateTime)
            Dim sql As String = "UPDATE System_GlobalProperties SET ValueDateTime = @dateSent WHERE PropertyName = @key " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueDateTime) VALUES(@key, @dateSent) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@dateSent", SqlDbType.DateTime).Value = dateSent
            cmd.Parameters.Add("@key", SqlDbType.NVarChar).Value = key
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub

        ''' <summary>
        ''' Fetches datetime value with the corresponding key form the global properties table
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' TODO: this doesn't belong to this class, it should probably be in one which is made for this purpose...
        Private Function FetchDateFromDatabase(ByVal key As String) As DateTime
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return Nothing
            End If
            Dim expirationDate As DateTime = CType(value, DateTime)
            Return expirationDate
        End Function

        Private Function MustSendMail(ByVal key As String) As Boolean
            Dim mailsent As DateTime = Me.FetchDateFromDatabase(key)
            Return mailsent = Nothing OrElse DateTime.Now.ToUniversalTime().Subtract(mailsent).TotalHours >= 24
        End Function

        ''' <summary>
        ''' Notifies appropriate recipients when the support and maintanence contract has expired
        ''' </summary>
        Private Sub SendExpiredSupportContractNotificationMails(ByVal expirationDate As DateTime)

            Dim notifier As New ContractExpirationNotifier(Me.cammWebManger, expirationDate, ContractExpirationNotificationTypes.SupportAndMaintananceContract)

            Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()
            Dim expiredSinceDays As Double = currentDate.Subtract(expirationDate).TotalDays

            If expiredSinceDays >= 30 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_SecurityAdministrators)
            End If
            If expiredSinceDays >= 14 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_DataProtectionCoordinators)
            End If
            If expiredSinceDays >= 7 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_Supervisors)
            End If
            If expiredSinceDays >= 0 Then
                Dim serverGroupsInfo As WMSystem.ServerGroupInformation() = Me.cammWebManger.System_GetServerGroupsInfo()
                If Not serverGroupsInfo Is Nothing Then
                    For Each info As WMSystem.ServerGroupInformation In serverGroupsInfo
                        notifier.AddRecipient(info.DevelopmentContactName, info.DevelopmentContactAddress)
                        notifier.AddRecipient(info.SecurityContactName, info.SecurityContactAddress)
                        notifier.AddRecipient(info.ContentManagementContactName, info.ContentManagementContactAddress)
                        notifier.AddRecipient(info.UnspecifiedContactName, info.UnspecifiedContactAddress)
                    Next
                End If
            End If
            Dim key As String = "SupportAndMaintanenceContractMailSent"
            If MustSendMail(key) Then
                notifier.Notify()
                UpdateExpirationMailSendingDate(key, currentDate)
            End If
        End Sub

        Private Sub SendExpiredUpdateContractMail(ByVal expirationDate As DateTime)
            Dim notifier As New ContractExpirationNotifier(Me.cammWebManger, expirationDate, ContractExpirationNotificationTypes.UpdateContract)

            Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()
            Dim expiredSinceDays As Double = currentDate.Subtract(expirationDate).TotalDays

            If expiredSinceDays >= 14 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_Supervisors)
            ElseIf expiredSinceDays >= 5 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_SecurityAdministrators)

            End If
            Dim key As String = "UpdateContractExpirationMailSent"
            If MustSendMail(key) Then
                notifier.Notify()
                UpdateExpirationMailSendingDate(key, DateTime.Now.ToUniversalTime())
            End If
        End Sub

        Private Sub SendExpiringLicenceNotificationMails(ByVal expirationDate As DateTime, ByVal daysTillExpiration As Double)
            Dim notifier As New ContractExpirationNotifier(Me.cammWebManger, expirationDate, ContractExpirationNotificationTypes.Licence)

            If daysTillExpiration <= 14 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_Supervisors)
            ElseIf daysTillExpiration <= 5 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_SecurityAdministrators)

            End If
            Dim key As String = "LicenceExpirationMailSent"
            If MustSendMail(key) Then
                notifier.Notify()
                UpdateExpirationMailSendingDate(key, DateTime.Now.ToUniversalTime())
            End If
        End Sub




        ''' <summary>
        ''' Returns whether the licence server is marked as being down 
        ''' </summary>
        ''' <returns></returns>
        Private Function IsServerMarkedOffline() As Boolean
            Dim downDate As DateTime = Nothing
            Dim cacheValue As Object = HttpContext.Current.Cache.Item(CacheKeyServerIsDown)
            If Not cacheValue Is Nothing Then
                downDate = CType(cacheValue, DateTime)
            Else
                downDate = FetchDateFromDatabase(CacheKeyServerIsDown)
            End If
            Return downDate <> Nothing AndAlso DateTime.Now.Subtract(downDate).TotalMinutes < 5
        End Function

        ''' <summary>
        ''' Mark the server as currently down to try again later.
        ''' </summary>
        Private Sub MarkLicenceServerOffline()
            Dim currentDate As DateTime = DateTime.Now
            HttpContext.Current.Cache.Insert(CacheKeyServerIsDown, currentDate)
            SaveDate(CacheKeyServerIsDown, currentDate)
        End Sub

        ''' <summary>
        ''' Returns whether it's time to fetch validation data from server
        ''' </summary>
        ''' <param name="hours">specifies how many hours must have passed since the last check</param>
        ''' <returns></returns>
        Public Function IsRefreshFromServerRequired(ByVal hours As Integer) As Boolean

            Dim lastCheckDate As DateTime = Nothing
            Dim cacheValue As Object = HttpContext.Current.Cache.Item(CacheKeyLastRegistrationUpdate)
            If Not cacheValue Is Nothing Then
                lastCheckDate = CType(cacheValue, DateTime)
            Else
                lastCheckDate = FetchDateFromDatabase(CacheKeyLastRegistrationUpdate)
            End If

            Return lastCheckDate <> Nothing AndAlso DateTime.Now.Subtract(lastCheckDate).TotalHours >= hours


        End Function

        Private Sub DowngradeToTrialLicence()
            'the problem here is that you get essentially a full installation by always deleting the database records
            'After discussion: can stay this way because there is always a way to trick the system.

            Dim validationDao As New InstanceValidationDao(Me.cammWebManger)
            validationDao.SetLicenceModel(LicenceData.LicenseModel.Trial)
            validationDao.SetLicenceExpirationDate(DateTime.Now.ToUniversalTime().AddDays(21))
        End Sub

        Private Sub DowngradeToDemoLicence()
            Dim validationDao As New InstanceValidationDao(Me.cammWebManger)

            validationDao.SetLicenceModel(LicenceData.LicenseModel.Demo)
            validationDao.SetLicenceExpirationDate(DateTime.Now.ToUniversalTime().AddDays(21))
        End Sub

        Private Sub ProcessSupportContractData(ByVal licenceModel As LicenceData.LicenseModel, ByVal expirationDate As DateTime)
            If LicenceInfo.IsLicenceModelWithSupport(licenceModel) Then
                If Not expirationDate = Nothing Then
                    If DateTime.Now.ToUniversalTime() > expirationDate Then
                        SendExpiredSupportContractNotificationMails(expirationDate)
                    End If
                End If
            End If
        End Sub

        Private Sub ProcessUpdateContractData(ByVal licenceModel As LicenceData.LicenseModel, ByVal expirationDate As DateTime)
            If LicenceInfo.IsLicenceModelWithSupport(licenceModel) Then
                If Not expirationDate = Nothing Then
                    If DateTime.Now.ToUniversalTime() > expirationDate Then
                        SendExpiredUpdateContractMail(expirationDate)
                    End If
                End If
            End If
        End Sub

        Private Sub ProcessLicenceData(ByVal licenceData As LicenceData)
            Dim currentDate As DateTime = DateTime.Now.ToUniversalTime()

            'No licence stored
            If licenceData.Model = LicenceData.LicenseModel.None Then
                DowngradeToTrialLicence()
                Return
            End If

            If LicenceInfo.IsExpiringLicenceModel(licenceData.Model) Then
                'Trial is over
                If licenceData.Model = LicenceData.LicenseModel.Trial Then
                    If currentDate > licenceData.ExpirationDate Then
                        DowngradeToDemoLicence()
                    End If
                Else
                    If Not licenceData.ExpirationDate = Nothing Then
                        'Other licences have a grace period...
                        If DateTime.Now.ToUniversalTime().AddMonths(-1) > licenceData.ExpirationDate Then
                            DowngradeToDemoLicence()
                        End If
                    End If

                End If
                    If licenceData.ExpirationDate > currentDate Then
                    Dim daysTillExpiration As Double = licenceData.ExpirationDate.Subtract(currentDate).TotalDays
                    SendExpiringLicenceNotificationMails(licenceData.ExpirationDate, daysTillExpiration)
                End If

            End If
        End Sub

        Private Sub SaveDate(ByVal key As String, ByVal datevalue As DateTime)

            Dim sql As String = "UPDATE System_GlobalProperties SET ValueDateTime = @date WHERE PropertyName = @propertyname " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueDateTime) VALUES(@propertyname, @date) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = datevalue
            cmd.Parameters.Add("@propertyname", SqlDbType.NVarChar).Value = key
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub

        Private Sub SaveLastRefreshDate(ByVal refreshDate As DateTime)

            HttpContext.Current.Cache.Insert(CacheKeyLastRegistrationUpdate, refreshDate)
            SaveDate(CacheKeyLastRegistrationUpdate, refreshDate)


        End Sub



        ''' <summary>
        ''' Fetches data from the licence server and saves it
        ''' </summary>
        Public Sub RefreshValidationDataFromServer()
            Dim validationResult As InstanceValidationResult = FetchValidationDataFromServer()
            If Not validationResult Is Nothing Then
                Dim validationDao As New InstanceValidationDao(Me.cammWebManger)
                validationDao.Save(validationResult)
                SaveLastRefreshDate(DateTime.Now)
            Else
                MarkLicenceServerOffline()
            End If
        End Sub

        Public Function GetCachedValidationResult() As InstanceValidationResult
            Dim validationDao As New InstanceValidationDao(Me.cammWebManger)
            Return validationDao.Load()

        End Function

        Shared CheckRegistrationLock As Object = New Object()

        ''' <summary>
        ''' Method called by the webservice to check the registration
        ''' </summary>
        Public Sub CheckRegistration()
            Try
                SyncLock CheckRegistrationLock
                    If Not IsServerMarkedOffline() AndAlso IsRefreshFromServerRequired(24) Then
                        RefreshValidationDataFromServer()
                    End If

                    Dim validationResult As InstanceValidationResult = GetCachedValidationResult()
                    If Not validationResult Is Nothing Then
                        ProcessLicenceData(validationResult.LicenceData)
                        ProcessSupportContractData(validationResult.LicenceData.Model, validationResult.SupportContractExpirationDate)
                        ProcessUpdateContractData(validationResult.LicenceData.Model, validationResult.UpdateContractExpirationDate)
                    End If
                End SyncLock
            Catch ex As Exception
                Me.cammWebManger.Log.Exception(ex, False)
            End Try
        End Sub





        Public Sub New(ByVal cammWebManger As WMSystem)
            Me.cammWebManger = cammWebManger
        End Sub


    End Class


    Public Class InstanceValidationDao

        Private cammWebManger As WMSystem

        Public Sub New(ByVal cwm As WMSystem)
            Me.cammWebManger = cwm
        End Sub

        Public Sub Save(ByVal validationResult As InstanceValidationResult)
            If Not validationResult Is Nothing Then
                Dim sql As String = "UPDATE System_GlobalProperties SET ValueInt = @licenceType WHERE PropertyName = 'LicenceType' " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueInt) VALUES('LicenceType', @licenceType) " & System.Environment.NewLine & _
                    "UPDATE System_GlobalProperties SET ValueDateTime = @licenceExpirationDate WHERE PropertyName = 'LicenceExpirationDate' " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueDateTime) VALUES('LicenceExpirationDate', @licenceExpirationDate) " & System.Environment.NewLine & _
                     "UPDATE System_GlobalProperties SET ValueInt = @licenceModel WHERE PropertyName = 'LicenceModel' " & _
                     "IF @@ROWCOUNT = 0 " & _
                     "INSERT INTO System_GlobalProperties (PropertyName, ValueInt) VALUES('LicenceModel', @licenceModel) " & System.Environment.NewLine & _
                    "UPDATE System_GlobalProperties SET ValueDateTime = @supportContractExpirationDate WHERE PropertyName = 'SnMContractExpires' " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueDateTime) VALUES('SnMContractExpires', @supportContractExpirationDate) " & _
                   "UPDATE System_GlobalProperties SET ValueDateTime = @updateContractExpires WHERE PropertyName = 'UpdateContractExpires' " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueDateTime) VALUES('UpdateContractExpires', @updateContractExpires) "
                Dim cmd As New SqlClient.SqlCommand(sql)
                Dim connection As SqlClient.SqlConnection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
                Try
                    connection.Open()
                    cmd.Connection = connection
                    cmd.Transaction = connection.BeginTransaction()
                    cmd.Parameters.Add("@updateContractExpires", SqlDbType.DateTime).Value = validationResult.UpdateContractExpirationDate
                    cmd.Parameters.Add("@supportContractExpirationDate", SqlDbType.DateTime).Value = validationResult.SupportContractExpirationDate
                    cmd.Parameters.Add("@licenceExpirationDate", SqlDbType.DateTime).Value = validationResult.LicenceData.ExpirationDate
                    cmd.Parameters.Add("@licenceType", SqlDbType.Int).Value = validationResult.LicenceData.Type
                    cmd.Parameters.Add("@licenceModel", SqlDbType.Int).Value = validationResult.LicenceData.Model
                    cmd.ExecuteNonQuery()
                    cmd.Transaction.Commit()
                Finally
                    If Not connection Is Nothing Then
                        connection.Close()
                        connection.Dispose()
                    End If
                End Try
            End If
        End Sub

        ''' <summary>
        ''' Fetches datetime value with the corresponding key form the global properties table
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' TODO: this doesn't belong to this class, it should probably be in one which is made for this purpose...
        Private Function GetExpirationDate(ByVal key As String) As DateTime
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return Nothing
            End If
            Dim expirationDate As DateTime = CType(value, DateTime)
            Return expirationDate
        End Function

        ''' <summary>
        ''' Returns the date on which the support and maintance contract expires
        ''' </summary>
        ''' <returns></returns>
        Private Function GetSupportContractExpirationDate() As DateTime
            Return GetExpirationDate("SnMContractExpires")
        End Function

        ''' <summary>
        ''' Returns the date on which the licence expires
        ''' </summary>
        ''' <returns></returns>
        Private Function GetLicenceExpirationDate() As DateTime
            Return GetExpirationDate("LicenceExpirationDate")
        End Function
        ''' <summary>
        ''' Returns the date on which the update contract expires
        ''' </summary>
        ''' <returns></returns>
        Private Function GetUpdateContractExpirationDate() As DateTime
            Return GetExpirationDate("UpdateContractExpires")
        End Function

        ''' <summary>
        ''' Returns the license model
        ''' </summary>
        ''' <returns></returns>
        Private Function GetLicenceModel() As LicenceData.LicenseModel
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueInt FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LicenceModel'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)

            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return Nothing
            End If
            Dim result As LicenceData.LicenseModel = CType(value, LicenceData.LicenseModel)
            Return result
        End Function

        Private Function GetLicenceType() As LicenceData.LicenceType
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueInt FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LicenceType'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)

            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return Nothing
            End If
            Dim result As LicenceData.LicenceType = CType(value, LicenceData.LicenceType)
            Return result
        End Function

        Public Function Load() As InstanceValidationResult
            Dim instanceValidationResult As New InstanceValidationResult

            instanceValidationResult.SupportContractExpirationDate = GetSupportContractExpirationDate()
            instanceValidationResult.UpdateContractExpirationDate = GetUpdateContractExpirationDate()
            instanceValidationResult.LicenceData = New LicenceData()
            instanceValidationResult.LicenceData.ExpirationDate = GetLicenceExpirationDate()
            instanceValidationResult.LicenceData.Model = GetLicenceModel()
            instanceValidationResult.LicenceData.Type = GetLicenceType()

            Return instanceValidationResult
        End Function

        Public Sub SetLicenceModel(ByVal model As LicenceData.LicenseModel)
            Dim sql As String = "UPDATE System_GlobalProperties SET ValueInt = @licenceModel WHERE PropertyName = 'LicenceModel' " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueInt) VALUES('LicenceModel', @licenceModel) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@licenceModel", SqlDbType.Int).Value = model
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub

        Public Sub SetLicenceExpirationDate(ByVal expirationDate As DateTime)
            Dim sql As String = "UPDATE System_GlobalProperties SET ValueDateTime = @expirationDate WHERE PropertyName = 'LicenceExpirationDate' " & _
                    "IF @@ROWCOUNT = 0 " & _
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueDateTime) VALUES('LicenceExpirationDate', @expirationDate) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@expirationDate", SqlDbType.DateTime).Value = expirationDate
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub
    End Class

End Namespace
