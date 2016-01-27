<%@ Page Language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server"></camm:WebManager>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
</head>
<body>
    <h1>
        <font face="Arial">Download Handler Samples </font> 
    </h1>
	<h3>
        <font face="Arial">Preparation of downloads and providing separate links 
		to them (downloads via separate HTTP requests)</font></h3>
	<ul>
		<li><font face="Arial" size="2">Requires full featured download handler; this is 
		the case if your webserver account (regulary the ASPNET user account) 
		got write access to <code><font face="Arial">/system/downloads/</font></code><br />Current status is: <%= IIf(cammWebManager.DownloadHandler.IsFullyFeatured, "FULLY FEATURED", "PARTIALLY FEATURED (MISSING WRITE ACCESS TO FOLDER)") %> </font></li>
		<li><code><font face="Arial" size="2">Samples</font></code><ul>
		<li><code><font face="Arial" size="2">Download of existing, single, uncompressed 
		files</font></code><ul>
			<li><font face="Arial" size="2"><a href="separate%20request%20transfers/DHTest0.aspx">Demo 1</a>: single-file / 
			public cache / very fast since no roundtrips to any databases or camm 
		Web-Manager is required<br>
			<a href="separate%20request%20transfers/DHTest0.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/separate%20request%20transfers/DHTest0.src">
			View source code</a></font></li>
			<li><font face="Arial" size="2"><a href="separate%20request%20transfers/DHTest2.aspx">Demo 3</a>: single-file / 
		security related object (no cache)<br>
			<a href="separate%20request%20transfers/DHTest2.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/separate%20request%20transfers/DHTest2.src">
			View source code</a></font></li>
		</ul></li>
		<li><code><font face="Arial" size="2">Download of existing, multiple or compressed 
		files</font></code><ul>
			<li><font face="Arial" size="2"><a href="separate%20request%20transfers/DHTest1.aspx">Demo 2</a>: multi-file (leads 
		to ZIP archive download) / 
			public cache / very fast since no roundtrips to any databases or camm 
		Web-Manager is required<br>
			<a href="separate%20request%20transfers/DHTest1.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/separate%20request%20transfers/DHTest1.src">
			View source code</a></font></li>
			<li><font face="Arial" size="2"><a href="separate%20request%20transfers/DHTest3.aspx">Demo 4</a>: multi-file (leads 
		to ZIP archive download) / 
		security related object (no cache)<br>
			<a href="separate%20request%20transfers/DHTest3.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/separate%20request%20transfers/DHTest3.src">
			View source code</a></font></li>
		</ul></li>
		<li><code><font face="Arial" size="2">Download of single dynamic (=temporary)
		files</font></code><ul>
			<li><font face="Arial" size="2"><a href="single%20dynamic%20file/DHTest0.aspx">Demo 5</a>: write into a temporary file / 
			webserver session cache<br>
			<a href="single%20dynamic%20file/DHTest0.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/single%20dynamic%20file/DHTest0.src">
			View source code</a></font></li>
		</ul></li>
	</ul>
		</li>
	</ul>
	<h3><font face="Arial">Preparation of downloads and send the file 
	immediately to the browser (downloads via current HTTP request)</font></h3>
	<ul>
		<li><font face="Arial" size="2">Full write access allows creation of ZIP 
		files and preparation of temporary files to redirect the browser to</font></li>
		<li><font face="Arial" size="2">Read only access requires that the 
		ASP.NET engine sends the data</font><ul>
		<li><font face="Arial" size="2">Advantages</font><ul>
			<li><font face="Arial" size="2">Run on all server configurations (with 
			write access to /system/downloads or also without)</font></li>
		</ul></li>
		<li><font face="Arial" size="2">Disadvantages</font><ul>
			<li><font face="Arial" size="2">Leads to exceptions 
		when processing download collections with multiple files (they get 
			automatically ZIPped) or when ZIPping 
		has been enforced</font></li>
			<li><font face="Arial" size="2">Some older browsers might not 
			recognize the MIME type of the sent file (the MIME type is always 
			sent in the HTTP headers), they look for the 
			extension of the download URL (which is something like &quot;.aspx&quot;)</font></li>
			<li><font face="Arial" size="2">Dynamic content (all things which 
			are not files, e. g. streams, database content) must be transferred 
			immediately and can't be saved as a file.</font></li>
			<li><font face="Arial" size="2">Browsers have to download the file 
			completely, first, before they can involve their plugins (like 
			Acrobat Reader for PDF documents)</font></li>
			<li><font face="Arial" size="2">ASP.NET always implements script 
			timeouts; if the download has to be processed by a slow modem 
			dial-up connection, it might happen that after 15 minutes the 
			download hasn't been finished but the ASP.NET worker process stops 
			your script (this scenario often happens for files with several MB 
			of size)</font></li>
		</ul></li>
	</ul>
		</li>
		<li><font face="Arial" size="2">Samples</font><ul>
			<li><font face="Arial" size="2">Creation of ZIP files (only with 
			write permissions!)</font><ul>
			<li><font face="Arial" size="2"><a href="current%20request%20transfers/DHTest1.aspx">Demo 2</a>: multi-file (leads 
		to ZIP archive download) / 
			public cache / very fast since no roundtrips to any databases or camm 
		Web-Manager is required<br>
			<a href="current%20request%20transfers/DHTest1.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/current%20request%20transfers/DHTest1.src">
			View source code</a></font></li>
			<li><font face="Arial" size="2"><a href="current%20request%20transfers/DHTest3.aspx">Demo 4</a>: multi-file (leads 
		to ZIP archive download) / 
		security related object (no cache)<br>
			<a href="current%20request%20transfers/DHTest3.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/current%20request%20transfers/DHTest3.src">
			View source code</a></font></li>
		</ul></li>
			<li><font face="Arial" size="2">Transfer of a multiple 
			files with dynamic content (e. g. binary data from a database)</font><ul>
			<li><font face="Arial" size="2"><a href="current%20request%20transfers/DHTest10.aspx">Demo</a><br>
			<a href="current%20request%20transfers/DHTest10.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/current%20request%20transfers/DHTest10.src">
			View source code</a></font></li>
		</ul></li>
			<li><font face="Arial" size="2">Transfer of a single, uncompressed 
			file either by storing a separate temporary file (with write 
			permissions)) or by data transfer initiated by your ASP.NET 
			script (without write permissions)</font><ul>
			<li><font face="Arial" size="2"><a href="current%20request%20transfers/DHTest0.aspx">Demo 1</a>: single-file / 
			public cache / very fast since no roundtrips to any databases or camm 
		Web-Manager is required<br>
			<a href="current%20request%20transfers/DHTest0.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/current%20request%20transfers/DHTest0.src">
			View source code</a></font></li>
			<li><font face="Arial" size="2"><a href="current%20request%20transfers/DHTest2.aspx">Demo 3</a>: single-file / 
		security related object (no cache)<br>
			<a href="current%20request%20transfers/DHTest2.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/current%20request%20transfers/DHTest2.src">
			View source code</a></font></li>
		</ul></li>
			<li><font face="Arial" size="2">Transfer of a single, uncompressed 
			file with dynamic content (e. g. binary data from a database)</font><ul>
			<li><font face="Arial" size="2"><a href="current%20request%20transfers/DHTest11.aspx">Demo</a><br>
			<a href="current%20request%20transfers/DHTest11.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/current%20request%20transfers/DHTest11.src">
			View source code</a></font></li>
		</ul></li>
		</ul>
		</li>
	</ul>
	<h3><font face="Arial">Download handler without usage of camm Web-Manager 
	security</font></h3>
    <ul>
      <li> <font face="Arial" size="2">The download files can be created also 
		without camm Web-Manager in charge. It will register those files with a 
		timeout in its database and save them in a server folder which is 
		dependent on the server name and port (to enable you to use virtual 
		servers on the same web root folder).<br>
		<a href="stand%20alone%20dh/DHTest0.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/stand alone dh/DHTest0.src">View source code</a></font></li>
		<li> <font face="Arial" size="2">When you only use the download handler 
      module of camm Web-Manager, but not the camm Web-Manager's security and 
      authorization feature, you have to trigger the clean-up processes by 
      yourself. It is recommended to clean up regulary by the event handlers in 
      global.asax. Don't forget to set the connection string for the database of 
      camm Web-Manager in your web.config file!<br>
		<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/stand alone dh/global.src">View source code</a></font></li>
    </ul>
	<h3>
        <font face="Arial">Temporary files cleanup</font></h3>
	<ul>
		<li><font face="Arial" size="2">The default clean up happens always and 
		automatically by camm Web-Manager when 
		a user logs out; it removes all registered files which have been timed 
		out<br>
			<a href="cleanup/DHTest0.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/cleanup/DHTest0.src">
			View source code</a></font></li>
		<li><code><font face="Arial" size="2">There is an extended method 
		CleanUpUnregisteredFiles which can be called manually. It searches for unregistered files in the 
		download folder for the temporary files. This is an extended security 
		feature&nbsp; to ensure that nothing/nobody has used the chance to place 
		some unwanted files in the download folder. Regulary, no file should be 
		there unregistered.</font></code><font face="Arial" size="2"><br>
			<a href="cleanup/DHTest1.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/cleanup/DHTest1.src">
			View source code</a></font></li>
		<li><code><font face="Arial" size="2">The method 
		CleanUpAllRegisteredFiles removes all registered files (be aware that you might also delete some files which 
		are in use by other users!)</font></code><font face="Arial" size="2"><br>
			<a href="cleanup/DHTest2.aspx">Run sample</a> |
			<a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/downloadhandler/cleanup/DHTest2.src">
			View source code</a></font></li>
	</ul>
</body>
</html>