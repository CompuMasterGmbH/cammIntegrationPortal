<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.BatchUserFlags.Editor" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Modify user flags" id="cammWebManager" SecurityObject="System - User Administration - Users" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
	<font face="Arial">Administration - Batch User Flags Editor</font></h3>
<hr />
<asp:placeholder runat="server" id="phFilter">
<h4>Select Group and Flagname which you want to edit.</h4>
    <table>
        <asp:tablerow runat="server" id="rowGroup">    
            <asp:tablecell runat="server">Group</asp:tablecell><asp:tablecell runat="server"><asp:dropdownlist runat="server" width="350px" id="ddlCwmGroupnames" /></asp:tablecell>
       </asp:tablerow>
		<asp:tablerow runat="server" id="rowApp">
			<asp:tablecell runat="server">Application</asp:tablecell><asp:tablecell runat="server"><asp:dropdownlist runat="server" width="350px" id="ddlCwmApps" /></asp:tablecell>
		</asp:tablerow>
		<tr>
            <td>Flagname</td><td><asp:textbox runat="server" width="350px" id="txtFlagname" /></td>
        </tr>
        <tr>    
            <td></td><td><asp:radiobuttonlist runat="server" id="rblFilterFlagvalues">
                <asp:listitem runat="server" text="All Users" value="1" selected="true" />
                <asp:listitem runat="server" text="Users which have this flag filled" value="2" />
                <asp:listitem runat="server" text="Users which have this flag empty" value="3" />
				<asp:listitem runat="server" text="Users which have this flag empty or an invalid type" value="4" />
            </asp:radiobuttonlist></td>
        </tr>
		<tr>    
            <td>Items per page</td><td><asp:dropdownlist runat="server" id="ddlItemsPerPage" autopostback="true">
            <asp:listitem runat="server" value="200" text="200" selected="true" />
            <asp:listitem runat="server" value="500" text="500" />
            <asp:listitem runat="server" value="1000" text="1000" />
            <asp:listitem runat="server" value="0" text="Unlimited" />
            </asp:dropdownlist>
            </td>
        </tr>
        <tr>    
            <td></td><td><asp:button width="350px" runat="server" id="btnDoFilter" text="Go" /></td>
        </tr>
    </table>
</asp:placeholder>
<hr />
<asp:checkbox runat="server" id="chkSearchAndReplace" text="Search and Replace" autopostback="true" causesvalidation="false" />
<asp:placeholder runat="server" id="phSearchAndReplace" visible="false">
Search for
<asp:textbox runat="server" id="txtSearch" />
and Replace it with
<asp:textbox runat="server" id="txtReplace" />
in the current flag values.
<asp:button runat="server" id="btnSearchAndReplace" text="Search & Replace" />
</asp:placeholder>
<hr />
<asp:placeholder runat="server" id="phContent" />
<asp:placeholder runat="server" id="phPaginationLinks" />
<br />
<asp:button runat="server" id="btnSaveAllTop" text="Save all" />
<asp:table runat="server" enableviewstate="true" id="tblUsers" />
<asp:button runat="server" id="btnSaveAllBottom" text="Save all" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
