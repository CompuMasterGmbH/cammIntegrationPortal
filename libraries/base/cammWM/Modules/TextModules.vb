'Copyright 2006,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Modules.Text

#Region " Public Class TextModules "
    ''' <summary>Module for (legal) text management</summary>
    ''' <remarks>
    '''     Note: WebManager web.config setting for cammWM.TextModule
    '''     1. TextModulesServerGroupIDDefault 
    '''         -   This value should be greater than or equal to 1
    '''         -   The value 0 is equivalent to Nothing
    '''         -   If this value is set, then complete cammWM.TextModule library will work with this constant
    '''             ServerGroupID. Otherwise it will use default ServerGroupID i.e. from cammWM 
    '''             (=> "_webManager.CurrentServerInfo.ParentServerGroupID") 
    '''     2. TextModulesServerGroupIDForced
    '''         -   This value should be greater than or equal to 1
    '''         -   The value 0 is equivalent to Nothing
    '''         -   If this value is set, then complete cammWM.TextModule library will work with this constant
    '''             ServerGroupID. Otherwise it will use from method parameter value
    ''' </remarks>
    Public Class TextModules

        Private _Data As Administration.Data
        Private ReadOnly Property Data() As Administration.Data
            Get
                If _Data Is Nothing Then
                    _Data = New Administration.Data(Me._webManager)
                End If
                Return _Data
            End Get
        End Property

        Private _Business As Administration.Business
        Private ReadOnly Property Business() As Administration.Business
            Get
                If _Business Is Nothing Then
                    _Business = New Administration.Business(Me._webManager)
                End If
                Return _Business
            End Get
        End Property

        Private _webManager As CompuMaster.camm.WebManager.WMSystem
        Public Sub New(ByVal webManager As CompuMaster.camm.WebManager.WMSystem)
            _webManager = webManager
        End Sub

        <Serializable()> Public Structure ModuleItem
            ''' <summary>
            '''     The official key
            ''' </summary>
            Public Key As String
            ''' <summary>
            '''     A plain text string or HTML code (defined by the variable type) with the complete value of the text
            ''' </summary>
            Public Value As String
            ''' <summary>
            '''     Type of Text Module
            ''' </summary>
            Public TypeID As TextModuleType
            ''' <summary>
            '''     Defines released state of text module
            ''' </summary>
            Public Released As Boolean
            ''' <summary>
            '''     Version of text module
            ''' </summary>
            Public Version As Integer
            ''' <summary>
            '''     Published date of text module
            ''' </summary>
            ''' <remarks>
            '''     PublishedOn = nothing means text module is unreleased
            ''' </remarks>
            Public PublishedOn As DateTime
        End Structure
        ''' <summary>
        '''     Defines type of text variable.
        ''' </summary>
        ''' <remarks>
        '''     Variable types are, _
        '''     1. PlainTextString
        '''     2. PlainTextBlock, can contain sub variables/blocks
        '''     3. HtmlTextBlock, can contain sub variables/blocks
        '''     4. HtmlTemplate, i.e. collection of one or more or all variable types from 1, 2, 3., can contain sub variables/blocks
        '''     Please pay attention: an html variable type can include plain text types, but inclusion of html types in plain text types will throw exceptions!
        ''' </remarks>
        Public Enum TextModuleType As Byte
            ''' <summary>
            '''     Plain string
            ''' </summary>
            PlainTextString = 1
            ''' <summary>
            '''     Plain text block, can contain sub variables/blocks
            ''' </summary>
            PlainTextBlock = 21
            ''' <summary>
            '''     Html text block, can contain sub variables/blocks
            ''' </summary>
            HtmlTextBlock = 22
            ''' <summary>
            '''     Template, i.e. collection of one or more or all variable types from 1, 2, 3., can contain sub variables/blocks
            ''' </summary>
            HtmlTemplate = 3
        End Enum
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        ''' <remarks>
        '''     By default, the requested websitAreaID is empty.
        ''' </remarks>
        Public Function Load(ByVal key As String) As String
            Return Load(key, New String() {""})
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaIDs">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        Public Function Load(ByVal key As String, ByVal websiteAreaIDs() As String) As String
            Return Load(key, websiteAreaIDs, _webManager.UI.MarketID)
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaIDs">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        Public Function Load(ByVal key As String, ByVal websiteAreaIDs() As String, ByVal marketID As Integer) As String
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDDefault = Nothing Then
                _serverGroupID = Me._webManager.CurrentServerInfo.ParentServerGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDDefault
            End If
            Return Load(key, websiteAreaIDs, marketID, _serverGroupID)
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaIDs">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function Load(ByVal key As String, ByVal websiteAreaIDs() As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As String
            Dim marketIDs As Integer() = New Integer() {marketID, _webManager.Internationalization.GetAlternativelySupportedLanguageID(marketID), 0}
            Return Me.Load(key, websiteAreaIDs, marketIDs, serverGroupID)
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaIDs">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketIDs">An array of Integer with unique marketIDs - the order defines the priority</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function Load(ByVal key As String, ByVal websiteAreaIDs() As String, ByVal marketIDs() As Integer, ByVal serverGroupID As Integer) As String
            Return Me.Load(key, websiteAreaIDs, marketIDs, serverGroupID, True, True, Nothing)
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaIDs">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketIDs">The list of one or more markets/languages which are allowed to be returned - the order defines the priority</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <param name="version">The requested version of the text module or zero (0) for the currently released version</param>
        ''' <param name="replacePlaceholders">replace place holders with their value or not</param>
        ''' <param name="useCachedData">use cache data or not</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        ''' <remarks>
        '''     Placeholders = e.g. &lt;%CompanyName%&gt;
        ''' </remarks>
        Private Function Load(ByVal key As String, ByVal websiteAreaIDs() As String, ByVal marketIDs As Integer(), ByVal serverGroupID As Integer, ByVal replacePlaceholders As Boolean, ByVal useCachedData As Boolean, ByVal version As Integer) As String
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced = Nothing Then
                _serverGroupID = serverGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced
            End If

            Dim result As String

            'Drop any duplicates, first
            Dim marketList As New ArrayList
            For MyCounter As Integer = 0 To marketIDs.Length - 1
                'key already exists - drop the duplicate
                If Not marketList.Contains(marketIDs(MyCounter)) Then
                    'key is fine, use it
                    marketList.Add(marketIDs(MyCounter))
                End If
            Next
            Dim markets As Integer() = CType(marketList.ToArray(GetType(Integer)), Integer())

            Dim item As ModuleItem = Nothing
            'first look for availability by markets and then websiteAreaID
            For Each marketID As Integer In markets
                For Each websiteAreaID As String In websiteAreaIDs
                    item = Me.LoadModuleItem(key, websiteAreaID, marketID, _serverGroupID, useCachedData, version)
                    If item.Key <> Nothing Then
                        Exit For
                    End If
                Next
                If item.Key <> Nothing Then
                    Exit For
                End If
            Next
            If item.Value <> Nothing Then
                Dim sb As New System.Text.StringBuilder(item.Value)

                If replacePlaceholders Then
                    Dim placeHolders() As String = Administration.Business.AllPlaceHolders(sb.ToString)
                    If Not placeHolders Is Nothing Then
                        If placeHolders.Length > 0 Then
                            For Each placeHolder As String In placeHolders
                                Dim lt As String = "&lt;%"
                                Dim gt As String = "%&gt;"
                                Dim value As String = Me.Load(placeHolder, websiteAreaIDs, marketIDs, _serverGroupID, True, True, Nothing)
                                sb.Replace(lt & placeHolder & gt, value)
                            Next
                        End If
                    End If
                End If

                If Business.IsHtmlType(item.TypeID) Then
                    result = sb.ToString
                Else
                    'result = Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(sb.ToString))
                    result = sb.ToString
                    If result.IndexOf("&lt;") > -1 Then
                        result = result.Replace("&lt;", "<")
                        result = result.Replace("&gt;", ">")
                    End If
                    If result.IndexOf(vbNewLine) > -1 Then
                        result = result.Replace(vbNewLine, "<br>").Replace(vbNewLine, "<br>")
                    End If

                End If
            Else
                result = item.Value
            End If

            Return result
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaIDs">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketIDs">An array of Integer with unique marketIDs - the order defines the priority</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <param name="version">Version of TextModule</param>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function Load(ByVal key As String, ByVal websiteAreaIDs() As String, ByVal marketIDs() As Integer, ByVal serverGroupID As Integer, ByVal version As Integer) As String
            Return Load(key, websiteAreaIDs, marketIDs, serverGroupID, True, False, version)
        End Function
        ''' <summary>
        '''     Load module item
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <param name="useCachedData">If True loads data from cached table else from non cached table</param>
        ''' <param name="version">Version of text module</param>
        Private Function LoadModuleItem(ByVal key As String, ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal useCachedData As Boolean, ByVal version As Integer) As ModuleItem
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced = Nothing Then
                _serverGroupID = serverGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced
            End If

            Dim result As ModuleItem = Nothing
            If version = Nothing Then
                Dim items() As ModuleItem
                items = Me.LoadModuleItems(websiteAreaID, marketID, _serverGroupID, useCachedData, key)
                If Not items Is Nothing Then
                    If items.Length > 0 Then
                        For Each item As ModuleItem In items
                            If item.Key = key Then
                                result = item
                            End If
                        Next
                    End If
                End If
            Else
                Dim textModules As DataTable
                textModules = Data.Load(websiteAreaID, marketID, _serverGroupID, key, version)
                If textModules.Rows.Count = 1 Then
                    result = New ModuleItem
                    result.Key = CStr(textModules.Rows(0)("Key"))
                    result.Value = CStr(textModules.Rows(0)("Value"))
                    result.Released = CBool(textModules.Rows(0)("Released"))
                    result.TypeID = CType(textModules.Rows(0)("TypeID"), TextModuleType)
                    If Not IsDBNull(textModules.Rows(0)("PublishedOn")) Then
                        result.PublishedOn = CDate(textModules.Rows(0)("PublishedOn"))
                    Else
                        result.PublishedOn = Nothing
                    End If
                End If
            End If
            Return result
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <param name="useCachedData">If True loads data from cached table else from non cached table</param>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <returns>An array of text module items</returns>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function LoadModuleItems(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal useCachedData As Boolean, ByVal key As String) As ModuleItem()
            Return Me.LoadModuleItems(websiteAreaID, marketID, serverGroupID, useCachedData, key, Nothing)
        End Function
        ''' <summary>
        '''     Load a array of cached text module items
        ''' </summary>
        ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <param name="useCachedData">If True loads data from cached table else from non cached table</param>
        ''' <param name="typeID">Type of TextModule</param>
        ''' <returns>An array of text module items</returns>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function LoadModuleItems(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal useCachedData As Boolean, ByVal typeID As TextModuleType) As ModuleItem()
            Return Me.LoadModuleItems(websiteAreaID, marketID, serverGroupID, useCachedData, Nothing, typeID)
        End Function
        ''' <summary>
        '''     Load a array of cached text module items
        ''' </summary>
        ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <returns>An array of text module items</returns>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function LoadModuleItems(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal useCachedData As Boolean) As ModuleItem()
            Return Me.LoadModuleItems(websiteAreaID, marketID, serverGroupID, useCachedData, Nothing, Nothing)
        End Function
        ''' <summary>
        '''     Load a array of cached text module items
        ''' </summary>
        ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <param name="useCachedData">If True loads data from cached table else from non cached table</param>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="typeID">Type of TextModule</param>
        ''' <returns>An array of text module items</returns>
        Private Function LoadModuleItems(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal useCachedData As Boolean, ByVal key As String, ByVal typeID As TextModuleType) As ModuleItem()
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced = Nothing Then
                _serverGroupID = serverGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced
            End If

            Dim textModules As DataTable = Nothing
            If key = Nothing AndAlso typeID = Nothing Then
                If useCachedData Then
                    textModules = Business.TextModulesCached(websiteAreaID, marketID, _serverGroupID)
                Else
                    textModules = Data.Load(websiteAreaID, marketID, _serverGroupID)
                End If
            ElseIf key <> Nothing AndAlso typeID = Nothing Then
                If useCachedData Then
                    textModules = Business.TextModulesCached(websiteAreaID, marketID, _serverGroupID, key)
                Else
                    textModules = Data.Load(websiteAreaID, marketID, _serverGroupID, key)
                End If
            ElseIf key = Nothing AndAlso typeID <> Nothing Then
                If useCachedData Then
                    textModules = Business.TextModulesCached(websiteAreaID, marketID, _serverGroupID, typeID)
                Else
                    textModules = Data.Load(websiteAreaID, marketID, _serverGroupID, typeID)
                End If
            ElseIf key <> Nothing AndAlso typeID <> Nothing Then
                If useCachedData Then
                    textModules = Business.TextModulesCached(websiteAreaID, marketID, _serverGroupID, key, typeID)
                Else
                    textModules = Data.Load(websiteAreaID, marketID, _serverGroupID, key, typeID)
                End If
            End If

            Return Me.ConvertResultTableToModuleItemsArray(textModules)
        End Function
        ''' <summary>
        '''     Converts textmodules datacontent into an array of type ModuleItem
        ''' </summary>
        ''' <param name="textModules">DataTable containing textmodules</param>
        Private Function ConvertResultTableToModuleItemsArray(ByVal textModules As DataTable) As ModuleItem()
            If textModules Is Nothing OrElse textModules.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim result(textModules.Rows.Count - 1) As ModuleItem
            For counter As Integer = 0 To textModules.Rows.Count - 1
                Dim item As New ModuleItem
                item.Key = CStr(textModules.Rows(counter)("Key"))
                item.Value = CStr(textModules.Rows(counter)("Value"))
                item.TypeID = CType(textModules.Rows(counter)("TypeID"), Text.TextModules.TextModuleType)
                If textModules.Columns.Contains("Released") Then
                    item.Released = CBool(textModules.Rows(counter)("Released"))
                    item.Version = CInt(textModules.Rows(counter)("Version"))
                    If Not IsDBNull(textModules.Rows(counter)("PublishedOn")) Then
                        item.PublishedOn = CDate(textModules.Rows(counter)("PublishedOn"))
                    Else
                        item.PublishedOn = Nothing
                    End If
                Else
                    item.Released = True
                    item.Version = Nothing
                    item.PublishedOn = Nothing
                End If
                result(counter) = item
            Next

            Return result
        End Function
        ''' <summary>
        '''     Save value of TextModule defined by key
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="value">Value of TextModule to be saved</param>
        ''' <param name="release">Defines Save and Release the TextModule or not</param>
        Public Sub Save(ByVal key As String, ByVal value As String, ByVal release As Boolean)
            Me.Save(key, value, release, "")
        End Sub
        ''' <summary>
        '''     Save value of TextModule defined by key
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the TextModule</param>
        ''' <param name="value">Value of TextModule to be saved</param>
        ''' <param name="release">Defines Save and Release the TextModule or not</param>
        ''' <param name="websiteAreaID">Unique WebsiteAreaID of TextModule</param>
        Public Sub Save(ByVal key As String, ByVal value As String, ByVal release As Boolean, ByVal websiteAreaID As String)
            Me.Save(key, value, release, websiteAreaID, Me._webManager.UI.MarketID)
        End Sub
        ''' <summary>
        '''     Save value of TextModule defined by key
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the TextModule</param>
        ''' <param name="value">Value of TextModule to be saved</param>
        ''' <param name="release">Defines Save and Release the TextModule or not</param>
        ''' <param name="websiteAreaID">Unique WebsiteAreaID of TextModule</param>
        ''' <param name="marketID">MarketID of TextModule</param>
        Public Sub Save(ByVal key As String, ByVal value As String, ByVal release As Boolean, ByVal websiteAreaID As String, ByVal marketID As Integer)
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDDefault = Nothing Then
                _serverGroupID = Me._webManager.CurrentServerInfo.ParentServerGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDDefault
            End If
            Me.Save(key, value, release, websiteAreaID, marketID, _serverGroupID)
        End Sub
        ''' <summary>
        '''     Save value of TextModule defined by key
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the TextModule</param>
        ''' <param name="value">Value of TextModule to be saved</param>
        ''' <param name="release">Defines Save and Release the TextModule or not</param>
        ''' <param name="websiteAreaID">Unique WebsiteAreaID of TextModule</param>
        ''' <param name="marketID">MarketID of TextModule</param>
        ''' <param name="serverGroupID">ServerGroupID of TextModule</param>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub Save(ByVal key As String, ByVal value As String, ByVal release As Boolean, ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer)
            Me.Save(key, value, release, websiteAreaID, marketID, Me._webManager.CurrentServerInfo.ParentServerGroupID, Nothing)
        End Sub
        ''' <summary>
        '''     Save value of TextModule defined by key
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the TextModule</param>
        ''' <param name="value">Value of TextModule to be saved</param>
        ''' <param name="release">Defines Save and Release the TextModule or not</param>
        ''' <param name="websiteAreaID">Unique WebsiteAreaID of TextModule</param>
        ''' <param name="marketID">MarketID of TextModule</param>
        ''' <param name="serverGroupID">ServerGroupID of TextModule</param>
        ''' <param name="typeID">Type of TextModule</param>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Sub Save(ByVal key As String, ByVal value As String, ByVal release As Boolean, ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal typeID As TextModuleType)
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced = Nothing Then
                _serverGroupID = serverGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced
            End If
            If typeID = TextModules.TextModuleType.PlainTextBlock Or typeID = TextModules.TextModuleType.PlainTextString Then
                Dim releasedItems() As Text.TextModules.ModuleItem
                releasedItems = Me.LoadModuleItems(websiteAreaID, marketID, _serverGroupID, True)
                If Administration.Business.IsPlainTextblockContainsHtmlTextModule(value, releasedItems) Then
                    Throw New Exception("Html type TextModule is found in plain text TextModule!")
                End If
            End If
            Data.Save(key, value, release, websiteAreaID, marketID, _serverGroupID, typeID)
        End Sub
        ''' <summary>
        '''     Array of all keys
        ''' </summary>
        ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function Keys(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As String()
            Dim _serverGroupID As System.Int32
            If CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced = Nothing Then
                _serverGroupID = serverGroupID
            Else
                _serverGroupID = CompuMaster.camm.WebManager.Configuration.TextModulesServerGroupIDForced
            End If
            Return Data.Keys(websiteAreaID, marketID, _serverGroupID)
        End Function
        ''' <summary>
        '''     Check validity of key
        ''' </summary>
        ''' <param name="key">Key to be checked</param>
        ''' <returns>True if the key name doesn't contain forbidden characters</returns>
        <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public Function IsValid(ByVal key As String) As Boolean
            Return Administration.Business.IsValid(key)
        End Function

    End Class
#End Region

    Namespace Administration

#Region " Friend Class Data "
        Friend Class Data

            Public Sub New(ByVal webManager As IWebManager)
                _webManager = webManager
            End Sub

            Private _webManager As IWebManager
            ''' <summary>
            '''     Connection string to database
            ''' </summary>
            ''' <value></value>
            Private ReadOnly Property ConnectionString() As String
                Get
                    Return _webManager.ConnectionString
                End Get
            End Property
            ''' <summary>
            '''     Saves the TextModule to database
            ''' </summary>
            ''' <param name="key">The name of the key which uniquely identifies the required value</param>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="value">value of Text Module</param>
            ''' <param name="typeID">TypeID for type of Text Module</param>
            ''' <param name="release">Defines Save and Release the TextModule or not</param>
            Public Sub Save(ByVal key As String, ByVal value As String, ByVal release As Boolean, ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal typeID As Text.TextModules.TextModuleType)

                'Argument validation
                If key Is Nothing Then
                    Throw New ArgumentException("Must be a value", "key")
                End If
                If key.Length > 100 Then
                    Throw New ArgumentException("Key can't contain more than 100 characters", "key")
                End If
                If Not Business.IsValid(key) Then
                    Throw New ArgumentException("Contains not allowed special characters.", "key")
                End If
                If Not websiteAreaID Is Nothing AndAlso websiteAreaID.Length > 50 Then
                    Throw New ArgumentException("WebsiteAreaID can't contain more than 50 characters", "websiteAreaID")
                End If

                'Preprare update/insert command
                Dim query As New System.Text.StringBuilder
                If release Then
                    query.Append("UPDATE [dbo].[TextModules] " & vbNewLine)
                    query.Append("SET [Released] = 0 " & vbNewLine)
                    query.Append("where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("and [Key] = @Key " & vbNewLine)
                    query.Append("  " & vbNewLine)
                    query.Append("DECLARE @now datetime " & vbNewLine)
                    query.Append("set @now = getdate() " & vbNewLine)
                    query.Append(" " & vbNewLine)
                    query.Append("UPDATE [dbo].[TextModules]  " & vbNewLine)
                    query.Append(" ")
                    If typeID = Nothing Then
                        query.Append("SET [Released] = 1, [Value] = @Value, [PublishedOn] = @now ")
                    Else
                        query.Append("SET [Released] = 1, [Value] = @Value, [TypeID] = @TypeID, [PublishedOn] = @now ")
                    End If
                    query.Append("where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID  " & vbNewLine)
                    query.Append("and [Key] = @Key and [PublishedOn] is null " & vbNewLine)
                    query.Append(" " & vbNewLine)
                    query.Append("if exists(select [Version], [TypeID] FROM [dbo].[TextModules] " & vbNewLine)
                    query.Append("where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("and [Key] = @Key and [PublishedOn] = @now ) " & vbNewLine)
                    query.Append("BEGIN " & vbNewLine)
                    query.Append("   Declare @Version int " & vbNewLine)
                    query.Append("   Declare @Type int " & vbNewLine)
                    query.Append(" " & vbNewLine)
                    query.Append("   SELECT @Version = [Version], @Type = [TypeID]  " & vbNewLine)
                    query.Append("   FROM [dbo].[TextModules] " & vbNewLine)
                    query.Append("   where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("   and [Key] = @Key and [PublishedOn] = @now " & vbNewLine)
                    query.Append(" " & vbNewLine)
                    query.Append("   if not exists(select * FROM [dbo].[TextModules] " & vbNewLine)
                    query.Append("   where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("   and [Key] = @Key and [PublishedOn] is null) " & vbNewLine)
                    query.Append("   BEGIN " & vbNewLine)
                    query.Append("       INSERT INTO [dbo].[TextModules]([MarketID], [WebsiteAreaID], [ServerGroupID], [Key], [Value], [Version], [TypeID], [Released], [PublishedOn], [Title], [IsDeleted])  " & vbNewLine)
                    query.Append("       VALUES( @MarketID, @WebsiteAreaID, @ServerGroupID, @Key, @Value, (@Version +1), @Type, 0, NULL, NULL, 0) " & vbNewLine)
                    query.Append("   End " & vbNewLine)
                    query.Append("End " & vbNewLine)
                    query.Append(" ")
                    'do not insert if typeID is nothing
                    If typeID <> Nothing Then
                        query.Append("ELSE " & vbNewLine)
                        query.Append("BEGIN " & vbNewLine)
                        query.Append("   INSERT INTO [dbo].[TextModules]([MarketID], [WebsiteAreaID], [ServerGroupID], [Key], [Value], [Version], [TypeID], [Released], [PublishedOn], [Title], [IsDeleted]) " & vbNewLine)
                        query.Append("   VALUES( @MarketID, @WebsiteAreaID, @ServerGroupID, @Key, @Value, 1, @TypeID, 1, GETDATE(), NULL, 0) " & vbNewLine)
                        query.Append("   INSERT INTO [dbo].[TextModules]([MarketID], [WebsiteAreaID], [ServerGroupID], [Key], [Value], [Version], [TypeID], [Released], [PublishedOn], [Title], [IsDeleted]) " & vbNewLine)
                        query.Append("   VALUES( @MarketID, @WebsiteAreaID, @ServerGroupID, @Key, @Value, 2, @TypeID, 0, NULL, NULL, 0) " & vbNewLine)
                        query.Append("End " & vbNewLine)
                        query.Append(" ")
                    End If

                Else
                    query.Append(" " & vbNewLine)
                    query.Append("if exists(SELECT * FROM [dbo].[TextModules] " & vbNewLine)
                    query.Append("where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("and [Key] = @Key and [PublishedOn] is null) " & vbNewLine)
                    query.Append("BEGIN " & vbNewLine)
                    query.Append("   UPDATE [dbo].[TextModules] " & vbNewLine)
                    query.Append(" ")
                    If typeID = Nothing Then
                        query.Append("SET [Value] = @Value ")
                    Else
                        query.Append("SET [Value] = @Value, [TypeID] = @TypeID ")
                    End If
                    query.Append("   where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("   and [Key] = @Key and [PublishedOn] is null " & vbNewLine)
                    query.Append("END " & vbNewLine)
                    query.Append(" ")
                    'do not insert if typeID is nothing
                    If typeID <> Nothing Then
                        query.Append("ELSE " & vbNewLine)
                        query.Append("BEGIN " & vbNewLine)
                        query.Append("   INSERT INTO [dbo].[TextModules]([MarketID], [WebsiteAreaID], [ServerGroupID], [Key], [Value], [Version], [TypeID], [Released], [PublishedOn], [Title], [IsDeleted]) " & vbNewLine)
                        query.Append("   VALUES( @MarketID, @WebsiteAreaID, @ServerGroupID, @Key, @Value, 1, @TypeID, 0, NULL, NULL, 0) " & vbNewLine)
                        query.Append("End " & vbNewLine)
                        query.Append(" ")
                    End If
                End If
                query.Append(" DELETE FROM [dbo].[TextModulesCache] " & vbNewLine)
                query.Append("where [MarketID]= @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                query.Append(" ")

                'Execute the update/insert command
                Dim cmd As New System.Data.SqlClient.SqlCommand(query.ToString, New System.Data.SqlClient.SqlConnection(ConnectionString))

                With cmd
                    .Parameters.Add(New SqlClient.SqlParameter("@MarketID", SqlDbType.Int)).Value = marketID
                    If websiteAreaID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = ""
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = websiteAreaID
                    End If
                    If serverGroupID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = 0
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = serverGroupID
                    End If
                    .Parameters.Add(New SqlClient.SqlParameter("@Key", SqlDbType.NVarChar)).Value = key
                    .Parameters.Add(New SqlClient.SqlParameter("@Value", SqlDbType.NText)).Value = value
                    If typeID <> Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)).Value = typeID
                    End If
                End With
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)

                'remove http cache
                If Not System.Web.HttpContext.Current Is Nothing Then
                    System.Web.HttpContext.Current.Cache.Remove("WebManager.TextModules." & marketID & "_" & websiteAreaID & "_" & serverGroupID)
                End If

            End Sub
            ''' <summary>
            '''     Load Text Module for Key
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <remarks>
            '''     Loads first released, then unreleased text modules
            '''     
            '''     The returned DataTable columns are
            '''     Key     - String
            '''     Value   - String
            '''     Version - Integer 
            '''     TypeID  - Integer
            '''     Released- Boolean
            '''     Title   - String
            '''     
            ''' </remarks>
            Public Function Load(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As DataTable
                Return Load(websiteAreaID, marketID, serverGroupID, Nothing, Nothing)
            End Function
            ''' <summary>
            '''     Loads all released as well unreleased Text Module.
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="typeID">TypeID for type of Text Module</param>
            ''' <returns>Retuns DataTable</returns>
            ''' <remarks>
            '''     Loads first released , then unreleased text modules for asked key with defined type
            '''     
            '''     If the type of required Text Module is HtmlTextBox or PlainTextBox, _
            '''     then it loads Text Module for both types i.e. HtmlTextBox and PlainTextBox
            '''     
            '''     The returned DataTable columns are
            '''     Key     - String
            '''     Value   - String
            '''     Version - Integer 
            '''     TypeID  - Integer
            '''     Released- Boolean
            '''     Title   - String
            '''     
            ''' </remarks>
            Public Function Load(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal typeID As Text.TextModules.TextModuleType) As DataTable
                Return Load(websiteAreaID, marketID, serverGroupID, Nothing, typeID)
            End Function
            ''' <summary>
            '''     Load Text Module for Key
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="key">Key of required Text Module</param>
            ''' <returns>DataTable</returns>
            ''' <remarks>
            '''     The returned DataTable columns are
            '''     Key         - String
            '''     Value       - String
            '''     Version     - Integer 
            '''     Released    - Boolean
            '''     TypeID      - Integer
            '''     PublishedOn - DateTime
            '''     
            ''' </remarks>
            Public Function Load(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal key As String) As DataTable
                Return Load(websiteAreaID, marketID, serverGroupID, key, Nothing)
            End Function
            ''' <summary>
            '''     Load Text Module for Key
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="key">Key of required Text Module</param>
            ''' <param name="typeID">type of text module</param>
            ''' <remarks>
            '''     Loads all version of text module for asked key
            ''' </remarks>
            Public Function Load(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal key As String, ByVal typeID As Text.TextModules.TextModuleType) As DataTable
                'Argument validation
                If key <> Nothing AndAlso key.Length > 100 Then
                    Throw New ArgumentException("Key can't contain more than 100 characters", "key")
                End If
                If Not websiteAreaID Is Nothing AndAlso websiteAreaID.Length > 50 Then
                    Throw New ArgumentException("WebsiteAreaID can't contain more than 50 characters", "websiteAreaID")
                End If

                Dim query As System.Text.StringBuilder
                query = New System.Text.StringBuilder("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [Key], [Value], [Version], [TypeID], [Released], [PublishedOn] " & vbNewLine)
                query.Append("FROM [dbo].[TextModules] " & vbNewLine)
                query.Append("where ( ([MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                query.Append("and [Released] = 1 and [PublishedOn] is not null ) " & vbNewLine)
                query.Append("or " & vbNewLine)
                query.Append("( " & vbNewLine)
                query.Append("[MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                query.Append("and [Released] = 0 and [PublishedOn] is null " & vbNewLine)
                query.Append("and [Key] not in ( " & vbNewLine)
                query.Append("SELECT [Key] FROM [dbo].[TextModules]  " & vbNewLine)
                query.Append("where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                query.Append("and [Released] = 1 and [PublishedOn] is not null ) " & vbNewLine)
                query.Append(") ) " & vbNewLine)
                query.Append("")
                If key = Nothing AndAlso typeID = Nothing Then
                    query.Append("ORDER BY [Released] DESC, [Key]  ")
                ElseIf key <> Nothing AndAlso typeID = Nothing Then
                    'load all versions for a Key
                    query = Nothing
                    query = New System.Text.StringBuilder("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "SELECT [Key], [Value], [Version], [Released], [TypeID], [PublishedOn] " & vbNewLine)
                    query.Append("FROM [dbo].[TextModules] " & vbNewLine)
                    query.Append("where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine)
                    query.Append("and [Key] = @Key ORDER BY [Version]" & vbNewLine)
                    query.Append(" ")
                ElseIf key = Nothing AndAlso typeID <> Nothing Then
                    query.Append("and ( [TypeID] like (Left(cast(@TypeID as nvarchar(4)), 1) + '_') or [TypeID] = @TypeID) " & vbNewLine)
                    query.Append("ORDER BY [Released] DESC, [Key]  ")
                ElseIf key <> Nothing AndAlso typeID <> Nothing Then
                    query.Append("and ( [TypeID] like (Left(cast(@TypeID as nvarchar(4)), 1) + '_') or [TypeID] = @TypeID) " & vbNewLine)
                    query.Append("and [Key] = @Key " & vbNewLine)
                    query.Append("ORDER BY [Released] DESC, [Key]  ")
                End If

                Dim MyCmd As New SqlClient.SqlCommand(query.ToString, New SqlClient.SqlConnection(ConnectionString))
                With MyCmd
                    .Parameters.Add(New SqlClient.SqlParameter("@MarketID", SqlDbType.Int)).Value = marketID
                    If websiteAreaID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = ""
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = websiteAreaID
                    End If
                    If serverGroupID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = 0
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = serverGroupID
                    End If
                    If typeID <> Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@TypeID", SqlDbType.Int)).Value = typeID
                    End If
                    If key <> Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@Key", SqlDbType.NVarChar)).Value = key
                    End If
                End With

                Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "TextModuleData")
            End Function
            ''' <summary>
            '''     Load Text Module for Key
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="key">Key of required Text Module</param>
            ''' <param name="version">Version of text module</param>
            ''' <remarks>
            '''     Loads text module for key with defined value of version
            ''' </remarks>
            Public Function Load(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal key As String, ByVal version As Integer) As DataTable
                'Argument validation
                If key Is Nothing Then
                    Throw New ArgumentException("Must be a value", "key")
                End If
                If key <> Nothing AndAlso key.Length > 100 Then
                    Throw New ArgumentException("Key can't contain more than 100 characters", "key")
                End If
                If Not websiteAreaID Is Nothing AndAlso websiteAreaID.Length > 50 Then
                    Throw New ArgumentException("WebsiteAreaID can't contain more than 50 characters", "websiteAreaID")
                End If

                Dim query As String
                query = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                  "SELECT [Key], [Value], [Version], [Released], [TypeID], [PublishedOn] " & vbNewLine & _
                  "FROM [dbo].[TextModules] " & vbNewLine & _
                  "where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine & _
                  "and [Key] = @Key and [Version] = @Version " & vbNewLine & _
                  " "

                Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(ConnectionString))
                With MyCmd
                    .Parameters.Add(New SqlClient.SqlParameter("@MarketID", SqlDbType.Int)).Value = marketID
                    If websiteAreaID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = ""
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = websiteAreaID
                    End If
                    If serverGroupID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = 0
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = serverGroupID
                    End If
                    If key <> Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@Key", SqlDbType.NVarChar)).Value = key
                    End If
                    If version <> Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@Version", SqlDbType.Int)).Value = version
                    End If
                End With

                Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "TextModuleData")
            End Function
            ''' <summary>
            '''     String array of distinct key(Name)
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <returns>String array</returns>
            Public Function Keys(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As String()
                If websiteAreaID <> Nothing Then
                    If websiteAreaID.Length > 50 Then
                        Throw New Exception("Key can't contain more than 50 characters")
                    End If
                End If

                Dim query As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                   "SELECT [Key] FROM [dbo].[TextModules] " & vbNewLine & _
                   "where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine & _
                   "GROUP BY [Key] " & vbNewLine & _
                   "Order by [Key] " & vbNewLine & _
                   " "

                Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(ConnectionString))
                With MyCmd
                    .Parameters.Add(New SqlClient.SqlParameter("@MarketID", SqlDbType.Int)).Value = marketID
                    If websiteAreaID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = ""
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = websiteAreaID
                    End If
                    If serverGroupID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = 0
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = serverGroupID
                    End If
                End With

                Dim result() As String = Nothing
                Dim keysDt As DataTable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "DistinctKeys")
                If Not keysDt Is Nothing Then
                    If keysDt.Rows.Count > 0 Then
                        ReDim result(keysDt.Rows.Count - 1)
                        Dim counter As Integer = 0
                        For Each dRow As DataRow In keysDt.Rows
                            result(counter) = CStr(dRow("Key"))
                            counter += 1
                        Next
                    End If
                End If

                Return result
            End Function
            ''' <summary>
            '''     Loads Text Modules from Cached Table
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <remarks>
            '''     The returned DataTable columns are
            '''     Key         - String
            '''     Value       - String
            '''     TypeID      - Integer
            '''     
            ''' </remarks>
            Public Function LoadCached(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As DataTable
                If websiteAreaID <> Nothing Then
                    If websiteAreaID.Length > 50 Then
                        Throw New ArgumentException("WebsiteAreaID can't contain more than 50 characters")
                    End If
                End If

                Dim query As String
                query = "if (( SELECT count([Key])FROM [dbo].[TextModulesCache] " & vbNewLine & _
                  "where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID )  = 0 )" & vbNewLine & _
                  "BEGIN " & vbNewLine & _
                  "   INSERT INTO [dbo].[TextModulesCache]([MarketID], [WebsiteAreaID], [ServerGroupID], [Key], [Value], [Title], [TypeID]) " & vbNewLine & _
                  "   SELECT [MarketID], [WebsiteAreaID], [ServerGroupID], [Key], [Value], [Title], [TypeID] " & vbNewLine & _
                  "   FROM [dbo].[TextModules] " & vbNewLine & _
                  "   where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine & _
                  "   and [Released] = 1 and [PublishedOn] is not null " & vbNewLine & _
                  "END " & vbNewLine & _
                  " " & vbNewLine & _
                  "SELECT [Key], [Value], [TypeID] " & vbNewLine & _
                  "FROM [dbo].[TextModulesCache] " & vbNewLine & _
                  "where [MarketID] = @MarketID and [WebsiteAreaID] = @WebsiteAreaID and [ServerGroupID] = @ServerGroupID " & vbNewLine & _
                  "" & vbNewLine & _
                  " "

                Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(ConnectionString))
                With MyCmd
                    .Parameters.Add(New SqlClient.SqlParameter("@MarketID", SqlDbType.Int)).Value = marketID
                    If websiteAreaID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = ""
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@WebsiteAreaID", SqlDbType.NVarChar)).Value = websiteAreaID
                    End If
                    If serverGroupID = Nothing Then
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = 0
                    Else
                        .Parameters.Add(New SqlClient.SqlParameter("@ServerGroupID", SqlDbType.Int)).Value = serverGroupID
                    End If

                End With

                Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "TextModuleData")
            End Function

        End Class
#End Region

#Region " Friend Class Business "
        Friend Class Business

            Public Sub New(ByVal webManager As IWebManager)
                _webManager = webManager
            End Sub

            Private _webManager As IWebManager
            ''' <summary>
            '''     Connection string to database
            ''' </summary>
            ''' <value></value>
            Private ReadOnly Property ConnectionString() As String
                Get
                    Return _webManager.ConnectionString
                End Get
            End Property

            Private _Data As Administration.Data
            Private ReadOnly Property Data() As Administration.Data
                Get
                    If _Data Is Nothing Then
                        _Data = New Administration.Data(_webManager)
                    End If
                    Return _Data
                End Get
            End Property
            ''' <summary>
            '''     True if text module is of html type else False
            ''' </summary>
            ''' <param name="typeID">type of text module</param>
            Public Function IsHtmlType(ByVal typeID As TextModules.TextModuleType) As Boolean
                Select Case typeID
                    Case TextModules.TextModuleType.HtmlTemplate, TextModules.TextModuleType.HtmlTextBlock
                        Return True
                    Case TextModules.TextModuleType.PlainTextBlock, TextModules.TextModuleType.PlainTextString
                        Return False
                    Case Else
                        Throw New ArgumentOutOfRangeException("typeID")
                End Select
            End Function
            ''' <summary>
            '''     Replaces all place holder with their value recursively
            ''' </summary>
            ''' <param name="value">A string where some replacing shall happen inside</param>
            ''' <param name="releasedItems">Array of all released text modules</param>
            Public Function ReplacePlaceHolders(ByVal value As String, ByVal valueIsHtml As Boolean, ByVal releasedItems() As Text.TextModules.ModuleItem, ByVal throwExceptionWhenNoChildCanBeFound As Boolean) As String
                Dim result As New System.Text.StringBuilder

                Dim lt As String = "&lt;%"
                Dim gt As String = "%&gt;"

                Dim temp As String = value
                If temp <> Nothing Then
                    While temp.IndexOf(lt) > -1
                        result.Append(temp.Substring(0, temp.IndexOf(lt)))
                        temp = temp.Substring(temp.IndexOf(lt) + lt.Length)
                        Dim key As String = temp.Substring(0, temp.IndexOf(gt))
                        temp = temp.Substring(temp.IndexOf(gt) + gt.Length)

                        Dim item As Text.TextModules.ModuleItem = Nothing
                        For Each myItem As Text.TextModules.ModuleItem In releasedItems
                            If myItem.Key = key Then
                                item = myItem
                            End If
                        Next

                        If item.Key <> Nothing Then
                            If item.TypeID <> Text.TextModules.TextModuleType.PlainTextString Then
                                If valueIsHtml AndAlso (Not IsHtmlType(item.TypeID)) Then
                                    'Encode plain text into HTML (characters as well as line breaks)
                                    'result.Append(Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(ReplacePlaceHolders(item.Value, IsHtmlType(item.TypeID), releasedItems, throwExceptionWhenNoChildCanBeFound))))
                                    Dim str As String = ReplacePlaceHolders(item.Value, IsHtmlType(item.TypeID), releasedItems, throwExceptionWhenNoChildCanBeFound)
                                    str = str.Replace("&lt;", "<")
                                    str = str.Replace("&gt;", ">")

                                    result.Append(str)
                                Else
                                    'Just do a normal replace
                                    result.Append(ReplacePlaceHolders(item.Value, IsHtmlType(item.TypeID), releasedItems, throwExceptionWhenNoChildCanBeFound))
                                End If
                            Else
                                result.Append(item.Value)
                            End If
                        Else
                            'Child entry hasn't been found - this is an error situation!
                            If throwExceptionWhenNoChildCanBeFound Then
                                Throw New Exception("Key """ & key & """ doesn't exist")
                            Else
                                result.Append("{" & key & "}")
                                'result &= "<b><font color='#ff0000'>" & lt & key & gt & "</font></b>"
                            End If
                        End If
                    End While
                End If

                result.Append(temp)

                Return result.ToString
            End Function
            ''' <summary>
            '''     Text module from cache
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="key">The name of the key which uniquely identifies the TextModule</param>
            Public Function TextModulesCached(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal key As String) As DataTable
                Return GetTextModulesCached(websiteAreaID, marketID, serverGroupID, "Key = '" & key & "'")
            End Function
            ''' <summary>
            '''     Text module from cache
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="typeID">type of text module</param>
            Public Function TextModulesCached(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal typeID As Text.TextModules.TextModuleType) As DataTable
                Return GetTextModulesCached(websiteAreaID, marketID, serverGroupID, "TypeID = '" & typeID & "'")
            End Function
            ''' <summary>
            '''     Text module from cache
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="key">The name of the key which uniquely identifies the TextModule</param>
            ''' <param name="typeID">Type of text module</param>
            Public Function TextModulesCached(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal key As String, ByVal typeID As Text.TextModules.TextModuleType) As DataTable
                Return GetTextModulesCached(websiteAreaID, marketID, serverGroupID, "Key = '" & key & "' AND TypeID = '" & typeID & "'")
            End Function
            ''' <summary>
            '''     Text module from cache
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            ''' <param name="query">Query to be made to database</param>
            Private Function GetTextModulesCached(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer, ByVal query As String) As DataTable
                Dim result As New DataTable
                Dim textModules As DataTable = TextModulesCached(websiteAreaID, marketID, serverGroupID)

                For Each col As DataColumn In textModules.Columns
                    If Not result.Columns.Contains(col.ColumnName) Then
                        result.Columns.Add(col.ColumnName, col.DataType)
                    End If
                Next

                For Each dRow As DataRow In textModules.Select(query)
                    result.ImportRow(dRow)
                Next

                Return result
            End Function
            ''' <summary>
            '''     Text module from cache
            ''' </summary>
            ''' <param name="websiteAreaID">WebsiteAreaID of a Text Module</param>
            ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
            ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
            Public Function TextModulesCached(ByVal websiteAreaID As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As DataTable
                Dim result As DataTable = Nothing
                If Not System.Web.HttpContext.Current Is Nothing Then
                    result = CType(System.Web.HttpContext.Current.Cache("WebManager.TextModules." & marketID & "_" & websiteAreaID & "_" & serverGroupID), DataTable)
                End If
                If result Is Nothing Then
                    result = Data.LoadCached(websiteAreaID, marketID, serverGroupID)
                    If Not System.Web.HttpContext.Current Is Nothing Then
                        System.Web.HttpContext.Current.Cache.Add("WebManager.TextModules." & marketID & "_" & websiteAreaID & "_" & serverGroupID, result, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, New TimeSpan(0, 20, 0), Web.Caching.CacheItemPriority.NotRemovable, Nothing)
                    End If
                End If

                Return result
            End Function
            ''' <summary>
            '''     True if value of text module contains html type text module else false
            ''' </summary>
            ''' <param name="value">value of text module to be checked</param>
            ''' <param name="releasedItems">Array of all released text modules</param>
            Public Shared Function IsPlainTextblockContainsHtmlTextModule(ByVal value As String, ByVal releasedItems() As Text.TextModules.ModuleItem) As Boolean
                Dim result As Boolean

                Dim lt As String = "&lt;%"
                Dim gt As String = "%&gt;"

                Dim temp As String = value
                If temp <> Nothing Then
                    While temp.IndexOf(lt) > -1
                        temp = temp.Substring(temp.IndexOf(lt) + lt.Length)
                        Dim key As String = temp.Substring(0, temp.IndexOf(gt))
                        temp = temp.Substring(temp.IndexOf(gt) + gt.Length)

                        Dim item As Text.TextModules.ModuleItem = Nothing
                        If Not releasedItems Is Nothing Then
                            For Each myItem As Text.TextModules.ModuleItem In releasedItems
                                If myItem.Key = key Then
                                    item = myItem
                                End If
                            Next
                        End If

                        If item.Key <> Nothing Then
                            If item.TypeID = TextModules.TextModuleType.HtmlTextBlock Or item.TypeID = TextModules.TextModuleType.HtmlTemplate Then
                                result = True
                            Else
                                If item.Value <> Nothing Then
                                    If item.Value.IndexOf(lt) > -1 Then
                                        result = IsPlainTextblockContainsHtmlTextModule(item.Value, releasedItems)
                                    End If
                                End If
                            End If
                        Else
                            result = False
                        End If
                    End While
                End If

                Return result
            End Function
            ''' <summary>
            '''     Array of all place holders in value
            ''' </summary>
            ''' <param name="value">value of text module</param>
            Public Shared Function AllPlaceHolders(ByVal value As String) As String()
                If value = Nothing Then
                    Return Nothing
                End If
                Dim lt As String = "&lt;%"
                Dim gt As String = "%&gt;"

                Dim list As New ArrayList
                Dim temp As String = value
                If temp <> Nothing Then
                    While temp.IndexOf(lt) > -1
                        temp = temp.Substring(temp.IndexOf(lt) + lt.Length)
                        Dim placeHolder As String = temp.Substring(0, temp.IndexOf(gt))
                        temp = temp.Substring(temp.IndexOf(gt) + gt.Length)

                        list.Add(placeHolder)
                    End While
                End If

                Return CType(list.ToArray(GetType(System.String)), String())
            End Function
            ''' <summary>
            '''     Check validity of key
            ''' </summary>
            ''' <param name="key">Key to be checked</param>
            Public Shared Function IsValid(ByVal key As String) As Boolean
                Dim result As Boolean = True
                Dim invalidCharacters As String = """!?$%&/\\()=?'|#+~-:;,.><"
                If key <> Nothing Then
                    For counter As Integer = 0 To key.Length - 1
                        Dim ch As Char = key.Chars(counter)
                        If invalidCharacters.IndexOf(ch) > -1 Then
                            Return False
                        End If
                    Next
                End If

                Return result
            End Function

        End Class
#End Region

    End Namespace

End Namespace

