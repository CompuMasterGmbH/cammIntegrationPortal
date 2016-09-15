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

#If NotImplemented Then
    Public Class AuthorizationMissingException
        Inherits Exception
        Implements System.Runtime.Serialization.ISerializable

        Friend Sub New(ByVal errorValue As WMSystem.System_AccessAuthorizationChecks_DBResults)
            MyBase.New(errorValue.ToString)
        End Sub

    End Class
    Public Class AuthentificationFailedException
        Inherits Exception
        Implements System.Runtime.Serialization.ISerializable

        Friend Sub New(ByVal errorValue As WMSystem.System_AccessAuthorizationChecks_DBResults)
            MyBase.New(errorValue.ToString)
        End Sub

    End Class
#End If

    ''' <summary>
    ''' An exception which occurs when a user account can't be found/loaded
    ''' </summary>
    ''' <remarks></remarks>
    Public Class UserNotFoundException
        Inherits Exception

        Public Sub New(ByVal userID As Long)
            MyBase.New("User account with the requested ID " & userID.ToString & " can't be found")
        End Sub

        Public Sub New(ByVal userName As String)
            MyBase.New("User account with the requested login name " & userName & " can't be found")
        End Sub

    End Class

    Public Class ImpersonationUserNotAbleToStartLoginProcessException
        Inherits Exception

        Public Sub New(ByVal userID As Long)
            MyBase.New("User account with the requested ID " & userID.ToString & " can't be used to start the standard login process")
        End Sub

        Public Sub New(ByVal userName As String)
            MyBase.New("User account with the requested login name " & userName & " can't be used to start the standard login process")
        End Sub

    End Class

    ''' <summary>
    ''' An exception which occurs when a password is too weak
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PasswordTooWeakException
        Inherits UserInfoDataException

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

    End Class

    ''' <summary>
    ''' An exception which occurs when a password is required
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PasswordRequiredException
        Inherits UserInfoDataException

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

    End Class

    ''' <summary>
    ''' An exception which occurs when a password is required
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RequiredFieldException
        Inherits UserInfoDataException

        Public Sub New(ByVal fieldName As String, ByVal message As String)
            MyBase.New(message)
            Me._fieldName = fieldName
        End Sub

        Private _fieldName As String
        Public Property FieldName() As String
            Get
                Return _fieldName
            End Get
            Set(ByVal value As String)
                _fieldName = value
            End Set
        End Property

    End Class

    Public MustInherit Class UserInfoDataException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub

    End Class

    Public Class UserInfoConflictingUniqueKeysException
        Inherits UserInfoDataException

        Friend Sub New(uniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues())
            MyBase.New()
            Me._UniqueKeyConflicts = uniqueKeyConflicts
        End Sub

        Public Overrides ReadOnly Property Message As String
            Get
                Return CreateMessage(UniqueKeyConflicts)
            End Get
        End Property

        Private Shared Function CreateMessage(uniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues()) As String
            If uniqueKeyConflicts Is Nothing OrElse uniqueKeyConflicts.Length = 0 Then Throw New ArgumentNullException("uniqueKeyConflicts")
            Dim NewMessage As String = "Unique keys conflicts found for:"
            For MyCounter As Integer = 0 To uniqueKeyConflicts.Length - 1
                NewMessage &= vbNewLine & uniqueKeyConflicts(MyCounter).ToString
            Next
            Return NewMessage
        End Function

        Private _UniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues()
        Public ReadOnly Property UniqueKeyConflicts As UserInfoConflictingUniqueKeysKeyValues()
            Get
                Return _UniqueKeyConflicts
            End Get
        End Property

    End Class

    Public Class UserInfoConflictingUniqueKeysKeyValues
        Friend Sub New(key As String, conflictingUserIDs As Long(), conflictingValue As String)
            Me._Key = key
            Me._ConflictingValue = conflictingValue
            Me._UserIDs = conflictingUserIDs
        End Sub
        Private _Key As String
        Private _UserIDs As Long()
        Private _ConflictingValue As String
        Public ReadOnly Property Key As String
            Get
                Return _Key
            End Get
        End Property
        Public ReadOnly Property ConflictingUserIDs As Long()
            Get
                Return _UserIDs
            End Get
        End Property
        Public ReadOnly Property ConflictingValue As String
            Get
                Return _ConflictingValue
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return "Key: """ & Me.Key & """, "" value: """ & Me.ConflictingValue & """, user IDs: " & Utils._JoinArrayToString(Me.ConflictingUserIDs, ", ")
        End Function
    End Class

End Namespace