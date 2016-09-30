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
            If Me.UILanguage = 2 Then
                Me.ltrlDescriptionText.Text = "Link Beschreibung:"
                Me.ltrlDocumentPath.Text = "Dokumenten Pfad:"
                Me.ltrlLinkText.Text = "Link Text:"


            Else
                Me.ltrlDescriptionText.Text = "Link Description:"
                Me.ltrlDocumentPath.Text = "Documents Path:"
                Me.ltrlLinkText.Text = "Link Text:"

            End If
        End Sub


    End Class

End Namespace