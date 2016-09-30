Option Strict On
Option Explicit On

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor.Pages
    Public Class DocumentsBrowser
        Inherits FileBrowser

        Protected ltrlDocumentPath As System.Web.UI.WebControls.Literal
        Protected ltrlDescriptionText As System.Web.UI.WebControls.Literal
        Protected ltrlLinkText As System.Web.UI.WebControls.Literal

        Protected Overrides Sub InternationalizeText()
            MyBase.InternationalizeText()
            Me.ltrlDescriptionText.Text = My.Resources.Label_DescriptionText
            Me.ltrlDocumentPath.Text = My.Resources.Label_DocumentPath
            Me.ltrlLinkText.Text = My.Resources.Label_LinkText
        End Sub


    End Class

End Namespace