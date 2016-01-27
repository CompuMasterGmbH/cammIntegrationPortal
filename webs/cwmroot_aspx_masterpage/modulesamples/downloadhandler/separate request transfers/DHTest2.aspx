<%@ Page Language="vb" autoeventwireup="false" Src="DHTest2.aspx.vb" Inherits="DHTest2" %>
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
            <a id="MyHtmlAnchor" runat="server">Download</a>
        </p>
    </form>
</body>
</html>
