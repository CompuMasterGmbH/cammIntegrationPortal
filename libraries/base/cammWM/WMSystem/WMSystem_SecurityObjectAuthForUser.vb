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

Namespace CompuMaster.camm.WebManager

    Partial Public Class WMSystem

        Public Class SecurityObjectAuthorizationForUser

            Friend Sub New(ByVal webmanager As WMSystem, ByVal authorizationID As Integer, ByVal userID As Long, ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, ByVal isDeveloperAuthorization As Boolean, isDenyRule As Boolean, ByVal releasedOn As DateTime, ByVal releasedBy As Long, isRepresentationOfEffectiveAuth As Boolean)
                Me.New(webmanager, authorizationID, userID, securityObjectID, serverGroupID, Nothing, Nothing, Nothing, isDeveloperAuthorization, isDenyRule, releasedOn, releasedBy, isRepresentationOfEffectiveAuth)
            End Sub

            Friend Sub New(ByVal webmanager As WMSystem, ByVal authorizationID As Integer, ByVal userID As Long, ByVal securityObjectID As Integer, ByVal serverGroupID As Integer, ByVal userInfo As UserInformation, ByVal securityObjectInfo As SecurityObjectInformation, ByVal serverGroupInfo As ServerGroupInformation, ByVal isDeveloperAuthorization As Boolean, isDenyRule As Boolean, ByVal releasedOn As DateTime, ByVal releasedBy As Long, isRepresentationOfEffectiveAuth As Boolean)
                Me._WebManager = webmanager
                Me.AuthorizationID = authorizationID
                Me.UserID = userID
                Me.SecurityObjectID = securityObjectID
                Me.ServerGroupID = serverGroupID
                Me.UserInfo = userInfo
                Me.SecurityObjectInfo = securityObjectInfo
                Me.ServerGroupInfo = serverGroupInfo
                Me.IsDeveloperAuthorization = isDeveloperAuthorization
                Me.ReleasedOn = releasedOn
                Me.ReleasedByUserID = releasedBy
                Me.IsDenyRule = isDenyRule
                Me._IsRepresentationOfEffectiveAuth = isRepresentationOfEffectiveAuth
            End Sub

            Private _WebManager As WMSystem
            Friend _IsRepresentationOfEffectiveAuth As Boolean = False

            Private _AuthorizationID As Integer
            Friend Property AuthorizationID() As Integer
                Get
                    Return _AuthorizationID
                End Get
                Set(ByVal value As Integer)
                    _AuthorizationID = value
                End Set
            End Property

            Private _UserID As Long
            Public Property UserID() As Long
                Get
                    Return _UserID
                End Get
                Set(ByVal value As Long)
                    _UserID = value
                End Set
            End Property

            Private _SecurityObjectID As Integer
            Public Property SecurityObjectID() As Integer
                Get
                    Return _SecurityObjectID
                End Get
                Set(ByVal value As Integer)
                    _SecurityObjectID = value
                End Set
            End Property

            Private _ServerGroupID As Integer
            Public Property ServerGroupID() As Integer
                Get
                    Return _ServerGroupID
                End Get
                Set(ByVal value As Integer)
                    _ServerGroupID = value
                End Set
            End Property

            Private _UserInfo As UserInformation
            Public Property UserInfo() As UserInformation
                Get
                    If _UserInfo Is Nothing Then
                        _UserInfo = New UserInformation(_UserID, _WebManager, False)
                    End If
                    Return _UserInfo
                End Get
                Set(ByVal value As UserInformation)
                    _UserInfo = value
                End Set
            End Property

            Private _SecurityObjectInfo As SecurityObjectInformation
            Public Property SecurityObjectInfo() As SecurityObjectInformation
                Get
                    If _SecurityObjectInfo Is Nothing Then
                        _SecurityObjectInfo = New SecurityObjectInformation(_SecurityObjectID, _WebManager, False)
                    End If
                    Return _SecurityObjectInfo
                End Get
                Set(ByVal value As SecurityObjectInformation)
                    _SecurityObjectInfo = value
                End Set
            End Property

            Private _ServerGroupInfo As ServerGroupInformation
            Public Property ServerGroupInfo() As ServerGroupInformation
                Get
                    If _ServerGroupInfo Is Nothing Then
                        _ServerGroupInfo = New ServerGroupInformation(_ServerGroupID, _WebManager)
                    End If
                    Return _ServerGroupInfo
                End Get
                Set(ByVal value As ServerGroupInformation)
                    _ServerGroupInfo = value
                End Set
            End Property

            Private _IsDeveloperAuthorization As Boolean
            Public Property IsDeveloperAuthorization() As Boolean
                Get
                    Return _IsDeveloperAuthorization
                End Get
                Set(ByVal value As Boolean)
                    _IsDeveloperAuthorization = value
                End Set
            End Property

            Private _ReleasedOn As DateTime
            Public Property ReleasedOn() As DateTime
                Get
                    Return _ReleasedOn
                End Get
                Set(ByVal value As DateTime)
                    _ReleasedOn = value
                End Set
            End Property

            Private _ReleasedByUserID As Long
            Public Property ReleasedByUserID() As Long
                Get
                    Return _ReleasedByUserID
                End Get
                Set(ByVal value As Long)
                    _ReleasedByUserID = value
                End Set
            End Property

            Private _IsDenyRule As Boolean
            Public Property IsDenyRule() As Boolean
                Get
                    Return _IsDenyRule
                End Get
                Set(ByVal value As Boolean)
                    _IsDenyRule = value
                End Set
            End Property

        End Class

    End Class

End Namespace
