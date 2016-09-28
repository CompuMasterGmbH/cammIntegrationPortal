<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.CopyMarketID" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<table cellpadding="3" cellspacing="0" width="100%">
    <tr>
        <td>
            <h1>
                Textmodule Administration</h1>
            <h2>
                Copy TextModuleItens from market to market or create a new market</h2>
            <a href="textmodules_overview.aspx">back to overview</a>
        </td>
    </tr>
    <tr>
        <td>
            <table style="border: dotted 1px #000000; width: 400px">
                <tr>
                    <td style="width: 200px">
                        <b>Select source</b>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        WebsiteAreaID
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlSourceWebsiteAreaIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        MarketID
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlCopySourceMarketIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        ServerGroup
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlCopySourceServerGroupIDs" width="200px" />
                    </td>
                </tr>
            </table>
            <br>
            <table style="border: dotted 1px #000000; width: 400px">
                <tr>
                    <td style="width: 200px">
                        <b>Select target</b>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        WebsiteAreaID
                    </td>
                    <td style="width: 200px">
                        <asp:dropdownlist runat="server" id="DdlTargetWebsiteAreaIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        MarketID</td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlCopyTargetMarketIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        or create a new MarketID</td>
                    <td>
                        <asp:textbox runat="server" id="txtCopyTargetMarketID" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        ServerGroup
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlCopyTargetServerGroupIDs" width="200px" />
                    </td>
                </tr>
            </table>
           <table style="width: 400px">
                <tr>
                    <td style="width: 200px">
                    </td>
                    <td><asp:button runat="server" id="btnCopy" text="Proceed to next step" />
                    </td>
                </tr>            
            <asp:literal runat="server" id="ltrCopyMsgs" />
        </td>
    </tr>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
