<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.Overview" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<table cellpadding="3" cellspacing="0" width="100%">
    <tr>
        <td>
            <h1>
                Textmodule Administration</h1>
            <h2>
                Overview</h2>
        </td>
    </tr>
    <tr>
        <td>
            <ul>
                <li><a href="textmodules_selectitemtoedit.aspx">Select a TextModule to edit</a></li>
                <li><a href="textmodules_copymarketid.aspx">Copy TextModuleItens from market to market <b>or</b> create a new market</a></li>
                <li><a href="textmodules_copywebsiteareaid.aspx">Copy a complete TextModule to an other TextModule</a></li>
            </ul>
        </td>
    </tr>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
