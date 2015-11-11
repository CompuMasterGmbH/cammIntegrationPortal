<%@ Page validateRequest=false language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Specialized.StartPageForFrameSet" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="camm" Namespace="CompuMaster.camm.WebManager.Controls" Assembly="cammWM" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>

<head>
<camm:META id="META" runat="server" />
<title><%= cammWebManager.Internationalization.OfficialServerGroup_Title %></title>
<base target="frame_main">
<script Language="JavaScript"><!--
//Aktuelle Seite im _TOP-Frame öffnen, falls derzeit innerhalb eines Frames
if (window.parent.length != 0) {window.top.location = window.location; };
//-->
</script>
</head>
<FRAMESET border="0" frameSpacing="0" rows="55,*,35" frameBorder="0">
<FRAME name="frame_banner" marginWidth="0" marginHeight="0" src="<%= Response.ApplyAppPathModifier("/sysdata/frames/frame_banner.aspx") %>?Lang=<%= cammWebManager.UI.MarketID() %>&dyn=<%= Now.Millisecond.ToString %>" frameBorder="no" noResize scrolling="no" target="frame_main">
<%  If cammWebManager.UIMarketInfo.Direction = "rtl" Then%>
<FRAMESET border="0" frameSpacing="0" cols="*,200">
	<FRAME name="frame_main" marginWidth="0" marginHeight="0" src="<%= System.Web.HttpUtility.HtmlEncode(FrameContentURL) %>" frameBorder="no" noResize scrolling="yes">
	<FRAME name="frame_navigation" marginWidth="0" marginHeight="0" src="<%= Response.ApplyAppPathModifier("/sysdata/nav/index.aspx") %>?Lang=<%= cammWebManager.UI.MarketID() %>&dyn=<%= Now.Millisecond.ToString %>" frameBorder="no" noResize target="frame_main">
</FRAMESET>
<% Else %>
<FRAMESET border="0" frameSpacing="0" cols="200,*">
	<FRAME name="frame_navigation" marginWidth="0" marginHeight="0" src="<%= Response.ApplyAppPathModifier("/sysdata/nav/index.aspx") %>?Lang=<%= cammWebManager.UI.MarketID() %>&dyn=<%= Now.Millisecond.ToString %>" frameBorder="no" noResize target="frame_main">
	<FRAME name="frame_main" marginWidth="0" marginHeight="0" src="<%= System.Web.HttpUtility.HtmlEncode(FrameContentURL) %>" frameBorder="no" noResize scrolling="yes">
</FRAMESET>
<% End If%>
<FRAME name="frame_subline" marginWidth="0" marginHeight="0" src="<%= Response.ApplyAppPathModifier("/sysdata/frames/frame_sub.aspx") %>?Lang=<%= cammWebManager.UI.MarketID() %>&dyn=<%= Now.Millisecond.ToString %>" frameBorder="no" noResize scrolling="no" target="frame_main">
<NOFRAMES>
  <BODY vLink="#585888" aLink="#000080" link="#000080" leftMargin="0" topMargin="0" marginwidth="0" marginheight="0" bgcolor="#FFFFFF">
    <h3><font face="Arial">This site uses frames. Frames are not support by your
    browser software.</font></h3><h4><font face="Arial">Please install a newer
    one, e. g. <a href="http://www.microsoft.com/">Microsoft Internet Explorer</a></font></h4><p><font face="Arial">In
    case of any questions please contact your administrator!</font></p>
  </body>
</NOFRAMES>
</FRAMESET>
</html>