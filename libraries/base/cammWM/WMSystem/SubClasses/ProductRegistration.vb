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

Option Strict On
Option Explicit On

Namespace CompuMaster.camm.WebManager.Registration
#Const ProdRegServiceCodeLocationIsClient = True

    ''' <summary>
    ''' Data sent to the webservice, used for validation purposes
    ''' </summary>
    <Obsolete("FEATURE DISABLED")>
    Public Class CwmInstallationInfo
        Public instanceId As String
        ''' <summary>
        ''' CIM assembly version
        ''' </summary>
        Friend Property version As Version
            Get
                Return New Version(Me.VersionAssembly)
            End Get
            Set(value As Version)
                Me.VersionAssembly = value.ToString
            End Set
        End Property
        ''' <summary>
        ''' CIM database version
        ''' </summary>
        Public VersionAssembly As String
        Friend Property databaseVersion As Version
            Get
                Return New Version(Me.VersionDatabase)
            End Get
            Set(value As Version)
                Me.VersionDatabase = value.ToString
            End Set
        End Property
        Public VersionDatabase As String
        ''' <summary>
        ''' CIM database version
        ''' </summary>
        Public VersionWebCron As String
        Friend Property webCronVersion As Version
            Get
                Return New Version(Me.VersionWebCron)
            End Get
            Set(value As Version)
            End Set
        End Property
        Public WebCronLastExecution As DateTime
        Public appInstancesCount As Integer
        Public serverGroupsCount As Integer
        Public serversCount As Integer
        Public enginesCount As Integer
        Public securityObjectsCount As Long
        Public groupsCount As Long
        Public membershipAssignmentsCount As Long
        Public usersCountTotal As Long
        Public usersCountPerServerGroup As Integer()
        Public AuthsDirectlyCount As Long
        Public AuthsIndirectlyCount As Long
        Public GroupAuthsDirectlyCount As Long
        Public supervisorsCount As Integer
        Public securityAdministratorsCount As Integer
        Public securityRelatedContactsCount As Integer
        Public dataProtectionCoordinatorsCount As Integer
        Public securityAccessEverythingCount As Integer
        Public ActiveMarketsCount As Integer
        Public RequestTime As DateTime
        Public ValidationHash As Byte()
        Public WebAppInstanceID As String
        Public UsersVerifiedCount As Long
        Public ProcessedAuthentificationsBy1FactorCount As Long
        Public ProcessedAuthentificationsBy2FactorsCount As Long
        Public ProcessedAuthentificationsByExternalAccountCount As Long
        ''' <summary>
        ''' The format version of this class
        ''' </summary>
        Public TransmissionPacketVersion As Integer = 1
        ''' <summary>
        ''' A hidden key for communication between instance and license server
        ''' </summary>
        Public KeyLicenseUsagePublishing As Byte()

        ''' <summary>
        ''' Serialize all data into a string
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Nothing
        End Function

        ''' <summary>
        ''' Deserialize all data from a string and create a new instance of this class
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Shared Function CreateInstanceFromString(data As String) As CwmInstallationInfo
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' Create instance of CwmInstallationInfo
    ''' </summary>
    <Obsolete("FEATURE DISABLED", True)>
    Public Class CwmInstallationInfoFactory

        Private cammWebManger As WMSystem
        Public Sub New(cwm As WMSystem)
            Me.cammWebManger = cwm
        End Sub

        ''' <summary>
        ''' Collect statistics and information on the current camm Web-Manager instance
        ''' </summary>
        <Obsolete("FEATURE DISABLED", True)>
        Public Function CollectInstallationInfo() As CwmInstallationInfo
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' License details
    ''' </summary>
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

        ''' <summary>
        ''' Serialize all data into a string
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Nothing
        End Function

        ''' <summary>
        ''' Deserialize all data from a string and create a new instance of this class
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Shared Function CreateInstanceFromString(data As String) As LicenceData
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' License checks
    ''' </summary>
    <Obsolete("FEATURE DISABLED", True)>
    Public Class LicenceInfo
        ''' <summary>
        ''' Returns whether support is provided for the passed licence model
        ''' </summary>
        ''' <param name="model"></param>
        Public Shared Function IsLicenceModelWithSupport(ByVal model As LicenceData.LicenseModel) As Boolean
            Return False
        End Function

    End Class

    ''' <summary>
    ''' Validation result data of the product registration service
    ''' </summary>
    <Obsolete("FEATURE DISABLED", True)>
    Public Class InstanceValidationResult
        ''' <summary>
        ''' Date when the support and maintenance contract expires
        ''' </summary>
        Public SupportContractExpirationDate As DateTime
        ''' <summary>
        ''' Date when the update contract expires
        ''' </summary>
        Public UpdateContractExpirationDate As DateTime
        ''' <summary>
        ''' The license detail data
        ''' </summary>
        Public LicenceData As LicenceData
        ''' <summary>
        ''' Time stamp for this result
        ''' </summary>
        Public ResponseTime As DateTime
        ''' <summary>
        ''' A hash to verify correct transmission of this result data
        ''' </summary>
        Public ValidationHash As Byte()
        ''' <summary>
        ''' General update available
        ''' </summary>
        Public UpdateAvailable As Boolean
        ''' <summary>
        ''' Security update available - strongly recommended to update ASAP
        ''' </summary>
        Public SecurityUpdateAvailable As Boolean
        ''' <summary>
        ''' Automatic license check or manual license check
        ''' </summary>
        Public ManualLicenseRevalidationOnly As Boolean
        ''' <summary>
        ''' The format version of this class
        ''' </summary>
        Public TransmissionPacketVersion As Integer = 1
        ''' <summary>
        ''' Disables publishing of license usage statistics
        ''' </summary>
        Public NoAutomaticLicenseUsageStatisticsPublishing As Boolean
        ''' <summary>
        ''' A hidden key for communication between instance and license server
        ''' </summary>
        Public KeyLicenseUsagePublishing As Byte()

        ''' <summary>
        ''' Serialize all data into a string
        ''' </summary>
        ''' <returns></returns>
        <Obsolete("FEATURE DISABLED", True)>
        Public Overrides Function ToString() As String
            Return Nothing
        End Function

        ''' <summary>
        ''' Deserialize all data from a string and create a new instance of this class
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        <Obsolete("FEATURE DISABLED", True)>
        Public Shared Function CreateInstanceFromString(data As String) As InstanceValidationResult
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' The service client which contacts the server to ask whether licence etc. are still valid
    ''' </summary>  
    <Obsolete("FEATURE DISABLED", True)>
    Public Class ProductRegistrationClient
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol

        Private info As CwmInstallationInfo

        Public Sub New(ByVal installationInfo As CwmInstallationInfo)
            MyBase.New()
            Me.info = installationInfo
        End Sub

        ''' <summary>
        ''' Lookup the version of the product registration server service
        ''' </summary>
        Public Function ServiceVersion() As Integer
            Return Integer.MaxValue
        End Function

        ''' <summary>
        ''' Send the validation data to the product registration server service and recieve its result
        ''' </summary>
        ''' <remarks>NEVER RUN THIS METHOD DIRECTLY - ONLY INTENDED FOR INTERNAL REFLECTION BY System.Web.Services</remarks>
        <Obsolete("FEATURE DISABLED", True)>
        Public Function ValidateInstallation(info As CwmInstallationInfo) As InstanceValidationResult
            Return Nothing
        End Function

        ''' <summary>
        ''' Send the validation data to the product registration server service and recieve its result
        ''' </summary>
        <Obsolete("FEATURE DISABLED", True)>
        Public Function ValidateInstallationHttpsOrHttp() As InstanceValidationResult
            Return Nothing
        End Function

    End Class

    ''' <summary>
    ''' Client logic for the product registration service 
    ''' </summary>
    <Obsolete("FEATURE DISABLED", True)>
    Public Class ProductRegistration

        Public Const CacheKeyLastRegistrationUpdate As String = "LastFetchFromServer"
        Public Const CacheKeyServerIsDown As String = "RegistrationServerOffline"
        Public Const CacheKeyManualLicenseRevalidationOnly As String = "ManualLicenseRevalidationOnly"

        Private cammWebManger As WMSystem

        ''' <summary>
        ''' Create the required information data package to access the licensing server
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateLicenseInfoTransmissionRequestData() As String
            Dim factory As New CwmInstallationInfoFactory(Me.cammWebManger)
            Dim ResultData As String = factory.CollectInstallationInfo().ToString
            Return ResultData
        End Function

        ''' <summary>
        ''' Returns whether it's time to fetch validation data from server
        ''' </summary>
        ''' <param name="hours">specifies how many hours must have passed since the last check</param>
        Friend Function IsRefreshFromRemoteLicenseServerRequired(ByVal hours As Integer) As Boolean
            Return False
        End Function

        Private Sub SaveDate2GlobalPropertiesDbTable(ByVal key As String, ByVal datevalue As DateTime)

            Dim sql As String = "UPDATE System_GlobalProperties SET ValueDateTime = @date WHERE PropertyName = @propertyname AND ValueNVarchar = N'camm WebManager' " &
                    "IF @@ROWCOUNT = 0 " &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarchar, ValueDateTime) VALUES(@propertyname, 'camm WebManager', @date) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = datevalue
            cmd.Parameters.Add("@propertyname", SqlDbType.NVarChar).Value = key
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        Private Sub SaveLastRefreshDate(ByVal refreshDate As DateTime)
            CachedLastRefreshDate = refreshDate
            SaveDate2GlobalPropertiesDbTable(CacheKeyLastRegistrationUpdate, refreshDate)
        End Sub

        Private _CachedLastRefreshDate As DateTime
        Private Property CachedLastRefreshDate As DateTime
            Get
                If HttpContext.Current Is Nothing Then
                    Return _CachedLastRefreshDate
                Else
                    Return CType(HttpContext.Current.Application(CacheKeyLastRegistrationUpdate), DateTime)
                End If
            End Get
            Set(value As DateTime)
                If HttpContext.Current Is Nothing Then
                    _CachedLastRefreshDate = value
                Else
                    HttpContext.Current.Application(CacheKeyLastRegistrationUpdate) = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Fetches data from the licence server and saves it into SystemGlobalProperties database table
        ''' </summary>
        <Obsolete("FEATURE DISABLED", True)>
        Public Sub RefreshValidationDataFromRemoteLicenseServer(throwExceptions As Boolean)
        End Sub

        ''' <summary>
        ''' Assign text data from the licence server and saves it into SystemGlobalProperties database table
        ''' </summary>
        Public Sub RefreshValidationDataManuallyFromLicenseDataFromLicenseServer(licenseData As String)
            SaveLastRefreshDate(DateTime.Now)
        End Sub

        ''' <summary>
        ''' Load validation result data from cache data in SystemGlobalProperties database table
        ''' </summary>
        Public Function GetCachedValidationResult() As InstanceValidationResult
            Return Nothing
        End Function

        Public Sub New(ByVal cammWebManger As WMSystem)
            Me.cammWebManger = cammWebManger
        End Sub

    End Class

End Namespace
