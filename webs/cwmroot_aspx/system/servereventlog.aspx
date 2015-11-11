<%@ Page Language="VB" %>
<%@ Import Namespace="System.Diagnostics" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script runat="server">

    sub Page_Init(sender as Object, e as EventArgs)
       if not (cammWebManager.System_IsSuperVisor(cammWebManager.CurrentUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous)))
              cammWebManager.SecurityObject="System - User Administration - ServerSetup"
       end if
    end sub
    
    sub Page_Load(sender as Object, e as EventArgs)
       BindGrid()
    end sub
    
    sub LogGrid_Change(sender as Object, e as DataGridPageChangedEventArgs)
    
       'Set CurrentPageIndex to the page the user clicked.
       LogGrid.CurrentPageIndex = e.NewPageIndex
    
       'Rebind the data.
       BindGrid()
    end sub
    
    sub BindGrid()
       dim aLog as new EventLog()
       aLog.Log = EventLogName.SelectedValue
       aLog.MachineName = "."
    
       LogGrid.DataSource = LogData (aLog.Entries)
       LogGrid.DataBind()
    end sub
    
    protected sub EventLogName_SelectedIndexChanged(sender as object, e as EventArgs)
    '    Response.Write (EventLogName.SelectedValue)
    end sub
    
    private Function LogData(Logs As System.Diagnostics.EventLogEntryCollection) as System.Data.DataTable
    
            'Create table structure
            Dim MyTable As New System.Data.DataTable("EventLog")
            MyTable.Columns.Add(New System.Data.DataColumn("Index", GetType(Integer)))
            MyTable.Columns.Add(New System.Data.DataColumn("MachineName", GetType(String)))
            MyTable.Columns.Add(New System.Data.DataColumn("Site", GetType(System.ComponentModel.ISite)))
            MyTable.Columns.Add(New System.Data.DataColumn("Source", GetType(String)))
            MyTable.Columns.Add(New System.Data.DataColumn("TimeGenerated", GetType(Date)))
            MyTable.Columns.Add(New System.Data.DataColumn("TimeWritten", GetType(Date)))
            MyTable.Columns.Add(New System.Data.DataColumn("UserName", GetType(String)))
            MyTable.Columns.Add(New System.Data.DataColumn("EventID", GetType(Integer)))
            MyTable.Columns.Add(New System.Data.DataColumn("Data", GetType(Byte())))
            MyTable.Columns.Add(New System.Data.DataColumn("CategoryNumber", GetType(Short)))
            MyTable.Columns.Add(New System.Data.DataColumn("Category", GetType(String)))
            MyTable.Columns.Add(New System.Data.DataColumn("EntryType", GetType(System.Diagnostics.EventLogEntryType)))
            MyTable.Columns.Add(New System.Data.DataColumn("Message", GetType(String)))

		Dim MaxItemsCounter as integer = 0
    
            'Iterate through log items
            For MyCounter As Integer = Logs.Count - 1 To 0 Step -1
		MaxItemsCounter += 1
                Dim MyLog As System.Diagnostics.EventLogEntry
                MyLog = Logs(MyCounter)
    
                'Copy data into table
                Dim MyRow As System.Data.DataRow
                MyRow = MyTable.NewRow
                MyRow(0) = NotNothingOrDBNull(MyLog.Index)
                MyRow(1) = NotNothingOrDBNull(MyLog.MachineName)
                MyRow(2) = NotNothingOrDBNull(MyLog.Site)
                MyRow(3) = NotNothingOrDBNull(MyLog.Source)
                MyRow(4) = NotNothingOrDBNull(MyLog.TimeGenerated)
                MyRow(5) = NotNothingOrDBNull(MyLog.TimeWritten)
                MyRow(6) = NotNothingOrDBNull(MyLog.UserName)
                MyRow(7) = NotNothingOrDBNull(MyLog.EventID)
                'MyRow(8) = NotNothingOrDBNull(MyLog.Data)
                MyRow(9) = NotNothingOrDBNull(MyLog.CategoryNumber)
                MyRow(10) = NotNothingOrDBNull(MyLog.Category)
                MyRow(11) = NotNothingOrDBNull(MyLog.EntryType)
                MyRow(12) = NotNothingOrDBNull(MyLog.Message)
                MyTable.Rows.Add(MyRow)

		if MaxItemsCounter >= 200 Then
			exit for
		end if    
            Next
    
        Return MyTable

    End Function


    Private Function NotNothingOrDBNull(ByVal value As Object) As Object
        If value Is Nothing Then
            Return System.DBNull.Value
        Else
            Return value
        End If
    End Function
	
</script>
<html>
<head>
</head>
<body bgcolor="#ffffff">
    <form runat="server">
        <h3>Event Log
        </h3>
        <asp:RadioButtonList id="EventLogName" runat="server" AutoPostBack="True" OnSelectedIndexChanged="EventLogName_SelectedIndexChanged">
            <asp:ListItem Value="Application" Selected="True">Application</asp:ListItem>
            <asp:ListItem Value="System">System</asp:ListItem>
        </asp:RadioButtonList>
        <p>
            <asp:DataGrid id="LogGrid" runat="server" AutoGenerateColumns="false" HeaderStyle-BackColor="#aaaadd" Font-Size="8pt" Font-Name="Verdana" CellSpacing="0" CellPadding="3" GridLines="Both" BorderWidth="1" BorderColor="black" OnPageIndexChanged="LogGrid_Change" PagerStyle-PrevPageText="Prev" PagerStyle-NextPageText="Next" PagerStyle-HorizontalAlign="Right" PagerStyle-Mode="NumericPages" PageSize="50" AllowPaging="True" AllowSorting="True">
                <HeaderStyle backcolor="#AAAADD"></HeaderStyle>
                <Columns>
                    <asp:BoundColumn DataField="EntryType" HeaderText="TOF"></asp:BoundColumn>
                    <asp:BoundColumn DataField="TimeGenerated" HeaderText="Date/Time"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Source" HeaderText="Source"></asp:BoundColumn>
                    <asp:BoundColumn DataField="EventID" HeaderText="Event ID"></asp:BoundColumn>
                    <asp:BoundColumn DataField="Message" HeaderText="Message"></asp:BoundColumn>
                    <asp:BoundColumn DataField="UserName" HeaderText="User name"></asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="TimeWritten" HeaderText="Time Written"></asp:BoundColumn>
                    <asp:BoundColumn DataField="MachineName" HeaderText="Machine Name"></asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="Category" HeaderText="Category"></asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="CategoryNumber" HeaderText="Category Number"></asp:BoundColumn>
                    <asp:BoundColumn Visible="False" DataField="Index" HeaderText="Index"></asp:BoundColumn>
                </Columns>
                <PagerStyle nextpagetext="Next" prevpagetext="Prev" horizontalalign="Right" mode="NumericPages"></PagerStyle>
            </asp:DataGrid>
        </p>
    </form>
</body>
</html>