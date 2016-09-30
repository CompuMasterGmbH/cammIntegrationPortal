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
            If Me.UILanguage = 2 Then

                Me.ltrlAltText.Text = "Alt text:"
                Me.ltrlImagePath.Text = "Bild Pfad:"

            Else
                Me.ltrlAltText.Text = "Alt text:"
                Me.ltrlImagePath.Text = "Image path:"

            End If
        End Sub


    End Class

End Namespace