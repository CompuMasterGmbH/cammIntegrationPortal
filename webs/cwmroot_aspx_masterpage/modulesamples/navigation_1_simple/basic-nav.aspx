<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.Page" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script runat="server">
sub OnPageLoad (sender as object, e as eventargs) Handles MyBase.Load

Dim NavItems as System.Data.DataTable = cammWebManager.System_GetUserNavigationElements (-1) 'Anonymous user items only

'Provide a very simple style of a navigation
Dim BasicNav as new System.Text.StringBuilder()
For MyCounter as Integer = 0 to navitems.rows.count-1
	BasicNav.Append ("<a href=""")
	BasicNav.Append (NavItems.Rows(MyCounter)("UrlAutoCompleted"))
	BasicNav.Append (""">")
	If NavItems.Rows(MyCounter)("IsHtmlEncoded") = True Then
		BasicNav.Append (NavItems.Rows(MyCounter)("Title"))
	Else
		BasicNav.Append (Server.HtmlEncode(NavItems.Rows(MyCounter)("Title")))
	End If
	BasicNav.Append ("</a><br>")
next
SimpleNavItems.Text = BasicNav.ToString()

'Render full navigation data
FullDataOnNavItems.DataSource = NavItems
FullDataOnNavItems.DataBind

end sub
</script>
<html>
<head><title>Navigation examples</title></head>
<body style="font-family: Arial">

    <h1>
        Basic Navigation Generation
    </h1>

<p><A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/navigation_1_simple/basic-nav.src">View source code of the page</A></p>


<h3>Simple navigation</h3>
<asp:Literal runat="server" id="SimpleNavItems" />

<h3>Full available data on navigation items</h3>
<asp:datagrid runat="server" id="FullDataOnNavItems" />

</body>
</html>