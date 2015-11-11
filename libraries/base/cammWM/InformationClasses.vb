Option Explicit On
Option Strict On

Imports System.Data.SqlClient
Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager

#Region "Interfaces"
    Public Interface IUserInformation

        'System reference
        Property WebManager() As IWebManager

        Enum GenderType
            Undefined = Sex.Undefined
            Masculin = Sex.Masculin
            Feminin = Sex.Feminin
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

#End Region

    Public Class UserInfoConflictingUniqueKeysException
        Inherits Exception

        Friend Sub New(uniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues())
            MyBase.New()
            Me._UniqueKeyConflicts = uniqueKeyConflicts
        End Sub

        Public Overrides ReadOnly Property Message As String
            Get
                Return CreateMessage(UniqueKeyConflicts)
            End Get
        End Property

        Private Shared Function CreateMessage(uniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues()) As String
            If uniqueKeyConflicts Is Nothing OrElse uniqueKeyConflicts.Length = 0 Then Throw New ArgumentNullException("uniqueKeyConflicts")
            Dim NewMessage As String = "Unique keys conflicts found for:"
            For MyCounter As Integer = 0 To uniqueKeyConflicts.Length - 1
                NewMessage &= vbNewLine & uniqueKeyConflicts(MyCounter).ToString
            Next
            Return NewMessage
        End Function

        Private _UniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues()
        Public ReadOnly Property UniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues()
            Get
                Return _UniqueKeyConflicts
            End Get
        End Property

    End Class

    Public Class UserInfoConflictingUniqueKeysKeyValues
        Friend Sub New(key As String, conflictingUserIDs As Long(), conflictingValue As String)
            Me._Key = key
            Me._ConflictingValue = conflictingValue
            Me._UserIDs = conflictingUserIDs
        End Sub
        Private _Key As String
        Private _UserIDs As Long()
        Private _ConflictingValue As String
        Public ReadOnly Property Key As String
            Get
                Return _Key
            End Get
        End Property
        Public ReadOnly Property ConflictingUserIDs As Long()
            Get
                Return _UserIDs
            End Get
        End Property
        Public ReadOnly Property ConflictingValue As String
            Get
                Return _ConflictingValue
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return "Key: """ & Me.Key & """, "" value: """ & Me.ConflictingValue & """, user IDs: " & Utils._JoinArrayToString(Me.ConflictingUserIDs, ", ")
        End Function
    End Class


#Region "in-memory-mirror of WebManager objects"

    Friend Class InformationClassTools

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Are there no invalid entries because of the content in the unique fields?
        ''' </summary>
        ''' <param name="userInfo">A user information object</param>
        ''' <returns>True if no conflicts, otherwise false</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	07.09.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function IsValidContentOfUniqueFields(ByVal userInfo As CompuMaster.camm.WebManager.IUserInformation) As Boolean

            Dim MyConn As New SqlConnection(userInfo.WebManager.ConnectionString)
            Dim Result As Boolean = True

            Try
                Dim ReturnValue As Integer

                'LoginName
                Dim CmdLoginName As New SqlCommand("SELECT COUNT(*) FROM dbo.Benutzer WHERE LoginName = @LoginName AND NOT ID = @ID", MyConn)
                CmdLoginName.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                CmdLoginName.Parameters.Add("@LoginName", SqlDbType.NVarChar).Value = userInfo.LoginName
                ReturnValue = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(CmdLoginName, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection), 0)
                CmdLoginName.Dispose()
                If ReturnValue <> 0 Then
                    Result = False
                    Exit Try
                End If

                'External account
                If userInfo.ExternalAccount <> Nothing Then
                    Dim CmdExternalAccount As New SqlCommand("SELECT COUNT(*) FROM dbo.Log_Users inner join dbo.Benutzer on dbo.Benutzer.ID = dbo.Log_Users.ID_User WHERE Type = 'ExternalAccount' AND Value = @Value AND NOT ID_User = @ID", MyConn)
                    CmdExternalAccount.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                    CmdExternalAccount.Parameters.Add("@Value", SqlDbType.NVarChar).Value = userInfo.ExternalAccount
                    ReturnValue = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(CmdExternalAccount, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection), 0)
                    CmdExternalAccount.Dispose()
                    If ReturnValue <> 0 Then
                        Result = False
                        Exit Try
                    End If
                End If

            Finally
                'Release resources
                If Not MyConn Is Nothing Then
                    If Not MyConn.State = ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            Return Result

        End Function

        ''' <summary>
        ''' Which users are already present using the values of unique fields of the given userInfo 
        ''' </summary>
        ''' <param name="userInfo">A user information object with values in unique fields which shall be evaluated</param>
        ''' <returns>A list of existing user IDs conflicting with the given values as unique keys</returns>
        ''' <remarks></remarks>
        Public Shared Function ExistingUsersConflictingWithContentOfUniqueFields(ByVal userInfo As CompuMaster.camm.WebManager.IUserInformation) As UserInfoConflictingUniqueKeysKeyValues()

            Dim MyConn As New SqlConnection(userInfo.WebManager.ConnectionString)
            Dim Result As New ArrayList

            Try
                Dim ReturnValues As ArrayList

                'LoginName
                Dim CmdLoginName As New SqlCommand("SELECT ID FROM dbo.Benutzer WHERE LoginName = @LoginName AND NOT ID = @ID", MyConn)
                CmdLoginName.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                CmdLoginName.Parameters.Add("@LoginName", SqlDbType.NVarChar).Value = userInfo.LoginName
                ReturnValues = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(CmdLoginName, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                CmdLoginName.Dispose()
                If ReturnValues.Count <> 0 Then
                    Result.Add(New UserInfoConflictingUniqueKeysKeyValues("LoginName", CType(ReturnValues.ToArray(GetType(Long)), Long()), userInfo.LoginName))
                End If

                'External account
                If userInfo.ExternalAccount <> Nothing Then
                    Dim CmdExternalAccount As New SqlCommand("SELECT dbo.Benutzer.ID FROM dbo.Log_Users inner join dbo.Benutzer on dbo.Benutzer.ID = dbo.Log_Users.ID_User WHERE Type = 'ExternalAccount' AND Value = @Value AND NOT ID_User = @ID", MyConn)
                    CmdExternalAccount.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                    CmdExternalAccount.Parameters.Add("@Value", SqlDbType.NVarChar).Value = userInfo.ExternalAccount
                    ReturnValues = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(CmdExternalAccount, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                    CmdExternalAccount.Dispose()
                    If ReturnValues.Count <> 0 Then
                        Result.Add(New UserInfoConflictingUniqueKeysKeyValues("ExternalAccount", CType(ReturnValues.ToArray(GetType(Long)), Long()), userInfo.ExternalAccount))
                    End If
                End If

            Finally
                'Release resources
                If Not MyConn Is Nothing Then
                    If Not MyConn.State = ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            Return CType(Result.ToArray(GetType(UserInfoConflictingUniqueKeysKeyValues)), UserInfoConflictingUniqueKeysKeyValues())

        End Function

        'Private Shared Sub CombineArrayListWithoutDbNullsOrNulls(baseList As ArrayList, addList As ArrayList)
        '    If baseList Is Nothing Then Throw New ArgumentNullException("baseList")
        '    If addList Is Nothing Then Return 'just do nothing
        '    For Each item As Object In addList
        '        If item Is Nothing Then
        '            'ignore
        '        ElseIf IsDBNull(item) Then
        '            'ignore
        '        Else
        '            If baseList.Contains(item) = False Then baseList.Add(item)
        '        End If
        '    Next
        'End Sub
    End Class


#If Implemented Then

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.UserInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User information
    ''' </summary>
    ''' <remarks>
    '''     This class contains all information of a user profile as well as all important methods for handling of that account
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserInformation
        implements CompuMaster.camm.WebManager.IUserInformation
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
        Private _Memberships As CompuMaster.camm.WebManager.WMSystem.GroupInformation()
        Private _AdditionalFlags As New Collections.Specialized.NameValueCollection
        Private _AccessLevel As AccessLevelInformation
        Private _AccessLevelID As Integer
        Private _System_InitOfAuthorizationsDone As Boolean
        Private _AccountProfileValidatedByEMailTest As Boolean
        Private _AutomaticLogonAllowedByMachineToMachineCommunication As Boolean
        Private _ExternalAccount As String

        ''' -----------------------------------------------------------------------------
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
        ''' <param name="AccessLevelID">The access level ID of the user</param>
        ''' <param name="WebManager">The current instance of camm Web-Manager</param>
        ''' <param name="ExternalAccount">An external account relation for single-sign-on purposes</param>
        ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	07.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete()> Public Sub New(ByVal UserID As Integer, ByVal LoginName As String, ByVal EMailAddress As String, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, ByVal ExternalAccount As String, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
            If UserID = SpecialUsers.User_Anonymous Or UserID = SpecialUsers.User_Code Or UserID = SpecialUsers.User_Public Or UserID = SpecialUsers.User_UpdateProcessor Then
                Throw New InvalidOperationException("Can't assign user details to this special system user")
            End If
            If Len(LoginName) > 20 Then
                Throw New NotSupportedException("Login names can't be larger than 20 characters")
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
                'Else 'Data for a new user
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
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
        ''' <param name="AccessLevelID">The access level ID of the user</param>
        ''' <param name="__reserved">Obsolete parameter</param>
        ''' <param name="WebManager">The current instance of camm Web-Manager</param>
        ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	07.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("use overloaded method instead")> Public Sub New(ByVal UserID As Integer, ByVal LoginName As String, ByVal EMailAddress As String, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByVal __reserved As Boolean, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
            Me.New(CLng(UserID), LoginName, EMailAddress, False, Company, Sex, NameAddition, FirstName, LastName, AcademicTitle, Street, ZipCode, City, State, Country, PreferredLanguage1ID, PreferredLanguage2ID, PreferredLanguage3ID, LoginDisabled, LoginLockedTemporary, LoginDeleted, AccessLevelID, WebManager, Nothing, AdditionalFlags)
        End Sub
        ''' -----------------------------------------------------------------------------
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
        ''' <history>
        ''' 	[adminwezel]	07.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete()> Public Sub New(ByVal UserID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
            _ID = CLng(UserID)
            _WebManager = WebManager
            ReadCompleteUserInformation(SearchForDeletedAccountsAsWell)
        End Sub
        ''' -----------------------------------------------------------------------------
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
        ''' <param name="AccessLevelID">The access level ID of the user</param>
        ''' <param name="WebManager">The current instance of camm Web-Manager</param>
        ''' <param name="ExternalAccount">An external account relation for single-sign-on purposes</param>
        ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	07.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal UserID As Long, ByVal LoginName As String, ByVal EMailAddress As String, ByVal AutomaticLogonAllowedByMachineToMachineCommunication As Boolean, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, ByVal ExternalAccount As String, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
            If UserID = SpecialUsers.User_Anonymous Or UserID = SpecialUsers.User_Code Or UserID = SpecialUsers.User_Public Or UserID = SpecialUsers.User_UpdateProcessor Then
                Throw New InvalidOperationException("Can't assign user details to this special system user")
            End If
            If Len(LoginName) > 20 Then
                Throw New NotSupportedException("Login names can't be larger than 20 characters")
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
                'Else 'Data for a new user
            End If
        End Sub
        ''' -----------------------------------------------------------------------------
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
        ''' <history>
        ''' 	[adminwezel]	07.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal UserID As Long, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
            _ID = UserID
            _WebManager = WebManager
            ReadCompleteUserInformation(SearchForDeletedAccountsAsWell)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is this user a system user (anonymous, public, etc.)
        ''' </summary>
        ''' <value></value>
        ''' <seealso cref="CompuMaster.camm.WebManager.WMSystem.SpecialUsers" />
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IsSystemUser() As Boolean
            Get
                Return CompuMaster.camm.WebManager.WMSystem.IsSystemUser(_ID)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is an automated logon procedure allowed for this account
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AutomaticLogonAllowedByMachineToMachineCommunication() As Boolean
            Get
                Return False
                Throw New NotImplementedException("Not yet implemented")
                'TODOs:
                '1. Add column to data table of users
                '1a. Update internal methods to use the new constructor and fill this information
                '2. Solve the problem that only 1 browser session can be logged in on the same time. Keep an eye on the GetLogonNameByBrowserSessionID (or similar method/SP name)
                '3. Make this property configurable in the administrator's menus
                Return _AutomaticLogonAllowedByMachineToMachineCommunication
            End Get
            Set(ByVal Value As Boolean)
                If Value = True Then
                    Throw New NotImplementedException("AutomaticLogonAllowedByMachineToMachineCommunication")
                Else
                    _AutomaticLogonAllowedByMachineToMachineCommunication = Value
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Read all the account data from database
        ''' </summary>
        ''' <param name="SearchForDeletedAccountsAsWell">Also load data of users who have been deleted in the past</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
            Try
                MyConn.Open()
                Dim MyReader As SqlDataReader
                Try
                    MyReader = MyCmd.ExecuteReader()
                    If MyReader.Read Then
                        _ID = MyReader("ID")
                        MyClass.Company = _WebManager.System_Nz(MyReader("Company"))
                        _LoginName = MyReader("LoginName")
                        _EMailAddress = MyReader("E-Mail")
                        MyClass.FirstName = MyReader("Vorname")
                        MyClass.LastName = MyReader("Nachname")
                        MyClass.AcademicTitle = _WebManager.System_Nz(MyReader("Titel"))
                        MyClass.Street = _WebManager.System_Nz(MyReader("Strasse"))
                        MyClass.ZipCode = _WebManager.System_Nz(MyReader("PLZ"))
                        MyClass.Location = _WebManager.System_Nz(MyReader("Ort"))
                        MyClass.State = _WebManager.System_Nz(MyReader("State"))
                        MyClass.Country = _WebManager.System_Nz(MyReader("Land"))
                        _PreferredLanguage1ID = _WebManager.System_Nz(MyReader("1stPreferredLanguage"), 1)
                        _PreferredLanguage2ID = _WebManager.System_Nz(MyReader("2ndPreferredLanguage"), Nothing)
                        _PreferredLanguage3ID = _WebManager.System_Nz(MyReader("3rdPreferredLanguage"), Nothing)
                        MyClass.NameAddition = _WebManager.System_Nz(MyReader("Namenszusatz"))
                        if utils.nz(_WebManager.System_Nz(MyReader("Anrede")),"") = "" then
                            if myclass.firstname = nothing orelse myclass.lastname = nothing then
                                _sex = gender.MissingNameOrGroupOfPersons
                            else
                                _sex = gender.undefined
                            end if
                        else
                            _Sex = IIf(Convert.ToString(_WebManager.System_Nz(MyReader("Anrede"))) = "Mr.", Gender.Masculin, Gender.Feminin)
                        endif
                        _LoginDisabled = MyReader("LoginDisabled")
                        _LoginLockedTemporary = Not IsDBNull(MyReader("LoginLockedTill"))
                        _LoginLockedTemporaryTill = _WebManager.System_Nz(MyReader("LoginLockedTill"))
                        _LoginDeleted = False
                        _AccessLevelID = MyReader("AccountAccessability")
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
                        If Not IsDBNull(MyReader("Type")) Then
                            Select Case Convert.ToString(MyReader("Type")).ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                Case "CompleteName".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                Case "CompleteNameInclAddresses".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                Case "email".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    _EMailAddress = _WebManager.System_Nz(MyReader("Value"))
                                Case "Sex".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    select case Convert.ToString(_WebManager.System_Nz(MyReader("Value"))).ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    case "m"
                                        _Sex = _Sex.Masculin
                                    case "u"
                                        _Sex = _Sex.Undefined
                                    case "w"
                                        _sex = _sex.feminin
                                    case else
                                        _Sex = _Sex.MissingNameOrGroupOfPersons
                                    end select
                                Case "Addresses".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                Case "1stPreferredLanguage".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Try
                                        _PreferredLanguage1ID = MyReader("Value")
                                    Catch
                                        _PreferredLanguage1ID = Nothing
                                    End Try
                                Case "2ndPreferredLanguage".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Try
                                        _PreferredLanguage2ID = MyReader("Value")
                                    Catch
                                        _PreferredLanguage2ID = Nothing
                                    End Try
                                Case "3rdPreferredLanguage".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    Try
                                        _PreferredLanguage3ID = MyReader("Value")
                                    Catch
                                        _PreferredLanguage3ID = Nothing
                                    End Try
                                Case "Company".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.Company = _WebManager.System_Nz(MyReader("Value"))
                                Case "FirstName".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    _FirstName = _WebManager.System_Nz(MyReader("Value"))
                                Case "LastName".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    _LastName = _WebManager.System_Nz(MyReader("Value"))
                                Case "NameAddition".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.NameAddition = _WebManager.System_Nz(MyReader("Value"))
                                Case "Street".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.Street = _WebManager.System_Nz(MyReader("Value"))
                                Case "ZIPCode".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.ZipCode = _WebManager.System_Nz(MyReader("Value"))
                                Case "Location".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.Location = _WebManager.System_Nz(MyReader("Value"))
                                Case "State".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.State = _WebManager.System_Nz(MyReader("Value"))
                                Case "Country".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    MyClass.Country = _WebManager.System_Nz(MyReader("Value"))
                                Case "AccountProfileValidatedByEMailTest"
                                    _AccountProfileValidatedByEMailTest = _WebManager.System_Nz(MyReader("Value"))
                                Case "InitAuthorizationsDone".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    _System_InitOfAuthorizationsDone = IIf(Convert.ToString(_WebManager.System_Nz(MyReader("Value"))) = "1", True, False)
                                Case "ExternalAccount".ToLower(System.Globalization.CultureInfo.InvariantCulture)
                                    _ExternalAccount = _WebManager.System_Nz(MyReader("Value"))
                                Case "AutomaticLogonAllowedByMachineToMachineCommunication"
                                    'ToDo: activation for reading here, but also for writing back
                                    '_AutomaticLogonAllowedByMachineToMachineCommunication = _WebManager.System_Nz(MyReader("Value"))
                                    'Me.AutomaticLogonAllowedByMachineToMachineCommunication()
                                Case Else
                                    MyClass.AdditionalFlags.Add(MyReader("Type"), _WebManager.System_Nz(MyReader("Value")))
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
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
            If AccountNotExists AndAlso SearchForDeletedAccountsAsWell = False AndAlso Me.LoginDeleted = False Then
                Dim Message As String = "User account hasn't existed"
                _WebManager.Log.RuntimeException(Message)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The account ID
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property IDLong() As Long
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The account ID
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Set the user ID for a new registered user
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Sub SetNewUserID(ByVal ID As Long)
            _ID = ID
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The login name of the user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LoginName() As String
            Get
                Return _LoginName
            End Get
            Set(ByVal Value As String)
                If Len(Value) > 20 Then
                    Throw New NotSupportedException("Login names can't be larger than 20 characters")
                End If
                _LoginName = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Indicate wether the user has already got an e-mail notification that he has got his first priviledges and/or memberships assigned
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AccountAuthorizationsAlreadySet() As Boolean
            Get
                Return _System_InitOfAuthorizationsDone
            End Get
            Set(ByVal Value As Boolean)
                _System_InitOfAuthorizationsDone = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The required e-mail address where all important messages will be sent to
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property EMailAddress() As String
            Get
                Return _EMailAddress
            End Get
            Set(ByVal Value As String)
                _EMailAddress = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The fax number
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FaxNumber() As String
            Get
                Return AdditionalFlags("Fax")
            End Get
            Set(ByVal Value As String)
                _AdditionalFlags("Fax") = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The phone number
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PhoneNumber() As String
            Get
                Return AdditionalFlags("Phone")
            End Get
            Set(ByVal Value As String)
                _AdditionalFlags("Phone") = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The mobile number
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property MobileNumber() As String
            Get
                Return AdditionalFlags("Mobile")
            End Get
            Set(ByVal Value As String)
                _AdditionalFlags("Mobile") = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The position in the company the user is working for
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Position() As String
            Get
                Return AdditionalFlags("Position")
            End Get
            Set(ByVal Value As String)
                _AdditionalFlags("Position") = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The user's first name
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property FirstName() As String
            Get
                Return _FirstName
            End Get
            Set(ByVal Value As String)
                _FirstName = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The company title 
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Company() As String
            Get
                Return _Company
            End Get
            Set(ByVal Value As String)
                _Company = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The surname of the user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LastName() As String
            Get
                Return _LastName
            End Get
            Set(ByVal Value As String)
                _LastName = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The full name of an user, e. g. "Adam van Vrede")
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	14.12.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function FullName() As String
            Return IIf(_AcademicTitle = "", "", _AcademicTitle & " ") & _
                _FirstName & " " & _
                IIf(_NameAddition = "", "", _NameAddition & " ") & _
                _LastName
        End Function
        <Obsolete("use FullName instead")> Public ReadOnly Property CompleteName() As String
            Get
                Return FullName()
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The salutation name, e. g. "Dr. van Vrede"
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	14.12.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SalutationNameOnly() As String
            Return IIf(_AcademicTitle = "", "", _AcademicTitle & " ") & _
                IIf(_NameAddition = "", "", _NameAddition & " ") & _
                _LastName
        End Function
        <Obsolete("use SalutationNameOnly instead")> Public ReadOnly Property CompleteSalutationName() As String
            Get
                Return SalutationNameOnly()
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Save this user information object with the default notifications
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Save()
            Save(_WebManager.DefaultNotifications)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Save this user information object
        ''' </summary>
        ''' <param name="Notifications">A notifications class which shall be used for messages to the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Save(ByVal Notifications As CompuMaster.camm.WebManager.WMNotifications)
            Save(Notifications, Nothing)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Save this user information object
        ''' </summary>
        ''' <param name="Notifications">A notifications class which shall be used for messages to the user</param>
        ''' <param name="NewPassword">The new password for the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Save(ByVal Notifications As CompuMaster.camm.WebManager.WMNotifications, ByVal NewPassword As String)
            _WebManager.System_SetUserInfo(Me, NewPassword, Notifications)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Save this user information object with the default notifications
        ''' </summary>
        ''' <param name="NewPassword">The new password for the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Save(ByVal NewPassword As String)
            Save(_WebManager.DefaultNotifications, NewPassword)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The general salutation for a person, e. g. "Mr. Bell" or "Dr. van Vrede"
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	14.12.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function Salutation() As String
            If _AcademicTitle = "" Then
                'Ms./Mr. required
                Select Case Me.Gender
                    Case Gender.Feminin
                        Return Me._WebManager.Internationalization.UserManagementAddressesMs & SalutationNameOnly()
                    Case Gender.Undefined
                        Return SalutationFullName()
                    Case Sex.Masculin
                        Return Me._WebManager.Internationalization.UserManagementAddressesMr & SalutationNameOnly()
                    case Sex.MissingNameOrGroupOfPersons
                        return ""
                End Select
            Else
                'Dr./Prof. or similar already present
                Return SalutationNameOnly()
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The salutation for mail purposes, e. g. "Dear Mr. van Vrede, " or "Dear Dr. van Vrede, "
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	14.12.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SalutationInMails() As String
            If _AcademicTitle = "" Then
                'Ms./Mr. required
                Select Case Me.Gender
                    Case Gender.Feminin
                        Return Me._WebManager.Internationalization.UserManagementEMailTextDearMs & SalutationNameOnly() & ", "
                    Case Gender.Undefined
                        Return Me._WebManager.Internationalization.UserManagementEMailTextDear & SalutationFullName() & ", "
                    Case Sex.Masculin
                        Return Me._WebManager.Internationalization.UserManagementEMailTextDearMr & SalutationNameOnly() & ", "
                    case else 'Sex.MissingNameOrGroupOfPersons
                        return Me._webmanager.internationalization.UserManagementEMailTextDearGroup & ", "
                End Select
            Else
                'Dr./Prof. or similar already present
                '"Dear " required
                Select Case Me.Gender
                    Case Gender.Feminin
                        Return Me._WebManager.Internationalization.UserManagementEMailTextDearMsWithAcademicTitle & SalutationNameOnly() & ", "
                    Case Gender.Undefined
                        Return Me._WebManager.Internationalization.UserManagementEMailTextDearUndefinedGenderWithAcademicTitle & SalutationFullName() & ", "
                    Case Sex.Masculin
                        Return Me._WebManager.Internationalization.UserManagementEMailTextDearMrWithAcademicTitle & SalutationNameOnly() & ", "
                    Case else 'Sex.MissingNameOrGroupOfPersons
                        return Me._webmanager.internationalization.UserManagementEMailTextDearGroup & ", "
                End Select
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The salutation for mail purposes, e. g. "Hello Mr. Bell, " or "Hello Dr. van Vrede, "
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	14.12.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SalutationUnformal() As String
            Select Case Me.Gender
                Case Gender.Feminin
                    Return Me._WebManager.Internationalization.UserManagementSalutationUnformalFeminin & Salutation() & ", "
                Case Sex.Masculin
                    Return Me._WebManager.Internationalization.UserManagementSalutationUnformalMasculin & Salutation() & ", "
                Case Sex.Undefined
                    Return Me._webManager.internationalization.UsermanagementsalutationunformalUndefinedGender & salutation() & ", "
                case else 'Sex.MissingNameOrGroupOfPersons
                    return me._webmanager.internationalization.usermanagementsalutationunformalGroup & ", "
            End Select
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional academic title, typically 'Prof.' or 'Dr.'
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AcademicTitle() As String
            Get
                Return _AcademicTitle
            End Get
            Set(ByVal Value As String)
                _AcademicTitle = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The street
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Street() As String
            Get
                Return _Street
            End Get
            Set(ByVal Value As String)
                _Street = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The zip code
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ZipCode() As String
            Get
                Return _ZipCode
            End Get
            Set(ByVal Value As String)
                _ZipCode = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The location or city
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Location() As String
            Get
                Return _City
            End Get
            Set(ByVal Value As String)
                _City = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The state in the country
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property State() As String
            Get
                Return _State
            End Get
            Set(ByVal Value As String)
                _State = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The country name 
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Country() As String
            Get
                Return _Country
            End Get
            Set(ByVal Value As String)
                _Country = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The primary preferred language or market
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The second preferred language or market
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The third preferred language or market
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An additional pre-name, e. g. 'de' in the name 'Jean-Claude de Verheugen'
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property NameAddition() As String
            Get
                Return _NameAddition
            End Get
            Set(ByVal Value As String)
                _NameAddition = Utils.StringNotNothingOrEmpty(Value)
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The gender of the user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Gender() As Sex
            Get
                    If _Sex = Sex.MissingNameOrGroupOfPersons OrElse _Sex = Sex.Undefined Then
                        If Me.FirstName <> Nothing AndAlso Me.LastName <> Nothing Then
                            Return Sex.Undefined
                        Else
                            Return Sex.MissingNameOrGroupOfPersons
                        End If
                    Else
                        Return _Sex
                    End If
            End Get
            Set(ByVal Value As Sex)
                _Sex = Gender
            End Set
        End Property
        <Obsolete("use Gender instead")> Public Property Sex() As Sex
            Get
                    If _Sex = Sex.MissingNameOrGroupOfPersons OrElse _Sex = Sex.Undefined Then
                        If Me.FirstName <> Nothing AndAlso Me.LastName <> Nothing Then
                            Return Sex.Undefined
                        Else
                            Return Sex.MissingNameOrGroupOfPersons
                        End If
                    Else
                        Return _Sex
                    End If
            End Get
            Set(ByVal Value As Sex)
                _Sex = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Login has been disabled
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LoginDisabled() As Boolean
            Get
                Return _LoginDisabled
            End Get
            Set(ByVal Value As Boolean)
                _LoginDisabled = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Get/set the temporary lock state
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LoginLockedTemporary() As Boolean
            Get
                Return _LoginLockedTemporary
            End Get
            Set(ByVal Value As Boolean)
                _LoginLockedTemporary = Value
                If Value = True Then
                    If _PartiallyLoadedDataCurrently Then
                        ReadCompleteUserInformation()
                    End If
                    If Not _LoginLockedTemporaryTill = Nothing Then
                        _LoginLockedTemporaryTill = Now
                    End If
                Else
                    _LoginLockedTemporaryTill = Nothing
                End If
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Login has been temporary locked till this date
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend ReadOnly Property LoginLockedTemporaryTill() As DateTime
            Get
                Return _LoginLockedTemporaryTill
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Login has been deleted
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LoginDeleted() As Boolean
            Get
                Return _LoginDeleted
            End Get
            Set(ByVal Value As Boolean)
                _LoginDeleted = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The groups list where the user is member of
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Memberships() As CompuMaster.camm.WebManager.WMSystem.GroupInformation()
            Get
                If _Memberships Is Nothing Then
                    Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                    Dim MyCmd As New SqlCommand("select gruppen.* from memberships left join gruppen on memberships.id_group = gruppen.id where id_user = @ID and gruppen.id is not null", MyConn)
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                    Dim MyDA As SqlDataAdapter
                    Dim MyDataSet As DataSet = New DataSet
                    Try
                        MyConn.Open()
                        MyDA = New SqlDataAdapter(MyCmd)
                        MyDA.Fill(MyDataSet, "Groups")
                        MyCmd.Dispose()
                        MyCmd = Nothing
                    Finally
                        If Not MyDA Is Nothing Then
                            MyDA.Dispose()
                        End If
                        If Not MyCmd Is Nothing Then
                            MyCmd.Dispose()
                        End If
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End Try
                    If MyDataSet.Tables("Groups").Rows.Count > 0 Then
                        ReDim Preserve _Memberships(MyDataSet.Tables("Groups").Rows.Count - 1)
                        Dim MyCounter As Integer = 0
                        For Each MyDataRow As DataRow In MyDataSet.Tables("Groups").Rows
                            _Memberships(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(MyDataRow("ID"), MyDataRow("Name"), _WebManager.System_Nz(MyDataRow("Description")), IIf(MyDataRow("SystemGroup") <> 0, True, False), _WebManager)
                            MyCounter += 1
                        Next
                    Else
                        _Memberships = Nothing
                    End If
                End If
                Return _Memberships
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.GroupInformation())
                _Memberships = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Additional, optional flags
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Additional flags are typically used by applications which have to store some data in the user's profile
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AdditionalFlags() As Collections.Specialized.NameValueCollection
            Get
                If _PartiallyLoadedDataCurrently Then
                    ReadCompleteUserInformation()
                End If
                Return _AdditionalFlags
            End Get
            Set(ByVal Value As Collections.Specialized.NameValueCollection)
                _AdditionalFlags = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The access level role of the user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AccessLevel() As AccessLevelInformation
            Get
                If _AccessLevel Is Nothing Then
                    _AccessLevel = New AccessLevelInformation(_AccessLevelID, _WebManager)
                End If
                Return _AccessLevel
            End Get
            Set(ByVal Value As AccessLevelInformation)
                _AccessLevel = Value
                _AccessLevelID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Indicates if the e-mail address has already been validated
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AccountProfileValidatedByEMailTest() As Boolean
            Get
                If _PartiallyLoadedDataCurrently Then
                    ReadCompleteUserInformation()
                End If
                Return _AccountProfileValidatedByEMailTest
            End Get
            Set(ByVal Value As Boolean)
                'ToDo - JW 2004-01-20: Where to write that property into?
                Dim Message As String = "Not yet supported"
                _WebManager.Log.RuntimeException(Message)
                _AccountProfileValidatedByEMailTest = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add a membership to a user group
        ''' </summary>
        ''' <param name="GroupID">The group ID</param>
        ''' <param name="Notifications">A notification class which contains the e-mail templates which might be sent</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddMembership(ByVal GroupID As Integer, Optional ByVal Notifications As WMNotifications = Nothing)

            If _ID = SpecialUsers.User_Anonymous OrElse _ID = SpecialUsers.User_Public OrElse _ID = SpecialUsers.User_Code OrElse _ID = SpecialUsers.User_UpdateProcessor Then
                Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                _WebManager.Log.RuntimeException(Message)
            ElseIf _ID = Nothing Then
                Dim Message As String = "User has to be created, first, before you can modify the list of memberships"
                _WebManager.Log.RuntimeException(Message)
            End If

            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("AdminPrivate_CreateMemberships", MyConn)
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _ID
            Try
                MyConn.Open()
                Dim Result
                Result = MyCmd.ExecuteScalar
                If IsDBNull(Result) OrElse Result Is Nothing Then
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf Result = -1 Then
                    'Success
                Else
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                End If
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            If _System_InitOfAuthorizationsDone = False Then
                'send e-mail when first membership has been set up
                _System_InitOfAuthorizationsDone = True
                'Check wether InitAuthorizationsDone flag has been set
                If _WebManager.System_SetUserDetail(_ID, "InitAuthorizationsDone", "1", True) then
                    Try
                        If Notifications Is Nothing Then
                            _WebManager.DefaultNotifications.NotificationForUser_AuthorizationsSet(Me)
                        Else
                            Notifications.NotificationForUser_AuthorizationsSet(Me)
                        End If
                    Catch
                    End Try
                End If
            End If

            'Requery the list of memberships next time it's required
            _Memberships = Nothing

        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Remove a membership
        ''' </summary>
        ''' <param name="GroupID">The group ID the user shall not be member of any more</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RemoveMembership(ByVal GroupID As Integer)

            If _ID = SpecialUsers.User_Anonymous OrElse _ID = SpecialUsers.User_Public OrElse _ID = SpecialUsers.User_Code OrElse _ID = SpecialUsers.User_UpdateProcessor Then
                Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                _WebManager.Log.RuntimeException(Message)
            ElseIf _ID = Nothing Then
                Dim Message As String = "User has to be created, first, before you can modify the list of memberships"
                _WebManager.Log.RuntimeException(Message)
            End If

            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID", MyConn)
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = _ID
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = GroupID
            Try
                MyConn.Open()
                MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            _Memberships = Nothing

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An external account relation
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ExternalAccount() As String
            Get
                Return _ExternalAccount
            End Get
            Set(ByVal Value As String)
                _ExternalAccount = Value
            End Set
        End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.LanguageInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Language details
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LanguageInformation
        Dim _WebManager As WMSystem
        Dim _ID As Integer
        Dim _LanguageName_English As String
        Dim _LanguageName_OwnLanguage As String
        Dim _IsActive As Boolean
        Dim _BrowserLanguageID As String
        Dim _Abbreviation As String
        Friend Sub New(ByVal ID As Integer, ByRef LanguageName_English As String, ByVal LanguageName_OwnLanguage As String, ByVal IsActive As Boolean, ByVal BrowserLanguageID As String, ByVal Abbreviation As String, ByRef WebManager As WMSystem)
            _ID = ID
            _LanguageName_English = LanguageName_English
            _LanguageName_OwnLanguage = LanguageName_OwnLanguage
            _IsActive = IsActive
            _BrowserLanguageID = BrowserLanguageID
            _Abbreviation = Abbreviation
            _WebManager = WebManager
        End Sub
        Public Sub New(ByVal ID As Integer, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            Dim MyConn As New SqlConnection(WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("select * from Languages where id = @ID", MyConn)
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
            Dim MyReader As SqlDataReader
            Try
                MyConn.Open()
                MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                If MyReader.Read Then
                    _ID = MyReader("ID")
                    _LanguageName_English = WebManager.System_Nz(MyReader("Description"))
                    _LanguageName_OwnLanguage = _WebManager.System_Nz(MyReader("Description_OwnLang"))
                    _IsActive = WebManager.System_Nz(MyReader("IsActive"))
                    _BrowserLanguageID = _WebManager.System_Nz(MyReader("BrowserLanguageID"))
                    _Abbreviation = _WebManager.System_Nz(MyReader("Abbreviation"))
                End If
            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The market/language ID
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The name of the market/language in English language
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LanguageName_English() As String
            Get
                Return _LanguageName_English
            End Get
            Set(ByVal Value As String)
                _LanguageName_English = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The name of the market/language in its own language
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property LanguageName_OwnLanguage() As String
            Get
                Return _LanguageName_OwnLanguage
            End Get
            Set(ByVal Value As String)
                _LanguageName_OwnLanguage = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Market/language has been activated for use in camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsActive() As Boolean
            Get
                Return _IsActive
            End Get
            Set(ByVal Value As Boolean)
                _IsActive = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional browser ID for the culture
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property BrowserLanguageID() As String
            Get
                Return _BrowserLanguageID
            End Get
            Set(ByVal Value As String)
                _BrowserLanguageID = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional abbreviation name for the language
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Abbreviation() As String
            Get
                Return _Abbreviation
            End Get
            Set(ByVal Value As String)
                _Abbreviation = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional alternative language, regulary present for market identifiers
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This information takes regulary effect for markets. 
        ''' </remarks>
        ''' <example>
        '''     For 'English (US)' as well as 'English (GB)', there is the alternative language 'English'.
        ''' </example>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property AlternativeLanguageInfo() As LanguageInformation
            Get
                Return New LanguageInformation(_WebManager.Internationalization.GetAlternativelySupportedLanguageID(_ID), _WebManager)
            End Get
        End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.AccessLevelInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Access level information
    ''' </summary>
    ''' <remarks>
    '''     Access levels are user roles defining the availability of the existant server groups for the user
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class AccessLevelInformation
        Dim _WebManager As WMSystem
        Dim _ID As Integer
        Dim _Title As String
        Dim _Remarks As String
        Dim _ServerGroups As ServerGroupInformation()
        Dim _Users As UserInformation()
        Public Sub New(ByVal ID As Integer, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            Dim MyConn As New SqlConnection(WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("select * from system_accesslevels where id = @ID", MyConn)
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
            Dim MyReader As SqlDataReader
            Try
                MyConn.Open()
                MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                If MyReader.Read Then
                    _ID = MyReader("ID")
                    _Title = WebManager.System_Nz(MyReader("Title"))
                    _Remarks = _WebManager.System_Nz(MyReader("Remarks"))
                End If
            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value for this access level role
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The title for this access level role
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal Value As String)
                _Title = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Some optional remarks on this role
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Remarks() As String
            Get
                Return _Remarks
            End Get
            Set(ByVal Value As String)
                _Remarks = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A list of server groups which are accessable by this role
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ServerGroups() As ServerGroupInformation()
            Get
                If _ServerGroups Is Nothing Then
                    _ServerGroups = _WebManager.System_GetServerGroupsInfo(_ID)
                End If
                Return _ServerGroups
            End Get
            Set(ByVal Value As ServerGroupInformation())
                _ServerGroups = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A list of users which are assigned to this role
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Users() As UserInformation()
            Get
                If _Users Is Nothing Then
                    Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                    Dim MyCmd As New SqlCommand("select * from benutzer where benutzer.AccountAccessability = @ID order by [1stPreferredLanguage]", MyConn)
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                    Dim MyDataSet As DataSet = New DataSet
                    Dim MyDA As SqlDataAdapter = New SqlDataAdapter(MyCmd)
                    Try
                        MyConn.Open()
                        MyDA.Fill(MyDataSet, "Users")
                    Finally
                        If Not MyDA Is Nothing Then
                            MyDA.Dispose()
                        End If
                        If Not MyCmd Is Nothing Then
                            MyCmd.Dispose()
                        End If
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End Try
                    If MyDataSet.Tables("Users").Rows.Count > 0 Then
                        ReDim Preserve _Users(MyDataSet.Tables("Users").Rows.Count - 1)
                        Dim MyCounter As Integer = 0
                        For Each MyDataRow As DataRow In MyDataSet.Tables("Users").Rows
                            _Users(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(MyDataRow("ID")), _
                                            MyDataRow("LoginName"), _
                                            MyDataRow("E-Mail"), _
                                            False, _
                                            _WebManager.System_Nz(MyDataRow("Company")), _
                                            IIf(Convert.ToString(_WebManager.System_Nz(MyDataRow("Anrede"))) = "", Sex.Undefined, IIf(Convert.ToString(_WebManager.System_Nz(MyDataRow("Anrede"))) = "Mr.", Sex.Masculin, Sex.Feminin)), _
                                            _WebManager.System_Nz(MyDataRow("Namenszusatz")), _
                                            MyDataRow("Vorname"), _
                                            MyDataRow("Nachname"), _
                                            _WebManager.System_Nz(MyDataRow("Titel")), _
                                            _WebManager.System_Nz(MyDataRow("Strasse")), _
                                            _WebManager.System_Nz(MyDataRow("PLZ")), _
                                            _WebManager.System_Nz(MyDataRow("Ort")), _
                                            _WebManager.System_Nz(MyDataRow("State")), _
                                            _WebManager.System_Nz(MyDataRow("Land")), _
                                            MyDataRow("1stPreferredLanguage"), _
                                            _WebManager.System_Nz(MyDataRow("2ndPreferredLanguage"), Nothing), _
                                            _WebManager.System_Nz(MyDataRow("3rdPreferredLanguage"), Nothing), _
                                            MyDataRow("LoginDisabled"), _
                                            Not IsDBNull(MyDataRow("LoginLockedTill")), _
                                            False, _
                                            MyDataRow("AccountAccessability"), _
                                            _WebManager, _
                                            Nothing)
                            MyCounter += 1
                        Next
                    Else
                        _Users = Nothing
                    End If
                End If
                Return _Users
            End Get
            Set(ByVal Value As UserInformation())
                _Users = Value
            End Set
        End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.GroupInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Group information
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GroupInformation
        Dim _WebManager As WMSystem
        Dim _ID As Integer
        Dim _Name As String
        Dim _Description As String
        Dim _IsSystemGroup As Boolean
        Dim _Members As CompuMaster.camm.WebManager.WMSystem.UserInformation()
        Friend Sub New(ByVal GroupID As Integer, ByVal InternalName As String, ByVal Description As String, ByVal IsSystemGroup As Boolean, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem)
            _ID = GroupID
            _Name = InternalName
            _Description = Description
            _IsSystemGroup = IsSystemGroup
            _WebManager = WebManager
        End Sub
        Public Sub New(ByVal GroupID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem)
            _WebManager = WebManager
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("select * from gruppen where id = @ID", MyConn)
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = GroupID
            Dim MyReader As SqlDataReader
            Try
                MyConn.Open()
                MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                If MyReader.Read Then
                    _ID = MyReader("ID")
                    _Name = _WebManager.System_Nz(MyReader("Name"))
                    _Description = _WebManager.System_Nz(MyReader("Description"))
                    _IsSystemGroup = IIf(MyReader("SystemGroup") = 0, False, True)
                End If
            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value for this group
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        <Obsolete("use Name instead")> Public Property InternalName() As String 'to be subject of removal in v3.x
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The title for this user group
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional description 
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Indicates wether this group is a system group (e. g. Security Administration)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IsSystemGroup() As Boolean
            Get
                Return _IsSystemGroup
            End Get
            Set(ByVal Value As Boolean)
                _IsSystemGroup = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A list of members
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Members() As CompuMaster.camm.WebManager.WMSystem.UserInformation()
            Get
                If _Members Is Nothing Then
                    Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                    Dim MyCmd As New SqlCommand("select benutzer.* from memberships left join benutzer on memberships.id_user = benutzer.id where memberships.id_group = @ID and benutzer.id is not null", MyConn)
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                    Dim MyDataSet As DataSet = New DataSet
                    Dim MyDA As SqlDataAdapter = New SqlDataAdapter(MyCmd)
                    Try
                        MyConn.Open()
                        MyDA.Fill(MyDataSet, "Users")
                    Finally
                        If Not MyDA Is Nothing Then
                            MyDA.Dispose()
                        End If
                        If Not MyCmd Is Nothing Then
                            MyCmd.Dispose()
                        End If
                        If Not MyConn Is Nothing Then
                            If MyConn.State <> ConnectionState.Closed Then
                                MyConn.Close()
                            End If
                            MyConn.Dispose()
                        End If
                    End Try
                    If MyDataSet.Tables("Users").Rows.Count > 0 Then
                        ReDim Preserve _Members(MyDataSet.Tables("Users").Rows.Count - 1)
                        Dim MyCounter As Integer = 0
                        For Each MyDataRow As DataRow In MyDataSet.Tables("Users").Rows
                            _Members(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(MyDataRow("ID")), _
                                            MyDataRow("LoginName"), _
                                            MyDataRow("E-Mail"), _
                                            False, _
                                            _WebManager.System_Nz(MyDataRow("Company")), _
                                            IIf(Convert.ToString(_WebManager.System_Nz(MyDataRow("Anrede"))) = "", Sex.Undefined, IIf(Convert.ToString(_WebManager.System_Nz(MyDataRow("Anrede"))) = "Mr.", Sex.Masculin, Sex.Feminin)), _
                                            _WebManager.System_Nz(MyDataRow("Namenszusatz")), _
                                            MyDataRow("Vorname"), _
                                            MyDataRow("Nachname"), _
                                            _WebManager.System_Nz(MyDataRow("Titel")), _
                                            _WebManager.System_Nz(MyDataRow("Strasse")), _
                                            _WebManager.System_Nz(MyDataRow("PLZ")), _
                                            _WebManager.System_Nz(MyDataRow("Ort")), _
                                            _WebManager.System_Nz(MyDataRow("State")), _
                                            _WebManager.System_Nz(MyDataRow("Land")), _
                                            MyDataRow("1stPreferredLanguage"), _
                                            _WebManager.System_Nz(MyDataRow("2ndPreferredLanguage"), Nothing), _
                                            _WebManager.System_Nz(MyDataRow("3rdPreferredLanguage"), Nothing), _
                                            MyDataRow("LoginDisabled"), _
                                            Not IsDBNull(MyDataRow("LoginLockedTill")), _
                                            False, _
                                            MyDataRow("AccountAccessability"), _
                                            _WebManager, _
                                            Nothing)
                            MyCounter += 1
                        Next
                    Else
                        _Members = Nothing
                    End If
                End If
                Return _Members
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.UserInformation())
                _Members = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add a new user to the list of members
        ''' </summary>
        ''' <param name="UserInfo">The new user</param>
        ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddMember(ByRef UserInfo As UserInformation, Optional ByVal Notifications As WMNotifications = Nothing)

            If UserInfo.ID = SpecialUsers.User_Anonymous OrElse UserInfo.ID = SpecialUsers.User_Public OrElse UserInfo.ID = SpecialUsers.User_UpdateProcessor OrElse UserInfo.ID = SpecialUsers.User_Code Then
                Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                _WebManager.Log.RuntimeException(Message)
            ElseIf _ID = Nothing Then
                Dim Message As String = "Group has to be created, first, before you can modify the list of members"
                _WebManager.Log.RuntimeException(Message)
            End If

            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("AdminPrivate_CreateMemberships", MyConn)
            MyCmd.CommandType = CommandType.StoredProcedure
            MyCmd.Parameters.Add("@ReleasedByUserID", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = _ID
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserInfo.ID
            Try
                MyConn.Open()
                Dim Result
                Result = MyCmd.ExecuteScalar
                If IsDBNull(Result) OrElse Result Is Nothing Then
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                ElseIf Result = -1 Then
                    'Success
                Else
                    Dim Message As String = "Membership creation failed"
                    _WebManager.Log.RuntimeException(Message)
                End If
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            If UserInfo.AccountAuthorizationsAlreadySet = False Then
                'send e-mail when first membership has been set up
                UserInfo.AccountAuthorizationsAlreadySet = True
                'Check wether InitAuthorizationsDone flag has been set
                If _WebManager.System_SetUserDetail(UserInfo.IDLong, "InitAuthorizationsDone", "1", True) Then
                    Try
                        If Notifications Is Nothing Then
                            _WebManager.DefaultNotifications.NotificationForUser_AuthorizationsSet(UserInfo)
                        Else
                            Notifications.NotificationForUser_AuthorizationsSet(UserInfo)
                        End If
                    Catch
                    End Try
                End If
            End If

            _Members = Nothing

        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add a new user to the list of members
        ''' </summary>
        ''' <param name="UserID">The ID value of the new user</param>
        ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete()> Public Sub AddMember(ByVal UserID As Integer, Optional ByVal Notifications As WMNotifications = Nothing)
            AddMember(CLng(UserID), Notifications)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add a new user to the list of members
        ''' </summary>
        ''' <param name="UserID">The ID value of the new user</param>
        ''' <param name="Notifications">A notification class which can be user for sending messages to the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub AddMember(ByVal UserID As Long, Optional ByVal Notifications As WMNotifications = Nothing)

            If UserID = SpecialUsers.User_Anonymous OrElse UserID = SpecialUsers.User_Public OrElse UserID = SpecialUsers.User_Code OrElse UserID = SpecialUsers.User_UpdateProcessor Then
                Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                _WebManager.Log.RuntimeException(Message)
            ElseIf _ID = Nothing Then 'Check here again before spending time on getting complete user infos when it's clear that our main method will fail
                Dim Message As String = "Group has to be created, first, before you can modify the list of members"
                _WebManager.Log.RuntimeException(Message)
            End If
            AddMember(New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, _WebManager), Notifications)

        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Remove a user from the list of members
        ''' </summary>
        ''' <param name="UserID">The ID value of the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete()> Public Sub RemoveMember(ByVal UserID As Integer)
            RemoveMember(CLng(UserID))
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Remove a user from the list of members
        ''' </summary>
        ''' <param name="UserID">The ID value of the user</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RemoveMember(ByVal UserID As Long)

            If UserID = SpecialUsers.User_Anonymous OrElse UserID = SpecialUsers.User_Public OrElse UserID = SpecialUsers.User_Code OrElse UserID = SpecialUsers.User_UpdateProcessor Then
                Dim Message As String = "An 'anonymous' user or a 'public' user never can be a member of another group"
                _WebManager.Log.RuntimeException(Message)
            ElseIf _ID = Nothing Then
                Dim Message As String = "Group has to be created, first, before you can modify the list of members"
                _WebManager.Log.RuntimeException(Message)
            End If

            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("DELETE FROM dbo.Memberships WHERE ID_User=@UserID AND ID_Group=@GroupID", MyConn)
            MyCmd.CommandType = CommandType.Text
            MyCmd.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID
            MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = _ID
            Try
                MyConn.Open()
                MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            _Members = Nothing

        End Sub
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.ServerGroupInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Server group information
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ServerGroupInformation
        Dim _WebManager As WMSystem
        Dim _ID As Integer
        Dim _Title As String
        Dim _NavTitle As String
        Dim _MasterServer As ServerInformation
        Dim _MasterServerID As Integer
        Dim _AdminServer As ServerInformation
        Dim _AdminServerID As Integer
        Dim _AccessLevelDefaultID As Integer
        Dim _AccessLevelDefault As AccessLevelInformation
        Dim _OfficialCompanyWebSiteTitle As String
        Dim _OfficialCompanyWebSiteURL As String
        Dim _CompanyTitle As String
        Dim _CompanyFormerTitle As String
        Dim _GroupAnonymousID As Integer
        Dim _GroupPublicID As Integer
        Dim _GroupAnonymous As GroupInformation
        Dim _GroupPublic As GroupInformation
        Dim _Servers As ServerInformation()
        Friend Sub New(ByVal ServerGroupID As Integer, ByVal Title As String, ByVal NavTitle As String, ByVal OfficialCompanyWebSiteTitle As String, ByVal OfficialCompanyWebSiteURL As String, ByVal CompanyTitle As String, ByVal CompanyFormerTitle As String, ByVal AccessLevelDefaultID As Integer, ByVal MasterServerID As Integer, ByVal AdminServerID As Integer, ByVal GroupAnonymousID As Integer, ByVal GroupPublicID As Integer, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            _ID = ServerGroupID
            _Title = Title
            _NavTitle = NavTitle
            _OfficialCompanyWebSiteTitle = OfficialCompanyWebSiteTitle
            _OfficialCompanyWebSiteURL = OfficialCompanyWebSiteURL
            _CompanyTitle = CompanyTitle
            _CompanyFormerTitle = CompanyFormerTitle
            _AccessLevelDefaultID = AccessLevelDefaultID
            _AdminServerID = AdminServerID
            _MasterServerID = MasterServerID
            _GroupAnonymousID = GroupAnonymousID
            _GroupPublicID = GroupPublicID
        End Sub
        Public Sub New(ByVal ServerGroupID As Integer, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            Dim MyConn As New SqlConnection(WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("select * from system_servergroups where id = @ID", MyConn)
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServerGroupID
            Dim MyReader As SqlDataReader
            Try
                MyConn.Open()
                MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                If MyReader.Read Then
                    _ID = MyReader("ID")
                    _Title = WebManager.System_Nz(MyReader("ServerGroup"))
                    _NavTitle = _WebManager.System_Nz(MyReader("AreaNavTitle"))
                    _OfficialCompanyWebSiteTitle = _WebManager.System_Nz(MyReader("AreaCompanyWebSiteTitle"))
                    _OfficialCompanyWebSiteURL = _WebManager.System_Nz(MyReader("AreaCompanyWebSiteURL"))
                    _CompanyTitle = _WebManager.System_Nz(MyReader("AreaCompanyTitle"))
                    _CompanyFormerTitle = _WebManager.System_Nz(MyReader("AreaCompanyFormerTitle"))
                    _AccessLevelDefaultID = _WebManager.System_Nz(MyReader("AccessLevel_Default"))
                    _AdminServerID = _WebManager.System_Nz(MyReader("UserAdminServer"))
                    _MasterServerID = _WebManager.System_Nz(MyReader("MasterServer"))
                    _GroupAnonymousID = _WebManager.System_Nz(MyReader("ID_Group_Anonymous"))
                    _GroupPublicID = _WebManager.System_Nz(MyReader("ID_Group_Public"))
                End If
            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
        End Sub
        '''  -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value of this server group
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The common title of this server group
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal Value As String)
                _Title = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The title of this server group in a shorter name, often used for the navigation bars
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property NavTitle() As String
            Get
                If _NavTitle <> "" Then
                    Return _NavTitle
                Else
                    Return _Title
                End If
            End Get
            Set(ByVal Value As String)
                _NavTitle = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The official website title of the company, typically used for the link/logo from the extranet to the internet website
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property OfficialCompanyWebSiteTitle() As String
            Get
                Return _OfficialCompanyWebSiteTitle
            End Get
            Set(ByVal Value As String)
                _OfficialCompanyWebSiteTitle = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The official website address of the company, typically used for the link/logo from the extranet to the internet website
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property OfficialCompanyWebSiteURL() As String
            Get
                Return _OfficialCompanyWebSiteURL
            End Get
            Set(ByVal Value As String)
                _OfficialCompanyWebSiteURL = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The company title, e. g. 'YourCompany'
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CompanyTitle() As String
            Get
                Return _CompanyTitle
            End Get
            Set(ByVal Value As String)
                _CompanyTitle = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The official company title, e. g. 'YourCompany Ltd.'
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CompanyFormerTitle() As String
            Get
                Return _CompanyFormerTitle
            End Get
            Set(ByVal Value As String)
                _CompanyFormerTitle = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value for the group of registered users
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property GroupPublic() As CompuMaster.camm.WebManager.WMSystem.GroupInformation
            Get
                If _GroupPublic Is Nothing Then
                    _GroupPublic = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(_GroupPublicID, _WebManager)
                End If
                Return _GroupPublic
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.GroupInformation)
                _GroupPublic = Value
                _GroupPublicID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value for the group of unregistered users
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property GroupAnonymous() As CompuMaster.camm.WebManager.WMSystem.GroupInformation
            Get
                If _GroupAnonymous Is Nothing Then
                    _GroupAnonymous = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(_GroupAnonymousID, _WebManager)
                End If
                Return _GroupAnonymous
            End Get
            Set(ByVal Value As CompuMaster.camm.WebManager.WMSystem.GroupInformation)
                _GroupAnonymous = Value
                _GroupAnonymousID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The master server which is the primary handler for all login requests
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property MasterServer() As ServerInformation
            Get
                If _MasterServer Is Nothing Then
                    _MasterServer = New ServerInformation(_MasterServerID, _WebManager)
                End If
                Return _MasterServer
            End Get
            Set(ByVal Value As ServerInformation)
                _MasterServer = Value
                _MasterServerID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A reference to an administration server
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     This administration server can be part of another servergroup. This allows you to remove any administration possibilities from your untrusted extranet and to only allow user administration on a server in your intranet.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AdminServer() As ServerInformation
            Get
                If _AdminServer Is Nothing Then
                    _AdminServer = New ServerInformation(_AdminServerID, _WebManager)
                End If
                Return _AdminServer
            End Get
            Set(ByVal Value As ServerInformation)
                _AdminServer = Value
                _AdminServerID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The default access level role for all users who register themselves in this server group
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property AccessLevelDefault() As AccessLevelInformation
            Get
                If _AccessLevelDefault Is Nothing Then
                    _AccessLevelDefault = New AccessLevelInformation(_AccessLevelDefaultID, _WebManager)
                End If
                Return _AccessLevelDefault
            End Get
            Set(ByVal Value As AccessLevelInformation)
                _AccessLevelDefault = Value
                _AccessLevelDefaultID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A list of attached servers to this server group
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Servers() As ServerInformation()
            Get
                If _Servers Is Nothing Then
                    _Servers = _WebManager.System_GetServersInfo(_ID)
                End If
                Return _Servers
            End Get
            Set(ByVal Value As ServerInformation())
                _Servers = Value
            End Set
        End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.ServerInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Server information
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ServerInformation
        Dim _WebManager As WMSystem
        Dim _ID As Integer
        Dim _IP_Or_HostHeader As String
        Dim _Description As String
        Dim _URL_Protocol As String
        Dim _URL_DomainName As String
        Dim _URL_Port As String
        Dim _Enabled As Boolean
        Dim _ParentServerGroupID As Integer
        Dim _ParentServerGroup As ServerGroupInformation
        Dim _ServerSessionTimeout As Integer
        Dim _ServerUserlockingsTimeout As Integer

        Friend Sub New(ByVal ServerID As Integer, ByVal IP_Or_HostHeader As String, ByVal Description As String, ByVal URL_Protocol As String, ByVal URL_DomainName As String, ByVal URL_Port As String, ByVal Enabled As Boolean, ByVal ParentServerGroupID As Integer, ByRef WebManager As WMSystem, Optional ByVal ServerSessionTimeout As Integer = 15, Optional ByVal ServerUserlockingsTimeout As Integer = 3)
            _WebManager = WebManager
            _ID = ServerID
            _IP_Or_HostHeader = IP_Or_HostHeader
            _Description = Description
            _ParentServerGroupID = ParentServerGroupID
            _URL_Protocol = URL_Protocol
            _URL_DomainName = URL_DomainName
            _URL_Port = URL_Port
            _Enabled = Enabled
            _ServerSessionTimeout = ServerSessionTimeout
            _ServerUserlockingsTimeout = ServerUserlockingsTimeout
        End Sub
        Public Sub New(ByVal ServerID As Integer, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            LoadServerInfoFromDatabase(ServerID)
        End Sub
        Public Sub New(ByVal ServerIP As String, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            Dim ServerID As Integer = _WebManager.System_GetServerID(ServerIP)
            LoadServerInfoFromDatabase(ServerID)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load server information from database
        ''' </summary>
        ''' <param name="ServerID">A server ID</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadServerInfoFromDatabase(ByVal ServerID As Integer)
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand("select * from system_servers where id = @ID", MyConn)
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ServerID
            Dim MyReader As SqlDataReader
            Try
                MyConn.Open()
                MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                If MyReader.Read Then
                    _ID = MyReader("ID")
                    _IP_Or_HostHeader = _WebManager.System_Nz(MyReader("IP"))
                    _Description = _WebManager.System_Nz(MyReader("ServerDescription"))
                    _ParentServerGroupID = _WebManager.System_Nz(MyReader("ServerGroup"))
                    _URL_Protocol = _WebManager.System_Nz(MyReader("ServerProtocol"))
                    _URL_DomainName = _WebManager.System_Nz(MyReader("ServerName"), _IP_Or_HostHeader)
                    _URL_Port = _WebManager.System_Nz(MyReader("ServerPort"))
                    _Enabled = MyReader("Enabled")
                    _ServerSessionTimeout = MyReader("WebSessionTimeout")
                    _ServerUserlockingsTimeout = MyReader("LockTimeout")
                End If
            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value of this server
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The server identification string
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        '''     Typically, this is either an IP address or a host header name. This value can hold any ID, you only have to ensure that the server tries to login with that server identification string again. This can be set up in the web.config file or in /sysdata/config.*
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property IPAddressOrHostHeader() As String
            Get
                Return _IP_Or_HostHeader
            End Get
            Set(ByVal Value As String)
                _IP_Or_HostHeader = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The protocol name for the server, http or https
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property URL_Protocol() As String
            Get
                Return _URL_Protocol
            End Get
            Set(ByVal Value As String)
                _URL_Protocol = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The domain name this server is available at
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property URL_DomainName() As String
            Get
                Return _URL_DomainName
            End Get
            Set(ByVal Value As String)
                _URL_DomainName = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional port information if it's not the default port
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property URL_Port() As String
            Get
                Return _URL_Port
            End Get
            Set(ByVal Value As String)
                _URL_Port = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The server URL without trailing slash, e. g. http://www.yourcompany:8080
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ServerURL() As String
            Dim Field_ServerAddress As String
            Field_ServerAddress = _URL_Protocol & "://" & _URL_DomainName
            If _URL_Port <> Nothing AndAlso Not ((_URL_Port = "80" AndAlso _URL_Protocol.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "http") OrElse (_URL_Port = "443" AndAlso _URL_Protocol.ToLower(System.Globalization.CultureInfo.InvariantCulture) = "https")) Then
                Field_ServerAddress = Field_ServerAddress & ":" & _URL_Port
            End If
            Return Field_ServerAddress
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is this server activated?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Enabled() As Boolean
            Get
                Return _Enabled
            End Get
            Set(ByVal Value As Boolean)
                _Enabled = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An optional description for this server
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The parent server group where this server is assigned to
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ParentServerGroup() As ServerGroupInformation
            Get
                If _ParentServerGroup Is Nothing Then
                    _ParentServerGroup = New ServerGroupInformation(_ParentServerGroupID, _WebManager)
                End If
                Return _ParentServerGroup
            End Get
            Set(ByVal Value As ServerGroupInformation)
                _ParentServerGroup = Value
                _ParentServerGroupID = Value.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A session timeout value
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ServerSessionTimeout() As Integer
            Get
                Return _ServerSessionTimeout
            End Get
            Set(ByVal Value As Integer)
                _ServerSessionTimeout = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A timeout value how fast temporary locked users can logon again
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ServerUserlockingsTimeout() As Integer
            Get
                Return _ServerUserlockingsTimeout
            End Get
            Set(ByVal Value As Integer)
                _ServerUserlockingsTimeout = Value
            End Set
        End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.Authorizations
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Authorizations
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Authorizations
        Dim _WebManager As WMSystem
        Dim _SecurityObjectID As Integer
        Dim _ServerGroupID As Integer

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.Authorizations.GroupAuthorizationInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An authorization for an user group
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class GroupAuthorizationInformation
            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _SecurityObjectID As Integer
            Dim _SecurityObjectInfo As SecurityObjectInformation
            Dim _GroupID As Integer
            Dim _GroupInfo As GroupInformation
            Dim _ServerGroupID As Integer
            Dim _ServerGroupInfo As ServerGroupInformation
            Sub New(ByRef WebManager As WMSystem, ByVal ID As Integer, ByVal SecurityObjectID As Integer, ByVal GroupID As Integer, ByVal ServerGroupID As Integer)
                _WebManager = WebManager
                _ID = ID
                _SecurityObjectID = SecurityObjectID
                _GroupID = GroupID
                _ServerGroupID = ServerGroupID
            End Sub
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The ID value for this authorization item
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property ID() As Integer
                Get
                    Return _ID
                End Get
                Set(ByVal Value As Integer)
                    _ID = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The security object which is pointed by this authorization
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property SecurityObjectInfo() As SecurityObjectInformation
                Get
                    If _SecurityObjectInfo Is Nothing Then
                        _SecurityObjectInfo = New SecurityObjectInformation(_SecurityObjectID, _WebManager)
                    End If
                    Return _SecurityObjectInfo
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     A user group which has been authorized
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property GroupInfo() As CompuMaster.camm.WebManager.WMSystem.GroupInformation
                Get
                    If _GroupInfo Is Nothing Then
                        _GroupInfo = New CompuMaster.camm.WebManager.WMSystem.GroupInformation(_GroupID, _WebManager)
                    End If
                    Return _GroupInfo
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     A server group where this authorization shall take effect
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property ServerGroupInfo() As ServerGroupInformation
                Get
                    If _ServerGroupInfo Is Nothing Then
                        _ServerGroupInfo = New ServerGroupInformation(_ServerGroupID, _WebManager)
                    End If
                    Return _ServerGroupInfo
                End Get
            End Property
            '''  -----------------------------------------------------------------------------
            ''' <summary>
            '''     The ID value of the user group
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property GroupID() As Integer
                Get
                    Return _GroupID
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The ID value of the targetted security object
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property SecurityObjectID() As Integer
                Get
                    Return _SecurityObjectID
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The ID value of the effected server group
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property ServerGroupID() As Integer
                Get
                    Return _ServerGroupID
                End Get
            End Property
            Friend WriteOnly Property Friend_SecurityObjectInfo() As SecurityObjectInformation
                Set(ByVal Value As SecurityObjectInformation)
                    _SecurityObjectInfo = Value
                End Set
            End Property
            Friend WriteOnly Property Friend_GroupInfo() As GroupInformation
                Set(ByVal Value As GroupInformation)
                    _GroupInfo = Value
                End Set
            End Property
            Friend WriteOnly Property Friend_ServerGroupInfo() As ServerGroupInformation
                Set(ByVal Value As ServerGroupInformation)
                    _ServerGroupInfo = Value
                End Set
            End Property
        End Class
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.Authorizations.GroupAuthorizationInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     An authorization for an user
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class UserAuthorizationInformation
            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _SecurityObjectID As Integer
            Dim _SecurityObjectInfo As SecurityObjectInformation
            Dim _UserID As Long
            Dim _UserInfo As UserInformation
            Dim _ServerGroupID As Integer
            Dim _ServerGroupInfo As ServerGroupInformation
            Dim _AlsoVisibleIfDisabled As Boolean
            Sub New(ByRef WebManager As WMSystem, ByVal ID As Integer, ByVal SecurityObjectID As Integer, ByVal UserID As Integer, ByVal ServerGroupID As Integer, ByVal AlsoVisibleIfDisabled As Boolean)
                _WebManager = WebManager
                _ID = ID
                _SecurityObjectID = SecurityObjectID
                _UserID = UserID
                _ServerGroupID = ServerGroupID
                _AlsoVisibleIfDisabled = AlsoVisibleIfDisabled
            End Sub
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The ID value for this authorization item
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property ID() As Integer
                Get
                    Return _ID
                End Get
                Set(ByVal Value As Integer)
                    _ID = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Is the user allowed to see and access the link to this security object application even if the security object hasn't been activated?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     Often, developers need access to test their new applcations before they can go live
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property AlsoVisibleIfDisabled() As Boolean
                Get
                    Return _AlsoVisibleIfDisabled
                End Get
                Set(ByVal Value As Boolean)
                    _AlsoVisibleIfDisabled = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     A security object which is pointed by this authorization 
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property SecurityObjectInfo() As SecurityObjectInformation
                Get
                    If _SecurityObjectInfo Is Nothing Then
                        _SecurityObjectInfo = New SecurityObjectInformation(_SecurityObjectID, _WebManager)
                    End If
                    Return _SecurityObjectInfo
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The user which has got the authorization
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property UserInfo() As UserInformation
                Get
                    If _UserInfo Is Nothing Then
                        _UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_UserID, _WebManager)
                    End If
                    Return _UserInfo
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The server group where this authorization shall take effect
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property ServerGroupInfo() As ServerGroupInformation
                Get
                    If _ServerGroupInfo Is Nothing Then
                        _ServerGroupInfo = New ServerGroupInformation(_ServerGroupID, _WebManager)
                    End If
                    Return _ServerGroupInfo
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The user which has got the authorization
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property UserID() As Integer
                Get
                    Return _UserID
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     A security object which is pointed by this authorization 
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property SecurityObjectID() As Integer
                Get
                    Return _SecurityObjectID
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The server group where this authorization shall take effect
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property ServerGroupID() As Integer
                Get
                    Return _ServerGroupID
                End Get
            End Property
            Friend WriteOnly Property Friend_SecurityObjectInfo() As SecurityObjectInformation
                Set(ByVal Value As SecurityObjectInformation)
                    _SecurityObjectInfo = Value
                End Set
            End Property
            Friend WriteOnly Property Friend_UserInfo() As UserInformation
                Set(ByVal Value As UserInformation)
                    _UserInfo = Value
                End Set
            End Property
            Friend WriteOnly Property Friend_ServerGroupInfo() As ServerGroupInformation
                Set(ByVal Value As ServerGroupInformation)
                    _ServerGroupInfo = Value
                End Set
            End Property
        End Class
        Dim _AuthorizedGroups As New Collection
        Dim _AuthorizedUsers As New Collection
        Dim _AuthorizedGroupInfos As GroupAuthorizationInformation()
        Dim _AuthorizedUserInfos As UserAuthorizationInformation()
        Dim _DBVersion As Version
        Dim _ReloadData As Boolean

        Public Sub New(ByVal SecurityObjectID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal ServerGroupID As Integer = Nothing)
            _WebManager = WebManager
            _SecurityObjectID = SecurityObjectID
            _ServerGroupID = ServerGroupID

            'Preparation
            If SecurityObjectID = Nothing And ServerGroupID <> Nothing Then
                Throw New Exception("Not yet supported: list of security objects of a specific server group")
            End If
            _DBVersion = _WebManager.System_DBVersion_Ex
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                Throw New Exception("Support for database version " & _DBVersion.ToString & " is currently not supported. Please update the camm WebManager software, first!")
            End If

            Dim MyConn As New SqlConnection(WebManager.ConnectionString)
            Dim MyCmd As SqlCommand
            Dim MyReader As SqlDataReader
            Try
                MyConn.Open()

                'Fill the list of authorized users
                If SecurityObjectID = Nothing Then
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByUser]", MyConn)
                ElseIf ServerGroupID = Nothing Then
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDApplication", MyConn)
                    MyCmd.Parameters.Add("@IDApplication", SqlDbType.Int).Value = SecurityObjectID
                Else
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDApplication AND ID_ServerGroup = @IDServerGroup", MyConn)
                    MyCmd.Parameters.Add("@IDApplication", SqlDbType.Int).Value = SecurityObjectID
                    MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = ServerGroupID
                End If
                MyReader = MyCmd.ExecuteReader()
                While MyReader.Read
                    Dim MyServerGroup As Integer
                    If MyReader.GetSchemaTable.Columns.Contains("ID_ServerGroup") Then
                        MyServerGroup = System_Nz(MyReader("ID_ServerGroup"))
                    Else
                        MyServerGroup = Nothing
                    End If
                    _AuthorizedUsers.Add(New UserAuthorizationInformation(_WebManager, _
                        MyReader("ID"), _
                        MyReader("ID_Application"), _
                        MyReader("ID_GroupOrPerson"), _
                        MyServerGroup, _
                        System_Nz(MyReader("DevelopmentTeamMember"), False)))
                End While
                MyReader.Close()

                'Fill the list of authorized groups
                If SecurityObjectID = Nothing Then
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByGroup]", MyConn)
                ElseIf ServerGroupID = Nothing Then
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDApplication", MyConn)
                    MyCmd.Parameters.Add("@IDApplication", SqlDbType.Int).Value = SecurityObjectID
                Else
                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDApplication AND ID_ServerGroup = @IDServerGroup", MyConn)
                    MyCmd.Parameters.Add("@IDApplication", SqlDbType.Int).Value = SecurityObjectID
                    MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = ServerGroupID
                End If
                MyReader = MyCmd.ExecuteReader()
                While MyReader.Read
                    Dim MyServerGroup As Integer
                    If MyReader.GetSchemaTable.Columns.Contains("ID_ServerGroup") Then
                        MyServerGroup = System_Nz(MyReader("ID_ServerGroup"))
                    Else
                        MyServerGroup = Nothing
                    End If
                    _AuthorizedGroups.Add(New GroupAuthorizationInformation(_WebManager, _
                        MyReader("ID"), _
                        MyReader("ID_Application"), _
                        MyReader("ID_GroupOrPerson"), _
                        MyServerGroup))
                End While
                MyReader.Close()

            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Quick loads
            LoadUserAndGroupInformations()
        End Sub
        Private Sub LoadUserAndGroupInformations()

            'Use quick load mechanisms for each group information object
            If Me._AuthorizedGroups.Count > 0 Then
                Dim NeededGroupIDs As New ArrayList
                For Each MyGroupAuthInfo As GroupAuthorizationInformation In Me._AuthorizedGroups
                    if not neededgroupids.contains(MyGroupAuthInfo.GroupID) then
                        NeededGroupIDs.Add(MyGroupAuthInfo.GroupID)
                    endif
                Next
                Dim MyGroupInfos As GroupInformation() = _WebManager.System_GetGroupInfos(NeededGroupIDs)
                If Not MyGroupInfos Is Nothing Then
                    For Each MyGroupInfo As GroupInformation In MyGroupInfos
                        For Each MyGroupAuthInfo As GroupAuthorizationInformation In _AuthorizedGroups
                            If MyGroupInfo.ID = MyGroupAuthInfo.GroupID Then
                                MyGroupAuthInfo.Friend_GroupInfo = MyGroupInfo
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If

            'Use quick load mechanisms for each user information object
            If Me._AuthorizedUsers.Count > 0 Then
                Dim NeededUserIDs As New ArrayList
                For Each MyUserAuthInfo As UserAuthorizationInformation In Me._AuthorizedUsers
                    NeededUserIDs.Add(MyUserAuthInfo.UserID)
                Next
                Dim MyUserInfos As UserInformation() = _WebManager.System_GetUserInfos(NeededUserIDs)
                If Not MyUserInfos Is Nothing Then
                    For Each MyUserInfo As UserInformation In MyUserInfos
                        For Each MyUserAuthInfo As UserAuthorizationInformation In _AuthorizedUsers
                            If MyUserInfo.ID = MyUserAuthInfo.UserID Then
                                MyUserAuthInfo.Friend_UserInfo = MyUserInfo
                                Exit For
                            End If
                        Next
                    Next
                End If
            End If

        End Sub
        Private Sub ReloadData()
            Dim MyReloadedData As New CompuMaster.camm.WebManager.WMSystem.Authorizations(_SecurityObjectID, _WebManager, _ServerGroupID)
            Me._AuthorizedUsers = MyReloadedData.GetUserAuthorizationInformations
            Me._AuthorizedGroups = MyReloadedData.GetGroupAuthorizationInformations
            LoadUserAndGroupInformations()
        End Sub

        Public ReadOnly Property GroupAuthorizationInformations(Optional ByVal GroupID As Integer = Nothing) As GroupAuthorizationInformation()
            Get
                'Check if data has changed
                If _ReloadData = True Then
                    ReloadData()
                    _AuthorizedGroupInfos = Nothing
                End If

                If _AuthorizedGroups.Count = 0 Then
                    Return Nothing
                ElseIf GroupID = Nothing Then
                    'Do the normal job
                    If _AuthorizedGroupInfos Is Nothing Then
                        ReDim _AuthorizedGroupInfos(_AuthorizedGroups.Count - 1)
                        For MyCounter As Integer = 0 To _AuthorizedGroups.Count - 1
                            _AuthorizedGroupInfos(MyCounter) = _AuthorizedGroups(MyCounter + 1)
                        Next
                        Return _AuthorizedGroupInfos
                    Else
                        Return _AuthorizedGroupInfos
                    End If
                Else
                    'only return those results which matches the given user id
                    Dim MyAuthorizedGroups As New Collection
                    For Each MyAuthorizedGroupInfo As Authorizations.GroupAuthorizationInformation In _AuthorizedGroups
                        If MyAuthorizedGroupInfo.GroupID = GroupID Then
                            MyAuthorizedGroups.Add(MyAuthorizedGroupInfo)
                        End If
                    Next
                    If MyAuthorizedGroups.Count = 0 Then
                        Return Nothing
                    Else
                        Dim MyAuthorizedGroupInfos As Authorizations.GroupAuthorizationInformation()
                        ReDim MyAuthorizedGroupInfos(MyAuthorizedGroups.Count - 1)
                        For MyCounter As Integer = 0 To MyAuthorizedGroups.Count - 1
                            MyAuthorizedGroupInfos(MyCounter) = MyAuthorizedGroups(MyCounter + 1)
                        Next
                        Return MyAuthorizedGroupInfos
                    End If
                End If
            End Get
        End Property
        Public ReadOnly Property GroupInformation(ByVal GroupID As Integer) As GroupAuthorizationInformation
            Get
                'Check if data has changed
                If _ReloadData = True Then
                    ReloadData()
                End If

                'Do the normal job
                For MyCounter As Integer = 0 To _AuthorizedGroups.Count - 1
                    If CType(_AuthorizedGroups(MyCounter), GroupInformation).ID = GroupID Then
                        Return _AuthorizedGroups(MyCounter)
                    End If
                Next
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property UserAuthorizationInformations(Optional ByVal UserID As Integer = Nothing) As UserAuthorizationInformation()
            Get
                Return UserAuthorizationInformations(CLng(UserID))
            End Get
        End Property
        Public ReadOnly Property UserAuthorizationInformations(ByVal UserID As Long) As UserAuthorizationInformation()
            Get
                'Check if data has changed
                If _ReloadData = True Then
                    ReloadData()
                    _AuthorizedUserInfos = Nothing
                End If

                If _AuthorizedUsers.Count = 0 Then
                    Return Nothing
                ElseIf UserID = Nothing Then
                    'Do the normal job
                    If _AuthorizedUserInfos Is Nothing Then
                        ReDim _AuthorizedUserInfos(_AuthorizedUsers.Count - 1)
                        For MyCounter As Integer = 0 To _AuthorizedUsers.Count - 1
                            _AuthorizedUserInfos(MyCounter) = _AuthorizedUsers(MyCounter + 1)
                        Next
                        Return _AuthorizedUserInfos
                    Else
                        Return _AuthorizedUserInfos
                    End If
                Else
                    'only return those results which matches the given user id
                    Dim MyAuthorizedUsers As New Collection
                    For Each MyAuthorizedUserInfo As Authorizations.UserAuthorizationInformation In _AuthorizedUsers
                        If MyAuthorizedUserInfo.UserID = UserID Then
                            MyAuthorizedUsers.Add(MyAuthorizedUserInfo)
                        End If
                    Next
                    If MyAuthorizedUsers.Count = 0 Then
                        Return Nothing
                    Else
                        Dim MyAuthorizedUserInfos As Authorizations.UserAuthorizationInformation()
                        ReDim MyAuthorizedUserInfos(MyAuthorizedUsers.Count - 1)
                        For MyCounter As Integer = 0 To MyAuthorizedUsers.Count - 1
                            MyAuthorizedUserInfos(MyCounter) = MyAuthorizedUsers(MyCounter + 1)
                        Next
                        Return MyAuthorizedUserInfos
                    End If
                End If
            End Get
        End Property
        <Obsolete()> Public ReadOnly Property UserInformation(ByVal UserID As Integer) As UserInformation
            Get
                Return UserInformation(CLng(UserID))
            End Get
        End Property
        Public ReadOnly Property UserInformation(ByVal UserID As Long) As UserInformation
            Get
                'Check if data has changed
                If _ReloadData = True Then
                    ReloadData()
                End If

                'Do the normal job
                For MyCounter As Integer = 0 To _AuthorizedUsers.Count - 1
                    If CType(_AuthorizedUsers(MyCounter), UserInformation).ID = UserID Then
                        Return _AuthorizedUsers(MyCounter)
                    End If
                Next
                Return Nothing
            End Get
        End Property

        Protected Function GetGroupAuthorizationInformations() As Collection
            'Check if data has changed
            If _ReloadData = True Then
                ReloadData()
            End If

            'Do the normal job
            Return Me._AuthorizedGroups
        End Function
        Protected Function GetUserAuthorizationInformations() As Collection
            'Check if data has changed
            If _ReloadData = True Then
                ReloadData()
            End If

            'Do the normal job
            Return Me._AuthorizedUsers
        End Function

        Public Sub AddGroupAuthorization(ByVal GroupID As Integer, ByVal ServerGroupID As Integer, Optional ByVal SecurityObjectID As Integer = Nothing)

            'Welche SecurityObjectID?
            Dim MySecurityObjectID As Integer
            If SecurityObjectID <> Nothing Then
                MySecurityObjectID = SecurityObjectID
            Else
                MySecurityObjectID = _SecurityObjectID
            End If

            'Alle Vorbedingungen erfüllt?
            If MySecurityObjectID = Nothing Then
                Throw New Exception("Parameter 'SecurityObjectID' required")
            ElseIf ServerGroupID <> Nothing AndAlso (_DBVersion.Major < 4 OrElse (_DBVersion.Major = 4 AndAlso _DBVersion.Minor < 10)) Then
                Throw New Exception("Parameter 'ServerGroupID' not supported by the currently used database version")
            End If

            'Fill the list of authorized users
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = MyConn
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND ID_ServerGroup = @IDServerGroup" & vbNewLine & _
                                    "INSERT INTO [dbo].[ApplicationsRightsByGroup] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn, ID_ServerGroup) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate(), @IDServerGroup)"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = IIf(ServerGroupID = Nothing, DBNull.Value, ServerGroupID)
                MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
            Else
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson" & vbNewLine & _
                                    "INSERT INTO [dbo].[ApplicationsRightsByGroup] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate())"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
            End If

            Try
                MyConn.Open()
                MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Internes Objekt aktualisieren
            If MySecurityObjectID = _SecurityObjectID Then
                'internes Memory-Objekt muss ebenfalls aktualisiert werden
                _ReloadData = True
            End If

        End Sub
        Public Sub AddUserAuthorization(ByRef UserInfo As UserInformation, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)

            'mycmd.CommandText = "SELECT * FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDApplication AND ID_ServerGroup = @IDServerGroup"
            'Welches SecurityObjectID?
            Dim MySecurityObjectID As Integer
            If SecurityObjectID <> Nothing Then
                MySecurityObjectID = SecurityObjectID
            Else
                MySecurityObjectID = _SecurityObjectID
            End If

            'Alle Vorbedingungen erfüllt?
            If MySecurityObjectID = Nothing Then
                Throw New Exception("Parameter 'SecurityObjectID' required")
            ElseIf ServerGroupID <> Nothing AndAlso (_DBVersion.Major < 4 OrElse (_DBVersion.Major = 4 AndAlso _DBVersion.Minor < 10)) Then
                Throw New Exception("Parameter 'ServerGroupID' not supported by the currently used database version")
            End If

            'Fill the list of authorized users
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = MyConn
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND ID_ServerGroup = @IDServerGroup AND DevelopmentTeamMember = @DevelopmentTeamMember" & vbNewLine & _
                                    "INSERT INTO [dbo].[ApplicationsRightsByUser] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn, ID_ServerGroup, DevelopmentTeamMember) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate(), @IDServerGroup, @DevelopmentTeamMember)"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = IIf(ServerGroupID = Nothing, DBNull.Value, ServerGroupID)
                MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = UserInfo.ID
                MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
            Else
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND DevelopmentTeamMember = @DevelopmentTeamMember" & vbNewLine & _
                                    "INSERT INTO [dbo].[ApplicationsRightsByUser] (ID_Application, ID_GroupOrPerson, ReleasedBy, ReleasedOn, DevelopmentTeamMember) VALUES (@IDSecurityObject, @IDGroupOrPerson, @IDCurUser, GetDate(), @DevelopmentTeamMember)"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = UserInfo.ID
                MyCmd.Parameters.Add("@IDCurUser", SqlDbType.Int).Value = _WebManager.CurrentUserID(SpecialUsers.User_Code)
                MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
            End If

            Try
                MyConn.Open()
                MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            If UserInfo.AccountAuthorizationsAlreadySet = False Then
                'send e-mail when first authorizations have been set up
                UserInfo.AccountAuthorizationsAlreadySet = True
                'Check wether InitAuthorizationsDone flag has been set
                If _WebManager.System_SetUserDetail(UserInfo.IDLong, "InitAuthorizationsDone", "1", True) Then
                    Try
                        If Notifications Is Nothing Then
                            _WebManager.DefaultNotifications.NotificationForUser_AuthorizationsSet(UserInfo)
                        Else
                            Notifications.NotificationForUser_AuthorizationsSet(UserInfo)
                        End If
                    Catch
                    End Try
                End If
            End If

            'Internes Objekt aktualisieren
            If MySecurityObjectID = _SecurityObjectID Then
                'internes Memory-Objekt muss ebenfalls aktualisiert werden
                _ReloadData = True
            End If

        End Sub
        <Obsolete()> Public Sub AddUserAuthorization(ByVal UserID As Integer, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)
            AddUserAuthorization(New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(UserID), _WebManager), ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled, SecurityObjectID, Notifications)
        End Sub
        Public Sub AddUserAuthorization(ByVal UserID As Long, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False, Optional ByVal SecurityObjectID As Integer = Nothing, Optional ByVal Notifications As WMNotifications = Nothing)
            AddUserAuthorization(New CompuMaster.camm.WebManager.WMSystem.UserInformation(UserID, _WebManager), ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled, SecurityObjectID, Notifications)
        End Sub
        ' TODO: change to sub
        Public Function RemoveGroupAuthorization(ByVal GroupID As Integer, ByVal ServerGroupID As Integer)

            'Fill the list of authorized users
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = MyConn
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND ID_ServerGroup = @IDServerGroup"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
                MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = IIf(ServerGroupID = Nothing, DBNull.Value, ServerGroupID)
            Else
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = GroupID
            End If

            Dim Result As Integer
            Try
                MyConn.Open()
                Result = MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Internes Objekt aktualisieren
            _ReloadData = True

            Return Result
        End Function
        ' TODO: change to sub
        Public Function RemoveGroupAuthorization(ByVal AuthorizationID As Integer)

            'Fill the list of authorized users
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = MyConn
            MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByGroup] WHERE ID = @ID"
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = AuthorizationID

            Dim Result As Integer
            Try
                MyConn.Open()
                Result = MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Internes Objekt aktualisieren
            _ReloadData = True

            Return Result
        End Function
        ' TODO: change to sub
        <Obsolete()> Public Function RemoveUserAuthorization(ByVal UserID As Integer, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False)
            Return RemoveUserAuthorization(CLng(UserID), ServerGroupID, AlsoVisibleWhileSecurityObjectIsDisabled)
        End Function
        ' TODO: change to sub
        Public Function RemoveUserAuthorization(ByVal UserID As Long, ByVal ServerGroupID As Integer, Optional ByVal AlsoVisibleWhileSecurityObjectIsDisabled As Boolean = False)

            'Fill the list of authorized users
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = MyConn
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND ID_ServerGroup = @IDServerGroup AND DevelopmentTeamMember = @DevelopmentTeamMember"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = UserID
                MyCmd.Parameters.Add("@IDServerGroup", SqlDbType.Int).Value = IIf(ServerGroupID = Nothing, DBNull.Value, ServerGroupID)
                MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
            Else
                MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID_Application = @IDSecurityObject AND ID_GroupOrPerson = @IDGroupOrPerson AND DevelopmentTeamMember = @DevelopmentTeamMember"
                MyCmd.Parameters.Add("@IDSecurityObject", SqlDbType.Int).Value = _SecurityObjectID
                MyCmd.Parameters.Add("@IDGroupOrPerson", SqlDbType.Int).Value = UserID
                MyCmd.Parameters.Add("@DevelopmentTeamMember", SqlDbType.Bit).Value = AlsoVisibleWhileSecurityObjectIsDisabled
            End If

            Dim Result As Integer
            Try
                MyConn.Open()
                Result = MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Internes Objekt aktualisieren
            _ReloadData = True

            Return Result

        End Function
        ' TODO: change to sub
        Public Function RemoveUserAuthorization(ByVal AuthorizationID As Integer)

            'Fill the list of authorized users
            Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = MyConn
            MyCmd.CommandText = "DELETE FROM [dbo].[ApplicationsRightsByUser] WHERE ID = @ID"
            MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = AuthorizationID

            Dim Result As Integer
            Try
                MyConn.Open()
                Result = MyCmd.ExecuteNonQuery()
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            'Internes Objekt aktualisieren
            _ReloadData = True

            Return Result

        End Function
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMSystem.SecurityObjectInformation
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Security object information
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SecurityObjectInformation
        Dim _WebManager As WMSystem
        Dim _ID As Integer
        Dim _Name As String
        Dim _DisplayName As String
        Dim _SystemType As Integer
        Dim _Disabled As Boolean
        Dim _Deleted As Boolean
        Dim _InheritFrom_SecurityObjectID As Integer
        Dim _InheritFrom_SecurityObjectInfo As SecurityObjectInformation
        Dim _ModifiedBy_UserID As Long
        Dim _ModifiedBy_UserInfo As UserInformation
        Dim _ModifiedOn As DateTime
        Dim _ReleasedBy_UserID As Long
        Dim _ReleasedBy_UserInfo As UserInformation
        Dim _ReleasedOn As DateTime
        Dim _Remarks As String
        Dim _DBVersion As Version

        Public Sub New(ByVal SecurityObjectID As Integer, ByRef WebManager As WMSystem, Optional ByVal AlsoSearchForDeletedSecurityObjects As Boolean = False)
            _WebManager = WebManager

            'Environment check
            If SecurityObjectID = Nothing Then
                Throw New Exception("Empty parameter SecurityObjectID currently not supported")
            End If
            _DBVersion = _WebManager.System_DBVersion_Ex
            If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                Throw New Exception("Support for database version " & _DBVersion.ToString & " is currently not supported. Please update the camm WebManager software, first!")
            End If

            'Get the security object
            Dim MyConn As New SqlConnection(WebManager.ConnectionString)
            Dim MyCmd As SqlCommand
            Dim MyReader As SqlDataReader

            Try
                MyConn.Open()

                MyCmd = New SqlCommand("SELECT * FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE ID = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = SecurityObjectID
                MyReader = MyCmd.ExecuteReader()
                If MyReader.Read AndAlso (AlsoSearchForDeletedSecurityObjects = True OrElse System_Nz(MyReader("AppDeleted")) = False) Then
                    _ID = System_Nz(MyReader("ID"))
                    _Deleted = System_Nz(MyReader("AppDeleted"))
                    _Disabled = System_Nz(MyReader("AppDisabled"))
                    _DisplayName = System_Nz(MyReader("TitleAdminArea"))
                    _InheritFrom_SecurityObjectID = System_Nz(MyReader("AuthsAsAppID"))
                    _ModifiedBy_UserID = System_Nz(MyReader("ModifiedBy"))
                    _ModifiedOn = System_Nz(MyReader("ModifiedOn"))
                    _Name = System_Nz(MyReader("Title"))
                    _ReleasedBy_UserID = System_Nz(MyReader("ReleasedBy"))
                    _ReleasedOn = System_Nz(MyReader("ReleasedOn"))
                    _Remarks = System_Nz(MyReader("Remarks"))
                    _SystemType = System_Nz(MyReader("SystemAppType"))
                Else
                    _WebManager.Log.RuntimeWarning("Security object ID " & SecurityObjectID & " cannot be found", DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails)
                    Throw New Exception("Security object ID " & SecurityObjectID & " cannot be found")
                End If

            Finally
                If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                    MyReader.Close()
                End If
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyConn Is Nothing Then
                    If MyConn.State <> ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

        End Sub
        Friend Sub New(ByVal SecurityObjectID As Integer, ByVal Name As String, ByVal DisplayName As String, ByVal Remarks As String, ByVal ModifiedByUserID As Long, ByVal ModifiedOn As DateTime, ByVal ReleasedByUserID As Long, ByVal ReleasedOn As DateTime, ByVal Disabled As Boolean, ByVal Deleted As Boolean, ByVal InheritedFromSecurityObjectID As Integer, ByVal SystemType As Integer, ByRef WebManager As WMSystem)
            _WebManager = WebManager
            _ID = SecurityObjectID
            _Deleted = Deleted
            _Disabled = Disabled
            _DisplayName = DisplayName
            _InheritFrom_SecurityObjectID = InheritedFromSecurityObjectID
            _ModifiedBy_UserID = ModifiedByUserID
            _ModifiedOn = ModifiedOn
            _Name = Name
            _ReleasedBy_UserID = ReleasedByUserID
            _ReleasedOn = ReleasedOn
            _Remarks = Remarks
            _SystemType = SystemType
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ID value for this security object
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ID() As Integer
            Get
                Return _ID
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The name of this security object
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A display title for this security object in the administration forms
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property DisplayName() As String
            Get
                If _DisplayName = "" Then
                    Return _Name
                Else
                    Return _DisplayName
                End If
            End Get
            Set(ByVal Value As String)
                If Value = _Name Then
                    'Set it to nothing to keep it the same as the Name value
                    _DisplayName = Nothing
                Else
                    _DisplayName = Value
                End If
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A system type value, not set in standard scenarios
        ''' </summary>
        ''' <value>Nothing for normal items, 1 for master server items, 2 for administration server items, negative values for custom values</value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SystemType() As Integer
            Get
                Return _SystemType
            End Get
            Set(ByVal Value As Integer)
                _SystemType = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is this an inactive security object?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Disabled() As Boolean
            Get
                Return _Disabled
            End Get
            Set(ByVal Value As Boolean)
                _Disabled = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Has this security object been deleted?
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Deleted() As Boolean
            Get
                Return _Deleted
            End Get
            Set(ByVal Value As Boolean)
                _Deleted = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Authorizations are inherited by another security object
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property InheritFrom_SecurityObjectID() As Integer
            Get
                Return _InheritFrom_SecurityObjectID
            End Get
            Set(ByVal Value As Integer)
                _InheritFrom_SecurityObjectID = Value
                _InheritFrom_SecurityObjectInfo = Nothing
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Authorizations are inherited by another security object
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property InheritFrom_SecurityObjectInfo() As SecurityObjectInformation
            Get
                If _InheritFrom_SecurityObjectInfo Is Nothing Then
                    _InheritFrom_SecurityObjectInfo = New SecurityObjectInformation(_ID, _WebManager)
                End If
                Return _InheritFrom_SecurityObjectInfo
            End Get
            Set(ByVal Value As SecurityObjectInformation)
                _InheritFrom_SecurityObjectInfo = InheritFrom_SecurityObjectInfo
                _InheritFrom_SecurityObjectID = _InheritFrom_SecurityObjectInfo.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Last modification by this user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModifiedBy_UserID() As Integer
            Get
                Return _ModifiedBy_UserID
            End Get
            Set(ByVal Value As Integer)
                _ModifiedBy_UserID = Value
                _ModifiedBy_UserInfo = Nothing
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Last modification by this user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModifiedBy_UserInfo() As UserInformation
            Get
                If _ModifiedBy_UserInfo Is Nothing Then
                    _ModifiedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ModifiedBy_UserID, _WebManager)
                End If
                Return _ModifiedBy_UserInfo
            End Get
            Set(ByVal Value As UserInformation)
                _ModifiedBy_UserInfo = Value
                _ModifiedBy_UserID = _ModifiedBy_UserInfo.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The date and time of the last modification
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModifiedOn() As DateTime
            Get
                Return _ModifiedOn
            End Get
            Set(ByVal Value As DateTime)
                _ModifiedOn = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The release has been done by this user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ReleasedBy_UserID() As Integer
            Get
                Return _ReleasedBy_UserID
            End Get
            Set(ByVal Value As Integer)
                _ReleasedBy_UserID = Value
                _ReleasedBy_UserInfo = Nothing
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The release has been done by this user
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ReleasedBy_UserInfo() As UserInformation
            Get
                If _ReleasedBy_UserInfo Is Nothing Then
                    _ReleasedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ReleasedBy_UserID, _WebManager)
                End If
                Return _ReleasedBy_UserInfo
            End Get
            Set(ByVal Value As UserInformation)
                _ReleasedBy_UserInfo = Value
                _ReleasedBy_UserID = _ReleasedBy_UserInfo.ID
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The release has been done on this date/time
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ReleasedOn() As DateTime
            Get
                Return _ReleasedOn
            End Get
            Set(ByVal Value As DateTime)
                _ReleasedOn = Value
            End Set
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Comments to this security object
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Remarks() As String
            Get
                Return _Remarks
            End Get
            Set(ByVal Value As String)
                _Remarks = Value
            End Set
        End Property
    End Class

#End If
#If NotYetImplemented Then
            Public Class NavItems

            End Class
#End If
#End Region

#Region "BuggyInheritingClassesInWMSystem"
#If Buggy Then

            ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.ServerGroupInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Server group information
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class ServerGroupInformation
            Inherits CompuMaster.camm.WebManager.ServerGroupInformation

            Friend Sub New(ByVal ServerGroupID As Integer, ByVal Title As String, ByVal NavTitle As String, ByVal OfficialCompanyWebSiteTitle As String, ByVal OfficialCompanyWebSiteURL As String, ByVal CompanyTitle As String, ByVal CompanyFormerTitle As String, ByVal AccessLevelDefaultID As Integer, ByVal MasterServerID As Integer, ByVal AdminServerID As Integer, ByVal GroupAnonymousID As Integer, ByVal GroupPublicID As Integer, ByRef WebManager As WMSystem)
                MyBase.new(ServerGroupID, Title, NavTitle, OfficialCompanyWebSiteTitle, OfficialCompanyWebSiteURL, CompanyTitle, CompanyFormerTitle, AccessLevelDefaultID, MasterServerID, AdminServerID, GroupAnonymousID, GroupPublicID, WebManager)
            End Sub
            Public Sub New(ByVal ServerGroupID As Integer, ByRef WebManager As WMSystem)
                MyBase.New(ServerGroupID, WebManager)
            End Sub
        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.ServerInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Server information
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class ServerInformation
            Inherits CompuMaster.camm.WebManager.ServerInformation

            Friend Sub New(ByVal ServerID As Integer, ByVal IP_Or_HostHeader As String, ByVal Description As String, ByVal URL_Protocol As String, ByVal URL_DomainName As String, ByVal URL_Port As String, ByVal Enabled As Boolean, ByVal ParentServerGroupID As Integer, ByRef WebManager As WMSystem, Optional ByVal ServerSessionTimeout As Integer = 15, Optional ByVal ServerUserlockingsTimeout As Integer = 3)
                MyBase.New(ServerID, IP_Or_HostHeader, Description, URL_Protocol, URL_DomainName, URL_Port, Enabled, ParentServerGroupID, WebManager)
            End Sub
            Public Sub New(ByVal ServerID As Integer, ByRef WebManager As WMSystem)
                MyBase.New(ServerID, WebManager)
            End Sub
            Public Sub New(ByVal ServerIP As String, ByRef WebManager As WMSystem)
                MyBase.New(ServerIP, WebManager)
            End Sub

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.Authorizations
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Authorizations
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class Authorizations
            Inherits CompuMaster.camm.WebManager.Authorizations

            Public Sub New(ByVal SecurityObjectID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal ServerGroupID As Integer = Nothing)
                MyBase.New(SecurityObjectID, WebManager, ServerGroupID)
            End Sub

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.SecurityObjectInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Security object information
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class SecurityObjectInformation
            Inherits CompuMaster.camm.WebManager.SecurityObjectInformation

            Public Sub New(ByVal SecurityObjectID As Integer, ByRef WebManager As WMSystem, Optional ByVal AlsoSearchForDeletedSecurityObjects As Boolean = False)
                MyBase.New(SecurityObjectID, WebManager, AlsoSearchForDeletedSecurityObjects)
            End Sub
            Friend Sub New(ByVal SecurityObjectID As Integer, ByVal Name As String, ByVal DisplayName As String, ByVal Remarks As String, ByVal ModifiedByUserID As Long, ByVal ModifiedOn As DateTime, ByVal ReleasedByUserID As Long, ByVal ReleasedOn As DateTime, ByVal Disabled As Boolean, ByVal Deleted As Boolean, ByVal InheritedFromSecurityObjectID As Integer, ByVal SystemType As Integer, ByRef WebManager As WMSystem)
                MyBase.New(SecurityObjectID, Name, DisplayName, Remarks, ModifiedByUserID, ModifiedOn, ReleasedByUserID, ReleasedOn, Disabled, Deleted, InheritedFromSecurityObjectID, SystemType, WebManager)
            End Sub

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.UserInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     User information
        ''' </summary>
        ''' <remarks>
        '''     This class contains all information of a user profile as well as all important methods for handling of that account
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class UserInformation
            Inherits CompuMaster.camm.WebManager.UserInformation

            ''' -----------------------------------------------------------------------------
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
            ''' <param name="AccessLevelID">The access level ID of the user</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="ExternalAccount">An external account relation for single-sign-on purposes</param>
            ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	07.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <Obsolete()> Public Sub New(ByVal UserID As Integer, ByVal LoginName As String, ByVal EMailAddress As String, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, ByVal ExternalAccount As String, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
                MyBase.New(CLng(UserID), LoginName, EMailAddress, False, Company, Sex, NameAddition, FirstName, LastName, AcademicTitle, Street, ZipCode, City, State, Country, PreferredLanguage1ID, PreferredLanguage2ID, PreferredLanguage3ID, LoginDisabled, LoginLockedTemporary, LoginDeleted, AccessLevelID, WebManager, ExternalAccount, AdditionalFlags)
            End Sub

            ''' -----------------------------------------------------------------------------
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
            ''' <param name="AccessLevelID">The access level ID of the user</param>
            ''' <param name="__reserved">Obsolete parameter</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	07.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <Obsolete("use overloaded method instead")> Public Sub New(ByVal UserID As Integer, ByVal LoginName As String, ByVal EMailAddress As String, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByVal __reserved As Boolean, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
                Me.New(CLng(UserID), LoginName, EMailAddress, False, Company, Sex, NameAddition, FirstName, LastName, AcademicTitle, Street, ZipCode, City, State, Country, PreferredLanguage1ID, PreferredLanguage2ID, PreferredLanguage3ID, LoginDisabled, LoginLockedTemporary, LoginDeleted, AccessLevelID, WebManager, Nothing, AdditionalFlags)
            End Sub
            ''' -----------------------------------------------------------------------------
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
            ''' <history>
            ''' 	[adminwezel]	07.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <Obsolete()> Public Sub New(ByVal UserID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
                MyBase.New(CLng(UserID), WebManager, SearchForDeletedAccountsAsWell)
            End Sub
            ''' -----------------------------------------------------------------------------
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
            ''' <param name="AccessLevelID">The access level ID of the user</param>
            ''' <param name="WebManager">The current instance of camm Web-Manager</param>
            ''' <param name="ExternalAccount">An external account relation for single-sign-on purposes</param>
            ''' <param name="AdditionalFlags">A collection of additional flags which are saved in the user's profile</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminwezel]	07.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal UserID As Long, ByVal LoginName As String, ByVal EMailAddress As String, ByVal AutomaticLogonAllowedByMachineToMachineCommunication As Boolean, ByVal Company As String, ByVal Sex As Sex, ByVal NameAddition As String, ByVal FirstName As String, ByVal LastName As String, ByVal AcademicTitle As String, ByVal Street As String, ByVal ZipCode As String, ByVal City As String, ByVal State As String, ByVal Country As String, ByVal PreferredLanguage1ID As Integer, ByVal PreferredLanguage2ID As Integer, ByVal PreferredLanguage3ID As Integer, ByVal LoginDisabled As Boolean, ByVal LoginLockedTemporary As Boolean, ByVal LoginDeleted As Boolean, ByVal AccessLevelID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, ByVal ExternalAccount As String, Optional ByVal AdditionalFlags As Collections.Specialized.NameValueCollection = Nothing)
                MyBase.New(UserID, LoginName, EMailAddress, AutomaticLogonAllowedByMachineToMachineCommunication, Company, Sex, NameAddition, FirstName, LastName, AcademicTitle, Street, ZipCode, City, State, Country, PreferredLanguage1ID, PreferredLanguage2ID, PreferredLanguage3ID, LoginDisabled, LoginLockedTemporary, LoginDeleted, AccessLevelID, WebManager, ExternalAccount, AdditionalFlags)
            End Sub
            ''' -----------------------------------------------------------------------------
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
            ''' <history>
            ''' 	[adminwezel]	07.07.2004	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal UserID As Long, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem, Optional ByVal SearchForDeletedAccountsAsWell As Boolean = False)
                MyBase.New(UserID, WebManager, SearchForDeletedAccountsAsWell)
            End Sub

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.LanguageInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Language details
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class LanguageInformation
            Inherits CompuMaster.camm.WebManager.LanguageInformation

            Friend Sub New(ByVal ID As Integer, ByRef LanguageName_English As String, ByVal LanguageName_OwnLanguage As String, ByVal IsActive As Boolean, ByVal BrowserLanguageID As String, ByVal Abbreviation As String, ByRef WebManager As WMSystem)
                MyBase.New(ID, LanguageName_English, LanguageName_OwnLanguage, IsActive, BrowserLanguageID, Abbreviation, WebManager)
            End Sub
            Public Sub New(ByVal ID As Integer, ByRef WebManager As WMSystem)
                MyBase.New(ID, WebManager)
            End Sub
        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.AccessLevelInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Access level information
        ''' </summary>
        ''' <remarks>
        '''     Access levels are user roles defining the availability of the existant server groups for the user
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class AccessLevelInformation
            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _Title As String
            Dim _Remarks As String
            Dim _ServerGroups As ServerGroupInformation()
            Dim _Users As UserInformation()
            Public Sub New(ByVal ID As Integer, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                Dim MyConn As New SqlConnection(WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("select * from system_accesslevels where id = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
                Dim MyReader As SqlDataReader
                Try
                    MyConn.Open()
                    MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    If MyReader.Read Then
                        _ID = MyReader("ID")
                        _Title = WebManager.System_Nz(MyReader("Title"))
                        _Remarks = _WebManager.System_Nz(MyReader("Remarks"))
                    End If
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyConn Is Nothing Then
                        If MyConn.State <> ConnectionState.Closed Then
                            MyConn.Close()
                        End If
                        MyConn.Dispose()
                    End If
                End Try
            End Sub
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The ID value for this access level role
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The title for this access level role
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Title() As String
                Get
                    Return _Title
                End Get
                Set(ByVal Value As String)
                    _Title = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Some optional remarks on this role
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Remarks() As String
                Get
                    Return _Remarks
                End Get
                Set(ByVal Value As String)
                    _Remarks = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     A list of server groups which are accessable by this role
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property ServerGroups() As ServerGroupInformation()
                Get
                    If _ServerGroups Is Nothing Then
                        _ServerGroups = _WebManager.System_GetServerGroupsInfo(_ID)
                    End If
                    Return _ServerGroups
                End Get
                Set(ByVal Value As ServerGroupInformation())
                    _ServerGroups = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     A list of users which are assigned to this role
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Users() As UserInformation()
                Get
                    If _Users Is Nothing Then
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("select * from benutzer where benutzer.AccountAccessability = @ID order by [1stPreferredLanguage]", MyConn)
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim MyDataSet As DataSet = New DataSet
                        Dim MyDA As SqlDataAdapter = New SqlDataAdapter(MyCmd)
                        Try
                            MyConn.Open()
                            MyDA.Fill(MyDataSet, "Users")
                        Finally
                            If Not MyDA Is Nothing Then
                                MyDA.Dispose()
                            End If
                            If Not MyCmd Is Nothing Then
                                MyCmd.Dispose()
                            End If
                            If Not MyConn Is Nothing Then
                                If MyConn.State <> ConnectionState.Closed Then
                                    MyConn.Close()
                                End If
                                MyConn.Dispose()
                            End If
                        End Try
                        If MyDataSet.Tables("Users").Rows.Count > 0 Then
                            ReDim Preserve _Users(MyDataSet.Tables("Users").Rows.Count - 1)
                            Dim MyCounter As Integer = 0
                            For Each MyDataRow As DataRow In MyDataSet.Tables("Users").Rows
                                _Users(MyCounter) = New CompuMaster.camm.WebManager.WMSystem.UserInformation(CLng(MyDataRow("ID")), _
                                                MyDataRow("LoginName"), _
                                                MyDataRow("E-Mail"), _
                                                False, _
                                                _WebManager.System_Nz(MyDataRow("Company")), _
                                                IIf(Convert.ToString(_WebManager.System_Nz(MyDataRow("Anrede"))) = "", Sex.Undefined, IIf(Convert.ToString(_WebManager.System_Nz(MyDataRow("Anrede"))) = "Mr.", Sex.Masculin, Sex.Feminin)), _
                                                _WebManager.System_Nz(MyDataRow("Namenszusatz")), _
                                                MyDataRow("Vorname"), _
                                                MyDataRow("Nachname"), _
                                                _WebManager.System_Nz(MyDataRow("Titel")), _
                                                _WebManager.System_Nz(MyDataRow("Strasse")), _
                                                _WebManager.System_Nz(MyDataRow("PLZ")), _
                                                _WebManager.System_Nz(MyDataRow("Ort")), _
                                                _WebManager.System_Nz(MyDataRow("State")), _
                                                _WebManager.System_Nz(MyDataRow("Land")), _
                                                MyDataRow("1stPreferredLanguage"), _
                                                _WebManager.System_Nz(MyDataRow("2ndPreferredLanguage"), Nothing), _
                                                _WebManager.System_Nz(MyDataRow("3rdPreferredLanguage"), Nothing), _
                                                MyDataRow("LoginDisabled"), _
                                                Not IsDBNull(MyDataRow("LoginLockedTill")), _
                                                False, _
                                                MyDataRow("AccountAccessability"), _
                                                _WebManager, _
                                                Nothing)
                                MyCounter += 1
                            Next
                        Else
                            _Users = Nothing
                        End If
                    End If
                    Return _Users
                End Get
                Set(ByVal Value As UserInformation())
                    _Users = Value
                End Set
            End Property
        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.WMSystem.GroupInformation
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Group information
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class GroupInformation
            Inherits CompuMaster.camm.WebManager.GroupInformation

            Friend Sub New(ByVal GroupID As Integer, ByVal InternalName As String, ByVal Description As String, ByVal IsSystemGroup As Boolean, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem)
                MyBase.New(GroupID, InternalName, Description, IsSystemGroup, WebManager)
            End Sub
            Public Sub New(ByVal GroupID As Integer, ByRef WebManager As CompuMaster.camm.WebManager.WMSystem)
                MyBase.New(GroupID, WebManager)
            End Sub
        End Class

#End If
#End Region

End Namespace