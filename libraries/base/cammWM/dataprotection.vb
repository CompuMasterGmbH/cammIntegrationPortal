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

Namespace CompuMaster.camm.WebManager
    Public Class DataProtectionSettings


        Private LogTypeDeletionList As New ArrayList

        Const PropertyName_AnonymizeIPs As String = "AnonymizeIPsAfterDays"
        Const PropertyName_DeleteUsersAfterDays As String = "DeleteUsersAfterDays"
        Const PropertyName_DeleteMailsAfterDays As String = "DeleteMailsAfterDays"
        Const PropertyName_LogTypeDeletion As String = "LogTypeDeletionSetting"

        Private _anonymizeIPsAfterDays As Integer
        Private _deleteDeactivatedUsersAfterDays As Integer
        Private _deleteMailsAfterDays As Integer



        Public Property AnonymizeIPsAfterDays As Integer
            Get
                Return _anonymizeIPsAfterDays
            End Get
            Set(value As Integer)
                _anonymizeIPsAfterDays = value
            End Set
        End Property

        Public Property DeleteDeactivatedUsersAfterDays As Integer
            Get
                Return _deleteDeactivatedUsersAfterDays
            End Get
            Set(value As Integer)
                _deleteDeactivatedUsersAfterDays = value
            End Set
        End Property

        Public Property DeleteMailsAfterDays As Integer
            Get
                Return _deleteMailsAfterDays
            End Get
            Set(value As Integer)
                _deleteMailsAfterDays = value
            End Set
        End Property


        Private ConnectionString As String

        Public Sub New(ByVal connectionString As String)
            Me.ConnectionString = connectionString
            LoadSettings()
        End Sub

        Private Sub LoadSettings()
            Dim cmd As New SqlClient.SqlCommand("SELECT PropertyName, ValueInt FROM System_GlobalProperties WHERE PropertyName IN ( '" & PropertyName_AnonymizeIPs & "', '" & PropertyName_DeleteUsersAfterDays & "', '" & PropertyName_DeleteMailsAfterDays & "'  )")
            cmd.Connection = New SqlClient.SqlConnection(Me.ConnectionString)
            Dim resultHashTable As Hashtable = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoHashtable(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)


            Dim anonymizeIpsDays As Object = resultHashTable(PropertyName_AnonymizeIPs)
            If Not anonymizeIpsDays Is Nothing AndAlso Not IsDBNull(anonymizeIpsDays) Then
                Me.AnonymizeIPsAfterDays = CType(anonymizeIpsDays, Integer)

            End If

            Dim deleteDeactivatedDays As Object = resultHashTable(PropertyName_DeleteUsersAfterDays)
            If Not deleteDeactivatedDays Is Nothing AndAlso Not IsDBNull(deleteDeactivatedDays) Then
                Me.DeleteDeactivatedUsersAfterDays = CType(deleteDeactivatedDays, Integer)
            End If

            Dim deleteMailsDays As Object = resultHashTable(PropertyName_DeleteMailsAfterDays)
            If Not deleteMailsDays Is Nothing AndAlso Not IsDBNull(deleteMailsDays) Then
                Me.DeleteMailsAfterDays = CType(deleteMailsDays, Integer)
            End If

        End Sub

        Public Sub AddLogTypeDeletionSetting(ByVal key As String, ByVal delete As Boolean)
            LogTypeDeletionList.Add(New DictionaryEntry(key, delete))
        End Sub

        ''' <summary>
        ''' Load currently in database used flag names and their related setting value
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLogTypes() As DictionaryEntry()
            Dim Sql As String = " SELECT Type, Max(IsNull(Config.RemoveOnUserDeletion, 0))" & vbNewLine & _
                    "    FROM dbo.Log_users" & vbNewLine & _
                    "        LEFT JOIN " & vbNewLine & _
                    "            (" & vbNewLine & _
                    "                SELECT ValueNVarChar, 1 AS RemoveOnUserDeletion" & vbNewLine & _
                    "                FROM [dbo].System_GlobalProperties " & vbNewLine & _
                    "                WHERE ValueBoolean = 1 " & vbNewLine & _
                    "                    AND PropertyName='" & PropertyName_LogTypeDeletion & "'" & vbNewLine & _
                    "            ) AS Config" & vbNewLine & _
                    "            ON dbo.Log_users.Type = Config.ValueNVarChar" & vbNewLine & _
                    "    GROUP BY Type" & vbNewLine & _
                    "    ORDER BY Type"
            Dim cmd As New SqlClient.SqlCommand(Sql, New SqlClient.SqlConnection(Me.ConnectionString))

            Return CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstTwoColumnsIntoDictionaryEntryArray(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

        ''' <summary>
        ''' Cleanup all settings of already deleted users
        ''' </summary>
        Public Sub CleanupSettings()
            Dim Sql As String = "    DELETE " & vbNewLine & _
                "    FROM dbo.Log_Users " & vbNewLine & _
                "    WHERE ID_User NOT IN (SELECT ID FROM Benutzer)" & vbNewLine & _
                "        AND [Type] IN (SELECT ValueNVarChar FROM [dbo].System_GlobalProperties WHERE PropertyName = 'LogTypeDeletionSetting' And ValueBoolean = 1)"
            Dim cmd As New SqlClient.SqlCommand(Sql, New SqlClient.SqlConnection(Me.ConnectionString))
            Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Sub

        ''' <summary>
        ''' Save all settings
        ''' </summary>
        Public Sub SaveSettings()
            If True Then
                Dim sql As String = "UPDATE System_GlobalProperties SET ValueInt = @value WHERE PropertyName = @key " & vbNewLine & _
                    "IF @@ROWCOUNT = 0 " & vbNewLine & _
                    "INSERT INTO [dbo].[System_GlobalProperties] (PropertyName, ValueInt) VALUES (@key, @value)"
                Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(Me.ConnectionString))
                cmd.Parameters.Add("@value", SqlDbType.Int)
                cmd.Parameters.Add("@key", SqlDbType.NVarChar)
                'Save the several settings one by one
                Try
                    cmd.Parameters("@key").Value = PropertyName_AnonymizeIPs
                    cmd.Parameters("@value").Value = Me.AnonymizeIPsAfterDays
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)

                    cmd.Parameters("@key").Value = PropertyName_DeleteUsersAfterDays
                    cmd.Parameters("@value").Value = Me.DeleteDeactivatedUsersAfterDays
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)

                    cmd.Parameters("@key").Value = PropertyName_DeleteMailsAfterDays
                    cmd.Parameters("@value").Value = Me.DeleteMailsAfterDays
                    Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                Finally
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeCommandAndConnection(cmd)
                End Try
            End If

            If True Then
                Dim sql As String = "UPDATE [dbo].[System_GlobalProperties] SET ValueBoolean = @value WHERE ValueNVarChar = @key AND PropertyName= '" & PropertyName_LogTypeDeletion & "'  " & vbNewLine & _
                  "IF @@ROWCOUNT = 0 " & vbNewLine & _
                  "INSERT INTO [dbo].[System_GlobalProperties] (PropertyName, ValueNVarChar, ValueBoolean) VALUES ('" & PropertyName_LogTypeDeletion & "', @key, @value)"
                Dim cmd As New SqlClient.SqlCommand(sql, New SqlClient.SqlConnection(Me.ConnectionString))
                cmd.Parameters.Add("@key", SqlDbType.NVarChar)
                cmd.Parameters.Add("@value", SqlDbType.Bit)
                Try
                    For Each logtype As DictionaryEntry In LogTypeDeletionList
                        cmd.Parameters("@key").Value = logtype.Key
                        cmd.Parameters("@value").Value = logtype.Value
                        Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenConnection)
                    Next
                Finally
                    Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeCommandAndConnection(cmd)
                End Try
            End If

        End Sub

    End Class
End Namespace