<%@ Page Language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server"></camm:WebManager>
<html>
<body>
<%
Response.Write ("<h1>This is a VB demo</h1>")
%>
<h3><asp:Label runat="server" id="UserWelcome" /></h3>
</body>
</html>
<script runat="server">
Sub Page_Load()
	UserWelcome.Text = "And you are " & cammWebManager.CurrentUserInfo.FullName() & "; a warm welcome to you!"
End Sub
</script>