<%@ Control Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Modules.Text.Administration.Controls.TextModules_Variable" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<TABLE cellSpacing="0" cellPadding="3" width="100%" border="0">
	<tr>
		<td><asp:Label ID="LabelTitle" Runat="server"></asp:Label></td>
	</tr>
	<TR>
		<TD>
			<asp:DataGrid id="DataGridVariable" width="100%" AutoGenerateColumns="False" CssClass="grid" CellPadding="3" EnableViewState=False 
				CellSpacing="0" runat="server">
				<SelectedItemStyle ForeColor="#000000" BackColor="#efefef"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="#eeeeee"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="#fefefe" backcolor="#808080"></HeaderStyle>
				<FooterStyle Backcolor="#aaaaaa"></FooterStyle>
				<ItemStyle VerticalAlign="Top"></ItemStyle>
				<Columns>
					<asp:BoundColumn DataField="Key" ReadOnly="True" HeaderStyle-Width="30%" />
					<asp:BoundColumn DataField="Value" ReadOnly="True" HeaderStyle-Width="40%" />
					<asp:BoundColumn DataField="Version" Visible="False" />
					<asp:TemplateColumn HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
						<ItemTemplate>
							<asp:CheckBox Runat=server ID="CheckboxReleased" Checked=<%#Container.DataItem("Released")%> Enabled=False>
							</asp:CheckBox>
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
					<TD width="25%">
						<asp:Label id="LabelTextBoxNewVariableName" Runat="server"></asp:Label>:&nbsp;<asp:TextBox id="TextBoxNewVariableName" Runat="server" MaxLength="100"></asp:TextBox></TD>
					<TD width="30%">
						<asp:Label id="LabelTextboxNewVariableValue" Runat="server"></asp:Label>:&nbsp;<asp:TextBox id="TextboxNewVariableValue" Columns="37" Runat="server"></asp:TextBox></TD>
					<TD>
						<asp:Button id="ButtonVariableAdd" Width="110" Runat="server" text=" Add "></asp:Button></TD>
					<td></td>
				</TR>
			</TABLE>
		</TD>
	</TR>
	<tr>
		<td align="right">
			<% if IsDevelopmentVersion then%>
			[Form: 103]&nbsp;&nbsp;
			<%end if%>
		</td>
	</tr>
</TABLE>
