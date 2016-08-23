'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Partial Public Class WMSystem

        ''' <summary>
        '''     Language details
        ''' </summary>
        Public Class LanguageInformation
            Implements ILanguageInformation

            Dim _WebManager As WMSystem
            Dim _ID As Integer
            Dim _LanguageName_English As String
            Dim _LanguageName_OwnLanguage As String
            Dim _IsActive As Boolean
            Dim _BrowserLanguageID As String
            Dim _Abbreviation As String
            Dim _Direction As String
            Friend Sub New(ByVal ID As Integer, ByRef LanguageName_English As String, ByVal LanguageName_OwnLanguage As String, ByVal IsActive As Boolean, ByVal BrowserLanguageID As String, ByVal Abbreviation As String, ByVal DirectionOfLetters As String, ByRef WebManager As WMSystem)
                _ID = ID
                _LanguageName_English = LanguageName_English
                _LanguageName_OwnLanguage = LanguageName_OwnLanguage
                _IsActive = IsActive
                _BrowserLanguageID = BrowserLanguageID
                _Abbreviation = Abbreviation
                _WebManager = WebManager
                _Direction = DirectionOfLetters
            End Sub
            Public Sub New(ByVal ID As Integer, ByRef WebManager As WMSystem)
                _WebManager = WebManager
                Dim MyConn As New SqlConnection(WebManager.ConnectionString)
                Dim MyCmd As New SqlCommand("Select * From Languages Where ID = @ID", MyConn)
                MyCmd.Parameters.Add("@ID", SqlDbType.Int).Value = ID
                Dim MyReader As SqlDataReader = Nothing
                Try
                    MyConn.Open()
                    MyReader = MyCmd.ExecuteReader(CommandBehavior.CloseConnection)
                    If MyReader.Read Then
                        _ID = CType(MyReader("ID"), Integer)
                        _LanguageName_English = Utils.Nz(MyReader("Description"), CType(Nothing, String))
                        _LanguageName_OwnLanguage = Utils.Nz(MyReader("Description_OwnLang"), CType(Nothing, String))
                        _IsActive = Utils.Nz(MyReader("IsActive"), False)
                        _BrowserLanguageID = Utils.Nz(MyReader("BrowserLanguageID"), CType(Nothing, String))
                        _Abbreviation = Utils.Nz(MyReader("Abbreviation"), CType(Nothing, String))
                        If Not CompuMaster.camm.WebManager.Tools.Data.DataQuery.DataReaderUtils.ContainsColumn(MyReader, "DirectionOfLetters") Then
                            'Column hasn't existed yet (database build >= 172 is required for this column to exist)
                            _Direction = "ltr" 'default value
                        Else
                            _Direction = Utils.Nz(MyReader("DirectionOfLetters"), "ltr")
                        End If
                    End If
                Finally
                    If Not MyReader Is Nothing AndAlso Not MyReader.IsClosed Then
                        MyReader.Close()
                    End If
                    If Not MyCmd Is Nothing Then
                        MyCmd.Dispose()
                    End If
                    If Not MyConn Is Nothing Then
                        If MyConn.State <> ConnectionState.Closed Then
                            MyConn.Close()
                        End If
                        MyConn.Dispose()
                    End If
                End Try
            End Sub
            ''' <summary>
            '''     The market/language ID
            ''' </summary>
            ''' <value></value>
            Public ReadOnly Property ID() As Integer
                Get
                    Return _ID
                End Get
            End Property
            ''' <summary>
            '''     The name of the market/language in English language
            ''' </summary>
            ''' <value></value>
            Public Property LanguageName_English() As String
                Get
                    Return _LanguageName_English
                End Get
                Set(ByVal Value As String)
                    _LanguageName_English = Value
                End Set
            End Property
            ''' <summary>
            '''     The name of the market/language in its own language
            ''' </summary>
            ''' <value></value>
            Public Property LanguageName_OwnLanguage() As String
                Get
                    Return _LanguageName_OwnLanguage
                End Get
                Set(ByVal Value As String)
                    _LanguageName_OwnLanguage = Value
                End Set
            End Property
            ''' <summary>
            '''     Market/language has been activated for use in camm Web-Manager
            ''' </summary>
            ''' <value></value>
            Public Property IsActive() As Boolean
                Get
                    Return _IsActive
                End Get
                Set(ByVal Value As Boolean)
                    _IsActive = Value
                End Set
            End Property
            ''' <summary>
            '''     Defines the writing direction, either left-to-right or right-to-left
            ''' </summary>
            ''' <value>A string &quot;ltr&quot; or &quot;rtl&quot;</value>
            Public Property Direction() As String
                Get
                    Return _Direction
                End Get
                Set(ByVal Value As String)
                    Select Case Value
                        Case "ltr", "rtl"
                            'okay
                        Case Else
                            Throw New ArgumentOutOfRangeException("Value", Value, "For direction are allowed values ""ltr"" And ""rtl"" only")
                    End Select
                    _Direction = Value
                End Set
            End Property
            ''' <summary>
            '''     An optional browser ID for the culture
            ''' </summary>
            ''' <value></value>
            Public Property BrowserLanguageID() As String
                Get
                    Return _BrowserLanguageID
                End Get
                Set(ByVal Value As String)
                    _BrowserLanguageID = Value
                End Set
            End Property
            ''' <summary>
            '''     An optional abbreviation name for the language
            ''' </summary>
            ''' <value></value>
            Public Property Abbreviation() As String
                Get
                    Return _Abbreviation
                End Get
                Set(ByVal Value As String)
                    _Abbreviation = Value
                End Set
            End Property
            ''' <summary>
            '''     An optional alternative language, regulary present for market identifiers
            ''' </summary>
            ''' <value></value>
            ''' <remarks>
            '''     This information takes regulary effect for markets. 
            ''' </remarks>
            ''' <example>
            '''     For 'English (US)' as well as 'English (GB)', there is the alternative language 'English'.
            ''' </example>
            Public ReadOnly Property AlternativeLanguageInfo() As LanguageInformation
                Get
                    Return New LanguageInformation(_WebManager.Internationalization.GetAlternativelySupportedLanguageID(_ID), _WebManager)
                End Get
            End Property
        End Class

    End Class

End Namespace