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
                If Me.VersionWebCron = Nothing Then
                    Return New Version()
                Else
                    Return New Version(Me.VersionWebCron)
                End If
            End Get
            Set(value As Version)
                If value Is Nothing Then
                    Me.VersionWebCron = Nothing
                Else
                    Me.VersionWebCron = value.ToString
                End If
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
            Dim Result As New System.Text.StringBuilder
            Result.Append("instanceId=" & System.Web.HttpUtility.UrlEncode(Me.instanceId))
            'Result.Append("&version=" & System.Web.HttpUtility.UrlEncode(Me.version.ToString))
            Result.Append("&VersionAssembly=" & System.Web.HttpUtility.UrlEncode(Me.VersionAssembly))
            'Result.Append("&databaseVersion=" & System.Web.HttpUtility.UrlEncode(Me.databaseVersion.ToString))
            Result.Append("&VersionDatabase=" & System.Web.HttpUtility.UrlEncode(Me.VersionDatabase))
            Result.Append("&VersionWebCron=" & System.Web.HttpUtility.UrlEncode(Me.VersionWebCron))
            'Result.Append("&webCronVersion=" & System.Web.HttpUtility.UrlEncode(Me.webCronVersion.ToString))
            Result.Append("&WebCronLastExecution=" & System.Web.HttpUtility.UrlEncode(Me.WebCronLastExecution.ToString("yyyy-MM-dd HH:mm:ss")))
            Result.Append("&appInstancesCount=" & System.Web.HttpUtility.UrlEncode(Me.appInstancesCount.ToString))
            Result.Append("&serverGroupsCount=" & System.Web.HttpUtility.UrlEncode(Me.serverGroupsCount.ToString))
            Result.Append("&serversCount=" & System.Web.HttpUtility.UrlEncode(Me.serversCount.ToString))
            Result.Append("&enginesCount=" & System.Web.HttpUtility.UrlEncode(Me.enginesCount.ToString))
            Result.Append("&securityObjectsCount=" & System.Web.HttpUtility.UrlEncode(Me.securityObjectsCount.ToString))
            Result.Append("&groupsCount=" & System.Web.HttpUtility.UrlEncode(Me.groupsCount.ToString))
            Result.Append("&membershipAssignmentsCount=" & System.Web.HttpUtility.UrlEncode(Me.membershipAssignmentsCount.ToString))
            Result.Append("&usersCountTotal=" & System.Web.HttpUtility.UrlEncode(Me.usersCountTotal.ToString))
            Result.Append("&usersCountPerServerGroup=" & System.Web.HttpUtility.UrlEncode(ProductRegistrationUtils.ConvertIntegersToString(Me.usersCountPerServerGroup)))
            Result.Append("&AuthsDirectlyCount=" & System.Web.HttpUtility.UrlEncode(Me.AuthsDirectlyCount.ToString))
            Result.Append("&AuthsIndirectlyCount=" & System.Web.HttpUtility.UrlEncode(Me.AuthsIndirectlyCount.ToString))
            Result.Append("&GroupAuthsDirectlyCount=" & System.Web.HttpUtility.UrlEncode(Me.GroupAuthsDirectlyCount.ToString))
            Result.Append("&supervisorsCount=" & System.Web.HttpUtility.UrlEncode(Me.supervisorsCount.ToString))
            Result.Append("&securityAdministratorsCount=" & System.Web.HttpUtility.UrlEncode(Me.securityAdministratorsCount.ToString))
            Result.Append("&securityRelatedContactsCount=" & System.Web.HttpUtility.UrlEncode(Me.securityRelatedContactsCount.ToString))
            Result.Append("&dataProtectionCoordinatorsCount=" & System.Web.HttpUtility.UrlEncode(Me.dataProtectionCoordinatorsCount.ToString))
            Result.Append("&securityAccessEverythingCount=" & System.Web.HttpUtility.UrlEncode(Me.securityAccessEverythingCount.ToString))
            Result.Append("&ActiveMarketsCount=" & System.Web.HttpUtility.UrlEncode(Me.ActiveMarketsCount.ToString))
            Result.Append("&RequestTime=" & System.Web.HttpUtility.UrlEncode(Me.RequestTime.ToString("yyyy-MM-dd HH:mm:ss")))
            Result.Append("&ValidationHash=" & System.Web.HttpUtility.UrlEncode(ProductRegistrationUtils.ConvertBytesToString(Me.ValidationHash)))
            Result.Append("&WebAppInstanceID=" & System.Web.HttpUtility.UrlEncode(Me.WebAppInstanceID))
            Result.Append("&UsersVerifiedCount=" & System.Web.HttpUtility.UrlEncode(Me.UsersVerifiedCount.ToString))
            Result.Append("&ProcessedAuthentificationsBy1FactorCount=" & System.Web.HttpUtility.UrlEncode(Me.ProcessedAuthentificationsBy1FactorCount.ToString))
            Result.Append("&ProcessedAuthentificationsBy2FactorsCount=" & System.Web.HttpUtility.UrlEncode(Me.ProcessedAuthentificationsBy2FactorsCount.ToString))
            Result.Append("&ProcessedAuthentificationsByExternalAccountCount=" & System.Web.HttpUtility.UrlEncode(Me.ProcessedAuthentificationsByExternalAccountCount.ToString))
            Result.Append("&TransmissionPacketVersion=" & System.Web.HttpUtility.UrlEncode(Me.TransmissionPacketVersion.ToString))
            Result.Append("&KeyLicenseUsagePublishing=" & System.Web.HttpUtility.UrlEncode(ProductRegistrationUtils.ConvertBytesToString(Me.KeyLicenseUsagePublishing)))
            Return Result.ToString()
        End Function

        ''' <summary>
        ''' Deserialize all data from a string and create a new instance of this class
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Shared Function CreateInstanceFromString(data As String) As CwmInstallationInfo
            If data Is Nothing Then
                Return Nothing
            Else
                Dim dcoll As System.Collections.Specialized.NameValueCollection = System.Web.HttpUtility.ParseQueryString(data)
                Dim Result As New CwmInstallationInfo
                Result.instanceId = dcoll("instanceId")
                Result.VersionAssembly = dcoll("VersionAssembly")
                Result.VersionDatabase = dcoll("VersionDatabase")
                Result.VersionWebCron = dcoll("VersionWebCron")
                Result.WebCronLastExecution = ProductRegistrationUtils.DateTimeParse(dcoll("WebCronLastExecution"))
                Result.appInstancesCount = ProductRegistrationUtils.IntegerParse(dcoll("appInstancesCount"))
                Result.serverGroupsCount = ProductRegistrationUtils.IntegerParse(dcoll("serverGroupsCount"))
                Result.serversCount = ProductRegistrationUtils.IntegerParse(dcoll("serversCount"))
                Result.enginesCount = ProductRegistrationUtils.IntegerParse(dcoll("enginesCount"))
                Result.securityObjectsCount = ProductRegistrationUtils.LongParse(dcoll("securityObjectsCount"))
                Result.groupsCount = ProductRegistrationUtils.LongParse(dcoll("groupsCount"))
                Result.membershipAssignmentsCount = ProductRegistrationUtils.LongParse(dcoll("membershipAssignmentsCount"))
                Result.usersCountTotal = ProductRegistrationUtils.LongParse(dcoll("usersCountTotal"))
                Result.usersCountPerServerGroup = ProductRegistrationUtils.ConvertIntegerStringToIntegers(dcoll("usersCountPerServerGroup"))
                Result.AuthsDirectlyCount = ProductRegistrationUtils.LongParse(dcoll("AuthsDirectlyCount"))
                Result.AuthsIndirectlyCount = ProductRegistrationUtils.LongParse(dcoll("AuthsIndirectlyCount"))
                Result.GroupAuthsDirectlyCount = ProductRegistrationUtils.LongParse(dcoll("GroupAuthsDirectlyCount"))
                Result.supervisorsCount = ProductRegistrationUtils.IntegerParse(dcoll("supervisorsCount"))
                Result.securityAdministratorsCount = ProductRegistrationUtils.IntegerParse(dcoll("securityAdministratorsCount"))
                Result.securityRelatedContactsCount = ProductRegistrationUtils.IntegerParse(dcoll("securityRelatedContactsCount"))
                Result.dataProtectionCoordinatorsCount = ProductRegistrationUtils.IntegerParse(dcoll("dataProtectionCoordinatorsCount"))
                Result.securityAccessEverythingCount = ProductRegistrationUtils.IntegerParse(dcoll("securityAccessEverythingCount"))
                Result.ActiveMarketsCount = ProductRegistrationUtils.IntegerParse(dcoll("ActiveMarketsCount"))
                Result.RequestTime = ProductRegistrationUtils.DateTimeParse(dcoll("RequestTime"))
                Result.ValidationHash = ProductRegistrationUtils.ConvertByteStringToBytes(dcoll("ValidationHash"))
                Result.WebAppInstanceID = dcoll("WebAppInstanceID")
                Result.UsersVerifiedCount = ProductRegistrationUtils.LongParse(dcoll("UsersVerifiedCount"))
                Result.ProcessedAuthentificationsBy1FactorCount = ProductRegistrationUtils.LongParse(dcoll("ProcessedAuthentificationsBy1FactorCount"))
                Result.ProcessedAuthentificationsBy2FactorsCount = ProductRegistrationUtils.LongParse(dcoll("ProcessedAuthentificationsBy2FactorsCount"))
                Result.ProcessedAuthentificationsByExternalAccountCount = ProductRegistrationUtils.LongParse(dcoll("ProcessedAuthentificationsByExternalAccountCount"))
                Result.TransmissionPacketVersion = ProductRegistrationUtils.IntegerParse(dcoll("TransmissionPacketVersion"))
                Result.KeyLicenseUsagePublishing = ProductRegistrationUtils.ConvertByteStringToBytes(dcoll("KeyLicenseUsagePublishing"))
                Return Result
            End If
        End Function

        ''' <summary>
        ''' Sign the transaction data and validate its completeness
        ''' </summary>
        Friend Sub SignData()
            Me.ValidationHash = GenerateTokenForInstallationInfo(Me)
            If ValidateInstallationInfo(Me) = False Then Throw New Exception("Invalid/incomplete data found")
        End Sub

        ''' <summary>
        ''' Perform some logical checks on data
        ''' </summary>
        ''' <param name="info"></param>
        Friend Shared Function ValidateInstallationInfo(ByVal info As CwmInstallationInfo) As Boolean
            Dim ValidationLevel As Byte = 0 'ValidationLevel: 0 for full validation by client with throwing exceptions, 1 for basic checks at server and without throwing of exceptions
#If ProdRegServiceCodeLocationIsClient <> True Then
            ValidationLevel = 1
#End If
            If info.instanceId = Nothing Then
                If ValidationLevel = 0 Then Throw New Exception("Missing instance ID") Else Return False
            ElseIf info.RequestTime = Nothing Then
                If ValidationLevel = 0 Then Throw New Exception("Missing RequestTime") Else Return False
            ElseIf ValidationLevel = 0 AndAlso info.VersionAssembly = Nothing Then
                If ValidationLevel = 0 Then Throw New Exception("Missing VersionAssembly") Else Return False
            ElseIf ValidationLevel = 0 AndAlso info.VersionDatabase = Nothing Then
                If ValidationLevel = 0 Then Throw New Exception("Missing VersionDatabase") Else Return False
            ElseIf info.ValidationHash Is Nothing OrElse info.ValidationHash.Length = 0 Then
                If ValidationLevel = 0 Then Throw New Exception("Missing ValidationHash") Else Return False
            ElseIf info.ActiveMarketsCount = 0 Then
                If ValidationLevel = 0 Then Throw New Exception("Missing ActiveMarketsCount") Else Return False
            ElseIf info.serverGroupsCount = 0 Then
                If ValidationLevel = 0 Then Throw New Exception("Missing serverGroupsCount") Else Return False
            ElseIf info.serversCount = 0 Then
                If ValidationLevel = 0 Then Throw New Exception("Missing serversCount") Else Return False
            ElseIf ValidationLevel = 0 AndAlso info.usersCountPerServerGroup Is Nothing OrElse info.usersCountPerServerGroup.Length <> info.serverGroupsCount Then
                'Always validate carefully
                If ValidationLevel = 0 Then Throw New Exception("Missing or invalid usersCountPerServerGroup") Else Return False
            ElseIf ValidationLevel = 1 AndAlso info.usersCountPerServerGroup Is Nothing Then
                'Always accept all data
                info.usersCountPerServerGroup = New Integer() {}
#If NetFrameWork <> "1_1" Then
            ElseIf info.version >= New Version(4, 10, 206) AndAlso info.AuthsIndirectlyCount = 0 Then 'can never be 0 even in fresh setup instances since supervisors are already present and authoirized
                If ValidationLevel = 0 Then Throw New Exception("Missing AuthsIndirectlyCount") Else Return False
            ElseIf info.version >= New Version(4, 10, 206) AndAlso info.GroupAuthsDirectlyCount = 0 Then 'can never be 0 even in fresh setup instances since supervisors are already present and authoirized
                If ValidationLevel = 0 Then Throw New Exception("Missing GroupAuthsDirectlyCount") Else Return False
            ElseIf info.version >= New Version(4, 10, 206) AndAlso info.WebAppInstanceID = "" Then
                If ValidationLevel = 0 Then Throw New Exception("Missing WebAppInstanceID") Else Return False
#End If
            Else
                Return True
            End If
        End Function

        ''' <summary>
        ''' Create the signing token
        ''' </summary>
        ''' <param name="info"></param>
        Friend Shared Function GenerateTokenForInstallationInfo(ByVal info As CwmInstallationInfo) As Byte()
            Dim hmac As New System.Security.Cryptography.HMACSHA1(New Byte() {62, 2, 42, 75, 222, 54, 200, 199, 20, 12})
            Dim memoryStream As New System.IO.MemoryStream
            Dim binaryWriter As New System.IO.BinaryWriter(memoryStream)
            'Most current interface standard
            binaryWriter.Write(info.appInstancesCount)
            binaryWriter.Write("/" & info.databaseVersion.ToString())
            binaryWriter.Write("/" & info.dataProtectionCoordinatorsCount)
            'TODO: add info.securityAccessEverythingCount as soon as version no. for interface can be fixed - into separate info.version-Check-Code-Block
            'binaryWriter.Write("/" & info.securityAccessEverythingCount)
            binaryWriter.Write("/" & info.enginesCount)
            binaryWriter.Write("/" & info.groupsCount)
            binaryWriter.Write("/" & info.instanceId)
            binaryWriter.Write("/" & info.membershipAssignmentsCount)
            binaryWriter.Write("/" & info.securityAdministratorsCount)
            binaryWriter.Write("/" & info.securityObjectsCount)
            binaryWriter.Write("/" & info.securityRelatedContactsCount)
            binaryWriter.Write("/" & info.serverGroupsCount)
            binaryWriter.Write("/" & info.serversCount)
            binaryWriter.Write("/" & info.ActiveMarketsCount)
            binaryWriter.Write("/" & info.AuthsDirectlyCount)
            binaryWriter.Write("/" & info.AuthsIndirectlyCount)
            binaryWriter.Write("/" & info.GroupAuthsDirectlyCount)
            binaryWriter.Write("/" & info.supervisorsCount)
            binaryWriter.Write("/" & info.usersCountTotal)
            binaryWriter.Write("/" & info.version.ToString())
            binaryWriter.Write("/" & info.RequestTime.Ticks)
            binaryWriter.Write("/" & info.WebAppInstanceID)
            Dim result As Byte() = hmac.ComputeHash(memoryStream)
            binaryWriter.Close()
            memoryStream.Close()
            Return result
        End Function
    End Class

    ''' <summary>
    ''' Create instance of CwmInstallationInfo
    ''' </summary>
    Friend Class CwmInstallationInfoFactory

        Private cammWebManger As WMSystem
        Public Sub New(cwm As WMSystem)
            Me.cammWebManger = cwm
        End Sub

        ''' <summary>
        ''' Lookup the number of users per server group
        ''' </summary>
        ''' <param name="serversGroupsCount"></param>
        Private Function GetCountPerServerGroup(ByVal serversGroupsCount As Integer) As Integer()
            Dim Sql As String = "SELECT COUNT(Benutzer.ID) " & vbNewLine &
                "FROM Benutzer " & vbNewLine &
                "    RIGHT JOIN [dbo].[System_ServerGroupsAndTheirUserAccessLevels] ON Benutzer.AccountAccessability = [dbo].[System_ServerGroupsAndTheirUserAccessLevels].ID_AccessLevel " & vbNewLine &
                "    RIGHT JOIN System_ServerGroups ON System_ServerGroups.ID = [dbo].[System_ServerGroupsAndTheirUserAccessLevels].ID_ServerGroup " & vbNewLine &
                "GROUP BY System_ServerGroups.ID " & vbNewLine &
                "ORDER BY System_ServerGroups.ID"
            Dim cmd As New SqlClient.SqlCommand(Sql, New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString))
            cmd.CommandType = CommandType.Text
            Dim result As ArrayList
            result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            Return CType(result.ToArray(GetType(Integer)), Integer())
        End Function

        ''' <summary>
        ''' Query from db command without automatic changes on connection status
        ''' </summary>
        ''' <param name="cmd"></param>
        Private Function GetDatetimeNzResult(ByVal cmd As IDbCommand) As DateTime
            Return Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), New DateTime())
        End Function

        ''' <summary>
        ''' Query from db command without automatic changes on connection status
        ''' </summary>
        ''' <param name="cmd"></param>
        Private Function GetStringNzResult(ByVal cmd As IDbCommand) As String
            Return Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), CType(Nothing, String))
        End Function

        ''' <summary>
        ''' Query from db command without automatic changes on connection status
        ''' </summary>
        ''' <param name="cmd"></param>
        Private Function GetIntegerResult(ByVal cmd As IDbCommand) As Integer
            Return CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), Integer)
        End Function

        ''' <summary>
        ''' Query from db command without automatic changes on connection status
        ''' </summary>
        ''' <param name="cmd"></param>
        Private Function GetLongResult(ByVal cmd As IDbCommand) As Long
            Return CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None), Long)
        End Function

        ''' <summary>
        ''' Collect statistics and information on the current camm Web-Manager instance
        ''' </summary>
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

                cmd.CommandText = "Select count(*) As AppInstancesCount FROM system_globalproperties WHERE propertyname Like 'AppInstance_%' and valuenvarchar = N'camm WebManager' and valuedatetime > dateadd(mm, -1, getdate()) and propertyname not like 'AppInstance_{ERROR detecting AssemblyLocation: %}' and ValueNText not like 'ServerIdentString={not yet configured at execution time}%Request={Nothing}' and ValueNText not like 'Error while looking up current assembly location'"
                result.appInstancesCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT count(ID) FROM System_ServerGroups"
                result.serverGroupsCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM System_Servers"
                result.serversCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM [System_Languages] WHERE IsActive <> 0"
                result.ActiveMarketsCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM System_WebAreaScriptEnginesAuthorization"
                result.enginesCount = GetIntegerResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM dbo.[Applications_CurrentAndInactiveOnes]"
                result.securityObjectsCount = GetLongResult(cmd)

                cmd.CommandText = "SELECT COUNT(ID) FROM Gruppen"
                result.groupsCount = GetLongResult(cmd)

                cmd.CommandText = "SELECT count(*) as MembershipAssignmentsCount from memberships"
                result.membershipAssignmentsCount = GetLongResult(cmd)

                cmd.CommandText = "SELECT count(*) as AuthsCount from [ApplicationsRightsByUser]"
                result.AuthsDirectlyCount = GetLongResult(cmd)

                If Setup.DatabaseUtils.Version(Me.cammWebManger, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) < 0 Then 'Older
                    Dim Sql As String = "SELECT count(*) AS AuthsCount " & vbNewLine &
                        "FROM (" & vbNewLine &
                        "	SELECT ID_Application, ID_User" & vbNewLine &
                        "	FROM [view_ApplicationRights_CommulatedPerUser_FAST]" & vbNewLine &
                        "	GROUP BY ID_Application, ID_User" & vbNewLine &
                        "	) AS UniqueAuths"
                    cmd.CommandText = Sql
                    result.AuthsIndirectlyCount = GetLongResult(cmd)
                Else
                    Dim Sql As String = "SELECT count(*) AS AuthsCount " & vbNewLine &
                        "FROM (" & vbNewLine &
                        "	SELECT ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule" & vbNewLine &
                        "	FROM [ApplicationsRightsByUser_EffectiveCumulative]" & vbNewLine &
                        "	GROUP BY ID_SecurityObject, ID_ServerGroup, ID_User, IsDevRule" & vbNewLine &
                        "	) AS UniqueAuths"
                    cmd.CommandText = Sql
                    result.AuthsIndirectlyCount = GetLongResult(cmd)
                End If

                cmd.CommandText = "SELECT count(*) as AuthsCount from [ApplicationsRightsByGroup]"
                result.GroupAuthsDirectlyCount = GetLongResult(cmd)

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

                cmd.CommandText = "Select COUNT(ID) FROM dbo.Memberships WHERE ID_GROUP = " & CompuMaster.camm.WebManager.WMSystem.SpecialGroups.Group_SecurityAccessEverything
                result.securityAccessEverythingCount = GetIntegerResult(cmd)

                result.WebAppInstanceID = cammWebManger.CurrentWebAppInstanceID

                cmd.CommandText = "SELECT ValueDateTime FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LastWebServiceExecutionDate' AND ValueNVarChar = N'camm WebManager'"
                result.WebCronLastExecution = GetDatetimeNzResult(cmd)
                If result.WebCronLastExecution = Nothing Then
                    'no full webcron activated, watch for last mail processing meaning version 1 or 4.10 of CWM WebCron (no)trigger service (or JIT mail processing without service) 
                    cmd.CommandText = "SELECT ValueDateTime FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LastMailQueueProcessing' AND ValueNVarChar = N'camm WebManager'"
                    result.WebCronLastExecution = GetDatetimeNzResult(cmd)
                    result.webCronVersion = New Version(4, 10, 0, 0)
                Else
                    'try to find a version info from trigger service
                    cmd.CommandText = "SELECT ValueNText FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LastWebServiceExecutionDate' AND ValueNVarChar = N'camm WebManager'"
                    Dim versionString As String = GetStringNzResult(cmd)
                    If versionString = Nothing Then
                        result.webCronVersion = Nothing
                    Else
                        result.webCronVersion = New Version(versionString)
                    End If
                End If

                result.usersCountPerServerGroup = GetCountPerServerGroup(result.serverGroupsCount)
                result.RequestTime = DateTime.Now.ToUniversalTime()
                result.SignData() 'Sign and validate the completeness of the data
                Return result

            Finally
                If Not cmd Is Nothing Then
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(cmd.Connection)
                End If
            End Try

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
            Dim Result As New System.Text.StringBuilder
            Result.Append("Model=" & System.Web.HttpUtility.UrlEncode(CType(Me.Model, Integer).ToString))
            Result.Append("&Type=" & System.Web.HttpUtility.UrlEncode(CType(Me.Type, Integer).ToString))
            Result.Append("&ExpirationDate=" & System.Web.HttpUtility.UrlEncode(Me.ExpirationDate.ToString("yyyy-MM-dd HH:mm:ss")))
            Return Result.ToString()
        End Function

        ''' <summary>
        ''' Deserialize all data from a string and create a new instance of this class
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Shared Function CreateInstanceFromString(data As String) As LicenceData
            If data Is Nothing Then
                Return Nothing
            Else
                Dim dcoll As System.Collections.Specialized.NameValueCollection = System.Web.HttpUtility.ParseQueryString(data)
                Dim Result As New LicenceData
                Result.Model = CType(Byte.Parse(dcoll("Model")), LicenseModel)
                Result.Type = CType(Byte.Parse(dcoll("Type")), LicenceType)
                Result.ExpirationDate = ProductRegistrationUtils.DateTimeParse(dcoll("ExpirationDate"))
                Return Result
            End If
        End Function

    End Class

    ''' <summary>
    ''' License checks
    ''' </summary>
    Public Class LicenceInfo
        ''' <summary>
        ''' Returns whether support is provided for the passed licence model
        ''' </summary>
        ''' <param name="model"></param>
        Public Shared Function IsLicenceModelWithSupport(ByVal model As LicenceData.LicenseModel) As Boolean
            Return Not (model = LicenceData.LicenseModel.Community OrElse model = LicenceData.LicenseModel.Demo OrElse model = LicenceData.LicenseModel.Trial OrElse model = LicenceData.LicenseModel.None)
        End Function

        ''' <summary>
        ''' Returns whether the licence model is one that can expire
        ''' </summary>
        ''' <param name="model"></param>
        Friend Shared Function IsExpiringLicenceModel(ByVal model As LicenceData.LicenseModel) As Boolean
            Return Not (model = LicenceData.LicenseModel.Demo OrElse model = LicenceData.LicenseModel.Community OrElse model = LicenceData.LicenseModel.None)
        End Function
    End Class

    ''' <summary>
    ''' Validation result data of the product registration service
    ''' </summary>
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

        Friend Sub New()
            Me.LicenceData = New LicenceData()
            Me.LicenceData.Model = LicenceData.LicenseModel.Community
        End Sub

        Friend Sub New(licenseModel As LicenceData.LicenseModel)
            Me.LicenceData = New LicenceData()
            Me.LicenceData.Model = licenseModel
        End Sub

        ''' <summary>
        ''' Serialize all data into a string
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim Result As New System.Text.StringBuilder
            Result.Append("SupportContractExpirationDate=" & System.Web.HttpUtility.UrlEncode(Me.SupportContractExpirationDate.ToString("yyyy-MM-dd HH:mm:ss")))
            Result.Append("&UpdateContractExpirationDate=" & System.Web.HttpUtility.UrlEncode(Me.UpdateContractExpirationDate.ToString("yyyy-MM-dd HH:mm:ss")))
            Result.Append("&LicenceData=" & System.Web.HttpUtility.UrlEncode(Me.LicenceData.ToString))
            Result.Append("&ResponseTime=" & System.Web.HttpUtility.UrlEncode(Me.ResponseTime.ToString("yyyy-MM-dd HH:mm:ss")))
            Result.Append("&ValidationHash=" & System.Web.HttpUtility.UrlEncode(ProductRegistrationUtils.ConvertBytesToString(Me.ValidationHash)))
            Result.Append("&UpdateAvailable=" & System.Web.HttpUtility.UrlEncode(Me.UpdateAvailable.ToString))
            Result.Append("&SecurityUpdateAvailable=" & System.Web.HttpUtility.UrlEncode(Me.SecurityUpdateAvailable.ToString))
            Result.Append("&ManualLicenseRevalidationOnly=" & System.Web.HttpUtility.UrlEncode(Me.ManualLicenseRevalidationOnly.ToString))
            Result.Append("&TransmissionPacketVersion=" & System.Web.HttpUtility.UrlEncode(Me.TransmissionPacketVersion.ToString))
            Result.Append("&NoAutomaticLicenseUsageStatisticsPublishing=" & System.Web.HttpUtility.UrlEncode(Me.NoAutomaticLicenseUsageStatisticsPublishing.ToString))
            Result.Append("&KeyLicenseUsagePublishing=" & System.Web.HttpUtility.UrlEncode(ProductRegistrationUtils.ConvertBytesToString(Me.KeyLicenseUsagePublishing)))
            Return Result.ToString()
        End Function

        ''' <summary>
        ''' Deserialize all data from a string and create a new instance of this class
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Shared Function CreateInstanceFromString(data As String) As InstanceValidationResult
            If data Is Nothing Then
                Return Nothing
            Else
                Dim dcoll As System.Collections.Specialized.NameValueCollection = System.Web.HttpUtility.ParseQueryString(data)
                Dim Result As New InstanceValidationResult
            Result.SupportContractExpirationDate = ProductRegistrationUtils.DateTimeParse(dcoll("SupportContractExpirationDate"))
            Result.UpdateContractExpirationDate = ProductRegistrationUtils.DateTimeParse(dcoll("UpdateContractExpirationDate"))
            Result.LicenceData = LicenceData.CreateInstanceFromString(dcoll("LicenceData"))
            Result.ResponseTime = ProductRegistrationUtils.DateTimeParse(dcoll("ResponseTime"))
            Result.ValidationHash = ProductRegistrationUtils.ConvertByteStringToBytes(dcoll("ValidationHash"))
            Result.UpdateAvailable = ProductRegistrationUtils.BooleanParse(dcoll("UpdateAvailable"))
            Result.SecurityUpdateAvailable = ProductRegistrationUtils.BooleanParse(dcoll("SecurityUpdateAvailable"))
            Result.ManualLicenseRevalidationOnly = ProductRegistrationUtils.BooleanParse(dcoll("ManualLicenseRevalidationOnly"))
            Result.TransmissionPacketVersion = ProductRegistrationUtils.IntegerParse(dcoll("TransmissionPacketVersion"))
            Result.NoAutomaticLicenseUsageStatisticsPublishing = ProductRegistrationUtils.BooleanParse(dcoll("NoAutomaticLicenseUsageStatisticsPublishing"))
            Result.KeyLicenseUsagePublishing = ProductRegistrationUtils.ConvertByteStringToBytes(dcoll("KeyLicenseUsagePublishing"))
                Return Result
            End If
        End Function

    End Class

    Friend Class ProductRegistrationUtils
        Public Shared Function DateTimeParse(value As String) As DateTime
            If value = Nothing Then
                Return Nothing
            Else
                Return DateTime.Parse(value)
            End If
        End Function
        Public Shared Function IntegerParse(value As String) As Integer
            If value = Nothing Then
                Return Nothing
            Else
                Return Integer.Parse(value)
            End If
        End Function
        Public Shared Function LongParse(value As String) As Long
            If value = Nothing Then
                Return Nothing
            Else
                Return Long.Parse(value)
            End If
        End Function
        Public Shared Function BooleanParse(value As String) As Boolean
            If value = Nothing Then
                Return Nothing
            Else
                Return Boolean.Parse(value)
            End If
        End Function
        Public Shared Function ConvertLongStringToLongs(data As String) As Long()
            If data = Nothing Then Return Nothing
            If data(0) <> "{"c OrElse data(data.Length - 1) <> "}"c Then Throw New ArgumentException("Data must be enocded in {}")
            Dim InternalData As String = data.Substring(1, data.Length - 2).Replace(" ", "")
            Dim ByteData As String() = InternalData.Split(","c)
            If ByteData.Length = 1 AndAlso ByteData(0) = "" Then Return New Long() {}
            Dim Result As New List(Of Long)
            For MyCounter As Integer = 0 To ByteData.Length - 1
                Result.Add(Long.Parse(ByteData(MyCounter)))
            Next
            Return Result.ToArray
        End Function
        Public Shared Function ConvertLongsToString(data As Long()) As String
            If data Is Nothing Then Return Nothing
            Dim Result As New System.Text.StringBuilder
            Result.Append("{")
            For MyCounter As Integer = 0 To data.Length - 1
                Result.Append(data(MyCounter).ToString)
                If MyCounter <> data.Length - 1 Then Result.Append(", ")
            Next
            Result.Append("}")
            Return Result.ToString
        End Function
        Public Shared Function ConvertIntegerStringToIntegers(data As String) As Integer()
            If data = Nothing Then Return Nothing
            If data(0) <> "{"c OrElse data(data.Length - 1) <> "}"c Then Throw New ArgumentException("Data must be enocded in {}")
            Dim InternalData As String = data.Substring(1, data.Length - 2).Replace(" ", "")
            Dim ByteData As String() = InternalData.Split(","c)
            If ByteData.Length = 1 AndAlso ByteData(0) = "" Then Return New Integer() {}
            Dim Result As New List(Of Integer)
            For MyCounter As Integer = 0 To ByteData.Length - 1
                Result.Add(Integer.Parse(ByteData(MyCounter)))
            Next
            Return Result.ToArray
        End Function
        Public Shared Function ConvertIntegersToString(data As Integer()) As String
            If data Is Nothing Then Return Nothing
            Dim Result As New System.Text.StringBuilder
            Result.Append("{")
            For MyCounter As Integer = 0 To data.Length - 1
                Result.Append(data(MyCounter).ToString)
                If MyCounter <> data.Length - 1 Then Result.Append(", ")
            Next
            Result.Append("}")
            Return Result.ToString
        End Function
        Public Shared Function ConvertByteStringToBytes(data As String) As Byte()
            If data = Nothing Then Return Nothing
            If data(0) <> "{"c OrElse data(data.Length - 1) <> "}"c Then Throw New ArgumentException("Data must be enocded in {}")
            Dim InternalData As String = data.Substring(1, data.Length - 2).Replace(" ", "")
            Dim ByteData As String() = InternalData.Split(","c)
            If ByteData.Length = 1 AndAlso ByteData(0) = "" Then Return New Byte() {}
            Dim Result As New List(Of Byte)
            For MyCounter As Integer = 0 To ByteData.Length - 1
                Result.Add(Byte.Parse(ByteData(MyCounter)))
            Next
            Return Result.ToArray
        End Function
        Public Shared Function ConvertBytesToString(data As Byte()) As String
            If data Is Nothing Then Return Nothing
            Dim Result As New System.Text.StringBuilder
            Result.Append("{")
            For MyCounter As Integer = 0 To data.Length - 1
                Result.Append(data(MyCounter).ToString)
                If MyCounter <> data.Length - 1 Then Result.Append(", ")
            Next
            Result.Append("}")
            Return Result.ToString
        End Function
    End Class

    ''' <summary>
    ''' The service client which contacts the server to ask whether licence etc. are still valid
    ''' </summary>
    <System.Web.Services.WebServiceBindingAttribute(Name:="CimRegistrationServiceClient",
                [Namespace]:="http://www.camm.biz/support/cim/services/")>
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
        ''' <param name="validationResult"></param>
        Private Shared Function GenerateTokenForValidationResult(ByVal validationResult As InstanceValidationResult) As Byte()
            Dim hmac As New System.Security.Cryptography.HMACSHA1(New Byte() {62, 2, 42, 75, 222, 54, 200, 199, 20, 12})
            Dim memoryStream As New System.IO.MemoryStream
            Dim binaryWriter As New System.IO.BinaryWriter(memoryStream)
            binaryWriter.Write(validationResult.ResponseTime.Ticks)

            If Not validationResult.SupportContractExpirationDate = Nothing Then
                binaryWriter.Write(validationResult.SupportContractExpirationDate.Ticks)
            End If
            If Not validationResult.UpdateContractExpirationDate = Nothing Then
                binaryWriter.Write(validationResult.UpdateContractExpirationDate.Ticks)
            End If
            If Not validationResult.LicenceData.ExpirationDate = Nothing Then
                binaryWriter.Write(validationResult.LicenceData.ExpirationDate.Ticks)
            End If
            binaryWriter.Write(validationResult.LicenceData.Model)
            binaryWriter.Write(validationResult.LicenceData.Type)

            Dim result As Byte() = hmac.ComputeHash(memoryStream)

            binaryWriter.Close()
            memoryStream.Close()

            Return result
        End Function

        Friend Enum ValidationRequestType As Integer
            ServiceRequestWithTTL = 0
            ManualRequest = 1
        End Enum
        ''' <summary>
        ''' Checks whether received data has been tampered with or not
        ''' </summary>
        ''' <param name="info"></param>
        Friend Shared Function VerifyValidationResult(ByVal info As InstanceValidationResult, validationType As ValidationRequestType) As Boolean
            If info.ValidationHash Is Nothing OrElse info.ValidationHash.Length = 0 Then Return False
            Dim hash As Byte() = GenerateTokenForValidationResult(info)
            Dim isValid As Boolean = BitConverter.ToString(hash) = BitConverter.ToString(info.ValidationHash)
            If isValid Then 'attempt to mitigate replay attacks
                If validationType = ValidationRequestType.ServiceRequestWithTTL Then
                    Return DateTime.Now.ToUniversalTime().Subtract(info.ResponseTime).TotalSeconds <= 300
                Else
                    Return True
                End If
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Lookup the version of the product registration server service
        ''' </summary>
        <System.Diagnostics.DebuggerStepThrough(), System.Web.Services.Protocols.SoapDocumentMethodAttribute(
            "http://www.camm.biz/support/cim/services/ServiceVersion",
            RequestNamespace:="http://www.camm.biz/support/cim/services/",
            ResponseNamespace:="http://www.camm.biz/support/cim/services/",
            Use:=System.Web.Services.Description.SoapBindingUse.Literal,
            ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Default)>
        Public Function ServiceVersion() As Integer
            Dim result() As Object = Nothing
            Try
                Me.Url = "https://www.camm.biz/support/cim/services/registration.asmx"
                result = Me.Invoke("ServiceVersion", New Object() {})
            Catch
                Me.Url = "http://www.camm.biz/support/cim/services/registration.asmx"
                result = Me.Invoke("ServiceVersion", New Object() {})
            End Try
            Return CType(result(0), Integer)
        End Function

        ''' <summary>
        ''' Lookup the version of the product registration server service - only for diagnostics by test library: HTTPS access to product registration server service
        ''' </summary>
        Friend Function ServiceVersionHttps() As Integer
            Dim result() As Object = Nothing
            Me.Url = "https://www.camm.biz/support/cim/services/registration.asmx"
            result = Me.Invoke("ServiceVersion", New Object() {})
            Return CType(result(0), Integer)
        End Function
        ''' <summary>
        ''' Lookup the version of the product registration server service - only for diagnostics by test library: HTTP access to product registration server service
        ''' </summary>
        Friend Function ServiceVersionHttp() As Integer
            Dim result() As Object = Nothing
            Me.Url = "http://www.camm.biz/support/cim/services/registration.asmx"
            result = Me.Invoke("ServiceVersion", New Object() {})
            Return CType(result(0), Integer)
        End Function

        ''' <summary>
        ''' Always send instance ID with every webservice request
        ''' </summary>
        ''' <param name="uri"></param>
        Protected Overrides Function GetWebRequest(ByVal uri As Uri) As System.Net.WebRequest
            Dim original As System.Net.WebRequest = MyBase.GetWebRequest(uri)
            original.Headers.Add("X-CWM-InstanceID", Me.info.instanceId)
            original.Timeout = 5 * 1000
            Return original
        End Function

        ''' <summary>
        ''' Send the validation data to the product registration server service and recieve its result
        ''' </summary>
        ''' <remarks>NEVER RUN THIS METHOD DIRECTLY - ONLY INTENDED FOR INTERNAL REFLECTION BY System.Web.Services</remarks>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never),
            System.Web.Services.Protocols.SoapDocumentMethodAttribute(
            "http://www.camm.biz/support/cim/services/ValidateInstallation",
            RequestNamespace:="http://www.camm.biz/support/cim/services/",
            ResponseNamespace:="http://www.camm.biz/support/cim/services/",
            Use:=System.Web.Services.Description.SoapBindingUse.Literal,
            ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Default)>
        Public Function ValidateInstallation(info As CwmInstallationInfo) As InstanceValidationResult
            Me.Invoke("ValidateInstallation", New Object() {info})
            Return Nothing
        End Function


        ''' <summary>
        ''' Send the validation data to the product registration server service and recieve its result
        ''' </summary>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute(
            "http://www.camm.biz/support/cim/services/ValidateInstallation",
            RequestNamespace:="http://www.camm.biz/support/cim/services/",
            ResponseNamespace:="http://www.camm.biz/support/cim/services/",
            Use:=System.Web.Services.Description.SoapBindingUse.Literal,
            ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Default)>
        Public Function ValidateInstallationHttpsOrHttp() As InstanceValidationResult
            If info Is Nothing Then Throw New NullReferenceException("info must contain a value")
            Dim result() As Object = Nothing
            Try
                Me.Url = "https://www.camm.biz/support/cim/services/registration.asmx"
                result = Me.Invoke("ValidateInstallation", New Object() {info})
            Catch
                Me.Url = "http://www.camm.biz/support/cim/services/registration.asmx"
                result = Me.Invoke("ValidateInstallation", New Object() {info})
            End Try

            Dim contractInfo As InstanceValidationResult = CType(result(0), InstanceValidationResult)
            If VerifyValidationResult(contractInfo, ValidationRequestType.ServiceRequestWithTTL) Then
                Return contractInfo
            End If
            Throw New Exception("Response manipulated during transfer or response took too long")
        End Function

        ''' <summary>
        ''' Send the validation data to the product registration server service and recieve its result - only for diagnostics by test library: HTTP access to product registration server service
        ''' </summary>
        Friend Function ValidateInstallationHttp() As InstanceValidationResult
            If info Is Nothing Then Throw New NullReferenceException("info must contain a value")
            Dim result() As Object = Nothing
            Me.Url = "http://www.camm.biz/support/cim/services/registration.asmx"
            result = Me.Invoke("ValidateInstallation", New Object() {info})

            Dim contractInfo As InstanceValidationResult = CType(result(0), InstanceValidationResult)
            If VerifyValidationResult(contractInfo, ValidationRequestType.ServiceRequestWithTTL) Then
                Return contractInfo
            End If
            Throw New Exception("Response manipulated during transfer or response took too long")
        End Function

        ''' <summary>
        ''' Send the validation data to the product registration server service and recieve its result - only for diagnostics by test library: HTTP access to product registration server service
        ''' </summary>
        Friend Function ValidateInstallationHttps() As InstanceValidationResult
            If info Is Nothing Then Throw New NullReferenceException("info must contain a value")
            Dim result() As Object = Nothing
            Me.Url = "https://www.camm.biz/support/cim/services/registration.asmx"
            result = Me.Invoke("ValidateInstallation", New Object() {info})

            Dim contractInfo As InstanceValidationResult = CType(result(0), InstanceValidationResult)
            If VerifyValidationResult(contractInfo, ValidationRequestType.ServiceRequestWithTTL) Then
                Return contractInfo
            End If
            Throw New Exception("Response manipulated during transfer or response took too long")
        End Function

    End Class

    ''' <summary>
    ''' The several kinds of notification e-mails
    ''' </summary>
    Friend Enum ContractExpirationNotificationTypes As Integer
        UpdateContract = 0
        SupportAndMaintananceContract = 1
        Licence = 2
    End Enum

    ''' <summary>
    ''' An e-mail recipient for expiration notifications
    ''' </summary>
    Friend Class ContractExpirationRecipient
        Public FullName As String
        Public EMail As String
    End Class

    Friend Class ContractExpirationNotifier

        Public Delegate Sub RecpientNotifcationFunction(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date, instanceReference As String)

        Private NotificationFunction As RecpientNotifcationFunction
        Private Recipients As New ArrayList
        Private ExpirationDate As DateTime
        Private InstanceReference As String

        Private cammwebmanager As WMSystem
        Public Sub New(ByVal cwm As WMSystem, ByVal expirationDate As DateTime, ByVal notificationType As ContractExpirationNotificationTypes, instanceReference As String)
            Me.cammwebmanager = cwm
            Me.ExpirationDate = expirationDate
            SetDelegate(notificationType)
            Me.InstanceReference = instanceReference
        End Sub

        Private Sub SetDelegate(ByVal notificationType As ContractExpirationNotificationTypes)
            Select Case notificationType
                Case ContractExpirationNotificationTypes.Licence
                    NotificationFunction = New RecpientNotifcationFunction(AddressOf Me.cammwebmanager.Notifications.SendLicenceHasExpiredMessage)
                Case ContractExpirationNotificationTypes.SupportAndMaintananceContract
                    NotificationFunction = New RecpientNotifcationFunction(AddressOf Me.cammwebmanager.Notifications.SendSupportContractHasExpiredMessage)
                Case ContractExpirationNotificationTypes.UpdateContract
                    NotificationFunction = New RecpientNotifcationFunction(AddressOf Me.cammwebmanager.Notifications.SendUpdateContractHasExpiredMessage)
            End Select
        End Sub

        ''' <summary>
        ''' Add a single user to the list of recipients
        ''' </summary>
        ''' <param name="recipient"></param>
        Public Sub AddRecipient(ByVal recipient As ContractExpirationRecipient)
            Me.Recipients.Add(recipient)
        End Sub

        ''' <summary>
        ''' Add a single user to the list of recipients
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="email"></param>
        Public Sub AddRecipient(ByVal name As String, ByVal email As String)
            Dim recipient As New ContractExpirationRecipient
            recipient.FullName = name
            recipient.EMail = email
            AddRecipient(recipient)
        End Sub

        ''' <summary>
        ''' Add a single user to the list of recipients
        ''' </summary>
        ''' <param name="user"></param>
        Public Sub AddRecipient(ByVal user As WMSystem.UserInformation)
            AddRecipient(user.FullName, user.EMailAddress)
        End Sub

        ''' <summary>
        ''' Add all members of a group to the list of recipients
        ''' </summary>
        ''' <param name="group"></param>
        Public Sub AddRecipient(ByVal group As WMSystem.SpecialGroups)
            Dim groupInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(group, cammwebmanager)
            If Not groupInfo Is Nothing AndAlso Not groupInfo.MembersByRule().Effective Is Nothing Then
                For Each user As WMSystem.UserInformation In groupInfo.MembersByRule().Effective
                    AddRecipient(user)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Send the notification e-mails to all recipients (but don't send 2 or more e-mails to the same e-mail address)
        ''' </summary>
        Public Sub Notify()
            Dim sentAlready As New ArrayList
            For Each recipient As ContractExpirationRecipient In Me.Recipients
                Dim email As String = recipient.EMail.Trim()
                Dim name As String = recipient.FullName.Trim()
                If Not sentAlready.Contains(email) Then
                    NotificationFunction(name, email, Me.ExpirationDate, Me.InstanceReference)
                    sentAlready.Add(email)
                End If
            Next
        End Sub
    End Class

    ''' <summary>
    ''' Client logic for the product registration service 
    ''' </summary>
    Public Class ProductRegistration

        Public Const CacheKeyLastRegistrationUpdate As String = "LastFetchFromServer"
        Public Const CacheKeyServerIsDown As String = "RegistrationServerOffline"
        Public Const CacheKeyManualLicenseRevalidationOnly As String = "ManualLicenseRevalidationOnly"

        Private cammWebManger As WMSystem

        Private UpdateRegistrationDataFromRemoteLicenseServerFailed As Boolean = False

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
        ''' Contacts our "licence server" and fetches expiration data etc.
        ''' </summary>
        Private Function FetchValidationDataFromRemoteLicenseServer(throwExceptions As Boolean) As InstanceValidationResult
            Dim installationInfo As InstanceValidationResult = Nothing
            Try
                Dim factory As New CwmInstallationInfoFactory(Me.cammWebManger)
                Dim client As New ProductRegistrationClient(factory.CollectInstallationInfo())
                installationInfo = client.ValidateInstallationHttpsOrHttp()
                UpdateRegistrationDataFromRemoteLicenseServerFailed = False
            Catch ex As Exception
                UpdateRegistrationDataFromRemoteLicenseServerFailed = True
                Me.cammWebManger.Log.Exception(ex, throwExceptions)
            End Try
            Return installationInfo
        End Function

        Private Sub UpdateExpirationMailSendingDateInGlobalPropertiesDbTable(ByVal key As String, ByVal dateSent As DateTime)
            Dim sql As String = "UPDATE System_GlobalProperties SET ValueDateTime = @dateSent WHERE PropertyName = @key AND ValueNVarchar = N'camm WebManager'" &
                    "IF @@ROWCOUNT = 0 " &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarchar, ValueDateTime) VALUES(@key, 'camm WebManager', @dateSent) "

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
        Private Function FetchDateFromGlobalPropertiesDbTable(ByVal key As String) As DateTime
            'TODO: this method should probably be in a class specialy made for this purpose...
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key AND ValueNVarchar = N'camm WebManager'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return Nothing
            Else
                Return CType(value, DateTime)
            End If
        End Function

        ''' <summary>
        ''' Fetches boolean value with the corresponding key form the global properties table
        ''' </summary>
        ''' <param name="key"></param>
        Private Function FetchBooleanFromGlobalPropertiesDbTable(ByVal key As String) As Boolean
            'TODO: this method should probably be in a class specialy made for this purpose...
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueBoolean FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key AND ValueNVarchar = N'camm WebManager'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return False
            Else
                Return CType(value, Boolean)
            End If
        End Function
        Private Function MustSendMail(ByVal key As String) As Boolean
            Dim mailsent As DateTime = Me.FetchDateFromGlobalPropertiesDbTable(key)
            Return mailsent = Nothing OrElse DateTime.Now.ToUniversalTime().Subtract(mailsent).TotalHours >= 24
        End Function

#Region "Mail distribution"

        ''' <summary>
        ''' Notifies appropriate recipients when the support and maintanence contract has expired
        ''' </summary>
        Private Sub SendExpiredSupportContractNotificationMails(ByVal expirationDate As DateTime)

            Dim notifier As New ContractExpirationNotifier(Me.cammWebManger, expirationDate, ContractExpirationNotificationTypes.SupportAndMaintananceContract, Me.cammWebManger.Environment.LicenceKeyShortened)

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
                UpdateExpirationMailSendingDateInGlobalPropertiesDbTable(key, currentDate)
            End If
        End Sub

        Private Sub SendExpiredUpdateContractMail(ByVal expirationDate As DateTime)
            Dim notifier As New ContractExpirationNotifier(Me.cammWebManger, expirationDate, ContractExpirationNotificationTypes.UpdateContract, Me.cammWebManger.Environment.LicenceKeyShortened)

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
                UpdateExpirationMailSendingDateInGlobalPropertiesDbTable(key, DateTime.Now.ToUniversalTime())
            End If
        End Sub

        Private Sub SendExpiringLicenceNotificationMails(ByVal expirationDate As DateTime, ByVal daysTillExpiration As Double)
            Dim notifier As New ContractExpirationNotifier(Me.cammWebManger, expirationDate, ContractExpirationNotificationTypes.Licence, Me.cammWebManger.Environment.LicenceKeyShortened)

            If daysTillExpiration <= 14 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_Supervisors)
            ElseIf daysTillExpiration <= 5 Then
                notifier.AddRecipient(WMSystem.SpecialGroups.Group_SecurityAdministrators)

            End If
            Dim key As String = "LicenceExpirationMailSent"
            If MustSendMail(key) Then
                notifier.Notify()
                UpdateExpirationMailSendingDateInGlobalPropertiesDbTable(key, DateTime.Now.ToUniversalTime())
            End If
        End Sub
#End Region

        ''' <summary>
        ''' Returns whether the licence server is marked as being down 
        ''' </summary>
        Private Function IsRemoteLicenseServerMarkedOffline() As Boolean
            Dim downDate As DateTime = Nothing
            Dim cacheValue As Object = HttpContext.Current.Cache.Item(CacheKeyServerIsDown)
            If Not cacheValue Is Nothing Then
                downDate = CType(cacheValue, DateTime)
            Else
                downDate = FetchDateFromGlobalPropertiesDbTable(CacheKeyServerIsDown)
            End If
            Return downDate <> Nothing AndAlso DateTime.Now.Subtract(downDate).TotalMinutes < 5
        End Function

        ''' <summary>
        ''' Mark the server as currently down to try again later.
        ''' </summary>
        Private Sub MarkRemoteLicenseServerAsOffline()
            Dim currentDate As DateTime = DateTime.Now
            HttpContext.Current.Cache(CacheKeyServerIsDown) = currentDate
            SaveDate2GlobalPropertiesDbTable(CacheKeyServerIsDown, currentDate)
        End Sub

        ''' <summary>
        ''' Returns whether it's time to fetch validation data from server
        ''' </summary>
        ''' <param name="hours">specifies how many hours must have passed since the last check</param>
        Friend Function IsRefreshFromRemoteLicenseServerRequired(ByVal hours As Integer) As Boolean
            Dim lastCheckDate As DateTime = CachedLastRefreshDate
            If lastCheckDate = Nothing Then
                Dim onlyManualChecksByUserAtLicenseServer As Boolean = FetchBooleanFromGlobalPropertiesDbTable(CacheKeyManualLicenseRevalidationOnly)
                If onlyManualChecksByUserAtLicenseServer = True Then
                    'Never do any checks with remote server -> set last check date into far future to prevent a new check
                    lastCheckDate = Now.AddYears(1)
                    CachedLastRefreshDate = lastCheckDate
                Else
                    'Asking remote server is allowed
                    lastCheckDate = FetchDateFromGlobalPropertiesDbTable(CacheKeyLastRegistrationUpdate)
                    CachedLastRefreshDate = lastCheckDate
                End If
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
        Public Sub RefreshValidationDataFromRemoteLicenseServer(throwExceptions As Boolean)
            Dim validationResult As InstanceValidationResult = FetchValidationDataFromRemoteLicenseServer(throwExceptions)
            If Not validationResult Is Nothing Then
                Dim validationDao As New InstanceValidationDao(Me.cammWebManger)
                validationDao.Save(validationResult)
                SaveLastRefreshDate(DateTime.Now)
            Else
                MarkRemoteLicenseServerAsOffline()
            End If
        End Sub

        ''' <summary>
        ''' Assign text data from the licence server and saves it into SystemGlobalProperties database table
        ''' </summary>
        Public Sub RefreshValidationDataManuallyFromLicenseDataFromLicenseServer(licenseData As String)
            If licenseData = Nothing Then Throw New Exception("License data missing")
            Dim validationResult As InstanceValidationResult = InstanceValidationResult.CreateInstanceFromString(licenseData)
            If validationResult Is Nothing Then
                Throw New Exception("License data invalid or missing")
            ElseIf ProductRegistrationClient.VerifyValidationResult(validationResult, ProductRegistrationClient.ValidationRequestType.ManualRequest) = False Then
                Throw New Exception("License data validation failed")
            End If
            Dim validationDao As New InstanceValidationDao(Me.cammWebManger)
            validationDao.Save(validationResult)
            SaveLastRefreshDate(DateTime.Now)
        End Sub


        ''' <summary>
        ''' Load validation result data from cache data in SystemGlobalProperties database table
        ''' </summary>
        Public Function GetCachedValidationResult() As InstanceValidationResult
            Dim validationDao As New InstanceValidationDao(Me.cammWebManger)
            Return validationDao.Load()
        End Function

        Private Shared CheckRegistrationLock As Object = New Object()
        ''' <summary>
        ''' Method called by the webservice to check the registration
        ''' </summary>
        Friend Sub CheckRegistration(throwExceptions As Boolean)
            Try
                SyncLock CheckRegistrationLock
                    If Not IsRemoteLicenseServerMarkedOffline() AndAlso IsRefreshFromRemoteLicenseServerRequired(24) Then
                        RefreshValidationDataFromRemoteLicenseServer(throwExceptions)
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

    ''' <summary>
    ''' Data access object/layer
    ''' </summary>
    Friend Class InstanceValidationDao

        Private cammWebManger As WMSystem



        Public Sub New(ByVal cwm As WMSystem)
            Me.cammWebManger = cwm
        End Sub

        Public Sub Save(ByVal validationResult As InstanceValidationResult)
            If Not validationResult Is Nothing Then
                Dim sql As String = "UPDATE System_GlobalProperties SET ValueInt = @licenceType WHERE PropertyName = 'LicenceType' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueInt) VALUES('LicenceType', 'camm WebManager', @licenceType) " & System.Environment.NewLine & System.Environment.NewLine &
                    "" &
                    "UPDATE System_GlobalProperties SET ValueDateTime = @licenceExpirationDate WHERE PropertyName = 'LicenceExpirationDate' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueDateTime) VALUES('LicenceExpirationDate', 'camm WebManager', @licenceExpirationDate) " & System.Environment.NewLine &
                    "" &
                     "UPDATE System_GlobalProperties SET ValueInt = @licenceModel WHERE PropertyName = 'LicenceModel' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                     "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                     "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueInt) VALUES('LicenceModel', 'camm WebManager', @licenceModel) " & System.Environment.NewLine & System.Environment.NewLine &
                    "" &
                    "UPDATE System_GlobalProperties SET ValueDateTime = @supportContractExpirationDate WHERE PropertyName = 'SnMContractExpires' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName,ValueNVarChar,  ValueDateTime) VALUES('SnMContractExpires', 'camm WebManager', @supportContractExpirationDate) " & System.Environment.NewLine &
                    "" &
                   "UPDATE System_GlobalProperties SET ValueDateTime = @updateContractExpires WHERE PropertyName = 'UpdateContractExpires' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueDateTime) VALUES('UpdateContractExpires', 'camm WebManager', @updateContractExpires) " & System.Environment.NewLine &
                    "" &
                   "UPDATE System_GlobalProperties SET ValueBoolean = @UpdatesAvailable WHERE PropertyName = 'UpdatesAvailable' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueBoolean) VALUES('UpdatesAvailable', 'camm WebManager', @UpdatesAvailable) " & System.Environment.NewLine &
                    "" &
                   "UPDATE System_GlobalProperties SET ValueBoolean = @SecurityUpdatesAvailable WHERE PropertyName = 'SecurityUpdatesAvailable' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueBoolean) VALUES('SecurityUpdatesAvailable', 'camm WebManager', @SecurityUpdatesAvailable) " & System.Environment.NewLine &
                    "" &
                   "UPDATE System_GlobalProperties SET ValueBoolean = @ManualLicenseRevalidationOnly WHERE PropertyName = 'ManualLicenseRevalidationOnly' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueBoolean) VALUES('ManualLicenseRevalidationOnly', 'camm WebManager', @ManualLicenseRevalidationOnly) " & System.Environment.NewLine &
                    "" &
                   "UPDATE System_GlobalProperties SET ValueBoolean = @NoAutomaticLicenseUsageStatisticsPublishing WHERE PropertyName = 'NoAutomaticLicenseUsageStatisticsPublishing' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueBoolean) VALUES('NoAutomaticLicenseUsageStatisticsPublishing', 'camm WebManager', @NoAutomaticLicenseUsageStatisticsPublishing) " & System.Environment.NewLine &
                    "" &
                   "UPDATE System_GlobalProperties SET ValueImage = @KeyLicenseUsagePublishing WHERE PropertyName = 'KeyLicenseUsagePublishing' AND ValueNVarchar = N'camm WebManager'" & System.Environment.NewLine &
                    "IF @@ROWCOUNT = 0 " & System.Environment.NewLine &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarChar, ValueImage) VALUES('KeyLicenseUsagePublishing', 'camm WebManager', @KeyLicenseUsagePublishing) " & System.Environment.NewLine
                Dim cmd As New SqlClient.SqlCommand(sql)
                Dim connection As SqlClient.SqlConnection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
                cmd.Connection = connection
                cmd.Parameters.Add("@updateContractExpires", SqlDbType.DateTime).Value = Utils.DateTimeNotNothingOrDBNull(validationResult.UpdateContractExpirationDate)
                cmd.Parameters.Add("@supportContractExpirationDate", SqlDbType.DateTime).Value = Utils.DateTimeNotNothingOrDBNull(validationResult.SupportContractExpirationDate)
                cmd.Parameters.Add("@licenceExpirationDate", SqlDbType.DateTime).Value = Utils.DateTimeNotNothingOrDBNull(validationResult.LicenceData.ExpirationDate)
                cmd.Parameters.Add("@licenceType", SqlDbType.Int).Value = validationResult.LicenceData.Type
                cmd.Parameters.Add("@licenceModel", SqlDbType.Int).Value = validationResult.LicenceData.Model
                cmd.Parameters.Add("@UpdatesAvailable", SqlDbType.Bit).Value = validationResult.UpdateAvailable
                cmd.Parameters.Add("@SecurityUpdatesAvailable", SqlDbType.Bit).Value = validationResult.SecurityUpdateAvailable
                If validationResult.KeyLicenseUsagePublishing Is Nothing OrElse validationResult.KeyLicenseUsagePublishing.Length = 0 Then
                    cmd.Parameters.Add("@KeyLicenseUsagePublishing", SqlDbType.VarBinary).Value = DBNull.Value
                Else
                    cmd.Parameters.Add("@KeyLicenseUsagePublishing", SqlDbType.VarBinary).Value = validationResult.KeyLicenseUsagePublishing
                End If
                cmd.Parameters.Add("@ManualLicenseRevalidationOnly", SqlDbType.Bit).Value = validationResult.ManualLicenseRevalidationOnly
                cmd.Parameters.Add("@NoAutomaticLicenseUsageStatisticsPublishing", SqlDbType.Bit).Value = validationResult.NoAutomaticLicenseUsageStatisticsPublishing
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                Me.cammWebManger.Environment.CachedProductName = Nothing
            End If
        End Sub

        ''' <summary>
        ''' Fetches datetime value with the corresponding key form the global properties table
        ''' </summary>
        ''' <param name="key"></param>
        ''' TODO: this doesn't belong to this class, it should probably be in one which is made for this purpose...
        Private Function GetExpirationDate(ByVal key As String) As DateTime
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueDateTime FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key AND ValueNVarchar = N'camm WebManager'"
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
        ''' Fetches bit value with the corresponding key form the global properties table
        ''' </summary>
        ''' <param name="key"></param>
        ''' TODO: this doesn't belong to this class, it should probably be in one which is made for this purpose...
        Private Function GetBinaryValue(ByVal key As String, defaultValue As Byte()) As Byte()
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueImage FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key AND ValueNVarchar = N'camm WebManager'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return defaultValue
            Else
                Return CType(value, Byte())
            End If
        End Function


        ''' <summary>
        ''' Fetches bit value with the corresponding key form the global properties table
        ''' </summary>
        ''' <param name="key"></param>
        ''' TODO: this doesn't belong to this class, it should probably be in one which is made for this purpose...
        Private Function GetBooleanValue(ByVal key As String, defaultValue As Boolean) As Boolean
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueBoolean FROM [dbo].[System_GlobalProperties] WHERE PropertyName = @key AND ValueNVarchar = N'camm WebManager'"
            cmd.CommandType = CommandType.Text
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            cmd.Parameters.Add("@key", SqlDbType.VarChar).Value = key
            Dim value As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            If IsDBNull(value) OrElse value Is Nothing Then
                Return defaultValue
            Else
                Return CType(value, Boolean)
            End If
        End Function

        ''' <summary>
        ''' Returns the license model
        ''' </summary>
        Private Function GetLicenceModel() As LicenceData.LicenseModel
            Dim cmd As New SqlClient.SqlCommand
            cmd.CommandText = "SELECT ValueInt FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LicenceModel' AND ValueNVarchar = N'camm WebManager'"
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
            cmd.CommandText = "SELECT ValueInt FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LicenceType' AND ValueNVarchar = N'camm WebManager'"
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
            Dim Result As New InstanceValidationResult
            Result.SupportContractExpirationDate = GetExpirationDate("SnMContractExpires")
            Result.UpdateContractExpirationDate = GetExpirationDate("UpdateContractExpires")
            Result.LicenceData = New LicenceData()
            Result.LicenceData.ExpirationDate = GetExpirationDate("LicenceExpirationDate")
            Result.LicenceData.Model = GetLicenceModel()
            Result.LicenceData.Type = GetLicenceType()
            Result.UpdateAvailable = GetBooleanValue("UpdatesAvailable", False)
            Result.SecurityUpdateAvailable = GetBooleanValue("SecurityUpdatesAvailable", False)
            Result.ManualLicenseRevalidationOnly = GetBooleanValue("ManualLicenseRevalidationOnly", False)
            Result.NoAutomaticLicenseUsageStatisticsPublishing = GetBooleanValue("NoAutomaticLicenseUsageStatisticsPublishing", False)
            Result.KeyLicenseUsagePublishing = GetBinaryValue("KeyLicenseUsagePublishing", Nothing)
            Return Result
        End Function

        Public Sub SetLicenceModel(ByVal model As LicenceData.LicenseModel)
            Dim sql As String = "UPDATE System_GlobalProperties SET ValueInt = @licenceModel WHERE PropertyName = 'LicenceModel' AND ValueNVarchar = N'camm WebManager' " &
                    "IF @@ROWCOUNT = 0 " &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarchar, ValueInt) VALUES('LicenceModel', 'camm WebManager', @licenceModel) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@licenceModel", SqlDbType.Int).Value = model
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub

        Public Sub SetLicenceExpirationDate(ByVal expirationDate As DateTime)
            Dim sql As String = "UPDATE System_GlobalProperties SET ValueDateTime = @expirationDate WHERE PropertyName = 'LicenceExpirationDate' AND ValueNVarchar = N'camm WebManager' " &
                    "IF @@ROWCOUNT = 0 " &
                    "INSERT INTO System_GlobalProperties (PropertyName, ValueNVarchar, ValueDateTime) VALUES('LicenceExpirationDate', 'camm WebManager', @expirationDate) "

            Dim cmd As New SqlClient.SqlCommand(sql)
            cmd.Parameters.Add("@expirationDate", SqlDbType.DateTime).Value = expirationDate
            cmd.Connection = New SqlClient.SqlConnection(Me.cammWebManger.ConnectionString)
            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub
    End Class

End Namespace
