Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest1
    Inherits CompuMaster.camm.webmanager.pages.Page

    Public MyHtmlAnchor As HtmlAnchor
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'multi-file / cache / very fast since no roundtrips to any databases or camm Web-Manager is required
        If cammWebManager.DownloadHandler.IsFullyFeatured Then
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "\images\", "\style_images.zip") Then
                MyHtmlAnchor.HRef = cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "\images\", "\style_images.zip")
            Else
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/passwort.gif"), "styles", "password.gif")

                'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                MyHtmlAnchor.HRef = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "\images\", False, Nothing, Nothing, "\style_images.zip")

                'If you want to send the file with this page request, the better activate following code line 
                'cammWebManager.DownloadHandler.ProcessDownload(DownloadHandler.DownloadLocations.PublicCache, "images", True, "style_images.zip")

            End If
        Else
            Throw New Exception("Downloads in separate HTTP requests are not supported on this webserver")
        End If

    End Sub

End Class
