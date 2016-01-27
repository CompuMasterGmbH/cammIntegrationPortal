Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest2
    Inherits CompuMaster.camm.webmanager.pages.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'single-file / security related object (no cache)
        If cammWebManager.DownloadHandler.IsFullyFeatured AndAlso cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", "styles", "handshake.gif") Then
            'This if statements helps you to save time when it consumes a lot of time to retrieve the data, e. g. you have to perform a database request, first
            Response.Redirect(cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", "styles", "handshake.gif"))
        Else
            'Prepare the download file and then send it directly to the browser (without write permission to the download handler's working folder) or redirect the browser to the provided file on the webserver (with write permission)
            cammWebManager.DownloadHandler.Clear
            cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")
            cammWebManager.DownloadHandler.ProcessDownload(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images")
        End If

    End Sub

End Class
