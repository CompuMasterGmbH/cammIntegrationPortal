<%@ Page Page MasterPageFile="/portal/MasterPage.master" Language="C#" Inherits="CompuMaster.camm.WebManager.Pages.Page" %>
<html>
<body>
<%
Response.Write ("<h1>This is a C# demo</h1>");
%>
<h3><asp:Label runat="server" id="UserWelcome" /></h3>
</body>
</html>
<script runat="server">
void Page_Load()
{
	UserWelcome.Text = "And you are " + cammWebManager.CurrentUserInfo().FullName() + "; a warm welcome to you!";
}
</script>