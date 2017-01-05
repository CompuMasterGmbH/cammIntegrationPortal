'Copyright 2006-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Friend Class InformationClassTools
        ''' <summary>
        '''     Are there no invalid entries because of the content in the unique fields?
        ''' </summary>
        ''' <param name="userInfo">A user information object</param>
        ''' <returns>True if no conflicts, otherwise false</returns>
        Public Shared Function IsValidContentOfUniqueFields(ByVal userInfo As CompuMaster.camm.WebManager.IUserInformation) As Boolean

            Dim MyConn As New SqlConnection(userInfo.WebManager.ConnectionString)
            Dim Result As Boolean = True

            Try
                Dim ReturnValue As Integer

                'LoginName
                Dim CmdLoginName As New SqlCommand("SELECT COUNT(*) FROM dbo.Benutzer WHERE LoginName = @LoginName AND NOT ID = @ID", MyConn)
                CmdLoginName.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                CmdLoginName.Parameters.Add("@LoginName", SqlDbType.NVarChar).Value = userInfo.LoginName
                ReturnValue = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(CmdLoginName, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection), 0)
                CmdLoginName.Dispose()
                If ReturnValue <> 0 Then
                    Result = False
                    Exit Try
                End If

                'External account
                If userInfo.ExternalAccount <> Nothing Then
                    Dim CmdExternalAccount As New SqlCommand("SELECT COUNT(*) FROM dbo.Log_Users inner join dbo.Benutzer on dbo.Benutzer.ID = dbo.Log_Users.ID_User WHERE Type = 'ExternalAccount' AND Value = @Value AND NOT ID_User = @ID", MyConn)
                    CmdExternalAccount.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                    CmdExternalAccount.Parameters.Add("@Value", SqlDbType.NVarChar).Value = userInfo.ExternalAccount
                    ReturnValue = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(CmdExternalAccount, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection), 0)
                    CmdExternalAccount.Dispose()
                    If ReturnValue <> 0 Then
                        Result = False
                        Exit Try
                    End If
                End If

            Finally
                'Release resources
                If Not MyConn Is Nothing Then
                    If Not MyConn.State = ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            Return Result

        End Function

        ''' <summary>
        ''' Which users are already present using the values of unique fields of the given userInfo 
        ''' </summary>
        ''' <param name="userInfo">A user information object with values in unique fields which shall be evaluated</param>
        ''' <returns>A list of existing user IDs conflicting with the given values as unique keys</returns>
        ''' <remarks></remarks>
        Public Shared Function ExistingUsersConflictingWithContentOfUniqueFields(ByVal userInfo As CompuMaster.camm.WebManager.IUserInformation) As UserInfoConflictingUniqueKeysKeyValues()

            Dim MyConn As New SqlConnection(userInfo.WebManager.ConnectionString)
            Dim Result As New ArrayList

            Try
                Dim ReturnValues As ArrayList

                'LoginName
                Dim CmdLoginName As New SqlCommand("SELECT ID FROM dbo.Benutzer WHERE LoginName = @LoginName AND NOT ID = @ID", MyConn)
                CmdLoginName.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                CmdLoginName.Parameters.Add("@LoginName", SqlDbType.NVarChar).Value = userInfo.LoginName
                ReturnValues = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(CmdLoginName, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                CmdLoginName.Dispose()
                If ReturnValues.Count <> 0 Then
                    Result.Add(New UserInfoConflictingUniqueKeysKeyValues("LoginName", CType(ReturnValues.ToArray(GetType(Long)), Long()), userInfo.LoginName))
                End If

                'External account
                If userInfo.ExternalAccount <> Nothing Then
                    Dim CmdExternalAccount As New SqlCommand("SELECT dbo.Benutzer.ID FROM dbo.Log_Users inner join dbo.Benutzer on dbo.Benutzer.ID = dbo.Log_Users.ID_User WHERE Type = 'ExternalAccount' AND Value = @Value AND NOT ID_User = @ID", MyConn)
                    CmdExternalAccount.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID
                    CmdExternalAccount.Parameters.Add("@Value", SqlDbType.NVarChar).Value = userInfo.ExternalAccount
                    ReturnValues = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoArrayList(CmdExternalAccount, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                    CmdExternalAccount.Dispose()
                    If ReturnValues.Count <> 0 Then
                        Result.Add(New UserInfoConflictingUniqueKeysKeyValues("ExternalAccount", CType(ReturnValues.ToArray(GetType(Long)), Long()), userInfo.ExternalAccount))
                    End If
                End If

            Finally
                'Release resources
                If Not MyConn Is Nothing Then
                    If Not MyConn.State = ConnectionState.Closed Then
                        MyConn.Close()
                    End If
                    MyConn.Dispose()
                End If
            End Try

            Return CType(Result.ToArray(GetType(UserInfoConflictingUniqueKeysKeyValues)), UserInfoConflictingUniqueKeysKeyValues())

        End Function

        'Private Shared Sub CombineArrayListWithoutDbNullsOrNulls(baseList As ArrayList, addList As ArrayList)
        '    If baseList Is Nothing Then Throw New ArgumentNullException("baseList")
        '    If addList Is Nothing Then Return 'just do nothing
        '    For Each item As Object In addList
        '        If item Is Nothing Then
        '            'ignore
        '        ElseIf IsDBNull(item) Then
        '            'ignore
        '        Else
        '            If baseList.Contains(item) = False Then baseList.Add(item)
        '        End If
        '    Next
        'End Sub
    End Class

End Namespace