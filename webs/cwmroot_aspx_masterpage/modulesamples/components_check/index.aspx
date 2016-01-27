<%@ Page MasterPage="/portal/MasterPage.master" Inherits="CompuMaster.camm.WebManager.Pages.Page" %>
    <h1>
        Components check</h1>
    <p>
        <a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/components_check/index.src">
            View source code of the page</a></p>
    <h2>
        Test result</h2>
    <%= cammWebManager.IsSupported.RequiredComponentsDetailedCheckHtmlResult() %>
