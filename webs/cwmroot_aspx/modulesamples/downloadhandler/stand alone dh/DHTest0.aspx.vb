Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest0
    Inherits Page

    Private cammWebManager as New CompuMaster.camm.WebManager.WMSystem ()

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

	    'cammWebManager.ConnectionString gets loaded from web.config automatically
	    cammWebManager.CurrentServerIdentString = "" 'Nothing 'Is not set in stand alone environments
	    cammWebManager.ConnectionString = System.Configuration.ConfigurationSettings.AppSettings.Item("WebManager.ConnectionString")

        'Prepare the download file and then send it directly to the browser (without write permission to the download handler's working folder) or redirect the browser to the provided file on the webserver (with write permission)
        cammWebManager.DownloadHandler.Clear
        cammWebManager.DownloadHandler.Add(Server.MapPath("/system/images/handshake.gif"), "styles", "handshake.gif")
        cammWebManager.DownloadHandler.ProcessDownload(cammWebManager.DownloadHandler.DownloadLocations.PublicCache, "images")

    End Sub

End Class
