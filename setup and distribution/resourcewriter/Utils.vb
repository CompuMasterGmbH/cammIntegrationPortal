Imports System
Imports System.Text
Imports System.IO
Imports ICSharpCode.SharpZipLib

Namespace CompuMaster.camm.WebManager

    Public Module Utils

        Public Function GetWorkstationID() As String

            Dim host As System.net.IPHostEntry = System.Net.Dns.Resolve(System.Net.Dns.GetHostName)

            ' Loop on the AddressList
            Dim curAdd As System.net.IPAddress
            For Each curAdd In host.AddressList

                ' Display the server IP address in the standard format. In 
                ' IPv4 the format will be dotted-quad notation, in IPv6 it will be
                ' in in colon-hexadecimal notation.
                Return curAdd.ToString()

            Next

            'If not already returned with a found IP, return with the host name
            Return System.Net.Dns.GetHostName

        End Function

#Region "Nz"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function IfNull(ByVal CheckValueIfDBNull As Object, Optional ByVal ReplaceWithThis As Object = Nothing) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns null (Nothing in VisualBasic) in that case
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function Nz(ByVal CheckValueIfDBNull As Object) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return Nothing
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Object) As Object
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Integer) As Integer
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Long) As Long
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Boolean) As Boolean
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for DBNull and returns the second value alternatively
        ''' </summary>
        ''' <param name="CheckValueIfDBNull">The value to be checked</param>
        ''' <param name="ReplaceWithThis">The alternative value, null (Nothing in VisualBasic) if not defined</param>
        ''' <returns>A value which is not DBNull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As String) As String
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return (CheckValueIfDBNull)
            End If
        End Function
#End Region

#Region "Type conversions"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted long value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function TryCLng(ByVal Expression As Object) As Long
            Dim Result As Long = Nothing
            Try
                Result = CLng(Expression)
            Catch
            End Try
            Return Result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted long value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function TryCInt(ByVal Expression As Object) As Integer
            Dim Result As Long = Nothing
            Try
                Result = CLng(Expression)
            Catch
            End Try
            Return Result
        End Function
#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Converts all line breaks into HTML line breaks (&quot;&lt;br&gt;&quot;)
        ''' </summary>
        ''' <param name="Text"></param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Supported line breaks are linebreaks of Windows, MacOS as well as Linux/Unix.
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function HTMLEncodeLineBreaks(ByVal Text As String) As String
            Return Text.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Cr, "<br>").Replace(ControlChars.Lf, "<br>")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Replaces placeholders in a string by defined values
        ''' </summary>
        ''' <param name="message">The string with the placeholders</param>
        ''' <param name="values">One or more values which should be used for replacement</param>
        ''' <returns>The new resulting string</returns>
        ''' <remarks>
        '''     Supported special character combinations are <code>\t</code>, <code>\r</code>, <code>\n</code>, <code>\\</code>, <code>\[</code><br>
        '''     Supported placeholders are <code>[*]</code>, <code>[n:1..9]</code>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function sprintf(ByVal message As String, ByVal ParamArray values() As Object) As String
            Const errpfNoClosingBracket = vbObjectError + 1
            Const errpfMissingValue = vbObjectError + 2
            Const errpfVBScriptNotSupported = vbObjectError + 3
            '*** Special chars ***
            '   \t = TAB
            '   \r = CR
            '   \n = CRLF
            '   \[ = [
            '   \\ = \
            '*** Placeholder ***
            '   []		= value of parameter list will be skipped
            '   [*]		= value will be inserted without any special format
            '   [###]	= "###" stands for a format string (further details in your SDK help for command "Format()")
            '   [n:...]	= n as 1..9: uses the n-th parameter; the internal parameter index will not be increased!
            '             There can be a format string behind the ":"

            Dim i%, iv%, orig_iv%, iob%, icb%
            Dim messageParts() As String, part As String, formatString As String

            If message Is Nothing Then message = ""
            message = message.Replace("\\", "[\]")
            message = message.Replace("\t", vbTab)
            message = message.Replace("\r", vbCr)
            message = message.Replace("\n", vbCrLf)
            message = message.Replace("\[", "[(]")

            iob = 1
            Do
                iob = InStr(iob, message, "[")
                If iob = 0 Then Exit Do

                icb = InStr(iob + 1, message, "]")
                If icb = 0 Then
                    Err.Raise(errpfNoClosingBracket, "printf", "Missing ']' after '[' at position " & iob & "!")
                End If

                formatString = Mid$(message, iob + 1, icb - iob - 1)

                If InStr("123456789", Mid$(formatString, 1, 1)) > 0 And Mid$(formatString, 2, 1) = ":" Then
                    orig_iv = iv

                    iv = CInt(Mid$(formatString, 1, 1)) - 1
                    If iv > UBound(values) Then iv = UBound(values)

                    formatString = Mid$(formatString, 3)
                Else
                    orig_iv = -1
                End If


                Select Case formatString
                    Case ""
                        formatString = ""
                    Case "("
                        formatString = "["
                        iv = iv - 1
                    Case "\"
                        formatString = "\"
                        iv = iv - 1
                    Case "*"
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = values(iv)
                    Case Else 'with user specified format string
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = values(iv) 'Format(values(iv), formatString)
                End Select

                message = Left$(message, iob - 1) & formatString & Mid$(message, icb + 1)
                iob = iob + Len(formatString) + IIf(orig_iv >= 0, 2, 0)

                If orig_iv >= 0 Then
                    iv = orig_iv
                Else
                    iv = iv + 1
                End If
            Loop

            sprintf = message

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Join all items of a NameValueCollection (for example Request.QueryString) to a simple string
        ''' </summary>
        ''' <param name="NameValueCollectionToString">A collection like Request.Form or Request.QueryString</param>
        ''' <param name="BeginningOfItem">A string in front of a key</param>
        ''' <param name="KeyValueSeparator">The string between key and value</param>
        ''' <param name="EndOfItem">The string to be placed at the end of a value</param>
        ''' <returns>A string containing all elements of the collection</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	09.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function JoinNameValueCollectionToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection, ByVal BeginningOfItem As String, ByVal KeyValueSeparator As String, ByVal EndOfItem As String) As String
            Dim Result As String
            For Each ParamItem As String In NameValueCollectionToString
                Result &= BeginningOfItem & ParamItem & KeyValueSeparator & NameValueCollectionToString(ParamItem) & EndOfItem
            Next
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Converts HTML messages to simple text
        ''' </summary>
        ''' <param name="HTML">A string with HTML code</param>
        ''' <returns>The rendered output as plain text</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	09.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ConvertHTMLToText(ByVal HTML As String) As String
            'TODO: 1. remove of all other HTML tags
            '      2. search case insensitive
            '      3. truncate content between <head> and </head>, <script> and </script>, <!-- and -->
            Dim Result As String = HTML
            Result = Result.Replace("<br>", vbNewLine)
            Result = Result.Replace("<p>", vbNewLine & vbNewLine)
            Result = Result.Replace("</p>", "")
            Result = Result.Replace("<li>", "- ")
            Result = Result.Replace("</li>", vbNewLine)
            Result = Result.Replace("<ul>", "    ")
            Result = Result.Replace("</ul>", vbNewLine)
            Result = Result.Replace("<b>", "")
            Result = Result.Replace("</b>", "")
            Result = Result.Replace("<strong>", "")
            Result = Result.Replace("</strong>", "")
            Result = Result.Replace("<html>", "")
            Result = Result.Replace("</html>", "")
            Result = Result.Replace("<body>", "")
            Result = Result.Replace("</body>", "")
            Result = Result.Replace("<font face=""Arial"">", "")
            Result = Result.Replace("<font face=""Arial"" color=""red"">", "")
            Result = Result.Replace("</font>", "")
            Result = Result.Replace("<hl>", "-------------------------------------------------------")
            Return Result
        End Function

#Region "LinkHighlighting"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Converts addresses or URLs in a string into HTML hyperlinks
        ''' </summary>
        ''' <param name="LinkInitiator">Search for this start string of a word</param>
        ''' <param name="Msg">Search inside this string</param>
        ''' <param name="AdditionalProtocolInitiator">An additionally required prefix</param>
        ''' <returns>HTML with hyperlinks</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <example language="vb">
        '''     Dim HTMLResult As String = ConvertProtocolAddressIntoHyperLink ("www.", Text, "http://")
        ''' </example>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ConvertProtocolAddressIntoHyperLink(ByVal LinkInitiator As String, ByVal Msg As String, ByVal AdditionalProtocolInitiator As String) As String
            Dim OldEndPoint As Integer
            Dim LinkStartPos As Integer
            Dim LinkEndPos As Integer
            Dim LinkEndPosBySpaceChar As Integer
            Dim LinkEndPosByReturnChar As Integer

            'String nach umwandelbaren Zeichenfolgen durchforsten und abändern
            OldEndPoint = 0
            Do
                LinkStartPos = InStr(OldEndPoint + 1, LCase(Msg), LinkInitiator)
                If LinkStartPos <> 0 Then
                    LinkEndPosBySpaceChar = InStr(LinkStartPos, LCase(Msg), " ") - 1
                    If LinkEndPosBySpaceChar = -1 Then LinkEndPosBySpaceChar = Len(Msg)
                    LinkEndPosByReturnChar = InStr(LinkStartPos, LCase(Msg), Chr(13)) - 1
                    If LinkEndPosByReturnChar = -1 Then LinkEndPosByReturnChar = Len(Msg)
                    LinkEndPos = IIf(LinkEndPosBySpaceChar < LinkEndPosByReturnChar, LinkEndPosBySpaceChar, LinkEndPosByReturnChar)
                    'Exclude Satzzeichen
                    If Mid(Msg, LinkEndPos, 1) = "." Or _
                            Mid(Msg, LinkEndPos, 1) = "!" Or _
                            Mid(Msg, LinkEndPos, 1) = "?" Or _
                            Mid(Msg, LinkEndPos, 1) = "," Or _
                            Mid(Msg, LinkEndPos, 1) = ";" Or _
                            Mid(Msg, LinkEndPos, 1) = ":" Then
                        LinkEndPos = LinkEndPos - 1
                    End If
                    If Len(LinkInitiator) <> LinkEndPos - LinkStartPos + 1 Then
                        Msg = Mid(Msg, 1, LinkStartPos - 1) & "<a href=""" & AdditionalProtocolInitiator & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & "</a>" & Mid(Msg, LinkEndPos + 1)
                        OldEndPoint = LinkEndPos + Len("<a href=""" & AdditionalProtocolInitiator & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & "</a>")
                    Else
                        OldEndPoint = LinkEndPos
                    End If
                End If
            Loop Until LinkStartPos = 0
            ConvertProtocolAddressIntoHyperLink = Msg

        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Convert e-mail addresses into hyperlinks
        ''' </summary>
        ''' <param name="Msg">The string where to search in</param>
        ''' <returns>HTML with hyperlinks</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ConvertEMailAddressIntoHyperLink(ByVal Msg As String) As String
            Dim OldEndPoint As Integer
            Dim LinkStartPos As Integer
            Dim LinkEndPos As Integer
            Dim LinkEndPosBySpaceChar As Integer
            Dim LinkEndPosByReturnChar As Integer
            Dim LefterChar

            'String nach umwandelbaren Zeichenfolgen durchforsten und abändern
            OldEndPoint = 0
            Do
                LinkStartPos = InStr(OldEndPoint + 1, LCase(Msg), "@")
                Do
                    'Anfang der e-mail-Adresse suchen
                    If LinkStartPos > 1 Then
                        LefterChar = Mid(Msg, LinkStartPos - 1, 1)
                    Else
                        LefterChar = " "
                    End If
                    If LefterChar = " " Or _
                            LefterChar = Chr(13) Or _
                            LefterChar = Chr(10) Or _
                            LefterChar = "!" Or _
                            LefterChar = "?" Or _
                            LefterChar = "," Or _
                            LefterChar = ";" Or _
                            LefterChar = ":" Then
                        Exit Do
                    Else
                        LinkStartPos = LinkStartPos - 1
                    End If
                Loop

                If LinkStartPos <> 0 Then
                    LinkEndPosBySpaceChar = InStr(LinkStartPos, LCase(Msg), " ") - 1
                    If LinkEndPosBySpaceChar = -1 Then LinkEndPosBySpaceChar = Len(Msg)
                    LinkEndPosByReturnChar = InStr(LinkStartPos, LCase(Msg), Chr(13)) - 1
                    If LinkEndPosByReturnChar = -1 Then LinkEndPosByReturnChar = Len(Msg)
                    LinkEndPos = IIf(LinkEndPosBySpaceChar < LinkEndPosByReturnChar, LinkEndPosBySpaceChar, LinkEndPosByReturnChar)
                    'Exclude Satzzeichen
                    If Mid(Msg, LinkEndPos, 1) = "." Or _
                            Mid(Msg, LinkEndPos, 1) = "!" Or _
                            Mid(Msg, LinkEndPos, 1) = "?" Or _
                            Mid(Msg, LinkEndPos, 1) = "," Or _
                            Mid(Msg, LinkEndPos, 1) = ";" Or _
                            Mid(Msg, LinkEndPos, 1) = ":" Then
                        LinkEndPos = LinkEndPos - 1
                    End If
                    If 6 < LinkEndPos - LinkStartPos + 1 Then
                        Msg = Mid(Msg, 1, LinkStartPos - 1) & "<a href=""mailto:" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & "</a>" & Mid(Msg, LinkEndPos + 1)
                        OldEndPoint = LinkEndPos + Len("<a href=""mailto:" & Mid(Msg, LinkStartPos, LinkEndPos - LinkStartPos + 1) & """>" & "</a>")
                    Else
                        OldEndPoint = LinkEndPos
                    End If
                End If
            Loop Until LinkStartPos = 0
            ConvertEMailAddressIntoHyperLink = Msg

        End Function
        Private Function ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink(ByVal SearchForTypicalString As String, ByVal Msg As String, ByVal ProtocolInitiator As String) As String
            Dim OldEndPoint As Integer
            Dim LinkAreaStartPos As Integer
            Dim LinkAreaStartPosByAnchorWithSpace As Integer
            Dim LinkAreaStartPosByAnchorWithReturn As Integer
            Dim ProbeArea As String

            'String nach umwandelbaren Adressen in nicht-Link-Bereichen aufspüren
            OldEndPoint = 0
            Do
                LinkAreaStartPosByAnchorWithSpace = InStr(OldEndPoint + 1, LCase(Msg), "<a ")
                If LinkAreaStartPosByAnchorWithSpace = 0 Then LinkAreaStartPosByAnchorWithSpace = Len(Msg) + 1
                LinkAreaStartPosByAnchorWithReturn = InStr(OldEndPoint + 1, LCase(Msg), "<a" & Chr(13))
                If LinkAreaStartPosByAnchorWithReturn = 0 Then LinkAreaStartPosByAnchorWithReturn = Len(Msg) + 1
                LinkAreaStartPos = IIf(LinkAreaStartPosByAnchorWithSpace < LinkAreaStartPosByAnchorWithReturn, LinkAreaStartPosByAnchorWithSpace, LinkAreaStartPosByAnchorWithReturn)
                ProbeArea = Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1)
                ProbeArea = ConvertProtocolAddressIntoHyperLink(SearchForTypicalString, ProbeArea, ProtocolInitiator)
                Msg = Mid(Msg, 1, OldEndPoint) & ProbeArea & Mid(Msg, LinkAreaStartPos)
                LinkAreaStartPos = LinkAreaStartPos + Len(ProbeArea) - Len(Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1))
                OldEndPoint = InStr(LinkAreaStartPos, LCase(Msg), "</a>") + 3
            Loop Until LinkAreaStartPos = Len(Msg) Or OldEndPoint = Len(Msg) Or OldEndPoint = 3
            ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink = Msg

        End Function
        Private Function ConvertEMailAddressCurrentlyWOHyperLinkToHyperLink(ByVal Msg As String) As String
            Dim OldEndPoint As Integer
            Dim LinkAreaStartPos As Integer
            Dim LinkAreaStartPosByAnchorWithSpace As Integer
            Dim LinkAreaStartPosByAnchorWithReturn As Integer
            Dim ProbeArea As String

            'String nach umwandelbaren e-mail-Adressen in nicht-Link-Bereichen aufspüren
            OldEndPoint = 0
            Do
                LinkAreaStartPosByAnchorWithSpace = InStr(OldEndPoint + 1, LCase(Msg), "<a ")
                If LinkAreaStartPosByAnchorWithSpace = 0 Then LinkAreaStartPosByAnchorWithSpace = Len(Msg) + 1
                LinkAreaStartPosByAnchorWithReturn = InStr(OldEndPoint + 1, LCase(Msg), "<a" & Chr(13))
                If LinkAreaStartPosByAnchorWithReturn = 0 Then LinkAreaStartPosByAnchorWithReturn = Len(Msg) + 1
                LinkAreaStartPos = IIf(LinkAreaStartPosByAnchorWithSpace < LinkAreaStartPosByAnchorWithReturn, LinkAreaStartPosByAnchorWithSpace, LinkAreaStartPosByAnchorWithReturn)
                ProbeArea = Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1)
                ProbeArea = ConvertEMailAddressIntoHyperLink(ProbeArea)
                Msg = Mid(Msg, 1, OldEndPoint) & ProbeArea & Mid(Msg, LinkAreaStartPos)
                LinkAreaStartPos = LinkAreaStartPos + Len(ProbeArea) - Len(Mid(Msg, OldEndPoint + 1, LinkAreaStartPos - OldEndPoint - 1))
                OldEndPoint = InStr(LinkAreaStartPos, LCase(Msg), "</a>") + 3
            Loop Until LinkAreaStartPos = Len(Msg) Or OldEndPoint = Len(Msg) Or OldEndPoint = 3
            ConvertEMailAddressCurrentlyWOHyperLinkToHyperLink = Msg

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Converts URLs and e-mail addresses in a string into HTML hyperlinks
        ''' </summary>
        ''' <param name="Text">The standard text without any HTML</param>
        ''' <returns>HTML with hyperlinks</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function HighlightLinksInMessage(ByVal Text As String) As String
            Dim HTMLMsg As String = Text

            HTMLMsg = ConvertProtocolAddressIntoHyperLink("http://", HTMLMsg, "")
            HTMLMsg = ConvertProtocolAddressIntoHyperLink("https://", HTMLMsg, "")
            HTMLMsg = ConvertProtocolAddressIntoHyperLink("ftp://", HTMLMsg, "")
            HTMLMsg = ConvertProtocolAddressIntoHyperLink("mailto:", HTMLMsg, "")

            HTMLMsg = ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink("www.", HTMLMsg, "http://")
            HTMLMsg = ConvertNonProtocolAddressCurrentlyWOHyperLinkToHyperLink("ftp.", HTMLMsg, "ftp://")

            HTMLMsg = ConvertEMailAddressCurrentlyWOHyperLinkToHyperLink(HTMLMsg)

            Return HTMLMsg

        End Function
#End Region

    End Module

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

End Namespace