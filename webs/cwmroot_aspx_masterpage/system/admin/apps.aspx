<%@ Page language="VB" EnableViewState="True" Inherits="CompuMaster.camm.WebManager.Pages.Administration.ApplicationList" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager PageTitle="Administration - Applications" id="cammWebManager" SecurityObject="System - User Administration - Applications" EnableViewState="False" runat="server" />
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockHeader" Src="adminblockheader.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockFooter" Src="adminblockfooter.ascx" %>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminBlockContentLine" Src="adminblockcontentline.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_serverform.aspx"-->

	<h3><font face="Arial">Administration - Applications</font></h3>
	<TABLE cellSpacing="0" cellPadding="0" bgColor="#ffffff" border="0" bordercolor="#C1C1C1">
	  <TBODY>
	  <TR>
	  <TD><FONT face="Arial" size="2">
	      <Table width="100%" border="0" cellpadding="0" cellspacing="0">
	      <tr>
	      <td colspan="2">Select Application:&nbsp;<asp:TextBox EnableViewState="True"  BorderColor="#00446E" BorderStyle="Dotted" BorderWidth="1px" BackColor="White" runat="server" id="txtApplication" Height="20"></asp:TextBox>&nbsp;<asp:checkbox runat="server" ID="chkTop50Only" checked="true" text="Top 50 results only" /></Td>
	      </tr> 
	      <tr>
	      <td >
	       Select Market:&nbsp;<asp:dropdownlist EnableViewState="True" runat="server" id="cmbMarket" Height="20"></asp:dropdownlist>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Select Server Group:&nbsp;
	       <asp:dropdownlist EnableViewState="True" runat="server" id="cmbServerGroup" Height="20"></asp:dropdownlist>&nbsp;<asp:button runat="server" id="btnsubmit" text="Go" BorderColor="#00446E" BorderStyle="Solid" BorderWidth="1px" style="CURSOR: pointer" BackColor="White" /></asp:Button><input type="text" style="display:none" />
	       </td>
	      </tr> 
	      </Table> 
	      </FONT>
	   </TD>
	  </TR>
	  <tr>
		<td colspan="2">
			<p><font face="Arial" size="2" color="black"><asp:label runat="server" EnableViewState="False" id="lblErrMsg" /></font></p>
		</td>
	  </tr>
	  <TR>
        <TD>
			<asp:Repeater id="rptAppList" runat="Server"  EnableViewState="False">
				<HeaderTemplate>
					<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0" bordercolor="#FFFFFF">
						<TR>
							<TD valign="top" BGCOLOR="#C1C1C1"><P><FONT face="Arial" size="2"><b>ID<br>&nbsp; </b></FONT></P></TD>
							<TD valign="top" BGCOLOR="#C1C1C1"><P><FONT face="Arial" size="2"><b>Title<br>URL<br>Name</b></FONT></P></TD>
							<TD valign="top" BGCOLOR="#C1C1C1"><P><FONT face="Arial" size="2"><b>Location<br>Market</b></FONT></P></TD>
							<TD valign="top" BGCOLOR="#C1C1C1"><P><FONT face="Arial" size="2"><b><nobr>Released by</nobr><br><nobr>Release date</nobr></b></FONT></P></TD>
							<TD valign="top" BGCOLOR="#C1C1C1"><P><FONT face="Arial" size="2"><b>Actions</b><br><asp:HyperLink ID="hlnNew" runat="server" EnableViewState="False" />&nbsp;
								<asp:HyperLink ID="hlnSecurity" EnableViewState="False" runat="server" /></FONT></P></TD>
						</TR>
				</HeaderTemplate>
				<ItemTemplate>
						<TR>
							<TD valign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink id="hlnAnchorID" runat="server"></asp:HyperLink><asp:HyperLink ID="hlnID" runat="server" EnableViewState="False" /><span id="gc" runat="server" EnableViewState="False" />&nbsp;</FONT></P></TD>
							<TD valign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink ID="hlnTitleAdminArea" runat="server" EnableViewState="False" />&nbsp;<br>
								<asp:label id="lblNavURL" runat="server" EnableViewState="False" />&nbsp;<br>
								<font title="Use this application name to validate the access your documents"><asp:label id="lblTitle" runat="server" EnableViewState="False" /></font></FONT></P></TD>
							<TD valign="Top" Width="110"><P><FONT face="Arial" size="2"><nobr><asp:label id="lblServerDescription" runat="server" EnableViewState="False" /></nobr><br>
								<nobr><asp:label runat="server" EnableViewState="False" id="lblDescription" /><asp:label id="lblAbbreviation" runat="server" EnableViewState="False" />&nbsp;</nobr><br>
								<asp:HyperLink ID="hlnDescription" runat="server" EnableViewState="False" /></FONT></P></TD>
							<TD valign="Top" Width="120"><P><FONT face="Arial" size="2"><nobr><asp:HyperLink ID="hlnReleasedByLastName" EnableViewState="False" runat="server" />&nbsp;</nobr><br>
								<nobr><asp:label id="lblReleasedOn" EnableViewState="False" runat="server" />&nbsp;</nobr></FONT></P></TD>
							<TD valign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink ID="hlnUpdate" runat="server" EnableViewState="False" />&nbsp;
								<asp:HyperLink ID="hlnDelete" EnableViewState="False" runat="server" />&nbsp;
								<asp:HyperLink ID="hlnClone" EnableViewState="False" runat="server" /> &nbsp;</FONT></P></TD>
						</TR>
				</ItemTemplate>
				<AlternatingItemTemplate>
						<TR>
							<TD BGCOLOR="#E1E1E1" valign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink id="hlnAnchorID" runat="server"></asp:HyperLink><asp:HyperLink ID="hlnID" runat="server" EnableViewState="False" /><span id="gc" EnableViewState="False" runat="server" />&nbsp;</FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink ID="hlnTitleAdminArea" EnableViewState="False" runat="server" />&nbsp;<br><asp:label id="lblNavURL" EnableViewState="False" runat="server" /> &nbsp;
								<br><font title="Use this application name to validate the access your documents"><asp:label id="lblTitle" EnableViewState="False" runat="server" /></font></FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top" Width="110"><P><FONT face="Arial" size="2"><nobr><asp:label id="lblServerDescription" EnableViewState="False" runat="server" /></nobr>
								<br><nobr><asp:label EnableViewState="False" runat="server" id="lblDescription" /><asp:label id="lblAbbreviation" EnableViewState="False" runat="server" />&nbsp;</nobr>
								<br><asp:HyperLink ID="hlnDescription" EnableViewState="False" runat="server" /></FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top" Width="120"><P><FONT face="Arial" size="2"><nobr><asp:HyperLink ID="hlnReleasedByLastName" EnableViewState="False" runat="server" />&nbsp;</nobr>
								<br><nobr><asp:label id="lblReleasedOn" EnableViewState="False" runat="server" />&nbsp;</nobr></FONT></P></TD>
							<TD BGCOLOR="#E1E1E1" valign="Top"><P><FONT face="Arial" size="2"><asp:HyperLink ID="hlnUpdate" EnableViewState="False" runat="server" />&nbsp;
								<asp:HyperLink EnableViewState="False" ID="hlnDelete" runat="server" />&nbsp;<asp:HyperLink EnableViewState="False" ID="hlnClone" runat="server" />&nbsp;</FONT></P></TD>
						</TR>
				</AlternatingItemTemplate>
				<FooterTemplate>
					</TABLE>
				</FooterTemplate>
			</asp:Repeater>	    
	    </TD>
	  </TR>
      </TBODY></TABLE>
<%@ Register TagPrefix="camm" TagName="WebManagerAdminMenu" Src="adminmenu.ascx" %>
<camm:WebManagerAdminMenu  id="cammWebManagerAdminMenu" runat="server" EnableViewState="False" />
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_serverform.aspx"-->
