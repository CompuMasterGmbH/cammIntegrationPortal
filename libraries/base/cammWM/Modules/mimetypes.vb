'Copyright 2005-2006,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

#If Not Linux Then
Imports Microsoft.Win32
Imports System.Security.Permissions
Imports System.IO
#End If

'ToDo 4 DotNetFramework V2: Look at System.Net.Mime ContentType (System.Net.Mime MediaTypeNames)

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     MIME type resolutions required for downloads to the browser
    ''' </summary>
    Public Class MimeTypes
        ''' <summary>
        '''     The content-type of a full mime type information
        ''' </summary>
        ''' <param name="mimeType"></param>
        ''' <example>
        '''     A mime type with &quot;image/jpeg&quot; has got a content-type &quot;image&quot; which will be returned
        ''' </example>
        Public Shared Function ContentType(ByVal mimeType As String) As String
            Return mimeType.Substring(0, mimeType.LastIndexOf("/"))
        End Function
        ''' <summary>
        '''     The sub-type of a full mime type information
        ''' </summary>
        ''' <param name="mimeType"></param>
        ''' <example>
        '''     A mime type with &quot;image/jpeg&quot; has got a sub-type &quot;jpeg&quot; which will be returned
        ''' </example>
        Public Shared Function SubType(ByVal mimeType As String) As String
            Return mimeType.Substring(mimeType.LastIndexOf("/") + 1)
        End Function
        ''' <summary>
        '''     Resolve the MIME type by a file extension
        ''' </summary>
        ''' <param name="fileExtension">The extension which leads to the returned MIME type</param>
        ''' <returns>A MIME type</returns>
        ''' <example>
        '''     <list>
        '''     <item>.jpg leads to &quot;image/jpeg&quot;</item>
        '''     <item>.bmp leads to &quot;image/bmp&quot;</item>
        '''     <item>.psd leads to &quot;image/psd&quot;</item>
        '''     <item>.zip leads to &quot;application/zip&quot;</item>
        '''     <item>.tiff leads to &quot;image/tiff&quot;</item>
        '''     <item>.avi leads to &quot;video/x-msvideo&quot;</item>
        '''     <item>.ppt leads to &quot;application/mspowerpoint&quot;</item>
        '''     </list>
        ''' </example>
        Public Shared Function MimeTypeByFileExtension(ByVal fileExtension As String) As String

            If Mid(fileExtension, 1, 1) = "." Then
                fileExtension = Mid(fileExtension, 2)
            End If
            Select Case LCase(fileExtension)
                Case "jpe" : Return "image/jpeg"
                Case "jpeg" : Return "image/jpeg"
                Case "jpg" : Return "image/jpeg"
                Case "bmp" : Return "image/bmp"
                Case "psd" : Return "image/psd"
                Case "gif" : Return "image/gif"
                Case "tif" : Return "image/tiff"
                Case "tiff" : Return "image/tiff"
                Case "png" : Return "image/png"
                Case "ai" : Return "application/postscript"
                Case "aif" : Return "audio/x-aiff"
                Case "aifc" : Return "audio/x-aiff"
                Case "aiff" : Return "audio/x-aiff"
                Case "asc" : Return "text/plain"
                Case "au" : Return "audio/basic"
                Case "avi" : Return "video/x-msvideo"
                Case "bcpio" : Return "application/x-bcpio"
                Case "bin" : Return "application/octet-stream"
                Case "c" : Return "text/plain"
                Case "cc" : Return "text/plain"
                Case "ccad" : Return "application/clariscad"
                Case "cdf" : Return "application/x-netcdf"
                Case "class" : Return "application/octet-stream"
                Case "cpio" : Return "application/x-cpio"
                Case "cpt" : Return "application/mac-compactpro"
                Case "csh" : Return "application/x-csh"
                Case "css" : Return "text/css"
                Case "dcr" : Return "application/x-director"
                Case "dir" : Return "application/x-director"
                Case "divx" : Return "video/x-msvideo"
                Case "dms" : Return "application/octet-stream"
                Case "doc" : Return "application/msword"
                Case "drw" : Return "application/drafting"
                Case "dvi" : Return "application/x-dvi"
                Case "dwg" : Return "application/acad"
                Case "dxf" : Return "application/dxf"
                Case "dxr" : Return "application/x-director"
                Case "eps" : Return "application/postscript"
                Case "etx" : Return "text/x-setext"
                Case "exe" : Return "application/octet-stream"
                Case "ez" : Return "application/andrew-inset"
                Case "f" : Return "text/plain"
                Case "f90" : Return "text/plain"
                Case "fli" : Return "video/x-fli"
                Case "gtar" : Return "application/x-gtar"
                Case "gz" : Return "application/x-gzip"
                Case "gzip" : Return "application/x-gzip"
                Case "h" : Return "text/plain"
                Case "hdf" : Return "application/x-hdf"
                Case "hh" : Return "text/plain"
                Case "hqx" : Return "application/mac-binhex40"
                Case "htm" : Return "text/html"
                Case "html" : Return "text/html"
                Case "ice" : Return "x-conference/x-cooltalk"
                Case "ief" : Return "image/ief"
                Case "iges" : Return "application/iges"
                Case "igs" : Return "application/iges"
                Case "ips" : Return "application/x-ipscript"
                Case "ipx" : Return "application/x-ipix"
                Case "js" : Return "application/x-javascript"
                Case "kar" : Return "audio/midi"
                Case "latex" : Return "application/x-latex"
                Case "lha" : Return "application/octet-stream"
                Case "lsp" : Return "application/x-lisp"
                Case "lzh" : Return "application/octet-stream"
                Case "m" : Return "text/plain"
                Case "man" : Return "application/x-troff-man"
                Case "me" : Return "application/x-troff-me"
                Case "mesh" : Return "model/mesh"
                Case "mid" : Return "audio/midi"
                Case "midi" : Return "audio/midi"
                Case "mif" : Return "application/vnd.mif"
                Case "mime" : Return "www/mime"
                Case "mov" : Return "video/quicktime"
                Case "movie" : Return "video/x-sgi-movie"
                Case "mp2" : Return "audio/mpeg"
                Case "mp3" : Return "audio/mpeg"
                Case "mpe" : Return "video/mpeg"
                Case "mpeg" : Return "video/mpeg"
                Case "mpg" : Return "video/mpeg"
                Case "mpga" : Return "audio/mpeg"
                Case "ms" : Return "application/x-troff-ms"
                Case "msh" : Return "model/mesh"
                Case "nc" : Return "application/x-netcdf"
                Case "oda" : Return "application/oda"
                Case "pbm" : Return "image/x-portable-bitmap"
                Case "pdb" : Return "chemical/x-pdb"
                Case "pdf" : Return "application/pdf"
                Case "pgm" : Return "image/x-portable-graymap"
                Case "pgn" : Return "application/x-chess-pgn"
                Case "pnm" : Return "image/x-portable-anymap"
                Case "pot" : Return "application/mspowerpoint"
                Case "ppm" : Return "image/x-portable-pixmap"
                Case "pps" : Return "application/mspowerpoint"
                Case "ppt" : Return "application/mspowerpoint"
                Case "ppz" : Return "application/mspowerpoint"
                Case "pre" : Return "application/x-freelance"
                Case "prt" : Return "application/pro_eng"
                Case "ps" : Return "application/postscript"
                Case "qt" : Return "video/quicktime"
                Case "ra" : Return "audio/x-realaudio"
                Case "ram" : Return "audio/x-pn-realaudio"
                Case "ras" : Return "image/cmu-raster"
                Case "rgb" : Return "image/x-rgb"
                Case "rm" : Return "audio/x-pn-realaudio"
                Case "roff" : Return "application/x-troff"
                Case "rpm" : Return "audio/x-pn-realaudio-plugin"
                Case "rtf" : Return "text/richtext"
                Case "rtx" : Return "text/richtext"
                Case "scm" : Return "application/x-lotusscreencam"
                Case "set" : Return "application/set"
                Case "sgm" : Return "text/sgml"
                Case "sgml" : Return "text/sgml"
                Case "sh" : Return "application/x-sh"
                Case "shar" : Return "application/x-shar"
                Case "silo" : Return "model/mesh"
                Case "sit" : Return "application/x-stuffit"
                Case "skd" : Return "application/x-koan"
                Case "skm" : Return "application/x-koan"
                Case "skp" : Return "application/x-koan"
                Case "skt" : Return "application/x-koan"
                Case "smi" : Return "application/smil"
                Case "smil" : Return "application/smil"
                Case "snd" : Return "audio/basic"
                Case "sol" : Return "application/solids"
                Case "spl" : Return "application/x-futuresplash"
                Case "src" : Return "application/x-wais-source"
                Case "step" : Return "application/STEP"
                Case "stl" : Return "application/SLA"
                Case "stp" : Return "application/STEP"
                Case "sv4cpio" : Return "application/x-sv4cpio"
                Case "sv4crc" : Return "application/x-sv4crc"
                Case "swf" : Return "application/x-shockwave-flash"
                Case "t" : Return "application/x-troff"
                Case "tar" : Return "application/x-tar"
                Case "tcl" : Return "application/x-tcl"
                Case "tex" : Return "application/x-tex"
                Case "texi" : Return "application/x-texinfo"
                Case "texinfo" : Return "application/x-texinfo"
                Case "tr" : Return "application/x-troff"
                Case "tsi" : Return "audio/TSP-audio"
                Case "tsp" : Return "application/dsptype"
                Case "tsv" : Return "text/tab-separated-values"
                Case "txt" : Return "text/plain"
                Case "unv" : Return "application/i-deas"
                Case "ustar" : Return "application/x-ustar"
                Case "vcd" : Return "application/x-cdlink"
                Case "vda" : Return "application/vda"
                Case "viv" : Return "video/vnd.vivo"
                Case "vivo" : Return "video/vnd.vivo"
                Case "vrml" : Return "model/vrml"
                Case "wav" : Return "audio/x-wav"
                Case "wrl" : Return "model/vrml"
                Case "xbm" : Return "image/x-xbitmap"
                Case "xlc" : Return "application/vnd.ms-excel"
                Case "xll" : Return "application/vnd.ms-excel"
                Case "xlm" : Return "application/vnd.ms-excel"
                Case "xls" : Return "application/vnd.ms-excel"
                Case "xlw" : Return "application/vnd.ms-excel"
                Case "xml" : Return "text/xml"
                Case "xpm" : Return "image/x-xpixmap"
                Case "xwd" : Return "image/x-xwindowdump"
                Case "zip" : Return "application/zip"

                Case Else
                    Return ResolveMimeTypeByLocalPlatform("." & LCase(fileExtension))

            End Select
        End Function
        ''' <summary>
        '''     Query the windows registry for a MIME type of an extension
        ''' </summary>
        ''' <param name="extension">The file name extension with a leading dot (&quot;.&quot;)</param>
        ''' <returns>A MIME type like &quot;image/jpeg&quot;</returns>
        Private Shared Function ResolveMimeTypeByLocalPlatform(ByVal extension As String) As String
#If Not Linux Then
            Select Case System.Environment.OSVersion.Platform
                Case PlatformID.Win32NT, PlatformID.Win32S, PlatformID.Win32Windows, PlatformID.WinCE
                    'running on Windows
                    If extension <> "" Then
                        Try
                            Dim regPerm As RegistryPermission = New RegistryPermission(RegistryPermissionAccess.Read, "\\HKEY_CLASSES_ROOT")
                            Dim classesRoot As RegistryKey = Registry.ClassesRoot
                            Dim typeKey As RegistryKey = classesRoot.OpenSubKey("MIME\Database\Content Type")
                            Dim keyname As String

                            For Each keyname In typeKey.GetSubKeyNames()
                                Dim curKey As RegistryKey = classesRoot.OpenSubKey("MIME\Database\Content Type\" & keyname)
                                Dim foundValue As String = LCase(CType(curKey.GetValue("Extension"), String))
                                curKey.Close()
                                If foundValue = extension Then
                                    Return keyname
                                End If
                            Next
                            classesRoot.Close()
                            typeKey.Close()
                        Catch
                        End Try
                    End If
                    Return "application/octet-stream"
                Case Else
                    'running on non-windows platform --> no registry is available
                    Return "application/octet-stream"
            End Select
#Else
            'compilation for non-windows platform --> no registry and no namespace "Microsoft" are available
            Return "application/octet-stream"
#End If
        End Function
        ''' <summary>
        '''     Resolve the MIME type by a file name
        ''' </summary>
        ''' <param name="fileName"></param>
        ''' <returns>A MIME type</returns>
        ''' <example>
        '''     <list>
        '''     <item>.jpg leads to &quot;image/jpeg&quot;</item>
        '''     <item>.bmp leads to &quot;image/bmp&quot;</item>
        '''     <item>.psd leads to &quot;image/psd&quot;</item>
        '''     <item>.zip leads to &quot;application/zip&quot;</item>
        '''     <item>.tiff leads to &quot;image/tiff&quot;</item>
        '''     <item>.avi leads to &quot;video/x-msvideo&quot;</item>
        '''     <item>.ppt leads to &quot;application/mspowerpoint&quot;</item>
        '''     </list>
        ''' </example>
        Public Shared Function MimeTypeByFileName(ByVal fileName As String) As String
            Return MimeTypeByFileExtension(System.IO.Path.GetExtension(fileName))
        End Function

    End Class

End Namespace