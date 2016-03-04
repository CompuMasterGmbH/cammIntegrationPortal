'Copyright 2009-2016 CompuMaster GmbH, http://www.compumaster.de
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

Namespace CompuMaster.camm.WebManager.Administration

    Friend Class FlagValidation

        ''' <summary>
        ''' The main result of the flag validation
        ''' </summary>
        Friend Enum FlagValidationResultCode
            Missing = 0
            InvalidValue = 1
            Success = 2
        End Enum

        ''' <summary>
        ''' The flag validation result information
        ''' </summary>
        Friend Structure FlagValidationResult
            Public Flag As String
            Public Code As FlagValidationResultCode
        End Structure

        Private _flagComplete As String
        Private _flagName As String
        Private _flagType As String
        Private _isTypeFlag As Boolean

        Public Sub New(ByVal flag As String)
            Me._flagComplete = flag
            _isTypeFlag = flag.IndexOf("{") > -1 AndAlso flag.EndsWith("}")
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
        ''' <returns></returns>
        Public ReadOnly Property IsTypeFlag As Boolean
            Get
                Return _isTypeFlag
            End Get
        End Property

        ''' <summary>
        ''' Returns the flag as it was passed here, so for example Birthday{DATE}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FlagComplete As String
            Get
                Return Me._flagComplete
            End Get
        End Property

        ''' <summary>
        ''' Returns the name of the flag without type information. So for "Birthday{DATE}" it would return "Birthday"
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
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
        ''' <returns></returns>
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
        ''' <returns></returns>
        Public Function IsCorrectValueForType(ByVal value As String) As Boolean
            If Not Me.IsTypeFlag Then
                Return True
            Else
                Select Case Me.FlagType
                    Case "INT", "INTEGER", "NUMBER"
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
                    Case "BOOL", "BOOLEAN"
                        Return UCase(value) = "TRUE" OrElse UCase(value) = "FALSE"
                    Case "DATE"
#If NetFramework = "1_1" Then
                    Try
                        DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture)
                        Return True
                    Catch
                        Return False
                    End Try
#Else
                        'Return DateTime.TryParseExact(value,   System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, Nothing)
                        Return DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, Nothing)
#End If
                    Case "DATE/ISO"
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
        ''' <returns></returns>
        Public Function Validate(ByVal userinfo As WMSystem.UserInformation) As FlagValidationResult
            Dim result As New FlagValidationResult
            Dim userAdditionalFlags As System.Collections.Specialized.NameValueCollection = userinfo.AdditionalFlags
            If Not userAdditionalFlags Is Nothing AndAlso userAdditionalFlags.Keys.Count > 0 Then
                Dim value As String = userAdditionalFlags.Item(Me.FlagComplete)
                If value Is Nothing Then
                    result.code = FlagValidationResultCode.Missing
                ElseIf Not Me.IsCorrectValueForType(value) Then
                    result.code = FlagValidationResultCode.InvalidValue
                Else
                    result.code = FlagValidationResultCode.Success
                End If
            Else
                result.code = FlagValidationResultCode.Missing
            End If

            result.flag = Me.FlagComplete
            Return result
        End Function

        ''' <summary>
        ''' For the supplied user, check whether it has the required flags and whether the flags value are valid for the "type" the flag has
        ''' </summary>
        ''' <param name="userinfo"></param>
        ''' <param name="requiredFlags"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetFlagValidationResults(ByVal userinfo As WMSystem.UserInformation, ByVal requiredFlags As String()) As FlagValidationResult()
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
            Return CType(result.ToArray(GetType(FlagValidationResult)), FlagValidationResult())
        End Function

    End Class

End Namespace