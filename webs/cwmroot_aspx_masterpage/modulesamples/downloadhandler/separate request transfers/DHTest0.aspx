<%@ Page Language="vb" autoeventwireup="false" Src="DHTest0.aspx.vb" Inherits=".DHTest0" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server"></camm:WebManager>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Download Handler test</title>
</head>
<body>
    <form id="Form1" method="post" runat="server">
        <p>
            <a id="MyHtmlAnchorPublicCache" runat="server">Download (via Public Cache)</a><br>
            <a id="MyHtmlAnchorUserSession" runat="server">Download (via user session)</a><br>
            <a id="MyHtmlAnchorWebserverSession" runat="server">Download (via webserver session)</a><br>
            <a id="MyHtmlAnchorSecurityObject" runat="server">Download (via security object)</a>
        </p>
    </form>
</body>
</html>
