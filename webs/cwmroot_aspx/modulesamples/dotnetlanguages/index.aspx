<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
	<title>.NET Languages Demo</title>
	<link href="/sysdata/style_standard.css" type="text/css" rel="stylesheet">
</head>
<body>
<font face="Arial" size="2">
<form id="PageContent" runat="server">
<H1>.NET Languages Demo</H1>
<P>This example shows you the difference between C#, VJ#, JScript and VB. All Pages do exactly the same but in different programming languages.</P>
<UL>
<LI>Shows the C# demo<br>
<A href="hello_world_csharp.aspx">Run demo</A> | <A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/dotnetlanguages/hello_world_csharp.src">View source code</A></LI>
<LI>Shows the J# demo (requires an installed Visual J# redistributable package to use this language)<br>
<A href="hello_world_jsharp.aspx">Run demo</A> | <A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/dotnetlanguages/hello_world_jsharp.src">View source code</A></LI>
<LI>Shows the JScript demo<br>
<A href="hello_world_jscript.aspx">Run demo</A> | <A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/dotnetlanguages/hello_world_jscript.src">View source code</A></LI>
<LI>Shows the VB demo<br>
<A href="hello_world_vb.aspx">Run demo</A> | <A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/dotnetlanguages/hello_world_vb.src">View source code</A></LI>
</UL>
</font>
</form>
</body>
</html>