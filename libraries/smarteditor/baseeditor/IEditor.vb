Option Explicit On
Option Strict On

Imports System.Web

Namespace CompuMaster.camm.SmartWebEditor

    Public Interface IEditor

        Property Html As String

        Property Editable As Boolean

        Property EnableViewState As Boolean

        Property Visible As Boolean

        ReadOnly Property ClientID As String

        Property CssWidth As String
        Property CssHeight As String

        Property TextareaRows As Integer
        Property TextareaColumns As Integer


    End Interface

End Namespace