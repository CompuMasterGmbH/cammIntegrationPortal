Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest1
    Inherits CompuMaster.camm.webmanager.pages.Page

    Protected Results as Label
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim RemovedFiles as System.Collections.Specialized.StringCollection = cammWebManager.DownloadHandler.CleanUpUnregisteredFiles()
        Results.Text = "CleanUpUnregisteredFiles performed"
	For Each RemovedFile as string in RemovedFiles
		Results.Text &= "<br>" & Server.HtmlEncode(RemovedFile)
	Next

    End Sub

End Class
