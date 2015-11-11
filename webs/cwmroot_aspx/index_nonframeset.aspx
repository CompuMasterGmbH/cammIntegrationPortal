<%@ Page validateRequest=false language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Specialized.StartPageWithoutFrameSet" %>
<%@ Register TagPrefix="camm" Namespace="CompuMaster.camm.WebManager.Controls" Assembly="cammWM" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<%@ Register TagPrefix="camm" TagName="PageHtmlBegin" Src="~/sysdata/includes/standardtemplate_top_nonframeset_serverform_html.ascx" %>
<%@ Register TagPrefix="camm" TagName="PageHtmlEnd" Src="~/sysdata/includes/standardtemplate_bottom_nonframeset_serverform_html.ascx" %>
<%@ Register TagPrefix="camm" TagName="PageBodyBegin" Src="~/sysdata/includes/standardtemplate_top_nonframeset_serverform_body.ascx" %>
<%@ Register TagPrefix="camm" TagName="PageBodyEnd" Src="~/sysdata/includes/standardtemplate_bottom_nonframeset_serverform_body.ascx" %>
<%@ Register TagPrefix="cammWebEdit" Namespace="CompuMaster.camm.WebManager.Modules.WebEdit.Controls" Assembly="cammWM" %>
<camm:WebManager id="cammWebManager" runat="server" />
<camm:PageHtmlBegin id="PageHtmlBegin" runat="server" />
<form runat="server">
<camm:PageBodyBegin id="PageBodyBegin" runat="server" />
			<cammWebEdit:SmartWcms MarketLookupMode="0" runat="server" id="mainEditor"
				SecurityObjectEditMode="@@Supervisor" 
				Docs="docs"
				DocsReadOnly="docs,~/docs,/docs"
				Media="~/media"
				MediaReadOnly="media,~/media,/media"
				Images="images"
				ImagesReadOnly="images,~/images,/images"
			>{Insert content here...}</cammWebEdit:SmartWcms>
<camm:PageBodyEnd id="PageBodyEnd" runat="server" />
</form>
<camm:PageHtmlEnd id="PageHtmlEnd" runat="server" />