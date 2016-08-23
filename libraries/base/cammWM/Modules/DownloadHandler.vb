'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.IO
Imports System.Data.SqlClient
Imports ICSharpCode.SharpZipLib.Zip

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     The download handler provides mechanisms for creating temporary files with automatic removal after they're not required any more.
    '''     If collection contains only single file, then download handler creates hard link for that file in the download location. In case there is error
    '''     while creating hard link the download handler copies the file to download location. For all other possible collection, download handler archives the
    '''     download collection to zip format and put this zip archive in download location. 
    ''' </summary>
    ''' <remarks>
    '''     The features are that
    '''     <list>
    '''         <item>The application only has to create those temporary files once and the next requests can use a cached file</item>
    '''         <item>All browsers are correctly handling the name of file because the downloaded file doesn't rely on a download script file with parameters in the URL respecitvely a file name or MIME type in the headers of the response (which is not always interpreted correctly by the different browsers/platforms)</item>
    '''         <item>Compression of files by providing ZIP archives</item>
    '''         <item>Multiple files can be downloaded in a ZIP file</item>
    '''         <item>High performance downloads inclusive multiple-stream and continue-aborted-downloads support since IIS can handle the file transfer itself</item>
    '''     </list>
    '''     This makes sense especially in following situations:
    '''     <list>
    '''         <item>Download of files from a database</item>
    '''         <item>Download of files which are stored in the local or a network file system but which are not available directly through the webserver</item>
    '''         <item>Download of several files (e. g. from a download cart)</item>
    '''         <item>Download of large files which would require the ASP.NET engine to hold a lot of memory as long as the request has been processed completely</item>
    '''     </list>
    '''     Construction of the complete path:
    '''         IISWebRoot/SystemDownloadFolderForTemporaryFiles/{ResolveSubFolderFromDownloadLocation}/pathInDownloadLocation/folderInVirtualDownloadLocation/FileName.Extension
    '''         IISWebRoot/SystemDownloadFolderForTemporaryFiles/{ResolveSubFolderFromDownloadLocation}/pathInDownloadLocation/zipFile.zip[/folderInVirtualDownloadLocation/FileName.Extension]
    ''' </remarks>
    Public Class DownloadHandler
        Inherits System.Collections.CollectionBase

#Region "Global variables"
        ''' <summary>
        ''' A relative or absolute file location on disc for temporary files
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Private ReadOnly Property SystemDownloadFolderForTemporaryFiles() As String
            Get
                If Me.IsWebApplication Then
                    Return Configuration.DownloadHandlerVirtualTempPath
                Else
                    Return System.IO.Path.DirectorySeparatorChar & "camm Web-Manager" & System.IO.Path.DirectorySeparatorChar
                End If
            End Get
        End Property

        Private Const SeparateRequestSessionKeyPrefix As String = "SeparateRequestSessionKey_"
        Private Const SeparateRequestCacheKeyPrefix As String = "SeparateRequestCacheKey_"

#End Region

#Region "Misc Functions"
        ''' <summary>
        '''     Returns value indicating whether a file exists.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="fileName">Name of file</param>
        Public Function DownloadFileAlreadyExists(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String) As Boolean
            Return DownloadFileAlreadyExists(downloadLocation, pathInDownloadLocation, Nothing, fileName)
        End Function
        ''' <summary>
        '''     Returns value indicating whether a file exists.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="fileName">Name of file</param>
        ''' <returns>True if file exists</returns>
        Public Function DownloadFileAlreadyExists(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal folderInVirtualDownloadLocation As String, ByVal fileName As String) As Boolean
            Dim DownloadFullPath As String = Me.DownloadFolderFullPath & Me.ResolveSubFolderFromDownloadLocation(downloadLocation) & System.IO.Path.DirectorySeparatorChar & pathInDownloadLocation

            If folderInVirtualDownloadLocation = Nothing Then
                DownloadFullPath &= System.IO.Path.DirectorySeparatorChar & fileName
            Else
                DownloadFullPath &= System.IO.Path.DirectorySeparatorChar & folderInVirtualDownloadLocation & System.IO.Path.DirectorySeparatorChar & fileName
            End If

            Dim DirInfo As System.IO.DirectoryInfo
            Try
                DirInfo = New System.IO.DirectoryInfo(DownloadFullPath)
            Catch ex As Exception
                If Me._WebManager.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                    Throw New Exception("Cannot check if downloadfolder for '" & DownloadFullPath & "' exists.", ex)
                Else
                    Throw New Exception("Cannot check if downloadfolder for '" & DownloadFullPath & "' exists.")
                End If
            End Try

            Return DirInfo.Exists
        End Function


        Private _IsFullyFeatured As TriState = TriState.UseDefault
        ''' <summary>
        '''     Returns value whether download folder has write access.
        ''' </summary>
        ''' <remarks>
        '''     This functions check for download folder access writes, like write access, to create new file, can execute zip archive utility.
        ''' </remarks>
        Public Function IsFullyFeatured() As Boolean
            'Find cached value if existing
            If Me.IsWebApplication Then
                'Web application, use HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess")
                If Not HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess") Is Nothing Then
                    Return CType(HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess"), Boolean)
                End If
            Else
                'Non web application, use internal variable
                If _IsFullyFeatured = TriState.False Then
                    Return False
                ElseIf _IsFullyFeatured = TriState.True Then
                    Return True
                End If
            End If

            'Some performance counters
#If False And DEBUG Then
        If HttpContext.Current Is Nothing Then
            System.Diagnostics.Trace.Write("IsFullyFeatured Lookup Begin")
        Else
            HttpContext.Current.Trace.Write("IsFullyFeatured Lookup Begin")
        End If
#End If

            'all following code will be executed seldom because there is a cached setting regulary

            'lock application (to avoid 2 process to access the same file on the same time)
            Dim returnBoolean As Boolean
            Try
                If IsWebApplication Then HttpContext.Current.Application.Lock()

                'Avoid unneccessary 2nd file access by a 2nd process immediately after a first file access by a first process (multi-tasking scenario)
                If Me.IsWebApplication AndAlso Not HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess") Is Nothing Then
                    HttpContext.Current.Application.UnLock()
                    Return CType(HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess"), Boolean)
                End If

                'Lookup the result value
                Try
                    Me.TestFileCreation()
                    returnBoolean = True
                Catch ex As Exception
                    returnBoolean = False
                    Me._WebManager.Log.Warn(ex)
                End Try

                'Cache our new value
                If IsWebApplication Then
                    'Web application, use HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess")
                    HttpContext.Current.Application("WebManager.DownloadHandler.WriteAccess") = returnBoolean
                Else
                    'Non web application, use internal variable
                    If returnBoolean = True Then
                        _IsFullyFeatured = TriState.True
                    Else
                        _IsFullyFeatured = TriState.False
                    End If
                End If
            Finally
                'unlock application
                If IsWebApplication Then HttpContext.Current.Application.UnLock()
            End Try

            'Some performance counters
#If False And DEBUG Then
        If HttpContext.Current Is Nothing Then
            System.Diagnostics.Trace.Write("IsFullyFeatured Lookup End")
        Else
            HttpContext.Current.Trace.Write("IsFullyFeatured Lookup End")
        End If
#End If

            Return returnBoolean
        End Function
        ''' <summary>
        '''     Test write access to download folder by creating test.txt file, _
        '''     on success no exception, on failure throws exception 
        ''' </summary>
        Private Sub TestFileCreation()
            Dim fi As New FileInfo(Me.DownloadFolderFullPath & "test.txt")
            If Not fi.Directory.Exists Then
                Try
                    fi.Directory.Create()
                Catch ex As Exception
                    Throw New Exception("DH's IsFullyFeatured test directory could not be created", ex)
                End Try
            End If
            Dim fs As ManagedFileStream = Nothing
            Try
                fs = New ManagedFileStream(fi.Create)
            Catch ex As Exception
                Throw New Exception("DH's IsFullyFeatured test file could not be created", ex)
            Finally
                If Not fs Is Nothing Then
                    fs.Dispose()
                End If
            End Try
        End Sub
        ''' <summary>
        ''' This function return an encrypted, file-system-safe value for the parameter value
        ''' </summary>
        ''' <param name="value">A string value which shall be hashed</param>
        Private Shared Function GetEncryptedValue(ByVal value As String) As String
            'Return the new string with file system compatible naming standard (windows, linux, mac)
            Return FileSystemCompatibleString(Utils.ComputeHash(value))
        End Function
        ''' <summary>
        '''     Computes a hashed filename compatible with the file system without hashing the path parts
        ''' </summary>
        ''' <param name="filePath">A filename with a leading path information</param>
        ''' <returns>A valid, hashed string which only contains characters that are compatible to the local file system</returns>
        Public Shared Function ComputeHashedFilenameFromLongFilename(ByVal filePath As String) As String
            Dim fileName As String = System.IO.Path.GetFileName(filePath)
            Dim parentPath As String = Mid(filePath, 1, InStrRev(filePath, fileName, , CompareMethod.Binary))
            Return parentPath & FileSystemCompatibleString(GetEncryptedValue(System.IO.Path.GetFileNameWithoutExtension(fileName))) & System.IO.Path.GetExtension(fileName)
        End Function
        ''' <summary>
        '''     Computes a unique encrypted file system compatible string from very long folder path.
        ''' </summary>
        ''' <param name="path">The piece of a path that shall be hashed</param>
        ''' <returns>A valid, hashed string which only contains characters that are compatible to the local file system</returns>
        Public Shared Function ComputeHashedPathFromLongPath(ByVal path As String) As String
            Return FileSystemCompatibleString(GetEncryptedValue(path))
        End Function
        ''' <summary>
        '''     Convert a string with invalid characters of a suggested file name to a string with compatible naming for file systems
        ''' </summary>
        ''' <param name="filename">A filename without path information</param>
        ''' <returns>A string which can be used to create file and folder names</returns>
        Public Shared Function FileSystemCompatibleString(ByVal filename As String) As String
#If VS2015OrHigher = True Then
#Disable Warning BC40000 'disable obsolete warnings because this code must be compatible to .NET 1.1
#End If
            Dim Result As String = filename.Replace(System.IO.Path.PathSeparator, "_"c).Replace(System.IO.Path.DirectorySeparatorChar, "_"c).Replace(System.IO.Path.AltDirectorySeparatorChar, "_"c).Replace("+"c, "_"c).Replace("-"c, "_"c).Replace(System.IO.Path.VolumeSeparatorChar, "_"c).Replace(ControlChars.Back, " "c).Replace(ControlChars.Cr, " "c).Replace(ControlChars.FormFeed, " "c).Replace(ControlChars.Lf, " "c).Replace(ControlChars.NullChar, " "c).Replace(ControlChars.Tab, " "c).Replace(ControlChars.VerticalTab, " "c).Replace("*"c, "p"c).Replace(","c, "4"c).Replace("?"c, "w"c).Replace("<"c, "I"c).Replace(">"c, "D"c).Replace("|"c, "x"c).Replace(""""c, "p"c).Replace("="c, "t"c).Replace("@"c, "G"c).Replace("'"c, "_"c).Replace("`"c, "_"c).Replace("´"c, "_"c).Replace("^"c, "_"c).Replace("~"c, "_"c).Replace("+"c, "_"c).Replace("*"c, "_"c).Replace("#"c, "_"c).Replace("µ"c, "_"c).Replace("€"c, "_"c).Replace("°"c, "_"c)
            For Each MyChar As Char In System.IO.Path.InvalidPathChars 'usage of .GetInvalidFileNameChars available with .NET 2 or higher, obsolete .InvalidPathChars required for .NET 1.1 compatibility
                Result = Result.Replace(MyChar, "k"c)
            Next
            Return Result
#If VS2015OrHigher = True Then
#Enable Warning BC40000 'obsolete warnings
#End If
        End Function
        Private Shared Function ConvertBytesToBase64String(ByVal byteData As Byte()) As String
            Return System.Convert.ToBase64String(byteData)
        End Function
        Private Shared Function ConvertStringToBytes(ByVal text As String) As Byte()
            ' Declare a UTF8Encoding object so we may use the GetByte 
            ' method to transform the plainText into a Byte array. 
            Dim utf8encoder As New System.Text.UTF8Encoding
            Return utf8encoder.GetBytes(text)
        End Function
        ''' <summary>
        '''     Creates a download link without taking any actions
        ''' </summary>
        ''' <param name="downloadLocation">download location</param>
        ''' <param name="pathInDownloadLocation">path in download location</param>
        ''' <param name="fileName">Name of file</param>
        Public Function CreateDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String) As String
            Return CreateDownloadLink(downloadLocation, pathInDownloadLocation, Nothing, fileName)
        End Function
        ''' <summary>
        '''     Creates a download link without taking any actions
        ''' </summary>
        ''' <param name="downloadLocation">download location</param>
        ''' <param name="pathInDownloadLocation">path in download location</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="fileName"></param>
        ''' <remarks>
        ''' This function check for download location, if the value is "PublicCache", it returns a download link with query string parameter "file"
        ''' (=files virtual path). If download location is not "PublicCache", return download link with query string parameter "fileID".
        ''' 
        ''' if web application 
        ''' return "/system/download.aspx?fid=UniqueCrypticID"
        ''' else
        ''' return "%temp%\filepath"
        ''' </remarks>
        Public Function CreateDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal folderInVirtualDownloadLocation As String, ByVal fileName As String) As String
            Dim downloadFullPath As String = Me.DownloadFolderFullPath & Me.ResolveSubFolderFromDownloadLocation(downloadLocation) & "/" & pathInDownloadLocation
            downloadFullPath = downloadFullPath.Replace("//", "/").Replace("//", "/")
            downloadFullPath = downloadFullPath & "/" & folderInVirtualDownloadLocation & "/" & fileName
            Dim downloadVirtualPath As String = Me.GetDownloadFileVirtualPath(downloadFullPath)
            downloadVirtualPath = downloadVirtualPath.Replace("\"c, "/"c)
            downloadVirtualPath = downloadVirtualPath.Replace("//", "/").Replace("//", "/")

            Dim downloadLink As String = ""
            If Me.IsWebApplication Then
                If downloadLocation = DownloadLocations.PublicCache Then
                    'Create Record in DB in case file has timedout
                    Me.AddDownloadFileRecord(downloadFullPath, Me.ServerID, Me.TimeLimitForPublicCache)
                    'Create download link
                    downloadLink = "/system/download.aspx?fPath=" & downloadVirtualPath
                Else
                    'Create Record in DB in case file has timedout
                    Me.AddDownloadFileRecord(downloadFullPath, Me.ServerID, Me.GeneralTimeLimitForFiles)
                    'Create download link
                    downloadLink = "/system/download.aspx?fid=" & Me.GetUniqueCrypticID(downloadVirtualPath)
                End If
            Else
                downloadLink = downloadFullPath
                downloadLink = downloadLink.Replace("/"c, System.IO.Path.DirectorySeparatorChar)
            End If
            Return downloadLink
        End Function
        ''' <summary>
        '''     Creates a download link without taking any actions
        ''' </summary>
        ''' <param name="downloadLocation">download location</param>
        ''' <param name="pathInDownloadLocation">path in download location</param>
        ''' <param name="fileName">Name of file</param>
        Public Function CreatePlainDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String) As String
            Return CreatePlainDownloadLink(downloadLocation, pathInDownloadLocation, Nothing, fileName)
        End Function
        ''' <summary>
        '''     Creates a download link without taking any actions
        ''' </summary>
        ''' <param name="downloadLocation">download location</param>
        ''' <param name="pathInDownloadLocation">path in download location</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="fileName"></param>
        ''' <remarks>
        ''' This function check for download location, if the value is "PublicCache", it returns a download link with query string parameter "file"
        ''' (=files virtual path). If download location is not "PublicCache", return download link with query string parameter "fileID".
        ''' 
        ''' if web application 
        ''' return "/system/download.aspx?fid=UniqueCrypticID"
        ''' else
        ''' return "%temp%\filepath"
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	13.01.2005	Created
        '''     [zeutzheim] 01.02.2010 Modified
        ''' </history>
        Public Function CreatePlainDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal folderInVirtualDownloadLocation As String, ByVal fileName As String) As String
            Dim downloadFullPath As String = Me.DownloadFolderFullPath & Me.ResolveSubFolderFromDownloadLocation(downloadLocation) & "/" & pathInDownloadLocation
            downloadFullPath = downloadFullPath.Replace("//", "/"c)
            downloadFullPath = downloadFullPath.Replace("//", "/"c)
            downloadFullPath = downloadFullPath & "/" & folderInVirtualDownloadLocation & "/" & fileName
            Dim downloadVirtualPath As String = Me.GetDownloadFileVirtualPath(downloadFullPath)
            downloadVirtualPath = downloadVirtualPath.Replace("//", "/"c)
            downloadVirtualPath = downloadVirtualPath.Replace("//", "/"c)
            Dim downloadLink As String = ""
            If Me.IsWebApplication Then
                If downloadLocation = DownloadLocations.PublicCache Then
                    'Create Record in DB in case file has timedout
                    Me.AddDownloadFileRecord(downloadFullPath, Me.ServerID, Me.TimeLimitForPublicCache)
                    'Create download link
                    'downloadLink = "/system/download.aspx?fPath=" & downloadVirtualPath
                    downloadLink = Me.DownloadFolderFullPath & downloadVirtualPath
                Else
                    'Create Record in DB in case file has timedout
                    Me.AddDownloadFileRecord(downloadFullPath, Me.ServerID, Me.GeneralTimeLimitForFiles)
                    'Create download link
                    downloadLink = Me.GetFileFullPath(Me.GetUniqueCrypticID(downloadVirtualPath))
                    'downloadLink = "/system/download.aspx?fid=" & Me.GetUniqueCrypticID(downloadVirtualPath)
                End If
            Else
                downloadLink = downloadFullPath
                downloadLink = downloadLink.Replace("/"c, System.IO.Path.DirectorySeparatorChar)
            End If
            downloadLink = Me.GetPathFromSystemFolder(downloadLink)
            Return downloadLink
        End Function

#End Region

#Region "DownloadCollection"
        ''' <summary>
        '''         Download Collection contains each element of type "FileData". Which is added to it by "Add" methods.
        ''' </summary>
        Public Structure FileData
            Dim Type As FileDataType
            ''' <summary>
            '''     File system full path
            ''' </summary>
            Dim FilePath As String
            Dim FolderVirtualPath As String
            Dim NewFileName As String
            Dim FileInfo As System.IO.FileInfo
            Dim DirectoryInfo As System.IO.DirectoryInfo

            Dim RawDataFile As RawDataSingleFile
            Dim RawDataCollection As RawDataCollectionMember
        End Structure
        ''' <summary>
        '''         This structure is used to identify the file type in "FileData" in the Download collection.
        ''' </summary>
        Public Enum FileDataType As Byte
            FilePath = 0
            FileInfo = 1
            Directory = 2
            RawFile = 3
            RawCollection = 4
        End Enum
        ''' <summary>
        '''     This Add method is used to add single file in Download collection.
        ''' </summary>
        ''' <param name="pathOnDisc">Describes the full path of file to be added in collection. e.g. "d:\temp\MyFolder"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        Public Sub Add(ByVal pathOnDisc As String, ByVal folderInVirtualDownloadLocation As String)
            Me.Add(pathOnDisc, folderInVirtualDownloadLocation, Nothing)
        End Sub
        ''' <summary>
        '''     This Add method is used to add single file in Download collection.
        ''' </summary>
        ''' <param name="pathOnDisc">Describes the full path of file to be added in collection. e.g. "d:\temp\MyFolder"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="newFilename">Describes the new name for the file added in Download collection.</param>
        ''' <remarks>
        '''     Following special strings in the file's new name will be converted into underscores ("_") because of security reasons:
        '''     <list>
        '''         <item>{</item>
        '''         <item>}</item>
        '''         <item>..</item>
        '''     </list>
        ''' </remarks>
        Public Sub Add(ByVal pathOnDisc As String, ByVal folderInVirtualDownloadLocation As String, ByVal newFilename As String)
            Dim fData As New FileData
            fData.Type = FileDataType.FilePath
            fData.FilePath = pathOnDisc
            fData.FolderVirtualPath = folderInVirtualDownloadLocation
            If newFilename <> "" Then
                fData.NewFileName = DownloadHandler.FileSystemCompatibleString(newFilename)
            Else
                fData.NewFileName = newFilename
            End If
            Me.InnerList.Add(fData)
        End Sub
        ''' <summary>
        '''     This Add method is used to add single file in Download collection.
        ''' </summary>
        ''' <param name="file">A temporary file</param>
        Public Sub Add(ByVal file As SingleFileInDownloadLocation)
            If System.IO.File.Exists(file.PhysicalPath) = False Then Throw New FileNotFoundException("SingleFileInDownloadLocation doesn't exist on disc", file.PhysicalPath)
            Me.Add(New System.IO.FileInfo(file.PhysicalPath), file.DirectoryName, Nothing)
        End Sub
        ''' <summary>
        '''     This Add method is used to add single file in Download collection.
        ''' </summary>
        ''' <param name="fileInfo">Describes the file to be added in Download collection. fileInfo is of type "System.IO.FileInfo"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        Public Sub Add(ByVal fileInfo As System.IO.FileInfo, ByVal folderInVirtualDownloadLocation As String)
            Me.Add(fileInfo, folderInVirtualDownloadLocation, Nothing)
        End Sub
        ''' <summary>
        '''     This Add method is used to add single file in Download collection.
        ''' </summary>
        ''' <param name="fileInfo">Describes the file to be added in Download collection. fileInfo is of type "System.IO.FileInfo"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="newFilename">Describes the new name for the file added in Download collection.</param>
        ''' <remarks>
        '''     Following special strings in the file's new name will be converted into underscores ("_") because of security reasons:
        '''     <list>
        '''         <item>{</item>
        '''         <item>}</item>
        '''         <item>..</item>
        '''     </list>
        ''' </remarks>
        Public Sub Add(ByVal fileInfo As System.IO.FileInfo, ByVal folderInVirtualDownloadLocation As String, ByVal newFilename As String)
            Dim fData As New FileData
            fData.Type = FileDataType.FileInfo
            fData.FileInfo = fileInfo
            fData.FolderVirtualPath = folderInVirtualDownloadLocation
            If newFilename <> "" Then
                fData.NewFileName = DownloadHandler.FileSystemCompatibleString(newFilename)
            Else
                fData.NewFileName = newFilename
            End If
            Me.InnerList.Add(fData)
        End Sub
        ''' <summary>
        '''     This Add method is used to add directory in Download collection.
        ''' </summary>
        ''' <param name="directoryInfo">Describes the directory to be added in Download collection. directoryInfo is of type "System.IO.DirectoryInfo"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <remarks>
        '''     The directory is archived into zip format during download process.
        ''' </remarks>
        Public Sub Add(ByVal directoryInfo As System.IO.DirectoryInfo, ByVal folderInVirtualDownloadLocation As String)
            Dim fData As New FileData
            fData.Type = FileDataType.Directory
            fData.DirectoryInfo = directoryInfo
            fData.FolderVirtualPath = folderInVirtualDownloadLocation
            Me.InnerList.Add(fData)
        End Sub

        ''' <summary>
        '''     This raw file data can be sent to the browser in all circumstances
        ''' </summary>
        ''' <remarks>
        '''     This structure describes the one of the type of "FileData". Please see the description for structure "FileData"
        ''' </remarks>
        Public Structure RawDataSingleFile
            Dim Filename As String
            Dim MimeType As String
            Dim Data As Byte()
        End Structure
        ''' <summary>
        '''     This file data must be added into a ZIP download or can be sent to the browser in fully featured mode. It cannot get transferred as a single, uncompressed file if the fully featured mode is not available. 
        ''' </summary>
        ''' <remarks>
        '''     This structure describes the one of the type of "FileData". Please see the description for structure "FileData"
        ''' </remarks>
        Public Structure RawDataCollectionMember
            Dim Filename As String
            Dim Data As Byte()
        End Structure
        ''' <summary>
        '''     This Add method is used to add file of type "RawDataSingleFile" in Download collection.
        ''' </summary>
        ''' <param name="file">This parameter is of type "RawDataSingleFile"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        Public Sub Add(ByVal file As RawDataSingleFile, ByVal folderInVirtualDownloadLocation As String)
            Me.Add(file, folderInVirtualDownloadLocation, Nothing)
        End Sub
        ''' <summary>
        '''     This Add method is used to add file of type "RawDataSingleFile" in Download collection.
        ''' </summary>
        ''' <param name="file">This parameter is of type "RawDataSingleFile"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="newFilename">Describes the new name for the file added in Download collection.</param>
        ''' <remarks>
        '''     Following special strings in the file's new name will be converted into underscores ("_") because of security reasons:
        '''     <list>
        '''         <item>{</item>
        '''         <item>}</item>
        '''         <item>..</item>
        '''     </list>
        ''' </remarks>
        Public Sub Add(ByVal file As RawDataSingleFile, ByVal folderInVirtualDownloadLocation As String, ByVal newFilename As String)
            Dim fData As New FileData
            fData.Type = FileDataType.RawFile
            fData.RawDataFile = file
            fData.FolderVirtualPath = folderInVirtualDownloadLocation
            If newFilename <> "" Then
                fData.NewFileName = DownloadHandler.FileSystemCompatibleString(newFilename)
            End If
            Me.InnerList.Add(fData)
        End Sub
        ''' <summary>
        '''     This Add method is used to add file of type "RawDataCollectionMemeber" in Download collection.
        ''' </summary>
        ''' <param name="file">This parameter is of type "RawDataCollectionMember"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        Public Sub Add(ByVal file As RawDataCollectionMember, ByVal folderInVirtualDownloadLocation As String)
            Me.Add(file, folderInVirtualDownloadLocation, Nothing)
        End Sub
        ''' <summary>
        '''     This Add method is used to add file of type "RawDataCollectionMember" in Download collection.
        ''' </summary>
        ''' <param name="file">This parameter is of type "RawDataCollectionMember"</param>
        ''' <param name="folderInVirtualDownloadLocation">Describes the virtual path to be added in download location. e.g. "foo/machines/images"</param>
        ''' <param name="newFilename">Describes the new name for the file added in Download collection.</param>
        ''' <remarks>
        '''     Following special strings in the file's new name will be converted into underscores ("_") because of security reasons:
        '''     <list>
        '''         <item>{</item>
        '''         <item>}</item>
        '''         <item>..</item>
        '''     </list>
        ''' </remarks>
        Public Sub Add(ByVal file As RawDataCollectionMember, ByVal folderInVirtualDownloadLocation As String, ByVal newFilename As String)
            Dim fData As New FileData
            fData.Type = FileDataType.RawCollection
            fData.RawDataCollection = file
            fData.FolderVirtualPath = folderInVirtualDownloadLocation
            If newFilename <> "" Then
                fData.NewFileName = DownloadHandler.FileSystemCompatibleString(newFilename)
            Else
                fData.NewFileName = newFilename
            End If
            Me.InnerList.Add(fData)
        End Sub

#End Region

#Region "ProcessDownload"

        Private _WebManager As WMSystem

        Public Sub New(ByVal webManager As WMSystem)
            _WebManager = webManager
            Me.CheckExistanceOfTempDownloadFolder()
        End Sub
        ''' <summary>
        '''          The download location is defined by this enumeration.
        ''' </summary>
        ''' <remarks>
        '''     
        ''' </remarks>
        Public Enum DownloadLocations As Byte
            WebServerSession = 1
            WebManagerUserSession = 2
            WebManagerSecurityObjectName = 3
            PublicCache = 4
        End Enum
        ''' <summary>
        ''' Create a temporary file path (not a file!) which is managed by Download-Handler
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="fileName">The file name which shall be created</param>
        ''' <returns>The virtual path of the temp file (which can be mapped to a physical file name in a 2nd step)</returns>
        ''' <remarks>
        ''' Download-Handler will automatically remove the file as soon as the regular timeout for the file has been reached.
        ''' </remarks>
        <Obsolete("Never implemented", True), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
        Public Function GetTempFileVirtualPath(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String) As String
            'TODO: implementation of DownloadHandler.GetTempFileName
            Throw New NotImplementedException
            '1. Create a plain download link
            '2. register the file in download-handler
        End Function

        ''' <summary>
        ''' Create a temporary file path (not a file!) that can be delivered to client with download handler
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation</param>
        ''' <param name="pathInDownloadLocation">This parameter defines the path in download location.</param>
        ''' <param name="fileName">The file name which shall be created</param>
        ''' <returns>The virtual path of the temp file (which can be mapped to a physical file name in a 2nd step)</returns>
        Public Function GetTempFile(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String) As SingleFileInDownloadLocation
            If Me.IsFullyFeatured = True Then
                Dim downloadFullPath As String = Me.DownloadFolderFullPath & Me.ResolveSubFolderFromDownloadLocation(downloadLocation) & "/" & pathInDownloadLocation
                downloadFullPath = downloadFullPath.Replace("//", "/").Replace("//", "/")
                downloadFullPath = downloadFullPath & "/" & fileName
                Dim physicalDirectoryPath As String = System.IO.Path.GetDirectoryName(downloadFullPath)
                If System.IO.Directory.Exists(physicalDirectoryPath) = False Then System.IO.Directory.CreateDirectory(physicalDirectoryPath)
                Dim downloadVirtualPath As String = Me.GetDownloadFileVirtualPath(downloadFullPath)
                downloadVirtualPath = downloadVirtualPath.Replace("\"c, "/"c)
                downloadVirtualPath = downloadVirtualPath.Replace("//", "/").Replace("//", "/")
                Dim Result As New SingleFileInDownloadLocation(ResolveSubFolderFromDownloadLocation(downloadLocation), downloadLocation, pathInDownloadLocation, fileName, Me.IsWebApplication, downloadFullPath, downloadVirtualPath, Me.SystemDownloadFolderForTemporaryFiles & EncryptedValueCurrentServerIdentString & "/" & downloadVirtualPath)
                Return Result
            Else
                Throw New DownloadHandlerNotSupportedException()
            End If
        End Function

        ''' <summary>
        ''' A reference to a single file in download handler area on disc
        ''' </summary>
        ''' <remarks></remarks>
        Public Class SingleFileInDownloadLocation
            ''' <summary>
            ''' Create a new instance of SingleFileInDownloadLocation
            ''' </summary>
            ''' <param name="downloadLocation">Defines the standard downloadLocation</param>
            ''' <param name="pathInDownloadLocation">This parameter defines the path in download location.</param>
            ''' <param name="fileName">The file name which shall be created (for security reasons, some special chars like { and } will be replaced by _ char</param>
            ''' <remarks></remarks>
            Friend Sub New(ByVal downloadLocationResolved As String, ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String, isWebApplication As Boolean, physicalPath As String, virtualPath As String, virtualPathFromWebRoot As String)
                Me._DownLoadLocationResolved = downloadLocationResolved
                Me._DownloadLocation = downloadLocation
                Me._DirectoryName = pathInDownloadLocation
                If fileName <> "" Then
                    Me._FileName = DownloadHandler.FileSystemCompatibleString(fileName)
                Else
                    Me._FileName = fileName
                End If
                Me._IsWebApplication = isWebApplication
                Me._PhysicalPath = physicalPath
                Me._VirtualPath = virtualPath
                Me._VirtualPathFromWebRoot = virtualPathFromWebRoot
            End Sub
            Private _VirtualPathFromWebRoot As String
            Private _PhysicalPath As String
            Private _VirtualPath As String
            Private _IsWebApplication As Boolean = True
            Private _DownloadLocation As DownloadLocations
            Private _DownLoadLocationResolved As String = Nothing
            Private _DirectoryName As String = Nothing
            Private _FileName As String = Nothing
            Public ReadOnly Property DownloadLocation As DownloadLocations
                Get
                    Return _DownloadLocation
                End Get
            End Property
            Public ReadOnly Property DirectoryName As String
                Get
                    Return _DirectoryName
                End Get
            End Property
            Public ReadOnly Property FileName As String
                Get
                    Return _FileName
                End Get
            End Property
            Public ReadOnly Property VirtualPath() As String
                Get
                    If Me._IsWebApplication = False Then
                        Throw New Exception("VirtualPath is only available for web environments")
                    Else
                        Return _VirtualPath
                    End If
                End Get
            End Property
            Public ReadOnly Property VirtualPathFromWebRoot() As String
                Get
                    If Me._IsWebApplication = False Then
                        Throw New Exception("VirtualPathFromWebRoot is only available for web environments")
                    Else
                        Return _VirtualPathFromWebRoot
                    End If
                End Get
            End Property
            Public ReadOnly Property PhysicalPath() As String
                Get
                    Return _PhysicalPath
                End Get
            End Property
        End Class
        ''' <summary>
        '''     This function used to get download location in string format. 
        ''' </summary>
        ''' <param name="downloadLocation">downloadLocation is of type "DownloadLocations"</param>
        ''' <returns>Returns download location in string format.</returns>
        ''' <remarks>
        '''     The download handler creates the download file at download location returned by this function.
        ''' </remarks>
        Private Function ResolveSubFolderFromDownloadLocation(ByVal downloadLocation As DownloadLocations) As String
            Select Case downloadLocation
                Case DownloadLocations.PublicCache
                    Return "cache"
                Case DownloadLocations.WebServerSession
                    If Me.IsWebApplication Then
                        Return DownloadHandler.GetEncryptedValue(Me._WebManager.CurrentScriptEngineSessionID)
                    Else
                        Throw New Exception("Download handler doesn't support this webserver session cache mode for non-web-applications.")
                    End If
                Case DownloadLocations.WebManagerSecurityObjectName
                    Dim Result As String
                    If Me._WebManager.SecurityObject = Nothing Then
                        Throw New EmptySecurityObjectException
                    Else
                        Result = DownloadHandler.GetEncryptedValue(_WebManager.SecurityObject)
                    End If
                    Return Result
                Case DownloadLocations.WebManagerUserSession
                    Dim CurUserIDString As String = _WebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous).ToString("00000000000000000")
                    'Dim CurUserIDByteStream As Byte()
                    'CurUserIDByteStream = System.Text.Encoding.UTF8.GetBytes(CurUserIDString)
                    'Dim Md5 As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create
                    'Dim Hash As Byte() = Md5.ComputeHash(CurUserIDByteStream)
                    Dim Result As String '= Convert.ToString(Hash)
                    Result = DownloadHandler.GetEncryptedValue(CurUserIDString)
                    Return Result
                Case Else
                    Throw New ArgumentOutOfRangeException("downloadLocation", "Invalid value")
            End Select
        End Function
        ''' <summary>
        '''     This method processes the download collection. 
        '''     And immediately sends file to client browser.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <remarks>
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        '''     This method throws exception, if occured.
        ''' </remarks>
        Public Sub ProcessDownload(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String)
            Me.ProcessDownload(downloadLocation, pathInDownloadLocation, False, New TimeSpan(0), False, "", False)
        End Sub
        ''' <summary>
        '''     This method processes the download collection. 
        '''     And immediately sends file to client browser.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        Public Sub ProcessDownload(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean)
            Me.ProcessDownload(downloadLocation, pathInDownloadLocation, overwrite, New TimeSpan(0), False, "", False)
        End Sub
        ''' <summary>
        '''     This method processes the download collection. 
        '''     And immediately sends file to client browser.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time for download file to be available for download</param>
        Public Sub ProcessDownload(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan)
            Me.ProcessDownload(downloadLocation, pathInDownloadLocation, overwrite, liveTime, False, "", False)
        End Sub
        ''' <summary>
        '''     This method processes the download collection. 
        '''     And immediately sends file to client browser.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time for download file to be available for download.</param>
        ''' <param name="zipFilesFirst">If true then download handler enforces the download collection in zip archive even it it is a single file.</param>
        ''' <param name="zipArchiveName">Defines the zip archive name. Otherwise default zip archive name is created by Download handler.</param>
        ''' <remarks>
        '''     This method throws exception, if occured.
        ''' </remarks>
        Public Sub ProcessDownload(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan, ByVal zipFilesFirst As Boolean, ByVal zipArchiveName As String)
            Me.ProcessDownload(downloadLocation, pathInDownloadLocation, overwrite, liveTime, zipFilesFirst, zipArchiveName, False)
        End Sub
        ''' <summary>
        '''     This method processes the download collection. 
        '''     And immediately sends file to client browser.
        ''' </summary>
        Public Sub ProcessDownload(ByVal file As SingleFileInDownloadLocation)
            If Not IsWebApplication Then
                Throw New Exception("ProcessDownload is not possible in non-web applications, use ProcessAndGetDownloadLink instead")
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Begin")
            End If
#End If

            'Define file timeout 
            Dim ts As TimeSpan
            If file.DownloadLocation = DownloadLocations.PublicCache Then
                ts = Me.TimeLimitForPublicCache
            Else
                ts = Me.GeneralTimeLimitForFiles
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Ready4Transmission")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Ready4Transmission")
            End If
#End If
            'Send data
            If Not Me.IsFullyFeatured AndAlso Me.IsWebApplication Then
                'a not-fully-featured download --> SendToCurrentRequest
                Me.SendDownloadFileToCurrentRequest(file.PhysicalPath)
            Else
                'a fully-featured download --> SendToSeparateRequest
                Me.AddDownloadFileRecord(file.PhysicalPath, Me.ServerID, ts)
                If Me.InnerList.Count = 1 AndAlso GetMimeType(System.IO.Path.GetExtension(file.FileName)) <> "" Then
                    Me.RedirectFileToBrowser(file.PhysicalPath, GetMimeType(System.IO.Path.GetExtension(file.FileName)))
                Else
                    Me.RedirectFileToBrowser(file.PhysicalPath)
                End If
            End If
        End Sub
        ''' <summary>
        '''     This method processes the download collection. 
        '''     And immediately sends file to client browser.
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time for download file to be available for download.</param>
        ''' <param name="zipFilesFirst">If true then download handler enforces the download collection in zip archive even it it is a single file.</param>
        ''' <param name="zipArchiveName">Defines the zip archive name. Otherwise default zip archive name is created by Download handler.</param>
        ''' <param name="enforceDownload">If true, then download handler enforces client browser to download file.</param>
        Public Sub ProcessDownload(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan, ByVal zipFilesFirst As Boolean, ByVal zipArchiveName As String, ByVal enforceDownload As Boolean)
            If Not IsWebApplication Then
                Throw New Exception("ProcessDownload is not possible in non-web applications, use ProcessAndGetDownloadLink instead")
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Begin")
            End If
#End If

            'Define file timeout 
            Dim ts As TimeSpan
            If downloadLocation = DownloadLocations.PublicCache Then
                ts = Me.TimeLimitForPublicCache
            Else
                ts = Me.GeneralTimeLimitForFiles
            End If
            If TimeSpan.Compare(liveTime, New TimeSpan(0)) > 0 Then
                ts = liveTime
            End If

            Dim dataToProcess As DataToProcessDownload = Me.GetDataToRecordAndProcess(downloadLocation, pathInDownloadLocation, overwrite, zipFilesFirst, zipArchiveName, enforceDownload)

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Ready4Transmission")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Ready4Transmission")
            End If
#End If
            'Send data
            If dataToProcess.SendToCurrentRequest Then
                If dataToProcess.PathToRecordAndProcess = "" Then
                    Me.SendDownloadFileToCurrentRequest(dataToProcess.RawFile)
                Else
                    Me.SendDownloadFileToCurrentRequest(dataToProcess.PathToRecordAndProcess)
                End If
            Else
                If downloadLocation = DownloadLocations.PublicCache Then
                    If dataToProcess.TargetFileExists Then
                        'if file already exists and we are in cache location and we have to overwrite the file, then update the removal time
                        'Me.UpdateDownloadFileRecord(Me.GetDownloadFileVirtualPath(dataToProcess.PathToRecordAndProcess), ts)
                    Else
                        Me.AddDownloadFileRecord(dataToProcess.PathToRecordAndProcess, Me.ServerID, ts)
                    End If
                Else
                    Me.AddDownloadFileRecord(dataToProcess.PathToRecordAndProcess, Me.ServerID, ts)
                End If

                If Me.InnerList.Count = 1 AndAlso dataToProcess.MimeType <> "" Then
                    Me.RedirectFileToBrowser(dataToProcess.PathToRecordAndProcess, dataToProcess.MimeType)
                Else
                    Me.RedirectFileToBrowser(dataToProcess.PathToRecordAndProcess)
                End If
            End If
        End Sub
        ''' <summary>
        '''     This method processes the download collection and returns the download link
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <returns>Returns the download link.</returns>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String) As String
            Return Me.ProcessAndGetDownloadLink(downloadLocation, pathInDownloadLocation, False, Nothing, False, "")
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean) As String
            Return Me.ProcessAndGetDownloadLink(downloadLocation, pathInDownloadLocation, overwrite, Nothing, False, "")
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time, the download file to be available for download.</param>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan) As String
            Return Me.ProcessAndGetDownloadLink(downloadLocation, pathInDownloadLocation, overwrite, liveTime, False, "")
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time, the download file to be available for download.</param>
        ''' <param name="alwaysZipFiles">If true then download handler enforces the download collection in zip archive even it it is a single file.</param>
        ''' <param name="zipArchiveName">Defines the zip archive name. Otherwise default zip archive name is created by Download handler.</param>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan, ByVal alwaysZipFiles As Boolean, ByVal zipArchiveName As String) As String
            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessAndGetDownloadLink Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessAndGetDownloadLink Begin")
            End If
#End If

            If Me.IsFullyFeatured = False Then
                Return Me.ProcessSeparateRequestAndGetDownloadLink(pathInDownloadLocation, alwaysZipFiles)
                'Throw New DownloadHandlerNotSupportedException
            End If

            Dim ts As TimeSpan
            If downloadLocation = DownloadLocations.PublicCache Then
                ts = Me.TimeLimitForPublicCache
            Else
                ts = Me.GeneralTimeLimitForFiles
            End If
            If TimeSpan.Compare(liveTime, Nothing) > 0 Then
                ts = liveTime
            End If

            Dim dataToProcess As DataToProcessDownload
            dataToProcess = Me.GetDataToRecordAndProcess(downloadLocation, pathInDownloadLocation, overwrite, alwaysZipFiles, zipArchiveName, False)

            Dim downloadPath As String = ""

            If IsWebApplication Then
                If dataToProcess.SendToCurrentRequest Then
                    If dataToProcess.PathToRecordAndProcess = "" Then
                        Me.SendDownloadFileToCurrentRequest(dataToProcess.RawFile)
                    Else
                        Me.SendDownloadFileToCurrentRequest(dataToProcess.PathToRecordAndProcess)
                    End If
                Else
                    If downloadLocation = DownloadLocations.PublicCache Then
                        If dataToProcess.TargetFileExists Then
                            'if file already exists and we are in public cache location and we have to overwrite the file, then update the removal time
                            'Me.UpdateDownloadFileRecord(Me.GetDownloadFileVirtualPath(dataToProcess.PathToRecordAndProcess), ts)
                        Else
                            Me.AddDownloadFileRecord(dataToProcess.PathToRecordAndProcess, Me.ServerID, ts)
                        End If
                        downloadPath = "/system/download.aspx?fPath=" & Me.GetDownloadFileVirtualPath(dataToProcess.PathToRecordAndProcess)
                    Else
                        downloadPath = "/system/download.aspx?fid=" & Me.AddDownloadFileRecord(dataToProcess.PathToRecordAndProcess, Me.ServerID, ts)
                    End If
                End If

            Else
                downloadPath = dataToProcess.PathToRecordAndProcess
                downloadPath = downloadPath.Replace("/"c, System.IO.Path.DirectorySeparatorChar)
                downloadPath = downloadPath.Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar).Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessAndGetDownloadLink End")
            Else
                HttpContext.Current.Trace.Write("ProcessAndGetDownloadLink End")
            End If
#End If

            Return downloadPath
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link
        ''' </summary>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        ''' </remarks>
        Public Function ProcessAndGetDownloadLink(ByVal file As SingleFileInDownloadLocation) As String
            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessAndGetDownloadLink Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessAndGetDownloadLink Begin")
            End If
#End If

            If Me.IsFullyFeatured = False Then
                Return Me.ProcessSeparateRequestAndGetDownloadLink(file)
                'Throw New DownloadHandlerNotSupportedException
            End If

            Dim ts As TimeSpan
            If file.DownloadLocation = DownloadLocations.PublicCache Then
                ts = Me.TimeLimitForPublicCache
            Else
                ts = Me.GeneralTimeLimitForFiles
            End If

            Dim downloadPath As String = ""

            If IsWebApplication Then
                If Not Me.IsFullyFeatured AndAlso Me.IsWebApplication Then
                    'a not-fully-featured download --> SendToCurrentRequest
                    Me.SendDownloadFileToCurrentRequest(file.PhysicalPath)
                Else
                    'a fully-featured download --> SendToSeparateRequest
                    If file.DownloadLocation = DownloadLocations.PublicCache Then
                        Me.AddDownloadFileRecord(file.PhysicalPath, Me.ServerID, ts)
                        downloadPath = "/system/download.aspx?fPath=" & Me.GetDownloadFileVirtualPath(file.PhysicalPath)
                    Else
                        downloadPath = "/system/download.aspx?fid=" & Me.AddDownloadFileRecord(file.PhysicalPath, Me.ServerID, ts)
                    End If
                End If

            Else
                downloadPath = file.PhysicalPath
                downloadPath = downloadPath.Replace("/"c, System.IO.Path.DirectorySeparatorChar)
                downloadPath = downloadPath.Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar).Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessAndGetDownloadLink End")
            Else
                HttpContext.Current.Trace.Write("ProcessAndGetDownloadLink End")
            End If
#End If

            Return downloadPath
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link as direct target to the file
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <returns>Returns the download link.</returns>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetPlainDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String) As String
            Return Me.ProcessAndGetPlainDownloadLink(downloadLocation, pathInDownloadLocation, False, Nothing, False, "")
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link as direct target to the file
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetPlainDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean) As String
            Return Me.ProcessAndGetPlainDownloadLink(downloadLocation, pathInDownloadLocation, overwrite, Nothing, False, "")
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link as direct target to the file
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This parameter defines the path in download location. 
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time, the download file to be available for download.</param>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetPlainDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan) As String
            Return Me.ProcessAndGetPlainDownloadLink(downloadLocation, pathInDownloadLocation, overwrite, liveTime, False, "")
        End Function
        ''' <summary>
        '''     This method processes the download collection and returns the download link as direct target to the file
        ''' </summary>
        ''' <param name="downloadLocation">Defines the standard downloadLocation.</param>
        ''' <param name="pathInDownloadLocation">
        '''     This method processes the download collection and returns the download link as direct target to the file
        ''' </param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="liveTime">Defines time, the download file to be available for download.</param>
        ''' <param name="zipFilesFirst">If true then download handler enforces the download collection in zip archive even it it is a single file.</param>
        ''' <param name="zipArchiveName">Defines the zip archive name. Otherwise default zip archive name is created by Download handler.</param>
        ''' <remarks>
        '''     <p>This method throws exception, if occured.</p>
        '''     If this is not the WebApplication, then Download handler creates download file in "%temp%"
        '''     folder and returns complete path.
        ''' 
        '''     The download handler creates processed download file at location defined by parameter "downloadLocation".
        ''' </remarks>
        Public Function ProcessAndGetPlainDownloadLink(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal liveTime As TimeSpan, ByVal zipFilesFirst As Boolean, ByVal zipArchiveName As String) As String

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessAndGetPlainDownloadLink Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessAndGetPlainDownloadLink Begin")
            End If
#End If

            If Me.IsFullyFeatured = False Then
                Throw New DownloadHandlerNotSupportedException
            End If

            Dim ts As TimeSpan
            If downloadLocation = DownloadLocations.PublicCache Then
                ts = Me.TimeLimitForPublicCache
            Else
                ts = Me.GeneralTimeLimitForFiles
            End If
            If TimeSpan.Compare(liveTime, Nothing) > 0 Then
                ts = liveTime
            End If

            Dim dataToProcess As DataToProcessDownload
            dataToProcess = Me.GetDataToRecordAndProcess(downloadLocation, pathInDownloadLocation, overwrite, zipFilesFirst, zipArchiveName, False)

            Dim downloadPath As String = ""

            If IsWebApplication Then
                If dataToProcess.SendToCurrentRequest Then
                    If dataToProcess.PathToRecordAndProcess = "" Then
                        Me.SendDownloadFileToCurrentRequest(dataToProcess.RawFile)
                    Else
                        Me.SendDownloadFileToCurrentRequest(dataToProcess.PathToRecordAndProcess)
                    End If
                Else
                    If downloadLocation = DownloadLocations.PublicCache Then
                        If Not dataToProcess.TargetFileExists Then
                            'if file already exists and we are in public cache location and we have to overwrite the file, then update the removal time
                            'Me.UpdateDownloadFileRecord(Me.GetDownloadFileVirtualPath(dataToProcess.PathToRecordAndProcess), ts)
                        Else
                            Me.AddDownloadFileRecord(dataToProcess.PathToRecordAndProcess, Me.ServerID, ts)
                        End If

                        downloadPath = Me.GetPathFromSystemFolder(Me.DownloadFolderFullPath & Me.GetDownloadFileVirtualPath(dataToProcess.PathToRecordAndProcess))
                    Else
                        downloadPath = Me.GetPathFromSystemFolder(Me.GetFileFullPath(Me.AddDownloadFileRecord(dataToProcess.PathToRecordAndProcess, Me.ServerID, ts)))
                    End If
                End If

            Else
                downloadPath = dataToProcess.PathToRecordAndProcess
                downloadPath = downloadPath.Replace("/"c, System.IO.Path.DirectorySeparatorChar)
                downloadPath = downloadPath.Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar).Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessAndGetPlainDownloadLink End")
            Else
                HttpContext.Current.Trace.Write("ProcessAndGetPlainDownloadLink End")
            End If
#End If

            Return downloadPath
        End Function
        ''' <summary>
        '''     Processes download and returns data to record in structure type DataToProcessDownload.
        ''' </summary>
        ''' <param name="downloadLocation">download location</param>
        ''' <param name="pathInDownloadLocation">path in download location</param>
        ''' <param name="overwrite">If true overwrites the file in download location</param>
        ''' <param name="zipFilesFirst">If true then download handler enforces the download collection in zip archive even it it is a single file.</param>
        ''' <param name="zipArchiveName">Defines the zip archive name. Otherwise default zip archive name is created by Download handler.</param>
        ''' <param name="enforceDownload">If true, then download handler enforces client browser to download file.</param>
        Private Function GetDataToRecordAndProcess(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal overwrite As Boolean, ByVal zipFilesFirst As Boolean, ByVal zipArchiveName As String, ByVal enforceDownload As Boolean) As DataToProcessDownload

            'check if download size is in the limits
            If Me.MaxDownloadCollectionSize < Me.GetDownloadCollectionSize Then
                If Me._WebManager.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError Then
                    Throw New DownloadHandlerCollectionSizeLimitException(Me.MaxDownloadCollectionSize, Me.GetDownloadCollectionSize)
                Else
                    Throw New DownloadHandlerCollectionSizeLimitException
                End If
            End If

            'TODO: BUG: parameter enforceDownload must be considered first --> requires to send the SingleFile or FileCollection in-request

            If Not Me.IsFullyFeatured AndAlso Me.IsWebApplication Then
                'a not-fully-featured download
                '============================= _
                If Me.InnerList.Count = 1 Then
                    Dim fData As FileData = CType(Me.InnerList.Item(0), FileData)
                    Dim fPath As String = ""
                    Dim retData As New DataToProcessDownload
                    Select Case fData.Type
                        Case FileDataType.FilePath
                            fPath = Utils.CombineUnixPaths(fData.FolderVirtualPath, System.IO.Path.GetFileName(fData.FilePath))
                            fPath = HttpContext.Current.Server.MapPath(fPath)

                            retData.PathToRecordAndProcess = fPath
                        Case FileDataType.FileInfo
                            fPath = Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.FileInfo.Name)
                            fPath = HttpContext.Current.Server.MapPath(fPath)

                            retData.PathToRecordAndProcess = fPath
                        Case FileDataType.RawFile
                            retData.RawFile = fData.RawDataFile
                            'This is raw file
                            fPath = "" 'CombineUnixPaths(fdata.FolderVirtualPath, fdata.RawDataFile.Filename)
                            retData.PathToRecordAndProcess = fPath
                        Case Else
                            Throw New DownloadHandlerNotSupportedException
                    End Select

                    retData.SendToCurrentRequest = True
                    retData.TargetFileExists = False
                    retData.MimeType = Nothing
                    Return retData
                Else
                    Throw New DownloadHandlerNotSupportedException
                End If
            Else
                'a fully-featured download
                '========================= _

                'retrieve the full path (path only, without filename)
                Dim downloadFullPath As String
                downloadFullPath = Me.DownloadFolderFullPath & Me.CombineUnixPathsSafely(Me.ResolveSubFolderFromDownloadLocation(downloadLocation), pathInDownloadLocation)
                downloadFullPath = downloadFullPath.Replace("//", "/").Replace("//", "/")
                If Not System.IO.Directory.Exists(downloadFullPath) Then
                    Try
                        System.IO.Directory.CreateDirectory(downloadFullPath)
                    Catch ex As Exception
                        If Me._WebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper Then
                            Throw New Exception("Download can't be provided, directory creation for """ & downloadFullPath & """ failed." & vbNewLine & ex.ToString)
                        Else
                            Throw New Exception("Download can't be provided, file access denied.")
                        End If
                    End Try
                End If

                'define zipfullPath variables to get return path
                Dim targetZipfileFullPath As String = downloadFullPath & "/" & System.Guid.NewGuid.ToString("n") & ".zip"
                If Not zipArchiveName = "" Then
                    targetZipfileFullPath = downloadFullPath & "/" & System.IO.Path.GetFileNameWithoutExtension(zipArchiveName) & ".zip"
                End If
                targetZipfileFullPath = targetZipfileFullPath.Replace("//", "/").Replace("//", "/")

                'define boolean variable to check weather download file alread exists or not.
                Dim targetFileExists As Boolean = False
                Dim targetZipFileExists As Boolean = False

                Dim zos As ZipOutputStream = Nothing
                Dim fstream As ManagedFileStream = Nothing
                If System.IO.File.Exists(targetZipfileFullPath) AndAlso Not overwrite Then
                    targetZipFileExists = True
                Else
                    fstream = Me.CreateFile(targetZipfileFullPath)
                    zos = New ZipOutputStream(fstream.BaseFileStream)
                End If
                Dim holdZipfileFullPathToDelete As String = targetZipfileFullPath

                'define fullPath variables to get return path
                Dim targetFileFullPath As String = ""
                Dim mimeType As String = ""
                Try
                    If Me.InnerList.Count = 1 AndAlso Not zipFilesFirst Then
                        Dim fData As FileData = CType(Me.InnerList.Item(0), FileData)
                        Select Case fData.Type
                            Case FileDataType.FilePath, FileDataType.FileInfo
                                targetZipfileFullPath = ""

                                Dim sourceFileInfo As FileInfo = Nothing
                                If fData.Type = FileDataType.FilePath Then
                                    sourceFileInfo = New FileInfo(fData.FilePath)
                                ElseIf fData.Type = FileDataType.FileInfo Then
                                    sourceFileInfo = fData.FileInfo
                                End If

                                If fData.NewFileName = "" Then
                                    targetFileFullPath = downloadFullPath & "/" & fData.FolderVirtualPath & "/" & sourceFileInfo.Name
                                Else
                                    targetFileFullPath = downloadFullPath & "/" & fData.FolderVirtualPath & "/" & fData.NewFileName
                                End If
                                targetFileFullPath = targetFileFullPath.Replace("//", "/").Replace("//", "/").Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)

                                Dim targetFi As New FileInfo(targetFileFullPath)

                                'if file get changed frequently, user should use overwrite = True to refresh the cached element
                                If targetFi.Exists AndAlso Not overwrite Then
                                    targetFileExists = True
                                Else
                                    If targetFi.Exists Then
                                        targetFi.Delete()
                                    End If
                                    If Not targetFi.Directory.Exists Then
                                        targetFi.Directory.Create()
                                    End If

                                    Dim TestCount As Integer = 0

                                    Dim CreationOfHardlinkFailed As Boolean = False
                                    Try
#If Not Linux Then
                                        Try
                                            'Don't run simultaneously in web/http context (e.g. double click requesting a hardlink twice at the same time results in file-already-present race exceptions)
                                            If Not System.Web.HttpContext.Current Is Nothing Then System.Web.HttpContext.Current.Application.Lock()
                                            CompuMaster.camm.WebManager.Tools.IO.Junctions.Create(sourceFileInfo.FullName, targetFileFullPath, CompuMaster.camm.WebManager.Tools.IO.Junctions.LinkTypeDirectives.HardLink)
                                        Finally
                                            If Not System.Web.HttpContext.Current Is Nothing Then System.Web.HttpContext.Current.Application.UnLock()
                                        End Try
#Else
                                        Throw New NotSupportedException("copy the file since hard links are not available")
#End If
                                        'test if the creation of the hardlink was successful
                                        'somehow the hardlink is not acessible directly after creation, so we try it a few times
                                        TestCount = 0
                                        Do
                                            Threading.Thread.Sleep((250 * TestCount))
                                            targetFi.Refresh()
                                            TestCount += 1
                                            Log.WriteEventLogTrace("CWM DH: Test No. " & TestCount & " if the hardlink creation was successful. SourceLocation '" & sourceFileInfo.FullName & "'. TargetLocation '" & targetFi.FullName & "'.", Diagnostics.EventLogEntryType.Warning)
                                        Loop Until TestCount >= 20 OrElse targetFi.Exists = True
                                        If targetFi.Exists = False Then
                                            Throw New Exception("The hardlink could not be created at " & targetFi.FullName)
                                        End If
                                    Catch ex As Exception
                                        Me._WebManager.Log.ReportWarningByEMail(ex, "Download handler FAILED to create a hard link")
                                        Log.WriteEventLogTrace("CWM DH: Cannot create hardlink. SourceLocation '" & sourceFileInfo.FullName & "'. TargetLocation '" & targetFi.FullName & "'." & vbNewLine & "Full exception: " & vbNewLine & ex.ToString, Diagnostics.EventLogEntryType.Warning)
                                        CreationOfHardlinkFailed = True
                                    End Try
                                    If CreationOfHardlinkFailed Then
                                        'creation of hardlink failed. try to copy the file via convential system.io.copy to the donwloadhandler cache.
                                        Try
                                            Dim tFileName As String = sourceFileInfo.FullName
                                            sourceFileInfo = Nothing
                                            sourceFileInfo = New FileInfo(tFileName)
                                            sourceFileInfo.CopyTo(targetFi.FullName, True)
                                            'System.IO.File.Copy(sourceFileInfo.FullName, targetFi.FullName, True)
                                            For GarbageCounter As Integer = 0 To 2
                                                If targetFi.Exists = True Then Exit For
#If NetFramework = "1_1" Then
                                                GC.Collect(2)
#Else
                                                GC.Collect(2, GCCollectionMode.Forced)
#End If
                                                System.Threading.Thread.Sleep(250)
                                            Next
                                        Catch ex As Exception
                                            Log.WriteEventLogTrace("CWM DH: Cannot copy files. SourceLocation '" & sourceFileInfo.FullName & "'. TargetLocation '" & targetFi.FullName & "'." & vbNewLine & "Full exception: " & vbNewLine & ex.ToString, Diagnostics.EventLogEntryType.Warning)
                                            If targetFi.Exists = False Then
                                                If Me._WebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingAndRedirectAllEMailsToDeveloper Then
                                                    Throw New Exception("Hardlink creation FAILED, alternative conventional file copy also FAILED to copy file to the downloadhandler location at """ & targetFi.FullName & """")
                                                Else
                                                    Throw New Exception("Hardlink creation FAILED, alternative conventional file copy also FAILED to copy file to the downloadhandler location", ex)
                                                End If
                                            End If
                                        End Try
                                    End If

#If NetFramework = "1_1" Then
                                    'test if copy of target file to downloadhandler location was successful
                                    'wait for file handle to end
                                    TestCount = 0
                                    Do
                                        Threading.Thread.Sleep((250 * TestCount))
                                        targetFi.Refresh()
                                        TestCount += 1
                                    Loop Until TestCount >= 5 OrElse targetFi.Exists = True
#End If
                                    'test if the targetfile exists
                                    If targetFi.Exists = False Then
                                        Log.WriteEventLogTrace("CWM DH: The DownloadHandler file does not exist at the expected location '" & targetFi.FullName & "'.", Diagnostics.EventLogEntryType.Warning)
                                        Throw New Exception("The file could not be copied to the downloadhandler location.")
                                        targetFileExists = False
                                    End If
                                End If
                            Case FileDataType.Directory
                                ' do not write to zip file if it already exists, send existing zip file for download
                                If Not targetZipFileExists Then
                                    Me.writeDirectoryToZipOutputStream(zos, fData.FolderVirtualPath, fData.DirectoryInfo.FullName)
                                End If
                            Case FileDataType.RawFile
                                targetZipfileFullPath = ""
                                If fData.NewFileName = "" Then
                                    targetFileFullPath = downloadFullPath & "/" & fData.FolderVirtualPath & "/" & fData.RawDataFile.Filename
                                Else
                                    targetFileFullPath = downloadFullPath & "/" & fData.FolderVirtualPath & "/" & fData.NewFileName
                                End If
                                targetFileFullPath = targetFileFullPath.Replace("//", "/").Replace("//", "/")

                                If System.IO.File.Exists(targetFileFullPath) AndAlso Not overwrite Then
                                    targetFileExists = True
                                Else
                                    Dim fs As ManagedFileStream = Me.CreateFile(targetFileFullPath)
                                    fs.Write(fData.RawDataFile.Data, 0, fData.RawDataFile.Data.Length)
                                    fs.Dispose()
                                    mimeType = fData.RawDataFile.MimeType
                                End If
                            Case FileDataType.RawCollection
                                ' do not write to zip file if it already exists, send existing zip file for download
                                If Not targetZipFileExists Then
                                    If fData.NewFileName = "" Then
                                        Me.writeByteArrayToZipOutputStream(zos, fData.FolderVirtualPath & "/" & fData.RawDataCollection.Filename, fData.RawDataCollection.Data)
                                    Else
                                        Me.writeByteArrayToZipOutputStream(zos, fData.FolderVirtualPath & "/" & fData.NewFileName, fData.RawDataCollection.Data)
                                    End If
                                End If
                        End Select
                    Else
                        'if targetZipFileExists then only create new archive otherwise return existing archive
                        If Not targetZipFileExists Then
                            For Each fData As FileData In Me.InnerList
                                Select Case fData.Type
                                    Case FileDataType.FilePath
                                        Dim fi As New FileInfo(fData.FilePath)
                                        If fData.NewFileName = "" Then
                                            Me.writeFileToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fi.Name), fi)
                                        Else
                                            Me.writeFileToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.NewFileName), fi)
                                        End If
                                    Case FileDataType.FileInfo
                                        If fData.NewFileName = "" Then
                                            Me.writeFileToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.FileInfo.Name), fData.FileInfo)
                                        Else
                                            Me.writeFileToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.NewFileName), fData.FileInfo)
                                        End If
                                    Case FileDataType.Directory
                                        Me.writeDirectoryToZipOutputStream(zos, fData.FolderVirtualPath, fData.DirectoryInfo.FullName)
                                    Case FileDataType.RawFile
                                        If fData.NewFileName = "" Then
                                            Me.writeByteArrayToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.RawDataFile.Filename), fData.RawDataFile.Data)
                                        Else
                                            Me.writeByteArrayToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.NewFileName), fData.RawDataFile.Data)
                                        End If
                                    Case FileDataType.RawCollection
                                        If fData.NewFileName = "" Then
                                            Me.writeByteArrayToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.RawDataCollection.Filename), fData.RawDataCollection.Data)
                                        Else
                                            Me.writeByteArrayToZipOutputStream(zos, Utils.CombineUnixPaths(fData.FolderVirtualPath, fData.NewFileName), fData.RawDataCollection.Data)
                                        End If
                                End Select
                            Next
                        End If
                    End If
                Finally
                    If Not zos Is Nothing Then
                        If Not zos.IsFinished Then
                            zos.Finish()
                        End If
                        Try
                            zos.Close()
                        Catch
                            'drop exception, we just wan't to ensure that the file is closed
                        End Try
                        CType(zos, IDisposable).Dispose()
                    End If
                    If Not fstream Is Nothing Then
                        fstream.Dispose()
                    End If
                End Try

                If targetZipfileFullPath = "" Then
                    System.IO.File.Delete(holdZipfileFullPathToDelete)
                End If

                'note- PathToRecordAndProcess is full file path
                Dim returnData As New DataToProcessDownload
                If targetZipfileFullPath <> "" AndAlso targetFileFullPath = "" Then
                    returnData.PathToRecordAndProcess = targetZipfileFullPath
                    returnData.TargetFileExists = targetZipFileExists
                ElseIf targetZipfileFullPath = "" AndAlso targetFileFullPath <> "" Then
                    returnData.PathToRecordAndProcess = targetFileFullPath
                    returnData.TargetFileExists = targetFileExists
                End If
                returnData.MimeType = mimeType
                returnData.SendToCurrentRequest = False

                Return returnData
            End If

        End Function

        ''' <summary>
        '''     Combine a unix path with another one, but handle the 2nd path always as as relative path
        ''' </summary>
        ''' <param name="path1">A first path</param>
        ''' <param name="relativePath2">A second path which must be relative and shall be appended to the first path</param>
        ''' <returns>The combined path</returns>
        ''' <remarks>
        ''' If path2 starts with &quot;/&quot;, an exception is thrown to prevent security issues
        ''' </remarks>
        Private Function CombineUnixPathsSafely(ByVal path1 As String, ByVal relativePath2 As String) As String
            If relativePath2 <> Nothing AndAlso relativePath2.StartsWith("/") Then
                Throw New ArgumentException("Invalid leading ""/"" in paramter value", "relativePath2")
            Else
                Return Utils.CombineUnixPaths(path1, relativePath2)
            End If
        End Function

        ''' <summary>
        '''     This structure used to hold data returned by method "GetDataToRecordAndProcess".
        ''' </summary>
        Private Structure DataToProcessDownload
            Dim PathToRecordAndProcess As String
            Dim TargetFileExists As Boolean
            Dim MimeType As String
            Dim SendToCurrentRequest As Boolean
            Dim RawFile As RawDataSingleFile
        End Structure
        ''' <summary>
        '''     Returns virtual path
        ''' </summary>
        ''' <param name="fileFullPath">The physical full path of a file</param>
        Private Function GetDownloadFileVirtualPath(ByVal fileFullPath As String) As String
            Dim fPath As String = fileFullPath
            fPath = fPath.Replace("\"c, "/"c)
            fPath = fPath.Replace("//", "/"c)
            Dim downloadFolder As String = Me.DownloadFolderFullPath
            downloadFolder = downloadFolder.Replace("\"c, "/"c)
            Try
                If fPath.IndexOf(Me.DownloadFolderFullPath) > 0 Then
                    fPath = fPath.Substring(fPath.IndexOf(downloadFolder))
                End If
                fPath = fPath.Replace(downloadFolder, "")
            Catch ex As Exception
                Throw New Exception("Inside GetDownloadFileVirtualPath. " & ex.Message)
            End Try
            Return fPath
        End Function
        ''' <summary>
        '''     This method creates file including parent directory if not exists 
        ''' </summary>
        ''' <param name="fileFullPath">Defines the files virtual path, which is to be created.</param>
        ''' <returns>Returns System.IO.FileStream object of file created.</returns>
        ''' <remarks>
        '''     This method throws exception message, if any error occured.
        '''     This method also looks for the parent directory of the file defined by "fileFullPath".
        '''     And also creates the parent directory/directories, if they do not exist. 
        ''' </remarks>
        Private Function CreateFile(ByVal fileFullPath As String) As ManagedFileStream
            Dim fi As New FileInfo(fileFullPath)
            If Not fi.Directory.Exists Then
                Try
                    fi.Directory.Create()
                Catch ex As Exception
                    Throw New Exception("Directory could not be created", ex)
                End Try
            End If
            Dim fs As ManagedFileStream
            Try
                fs = New ManagedFileStream(fi.Create)
            Catch ex As Exception
                Throw New Exception("File could not be created", ex)
            End Try
            Return fs
        End Function
        ''' <summary>
        '''     Send a file inside the current request
        ''' </summary>
        ''' <param name="fileFullPath">Defines the files virtual path.</param>
        Private Sub SendDownloadFileToCurrentRequest(ByVal fileFullPath As String)
            Me.SendDownloadFileToCurrentRequest(fileFullPath, False, Nothing)
        End Sub
        ''' <summary>
        '''     Send a file inside the current request
        ''' </summary>
        ''' <param name="fileFullPath">Defines files virtual path.</param>
        ''' <param name="enforceDownload">If true, download handler enforces client browswer to download the file.</param>
        ''' <param name="mimeType">Defines the mime type of the file.</param>
        Private Sub SendDownloadFileToCurrentRequest(ByVal fileFullPath As String, ByVal enforceDownload As Boolean, ByVal mimeType As String)
            Dim fi As New FileInfo(fileFullPath)
            If Not fi.Exists Then
                If Me._WebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                    Throw New HttpException(404, "Download file """ & fi.FullName & """ does not exists.")
                Else
                    Throw New HttpException(404, "Download file """ & fi.Name & """ does not exists.")
                End If
            End If
            If fi.Length > Me.MaxDownloadSize Then
                Throw New DownloadHandlerFileSizeLimitException
            End If

            HttpContext.Current.Response.AddHeader("Content-Disposition", "filename=" & fi.Name)
            If enforceDownload Then
                HttpContext.Current.Response.ContentType = "application/octet-stream"
            Else
                If mimeType = Nothing Then
                    HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(fi.Name))
                Else
                    HttpContext.Current.Response.ContentType = mimeType
                End If
            End If

            HttpContext.Current.Response.WriteFile(fi.FullName)
        End Sub
        ''' <summary>
        '''     Send a raw file inside the current request
        ''' </summary>
        ''' <param name="rawDataFile"></param>
        Private Sub SendDownloadFileToCurrentRequest(ByVal rawDataFile As RawDataCollectionMember)
            HttpContext.Current.Response.AddHeader("Content-disposition", "filename=" & rawDataFile.Filename)
            HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(rawDataFile.Filename))
            HttpContext.Current.Response.BinaryWrite(rawDataFile.Data)
        End Sub
        ''' <summary>
        '''     Send a raw file inside the current request
        ''' </summary>
        ''' <param name="rawDataFile"></param>
        Private Sub SendDownloadFileToCurrentRequest(ByVal rawDataFile As RawDataSingleFile)
            HttpContext.Current.Response.AddHeader("Content-disposition", "filename=" & rawDataFile.Filename)
            If rawDataFile.MimeType = "" Then
                HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(rawDataFile.Filename))
            Else
                HttpContext.Current.Response.ContentType = rawDataFile.MimeType
            End If
            HttpContext.Current.Response.BinaryWrite(rawDataFile.Data)
        End Sub
        ''' <summary>
        '''     Redirects file to browser
        ''' </summary>
        ''' <param name="fileFullPath">File full path</param>
        Private Sub RedirectFileToBrowser(ByVal fileFullPath As String)
            Me.RedirectFileToBrowser(fileFullPath, Nothing)
        End Sub
        ''' <summary>
        '''     Redirects file to browser
        ''' </summary>
        ''' <param name="fileFullPath">Defines files virtual path.</param>
        ''' <param name="mimeType"></param>
        Private Sub RedirectFileToBrowser(ByVal fileFullPath As String, ByVal mimeType As String)
            Log.WriteEventLogTrace("DH RedirectFileToBrowser Start")
            Dim fi As New FileInfo(fileFullPath)
            If Not fi.Exists Then
                Log.WriteEventLogTrace("RedirectFileToBrowser Diwnloadfile does not exist")
                If WMSystem.Configuration.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                    Throw New HttpException(404, "Download file """ & fileFullPath & """ does not exist")
                Else
                    Throw New HttpException(404, "Download file does not exist")
                End If
            End If
            If fi.Length > Me.MaxDownloadSize Then
                If WMSystem.Configuration.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails Then
                    Throw New DownloadHandlerFileSizeLimitException(fileFullPath)
                Else
                    Throw New DownloadHandlerFileSizeLimitException
                End If
            End If

            Dim fPath As String = fi.FullName
            fPath = fPath.Replace("\", "/")
            fPath = fPath.Substring(fPath.IndexOf(Me.SystemDownloadFolderForTemporaryFiles))
            If mimeType = Nothing Then
                HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(fi.Name))
            Else
                HttpContext.Current.Response.ContentType = mimeType
            End If
            Log.WriteEventLogTrace("DH RedirectFileToBrowser Response.Redirect: " & fPath)
            Utils.RedirectTemporary(HttpContext.Current, fPath)
            Log.WriteEventLogTrace("DH RedirectFileToBrowser End")
        End Sub
        ''' <summary>
        '''     Returns mime type for file extension type.
        ''' </summary>
        ''' <param name="extension">e.g. ".pdf"</param>
        Private Function GetMimeType(ByVal extension As String) As String
            Return CompuMaster.camm.WebManager.MimeTypes.MimeTypeByFileExtension(extension)
        End Function
        ''' <summary>
        '''     Redirect the requested file to the browser
        ''' </summary>
        ''' <param name="UniqueCrypticID">Defines the unique identifier, of the requested download file.</param>
        Friend Sub DownloadFileByID(ByVal UniqueCrypticID As String)
            Dim FileOnDisc As String = Me.GetFileFullPath(UniqueCrypticID)
            Me._WebManager.Trace.Write("CWM DH", "FileOnDisc=" & FileOnDisc)
            Me.RedirectFileToBrowser(FileOnDisc)
        End Sub
        ''' <summary>
        '''     Redirect the requested file to the browser
        ''' </summary>
        ''' <param name="fileVirtualPath">Defines the virtual file path of the requested download file.</param>
        Friend Sub DownloadFileByPath(ByVal fileVirtualPath As String)
            'check for malicious fileVirtualPath.
            Dim fFullPath As String = HttpContext.Current.Server.MapPath(Me.SystemDownloadFolderForTemporaryFiles & EncryptedValueCurrentServerIdentString & "/" & fileVirtualPath)
            fFullPath = fFullPath.Replace("\"c, "/"c)
            Dim dFolderPath As String = Me.DownloadFolderFullPath
            dFolderPath = dFolderPath.Replace("\"c, "/"c)
            If fFullPath.IndexOf(dFolderPath) < 0 Then
                Throw New Exception("Access denied")
            End If

            Me._WebManager.Trace.Write("CWM DH", "fileVirtualPath=" & fileVirtualPath)
            Me._WebManager.Trace.Write("CWM DH", "fFullPath=" & fFullPath)
            Me._WebManager.Trace.Write("CWM DH", "dFolderPath=" & dFolderPath)
            Me._WebManager.Trace.Write("CWM DH", "Me.DownloadFolderFullPath & fileVirtualPath=" & Me.DownloadFolderFullPath & fileVirtualPath)

            Log.WriteEventLogTrace("CWM DH fileVirtualPath=" & fileVirtualPath)
            Log.WriteEventLogTrace("CWM DH fFullPath=" & fFullPath)
            Log.WriteEventLogTrace("CWM DH dFolderPath=" & dFolderPath)
            Log.WriteEventLogTrace("CWM DH DownloadFolderFullPath & fileVirtualPath=" & Me.DownloadFolderFullPath & fileVirtualPath)

            Me.RedirectFileToBrowser(Me.DownloadFolderFullPath & fileVirtualPath)
        End Sub
        ''' <summary>
        '''     A cached value for the encrypted server identification string of the current server
        '''     This value is cached because the encryption process is time expensive.
        '''     But in web-version we must cache it in a namevaluecollection because its key can differ during runtime by each request.
        ''' </summary>
        ''' <value></value>
        Private ReadOnly Property EncryptedValueCurrentServerIdentString() As String
            Get
                If HttpContext.Current Is Nothing Then
                    Static _Result As String
                    'Non-Web
                    If _Result Is Nothing Then
                        _Result = DownloadHandler.GetEncryptedValue(Me.CurrentServerIdentString)
                    End If
                    Return _Result
                Else
                    'Web
                    If CachedEncryptedCurrentServerIdentString.Item(Me.CurrentServerIdentString) = Nothing Then
                        CachedEncryptedCurrentServerIdentString(Me.CurrentServerIdentString) = DownloadHandler.GetEncryptedValue(Me.CurrentServerIdentString)
                    End If
                    Return CachedEncryptedCurrentServerIdentString.Item(Me.CurrentServerIdentString)
                End If
            End Get
        End Property

        ''' <summary>
        ''' A cached NameValueCollection of encrypted serveridentstrings.
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Private Shared ReadOnly Property CachedEncryptedCurrentServerIdentString() As Collections.Specialized.NameValueCollection
            Get
                If Not HttpContext.Current Is Nothing Then
                    'web
                    If HttpContext.Current.Cache("CachedEncryptedCurrentServerIdentString") Is Nothing Then
                        HttpContext.Current.Cache("CachedEncryptedCurrentServerIdentString") = New Collections.Specialized.NameValueCollection
                    End If
                    Return CType(HttpContext.Current.Cache("CachedEncryptedCurrentServerIdentString"), Collections.Specialized.NameValueCollection)
                Else
                    'non web
                    Static Result As Collections.Specialized.NameValueCollection
                    If Result Is Nothing Then
                        Result = New Collections.Specialized.NameValueCollection
                    End If
                    Return Result
                End If
            End Get
        End Property
        ''' <summary>
        '''      Verifies that the download location exists, otherwise creates it
        ''' </summary>
        Private Sub CheckExistanceOfTempDownloadFolder()
            'Read cache
            If Not HttpContext.Current Is Nothing Then
                'Web applications
                If Not HttpContext.Current.Application("WebManager.DownloadHandler.CheckExistanceOfTempDownloadFolder") Is Nothing Then
                    Exit Sub
                End If
            End If
            'Do the check
            Try
                Dim dirInfo As New DirectoryInfo(Me.DownloadFolderFullPath)
                If Not dirInfo.Exists Then
                    dirInfo.Create()
                End If
            Catch ex As Exception
                Me._IsFullyFeatured = TriState.False
            End Try
            'Save cache value
            If Not HttpContext.Current Is Nothing Then
                'Web applications
                HttpContext.Current.Application("WebManager.DownloadHandler.CheckExistanceOfTempDownloadFolder") = True
            End If
        End Sub
        ''' <summary>
        '''     Calculates the size of the file/directory collection
        ''' </summary>
        ''' <remarks>
        '''     This is the raw size of the collection before compression takes effect
        ''' </remarks>
        Private Function GetDownloadCollectionSize() As Int64
            Dim size As Int64 = 0
            For Each fData As FileData In Me.InnerList
                Select Case fData.Type
                    Case FileDataType.FilePath
                        Dim fi As New FileInfo(fData.FilePath)
                        size = size + fi.Length
                    Case FileDataType.FileInfo
                        size = size + fData.FileInfo.Length
                    Case FileDataType.Directory
                        size = size + Me.GetDirectorySize(fData.DirectoryInfo)
                    Case FileDataType.RawFile
                        size = size + fData.RawDataFile.Data.Length
                    Case FileDataType.RawCollection
                        size = size + fData.RawDataCollection.Data.Length
                End Select
            Next
            Return size
        End Function
        ''' <summary>
        '''     Calculate the size of a complete directory structure.
        ''' </summary>
        ''' <param name="directory">Denotes the directory structure of type System.IO.DirectoryInfo, whose size is to be calculated.</param>
        ''' <returns>Returns size of complete directory structure as "Long"</returns>
        Private Function GetDirectorySize(ByVal directory As System.IO.DirectoryInfo) As Int64
            Dim size As Int64 = 0
            For Each d As System.IO.DirectoryInfo In directory.GetDirectories
                size = size + Me.GetDirectorySize(d)
            Next
            For Each fi As System.IO.FileInfo In directory.GetFiles
                size = size + fi.Length
            Next
            Return size
        End Function
        ''' <summary>
        '''     Returns file full path from file's unique identifier.
        ''' </summary>
        ''' <param name="UniqueCrypticID"></param>
        Private Function GetFileFullPath(ByVal UniqueCrypticID As String) As String
            If UniqueCrypticID = Nothing Then Throw New Exception("Empty UniqueCrypticID")
            Dim VirtuDownLoc As String = Me.GetVirtualDownloadLocation(UniqueCrypticID)
            If VirtuDownLoc = Nothing Then Throw New Exception("Empty Me.GetVirtualDownloadLocation(UniqueCrypticID) for " & UniqueCrypticID)
            Dim fileFullPath As String = System.IO.Path.Combine(Me.DownloadFolderFullPath, VirtuDownLoc.Replace("/"c, System.IO.Path.DirectorySeparatorChar))

            'why??? the expected result is a physical file path
            'fileFullPath = fileFullPath.Replace("\"c, "/"c)
            'fileFullPath = fileFullPath.Replace("//", "/"c)
            'fileFullPath = fileFullPath.Replace("//", "/"c)

            Log.WriteEventLogTrace("Downloadfilepath: " & fileFullPath, Diagnostics.EventLogEntryType.Information)
            Return fileFullPath
        End Function
        ''' <summary>
        '''     Convert a physical file name to a virtual one by truncating first chars
        ''' </summary>
        ''' <param name="fullPath">A physical file name</param>
        ''' <returns>The virtual path</returns>
        Private Function GetPathFromSystemFolder(ByVal fullPath As String) As String
            Dim myPath As String
            myPath = fullPath.Replace("\", "/")
            Return myPath.Substring(myPath.IndexOf(Me.SystemDownloadFolderForTemporaryFiles))
        End Function

#End Region

#Region "Separate request"

        Private Function ProcessSeparateRequestAndGetDownloadLink(ByVal file As SingleFileInDownloadLocation) As String
            If Not IsWebApplication Then
                Throw New Exception("ProcessDownload is not possible in non-web applications, use ProcessAndGetDownloadLink instead")
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Begin")
            End If
#End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Ready4Transmission")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Ready4Transmission")
            End If
#End If

            'Send data
            'AaB
            'A=1 -> Data to process is RawFile
            'A=2 -> Data to process is FilePath
            'B=1 -> Data is in Session
            'B=2 -> Data is in Cache

            Dim cat As String
            Dim dataID As String
            If CompuMaster.camm.WebManager.Configuration.DownloadHandlerSeparateRequestPutDataInSession Then
                cat = "2a1"
                dataID = Me.PutDataInSession(file.PhysicalPath)
            Else
                cat = "2a2"
                dataID = Me.PutDataInCache(file.PhysicalPath)
            End If

            Return "/system/download.aspx?cat=" & cat & "&dataid=" & dataID

            ''HttpContext.Current.Response.Redirect("/system/download.aspx?cat=" & cat & "&rid=" & randomID)
        End Function

        Private Function ProcessSeparateRequestAndGetDownloadLink(ByVal pathInDownloadLocation As String, ByVal zipFilesFirst As Boolean) As String
            If Not IsWebApplication Then
                Throw New Exception("ProcessDownload is not possible in non-web applications, use ProcessAndGetDownloadLink instead")
            End If

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Begin")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Begin")
            End If
#End If

            'Define file timeout 

            Dim dataToProcess As DataToProcessDownload = Me.GetDataToRecordAndProcess_SeparateRequest(zipFilesFirst)

            'Some performance counters
#If DEBUG Then
            If HttpContext.Current Is Nothing Then
                System.Diagnostics.Trace.Write("ProcessDownload Ready4Transmission")
            Else
                HttpContext.Current.Trace.Write("ProcessDownload Ready4Transmission")
            End If
#End If

            'Send data
            'AaB
            'A=1 -> Data to process is RawFile
            'A=2 -> Data to process is FilePath
            'B=1 -> Data is in Session
            'B=2 -> Data is in Cache

            Dim cat As String
            Dim dataID As String
            If dataToProcess.PathToRecordAndProcess = "" Then
                If CompuMaster.camm.WebManager.Configuration.DownloadHandlerSeparateRequestPutDataInSession Then
                    cat = "1a1"
                    dataID = Me.PutDataInSession(dataToProcess.RawFile)
                Else
                    cat = "1a2"
                    dataID = Me.PutDataInCache(dataToProcess.RawFile)
                End If
            Else
                If CompuMaster.camm.WebManager.Configuration.DownloadHandlerSeparateRequestPutDataInSession Then
                    cat = "2a1"
                    dataID = Me.PutDataInSession(dataToProcess.PathToRecordAndProcess)
                Else
                    cat = "2a2"
                    dataID = Me.PutDataInCache(dataToProcess.PathToRecordAndProcess)
                End If
            End If

            Return "/system/download.aspx?cat=" & cat & "&dataid=" & dataID

            ''HttpContext.Current.Response.Redirect("/system/download.aspx?cat=" & cat & "&rid=" & randomID)
        End Function

        Private Function GetDataToRecordAndProcess_SeparateRequest(ByVal zipFilesFirst As Boolean) As DataToProcessDownload
            If Me.MaxDownloadCollectionSize < Me.GetDownloadCollectionSize Then
                If Me._WebManager.DebugLevel >= WMSystem.DebugLevels.Low_WarningMessagesOnAccessError Then
                    Throw New DownloadHandlerCollectionSizeLimitException(Me.MaxDownloadCollectionSize, Me.GetDownloadCollectionSize)
                Else
                    Throw New DownloadHandlerCollectionSizeLimitException
                End If
            End If

            If zipFilesFirst Then
                Throw New DownloadHandlerNotSupportedException
            End If

            Dim retData As New DataToProcessDownload
            If Not Me.IsFullyFeatured AndAlso Me.IsWebApplication Then
                If Me.InnerList.Count = 1 Then
                    Dim fData As FileData = CType(Me.InnerList.Item(0), FileData)
                    Dim fPath As String = ""

                    Select Case fData.Type
                        Case FileDataType.FilePath
                            fPath = fData.FilePath
                        Case FileDataType.FileInfo
                            fPath = fData.FileInfo.FullName
                        Case FileDataType.RawFile
                            retData.RawFile = fData.RawDataFile
                        Case Else
                            Throw New DownloadHandlerNotSupportedException
                    End Select

                    retData.SendToCurrentRequest = True
                    fPath = fPath
                    fPath = fPath.Replace("//", "/"c)
                    fPath = fPath.Replace("//", "/"c)

                    retData.PathToRecordAndProcess = fPath
                    retData.TargetFileExists = False
                    retData.MimeType = Nothing

                Else
                    Throw New DownloadHandlerNotSupportedException
                End If
            End If
            Return retData

        End Function

        Private Function PutDataInCache(ByVal data As Object) As String
            Dim randomID As String = System.Guid.NewGuid.ToString.Replace("-", "")
            HttpContext.Current.Cache.Add(Me.SeparateRequestCacheKeyName & "_" & randomID, data, Nothing, Now.AddSeconds(CompuMaster.camm.WebManager.Configuration.DownloadHandlerSeparateRequestCacheTimeLimit_InMinutes), Caching.Cache.NoSlidingExpiration, Caching.CacheItemPriority.Normal, Nothing)
            Return randomID
        End Function

        Private Function PutDataInSession(ByVal data As Object) As String
            Dim randomID As String = System.Guid.NewGuid.ToString.Replace("-"c, "")
            HttpContext.Current.Session.Add(Me.SeparateRequestSessionKeyName & "_" & randomID, data)
            Return randomID
        End Function

        Private Function GetSeparateRequestKeyDateTime(ByVal key As String) As Date
            Dim myString As String = key.Substring(key.IndexOf("_") + 1, 14)
            Dim myDate As New Date(CType(myString.Substring(0, 4), Integer), CType(myString.Substring(4, 2), Integer), CType(myString.Substring(6, 2), Integer), CType(myString.Substring(8, 2), Integer), CType(myString.Substring(10, 2), Integer), CType(myString.Substring(12, 2), Integer))
            Return myDate
        End Function

        Private Function DownloadHandlerSeparateRequestPutDataInSession_() As Boolean
            Dim result As Boolean = False
            If CompuMaster.camm.WebManager.Configuration.WebManagerSettings("WebManager.DownloadHandlerSeparateRequestPutDataInSession").ToLower = "true" Then
                result = True
            End If
            Return result
        End Function

        Private Function SeparateRequestSessionKeyName() As String
            Return DownloadHandler.SeparateRequestSessionKeyPrefix & System.Guid.NewGuid.ToString("n")
        End Function

        Private Function SeparateRequestCacheKeyName() As String
            Return DownloadHandler.SeparateRequestCacheKeyPrefix & System.Guid.NewGuid.ToString("n")
        End Function

        'to called throgh download.aspx 'ProcessSeparateRequestDownload
        Public Sub DownloadFileFromCache(ByVal cat As String, ByVal dataID As String)
            Dim catData As Integer = CType(cat.Substring(0, 1), Integer)
            Dim catSession As Integer = CType(cat.Substring(2, 1), Integer)
            If catSession = 1 Then
                'process data from session
                Me.ProcessSeparateRequestFromSession(catData, dataID)
            Else
                'process data from cache
                Me.ProcessSeparateRequestFromCache(catData, dataID)
            End If
        End Sub

        Private Sub ProcessSeparateRequestFromCache(ByVal cat As Integer, ByVal dataID As String)
            Dim cacheKey As String = Nothing
            Dim myEnum As IDictionaryEnumerator = HttpContext.Current.Cache.GetEnumerator
            While myEnum.MoveNext
                If myEnum.Key.GetType().ToString = "System.String" Then
                    Dim key As String = CType(myEnum.Key, String)
                    If key.IndexOf(DownloadHandler.SeparateRequestCacheKeyPrefix) = 0 Then
                        If key.IndexOf(dataID) > 0 Then
                            cacheKey = key
                        End If
                    End If
                End If
            End While
            Dim downloadObject As Object = HttpContext.Current.Cache.Item(cacheKey)
            If downloadObject Is Nothing Then
                Throw New Exception("#101 " & "Critical error occured.")
            End If
            If cat = 1 Then
                'raw data
                Dim rawFile As CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile = CType(downloadObject, CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile)

                HttpContext.Current.Response.AddHeader("Content-disposition", "filename=" & rawFile.Filename)
                If rawFile.MimeType = "" Then
                    HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(rawFile.Filename))
                Else
                    HttpContext.Current.Response.ContentType = rawFile.MimeType
                End If
                HttpContext.Current.Response.BinaryWrite(rawFile.Data)
            Else
                'filepath
                Dim fileFullPath As String = CType(downloadObject, String)
                Dim fi As New FileInfo(fileFullPath)
                If Not fi.Exists Then
                    Throw New HttpException(404, "Download file """ & fi.Name & """ does not exists.")
                End If
                If fi.Length > Me.MaxDownloadSize Then
                    Throw New DownloadHandlerFileSizeLimitException
                End If

                HttpContext.Current.Response.AddHeader("Content-Disposition", "filename=" & fi.Name)
                'HttpContext.Current.Response.ContentType = "application/octet-stream"
                HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(fi.Name))


                'HttpContext.Current.Server.Transfer(fileVirtualPath, True)

                Dim fileSize As Long = fi.Length
                HttpContext.Current.Response.WriteFile(fi.FullName)
            End If

            'TODO: discuss
            HttpContext.Current.Cache.Remove(cacheKey)
        End Sub

        Private Sub ProcessSeparateRequestFromSession(ByVal cat As Integer, ByVal dataID As String)
            Dim sessionKey As String = Nothing
            For Each key As String In HttpContext.Current.Session.Keys
                If key.IndexOf(DownloadHandler.SeparateRequestSessionKeyPrefix) = 0 Then
                    If key.IndexOf(dataID) > 0 Then
                        sessionKey = key
                    End If
                End If
            Next

            Dim downloadObject As Object = HttpContext.Current.Session(sessionKey)
            If cat = 1 Then
                'RawFile
                Dim rawFile As CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile = CType(downloadObject, CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile)

                HttpContext.Current.Response.AddHeader("Content-disposition", "filename=" & rawFile.Filename)
                If rawFile.MimeType = "" Then
                    HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(rawFile.Filename))
                Else
                    HttpContext.Current.Response.ContentType = rawFile.MimeType
                End If
                HttpContext.Current.Response.BinaryWrite(rawFile.Data)
            Else
                'FilePath
                Dim fileFullPath As String = CType(downloadObject, String)
                Dim fi As New FileInfo(fileFullPath)
                If Not fi.Exists Then
                    Throw New HttpException(404, "Download file """ & fi.FullName & """ does not exists.")
                End If
                If fi.Length > Me.MaxDownloadSize Then
                    Throw New DownloadHandlerFileSizeLimitException
                End If

                HttpContext.Current.Response.AddHeader("Content-Disposition", "filename=" & fi.Name)
                HttpContext.Current.Response.ContentType = "application/octet-stream"
                HttpContext.Current.Response.ContentType = Me.GetMimeType(System.IO.Path.GetExtension(fi.Name))


                'HttpContext.Current.Server.Transfer(fileVirtualPath, True)

                Dim fileSize As Long = fi.Length
                HttpContext.Current.Response.WriteFile(fi.FullName)
            End If

            'TODO: discuss 
            HttpContext.Current.Session.Remove(sessionKey)
        End Sub


#End Region

#Region "ZipMethods"
        ''' <summary>
        '''     This method archives the files defined by parameter "files()" in zip format.
        ''' </summary>
        ''' <param name="files">
        '''     Defines the array of files which are to be archived in zip. 
        '''     Each element of array should contain complete path of file.
        ''' </param>
        ''' <param name="targetZipfilePath">Defines complete path of target zip archive.</param>
        ''' <remarks>
        '''     Throws execption if occured.
        ''' </remarks>
        Private Sub zipFiles(ByVal files() As String, ByVal targetZipfilePath As String)
            Dim zos As New ZipOutputStream(File.Create(targetZipfilePath))

            Dim fPath As String = ""
            For Each fPath In files
                Dim fi As New FileInfo(fPath)
                Try
                    Me.writeFileToZipOutputStream(zos, fi.Name, fi)
                Catch ex As Exception
                    Throw New Exception("Inside zipFiles. " & ex.Message)
                Finally
                    Try
                        zos.Flush()
                    Catch
                        'drop exception, we just wan't to ensure that the file is closed
                    End Try
                    Try
                        zos.Close()
                    Catch
                        'drop exception, we just wan't to ensure that the file is closed
                    End Try
                    CType(zos, IDisposable).Dispose()
                End Try
            Next
        End Sub
        ''' <summary>
        '''     This method adds complete directory structure defined by "directoryPath", into 
        '''     zip archive defined by "zos" which is of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream".
        ''' </summary>
        ''' <param name="zos">This is the object of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream"</param>
        ''' <param name="fPathToIncludeInZip">
        '''     Defines additional path to include in zip archive
        '''     <example>e.g. 
        '''     fPathToIncludeInZip is "foo/images";
        '''     directoryPath is "d:\temp\myFolder";
        '''     rootPath is "d:\temp\";
        '''     the file name to be archived is "myPicture.jpg";
        '''     the resulting file full name to be added in archive is  
        '''     "myFolder\foo\images\myPicture.jpg"
        '''     </example>
        ''' </param>
        ''' <param name="rootPath">
        '''     rootPath tells zipDir which part of dPath should be removed before compressing and zip in zipfile. 
        '''     e.g. dPath is c:\temp\myFolder\, and you want to store myFile.txt with path myFolder\myFile.txt
        '''     in this case rootPath will be c:\temp\
        ''' </param>
        ''' <param name="directoryPath"></param>
        ''' <remarks>
        '''     Throws exception, if occured.
        ''' </remarks>
        Private Sub zipDir(ByVal zos As ZipOutputStream, ByVal fPathToIncludeInZip As String, ByVal rootPath As String, ByVal directoryPath As String)
            Dim d As String
            Dim f As String
            Try
                For Each d In Directory.GetDirectories(directoryPath)
                    zipDir(zos, fPathToIncludeInZip, rootPath, d)
                Next
                For Each f In Directory.GetFiles(directoryPath)
                    Dim fi As New FileInfo(f)
                    Me.writeFileToZipOutputStream(zos, fPathToIncludeInZip & "/" & f.Replace(rootPath, ""), fi)
                Next
            Catch ex As System.Exception
                Throw New Exception("Inside zipDir. " & ex.Message)
            End Try
        End Sub

        Private Sub zipFile_NotInUse(ByVal fPath As String, ByVal targetZipfilePath As String)
            Dim zos As New ZipOutputStream(File.Create(targetZipfilePath))

            Dim fi As New FileInfo(fPath)
            Try
                Me.writeFileToZipOutputStream(zos, fi.Name, fi)
            Catch ex As Exception
                Throw New Exception(ex.Message)
            Finally
                Try
                    zos.Flush()
                Catch
                    'drop exception, we just wan't to ensure that the file is closed
                End Try
                Try
                    zos.Close()
                Catch
                    'drop exception, we just wan't to ensure that the file is closed
                End Try
                CType(zos, IDisposable).Dispose()
            End Try

        End Sub
        ''' <summary>
        '''     This method adds file defined by "fileInfo", into 
        '''     zip archive defined by "zos" which is of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream".
        ''' </summary>
        ''' <param name="zos">This is the object of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream"</param>
        ''' <param name="fPathToIncludeInZip">
        '''     Defines additional path to include in zip archive
        '''     <example>
        '''     fPathToIncludeInZip is "foo/machine"; 
        '''     the file full name to be archived is "images/myPicture.jpg"; 
        '''     the resulting file full name to be added in archive is 
        '''     "foo\machine\images\myPicture.jpg" 
        '''     </example>
        ''' </param>
        ''' <param name="fileInfo">Defines the file object of type "System.IO.FileInfo" to be added in zip archive.</param>
        Private Sub writeFileToZipOutputStream(ByVal zos As ZipOutputStream, ByVal fPathToIncludeInZip As String, ByVal fileInfo As FileInfo)
            'crc-disabled since this is a problem in 7-zip and causes a invalid zip 
            'Dim crc As New ICSharpCode.SharpZipLib.Checksums.Crc32

            zos.SetLevel(6) '0 - store only to 9 - means best compression					

            Dim entry As New ZipEntry(fPathToIncludeInZip)
            Dim fs As ManagedFileStream = New ManagedFileStream(fileInfo.OpenRead)
            Dim buf(CType(fs.length, Integer) - 1) As Byte
            Try
                fs.Read(buf, 0, buf.Length)
                entry.DateTime = DateTime.Now
                entry.Size = fs.length
            Finally
                fs.Dispose()
            End Try
            'crc.Reset()
            'crc.Update(buf)
            'entry.Crc = crc.Value
            zos.PutNextEntry(entry)
            zos.Write(buf, 0, buf.Length)

        End Sub
        ''' <summary>
        '''     This method adds directory defined by "directoryPath", into 
        '''     zip archive defined by "zos" of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream".
        ''' </summary>
        ''' <param name="zos">This is the object of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream"</param>
        ''' <param name="fPathToIncludeInZip">
        '''     For description please check parameter with same name in method "zipDir".
        ''' </param>
        ''' <param name="directoryPath">
        '''     Defines the full path of directory which is to be added in archive defined by object
        '''     "zos" of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream".
        ''' </param>
        ''' <remarks>
        '''     Throws exception if occured.
        ''' </remarks>
        Private Sub writeDirectoryToZipOutputStream(ByVal zos As ZipOutputStream, ByVal fPathToIncludeInZip As String, ByVal directoryPath As String)
            Dim dName As String = ""
            Dim rootPath As String = ""
            Dim index As Integer = 0
            index = directoryPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)
            If index > 0 Then
                dName = directoryPath.Substring(index + 1)
                rootPath = directoryPath.Substring(0, index + 1)
            End If
            Try
                Me.zipDir(zos, fPathToIncludeInZip, rootPath, directoryPath)
            Catch ex As Exception
                Throw New Exception("Inside writeDirectoryToZipOutputStream. " & ex.Message)
            End Try
        End Sub
        ''' <summary>
        '''     This method adds the file in byte array format to zip archive defined by object
        '''     "zos" of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream".
        ''' </summary>
        ''' <param name="zos">This is the object of type "ICSharpCode.SharpZipLib.Zip.ZipOutputStream"</param>
        ''' <param name="fileNameWithVirtualPathToIncludeInZip">
        '''     Defines the full file name for the file defined by byte array "buffer".
        ''' </param>
        ''' <param name="buffer">Defines the file in byte array format.</param>
        Private Sub writeByteArrayToZipOutputStream(ByVal zos As ZipOutputStream, ByVal fileNameWithVirtualPathToIncludeInZip As String, ByVal buffer() As Byte)
            'crc-disabled since this is a problem in 7-zip and causes a invalid zip 
            'Dim crc As New ICSharpCode.SharpZipLib.Checksums.Crc32
            zos.SetLevel(6)

            '0 - store only to 9 - means best compression					

            Dim entry As New ZipEntry(fileNameWithVirtualPathToIncludeInZip)
            entry.DateTime = DateTime.Now
            entry.Size = buffer.Length
            'crc.Reset()
            'crc.Update(buffer)
            'entry.Crc = crc.Value
            zos.PutNextEntry(entry)
            zos.Write(buffer, 0, buffer.Length)

        End Sub
#End Region

#Region "DBRegion"

        'Database table structure
        'Primary key int
        'UniqeCrypticID char(16)
        'VirtualDownloadLocation nvarchar(1800)
        'ServerID int
        'TimeOfRemoval datetime
        ''' <summary>
        '''     This method returns the Database connection string.
        ''' </summary>
        Private Function getConnectString() As String
            Dim connectString As String
            connectString = _WebManager.ConnectionString
            Return connectString
        End Function
        ''' <summary>
        '''     This method create unique identifier for each record to be added in database
        '''     table. And returns it as a string.
        ''' </summary>
        ''' <param name="targetFileFullPath"></param>
        ''' <param name="serverID"></param>
        ''' <param name="ts"></param>
        ''' <returns>Returns unique idenfier of the record added by this method as Sting.</returns>
        ''' <remarks>
        '''     Throws exception, if occured.
        ''' </remarks>
        Private Function AddDownloadFileRecord(ByVal targetFileFullPath As String, ByVal serverID As Integer, ByVal ts As TimeSpan) As String
            Dim returnString As String = System.Guid.NewGuid.ToString

            Try
                Me.AddDownloadFileRecordToDB(returnString, Me.GetDownloadFileVirtualPath(targetFileFullPath), ts)
            Catch ex As SqlException
                Throw New Exception("Error in AddDownloadFileRecord", ex)
            End Try

            Return returnString
        End Function
        ''' <summary>
        '''     Adds record to database table.
        ''' </summary>
        ''' <param name="location">location should be virtual filepath with file Name</param>
        ''' <param name="ts"></param>
        Private Sub AddDownloadFileRecordToDB(ByVal UniqueCrypticID As String, ByVal location As String, ByVal ts As TimeSpan)
            Dim hour As Integer = ts.Hours
            Dim day As Integer = ts.Days
            hour = hour + (day * 24)

            Dim SqlCmd As New SqlCommand
            SqlCmd.Parameters.Add("@UniqueCrypticID", SqlDbType.Char).Value = UniqueCrypticID
            SqlCmd.Parameters.Add("@VirtualDownloadLocation", SqlDbType.NVarChar).Value = location.Replace("'"c, "''")
            SqlCmd.Parameters.Add("@ServerID", SqlDbType.Int).Value = Me.ServerID
            SqlCmd.Parameters.Add("@hour", SqlDbType.Int).Value = hour
            SqlCmd.CommandText = "INSERT INTO [WebManager_DownloadHandler_Files] ([UniqueCrypticID], [VirtualDownloadLocation], [ServerID], [TimeOfRemoval]) VALUES (@UniqueCrypticID, @VirtualDownloadLocation, @ServerID, DATEADD(hh, @hour, GETDATE()))"
            SqlCmd.Connection = New SqlConnection(Me.getConnectString)
            Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(SqlCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

        End Sub
        ''' <summary>
        '''     Returns list of "VirtualDownloadLocation" for all registered files in the database.
        ''' </summary>
        ''' <remarks>
        '''     Throws exception if occured.
        ''' </remarks>
        Private Function GetAllFileRecordInDB() As System.Collections.Specialized.StringCollection
            Dim returnList As New System.Collections.Specialized.StringCollection

            Dim dbConn As New SqlConnection(Me.getConnectString)
            Dim selectQuery As String = "SELECT [VirtualDownloadLocation], [TimeOfRemoval] FROM [dbo].[WebManager_DownloadHandler_Files] where [ServerID] = " & Me.ServerID
            Dim reader As SqlDataReader = Nothing
            Dim location As String = ""
            Try
                reader = CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteReader(dbConn, selectQuery, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection), SqlDataReader)
                If reader.HasRows Then
                    While reader.Read
                        location = CType(reader("VirtualDownloadLocation"), String)
                        If Not returnList.Contains(location) Then
                            returnList.Add(location)
                        End If
                    End While
                End If
            Catch ex As Exception
                Throw New Exception("Inside GetAllFileRecordInDB. " & ex.Message)
            Finally
                If Not reader Is Nothing Then
                    Try
                        reader.Close()
                    Catch
                        'drop exception, we just wan't to ensure that the file is closed
                    End Try
                End If
                Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(dbConn)
            End Try
            Return returnList
        End Function
        ''' <summary>
        '''     Returns "VirtualDownloadLocation" of the record with unique identifier as "UniqueCrypticID" at current server.
        ''' </summary>
        ''' <param name="UniqueCrypticID">Defines UniqueCrypticID which record to be found</param>
        ''' <remarks>
        '''     Throws exception if occured.
        ''' </remarks>
        Private Function GetVirtualDownloadLocation(ByVal UniqueCrypticID As String) As String
            If UniqueCrypticID = Nothing Then Throw New ArgumentNullException("UniqueCrypticID")
            Dim dbConn As New SqlConnection(Me.getConnectString)
            Dim selectQuery As String = "SELECT [VirtualDownloadLocation] FROM [dbo].[WebManager_DownloadHandler_Files] where [UniqueCrypticID] = N'" & Replace(UniqueCrypticID, "'", "''") & "' and [ServerID] = " & Me.ServerID
            Return CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(dbConn, selectQuery, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String)
        End Function
        ''' <summary>
        '''     Look for the record for location value "VirtualDownloadLocation". And returns unique identifier.
        ''' </summary>
        ''' <param name="VirtualDownloadLocation"></param>
        Private Function GetUniqueCrypticID(ByVal VirtualDownloadLocation As String) As String
            Dim returnString As String = ""
            Dim dbConn As New SqlConnection(Me.getConnectString)
            Dim selectQuery As String = "SELECT [UniqueCrypticID] FROM [dbo].[WebManager_DownloadHandler_Files] where [VirtualDownloadLocation] = N'" & VirtualDownloadLocation.Replace("'", "''") & "' and [ServerID] = " & Me.ServerID
            Return CType(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(dbConn, selectQuery, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), String)
        End Function
        ''' <summary>
        '''     Update file record with new TimeOfRemoval
        ''' </summary>
        ''' <param name="virtualDownloadLocation"></param>
        ''' <param name="timeOfRemoval"></param>
        <Obsolete("the file shall continue with its timeout even if the file has been reused")> _
        Private Sub UpdateDownloadFileRecord(ByVal virtualDownloadLocation As String, ByVal timeOfRemoval As TimeSpan)
            Dim hour As Integer = timeOfRemoval.Hours
            Dim day As Integer = timeOfRemoval.Days
            hour = hour + (day * 24)

            Dim dbConn As New SqlConnection(Me.getConnectString)
            Dim cmdUpdate As New SqlCommand
            With cmdUpdate
                .Parameters.Add(New SqlClient.SqlParameter("@VirtualDownloadLocation", SqlDbType.NVarChar)).Value = virtualDownloadLocation.Replace("'"c, "''")
                .Parameters.Add(New SqlClient.SqlParameter("@ServerID", SqlDbType.Int)).Value = Me.ServerID
                .Parameters.Add(New SqlClient.SqlParameter("@hour", SqlDbType.Int)).Value = hour
                .CommandText = "UPDATE [WebManager_DownloadHandler_Files] SET [TimeOfRemoval] =  DATEADD(hh, @hour, GETDATE()) WHERE [VirtualDownloadLocation] = @VirtualDownloadLocation and [ServerID]= @ServerID"
                .CommandType = CommandType.Text
                .Connection = dbConn
            End With
            Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmdUpdate, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub
        ''' <summary>
        '''     Returns collection of "VirtualDownloadLocation" as current server and deletes those records.
        ''' </summary>
        ''' <param name="regardlessToTheirTimeout">Parameter indicates record search.</param>
        Private Function GetTimedout_VirtualDownloadLocationCollectionAndCleanupRecordsInDB(ByVal regardlessToTheirTimeout As Boolean) As System.Collections.Specialized.StringCollection
            'always remove only files from the current server
            Dim returnCollection As New System.Collections.Specialized.StringCollection

            Dim dbConn As New SqlConnection(Me.getConnectString)
            dbConn.Open()
            Dim cmd As New SqlCommand
            cmd.Connection = dbConn
            Dim deleteQuery As String = ""
            Dim selectQuery As String = ""
            Dim reader As SqlDataReader
            Try
                If regardlessToTheirTimeout Then
                    selectQuery = "SELECT [VirtualDownloadLocation] FROM [WebManager_DownloadHandler_Files] where [ServerID] = " & Me.ServerID
                Else
                    selectQuery = "SELECT [VirtualDownloadLocation] FROM [WebManager_DownloadHandler_Files] where DATEDIFF(ss, TimeOfRemoval, GETDATE()) > 0 and [ServerID] = " & Me.ServerID
                End If
                cmd.CommandText = selectQuery
                reader = cmd.ExecuteReader
                If reader.HasRows Then
                    While reader.Read
                        If Not returnCollection.Contains(CType(reader("VirtualDownloadLocation"), String)) Then
                            returnCollection.Add(CType(reader("VirtualDownloadLocation"), String))
                        End If
                    End While
                End If
                Try
                    reader.Close()
                Catch
                    'drop exception, we just wan't to ensure that the file is closed
                End Try

                'now delete the records
                If regardlessToTheirTimeout Then
                    deleteQuery = "DELETE FROM [dbo].[WebManager_DownloadHandler_Files] where [ServerID] = " & Me.ServerID
                Else
                    deleteQuery = "DELETE FROM [dbo].[WebManager_DownloadHandler_Files] where DATEDIFF(ss, TimeOfRemoval, GETDATE()) > 0 and [ServerID] = " & Me.ServerID
                End If
                cmd.CommandText = deleteQuery
                cmd.ExecuteNonQuery()
            Finally
                Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(dbConn)
            End Try
            Return returnCollection
        End Function

#End Region

#Region " CleanUp "
        ''' <summary>
        '''     Look up for files in the database whose time has run out and deletes them.
        ''' </summary>
        Public Sub CleanUp()
            If Me.IsFullyFeatured Then
                If Me.IsWebApplication Then
                    Me.RemoveFilesFromDownloadFolder(Me.GetTimedout_VirtualDownloadLocationCollectionAndCleanupRecordsInDB(False))
                Else
                    Throw New Exception("Download handler is not supported.")
                End If
            End If
        End Sub
        ''' <summary>
        '''     Look up for files in the database and deletes them.
        ''' </summary>
        Public Sub CleanUpAllRegisteredFiles()
            'Clean up all registered files regardless to their timeouts
            If Me.IsFullyFeatured Then
                If Me.IsWebApplication Then
                    Me.RemoveFilesFromDownloadFolder(Me.GetTimedout_VirtualDownloadLocationCollectionAndCleanupRecordsInDB(True))
                Else
                    Throw New Exception("Download handler is not supported.")
                End If
            End If
        End Sub
        ''' <summary>
        '''     Gets called by the camm Web-Manager administration forms when a supervisor demands it manually
        ''' </summary>
        Public Function CleanUpUnregisteredFiles() As System.Collections.Specialized.StringCollection
            If Me.IsFullyFeatured Then
                'Look up for files in the download folder which aren't registered in the database and delete them immediately
                'Also look up files of the public cache
                If Me.IsWebApplication Then
                    Return Me.RemoveFilesNotRegisteredInDatabase()
                Else
                    Throw New Exception("Download handler is not supported.")
                End If
            Else
                Return New System.Collections.Specialized.StringCollection
            End If
        End Function
        ''' <summary>
        '''     Removes files from current server disc.
        ''' </summary>
        ''' <param name="fileVirtualPathCollection"></param>
        Private Sub RemoveFilesFromDownloadFolder(ByVal fileVirtualPathCollection As System.Collections.Specialized.StringCollection)
            If fileVirtualPathCollection Is Nothing Then
                Exit Sub
            End If

            For Each vPath As String In fileVirtualPathCollection
                Dim fileFullPath As String = Me.DownloadFolderFullPath & vPath
                fileFullPath = fileFullPath.Replace("/"c, System.IO.Path.DirectorySeparatorChar)
                fileFullPath = fileFullPath.Replace("\"c, System.IO.Path.DirectorySeparatorChar)
                fileFullPath = fileFullPath.Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
                fileFullPath = fileFullPath.Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)

                If System.IO.File.Exists(fileFullPath) Then
                    Try
                        System.IO.File.Delete(fileFullPath)
                    Catch

                    End Try
                End If
            Next
        End Sub
        ''' <summary>
        '''     Removes unregistered files from current server disc.
        ''' </summary>
        Private Function RemoveFilesNotRegisteredInDatabase() As System.Collections.Specialized.StringCollection
            Dim returnCollection As New System.Collections.Specialized.StringCollection
            Dim dbList As System.Collections.Specialized.StringCollection = Me.GetAllFileRecordInDB
            Dim dPath As String = Me.DownloadFolderFullPath
            'remove files which are not registered, i.e. not in dbList
            For Each fi As FileInfo In New AllFilesInDirectory(New DirectoryInfo(dPath))
                Dim virtualPath As String = fi.FullName
                virtualPath = virtualPath.Replace("\"c, "/"c)
                Dim downloadPath As String = dPath
                downloadPath = downloadPath.Replace("\"c, "/"c)
                virtualPath = virtualPath.Replace(downloadPath, "")
                If Not dbList.Contains(virtualPath) Then
                    Try
                        fi.Delete()
                        returnCollection.Add(virtualPath)
                    Catch
                    End Try
                End If
            Next
            'Remove empty folders
            For Each di As DirectoryInfo In New AllDirsInDirectory(New DirectoryInfo(dPath))
                If di.GetDirectories.Length = 0 AndAlso di.GetFiles.Length = 0 Then
                    Try
                        di.Delete()
                    Catch
                    End Try
                End If
            Next

            'Remove unauthorised folders in "SystemDownloadFolderForTemporaryFiles"
            Dim availableServerIdentList As System.Collections.Specialized.StringCollection = Me.GetListOfAvailableServersIdentString
            For Each di As DirectoryInfo In New DirectoryInfo(HttpContext.Current.Server.MapPath(Me.SystemDownloadFolderForTemporaryFiles)).GetDirectories
                If Not availableServerIdentList.Contains(di.Name) Then
                    Try
                        di.Delete(True)
                    Catch
                    End Try
                End If
            Next

            'Remove all file in "SystemDownloadFolderForTemporaryFiles"
            For Each fi As FileInfo In New DirectoryInfo(HttpContext.Current.Server.MapPath(Me.SystemDownloadFolderForTemporaryFiles)).GetFiles
                Try
                    fi.Delete()
                    returnCollection.Add(fi.FullName.Replace(HttpContext.Current.Server.MapPath(Me.SystemDownloadFolderForTemporaryFiles), ""))
                Catch
                End Try
            Next


            Return returnCollection
        End Function
        ''' <summary>
        '''     Returns list of available servers identification string.
        ''' </summary>
        Private Function GetListOfAvailableServersIdentString() As System.Collections.Specialized.StringCollection
            Dim result As New System.Collections.Specialized.StringCollection

            Dim serverInformation() As CompuMaster.camm.WebManager.WMSystem.ServerInformation = Me._WebManager.System_GetServersInfo()
            For MyCounter As Integer = 0 To serverInformation.Length - 1
                result.Add(DownloadHandler.GetEncryptedValue(serverInformation(MyCounter).IPAddressOrHostHeader))
            Next

            Return result
        End Function

        Private Sub CleanUpFilesInDownloadFolderManually()
            Me.RemoveFilesNotRegisteredInDatabase()
        End Sub
        ''' <summary>
        '''     Removes cached file
        ''' </summary>
        ''' <param name="downloadLocation">Defines download location</param>
        ''' <param name="pathInDownloadLocation">Path in download location</param>
        ''' <param name="fileName">File name in cache to be removed</param>
        Public Sub RemoveDownloadFileFromCache(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal fileName As String)
            Me.RemoveDownloadFileFromCache(downloadLocation, pathInDownloadLocation, Nothing, fileName)
        End Sub
        ''' <summary>
        '''  Removes cached file
        ''' </summary>
        ''' <param name="downloadLocation">download location</param>
        ''' <param name="pathInDownloadLocation">path in download location</param>
        ''' <param name="folderInVirtualDownloadLocation">folder/s in virtual download location</param>
        ''' <param name="fileName">File name in cache to be removed</param>
        Public Sub RemoveDownloadFileFromCache(ByVal downloadLocation As DownloadLocations, ByVal pathInDownloadLocation As String, ByVal folderInVirtualDownloadLocation As String, ByVal fileName As String)
            If Me.IsFullyFeatured Then
                Select Case downloadLocation
                    Case DownloadLocations.PublicCache
                        Dim cachedFileFullPath As String = Me.DownloadFolderFullPath & Me.ResolveSubFolderFromDownloadLocation(downloadLocation) & "/" & pathInDownloadLocation
                        If folderInVirtualDownloadLocation = Nothing Then
                            cachedFileFullPath &= System.IO.Path.DirectorySeparatorChar & fileName
                        Else
                            cachedFileFullPath &= System.IO.Path.DirectorySeparatorChar & folderInVirtualDownloadLocation & System.IO.Path.DirectorySeparatorChar & fileName
                        End If
                        cachedFileFullPath = cachedFileFullPath.Replace("//", "/").Replace("//", "/")
                        cachedFileFullPath = cachedFileFullPath.Replace("\\", "\").Replace("\\", "\")
                        Dim fi As New FileInfo(cachedFileFullPath)
                        If fi.Exists Then
                            fi.Delete()
                        End If
                    Case Else
                        Dim filePath As String = pathInDownloadLocation
                        If folderInVirtualDownloadLocation = Nothing Then
                            filePath &= System.IO.Path.DirectorySeparatorChar & fileName
                        Else
                            filePath &= System.IO.Path.DirectorySeparatorChar & folderInVirtualDownloadLocation & System.IO.Path.DirectorySeparatorChar & fileName
                        End If
                        filePath = filePath.Replace("//", "/").Replace("//", "/")
                        filePath = filePath.Replace("\\", "\").Replace("\\", "\")

                        Dim downloadDir As New DirectoryInfo(HttpContext.Current.Server.MapPath(SystemDownloadFolderForTemporaryFiles))

                        Me.RemoveFileFromDownloadLocations(downloadDir, filePath, downloadDir.FullName)

                End Select
            End If
        End Sub
        ''' <summary>
        '''     Removes download files from cache. This is a recursive method, it traverse through all inner directories.
        ''' </summary>
        ''' <param name="directoryInfo">System download directory object for temprary files, e.g. c:\inetpub\wwwroot\system\downloads\. This paramter will get initialized to inner directory at every recursive call.</param>
        ''' <param name="filePath">Virtual file path to be removed i.e. pathInDownloadLocation + folderInVirtualDownloadlocation + file name</param>
        ''' <param name="rootPath">System download directory full name for temprary files, e.g. c:\inetpub\wwwroot\system\downloads\</param>
        ''' <remarks>
        '''     Removes download files from cache i.e. from downloadlocation 
        '''     WebServerSession, WebManagerUserSession and WebManagerSecurityObjectName.
        ''' </remarks>
        Private Sub RemoveFileFromDownloadLocations(ByVal directoryInfo As DirectoryInfo, ByVal filePath As String, ByVal rootPath As String)
            For Each di As DirectoryInfo In directoryInfo.GetDirectories
                Me.RemoveFileFromDownloadLocations(di, filePath, rootPath)
            Next
            For Each fi As FileInfo In directoryInfo.GetFiles
                Dim _filePath As String = fi.FullName
                _filePath = _filePath.Replace(rootPath, "")
                If _filePath.Chars(0) = Path.DirectorySeparatorChar Then
                    _filePath = _filePath.Substring(1)
                End If
                _filePath = _filePath.Substring(_filePath.IndexOf(Path.DirectorySeparatorChar) + 1)
                _filePath = _filePath.Substring(_filePath.IndexOf(Path.DirectorySeparatorChar) + 1)
                If _filePath.Chars(0) = Path.DirectorySeparatorChar Then
                    _filePath = _filePath.Substring(1)
                End If
                If _filePath = filePath Then
                    fi.Delete()
                End If
            Next
        End Sub



#Region "Class AllFilesInDirectory"
        ''' <summary>
        '''     Inherites abstract class "CollectionBase". 
        ''' </summary>
        ''' <remarks>
        '''     The object of this class contains list of all files in directory structure 
        '''     defined by "directoryInfo" as "FileInfo" object.
        ''' </remarks>
        Private Class AllFilesInDirectory
            Inherits CollectionBase

            Public Sub New(ByVal dirInfo As DirectoryInfo)
                Me.getAllFilesInDirectory(dirInfo)
            End Sub
            Private Sub getAllFilesInDirectory(ByVal directoryInfo As DirectoryInfo)
                For Each di As DirectoryInfo In directoryInfo.GetDirectories
                    Me.getAllFilesInDirectory(di)
                Next
                For Each fi As FileInfo In directoryInfo.GetFiles
                    Me.InnerList.Add(fi)
                Next
            End Sub

        End Class
#End Region

#Region "Class AllDirsInDirectory"
        ''' <summary>
        '''     Inherites abstract class "CollectionBase". 
        ''' </summary>
        ''' <remarks>
        '''     The object of this class contains list of all directories in directory structure
        '''     defined by "directoryInfo" as "DirectoryInfo" object.
        ''' </remarks>
        Private Class AllDirsInDirectory
            Inherits CollectionBase

            Public Sub New(ByVal dirInfo As DirectoryInfo)
                Me.getAllDirsInDirectory(dirInfo)
            End Sub
            Private Sub getAllDirsInDirectory(ByVal dirInfo As DirectoryInfo)
                For Each di As DirectoryInfo In dirInfo.GetDirectories
                    Me.getAllDirsInDirectory(di)
                    Me.InnerList.Add(di)
                Next
            End Sub
        End Class
#End Region

#End Region

#Region " Properties "
        ''' <summary>
        '''     Returns the physical download folder path.
        ''' </summary>
        ''' <value></value>
        Private ReadOnly Property DownloadFolderFullPath() As String
            Get
                Dim returnPath As String
                If Not IsWebApplication Then
                    'non-web-app
                    returnPath = System.IO.Path.GetTempPath & System.IO.Path.DirectorySeparatorChar & Me.SystemDownloadFolderForTemporaryFiles
                    returnPath = returnPath.Replace("/", System.IO.Path.DirectorySeparatorChar)
                    returnPath = returnPath.Replace(System.IO.Path.DirectorySeparatorChar & System.IO.Path.DirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
                Else
                    'web-app
                    returnPath = HttpContext.Current.Server.MapPath(Me.SystemDownloadFolderForTemporaryFiles & EncryptedValueCurrentServerIdentString & "/")
                End If
                Return returnPath
            End Get
        End Property
        ''' <summary>
        '''     Returns value indicating whether calling application is Web.
        ''' </summary>
        ''' <value></value>
        Friend ReadOnly Property IsWebApplication() As Boolean
            Get
                If HttpContext.Current Is Nothing Then
                    Return False
                Else
                    Return True
                End If
            End Get
        End Property
        ''' <summary>
        '''     The current server identification string of camm Web-Manager or otherwise the name of the current host
        ''' </summary>
        ''' <value></value>
        Private ReadOnly Property CurrentServerIdentString() As String
            Get
                Dim Result As String
                If Me._WebManager.CurrentServerIdentString = Nothing Then
                    If Me.IsWebApplication Then
                        'web application
                        Result = HttpContext.Current.Request.Url.Host
                        If Not HttpContext.Current.Request.Url.IsDefaultPort Then
                            Result &= Uri.SchemeDelimiter & HttpContext.Current.Request.Url.Port
                        End If
                    Else
                        'non-web application
                        Result = CompuMaster.camm.WebManager.Utils.GetWorkstationID
                    End If
                Else
                    Result = Me._WebManager.CurrentServerIdentString
                End If
                Return Result
            End Get
        End Property
        ''' <summary>
        '''     A server ID reported by camm Web-Manager or otherwise a virtual, negative ID value calculated on the letters of the host name
        ''' </summary>
        ''' <value></value>
        Private ReadOnly Property ServerID() As Integer
            Get
                If Me._WebManager.CurrentServerIdentString <> Nothing Then
                    Return Me._WebManager.CurrentServerInfo.ID
                Else
                    Dim ServerName As String = CurrentServerIdentString
                    Dim Letters As Byte() = System.Text.UnicodeEncoding.Unicode.GetBytes(ServerName)
                    Dim Result As Integer
                    For MyCounter As Integer = 0 To Letters.Length - 1
                        Result += Letters(MyCounter)
                    Next
                    Return Result * -1
                End If
            End Get
        End Property

        Private _MaxDownloadCollectionSize As Long = WMSystem.Configuration.DownloadHandlerMaxFileCollectionSize
        ''' <summary>
        '''     Defines maximum download collection size allowed in bytes.
        ''' </summary>
        ''' <value></value>
        Public Property MaxDownloadCollectionSize() As Long
            Get
                Return Me._MaxDownloadCollectionSize
            End Get
            Set(ByVal Value As Long)
                Me._MaxDownloadCollectionSize = Value
            End Set
        End Property

        Private _MaxDownloadSize As Long = WMSystem.Configuration.DownloadHandlerMaxFileSize
        ''' <summary>
        '''     Defines maximum download size(in bytes) allowed.
        ''' </summary>
        Public Property MaxDownloadSize() As Long
            Get
                Return Me._MaxDownloadSize
            End Get
            Set(ByVal Value As Long)
                Me._MaxDownloadSize = Value
            End Set
        End Property

        Private _GeneralTimeLimitForFiles As TimeSpan = New TimeSpan(1, 0, 0, 0)
        ''' <summary>
        '''     Default live time for the download collection.
        ''' </summary>
        Public Property GeneralTimeLimitForFiles() As TimeSpan
            Get
                Return Me._GeneralTimeLimitForFiles
            End Get
            Set(ByVal Value As TimeSpan)
                Me._GeneralTimeLimitForFiles = Value
            End Set
        End Property


        Private _TimeLimitForPublicCache As TimeSpan = New TimeSpan(90, 0, 0, 0)
        ''' <summary>
        '''     Default live time for the public cache.
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TimeLimitForPublicCache() As TimeSpan
            Get
                Return Me._TimeLimitForPublicCache
            End Get
        End Property

        Private _RemoveFilesInWebManagerUserSessionImmediatelyAfterLogout As Boolean = True
        Public Property RemoveFilesInWebManagerUserSessionImmediatelyAfterLogout() As Boolean
            Get
                Return Me._RemoveFilesInWebManagerUserSessionImmediatelyAfterLogout
            End Get
            Set(ByVal Value As Boolean)
                Me._RemoveFilesInWebManagerUserSessionImmediatelyAfterLogout = Value
            End Set
        End Property

#End Region

#Region "Internal class FileStream with Dispose"
        Friend Class ManagedFileStream
            Implements IDisposable

            Private _BaseFileStream As System.IO.FileStream

            Public Sub New(ByVal fileStream As System.IO.FileStream)
                _BaseFileStream = fileStream
            End Sub

            Public Sub Write(ByVal array As Byte(), ByVal offset As Integer, ByVal count As Integer)
                _BaseFileStream.Write(array, offset, count)
            End Sub

            Public Function Read(ByVal array As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
                Return _BaseFileStream.Read(array, offset, count)
            End Function

            Public ReadOnly Property length() As Long
                Get
                    Return _BaseFileStream.Length
                End Get
            End Property

            Public ReadOnly Property BaseFileStream() As System.IO.FileStream
                Get
                    Return _BaseFileStream
                End Get
            End Property

            Private Sub Close()
                Try
                    _BaseFileStream.Close()
                Catch
                End Try
            End Sub

            Private disposedValue As Boolean = False        ' So ermitteln Sie ?berfl?ssige Aufrufe

            ' IDisposable
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        'verwaltete Objekte freigeben
                    End If

                    Me.Close()
                    CType(_BaseFileStream, IDisposable).Dispose()
                End If
                Me.disposedValue = True
            End Sub

#Region " IDisposable Support "
            ' Dieser Code wird von Visual Basic hinzugef?gt, um das Dispose-Muster richtig zu implementieren.
            Public Sub Dispose() Implements IDisposable.Dispose
                ' ?ndern Sie diesen Code nicht. F?gen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region

        End Class
#End Region

    End Class

#Region "Exceptions"
    ''' <summary>
    '''     The exception that is thrown when a download size exceeds limit.
    ''' </summary>
    Public Class DownloadHandlerFileSizeLimitException
        Inherits Exception

        Public Sub New()
            MyBase.New("Maximum file size to download is exceeded.")
        End Sub

        Public Sub New(ByVal path As String)
            MyBase.New("Maximum file size to download is exceeded by """ & path & """.")
        End Sub

    End Class

    ''' <summary>
    '''     The exception that is thrown when a download collection exceeds limit.
    ''' </summary>
    Public Class DownloadHandlerCollectionSizeLimitException
        Inherits Exception

        Public Sub New()
            MyBase.New("Maximum collection size to download is exceeded.")
        End Sub
        ''' <summary>
        '''     Message to get more infromation
        ''' </summary>
        Public Sub New(ByVal currentMaxCollectionSize As Long, ByVal collectionSizeTriedToDownload As Long)
            MyBase.New("Maximum collection size to download is exceeded. " & _
                        "Current MaxCollectionSize = " & currentMaxCollectionSize & _
                        " CollectionSize tried to download = " & collectionSizeTriedToDownload)
        End Sub

    End Class

    ''' <summary>
    '''     The exception that is thrown when a download requires full features respectively write access to the working folder
    ''' </summary>
    Public Class DownloadHandlerNotSupportedException
        Inherits Exception

        Public Sub New()
            MyBase.New("Download handler has to run in fully featured mode for this task.")
        End Sub

    End Class

    ''' <summary>
    '''     This exception is thrown when Security Object for camm WebManager is undefined.
    ''' </summary>
    Public Class EmptySecurityObjectException
        Inherits Exception

        Public Sub New()
            MyBase.New("Security object for camm Web Manager is Nothing, but it is required for this cache mode.")
        End Sub

    End Class

    <Obsolete("Use EmptySecurityObjectException instead"), ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
    Public Class SecurityObjectIsNothingException
        Inherits EmptySecurityObjectException

    End Class

#End Region

End Namespace
