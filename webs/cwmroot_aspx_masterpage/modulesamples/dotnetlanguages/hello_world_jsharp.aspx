<%@ Page Language="VJ#" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server"></camm:WebManager>
<html>
<body>
<%
this.get_Response().Write ("<h1>This is a Visual J# demo</h1>");
%>
<h3><asp:Label runat="server" id="UserWelcome" Text="lala"/></h3>
</body>
</html>
<script runat="server">
void Page_Load()
{
	this.UserWelcome.set_Text ("And you are " + cammWebManager.CurrentUserInfo().FullName() + "; a warm welcome to you!");
}
</script>