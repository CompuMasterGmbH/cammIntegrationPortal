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

        ''' <summary>
        ''' Provider for loading and saving of data from and to the database
        ''' </summary>
        ''' <remarks>
        ''' Based on cammWM.SmartEditor project (=re-import)
        ''' </remarks>
        Public Class SmartWcmsDatabaseAccessLayer

#Region "Constructor"
            Private _swcms As ISmartWcmsEditor
            Friend Sub New(ByVal swcms As ISmartWcmsEditor)
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
                    "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                    "select LanguageID from webmanager_webeditor where Url = @Url " & vbNewLine & _
                    "and EditorID = @EditorID and version = @Version AND ServerID = @ServerID group by LanguageID"
                myConnection = New SqlClient.SqlConnection(ConnectionString)
                myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                myCommand.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                myCommand.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                myCommand.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                myCommand.Parameters.Add("@Version", SqlDbType.Int).Value = Version
                Return CType(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToArray(GetType(Integer)), Integer())
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
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
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
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
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
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
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
                    results = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)(0)
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
                        "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
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
                    _CachedCompleteData_Result = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "result")
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
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
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
                        "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                        "select ID, Description_English, AlternativeLanguage from system_languages where [IsActive] = 1"
                    myConnection = New SqlClient.SqlConnection(ConnectionString)
                    myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                    Dim Result As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(myCommand, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "result")

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
                    "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                    "SELECT [Content] " & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE IsActive = 1 AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine, _
                    New SqlClient.SqlConnection(Me.ConnectionString))
                MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                MyCmd.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
            End Function

            ''' <summary>
            ''' Returns the first version that differs and that is lower (came before)
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <param name="version"></param>
            ''' <returns></returns>
            Public Function GetFirstPreviousVersionThatDiffers(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer) As Integer
                Dim command As New SqlClient.SqlCommand("SELECT TOP 1 version FROM dbo.WebManager_WebEditor " & vbNewLine & _
"WHERE content NOT LIKE ( SELECT  content FROM dbo.WebManager_WebEditor a " & vbNewLine & _
"WHERE a.URL = @URL  " & vbNewLine & _
"AND a.LanguageID = @LanguageID AND a.version = @VERSION AND a.ServerId = @ServerID AND a.EditorID = @EditorID) " & vbNewLine & _
" AND LanguageID =  @LanguageID AND URL = @URL AND ServerId = @ServerID AND EditorID = @EditorID AND version < @VERSION ORDER BY VERSION DESC ", New SqlClient.SqlConnection(Me.ConnectionString))

                command.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                command.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                command.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                command.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                command.Parameters.Add("@VERSION", SqlDbType.Int).Value = version


                Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(command, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, Integer))

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
                    "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                    "SELECT [Content] " & vbNewLine & _
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine & _
                    "WHERE Version = @Version AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine, _
                    New SqlClient.SqlConnection(Me.ConnectionString))
                MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
                MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
                MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
                MyCmd.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
                MyCmd.Parameters.Add("@Version", SqlDbType.Int).Value = version
                Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
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

        ''' <summary>
        '''     A base implementation of a smart wcms editor control providing access to the database acces layer
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public MustInherit Class SmartWcmsEditorBase
            Inherits CompuMaster.camm.WebManager.Controls.Control
            Implements UI.INamingContainer, ISmartWcmsEditor

            Public ReadOnly Property Configuration() As Configuration
                Get
                    Static _Configuration As Configuration
                    If _Configuration Is Nothing Then _Configuration = New Configuration
                    Return _Configuration
                End Get
            End Property

            ''' <summary>
            ''' The editor control to display or edit the content
            ''' </summary>
            ''' <returns></returns>
            Protected MustOverride ReadOnly Property MainEditor As IEditor

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
            Protected ReadOnly Property Database() As SmartWcmsDatabaseAccessLayer
                Get
                    Static _Database As SmartWcmsDatabaseAccessLayer
                    If _Database Is Nothing Then
                        _Database = New SmartWcmsDatabaseAccessLayer(Me)
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
            Private ReadOnly Property _cammWebManager() As CompuMaster.camm.WebManager.IWebManager Implements ISmartWcmsEditor.cammWebManager
                Get
                    Return Me.cammWebManager
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Is this editor in edit mode?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public MustOverride ReadOnly Property EditModeActive() As Boolean

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

        Public MustInherit Class SmartWcmsEditorCommonBase
            Inherits SmartWcmsEditorBase

#Region "Version tracking"
            Dim _LatestData As Integer
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The pages latest data (RowID)
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	11.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property LatestData() As Integer
                Get
                    Return _LatestData
                End Get
                Set(ByVal Value As Integer)
                    _LatestData = Value
                End Set
            End Property

            Dim _LatestVersion As Integer
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The pages latest version
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	11.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property LatestVersion() As Integer
                Get
                    Return _LatestVersion
                End Get
                Set(ByVal Value As Integer)
                    _LatestVersion = Value
                End Set
            End Property

            Private _CurrentVersionAvailableForReadAccess As Boolean
            Private _CurrentVersionHasChanged As Boolean
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The pages current version
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	11.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property CurrentVersion() As Integer
                Get
                    If _CurrentVersionAvailableForReadAccess = False Then
                        Throw New InvalidOperationException("Page Load must be executed, first")
                    End If
                    Return CType(ViewState("WebEditorXXL.showversion"), Integer)
                End Get
                Set(ByVal Value As Integer)
                    If CType(ViewState("WebEditorXXL.showversion"), Integer) <> Value Then
                        _CurrentVersionHasChanged = True
                    End If
                    ViewState("WebEditorXXL.showversion") = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Only for saving the new value without a "_CurrentVersionHasChanged = True"
            ''' </summary>
            ''' <param name="value"></param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	31.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub CurrentVersionSetWithoutInternalChangeFlag(ByVal value As Integer)
                ViewState("WebEditorXXL.showversion") = value
            End Sub
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Indicate if the viewstate already contains an information about the version which shall be shown
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	31.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function CurrentVersionNotYetDefinedByViewstate() As Boolean
                If ViewState("WebEditorXXL.showversion") Is Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Function
#End Region

#Region " Properties "

            ''' <summary>
            ''' Is this smart editor in one of the EditModes (editor control visible in edit or view mode)?
            ''' </summary>
            ''' <returns></returns>
            Public NotOverridable Overrides ReadOnly Property EditModeActive As Boolean
                Get
                    Return Me.MainEditor.Visible
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Indicates the visibility of the switch to edit mode image button
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     Is needed to make this button only visible if it shall appear
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property EditModeSwitchAvailable() As Boolean
                Get
                    Return Me.ibtnSwitchToEditMode.Visible
                End Get
                Set(ByVal Value As Boolean)
                    Me.ibtnSwitchToEditMode.Visible = Value
                End Set
            End Property

            Private _EditorID As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     An identifier of the editor instance in the current document, by default the ClientID property value
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     This ID is required for support of multiple editor instances on the same page.
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property EditorID() As String
                Get
                    Return _EditorID
                End Get
                Set(ByVal Value As String)
                    _EditorID = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The edit mode as it is defined in the viewstate
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	01.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property EditModeAsDefinedInViewstate() As TriState
                Get
                    If ViewState("WebEditorXXL.EditMode") Is Nothing Then
                        Return TriState.UseDefault
                    ElseIf CType(ViewState("WebEditorXXL.EditMode"), Boolean) = False Then
                        Return TriState.False
                    Else
                        Return TriState.True
                    End If
                End Get
                Set(ByVal Value As TriState)
                    If Value = TriState.True Then
                        ViewState("WebEditorXXL.EditMode") = True
                    ElseIf Value = TriState.False Then
                        ViewState("WebEditorXXL.EditMode") = False
                    Else
                        Throw New ArgumentOutOfRangeException("Value must be true or false")
                    End If
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Is the control editable in general or not?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	01.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Enabled() As Boolean
                Get
                    If ViewState Is Nothing OrElse ViewState("WebEditorXXL.Enabled") Is Nothing Then
                        Return True
                    Else
                        Return CType(ViewState("WebEditorXXL.Enabled"), Boolean)
                    End If
                End Get
                Set(ByVal Value As Boolean)
                    ViewState("WebEditorXXL.Enabled") = Value
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Possible actions requested by the form front-end
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Enum RequestModes As Integer
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Nothing to do
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                NoActionRequested = 0
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Save changes
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                Update = 1
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Create a new version for modification
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                NewVersion = 2
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Save changes and release the whole version
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                Activation = 3
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     Drop the content of the current market
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	02.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                DropCurrentMarketData = 6
                ''' -----------------------------------------------------------------------------
                ''' <summary>
                '''     There is no version data available, but the first version shall be created now
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                ''' <history>
                ''' 	[adminsupport]	07.02.2006	Created
                ''' </history>
                ''' -----------------------------------------------------------------------------
                CreateFirstVersion = 7
            End Enum

            Private _RequestMode As RequestModes
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the requested controls mode
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	09.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property RequestMode() As RequestModes
                Get
                    Return _RequestMode
                End Get
                Set(ByVal Value As RequestModes)
                    _RequestMode = Value
                End Set
            End Property 'MyRequestMode()

            Protected ReadOnly Property IsEditVersionAvailable() As Boolean
                Get
                    Return CountAvailableEditVersions() > 0
                End Get
            End Property
            ''' <summary>
            ''' Counts available edit versions
            ''' </summary>
            ''' <remarks>Based on old IsEditVersionAvailable() Code, slightly modified</remarks>
            ''' <returns></returns>
            Protected ReadOnly Property CountAvailableEditVersions() As Integer
                Get
                    Dim result As Integer = 0
                    Dim myDataTable As DataTable = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID)
                    If myDataTable.Rows.Count > 0 Then
                        'Find highest version
                        Dim MaxVersion As Integer = Integer.MinValue
                        For MyCounter As Integer = 0 To myDataTable.Rows.Count - 1
                            MaxVersion = System.Math.Max(MaxVersion, CType(myDataTable.Rows(MyCounter)("Version"), Integer))
                        Next
                        'If highest version is activated, this is a release version, otherwise it must be an edit version
                        Dim myRows() As DataRow
                        myRows = myDataTable.Select("version=" & MaxVersion)

                        For Each row As DataRow In myRows
                            If CType(myRows(0)("IsActive"), Boolean) = False Then
                                result += 1
                            End If
                        Next

                    End If
                    Return result
                End Get
            End Property


            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Indicates which language shall be opened
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     <list>
            '''         <item>Value &gt;= 0: valid, selected market</item>
            '''         <item>Value = -1 (&lt;0): default initialization value to use current CWM market</item>
            '''     </list>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	28.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property LanguageToShow() As Integer
                Get
                    If ViewState("WebEditorXXL.showlanguage") Is Nothing Then
                        If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                            LanguageToShow = 0
                        Else
                            LanguageToShow = Me.cammWebManager.UI.MarketID
                        End If
                    End If
                    Return CType(ViewState("WebEditorXXL.showlanguage"), Integer)
                End Get
                Set(ByVal Value As Integer)
                    ViewState("WebEditorXXL.showlanguage") = Value
                End Set
            End Property 'LanguageToShow()

            Private _ShowWithEditRights As Boolean = False
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Has the user authorization to change the content and does he see at least the pencil to start editing?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property ShowWithEditRights() As Boolean
                Get
                    Return _ShowWithEditRights
                End Get
                Set(ByVal Value As Boolean)
                    _ShowWithEditRights = Value
                End Set
            End Property 'ShowInEditMode()

            Protected Enum ToolbarSettings As Integer
                None = 0
                EditEditableVersion = 1
                EditNoneEditableVersions = 2
                EditWithValidEditVersion = 3
                NoVersionAvailable = 4
            End Enum

            Private _ToolbarSetting As ToolbarSettings
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Indicates which toolbar shall be used
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Property ToolbarSetting() As ToolbarSettings
                Get
                    Return _ToolbarSetting
                End Get
                Set(ByVal Value As ToolbarSettings)
                    _ToolbarSetting = Value
                End Set
            End Property 'ToolbarSetting()

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Creates the text value for the edit information label which gives you information
            '''     about the currently opened version and it's status
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private ReadOnly Property HtmlCodeCurrentEditInformation() As String
                Get
                    If CurrentVersion = 0 Then
                        If Me.ShowWithEditRights = False Then
                            'No version info available in this case
                            Return String.Empty
                        Else
                            If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                Return "Language/Market - Neutral / " & "<font color=""green"">Predefined default version</font>"
                            Else
                                If LanguageToShow < 0 Then
                                    Throw New Exception("Invalid language/market " & LanguageToShow)
                                End If
                                Dim MarketName As String
                                If LanguageToShow = 0 Then
                                    MarketName = "All unconfigured languages/markets"
                                Else
                                    MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                                End If
                                Return "Language/Market - " & MarketName & " / " & "<font color=""green"">International, predefined default version</font>"
                            End If
                        End If
                    Else
                        Dim activeVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                        If activeVersion = CurrentVersion Then
                            If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                Return "Language/Market - Neutral / " & "<font color=""green"">Released version</font>"
                            Else
                                If LanguageToShow < 0 Then
                                    Throw New Exception("Invalid language/market " & LanguageToShow)
                                End If
                                Dim MarketName As String
                                If LanguageToShow = 0 Then
                                    MarketName = "All unconfigured languages/markets"
                                Else
                                    MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                                End If
                                Return "Language/Market - " & MarketName & " / " & "<font color=""green"">Released version</font>"
                            End If
                        ElseIf activeVersion < CurrentVersion Then
                            If Me.IsEditVersionAvailable Then
                                If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                    Return "Language/Market - Neutral / " & "<font color=""red"">Edit version</font>"
                                Else
                                    If LanguageToShow < 0 Then
                                        Throw New Exception("Invalid language/market " & LanguageToShow)
                                    End If
                                    Dim MarketName As String
                                    If LanguageToShow = 0 Then
                                        MarketName = "All unconfigured languages/markets"
                                    Else
                                        MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                                    End If
                                    Return "Language/Market - " & MarketName & " / " & "<font color=""red"">Edit version</font>"
                                End If
                            Else
                                Throw New Exception("Error: CurrentVersion " & CurrentVersion & " is higher than ActiveVersion " & activeVersion & ", but IsEditVersionAvailable = False!")
                            End If
                        Else
                            If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                Return "Language/Market - Neutral / " & "<font color=""red"">Former version (V" & Me.CurrentVersion & ")</font>"
                            Else
                                If LanguageToShow < 0 Then
                                    Throw New Exception("Invalid language/market " & LanguageToShow)
                                End If
                                Dim MarketName As String
                                If LanguageToShow = 0 Then
                                    MarketName = "All unconfigured languages/markets"
                                Else
                                    MarketName = GetWMLanguageDescriptionByLanguageID(CType(LanguageToShow, Integer))
                                End If
                                Return "Language/Market - " & MarketName & " / " & "<font color=""red"">Former version (V" & Me.CurrentVersion & ")</font>"
                            End If
                        End If
                    End If
                End Get
            End Property
#End Region

#Region " Control specific methods "

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Image button SwitchToEditMode click events
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub ibtnSwitchToEditMode_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtnSwitchToEditMode.Click
                If CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = "" OrElse CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = Me.EditorID Then
                    ViewState("WebEditorXXL.CurrentEditorInEditMode") = Me.EditorID
                    editorMain.Visible = True
                    Me.ibtnSwitchToEditMode.Visible = False
                    EditModeAsDefinedInViewstate = TriState.True
                    'Switch version to edit version!
                    Me.CurrentVersion = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    editorMain.Editable = CanEditCurrentVersion()
                End If
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Adds the controls components to it's form
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	19.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Overrides Sub CreateChildControls()

                lblCurrentEditInformation = New System.Web.UI.WebControls.Label
                lblCurrentEditInformation.Visible = False
                Controls.Add(lblCurrentEditInformation)

                ibtnSwitchToEditMode = New System.Web.UI.WebControls.ImageButton
                ibtnSwitchToEditMode.Visible = False
                ibtnSwitchToEditMode.EnableViewState = False
                ibtnSwitchToEditMode.AlternateText = "Edit"
                ibtnSwitchToEditMode.ImageUrl = "/RadControls/Editor/Img/editor.gif"
                ibtnSwitchToEditMode.ToolTip = "Edit"
                Controls.Add(ibtnSwitchToEditMode)

                lblViewOnlyContent = New System.Web.UI.HtmlControls.HtmlGenericControl("div")
                lblViewOnlyContent.Visible = False
                lblViewOnlyContent.EnableViewState = False
                Controls.Add(lblViewOnlyContent)

                txtHiddenActiveVersion = New System.Web.UI.WebControls.TextBox
                txtHiddenActiveVersion.Visible = False
                Controls.Add(txtHiddenActiveVersion)

                txtHiddenLastVersion = New System.Web.UI.WebControls.TextBox
                txtHiddenLastVersion.Visible = False
                Controls.Add(txtHiddenLastVersion)

                txtRequestedAction = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtRequestedAction.ID = "txtNewVersion"
                txtRequestedAction.Name = "txtNewVersion"
                Controls.Add(txtRequestedAction)

                txtCurrentURL = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtCurrentURL.ID = "txtCurrentURL"
                txtCurrentURL.Name = "txtCurrentURL"
                Controls.Add(txtCurrentURL)

                txtActivate = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtActivate.ID = "txtActivate"
                txtActivate.Name = "txtActivate"
                Controls.Add(txtActivate)

                txtBrowseToMarketVersion = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtBrowseToMarketVersion.ID = "txtRedirUrl"
                txtBrowseToMarketVersion.Name = "txtRedirUrl"
                Controls.Add(txtBrowseToMarketVersion)

                txtLastVersion = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtLastVersion.ID = "txtLastVersion"
                txtLastVersion.Name = "txtLastVersion"
                Controls.Add(txtLastVersion)

                txtCurrentlyLoadedVersion = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtCurrentlyLoadedVersion.ID = "txtCurrentlyLoadedVersion"
                txtCurrentlyLoadedVersion.Name = "txtCurrentlyLoadedVersion"
                Controls.Add(txtCurrentlyLoadedVersion)

                txtEditModeRequested = New System.Web.UI.HtmlControls.HtmlInputHidden
                txtEditModeRequested.ID = "txtEditMode"
                txtEditModeRequested.Name = "txtEditMode"
                Controls.Add(txtEditModeRequested)

                pnlEditorToolbar = New UI.WebControls.Panel()
                pnlEditorToolbar.Visible = False
                Controls.Add(pnlEditorToolbar)


            End Sub    'CreateChildControls()

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Remove any duplicate paths and return the new array with path values with all different meaning
            ''' </summary>
            ''' <param name="paths">Array of several virtual paths</param>
            ''' <param name="singlePath">One virtual path</param>
            ''' <returns>Array without any duplicate, virtual path meanings</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	21.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Function PathsWithoutDuplicateMeaning(ByVal paths As String(), ByVal singlePath As String) As String()
                Dim Result As New ArrayList
                '1st parameter
                For MyCounter As Integer = 0 To paths.Length - 1
                    Dim path As String = Utils.FullyInterpretedVirtualPath(paths(MyCounter))
                    If Not Result.Contains(path) Then
                        Result.Add(path)
                    End If
                Next
                '2nd parameter
                If singlePath <> Nothing Then
                    Dim path As String = Utils.FullyInterpretedVirtualPath(singlePath)
                    If Not Result.Contains(path) Then
                        Result.Add(path)
                    End If
                End If
                'Return values
                Return CType(Result.ToArray(GetType(String)), String())
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Detects which toolbar shall appear
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	23.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub GetToolbarSetting()
                If CurrentVersion = 0 AndAlso Me.IsEmptyInnerHtml Then
                    ToolbarSetting = ToolbarSettings.NoVersionAvailable
                Else
                    Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    If System.Math.Max(1, CurrentVersion) <= ReleasedVersion AndAlso Not IsEditVersionAvailable Then
                        ToolbarSetting = ToolbarSettings.EditNoneEditableVersions
                    ElseIf System.Math.Max(1, CurrentVersion) <= ReleasedVersion AndAlso IsEditVersionAvailable Then
                        ToolbarSetting = ToolbarSettings.EditWithValidEditVersion
                    ElseIf System.Math.Max(1, CurrentVersion) > ReleasedVersion Then
                        ToolbarSetting = ToolbarSettings.EditEditableVersion
                    End If
                End If
            End Sub    'GetToolbarSetting()

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Raises the standard 404 error message as defined right in the IIS
            ''' </summary>
            ''' <param name="message">An error message with some details on the missing item</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	15.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Overridable Sub Raise404(ByVal message As String)

                If Not cammWebManager Is Nothing AndAlso cammWebManager.DebugLevel >= CompuMaster.camm.WebManager.WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Throw New HttpException(404, message)
                Else
                    Throw New HttpException(404, "File not found!")
                End If

                'HttpContext.Current.Response.Clear()
                'HttpContext.Current.Response.StatusCode = 404
                'HttpContext.Current.Response.End()
            End Sub    'Raise404()

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     If the inner html property contains spaces, tabs, CR or LF chars only, then this will be indicated as True, otherwise there is some real text/content and this method will return False
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function IsEmptyInnerHtml() As Boolean
                If Me._InnerHtml Is Nothing Then
                    Return True
                Else
                    For MyCounter As Integer = 0 To Me._InnerHtml.Length - 1
                        Select Case Me._InnerHtml.Chars(MyCounter)
                            Case " "c, ControlChars.Lf, ControlChars.Cr, ControlChars.Tab
                            Case Else
                                Return False
                        End Select
                    Next
                    Return True
                End If
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Detect the language/market for first-time-data-creation in edit-requests
            ''' </summary>
            ''' <returns>An integer ID of the market/language</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	09.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function LookupDefaultMarketBasedOnMarketLookupMode() As Integer
                Dim Result As Integer
                Select Case Me.MarketLookupMode
                    Case MarketLookupModes.BestMatchingLanguage
                        Result = Me.cammWebManager.UI.MarketID
                    Case MarketLookupModes.Language
                        Result = Me.cammWebManager.UI.LanguageID
                    Case MarketLookupModes.Market
                        Result = Me.cammWebManager.UI.MarketID
                    Case MarketLookupModes.SingleMarket
                        Result = 0
                    Case Else
                        Throw New Exception("Invalid market lookup mode")
                End Select
                Return Result
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Change the LanguageToShow variable to an existing value when the current one is not valid
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub LookupMatchingMarketDataBasedOnExistingContent()
                'Switch to another language which is there - if required only
                Dim myLangs() As Integer = Database.AvailableMarketsInData(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.CurrentVersion)
                Dim Found As Boolean = False
                For MyCounter As Integer = 0 To myLangs.Length - 1
                    If Me.LanguageToShow = myLangs(MyCounter) Then
                        Found = True
                        Exit For
                    End If
                Next
                If Not Found Then
                    'Current LanguageToShow value is invalid
                    If myLangs.Length = 0 Then
                        'Default element
                        Me.LanguageToShow = Me.LookupDefaultMarketBasedOnMarketLookupMode
                    Else
                        'First element
                        Me.LanguageToShow = myLangs(0)
                    End If
                    'FillEditor must be enforced now by setting the following flag
                    _CurrentVersionHasChanged = True
                End If
            End Sub


            Private _AlternativeDataMarkets As Integer() = New Integer() {1, 10000}
            ''' <summary>
            ''' Alternative markets which might contain data if the typical market doesn't contain any released data
            ''' </summary>
            ''' <returns></returns>
            <System.ComponentModel.TypeConverter(GetType(IntegerArrayConverter))> Public Property AlternativeDataMarkets As Integer()
                Get
                    Return _AlternativeDataMarkets
                End Get
                Set(value As Integer())
                    _AlternativeDataMarkets = value
                End Set
            End Property


            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Detect the LanguageToShow for view-only requests
            ''' </summary>
            ''' <param name="returnCode404InCaseOfMissingData">True raises a 404 error when it can't be looked up</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	09.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub LookupMatchingMarketDataForReleasedContent(ByVal returnCode404InCaseOfMissingData As Boolean)
                Dim myLangs() As Integer = Database.AvailableMarketsInData(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID))
                Dim IsLangExisting As Boolean = False
                Dim ErrorMessageInCaseOfMissingData As String = Nothing
                If Me.MarketLookupMode = MarketLookupModes.BestMatchingLanguage Then
                    ErrorMessageInCaseOfMissingData = "No matching market version found for this file"
                    For Each myLang As Integer In myLangs
                        If myLang = Me.cammWebManager.UI.MarketID Then
                            IsLangExisting = True
                            LanguageToShow = myLang
                        End If
                    Next
                    If Not IsLangExisting Then
                        For Each myLang As Integer In myLangs
                            If myLang = Me.cammWebManager.UI.LanguageID Then
                                IsLangExisting = True
                                LanguageToShow = myLang
                            End If
                        Next
                    End If
                    If Not IsLangExisting Then
                        For Each myLang As Integer In Me.AlternativeDataMarkets
                            For Each alternativeLang As Integer In AlternativeDataMarkets
                                If myLang = alternativeLang Then
                                    IsLangExisting = True
                                    LanguageToShow = myLang
                                End If
                            Next
                        Next
                    End If
                    If Not IsLangExisting Then
                        For Each myLang As Integer In Me.AlternativeDataMarkets
                            If myLang = 0 Then
                                IsLangExisting = True
                                LanguageToShow = 0
                            End If
                        Next
                    End If
                ElseIf Me.MarketLookupMode = MarketLookupModes.Language Then
                    ErrorMessageInCaseOfMissingData = "No language version found for for this file"
                    For Each myLang As Integer In myLangs
                        If myLang = Me.cammWebManager.UI.MarketID Then
                            IsLangExisting = True
                            LanguageToShow = myLang
                        End If
                    Next
                    If Not IsLangExisting Then
                        For Each myLang As Integer In myLangs
                            If myLang = Me.cammWebManager.UI.LanguageID Then
                                IsLangExisting = True
                                LanguageToShow = myLang
                            End If
                        Next
                    End If
                    If Not IsLangExisting Then
                        For Each myLang As Integer In myLangs
                            If myLang = 0 Then
                                IsLangExisting = True
                                LanguageToShow = 0
                            End If
                        Next
                    End If
                ElseIf Me.MarketLookupMode = MarketLookupModes.Market Then
                    'Check if MarketLookupModeLangID is contained by the document
                    ErrorMessageInCaseOfMissingData = "No market version found for for this file"
                    For Each myLang As Integer In myLangs
                        If myLang = Me.cammWebManager.UI.MarketID Then
                            IsLangExisting = True
                            LanguageToShow = Me.cammWebManager.UI.MarketID
                        End If
                    Next
                    If Not IsLangExisting Then
                        For Each myLang As Integer In myLangs
                            If myLang = 0 Then
                                IsLangExisting = True
                                LanguageToShow = 0
                            End If
                        Next
                    End If
                ElseIf Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                    ErrorMessageInCaseOfMissingData = "No version available for this file"
                    For Each myLang As Integer In myLangs
                        If myLang = 0 Then
                            IsLangExisting = True
                            LanguageToShow = 0
                        End If
                    Next
                End If
                If Not IsLangExisting AndAlso Not Me.IsEmptyInnerHtml Then
                    Throw New UseInnerHtmlException
                ElseIf Not IsLangExisting Then
                    If returnCode404InCaseOfMissingData Then
                        Raise404(ErrorMessageInCaseOfMissingData)
                    End If
                End If
            End Sub    'GetAllowedLanguageDefinedByUserProfile()

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Generates a url conform string to pass trough url as parameter for later usement
            ''' </summary>
            ''' <param name="sourceString"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	17.11.2005	Created
            '''		[link]		07.03.2007	Update due to changes in base64 encryption in .net 2.0
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Function EncryptAStringForUrlUsement(ByVal sourceString As String) As String
                Dim result As String = Nothing
                Try
                    Dim myCryptingEngine As CompuMaster.camm.WebManager.ICrypt = New CompuMaster.camm.WebManager.TripleDesBase64Encryption
                    result = myCryptingEngine.CryptString(sourceString)
                Catch ex As Exception
                    Throw New Exception("Error while trying to encrypt an url parameter. '" & ex.Message & "'")
                End Try
                Return HttpUtility.UrlEncode(result)
            End Function    'EncryptAStringForUrlUsement(ByVal sourceString As String) As String

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Update an already existing document in given language or create a new language for the document, depends on several parameters
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	09.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub DoUpdateAction()
                If CurrentVersion <> 0 Then
                    'Create a new version or update an older one
                    If LanguageToShow >= 0 Then
                        SaveEditorContent(ContentOfServerID, DocumentID, Me.EditorID, LanguageToShow, editorMain.Html, cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                    Else
                        SaveEditorContent(ContentOfServerID, DocumentID, Me.EditorID, cammWebManager.UI.MarketID, editorMain.Html, cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                    End If
                End If
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Creates a new pages version
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	09.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub CreateANewPageVersion()
                'Check for clicked savebutton with update instructions without an already existing version in the given language
                'Case new version
                Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                Dim MaxVersion As Integer = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                If ReleasedVersion <> 0 AndAlso MaxVersion <> 0 Then
                    If MaxVersion - ReleasedVersion > 0 Then
                        'Invalid request
                        Throw New Exception("Invalid request (HighestVersion = " & MaxVersion & "; ActiveVersion=" & ReleasedVersion & ")")
                    Else
                        If LanguageToShow >= 0 Then
                            Database.CopyEditorVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, ReleasedVersion)
                        Else
                            'Invalid request
                            Throw New Exception("LanguageToShow hasn't been initialized")
                        End If
                    End If
                End If
                If CurrentVersion = 0 Then
                    'Create the first version for this document
                    SaveEditorContent(ContentOfServerID, DocumentID, Me.EditorID, LanguageToShow, Me.InnerHtml, cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                End If

            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Returns the languages description in english as string
            ''' </summary>
            ''' <param name="ID">The language id whose description you want to get</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	29.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Function GetWMLanguageDescriptionByLanguageID(ByVal ID As Integer) As String
                Dim myDataTable As DataTable = CType(HttpContext.Current.Application("WebManager.ActiveLanguages"), DataTable)
                If myDataTable Is Nothing Then
                    myDataTable = Database.ActiveMarketsInWebManager()
                End If
                For Each myDataRow As DataRow In myDataTable.Rows
                    If CType(myDataRow(0), Integer) = ID Then
                        Return CType(myDataRow(1), String)
                    End If
                Next
                Return Nothing
                'Returns it's description in english
            End Function    'GetWMLanguageDescriptionByLanguageID(ByVal ID As Integer)
#End Region

#Region " MultipleEditorPerPage Communications "
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Is there at least one SmartWcms control in edit mode?
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            Private Function FindSWcmsControlsInEditModeOnThisPage() As Boolean
                Dim Result As Boolean
                Dim EditorControls As New ArrayList
                FindSWcmsControlsOnThisPage(EditorControls, Me.Page.Controls)
                For MyCounter As Integer = 0 To EditorControls.Count - 1
                    If EditorControls(MyCounter) Is Me Then
                        'do nothing
                    ElseIf Me.EditModeActive = True Then
                        Result = True
                    End If
                Next
                Return Result
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Walk recursive through all children controls and collect all SmartWcms items
            ''' </summary>
            ''' <param name="results">An arraylist which will contain all positive matches</param>
            ''' <param name="controlsCollection">The control collection which shall be browsed</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub FindSWcmsControlsOnThisPage(ByVal results As ArrayList, ByVal controlsCollection As System.Web.UI.ControlCollection)
                For MyCounter As Integer = 0 To controlsCollection.Count - 1
                    If Me.GetType.IsInstanceOfType(controlsCollection(MyCounter)) Then
                        results.Add(controlsCollection(MyCounter))
                    ElseIf controlsCollection(MyCounter).Controls.Count > 0 Then
                        FindSWcmsControlsOnThisPage(results, controlsCollection(MyCounter).Controls)
                    End If
                Next
            End Sub
#End Region


            Private _RemoveServerNameFromLinks As Boolean = True
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' Remove all occurances in links of the current server name, e. g. http://www.yourcompany.com
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[wezel]	10.11.2007	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property RemoveServerNameFromLinks() As Boolean
                Get
                    Return _RemoveServerNameFromLinks
                End Get
                Set(ByVal Value As Boolean)
                    _RemoveServerNameFromLinks = Value
                End Set
            End Property

            Private Function RemoveCurrentServerNameFromLinks(ByVal html As String) As String
                If html = Nothing Then
                    Return html
                ElseIf RemoveServerNameFromLinks Then
                    'Find occurances of ="http://www.yourcompany.com 
                    'or =http://www.yourcompany.com 
                    'and replace them by =" or = _
                    Return html.Replace("=""" & Me.cammWebManager.CurrentServerInfo.ServerURL(), "=""").Replace("=" & Me.cammWebManager.CurrentServerInfo.ServerURL(), "=")
                Else
                    Return html
                End If
            End Function

            Protected Friend Sub SaveEditorContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal content As String, ByVal modifiedBy As Long)
                Me.Database.SaveEditorContent(serverID, url, editorID, marketID, RemoveCurrentServerNameFromLinks(content), modifiedBy)
            End Sub

            Private _EnableCache As Boolean = True
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Use HttpCache to boost the performance by decreasing the number of required database queries
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property EnableCache() As Boolean
                Get
                    Return _EnableCache
                End Get
                Set(ByVal Value As Boolean)
                    _EnableCache = Value
                End Set
            End Property

            Private _InnerHtml As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The static inner HTML code is the default HTML when the database doesn't contain any released content
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     If there is no released content from database and this inner HTML contains whitespaces only, you'll run into a 404 HTTP error page (file not found).
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	02.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public ReadOnly Property InnerHtml() As String
                Get
                    Return _InnerHtml
                End Get
            End Property

#Const DebugMode = False

            Private ReadOnly Property editorMain As IEditor
                Get
                    Return Me.MainEditor
                End Get
            End Property
            Protected txtHiddenActiveVersion As System.Web.UI.WebControls.TextBox
            Protected txtHiddenLastVersion As System.Web.UI.WebControls.TextBox
            Protected txtRequestedAction As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected txtCurrentURL As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected txtActivate As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected txtBrowseToMarketVersion As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected txtLastVersion As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected txtCurrentlyLoadedVersion As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected txtEditModeRequested As System.Web.UI.HtmlControls.HtmlInputHidden
            Protected lblCurrentEditInformation As System.Web.UI.WebControls.Label
            Protected lblViewOnlyContent As System.Web.UI.HtmlControls.HtmlGenericControl
            Protected pnlEditorToolbar As System.Web.UI.WebControls.Panel
            Protected WithEvents ibtnSwitchToEditMode As System.Web.UI.WebControls.ImageButton

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The requested edit mode for the processing of this page request
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     Typically, this value is predefined to a default or will be changed by user via form
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	31.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Property RequestedEditMode() As TriState
                Get
                    If txtEditModeRequested.Value = "false" Then
                        Return TriState.False
                    ElseIf txtEditModeRequested.Value = "true" Then
                        Return TriState.True
                    Else
                        Return TriState.UseDefault
                    End If
                End Get
                Set(ByVal Value As TriState)
                    If Value = TriState.False Then
                        txtEditModeRequested.Value = "false"
                    ElseIf Value = TriState.True Then
                        txtEditModeRequested.Value = "true"
                    Else
                        Throw New ArgumentOutOfRangeException("Value must be true or false")
                    End If
                End Set
            End Property

#Region "Page events"

            Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

                'Prevent a second execution
                Static AlreadyRun As Boolean
                If AlreadyRun = True Then
                    Exit Sub
                Else
                    AlreadyRun = True
                End If

                LanguageToShow = 0

                'Initialze the editors id for internal usage right now
                If Me.ID = "" Then
                    Me.ID = Me.ClientID
                    If Me.EditorID = "" Then
                        Me.EditorID = "Standard.Editor"
                    End If
                Else
                    Me.EditorID = Me.ID
                End If

                'Check for needed SecurityObjectEditMode
                RaiseEvent BeforeSecurityCheck()

                Dim IsUserAuthorized As Boolean = False
                If Me.SecurityObjectEditMode <> "" Then
                    If Me.cammWebManager.IsUserAuthorized(Me.SecurityObjectEditMode, True, True) Then
                        'open the currently released version in ui language in edit mode
                        IsUserAuthorized = True
                    End If
                End If
                Me.ShowWithEditRights = IsUserAuthorized

                'Checks if the control has child elements
                'if none available execute CreateChildControls()
                EnsureChildControls()

                'Enable/disable the viewstate dependent on the rights
                If Me.ShowWithEditRights Then
                    'View state for controls
                    Me.editorMain.EnableViewState = True
                    Me.lblViewOnlyContent.EnableViewState = True
                Else
                    'View state for controls
                    Me.editorMain.EnableViewState = False
                    Me.lblViewOnlyContent.EnableViewState = False
                End If

                'Check for sub controls - allowed subcontrols are either none or 1 literal control
                Dim ForeignControls As New ArrayList
                For MyCounter As Integer = 0 To Me.Controls.Count - 1
                    Dim compareControl As UI.Control = Me.Controls(MyCounter)
                    If compareControl Is editorMain OrElse _
                                    compareControl Is txtHiddenActiveVersion OrElse _
                                    compareControl Is txtHiddenLastVersion OrElse _
                                    compareControl Is txtRequestedAction OrElse _
                                    compareControl Is txtCurrentURL OrElse _
                                    compareControl Is txtActivate OrElse _
                                    compareControl Is txtBrowseToMarketVersion OrElse _
                                    compareControl Is txtLastVersion OrElse _
                                    compareControl Is txtCurrentlyLoadedVersion OrElse _
                                    compareControl Is txtEditModeRequested OrElse _
                                    compareControl Is lblCurrentEditInformation OrElse _
                                    compareControl Is lblViewOnlyContent OrElse _
                                    compareControl Is ibtnSwitchToEditMode OrElse _
                                    compareControl Is pnlEditorToolbar Then

                        'All is fine - this is an internally managed control
                    Else
                        'This is a foreign control added by the user
                        ForeignControls.Add(compareControl)
                    End If
                Next
                If ForeignControls.Count = 1 Then
                    For MyCounter As Integer = 0 To ForeignControls.Count - 1
                        If Not GetType(UI.LiteralControl).IsInstanceOfType(ForeignControls(MyCounter)) Then
                            Throw New Exception("This control """ & ForeignControls(MyCounter).GetType.ToString & """ only supports literal text and no active element (runat=server)")
                        Else
                            Me._InnerHtml = CType(ForeignControls(MyCounter), UI.LiteralControl).Text
                            Me.Controls.Remove(CType(ForeignControls(MyCounter), UI.Control))
                        End If
                    Next
                ElseIf ForeignControls.Count > 1 Then
                    Dim strBuilder As New System.Text.StringBuilder
                    For MyCounter As Integer = 0 To ForeignControls.Count - 1
                        If Not MyCounter = 0 Then strBuilder.Append(","c)
                        Dim MyControl As UI.Control = CType(ForeignControls(MyCounter), UI.Control)
                        strBuilder.Append(MyControl.GetType.ToString)
                    Next
                    Throw New Exception("Too many subcontrols (" & ForeignControls.Count & " elements: " & strBuilder.ToString & "), there is only allowed 1 literal control")
                Else 'ForeignControls.Count = 0
                    'No controls are fine - do nothing here
                End If

                'Get the language which shall be opened if this is a view only request
                If Not ShowWithEditRights OrElse (ShowWithEditRights AndAlso Page.IsPostBack = False) Then
                    'Not authorized or authorized but first page access
                    If Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) <> 0 Then
                        'Check workflow for the documents opening language
                        Try
                            LookupMatchingMarketDataForReleasedContent(Not ShowWithEditRights)
                        Catch ex As UseInnerHtmlException
                            'Use inner html 
                            lblViewOnlyContent.InnerHtml = Me.InnerHtml
                            If ShowWithEditRights Then
                                Me.editorMain.Html = Me.InnerHtml
                            End If
                        Catch
                            Throw
                        End Try
                        If Me.LanguageToShow < 0 Then 'ShowWithEditRights=True is the only possibility where the LanguageToShow has kept at value -1
                            Me.LanguageToShow = Me.LookupDefaultMarketBasedOnMarketLookupMode
                        End If
                    ElseIf Not Me.IsEmptyInnerHtml Then
                        'Use inner html 
                        lblViewOnlyContent.InnerHtml = Me.InnerHtml
                        If ShowWithEditRights Then
                            Me.editorMain.Html = Me.InnerHtml
                        End If
                    ElseIf ShowWithEditRights AndAlso Page.IsPostBack = False Then
                        'User is authorized to edit an empty document, too
                        CurrentVersionSetWithoutInternalChangeFlag(0)
                    Else
                        'In this case there is no view version available
                        Raise404("No file version available for unauthorized acces")
                    End If
                End If
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     This event will happen while Page_Init and allows application developers to define the security object name just-in-time
            ''' </summary>
            ''' <remarks>
            '''     This early execution of the security check is required to decide about required viewstate of this control's data controls. (After Page_Init, the viewstate wouldn't be loaded any more.)
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	31.01.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Event BeforeSecurityCheck()

            Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

                'First, reset visibilities after viewstate-loading
                Me.lblViewOnlyContent.Visible = False
                Me.editorMain.Visible = False

                'CurrentVersion must be initialized before the JavaScriptRegistration method gets fired
                If Me.ShowWithEditRights Then

                    'ViewMode first implementation
                    If Not Me.Page.IsPostBack Then
                        'First page access
                        If EditModeAsDefinedInViewstate = TriState.UseDefault Then
                            Me.editorMain.Visible = False
                            Me.lblCurrentEditInformation.Visible = False
                            Me.lblViewOnlyContent.Visible = True
                            Me.lblViewOnlyContent.InnerHtml = "<br>" & Me.editorMain.Html
                            EditModeAsDefinedInViewstate = TriState.False
                            Me.RequestedEditMode = TriState.False
                        End If
                        If EditModeAsDefinedInViewstate = TriState.False AndAlso Me.RequestedEditMode = TriState.False Then
                            Me.editorMain.Editable = False
                            Me.ibtnSwitchToEditMode.Visible = True
                            Me.lblCurrentEditInformation.Visible = False
                            Me.lblViewOnlyContent.Visible = True
                            Me.lblViewOnlyContent.InnerHtml = "<br>" & Me.editorMain.Html
                            Me.RequestedEditMode = TriState.True
                        End If
                    Else 'Me.Page.IsPostBack Then
                        'If page has been refreshed (gets fired in nearly every javascript event), set the editmode to true
                        If EditModeAsDefinedInViewstate = TriState.False AndAlso Me.RequestedEditMode = TriState.True Then
                            Me.ibtnSwitchToEditMode.Visible = True
                            editorMain.Visible = False
                            editorMain.Editable = False
                        ElseIf EditModeAsDefinedInViewstate = TriState.False AndAlso Me.RequestedEditMode = TriState.False Then
                            Me.editorMain.Editable = False
                            If CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = Me.EditorID Then
                                Me.ibtnSwitchToEditMode.Visible = True
                                Me.RequestedEditMode = TriState.True
                                EditModeAsDefinedInViewstate = TriState.True
                            End If
                        ElseIf EditModeAsDefinedInViewstate = TriState.True AndAlso Me.RequestedEditMode = TriState.True Then
                            editorMain.Visible = True
                            editorMain.Editable = True
                            ibtnSwitchToEditMode.Visible = False
                        ElseIf EditModeAsDefinedInViewstate = TriState.True AndAlso Me.RequestedEditMode = TriState.False Then
                            editorMain.Visible = False
                            editorMain.Editable = False
                            If CType(ViewState("WebEditorXXL.CurrentEditorInEditMode"), String) = Me.EditorID Then
                                ibtnSwitchToEditMode.Visible = True
                            End If
                        End If
                    End If

                    'Version/language browsing filled the txtRedirUrl field
                    If Me.Page.IsPostBack AndAlso txtBrowseToMarketVersion.Value <> "" Then
                        'Example: txtBrowseToMarketVersion.Value = "1;0"
                        Dim myDocInfo() As String = Split(txtBrowseToMarketVersion.Value, ";")
                        Dim RequestedVersion As Integer = Integer.Parse(myDocInfo(0))
                        Dim RequestedMarket As Integer = Integer.Parse(myDocInfo(1))
                        Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                        If RequestedVersion > ReleasedVersion Then
                            'Yes, it's sure that we are in an edit version, currently
                            Dim PreviouslyShownContent As String = Me.editorMain.Html
                            'Deactivated by JW until this feature is requested (but this code might be already stable) - 20060218
                            ''Save the old market data when that was the edit version and it has changed
                            'If Me.CurrentVersion > Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) AndAlso PreviouslyShownContent <> Me.Database.ReadContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow, Me.CurrentVersion) Then
                            '    Me.Database.SaveEditorContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow, PreviouslyShownContent, Me.cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous))
                            'End If
                            editorMain.Editable = True
                        Else
                            editorMain.Editable = IsEditableWhenBrowsingVersions()
                        End If
                        CurrentVersion = RequestedVersion
                        If Not LanguageToShow = RequestedMarket Then
                            'Only when it has changed, apply the change 
                            LanguageToShow = RequestedMarket
                            'FillEditor must be enforced now by setting the following flag
                            _CurrentVersionHasChanged = True
                        End If
                        txtBrowseToMarketVersion.Value = "" 'Reset the value
                        editorMain.Visible = True
                        ibtnSwitchToEditMode.Visible = False
                    End If

                    If CurrentVersionNotYetDefinedByViewstate() Then
                        'View-mode release version
                        Dim ActiveVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                        If Not Me.IsEmptyInnerHtml AndAlso ActiveVersion = 0 Then
                            CurrentVersionSetWithoutInternalChangeFlag(0)
                        ElseIf ActiveVersion > 0 Then
                            Me.CurrentVersionSetWithoutInternalChangeFlag(ActiveVersion)
                        Else
                            'ActiveVersion = 0, but InnerHtml is empty
                            Throw New Exception("Unexpected decision workflow of operation")
                            'Me.CurrentVersionSetWithoutInternalChangeFlag(GetHighestAvailableEditorControlVersion(DocumentID))
                        End If
                    End If
                    If Not Page.IsPostBack Then
                        Me._CurrentVersionHasChanged = True
                    End If
                    _CurrentVersionAvailableForReadAccess = True

                    'Logical data verification
                    Dim myIsValidVersionRequest As Boolean = False
                    If Not Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) < CurrentVersion Then
                        myIsValidVersionRequest = True
                    End If
                    If myIsValidVersionRequest = False Then
                        Me.cammWebManager.Log.Write("SmartWcms:<br> - WebEditor<br>There was an invalid request blocked by the security.")
                        Raise404("Invalid data found: the highest document version is lesser than the current version of this file")
                    End If

                    'Init of the image upload folder to get all the files who 
                    'are already uploaded, the global and control dependent ones

                    If Me.Page.IsPostBack Then
                        'Detects the controls internal request mode
                        CheckForRequestMode()

                        If RequestMode = RequestModes.Activation Then
                            'Activation actions = Save + Release
                            'first save
                            DoUpdateAction()
                            'then release
                            Database.ReleaseLatestVersion(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                            'The cache data has to be refreshed
                            ClearCache()

                            Me.editorMain.Editable = CanEditCurrentVersion()
                            'This redirect is necessary because of the javascript for versionbrowsing used by the RadEditor
                        ElseIf RequestMode = RequestModes.Update Then
                            'Do update or create document in new language action will be triggered here
                            DoUpdateAction()
                            'The cache data doesn't need to be refreshed since released data hasn't changed
                        ElseIf RequestMode = RequestModes.NewVersion Then
                            'Create a new Version right here
                            CreateANewPageVersion()
                            CurrentVersion = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                            'The cache data doesn't need to be refreshed since released data hasn't changed
                        ElseIf RequestMode = RequestModes.CreateFirstVersion Then
                            Dim firstContent As String = ""
                            If Me.InnerHtml <> Nothing Then
                                'Create 1st version from inner html
                                firstContent = Me.InnerHtml
                            End If
                            If LanguageToShow < 0 Then
                                LanguageToShow = Me.LookupDefaultMarketBasedOnMarketLookupMode
                            End If
                            If LanguageToShow >= 0 Then
                                Me.SaveEditorContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow, firstContent, Me.cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                            Else
                                Me.SaveEditorContent(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LookupDefaultMarketBasedOnMarketLookupMode, firstContent, Me.cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous))
                            End If
                            CurrentVersion = 1
                            'The cache data doesn't need to be refreshed since released data hasn't changed
                        ElseIf RequestMode = RequestModes.DropCurrentMarketData Then
                            'Drop the current market's data
                            Me.Database.RemoveMarketFromEditVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.LanguageToShow)
                            'The cache data doesn't need to be refreshed since released data hasn't changed
                        End If

                    End If
                Else
                    'View mode
                    _CurrentVersionAvailableForReadAccess = True
                End If

            End Sub

            Private Sub PagePreRender(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
                'Proceeds the standard content loading for the editor control
                StandardLoading()

#If DebugMode Then
                CreateDebugOutput("PreRender Start")
#End If

                'Make the invalid edit buttons invisible
                If Me.ShowWithEditRights Then

                    'Update visibility of main controls
                    If Me.Enabled = False OrElse FindSWcmsControlsInEditModeOnThisPage() Then
                        Me.EditModeSwitchAvailable = False
                        Me.editorMain.Visible = False
                        Me.ibtnSwitchToEditMode.Visible = False
                        Me.lblViewOnlyContent.Visible = True
                        Me.lblViewOnlyContent.InnerHtml = "<br>" & Me.editorMain.Html
                        Me.lblCurrentEditInformation.Visible = False
                    ElseIf Me.editorMain.Visible = True Then
                        Me.ibtnSwitchToEditMode.Visible = False
                    ElseIf Me.editorMain.Visible = False Then
                        Me.ibtnSwitchToEditMode.Visible = True
                        Me.lblCurrentEditInformation.Visible = False
                        Me.lblViewOnlyContent.Visible = True
                        Me.lblViewOnlyContent.InnerHtml = "<br>" & editorMain.Html
                    End If

                    'Shows which version and language is currently opened
                    lblCurrentEditInformation.Visible = Me.editorMain.Visible
                    Me.lblCurrentEditInformation.ForeColor = System.Drawing.Color.Black
                    Me.lblCurrentEditInformation.Font.Bold = True
                    If Trim(HtmlCodeCurrentEditInformation) = Nothing Then
                        Me.lblCurrentEditInformation.Text = Nothing
                    Else
                        Me.lblCurrentEditInformation.Text = HtmlCodeCurrentEditInformation & "<br>"
                    End If

                    'Initialize ToolbarSetting who indicates which toolbar shall be used
                    GetToolbarSetting()
                    PagePreRender_InitializeToolbar()

                    'Prepare JavaScripts and register them if necessary
                    If Me.editorMain.Editable = True Then
                        'define this temporary helper var with the version history HTML String
                        'Registers some JavaScripts needed for the editors toolbar
                        PagePreRender_JavaScriptRegistration()
                    End If

                End If

#If DebugMode Then
                CreateDebugOutput("PreRender End")
                WriteDebugOutput()
#End If

            End Sub

            Protected MustOverride Sub PagePreRender_JavaScriptRegistration()
            Protected MustOverride Sub PagePreRender_InitializeToolbar()

            Protected MustOverride Function IsEditableWhenBrowsingVersions() As Boolean

            Protected MustOverride Function CanEditCurrentVersion() As Boolean



#End Region

#Region "Debug output methods"
#If DebugMode Then
            Private _DebugOutput As New System.Text.StringBuilder
            Private Sub WriteDebugOutput()
                If Me.cammWebManager.DebugLevel < WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Exit Sub
                End If
                If _DebugOutput.Length > 0 Then
                    HttpContext.Current.Response.Write("<p>RawUrl=" & HttpContext.Current.Server.HtmlEncode(HttpContext.Current.Request.RawUrl))
                    HttpContext.Current.Response.Write("<table><tr>")
                    HttpContext.Current.Response.Write(_DebugOutput.ToString)
                    HttpContext.Current.Response.Write("</tr></table>")
                End If
            End Sub

            Private Sub WriteDebugOutputDirectly(ByVal text As String)
                If Me.cammWebManager.DebugLevel < WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Exit Sub
                End If
                HttpContext.Current.Response.Write(text)
            End Sub

            Private Sub CreateDebugOutput(ByVal stepLocation As String)
                If Me.cammWebManager.DebugLevel < WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Exit Sub
                End If
                _DebugOutput.Append("<td>")
                _DebugOutput.Append("<h4>" & stepLocation & "</h4>")
                _DebugOutput.Append("ShowInEditMode=" & Me.ShowWithEditRights & "<br>")
                _DebugOutput.Append("editorMain.Visible=" & Me.editorMain.Visible & "<br>")
                _DebugOutput.Append("txtEditModeRequested.Value=" & Me.txtEditModeRequested.Value & "<br>")
                _DebugOutput.Append("txtRequestedAction.Value=" & Me.txtRequestedAction.Value & "<br>")
                _DebugOutput.Append("RequestMode=" & Me.RequestMode.ToString & "<br>")
                _DebugOutput.Append("EditModeSwitchAvailable=" & Me.EditModeSwitchAvailable & "<br>")
                _DebugOutput.Append("EditModeActive=" & Me.EditModeActive & "<br>")
                _DebugOutput.Append("CurrentMarket=" & Me.LanguageToShow & "<br>")
                _DebugOutput.Append("CurrentVersion=" & Me.CurrentVersion & "<br>")
                _DebugOutput.Append("txtCurrentlyLoadedVersion=" & Me.txtCurrentlyLoadedVersion.Value & "<br>")
                _DebugOutput.Append("txtHiddenActiveVersion=" & Me.txtHiddenActiveVersion.Text & "<br>")
                _DebugOutput.Append("_ToolbarSetting=" & Me._ToolbarSetting & "<br>")
                _DebugOutput.Append("lblViewOnlyContent.Visible=" & Me.lblViewOnlyContent.Visible & "<br>")
                _DebugOutput.Append("editorMain.Visible=" & Me.editorMain.Visible & "<br>")
                _DebugOutput.Append("</td>")
            End Sub
#End If
#End Region

#Region "Anything else "





            Public Function GetAvailableLanguagesDataTable() As DataTable
                Dim myLanguages As DataTable = Nothing
                Select Case Me.MarketLookupMode
                    Case MarketLookupModes.BestMatchingLanguage, MarketLookupModes.Market
                        'Show languages and markets
                        myLanguages = CompuMaster.camm.WebManager.Tools.Data.DataTables.GetDataTableClone(Database.ActiveMarketsInWebManager())
                        'Remove languages not available when in old or released version
                        If CurrentVersion <= Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) Then
                            RemoveLanguageItemsNotInAllowedList(myLanguages, Me.Database.AvailableMarketsInData(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.CurrentVersion))
                        End If
                    Case MarketLookupModes.Language
                        'Only show languages, no markets
                        myLanguages = CompuMaster.camm.WebManager.Tools.Data.DataTables.GetDataTableClone(Database.ActiveMarketsInWebManager(), "AlternativeLanguage IS NULL")
                        'Remove languages not available when in old or released version
                        If CurrentVersion <= Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) Then
                            RemoveLanguageItemsNotInAllowedList(myLanguages, Me.Database.AvailableMarketsInData(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.CurrentVersion))
                        End If
                    Case MarketLookupModes.SingleMarket
                        'Do nothing special
                    Case Else
                        Throw New Exception("Invalid market lookup mode " & Me.MarketLookupMode)
                End Select
                Return myLanguages
            End Function

            Public Structure AvailableLanguage
                Public id As Integer
                Public languageDescriptionEnglish As String
                Public available As Boolean
            End Structure

            ''' <summary>
            ''' Returns an array of available languages
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>Based on code by Swiercz, extracted into this function to not mix it with HTML code</remarks>
            Public Function GetAvailableLanguages() As AvailableLanguage()
                Dim result As AvailableLanguage() = Nothing
                Dim myLanguages As DataTable = GetAvailableLanguagesDataTable()
                If Not myLanguages Is Nothing Then
                    Dim myDataRows() As DataRow = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID).Select("Version=" & CurrentVersion)
                    Dim myDocumentsGivenLanguages() As Integer = Nothing
                    Dim myIndex As Integer = 0

                    For Each myDataRow As DataRow In myDataRows
                        ReDim Preserve myDocumentsGivenLanguages(myIndex)
                        myDocumentsGivenLanguages(myIndex) = CType(myDataRow("LanguageID"), Integer)
                        myIndex = myIndex + 1
                    Next
                    ReDim result(myLanguages.Rows.Count - 1)

                    Dim index As Integer = 0
                    For Each myDataRow As DataRow In myLanguages.Rows
                        'Detect availability of activated market in current editor data
                        Dim myIsAvailable As Boolean = False
                        If Not myDocumentsGivenLanguages Is Nothing Then
                            myIsAvailable = Array.IndexOf(myDocumentsGivenLanguages, myDataRow("ID")) > -1
                        End If

                        result(index).id = CType(myDataRow("ID"), Integer)
                        result(index).languageDescriptionEnglish = CType(myDataRow("Description_English"), String)
                        result(index).available = myIsAvailable
                        index += 1
                    Next

                End If
                Return result
            End Function


            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Remove all elements which are not in the allow-list and return all remaining items
            ''' </summary>
            ''' <param name="list">An array containing some elements</param>
            ''' <param name="allowedItems">Allowed values</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	18.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub RemoveLanguageItemsNotInAllowedList(ByVal list As DataTable, ByVal allowedItems As Integer())
                Dim allowedLanguages As New ArrayList(allowedItems)
                For MyCounter As Integer = list.Rows.Count - 1 To 0 Step -1
                    If Not allowedLanguages.Contains(list.Rows(MyCounter)("ID")) Then
                        list.Rows.RemoveAt(MyCounter)
                    End If
                Next
            End Sub


            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Set the editor by a given id
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	31.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub FillEditorContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer)
                Try
                    Dim myContent As String = Database.ReadContent(serverID, url, editorID, marketID, version)
                    editorMain.Html = myContent
                Catch ex As Exception
                    Me.cammWebManager.Log.Write("SmartWcms:<br> - WebEditor<br>" & ex.ToString)
                End Try
            End Sub 'SetEditorContent(byval ID as integer)

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Gets information to decide which request mode shall be shown
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	21.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub CheckForRequestMode()

                'Lookup requested action
                If txtRequestedAction.Value = Nothing AndAlso Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID).Rows.Count = 0 Then
                    RequestMode = RequestModes.CreateFirstVersion
                ElseIf txtRequestedAction.Value.ToLower = "newversion" AndAlso Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) = 0 AndAlso Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) > 0 Then
                    'Special case when new version requested and editversion = 1 (=not yet released)
                    RequestMode = Nothing
                ElseIf txtRequestedAction.Value.ToLower = "update" Then
                    RequestMode = RequestModes.Update
                ElseIf txtRequestedAction.Value.ToLower = "dropcurrentmarket" Then
                    RequestMode = RequestModes.DropCurrentMarketData
                ElseIf txtRequestedAction.Value.ToLower = "newversion" Then
                    RequestMode = RequestModes.NewVersion
                ElseIf txtActivate.Value.ToLower = "activate" Then
                    RequestMode = RequestModes.Activation
                ElseIf txtBrowseToMarketVersion.Value <> "" Then 'AndAlso Page.IsPostBack Then
                    Throw New Exception("Unexpected workflow")
                End If

                'Reset form action fields
                txtRequestedAction.Value = Nothing
                txtActivate.Value = Nothing
                txtBrowseToMarketVersion.Value = Nothing

            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The standard loading functionallity for this document
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[Swiercz]	31.10.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub StandardLoading()

#If DebugMode Then
                CreateDebugOutput("StandardLoading Start")
#End If
                'Contains every data available for this document
                'Dim myDataTable As DataTable = myDataTable = GetAllDataForAGivenEditorControl(DocumentID, False)

                'Access Allowed by security 
                Try
                    If Me.ShowWithEditRights Then
                        'Load edit mode
                        'Ensure that LanguageToShow contains a valid value also when this page runs with edit rights
                        'Define some needed vars
                        Dim myCurrentLanguage As Integer = 0
                        Dim myCurrentVersion As Integer = 0
                        Dim MyActiveVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                        'Check for parameters passed by url
                        If CurrentVersion <> 0 Then
                            myCurrentVersion = CurrentVersion
                        ElseIf editorMain.Visible Then
                            myCurrentVersion = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                            CurrentVersion = myCurrentVersion
                        End If
                        If LanguageToShow >= 0 Then
                            myCurrentLanguage = LanguageToShow
                        End If
                        'Check if formular was redirected to itself
                        If myCurrentVersion = 0 AndAlso Me.IsEmptyInnerHtml = False Then
                            'No active/released version available, but inner html present
                            Me.editorMain.Html = Me.InnerHtml
                        ElseIf myCurrentLanguage = 0 AndAlso myCurrentVersion = 0 AndAlso MyActiveVersion <> 0 Then
                            'Formular wasn't redirected and shall be opened in it's currently released version in UIMarket Language
                            If _CurrentVersionHasChanged Then
                                FillEditorContent(Me.ContentOfServerID, DocumentID, Me.EditorID, cammWebManager.UI.MarketID, MyActiveVersion)
                            End If
                        Else
                            'Formular was redirected and shall be opened by the given parameters
                            'Use the informations passed by querystring to open the correct version
                            If _CurrentVersionHasChanged Then
                                FillEditorContent(Me.ContentOfServerID, DocumentID, Me.EditorID, myCurrentLanguage, myCurrentVersion)
                            End If
                        End If
                    Else 'Without EditRights
                        'Load view only mode
                        lblViewOnlyContent.Visible = True
                        editorMain.Visible = False
                        ibtnSwitchToEditMode.Visible = False
                        If Me.EnableCache AndAlso Not CachedItemContent Is Nothing Then
                            lblViewOnlyContent.InnerHtml = CachedItemContent
#If DebugMode Then
                            WriteDebugOutputDirectly("cached ")
#End If
                        Else
                            'ToDo 4 JW: optimize code to just read released content and no version number, innerhtml only when db-content has no released data
                            Try
                                Dim HighestAvailableVersion As Integer = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                                If HighestAvailableVersion = 0 OrElse (HighestAvailableVersion = 1 AndAlso Not Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) = 1) Then
                                    lblViewOnlyContent.InnerHtml = Me.InnerHtml
                                Else
                                    If Me.MarketLookupMode = MarketLookupModes.SingleMarket Then
                                        lblViewOnlyContent.InnerHtml = Database.ReadReleasedContent(Me.ContentOfServerID, DocumentID, Me.EditorID, 0)
                                    Else
                                        Try
                                            LookupMatchingMarketDataForReleasedContent(True)
                                        Catch ex As UseInnerHtmlException
                                            'Use inner html 
                                            lblViewOnlyContent.InnerHtml = Me.InnerHtml
                                        Catch
                                            Throw
                                        End Try
                                        lblViewOnlyContent.InnerHtml = Database.ReadReleasedContent(Me.ContentOfServerID, DocumentID, Me.EditorID, Me.LanguageToShow)
                                    End If
                                End If
                            Catch ex As UseInnerHtmlException
#If DebugMode Then
                                WriteDebugOutputDirectly("with inner html ")
#End If
                                lblViewOnlyContent.InnerHtml = Me.InnerHtml
                            End Try
                            CachedItemContent = lblViewOnlyContent.InnerHtml
#If DebugMode Then
                            WriteDebugOutputDirectly("queried ")
#End If
                        End If
                    End If
#If DebugMode Then
                    WriteDebugOutputDirectly("innherhtml=" & Me.InnerHtml & "<br>")
                    WriteDebugOutputDirectly("currentversion=" & Me.CurrentVersion & "<br>")
#End If
                Catch ex As Exception
                    Me.cammWebManager.Log.Write("SmartWcms:<br> - WebEditor<br>" & ex.ToString)
                End Try

#If DebugMode Then
                CreateDebugOutput("StandardLoading End")
#End If

            End Sub
#End Region

#Region "UseInnerHtmlException"
            Private Class UseInnerHtmlException
                Inherits Exception

            End Class
#End Region

#Region " Caching of released data"
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     This is the name of the key for the content for this editor in the HttpCache
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private ReadOnly Property CachedItemKey() As String
                Get
                    Return Me.GetType.ToString & "&" & HttpContext.Current.Server.UrlEncode(Me.DocumentID) & "&" & HttpContext.Current.Server.UrlEncode(Me.EditorID) & "&" & LanguageToShow.ToString
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Clear all keys related to this document (all languages/markets)
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub ClearCache()
                Dim SearchCacheKeys As String = Me.GetType.ToString & "&" & HttpContext.Current.Server.UrlEncode(Me.DocumentID) & "&" & HttpContext.Current.Server.UrlEncode(Me.EditorID) & "&"
                For Each item As Collections.DictionaryEntry In HttpContext.Current.Cache
                    Dim Key As String = CType(item.Key, String)
                    If Key.GetType Is GetType(String) AndAlso CType(Key, String).StartsWith(SearchCacheKeys) Then
                        HttpContext.Current.Cache.Remove(Key)
                    End If
                Next
            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The value of the cached, released content or null (Nothing in VisualBasic) when it's not cached yet
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Property CachedItemContent() As String
                Get
                    Return CType(HttpContext.Current.Cache(CachedItemKey), String)
                End Get
                Set(ByVal Value As String)
                    Utils.SetHttpCacheValue(CachedItemKey, Value, Me.CachedItemLivetime, Me.CachedItemPriority)
                End Set
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The default cache duration takes 15 minutes at maximum
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Overridable ReadOnly Property CachedItemLivetime() As TimeSpan
                Get
                    Return New TimeSpan(0, 15, 0) '15 minutes
                End Get
            End Property

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     The default cache priority is set to low to allow remval of this cache item at first
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Protected Overridable ReadOnly Property CachedItemPriority() As Caching.CacheItemPriority
                Get
                    Return Caching.CacheItemPriority.Low
                End Get
            End Property

#End Region

        End Class

    End Namespace

End Namespace