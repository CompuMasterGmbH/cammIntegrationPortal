Module Main

    Sub Main()
        Dim CsvOriginUrl As String = "https://raw.githubusercontent.com/datasets/country-codes/master/data/country-codes.csv"
        Dim CountryCodes As DataTable = CompuMaster.Data.Csv.ReadDataTableFromCsvFile(CsvOriginUrl, True, System.Text.Encoding.UTF8, System.Globalization.CultureInfo.InvariantCulture, """"c, False, False)
        CompuMaster.Data.Csv.WriteDataTableToCsvFile(OutputFilePath("country-codes.csv"), CountryCodes, True, "UTF-8", ","c, """"c, "."c)
        System.IO.File.WriteAllText(OutputFilePath("country-codes.plain.txt"), CompuMaster.Data.DataTables.ConvertToPlainTextTable(CountryCodes))
        System.IO.File.WriteAllText(OutputFilePath("country-codes.fixedplain.txt"), CompuMaster.Data.DataTables.ConvertToPlainTextTableFixedColumnWidths(CountryCodes))
        System.IO.File.WriteAllBytes(OutputFilePath("country-codes.html"), System.Text.Encoding.UTF8.GetPreamble)
        System.IO.File.AppendAllText(OutputFilePath("country-codes.html"), CompuMaster.Data.DataTables.ConvertToHtmlTable(CountryCodes), System.Text.Encoding.UTF8)
        System.IO.File.WriteAllText(OutputFilePath("country-codes.wiki.txt"), CompuMaster.Data.DataTables.ConvertToWikiTable(CountryCodes))
        CountryCodes.WriteXml(OutputFilePath("country-codes.xml"), XmlWriteMode.WriteSchema, False)
    End Sub

    Private Function OutputFilePath(fileName As String) As String
        Dim OutputDir As String = System.Environment.CurrentDirectory
        Return System.IO.Path.Combine(OutputDir, fileName)
    End Function

End Module
