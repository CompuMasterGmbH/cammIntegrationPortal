<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ConfigurationLogging" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Advanced Configuration" id="cammWebManager"
    SecurityObject="System - User Administration - ServerSetup" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>

<script runat="server">
	
</script>

<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->
<h3>
    <font face="Arial">Administration - Logging configuration</font></h3>
<asp:label runat="server" id="lblMsg" forecolor="green" />
<br />
Current Log-Entries in database:
<asp:label runat="server" id="lblRowsInLogTable" />
<br />
<br />
Maximum number of Log-Entries in database:
<asp:textbox runat="server" id="txtMaxRowsInLogTable" />
<br />
Maximum age of Log-Entries in database:
<asp:textbox runat="server" id="txtMaxDaysOfLogEntries" /> days 
<br /><br />
<asp:button runat="server" id="btnSaveSettings" text="Save settings" />
<br />
<hr>
Here you can specify how long a specific ConflictType should be kept. Otherwise, the default value above will be used. 
<style>
table#tblConflictTypes.TableWithAlternatingRowBackground tr:nth-child(odd) { background-color:lightgray; }
table#tblConflictTypes.TableWithAlternatingRowBackground th { background-color:gray; }
table#tblConflictTypes.TableWithAlternatingRowBackground { border-spacing:0px; cellspacing:0px; }
</style>
<asp:Table class="TableWithAlternatingRowBackground" id="tblConflictTypes" BorderWidth="0" runat="server" />
<br />
<asp:button runat="server" id="btnSaveConflictTypes" text="Save settings" />
<hr>
Delete old logs now!&nbsp;<asp:button runat="server" id="btnDeleteOldLogsNow" text="Delete now!" /><hr>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu HRef="configuration.aspx" id="cammWebManagerAdminMenu"
    runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
