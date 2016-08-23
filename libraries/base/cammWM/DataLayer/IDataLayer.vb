'Copyright 2014-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager

    Public Interface IDataLayer

        '<Obsolete("STRONGLY RECOMMENDED: Use RemoveUserAuthorization with additional parameters")> _
        Sub RemoveUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer)
        Sub RemoveUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
        '<Obsolete("STRONGLY RECOMMENDED: Use RemoveGroupAuthorization with additional parameters")> _
        Sub RemoveGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer)
        Sub RemoveGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
        '<Obsolete("STRONGLY RECOMMENDED: Use AddGroupAuthorization with additional parameters")> _
        '<Obsolete("STRONGLY RECOMMENDED: Use AddGroupAuthorization with additional parameters")> Sub AddGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer)
        Sub AddGroupAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal groupID As Integer, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)

        'Still implemented in WMSystem.UserInformation because of some difficult stuff/magic inside
        '<Obsolete("STRONGLY RECOMMENDED: Use AddUserAuthorization with additional parameters")> Sub AddUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer)
        'Sub AddUserAuthorization(ByVal webmanager As IWebManager, ByVal securityObjectID As Integer, ByVal userID As Long, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
        ''' <summary>
        ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
        ''' </summary>
        ''' <param name="securityObjectID">The security object ID</param>
        ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
        ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
        ''' <param name="isDenyRule">True for a deny rule or False for a grant access rule</param>
        ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
        ''' <remarks>This action will be done immediately without the need for saving</remarks>
        Sub AddUserAuthorization(webmanager As WMSystem, dbConnection As IDbConnection, ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, userInfo As WMSystem.UserInformation, userID As Long, ByVal developerAuthorization As Boolean, isDenyRule As Boolean, modifyingUserID As Long, Optional ByVal notifications As Notifications.INotifications = Nothing)

        ''' <summary>
        '''     Set a user profile setting
        ''' </summary>
        ''' <param name="webManager">A valid instance of camm Web-Manager</param>
        ''' <param name="dbConnection">An open connection which shall be used or nothing if a new one shall be created independently and on the fly</param>
        ''' <param name="userID">The ID of the user who shall receive the updated value</param>
        ''' <param name="propertyName">The key name of the flag</param>
        ''' <param name="value">The new value of the flag</param>
        ''' <param name="doNotLogSuccess">False will lead to an informational log entry in the database after the value has been saved; in case of True there won't be created a log entry</param>
        Function SetUserDetail(ByVal webManager As IWebManager, ByVal dbConnection As IDbConnection, ByVal userID As Long, ByVal propertyName As String, ByVal value As String, Optional ByVal doNotLogSuccess As Boolean = False) As Boolean

        ''' <summary>
        '''     Reads a single user detail value from the database
        ''' </summary>
        ''' <param name="webManager">A reference to a camm Web-Manager instance</param>
        ''' <param name="userID">The user ID</param>
        ''' <param name="propertyName">The requested property name</param>
        ''' <returns>The resulting value as a String</returns>
        Function GetUserDetail(ByVal webManager As IWebManager, ByVal userID As Long, ByVal propertyName As String) As String

        ''' <summary>
        '''     Get a string with all logon servers for a user 
        ''' </summary>
        ''' <param name="webManager">An instance of camm Web-Manager</param>
        ''' <param name="userID">A user ID</param>
        ''' <returns>A string with all relative server groups; every server group is placed in a new line.</returns>
        ''' <remarks>
        '''     If there is only 1 server group available, the returned string contains only the simply URL of the master server of this server group.
        '''     Are there 2 or more server groups available then each URL of the corresponding master server is followed by the server group title in parenthesis.
        ''' </remarks>
        Function GetUserLogonServers(ByVal webManager As IWebManager, ByVal userID As Long) As String

        ''' <summary>
        ''' Get a list of all userIDs by additional flag
        ''' </summary>
        ''' <param name="FlagName"></param>
        ''' <param name="webmanager"></param>
        ''' <returns>All userIDs by additional flag</returns>
        Function ListOfUsersByAdditionalFlag(ByVal FlagName As String, ByVal webmanager As IWebManager) As Long()

        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>All flag names which are used in the user profiles</returns>
        Function ListOfAdditionalFlagsInUseByUserProfiles(ByVal webmanager As IWebManager) As String()

        ''' <summary>
        ''' Get the list of additional flags which are in use by at least one user profile
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashlist with the flag name as key and the count of occurances as the value</returns>
        Function ListOfAdditionalFlagsInUseByUserProfilesWithCount(ByVal webmanager As IWebManager) As Hashtable

        ''' <summary>
        ''' Get the list of additional flags which are required by the security objects
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        Function ListOfAddtionalFlagsRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String()

        ''' <summary>
        ''' Get the list of additional flags which are not required by the security objects
        ''' </summary>
        ''' <param name="webmanager"></param>
        Function ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(ByVal webmanager As IWebManager) As String()

        ''' <summary>
        ''' Copy authorizations from one security object to another one (without creating duplicates if they already exist)
        ''' </summary>
        ''' <param name="webmanager">A valid web-manager instance</param>
        ''' <param name="sourceSecurityObjectID">The security object ID which shall be the source</param>
        ''' <param name="destinationSecurityObjectID">The ID of the security ojbect which shall receive the additional authorizations</param>
        ''' <remarks>
        ''' Only missing authorizations will be copied to the destination security object.
        ''' </remarks>
        Sub CopyAuthorizations(ByVal webmanager As IWebManager, ByVal sourceSecurityObjectID As Integer, ByVal destinationSecurityObjectID As Integer)

        Function SaveSecurityObject(webmanager As IWebManager, dbConnection As IDbConnection, securityObject As WMSystem.SecurityObjectInformation, modifyingUserID As Long) As Integer

        ''' <summary>
        ''' Create an appropriate log entry for an external, not-yet-assigned user account
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        ''' <param name="fullUserName">The complete name of the user, e. g. &quot;Dr. Bill Wilson&quot;</param>
        Sub AddMissingExternalAccountAssignment(ByVal webmanager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String, ByVal fullUserName As String, ByVal emailAddress As String, ByVal errorDetails As String)

        ''' <summary>
        ''' Remove an existing log entry of an external account which is successfully assigned, now
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <param name="externalAccountSystemName">The name of an external account system, e. g. &quot;MS ADS&quot;</param>
        ''' <param name="fullUserLogonName">The full logon name, e. g. &quot;YOUR-COMPANY\billwilson&quot;</param>
        Sub RemoveMissingExternalAccountAssignment(ByVal webmanager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String)

        ''' <summary>
        ''' Query a list of existing user IDs
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        Function ActiveUsers(ByVal webmanager As IWebManager) As Long()

        ''' <summary>
        ''' Query a list of user IDs from existing plus deleted users
        ''' </summary>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns>A hashtable containing the user ID as key field (Int64) and the status &quot;Deleted&quot; as a boolean value in the hashtable's value field (true indicates a deleted user)</returns>
        Function ActiveAndDeletedUsers(ByVal webmanager As IWebManager) As Hashtable

        Function QueryLastServiceExecutionDate(webmanager As IWebManager, dbConnection As IDbConnection) As Date
        Sub SaveLastServiceExecutionDate(webmanager As IWebManager, dbConnection As IDbConnection, triggerServiceVersion As String)

    End Interface

End Namespace