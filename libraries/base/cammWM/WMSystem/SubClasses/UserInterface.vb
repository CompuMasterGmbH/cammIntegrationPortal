'Copyright 2006,2015,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     Provide the cammWebManager.UI elements
    ''' </summary>
    Public Class UserInterface

        Private _webManager As CompuMaster.camm.WebManager.WMSystem
        Friend Sub New(ByVal webManager As CompuMaster.camm.WebManager.WMSystem)
            _webManager = webManager
        End Sub

        Public Function MarketID() As Integer
            Return _webManager.UIMarket(0)
        End Function

        Public Function LanguageID() As Integer
            Return _webManager.Internationalization.GetAlternativelySupportedLanguageID(MarketID)
        End Function

        Private ReadOnly Property TextModules() As CompuMaster.camm.WebManager.Modules.Text.TextModules
            Get
                Static _TextModules As CompuMaster.camm.WebManager.Modules.Text.TextModules
                If _TextModules Is Nothing Then
                    _TextModules = New CompuMaster.camm.WebManager.Modules.Text.TextModules(_webManager)
                End If
                Return _TextModules
            End Get
        End Property
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        ''' <remarks>
        '''     By default, the requested websitAreaID is empty.
        ''' </remarks>
        Public Function TextModule(ByVal key As String) As String
            Return TextModules.Load(key)
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websitAreaID">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        Public Function TextModule(ByVal key As String, ByVal websitAreaID() As String) As String
            Return TextModules.Load(key, websitAreaID)
        End Function
        ''' <summary>
        '''     Load a text module
        ''' </summary>
        ''' <param name="key">The name of the key which uniquely identifies the required value</param>
        ''' <param name="websitAreaID">An array of strings with unique IDs of the website area, e. g. {"Shop", "default", ""} - the order defines the priority</param>
        ''' <param name="marketID">The queried and returned data must match to this market or its alternative language or a neutral culture</param>
        ''' <param name="serverGroupID">The server group for which the requested text module must be available (e. g. Extranet might contain a different editorial than the Intranet area)</param>
        ''' <returns>A plain text string or HTML code (defined by the variable type) with the complete value of the text</returns>
        Public Function TextModule(ByVal key As String, ByVal websitAreaID() As String, ByVal marketID As Integer, ByVal serverGroupID As Integer) As String
            Return TextModules.Load(key, websitAreaID, marketID, serverGroupID)
        End Function

    End Class

End Namespace