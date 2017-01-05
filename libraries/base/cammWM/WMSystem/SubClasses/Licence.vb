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

Option Explicit On
Option Strict On

Imports System.Data.SqlClient

Namespace CompuMaster.camm.WebManager

    Public Class Environment

        Private Const _ExceptionalDefaultKey As String = "ah3dkjf7JHSLIeuzw2ah94kEMAq"
        Private _WebManager As WMSystem

        Friend Sub New(ByVal webmanager As WMSystem)
            _WebManager = webmanager
        End Sub

        ''' <summary>
        '''     Licence details of the camm Web-Manager instance
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property Licence() As Licence
            Get
                Static _Licence As Licence
                If _Licence Is Nothing Then
                    _Licence = New Licence(_WebManager)
                End If
                Return _Licence
            End Get
        End Property


        Private _CachedProductName As String
        Public Const CacheKeyProductName As String = "CwmProductName"
        Public Property CachedProductName As String
            Get
                If HttpContext.Current Is Nothing Then
                    Return _CachedProductName
                Else
                    Return CType(HttpContext.Current.Application(CacheKeyProductName), String)
                End If
            End Get
            Set(value As String)
                If HttpContext.Current Is Nothing Then
                    _CachedProductName = value
                Else
                    HttpContext.Current.Application(CacheKeyProductName) = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Product name of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Product name could be for example "camm Enterprise WebManager"</remarks>
        Public ReadOnly Property ProductName() As String
            Get
                If CachedProductName = Nothing Then
                    Dim validationDao As New Registration.InstanceValidationDao(Me._WebManager)
                    Dim licenceModel As Registration.LicenceData.LicenseModel = validationDao.Load().LicenceData.Model

                    Dim Result As String
                    Select Case licenceModel
                        Case Registration.LicenceData.LicenseModel.Enterprise
                            Result = "camm WebManager Enterprise Edition"
                        Case Registration.LicenceData.LicenseModel.Standard
                            Result = "camm WebManager Standard Edition"
                        Case Registration.LicenceData.LicenseModel.Light
                            Result = "camm WebManager Light Edition"
                        Case Registration.LicenceData.LicenseModel.Professional
                            Result = "camm WebManager Professional Edition"
                        Case Registration.LicenceData.LicenseModel.Demo
                            Result = "camm WebManager Demo Edition"
                        Case Registration.LicenceData.LicenseModel.Trial
                            Result = "camm WebManager Trial Edition"
                        Case Registration.LicenceData.LicenseModel.Community
                            Result = "camm WebManager Community Edition"
                        Case Else
                            'TODO: Verify if this matches the product philosophie of the camm product line
                            Result = "camm WebManager Community Edition"
                    End Select
                    CachedProductName = Result
                    Return Result
                Else
                    Return CachedProductName
                End If
            End Get
        End Property

        Private _CachedLicenceKey As String
        Private Property CachedLicenceKey As String
            Get
                If HttpContext.Current Is Nothing Then
                    Return _CachedLicenceKey
                Else
                    Return CType(HttpContext.Current.Application("CwmLicenceKey"), String)
                End If
            End Get
            Set(value As String)
                If value <> _ExceptionalDefaultKey Then
                    If HttpContext.Current Is Nothing Then
                        _CachedLicenceKey = value
                    Else
                        HttpContext.Current.Application("CwmLicenceKey") = value
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Shortened licence hash code for camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>This shortened license code is not intended for licence identification, but for project discussions to clarify the instance in discussion</remarks>
        Public ReadOnly Property LicenceKeyShortened As String
            Get
                Static _Result As String
                If _Result = Nothing Then
                    _Result = Me.LicenceKey.Substring(0, 5)
                End If
                Return _Result
            End Get
        End Property

        ''' <summary>
        ''' Licence hash code for camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property LicenceKey() As String
            Get
                If CachedLicenceKey <> Nothing Then
                    Return CachedLicenceKey
                Else
                    Try
                        Dim cmd As New SqlClient.SqlCommand
                        cmd.CommandText = "SELECT ValueNVarChar FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LicenceKey'"
                        cmd.CommandType = CommandType.Text
                        cmd.Connection = New SqlClient.SqlConnection(Me._WebManager.ConnectionString)

                        Dim key As String = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), "")
                        If key = Nothing Then
                            'JIT-create a new license key and also store it to the database
                            Dim newLicenceKey As String = Guid.NewGuid().ToString("N")
                            Dim insertCommand As New SqlClient.SqlCommand
                            insertCommand.CommandText = "INSERT INTO [dbo].[System_GlobalProperties] (ValueNVarChar, PropertyName) VALUES(@guid, 'LicenceKey')"
                            insertCommand.CommandType = CommandType.Text
                            insertCommand.Connection = New SqlClient.SqlConnection(Me._WebManager.ConnectionString)
                            insertCommand.Parameters.Add("@guid", SqlDbType.NVarChar).Value = newLicenceKey
                            CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(insertCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                            key = newLicenceKey
                        End If
                        CachedLicenceKey = key
                        Return key
                    Catch ex As Exception
                        _WebManager.Log.ReportErrorByEMail(ex, "Error accessing System_GlobalProperties table")
                        Return _ExceptionalDefaultKey
                    End Try
                End If
            End Get
        End Property
    End Class

    Public Class Licence

        Private _WebManager As WMSystem
        Friend Sub New(ByVal cammWebManager As WMSystem)
            _WebManager = cammWebManager
        End Sub

        Private _CachedLicencee As String
        Private Property CachedLicencee As String
            Get
                If HttpContext.Current Is Nothing Then
                    Return _CachedLicencee
                Else
                    Return CType(HttpContext.Current.Application("CwmLicenceeName"), String)
                End If
            End Get
            Set(value As String)
                If HttpContext.Current Is Nothing Then
                    _CachedLicencee = value
                Else
                    HttpContext.Current.Application("CwmLicenceeName") = value
                End If
            End Set
        End Property
        ''' <summary>
        '''     The licencee, the name or an identifier of the organization 
        ''' </summary>
        ''' <value></value>
        Public Property Licencee() As String
            Get
                If CachedLicencee = Nothing Then
                    Dim Result As String = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(_WebManager.ConnectionString), "SELECT [ValueNText] FROM [dbo].[System_GlobalProperties] WHERE [PropertyName] = 'LicenceeName' AND ValueNVarChar = 'camm WebManager'", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
                    If Result <> Nothing Then
                        CachedLicencee = Result
                    Else
                        'JIT-create the licensee name based on first server group name
                        'Take the name of the first server group with a positive ID
                        Dim ServerGroupName As String
                        ServerGroupName = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(_WebManager.ConnectionString), "SELECT TOP 1 ServerGroup FROM dbo.System_ServerGroups WHERE ID > 0", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
                        'Save this value in the database
                        Me.Licencee = ServerGroupName
                    End If
                End If
                Return CachedLicencee
            End Get
            Set(ByVal Value As String)
                Dim MyCmd As New SqlCommand( _
                    "declare @ExistingValue bit" & vbNewLine & _
                        "SELECT TOP 1 @ExistingValue  = 1" & vbNewLine & _
                        "FROM [dbo].[System_GlobalProperties] " & vbNewLine & _
                        "WHERE [PropertyName] = 'LicenceeName' AND ValueNVarChar = 'camm WebManager'" & vbNewLine & _
                        "IF @ExistingValue IS NULL" & vbNewLine & _
                        "	INSERT INTO [dbo].[System_GlobalProperties]([PropertyName], [ValueNVarChar], [ValueNText])" & vbNewLine & _
                        "	VALUES ('LicenceeName', 'camm WebManager', @Name)" & vbNewLine & _
                        "ELSE" & vbNewLine & _
                        "	UPDATE [dbo].[System_GlobalProperties] SET [ValueNText] = @Name WHERE [PropertyName] = 'LicenceeName' AND ValueNVarChar = 'camm WebManager'", _
                    New SqlConnection(_WebManager.ConnectionString))
                MyCmd.Parameters.Add("@Name", SqlDbType.NText).Value = Value
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                CachedLicencee = Value
            End Set
        End Property
        ''' <summary>
        '''     A cached value wether this application has been unlocked successfully
        ''' </summary>
        ''' <value></value>
        Friend Property IsLicenced() As Boolean
            Get

            End Get
            Set(ByVal Value As Boolean)

            End Set
        End Property

    End Class

End Namespace
