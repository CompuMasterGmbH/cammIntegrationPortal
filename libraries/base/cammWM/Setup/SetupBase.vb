'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Setup

    ''' <summary>
    ''' camm Web-Manager setup base class; must be inherited
    ''' </summary>
    ''' <ToDo>
    '''     Allow recreation of database only when there is not a functional version of the database.
    '''     An indicator for this situation is when in the log table are lesser than 10 entries of page views.
    ''' </ToDo>
    <CLSCompliant(False)> Public MustInherit Class SetupBase
        Public Event WarningsQueueChanged()

        Public Sub New(ByVal ProductName As String)
            _ProductName = ProductName
            If System.Web.HttpContext.Current Is Nothing Then
                WorkDir = AddMissingTrailingSlashToPathString(System.Environment.CurrentDirectory)
            Else
                WorkDir = AddMissingTrailingSlashToPathString(System.Web.HttpContext.Current.Server.MapPath("."))
            End If

            Dim configurationAppSettings As System.Configuration.AppSettingsReader = New System.Configuration.AppSettingsReader
            Try
                _DebugLevel = CType(configurationAppSettings.GetValue("WebManagerDebuglevel", GetType(System.Int32)), Integer)
            Catch
                _DebugLevel = 0
            End Try
        End Sub

        Private _ProductName As String
        Protected _DebugLevel As Integer
        Protected _CurWorkDir As String
        Protected _LogString As New System.Text.StringBuilder
        Protected _WarningsString As New System.Text.StringBuilder
        Protected _LogFile As String
        Protected _LogFileEnabled As Boolean

        Public Property WorkDir() As String
            Get
                Return _CurWorkDir
            End Get
            Set(ByVal Value As String)
                _CurWorkDir = Value
                _LogFile = _CurWorkDir & "camm " & _ProductName & ".log"

                If Not System.Web.HttpContext.Current Is Nothing Then
                    'Never produce logfiles on the server which would be publicly downloadable
                    _LogFileEnabled = False
                Else
                    Try
                        Dim log As System.IO.TextWriter
                        log = New System.IO.StreamWriter(_LogFile, True)
                        log.Close()
                        _LogFileEnabled = True
                    Catch
                        _LogFileEnabled = False
                    End Try
                End If

            End Set
        End Property
        Private Function AddMissingTrailingSlashToPathString(ByVal Path As String) As String
            Dim NeededChar As String = System.IO.Path.DirectorySeparatorChar
            If Path.Length = 0 Then
                Return (Path)
            ElseIf Path.EndsWith(NeededChar) Then
                Return (Path)
            Else
                Return (Path & NeededChar)
            End If
        End Function

        Public Function GetLogData() As String
            Return _LogString.ToString
        End Function

        Public Sub WriteToLog(ByVal Text As String)
            If _LogFileEnabled Then
                Dim log As System.IO.TextWriter
                log = New System.IO.StreamWriter(_LogFile, True)
                log.WriteLine(Now & ": " & Utils.ConvertHTMLToText(Text))
                log.Flush()
                log.Close()
            End If
            _LogString.Append(Text & ControlChars.NewLine)
        End Sub

        Protected Sub RaiseWarning(ByVal Text As String)
            If _WarningsString.Length > 0 Then
                _WarningsString.Append(ControlChars.NewLine)
            End If
            _WarningsString.Append(Text)
            RaiseEvent WarningsQueueChanged()
        End Sub
        Public Property Warnings() As String
            Get
                Return _WarningsString.ToString
            End Get
            Set(ByVal Value As String)
                _WarningsString = New System.Text.StringBuilder
                _WarningsString.Append(Value)
            End Set
        End Property

        Public Property DebugLevel() As Integer
            Get
                Return _DebugLevel
            End Get
            Set(ByVal Value As Integer)
                _DebugLevel = Value
            End Set
        End Property

    End Class

End Namespace