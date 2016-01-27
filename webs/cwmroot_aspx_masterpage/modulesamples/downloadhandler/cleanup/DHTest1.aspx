<%@ Page Language="vb" autoeventwireup="false" Src="DHTest1.aspx.vb" Inherits="DHTest1" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server"></camm:WebManager>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>Download Handler test</title>
</head>
<body>
	<asp:Label id="Results" runat="server" />
</body>
</html>
