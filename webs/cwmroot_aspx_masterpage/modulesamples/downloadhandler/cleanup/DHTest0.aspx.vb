Imports Microsoft.VisualBasic
Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public Class DHTest0
    Inherits CompuMaster.camm.webmanager.pages.Page

    Protected Results as Label
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        cammWebManager.DownloadHandler.CleanUp()
        Results.Text = "CleanUp performed"

    End Sub

End Class
