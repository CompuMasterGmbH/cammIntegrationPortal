Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest0
    Inherits CompuMaster.camm.webmanager.pages.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'single-file / cache / very fast since no roundtrips to any databases or camm Web-Manager is required
        If cammWebManager.DownloadHandler.IsFullyFeatured AndAlso cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", "styles", "handshake.gif") Then
            Response.Redirect(cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", "styles", "handshake.gif"))
        Else
            'Prepare the download file and then send it directly to the browser (without write permission to the download handler's working folder) or redirect the browser to the provided file on the webserver (with write permission)
            cammWebManager.DownloadHandler.Clear
            cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")
            cammWebManager.DownloadHandler.ProcessDownload(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images")
        End If

    End Sub

End Class
