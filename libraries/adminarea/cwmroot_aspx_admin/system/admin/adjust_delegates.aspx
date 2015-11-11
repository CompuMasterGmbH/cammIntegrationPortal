<%@ Page Language="VB" EnableViewState="true" Inherits="CompuMaster.camm.WebManager.Pages.Administration.AdjustDelegates" %>

<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="System - User Administration - Users" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

<script language="JavaScript">
    function ValidateForm() {
        var objAuthType = document.getElementById("cmbAuthorizationType");
        var objUser = document.getElementById("cmbUser");
        if (objAuthType != null && objUser != null) {

            if ((objAuthType.value == '') || (objUser.value == -1)) {
                confirm("Please select all required fields");
                return (false);
            }
            else
                return (true);
        }
    }

    function SetHeaderColumn(columnname) {
        var oldvalue = document.getElementById("hdColumnName").value;
        document.getElementById("hdColumnName").value = columnname;
        if (oldvalue == columnname) {
            var oldsorting = document.getElementById("hdSortOrder").value;
            if (oldsorting == 'asc') {
                document.getElementById("hdSortOrder").value = "desc";
            }
            else {
            }
        }
        else {
            document.getElementById("hdSortOrder").value = "asc";
        }

        return true;
    }

</script>

<h3>
	Administration -
	<asp:label runat="server" id="lblTableName" />
	- Security delegates</h3>
<input type="hidden" id="hdColumnName" value="" runat="Server" />
<input type="hidden" id="hdSortOrder" value="" runat="Server" />
<table cellspacing="0" cellpadding="0" bgcolor="#ffffff" border="0" bordercolor="#C1C1C1">
	<tbody>
		<tr>
			<td valign="top" colspan="3">
				<h4>
					<asp:label runat="server" id="lblTitleName" />
				</h4>
				<asp:repeater id="rptAdjust" runat="server" enableviewstate="True">
					<HeaderTemplate>
					<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TBODY>						
						<TR>						
						<TH align="left" width="200" valign="top" BGCOLOR="#C1C1C1"><P><FONT size="2"><asp:LinkButton Runat=server ID="LinkButtonDelegation" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Security delegation object</asp:LinkButton></font></P></TH>
						<TH align="left" width="250" valign="top" BGCOLOR="#C1C1C1"><P><FONT size="2"><asp:LinkButton Runat=server ID="LinkButtonAdministrator" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Security administrator</asp:LinkButton></font></P></TH>
						<TH align="left" width="100" valign="top" BGCOLOR="#C1C1C1"><P><FONT size="2"><asp:LinkButton Runat=server ID="LinkButtonSecurityObject" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Security Object</asp:LinkButton><asp:Label Text="Action" runat="server" id="lblAction"/></font></P></TH>
						<TH align="left" width="250" valign="top" BGCOLOR="#C1C1C1"><P><FONT size="2"><asp:LinkButton Runat=server ID="LinkButtonReleasedBy" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Released By</asp:LinkButton></font></P></TH>
						<TH align="left" width="250" valign="top" BGCOLOR="#C1C1C1"><P><FONT size="2"><asp:LinkButton Runat=server ID="LinkButtonReleasedOn" Font-Bold=True ForeColor="#000000" ToolTip="Click to sort ...">Released On</asp:LinkButton></font></P></TH>
						</TR>
					</HeaderTemplate>
					<ItemTemplate>
						<TR>
							<TD valign="Top">
									<FONT  size="2"><%#DataBinder.Eval(Container.DataItem,"AuthorizationType")%> 
									</FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2"><asp:Label id="lblSecurityAdmin" runat="server" /></FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2">										
										<asp:HyperLink id="hypScript" runat="server"/>
										<asp:Label runat="server" id="lblSecuredObject"/>
									</FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2"><asp:Label id="lblReleasedBy" runat="server" /></FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2"><asp:Label id="lblReleasedOn" runat="server" /></FONT>
							</TD>
						</TR>	
					</ItemTemplate>
					<AlternatingItemTemplate>
						<TR BGCOLOR="#E1E1E1">
							<TD valign="Top">
									<FONT  size="2"><%#DataBinder.Eval(Container.DataItem,"AuthorizationType")%> 
									</FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2"><asp:Label id="lblSecurityAdmin" runat="server" /></FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2">
										
										<asp:HyperLink id="hypScript" runat="server"/>
										<asp:Label runat="server" id="lblSecuredObject"/>
									</FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2"><asp:Label id="lblReleasedBy" runat="server" /></FONT>
							</TD>
							<TD valign="Top">
									<FONT  size="2"><asp:Label id="lblReleasedOn" runat="server" /></FONT>
							</TD>
						</TR>	
					</AlternatingItemTemplate>
					<FooterTemplate>
		</FooterTemplate> </asp:Repeater>
		<tr id="trAdd" runat="server">
			<td valign="Top" align="left">
				<font color="TextColorOfLine" size="2">
					<asp:dropdownlist runat="server" id="cmbAuthorizationType" style="width: 150px" size="1" />
				</font>
			</td>
			<td align="left">
				<font color="TextColorOfLine" size="2">
					<asp:dropdownlist runat="server" id="cmbUser" style="width: 200px" size="1" />
				</font>
			</td>
			<td align="left">
				<font color="TextColorOfLine" size="2">
					<asp:button id="btnSubmit" runat="server" text="Save" commandname="Save" align="left" />
				</font>
			</td>
			<td>
			</td>
			<td>
			</td>
		</tr>
		<tr>
			<td colspan="5" align="LEFT">
				&nbsp;<br>
				<asp:hyperlink runat="server" id="hypDeligates" />
			</td>
		</tr>
	</tbody>
</table>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu ID="cammWebManagerAdminMenu" runat="server"></camm:WebManagerAdminMenu>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
