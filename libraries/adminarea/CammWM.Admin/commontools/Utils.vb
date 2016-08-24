'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Administration

    Public Class Utils

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="userInfo">The userinformation object.</param>
        Public Shared Function FormatUserName(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As String
            Return FormatUserName(userInfo, False)
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="userInfo">The userinformation object.</param>
        ''' <param name="additionallyWithLoginName">True to enable additional output of login name, e.g. &quot;User Full Name (Login name)&quot;</param>
        Public Shared Function FormatUserName(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation, additionallyWithLoginName As Boolean) As String
            Return FormatUserName(userInfo.FirstName, userInfo.LastName, userInfo.LoginName, userInfo.NameAddition, userInfo.IDLong, userInfo.FullName, additionallyWithLoginName)
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Public Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object) As String
            Return FormatUserName(firstName, lastName, CType(Nothing, Object), CType(Nothing, Object), CType(Nothing, Long))
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <param name="userID">The id of the user.</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Public Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object, ByVal userID As Long) As String
            Return FormatUserName(firstName, lastName, CType(Nothing, Object), CType(Nothing, Object), userID)
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <param name="loginName">The login name of the user.</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Public Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object, ByVal loginName As Object) As String
            Return FormatUserName(firstName, lastName, loginName, Nothing, CType(Nothing, Long))
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <param name="loginName">The login name of the user.</param>
        ''' <param name="userID">The id of the user.</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Public Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object, ByVal loginName As Object, ByVal userID As Long) As String
            Return FormatUserName(firstName, lastName, loginName, Nothing, userID)
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <param name="loginName">The login name of the user.</param>
        ''' <param name="nameAddittion">The name addittion of the user.</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Public Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object, ByVal loginName As Object, ByVal nameAddittion As Object) As String
            Return FormatUserName(firstName, lastName, loginName, nameAddittion, CType(Nothing, Long))
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <param name="loginName">The login name of the user.</param>
        ''' <param name="nameAddittion">The name addittion of the user.</param>
        ''' <param name="userID">The id of the user.</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Public Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object, ByVal loginName As Object, ByVal nameAddittion As Object, ByVal userID As Long) As String
            Return FormatUserName(firstName, lastName, loginName, nameAddittion, userID, "", False)
        End Function

        ''' <summary>
        ''' Formats a username depending of which parts of the username exists.
        ''' </summary>
        ''' <param name="firstName">The first name of the user.</param>
        ''' <param name="lastName">The last name of the user.</param>
        ''' <param name="loginName">The login name of the user.</param>
        ''' <param name="nameAddittion">The name addittion of the user.</param>
        ''' <param name="userID">The id of the user.</param>
        ''' <param name="fullNameFromLog">The name information from the user log which might be the only information on a deleted user account</param>
        ''' <param name="additionallyWithLoginName">True to enable additional output of login name, e.g. &quot;User Full Name (Login name)&quot;</param>
        ''' <remarks>The parameters are mostly expected as a Datarow item. This is why there are of type Object. The function itself will do a check if theyre are NullReference, NullValue or DBnull.Value.</remarks>
        Friend Shared Function FormatUserName(ByVal firstName As Object, ByVal lastName As Object, ByVal loginName As Object, ByVal nameAddittion As Object, ByVal userID As Long, fullNameFromLog As String, additionallyWithLoginName As Boolean) As String
            Dim First As String = Trim(CompuMaster.camm.WebManager.Utils.Nz(firstName, String.Empty))
            Dim Last As String = Trim(CompuMaster.camm.WebManager.Utils.Nz(lastName, String.Empty))
            Dim Login As String = Trim(CompuMaster.camm.WebManager.Utils.Nz(loginName, String.Empty))
            Dim Addittion As String = Trim(CompuMaster.camm.WebManager.Utils.Nz(nameAddittion, String.Empty))

            Dim Result As String = Nothing

            If First <> Nothing Or Last <> Nothing Then
                'use realname
                If Addittion <> Nothing Then Result &= Addittion & " "
                If First <> Nothing AndAlso Last <> Nothing Then
                    Result &= Last & ", " & First
                ElseIf First <> Nothing Then
                    Result &= First
                ElseIf Last <> Nothing Then
                    Result &= Last
                End If
                If Login <> Nothing Then
                    Result &= " (" & Login & ")"
                End If
            ElseIf fullNameFromLog <> Nothing Then
                'use the full name from table log_users
                Result = fullNameFromLog
            ElseIf Login <> Nothing Then
                'use loginname
                Result = Login
            ElseIf userID <> Nothing Then
                'use userid
                Result = "UserID " & CStr(userID)
            Else
                Result = "[?]"
            End If

            If additionallyWithLoginName AndAlso Result <> Login Then
                Result &= " (" & Login & ")"
            End If

            Return Result
        End Function

        Friend Shared Function ConstructLinkToFlagUpdatePage(ByVal validationResult As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult, ByVal userInfo As WMSystem.UserInformation) As String
            Dim result As String = String.Empty
            If validationResult.ValidationResult <> CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.Success Then
                Dim problem As String = String.Empty
                If validationResult.ValidationResult = CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.Missing Then
                    problem = "missing"
                ElseIf validationResult.ValidationResult = CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.InvalidValue Then
                    problem = "invalid value for type"
                End If

                result = "<a target=""_blank"" href=""users_update_flag.aspx?ID=" & userInfo.IDLong.ToString() & "&Type=" & System.Web.HttpUtility.UrlEncode(validationResult.Flag) & """>" & System.Web.HttpUtility.HtmlEncode(userInfo.LoginName) & " - Flag: " & System.Web.HttpUtility.HtmlEncode(validationResult.Flag) & ", Problem: " & System.Web.HttpUtility.HtmlEncode(problem) & "</a>"
            End If
            Return result
        End Function

        Friend Shared Function FormatLinksToFlagUpdatePages(ByVal validationResults As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult(), ByVal userInfo As WMSystem.UserInformation) As String
            Dim result As String = String.Empty
            For Each flagResult As CompuMaster.camm.WebManager.FlagValidation.FlagValidationResult In validationResults
                If flagResult.ValidationResult <> CompuMaster.camm.WebManager.FlagValidation.FlagValidationResultCode.Success Then
                    result &= CompuMaster.camm.WebManager.Administration.Utils.ConstructLinkToFlagUpdatePage(flagResult, userInfo) & "<br>"
                End If
            Next
            Return result
        End Function

        ''' <summary>
        ''' Searches for a string in the supplied string and if found, removes the last occurance of it
        ''' </summary>
        ''' <param name="text">text</param>
        ''' <param name="strToKill">last occurance of this string which should be removed</param>
        ''' <remarks></remarks>
        Friend Shared Function RemoveLastOf(ByVal text As String, ByVal strToKill As String) As String
            Dim position As Integer = text.LastIndexOf(strToKill)
            If position > -1 Then
                text = text.Remove(position, strToKill.Length)
            End If
            Return text
        End Function

        ''' <summary>
        ''' Generates a token to guard against CSRF attacks
        ''' </summary>
        ''' <param name="additionalInput">string to pass to the hash function </param>
        Friend Shared Function GetCsrfToken(ByVal additionalInput As String) As String
            Dim token As String = additionalInput
            If Not HttpContext.Current Is Nothing Then
                If Not HttpContext.Current.Session Is Nothing Then
                    token &= HttpContext.Current.Session.SessionID
                End If
            End If

            Dim hmac As New System.Security.Cryptography.HMACSHA1(New Byte() {9, 12, 49, 10, 98, 20, 9, 12, 59, 12})
            Dim hash As Byte() = hmac.ComputeHash(System.Text.Encoding.Unicode.GetBytes(token))
            Return BitConverter.ToString(hash).Replace("-", "")
        End Function

#Region "ReadString/ByteDataFromUri"

        Public Shared Function ReadByteDataFromUri(ByVal uri As String) As Byte()
            Dim client As New System.Net.WebClient
            Return client.DownloadData(uri)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal uri As String, ByVal encodingName As String) As String
            Return ReadStringDataFromUri(CType(Nothing, System.Net.WebClient), uri, encodingName)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal uri As String, ByVal encodingName As String, ByVal ignoreSslValidationExceptions As Boolean) As String
            Return ReadStringDataFromUri(CType(Nothing, System.Net.WebClient), uri, encodingName, False)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal client As System.Net.WebClient, ByVal uri As String, ByVal encodingName As String) As String
            Return ReadStringDataFromUri(client, uri, encodingName, False)
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal client As System.Net.WebClient, ByVal uri As String, ByVal encodingName As String, ByVal ignoreSslValidationExceptions As Boolean) As String
            Return ReadStringDataFromUri(client, uri, encodingName, False, CType(Nothing, String))
        End Function

        Public Shared Function ReadStringDataFromUri(ByVal client As System.Net.WebClient, ByVal uri As String, ByVal encodingName As String, ByVal ignoreSslValidationExceptions As Boolean, ByVal postData As String) As String
            If client Is Nothing Then client = New System.Net.WebClient
            'https://compumaster.dyndns.biz/.....asmx without trusted certificate
#If Not NET_1_1 Then
            Dim CurrentValidationCallback As System.Net.Security.RemoteCertificateValidationCallback = System.Net.ServicePointManager.ServerCertificateValidationCallback
            Try
                If ignoreSslValidationExceptions Then System.Net.ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf OnValidationCallback)
#End If
                If encodingName <> Nothing Then
                    Dim bytes As Byte()
                    If postData Is Nothing Then
                        bytes = client.DownloadData(uri)
                    Else
                        bytes = client.UploadData(uri, System.Text.Encoding.GetEncoding(encodingName).GetBytes(postData))
                    End If
                    Return System.Text.Encoding.GetEncoding(encodingName).GetString(bytes)
                Else
#If NET_1_1 Then
                Dim encoding As System.Text.Encoding
                Try
                    Dim encName As String = client.ResponseHeaders("Content-Type")
                    If encName <> "" And encName.IndexOf("charset=") > -1 Then
                        encName = encName.Substring(encName.IndexOf("charset=") + "charset=".Length)
                        encoding = System.Text.Encoding.GetEncoding(encName)
                    Else
                        encoding = System.Text.Encoding.Default
                    End If
                Catch
                    encoding = System.Text.Encoding.Default
                End Try
                Dim bytes As Byte()
                If postData Is Nothing Then
                    bytes = client.DownloadData(uri)
                Else
                    bytes = client.UploadData(uri, encoding.GetBytes(postData))
                End If
                Return encoding.GetString(bytes)
#Else
                    If postData Is Nothing Then
                        Return client.DownloadString(uri)
                    Else
                        Return client.UploadString(uri, postData)
                    End If
#End If
                End If
#If Not NET_1_1 Then
            Finally
                System.Net.ServicePointManager.ServerCertificateValidationCallback = CurrentValidationCallback
            End Try
#End If
        End Function

#If Not NET_1_1 Then
        ''' <summary>
        ''' Suppress all SSL certification requirements - just use the webservice SSL URL
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="cert"></param>
        ''' <param name="chain"></param>
        ''' <param name="errors"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function OnValidationCallback(ByVal sender As Object, ByVal cert As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal errors As System.Net.Security.SslPolicyErrors) As Boolean
            Return True
        End Function
#End If

#End Region

    End Class

End Namespace
