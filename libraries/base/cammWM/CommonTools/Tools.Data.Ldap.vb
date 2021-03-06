'Copyright 2005-2008,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie k�nnen es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder sp�teren ver�ffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es n�tzlich sein wird, aber OHNE JEDE GEW�HRLEISTUNG, bereitgestellt; sogar ohne die implizite Gew�hrleistung der MARKTF�HIGKEIT oder EIGNUNG F�R EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License f�r weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden f�r Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Explicit On
Option Strict On

'Entkoppelt von Ursprungs-Version durch ge�nderten Namespace

Namespace CompuMaster.camm.WebManager.Tools.Data

    ''' <summary>
    '''     LDAP access to retrieve data
    ''' </summary>
    Friend Class Ldap

        ''' <summary>
        '''     Returns different information on all the users matching the filter expression within the given domain as contents of a DataTable
        ''' </summary>
        ''' <remarks>
        '''     The Table contains the following columns:
        '''     - UserName      User's accountname
        '''     - FirstName     First name
        '''     - LastName      Last name
        '''     - DiplayName    Diplayed name
        '''     - Title         Position
        '''     - EMail         E-Mail address
        '''     - Phone         Phone number
        '''     - MobilePhone   Cell / mobile phone number
        '''     - VoIPPhone     VoIP phone number
        '''     - Street        Street and house number
        '''     - ZIP           Zip / postal code
        '''     - City          City name
        '''     - Country       Country name
        '''     - Company       Company name
        '''     - Department    Department name
        '''     - Initials      The initials of the user
        '''
        '''     Note that any field except "UserName" is optional.
        '''     All fields are of type String.
        '''     Each user account is represented by a DataRow.
        ''' </remarks>
        ''' <param name="domain">The domain from which to gather the information</param>
        ''' <param name="SearchFilterExpression">The filter expression for specific selection purposes.
        '''             For valid filter expressions see the documentation about
        '''             System.DirectoryServices.DirectorySearcher.Filter</param>
        ''' <returns>A DataTable containing the information, Nothing if an error occurs during execution</returns>
        Public Shared Function QueryUsers(ByVal domain As String, ByVal SearchFilterExpression As String) As DataTable
            Dim UserDataTable As DataTable = QueryUsers_CreateInfoTable(domain)
            Try
                ' User-Eintr�ge abfragen
                Dim MyDirEntry As New System.DirectoryServices.DirectoryEntry("LDAP://" & domain)
                Dim MyDirSearcher As New System.DirectoryServices.DirectorySearcher(MyDirEntry)
                If SearchFilterExpression = Nothing Then
                    ' Alle Benutzer anzeigen falls Filter leer ist
                    MyDirSearcher.Filter = "(objectCategory=user)"
                Else
                    ' Ansonsten den Filter verwenden
                    MyDirSearcher.Filter = SearchFilterExpression
                End If
                Dim MySearchResults As System.DirectoryServices.SearchResultCollection = MyDirSearcher.FindAll()
                Try
                    For counter As Integer = 0 To (MySearchResults.Count - 1)
                        Dim MyUserEntry As System.DirectoryServices.DirectoryEntry = MySearchResults.Item(counter).GetDirectoryEntry()
                        If (Not IsNothing(MyUserEntry)) Then
                            ' Daten des aktuellen Users abfragen
                            Dim _username As String = Convert.ToString(MyUserEntry.Properties.Item("sAMAccountName").Value)
                            Dim _firstname As String = Convert.ToString(MyUserEntry.Properties.Item("GivenName").Value)
                            Dim _lastname As String = Convert.ToString(MyUserEntry.Properties.Item("sn").Value)
                            Dim _displayname As String = Convert.ToString(MyUserEntry.Properties.Item("displayName").Value)
                            Dim _title As String = Convert.ToString(MyUserEntry.Properties.Item("title").Value)
                            Dim _email As String = Convert.ToString(MyUserEntry.Properties.Item("mail").Value)
                            Dim _phone As String = Convert.ToString(MyUserEntry.Properties.Item("telephoneNumber").Value)
                            Dim _mobilephone As String = Convert.ToString(MyUserEntry.Properties.Item("mobile").Value)
                            Dim _voipphone As String = Convert.ToString(MyUserEntry.Properties.Item("ipPhone").Value)
                            Dim _fax As String = Convert.ToString(MyUserEntry.Properties.Item("facsimileTelephoneNumber").Value)
                            Dim _street As String = Convert.ToString(MyUserEntry.Properties.Item("streetAddress").Value)
                            Dim _zip As String = Convert.ToString(MyUserEntry.Properties.Item("postalCode").Value)
                            Dim _city As String = Convert.ToString(MyUserEntry.Properties.Item("l").Value)
                            Dim _country As String = Convert.ToString(MyUserEntry.Properties.Item("co").Value)
                            Dim _company As String = Convert.ToString(MyUserEntry.Properties.Item("company").Value)
                            Dim _department As String = Convert.ToString(MyUserEntry.Properties.Item("department").Value)
                            Dim _initials As String = Convert.ToString(MyUserEntry.Properties.Item("initials").Value)

                            ' Neue DataRow mit den Daten bef�llen und zur Table hinzuf�gen
                            Dim CurrUserRow As DataRow = UserDataTable.NewRow()
                            CurrUserRow.Item("UserName") = _username
                            CurrUserRow.Item("FirstName") = _firstname
                            CurrUserRow.Item("LastName") = _lastname
                            CurrUserRow.Item("DisplayName") = _displayname
                            CurrUserRow.Item("Title") = _title
                            CurrUserRow.Item("EMail") = _email
                            CurrUserRow.Item("Phone") = _phone
                            CurrUserRow.Item("Fax") = _fax
                            CurrUserRow.Item("MobilePhone") = _mobilephone
                            CurrUserRow.Item("IPPhone") = _voipphone
                            CurrUserRow.Item("Street") = _street
                            CurrUserRow.Item("ZipCode") = _zip
                            CurrUserRow.Item("City") = _city
                            CurrUserRow.Item("Country") = _country
                            CurrUserRow.Item("Company") = _company
                            CurrUserRow.Item("Department") = _department
                            CurrUserRow.Item("Initials") = _initials
                            UserDataTable.Rows.Add(CurrUserRow)
                        End If
                    Next
                Finally
                    If Not MyDirEntry Is Nothing Then
                        MyDirEntry.Close()
                        MyDirEntry.Dispose()
                    End If
                    MyDirSearcher.Dispose()
                End Try
                Return UserDataTable
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        '''     Returns different information on all the users with the given account
        '''     name within the given domain as contents of a DataTable
        ''' </summary>
        ''' <remarks>
        '''     The Table contains the following columns:
        '''     - UserName      User's accountname
        '''     - FirstName     First name
        '''     - LastName      Last name
        '''     - DiplayName    Diplayed name
        '''     - Title         Position
        '''     - EMail         E-Mail address
        '''     - Phone         Phone number
        '''     - MobilePhone   Cell / mobile phone number
        '''     - VoIPPhone     VoIP phone number
        '''     - Street        Street and house number
        '''     - ZIP           Zip / postal code
        '''     - City          City name
        '''     - Country       Country name
        '''     - Company       Company name
        '''     - Department    Department name
        '''     - Initials      The initials of the user
        '''
        '''     Note that any field except "UserName" is optional.
        '''     All fields are of type String.
        '''     Each user account is represented by a DataRow.
        '''     
        ''' </remarks>
        ''' <param name="domain">The domain from which to gather the information</param>
        ''' <param name="UserAccountName">The account name for which to search</param>
        ''' <returns>A DataTable containing the information, Nothing if an error occurs during execution</returns>
        Public Shared Function QueryUsersByAccountName(ByVal domain As String, ByVal UserAccountName As String) As DataTable
            If UserAccountName = Nothing Then
                Return Nothing
            Else
                Return QueryUsers(domain, "(&(objectCategory=user)(sAMAccountName=" & UserAccountName & "))")
            End If
        End Function

        ''' <summary>
        '''     Returns different information on all the users with the given first
        '''     and / or last name within the given domain as contents of a DataTable
        ''' </summary>
        ''' <remarks>
        '''     The Table contains the following columns:
        '''     - UserName      User's accountname
        '''     - FirstName     First name
        '''     - LastName      Last name
        '''     - DiplayName    Diplayed name
        '''     - Title         Position
        '''     - EMail         E-Mail address
        '''     - Phone         Phone number
        '''     - MobilePhone   Cell / mobile phone number
        '''     - VoIPPhone     VoIP phone number
        '''     - Street        Street and house number
        '''     - ZIP           Zip / postal code
        '''     - City          City name
        '''     - Country       Country name
        '''     - Company       Company name
        '''     - Department    Department name
        '''     - Initials      The initials of the user
        '''
        '''     Note that any field except "UserName" is optional.
        '''     All fields are of type String.
        '''     Each user account is represented by a DataRow.
        '''     
        ''' </remarks>
        ''' <param name="domain">The domain from which to gather the information</param>
        ''' <param name="UserFirstName">The first name for which to search (may be empty or nothing if last name is given)</param>
        ''' <param name="UserLastName">The last name for which to search (may be empty or nothing if first name is given)</param>
        ''' <returns>A DataTable containing the information, Nothing if an error occurs during execution</returns>
        Public Shared Function QueryUsersByName(ByVal domain As String, ByVal UserFirstName As String, ByVal UserLastName As String) As DataTable
            If ((IsNothing(UserFirstName) OrElse (UserFirstName.Trim() = "")) AndAlso (IsNothing(UserLastName) OrElse (UserLastName.Trim() = ""))) Then
                Return Nothing
            Else
                If (IsNothing(UserFirstName) OrElse (UserFirstName.Trim() = "")) Then
                    ' First name is empty --> Query only by last name
                    Return QueryUsers(domain, "(&(objectCategory=user)(sn=" & UserLastName & "))")
                Else
                    If (IsNothing(UserLastName) OrElse (UserLastName.Trim() = "")) Then
                        ' Last name is empty --> Query only by first name
                        Return QueryUsers(domain, "(&(objectCategory=user)(GivenName=" & UserFirstName & "))")
                    Else
                        ' Both parameters not empty --> Query user by first and last name
                        Return QueryUsers(domain, "(&(objectCategory=user)(GivenName=" & UserFirstName & ")(sn=" & UserLastName & "))")
                    End If
                End If
            End If
        End Function

        ''' <summary>
        '''     Query the LDAP
        ''' </summary>
        ''' <param name="domain">The domain name which will be used as LDAP server name (to query the domain controller)</param>
        ''' <param name="searchFilterExpression">A search expression to filter the results</param>
        ''' <returns>A datatable containing all data as strings</returns>
        Public Shared Function Query(ByVal domain As String, ByVal searchFilterExpression As String) As DataTable

            ' User-Eintr�ge abfragen
            Dim MyDirEntry As New System.DirectoryServices.DirectoryEntry("LDAP://" & domain)
            Dim MyDirSearcher As New System.DirectoryServices.DirectorySearcher(MyDirEntry)
            If searchFilterExpression = Nothing Then
                ' Alle Benutzer anzeigen falls Filter leer ist
                MyDirSearcher.Filter = String.Empty
            Else
                ' Ansonsten den Filter verwenden
                MyDirSearcher.Filter = searchFilterExpression
            End If
            'Create return table
            Dim Result As New DataTable
            Dim MySearchResults As System.DirectoryServices.SearchResultCollection = MyDirSearcher.FindAll()
            Try
                For counter As Integer = 0 To (MySearchResults.Count - 1)
                    Dim MyUserEntry As System.DirectoryServices.DirectoryEntry = MySearchResults.Item(counter).GetDirectoryEntry()
                    Dim MyRow As DataRow = Result.NewRow
                    For Each PropertyName As String In MyUserEntry.Properties.PropertyNames
                        'Add missing columns (when something is missing)
                        If Result.Columns.Contains(PropertyName) = False Then
                            Result.Columns.Add(PropertyName, GetType(String))
                        End If
                        'Assign values
                        If MyUserEntry.Properties.Item(PropertyName).Value Is Nothing Then
                            MyRow(PropertyName) = DBNull.Value
                        Else
                            MyRow(PropertyName) = CType(MyUserEntry.Properties.Item(PropertyName).Value, Object).ToString
                        End If
                    Next
                    Result.Rows.Add(MyRow)
                Next
            Finally
                MyDirEntry.Close()
                MyDirEntry.Dispose()
                MyDirSearcher.Dispose()
            End Try
            Return Result
        End Function


        ''' <summary>
        '''     Returns different information on all users within the given domain
        '''     as contents of a DataTable
        ''' </summary>
        ''' <remarks>
        '''     The Table contains the following columns:
        '''     - UserName      User's accountname
        '''     - FirstName     First name
        '''     - LastName      Last name
        '''     - DiplayName    Diplayed name
        '''     - Title         Position
        '''     - EMail         E-Mail address
        '''     - Phone         Phone number
        '''     - MobilePhone   Cell / mobile phone number
        '''     - VoIPPhone     VoIP phone number
        '''     - Street        Street and house number
        '''     - ZIP           Zip / postal code
        '''     - City          City name
        '''     - Country       Country name
        '''     - Company       Company name
        '''     - Department    Department name
        '''     - Initials      The initials of the user
        '''
        '''     Note that any field except "UserName" is optional.
        '''     All fields are of type String.
        '''     Each user account is represented by a DataRow.
        '''     
        ''' </remarks>
        ''' <param name="domain">The domain from which to gather the information</param>
        ''' <returns>A DataTable containing the information, Nothing if an error occurs during execution</returns>
        Public Shared Function QueryAllUsers(ByVal domain As String) As DataTable
            Return QueryUsers(domain, "(objectCategory=user)")
        End Function

        ''' <summary>
        '''     Creates a DataTable with all the required columns for containing
        '''     user information
        ''' </summary>
        ''' <param name="DomName">The domain name to be included in the table's title</param>
        ''' <returns>An empty DataTable containing the prepared columns</returns>
        Private Shared Function QueryUsers_CreateInfoTable(ByVal DomName As String) As DataTable
            ' Neue DataTable mit den ben�tigten Columns erzeugen
            Dim MyTable As New DataTable("UserInformation")
            MyTable.Columns.Add("UserName", System.Type.GetType("System.String"))
            MyTable.Columns.Add("FirstName", System.Type.GetType("System.String"))
            MyTable.Columns.Add("LastName", System.Type.GetType("System.String"))
            MyTable.Columns.Add("DisplayName", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Title", System.Type.GetType("System.String"))
            MyTable.Columns.Add("EMail", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Phone", System.Type.GetType("System.String"))
            MyTable.Columns.Add("MobilePhone", System.Type.GetType("System.String"))
            MyTable.Columns.Add("IPPhone", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Fax", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Street", System.Type.GetType("System.String"))
            MyTable.Columns.Add("ZipCode", System.Type.GetType("System.String"))
            MyTable.Columns.Add("City", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Country", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Company", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Department", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Initials", System.Type.GetType("System.String"))
            MyTable.Columns.Add("Manager", System.Type.GetType("System.String"))
            Return MyTable
        End Function

    End Class

End Namespace