<%@ Register TagPrefix="camm" TagName="WebManager" Src="/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server" />
<html>
<body style="font-family: Arial">
    <h1>
        Components check</h1>
    <p>
        <a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/components_check/index.src">
            View source code of the page</a></p>
    <h2>
        Test result</h2>
    <%= cammWebManager.IsSupported.RequiredComponentsDetailedCheckHtmlResult() %>
</body>
</html>
