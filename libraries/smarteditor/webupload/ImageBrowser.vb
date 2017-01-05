Option Strict On
Option Explicit On

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor.Pages
    Public Class ImageBrowser
        Inherits FileBrowser

        Protected ltrlAltText As System.Web.UI.WebControls.Literal
        Protected ltrlImagePath As System.Web.UI.WebControls.Literal

        Protected Overrides Sub InternationalizeText()
            MyBase.InternationalizeText()

            Me.ltrlAltText.Text = My.Resources.Label_AltText
            Me.ltrlImagePath.Text = My.Resources.Label_ImagePath

        End Sub


    End Class

End Namespace