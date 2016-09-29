<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Pages.Page" %>

<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Navigation" id="cammWebManager" runat="server" />

<script language="VB" runat="server">

    Dim AdditionalFeaturesEnabled As Boolean
    Dim RedirectionParams As String
    Dim CurUserID As Integer

    Dim Level1Counter As Integer
    Dim Level1TitleOld As String
    Dim Level1Title As String
    Dim Level1HRef As String
    Dim Level1Target As String
    Dim Level1Tooltip As String
    Dim Level2Present As Boolean
    Dim Level2TitleOld As String
    Dim Level2Title As String
    Dim Level2HRef As String
    Dim Level2Target As String
    Dim Level2Tooltip As String
    Dim Level3Present As Boolean
    Dim Level3TitleOld As String
    Dim Level3Title As String
    Dim Level3HRef As String
    Dim Level3Target As String
    Dim Level3Tooltip As String
    Dim Level4Present As Boolean
    Dim Level4TitleOld As String
    Dim Level4Title As String
    Dim Level4HRef As String
    Dim Level4Target As String
    Dim Level4Tooltip As String
    Dim Level5Present As Boolean
    Dim Level5TitleOld As String
    Dim Level5Title As String
    Dim Level5HRef As String
    Dim Level5Target As String
    Dim Level5Tooltip As String
    Dim Level6Present As Boolean
    Dim Level6TitleOld As String
    Dim Level6Title As String
    Dim Level6HRef As String
    Dim Level6Target As String
    Dim Level6Tooltip As String
    Dim NavTitle As String
    Dim NavTitleHRef As String
    Dim NextLevel1 As Boolean
    Dim NextLevel2 As Boolean
    Dim NextLevel3 As Boolean
    Dim NextLevel4 As Boolean
    Dim NextLevel5 As Boolean
    Dim NextLevel6 As Boolean
    Dim Level1IsUpdated As Boolean
    Dim Level2IsUpdated As Boolean
    Dim Level3IsUpdated As Boolean
    Dim Level4IsUpdated As Boolean
    Dim Level5IsUpdated As Boolean
    Dim Level6IsUpdated As Boolean
    Dim AppDisabled As Boolean
    Dim JSElementLevels As String
    
    Dim IsHtmlEnCoded As Boolean
    Dim Title As String

    Dim ErrMsg As String
    Dim MoveNextDone As Boolean

    Dim CurRowPosition As Integer
    Dim MyDataTable As Data.DataTable
    Dim MyRow As Data.DataRow

    Sub ShowItemStatus(ByVal StyleDef As Integer)
        Dim Spacer As String
        If StyleDef = 1 Then
            Spacer = "<br>"
        Else
            Spacer = "<span style=""FONT-SIZE: 10px;""> </SPAN>"
        End If
        If IsNewItem() Then
            Response.Write(Spacer & "<img border=0 src=""images/button_new_29x14.gif"" lang=en title=""New"" width=29 height=14>")
        ElseIf IsUpdatedItem() Then
            Response.Write(Spacer & "<img border=0 src=""images/button_update_40x14.gif"" lang=en title=""Update"" width=40 height=14>")
        End If
    End Sub

    Function IsNewItem() As Boolean
        IsNewItem = MyRow("IsNew")
    End Function

    Function IsUpdatedItem() As Boolean
        IsUpdatedItem = MyRow("IsUpdated")
    End Function

    Sub MoveNextAndReadRecordData()
        If Not RowsCursorIsAtEOF() Then
            MyRow = GetNextRow()
            ReadRecordData()
        End If
    End Sub

    Sub ResetLevelLinkData()
        Level6HRef = Nothing
        Level6Target = Nothing
        Level6Tooltip = Nothing
        Level5HRef = Nothing
        Level5Target = Nothing
        Level5Tooltip = Nothing
        Level4HRef = Nothing
        Level4Target = Nothing
        Level4Tooltip = Nothing
        Level3HRef = Nothing
        Level3Target = Nothing
        Level3Tooltip = Nothing
        Level2HRef = Nothing
        Level2Target = Nothing
        Level2Tooltip = Nothing
        Level1HRef = Nothing
        Level1Target = Nothing
        Level1Tooltip = Nothing
    End Sub

    Sub ReadRecordData()
        If Not RowsCursorIsAtEOF() Then
            Level1TitleOld = Level1Title
            Level2TitleOld = Level2Title
            Level3TitleOld = Level3Title
            Level4TitleOld = Level4Title
            Level5TitleOld = Level5Title
            Level6TitleOld = Level6Title
            
            Title = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Title"))
                                                         
            IsHtmlEnCoded = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsHtmlEncoded"))
                                   
            If IsHtmlEnCoded = False Then
                If Title.IndexOf("\") >= 0 Then
                    Level1Title = Server.HtmlEncode(Title.Substring(0, Title.IndexOf("\")))
                Else
                    Level1Title = Server.HtmlEncode(Title)
                End If
            Else
                If Title.IndexOf("\") >= 0 Then
                    Level1Title = Title.Substring(0, Title.IndexOf("\"))
                Else
                    Level1Title = Title
                End If
            End If
            
            If Title.IndexOf("\") >= 0 Then
                Title = Title.Remove(0, Title.IndexOf("\") + 1)
            Else
                Title = ""
            End If
            
            If IsHtmlEnCoded = False Then
                If Title.IndexOf("\") >= 0 Then
                    Level2Title = Server.HtmlEncode(Title.Substring(0, Title.IndexOf("\")))
                Else
                    Level2Title = Server.HtmlEncode(Title)
                End If
            Else
                If Title.IndexOf("\") >= 0 Then
                    Level2Title = Title.Substring(0, Title.IndexOf("\"))
                Else
                    Level2Title = Title
                End If
            End If
            
            If Title.IndexOf("\") >= 0 Then
                Title = Title.Remove(0, Title.IndexOf("\") + 1)
            Else
                Title = ""
            End If
            
            If IsHtmlEnCoded = False Then
                If Title.IndexOf("\") >= 0 Then
                    Level3Title = Server.HtmlEncode(Title.Substring(0, Title.IndexOf("\")))
                Else
                    Level3Title = Server.HtmlEncode(Title)
                End If
            Else
                If Title.IndexOf("\") >= 0 Then
                    Level3Title = Title.Substring(0, Title.IndexOf("\"))
                Else
                    Level3Title = Title
                End If
            End If
            
            If Title.IndexOf("\") >= 0 Then
                Title = Title.Remove(0, Title.IndexOf("\") + 1)
            Else
                Title = ""
            End If
            
            If IsHtmlEnCoded = False Then
                If Title.IndexOf("\") >= 0 Then
                    Level4Title = Server.HtmlEncode(Title.Substring(0, Title.IndexOf("\")))
                Else
                    Level4Title = Server.HtmlEncode(Title)
                End If
            Else
                If Title.IndexOf("\") >= 0 Then
                    Level4Title = Title.Substring(0, Title.IndexOf("\"))
                Else
                    Level4Title = Title
                End If
            End If
            
            If Title.IndexOf("\") >= 0 Then
                Title = Title.Remove(0, Title.IndexOf("\") + 1)
            Else
                Title = ""
            End If
            
            If IsHtmlEnCoded = False Then
                If Title.IndexOf("\") >= 0 Then
                    Level5Title = Server.HtmlEncode(Title.Substring(0, Title.IndexOf("\")))
                Else
                    Level5Title = Server.HtmlEncode(Title)
                End If
            Else
                If Title.IndexOf("\") >= 0 Then
                    Level5Title = Title.Substring(0, Title.IndexOf("\"))
                Else
                    Level5Title = Title
                End If
            End If
            
            If Title.IndexOf("\") >= 0 Then
                Title = Title.Remove(0, Title.IndexOf("\") + 1)
            Else
                Title = ""
            End If
            
            If IsHtmlEnCoded = False Then
                If Title.IndexOf("\") >= 0 Then
                    Level6Title = Server.HtmlEncode(Title.Substring(0, Title.IndexOf("\")))
                Else
                    Level6Title = Server.HtmlEncode(Title)
                End If
            Else
                If Title.IndexOf("\") >= 0 Then
                    Level6Title = Title.Substring(0, Title.IndexOf("\"))
                Else
                    Level6Title = Title
                End If
            End If
            
            If Not Level6Title = "" Then
                ResetLevelLinkData()
                Level6HRef = MyRow("URLAutoCompleted")
                Level6Target = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Target"))
                Level6Tooltip = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(MyRow("Tooltip")))
            ElseIf Not Level5Title = "" Then
                ResetLevelLinkData()
                Level5HRef = MyRow("URLAutoCompleted")
                Level5Target = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Target"))
                Level5Tooltip = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(MyRow("Tooltip")))
            ElseIf Not Level4Title = "" Then
                ResetLevelLinkData()
                Level4HRef = MyRow("URLAutoCompleted")
                Level4Target = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Target"))
                Level4Tooltip = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(MyRow("Tooltip")))
            ElseIf Not Level3Title = "" Then
                ResetLevelLinkData()
                Level3HRef = MyRow("URLAutoCompleted")
                Level3Target = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Target"))
                Level3Tooltip = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(MyRow("Tooltip")))
            ElseIf Not Level2Title = "" Then
                ResetLevelLinkData()
                Level2HRef = MyRow("URLAutoCompleted")
                Level2Target = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Target"))
                Level2Tooltip = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(MyRow("Tooltip")))
            Else
                ResetLevelLinkData()
                Level1HRef = MyRow("URLAutoCompleted")
                Level1Target = CompuMaster.camm.WebManager.Utils.Nz(MyRow("Target"))
                Level1Tooltip = Server.HtmlEncode(CompuMaster.camm.WebManager.Utils.Nz(MyRow("Tooltip")))
            End If
            Level1IsUpdated = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsUpdated"))
            Level2IsUpdated = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsUpdated"))
            Level3IsUpdated = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsUpdated"))
            Level4IsUpdated = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsUpdated"))
            Level5IsUpdated = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsUpdated"))
            Level6IsUpdated = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsUpdated"))
            AppDisabled = CompuMaster.camm.WebManager.Utils.Nz(MyRow("IsDisabledInStandardSituations"))

            If CompuMaster.camm.WebManager.Utils.Nz(Level1Target, "") = "" Then
                Level1Target = ""
            Else
                Level1Target = " target=""" & Level1Target & """"
            End If
            If CompuMaster.camm.WebManager.Utils.Nz(Level2Target, "") = "" Then
                Level2Target = ""
            Else
                Level2Target = " target=""" & Level2Target & """"
            End If
            If CompuMaster.camm.WebManager.Utils.Nz(Level3Target, "") = "" Then
                Level3Target = ""
            Else
                Level3Target = " target=""" & Level3Target & """"
            End If
            If CompuMaster.camm.WebManager.Utils.Nz(Level4Target, "") = "" Then
                Level4Target = ""
            Else
                Level4Target = " target=""" & Level4Target & """"
            End If
            If CompuMaster.camm.WebManager.Utils.Nz(Level5Target, "") = "" Then
                Level5Target = ""
            Else
                Level5Target = " target=""" & Level5Target & """"
            End If
            If CompuMaster.camm.WebManager.Utils.Nz(Level6Target, "") = "" Then
                Level6Target = ""
            Else
                Level6Target = " target=""" & Level6Target & """"
            End If
            If Not Level2Title = "" AndAlso Not Level2HRef = "" Then
                Level2Present = True
            Else
                Level2Present = False
            End If
            If Not Level3Title = "" AndAlso Not Level3HRef = "" Then
                Level3Present = True
                Level2Present = True
            Else
                Level3Present = False
            End If
            If Not Level4Title = "" AndAlso Not Level4HRef = "" Then
                Level4Present = True
                Level3Present = True
                Level2Present = True
            Else
                Level4Present = False
            End If
            If Not Level5Title = "" AndAlso Not Level5HRef = "" Then
                Level5Present = True
                Level4Present = True
                Level3Present = True
                Level2Present = True
            Else
                Level5Present = False
            End If
            If Not Level6Title = "" AndAlso Not Level6HRef = "" Then
                Level6Present = True
                Level5Present = True
                Level4Present = True
                Level3Present = True
                Level2Present = True
            Else
                Level6Present = False
            End If
            If LCase(Level1TitleOld) = LCase(Level1Title) Then
                NextLevel1 = False
            Else
                NextLevel1 = True
            End If
            If LCase(Level2TitleOld) = LCase(Level2Title) And NextLevel1 = False Then
                NextLevel2 = False
            Else
                NextLevel2 = True
            End If
            If LCase(CompuMaster.camm.WebManager.Utils.Nz(Level3TitleOld)) = LCase(CompuMaster.camm.WebManager.Utils.Nz(Level3Title)) And NextLevel2 = False Then
                NextLevel3 = False
            Else
                NextLevel3 = True
            End If
            If LCase(CompuMaster.camm.WebManager.Utils.Nz(Level4TitleOld)) = LCase(CompuMaster.camm.WebManager.Utils.Nz(Level4Title)) And NextLevel3 = False Then
                NextLevel4 = False
            Else
                NextLevel4 = True
            End If
            If LCase(CompuMaster.camm.WebManager.Utils.Nz(Level5TitleOld)) = LCase(CompuMaster.camm.WebManager.Utils.Nz(Level5Title)) And NextLevel4 = False Then
                NextLevel5 = False
            Else
                NextLevel5 = True
            End If
            If Level1Tooltip <> "" Then
                Level1Tooltip = ""
            Else
                Level1Tooltip = " title=""" & Level1Tooltip & """"
            End If
            If Level2Tooltip <> "" Then
                Level2Tooltip = ""
            Else
                Level2Tooltip = " title=""" & Level2Tooltip & """"
            End If
            If Level3Tooltip <> "" Then
                Level3Tooltip = ""
            Else
                Level3Tooltip = " title=""" & Level3Tooltip & """"
            End If
            If Level4Tooltip <> "" Then
                Level4Tooltip = " title=""" & Level4Tooltip & """"
            End If
            If Level5Tooltip <> "" Then
                Level5Tooltip = ""
            Else
                Level5Tooltip = " title=""" & Level5Tooltip & """"
            End If
            If Level6Tooltip <> "" Then
                Level6Tooltip = ""
            Else
                Level6Tooltip = " title=""" & Level6Tooltip & """"
            End If
            'Alle DoLoop schließen
        Else
            NextLevel1 = True
            NextLevel2 = True
            NextLevel3 = True
            NextLevel4 = True
            NextLevel5 = True
            NextLevel6 = True
        End If
    End Sub

    Sub Level1Init()
        Level1Counter = Level1Counter + 1
        JSElementLevels = JSElementLevels & ", 0"

        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "    <TD valign=middle align=center width=18 bgColor=""#ffff66"" height=23><IMG" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      height=13" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      src=""")
        If Level1IsUpdated <> False Then Response.Write("images_style_3/button_red_arrow_right_bgyellow_13x13.gif"" alt=""" & cammWebManager.Internationalization.NavPointUpdatedHint) Else Response.Write("images/button_arrow_right_bgyellow_13x13.gif")
        Response.Write("""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      id=NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_ArrowImage" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "  style=""CURSOR: pointer""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      onclick=""expandit('NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_SectionPanel',1,")
        If Level1IsUpdated <> False Then Response.Write("1") Else Response.Write("0")
        Response.Write(")""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      width=13 border=0><img src=""")
        Response.Write("images/space.gif")
        Response.Write(""" width=5 height=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "    <TD valign=middle align=left width=149" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & _
                        "  style=""CURSOR: pointer""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      onclick=""expandit('NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_SectionPanel',1,")
        If Level1IsUpdated <> False Then Response.Write("1") Else Response.Write("0")
        Response.Write(")""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      background=""images/button_NavPoints_172x23.gif""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      height=23><SPAN " & _
                        "style=""CURSOR: pointer"" class=NavHeader1>")
        Response.Write(Level1Title)
        Response.Write("</SPAN></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "    <TD valign=middle align=center width=18 bgColor=""#ffff66""><IMG height=1" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      src=""images/space.gif"" width=18 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "    <" & _
                        "TD valign=middle align=left width=149><div style=""DISPLAY: none"" id=""NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_SectionPanel""><table cellpadding=0 cellspacing=0 border=0>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))

    End Sub

    'Inserted by JW on 2005-06-10, but needs an update for flags Update/New/Inactive
    Sub Level1NoSubs()
        JSElementLevels = JSElementLevels & ", 0"

        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "    <TD valign=middle align=center width=18 bgColor=""#ffff66"" height=23><IMG" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      height=13" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      src=""")
        If Level1IsUpdated <> False Then Response.Write("images_style_3/button_red_arrow_right_bgyellow_13x13.gif"" alt=""" & cammWebManager.Internationalization.NavPointUpdatedHint) Else Response.Write("images/button_arrow_right_bgyellow_13x13.gif")
        Response.Write("""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      id=NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_ArrowImage" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "  style=""CURSOR: pointer""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & " ")
        Response.Write(Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      width=13 border=0><img src=""")
        Response.Write("images/space.gif")
        Response.Write(""" width=5 height=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "    <TD valign=middle align=left width=149" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & _
                        "  style=""CURSOR: pointer""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & " ")
        Response.Write(Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      background=""images/button_NavPoints_172x23.gif""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "      height=23><A class=NavHeader1")
        Response.Write(Level1Target & Level1Tooltip)
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            href=""")
        Response.Write(Level1HRef)
        Response.Write(""">")
        Response.Write(Level1Title)
        Response.Write("</A></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))

    End Sub

    Sub Level1Close()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "</TABLE></DIV></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))
    End Sub

    Sub Level2Init()
        Level1Counter = Level1Counter + 1
        JSElementLevels = JSElementLevels & ", 1"

        Response.Write("<TR><TD vAlign=top align=left width=149 background=""images/bg_progressbar_grey_172x2.gif"" bgColor=""#c6c6c6""><TABLE cellSpacing=0 cellPadding=0 width=""100%"" border=0><TBODY><TR><TD valign=""top"" width=13><IMG height=12 src=""")
        If Level2IsUpdated <> False Then Response.Write("images_style_3/button_red_arrow_right_lev2_9x12.gif"" alt=""" & cammWebManager.Internationalization.NavPointUpdatedHint) Else Response.Write("images/button_arrow_right_lev2_9x12.gif")
        Response.Write("""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "  id=NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_ArrowImage" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                        Microsoft.VisualBasic.ChrW(9) & "  style=""CURSOR: pointer""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "  onclick=""expandit('NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_SectionPanel',2,")
        If Level2IsUpdated <> False Then Response.Write("1") Else Response.Write("0")
        Response.Write(")""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "  width=9 border=0><img src=""")
        Response.Write("images/space.gif")
        Response.Write(""" width=4 height=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                        Microsoft.VisualBasic.ChrW(9) & "<TD valign=top align=left width=149" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & " style=""CURSOR: pointer""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "  onclick=""expandit('NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_SectionPanel',2,")
        If Level2IsUpdated <> False Then Response.Write("1") Else Response.Write("0")
        Response.Write(")""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "  ><SPAN style=""CURSOR: pointer"" class=NavHeader2>")
        Response.Write(Level2Title)
        Response.Write("</SPAN></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                    "  </TBODY>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                    "</TABLE></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "  <TD vAlign=top " & _
                    "align=left width=149" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "  background=""images/bg_progressbar_grey_172x2.gif""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "  bgColor=""#c6c6c6""><div style=""DISPLAY: none"" id=""NavMenu_NavSecs__ctl")
        Response.Write(Level1Counter)
        Response.Write("_SectionPanel""><table cellpadding=0 cellspacing=0 border=0><tbody>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))
    End Sub

    Sub Level2Close()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & "          </TBODY></TABLE></div></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))
    End Sub

    Sub Level2NoSubs()
        Response.Write("<TR><TD vAlign=top align=left width=149 background=""images/bg_progressbar_grey_172x2.gif"" bgColor=""#c6c6c6""><TABLE cellSpacing=0 cellPadding=0 width=""100%"" border=0><TBODY><TR><TD valign=""top"" width=13><IMG height=10 ")
        If AppDisabled Then
            Response.Write("src=""images/button_disabled_or_underconstruction_13x10.gif"" alt=""")
            Response.Write(cammWebManager.Internationalization.NavPointTemporaryHiddenHint)
            Response.Write("""")
        Else
            Response.Write("src=""images/space.gif""")
        End If
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "            width=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top""><img src=""images/space.gif"" border=0 width=100 height=1><br><A class=NavHeader3")
        Response.Write(Level2Target & Level2Tooltip)
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            href=""")
        Response.Write(Level2HRef)
        Response.Write(""">")
        Response.Write(Level2Title)
        Response.Write("</A>")
        ShowItemStatus(0)
        Response.Write("<br><img src=""images/space.gif"" border=0 width=100 height=3>")

        If MoveNextDone = False Then
            MoveNextAndReadRecordData()
            If Not NextLevel1 And Not Level3Present Then
                Response.Write("<br><img src=""images/navline_separator.gif"" width=112 height=2 border=0><br><img src=""images/space.gif"" border=0 width=100 height=1>")
            End If
            MoveNextDone = True
        End If
        Response.Write("</TD></TR></TBODY></TABLE></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))
    End Sub

    Sub Level3Init()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "    <TD valign=""top"" width=3><IMG height=10" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "      src=""images/space.gif""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "      width=3 border=0></TD>" & _
                 Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "    <TD vAlign=top align=left width=123><img src=""images/space.gif"" border=0 width=100 height=1><DIV class=NavHeader3>")
        Response.Write(Level3Title)
        Response.Write("</DIV><img src=""images/space.gif"" border=0 width=100 height=1></TD></TR><TR><TD valign=""top"" width=13><IMG height=10 src=""images/space.gif"" width=13 border=0></TD><TD vAlign=top align=left width=136 bgColor=""#F7F7F7""><TABLE bgcolor=""#F7F7F7"" cellSpacing=0 cellPadding=0 width=""100%"" border=0><TBODY>")
    End Sub

    Sub Level3Close()
        Response.Write("</TBODY></TABLE></TD></TR>")
    End Sub

    Sub Level3NoSubs()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
             "        <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top"" width=13><img src=""images/space.gif"" width=""1"" height=""4"" border=""0""><br><IMG height=10 ")
        If AppDisabled Then
            Response.Write("src=""images/button_disabled_or_underconstruction_13x10.gif"" alt=""")
            Response.Write(cammWebManager.Internationalization.NavPointTemporaryHiddenHint)
            Response.Write("""")
        Else
            Response.Write("src=""images/space.gif""")
        End If
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "            width=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top""><table border=0 cellpadding=0 cellspacing=0><tr><td><img src=""images/space.gif"" border=0 width=100 height=1><br><A" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            class=NavHeader3")
        Response.Write(Level3Target & Level3Tooltip)
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            href=""")
        Response.Write(Level3HRef)
        Response.Write(""">")
        Response.Write(Level3Title)
        Response.Write("</A>")
        ShowItemStatus(0)
        Response.Write("<br><img src=""images/space.gif"" border=0 width=100 height=1>")

        If MoveNextDone = False Then
            MoveNextAndReadRecordData()
            If Not NextLevel2 Then
                Response.Write("<br><img src=""images/navline_separator.gif"" width=100 height=2 border=0><br><img src=""images/space.gif"" border=0 width=100 height=1>")
            End If
            MoveNextDone = True
        End If
        Response.Write("</td></tr></table></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))

    End Sub

    Sub Level4Init()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "    <TD valign=""top"" width=3><IMG height=10" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & _
                 Microsoft.VisualBasic.ChrW(9) & "      src=""images/space.gif""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                 "      width=3 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "    <TD vAlign=top align=left width=123><img src=""images/space.gif"" border=0 width=87 height=1><DIV class=NavHeader4>")
        Response.Write(Level4Title)
        Response.Write("</DIV><img src=""images/space.gif"" border=0 width=87 height=1></TD></TR><TR><TD valign=""top"" width=3><IMG height=10 src=""images/space.gif"" width=3 border=0></TD><TD vAlign=top align=left width=123><TABLE cellSpacing=0 cellPadding=0 width=""100%"" border=0><TBODY>")
    End Sub

    Sub Level4Close()
        Response.Write("</TBODY></TABLE></TD></TR>")
        Response.Write("<TR><TD background=""images/bg_progressbar_grey_172x2.gif"" colspan=2 bgcolor=""#c6c6c6""><img src=""images/space.gif"" width=100 height=2 border=0></TD></TR>")
    End Sub

    Sub Level4NoSubs()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
             "        <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top"" width=3><img src=""images/space.gif" & _
            """ width=""1"" height=""4"" border=""0""><br><IMG height=10" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            ")
        If AppDisabled Then
            Response.Write("src=""images/button_disabled_or_underconstruction_13x10.gif"" alt=""")
            Response.Write(cammWebManager.Internationalization.NavPointTemporaryHiddenHint)
            Response.Write("""")
        Else
            Response.Write("src=""images/space.gif""")
        End If
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "            width=3 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top"" bgcolor=""#F7F7F7""><font size=""2""><A" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            class=NavHeader4")
        Response.Write(Level4Target & Level4Tooltip)
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            href=""")
        Response.Write(Level4HRef)
        Response.Write(""">")
        Response.Write(Level4Title)
        Response.Write("</A>")
        ShowItemStatus(0)
        Response.Write("</font></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))

        If MoveNextDone = False Then
            MoveNextAndReadRecordData()
            Response.Write("<TR background=""images/bg_progressbar_grey_172x2.gif"" bgcolor=""#c6c6c6""><td></td>" & _
                    "" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                    Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "<TD background=""images/bg_progressbar_grey_172x2.gif"" bgcolor=""#c6c6c6"">" & _
                    "<img src=""images/space.gif"" width=100 height=2 border=0></TD></TR>")

            MoveNextDone = True
        End If
    End Sub

    Sub Level5Init()

        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "  <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "    <TD valign=""top"" width=13><IMG height=10" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "      src=""images/space.gif""" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "      width=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "    <TD vAlign=top align=left width=110><img src=""images/space.gif"" border=0 width=74 height=1><DIV class=NavHeader5>")
        Response.Write(Level5Title)
        Response.Write("</DIV><img src=""images/space.gif"" border=0 width=74 height=1></TD></TR><TR><TD valign=""top"" width=13><IMG height=10 src=""images/space.gif"" width=13 border=0></TD><TD vAlign=top align=left width=110><TABLE cellSpacing=0 cellPadding=0 width=""100%"" border=0><TBODY>")
    End Sub

    Sub Level5Close()
        Response.Write("</TBODY></TABLE></TD></TR>")
    End Sub

    Sub Level5NoSubs()
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "        <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "          <TD valign=""top"" width=13><IMG height=10" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "       " & _
                "     ")
        If AppDisabled Then
            Response.Write("src=""images/button_disabled_or_underconstruction_13x10.gif"" alt=""")
            Response.Write(cammWebManager.Internationalization.NavPointTemporaryHiddenHint)
            Response.Write("""")
        Else
            Response.Write("src=""images/space.gif""")
        End If
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                "            width=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top""><font size=""1""><A" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            class=NavHeader5")
        Response.Write(Level5Target & Level5Tooltip)
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            href=""")
        Response.Write(Level5HRef)
        Response.Write(""">")
        Response.Write(Level5Title)
        Response.Write("</A>")
        ShowItemStatus(1)
        Response.Write("</font></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))

        If MoveNextDone = False Then
            MoveNextAndReadRecordData()
            If Not NextLevel4 Then
                Response.Write("<TR><TD></TD><TD><img src=""images/space.gif"" width=100 height=1 border=0></TD></TR>")
                Response.Write("<TR><TD></TD><TD bgcolor=""#c6c6c6""><img src=""images/space.gif"" width=100 height=1 border=0></TD></TR>")
                Response.Write("<TR><TD></TD><TD><img src=""images/space.gif"" width=100 height=1 border=0></TD></TR>")
            End If
            MoveNextDone = True
        End If
    End Sub

    Sub Level6NoSubs()

        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                                 "	        <TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                                "          <TD valign=""top"" width=13><IMG height=10" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "       " & _
                                "     ")
        If AppDisabled Then
            Response.Write("src=""images/button_disabled_or_underconstruction_13x10.gif"" alt=""")
            Response.Write(cammWebManager.Internationalization.NavPointTemporaryHiddenHint)
            Response.Write("""")
        Else
            Response.Write("src=""images/space.gif""")
        End If
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & _
                    "            width=13 border=0></TD>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "          <TD valign=""top""><font size=""1""><A" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            class=NavHeader6")
        Response.Write(Level6Target & Level6Tooltip)
        Response.Write("" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10) & Microsoft.VisualBasic.ChrW(9) & Microsoft.VisualBasic.ChrW(9) & "            href=""")
        Response.Write(Level6HRef)
        Response.Write(""">")
        Response.Write(Level6Title)
        Response.Write("</A>")
        ShowItemStatus(1)
        Response.Write("</font></TD></TR>" & Microsoft.VisualBasic.ChrW(13) & Microsoft.VisualBasic.ChrW(10))
        If MoveNextDone = False Then
            MoveNextAndReadRecordData()
            If Not NextLevel5 Then
                Response.Write("<TR><TD></TD><TD><img src=""images/space.gif"" width=100 height=1 border=0></TD></TR>")
                Response.Write("<TR><TD></TD><TD bgcolor=""#c6c6c6""><img src=""images/space.gif"" width=100 height=1 border=0></TD></TR>")
                Response.Write("<TR><TD></TD><TD><img src=""images/space.gif"" width=100 height=1 border=0></TD></TR>")
            End If
            MoveNextDone = True
        End If
    End Sub

    Sub Preparations(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        'Handle the case of the preview mode
        '===================================

        If UCase(Request.QueryString("Mode")) = "PREVIEW" AndAlso _
            (cammWebManager.System_CheckForAccessAuthorization_NoRedirect("System - User Administration - Users") = CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_DBResults.AccessGranted OrElse _
            cammWebManager.System_CheckForAccessAuthorization_NoRedirect("System - User Administration - NavPreview") = CompuMaster.camm.WebManager.WMSystem.System_AccessAuthorizationChecks_DBResults.AccessGranted) Then
            cammWebManager.UIMarket(CLng(Request.QueryString("PreviewLang")))  'Sprache nur vorrübergehend setzen
            If Request.QueryString("ID") <> "" Then
                CurUserID = CLng(Request.QueryString("ID"))
            Else
                CurUserID = -1 'Ungültiger Wert --> Anonymer user
            End If
            AdditionalFeaturesEnabled = True
        Else
            AdditionalFeaturesEnabled = False
        End If

        Dim strServerIP As String
        If Request.QueryString("Server") <> "" And AdditionalFeaturesEnabled = True Then
            strServerIP = Request.QueryString("Server")
        Else
            strServerIP = Nothing
        End If

        'Get the navigation items
        '========================

        If AdditionalFeaturesEnabled = False Then 'only in standard mode
			CurUserID = cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous)
        End If

        If AdditionalFeaturesEnabled = True AndAlso Request.QueryString("GroupID") <> "" AndAlso Request.QueryString("GroupID") <> "undefined" Then
            MyDataTable = cammWebManager.System_GetGroupNavigationElements(CType(Request.QueryString("GroupID"), Long), Nothing, strServerIP)
        Else
            MyDataTable = cammWebManager.System_GetUserNavigationElements(CurUserID, 0, strServerIP)
        End If
        
        'Get additional navigation information
        '=====================================

        Dim MyServerInfo As CompuMaster.camm.WebManager.WMSystem.ServerInformation
        MyServerInfo = cammWebManager.System_GetServerInfo(strServerIP)
        NavTitle = MyServerInfo.ParentServerGroup.NavTitle

        '	if not CustomHeadHTML is nothing then CustomHeadHTML.cammWebManager = cammWebManager
        If Not CustomNavMainJScript Is Nothing Then CustomNavMainJScript.LanguageID = cammWebManager.UIMarket

    End Sub

    Function RowsCursorIsAtEOF() As Boolean
        If MyDataTable.Rows.Count = 0 OrElse MyDataTable.Rows.Count < CurRowPosition Then
            Return True
        Else
            Return False
        End If
    End Function

    Function GetNextRow() As DataRow
        CurRowPosition += 1
        If CurRowPosition > MyDataTable.Rows.Count Then
            Return Nothing
        Else
            Return MyDataTable.Rows(CurRowPosition - 1)
        End If
    End Function

</script>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>
        <%= Server.htmlencode(NavTitle) %>
        - Navigation</title>
    <%@ register tagprefix="Custom" tagname="HeadHTML" src="index_style3_headhtml.ascx" %>
    <Custom:HeadHTML id="CustomHeadHTML" runat="server" />
</head>
<body marginheight="0" marginwidth="0" leftmargin="0" bgcolor="#FFFF66" topmargin="0">
    <table cellspacing="0" cellpadding="0" width="167" bgcolor="#ffff66" border="0">
        <tbody>
            <tr>
                <td valign="middle" style="height: 1px;" height="1" align="left" width="167" colspan="2">
                    <img height="1" src="images/space.gif" width="1" border="0">
                </td>
            </tr>
            <tr>
                <td valign="middle" align="left" width="167" background="images/button_sitename_167x23.jpg"
                    bgcolor="#00446e" colspan="2" height="23">
                    <b><span style="font-size: 13px;">&nbsp;</span><a class="NavHeader0" title="Home"
                        target="_top" href="<%= cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL %>"><%= server.htmlencode(NavTitle) %></a></b>
                </td>
            </tr>
            <%
                Level1Counter = -1 'after increasing we start with 0


                If Not RowsCursorIsAtEOF() Then
                    MyRow = GetNextRow()
                    ReadRecordData() 'Read first record
                End If
                Response.Write("<!-- RootLevelInit -->")
                While Not RowsCursorIsAtEOF() 'Level1
                    MoveNextDone = False
                    If Level2Present Then
                        Response.Write("<!-- Level1Init -->")
                        Level1Init()
                        Do 'Level2
                            MoveNextDone = False
                            If Level3Present Then
                                If Not NextLevel1 Then%><!-- Level2Init --><tr>
                    <td colspan="2" bgcolor="#FFFF66">
                      <img src="images/space.gif" width="100" height="5" border="0" />
                    </td>
                </tr>
            <%  End If
                Level2Init()
                Do 'Level3
                    MoveNextDone = False
                    If Level4Present Then
                        Level3Init()
                        Do 'Level4
                            MoveNextDone = False
                            If Level5Present Then
                                Level4Init()
                                Do 'Level5
                                    MoveNextDone = False
                                    If Level6Present Then
                                        Level5Init()
                                        Do 'Level6
                                            MoveNextDone = False
            %><!-- Level6Begin --><%
                                      Level6NoSubs()
            %><!-- Level6End --><%
                                Loop Until NextLevel5 'Level6
                                Level5Close()
            %><!-- Level5Close --><%
                                  Else
            %><!-- Level5Begin --><%
                                      Level5NoSubs()
            %><!-- Level5End --><%
                                End If
                            Loop Until NextLevel4 'Level5
                            Level4Close()
            %><!-- Level4Close --><%
                                  Else
            %><!-- Level4Begin --><%
                                      Level4NoSubs()
            %><!-- Level4End --><%
                                End If
                            Loop Until NextLevel3 'Level4
                            Level3Close()
            %><!-- Level3Close --><%
                                  Else
            %><!-- Level3Begin --><%
                                      Level3NoSubs()
            %><!-- Level3End --><%
                                End If
                            Loop Until NextLevel2 'Level3
                            Level2Close()
                            If Not Level3Present Then%><tr>
                    <td colspan="2" bgcolor="#FFFF66">
                        <img src="images/space.gif" width="100" height="5" border="0">
                    </td>
                </tr>
            <%  End If
            %><!-- Level2Close --><%
                                  Else
            %><!-- Level2Begin --><%
                                      Level2NoSubs()
            %><!-- Level2End --><%
                                End If
                            Loop Until NextLevel1 'Level2
                            Level1Close()
            %><!-- Level1Close --><%
                                  Else
            %><!-- Level1Begin --><%
                                      Level1NoSubs()
            %><!-- Level1End --><%
                                End If
                                If MoveNextDone = False Then
                                    MoveNextAndReadRecordData()
                                    MoveNextDone = True
                                End If
                            End While 'Level1
                            Level1Counter = Level1Counter + 1
                            JSElementLevels = JSElementLevels & ", 0"
            %>
        </tbody>
    </table>

    <script type="text/javascript" language="JavaScript">
<!--
	var sectionCount = <%= Level1Counter %>;
	var menuClientId = "NavMenu_Menu";
	var menuId = "Menu";
	var arrowDown = new Image();
	arrowDown.src = "images/button_arrow_up_bgyellow_13x13.gif";
	var arrowDown2 = new Image();
	arrowDown2.src = "images/button_arrow_down_lev2_9x12.gif";
	var arrowDown_Updated = new Image();
	arrowDown_Updated.src = "images/button_arrow_up_bgyellow_13x13.gif";
	var arrowDown2_Updated = new Image();
	arrowDown2_Updated.src = "images/button_arrow_down_lev2_9x12.gif";
	var arrowRight = new Image();
	arrowRight.src = "images/button_arrow_right_bgyellow_13x13.gif";
	var arrowRight2 = new Image();
	arrowRight2.src = "images/button_arrow_right_lev2_9x12.gif";
	var arrowRight_Updated = new Image();
	arrowRight_Updated.src = "images_style_3/button_red_arrow_right_bgyellow_13x13.gif";
	var arrowRight2_Updated = new Image();
	arrowRight2_Updated.src = "images_style_3/button_red_arrow_right_lev2_9x12.gif";
	var arrowDownArray = [	arrowDown, arrowDown2	] ;
	var arrowRightArray = [	arrowRight, arrowRight2	] ;
	var arrowDownUpdatedArray = [	arrowDown_Updated, arrowDown2_Updated	] ;
	var arrowRightUpdatedArray = [	arrowRight_Updated, arrowRight2_Updated	] ;
	var ElementLevels = [<%= Mid(JSElementLevels,3) %>];
--></script>

    <%@ register tagprefix="Custom" tagname="NavMainJScript" src="index_style3_jscript.ascx" %>
    <Custom:NavMainJScript id="CustomNavMainJScript" runat="server" />
</body>
</html>
