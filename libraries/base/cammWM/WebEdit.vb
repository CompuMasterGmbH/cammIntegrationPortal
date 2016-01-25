Option Explicit On 
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.WebManager.Modules.WebEdit

    Namespace Controls

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Controls for online editing of web pages (and web controls)
        ''' </summary>
        ''' <history>
        ''' 	[adminsupport]	19.01.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Class NamespaceDoc
        End Class

        Public Class Configuration

            Friend Sub New()
                'Creatable only assembly-internally
            End Sub

            Public ReadOnly Property ContentOfServerID() As Integer
                Get
                    Return WebManager.Configuration.WebEditorContentOfServerID
                End Get
            End Property

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Modules.WebEdit.Controls.SmartWcms.DatabaseAccessLayer
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Provider for loading and saving of data from and to the database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Class SmartWcmsDatabaseAccessLayer

#Region "Constructor"
            Private _swcms As CompuMaster.camm.WebManager.Modules.WebEdit.Controls.ISmartWcmsEditor
            Friend Sub New(ByVal swcms As CompuMaster.camm.WebManager.Modules.WebEdit.Controls.ISmartWcmsEditor)
                _swcms = swcms
            End Sub
#End Region

#Region "Property redirects"
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The connection string
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	11.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private ReadOnly Property ConnectionString() As String
                Get
                    Return Me._swcms.cammWebManager.ConnectionString
                End Get
            End Property

#End Region

#Region "Cache reset"
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Clear all cached variables which contain database values
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub ClearCachedDbValues()
                _CachedMaxVersion_EditorID = Nothing
                _CachedMaxVersion_Result = Integer.MinValue
                _CachedMaxVersion_ServerID = Integer.MinValue
                _CachedMaxVersion_Url = Nothing
                _CachedReleasedVersion_EditorID = Nothing
                _CachedReleasedVersion_Result = Integer.MinValue
                _CachedReleasedVersion_ServerID = Integer.MinValue
                _CachedReleasedVersion_Url = Nothing
                _CachedCompleteData_EditorID = Nothing
                _CachedCompleteData_Result = Nothing
                _CachedCompleteData_ServerID = Integer.MinValue
                _CachedCompleteData_Url = Nothing
            End Sub

#End Region

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     An array of available languages within an editor controls version
            ''' </summary>
            ''' <param name="Url"></param>
            ''' <param name="Version"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function AvailableMarketsInData(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal Version As Integer) As Integer()
                Dim myConnection As SqlClient.SqlConnection
                Dim myCommand As SqlClient.SqlCommand
                Dim myQuery As String = _
                    "select LanguageID from webmanager_webeditor where Url = @Url " & vbNewLine & _
                    "and EditorID = @EditorID and version = @Version AND ServerID = @ServerID group by LanguageID"
                myConnection = New SqlClient.SqlConnection(ConnectionString)
                myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                myCommand.Parameters.Add("@Version", SqlDbType.Int).Value = Version
                Return CType(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToArray(GetType(Integer)), Integer())
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Save the content of the editor control into the database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <param name="content"></param>
            ''' <param name="modifiedBy"></param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub SaveEditorContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal content As String, ByVal modifiedBy As Long)
                'Prepare query:
                Dim myQuery As String = _
                    "DECLARE @Version int" & vbNewLine & _
                    "DECLARE @ReplaceExistingValue bit" & vbNewLine & _
                    "DECLARE @ModifiedOn datetime" & vbNewLine & _
                    "-- Retrieve editable version" & vbNewLine & _
                    "SELECT @Version = IsNull(MAX(Version), 0) + 1" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE IsActive = 1 AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "-- Insert or update data?" & vbNewLine & _
                    "SELECT @ReplaceExistingValue = CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE Version = @Version AND LanguageID = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "-- Insert new or update existing data" & vbNewLine & _
                    "SELECT @ModifiedOn = GETDATE()" & vbNewLine & _
                    "IF @ReplaceExistingValue = 1" & vbNewLine & _
                    "   UPDATE [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "   SET [ServerID]=@ServerID, [LanguageID]=@LanguageID, [IsActive]=0," & vbNewLine & _
                    "       [URL]=@URL, [EditorID]=@EditorID, [Content]=@Content, [ModifiedOn]=@ModifiedOn," & vbNewLine & _
                    "       [ModifiedByUser]=@ModifiedByUser, [ReleasedOn]=NULL, " & vbNewLine & _
                    "       [ReleasedByUser]=NULL, [Version]=@Version" & vbNewLine & _
                    "   WHERE Version = @Version AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "ELSE" & vbNewLine & _
                    "   INSERT INTO [dbo].[WebManager_WebEditor](" & vbNewLine & _
                    "       [ServerID], [LanguageID], [IsActive], [URL], [EditorID], [Content], [ModifiedOn], " & vbNewLine & _
                    "       [ModifiedByUser], [ReleasedOn], [ReleasedByUser], [Version]" & vbNewLine & _
                    "       )" & vbNewLine & _
                    "   VALUES(" & vbNewLine & _
                    "       @ServerID, @LanguageID, 0, " & vbNewLine & _
                    "       @URL, @EditorID, @Content, @ModifiedOn, " & vbNewLine & _
                    "       @ModifiedByUser, NULL, NULL, " & vbNewLine & _
                    "       @Version)"

                'Establish connection
                Dim myCommand As SqlClient.SqlCommand
                myCommand = New SqlClient.SqlCommand(myQuery, New SqlClient.SqlConnection(ConnectionString))
                'Add params:
                myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                myCommand.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                myCommand.Parameters.Add("@Content", SqlDbType.NText).Value = content
                myCommand.Parameters.Add("@ModifiedByUser", SqlDbType.Int).Value = modifiedBy
                'Execute the insert statement
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Identify if a given market exists in the defined, versioned document
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <param name="version"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function IsMarketAvailable(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer) As Boolean
                Dim Data As DataRow() = Me.ReadAllData(serverID, url, editorID).Select("LanguageID = " & marketID & " AND Version = " & version)
                If Data.Length = 0 Then
                    Return False
                Else
                    Return True
                End If
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Remove a given market from the editable version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub RemoveMarketFromEditVersion(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer)
                'Prepare query:
                Dim myQuery As String = _
                    "DECLARE @Version int" & vbNewLine & _
                    "-- Retrieve editable version" & vbNewLine & _
                    "SELECT @Version = IsNull(MAX(Version), 0) + 1" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE IsActive = 1 AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "-- Remove rows regarding the defined market" & vbNewLine & _
                    "DELETE" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID AND Version = @Version AND LanguageID = @LanguageID"

                'Establish connection
                Dim myCommand As SqlClient.SqlCommand
                myCommand = New SqlClient.SqlCommand(myQuery, New SqlClient.SqlConnection(ConnectionString))
                'Add params:
                myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                myCommand.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                'Execute the insert statement
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Release the latest version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="releasedByUser"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function ReleaseLatestVersion(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal releasedByUser As Long) As Boolean
                Dim myQuery As String = _
                    "DECLARE @Version int" & vbNewLine & _
                    "-- Retrieve latest version" & vbNewLine & _
                    "SELECT @Version = IsNull(MAX(Version), 0)" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "-- Release latest version" & vbNewLine & _
                    "UPDATE [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "SET [IsActive] = 0" & vbNewLine & _
                    "WHERE [URL] = @URL AND [EditorID] = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "UPDATE [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "SET [IsActive] = 1, [ReleasedByUser] = @ReleasedByUser, ReleasedOn = GETDATE()" & vbNewLine & _
                    "WHERE [URL] = @URL AND [EditorID] = @EditorID AND ServerID = @ServerID AND [Version] = @Version"

                Dim myConnection As SqlClient.SqlConnection
                Dim myCommand As SqlClient.SqlCommand
                'Establish connection
                myConnection = New SqlClient.SqlConnection(ConnectionString)
                myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                'Add params:
                myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url
                myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                myCommand.Parameters.Add("@ReleasedByUser", SqlDbType.Int).Value = releasedByUser
                'Execute the insert statement
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Function

            Private _CachedMaxVersion_ServerID As Integer = Integer.MinValue
            Private _CachedMaxVersion_Url As String = Nothing
            Private _CachedMaxVersion_EditorID As String = Nothing
            Private _CachedMaxVersion_Result As Integer = Integer.MinValue
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The highest version available, this is either the released version number or the version number of the current edit version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property MaxVersion(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String) As Integer
                Get
                    If serverID = Me._CachedMaxVersion_ServerID AndAlso url = Me._CachedMaxVersion_Url AndAlso editorID = Me._CachedMaxVersion_EditorID AndAlso Me._CachedMaxVersion_Result <> Integer.MinValue Then
                        'Valid, already queried version number
                    Else
                        Me.ReadRelatedVersionInfo(serverID, url, editorID)
                    End If
                    Return _CachedMaxVersion_Result
                End Get
            End Property

            Private _CachedReleasedVersion_ServerID As Integer = Integer.MinValue
            Private _CachedReleasedVersion_Url As String = Nothing
            Private _CachedReleasedVersion_EditorID As String = Nothing
            Private _CachedReleasedVersion_Result As Integer = Integer.MinValue
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The number of the released version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property ReleasedVersion(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String) As Integer
                Get
                    If serverID = Me._CachedReleasedVersion_ServerID AndAlso url = Me._CachedReleasedVersion_Url AndAlso editorID = Me._CachedReleasedVersion_EditorID AndAlso Me._CachedReleasedVersion_Result <> Integer.MinValue Then
                        'Valid, already queried version number
                    Else
                        Me.ReadRelatedVersionInfo(serverID, url, editorID)
                    End If
                    Return _CachedReleasedVersion_Result
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Read max. version and released version number from database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <remarks>
            '''     If a version number can't be looked up because there is no data available, the read version number will be 0 (zero).
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub ReadRelatedVersionInfo(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String)

                'Try to lookup the important version numbers from already queried data when it is there
                If serverID <> Me._CachedCompleteData_ServerID OrElse url <> Me._CachedCompleteData_Url OrElse editorID = Me._CachedCompleteData_EditorID OrElse Me._CachedCompleteData_Result Is Nothing Then
                    'Yes! Complete data has already been queried
                    Dim myDataTable As DataTable = ReadAllData(serverID, url, editorID)
                    Dim _MaxVersion As Integer = 0
                    Dim _ReleasedVersion As Integer = 0
                    For Each myDataRow As DataRow In myDataTable.Rows
                        If CType(CompuMaster.camm.WebManager.Utils.Nz(myDataRow("IsActive")), Boolean) = True Then
                            _ReleasedVersion = CompuMaster.camm.WebManager.Utils.Nz(myDataRow("Version"), 0)
                            Exit For
                        End If
                    Next
                    For MyCounter As Integer = 0 To myDataTable.Rows.Count - 1
                        _MaxVersion = System.Math.Max(_MaxVersion, CType(myDataTable.Rows(MyCounter)("Version"), Integer))
                    Next
                    'Take results
                    _CachedMaxVersion_Result = _MaxVersion
                    _CachedReleasedVersion_Result = _ReleasedVersion
                Else
                    'No, then perform a special query to just lookup the requested two important version numbers
                    Dim myConnection As SqlClient.SqlConnection
                    Dim myCommand As SqlClient.SqlCommand
                    Dim myQuery As String = _
                        "DECLARE @MaxVersion int, @ReleasedVersion int" & vbNewLine & _
                        "select @MaxVersion = Max([Version]) from [dbo].[WebManager_WebEditor]" & vbNewLine & _
                        "where [URL] = @URL AND [EditorID] = @EditorID and ServerID = @ServerID" & vbNewLine & _
                        "select @ReleasedVersion = Max([Version]) from [dbo].[WebManager_WebEditor]" & vbNewLine & _
                        "where IsActive = 1 AND [URL] = @URL AND [EditorID] = @EditorID and ServerID = @ServerID" & vbNewLine & _
                        "select @MaxVersion, @ReleasedVersion"
                    myConnection = New SqlClient.SqlConnection(ConnectionString)
                    myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                    myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url
                    myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                    myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID

                    'Execute command
                    Dim results As DictionaryEntry
                    results = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)(0)
                    'Take results
                    _CachedMaxVersion_Result = Utils.Nz(results.Key, 0)
                    _CachedReleasedVersion_Result = Utils.Nz(results.Value, 0)
                End If

                'Update caching environment for cached values
                _CachedMaxVersion_ServerID = serverID
                _CachedMaxVersion_Url = url
                _CachedMaxVersion_EditorID = editorID
                _CachedReleasedVersion_ServerID = serverID
                _CachedReleasedVersion_Url = url
                _CachedReleasedVersion_EditorID = editorID
            End Sub

            Private _CachedCompleteData_Result As DataTable = Nothing
            Private _CachedCompleteData_ServerID As Integer = Integer.MinValue
            Private _CachedCompleteData_Url As String = Nothing
            Private _CachedCompleteData_EditorID As String = Nothing
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Read all data from database regarding a defined document
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function ReadAllData(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String) As DataTable
                If serverID <> Me._CachedCompleteData_ServerID OrElse url <> Me._CachedCompleteData_Url OrElse editorID = Me._CachedCompleteData_EditorID OrElse Me._CachedCompleteData_Result Is Nothing Then
                    Dim myConnection As SqlClient.SqlConnection
                    Dim myCommand As SqlClient.SqlCommand
                    Dim myQuery As String = _
                        "select [ServerID],[LanguageID],[IsActive],[URL],[EditorID],[ModifiedOn],[ModifiedByUser],[ReleasedOn],[ReleasedByUser],[Version]" & vbNewLine & _
                        "from [dbo].[WebManager_WebEditor]" & vbNewLine & _
                        "where [URL] = @URL AND [EditorID] = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                        "order by [ID]"
                    myConnection = New SqlClient.SqlConnection(ConnectionString)
                    myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                    myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url
                    myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                    myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                    'Execute command
                    _CachedCompleteData_Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "result")
                    'Update caching environment for cached values
                    _CachedCompleteData_ServerID = serverID
                    _CachedCompleteData_Url = url
                    _CachedCompleteData_EditorID = editorID
                End If
                Return _CachedCompleteData_Result
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Copys a editor controls version to a new higher one.
            ''' </summary>
            ''' <param name="sourceVersion">The version you want to copy</param>
            ''' <param name="serverID">The WebManagers serverID where the new version shall appear (might be usefull for later feature requests)</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub CopyEditorVersion(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal sourceVersion As Integer)
                If sourceVersion = Nothing Then
                    Throw New ArgumentNullException("sourceVersion")
                End If

                'Prepare query:
                Dim myQuery As String = _
                    "DECLARE @Version int" & vbNewLine & _
                    "-- Retrieve editable version" & vbNewLine & _
                    "SELECT @Version = IsNull(MAX(Version), 0) + 1" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE IsActive = 1 AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine & _
                    "-- Remove any pre-existing data (but regulary, there should be nothing, but sure is sure)" & vbNewLine & _
                    "DELETE" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID AND Version = @Version" & vbNewLine & _
                    "-- Copy data rows" & vbNewLine & _
                    "INSERT INTO [dbo].[WebManager_WebEditor](" & vbNewLine & _
                    "   [ServerID], [LanguageID], [IsActive], [URL], [EditorID], [Content], [ModifiedOn], " & vbNewLine & _
                    "   [ModifiedByUser], [ReleasedOn], [ReleasedByUser], [Version]" & vbNewLine & _
                    "   )" & vbNewLine & _
                    "SELECT ServerID, LanguageID, 0, [URL], EditorID, Content, ModifiedOn, ModifiedByUser, NULL, NULL, @Version" & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID AND Version = @SourceVersion"

                'Establish connection
                Dim myCommand As SqlClient.SqlCommand
                myCommand = New SqlClient.SqlCommand(myQuery, New SqlClient.SqlConnection(ConnectionString))
                'Add params:
                myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                myCommand.Parameters.Add("@SourceVersion", SqlDbType.Int).Value = sourceVersion
                'Execute the insert statement
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     List the activated markets as defined in camm Web-Manager
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function ActiveMarketsInWebManager() As DataTable
                If HttpContext.Current.Application("WebManager.sWcms.ActiveLanguages") Is Nothing Then
                    Dim myConnection As SqlClient.SqlConnection
                    Dim myCommand As SqlClient.SqlCommand
                    Dim myQuery As String = _
                        "select ID, Description_English, AlternativeLanguage from system_languages where [IsActive] = 1"
                    myConnection = New SqlClient.SqlConnection(ConnectionString)
                    myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                    Dim Result As DataTable = Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(myCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "result")

                    'Also store the active language informations into the application
                    'object to avoid roundtrips to the database
                    HttpContext.Current.Application("WebManager.sWcms.ActiveLanguages") = Result

                    Return Result
                Else
                    Return CType(HttpContext.Current.Application("WebManager.sWcms.ActiveLanguages"), DataTable)
                End If
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     List the activated markets as defined in camm Web-Manager
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function ActiveMarketIDsInWebManager() As Integer()
                Dim Data As DataTable = Me.ActiveMarketsInWebManager
                Dim Result As New ArrayList
                For MyCounter As Integer = 0 To Data.Rows.Count - 1
                    If Not Result.Contains(CType(Data.Rows(MyCounter)("ID"), Integer)) Then
                        Result.Add(CType(Data.Rows(MyCounter)("ID"), Integer))
                    End If
                Next
                Return CType(Result.ToArray(GetType(Integer)), Integer())
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Read the released HTML for the requested document from the database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	17.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function ReadReleasedContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer) As String
                Dim MyCmd As New SqlClient.SqlCommand( _
                    "SELECT [Content] " & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE IsActive = 1 AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine, _
                    New SqlClient.SqlConnection(Me.ConnectionString))
                MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                MyCmd.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Read the HTML in a requested version for the requested document from the database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="marketID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="version"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	17.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function ReadContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer) As String
                Dim MyCmd As New SqlClient.SqlCommand( _
                    "SELECT [Content] " & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE Version = @Version AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine, _
                    New SqlClient.SqlConnection(Me.ConnectionString))
                MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                MyCmd.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                MyCmd.Parameters.Add("@Version", SqlDbType.Int).Value = version
                Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
            End Function

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Interface	 : camm.WebManager.Modules.WebEdit.Controls.ISmartWcmsEditor
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The common interface for all SmartWcms editor controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Interface ISmartWcmsEditor

            ReadOnly Property cammWebManager() As IWebManager

        End Interface

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Modules.WebEdit.Controls.SmartWcmsEditorBase
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A base implementation of a smart wcms editor control providing access to the database acces layer
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public MustInherit Class SmartWcmsEditorBase
            Inherits CompuMaster.camm.WebManager.Controls.Control
            Implements UI.INamingContainer, ISmartWcmsEditor

            Public ReadOnly Property Configuration() As Modules.WebEdit.Controls.Configuration
                Get
                    Static _Configuration As Modules.WebEdit.Controls.Configuration
                    If _Configuration Is Nothing Then _Configuration = New Modules.WebEdit.Controls.Configuration
                    Return _Configuration
                End Get
            End Property

#Region " Database methods "

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Database access layer
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	17.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected ReadOnly Property Database() As Modules.WebEdit.Controls.SmartWcmsDatabaseAccessLayer
                Get
                    Static _Database As Modules.WebEdit.Controls.SmartWcmsDatabaseAccessLayer
                    If _Database Is Nothing Then
                        _Database = New Modules.WebEdit.Controls.SmartWcmsDatabaseAccessLayer(Me)
                    End If
                    Return _Database
                End Get
            End Property

#End Region

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The interface implementation required for the database access layer
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private ReadOnly Property _cammWebManager() As IWebManager Implements ISmartWcmsEditor.cammWebManager
                Get
                    Return Me.cammWebManager
                End Get
            End Property

#Region "Properties"

            Private _DocumentID As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     An identifier of the current document, by default its URL
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property DocumentID() As String
                Get
                    If _DocumentID Is Nothing Then
                        'Needs to be initialized before the preperation of the VersionHistory HTML String gets started
                        Dim rawUrlWithScriptName As String
                        If System.Environment.Version.Major >= 4 AndAlso HttpContext.Current.Request.RawUrl.EndsWith("/") Then
                            'Beginning with .NET 4, RawUrl contains the URL as requested by the client, so the script name after a folder might be missing; e.g. /test/ is given, but required is /test/default.aspx later on
                            rawUrlWithScriptName = HttpContext.Current.Request.Url.AbsolutePath
                        Else
                            '.NET 1 + 2: RawUrl contains the URL as requested by the client + the request script name, so the script name after a folder is present; e.g. /test/ is given, RawUrl returns the expected /test/default.aspx
                            rawUrlWithScriptName = HttpContext.Current.Request.RawUrl
                        End If
                        If rawUrlWithScriptName.IndexOf("?") >= 0 Then
                            _DocumentID = rawUrlWithScriptName.ToLower.Substring(0, rawUrlWithScriptName.IndexOf("?"))
                        Else
                            _DocumentID = rawUrlWithScriptName.ToLower
                        End If
                    End If
                    Return _DocumentID
                End Get
                Set(ByVal Value As String)
                    _DocumentID = Value
                End Set
            End Property

            Private _ServerID As Integer
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Regulary, content is always related to the current server, only. In some special cases, you might want to override this to show content from another server.
            ''' </summary>
            ''' <value>The ID value of the server to whome the content is related</value>
            ''' <remarks>
            '''     By default, the address (e. g.) "/content.aspx" provides different content on different servers. So, the intranet and the extranet are able to show independent content.
            '''     In some cases, you might want to override this behaviour and you want to show on the same URL the same content in the extranet as well as in the intranet. In this case, you would setup this property on the extranet server's scripts to show the content of the intranet server.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	07.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property ContentOfServerID() As Integer
                Get
                    If _ServerID = Nothing Then
                        _ServerID = Me.Configuration.ContentOfServerID()
                        If _ServerID = Nothing Then
                            _ServerID = Me.cammWebManager.CurrentServerInfo.ID
                        End If
                    End If
                    Return _ServerID
                End Get
                Set(ByVal Value As Integer)
                    _ServerID = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains informations about how to handle the viewonly mode in different market, langs
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	31.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Enum MarketLookupModes As Integer
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Data is only available in an international version and this is valid for all languages/markets
                ''' </summary>
                ''' <remarks>
                '''     This value is the same as None, just the name is more explainable
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                SingleMarket = 0
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Data is only available in an international version and this is valid for all languages/markets
                ''' </summary>
                ''' <remarks>
                '''     This value is the same as SingleMarket, just the name is more simplified
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                None = 0
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Data is maintained for every market separately, the language markets (e. g. "English", "French", etc. are handled as a separate market)
                ''' </summary>
                ''' <remarks>
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                Market = 1
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Data is maintained for every language/market separately; when there is no value for a market it will be searched for some compatible language data
                ''' </summary>
                ''' <remarks>
                '''     Example: When the visitor is in market "German/Austria" but there is only some content available for market "German", the German data will be used.
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                Language = 2
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Data is maintained for every language/market separately; when there is no value for the current market, the sWCMS control tries to lookup a best matching content
                ''' </summary>
                ''' <remarks>
                '''     When the user requests a page in e. g. market 559 ("French/France"), there will be the following order for the lookup process:
                '''     <list>
                '''         <item>Current market, in ex. ID 559 / French/France</item>
                '''         <item>Current language of market, in ex. ID 3 / French</item>
                '''         <item>English universal, ID 1</item>
                '''         <item>Worldwide market, ID 10000</item>
                '''         <item>International, ID 0</item>
                '''     </list>
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                BestMatchingLanguage = 3
            End Enum

            Private _MarketLookupMode As MarketLookupModes
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Represents the current MarketLookupMode, passed as parameter by the ctrl
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	31.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property MarketLookupMode() As MarketLookupModes
                Get
                    Return _MarketLookupMode
                End Get
                Set(ByVal Value As MarketLookupModes)
                    _MarketLookupMode = Value
                End Set
            End Property 'MarketLookupMode()

            Private _SecurityObjectEditMode As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Indicates which application is needed to edit the formular
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	31.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property SecurityObjectEditMode() As String
                Get
                    Return _SecurityObjectEditMode
                End Get
                Set(ByVal Value As String)
                    _SecurityObjectEditMode = Value
                End Set
            End Property 'SecurityObjectEditMode()

#End Region
        End Class

    End Namespace

End Namespace