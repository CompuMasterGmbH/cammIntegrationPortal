Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest0
    Inherits CompuMaster.camm.WebManager.Pages.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Prepare the download file and then redirect the browser to the provided file on the webserver (requires write permission)
        Dim TempFile As CompuMaster.camm.WebManager.DownloadHandler.SingleFileInDownloadLocation
		'PLEASE NOTE: It's recommended to always use unique file names so that the browser cache doesn't  
		'             serve the user with a file which has been downloaded already a few minutes ago.
		'             You may like to use datetime strings (e.g. DateTime.Now.ToString("yyyyMMddHHmmss")) 
		'             in your file or folder name or use Guid.NewGuid.ToString("n") 
        TempFile = cammWebManager.DownloadHandler.GetTempFile(CompuMaster.camm.WebManager.DownloadHandler.DownloadLocations.WebServerSession, "my/sub\folder\" & Guid.NewGuid.ToString("n"), "current-datetime.txt")
        Try
            System.IO.File.WriteAllText(TempFile.PhysicalPath, "Current date/time is: " & Now.ToString("yyyy-MM-dd HH:mm:ss"))
            Dim RedirLink As String = cammWebManager.DownloadHandler.ProcessAndGetDownloadLink(TempFile)
			Response.Redirect(RedirLink)
        Catch ex As Exception
            Response.Write(ex.ToString.Replace(vbNewLine, "<br />"))
        End Try
    End Sub

End Class
