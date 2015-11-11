Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest2
    Inherits CompuMaster.camm.webmanager.pages.Page

    Public MyHtmlAnchor As HtmlAnchor
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'single-file / security related object (no cache)
        If cammWebManager.DownloadHandler.IsFullyFeatured Then
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", "styles", "handshake.gif") Then
                MyHtmlAnchor.HRef = cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", "styles", "handshake.gif")
            Else
                'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")
                
                'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                MyHtmlAnchor.HRef = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images")
            End If
        Else
            Throw New Exception("Downloads in separate HTTP requests are not supported on this webserver")
        End If

    End Sub

End Class
