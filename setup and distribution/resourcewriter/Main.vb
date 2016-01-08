Imports System
Imports System.Collections
Imports System.Resources

Namespace CompuMaster.ResxWriter

    Class MainApp

        Public Shared Sub Main()

            Dim BaseDir As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location)
            Console.WriteLine("BaseDir=" & BaseDir)

            System.IO.File.SetAttributes(System.IO.Path.Combine(BaseDir, "..\WebManager\dbsetup.resx"), IO.FileAttributes.Normal)
            Dim rw As ResXResourceWriter = New ResXResourceWriter(System.IO.Path.Combine(BaseDir, "..\WebManager\dbsetup.resx"))

            '*.sql files
            rw.AddResource("InitDatabase.sql_bzip2", CompuMaster.camm.WebManager.Compression.Compress(TextFileContent(System.IO.Path.Combine(BaseDir, "camm_WebManager_InitDatabase.sql")), camm.WebManager.Compression.CompressionType.BZip2))
            For Each FileName As String In System.IO.Directory.GetFiles(BaseDir, "*.sql")
                If InStr(filename, "camm_WebManager_InitDatabase.sql") = 0 Then 'nicht nochmals den Initializer!
                    rw.AddResource(System.IO.Path.GetFileName(FileName) & "_bzip2", CompuMaster.camm.WebManager.Compression.Compress(TextFileContent(FileName), camm.WebManager.Compression.CompressionType.BZip2))
                End If
            Next

            '*.xml files
            rw.AddResource("build_index.xml_bzip2", CompuMaster.camm.WebManager.Compression.Compress(TextFileContent(System.IO.Path.Combine(BaseDir, "camm_WebManager_build_index.xml")), camm.WebManager.Compression.CompressionType.BZip2))
            For Each FileName As String In System.IO.Directory.GetFiles(BaseDir, "*.xml")
                If InStr(filename, "camm_WebManager_build_index.xml") = 0 Then 'nicht nochmals den Index!
                    rw.AddResource(System.IO.Path.GetFileName(FileName) & "_bzip2", CompuMaster.camm.WebManager.Compression.Compress(TextFileContent(FileName), camm.WebManager.Compression.CompressionType.BZip2))
                End If
            Next

            '*.rtf files (=Licence files)
            For Each FileName As String In System.IO.Directory.GetFiles(BaseDir, "*.rtf")
                rw.AddResource(System.IO.Path.GetFileName(FileName) & "_bzip2", CompuMaster.camm.WebManager.Compression.Compress(BinaryFileContent(FileName), camm.WebManager.Compression.CompressionType.BZip2))
            Next

            rw.Close()

        End Sub 'Main 

        Public Shared Function TextFileContent(ByVal path As String) As String
            Dim Result As String
            Dim TextReader As System.IO.TextReader = New System.IO.StreamReader(path, System.Text.Encoding.Default, True)
            Result = TextReader.ReadToEnd
            TextReader.Close()
            Return Result
        End Function

        Public Shared Function BinaryFileContent(ByVal path As String) As Byte()
            Dim Result() As Byte
            Dim FileStream As System.IO.Stream = New System.IO.FileStream(path, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
            Dim FileLength As Long = FileStream.Length
            ReDim Result(FileLength)
            FileStream.Read(Result, 0, FileLength)
            FileStream.Close()
            Return Result
        End Function

    End Class 'MainApp

End Namespace
