<%@ Page MasterPageFile="~/portal/MasterPage.master" ValidateRequest="false" Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Specialized.StartPageWithoutFrameSet" %>

<%@ Register TagPrefix="camm" Namespace="CompuMaster.camm.WebManager.Controls" Assembly="cammWM" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.WebManager.Modules.WebEdit.Controls" Assembly="cammWM" %>
<cammWebEdit:SmartWcms MarketLookupMode="0" runat="server" ID="mainEditor"
    SecurityObjectEditMode="@@Supervisor"
    Docs="docs"
    DocsReadOnly="docs,~/docs,/docs"
    Media="~/media"
    MediaReadOnly="media,~/media,/media"
    Images="images"
    ImagesReadOnly="images,~/images,/images">
    {Insert content here...}
</cammWebEdit:SmartWcms>
