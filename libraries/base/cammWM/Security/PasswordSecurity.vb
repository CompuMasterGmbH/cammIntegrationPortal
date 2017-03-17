'Copyright 2004-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager

    ''' <summary>
    '''     Container for policy rules for password security
    ''' </summary>
    ''' <remarks>
    '''     Defines a set of rules to validate and generate user passwords
    ''' </remarks>
    Public Class WMPasswordSecurity
        Inherits CompuMaster.camm.WebManager.PasswordSecurity
        ''' <summary>
        '''     Creates a new WMPasswordSecurity class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        Public Sub New(ByVal webManager As WMSystem)
            MyBase.New(webManager)
        End Sub
    End Class

    ''' <summary>
    '''     Policy rules for password security
    ''' </summary>
    ''' <remarks>
    '''     Defines a set of rules to validate and generate user passwords
    ''' </remarks>
    Public Class WMPasswordSecurityInspectionSeverity
        Inherits CompuMaster.camm.WebManager.PasswordSecurityInspectionSeverity
        ''' <summary>
        '''     Creates a new WMPasswordSecurityInspectionSeverity class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        Public Sub New(ByVal webManager As WMSystem)
            MyBase.New(webManager)
        End Sub
        ''' <summary>
        '''     Creates a new WMPasswordSecurityInspectionSeverity class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        ''' <param name="requiredPasswordLength">The required password length</param>
        ''' <param name="requiredComplexityPoints">A number of required complexity points for a successfull validation</param>
        Public Sub New(ByVal webmanager As WMSystem, ByVal requiredPasswordLength As Integer, ByVal requiredComplexityPoints As Integer)
            MyBase.New(webmanager, requiredPasswordLength, requiredComplexityPoints)
        End Sub

    End Class

    ''' <summary>
    '''     Container for policy rules for password security
    ''' </summary>
    ''' <remarks>
    '''     Defines a set of rules to validate and generate user passwords
    ''' </remarks>
    Public Class PasswordSecurity
        Private _webManager As WMSystem
        ''' <summary>
        '''     Creates a new WMPasswordSecurity class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        Public Sub New(ByRef webManager As WMSystem)
            _webManager = webManager
        End Sub
        Dim _InspectionSeverities As PasswordSecurityInspectionSeverity()
        ''' <summary>
        '''     A WMPasswordSecurityInspectionSeverity for the selected access level
        ''' </summary>
        ''' <param name="accessLevelID">An access level ID</param>
        ''' <value>A new WMPasswordSecurityInspectionSeverity which shall be assigned</value>
        Default Public Property InspectionSeverities(ByVal accessLevelID As Integer) As WMPasswordSecurityInspectionSeverity
            Get
                If _InspectionSeverities Is Nothing OrElse UBound(_InspectionSeverities) < accessLevelID Then
                    ReDim Preserve _InspectionSeverities(accessLevelID)
                End If
                If _InspectionSeverities(accessLevelID) Is Nothing Then
                    _InspectionSeverities(accessLevelID) = New WMPasswordSecurityInspectionSeverity(_webManager)
                End If
                Return CType(_InspectionSeverities(accessLevelID), WMPasswordSecurityInspectionSeverity)
            End Get
            Set(ByVal Value As WMPasswordSecurityInspectionSeverity)
                If _InspectionSeverities Is Nothing OrElse UBound(_InspectionSeverities) < accessLevelID Then
                    ReDim Preserve _InspectionSeverities(accessLevelID)
                End If
                _InspectionSeverities(accessLevelID) = Value
            End Set
        End Property
    End Class

    ''' <summary>
    '''     Policy rules for password security
    ''' </summary>
    ''' <remarks>
    '''     Defines a set of rules to validate and generate user passwords
    ''' </remarks>
    Public Class PasswordSecurityInspectionSeverity
        Dim _RequiredComplexityPoints As Integer = 1
        Dim _RequiredPasswordLength As Integer = 3
        Dim _RequiredMaximumPasswordLength As Integer = 20
        Dim _RecommendedPasswordLength As Integer = 8
        Dim _AllowedChars As Char() = Nothing
        Dim _ForbiddenChars As Char() = Nothing
        Dim _WebManager As WMSystem
        Dim _ErrorMessageComplexityPoints As New Collections.SortedList
        ''' <summary>
        '''     Creates a new WMPasswordSecurityInspectionSeverity class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        Public Sub New(ByVal webManager As WMSystem)
            _WebManager = webManager
        End Sub
        ''' <summary>
        '''     Creates a new WMPasswordSecurityInspectionSeverity class
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
        ''' <param name="requiredPasswordLength">The required password length</param>
        ''' <param name="requiredComplexityPoints">A number of required complexity points for a successfull validation</param>
        Public Sub New(ByVal webManager As WMSystem, ByVal requiredPasswordLength As Integer, ByVal requiredComplexityPoints As Integer)
            _RequiredPasswordLength = requiredPasswordLength
            _RequiredComplexityPoints = requiredComplexityPoints
            _WebManager = webManager
        End Sub
        ''' <summary>
        '''     The error message for the selected language when the required complexity points haven't been reached
        ''' </summary>
        ''' <param name="marketID">The language of the error message</param>
        ''' <value>The new error message</value>
        Public Property ErrorMessageComplexityPoints(ByVal marketID As Integer) As String
            Get
                If Not _ErrorMessageComplexityPoints.Item(marketID) Is Nothing Then
                    Return CType(_ErrorMessageComplexityPoints.Item(marketID), String)
                ElseIf Not _ErrorMessageComplexityPoints.Item(Me._WebManager.Internationalization.GetAlternativelySupportedLanguageID(marketID)) Is Nothing Then
                    Return CType(_ErrorMessageComplexityPoints.Item(Me._WebManager.Internationalization.GetAlternativelySupportedLanguageID(marketID)), String)
                Else
                    Return _WebManager.Internationalization.UpdatePW_Error_PasswordComplexityPolicy
                End If
            End Get
            Set(ByVal Value As String)
                _ErrorMessageComplexityPoints.Item(marketID) = Value
            End Set
        End Property
        ''' <summary>
        '''     The number of characters needed to successfully build or validate a password 
        ''' </summary>
        ''' <value>The number of required characters</value>
        Public Property RequiredPasswordLength() As Integer
            Get
                Return _RequiredPasswordLength
            End Get
            Set(ByVal Value As Integer)
                _RequiredPasswordLength = Value
            End Set
        End Property
        ''' <summary>
        '''     The maximum number of characters allowed to successfully build or validate a password 
        ''' </summary>
        ''' <value>The maximum number of characters</value>
        Public Property RequiredMaximumPasswordLength() As Integer
            Get
                Return _RequiredMaximumPasswordLength
            End Get
            Set(ByVal Value As Integer)
                _RequiredMaximumPasswordLength = Value
            End Set
        End Property
        ''' <summary>
        '''     The number of complexity points to successfully build or validate a password 
        ''' </summary>
        ''' <value>The number of required complexity points</value>
        Public Property RequiredComplexityPoints() As Integer
            Get
                Return _RequiredComplexityPoints
            End Get
            Set(ByVal Value As Integer)
                _RequiredComplexityPoints = Value
            End Set
        End Property
        ''' <summary>
        '''     The number of recommended characters to successfully generate a secure password 
        ''' </summary>
        ''' <value>The number of characters for randomly generated passwords</value>
        Public Property RecommendedPasswordLength() As Integer
            Get
                Return _RecommendedPasswordLength
            End Get
            Set(ByVal Value As Integer)
                _RecommendedPasswordLength = Value
            End Set
        End Property
        ''' <summary>
        '''     A list of forbidden characters
        ''' </summary>
        ''' <value>The number of characters for randomly generated passwords</value>
        Public Property ForbiddenChars() As Char()
            Get
                Return _ForbiddenChars
            End Get
            Set(ByVal Value As Char())
                _ForbiddenChars = Value
            End Set
        End Property
        ''' <summary>
        '''     A list of allowed characters or null/Nothing to allow all characters
        ''' </summary>
        ''' <value>The number of characters for randomly generated passwords</value>
        Public Property AllowedChars() As Char()
            Get
                Return _AllowedChars
            End Get
            Set(ByVal Value As Char())
                _AllowedChars = Value
            End Set
        End Property
        ''' <summary>
        '''     Create a random, secure password
        ''' </summary>
        ''' <param name="length">The length of the new password</param>
        ''' <returns>The new generated password</returns>
        Public Overridable Function CreateRandomSecurePassword(Optional ByVal length As Integer = Nothing) As String
            Dim Result As String
            Dim _Length As Integer
            If length = Nothing Then
                _Length = Me.RecommendedPasswordLength
            ElseIf length < Me.RequiredPasswordLength Then
                _Length = Me.RequiredPasswordLength
            ElseIf length > Me.RequiredMaximumPasswordLength Then
                _Length = Me.RequiredMaximumPasswordLength
            Else
                _Length = length
            End If
            Dim Counter As Int32
            Do
                'If the customizing has been made faulty, here could be an endless loop --> detect this situation and abort
                Counter += 1
                If Counter > 100 Then
                    Throw New Exception("CreateRandomSecurePassword isn't able to create a valid password")
                End If
                'create a new random password
                Result = CreateRandomPassword(_Length)
            Loop Until ValidatePasswordComplexity(Result) = PasswordComplexityValidationResult.Success
            Return Result
        End Function
        ''' <summary>
        ''' Create a random, secure password with the given length
        ''' </summary>
        ''' <param name="length">The desired length for the new password</param>
        ''' <returns>A string with a strong typed password</returns>
        Protected Overridable Function CreateRandomPassword(ByVal length As Integer) As String
            Const specchars As String = "@#$?!.+~%=;:()_-\/*&"
            Const alphabetnumbers As String = _
                "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"

            Dim _Length As Integer
            If length = Nothing Then
                _Length = Me.RecommendedPasswordLength
            ElseIf length < Me.RequiredPasswordLength Then
                _Length = Me.RequiredPasswordLength
            ElseIf length > Me.RequiredMaximumPasswordLength Then
                _Length = Me.RequiredMaximumPasswordLength
            Else
                _Length = length
            End If

            'Prepare random base string
            Dim max As Integer, loc As String, i As Integer, a As Integer, tmp As String
            max = 62
            loc = alphabetnumbers
            tmp = ""
            For i = 1 To CInt(_Length - 1)
                Randomize()
                a = CInt(Rnd() * max) + 1
                Randomize()
                a = CInt(Rnd() * a) + 1
                If a > max Or a < 1 Then a = 3
                tmp = tmp & Mid(loc, a, 1)
            Next
            tmp = StrReverse(tmp)

            'Add special char
            Randomize()
            a = CInt(Rnd() * Len(specchars)) + 1
            If a > Len(specchars) Then a = 1
            loc = specchars
            'add it into the middle of the string
            Dim MiddleOfString As Integer
            MiddleOfString = CType(System.Math.Floor(CDbl(tmp.Length) / 2), Integer)
            If tmp.Length Mod 2 = 1 Then MiddleOfString += 1
            If MiddleOfString = 0 Then
                tmp &= Mid(loc, a, 1)
            Else
                tmp = Mid(tmp, 1, MiddleOfString) & Mid(loc, a, 1) & Mid(tmp, MiddleOfString + 1)
            End If

            'Prepare output
            Dim wordarray As Char()
            ReDim wordarray(length - 1)
            For i = 1 To Len(tmp)
                wordarray(i - 1) = Mid(tmp, i, 1).Chars(0)
            Next
            Dim tmpRearranged As String = ""
            For i = 0 To UBound(wordarray) Step 2
                If i > UBound(wordarray) Then Exit For
                tmpRearranged = tmpRearranged & wordarray(i)
            Next
            For i = 1 To UBound(wordarray) Step 2
                If i > UBound(wordarray) Then Exit For
                tmpRearranged = tmpRearranged & wordarray(i)
            Next

            Dim Result As String = StrReverse(tmpRearranged)
            If Result = Nothing Then
                Throw New Exception("Internal exception - resulting password was empty")
            ElseIf Result.IndexOf(ChrW(0)) > -1 Then
                Throw New Exception("Internal exception - resulting password contains invalid zero-characters")
            ElseIf Result.Length > 2 Then
                If Result.IndexOfAny(specchars.ToCharArray) = 0 Then
                    'Special char at the very beginning should be moved into the middle of the string (lesser confusions for users)
                    Result = Result.Chars(1) & Result.Chars(0) & Result.Substring(2)
                End If
                If Result.LastIndexOfAny(specchars.ToCharArray) = Result.Length - 1 Then
                    'Special char at the very ending should be moved into the middle of the string (lesser confusions for users)
                    Result = Result.Substring(0, Result.Length - 2) & Result.Chars(Result.Length - 1) & Result.Chars(Result.Length - 2)
                End If
            End If
            Return Result

        End Function

        Public Enum PasswordComplexityValidationResult As Integer
            Success = -1
            Failure_Unspecified = 0
            Failure_LengthMinimum = 1
            Failure_LengthMaximum = 2
            Failure_HigherPasswordComplexityRequired = 3
            Failure_NotAllowed_PartOfProfileInformation = 4
            Failure_NotAllowed_ForbiddenChar = 5
            Failure_NotAllowed_OnlyWhiteListedCharsAllowed = 6
        End Enum
        ''' <summary>
        '''     Validates the complexity of a password with the user information
        ''' </summary>
        ''' <param name="password">The password to be validated</param>
        ''' <param name="userInformation">The user's profile</param>
        ''' <returns>The result of the validation</returns>
        ''' <remarks>
        '''     A password should never contain parts of the user name or other things often used by hackers.
        '''     To prevent a user from creating those lazy passwords, there is the need to validate it against the data of his user profile.
        ''' </remarks>
        Public Overridable Function ValidatePasswordComplexity(ByVal password As String, ByVal userInformation As CompuMaster.camm.WebManager.WMSystem.UserInformation) As PasswordComplexityValidationResult
            Dim MyString As New ArrayList
            If userInformation.LoginName <> Nothing Then MyString.Add(Mid(userInformation.LoginName, 1, 4))
            If userInformation.FirstName <> Nothing Then MyString.Add(Mid(userInformation.FirstName, 1, 4))
            If userInformation.LastName <> Nothing Then MyString.Add(Mid(userInformation.LastName, 1, 4))
            Return ValidatePasswordComplexity(password, CType(MyString.ToArray(GetType(String)), String()))
        End Function
        ''' <summary>
        '''     Validates the complexity of a password with the user information
        ''' </summary>
        ''' <param name="password">The password to be validated</param>
        ''' <param name="userID">The ID of the user the password is planned for</param>
        ''' <returns>The result of the validation</returns>
        ''' <remarks>
        '''     A password should never contain parts of the user name or other things often used by hackers.
        '''     To prevent a user from creating those lazy passwords, there is the need to validate it against the data of his user profile.
        ''' </remarks>
        Public Overridable Function ValidatePasswordComplexity(ByVal password As String, ByVal userID As Long) As PasswordComplexityValidationResult
            Return ValidatePasswordComplexity(password, New UserInformation(userID, _WebManager))
        End Function
        ''' <summary>
        '''     Validates the complexity of a password
        ''' </summary>
        ''' <param name="password">The password to be validated</param>
        ''' <returns>The result of the validation</returns>
        ''' <remarks>
        '''     A password should never contain parts of the user name or other things often used by hackers.
        '''     To prevent a user from creating those lazy passwords, there is the need to validate it against the data of his user profile.
        ''' </remarks>
        Protected Function ValidatePasswordComplexity(ByVal password As String) As PasswordComplexityValidationResult
            Return ValidatePasswordComplexity(password, New String() {})
        End Function
        ''' <summary>
        '''     Validates the complexity of a password
        ''' </summary>
        ''' <param name="password">The password to be validated</param>
        ''' <param name="textExcludes">Array of strings which should never be inside of the password</param>
        ''' <returns>The result of the validation</returns>
        ''' <remarks>
        '''     A password should never contain parts of the user name or other things often used by hackers.
        '''     To prevent a user from creating those lazy passwords, there is the need to validate it against the data of his user profile.
        ''' </remarks>
        Public Overridable Function ValidatePasswordComplexity(ByVal password As String, ByVal textExcludes As String()) As PasswordComplexityValidationResult
            Const SpecChars As String = "@#$?!,.+~%=;:()_-\/*&"
            Const SpecChars_IncreasingSecurity As String = " äöüÄÖÜß§{}[]´`^°éóíúáàòìùèêîôûâ|'""µ€²³"
            Const SmallLetters As String = "abcdefghijklmnopqrstuvwxyz"
            Const LargeLetters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            Const Numbers As String = "0123456789"
            Const CharsEqualOrGreaterThanAscW As Integer = 128

            'Password length
            If Len(password) < _RequiredPasswordLength Then
                Return PasswordComplexityValidationResult.Failure_LengthMinimum
            ElseIf Len(password) > _RequiredMaximumPasswordLength Then
                Return PasswordComplexityValidationResult.Failure_LengthMaximum
            End If

            'Analyse the complexity
            Dim CollectedComplexityPoints As Integer
            Dim MyCounter As Integer
            Dim CollectedComplexityPoints_Numbers As Integer
            Dim CollectedComplexityPoints_CharsEqualOrGreaterThanAscW As Integer
            Dim CollectedComplexityPoints_LargeLetters As Integer
            Dim CollectedComplexityPoints_SmallLetters As Integer
            Dim CollectedComplexityPoints_SpecChars As Integer
            Dim CollectedComplexityPoints_IncreaseingSecurity As Integer
            Dim CollectedComplexityPoints_Other As Integer
            Dim CollectedComplexityPoints_NotAllowed_PartOfProfileInformation As Integer
            CollectedComplexityPoints_Numbers = 0
            CollectedComplexityPoints_CharsEqualOrGreaterThanAscW = 0
            CollectedComplexityPoints_LargeLetters = 0
            CollectedComplexityPoints_SmallLetters = 0
            CollectedComplexityPoints_SpecChars = 0
            CollectedComplexityPoints_IncreaseingSecurity = 0
            CollectedComplexityPoints_Other = 0
            CollectedComplexityPoints_NotAllowed_PartOfProfileInformation = 0
            CollectedComplexityPoints = 0
            For MyCounter = 1 To Len(password)
                Dim MyChar As Char = Mid(password, MyCounter, 1).Chars(0)
                If InStr(specchars, MyChar) > 0 Then
                    CollectedComplexityPoints_SpecChars = 1
                ElseIf InStr(specchars_increasingsecurity, MyChar) > 0 Then
                    CollectedComplexityPoints_IncreaseingSecurity = 1
                ElseIf InStr(SmallLetters, MyChar) > 0 Then
                    CollectedComplexityPoints_SmallLetters = 1
                ElseIf InStr(LargeLetters, MyChar) > 0 Then
                    CollectedComplexityPoints_LargeLetters = 1
                ElseIf InStr(Numbers, MyChar) > 0 Then
                    CollectedComplexityPoints_Numbers = 1
                ElseIf AscW(MyChar) >= CharsEqualOrGreaterThanAscW Then
                    CollectedComplexityPoints_CharsEqualOrGreaterThanAscW = 1
                Else
                    CollectedComplexityPoints = 1
                End If
            Next

            If Not textExcludes Is Nothing Then
                For Each MyString As String In textExcludes
                    If InStr(LCase(password), LCase(MyString)) > 0 Then
                        Return PasswordComplexityValidationResult.Failure_NotAllowed_PartOfProfileInformation
                    End If
                Next
            End If

            'Summarize the complexity points
            CollectedComplexityPoints = CollectedComplexityPoints_Numbers + _
                CollectedComplexityPoints_CharsEqualOrGreaterThanAscW + _
                CollectedComplexityPoints_LargeLetters + _
                CollectedComplexityPoints_SmallLetters + _
                CollectedComplexityPoints_SpecChars + _
                CollectedComplexityPoints_IncreaseingSecurity + CollectedComplexityPoints

            'Result of complexity points comparison
            If CollectedComplexityPoints >= _RequiredComplexityPoints Then
                Return PasswordComplexityValidationResult.Success
            Else
                Return PasswordComplexityValidationResult.Failure_HigherPasswordComplexityRequired
            End If

            'In all other cases
            Return PasswordComplexityValidationResult.Failure_Unspecified

        End Function

    End Class

End Namespace