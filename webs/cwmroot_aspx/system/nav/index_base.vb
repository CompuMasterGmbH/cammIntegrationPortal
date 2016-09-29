Imports System.Collections
Imports System.Data.SqlClient
Imports Microsoft.Web.UI.WebControls

    Dim AdditionalFeaturesEnabled As Boolean
    Dim RedirectionParams As String
    Dim CurUserID As Integer

    Dim MyDataTable As Data.DataTable

    Sub Preparations (ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        'Handle the case of the preview mode
        '===================================

        If UCase(Request.QueryString("Mode")) = "PREVIEW" Then
            cammWebManager.System_CheckForAccessAuthorization("System - User Administration - Users")
            cammWebManager.System_CheckForAccessAuthorization("System - User Administration - NavPreview")
            cammWebManager.UIMarket(CLng(Request.QueryString("Lang")))  'Sprache nur vorrübergehend setzen
            If Request.QueryString("ID") <> "" Then
                CurUserID = CLng(Request.QueryString("ID"))
            Else
                CurUserID = -1 'Ungültiger Wert --> Anonymer user
            End If
            AdditionalFeaturesEnabled = True
        Else
            AdditionalFeaturesEnabled = False
        End If

        Dim strServerIP As String
        If Request.QueryString("Server") <> "" And AdditionalFeaturesEnabled = True Then
            strServerIP = Request.QueryString("Server")
        Else
            strServerIP = Nothing
        End If

        'Get the navigation items
        '========================

        If AdditionalFeaturesEnabled = False Then 'only in standard mode
            CurUserID = cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous)
        End If

        MyDataTable = cammWebManager.System_GetNavItems(CurUserID, Nothing, strServerIP)


        'Get additional navigation information
        '=====================================

        Dim MyServerInfo As CompuMaster.camm.WebManager.WMSystem.ServerInformation
        MyServerInfo = cammWebManager.System_GetServerInfo(strServerIP)

    End Sub
