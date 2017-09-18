<%@ Control Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Controls.TextModules_Textblock" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0">
	<tr>
		<td><asp:Label ID="LabelTitle" Runat="server"></asp:Label></td>
	</tr>
	<TR>
		<TD>
			<asp:DataGrid id="DataGridTextBlock" runat="server" CellPadding="3" CellSpacing="0" CssClass="grid" EnableViewState=False 
				AutoGenerateColumns="False" width="100%">
				<SelectedItemStyle ForeColor="#000000" BackColor="#efefef"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="#eeeeee"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="#fefefe" backcolor="#808080"></HeaderStyle>
				<FooterStyle Backcolor="#aaaaaa"></FooterStyle>
				<ItemStyle VerticalAlign="Top"></ItemStyle>
				<Columns>
					<asp:BoundColumn DataField="Key" ReadOnly="True" HeaderStyle-Width="20%" />
					<asp:BoundColumn DataField="Value" ReadOnly="True" HeaderStyle-Width="50%" />
					<asp:BoundColumn DataField="Version" Visible="False" />
					<asp:BoundColumn DataField="TypeID" Visible="False" />
					<asp:TemplateColumn HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:CheckBox Runat=server ID="CheckboxReleased" Checked=<%#Container.DataItem("Released")%> Enabled=False>
							</asp:CheckBox>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:HyperLink ID="HyperLinkPreview" NavigateUrl="#" Runat=server Target=_blank onclick=<%#Container.DataItem("Preview")%> >
								<%#Container.DataItem("LabelPreview")%>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:HyperLink ID="HyperlinkEdit" Runat=server NavigateUrl=<%#Container.DataItem("EditURL")%> Target="_self" >
								<%#Container.DataItem("LabelEdit")%>
							</asp:HyperLink>
						</ItemTemplate>
					</asp:TemplateColumn>
				</Columns>
			</asp:DataGrid></TD>
	</TR>
	<TR>
		<TD>
			<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0">
				<TR>
					<TD width="30%">
						<asp:Label ID="LabelNewTextblockName" Runat="server"></asp:Label>:&nbsp;<asp:TextBox id="TextBoxNewTextblockName" Runat="server" MaxLength="100"></asp:TextBox></TD>
					<TD width="40%">
						<asp:Button id="ButtonAddTextblock" Width="110" Runat="server"></asp:Button></TD>
					<TD>
					</TD>
					<TD>
					</TD>
				</TR>
			</TABLE>
		</TD>
	</TR>
	<tr>
		<td align="right">
			<% if IsDevelopmentVersion then%>
			[Form: 107]&nbsp;&nbsp;
			<%end if%>
		</td>
	</tr>
</TABLE>
