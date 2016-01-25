Option Explicit On 
Option Strict On

Imports System.Data
Imports System.Data.SqlClient
Imports System.Web
Imports System.Web.ui
Imports System.Web.ui.WebControls

Namespace CompuMaster.camm.WebManager.Modules.Redirector.Pages

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Redirector.Pages.Redirector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Redirect to an URL which is referenced by the R-parameter in the querystring
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[wezel]	15.07.2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public Class Redirector
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Protected Overridable Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            'RedirectID
            Dim UseTemporaryRedirection As Boolean = False
            Dim MyRedirect2Addr As String
            Dim MyRedirect2ID As Long
            If Request.QueryString("R") <> Nothing Then
                Try
                    MyRedirect2ID = CType(Request.QueryString("R"), Long)
                Catch
                    MyRedirect2ID = 0 'Unknown ID leads to NULL result
                    UseTemporaryRedirection = True
                End Try
            Else
                MyRedirect2ID = 0 'Unknown ID leads to NULL result
                UseTemporaryRedirection = True
            End If
            Try
                MyRedirect2Addr = Redirector_GetLink(MyRedirect2ID)
                If MyRedirect2Addr = Nothing Then
                    MyRedirect2Addr = "/"
                    UseTemporaryRedirection = True
                End If
            Catch ex As Exception
                ReportException(ex)
                UseTemporaryRedirection = True
                MyRedirect2Addr = "/"
            End Try
            Dim RedirQueryString As String = Utils.QueryStringWithoutSpecifiedParameters(New String() {"R"})
            If RedirQueryString <> "" Then
                If InStr(MyRedirect2Addr, "?") <> 0 Then
                    'Attach to URL with already present query string
                    MyRedirect2Addr &= "&" & RedirQueryString
                Else
                    'Attach to URL as new query string
                    MyRedirect2Addr &= "?" & RedirQueryString
                End If
            End If
            If UseTemporaryRedirection = False AndAlso Me.cammWebManager.DebugLevel = 0 Then
                Me.RedirectPermanently(MyRedirect2Addr) '301 permanent redirection
            Else
                cammWebManager.RedirectTo(MyRedirect2Addr) '301 temporary redirection
            End If
        End Sub

        ''' <summary>
        ''' Report exception by e-mail since there might be critical website errors if redirections doesn't work properly any more
        ''' </summary>
        ''' <param name="exception"></param>
        ''' <remarks></remarks>
        Protected Overridable Sub ReportException(ByVal exception As Exception)
            Try
                cammWebManager.Log.ReportErrorViaEMail(exception, "Redirection link invalid")
            Catch
            End Try
        End Sub

        ''' <summary>
        ''' Look up the redirection link and (usually) log/count the access
        ''' </summary>
        ''' <param name="RedirectID"></param>
        ''' <returns>The URL of the requested redirection link</returns>
        ''' <remarks></remarks>
        Protected Overridable Function Redirector_GetLink(ByVal RedirectID As Long) As String
            Dim MyCmd As New SqlCommand
            MyCmd.Connection = New SqlConnection(cammWebManager.ConnectionString)

            If Utils.IsRequestFromCrawlerOrPotentialMachineAgent(Me.Request) = False Then
                'User agent in meaning of a real user - shall be logged and counted
                MyCmd.CommandText = "Redirects_LogAndGetURL"
                MyCmd.CommandType = CommandType.StoredProcedure
                MyCmd.Parameters.Add("@IDRedirector", SqlDbType.Int).Value = RedirectID
            Else
                'Crawler/machine agent - shall not be logged as really executed user redirection
                MyCmd.CommandText = "SELECT RedirectTo FROM dbo.Redirects_ToAddr WHERE ID = @IDRedirector"
                MyCmd.CommandType = CommandType.Text
                MyCmd.Parameters.Add("@IDRedirector", SqlDbType.Int).Value = RedirectID
            End If

            Return Utils.Nz(Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(MyCmd, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), CType(Nothing, String))

        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Redirector.Pages.Data
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Data handling for the module of redirections
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	11.01.2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Data

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Read the number of redirections
        ''' </summary>
        ''' <param name="id">The redirection ID as visible in the administration area</param>
        ''' <param name="webmanager">An instance of camm Web-Manager</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	11.01.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function NumberOfRedirections(ByVal id As Integer, ByVal webmanager As CompuMaster.camm.WebManager.IWebManager) As Integer
            Return Utils.Nz(CompuMaster.camm.WebManager.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New SqlConnection(webmanager.ConnectionString), _
                                    "Select numberofredirections from Redirects_ToAddr where id = " & id.ToString, CommandType.Text, Nothing, Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection), 0)
        End Function

    End Class

End Namespace