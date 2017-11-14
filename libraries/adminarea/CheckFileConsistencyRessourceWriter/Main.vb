Imports System.Resources
Imports System.Collections.Generic
Imports System.Security.Cryptography
Imports System.Text
Imports System.IO

Module Main

    Dim fl As New System.Collections.Generic.List(Of flInfo)
    Class flInfo
        Private Shared _MD5Hasher As MD5 = System.Security.Cryptography.MD5.Create

        Public Sub New(fileNameRelative As String, fileNameRooted As String)
            Me.FileName = fileNameRelative
            Dim fi As New System.IO.FileInfo(fileNameRelative)
            Me.FileSize = fi.Length
            Me.MD5 = _MD5Hasher.ComputeHash(System.IO.File.ReadAllBytes(fi.FullName))
        End Sub
        Public Property FileName As String
        Public Property FileSize As Integer
        Public Property MD5 As Byte()
    End Class

    Private VerboseLevel As VerboseLevels = VerboseLevels.FullDetails
    Private Enum VerboseLevels
        FullDetails = 2
        MostImportantInfoOnly = 1
        Quiet = 0 'TODO: ToBeImplemented in code below
    End Enum

    Sub Main(ByVal args() As String)

        Try

            Console.WriteLine(System.IO.Path.GetFileNameWithoutExtension(System.Environment.GetCommandLineArgs(0)) & " V" & System.Reflection.Assembly.GetEntryAssembly.GetName().Version.ToString)

            Console.WriteLine("Initialize commandline arguments")

            Dim source As String = "-s:"
            Dim destination As String = "-d:"
            For Each argument As String In args
                If argument.ToLower = "-v:2" Then
                    VerboseLevel = VerboseLevels.FullDetails
                ElseIf argument.ToLower = "-v:1" Then
                    VerboseLevel = VerboseLevels.MostImportantInfoOnly
                ElseIf argument.ToLower = "-v:0" Then
                    VerboseLevel = VerboseLevels.Quiet
                Else
                    If argument.ToLower.StartsWith("-s:") Then
                        source = argument.Remove(0, source.Length)
                    End If
                    If argument.ToLower.StartsWith("-d:") Then
                        destination = argument.Remove(0, destination.Length)
                    End If
                End If
            Next

            If source = "-s:" Then
                source = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location)
            End If
            If Not Directory.Exists(source) Then
                Dim Ex As New Exception("Source directory " & source & " does not exist")
                Dim sw As New StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location) & "\FileConsitencyRessourceWriter.log")
                sw.WriteLine(Ex.ToString)
                sw.Close()
                Throw Ex
            End If
            Console.WriteLine("Source directory is " & source)
            If destination = "-d:" Then
                destination = System.Environment.CurrentDirectory
            End If
            If Not Directory.Exists(destination) Then
                Dim Ex As New Exception("Source directory " & destination & " does not exist")
                Dim sw2 As New StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location) & "\FileConsitencyRessourceWriter.log")
                sw2.WriteLine(Ex.ToString)
                sw2.Close()
                Throw Ex
            End If

            Console.WriteLine("Destination directory is " & destination)

            If VerboseLevel = VerboseLevels.FullDetails Then Console.WriteLine(vbNewLine)
            Console.WriteLine("Get files ...")
            If VerboseLevel = VerboseLevels.FullDetails Then Console.WriteLine(vbNewLine)

            Dim dt As New DataTable("filesList")
            dt.Columns.Add("relativeFilePath", GetType(String))
            dt.Columns.Add("size", GetType(Integer))
            dt.Columns.Add("md5", GetType(Byte()))
            Dim ds As New DataSet("filesListDataSet")
            dirSearch(source, True)
            For Each fileItem As flInfo In fl
                Dim fileName As String = fileItem.FileName
                If VerboseLevel = VerboseLevels.FullDetails Then Console.WriteLine(fileName)
                Dim row As DataRow = dt.NewRow
                If fileName.IndexOf(".svn") = -1 Then
                    If fileName.IndexOf("\sysdata\") > 0 Then
                        row("relativeFilePath") = fileName.Remove(0, fileName.IndexOf("\sysdata\")).Replace("\", "/")
                    ElseIf fileName.IndexOf("\system\") > 0 Then
                        row("relativeFilePath") = fileName.Remove(0, fileName.IndexOf("\system\")).Replace("\", "/")
                    End If
                    If Not row("relativeFilePath") Is DBNull.Value Then
                        row("size") = fileItem.FileSize
                        row("md5") = fileItem.MD5
                        dt.Rows.Add(row)
                    End If
                End If
            Next
            ds.Tables.Add(dt)
            Console.WriteLine(dt.Rows.Count & " files have been found")

            Console.WriteLine("Write XML file ...")
            Dim sw3 As New StreamWriter(destination & "\fileList.xml")
            sw3.Write(CompuMaster.Data.DataTables.ConvertDatasetToXml(ds))
            sw3.Close()

            BuildRessourceFile(destination)

            Console.WriteLine("Cleanup of temporary XML file ...")
            System.IO.File.Delete(destination & "\fileList.xml")

            Console.WriteLine("Process finished successfully!")

        Catch ex As Exception
            Console.WriteLine("CRITICAL ERROR: " & ex.ToString)
        End Try

    End Sub

    Private Sub dirSearch(ByVal strDir As String, isInitLevel As Boolean)
        If strDir.IndexOf(".svn") = -1 Then
            If isInitLevel Then
                'only on very first call, take the files from root dir, too
                For Each strFile As String In Directory.GetFiles(strDir)
                    If VerboseLevel = VerboseLevels.FullDetails Then Console.WriteLine(strFile)
                    fl.Add(New flInfo(strFile, System.IO.Path.Combine(strDir, strFile)))
                Next
            End If
            For Each strDirectory As String In Directory.GetDirectories(strDir)
                For Each strFile As String In Directory.GetFiles(strDirectory)
                    If VerboseLevel = VerboseLevels.FullDetails Then Console.WriteLine(strFile)
                    fl.Add(New flInfo(strFile, System.IO.Path.Combine(strDirectory, strFile)))
                Next
                dirSearch(strDirectory, False)
            Next
        End If
    End Sub

    Private Sub BuildRessourceFile(ByVal destination As String)
        If Not File.Exists(destination & "\fileConsistencyList.resx") Then
            File.Create(destination & "\fileConsistencyList.resx")
            Console.WriteLine("Create file: " & destination & "\fileConsistencyList.resx")
        Else
            Console.WriteLine("Re-creating file: " & destination & "\fileConsistencyList.resx")
        End If
        Console.WriteLine("Set file attributes")
        System.IO.File.SetAttributes(destination & "\fileConsistencyList.resx", IO.FileAttributes.Normal)
        Console.WriteLine("Compress data and write into file ...")
        Dim rw As ResXResourceWriter = New ResXResourceWriter(destination & "\fileConsistencyList.resx")
        rw.AddResource("fileList.xml_bzip2", Compression.Compress(TextFileContent(destination & "\fileList.xml"), Compression.CompressionType.BZip2))
        rw.Close()

    End Sub
    Private Function TextFileContent(ByVal path As String) As String
        Dim Result As String
        Dim TextReader As System.IO.TextReader = New System.IO.StreamReader(path, System.Text.Encoding.Default, True)
        Result = TextReader.ReadToEnd
        TextReader.Close()
        Return Result
    End Function

#Region "Compression"
    Public Class Compression

        Public Enum CompressionType
            GZip
            BZip2
            Zip
        End Enum

        Private Shared Function OutputStream(ByVal InputStream As Stream, ByVal CompressionProvider As CompressionType) As Stream

            Select Case CompressionProvider
                Case CompressionType.BZip2
                    Return New ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(InputStream)

                Case CompressionType.GZip
                    Return New ICSharpCode.SharpZipLib.GZip.GZipOutputStream(InputStream)

                Case CompressionType.Zip
                    Return New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(InputStream)

                Case Else
                    Return New ICSharpCode.SharpZipLib.GZip.GZipOutputStream(InputStream)

            End Select
        End Function

        Private Shared Function InputStream(ByVal InStream As Stream, ByVal CompressionProvider As CompressionType) As Stream

            Select Case CompressionProvider
                Case CompressionType.BZip2
                    Return New ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(InStream)

                Case CompressionType.GZip
                    Return New ICSharpCode.SharpZipLib.GZip.GZipInputStream(InStream)

                Case CompressionType.Zip
                    Return New ICSharpCode.SharpZipLib.Zip.ZipInputStream(InStream)

                Case Else
                    Return New ICSharpCode.SharpZipLib.GZip.GZipInputStream(InStream)

            End Select

        End Function

        Public Shared Function Compress(ByVal bytesToCompress As Byte(), ByVal CompressionProvider As CompressionType) As Byte()
            Dim ms As MemoryStream = New MemoryStream
            Dim s As Stream = OutputStream(ms, CompressionProvider)
            s.Write(bytesToCompress, 0, bytesToCompress.Length)
            s.Close()
            Return ms.ToArray()
        End Function

        Public Shared Function Compress(ByVal stringToCompress As String, ByVal CompressionProvider As CompressionType) As String
            Dim compressedData As Byte() = CompressToByte(stringToCompress, CompressionProvider)
            Dim strOut As String = Convert.ToBase64String(compressedData)
            Return strOut
        End Function

        Private Shared Function CompressToByte(ByVal stringToCompress As String, ByVal CompressionProvider As CompressionType) As Byte()
            Dim bytData As Byte() = Encoding.Unicode.GetBytes(stringToCompress)
            Return Compress(bytData, CompressionProvider)
        End Function

        Public Shared Function DeCompress(ByVal stringToDecompress As String, ByVal CompressionProvider As CompressionType) As String
            Dim outString As String = String.Empty

            If stringToDecompress = "" Then
                Throw New ArgumentNullException("stringToDecompress", "You tried to use an empty string")
            End If
            Try
                Dim inArr As Byte() = Convert.FromBase64String(stringToDecompress.Trim())
                outString = System.Text.Encoding.Unicode.GetString(DeCompress(inArr, CompressionProvider))

            Catch nEx As NullReferenceException
                Throw 'Return nEx.Message
            End Try
            Return outString

        End Function

        Public Shared Function DeCompress(ByVal bytesToDecompress As Byte(), ByVal CompressionProvider As CompressionType) As Byte()
            Dim writeData(4096) As Byte
            Dim s2 As Stream = InputStream(New MemoryStream(bytesToDecompress), CompressionProvider)
            Dim outStream As MemoryStream = New MemoryStream
            While True
                Dim size As Integer = s2.Read(writeData, 0, writeData.Length)
                If (size > 0) Then
                    outStream.Write(writeData, 0, size)
                Else
                    Exit While
                End If
            End While
            s2.Close()

            Dim outArr As Byte() = outStream.ToArray()
            outStream.Close()
            Return outArr

        End Function

    End Class
#End Region

End Module

