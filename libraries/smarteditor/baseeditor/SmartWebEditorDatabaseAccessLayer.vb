﻿'Copyright 2006,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor

    ''' <summary>
    '''     Provider for loading and saving of data from and to the database
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class SmartWebEditorDatabaseAccessLayer

        Private _swcms As ISmartWebEditor
        Friend Sub New(ByVal swcms As ISmartWebEditor)
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
            Dim myQuery As String =
                    "select LanguageID from webmanager_webeditor where Url = @Url " & vbNewLine &
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
            Dim myQuery As String =
                    "DECLARE @Version int" & vbNewLine &
                    "DECLARE @ReplaceExistingValue bit" & vbNewLine &
                    "DECLARE @ModifiedOn datetime" & vbNewLine &
                    "-- Retrieve editable version" & vbNewLine &
                    "SELECT @Version = IsNull(MAX(Version), 0) + 1" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE IsActive = 1 AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "-- Insert or update data?" & vbNewLine &
                    "SELECT @ReplaceExistingValue = CASE WHEN COUNT(*) = 0 THEN 0 ELSE 1 END" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE Version = @Version AND LanguageID = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "-- Insert new or update existing data" & vbNewLine &
                    "SELECT @ModifiedOn = GETDATE()" & vbNewLine &
                    "IF @ReplaceExistingValue = 1" & vbNewLine &
                    "   UPDATE [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "   SET [ServerID]=@ServerID, [LanguageID]=@LanguageID, [IsActive]=0," & vbNewLine &
                    "       [URL]=@URL, [EditorID]=@EditorID, [Content]=@Content, [ModifiedOn]=@ModifiedOn," & vbNewLine &
                    "       [ModifiedByUser]=@ModifiedByUser, [ReleasedOn]=NULL, " & vbNewLine &
                    "       [ReleasedByUser]=NULL, [Version]=@Version" & vbNewLine &
                    "   WHERE Version = @Version AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "ELSE" & vbNewLine &
                    "   INSERT INTO [dbo].[WebManager_WebEditor](" & vbNewLine &
                    "       [ServerID], [LanguageID], [IsActive], [URL], [EditorID], [Content], [ModifiedOn], " & vbNewLine &
                    "       [ModifiedByUser], [ReleasedOn], [ReleasedByUser], [Version]" & vbNewLine &
                    "       )" & vbNewLine &
                    "   VALUES(" & vbNewLine &
                    "       @ServerID, @LanguageID, 0, " & vbNewLine &
                    "       @URL, @EditorID, @Content, @ModifiedOn, " & vbNewLine &
                    "       @ModifiedByUser, NULL, NULL, " & vbNewLine &
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
            Dim myQuery As String =
                    "DECLARE @Version int" & vbNewLine &
                    "-- Retrieve editable version" & vbNewLine &
                    "SELECT @Version = IsNull(MAX(Version), 0) + 1" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE IsActive = 1 AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "-- Remove rows regarding the defined market" & vbNewLine &
                    "DELETE" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
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
            Dim myQuery As String =
                    "DECLARE @Version int" & vbNewLine &
                    "-- Retrieve latest version" & vbNewLine &
                    "SELECT @Version = IsNull(MAX(Version), 0)" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "-- Release latest version" & vbNewLine &
                    "UPDATE [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "SET [IsActive] = 0" & vbNewLine &
                    "WHERE [URL] = @URL AND [EditorID] = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "UPDATE [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "SET [IsActive] = 1, [ReleasedByUser] = @ReleasedByUser, ReleasedOn = GETDATE()" & vbNewLine &
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
                Dim myQuery As String =
                        "DECLARE @MaxVersion int, @ReleasedVersion int" & vbNewLine &
                        "select @MaxVersion = Max([Version]) from [dbo].[WebManager_WebEditor]" & vbNewLine &
                        "where [URL] = @URL AND [EditorID] = @EditorID and ServerID = @ServerID" & vbNewLine &
                        "select @ReleasedVersion = Max([Version]) from [dbo].[WebManager_WebEditor]" & vbNewLine &
                        "where IsActive = 1 AND [URL] = @URL AND [EditorID] = @EditorID and ServerID = @ServerID" & vbNewLine &
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
                Dim myQuery As String =
                        "select [ServerID],[LanguageID],[IsActive],[URL],[EditorID],[ModifiedOn],[ModifiedByUser],[ReleasedOn],[ReleasedByUser],[Version]" & vbNewLine &
                        "from [dbo].[WebManager_WebEditor]" & vbNewLine &
                        "where [URL] = @URL AND [EditorID] = @EditorID AND ServerID = @ServerID" & vbNewLine &
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
            Dim myQuery As String =
                    "DECLARE @Version int" & vbNewLine &
                    "-- Retrieve editable version" & vbNewLine &
                    "SELECT @Version = IsNull(MAX(Version), 0) + 1" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE IsActive = 1 AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine &
                    "-- Remove any pre-existing data (but regulary, there should be nothing, but sure is sure)" & vbNewLine &
                    "DELETE" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID AND Version = @Version" & vbNewLine &
                    "-- Copy data rows" & vbNewLine &
                    "INSERT INTO [dbo].[WebManager_WebEditor](" & vbNewLine &
                    "   [ServerID], [LanguageID], [IsActive], [URL], [EditorID], [Content], [ModifiedOn], " & vbNewLine &
                    "   [ModifiedByUser], [ReleasedOn], [ReleasedByUser], [Version]" & vbNewLine &
                    "   )" & vbNewLine &
                    "SELECT ServerID, LanguageID, 0, [URL], EditorID, Content, ModifiedOn, ModifiedByUser, NULL, NULL, @Version" & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
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
                Dim myQuery As String =
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
            Dim MyCmd As New SqlClient.SqlCommand(
                    "SELECT [Content] " & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE IsActive = 1 AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine,
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
            Dim command As New SqlClient.SqlCommand(
                "SELECT TOP 1 version FROM dbo.WebManager_WebEditor " & vbNewLine &
                "WHERE content NOT LIKE ( SELECT  content FROM dbo.WebManager_WebEditor a " & vbNewLine &
                "WHERE a.URL = @URL  " & vbNewLine &
                "   AND a.LanguageID = @LanguageID AND a.version = @VERSION AND a.ServerId = @ServerID AND a.EditorID = @EditorID) " & vbNewLine &
                "   AND LanguageID =  @LanguageID AND URL = @URL AND ServerId = @ServerID AND EditorID = @EditorID AND version < @VERSION ORDER BY VERSION DESC ", New SqlClient.SqlConnection(Me.ConnectionString))
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
        ''' <param name="marketID">The exact market ID (no market lookup mode applied)</param>
        ''' <param name="url"></param>
        ''' <param name="editorID"></param>
        ''' <param name="version"></param>
        ''' <returns></returns>
        Public Function ReadContent(ByVal serverID As Integer, ByVal url As String, ByVal editorID As String, ByVal marketID As Integer, ByVal version As Integer) As String
            Dim MyCmd As New SqlClient.SqlCommand(
                    "SELECT [Content] " & vbNewLine &
                    "FROM [dbo].[WebManager_WebEditor]" & vbNewLine &
                    "WHERE Version = @Version AND [LanguageID] = @LanguageID AND [URL] = @URL AND EditorID = @EditorID AND ServerID = @ServerID" & vbNewLine,
                    New SqlClient.SqlConnection(Me.ConnectionString))
            MyCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = serverID
            MyCmd.Parameters.Add("@LanguageID", SqlDbType.Int).Value = marketID
            MyCmd.Parameters.Add("@URL", SqlDbType.NVarChar).Value = url.ToLower
            MyCmd.Parameters.Add("@EditorID", SqlDbType.NVarChar).Value = editorID
            MyCmd.Parameters.Add("@Version", SqlDbType.Int).Value = version
            Return Utils.Nz(Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
        End Function

    End Class

End Namespace