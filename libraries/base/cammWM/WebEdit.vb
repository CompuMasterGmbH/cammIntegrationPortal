'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Explicit On
Option Strict On

Imports CompuMaster.camm.WebManager.Tools
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.ComponentModel

Namespace CompuMaster.camm.WebManager.Modules.WebEdit

    Namespace Controls

        ''' <summary>
        '''     Controls for online editing of web pages (and web controls)
        ''' </summary>
        Friend Class NamespaceDoc
        End Class

        Public Class Configuration

            Friend Sub New()
                'Creatable only assembly-internally
            End Sub

            Public ReadOnly Property ContentOfServerID() As Integer
                Get
                    Return ConfigurationWebManager.WebEditorContentOfServerID
                End Get
            End Property

            Public Shared Function WebManagerSettings(settingName As String) As String
                If settingName.StartsWith("WebManager.Wcms.") Then
                    Return ConfigurationWebManager.WebManagerSettings(settingName)
                Else
                    Throw New ArgumentException("Not a SmartEditor setting")
                End If
            End Function

        End Class

        Friend Class ConfigurationWebManager

            Public Class CwmConfigAccessor
                Inherits CompuMaster.camm.WebManager.Modules.WebEdit.Controls.SmartWcmsEditorBase

                Friend Sub New()
                End Sub

            End Class

            ''' <summary>
            ''' Every WebEditor content is related to a server; this property overrides the server ID value where to read from/save to
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' 0 = currently used server; other values = forced server ID
            ''' </remarks>
            Public Shared ReadOnly Property WebEditorContentOfServerID() As Integer
                Get
                    Dim WMConfigAccessor As New CwmConfigAccessor
                    Return WMConfigAccessor.Configuration.ContentOfServerID

                    'Maybe a better way for future - but still inactive due to backwards-compatibility with CWM v4.10.192
                    'Return LoadIntegerSetting("WebManager.WebEditor.ContentOfServerID", 0, False)
                End Get
            End Property

            '#Region "Load configuration setting helper methods"

            Friend Shared ReadOnly Property WebManagerSettings() As System.Collections.Specialized.NameValueCollection
                Get
                    Return System.Configuration.ConfigurationSettings.AppSettings
                End Get
            End Property

            '        ''' <summary>
            '        '''     Load an integer value from the configuration file
            '        ''' </summary>
            '        ''' <param name="appSettingName">The name of the appSetting item</param>
            '        ''' <param name="defaultValue">A default value if not configured of configured invalid</param>
            '        ''' <param name="suppressExceptions">True if exceptions shall be suppressed and default value returned or False if exception shall be thrown if there is an error</param>
            '        ''' <returns></returns>
            '        Private Shared Function LoadIntegerSetting(ByVal appSettingName As String, ByVal defaultValue As Integer, ByVal suppressExceptions As Boolean) As Integer
            '            Dim Result As Integer = defaultValue
            '            Try
            '                Dim value As String = CType(WebManagerSettings.Item(appSettingName), String)
            '                If value = Nothing Then value = AdditionalConfiguration(appSettingName)
            '                If value = Nothing Then
            '                    Result = defaultValue
            '                Else
            '                    Result = Integer.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
            '                End If
            '            Catch ex As Exception
            '                If suppressExceptions = False Then
            '                    Throw New ArgumentException("Configuration setting for """ & appSettingName & """ is invalid. Choose a valid integer number, please.", ex)
            '                End If
            '            End Try
            '            Return Result
            '        End Function
            '#End Region

        End Class

        ''' <summary>
        '''     Provider for loading and saving of data from and to the database
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Class SmartWcmsDatabaseAccessLayer

            Private _swcms As ISmartWcmsEditor
            Friend Sub New(ByVal swcms As ISmartWcmsEditor)
                _swcms = swcms
            End Sub

            ''' <summary>
            '''     The connection string
            ''' </summary>
            Private ReadOnly Property ConnectionString() As String
                Get
                    Return Me._swcms.cammWebManager.ConnectionString
                End Get
            End Property

            ''' <summary>
            '''     Clear all cached variables which contain database values
            ''' </summary>
            Friend Sub ClearCachedDbValues()
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

            ''' <summary>
            '''     An array of available languages within an editor controls version
            ''' </summary>
            ''' <returns></returns>
            Public Function AvailableMarketsInData(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal version As Integer) As Integer()
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
                myCommand.Parameters.Add("@Version", SqlDbType.Int).Value = version
                Return CType(Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection).ToArray(GetType(Integer)), Integer())
            End Function

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
                Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Sub

            ''' <summary>
            '''     Identify if a given market exists in the defined, versioned document
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <param name="version"></param>
            ''' <returns></returns>
            Public Function IsMarketAvailable(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer) As Boolean
                Dim Data As DataRow() = Me.ReadAllData(serverID, url, editorID).Select("LanguageID = " & marketID & " AND Version = " & version)
                If Data.Length = 0 Then
                    Return False
                Else
                    Return True
                End If
            End Function

            ''' <summary>
            '''     Remove a given market from the editable version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
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
                Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Sub

            ''' <summary>
            '''     Release the latest version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="releasedByUser"></param>
            ''' <returns></returns>
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
                Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Function

            Private _CachedMaxVersion_ServerID As Integer = Integer.MinValue
            Private _CachedMaxVersion_Url As String = Nothing
            Private _CachedMaxVersion_EditorID As String = Nothing
            Private _CachedMaxVersion_Result As Integer = Integer.MinValue

            ''' <summary>
            '''     The highest version available, this is either the released version number or the version number of the current edit version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <value></value>
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

            ''' <summary>
            '''     The number of the released version
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <value></value>
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

            ''' <summary>
            '''     Read max. version and released version number from database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <remarks>
            '''     If a version number can't be looked up because there is no data available, the read version number will be 0 (zero).
            ''' </remarks>
            Private Sub ReadRelatedVersionInfo(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String)

                'Try to lookup the important version numbers from already queried data when it is there
                If serverID <> Me._CachedCompleteData_ServerID OrElse url <> Me._CachedCompleteData_Url OrElse editorID = Me._CachedCompleteData_EditorID OrElse Me._CachedCompleteData_Result Is Nothing Then
                    'Yes! Complete data has already been queried
                    Dim myDataTable As DataTable = ReadAllData(serverID, url, editorID)
                    Dim _MaxVersion As Integer = 0
                    Dim _ReleasedVersion As Integer = 0
                    For Each myDataRow As DataRow In myDataTable.Rows
                        If CType(Utils.Nz(myDataRow("IsActive")), Boolean) = True Then
                            _ReleasedVersion = Utils.Nz(myDataRow("Version"), 0)
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
                    results = Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)(0)
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

            ''' <summary>
            '''     Read all data from database regarding a defined document
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
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
                    _CachedCompleteData_Result = Data.DataQuery.AnyIDataProvider.FillDataTable(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "result")
                    'Update caching environment for cached values
                    _CachedCompleteData_ServerID = serverID
                    _CachedCompleteData_Url = url
                    _CachedCompleteData_EditorID = editorID
                End If
                Return _CachedCompleteData_Result
            End Function

            ''' <summary>
            '''     Copys a editor controls version to a new higher one.
            ''' </summary>
            ''' <param name="sourceVersion">The version you want to copy</param>
            ''' <param name="serverID">The WebManagers serverID where the new version shall appear (might be usefull for later feature requests)</param>
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
                Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                'Reset cached values
                Me.ClearCachedDbValues()
            End Sub

            ''' <summary>
            '''     List the activated markets as defined in camm Web-Manager
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            Public Function ActiveMarketsInWebManager() As DataTable
                If HttpContext.Current.Application("WebManager.sWcms.ActiveLanguages") Is Nothing Then
                    Dim myConnection As SqlClient.SqlConnection
                    Dim myCommand As SqlClient.SqlCommand
                    Dim myQuery As String = _
                        "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                        "select ID, Description_English, AlternativeLanguage from system_languages where [IsActive] = 1"
                    myConnection = New SqlClient.SqlConnection(ConnectionString)
                    myCommand = New SqlClient.SqlCommand(myQuery, myConnection)
                    Dim Result As DataTable = Data.DataQuery.AnyIDataProvider.FillDataTable(myCommand, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "result")

                    'Also store the active language informations into the application
                    'object to avoid roundtrips to the database
                    HttpContext.Current.Application("WebManager.sWcms.ActiveLanguages") = Result

                    Return Result
                Else
                    Return CType(HttpContext.Current.Application("WebManager.sWcms.ActiveLanguages"), DataTable)
                End If
            End Function

            ''' <summary>
            '''     List the activated markets as defined in camm Web-Manager
            ''' </summary>
            ''' <returns></returns>
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

            ''' <summary>
            '''     Read the released HTML for the requested document from the database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="marketID"></param>
            ''' <returns></returns>
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
                Return Utils.Nz(Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
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


                Return Utils.Nz(Data.DataQuery.AnyIDataProvider.ExecuteScalar(command, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, Integer))

            End Function

            ''' <summary>
            '''     Read the HTML in a requested version for the requested document from the database
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <param name="marketID"></param>
            ''' <param name="url"></param>
            ''' <param name="editorID"></param>
            ''' <param name="version"></param>
            ''' <returns></returns>
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
                Return Utils.Nz(Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
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
            '''     The interface implementation required for the database access layer
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Private ReadOnly Property _cammWebManager() As CompuMaster.camm.WebManager.IWebManager Implements ISmartWcmsEditor.cammWebManager
                Get
                    Return Me.cammWebManager
                End Get
            End Property

#Region " Database methods "

            ''' <summary>
            '''     Database access layer
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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

#Region "Properties"

            Private _DocumentID As String
            ''' <summary>
            '''     An identifier of the current document, by default its URL
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     Regulary, content is always related to the current server, only. In some special cases, you might want to override this to show content from another server.
            ''' </summary>
            ''' <value>The ID value of the server to whome the content is related</value>
            ''' <remarks>
            '''     By default, the address (e. g.) "/content.aspx" provides different content on different servers. So, the intranet and the extranet are able to show independent content.
            '''     In some cases, you might want to override this behaviour and you want to show on the same URL the same content in the extranet as well as in the intranet. In this case, you would setup this property on the extranet server's scripts to show the content of the intranet server.
            ''' </remarks>
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

            ''' <summary>
            '''     Contains informations about how to handle the viewonly mode in different market, langs
            ''' </summary>
            Public Enum MarketLookupModes As Integer
                ''' <summary>
                '''     Data is only available in an international version and this is valid for all languages/markets
                ''' </summary>
                ''' <remarks>
                '''     This value is the same as None, just the name is more explainable
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                SingleMarket = 0
                ''' <summary>
                '''     Data is only available in an international version and this is valid for all languages/markets
                ''' </summary>
                ''' <remarks>
                '''     This value is the same as SingleMarket, just the name is more simplified
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                None = 0
                ''' <summary>
                '''     Data is maintained for every market separately, the language markets (e. g. "English", "French", etc. are handled as a separate market)
                ''' </summary>
                ''' <remarks>
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                Market = 1
                ''' <summary>
                '''     Data is maintained for every language/market separately; when there is no value for a market it will be searched for some compatible language data
                ''' </summary>
                ''' <remarks>
                '''     Example: When the visitor is in market "German/Austria" but there is only some content available for market "German", the German data will be used.
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                Language = 2
                ''' <summary>
                '''     Data is maintained for every language/market separately; when there is no value for the current market, the sWCMS control tries to lookup a best matching content
                ''' </summary>
                ''' <remarks>
                '''     When the user requests a page in e. g. market 559 ("French/France"), there will be the following order for the lookup process:
                '''     <list>
                '''         <item>Current market, in ex. ID 559 / French/France</item>
                '''         <item>Current language of market, in ex. ID 3 / French</item>
                '''         <item>Until customized by propert AlternativeDataMarkets: English universal, ID 1</item>
                '''         <item>Until customized by propert AlternativeDataMarkets: Worldwide market, ID 10000</item>
                '''         <item>International, ID 0</item>
                '''     </list>
                '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
                ''' </remarks>
                BestMatchingLanguage = 3
            End Enum

            Private _MarketLookupMode As MarketLookupModes
            ''' <summary>
            '''     Represents the current MarketLookupMode, passed as parameter by the ctrl
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Public Property MarketLookupMode() As MarketLookupModes
                Get
                    Return _MarketLookupMode
                End Get
                Set(ByVal Value As MarketLookupModes)
                    _MarketLookupMode = Value
                End Set
            End Property 'MarketLookupMode()

            Private _SecurityObjectEditMode As String
            ''' <summary>
            '''     Indicates which application is needed to edit the formular
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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

        Public MustInherit Class SmartWcmsEditorBaseLevel2
            Inherits SmartWcmsEditorBase

            ''' <summary>
            ''' The editor control to display or edit the content
            ''' </summary>
            ''' <returns></returns>
            Protected MustOverride ReadOnly Property MainEditor As IEditor

            ''' <summary>
            '''     Is this editor in edit mode?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Public MustOverride ReadOnly Property EditModeActive() As Boolean

        End Class

        Public MustInherit Class SmartWcmsEditorCommonBase
            Inherits SmartWcmsEditorBaseLevel2

#Region "Version tracking"
            Dim _LatestData As Integer
            ''' <summary>
            '''     The pages latest data (RowID)
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Public Property LatestData() As Integer
                Get
                    Return _LatestData
                End Get
                Set(ByVal Value As Integer)
                    _LatestData = Value
                End Set
            End Property

            Dim _LatestVersion As Integer
            ''' <summary>
            '''     The pages latest version
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     The pages current version
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     Only for saving the new value without a "_CurrentVersionHasChanged = True"
            ''' </summary>
            ''' <param name="value"></param>
            ''' <remarks>
            ''' </remarks>
            Private Sub CurrentVersionSetWithoutInternalChangeFlag(ByVal value As Integer)
                ViewState("WebEditorXXL.showversion") = value
            End Sub
            ''' <summary>
            '''     Indicate if the viewstate already contains an information about the version which shall be shown
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Indicates the visibility of the switch to edit mode image button
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     Is needed to make this button only visible if it shall appear
            ''' </remarks>
            Public Property EditModeSwitchAvailable() As Boolean
                Get
                    Return Me.ibtnSwitchToEditMode.Visible
                End Get
                Set(ByVal Value As Boolean)
                    Me.ibtnSwitchToEditMode.Visible = Value
                End Set
            End Property

            Public ReadOnly Property EditorClientID As String
                Get
                    Return Me.editorMain.ClientID
                End Get
            End Property

            Private _EditorID As String
            ''' <summary>
            '''     An identifier of the editor instance in the current document, by default the ClientID property value (but don't use this as ClientID for access in client scripting!)
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     This ID is required for support of multiple editor instances on the same page.
            ''' </remarks>
            Public Property EditorID() As String
                Get
                    Return _EditorID
                End Get
                Set(ByVal Value As String)
                    _EditorID = Value
                End Set
            End Property

            ''' <summary>
            '''     The edit mode as it is defined in the viewstate
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Is the control editable in general or not?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Possible actions requested by the form front-end
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            Public Enum RequestModes As Integer
                ''' <summary>
                '''     Nothing to do
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                NoActionRequested = 0
                ''' <summary>
                '''     Save changes
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                Update = 1
                ''' <summary>
                '''     Create a new version for modification
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                NewVersion = 2
                ''' <summary>
                '''     Save changes and release the whole version
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                Activation = 3
                ''' <summary>
                '''     Drop the content of the current market
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                DropCurrentMarketData = 6
                ''' <summary>
                '''     There is no version data available, but the first version shall be created now
                ''' </summary>
                ''' <remarks>
                ''' </remarks>
                CreateFirstVersion = 7
            End Enum

            Private _RequestMode As RequestModes
            ''' <summary>
            '''     Contains the requested controls mode
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     Has the user authorization to change the content and does he see at least the pencil to start editing?
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     Indicates which toolbar shall be used
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Protected Property ToolbarSetting() As ToolbarSettings
                Get
                    Return _ToolbarSetting
                End Get
                Set(ByVal Value As ToolbarSettings)
                    _ToolbarSetting = Value
                End Set
            End Property 'ToolbarSetting()

            ''' <summary>
            '''     Creates the text value for the edit information label which gives you information
            '''     about the currently opened version and it's status
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Image button SwitchToEditMode click events
            ''' </summary>
            ''' <param name="sender"></param>
            ''' <param name="e"></param>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Adds the controls components to it's form
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Remove any duplicate paths and return the new array with path values with all different meaning
            ''' </summary>
            ''' <param name="paths">Array of several virtual paths</param>
            ''' <param name="singlePath">One virtual path</param>
            ''' <returns>Array without any duplicate, virtual path meanings</returns>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Detects which toolbar shall appear
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Raises the standard 404 error message as defined right in the IIS
            ''' </summary>
            ''' <param name="message">An error message with some details on the missing item</param>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     If the inner html property contains spaces, tabs, CR or LF chars only, then this will be indicated as True, otherwise there is some real text/content and this method will return False
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Detect the language/market for first-time-data-creation in edit-requests
            ''' </summary>
            ''' <returns>An integer ID of the market/language</returns>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Change the LanguageToShow variable to an existing value when the current one is not valid
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
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


            ''' <summary>
            '''     Detect the LanguageToShow for view-only requests
            ''' </summary>
            ''' <param name="returnCode404InCaseOfMissingData">True raises a 404 error when it can't be looked up</param>
            ''' <remarks>
            ''' </remarks>
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

            Protected Function LookupParentServerFormName() As String
                Dim FormID As String = Me.LookupParentServerForm.Name
                If FormID = Nothing Then
                    FormID = Me.LookupParentServerForm.ClientID
                End If
                If FormID = Nothing Then
                    FormID = Replace(Me.LookupParentServerForm.UniqueID, ":", "_")
                End If
                If FormID = Nothing Then
                    Throw New Exception("Empty Form-Name" & vbNewLine & "UniqueID=" & Me.LookupParentServerForm.UniqueID & vbNewLine & "ID=" & Me.LookupParentServerForm.ID & vbNewLine & "FormName=" & Me.LookupParentServerForm.Name & vbNewLine & "FormNewClientID=" & Me.LookupParentServerForm.NamingContainer.ClientID)
                End If
                Return FormID
            End Function

            ''' <summary>
            '''     Generates a url conform string to pass trough url as parameter for later usement
            ''' </summary>
            ''' <param name="sourceString"></param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Update an already existing document in given language or create a new language for the document, depends on several parameters
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     Creates a new pages version
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            Private Sub CreateANewPageVersion()
                'Check for clicked savebutton with update instructions without an already existing version in the given language
                'Case new version
                Try
                    Me.Page.Application.Lock()
                    Database.ClearCachedDbValues()
                    Dim ReleasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    Dim MaxVersion As Integer = Me.Database.MaxVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                    If ReleasedVersion <> 0 AndAlso MaxVersion <> 0 Then
                        If MaxVersion - ReleasedVersion > 0 Then
                            'Invalid request - but sometimes, IE <= 11 do submit the form 2 times for whatever reason
                            'Throw New Exception("Invalid request (HighestVersion = " & MaxVersion & "; ActiveVersion=" & ReleasedVersion & ")")
                            'HACK: just ignore and continue loading the page as usual in edit mode --> do nothing here 
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
                    Database.ClearCachedDbValues()
                Finally
                    Me.Page.Application.UnLock()
                End Try

            End Sub

            ''' <summary>
            '''     Returns the languages description in english as string
            ''' </summary>
            ''' <param name="ID">The language id whose description you want to get</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     Is there at least one SmartWcms control in edit mode?
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            Private Function FindSWcmsControlsInEditModeOnThisPage() As Boolean
                Dim Result As Boolean
#If NetFrameWork = "1_1" Then
            Dim EditorControls As New ArrayList
#Else
                Dim EditorControls As New List(Of System.Web.UI.Control)
#End If
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

            ''' <summary>
            '''     Walk recursive through all children controls and collect all SmartWcms items
            ''' </summary>
            ''' <param name="results">An arraylist which will contain all positive matches</param>
            ''' <param name="controlsCollection">The control collection which shall be browsed</param>
            ''' <remarks>
            ''' </remarks>
#If NetFrameWork = "1_1" Then
            Protected Sub FindSWcmsControlsOnThisPage(ByVal results As ArrayList, ByVal controlsCollection As System.Web.UI.ControlCollection)
#Else
            Protected Sub FindSWcmsControlsOnThisPage(ByVal results As List(Of System.Web.UI.Control), ByVal controlsCollection As System.Web.UI.ControlCollection)
#End If
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
            ''' <summary>
            ''' Remove all occurances in links of the current server name, e. g. http://www.yourcompany.com
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     Use HttpCache to boost the performance by decreasing the number of required database queries
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Public Property EnableCache() As Boolean
                Get
                    Return _EnableCache
                End Get
                Set(ByVal Value As Boolean)
                    _EnableCache = Value
                End Set
            End Property

            Private _InnerHtml As String
            ''' <summary>
            '''     The static inner HTML code is the default HTML when the database doesn't contain any released content
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     If there is no released content from database and this inner HTML contains whitespaces only, you'll run into a 404 HTTP error page (file not found).
            ''' </remarks>
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

            ''' <summary>
            '''     The requested edit mode for the processing of this page request
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     Typically, this value is predefined to a default or will be changed by user via form
            ''' </remarks>
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

            ''' <summary>
            '''     This event will happen while Page_Init and allows application developers to define the security object name just-in-time
            ''' </summary>
            ''' <remarks>
            '''     This early execution of the security check is required to decide about required viewstate of this control's data controls. (After Page_Init, the viewstate wouldn't be loaded any more.)
            ''' </remarks>
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
                            ClearCache() 'The cache data has to be refreshed
                            CreateANewPageVersion()
                            ClearCache() 'The cache data has to be refreshed
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
                        myLanguages = Data.DataTables.GetDataTableClone(Database.ActiveMarketsInWebManager())
                        'Remove languages not available when in old or released version
                        If CurrentVersion <= Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID) Then
                            RemoveLanguageItemsNotInAllowedList(myLanguages, Me.Database.AvailableMarketsInData(Me.ContentOfServerID, Me.DocumentID, Me.EditorID, Me.CurrentVersion))
                        End If
                    Case MarketLookupModes.Language
                        'Only show languages, no markets
                        myLanguages = Data.DataTables.GetDataTableClone(Database.ActiveMarketsInWebManager(), "AlternativeLanguage IS NULL")
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


            ''' <summary>
            '''     Remove all elements which are not in the allow-list and return all remaining items
            ''' </summary>
            ''' <param name="list">An array containing some elements</param>
            ''' <param name="allowedItems">Allowed values</param>
            ''' <remarks>
            ''' </remarks>
            Private Sub RemoveLanguageItemsNotInAllowedList(ByVal list As DataTable, ByVal allowedItems As Integer())
                Dim allowedLanguages As New ArrayList(allowedItems)
                For MyCounter As Integer = list.Rows.Count - 1 To 0 Step -1
                    If Not allowedLanguages.Contains(list.Rows(MyCounter)("ID")) Then
                        list.Rows.RemoveAt(MyCounter)
                    End If
                Next
            End Sub


            ''' <summary>
            '''     Set the editor by a given id
            ''' </summary>
            ''' <param name="serverID"></param>
            ''' <remarks>
            ''' </remarks>
            Private Sub FillEditorContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer)
                Try
                    Dim myContent As String = Database.ReadContent(serverID, url, editorID, marketID, version)
                    editorMain.Html = myContent
                Catch ex As Exception
                    Me.cammWebManager.Log.Write("SmartWcms:<br> - WebEditor<br>" & ex.ToString)
                End Try
            End Sub 'SetEditorContent(byval ID as integer)

            ''' <summary>
            '''     Gets information to decide which request mode shall be shown
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
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

            ''' <summary>
            '''     The standard loading functionallity for this document
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
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
            ''' <summary>
            '''     This is the name of the key for the content for this editor in the HttpCache
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Private ReadOnly Property CachedItemKey() As String
                Get
                    Return Me.GetType.ToString & "&" & HttpContext.Current.Server.UrlEncode(Me.DocumentID) & "&" & HttpContext.Current.Server.UrlEncode(Me.EditorID) & "&" & LanguageToShow.ToString
                End Get
            End Property

            ''' <summary>
            '''     Clear all keys related to this document (all languages/markets)
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            Private Sub ClearCache()
                Dim SearchCacheKeys As String = Me.GetType.ToString & "&" & HttpContext.Current.Server.UrlEncode(Me.DocumentID) & "&" & HttpContext.Current.Server.UrlEncode(Me.EditorID) & "&"
                For Each item As Collections.DictionaryEntry In HttpContext.Current.Cache
                    Dim Key As String = CType(item.Key, String)
                    If Key.GetType Is GetType(String) AndAlso CType(Key, String).StartsWith(SearchCacheKeys) Then
                        HttpContext.Current.Cache.Remove(Key)
                    End If
                Next
            End Sub

            ''' <summary>
            '''     The value of the cached, released content or null (Nothing in VisualBasic) when it's not cached yet
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Private Property CachedItemContent() As String
                Get
                    Return CType(HttpContext.Current.Cache(CachedItemKey), String)
                End Get
                Set(ByVal Value As String)
                    Utils.SetHttpCacheValue(CachedItemKey, Value, Me.CachedItemLivetime, Me.CachedItemPriority)
                End Set
            End Property

            ''' <summary>
            '''     The default cache duration takes 15 minutes at maximum
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Protected Overridable ReadOnly Property CachedItemLivetime() As TimeSpan
                Get
                    Return New TimeSpan(0, 15, 0) '15 minutes
                End Get
            End Property

            ''' <summary>
            '''     The default cache priority is set to low to allow remval of this cache item at first
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            Protected Overridable ReadOnly Property CachedItemPriority() As Caching.CacheItemPriority
                Get
                    Return Caching.CacheItemPriority.Low
                End Get
            End Property

#End Region

        End Class

        ''' <summary>
        '''     WYSIWIG online editor
        ''' </summary>
        ''' <remarks>
        '''     Requires write permission for the AspNet worker process (regulary the ASPNET account of the webserver machine) to the file which contains this control.
        ''' </remarks>
        <Obsolete("Better use one of the cammWM.SmartEditor controls instead", True), System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> Public Class RadEditor
            Inherits System.Web.UI.Control

        End Class

        <DefaultProperty("Html"), ToolboxData("<{0}:PlainTextEditor1 runat=server></{0}:PlainTextEditor1>")> _
        Friend Class PlainTextEditor
            Inherits System.Web.UI.WebControls.TextBox
            Implements IEditor

            '<Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property Text() As String
            '    Get
            '        Dim s As String = CStr(ViewState("Text"))
            '        If s Is Nothing Then
            '            Return String.Empty
            '        Else
            '            Return s
            '        End If
            '    End Get

            '    Set(ByVal Value As String)
            '        ViewState("Text") = Value
            '    End Set
            'End Property

            'Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
            '    writer.Write(Text)
            'End Sub

            Public Sub New()
                MyBase.New()
                Me.TextMode = TextBoxMode.MultiLine
                Me.CssWidth = "100%"
                Me.CssHeight = "280px"
            End Sub

            ''' <summary>
            ''' Always provide client scripts (only in Edit mode) to encode raw text editor data (e.g. HTML code) so that RequestValidation doesn't throw an exception because of potentially dangerous data
            ''' </summary>
            ''' <seealso cref="EscapeRecognitionSequence"/>
            Private Sub PlainTextEditor_PreRender_EncodeRawData(sender As Object, e As EventArgs) Handles MyBase.PreRender
                If Me.Editable Then
#If NetFrameWork = "1_1" Then
                    Me.Page.RegisterClientScriptBlock("EncodeRawData|Base", "function EncodeRawDataIfNotEncoded (item) " & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "if ((item.value != null) && (item.value.length >= 5) && (item.value.substring(0, 5) != String.fromCharCode(27) + 'ESC' + String.fromCharCode(27))) " & vbNewLine & _
                                                                   "    { " & vbNewLine & _
                                                                   "    item.value = EncodeRawData(item); " & vbNewLine & _
                                                                   "    } " & vbNewLine & _
                                                                   "}" & vbNewLine & _
                                                                   "function EncodeRawData (item) " & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "    return String.fromCharCode(27) + 'ESC' + String.fromCharCode(27) + item.value.replace(/%/g,escape('%')).replace(/</g,escape('<')).replace(/>/g,escape('>')); " & vbNewLine & _
                                                                   "}" & vbNewLine & _
                                                                   "")
                    Me.Page.RegisterOnSubmitStatement( "EncodeRawData_" & Me.ClientID, String.Format("EncodeRawDataIfNotEncoded (document.getElementById('{0}'));", Me.ClientID))
#Else
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "EncodeRawData|Base", "function EncodeRawDataIfNotEncoded (item) " & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "if ((item.value != null) && (item.value.length >= 5) && (item.value.substring(0, 5) != String.fromCharCode(27) + 'ESC' + String.fromCharCode(27))) " & vbNewLine & _
                                                                   "    { " & vbNewLine & _
                                                                   "    item.value = EncodeRawData(item); " & vbNewLine & _
                                                                   "    } " & vbNewLine & _
                                                                   "}" & vbNewLine & _
                                                                   "function EncodeRawData (item) " & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "    return String.fromCharCode(27) + 'ESC' + String.fromCharCode(27) + item.value.replace(/%/g,escape('%')).replace(/</g,escape('<')).replace(/>/g,escape('>')); " & vbNewLine & _
                                                                   "}" & vbNewLine & _
                                                                   "", True)
                    Me.Page.ClientScript.RegisterOnSubmitStatement(Me.GetType, "EncodeRawData_" & Me.ClientID, String.Format("EncodeRawDataIfNotEncoded (document.getElementById('{0}'));", Me.ClientID))
#End If
                End If
            End Sub

            Public Property CssWidth As String Implements IEditor.CssWidth
                Get
                    Return Me.Style.Item("width")
                End Get
                Set(value As String)
                    Me.Style.Add("width", value)
                End Set
            End Property

            Public Property CssHeight As String Implements IEditor.CssHeight
                Get
                    Return Me.Style.Item("height")
                End Get
                Set(value As String)
                    Me.Style.Add("height", value)
                End Set
            End Property

            Public Property TextareaColumns As Integer Implements IEditor.TextareaColumns
                Get
                    Return Me.Columns
                End Get
                Set(value As Integer)
                    Me.Columns = value
                End Set
            End Property

            Public Property TextareaRows As Integer Implements IEditor.TextareaRows
                Get
                    Return Me.Rows
                End Get
                Set(value As Integer)
                    Me.Rows = value
                End Set
            End Property




            Public Property Editable As Boolean Implements IEditor.Editable
                Get
                    Return Me.Enabled
                End Get
                Set(value As Boolean)
                    Me.Enabled = value
                End Set
            End Property

            ''' <summary>
            ''' Escape sequence at the start of the editors text data that proclaims the text being encoded
            ''' </summary>
            Private Const EscapeRecognitionSequence As String = ChrW(27) & "ESC" & ChrW(27)

            Private Property IEditor_Html As String Implements IEditor.Html
                Get
                    If Me.Text <> Nothing AndAlso Me.Text.StartsWith(EscapeRecognitionSequence) Then
                        Me.Text = System.Web.HttpUtility.UrlDecode(Me.Text.Substring(EscapeRecognitionSequence.Length))
                    End If
                    Return Me.Text
                End Get
                Set(value As String)
                    Me.Text = value
                End Set
            End Property

            Private Property IEditor_EnableViewState As Boolean Implements IEditor.EnableViewState
                Get
                    Return Me.EnableViewState
                End Get
                Set(value As Boolean)
                    Me.EnableViewState = value
                End Set
            End Property

            Private Property IEditor_Visible As Boolean Implements IEditor.Visible
                Get
                    Return Me.Visible
                End Get
                Set(value As Boolean)
                    Me.Visible = value
                End Set
            End Property

            Private ReadOnly Property IEditor_ClientID As String Implements IEditor.ClientID
                Get
                    Return Me.ClientID
                End Get
            End Property

        End Class

        ''' <summary>
        '''     The smart and built-in content management system of camm Web-Manager
        ''' </summary>
        ''' <remarks>
        '''     This page contains a web editor which saves/load the content to/from the CWM database. The editor will only be visible for those people with appropriate authorization. All other people will only see the content, but nothing to modify it.
        '''     The content may be different for languages or markets. In all cases, there will be a version history.
        '''     When there is no content for an URL in the database - or it hasn't been released - the page request will lead to an HTTP 404 error code.
        ''' </remarks>
        Public Class SmartPlainHtmlEditor
            Inherits SmartWcmsEditorCommonBase

            Protected Overrides Sub PagePreRender_JavaScriptRegistration()
                Return
            End Sub


            Protected Overrides Function IsEditableWhenBrowsingVersions() As Boolean
                Return False
            End Function

            Protected Overrides Function CanEditCurrentVersion() As Boolean
                Dim releasedVersion As Integer = Me.Database.ReleasedVersion(Me.ContentOfServerID, Me.DocumentID, Me.EditorID)
                Return Me.CurrentVersion > releasedVersion
            End Function

            Private editorMain As IEditor

            Protected Overrides ReadOnly Property MainEditor As IEditor
                Get
                    Return Me.editorMain
                End Get
            End Property

            Public Property CssWidth As String
                Get
                    Return Me.editorMain.CssWidth
                End Get
                Set(value As String)
                    Me.editorMain.CssWidth = value
                End Set
            End Property

            Public Property CssHeight As String
                Get
                    Return Me.editorMain.CssHeight
                End Get
                Set(value As String)
                    Me.editorMain.CssHeight = value
                End Set
            End Property

            Public Property Columns As Integer
                Get
                    Return Me.editorMain.TextareaColumns
                End Get
                Set(value As Integer)
                    Me.editorMain.TextareaColumns = value
                End Set
            End Property

            Public Property Rows As Integer
                Get
                    Return Me.editorMain.TextareaRows
                End Get
                Set(value As Integer)
                    Me.editorMain.TextareaRows = value
                End Set
            End Property

            Public Sub New()
                editorMain = New PlainTextEditor
                editorMain.EnableViewState = False
            End Sub

            Protected Overrides Sub CreateChildControls()
                MyBase.CreateChildControls()
                Controls.Add(CType(editorMain, System.Web.UI.Control))
            End Sub

            Protected SaveButton As System.Web.UI.WebControls.Button = Nothing
            Protected ActivateButton As System.Web.UI.WebControls.Button = Nothing
            Protected PreviewButton As System.Web.UI.WebControls.Button = Nothing
            Protected NewVersionButton As System.Web.UI.WebControls.Button = Nothing
            Protected DeleteLanguageButton As System.Web.UI.WebControls.Button = Nothing

            Protected VersionDifferenceLabel As System.Web.UI.WebControls.Label = Nothing

            Protected VersionDropDownBox As New System.Web.UI.WebControls.DropDownList
            Protected LanguagesDropDownBox As System.Web.UI.WebControls.DropDownList

            Private Sub CreateToolBarButtons()
                ''WARNING following isDirty check must check for all editors (and not just editor with name of Me.ClientID)
                'Dim EncodeRawDataJScriptSnippet As String = Nothing
                'Dim AllEditorControls As New List(Of System.Web.UI.Control)
                'Me.FindSWcmsControlsOnThisPage(AllEditorControls, Me.Page.Controls)
                'For Each Editor As System.Web.UI.Control In AllEditorControls
                '    'EncodeRawDataJScriptSnippet &= String.Format("EncodeRawDataIfNotEncoded (document.getElementById('{0}')); ", CType(Editor, SmartWcmsEditorCommonBase).EditorClientID)
                'Next

                Me.SaveButton = New System.Web.UI.WebControls.Button()
                AssignOnClientClickAttribute(Me.SaveButton, "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'update'; unbindCloseCheck(); ExecPostBack('SaveButton', 'Click', true);") ' " & EncodeRawDataJScriptSnippet & "; document.forms['" & LookupParentServerFormName() & "'].submit(); return false;")
                Me.SaveButton.Text = "Save"

                Me.ActivateButton = New System.Web.UI.WebControls.Button
                AssignOnClientClickAttribute(Me.ActivateButton, "document.getElementById('" & Me.txtActivate.ClientID & "').value = 'activate'; unbindCloseCheck(); ExecPostBack('ActivateButton', 'Click', true);") '" & EncodeRawDataJScriptSnippet & "; document.forms['" & LookupParentServerFormName() & "'].submit(); return false;")
                Me.ActivateButton.Text = "Save & Activate"
                Me.ActivateButton.ToolTip = "Save & Activate all languages/markets of this version"

                Me.PreviewButton = New System.Web.UI.WebControls.Button()
                AssignOnClientClickAttribute(Me.PreviewButton, "document.getElementById('" & Me.txtEditModeRequested.ClientID & "').value = 'false'; unbindCloseCheck(); ExecPostBack('PreviewButton', 'Click', true);") '" & EncodeRawDataJScriptSnippet & "; document.forms['" & LookupParentServerFormName() & "'].submit(); return false;")
                Me.PreviewButton.Text = "Preview"

                Me.NewVersionButton = New System.Web.UI.WebControls.Button()
                AssignOnClientClickAttribute(Me.NewVersionButton, "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'newversion'; document.getElementById('" & Me.txtEditModeRequested.ClientID & "').value = 'true';  unbindCloseCheck(); ExecPostBack('NewVersionButton', 'Click', true);") '" & EncodeRawDataJScriptSnippet & "; document.forms['" & LookupParentServerFormName() & "'].submit(); return false;")
                Me.NewVersionButton.Text = "New Version"

                Me.VersionDifferenceLabel = New System.Web.UI.WebControls.Label()
                Me.VersionDifferenceLabel.EnableViewState = False
            End Sub

            Private Sub AssignOnClientClickAttribute(control As System.Web.UI.WebControls.Button, script As String)
#If NetFrameWork = "1_1" Then
                control.Attributes("onclick") = script
#Else
                control.OnClientClick = script
#End If
            End Sub

            ''' <summary>
            ''' Fil the verison dropdown box with the versions available for this document.
            ''' <remarks>I have for now overall copied the code from the original smartwcms editor. Needs refactoring </remarks>
            ''' </summary>
            Private Sub InitializeVersionDropDownBox()
                Dim myDataTable As DataTable = Database.ReadAllData(Me.ContentOfServerID, DocumentID, Me.EditorID)
                Dim counter As Integer = 1
                Dim versionRows As DataRow() = myDataTable.Select("LanguageID = " & Me.LanguageToShow.ToString(), "Version DESC")
                Dim currentVersionExists As Boolean = False
                For Each row As DataRow In versionRows
                    If counter = 7 Then
                        Exit For
                    End If
                    Dim version As Integer = CType(row("Version"), Integer)
                    Dim myDate As Date = Utils.Nz(row("ReleasedOn"), CType(Nothing, Date))
                    Dim ReleaseDate As String
                    If myDate = Nothing Then
                        ReleaseDate = "(no release)"
                    Else
                        ReleaseDate = myDate.ToShortDateString()
                    End If
                    Dim listItem As New System.Web.UI.WebControls.ListItem
                    listItem.Text = "v" & version.ToString() & " " & ReleaseDate
                    listItem.Value = version.ToString() & ";" & Me.LanguageToShow.ToString()
                    Me.VersionDropDownBox.Items.Insert(0, listItem)
                    counter += 1

                    If version = CurrentVersion Then
                        currentVersionExists = True
                    End If
                Next
                Dim dropDownScript As String = "document.getElementById('" & txtBrowseToMarketVersion.ClientID & "').value = this.value; "
                Me.VersionDropDownBox.Attributes.Add("onchange", "document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = ''; if(confirmPageClose()){ " & dropDownScript & " this.selectedIndex = -1; unbindCloseCheck(); ExecPostBack('VersionDropDownBox', 'Change', true);  } else resetSelectBox(this); ") 'document.forms['" & LookupParentServerFormName() & "'].submit();  } else resetSelectBox(this); ")

                Dim selectedValue As String = Me.CurrentVersion & ";" & LanguageToShow.ToString()

                If currentVersionExists Then
                    Me.VersionDropDownBox.SelectedValue = Me.CurrentVersion & ";" & LanguageToShow.ToString()
                Else
                    Me.VersionDropDownBox.Items.Add("(new)")
                    Me.VersionDropDownBox.SelectedValue = "(new)"
                End If

            End Sub

            ''' <summary>
            ''' Fill the languages/market drop down list...
            ''' </summary>
            Private Sub InitializeLanguageDropDownList()
                Dim languages As AvailableLanguage() = GetAvailableLanguages()
                Me.LanguagesDropDownBox = New System.Web.UI.WebControls.DropDownList
                Dim neutralItem As New System.Web.UI.WebControls.ListItem
                neutralItem.Text = "0 / Neutral / All"
                neutralItem.Value = CurrentVersion.ToString() & ";0"
                LanguagesDropDownBox.Items.Add(neutralItem)


                If Not languages Is Nothing Then
                    For Each language As AvailableLanguage In languages
                        Dim text As String = language.id.ToString() & " / " & language.languageDescriptionEnglish
                        Dim value As String = CurrentVersion.ToString() & ";" & language.id.ToString()
                        Dim item As New System.Web.UI.WebControls.ListItem
                        If Not language.available Then
                            text &= " (inactive)"
                        End If
                        item.Text = text
                        item.Value = value
                        LanguagesDropDownBox.Items.Add(item)
                    Next
                    LanguagesDropDownBox.SelectedValue = CurrentVersion.ToString() & ";" & LanguageToShow.ToString()
                End If

                Dim script As String = "document.getElementById('" & txtBrowseToMarketVersion.ClientID & "').value = this.value;"
                LanguagesDropDownBox.Attributes.Add("onchange", "if(confirmPageClose()){" & script & "; this.selectedIndex = -1; document.forms['" & LookupParentServerFormName() & "'].submit(); } else resetSelectBox(this); ")
            End Sub
            ''' <summary>
            ''' Shows the buttons that we need
            ''' </summary>
            ''' <param name="toolbartype"></param>
            Private Sub AssembleToolbar(ByVal toolbartype As ToolbarSettings)
                If toolbartype = ToolbarSettings.EditEditableVersion Then
                    Me.pnlEditorToolbar.Controls.Add(SaveButton)
                    Me.pnlEditorToolbar.Controls.Add(ActivateButton)
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)
                    If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                        InitializeLanguageDropDownList()
                        Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    End If

                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                ElseIf toolbartype = ToolbarSettings.EditNoneEditableVersions Then
                    Me.pnlEditorToolbar.Controls.Add(NewVersionButton)
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)

                    If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                        InitializeLanguageDropDownList()
                        Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    End If

                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                ElseIf toolbartype = ToolbarSettings.EditWithValidEditVersion Then
                    Me.pnlEditorToolbar.Controls.Add(PreviewButton)
                    If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                        InitializeLanguageDropDownList()
                        Me.pnlEditorToolbar.Controls.Add(Me.LanguagesDropDownBox)
                    End If

                    Me.pnlEditorToolbar.Controls.Add(VersionDropDownBox)
                End If

                If Me.MarketLookupMode <> MarketLookupModes.SingleMarket Then
                    Me.DeleteLanguageButton = New UI.WebControls.Button()
                    Me.DeleteLanguageButton.Text = "Delete/Deactivate this market"
                    AssignOnClientClickAttribute(Me.DeleteLanguageButton, "if(confirmPageClose()) { document.getElementById('" & Me.txtRequestedAction.ClientID & "').value = 'dropcurrentmarket'; ExecPostBack('DropCurrentMarketButton', 'Click', true); } return false; ") 'document.forms['" & LookupParentServerFormName() & "'].submit(); } return false; ")

                    If Me.LanguageToShow <> 0 AndAlso Me.CountAvailableEditVersions() > 1 AndAlso Me.pnlEditorToolbar.Controls.Contains(SaveButton) Then
                        Me.pnlEditorToolbar.Controls.Add(Me.DeleteLanguageButton)
                    End If

                End If

                If toolbartype = ToolbarSettings.EditWithValidEditVersion OrElse toolbartype = ToolbarSettings.EditNoneEditableVersions Then
                    Me.pnlEditorToolbar.Controls.Add(VersionDifferenceLabel)
                End If
            End Sub
            Private Sub SetToolbar(ByVal toolbar As ToolbarSettings)
                CreateToolBarButtons()
                InitializeVersionDropDownBox()

                AssembleToolbar(toolbar)
            End Sub

            Private Sub SetVersionDifferenceLabelText()
                Dim currentVersion As Integer = Me.CurrentVersion
                Dim currentMarket As Integer = Me.LanguageToShow


                If currentVersion > 1 Then
                    Dim differentVersion As Integer = Database.GetFirstPreviousVersionThatDiffers(Me.ContentOfServerID, DocumentID, Me.EditorID, currentMarket, currentVersion)

                    'No version differs, so all are the same since the beginning
                    If differentVersion = 0 Then
                        If Me.Database.IsMarketAvailable(Me.ContentOfServerID, DocumentID, Me.EditorID, currentMarket, 1) Then
                            Me.VersionDifferenceLabel.Text = "Without changes since version v1"
                        End If

                    ElseIf differentVersion = currentVersion - 1 Then
                        Me.VersionDifferenceLabel.Text = "With changes"
                    Else
                        Dim firstSameVersion As Integer = differentVersion + 1
                        'It's possible the market was deactivated before and the version doesn't actually exist
                        If Me.Database.IsMarketAvailable(Me.ContentOfServerID, DocumentID, Me.EditorID, currentMarket, firstSameVersion) Then
                            Me.VersionDifferenceLabel.Text = "Without changes since version v" & (differentVersion + 1).ToString()
                        Else
                            Me.VersionDifferenceLabel.Text = "With changes"
                        End If

                    End If
                End If


            End Sub

            Protected Overrides Sub PagePreRender_InitializeToolbar()
                Dim label As System.Web.UI.WebControls.Label = New System.Web.UI.WebControls.Label()

                If Me.editorMain.Visible Then
                    SetToolbar(Me.ToolbarSetting)
                    SetVersionDifferenceLabelText()
                End If

                If Me.ToolbarSetting = ToolbarSettings.NoVersionAvailable Then
                    label.Text = "No versions available"
                    Me.pnlEditorToolbar.Controls.Add(label)
                End If

                Me.pnlEditorToolbar.Visible = Me.editorMain.Visible

            End Sub

            ''' <summary>
            ''' Always provide client scripts (only in Edit mode) for IsDirty detection
            ''' </summary>
            Private Sub SmartPlainHtmlEditor_PreRender(sender As Object, e As EventArgs) Handles MyBase.PreRender
                If Me.EditModeActive Then
                    'following isDirty check must check for all editors (and not just editor with name of EditorMain.ClientID)
#If NetFrameWork = "1_1" Then
                    Dim AllEditorControls As New ArrayList
#Else
                    Dim AllEditorControls As New List(Of System.Web.UI.Control)
#End If
                    Me.FindSWcmsControlsOnThisPage(AllEditorControls, Me.Page.Controls)
                    Dim IsDirtyChecksJScriptSnippet As String = Nothing
                    For Each Editor As System.Web.UI.Control In AllEditorControls
                        Dim EditorInstanceIsDirtyScript As String = "function isDirty_" & Editor.ClientID & "() " & vbNewLine & _
                                                                   "{  " & vbNewLine & _
                                                                   "var editor = document.getElementById('" & CType(Editor, SmartWcmsEditorCommonBase).EditorClientID & "'); " & vbNewLine & _
                                                                   "if(editor && (editor.defaultValue != editor.value) && (editor.defaultValue != EncodeRawData(editor)))" & vbNewLine & _
                                                                   "    return true; " & vbNewLine & _
                                                                   "else " & vbNewLine & _
                                                                   "    return false;  " & vbNewLine & _
                                                                   "}" & vbNewLine
#If NetFrameWork = "1_1" Then
                        Me.Page.RegisterClientScriptBlock("IsDirty_" & Editor.ClientID, EditorInstanceIsDirtyScript)
#Else
                        Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "IsDirty_" & Editor.ClientID, EditorInstanceIsDirtyScript, True)
#End If
                        If IsDirtyChecksJScriptSnippet <> "" Then IsDirtyChecksJScriptSnippet &= " && "
                        IsDirtyChecksJScriptSnippet &= "(isDirty_" & Editor.ClientID & "())"
                    Next
                    Dim IsDirtySnippet As String = "function isDirty() " & vbNewLine & _
                                                                   "{  " & vbNewLine & _
                                                                   "    return (" & IsDirtyChecksJScriptSnippet & ");  " & vbNewLine & _
                                                                   "}" & vbNewLine & _
                                                                   ""
#If NetFrameWork = "1_1" Then
                    Me.Page.RegisterClientScriptBlock("IsDirty", IsDirtySnippet)
#Else
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "IsDirty", IsDirtySnippet, True)
#End If

                    'Single script instances
                    Const ExecPostBackSnippet As String = "function ExecPostBack(caller, event, ensureUnbindedCloseCheck) " & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "    if(ensureUnbindedCloseCheck) unbindCloseCheck(); " & vbNewLine & _
                                                                   "    __doPostBack (caller, event);" & vbNewLine & _
                                                                   "} " & vbNewLine & _
                                                                   ""
                    Dim UnbindCloseCheckSnippet As String = "function unbindCloseCheck() " & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "    if(window.removeEventListener) window.removeEventListener('beforeunload', closeCheck); " & vbNewLine & _
                                                                   "} " & vbNewLine & _
                                                                   "var documentForm = document.forms['" & LookupParentServerFormName() & "']; " & vbNewLine & _
                                                                   "if(documentForm.addEventListener) " & vbNewLine & _
                                                                   "    documentForm.addEventListener('submit', function(e) { if(window.removeEventListener) window.removeEventListener('beforeunload', closeCheck); });" & vbNewLine & _
                                                                   ""
                    Const ConfirmPageCloseSnippet As String = "function confirmPageClose() " & vbNewLine & _
                                                                   "{" & vbNewLine & _
                                                                   "var result = false; " & vbNewLine & _
                                                                   "if(isDirty()) " & vbNewLine & _
                                                                   "    result = confirm('Do you want to leave? All your changes will be lost'); " & vbNewLine & _
                                                                   "else " & vbNewLine & _
                                                                   "    { " & vbNewLine & _
                                                                   "    result = true " & vbNewLine & _
                                                                   "    } " & vbNewLine & _
                                                                   "if(result) " & vbNewLine & _
                                                                   "    unbindCloseCheck(); " & vbNewLine & _
                                                                   "return result; " & vbNewLine & _
                                                                   "}" & vbNewLine
                    Const ResetSelectBoxSnippet As String = "function resetSelectBox(box)" & vbNewLine & _
                                                                   "{ " & vbNewLine & _
                                                                   "for(var i = 0; i < box.options.length; i++) " & vbNewLine & _
                                                                   "    {	" & vbNewLine & _
                                                                   "    if(box.options[i].defaultSelected) " & vbNewLine & _
                                                                   "        {" & vbNewLine & _
                                                                   "        box.options[i].selected = true; " & vbNewLine & _
                                                                   "        return;	" & vbNewLine & _
                                                                   "        } " & vbNewLine & _
                                                                   "    }" & vbNewLine & _
                                                                   "}" & vbNewLine
                    Const CloseCheckSnippet As String = "function closeCheck(e) " & vbNewLine & _
                                                                       "{ " & vbNewLine & _
                                                                       "if(!isDirty())  " & vbNewLine & _
                                                                       "    return; " & vbNewLine & _
                                                                       "var confirmationMessage = 'Do you want to close this site without saving your changes?';  " & vbNewLine & _
                                                                       "e.returnValue = confirmationMessage; " & vbNewLine & _
                                                                       "return confirmationMessage;" & vbNewLine & _
                                                                       "} " & vbNewLine & _
                                                                       "if(window.addEventListener) " & vbNewLine & _
                                                                       "    window.addEventListener(""beforeunload"", closeCheck);" & vbNewLine & _
                                                                       ""
#If NetFrameWork = "1_1" Then
                    Me.Page.RegisterClientScriptBlock("ExecPostBack", ExecPostBackSnippet)
                    Me.Page.RegisterClientScriptBlock("UnbindCloseCheck", UnbindCloseCheckSnippet)
                    Me.Page.RegisterClientScriptBlock("confirmPageClose", ConfirmPageCloseSnippet)
                    Me.Page.RegisterClientScriptBlock("ResetSelectBox", ResetSelectBoxSnippet)
                    Me.Page.RegisterClientScriptBlock("CloseCheck", CloseCheckSnippet)
#Else
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "ExecPostBack", ExecPostBackSnippet, True)
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "UnbindCloseCheck", UnbindCloseCheckSnippet, True)
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "confirmPageClose", ConfirmPageCloseSnippet, True)
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "ResetSelectBox", ResetSelectBoxSnippet, True)
                    Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "CloseCheck", CloseCheckSnippet, True)
#End If

                    'Enforce __doPostBack javascript function being existent
#If NetFrameWork = "1_1" Then
                    Me.Page.GetPostBackEventReference(Me, "")
#Else
                    Me.Page.ClientScript.GetPostBackEventReference(Me, String.Empty)
#End If

                End If
            End Sub
        End Class

        <Obsolete("Better use one of the cammWM.SmartEditor controls instead", True), System.ComponentModel.EditorBrowsable(EditorBrowsableState.Never)> Public Class sWcmsRadEditor3
            Inherits System.Web.UI.Control

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Modules.WebEdit.Controls.SmartWcms
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The smart and built-in content management system of camm Web-Manager
        ''' </summary>
        ''' <remarks>
        '''     This page contains a web editor which saves/load the content to/from the CWM database. The editor will only be visible for those people with appropriate authorization. All other people will only see the content, but nothing to modify it.
        '''     The content may be different for languages or markets. In all cases, there will be a version history.
        '''     When there is no content for an URL in the database - or it hasn't been released - the page request will lead to an HTTP 404 error code.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	29.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Better use one of the cammWM.SmartEditor controls instead")> Public Class SmartWcms3
            Inherits SmartPlainHtmlEditor

            Property CachedData_WarningAlreadySent As Boolean
                Get
                    Return CType(Me.Page.Cache("CachedData_WarningAlreadySent|" & Me.Page.Request.Url.AbsolutePath), Boolean)
                End Get
                Set(value As Boolean)
                    Me.Page.Cache("CachedData_WarningAlreadySent|" & Me.Page.Request.Url.AbsolutePath) = value
                End Set
            End Property

            Private Sub SendWarningMail(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
                If CachedData_WarningAlreadySent = False Then
                    CachedData_WarningAlreadySent = True
                    Dim PlainTextBody As String = "Please replace control SmartWcms3 and SmartWcms (both obsolete, both provided by cammWM.dll) by another SmartEditor control (see e.g. cammWM.SmartEditor.dll)"
                    Dim HtmlBody As String = System.Web.HttpUtility.HtmlEncode(PlainTextBody)
                    Me.cammWebManager.Log.ReportWarningViaEMail(PlainTextBody, HtmlBody, "WARNING: obsolete SmartEditor to be replaced")
                End If
            End Sub

#Region "For compatibility only: binary interface stays untouched"
            Private _Docs As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific upload folder for documents
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Docs() As String
                Get
                    Return _Docs
                End Get
                Set(ByVal Value As String)
                    _Docs = Value
                    If Me.DocsReadOnly Is Nothing OrElse Me.DocsReadOnly.Length = 0 Then
                        Me.DocsReadOnly = New String() {Value}
                    End If
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
            Private _DocsUploadSizeMax As Integer = 512000
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Max. upload size for documents in Bytes
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property DocsUploadSizeMax() As Integer
                Get
                    Return _DocsUploadSizeMax
                End Get
                Set(ByVal Value As Integer)
                    _DocsUploadSizeMax = Value
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property

            Private _BaseDocsUploadFilter As String() = New String() {"*.rtf", "*.csv", "*.xml", "*.ppt", "*.pptm", "*.pptx", "*.pps", "*.ppsx", "*.pdf", "*.txt", "*.doc", "*.docm", "*.docx", "*.xls", "*.xlsx", "*.xlsm", "*.xlsb", "*.xlt", "*.xltx", "*.xltm", "*.pot", "*.potx", "*.potm", "*.dot", "*.dotx", "*.dotm", "*.xps", "*.odt", "*.ott", "*.odp", "*.otp", "*.ods", "*.ots", "*.odg"}
            Private _DocsUploadFilter As String() = New String() {}
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific upload filter for documents
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            '''     [link]		30.08.2007  Fixed
            '''     [zeutzheim] 10.05.2007  Fixed
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Property DocsUploadFilter() As String()
                Get
                    'Here we also add the upper and lower case of the given filters, otherwise you are not allowed
                    'to upload e.g. test.TXT
                    'This workarround is necessary, because we do not have the source code of rad-editor to disable
                    'case sensitive handling of the filters


                    Dim savecount As Integer = UBound(_BaseDocsUploadFilter)
                    ReDim _DocsUploadFilter((2 * _BaseDocsUploadFilter.Length) - 1)

                    For i As Integer = 0 To savecount
                        _DocsUploadFilter(i + savecount + 1) = _BaseDocsUploadFilter(i).ToUpper
                        _DocsUploadFilter(i) = _BaseDocsUploadFilter(i).ToLower
                    Next

                    Return _DocsUploadFilter
                End Get
                Set(ByVal Value As String())
                    _BaseDocsUploadFilter = Value

                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
            Private _DocsReadOnly As String() = New String() {}
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific readonly folders for documents
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Overloads Property DocsReadOnly() As String()
                Get
                    Return _DocsReadOnly
                End Get
                Set(ByVal Value As String())
                    _DocsReadOnly = Value
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property

            Private _Media As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific upload folder for media files
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Media() As String
                Get
                    Return _Media
                End Get
                Set(ByVal Value As String)
                    _Media = Value
                    If Me.MediaReadOnly Is Nothing OrElse Me.MediaReadOnly.Length = 0 Then
                        Me.MediaReadOnly = New String() {Value}
                    End If
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
            Private _MediaUploadSizeMax As Integer = 512000
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Max. upload size for media files in Bytes
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property MediaUploadSizeMax() As Integer
                Get
                    Return _MediaUploadSizeMax
                End Get
                Set(ByVal Value As Integer)
                    _MediaUploadSizeMax = Value
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
            Private _MediaReadOnly As String() = New String() {}
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific readonly folders for media files
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Overloads Property MediaReadOnly() As String()
                Get
                    Return _MediaReadOnly
                End Get
                Set(ByVal Value As String())
                    _MediaReadOnly = Value
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property

            Private _Images As String
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific upload folder for images
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property Images() As String
                Get
                    Return _Images
                End Get
                Set(ByVal Value As String)
                    _Images = Value
                    If Me.ImagesReadOnly Is Nothing OrElse Me.ImagesReadOnly.Length = 0 Then
                        Me.ImagesReadOnly = New String() {Value}
                    End If
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
            Private _ImagesUploadSizeMax As Integer = 512000
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Max. upload size for images in Bytes
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[adminsupport]	23.02.2006	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Property ImagesUploadSizeMax() As Integer
                Get
                    Return _ImagesUploadSizeMax
                End Get
                Set(ByVal Value As Integer)
                    _ImagesUploadSizeMax = Value
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
            Private _ImagesReadOnly As String() = New String() {}
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Contains the control specific readonly folders for images
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[swiercz]	06.12.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            <System.ComponentModel.TypeConverter(GetType(CompuMaster.camm.WebManager.StringArrayConverter))> Public Overloads Property ImagesReadOnly() As String()
                Get
                    Return _ImagesReadOnly
                End Get
                Set(ByVal Value As String())
                    _ImagesReadOnly = Value
                    If Me.ChildControlsCreated Then
                        'AssignPropertiesToChildEditor()
                    End If
                End Set
            End Property
#End Region

        End Class

        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Modules.WebEdit.Controls.SmartWcms
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The smart and built-in content management system of camm Web-Manager
        ''' </summary>
        ''' <remarks>
        '''     This page contains a web editor which saves/load the content to/from the CWM database. The editor will only be visible for those people with appropriate authorization. All other people will only see the content, but nothing to modify it.
        '''     The content may be different for languages or markets. In all cases, there will be a version history.
        '''     When there is no content for an URL in the database - or it hasn't been released - the page request will lead to an HTTP 404 error code.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	29.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Better use one of the cammWM.SmartEditor controls instead")> Public Class SmartWcms
            Inherits SmartWcms3

        End Class

        ''' <summary>
        ''' Editor control interface
        ''' </summary>
        Public Interface IEditor

            Property Html As String

            Property Editable As Boolean

            Property EnableViewState As Boolean

            Property Visible As Boolean

            ReadOnly Property ClientID As String

            Property CssWidth As String
            Property CssHeight As String

            Property TextareaRows As Integer
            Property TextareaColumns As Integer

        End Interface

    End Namespace

End Namespace