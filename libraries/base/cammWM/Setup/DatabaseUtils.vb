'Copyright 2007-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Setup

    Public Class DatabaseUtils

        ''' <summary>
        ''' Lookup the database version
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance to query a database</param>
        Public Shared Function Version(ByVal webManager As IWebManager) As Version
            Return Version(webManager, True)
        End Function
        ''' <summary>
        ''' Lookup the database version
        ''' </summary>
        ''' <param name="webManager">The camm Web-Manager instance to query a database</param>
        ''' <param name="allowCaching">True allows usage of a cached value, False forces a direct query to the database</param>
        Public Shared Function Version(ByVal webManager As IWebManager, ByVal allowCaching As Boolean) As Version
            Static _System_DBVersion_Ex As Version
            Static _webManager As IWebManager 'when used in not-http contexts, there might be multiple CWM instances in parallel use (e.g. on user profile restore from a first instance to a second instance)
            Const cacheItemKey As String = "WebManager.Version.Database"
            If allowCaching Then
                If Not _System_DBVersion_Ex Is Nothing AndAlso webManager Is _webManager Then
                    Return _System_DBVersion_Ex
                End If
                If Not HttpContext.Current Is Nothing AndAlso Not HttpContext.Current.Cache(cacheItemKey) Is Nothing Then 'no check for another instance since it might not work to detect it if older a newer databases are accessed simultaneously (could only be detected if address of variable (pointer) is identified as being different - comparing database content might incorrectly indicate very same contents)
                    Return CType(HttpContext.Current.Cache(cacheItemKey), Version)
                End If
            End If
            Dim Result As Version = Nothing
            Dim MyDBConn As New SqlConnection
            Dim MyRecSet As SqlDataReader = Nothing
            Dim MyCmd As SqlCommand

            MyCmd = New SqlCommand

            'Create connection
            MyDBConn.ConnectionString = webManager.ConnectionString

            Try
                MyDBConn.Open()

                'Get parameter value and append parameter
                With MyCmd
                    .CommandText = "SELECT PropertyName, ValueInt, ValueNVarchar FROM System_GlobalProperties WHERE (PropertyName like N'DBVersion%' or Propertyname = N'DBProductName') and ValueNVarChar = N'camm WebManager'"
                    .CommandType = CommandType.Text
                End With

                'Create recordset by executing the command
                MyCmd.Connection = MyDBConn
                Try
                    MyRecSet = MyCmd.ExecuteReader()

                    Try
                        Dim IsValidCWMDatabase As Boolean = False
                        Dim MyMajorVersion As Integer
                        Dim MyMinorVersion As Integer
                        Dim MyBuildVersion As Integer
                        While MyRecSet.Read
                            Select Case Utils.Nz(MyRecSet("PropertyName"), "")
                                Case "DBVersion_Major"
                                    MyMajorVersion = CType(MyRecSet("ValueInt"), Integer)
                                Case "DBVersion_Minor"
                                    MyMinorVersion = CType(MyRecSet("ValueInt"), Integer)
                                Case "DBVersion_Build"
                                    MyBuildVersion = CType(MyRecSet("ValueInt"), Integer)
                                    IsValidCWMDatabase = True
                            End Select
                        End While
                        If IsValidCWMDatabase Then
                            Result = New Version(MyMajorVersion, MyMinorVersion, MyBuildVersion)
                        Else
                            Result = Nothing
                        End If
                    Catch ex As Exception
                        CType(webManager, WMSystem).Log.RuntimeWarning("System_DBVersion_Ex #I1 - " & ex.Message, ex.StackTrace, WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails, False, False)
                        Result = Nothing
                    End Try

                Finally
                    If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                        MyRecSet.Close()
                    End If
                End Try

                If Result Is Nothing Then

                    'Create recordset by executing the command
                    MyCmd.CommandText = "SELECT PropertyName, ValueInt, ValueNVarchar FROM System_GlobalProperties WHERE PropertyName like N'DBVersion%' or Propertyname = N'DBProductName'"
                    Try
                        MyRecSet = MyCmd.ExecuteReader()

                        Try
                            Dim IsValidCWMDatabase As Boolean = False
                            Dim MyMajorVersion As Integer
                            Dim MyMinorVersion As Integer
                            Dim MyBuildVersion As Integer
                            While MyRecSet.Read
                                Select Case Utils.Nz(MyRecSet("PropertyName"), "")
                                    Case "DBProductName"
                                        If Utils.Nz(MyRecSet("ValueNVarchar"), "") = "camm WebManager" Then
                                            IsValidCWMDatabase = True
                                        End If
                                    Case "DBVersion_Major"
                                        MyMajorVersion = CType(MyRecSet("ValueInt"), Integer)
                                    Case "DBVersion_Minor"
                                        MyMinorVersion = CType(MyRecSet("ValueInt"), Integer)
                                    Case "DBVersion_Build"
                                        MyBuildVersion = CType(MyRecSet("ValueInt"), Integer)
                                End Select
                            End While
                            If IsValidCWMDatabase Then
                                Result = New Version(MyMajorVersion, MyMinorVersion, MyBuildVersion)
                            Else
                                Result = Nothing
                            End If
                        Catch ex As Exception
                            CType(webManager, WMSystem).Log.RuntimeWarning("System_DBVersion_Ex #I2 - " & ex.Message, ex.StackTrace, WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails, False, False)
                            Result = Nothing
                        End Try
                    Finally
                        If Not MyRecSet Is Nothing AndAlso Not MyRecSet.IsClosed Then
                            MyRecSet.Close()
                        End If
                    End Try
                End If

            Catch ex As Exception
                CType(webManager, WMSystem).Log.RuntimeWarning("System_DBVersion_Ex - " & ex.Message, ex.StackTrace, WMSystem.DebugLevels.Low_WarningMessagesOnAccessError_AdditionalDetails, False, False)
                Result = Nothing
            Finally
                If Not MyCmd Is Nothing Then
                    MyCmd.Dispose()
                End If
                If Not MyDBConn Is Nothing Then
                    If MyDBConn.State <> ConnectionState.Closed Then
                        MyDBConn.Close()
                    End If
                    MyDBConn.Dispose()
                End If
            End Try

            'Store cache values
            If Not HttpContext.Current Is Nothing Then
                Utils.SetHttpCacheValue(cacheItemKey, Result, Caching.CacheItemPriority.NotRemovable)
            End If
            _System_DBVersion_Ex = Result
            _webManager = webManager

            Return Result

        End Function

    End Class

End Namespace