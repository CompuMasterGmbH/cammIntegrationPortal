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
        '''     Security object information
        ''' </summary>
        Public Class SecurityObjectInformation
            Implements ISecurityObjectInformation

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
            Dim _RequiredFlags As String
            Dim _RequiredFlagsRemarks As String

            'TODO: Property AdministrationPrivileges As AdministrationPrivilegesInformation

            Public Sub New(ByVal SecurityObjectID As Integer, ByRef WebManager As WMSystem, Optional ByVal AlsoSearchForDeletedSecurityObjects As Boolean = False)
                _WebManager = WebManager

                'Environment check
                If SecurityObjectID = Nothing Then
                    Throw New ArgumentNullException("Empty parameter SecurityObjectID currently not supported")
                End If
                _DBVersion = Setup.DatabaseUtils.Version(_WebManager, True)
                If _DBVersion.CompareTo(MilestoneDBVersion_ApplicationsDividedIntoNavItemsAndSecurityObjects) >= 0 Then 'Newer
                    Throw New NotImplementedException("Support for database version " & _DBVersion.ToString & " is currently not supported. Please update the camm WebManager software, first!")
                End If

                'Get the security object
                Dim MyConn As New SqlConnection(WebManager.ConnectionString)
                Dim MyCmd As SqlCommand = Nothing
                Dim MyReader As SqlDataReader = Nothing

                Try
                    MyConn.Open()

                    MyCmd = New SqlCommand("SELECT * FROM [dbo].[Applications_CurrentAndInactiveOnes] WHERE ID = @ID", MyConn)
                    MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = SecurityObjectID
                    MyReader = MyCmd.ExecuteReader()
                    If MyReader.Read AndAlso (AlsoSearchForDeletedSecurityObjects = True OrElse Utils.Nz(MyReader("AppDeleted"), False) = False) Then
                        _ID = Utils.Nz(MyReader("ID"), 0)
                        _Deleted = Utils.Nz(MyReader("AppDeleted"), False)
                        _Disabled = Utils.Nz(MyReader("AppDisabled"), False)
                        _DisplayName = Utils.Nz(MyReader("TitleAdminArea"), CType(Nothing, String))
                        _InheritFrom_SecurityObjectID = Utils.Nz(MyReader("AuthsAsAppID"), 0)
                        _ModifiedBy_UserID = Utils.Nz(MyReader("ModifiedBy"), 0&)
                        _ModifiedOn = Utils.Nz(MyReader("ModifiedOn"), CType(Nothing, Date))
                        _Name = Utils.Nz(MyReader("Title"), CType(Nothing, String))
                        _ReleasedBy_UserID = Utils.Nz(MyReader("ReleasedBy"), 0&)
                        _ReleasedOn = Utils.Nz(MyReader("ReleasedOn"), CType(Nothing, DateTime))
                        _Remarks = Utils.Nz(MyReader("Remarks"), CType(Nothing, String))
                        _SystemType = Utils.Nz(MyReader("SystemAppType"), 0)
                        _RequiredFlags = Utils.Nz(MyReader("RequiredUserProfileFlags"), CType(Nothing, String))
                        If Setup.DatabaseUtils.Version(WebManager, True).Build >= 185 Then
                            _RequiredFlagsRemarks = Utils.Nz(MyReader("RequiredUserProfileFlagsRemarks"), CType(Nothing, String))
                        End If
                        _NavigationItems = New Security.NavigationInformation() {New Security.NavigationInformation(
                            _ID,
                            Me,
                            Utils.Nz(MyReader("Level1Title"), String.Empty),
                            Utils.Nz(MyReader("Level2Title"), String.Empty),
                            Utils.Nz(MyReader("Level3Title"), String.Empty),
                            Utils.Nz(MyReader("Level4Title"), String.Empty),
                            Utils.Nz(MyReader("Level5Title"), String.Empty),
                            Utils.Nz(MyReader("Level6Title"), String.Empty),
                            Utils.Nz(MyReader("Level1TitleIsHtmlCoded"), False),
                            Utils.Nz(MyReader("Level2TitleIsHtmlCoded"), False),
                            Utils.Nz(MyReader("Level3TitleIsHtmlCoded"), False),
                            Utils.Nz(MyReader("Level4TitleIsHtmlCoded"), False),
                            Utils.Nz(MyReader("Level5TitleIsHtmlCoded"), False),
                            Utils.Nz(MyReader("Level6TitleIsHtmlCoded"), False),
                            Utils.Nz(MyReader("NavURL"), String.Empty),
                            Utils.Nz(MyReader("NavFrame"), String.Empty),
                            Utils.Nz(MyReader("NavTooltipText"), String.Empty),
                            Utils.Nz(MyReader("AddLanguageID2URL"), False),
                            Utils.Nz(MyReader("LanguageID"), 0),
                            Utils.Nz(MyReader("LocationID"), 0),
                            Utils.Nz(MyReader("Sort"), 0),
                            Utils.Nz(MyReader("IsNew"), False),
                            Utils.Nz(MyReader("IsUpdated"), False),
                            Utils.Nz(MyReader("ResetIsNewUpdatedStatusOn"), DateTime.MinValue),
                            Utils.Nz(MyReader("OnMouseOver"), String.Empty),
                            Utils.Nz(MyReader("OnMouseOut"), String.Empty),
                            Utils.Nz(MyReader("OnClick"), String.Empty))}
                    Else
                        'System.Environment.StackTrace doesn't work with medium-trust --> work around it using a new exception class
                        Dim WorkaroundEx As New Exception("")
                        Dim WorkaroundStackTrace As String = WorkaroundEx.StackTrace 'contains only last few lines of stacktrace
                        Try
                            WorkaroundStackTrace = System.Environment.StackTrace 'contains full stacktrace
                        Catch
                        End Try
                        _WebManager.Log.RuntimeWarning("Security object ID " & SecurityObjectID & " cannot be found", WorkaroundStackTrace, DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails, False, False)
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

            ''' <summary>
            ''' Create a SecurityObjectInformation instance
            ''' </summary>
            ''' <param name="securityObjectID">ID</param>
            ''' <param name="name">Name</param>
            ''' <param name="displayName">Title in administration area</param>
            ''' <param name="remarks">User's comment on this security object</param>
            ''' <param name="modifiedByUserID">Who modified this item last time</param>
            ''' <param name="modifiedOn">When has this item been modified</param>
            ''' <param name="releasedByUserID">Who released this item last time</param>
            ''' <param name="releasedOn">When has this item been released</param>
            ''' <param name="disabled">Is this item enabled (active) or disabled (only accessible with development authorizations)</param>
            ''' <param name="deleted">It this security object deleted</param>
            ''' <param name="inheritedFromSecurityObjectID">An ID of another security object whose authorizations apply also to this one</param>
            ''' <param name="systemType">A type value for system purposes as well as for custom purposes (0 for normal items, 1 for master server items, 2 for administration server items, negative values for custom values)</param>
            ''' <param name="requiredFlags">Comma-separated list of required flag names/definitions</param>
            ''' <param name="requiredFlagsRemarks">Remarks on required flags</param>
            ''' <param name="navigationItems">Navigation items related to this SecurityObject</param>
            ''' <param name="webManager">A reference to a cammWebManager instance</param>
            Friend Sub New(ByVal securityObjectID As Integer, ByVal name As String, ByVal displayName As String, ByVal remarks As String, ByVal modifiedByUserID As Long, ByVal modifiedOn As DateTime, ByVal releasedByUserID As Long, ByVal releasedOn As DateTime, ByVal disabled As Boolean, ByVal deleted As Boolean, ByVal inheritedFromSecurityObjectID As Integer, ByVal systemType As Integer, requiredFlags As String, requiredFlagsRemarks As String, navigationItems As Security.NavigationInformation, webManager As WMSystem)
                _WebManager = webManager
                _ID = securityObjectID
                _Deleted = deleted
                _Disabled = disabled
                _DisplayName = displayName
                _InheritFrom_SecurityObjectID = inheritedFromSecurityObjectID
                _ModifiedBy_UserID = modifiedByUserID
                _ModifiedOn = modifiedOn
                _Name = name
                _ReleasedBy_UserID = releasedByUserID
                _ReleasedOn = releasedOn
                _Remarks = remarks
                _SystemType = systemType
                _RequiredFlags = requiredFlags
                _RequiredFlagsRemarks = requiredFlagsRemarks
                If navigationItems.SecurityObjectID = 0 AndAlso navigationItems.SecurityObjectInfo Is Nothing Then
                    navigationItems.SetSecurityObjectInfoInternal(Me)
                Else
                    Throw New NotSupportedException("NavSplit feature not yet supported")
                End If
                _NavigationItems = New Security.NavigationInformation() {navigationItems}
            End Sub

            ''' <summary>
            '''     The ID value for this security object
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property

            <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Friend Sub SetIDInternal(value As Integer)
                _ID = value
            End Sub

            ''' <summary>
            '''     The name of this security object
            ''' </summary>
            ''' <value></value>
            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    If Utils.StringNotNothingOrEmpty(Value).Trim.ToLower = "public" OrElse Utils.StringNotNothingOrEmpty(Value).Trim.ToLower = "anonymous" Then
                        Throw New Exception("Invalid name for a security object: forbidden names are 'public' and 'anonymous'")
                    ElseIf Utils.StringNotNothingOrEmpty(Value).TrimStart.ToLower.StartsWith("@@") Then
                        Throw New Exception("Invalid name for a security object: name must not start with '@@'")
                    ElseIf Utils.StringNotNothingOrEmpty(Value).IndexOf(","c) >= 0 Then
                        Throw New Exception("Invalid name for a security object: name must not contain a comma (',')")
                    End If
                    _Name = Value
                End Set
            End Property

            ''' <summary>
            '''     A display title for this security object in the administration forms
            ''' </summary>
            ''' <value></value>
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

            ''' <summary>
            ''' Based on current authorization of this group and their additional flags requirements, every member user account must provide the requested flag data
            ''' </summary>
            ''' <returns>Array of strings representing required flag names (with type information)</returns>
            Public Function RequiredAdditionalFlags() As String()
                Return Me.RequiredFlags.Split(","c)
            End Function

            Friend Shared Function RequiredAdditionalFlags(secObjID As Integer, webManager As WMSystem) As String()
                Dim Sql As String = "SELECT RequiredUserProfileFlags FROM [dbo].[applications_currentandinactiveones] WHERE ID = @SecObjID"
                Dim command As New SqlCommand(Sql, New SqlConnection(webManager.ConnectionString))
                command.Parameters.Add("@SecObjID", SqlDbType.Int).Value = secObjID
                Dim Result As Object = Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(command, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                Return Utils.Nz(Result, "").Split(","c)
            End Function


            ''' <summary>
            ''' Comma-separated list of reqired flags/definitions
            ''' </summary>
            Public Property RequiredFlags() As String
                Get
                    Return _RequiredFlags
                End Get
                Set(ByVal Value As String)
                    _RequiredFlags = Value
                End Set
            End Property

            ''' <summary>
            ''' User comments on required flags
            ''' </summary>
            Public Property RequiredFlagsRemarks() As String
                Get
                    Return _RequiredFlagsRemarks
                End Get
                Set(ByVal Value As String)
                    _RequiredFlagsRemarks = Value
                End Set
            End Property

            ''' <summary>
            '''     A type value for system purposes as well as for custom purposes (0 for normal items, 1 for master server items, 2 for administration server items, negative values for custom values)
            ''' </summary>
            ''' <value>0 for normal items, 1 for master server items, 2 for administration server items, negative values for custom values</value>
            Public Property SystemType() As Integer
                Get
                    Return _SystemType
                End Get
                Set(ByVal Value As Integer)
                    _SystemType = Value
                End Set
            End Property

            ''' <summary>
            '''     Is this an inactive security object?
            ''' </summary>
            ''' <value></value>
            Public Property Disabled() As Boolean
                Get
                    Return _Disabled
                End Get
                Set(ByVal Value As Boolean)
                    _Disabled = Value
                End Set
            End Property

            ''' <summary>
            '''     Has this security object been deleted?
            ''' </summary>
            ''' <value></value>
            Public Property Deleted() As Boolean
                Get
                    Return _Deleted
                End Get
                Set(ByVal Value As Boolean)
                    _Deleted = Value
                End Set
            End Property

            ''' <summary>
            '''     Authorizations are inherited by another security object
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use InheritFrom_SecurityObjectIDs instead - property is subject to be dropped in future")> Public Property InheritFrom_SecurityObjectID() As Integer
                Get
                    Return _InheritFrom_SecurityObjectID
                End Get
                Set(ByVal Value As Integer)
                    _InheritFrom_SecurityObjectID = Value
                    _InheritFrom_SecurityObjectInfo = Nothing
                End Set
            End Property
            ''' <summary>
            ''' Authorizations are inherited by other security objects
            ''' </summary>
            Public Property InheritFrom_SecurityObjectIDs() As Integer()
                Get
                    If _InheritFrom_SecurityObjectID = Nothing Then
                        Return New Integer() {}
                    Else
                        Return New Integer() {_InheritFrom_SecurityObjectID}
                    End If
                End Get
                Set(ByVal Value As Integer())
                    If Value Is Nothing OrElse Value.Length = 0 Then
                        _InheritFrom_SecurityObjectID = Nothing
                    ElseIf Value.Length <> 1 Then
                        _InheritFrom_SecurityObjectID = Value(0)
                    Else
                        'Not yet done: support multiple security objects to inherit from
                        Throw New NotSupportedException("This version only supports 0 or 1 items")
                    End If
                    _InheritFrom_SecurityObjectInfo = Nothing
                End Set
            End Property

            ''' <summary>
            '''     Authorizations are inherited by another security object
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use InheritFrom_SecurityObjectInfos instead - property is subject to be dropped in future")> Public Property InheritFrom_SecurityObjectInfo() As SecurityObjectInformation
                Get
                    If _InheritFrom_SecurityObjectInfo Is Nothing AndAlso _InheritFrom_SecurityObjectID <> Nothing Then
                        _InheritFrom_SecurityObjectInfo = New SecurityObjectInformation(_InheritFrom_SecurityObjectID, _WebManager)
                    End If
                    Return _InheritFrom_SecurityObjectInfo
                End Get
                Set(ByVal Value As SecurityObjectInformation)
                    _InheritFrom_SecurityObjectInfo = InheritFrom_SecurityObjectInfo
                    _InheritFrom_SecurityObjectID = _InheritFrom_SecurityObjectInfo.ID
                End Set
            End Property
            ''' <summary>
            ''' Authorizations are inherited by other security objects
            ''' </summary>
            Public Property InheritFrom_SecurityObjectInfos() As SecurityObjectInformation()
                Get
                    If _InheritFrom_SecurityObjectInfo Is Nothing AndAlso _InheritFrom_SecurityObjectID <> Nothing Then
                        _InheritFrom_SecurityObjectInfo = New SecurityObjectInformation(_InheritFrom_SecurityObjectID, _WebManager)
                    End If
                    If _InheritFrom_SecurityObjectInfo Is Nothing Then
                        Return New SecurityObjectInformation() {}
                    Else
                        Return New SecurityObjectInformation() {_InheritFrom_SecurityObjectInfo}
                    End If
                End Get
                Set(ByVal Value As SecurityObjectInformation())
                    If Value Is Nothing OrElse Value.Length = 0 Then
                        _InheritFrom_SecurityObjectInfo = Nothing
                        _InheritFrom_SecurityObjectID = Nothing
                    ElseIf Value.Length <> 1 Then
                        _InheritFrom_SecurityObjectInfo = Value(0)
                        _InheritFrom_SecurityObjectID = _InheritFrom_SecurityObjectInfo.ID
                    Else
                        'Not yet done: support multiple security objects to inherit from
                        Throw New NotSupportedException("This version only supports 0 or 1 items")
                    End If
                End Set
            End Property

            ''' <summary>
            '''     Last modification by this user
            ''' </summary>
            ''' <value></value>
            Public Property ModifiedBy_UserID() As Long
                Get
                    Return CType(_ModifiedBy_UserID, Long)
                End Get
                Set(ByVal Value As Long)
                    _ModifiedBy_UserID = Value
                    _ModifiedBy_UserInfo = Nothing
                End Set
            End Property

            ''' <summary>
            '''     Last modification by this user
            ''' </summary>
            ''' <value></value>
            Public Property ModifiedBy_UserInfo() As UserInformation
                Get
                    If _ModifiedBy_UserInfo Is Nothing Then
                        _ModifiedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ModifiedBy_UserID, _WebManager, True)
                    End If
                    Return _ModifiedBy_UserInfo
                End Get
                Set(ByVal Value As UserInformation)
                    _ModifiedBy_UserInfo = Value
                    _ModifiedBy_UserID = _ModifiedBy_UserInfo.IDLong
                End Set
            End Property

            ''' <summary>
            '''     The date and time of the last modification
            ''' </summary>
            ''' <value></value>
            Public Property ModifiedOn() As DateTime
                Get
                    Return _ModifiedOn
                End Get
                Set(ByVal Value As DateTime)
                    _ModifiedOn = Value
                End Set
            End Property

            ''' <summary>
            '''     The release has been done by this user
            ''' </summary>
            ''' <value></value>
            Public Property ReleasedBy_UserID() As Long
                Get
                    Return CType(_ReleasedBy_UserID, Long)
                End Get
                Set(ByVal Value As Long)
                    _ReleasedBy_UserID = Value
                    _ReleasedBy_UserInfo = Nothing
                End Set
            End Property

            ''' <summary>
            '''     The release has been done by this user
            ''' </summary>
            ''' <value></value>
            Public Property ReleasedBy_UserInfo() As UserInformation
                Get
                    If _ReleasedBy_UserInfo Is Nothing Then
                        _ReleasedBy_UserInfo = New CompuMaster.camm.WebManager.WMSystem.UserInformation(_ReleasedBy_UserID, _WebManager, True)
                    End If
                    Return _ReleasedBy_UserInfo
                End Get
                Set(ByVal Value As UserInformation)
                    _ReleasedBy_UserInfo = Value
                    _ReleasedBy_UserID = _ReleasedBy_UserInfo.IDLong
                End Set
            End Property

            ''' <summary>
            '''     The release has been done on this date/time
            ''' </summary>
            ''' <value></value>
            Public Property ReleasedOn() As DateTime
                Get
                    Return _ReleasedOn
                End Get
                Set(ByVal Value As DateTime)
                    _ReleasedOn = Value
                End Set
            End Property

            ''' <summary>
            '''     Comments to this security object
            ''' </summary>
            ''' <value></value>
            Public Property Remarks() As String
                Get
                    Return _Remarks
                End Get
                Set(ByVal Value As String)
                    _Remarks = Value
                End Set
            End Property

            Private _NavigationItems As Security.NavigationInformation()
            Public Property NavigationItems As Security.NavigationInformation()
                Get
                    Return _NavigationItems
                End Get
                Set(value As Security.NavigationInformation())
                    _NavigationItems = value
                End Set
            End Property

            Private _AuthorizationsForGroupsByRule As Security.GroupAuthorizationItemsByRuleForSecurityObjects
            ''' <summary>
            ''' Authorizations of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property AuthorizationsForGroupsByRule As Security.GroupAuthorizationItemsByRuleForSecurityObjects
                Get
                    If _AuthorizationsForGroupsByRule Is Nothing OrElse _AuthorizationsForGroupsByRule.CurrentContextServerGroupIDInitialized <> (_WebManager.CurrentServerInfo IsNot Nothing) Then 'no cache object available OR srv. group initialization context changed
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("", MyConn)
                        MyCmd.CommandType = CommandType.Text
                        If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                            MyCmd.CommandText = "select applicationsrightsbygroup.ID as AuthorizationID, applicationsrightsbygroup.ID_Application as AuthorizationSecurityObjectID, applicationsrightsbygroup.ID_GroupOrPerson as AuthorizationGroupID, applicationsrightsbygroup.ID_ServerGroup as AuthorizationServerGroupID, applicationsrightsbygroup.ReleasedOn as AuthorizationReleasedOn, applicationsrightsbygroup.ReleasedBy as AuthorizationReleasedBy, applicationsrightsbygroup.DevelopmentTeamMember As AuthorizationIsDeveloper, applicationsrightsbygroup.IsDenyRule from applicationsrightsbygroup inner join Applications_CurrentAndInactiveOnes on applicationsrightsbygroup.id_application = Applications_CurrentAndInactiveOnes.id where applicationsrightsbygroup.id_application = @ID and Applications_CurrentAndInactiveOnes.id is not null"
                        Else
                            MyCmd.CommandText = "select applicationsrightsbygroup.ID as AuthorizationID, applicationsrightsbygroup.ID_Application as AuthorizationSecurityObjectID, applicationsrightsbygroup.ID_GroupOrPerson as AuthorizationGroupID, NULL as AuthorizationServerGroupID, applicationsrightsbygroup.ReleasedOn as AuthorizationReleasedOn, applicationsrightsbygroup.ReleasedBy as AuthorizationReleasedBy, CAST(0 As bit) As AuthorizationIsDeveloper, CAST(0 As bit) As IsDenyRule from applicationsrightsbygroup inner join Applications_CurrentAndInactiveOnes on applicationsrightsbygroup.id_application = Applications_CurrentAndInactiveOnes.id where applicationsrightsbygroup.id_application = @ID and Applications_CurrentAndInactiveOnes.id is not null"
                        End If
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim SecObjects As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "SecurityObjects")
                        Dim AllowRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForGroup)
                        Dim AllowRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForGroup)
                        Dim DenyRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForGroup)
                        Dim DenyRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForGroup)
                        For MyCounter As Integer = 0 To SecObjects.Rows.Count - 1
                            Dim MyDataRow As DataRow = SecObjects.Rows(MyCounter)
                            Dim secObjAuth As New SecurityObjectAuthorizationForGroup(_WebManager, CType(MyDataRow("AuthorizationID"), Integer), CType(MyDataRow("AuthorizationGroupID"), Integer), CType(MyDataRow("AuthorizationSecurityObjectID"), Integer), Utils.Nz(MyDataRow("AuthorizationServerGroupID"), 0), Nothing, Me, Nothing, Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False), Utils.Nz(MyDataRow("IsDenyRule"), False), CType(MyDataRow("AuthorizationReleasedOn"), DateTime), CType(MyDataRow("AuthorizationReleasedBy"), Integer), False)
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
                            _AuthorizationsForGroupsByRule = New Security.GroupAuthorizationItemsByRuleForSecurityObjects(
                                0,
                                Me._ID,
                                AllowRuleAuthsNonDev.ToArray(),
                                AllowRuleAuthsIsDev.ToArray(),
                                DenyRuleAuthsNonDev.ToArray(),
                                DenyRuleAuthsIsDev.ToArray(),
                                Me._WebManager)
                        Else
                            _AuthorizationsForGroupsByRule = New Security.GroupAuthorizationItemsByRuleForSecurityObjects(
                                _WebManager.CurrentServerInfo.ParentServerGroupID,
                                0,
                                Me._ID,
                                AllowRuleAuthsNonDev.ToArray(),
                                AllowRuleAuthsIsDev.ToArray(),
                                DenyRuleAuthsNonDev.ToArray(),
                                DenyRuleAuthsIsDev.ToArray(),
                                Me._WebManager)
                        End If
                    End If
                    Return _AuthorizationsForGroupsByRule
                End Get
            End Property

            Private _AuthorizationsForUsersByRule As Security.UserAuthorizationItemsByRuleForSecurityObjects
            ''' <summary>
            ''' Authorizations of the current user by rule-set
            ''' </summary>
            Public ReadOnly Property AuthorizationsForUsersByRule As Security.UserAuthorizationItemsByRuleForSecurityObjects
                Get
                    If _AuthorizationsForUsersByRule Is Nothing OrElse _AuthorizationsForUsersByRule.CurrentContextServerGroupIDInitialized <> (_WebManager.CurrentServerInfo IsNot Nothing) Then 'no cache object available OR srv. group initialization context changed
                        Dim MyConn As New SqlConnection(_WebManager.ConnectionString)
                        Dim MyCmd As New SqlCommand("", MyConn)
                        MyCmd.CommandType = CommandType.Text
                        If Setup.DatabaseUtils.Version(_WebManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                            MyCmd.CommandText = "select applicationsrightsbyuser.ID as AuthorizationID, applicationsrightsbyuser.ID_Application as AuthorizationSecurityObjectID, applicationsrightsbyuser.ID_GroupOrPerson as AuthorizationGroupID, applicationsrightsbyuser.ID_ServerGroup as AuthorizationServerGroupID, applicationsrightsbyuser.ReleasedOn as AuthorizationReleasedOn, applicationsrightsbyuser.ReleasedBy as AuthorizationReleasedBy, applicationsrightsbyuser.DevelopmentTeamMember as AuthorizationIsDeveloper, applicationsrightsbyuser.IsDenyRule from applicationsrightsbyuser inner join Applications_CurrentAndInactiveOnes on applicationsrightsbyuser.id_application = Applications_CurrentAndInactiveOnes.id where applicationsrightsbyuser.id_application = @ID and Applications_CurrentAndInactiveOnes.id is not null"
                        Else
                            MyCmd.CommandText = "select applicationsrightsbyuser.ID as AuthorizationID, applicationsrightsbyuser.ID_Application as AuthorizationSecurityObjectID, applicationsrightsbyuser.ID_GroupOrPerson as AuthorizationGroupID, NULL as AuthorizationServerGroupID, applicationsrightsbyuser.ReleasedOn as AuthorizationReleasedOn, applicationsrightsbyuser.ReleasedBy as AuthorizationReleasedBy, applicationsrightsbyuser.DevelopmentTeamMember as AuthorizationIsDeveloper, CAST(0 As bit) As IsDenyRule from applicationsrightsbyuser inner join Applications_CurrentAndInactiveOnes on applicationsrightsbyuser.id_application = Applications_CurrentAndInactiveOnes.id where applicationsrightsbyuser.id_application = @ID and Applications_CurrentAndInactiveOnes.id is not null"
                        End If
                        MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = _ID
                        Dim SecObjects As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "SecurityObjects")
                        Dim AllowRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForUser)
                        Dim AllowRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForUser)
                        Dim DenyRuleAuthsNonDev As New List(Of SecurityObjectAuthorizationForUser)
                        Dim DenyRuleAuthsIsDev As New List(Of SecurityObjectAuthorizationForUser)
                        For MyCounter As Integer = 0 To SecObjects.Rows.Count - 1
                            Dim MyDataRow As DataRow = SecObjects.Rows(MyCounter)
                            Dim secObjAuth As New SecurityObjectAuthorizationForUser(_WebManager, CType(MyDataRow("AuthorizationID"), Integer), CType(MyDataRow("AuthorizationGroupID"), Integer), CType(MyDataRow("AuthorizationSecurityObjectID"), Integer), Utils.Nz(MyDataRow("AuthorizationServerGroupID"), 0), Nothing, Me, Nothing, Utils.Nz(MyDataRow("AuthorizationIsDeveloper"), False), Utils.Nz(MyDataRow("IsDenyRule"), False), CType(MyDataRow("AuthorizationReleasedOn"), DateTime), CType(MyDataRow("AuthorizationReleasedBy"), Integer), False)
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
                            _AuthorizationsForUsersByRule = New Security.UserAuthorizationItemsByRuleForSecurityObjects(
                                0L,
                                Me._ID,
                                AllowRuleAuthsNonDev.ToArray(),
                                AllowRuleAuthsIsDev.ToArray(),
                                DenyRuleAuthsNonDev.ToArray(),
                                DenyRuleAuthsIsDev.ToArray(),
                                Me._WebManager)
                        Else
                            _AuthorizationsForUsersByRule = New Security.UserAuthorizationItemsByRuleForSecurityObjects(
                                _WebManager.CurrentServerInfo.ParentServerGroupID,
                                0L,
                                Me._ID,
                                AllowRuleAuthsNonDev.ToArray(),
                                AllowRuleAuthsIsDev.ToArray(),
                                DenyRuleAuthsNonDev.ToArray(),
                                DenyRuleAuthsIsDev.ToArray(),
                                Me._WebManager)
                        End If
                    End If
                    Return _AuthorizationsForUsersByRule
                End Get
            End Property

            ''' <summary>
            '''     The authorizations list which users are authorized for this security object
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use AuthorizationsForUsersByRule instead")> Public ReadOnly Property AuthorizationsForUsers() As SecurityObjectAuthorizationForUser()
                Get
                    Return AuthorizationsForUsersByRule.EffectiveByDenyRuleStandard()
                End Get
            End Property

            ''' <summary>
            '''     The authorizations list which groups are authorized for this security object
            ''' </summary>
            ''' <value></value>
            <Obsolete("Use AuthorizationsForGroupsByRule instead")> Public ReadOnly Property AuthorizationsForGroups() As SecurityObjectAuthorizationForGroup()
                Get
                    Return AuthorizationsForGroupsByRule.EffectiveByDenyRuleStandard()
                End Get
            End Property

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userID">The user ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub AddAuthorizationForUser(ByVal userID As Long, ByVal serverGroupID As Integer, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorizationForUser(userID, serverGroupID, False, notifications)
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userID">The user ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub AddAuthorizationForUser(ByVal userID As Long, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorizationForUser(userID, serverGroupID, developerAuthorization, False, notifications)
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userID">The user ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorizationForUser(ByVal userID As Long, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                AddAuthorizationForUser(New UserInformation(userID, Me._WebManager), serverGroupID, developerAuthorization, isDenyRule, notifications)
                'Requery the list of authorization next time it's required
                _AuthorizationsForUsersByRule = Nothing
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userInfo">The user object</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <param name="developerAuthorization">The developer authorization allows a user to see/access applications with this security objects even if it is currently disabled</param>
            ''' <param name="notifications">A notification class which contains the e-mail templates which might be sent</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorizationForUser(ByVal userInfo As UserInformation, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean, Optional ByVal notifications As Notifications.INotifications = Nothing)
                If isDenyRule = False Then
                    Dim RequiredApplicationFlags As String() = Me.RequiredAdditionalFlags
                    Dim RequiredFlagsValidationResults As FlagValidation.FlagValidationResult() = FlagValidation.ValidateRequiredFlags(userInfo, RequiredApplicationFlags, True)
                    If RequiredFlagsValidationResults.Length <> 0 Then
                        Throw New FlagValidation.RequiredFlagException(RequiredFlagsValidationResults)
                    End If
                End If
                DataLayer.Current.AddUserAuthorization(_WebManager, Nothing, Me._ID, serverGroupID, userInfo, userInfo.IDLong, developerAuthorization, isDenyRule, _WebManager.CurrentUserID(SpecialUsers.User_Anonymous), notifications)
                'Requery the list of authorization next time it's required
                _AuthorizationsForUsersByRule = Nothing
                userInfo.ResetAuthorizationsCache()
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userID">The user ID</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub RemoveAuthorizationForUser(ByVal userID As Long, ByVal serverGroupID As Integer)
                RemoveAuthorizationForUser(userID, serverGroupID, False, False)
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userID">The user ID</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorizationForUser(ByVal userID As Long, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveUserAuthorization(Me._WebManager, Me._ID, userID, serverGroupID, isDeveloperAuthorization, isDenyRule)
                _AuthorizationsForUsersByRule = Nothing
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="userInfo">The user</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorizationForUser(ByVal userInfo As WMSystem.UserInformation, ByVal serverGroupID As Integer, isDeveloperAuthorization As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveUserAuthorization(Me._WebManager, Me._ID, userInfo.IDLong, serverGroupID, isDeveloperAuthorization, isDenyRule)
                userInfo.ResetAuthorizationsCache()
                _AuthorizationsForUsersByRule = Nothing
            End Sub

            ''' <summary>
            ''' Reset cached/calculated authorizations
            ''' </summary>
            Friend Sub ResetAuthorizationsCacheForGroups()
                _AuthorizationsForGroupsByRule = Nothing
            End Sub

            ''' <summary>
            ''' Reset cached/calculated authorizations
            ''' </summary>
            Friend Sub ResetAuthorizationsCacheForUsers()
                _AuthorizationsForUsersByRule = Nothing
            End Sub

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The group ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDev/isDenyRule parameter")> Public Sub AddAuthorizationForGroup(ByVal groupID As Integer, ByVal serverGroupID As Integer)
                AddAuthorizationForGroup(groupID, serverGroupID)
            End Sub

            ''' <summary>
            ''' Checks if all effective members of a group have got the required flags for a security object
            ''' </summary>
            ''' <param name="requiredFlags"></param>
            ''' <param name="groupID"></param>
            ''' <param name="isDenyRule"></param>
            ''' <returns>0 if all required flags are available, or the first user ID of the error users list if at 1 or more flags are missing at 1 or more users</returns>
            Private Function ValidateRequiredFlagsOnAllRelatedUsers(requiredFlags As String(), groupID As Integer, isDenyRule As Boolean) As Long
                Return ValidateRequiredFlagsOnAllRelatedUsers(requiredFlags, Me._ID, groupID, isDenyRule, Me._WebManager)
            End Function

            ''' <summary>
            ''' Checks if all effective members of a group have got the required flags for a security object
            ''' </summary>
            ''' <param name="requiredFlags"></param>
            ''' <param name="securityObjectID"></param>
            ''' <param name="groupID"></param>
            ''' <param name="isDenyRule"></param>
            ''' <param name="webManager"></param>
            ''' <returns>0 if all required flags are available, or the first user ID of the error users list if 1 or more flags are missing at 1 or more users</returns>
            Friend Shared Function ValidateRequiredFlagsOnAllRelatedUsers(requiredFlags As String(), securityObjectID As Integer, groupID As Integer, isDenyRule As Boolean, webManager As WMSystem) As Long
                If isDenyRule = True Then
                    'Deny rules don't need to check required flags
                    Return 0L
                ElseIf requiredFlags.Length = 0 Then
                    Return 0L
                ElseIf Setup.DatabaseUtils.Version(webManager, True).CompareTo(WMSystem.MilestoneDBVersion_AuthsWithSupportForDenyRule) >= 0 Then 'Newer
                    Dim SqlFlagsEnumeration As New Text.StringBuilder
                    For MyCounter As Integer = 0 To requiredFlags.Length - 1
                        If SqlFlagsEnumeration.Length <> 0 Then
                            SqlFlagsEnumeration.Append(","c)
                        End If
                        SqlFlagsEnumeration.Append("N'" & requiredFlags(MyCounter).Replace("'", "''") & "'")
                    Next
                    Dim Sql As String = "    SELECT TOP 1 ID_User, COUNT(*) AS FoundFlagsCount" & vbNewLine &
                            "    FROM dbo.Log_Users" & vbNewLine &
                            "    WHERE Type IN (" & SqlFlagsEnumeration.ToString & ")" & vbNewLine &
                            "    AND ID_User IN " & vbNewLine &
                            "    (" & vbNewLine &
                            "        SELECT [dbo].[Memberships_EffectiveRulesWithClonesNthGrade].ID_User" & vbNewLine &
                            "        FROM [dbo].[ApplicationsRightsByGroup] " & vbNewLine &
                            "            INNER JOIN [dbo].[Memberships_EffectiveRulesWithClonesNthGrade]" & vbNewLine &
                            "                ON [dbo].[ApplicationsRightsByGroup].ID_GroupOrPerson = [dbo].[Memberships_EffectiveRulesWithClonesNthGrade].ID_Group" & vbNewLine &
                            "        WHERE [dbo].[ApplicationsRightsByGroup].isdenyrule = 0" & vbNewLine &
                            "            AND [dbo].[ApplicationsRightsByGroup].ID_Application = @SecObjID" & vbNewLine &
                            "            AND [dbo].[ApplicationsRightsByGroup].ID_GroupOrPerson = @GroupID" & vbNewLine &
                            "    )" & vbNewLine &
                            "    GROUP BY ID_User" & vbNewLine &
                            "    HAVING COUNT(*) <> @RequiredFlagsCount"
                    Dim MyCmd As New SqlCommand(Sql, New SqlConnection(webManager.ConnectionString))
                    MyCmd.CommandType = CommandType.Text
                    MyCmd.Parameters.Add("@SecObjID", SqlDbType.Int).Value = securityObjectID
                    MyCmd.Parameters.Add("@GroupID", SqlDbType.Int).Value = groupID
                    MyCmd.Parameters.Add("@RequiredFlagsCount", SqlDbType.Int).Value = requiredFlags.Length
                    Dim FoundFirstUserWithMissingFlag As Long = Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0L)
                    Return FoundFirstUserWithMissingFlag
                Else
                    'no check - depending views only exist since that milestone
                    Return 0L
                End If
            End Function

            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The group ID</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorizationForGroup(ByVal groupID As Integer, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean)
                If isDenyRule = False Then
                    Dim FirstUserIDWithMissingFlags As Long = Me.ValidateRequiredFlagsOnAllRelatedUsers(Me.RequiredAdditionalFlags, groupID, isDenyRule)
                    If FirstUserIDWithMissingFlags <> 0L Then
                        Dim FirstError As New FlagValidation.FlagValidationResult(FirstUserIDWithMissingFlags, FlagValidation.FlagValidationResultCode.Missing)
                        Throw New FlagValidation.RequiredFlagException(FirstError)
                    End If
                End If
                CompuMaster.camm.WebManager.DataLayer.Current.AddGroupAuthorization(Me._WebManager, Me._ID, groupID, serverGroupID, developerAuthorization, isDenyRule)
                _AuthorizationsForGroupsByRule = Nothing
            End Sub
            ''' <summary>
            ''' Add an authorization to a security object (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupInfo">The group</param>
            ''' <param name="serverGroupID">The authorization will be related only for the given server group ID, otherwise use 0 (zero value) for assigning authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub AddAuthorizationForGroup(ByVal groupInfo As GroupInformation, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean)
                AddAuthorizationForGroup(groupInfo.ID, serverGroupID, developerAuthorization, isDenyRule)
                groupInfo.ResetAuthorizationsCache()
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The user ID</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            <Obsolete("Better use overloaded method with isDenyRule parameter")> Public Sub RemoveAuthorizationForGroup(ByVal groupID As Integer, ByVal serverGroupID As Integer)
                RemoveAuthorizationForGroup(groupID, serverGroupID, False, False)
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupID">The user ID</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorizationForGroup(ByVal groupID As Integer, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveGroupAuthorization(Me._WebManager, Me._ID, groupID, serverGroupID, developerAuthorization, isDenyRule)
                _AuthorizationsForGroupsByRule = Nothing
            End Sub

            ''' <summary>
            ''' Remove an authorization (doesn't require saving, action is performed immediately on database)
            ''' </summary>
            ''' <param name="groupInfo">The user ID</param>
            ''' <param name="serverGroupID">The authorization related only to the given server group ID will be removed, otherwise use 0 (zero value) for specifying the authorization to all server groups</param>
            ''' <remarks>This action will be done immediately without the need for saving</remarks>
            Public Sub RemoveAuthorizationForGroup(ByVal groupInfo As GroupInformation, ByVal serverGroupID As Integer, ByVal developerAuthorization As Boolean, isDenyRule As Boolean)
                CompuMaster.camm.WebManager.DataLayer.Current.RemoveGroupAuthorization(Me._WebManager, Me._ID, groupInfo.ID, serverGroupID, developerAuthorization, isDenyRule)
                groupInfo.ResetAuthorizationsCache()
                _AuthorizationsForGroupsByRule = Nothing
            End Sub

        End Class

    End Class

End Namespace