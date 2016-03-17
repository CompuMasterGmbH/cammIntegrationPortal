'Copyright 2014-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager.Security

    Public Class NavigationInformation

        Friend Sub New(securityObjectID As Integer, _
                        securityObjectInfo As SecurityObjectInformation, _
                        level1Title As String, _
                        level2Title As String, _
                        level3Title As String, _
                        level4Title As String, _
                        level5Title As String, _
                        level6Title As String, _
                        level1TitleIsHtmlCoded As Boolean, _
                        level2TitleIsHtmlCoded As Boolean, _
                        level3TitleIsHtmlCoded As Boolean, _
                        level4TitleIsHtmlCoded As Boolean, _
                        level5TitleIsHtmlCoded As Boolean, _
                        level6TitleIsHtmlCoded As Boolean, _
                        navUrl As String, _
                        navFrame As String, _
                        navTooltipText As String, _
                        addMarketIDToUrl As Boolean, _
                        marketID As Integer, _
                        serverID As Integer, _
                        sort As Long, _
                        isNew As Boolean, _
                        isUpdated As Boolean, _
                        resetIsNewUpdatedStatusOn As DateTime, _
                        onMouseOver As String, _
                        onMouseOut As String, _
                        onClick As String)
            Me._SecurityObjectID = securityObjectID
            Me._SecurityObjectInfo = securityObjectInfo
            Me._Level1Title = level1Title
            Me._Level2Title = level2Title
            Me._Level3Title = level3Title
            Me._Level4Title = level4Title
            Me._Level5Title = level5Title
            Me._Level6Title = level6Title
            Me._Level1TitleIsHtmlCoded = level1TitleIsHtmlCoded
            Me._Level2TitleIsHtmlCoded = level2TitleIsHtmlCoded
            Me._Level3TitleIsHtmlCoded = level3TitleIsHtmlCoded
            Me._Level4TitleIsHtmlCoded = level4TitleIsHtmlCoded
            Me._Level5TitleIsHtmlCoded = level5TitleIsHtmlCoded
            Me._Level6TitleIsHtmlCoded = level6TitleIsHtmlCoded
            Me._NavUrl = navUrl
            Me._NavFrame = navFrame
            Me._NavTooltipText = navTooltipText
            Me._AddLanguageIDToUrl = addMarketIDToUrl
            Me._LanguageID = marketID
            Me._LocationID = serverID
            Me._Sort = sort
            Me._IsNew = isNew
            Me._IsUpdated = isUpdated
            Me._ResetIsNewUpdatedStatusOn = resetIsNewUpdatedStatusOn
            Me._OnMouseOver = onMouseOver
            Me._OnMouseOut = onMouseOut
            Me._OnClick = onClick

        End Sub

        Private _SecurityObjectID As Integer
        Private _SecurityObjectInfo As SecurityObjectInformation
        Private _Level1Title As String
        Private _Level2Title As String
        Private _Level3Title As String
        Private _Level4Title As String
        Private _Level5Title As String
        Private _Level6Title As String
        Private _Level1TitleIsHtmlCoded As Boolean
        Private _Level2TitleIsHtmlCoded As Boolean
        Private _Level3TitleIsHtmlCoded As Boolean
        Private _Level4TitleIsHtmlCoded As Boolean
        Private _Level5TitleIsHtmlCoded As Boolean
        Private _Level6TitleIsHtmlCoded As Boolean
        Private _NavUrl As String
        Private _NavFrame As String
        Private _NavTooltipText As String
        Private _AddLanguageIDToUrl As Boolean
        Private _LanguageID As Integer
        Private _LocationID As Integer
        Private _Sort As Long
        Private _IsNew As Boolean
        Private _IsUpdated As Boolean
        Private _ResetIsNewUpdatedStatusOn As DateTime
        Private _OnMouseOver As String
        Private _OnMouseOut As String
        Private _OnClick As String

        Public ReadOnly Property SecurityObjectID As Integer
            Get
                If Not _SecurityObjectInfo Is Nothing Then
                    Return _SecurityObjectInfo.ID
                Else
                    Return _SecurityObjectID
                End If
            End Get
        End Property
        Public ReadOnly Property SecurityObjectInfo As SecurityObjectInformation
            Get
                Return _SecurityObjectInfo
            End Get
        End Property
        ''' <summary>
        ''' Required as long as construction of NavInfos in not independent from SecurityObjectInfo (=as long as SplittedNav feature not fully supported)
        ''' </summary>
        ''' <param name="securityObject"></param>
        <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Friend Sub SetSecurityObjectInfoInternal(securityObject As SecurityObjectInformation)
            Me._SecurityObjectInfo = securityObject
        End Sub

        Public Property Level1Title As String
            Get
                Return _Level1Title
            End Get
            Set(value As String)
                _Level1Title = value
            End Set
        End Property
        Public Property Level2Title As String
            Get
                Return _Level2Title
            End Get
            Set(value As String)
                _Level2Title = value
            End Set
        End Property
        Public Property Level3Title As String
            Get
                Return _Level3Title
            End Get
            Set(value As String)
                _Level3Title = value
            End Set
        End Property
        Public Property Level4Title As String
            Get
                Return _Level4Title
            End Get
            Set(value As String)
                _Level4Title = value
            End Set
        End Property
        Public Property Level5Title As String
            Get
                Return _Level5Title
            End Get
            Set(value As String)
                _Level5Title = value
            End Set
        End Property
        Public Property Level6Title As String
            Get
                Return _Level6Title
            End Get
            Set(value As String)
                _Level6Title = value
            End Set
        End Property
        Public Property Level1TitleIsHtmlCoded As Boolean
            Get
                Return _Level1TitleIsHtmlCoded
            End Get
            Set(value As Boolean)
                _Level1TitleIsHtmlCoded = value
            End Set
        End Property
        Public Property Level2TitleIsHtmlCoded As Boolean
            Get
                Return _Level2TitleIsHtmlCoded
            End Get
            Set(value As Boolean)
                _Level2TitleIsHtmlCoded = value
            End Set
        End Property
        Public Property Level3TitleIsHtmlCoded As Boolean
            Get
                Return _Level3TitleIsHtmlCoded
            End Get
            Set(value As Boolean)
                _Level3TitleIsHtmlCoded = value
            End Set
        End Property
        Public Property Level4TitleIsHtmlCoded As Boolean
            Get
                Return _Level4TitleIsHtmlCoded
            End Get
            Set(value As Boolean)
                _Level4TitleIsHtmlCoded = value
            End Set
        End Property
        Public Property Level5TitleIsHtmlCoded As Boolean
            Get
                Return _Level5TitleIsHtmlCoded
            End Get
            Set(value As Boolean)
                _Level5TitleIsHtmlCoded = value
            End Set
        End Property
        Public Property Level6TitleIsHtmlCoded As Boolean
            Get
                Return _Level6TitleIsHtmlCoded
            End Get
            Set(value As Boolean)
                _Level6TitleIsHtmlCoded = value
            End Set
        End Property
        Public Property NavUrl As String
            Get
                Return _NavUrl
            End Get
            Set(value As String)
                _NavUrl = value
            End Set
        End Property
        Public Property NavFrame As String
            Get
                Return _NavFrame
            End Get
            Set(value As String)
                _NavFrame = value
            End Set
        End Property
        Public Property NavTooltipText As String
            Get
                Return _NavTooltipText
            End Get
            Set(value As String)
                _NavTooltipText = value
            End Set
        End Property
        Public Property IsNew As Boolean
            Get
                Return _IsNew
            End Get
            Set(value As Boolean)
                _IsNew = value
            End Set
        End Property
        Public Property IsUpdated As Boolean
            Get
                Return _IsUpdated
            End Get
            Set(value As Boolean)
                _IsUpdated = value
            End Set
        End Property
        Public Property ServerID As Integer
            Get
                Return _LocationID
            End Get
            Set(value As Integer)
                _LocationID = value
            End Set
        End Property
        Public Property MarketID As Integer
            Get
                Return _LanguageID
            End Get
            Set(value As Integer)
                _LanguageID = value
            End Set
        End Property
        Public Property Sort As Long
            Get
                Return _Sort
            End Get
            Set(value As Long)
                _Sort = value
            End Set
        End Property
        Public Property ResetIsNewUpdatedStatusOn As DateTime
            Get
                Return _ResetIsNewUpdatedStatusOn
            End Get
            Set(value As DateTime)
                _ResetIsNewUpdatedStatusOn = value
            End Set
        End Property
        Public Property OnMouseOver As String
            Get
                Return _OnMouseOver
            End Get
            Set(value As String)
                _OnMouseOver = value
            End Set
        End Property
        Public Property OnMouseOut As String
            Get
                Return _OnMouseOut
            End Get
            Set(value As String)
                _OnMouseOut = value
            End Set
        End Property
        Public Property OnClick As String
            Get
                Return _OnClick
            End Get
            Set(value As String)
                _OnClick = value
            End Set
        End Property
        Public Property AddMarketIDToUrl As Boolean
            Get
                Return _AddLanguageIDToUrl
            End Get
            Set(value As Boolean)
                _AddLanguageIDToUrl = value
            End Set
        End Property


    End Class

End Namespace