<%@ Page Language="VB" %>
<html>
<head>
</head>
<body>
    <h1>
        <font face="Arial">camm Web-Manager - module samples </font> 
    </h1>
	<ul>
		<li><font face="Arial"><a href="smartwcms/index.aspx">Smart web content management system (WCMS)</a><br>
		Additionally to the features of WebEditor, the Smart WCMS provides a version history and saves the data to the database instead of changing the files themselves. This enables you to provide different texts for different languages or markets.</font></li>
		<li><font face="Arial"><a href="webcontrols/index.aspx">camm WebControls</a><br>
		Benefit from the resizer controls which allows to rotate, create frames and more. Or use the other useful controls. You never want to miss them!<br>Please note: the very first access to this page will consume lot of time because there are dozens of images which have to be resized. All other requests will perform absolutely fast.</font></li>
		<li><font face="Arial"><a href="navigation_1_simple/index.aspx">Dynamic generation of navigation</a><br>
		A basic feature is the navigation which is language/market dependent</font></li>		
		<li><font face="Arial"><a href="navigation_2_advanced_componentart/default.aspx">Advanced navigation creation</a><br>
		Customize your camm Web-Manager navigation with ComponentArt or any other 3rd party controls</font></li>		
<% If System.IO.File.Exists(Server.MapPath("explorer/config.xml")) = True Then%>
		<li><font face="Arial"><a href="explorer/index.aspx">camm Web-Explorer</a><br>
		Provides an interface for up-/downloading files; <A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/explorer/sources-w-config.src">view source code of the page</A></font></li>		
<% Else %>
		<li><font face="Arial">camm Web-Explorer<br>
		Provides an interface for up-/downloading files (requires you to manually create the file config.xml, first; a config.sample.xml is available). <A href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/explorer/sources.src">View source code of the page</A></font></li>		
<% End If %>
		<li><font face="Arial"><a href="downloadhandler/default.aspx">Download 
		handler</a><br>
		Send one or multiple files or stream data to the browser and take 
		benefit from the great performace advantages!</font></li>
		<li><font face="Arial"><a href="charts/index.aspx">Charts</a><br>
		Create nice charts on the fly</font></li>
		<li><font face="Arial"><a href="feedbackform/index.aspx">Custom feedback 
		forms</a><br>
		With them, you'll get an easy opportunity to write feedback forms. All 
		you need is the free ASP.Net WebMatrix from <a href="http://www.asp.net">
		www.asp.net</a> and some very low programming experience (some HTML 
		combined with very basic ASP.NET knowledge)</font></li>
		<li><font face="Arial"><a href="templatepage/index.aspx">Template Page</a><br>
		Take a look at a possible way to integrate the camm Web-Manager and the Webeditor in to your Layout</font></li>		
		<li><font face="Arial"><a href="dotnetlanguages/index.aspx">.NET languages comparison</a><br>
		See how to use camm Web-Manager with different programming languages supported by the .NET Framweork</font></li>
		<li><font face="Arial"><a href="codedemos/index.aspx">Code demo: accessing user profile information</a><br>
		Review some easy to understand code samples for the most often requirements</font></li>
		<li><font face="Arial"><a href="customizations/index.aspx">Customization templates</a><br>
		Customization starts with exchange of logos and texts. Find some valueable hints and templates to make this working easily</font></li>
		<li><font face="Arial"><a href="components_check/index.aspx">camm Web-Manager Components check</a><br>
		Verify if all required components for camm Web-Manager to run are available and usable.</font></li>
	</ul>
</body>
</html>