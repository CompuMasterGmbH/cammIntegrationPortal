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

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Partial Public Class WMSystem

        ''' <summary>
        '''     User information
        ''' </summary>
        ''' <remarks>
        '''     This class contains all information of a user profile as well as all important methods for handling of that account
        ''' </remarks>
        ''' <attention>
        '''     When the list of properties changes, you might also update the import and export methods to match the new, extended set of fields
        ''' </attention>
        Public Class UserInformation
            Implements IUserInformation

            'When the list of properties changes, you might also update the import and export methods to match the new, extended set of fields
            Private _WebManager As WMSystem
            Private _PartiallyLoadedDataCurrently As Boolean
            Private _ID As Long
            Private _LoginName As String
            Private _EMailAddress As String
            Private _Company As String
            Private _FirstName As String
            Private _LastName As String
            Private _AcademicTitle As String
            Private _Street As String
            Private _ZipCode As String
            Private _City As String
            Private _State As String
            Private _Country As String
            Private _PreferredLanguage1 As LanguageInformation
            Private _PreferredLanguage2 As LanguageInformation
            Private _PreferredLanguage3 As LanguageInformation
            Private _PreferredLanguage1ID As Integer
            Private _PreferredLanguage2ID As Integer
            Private _PreferredLanguage3ID As Integer
            Private _NameAddition As String
            Private _Sex As Sex
            Private _LoginDisabled As Boolean
            Private _LoginLockedTemporary As Boolean
            Private _LoginLockedTemporaryTill As DateTime
            Private _LoginDeleted As Boolean
            Private _AdditionalFlags As New Collections.Specialized.NameValueCollection
            Private _AccessLevel As AccessLevelInformation
            Private _AccessLevelID As Integer = Integer.MinValue
            Private _System_InitOfAuthorizationsDone As Boolean
            Private _AccountProfileValidatedByEMailTest As Boolean
            Private _AutomaticLogonAllowedByMachineToMachineCommunication As Boolean
            Private _ExternalAccount As String
            'When the list of properties changes, you might also update the import and export methods to match the new, extended set of fields

            Private Property WebManager() As IWebManager Implements IUserInformation.WebManager
                Get
                    Return _WebManager
                End Get
                Set(ByVal Value As IWebManager)
                    _WebManager = CType(Value, WMSystem)
                End Set
            End Property

            ''' <summary>
            '''     Create a new user profile
            ''' </summary>
            ''' <param name="UserID">Should be null (Nothing in VisualBasic)</param>
            ''' <param name="LoginName">Login name of the user</param>
            ''' <param name="EMailAddress">e-mail address</param>
            ''' <param name="Company">The user's company</param>
            ''' <param name="Sex">The user's gender</param>
            ''' <param name="NameAddition">An additional word in front of the name, for example the &quot;de&quot; in &quot;de Vries&quot;</param>
            ''' <param name="FirstName">The first name</param>
            ''' <param name="LastName">The family name</param>
            ''' <param name="AcademicTitle">An academic title, for example &quot;Dr.&quot; or &quot;Prof.&quot;</param>
            ''' <param name="Street">The street of the user's postal address</param>
            ''' <param name="ZipCode">The zip code of the user's postal address</param>
            ''' <param name="City">The name of the city</param>
            ''' <param name="State">In some countries (USA, Canada, etc.) you also have to identify the state</param>
            ''' <param name="Country">The country</param>
            ''' <param name="PreferredLanguage1ID">The ID of the user's favorite language</param>
            ''' <param name="PreferredLanguage2ID">The ID of the first alternative language</param>
            ''' <param name="PreferredLanguage3ID">The ID of the second alternative language</param>
            ''' <param name="LoginDisabled">Disable the possiblity to login with this account</param>
            ''' <param name="LoginLockedTemporary">Lock the possibility to login for a short period of time</param>
            ''' <param name="LoginDeleted">Mark this account as deleted</param>
            ''' <param name="AccessLevelID">The access level ID of the user (set to Integer.MinValue to decide based on the default access location of the current server environment)</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="ExternalAccount">An external account relation for single-sign-on purposes</param>
            ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
            <Obsolete("UserIDs should be Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub New(ByVal UserID As Integer, ByVal LoginName As String, ByVal EMailAddress As String, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, ByVal ExternalAccount As String, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
                If UserID = SpecialUsers.User_Anonymous Or UserID = SpecialUsers.User_Code Or UserID = SpecialUsers.User_Public Or UserID = SpecialUsers.User_UpdateProcessor Then
                    Throw New InvalidOperationException("Can't assign user details to this special system user")
                End If
                If Len(LoginName) > 20 Then
                    If Len(LoginName) > 50 Then
                        Throw New NotSupportedException("Login names can't be larger than 50 characters")
                    ElseIf Setup.DatabaseUtils.Version(WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer (support already with earlier versions (build 178), but activated now)
                        'up to 50 chars is supported
                    Else
                        Throw New NotSupportedException("Login names can't be larger than 20 characters")
                    End If
                End If
                _ID = UserID
                _LoginName = LoginName
                _EMailAddress = EMailAddress
                MyClass.Company = Company
                _Sex = Sex
                MyClass.AcademicTitle = AcademicTitle
                MyClass.FirstName = FirstName
                MyClass.NameAddition = NameAddition
                MyClass.LastName = LastName
                MyClass.Street = Street
                MyClass.ZipCode = ZipCode
                MyClass.Location = City
                MyClass.State = State
                MyClass.Country = Country
                _PreferredLanguage1ID = PreferredLanguage1ID
                _PreferredLanguage2ID = PreferredLanguage2ID
                _PreferredLanguage3ID = PreferredLanguage3ID
                _LoginDisabled = LoginDisabled
                _LoginLockedTemporary = LoginLockedTemporary
                _LoginDeleted = LoginDeleted
                _AccessLevelID = AccessLevelID
                _ExternalAccount = ExternalAccount
                _WebManager = WebManager
                If Not AdditionalFlags Is Nothing Then _AdditionalFlags = AdditionalFlags
                If UserID <> Nothing Then
                    'Quick fill mode
                    _PartiallyLoadedDataCurrently = True
                    _System_InitOfAuthorizationsDone = ReadInitAuthorizationsDoneValue()
                    'Else 'Data for a new user
                End If
            End Sub

            ''' <summary>
            '''     Create a new user profile
            ''' </summary>
            ''' <param name="UserID">Should be null (Nothing in VisualBasic)</param>
            ''' <param name="LoginName">Login name of the user</param>
            ''' <param name="EMailAddress">e-mail address</param>
            ''' <param name="Company">The user's company</param>
            ''' <param name="Sex">The user's gender</param>
            ''' <param name="NameAddition">An additional word in front of the name, for example the &quot;de&quot; in &quot;de Vries&quot;</param>
            ''' <param name="FirstName">The first name</param>
            ''' <param name="LastName">The family name</param>
            ''' <param name="AcademicTitle">An academic title, for example &quot;Dr.&quot; or &quot;Prof.&quot;</param>
            ''' <param name="Street">The street of the user's postal address</param>
            ''' <param name="ZipCode">The zip code of the user's postal address</param>
            ''' <param name="City">The name of the city</param>
            ''' <param name="State">In some countries (USA, Canada, etc.) you also have to identify the state</param>
            ''' <param name="Country">The country</param>
            ''' <param name="PreferredLanguage1ID">The ID of the user's favorite language</param>
            ''' <param name="PreferredLanguage2ID">The ID of the first alternative language</param>
            ''' <param name="PreferredLanguage3ID">The ID of the second alternative language</param>
            ''' <param name="LoginDisabled">Disable the possiblity to login with this account</param>
            ''' <param name="LoginLockedTemporary">Lock the possibility to login for a short period of time</param>
            ''' <param name="LoginDeleted">Mark this account as deleted</param>
            ''' <param name="AccessLevelID">The access level ID of the user (set to Integer.MinValue to decide based on the default access location of the current server environment)</param>
            ''' <param name="__reserved">Obsolete parameter</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub New(ByVal UserID As Integer, ByVal LoginName As String, ByVal EMailAddress As String, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByVal __reserved As Boolean, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
                Me.New(CType(UserID, Long), LoginName, EMailAddress, False, Company, Sex, NameAddition, FirstName, LastName, AcademicTitle, Street, ZipCode, City, State, Country, PreferredLanguage1ID, PreferredLanguage2ID, PreferredLanguage3ID, LoginDisabled, LoginLockedTemporary, LoginDeleted, AccessLevelID, WebManager, Nothing, AdditionalFlags)
            End Sub
            ''' <summary>
            '''     Load a user profile from the system database
            ''' </summary>
            ''' <param name="UserID">The user ID of the profile to be loaded</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="SearchForDeletedAccountsAsWell">Search for deleted accounts, too</param>
            ''' <remarks>
            '''     If you've loaded an already deleted user profile, you may miss the following information:
            '''     <list>
            '''         <item>Access level</item>
            '''         <item>Login disabled</item>
            '''         <item>LoginLockedTemporary</item>
            '''     </list>
            ''' </remarks>
            <Obsolete("UserID should be of type Int64"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub New(ByVal UserID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
                Me.New(CType(UserID, Long), WebManager, SearchForDeletedAccountsAsWell)
            End Sub
            ''' <summary>
            '''     Create a new user profile
            ''' </summary>
            ''' <param name="UserID">Should be null (Nothing in VisualBasic)</param>
            ''' <param name="LoginName">Login name of the user</param>
            ''' <param name="EMailAddress">e-mail address</param>
            ''' <param name="AutomaticLogonAllowedByMachineToMachineCommunication">Not yet supported, use false to prevent the throwing of an exception</param>
            ''' <param name="Company">The user's company</param>
            ''' <param name="Sex">The user's gender</param>
            ''' <param name="NameAddition">An additional word in front of the name, for example the &quot;de&quot; in &quot;de Vries&quot;</param>
            ''' <param name="FirstName">The first name</param>
            ''' <param name="LastName">The family name</param>
            ''' <param name="AcademicTitle">An academic title, for example &quot;Dr.&quot; or &quot;Prof.&quot;</param>
            ''' <param name="Street">The street of the user's postal address</param>
            ''' <param name="ZipCode">The zip code of the user's postal address</param>
            ''' <param name="City">The name of the city</param>
            ''' <param name="State">In some countries (USA, Canada, etc.) you also have to identify the state</param>
            ''' <param name="Country">The country</param>
            ''' <param name="PreferredLanguage1ID">The ID of the user's favorite language</param>
            ''' <param name="PreferredLanguage2ID">The ID of the first alternative language</param>
            ''' <param name="PreferredLanguage3ID">The ID of the second alternative language</param>
            ''' <param name="LoginDisabled">Disable the possiblity to login with this account</param>
            ''' <param name="LoginLockedTemporary">Lock the possibility to login for a short period of time</param>
            ''' <param name="LoginDeleted">Mark this account as deleted</param>
            ''' <param name="AccessLevelID">The access level ID of the user (set to Integer.MinValue to decide based on the default access location of the current server environment)</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="ExternalAccount">An external account relation for single-sign-on purposes</param>
            ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
            Public Sub New(ByVal UserID As Long, ByVal LoginName As String, ByVal EMailAddress As String, ByVal AutomaticLogonAllowedByMachineToMachineCommunication As Boolean, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, ByVal ExternalAccount As String, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
                If UserID = SpecialUsers.User_Anonymous Or UserID = SpecialUsers.User_Code Or UserID = SpecialUsers.User_Public Or UserID = SpecialUsers.User_UpdateProcessor Then
                    Throw New InvalidOperationException("Can't assign user details to this special system user")
                End If
                If Len(LoginName) > 20 Then
                    If Len(LoginName) > 50 Then
                        Throw New NotSupportedException("Login names can't be larger than 50 characters")
                    ElseIf Setup.DatabaseUtils.Version(WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer (support already with earlier versions (build 178), but activated now)
                        'up to 50 chars is supported
                    Else
                        Throw New NotSupportedException("Login names can't be larger than 20 characters")
                    End If
                End If
                _ID = UserID
                _LoginName = LoginName
                _EMailAddress = EMailAddress
                _Company = Company
                _AutomaticLogonAllowedByMachineToMachineCommunication = AutomaticLogonAllowedByMachineToMachineCommunication
                _Sex = Sex
                MyClass.AcademicTitle = AcademicTitle
                MyClass.FirstName = FirstName
                MyClass.NameAddition = NameAddition
                MyClass.LastName = LastName
                MyClass.Street = Street
                MyClass.ZipCode = ZipCode
                MyClass.Location = City
                MyClass.State = State
                MyClass.Country = Country
                _PreferredLanguage1ID = PreferredLanguage1ID
                _PreferredLanguage2ID = PreferredLanguage2ID
                _PreferredLanguage3ID = PreferredLanguage3ID
                _LoginDisabled = LoginDisabled
                _LoginLockedTemporary = LoginLockedTemporary
                _LoginDeleted = LoginDeleted
                _AccessLevelID = AccessLevelID
                _WebManager = WebManager
                _ExternalAccount = ExternalAccount
                If Not AdditionalFlags Is Nothing Then _AdditionalFlags = AdditionalFlags
                If UserID <> Nothing Then
                    'Quick fill mode
                    _PartiallyLoadedDataCurrently = True
                    _System_InitOfAuthorizationsDone = ReadInitAuthorizationsDoneValue()
                    'Else 'Data for a new user
                End If
            End Sub
            ''' <summary>
            '''     Load a user profile from the system database
            ''' </summary>
            ''' <param name="UserID">The user ID of the profile to be loaded</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="SearchForDeletedAccountsAsWell">Search for deleted accounts, too</param>
            ''' <remarks>
            '''     If you've loaded an already deleted user profile, you may miss the following information:
            '''     <list>
            '''         <item>Access level</item>
            '''         <item>Login disabled</item>
            '''         <item>LoginLockedTemporary</item>
            '''     </list>
            ''' </remarks>
            Public Sub New(ByVal UserID As Long, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
                If UserID = Nothing Then
                    Throw New ArgumentNullException("userID")
                End If
                If WebManager Is Nothing Then
                    Throw New ArgumentNullException("webManager")
                End If
                _ID = UserID
                _WebManager = WebManager
                ReadCompleteUserInformation(SearchForDeletedAccountsAsWell)
            End Sub
            ''' <summary>
            '''     Assign properties of a user profile from a table row of the system database
            ''' </summary>
            ''' <param name="dataRow">The row from the data table containing the full user data</param>
            ''' <param name="webManager">The current instance of camm Web-Manager</param>
            Friend Sub New(dataRow As DataRow, webManager As WMSystem)
                Me.New(CType(dataRow("ID"), Long), _
                                                CType(dataRow("LoginName"), String), _
                                                CType(dataRow("E-Mail"), String), _
                                                False, _
                                                Utils.Nz(dataRow("Company"), CType(Nothing, String)), _
                                                CType(IIf(Convert.ToString(Utils.Nz(dataRow("Anrede"), "")) = "", Sex.Undefined, IIf(Convert.ToString(Utils.Nz(dataRow("Anrede"), "")) = "Mr.", Sex.Masculine, Sex.Feminine)), Sex), _
                                                Utils.Nz(dataRow("Namenszusatz"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("Vorname"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("Nachname"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("Titel"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("Strasse"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("PLZ"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("Ort"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("State"), CType(Nothing, String)), _
                                                Utils.Nz(dataRow("Land"), CType(Nothing, String)), _
                                                CType(dataRow("1stPreferredLanguage"), Integer), _
                                                Utils.Nz(dataRow("2ndPreferredLanguage"), 0), _
                                                Utils.Nz(dataRow("3rdPreferredLanguage"), 0), _
                                                CType(dataRow("LoginDisabled"), Boolean), _
                                                Not IsDBNull(dataRow("LoginLockedTill")), _
                                                False, _
                                                CType(dataRow("AccountAccessability"), Integer), _
                                                webManager, _
                                                CType(Nothing, String), _
                                                CType(Nothing, System.Collections.Specialized.NameValueCollection))
            End Sub

            ''' <summary>
            ''' Returns a value from the database which indicates whether the authorizations for this user have already been set
            ''' </summary>
            ''' <remarks></remarks>
            Private Function ReadInitAuthorizationsDoneValue() As Boolean
                Dim result As Boolean = False
                If _ID <> Nothing Then
                    result = ReadInitAuthorizationsDoneValue(New SqlConnection(_WebManager.ConnectionString), _ID)
                End If
                Return result
            End Function
            ''' <summary>
            ''' Returns a value from the database which indicates whether the authorizations for this user have already been set
            ''' </summary>
            ''' <param name="dbConnection"></param>
            ''' <param name="userID"></param>
            Friend Shared Function ReadInitAuthorizationsDoneValue(dbConnection As SqlConnection, userID As Long) As Boolean
                Dim result As Boolean = False
                Dim cmd As New SqlCommand("SELECT [ID_User],[Type],[Value] FROM Log_Users WHERE Type='InitAuthorizationsDone' AND ID_User = @ID", dbConnection)
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = userID
                Try
                    dbConnection.Open()
                    Dim reader As SqlDataReader = Nothing
                    Try
                        reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            result = CType(IIf(Convert.ToString(Utils.Nz(reader("Value"), "")) = "1", True, False), Boolean)
                        End If
                    Finally
                        If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                            reader.Close()
                        End If
                    End Try
                Finally
                    If Not cmd Is Nothing Then
                        cmd.Dispose()
                    End If
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(dbConnection)
                End Try
                Return result
            End Function

            ''' <summary>
            '''     Is this user a system user (anonymous, public, etc.)
            ''' </summary>
            ''' <value></value>
            ''' <seealso cref="CompuMaster.camm.WebManager.WMSystem.SpecialUsers" />
            Public ReadOnly Property IsSystemUser() As Boolean
                Get
                    Return CompuMaster.camm.WebManager.WMSystem.IsSystemUser(_ID)
                End Get
            End Property

            ''' <summary>
            '''     Suggest some login names for a new user account based on the already given data
            ''' </summary>
            ''' <returns>An array of free and available loginnames with at least 1 possible loginname</returns>
            Public Function SuggestedFreeLoginNames() As String()
                Return SuggestedFreeLoginNames(Nothing)
            End Function

            ''' <summary>
            '''     Suggest some login names for a new user account based on the already given data
            ''' </summary>
            ''' <param name="favorites">Favorites for suggested file names</param>
            ''' <returns>An array of free and available loginnames with at least 1 possible loginname</returns>
            Public Function SuggestedFreeLoginNames(ByVal favorites As String()) As String()

                Dim Result As New ArrayList

                'Add favorites, first
                If Not favorites Is Nothing Then
                    For MyCounter As Integer = 0 To favorites.Length - 1
                        SuggestedFreeLoginNamesValidation(Result, favorites(MyCounter))
                    Next
                End If

                'Make some suggestions, second
                SuggestedFreeLoginNamesValidation(Result, Mid(Me.FirstName, 1, 1) & Me.LastName)
                SuggestedFreeLoginNamesValidation(Result, Mid(Me.FirstName, 1, 1) & "." & Me.LastName)
                SuggestedFreeLoginNamesValidation(Result, Me.FirstName)
                SuggestedFreeLoginNamesValidation(Result, Me.LastName)
                SuggestedFreeLoginNamesValidation(Result, Me.FirstName & Me.LastName)
                SuggestedFreeLoginNamesValidation(Result, Me.FirstName & "." & Me.LastName)
                SuggestedFreeLoginNamesValidation(Result, Me.EMailAddress)
                SuggestedFreeLoginNamesValidation(Result, Me.Company)
                SuggestedFreeLoginNamesValidation(Result, System.Guid.NewGuid.ToString.Replace("-", ""))
                SuggestedFreeLoginNamesValidation(Result, System.Guid.NewGuid.ToString.Replace("-", "")) 'a 2nd one in case the line before didn't produced a free available login name

                'Check which suggestions are already in use in the user database and remove those items
                If Result.Count > 0 Then
                    Dim ExistingUsers As DataTable = _WebManager.SearchUserData(New UserFilter() {New UserFilter("LoginName", UserFilter.SearchMethods.MatchExactly, CType(Result.ToArray(GetType(String)), String()))}, New UserSortArgument() {})
                    For MyCounter As Integer = Result.Count - 1 To 0 Step -1
                        For MyInnerCounter As Integer = 0 To ExistingUsers.Rows.Count - 1
                            If LCase(CType(ExistingUsers.Rows(MyInnerCounter)("LoginName"), String)) = LCase(CType(Result(MyCounter), String)) Then
                                Result.RemoveAt(MyCounter)
                                Exit For
                            End If
                        Next
                    Next
                End If

                Return CType(Result.ToArray(GetType(String)), String())
            End Function

            ''' <summary>
            '''     Ensure that the suggestion is acceptable as well as unique for the result list 
            ''' </summary>
            ''' <param name="list">The result list where the validated value shall be added</param>
            ''' <param name="name">A loginname suggestion</param>
            Private Sub SuggestedFreeLoginNamesValidation(ByVal list As ArrayList, ByVal name As String)
                name = Mid(Trim(name), 1, 20)
                If name.Length < 2 OrElse name.StartsWith(".") OrElse name.EndsWith(".") OrElse name.StartsWith("_") OrElse name.EndsWith("_") OrElse name.StartsWith("-") OrElse name.EndsWith("-") Then
                    name = Nothing 'Ignore this suggestion
                End If
                If name <> Nothing Then
                    'Max 20 characters
                    Dim NewValue As String = Mid(Trim(name), 1, 20).ToLower(System.Globalization.CultureInfo.InvariantCulture)   'Max 20 characters for loginname
                    'Only add when it's not already there
                    Dim AlreadyExists As Boolean = False
                    For MyCounter As Integer = 0 To list.Count - 1
                        If NewValue = CType(list(MyCounter), String) Then
                            AlreadyExists = True
                        End If
                    Next
                    If Not AlreadyExists Then
                        list.Add(NewValue)
                    End If
                End If
            End Sub

            ''' <summary>
            '''     Is an automated logon procedure allowed for this account
            ''' </summary>
            ''' <value></value>
            Public Property AutomaticLogonAllowedByMachineToMachineCommunication() As Boolean
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    'TODOs: AutomaticLogonAllowedByMachineToMachineCommunication
                    '1. Add column to data table of users
                    '1a. Update internal methods to use the new constructor and fill this information
                    '2. Solve the problem that only 1 browser session can be logged in on the same time. Keep an eye on the GetLogonNameByBrowserSessionID (or similar method/SP name)
                    '3. Make this property configurable in the administrator's menus
                    Return _AutomaticLogonAllowedByMachineToMachineCommunication
                End Get
                Set(ByVal Value As Boolean)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AutomaticLogonAllowedByMachineToMachineCommunication = Value
                End Set
            End Property

            ''' <summary>
            '''     In some circumstances, it might make sense to reload the data
            ''' </summary>
            ''' <remarks>
            '''     If you loaded multiple user information objects with method System_GetUserInfos, it is recommended to first reload the data before starting update (because that method does a quick-load and the reload will be triggered internally, but may be after you already changed first fields; so your changes would be lost)
            ''' </remarks>
            Public Sub ReloadFullUserData()
                ReadCompleteUserInformation()
            End Sub

            Friend Shared ReservedFlagNames As String() = New String() {"Type", "CompleteName", "CompleteNameInclAddresses", "email", "Sex", "Addresses", "1stPreferredLanguage", "2ndPreferredLanguage", "3rdPreferredLanguage", "Company", "FirstName", "LastName", "NameAddition", "Street", "ZIPCode", "Location", "State", "Country", "AccountProfileValidatedByEMailTest", "InitAuthorizationsDone", "ExternalAccount", "AutomaticLogonAllowedByMachineToMachineCommunication", "Phone", "Fax", "Mobile", "Position", "IsImpersonationUser", "DeletedOn"}

            ''' <summary>
            '''     Read all the account data from database
            ''' </summary>
            ''' <param name="SearchForDeletedAccountsAsWell">Also load data of users who have been deleted in the past</param>
            Private Sub ReadCompleteUserInformation(Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
                If _ID = Nothing Then
                    Dim Message As String = "Cannot read user profile data with an empty ID value"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf _ID = SpecialUsers.User_Anonymous Then
                    'Login names can't be larger than 20 characters, but this is a special pseudo-account, so this is okay here
                    _LoginName = "camm Web-Manager anonymous users"
                    _FirstName = "Users without a logon"
                    _LastName = "Anonymous Users"
                    Return
                ElseIf _ID = SpecialUsers.User_Code Then
                    'Login names can't be larger than 20 characters, but this is a special pseudo-account, so this is okay here
                    _LoginName = "camm Web-Manager based client application"
                    _FirstName = "External application"
                    _LastName = "Client application"
                    Return
                ElseIf _ID = SpecialUsers.User_Public Then
                    'Login names can't be larger than 20 characters, but this is a special pseudo-account, so this is okay here
                    _LoginName = "camm Web-Manager public users"
                    _FirstName = "Users with a successfull logon"
                    _LastName = "Public Users"
                    Return
                ElseIf _ID = SpecialUsers.User_UpdateProcessor Then
                    'Login names can't be larger than 20 characters, but this is a special pseudo-account, so this is okay here
                    _LoginName = "camm Web-Manager Setup"
                    _FirstName = "camm Web-Manager"
                    _LastName = "Setup"
                    Return
                End If
                _LoginDeleted = True 'will be resetted to False again later if it exists
                _PartiallyLoadedDataCurrently = False 'Now, the data will be loaded
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("select * from benutzer where benutzer.id = @ID", MyConn)
                Dim AccountNotExists As Boolean
                Dim LogUsersDataFound As Boolean = False
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                Try
                    MyConn.Open()
                    Dim MyReader As SqlDataReader = Nothing
                    Try
                        MyReader = MyCmd.ExecuteReader()
                        If MyReader.Read Then
                            _ID = CType(MyReader("ID"), Long)
                            MyClass.Company = Utils.Nz(MyReader("Company"), CType(Nothing, String))
                            _LoginName = CType(MyReader("LoginName"), String)
                            _EMailAddress = CType(MyReader("E-Mail"), String)
                            MyClass.FirstName = Utils.Nz(MyReader("Vorname"), CType(Nothing, String))
                            MyClass.LastName = Utils.Nz(MyReader("Nachname"), CType(Nothing, String))
                            MyClass.AcademicTitle = Utils.Nz(MyReader("Titel"), CType(Nothing, String))
                            MyClass.Street = Utils.Nz(MyReader("Strasse"), CType(Nothing, String))
                            MyClass.ZipCode = Utils.Nz(MyReader("PLZ"), CType(Nothing, String))
                            MyClass.Location = Utils.Nz(MyReader("Ort"), CType(Nothing, String))
                            MyClass.State = Utils.Nz(MyReader("State"), CType(Nothing, String))
                            MyClass.Country = Utils.Nz(MyReader("Land"), CType(Nothing, String))
                            _PreferredLanguage1ID = Utils.Nz(MyReader("1stPreferredLanguage"), 1)
                            _PreferredLanguage2ID = Utils.Nz(MyReader("2ndPreferredLanguage"), CType(Nothing, Integer))
                            _PreferredLanguage3ID = Utils.Nz(MyReader("3rdPreferredLanguage"), CType(Nothing, Integer))
                            MyClass.NameAddition = Utils.Nz(MyReader("Namenszusatz"), CType(Nothing, String))
                            If Utils.Nz(MyReader("Anrede"), "") = "Mr." Then
                                _Sex = WMSystem.Sex.Masculine
                            ElseIf Utils.Nz(MyReader("Anrede"), "") = "Ms." Then
                                _Sex = WMSystem.Sex.Feminine
                            Else 'If Utils.Nz(MyReader("Anrede"), "") = "" Then
                                If MyClass.FirstName = Nothing OrElse MyClass.LastName = Nothing Then
                                    _Sex = WMSystem.Sex.MissingNameOrGroupOfPersons
                                Else
                                    _Sex = WMSystem.Sex.Undefined
                                End If
                            End If
                            _LoginDisabled = CType(MyReader("LoginDisabled"), Boolean)
                            _LoginLockedTemporary = Not IsDBNull(MyReader("LoginLockedTill"))
                            _LoginLockedTemporaryTill = Utils.Nz(MyReader("LoginLockedTill"), CType(Nothing, DateTime))
                            _LoginDeleted = False
                            _AccessLevelID = CType(MyReader("AccountAccessability"), Integer)
                            _AccountSuccessfullLogins = Utils.Nz(MyReader("LoginCount"), 0)
                            _AccountLoginFailures = Utils.Nz(MyReader("LoginFailures"), 0)
                            _AccountCreatedOn = Utils.Nz(MyReader("CreatedOn"), CType(Nothing, DateTime))
                            _AccountModifiedOn = Utils.Nz(MyReader("ModifiedOn"), CType(Nothing, DateTime))
                            _AccountLastLoginOn = Utils.Nz(MyReader("LastLoginOn"), CType(Nothing, DateTime))
                            _AccountLastLoginFromAddress = Utils.Nz(MyReader("LastLoginViaRemoteIP"), "")
                        Else
                            AccountNotExists = True
                        End If
                    Finally
                        If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                            MyReader.Close()
                        End If
                    End Try
                    MyCmd.CommandText = "select * from Log_Users where id_user = @ID"
                    Try
                        MyReader = MyCmd.ExecuteReader()
                        While MyReader.Read
                            LogUsersDataFound = True
                            If Not IsDBNull(MyReader("Type")) Then
                                Select Case Convert.ToString(MyReader("Type")).ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Case "CompleteName".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Case "CompleteNameInclAddresses".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Case "email".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        'keep the already read value from the table [benutzer]
                                        '_EMailAddress = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "Sex".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        Select Case Utils.Nz(MyReader("Value"), "").ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                            Case "m"
                                                _Sex = WMSystem.Sex.Masculine
                                            Case "u"
                                                _Sex = WMSystem.Sex.Undefined
                                            Case "w"
                                                _Sex = WMSystem.Sex.Feminine
                                            Case Else
                                                _Sex = WMSystem.Sex.MissingNameOrGroupOfPersons
                                        End Select
                                    Case "Addresses".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Case "1stPreferredLanguage".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        Try
                                            _PreferredLanguage1ID = CType(MyReader("Value"), Integer)
                                        Catch
                                            _PreferredLanguage1ID = Nothing
                                        End Try
                                    Case "2ndPreferredLanguage".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        Try
                                            _PreferredLanguage2ID = CType(MyReader("Value"), Integer)
                                        Catch
                                            _PreferredLanguage2ID = Nothing
                                        End Try
                                    Case "3rdPreferredLanguage".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        Try
                                            _PreferredLanguage3ID = CType(MyReader("Value"), Integer)
                                        Catch
                                            _PreferredLanguage3ID = Nothing
                                        End Try
                                    Case "Company".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.Company = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "FirstName".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        _FirstName = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "LastName".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        _LastName = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "NameAddition".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.NameAddition = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "Street".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.Street = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "ZIPCode".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.ZipCode = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "Location".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.Location = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "State".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.State = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "Country".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        MyClass.Country = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "AccountProfileValidatedByEMailTest"
                                        _AccountProfileValidatedByEMailTest = CType(IIf(Convert.ToString(Utils.Nz(MyReader("Value"), "")) = "1", True, False), Boolean)
                                    Case "InitAuthorizationsDone".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        _System_InitOfAuthorizationsDone = CType(IIf(Convert.ToString(Utils.Nz(MyReader("Value"), "")) = "1", True, False), Boolean)
                                    Case "ExternalAccount".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                        _ExternalAccount = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                    Case "AutomaticLogonAllowedByMachineToMachineCommunication" 'WARNING: flag name too long, saved in table as: "AutomaticLogonAllowedByMachineToMachineCommunicati"
                                        _AutomaticLogonAllowedByMachineToMachineCommunication = CType(IIf(Convert.ToString(Utils.Nz(MyReader("Value"), "")) = "1", True, False), Boolean)
                                    Case Else
                                        MyClass.AdditionalFlags(CType(MyReader("Type"), String)) = Utils.Nz(MyReader("Value"), CType(Nothing, String))
                                End Select
                            End If
                        End While
                    Finally
                        If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                            MyReader.Close()
                        End If
                    End Try
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                End Try
                If SearchForDeletedAccountsAsWell = False AndAlso AccountNotExists = True Then
                    _WebManager.Log.RuntimeException(New CompuMaster.camm.WebManager.UserNotFoundException(_ID), False, True, DebugLevels.NoDebug)
                ElseIf SearchForDeletedAccountsAsWell = True AndAlso AccountNotExists = True AndAlso LogUsersDataFound = False Then
                    _WebManager.Log.RuntimeException(New CompuMaster.camm.WebManager.UserNotFoundException(_ID), False, True, DebugLevels.NoDebug)
                End If
            End Sub

            ''' <summary>
            '''     Verify if a given value matches the current password
            ''' </summary>
            ''' <param name="password">A password which shall be verified</param>
            Public Function TestForPassword(ByVal password As String) As Boolean
                password = Trim(password)
                If password = Nothing Then
                    Return False
                End If

                Dim MyCmd As New SqlCommand("SELECT LoginPW FROM dbo.Benutzer WHERE ID = @ID", New SqlConnection(_WebManager.ConnectionString))
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = Me.IDLong

                Dim CurrentlySavedPassword As String
                CurrentlySavedPassword = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), "")

                Dim transformationResult As CryptoTransformationResult = Me._WebManager.System_GetUserPasswordTransformationResult(Me._LoginName)
                Dim transformer As IWMPasswordTransformation = PasswordTransformerFactory.ProduceCryptographicTransformer(transformationResult.Algorithm)
                Dim transformedPassword As String = transformer.TransformString(password, transformationResult.Noncevalue)

                Return transformedPassword = CurrentlySavedPassword
            End Function

            ''' <summary>
            '''     Verify if a given value matches the current password
            ''' </summary>
            ''' <param name="password">A password which shall be verified</param>
            <Obsolete("Use TestForPassword instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Public Function ValidatePassword(ByVal password As String) As Boolean
                Return TestForPassword(password)
            End Function

            ''' <summary>
            '''     The account ID
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property IDLong() As Long Implements IUserInformation.ID
                Get
                    Return _ID
                End Get
            End Property
            ''' <summary>
            '''     The account ID
            ''' </summary>
            ''' <value></value>
            <Obsolete("Better use IDLong instead")> Public ReadOnly Property ID() As Integer
                Get
                    Return CType(_ID, Integer)
                End Get
            End Property
            ''' <summary>
            '''     Set the user ID for a new registered user
            ''' </summary>
            ''' <param name="ID"></param>
            Friend Sub SetNewUserID(ByVal ID As Long)
                _ID = ID
            End Sub
            ''' <summary>
            '''     The login name of the user
            ''' </summary>
            ''' <value></value>
            Public Property LoginName() As String Implements IUserInformation.LoginName
                Get
                    Return Trim(_LoginName)
                End Get
                Set(ByVal Value As String)
                    If Len(Value) > 20 Then
                        Throw New NotSupportedException("Login names can't be larger than 20 characters")
                    End If
                    _LoginName = Value
                End Set
            End Property
            ''' <summary>
            '''     Indicate wether the user has already got an e-mail notification that he has got his first priviledges and/or memberships assigned
            ''' </summary>
            ''' <value></value>
            Public Property AccountAuthorizationsAlreadySet() As Boolean
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _System_InitOfAuthorizationsDone
                End Get
                Set(ByVal Value As Boolean)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _System_InitOfAuthorizationsDone = Value
                End Set
            End Property
            ''' <summary>
            '''     The required e-mail address where all important messages will be sent to
            ''' </summary>
            ''' <value></value>
            Public Property EMailAddress() As String Implements IUserInformation.EMailAddress
                Get
                    Return _EMailAddress
                End Get
                Set(ByVal Value As String)
                    _EMailAddress = Value
                End Set
            End Property
            ''' <summary>
            '''     The fax number
            ''' </summary>
            ''' <value></value>
            Public Property FaxNumber() As String Implements IUserInformation.FaxNumber
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return AdditionalFlags("Fax")
                End Get
                Set(ByVal Value As String)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AdditionalFlags("Fax") = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The phone number
            ''' </summary>
            ''' <value></value>
            Public Property PhoneNumber() As String Implements IUserInformation.PhoneNumber
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return AdditionalFlags("Phone")
                End Get
                Set(ByVal Value As String)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AdditionalFlags("Phone") = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The mobile number
            ''' </summary>
            ''' <value></value>
            Public Property MobileNumber() As String Implements IUserInformation.MobileNumber
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return AdditionalFlags("Mobile")
                End Get
                Set(ByVal Value As String)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AdditionalFlags("Mobile") = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            ''' Indicate a user account solely for the purpose of impersonation for test and development
            ''' </summary>
            ''' <value></value>
            Public Property IsImpersonationUser() As Boolean Implements IUserInformation.IsImpersonationUser
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return (AdditionalFlags("IsImpersonationUser") = "1")
                End Get
                Set(ByVal Value As Boolean)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AdditionalFlags("IsImpersonationUser") = Utils.IIf(Of String)(Value, "1", Nothing)
                End Set
            End Property
            ''' <summary>
            '''     The position in the company the user is working for
            ''' </summary>
            ''' <value></value>
            Public Property Position() As String Implements IUserInformation.Position
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return AdditionalFlags("Position")
                End Get
                Set(ByVal Value As String)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AdditionalFlags("Position") = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The user's first name
            ''' </summary>
            ''' <value></value>
            Public Property FirstName() As String Implements IUserInformation.FirstName
                Get
                    Return _FirstName
                End Get
                Set(ByVal Value As String)
                    _FirstName = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The company title 
            ''' </summary>
            ''' <value></value>
            Public Property Company() As String Implements IUserInformation.Company
                Get
                    Return _Company
                End Get
                Set(ByVal Value As String)
                    _Company = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The surname of the user
            ''' </summary>
            ''' <value></value>
            Public Property LastName() As String Implements IUserInformation.LastName
                Get
                    Return _LastName
                End Get
                Set(ByVal Value As String)
                    _LastName = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property

            ''' <summary>
            '''     The full name of an user, e. g. "Dr. Adam van Vrede")
            ''' </summary>
            Public Function FullName() As String
                Return CType(IIf(_AcademicTitle = "", "", _AcademicTitle & " "), String) & _
                    _FirstName & " " & _
                    CType(IIf(_NameAddition = "", "", _NameAddition & " "), String) & _
                    _LastName
            End Function
            <Obsolete("use FullName instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public ReadOnly Property CompleteName() As String
                Get
                    Return FullName()
                End Get
            End Property

            ''' <summary>
            '''     The salutation name, e. g. "Dr. van Vrede" or "Vrede"
            ''' </summary>
            ''' <remarks>
            ''' If the last name is not available, this function returns null (Nothing in VisualBasic).
            ''' This method doesn't rely on gender information.
            ''' </remarks>
            Public Function SalutationNameOnly() As String
                If Me.LastName = Nothing Then
                    Return ""
                Else
                    Return CType(IIf(_AcademicTitle = "", "", _AcademicTitle & " "), String) & _
                        CType(IIf(_NameAddition = "", "", _NameAddition & " "), String) & _
                        _LastName
                End If
            End Function
            <Obsolete("use SalutationNameOnly instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public ReadOnly Property CompleteSalutationName() As String
                Get
                    Return SalutationNameOnly()
                End Get
            End Property

            ''' <summary>
            ''' Create a clone of a user account inclusive additional flags, memberships and authorizations
            ''' </summary>
            ''' <param name="newLoginName">The login name for the new user</param>
            ''' <param name="newGender">The gender for the new user</param>
            ''' <param name="newAcademicTitle">The academic title for the new user</param>
            ''' <param name="newFirstName">The first name for the new user</param>
            ''' <param name="newNameAddition">The name addition for the new user</param>
            ''' <param name="newLastName">The family name for the new user</param>
            ''' <param name="newEmailAddress">The e-mail adress for the new user</param>
            ''' <returns>The cloned user account (which is already saved in the database)</returns>
            ''' <remarks>
            ''' The password will be auto-generated.
            ''' Exceptions may be thrown e. g. in case of already existing login name or invalid password (strength)
            ''' </remarks>
            Public Function Clone(ByVal newLoginName As String, ByVal newGender As Sex, ByVal newAcademicTitle As String, ByVal newFirstName As String, ByVal newNameAddition As String, ByVal newLastName As String, ByVal newEmailAddress As String) As UserInformation
                Return Me.Clone(newLoginName, newGender, newAcademicTitle, newFirstName, newNameAddition, newLastName, newEmailAddress, CType(Nothing, String))
            End Function

            ''' <summary>
            ''' Create a clone of a user account inclusive additional flags, memberships and authorizations
            ''' </summary>
            ''' <param name="newLoginName">The login name for the new user</param>
            ''' <param name="newGender">The gender for the new user</param>
            ''' <param name="newAcademicTitle">The academic title for the new user</param>
            ''' <param name="newFirstName">The first name for the new user</param>
            ''' <param name="newNameAddition">The name addition for the new user</param>
            ''' <param name="newLastName">The family name for the new user</param>
            ''' <param name="newEmailAddress">The e-mail adress for the new user</param>
            ''' <param name="newPassword">The password for the new user</param>
            ''' <returns>The cloned user account (which is already saved in the database)</returns>
            ''' <remarks>
            ''' Exceptions may be thrown e. g. in case of already existing login name or invalid password (strength)
            ''' </remarks>
            Public Function Clone(ByVal newLoginName As String, ByVal newGender As Sex, ByVal newAcademicTitle As String, ByVal newFirstName As String, ByVal newNameAddition As String, ByVal newLastName As String, ByVal newEmailAddress As String, ByVal newPassword As String) As UserInformation

                'TODO: outgoing email is missing yet
                Dim TemplateUser As New WebManager.WMSystem.UserInformation(Me.IDLong, Me._WebManager, False)
                Dim NewUser As New WebManager.WMSystem.UserInformation(0&, newLoginName, newEmailAddress, False, TemplateUser.Company, newGender, newNameAddition, newFirstName, newLastName, newAcademicTitle, TemplateUser.Street, TemplateUser.ZipCode, TemplateUser.Location, TemplateUser.State, TemplateUser.Country, TemplateUser.PreferredLanguage1.ID, TemplateUser.PreferredLanguage2.ID, TemplateUser.PreferredLanguage3.ID, TemplateUser.LoginDisabled, False, False, TemplateUser.AccessLevel.ID, Me._WebManager, CType(Nothing, String), New System.Collections.Specialized.NameValueCollection(TemplateUser.AdditionalFlags))

                NewUser.AccountAuthorizationsAlreadySet = False
                NewUser.AccountProfileValidatedByEMailTest = False
                NewUser.AutomaticLogonAllowedByMachineToMachineCommunication = TemplateUser.AutomaticLogonAllowedByMachineToMachineCommunication
                NewUser.FaxNumber = TemplateUser.FaxNumber
                NewUser.MobileNumber = TemplateUser.MobileNumber
                NewUser.PhoneNumber = TemplateUser.PhoneNumber
                NewUser.Position = TemplateUser.Position
                NewUser.IsImpersonationUser = TemplateUser.IsImpersonationUser
                NewUser.Save(newPassword)  'Intermediate save point

                'Following actions take place at database directly
                For MyCounter As Integer = 0 To TemplateUser.MembershipsByRule().AllowRule.Length - 1
                    NewUser.AddMembership(TemplateUser.MembershipsByRule().AllowRule(MyCounter).ID, False)
                Next
                For MyCounter As Integer = 0 To TemplateUser.MembershipsByRule().DenyRule.Length - 1
                    NewUser.AddMembership(TemplateUser.MembershipsByRule().DenyRule(MyCounter).ID, True)
                Next
                For MyCounter As Integer = 0 To TemplateUser.AuthorizationsByRule.AllowRuleDevelopers.Length - 1
                    Dim usrItem As SecurityObjectAuthorizationForUser = TemplateUser.AuthorizationsByRule.AllowRuleDevelopers(MyCounter)
                    NewUser.AddAuthorization(usrItem.SecurityObjectID, usrItem.ServerGroupID, usrItem.IsDeveloperAuthorization, usrItem.IsDenyRule)
                Next
                For MyCounter As Integer = 0 To TemplateUser.AuthorizationsByRule.AllowRuleStandard.Length - 1
                    Dim usrItem As SecurityObjectAuthorizationForUser = TemplateUser.AuthorizationsByRule.AllowRuleStandard(MyCounter)
                    NewUser.AddAuthorization(usrItem.SecurityObjectID, usrItem.ServerGroupID, usrItem.IsDeveloperAuthorization, usrItem.IsDenyRule)
                Next
                For MyCounter As Integer = 0 To TemplateUser.AuthorizationsByRule.DenyRuleDevelopers.Length - 1
                    Dim usrItem As SecurityObjectAuthorizationForUser = TemplateUser.AuthorizationsByRule.DenyRuleDevelopers(MyCounter)
                    NewUser.AddAuthorization(usrItem.SecurityObjectID, usrItem.ServerGroupID, usrItem.IsDeveloperAuthorization, usrItem.IsDenyRule)
                Next
                For MyCounter As Integer = 0 To TemplateUser.AuthorizationsByRule.DenyRuleStandard.Length - 1
                    Dim usrItem As SecurityObjectAuthorizationForUser = TemplateUser.AuthorizationsByRule.DenyRuleStandard(MyCounter)
                    NewUser.AddAuthorization(usrItem.SecurityObjectID, usrItem.ServerGroupID, usrItem.IsDeveloperAuthorization, usrItem.IsDenyRule)
                Next

                Return NewUser

            End Function

            ''' <summary>
            ''' Quickly save the flag name and value and assign it to the current user profile, too
            ''' </summary>
            ''' <remarks>Ideal for saving single values quickly</remarks>
            Public Sub SaveAdditionalFlag(ByVal flagName As String, ByVal value As String)
                Me.AdditionalFlags(flagName) = value
                DataLayer.Current.SetUserDetail(Me._WebManager, Nothing, Me.IDLong, flagName, value, True)
            End Sub

#Region "Save"
            ''' <summary>
            '''     Save this user information object with the default notifications
            ''' </summary>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save()
                Save(_WebManager.Notifications)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal notifications As CompuMaster.camm.WebManager.Notifications.INotifications)
                Save(notifications, Nothing)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="Notifications">A notifications class which shall be used for messages to the user</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal notifications As CompuMaster.camm.WebManager.WMNotifications)
                Save(notifications, Nothing)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal notifications As CompuMaster.camm.WebManager.WMNotifications, ByVal newPassword As String)
                Save(notifications, newPassword, False)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <param name="suppressAllNotifications">Never send any notification mails</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Private Sub Save(ByVal notifications As CompuMaster.camm.WebManager.WMNotifications, ByVal newPassword As String, ByVal suppressAllNotifications As Boolean)
                Save(notifications, newPassword, suppressAllNotifications, suppressAllNotifications)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <param name="suppressUserNotifications">Never send any notification mails to the user</param>
            ''' <param name="suppressSecurityAdminNotifications">Never send any notification mails to the security admins</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Private Sub Save(ByVal notifications As CompuMaster.camm.WebManager.WMNotifications, ByVal newPassword As String, ByVal suppressUserNotifications As Boolean, ByVal suppressSecurityAdminNotifications As Boolean)
                Me.Save_Internal(newPassword, notifications, suppressUserNotifications, suppressSecurityAdminNotifications)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal notifications As CompuMaster.camm.WebManager.Notifications.INotifications, ByVal newPassword As String)
                Save(notifications, newPassword, False)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <param name="suppressAllNotifications">Never send any notification mails</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Private Sub Save(ByVal notifications As CompuMaster.camm.WebManager.Notifications.INotifications, ByVal newPassword As String, ByVal suppressAllNotifications As Boolean)
                Save(notifications, newPassword, suppressAllNotifications, suppressAllNotifications)
            End Sub
            ''' <summary>
            '''     Save this user information object
            ''' </summary>
            ''' <param name="notifications">A notifications class which shall be used for messages to the user</param>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <param name="suppressUserNotifications">Never send any notification mails to the user</param>
            ''' <param name="suppressSecurityAdminNotifications">Never send any notification mails to the security admins</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Private Sub Save(ByVal notifications As CompuMaster.camm.WebManager.Notifications.INotifications, ByVal newPassword As String, ByVal suppressUserNotifications As Boolean, ByVal suppressSecurityAdminNotifications As Boolean)
                Save_Internal(newPassword, notifications, suppressUserNotifications, suppressSecurityAdminNotifications)
            End Sub
            ''' <summary>
            '''     Save this user information object with the default notifications
            ''' </summary>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal newPassword As String)
                Save(_WebManager.Notifications, newPassword)
            End Sub

            ''' <summary>
            '''     Save this user information object with the default notifications
            ''' </summary>
            ''' <param name="suppressAllNotifications">Never send any notification mails</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal suppressAllNotifications As Boolean)
                Save(_WebManager.Notifications, Nothing, suppressAllNotifications)
            End Sub

            ''' <summary>
            '''     Save this user information object with the default notifications
            ''' </summary>
            ''' <param name="suppressUserNotifications">Never send any notification mails to the user</param>
            ''' <param name="suppressSecurityAdminNotifications">Never send any notification mails to the security admins</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal suppressUserNotifications As Boolean, ByVal suppressSecurityAdminNotifications As Boolean)
                Save(_WebManager.Notifications, Nothing, suppressUserNotifications, suppressSecurityAdminNotifications)
            End Sub

            ''' <summary>
            '''     Save this user information object with the default notifications
            ''' </summary>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <param name="suppressNotifications">Never send any notification mails</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal newPassword As String, ByVal suppressNotifications As Boolean)
                Save(_WebManager.Notifications, newPassword, suppressNotifications)
            End Sub

            ''' <summary>
            '''     Save this user information object with the default notifications
            ''' </summary>
            ''' <param name="newPassword">The new password for the user</param>
            ''' <param name="suppressUserNotifications">Never send any notification mails to the user</param>
            ''' <param name="suppressSecurityAdminNotifications">Never send any notification mails to the security admins</param>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Public Sub Save(ByVal newPassword As String, ByVal suppressUserNotifications As Boolean, ByVal suppressSecurityAdminNotifications As Boolean)
                Save(_WebManager.Notifications, newPassword, suppressUserNotifications, suppressSecurityAdminNotifications)
            End Sub

            Public Enum ValidationItem As Byte
                All = 0
                ProfileData = 1
                PasswordComplexityRequirements = 2
                RequiredFlags = 3
            End Enum

            ''' <summary>
            ''' Validate user profile data and password complexity
            ''' </summary>
            ''' <param name="checks">Kind of validation - but All won't check for password complexity requirements until new password has been specified</param>
            ''' <param name="throwExceptions">True for throwing exception with details on first issue, False for suppressing exceptions and to just return the boolean result</param>
            ''' <returns>True for successful validation, False for failed validation</returns>
            Private Function Validate(checks As ValidationItem, throwExceptions As Boolean) As Boolean
                Return Validate(checks, throwExceptions, Nothing)
            End Function

            ''' <summary>
            ''' Validate user profile data and password complexity
            ''' </summary>
            ''' <param name="checks">Kind of validation</param>
            ''' <param name="throwExceptions">True for throwing exception with details on first issue, False for suppressing exceptions and to just return the boolean result</param>
            ''' <param name="newPassword">If a new password has to be set, this argument is required, otherwise no password complexity check will be done</param>
            ''' <returns>True for successful validation, False for failed validation</returns>
            Private Function Validate(checks As ValidationItem, throwExceptions As Boolean, newPassword As String) As Boolean
                If throwExceptions Then
                    Validate(checks, newPassword)
                    Return True
                Else
                    Try
                        Validate(checks, newPassword)
                        Return True
                    Catch ex As Exception
                        Return False
                    End Try
                End If
            End Function

            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared Function CentralConfig_AllowedValues_FieldCountry(webManager As WMSystem) As List(Of String)
                Dim AllowedValues As String = webManager.GlobalConfiguration.QueryStringConfigEntry("UserProfile_AllowedValues_FieldCountry")
                If AllowedValues Is Nothing Then
                    Return Nothing
                Else
                    Return New List(Of String)(AllowedValues.Split("|"c))
                End If
            End Function
            <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Shared Sub CentralConfig_AllowedValues_FieldCountrySetup(webManager As WMSystem, allowedValues As List(Of String))
                webManager.GlobalConfiguration.WriteConfigRecord(New GlobalConfiguration.ConfigRecord("UserProfile_AllowedValues_FieldCountry", Strings.Join(allowedValues.ToArray, "|"c)))
            End Sub

            ''' <summary>
            ''' Validate user profile data and password complexity
            ''' </summary>
            ''' <param name="checks">Kind of validation</param>
            ''' <param name="newPassword">If a new password has to be set, this argument is required, otherwise no password complexity check will be done</param>
            ''' <exception cref="FieldLimitedToAllowedValuesException">A field is limited to its defined values only</exception>
            ''' <exception cref="FlagValidation.RequiredFlagException">A security object requires an additional flag</exception>
            ''' <exception cref="RequiredFieldException">A security object authorized for the user requires the existance of an additional flag</exception>
            ''' <exception cref="PasswordRequiredException">A password must be specified to validate for the password complexity</exception>
            ''' <exception cref="PasswordComplexityException">The password doesn't match the complexity requirement for the user's access level</exception>
            ''' <exception cref="UserInfoConflictingUniqueKeysException">Unique keys already exist</exception>
            Private Sub Validate(checks As ValidationItem, newPassword As String)
                If checks = ValidationItem.All Or checks = ValidationItem.ProfileData Then
                    If Me.LoginName = String.Empty Then
                        Throw New RequiredFieldException("LoginName", "There must be a login name for this user account")
                    ElseIf Me.LoginName.Length > 50 Then
                        Throw New NotSupportedException("User login name too long (more than 50 characters)")
                    ElseIf Me.EMailAddress = String.Empty Then
                        Throw New RequiredFieldException("EMail", "The e-mail address is required")
                    ElseIf Me.PreferredLanguage1 Is Nothing Then
                        Throw New RequiredFieldException("1stPreferredLanguage", "Select the first preferred language, first")
                    ElseIf Me.AccessLevel Is Nothing Then
                        Throw New RequiredFieldException("AccessLevel", "Please select an access level, first")
                    ElseIf Me.AccessLevel.ServerGroups Is Nothing Then
                        Throw New ArgumentException("Invalid access level, it must contain at least one server group")
                    ElseIf Me.LoginDeleted = False AndAlso CompuMaster.camm.WebManager.InformationClassTools.IsValidContentOfUniqueFields(Me) = False Then
                        Dim Conflicts As CompuMaster.camm.WebManager.UserInfoConflictingUniqueKeysKeyValues() = CompuMaster.camm.WebManager.InformationClassTools.ExistingUsersConflictingWithContentOfUniqueFields(Me)
                        Throw New CompuMaster.camm.WebManager.UserInfoConflictingUniqueKeysException(Conflicts)
                    End If
                    Dim AllowedCountryValues As List(Of String) = CentralConfig_AllowedValues_FieldCountry(Me._WebManager)
                    If AllowedCountryValues IsNot Nothing AndAlso AllowedCountryValues.Count > 0 Then
                        If AllowedCountryValues.Contains(Me.Country) = False Then
                            Throw New FieldLimitedToAllowedValuesException("Country", Me.Country)
                        End If
                    End If
                End If
                If (checks = ValidationItem.All AndAlso (newPassword <> "" OrElse Me.IDLong = 0)) Or checks = ValidationItem.PasswordComplexityRequirements Then
                    If Trim(newPassword) = "" Then
                        Throw New PasswordRequiredException("User profile validation failed: password required")
                    End If
                    Dim ValidationResult As CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult
                    ValidationResult = Me._WebManager.PasswordSecurity(Me.AccessLevel.ID).ValidatePasswordComplexity(newPassword, Me)
                    Select Case ValidationResult
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
                            Throw New PasswordComplexityException(Me._WebManager.PasswordSecurity(Me.AccessLevel.ID).ErrorMessageComplexityPoints(1), ValidationResult)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMaximum
                            Throw New PasswordComplexityException("The password requires to be not bigger than " & Me._WebManager.PasswordSecurity(Me.AccessLevel.ID).RequiredMaximumPasswordLength & " characters!", ValidationResult)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_LengthMinimum
                            Throw New PasswordComplexityException("The password requires to be not smaller than " & Me._WebManager.PasswordSecurity(Me.AccessLevel.ID).RequiredPasswordLength & " characters!", ValidationResult)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                            Throw New PasswordComplexityException("The password shouldn't contain pieces of the user account profile, especially login name, first or last name!", ValidationResult)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Failure_Unspecified
                            Throw New PasswordComplexityException("There are some unknown errors when validating with the security policy for passwords!", ValidationResult)
                        Case CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success
                            'everything fine
                    End Select
                End If
                If checks = ValidationItem.All Or checks = ValidationItem.RequiredFlags Then
                    'Check all security objects allow-authorized by user authorization
                    Dim SecurityObjectIDs As New List(Of Integer) 'First, collect all security object IDs in a new list to prevent duplicate checks for the same security object ID because of standard+developer auth
                    Dim SecurityObjectAuthsForUser As SecurityObjectAuthorizationForUser()
                    SecurityObjectAuthsForUser = Me.AuthorizationsByRule().AllowRuleStandard
                    For MyCounter As Integer = 0 To SecurityObjectAuthsForUser.Length - 1
                        If SecurityObjectIDs.Contains(SecurityObjectAuthsForUser(MyCounter).SecurityObjectID) = False Then
                            SecurityObjectIDs.Add(SecurityObjectAuthsForUser(MyCounter).SecurityObjectID)
                        End If
                    Next
                    SecurityObjectAuthsForUser = Me.AuthorizationsByRule().AllowRuleDevelopers
                    For MyCounter As Integer = 0 To SecurityObjectAuthsForUser.Length - 1
                        If SecurityObjectIDs.Contains(SecurityObjectAuthsForUser(MyCounter).SecurityObjectID) = False Then
                            SecurityObjectIDs.Add(SecurityObjectAuthsForUser(MyCounter).SecurityObjectID)
                        End If
                    Next
                    For MyCounter As Integer = 0 To SecurityObjectAuthsForUser.Length - 1
                        Dim _RequiredApplicationFlags As String()
                        _RequiredApplicationFlags = SecurityObjectInformation.RequiredAdditionalFlags(SecurityObjectIDs(MyCounter), Me._WebManager)
                        Dim RequiredFlagsValidationResults As FlagValidation.FlagValidationResult() = FlagValidation.ValidateRequiredFlags(Me, _RequiredApplicationFlags, True)
                        If RequiredFlagsValidationResults.Length <> 0 Then
                            Throw New FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                        End If
                    Next
                    'Check all security objects allow-authorized by group memberships
                    Dim MembershipGroups As GroupInformation() = Me.MembershipsByRule().AllowRule
                    For MyCounter As Integer = 0 To MembershipGroups.Length - 1
                        Dim _RequiredApplicationFlags As String()
                        _RequiredApplicationFlags = GroupInformation.RequiredAdditionalFlags(MembershipGroups(MyCounter).ID, Me._WebManager)
                        Dim RequiredFlagsValidationResults As FlagValidation.FlagValidationResult() = FlagValidation.ValidateRequiredFlags(Me, _RequiredApplicationFlags, True)
                        If RequiredFlagsValidationResults.Length <> 0 Then
                            Throw New FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                        End If
                    Next
                End If
            End Sub

            Public Class FieldLimitedToAllowedValuesException
                Inherits UserInfoDataException

                Friend Sub New(profileFieldName As String, invalidValue As String)
                    MyBase.New("The profile field """ & profileFieldName & """ has value """ & invalidValue & """ which doesn't match the requirement of allowed values for this field")
                End Sub

            End Class

            Public Class PasswordComplexityException
                Inherits PasswordTooWeakException

                Friend Sub New(message As String, passwordComplexityValidationResult As CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult)
                    MyBase.New(message)
                    Me.PasswordComplexityValidationResult = passwordComplexityValidationResult
                End Sub

                Private _PasswordComplexityValidationResult As CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult
                Public Property PasswordComplexityValidationResult As CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult
                    Get
                        Return _PasswordComplexityValidationResult
                    End Get
                    Set(value As CompuMaster.camm.WebManager.WMSystem.WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult)
                        _PasswordComplexityValidationResult = value
                    End Set
                End Property

            End Class

            ''' <summary>
            '''     Set a new password for an user account and send required notification messages
            ''' </summary>
            ''' <param name="newPassword">A new password</param>
            ''' <param name="notificationProvider">An instance of a NotificationProvider class which handles the distribution of all required mails</param>
            Friend Sub SetUserPassword_Internal(ByVal newPassword As String, ByVal notificationProvider As Notifications.INotifications)
                Dim userInfo As UserInformation = Me

                If CompuMaster.camm.WebManager.WMSystem.IsSystemUser(userInfo.IDLong) Then
                    Throw New Exception("Can't set user details for system users")
                End If

                Dim MyNotifications As Notifications.INotifications
                If notificationProvider Is Nothing Then
                    MyNotifications = Me._WebManager.Notifications
                Else
                    MyNotifications = notificationProvider
                End If

                'Only update passwords if this is 
                '- a stand alone application
                '- a security operator (or supervisor)
                '- the user itself 
                'who is changing the password
                If HttpContext.Current Is Nothing Then 'a stand alone application
                    userInfo.SetUserPassword_Internal(newPassword, False)
                    Try
                        MyNotifications.NotificationForUser_ResettedPassword(userInfo, newPassword)
                    Catch ex As Exception
                        Me._WebManager.Log.Warn(ex)
                    End Try
                ElseIf userInfo.IDLong = Me._WebManager.CurrentUserID(SpecialUsers.User_Code) Then 'the user itself 
                    userInfo.SetUserPassword_Internal(newPassword, False)
                ElseIf Me._WebManager.System_IsSecurityOperator(Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous)) Then  'a security operator (or supervisor)
                    userInfo.SetUserPassword_Internal(newPassword, False)
                    Try
                        MyNotifications.NotificationForUser_ResettedPassword(userInfo, newPassword)
                    Catch ex As Exception
                        Me._WebManager.Log.Warn(ex)
                    End Try
                Else
                    Throw New Exception("No authorization to set passwords")
                End If
            End Sub

            ''' <summary>
            '''     Set new password for an user account (without further activities like mails)
            ''' </summary>
            ''' <param name="newPassword">A new password</param>
            ''' <param name="doNotLogSuccess">True to</param>
            Friend Sub SetUserPassword_Internal(ByVal newPassword As String, ByVal doNotLogSuccess As Boolean)
                Dim userInfo As UserInformation = Me

                Select Case userInfo.IDLong
                    Case SpecialUsers.User_Anonymous, SpecialUsers.User_Code, SpecialUsers.User_Public, SpecialUsers.User_Invalid, SpecialUsers.User_UpdateProcessor
                        Throw New ArgumentException("Invalid user ID", "userInfo")
                End Select

                'Trim+Validate password
                newPassword = Trim(newPassword)
                If newPassword = "" Then
                    Throw New ArgumentNullException("newPassword")
                End If
                Me.Validate(ValidationItem.PasswordComplexityRequirements, True, newPassword)

                Dim MyDBConn As New SqlConnection(Me._WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand
                MyCmd.CommandText = "AdminPrivate_UpdateUserPW"
                MyCmd.CommandType = CommandType.StoredProcedure

                ' Get parameter value and append parameter.
                Dim CryptingEngine As New CompuMaster.camm.WebManager.DefaultAlgoCryptor(Me._WebManager)
                Dim transformationResult As CryptoTransformationResult = CryptingEngine.TransformPlaintext(newPassword)
                MyCmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = userInfo.LoginName
                MyCmd.Parameters.Add("@NewPasscode", SqlDbType.VarChar).Value = transformationResult.TransformedText
                If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 123 Then
                    MyCmd.Parameters.Add("@DoNotLogSuccess", SqlDbType.Bit).Value = doNotLogSuccess
                    Dim IsUserChange As Boolean
                    If userInfo.IDLong = Me._WebManager.CurrentUserInfo(SpecialUsers.User_Anonymous).IDLong Then
                        IsUserChange = True
                    Else
                        IsUserChange = False
                    End If
                    MyCmd.Parameters.Add("@IsUserChange", SqlDbType.Bit).Value = IsUserChange
                End If
                If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 174 Then
                    MyCmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                End If
                If Me._WebManager.System_SupportsMultiplePasswordAlgorithms() Then
                    MyCmd.Parameters.Add("@LoginPWAlgorithm", SqlDbType.Int).Value = transformationResult.Algorithm
                    MyCmd.Parameters.Add("@LoginPWNonceValue", SqlDbType.VarBinary, 4096).Value = transformationResult.Noncevalue
                End If
                ' Create recordset by executing the command.
                MyCmd.Connection = MyDBConn
                Try
                    MyDBConn.Open()
                    MyCmd.ExecuteNonQuery()
                Finally
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyDBConn Is Nothing Then
                        If MyDBConn.State <> ConnectionState.Closed Then
                            MyDBConn.Close()
                        End If
                        MyDBConn.Dispose()
                    End If
                End Try
            End Sub

            ''' <summary>
            '''     Save changes to a user information object 
            ''' </summary>
            ''' <param name="newPassword">A new password</param>
            ''' <param name="notifications">The notifications for sending appropriate information to the user</param>
            ''' <param name="suppressUserNotifications">False sends e-mails regulary, true disables all user notifications</param>
            ''' <param name="suppressSecurityAdminNotifications">False sends e-mails regulary, true disables all admin notifications</param>
            ''' <returns>The ID of that user profile that has been saved</returns>
            ''' <remarks>
            ''' ATTENTION: if the user profile hasn't been fully loaded, changes might be lost because of internally caused full load commands initiated by the save method
            ''' </remarks>
            Friend Function Save_Internal(ByRef newPassword As String, notifications As Notifications.INotifications, suppressUserNotifications As Boolean, suppressSecurityAdminNotifications As Boolean) As Long
                Dim userInfo As UserInformation = Me

                'TODO: detect and send information about changed loginname to user --> requires extension of notification classes

                'Never change virtual system users
                If CompuMaster.camm.WebManager.WMSystem.IsSystemUser(userInfo.IDLong) Then
                    Throw New Exception("Can't set user details for system users")
                End If

                'Validate the information before writing back to the database
                If userInfo.LoginDeleted = True And userInfo.IDLong = Nothing Then
                    Throw New Exception("Login cannot be deleted when the Login ID is not existent")
                ElseIf userInfo.IDLong = Nothing AndAlso Not newPassword Is Nothing Then
                    'Validate password first
                    newPassword = Trim(newPassword)
                    If newPassword = "" Then
                        Throw New CompuMaster.camm.WebManager.PasswordRequiredException("Password must be provided")
                    ElseIf Not Me._WebManager.PasswordSecurity(userInfo.AccessLevel.ID).ValidatePasswordComplexity(newPassword, userInfo) = WMPasswordSecurityInspectionSeverity.PasswordComplexityValidationResult.Success Then
                        Throw New PasswordTooWeakException("Password doesn't match the current policy for passwords")
                    End If
                ElseIf userInfo.IDLong <> Nothing AndAlso Not newPassword Is Nothing Then
                    Throw New ArgumentException("Password cannot be set by this method. Please use System_SetUserPassword instead.", "NewPassword")
                End If
                Me.Validate(ValidationItem.All, True, newPassword)

                'Prepare data if action = delete
                If userInfo.LoginDeleted = True Then
                    userInfo.ExternalAccount = Nothing 'Prevent conflicts on a later date when accessing a user with the same, external account name
                End If

                'Prepare data if Gender = Undefined or MissingNameOrGroupOfPersons
                If userInfo.Gender = Sex.Undefined Or userInfo.Gender = Sex.MissingNameOrGroupOfPersons Then
                    'Setup the new/correct type of Gender now
                    If userInfo.FirstName = Nothing OrElse userInfo.LastName = Nothing Then
                        userInfo.Gender = Sex.MissingNameOrGroupOfPersons
                    Else
                        userInfo.Gender = Sex.Undefined
                    End If
                End If

                'Proceed now
                Dim WriteForUserID As Long = userInfo.IDLong
                Dim NewAccountCreated As Boolean = False
                Dim MyConn As New SqlConnection(Me._WebManager.ConnectionString)
                Try
                    MyConn.Open()

                    If userInfo.LoginDeleted = True Then
                        'will be resetted to False again later if it exists
                        Dim MyCmd As New SqlCommand("AdminPrivate_DeleteUser", MyConn)
                        MyCmd.CommandType = CommandType.StoredProcedure
                        MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userInfo.IDLong
                        Dim _DBVersion As Version = Setup.DatabaseUtils.Version(Me._WebManager, True)
                        If _DBVersion.Build >= 138 Then  'Newer
                            MyCmd.Parameters.Add("@AdminUserID", SqlDbType.Int).Value = Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                        End If
                        MyCmd.ExecuteNonQuery()
                        MyCmd.Dispose()
                        MyCmd = Nothing
                    End If

                    Dim IsUserChange As Boolean
                    Dim IsNewUser As Boolean
                    If userInfo.IDLong = Nothing Then

                        IsNewUser = True

                        'create new user account (with a temporary, empty password)
                        Dim MyCmd As New SqlCommand
                        MyCmd.Connection = MyConn
                        MyCmd.CommandText = "AdminPrivate_CreateUserAccount"
                        MyCmd.CommandType = CommandType.StoredProcedure

                        MyCmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = userInfo.LoginName
                        MyCmd.Parameters.Add("@Passcode", SqlDbType.VarChar, 4096).Value = ""
                        MyCmd.Parameters.Add("@WebApplication", SqlDbType.NVarChar, 1024).Value = DBNull.Value
                        MyCmd.Parameters.Add("@ServerIP", SqlDbType.NVarChar).Value = Me._WebManager.CurrentServerIdentString
                        MyCmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = userInfo.Company
                        Select Case userInfo.Gender
                            Case Sex.Feminine
                                MyCmd.Parameters.Add("@Anrede", SqlDbType.NVarChar).Value = "Ms."
                            Case Sex.Masculine
                                MyCmd.Parameters.Add("@Anrede", SqlDbType.NVarChar).Value = "Mr."
                            Case Else
                                MyCmd.Parameters.Add("@Anrede", SqlDbType.NVarChar).Value = ""
                        End Select
                        MyCmd.Parameters.Add("@Titel", SqlDbType.NVarChar).Value = userInfo.AcademicTitle
                        MyCmd.Parameters.Add("@Vorname", SqlDbType.NVarChar).Value = userInfo.FirstName
                        MyCmd.Parameters.Add("@Nachname", SqlDbType.NVarChar).Value = userInfo.LastName
                        MyCmd.Parameters.Add("@Namenszusatz", SqlDbType.NVarChar).Value = userInfo.NameAddition
                        MyCmd.Parameters.Add("@eMail", SqlDbType.NVarChar).Value = userInfo.EMailAddress
                        MyCmd.Parameters.Add("@Strasse", SqlDbType.NVarChar).Value = userInfo.Street
                        MyCmd.Parameters.Add("@PLZ", SqlDbType.NVarChar).Value = userInfo.ZipCode
                        MyCmd.Parameters.Add("@Ort", SqlDbType.NVarChar).Value = userInfo.Location
                        MyCmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = userInfo.State
                        MyCmd.Parameters.Add("@Land", SqlDbType.NVarChar).Value = userInfo.Country
                        MyCmd.Parameters.Add("@1stPreferredLanguage", SqlDbType.Int).Value = userInfo.PreferredLanguage1.ID
                        MyCmd.Parameters.Add("@2ndPreferredLanguage", SqlDbType.Int).Value = IIf(userInfo.PreferredLanguage2.ID = Nothing, DBNull.Value, userInfo.PreferredLanguage2.ID)
                        MyCmd.Parameters.Add("@3rdPreferredLanguage", SqlDbType.Int).Value = IIf(userInfo.PreferredLanguage3.ID = Nothing, DBNull.Value, userInfo.PreferredLanguage3.ID)
                        MyCmd.Parameters.Add("@AccountAccessability", SqlDbType.Int).Value = userInfo.AccessLevel.ID
                        MyCmd.Parameters.Add("@CustomerNo", SqlDbType.NVarChar).Value = DBNull.Value
                        MyCmd.Parameters.Add("@SupplierNo", SqlDbType.NVarChar).Value = DBNull.Value
                        If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 123 Then
                            If Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous) = SpecialUsers.User_Anonymous Then
                                'The user was anonymous and now he gets a named user
                                IsUserChange = True
                            Else
                                IsUserChange = False
                            End If
                            MyCmd.Parameters.Add("@IsUserChange", SqlDbType.Bit).Value = IsUserChange
                        End If
                        If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 174 Then
                            MyCmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                        End If


                        Dim Result As Object = MyCmd.ExecuteScalar()

                        If IsDBNull(Result) Then
                            Me._WebManager.Log.RuntimeException(Me._WebManager.Internationalization.ErrorUnknown, "Unexpected error creating user profile")
                        ElseIf CType(Result, Integer) = 0 Then
                            'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                            Dim WorkaroundEx As New Exception("")
                            Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                            Try
                                WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                            Catch
                            End Try
                            Me._WebManager.Log.RuntimeWarning("User """ & userInfo.LoginName & """ already exists", WorkaroundStackTrace, DebugLevels.Medium_LoggingOfDebugInformation, False, False)
                            Throw New Exception(Me._WebManager.Internationalization.ErrorUserAlreadyExists)
                        ElseIf CType(Result, Integer) = -1 Then
                            WriteForUserID = CType(Me._WebManager.System_GetUserID(userInfo.LoginName), Long)
                            userInfo.SetNewUserID(WriteForUserID) 'Save new user id in the user info object
                            NewAccountCreated = True
                        ElseIf CType(Result, Integer) = -10 Then
                            Me._WebManager.Log.RuntimeException(Me._WebManager.Internationalization.ErrorServerConfigurationError, "The current server '" & Me._WebManager.CurrentServerIdentString & "' is not a member of this camm webmanager instance")
                        Else
                            Me._WebManager.Log.RuntimeException(Me._WebManager.Internationalization.ErrorUnknown, "Unexpected error creating user profile")
                        End If
                        MyCmd.Dispose()
                        MyCmd = Nothing

                        'Notifications class
                        Dim CurNotifications As CompuMaster.camm.WebManager.Notifications.INotifications
                        If notifications Is Nothing Then
                            CurNotifications = Me._WebManager.Notifications
                        Else
                            CurNotifications = notifications
                        End If

                        'Set password
                        Dim PasswordMustBeSend As Boolean
                        If newPassword = "" Then
                            If suppressUserNotifications = True OrElse GetType(CompuMaster.camm.WebManager.Notifications.NoNotifications).IsInstanceOfType(CurNotifications) Then
                                'No e-mails will go out; no auto-generated password could be communicated
                                Throw New PasswordRequiredException("Password required when creating account with e-mails suppressed by using the CompuMaster.camm.WebManager.Notifications.NoNotifications class")
                            End If
                            newPassword = Trim(Me._WebManager.PasswordSecurity(userInfo.AccessLevel.ID).CreateRandomSecurePassword)
                            PasswordMustBeSend = True
                        End If
                        Me.SetUserPassword_Internal(newPassword, True) 'previously reloaded userInfo - but not clear why: New CompuMaster.camm.WebManager.WMSystem.UserInformation(WriteForUserID, Me._WebManager)

                        'Send e-mail
                        If suppressUserNotifications = False Then
                            Try
                                'if current logged on user is anonymous, then the user has created his account himself
                                If Not HttpContext.Current Is Nothing AndAlso Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous) = SpecialUsers.User_Anonymous Then
                                    'No user logged in --> we have created our own account, now (the session wouldn't get the user information before returning from this method)
                                    If PasswordMustBeSend Then
                                        CurNotifications.NotificationForUser_Welcome_UserRegisteredByHimself(userInfo, newPassword)
                                    Else
                                        CurNotifications.NotificationForUser_Welcome_UserRegisteredByHimself(userInfo)
                                    End If
                                Else
                                    'Created by code or by an already logged in user (= another user)
                                    CurNotifications.NotificationForUser_Welcome_UserHasBeenCreated(userInfo, newPassword)
                                End If
                            Catch ex As Exception
                                Me._WebManager.Log.RuntimeWarning("Password for account """ & New CompuMaster.camm.WebManager.WMSystem.UserInformation(WriteForUserID, Me._WebManager).LoginName & """ has been resetted, but the mail couldn't be sent (" & ex.Message & ")", ex.StackTrace, DebugLevels.NoDebug, False, False)
                            End Try
                        End If
                        If suppressSecurityAdminNotifications = False Then
                            Try
                                'Send e-mails to all security administratiors for reviewing
                                CurNotifications.NotificationForSecurityAdministration_ReviewNewUserAccount(userInfo)
                            Catch ex As Exception
                                Me._WebManager.Log.RuntimeWarning("Account """ & New CompuMaster.camm.WebManager.WMSystem.UserInformation(WriteForUserID, Me._WebManager).LoginName & """ has been created, the user has got his welcome mail, but one or more security administrators haven't got their notification mail (" & ex.Message & ")", ex.StackTrace, DebugLevels.NoDebug, False, False)
                            End Try
                        End If
                    End If
                    If Not userInfo.LoginDeleted Then 'UserInfo.ID <> Nothing orelse (userinfo.ID = nothing andalsoThen
                        'update existing user account
                        'UserInfo.LoginLockedTemporary will be resetted here in this method!!

                        'Login name changes
                        If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 178 Then
                            Dim MyLogonNameChangeCmd As New SqlCommand
                            MyLogonNameChangeCmd.Connection = MyConn
                            MyLogonNameChangeCmd.CommandType = CommandType.StoredProcedure
                            MyLogonNameChangeCmd.CommandText = "dbo.AdminPrivate_RenameLoginName"
                            MyLogonNameChangeCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = userInfo.IDLong
                            MyLogonNameChangeCmd.Parameters.Add("@LogonName", SqlDbType.NVarChar).Value = userInfo.LoginName
                            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyLogonNameChangeCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.None)
                        End If

                        'General fields
                        Dim MyCmd As New SqlCommand
                        MyCmd.Connection = MyConn
                        MyCmd.CommandText = "AdminPrivate_UpdateUserDetails"
                        MyCmd.CommandType = CommandType.StoredProcedure

                        MyCmd.Parameters.Add("@CurUserID", SqlDbType.Int).Value = userInfo.IDLong
                        MyCmd.Parameters.Add("@WebApplication", SqlDbType.NVarChar, 1024).Value = DBNull.Value
                        MyCmd.Parameters.Add("@Company", SqlDbType.NVarChar).Value = IIf(userInfo.Company = "", DBNull.Value, userInfo.Company)
                        Select Case userInfo.Gender
                            Case Sex.Feminine
                                MyCmd.Parameters.Add("@Anrede", SqlDbType.NVarChar).Value = "Ms."
                            Case Sex.Masculine
                                MyCmd.Parameters.Add("@Anrede", SqlDbType.NVarChar).Value = "Mr."
                            Case Else
                                MyCmd.Parameters.Add("@Anrede", SqlDbType.NVarChar).Value = ""
                        End Select
                        MyCmd.Parameters.Add("@Titel", SqlDbType.NVarChar).Value = IIf(userInfo.AcademicTitle = "", DBNull.Value, userInfo.AcademicTitle)
                        MyCmd.Parameters.Add("@Vorname", SqlDbType.NVarChar).Value = userInfo.FirstName
                        MyCmd.Parameters.Add("@Nachname", SqlDbType.NVarChar).Value = userInfo.LastName
                        MyCmd.Parameters.Add("@Namenszusatz", SqlDbType.NVarChar).Value = IIf(userInfo.NameAddition = "", DBNull.Value, userInfo.NameAddition)
                        MyCmd.Parameters.Add("@eMail", SqlDbType.NVarChar).Value = userInfo.EMailAddress
                        MyCmd.Parameters.Add("@Strasse", SqlDbType.NVarChar).Value = IIf(userInfo.Street = "", DBNull.Value, userInfo.Street)
                        MyCmd.Parameters.Add("@PLZ", SqlDbType.NVarChar).Value = IIf(userInfo.ZipCode = "", DBNull.Value, userInfo.ZipCode)
                        MyCmd.Parameters.Add("@Ort", SqlDbType.NVarChar).Value = IIf(userInfo.Location = "", DBNull.Value, userInfo.Location)
                        MyCmd.Parameters.Add("@State", SqlDbType.NVarChar).Value = IIf(userInfo.State = "", DBNull.Value, userInfo.State)
                        MyCmd.Parameters.Add("@Land", SqlDbType.NVarChar).Value = IIf(userInfo.Country = "", DBNull.Value, userInfo.Country)
                        MyCmd.Parameters.Add("@1stPreferredLanguage", SqlDbType.Int).Value = userInfo.PreferredLanguage1.ID
                        MyCmd.Parameters.Add("@2ndPreferredLanguage", SqlDbType.Int).Value = IIf(userInfo.PreferredLanguage2.ID = Nothing, DBNull.Value, userInfo.PreferredLanguage2.ID)
                        MyCmd.Parameters.Add("@3rdPreferredLanguage", SqlDbType.Int).Value = IIf(userInfo.PreferredLanguage3.ID = Nothing, DBNull.Value, userInfo.PreferredLanguage3.ID)
                        MyCmd.Parameters.Add("@AccountAccessability", SqlDbType.Int).Value = userInfo.AccessLevel.ID
                        MyCmd.Parameters.Add("@LoginDisabled", SqlDbType.Bit).Value = userInfo.LoginDisabled
                        MyCmd.Parameters.Add("@LoginLockedTill", SqlDbType.DateTime).Value = IIf(userInfo.LoginLockedTemporaryTill = Nothing, DBNull.Value, userInfo.LoginLockedTemporaryTill)
                        MyCmd.Parameters.Add("@CustomerNo", SqlDbType.NVarChar).Value = DBNull.Value
                        MyCmd.Parameters.Add("@SupplierNo", SqlDbType.NVarChar).Value = DBNull.Value
                        If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 123 Then
                            MyCmd.Parameters.Add("@DoNotLogSuccess", SqlDbType.Bit).Value = IsNewUser 'Not log the change if there is already a user-created-log-item
                            If IsNewUser Then
                                'Is already defined by the creation of the new user block
                            ElseIf userInfo.IDLong = Me._WebManager.CurrentUserInfo(SpecialUsers.User_Anonymous).IDLong Then
                                IsUserChange = True
                            Else
                                IsUserChange = False
                            End If
                            MyCmd.Parameters.Add("@IsUserChange", SqlDbType.Bit).Value = IsUserChange
                        End If
                        If Setup.DatabaseUtils.Version(Me._WebManager, True).Build >= 174 Then
                            MyCmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = Me._WebManager.CurrentUserID(SpecialUsers.User_Anonymous)
                        End If

                        Dim result As Object = MyCmd.ExecuteScalar()
                        If IsDBNull(result) Then
                            Throw New Exception("Unexpected error writing user profile")
                        ElseIf CType(result, Integer) = -1 Then
                            'Fine :)
                        Else
                            Throw New Exception("Unexpected error writing user profile")
                        End If
                        MyCmd.Dispose()
                        MyCmd = Nothing

                    End If


                    If Not userInfo.LoginDeleted Then
                        'update additional flags (table log_users)
                        For Each MyFlag As String In userInfo.AdditionalFlags
                            DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, MyFlag, userInfo.AdditionalFlags(MyFlag), True)
                        Next
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Company", userInfo.Company, True)
                        Select Case userInfo.Gender
                            Case Sex.Feminine
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Sex", "w", True)
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Addresses", "Ms." & CType(IIf(userInfo.AcademicTitle <> "", " " & userInfo.AcademicTitle, ""), String), True)
                            Case Sex.Masculine
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Sex", "m", True)
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Addresses", "Mr." & CType(IIf(userInfo.AcademicTitle <> "", " " & userInfo.AcademicTitle, ""), String), True)
                            Case Sex.Undefined
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Sex", "u", True)
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Addresses", "", True)
                            Case Else 'Sex.MissingNameOrGroupOfPersons
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Sex", "g", True)
                                DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Addresses", "", True)
                        End Select
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "FirstName", userInfo.FirstName, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "LastName", userInfo.LastName, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "NameAddition", userInfo.NameAddition, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "email", userInfo.EMailAddress, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Street", userInfo.Street, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "ZipCode", userInfo.ZipCode, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Location", userInfo.Location, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "State", userInfo.State, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "Country", userInfo.Country, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "1stPreferredLanguage", CType(userInfo.PreferredLanguage1.ID, String), True)
                        If userInfo.PreferredLanguage2.ID = Nothing Then DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "2ndPreferredLanguage", DBNull.Value.ToString, True) Else DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "2ndPreferredLanguage", CType(userInfo.PreferredLanguage2.ID, String), True)
                        If userInfo.PreferredLanguage3.ID = Nothing Then DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "3rdPreferredLanguage", DBNull.Value.ToString, True) Else DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "3rdPreferredLanguage", CType(userInfo.PreferredLanguage3.ID, String), True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "InitAuthorizationsDone", CType(IIf(userInfo.AccountAuthorizationsAlreadySet = True, "1", Nothing), String), True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "AccountProfileValidatedByEMailTest", CType(IIf(userInfo.AccountProfileValidatedByEMailTest = True, "1", Nothing), String), True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "AutomaticLogonAllowedByMachineToMachineCommunication", CType(IIf(userInfo.AutomaticLogonAllowedByMachineToMachineCommunication = True, "1", Nothing), String), True)  'WARNING: flag name too long, saved in table as: "AutomaticLogonAllowedByMachineToMachineCommunicati"
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "ExternalAccount", userInfo.ExternalAccount, True)
                        DataLayer.Current.SetUserDetail(Me._WebManager, MyConn, WriteForUserID, "IsImpersonationUser", userInfo.AdditionalFlags("IsImpersonationUser"), True)
                    End If
                Finally
                    CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyConn)
                End Try

                Return WriteForUserID

            End Function

            ''' <summary>
            '''     Set a new password for an user account and sends required notification messages
            ''' </summary>
            ''' <param name="newPassword">A new password</param>
            Public Sub SetPassword(ByVal newPassword As String)
                SetPassword(newPassword, _WebManager.Notifications)
            End Sub

            ''' <summary>
            '''     Set a new password for an user account and sends required notification messages
            ''' </summary>
            ''' <param name="newPassword">A new password</param>
            ''' <param name="notificationProvider">An instance of a NotificationProvider class which handles the distribution of all required mails</param>
            Public Sub SetPassword(ByVal newPassword As String, ByVal notificationProvider As Notifications.INotifications)
                Me.SetUserPassword_Internal(newPassword, notificationProvider)
            End Sub

            ''' <summary>
            '''     Set a new password for an user account and sends required notification messages
            ''' </summary>
            ''' <param name="newPassword">A new password</param>
            ''' <param name="suppressNotifications">True disables all mail transfer, false sends the configured notification message</param>
            Public Sub SetPassword(ByVal newPassword As String, ByVal suppressNotifications As Boolean)
                If suppressNotifications Then
                    SetPassword(newPassword, New CompuMaster.camm.WebManager.Notifications.NoNotifications(_WebManager))
                Else
                    SetPassword(newPassword, _WebManager.Notifications)
                End If
            End Sub
#End Region

            ''' <summary>
            '''     The general salutation for a person, e. g. "Mr. Bell" or "Ms. Dr. van Vrede" or (if gender is undefined) "Jonathan Taylor" or (if gender is a group) an empty string
            ''' </summary>
            ''' <returns>Empty string in case of gender type group of persons</returns>
            Public Function Salutation() As String
                'SalutationFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                'SalutationMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                'UndefinedGender = "{FullName}"
                'MissingNameOrGroupOfPersons = ""
                Select Case Me.Gender
                    Case WMSystem.Sex.Feminine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaFeminin)
                        End If
                    Case WMSystem.Sex.Masculine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaMasculin)
                        End If
                    Case WMSystem.Sex.Undefined
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUndefinedGender)
                    Case Else 'wmsystem.Sex.MissingNameOrGroupOfPersons
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaGroup)
                End Select
            End Function

            ''' <summary>
            '''     The simple salutation for a person, "Mr. " or "Ms. "
            ''' </summary>
            Public Function SalutationMrOrMs() As String
                Select Case Me.Gender
                    Case WMSystem.Sex.Feminine
                        Return Me._WebManager.Internationalization.UserManagementAddressesMs
                    Case WMSystem.Sex.Masculine
                        Return Me._WebManager.Internationalization.UserManagementAddressesMr
                    Case Else 'WMSystem.Sex.Undefined, WMSystem.Sex.MissingNameOrGroupOfPersons
                        Return ""
                End Select
            End Function

            ''' <summary>
            '''     The salutation for mail purposes, e. g. "Dear Mr. van Vrede, " or "Dear Mr. Dr. van Vrede, " or (if gender is undefined) "Dear Dr. Heribert van Vrede, " or (if gender is a group) "Dear Sirs, "
            ''' </summary>
            Public Function SalutationInMails() As String
                'UserManagementSalutationInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                'UserManagementSalutationInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                'UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                'UserManagementSalutationFormulaInMailsGroup = "Dear Sirs, "
                Select Case Me.Gender
                    Case WMSystem.Sex.Feminine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsFeminin)
                        End If
                    Case WMSystem.Sex.Masculine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsMasculin)
                        End If
                    Case WMSystem.Sex.Undefined
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsUndefinedGender)
                    Case WMSystem.Sex.MissingNameOrGroupOfPersons
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaInMailsGroup)
                    Case Else
                        Throw New Exception("Invalid gender value")
                End Select
            End Function

            ''' <summary>
            '''     The salutation for mail purposes, e. g. "Hello Mr. Bell, " or "Hello Ms. Dr. van Vrede, " or (if gender is undefined) "Hello Dr. Heribert van Vrede, " or (if gender is group) "Hello together, "
            ''' </summary>
            Public Function SalutationUnformal() As String
                'SalutationUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                'SalutationUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                'SalutationUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                'SalutationUnformalGroup = "Hello together, "
                Select Case Me.Gender
                    Case WMSystem.Sex.Feminine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalFeminin)
                        End If
                    Case WMSystem.Sex.Masculine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalMasculin)
                        End If
                    Case WMSystem.Sex.Undefined
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalUndefinedGender)
                    Case Else 'WMSystem.Sex.MissingNameOrGroupOfPersons
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalGroup)
                End Select
            End Function

            ''' <summary>
            '''     The salutation for mail purposes, e. g. "Hello Roger, " or "Hello Claire, " or (if gender is group) "Hello together, "
            ''' </summary>
            Public Function SalutationUnformalWithFirstName() As String
                'SalutationUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                'SalutationUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                'SalutationUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                'SalutationUnformalWithFirstNameGroup = "Hello together, "
                Select Case Me.Gender
                    Case WMSystem.Sex.Feminine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameFeminin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameFeminin)
                        End If
                    Case WMSystem.Sex.Masculine
                        If Me.LastName = Nothing AndAlso Me.SalutationTextModuleRequiresLastName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameGroup)
                        ElseIf Me.FirstName = Nothing AndAlso Me.SalutationTextModuleRequiresFirstName(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameMasculin) Then
                            'return the result as for a missing-name/group-of-persons user
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameGroup)
                        Else
                            'return the result as regular
                            Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameMasculin)
                        End If
                    Case WMSystem.Sex.Undefined
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender)
                    Case Else 'WMSystem.Sex.MissingNameOrGroupOfPersons
                        Return Me.ResolveSalutationTextModule(Me._WebManager.Internationalization.UserManagementSalutationFormulaUnformalWithFirstNameGroup)
                End Select
            End Function

            ''' <summary>
            ''' Replace inner text modules by their current values
            ''' </summary>
            ''' <param name="text">A text module which may contain some other text modules</param>
            ''' <returns>The finally resolved text</returns>
            Private Function ResolveSalutationTextModule(ByVal text As String) As String
                If text Is Nothing OrElse text.IndexOf("{"c) < 0 Then
                    Return text
                Else
                    'Name fields
                    text = text.Replace("{FullName}", Me.FullName)
                    text = text.Replace("{AcademicTitle}", Me.AcademicTitle)
                    text = text.Replace("{FirstName}", Me.FirstName)
                    text = text.Replace("{NameAddition}", Me.NameAddition)
                    text = text.Replace("{LastName}", Me.FirstName)
                    text = text.Replace("{SalutationNameOnly}", Me.SalutationNameOnly)
                    'Salutation fields
                    text = text.Replace("{SalutationUnformalFeminin}", Me._WebManager.Internationalization.UserManagementSalutationUnformalFeminin)
                    text = text.Replace("{SalutationUnformalMasculin}", Me._WebManager.Internationalization.UserManagementSalutationUnformalMasculin)
                    text = text.Replace("{SalutationUnformalUndefinedGender}", Me._WebManager.Internationalization.UserManagementSalutationUnformalUndefinedGender)
                    text = text.Replace("{SalutationInMailsFeminin}", Me._WebManager.Internationalization.UserManagementEMailTextDearMs)
                    text = text.Replace("{SalutationInMailsMasculin}", Me._WebManager.Internationalization.UserManagementEMailTextDearMr)
                    text = text.Replace("{SalutationInMailsUndefinedGender}", Me._WebManager.Internationalization.UserManagementEMailTextDearUndefinedGender)
                    text = text.Replace("{SalutationFeminin}", Me._WebManager.Internationalization.UserManagementAddressesMs)
                    text = text.Replace("{SalutationMasculin}", Me._WebManager.Internationalization.UserManagementAddressesMr)
                    'Now it must contain 0 brackets
                    If text.IndexOf("{"c) >= 0 Then
                        Throw New NotSupportedException("Invalid variable names in brackets: " & text)
                    End If
                    Return text
                End If
            End Function

            ''' <summary>
            ''' Is the first name required by the text module for this salutation formula for the replace engine in method ResolveSalutationTextModule?
            ''' </summary>
            ''' <param name="text"></param>
            Private Function SalutationTextModuleRequiresFirstName(ByVal text As String) As Boolean
                If text.IndexOf("{FirstName}") >= 0 Then
                    'Hit found - first name is required
                    Return True
                ElseIf text.IndexOf("{FullName}") >= 0 Then
                    'Hit found - first name is required
                    Return True
                Else
                    Return False
                End If
            End Function

            ''' <summary>
            ''' Is the last name required by the text module for this salutation formula for the replace engine in method ResolveSalutationTextModule?
            ''' </summary>
            ''' <param name="text"></param>
            Private Function SalutationTextModuleRequiresLastName(ByVal text As String) As Boolean
                If text.IndexOf("{LastName}") >= 0 Then
                    'Hit found - last name is required
                    Return True
                ElseIf text.IndexOf("{FullName}") >= 0 Then
                    'Hit found - last name is required
                    Return True
                ElseIf text.IndexOf("{SalutationNameOnly}") >= 0 Then
                    'Hit found - last name is required
                    Return True
                Else
                    Return False
                End If
            End Function

            ''' <summary>
            '''     An optional academic title, typically 'Prof.' or 'Dr.'
            ''' </summary>
            ''' <value></value>
            Public Property AcademicTitle() As String Implements IUserInformation.AcademicTitle
                Get
                    Return _AcademicTitle
                End Get
                Set(ByVal Value As String)
                    _AcademicTitle = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The street
            ''' </summary>
            ''' <value></value>
            Public Property Street() As String Implements IUserInformation.Street
                Get
                    Return _Street
                End Get
                Set(ByVal Value As String)
                    _Street = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The zip code
            ''' </summary>
            ''' <value></value>
            Public Property ZipCode() As String Implements IUserInformation.ZipCode
                Get
                    Return _ZipCode
                End Get
                Set(ByVal Value As String)
                    _ZipCode = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The location or city
            ''' </summary>
            ''' <value></value>
            Public Property Location() As String Implements IUserInformation.Location
                Get
                    Return _City
                End Get
                Set(ByVal Value As String)
                    _City = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The state in the country
            ''' </summary>
            ''' <value></value>
            Public Property State() As String Implements IUserInformation.State
                Get
                    Return _State
                End Get
                Set(ByVal Value As String)
                    _State = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The country name 
            ''' </summary>
            ''' <value></value>
            Public Property Country() As String Implements IUserInformation.Country
                Get
                    Return _Country
                End Get
                Set(ByVal Value As String)
                    _Country = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The primary preferred language or market
            ''' </summary>
            ''' <value></value>
            Public Property PreferredLanguage1() As LanguageInformation
                Get
                    If _PreferredLanguage1 Is Nothing Then
                        _PreferredLanguage1 = New LanguageInformation(_PreferredLanguage1ID, _WebManager)
                    End If
                    Return New LanguageInformation(_PreferredLanguage1ID, _WebManager)
                End Get
                Set(ByVal Value As LanguageInformation)
                    _PreferredLanguage1 = Value
                    _PreferredLanguage1ID = Value.ID
                End Set
            End Property
            ''' <summary>
            '''     The second preferred language or market
            ''' </summary>
            ''' <value></value>
            Public Property PreferredLanguage2() As LanguageInformation
                Get
                    If _PreferredLanguage2 Is Nothing Then
                        _PreferredLanguage2 = New LanguageInformation(_PreferredLanguage2ID, _WebManager)
                    End If
                    Return New LanguageInformation(_PreferredLanguage2ID, _WebManager)
                End Get
                Set(ByVal Value As LanguageInformation)
                    _PreferredLanguage2 = Value
                    _PreferredLanguage2ID = Value.ID
                End Set
            End Property
            ''' <summary>
            '''     The third preferred language or market
            ''' </summary>
            ''' <value></value>
            Public Property PreferredLanguage3() As LanguageInformation
                Get
                    If _PreferredLanguage3 Is Nothing Then
                        _PreferredLanguage3 = New LanguageInformation(_PreferredLanguage3ID, _WebManager)
                    End If
                    Return New LanguageInformation(_PreferredLanguage3ID, _WebManager)
                End Get
                Set(ByVal Value As LanguageInformation)
                    _PreferredLanguage3 = Value
                    _PreferredLanguage3ID = Value.ID
                End Set
            End Property
            ''' <summary>
            '''     An additional pre-name, e. g. 'de' in the name 'Jean-Claude de Verheugen'
            ''' </summary>
            ''' <value></value>
            Public Property NameAddition() As String Implements IUserInformation.NameAddition
                Get
                    Return _NameAddition
                End Get
                Set(ByVal Value As String)
                    _NameAddition = Utils.StringNotNothingOrEmpty(Value)
                End Set
            End Property
            ''' <summary>
            '''     The gender of the user
            ''' </summary>
            ''' <value></value>
            Public Property Gender() As Sex
                Get
                    If _Sex = WMSystem.Sex.MissingNameOrGroupOfPersons OrElse _Sex = WMSystem.Sex.Undefined Then
                        If Me.FirstName <> Nothing AndAlso Me.LastName <> Nothing Then
                            Return WMSystem.Sex.Undefined
                        Else
                            Return WMSystem.Sex.MissingNameOrGroupOfPersons
                        End If
                    Else
                        Return _Sex
                    End If
                End Get
                Set(ByVal Value As Sex)
                    _Sex = Value
                End Set
            End Property
            <Obsolete("use Gender instead"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property Sex() As Sex
                Get
                    If _Sex = WMSystem.Sex.MissingNameOrGroupOfPersons OrElse _Sex = WMSystem.Sex.Undefined Then
                        If Me.FirstName <> Nothing AndAlso Me.LastName <> Nothing Then
                            Return WMSystem.Sex.Undefined
                        Else
                            Return WMSystem.Sex.MissingNameOrGroupOfPersons
                        End If
                    Else
                        Return _Sex
                    End If
                End Get
                Set(ByVal Value As Sex)
                    _Sex = Value
                End Set
            End Property
            Private Property _Gender() As IUserInformation.GenderType Implements IUserInformation.Gender
                Get
                    Return CType(Me.Gender, IUserInformation.GenderType)
                End Get
                Set(ByVal Value As IUserInformation.GenderType)
                    _Sex = CType(Value, Sex)
                End Set
            End Property
            ''' <summary>
            '''     Login has been disabled
            ''' </summary>
            ''' <value></value>
            Public Property LoginDisabled() As Boolean
                Get
                    Return _LoginDisabled
                End Get
                Set(ByVal Value As Boolean)
                    _LoginDisabled = Value
                End Set
            End Property
            ''' <summary>
            '''     Get/set the temporary lock state
            ''' </summary>
            ''' <value></value>
            Public Property LoginLockedTemporary() As Boolean
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _LoginLockedTemporary
                End Get
                Set(ByVal Value As Boolean)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _LoginLockedTemporary = Value
                    If Value = True Then
                        If Not _LoginLockedTemporaryTill = Nothing Then
                            _LoginLockedTemporaryTill = Now
                        End If
                    Else
                        _LoginLockedTemporaryTill = Nothing
                    End If
                End Set
            End Property
            ''' <summary>
            '''     Login has been temporary locked till this date
            ''' </summary>
            ''' <value></value>
            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Property LoginLockedTemporaryTill() As DateTime
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _LoginLockedTemporaryTill
                End Get
                Set(ByVal Value As DateTime)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    If Value = Nothing Then
                        _LoginLockedTemporary = False
                        _LoginLockedTemporaryTill = Nothing
                    Else
                        _LoginLockedTemporary = True
                        _LoginLockedTemporaryTill = Value
                    End If
                End Set
            End Property

            Private _AccountSuccessfullLogins As Integer
            ''' <summary>
            '''     The number of logins since the account has been created
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccountSuccessfullLogins() As Integer
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountSuccessfullLogins
                End Get
            End Property

            Private _AccountLoginFailures As Integer
            ''' <summary>
            '''     The number of failed logins (this number will be resetted after every successfull login)
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccountLoginFailures() As Integer
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountLoginFailures
                End Get
            End Property

            Private _AccountCreatedOn As DateTime
            ''' <summary>
            '''     The date and time when the user account has been created
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccountCreatedOn() As DateTime
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountCreatedOn
                End Get
            End Property

            Private _AccountModifiedOn As DateTime
            ''' <summary>
            '''     The date and time when the account has been updated the last time
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccountModifiedOn() As DateTime
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountModifiedOn
                End Get
            End Property

            Private _AccountLastLoginOn As DateTime
            ''' <summary>
            '''     The date and time when the user logged in on the last time
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccountLastLoginOn() As DateTime
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountLastLoginOn
                End Get
            End Property

            Private _AccountLastLoginFromAddress As String
            ''' <summary>
            '''     The last login address of the remote client
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property AccountLastLoginFromAddress() As String
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountLastLoginFromAddress
                End Get
            End Property

            ''' <summary>
            '''     Login has been deleted
            ''' </summary>
            ''' <value></value>
            Public Property LoginDeleted() As Boolean
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _LoginDeleted
                End Get
                Set(ByVal Value As Boolean)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _LoginDeleted = Value
                End Set
            End Property

            ''' <summary>
            '''     The groups list where the user is member of
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use MembershipsByRule instead")> Public ReadOnly Property Memberships() As CompuMaster.camm.WebManager.WMSystem.GroupInformation()
                Get
                    Return MembershipsByRule().Effective
                End Get
            End Property

            Private _MembershipsByRule As Security.MembershipItemsByRule
            ''' <summary>
            ''' Memberships of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property MembershipsByRule() As Security.MembershipItemsByRule
                Get
                    If Me._ID = 0 AndAlso _PartiallyLoadedDataCurrently Then 'prevent access to this property while the user hasn't been saved (ID = 0 will throw exception in following)
                        ReadCompleteUserInformation()
                    End If
                    If _MembershipsByRule Is Nothing Then
                        _MembershipsByRule = New Security.MembershipItemsByRule(_WebManager, _ID)
                    End If
                    Return _MembershipsByRule
                End Get
            End Property

            ''' <summary>
            '''     Add a membership to a user group (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The group ID</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Better use overloaded method which implements INotifications"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
            Public Sub AddMembership(ByVal groupID As Integer, ByVal notifications As WMNotifications)
                AddMembership(groupID, CType(notifications, Notifications.INotifications))
            End Sub

            ''' <summary>
            '''     Add a membership to a user group (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The group ID</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving. If the membership already exists, it won't be created for a 2nd time.
            ''' </remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub AddMembership(ByVal groupID As Integer, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddMembership(groupID, False, notifications)
            End Sub

            ''' <summary>
            ''' Validate if all required flags available to add allow-membership-relation
            ''' </summary>
            ''' <param name="groupID"></param>
            ''' <returns>Empty array if nothing missing</returns>
            Friend Function ValidateRequiredFlagsForGroupMembership(groupID As Integer, isDenyRule As Boolean) As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult()
                If isDenyRule Then
                    'scenario: a user might be allowed for a security object because he's on the allow-list (directly or indirectly) of a security object ANDALSO he's on the group-memberships-deny-list of a group which is on the access-denied-list of this security object
                    '--> no check required for the double-negative-deny-rule because this scenario explicitly requires an allow-rule - which is already validated
                    '--> BUT: the user might be on an allow-list but effectively he might not be allowed to this security object because of additional deny rules
                    Return New CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() {}
                Else
                    Dim RequiredApplicationFlags As String() = GroupInformation.RequiredAdditionalFlags(groupID, Me._WebManager)
                    Dim RequiredFlagsValidationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(Me, RequiredApplicationFlags, True)
                    Return RequiredFlagsValidationResults
                End If
            End Function

            ''' <summary>
            ''' Validate if all required flags available to add allow-membership-relation
            ''' </summary>
            ''' <param name="securityObjectID"></param>
            ''' <returns>Empty array if nothing missing</returns>
            Friend Function ValidateRequiredFlagsForSecurityObject(securityObjectID As Integer, isDenyRule As Boolean) As FlagValidation.FlagValidationResult()
                If isDenyRule Then
                    Return New CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() {}
                Else
                    Dim RequiredApplicationFlags As String() = SecurityObjectInformation.RequiredAdditionalFlags(securityObjectID, Me._WebManager)
                    Dim RequiredFlagsValidationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(Me, RequiredApplicationFlags, True)
                    Return RequiredFlagsValidationResults
                End If
            End Function

            ''' <summary>
            '''     Add a membership to a user group (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The group ID</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving. If the membership already exists, it won't be created for a 2nd time.
            ''' </remarks>
            Public Sub AddMembership(ByVal groupID As Integer, isDenyRule As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)

                If _ID = SpecialUsers.User_Anonymous OrElse _ID = SpecialUsers.User_Public OrElse _ID = SpecialUsers.User_Code OrElse _ID = SpecialUsers.User_UpdateProcessor Then
                    Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf _ID = Nothing Then
                    Dim Message As String = "User has to be created, first, before you can modify the list of memberships"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf isDenyRule = False Then
                    Dim RequiredApplicationFlags As String() = GroupInformation.RequiredAdditionalFlags(groupID, Me._WebManager)
                    Dim RequiredFlagsValidationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult() = CompuMaster.camm.WebManager.FlagValidation.ValidateRequiredFlags(Me, RequiredApplicationFlags, True)
                    If RequiredFlagsValidationResults.Length <> 0 Then
                        Throw New CompuMaster.camm.WebManager.FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                    End If
                End If

                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("AdminPrivate_CreateMemberships", MyConn) 'This stored procedure is intelligent and doesn't add a duplicate entry
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
                MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _ID
                If Setup.DatabaseUtils.Version(WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
                ElseIf isDenyRule Then
                    Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
                End If
                Dim Result As Object = Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                If IsDBNull(Result) OrElse Result Is Nothing Then
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf CType(Result, Integer) = -1 Then
                    'Success
                Else
                    Dim Message As String = String.Format("Membership creation failed ({0})", CType(Result, Integer))
                    _WebManager.Log.RuntimeException(Message & " UID=" & _ID & " GID=" & groupID & " AID=" & _WebManager.CurrentUserID(SpecialUsers.User_Code))
                End If

                If _System_InitOfAuthorizationsDone = False Then
                    'send e-mail when first membership has been set up
                    _System_InitOfAuthorizationsDone = True 'save this value locally in this class instance
                    'Check wether InitAuthorizationsDone has been set
                    If DataLayer.Current.SetUserDetail(_WebManager, Nothing, _ID, "InitAuthorizationsDone", "1", True) Then
                        Try
                            If notifications Is Nothing Then
                                _WebManager.Notifications.NotificationForUser_AuthorizationsSet(Me)
                            Else
                                notifications.NotificationForUser_AuthorizationsSet(Me)
                            End If
                        Catch
                        End Try
                    End If
                End If

                'Requery the list of memberships next time it's required
                _MembershipsByRule = Nothing

            End Sub

            ''' <summary>
            '''     Add a membership to a user group (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupInfo">The group</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving. If the membership already exists, it won't be created for a 2nd time.
            ''' </remarks>
            Public Sub AddMembership(ByVal groupInfo As GroupInformation, isDenyRule As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddMembership(groupInfo.ID, isDenyRule, notifications)
                groupInfo.ResetMembershipsCache()
            End Sub


            ''' <summary>
            ''' Reset cached/calculated authorizations
            ''' </summary>
            Friend Sub ResetMembershipsCache()
                _MembershipsByRule = Nothing
            End Sub

            ''' <summary>
            '''     Remove a membership (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="GroupID">The group ID the user shall not be member of any more</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub RemoveMembership(ByVal GroupID As Integer)
                RemoveMembership(GroupID, False)
            End Sub
            ''' <summary>
            '''     Remove a membership (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="GroupID">The group ID the user shall not be member of any more</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            Public Sub RemoveMembership(ByVal groupID As Integer, isDenyRule As Boolean)
                If _ID = SpecialUsers.User_Anonymous OrElse _ID = SpecialUsers.User_Public OrElse _ID = SpecialUsers.User_Code OrElse _ID = SpecialUsers.User_UpdateProcessor Then
                    Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf _ID = Nothing Then
                    Dim Message As String = "User has to be created, first, before you can modify the list of memberships"
                    _WebManager.Log.RuntimeException(Message)
                End If
                'Save to DB
                Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("", MyConn)
                MyCmd.CommandType = CommandType.Text
                If Setup.DatabaseUtils.Version(WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    MyCmd.CommandText = "DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID AND IsDenyRule = @IsDenyRule"
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _ID
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
                    MyCmd.Parameters.Add("@IsDenyRule", SqlDbType.Bit).Value = isDenyRule
                Else
                    If isDenyRule Then Throw New NotSupportedException("Current DB build doesn't support feature DenyRule")
                    MyCmd.CommandText = "DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID"
                    MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _ID
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
                End If
                Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Requery the list of memberships next time it's required
                _MembershipsByRule = Nothing
            End Sub
            ''' <summary>
            '''     Remove a membership (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="GroupInfo">The group the user shall not be member of any more</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            Public Sub RemoveMembership(ByVal groupInfo As GroupInformation, isDenyRule As Boolean)
                RemoveMembership(groupInfo.ID, isDenyRule)
                groupInfo.ResetMembershipsCache()
            End Sub

            ''' <summary>
            ''' Is the current user a member of the given group?
            ''' </summary>
            ''' <param name="groupName">The name of the group which shall be checked</param>
            ''' <returns>True if the user is a member, otherwise False</returns>
            Public Function IsMember(ByVal groupName As String) As Boolean
                For MyCounter As Integer = 0 To Me.MembershipsByRule().Effective.Length - 1
                    If LCase(Me.MembershipsByRule().Effective(MyCounter).Name) = LCase(groupName) Then
                        Return True
                    End If
                Next
                Return False
            End Function
            ''' <summary>
            ''' Is the current user a member of the given group?
            ''' </summary>
            ''' <param name="groupID">The ID of the group which shall be checked</param>
            ''' <returns>True if the user is a member, otherwise False</returns>
            Public Function IsMember(ByVal groupID As Integer) As Boolean
                For MyCounter As Integer = 0 To Me.MembershipsByRule().Effective.Length - 1
                    If Me.MembershipsByRule().Effective(MyCounter).ID = groupID Then
                        Return True
                    End If
                Next
                Return False
            End Function
            ''' <summary>
            '''     Additional, optional flags
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     <para>Additional flags are typically used by applications which have to store some data in the user's profile.</para>
            '''     <para>Following names are reserved</para>
            ''' <list>
            '''     <item><code>1stPreferredLanguage</code></item>
            '''     <item><code>2ndPreferredLanguage</code></item>
            '''     <item><code>3rdPreferredLanguage</code></item>
            '''     <item><code>AccountProfileValidatedByEMailTest</code></item>
            '''     <item><code>Addresses</code></item>
            '''     <item><code>AutomaticLogonAllowedByMachineToMachineCommunication</code></item>
            '''     <item><code>ComesFrom</code></item>
            '''     <item><code>Company</code></item>
            '''     <item><code>CompleteName</code></item>
            '''     <item><code>Country</code></item>
            '''     <item><code>email</code></item>
            '''     <item><code>ExternalAccount</code></item>
            '''     <item><code>Fax</code></item>
            '''     <item><code>FirstName</code></item>
            '''     <item><code>InitAuthorizationsDone</code></item>
            '''     <item><code>LastName</code></item>
            '''     <item><code>Location</code></item>
            '''     <item><code>Mobile</code></item>
            '''     <item><code>Motivation</code></item>
            '''     <item><code>NameAddition</code></item>
            '''     <item><code>Phone</code></item>
            '''     <item><code>Position</code></item>
            '''     <item><code>Sex</code></item>
            '''     <item><code>State</code></item>
            '''     <item><code>Street</code></item>
            '''     <item><code>ZipCode</code></item>
            ''' </list>
            ''' </remarks>
            Public Property AdditionalFlags() As Collections.Specialized.NameValueCollection Implements IUserInformation.AdditionalFlags
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AdditionalFlags
                End Get
                <Obsolete("You can't replace the additional flags collection, but you can add, update or remove its values", True)> Set(ByVal Value As Collections.Specialized.NameValueCollection)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AdditionalFlags = Value
                End Set
            End Property

            ''' <summary>
            '''     The access level role of the user
            ''' </summary>
            ''' <value></value>
            Public Property AccessLevel() As AccessLevelInformation
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    If _AccessLevel Is Nothing Then
                        If _AccessLevelID = Integer.MinValue Then
                            _AccessLevelID = Me._WebManager.CurrentServerInfo.ParentServerGroup.AccessLevelDefault.ID
                        End If
                        _AccessLevel = New AccessLevelInformation(_AccessLevelID, _WebManager)
                    End If
                    Return _AccessLevel
                End Get
                Set(ByVal Value As AccessLevelInformation)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AccessLevel = Value
                    _AccessLevelID = Value.ID
                End Set
            End Property

            ''' <summary>
            '''     Indicates if the e-mail address has already been validated
            ''' </summary>
            ''' <value></value>
            Public Property AccountProfileValidatedByEMailTest() As Boolean
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _AccountProfileValidatedByEMailTest
                End Get
                Set(ByVal Value As Boolean)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _AccountProfileValidatedByEMailTest = Value
                End Set
            End Property

            ''' <summary>
            '''     The list of authorizations for standard access by the current user (AllowDevelopment - DenyDevelopment + AllowStandard - DenyStandard)
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use AuthorizationsByRule instead")> Public ReadOnly Property Authorizations() As SecurityObjectAuthorizationForUser()
                Get
                    Return AuthorizationsByRule.EffectiveByDenyRuleStandard()
                End Get
            End Property

            Private _AuthorizationsByRule As Security.UserAuthorizationItemsByRuleForUsers
            ''' <summary>
            ''' Authorizations of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property AuthorizationsByRule As Security.UserAuthorizationItemsByRuleForUsers
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    If _AuthorizationsByRule Is Nothing OrElse _AuthorizationsByRule.CurrentContextServerGroupIDInitialized <> (_WebManager.CurrentServerInfo IsNot Nothing) Then 'no cache object available OR srv. group initialization context changed
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("", MyConn)
                        MyCmd.CommandType = CommandType.Text
                        If Setup.DatabaseUtils.Version(WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                            MyCmd.CommandText = "Select applicationsrightsbyuser.ID As AuthorizationID, applicationsrightsbyuser.ID_Application As AuthorizationSecurityObjectID, applicationsrightsbyuser.ID_GroupOrPerson As AuthorizationGroupID, applicationsrightsbyuser.ID_ServerGroup As AuthorizationServerGroupID, applicationsrightsbyuser.ReleasedOn As AuthorizationReleasedOn, applicationsrightsbyuser.ReleasedBy As AuthorizationReleasedBy, applicationsrightsbyuser.DevelopmentTeamMember As AuthorizationIsDeveloper, applicationsrightsbyuser.IsDenyRule, Applications_CurrentAndInactiveOnes.* From applicationsrightsbyuser inner Join Applications_CurrentAndInactiveOnes On applicationsrightsbyuser.id_application = Applications_CurrentAndInactiveOnes.id Where applicationsrightsbyuser.id_grouporperson = @ID And Applications_CurrentAndInactiveOnes.id Is Not null"
                        Else
                            MyCmd.CommandText = "Select applicationsrightsbyuser.ID As AuthorizationID, applicationsrightsbyuser.ID_Application As AuthorizationSecurityObjectID, applicationsrightsbyuser.ID_GroupOrPerson As AuthorizationGroupID, NULL As AuthorizationServerGroupID, applicationsrightsbyuser.ReleasedOn As AuthorizationReleasedOn, applicationsrightsbyuser.ReleasedBy As AuthorizationReleasedBy, applicationsrightsbyuser.DevelopmentTeamMember As AuthorizationIsDeveloper, CAST(0 As bit) As IsDenyRule, Applications_CurrentAndInactiveOnes.* From applicationsrightsbyuser inner Join Applications_CurrentAndInactiveOnes On applicationsrightsbyuser.id_application = Applications_CurrentAndInactiveOnes.id Where applicationsrightsbyuser.id_grouporperson = @ID And Applications_CurrentAndInactiveOnes.id Is Not null"
                        End If
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim SecObjects As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "SecurityObjects")
                        Dim AllowRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForUser)
                        Dim AllowRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForUser)
                        Dim DenyRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForUser)
                        Dim DenyRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForUser)
                        For MyCounter As Integer = 0 To SecObjects.Rows.Count - 1
                            Dim MyDataRow As DataRow = SecObjects.Rows(MyCounter)
                            Dim NavInfo As New Security.NavigationInformation( _
                                        0, _
                                        Nothing, _
                                        Utils.Nz(MyDataRow("Level1Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level2Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level3Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level4Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level5Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level6Title"), String.Empty), _
                                        Utils.Nz(MyDataRow("Level1TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level2TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level3TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level4TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level5TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("Level6TitleIsHtmlCoded"), False), _
                                        Utils.Nz(MyDataRow("NavURL"), String.Empty), _
                                        Utils.Nz(MyDataRow("NavFrame"), String.Empty), _
                                        Utils.Nz(MyDataRow("NavTooltipText"), String.Empty), _
                                        Utils.Nz(MyDataRow("AddLanguageID2URL"), False), _
                                        Utils.Nz(MyDataRow("LanguageID"), 0), _
                                        Utils.Nz(MyDataRow("LocationID"), 0), _
                                        Utils.Nz(MyDataRow("Sort"), 0), _
                                        Utils.Nz(MyDataRow("IsNew"), False), _
                                        Utils.Nz(MyDataRow("IsUpdated"), False), _
                                        Utils.Nz(MyDataRow("ResetIsNewUpdatedStatusOn"), DateTime.MinValue), _
                                        Utils.Nz(MyDataRow("OnMouseOver"), String.Empty), _
                                        Utils.Nz(MyDataRow("OnMouseOut"), String.Empty), _
                                        Utils.Nz(MyDataRow("OnClick"), String.Empty))
                            Dim secObjInfo As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(CType(MyDataRow("ID"), Integer), CType(MyDataRow("Title"), String), Utils.Nz(MyDataRow("TitleAdminArea"), CType(Nothing, String)), Utils.Nz(MyDataRow("Remarks"), CType(Nothing, String)), CType(MyDataRow("ModifiedBy"), Long), Utils.Nz(MyDataRow("ModifiedOn"), CType(Nothing, Date)), CType(MyDataRow("ReleasedBy"), Long), Utils.Nz(MyDataRow("ReleasedOn"), CType(Nothing, Date)), Utils.Nz(MyDataRow("AppDisabled"), False), Utils.Nz(MyDataRow("AppDeleted"), False), Utils.Nz(MyDataRow("AuthsAsAppID"), 0), Utils.Nz(MyDataRow("SystemAppType"), 0), Utils.Nz(Utils.CellValueIfColumnExists(MyDataRow, "RequiredUserProfileFlags"), ""), Utils.Nz(Utils.CellValueIfColumnExists(MyDataRow, "RequiredUserProfileFlagsRemarks"), ""), NavInfo, _WebManager)
                            Dim secObjAuth As New SecurityObjectAuthorizationForUser(_WebManager, CType(MyDataRow("AuthorizationID"), Integer), CType(MyDataRow("AuthorizationGroupID"), Integer), CType(MyDataRow("AuthorizationSecurityObjectID"), Integer), Utils.Nz(MyDataRow("AuthorizationServerGroupID"), 0), Me, secObjInfo, Nothing, Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False), Utils.Nz(MyDataRow("IsDenyRule"), False), CType(MyDataRow("AuthorizationReleasedOn"), DateTime), CType(MyDataRow("AuthorizationReleasedBy"), Integer), False)
                            If Utils.Nz(MyDataRow("IsDenyRule"), False) = False Then
                                If Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False) = True Then
                                    AllowRuleAuthsIsDev.Add(secObjAuth)
                                Else
                                    AllowRuleAuthsNonDev.Add(secObjAuth)
                                End If
                            Else
                                If Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False) = True Then
                                    DenyRuleAuthsIsDev.Add(secObjAuth)
                                Else
                                    DenyRuleAuthsNonDev.Add(secObjAuth)
                                End If
                            End If
                        Next
                        If _WebManager.CurrentServerInfo Is Nothing Then
                            _AuthorizationsByRule = New Security.UserAuthorizationItemsByRuleForUsers( _
                                Me._ID, _
                                0, _
                                AllowRuleAuthsNonDev.ToArray(), _
                                AllowRuleAuthsIsDev.ToArray(), _
                                DenyRuleAuthsNonDev.ToArray(), _
                                DenyRuleAuthsIsDev.ToArray(), _
                                Me._WebManager)
                        Else
                            _AuthorizationsByRule = New Security.UserAuthorizationItemsByRuleForUsers( _
                                _WebManager.CurrentServerInfo.ParentServerGroupID, _
                                Me._ID, _
                                0, _
                                AllowRuleAuthsNonDev.ToArray(), _
                                AllowRuleAuthsIsDev.ToArray(), _
                                DenyRuleAuthsNonDev.ToArray(), _
                                DenyRuleAuthsIsDev.ToArray(), _
                                Me._WebManager)
                        End If
                    End If
                    Return _AuthorizationsByRule
                End Get
            End Property

            ''' <summary>
            '''     Add an authorization to a security object for all server groups (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Use overloaded method with parameter serverGroupID"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub AddAuthorization(ByVal securityObjectID As Integer, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorization(securityObjectID, 0, notifications)
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDev/isDenyRule parameter")> Public Sub AddAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorization(securityObjectID, serverGroupID, False, notifications)
            End Sub
            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub AddAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorization(securityObjectID, serverGroupID, developerAuthorization, False, notifications)
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="isDenyRule">True for a deny rule or False for a grant access rule</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                If isDenyRule = False Then
                    Dim RequiredApplicationFlags As String() = SecurityObjectInformation.RequiredAdditionalFlags(securityObjectID, Me._WebManager)
                    Dim RequiredFlagsValidationResults As FlagValidation.FlagValidationResult() = FlagValidation.ValidateRequiredFlags(Me, RequiredApplicationFlags, True)
                    If RequiredFlagsValidationResults.Length <> 0 Then
                        Throw New FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                    End If
                End If
                Try
                    DataLayer.Current.AddUserAuthorization(_WebManager, Nothing, securityObjectID, serverGroupID, Me, Me.IDLong, developerAuthorization, isDenyRule, _WebManager.CurrentUserID(SpecialUsers.User_Anonymous), notifications)
                Catch ex As Exception
                    _WebManager.Log.RuntimeException(ex, False, False)
                End Try
                'Requery the list of authorization next time it's required
                _AuthorizationsByRule = Nothing
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectInfo">The security object</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="isDenyRule">True for a deny rule or False for a grant access rule</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorization(ByVal securityObjectInfo As SecurityObjectInformation, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorization(securityObjectInfo.ID, serverGroupID, developerAuthorization, isDenyRule, notifications)
                securityObjectInfo.ResetAuthorizationsCacheForUsers()
            End Sub

            ''' <summary>
            '''     Add an authorization to a security object for all server groups (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Use overloaded method with parameter serverGroupID"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub AddAuthorization(ByVal securityObjectID As Integer, ByVal developerAuthorization As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorization(securityObjectID, 0, developerAuthorization, notifications)
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID the user shall not be authorized for any more</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDev/isDenyRule parameter")> Public Sub RemoveAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer)
                RemoveAuthorization(securityObjectID, serverGroupID, False, False)
            End Sub
            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID the user shall not be authorized for any more</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorization(ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, isDevRule As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveUserAuthorization(Me._WebManager, securityObjectID, Me._ID, serverGroupID, isDevRule, isDenyRule)
                _AuthorizationsByRule = Nothing
            End Sub
            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObject">The security object the user shall not be authorized for any more</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorization(ByVal securityObject As WMSystem.SecurityObjectInformation, ByVal serverGroupID As Integer, isDevRule As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveUserAuthorization(Me._WebManager, securityObject.ID, Me._ID, serverGroupID, isDevRule, isDenyRule)
                securityObject.ResetAuthorizationsCacheForUsers()
                _AuthorizationsByRule = Nothing
            End Sub

            ''' <summary>
            ''' Reset cached/calculated authorizations
            ''' </summary>
            Friend Sub ResetAuthorizationsCache()
                _AuthorizationsByRule = Nothing
            End Sub

            ''' <summary>
            '''     Remove an authorization which is assigned to all server groups (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="securityObjectID">The security object ID the user shall not be authorized for any more</param>
            ''' <remarks>
            ''' This action will be done immediately without the need for saving
            ''' </remarks>
            <Obsolete("Use overloaded method with parameter serverGroupID"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub RemoveAuthorization(ByVal securityObjectID As Integer)
                Me.RemoveAuthorization(securityObjectID, 0)
            End Sub

            ''' <summary>
            '''     An external account relation
            ''' </summary>
            ''' <value></value>
            Public Property ExternalAccount() As String Implements IUserInformation.ExternalAccount
                Get
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    Return _ExternalAccount
                End Get
                Set(ByVal Value As String)
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    _ExternalAccount = Value
                End Set
            End Property

        End Class

    End Class

End Namespace