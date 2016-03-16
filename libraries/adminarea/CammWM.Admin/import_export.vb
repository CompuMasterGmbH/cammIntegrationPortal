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

Namespace CompuMaster.camm.WebManager.Administration

    ''' <summary>
    '''     Export of data
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class Export

        Public Shared Sub SendExportFileAsCsv(ByVal webmanager As WMSystem, ByVal data As DataTable)
            SendExportFileAsCsv(webmanager, data, "UTF-8")
        End Sub

        Public Shared Sub SendExportFileAsCsv(ByVal webmanager As WMSystem, ByVal data As DataTable, ByVal encoding As String)
            SendExportFileAsCsv(webmanager, data, encoding, "export")
        End Sub

        Public Shared Sub SendExportFileAsCsv(ByVal webmanager As WMSystem, ByVal data As DataTable, ByVal encoding As String, ByVal outputFilePrefix As String)
            If System.Web.HttpContext.Current Is Nothing Then
                Throw New NullReferenceException("HttpContext.Current is required for this method")
            End If

            Dim _webmanager As WMSystem = webmanager
            Dim fileName As String = outputFilePrefix & "-" & Now.ToString("yyyyMMddHHmmss") & ".csv"
            Dim subFolder As String = "webmanager/administration"
            Dim fileLink As String = _webmanager.DownloadHandler.CreatePlainDownloadLink(DownloadHandler.DownloadLocations.WebManagerUserSession, subFolder, fileName)
            Dim rawSingleFile As New CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile
            Dim bytes As Byte() = CompuMaster.camm.WebManager.Administration.Tools.Data.Csv.WriteDataTableToCsvBytes(data, True, System.Text.Encoding.GetEncoding(encoding), System.Globalization.CultureInfo.CurrentCulture)
            rawSingleFile.Filename = fileName
            rawSingleFile.MimeType = "application/octet-stream"
            rawSingleFile.Data = bytes
            _webmanager.DownloadHandler.Clear()
            _webmanager.DownloadHandler.Add(rawSingleFile, subFolder)
            _webmanager.DownloadHandler.ProcessDownload(DownloadHandler.DownloadLocations.WebManagerUserSession, Nothing, True)

        End Sub

#Region "Memberships"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A table with all group memberships of the requested user
        ''' </summary>
        ''' <param name="webmanager">An instance of a valid camm Web-Manager</param>
        ''' <param name="userID">The requested user ID</param>
        ''' <returns>A data table containing groups and users information</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function MembershipsOfUser(ByVal webmanager As WMSystem, ByVal userID As Long) As DataTable

            Dim uInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(userID, webmanager)
            Dim Result As DataTable = MembershipsTableCreate("memberships")

            For MyCounter As Integer = 0 To uInfo.Memberships.Length - 1
                Dim newRow As DataRow = Result.NewRow
                MembershipsTableAddRow(Result, uInfo.Memberships(MyCounter), uInfo)
            Next

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A table with all users of the requested user group
        ''' </summary>
        ''' <param name="webmanager">An instance of a valid camm Web-Manager</param>
        ''' <param name="groupID">The requested user group ID</param>
        ''' <returns>A data table containing groups and users information</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function MembersOfGroup(ByVal webmanager As WMSystem, ByVal groupID As Integer) As DataTable

            Return MembersOfGroup(webmanager, groupID, True)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A table with active/ all users of the requested user group
        ''' </summary>
        ''' <param name="webmanager"></param>
        ''' <param name="groupID"></param>
        ''' <param name="includeDisabledUsers"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	24.05.2012	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function MembersOfGroup(ByVal webmanager As WMSystem, ByVal groupID As Integer, ByVal includeDisabledUsers As Boolean) As DataTable

            Dim gInfo As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(groupID, webmanager)
            Dim Result As DataTable = MembershipsTableCreate("members")

            For MyCounter As Integer = 0 To gInfo.Members.Length - 1
                If includeDisabledUsers = False AndAlso gInfo.Members(MyCounter).LoginDisabled Then
                    'don't add a row with disabled user
                Else
                    Dim newRow As DataRow = Result.NewRow
                    MembershipsTableAddRow(Result, gInfo, gInfo.Members(MyCounter))
                End If
            Next

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Attach one line to the output table
        ''' </summary>
        ''' <param name="memberships">The output table</param>
        ''' <param name="groupInfo">One group information object</param>
        ''' <param name="userInfo">One user information object</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub MembershipsTableAddRow(ByVal memberships As DataTable, ByVal groupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation, ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation)

            'parameter validation
            If userInfo Is Nothing AndAlso groupInfo Is Nothing Then
                Throw New ArgumentNullException("groupInfo and userInfo")
            End If
            If memberships Is Nothing Then
                Throw New ArgumentNullException("memberships")
            End If

            'add the new row
            Dim newRow As DataRow = memberships.NewRow
            If Not groupInfo Is Nothing Then
                newRow(0) = groupInfo.ID
                newRow(1) = groupInfo.Name
                newRow(2) = groupInfo.Description
                newRow(3) = groupInfo.IsSystemGroup
            End If
            If Not userInfo Is Nothing Then
                newRow(4) = userInfo.IDLong
                newRow(5) = userInfo.LoginName
                newRow(6) = userInfo.EMailAddress
                newRow(7) = userInfo.Gender.ToString
                newRow(8) = userInfo.AcademicTitle
                newRow(9) = userInfo.FirstName
                newRow(10) = userInfo.LastName
                newRow(11) = userInfo.NameAddition
                newRow(12) = userInfo.FullName
                newRow(13) = userInfo.SalutationNameOnly
                newRow(14) = userInfo.Company
                newRow(15) = userInfo.Position
                newRow(16) = userInfo.Street
                newRow(17) = userInfo.ZipCode
                newRow(18) = userInfo.Location
                newRow(19) = userInfo.State
                newRow(20) = userInfo.Country
                newRow(21) = userInfo.PhoneNumber
                newRow(22) = userInfo.MobileNumber
                newRow(23) = userInfo.FaxNumber
                newRow(24) = userInfo.PreferredLanguage1.ID
                newRow(25) = userInfo.PreferredLanguage2.ID
                newRow(26) = userInfo.PreferredLanguage3.ID
                newRow(27) = userInfo.LoginDisabled
                newRow(28) = userInfo.LoginDeleted
                newRow(29) = userInfo.LoginLockedTemporaryTill
                newRow(30) = userInfo.IsSystemUser()
                newRow(31) = userInfo.ExternalAccount()
                newRow(32) = userInfo.AccessLevel.ID
                newRow(33) = userInfo.AccountAuthorizationsAlreadySet()
                newRow(34) = userInfo.AccountProfileValidatedByEMailTest()
                newRow(35) = userInfo.AutomaticLogonAllowedByMachineToMachineCommunication()
                newRow(36) = CompuMaster.camm.WebManager.Utils.JoinNameValueCollectionWithUrlEncodingToString(userInfo.AdditionalFlags)
                newRow(37) = userInfo.LoginLockedTemporary
            End If
            memberships.Rows.Add(newRow)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create an empty datatable for output
        ''' </summary>
        ''' <param name="tableName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function MembershipsTableCreate(ByVal tableName As String) As DataTable

            Dim Result As New DataTable(tableName)
            Result.Columns.Add(New DataColumn("Group_ID", GetType(Integer)))
            Result.Columns.Add(New DataColumn("Group_Name", GetType(String)))
            Result.Columns.Add(New DataColumn("Group_Description", GetType(String)))
            Result.Columns.Add(New DataColumn("Group_IsSystemGroup", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_ID", GetType(Long)))
            Result.Columns.Add(New DataColumn("User_LoginName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_EMailAddress", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Gender", GetType(String)))
            Result.Columns.Add(New DataColumn("User_AcademicTitle", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FirstName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_LastName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_NameAddition", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FullName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_SalutationNameOnly", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Company", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Position", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Street", GetType(String)))
            Result.Columns.Add(New DataColumn("User_ZipCode", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Location", GetType(String)))
            Result.Columns.Add(New DataColumn("User_State", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Country", GetType(String)))
            Result.Columns.Add(New DataColumn("User_PhoneNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_MobileNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FaxNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage1", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage2", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage3", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_LoginDisabled", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginDeleted", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginLockedTemporaryTill", GetType(DateTime)))
            Result.Columns.Add(New DataColumn("User_IsSystemUser", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_ExternalAccount", GetType(String)))
            Result.Columns.Add(New DataColumn("User_AccessLevel", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_AccountAuthorizationsAlreadySet", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AccountProfileValidatedByEMailTest", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AutomaticLogonAllowedByMachineToMachineCommunication", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AdditionalFlags", GetType(String)))
            Result.Columns.Add(New DataColumn("User_LoginLockedTemporary", GetType(Boolean)))
            Return Result
        End Function
#End Region

#Region "Authorizations"

        Public Shared Function AuthorizationsOfUser(ByVal webmanager As WMSystem, ByVal userID As Long) As DataTable
            Throw New NotImplementedException("ToDo: implementatation of AuthorizationsOfUser")
        End Function

        Public Shared Function AuthorizationsOfGroup(ByVal webmanager As WMSystem, ByVal groupID As Integer) As DataTable
            Throw New NotImplementedException("ToDo: implementatation of AuthorizationsOfGroup")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load the list of directly and inherited authorizations for a security object
        ''' </summary>
        ''' <param name="webmanager"></param>
        ''' <param name="securityObjectID"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function AuthorizedGroupsAndPersons(ByVal webmanager As WMSystem, ByVal securityObjectID As Integer, ByVal alsoQueryInheritedAuthorizations As Boolean) As DataTable

            'Dim soInfo As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(securityObjectID, webmanager)
            Dim serverGroupID As Integer, groupID As Integer, userID As Long
            Dim authsInfo As New CompuMaster.camm.WebManager.WMSystem.Authorizations(securityObjectID, webmanager, serverGroupID, groupID, userID)

            Dim Result As DataTable = AuthorizationsTableCreate("authorizations")

            Dim gAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.GroupAuthorizationInformation()
            Dim uAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation()
            Dim securityObjectInfo As CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation = Nothing
            gAuths = authsInfo.GroupAuthorizationInformations
            uAuths = authsInfo.UserAuthorizationInformations

            'Load directly wired items
            If Not gAuths Is Nothing Then
                If securityObjectInfo Is Nothing AndAlso gAuths.Length > 0 Then
                    'Cache the current security object info for later usage
                    securityObjectInfo = gAuths(0).SecurityObjectInfo
                End If
                'Add all user groups to the result table
                For MyCounter As Integer = 0 To gAuths.Length - 1
                    Dim newRow As DataRow = Result.NewRow
                    AuthorizationsTableAddRow(Result, gAuths(MyCounter).SecurityObjectInfo, gAuths(MyCounter).GroupInfo, Nothing, gAuths(MyCounter).ServerGroupInfo, False)
                Next
            End If
            If Not uAuths Is Nothing Then
                If securityObjectInfo Is Nothing AndAlso uAuths.Length > 0 Then
                    'Cache the current security object info for later usage
                    securityObjectInfo = uAuths(0).SecurityObjectInfo
                End If
                'Add all users to the result table
                For MyCounter As Integer = 0 To uAuths.Length - 1
                    Dim newRow As DataRow = Result.NewRow
                    AuthorizationsTableAddRow(Result, uAuths(MyCounter).SecurityObjectInfo, Nothing, uAuths(MyCounter).UserInfo, uAuths(MyCounter).ServerGroupInfo, False)
                Next
            End If

            If alsoQueryInheritedAuthorizations Then
                'Add all inherited items, too
                Dim iAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations
                iAuths = authsInfo.InheritedAuthorizations
                If Not iAuths Is Nothing Then
                    If securityObjectInfo Is Nothing Then
                        'These inherited items shall be added into the resulting table as they were the originally specified security object info
                        securityObjectInfo = New WMSystem.SecurityObjectInformation(securityObjectID, webmanager)
                    End If
                    Dim igAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.GroupAuthorizationInformation()
                    Dim iuAuths As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation()
                    igAuths = authsInfo.InheritedAuthorizations.GroupAuthorizationInformations
                    iuAuths = authsInfo.InheritedAuthorizations.UserAuthorizationInformations
                    'Lookup inherited authorization items
                    If Not igAuths Is Nothing Then
                        'Add all inherited user groups to the result table
                        For MyCounter As Integer = 0 To igAuths.Length - 1
                            Dim newRow As DataRow = Result.NewRow
                            AuthorizationsTableAddRow(Result, securityObjectInfo, igAuths(MyCounter).GroupInfo, Nothing, igAuths(MyCounter).ServerGroupInfo, True)
                        Next
                    End If
                    If Not iuAuths Is Nothing Then
                        'Add all inherited users to the result table
                        For MyCounter As Integer = 0 To iuAuths.Length - 1
                            Dim newRow As DataRow = Result.NewRow
                            AuthorizationsTableAddRow(Result, securityObjectInfo, Nothing, iuAuths(MyCounter).UserInfo, iuAuths(MyCounter).ServerGroupInfo, True)
                        Next
                    End If
                End If
            End If

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Attach one line to the output table
        ''' </summary>
        ''' <param name="authorizations">The output table</param>
        ''' <param name="securityObjectInfo"></param>
        ''' <param name="groupInfo">One group information object</param>
        ''' <param name="userInfo">One user information object</param>
        ''' <param name="serverGroupInfo"></param>
        ''' <param name="isInherited"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AuthorizationsTableAddRow(ByVal authorizations As DataTable, ByVal securityObjectInfo As WMSystem.SecurityObjectInformation, ByVal groupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation, ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation, ByVal serverGroupInfo As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation, ByVal isInherited As Boolean)

            'parameter validation
            If securityObjectInfo Is Nothing AndAlso userInfo Is Nothing AndAlso groupInfo Is Nothing Then
                Throw New ArgumentNullException("groupInfo and userInfo")
            End If
            If authorizations Is Nothing Then
                Throw New ArgumentNullException("memberships")
            End If

            'add the new row
            Dim newRow As DataRow = authorizations.NewRow
            If Not securityObjectInfo Is Nothing Then
                newRow(0) = securityObjectInfo.ID
                newRow(1) = securityObjectInfo.Name
                newRow(2) = securityObjectInfo.DisplayName
                newRow(3) = securityObjectInfo.InheritFrom_SecurityObjectID
                newRow(4) = securityObjectInfo.SystemType
                newRow(5) = securityObjectInfo.Disabled
                newRow(6) = securityObjectInfo.Deleted
            End If
            If Not groupInfo Is Nothing Then
                newRow(7) = groupInfo.ID
                newRow(8) = groupInfo.Name
                newRow(9) = groupInfo.Description
                newRow(10) = groupInfo.IsSystemGroup
            End If
            If Not userInfo Is Nothing Then
                newRow(11) = userInfo.IDLong
                newRow(12) = userInfo.LoginName
                newRow(13) = userInfo.EMailAddress
                newRow(14) = userInfo.Gender.ToString
                newRow(15) = userInfo.AcademicTitle
                newRow(16) = userInfo.FirstName
                newRow(17) = userInfo.LastName
                newRow(18) = userInfo.NameAddition
                newRow(19) = userInfo.FullName
                newRow(20) = userInfo.SalutationNameOnly
                newRow(21) = userInfo.Company
                newRow(22) = userInfo.Position
                newRow(23) = userInfo.Street
                newRow(24) = userInfo.ZipCode
                newRow(25) = userInfo.Location
                newRow(26) = userInfo.State
                newRow(27) = userInfo.Country
                newRow(28) = userInfo.PhoneNumber
                newRow(29) = userInfo.MobileNumber
                newRow(30) = userInfo.FaxNumber
                newRow(31) = userInfo.PreferredLanguage1.ID
                newRow(32) = userInfo.PreferredLanguage2.ID
                newRow(33) = userInfo.PreferredLanguage3.ID
                newRow(34) = userInfo.LoginDisabled
                newRow(35) = userInfo.LoginDeleted
                newRow(36) = userInfo.LoginLockedTemporaryTill
                newRow(37) = userInfo.IsSystemUser()
                newRow(38) = userInfo.ExternalAccount()
                newRow(39) = userInfo.AccessLevel.ID
                newRow(40) = userInfo.AccountAuthorizationsAlreadySet()
                newRow(41) = userInfo.AccountProfileValidatedByEMailTest()
                newRow(42) = userInfo.AutomaticLogonAllowedByMachineToMachineCommunication()
                newRow(43) = CompuMaster.camm.WebManager.Utils.JoinNameValueCollectionWithUrlEncodingToString(userInfo.AdditionalFlags)
                newRow(44) = userInfo.LoginLockedTemporary
            End If
            newRow(44) = isInherited
            authorizations.Rows.Add(newRow)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create an empty datatable for output
        ''' </summary>
        ''' <param name="tableName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function AuthorizationsTableCreate(ByVal tableName As String) As DataTable

            Dim Result As New DataTable(tableName)
            Result.Columns.Add(New DataColumn("SecurityObject_ID", GetType(Integer)))
            Result.Columns.Add(New DataColumn("SecurityObject_Name", GetType(String)))
            Result.Columns.Add(New DataColumn("SecurityObject_DisplayName", GetType(String)))
            Result.Columns.Add(New DataColumn("SecurityObject_InheritFrom_SecurityObjectID", GetType(Integer)))
            Result.Columns.Add(New DataColumn("SecurityObject_SystemType", GetType(Integer)))
            Result.Columns.Add(New DataColumn("SecurityObject_Disabled", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("SecurityObject_Deleted", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("Group_ID", GetType(Integer)))
            Result.Columns.Add(New DataColumn("Group_Name", GetType(String)))
            Result.Columns.Add(New DataColumn("Group_Description", GetType(String)))
            Result.Columns.Add(New DataColumn("Group_IsSystemGroup", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_ID", GetType(Long)))
            Result.Columns.Add(New DataColumn("User_LoginName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_EMailAddress", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Gender", GetType(String)))
            Result.Columns.Add(New DataColumn("User_AcademicTitle", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FirstName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_LastName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_NameAddition", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FullName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_SalutationNameOnly", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Company", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Position", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Street", GetType(String)))
            Result.Columns.Add(New DataColumn("User_ZipCode", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Location", GetType(String)))
            Result.Columns.Add(New DataColumn("User_State", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Country", GetType(String)))
            Result.Columns.Add(New DataColumn("User_PhoneNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_MobileNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FaxNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage1", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage2", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage3", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_LoginDisabled", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginDeleted", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginLockedTemporaryTill", GetType(DateTime)))
            Result.Columns.Add(New DataColumn("User_IsSystemUser", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_ExternalAccount", GetType(String)))
            Result.Columns.Add(New DataColumn("User_AccessLevel", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_AccountAuthorizationsAlreadySet", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AccountProfileValidatedByEMailTest", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AutomaticLogonAllowedByMachineToMachineCommunication", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AdditionalFlags", GetType(String)))
            Result.Columns.Add(New DataColumn("IsInherited", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginLockedTemporary", GetType(Boolean)))
            Return Result
        End Function

#End Region

#Region "Users"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A table with all users of the requested user group
        ''' </summary>
        ''' <param name="webmanager">An instance of a valid camm Web-Manager</param>
        ''' <param name="userInfos">The requested users</param>
        ''' <returns>A data table containing groups and users information</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function Users(ByVal webmanager As WMSystem, ByVal userInfos As CompuMaster.camm.WebManager.WMSystem.UserInformation()) As DataTable

            Return Users(webmanager, userInfos, True)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A table with active/ all users of the requested user group
        ''' </summary>
        ''' <param name="webmanager"></param>
        ''' <param name="userInfos"></param>
        ''' <param name="includeDisabledUsers"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[zeutzheim]	24.05.2012	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function Users(ByVal webmanager As WMSystem, ByVal userInfos As CompuMaster.camm.WebManager.WMSystem.UserInformation(), ByVal includeDisabledUsers As Boolean) As DataTable

            Dim Result As DataTable = UsersTableCreate("members")

            If Not userInfos Is Nothing Then
                For MyCounter As Integer = 0 To userInfos.Length - 1
                    If includeDisabledUsers = False AndAlso userInfos(MyCounter).LoginDisabled Then
                        'don't add a row with disabled user
                    Else
                        Dim newRow As DataRow = Result.NewRow
                        UsersTableAddRow(Result, userInfos(MyCounter))
                    End If
                Next
            End If

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Attach one line to the output table
        ''' </summary>
        ''' <param name="users">The output table</param>
        ''' <param name="userInfo">One user information object</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub UsersTableAddRow(ByVal users As DataTable, ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation)

            'parameter validation
            If userInfo Is Nothing Then
                Throw New ArgumentNullException("userInfo")
            End If

            'add the new row
            Dim newRow As DataRow = users.NewRow
            If Not userInfo Is Nothing Then
                newRow(0) = userInfo.IDLong
                newRow(1) = userInfo.LoginName
                newRow(2) = userInfo.EMailAddress
                newRow(3) = userInfo.Gender.ToString
                newRow(4) = userInfo.AcademicTitle
                newRow(5) = userInfo.FirstName
                newRow(6) = userInfo.LastName
                newRow(7) = userInfo.NameAddition
                newRow(8) = userInfo.FullName
                newRow(9) = userInfo.SalutationNameOnly
                newRow(10) = userInfo.Company
                newRow(11) = userInfo.Position
                newRow(12) = userInfo.Street
                newRow(13) = userInfo.ZipCode
                newRow(14) = userInfo.Location
                newRow(15) = userInfo.State
                newRow(16) = userInfo.Country
                newRow(17) = userInfo.PhoneNumber
                newRow(18) = userInfo.MobileNumber
                newRow(19) = userInfo.FaxNumber
                newRow(20) = userInfo.PreferredLanguage1.ID
                newRow(21) = userInfo.PreferredLanguage2.ID
                newRow(22) = userInfo.PreferredLanguage3.ID
                newRow(23) = userInfo.LoginDisabled
                newRow(24) = userInfo.LoginDeleted
                newRow(25) = userInfo.LoginLockedTemporaryTill
                newRow(26) = userInfo.IsSystemUser()
                newRow(27) = userInfo.ExternalAccount()
                newRow(28) = userInfo.AccessLevel.ID
                newRow(29) = userInfo.AccountAuthorizationsAlreadySet()
                newRow(30) = userInfo.AccountProfileValidatedByEMailTest()
                newRow(31) = userInfo.AutomaticLogonAllowedByMachineToMachineCommunication()
                newRow(32) = CompuMaster.camm.WebManager.Utils.JoinNameValueCollectionWithUrlEncodingToString(userInfo.AdditionalFlags)
                newRow(33) = userInfo.LoginLockedTemporary
            End If
            users.Rows.Add(newRow)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create an empty datatable for output
        ''' </summary>
        ''' <param name="tableName"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	16.08.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function UsersTableCreate(ByVal tableName As String) As DataTable

            Dim Result As New DataTable(tableName)
            Result.Columns.Add(New DataColumn("User_ID", GetType(Long)))
            Result.Columns.Add(New DataColumn("User_LoginName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_EMailAddress", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Gender", GetType(String)))
            Result.Columns.Add(New DataColumn("User_AcademicTitle", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FirstName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_LastName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_NameAddition", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FullName", GetType(String)))
            Result.Columns.Add(New DataColumn("User_SalutationNameOnly", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Company", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Position", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Street", GetType(String)))
            Result.Columns.Add(New DataColumn("User_ZipCode", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Location", GetType(String)))
            Result.Columns.Add(New DataColumn("User_State", GetType(String)))
            Result.Columns.Add(New DataColumn("User_Country", GetType(String)))
            Result.Columns.Add(New DataColumn("User_PhoneNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_MobileNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_FaxNumber", GetType(String)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage1", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage2", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_PreferredLanguage3", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_LoginDisabled", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginDeleted", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_LoginLockedTemporaryTill", GetType(DateTime)))
            Result.Columns.Add(New DataColumn("User_IsSystemUser", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_ExternalAccount", GetType(String)))
            Result.Columns.Add(New DataColumn("User_AccessLevel", GetType(Integer)))
            Result.Columns.Add(New DataColumn("User_AccountAuthorizationsAlreadySet", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AccountProfileValidatedByEMailTest", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AutomaticLogonAllowedByMachineToMachineCommunication", GetType(Boolean)))
            Result.Columns.Add(New DataColumn("User_AdditionalFlags", GetType(String)))
            Result.Columns.Add(New DataColumn("User_LoginLockedTemporary", GetType(Boolean)))
            Return Result
        End Function

#End Region

    End Class

End Namespace