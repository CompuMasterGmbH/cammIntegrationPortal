Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports Microsoft.VisualBasic

Namespace Customized.Pages

    Public Class Page
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Protected NavigationArea As TableCell

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Load the navigation data
            Dim navigationData As System.Data.DataTable = CompuMaster.camm.WebManager.Navigation.CreateEmptyDataTable
            Dim currentUserID As Long = cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous)
            navigationData = cammWebManager.System_GetUserNavigationElements(currentUserID)

            'Create a treeview control and add it to the page
            Me.NavigationArea.Controls.Add(New LiteralControl("<h5>WebManager Navigation</h5>"))
            Dim mycTree As New ComponentArt.Web.UI.TreeView
            Me.NavigationArea.Controls.Add(mycTree)

            'Fill the treeview and define the styles
            Dim cTree As New Customization.Navigation.ComponentArtTreeView
            cTree.Fill(navigationData, mycTree)
            mycTree.DragAndDropEnabled = "true"
            mycTree.NodeEditingEnabled = "false"
            mycTree.KeyboardEnabled = "false"
            mycTree.CssClass = "TreeView"
            mycTree.NodeCssClass = "TreeNode"
            mycTree.HoverNodeCssClass = "HoverTreeNode"
            mycTree.SelectedNodeCssClass = "SelectedTreeNode"
            mycTree.NodeEditCssClass = "NodeEdit"
            mycTree.DefaultImageWidth = "16"
            mycTree.DefaultImageHeight = "16"
            mycTree.ExpandCollapseImageWidth = "17"
            mycTree.ExpandCollapseImageHeight = "15"
            mycTree.NodeLabelPadding = "3"
            mycTree.ItemSpacing = "2"
            mycTree.NodeIndent = "17"
            mycTree.ClientScriptLocation = "/system/nav/componentart_webui_client/"
            mycTree.ParentNodeImageUrl = mycTree.ClientScriptLocation & "images/folder.gif"
            mycTree.LeafNodeImageUrl = mycTree.ClientScriptLocation & "images/outbox.gif"
            mycTree.ExpandImageUrl = mycTree.ClientScriptLocation & "images/col.gif"
            mycTree.CollapseImageUrl = mycTree.ClientScriptLocation & "images/exp.gif"
            mycTree.EnableViewState = "false"


        End Sub
    End Class

End Namespace