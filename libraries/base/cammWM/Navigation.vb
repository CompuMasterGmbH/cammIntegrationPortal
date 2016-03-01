'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.

Option Explicit On
Option Strict On

Namespace CompuMaster.camm.WebManager.Navigation

    ''' <summary>
    '''     Common utilities for creation of a navigation menu
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Module Utils

        ''' <summary>
        ''' Render an SEO optimized navigation inside of a noscript tag
        ''' </summary>
        ''' <param name="navitems">The navigation items</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SeoNavigation(ByVal navitems As System.Data.DataTable) As String
            Dim BasicNav As New System.Text.StringBuilder()
            BasicNav.Append("<noscript><div class=""SeoNavigation"">")
            For MyCounter As Integer = 0 To navitems.Rows.Count - 1
                Dim NavUrl As String = CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("UrlAutoCompleted"), "")
                Dim NavTarget As String = CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Target"), "")
                Dim NavTooltipTitle As String = CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Tooltip"), "")
                If NavUrl <> "" Then
                    BasicNav.Append("<a href=""")
                    BasicNav.Append(NavUrl)
                    BasicNav.Append("""")
                    If NavTarget <> "" Then
                        BasicNav.Append(" target=""" & NavTarget & """")
                    End If
                    If NavTooltipTitle <> "" Then
                        BasicNav.Append(" title=""" & NavTooltipTitle & """")
                    End If
                    BasicNav.Append(">")
                    If CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("IsHtmlEncoded"), False) = True Then
                        BasicNav.Append(TitleLastLevel(CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Title"), "")))
                    Else
                        BasicNav.Append(HttpUtility.HtmlEncode(TitleLastLevel(CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Title"), ""))))
                    End If
                    BasicNav.Append("</a><br>")
                End If
            Next
            BasicNav.Append("</div></noscript>")
            Return BasicNav.ToString()
        End Function

        ''' <summary>
        ''' Lookup the last title level from a back-slash-separated title hierarchy
        ''' </summary>
        ''' <param name="fullTitleHierarchy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TitleLastLevel(ByVal fullTitleHierarchy As String) As String
            If fullTitleHierarchy = Nothing Then
                Return fullTitleHierarchy
            ElseIf fullTitleHierarchy.LastIndexOf("\"c) >= 0 Then
                Return Mid(fullTitleHierarchy, fullTitleHierarchy.LastIndexOf("\"c))
            Else
                Return fullTitleHierarchy
            End If
        End Function

        ''' <summary>
        ''' Lookup the index of the given control in the control collection of the parent control
        ''' </summary>
        ''' <param name="control">A control</param>
        ''' <returns>An index number or -1 if no parent control is available</returns>
        ''' <remarks></remarks>
        Public Function LookupControlIndexAtParentControl(ByVal control As System.Web.UI.Control) As Integer
            If control.Parent Is Nothing Then
                Return -1
            Else
                For MyCounter As Integer = 0 To control.Parent.Controls.Count - 1
                    If control Is control.Parent.Controls(MyCounter) Then
                        Return MyCounter
                    End If
                Next
                Return -1
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create an empty data table which can contain navigation items
        ''' </summary>
        ''' <returns>A DataTable with all required DataColumns</returns>
        ''' <remarks>
        '''     The table has got following columns:
        ''' <list>
        '''     <item>ID: An ID to reidentify the navigation item</item>
        '''     <item>Title: A back slash ("\") separated string with the complete path for this navigation item
        '''         <example>News\Company\Investments</example></item>
        '''     <item>Sort: An integer value for sorting purposes</item>
        '''     <item>Tooltip: the tooltip text of an element of the navigation</item>
        ''' </list>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateEmptyDataTable() As DataTable
            Dim Result As New DataTable
            Result.Columns.Add(New DataColumn("ID", GetType(System.String)))
            Result.Columns.Add(New DataColumn("Title", GetType(System.String)))
            Result.Columns.Add(New DataColumn("Sort", GetType(Integer)))
            Result.Columns.Add(New DataColumn("Tooltip", GetType(System.String)))
            Result.Columns.Add(New DataColumn("IsNew", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("IsUpdated", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("URLPreDefinition", GetType(System.String)))
            Result.Columns.Add(New DataColumn("URLAutoCompleted", GetType(System.String)))
            Result.Columns.Add(New DataColumn("Target", GetType(System.String)))
            Result.Columns.Add(New DataColumn("IsHtmlEncoded", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("IsDisabledInStandardSituations", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("LeftIconSrc", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconSrcOver", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconSrcDown", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconWidth", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconHeight", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconSrc", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconSrcOver", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconSrcDown", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconWidth", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconHeight", GetType(System.String)))
            Result.Columns.Add(New DataColumn("NoWrap", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("CssClass", GetType(System.String)))
            Result.Columns.Add(New DataColumn("CssClassOver", GetType(System.String)))
            Result.Columns.Add(New DataColumn("CssClassDown", GetType(System.String)))
            Result.Columns.Add(New DataColumn("ClientSideOnClick", GetType(System.String)))

            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Verify the data to contain nothing invalid or duplicates and add required intermediate levels
        ''' </summary>
        ''' <param name="navData">The table with the navigation elements</param>
        ''' <remarks>
        '''     Other checked things are e. g. no singles back-slashes in category or that the URLAutoCompleted contains at least the URLPreDefinition value
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub VerifyDataAndFillMissingElements(ByVal navData As DataTable)
            Dim ColumnIndexTitle As Integer = navData.Columns("Title").Ordinal

            VerifyDataAndFillMissingElementsFixTitleContent(navData, ColumnIndexTitle)
            VerifyDataAndFillMissingElementsAddMissingHierarchyLevels(navData, ColumnIndexTitle)
            VerifyDataAndFillMissingAutoCompletedURLs(navData)
            VerifyDataAndFillMissingElementsRemoveDuplicates(navData, ColumnIndexTitle)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fix title contents
        ''' </summary>
        ''' <param name="navdata"></param>
        ''' <param name="columnIndexTitle"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingElementsFixTitleContent(ByVal navdata As DataTable, ByVal columnIndexTitle As Integer)

            'Fix title content by truncating trailing backslashes, etc.
            Dim MyRows As DataRow() = navdata.Select("", "Title ASC")

            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim MyCurrentRow As DataRow = MyRows(MyCounter)
                Dim MyCurrentTitle As String = CompuMaster.camm.WebManager.Utils.Nz(MyCurrentRow(columnIndexTitle), "")
                'fix leading but prohibited back slashes in front of a non-root value
                While MyCurrentTitle.Length > 1 AndAlso Mid(MyCurrentTitle, 1, 1) = "\"
                    MyCurrentTitle = Mid(MyCurrentTitle, 2)
                    MyCurrentRow(columnIndexTitle) = MyCurrentTitle
                End While
                'fix trailing but prohibited back slashes in front of a non-root value
                While MyCurrentTitle.Length > 1 AndAlso Right(MyCurrentTitle, 1) = "\"
                    MyCurrentTitle = Mid(MyCurrentTitle, 1, MyCurrentTitle.Length - 1)
                    MyCurrentRow(columnIndexTitle) = MyCurrentTitle
                End While
                If MyCurrentTitle = "\" Then
                    MyCurrentTitle = ""
                    MyCurrentRow(columnIndexTitle) = MyCurrentTitle
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add missing hierarchy levels
        ''' </summary>
        ''' <param name="navData"></param>
        ''' <param name="columnIndexTitle"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingElementsAddMissingHierarchyLevels(ByVal navData As DataTable, ByVal columnIndexTitle As Integer)

            Dim ColumnIndexID As Integer = navData.Columns("ID").Ordinal
            Dim MyRows As DataRow() = navData.Select("", "Title ASC")

            'Fill hashtable with all available hierarchy titles
            Dim ht As New Hashtable
            For Each r As DataRow In MyRows
                If Not ht.ContainsKey(r(columnIndexTitle)) Then
                    ht.Add(CompuMaster.camm.WebManager.Utils.Nz(r(columnIndexTitle), ""), Nothing)
                End If
            Next

            'Find missing intermediate levels by splitting every hierarchy title, reconstruction of every sublevel and adding to the hashtable if not yet there
            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim MyCurrentRow As DataRow
                Dim MyCurrentTitle As String
                'update references to current/previous row
                MyCurrentRow = MyRows(MyCounter)
                MyCurrentTitle = CompuMaster.camm.WebManager.Utils.Nz(MyCurrentRow(columnIndexTitle), "")
                'comparisons to detect missing intermediate levels
                Dim MyCurrentTitleSubParts As String() = MyCurrentTitle.Split("\"c)
                For MyLevelCounter As Integer = 0 To MyCurrentTitleSubParts.Length - 1
                    Dim NewEntry As String = String.Join("\", MyCurrentTitleSubParts, 0, MyLevelCounter + 1)
                    If Not ht.ContainsKey(NewEntry) Then
                        'missing intermediate level found - add it now
                        Dim MyNewRow As DataRow = navData.NewRow()
                        MyNewRow(columnIndexTitle) = NewEntry
                        navData.Rows.Add(MyNewRow)
                        ht.Add(NewEntry, Nothing)
                    End If
                Next
            Next
            ht = Nothing

        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Copy missing values from base navigation URL to auto-completed URL field
        ''' </summary>
        ''' <param name="navdata"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingAutoCompletedURLs(ByVal navdata As DataTable)

            Dim MyRows As DataRow() = navdata.Select("", "Title ASC")

            'Copy the URLPreDefinition value to the URLAutoCompleted field if there hasn`t been a value yet
            Dim ColumnIndexUrlAutoCompleted As Integer = navdata.Columns("URLAutoCompleted").Ordinal
            Dim ColumnIndexUrlPredefined As Integer = navdata.Columns("URLPreDefinition").Ordinal
            For MyCounter As Integer = 0 To MyRows.Length - 1
                If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)(ColumnIndexUrlAutoCompleted), "") = "" Then
                    MyRows(MyCounter)(ColumnIndexUrlAutoCompleted) = MyRows(MyCounter)(ColumnIndexUrlPredefined)
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Remove any doubled rows
        ''' </summary>
        ''' <param name="navData"></param>
        ''' <param name="columnIndexTitle"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingElementsRemoveDuplicates(ByVal navData As DataTable, ByVal columnIndexTitle As Integer)
            'Remove now all duplicates found 
            Dim MyRows As DataRow() = navData.Select("", "Title ASC")
            Dim MyCurrentRow As DataRow
            Dim MyPreviousRow As DataRow
            MyCurrentRow = Nothing
            MyPreviousRow = Nothing
            For MyCounter As Integer = 0 To MyRows.Length - 1
                If Not MyCurrentRow Is Nothing AndAlso MyCurrentRow.RowState = DataRowState.Detached Then
                    'if we are in a second round and the old (currently called as "MyCurrentRow" because 
                    'it would be switched in the next line) row has been deleted, then keep the value of the very previous row
                Else
                    MyPreviousRow = MyCurrentRow
                End If
                MyCurrentRow = MyRows(MyCounter)
                Dim MyPreviousTitle As String
                Dim MyCurrentTitle As String
                If MyPreviousRow Is Nothing OrElse IsDBNull(MyPreviousRow(columnIndexTitle)) Then
                    MyPreviousTitle = Nothing
                Else
                    MyPreviousTitle = CompuMaster.camm.WebManager.Utils.Nz(MyPreviousRow(columnIndexTitle), CType(Nothing, String))
                End If
                If IsDBNull(MyCurrentRow(columnIndexTitle)) Then
                    MyCurrentTitle = Nothing
                Else
                    MyCurrentTitle = CompuMaster.camm.WebManager.Utils.Nz(MyCurrentRow(columnIndexTitle), CType(Nothing, String))
                End If
                If MyCurrentTitle = MyPreviousTitle Or MyCurrentTitle = "" Then
                    'this is a dubplicate, mark to remove this line
                    navData.Rows.Remove(MyCurrentRow)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is a searched category member of another list of categories at a defined sub level?
        ''' </summary>
        ''' <param name="searchForCategory">The searched category name</param>
        ''' <param name="listOfCategories">The semi-colon separated list of categories</param>
        ''' <param name="searchWithinSubelements">The path of the sub level where the searched category shall be found</param>
        ''' <returns>True if the searched category is already there</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsCategoryInListOfCategories(ByVal searchForCategory As String, ByVal listOfCategories As String, ByVal searchWithinSubelements As Boolean) As Boolean
            Dim CurCategory As String
            If Trim(listOfCategories) = "" Then
                listOfCategories = "\"
            End If
            If Trim(searchForCategory) = "" Then
                searchForCategory = "\"
            End If
            If Left(searchForCategory, 1) = "\" AndAlso searchForCategory.Length > 1 Then
                'truncate unneeded, trailing slash
                searchForCategory = Mid(searchForCategory, 2)
            End If
            If Right(searchForCategory, 1) = "\" AndAlso searchForCategory.Length > 1 Then
                'truncate unneeded, trailing slash
                searchForCategory = Mid(searchForCategory, 1, searchForCategory.Length - 1)
            End If
            If searchWithinSubelements = True AndAlso searchForCategory = "\" Then
                'root level is always there
                Return True
            End If
            For Each CurCategory In listOfCategories.Split(";"c)
                CurCategory = Trim(CurCategory)
                If CurCategory = "" Then
                    CurCategory = "\"
                End If
                If Right(CurCategory, 1) = "\" AndAlso CurCategory.Length > 1 Then
                    'truncate unneeded, trailing slash
                    CurCategory = Mid(CurCategory, 1, CurCategory.Length - 1)
                End If
                If searchWithinSubelements = True Then
                    'searched category found by same path or by any corresponding sub paths
                    If CurCategory = searchForCategory OrElse Mid(CurCategory, 1, searchForCategory.Length + 1) = searchForCategory & "\" Then
                        Return True
                    End If
                Else
                    'exact match required
                    If CurCategory = searchForCategory Then
                        Return True
                    End If
                End If
            Next
            Return False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A list of category paths for all possibilities levels
        ''' </summary>
        ''' <param name="CategoryPath">The category to split</param>
        ''' <returns>A sorted list of category paths for all possibilities levels</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CategoryPathElements(ByVal CategoryPath As String) As SortedList
            CategoryPath = ValidNavigationPath(CategoryPath)
            If CategoryPath = "\" Then
                'no elements in path
                Return New SortedList
            End If
            Dim Categories As New Collections.SortedList
            Dim CurCategoryPath As String = Nothing
            Dim CurCategory As String
            For Each CurCategory In CategoryPath.Split("\"c)
                CurCategory = Trim(CurCategory)
                If CurCategory <> "" Then
                    If CurCategoryPath = "" Then
                        CurCategoryPath = CurCategory
                    Else
                        CurCategoryPath &= "\" & CurCategory
                    End If
                    Categories.Add(CurCategoryPath, CurCategory)
                End If
            Next
            Return Categories
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieve all rows of the data table which are direct sub elements of a specified base level
        ''' </summary>
        ''' <param name="NavData">The data table containing all navigation data</param>
        ''' <param name="CurrentBaseLevel">The base level where to search for child elements</param>
        ''' <returns>All child rows which match the specified base level</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SubCategories(ByVal NavData As DataTable, ByVal CurrentBaseLevel As String) As DataTable
            CurrentBaseLevel = ValidNavigationPath(CurrentBaseLevel)
            If CurrentBaseLevel = "\" Then
                CurrentBaseLevel = ""
            End If
            Dim MyTable As DataTable = NavData
            Dim Result As DataTable = CreateEmptyDataTable()
            If MyTable Is Nothing Then
                Return Result
            End If

            Dim MyRows As DataRow() = MyTable.Select("", "Sort ASC, Title ASC")
            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim CurNewsCategories As String = CType(MyRows(MyCounter)("Title"), System.String)
                Dim CurCategory As String = CurNewsCategories
                'For Each CurCategory In CurNewsCategories.Split(";")
                CurCategory = ValidNavigationPath(CurCategory)
                Dim EndPos As Integer = InStr(Len(CurrentBaseLevel & "\") + 1, CurCategory, "\")
                Dim NextCategoryLevelName As String 'next sub element without base category
                Dim NextCategoryLevelTitle As String 'complete nav title path
                If EndPos = 0 Then
                    NextCategoryLevelName = CurCategory
                Else
                    NextCategoryLevelName = Mid(CurCategory, 1, EndPos - 1)
                End If
                If NextCategoryLevelName = "" Then
                    NextCategoryLevelName = "\"
                End If
                If InStr(NextCategoryLevelName, CurrentBaseLevel) <> 1 Then
                    NextCategoryLevelTitle = ""
                Else
                    If CurrentBaseLevel = "" Then
                        NextCategoryLevelTitle = Mid(NextCategoryLevelName, 1)
                    Else
                        NextCategoryLevelTitle = Mid(NextCategoryLevelName, Len(CurrentBaseLevel & "\") + 1)
                    End If
                End If
                If NextCategoryLevelName <> "\" AndAlso NextCategoryLevelTitle <> "" AndAlso CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Title"), CType(Nothing, String)) = NextCategoryLevelName Then
                    'Copy data row
                    Dim MyNewRow As DataRow
                    MyNewRow = Result.NewRow
                    For Each MyCol As DataColumn In Result.Columns
                        If Not MyTable.Columns(MyCol.ColumnName) Is Nothing Then
                            'only if the source column exists
                            MyNewRow(MyCol.ColumnName) = MyRows(MyCounter)(MyCol.ColumnName)
                        End If
                    Next
                    Result.Rows.Add(MyNewRow)
                End If
            Next
            'Next
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Looks up for a value in a defined column of a data table
        ''' </summary>
        ''' <param name="DataTable">The data table where to search in</param>
        ''' <param name="ColumnName">The column of the data table where to search in</param>
        ''' <param name="Value">The value to search for</param>
        ''' <returns>True if the value already exists in the list of active rows</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValueAlreadyExistsInDataTable(ByVal DataTable As DataTable, ByVal ColumnName As String, ByVal Value As Long) As Boolean
            Dim MyRows As DataRow() = DataTable.Select("[" & ColumnName.Replace("]", "[]") & "]=" & Value)
            If MyRows.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Looks up for a value in a defined column of a data table
        ''' </summary>
        ''' <param name="DataTable">The data table where to search in</param>
        ''' <param name="ColumnName">The column of the data table where to search in</param>
        ''' <param name="Value">The value to search for</param>
        ''' <returns>True if the value already exists in the list of active rows</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValueAlreadyExistsInDataTable(ByVal DataTable As DataTable, ByVal ColumnName As String, ByVal Value As String) As Boolean
            Dim MyRows As DataRow() = DataTable.Select("[" & ColumnName.Replace("]", "[]") & "]='" & Value.Replace("'", "''") & "'")
            If MyRows.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Looks up for a value in a defined column of a data table
        ''' </summary>
        ''' <param name="DataTable">The data table where to search in</param>
        ''' <param name="ColumnName">The column of the data table where to search in</param>
        ''' <param name="Value">The value to search for</param>
        ''' <returns>True if the value already exists in the list of active rows</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValueAlreadyExistsInDataTable(ByVal DataTable As DataTable, ByVal ColumnName As String, ByVal Value As DBNull) As Boolean
            Dim MyRows As DataRow() = DataTable.Select("[" & ColumnName.Replace("]", "[]") & "]=NULL")
            If MyRows.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the upper level of a category path
        ''' </summary>
        ''' <param name="category">A category path</param>
        ''' <returns>The parent category or a backslash for the root value</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ParentCategory(ByVal category As String) As String

            If category = "" OrElse category = "\" Then
                'No parent path available, we're already there!
                Return Nothing
            End If
            category = ValidNavigationPath(category)

            Dim EndPos As Integer = InStrRev(category, "\")
            Dim NextCategoryLevelName As String
            If EndPos = 0 Then
                NextCategoryLevelName = "\"
            Else
                NextCategoryLevelName = Mid(category, 1, EndPos - 1)
            End If
            Return NextCategoryLevelName

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Validates a correct construction of a category title
        ''' </summary>
        ''' <param name="path">The path to be validated</param>
        ''' <returns>The path without starting or trailing back slashes or one backslash in case of the root level</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ValidNavigationPath(ByVal path As String) As String

            Dim Result As String = Trim(path)

            If Result = "" Then
                'No parent path available, we're already there!
                Result = "\"
            End If

            If Left(Result, 1) = "\" AndAlso Result.Length > 1 Then
                'truncate unneeded, beginning slash
                Result = Mid(Result, 2)
            End If
            If Right(Result, 1) = "\" AndAlso Result.Length > 1 Then
                'truncate unneeded, trailing slash
                Result = Mid(Result, 1, Result.Length - 1)
            End If

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves the sub categories
        ''' </summary>
        ''' <param name="basePath">A string containing the path which should be removed</param>
        ''' <param name="completePath">The complete path of a category</param>
        ''' <returns>The relative sub path between the basePath and the completePath. Empty if the basePath is not part of the completePath.</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function RelativeCategory(ByVal basePath As String, ByVal completePath As String) As String
            If Mid(basePath, 1, 1) = "\" Then
                'remove starting back slahes
                basePath = Mid(basePath, 2)
            End If
            If Mid(completePath, 1, 1) = "\" Then
                'remove starting back slahes
                completePath = Mid(completePath, 2)
            End If
            If basePath <> "" Then
                'ensure a trailing back slash
                If Right(basePath, 1) <> "\" Then
                    basePath &= "\"
                End If
            Else
                'if nothing let it be one back slash
                basePath = "\"
            End If
            If completePath = "" Then
                'if nothing let it be one back slash
                completePath = "\"
            Else
                'ensure a trailing back slash
                If Right(completePath, 1) <> "\" Then
                    completePath &= "\"
                End If
            End If
            'retrieve the sub path
            Dim Result As String
            If completePath = basePath Then
                Result = "\" 'gets cutted later
            ElseIf basePath = "\" Then
                'starting from root, the result is always the complete path
                Result = completePath
            ElseIf InStr(completePath, basePath) <> 1 Then
                Result = "\" 'don't return completePath here even if this might make sense. But we're not accepting the result to say go to root level first. We must return a relative value.
            Else
                Result = Mid(completePath, basePath.Length + 1)
            End If
            'return without the trailing slash
            Return Mid(Result, 1, Result.Length - 1)
        End Function

    End Module

End Namespace