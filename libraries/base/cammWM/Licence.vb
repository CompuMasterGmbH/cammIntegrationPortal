Option Explicit On 
Option Strict On

Imports System.Data.SqlClient
Imports System.Web

Namespace CompuMaster.camm.WebManager

    Public Class Environment

        Private Const _ExceptionalDefaultKey As String = "ah3dkjf7JHSLIeuzw2ah94kEMAq"
        Private _WebManager As WMSystem

        Friend Sub New(ByVal webmanager As WMSystem)
            _WebManager = webmanager
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Licence details of the camm Web-Manager instance
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	11.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property Licence() As Licence
            Get
                Static _Licence As Licence
                If _Licence Is Nothing Then
                    _Licence = New Licence(_WebManager)
                End If
                Return _Licence
            End Get
        End Property

        ''' <summary>
        ''' Product name of camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks>Product name could be for example "camm Enterprise WebManager"</remarks>
        Public ReadOnly Property ProductName() As String
            Get
                Const ProductType_Enterprise As String = "camm Enterprise WebManager"
                Const ProductType_Standard As String = "camm WebManager"
                Const ProductType_SmallBusiness As String = "camm WebManager Small Business Edition"
                Const ProductType_CommunityEdition As String = "camm WebManager Community Edition"

                Dim validationDao As New Registration.InstanceValidationDao(Me._WebManager)

                Dim licenceModel As Registration.LicenceData.LicenseModel = validationDao.Load().LicenceData.Model
                Select Case licenceModel
                    Case Registration.LicenceData.LicenseModel.Enterprise
                        Return ProductType_Enterprise
                    Case Registration.LicenceData.LicenseModel.Standard
                        Return ProductType_Standard
                    Case Registration.LicenceData.LicenseModel.Light, Registration.LicenceData.LicenseModel.Professional
                        Return ProductType_SmallBusiness
                    Case Registration.LicenceData.LicenseModel.Demo, Registration.LicenceData.LicenseModel.Trial, Registration.LicenceData.LicenseModel.Community
                        Return ProductType_CommunityEdition
                    Case Else
                        'ToDo: Verify if this matches the product philosophie of the camm product line
                        Return ProductType_CommunityEdition
                End Select
            End Get
        End Property
        ''' <summary>
        ''' Licence hash code for camm Web-Manager
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public ReadOnly Property LicenceKey() As String
            Get
                Try
                    Dim cmd As New SqlClient.SqlCommand
                    cmd.CommandText = "SELECT ValueNVarChar FROM [dbo].[System_GlobalProperties] WHERE PropertyName = 'LicenceKey'"
                    cmd.CommandType = CommandType.Text
                    cmd.Connection = New SqlClient.SqlConnection(Me._WebManager.ConnectionString)

                    Dim key As Object = CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(cmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                    If IsDBNull(key) OrElse key Is Nothing Then
                        Dim newLicenceKey As String = Guid.NewGuid().ToString("N")
                        Dim insertCommand As New SqlClient.SqlCommand
                        insertCommand.CommandText = "INSERT INTO [dbo].[System_GlobalProperties] (ValueNVarChar, PropertyName) VALUES(@guid, 'LicenceKey')"
                        insertCommand.CommandType = CommandType.Text
                        insertCommand.Connection = New SqlClient.SqlConnection(Me._WebManager.ConnectionString)
                        insertCommand.Parameters.Add("@guid", SqlDbType.NVarChar).Value = newLicenceKey
                        CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(insertCommand, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                        Return newLicenceKey
                    Else
                        Return CType(key, String)
                    End If
                Catch ex As Exception
                    _WebManager.Log.ReportErrorViaEMail(ex, "Error accessing System_GlobalProperties table")
                    Return _ExceptionalDefaultKey
                End Try
            End Get
        End Property
    End Class

    Public Class Licence

        Private _WebManager As WMSystem
        Friend Sub New(ByVal cammWebManager As WMSystem)
            _WebManager = cammWebManager
        End Sub

        Private _Licencee As String
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The licencee, the name or an identifier of the organization 
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	11.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property Licencee() As String
            Get
                If _Licencee Is Nothing Then
                    _Licencee = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(_WebManager.ConnectionString), "SELECT [ValueNText] FROM [dbo].[System_GlobalProperties] WHERE [PropertyName] = 'LicenceName' AND ValueNVarChar = 'camm WebManager'", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
                    If _Licencee = Nothing Then
                        'Take the name of the first server group with a positive ID
                        Dim ServerGroupName As String
                        ServerGroupName = Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(_WebManager.ConnectionString), "SELECT TOP 1 ServerGroup FROM dbo.System_ServerGroups WHERE ID > 0", CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))
                        'Save this value in the database
                        Me.Licencee = ServerGroupName
                    End If
                End If
                Return _Licencee
            End Get
            Set(ByVal Value As String)
                Dim MyCmd As New SqlCommand( _
                    "declare @ExistingValue bit" & vbNewLine & _
                        "SELECT TOP 1 @ExistingValue  = 1" & vbNewLine & _
                        "FROM [dbo].[System_GlobalProperties] " & vbNewLine & _
                        "WHERE [PropertyName] = 'LicenceName' AND ValueNVarChar = 'camm WebManager'" & vbNewLine & _
                        "IF @ExistingValue IS NULL" & vbNewLine & _
                        "	INSERT INTO [dbo].[System_GlobalProperties]([PropertyName], [ValueNVarChar], [ValueNText])" & vbNewLine & _
                        "	VALUES ('LicenceName', 'camm WebManager', @Name)" & vbNewLine & _
                        "ELSE" & vbNewLine & _
                        "	UPDATE [dbo].[System_GlobalProperties] SET [ValueNText] = @Name WHERE [PropertyName] = 'LicenceName' AND ValueNVarChar = 'camm WebManager'", _
                    New SqlConnection(_WebManager.ConnectionString))
                MyCmd.Parameters.Add("@Name", SqlDbType.NText).Value = Value
                CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(mycmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
                _Licencee = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A cached value wether this application has been unlocked successfully
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[AdminSupport]	11.05.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Property IsLicenced() As Boolean
            Get

            End Get
            Set(ByVal Value As Boolean)

            End Set
        End Property

    End Class

End Namespace
