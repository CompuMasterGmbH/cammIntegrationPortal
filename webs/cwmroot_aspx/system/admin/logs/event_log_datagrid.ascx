<%@ Control Language="vb" Inherits="CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventlogDatagridControl" %>
<meta content="False" name="vs_snapToGrid">
<meta content="True" name="vs_showGrid">


<SCRIPT language="JavaScript">
//pop-up window with the information on event
function popup(url){
<!--
	if (document.all)
        var xMax = screen.width, yMax = screen.height;
    else {
        if (document.layers)
            var xMax = window.outerWidth, yMax = window.outerHeight;
        else
            var xMax = 640, yMax=480;
		}
    var xOffset = (xMax - 480)/2, yOffset = (yMax - 250)/2;

	cuteLittleWindow = window.open(url, 'littleWindow', 
	'width=480,height=250,scrollbars=yes,resizable=yes,screenX=' + xOffset + ',screenY=' + yOffset + 
	',top=' + yOffset + ',left=' + xOffset + '');
}
function selectAll(){

	for (var i=0;i<document.forms[0].elements.length;i++)
	{
		var e = document.forms[0].elements[i];
		if ((e.name == 'chkbx') && (e.type=='checkbox'))
		{
			e.checked = document.forms[0].EventlogDatagridControl_DataGrid1__ctl1_check_all.checked;
		}
	}
}
//-->
</SCRIPT>
<P align="center"><asp:datagrid id="DataGrid1" runat="server" GridLines="None" CellSpacing="2" CellPadding="4" AutoGenerateColumns="False"
		OnItemDataBound="Cl_ItemDataBound" HorizontalAlign="Center">
		<HeaderStyle Font-Bold="True" HorizontalAlign="Center" BackColor="#C0C0FF"></HeaderStyle>
		<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="Navy"></SelectedItemStyle>
		<ItemStyle ForeColor="Black"></ItemStyle>
		<Columns>
			<asp:BoundColumn DataField="id" HeaderText="id"></asp:BoundColumn>
			<asp:BoundColumn DataField="nm" HeaderText="Name"></asp:BoundColumn>
			<asp:BoundColumn DataField="ct" HeaderText="Typ"></asp:BoundColumn>
			<asp:BoundColumn DataField="ld" HeaderText="Date"></asp:BoundColumn>
			<asp:TemplateColumn HeaderText="Close event">
			<HeaderTemplate>
			<asp:Checkbox Title="All rows" id="check_all" text="Close event" textalign="left" runat="server"></asp:Checkbox>
			</HeaderTemplate>
			<ItemTemplate>
			<input type=checkbox name="chkbx" value='<%# DataBinder.Eval(Container.DataItem,"id") %>'>
			</ItemTemplate>
			</asp:TemplateColumn>
			<asp:BoundColumn DataField="cd" HeaderText="cd"></asp:BoundColumn>
		</Columns>
	</asp:datagrid></P>
<P align="center"><asp:label id="LabelIsData" runat="server" Font-Size="Larger" Font-Bold="True"></asp:label></P>
<P align="center"><asp:label id="LabelDescription" runat="server" Width="80%">Description</asp:label></P>
