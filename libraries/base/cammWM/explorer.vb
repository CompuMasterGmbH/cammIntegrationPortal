Option Explicit On 
'Option Strict On

Imports System.Data
Imports System.Xml
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Collections
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

'-----------------------------------------------------------------
'Obsolete namespace - use CompuMaster.camm.WebExplorer.* instead
'-----------------------------------------------------------------
Namespace CompuMaster.camm.WebManager.Modules.WebExplorer
#Region "Obsolete WebExplorer"
#Region " Public Class Handling "
    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Class Handling

        Public ReadOnly Property CacheMode() As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
            Get
                Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                Select Case Me.getXmlValue(0, "General", "DownloadLocation", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                    Case "WebServerSession"
                        downloadLocation = DownloadHandler.DownloadLocations.WebManagerUserSession
                    Case "WebManagerUserSession"
                        downloadLocation = DownloadHandler.DownloadLocations.WebManagerUserSession
                    Case "WebManagerSecurityObjectName"
                        downloadLocation = DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                    Case Else
                        downloadLocation = DownloadHandler.DownloadLocations.PublicCache
                End Select
                Return downloadLocation
            End Get
        End Property

#Region " Public Variables "
        Public cultureObj As CultureInfo
        Public AppTitle As String
        Public ColumnHeader_Name As String
        Public ColumnHeader_Type As String
        Public ColumnHeader_LastModified As String
        Public ColumnHeader_Size As String
        Public FileIconTooltip_Edit As String
        Public FileIconTooltip_View As String
        Public FileDownloadTooltip As String
        Public FolderBrowseTooltip As String
        Public ApplicationTopTitle As String
        Public DEditor_DialogTitle As String
        Public Editor_DialogTitle As String
        Public Editor_CreationTime As String
        Public Editor_DeleteQuestionmark As String
        Public Editor_DeleteConfirmation As String
        Public Editor_DeleteButton As String
        Public Editor_SaveButton As String
        Public Editor_CancelButton As String
        Public Viewer_DialogTitle As String
        Public Viewer_BackButton As String
        Public Browse_DialogTitle As String
        Public Modify_Forbidden As String
        Public DataType_Folder As String

        Public pathBoxLblStr As String
        Public yesStr As String
        Public noStr As String
        Public cancelStr As String
        Public uploadStr As String
        Public upToolTipStr As String
        Public newFileToolTipStr As String
        Public newFolderToolTipStr As String
        Public cutBtnTooltipStr As String
        Public copyBtnTooltipStr As String
        Public pasteBtnTooltipStr As String
        Public deleteBtnTooltipStr As String
        Public renameBtnTooltipStr As String
        Public selectAllStr As String
        Public mbDeleteHeaderStr As String
        Public mbDeleteMsgStr As String
        Public mbPasteHeaderStr As String
        Public mbPasteMsgStr As String
        Public btnGoToolTipStr As String
        Public NewFolderMsgBoxHeaderString As String
        Public NewFileMsgBoxHeaderString As String
        Public hideStr As String
        Public unhideStr As String
        Public btnShowLineTrueTooltipStr As String
        Public btnShowLineFalseTooltipStr As String
        Public overWriteAllStr As String
        Public iterateNumericStr As String
        Public iterateByDateStr As String
        Public newName_TextBoxTemplateStr As String
        Public german_textBoxTemplateStr As String
        Public english_textBoxTemplateStr As String
        Public hiddenStr_toAttachwith_fName As String

        Dim XmlConfigDoc As New System.Xml.XmlDocument
        Dim XmlConfigFileLoaded As String

        Dim Config_PathStart As String
        Dim Config_AuthAppTitle2ViewOnly As String
        Dim Config_AuthAppTitle2ReadWrite As String
        Dim Config_ReadWriteModeEnabled As Boolean = False
        Dim Config_BrowseFileFilter As String = "*.*"
        Dim Config_BrowseSubFolders As Boolean = True
        Dim Config_RegexEnabled As Boolean
        Dim Config_ExpressionPlaceholder As String
        Dim Config_RegexExpression_Searches As String
        Dim Config_RegexExpression_Browsing As String '=== NOT YET IMPLEMENTED === _
        Public ExceedsDownloadFileSizeLimit As String

#End Region

#Region " Properties "
        Public ReadOnly Property ConfigSetting_BrowseSubFolders() As Boolean
            Get
                Return Config_BrowseSubFolders
            End Get
        End Property

        Public ReadOnly Property ConfigSetting_ReadWriteModeEnabled() As Boolean
            Get
                Return Config_ReadWriteModeEnabled
            End Get
        End Property

        Public ReadOnly Property ConfigSetting_RegexExpression_Searches() As String
            Get
                Return Config_RegexExpression_Searches
            End Get
        End Property

#End Region

        '=== NOT YET IMPLEMENTED === _
        'Public ReadOnly Property ConfigSetting_RegexExpression_Browsing() As String
        '    Get
        '        Return Config_RegexExpression_Browsing
        '    End Get
        'End Property

        Public Function PathStart() As String
            Return Config_PathStart
        End Function

        Public Function AuthAppTitle2ViewOnly() As String
            Return Config_AuthAppTitle2ViewOnly
        End Function

        Public Function AuthAppTitle2ReadWrite() As String
            Return Config_AuthAppTitle2ReadWrite
        End Function

        Private Function CheckedForEmptyString(ByVal validate As String, ByVal alternative As String) As String
            If validate = "" Then
                Return alternative
            Else
                Return validate
            End If
        End Function

        Public Sub InitializeLanguage(ByVal LangID As Integer, ByVal physicalPath As String, ByVal serverName As String)
            'Initialize config settings (required)
            Dim confingXMLPath As String = Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"
            Dim myInnerException As Exception = Nothing
            Config_PathStart = getXmlValue(0, "General", "PathStart", , confingXMLPath, myInnerException)
            Config_AuthAppTitle2ViewOnly = getXmlValue(0, "General", "AuthAppTitle2ViewOnly", , confingXMLPath, myInnerException)
            Config_AuthAppTitle2ReadWrite = getXmlValue(0, "General", "AuthAppTitle2ReadWrite", , confingXMLPath, myInnerException)
            If Config_AuthAppTitle2ViewOnly = "" OrElse Config_AuthAppTitle2ReadWrite = "" OrElse Config_PathStart = "" Then
                Dim msg As String = "Config_AuthAppTitle2ViewOnly = " & Config_AuthAppTitle2ViewOnly & _
                " ,Config_AuthAppTitle2ReadWrite = " & Config_AuthAppTitle2ReadWrite & _
                " ,Config_PathStart = " & Config_PathStart & _
                " ,confingXMLPath = " & confingXMLPath & ". "

                Throw New Exception("Configuration file doesn't contains required settings  " & msg, myInnerException)
            End If

            'Additionally, recommended config settings
            If Convert.ToBoolean(CheckedForEmptyString(getXmlValue(0, "General", "ReadWriteModeEnabled", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"), "true")) = True Then
                Config_ReadWriteModeEnabled = True
            Else
                Config_ReadWriteModeEnabled = False
            End If
            Config_BrowseFileFilter = CheckedForEmptyString(getXmlValue(0, "BrowseFilter", "FileFilter", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"), "*.*")
            If Convert.ToBoolean(CheckedForEmptyString(getXmlValue(0, "BrowseFilter", "ShowFolders", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"), "true")) = True Then
                Config_BrowseSubFolders = True
            Else
                Config_BrowseSubFolders = False
            End If

            'Additionally config settings regarding Regex Versioning System
            If Convert.ToBoolean(CheckedForEmptyString(getXmlValue(0, "RegexVersioningSystem", "Enabled", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"), "false")) = True Then
                Config_RegexEnabled = True
            Else
                Config_RegexEnabled = False
            End If
            Config_ExpressionPlaceholder = getXmlValue(0, "RegexVersioningSystem", "ExpressionPlaceholder", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml")
            Config_RegexExpression_Searches = getXmlValue(0, "RegexVersioningSystem", "RegexExpression_Searches", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml")
            Config_RegexExpression_Browsing = getXmlValue(0, "RegexVersioningSystem", "RegexExpression_Browsing", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml")
            If Config_ExpressionPlaceholder = "" OrElse Config_RegexExpression_Searches = "" Then
                Config_RegexEnabled = False
            End If
            'Initialize text string

            Dim CWMModuleWebExplorer_BrowseDialogTitle As String
            ApplicationTopTitle = serverName         'Request.ServerVariables("SERVER_NAME")
            Select Case LangID
                Case 2 'German
                    cultureObj = CultureInfo.CreateSpecificCulture("de-DE")
                    System.Threading.Thread.CurrentThread.CurrentCulture = cultureObj
                    AppTitle = "Root Ordner"
                    ColumnHeader_Name = "Name"
                    ColumnHeader_Type = "Dateityp"
                    ColumnHeader_LastModified = "Letzte Änderung"
                    ColumnHeader_Size = "Größe"
                    FileIconTooltip_Edit = "Bearbeiten"
                    FileIconTooltip_View = "Text anzeigen"
                    FileDownloadTooltip = "Datei öffnen/downloaden"
                    FolderBrowseTooltip = "Ordner öffnen"
                    CWMModuleWebExplorer_BrowseDialogTitle = "WEB EXPLORER"
                    DEditor_DialogTitle = "ORDNER EDITOR"
                    Editor_DialogTitle = "DATEI EDITOR"
                    Editor_CreationTime = "Erstellt am"
                    Editor_DeleteQuestionmark = "Löschen?"
                    Editor_DeleteConfirmation = "Löschen bestätigen"
                    Editor_DeleteButton = "Löschen"
                    Editor_SaveButton = "Speichern"
                    Editor_CancelButton = "Beenden"
                    Viewer_DialogTitle = "TEXT VIEWER"
                    Viewer_BackButton = "Beenden"
                    Modify_Forbidden = "Verboten"
                    DataType_Folder = "Ordner"

                    pathBoxLblStr = "Adresse"
                    yesStr = "Ja"
                    noStr = "Nein"
                    cancelStr = "Abbrechen"
                    uploadStr = "Hochladen"
                    upToolTipStr = "Aufwärts"
                    newFileToolTipStr = "Neue Datei"
                    newFolderToolTipStr = "Neuer Ordner"
                    cutBtnTooltipStr = "Ausschneiden"
                    copyBtnTooltipStr = "Kopieren"
                    pasteBtnTooltipStr = "Einfügen"
                    deleteBtnTooltipStr = "Löschen"
                    renameBtnTooltipStr = "Umbenennen"
                    selectAllStr = "Alles auswählen"
                    mbDeleteHeaderStr = "Löschen von meheren Dateien bestätigen"
                    mbDeleteMsgStr = "Möchten Sie wirklich diese Datei(en) löschen?"
                    mbPasteHeaderStr = "Ersetzen von Elementen bestätigen"
                    mbPasteMsgStr = " Es wurden Objekte mit dem gleichen <br>Namen im Zielordner gefunden. <br>Auf welche Weise möchten Sie fortfahren?"
                    btnGoToolTipStr = "Wechseln zu"
                    NewFolderMsgBoxHeaderString = "Neu Ordner"
                    NewFileMsgBoxHeaderString = "Neu Datei"
                    hideStr = "Verstecken Datei/Ordner"
                    unhideStr = "Aufdecken Datei/Ordner"
                    btnShowLineTrueTooltipStr = "Linie Nr. verstecken"
                    btnShowLineFalseTooltipStr = "Linie Nr. anzeigen"
                    overWriteAllStr = "Alle Überschreiben"
                    iterateNumericStr = "Durchnummerieren"
                    iterateByDateStr = "Aktuelles Datum anhängen"
                    newName_TextBoxTemplateStr = "Neue Name: "
                    german_textBoxTemplateStr = "Deutsch: "
                    english_textBoxTemplateStr = "Englisch: "
                    hiddenStr_toAttachwith_fName = " <-(Versteckt)"
                    Me.ExceedsDownloadFileSizeLimit = "Überschreitet das Dateigrößen Limit!"

                Case 3 'french
                    cultureObj = CultureInfo.CreateSpecificCulture("fr-FR")
                    System.Threading.Thread.CurrentThread.CurrentCulture = cultureObj
                    AppTitle = "Racine Dossier"
                    ColumnHeader_Name = "Nom"
                    ColumnHeader_Type = "Type"
                    ColumnHeader_LastModified = "Bout Modifié"
                    ColumnHeader_Size = "Grandeur"
                    FileIconTooltip_Edit = "Éditer"
                    FileIconTooltip_View = "Vue texte"
                    FileDownloadTooltip = "Ouvert/Télécharger Fichier"
                    FolderBrowseTooltip = "Browse to folder"
                    CWMModuleWebExplorer_BrowseDialogTitle = "Toile Explorateur"
                    DEditor_DialogTitle = "Dossier Éditeur"
                    Editor_DialogTitle = "Fichier Éditeur"
                    Editor_CreationTime = "Temps De Création"
                    Editor_DeleteQuestionmark = "Effacer?"
                    Editor_DeleteConfirmation = "Confirmer Effacer"
                    Editor_DeleteButton = "Effacer"
                    Editor_SaveButton = "Enregistrer"
                    Editor_CancelButton = "Close"
                    Viewer_DialogTitle = "TEXT VIEWER"
                    Viewer_BackButton = "Fermer"
                    Modify_Forbidden = "Interdit"
                    DataType_Folder = "Dossier"



                    pathBoxLblStr = "Adresse"
                    yesStr = "Oui"
                    noStr = "Non"
                    cancelStr = "Annuler"
                    uploadStr = "Transférer"
                    upToolTipStr = "Up"
                    newFileToolTipStr = "Créez Le Nouveau Fichier"
                    newFolderToolTipStr = "Créez Le Nouveau Dossier"
                    cutBtnTooltipStr = "Couper"
                    copyBtnTooltipStr = "Copie"
                    pasteBtnTooltipStr = "Coller"
                    deleteBtnTooltipStr = "Effacer"
                    renameBtnTooltipStr = "Retitrez"
                    selectAllStr = "Choisir Tous"
                    mbDeleteHeaderStr = "Confirm to deletion of objects"
                    mbDeleteMsgStr = "Are you sure to delete these files?"
                    mbPasteHeaderStr = "Confirm replace of Objects"
                    mbPasteMsgStr = " Objects have been found with same <br>names in the destination Folder. <br>Please provide a substitute!"
                    btnGoToolTipStr = "Allez"
                    NewFolderMsgBoxHeaderString = "Nouveau Dossier"
                    NewFileMsgBoxHeaderString = "Nouveau Fichier"
                    hideStr = "Cacher Fichiers/Dossiers"
                    unhideStr = "Montrez Caché Fichiers/Dossiers"
                    btnShowLineTrueTooltipStr = "Numéros De Ligne De Peau"
                    btnShowLineFalseTooltipStr = "Montrez les numéros de Ligne"
                    overWriteAllStr = "Recouvrez, Tous"
                    iterateNumericStr = "Réitérez Numériquement"
                    iterateByDateStr = "Réitérez À Date"
                    newName_TextBoxTemplateStr = "Nouveau Nom: "
                    german_textBoxTemplateStr = "Allemand: "
                    english_textBoxTemplateStr = "Anglais: "
                    hiddenStr_toAttachwith_fName = " <-(Caché)"
                    Me.ExceedsDownloadFileSizeLimit = "Exceeds download filesize limit."


                Case Else 'English
                    cultureObj = CultureInfo.CreateSpecificCulture("en-US")
                    System.Threading.Thread.CurrentThread.CurrentCulture = cultureObj
                    AppTitle = "Root folder"
                    ColumnHeader_Name = "Name"
                    ColumnHeader_Type = "Type"
                    ColumnHeader_LastModified = "Last Modified"
                    ColumnHeader_Size = "Size"
                    FileIconTooltip_Edit = "Edit"
                    FileIconTooltip_View = "View text"
                    FileDownloadTooltip = "Open/Download file"
                    FolderBrowseTooltip = "Browse to folder"
                    CWMModuleWebExplorer_BrowseDialogTitle = "WEB EXPLORER"
                    DEditor_DialogTitle = "FOLDER EDITOR"
                    Editor_DialogTitle = "FILE EDITOR"
                    Editor_CreationTime = "Creation Time"
                    Editor_DeleteQuestionmark = "Delete?"
                    Editor_DeleteConfirmation = "Confirm Delete"
                    Editor_DeleteButton = "Delete"
                    Editor_SaveButton = "Save"
                    Editor_CancelButton = "Close"
                    Viewer_DialogTitle = "TEXT VIEWER"
                    Viewer_BackButton = "Close"
                    Modify_Forbidden = "Forbidden"
                    DataType_Folder = "Folder"



                    pathBoxLblStr = "Address"
                    yesStr = "Yes"
                    noStr = "No"
                    cancelStr = "Cancel"
                    uploadStr = "Upload"
                    upToolTipStr = "Up"
                    newFileToolTipStr = "Create New File"
                    newFolderToolTipStr = "Create New Folder"
                    cutBtnTooltipStr = "Cut"
                    copyBtnTooltipStr = "Copy"
                    pasteBtnTooltipStr = "Paste"
                    deleteBtnTooltipStr = "Delete"
                    renameBtnTooltipStr = "Rename"
                    selectAllStr = "Select All"
                    mbDeleteHeaderStr = "Confirm to deletion of objects"
                    mbDeleteMsgStr = "Are you sure to delete these files?"
                    mbPasteHeaderStr = "Confirm replace of Objects"
                    mbPasteMsgStr = " Objects have been found with same <br>names in the destination Folder. <br>Please provide a substitute!"
                    btnGoToolTipStr = "Go"
                    NewFolderMsgBoxHeaderString = "New Folder"
                    NewFileMsgBoxHeaderString = "New File"
                    hideStr = "Hide Files/Folders"
                    unhideStr = "Show hidden Files/Folders"
                    btnShowLineTrueTooltipStr = "Hide Line Nos."
                    btnShowLineFalseTooltipStr = "Show Line Nos."
                    overWriteAllStr = "Overwrite,All"
                    iterateNumericStr = "Iterate Numerically"
                    iterateByDateStr = "Iterate By Date"
                    newName_TextBoxTemplateStr = "New Name: "
                    german_textBoxTemplateStr = "German: "
                    english_textBoxTemplateStr = "English: "
                    hiddenStr_toAttachwith_fName = " <-(Hidden)"
                    Me.ExceedsDownloadFileSizeLimit = "Exceeds download filesize limit."
            End Select

            If GetTitle(2, physicalPath) = "" Then
                Browse_DialogTitle = CWMModuleWebExplorer_BrowseDialogTitle
            Else
                Browse_DialogTitle = GetTitle(2, physicalPath)
            End If

        End Sub

        Public Function GetTitle(ByVal LangID As Integer, ByVal physicalPath As String) As String
            '4DebuggingPurposesOnly
            '======================== _
            'dim ex as exception
            'dim result as string
            'result = getXmlValue(LangID, "General", "DirTitle",,Mid(physicalPath, 1, InStrRev(physicalPath, "\") - 1) & "\config.xml",ex)
            'if not ex is nothing then throw ex
            'return result
            Return getXmlValue(LangID, "General", "DirTitle", , Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml")
        End Function

        Public Function GetDirectories(ByVal path As String) As String()
            If Config_BrowseSubFolders Then
                Return System.IO.Directory.GetDirectories(path)
            Else
                Return Nothing
            End If

        End Function

        Public Function GetDirectories(ByVal DirectoryInfo As DirectoryInfo) As DirectoryInfo()
            If Config_BrowseSubFolders Then
                Return DirectoryInfo.GetDirectories()
            Else
                Return Nothing
            End If
        End Function

        Public Function GetFiles(ByVal path As String, ByVal physicalPath As String, Optional ByVal ShowSystemFiles As Boolean = False) As String()
            Dim FoundFiles As String()
            Dim MyFile As String = Nothing
            Dim MyCounter As Integer

            FoundFiles = System.IO.Directory.GetFiles(path, Config_BrowseFileFilter)
            For MyCounter = 0 To FoundFiles.Length - 1
                If Not IsOwnSystemFile(MyFile, physicalPath, ShowSystemFiles) Then
                    FoundFiles(MyCounter).Remove(MyCounter, 1)
                End If
            Next

            Return FoundFiles
        End Function

        Public Function GetFiles(ByVal DirectoryInfo As DirectoryInfo, ByVal physicalPath As String, Optional ByVal ShowSystemFiles As Boolean = False) As FileInfo()
            Dim FoundFiles As FileInfo()
            Dim Result As New ArrayList
            Dim ResultFileInfo() As FileInfo = Nothing
            Dim MyFile As FileInfo

            'Get files
            FoundFiles = DirectoryInfo.GetFiles(Config_BrowseFileFilter)
            If FoundFiles Is Nothing Then
                Return Nothing
            Else
                For Each MyFile In FoundFiles
                    If Not IsOwnSystemFile(MyFile.Name, physicalPath, ShowSystemFiles) Then
                        Result.Add(MyFile)
                    End If
                Next
            End If

            If Result.Count > 0 Then
                ReDim ResultFileInfo(Result.Count - 1)
                Dim MyCounter As Integer
                For MyCounter = 0 To Result.Count - 1
                    ResultFileInfo(MyCounter) = CType(Result(MyCounter), System.IO.FileInfo)
                Next
            End If

            Return ResultFileInfo

        End Function

        Private Structure MatchItems
            Dim MaxVersion As Integer
            Dim ItemName As String
            Dim CurItemList As ArrayList
        End Structure

        Public Function GetQueriedVersionedFiles(ByVal path As String, ByVal physicalPath As String, ByVal RegexExpression As String, ByVal QueryValue As String, Optional ByVal QueryUpToValue As String() = Nothing) As String()
            Dim Result() As String
            Result = GetFiles(path, physicalPath, False)
            Result = GetRegexFilteredFiles(Result, RegexExpression, QueryValue, QueryUpToValue)
            Return Result
        End Function

        Public Function GetQueriedVersionedFiles(ByVal DirectoryInfo As DirectoryInfo, ByVal physicalPath As String, ByVal RegexExpression As String, ByVal QueryValue As String, Optional ByVal QueryUpToValue As String() = Nothing) As FileInfo()
            Dim Result As ArrayList
            Dim FoundFiles As FileInfo()
            FoundFiles = GetFiles(DirectoryInfo, physicalPath, False)
            If Not FoundFiles Is Nothing Then
                'Get filtered files as arraylist of type FileInfo
                Result = GetRegexFilteredFiles(New System.collections.ArrayList(FoundFiles), RegexExpression, QueryValue, QueryUpToValue)
                If Result Is Nothing Then
                    Return Nothing
                End If
                'Convert arraylist back to array of FileInfo
                Dim MyCounter As Integer
                If Result.Count > 0 Then
                    ReDim FoundFiles(Result.Count - 1)
                    For MyCounter = 0 To Result.Count - 1
                        FoundFiles(MyCounter) = CType(Result(MyCounter), System.IO.FileInfo)
                    Next
                Else
                    FoundFiles = Nothing
                End If
                Return FoundFiles
            Else
                Return Nothing
            End If
        End Function

        Private Function GetRegexFilteredFiles(ByVal Files As String(), ByVal RegexExpression As String, ByVal QueryValue As String, Optional ByVal QueryUpToValue As String() = Nothing) As String()
            Dim Filter_Placeholder As String = Config_ExpressionPlaceholder
            Dim Filter_Replacement As String = QueryValue
            Dim pat As String = RegexExpression
            Dim additionalpat As String = ""
            pat = pat.Replace(Filter_Placeholder, Filter_Replacement)
            If Not QueryUpToValue Is Nothing AndAlso QueryUpToValue.Length > 0 Then
                For MyCounter As Integer = 0 To QueryUpToValue.GetUpperBound(0)
                    additionalpat &= "|" & RegexExpression
                    additionalpat = additionalpat.Replace(Filter_Placeholder, "" & QueryUpToValue(MyCounter) & "")
                Next
            End If
            pat = "" & pat & "" & additionalpat & ""
            If pat.IndexOf("<version>") < 0 Or pat.IndexOf("<matchno") < 0 Then
                Throw New Exception("Regex expression must contain group names ""<version>"" and ""<matchno>""")
            End If

            ' Compile the regular expression.
            Dim MyRegex As New Regex(pat, RegexOptions.IgnoreCase + RegexOptions.Compiled)
            ' Match the regular expression pattern against a text string.
            Dim CurItems() As MatchItems = Nothing
            For Each FoundItem As String In Files
                Dim RegexMatch As Match = MyRegex.Match(FoundItem)
                While RegexMatch.Success
                    ' Display Group1 and its capture set.
                    Dim CurMatchGroup As Group = RegexMatch.Groups("matchno")
                    Dim CurRegexGroup As Group = RegexMatch.Groups("version")
                    Dim CurItemVersion As Int32

                    'Get Match no. as well as a new arraylist for it
                    Dim MyCounter As Integer
                    mycounter = Get_CurItems_MatchNo(CurItems, CurMatchGroup.ToString)
                    If CurItems Is Nothing Then
                        ReDim CurItems(mycounter)
                        CurItems(mycounter).CurItemList = New ArrayList
                        CurItems(mycounter).ItemName = CurMatchGroup.ToString
                    ElseIf mycounter >= CurItems.Length Then
                        ReDim Preserve CurItems(mycounter)
                        CurItems(mycounter).CurItemList = New ArrayList
                        CurItems(mycounter).ItemName = CurMatchGroup.ToString
                    End If

                    CurItemVersion = CType(CurRegexGroup.Value, System.Int32)
                    If CurItemVersion > CurItems(mycounter).MaxVersion Then
                        CurItems(mycounter).MaxVersion = CurItemVersion
                        CurItems(mycounter).curItemList.Clear()
                        CurItems(mycounter).CurItemList.Add(FoundItem)
                    ElseIf CurItemVersion = CurItems(mycounter).MaxVersion Then
                        CurItems(mycounter).CurItemList.Add(FoundItem)
                    End If
                    ' Advance to the next match.
                    RegexMatch = RegexMatch.NextMatch()
                End While
            Next

            If CurItems Is Nothing Then
                Return Nothing
            Else
                Dim CurItemList As New ArrayList
                Dim MyCounter As Integer
                For mycounter = 0 To CurItems.GetUpperBound(0)
                    If Not CurItems(mycounter).CurItemList Is Nothing Then
                        Dim MyInnerCounter As Integer
                        For MyInnerCounter = 0 To CurItems(mycounter).CurItemList.Count - 1
                            CurItemList.Add(CurItems(mycounter).CurItemList(MyInnerCounter))
                        Next
                    End If
                Next

                If CurItemList Is Nothing Then
                    Return Nothing
                Else
                    Dim Result As String()
                    ReDim Result(CurItemList.Count - 1)
                    For MyCounter = 0 To CurItemList.Count - 1
                        Result(mycounter) = CType(CurItemList(mycounter), String)
                    Next
                    Return Result
                End If
            End If

        End Function

        Private Function GetRegexFilteredFiles(ByVal Files As ArrayList, ByVal RegexExpression As String, ByVal QueryValue As String, Optional ByVal QueryUpToValue As String() = Nothing) As ArrayList
            Dim Filter_Placeholder As String = Config_ExpressionPlaceholder
            Dim Filter_Replacement As String = QueryValue
            Dim pat As String = RegexExpression
            Dim additionalpat As String = ""
            pat = pat.Replace(Filter_Placeholder, Filter_Replacement)
            If Not QueryUpToValue Is Nothing AndAlso QueryUpToValue.Length > 0 Then
                For MyCounter As Integer = 0 To QueryUpToValue.GetUpperBound(0)
                    additionalpat &= "|" & RegexExpression
                    additionalpat = additionalpat.Replace(Filter_Placeholder, "" & QueryUpToValue(MyCounter) & "")
                Next
            End If
            pat = "" & pat & "" & additionalpat & ""
            If pat.IndexOf("<version>") < 0 Or pat.IndexOf("<matchno>") < 0 Then
                Throw New Exception("Regex expression must contain group names ""<version>"" and ""<matchno>""")
            End If
            ' Compile the regular expression.
            Dim MyRegex As New Regex(pat, RegexOptions.IgnoreCase + RegexOptions.Compiled)
            ' Match the regular expression pattern against a text string.
            Dim CurItems() As MatchItems = Nothing
            Dim FoundItem As FileInfo
            For Each FoundItem In Files
                Dim RegexMatch As Match = MyRegex.Match(FoundItem.Name)
                While RegexMatch.Success
                    ' Display Group1 and its capture set.
                    Dim CurMatchGroup As Group = RegexMatch.Groups("matchno")
                    Dim CurRegexGroup As Group = RegexMatch.Groups("version")
                    Dim CurItemVersion As Integer

                    'Get Match no. as well as a new arraylist for it
                    Dim MyCounter As Integer
                    mycounter = Get_CurItems_MatchNo(CurItems, CurMatchGroup.ToString)
                    If CurItems Is Nothing Then
                        ReDim CurItems(mycounter)
                        CurItems(mycounter).CurItemList = New ArrayList
                        CurItems(mycounter).ItemName = CurMatchGroup.ToString
                    ElseIf mycounter >= CurItems.Length Then
                        ReDim Preserve CurItems(mycounter)
                        CurItems(mycounter).CurItemList = New ArrayList
                        CurItems(mycounter).ItemName = CurMatchGroup.ToString
                    End If

                    CurItemVersion = CType(CurRegexGroup.Value, System.Int32)
                    If CurItemVersion > CurItems(mycounter).MaxVersion Then
                        CurItems(mycounter).MaxVersion = CurItemVersion
                        CurItems(mycounter).curItemList.Clear()
                        CurItems(mycounter).CurItemList.Add(FoundItem)
                    ElseIf CurItemVersion = CurItems(mycounter).MaxVersion Then
                        CurItems(mycounter).CurItemList.Add(FoundItem)
                    End If

                    ' Advance to the next match.
                    RegexMatch = RegexMatch.NextMatch()
                End While
            Next

            If CurItems Is Nothing Then
                Return Nothing
            Else
                Dim CurItemList As New ArrayList
                Dim MyCounter As Integer
                For mycounter = 0 To CurItems.GetUpperBound(0)
                    If Not CurItems(mycounter).CurItemList Is Nothing Then
                        Dim MyInnerCounter As Integer
                        For MyInnerCounter = 0 To CurItems(mycounter).CurItemList.Count - 1
                            CurItemList.Add(CurItems(mycounter).CurItemList(MyInnerCounter))
                        Next
                    End If
                Next
                Return CurItemList
            End If

        End Function

        Private Function Get_CurItems_MatchNo(ByVal CurItems_MatchNos As MatchItems(), ByVal KeyName As String) As Integer
            Dim MyCounter As Integer
            If CurItems_MatchNos Is Nothing Then
                Return MyCounter
            Else
                For MyCounter = 0 To CurItems_MatchNos.Length - 1
                    If CurItems_MatchNos(MyCounter).ItemName.ToLower(System.Globalization.CultureInfo.InvariantCulture) = KeyName.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                        Return MyCounter
                    End If
                Next
                Return MyCounter
            End If
        End Function

        Public Function getXmlValueHT(ByVal LangID As Integer, ByVal ConfigType As String, ByVal ValueName As String, Optional ByVal XMLFile As String = "", Optional ByRef buf_ex As Exception = Nothing) As System.Collections.Hashtable
            Dim XMLLangNode As System.Xml.XmlElement
            Dim XMLConfigTypeNode As System.Xml.XmlElement
            Dim XMLValueNodes As System.Xml.XmlNodeList
            Dim XMLvalueTestNode As System.Xml.XmlElement
            Dim FileContent As String

            If XMLFile <> XmlConfigFileLoaded Then
                If File.Exists(XMLFile) Then
                    Dim fs As FileStream
                    Dim sr As StreamReader
                    fs = New FileStream(XMLFile, FileMode.Open, FileAccess.Read)
                    sr = New StreamReader(fs)
                    FileContent = sr.ReadToEnd
                    fs.Close()
                    sr.close()
                Else
                    buf_ex = New Exception("Configuration file not found")
                    Return Nothing
                End If
                XmlConfigDoc.LoadXml(FileContent)

                XmlConfigFileLoaded = XMLFile
            ElseIf XMLFile = "" And XmlConfigFileLoaded = "" Then
                If File.Exists(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml") Then
                    Dim fs As FileStream
                    Dim sr As StreamReader
                    fs = New FileStream(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml", FileMode.Open, FileAccess.Read)
                    sr = New StreamReader(fs)
                    FileContent = sr.ReadToEnd
                    fs.Close()
                    sr.close()
                Else
                    buf_ex = New Exception("Index data file not found")
                    Return Nothing
                End If
                XmlConfigDoc.LoadXml(FileContent)
                XmlConfigFileLoaded = PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
            End If

            Try
                XMLLangNode = XmlConfigDoc.DocumentElement.SelectSingleNode("LanguageID" & LangID.ToString)
                If XMLLangNode Is Nothing Then
                    buf_ex = New Exception("Language node not found")
                    Return Nothing
                    Exit Try
                End If
                XMLConfigTypeNode = XMLLangNode.SelectSingleNode(ConfigType)
                If XMLConfigTypeNode Is Nothing Then
                    buf_ex = New Exception("Config type node not found")
                    Return Nothing
                    Exit Try
                End If
                XMLValueNodes = XMLConfigTypeNode.SelectNodes(ValueName)

                If XMLValueNodes Is Nothing OrElse XMLValueNodes.Count = 0 Then
                    buf_ex = New Exception("Config item value not found")
                    Return Nothing
                    Exit Try
                End If

                Dim ht As New Hashtable
                'Dim Results(XMLValueNodes.Count - 1) As String
                'Dim NodeCounter As Integer
                'For NodeCounter = 0 To XMLValueNodes.Count - 1
                '    'Results(NodeCounter) = XMLValueNodes(NodeCounter).InnerText

                'Next

                Dim i As Integer = 0
                For Each XMLvalueTestNode In XMLValueNodes
                    If XMLvalueTestNode.HasAttribute("name") Then
                        ht.Add(XMLvalueTestNode.Attributes("name").Value, XMLvalueTestNode.InnerText)
                        'Results(i) = XMLvalueTestNode.Attributes("name").Value
                    End If
                Next

                Return ht
            Catch ex As Exception
                buf_ex = ex
                Return Nothing
            End Try
        End Function

        Public Function getXmlAttributes(ByVal LangID As Integer, ByVal ConfigType As String, ByVal ValueName As String, Optional ByVal AttributeName As String = "", Optional ByVal XMLFile As String = "", Optional ByRef buf_ex As Exception = Nothing) As String()
            Dim XMLLangNode As System.Xml.XmlElement
            Dim XMLConfigTypeNode As System.Xml.XmlElement
            Dim XMLValueNodes As System.Xml.XmlNodeList
            Dim XMLvalueTestNode As System.Xml.XmlElement
            Dim FileContent As String

            Try
                If XMLFile <> XmlConfigFileLoaded Then
                    If File.Exists(XMLFile) Then
                        Dim fs As FileStream
                        Dim sr As StreamReader
                        fs = New FileStream(XMLFile, FileMode.Open, FileAccess.Read)
                        sr = New StreamReader(fs)
                        FileContent = sr.ReadToEnd
                        fs.Close()
                        sr.close()
                    Else
                        buf_ex = New Exception("Configuration file not found")
                        Return Nothing
                    End If
                    XmlConfigDoc.LoadXml(FileContent)

                    XmlConfigFileLoaded = XMLFile
                ElseIf XMLFile = "" And XmlConfigFileLoaded = "" Then
                    If File.Exists(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml") Then
                        Dim fs As FileStream
                        Dim sr As StreamReader
                        fs = New FileStream(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml", FileMode.Open, FileAccess.Read)
                        sr = New StreamReader(fs)
                        FileContent = sr.ReadToEnd
                        fs.Close()
                        sr.close()
                    Else
                        buf_ex = New Exception("Index data file not found")
                        Return Nothing
                    End If
                    XmlConfigDoc.LoadXml(FileContent)
                    XmlConfigFileLoaded = PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                End If


                XMLLangNode = XmlConfigDoc.DocumentElement.SelectSingleNode("LanguageID" & LangID.ToString)
                If XMLLangNode Is Nothing Then
                    buf_ex = New Exception("Language node not found")
                    Return Nothing
                    Exit Try
                End If
                XMLConfigTypeNode = XMLLangNode.SelectSingleNode(ConfigType)
                If XMLConfigTypeNode Is Nothing Then
                    buf_ex = New Exception("Config type node not found")
                    Return Nothing
                    Exit Try
                End If
                XMLValueNodes = XMLConfigTypeNode.SelectNodes(ValueName)

                If XMLValueNodes Is Nothing OrElse XMLValueNodes.Count = 0 Then
                    buf_ex = New Exception("Config item value not found")
                    Return Nothing
                    Exit Try
                End If

                Dim Results(XMLValueNodes.Count - 1) As String
                Dim NodeCounter As Integer
                For NodeCounter = 0 To XMLValueNodes.Count - 1
                    'Results(NodeCounter) = XMLValueNodes(NodeCounter).InnerText

                Next

                Dim XMLTestNodeList As XmlNodeList
                Dim i As Integer = 0
                For Each XMLvalueTestNode In XMLValueNodes
                    If XMLvalueTestNode.HasAttribute("name") Then
                        Results(i) = XMLvalueTestNode.Attributes("name").Value
                        i += 1
                    End If
                Next


                If AttributeName <> "" Then
                    'Dim XMLTestNodeList As XmlNodeList
                    XMLTestNodeList = XMLConfigTypeNode.SelectNodes(ValueName)
                    If XMLTestNodeList Is Nothing Then
                        buf_ex = New Exception("Setting node not found")
                        Return Nothing
                        Exit Try
                    End If
                    For Each XMLvalueTestNode In XMLTestNodeList
                        If XMLvalueTestNode.HasAttribute("name") Then
                            'Results(NodeCounter) = XMLvalueTestNode.Attributes("name").Value
                        End If
                    Next
                Else
                    'XMLValueNode = XMLConfigTypeNode.SelectSingleNode(ValueName)
                End If

                Return Results
            Catch ex As Exception
                buf_ex = ex
                Return Nothing
            End Try
        End Function

        Public Function getXmlAttribute(ByVal LangID As Integer, ByVal ConfigType As String, ByVal ValueName As String, Optional ByVal innerTextStr As String = "", Optional ByVal XMLFile As String = "", Optional ByRef buf_ex As Exception = Nothing) As String
            Dim XMLLangNode As System.Xml.XmlElement
            Dim XMLConfigTypeNode As System.Xml.XmlElement
            Dim XMLValueNode As System.Xml.XmlElement = Nothing
            Dim XMLvalueTestNode As System.Xml.XmlElement
            Dim FileContent As String

            Try
                If XMLFile <> XmlConfigFileLoaded Then
                    If File.Exists(XMLFile) Then
                        Dim fs As FileStream
                        Dim sr As StreamReader
                        fs = New FileStream(XMLFile, FileMode.Open, FileAccess.Read)
                        sr = New StreamReader(fs)
                        FileContent = sr.ReadToEnd
                        fs.Close()
                        sr.close()
                    Else
                        buf_ex = New Exception("Configuration file not found")
                        Return ""
                    End If
                    XmlConfigDoc.LoadXml(FileContent)

                    XmlConfigFileLoaded = XMLFile
                ElseIf XMLFile = "" And XmlConfigFileLoaded = "" Then
                    If File.Exists(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml") Then
                        Dim fs As FileStream
                        Dim sr As StreamReader
                        fs = New FileStream(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml", FileMode.Open, FileAccess.Read)
                        sr = New StreamReader(fs)
                        FileContent = sr.ReadToEnd
                        fs.Close()
                        sr.close()
                    Else
                        buf_ex = New Exception("Index data file not found")
                        Return ""
                    End If
                    XmlConfigDoc.LoadXml(FileContent)
                    XmlConfigFileLoaded = PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                End If


                XMLLangNode = XmlConfigDoc.DocumentElement.SelectSingleNode("LanguageID" & LangID.ToString)
                If XMLLangNode Is Nothing Then
                    buf_ex = New Exception("Language node not found")
                    Return ""
                    Exit Try
                End If
                XMLConfigTypeNode = XMLLangNode.SelectSingleNode(ConfigType)
                If XMLConfigTypeNode Is Nothing Then
                    buf_ex = New Exception("Config type node not found")
                    Return ""
                    Exit Try
                End If
                If innerTextStr <> "" Then
                    Dim XMLTestNodeList As XmlNodeList
                    XMLTestNodeList = XMLConfigTypeNode.SelectNodes(ValueName)
                    If XMLTestNodeList Is Nothing Then
                        buf_ex = New Exception("Setting node not found")
                        Return ""
                        Exit Try
                    End If
                    For Each XMLvalueTestNode In XMLTestNodeList
                        If XMLvalueTestNode.HasAttribute("name") Then
                            If XMLvalueTestNode.InnerText = innerTextStr Then
                                XMLValueNode = XMLvalueTestNode
                                Exit For
                            End If
                        End If
                    Next
                Else
                    XMLValueNode = XMLConfigTypeNode.SelectSingleNode(ValueName)
                End If

                If XMLValueNode Is Nothing Then
                    Return ""
                    buf_ex = New Exception("Config item value not found")
                    Exit Try
                End If

                'Dim XMLTestNodeList As XmlNodeList
                'Dim i As Integer = 0
                'For Each XMLvalueTestNode In XMLValueNodes
                '    If XMLvalueTestNode.HasAttribute("name") Then
                '        Results(i) = XMLvalueTestNode.Attributes("name").Value
                '        i += 1
                '    End If
                'Next

                Return XMLValueNode.Attributes("name").Value
            Catch ex As Exception
                buf_ex = ex
                Return ""
            End Try
        End Function

        Public Function getXmlValues(ByVal LangID As Integer, ByVal ConfigType As String, ByVal ValueName As String, Optional ByVal AttributeName As String = "", Optional ByVal XMLFile As String = "", Optional ByRef buf_ex As Exception = Nothing) As String()
            Dim XMLLangNode As System.Xml.XmlElement
            Dim XMLConfigTypeNode As System.Xml.XmlElement
            Dim XMLValueNodes As System.Xml.XmlNodeList
            Dim FileContent As String

            If XMLFile <> XmlConfigFileLoaded Then
                If File.Exists(XMLFile) Then
                    Dim fs As FileStream
                    Dim sr As StreamReader
                    fs = New FileStream(XMLFile, FileMode.Open, FileAccess.Read)
                    sr = New StreamReader(fs)
                    FileContent = sr.ReadToEnd
                    fs.Close()
                    sr.close()
                Else
                    buf_ex = New Exception("Configuration file not found")
                    Return Nothing
                End If
                XmlConfigDoc.LoadXml(FileContent)

                XmlConfigFileLoaded = XMLFile
            ElseIf XMLFile = "" And XmlConfigFileLoaded = "" Then
                If File.Exists(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml") Then
                    Dim fs As FileStream
                    Dim sr As StreamReader
                    fs = New FileStream(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml", FileMode.Open, FileAccess.Read)
                    sr = New StreamReader(fs)
                    FileContent = sr.ReadToEnd
                    fs.Close()
                    sr.close()
                Else
                    buf_ex = New Exception("Index data file not found")
                    Return Nothing
                End If
                XmlConfigDoc.LoadXml(FileContent)
                XmlConfigFileLoaded = PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
            End If

            Try
                XMLLangNode = XmlConfigDoc.DocumentElement.SelectSingleNode("LanguageID" & LangID.ToString)
                If XMLLangNode Is Nothing Then
                    buf_ex = New Exception("Language node not found")
                    Return Nothing
                    Exit Try
                End If
                XMLConfigTypeNode = XMLLangNode.SelectSingleNode(ConfigType)
                If XMLConfigTypeNode Is Nothing Then
                    buf_ex = New Exception("Config type node not found")
                    Return Nothing
                    Exit Try
                End If
                XMLValueNodes = XMLConfigTypeNode.SelectNodes(ValueName)

                If XMLValueNodes Is Nothing OrElse XMLValueNodes.Count = 0 Then
                    buf_ex = New Exception("Config item value not found")
                    Return Nothing
                    Exit Try
                End If
                Dim Results(XMLValueNodes.Count - 1) As String
                Dim NodeCounter As Integer
                For NodeCounter = 0 To XMLValueNodes.Count - 1
                    Results(NodeCounter) = XMLValueNodes(NodeCounter).InnerText
                Next
                Return Results
            Catch ex As Exception
                buf_ex = ex
                Return Nothing
            End Try
        End Function

        Public Function getXmlValue(ByVal LangID As Integer, ByVal ConfigType As String, ByVal ValueName As String, Optional ByVal AttributeName As String = "", Optional ByVal XMLFile As String = "", Optional ByRef buf_ex As Exception = Nothing) As String
            Dim XMLLangNode As System.Xml.XmlElement
            Dim XMLConfigTypeNode As System.Xml.XmlElement
            Dim XMLValueNode As System.Xml.XmlElement = Nothing
            Dim XMLvalueTestNode As System.Xml.XmlElement
            Dim FileContent As String

            Try
                If XMLFile <> XmlConfigFileLoaded Then
                    If File.Exists(XMLFile) Then
                        Dim fs As FileStream
                        Dim sr As StreamReader
                        fs = New FileStream(XMLFile, FileMode.Open, FileAccess.Read)
                        sr = New StreamReader(fs)
                        FileContent = sr.ReadToEnd
                        fs.Close()
                        sr.close()
                    Else
                        buf_ex = New Exception("Configuration file not found")
                        Return ""
                    End If
                    XmlConfigDoc.LoadXml(FileContent)

                    XmlConfigFileLoaded = XMLFile
                ElseIf XMLFile = "" And XmlConfigFileLoaded = "" Then
                    If File.Exists(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml") Then
                        Dim fs As FileStream
                        Dim sr As StreamReader
                        fs = New FileStream(PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml", FileMode.Open, FileAccess.Read)
                        sr = New StreamReader(fs)
                        FileContent = sr.ReadToEnd
                        fs.Close()
                        sr.close()
                    Else
                        buf_ex = New Exception("Index data file not found")
                        Return ""
                    End If
                    XmlConfigDoc.LoadXml(FileContent)
                    XmlConfigFileLoaded = PathStart() & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                End If


                XMLLangNode = XmlConfigDoc.DocumentElement.SelectSingleNode("LanguageID" & LangID.ToString)
                If XMLLangNode Is Nothing Then
                    buf_ex = New Exception("Language node not found")
                    Return ""
                    Exit Try
                End If
                XMLConfigTypeNode = XMLLangNode.SelectSingleNode(ConfigType)
                If XMLConfigTypeNode Is Nothing Then
                    buf_ex = New Exception("Config type node not found")
                    Return ""
                    Exit Try
                End If
                If AttributeName <> "" Then
                    Dim XMLTestNodeList As XmlNodeList
                    XMLTestNodeList = XMLConfigTypeNode.SelectNodes(ValueName)
                    If XMLTestNodeList Is Nothing Then
                        buf_ex = New Exception("Setting node not found")
                        Return ""
                        Exit Try
                    End If
                    For Each XMLvalueTestNode In XMLTestNodeList
                        If XMLvalueTestNode.HasAttribute("name") Then
                            If XMLvalueTestNode.Attributes("name").Value = AttributeName Then
                                XMLValueNode = XMLvalueTestNode
                                Exit For
                            End If
                        End If
                    Next
                Else
                    XMLValueNode = XMLConfigTypeNode.SelectSingleNode(ValueName)
                End If

                If XMLValueNode Is Nothing Then
                    Return ""
                    buf_ex = New Exception("Config item value not found")
                    Exit Try
                End If
                Return XMLValueNode.InnerText
            Catch ex As Exception
                buf_ex = ex
                Return ""
            End Try
        End Function

        Public Function GetRelativePath2StartDir(ByVal FullPath As String, ByVal relativePath As String) As String
            Return Mid(FullPath, Len(StrDir(relativePath, True)) + 2)
            'Mid(FullPath, Len(StrDir(Request.Params("path"),True)) + 2)
        End Function

        Public Function GetHackCheckedSubPath(ByVal UncheckedPath As String) As String
            Dim CheckedBuffer_Path As String = UncheckedPath
            CheckedBuffer_Path = Replace(CheckedBuffer_Path, ":", "")
            CheckedBuffer_Path = Replace(CheckedBuffer_Path, "..", "")
            CheckedBuffer_Path = Replace(CheckedBuffer_Path, "{", "")
            CheckedBuffer_Path = Replace(CheckedBuffer_Path, "}", "")
            Do While (Mid(CheckedBuffer_Path, 1, 1) = "/" Or Mid(CheckedBuffer_Path, 1, 1) = "\") And CheckedBuffer_Path <> ""
                CheckedBuffer_Path = Mid(CheckedBuffer_Path, 2)
            Loop
            Return CheckedBuffer_Path
        End Function

        Public Function StrDir(ByVal path As String, Optional ByVal GetStartingPathOnly As Boolean = False) As String
            Dim Topdir As String = PathStart()
            If path = "" Or GetStartingPathOnly = True Then
                Return Topdir
            Else
                Dim checkedBuffer_Path As String
                checkedBuffer_Path = GetHackCheckedSubPath(path)
                checkedBuffer_Path = Topdir & System.IO.Path.DirectorySeparatorChar & checkedBuffer_Path
                Try
                    Dim dummy As Object
                    dummy = New DirectoryInfo(checkedBuffer_Path)
                    Return checkedBuffer_Path
                Catch
                    Return Topdir
                End Try
            End If
        End Function

        Public Function Tempdir(ByVal path As String) As String
            If StrDir(path) <> "" Then
                Return Left(StrDir(path), InStrRev(StrDir(path), "\") - 1)
            Else
                Return Nothing
            End If
        End Function

        Public Function ScriptName() As String
            Dim s1 As String
            s1 = System.Web.HttpContext.Current.Request.ServerVariables("SCRIPT_NAME")
            Return s1.Substring(s1.LastIndexOf("/") + 1)
        End Function

        Public Function PhysicalPath2DownloadURL(ByVal FullPath As String) As String
            Return "download.aspx?file=" & System.Web.HttpUtility.UrlEncode(GetHackCheckedSubPath(Mid(FullPath, Len(StrDir(True)) + 2)))
        End Function

        Public Function IsOwnSystemFile(ByVal TestFileName As String, ByVal physicalPath As String, Optional ByVal IsDebug As Boolean = False) As Boolean

            Dim hiddenFileArrayList As New ArrayList
            Dim xmlFile As String
            xmlFile = Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"
            Dim hiddenFiles As String() = Nothing
            Try
                hiddenFiles = getXmlValues(0, "HideFiles", "File", , xmlFile)
            Catch ex As Exception
                System.Web.HttpContext.Current.Response.Write("---in side catch error occured config.xml...." & "<br>")
            End Try
            Dim hFile As String
            For Each hFile In hiddenFiles
                'Response.Write("-add fiels--" & "<br>")
                hiddenFileArrayList.Add(hFile)
            Next

            Dim returnBool As Boolean = False
            If hiddenFileArrayList Is Nothing Then
                returnBool = False
            ElseIf hiddenFileArrayList.Contains(TestFileName) Then
                returnBool = True
                'End If
                'Return returnBool
            Else
                'old processing of system file identification
                Select Case LCase(TestFileName)
                    Case _
                        "downloadfile.aspx", _
                        "index.aspx", _
                        "explorer.vb", _
                        "explorer.css", _
                        "editor.aspx", _
                        "config.xml", _
                        "deditor.aspx", _
                        "viewer.aspx"
                        'hide own application files
                        returnBool = True
                    Case _
                        "indexdata.xml"
                        If IsDebug = True Then
                            returnBool = False
                        Else
                            returnBool = True
                        End If
                    Case Else
                        returnBool = False
                End Select
            End If
            Return returnBool
        End Function

        Public Function GetWin95IE5CompatibleFileName(ByVal IncompatibleFileName As String) As String
            Dim lastdotpos As Integer

            lastdotpos = InStrRev(IncompatibleFileName, ".")
            If lastdotpos > 0 Then
                Return System.Web.HttpUtility.UrlEncode(Mid(IncompatibleFileName, 1, lastdotpos - 1)) & _
                Mid(IncompatibleFileName, lastdotpos)
            Else
                Return IncompatibleFileName
            End If

        End Function

        Public Function GetFileSize_Formatted(ByVal FileSizeInBytes As Decimal, Optional ByVal LongByteText As Boolean = False) As String
            Dim FileSize As Decimal

            If FileSizeInBytes < 10 ^ 3 Then
                FileSize = FileSizeInBytes
                If LongByteText = True Then
                    Return FileSize.ToString & " Byte"
                Else
                    Return FileSize.ToString & " B"
                End If
            ElseIf FileSizeInBytes < 10 ^ 6 Then
                FileSize = System.Math.Round(FileSizeInBytes / 1024, 2)
                Return FileSize.ToString & " KB"
            ElseIf FileSizeInBytes < 10 ^ 9 Then
                FileSize = System.Math.Round(CType(FileSizeInBytes / 1024 ^ 2, Decimal), 2)
                Return FileSize.ToString & " MB"
            Else
                FileSize = System.Math.Round(CType(FileSizeInBytes / 1024 ^ 2, Decimal), 2)
                Return FileSize.ToString & " GB"
            End If
        End Function



    End Class
#End Region

#Region " Friend Class ChkBoxItemTemplate "
    Friend Class ChkBoxItemTemplate
        Implements ITemplate
        Public Overridable Overloads Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
            Dim chkBox As CheckBox = New CheckBox
            chkBox.ToolTip = "Select files to Delete/Hide"
            'chkBox.AutoPostBack = True
            container.Controls.Add(chkBox)
        End Sub

        Public Sub BindCheckBox(ByVal sender As Object, ByVal e As EventArgs)
            Dim chkBox As CheckBox = CType(sender, CheckBox)
            Dim container As DataGridItem = CType(chkBox.NamingContainer, DataGridItem)
            If container.DataItem("harish").GetType.ToString = "System.DBNull" Then
                chkBox.Checked = False
                'right now dont know how to work with events :( _
            Else
                'right now dont know how to work with events :( _
            End If
        End Sub
    End Class
#End Region

#Region " Friend Class TextBoxTemplate "
    Friend Class TextBoxTemplate
        Implements ITemplate
        Public handle As New Handling
        Dim nameStr As String
        Dim geStr As String
        Dim enStr As String

        Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal name As String, ByVal geName As String, ByVal enName As String)
            MyBase.New()
            nameStr = name
            geStr = geName
            enStr = enName
        End Sub


        Public Overridable Overloads Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
            Dim txtBox As TextBox
            Dim txtBox_En As TextBox
            Dim txtBox_De As TextBox
            'txtBox.AutoPostBack = True
            Dim title As Label
            Dim title_En As Label
            Dim title_De As Label



            'Response.write("im here")
            'AddHandler txtBox.DataBinding, AddressOf BindTextBox
            Dim gridItemTable As Table = New Table
            gridItemTable.CssClass = "GridStyle"
            gridItemTable.Font.Name = "Arial"
            gridItemTable.Font.Size = FontUnit.Point(9)

            Dim tRow As TableRow
            Dim tRow2 As TableRow
            Dim tRow3 As TableRow
            Dim tCell As TableCell

            tRow = New TableRow
            tCell = New TableCell
            title = New Label
            title.Text = nameStr  ' "New Name: "
            tCell.Controls.Add(title)
            tRow.Cells.Add(tCell)

            tCell = New TableCell
            txtBox = New TextBox
            txtBox.ForeColor = System.Drawing.Color.Blue
            txtBox.Width = Unit.Pixel(300)
            txtBox.Height = Unit.Pixel(22)
            tCell.Controls.Add(txtBox)
            tRow.Cells.Add(tCell)
            gridItemTable.Controls.Add(tRow)

            tRow2 = New TableRow
            'tRow2.Visible = False
            tCell = New TableCell
            title_De = New Label
            title_De.Text = geStr '"German: "
            tCell.Controls.Add(title_De)
            tRow2.Cells.Add(tCell)

            tCell = New TableCell
            txtBox_De = New TextBox
            txtBox_De.ForeColor = System.Drawing.Color.Blue
            txtBox_De.Width = Unit.Pixel(300)
            txtBox_De.Height = Unit.Pixel(22)
            tCell.Controls.Add(txtBox_De)
            tRow2.Cells.Add(tCell)
            gridItemTable.Controls.Add(tRow2)

            tRow3 = New TableRow
            'tRow3.Visible = False
            tCell = New TableCell
            title_En = New Label
            title_En.Text = enStr  '"English: "
            tCell.Controls.Add(title_En)
            tRow3.Cells.Add(tCell)

            tCell = New TableCell
            txtBox_En = New TextBox
            txtBox_En.ForeColor = System.Drawing.Color.Blue
            txtBox_En.Width = Unit.Pixel(300)
            txtBox_En.Height = Unit.Pixel(22)
            tCell.Controls.Add(txtBox_En)
            tRow3.Cells.Add(tCell)
            gridItemTable.Controls.Add(tRow3)


            container.Controls.Add(gridItemTable)
            'container.Controls.Add(New PlaceHolder)

        End Sub

        Public Sub BindTextBox(ByVal sender As Object, ByVal e As EventArgs)
            Dim txtBox As TextBox = CType(sender, TextBox)
            Dim container As DataGridItem = CType(txtBox.NamingContainer, DataGridItem)
            'If container.DataItem("Delete").GetType.ToString = "System.DBNull" Then

            'right now dont know how to work with events :( _
            'Else
            'dropDown.Items.Add("DeleteElse")
            'dropDown
            'End If
        End Sub
    End Class
#End Region

    Namespace Pages

#Region " Public Class BaseExplorer "
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never), Obsolete("Use CompuMaster.camm.WebExplorer.* instead")> Public Class BaseExplorer
            Inherits System.Web.UI.Page
            'Compatibility implementation doesn't inherits from InternalBaseExplorer since already compiled solutions expect implemented fields in this class and don't allow FIELDS to be implemented in base classes (but properties/methods work!)

            Protected cammWebManager As WMSystem
            Protected DisplLang As Integer = 1
            Protected handle As New Handling

            Protected Sub PageAuthValidation(ByVal obj As Object, ByVal e As EventArgs) Handles MyBase.Init

                'Load settings
                DisplLang = cammWebManager.UI.MarketID
                handle.InitializeLanguage(DisplLang, Request.PhysicalPath, Request.ServerVariables("SERVER_NAME"))

                'Stop all invalid configurations regarding security object cache mode
                If handle.CacheMode = CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName AndAlso (handle.AuthAppTitle2ViewOnly = Nothing Or LCase(handle.AuthAppTitle2ViewOnly) = "anonymous" Or LCase(handle.AuthAppTitle2ViewOnly) = "@@anonymous") Then
                    Throw New Exception("The download handler cach mode has been setup up to security objects, but no security object or anonymous access has been configured for read access")
                End If

                'Validate access
                Me.cammWebManager.SecurityObject = Me.handle.AuthAppTitle2ViewOnly
                Me.cammWebManager.AuthorizeDocumentAccess(Me.handle.AuthAppTitle2ViewOnly)

            End Sub
        End Class

        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Class InternalBaseExplorer
            Inherits CompuMaster.camm.WebManager.Pages.Page

            Private _DisplLang As Integer = 1
            Protected Property DisplLang() As Integer
                Get
                    Return _DisplLang
                End Get
                Set(ByVal Value As Integer)
                    _DisplLang = Value
                End Set
            End Property
            Private _handle As New Handling
            Protected Property handle() As Handling
                Get
                    Return _handle
                End Get
                Set(ByVal Value As Handling)
                    _handle = Value
                End Set
            End Property

            Protected Sub PageAuthValidation(ByVal obj As Object, ByVal e As EventArgs) Handles MyBase.Init

                'Load settings
                DisplLang = cammWebManager.UI.MarketID
                handle.InitializeLanguage(DisplLang, Request.PhysicalPath, Request.ServerVariables("SERVER_NAME"))

                'Stop all invalid configurations regarding security object cache mode
                If handle.CacheMode = CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName AndAlso (handle.AuthAppTitle2ViewOnly = Nothing Or LCase(handle.AuthAppTitle2ViewOnly) = "anonymous" Or LCase(handle.AuthAppTitle2ViewOnly) = "@@anonymous") Then
                    Throw New Exception("The download handler cache mode has been setup up to security objects, but no security object or anonymous access has been configured for read access")
                End If

                'Validate access
                Me.cammWebManager.SecurityObject = Me.handle.AuthAppTitle2ViewOnly
                Me.cammWebManager.AuthorizeDocumentAccess(Me.handle.AuthAppTitle2ViewOnly)

            End Sub
        End Class
#End Region

#Region " Public Class Explorer "
        <Obsolete("Use CompuMaster.camm.WebExplorer.* instead")> Public Class Explorer
            Inherits InternalBaseExplorer

            <Obsolete("Rename this method to ""PageOnLoad"" to prevent doubled execution")> Protected Overridable Sub Page_Load(ByVal obj As Object, ByVal e As EventArgs)

                If cammWebManager.IsUserAuthorized(handle.AuthAppTitle2ReadWrite) And handle.ConfigSetting_ReadWriteModeEnabled = True Then
                    FileExplorerAdmin()
                Else
                    FileExplorerUser()
                End If

            End Sub

            'Request.Params("path")
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            '--------------------------------------------------------------------------

#Region " Variables "
            Protected WithEvents testBtn As Button
            'need to work on 

            'Private menuTable1 As Table
            Private WithEvents newFile As LinkButton
            Private WithEvents newFolder As LinkButton
            Private WithEvents btnCut As LinkButton
            Private WithEvents btnCopy As LinkButton
            Private WithEvents btnPaste As LinkButton
            Private WithEvents btnDel As LinkButton
            Private WithEvents btnUpload As LinkButton
            Private WithEvents btnRename As LinkButton
            Private WithEvents btnSelectAll As LinkButton
            Private WithEvents btnHide As LinkButton
            Private WithEvents btnUnHide As LinkButton

            Private WithEvents btnRenConfirm As Button
            Private WithEvents btnRenConfirmCancel As Button

            Private pathBoxLbl As Label
            Private WithEvents pathBoxTitle As LinkButton
            Private WithEvents pathBox As TextBox
            Private WithEvents btnGo As LinkButton
            Protected WithEvents pageSizeList As ListBox

            Protected WithEvents renameGrid As New System.Web.UI.WebControls.DataGrid
            Private renameFNArr() As String
            Private renameDNArr() As String
            Protected tCellGrid As TableCell

            'Protected WithEvents btnNew As Button

            Protected fCount As Integer = 0
            Protected dCount As Integer = 0

            Protected WithEvents btnSubmit As Button
            Protected tboxName As System.Web.UI.HtmlControls.HtmlInputText
            Protected file1 As System.Web.UI.HtmlControls.HtmlInputFile

            Protected WithEvents msgYes As Button
            Protected WithEvents msgNo As Button

            Protected WithEvents overWriteAll As Button
            Protected WithEvents iterate As Button
            Protected WithEvents iterateDT As Button
            Protected WithEvents newName As Button
            Protected WithEvents msgCancel As Button

            Protected WithEvents uploadFile As HtmlInputFile
            Protected WithEvents msgUpload As Button
            Protected WithEvents msgUploadCancel As Button

            Protected msgBoxlbl10 As Label 'global variable
            Protected txtBox As TextBox
            Protected WithEvents mBoxNFOk As Button
            Protected WithEvents mBoxNFCancel As Button

            'Protected PlaceBody As PlaceHolder


            Protected WithEvents expUserGrid As New DataGrid

            Protected WithEvents expAdminGrid As New DataGrid
            Protected tCellexpAdminGrid As TableCell
            Protected gridTxtBox As TextBox

            Protected TableBody As Table
            Protected TitleLeftCell As TableCell
            Protected TitleRightCell As TableCell

            Private dirGroup As DirectoryInfo
            Private relativePath As String
            Private fileInfo As fileInfo

            Private sortString As String = "SortColumn,LastModified ASC"

            Private startIndex As Integer = 0
            Private startDirIndex As Integer = 0
            Private startFileIndex As Integer = 0

            Public ReadOnly mbBtnHeight As Integer = 25
            Public ReadOnly bgImgBtnStr As String = "/system/modules/explorer/images/background_symbolbar.jpg"

#End Region


            Private Sub createTable()

                Dim tCell As TableCell = New TableCell
                Dim tRow As TableRow = New TableRow

                Dim tbCell As TableCell = New TableCell
                Dim tbRow As TableRow = New TableRow

                'strDir = D:\Temp
                'Response.buffer = true
                relativePath = Request.Params("path")
                dirGroup = New DirectoryInfo(handle.StrDir(relativePath))
                If Not dirGroup.Exists Then
                    dirGroup = New DirectoryInfo(handle.StrDir(relativePath, True))
                End If

                TableBody.Visible = True
                TableBody.CellPadding = 0
                TableBody.CellSpacing = 0
                TableBody.Rows.Clear()


                Dim header As Table
                'here starts menuTable1 table
                header = getHeader()
                tbRow = New TableRow
                tbCell = New TableCell
                'tbCell.Width = Unit.Percentage(100)
                tbCell.Attributes.Add("background", "/system/modules/explorer/images/background_titlebar.jpg")
                tbCell.Height = Unit.Pixel(25)
                tbCell.Controls.Add(header)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'ends 0th row


                Dim menuTable1 As Table
                'here starts menuTable1 table
                menuTable1 = getMenu1()
                tbRow = New TableRow
                tbCell = New TableCell
                'tbCell.Width = Unit.Percentage(100)
                tbCell.Attributes.Add("background", bgImgBtnStr)
                tbCell.Height = Unit.Pixel(25)
                tbCell.Controls.Add(menuTable1)
                'tbCell.ColumnSpan = 1
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'ends 1st row

                Dim menuTable2 As Table
                menuTable2 = getMenu2()
                tbRow = New TableRow
                tbCell = New TableCell
                tbCell.Attributes.Add("background", bgImgBtnStr)
                tbCell.Controls.Add(menuTable2)
                'tbCell.ColumnSpan = 1
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'Ends 2nd row pathbox

                'to Display delMsgBox
                Dim delMsgBox As Table
                delMsgBox = getDeleteMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(delMsgBox)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'end of 3rd row delete msg box

                'to Display MsgBoxPaste
                Dim msgBoxPaste As Table
                msgBoxPaste = getPasteMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(msgBoxPaste)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'end of 4th row 

                'to Display msgBoxUpload
                Dim msgBoxUpload As Table
                msgBoxUpload = getUploadMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(msgBoxUpload)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'end of 5th row 

                'to Display msgBoxNewFile/Foldera
                Dim msgBoxNewF As Table
                msgBoxNewF = getNewFMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(msgBoxNewF)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'end of 6th row 

                tbRow = New TableRow
                'tRow.Visible = False
                tCellGrid = New TableCell
                'tCellGrid.ColumnSpan = 9
                tbRow.Cells.Add(tCellGrid)
                TableBody.Rows.Add(tbRow) 'end of 7th row 

                Dim mbRow As TableRow = New TableRow
                Dim mbCell As TableCell = New TableCell

                Dim renTable As Table = New Table
                renTable = New Table
                mbRow = New TableRow

                mbCell = New TableCell
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.ColumnSpan = 1
                btnRenConfirm = New Button
                btnRenConfirm.Attributes.Add("background", bgImgBtnStr)
                btnRenConfirm.Text = handle.renameBtnTooltipStr
                btnRenConfirm.ToolTip = handle.renameBtnTooltipStr
                btnRenConfirm.EnableViewState = False
                btnRenConfirm.Height = Unit.Pixel(mbBtnHeight)
                btnRenConfirm.Enabled = True
                mbCell.Controls.Add(btnRenConfirm)
                'mbCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                mbRow.Cells.Add(mbCell)

                mbCell = New TableCell
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.ColumnSpan = 1
                btnRenConfirmCancel = New Button
                btnRenConfirmCancel.Attributes.Add("background", bgImgBtnStr)
                btnRenConfirmCancel.Text = handle.cancelStr
                btnRenConfirmCancel.ToolTip = handle.cancelStr
                btnRenConfirmCancel.EnableViewState = False
                btnRenConfirmCancel.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(btnRenConfirmCancel)
                mbRow.Cells.Add(mbCell)

                renTable.Rows.Add(mbRow)

                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.BackColor = System.Drawing.Color.Silver
                'tCell.ColumnSpan = 9
                tbCell.Controls.Add(renTable)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) '8th row ends'

                tbRow = New TableRow
                'tbRow.Visible = False
                tCellexpAdminGrid = New TableCell
                tCellexpAdminGrid.Width = Unit.Percentage(100)
                'tCellexpAdminGrid.ColumnSpan = 9
                tbRow.Cells.Add(tCellexpAdminGrid)
                TableBody.Rows.Add(tbRow) 'end of 9th row



                tbRow.Cells.Add(tCell)
                TableBody.Rows.Add(tbRow)



            End Sub

            Private Function getHeader() As Table
                Dim tCell As TableCell
                Dim tRow As TableRow
                Dim imgCtrl As Image

                Dim header As Table
                header = New Table
                header.CssClass = "TitleBlue"
                'header.BackColor = System.Drawing.Color.Transparent
                header.CellPadding = 0
                header.CellSpacing = 0

                tRow = New TableRow '0th row 
                tRow.HorizontalAlign = HorizontalAlign.Left
                tRow.VerticalAlign = VerticalAlign.Middle


                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(5)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)



                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                'tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                tCell.Height = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(16)
                imgCtrl.Width = Unit.Pixel(16)
                imgCtrl.ImageUrl = "/system/modules/explorer/images/camm_wm_fileicon.gif"
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(10)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)



                tCell = New TableCell
                tCell.Width = Unit.Percentage(50)
                'tCell.HorizontalAlign = HorizontalAlign.Left
                tCell.Text = handle.Browse_DialogTitle
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.Width = Unit.Percentage(50)
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                tCell.Text = handle.ApplicationTopTitle
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(10)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)

                header.Rows.Add(tRow)

                Return header


            End Function

            Private Function getMenu1() As Table
                Dim tCell As TableCell
                Dim tRow As TableRow

                Dim imgCtrl As Image
                Dim menuTable1 As Table
                menuTable1 = New Table
                menuTable1.BackColor = System.Drawing.Color.Transparent
                menuTable1.CellPadding = 0
                menuTable1.CellSpacing = 0

                tRow = New TableRow '0th row 
                tRow.HorizontalAlign = HorizontalAlign.Left
                tRow.VerticalAlign = VerticalAlign.Middle

                tCell = New TableCell

                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(12)
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_start_symbols.gif"
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                'insert new tCell for upArrow
                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                imgCtrl.ToolTip = handle.upToolTipStr
                If Not dirGroup.Root.FullName = dirGroup.FullName And Not LCase(dirGroup.FullName) = LCase(handle.StrDir(relativePath, True)) Then
                    Dim hLink As HyperLink
                    hLink = New HyperLink
                    hLink.CssClass = "GridStyle"
                    hLink.NavigateUrl = "index.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirGroup.Parent.FullName, relativePath))
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/upfolder.gif"
                    hLink.Controls.Add(imgCtrl)
                    tCell.Controls.Add(hLink)
                Else
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/upfolder_.gif"
                    tCell.Controls.Add(imgCtrl)
                End If
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                tCell.Wrap = True
                newFile = New LinkButton
                newFile.ToolTip = handle.newFileToolTipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/newdoc.gif"
                imgCtrl.ImageAlign = ImageAlign.AbsMiddle
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                newFile.Controls.Add(imgCtrl)
                newFile.CssClass = "GridStyle"
                newFile.EnableViewState = False
                tCell.Controls.Add(newFile)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                newFolder = New LinkButton
                newFolder.Width = Unit.Pixel(20)
                newFolder.Text = "New Folder"
                newFolder.ToolTip = handle.newFolderToolTipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/newfolder.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                newFolder.Controls.Add(imgCtrl)
                newFolder.CssClass = "GridStyle"
                newFolder.EnableViewState = False
                tCell.Controls.Add(newFolder)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnCut = New LinkButton
                btnCut.Width = Unit.Pixel(20)
                btnCut.ID = "buttonCut"
                btnCut.Text = handle.cutBtnTooltipStr
                btnCut.CssClass = "GridStyle"
                btnCut.EnableViewState = False
                btnCut.ToolTip = handle.cutBtnTooltipStr
                btnCut.Enabled = True
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/cut.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnCut.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnCut)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnCopy = New LinkButton
                btnCopy.CssClass = "GridStyle"
                btnCopy.EnableViewState = False
                btnCopy.Enabled = True
                btnCopy.ToolTip = handle.copyBtnTooltipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/copy.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnCopy.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnCopy)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnPaste = New LinkButton
                btnPaste.CssClass = "GridStyle"
                btnPaste.EnableViewState = False
                btnPaste.ToolTip = handle.pasteBtnTooltipStr
                btnPaste.Enabled = True
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/paste.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnPaste.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnPaste)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnDel = New LinkButton
                btnDel.CssClass = "GridStyle"
                btnDel.EnableViewState = False
                btnDel.Enabled = True
                btnDel.ToolTip = handle.deleteBtnTooltipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/delete.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnDel.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnDel)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnRename = New LinkButton
                btnRename.CssClass = "GridStyle"
                btnRename.EnableViewState = False
                btnRename.Enabled = True
                btnRename.ToolTip = handle.renameBtnTooltipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/rename.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnRename.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnRename)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnUpload = New LinkButton
                btnUpload.Text = handle.uploadStr
                btnUpload.CssClass = "GridStyle"
                btnUpload.EnableViewState = False
                btnUpload.Height = Unit.Pixel(20)
                btnUpload.Width = Unit.Pixel(20)
                btnUpload.ToolTip = handle.uploadStr
                btnUpload.Enabled = True
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/upload.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnUpload.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnUpload)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnSelectAll = New LinkButton
                'btnSelectAll.Text = handle.selectAllStr
                btnSelectAll.CssClass = "GridStyle"
                btnSelectAll.EnableViewState = False
                btnSelectAll.Height = Unit.Pixel(20)
                btnSelectAll.Width = Unit.Pixel(20)
                btnSelectAll.ToolTip = handle.selectAllStr
                btnSelectAll.Enabled = True
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/selectall.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnSelectAll.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnSelectAll)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnHide = New LinkButton
                btnHide.CssClass = "GridStyle"
                btnHide.EnableViewState = False
                btnHide.Height = Unit.Pixel(20)
                btnHide.Width = Unit.Pixel(20)
                btnHide.ToolTip = handle.hideStr
                btnHide.Enabled = True
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/hide_files.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnHide.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnHide)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                btnUnHide = New LinkButton
                btnUnHide.CssClass = "GridStyle"
                btnUnHide.EnableViewState = False
                btnUnHide.Height = Unit.Pixel(20)
                btnUnHide.Width = Unit.Pixel(20)
                btnUnHide.ToolTip = handle.unhideStr
                btnUnHide.Enabled = True
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/unhide_files.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnUnHide.Controls.Add(imgCtrl)
                tCell.Controls.Add(btnUnHide)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)



                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_end_symbols.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(5)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.Width = Unit.Percentage(100)
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                'imgCtrl.Width = Unit.Pixel(20)
                tCell.Controls.Add(imgCtrl)
                'tCell.ColumnSpan = 1
                tRow.Cells.Add(tCell)

                menuTable1.Rows.Add(tRow)

                Return menuTable1

            End Function

            Private Function getMenu2() As Table
                Dim tCell As TableCell
                Dim tRow As TableRow

                Dim imgCtrl As Image
                Dim menuTable2 As Table
                menuTable2 = New Table
                menuTable2.BackColor = System.Drawing.Color.Transparent
                menuTable2.CellPadding = 0
                menuTable2.CellSpacing = 0

                tRow = New TableRow '1st row

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(12)
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_start_symbols.gif"
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                'tCell = New TableCell
                'tCell.HorizontalAlign = HorizontalAlign.Right
                'tCell.VerticalAlign = VerticalAlign.Middle
                'tCell.ColumnSpan = 2
                'pathBoxLbl = New Label
                'pathBoxLbl.Font.Name = "Arial"
                'pathBoxLbl.Font.Size = FontUnit.Point(10)
                'pathBoxLbl.BackColor = System.Drawing.Color.Transparent
                'pathBoxLbl.ForeColor = System.Drawing.Color.Black
                'pathBoxLbl.Text = handle.pathBoxLblStr & "&nbsp;"
                'tCell.Controls.Add(pathBoxLbl)
                'tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                'tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 2
                pathBoxTitle = New LinkButton
                'pathBoxTitle.Width = Unit.Pixel(50)
                pathBoxTitle.Font.Name = "Arial"
                pathBoxTitle.Font.Size = FontUnit.Point(10)
                pathBoxTitle.BackColor = System.Drawing.Color.Transparent
                pathBoxTitle.ForeColor = System.Drawing.Color.Black
                pathBoxTitle.Text = handle.pathBoxLblStr
                tCell.Controls.Add(pathBoxTitle)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(4)
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                'tCell.ColumnSpan = 1
                tRow.Cells.Add(tCell)




                'AppTitle = ROOT Ordner
                'Response.write(strDir(True))
                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Left
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 12
                pathBox = New TextBox
                pathBox.Columns = 90
                pathBox.Visible = True
                pathBox.EnableViewState = False
                Dim pathBoxStr As String
                If pathBoxTitle.Text = "Path" Then
                    pathBoxStr = handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)
                Else
                    pathBoxStr = getDisplayLangePath(relativePath)
                End If
                pathBoxStr = pathBoxStr.Replace(System.IO.Path.DirectorySeparatorChar, "/")
                pathBox.Text = "/" & pathBoxStr
                tCell.Controls.Add(pathBox)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                tCell.Wrap = True
                btnGo = New LinkButton
                btnGo.ToolTip = handle.btnGoToolTipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/go.gif"
                imgCtrl.ImageAlign = ImageAlign.AbsMiddle
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnGo.Controls.Add(imgCtrl)
                btnGo.CssClass = "GridStyle"
                btnGo.EnableViewState = False
                tCell.Controls.Add(btnGo)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_end_symbols.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(5)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)



                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                tCell.Width = Unit.Percentage(100)
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                'imgCtrl.Width = Unit.Pixel(20)
                tCell.Controls.Add(imgCtrl)
                'tCell.ColumnSpan = 1
                tRow.Cells.Add(tCell)


                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(12)
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_start_symbols.gif"
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                'to set customized page size    
                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                pageSizeList = New ListBox
                pageSizeList.Enabled = True
                pageSizeList.Visible = True
                pageSizeList.AutoPostBack = True
                pageSizeList.Rows = 1
                'pageSizeList.Height = Unit.Pixel(20)
                'pageSizeList.Width = Unit.Pixel(50)
                pageSizeList.SelectionMode = ListSelectionMode.Single
                pageSizeList.ToolTip = "Select Page Size"
                pageSizeList.Items.Add(10)
                pageSizeList.Items.Add(20)
                pageSizeList.Items.Add(30)
                pageSizeList.Items.Add(40)
                pageSizeList.Items.Add(50)
                pageSizeList.Items.Add("All")
                pageSizeList.Items(1).Selected = True
                tCell.Controls.Add(pageSizeList)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_end_symbols.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(5)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)



                menuTable2.Rows.Add(tRow)
                Return menuTable2
            End Function

            Private Function getDisplayLangePath(ByVal relPath As String) As String
                Dim returnPath As String = ""

                If relPath = "" Then
                    Return ""
                End If

                Dim phyPath As String = handle.StrDir(relPath)
                Dim startIndex As Integer = phyPath.IndexOf(relPath) - 1
                Dim rootPath As String = phyPath.Remove(startIndex, relPath.Length + 1)
                Dim rootName As String = Path.GetFileName(rootPath)

                Dim dir As DirectoryInfo
                dir = New DirectoryInfo(phyPath)
                Dim dirName As String = dir.Name

                Dim parentPath As String
                parentPath = phyPath.Remove(phyPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar), dir.Name.Length + 1)
                dir = New DirectoryInfo(parentPath)
                Dim langPath As String
                langPath = handle.getXmlValue(DisplLang, "FileTitles", "File", dirName, dir.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                If langPath = "" Then
                    returnPath = dirName
                Else
                    returnPath = langPath
                End If

                Do Until rootName = dir.Name
                    phyPath = parentPath
                    parentPath = phyPath.Remove(phyPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar), dir.Name.Length + 1)
                    dirName = dir.Name
                    dir = New DirectoryInfo(parentPath)
                    Dim parentDirName As String
                    parentDirName = handle.getXmlValue(DisplLang, "FileTitles", "File", dirName, dir.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                    If parentDirName = "" Then
                        langPath = dirName
                    Else
                        langPath = parentDirName
                    End If
                    returnPath = langPath & System.IO.Path.DirectorySeparatorChar & returnPath
                Loop
                'Response.Write("-langPath--" & returnPath & "<br>")

                Return returnPath
            End Function

            Private Function getDeleteMsgBox() As Table
                Dim delMsgBox As Table = New Table
                Dim imgCtrl As Image

                delMsgBox.Rows.Clear()
                delMsgBox.BackColor = System.Drawing.Color.Silver

                Dim mbRow As TableRow
                Dim mbCell As TableCell

                mbRow = New TableRow 'delMsgBox 1st row
                mbRow.BorderStyle = BorderStyle.None
                mbRow.BorderWidth = Unit.Empty

                'tRow.Visible = False
                mbCell = New TableCell
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.ColumnSpan = 4
                mbCell.BackColor = System.Drawing.Color.Blue
                mbCell.BorderWidth = Unit.Empty
                mbCell.BorderStyle = BorderStyle.None
                Dim msgBoxlbl1 As Label
                msgBoxlbl1 = New Label
                msgBoxlbl1.BorderWidth = Unit.Empty
                msgBoxlbl1.BorderStyle = BorderStyle.None
                msgBoxlbl1.BackColor = System.Drawing.Color.Blue
                msgBoxlbl1.ForeColor = System.Drawing.Color.White
                msgBoxlbl1.Text = handle.mbDeleteHeaderStr
                mbCell.Controls.Add(msgBoxlbl1)


                mbRow.Cells.Add(mbCell)
                delMsgBox.Rows.Add(mbRow) 'delMsgBox 1st row ends

                mbRow = New TableRow 'delMsgBox 2nd row
                mbRow.BorderStyle = BorderStyle.None


                mbCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/msgbox_delete.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.Controls.Add(imgCtrl)
                mbCell.ColumnSpan = 1
                mbRow.Cells.Add(mbCell)

                'mbRow.Visible = False
                mbCell = New TableCell
                Dim msgBoxlbl2 As Label
                msgBoxlbl2 = New Label
                msgBoxlbl2.Text = handle.mbDeleteMsgStr
                mbCell.Controls.Add(msgBoxlbl2)
                mbCell.ColumnSpan = 3
                mbCell.Height = Unit.Pixel(100)
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.ForeColor = System.Drawing.Color.Black
                mbRow.Cells.Add(mbCell)
                delMsgBox.Rows.Add(mbRow) 'delMsgBox 2nd row ends

                mbRow = New TableRow 'delMsgBox 3rd row
                mbRow.Height = Unit.Pixel(25)
                mbCell = New TableCell
                msgYes = New Button
                msgYes.Text = handle.yesStr
                msgYes.Height = Unit.Pixel(mbBtnHeight)
                msgYes.Width = Unit.Pixel(75)
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.Controls.Add(msgYes)
                mbCell.ColumnSpan = 2
                mbRow.Cells.Add(mbCell)

                mbCell = New TableCell
                msgNo = New Button
                msgNo.Text = handle.noStr
                msgNo.Height = Unit.Pixel(mbBtnHeight)
                msgNo.Width = Unit.Pixel(70)
                mbCell.Controls.Add(msgNo)
                mbCell.ColumnSpan = 2
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent

                mbRow.Cells.Add(mbCell)
                delMsgBox.Rows.Add(mbRow) 'delMsgBox 3 row ends

                Return delMsgBox
            End Function

            Private Function getPasteMsgBox() As Table
                Dim msgBoxPaste As Table = New Table
                Dim imgCtrl As Image
                msgBoxPaste.Rows.Clear()
                msgBoxPaste.BackColor = System.Drawing.Color.Silver

                Dim mbRow As TableRow
                Dim mbCell As TableCell

                mbRow = New TableRow 'MsgBoxPaste 1st row
                mbRow.BorderStyle = BorderStyle.None
                mbRow.BorderWidth = Unit.Empty


                'mbRow.Visible = False
                mbCell = New TableCell
                mbCell.BorderWidth = Unit.Empty
                mbCell.BorderStyle = BorderStyle.None
                Dim msgBoxlbl4 As Label
                msgBoxlbl4 = New Label
                msgBoxlbl4.BorderWidth = Unit.Empty
                msgBoxlbl4.BorderStyle = BorderStyle.None
                msgBoxlbl4.BackColor = System.Drawing.Color.Blue
                msgBoxlbl4.ForeColor = System.Drawing.Color.White
                msgBoxlbl4.Text = handle.mbPasteHeaderStr
                mbCell.Controls.Add(msgBoxlbl4)
                mbCell.ColumnSpan = 5
                mbCell.BackColor = System.Drawing.Color.Blue
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbRow.Cells.Add(mbCell)
                msgBoxPaste.Rows.Add(mbRow) 'MsgBoxPaste 1st row ends

                mbRow = New TableRow 'MsgBoxPaste 2nd row
                mbRow.BorderStyle = BorderStyle.None


                mbCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/msgbox_overwrite.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.Controls.Add(imgCtrl)
                mbCell.ColumnSpan = 1
                mbRow.Cells.Add(mbCell)

                mbCell = New TableCell
                mbCell.Text = handle.mbPasteMsgStr
                mbCell.ColumnSpan = 4
                mbCell.Height = Unit.Pixel(100)
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.ForeColor = System.Drawing.Color.Black
                mbRow.Cells.Add(mbCell)
                msgBoxPaste.Rows.Add(mbRow) 'MsgBoxPaste 2nd row ends


                mbRow = New TableRow 'MsgBoxPaste 3rd row
                mbRow.Height = Unit.Pixel(25)
                'mbRow.Visible = False



                mbCell = New TableCell
                overWriteAll = New Button
                overWriteAll.Text = handle.overWriteAllStr
                overWriteAll.Height = Unit.Pixel(mbBtnHeight)
                'overWriteAll.Width = Unit.Pixel(90)
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.Controls.Add(overWriteAll)
                mbCell.ColumnSpan = 1
                mbRow.Cells.Add(mbCell)

                mbCell = New TableCell
                iterate = New Button
                iterate.Text = handle.iterateNumericStr
                iterate.Height = Unit.Pixel(mbBtnHeight)
                'iterate.Width = Unit.Pixel(90)
                mbCell.Controls.Add(iterate)
                mbCell.ColumnSpan = 1
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbRow.Cells.Add(mbCell)
                msgBoxPaste.Rows.Add(mbRow)

                mbCell = New TableCell
                newName = New Button
                newName.Visible = False
                newName.Text = "Customize"
                newName.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(newName)
                mbCell.ColumnSpan = 1
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                'mbRow.Cells.Add(mbCell)
                'msgBoxPaste.Rows.Add(mbRow)

                mbCell = New TableCell
                iterateDT = New Button
                iterateDT.Text = handle.iterateByDateStr
                iterateDT.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(iterateDT)
                mbCell.ColumnSpan = 1
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbRow.Cells.Add(mbCell)
                msgBoxPaste.Rows.Add(mbRow)

                mbCell = New TableCell
                msgCancel = New Button
                msgCancel.Text = handle.cancelStr
                msgCancel.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(msgCancel)
                mbCell.ColumnSpan = 1
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbRow.Cells.Add(mbCell)
                msgBoxPaste.Rows.Add(mbRow) 'MsgBoxPaste 3 row ends

                Return msgBoxPaste
            End Function

            Private Function getUploadMsgBox() As Table
                Dim msgBoxUpload As Table = New Table
                msgBoxUpload.Rows.Clear()
                msgBoxUpload.BackColor = System.Drawing.Color.Silver

                Dim mbRow As TableRow
                Dim mbCell As TableCell

                mbRow = New TableRow 'msgBox 1st row
                mbRow.BorderStyle = BorderStyle.None
                mbRow.BorderWidth = Unit.Empty

                mbCell = New TableCell
                mbCell.BorderWidth = Unit.Empty
                mbCell.BorderStyle = BorderStyle.None
                Dim msgBoxlbl7 As Label
                msgBoxlbl7 = New Label
                msgBoxlbl7.BorderWidth = Unit.Empty
                msgBoxlbl7.BorderStyle = BorderStyle.None
                msgBoxlbl7.BackColor = System.Drawing.Color.Blue
                msgBoxlbl7.ForeColor = System.Drawing.Color.White
                msgBoxlbl7.Text = "Select File to upload"
                mbCell.Controls.Add(msgBoxlbl7)
                mbCell.ColumnSpan = 2
                mbCell.BackColor = System.Drawing.Color.Blue
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbRow.Cells.Add(mbCell)
                msgBoxUpload.Rows.Add(mbRow) 'msgBoxUpload 1st row ends

                mbRow = New TableRow 'msgBoxUpload 2nd row
                mbRow.BorderStyle = BorderStyle.None
                mbCell = New TableCell
                mbCell.Height = Unit.Pixel(100)
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.ForeColor = System.Drawing.Color.Black
                mbCell.ColumnSpan = 2
                uploadFile = New HtmlInputFile
                mbCell.Controls.Add(uploadFile)
                mbRow.Cells.Add(mbCell)
                msgBoxUpload.Rows.Add(mbRow) 'msgBoxUpload 2nd row ends

                mbRow = New TableRow 'msgBoxUpload 3rd row
                mbRow.Height = Unit.Pixel(25)
                mbCell = New TableCell
                msgUpload = New Button
                msgUpload.Text = handle.uploadStr
                msgUpload.Height = Unit.Pixel(mbBtnHeight)
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.Controls.Add(msgUpload)
                mbCell.ColumnSpan = 1
                mbRow.Cells.Add(mbCell)

                mbCell = New TableCell
                msgUploadCancel = New Button
                msgUploadCancel.Text = handle.cancelStr
                msgUploadCancel.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(msgUploadCancel)
                mbCell.ColumnSpan = 1
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbRow.Cells.Add(mbCell)
                msgBoxUpload.Rows.Add(mbRow) 'msgBoxUpload 3 row ends

                Return msgBoxUpload
            End Function

            Private Function getNewFMsgBox() As Table
                Dim msgBoxNewF As Table = New Table
                msgBoxNewF.Rows.Clear()
                msgBoxNewF.BackColor = System.Drawing.Color.Silver

                Dim mbRow As TableRow
                Dim mbCell As TableCell

                mbRow = New TableRow 'msgBoxNewF 1st row
                mbRow.BorderStyle = BorderStyle.None
                mbRow.BorderWidth = Unit.Empty
                mbCell = New TableCell
                mbCell.BorderWidth = Unit.Empty
                mbCell.BorderStyle = BorderStyle.None
                'global variable
                msgBoxlbl10 = New Label
                msgBoxlbl10.BorderWidth = Unit.Empty
                msgBoxlbl10.BorderStyle = BorderStyle.None
                msgBoxlbl10.BackColor = System.Drawing.Color.Blue
                msgBoxlbl10.ForeColor = System.Drawing.Color.White
                msgBoxlbl10.Text = "New Folder"
                mbCell.ColumnSpan = 3
                mbCell.BackColor = System.Drawing.Color.Blue
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.Controls.Add(msgBoxlbl10)
                mbRow.Cells.Add(mbCell)
                msgBoxNewF.Rows.Add(mbRow) 'msgBoxNewF 1st row ends

                mbRow = New TableRow  'msgBoxNewF 2nd row
                mbRow.HorizontalAlign = HorizontalAlign.Center
                mbRow.VerticalAlign = VerticalAlign.Bottom
                mbRow.Height = Unit.Pixel(50)
                mbCell = New TableCell
                mbCell.BorderWidth = Unit.Empty
                mbCell.BorderStyle = BorderStyle.None
                Dim msgBoxlbl11 As Label
                msgBoxlbl11 = New Label
                msgBoxlbl11.BorderWidth = Unit.Empty
                msgBoxlbl11.BorderStyle = BorderStyle.None
                msgBoxlbl11.BackColor = System.Drawing.Color.Transparent
                msgBoxlbl11.ForeColor = System.Drawing.Color.Black
                msgBoxlbl11.Text = "Name:"
                mbCell.Controls.Add(msgBoxlbl11)
                mbCell.ColumnSpan = 1
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.HorizontalAlign = HorizontalAlign.Center
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbRow.Cells.Add(mbCell)
                'msgBoxNewF.Rows.Add(mbRow)

                mbCell = New TableCell
                'mbCell.Height = Unit.Pixel(100)
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.ForeColor = System.Drawing.Color.Black
                mbCell.ColumnSpan = 1
                txtBox = New TextBox
                txtBox.TextMode = TextBoxMode.SingleLine
                txtBox.Columns = 30
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.Controls.Add(txtBox)
                mbRow.Cells.Add(mbCell)
                'msgBoxNewF.Rows.Add(mbRow) 'msgBoxNewF 2nd row ends

                'msgBoxNewF 3rd row
                mbCell = New TableCell
                mBoxNFOk = New Button
                mBoxNFOk.Text = "Ok"
                mBoxNFOk.Height = Unit.Pixel(mbBtnHeight)
                mBoxNFOk.Width = Unit.Pixel(70)
                mbCell.ColumnSpan = 1
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbCell.Controls.Add(mBoxNFOk)
                mbRow.Cells.Add(mbCell)
                msgBoxNewF.Rows.Add(mbRow)

                mbRow = New TableRow
                mbRow.HorizontalAlign = HorizontalAlign.Center
                mbRow.VerticalAlign = VerticalAlign.Top
                mbRow.Height = Unit.Pixel(40)
                mbCell = New TableCell
                mBoxNFCancel = New Button
                mBoxNFCancel.Text = handle.cancelStr
                mBoxNFCancel.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(mBoxNFCancel)
                mbCell.ColumnSpan = 3
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Top
                mbCell.BackColor = System.Drawing.Color.Transparent
                mbRow.Cells.Add(mbCell)
                msgBoxNewF.Rows.Add(mbRow) 'msgBoxNewF 3 row ends

                Return msgBoxNewF
            End Function

            Private Function getErrorMsgBox() As Table
                Return Nothing
            End Function

            Private Function getUserMenu2() As Table
                Dim tCell As TableCell
                Dim tRow As TableRow
                Dim imgCtrl As Image

                Dim menuTable2 As Table
                menuTable2 = New Table
                menuTable2.BackColor = System.Drawing.Color.Transparent
                menuTable2.CellPadding = 0
                menuTable2.CellSpacing = 0

                tRow = New TableRow '1st row

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(12)
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_start_symbols.gif"
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                imgCtrl.ToolTip = handle.upToolTipStr
                If Not dirGroup.Root.FullName = dirGroup.FullName And Not LCase(dirGroup.FullName) = LCase(handle.StrDir(relativePath, True)) Then
                    Dim hLink As HyperLink
                    hLink = New HyperLink
                    hLink.CssClass = "GridStyle"
                    hLink.NavigateUrl = "index.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirGroup.Parent.FullName, relativePath))
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/upfolder.gif"
                    hLink.Controls.Add(imgCtrl)
                    tCell.Controls.Add(hLink)
                Else
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/upfolder_.gif"
                    tCell.Controls.Add(imgCtrl)
                End If
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)


                'tCell = New TableCell
                'tCell.HorizontalAlign = HorizontalAlign.Right
                'tCell.VerticalAlign = VerticalAlign.Middle
                'tCell.ColumnSpan = 2
                'pathBoxLbl = New Label
                'pathBoxLbl.Font.Name = "Arial"
                'pathBoxLbl.Font.Size = FontUnit.Point(10)
                'pathBoxLbl.BackColor = System.Drawing.Color.Transparent
                'pathBoxLbl.ForeColor = System.Drawing.Color.Black
                'pathBoxLbl.Text = handle.pathBoxLblStr & "&nbsp;"
                'tCell.Controls.Add(pathBoxLbl)
                'tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                'tRow.Cells.Add(tCell)



                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 2
                pathBoxTitle = New LinkButton
                pathBoxTitle.Width = Unit.Pixel(50)
                pathBoxTitle.Font.Name = "Arial"
                pathBoxTitle.Font.Size = FontUnit.Point(10)
                pathBoxTitle.BackColor = System.Drawing.Color.Transparent
                pathBoxTitle.ForeColor = System.Drawing.Color.Black
                pathBoxTitle.Text = handle.pathBoxLblStr
                tCell.Controls.Add(pathBoxTitle)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(4)
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                'tCell.ColumnSpan = 1
                tRow.Cells.Add(tCell)



                'AppTitle = ROOT Ordner
                'Response.write(strDir(True))
                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Left
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 12
                pathBox = New TextBox
                pathBox.Columns = 90
                pathBox.Visible = True
                pathBox.EnableViewState = False
                Dim pathBoxStr As String
                If pathBoxTitle.Text = "Path" Then
                    pathBoxStr = handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)
                Else
                    pathBoxStr = getDisplayLangePath(relativePath)
                End If
                pathBoxStr = pathBoxStr.Replace(System.IO.Path.DirectorySeparatorChar, "/")
                pathBox.Text = "/" & pathBoxStr
                tCell.Controls.Add(pathBox)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                tCell.Wrap = True
                btnGo = New LinkButton
                btnGo.ToolTip = handle.btnGoToolTipStr
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/go.gif"
                imgCtrl.ImageAlign = ImageAlign.AbsMiddle
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                imgCtrl.Width = Unit.Pixel(20)
                btnGo.Controls.Add(imgCtrl)
                btnGo.CssClass = "GridStyle"
                btnGo.EnableViewState = False
                tCell.Controls.Add(btnGo)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_end_symbols.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(5)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Right
                tCell.Width = Unit.Percentage(100)
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/space.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(20)
                'imgCtrl.Width = Unit.Pixel(20)
                tCell.Controls.Add(imgCtrl)
                'tCell.ColumnSpan = 1
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                tCell.Width = Unit.Pixel(25)
                imgCtrl = New Image
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(12)
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_start_symbols.gif"
                tCell.Controls.Add(imgCtrl)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                'to set customized page size    
                tCell = New TableCell
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.VerticalAlign = VerticalAlign.Middle
                tCell.ColumnSpan = 1
                pageSizeList = New ListBox
                pageSizeList.Enabled = True
                pageSizeList.Visible = True
                pageSizeList.AutoPostBack = True
                pageSizeList.Rows = 1
                'pageSizeList.Height=unit.Pixel(20)
                'pageSizeList.Width=unit.Pixel(50)
                pageSizeList.SelectionMode = ListSelectionMode.Single
                pageSizeList.ToolTip = "Select Page Size"
                pageSizeList.Items.Add("10")
                pageSizeList.Items.Add("20")
                pageSizeList.Items.Add("30")
                pageSizeList.Items.Add("40")
                pageSizeList.Items.Add("50")
                pageSizeList.Items.Add("All")
                pageSizeList.Items(1).Selected = True
                tCell.Controls.Add(pageSizeList)
                tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                tRow.Cells.Add(tCell)

                tCell = New TableCell
                imgCtrl = New Image
                imgCtrl.ImageUrl = "/system/modules/explorer/images/splitter_end_symbols.gif"
                imgCtrl.BorderStyle = BorderStyle.None
                imgCtrl.Height = Unit.Pixel(25)
                imgCtrl.Width = Unit.Pixel(5)
                tCell.Controls.Add(imgCtrl)
                tRow.Cells.Add(tCell)



                menuTable2.Rows.Add(tRow)
                Return menuTable2
            End Function

            Private Sub createUserTable()
                'strDir = D:\Temp
                'Response.buffer = true
                relativePath = Request.Params("path")
                dirGroup = New DirectoryInfo(handle.StrDir(relativePath))
                If Not dirGroup.Exists Then
                    dirGroup = New DirectoryInfo(handle.StrDir(relativePath, True))
                End If

                TableBody.Visible = True
                TableBody.CellPadding = 0
                TableBody.CellSpacing = 0
                TableBody.Rows.Clear()

                Dim tCell As TableCell = New TableCell
                Dim tRow As TableRow = New TableRow

                Dim tbCell As TableCell = New TableCell
                Dim tbRow As TableRow = New TableRow

                Dim header As Table
                'here starts menuTable1 table
                header = getHeader()
                tbRow = New TableRow
                tbCell = New TableCell
                'tbCell.Width = Unit.Percentage(100)
                tbCell.Attributes.Add("background", "/system/modules/explorer/images/background_titlebar.jpg")
                tbCell.Height = Unit.Pixel(25)
                tbCell.Controls.Add(header)
                'tbCell.ColumnSpan = 1
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'ends 0th row



                Dim menuTable1 As Table
                'here starts menuTable1 table
                menuTable1 = getMenu1()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Attributes.Add("background", bgImgBtnStr)
                tbCell.Height = Unit.Pixel(25)
                tbCell.Controls.Add(menuTable1)
                'tbCell.ColumnSpan = 1
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'ends 1st row

                Dim menuTable2 As Table
                menuTable2 = getUserMenu2()
                tbRow = New TableRow
                tbCell = New TableCell
                tbCell.Attributes.Add("background", bgImgBtnStr)
                tbCell.Controls.Add(menuTable2)
                'tbCell.ColumnSpan = 1
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'Ends 2nd row pathbox

                'to Display delMsgBox
                Dim delMsgBox As Table
                delMsgBox = getDeleteMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(delMsgBox)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) 'end of 3rd row delete msg box

                'to Display MsgBoxPaste
                Dim msgBoxPaste As Table
                msgBoxPaste = getPasteMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(msgBoxPaste)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) '4th row ends

                'to Display msgBoxUpload
                Dim msgBoxUpload As Table
                msgBoxUpload = getUploadMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(msgBoxUpload)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) '5th row ends

                'to Display msgBoxNewFile/Folder
                Dim msgBoxNewF As Table
                msgBoxNewF = getNewFMsgBox()
                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.Height = Unit.Pixel(250)
                tbCell.BackColor = System.Drawing.Color.White
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.VerticalAlign = VerticalAlign.Bottom
                tbCell.Controls.Add(msgBoxNewF)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) '6th row ends

                tbRow = New TableRow
                'tRow.Visible = False
                tCellGrid = New TableCell
                'tCellGrid.ColumnSpan = 9
                tbRow.Cells.Add(tCellGrid)
                TableBody.Rows.Add(tbRow) '7th row ends

                Dim mbRow As TableRow = New TableRow
                Dim mbCell As TableCell = New TableCell

                Dim renTable As Table = New Table
                renTable = New Table
                mbRow = New TableRow

                mbCell = New TableCell
                mbCell.HorizontalAlign = HorizontalAlign.Right
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.ColumnSpan = 1
                btnRenConfirm = New Button
                btnRenConfirm.Attributes.Add("background", bgImgBtnStr)
                btnRenConfirm.Text = handle.renameBtnTooltipStr
                btnRenConfirm.ToolTip = handle.renameBtnTooltipStr
                btnRenConfirm.EnableViewState = False
                btnRenConfirm.Height = Unit.Pixel(mbBtnHeight)
                btnRenConfirm.Enabled = True
                mbCell.Controls.Add(btnRenConfirm)
                'mbCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                mbRow.Cells.Add(mbCell)

                mbCell = New TableCell
                mbCell.HorizontalAlign = HorizontalAlign.Left
                mbCell.VerticalAlign = VerticalAlign.Middle
                mbCell.ColumnSpan = 1
                btnRenConfirmCancel = New Button
                btnRenConfirmCancel.Attributes.Add("background", bgImgBtnStr)
                btnRenConfirmCancel.Text = handle.cancelStr
                btnRenConfirmCancel.ToolTip = handle.cancelStr
                btnRenConfirmCancel.EnableViewState = False
                btnRenConfirmCancel.Height = Unit.Pixel(mbBtnHeight)
                mbCell.Controls.Add(btnRenConfirmCancel)
                mbRow.Cells.Add(mbCell)

                renTable.Rows.Add(mbRow)

                tbRow = New TableRow
                tbRow.Visible = False
                tbCell = New TableCell
                tbCell.HorizontalAlign = HorizontalAlign.Center
                tbCell.BackColor = System.Drawing.Color.Silver
                'tbCell.ColumnSpan = 9
                tbCell.Controls.Add(renTable)
                tbRow.Cells.Add(tbCell)
                TableBody.Rows.Add(tbRow) '8th row ends'6th row


                tbRow = New TableRow
                'tbRow.Visible = False
                tCellexpAdminGrid = New TableCell
                'tCellexpAdminGrid.ColumnSpan = 9
                tbRow.Cells.Add(tCellexpAdminGrid)
                TableBody.Rows.Add(tbRow) '9th row

            End Sub

            Public Sub FileExplorerUser()



                createUserTable()
                'here starts expAdminGrid   
                expAdminGrid = createUserGrid(tCellexpAdminGrid)
                'renameGrid = createRenameGrid(tCellGrid)
                'renameGrid.Visible = False

            End Sub 'FileExplorer ends

            Private Function createUserGrid(ByVal parentXS As Object) As DataGrid
                parentXS.controls.add(expAdminGrid)

                expAdminGrid.Width = Unit.Percentage(100)
                expAdminGrid.Height = Unit.Percentage(100)
                expAdminGrid.CssClass = "A.GridStyle"
                expAdminGrid.Font.Name = "Arial"
                expAdminGrid.Font.Size = FontUnit.Point(9)
                expAdminGrid.HeaderStyle.Font.Size = FontUnit.Point(9)
                expAdminGrid.HeaderStyle.Font.Bold = True
                expAdminGrid.HeaderStyle.BackColor = System.Drawing.Color.LightGray
                expAdminGrid.BackColor = System.Drawing.Color.White
                expAdminGrid.ForeColor = System.Drawing.Color.Black
                'expAdminGrid.Font.Underline = False
                expAdminGrid.AutoGenerateColumns = False
                expAdminGrid.AllowSorting = True
                expAdminGrid.AllowPaging = True
                expAdminGrid.PagerStyle.Visible = True
                expAdminGrid.PagerStyle.Mode = PagerMode.NumericPages
                expAdminGrid.PagerStyle.HorizontalAlign = HorizontalAlign.Center
                expAdminGrid.PagerStyle.Font.Bold = True
                expAdminGrid.PagerStyle.BackColor = System.Drawing.Color.LightGray
                expAdminGrid.PagerStyle.PageButtonCount = 12
                '****do not set pageSize here
                expAdminGrid.GridLines = GridLines.None
                expAdminGrid.ItemStyle.Height = Unit.Pixel(20)


                If Session("PageSizeSession") Is Nothing Then
                    If pageSizeList.SelectedIndex = 5 Then
                        expAdminGrid.AllowPaging = False
                    Else
                        expAdminGrid.PageSize = CType(pageSizeList.SelectedValue, Integer)
                    End If
                Else
                    Dim size As Integer = CType(Session("PageSizeSession"), Integer)
                    If size = 5 Then
                        expAdminGrid.AllowPaging = False
                    Else
                        pageSizeList.SelectedIndex = size
                        expAdminGrid.PageSize = CType(pageSizeList.SelectedValue, Integer)
                    End If

                End If


                Dim column0 As New TemplateColumn
                column0.HeaderStyle.Width = Unit.Pixel(20)
                column0.ItemStyle.Width = Unit.Pixel(20)
                column0.ItemTemplate = New ChkBoxItemTemplate
                column0.EditItemTemplate = New ChkBoxItemTemplate
                column0.Visible = False

                Dim column1 As New HyperLinkColumn
                column1.HeaderStyle.Width = Unit.Pixel(16)
                column1.ItemStyle.Width = Unit.Pixel(16)
                column1.DataTextField = "ImageURL"
                column1.DataNavigateUrlField = "NameNavigateURL"
                column1.Visible = True

                Dim column2 As New BoundColumn
                column2.HeaderText = handle.ColumnHeader_Name
                column2.ItemStyle.ForeColor = System.Drawing.Color.Black
                column2.DataField = "Name"
                column2.Visible = False

                Dim column3 As New HyperLinkColumn
                column3.HeaderText = handle.ColumnHeader_Name
                column3.DataNavigateUrlField = "NameNavigateURL"
                column3.ItemStyle.ForeColor = System.Drawing.Color.Black
                column3.SortExpression = "DisplayName"
                column3.DataTextField = "DisplayName"


                Dim column4 As New HyperLinkColumn
                column4.HeaderStyle.Width = Unit.Pixel(16)
                column4.ItemStyle.Width = Unit.Pixel(16)
                column4.DataTextField = "EditFileImageURL"
                column4.DataNavigateUrlField = "EditFileNavigateURL"
                column4.Visible = False

                Dim column5 As New HyperLinkColumn
                column5.HeaderStyle.Width = Unit.Pixel(16)
                column5.ItemStyle.Width = Unit.Pixel(16)
                column5.DataTextField = "ShowTextImageURL"
                'column5.DataNavigateUrlField = "ShowTextNavigateURL"
                column5.DataNavigateUrlField = "EditFileNavigateURL"

                Dim column6 As New BoundColumn
                column6.HeaderStyle.Width = Unit.Pixel(75)
                column6.ItemStyle.Width = Unit.Pixel(75)
                column6.HeaderText = handle.ColumnHeader_Type
                column6.DataField = "DataType"
                column6.SortExpression = "DataType"

                Dim column7 As New BoundColumn
                column7.HeaderStyle.Width = Unit.Pixel(140)
                column7.ItemStyle.Width = Unit.Pixel(140)
                column7.HeaderText = handle.ColumnHeader_LastModified
                column7.DataField = "LastModified"
                column7.SortExpression = "LastModified"

                Dim column8 As New BoundColumn
                column8.HeaderStyle.Width = Unit.Pixel(70)
                column8.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
                column8.ItemStyle.Width = Unit.Pixel(70)
                column8.HeaderText = handle.ColumnHeader_Size
                column8.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                column8.DataField = "Size"
                column8.SortExpression = "Size"

                Dim column9 As New BoundColumn
                column9.HeaderStyle.Width = Unit.Pixel(25)
                column9.ItemStyle.Width = Unit.Pixel(25)
                column9.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                column9.DataField = "SizeStr"

                Dim column10 As New BoundColumn
                column10.Visible = False
                column10.DataField = "SortColumn"
                column10.SortExpression = "SortColumn"

                Dim column11 As New BoundColumn
                column11.Visible = False
                column11.DataField = "SortSize"
                column11.SortExpression = "SortSize"

                'Dim column11 As New BoundColumn
                ''column11.Visible = False
                'column11.DataField = "SortSizeStr"
                'column11.SortExpression = "SortSizeStr"

                expAdminGrid.Columns.Add(column0)
                expAdminGrid.Columns.Add(column1)
                expAdminGrid.Columns.Add(column2)
                expAdminGrid.Columns.Add(column3)
                expAdminGrid.Columns.Add(column4)
                expAdminGrid.Columns.Add(column5)
                expAdminGrid.Columns.Add(column6)
                expAdminGrid.Columns.Add(column7)
                expAdminGrid.Columns.Add(column8)
                expAdminGrid.Columns.Add(column9)
                expAdminGrid.Columns.Add(column10)
                'expAdminGrid.Columns.Add(column11)

                expAdminGrid.DataSource = createUserDataSource()
                expAdminGrid.DataBind()

                Return (expAdminGrid)
            End Function 'createDirGrid ends

            Private Function createUserDataSource() As ICollection
                'DisplLang = cammwebmanager.CurLanguage
                Dim fileTable As DataTable
                Dim dataRow As dataRow

                'create a DataTable
                fileTable = New DataTable

                '0th this column to store icon image url
                fileTable.Columns.Add(New DataColumn("ImageURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("NameNavigateURL", GetType(String)))
                '2nd to store name of file
                fileTable.Columns.Add(New DataColumn("Name", GetType(String)))
                fileTable.Columns.Add(New DataColumn("DisplayName", GetType(String)))
                fileTable.Columns.Add(New DataColumn("EditFileImageURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("EditFileNavigateURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("ShowTextImageURL", GetType(String)))
                '6th to store url to show text
                fileTable.Columns.Add(New DataColumn("ShowTextNavigateURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("DataType", GetType(String)))
                fileTable.Columns.Add(New DataColumn("LastModified", GetType(System.DateTime)))
                fileTable.Columns.Add(New DataColumn("Size", GetType(String)))
                fileTable.Columns.Add(New DataColumn("SizeStr", GetType(String)))
                fileTable.Columns.Add(New DataColumn("SortColumn", GetType(String)))
                fileTable.Columns.Add(New DataColumn("SortSize", GetType(Double)))
                'fileTable.Columns.Add(New DataColumn("SortSizeStr", GetType(String)))


                'browse from root directory
                Try

                    Dim di As DirectoryInfo = New DirectoryInfo(dirGroup.FullName)
                    Dim dirs As DirectoryInfo() = di.GetDirectories()

                    Dim dirNext As DirectoryInfo
                    For Each dirNext In dirs
                        If Not IsHiddenFile(dirNext.Name) Then
                            dataRow = fileTable.NewRow()
                            dataRow(0) = FileIconHtmlCode("FolderImage")
                            dataRow(1) = getOpenFolderURL(dirNext.FullName, relativePath)
                            '"index.aspx?path=" & System.Web.HttpUtility.URLEncode(GetRelativePath2StartDir(fullnameArray(ctr)))
                            Dim dName As String
                            dataRow(2) = dirNext.Name
                            dName = handle.getXmlValue(DisplLang, "FileTitles", "File", dirNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                            If dName = "" Then
                                dataRow(3) = dirNext.Name
                            Else
                                dataRow(3) = dName
                            End If
                            dataRow(4) = ""
                            dataRow(5) = ""
                            dataRow(6) = ""
                            dataRow(7) = ""
                            dataRow(8) = handle.DataType_Folder
                            dataRow(9) = dirNext.LastWriteTime '.ToString()
                            dataRow(10) = ""
                            dataRow(11) = ""
                            dataRow(12) = "a" 'Folder sort ID
                            dataRow(13) = 0
                            'dataRow(13) = "aaaaaaaaa"
                            fileTable.Rows.Add(dataRow)
                        End If
                    Next

                    Dim files As fileInfo() = di.GetFiles()
                    Dim fileNext As fileInfo
                    If Request.QueryString("q") Is Nothing Or Request.QueryString("q") = "" Then
                        For Each fileNext In files
                            If Not IsHiddenFile(fileNext.Name, Request.PhysicalPath) And Not (LCase(fileNext.Name) = "indexdata.xml") And Not IsHiddenFile(fileNext.Name) Then
                                'Response.write("---" & fileNext.Name & "<br>" )
                                dataRow = fileTable.NewRow()
                                dataRow(0) = FileIconHtmlCode(fileNext.Extension)
                                Dim navUrl As String = Me.GetFileNavigationURL(fileNext, relativePath)
                                dataRow(1) = navUrl
                                dataRow(2) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                Dim fName As String
                                fName = handle.getXmlValue(DisplLang, "FileTitles", "File", fileNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                                If fName = "" Then
                                    fName = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                End If
                                If navUrl = "" Then
                                    fname = fName & " <i>&lt;" & Me.handle.ExceedsDownloadFileSizeLimit & "&gt;</i>"
                                End If
                                dataRow(3) = fName
                                dataRow(4) = getEditFileImageURL(fileNext.Extension)
                                dataRow(5) = getEditFileNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(6) = getShowTextImageURL(fileNext.Extension)
                                dataRow(7) = getShowTextNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(8) = Mid(fileNext.Extension, 2)
                                dataRow(9) = fileNext.LastWriteTime '.ToString()
                                dataRow(10) = getFileLength(fileNext.Length).ToString()
                                dataRow(11) = getSizeAppendString(fileNext.Length)
                                dataRow(12) = "aaaaaaaaa" ' 'File sort ID
                                dataRow(13) = getFileLength(fileNext.Length)
                                fileTable.Rows.Add(dataRow)
                            End If
                        Next
                    Else
                        Dim qStr As String = Request.QueryString("q")
                        Dim len As Integer = 0

                        For Each fileNext In files
                            If Not IsHiddenFile(fileNext.Name, Request.PhysicalPath) And Not (LCase(fileNext.Name) = "indexdata.xml") And Not IsHiddenFile(fileNext.Name) Then
                                dataRow = fileTable.NewRow()
                                dataRow(0) = FileIconHtmlCode(fileNext.Extension)
                                Dim navUrl As String = Me.GetFileNavigationURL(fileNext, relativePath)
                                dataRow(1) = navurl
                                dataRow(2) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                Dim fName As String
                                fName = handle.getXmlValue(DisplLang, "FileTitles", "File", fileNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                                If fName = "" Then
                                    fname = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                End If
                                If navurl = "" Then
                                    fname = fName & " <i>&lt;" & Me.handle.ExceedsDownloadFileSizeLimit & "&gt;</i>"
                                End If
                                dataRow(3) = fName
                                dataRow(4) = getEditFileImageURL(fileNext.Extension)
                                dataRow(5) = getEditFileNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(6) = getShowTextImageURL(fileNext.Extension)
                                dataRow(7) = getShowTextNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(8) = Mid(fileNext.Extension, 2)
                                dataRow(9) = fileNext.LastWriteTime '.ToString()
                                dataRow(10) = getFileLength(fileNext.Length).ToString()
                                dataRow(11) = getSizeAppendString(fileNext.Length)
                                dataRow(12) = "aaaaaaaaa"
                                dataRow(13) = getFileLength(fileNext.Length)

                                len = qStr.Length
                                Dim chkStr As String
                                chkStr = Mid(fileNext.Name, 1, len)
                                If UCase(chkStr) = UCase(qStr) Then
                                    fileTable.Rows.Add(dataRow)
                                Else
                                    If Request.QueryString("qt") Is Nothing Then
                                    Else
                                        Dim qtStr As String
                                        For Each qtStr In Request.QueryString.GetValues("qt")
                                            len = qtStr.Length
                                            chkStr = Mid(fileNext.Name, 1, len)
                                            If UCase(chkStr) = UCase(qtStr) Then
                                                fileTable.Rows.Add(dataRow)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        Next
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

                'return a DataView to the FileTable
                Dim dataGridView As DataView = New DataView(fileTable)

                If Session("ascSession") Is Nothing Then
                    dataGridView.Sort = sortString
                Else
                    sortString = CType(Session("ascSession"), String)
                    dataGridView.Sort = sortString
                End If
                Session("Source") = dataGridView
                createUserDataSource = dataGridView

            End Function 'createDirDataSource ends.

            Public Sub FileExplorerAdmin()


                createTable()
                'here starts expAdminGrid   
                expAdminGrid = createGrid(tCellexpAdminGrid)

                'expAdminGrid.Visible = False
                renameGrid = createRenameGrid(tCellGrid)
                renameGrid.Visible = False

            End Sub 'FileExplorer ends

            Private Function createGrid(ByVal parentXS As Object) As DataGrid
                parentXS.controls.add(expAdminGrid)

                expAdminGrid.Width = Unit.Percentage(100)
                expAdminGrid.Height = Unit.Percentage(100)
                expAdminGrid.CssClass = "A.GridStyle"
                expAdminGrid.Font.Name = "Arial"
                expAdminGrid.Font.Size = FontUnit.Point(9)
                expAdminGrid.HeaderStyle.Font.Size = FontUnit.Point(9)
                expAdminGrid.HeaderStyle.Font.Bold = True
                expAdminGrid.HeaderStyle.BackColor = System.Drawing.Color.LightGray
                expAdminGrid.BackColor = System.Drawing.Color.White
                expAdminGrid.ForeColor = System.Drawing.Color.Black
                expAdminGrid.Font.Underline = False
                expAdminGrid.AutoGenerateColumns = False
                expAdminGrid.AllowSorting = True
                expAdminGrid.AllowPaging = True
                expAdminGrid.PagerStyle.Visible = True
                expAdminGrid.PagerStyle.Mode = PagerMode.NumericPages
                expAdminGrid.PagerStyle.HorizontalAlign = HorizontalAlign.Center
                expAdminGrid.PagerStyle.Font.Bold = True
                expAdminGrid.PagerStyle.BackColor = System.Drawing.Color.LightGray
                expAdminGrid.PagerStyle.PageButtonCount = 12
                '****do not set pageSize here
                expAdminGrid.GridLines = GridLines.None
                expAdminGrid.ItemStyle.Height = Unit.Pixel(20)


                If Session("PageSizeSession") Is Nothing Then
                    If pageSizeList.SelectedIndex = 5 Then
                        expAdminGrid.AllowPaging = False
                    Else
                        expAdminGrid.PageSize = CType(pageSizeList.SelectedValue, Integer)
                    End If
                Else
                    Dim size As Integer = CType(Session("PageSizeSession"), Integer)
                    If size = 5 Then
                        expAdminGrid.AllowPaging = False
                    Else
                        pageSizeList.SelectedIndex = size
                        expAdminGrid.PageSize = CType(pageSizeList.SelectedValue, Integer)
                    End If

                End If


                Dim column0 As New TemplateColumn
                column0.HeaderStyle.Width = Unit.Pixel(20)
                column0.ItemStyle.Width = Unit.Pixel(20)
                column0.ItemTemplate = New ChkBoxItemTemplate
                column0.EditItemTemplate = New ChkBoxItemTemplate

                Dim column1 As New HyperLinkColumn
                column1.HeaderStyle.Width = Unit.Pixel(16)
                column1.ItemStyle.Width = Unit.Pixel(16)
                column1.DataTextField = "ImageURL"
                column1.DataNavigateUrlField = "NameNavigateURL"
                column1.Visible = True

                Dim column2 As New BoundColumn
                column2.HeaderText = handle.ColumnHeader_Name
                column2.ItemStyle.ForeColor = System.Drawing.Color.Black
                column2.DataField = "Name"
                column2.Visible = False

                Dim column3 As New HyperLinkColumn
                column3.HeaderText = handle.ColumnHeader_Name
                column3.DataNavigateUrlField = "NameNavigateURL"
                column3.ItemStyle.ForeColor = System.Drawing.Color.Black
                column3.SortExpression = "DisplayName"
                column3.DataTextField = "DisplayName"


                Dim column4 As New HyperLinkColumn
                column4.HeaderStyle.Width = Unit.Pixel(16)
                column4.ItemStyle.Width = Unit.Pixel(16)
                column4.DataTextField = "EditFileImageURL"
                column4.DataNavigateUrlField = "EditFileNavigateURL"

                Dim column5 As New HyperLinkColumn
                column5.Visible = False
                column5.HeaderStyle.Width = Unit.Pixel(16)
                column5.ItemStyle.Width = Unit.Pixel(16)
                column5.DataTextField = "ShowTextImageURL"
                column5.DataNavigateUrlField = "ShowTextNavigateURL"

                Dim column6 As New BoundColumn
                column6.HeaderStyle.Width = Unit.Pixel(75)
                column6.ItemStyle.Width = Unit.Pixel(75)
                column6.HeaderText = handle.ColumnHeader_Type
                column6.DataField = "DataType"
                column6.SortExpression = "DataType"

                Dim column7 As New BoundColumn
                column7.HeaderStyle.Width = Unit.Pixel(140)
                column7.ItemStyle.Width = Unit.Pixel(140)
                column7.HeaderText = handle.ColumnHeader_LastModified
                column7.DataField = "LastModified"
                column7.SortExpression = "LastModified"

                Dim column8 As New BoundColumn
                column8.HeaderStyle.Width = Unit.Pixel(70)
                column8.HeaderStyle.HorizontalAlign = HorizontalAlign.Right
                column8.ItemStyle.Width = Unit.Pixel(70)
                column8.HeaderText = handle.ColumnHeader_Size
                column8.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                column8.DataField = "Size"
                column8.SortExpression = "Size"

                Dim column9 As New BoundColumn
                column9.HeaderStyle.Width = Unit.Pixel(25)
                column9.ItemStyle.Width = Unit.Pixel(25)
                column9.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                column9.DataField = "SizeStr"

                Dim column10 As New BoundColumn
                column10.Visible = False
                column10.DataField = "SortColumn"
                column10.SortExpression = "SortColumn"

                Dim column11 As New BoundColumn
                column11.Visible = False
                column11.DataField = "SortSize"
                column11.SortExpression = "SortSize"

                'Dim column11 As New BoundColumn
                'column11.HeaderText = "SHEHEEH"
                ''column11.Visible = False
                'column11.DataField = "SortSizeStr"
                'column11.SortExpression = "SortSizeStr"

                expAdminGrid.Columns.Add(column0)
                expAdminGrid.Columns.Add(column1)
                expAdminGrid.Columns.Add(column2)
                expAdminGrid.Columns.Add(column3)
                expAdminGrid.Columns.Add(column4)
                expAdminGrid.Columns.Add(column5)
                expAdminGrid.Columns.Add(column6)
                expAdminGrid.Columns.Add(column7)
                expAdminGrid.Columns.Add(column8)
                expAdminGrid.Columns.Add(column9)
                expAdminGrid.Columns.Add(column10)
                expAdminGrid.Columns.Add(column11)

                expAdminGrid.DataSource = createDataSource()
                expAdminGrid.DataBind()

                Return (expAdminGrid)
            End Function 'createDirGrid ends

            Private Sub bindData(ByVal sortStr As String)
                Dim dv As DataView = CType(Session("Source"), DataView)
                dv.Sort = sortStr
                expAdminGrid.DataSource = dv
                expAdminGrid.DataBind()
                Session("Source") = dv
            End Sub ' bindData ends

            Private Function createDataSource() As ICollection
                'DisplLang = cammwebmanager.CurLanguage
                Dim fileTable As DataTable
                Dim dataRow As dataRow

                'create a DataTable
                fileTable = New DataTable

                '0th this column to store icon image url
                fileTable.Columns.Add(New DataColumn("ImageURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("NameNavigateURL", GetType(String)))
                '2nd to store name of file
                fileTable.Columns.Add(New DataColumn("Name", GetType(String)))
                fileTable.Columns.Add(New DataColumn("DisplayName", GetType(String)))
                fileTable.Columns.Add(New DataColumn("EditFileImageURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("EditFileNavigateURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("ShowTextImageURL", GetType(String)))
                '6th to store url to show text
                fileTable.Columns.Add(New DataColumn("ShowTextNavigateURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("DataType", GetType(String)))
                fileTable.Columns.Add(New DataColumn("LastModified", GetType(System.DateTime)))
                fileTable.Columns.Add(New DataColumn("Size", GetType(String)))
                fileTable.Columns.Add(New DataColumn("SizeStr", GetType(String)))
                fileTable.Columns.Add(New DataColumn("SortColumn", GetType(String)))
                fileTable.Columns.Add(New DataColumn("SortSize", GetType(Double)))
                'fileTable.Columns.Add(New DataColumn("SortSizeStr", GetType(String)))


                'browse from root directory
                Try

                    Dim di As DirectoryInfo = New DirectoryInfo(dirGroup.FullName)
                    Dim dirs As DirectoryInfo() = di.GetDirectories()

                    Dim dirNext As DirectoryInfo
                    For Each dirNext In dirs
                        If IsHiddenFile(dirNext.Name) Then
                            dataRow = fileTable.NewRow()
                            dataRow(0) = getHiddenImageURL("FolderImage")
                            dataRow(1) = getOpenFolderURL(dirNext.FullName, relativePath)
                            '"index.aspx?path=" & System.Web.HttpUtility.URLEncode(GetRelativePath2StartDir(fullnameArray(ctr)))
                            Dim dName As String
                            dataRow(2) = dirNext.Name
                            dName = handle.getXmlValue(DisplLang, "FileTitles", "File", dirNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                            If dName = "" Then
                                dataRow(3) = dirNext.Name & handle.hiddenStr_toAttachwith_fName
                            Else
                                dataRow(3) = dName & handle.hiddenStr_toAttachwith_fName
                            End If
                            dataRow(4) = ""
                            dataRow(5) = ""
                            dataRow(6) = ""
                            dataRow(7) = ""
                            dataRow(8) = handle.DataType_Folder
                            dataRow(9) = dirNext.LastWriteTime '.ToString()
                            dataRow(10) = ""
                            dataRow(11) = ""
                            dataRow(12) = "a" 'Folder sort ID
                            dataRow(13) = 0
                            'dataRow(13) = "aaaaaaaaa"
                            fileTable.Rows.Add(dataRow)
                        Else
                            dataRow = fileTable.NewRow()
                            dataRow(0) = FileIconHtmlCode("FolderImage")
                            dataRow(1) = getOpenFolderURL(dirNext.FullName, relativePath)
                            '"index.aspx?path=" & System.Web.HttpUtility.URLEncode(GetRelativePath2StartDir(fullnameArray(ctr)))
                            Dim dName As String
                            dataRow(2) = dirNext.Name
                            dName = handle.getXmlValue(DisplLang, "FileTitles", "File", dirNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                            If dName = "" Then
                                dataRow(3) = dirNext.Name
                            Else
                                dataRow(3) = dName
                            End If
                            dataRow(4) = ""
                            dataRow(5) = ""
                            dataRow(6) = ""
                            dataRow(7) = ""
                            dataRow(8) = handle.DataType_Folder
                            dataRow(9) = dirNext.LastWriteTime '.ToString()
                            dataRow(10) = ""
                            dataRow(11) = ""
                            dataRow(12) = "a" 'Folder sort ID
                            dataRow(13) = 0
                            'dataRow(13) = "aaaaaaaaa"
                            fileTable.Rows.Add(dataRow)

                        End If
                    Next

                    Dim files As fileInfo() = di.GetFiles()
                    Dim fileNext As fileInfo
                    If Request.QueryString("q") Is Nothing Or Request.QueryString("q") = "" Then
                        For Each fileNext In files
                            If IsHiddenFile(fileNext.Name) Then
                                'Response.write("---" & fileNext.Name & "<br>" )
                                dataRow = fileTable.NewRow()
                                dataRow(0) = getHiddenImageURL(fileNext.Extension)
                                Dim navUrl As String = Me.GetFileNavigationURL(fileNext, relativePath)
                                dataRow(1) = navUrl
                                dataRow(2) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                Dim fName As String
                                fName = handle.getXmlValue(DisplLang, "FileTitles", "File", fileNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                                If fName = "" Then
                                    fname = Path.GetFileNameWithoutExtension(fileNext.FullName) & handle.hiddenStr_toAttachwith_fName
                                Else
                                    fname = fName & handle.hiddenStr_toAttachwith_fName
                                End If
                                If navUrl = "" Then
                                    fname = fName & " <i>&lt;" & Me.handle.ExceedsDownloadFileSizeLimit & "&gt;</i>"
                                End If
                                dataRow(3) = fname
                                dataRow(4) = getEditFileImageURL(fileNext.Extension)
                                dataRow(5) = getEditFileNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(6) = getShowTextImageURL(fileNext.Extension)
                                dataRow(7) = getShowTextNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(8) = Mid(fileNext.Extension, 2)
                                dataRow(9) = fileNext.LastWriteTime '.ToString()
                                dataRow(10) = getFileLength(fileNext.Length).ToString()
                                dataRow(11) = getSizeAppendString(fileNext.Length)
                                dataRow(12) = "aaaaaaaaa" ' 'File sort ID
                                dataRow(13) = getFileLength(fileNext.Length)
                                fileTable.Rows.Add(dataRow)
                            ElseIf Not IsHiddenFile(fileNext.Name, Request.PhysicalPath) And Not (LCase(fileNext.Name) = "indexdata.xml") Then
                                'Response.write("---" & fileNext.Name & "<br>" )
                                dataRow = fileTable.NewRow()
                                dataRow(0) = FileIconHtmlCode(fileNext.Extension)
                                Dim navUrl As String = Me.GetFileNavigationURL(fileNext, relativePath)
                                dataRow(1) = navurl
                                dataRow(2) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                Dim fName As String
                                fName = handle.getXmlValue(DisplLang, "FileTitles", "File", fileNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                                If fName = "" Then
                                    fname = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                End If
                                If navUrl = "" Then
                                    fname = fName & " <i>&lt;" & Me.handle.ExceedsDownloadFileSizeLimit & "&gt;</i>"
                                End If
                                dataRow(3) = fName
                                dataRow(4) = getEditFileImageURL(fileNext.Extension)
                                dataRow(5) = getEditFileNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(6) = getShowTextImageURL(fileNext.Extension)
                                dataRow(7) = getShowTextNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(8) = Mid(fileNext.Extension, 2)
                                dataRow(9) = fileNext.LastWriteTime '.ToString()
                                dataRow(10) = getFileLength(fileNext.Length).ToString()
                                dataRow(11) = getSizeAppendString(fileNext.Length)
                                dataRow(12) = "aaaaaaaaa" ' 'File sort ID
                                dataRow(13) = getFileLength(fileNext.Length)
                                fileTable.Rows.Add(dataRow)


                            End If
                        Next
                    Else
                        Dim qStr As String = Request.QueryString("q")
                        Dim len As Integer = 0

                        For Each fileNext In files
                            If IsHiddenFile(fileNext.Name) Then
                                dataRow = fileTable.NewRow()
                                dataRow(0) = getHiddenImageURL(fileNext.Extension)
                                Dim navUrl As String = Me.GetFileNavigationURL(fileNext, relativePath)
                                dataRow(1) = navurl
                                dataRow(2) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                Dim fName As String
                                fName = handle.getXmlValue(DisplLang, "FileTitles", "File", fileNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                                If fName = "" Then
                                    fname = Path.GetFileNameWithoutExtension(fileNext.FullName) & handle.hiddenStr_toAttachwith_fName
                                Else
                                    fname = fName & handle.hiddenStr_toAttachwith_fName
                                End If
                                If navUrl = "" Then
                                    fname = fName & " <i>&lt;" & Me.handle.ExceedsDownloadFileSizeLimit & "&gt;</i>"
                                End If
                                dataRow(3) = fName
                                dataRow(4) = getEditFileImageURL(fileNext.Extension)
                                dataRow(5) = getEditFileNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(6) = getShowTextImageURL(fileNext.Extension)
                                dataRow(7) = getShowTextNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(8) = Mid(fileNext.Extension, 2)
                                dataRow(9) = fileNext.LastWriteTime '.ToString()
                                dataRow(10) = getFileLength(fileNext.Length).ToString()
                                dataRow(11) = getSizeAppendString(fileNext.Length)
                                dataRow(12) = "aaaaaaaaa"
                                dataRow(13) = getFileLength(fileNext.Length)

                                len = qStr.Length
                                Dim chkStr As String
                                chkStr = Mid(fileNext.Name, 1, len)
                                If UCase(chkStr) = UCase(qStr) Then
                                    fileTable.Rows.Add(dataRow)
                                Else
                                    If Request.QueryString("qt") Is Nothing Then
                                    Else
                                        Dim qtStr As String
                                        For Each qtStr In Request.QueryString.GetValues("qt")
                                            len = qtStr.Length
                                            chkStr = Mid(fileNext.Name, 1, len)
                                            If UCase(chkStr) = UCase(qtStr) Then
                                                fileTable.Rows.Add(dataRow)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If

                            ElseIf Not IsHiddenFile(fileNext.Name, Request.PhysicalPath) And Not (LCase(fileNext.Name) = "indexdata.xml") Then
                                dataRow = fileTable.NewRow()
                                dataRow(0) = FileIconHtmlCode(fileNext.Extension)
                                Dim navUrl As String = Me.GetFileNavigationURL(fileNext, relativePath)
                                dataRow(1) = navurl
                                dataRow(2) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                Dim fName As String
                                fName = handle.getXmlValue(DisplLang, "FileTitles", "File", fileNext.Name, dirGroup.FullName & System.IO.Path.DirectorySeparatorChar & "indexdata.xml")
                                If fName = "" Then
                                    fname = Path.GetFileNameWithoutExtension(fileNext.FullName)
                                End If
                                If navUrl = "" Then
                                    fname = fName & " <i>&lt;" & Me.handle.ExceedsDownloadFileSizeLimit & "&gt;</i>"
                                End If
                                dataRow(3) = fName
                                dataRow(4) = getEditFileImageURL(fileNext.Extension)
                                dataRow(5) = getEditFileNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(6) = getShowTextImageURL(fileNext.Extension)
                                dataRow(7) = getShowTextNavigateURL(fileNext.Name, fileNext.Extension)
                                dataRow(8) = Mid(fileNext.Extension, 2)
                                dataRow(9) = fileNext.LastWriteTime '.ToString()
                                dataRow(10) = getFileLength(fileNext.Length).ToString()
                                dataRow(11) = getSizeAppendString(fileNext.Length)
                                dataRow(12) = "aaaaaaaaa"
                                dataRow(13) = getFileLength(fileNext.Length)

                                len = qStr.Length
                                Dim chkStr As String
                                chkStr = Mid(fileNext.Name, 1, len)
                                If UCase(chkStr) = UCase(qStr) Then
                                    fileTable.Rows.Add(dataRow)
                                Else
                                    If Request.QueryString("qt") Is Nothing Then
                                    Else
                                        Dim qtStr As String
                                        For Each qtStr In Request.QueryString.GetValues("qt")
                                            len = qtStr.Length
                                            chkStr = Mid(fileNext.Name, 1, len)
                                            If UCase(chkStr) = UCase(qtStr) Then
                                                fileTable.Rows.Add(dataRow)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If


                            End If
                        Next
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

                'return a DataView to the FileTable
                Dim dataGridView As DataView = New DataView(fileTable)

                If Session("ascSession") Is Nothing Then
                    dataGridView.Sort = sortString
                Else
                    sortString = CType(Session("ascSession"), String)
                    dataGridView.Sort = sortString
                End If
                Session("Source") = dataGridView
                createDataSource = dataGridView

            End Function 'createDirDataSource ends.

            Public Function IsHiddenFile(ByVal TestFileName As String, ByVal physicalPath As String) As Boolean
                Dim returnBool As Boolean = False
                Dim hiddenFileArrayList As ArrayList = getXmlHiddenFileList(physicalPath)
                If hiddenFileArrayList Is Nothing Then
                    returnBool = False
                ElseIf hiddenFileArrayList.Contains(TestFileName) Then
                    returnBool = True
                End If
                Return returnBool
            End Function

            Private Function getXmlHiddenFileList(ByVal physicalPath As String) As ArrayList

                Dim hiddenFileArrayList As New ArrayList
                Dim xmlFile As String
                xmlFile = Mid(physicalPath, 1, InStrRev(physicalPath, System.IO.Path.DirectorySeparatorChar) - 1) & System.IO.Path.DirectorySeparatorChar & "config.xml"
                Dim hiddenFiles As String() = Nothing
                Try
                    hiddenFiles = handle.getXmlValues(0, "HideFiles", "File", , xmlFile)
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

                Dim hFile As String
                For Each hFile In hiddenFiles
                    'Response.Write("-add fiels--" & "<br>")
                    hiddenFileArrayList.Add(hFile)
                Next

                Return hiddenFileArrayList
            End Function

            Public Function IsHiddenFile(ByVal fName As String) As Boolean
                Dim returnBool As Boolean = False
                Dim testFileName As String = fName
                Dim hiddenFileArrayList As ArrayList = getXmlHiddenFileList()
                If hiddenFileArrayList Is Nothing Then
                    returnBool = False
                ElseIf hiddenFileArrayList.Contains(testFileName) Then
                    returnBool = True
                End If
                Return returnBool
            End Function

            ''' <summary>
            ''' Provide HTML code for embedding an appropriate file/folder icon based on the extension
            ''' </summary>
            ''' <param name="extension">e.g. &quot;.pdf&quot;</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Private Function FileIconHtmlCode(ByVal extension As String) As String
                Dim tooltipStr As String = handle.FileDownloadTooltip
                Dim iconame As String = getIconImageFile(extension)
                If iconame = "folderimage" Then
                    tooltipStr = handle.FolderBrowseTooltip
                End If

                Return "<img title=""" & tooltipStr & """ height=""16"" width=""16"" border=""0"" src=""/system/modules/explorer/images/icons/" & iconame & """>"
                'Return "/system/modules/explorer/images/" & iconame

            End Function 'getImageURL ends.

            ''' <summary>
            ''' Provide an icon based on the file extension
            ''' </summary>
            ''' <param name="extension">e.g. &quot;.pdf&quot;</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Private Function getIconImageFile(ByVal extension As String) As String

                Dim iconame As String

                Select Case LCase(extension)
                    Case ".bmp", ".gif", ".jpg", ".tif", ".jpeg", ".tiff", ".png"
                        iconame = "picture.gif"
                    Case ".doc", ".docm", ".docx", ".dot", ".dotx", ".dotm", ".rtf", ".odt", ".ott"
                        iconame = "dir_doc.gif"
                    Case ".exe", ".bat", ".com", ".cmd"
                        iconame = "dir_exe.gif"
                    Case ".htm", ".html"
                        iconame = "www.gif"
                    Case ".css", ".asa", ".asp", ".cfm", ".php3", ".inc", ".src"
                        iconame = "dir_asp.gif"
                    Case ".aspx", ".asmx", ".ashx", ".ascx"
                        iconame = "dir_aspx.gif"
                    Case ".cs"
                        iconame = "dir_cs.gif"
                    Case ".vb"
                        iconame = "dir_vb.gif"
                    Case ".c"
                        iconame = "dir_c.gif"
                    Case ".h"
                        iconame = "dir_h.gif"
                    Case ".emf"
                        iconame = "dir_emf.gif"
                    Case ".bas", ".vbp", ".res", ".frm", ".frx"
                        iconame = "dir_bas.gif"
                    Case ".wav", ".mp3", ".mpg", ".avi", ".asf", ".rm", ".mov", ".divx"
                        iconame = "dir_avi.gif"
                    Case ".txt", ".ini", ".log", ".pl", ".csv"
                        iconame = "dir_txt.gif"
                    Case ".inf"
                        iconame = "dir_inf.gif"
                    Case ".7z", ".zip", ".arc", ".sit", ".rar"
                        iconame = "dir_zip.gif"
                    Case ".dll", ".tlb"
                        iconame = "dir_dll.gif"
                    Case ".pdf", ".xps"
                        iconame = "dir_pdf.gif"
                    Case ".ppt", ".pps", ".ppt", ".pptm", ".pptx", ".pps", ".ppsx", ".pot", ".potx", ".potm", ".odp", ".otp", ".odg", ".vsd"
                        iconame = "dir_ppt.gif"
                    Case ".mdb", ".mde", ".adp", ".ade"
                        iconame = "dir_mdb.gif"
                    Case ".pst"
                        iconame = "dir_pst.gif"
                    Case ".xls", ".xlsx", ".xlsm", ".xlsb", ".xlt", ".xltx", ".xltm", ".ods", ".ots"
                        iconame = "dir_xls.gif"
                    Case ".xsl"
                        iconame = "dir_xsl.gif"
                    Case ".xslt"
                        iconame = "dir_xslt.gif"
                    Case ".config"
                        iconame = "dir_config.gif"
                    Case ".csproj"
                        iconame = "dir_csproj.gif"
                    Case ".wmf"
                        iconame = "dir_wmf.gif"
                    Case ".xml"
                        iconame = "dir_xml.gif"
                    Case ".js"
                        iconame = "dir_js.gif"
                    Case ".sln"
                        iconame = "dir_sln.gif"
                    Case ".php"
                        iconame = "dir_php.gif"
                    Case ".vbproj"
                        iconame = "dir_vbproj.gif"
                    Case ".cdr", ".ps", ".eps", ".psp", ".psd"
                        iconame = "dir_ps.gif"
                    Case "view"
                        iconame = "view.gif"
                    Case "edit"
                        iconame = "propertyicon.gif"
                    Case "folderimage"
                        iconame = "folder.gif"
                    Case "uparrowimage"
                        iconame = "arrow057.gif"
                    Case Else
                        iconame = "dir_unknown.gif"
                End Select
                Return iconame

            End Function

            ''' <summary>
            ''' Provide HTML code for embedding an appropriate file/folder icon (hidden style) based on the extension
            ''' </summary>
            ''' <param name="extension">e.g. &quot;.pdf&quot;</param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Private Function getHiddenImageURL(ByVal extension As String) As String
                Dim tooltipStr As String = handle.FileDownloadTooltip
                Dim iconame As String = getIconImageFile(extension)
                If iconame = "folderimage" Then
                    tooltipStr = handle.FolderBrowseTooltip
                End If

                Return "<img title=""" & tooltipStr & """ height=""16"" width=""16"" border=""0"" src=""/system/modules/explorer/images/icons_hidden/" & iconame & """>"
                'Return "/system/modules/explorer/images/icons_hidden/" & iconame

            End Function 'getImageURL ends.

            Private Function getShowTextImageURL(ByVal txt As String) As String
                Dim iconame As String
                Dim tooltipStr As String = ""
                Select Case LCase(txt)
                    Case ".aspx", ".ascx", ".asmx", ".ashx", ".asp", ".html", ".htm", ".cs", ".vb", ".css", ".txt", ".php", ".php3", ".php4", ".php5", ".php6", ".php7", ".php8", ".php9", ".cfm", ".js", ".ascx", ".log", ".xml", ".csv", ".xslt", ".xsl"
                        iconame = "view.gif"
                        tooltipStr = handle.FileIconTooltip_View
                    Case Else
                        iconame = "space.gif"
                End Select
                Return "<img Width=""16"" title=""" & tooltipStr & """ border=""0"" Height=""16"" src=""/system/modules/explorer/images/" & iconame & """>"
            End Function 'getShowTextImageURL ends

            Private Function getNameNavigateURL_(ByVal txt As String, ByVal relativePath As String) As String
                Return "downloadfile.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)) & "&file=" & System.Web.HttpUtility.UrlEncode(txt) & "&ie=" & handle.GetWin95IE5CompatibleFileName(txt)
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Returns file navigation URL.
            ''' </summary>
            ''' <param name="fileInfo"></param>
            ''' <param name="relativePath"></param>
            ''' <returns></returns>
            ''' <remarks>
            '''     On Exception returns empty string.
            '''     
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	05.07.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function GetFileNavigationURL(ByVal fileInfo As fileInfo, ByVal relativePath As String) As String
                Dim result As String = ""
                Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                Select Case Me.handle.getXmlValue(0, "General", "DownloadLocation", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                    Case "WebServerSession"
                        downloadLocation = DownloadHandler.DownloadLocations.WebManagerUserSession
                    Case "WebManagerUserSession"
                        downloadLocation = DownloadHandler.DownloadLocations.WebManagerUserSession
                    Case "WebManagerSecurityObjectName"
                        downloadLocation = DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                    Case Else
                        downloadLocation = DownloadHandler.DownloadLocations.PublicCache
                End Select
                Dim limit As Long
                Dim ts As New TimeSpan(0, 5, 0)
                Try
                    Dim temp As String = Me.handle.getXmlValue(0, "General", "FileDownloadSizeLimit", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                    If temp <> "" Then
                        limit = System.Convert.ToInt64(temp)
                    End If
                    temp = Me.handle.getXmlValue(0, "General", "CachingTimeLimitInMinits", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                    If temp <> "" Then
                        ts = Nothing
                        ts = New TimeSpan(0, System.Convert.ToInt32(temp), 0)
                    End If
                Catch ex As Exception
                    ts = New TimeSpan(0, 5, 0)
                End Try

                If limit <> Nothing Then
                    cammWebManager.DownloadHandler.MaxDownloadSize = limit
                    cammWebManager.DownloadHandler.MaxDownloadCollectionSize = limit
                End If
                Try
                    Dim folderInVirtualDownloadLocation As String = DownloadHandler.ComputeHashedPathFromLongPath(Me.handle.PathStart & "\" & relativePath)
                    If Me.cammWebManager.DownloadHandler.IsFullyFeatured Then
                        If Me.cammWebManager.DownloadHandler.DownloadFileAlreadyExists(downloadLocation, "cwx", folderInVirtualDownloadLocation, fileInfo.Name) Then
                            result = Me.cammWebManager.DownloadHandler.CreateDownloadLink(downloadLocation, "cwx", folderInVirtualDownloadLocation, fileInfo.Name)
                        Else
                            'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                            Me.cammWebManager.DownloadHandler.Clear()
                            '
                            Me.cammWebManager.DownloadHandler.Add(fileInfo, folderInVirtualDownloadLocation)

                            'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                            result = Me.cammWebManager.DownloadHandler.ProcessAndGetPlainDownloadLink(downloadLocation, "cwx", False, ts)
                        End If
                    Else
                        'Throw New Exception("Downloads in separate HTTP requests are not supported on this webserver")
                        result = "downloadfile.aspx?lang=" & Me.DisplLang & "&path=" & HttpContext.Current.Server.UrlEncode(handle.GetRelativePath2StartDir(fileInfo.Directory.FullName, relativePath)) & "&file=" & HttpContext.Current.Server.UrlEncode(fileInfo.Name) & "&ie=" & handle.GetWin95IE5CompatibleFileName(fileInfo.Name)
                    End If
                Catch ex As Exception
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                    result = ""
                End Try

                Return result
            End Function


            Private Function getEditFileImageURL(ByVal txt As String) As String
                Dim iconame As String
                Dim tooltipStr As String = ""
                Select Case LCase(txt)
                    Case ".aspx", ".asp", ".html", ".htm", ".asmx", ".cs", ".vb", ".css", ".inc", ".txt", ".php", ".php3", ".cfm", ".js", ".ascx", ".log", ".xml", ".xsl"
                        iconame = "edit_file.gif"
                        tooltipStr = handle.FileIconTooltip_Edit
                    Case Else
                        iconame = "space.gif"
                End Select
                Return "<img Width=""16"" title=""" & tooltipStr & """ border=""0"" Height=""16"" src=""/system/modules/explorer/images/" & iconame & """>"
                '        Return "<img border=""0"" src=""/system/modules/explorer/images/" & iconame & """>"
            End Function 'getEditFileImageURL ends

            Private Function getEditFileNavigateURL(ByVal txt As String, ByVal ext As String) As String
                Dim url As String
                Select Case LCase(ext)
                    Case ".aspx", ".asp", ".html", ".htm", ".asmx", ".cs", ".vb", ".css", ".inc", ".txt", ".php", ".php3", ".cfm", ".js", ".ascx", ".log", ".xml", ".xsl"
                        url = "editor.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)) & "&file=" & System.Web.HttpUtility.UrlEncode(txt)
                    Case Else
                        url = ""
                End Select
                Return url
            End Function

            Private Function getUpArrowURL(ByVal txt As String, ByVal relativePath As String) As String
                Return "index.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(txt, relativePath))
            End Function

            Private Function getOpenFolderURL(ByVal txt As String, ByVal rPath As String) As String
                Return "index.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(txt, rPath))
            End Function

            Private Function getShowTextNavigateURL(ByVal txt As String, ByVal ext As String) As String
                Dim url As String
                Select Case LCase(ext)
                    Case ".aspx", ".asp", ".html", ".htm", ".asmx", ".cs", ".vb", ".css", ".inc", ".txt", ".php", ".php3", ".cfm", "..js", ".ascx", ".log", ".xml", ".xsl"
                        url = "viewer.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)) & "&file=" & System.Web.HttpUtility.UrlEncode(txt)
                    Case Else
                        url = ""
                End Select
                Return url
            End Function

            Private Function getFileLength(ByVal len As Long) As Integer
                If (len < 1024) Then
                    Return 1
                Else
                    Return CType(System.Math.Round(len / 1024, 2), Integer)
                End If

                'Dim FileSize As Double

                'Dim fLength As Double = len

                'If fLength < 10 ^ 3 Then
                '    FileSize = 1

                'ElseIf fLength < 10 ^ 6 Then
                '    FileSize = System.Math.Round(fLength / 1024, 2)
                '    'Return FileSize.ToString & " KB"
                'ElseIf fLength < 10 ^ 9 Then
                '    FileSize = System.Math.Round(fLength / 1024 ^ 2, 2)
                '    'Return FileSize.ToString & " MB"
                'Else
                '    FileSize = System.Math.Round(fLength / 1024 ^ 2, 2)
                '    'Return FileSize.ToString & " GB"
                'End If
                'Return FileSize

            End Function

            Private Function getSizeAppendString(ByVal len As Double) As String
                If (len < 1000) Then
                    Return " KB"
                Else
                    Return " KB"
                End If

                'Dim returnStr As String

                'Dim fLength As Double = len

                'If fLength < 10 ^ 3 Then
                '    returnStr = " KB"
                'ElseIf fLength < 10 ^ 6 Then
                '    returnStr = " KB"
                'ElseIf fLength < 10 ^ 9 Then
                '    returnStr = " MB"
                'Else
                '    returnStr = " GB"
                'End If
                'Return returnStr

            End Function

            Private Function getFileSortString(ByVal sizeStr As String) As String

                Dim returnStr As String = Nothing

                Dim sStr As String = sizeStr

                If sStr = " KB" Then
                    returnStr = "ba"
                ElseIf sStr = " MB" Then
                    returnStr = "cb"
                ElseIf sStr = " GB" Then
                    returnStr = "dc"
                Else

                End If
                Return returnStr

            End Function

            Private Sub expAdminGrid_SortCommand(ByVal source As Object, ByVal e As DataGridSortCommandEventArgs) Handles expAdminGrid.SortCommand
                sortString = CType(Session("ascSession"), String)
                Select Case e.SortExpression
                    Case "DisplayName"
                        If sortString = "SortColumn,DisplayName ASC" Then
                            sortString = "SortColumn,DisplayName DESC"
                        Else
                            sortString = "SortColumn,DisplayName ASC"
                        End If
                    Case "DataType"
                        If sortString = "SortColumn,DataType ASC" Then
                            sortString = "SortColumn,DataType DESC"
                        Else
                            sortString = "SortColumn,DataType ASC"
                        End If
                    Case "LastModified"
                        If sortString = "SortColumn,LastModified ASC" Then
                            sortString = "SortColumn,LastModified DESC"
                        Else
                            sortString = "SortColumn,LastModified ASC"
                        End If
                    Case "Size"
                        If sortString = "SortColumn,SortSize ASC" Then
                            sortString = "SortColumn,SortSize DESC"
                        Else
                            sortString = "SortColumn,SortSize ASC"
                        End If
                End Select
                Session("ascSession") = sortString
                bindData(sortString)
            End Sub 'dataGrid_SortCommand ends

            Private Sub expAdminGrid_PageIndexChanged(ByVal source As System.Object, ByVal e As DataGridPageChangedEventArgs) Handles expAdminGrid.PageIndexChanged
                expAdminGrid.CurrentPageIndex = e.NewPageIndex
                If expAdminGrid.CurrentPageIndex < 0 Or expAdminGrid.CurrentPageIndex >= expAdminGrid.PageCount Then
                    expAdminGrid.CurrentPageIndex = 0
                End If
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)

                End If

                bindData(sortString)
            End Sub 'dataGrid_PageIndexChanged ends

            Private Sub expAdminGrid_ItemDataBound(ByVal source As Object, ByVal e As DataGridItemEventArgs) Handles expAdminGrid.ItemDataBound
                'Response.Write(",  " & e.Item.GetType.ToString)
                If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                    e.Item.Attributes.Add("onmouseover", "this.style.backgroundColor='lightBlue'")
                End If
                If e.Item.ItemType = ListItemType.Header Then
                    e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='lightgrey'")
                ElseIf e.Item.ItemType = ListItemType.Item Then
                    e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='White'")
                Else
                    e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='White'")
                End If
            End Sub

            Private Sub expAdminGrid_ItemCommand(ByVal source As Object, ByVal e As DataGridCommandEventArgs) Handles expAdminGrid.ItemCommand
                'Response.Write("     " & e.Item.GetType.ToString())
                'Response.write(",  " & CType(e.Item.Cells(0).Controls(0), CheckBox).Text  & "." & e.Item.Cells(6).Text )


                ''e.Item.Cells(7).GetType().ToString()
                ''Response.write("  " & e.Item.Cells(3).FindControl("Click").GetType.ToString())
                'If e.Item.ItemIndex >= 0 Then
                '    Dim item As DataGridItem
                '    item = CType(e.Item, DataGridItem)
                '    Response.Write(", " & item.GetType.ToString())
                '    Dim btn As Button
                '    btn = CType(item.Cells(3).FindControl("Click"), Button)
                '    'Response.write(", im here " & btn.Text)

                '    Response.Write(", " & item.ItemIndex.ToString)
                '    Response.Write(", " & item.DataSetIndex.ToString())
                '    'dt.Rows(5).Item("Name").GetType.ToString()

                '    Dim dv As DataView = CType(Session("Source"), DataView)
                '    Dim dt As DataTable = dv.Table
                '    Dim name As String = CType(dt.Rows(item.DataSetIndex).Item("Name"), String) & "." & CType(dt.Rows(item.DataSetIndex).Item("DataType"), String)

                '    Response.Write(", " & name)


                'End If
                'Response.write("   inside ItemCommand  " )'& e.Item.GetType.ToString() )
            End Sub

            Private Sub expAdminGrid_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles expAdminGrid.SelectedIndexChanged
                'Response.Write("  i am here!!!! dataGrid_SelectedIndexChanged !!!!!!!")
            End Sub

            Private Sub btnCut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCut.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString

                Dim gridItem As DataGridItem
                Dim count As Integer = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        count += 1
                    End If
                Next
                Session("CheckedCountSession") = count
                If count = 0 Then
                    Exit Sub
                End If
                Dim fileNameArr() As String
                Dim dirNameArr() As String

                ReDim fileNameArr(count)
                ReDim dirNameArr(count)
                fCount = 0
                dCount = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        Dim name As String
                        If CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String) = handle.DataType_Folder Then
                            name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            gridItem.Enabled = False
                            dirNameArr(dCount) = name
                            dCount += 1
                        Else
                            Dim ext As String = CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            If ext = "" Then
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            Else
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & ext
                            End If
                            gridItem.Enabled = False
                            name = relativePath & System.IO.Path.DirectorySeparatorChar & name
                            fileNameArr(fCount) = name
                            fCount += 1
                        End If
                        count += 1
                        'chkBox.Checked = False
                    End If
                Next

                Dim i As Integer
                For i = 0 To count
                    'Response.write(" " & fileName(i))
                Next
                Session("btnSession") = "BtnCutClicked"
                Session("dirNameArray") = dirNameArr
                Session("dCountArray") = dCount
                Session("fileNameArray") = fileNameArr
                Session("fCountArray") = fCount


                Session("PageSizeSession") = pageSizeList.SelectedIndex
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                'bindData(sortString)
                'Response.write("selected files " & count.ToString())
            End Sub 'end of btnCut_Click

            Private Sub btnCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCopy.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString
                Dim gridItem As DataGridItem
                Dim count As Integer = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        count += 1
                    End If
                Next
                Session("CheckedCountSession") = count
                If count = 0 Then
                    Exit Sub
                End If
                Dim fileNameArr() As String
                Dim dirNameArr() As String

                ReDim fileNameArr(count)
                ReDim dirNameArr(count)
                count = 0
                fCount = 0
                dCount = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        Dim name As String
                        If CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String) = handle.DataType_Folder Then
                            name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            dirNameArr(dCount) = name
                            dCount += 1
                        Else
                            Dim ext As String = CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            If ext = "" Then
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            Else
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & ext
                            End If
                            name = relativePath & System.IO.Path.DirectorySeparatorChar & name
                            fileNameArr(fCount) = name
                            fCount += 1
                        End If
                        count += 1
                        'chkBox.Checked = False
                    End If
                Next

                Session("btnSession") = "BtnCopyClicked"
                Session("dirNameArray") = dirNameArr
                Session("dCountArray") = dCount
                Session("fileNameArray") = fileNameArr
                Session("fCountArray") = fCount

                Session("PageSizeSession") = pageSizeList.SelectedIndex
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)

                End If
                'bindData(sortString)
                'Response.write("selected files " & count.ToString())
            End Sub ' end of btnCopy_Click

            Private Sub btnPaste_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPaste.Click
                If Session("btnSession") Is Nothing And Session("fCountArray") Is Nothing And Session("dCountArray") Is Nothing Then
                    Exit Sub
                End If
                Dim gridItem As DataGridItem
                Dim count As Integer
                count = CType(Session("CheckedCountSession"), Integer)
                If count = 0 Then
                    Exit Sub
                End If

                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        chkBox.Checked = False
                    End If
                Next
                Dim btnStr As String = CType(Session("btnSession"), String)
                fCount = CType(Session("fCountArray"), Integer)
                dCount = CType(Session("dCountArray"), Integer)
                Dim fn(fCount) As String
                Array.Copy(CType(Session("fileNameArray"), Array), fn, fCount)
                Dim dn(dCount) As String
                Array.Copy(CType(Session("dirNameArray"), Array), dn, dCount)

                Dim conflictFNArr() As String
                Dim conflictDNArr() As String
                ReDim conflictDNArr(dCount)
                ReDim conflictFNArr(fCount)
                Dim conflictDCount As Integer
                Dim conflictFCount As Integer
                conflictDCount = 0
                conflictFCount = 0

                Dim str As String
                Dim i As Integer
                Select Case btnStr
                    Case "BtnCopyClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            str = copyDirTo(dn(i), relativePath)
                            If (str = "") = False Then
                                conflictDNArr(conflictDCount) = str
                                conflictDCount += 1
                            End If
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            str = copyFileTo(fn(i), relativePath)
                            If (str = "") = False Then
                                conflictFNArr(conflictFCount) = str
                                conflictFCount += 1
                            End If
                        Next
                    Case "BtnCutClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            str = moveDirTo(dn(i), relativePath)
                            If (str = "") = False Then
                                conflictDNArr(conflictDCount) = str
                                conflictDCount += 1
                            End If
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            str = moveFileTo(fn(i), relativePath)
                            If (str = "") = False Then
                                conflictFNArr(conflictFCount) = str
                                conflictFCount += 1
                            End If
                        Next

                End Select

                If (conflictDCount = 0) And (conflictFCount = 0) Then
                    Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
                Else
                    expAdminGrid.Visible = False
                    TableBody.Rows(0).Enabled = False
                    TableBody.Rows(1).Enabled = False
                    pageSizeList.Enabled = False
                    TableBody.Rows(4).Visible = True

                    Session("PageSizeSession") = pageSizeList.SelectedIndex
                    Session("ConflictDNArray") = conflictDNArr
                    Session("ConflictDCount") = conflictDCount
                    Session("ConflictFNArray") = conflictFNArr
                    Session("ConflictFCount") = conflictFCount
                    Session("btnSession") = btnStr

                End If
                Session("PageSizeSession") = pageSizeList.SelectedIndex
            End Sub ' end of btnPaste_Click

            Private Sub msgCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles msgCancel.Click

                Session.Contents.Remove("ConflictDNArray")
                Session.Contents.Remove("ConflictDCount")
                Session.Contents.Remove("ConflictFNArray")
                Session.Contents.Remove("ConflictFCount")
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of msgCancel_Click

            Private Sub overWriteAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles overWriteAll.Click
                'If Session("btnSession") Is Nothing And Session("fCountArray") Is Nothing And Session("dCountArray") Is Nothing Then
                '    Exit Sub
                'End If
                Dim btnStr As String = CType(Session("btnSession"), String)
                fCount = 0
                dCount = 0
                fCount = CType(Session("ConflictFCount"), Integer)
                dCount = CType(Session("ConflictDCount"), Integer)
                Dim fn(fCount) As String
                Array.Copy(CType(Session("ConflictFNArray"), Array), fn, fCount)
                Dim dn(dCount) As String
                Array.Copy(CType(Session("ConflictDNArray"), Array), dn, dCount)

                'Response.write("---msgYes---" & fCount.ToString())

                Dim i As Integer
                Select Case btnStr
                    Case "BtnCopyClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            overWriteDirTo(dn(i), relativePath)
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            overWriteFileTo(fn(i), relativePath)
                        Next
                    Case "BtnCutClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            moveOverWriteDirTo(dn(i), relativePath)
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            moveOverWriteFileTo(fn(i), relativePath)
                        Next
                End Select
                'Session.Contents.Remove("fCountArray")
                'Session.Contents.Remove("fileNameArray")
                'Session.Contents.Remove("dCountArray")
                'Session.Contents.Remove("dirNameArray")
                Session("PageSizeSession") = pageSizeList.SelectedIndex

                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))

            End Sub 'end of overWriteAll_Click

            Private Sub overWriteDirTo(ByVal sourceDirPath As String, ByVal destDirPath As String)
                Dim sDirPath As String
                Dim dDirPath As String
                sDirPath = handle.StrDir(sourceDirPath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sDir As DirectoryInfo = New DirectoryInfo(sDirPath)
                Dim newDDirPath As String
                newDDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name
                Try
                    If Directory.Exists(newDDirPath) Then
                        Directory.Delete(newDDirPath, True)
                    End If
                    copyDirectory(sDirPath, dDirPath)
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of overWriteDirTo

            Private Sub overWriteFileTo(ByVal sourceFilePath As String, ByVal destDirPath As String)
                Dim sFilePath As String
                Dim dDirPath As String
                sFilePath = handle.StrDir(sourceFilePath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sf As fileInfo = New fileInfo(sFilePath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sf.Name
                Try
                    sf.CopyTo(dDirPath, True)
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of overWriteFileTo

            Private Sub moveOverWriteDirTo(ByVal sourceDirPath As String, ByVal destDirPath As String)
                Dim sDirPath As String
                Dim dDirPath As String
                sDirPath = handle.StrDir(sourceDirPath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sDir As DirectoryInfo = New DirectoryInfo(sDirPath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name
                'Response.write("----" & sDir.Name)
                Try
                    ' Determine whethers the directory exists.
                    If Directory.Exists(dDirPath) Then
                        Directory.Delete(dDirPath, True)
                    End If
                    sDir.MoveTo(dDirPath)

                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of moveOverWriteDirTo

            Private Sub moveOverWriteFileTo(ByVal sourceFilePath As String, ByVal destDirPath As String)
                Dim sFilePath As String
                Dim dDirPath As String
                sFilePath = handle.StrDir(sourceFilePath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sf As fileInfo = New fileInfo(sFilePath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sf.Name
                Dim df As fileInfo = New fileInfo(dDirPath)
                'Response.write("----" & sf.Name)
                Try
                    If df.Exists Then
                        df.Delete()
                    End If
                    sf.MoveTo(dDirPath)

                    'sf.CopyTo(dDirPath, True)

                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of moveOverWriteFileTo

            Private Sub iterate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles iterate.Click
                'If Session("btnSession") Is Nothing And Session("fCountArray") Is Nothing And Session("dCountArray") Is Nothing Then
                '    Exit Sub
                'End If
                Dim btnStr As String = CType(Session("btnSession"), String)
                fCount = 0
                dCount = 0
                fCount = CType(Session("ConflictFCount"), Integer)
                dCount = CType(Session("ConflictDCount"), Integer)
                Dim fn(fCount) As String
                Array.Copy(CType(Session("ConflictFNArray"), Array), fn, fCount)
                Dim dn(dCount) As String
                Array.Copy(CType(Session("ConflictDNArray"), Array), dn, dCount)

                Dim i As Integer
                Select Case btnStr
                    Case "BtnCopyClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            iterateNCopyDirTo(dn(i), relativePath, "_(" & (i + 1).ToString & ")")
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            iterateNCopyFileTo(fn(i), relativePath, "_(" & (i + 1).ToString & ")")
                        Next
                    Case "BtnCutClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            iterateNMoveDirTo(dn(i), relativePath, "_(" & (i + 1).ToString & ")")
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            iterateNMoveFileTo(fn(i), relativePath, "_(" & (i + 1).ToString & ")")
                        Next
                End Select
                'Session.Contents.Remove("fCountArray")
                'Session.Contents.Remove("fileNameArray")
                'Session.Contents.Remove("dCountArray")
                'Session.Contents.Remove("dirNameArray")

                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))

            End Sub 'end of iterate_Click

            Private Sub iterateDT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles iterateDT.Click
                'If Session("btnSession") Is Nothing And Session("fCountArray") Is Nothing And Session("dCountArray") Is Nothing Then
                '    Exit Sub
                'End If
                Dim btnStr As String = CType(Session("btnSession"), String)
                fCount = 0
                dCount = 0
                fCount = CType(Session("ConflictFCount"), Integer)
                dCount = CType(Session("ConflictDCount"), Integer)
                Dim fn(fCount) As String
                Array.Copy(CType(Session("ConflictFNArray"), Array), fn, fCount)
                Dim dn(dCount) As String
                Array.Copy(CType(Session("ConflictDNArray"), Array), dn, dCount)

                'Response.write("---msgYes---" & fCount.ToString())




                Dim str As String
                str = getDateTimeNow()
                Dim i As Integer
                Select Case btnStr
                    Case "BtnCopyClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            iterateNCopyDirTo(dn(i), relativePath, str)
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            iterateNCopyFileTo(fn(i), relativePath, str)
                        Next
                    Case "BtnCutClicked"
                        i = 0
                        For i = 0 To (dCount - 1)
                            iterateNMoveDirTo(dn(i), relativePath, str)
                        Next
                        i = 0
                        For i = 0 To (fCount - 1)
                            'Response.write(" " &  fn(i))
                            iterateNMoveFileTo(fn(i), relativePath, str)
                        Next
                End Select
                'Session.Contents.Remove("fCountArray")
                'Session.Contents.Remove("fileNameArray")
                'Session.Contents.Remove("dCountArray")
                'Session.Contents.Remove("dirNameArray")

                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))

            End Sub 'end of iterateDT_Click

            Private Function getDateTimeNow() As String
                Dim str As String
                Dim dStr As String
                dStr = "_"
                str = System.DateTime.Today.ToShortDateString.ToString
                dStr = dStr & Mid(str, 7, 4) 'year
                dStr = dStr & Mid(str, 4, 2) 'month
                dStr = dStr & Mid(str, 1, 2) 'day
                str = System.DateTime.Now.ToLongTimeString
                dStr = dStr & Mid(str, 1, 2) 'hour
                dStr = dStr & Mid(str, 4, 2) 'min
                dStr = dStr & Mid(str, 7, 2) 'second
                Return dStr
            End Function

            Private Function getNumericIterateString(ByVal sName As String, ByVal i As Integer) As String
                ' "_(" & (i + 1).ToString & ")")
                Dim returnStr As String
                Dim len As Integer = sName.Length
                Dim chkStr As String = Mid(sName, len, 1)
                Dim int As Integer
                If chkStr = ")" Then
                    int = Integer.Parse(Mid(sName, len - 1, 1))
                    returnStr = "_(" & (int + 1).ToString & ")"
                Else
                    returnStr = "_(" & (i + 1).ToString & ")"
                End If

                Return returnStr

            End Function

            Private Sub iterateNCopyDirTo(ByVal sourceDirPath As String, ByVal destDirPath As String, ByVal iterateStr As String)
                Dim sDirPath As String
                Dim dDirPath As String
                sDirPath = handle.StrDir(sourceDirPath)
                dDirPath = handle.StrDir(destDirPath)
                Try
                    renameNCopyDirectory(sDirPath, dDirPath, iterateStr)
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of iterateNCopyDirTo

            Private Sub renameNCopyDirectory(ByVal sourceDirPath As String, ByVal destDirPath As String, ByVal iterateStr As String)
                Dim sDir As DirectoryInfo = New DirectoryInfo(sourceDirPath)
                Dim dDirPath As String
                dDirPath = destDirPath
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name & iterateStr
                Try
                    Directory.CreateDirectory(dDirPath)
                    Dim dDir As DirectoryInfo = New DirectoryInfo(dDirPath)
                    Dim files As fileInfo() = sDir.GetFiles()
                    Dim fileNext As fileInfo
                    dDirPath = dDir.FullName
                    Dim newFilePath As String
                    For Each fileNext In files
                        newFilePath = dDirPath & System.IO.Path.DirectorySeparatorChar & fileNext.Name
                        fileNext.CopyTo(newFilePath)
                    Next fileNext
                    Dim dirs As DirectoryInfo() = sDir.GetDirectories()
                    Dim dirNext As DirectoryInfo
                    For Each dirNext In dirs
                        copyDirectory(dirNext.FullName, dDir.FullName)
                    Next dirNext
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub 'end of copyDirectory

            Private Sub iterateNCopyFileTo(ByVal sourceFilePath As String, ByVal destDirPath As String, ByVal iterateStr As String)
                Dim sFilePath As String
                Dim dDirPath As String
                sFilePath = handle.StrDir(sourceFilePath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sf As fileInfo = New fileInfo(sFilePath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sf.Name.Substring(0, sf.Name.IndexOf(".")) & iterateStr & sf.Extension  'sf.Name
                Try
                    sf.CopyTo(dDirPath)
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of iterateNCopyFileTo

            Private Sub iterateNMoveDirTo(ByVal sourceDirPath As String, ByVal destDirPath As String, ByVal iterateStr As String)
                Dim sDirPath As String
                Dim dDirPath As String
                sDirPath = handle.StrDir(sourceDirPath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sDir As DirectoryInfo = New DirectoryInfo(sDirPath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name & iterateStr
                'Response.write("----" & sDir.Name)
                Try
                    ' Determine whethers the directory exists.
                    sDir.MoveTo(dDirPath)

                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of iterateNMoveDirTo

            Private Sub iterateNMoveFileTo(ByVal sourceFilePath As String, ByVal destDirPath As String, ByVal iterateStr As String)
                Dim sFilePath As String
                Dim dDirPath As String
                sFilePath = handle.StrDir(sourceFilePath)
                dDirPath = handle.StrDir(destDirPath)
                Dim sf As fileInfo = New fileInfo(sFilePath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sf.Name.Substring(0, sf.Name.IndexOf(".")) & iterateStr & sf.Extension  'sf.Name
                Dim df As fileInfo = New fileInfo(dDirPath)
                'Response.write("----" & sf.Name)
                Try
                    sf.MoveTo(dDirPath)
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub ' end of iterateNMoveFileTo

            Private Function copyDirTo(ByVal sourceDirPath As String, ByVal destDirPath As String) As String

                'Response.Write("----" & sourceDirPath & "<br>")
                Dim returnStr As String = ""
                Dim sDirPath As String
                Dim dDirPath As String
                sDirPath = handle.StrDir(sourceDirPath)
                dDirPath = handle.StrDir(destDirPath)

                Dim sDir As DirectoryInfo = New DirectoryInfo(sDirPath)
                Dim newDDirPath As String
                newDDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name
                Try
                    If Directory.Exists(newDDirPath) Then
                        'create appropriate msg for user... to be implemented.
                        returnStr = sourceDirPath
                    Else
                        copyDirectory(sDirPath, dDirPath)
                        returnStr = ""
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
                Return returnStr
            End Function ' end of copyDirTo

            Private Sub copyDirectory(ByVal sourceDirPath As String, ByVal destDirPath As String)
                Dim sDir As DirectoryInfo = New DirectoryInfo(sourceDirPath)
                Dim dDirPath As String
                dDirPath = destDirPath
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name
                Try
                    Dim dirs As DirectoryInfo() = sDir.GetDirectories() 'collect  first directory list in source dir
                    Dim files As fileInfo() = sDir.GetFiles()

                    Directory.CreateDirectory(dDirPath)
                    Dim dDir As DirectoryInfo = New DirectoryInfo(dDirPath)
                    Dim fileNext As fileInfo
                    dDirPath = dDir.FullName
                    Dim newFilePath As String
                    For Each fileNext In files
                        newFilePath = dDirPath & System.IO.Path.DirectorySeparatorChar & fileNext.Name
                        fileNext.CopyTo(newFilePath)
                    Next
                    Dim dirNext As DirectoryInfo
                    For Each dirNext In dirs
                        copyDirectory(dirNext.FullName, dDirPath)
                    Next
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
            End Sub 'end of copyDirectory

            Private Function copyFileTo(ByVal sourceFilePath As String, ByVal destDirPath As String) As String
                Dim sFilePath As String
                Dim dDirPath As String
                sFilePath = handle.StrDir(sourceFilePath)
                dDirPath = handle.StrDir(destDirPath)
                Dim returnStr As String
                returnStr = ""
                Dim sf As fileInfo = New fileInfo(sFilePath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sf.Name
                Dim df As fileInfo = New fileInfo(dDirPath)
                Try
                    If df.Exists Then
                        returnStr = sourceFilePath
                    Else
                        sf.CopyTo(dDirPath)
                        returnStr = ""
                        Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                        Dim configXMLPath As String = physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml"
                        Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                        Select Case handle.getXmlValue(0, "General", "DownloadLocation", , configXMLPath)
                            Case "WebServerSession"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebServerSession
                            Case "WebManagerUserSession"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerUserSession
                            Case "WebManagerSecurityObjectName"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                            Case Else
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.PublicCache
                        End Select

                        Me.cammWebManager.DownloadHandler.RemoveDownloadFileFromCache(downloadLocation, "cwx", DownloadHandler.ComputeHashedPathFromLongPath(df.Directory.FullName), df.Name)
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
                Return returnStr
            End Function ' end of copyFileTo

            Private Function moveDirTo(ByVal sourceDirPath As String, ByVal destDirPath As String) As String
                Dim returnStr As String
                returnStr = ""
                Dim sDirPath As String
                Dim dDirPath As String
                If sourceDirPath = destDirPath Then
                    'display error can not cut directory and paste in same directory.
                    Return Nothing
                End If
                sDirPath = handle.StrDir(sourceDirPath)
                dDirPath = handle.StrDir(destDirPath)

                Dim sDir As DirectoryInfo = New DirectoryInfo(sDirPath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sDir.Name
                'Response.write("----" & sDir.Name)
                Try
                    ' Determine whethers the directory exists.
                    If Directory.Exists(dDirPath) Then
                        'create appropriate msg for user... to be implemented.
                        returnStr = sourceDirPath
                    Else
                        sDir.MoveTo(dDirPath)
                        returnStr = ""
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
                Return returnStr
            End Function ' end of moveDirTo

            Private Function moveFileTo(ByVal sourceFilePath As String, ByVal destDirPath As String) As String
                Dim sFilePath As String
                Dim dDirPath As String
                sFilePath = handle.StrDir(sourceFilePath)
                dDirPath = handle.StrDir(destDirPath)
                Dim returnStr As String
                returnStr = ""
                Dim sf As fileInfo = New fileInfo(sFilePath)
                dDirPath = dDirPath & System.IO.Path.DirectorySeparatorChar & sf.Name
                Dim df As fileInfo = New fileInfo(dDirPath)
                'Response.write("----" & sf.Name)
                Try
                    If df.Exists Then
                        returnStr = sourceFilePath
                    Else
                        sf.MoveTo(dDirPath)
                        returnStr = ""
                        Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                        Dim configXMLPath As String = physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml"
                        Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                        Select Case handle.getXmlValue(0, "General", "DownloadLocation", , configXMLPath)
                            Case "WebServerSession"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebServerSession
                            Case "WebManagerUserSession"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerUserSession
                            Case "WebManagerSecurityObjectName"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                            Case Else
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.PublicCache
                        End Select

                        Me.cammWebManager.DownloadHandler.RemoveDownloadFileFromCache(downloadLocation, "cwx", DownloadHandler.ComputeHashedPathFromLongPath(df.Directory.FullName), df.Name)
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

                Return returnStr
            End Function ' end of moveFileTo

            Private Function getDirSize(ByVal dir As DirectoryInfo) As Long
                Dim size As Long = 0
                Dim files As fileInfo() = dir.GetFiles()
                Dim file As fileInfo
                For Each file In files
                    size += file.Length
                Next file

                Dim dirs As DirectoryInfo() = dir.GetDirectories()
                Dim dInfo As DirectoryInfo
                For Each dInfo In dirs
                    size += getDirSize(dInfo)
                Next dInfo
                Return size
            End Function 'end of getDirSize

            Private Sub btnDel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDel.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString
                Dim count As Integer = 0
                Dim gridItem As DataGridItem
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        count += 1
                        'btnDel.Enabled = True
                    End If
                Next
                If count = 0 Then
                    Exit Sub
                End If
                Dim fileNameArr() As String
                Dim dirNameArr() As String

                ReDim fileNameArr(count)
                ReDim dirNameArr(count)
                count = 0
                fCount = 0
                dCount = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)


                    If chkBox.Checked = True Then
                        Dim name As String
                        If CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String) = handle.DataType_Folder Then
                            name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            dirNameArr(dCount) = name
                            dCount += 1
                        Else
                            Dim ext As String = CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            If ext = "" Then
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            Else
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & ext
                            End If
                            name = relativePath & System.IO.Path.DirectorySeparatorChar & name
                            'Response.write("----" & name)
                            fileNameArr(fCount) = name
                            fCount += 1
                        End If
                        count += 1
                        chkBox.Checked = False
                    End If
                Next



                'Display confirmation msgBox

                expAdminGrid.Visible = False
                TableBody.Rows(0).Enabled = False
                TableBody.Rows(1).Enabled = False
                pageSizeList.Enabled = False
                TableBody.Rows(3).Visible = True

                'Session("btnSession") = "BtnCopyClicked"
                Session("dirNameArray") = dirNameArr
                Session("dCountArray") = dCount
                Session("fileNameArray") = fileNameArr
                Session("fCountArray") = fCount

                Session("PageSizeSession") = pageSizeList.SelectedIndex
                'Response.write("selected files " & count.ToString())
            End Sub 'end of btnDel_Click

            'delete file
            Private Sub msgYes_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles msgYes.Click
                fCount = 0
                dCount = 0
                fCount = CType(Session("fCountArray"), Integer)
                dCount = CType(Session("dCountArray"), Integer)
                Dim fn(fCount) As String
                Array.Copy(CType(Session("fileNameArray"), Array), fn, fCount)
                Dim dn(dCount) As String
                Array.Copy(CType(Session("dirNameArray"), Array), dn, dCount)

                'Response.Write("---msgYes---" & fCount.ToString())
                'Response.Write("---relativePath---" & relativePath & "<br>")
                Try
                    Dim sourcePath As String
                    sourcePath = ""

                    Dim i As Integer
                    Dim dir As DirectoryInfo
                    i = 0
                    For i = 0 To (dCount - 1)
                        sourcePath = handle.StrDir(dn(i))
                        dir = New DirectoryInfo(sourcePath)
                        dir.Delete(True)
                    Next
                    i = 0
                    Dim file As fileInfo
                    For i = 0 To (fCount - 1)
                        sourcePath = handle.StrDir(fn(i))
                        file = New fileInfo(sourcePath)
                        file.Delete()
                    Next
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

                'Session.Contents.Remove("fCountArray")
                'Session.Contents.Remove("fileNameArray")
                'Session.Contents.Remove("dCountArray")
                'Session.Contents.Remove("dirNameArray")
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of msgYes_Click
            'delete file
            Private Sub msgNo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles msgNo.Click

                Session.Contents.Remove("fCountArray")
                Session.Contents.Remove("fileNameArray")
                Session.Contents.Remove("dCountArray")
                Session.Contents.Remove("dirNameArray")
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))

            End Sub 'end of msgNo_Click

            Private Sub pageSizeList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pageSizeList.SelectedIndexChanged
                expAdminGrid.CurrentPageIndex = 0
                If pageSizeList.SelectedIndex = 5 Then
                    expAdminGrid.AllowPaging = False
                Else
                    expAdminGrid.AllowPaging = True
                    expAdminGrid.PageSize = CType(pageSizeList.SelectedValue, Integer)

                End If

                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)

                End If
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                bindData(sortString)

            End Sub

            Private Sub btnUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpload.Click
                Dim uploadRelativePath As String = relativePath
                Session("UploadPath") = uploadRelativePath
                expAdminGrid.Visible = False
                TableBody.Rows(0).Enabled = False
                TableBody.Rows(1).Enabled = False
                pageSizeList.Enabled = False
                TableBody.Rows(5).Visible = True
            End Sub 'end of btnUpload_Click

            Private Sub msgUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles msgUpload.Click
                Dim uploadRelativePath As String
                uploadRelativePath = CType(Session("UploadPath"), String)
                Dim fPath As String
                fPath = uploadFile.PostedFile.FileName.ToString()
                If Len(fPath) = 0 Then
                    'do nothing
                Else
                    Dim pos As Integer = fPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1
                    Dim fName As String = fPath.Substring(pos)
                    Dim uploadFileFullPath As String = handle.StrDir(uploadRelativePath) & System.IO.Path.DirectorySeparatorChar & fName
                    uploadFile.PostedFile.SaveAs(uploadFileFullPath)

                    Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                    Dim configXMLPath As String = physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml"
                    Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                    Select Case handle.getXmlValue(0, "General", "DownloadLocation", , configXMLPath)
                        Case "WebServerSession"
                            downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebServerSession
                        Case "WebManagerUserSession"
                            downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerUserSession
                        Case "WebManagerSecurityObjectName"
                            downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                        Case Else
                            downloadLocation = WebManager.DownloadHandler.DownloadLocations.PublicCache
                    End Select

                    Me.cammWebManager.DownloadHandler.RemoveDownloadFileFromCache(downloadLocation, "cwx", DownloadHandler.ComputeHashedPathFromLongPath(handle.StrDir(uploadRelativePath)), fName)
                End If
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of msgUpload_Click

            Private Sub msgUploadCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles msgUploadCancel.Click
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of msgUploadCancel_Click

            Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString

                Dim gridItem As DataGridItem
                Dim count As Integer = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = False Then
                        chkBox.Checked = True
                    Else
                        chkBox.Checked = False
                    End If
                Next

                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                'bindData(sortString)
                'Response.write("selected files " & count.ToString())
            End Sub 'end of btnSelectAll_Click

            Private Sub newFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles newFile.Click
                Dim uploadRelativePath As String = relativePath
                Session("UploadPath") = uploadRelativePath
                msgBoxlbl10.Text = handle.NewFileMsgBoxHeaderString
                expAdminGrid.Visible = False
                TableBody.Rows(0).Enabled = False
                TableBody.Rows(1).Enabled = False
                pageSizeList.Enabled = False
                TableBody.Rows(6).Visible = True
                Session("MsgBoxNewFileFolder") = "NewFile"
            End Sub 'end of newFile_Click

            Private Sub newFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles newFolder.Click
                Dim uploadRelativePath As String = relativePath
                Session("UploadPath") = uploadRelativePath
                msgBoxlbl10.Text = handle.NewFolderMsgBoxHeaderString
                expAdminGrid.Visible = False
                TableBody.Rows(0).Enabled = False
                TableBody.Rows(1).Enabled = False
                pageSizeList.Enabled = False
                TableBody.Rows(6).Visible = True
                Session("MsgBoxNewFileFolder") = "NewFolder"
            End Sub 'end of newFolder_Click

            Private Sub mBoxNFOk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mBoxNFOk.Click
                Dim newPath As String = CType(Session("UploadPath"), String)
                Dim FileOrFolder As String = CType(Session("MsgBoxNewFileFolder"), String)
                Dim name As String = txtBox.Text
                If LCase(name) = "indexdata.xml" Then
                    'display error can not crate file with this name
                    Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
                End If
                'Response.Write("---" & name)
                Dim path As String = handle.StrDir(newPath) & System.IO.Path.DirectorySeparatorChar & name
                Select Case FileOrFolder
                    Case "NewFile"
                        If File.Exists(path) = False Then
                            Dim fs As FileStream = Nothing
                            Try
                                fs = File.Create(path)
                            Catch
                            Finally
                                If Not fs Is Nothing Then
                                    fs.Flush()
                                    fs.Close()
                                End If
                            End Try
                        End If
                    Case "NewFolder"
                        If Directory.Exists(path) = False Then
                            Try
                                Directory.CreateDirectory(path)
                            Catch
                            End Try
                        End If
                End Select
                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of mBoxNFOk_Click

            Private Sub mBoxNFCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mBoxNFCancel.Click
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of mBoxNFCancel_Click

            Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click

                Select Case UCase(Request.Form("RBList1"))
                    Case UCase("upload")
                        Dim s1 As String
                        If tboxName.Value = "" Then
                            s1 = file1.PostedFile.FileName
                        Else
                            s1 = tboxName.Value
                        End If
                        If handle.IsOwnSystemFile(s1, Request.PhysicalPath, True) Then
                            'PlaceBody.Controls.Add(New LiteralControl("<strong class=""ErrorMessage"">" & handle.Modify_Forbidden & ": """ & s1 & """</strong>"))
                        Else
                            If Len(file1.PostedFile.FileName.ToString()) = 0 Then
                                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)))
                            Else
                                Dim pos As Integer = s1.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1
                                Dim s2 As String = s1.Substring(pos)
                                Dim SavedFile As fileInfo
                                File.SetAttributes(dirGroup.ToString & System.IO.Path.DirectorySeparatorChar & s2, FileAttributes.Normal)
                                file1.PostedFile.SaveAs(dirGroup.ToString & System.IO.Path.DirectorySeparatorChar & s2)
                                SavedFile = New fileInfo(s2)
                                'PlaceBody.Controls.Add(New LiteralControl("<strong class=""ErrorMessage"">File uploaded as " & s2 & " !</strong>"))
                                If SavedFile.Extension = ".txt" Or SavedFile.Extension = ".xml" Or SavedFile.Extension = ".log" Then
                                    'PlaceBody.Controls.Add(New LiteralControl("<p class=""ErrorMessage"" style=""font-size=10pt"">Attention: to properly display international characters like Asian letters or German umlauts please encode all text files in UTF-8 charset. You might use Microsoft Notepad, say ""Save as..."" and overwrite it after changing the charset from ANSI to UTF-8.</p>"))
                                End If
                            End If
                        End If
                        tboxName.Value = ""
                    Case UCase("folder")
                        If handle.IsOwnSystemFile(tboxName.Value, Request.PhysicalPath, True) Then
                            'PlaceBody.Controls.Add(New LiteralControl("<strong>" & handle.Modify_Forbidden & ": """ & tboxName.Value & """</strong>"))
                        Else
                            Directory.CreateDirectory(dirGroup.ToString & System.IO.Path.DirectorySeparatorChar & tboxName.Value)
                        End If
                        tboxName.Value = ""
                    Case UCase("file")
                        If handle.IsOwnSystemFile(tboxName.Value, Request.PhysicalPath, True) Then
                            'PlaceBody.Controls.Add(New LiteralControl("<strong class=""ErrorMessage"">" & handle.Modify_Forbidden & ": """ & tboxName.Value & """</strong>"))
                        Else
                            File.Create(dirGroup.ToString & System.IO.Path.DirectorySeparatorChar & tboxName.Value)
                        End If
                        tboxName.Value = ""
                End Select

                'Update file list
                'FileEditor()

            End Sub

            Private Function getDirCount() As Integer
                dirGroup = New DirectoryInfo(handle.StrDir(Request.Params("path")))
                If Not dirGroup.Exists Then
                    dirGroup = New DirectoryInfo(handle.StrDir(Request.Params("path"), True))
                End If
                Dim count As Integer = 0
                Try
                    Dim di As DirectoryInfo = New DirectoryInfo(dirGroup.FullName)
                    Dim dirs As DirectoryInfo() = di.GetDirectories()

                    Dim dirNext As DirectoryInfo
                    For Each dirNext In dirs
                        count += 1
                    Next
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
                Return count
            End Function

            Private Function getFileCount() As Integer
                dirGroup = New DirectoryInfo(handle.StrDir(Request.Params("path")))
                If Not dirGroup.Exists Then
                    dirGroup = New DirectoryInfo(handle.StrDir(Request.Params("path"), True))
                End If
                Dim count As Integer = 0
                Try
                    Dim di As DirectoryInfo = New DirectoryInfo(dirGroup.FullName)
                    Dim dirs As DirectoryInfo() = di.GetDirectories()
                    Dim files As fileInfo() = di.GetFiles()
                    Dim fileNext As fileInfo
                    For Each fileNext In files
                        count += 1
                    Next
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
                Return count
            End Function

            Private Sub createBlankIndexDataXML()
                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                'Response.Write("here reached....!" & xmlFile & "<br>")

                Dim writer As XmlTextWriter = New XmlTextWriter(xmlFile, Nothing)

                writer.Formatting = Formatting.Indented
                writer.WriteStartDocument()

                writer.WriteStartElement("Config")
                writer.WriteStartElement("LanguageID0")
                writer.WriteStartElement("FileNumbers")
                writer.WriteStartElement("File")
                writer.WriteAttributeString("name", "")
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("HideFiles")
                writer.WriteStartElement("File")
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID1")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                Dim title As String
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                writer.WriteStartElement("File")
                writer.WriteAttributeString("name", "")
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID2")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                writer.WriteStartElement("File")
                writer.WriteAttributeString("name", "")
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndDocument()

                writer.Flush()
                writer.Close()

            End Sub

            Private Function getXmlFileNoHashTable() As Hashtable
                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                Dim ht As Hashtable = handle.getXmlValueHT(0, "FileNumbers", "File", xmlFile)
                Return ht
            End Function

            Private Function getXmlEnglishFileNameHashTable() As Hashtable
                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                Dim ht As Hashtable = handle.getXmlValueHT(1, "FileTitles", "File", xmlFile)
                Return ht
            End Function

            Private Function getXmlGermanFileNameHashTable() As Hashtable
                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                Dim ht As Hashtable = handle.getXmlValueHT(2, "FileTitles", "File", xmlFile)
                Return ht
            End Function

            Private Sub btnRename_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRename.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString

                'Create xml data
                'check for indexdata.xml file, does not exist then create it.
                Dim indexdataXmlFile As String
                indexdataXmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                Dim xmlFI As fileInfo = New fileInfo(indexdataXmlFile)
                If Not xmlFI.Exists Then
                    'Response.Write("Error!!!! indexdata.xml doesnot exist. <br>")
                    Dim fs As FileStream = File.Create(indexdataXmlFile)
                    fs.Close()
                    'create indexdata.xml
                    createBlankIndexDataXML()
                    Exit Sub
                End If

                'read indexdata.xml file and hold data into arrays
                Dim xmlFileNoHt As Hashtable = getXmlFileNoHashTable()
                Dim xmlGeFNHt As Hashtable = getXmlGermanFileNameHashTable()
                Dim xmlEnFNHt As Hashtable = getXmlEnglishFileNameHashTable()


                Dim gridItem As DataGridItem
                For Each gridItem In renameGrid.Items
                    gridItem.Visible = False
                Next
                Dim count As Integer = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        count += 1
                        'btnDel.Enabled = True
                    End If
                Next
                If count = 0 Then
                    Exit Sub
                End If
                Dim fileNameArr() As String
                Dim dirNameArr() As String

                ReDim fileNameArr(count)
                ReDim dirNameArr(count)
                count = 0
                fCount = 0
                dCount = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        Dim name As String
                        If CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String) = handle.DataType_Folder Then
                            'name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            dirNameArr(dCount) = name
                            dCount += 1
                        Else
                            'name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            Dim ext As String = CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            If ext = "" Then
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            Else
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & ext
                            End If

                            fileNameArr(fCount) = name
                            fCount += 1
                        End If
                        count += 1
                        chkBox.Checked = False
                    End If
                Next

                Dim i As Integer = 0
                For i = 0 To (dCount - 1)
                    For Each gridItem In renameGrid.Items
                        If dirNameArr(i) = gridItem.Cells(1).Text Then
                            gridItem.Visible = True
                            CType(CType(gridItem.Cells(2).Controls(0), Table).Rows(0).Cells(1).Controls(0), TextBox).Text = dirNameArr(i)
                            'Rows(1).Cells(1).Controls(0), TextBox)
                        End If
                    Next
                Next
                i = 0
                For i = 0 To (fCount - 1)
                    For Each gridItem In renameGrid.Items
                        If fileNameArr(i) = gridItem.Cells(1).Text Then
                            gridItem.Visible = True
                            CType(CType(gridItem.Cells(2).Controls(0), Table).Rows(0).Cells(1).Controls(0), TextBox).Text = fileNameArr(i)
                        End If
                    Next
                Next

                Dim fEnum As IDictionaryEnumerator = xmlGeFNHt.GetEnumerator()
                While fEnum.MoveNext()
                    For Each gridItem In renameGrid.Items
                        If fEnum.Key = gridItem.Cells(1).Text Then
                            'gridItem.Visible = True
                            CType(CType(gridItem.Cells(2).Controls(0), Table).Rows(1).Cells(1).Controls(0), TextBox).Text = CType(fEnum.Value, String)
                        End If
                    Next
                End While


                fEnum = xmlEnFNHt.GetEnumerator()
                While fEnum.MoveNext()
                    For Each gridItem In renameGrid.Items
                        If fEnum.Key = gridItem.Cells(1).Text Then
                            'gridItem.Visible = True
                            CType(CType(gridItem.Cells(2).Controls(0), Table).Rows(2).Cells(1).Controls(0), TextBox).Text = CType(fEnum.Value, String)
                        End If
                    Next
                End While


                'Display confirmation msgBox

                expAdminGrid.Visible = False
                'TableBody.Rows(0).Enabled = False

                newFile.Enabled = False
                newFolder.Enabled = False
                btnCut.Enabled = False
                btnCopy.Enabled = False
                btnPaste.Enabled = False
                btnDel.Enabled = False
                btnRename.Enabled = False
                btnUpload.Enabled = False
                btnSelectAll.Enabled = False

                pageSizeList.Enabled = False

                'TableBody.Rows(1).Enabled = False
                TableBody.Rows(8).Visible = True
                renameGrid.Visible = True


                Session("PageSizeSession") = pageSizeList.SelectedIndex
                Session("dirNameArray") = dirNameArr
                Session("dCountArray") = dCount
                Session("fileNameArray") = fileNameArr
                Session("fCountArray") = fCount


                'Response.write("selected files " & count.ToString())
            End Sub 'end of btnRename_Click

            Private Sub btnRenConfirm_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRenConfirm.Click

                fCount = 0
                dCount = 0
                fCount = CType(Session("fCountArray"), Integer)
                dCount = CType(Session("dCountArray"), Integer)
                Dim fn(fCount) As String
                Array.Copy(CType(Session("fileNameArray"), Array), fn, fCount)
                Dim dn(dCount) As String
                Array.Copy(CType(Session("dirNameArray"), Array), dn, dCount)
                'Response.Write("--" & relativePath & "<br>")
                'renameGrid.Visible = True

                'Define hashtables to hold data to write into indexdata.html
                Dim xmlFileNoHt As Hashtable = getXmlFileNoHashTable()
                Dim xmlGeFNHt As Hashtable = getXmlGermanFileNameHashTable()
                Dim xmlEnFNHt As Hashtable = getXmlEnglishFileNameHashTable()


                Dim geFName As String
                Dim enFName As String


                Dim gridItem As DataGridItem
                Dim i As Integer = 0
                For i = 0 To (dCount - 1)
                    For Each gridItem In renameGrid.Items
                        If dn(i) = gridItem.Cells(1).Text Then
                            'Response.Write("--" & dn(i) & "=" & gridItem.Cells(1).Text & "---")
                            Dim itemTable As Table = CType(gridItem.Cells(2).Controls(0), Table)
                            Dim dPath As String = relativePath & System.IO.Path.DirectorySeparatorChar & dn(i)
                            Dim nName As String = CType(itemTable.Rows(0).Cells(1).Controls(0), TextBox).Text
                            'Response.Write("--" & dPath & "=" & nName & "<br>")
                            renameDirectory(dPath, nName)

                            geFName = CType(itemTable.Rows(1).Cells(1).Controls(0), TextBox).Text
                            If xmlGeFNHt.ContainsKey(nName) Then
                                xmlGeFNHt.Item(nName) = geFName
                            Else
                                xmlGeFNHt.Add(nName, geFName)
                            End If
                            'setNameInGerman(dPath,nName)
                            enFName = CType(itemTable.Rows(2).Cells(1).Controls(0), TextBox).Text
                            If xmlEnFNHt.ContainsKey(nName) Then
                                xmlEnFNHt.Item(nName) = enFName
                            Else
                                xmlEnFNHt.Add(nName, enFName)
                            End If
                            'setNameInEnglish(dPath,nName)

                        End If
                    Next
                Next
                i = 0
                For i = 0 To (fCount - 1)
                    For Each gridItem In renameGrid.Items
                        If fn(i) = gridItem.Cells(1).Text Then
                            'Response.Write("--" & fn(i) & "=" & gridItem.Cells(1).Text & "---")
                            Dim itemTable As Table = CType(gridItem.Cells(2).Controls(0), Table)
                            Dim fPath As String = relativePath & System.IO.Path.DirectorySeparatorChar & fn(i)
                            Dim nName As String = CType(itemTable.Rows(0).Cells(1).Controls(0), TextBox).Text
                            'Response.Write("--" & fPath & "=" & nName & "<br>")
                            renameFile(fPath, nName)

                            geFName = CType(itemTable.Rows(1).Cells(1).Controls(0), TextBox).Text
                            If xmlGeFNHt.ContainsKey(nName) Then
                                xmlGeFNHt.Item(nName) = geFName
                            Else
                                xmlGeFNHt.Add(nName, geFName)
                            End If


                            'Response.Write("--" & nName & "<br>")
                            'setNameInGerman(fPath,nName)
                            enFName = CType(itemTable.Rows(2).Cells(1).Controls(0), TextBox).Text
                            If xmlEnFNHt.ContainsKey(nName) Then
                                xmlEnFNHt.Item(nName) = enFName
                            Else
                                xmlEnFNHt.Add(nName, enFName)
                            End If

                            'Response.Write("--" & nName & "<br>")
                            'setNameInEnglish(fPath,nName)
                        End If
                    Next
                Next


                writeToIndexdataXml(xmlGeFNHt, xmlEnFNHt)



                'Session("PageSizeSession") = pageSizeList.SelectedIndex
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of btnRenConfirm_Click

            Private Sub writeToIndexdataXml(ByVal xmlGe As Hashtable, ByVal xmlEn As Hashtable)

                Dim xmlGeFNHt As Hashtable = xmlGe
                Dim xmlEnFNHt As Hashtable = xmlEn

                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"

                Dim writer As XmlTextWriter = New XmlTextWriter(xmlFile, Nothing)

                writer.Formatting = Formatting.Indented
                writer.WriteStartDocument()

                writer.WriteStartElement("Config")
                writer.WriteStartElement("LanguageID0")
                writer.WriteStartElement("FileNumbers")
                writer.WriteStartElement("File")
                writer.WriteAttributeString("name", "")
                writer.WriteString("")

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID1")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                Dim title As String
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                Dim efEnum As IDictionaryEnumerator = xmlEnFNHt.GetEnumerator()
                While efEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("name", CType(efEnum.Key, String))
                    writer.WriteString(CType(efEnum.Value, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID2")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                Dim gfEnum As IDictionaryEnumerator = xmlGeFNHt.GetEnumerator()
                While gfEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("name", CType(gfEnum.Key, String))
                    writer.WriteString(CType(gfEnum.Value, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndDocument()

                writer.Flush()
                writer.Close()



            End Sub

            Private Sub btnRenConfirmCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRenConfirmCancel.Click
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of btnRenConfirmCancel_Click

            Private Sub renameDirectory(ByVal dirPath As String, ByVal newName As String)
                Dim sourcePath As String
                sourcePath = handle.StrDir(dirPath)
                Dim dPath As String
                If newName = "" Then
                    Exit Sub
                ElseIf newName = Path.GetFileName(sourcePath) Then
                    Exit Sub
                End If

                Try
                    Dim dir As DirectoryInfo
                    dir = New DirectoryInfo(sourcePath)
                    dPath = dir.Parent.FullName & System.IO.Path.DirectorySeparatorChar & newName
                    If Directory.Exists(dPath) = False Then
                        dir.MoveTo(dPath)
                    Else
                        'need to implement
                    End If

                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

            End Sub

            Private Sub renameFile(ByVal filePath As String, ByVal nName As String)
                Dim sourcePath As String
                sourcePath = handle.StrDir(filePath)
                Dim f As fileInfo = New fileInfo(sourcePath)
                Dim newName As String = nName
                If newName = "" Then
                    Exit Sub
                ElseIf newName = f.Name Then
                    Exit Sub
                ElseIf LCase(newName) = "indexdata.xml" Then
                    Exit Sub
                ElseIf Path.GetExtension(newName) = "" Then
                    newName = Path.GetFileNameWithoutExtension(newName) & Path.GetExtension(filePath)

                End If
                Dim fPath As String
                Try
                    fPath = f.DirectoryName & System.IO.Path.DirectorySeparatorChar & newName
                    If File.Exists(fPath) = False Then
                        f.MoveTo(fPath)

                        Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                        Dim configXMLPath As String = physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml"
                        Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                        Select Case handle.getXmlValue(0, "General", "DownloadLocation", , configXMLPath)
                            Case "WebServerSession"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebServerSession
                            Case "WebManagerUserSession"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerUserSession
                            Case "WebManagerSecurityObjectName"
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                            Case Else
                                downloadLocation = WebManager.DownloadHandler.DownloadLocations.PublicCache
                        End Select
                        Dim fi As New fileInfo(fPath)
                        Me.cammWebManager.DownloadHandler.RemoveDownloadFileFromCache(downloadLocation, "cwx", DownloadHandler.ComputeHashedPathFromLongPath(fi.Directory.FullName), newName)
                    Else
                        'need to implement
                    End If

                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try

            End Sub

            Private Function createRenameGrid(ByVal parentXS As Object) As System.Web.UI.WebControls.DataGrid
                parentXS.controls.add(renameGrid)

                'renameGrid.CssClass = "GridStyle"
                renameGrid.BackColor = System.Drawing.Color.White
                renameGrid.Width = Unit.Percentage(100)

                renameGrid.Font.Name = "Arial"
                renameGrid.Font.Size = FontUnit.Point(9)
                renameGrid.Font.Underline = False
                renameGrid.AutoGenerateColumns = False
                renameGrid.GridLines = GridLines.Horizontal
                renameGrid.BorderStyle = BorderStyle.None
                renameGrid.EnableViewState = False
                renameGrid.HeaderStyle.BackColor = System.Drawing.Color.LightGray
                renameGrid.HeaderStyle.Font.Bold = True
                renameGrid.AllowPaging = False
                'renameGrid.Visible = False


                Dim column0 As New HyperLinkColumn
                column0.HeaderStyle.Width = Unit.Pixel(30)
                column0.ItemStyle.Width = Unit.Pixel(30)
                column0.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                column0.ItemStyle.VerticalAlign = VerticalAlign.Top
                column0.DataTextField = "ImageURL"


                Dim column1 As New BoundColumn
                column1.HeaderStyle.Width = Unit.Pixel(300)
                column1.ItemStyle.Width = Unit.Pixel(300)
                column1.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                column1.ItemStyle.VerticalAlign = VerticalAlign.Top
                'column1.ItemStyle.CssClass = "GridStyle"
                column1.HeaderText = handle.ColumnHeader_Name
                column1.DataField = "Name"

                Dim column2 As New TemplateColumn
                'column2.HeaderStyle.Width = Unit.Percentage(50)
                'column2.ItemStyle.Width = Unit.Percentage(50)
                'column2.HeaderText = "Enter New Names Here"
                column2.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
                column2.HeaderStyle.VerticalAlign = VerticalAlign.Middle
                column2.ItemTemplate = New TextBoxTemplate(handle.newName_TextBoxTemplateStr, handle.german_textBoxTemplateStr, handle.english_textBoxTemplateStr)


                renameGrid.Columns.Add(column0)
                renameGrid.Columns.Add(column1)
                renameGrid.Columns.Add(column2)

                renameGrid.DataSource = createRenameGridDataSource()
                renameGrid.DataBind()


                Return (renameGrid)
            End Function 'createRenameGrid ends

            Private Sub bindRenameGrid()
                renameGrid.DataSource = CType(Session("RenameGridSource"), DataView)
                renameGrid.DataBind()
            End Sub ' bindRenameData ends

            Private Function createRenameGridDataSource() As ICollection

                Dim fileTable As DataTable
                Dim dataRow As dataRow
                fileTable = New DataTable

                fileTable.Columns.Add(New DataColumn("ImageURL", GetType(String)))
                fileTable.Columns.Add(New DataColumn("Name", GetType(String)))
                'browse from root directory
                'Dim i As Integer
                'For i = 0 To (dCount - 1)
                '    dataRow = fileTable.NewRow
                '    dataRow(0) = getImageURL("FolderImage")
                '    dataRow(1) = Path.GetFileNameWithoutExtension(renameDNArr(i))
                '    fileTable.Rows.Add(dataRow)
                'Next
                'For i = 0 To (fCount - 1)
                '    Response.Write("---" & Path.GetExtension(renameFNArr(i)) & "<br>")
                '    dataRow = fileTable.NewRow
                '    dataRow(0) = getImageURL(Path.GetExtension(renameFNArr(i)))
                '    dataRow(1) = Path.GetFileNameWithoutExtension(renameFNArr(i))
                '    fileTable.Rows.Add(dataRow)
                'Next

                Try

                    Dim di As DirectoryInfo = New DirectoryInfo(dirGroup.FullName)
                    Dim dirs As DirectoryInfo() = di.GetDirectories()

                    Dim dirNext As DirectoryInfo
                    For Each dirNext In dirs
                        dataRow = fileTable.NewRow()
                        dataRow(0) = FileIconHtmlCode("FolderImage")
                        dataRow(1) = dirNext.Name
                        fileTable.Rows.Add(dataRow)
                    Next
                    Dim files As fileInfo() = di.GetFiles()
                    Dim fileNext As fileInfo
                    For Each fileNext In files
                        dataRow = fileTable.NewRow()
                        dataRow(0) = FileIconHtmlCode(fileNext.Extension)
                        'dataRow(1) = Path.GetFileNameWithoutExtension(fileNext.FullName)
                        dataRow(1) = fileNext.Name
                        fileTable.Rows.Add(dataRow)
                    Next


                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try


                Dim renameGridView As DataView = New DataView(fileTable)
                Session("RenameGridSource") = renameGridView
                createRenameGridDataSource = renameGridView
            End Function 'createRenameDataSource ends.

            Private Sub pathBoxTitle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pathBoxTitle.Click
                Dim pathBoxStr As String
                If pathBoxTitle.Text = "Path" Then
                    pathBoxTitle.Text = handle.pathBoxLblStr & "&nbsp;"
                    pathBoxStr = getDisplayLangePath(relativePath)
                Else
                    pathBoxTitle.Text = "Path"
                    pathBoxStr = handle.GetRelativePath2StartDir(dirGroup.ToString, relativePath)
                End If

                pathBoxStr = pathBoxStr.Replace(System.IO.Path.DirectorySeparatorChar, "/")
                pathBox.Text = "/" & pathBoxStr



                'Session("PageSizeSession") = pageSizeList.SelectedIndex
                'Session("dirNameArray") = dirNameArr
                'Session("dCountArray") = dCount
                'Session("fileNameArray") = fileNameArr
                'Session("fCountArray") = fCount


                'Response.write("selected files " & count.ToString())
            End Sub 'pathBoxTitle_Click

            Private Function convertdisplayLangPathToRelativePath(ByVal dPath As String) As String
                Dim returnPath As String = ""
                Dim displayPath As String = dPath
                If displayPath = "" Then
                    Return ""
                End If

                displayPath = displayPath.Replace("/", System.IO.Path.DirectorySeparatorChar)
                Dim len As Integer = 0
                len = displayPath.Length
                If len = 0 Then
                    Return ""
                End If
                Dim ch As Char
                If len = 1 Then
                    Return ""
                    ch = displayPath.Chars(0)
                Else
                    ch = displayPath.Chars(len - 1)
                End If

                If ch = System.IO.Path.DirectorySeparatorChar Then
                    displayPath = displayPath.Substring(1, len - 2)
                End If
                len = displayPath.Length
                ch = displayPath.Chars(0)
                If ch = System.IO.Path.DirectorySeparatorChar Then
                    displayPath = displayPath.Substring(1, len - 1)
                End If
                Dim rootPath As String = handle.StrDir("", True)
                Dim xmlFilePath As String
                xmlFilePath = rootPath & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                Response.Write(xmlFilePath & "<br>")
                Dim fName As String
                Dim index As Integer
                If displayPath.IndexOf(System.IO.Path.DirectorySeparatorChar) = -1 Then
                    Response.Write(" im in if..." & "<br>")
                    fName = displayPath
                    displayPath = ""
                Else
                    Response.Write(" im in else..." & "<br>")
                    index = displayPath.IndexOf(System.IO.Path.DirectorySeparatorChar)
                    fName = displayPath.Substring(0, index)
                    displayPath = displayPath.Substring(index + 1)
                End If
                Dim originalFileName As String
                originalFileName = handle.getXmlAttribute(DisplLang, "FileTitles", "File", fName, xmlFilePath)
                If originalFileName = "" Then
                    returnPath = fName
                    rootPath = rootPath & System.IO.Path.DirectorySeparatorChar & fName
                Else
                    returnPath = originalFileName
                    rootPath = rootPath & System.IO.Path.DirectorySeparatorChar & originalFileName
                End If
                Do Until displayPath = ""
                    xmlFilePath = rootPath & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                    Response.Write("xmlFilePath---" & xmlFilePath & "<br>")
                    If displayPath.IndexOf(System.IO.Path.DirectorySeparatorChar) = -1 Then
                        fName = displayPath
                        displayPath = ""
                    Else
                        index = displayPath.IndexOf(System.IO.Path.DirectorySeparatorChar)
                        fName = displayPath.Substring(0, index)
                        displayPath = displayPath.Substring(index + 1)
                    End If
                    originalFileName = handle.getXmlAttribute(DisplLang, "FileTitles", "File", fName, xmlFilePath)
                    If originalFileName = "" Then
                        returnPath = returnPath & System.IO.Path.DirectorySeparatorChar & fName
                        rootPath = rootPath & System.IO.Path.DirectorySeparatorChar & fName
                    Else
                        returnPath = returnPath & System.IO.Path.DirectorySeparatorChar & originalFileName
                        rootPath = rootPath & System.IO.Path.DirectorySeparatorChar & originalFileName
                    End If
                Loop
                Return returnPath
            End Function

            Private Sub pathBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pathBox.TextChanged
                Dim pathStr As String
                If pathBoxTitle.Text = "Path" Then
                    pathStr = pathBox.Text
                    pathStr = pathStr.Replace("/", System.IO.Path.DirectorySeparatorChar)
                    Dim len As Integer
                    len = pathStr.Length
                    Dim ch As Char = pathStr.Chars(len - 1)
                    If ch = System.IO.Path.DirectorySeparatorChar Then
                        pathStr = pathStr.Substring(1, len - 2)
                    End If
                    pathStr = handle.StrDir(pathStr)
                    pathStr = getOpenFolderURL(pathStr, relativePath)
                    Utils.RedirectTemporary(HttpContext.Current, pathStr)
                Else
                    Response.Write("in textchanged   " & pathBox.Text & "<br>")
                    Dim str As String
                    str = convertdisplayLangPathToRelativePath(pathBox.Text)
                    str = handle.StrDir(str)
                    str = getOpenFolderURL(str, relativePath)
                    Utils.RedirectTemporary(HttpContext.Current, str)
                End If

                'Response.Redirect("index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub

            Private Sub btnGo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGo.Click
                pathBox_TextChanged(sender, e)

            End Sub 'btnGo_Click

            Private Sub addHideFilesTagToOldIndexdataXmlFile(ByVal xmlGe As Hashtable, ByVal xmlEn As Hashtable)

                Dim xmlGeFNHt As Hashtable = xmlGe
                Dim xmlEnFNHt As Hashtable = xmlEn

                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"

                Dim writer As XmlTextWriter = New XmlTextWriter(xmlFile, Nothing)

                writer.Formatting = Formatting.Indented
                writer.WriteStartDocument()

                writer.WriteStartElement("Config")
                writer.WriteStartElement("LanguageID0")
                writer.WriteStartElement("FileNumbers")
                writer.WriteStartElement("File")
                writer.WriteAttributeString("name", "")
                writer.WriteString("")

                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("HideFiles")
                writer.WriteStartElement("File")
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID1")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                Dim title As String
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                Dim efEnum As IDictionaryEnumerator = xmlEnFNHt.GetEnumerator()
                While efEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("name", CType(efEnum.Key, String))
                    writer.WriteString(CType(efEnum.Value, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID2")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                Dim gfEnum As IDictionaryEnumerator = xmlGeFNHt.GetEnumerator()
                While gfEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("name", CType(gfEnum.Key, String))
                    writer.WriteString(CType(gfEnum.Value, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()



                writer.WriteEndElement()

                writer.WriteEndDocument()

                writer.Flush()
                writer.Close()



            End Sub

            Private Function getXmlHiddenFileList() As ArrayList
                Dim hiddenFileArrayList As New ArrayList
                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"
                Dim xmlFI As fileInfo = New fileInfo(xmlFile)
                If Not xmlFI.Exists Then
                    'Response.Write("Error!!!! indexdata.xml doesnot exist. <br>")
                    Dim fs As FileStream = File.Create(xmlFile)
                    fs.Close()
                    'create indexdata.xml
                    createBlankIndexDataXML()
                    'Exit Function
                End If

                Dim hiddenFiles As String() = Nothing
                Try
                    hiddenFiles = handle.getXmlValues(0, "HideFiles", "File", , xmlFile)
                    If hiddenFiles Is Nothing Then
                        Dim xmlFileNoHt As Hashtable = getXmlFileNoHashTable()
                        Dim xmlGeFNHt As Hashtable = getXmlGermanFileNameHashTable()
                        Dim xmlEnFNHt As Hashtable = getXmlEnglishFileNameHashTable()
                        addHideFilesTagToOldIndexdataXmlFile(xmlGeFNHt, xmlEnFNHt)
                        hiddenFiles = handle.getXmlValues(0, "HideFiles", "File", , xmlFile)
                    End If
                Catch ex As Exception
                    cammWebManager.Log.RuntimeWarning(ex)
                    Trace.Warn(String.Format("The process failed: {0}", ex.ToString()))
                End Try
                Dim hFile As String
                For Each hFile In hiddenFiles
                    hiddenFileArrayList.Add(hFile)
                Next

                Return hiddenFileArrayList
            End Function

            Private Sub btnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHide.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString

                Dim gridItem As DataGridItem
                Dim count As Integer = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        count += 1
                    End If
                Next
                Session("CheckedCountSession") = count
                If count = 0 Then
                    Exit Sub
                End If
                'collect hidden files list from indexdata.xml file
                Dim hiddenFileArrayList As ArrayList = getXmlHiddenFileList()

                fCount = 0
                dCount = 0
                count = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        Dim name As String
                        If CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String) = handle.DataType_Folder Then
                            'name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            'gridItem.Enabled = False
                            'dirNameArr(dCount) = name
                            name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            If Not hiddenFileArrayList.Contains(name) Then
                                hiddenFileArrayList.Add(name)
                            End If

                            dCount += 1
                        Else
                            Dim ext As String = CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            If ext = "" Then
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            Else
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & ext
                            End If
                            'gridItem.Enabled = False
                            'name = relativePath & System.IO.Path.DirectorySeparatorChar & name
                            If Not hiddenFileArrayList.Contains(name) Then
                                hiddenFileArrayList.Add(name)
                            End If
                            'fileNameArr(fCount) = name
                            fCount += 1
                        End If
                        count += 1
                        'chkBox.Checked = False
                    End If
                Next

                Dim xmlFileNoHt As Hashtable = getXmlFileNoHashTable()
                Dim xmlGeFNHt As Hashtable = getXmlGermanFileNameHashTable()
                Dim xmlEnFNHt As Hashtable = getXmlEnglishFileNameHashTable()

                addHiddenFilesToIndexdataXml(xmlGeFNHt, xmlEnFNHt, hiddenFileArrayList)


                Session("PageSizeSession") = pageSizeList.SelectedIndex
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                'bindData(sortString)
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))

            End Sub 'end of btnHide_Click

            Private Sub addHiddenFilesToIndexdataXml(ByVal xmlGe As Hashtable, ByVal xmlEn As Hashtable, ByVal hArr As ArrayList)

                Dim xmlGeFNHt As Hashtable = xmlGe
                Dim xmlEnFNHt As Hashtable = xmlEn
                Dim hiddenFileArr As ArrayList = hArr

                Dim xmlFile As String
                xmlFile = handle.StrDir(relativePath) & System.IO.Path.DirectorySeparatorChar & "indexdata.xml"

                Dim writer As XmlTextWriter = New XmlTextWriter(xmlFile, Nothing)

                writer.Formatting = Formatting.Indented
                writer.WriteStartDocument()

                writer.WriteStartElement("Config")
                writer.WriteStartElement("LanguageID0")
                writer.WriteStartElement("FileNumbers")
                writer.WriteStartElement("File")
                writer.WriteAttributeString("name", "")
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("HideFiles")
                Dim hfEnum As IEnumerator = hiddenFileArr.GetEnumerator()
                While hfEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteString(CType(hfEnum.Current, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID1")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                Dim title As String
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                Dim efEnum As IDictionaryEnumerator = xmlEnFNHt.GetEnumerator()
                While efEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("name", CType(efEnum.Key, String))
                    writer.WriteString(CType(efEnum.Value, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteStartElement("LanguageID2")
                writer.WriteStartElement("General")
                writer.WriteStartElement("DirTitle")
                If handle.GetTitle(2, Request.PhysicalPath) = "" Then
                    title = "WEBSITE EXPLORER"
                Else
                    title = handle.GetTitle(2, Request.PhysicalPath)
                End If
                writer.WriteString(title)
                writer.WriteEndElement()
                writer.WriteEndElement()
                writer.WriteStartElement("FileTitles")
                Dim gfEnum As IDictionaryEnumerator = xmlGeFNHt.GetEnumerator()
                While gfEnum.MoveNext()
                    writer.WriteStartElement("File")
                    writer.WriteAttributeString("name", CType(gfEnum.Key, String))
                    writer.WriteString(CType(gfEnum.Value, String))
                    writer.WriteEndElement()
                End While
                writer.WriteEndElement()
                writer.WriteEndElement()

                writer.WriteEndElement()

                writer.WriteEndDocument()

                writer.Flush()
                writer.Close()

            End Sub

            Private Sub btnUnHide_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnHide.Click
                Dim dv1 As DataView = CType(Session("Source"), DataView)
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                dv1.Sort = sortString

                Dim gridItem As DataGridItem
                Dim count As Integer = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        count += 1
                    End If
                Next
                Session("CheckedCountSession") = count
                If count = 0 Then
                    Exit Sub
                End If
                'collect hidden files list from indexdata.xml file
                Dim hiddenFileArrayList As ArrayList = getXmlHiddenFileList()

                fCount = 0
                dCount = 0
                count = 0
                For Each gridItem In expAdminGrid.Items
                    Dim chkBox As CheckBox = CType(gridItem.Cells(0).Controls(0), CheckBox)
                    If chkBox.Checked = True Then
                        Dim name As String
                        If CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String) = handle.DataType_Folder Then
                            'name = relativePath & System.IO.Path.DirectorySeparatorChar & CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            'gridItem.Enabled = False
                            'dirNameArr(dCount) = name
                            name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            If hiddenFileArrayList.Contains(name) Then
                                hiddenFileArrayList.Remove(name)
                            End If

                            dCount += 1
                        Else
                            Dim ext As String = CType(dv1.Item(gridItem.DataSetIndex).Item("DataType"), String)
                            If ext = "" Then
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String)
                            Else
                                name = CType(dv1.Item(gridItem.DataSetIndex).Item("Name"), String) & "." & ext
                            End If
                            'gridItem.Enabled = False
                            'name = relativePath & System.IO.Path.DirectorySeparatorChar & name
                            If hiddenFileArrayList.Contains(name) Then
                                hiddenFileArrayList.Remove(name)
                            End If
                            'fileNameArr(fCount) = name
                            fCount += 1
                        End If
                        count += 1
                        'chkBox.Checked = False
                    End If
                Next

                Dim xmlFileNoHt As Hashtable = getXmlFileNoHashTable()
                Dim xmlGeFNHt As Hashtable = getXmlGermanFileNameHashTable()
                Dim xmlEnFNHt As Hashtable = getXmlEnglishFileNameHashTable()

                addHiddenFilesToIndexdataXml(xmlGeFNHt, xmlEnFNHt, hiddenFileArrayList)


                Session("PageSizeSession") = pageSizeList.SelectedIndex
                If Session("ascSession") Is Nothing Then
                    sortString = "SortColumn,LastModified ASC"
                Else
                    sortString = CType(Session("ascSession"), String)
                End If
                'bindData(sortString)
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub 'end of btnUnHide_Click

        End Class
#End Region

#Region " Public Class Editor "
        <Obsolete("Use CompuMaster.camm.WebExplorer.* instead")> Public Class Editor
            Inherits InternalBaseExplorer

#Region " Variables "
            Protected tCell As TableCell
            Protected tRow As TableRow
            Protected dirgroup As DirectoryInfo
            Protected f As FileInfo
            Protected nameOfFile As String
            Protected myFilePath As String
            Protected relativePath As String
            Protected showLines As Boolean = False

            Protected tboxedit As System.Web.UI.WebControls.TextBox
            Protected btncancel As System.Web.UI.WebControls.ImageButton
            Protected btnsave As System.Web.UI.WebControls.ImageButton
            Protected btnShowLineNo As System.Web.UI.WebControls.ImageButton
            Protected TableTitleMain As System.Web.UI.WebControls.Table
            Protected txtViewTable As System.Web.UI.WebControls.Table

#End Region

            Protected Sub LoadAndInitialize(ByVal obj As Object, ByVal e As EventArgs) Handles MyBase.Load

                nameOfFile = Replace(Replace(handle.GetHackCheckedSubPath(Request.QueryString("file")), "\", ""), "/", "")

                myFilePath = handle.StrDir(Request.Params("path")) & System.IO.Path.DirectorySeparatorChar & nameOfFile
                btnsave.ToolTip = handle.Editor_SaveButton
                btncancel.ToolTip = handle.Editor_CancelButton

                If Not cammWebManager.IsUserAuthorized(handle.AuthAppTitle2ReadWrite) Or handle.IsOwnSystemFile(nameOfFile, Request.PhysicalPath, cammWebManager.IsUserAuthorized(handle.AuthAppTitle2ReadWrite)) Then
                    If Not Session("ShowLineSession") Is Nothing Then
                        showLines = CType(Session("ShowLineSession"), Boolean)
                    End If
                    If showLines Then
                        FileViewer()
                    Else
                        FileViewerWithLineNos()
                    End If
                Else
                    FileEditor()
                End If
            End Sub

            Protected Sub FileEditor()
                tboxedit.ReadOnly = False
                btnsave.Visible = True
                'PlaceBody.Visible = False 'TODO 4 HP: Review
                btnShowLineNo.Visible = False

                If File.Exists(myFilePath) Then
                    Dim fs As FileStream
                    Dim sr As StreamReader
                    Dim fc As String

                    f = New FileInfo(myFilePath)

                    tRow = New TableRow

                    tCell = New TableCell
                    Dim imgCtrl As Image
                    imgCtrl = New Image
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/editor_header_ico.gif"
                    imgCtrl.BorderStyle = BorderStyle.None
                    tCell.Controls.Add(imgCtrl)
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    tCell.Width = Unit.Percentage(100)
                    Dim fName As String = handle.GetRelativePath2StartDir(f.FullName, Request.PhysicalPath)
                    tCell.Text = Path.GetFileName(fName) & " - " & handle.Editor_DialogTitle
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    tCell.Text = handle.ApplicationTopTitle
                    tRow.Cells.Add(tCell)

                    TableTitleMain.Rows.Add(tRow)

                    Select Case LCase(Replace(f.Extension, ".", "", , 1))
                        Case "aspx", "asp", "html", "htm", "asmx", "cs", "vb", "css", "inc", "txt", "php", "php3", "cfm", "js", "ascx", "log", "xml", "xsl"
                            fs = New FileStream(f.FullName, FileMode.Open)
                            sr = New StreamReader(fs)
                            Do While sr.Peek() <> -1
                                fc = sr.ReadLine()
                                tboxedit.Text += fc & System.Environment.NewLine
                            Loop
                            If Right(tboxedit.Text, Len(System.Environment.NewLine)) = System.Environment.NewLine Then
                                'remove last System.Environment.NewLine which has been added one time too much
                                tboxedit.Text = Mid(tboxedit.Text, 1, Len(tboxedit.Text) - Len(System.Environment.NewLine))
                            End If
                            fs.Close()
                            sr.Close()
                        Case "jpg", "gif", "bmp", "jpeg", "ico"
                            'TODO 4 HP: Review
                            'Dim img1 As New Image
                            'img1.ImageUrl = "downloadfile.aspx?path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")) & "&file=" & System.Web.HttpUtility.UrlEncode(nameOfFile)
                            'PlaceBody.Controls.Add(img1)
                            tboxedit.Visible = False
                            btnsave.Visible = False
                        Case Else
                            tboxedit.Visible = False
                            btnsave.Visible = False
                    End Select
                End If

            End Sub

            Protected Sub FileViewer()
                tboxedit.ReadOnly = True
                tboxedit.Visible = True
                tboxedit.Text = ""
                btnsave.Visible = False
                'PlaceBody.Visible = False 'TODO 4 HP: Review
                btnShowLineNo.Visible = True
                btnShowLineNo.ImageUrl = "/system/modules/explorer/images/showFileWithNoLineNumber.gif"
                btnShowLineNo.ToolTip = handle.btnShowLineFalseTooltipStr

                If File.Exists(myFilePath) Then
                    Dim fs As FileStream = Nothing
                    Dim sr As StreamReader = Nothing
                    Dim fc As String
                    Dim f As FileInfo
                    f = New FileInfo(myFilePath)

                    tRow = New TableRow

                    tCell = New TableCell
                    Dim imgCtrl As Image
                    imgCtrl = New Image
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/editor_header_ico.gif"
                    imgCtrl.BorderStyle = BorderStyle.None
                    tCell.Controls.Add(imgCtrl)
                    'tCell.Attributes.Add("background", "/system/modules/explorer/images/background_symbolline.gif")
                    tRow.Cells.Add(tCell)


                    tCell = New TableCell
                    tCell.Width = Unit.Percentage(100)
                    Dim fName As String = handle.GetRelativePath2StartDir(f.FullName, Request.PhysicalPath)
                    tCell.Text = Path.GetFileName(fName) & " - " & handle.Viewer_DialogTitle
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    'tCell.Width = Unit.Percentage(100)
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    tCell.Text = handle.ApplicationTopTitle
                    tRow.Cells.Add(tCell)

                    TableTitleMain.Rows.Add(tRow)

                    Select Case LCase(Replace(f.Extension, ".", "", , 1))
                        Case "aspx", "asp", "html", "htm", "asmx", "cs", "vb", "css", "inc", "txt", "php", "php3", "cfm", "js", "ascx", "log", "xml", "xsl"
                            fs = New FileStream(f.FullName, FileMode.Open)
                            sr = New StreamReader(fs)
                            Do While sr.Peek() <> -1
                                fc = sr.ReadLine()
                                tboxedit.Text += fc & System.Environment.NewLine
                            Loop
                            If Right(tboxedit.Text, Len(System.Environment.NewLine)) = System.Environment.NewLine Then
                                'remove last System.Environment.NewLine which has been added one time too much
                                tboxedit.Text = Mid(tboxedit.Text, 1, Len(tboxedit.Text) - Len(System.Environment.NewLine))
                            End If
                            fs.Close()
                            sr.Close()
                        Case "jpg", "gif", "bmp", "jpeg", "ico"
                            'TODO 4 HP: Review
                            'Dim img1 As New Image
                            'img1.ImageUrl = "downloadfile.aspx?path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")) & "&file=" & System.Web.HttpUtility.UrlEncode(nameOfFile)
                            'PlaceBody.Controls.Add(img1)
                            tboxedit.Visible = False
                            'btnSave.visible = false
                        Case Else
                            tboxedit.Visible = False
                            'btnSave.visible = false
                    End Select
                    If Not fs Is Nothing Then
                        fs.Close()
                    End If
                    If Not sr Is Nothing Then
                        sr.Close()
                    End If
                End If


            End Sub

            Protected Sub FileViewerWithLineNos()

                tboxedit.ReadOnly = True
                btnsave.Visible = False
                tboxedit.Visible = False
                'PlaceBody.Visible = True 'TODO 4 HP: Review
                btnShowLineNo.Visible = True
                btnShowLineNo.ImageUrl = "/system/modules/explorer/images/showFileWithLineNo.gif"
                btnShowLineNo.ToolTip = handle.btnShowLineTrueTooltipStr

                If File.Exists(myFilePath) Then
                    Dim f As FileInfo
                    f = New FileInfo(myFilePath)

                    tRow = New TableRow

                    tCell = New TableCell
                    Dim imgCtrl As Image
                    imgCtrl = New Image
                    imgCtrl.ImageUrl = "/system/modules/explorer/images/editor_header_ico.gif"
                    imgCtrl.BorderStyle = BorderStyle.None
                    tCell.Controls.Add(imgCtrl)
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    tCell.Width = Unit.Percentage(100)
                    Dim fName As String = handle.GetRelativePath2StartDir(f.FullName, Request.PhysicalPath)
                    tCell.Text = Path.GetFileName(fName) & " - " & handle.Viewer_DialogTitle
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    'tCell.Width = Unit.Percentage(100)
                    tRow.Cells.Add(tCell)

                    tCell = New TableCell
                    tCell.Text = handle.ApplicationTopTitle
                    tRow.Cells.Add(tCell)

                    TableTitleMain.Rows.Add(tRow)
                End If
                WebViewerTable()
            End Sub

            Protected Sub WebViewerTable()

                Dim fc As String
                Dim ctr As Integer = 0
                Dim fs As FileStream
                fs = New FileStream(myFilePath, FileMode.Open)
                Dim sr As StreamReader
                sr = New StreamReader(fs)

                Do While sr.Peek() <> -1
                    ctr += 1
                    tRow = New TableRow

                    tCell = New TableCell
                    tCell.ForeColor = System.Drawing.Color.Green
                    tCell.HorizontalAlign = HorizontalAlign.Right
                    tCell.VerticalAlign = VerticalAlign.Top
                    tCell.Width = Unit.Pixel(40)
                    tCell.Text = ctr.ToString() & ":"
                    tRow.Cells.Add(tCell)

                    fc = sr.ReadLine()
                    Dim str As String
                    str = System.Web.HttpUtility.HtmlEncode(fc)

                    Dim newLineStr As String = System.Environment.NewLine
                    str.Replace(newLineStr, "")

                    tCell = New TableCell
                    tCell.VerticalAlign = VerticalAlign.Middle
                    tCell.HorizontalAlign = HorizontalAlign.Left
                    tCell.Text = str
                    tRow.Cells.Add(tCell)

                    txtViewTable.Rows.Add(tRow)
                Loop
                fs.Close()
                sr.Close()
            End Sub

            Protected Sub SaveChanges(ByVal obj As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
                If Not handle.IsOwnSystemFile(nameOfFile, Request.PhysicalPath, cammWebManager.IsUserAuthorized(handle.AuthAppTitle2ReadWrite)) Then
                    Dim Filewriter As StreamWriter
                    Filewriter = File.CreateText(myFilePath)
                    Filewriter.WriteLine(Request.Form("tboxedit"))
                    Filewriter.Close()
                End If
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub

            Protected Sub ShowLineNos(ByVal obj As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
                If showLines Then
                    'FileViewer()
                    showLines = False
                Else
                    'FileViewerWithLineNos()
                    showLines = True
                End If
                Session("ShowLineSession") = showLines
            End Sub

            Protected Sub CancelChanges(ByVal obj As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
                Utils.RedirectTemporary(HttpContext.Current, "index.aspx?xn=d&path=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("path")))
            End Sub

        End Class
#End Region

#Region " Public Class DownloadFile "
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Modules.WebExplorer.Pages.DownloadFile
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Processes the download of file.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	05.07.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <Obsolete("Use CompuMaster.camm.WebExplorer.* instead")> Public Class DownloadFile
            Inherits InternalBaseExplorer

            <Obsolete("Rename this method to ""PageOnLoad"" to prevent doubled execution")> Protected Sub Page_Load(ByVal obj As Object, ByVal e As EventArgs) Handles MyBase.Load

                HttpContext.Current.Response.Buffer = True
                Dim nameOfFile As String
                Dim f As String = Nothing
                Try
                    HttpContext.Current.Response.Charset = "UTF-8" '"WINDOWS-1252"

                    nameOfFile = Replace(Replace(MyBase.handle.GetHackCheckedSubPath(Request.QueryString("file")), "\", ""), "/", "")
                    'Dim relativePath As String = MyBase.handle.StrDir(Request.Params("path"))
                    Dim relativePath As String = Request.Params("path")

                    If nameOfFile = "" Then
                        f = ""
                    Else
                        f = MyBase.handle.StrDir(Request.Params("path")) & "\" & Request.QueryString("file")
                    End If
                    Dim fPath As String = f

                    Dim fi As New FileInfo(fPath)

                    If fi.Exists Then
                        Me.ProcessDownload(fi, relativePath)
                    Else
                        Throw New Exception("File """ & System.IO.Path.GetFileName(f) & """ doesn't exist")
                    End If

                Catch ex As Exception
                    Throw New HttpException(404, "File """ & System.IO.Path.GetFileName(f) & """ doesn't exist", ex)
                End Try

            End Sub

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Processes the download.
            ''' </summary>
            ''' <param name="fileInfo"></param>
            ''' <param name="relativePath"></param>
            ''' <remarks>
            '''     The download is processed by "Download Hander" with write access available, else 
            '''     file is redirected to browser.
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	05.07.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Private Sub ProcessDownload(ByVal fileInfo As FileInfo, ByVal relativePath As String)
                Try
                    Dim physicalPath As String = HttpContext.Current.Request.PhysicalPath
                    Dim downloadLocation As CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations
                    Select Case Me.handle.getXmlValue(0, "General", "DownloadLocation", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                        Case "WebServerSession"
                            downloadLocation = DownloadHandler.DownloadLocations.WebServerSession
                        Case "WebManagerUserSession"
                            downloadLocation = DownloadHandler.DownloadLocations.WebManagerUserSession
                        Case "WebManagerSecurityObjectName"
                            downloadLocation = DownloadHandler.DownloadLocations.WebManagerSecurityObjectName
                        Case Else
                            downloadLocation = DownloadHandler.DownloadLocations.PublicCache
                    End Select

                    Dim limit As Long
                    Dim ts As New TimeSpan(0, 5, 0)
                    Try
                        Dim temp As String = Me.handle.getXmlValue(0, "General", "FileDownloadSizeLimit", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                        If temp <> "" Then
                            limit = System.Convert.ToInt64(temp)
                        End If
                        temp = Me.handle.getXmlValue(0, "General", "CachingTimeLimitInMinits", , physicalPath.Substring(0, physicalPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)) & System.IO.Path.DirectorySeparatorChar & "config.xml")
                        If temp <> "" Then
                            ts = Nothing
                            ts = New TimeSpan(0, System.Convert.ToInt32(temp), 0)
                        End If
                    Catch ex As Exception
                        ts = New TimeSpan(0, 5, 0)
                    End Try

                    If limit <> Nothing Then
                        cammWebManager.DownloadHandler.MaxDownloadSize = limit
                        cammWebManager.DownloadHandler.MaxDownloadCollectionSize = limit
                    End If

                    Dim folderInVirtualDownloadLocation As String = DownloadHandler.ComputeHashedPathFromLongPath(Me.handle.PathStart & "\" & relativePath)
                    If cammWebManager.DownloadHandler.IsFullyFeatured Then
                        If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(downloadLocation, "cwx", folderInVirtualDownloadLocation, fileInfo.Name) Then
                            Utils.RedirectTemporary(HttpContext.Current, cammWebManager.DownloadHandler.CreateDownloadLink(downloadLocation, "cwx", folderInVirtualDownloadLocation, fileInfo.Name))
                        Else
                            'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                            cammWebManager.DownloadHandler.Clear()
                            cammWebManager.DownloadHandler.Add(fileInfo, folderInVirtualDownloadLocation)

                            'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                            cammWebManager.DownloadHandler.ProcessDownload(downloadLocation, "cwx", False, ts)
                        End If
                    Else
                        'Throw New Exception("Downloads in separate HTTP requests are not supported on this webserver")
                        'result = "downloadfile.aspx?lang=" & MyBase.DisplLang & "&path=" & HttpContext.Current.Server.UrlEncode(handle.GetRelativePath2StartDir(fileInfo.Directory.FullName, relativePath)) & "&file=" & HttpContext.Current.Server.UrlEncode(fileInfo.Name) & "&ie=" & handle.GetWin95IE5CompatibleFileName(fileInfo.Name)

                        ' If the file exists, send it to the browser as HTML.
                        'Response.AddHeader ("Content-disposition","attachment;filename=" & System.Web.HttpUtility.urlencode(nameoffile))  'ImmerDownload
                        Response.AddHeader("Content-disposition", "filename=" & fileInfo.Name)  'AutoDownload oder NurWennKeineAndereAktion ???
                        'Response.AddHeader ("Content-disposition","inline;filename=" & nameoffile) 'NoDownload oder NurWennKeineAndereAktion ???
                        'Response.AddHeader ("content-length", fsoFile.Size)
                        If Len(fileInfo.FullName) >= 4 Then
                            Select Case LCase(Right(fileInfo.Name, 4))
                                Case ".asf"
                                    Response.ContentType = "video/x-ms-asf"
                                Case ".avi"
                                    Response.ContentType = "video/avi"
                                Case ".doc", ".dot"
                                    Response.ContentType = "application/msword"
                                Case ".zip"
                                    Response.ContentType = "application/zip"
                                Case ".xls"
                                    Response.ContentType = "application/vnd.ms-excel"
                                Case ".gif"
                                    Response.ContentType = "image/gif"
                                Case ".bmp"
                                    Response.ContentType = "image/bmp"
                                Case ".jpg", "jpeg"
                                    Response.ContentType = "image/jpeg"
                                Case ".wav"
                                    Response.ContentType = "audio/wav"
                                Case ".mp3"
                                    Response.ContentType = "audio/mpeg3"
                                Case ".mpg", "mpeg", "divx"
                                    Response.ContentType = "video/mpeg"
                                Case ".rtf"
                                    Response.ContentType = "application/rtf"
                                Case ".htm", "html"
                                    Response.ContentType = "text/html"
                                Case ".asp"
                                    Response.ContentType = "text/asp"
                                Case ".txt", ".log"
                                    Response.ContentType = "text/plain"
                                Case ".pdf"
                                    Response.ContentType = "application/pdf"
                                Case ".doc"
                                    Response.ContentType = "application/msword"
                                Case ".xls"
                                    Response.ContentType = "application/vnd.ms-excel"
                                Case ".pps"
                                    Response.ContentType = "application/vnd.ms-powerpoint"
                                Case ".ppt"
                                    Response.ContentType = "application/vnd.ms-powerpoint"
                                Case ".zip"
                                    Response.ContentType = "application/x-zip-compressed"
                                Case ".eps"
                                    Response.ContentType = "application/postscript"
                                Case ".tif", "tiff"
                                    Response.ContentType = "image/tiff"
                                Case ".svg", "svgz"
                                    Response.ContentType = "image/svg+xml"
                                Case ".swf"
                                    Response.ContentType = "application/x-shockwave-flash"
                                Case Else
                                    Response.ContentType = "application/octet-stream"
                            End Select
                        Else
                            Response.ContentType = "application/octet-stream"
                        End If
                        Response.WriteFile(fileInfo.FullName)
                    End If

                Catch exFileSize As CompuMaster.camm.WebManager.DownloadHandlerFileSizeLimitException
                    Dim ErrMessage As String
                    ErrMessage = fileInfo.Name & " exceeds download file size limit."
                    If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        ErrMessage &= " The detailled message is "
                        ErrMessage &= exFileSize.ToString
                    End If
                    Throw New Exception(ErrMessage)
                Catch exCollectionSize As CompuMaster.camm.WebManager.DownloadHandlerCollectionSizeLimitException
                    Dim ErrMessage As String
                    ErrMessage = fileInfo.Name & " exceeds download collection size limit."
                    If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        ErrMessage &= " The detailled message is "
                        ErrMessage &= exCollectionSize.ToString
                    End If
                    Throw New Exception(ErrMessage)
                Catch exSecurityObjectNothing As CompuMaster.camm.WebManager.EmptySecurityObjectException
                    Dim ErrMessage As String
                    ErrMessage = exSecurityObjectNothing.Message
                    If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        ErrMessage &= " The detailled message is "
                        ErrMessage &= exSecurityObjectNothing.ToString
                    End If
                    Throw New Exception(ErrMessage)
                Catch ex As Exception
                    Dim ErrMessage As String
                    ErrMessage = ex.Message
                    If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                        ErrMessage &= " The detailled message is "
                        ErrMessage &= ex.tostring
                    End If
                    Throw New Exception(ErrMessage)
                End Try

            End Sub

        End Class
#End Region

#Region " Public Class AutoDownload "
        <Obsolete("Use CompuMaster.camm.WebExplorer.* instead")> Public Class AutoDownload
            Inherits InternalBaseExplorer

            Public dirgroup As DirectoryInfo
            Public f As FileInfo
            Public d As DirectoryInfo

            <Obsolete("Rename this method to ""PageOnLoad"" with handles clause to prevent doubled execution")> Sub Page_Load(ByVal obj As Object, ByVal e As EventArgs)
                DisplLang = cammWebManager.UI.MarketID
                handle.InitializeLanguage(DisplLang, Request.PhysicalPath, Request.ServerVariables("SERVER_NAME"))

                Dim ServerTransferURL As String = Nothing
                Try
                    ServerTransferURL = SearchDocuments()
                Catch ex As Exception
                    Response.Write("<html><head><title>Error</title></head><body><h1>404 File not found</h1>" & vbNewLine & "<!-- " & System.Web.HttpUtility.HtmlEncode(ex.Message) & " --></body></html>")
                End Try
                If ServerTransferURL <> "" Then
                    Response.Clear()
                    Server.Transfer(ServerTransferURL)
                End If
            End Sub

            Function SearchDocuments() As String

                If IsNothing(Request.QueryString("path")) Then
                    dirgroup = New DirectoryInfo(handle.StrDir(Request.PhysicalPath))
                    If Not dirgroup.Exists Then
                        dirgroup = New DirectoryInfo(handle.StrDir(Request.PhysicalPath, True))
                    End If
                Else
                    Dim dPath As String = ""
                    dPath = handle.StrDir(Request.PhysicalPath)
                    dirgroup = New DirectoryInfo(dPath & Request.QueryString("path"))
                    If Not dirgroup.Exists Then
                        dPath = handle.StrDir(Request.PhysicalPath, True)
                        dirgroup = New DirectoryInfo(dPath & Request.QueryString("path"))
                    End If
                End If

                Dim DirectoryContent_Files As FileInfo()
                If Request.QueryString("q") Is Nothing AndAlso Request.QueryString("q") = "" Then
                    Throw New Exception("Parameter missing")
                Else
                    If Request.QueryString("qt") Is Nothing OrElse Request.QueryString.GetValues("qt").Length = 0 Then
                        DirectoryContent_Files = MyBase.handle.GetQueriedVersionedFiles(dirgroup, Request.PhysicalPath, MyBase.handle.ConfigSetting_RegexExpression_Searches, Request.QueryString("q"))
                    Else
                        DirectoryContent_Files = MyBase.handle.GetQueriedVersionedFiles(dirgroup, Request.PhysicalPath, MyBase.handle.ConfigSetting_RegexExpression_Searches, Request.QueryString("q"), Request.QueryString.GetValues("qt"))
                    End If
                End If


                If DirectoryContent_Files Is Nothing Then
                    Throw New Exception("No file found matching the required specifications")
                ElseIf DirectoryContent_Files.Length = 1 Then
                    Utils.RedirectTemporary(HttpContext.Current, "downloadfile.aspx?path=" & System.Web.HttpUtility.UrlEncode(handle.GetRelativePath2StartDir(dirgroup.ToString, Request.PhysicalPath)) & "&file=" & System.Web.HttpUtility.UrlEncode(DirectoryContent_Files(0).Name) & "&ie=" & handle.GetWin95IE5CompatibleFileName(DirectoryContent_Files(0).Name))
                Else
                    Dim path As String = Request.QueryString("path")
                    If path.Length > 1 Then
                        If path.Chars(0) = "\" Then
                            path = path.Substring(1)
                        End If
                    End If

                    If Request.QueryString("qt") Is Nothing OrElse Request.QueryString.GetValues("qt").Length = 0 Then
                        'Return "index.aspx?q=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("q"))
                        Return "index.aspx?path=" & path & "&q=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("q"))
                    Else
                        Dim QTs As String = ""
                        For MyCounter As Integer = 0 To Request.QueryString.GetValues("qt").GetUpperBound(0)
                            QTs &= "&qt=" & System.Web.HttpUtility.UrlEncode(Request.QueryString.GetValues("qt")(MyCounter))
                        Next
                        'Return "index.aspx?q=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("q")) & QTs
                        Return "index.aspx?path=" & path & "&q=" & System.Web.HttpUtility.UrlEncode(Request.QueryString("q")) & QTs
                    End If
                End If

                Return Nothing
            End Function

        End Class
#End Region

    End Namespace
#End Region
End Namespace