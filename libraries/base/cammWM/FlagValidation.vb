'Copyright 2009-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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
    ''' Additional flags of users can be strongly typed
    ''' </summary>
    ''' <remarks>
    ''' Following strong typing is available:
    ''' <list type="">
    ''' <item>String</item>
    ''' <list type="">
    ''' <item>FlagName</item>
    ''' <item>FlagName{String}</item>
    ''' <item>FlagName{Text}</item>
    ''' </list>
    ''' <item>Integer 32 bit</item>
    ''' <list type="">
    ''' <item>FlagName{Integer}</item>
    ''' <item>FlagName{Int}</item>
    ''' <item>FlagName{Int32}</item>
    ''' <item>FlagName{Number}</item>
    ''' </list>
    ''' <item>Integer 64 bit</item>
    ''' <list type="">
    ''' <item>FlagName</item>
    ''' <item>FlagName{Long}</item>
    ''' <item>FlagName{Int64}</item>
    ''' </list>
    ''' <item>Boolean (store values are "1" and "0")</item>
    ''' <list type="">
    ''' <item>FlagName{Bit}</item>
    ''' </list>
    ''' <item>Boolean (store values are "true" and "false")</item>
    ''' <list type="">
    ''' <item>FlagName{Bool}</item>
    ''' <item>FlagName{Boolean}</item>
    ''' </list>
    ''' <item>DateTime (String with a date/time in international ISO format, e.g. "yyyy-MM-dd HH:mm:ss")</item>
    ''' <list type="">
    ''' <item>FlagName{Date}</item>
    ''' <item>FlagName{Date/ISO}</item>
    ''' </list>
    ''' </list>
    ''' </remarks>
    Public Class FlagValidation

        ''' <summary>
        ''' The main result of the flag validation
        ''' </summary>
        Public Enum FlagValidationResultCode
            Missing = 0
            InvalidValue = 1
            Success = 2
        End Enum

        ''' <summary>
        ''' The flag validation result information
        ''' </summary>
        Public Class FlagValidationResult
            Public Sub New(userID As Long, flag As String, validationResult As FlagValidationResultCode)
                Me.UserID = userID
                Me.Flag = flag
                Me.ValidationResult = validationResult
            End Sub
            Public Sub New(userID As Long, validationResult As FlagValidationResultCode)
                Me.UserID = userID
                Me.ValidationResult = validationResult
            End Sub

            Public UserID As Long
            Public Flag As String
            Public ValidationResult As FlagValidationResultCode
        End Class

        Private _flagComplete As String
        Private _flagName As String
        Private _flagType As String
        Private _isTypeFlag As Boolean

        Public Sub New(ByVal completeFlagDefinition As String)
            Me._flagComplete = completeFlagDefinition
            _isTypeFlag = completeFlagDefinition.IndexOf("{") > -1 AndAlso completeFlagDefinition.EndsWith("}")
            SetNameAndType()
        End Sub

        ''' <summary>
        ''' Internal assignment of FlagName and FlagType
        ''' </summary>
        Private Sub SetNameAndType()
            If _isTypeFlag Then
                Dim splittedFlag As String() = Me._flagComplete.Split("{"c)
                Me._flagName = Trim(splittedFlag(0))
                Me._flagType = Trim(splittedFlag(1).Substring(0, splittedFlag(1).Length - 1).ToUpper()) 'Remove the { at the end
            Else
                Me._flagName = Trim(Me._flagComplete)
                Me._flagType = "STRING"
            End If
        End Sub

        ''' <summary>
        ''' Is the flag a strong type
        ''' </summary>
        Public ReadOnly Property IsTypeFlag As Boolean
            Get
                Return _isTypeFlag
            End Get
        End Property

        ''' <summary>
        ''' Returns the flag as it was passed here, so for example "Birthday{DATE}"
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property FlagCompleteDefinition As String
            Get
                Return Me._flagComplete
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the flag without type information. So for "Birthday{DATE}" it would return "Birthday"
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property FlagName As String
            Get
                Return Me._flagName
            End Get
        End Property

        ''' <summary>
        ''' Returns the type of the flag. So for "Birthday{DATE}" it would return "DATE"
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property FlagType As String
            Get
                Return UCase(Me._flagType)
            End Get
        End Property

        ''' <summary>
        ''' Validates the given value against the type of this flag. 
        ''' </summary>
        ''' <param name="value">the value to be checked</param>
        Public Function IsCorrectValueForType(ByVal value As String) As Boolean
            If Not Me.IsTypeFlag Then
                Return True
            Else
                Select Case Me.FlagType
                    Case "INT", "INT32", "INTEGER", "NUMBER"
                        If IsNumeric(value) Then
#If NetFramework = "1_1" Then
                        Try
                            Integer.Parse(value)
                            Return True
                        Catch
                            Return False
                        End Try
#Else
                            Return Integer.TryParse(value, Nothing)
#End If
                        Else
                            Return False
                        End If
                    Case "LONG", "INT64"
                        If IsNumeric(value) Then
#If NetFramework = "1_1" Then
                        Try
                            Long.Parse(value)
                            Return True
                        Catch
                            Return False
                        End Try
#Else
                            Return Long.TryParse(value, Nothing)
#End If
                        Else
                            Return False
                        End If
                    Case "BIT"
                        Return value = "1" OrElse value = "0"
                    Case "BOOL", "BOOLEAN"
                        Return UCase(value) = "TRUE" OrElse UCase(value) = "FALSE"
                    Case "DATE", "DATE/ISO"
                        If CompuMaster.camm.WebManager.Utils.CountOfOccurances(value, "-") <> 2 Then
                            'ISO format always has got a date part separated by dashes ("-"): yyyy-MM-dd --> not an ISO format, here
                            Return False
                        End If
#If NetFramework = "1_1" Then
                    Try
                        DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
                        Return True
                    Catch
                        Return False
                    End Try
#Else
                        Return DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, Nothing)
#End If
                    Case "STRING", "TEXT"
                        Return True 'strings are always valid
                    Case Else
                        Throw New ArgumentOutOfRangeException("value", "Invalid FlagType: " & FlagType)
                End Select
            End If
        End Function

        ''' <summary>
        ''' Validate this flag requirement on a user information object
        ''' </summary>
        ''' <param name="userinfo"></param>
        Public Function Validate(ByVal userinfo As WMSystem.UserInformation) As FlagValidationResult
            Dim result As New FlagValidationResult(userinfo.IDLong, Me.FlagCompleteDefinition, Nothing)
            Dim userAdditionalFlags As System.Collections.Specialized.NameValueCollection = userinfo.AdditionalFlags
            If Not userAdditionalFlags Is Nothing AndAlso userAdditionalFlags.Keys.Count > 0 Then
                Dim value As String = userAdditionalFlags.Item(Me.FlagCompleteDefinition)
                If value = Nothing Then
                    result.ValidationResult = FlagValidationResultCode.Missing
                ElseIf Not Me.IsCorrectValueForType(value) Then
                    result.ValidationResult = FlagValidationResultCode.InvalidValue
                Else
                    result.ValidationResult = FlagValidationResultCode.Success
                End If
            Else
                result.ValidationResult = FlagValidationResultCode.Missing
            End If
            Return result
        End Function

        ''' <summary>
        ''' For the supplied user, check whether it has the required flags and whether the flags value are valid for the "type" of the flag
        ''' </summary>
        ''' <param name="userinfo"></param>
        ''' <param name="requiredFlags"></param>
        ''' <remarks></remarks>
        Public Shared Function ValidateRequiredFlags(ByVal userinfo As WMSystem.UserInformation, ByVal requiredFlags As String(), filterForErrorsOnly As Boolean) As FlagValidationResult()
            Dim result As New ArrayList
            If Not userinfo Is Nothing AndAlso Not requiredFlags Is Nothing Then
                For Each requiredFlag As String In requiredFlags
                    If Trim(requiredFlag) <> String.Empty Then
                        Dim flagValidation As New FlagValidation(Trim(requiredFlag))
                        Dim validationResult As FlagValidationResult = flagValidation.Validate(userinfo)
                        result.Add(validationResult)
                    End If
                Next
            End If
            If filterForErrorsOnly Then
                For MyCounter As Integer = result.Count - 1 To 0 Step -1
                    If CType(result(MyCounter), FlagValidationResult).ValidationResult = FlagValidationResultCode.Success Then
                        result.RemoveAt(MyCounter)
                    End If
                Next
            End If
            Return CType(result.ToArray(GetType(FlagValidationResult)), FlagValidationResult())
        End Function

        ''' <summary>
        ''' An exception with details on the failed flag validation results
        ''' </summary>
        Public Class RequiredFlagException
            Inherits Exception

            Public Sub New(validationResult As FlagValidationResult)
                If validationResult Is Nothing Then
                    Throw New ArgumentNullException("validationResult")
                ElseIf validationResult.ValidationResult = FlagValidationResultCode.Success Then
                    Throw New ArgumentException("Validation result ""Success"" can't lead to an exception")
                Else
                    _validationResults = New FlagValidationResult() {validationResult}
                    _MoreWarningsAvailable = True
                End If
            End Sub

            Public Sub New(validationResults As FlagValidationResult())
                If validationResults Is Nothing Then
                    Throw New ArgumentNullException("validationResults")
                Else
                    For MyCounter As Integer = 0 To validationResults.Length - 1
                        If validationResults(MyCounter).ValidationResult = FlagValidationResultCode.Success Then
                            Throw New ArgumentException("Validation result ""Success"" can't lead to an exception (" & MyCounter + 1 & ")")
                        End If
                    Next
                    _validationResults = validationResults
                End If
            End Sub

            Private _validationResults As FlagValidationResult()
            Public ReadOnly Property ValidationResults As FlagValidationResult()
                Get
                    Return _validationResults
                End Get
            End Property

            Private _MoreWarningsAvailable As Boolean
            ''' <summary>
            ''' Only 1st warning filled, more warnings might be available
            ''' </summary>
            Public ReadOnly Property MoreWarningsAvailable As Boolean
                Get
                    Return _MoreWarningsAvailable
                End Get
            End Property

            Public Overrides ReadOnly Property Message As String
                Get
                    Return "Missing or invalid flags, required flags validation failed"
                End Get
            End Property

        End Class
    End Class

End Namespace