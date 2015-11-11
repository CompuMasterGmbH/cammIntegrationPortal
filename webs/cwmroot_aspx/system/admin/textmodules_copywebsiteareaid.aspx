<%@ Page Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Pages.CopyWebsiteAreaID" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebmanager" runat="server" SecurityObject="System - TextModules" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<table cellpadding="3" cellspacing="0" width="100%">
    <tr>
        <td>
            <h1>
                Textmodule Administration</h1>
            <h2>
                Copy a complete TextModule to an other TextModule</h2>
            <a href="textmodules_overview.aspx">back to overview</a>
        </td>
    </tr>
    <tr>
        <td>
            <table style="border: dotted 1px #000000; width: 400px">
                <tr>
                    <td colspan="2">
                        <b>Select source</b>
                    </td>
                </tr>
                <tr>
                    <td style="width: 200px">
                        WebsiteAreaID
                    </td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlCopySourceWebsiteAreaIDs" width="200px" />
                    </td>
                </tr>
            </table>
            <br>
            <table style="border: dotted 1px #000000; width: 400px">
                <tr>
                    <td colspan="2">
                        <b>Select target</b>
                    </td>
                </tr>
                <tr>
                    <td style="width: 200px">
                        WebsiteAreaID</td>
                    <td>
                        <asp:dropdownlist runat="server" id="DdlCopyTargetWebsiteAreaIDs" width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        or create a new one</td>
                    <td>
                        <asp:textbox runat="server" id="txtCopyTargetWebsiteAreaID" width="200px" />
                    </td>
                </tr>
            </table>
            <table style="width: 400px">
                <tr>
                    <td style="width: 200px">
                    </td>
                    <td>
                        <asp:button runat="server" id="btnCopy" text="Copy" />
                    </td>
                </tr>
            </table>
            <asp:literal runat="server" id="ltrCopyMsgs" />
        </td>
    </tr>
</table>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
