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

    ''' <summary>
    '''     Required for NUnit tests only to make requried methods accessable
    ''' </summary>
    Friend Class TestLayer
        Public Shared Function ActiveAndDeletedUsers(ByVal WebManager As IWebManager) As Hashtable
            Return CompuMaster.camm.WebManager.DataLayer.Current.ActiveAndDeletedUsers(WebManager)
        End Function
        Public Shared Function ActiveUsers(ByVal WebManager As IWebManager) As Long()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ActiveUsers(WebManager)
        End Function
        Public Shared Sub AddMissingExternalAccountAssignment(ByVal WebManager As IWebManager, ByVal externalAccountSystemName As String, ByVal fullUserLogonName As String, ByVal fullUserName As String, ByVal emailAdress As String, ByVal errorDetails As String)
            DataLayer.Current.AddMissingExternalAccountAssignment(WebManager, externalAccountSystemName, fullUserLogonName, fullUserName, emailAdress, errorDetails)
        End Sub
        Public Shared Sub CopyAuthorizations(ByVal WebManager As IWebManager, ByVal sourceSecurityObjectID As Integer, ByVal destinationSecurityObjectID As Integer)
            DataLayer.Current.CopyAuthorizations(WebManager, sourceSecurityObjectID, destinationSecurityObjectID)
        End Sub
        Public Shared Function GetUserDetail(ByVal WebManager As IWebManager, ByVal userID As Long, ByVal propertyName As String) As String
            Return CompuMaster.camm.WebManager.DataLayer.Current.GetUserDetail(WebManager, userID, propertyName)
        End Function
        Public Shared Function GetUserLogonServers(ByVal WebManager As IWebManager, ByVal userID As Long) As String
            Return CompuMaster.camm.WebManager.DataLayer.Current.GetUserLogonServers(WebManager, userID)
        End Function
        Public Shared Function ListOfUsersByAdditionalFlag(ByVal FlagName As String, ByVal WebManager As IWebManager) As Long()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfUsersByAdditionalFlag(FlagName, WebManager)
        End Function
        Public Shared Function ListOfAdditionalFlagsInUseByUserProfiles(ByVal WebManager As IWebManager) As String()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfiles(WebManager)
        End Function
        Public Shared Function ListOfAdditionalFlagsInUseByUserProfilesWithCount(ByVal WebManager As IWebManager) As Hashtable
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfilesWithCount(WebManager)
        End Function
        Public Shared Function ListOfAddtionalFlagsRequiredBySecurityObjects(ByVal WebManager As IWebManager) As String()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAddtionalFlagsRequiredBySecurityObjects(WebManager)
        End Function
        Public Shared Function ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(ByVal WebManager As IWebManager) As String()
            Return CompuMaster.camm.WebManager.DataLayer.Current.ListOfAdditionalFlagsInUseByUserProfilesNotRequiredBySecurityObjects(WebManager)
        End Function
        Public Shared Sub RemoveMissingExternalAccountAssignment(ByVal WebManager As IWebManager, ByVal externalaccountsystemname As String, ByVal fullUserLogonName As String)
            CompuMaster.camm.WebManager.DataLayer.Current.RemoveMissingExternalAccountAssignment(WebManager, externalaccountsystemname, fullUserLogonName)
        End Sub
        Public Shared Function SetUserDetail(ByVal WebManager As IWebManager, ByVal dbConnection As IDbConnection, ByVal userID As Long, ByVal propertyName As String, ByVal value As String, Optional ByVal doNotLogSuccess As Boolean = False) As Boolean
            Return CompuMaster.camm.WebManager.DataLayer.Current.SetUserDetail(WebManager, dbConnection, userID, propertyName, value, doNotLogSuccess)
        End Function
    End Class

End Namespace