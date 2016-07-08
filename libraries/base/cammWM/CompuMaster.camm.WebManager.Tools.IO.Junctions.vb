'Copyright 2003-2004,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

'Entkoppelt von Ursprungs-Version durch geänderten Namespace

Imports System.Runtime.InteropServices
Imports System

Namespace CompuMaster.camm.WebManager.Tools.IO

    ''' <summary>
    '''     Filesystem Input/Output operations
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Friend Class NamespaceDoc
    End Class

    ''' <summary>
    '''     Hardlinks, Softlinks and Junctions
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Friend Class Junctions

#Region "Get link information details"
        ''' <summary>
        ''' Checks for existance of a linking file
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="LinkType"></param>
        ''' <returns>Returns true if the file exists and the file is a link to another file.</returns>
        Public Shared Function LinkingFileExists(ByVal Path As String, ByVal LinkType As LinkTypes) As Boolean
            'TODO: implementation
            Return False
        End Function

        ''' <summary>
        ''' Checks for existance of a linking folder
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="LinkType"></param>
        ''' <returns>Returns true if the folder exists and the folder is a link to another folder.</returns>
        Public Shared Function LinkingFolderExists(ByVal Path As String, ByVal LinkType As LinkTypes) As Boolean
            'TODO: implementation
            Return False
        End Function

        ''' <summary>
        ''' Checks if a file or folder is a sub element of an arbitrary hirarchy level of the folder structure on the current workstation
        ''' This information is helpfull to detect if a file or folder exists two or more times in reality and if e. g. a removal of this item would influence a second folder structure.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="LinkType"></param>
        ''' <returns>Returns true if this file or folder exists</returns>
        Public Shared Function ExistsAsItemInASubLevelOfAJunctionFolder(ByVal Path As String, ByVal LinkType As LinkTypes) As Boolean
            'TODO: implementation
            Return False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	07.07.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetLinkType(ByVal Path As String) As LinkTypeDirectives
            'TODO: reviewing
            If 1 = 1 Then
                Return LinkTypeDirectives.None
            ElseIf System.IO.File.Exists(Path) AndAlso System.IO.Path.GetExtension(Path).ToLower(System.Globalization.CultureInfo.InvariantCulture) = "lnk" Then
                'It's a windows file link
                Return LinkTypeDirectives.FileLink
            Else
                Return LinkTypeDirectives.None
            End If
        End Function
#End Region

#Region "Link handling"
        ''' <summary>
        ''' Creates a junction link
        ''' </summary>
        ''' <param name="ExistingFileSystemObject"></param>
        ''' <param name="NewLinkingFileSystemObject"></param>
        ''' <param name="LinkType"></param>
        Public Shared Sub Create(ByVal ExistingFileSystemObject As String, ByVal NewLinkingFileSystemObject As String, ByVal LinkType As LinkTypeDirectives)
            Dim NewLinkLocation As String = NewLinkingFileSystemObject
            Dim ExistingTargetLocation As String = ExistingFileSystemObject
            If System.Environment.OSVersion.Platform = System.PlatformID.Win32NT Then
                If (LinkType = LinkTypeDirectives.HardLink) Then
#If LINUX Then
                    Throw New NotSupportedException("Only available on camm Web-Manager for Windows NT systems (Linux support is in plannings)")
#Else
                    If GetFileSystem(NewLinkLocation) = "NTFS" And GetFileSystem(ExistingTargetLocation) = "NTFS" Then 'Check the volume type of both files; they must be the same
                        If System.IO.File.Exists(ExistingTargetLocation) Then
                            Dim DeletionCountOut As TriState = TriState.UseDefault
                            If System.IO.File.Exists(NewLinkLocation) Then
                                System.IO.File.Delete(NewLinkLocation)
                                DeletionCountOut = TriState.False
                                Dim TestCount As Integer = 0
                                Do
                                    GC.Collect(2)
                                    Threading.Thread.Sleep((250 * TestCount))
                                    TestCount += 1
                                Loop Until TestCount >= 5 OrElse System.IO.File.Exists(NewLinkLocation) = False
                                If System.IO.File.Exists(NewLinkLocation) = True Then DeletionCountOut = TriState.True
                            End If

                            Dim Result As Boolean = _CreateNTFSHardLinkWin(NewLinkLocation, ExistingTargetLocation, Nothing)
                            'If Result = False Then Result = _CreateNTFSHardLinkWinANSI(LinkLocation, TargetLocation, Nothing)
                            If Result = False Then
                                Dim DeletionWarning As String = Nothing
                                If DeletionCountOut = TriState.True Then
                                    DeletionWarning = "WARNING: Deleting a previous file at target location FAILED" & vbNewLine
                                ElseIf DeletionCountOut = TriState.False Then
                                    DeletionWarning = "INFO: A previous file at target location has been DELETED" & vbNewLine
                                Else
                                    DeletionWarning = "INFO: No previous file existed" & vbNewLine
                                End If
                                Dim Win32ErrorMessage As System.ComponentModel.Win32Exception = GetWin32ErrorMessage()
                                If Win32ErrorMessage Is Nothing Then
                                    Throw New Exception(DeletionWarning & "Creation of hard file link failed; source and destination files must reside on the same NTFS volume")
                                Else
                                    Throw New Exception(DeletionWarning & _
                                        "Win32 Fehlermeldung: '" & Win32ErrorMessage.ToString & "'" & vbNewLine & _
                                        "Hardlink-Sourcefile: '" & ExistingFileSystemObject & "'" & vbNewLine & _
                                        "Hardlink-Targetfile: '" & NewLinkingFileSystemObject & "'")
                                End If
                            ElseIf System.IO.File.Exists(ExistingTargetLocation) = False Then
                                Dim TestCount As Integer = 0
                                Do
                                    GC.Collect(2)
                                    Threading.Thread.Sleep((250 * TestCount))
                                    TestCount += 1
                                Loop Until TestCount >= 5 OrElse System.IO.File.Exists(NewLinkLocation) = True
                                If System.IO.File.Exists(ExistingTargetLocation) = False Then
                                    Log.WriteEventLogTrace("CreateNTFSHardLink successful, but target file still missing", System.Diagnostics.EventLogEntryType.Error)
                                    Throw New System.IO.IOException("CreateNTFSHardLink successful, but new target file link is still missing")
                                End If
                            End If
                        ElseIf System.IO.Directory.Exists(ExistingTargetLocation) Then
                            Throw New System.Exception("Junctions haven't been supported yet.")
                        Else
                            Throw New System.Exception("Link target doesn't exist")
                        End If
                    Else
                        Throw New System.Exception("NTFS volume required")
                    End If
#End If
                ElseIf (LinkType = LinkTypeDirectives.SoftLink) Then
                    If System.IO.File.Exists(ExistingTargetLocation) Then
                        Throw New System.Exception("Soft links haven't been supported yet.")
                    ElseIf System.IO.Directory.Exists(ExistingTargetLocation) Then
                        Throw New System.Exception("Soft links haven't been supported yet.")
                    Else
                        Throw New System.Exception("Link target doesn't exist")
                    End If
                ElseIf (LinkType = LinkTypeDirectives.FileLink) Then
                    'WshShell = WScript.CreateObject("WScript.Shell")
                    'strDesktop = WshShell.SpecialFolders("Desktop")
                    'oShellLink = WshShell.CreateShortcut(strDesktop & "\Shortcut Script.lnk")
                    'oShellLink.TargetPath = WScript.ScriptFullName
                    'oShellLink.WindowStyle = 1
                    'oShellLink.Hotkey = "CTRL+SHIFT+F"
                    'oShellLink.IconLocation = "notepad.exe, 0"
                    'oShellLink.Description = "Shortcut Script"
                    'oShellLink.WorkingDirectory = strDesktop
                    'oShellLink.Save()
                    If System.IO.File.Exists(ExistingTargetLocation) Then
                        Throw New System.Exception("File links haven't been supported yet.")
                    ElseIf System.IO.Directory.Exists(ExistingTargetLocation) Then
                        Throw New System.Exception("Folders can never be a file link.")
                    Else
                        Throw New System.Exception("Link target doesn't exist")
                    End If
                ElseIf LinkType = LinkTypeDirectives.None Then
                    Throw New System.Exception("Creation of non-linking file system objects not supported by this method. Use the standard methods instead.")
                Else
                    Throw New System.Exception("Invalid parameter LinkType")
                End If
            Else
                Throw New NotSupportedException("Only available on Windows NT systems (Linux support is in plannings)")
            End If
            'Validate the result
            If Not System.IO.File.Exists(NewLinkLocation) AndAlso Not System.IO.Directory.Exists(NewLinkLocation) Then
                Throw New System.Exception("Unexpected error while creating file system object")
            End If
        End Sub

#If Not LINUX Then
        Public Shared Function GetWin32ErrorMessage() As System.ComponentModel.Win32Exception
            If Err.LastDllError <> 0 Then
                Return New System.ComponentModel.Win32Exception(Err.LastDllError)
            Else
                Return Nothing
            End If
        End Function
#End If
        ''' <summary>
        ''' Deletes a link
        ''' </summary>
        ''' <param name="linkLocation"></param>
        Public Shared Sub Delete(ByVal linkLocation As String)
            Delete(linkLocation, LinkTypeDirectives.Automatic)
        End Sub

        ''' <summary>
        ''' Deletes a given LinkType link
        ''' </summary>
        ''' <param name="LinkLocation"></param>
        ''' <param name="LinkType"></param>
        Public Shared Sub Delete(ByVal LinkLocation As String, ByVal LinkType As LinkTypeDirectives)
#If LINUX Then
            Throw New NotImplementedException("Not implemented in camm Web-Manager Linux edition")
#Else
            If System.Environment.OSVersion.Platform = System.PlatformID.Win32NT Then
                If GetFileSystem(LinkLocation) = "NTFS" Then
                    Select Case LinkType
                        Case LinkTypeDirectives.Automatic
                            If System.IO.File.Exists(LinkLocation) Then
                                System.IO.File.Delete(LinkLocation)
                            ElseIf System.IO.Directory.Exists(LinkLocation) Then
                                LinkLocation = System.IO.Path.GetDirectoryName(LinkLocation)
                                System.IO.Directory.Delete(LinkLocation)
                            Else
                                Throw New System.Exception("File or folder doesn't exists")
                            End If
                        Case LinkTypeDirectives.FileLink
                            Throw New System.Exception("Explicitly erasing of file links hasn't been supported")
                        Case LinkTypeDirectives.HardLink
                            Throw New System.Exception("Explicitly erasing of hard/junction links hasn't been supported")
                        Case LinkTypeDirectives.SoftLink
                            Throw New System.Exception("Explicitly erasing of soft links hasn't been supported")
                        Case LinkTypeDirectives.None
                            Throw New System.Exception("Explicitly erasing of normal file system objects and ONLY normal file system objects hasn't been supported")
                        Case Else
                            Throw New System.Exception("Link object doesn't exist")
                    End Select
                Else
                    Throw New System.Exception("NTFS volume required")
                End If
            Else
                Throw New NotSupportedException("Only available on Windows NT systems (Linux support is in plannings)")
            End If
#End If
        End Sub

        ''' <summary>
        ''' None: Real file (copy) 
        ''' Auto: Automatic decision in following order: hard link, soft link, file link.
        ''' HardLink: Also JunctionLinks, only available on local NTFS volumes, the NTFS does all the work. This technique is transparent to windows and all applications.
        ''' SoftLink: Soft links are not .LNK files! They can appear in windows explorer as normal folders/files. Windows handles the soft links, not the files system. Applications may need to support soft links to use their full functionality.
        ''' FileLink: File links are the well known .LNK files which you see in your windows start menu for example.
        ''' </summary>
        Public Enum LinkTypeDirectives As Byte
            None = 255
            Automatic = 0
            HardLink = 1
            SoftLink = 2
            FileLink = 3
        End Enum

        ''' <summary>
        ''' HardLink: Also JunctionLinks, only available on local NTFS volumes, the NTFS does all the work. This technique is transparent to windows and all applications.
        ''' SoftLink: Soft links are not .LNK files! They can appear in windows explorer as normal folders/files. Windows handles the soft links, not the files system. Applications may need to support soft links to use their full functionality.
        ''' FileLink: File links are the well known .LNK files which you see in your windows start menu for example.
        ''' </summary>
        Public Enum LinkTypes As Byte
            HardLink = 1
            SoftLink = 2
            FileLink = 3
        End Enum

#If Not LINUX Then
        ''' <summary>
        ''' GefFileSystem returns the file system type of root directory of the the given path (e. g. from D: instead of D:\temp\).
        ''' This problematic behaviour should be fixed in a later version and should return the file system of the given path itself (to support volumes without drive letters in Win2K or newer and Linux, too).
        ''' </summary>
        ''' <param name="Path">Path to get the file system type of</param>
        ''' <returns>One of the following file system types: "NTFS", "" (for any unknown/unsupported file systems)</returns>
        Public Shared Function GetFileSystem(ByVal Path As String) As String
            Dim result As String = Nothing
            If System.Environment.OSVersion.Platform = System.PlatformID.Win32NT Then
                'ask the system
                Path = System.IO.Directory.GetDirectoryRoot(Path)
                Dim DriveLetter As String = Path.Substring(0, Path.IndexOf(System.IO.Path.VolumeSeparatorChar) + 1)
                Dim myMOC As System.Management.ManagementObjectCollection = (New System.Management.ManagementObjectSearcher(New System.Management.SelectQuery("SELECT VolumeName, FileSystem FROM Win32_LogicalDisk WHERE deviceID = '" + DriveLetter + "'"))).Get()
                Try
                    Dim itemsfound As Boolean = False
                    Dim myMO As System.Management.ManagementObject
                    For Each myMO In myMOC
                        itemsfound = True
                        result = myMO.Properties("FileSystem").Value.ToString()
                    Next
                    If itemsfound = False Then
                        Throw New Exception("File system information not available for network shares")
                    End If
                Catch ex As System.Exception
                    Throw New Exception("Junction engine internal error / no such device or device is empty", ex)
                Finally
                    myMOC.Dispose()
                End Try

                'preserve correct results on every supported platform
                If result = "NTFS" Then
                    Return result
                Else
                    Return ""
                End If
            Else
                Throw New NotSupportedException("Only available on Windows NT systems (Linux is in plannings)")
            End If
        End Function
#End If

#End Region

#Region "Methods of external libraries"

#If Not LINUX Then
        'possibliy of interest for further development:
        'CreateDirectoryW
        'CreateDirectoryExA
        'CreateDirectoryExW
        'CreateHardLinkA
        'CreateHardLinkW
        <DllImport("kernel32.dll", CallingConvention:=CallingConvention.Winapi, EntryPoint:="CreateHardLinkW", CharSet:=CharSet.Unicode, SetLastError:=True)> _
        Private Shared Function _CreateNTFSHardLinkWin(ByVal NewFileName As String, ByVal ExistingFileName As String, ByVal lpSecurityAttributes As System.UInt16) As Boolean
        End Function

        <DllImport("kernel32.dll", CallingConvention:=CallingConvention.Winapi, EntryPoint:="CreateHardLinkA", CharSet:=CharSet.Ansi, SetLastError:=False)> _
        Private Shared Function _CreateNTFSHardLinkWinANSI(ByVal NewFileName As String, ByVal ExistingFileName As String, ByVal lpSecurityAttributes As System.UInt16) As Boolean
        End Function

        '<DllImport("kernel32.dll")> _
        'Private Shared Function GetVolumeInformation(ByVal PathName As String, ByVal VolumeNameBuffer As System.Text.StringBuilder, ByVal VolumeNameSize As UInt32, ByRef VolumeSerialNumber As UInt32, ByRef MaximumComponentLength As UInt32, ByRef FileSystemFlags As UInt32, ByVal FileSystemNameBuffer As System.Text.StringBuilder, ByVal FileSystemNameSize As UInt32) As Long
        'End Function

        '<DllImport("kernel32.dll", CallingConvention:=CallingConvention.Winapi, EntryPoint:="GetLastError", CharSet:=CharSet.Unicode)> _
        'Private Shared Function _GetLastError() As Long
        'End Function
#End If


#End Region

    End Class
End Namespace
