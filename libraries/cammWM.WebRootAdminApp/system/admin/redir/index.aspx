<%@ Page Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.Redirector.Pages.Administration.Overview" %>
<%@ OutputCache Duration="1" VaryByParam="none" Location="None" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top_wo_form.aspx"-->
<camm:webmanager id="cammWebManager" runat="server"></camm:webmanager>
		<SCRIPT language="JavaScript">
		<!--
		//pop-up window with the information on event
		function popup(url){
			if (document.all)
				var xMax = screen.width, yMax = screen.height;
			else {
				if (document.layers)
					var xMax = window.outerWidth, yMax = window.outerHeight;
				else
					var xMax = 740, yMax=480;
				}
			var xOffset = (xMax - 480)/2, yOffset = (yMax - 250)/2;

			cuteLittleWindow = window.open(url, 'littleWindow', 
			'width=480,height=290,scrollbars=yes,resizable=yes,screenX=' + xOffset + ',screenY=' + yOffset + 
			',top=' + yOffset + ',left=' + xOffset + '');
		}
		function popup2(url){
			if (document.all)
				var xMax = screen.width, yMax = screen.height;
			else {
				if (document.layers)
					var xMax = window.outerWidth, yMax = window.outerHeight;
				else
					var xMax = 740, yMax=350;
				}
			var xOffset = (xMax - 650)/2, yOffset = (yMax - 200)/2;

			cuteLittleWindow = window.open(url, 'littleWindow', 
			'width=740,height=300,scrollbars=yes,resizable=yes,screenX=' + xOffset + ',screenY=' + yOffset + 
			',top=' + yOffset + ',left=' + xOffset + '');
		}
		//-->
		</SCRIPT>
		<form method="post" runat="server">
			<P align="center"><asp:datagrid id="DataTable" runat="server" OnItemDataBound="button_itemdatabound" AutoGenerateColumns="False" width="90%">
					<HeaderStyle Font-Bold="True" HorizontalAlign="Center" ForeColor="Black" VerticalAlign="Middle"	BackColor="#CCCCFF"></HeaderStyle>
					<Columns>
						<asp:BoundColumn DataField="id" HeaderText="ID"></asp:BoundColumn>
						<asp:BoundColumn DataField="description" HeaderText="Description"></asp:BoundColumn>
						<asp:BoundColumn DataField="redirectto" HeaderText="Destination URL"></asp:BoundColumn>
						<asp:BoundColumn DataField="numberofredirections" HeaderText="Number of redirections"></asp:BoundColumn>
						<asp:TemplateColumn HeaderText="New">
							<HeaderTemplate>
								<asp:LinkButton ID="new1" Runat="server" Text="New"></asp:LinkButton>
							</HeaderTemplate>
							<ItemTemplate>
								<asp:LinkButton id="lb1" text="Modify" CommandName='<%# DataBinder.Eval(Container.DataItem,"id") %>' Runat=server>
								</asp:LinkButton>
								<asp:LinkButton id="lb2" text="Delete" CommandName='<%# DataBinder.Eval(Container.DataItem,"id") %>' OnCommand="delete_button_click" Runat=server>
								</asp:LinkButton>
								<nobr><asp:LinkButton id="lb3" text="Show Link" CommandName='<%# DataBinder.Eval(Container.DataItem,"id") %>' Runat=server>
								</asp:LinkButton></nobr>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:datagrid></P>
			<P align="center">
			<asp:Button id="refresh" runat="server" Text="Refresh"></asp:Button></P>
		</form>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom_wo_form.aspx"-->