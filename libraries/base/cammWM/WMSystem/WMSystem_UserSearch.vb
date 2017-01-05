'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

    Partial Public Class WMSystem

        ''' <summary>
        '''     Filter credentials for searching for users
        ''' </summary>
        Public Class UserFilter

            Public Sub New(ByVal propertyName As String, ByVal searchMethod As SearchMethods)
                Me.New(propertyName, searchMethod, Nothing)
            End Sub

            Public Sub New(ByVal propertyName As String, ByVal searchMethod As SearchMethods, ByVal matchExpressions As String())
                Me.PropertyName = propertyName
                Me.SearchMethod = searchMethod
                Me.MatchExpressions = matchExpressions
            End Sub

            ''' <summary>
            '''     The name of a user property which shall be investigated
            ''' </summary>
            Public PropertyName As String

            ''' <summary>
            '''     Available search methods for filtering of users by their defined properties
            ''' </summary>
            Public Enum SearchMethods
                ''' <summary>
                '''     No filtering
                ''' </summary>
                ''' <remarks>
                '''     Only add this property to the list of queried properties to allow returnage or sorting on this value
                ''' </remarks>
                All = 0
                ''' <summary>
                '''     Value must exist (not DBNull)
                ''' </summary>
                Exist = 10
                ''' <summary>
                '''     Value is DBNull, respecitvely in case of a string it must be empty
                ''' </summary>
                IsEmpty = 20
                ''' <summary>
                '''     Value is equal to the searched string (regulary case in-sensitive)
                ''' </summary>
                MatchExactly = 30
                ''' <summary>
                '''     Value begins with the searched string (LIKE search)
                ''' </summary>
                MatchAtTheBeginning = 40
                ''' <summary>
                '''     Value isn't allowed to exist (is DBNull)
                ''' </summary>
                NotExist = 50
                ''' <summary>
                '''     All users which haven't got any of these strings
                ''' </summary>
                NoMatch = 60
            End Enum

            ''' <summary>
            '''     The search method
            ''' </summary>
            Public SearchMethod As SearchMethods

            ''' <summary>
            '''     A list of expression to search for matching search methods
            ''' </summary>
            ''' <remarks>
            '''     The elements of this list will be queried by a logical Or _
            ''' </remarks>
            Public MatchExpressions As String()

            ''' <summary>
            '''     Name of the column for this property name - if it is saved there
            ''' </summary>
            ''' <remarks>
            '''     Keep it empty to search in table Log_Users
            ''' </remarks>
            Friend UsersTableColumnName As String
            ''' <summary>
            '''     The data type in the users table if it's not a string
            ''' </summary>
            Friend UsersTableColumnType As String

        End Class

        ''' <summary>
        '''     Sort arguments for the user search
        ''' </summary>
        Public Class UserSortArgument

            Public Sub New(ByVal columnName As String)
                Me.New(columnName, Nothing)
            End Sub

            Public Sub New(ByVal columnName As String, ByVal directionDescending As Boolean)
                Me.ColumnName = columnName
                If directionDescending = True Then
                    Me.Direction = "DESC"
                Else
                    Me.Direction = ""
                End If
            End Sub

            ''' <summary>
            '''     The name of the column which shall be sorted
            ''' </summary>
            Public ColumnName As String

            ''' <summary>
            '''     ASC or DESC
            ''' </summary>
            Public Direction As String
        End Class

        ''' <summary>
        '''     Search for some users
        ''' </summary>
        ''' <param name="userFilter">An array of filter settings</param>
        ''' <param name="sortByPropertyName">The name of the properties to sort the resulting users by</param>
        ''' <remarks>
        '''     <para>The property names will be either recognized as a system property or used for search in the list of additional flags.</para>
        '''     <para>Property names of system properties of user profiles</para>
        '''     <list>
        '''     <item>id</item>
        '''     <item>company</item>
        '''     <item>loginname</item>
        '''     <item>emailaddress</item>
        '''     <item>firstname</item>
        '''     <item>lastname</item>
        '''     <item>academictitle</item>
        '''     <item>street</item>
        '''     <item>zipcode</item>
        '''     <item>location</item>
        '''     <item>state</item>
        '''     <item>country</item>
        '''     <item>preferredlanguage1id</item>
        '''     <item>preferredlanguage2id</item>
        '''     <item>preferredlanguage3id</item>
        '''     <item>nameaddition</item>
        '''     <item>logindisabled</item>
        '''     <item>loginlockedtill</item>
        '''     <item>accesslevelid</item>
        '''     </list>
        ''' </remarks>
        Public Function SearchUsers(ByVal userFilter As UserFilter(), ByVal sortByPropertyName As UserSortArgument()) As Long()
            Dim Users As DataTable = SearchUserData(Nothing, userFilter, sortByPropertyName)
            Return CType(CompuMaster.camm.WebManager.Tools.Data.DataTables.ConvertDataTableToArrayList(Users.Columns("ID")).ToArray(GetType(Long)), Long())
        End Function

        ''' <summary>
        '''     Search for some users and collect some data on them
        ''' </summary>
        ''' <param name="userFilter">An array of filter settings</param>
        ''' <param name="sortByPropertyName">The name of the properties to sort the resulting users by</param>
        ''' <remarks>
        '''     <para>Always returned is the ID value of the user</para>
        '''     <para>The property names will be either recognized as a system property or used for search in the list of additional flags.</para>
        '''     <para>Property names of system properties of user profiles</para>
        '''     <list>
        '''     <item>id</item>
        '''     <item>company</item>
        '''     <item>loginname</item>
        '''     <item>emailaddress</item>
        '''     <item>firstname</item>
        '''     <item>lastname</item>
        '''     <item>academictitle</item>
        '''     <item>street</item>
        '''     <item>zipcode</item>
        '''     <item>location</item>
        '''     <item>state</item>
        '''     <item>country</item>
        '''     <item>preferredlanguage1id</item>
        '''     <item>preferredlanguage2id</item>
        '''     <item>preferredlanguage3id</item>
        '''     <item>nameaddition</item>
        '''     <item>logindisabled</item>
        '''     <item>loginlockedtill</item>
        '''     <item>accesslevelid</item>
        '''     </list>
        ''' </remarks>
        Public Function SearchUserData(ByVal userFilter As UserFilter(), ByVal sortByPropertyName As UserSortArgument()) As DataTable
            Dim selects As New ArrayList
            If Not userFilter Is Nothing Then
                For MyCounter As Integer = 0 To userFilter.Length - 1
                    selects.Add(userFilter(MyCounter).PropertyName)
                Next
            End If
            Return SearchUserData(CType(selects.ToArray(GetType(String)), String()), userFilter, sortByPropertyName)
        End Function

        ''' <summary>
        '''     Search for some users and collect some data on them
        ''' </summary>
        ''' <param name="returnedProperties">An array of property names which shall be returned</param>
        ''' <param name="userFilter">An array of filter settings</param>
        ''' <param name="sortByPropertyName">The name of the properties to sort the resulting users by</param>
        ''' <remarks>
        '''     <para>Always returned is the ID value of the user</para>
        '''     <para>The property names will be either recognized as a system property or used for search in the list of additional flags.</para>
        '''     <para>Please note that the returned data table might contain the columns in a different order than you requested them by the returnedProperties</para>
        '''     <para>Property names of system properties of user profiles</para>
        '''     <list>
        '''     <item>id</item>
        '''     <item>company</item>
        '''     <item>loginname</item>
        '''     <item>emailaddress</item>
        '''     <item>firstname</item>
        '''     <item>lastname</item>
        '''     <item>academictitle</item>
        '''     <item>street</item>
        '''     <item>zipcode</item>
        '''     <item>location</item>
        '''     <item>state</item>
        '''     <item>country</item>
        '''     <item>preferredlanguage1id</item>
        '''     <item>preferredlanguage2id</item>
        '''     <item>preferredlanguage3id</item>
        '''     <item>nameaddition</item>
        '''     <item>logindisabled</item>
        '''     <item>loginlockedtill</item>
        '''     <item>accesslevelid</item>
        '''     </list>
        ''' </remarks>
        Public Function SearchUserData(ByVal returnedProperties As String(), ByVal userFilter As UserFilter(), ByVal sortByPropertyName As UserSortArgument()) As DataTable

            'Parameter check
            If userFilter Is Nothing AndAlso Not returnedProperties Is Nothing AndAlso returnedProperties.Length > 0 Then
                Throw New ArgumentNullException("Can't be null when returnedProperties isn't null", "userFilter")
            End If
            If userFilter Is Nothing AndAlso Not sortByPropertyName Is Nothing AndAlso sortByPropertyName.Length > 0 Then
                Throw New ArgumentNullException("Can't be null when sortByPropertyName isn't null", "userFilter")
            End If
            If Not returnedProperties Is Nothing Then
                For MyReturnedPropertiesCounter As Integer = 0 To returnedProperties.Length - 1
                    Dim ReturnValueFound As Boolean = False
                    For MyUserFilterCounter As Integer = 0 To userFilter.Length - 1
                        If returnedProperties(MyReturnedPropertiesCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture) = userFilter(MyUserFilterCounter).PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                            ReturnValueFound = True
                            Exit For
                        End If
                    Next
                    If returnedProperties(MyReturnedPropertiesCounter).ToLower = "id" Then
                        ReturnValueFound = True
                    End If
                    If ReturnValueFound = False Then
                        Throw New ArgumentException("All array elements must match one of the userFilter's PropertyName values", "returnedProperties")
                    End If
                Next
            End If
            If Not sortByPropertyName Is Nothing Then
                For MySortByPropertyNameCounter As Integer = 0 To sortByPropertyName.Length - 1
                    Dim SortByPropertyNameValueFound As Boolean = False
                    For MyUserFilterCounter As Integer = 0 To userFilter.Length - 1
                        If sortByPropertyName(MySortByPropertyNameCounter).ColumnName.ToLower(System.Globalization.CultureInfo.InvariantCulture) = userFilter(MyUserFilterCounter).PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                            SortByPropertyNameValueFound = True
                            Exit For
                        End If
                    Next
                    If sortByPropertyName(MySortByPropertyNameCounter).ColumnName.ToLower = "id" Then
                        SortByPropertyNameValueFound = True
                    End If
                    If SortByPropertyNameValueFound = False Then
                        Throw New ArgumentException("All array elements must match one of the userFilter's PropertyName values", "sortByPropertyName")
                    End If
                Next
            End If

            'Lookup the correct column name in database table Benutzer - if it's a column of that table
            If Not userFilter Is Nothing Then
                For FilterCounter As Integer = 0 To userFilter.Length - 1
                    Select Case userFilter(FilterCounter).PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                        Case "id"
                            userFilter(FilterCounter).UsersTableColumnName = "ID"
                            userFilter(FilterCounter).UsersTableColumnType = "int"
                        Case "company"
                            userFilter(FilterCounter).UsersTableColumnName = "Company"
                        Case "loginname"
                            userFilter(FilterCounter).UsersTableColumnName = "LoginName"
                        Case "emailaddress", "email", "e-mail"
                            userFilter(FilterCounter).UsersTableColumnName = "E-Mail"
                        Case "firstname"
                            userFilter(FilterCounter).UsersTableColumnName = "Vorname"
                        Case "lastname", "familyname"
                            userFilter(FilterCounter).UsersTableColumnName = "Nachname"
                        Case "academictitle"
                            userFilter(FilterCounter).UsersTableColumnName = "Titel"
                        Case "street"
                            userFilter(FilterCounter).UsersTableColumnName = "Strasse"
                        Case "zipcode"
                            userFilter(FilterCounter).UsersTableColumnName = "PLZ"
                        Case "location"
                            userFilter(FilterCounter).UsersTableColumnName = "Ort"
                        Case "state"
                            userFilter(FilterCounter).UsersTableColumnName = "State"
                        Case "country"
                            userFilter(FilterCounter).UsersTableColumnName = "Land"
                        Case "preferredlanguage1id"
                            userFilter(FilterCounter).UsersTableColumnName = "1stPreferredLanguage"
                            userFilter(FilterCounter).UsersTableColumnType = "int"
                        Case "preferredlanguage2id"
                            userFilter(FilterCounter).UsersTableColumnName = "2ndPreferredLanguage"
                            userFilter(FilterCounter).UsersTableColumnType = "int"
                        Case "preferredlanguage3id"
                            userFilter(FilterCounter).UsersTableColumnName = "3rdPreferredLanguage"
                            userFilter(FilterCounter).UsersTableColumnType = "int"
                        Case "nameaddition"
                            userFilter(FilterCounter).UsersTableColumnName = "Namenszusatz"
                        Case "logindisabled"
                            userFilter(FilterCounter).UsersTableColumnName = "LoginDisabled"
                            userFilter(FilterCounter).UsersTableColumnType = "bit"
                        Case "loginlockedtill"
                            userFilter(FilterCounter).UsersTableColumnName = "LoginLockedTill"
                            userFilter(FilterCounter).UsersTableColumnType = "datetime"
                        Case "accesslevelid"
                            userFilter(FilterCounter).UsersTableColumnName = "AccountAccessability"
                            userFilter(FilterCounter).UsersTableColumnType = "int"
                    End Select
                Next
            End If

            'Prepare the search command, exemplary:
            '   select ID_User, Value
            '   into #sex
            '   from dbo.Log_Users
            '   where Type = 'sex'
            '
            '   select id, min(loginname) as LoginName, min(nachname) as Nachname, min([#sex].value) as [Sex]
            '   from dbo.benutzer
            '   	left join #sex on dbo.benutzer.id = #sex.ID_User
            '   where dbo.benutzer.loginname like n'l%' and #sex.value = 'm'
            '   group by ID
            '   order by min(nachname) 
            '
            '   drop table #sex
            Dim sql As New System.Text.StringBuilder
            'Temporary tables
            If Not userFilter Is Nothing Then
                For MyCounter As Integer = 0 To userFilter.Length - 1
                    If userFilter(MyCounter).UsersTableColumnName = Nothing Then
                        sql.Append("SELECT ID_USER, Value")
                        sql.Append(vbNewLine)
                        sql.Append("INTO [#")
                        sql.Append(userFilter(MyCounter).PropertyName)
                        sql.Append("]")
                        sql.Append(vbNewLine)
                        sql.Append("FROM dbo.Log_Users")
                        sql.Append(vbNewLine)
                        sql.Append("WHERE Type = N'")
                        sql.Append(userFilter(MyCounter).PropertyName.Replace("'", "''"))
                        sql.Append("'")
                        sql.Append(vbNewLine)
                    End If
                Next
                sql.Append(vbNewLine)
            End If
            'Select
            sql.Append("SELECT ID")
            If Not returnedProperties Is Nothing Then
                For MyCounter As Integer = 0 To returnedProperties.Length - 1
                    If returnedProperties(MyCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture) <> "id" Then
                        'Find the appropriate UserFilter
                        Dim MyUserFilter As UserFilter = Nothing
                        For MyUserFilterCounter As Integer = 0 To userFilter.Length - 1
                            If returnedProperties(MyCounter).ToLower(System.Globalization.CultureInfo.InvariantCulture) = userFilter(MyUserFilterCounter).PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                                MyUserFilter = userFilter(MyUserFilterCounter)
                                Exit For
                            End If
                        Next
                        'Now create some SQL
                        If MyUserFilter.UsersTableColumnName <> Nothing Then
                            sql.Append(", Min(dbo.Benutzer.[")
                            sql.Append(MyUserFilter.UsersTableColumnName.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                            sql.Append("]) as [")
                            sql.Append(MyUserFilter.PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                            sql.Append("]")
                        Else
                            sql.Append(", Min([#")
                            sql.Append(MyUserFilter.PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                            sql.Append("].Value) as [")
                            sql.Append(MyUserFilter.PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                            sql.Append("]")
                        End If
                    End If
                Next
            End If
            sql.Append(vbNewLine)
            'From
            sql.Append("FROM dbo.Benutzer")
            sql.Append(vbNewLine)
            If Not userFilter Is Nothing Then
                For MyCounter As Integer = 0 To userFilter.Length - 1
                    If userFilter(MyCounter).UsersTableColumnName = Nothing Then
                        sql.Append("  LEFT JOIN [#")
                        sql.Append(userFilter(MyCounter).PropertyName)
                        sql.Append("] ON dbo.benutzer.id = [#")
                        sql.Append(userFilter(MyCounter).PropertyName)
                        sql.Append("].ID_User")
                        sql.Append(vbNewLine)
                    End If
                Next
            End If
            'Where
            If Not userFilter Is Nothing Then
                For MyUserFilterCounter As Integer = 0 To userFilter.Length - 1
                    Dim MyUserFilter As UserFilter = userFilter(MyUserFilterCounter)
                    If MyUserFilterCounter = 0 Then
                        sql.Append("WHERE ")
                    Else
                        sql.Append(" AND ")
                    End If
                    Dim CompareColumn As String = ""
                    If MyUserFilter.UsersTableColumnName <> Nothing Then
                        CompareColumn &= "dbo.Benutzer.["
                        CompareColumn &= MyUserFilter.UsersTableColumnName.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                        CompareColumn &= "]"
                    Else
                        CompareColumn &= "[#"
                        CompareColumn &= MyUserFilter.PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                        CompareColumn &= "].Value"
                    End If
                    Select Case MyUserFilter.SearchMethod
                        Case WMSystem.UserFilter.SearchMethods.All
                            sql.Append("1 = 1")
                        Case WMSystem.UserFilter.SearchMethods.Exist
                            sql.Append(CompareColumn)
                            sql.Append(" IS NOT NULL")
                        Case WMSystem.UserFilter.SearchMethods.NotExist
                            sql.Append(CompareColumn)
                            sql.Append(" IS NULL")
                        Case WMSystem.UserFilter.SearchMethods.IsEmpty
                            Select Case MyUserFilter.UsersTableColumnType
                                Case ""
                                    sql.Append(CompareColumn)
                                    sql.Append(" IS NULL OR ")
                                    sql.Append(CompareColumn)
                                    sql.Append("=''")
                                Case Else
                                    sql.Append(CompareColumn)
                                    sql.Append("IS NULL")
                            End Select
                        Case WMSystem.UserFilter.SearchMethods.MatchAtTheBeginning
                            If MyUserFilter.MatchExpressions Is Nothing OrElse MyUserFilter.MatchExpressions.Length = 0 Then
                                Throw New Exception("SearchMethods.MatchAtTheBeginning requires existing values in userFilter.MatchExpressions for property " & MyUserFilter.PropertyName)
                            End If
                            Select Case MyUserFilter.UsersTableColumnType
                                Case ""
                                    sql.Append("(")
                                    For MyCounter As Integer = 0 To MyUserFilter.MatchExpressions.Length - 1
                                        If MyCounter <> 0 Then
                                            sql.Append(" OR ")
                                        End If
                                        sql.Append(CompareColumn)
                                        sql.Append(" LIKE N'")
                                        sql.Append(MyUserFilter.MatchExpressions(MyCounter).Replace("'", "''"))
                                        sql.Append("%'")
                                    Next
                                    sql.Append(")")
                                Case Else
                                    Throw New Exception("SearchMethods.MatchAtTheBeginning not possible for property " & MyUserFilter.PropertyName)
                            End Select
                        Case WMSystem.UserFilter.SearchMethods.MatchExactly
                            If MyUserFilter.MatchExpressions Is Nothing OrElse MyUserFilter.MatchExpressions.Length = 0 Then
                                Throw New Exception("SearchMethods.MatchExactly requires existing values in userFilter.MatchExpressions for property " & MyUserFilter.PropertyName)
                            End If
                            Select Case MyUserFilter.UsersTableColumnType
                                Case ""
                                    sql.Append("(")
                                    For MyCounter As Integer = 0 To MyUserFilter.MatchExpressions.Length - 1
                                        If MyCounter <> 0 Then
                                            sql.Append(" OR ")
                                        End If
                                        sql.Append(CompareColumn)
                                        sql.Append(" = N'")
                                        sql.Append(MyUserFilter.MatchExpressions(MyCounter).Replace("'", "''"))
                                        sql.Append("'")
                                    Next
                                    sql.Append(")")
                                Case Else
                                    Throw New Exception("SearchMethods.MatchExactly not possible for property " & MyUserFilter.PropertyName)
                            End Select
                        Case WMSystem.UserFilter.SearchMethods.NoMatch
                            If MyUserFilter.MatchExpressions Is Nothing OrElse MyUserFilter.MatchExpressions.Length = 0 Then
                                Throw New Exception("SearchMethods.NoMatch requires existing values in userFilter.MatchExpressions for property " & MyUserFilter.PropertyName)
                            End If
                            Select Case MyUserFilter.UsersTableColumnType
                                Case ""
                                    sql.Append("(")
                                    For MyCounter As Integer = 0 To MyUserFilter.MatchExpressions.Length - 1
                                        If MyCounter <> 0 Then
                                            sql.Append(" OR ")
                                        End If
                                        sql.Append(CompareColumn)
                                        sql.Append(" <> '")
                                        sql.Append(MyUserFilter.MatchExpressions(MyCounter).Replace("'", "''"))
                                        sql.Append("'")
                                    Next
                                    sql.Append(")")
                                Case Else
                                    Throw New Exception("SearchMethods.NoMatch not possible for property " & MyUserFilter.PropertyName)
                            End Select
                    End Select
                Next
                sql.Append(vbNewLine)
            End If
            'Group By
            sql.Append("GROUP BY dbo.Benutzer.ID")
            sql.Append(vbNewLine)
            'Order By
            If Not sortByPropertyName Is Nothing Then
                For MyCounter As Integer = 0 To sortByPropertyName.Length - 1
                    'Find the appropriate UserFilter
                    Dim MyUserFilter As UserFilter = Nothing
                    For MyUserFilterCounter As Integer = 0 To userFilter.Length - 1
                        If sortByPropertyName(MyCounter).ColumnName.ToLower(System.Globalization.CultureInfo.InvariantCulture) = userFilter(MyUserFilterCounter).PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                            MyUserFilter = userFilter(MyUserFilterCounter)
                            Exit For
                        End If
                    Next
                    'Now create some SQL
                    If MyCounter = 0 Then
                        sql.Append("ORDER BY ")
                    Else
                        sql.Append(", ")
                    End If
                    If sortByPropertyName(MyCounter).ColumnName = "ID" Then
                        'Always there
                        sql.Append("ID")
                    Else
                        'Lookup the correct column name
                        If MyUserFilter.UsersTableColumnName <> Nothing Then
                            'Column from table Benutzer
                            sql.Append("Min(dbo.Benutzer.[")
                            sql.Append(MyUserFilter.UsersTableColumnName.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                            sql.Append("])")
                        Else
                            'Column created from table Log_Users
                            sql.Append("Min([#")
                            sql.Append(MyUserFilter.PropertyName.ToLower(System.Globalization.CultureInfo.InvariantCulture))
                            sql.Append("].Value)")
                        End If
                    End If
                    'Sort direction
                    If sortByPropertyName(MyCounter).Direction.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "desc" Then
                        sql.Append(" DESC")
                    End If
                Next
                sql.Append(vbNewLine)
            End If
            sql.Append(vbNewLine)
            'Clean up
            If Not userFilter Is Nothing Then
                For MyCounter As Integer = 0 To userFilter.Length - 1
                    If userFilter(MyCounter).UsersTableColumnName = Nothing Then
                        sql.Append("DROP TABLE [#")
                        sql.Append(userFilter(MyCounter).PropertyName)
                        sql.Append("]")
                        sql.Append(vbNewLine)
                    End If
                Next
            End If
            'Read without locks
            sql.Insert(0, "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine)

            'Execute the search command and return the results
            Dim Result As DataTable = Nothing
            Try
                'Query the data
                Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New System.Data.SqlClient.SqlConnection(Me.ConnectionString), sql.ToString, CommandType.Text, Nothing, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "users")
            Catch ex As Exception
                Me.Log.RuntimeException("Exception while processing a search for users: " & ex.ToString & vbNewLine & "This is the prepared SQL string:" & vbNewLine & sql.ToString)
            End Try

            'Change datatype of ID (int or bigint in database) to Long
            Result.Columns.Add("LongID_User", GetType(Long))
            Dim IndexID As Integer = Result.Columns("ID").Ordinal
            Dim IndexIDLong As Integer = Result.Columns("LongID_User").Ordinal
            For RowCounter As Integer = 0 To Result.Rows.Count - 1
                Result.Rows(RowCounter)(IndexIDLong) = Result.Rows(RowCounter)(IndexID)
            Next
            Result.Columns.Remove("ID")
            Result.Columns("LongID_User").ColumnName = "ID"

            'Return with result
            Return Result

        End Function

    End Class

End Namespace
