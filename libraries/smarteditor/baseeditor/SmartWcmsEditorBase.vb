'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de
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

Imports System.Web
Imports CompuMaster.camm.WebManager

Namespace CompuMaster.camm.SmartWebEditor

    ''' <summary>
    '''     A base implementation of a smart wcms editor control providing access to the database acces layer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public MustInherit Class SmartWcmsEditorBase
        Inherits CompuMaster.camm.WebManager.Controls.Control
        Implements UI.INamingContainer, ISmartWcmsEditor

        Public ReadOnly Property Configuration() As Configuration
            Get
                Static _Configuration As Configuration
                If _Configuration Is Nothing Then _Configuration = New Configuration
                Return _Configuration
            End Get
        End Property

        ''' <summary>
        '''     The interface implementation required for the database access layer
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Private ReadOnly Property _cammWebManager() As CompuMaster.camm.WebManager.IWebManager Implements ISmartWcmsEditor.cammWebManager
            Get
                Return Me.cammWebManager
            End Get
        End Property

#Region " Database methods "

        ''' <summary>
        '''     Database access layer
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Protected ReadOnly Property Database() As SmartWcmsDatabaseAccessLayer
            Get
                Static _Database As SmartWcmsDatabaseAccessLayer
                If _Database Is Nothing Then
                    _Database = New SmartWcmsDatabaseAccessLayer(Me)
                End If
                Return _Database
            End Get
        End Property

#End Region

#Region "Properties"

        Private _DocumentID As String
        ''' <summary>
        '''     An identifier of the current document, by default its URL
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property DocumentID() As String
            Get
                If _DocumentID Is Nothing Then
                    'Needs to be initialized before the preperation of the VersionHistory HTML String gets started
                    Dim rawUrlWithScriptName As String
                    If System.Environment.Version.Major >= 4 AndAlso HttpContext.Current.Request.RawUrl.EndsWith("/") Then
                        'Beginning with .NET 4, RawUrl contains the URL as requested by the client, so the script name after a folder might be missing; e.g. /test/ is given, but required is /test/default.aspx later on
                        rawUrlWithScriptName = HttpContext.Current.Request.Url.AbsolutePath
                    Else
                        '.NET 1 + 2: RawUrl contains the URL as requested by the client + the request script name, so the script name after a folder is present; e.g. /test/ is given, RawUrl returns the expected /test/default.aspx
                        rawUrlWithScriptName = HttpContext.Current.Request.RawUrl
                    End If
                    If rawUrlWithScriptName.IndexOf("?") >= 0 Then
                        _DocumentID = rawUrlWithScriptName.ToLower.Substring(0, rawUrlWithScriptName.IndexOf("?"))
                    Else
                        _DocumentID = rawUrlWithScriptName.ToLower
                    End If
                End If
                Return _DocumentID
            End Get
            Set(ByVal Value As String)
                _DocumentID = Value
            End Set
        End Property

        Private _ServerID As Integer
        ''' <summary>
        '''     Regulary, content is always related to the current server, only. In some special cases, you might want to override this to show content from another server.
        ''' </summary>
        ''' <value>The ID value of the server to whome the content is related</value>
        ''' <remarks>
        '''     By default, the address (e. g.) "/content.aspx" provides different content on different servers. So, the intranet and the extranet are able to show independent content.
        '''     In some cases, you might want to override this behaviour and you want to show on the same URL the same content in the extranet as well as in the intranet. In this case, you would setup this property on the extranet server's scripts to show the content of the intranet server.
        ''' </remarks>
        Public Property ContentOfServerID() As Integer
            Get
                If _ServerID = Nothing Then
                    _ServerID = Me.Configuration.ContentOfServerID()
                    If _ServerID = Nothing Then
                        _ServerID = Me.cammWebManager.CurrentServerInfo.ID
                    End If
                End If
                Return _ServerID
            End Get
            Set(ByVal Value As Integer)
                _ServerID = Value
            End Set
        End Property

        ''' <summary>
        '''     Contains informations about how to handle the viewonly mode in different market, langs
        ''' </summary>
        Public Enum MarketLookupModes As Integer
            ''' <summary>
            '''     Data is only available in an international version and this is valid for all languages/markets
            ''' </summary>
            ''' <remarks>
            '''     This value is the same as None, just the name is more explainable
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            SingleMarket = 0
            ''' <summary>
            '''     Data is only available in an international version and this is valid for all languages/markets
            ''' </summary>
            ''' <remarks>
            '''     This value is the same as SingleMarket, just the name is more simplified
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            None = 0
            ''' <summary>
            '''     Data is maintained for every market separately, the language markets (e. g. "English", "French", etc. are handled as a separate market)
            ''' </summary>
            ''' <remarks>
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            Market = 1
            ''' <summary>
            '''     Data is maintained for every language/market separately; when there is no value for a market it will be searched for some compatible language data
            ''' </summary>
            ''' <remarks>
            '''     Example: When the visitor is in market "German/Austria" but there is only some content available for market "German", the German data will be used.
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            Language = 2
            ''' <summary>
            '''     Data is maintained for every language/market separately; when there is no value for the current market, the sWCMS control tries to lookup a best matching content
            ''' </summary>
            ''' <remarks>
            '''     When the user requests a page in e. g. market 559 ("French/France"), there will be the following order for the lookup process:
            '''     <list>
            '''         <item>Current market, in ex. ID 559 / French/France</item>
            '''         <item>Current language of market, in ex. ID 3 / French</item>
            '''         <item>Until customized by propert AlternativeDataMarkets: English universal, ID 1</item>
            '''         <item>Until customized by propert AlternativeDataMarkets: Worldwide market, ID 10000</item>
            '''         <item>International, ID 0</item>
            '''     </list>
            '''     If the lookup process fails, an HTTP error 404 will be returned to the browser.
            ''' </remarks>
            BestMatchingLanguage = 3
        End Enum

        Private _MarketLookupMode As MarketLookupModes
        ''' <summary>
        '''     Represents the current MarketLookupMode, passed as parameter by the ctrl
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property MarketLookupMode() As MarketLookupModes
            Get
                Return _MarketLookupMode
            End Get
            Set(ByVal Value As MarketLookupModes)
                _MarketLookupMode = Value
            End Set
        End Property 'MarketLookupMode()

        Private _SecurityObjectEditMode As String
        ''' <summary>
        '''     Indicates which application is needed to edit the formular
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property SecurityObjectEditMode() As String
            Get
                Return _SecurityObjectEditMode
            End Get
            Set(ByVal Value As String)
                _SecurityObjectEditMode = Value
            End Set
        End Property 'SecurityObjectEditMode()

#End Region

    End Class

End Namespace