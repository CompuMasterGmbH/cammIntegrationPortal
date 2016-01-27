Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest0
    Inherits CompuMaster.camm.webmanager.pages.Page

    Public MyHtmlAnchorPublicCache As HtmlAnchor
    Public MyHtmlAnchorUserSession As HtmlAnchor
    Public MyHtmlAnchorWebServerSession As HtmlAnchor
    Public MyHtmlAnchorSecurityObject As HtmlAnchor
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'single-file / cache / very fast since no roundtrips to any databases or camm Web-Manager is required
        If cammWebManager.DownloadHandler.IsFullyFeatured Then
            'Public Cache
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", "styles", "handshake.gif") Then
                MyHtmlAnchorPublicCache.HRef = cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", "styles", "handshake.gif")
            Else
                'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")

                'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                MyHtmlAnchorPublicCache.HRef = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", False, Nothing)
            End If
            'User Session
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", "styles", "handshake.gif") Then
                MyHtmlAnchorUserSession.HRef = cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", "styles", "handshake.gif")
            Else
                'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")

                'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                MyHtmlAnchorUserSession.HRef = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerUserSession, "images", False, Nothing)
            End If
            'Webserver session
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.WebServerSession, "images", "styles", "handshake.gif") Then
                MyHtmlAnchorWebServerSession.HRef = cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebServerSession, "images", "styles", "handshake.gif")
            Else
                'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")

                'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                MyHtmlAnchorWebServerSession.HRef = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebServerSession, "images", False, Nothing)
            End If
            'Security Object
            cammwebmanager.securityobject = "@@anonymous"
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName, "images", "styles", "handshake.gif") Then
                MyHtmlAnchorSecurityObject.HRef = cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName, "images", "styles", "handshake.gif")
            Else
                'Prepare the download file and then send it directly to the browser (no write permission) or redirect the browser to the provided file on the webserver
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")

                'File can be delivered by another page request and that's why we exemplarily fill an HtmlAnchor here, now
                MyHtmlAnchorSecurityObject.HRef = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.WebManagerSecurityObjectName, "images", False, Nothing)
            End If
        Else
            Throw New Exception("Downloads in separate HTTP requests are not supported on this webserver")
        End If

    End Sub

End Class
