<%@ Page Language="vb" AutoEventWireup="false" Src="basepages.vb" Inherits="Customized.Pages.Page" %>

<%@ Assembly Src="navigation_controlfilling.vb" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.WebManager.Modules.WebEdit.Controls"
    Assembly="cammWM" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>sWcms template page demo</title>
    <link href="/sysdata/style_standard.css" type="text/css" rel="stylesheet">
    <link href="navigationstyle.css" type="text/css" rel="stylesheet">
</head>
<body>
    <p>
        <a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/templatepage/sources.src">
            View source code of the page</a></p>
    <table height="100%" width="100%" border="1">
        <form id="PageContent" runat="server">
            <tr>
                <td valign="top" colspan="3" height="50">
                    Banner
                </td>
            </tr>
            <tr valign="top">
                <td valign="top" height="400" width="30%">
                    <asp:Table runat="server" CellPadding="0" CellSpacing="0" Width="100%">
                        <asp:TableRow runat="server">
                            <asp:TableCell runat="server" ID="NavigationArea" />
                        </asp:TableRow>
                    </asp:Table>
                </td>
                <td valign="top">
                    <cammWebEdit:SmartWcms id="DemoSWcms" MarketLookupMode="Market" runat="server" SecurityObjectEditMode="@@Public"
                        Docs="" DocsReadOnly="" Media="" MediaReadOnly="" Images="" ImagesReadOnly="">WebEditor content; you must be logged on to update this text.<br>This is some predefined code in the ASPX file. This content will be overlapped by the first, released version of new content stored in the CWM database.</cammWebEdit:SmartWcms>
                </td>
                <td valign="top" height="400" width="20%">
                    Right area
                </td>
            </tr>
        </form>
    </table>
</body>
</html>
