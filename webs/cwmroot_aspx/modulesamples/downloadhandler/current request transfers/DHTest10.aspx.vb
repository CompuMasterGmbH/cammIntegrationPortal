Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest10
    Inherits CompuMaster.camm.webmanager.pages.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'multi-file / cache / very fast since no roundtrips to any databases or camm Web-Manager is required
        If cammWebManager.DownloadHandler.IsFullyFeatured Then
            'This if statements helps you to save time when it consumes a lot of time to retrieve the data, e. g. you have to perform a database request, first
            If cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "\images\", "\style_data.zip") Then
                Response.Redirect(cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "\images\", "\style_data.zip"))
            Else
                cammWebManager.DownloadHandler.Clear
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")
                cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/passwort.gif"), "styles", "password.gif")
                Dim FileData As CompuMaster.camm.WebManager.DownloadHandler.RawDataCollectionMember
                FileData.Filename = "somebytes.bin"
                FileData.Data = New Byte() {53, 87, 157, 98, 241, 0, 154, 64}
                cammWebManager.DownloadHandler.Add(FileData, "test/binary")
                cammWebManager.DownloadHandler.ProcessDownload(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", True, Nothing, True, "style_data.zip")
            End If
        Else
            Throw New Exception("Download collections are not supported on this webserver")
        End If

    End Sub

End Class
