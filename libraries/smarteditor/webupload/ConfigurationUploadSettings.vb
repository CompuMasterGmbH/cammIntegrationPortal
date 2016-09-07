'Copyright 2005,2007,2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Option Strict On
Option Explicit On

Imports CompuMaster.camm.WebManager

Namespace CompuMaster.camm.SmartWebEditor.Pages

    Friend Class ConfigurationUploadSettings

        Private Shared ReadOnly Property WebEditorSettings(settingName As String) As String
            Get
                If settingName.StartsWith("WebManager.Wcms.") Then
                    Return ConfigurationUploadWebManager.WebManagerSettings(settingName)
                Else
                    Throw New ArgumentException("Not a SmartEditor setting")
                End If
            End Get
        End Property


        Private _NumberOfFrameTypes As Integer
        Public Property NumberOfFrameTypes() As Integer
            Get
                If _NumberOfFrameTypes = Nothing Then
                    _NumberOfFrameTypes = Utils.TryCInt(WebEditorSettings("WebManager.Wcms.ImageUploads.NumberOfFrameTypes"))
                End If
                If _NumberOfFrameTypes = Nothing Then
                    _NumberOfFrameTypes = 4
                End If
                Return _NumberOfFrameTypes
            End Get
            Set(ByVal Value As Integer)
                _NumberOfFrameTypes = Value
            End Set
        End Property

        Private _MiniatureViewMaxWidth As Integer
        Public Property MiniatureViewMaxWidth() As Integer
            Get
                If _MiniatureViewMaxWidth = Nothing Then
                    Try
                        _MiniatureViewMaxWidth = Utils.TryCInt(WebEditorSettings("WebManager.Wcms.ImageUploads.MiniatureViewMaxWidth"))
                    Catch
                    End Try
                End If
                If _MiniatureViewMaxWidth = Nothing Then
                    _MiniatureViewMaxWidth = 150
                End If
                Return _MiniatureViewMaxWidth
            End Get
            Set(ByVal Value As Integer)
                _MiniatureViewMaxWidth = Value
            End Set
        End Property

        Private _MiniatureViewMaxHeight As Integer
        Public Property MiniatureViewMaxHeight() As Integer
            Get
                If _MiniatureViewMaxHeight = Nothing Then
                    _MiniatureViewMaxHeight = Utils.TryCInt(WebEditorSettings("WebManager.Wcms.ImageUploads.MiniatureViewMaxHeight"))
                End If
                If _MiniatureViewMaxHeight = Nothing Then
                    _MiniatureViewMaxHeight = 150
                End If
                Return _MiniatureViewMaxHeight
            End Get
            Set(ByVal Value As Integer)
                _MiniatureViewMaxHeight = Value
            End Set
        End Property

        Private _NormalViewMaxWidth As Integer
        Public Property NormalViewMaxWidth() As Integer
            Get
                If _NormalViewMaxWidth = Nothing Then
                    _NormalViewMaxWidth = Utils.TryCInt(WebEditorSettings("WebManager.Wcms.ImageUploads.NormalViewMaxWidth"))
                End If
                If _NormalViewMaxWidth = Nothing Then
                    _NormalViewMaxWidth = 300
                End If
                Return _NormalViewMaxWidth
            End Get
            Set(ByVal Value As Integer)
                _NormalViewMaxWidth = Value
            End Set
        End Property

        Private _NormalViewMaxHeight As Integer
        Public Property NormalViewMaxHeight() As Integer
            Get
                If _NormalViewMaxHeight = Nothing Then
                    _NormalViewMaxHeight = Utils.TryCInt(WebEditorSettings("WebManager.Wcms.ImageUploads.NormalViewMaxHeight"))
                End If
                If _NormalViewMaxHeight = Nothing Then
                    _NormalViewMaxHeight = 250
                End If
                Return _NormalViewMaxHeight
            End Get
            Set(ByVal Value As Integer)
                _NormalViewMaxHeight = Value
            End Set
        End Property

        Private _MiniatureView As TriState = TriState.UseDefault
        Public Property MiniatureView() As Boolean
            Get
                Dim Result As Boolean
                If _MiniatureView = TriState.UseDefault Then
                    Dim ConfigValue As String = LCase(WebEditorSettings("WebManager.Wcms.ImageUploads.MiniatureView"))
                    If ConfigValue = "false" Then
                        Result = False
                    ElseIf ConfigValue = "true" Then
                        Result = True
                    ElseIf ConfigValue = "" Then
                        Result = True
                    Else
                        Throw New Exception("Invalid configuration parameter is given! - Parameter: MiniatureView")
                    End If
                ElseIf _MiniatureView = TriState.True Then
                    Result = True
                Else
                    Result = False
                End If
                Return Result
            End Get
            Set(ByVal Value As Boolean)
                If Value = True Then
                    _MiniatureView = TriState.True
                Else
                    _MiniatureView = TriState.False
                End If
            End Set
        End Property

        Private _NormalView As TriState = TriState.UseDefault
        Public Property NormalView() As Boolean
            Get
                Dim Result As Boolean
                If _NormalView = TriState.UseDefault Then
                    Dim ConfigValue As String = LCase(WebEditorSettings("WebManager.Wcms.ImageUploads.NormalView"))
                    If ConfigValue = "false" Then
                        Result = False
                    ElseIf ConfigValue = "true" Then
                        Result = True
                    ElseIf ConfigValue = "" Then
                        Result = True
                    Else
                        Throw New Exception("Invalid configuration parameter is given! - Parameter: NormalView")
                    End If
                ElseIf _NormalView = TriState.True Then
                    Result = True
                Else
                    Result = False
                End If
                Return Result
            End Get
            Set(ByVal Value As Boolean)
                If Value = True Then
                    _NormalView = TriState.True
                Else
                    _NormalView = TriState.False
                End If
            End Set
        End Property

    End Class

End Namespace