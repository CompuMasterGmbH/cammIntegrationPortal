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

Imports System
Imports System.Web
Imports System.Web.Caching
Imports System.Text
Imports System.IO

Namespace CompuMaster.camm.SmartWebEditor

    Friend Class Utils

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
        Public Shared Function IfNull(ByVal CheckValueIfDBNull As Object, Optional ByVal ReplaceWithThis As Object = Nothing) As Object
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object) As Object
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Object) As Object
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Integer) As Integer
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Integer)
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Long) As Long
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Long)
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As Boolean) As Boolean
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, Boolean)
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As DateTime) As DateTime
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, DateTime)
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
        Public Shared Function Nz(ByVal CheckValueIfDBNull As Object, ByVal ReplaceWithThis As String) As String
            If IsDBNull(CheckValueIfDBNull) Then
                Return (ReplaceWithThis)
            Else
                Return CType(CheckValueIfDBNull, String)
            End If
        End Function
#End Region

#Region "InlineIf"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	10.08.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As String, ByVal falseValue As String) As String
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	10.08.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Integer, ByVal falseValue As Integer) As Integer
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	10.08.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Date, ByVal falseValue As Date) As Date
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	10.08.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Long, ByVal falseValue As Long) As Long
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	10.08.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Double, ByVal falseValue As Double) As Double
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return one of the parameters based on the expression value
        ''' </summary>
        ''' <param name="expression">An expression which shall be validated</param>
        ''' <param name="trueValue">If the expression is True, this parameter will be returned</param>
        ''' <param name="falseValue">If the expression is False, this parameter will be returned</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	10.08.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function InlineIf(ByVal expression As Boolean, ByVal trueValue As Boolean, ByVal falseValue As Boolean) As Boolean
            If expression Then
                Return trueValue
            Else
                Return falseValue
            End If
        End Function
#End Region

#Region "String manipulation and HTML conversions"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Combine a unix path with another one
        ''' </summary>
        ''' <param name="path1">A first path</param>
        ''' <param name="path2">A second path which shall be appended to the first path</param>
        ''' <returns>The combined path</returns>
        ''' <remarks>
        ''' If path2 starts with &quot;/&quot;, it is considered as root folder and will be the only return value.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	11.01.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function CombineUnixPaths(ByVal path1 As String, ByVal path2 As String) As String
            If path1 = Nothing OrElse (path2 <> Nothing AndAlso path2.StartsWith("/")) Then
                Return path2
            ElseIf path2 = Nothing Then
                Return path1
            Else
                'path2.StartsWith("/") can never happen since it has already been evaluated above
                If path1.EndsWith("/") Then
                    Return path1 & path2
                Else
                    Return path1 & "/" & path2
                End If
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Get the complete query string of the current request in a form usable for recreating this query string for a following request
        ''' </summary>
        ''' <param name="removeParameters">Remove all values with this name form the query string</param>
        ''' <returns>A new string with all query string information without the starting questionmark character</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function QueryStringWithoutSpecifiedParameters(ByVal removeParameters As String()) As String
            Return NameValueCollectionWithoutSpecifiedKeys(System.Web.HttpContext.Current.Request.QueryString, removeParameters)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Get the complete string of a collection in a form usable for recreating a query string for a following request
        ''' </summary>
        ''' <param name="collection">A NameValueCollection, e. g. Request.QueryString</param>
        ''' <param name="removeKeys">Names of keys which shall not be in the output</param>
        ''' <returns>A string of the collection data which can be appended to any URL (with url encoding)</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	16.09.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function NameValueCollectionWithoutSpecifiedKeys(ByVal collection As System.Collections.Specialized.NameValueCollection, ByVal removeKeys As String()) As String
            Dim RedirectionParams As String = ""
            For Each ParamItem As String In collection
                Dim RemoveThisParameter As Boolean = False
                If ParamItem = "" Then
                    RemoveThisParameter = True
                ElseIf Not removeKeys Is Nothing Then
                    Dim MyParamItem As String = ParamItem.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    For Each My2RemoveParameter As String In removeKeys
                        If MyParamItem = My2RemoveParameter.ToLower(System.Globalization.CultureInfo.InvariantCulture) Then
                            RemoveThisParameter = True
                        End If
                    Next
                End If
                If RemoveThisParameter = False Then
                    'do not collect empty items (ex. the item between "?" and "&" in "index.aspx?&Lang=2")
                    RedirectionParams = RedirectionParams & "&" & System.Web.HttpUtility.UrlEncode(ParamItem) & "=" & System.Web.HttpUtility.UrlEncode(collection(ParamItem))
                End If
            Next
            Return Mid(RedirectionParams, 2)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Split a string by a separator if there is not a special leading character
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="separator"></param>
        ''' <param name="exceptLeadingCharacter"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	09.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function SplitString(ByVal text As String, ByVal separator As Char, ByVal exceptLeadingCharacter As Char) As String()
            If text = Nothing Then
                Return New String() {}
            End If
            Dim Result As New ArrayList
            'Go through every char of the string
            For MyCounter As Integer = 0 To text.Length - 1
                Dim SplitHere As Boolean
                Dim StartPosition As Integer
                'Find split points
                If text.Chars(MyCounter) = separator Then
                    If MyCounter = 0 Then
                        SplitHere = True
                    ElseIf text.Chars(MyCounter - 1) <> exceptLeadingCharacter Then
                        SplitHere = True
                    End If
                End If
                'Add partial string
                If SplitHere OrElse MyCounter = text.Length - 1 Then
                    Result.Add(text.Substring(StartPosition, CType(IIf(SplitHere = False, 1, 0), Integer) + MyCounter - StartPosition)) 'If Split=False then this if-block was caused by the end of the text; in this case we have to simulate to be after the last character position to ensure correct extraction of the last text element
                    SplitHere = False 'Reset status
                    StartPosition = MyCounter + 1 'Next string starts after the current char
                End If
            Next
            Return CType(Result.ToArray(GetType(String)), String())
        End Function

        ''' <summary>
        ''' Split a string into an array of integers
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="separator"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SplitStringToInteger(ByVal text As String, ByVal separator As Char) As Integer()
            If text Is Nothing Then
                Return New Integer() {}
            End If
            Dim Result As New ArrayList
            Dim SplittedText As String() = text.Split(separator)
            For MyCounter As Integer = 0 To SplittedText.Length - 1
                Result.Add(Integer.Parse(SplittedText(MyCounter)))
            Next
            Return CType(Result.ToArray(GetType(Integer)), Integer())
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Converts all line breaks into HTML line breaks (&quot;&lt;br&gt;&quot;)
        ''' </summary>
        ''' <param name="text">A text string which might contain line breaks of any platform type</param>
        ''' <returns>The text string with encoded line breaks to &quot;&lt;br&gt;&quot;</returns>
        ''' <remarks>
        '''     Supported line breaks are linebreaks of Windows, MacOS as well as Linux/Unix.
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function HTMLEncodeLineBreaks(ByVal text As String) As String
            If text = Nothing Then
                Return text
            Else
                Return text.Replace(ControlChars.CrLf, "<br>").Replace(ControlChars.Cr, "<br>").Replace(ControlChars.Lf, "<br>")
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for a string in another string and return the number of matches
        ''' </summary>
        ''' <param name="source">The string where to search in</param>
        ''' <param name="searchFor">The searched string (binary comparison)</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.01.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CountOfOccurances(ByVal source As String, ByVal searchFor As String) As Integer
            Return CountOfOccurances(source, searchFor, CompareMethod.Binary)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for a string in another string and return the number of matches
        ''' </summary>
        ''' <param name="source">The string where to search in</param>
        ''' <param name="searchFor">The searched string</param>
        ''' <param name="compareMethod">Binary or text search</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	17.01.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CountOfOccurances(ByVal source As String, ByVal searchFor As String, ByVal compareMethod As Microsoft.VisualBasic.CompareMethod) As Integer

            If searchFor = "" Then
                Throw New ArgumentNullException("searchFor")
            End If

            Dim Result As Integer

            'Initial values
            Dim Position As Integer = 0
            Dim NextMatch As Integer = InStr(Position + 1, source, searchFor, compareMethod)

            'and now loop through the source string
            While NextMatch > 0
                Result += 1
                If NextMatch <= Position Then
                    Throw New Exception("NextMatch=" & NextMatch & ", OldPosition=" & Position & ", searchFor=""" & searchFor & """, source=""" & source & """")
                End If
                Position = NextMatch

                'Search for next value
                NextMatch = InStr(Position + 1, source, searchFor, compareMethod)
            End While

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Replaces placeholders in a string by defined values
        ''' </summary>
        ''' <param name="message">The string with the placeholders</param>
        ''' <param name="values">One or more values which should be used for replacement</param>
        ''' <returns>The new resulting string</returns>
        ''' <remarks>
        '''     <para>Supported special character combinations are <code>\t</code>, <code>\r</code>, <code>\n</code>, <code>\\</code>, <code>\[</code></para>
        '''     <para>Supported placeholders are <code>[*]</code>, <code>[n:1..9]</code></para>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function sprintf(ByVal message As String, ByVal ParamArray values() As Object) As String
            Const errpfNoClosingBracket As Integer = vbObjectError + 1
            Const errpfMissingValue As Integer = vbObjectError + 2
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

            Dim iv%, orig_iv%, iob%, icb%
            Dim formatString As String

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
                        formatString = CType(values(iv), String)
                    Case Else 'with user specified format string
                        If iv > UBound(values) Then Err.Raise(errpfMissingValue, "printf", "Missing value in printf-call for format string '[" & formatString & "]'!")
                        formatString = CType(values(iv), String) 'Format(values(iv), formatString)
                End Select

                message = Left$(message, iob - 1) & formatString & Mid$(message, icb + 1)
                iob = iob + Len(formatString) + CType(IIf(orig_iv >= 0, 2, 0), Integer)

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
        ''' Join the elements of an integer array to a single string
        ''' </summary>
        ''' <param name="values">An array of values</param>
        ''' <param name="delimiter">A delimiter which shall separate the values in the string</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	20.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function JoinArrayToString(ByVal values As Integer(), ByVal delimiter As String) As String
            Dim result As New System.Text.StringBuilder
            If values Is Nothing Then
                Throw New ArgumentNullException("values")
            End If
            For MyCounter As Integer = 0 To values.Length - 1
                If MyCounter <> 0 Then result.Append(delimiter)
                result.Append(values(MyCounter))
            Next
            Return result.ToString
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Join the elements of an integer array to a single string
        ''' </summary>
        ''' <param name="values">An array of values</param>
        ''' <param name="delimiter">A delimiter which shall separate the values in the string</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	20.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function _JoinArrayToString(ByVal values As Long(), ByVal delimiter As String) As String
            Dim result As New System.Text.StringBuilder
            If values Is Nothing Then
                Throw New ArgumentNullException("values")
            End If
            For MyCounter As Integer = 0 To values.Length - 1
                If MyCounter <> 0 Then result.Append(delimiter)
                result.Append(values(MyCounter))
            Next
            Return result.ToString
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
        Public Shared Function JoinNameValueCollectionToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection, ByVal BeginningOfItem As String, ByVal KeyValueSeparator As String, ByVal EndOfItem As String) As String
            Dim Result As String = Nothing
            For Each ParamItem As String In NameValueCollectionToString
                Result &= BeginningOfItem & ParamItem & KeyValueSeparator & NameValueCollectionToString(ParamItem) & EndOfItem
            Next
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Join all items of a NameValueCollection (for example Request.QueryString) to a simple string
        ''' </summary>
        ''' <param name="NameValueCollectionToString">A collection like Request.Form or Request.QueryString</param>
        ''' <returns>A string containing all elements of the collection which can be appended to any internet address</returns>
        ''' <remarks>
        '''     If you need to read the values directly from the returned string, pay attention that all names and values might be UrlEncoded and you have to decode them, first.
        ''' </remarks>
        ''' <see also="FillNameValueCollectionWith" />
        ''' <history>
        ''' 	[adminwezel]	09.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function JoinNameValueCollectionWithUrlEncodingToString(ByVal NameValueCollectionToString As Collections.Specialized.NameValueCollection) As String
            Dim Result As String = Nothing
            For Each ParamItem As String In NameValueCollectionToString
                If Result <> Nothing Then
                    Result &= "&"
                End If
                Result &= System.Web.HttpUtility.UrlEncode(ParamItem) & "=" & System.Web.HttpUtility.UrlEncode(NameValueCollectionToString(ParamItem))
            Next
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Restore a NameValueCollection's content which has been previously converted to a string
        ''' </summary>
        ''' <param name="nameValueCollection">An already existing NameValueCollection which shall receive the new values</param>
        ''' <param name="nameValueCollectionWithUrlEncoding">A string containing the UrlEncoded writing style of a NameValueCollection</param>
        ''' <remarks>
        '''     Please note: existing values in the collection won't be appended, they'll be overridden
        ''' </remarks>
        ''' <see also="JoinNameValueCollectionWithUrlEncodingToString" />
        ''' <history>
        ''' 	[AdminSupport]	01.09.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub ReFillNameValueCollection(ByVal nameValueCollection As System.Collections.Specialized.NameValueCollection, ByVal nameValueCollectionWithUrlEncoding As String)

            If nameValueCollection Is Nothing Then
                Throw New ArgumentNullException("nameValueCollection")
            End If
            If nameValueCollectionWithUrlEncoding = Nothing Then
                'Nothing to do
                Return
            End If

            'Split at "&"
            Dim parameters As String() = nameValueCollectionWithUrlEncoding.Split(New Char() {"&"c})
            For MyCounter As Integer = 0 To parameters.Length - 1
                Dim KeyValuePair As String() = parameters(MyCounter).Split(New Char() {"="c})
                If KeyValuePair.Length = 0 Then
                    'empty - nothing to do
                ElseIf KeyValuePair.Length = 1 Then
                    'key name only
                    nameValueCollection(System.Web.HttpUtility.UrlDecode(KeyValuePair(0))) = Nothing
                ElseIf KeyValuePair.Length = 2 Then
                    'key/value pair
                    nameValueCollection(System.Web.HttpUtility.UrlDecode(KeyValuePair(0))) = System.Web.HttpUtility.UrlDecode(KeyValuePair(1))
                Else
                    'invalid data
                    Throw New ArgumentException("Invalid data - more than one equals signs (""="") found", "nameValueCollectionWithUrlEncoding")
                End If
            Next

        End Sub

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
        Public Shared Function ConvertHTMLToText(ByVal HTML As String) As String
            'TODO: 1. remove of all other HTML tags
            '      2. search case insensitive
            '      3. truncate content between <head> and </head>, <script> and </script>, <!-- and -->
            '      Please note: there is already a camm HTML file filter which might get reused here
            Dim Result As String = HTML
            Result = ReplaceString(Result, vbNewLine, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, vbCr, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, vbLf, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&lt;", "<", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&gt;", "<", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&amp;", "&", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&quot;", """", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&micro;", "µ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&sect;", "§", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&copy;", "©", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&reg;", "®", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&trade;", "™", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&bull;", "• ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&euro;", "€", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&mdash;", " – ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&mdash;", " — ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<p>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<br>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<br/>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<br />", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<p.*?>", vbNewLine & vbNewLine)
            Result = ReplaceString(Result, "</p>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<li.*?>", "- ")
            Result = ReplaceString(Result, "</li>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<ol.*?>", "- ")
            Result = ReplaceString(Result, "</ol>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<ul.*?>", "")
            Result = ReplaceString(Result, "</ul>", vbNewLine, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<i.*?>", "")
            Result = ReplaceString(Result, "</i>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<b.*?>", "")
            Result = ReplaceString(Result, "</b>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<em.*?>", "")
            Result = ReplaceString(Result, "</em>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<strong.*?>", "")
            Result = ReplaceString(Result, "</strong>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "<small>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "</small>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<html.*?>", "")
            Result = ReplaceString(Result, "</html>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<body.*?>", "")
            Result = ReplaceString(Result, "</body>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<span.*?>", "")
            Result = ReplaceString(Result, "</span>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<a.*?>", "")
            Result = ReplaceString(Result, "</a>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<font.*?>", "")
            Result = ReplaceString(Result, "</font>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<div.*?>", vbNewLine)
            Result = ReplaceString(Result, "</div>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<input.*?>", vbNewLine)
            Result = ReplaceString(Result, "</input>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<label.*?>", vbNewLine)
            Result = ReplaceString(Result, "</label>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<fieldset.*?>", vbNewLine)
            Result = ReplaceString(Result, "</fieldset>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<option.*?>", vbNewLine)
            Result = ReplaceString(Result, "</option>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<select.*?>", vbNewLine)
            Result = ReplaceString(Result, "</select>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<form.*?>", vbNewLine)
            Result = ReplaceString(Result, "</form>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<optgroup.*?>", vbNewLine)
            Result = ReplaceString(Result, "</optgroup>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<!DOCTYPE.*?>", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<table.*?>", vbNewLine)
            Result = ReplaceString(Result, "</table>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<tr.*?>", vbNewLine)
            Result = ReplaceString(Result, "</tr>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<th.*?>", "")
            Result = ReplaceString(Result, "</th>", vbTab, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<td.*?>", "")
            Result = ReplaceString(Result, "</td>", vbTab, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<thead.*?>", "")
            Result = ReplaceString(Result, "</thead>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<tbody.*?>", "")
            Result = ReplaceString(Result, "</tbody>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<tfoot.*?>", "")
            Result = ReplaceString(Result, "</tfoot>", "", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<caption.*?>", "")
            Result = ReplaceString(Result, "</caption>", vbTab, ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceByRegExIgnoringCase(Result, "<!--.*-->", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<script.*</script?>", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<head?>.*</head?>", "")
            Result = ReplaceByRegExIgnoringCase(Result, "<img.*?>", "")
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreaksBeforeAndAfter(Result, "<hl.*?>", vbNewLine & "-------------------------------------------------------" & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreaksBeforeAndAfter(Result, "<hr.*?>", vbNewLine & "-------------------------------------------------------" & vbNewLine)
            'Result = ReplaceByRegExIgnoringCase(Result, "<h1>.*?</h1>", vbNewLine & Result.ToUpperInvariant & vbnewline)
            'Result = ReplaceByRegExIgnoringCase(Result, "<h2>.*?</h2>", vbNewLine & Result.ToUpperInvariant & vbnewline)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h1.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h1?>", vbNewLine & "=======================================================" & vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h2.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h2?>", vbNewLine & "-------------------------------------------------------" & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h3.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h3?>", vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h4.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h4?>", vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h5.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h5?>", vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(Result, "<h6.*?>", vbNewLine & vbNewLine)
            Result = ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(Result, "</h6?>", vbNewLine)
            Result = ReplaceString(Result, vbTab, " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Dim TextLength As Integer
            Do
                'Loop as long as replacements takes effect
                TextLength = Result.Length
                Result = Result.Replace("  ", " ")
            Loop Until TextLength = Result.Length
            Result = ReplaceString(Result, " &nbsp;", " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&nbsp; ", " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Result = ReplaceString(Result, "&nbsp;", " ", ReplaceComparisonTypes.InvariantCultureIgnoreCase)
            Return Result
        End Function

        Private Shared Function ReplaceByRegExIgnoringCaseEnforcingLineBreaksBeforeAndAfter(html As String, searchExpression As String, replaceValueWithLineBreaksBeforeAndAfter As String) As String
            Dim Result As String = html
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*" & searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreaksBeforeAndAfter)
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*" & searchExpression & "p{Zs}*", replaceValueWithLineBreaksBeforeAndAfter)
            Result = ReplaceByRegExIgnoringCase(Result, "\p{Zs}*" & searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreaksBeforeAndAfter)
            Result = ReplaceByRegExIgnoringCase(Result, "\p{Zs}*" & searchExpression & "\p{Zs}*", replaceValueWithLineBreaksBeforeAndAfter)
            Return Result
        End Function

        Private Shared Function ReplaceByRegExIgnoringCaseEnforcing2LineBreaksBefore(html As String, searchExpression As String, replaceValueWithLineBreaksBefore As String) As String
            Dim Result As String = html
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*(?:\r\n|\r|\n)\p{Zs}*" & searchExpression, replaceValueWithLineBreaksBefore)
            Result = ReplaceByRegExIgnoringCase(Result, "(?:\r\n|\r|\n)\p{Zs}*" & searchExpression, replaceValueWithLineBreaksBefore)
            Result = ReplaceByRegExIgnoringCase(Result, searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreaksBefore)
            Return Result
        End Function

        Private Shared Function ReplaceByRegExIgnoringCaseEnforcingLineBreakAfter(html As String, searchExpression As String, replaceValueWithLineBreakAfter As String) As String
            Dim Result As String = html
            Result = ReplaceByRegExIgnoringCase(Result, searchExpression & "\p{Zs}*(?:\r\n|\r|\n)", replaceValueWithLineBreakAfter)
            Result = ReplaceByRegExIgnoringCase(Result, searchExpression, replaceValueWithLineBreakAfter)
            Return Result
        End Function

        Friend Shared Function ReplaceByRegExIgnoringCase(ByVal text As String, ByVal searchForRegExpression As String, ByVal replaceBy As String) As String
            Return ReplaceByRegExIgnoringCase(text, searchForRegExpression, replaceBy, System.Text.RegularExpressions.RegexOptions.IgnoreCase Or System.Text.RegularExpressions.RegexOptions.CultureInvariant Or System.Text.RegularExpressions.RegexOptions.Multiline)
        End Function

        Friend Shared Function ReplaceByRegExIgnoringCase(ByVal text As String, ByVal searchForRegExpression As String, ByVal replaceBy As String, ByVal options As System.Text.RegularExpressions.RegexOptions) As String
            Return System.Text.RegularExpressions.Regex.Replace(text, searchForRegExpression, replaceBy, options)
        End Function

        ''' <summary>
        ''' String comparison types for ReplaceString method
        ''' </summary>
        ''' <remarks></remarks>
        Friend Enum ReplaceComparisonTypes As Byte
            ''' <summary>
            ''' Compare 2 strings with case sensitivity
            ''' </summary>
            ''' <remarks></remarks>
            CaseSensitive = 0
            ''' <summary>
            ''' Compare 2 strings by lowering their case based on the current culture
            ''' </summary>
            ''' <remarks></remarks>
            CurrentCultureIgnoreCase = 1
            ''' <summary>
            ''' Compare 2 strings by lowering their case following invariant culture rules
            ''' </summary>
            ''' <remarks></remarks>
            InvariantCultureIgnoreCase = 2
        End Enum

        ''' <summary>
        ''' Replace a string in another string based on a defined StringComparison type
        ''' </summary>
        ''' <param name="original">The original string</param>
        ''' <param name="pattern">The search expression</param>
        ''' <param name="replacement">The string which shall be inserted instead of the pattern</param>
        ''' <param name="comparisonType">The comparison type for searching for the pattern</param>
        ''' <returns>A new string with all replacements</returns>
        ''' <remarks></remarks>
        Friend Shared Function ReplaceString(ByVal original As String, ByVal pattern As String, ByVal replacement As String, ByVal comparisonType As ReplaceComparisonTypes) As String
            If original = Nothing OrElse pattern = Nothing Then
                Return original
            End If
            Dim lenPattern As Integer = pattern.Length
            Dim idxPattern As Integer = -1
            Dim idxLast As Integer = 0
            Dim result As New System.Text.StringBuilder
            Select Case comparisonType
                Case ReplaceComparisonTypes.CaseSensitive
                    While True
                        idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType)
                        If idxPattern < 0 Then
                            result.Append(original, idxLast, original.Length - idxLast)
                            Exit While
                        End If
                        result.Append(original, idxLast, idxPattern - idxLast)
                        result.Append(replacement)
                        idxLast = idxPattern + lenPattern
                    End While
                Case ReplaceComparisonTypes.CurrentCultureIgnoreCase
                    While True
                        Dim comparisonStringOriginal As String, comparisonStringPattern As String
                        comparisonStringOriginal = original.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        comparisonStringPattern = pattern.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        idxPattern = comparisonStringOriginal.IndexOf(comparisonStringPattern, idxPattern + 1)
                        If idxPattern < 0 Then
                            result.Append(original, idxLast, original.Length - idxLast)
                            Exit While
                        End If
                        result.Append(original, idxLast, idxPattern - idxLast)
                        result.Append(replacement)
                        idxLast = idxPattern + lenPattern
                    End While
                Case ReplaceComparisonTypes.InvariantCultureIgnoreCase
                    While True
                        Dim comparisonStringOriginal As String, comparisonStringPattern As String
                        comparisonStringOriginal = original.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        comparisonStringPattern = pattern.ToLower(System.Globalization.CultureInfo.CurrentCulture)
                        idxPattern = comparisonStringOriginal.IndexOf(comparisonStringPattern, idxPattern + 1)
                        If idxPattern < 0 Then
                            result.Append(original, idxLast, original.Length - idxLast)
                            Exit While
                        End If
                        result.Append(original, idxLast, idxPattern - idxLast)
                        result.Append(replacement)
                        idxLast = idxPattern + lenPattern
                    End While
                Case Else
                    Throw New ArgumentOutOfRangeException("comparisonType", "Invalid value")
            End Select
            Return result.ToString()
        End Function

#End Region

#Region "Type conversions"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Convert a boolean value into the corresponding value of a TriState
        ''' </summary>
        ''' <param name="value">A boolean value</param>
        ''' <returns>A value of type TriState with either TriState.True or TriState.False</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	09.11.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function BooleanToTristate(ByVal value As Boolean) As TriState
            If value Then
                Return TriState.True
            Else
                Return TriState.False
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Convert a TriState value into a corresponding boolean value
        ''' </summary>
        ''' <param name="value">A value of type TriState with either TriState.True or TriState.False</param>
        ''' <returns>A boolean value with either True or False</returns>
        ''' <remarks>
        '''     If the input value is TriState.Default, there will be thrown an ArgumentException
        ''' </remarks>
        ''' <history>
        ''' 	[wezel]	09.11.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function TristateToBoolean(ByVal value As TriState) As Boolean
            If value = TriState.True Then
                Return True
            ElseIf value = TriState.False Then
                Return False
            Else
                Throw New ArgumentException("value must be TriState.True or TriState.False", "value")
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the string which is not nothing or else String.Empty
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StringNotEmptyOrNothing(ByVal value As String) As String
            If value = Nothing Then
                Return Nothing
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the string which is not nothing or else String.Empty
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StringNotNothingOrEmpty(ByVal value As String) As String
            If value Is Nothing Then
                Return String.Empty
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the string which is not nothing or else the alternative value
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <param name="alternativeValue">An alternative value if the first value is nothing</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StringNotNothingOrAlternativeValue(ByVal value As String, ByVal alternativeValue As String) As String
            If value Is Nothing Then
                Return alternativeValue
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the string which is not empty or else the alternative value
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <param name="alternativeValue">An alternative value if the first value is empty</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StringNotEmptyOrAlternativeValue(ByVal value As String, ByVal alternativeValue As String) As String
            If value = Nothing Then
                Return alternativeValue
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the string which is not empty or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StringNotEmptyOrDBNull(ByVal value As String) As Object
            If value = Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the object which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ObjectNotNothingOrEmptyString(ByVal value As Object) As Object
            If value Is Nothing Then
                Return String.Empty
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the object which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ObjectNotNothingOrDBNull(ByVal value As Object) As Object
            If value Is Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the object which is not an empty string or otherwise return Nothing
        ''' </summary>
        ''' <param name="value">The object to be validated</param>
        ''' <returns>A string with length > 0 (the value) or nothing</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ObjectNotEmptyStringOrNothing(ByVal value As Object) As Object
            If value Is Nothing Then
                Return Nothing
            ElseIf value.GetType Is GetType(String) AndAlso CType(value, String) = "" Then
                Return Nothing
            Else
                Return value
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the string which is not nothing or otherwise return DBNull.Value 
        ''' </summary>
        ''' <param name="value">The string to be validated</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	09.11.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function StringNotNothingOrDBNull(ByVal value As String) As Object
            If value Is Nothing Then
                Return DBNull.Value
            Else
                Return value
            End If
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
        Public Shared Function TryCLng(ByVal Expression As Object) As Long
            Return TryCLng(Expression, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a long value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted long value or the alternative value if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCLng(ByVal Expression As Object, ByVal AlternativeValue As Long) As Long
            Try
                Return CLng(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a integer value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted integer value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCInt(ByVal Expression As Object) As Integer
            Return TryCInt(Expression, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a integer value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted integer value or the alternative value if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCInt(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Integer
            Try
                Return CInt(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a double value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted double value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCDbl(ByVal Expression As Object) As Double
            Return TryCDbl(Expression, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a double value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted double value or the alternative value if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCDbl(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Double
            Try
                Return CDbl(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a decimal value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <returns>The converted decimal value or null (Nothing in VisualBasic) if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCDec(ByVal Expression As Object) As Decimal
            Return TryCDec(Expression, Nothing)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Tries to convert the expression into a decimal value, but never throws an exception
        ''' </summary>
        ''' <param name="Expression">The expression to be converted</param>
        ''' <param name="AlternativeValue">The alternative value in case of conversion errors</param>
        ''' <returns>The converted decimal value or the alternative value if the conversion was unsuccessfull</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function TryCDec(ByVal Expression As Object, ByVal AlternativeValue As Integer) As Decimal
            Try
                Return CDec(Expression)
            Catch
                Return AlternativeValue
            End Try
        End Function

#End Region

#Region "Assembly & Resources"

        ''' <summary>
        ''' Create an instance of an object which is defined by assembly name and class name
        ''' </summary>
        ''' <param name="assemblyName">The name of an assembly (e.g. "System.Data")</param>
        ''' <param name="className">A class name (e.g. "System.Data.DataTable")</param>
        ''' <returns>The created instance of the requested</returns>
        ''' <remarks>The loading procedure is in following order:
        ''' <list>
        ''' <item>Assemblies already loaded into the current application domain</item>
        ''' <item>Assembly loaded from disc (with standard search order in application folder, GAC, etc.)</item>
        ''' </list>
        ''' <para></para>
        ''' </remarks>
        ''' <exception cref="Exception">If the object can't be created or the assembly can't be loaded, there will be a System.Exception</exception>
        Friend Function CreateObject(ByVal assemblyName As String, ByVal className As String) As Object
            Return CreateObject(assemblyName, className, New Object() {})
        End Function
        ''' <summary>
        ''' Create an instance of an object which is defined by assembly name and class name
        ''' </summary>
        ''' <param name="assemblyName">An assembly name which is already loaded into the current application domain (e.g. "System.Data")</param>
        ''' <param name="className">A class name (e.g. "System.Data.DataTable")</param>
        ''' <param name="parameters">Parameters for the constructor of the new class instance</param>
        ''' <returns>The created instance of the requested</returns>
        ''' <remarks>The loading procedure is in following order:
        ''' <list>
        ''' <item>Assemblies already loaded into the current application domain</item>
        ''' <item>Assembly loaded from disc (with standard search order in application folder, GAC, etc.)</item>
        ''' </list>
        ''' <para></para>
        ''' </remarks>
        ''' <exception cref="Exception">If the object can't be created or the assembly can't be loaded, there will be a System.Exception</exception>
        Friend Function CreateObject(ByVal assemblyName As String, ByVal className As String, ByVal parameters As Object()) As Object
            Dim Result As Object = Nothing
            For Each MyAssembly As System.Reflection.Assembly In System.AppDomain.CurrentDomain.GetAssemblies
                If MyAssembly.GetName.Name.ToLower = assemblyName.ToLower Then
                    Result = MyAssembly.CreateInstance(className, True, Reflection.BindingFlags.Default, Nothing, parameters, Nothing, Nothing)
                    If Result Is Nothing Then
                        Throw New Exception("Object class could not be created")
                    End If
                    Exit For
                End If
            Next
            If Result Is Nothing Then
                Dim MyAssembly As System.Reflection.Assembly
                MyAssembly = System.Reflection.Assembly.Load(assemblyName)
                Result = MyAssembly.CreateInstance(className, True, Reflection.BindingFlags.Default, Nothing, parameters, Nothing, Nothing)
                If Result Is Nothing Then
                    Throw New Exception("Object class could not be created")
                End If
            End If
            If Result Is Nothing Then
                Throw New Exception("Assembly name not loaded in current application domain and not loadable on request")
            End If
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is the assembly already loaded into the current application domain?
        ''' </summary>
        ''' <param name="assemblyName">The name of an assembly</param>
        ''' <returns>True if the assembly is loaded, False if not</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	16.02.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function IsAssemblyAlreadyLoaded(ByVal assemblyName As String) As Boolean
            For Each MyAssembly As System.Reflection.Assembly In System.AppDomain.CurrentDomain.GetAssemblies
                If MyAssembly.GetName.Name.ToLower = assemblyName.ToLower Then
                    Return True
                End If
            Next
            Return False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load a value from an embedded resource file
        ''' </summary>
        ''' <param name="resourceFileWithoutExtension">The name of the .resx file</param>
        ''' <param name="key">The key of the requested value</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	21.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function ResourceValue(ByVal resourceFileWithoutExtension As String, ByVal key As String) As String
            Dim Result As String = Nothing

            Dim ResMngr As Resources.ResourceManager = Nothing
            Try
                ResMngr = New Resources.ResourceManager(resourceFileWithoutExtension, System.Reflection.Assembly.GetExecutingAssembly)
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(key)
            Catch ex As Exception
                Throw New Exception("Embedded resource """ & key & """ in """ & resourceFileWithoutExtension & """ can't be found", ex)
            Finally
                If Not ResMngr Is Nothing Then ResMngr.ReleaseAllResources()
            End Try

            Return Result

        End Function

        ''' <summary>
        ''' Read an embedded, binary resource file
        ''' </summary>
        ''' <param name="embeddedFileName">The name of the resouces</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Shared Function ResourceBinaryValue(ByVal embeddedFileName As String) As Byte()
            Dim stream As System.IO.Stream = Nothing
            Dim buffer As Byte()
            Try
                Try
                    stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedFileName)
                Catch ex As Exception
                    Throw New Exception("Failure while loading resource name """ & embeddedFileName & """" & vbNewLine & "Available resource names are: " & String.Join(",", System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames), ex)
                End Try
                ReDim buffer(CInt(stream.Length) - 1)
                stream.Read(buffer, 0, CInt(stream.Length))
            Finally
                If Not stream Is Nothing Then stream.Close()
            End Try
            Return buffer
        End Function

#End Region

#Region "Paths and URIs"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Remove a possibly trailing slash from an URL
        ''' </summary>
        ''' <param name="url">An URL address</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	08.10.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function RemoveTrailingSlash(ByVal url As String) As String
            If url.Length > 0 AndAlso Right(url, 1) = "/" Then
                Return Mid(url, 1, url.Length - 1)
            Else
                Return url
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     If the path is a unix path with a filename at the end, the file name will be removed. The resulting path always ends with a "/".
        ''' </summary>
        ''' <param name="path">A unix path with or without a filename</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' All letters behind the last slash will be removed, so a path ending with a slash will never be modified.
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	07.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function RemoveFilenameInUnixPath(ByVal path As String) As String
            If path Is Nothing Then
                Return Nothing
            ElseIf path.EndsWith("/") Then
                Return path
            ElseIf path.IndexOf("/"c) < 0 Then
                Return path
            Else
                Return path.Substring(0, path.LastIndexOf("/"c) + 1)
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the full virtual path based on the given string
        ''' </summary>
        ''' <param name="virtualPath">A path like ~/images or images/styles or /images/</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[swiercz]	06.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function FullyInterpretedVirtualPath(ByVal virtualPath As String) As String
            If virtualPath Is Nothing Then
                Throw New ArgumentNullException("virtualPath")
            End If
            If virtualPath.StartsWith("~/") Then
                virtualPath = Replace(virtualPath, "~/", "")
                Dim myApplicationPath As String = System.Web.HttpContext.Current.Request.ApplicationPath()
                If Not myApplicationPath.EndsWith("/") Then
                    myApplicationPath = myApplicationPath & "/"
                End If
                virtualPath = myApplicationPath & virtualPath
            ElseIf virtualPath.StartsWith("/") Then
                'Do nothing, because it is already the servers rootpath
            Else
                Dim currentVirtualPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
                If Not currentVirtualPath.EndsWith("/") Then
                    currentVirtualPath = currentVirtualPath.Substring(0, currentVirtualPath.LastIndexOf("/") + 1)
                End If
                virtualPath = currentVirtualPath & virtualPath
            End If
            Return virtualPath
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the full physical path based on the given string
        ''' </summary>
        ''' <param name="virtualPath">A path like ~/images or images/styles or /images/</param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Requires execution on a web server (because HttpContext must be there)
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	06.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function FullyInterpredetPhysicalPath(ByVal virtualPath As String) As String
            Return System.Web.HttpContext.Current.Server.MapPath(virtualPath)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The script name without path and query information, only the file name itself
        ''' </summary>
        ''' <returns>E. g. "index.aspx"</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	24.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function ScriptNameWithoutPath() As String
            Dim result As String

            If System.Environment.Version.Major >= 4 AndAlso System.Web.HttpContext.Current.Request.RawUrl.EndsWith("/") Then
                'Beginning with .NET 4, RawUrl contains the URL as requested by the client, so the script name after a folder might be missing; e.g. /test/ is given, but required is /test/default.aspx later on
                result = System.Web.HttpContext.Current.Request.Url.AbsolutePath
            Else
                '.NET 1 + 2: RawUrl contains the URL as requested by the client + the request script name, so the script name after a folder is present; e.g. /test/ is given, RawUrl returns the expected /test/default.aspx
                result = System.Web.HttpContext.Current.Request.RawUrl
            End If

            If result.IndexOf("?"c) > -1 Then
                result = result.Substring(0, result.IndexOf("?"c))
            End If
            If result.LastIndexOf("/"c) > -1 Then
                result = result.Substring(result.LastIndexOf("/"c) + 1)
            End If

            Return result
        End Function
#End Region

#Region "HttpCaching"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Set an HttpCache item to a new value respectively remove it if the value is Nothing
        ''' </summary>
        ''' <param name="key">The key name for the cache item</param>
        ''' <param name="value">The new value</param>
        ''' <param name="priority">The cache priority</param>
        ''' <exception cref="System.SystemException">
        ''' If HttpContext.Current is not available (if it is not a web application), there will be thrown a normal System.Exception
        ''' </exception>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	27.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Sub SetHttpCacheValue(ByVal key As String, ByVal value As Object, ByVal priority As System.Web.Caching.CacheItemPriority)
            SetHttpCacheValue(key, value, System.Web.Caching.Cache.NoSlidingExpiration, priority)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Set an HttpCache item to a new value respectively remove it if the value is Nothing
        ''' </summary>
        ''' <param name="key">The key name for the cache item</param>
        ''' <param name="value">The new value</param>
        ''' <param name="slidingTimespan">A timespan with a sliding expiration date</param>
        ''' <param name="priority">The cache priority</param>
        ''' <exception cref="System.SystemException">
        ''' If HttpContext.Current is not available (if it is not a web application), there will be thrown a normal System.Exception
        ''' </exception>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	27.04.2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Sub SetHttpCacheValue(ByVal key As String, ByVal value As Object, ByVal slidingTimespan As TimeSpan, ByVal priority As System.Web.Caching.CacheItemPriority)
            If key = Nothing Then
                Throw New ArgumentNullException("key")
            End If
            If Not System.Web.HttpContext.Current Is Nothing Then
                If value Is Nothing Then
                    'Remove item
                    System.Web.HttpContext.Current.Cache.Remove(key)
                ElseIf System.Web.HttpContext.Current.Cache(key) Is Nothing Then
                    'Insert new item
                    System.Web.HttpContext.Current.Cache.Add(key, value, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingTimespan, priority, Nothing)
                Else
                    'Update existing item
                    System.Web.HttpContext.Current.Cache(key) = value
                End If
            Else
                Throw New Exception("HttpContext.Current not available - caching impossible")
            End If
        End Sub
#End Region

#Region "Lookup of server form"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <param name="control"></param>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function LookupParentServerForm(ByVal control As System.Web.UI.Control) As System.Web.UI.HtmlControls.HtmlForm
            If control Is Nothing Then
                Throw New ArgumentNullException("control")
            End If
            If control.Parent Is Nothing Then
                Return Nothing
            Else
                Dim Result As System.Web.UI.HtmlControls.HtmlForm = Nothing
                'Is the parent already the server form?
                If GetType(System.Web.UI.HtmlControls.HtmlForm).IsInstanceOfType(control.Parent) Then
                    Result = CType(control.Parent, System.Web.UI.HtmlControls.HtmlForm)
                End If
                If Result Is Nothing Then
                    'Retry search in parent control
                    Return LookupParentServerForm(control.Parent)
                End If
                Return Result
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Lookup the server form which resides on the page
        ''' </summary>
        ''' <param name="page"></param>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Shared Function LookupServerForm(ByVal page As System.Web.UI.Page) As System.Web.UI.HtmlControls.HtmlForm
            Return LookupServerForm(page.Controls, True)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Search in the controls collection for a server form
        ''' </summary>
        ''' <param name="controls"></param>
        ''' <param name="searchInChildren">True to execute the search in the controls collection recursively, False to not search in any children controls</param>
        ''' <returns>The control of the server form if it's existant</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function LookupServerForm(ByVal controls As System.Web.UI.ControlCollection, ByVal searchInChildren As Boolean) As System.Web.UI.HtmlControls.HtmlForm
            Dim Result As System.Web.UI.HtmlControls.HtmlForm = Nothing
            'Search in controls
            For MyCounter As Integer = 0 To controls.Count - 1
                If GetType(System.Web.UI.HtmlControls.HtmlForm).IsInstanceOfType(controls(MyCounter)) Then
                    Result = CType(controls(MyCounter), System.Web.UI.HtmlControls.HtmlForm)
                End If
            Next
            'Search in children of controls
            If Result Is Nothing Then
                For MyCounter As Integer = 0 To controls.Count - 1
                    If controls(MyCounter).HasControls = True Then
                        Result = LookupServerForm(controls(MyCounter).Controls, True)
                    End If
                Next
            End If
            'Return the result
            Return Result
        End Function
#End Region

    End Class

#Region "StringArray conversion"
    ''' <summary>
    '''     Allow a string array property to be filled by a comma separated string
    ''' </summary>
    Friend Class StringArrayConverter
        Inherits System.ComponentModel.CollectionConverter

        Public Sub New()
        End Sub

        Public Overloads Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            If (sourceType Is GetType(String)) Then
                Return True
            End If
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overloads Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            If (destinationType Is GetType(String())) Then
                Return True
            End If
            Return MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overloads Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal sourceObj As Object) As Object
            If TypeOf sourceObj Is String Then
                Dim chArray1 As Char() = New Char() {","c}
                Return CType(sourceObj, String).Split(chArray1)
            End If
            Return MyBase.ConvertFrom(context, culture, sourceObj)
        End Function

        Public Overloads Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal destinationObj As Object, ByVal destinationType As Type) As Object
            If TypeOf destinationObj Is String() Then
                Dim text1 As String = String.Join(",", CType(destinationObj, String()))
                If (destinationType Is GetType(System.ComponentModel.Design.Serialization.InstanceDescriptor)) Then
                    Dim typeArray1 As Type() = New Type() {GetType(String)}
                    Dim info1 As System.Reflection.ConstructorInfo = GetType(String).GetConstructor(typeArray1)
                    If (info1 Is Nothing) Then
                        GoTo Label_007B
                    End If
                    Dim objArray1 As Object() = New Object() {text1}
                    Return New System.ComponentModel.Design.Serialization.InstanceDescriptor(info1, objArray1)
                End If
                If (destinationType Is GetType(String)) Then
                    Return text1
                End If
            End If
Label_007B:
            Return MyBase.ConvertTo(context, culture, destinationObj, destinationType)
        End Function
    End Class

    ''' <summary>
    '''     Allow an integer array property to be filled by a comma separated string
    ''' </summary>
    Friend Class IntegerArrayConverter
        Inherits System.ComponentModel.CollectionConverter

        Public Sub New()
        End Sub

        Public Overloads Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As Type) As Boolean
            If (sourceType Is GetType(String)) Then
                Return True
            End If
            Return MyBase.CanConvertFrom(context, sourceType)
        End Function

        Public Overloads Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As Type) As Boolean
            If (destinationType Is GetType(Integer())) Then
                Return True
            End If
            Return MyBase.CanConvertTo(context, destinationType)
        End Function

        Public Overloads Overrides Function ConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal sourceObj As Object) As Object
            If TypeOf sourceObj Is String Then
                Dim chArray1 As Char() = New Char() {","c}
                Return ConvertStringArrayToIntegerArray(CType(sourceObj, String).Split(chArray1))
            End If
            Return MyBase.ConvertFrom(context, culture, sourceObj)
        End Function

        Private Function ConvertStringArrayToIntegerArray(values As String()) As Integer()
            Dim Result As New ArrayList
            For Each value As String In values
                If value = "" Then
                    Result.Add(0)
                Else
                    Result.Add(Integer.Parse(value))
                End If
            Next
            Return CType(Result.ToArray(GetType(Integer)), Integer())
        End Function

        Public Overloads Overrides Function ConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal culture As System.Globalization.CultureInfo, ByVal destinationObj As Object, ByVal destinationType As Type) As Object
            If TypeOf destinationObj Is Integer() Then
                Dim text1 As String = String.Join(",", CType(destinationObj, String()))
                If (destinationType Is GetType(System.ComponentModel.Design.Serialization.InstanceDescriptor)) Then
                    Dim typeArray1 As Type() = New Type() {GetType(Integer)}
                    Dim info1 As System.Reflection.ConstructorInfo = GetType(Integer).GetConstructor(typeArray1)
                    If (info1 Is Nothing) Then
                        GoTo Label_007B
                    End If
                    Dim objArray1 As Object() = New Object() {text1}
                    Return New System.ComponentModel.Design.Serialization.InstanceDescriptor(info1, objArray1)
                End If
                If (destinationType Is GetType(Integer)) Then
                    Return text1
                End If
            End If
Label_007B:
            Return MyBase.ConvertTo(context, culture, destinationObj, destinationType)
        End Function
    End Class
#End Region

End Namespace