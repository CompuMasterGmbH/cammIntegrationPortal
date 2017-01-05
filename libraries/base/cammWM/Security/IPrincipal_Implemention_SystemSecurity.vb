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

    <Serializable()> Public Class WebManagerPrincipal
        Implements System.Security.Principal.IPrincipal

        Private _userInfo As CompuMaster.camm.WebManager.IUserInformation
        Friend Sub New(ByVal user As CompuMaster.camm.WebManager.IUserInformation)
            If user Is Nothing Then
                Throw New ArgumentNullException("user")
            End If
            _userInfo = user
        End Sub

        Public ReadOnly Property Identity() As System.Security.Principal.IIdentity Implements System.Security.Principal.IPrincipal.Identity
            Get
                Return New WebManagerIdentity(_userInfo)
            End Get
        End Property

        Public Function IsInRole(ByVal role As String) As Boolean Implements System.Security.Principal.IPrincipal.IsInRole
            Static groupMemberships As CompuMaster.camm.WebManager.WMSystem.GroupInformation()
            If groupMemberships Is Nothing Then
                groupMemberships = CType(_userInfo, CompuMaster.camm.WebManager.WMSystem.UserInformation).Memberships
            End If
            For MyCounter As Integer = 0 To groupMemberships.Length - 1
                If groupMemberships(MyCounter).Name = role Then
                    Return True
                End If
            Next
            Return False
        End Function
    End Class

    <Serializable()> Public Class WebManagerIdentity
        Implements System.Security.Principal.IIdentity

        Private _userInfo As CompuMaster.camm.WebManager.IUserInformation
        Friend Sub New(ByVal user As CompuMaster.camm.WebManager.IUserInformation)
            If user Is Nothing Then
                Throw New ArgumentNullException("user")
            End If
            _userInfo = user
        End Sub

        Public ReadOnly Property AuthenticationType() As String Implements System.Security.Principal.IIdentity.AuthenticationType
            Get
                Return "camm Web-Manager"
            End Get
        End Property

        Public ReadOnly Property IsAuthenticated() As Boolean Implements System.Security.Principal.IIdentity.IsAuthenticated
            Get
                If Name <> Nothing Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property Name() As String Implements System.Security.Principal.IIdentity.Name
            Get
                Return _userInfo.LoginName
            End Get
        End Property

    End Class

                ' TODO 4 .NET 2.x: Code zum Durchführen der benutzerdefinierten Authentifizierung mithilfe des angegebenen Benutzernamens und des Kennworts hinzufügen 
                ' (Siehe http://go.microsoft.com/fwlink/?LinkId=35339).  
                ' Der benutzerdefinierte Prinzipal kann anschließend wie folgt an den Prinzipal des aktuellen Threads angefügt werden: 
                '     My.User.CurrentPrincipal = CustomPrincipal
                ' wobei CustomPrincipal die IPrincipal-Implementierung ist, die für die Durchführung der Authentifizierung verwendet wird. 
                ' Anschließend gibt My.User Identitätsinformationen zurück, die in das CustomPrincipal-Objekt eingekapselt sind, _
                ' z.B. den Benutzernamen, den Anzeigenamen usw.

                ' New WebManagerPrincipal()
                Dim identity As New System.Security.Principal.GenericIdentity("compumaster")
                My.User.CurrentPrincipal = New System.Security.Principal.GenericPrincipal(identity, New String() {"role1"})

                'Using the principal
                My.User.IsInRole(ApplicationServices.BuiltInRole.AccountOperator) '=Security admins
                My.User.IsInRole(ApplicationServices.BuiltInRole.Administrator) '=Supervisors
                My.User.IsInRole("Supervisors")

                'Auslesen des aktuellen UserNamens
                MsgBox(System.Threading.Thread.CurrentPrincipal.Identity.Name)

                'Exceptions
                System.Security.Principal.IdentityNotMappedException
                System.Security.SecurityException

                ' New WebManagerPrincipal()
                Dim identity As New System.Security.Principal.GenericIdentity("compumaster")
                My.User.CurrentPrincipal = New System.Security.Principal.GenericPrincipal(identity, New String() {"role1"})
                System.Threading.Thread.CurrentPrincipal = New WebManagerPrincipal(Me.cammWebManager.CurrentUserInfo)

                'Using the principal
                My.User.IsInRole(ApplicationServices.BuiltInRole.AccountOperator) '=Security admins
                My.User.IsInRole(ApplicationServices.BuiltInRole.Administrator) '=Supervisors
                My.User.IsInRole("Supervisors")
                My.User.CurrentPrincipal = New System.Security.Principal.GenericPrincipal(identity, New String() {"role1"})

                'By the way: My.User.CurrentPrincipal Is System.Threding.Thread.CurrentPrincipal !!

#End If

End Namespace