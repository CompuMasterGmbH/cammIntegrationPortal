<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.SelectWebsiteAreaToEdit" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<table cellpadding="3" cellspacing="0" width="100%">
    <tr>
        <td>
            <h1>
                Textmodule Administration</h1>
            <h2>
                Select a TextModule to edit </h2>
            <a href="textmodules_overview.aspx">back to overview</a>
        </td>
    </tr>
    <tr>
        <td>
            <table style="border: dotted 1px #000000; width: 400px">
                <tr>
                    <td colspan="2">
                        <b>Select TextModule parameters</b>
                    </td>
                </tr>
                <tr>
                    <td style="width: 200px">
                        WebsiteAreaID
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlEditWebsiteAreaIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        MarketID
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlEditMarketIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        ServerGroupID
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlEditServerGroupIDs" autopostback="true" width="200px" />
                    </td>
                </tr>
            </table>
            <table style="width: 400px">
                <tr>
                    <td style="width: 200px">
                    </td>
                    <td>
                        <asp:button runat="server" id="BtnEditSelectedWebsiteArea" text="Edit selection" />
                    </td>
                </tr>
            </table>
            <asp:literal runat="server" id="ltrEditMsgs" />
        </td>
    </tr>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
