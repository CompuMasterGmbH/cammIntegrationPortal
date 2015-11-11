Option Strict On
Option Explicit On 

Imports System
Imports System.IO
Imports System.Data
Imports System.Data.OleDb
Imports System.Data.OleDb.OleDbDataReader
Imports System.Xml
Imports System.Xml.Xsl
Imports System.Xml.XPath

Module Main

    Sub Main()

        Try
            Create_InternationlizationsScriptFile("out\web-scripts-aspx\sysdata\custom_internationalization.vb", "ScriptEngineValue_VB_NET_DLL", "custom_internationalization.vb.xsl", "source.xml")
            Create_InternationlizationsScriptFile("out\web-scripts-asp\system\internationalization.asp", "ScriptEngineValue_ASP", "internationlization.asp.xsl")
            Create_InternationlizationsScriptFile("out\cwm-lib\internationalization.vb", "ScriptEngineValue_VB_NET_DLL", "internationlization.vb.xsl")
            Create_InternationlizationsScriptFile("out\db-sqls\camm_WebManager_markets.sql", "ScriptEngineValue_VB_NET_DLL", "camm_WebManager_markets.sql.xsl")
            Create_InternationlizationsScriptFile("out\db-sqls\camm_WebManager_markets.azure.sql", "ScriptEngineValue_VB_NET_DLL", "camm_WebManager_markets.azure.sql.xsl")

        Catch ex As Exception
            MsgBox(ex.ToString)
            'Console.WriteLine(ex.ToString)
            'Console.ReadLine()
        Finally
        End Try

    End Sub

    Function MyDataSource(ByVal SpecialsColumnName As String) As XmlDocument 'Data.DataSet

        Dim xmldoc As System.Xml.XmlDocument = New XmlDocument
        Dim MyInternationalizationConnection As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Environment.CurrentDirectory & "\app_data\Internationalizations.mdb;User Id=admin;Password=;")
        Dim MyCountriesAndLanguagesConnection As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Environment.CurrentDirectory & "\app_data\CountriesAndLanguages.mdb;User Id=admin;Password=;")
        MyInternationalizationConnection.Open()
        MyCountriesAndLanguagesConnection.Open()

        Try
            Dim MyDataTable As DataTable
            Dim declarationsblock As XmlElement
            Dim languageblock As XmlElement
            Dim datetimeblock As XmlElement
            Dim systemlanguagesblock As XmlElement
            Dim browseridsblock As XmlElement
            Dim markets2languagesblock As XmlElement
            Dim root As XmlElement

            Dim dataSet As System.Data.DataSet
            Dim MyDataPrepTable_Languages As DataTable

            root = CType(xmldoc.CreateNode(XmlNodeType.Element, "root", ""), XmlElement)
            xmldoc.AppendChild(root)

            'System Languages block
            dataSet = New System.Data.DataSet("dataroot")
            MyDataTable = CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCountriesAndLanguagesConnection, _
                "select * from Languages", _
                CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection, _
                "systemlanguages")
            dataSet.Tables.Add(MyDataTable)
            For Each MyRow As DataRow In MyDataTable.Rows
                For MyCounter As Integer = 1 To MyDataTable.Columns.Count
                    Dim MyField As Object = MyRow(MyCounter - 1)
                    If MyField.GetType.ToString = "System.DBNull" AndAlso MyDataTable.Columns(MyCounter - 1).DataType Is GetType(System.String) Then
                        MyRow(MyCounter - 1) = "NULL"
                    ElseIf MyField.GetType.ToString = "System.String" AndAlso MyDataTable.Columns(MyCounter - 1).DataType Is GetType(System.String) Then
                        MyRow(MyCounter - 1) = "'" & CStr(MyField).Replace("'", "''") & "'"
                    ElseIf MyField.GetType.ToString = "System.Int32" Then
                        'do nothing
                    End If
                Next
            Next
            systemlanguagesblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "systemlanguagesblock", ""), XmlElement)
            systemlanguagesblock.InnerXml = dataSet.GetXml
            xmldoc.DocumentElement.AppendChild(systemlanguagesblock)

            ''Markets2Languages-Lookup block
            'dataSet = New System.Data.DataSet("dataroot")
            MyDataTable = CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCountriesAndLanguagesConnection, _
                "select AlternativeLanguage from Languages WHERE NOT AlternativeLanguage IS NULL GROUP BY AlternativeLanguage ORDER BY AlternativeLanguage DESC", _
                CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection, _
                "LanguagesWithMarkets")
            'dataSet.Tables.Add(MyDataTable)
            'markets2languagesblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "markets2languagesblock", ""), XmlElement)
            'markets2languagesblock.InnerXml = dataSet.GetXml
            'xmldoc.DocumentElement.AppendChild(markets2languagesblock)
            'Markets2Languages-Lookup block
            markets2languagesblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "markets2languagesblock", ""), XmlElement)
            xmldoc.DocumentElement.AppendChild(markets2languagesblock)
            For Each MyFoundLangID As DataRow In MyDataTable.Rows
                Dim langid As XmlElement
                Dim langdata As XmlElement
                Dim markets As XmlElement
                Dim myreader As IDataReader
                myreader = CompuMaster.Data.DataQuery.AnyIDataProvider.ExecuteReader(MyCountriesAndLanguagesConnection, _
                    "select ID from Languages WHERE AlternativeLanguage = " & CLng(MyFoundLangID("AlternativeLanguage")).ToString & " ORDER BY ID ASC", _
                    CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                langdata = xmldoc.CreateElement("languagedata")
                langid = xmldoc.CreateElement("languageid")
                markets = xmldoc.CreateElement("markets")
                langdata.AppendChild(langid)
                langid.InnerText = CLng(MyFoundLangID("AlternativeLanguage")).ToString
                markets2languagesblock.AppendChild(langdata)
                langdata.AppendChild(markets)
                markets.InnerXml = ReaderToXML(myreader, "")
                myreader.Close()
            Next

            'List of language IDs
            MyDataPrepTable_Languages = CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(MyInternationalizationConnection, _
                "SELECT LangID FROM __Internationalizations_Export WHERE LangID <> 0 GROUP BY LangID ORDER BY LangID DESC", _
                CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection, _
                "ExistingLanguages")

            'Browser IDs block
            dataSet = New System.Data.DataSet("dataroot")
            MyDataTable = New DataTable("BrowserIDs")
            MyDataTable = CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCountriesAndLanguagesConnection, _
                "SELECT LanguagesAndTheirTranslations.ID, LanguagesAndTheirTranslations.PropertyValue_Text" & vbNewLine & _
                    "FROM LanguagesAndTheirTranslations" & vbNewLine & _
                    "WHERE (((LanguagesAndTheirTranslations.PropertyValue_Text) Is Not Null) AND ((LanguagesAndTheirTranslations.LangID)=0) AND ((LanguagesAndTheirTranslations.PropertyName)='BrowserLanguageID'))" & vbNewLine & _
                    "UNION " & vbNewLine & _
                    "SELECT LanguagesAndTheirTranslations.ID, LanguagesAndTheirTranslations.PropertyValue_Text + '-' + LanguagesAndTheirTranslations.PropertyValue_Text" & vbNewLine & _
                    "FROM LanguagesAndTheirTranslations" & vbNewLine & _
                    "WHERE (((LanguagesAndTheirTranslations.PropertyValue_Text) Is Not Null And (LanguagesAndTheirTranslations.PropertyValue_Text) Like '__') AND ((LanguagesAndTheirTranslations.LangID)=0) AND ((LanguagesAndTheirTranslations.PropertyName)='BrowserLanguageID')) AND ( LanguagesAndTheirTranslations.PropertyValue_Text + '-' + LanguagesAndTheirTranslations.PropertyValue_Text NOT IN (SELECT LanguagesAndTheirTranslations.PropertyValue_Text " & vbNewLine & _
                    "           FROM(LanguagesAndTheirTranslations) " & vbNewLine & _
                    "           WHERE (((LanguagesAndTheirTranslations.PropertyValue_Text) Is Not Null) And ((LanguagesAndTheirTranslations.PropertyValue_Text) Like '__-__') AND ((LanguagesAndTheirTranslations.LangID)=0) AND ((LanguagesAndTheirTranslations.PropertyName)='BrowserLanguageID'))))", _
                CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection, _
                "BrowserIDs")
            dataSet.Tables.Add(MyDataTable)
            browseridsblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "browseridsblock", ""), XmlElement)
            browseridsblock.InnerXml = dataSet.GetXml
            xmldoc.DocumentElement.AppendChild(browseridsblock)

            'Declarations block
            dataSet = New System.Data.DataSet("dataroot")
            MyDataTable = New DataTable("declarations")
            MyDataTable = CompuMaster.Data.DataQuery.AnyIDataProvider.FillDataTable(MyInternationalizationConnection, _
                "SELECT TypeID, LangID, SortID, What2Setup, Value2Setup, [" & SpecialsColumnName & "] As Specials, Obsolete FROM __Internationalizations_Export WHERE TypeID = 0", _
                CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection, _
                "declarations")
            dataSet.Tables.Add(MyDataTable)
            declarationsblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "declarationsblock", ""), XmlElement)
            declarationsblock.InnerXml = dataSet.GetXml
            xmldoc.DocumentElement.AppendChild(declarationsblock)

            'Languages setup block
            languageblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "languagesblock", ""), XmlElement)
            xmldoc.DocumentElement.AppendChild(languageblock)
            For Each MyFoundLangID As DataRow In MyDataPrepTable_Languages.Rows
                Dim langid As XmlElement
                Dim langdata As XmlElement
                Dim langsetup As XmlElement
                Dim myreader As IDataReader
                myreader = CompuMaster.Data.DataQuery.AnyIDataProvider.ExecuteReader(MyInternationalizationConnection, _
                    "SELECT SortID, What2Setup, Value2Setup, [" & SpecialsColumnName & "] As Specials FROM __Internationalizations_Export WHERE (Value2Setup Is Not Null OR [" & SpecialsColumnName & "] Is Not Null) AND TypeID = 1 And LangID = " & CLng(MyFoundLangID("LangID")).ToString, _
                    CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                langdata = xmldoc.CreateElement("languagedata")
                langid = xmldoc.CreateElement("languageid")
                langsetup = xmldoc.CreateElement("languagesetups")
                langdata.AppendChild(langid)
                langid.InnerText = CLng(MyFoundLangID("LangID")).ToString
                languageblock.AppendChild(langdata)
                langdata.AppendChild(langsetup)
                langsetup.InnerXml = ReaderToXML(myreader, "languagesetup")
                myreader.Close()
            Next

            'Datetime functions block
            datetimeblock = CType(xmldoc.CreateNode(XmlNodeType.Element, "datetimeblock", ""), XmlElement)
            xmldoc.DocumentElement.AppendChild(datetimeblock)
            For Each MyFoundLangID As DataRow In MyDataPrepTable_Languages.Rows
                Dim langid As XmlElement
                Dim langdata As XmlElement
                Dim langsetup As XmlElement
                Dim myreader As IDataReader
                myreader = CompuMaster.Data.DataQuery.AnyIDataProvider.ExecuteReader(MyInternationalizationConnection, _
                    "SELECT SortID, What2Setup, Value2Setup, [" & SpecialsColumnName & "] As Specials FROM __Internationalizations_Export WHERE (Value2Setup Is Not Null OR [" & SpecialsColumnName & "] Is Not Null) AND TypeID = 2 AND LangID = " & CLng(MyFoundLangID("LangID")).ToString, _
                    CommandType.Text, Nothing, CompuMaster.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                langdata = xmldoc.CreateElement("languagedata")
                langid = xmldoc.CreateElement("languageid")
                langsetup = xmldoc.CreateElement("languagesetups")
                langdata.AppendChild(langid)
                langid.InnerText = CLng(MyFoundLangID("LangID")).ToString
                datetimeblock.AppendChild(langdata)
                langdata.AppendChild(langsetup)
                langsetup.InnerXml = ReaderToXML(myreader, "languagesetup")
                myreader.Close()
            Next


        Finally
            'Release memory
            CompuMaster.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyInternationalizationConnection)
            CompuMaster.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(MyCountriesAndLanguagesConnection)
        End Try

        Return xmldoc

    End Function

    Sub Create_InternationlizationsScriptFile(ByVal ScriptFile As String, ByVal SpecialsColumnName As String, ByVal XSLFileName As String, Optional ByVal SourceXMLFile As String = "")

        Dim OutputStream As System.IO.StreamWriter
        Try
            Console.WriteLine("Creating output file: " & Environment.CurrentDirectory & "\" & ScriptFile)
            If System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Environment.CurrentDirectory & "\" & ScriptFile)) = False Then
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Environment.CurrentDirectory & "\" & ScriptFile))
            End If
            OutputStream = New System.IO.StreamWriter(Environment.CurrentDirectory & "\" & ScriptFile, False, System.Text.Encoding.UTF8)

            'begin XML-Defintion
            Dim xmlDoc As XmlDocument = MyDataSource(SpecialsColumnName)
            If SourceXMLFile <> "" Then
                xmlDoc.Save(Environment.CurrentDirectory & "\" & SourceXMLFile)
            End If

            'Create the XslTransform.
            Dim xslt As XslTransform = New XslTransform

            Dim xslArgs As XsltArgumentList = New XsltArgumentList
            'xslargs.AddParam("acurrency", "", view_category_currency)
            'xslargs.AddParam("apartnumber", "", view_category_partnumber)
            'xslargs.AddParam("amore", "", view_category_more)
            'xslargs.AddParam("aorder", "", view_category_order)
            'xslargs.AddParam("acategory", "", ls_category)
            'xslargs.AddParam("acount", "", ds_cart_unit)
            'xslargs.AddParam("image_cart", "", ds_image_cart2)
            'xslargs.AddParam("aindividual_weight", "", view_category_individual_weight)
            'xslargs.AddParam("astock_management", "", db_stock_management)

            xslt.Load(Environment.CurrentDirectory & "\" & XSLFileName)
            xslt.Transform(xmlDoc, xslArgs, OutputStream)
        Finally
            'mydataset.Dispose()
            If Not OutputStream Is Nothing Then OutputStream.Close()
        End Try

    End Sub

    Public Function ReaderToXML(ByVal objReader As IDataReader, _
                    ByVal RowsNodeName As String, _
                    Optional ByVal ParentNodeName As String = "") As String
        'If ParentNodeName is not blank, it will be used as
        'Start End node of the XML

        Dim sXML As String = ""
        Dim intColumnCount As Integer
        Dim intCtr As Integer
        Dim sValue As String

        ParentNodeName = Trim(ParentNodeName)
        Try
            intColumnCount = objReader.FieldCount
            If ParentNodeName <> "" Then sXML += "<" & ParentNodeName & ">"

            Do While objReader.Read

                If RowsNodeName <> "" Then sXML += "<" & RowsNodeName & ">"

                'Loop through each row
                For intCtr = 0 To intColumnCount - 1
                    'Get the Value of each column
                    'Does not include nodes for null/blank values

                    If Not objReader.IsDBNull(intCtr) Then
                        sValue = objReader.Item(intCtr).ToString
                        If Trim(sValue) <> "" Then
                            sXML += "<" & objReader.GetName(intCtr) & ">" & XMLizeString(sValue) & "</" & objReader.GetName(intCtr) & ">"
                        End If
                    End If
                Next

                If RowsNodeName <> "" Then sXML += "</" & RowsNodeName & ">"

            Loop
            If ParentNodeName <> "" Then sXML += "</" & ParentNodeName & ">"

        Catch Ex As Exception
            Return ("")
        End Try
        Return sXML
    End Function


    Private Function XMLizeString(ByVal sInput As String) As String
        Dim s As String

        'SHORTENED VERSION TO REDUCE EXECUTION TIME
        'Return " <![CDATA[" & sInput & "]]>"
        'THIS WILL INCREASE THE SIZE OF YOUR XML String
        If Not (IsAlphaNumeric(sInput)) Then
            Return "<![CDATA[" & sInput & "]]>"
        Else
            Return sInput
        End If
    End Function

    Private Function IsAlphaNumeric(ByVal TestString As String) As Boolean

        Dim sTemp As String
        Dim iLen As Integer
        Dim iCtr As Integer
        Dim sChar As String

        'returns true if all characters in a string are alphabetical
        '   or numeric
        'returns false otherwise or for empty string

        sTemp = TestString
        iLen = Len(sTemp)
        If iLen > 0 Then
            For iCtr = 1 To iLen
                sChar = Mid(sTemp, iCtr, 1)
                If Not sChar Like "[0-9A-Za-z.:, $%{}#+""<>\/!?;()&[]][[]]" Then _
                    Exit Function
            Next

            IsAlphaNumeric = True
        End If
    End Function

End Module
