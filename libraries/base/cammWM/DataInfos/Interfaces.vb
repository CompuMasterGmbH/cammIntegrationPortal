'Copyright 2006-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

    Public Interface IUserInformation

        'System reference
        Property WebManager() As IWebManager

        Enum GenderType
            Undefined = Sex.Undefined
            Masculine = Sex.Masculine
            Feminine = Sex.Feminine
            <Obsolete("Use value Masculine instead", False), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Masculin = Sex.Masculine
            <Obsolete("Use value Feminine instead", False), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Feminin = Sex.Feminine
            MissingNameOrGroupOfPersons = Sex.MissingNameOrGroupOfPersons
        End Enum

        'Must or very important fields
        ReadOnly Property ID() As Long
        Property LoginName() As String
        Property EMailAddress() As String
        Property ExternalAccount() As String
        Property FirstName() As String
        Property LastName() As String
        Property Gender() As GenderType

        'Optional fields
        Property Position() As String
        Property Company() As String
        Property AcademicTitle() As String
        Property NameAddition() As String
        Property Street() As String
        Property ZipCode() As String
        Property Location() As String
        Property State() As String
        Property Country() As String
        Property FaxNumber() As String
        Property PhoneNumber() As String
        Property MobileNumber() As String

        'Additional, custom fields
        Property AdditionalFlags() As Collections.Specialized.NameValueCollection

    End Interface

    Public Interface IGroupInformation

    End Interface

    Public Interface ISecurityObjectInformation

    End Interface

    Public Interface IAuthorizationInformation

    End Interface

    Public Interface IUserAuthorizationInformation

    End Interface

    Public Interface IGroupAuthorizationInformation

    End Interface

    Public Interface IServerInformation

    End Interface

    Public Interface IServerGroupInformation

    End Interface

    Public Interface ILanguageInformation

    End Interface

End Namespace