Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest11
    Inherits CompuMaster.camm.webmanager.pages.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'single-file / cache / very fast since no roundtrips to any databases or camm Web-Manager is required
        If cammWebManager.DownloadHandler.IsFullyFeatured AndAlso cammWebManager.DownloadHandler.DownloadFileAlreadyExists(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images", "test\binary", "somebytes.bin") Then
            Response.Redirect(cammWebManager.DownloadHandler.CreateDownloadLink(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "\images\", "\test\binary\somebytes.bin"))
        Else
            'Prepare the download file and then send it directly to the browser (without write permission to the download handler's working folder) or redirect the browser to the provided file on the webserver (with write permission)
            Dim FileData As CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile
            FileData.Filename = "somebytes.bin"
            FileData.MimeType = "application/octet-stream"
            FileData.Data = New Byte() {53, 87, 157, 98, 241, 0, 154, 64}
            cammWebManager.DownloadHandler.Clear
            cammWebManager.DownloadHandler.Add(FileData, "test/binary")
            cammWebManager.DownloadHandler.ProcessDownload(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images")
        End If

    End Sub

End Class
