Option Strict Off
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Telerik.WebControls

Namespace CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.Page
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Base page for all log analysis
    ''' </summary>
    ''' <remarks>
    '''     Common tasks are the definition of the security object name and the providing of common calendar and button controls
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	02.12.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Page
        'ToDo: ServerIPs could lead to Sql errors/injections if the server IP contains a accent ("'") --> BUG!
        Inherits CompuMaster.camm.WebManager.Pages.ProtectedPage

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            cammWebManager.AuthorizeDocumentAccess("System - User Administration - LogAnalysis")
        End Sub

        Protected CheckBoxListServerGroups As New CheckBoxList
        Protected ServerGroupsPrintTable As New Table
        Protected IsPrintView As Boolean
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Contains a list of all server IPs of all servergroups
        ''' </summary>
        ''' <remarks>
        '''     1st dimension: number of servergroups
        '''     2nd dimension: number of server in a servergroup
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ServerGroupIPs(10, 10) As String 'ToDo: allow more than 11 servergroups, let's say unlimited --> dynamically expand this array
        Protected ServerGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()

        Protected TextboxDateFrom, TextboxDateTo As TextBox
        Protected LabelDateToError, LabelDateFromError, LabelInterval As Label
        Protected ButtonShowReport As Button
        Protected TopBorderTRow, BottomBorderTRow, TitleTRow, MenuBarTRow, cammlogo As TableRow
        Protected AppNameCell, TitleTCell, LeftTCell, RightTCell As TableCell
        Protected LinkButtonPrintView As LinkButton
        Protected PrintViewTable As Table
        Protected WithEvents CalendarFrom, CalendarTo As Calendar
        Protected CellServerGroupsPrintTable, CellTo, CellFrom As TableCell
        Protected CheckBoxCell As HtmlControls.HtmlTableCell
        Protected SelectTable As HtmlControls.HtmlTable
        Protected PrintPage As HtmlControls.HtmlAnchor

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''      Styles for the calendars and the button 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub PagesInit()
            ButtonShowReport.Attributes.Add("style", "color:White;background-color:#648BED;border-color:White;border-width:1px;border-style:Outset;")
            ButtonShowReport.Attributes.Add("ONMOUSEOVER", "this.style.backgroundColor='#EBF1FA'; this.style.borderColor='#648BED';this.style.color='#648BED';")
            ButtonShowReport.Attributes.Add("ONMOUSEOUT", "this.style.backgroundColor='#648BED';this.style.color='white';")
            ButtonShowReport.Font.Bold = True
            CalendarFrom.Width = Unit.Pixel(170)
            CalendarFrom.Font.Name = "MS Sans Serif"
            CalendarFrom.Font.Size = FontUnit.Point(9)
            CalendarFrom.NextPrevFormat = NextPrevFormat.CustomText
            CalendarFrom.NextMonthText = ">>>"
            CalendarFrom.PrevMonthText = "<<<"
            CalendarFrom.NextPrevStyle.BorderStyle = BorderStyle.None
            CalendarFrom.NextPrevStyle.Font.Bold = True
            CalendarFrom.NextPrevStyle.BackColor = Color.Gray
            CalendarFrom.NextPrevStyle.ForeColor = Color.White
            CalendarFrom.NextPrevStyle.Height = Unit.Pixel(20)

            CalendarFrom.TitleStyle.BorderStyle = BorderStyle.None
            CalendarFrom.TitleStyle.ForeColor = Color.White
            CalendarFrom.TitleStyle.BackColor = Color.Gray
            CalendarFrom.TitleStyle.Font.Bold = True

            CalendarFrom.BackColor = Color.FromArgb(230, 230, 230)
            CalendarFrom.ForeColor = Color.Blue
            CalendarFrom.BorderStyle = BorderStyle.None
            CalendarFrom.DayHeaderStyle.BackColor = Color.FromArgb(210, 210, 210)
            CalendarFrom.DayHeaderStyle.ForeColor = Color.Blue

            CalendarFrom.TodayDayStyle.BorderStyle = BorderStyle.Solid
            CalendarFrom.TodayDayStyle.BorderWidth = Unit.Pixel(1)
            CalendarFrom.TodayDayStyle.BorderColor = Color.Blue
            CalendarFrom.SelectedDayStyle.Font.Bold = True
            CalendarFrom.SelectedDayStyle.BackColor = Color.Red
            CalendarFrom.OtherMonthDayStyle.ForeColor = Color.LightBlue

            CalendarTo.Width = Unit.Pixel(170)
            CalendarTo.Font.Name = "MS Sans Serif"
            CalendarTo.Font.Size = FontUnit.Point(9)
            CalendarTo.NextPrevFormat = NextPrevFormat.CustomText
            CalendarTo.NextMonthText = ">>>"
            CalendarTo.PrevMonthText = "<<<"
            CalendarTo.NextPrevStyle.BorderStyle = BorderStyle.None
            CalendarTo.NextPrevStyle.Font.Bold = True
            CalendarTo.NextPrevStyle.BackColor = Color.Gray
            CalendarTo.NextPrevStyle.ForeColor = Color.White
            CalendarTo.NextPrevStyle.Height = Unit.Pixel(20)

            CalendarTo.TitleStyle.BorderStyle = BorderStyle.None
            CalendarTo.TitleStyle.BackColor = Color.Gray
            CalendarTo.TitleStyle.Font.Bold = True
            CalendarTo.TitleStyle.ForeColor = Color.White

            CalendarTo.BackColor = Color.FromArgb(230, 230, 230)
            CalendarTo.ForeColor = Color.Blue
            CalendarTo.BorderStyle = BorderStyle.None
            CalendarTo.DayHeaderStyle.BackColor = Color.FromArgb(210, 210, 210)
            CalendarTo.DayHeaderStyle.ForeColor = Color.Blue

            CalendarTo.TodayDayStyle.BorderStyle = BorderStyle.Solid
            CalendarTo.TodayDayStyle.BorderWidth = Unit.Pixel(1)
            CalendarTo.TodayDayStyle.BorderColor = Color.Blue
            CalendarTo.SelectedDayStyle.Font.Bold = True
            CalendarTo.SelectedDayStyle.BackColor = Color.Red
            CalendarTo.OtherMonthDayStyle.ForeColor = Color.LightBlue
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default page options (server groups, dates)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub PagesLoad()
            PrintPage.Visible = False
            Server.ScriptTimeout = 10000
            ServerGroupsPrintTable.CellPadding = 0
            ServerGroupsPrintTable.CellSpacing = 0
            IsPrintView = True
            Try
                If TextboxDateFrom.Text = "" Then
                    CalendarFrom.VisibleDate = DateTime.Now.AddDays(-1)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                    TextboxDateFrom.Text = CalendarFrom.VisibleDate
                Else
                    CalendarFrom.VisibleDate = CType(TextboxDateFrom.Text, DateTime)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                End If
                If TextboxDateTo.Text = "" Then
                    CalendarTo.VisibleDate = DateTime.Now
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                    TextboxDateTo.Text = CalendarTo.VisibleDate
                Else
                    CalendarTo.VisibleDate = CType(TextboxDateTo.Text, DateTime)
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                End If
            Catch
            End Try
            CellFrom.Text = "<b>From: </b>" & TextboxDateFrom.Text
            CellTo.Text = "<b>To: </b>" & TextboxDateTo.Text
            Dim ServerInfo As CompuMaster.camm.WebManager.WMSystem.ServerInformation()
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    ServerInfo = cammWebManager.System_GetServersInfo(ServerGroups(i).ID)
                    ServerGroupIPs(i, j) = ServerInfo(j).IPAddressOrHostHeader
                Next
                CheckBoxListServerGroups.Items.Add(ServerGroups(i).Title)
                CheckBoxListServerGroups.Items(i).Selected = True
            Next
            CheckBoxCell.Controls.Add(CheckBoxListServerGroups)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Event for the button "Show report" 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            TopBorderTRow.Visible = True
            TitleTRow.Visible = True
            LeftTCell.Visible = True
            RightTCell.Visible = True
            cammlogo.Visible = True
            MenuBarTRow.Visible = True
            BottomBorderTRow.Visible = True
            SelectTable.Visible = True
            PrintViewTable.Visible = False
            IsPrintView = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Event for the linkbutton "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                If CheckBoxListServerGroups.Items(i).Selected Then
                    Dim row As New TableRow
                    Dim cell As New TableCell
                    cell.Text = CheckBoxListServerGroups.Items(i).Text
                    row.Controls.Add(cell)
                    ServerGroupsPrintTable.Controls.Add(row)
                    CellServerGroupsPrintTable.Controls.Add(ServerGroupsPrintTable)
                End If
            Next
            LinkButtonPrintView.Visible = False
            IsPrintView = False
            SelectTable.Visible = False
            PrintViewTable.Visible = True
            PrintPage.Visible = True
            TopBorderTRow.Visible = False
            TitleTRow.Visible = False
            LeftTCell.Visible = False
            RightTCell.Visible = False
            cammlogo.Visible = False
            MenuBarTRow.Visible = False
            BottomBorderTRow.Visible = False
            AppNameCell.Visible = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Select the date from the calendar "From" 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub CalendarFrom_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim CalendarDateFrom As DateTime
            CalendarDateFrom = CalendarFrom.SelectedDate
            CalendarFrom.VisibleDate = CalendarDateFrom
            TextboxDateFrom.Text = CalendarDateFrom
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Select the date from the calendar "To"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub CalendarTo_SelectionChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim CalendarDateTo As DateTime
            CalendarDateTo = CalendarTo.SelectedDate
            CalendarTo.VisibleDate = CalendarDateTo
            TextboxDateTo.Text = CalendarDateTo
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Border and title for the page 
        ''' </summary>
        ''' <param name="title"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	24.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Function getCWMHeader(ByVal title As String) As Table
            Dim returnTable As New Table
            Dim tRow As TableRow
            Dim tCell As TableCell
            Dim img As System.Web.UI.WebControls.Image
            returnTable.BorderStyle = BorderStyle.None
            returnTable.Width = Unit.Percentage(100)
            returnTable.Height = Unit.Pixel(20)
            returnTable.CellPadding = 0
            returnTable.CellSpacing = 0
            returnTable.Rows.Clear()
            If title = "" Then
                tRow = New TableRow
                tCell = New TableCell
                tCell.Width = Unit.Percentage(100)
                tCell.Height = Unit.Pixel(20)
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.Attributes.Add("background", "/system/admin/logs/images/img_style_tablebar-background_4x4048.gif")
                tCell.ColumnSpan = 1
                tCell.Text = "&nbsp;"
                tRow.Cells.Add(tCell)
                returnTable.Rows.Add(tRow)
            Else
                tRow = New TableRow
                tCell = New TableCell
                tCell.Width = Unit.Pixel(20)
                tCell.Height = Unit.Pixel(20)
                tCell.HorizontalAlign = HorizontalAlign.Center
                tCell.Attributes.Add("background", "/system/admin/logs/images/img_style_tablebar-background_4x4048.gif")
                tCell.ColumnSpan = 1
                img = New System.Web.UI.WebControls.Image
                img.ImageUrl = "/system/admin/logs/images/camm_wm_fileicon.gif"
                img.BorderStyle = BorderStyle.None
                tCell.Controls.Add(img)
                tRow.Cells.Add(tCell)
                tCell = New TableCell
                tCell.Width = Unit.Percentage(100)
                tCell.Height = Unit.Pixel(20)
                tCell.HorizontalAlign = HorizontalAlign.Left
                tCell.Attributes.Add("background", "/system/admin/logs/images/img_style_tablebar-background_4x4048.gif")
                tCell.ForeColor = System.Drawing.Color.White
                tCell.Font.Bold = True
                tCell.ColumnSpan = 1
                tCell.Text = "&nbsp;" & title
                tRow.Cells.Add(tCell)
                returnTable.Rows.Add(tRow)
            End If
            Return returnTable
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.Start_page
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''    Start page
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Start_page
        Inherits Page

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            TitleTCell.Controls.Add(getCWMHeader("Menu"))
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ApplicationClicks
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Application clicks"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ApplicationClicks
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected ApplicationTotalsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationTotalsControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Application clicks"))
            AppNameCell.Text = "APPLICATION CLICKS"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                ApplicationTotalsControl.cammWebManager = cammWebManager
                ApplicationTotalsControl.LoadData(DateFrom, DateTo, ServerIP, False, True, IsPrintView) 'application clicks control
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.UserClicks
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "User clicks" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserClicks
        Inherits Page


        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected UserClicksControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UserClicksControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("User clicks"))
            AppNameCell.Text = "USER CLICKS"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                UserClicksControl.cammWebManager = cammWebManager
                UserClicksControl.LoadData(DateFrom, DateTo, ServerIP, False, True, IsPrintView) 'user clicks
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ApplicationClicksByUsers
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Application clicks by users"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ApplicationClicksByUsers
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected UserList As DropDownList
        Protected UserSelectTable As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UserTable


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Application clicks by users"))
            AppNameCell.Text = "APPLICATION CLICKS MADE BY USERS"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Filling of the dropdown list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub DropDown()
            Dim arr(1) As String
            If UserList.SelectedIndex <> -1 Then 'if the data is selected
                If UserList.SelectedIndex = 1 Then 'if "All applications" is selected
                    For i As Integer = 2 To UserList.Items.Count - 1
                        UserList.SelectedIndex = i
                        ReDim Preserve arr(i - 2) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        arr(i - 2) = UserList.SelectedItem.Text 'array with all applications
                    Next
                    UserList.SelectedIndex = 1
                    UserSelectTable.AllApplications(arr, DateFrom, DateTo, ServerIP, IsPrintView)
                Else
                    UserSelectTable.LoadData(UserList.SelectedItem.Text, DateFrom, DateTo, ServerIP, IsPrintView, False) 'if one application is selected
                End If
            Else
                If Me.ViewState("_DataAlreadyLoaded") = False Then
                    Dim constr As String = cammWebManager.ConnectionString
                    Dim myCon As New SqlConnection(constr)
                    Dim selectSQL As String
                    selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select title from applications where " & _
                      "systemapp<>1 group by title"
                    Dim cmd As New SqlCommand(selectSQL, myCon)
                    Dim reader As SqlDataReader = Nothing
                    Dim arr_len As Integer
                    Dim arr_name(1) As String
                    Try
                        myCon.Open()
                        reader = cmd.ExecuteReader()
                        Do While reader.Read()
                            arr_name(arr_len) = reader(0) 'title
                            arr_len += 1
                            ReDim Preserve arr_name(arr_len) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        Loop
                        arr_len -= 1
                    Catch ex As Exception
                        cammWebManager.Log.Warn(ex)
                    Finally
                        If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                            reader.Close()
                        End If
                        If Not cmd Is Nothing Then
                            cmd.Dispose()
                        End If
                        If Not myCon Is Nothing Then
                            If myCon.State <> ConnectionState.Closed Then
                                myCon.Close()
                            End If
                            myCon.Dispose()
                        End If
                    End Try
                    Me.ViewState("name") = arr_name
                    Me.ViewState("len") = arr_len
                    Me.ViewState("_DataAlreadyLoaded") = True
                End If
                UserList.Items.Add("(Select)")
                UserList.Items(0).Value = 0
                UserList.Items.Add("(All applications)")
                For i As Integer = 0 To Me.ViewState("len")
                    UserList.Items.Add(Me.ViewState("name")(i))
                Next
                UserList.SelectedIndex = 0
                UserSelectTable.LoadData(UserList.SelectedItem.Text, DateFrom, DateTo, ServerIP, IsPrintView, False) 'if application is not selected
            End If
            If IsPrintView Then
                UserList.Visible = True
            Else
                UserList.Visible = False
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim ischeckbox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not ischeckbox Then
                                ischeckbox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not ischeckbox Then ServerIP = ""
                DropDown()
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ApplicationHistory
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Application history"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ApplicationHistory
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected ApplicationHistoryControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationHistoryControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Application history"))
            AppNameCell.Text = "APPLICATION HISTORY"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                ApplicationHistoryControl.cammWebManager = cammWebManager
                ApplicationHistoryControl.LoadData(DateFrom, DateTo, ServerIP, IsPrintView) 'application history
            End If
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.CurrentActivity
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Current Activity"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class CurrentActivity
        Inherits Page

        Dim ServerIP As String
        Protected OnlineMomentControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.OnlineMomentControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Styles for the button and a title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            ButtonShowReport.Attributes.Add("style", "color:White;background-color:#648BED;border-color:White;border-width:1px;border-style:Outset;")
            ButtonShowReport.Attributes.Add("ONMOUSEOVER", "this.style.backgroundColor='#EBF1FA'; this.style.borderColor='#648BED';this.style.color='#648BED';")
            ButtonShowReport.Attributes.Add("ONMOUSEOUT", "this.style.backgroundColor='#648BED';this.style.color='white';")
            ButtonShowReport.Font.Bold = True
            TitleTCell.Controls.Add(getCWMHeader("Current activity"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Server.ScriptTimeout = 600
            Dim ServerInfo As CompuMaster.camm.WebManager.WMSystem.ServerInformation()
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    ServerInfo = cammWebManager.System_GetServersInfo(ServerGroups(i).ID)
                    ServerGroupIPs(i, j) = ServerInfo(j).IPAddressOrHostHeader
                Next
                CheckBoxListServerGroups.Items.Add(ServerGroups(i).Title)
                CheckBoxListServerGroups.Items(i).Selected = True
            Next
            CheckBoxCell.Controls.Add(CheckBoxListServerGroups)
            CheckBoxCell.Controls.Add(New LiteralControl("<br>"))
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            ServerIP = " and (serverip = N'"
            Dim IsCheckBox As Boolean = False
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    If CheckBoxListServerGroups.Items(i).Selected Then
                        If Not IsCheckBox Then
                            IsCheckBox = True
                            ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                        Else
                            ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                        End If
                    End If
                Next
            Next
            ServerIP &= ")"
            If Not IsCheckBox Then ServerIP = ""
            OnlineMomentControl.cammWebManager = cammWebManager
            OnlineMomentControl.LoadData(ServerIP, True) 'current activity
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.DeletedUsersList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Deleted users - list of users"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DeletedUsersList
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected DeletedUsersListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.DeletedUsersListControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Deleted users - List of users"))
            AppNameCell.Text = "DELETED USERS - LIST"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                DeletedUsersListControl.cammWebManager = cammWebManager
                DeletedUsersListControl.LoadData(DateFrom, DateTo, ServerIP, False, True) 'deleted users - list
            End If
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.DeletedUsersOverview
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Deleted users - overview" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DeletedUsersOverview
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected NumberDeletedUsersControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberDeletedUsersControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Deleted users - Overview"))
            AppNameCell.Text = "DELETED USERS - OVERVIEW"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                NumberDeletedUsersControl.cammWebManager = cammWebManager
                NumberDeletedUsersControl.LoadData(DateFrom, DateTo, ServerIP, True) 'deleted users - overview
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.EventLog
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Event log" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class EventLog
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected EventlogDatagridControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventlogDatagridControl
        Protected EventLogControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventLogControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Event log"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Server.ScriptTimeout = 600
            Try
                If TextboxDateFrom.Text = "" Then
                    CalendarFrom.VisibleDate = DateTime.Now.AddDays(-3)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                    TextboxDateFrom.Text = CalendarFrom.VisibleDate
                Else
                    CalendarFrom.VisibleDate = CType(TextboxDateFrom.Text, DateTime)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                End If
                If TextboxDateTo.Text = "" Then
                    CalendarTo.VisibleDate = DateTime.Now
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                    TextboxDateTo.Text = CalendarTo.VisibleDate
                Else
                    CalendarTo.VisibleDate = CType(TextboxDateTo.Text, DateTime)
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                End If
            Catch
            End Try
            Dim ServerInfo As CompuMaster.camm.WebManager.WMSystem.ServerInformation()
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    ServerInfo = cammWebManager.System_GetServersInfo(ServerGroups(i).ID)
                    ServerGroupIPs(i, j) = ServerInfo(j).IPAddressOrHostHeader
                Next
                CheckBoxListServerGroups.Items.Add(ServerGroups(i).Title)
                CheckBoxListServerGroups.Items(i).Selected = True
            Next
            CheckBoxCell.Controls.Add(CheckBoxListServerGroups)
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                EventLogControl.cammWebManager = cammWebManager
                EventlogDatagridControl.LoadData(EventLogControl.dataTable(DateFrom, DateTo, ServerIP, False)) 'event log
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.Info
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Popup window for report "Event log" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Info
        Inherits Page

        Dim ServerName As String

        Protected InfoTable As Table

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from the database and creating of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim id As Integer
            Dim ct As String
            Dim nm As String
            id = Request.QueryString("id")
            ct = Request.QueryString("ct") 'Conflict type
            nm = Request.QueryString("nm") 'User name
            Dim constr As String = cammWebManager.ConnectionString
            Dim selectSQL As String
            selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select * from log where id= @id"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL, myCon)
            cmd1.Parameters.Add("@ID", SqlDbType.Int).Value = CType(id, Integer)
            cmd1.CommandTimeout = 0
            Dim reader As SqlDataReader = Nothing
            Dim aid As Integer
            Dim url As String
            Dim desc As String
            Dim dat As DateTime
            Try
                myCon.Open()
                reader = cmd1.ExecuteReader()
                reader.Read()
                If reader(5) Is System.DBNull.Value Then
                    aid = -1
                Else
                    aid = reader(5) 'application id
                End If
                If reader(6) Is System.DBNull.Value Then
                    url = ""
                Else
                    url = reader(6) 'URL
                End If
                If reader(8) Is System.DBNull.Value Then
                    desc = ""
                Else
                    desc = reader(8) 'conflict description
                End If
                Dim rownew1 As New TableRow
                Dim cellUser As New TableCell
                cellUser.ColumnSpan = 2
                Dim lblUser As New Label
                lblUser.Text = nm
                cellUser.Controls.Add(New LiteralControl("<b>User: </b>"))
                cellUser.Controls.Add(lblUser)
                rownew1.Controls.Add(cellUser)
                InfoTable.Controls.Add(rownew1)
                dat = reader(2) 'logindate
                Dim rowNew2 As New TableRow
                Dim cellDate As New TableCell
                Dim lblDate As New Label
                lblDate.Text = dat.ToString("dd.MM.yy")
                cellDate.Controls.Add(New LiteralControl("<b>Date: </b>"))
                cellDate.Controls.Add(lblDate)
                rowNew2.Controls.Add(cellDate)
                Dim cellRIP As New TableCell
                Dim lblRemIP As New Label
                lblRemIP.Text = reader(3) 'remote IP
                cellRIP.Controls.Add(New LiteralControl("<b>RemoteIP: </b>"))
                cellRIP.Controls.Add(lblRemIP)
                rowNew2.Controls.Add(cellRIP)
                InfoTable.Controls.Add(rowNew2)
                Dim rowNew3 As New TableRow
                Dim cellTime As New TableCell
                Dim lblTime As New Label
                lblTime.Text = dat.ToString("T")
                cellTime.Controls.Add(New LiteralControl("<b>Time: </b>"))
                cellTime.Controls.Add(lblTime)
                rowNew3.Controls.Add(cellTime)
                Dim cellSg As New TableCell
                Dim lblSg As New Label
                lblSg.Text = serverGroup(reader(4)) 'server IP
                cellSg.Controls.Add(New LiteralControl("<b>Server group: </b>"))
                cellSg.Controls.Add(lblSg)
                rowNew3.Controls.Add(cellSg)
                InfoTable.Controls.Add(rowNew3)
                Dim rowNew4 As New TableRow
                Dim cellTyp As New TableCell
                Dim lblTyp As New Label
                lblTyp.Text = ct
                cellTyp.Controls.Add(New LiteralControl("<b>Typ: </b>"))
                cellTyp.Controls.Add(lblTyp)
                rowNew4.Controls.Add(cellTyp)
                Dim cellApp As New TableCell
                Dim lblApp As New Label
                lblApp.Text = appName(aid)
                cellApp.Controls.Add(New LiteralControl("<b>Application: </b>"))
                cellApp.Controls.Add(lblApp)
                rowNew4.Controls.Add(cellApp)
                InfoTable.Controls.Add(rowNew4)
                Dim rownew5 As New TableRow
                Dim cellSn As New TableCell
                cellSn.ColumnSpan = 2
                Dim lblSn As New Label
                lblSn.Text = ServerName 'server name
                cellSn.Controls.Add(New LiteralControl("<b>Server name: </b>"))
                cellSn.Controls.Add(lblSn)
                rownew5.Controls.Add(cellSn)
                InfoTable.Controls.Add(rownew5)
                If url <> "" Then
                    Dim rownew6 As New TableRow
                    Dim cellURL As New TableCell
                    cellURL.ColumnSpan = 2
                    Dim lblUrl As New Label
                    lblUrl.Text = url
                    cellURL.Controls.Add(New LiteralControl("<b>URL: </b>"))
                    cellURL.Controls.Add(lblUrl)
                    rownew6.Controls.Add(cellURL)
                    InfoTable.Controls.Add(rownew6)
                End If
                If desc <> "" Then
                    Dim rownew7 As New TableRow
                    Dim cellDesc As New TableCell
                    cellDesc.ColumnSpan = 2
                    Dim lblDesc As New Label
                    lblDesc.Text = Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(desc))
                    cellDesc.Controls.Add(New LiteralControl("<b>Description: </b><br>"))
                    cellDesc.Controls.Add(lblDesc)
                    rownew7.Controls.Add(cellDesc)
                    InfoTable.Controls.Add(rownew7)
                End If
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the application's name from camm Web-Manager
        ''' </summary>
        ''' <param name="app_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function appName(ByVal app_id As Integer) As String
            Try
                Dim app_info As New CompuMaster.camm.webmanager.WMSystem.SecurityObjectInformation(app_id, cammWebManager, True)
                appName = app_info.Name
            Catch ex As Exception
                appName = "Unknown"
            End Try
        End Function




        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the server's group from camm Web-Manager
        ''' </summary>
        ''' <param name="serverIP"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function serverGroup(ByVal serverIP As String) As String
            Dim sid As Integer
            Dim ServerGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
            Try
                Dim sInfo As New WMSystem.ServerInformation(serverIP, Me.cammWebManager)
                sid = sInfo.ID
                ServerName = sInfo.Description
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If sid = ServerGroups(i).Servers(j).ID Then
                            Return ServerGroups(i).Title
                        End If
                    Next
                Next
                Return Nothing
            Catch
                Return "undefined"
            End Try
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ListOfUsers
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Table of users for the report "Last login dates"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ListOfUsers
        Inherits Page

        Dim FirstName As String
        Dim LastName As String
        Dim Company As String
        Dim Country As String
        Dim Email As String
        Dim LoginName As String
        Dim Language(2) As String
        Dim Street As String
        Dim ZipCode As String
        Dim City As String
        Dim Membership(50) As String
        Dim UserName As String
        Dim Num_memb As Integer
        Dim du As Boolean
        Dim _WebManager As CompuMaster.camm.WebManager.WMSystem

        Protected LabelTitle, LabelIsUsers As Label
        Protected DataValues As Table

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Hold the camm Web-Manager instance created on the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            _WebManager = cammWebManager
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from the database and creating of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Server.ScriptTimeout = 600
            Dim id As Integer
            Dim ServerIP As String
            id = Request.QueryString("id")
            Dim sip As Array = CType(Session("sip"), Array)
            ServerIP = " and ("
            For i As Integer = 0 To sip.GetUpperBound(0)
                If i = 0 Then
                    ServerIP &= "serverip = N'" & Replace(sip(i), "'", "''") & "'"
                Else
                    ServerIP &= " or serverip = N'" & Replace(sip(i), "'", "''") & "'"
                End If
            Next
            ServerIP &= ")"
            If sip(0) = "" Then ServerIP = ""
            Dim DateFrom As DateTime
            Dim DateTo As DateTime
            Dim selectSQL As String
            Dim constr As String = cammWebManager.ConnectionString
            Select Case id
                Case 0
                    LabelTitle.Text = "Today"
                    DateFrom = DateTime.Today
                    DateTo = DateTime.Now
                Case 1
                    LabelTitle.Text = "Last 7 days"
                    DateFrom = DateTime.Today.AddDays(-7)
                    DateTo = DateTime.Today
                Case 2
                    LabelTitle.Text = "Last 8-14 days"
                    DateFrom = DateTime.Today.AddDays(-14)
                    DateTo = DateTime.Today.AddDays(-8)
                Case 3
                    LabelTitle.Text = "Last 15-30 days"
                    DateFrom = DateTime.Today.AddDays(-30)
                    DateTo = DateTime.Today.AddDays(-15)
                Case 4
                    LabelTitle.Text = "Last 31-90 days"
                    DateFrom = DateTime.Today.AddDays(-90)
                    DateTo = DateTime.Today.AddDays(-31)
                Case 5
                    LabelTitle.Text = "Last year"
                    DateFrom = DateTime.Today.AddYears(-1)
                    DateTo = DateTime.Now
                Case 6
                    LabelTitle.Text = "More than 1 year"
                    DateTo = DateTime.Today.AddYears(-1)
                    DateFrom = DateTo
                Case 7
                    LabelTitle.Text = "Account created by admin, but never logged in"
                    DateTo = DateTime.Today.AddYears(-1)
                    DateFrom = DateTo
            End Select
            Dim interval As String
            interval = " and logindate between @DateFrom and @DateTo "
            If id = 6 Then interval = " and logindate<@DateTo "
            selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select log.userid,logindate from log," & _
               "(Select userid from log where userid<>0" & _
               interval & ServerIP & " and (conflicttype=98 or conflicttype=0) and conflicttype<>-31 group by userid) as t " & _
               "where t.userid=log.userid and (conflicttype=1 or conflicttype=3)"
            If id = 7 Then selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select log.userid,logindate from log," & _
                 "(Select userid from log " & _
                 "where conflicttype=3" & ServerIP & _
                 "and userid not in " & _
                 "(Select userid from log where (conflicttype=98 or conflicttype=0) and conflicttype<>-31 group by userid)) as t " & _
                 "where t.userid=log.userid and (conflicttype=1 or conflicttype=3)"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlDataReader = Nothing
            Dim counter As Integer
            Try
                myCon.Open()
                reader = cmd1.ExecuteReader()
                Do While reader.Read()
                    If Not IsDBNull(reader(0)) Then
                        Dim t1 As New Table
                        Dim t2 As New Table
                        Dim t3 As New Table
                        t2.CellPadding = 0
                        t2.CellSpacing = 0
                        t3.CellPadding = 0
                        t3.CellSpacing = 0
                        Dim rowNew As New TableRow
                        Dim cellNew1 As New TableCell
                        Dim cellNew2 As New TableCell
                        Dim cellNew3 As New TableCell
                        Dim cellNew4 As New TableCell
                        If counter Mod 2 = 0 Then
                            rowNew.BackColor = Color.Silver
                        Else
                            rowNew.BackColor = Color.Gainsboro
                        End If
                        userInfo(reader(0)) 'user id
                        cellNew1.Text = UserName
                        cellNew2.Text = reader(1) 'logindate
                        If Not du Then
                            For i As Integer = 0 To Num_memb
                                Dim r1 As New TableRow
                                Dim c1 As New TableCell
                                c1.Text = Membership(i)
                                r1.Controls.Add(c1)
                                t1.Controls.Add(r1)
                            Next
                            cellNew3.Controls.Add(t1)
                            Dim lbl1 As New Label
                            Dim lbl2 As New Label
                            lbl1.Text = "<b>Name: </b>" & LastName & "<br>"
                            lbl1.Text &= "<b>Firstname: </b>" & FirstName & "<br>"
                            lbl1.Text &= "<b>Loginname: </b>" & LoginName & "<br>"
                            cellNew4.Controls.Add(lbl1)
                            Dim c2 As New TableCell
                            Dim c3 As New TableCell
                            Dim r2 As New TableRow
                            Dim addr As String = Street & "<br>" & ZipCode & " " & City
                            c2.Text = "<b>Address:&nbsp;</b>"
                            c2.VerticalAlign = 1
                            c3.Text = addr
                            r2.Controls.Add(c2)
                            r2.Controls.Add(c3)
                            t2.Controls.Add(r2)
                            cellNew4.Controls.Add(t2)
                            lbl2.Text = "<b>Country: </b>" & Country & "<br>"
                            lbl2.Text &= "<b>E-mail: </b>" & Email & "<br>"
                            cellNew4.Controls.Add(lbl2)
                            Dim r3 As New TableRow
                            Dim c4 As New TableCell
                            Dim c5 As New TableCell
                            c4.Text = "<b>Languages:&nbsp;</b>"
                            c4.VerticalAlign = 1
                            Dim lg As String = Language(0) & "<br>" & Language(1) & "<br>" & Language(2)
                            c5.Text = lg
                            r3.Controls.Add(c4)
                            r3.Controls.Add(c5)
                            t3.Controls.Add(r3)
                            cellNew4.Controls.Add(t3)
                        End If
                        rowNew.Controls.Add(cellNew1)
                        rowNew.Controls.Add(cellNew2)
                        rowNew.Controls.Add(cellNew3)
                        rowNew.Controls.Add(cellNew4)
                        DataValues.Controls.Add(rowNew)
                        counter += 1
                    End If
                Loop
                If counter = 0 Then
                    LabelIsUsers.Text = "No users"
                    DataValues.Visible = False
                Else
                    LabelIsUsers.Text = ""
                    DataValues.Visible = True
                End If
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's information from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub userInfo(ByVal user_id As Long)
            Dim gr As CompuMaster.camm.WebManager.WMSystem.GroupInformation()
            Dim User_Info As New CompuMaster.camm.WebManager.WMSystem.UserInformation(user_id, _WebManager, True)
            Try
                Try
                    gr = cammWebManager.System_GetUserInfo(user_id).Memberships
                    Num_memb = gr.GetUpperBound(0)
                    For i As Integer = 0 To Num_memb
                        Membership(i) = Server.HtmlEncode(gr(i).Name)
                    Next
                Catch
                    Num_memb = 0
                    Membership(0) = "&nbsp;"
                End Try
                FirstName = Server.HtmlEncode(User_Info.FirstName)
                LastName = Server.HtmlEncode(User_Info.LastName)
                Company = Server.HtmlEncode(User_Info.Company)
                Country = Server.HtmlEncode(User_Info.Country)
                Email = Server.HtmlEncode(User_Info.EMailAddress)
                LoginName = Server.HtmlEncode(User_Info.LoginName)
                Language(0) = Server.HtmlEncode(User_Info.PreferredLanguage1.LanguageName_English)
                Language(1) = Server.HtmlEncode(User_Info.PreferredLanguage2.LanguageName_English)
                Language(2) = Server.HtmlEncode(User_Info.PreferredLanguage3.LanguageName_English)
                Street = Server.HtmlEncode(User_Info.Street)
                ZipCode = Server.HtmlEncode(User_Info.ZipCode)
                City = Server.HtmlEncode(User_Info.Location)
                If LoginName = "" Then
                    UserName = Server.HtmlEncode(User_Info.FullName) & " (ID " & User_Info.IDLong & ")"
                Else
                    UserName = Server.HtmlEncode(User_Info.FullName) & " (" & LoginName & ")"
                End If
                du = False
            Catch
                du = True
            End Try
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.NewUsersDetails
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "New users - contact details"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NewUsersDetails
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected NewUsersDetailsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUsersDetailsControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("New users - Contact details"))
            AppNameCell.Text = "NEW USERS - DETAILS"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                NewUsersDetailsControl.cammWebManager = cammWebManager
                NewUsersDetailsControl.LoadData(DateFrom, DateTo, ServerIP, False, True) 'New users - details
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.NewUsersList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "New users - list of users"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NewUsersList
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected NewUserListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUserListControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("New users - List of users"))
            AppNameCell.Text = "LIST OF NEW USERS"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                NewUserListControl.cammWebManager = cammWebManager
                NewUserListControl.LoadData(DateFrom, DateTo, ServerIP, False, True, IsPrintView) 'New users - list
            End If
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.LatestLogonDates
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Latest logon dates"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	04.10.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LatestLogonDates
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected ModeList As DropDownList
        Protected LatestLogonDatesControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.LatestLogonDatesControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Latest logon dates"))
            AppNameCell.Text = "LATEST LOGON DATES"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            If ModeList.Items.Count = 0 Then
                ModeList.Items.Add("Logged users in selected timespan")
                ModeList.Items(0).Value = 1
                ModeList.Items.Add("Unlogged users in selected timespan")
                ModeList.Items(1).Value = 0
            End If
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                LatestLogonDatesControl.cammWebManager = cammWebManager
                LatestLogonDatesControl.LoadData(ModeList.SelectedItem.Value, DateFrom, DateTo, ServerIP, False, True, IsPrintView)
            End If
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.NewUsersOverview
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "New users - overview" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NewUsersOverview
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected NumberNewUsersControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberNewUsersControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("New users - Overview"))
            AppNameCell.Text = "NEW USERS - OVERVIEW"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                NumberNewUsersControl.cammWebManager = cammWebManager
                NumberNewUsersControl.LoadData(DateFrom, DateTo, ServerIP, True) 'New users - overview
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.Redirections
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Redirections" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Redirections
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime

        Protected RedirectionsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.RedirectionsControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Redirections"))
            AppNameCell.Text = "REDIRECTIONS"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            PrintPage.Visible = False
            Server.ScriptTimeout = 600
            IsPrintView = True
            If Not Request.QueryString("dFrom") = "" AndAlso TextboxDateFrom.Text = "" Then
                TextboxDateFrom.Text = Request.QueryString("dFrom")
            End If
            If Not Request.QueryString("dTo") = "" AndAlso TextboxDateTo.Text = "" Then
                TextboxDateTo.Text = Request.QueryString("dTo")
            End If

            Try
                If TextboxDateFrom.Text = "" Then
                    CalendarFrom.VisibleDate = DateTime.Now.AddDays(-3)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                    TextboxDateFrom.Text = CalendarFrom.VisibleDate
                Else
                    CalendarFrom.VisibleDate = CType(TextboxDateFrom.Text, DateTime)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                End If
                If TextboxDateTo.Text = "" Then
                    CalendarTo.VisibleDate = DateTime.Now
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                    TextboxDateTo.Text = CalendarTo.VisibleDate
                Else
                    CalendarTo.VisibleDate = CType(TextboxDateTo.Text, DateTime)
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                End If
            Catch
            End Try
            cellFrom.Text = "<b>From: </b>" & TextboxDateFrom.Text
            cellTo.Text = "<b>To: </b>" & TextboxDateTo.Text
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            TopBorderTRow.Visible = True
            TitleTRow.Visible = True
            LeftTCell.Visible = True
            RightTCell.Visible = True
            cammlogo.Visible = True
            MenuBarTRow.Visible = True
            BottomBorderTRow.Visible = True
            IsPrintView = True
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            LinkButtonPrintView.Visible = False
            IsPrintView = False
            SelectTable.Visible = False
            printViewTable.Visible = True
            PrintPage.Visible = True
            TopBorderTRow.Visible = False
            TitleTRow.Visible = False
            LeftTCell.Visible = False
            RightTCell.Visible = False
            cammlogo.Visible = False
            MenuBarTRow.Visible = False
            BottomBorderTRow.Visible = False
            AppNameCell.Visible = True
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                RedirectionsControl.cammWebManager = cammWebManager
                RedirectionsControl.LoadData(DateFrom, DateTo, False, True, IsPrintView) 'redirections
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ServerGroups
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Server groups" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ServerGroups
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime

        Protected ServerGroupsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ServerGroupsControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Server groups history"))
            AppNameCell.Text = "SERVER GROUPS HISTORY"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            PrintPage.Visible = False
            Server.ScriptTimeout = 600
            IsPrintView = True
            Try
                If TextboxDateFrom.Text = "" Then
                    CalendarFrom.VisibleDate = DateTime.Now.AddDays(-3)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                    TextboxDateFrom.Text = CalendarFrom.VisibleDate
                Else
                    CalendarFrom.VisibleDate = CType(TextboxDateFrom.Text, DateTime)
                    CalendarFrom.SelectedDate = CalendarFrom.VisibleDate
                End If
                If TextboxDateTo.Text = "" Then
                    CalendarTo.VisibleDate = DateTime.Now
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                    TextboxDateTo.Text = CalendarTo.VisibleDate
                Else
                    CalendarTo.VisibleDate = CType(TextboxDateTo.Text, DateTime)
                    CalendarTo.SelectedDate = CalendarTo.VisibleDate
                End If
            Catch
            End Try
            cellFrom.Text = "<b>From: </b>" & TextboxDateFrom.Text
            cellTo.Text = "<b>To: </b>" & TextboxDateTo.Text
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            TopBorderTRow.Visible = True
            TitleTRow.Visible = True
            LeftTCell.Visible = True
            RightTCell.Visible = True
            cammlogo.Visible = True
            MenuBarTRow.Visible = True
            BottomBorderTRow.Visible = True
            IsPrintView = True
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            LinkButtonPrintView.Visible = False
            IsPrintView = False
            SelectTable.Visible = False
            printViewTable.Visible = True
            PrintPage.Visible = True
            TopBorderTRow.Visible = False
            TitleTRow.Visible = False
            LeftTCell.Visible = False
            RightTCell.Visible = False
            cammlogo.Visible = False
            MenuBarTRow.Visible = False
            BottomBorderTRow.Visible = False
            AppNameCell.Visible = True
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerGroupsControl.cammWebManager = cammWebManager
                ServerGroupsControl.LoadData(DateFrom, DateTo, True, IsPrintView) 'server group history
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.UpdatedProfilesList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Updated user profiles - list of users"
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UpdatedProfilesList
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected UpdatedProfilesListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UpdatedProfilesListControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Updated user profiles - List of users"))
            AppNameCell.Text = "UPDATED USER PROFILES - LIST"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                UpdatedProfilesListControl.cammWebManager = cammWebManager
                UpdatedProfilesListControl.LoadData(DateFrom, DateTo, ServerIP, False, True, IsPrintView) 'updated user profiles - list
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.UpdatedProfilesOverview
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Updated user profiles - overview" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UpdatedProfilesOverview
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected NumberUpdatedUsersControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberUpdatedUsersControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Updated user profiles - Overview"))
            AppNameCell.Text = "UPDATED USER PROFILES - OVERVIEW"
            AppNameCell.Visible = False
            Me.CheckBoxListServerGroups.Visible = False
            Me.ServerGroupsPrintTable.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                NumberUpdatedUsersControl.cammWebManager = cammWebManager
                NumberUpdatedUsersControl.LoadData(DateFrom, DateTo, ServerIP, True, IsPrintView) 'updated user profiles - overview
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.User15Minutes
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Table with "Activity log" of a user
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class User15Minutes
        Inherits Page

        Protected cellTitle As TableCell
        Protected DataValues As Table

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from the database and creating of the table 
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim constr As String = cammWebManager.ConnectionString
            Response.AppendHeader("Refresh", 10)
            Dim myCon As New SqlConnection(constr)
            Dim b As Integer
            Dim rip As String
            Dim n As String
            b = CType(Request.QueryString("id"), Integer)
            n = Request.QueryString("name") 'User name
            rip = Request.QueryString("rip") 'Remote IP
            cellTitle.Text = n
            Dim remoteip As String
            If b = -1 Then
                remoteip = "remoteip=@remIP and "
            Else
                remoteip = ""
            End If
            Dim selectSQL1 As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select applicationid,conflicttype,logindate,conflictdescription,remoteip from log " & _
                            "where userid= @ID and " & _
                            remoteip & _
                            "datediff(mi, logindate, CURRENT_TIMESTAMP) < 15 " & _
                            "order by logindate desc"
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.Parameters.Add("@ID", SqlDbType.Int).Value = b
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@remIP", SqlDbType.VarChar)).Value = rip
            Dim reader As SqlDataReader = Nothing
            Dim rdr As Integer
            Dim counter As Integer
            Try
                myCon.Open()
                reader = cmd1.ExecuteReader()
                Do While reader.Read()
                    Dim rowNew As New TableRow
                    Dim cellNew1 As New TableCell
                    Dim cellNew2 As New TableCell
                    Dim cellNew3 As New TableCell
                    Dim cellNew4 As New TableCell
                    If counter Mod 2 = 0 Then
                        rowNew.BackColor = Color.Silver
                    Else
                        rowNew.BackColor = Color.Gainsboro
                    End If
                    If reader(0) Is System.DBNull.Value Then
                        rdr = -1
                    Else
                        rdr = reader(0) 'application id
                    End If
                    cellNew1.Text = appName(rdr)
                    cellNew2.Text = description(reader(1)) 'conflicttype
                    If reader(3) Is System.DBNull.Value Then
                        cellNew2.ToolTip = "No details"
                    Else
                        cellNew2.ToolTip = "Details: " & reader(3)
                    End If
                    cellNew3.Text = reader(2)
                    cellNew4.Text = reader(4)
                    rowNew.Controls.Add(cellNew1)
                    rowNew.Controls.Add(cellNew2)
                    rowNew.Controls.Add(cellNew3)
                    rowNew.Controls.Add(cellNew4)
                    DataValues.Controls.Add(rowNew)
                    counter += 1
                Loop
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the application's name from camm Web-Manager
        ''' </summary>
        ''' <param name="app_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function appName(ByVal app_id As Integer) As String
            If app_id = -1 Then
                appName = ""
            Else
                Dim app_info As New CompuMaster.camm.webmanager.WMSystem.SecurityObjectInformation(app_id, cammWebManager, True)
                appName = app_info.Name
            End If
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Descriptions of conflict's types
        ''' </summary>
        ''' <param name="ct"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function description(ByVal ct As Integer) As String
            Select Case ct
                Case -99
                    description = "Debug"
                Case -98
                    description = "Unsuccessfull login"
                Case -95
                    description = "Login locked temporary (PW check failed too often)"
                Case -31
                    description = "User deleted"
                Case -29
                    description = "User account temporary locked on document validation"
                Case -28
                    description = "User account disabled on document validation"
                Case -27
                    description = "Missing authorization on document validation"
                Case -26
                    description = "No valid login data (e.g. PW)"
                Case -25
                    description = "Missing login on document validation"
                Case -9
                    description = "User profile attributes changed by admin or the user itself"
                Case -8
                    description = "Authorizations of group modified"
                Case -7
                    description = "Authorizations of user modified indirectly via group membershipment"
                Case -6
                    description = "Authorizations of user modified"
                Case -5
                    description = "Account lock has been resetted by admin"
                Case -4
                    description = "User profile modified by the user itself"
                Case 0
                    description = "Page hit"
                Case 1
                    description = "User created by the user itself"
                Case 2
                    description = "Password requested for sending via e-mail"
                Case 3
                    description = "User created by admin"
                Case 4
                    description = "User profile modified by admin"
                Case 5
                    description = "User password modified by admin"
                Case 6
                    description = "User password modified by the user itself"
                Case 97
                    description = "Preparation for GUID Login"
                Case 98
                    description = "Login"
                Case 99
                    description = "Logout"
                Case -70
                    description = "Runtime information"
                Case -71
                    description = "Runtime warning"
                Case -72
                    description = "Runtime exception"
                Case -80
                    description = "Application information"
                Case -81
                    description = "Application warning"
                Case -82
                    description = "Application exception"
                Case Else
                    description = ""
            End Select
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ViewTraceLog
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "View trace log" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ViewTraceLog
        Inherits Page

        Dim ServerIP As String

        Protected LabelDescription As Label
        Protected ViewTraceLogControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ViewTraceLogControl
        Protected UserList As DropDownList

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Styles for the button and a title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            ButtonShowReport.Attributes.Add("style", "color:White;background-color:#648BED;border-color:White;border-width:1px;border-style:Outset;")
            ButtonShowReport.Attributes.Add("ONMOUSEOVER", "this.style.backgroundColor='#EBF1FA'; this.style.borderColor='#648BED';this.style.color='#648BED';")
            ButtonShowReport.Attributes.Add("ONMOUSEOUT", "this.style.backgroundColor='#648BED';this.style.color='white';")
            ButtonShowReport.Font.Bold = True
            TitleTCell.Controls.Add(getCWMHeader("Trace log"))
            AppNameCell.Text = "TRACE LOG"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            PrintPage.Visible = False
            Server.ScriptTimeout = 600
            ServerGroupsPrintTable.CellPadding = 0
            ServerGroupsPrintTable.CellSpacing = 0
            LabelDescription.Text = "This chart shows last 50 entries with application name, URL, remote IP, date and description of each selected user."
            Dim ServerInfo As CompuMaster.camm.WebManager.WMSystem.ServerInformation()
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    ServerInfo = cammWebManager.System_GetServersInfo(ServerGroups(i).ID)
                    ServerGroupIPs(i, j) = ServerInfo(j).IPAddressOrHostHeader
                Next
                CheckBoxListServerGroups.Items.Add(ServerGroups(i).Title)
                CheckBoxListServerGroups.Items(i).Selected = True
            Next
            CheckBoxCell.Controls.Add(CheckBoxListServerGroups)
            CheckBoxCell.Controls.Add(New LiteralControl("<br>"))
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Filling of the dropdown list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub DropDownUserList()
            If UserList.Items.Count = 0 Then
                ViewTraceLogControl.Visible = False
                Dim ln As String
                Dim user As String
                Dim constr As String = cammWebManager.ConnectionString
                Dim myCon As New SqlConnection(constr)
                Dim selectSQL As String
                selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid from log where userid>0 and userid " & _
                   "not in (select userid from log where conflicttype=-31) group by userid"
                Dim cmd1 As New SqlCommand(selectSQL, myCon)
                Dim reader As SqlDataReader = Nothing
                UserList.Items.Add("(Select)")
                Dim sl_user As New SortedList
                Try
                    myCon.Open()
                    reader = cmd1.ExecuteReader()
                    Do While reader.Read()
                        Try
                            ln = cammWebManager.System_GetUserInfo(reader("UserID")).LoginName
                            user = cammWebManager.System_GetUserInfo(reader("UserID")).CompleteName & _
                            " (" & ln & ")"
                            sl_user.Add(user, reader("UserID"))
                        Catch
                        End Try
                    Loop
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
                For i As Integer = 0 To sl_user.Count - 1
                    UserList.Items.Add(sl_user.GetKey(i))
                    UserList.Items(i + 1).Value = sl_user.GetByIndex(i)
                Next
            Else
                ViewTraceLogControl.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            ServerIP = " and (serverip = N'"
            Dim IsCheckBox As Boolean = False
            For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    If CheckBoxListServerGroups.Items(i).Selected Then
                        If Not IsCheckBox Then
                            IsCheckBox = True
                            ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                        Else
                            ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                        End If
                    End If
                Next
            Next
            ServerIP &= ")"
            If Not IsCheckBox Then ServerIP = ""
            DropDownUserList()
            ViewTraceLogControl.LoadData(ServerIP, UserList.SelectedItem.Text, UserList.SelectedItem.Value, IsPrintView) 'view trace log
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ReportMarketing
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Marketing department" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ReportMarketing
        Inherits Page

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        Protected ApplicationTotalsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationTotalsControl
        Protected UserClicksControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UserClicksControl
        Protected NumberClicksDifferentIntervals As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberClicksDifferentIntervals
        Protected NumberNewUsersControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberNewUsersControl
        Protected NumberDeletedUsersControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NumberDeletedUsersControl
        Protected ServerGroupsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ServerGroupsControl
        Protected RedirectionsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.RedirectionsControl

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Marketing department"))
            AppNameCell.Text = "MARKETING REPORT"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                ApplicationTotalsControl.cammWebManager = cammWebManager
                ApplicationTotalsControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'application clicks
                UserClicksControl.cammWebManager = cammWebManager
                UserClicksControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'user clicks
                NumberClicksDifferentIntervals.cammWebManager = cammWebManager
                NumberClicksDifferentIntervals.LoadData(DateFrom, DateTo, ServerIP, IsPrintView) 'number of clicks for different intervals
                NumberNewUsersControl.cammWebManager = cammWebManager
                NumberNewUsersControl.LoadData(DateFrom, DateTo, ServerIP, False) 'new users - overview
                NumberDeletedUsersControl.cammWebManager = cammWebManager
                NumberDeletedUsersControl.LoadData(DateFrom, DateTo, ServerIP, False) 'deleted users - overview
                RedirectionsControl.cammWebManager = cammWebManager
                RedirectionsControl.LoadData(DateFrom, DateTo, True, False, IsPrintView) 'redirections
                ServerGroupsControl.cammWebManager = cammWebManager
                ServerGroupsControl.LoadData(DateFrom, DateTo, False, IsPrintView) 'server group history
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ReportSales
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Sales department" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ReportSales
        Inherits Page

        Protected NewUsersDetailsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUsersDetailsControl
        Protected DeletedUsersListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.DeletedUsersListControl
        Protected LastLoginDatesControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.LastLoginDatesControl

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Sales department"))
            AppNameCell.Text = "SALES REPORT"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                MyBase.PagesLoad()
            Catch ex As Exception

            End Try

            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                Dim counter As Integer
                Dim sip(0) As String
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                            ReDim Preserve sip(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            sip(counter) = ServerGroupIPs(i, j)
                            counter += 1
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                NewUsersDetailsControl.cammWebManager = cammWebManager
                NewUsersDetailsControl.LoadData(DateFrom, DateTo, ServerIP, True, False) 'new users - details
                DeletedUsersListControl.cammWebManager = cammWebManager
                DeletedUsersListControl.LoadData(DateFrom, DateTo, ServerIP, True, False) 'deleted users - list
                LastLoginDatesControl.cammWebManager = cammWebManager
                LastLoginDatesControl.LoadData(ServerIP, sip, IsPrintView) 'last login dates of users
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ReportSecurityAdministration
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Security administration" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ReportSecurityAdministration
        Inherits Page

        Protected OnlineMomentControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.OnlineMomentControl
        Protected LatestLogonDatesControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.LatestLogonDatesControl
        Protected UserClicksControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UserClicksControl
        Protected NewUserListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUserListControl
        Protected UpdatedProfilesListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.UpdatedProfilesListControl
        Protected DeletedUsersListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.DeletedUsersListControl
        Protected EventLogControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventLogControl
        Protected EventlogDatagridControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventlogDatagridControl

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Security administration"))
            AppNameCell.Text = "SECURITY ADMINISTRATION REPORT"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            OnlineMomentControl.Visible = False
            EventLogControl.Visible = False
            EventlogDatagridControl.Visible = False
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                LatestLogonDatesControl.cammWebManager = cammWebManager
                LatestLogonDatesControl.LoadData(1, DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'latest logon dates
                UserClicksControl.cammWebManager = cammWebManager
                UserClicksControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'user clicks
                NewUserListControl.cammWebManager = cammWebManager
                NewUserListControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'new users - list
                UpdatedProfilesListControl.cammWebManager = cammWebManager
                UpdatedProfilesListControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'updated user profiles - list
                DeletedUsersListControl.cammWebManager = cammWebManager
                DeletedUsersListControl.LoadData(DateFrom, DateTo, ServerIP, True, False) 'deleted users - list
                EventLogControl.cammWebManager = cammWebManager
                EventlogDatagridControl.LoadData(EventLogControl.dataTable(DateFrom, DateTo, ServerIP, True)) 'event log
                OnlineMomentControl.cammWebManager = cammWebManager
                OnlineMomentControl.LoadData(ServerIP, False) 'current activity
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ReportNetworkAdministration
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Network administration" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ReportNetworkAdministration
        Inherits Page

        Protected ApplicationTotalsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationTotalsControl
        Protected EventLogControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventLogControl
        Protected EventlogDatagridControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventlogDatagridControl
        Protected ServerGroupsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ServerGroupsControl

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Network administration"))
            AppNameCell.Text = "NETWORK ADMINISTRATION REPORT"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            EventLogControl.Visible = False
            EventlogDatagridControl.Visible = False
            AfterClick()
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip=N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                ApplicationTotalsControl.cammWebManager = cammWebManager
                ApplicationTotalsControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'application clicks
                EventLogControl.cammWebManager = cammWebManager
                EventlogDatagridControl.LoadData(EventLogControl.dataTable(DateFrom, DateTo, ServerIP, True)) 'event log
                ServerGroupsControl.cammWebManager = cammWebManager
                ServerGroupsControl.LoadData(DateFrom, DateTo, False, IsPrintView) 'server group history
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Pages.ReportWebmaster
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Report "Web-Master" 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	24.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ReportWebmaster
        Inherits Page

        Protected ApplicationTotalsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ApplicationTotalsControl
        Protected NewUserListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.NewUserListControl
        Protected DeletedUsersListControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.DeletedUsersListControl
        Protected EventLogControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventLogControl
        Protected EventlogDatagridControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.EventlogDatagridControl
        Protected ServerGroupsControl As CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls.ServerGroupsControl

        Dim DateFrom As DateTime
        Dim DateTo As DateTime
        Dim ServerIP As String

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title for the page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            MyBase.PagesInit()
            TitleTCell.Controls.Add(getCWMHeader("Web-Master"))
            AppNameCell.Text = "WEB-MASTER REPORT"
            AppNameCell.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            MyBase.PagesLoad()
            If Not IsPostBack Then AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "Show report"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.Button_Click(sender, e)
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Click on "print view"
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Sub LinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            MyBase.LinkButton_Click(sender, e)
            EventLogControl.Visible = False
            EventlogDatagridControl.Visible = False
            AfterClick()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Transfer entered data to user controls
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AfterClick()
            LabelDateFromError.Text = ""
            LabelDateToError.Text = ""
            Try
                DateTime.Parse(TextboxDateFrom.Text)
            Catch
                LabelDateFromError.Text = "Error!"
            End Try
            Try
                DateTime.Parse(TextboxDateTo.Text)
            Catch
                LabelDateToError.Text = "Error!"
            End Try
            If LabelDateFromError.Text = "" And LabelDateToError.Text = "" Then
                If TextboxDateFrom.Text <> "" Then
                    DateFrom = CType(TextboxDateFrom.Text, DateTime)
                Else
                    DateFrom = CalendarFrom.SelectedDate
                End If
                If TextboxDateTo.Text <> "" Then
                    DateTo = CType(TextboxDateTo.Text, DateTime)
                Else
                    DateTo = CalendarTo.SelectedDate
                End If
                If DateTo < DateFrom Then
                    LabelInterval.Text = "The date 'To' should be later than the date 'From'"
                    Exit Sub
                Else
                    LabelInterval.Text = ""
                End If
                ServerIP = " and (serverip = N'"
                Dim IsCheckBox As Boolean = False
                For i As Integer = 0 To cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                    ServerGroups = cammWebManager.System_GetServerGroupsInfo
                    For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                        If CheckBoxListServerGroups.Items(i).Selected Then
                            If Not IsCheckBox Then
                                IsCheckBox = True
                                ServerIP &= Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            Else
                                ServerIP &= " or serverip = N'" & Replace(ServerGroupIPs(i, j), "'", "''") & "'"
                            End If
                        End If
                    Next
                Next
                ServerIP &= ")"
                If Not IsCheckBox Then ServerIP = ""
                ApplicationTotalsControl.cammWebManager = cammWebManager
                ApplicationTotalsControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'application clicks
                NewUserListControl.cammWebManager = cammWebManager
                NewUserListControl.LoadData(DateFrom, DateTo, ServerIP, True, False, IsPrintView) 'new users - list
                DeletedUsersListControl.cammWebManager = cammWebManager
                DeletedUsersListControl.LoadData(DateFrom, DateTo, ServerIP, True, False) 'deleted users - list
                EventLogControl.cammWebManager = cammWebManager
                EventlogDatagridControl.LoadData(EventLogControl.dataTable(DateFrom, DateTo, ServerIP, True)) 'event log
                ServerGroupsControl.cammWebManager = cammWebManager
                ServerGroupsControl.LoadData(DateFrom, DateTo, False, IsPrintView) 'server group history
            End If
        End Sub

    End Class

End Namespace

Namespace CompuMaster.camm.WebManager.Modules.LogAnalysis.Controls

    Public Class BaseControl
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        Protected WM As CompuMaster.camm.WebManager.WMSystem
        Protected DateFrom As DateTime
        Protected DateTo As DateTime
        Protected CurrentDate As DateTime

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Control titles for "Summaries"
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overridable Function guiTable(ByVal name As String) As Table
            Dim rTable As New Table
            Dim rRow As New TableRow
            Dim rCell As New TableCell
            rTable.BorderStyle = BorderStyle.None
            rTable.Width = Unit.Percentage(100)
            rTable.Height = Unit.Pixel(20)
            rTable.CssClass = "Rt"
            rCell.Text = name
            rCell.HorizontalAlign = HorizontalAlign.Left
            rRow.Controls.Add(rCell)
            rTable.Controls.Add(rRow)
            Return rTable
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of a user name from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overridable Function userName(ByVal user_id As Long) As String
            Dim ln As String
            WM = cammWebManager
            Try
                ln = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).LoginName)
                userName = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).FullName) & _
                " (" & ln & ")"
            Catch ex As Exception
                If user_id = -1 Then
                    userName = "Unknown user"
                Else
                    Dim UserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(user_id, WM, True)
                    userName = Server.HtmlEncode(UserInfo.FullName) & _
                    " (ID " & user_id & ")"
                End If
            End Try
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.ApplicationTotalsControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - number of clicks for every application
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ApplicationTotalsControl
        Inherits BaseControl

        Protected LabelDescription As Label
        Protected LabelIsData As Label
        Protected PieChart1 As PieChart
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Application Totals"
            LabelDescription.Text = "This graphic shows the number of clicks for every application on the website. The data for selected " & _
            "periods and server groups is shown as graphics. The first diagram (pie chart) shows 10 applications with their maximum " & _
            "number of clicks. If the application has received less than 1% clicks, the value is not shown in the chart. Other " & _
            "applications are listed as 'others' and show the sum of all clicks they have received. If the number of visited applications " & _
            "is more than 10, all applications in the table are represented as horizontal diagrams."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a datebase
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP"></param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            Dim ii As Integer
            Dim s As Integer
            Dim s1 As Integer
            Dim arr_n(1) As String
            Dim arr_v(1) As String
            Dim others As Integer
            Dim arr_len As Integer
            If IsPrintView Then
                Dim selectSQL1 As String
                Dim constr As String = cammWebManager.ConnectionString
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select title, count(title) as Clicks from applications,log " & _
                   "where userid not in (select id_user from memberships where id_group=6)" & serverIP & _
                   " and logindate>=@DateFrom and logindate<=@DateTo" & _
                   " and applicationid=applications.id and systemapp<>1" & _
                   " and conflicttype=0" & _
                   " group by title order by count(title) desc"
                Dim myCon As New SqlConnection(constr)
                Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    Me.Visible = True
                    LabelIsData.Text = ""
                    PieChart1.Visible = True
                    reader = cmd1.ExecuteReader()
                    Do While reader.Read()
                        arr_v(ii) = reader(1) 'count(title)
                        If ii = 0 Then ViewState("max") = arr_v(ii)
                        If ii < 10 Then s += arr_v(ii)
                        s1 += arr_v(ii)
                        arr_n(ii) = Server.HtmlEncode(reader(0)) 'title
                        ii += 1
                        ReDim Preserve arr_v(ii) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve arr_n(ii) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Loop
                    reader.Close()
                    If s1 = 0 Then
                        LabelIsData.Text = "No data for current time period"
                        If sum_rep Then
                            Me.Visible = False
                        Else
                            Me.Visible = True
                        End If
                        PieChart1.Visible = False
                        DataValues.Visible = False
                        ViewState("counter") = 0
                    Else
                        ViewState("counter") = ii
                        ViewState("app_name") = arr_n
                        ViewState("clicks") = arr_v
                        If s1 - s = 0 Then
                            others = 0
                        Else
                            others = s1 - s
                        End If
                        If ii > 9 Then
                            arr_len = 9
                        Else
                            arr_len = ii - 1
                        End If
                        Dim k As Integer = arr_len + 1
                        Dim isothers As Boolean = False
                        For MyCounter As Integer = 0 To arr_len
                            If arr_v(MyCounter) / s1 < 0.01 And Not isothers Then
                                k = MyCounter
                                isothers = True
                            End If
                            If isothers Then
                                others += arr_v(MyCounter)
                            End If
                        Next
                        ReDim Preserve arr_n(k - 1)
                        ReDim Preserve arr_v(k - 1)
                        PieChart1.showpiechart(arr_n, arr_v, others)
                        ViewState("sum") = s1
                    End If
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            End If
            table_fill()
            If ViewState("counter") < 10 Then
                DataValues.Visible = False
            Else
                DataValues.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            Dim width As Integer
            Dim pr As Single
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                Dim cellNew3 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("app_name")(i)
                rowNew.Controls.Add(cellNew1)
                Dim diagrImg As New System.Web.UI.WebControls.Image
                Dim lbl1 As New Label
                diagrImg.ImageUrl = "./Images/1.gif"
                diagrImg.Height = Unit.Pixel(10)
                width = 85 * ViewState("clicks")(i) / ViewState("max")
                diagrImg.Width = Unit.Percentage(width)
                lbl1.Text = " " & ViewState("clicks")(i)
                cellNew2.Controls.Add(diagrImg)
                cellNew2.Controls.Add(lbl1)
                pr = ViewState("clicks")(i) / ViewState("sum") * 100
                cellNew3.Text = pr.ToString("##0.00")
                rowNew.Controls.Add(cellNew2)
                rowNew.Controls.Add(cellNew3)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.PieChart
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - creating of the piechart
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class PieChart
        Inherits BaseControl

        Private Shared colors As Color() = {Color.Red, Color.Lime, _
                  Color.Yellow, Color.Olive, Color.Blue, Color.Green, Color.Fuchsia, Color.Aqua, Color.Purple, _
                  Color.Gray, Color.Brown, Color.Gold, Color.MediumVioletRed, Color.Navy, Color.Maroon, Color.Salmon, _
                  Color.Indigo, Color.Orange, Color.Plum, Color.Violet, Color.Silver, Color.YellowGreen, Color.SandyBrown, _
                  Color.Tan, Color.Moccasin, Color.MediumSpringGreen, Color.Peru, Color.PaleVioletRed, Color.RosyBrown, Color.Sienna, _
                  Color.DarkOliveGreen, Color.DarkSlateBlue, Color.Aquamarine, Color.DarkRed, Color.CornflowerBlue, Color.Goldenrod, Color.Black, Color.MediumPurple, _
                  Color.BurlyWood, Color.CadetBlue, Color.Chartreuse, Color.Chocolate, Color.Coral, Color.MediumVioletRed, Color.Crimson, Color.DarkGoldenrod, _
                  Color.DarkKhaki, Color.DimGray, Color.Firebrick, Color.ForestGreen, Color.Honeydew, Color.Lavender, Color.LemonChiffon, Color.MistyRose, Color.PeachPuff}

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     create a piechart
        ''' </summary>
        ''' <param name="arr_name"></param>
        ''' <param name="arr_value"></param>
        ''' <param name="others"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub showpiechart(ByVal arr_name As Array, ByVal arr_value As Array, ByVal others As Integer)
            RadChart1.Clear()
            Dim s0 As ChartPieSeries = CType(RadChart1.GetChartSeries(0), ChartPieSeries)
            If s0 Is Nothing Then
                s0 = RadChart1.CreatePieSeries(Color.Blue, "Makes")
                s0.ShowValues = True
                s0.ShowPercent = True
                s0.PercentFormat = "##0.00"
                s0.ShowLabels = False
                s0.LabelDelimiter = " - "
                s0.DiameterScale = 0.6
            End If
            Dim sum As Integer
            Dim i As Integer
            For i = 0 To arr_value.GetUpperBound(0)
                s0.AddItem(arr_value(i), arr_name(i), colors(i))
                sum += arr_value(i)
            Next
            sum += others
            s0.AddItem(others, "others", colors(i))
            RadChart1.Title.DrawBorder = True
            RadChart1.Title.DrawBack = True
            RadChart1.Title.BackColor = Color.White
            RadChart1.Title.Text = "Total: " & sum
            RadChart1.Title.VAlignment = ChartVAlignment.Middle
            RadChart1.Title.HAlignment = ChartHAlignment.Center
            RadChart1.Title.Location = ChartLocation.InsideChart
        End Sub

#Region " Vom Web Form Designer generierter Code "

        'Dieser Aufruf ist f?r den Web Form-Designer erforderlich.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub
        <CLSCompliantAttribute(False)> Protected WithEvents RadChart1 As Telerik.WebControls.RadChart

        'HINWEIS: Die folgende Platzhalterdeklaration ist f?r den Web Form-Designer erforderlich.
        'Nicht l?schen oder verschieben.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: Dieser Methodenaufruf ist f?r den Web Form-Designer erforderlich
            'Verwenden Sie nicht den Code-Editor zur Bearbeitung.
            InitializeComponent()
        End Sub

#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.LineChart
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - creating of the linechart
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class LineChart
        Inherits BaseControl

        Private Shared colors As Color() = {Color.Red, Color.Lime, _
            Color.Yellow, Color.Olive, Color.Blue, Color.Green, Color.Fuchsia, Color.Aqua, Color.Purple, _
            Color.Gray, Color.Brown, Color.Gold, Color.MediumVioletRed, Color.Navy, Color.Maroon, Color.Salmon, _
            Color.Indigo, Color.Orange, Color.Plum, Color.Violet, Color.Silver, Color.YellowGreen, Color.SandyBrown, _
            Color.Tan, Color.Moccasin, Color.MediumSpringGreen, Color.Peru, Color.PaleVioletRed, Color.RosyBrown, Color.Sienna, _
            Color.DarkOliveGreen, Color.DarkSlateBlue, Color.Aquamarine, Color.DarkRed, Color.CornflowerBlue, Color.Goldenrod, Color.Black, Color.MediumPurple, _
            Color.BurlyWood, Color.CadetBlue, Color.Chartreuse, Color.Chocolate, Color.Coral, Color.MediumVioletRed, Color.Crimson, Color.DarkGoldenrod, _
            Color.DarkKhaki, Color.DimGray, Color.Firebrick, Color.ForestGreen, Color.Honeydew, Color.Lavender, Color.LemonChiffon, Color.MistyRose, Color.PeachPuff}

#Region " Vom Web Form Designer generierter Code "

        'Dieser Aufruf ist f?r den Web Form-Designer erforderlich.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub
        <CLSCompliantAttribute(False)> Protected WithEvents RadChart3 As Telerik.WebControls.RadChart

        'HINWEIS: Die folgende Platzhalterdeklaration ist f?r den Web Form-Designer erforderlich.
        'Nicht l?schen oder verschieben.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: Dieser Methodenaufruf ist f?r den Web Form-Designer erforderlich
            'Verwenden Sie nicht den Code-Editor zur Bearbeitung.
            InitializeComponent()
        End Sub

#End Region
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     create a linechart for "different intervals"
        ''' </summary>
        ''' <param name="arr_xaxis"></param>
        ''' <param name="arr_value"></param>
        ''' <param name="arr_ymax"></param>
        ''' <param name="arr_ystep"></param>
        ''' <param name="col"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub showlinechart(ByVal arr_xaxis As Array, ByVal arr_value As Array, ByVal arr_ymax As Double, ByVal arr_ystep As Double, ByVal col As Color)
            RadChart3.Clear()
            Dim s0 As ChartLineSeries = CType(RadChart3.GetChartSeries(0), ChartLineSeries)
            If s0 Is Nothing Then
                s0 = RadChart3.CreateLineSeries(Color.Red, "Makes")
                s0.ShowValues = False
            End If
            RadChart3.Title.Visible = False
            RadChart3.Legend.Visible = False
            RadChart3.Margins.Right = Unit.Pixel(20)
            RadChart3.XAxis.MarkLength = 2
            RadChart3.XAxis.LayoutStyle = ChartAxisLayoutStyle.Normal
            RadChart3.YAxis.AddRange(0, arr_ymax, arr_ystep)
            For i As Integer = 0 To arr_xaxis.GetUpperBound(0)
                RadChart3.XAxis.AddItem(arr_xaxis(i))
                s0.AddItem(arr_value(i), "", col)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     create a linechart for "application history"
        ''' </summary>
        ''' <param name="arr_xaxis"></param>
        ''' <param name="arr_name"></param>
        ''' <param name="arr_value"></param>
        ''' <param name="arr_ymax"></param>
        ''' <param name="arr_ystep"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub showlinechart2(ByVal arr_xaxis As Array, ByVal arr_name As Array, ByVal arr_value As Array, ByVal arr_ymax As Double, ByVal arr_ystep As Double)
            RadChart3.Clear()
            Dim s0 As ChartLineSeries = Nothing
            RadChart3.Height = Unit.Pixel(arr_name.GetUpperBound(0) * 10 + 320)
            RadChart3.Margins.Top = Unit.Pixel(arr_name.GetUpperBound(0) * 10)
            RadChart3.Margins.Right = Unit.Pixel(40)
            RadChart3.Title.Visible = False
            RadChart3.Legend.Visible = True
            RadChart3.Legend.BackgroundColor = Color.White
            RadChart3.XAxis.MarkLength = 2
            RadChart3.XAxis.LayoutStyle = ChartAxisLayoutStyle.Normal
            RadChart3.YAxis.AddRange(0, arr_ymax, arr_ystep)
            RadChart3.Legend.Location = ChartLocation.OutsideChart
            RadChart3.Legend.Position = ChartPosition.Top
            RadChart3.Legend.AutoSize = True
            For i As Integer = 0 To arr_xaxis.GetUpperBound(0)
                RadChart3.XAxis.AddItem(arr_xaxis(i))
            Next
            For j As Integer = 0 To arr_name.GetUpperBound(0)
                s0 = RadChart3.CreateLineSeries(colors(j), arr_name(j))
                s0.ShowValues = False
                s0.LineWidth = 2
                For i As Integer = 0 To arr_xaxis.GetUpperBound(0)
                    s0.AddItem(arr_value(j, i), arr_name(j))
                Next
            Next
        End Sub


    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.AreaChart
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - creating of the areachart
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class AreaChart
        Inherits BaseControl

        'Private Shared colors As Color() = {Color.LightGreen, Color.Bisque, Color.Aqua, Color.Aquamarine, Color.Azure, Color.Beige, Color.Crimson, Color.GreenYellow, Color.Ivory, Color.IndianRed, Color.LemonChiffon, Color.LightBlue, Color.LightGoldenrodYellow, Color.LightPink, Color.LimeGreen, Color.LightSteelBlue, Color.YellowGreen}
        Private Shared colors As Color() = {Color.Red, Color.Lime, _
          Color.Yellow, Color.Olive, Color.Blue, Color.Green, Color.Fuchsia, Color.Aqua, Color.Purple, _
          Color.Gray, Color.Brown, Color.Gold, Color.MediumVioletRed, Color.Navy, Color.Maroon, Color.Salmon, _
          Color.Indigo, Color.Orange, Color.Plum, Color.Violet, Color.Silver, Color.YellowGreen, Color.SandyBrown, _
          Color.Tan, Color.Moccasin, Color.MediumSpringGreen, Color.Peru, Color.PaleVioletRed, Color.RosyBrown, Color.Sienna, _
          Color.DarkOliveGreen, Color.DarkSlateBlue, Color.Aquamarine, Color.DarkRed, Color.CornflowerBlue, Color.Goldenrod, Color.Black, Color.MediumPurple, _
          Color.BurlyWood, Color.CadetBlue, Color.Chartreuse, Color.Chocolate, Color.Coral, Color.MediumVioletRed, Color.Crimson, Color.DarkGoldenrod, _
          Color.DarkKhaki, Color.DimGray, Color.Firebrick, Color.ForestGreen, Color.Honeydew, Color.Lavender, Color.LemonChiffon, Color.MistyRose, Color.PeachPuff}

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     create an areachart
        ''' </summary>
        ''' <param name="arr_xaxis"></param>
        ''' <param name="arr_value"></param>
        ''' <param name="arr_name"></param>
        ''' <param name="ymax"></param>
        ''' <param name="ystep"></param>
        ''' <param name="arr_len"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub showareachart(ByVal arr_xaxis As Array, ByVal arr_value As Array, ByVal arr_name As Array, ByVal ymax As Double, ByVal ystep As Double, ByVal arr_len As Integer)
            Dim currentSeries As ChartAreaSeries = Nothing
            RadChart4.Clear()
            RadChart4.Title.Visible = False
            RadChart4.XAxis.LayoutStyle = ChartAxisLayoutStyle.Normal
            RadChart4.Height = Unit.Pixel(400)
            RadChart4.Margins.Top = Unit.Pixel(arr_name.GetUpperBound(0) * 12)
            RadChart4.Margins.Right = Unit.Pixel(30)
            RadChart4.Legend.Location = ChartLocation.OutsideChart
            RadChart4.Legend.Position = ChartPosition.Top
            RadChart4.Legend.BackgroundColor = Color.White
            RadChart4.Legend.AutoSize = True
            For i As Integer = 0 To arr_xaxis.GetUpperBound(0)
                RadChart4.XAxis.AddItem(arr_xaxis(i))
            Next
            RadChart4.YAxis.AddRange(0, ymax, ystep)
            For j As Integer = 0 To arr_len
                currentSeries = RadChart4.CreateAreaSeries(colors(j), arr_name(j))
                For i As Integer = 0 To arr_xaxis.GetUpperBound(0)
                    currentSeries.ShowValues = False
                    currentSeries.AddItem(arr_value(j, i))
                Next
            Next
        End Sub

#Region " Vom Web Form Designer generierter Code "

        'Dieser Aufruf ist f?r den Web Form-Designer erforderlich.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub
        <CLSCompliantAttribute(False)> Protected WithEvents RadChart4 As Telerik.WebControls.RadChart

        'HINWEIS: Die folgende Platzhalterdeklaration ist f?r den Web Form-Designer erforderlich.
        'Nicht l?schen oder verschieben.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: Dieser Methodenaufruf ist f?r den Web Form-Designer erforderlich
            'Verwenden Sie nicht den Code-Editor zur Bearbeitung.
            InitializeComponent()
        End Sub

#End Region

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.BarChart
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - creating of the barchart
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class BarChart
        Inherits BaseControl

#Region " Vom Web Form Designer generierter Code "

        'Dieser Aufruf ist f?r den Web Form-Designer erforderlich.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub
        <CLSCompliantAttribute(False)> Protected WithEvents RadChart1 As Telerik.WebControls.RadChart
        <CLSCompliantAttribute(False)> Protected WithEvents RadChart2 As Telerik.WebControls.RadChart

        'HINWEIS: Die folgende Platzhalterdeklaration ist f?r den Web Form-Designer erforderlich.
        'Nicht l?schen oder verschieben.
        Private designerPlaceholderDeclaration As System.Object

        Private Sub PageOnInit(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: Dieser Methodenaufruf ist f?r den Web Form-Designer erforderlich
            'Verwenden Sie nicht den Code-Editor zur Bearbeitung.
            InitializeComponent()
        End Sub

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     create a barchart
        ''' </summary>
        ''' <param name="arr_xaxis"></param>
        ''' <param name="arr_value"></param>
        ''' <param name="arr_ymax"></param>
        ''' <param name="arr_ystep"></param>
        ''' <param name="col"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub showbarchart(ByVal arr_xaxis As Array, ByVal arr_value As Array, ByVal arr_ymax As Double, ByVal arr_ystep As Double, ByVal col As Color)
            RadChart2.Clear()
            Dim s0 As ChartBarSeries = CType(RadChart2.GetChartSeries(0), ChartBarSeries)
            If s0 Is Nothing Then
                s0 = RadChart2.CreateBarSeries(Color.Blue, "Makes")
            End If
            RadChart2.Title.Visible = False
            RadChart2.Legend.Visible = False
            RadChart2.YAxis.AddRange(0, arr_ymax, arr_ystep)
            For i As Integer = 0 To arr_xaxis.GetUpperBound(0)
                RadChart2.XAxis.AddItem(arr_xaxis(i))
                s0.AddItem(arr_value(i), "", col)
            Next
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.UserClicksControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - number of clicks made by each user
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserClicksControl
        Inherits BaseControl

        Protected LabelDescription, LabelIsData As Label
        Protected Piechart1 As PieChart
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "User clicks"
            LabelDescription.Text = "This graphic shows the total number of clicks made by each user in selected time periods and server groups. " & _
                "The first diagram (pie chart) shows 10 users with their maximum number of clicks. If the number of " & _
                "clicks made by user equals less than 1%, the value is not shown in the chart. Other users are listed as " & _
                "'others' and show the total sum of clicks they have made. If the number of users is more than 10, they are represented " & _
                "as horizontal diagrams in the table."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP"></param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            Dim arr_len As Integer
            Dim ii As Integer
            Dim s As Integer
            Dim s1 As Integer
            Dim arr_n(1) As String
            Dim arr_v(1) As Integer
            Dim others As Integer
            If IsPrintView Then
                Dim selectSQL1 As String
                Dim constr As String = cammWebManager.ConnectionString
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select UserID, count(id) as Clicks from log " & _
                   "where UserID not in (select id_user from memberships where id_group=6)" & serverIP & _
                   " and conflicttype=0" & _
                   " and logindate>=@DateFrom and logindate<=@DateTo" & _
                   " and applicationid not in (select id from applications where systemapp=1)" & _
                   " and applicationid<>0" & _
                   " group by UserID order by count(id) desc"
                Dim myCon As New SqlConnection(constr)
                Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    Me.Visible = True
                    LabelIsData.Text = ""
                    Piechart1.Visible = True
                    reader = cmd1.ExecuteReader()
                    Do While reader.Read()
                        If Not IsDBNull(reader(0)) Then
                            arr_v(ii) = reader(1) 'count(id)
                            If ii = 0 Then ViewState("max") = arr_v(ii)
                            If ii < 10 Then s += arr_v(ii)
                            s1 += arr_v(ii)
                            arr_n(ii) = userName(reader(0)) 'user id
                            ii += 1
                            ReDim Preserve arr_v(ii) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            ReDim Preserve arr_n(ii) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        End If
                    Loop
                    reader.Close()
                    If s1 = 0 Then
                        LabelIsData.Text = "No data for current time period"
                        Piechart1.Visible = False
                        DataValues.Visible = False
                        If sum_rep Then
                            Me.Visible = False
                        Else
                            Me.Visible = True
                        End If
                        ViewState("counter") = 0
                    Else
                        ViewState("counter") = ii
                        ViewState("user_name") = arr_n
                        ViewState("clicks") = arr_v
                        If s1 - s = 0 Then
                            others = 0
                        Else
                            others = s1 - s
                        End If
                        If ii > 9 Then
                            arr_len = 9
                        Else
                            arr_len = ii - 1
                        End If
                        Dim k As Integer = arr_len + 1
                        Dim isothers As Boolean = False
                        For MyCounter As Integer = 0 To arr_len
                            If arr_v(MyCounter) / s1 < 0.01 And Not isothers Then
                                k = MyCounter
                                isothers = True
                            End If
                            If isothers Then
                                others += arr_v(MyCounter)
                            End If
                        Next
                        ReDim Preserve arr_n(k - 1)
                        ReDim Preserve arr_v(k - 1)
                        Piechart1.showpiechart(arr_n, arr_v, others)
                        ViewState("sum") = s1
                    End If
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            End If
            table_fill()
            If ViewState("counter") < 10 Then
                DataValues.Visible = False
            Else
                DataValues.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            Dim width As Integer
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("user_name")(i)
                rowNew.Controls.Add(cellNew1)
                Dim diagrImg As New System.Web.UI.WebControls.Image
                Dim lbl1 As New Label
                diagrImg.ImageUrl = "./Images/1.gif"
                diagrImg.Height = Unit.Pixel(10)
                width = 85 * ViewState("clicks")(i) / ViewState("max")
                diagrImg.Width = Unit.Percentage(width)
                lbl1.Text = " " & ViewState("clicks")(i)
                cellNew2.Controls.Add(diagrImg)
                cellNew2.Controls.Add(lbl1)
                rowNew.Controls.Add(cellNew2)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub


    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.UserTable
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - application clicks made by users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UserTable
        Inherits BaseControl

        Dim ServerIP As String
        Dim Arr_Username(1) As String
        Dim Arr_Latest_date(1) As Date
        Dim Arr_Duration(1) As String
        Dim Arr_Clicks(1) As Integer
        Dim appn As String

        Protected LabelDescription, LabelIsVisits As Label
        Protected DataValues As Table

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     All applications mode
        ''' </summary>
        ''' <param name="Arr_app"></param>
        ''' <param name="dfr"></param>
        ''' <param name="dto"></param>
        ''' <param name="sr"></param>
        ''' <param name="ipv"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	04.10.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub AllApplications(ByVal Arr_app As Array, ByVal dfr As DateTime, ByVal dto As DateTime, ByVal sr As String, ByVal ipv As Boolean)
            For i As Integer = 0 To Arr_app.GetUpperBound(0)
                LoadData(Arr_app(i), dfr, dto, sr, ipv, True)
            Next
            DataValues.Visible = True
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="DateFrom1"></param>
        ''' <param name="DateTo1"></param>
        ''' <param name="s"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal n As String, ByVal DateFrom1 As DateTime, ByVal DateTo1 As DateTime, ByVal s As String, ByVal IsPrintView As Boolean, ByVal IsAllApp As Boolean)
            LabelDescription.Text = "This chart defines users who have visited the selected application during selected periods. " & _
                  "The application is selected from a list, and is then represented as user name, date " & _
                  "and time of last visit and duration of visits in the table. The number of clicks is also represented as a horizontal diagram."
            appn = n
            If appn = "(Select)" Then
                LabelIsVisits.Visible = False
                DataValues.Visible = False
            Else
                LabelIsVisits.Visible = True
                If IsPrintView Then
                    ServerIP = s
                    DateFrom = DateFrom1
                    DateTo = DateTo1
                    DateTo = DateTo.AddDays(1)
                    Dim constr As String = cammWebManager.ConnectionString
                    Dim myCon As New SqlConnection
                    myCon.ConnectionString = constr
                    Dim selectSQL1 As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select UserID, max(logindate) as latest_date," & _
                        "count(log.id) as Clicks from log,applications where " & _
                        "UserID not in (select id_user from memberships where id_group=6) and title = @Appn " & ServerIP & _
                        " and log.applicationid=applications.id" & _
                        " and conflicttype=0" & _
                        " and logindate between @DateFrom and @DateTo" & _
                        " group by UserID order by Clicks desc"
                    Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                    cmd1.Parameters.Add("@Appn", SqlDbType.NVarChar).Value = Replace(appn, "'", "''")

                    cmd1.CommandTimeout = 0
                    cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                    cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                    Dim reader As SqlDataReader = Nothing
                    Try
                        myCon.Open()
                        reader = cmd1.ExecuteReader()
                        Dim i As Integer
                        Do While reader.Read()
                            If Not IsDBNull(reader(0)) Then
                                ReDim Preserve Arr_Username(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                ReDim Preserve Arr_Latest_date(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                ReDim Preserve Arr_Duration(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                ReDim Preserve Arr_Clicks(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                Arr_Username(i) = userName(reader(0)) 'user id
                                Arr_Latest_date(i) = reader(1) 'logindate
                                If Not IsAllApp Then Arr_Duration(i) = duration(reader(0), appn)
                                Arr_Clicks(i) = reader(2) 'count(log.id)
                                If i = 0 Then ViewState("max" & appn) = Arr_Clicks(i)
                                i += 1
                            End If
                        Loop
                        ViewState("user_name" & appn) = Arr_Username
                        ViewState("latest_date" & appn) = Arr_Latest_date
                        If Not IsAllApp Then ViewState("duration" & appn) = Arr_Duration
                        ViewState("clicks" & appn) = Arr_Clicks
                        ViewState("counter" & appn) = i
                    Catch ex As Exception
                        cammWebManager.Log.Warn(ex)
                    Finally
                        If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                            reader.Close()
                        End If
                        If Not cmd1 Is Nothing Then
                            cmd1.Dispose()
                        End If
                        If Not myCon Is Nothing Then
                            If myCon.State <> ConnectionState.Closed Then
                                myCon.Close()
                            End If
                            myCon.Dispose()
                        End If
                    End Try
                End If
                table_fill(IsAllApp)
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill(ByVal iaa As Boolean)
            Dim i As Integer
            Dim width As Integer
            'table DataValues
            If ViewState("counter" & appn) <> 0 Then
                Dim rowEmpty As New TableRow
                Dim cellEmpty As New TableCell
                If iaa Then
                    cellEmpty.ColumnSpan = 3
                Else
                    cellEmpty.ColumnSpan = 4
                End If
                cellEmpty.BackColor = Color.White
                cellEmpty.Text = "&nbsp;"
                rowEmpty.Controls.Add(cellEmpty)
                DataValues.Controls.Add(rowEmpty)
                Dim rowTitle As New TableRow
                Dim cellTitle As New TableCell
                rowTitle.Font.Bold = True
                cellTitle.HorizontalAlign = HorizontalAlign.Center
                cellTitle.VerticalAlign = VerticalAlign.Middle
                cellTitle.ForeColor = Color.White
                cellTitle.BackColor = Color.Navy
                cellTitle.Font.Size = FontUnit.Larger
                If iaa Then
                    cellTitle.ColumnSpan = 3
                Else
                    cellTitle.ColumnSpan = 4
                End If
                cellTitle.Text = appn
                rowTitle.Controls.Add(cellTitle)
                DataValues.Controls.Add(rowTitle)
                Dim rowName As New TableRow
                rowName.BackColor = Color.FromArgb(192, 192, 255)
                rowName.HorizontalAlign = HorizontalAlign.Center
                rowName.VerticalAlign = VerticalAlign.Middle
                rowName.Font.Bold = True
                Dim cellUserName As New TableCell
                Dim cellAccessDate As New TableCell
                Dim cellDuration As New TableCell
                Dim cellClickRate As New TableCell
                cellUserName.Width = Unit.Percentage(30)
                cellClickRate.Width = Unit.Percentage(50)
                cellUserName.Text = "User name"
                cellAccessDate.Text = "Last access date"
                cellDuration.Text = "Duration"
                cellClickRate.Text = "Click rate"
                rowName.Controls.Add(cellUserName)
                rowName.Controls.Add(cellAccessDate)
                If Not iaa Then rowName.Controls.Add(cellDuration)
                rowName.Controls.Add(cellClickRate)
                DataValues.Controls.Add(rowName)
            End If
            For i = 0 To ViewState("counter" & appn) - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                Dim cellNew3 As New TableCell
                Dim cellNew4 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("user_name" & appn)(i)
                cellNew2.Text = ViewState("latest_date" & appn)(i)
                If Not iaa Then cellNew3.Text = ViewState("duration" & appn)(i)
                Dim diagrImg As New System.Web.UI.WebControls.Image
                Dim lbl As New Label
                diagrImg.ImageUrl = "./Images/8.gif"
                diagrImg.Height = Unit.Pixel(10)
                width = 85 * ViewState("clicks" & appn)(i) / ViewState("max" & appn)
                diagrImg.Width = Unit.Percentage(width)
                lbl.Text = " " & ViewState("clicks" & appn)(i)
                cellNew4.Controls.Add(diagrImg)
                cellNew4.Controls.Add(lbl)
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                If Not iaa Then rowNew.Controls.Add(cellNew3)
                rowNew.Controls.Add(cellNew4)
                DataValues.Controls.Add(rowNew)
            Next
            If ViewState("counter" & appn) = 0 And iaa = False Then
                LabelIsVisits.Text = "No visits"
                DataValues.Visible = False
            ElseIf ViewState("counter" & appn) = 0 And iaa = True Then
                LabelIsVisits.Text = ""
                DataValues.Visible = False
            Else
                LabelIsVisits.Text = ""
                DataValues.Visible = True
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Duration of stay of the user on page
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <param name="appname"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function duration(ByVal user_id As Long, ByVal appname As String) As String
            Dim constr As String = cammWebManager.ConnectionString
            Dim myCon1 As New SqlConnection(constr)
            Dim date0 As DateTime
            Dim date1 As DateTime
            Dim diff As TimeSpan
            Dim dur As TimeSpan
            Dim sec As Long
            Dim sum As Long
            Dim sel_date As String = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select title,logindate from log,applications where " & _
                                    "UserID=" & _
                                    user_id & _
                                    " and log.applicationid=applications.id" & _
                                    " and logindate between '" & DateFrom & "' and '" & DateTo & _
                                    "' order by logindate"
            Dim cmd_date As New SqlCommand(sel_date, myCon1)
            cmd_date.CommandTimeout = 0
            Dim read_date As SqlDataReader = Nothing
            Try
                myCon1.Open()
                read_date = cmd_date.ExecuteReader()
                Dim i As Boolean = False
                Do While read_date.Read()
                    If appname = read_date(0) Then
                        If Not i Then
                            date0 = read_date(1)
                            i = True
                        Else
                            date1 = read_date(1)
                            diff = date1.Subtract(date0)
                            sec = diff.TotalSeconds
                            If sec < 900 Then
                                sum += sec
                            Else
                                sum += 900
                                i = False
                            End If
                            date0 = date1
                        End If
                    ElseIf i Then
                        date1 = read_date(1)
                        diff = date1.Subtract(date0)
                        sec = diff.TotalSeconds
                        If sec < 900 Then
                            sum += sec
                        Else
                            sum += 900
                        End If
                        i = False
                    End If
                Loop
                If sec = 0 And i = True Then sum = 900
                dur = TimeSpan.FromSeconds(sum)
                If dur.TotalHours < 10 Then duration = "0"
                Dim Result As String
                Result = Int(dur.TotalHours) & ":"
                If dur.Minutes < 10 Then Result &= "0"
                Result &= dur.Minutes & ":"
                If dur.Seconds < 10 Then Result &= "0"
                Result &= dur.Seconds
                Return Result
            Finally
                If Not read_date Is Nothing AndAlso Not read_date.IsClosed Then
                    read_date.Close()
                End If
                If Not cmd_date Is Nothing Then
                    cmd_date.Dispose()
                End If
                If Not myCon1 Is Nothing Then
                    If myCon1.State <> ConnectionState.Closed Then
                        myCon1.Close()
                    End If
                    myCon1.Dispose()
                End If
            End Try
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.ApplicationHistoryControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - changing number of clicks for groups of applications
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ApplicationHistoryControl
        Inherits BaseControl

        Dim ServerIP As String
        Dim Arr_value(100, 100) As Integer
        Dim Arr_xaxis(1) As String
        Dim Ymax As Double
        Dim Ystep As Double
        Dim Arr_st(1) As String
        Dim Arr_name(1) As String
        Dim Arr_name1(1) As String

        Protected LabelDescription As Label
        Protected LineChart1 As LineChart


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            LabelDescription.Text = "This diagram shows the changing " & _
                  "number of clicks for groups of applications " & _
                  "on the website during selected periods."
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of application groups from a database
        ''' </summary>
        ''' <param name="DateFrom1"></param>
        ''' <param name="DateTo1"></param>
        ''' <param name="serverIP1"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom1 As DateTime, ByVal DateTo1 As DateTime, ByVal serverIP1 As String, ByVal IsPrintView As Boolean)
            If IsPrintView Then
                DateFrom = DateFrom1
                DateTo = DateTo1
                ServerIP = serverIP1
                Dim constr As String = cammWebManager.ConnectionString
                Dim myCon As New SqlConnection(constr)
                Dim selectSQL As String
                selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select id,case when charindex(' -',title)=0 then title " & _
                   "else left(title,charindex(' -',title)-1) " & _
                   "end as app_group from applications where systemapp<>1"
                Dim cmd1 As New SqlCommand(selectSQL, myCon)
                cmd1.CommandTimeout = 0
                Dim reader As SqlDataReader = Nothing
                Dim mode As String
                If Me.ViewState("_DataAlreadyLoaded") = False Then
                    Try
                        myCon.Open()
                        reader = cmd1.ExecuteReader()
                        Dim counter As Integer
                        Do While reader.Read()
                            For i As Integer = 0 To counter
                                If reader(1) = Arr_name(i) Then
                                    Arr_st(i) &= " or applicationid=" & reader(0)
                                    Exit For
                                ElseIf i = counter Then
                                    ReDim Preserve Arr_name(i + 1) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                    ReDim Preserve Arr_st(i + 1) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                    Arr_name(i) = reader(1) 'application group's name
                                    Arr_st(i) = "applicationid=" & reader(0) 'application's id
                                    counter += 1
                                    Exit For
                                End If
                            Next
                        Loop
                        ReDim Preserve Arr_name(counter - 1)
                        ReDim Preserve Arr_st(counter - 1)
                    Catch ex As Exception
                        cammWebManager.Log.Warn(ex)
                    Finally
                        If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                            reader.Close()
                        End If
                        If Not cmd1 Is Nothing Then
                            cmd1.Dispose()
                        End If
                        If Not myCon Is Nothing Then
                            If myCon.State <> ConnectionState.Closed Then
                                myCon.Close()
                            End If
                            myCon.Dispose()
                        End If
                    End Try
                    Me.ViewState("st") = Arr_st
                    Me.ViewState("name") = Arr_name
                    Me.ViewState("_DataAlreadyLoaded") = True
                End If
                For i As Integer = 0 To Me.ViewState("name").GetUpperBound(0)
                    ReDim Preserve Arr_name1(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    ReDim Preserve Arr_name(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Arr_name1(i) = Me.ViewState("name")(i)
                    Arr_name(i) = Arr_name1(i)
                Next
                mode = "Day"
                Dim CurrentDate1 As DateTime = DateTo.AddMonths(-2)
                Dim CurrentDate2 As DateTime = DateTo.AddMonths(-24)
                If CurrentDate1 > DateFrom And mode = "Day" Then
                    mode = "Month"
                ElseIf CurrentDate2 > DateFrom And mode = "Month" Then
                    mode = "Year"
                End If
                arrays(mode)
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the data for the linechart
        ''' </summary>
        ''' <param name="mode"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub arrays(ByVal mode As String)
            Dim diff As TimeSpan
            Dim days As Integer
            Dim k As Integer
            Dim scale_x As Integer
            diff = DateTo.Subtract(DateFrom)
            days = diff.Days
            axis_X(days, mode, k, scale_x)
            Dim ag As Integer
            Dim max As Single
            Dim arr_sum(1) As Integer
            ReDim Preserve arr_sum(Arr_name1.GetUpperBound(0))
            CurrentDate = DateFrom
            Dim constr As String = cammWebManager.ConnectionString

            Dim myCon1 As New SqlConnection(constr)
            Try
                myCon1.Open() 'often used in function "clicks" (used in this block)
                If mode = "Day" Then
                    For i As Integer = 0 To scale_x - 1
                        For j As Integer = 0 To k - 1
                            ReDim Preserve Arr_xaxis(i * k + j) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            If j = 0 Then
                                Arr_xaxis(i * k + j) = CurrentDate.ToString("dd.MM.yy")
                            Else
                                Arr_xaxis(i * k + j) = ""
                            End If
                            For ag = 0 To ViewState("name").GetUpperBound(0)
                                Arr_value(ag, i * k + j) = clicks(ag, mode, myCon1)
                                arr_sum(ag) += Arr_value(ag, i * k + j)
                                If Arr_value(ag, i * k + j) > max Then max = Arr_value(ag, i * k + j)
                            Next
                            CurrentDate = CurrentDate.AddDays(1)
                        Next
                    Next
                ElseIf mode = "Month" Then
                    For i As Integer = 0 To scale_x - 1
                        ReDim Preserve Arr_xaxis(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        Arr_xaxis(i) = CurrentDate.ToString("MMM yy")
                        For ag = 0 To ViewState("name").GetUpperBound(0)
                            Arr_value(ag, i) = clicks(ag, mode, myCon1)
                            arr_sum(ag) += Arr_value(ag, i)
                            If Arr_value(ag, i) > max Then max = Arr_value(ag, i)
                        Next
                        CurrentDate = CurrentDate.AddMonths(1)
                    Next
                Else
                    For i As Integer = 0 To scale_x - 1
                        ReDim Preserve Arr_xaxis(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        Arr_xaxis(i) = CurrentDate.Year
                        For ag = 0 To ViewState("name").GetUpperBound(0)
                            Arr_value(ag, i) = clicks(ag, mode, myCon1)
                            arr_sum(ag) += Arr_value(ag, i)
                            If Arr_value(ag, i) > max Then max = Arr_value(ag, i)
                        Next
                        CurrentDate = CurrentDate.AddYears(1)
                    Next
                End If
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not myCon1 Is Nothing Then
                    If myCon1.State <> ConnectionState.Closed Then
                        myCon1.Close()
                    End If
                    myCon1.Dispose()
                End If
            End Try
            Dim ii As Integer = 0
            Do While max >= 10
                max /= 10
                ii += 1
            Loop
            Ymax = (Int(max) + 1) * 10 ^ ii
            Ystep = 10 ^ ii
            If DateFrom = DateTo Then
                ReDim Preserve Arr_xaxis(1)
                Arr_xaxis(1) = DateFrom.AddDays(1)
            End If
            Array.Sort(arr_sum, Arr_name1)
            Dim ilimit As Integer
            If Arr_name1.GetUpperBound(0) >= 9 Then
                ilimit = Arr_name1.GetUpperBound(0) - 9
            Else
                ilimit = 0
            End If

            For i As Integer = Arr_name1.GetUpperBound(0) To ilimit Step -1
                For j As Integer = 0 To Arr_name1.GetUpperBound(0)
                    If Arr_name1(i) = Arr_name(j) Then
                        Arr_name(j) &= " (Top 10)"
                        Exit For
                    End If
                Next
            Next i
            LineChart1.showlinechart2(Arr_xaxis, Arr_name, Arr_value, Ymax, Ystep)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the data for horizontal axis
        ''' </summary>
        ''' <param name="days"></param>
        ''' <param name="mode"></param>
        ''' <param name="k"></param>
        ''' <param name="scale_x"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub axis_X(ByVal days As String, ByVal mode As String, ByRef k As Integer, ByRef scale_x As Integer)
            Dim i As Integer
            Dim m As Integer
            If mode = "Day" Then
                m = days
                If m / 12 < 1 Then
                    scale_x = m + 1
                    k = 1
                ElseIf m Mod 12 = 0 Then
                    scale_x = 13
                    k = m / 12
                Else
                    k = Int(m / 12) + 1
                    CurrentDate = DateFrom
                    Do While CurrentDate <= DateTo
                        CurrentDate = CurrentDate.AddDays(k)
                        i += 1
                    Loop
                    scale_x = i
                End If
            ElseIf mode = "Month" Then
                CurrentDate = DateFrom
                scale_x = 1
                Do While DateTo.Month <> CurrentDate.Month Or DateTo.Year <> CurrentDate.Year
                    scale_x += 1
                    CurrentDate = CurrentDate.AddMonths(1)
                Loop
            Else
                CurrentDate = DateFrom
                scale_x = 1
                Do While DateTo.Year <> CurrentDate.Year
                    scale_x += 1
                    CurrentDate = CurrentDate.AddYears(1)
                Loop
            End If
        End Sub



        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Number of clicks for the application group
        ''' </summary>
        ''' <param name="ag"></param>
        ''' <param name="mode"></param>
        ''' <returns></returns>
        ''' <remarks>
        '''     Internal service function for method "arrays"
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function clicks(ByVal ag As Integer, ByVal mode As String, ByVal connection As SqlConnection) As Integer
            Dim selectSQL1 As String
            If mode = "Day" Then
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(0) from log where " & _
                            "logindate>=@DateFrom" & _
                            " and logindate<@DateTo " & _
                            ServerIP & " and (" & ViewState("st")(ag) & ")" & _
                            " and conflicttype=0" & _
                            " and userid not in (select id_user from memberships where id_group=6)"
            ElseIf mode = "Month" Then
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(0) from log where " & _
                           "logindate>=@DateFrom" & _
                           " and logindate<=@DateTo" & _
                           " and month(logindate)=" & CurrentDate.Month & _
                           " and year(logindate)=" & CurrentDate.Year & _
                           ServerIP & " and (" & ViewState("st")(ag) & ")" & _
                           " and conflicttype=0" & _
                           " and userid not in (select id_user from memberships where id_group=6)"
            Else
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select count(0) from log where " & _
                            "logindate>=@DateFrom" & _
                            " and logindate<=@DateTo" & _
                            " and year(logindate)=" & CurrentDate.Year & _
                            ServerIP & " and (" & ViewState("st")(ag) & ")" & _
                            " and conflicttype=0" & _
                            " and userid not in (select id_user from memberships where id_group=6)"
            End If
            Dim cmd1 As New SqlCommand(selectSQL1, connection)
            cmd1.CommandTimeout = 0
            If mode = "Day" Then
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = CurrentDate
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = CurrentDate.AddDays(1)
            Else
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo.AddDays(1)
            End If
            Try
                clicks = cmd1.ExecuteScalar()
            Finally
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
            End Try

        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.EventLogControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - all selected events
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class EventLogControl
        Inherits BaseControl

        Protected CheckboxRuntimeWarnings, CheckboxRuntimeExceptions, CheckboxRuntimeInformation, CheckboxApplicationWarnings, CheckboxApplicationExceptions, CheckboxApplicationInformation, CheckboxLogin, ChecboxkLogout, CheckboxDebug As CheckBox
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Event Log"
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub



        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the data table
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP"></param>
        ''' <param name="lbl"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function dataTable(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal lbl As Boolean) As dataTable
            If lbl Then
                ReportNameCell.Visible = True
            Else
                ReportNameCell.Visible = False
            End If
            DateTo = DateTo.AddDays(1)
            If Not IsPostBack Then
                CheckboxRuntimeWarnings.Text = "Runtime Warnings"
                CheckboxRuntimeWarnings.Checked = True
                CheckboxRuntimeExceptions.Text = "Runtime Exceptions"
                CheckboxRuntimeExceptions.Checked = True
                CheckboxRuntimeInformation.Text = "Runtime Information"
                CheckboxRuntimeInformation.Checked = True
                CheckboxApplicationWarnings.Text = "Application Warnings"
                CheckboxApplicationWarnings.Checked = True
                CheckboxApplicationExceptions.Text = "Application Exceptions"
                CheckboxApplicationExceptions.Checked = True
                CheckboxApplicationInformation.Text = "Application Information"
                CheckboxApplicationInformation.Checked = False
                CheckboxLogin.Text = "Login"
                CheckboxLogin.Checked = False
                ChecboxkLogout.Text = "Logout"
                ChecboxkLogout.Checked = False
                CheckboxDebug.Text = "Debug"
                CheckboxDebug.Checked = False
            End If
            Dim constr As String = cammWebManager.ConnectionString
            Dim selectSQL As String
            Dim chk_state(8) As Boolean
            Dim cft(8) As Integer
            Dim chk_title(8) As String
            Dim conflict As String = Nothing
            If CheckboxRuntimeWarnings.Checked Then
                chk_title(0) = CheckboxRuntimeWarnings.Text
                chk_state(0) = True
                cft(0) = -71
            End If
            If CheckboxRuntimeExceptions.Checked Then
                chk_title(1) = CheckboxRuntimeExceptions.Text
                chk_state(1) = True
                cft(1) = -72
            End If
            If CheckboxRuntimeInformation.Checked Then
                chk_title(2) = CheckboxRuntimeInformation.Text
                chk_state(2) = True
                cft(2) = -70
            End If
            If CheckboxApplicationWarnings.Checked Then
                chk_title(3) = CheckboxApplicationWarnings.Text
                chk_state(3) = True
                cft(3) = -81
            End If
            If CheckboxApplicationExceptions.Checked Then
                chk_title(4) = CheckboxApplicationExceptions.Text
                chk_state(4) = True
                cft(4) = -82
            End If
            If CheckboxApplicationInformation.Checked Then
                chk_title(5) = CheckboxApplicationInformation.Text
                chk_state(5) = True
                cft(5) = -80
            End If
            If CheckboxLogin.Checked Then
                chk_title(6) = CheckboxLogin.Text
                chk_state(6) = True
                cft(6) = 98
            End If
            If ChecboxkLogout.Checked Then
                chk_title(7) = ChecboxkLogout.Text
                chk_state(7) = True
                cft(7) = 99
            End If
            If CheckboxDebug.Checked Then
                chk_title(8) = CheckboxDebug.Text
                chk_state(8) = True
                cft(8) = -99
            End If
            Dim isChk As Boolean
            For i As Integer = 0 To 8
                If chk_state(i) Then
                    If Not isChk Then
                        isChk = True
                        conflict = "conflicttype=" & cft(i)
                    Else
                        conflict &= " or conflicttype=" & cft(i)
                    End If
                End If
            Next
            Dim PostedValues As String = String.Empty
            For Each el As String In Request.Form
                If el = "chkbx" Then
                    PostedValues = Request.Form(el)
                End If
            Next
            Dim UpdateSQL As String = "Update log Set reviewedandclosed=1 where "
            Dim PostedValuesSplitted() As String = Split(PostedValues, ",")
            For i As Integer = 0 To PostedValuesSplitted.GetUpperBound(0)
                If PostedValuesSplitted(i) <> Nothing Then
                    If i = 0 Then
                        UpdateSQL &= "id=" & CType(PostedValuesSplitted(i), Integer)
                    Else
                        UpdateSQL &= " or id=" & CType(PostedValuesSplitted(i), Integer)
                    End If
                End If
            Next
            selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select id,userid,logindate,conflicttype,conflictdescription,isnull(reviewedandclosed,0) from log where " & _
               "logindate between @DateFrom and @DateTo " & _
               serverIP & " and (" & conflict & _
               ") and isnull(reviewedandclosed,0)=0 order by logindate desc"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL, myCon)
            Dim cmd2 As New SqlCommand(UpdateSQL, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlDataReader = Nothing
            Dim dtable As dataTable = New dataTable
            Dim drow As DataRow
            dtable.Columns.Add(New DataColumn("id", GetType(Integer)))
            dtable.Columns.Add(New DataColumn("nm", GetType(String)))
            dtable.Columns.Add(New DataColumn("ct", GetType(String)))
            dtable.Columns.Add(New DataColumn("ld", GetType(Date)))
            dtable.Columns.Add(New DataColumn("rc", GetType(CheckBox)))
            dtable.Columns.Add(New DataColumn("cd", GetType(String)))
            Try
                myCon.Open()
                Dim updated As Integer
                If PostedValues <> "" Then updated = cmd2.ExecuteNonQuery()
                reader = cmd1.ExecuteReader()
                Dim counter As Integer
                Do While reader.Read()
                    If Not IsDBNull(reader(1)) And Not IsDBNull(reader(3)) Then
                        drow = dtable.NewRow()
                        drow(0) = reader(0) 'id
                        drow(1) = userName(reader(1)) 'user id
                        drow(2) = typName(reader(3)) 'conflicttype
                        drow(3) = reader(2) 'logindate
                        drow(5) = reader(4) 'conflictdescription 
                        Dim CheckboxCloseIssue As New CheckBox
                        If reader(5) = 1 Then
                            CheckboxCloseIssue.Checked = True
                        Else
                            CheckboxCloseIssue.Checked = False
                        End If
                        drow(4) = CheckboxCloseIssue
                        dtable.Rows.Add(drow)
                        counter += 1
                        If counter > 1000 Then Exit Do
                    End If
                Loop
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
            dataTable = dtable
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Description of conflict's types
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function typName(ByVal id As Integer) As String
            Select Case id
                Case 98
                    Return "Login"
                Case 99
                    Return "Logout"
                Case -70
                    Return "Runtime Information"
                Case -71
                    Return "Runtime Warning"
                Case -72
                    Return "Runtime Exception"
                Case -80
                    Return "Application Information"
                Case -81
                    Return "Application Warning"
                Case -82
                    Return "Application Exception"
                Case -99
                    Return "Debug"
                Case Else
                    Return "{unexpected type}"
            End Select
        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.EventlogDatagridControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - creating of the datagrid
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class EventlogDatagridControl
        Inherits BaseControl

        Dim counter As Integer

        Protected LabelDescription, LabelIsData As Label
        Protected Datagrid1 As DataGrid

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Bind of the data
        ''' </summary>
        ''' <param name="DateToable"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateToable As DataTable)
            LabelDescription.Text = "This chart shows all selected events for current periods. Every row refers to the " & _
                  "page with information about a user which has called a event, date, server group and application. " & _
                  "The events marked by checkboxes are not displayed during following invocations."

            If DateToable.Rows.Count = 0 Then
                LabelIsData.Text = "No data for current time period"
                Datagrid1.Visible = False
            Else
                LabelIsData.Text = ""
                Datagrid1.Visible = True
                Dim dview As DataView = New DataView(DateToable)
                Datagrid1.DataSource = dview
                Datagrid1.DataBind()
                Datagrid1.Columns(0).Visible = False
                Datagrid1.Columns(5).Visible = False
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table with references
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub Cl_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs)
            If e.Item.ItemType = ListItemType.Header Then
                CType(e.Item.Cells(4).Controls(1), CheckBox).Attributes.Add("onclick", "selectAll();")
            End If
            If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                e.Item.Style("cursor") = "hand"
                e.Item.Attributes.Add("onmouseover", "this.style.backgroundColor='#8080FF';")
                If e.Item.Cells(5).Text.Length <= 80 Then
                    e.Item.ToolTip = e.Item.Cells(5).Text
                Else
                    e.Item.ToolTip = e.Item.Cells(5).Text.Substring(0, 80) & "..."
                End If
                e.Item.Cells(4).Style("cursor") = "default"
                e.Item.Cells(4).HorizontalAlign = HorizontalAlign.Center
                If counter Mod 2 = 0 Then
                    e.Item.Style("background-Color") = "silver"
                    e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='silver';")
                Else
                    e.Item.Style("background-Color") = "gainsboro"
                    e.Item.Attributes.Add("onmouseout", "this.style.backgroundColor='gainsboro';")
                End If
                e.Item.Cells(1).Attributes.Add("onclick", "popup('info.aspx?id=" & e.Item.Cells(0).Text & "&nm=" & e.Item.Cells(1).Text & "&ct=" & e.Item.Cells(2).Text & "');")
                e.Item.Cells(2).Attributes.Add("onclick", "popup('info.aspx?id=" & e.Item.Cells(0).Text & "&nm=" & e.Item.Cells(1).Text & "&ct=" & e.Item.Cells(2).Text & "');")
                e.Item.Cells(3).Attributes.Add("onclick", "popup('info.aspx?id=" & e.Item.Cells(0).Text & "&nm=" & e.Item.Cells(1).Text & "&ct=" & e.Item.Cells(2).Text & "');")
                counter += 1
            End If
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.LastLoginDatesControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - how many users have logged onto different intervals
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LastLoginDatesControl
        Inherits BaseControl

        Protected LabelDescription As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Last user login dates"
            LabelDescription.Text = "This chart shows how many users have logged onto different intervals. " & _
               "Each number in the table is a reference to a user showing user's name, contact data " & _
               "and date on which he was created."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database and creating of the table
        ''' </summary>
        ''' <param name="serverIP"></param>
        ''' <param name="sip"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal serverIP As String, ByVal sip As Array, ByVal IsPrintView As Boolean)
            Dim DateFrom As DateTime
            Dim DateTo As DateTime
            DateTo = DateTo.AddDays(1)
            Dim arr_1(7) As Integer
            Dim selectSQL As String
            Dim constr As String = cammWebManager.ConnectionString
            Dim myCon As New SqlConnection(constr)

            Try
                For i As Integer = 0 To 7
                    Dim rowNew As New TableRow
                    Dim cellNew1 As New TableCell
                    Dim cellNew2 As New TableCell
                    If i Mod 2 = 0 Then
                        rowNew.BackColor = Color.Silver
                    Else
                        rowNew.BackColor = Color.Gainsboro
                    End If
                    Select Case i
                        Case 0
                            cellNew1.Text = "Today"
                            DateFrom = DateTime.Today
                            DateTo = DateTime.Now
                        Case 1
                            cellNew1.Text = "Last 7 days"
                            DateFrom = DateTime.Today.AddDays(-7)
                            DateTo = DateTime.Today
                        Case 2
                            cellNew1.Text = "Last 8-14 days"
                            DateFrom = DateTime.Today.AddDays(-14)
                            DateTo = DateTime.Today.AddDays(-8)
                        Case 3
                            cellNew1.Text = "Last 15-30 days"
                            DateFrom = DateTime.Today.AddDays(-30)
                            DateTo = DateTime.Today.AddDays(-15)
                        Case 4
                            cellNew1.Text = "Last 31-90 days"
                            DateFrom = DateTime.Today.AddDays(-90)
                            DateTo = DateTime.Today.AddDays(-31)
                        Case 5
                            cellNew1.Text = "Last year"
                            DateFrom = DateTime.Today.AddYears(-1)
                            DateTo = DateTime.Now
                        Case 6
                            cellNew1.Text = "More than 1 year"
                            DateTo = DateTime.Today.AddYears(-1)
                        Case 7
                            cellNew1.Text = "Account created by admin, but never logged in"
                    End Select
                    If IsPrintView Then
                        Dim interval As String
                        interval = " and logindate between @DateFrom and @DateTo "

                        'TODO: Log - inconsistancy between count and list. Check if this function of I-link works

                        '-------- Commented by I-link on 29.7.2008 to solve the problem of inconsistancy between count and list.
                        'If i = 6 Then interval = " and logindate<@DateTo "
                        'selectSQL = "Select count(0) from (Select userid from log where userid>0" & _
                        '   interval & serverIP & " and (conflicttype=98 or conflicttype=0) group by userid) as t"
                        'If i = 7 Then selectSQL = "Select count(0) from (Select userid from log " & _
                        '     "where conflicttype=3" & serverIP & _
                        '     "and userid not in " & _
                        '     "(Select userid from log where conflicttype=98 or conflicttype=0 group by userid)) as t"
                        '-------------------------------------------------

                        '-------- Added by I-link on 29.7.2008 to solve the problem of inconsistancy between count and list.
                        If i = 6 Then interval = " and logindate<@DateTo "
                        selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(0) from log," & _
                           "(Select userid from log where userid<>0" & _
                           interval & serverIP & " and (conflicttype=98 or conflicttype=0) and conflicttype<>-31 group by userid) as t " & _
                           "where t.userid=log.userid and (conflicttype=1 or conflicttype=3)"
                        If i = 7 Then selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(0) from log," & _
                             "(Select userid from log " & _
                             "where conflicttype=3" & serverIP & _
                             "and userid not in " & _
                             "(Select userid from log where (conflicttype=98 or conflicttype=0) and conflicttype<>-31 group by userid)) as t " & _
                             "where t.userid=log.userid and (conflicttype=1 or conflicttype=3)"
                        '------------------------------------------------------------------------- 
                        Dim cmd1 As New SqlCommand(selectSQL, myCon)
                        cmd1.CommandTimeout = 0
                        cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                        cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                        Try
                            myCon.Open()
                            Dim hl As New HyperLink
                            arr_1(i) = cmd1.ExecuteScalar()
                            hl.Text = arr_1(i)
                            hl.Target = "_blank"
                            hl.NavigateUrl = "list_of_users.aspx?id=" & i '  "&sip=" & serverIP
                            cellNew2.Controls.Add(hl)
                        Catch ex As Exception
                            cammWebManager.Log.Warn(ex)
                        Finally
                            If Not cmd1 Is Nothing Then
                                cmd1.Dispose()
                            End If
                            If Not myCon Is Nothing Then
                                If myCon.State <> ConnectionState.Closed Then
                                    myCon.Close()
                                End If
                            End If
                        End Try
                    Else
                        cellNew2.Text = ViewState("number")(i)
                    End If
                    rowNew.Controls.Add(cellNew1)
                    rowNew.Controls.Add(cellNew2)
                    DataValues.Controls.Add(rowNew)
                Next
                Session("sip") = sip
                ViewState("number") = arr_1
            Catch ex As Exception

            Finally
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.NewUsersDetailsControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - detailed information of new users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NewUsersDetailsControl
        Inherits BaseControl

        Dim FirstName As String
        Dim LastName As String
        Dim Company As String
        Dim Country As String
        Dim Email As String
        Dim LoginName As String
        Dim Language(2) As String
        Dim Street As String
        Dim ZipCode As String
        Dim City As String
        Dim AccessLevel As String
        Dim Membership(50) As String
        Dim User_Name As String
        Dim Num_memb As Integer

        Protected LabelDescription, LabelIsData As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "New users"
            LabelDescription.Text = "This chart shows detailed information of new users with their contact data."
            ReportNameCell.Controls.Add(guiTable(title))

        End Sub



        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP">Server IPs filter - will be ignored</param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean)
            serverIP = Nothing
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            Dim selectSQL1 As String
            Dim constr As String = cammWebManager.ConnectionString
            selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid, logindate from log " & _
                        "where (conflicttype=1 or conflicttype=3)" & serverIP & _
                        " and logindate>=@DateFrom and logindate<=@DateTo" & _
                        " and userid not in (select userid from log where conflicttype=-31)" & _
                        " order by logindate desc"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlDataReader = Nothing
            Try
                myCon.Open()
                reader = cmd1.ExecuteReader()
                Dim counter As Integer
                Do While reader.Read()
                    If Not IsDBNull(reader(0)) Then
                        Dim t1 As New Table
                        Dim t2 As New Table
                        Dim t3 As New Table
                        t2.CellPadding = 0
                        t2.CellSpacing = 0
                        t3.CellPadding = 0
                        t3.CellSpacing = 0
                        Dim rowNew As New TableRow
                        Dim cellNew1 As New TableCell
                        Dim cellNew2 As New TableCell
                        Dim cellNew3 As New TableCell
                        Dim cellNew4 As New TableCell
                        Dim cellNew5 As New TableCell
                        If counter Mod 2 = 0 Then
                            rowNew.BackColor = Color.Silver
                        Else
                            rowNew.BackColor = Color.Gainsboro
                        End If
                        userInfo(reader(0)) 'user id
                        cellNew1.Text = User_Name
                        cellNew2.Text = reader(1) 'logindate
                        cellNew3.Text = AccessLevel
                        For i As Integer = 0 To Num_memb
                            Dim r1 As New TableRow
                            Dim c1 As New TableCell
                            c1.Text = Membership(i)
                            r1.Controls.Add(c1)
                            t1.Controls.Add(r1)
                        Next
                        cellNew4.Controls.Add(t1)
                        Dim lbl1 As New Label
                        Dim lbl2 As New Label
                        lbl1.Text = "<b>Name: </b>" & LastName & "<br>"
                        lbl1.Text &= "<b>Firstname: </b>" & FirstName & "<br>"
                        lbl1.Text &= "<b>Loginname: </b>" & LoginName & "<br>"
                        cellNew5.Controls.Add(lbl1)
                        Dim c2 As New TableCell
                        Dim c3 As New TableCell
                        Dim r2 As New TableRow
                        Dim addr As String = Street & "<br>" & ZipCode & " " & City
                        c2.Text = "<b>Address:&nbsp;</b>"
                        c2.VerticalAlign = 1
                        c3.Text = addr
                        r2.Controls.Add(c2)
                        r2.Controls.Add(c3)
                        t2.Controls.Add(r2)
                        cellNew5.Controls.Add(t2)
                        lbl2.Text = "<b>Country: </b>" & Country & "<br>"
                        lbl2.Text &= "<b>E-mail: </b>" & Email & "<br>"
                        cellNew5.Controls.Add(lbl2)
                        Dim r3 As New TableRow
                        Dim c4 As New TableCell
                        Dim c5 As New TableCell
                        c4.Text = "<b>Languages:&nbsp;</b>"
                        c4.VerticalAlign = 1
                        Dim lg As String = Language(0) & "<br>" & Language(1) & "<br>" & Language(2)
                        c5.Text = lg
                        r3.Controls.Add(c4)
                        r3.Controls.Add(c5)
                        t3.Controls.Add(r3)
                        cellNew5.Controls.Add(t3)
                        rowNew.Controls.Add(cellNew1)
                        rowNew.Controls.Add(cellNew2)
                        rowNew.Controls.Add(cellNew3)
                        rowNew.Controls.Add(cellNew4)
                        rowNew.Controls.Add(cellNew5)
                        DataValues.Controls.Add(rowNew)
                        counter += 1
                    End If
                Loop
                If counter = 0 Then
                    LabelIsData.Text = "No data for current time period"
                    DataValues.Visible = False
                    If sum_rep Then
                        Me.Visible = False
                    Else
                        Me.Visible = True
                    End If
                Else
                    Me.Visible = True
                    LabelIsData.Text = ""
                    DataValues.Visible = True
                End If
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub



        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's information from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub userInfo(ByVal user_id As Long)
            wm = cammWebManager
            Dim gr As CompuMaster.camm.WebManager.WMSystem.GroupInformation()
            Dim userinfo As New CompuMaster.camm.Webmanager.WMSystem.UserInformation(user_id, wm, True)
            Try
                FirstName = Server.HtmlEncode(userinfo.FirstName)
                LastName = Server.HtmlEncode(userinfo.LastName)
                Company = Server.HtmlEncode(userinfo.Company)
                Country = Server.HtmlEncode(userinfo.Country)
                Email = Server.HtmlEncode(userinfo.EMailAddress)
                LoginName = Server.HtmlEncode(userinfo.LoginName)
                Language(0) = Server.HtmlEncode(userinfo.PreferredLanguage1.LanguageName_English)
                Language(1) = Server.HtmlEncode(userinfo.PreferredLanguage2.LanguageName_English)
                Language(2) = Server.HtmlEncode(userinfo.PreferredLanguage3.LanguageName_English)
                Street = Server.HtmlEncode(userinfo.Street)
                ZipCode = Server.HtmlEncode(userinfo.ZipCode)
                City = Server.HtmlEncode(userinfo.Location)
                User_Name = Server.HtmlEncode(userinfo.FullName) & " (" & LoginName & ")"
                AccessLevel = Server.HtmlEncode(userinfo.AccessLevel.Title)
                gr = userinfo.Memberships
                Num_memb = gr.GetUpperBound(0)
                For i As Integer = 0 To Num_memb
                    Membership(i) = gr(i).Name
                Next
            Catch
                Array.Clear(Membership, 0, Membership.Length)
            End Try
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.NumberDeletedUsersControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - number of deleted users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NumberDeletedUsersControl
        Inherits BaseControl

        Protected LabelNumberDeletedUsersControl, LabelDescription As Label
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Number of deleted users"
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP">Server IPs filter - will be ignored</param>
        ''' <param name="lbl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal lbl As Boolean)
            serverIP = Nothing
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            Dim deleted As Integer
            Dim selectSQL1 As String
            Dim constr As String = cammWebManager.ConnectionString
            selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(userid) from log " & _
               "where conflicttype=-31 and userid in " & _
               "(select userid from log where (conflicttype=1 or conflicttype=3)" & _
               serverIP & _
               ") and logindate>=@DateFrom and logindate<=@DateTo"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Try
                myCon.Open()
                deleted = cmd1.ExecuteScalar()
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
            LabelNumberDeletedUsersControl.Text = "NUMBER OF DELETED USERS: " & deleted
            LabelDescription.Text = "This report shows the number of deleted users for a selected period."
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.NumberNewUsersControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - number of new users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NumberNewUsersControl
        Inherits BaseControl

        Protected LabelDescription As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        Dim AccessLevel(50) As String
        Dim num_acc As Integer
        Dim New_users(50) As Integer
        Dim Exist_users(50) As Integer
        Dim ServerIP As String

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Number of new users"
            LabelDescription.Text = "This chart shows the number of new users with different access levels. " & _
            "It also shows the number of existing users. The ratio number of new users to existing users are shown in percentage."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom1"></param>
        ''' <param name="DateTo1"></param>
        ''' <param name="s">Server IPs filter - will be ignored</param>
        ''' <param name="lbl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom1 As DateTime, ByVal DateTo1 As DateTime, ByVal s As String, ByVal lbl As Boolean)
            s = Nothing

            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateFrom = DateFrom1
            DateTo = DateTo1
            ServerIP = s
            DateTo = DateTo.AddDays(1)
            access_level()
            For i As Integer = 0 To num_acc
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                Dim cellNew3 As New TableCell
                Dim cellNew4 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = AccessLevel(i)
                cellNew2.Text = New_users(i)
                cellNew3.Text = Exist_users(i)
                If Exist_users(i) = 0 Then
                    cellNew4.Text = "0"
                Else
                    cellNew4.Text = Math.Round(New_users(i) / Exist_users(i) * 100, 2)
                End If
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                rowNew.Controls.Add(cellNew3)
                rowNew.Controls.Add(cellNew4)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub


        'ToDo: Calculate the number of existing users not by the data of the log, calculate it by the table "log_users" or "benutzer"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the number of new users for different access levels
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub access_level()
            Dim selectSQL1 As String
            Dim selectSQL2 As String
            Dim constr As String = cammWebManager.ConnectionString
            selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid from log " & _
                        "where (conflicttype=1 or conflicttype=3)" & ServerIP & _
                        " and logindate>=@DateFrom and logindate<=@DateTo"
            selectSQL2 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid from log " & _
                        "where (conflicttype=1 or conflicttype=3)" & ServerIP & _
                        " and logindate<@DateTo"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            Dim cmd2 As New SqlCommand(selectSQL2, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            cmd2.CommandTimeout = 0
            cmd2.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlDataReader = Nothing
            Dim al As CompuMaster.camm.WebManager.WMSystem.UserInformation()
            Try
                myCon.Open()
                num_acc = cammWebManager.System_GetAccessLevelInfos.GetUpperBound(0)
                For i As Integer = 0 To num_acc
                    reader = cmd1.ExecuteReader()
                    AccessLevel(i) = cammWebManager.System_GetAccessLevelInfos()(i).Title
                    Try
                        al = cammWebManager.System_GetAccessLevelInfos()(i).Users
                        Do While reader.Read()
                            For j As Integer = 0 To al.GetUpperBound(0)
                                If al(j).ID = reader(0) Then New_users(i) += 1
                            Next
                        Loop
                    Catch
                        New_users(i) = 0
                    End Try
                    reader.Close()
                Next
                For i As Integer = 0 To num_acc
                    reader = cmd2.ExecuteReader()
                    Try
                        al = cammWebManager.System_GetAccessLevelInfos()(i).Users
                        Do While reader.Read()
                            For j As Integer = 0 To al.GetUpperBound(0)
                                If al(j).ID = reader(0) Then Exist_users(i) += 1
                            Next
                        Loop
                    Catch
                        Exist_users(i) = 0
                    End Try
                    reader.Close()
                Next
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.NumberUpdatedUsersControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - number of updated user profiles
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NumberUpdatedUsersControl
        Inherits BaseControl

        Dim AccessLevel(50) As String
        Dim num_acc As Integer
        Dim Updated_users(50) As Integer
        Dim ServerIP As String

        Protected LabelDescription As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Number of updated user profiles"
            LabelDescription.Text = "This chart shows the number of updated user profiles for users " & _
            "with different access levels in selected periods."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom1"></param>
        ''' <param name="DateTo1"></param>
        ''' <param name="s">Server IPs filter - will be ignored</param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom1 As DateTime, ByVal DateTo1 As DateTime, ByVal s As String, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            s = Nothing

            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            If IsPrintView Then
                DateFrom = DateFrom1
                DateTo = DateTo1
                ServerIP = s
                DateTo = DateTo.AddDays(1)
                access_level()
                Dim arr_1(1) As String
                Dim arr_2(1) As String
                For i As Integer = 0 To num_acc
                    arr_1(i) = AccessLevel(i)
                    arr_2(i) = Updated_users(i)
                    ReDim Preserve arr_1(i + 1) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    ReDim Preserve arr_2(i + 1) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                Next
                ViewState("num_acc") = num_acc
                ViewState("accesslevel") = arr_1
                ViewState("updated_users") = arr_2
            End If
            table_fill()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            For i As Integer = 0 To ViewState("num_acc")
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("accesslevel")(i)
                cellNew2.Text = ViewState("updated_users")(i)
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the number of updated user's profiles for different access levels
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub access_level()
            Dim selectSQL1 As String
            Dim constr As String = cammWebManager.ConnectionString
            selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select system_accesslevels.title, count(log.id) from " & _
         "system_accesslevels left outer join benutzer on accountaccessability=system_accesslevels.id " & _
                        "left outer join log on benutzer.id=userid " & _
                        "and log.logindate between @DateFrom and @DateTo " & _
                        ServerIP & _
                        "and (conflicttype between -7 and -4 or conflicttype between 4 and 6 or conflicttype=-9) " & _
                        "and userid not in (select userid from log where conflicttype=-31) " & _
                        " group by system_accesslevels.title"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlDataReader = Nothing
            Try
                Dim i As Integer
                myCon.Open()
                reader = cmd1.ExecuteReader()
                Try
                    Do While reader.Read()
                        AccessLevel(i) = reader(0) 'title of accesslevel
                        Updated_users(i) = reader(1) 'count (log.id)
                        i += 1
                    Loop
                Catch
                End Try
                num_acc = i - 1
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.RedirectionsControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - redirections
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class RedirectionsControl
        Inherits BaseControl

        Protected LabelDescription, LabelIsData As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            If Request.QueryString("DescOrder") = "desc" Then
                Session("DescOrder") = "asc"
            Else
                Session("DescOrder") = "desc"
            End If
            If Request.QueryString("ClickOrder") = "desc" Then
                Session("ClickOrder") = "asc"
            Else
                Session("ClickOrder") = "desc"
            End If
            Dim title As String = "Redirections"
            LabelDescription.Text = "All redirections for the selected periods are shown here. The number of clicks " & _
        "for every redirection are shown as horizontal diagrams."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal sum_rep As Boolean, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            Session("dFrom") = DateFrom.ToString
            Session("dTo") = DateTo.ToString
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            Dim sortList As String = "clicks desc"
            If Request.QueryString("sort") = "description" Then
                If Request.QueryString("DescOrder") = "desc" Then
                    sortList = "description desc"
                Else
                    sortList = "description asc"
                End If
            End If
            If Request.QueryString("sort") = "Click" Then
                If Request.QueryString("ClickOrder") = "desc" Then
                    sortList = "clicks desc"
                Else
                    sortList = "clicks asc"
                End If
            End If

            If IsPrintView Then
                Dim selectSQL1 As String
                Dim constr As String = cammWebManager.ConnectionString
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select description, clicks from redirects_toaddr, " & _
                   "(Select id_redirector, count(*) as clicks from redirects_log " & _
                   "where accessdatetime between @DateFrom and @DateTo" & _
                   " group by id_redirector) as t" & _
                   " where redirects_toaddr.id=id_redirector" & vbNewLine & _
                   "ORDER by " & sortList
                Dim myCon As New SqlConnection(constr)
                Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    Me.Visible = True
                    reader = cmd1.ExecuteReader()
                    LabelIsData.Text = ""
                    Dim max As Integer
                    Dim counter As Integer
                    Dim arr_1(1) As String
                    Dim arr_2(1) As Integer
                    Do While reader.Read()
                        arr_1(counter) = Server.HtmlEncode(Convert.ToString(reader(0))) 'description
                        arr_2(counter) = reader(1) 'clicks
                        If arr_2(counter) > max Then max = arr_2(counter)
                        counter += 1
                        ReDim Preserve arr_1(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve arr_2(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Loop
                    ViewState("max") = max
                    ViewState("description") = arr_1
                    ViewState("clicks") = arr_2
                    ViewState("counter") = counter
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            End If
            table_fill()
            If ViewState("counter") = 0 Then
                LabelIsData.Text = "No data for current time period"
                DataValues.Visible = False
                If sum_rep Then
                    Me.Visible = False
                Else
                    Me.Visible = True
                End If
            Else
                Me.Visible = True
                LabelIsData.Text = ""
                DataValues.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            Dim width As Integer
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("description")(i)
                rowNew.Controls.Add(cellNew1)
                Dim diagrImg As New System.Web.UI.WebControls.Image
                Dim lbl1 As New Label
                diagrImg.ImageUrl = "./Images/7.gif"
                diagrImg.Height = Unit.Pixel(10)
                width = 85 * ViewState("clicks")(i) / ViewState("max")
                diagrImg.Width = Unit.Percentage(width)
                lbl1.Text = " " & ViewState("clicks")(i)
                cellNew2.Controls.Add(diagrImg)
                cellNew2.Controls.Add(lbl1)
                rowNew.Controls.Add(cellNew2)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.ServerGroupsControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - numeral change of clicks for server groups
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ServerGroupsControl
        Inherits BaseControl



        Dim ServerGroups As CompuMaster.camm.WebManager.WMSystem.ServerGroupInformation()
        Dim ServerTitle(1) As String
        Dim Arr_xaxis(1) As String
        Dim Arr_value(10, 1) As Integer
        Dim Ymax As Double
        Dim Ystep As Double
        Dim ServerIP(10) As String
        Dim arr_len As Integer
        Dim mode As String

        Protected LabelDescription As Label
        Protected Areachart1 As AreaChart
        Protected nameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Total history of server groups"
            LabelDescription.Text = "This graphic shows the numeral changes of clicks for server groups " & _
                  "on the website during selected periods (area chart)."
            nameCell.Controls.Add(guiTable(title))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom1"></param>
        ''' <param name="DateTo1"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom1 As DateTime, ByVal DateTo1 As DateTime, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            If lbl Then
                nameCell.Visible = False
            Else
                nameCell.Visible = True
            End If
            If IsPrintView Then
                DateFrom = DateFrom1
                DateTo = DateTo1
                mode = "Day"
                Dim CurrentDate1 As DateTime = DateTo.AddMonths(-2)
                If CurrentDate1 > DateFrom And mode = "Day" Then mode = "Month"
                Dim diff As TimeSpan
                Dim scale_x As Integer
                Dim days As Integer
                Dim k As Integer
                diff = DateTo.Subtract(DateFrom)
                days = diff.Days
                axis_X(days, mode, k, scale_x)
                CurrentDate = DateFrom
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                arr_len = cammWebManager.System_GetServerGroupsInfo.GetUpperBound(0)
                For i As Integer = 0 To arr_len
                    ReDim Preserve ServerTitle(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    ServerTitle(i) = ServerGroups(i).Title
                Next
                serverIPExtraction()
                Dim constr As String = cammWebManager.ConnectionString
                Dim myCon1 As New SqlConnection(constr)
                Try
                    myCon1.Open()
                    Dim max As Integer
                    If mode = "Day" Then
                        For i As Integer = 0 To scale_x - 1
                            For j As Integer = 0 To k - 1
                                ReDim Preserve Arr_xaxis(i * k + j) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                ReDim Preserve Arr_value(10, i * k + j) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                If j = 0 Then
                                    Arr_xaxis(i * k + j) = CurrentDate.ToString("dd.MM.yy")
                                Else
                                    Arr_xaxis(i * k + j) = ""
                                End If
                                For jj As Integer = 0 To arr_len
                                    Arr_value(jj, i * k + j) = clicks(jj, mode, myCon1)
                                    If Arr_value(jj, i * k + j) > max Then max = Arr_value(jj, i * k + j)
                                Next
                                CurrentDate = CurrentDate.AddDays(1)
                            Next
                        Next
                    Else
                        For i As Integer = 0 To scale_x - 1
                            ReDim Preserve Arr_xaxis(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            ReDim Preserve Arr_value(10, i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            Arr_xaxis(i) = CurrentDate.ToString("MMM yy")
                            For j As Integer = 0 To arr_len
                                Arr_value(j, i) = clicks(j, mode, myCon1)
                                If Arr_value(j, i) > max Then max = Arr_value(j, i)
                            Next
                            CurrentDate = CurrentDate.AddMonths(1)
                        Next
                    End If
                    axis_Y(max)
                    Areachart1.showareachart(Arr_xaxis, Arr_value, ServerTitle, Ymax, Ystep, arr_len)
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not myCon1 Is Nothing Then
                        If myCon1.State <> ConnectionState.Closed Then
                            myCon1.Close()
                        End If
                        myCon1.Dispose()
                    End If
                End Try
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the interval for the horizontal axis
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub axis_X(ByVal days As Integer, ByVal mode As String, ByRef k As Integer, ByRef scale_x As Integer)
            Dim i As Integer
            Dim m As Integer
            If mode = "Day" Then
                m = days
                If m / 12 < 1 Then
                    scale_x = m + 1
                    k = 1
                ElseIf m Mod 12 = 0 Then
                    scale_x = 13
                    k = m / 12
                Else
                    k = Int(m / 12) + 1
                    CurrentDate = DateFrom
                    Do While CurrentDate <= DateTo
                        CurrentDate = CurrentDate.AddDays(k)
                        i += 1
                    Loop
                    scale_x = i
                End If
            Else
                CurrentDate = DateFrom
                scale_x = 1
                Do While DateTo.Month <> CurrentDate.Month Or DateTo.Year <> CurrentDate.Year
                    scale_x += 1
                    CurrentDate = CurrentDate.AddMonths(1)
                Loop
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the interval for the vertical axis
        ''' </summary>
        ''' <param name="cl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub axis_Y(ByVal cl As Single)
            Dim i As Integer
            Do While cl >= 10
                cl /= 10
                i += 1
            Loop
            Ymax = (Int(cl) + 1) * 10 ^ i
            Ystep = 10 ^ i
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of server IP's from camm Web-Manager
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub serverIPExtraction()
            For i As Integer = 0 To arr_len
                Dim srv As CompuMaster.camm.WebManager.WMSystem.ServerInformation()
                ServerGroups = cammWebManager.System_GetServerGroupsInfo
                Dim isselect As Boolean = False
                For j As Integer = 0 To cammWebManager.System_GetServersInfo(ServerGroups(i).ID).GetUpperBound(0)
                    srv = cammWebManager.System_GetServersInfo(ServerGroups(i).ID)
                    If Not isselect Then
                        isselect = True
                        ServerIP(i) = "(serverip = N'" & Replace(srv(j).IPAddressOrHostHeader, "'", "''") & "'"
                    Else
                        ServerIP(i) &= " or serverip = N'" & Replace(srv(j).IPAddressOrHostHeader, "'", "''") & "'"
                    End If
                Next
                ServerIP(i) &= ")"
            Next
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Number of clicks for the server group
        ''' </summary>
        ''' <param name="i"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function clicks(ByVal i As Integer, ByVal mode As String, ByVal connection As SqlConnection) As Integer
            Dim selectSQL1 As String
            If mode = "Day" Then
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(0) from log where " & _
                                    "logindate>=@DateFrom" & _
                                     " and logindate<=@DateTo" & _
                                    " and " & ServerIP(i) & _
                                    " and userid not in (select id_user from memberships where id_group=6)" & _
                                     " and applicationid not in (select id from applications where systemapp=1)" & _
                                     " and conflicttype=0"
            Else
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(0) from log where " & _
                                     "logindate>=@DateFrom" & _
                                     " and logindate<=@DateTo" & _
                                     " and month(logindate)=" & CurrentDate.Month & _
                                     " and year(logindate)=" & CurrentDate.Year & " and " & ServerIP(i) & _
                                     " and userid not in (select id_user from memberships where id_group=6)" & _
                                     " and applicationid not in (select id from applications where systemapp=1)" & _
                                     " and conflicttype=0"
            End If
            Dim cmd1 As New SqlCommand(selectSQL1, connection)
            cmd1.CommandTimeout = 0
            If mode = "Day" Then
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = CurrentDate
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = CurrentDate.AddDays(1)
            Else
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo.AddDays(1)
            End If
            Try
                clicks = cmd1.ExecuteScalar()
            Finally
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
            End Try
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.DeletedUsersListControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - deleted users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DeletedUsersListControl
        Inherits BaseControl

        Protected LabelDescription, LabelIsData As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Deleted users"
            LabelDescription.Text = "This chart shows all deleted users in the selected periods and the date of deleting."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP">Server IPs filter - will be ignored</param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean)
            serverIP = Nothing
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            Dim selectSQL1 As String
            Dim constr As String = cammWebManager.ConnectionString
            selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid, logindate from log " & _
                        "where conflicttype=-31 and userid in " & _
                        "(select userid from log where (conflicttype=1 or conflicttype=3)" & _
                        serverIP & _
                        ") and logindate>=@DateFrom and logindate<=@DateTo" & _
                        " order by logindate desc"
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlDataReader = Nothing
            Try
                myCon.Open()
                reader = cmd1.ExecuteReader()
                Dim counter As Integer
                Do While reader.Read()
                    Dim rowNew As New TableRow
                    Dim cellNew1 As New TableCell
                    Dim cellNew2 As New TableCell
                    If counter Mod 2 = 0 Then
                        rowNew.BackColor = Color.Silver
                    Else
                        rowNew.BackColor = Color.Gainsboro
                    End If
                    cellNew1.Text = userName(reader(0)) 'user id
                    cellNew2.Text = reader(1) 'logindate
                    rowNew.Controls.Add(cellNew1)
                    rowNew.Controls.Add(cellNew2)
                    DataValues.Controls.Add(rowNew)
                    counter += 1
                Loop
                If counter = 0 Then
                    LabelIsData.Text = "No data for current time period"
                    DataValues.Visible = False
                    If sum_rep Then
                        Me.Visible = False
                    Else
                        Me.Visible = True
                    End If
                Else
                    Me.Visible = True
                    LabelIsData.Text = ""
                    DataValues.Visible = True
                End If
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's name from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Function userName(ByVal user_id As Long) As String
            wm = cammWebManager
            Dim UserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(user_id, wm, True)
            Try
                userName = Server.HtmlEncode(UserInfo.FullName) & _
                " (ID " & user_id & ")"
            Catch ex As Exception
                userName = "Unknown or deleted user"
            End Try
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.LatestLogonDatesControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - last login dates made by users (last 50)
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class LatestLogonDatesControl
        Inherits BaseControl

        Dim du As Boolean

        Protected LabelDescription, LabelIsData, LabelUsers As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Latest logon dates"
            LabelDescription.Text = "The last logon dates made by users (latest 50) are shown here."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP"></param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal IsLogged As Boolean, ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            If IsLogged Then
                LabelUsers.Text = "Logged users"
            Else
                LabelUsers.Text = "Unlogged users"
            End If
            DateTo = DateTo.AddDays(1)
            If IsPrintView Then
                LabelUsers.Visible = False
                Dim selectSQL1 As String
                Dim selectSQL2 As String
                Dim constr As String = cammWebManager.ConnectionString
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select top 50 userid, max(logindate) as latest_date from log " & _
                   "where (conflicttype=98 or conflicttype=0)" & _
                   " and logindate>=@DateFrom and logindate<=@DateTo " & _
                   serverIP & _
                   " and userid not in (select userid from log where conflicttype=-31)" & _
                   " group by userid order by latest_date desc"
                selectSQL2 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid,max(logindate) as latest_date from log where userid not in " & _
                            "(Select userid from log where (conflicttype=98 or conflicttype=0) " & _
                            "and logindate>=@DateFrom and logindate<=@DateTo) " & _
                            "and userid not in (select userid from log where conflicttype=-31) " & serverIP & _
                            " and (conflicttype=98 or conflicttype=0) group by userid order by latest_date desc"
                Dim myCon As New SqlConnection(constr)
                Dim cmd1 As SqlCommand
                If IsLogged Then
                    cmd1 = New SqlCommand(selectSQL1, myCon)
                Else
                    cmd1 = New SqlCommand(selectSQL2, myCon)
                End If
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    reader = cmd1.ExecuteReader()
                    Dim counter As Integer
                    Dim arr_1(1) As String
                    Dim arr_2(1) As DateTime
                    Do While reader.Read()
                        If Not IsDBNull(reader(0)) Then
                            Dim un As String = userName(reader(0)) 'user id
                            If du = False Then
                                arr_1(counter) = un
                                arr_2(counter) = reader(1) 'logindate
                                counter += 1
                                ReDim Preserve arr_1(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                                ReDim Preserve arr_2(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            End If
                        End If
                    Loop
                    ViewState("user_name") = arr_1
                    ViewState("latest_date") = arr_2
                    ViewState("counter") = counter
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            Else
                LabelUsers.Visible = True
            End If
            table_fill()
            If ViewState("counter") = 0 Then
                LabelIsData.Text = "No data for current time period"
                DataValues.Visible = False
                If sum_rep Then
                    Me.Visible = False
                Else
                    Me.Visible = True
                End If
            Else
                Me.Visible = True
                LabelIsData.Text = ""
                DataValues.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("user_name")(i)
                cellNew2.Text = ViewState("latest_date")(i)
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's name from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Function userName(ByVal user_id As Long) As String
            Dim Result As String = Nothing
            Dim ln As String = Nothing
            Try
                ln = cammWebManager.System_GetUserInfo(user_id).LoginName
                Result = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).FullName) & " (" & ln & ")"
                du = False
                Return Result
            Catch ex As Exception
                du = True
            End Try
            Return Result
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.NewUserListControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - list of new users
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NewUserListControl
        Inherits BaseControl

        Protected LabelDescription, LabelIsData As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "List of new users"
            LabelDescription.Text = "All new registered users in selected periods are shown in this chart. " & _
            "There is also information to how and when the user has been registered."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP">Server IPs filter - will be ignored</param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            serverIP = Nothing
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            If IsPrintView Then
                Dim selectSQL1 As String
                Dim constr As String = cammWebManager.ConnectionString
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select userid, logindate, conflicttype from log " & _
                   "where (conflicttype=1 or conflicttype=3)" & serverIP & _
                   " and logindate>=@DateFrom and logindate<=@DateTo" & _
                   " and userid not in (select userid from log where conflicttype=-31)" & _
                   " order by logindate desc"
                Dim myCon As New SqlConnection(constr)
                Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    reader = cmd1.ExecuteReader()
                    Dim arr_1(1) As String
                    Dim arr_2(1) As DateTime
                    Dim arr_3(1) As String
                    Dim counter As Integer
                    Do While reader.Read()
                        If Not IsDBNull(reader(0)) Then
                            arr_1(counter) = userName(reader(0)) 'user id
                            arr_2(counter) = reader(1) 'logindate
                            If reader(2) = 1 Then
                                arr_3(counter) = "user"
                            Else
                                arr_3(counter) = "admin"
                            End If
                            counter += 1
                            ReDim Preserve arr_1(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            ReDim Preserve arr_2(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            ReDim Preserve arr_3(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        End If
                    Loop
                    ViewState("user_name") = arr_1
                    ViewState("logindate") = arr_2
                    ViewState("conflicttype") = arr_3
                    ViewState("counter") = counter
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            End If
            table_fill()
            If ViewState("counter") = 0 Then
                LabelIsData.Text = "No data for current time period"
                DataValues.Visible = False
                If sum_rep Then
                    Me.Visible = False
                Else
                    Me.Visible = True
                End If
            Else
                Me.Visible = True
                LabelIsData.Text = ""
                DataValues.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                Dim cellNew3 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("user_name")(i)
                cellNew2.Text = ViewState("logindate")(i)
                cellNew3.Text = ViewState("conflicttype")(i)
                'If ViewState("conflicttype")(i) = "1" Then
                '    cellNew3.Text = "user"
                'Else
                '    cellNew3.Text = "admin"
                'End If
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                rowNew.Controls.Add(cellNew3)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's name from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Function userName(ByVal user_id As Long) As String
            Dim Result As String = Nothing
            Dim ln As String = Nothing
            Try
                ln = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).LoginName)
                Result = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).FullName) & " (" & ln & ")"
            Catch
            End Try
            Return Result
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.OnlineMomentControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - which user is online at the moment
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class OnlineMomentControl
        Inherits BaseControl

        Protected LabelDescription As Label
        Protected Users As Table
        Protected cellTotalUsers As TableCell
        Protected rowTotalUsers As TableRow
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Current users online"
            LabelDescription.Text = "This chart shows which user is online at the moment. The hyperlink 'activity log' " & _
         "refers to a table with information on which application was visited by the " & _
         "user in the past 15 minutes (the table is updated every 10 seconds)."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="serverIP"></param>
        ''' <param name="lbl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal serverIP As String, ByVal lbl As Boolean)
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            Dim selectSQL1 As String
            selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select userid, remoteip from log where " & _
                        "datediff(mi, logindate, CURRENT_TIMESTAMP) < 15 " & _
                        serverIP & _
                        " and userid not in (select id_user from memberships where id_group=6)" & _
                        " group by userid, remoteip"
            Dim constr As String = cammWebManager.ConnectionString
            Dim myCon As New SqlConnection(constr)
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.CommandTimeout = 0
            Dim reader As SqlDataReader = Nothing
            Try
                myCon.Open()
                reader = cmd1.ExecuteReader()
                Dim i As Integer
                Do While reader.Read()
                    If Not IsDBNull(reader(0)) Then
                        Dim rowNew As New TableRow
                        Dim cellNew1 As New TableCell
                        Dim cellNew2 As New TableCell
                        Dim cellNew3 As New TableCell
                        Dim cellNew4 As New TableCell
                        If i Mod 2 = 0 Then
                            rowNew.BackColor = Color.Silver
                        Else
                            rowNew.BackColor = Color.Gainsboro
                        End If
                        Dim hl As New HyperLink
                        cellNew1.Text = userName(reader(0)) 'user id
                        hl.Text = "Activity log"
                        hl.Target = "_blank"
                        cellNew2.Text = reader(1) 'remote IP
                        hl.NavigateUrl = "user_15_minutes.aspx?id=" & reader(0) & "&name=" & cellNew1.Text & "&rip=" & cellNew2.Text
                        cellNew3.Text = user_email(reader(0))
                        cellNew4.Controls.Add(hl)
                        rowNew.Controls.Add(cellNew1)
                        rowNew.Controls.Add(cellNew2)
                        rowNew.Controls.Add(cellNew3)
                        rowNew.Controls.Add(cellNew4)
                        Users.Controls.Add(rowNew)
                        i += 1
                    End If
                Loop
                cellTotalUsers.Text = "Total: " & i & " users"
                If i = 0 Then
                    rowTotalUsers.Visible = False
                Else
                    rowTotalUsers.Visible = True
                End If
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                    reader.Close()
                End If
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's name from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Overrides Function userName(ByVal user_id As Long) As String
            Dim ln As String
            Try
                ln = cammWebManager.System_GetUserInfo(user_id).LoginName
                userName = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).FullName) & _
                " (" & ln & ")"
            Catch ex As Exception
                userName = "Unknown user"
            End Try
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user's e-mail from camm Web-Manager
        ''' </summary>
        ''' <param name="user_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function user_email(ByVal user_id As Long) As String
            Try
                user_email = cammWebManager.System_GetUserInfo(user_id).EMailAddress
            Catch ex As Exception
                user_email = ""
            End Try
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.UpdatedProfilesListControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - list of updated user profiles
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class UpdatedProfilesListControl
        Inherits BaseControl

        Protected LabelDescription, LabelIsData As Label
        Protected DataValues As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Updated user profiles"
            LabelDescription.Text = "This chart shows all user profiles updated in the selected periods. " & _
            "There is also information on how the user profiles were changed as well as the way they were changed."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the data from a database
        ''' </summary>
        ''' <param name="DateFrom"></param>
        ''' <param name="DateTo"></param>
        ''' <param name="serverIP">Server IPs filter - will be ignored</param>
        ''' <param name="sum_rep"></param>
        ''' <param name="lbl"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub LoadData(ByVal DateFrom As DateTime, ByVal DateTo As DateTime, ByVal serverIP As String, ByVal sum_rep As Boolean, ByVal lbl As Boolean, ByVal IsPrintView As Boolean)
            serverIP = Nothing
            If lbl Then
                ReportNameCell.Visible = False
            Else
                ReportNameCell.Visible = True
            End If
            DateTo = DateTo.AddDays(1)
            If IsPrintView Then
                Dim selectSQL1 As String
                Dim constr As String = cammWebManager.ConnectionString
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select userid,logindate,conflicttype from log " & _
                   "where (conflicttype between -7 and -4 or " & _
                   "conflicttype between 4 and 6 or conflicttype=-9" & _
                   ")" & serverIP & _
                   " and logindate>=@DateFrom and logindate<=@DateTo" & _
                   " and userid not in (select userid from log where conflicttype=-31)" & _
                   " group by userid,logindate, conflicttype " & _
                   "order by logindate desc"
                Dim myCon As New SqlConnection(constr)
                Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    reader = cmd1.ExecuteReader()
                    Dim counter As Integer
                    Dim arr_1(1) As String
                    Dim arr_2(1) As DateTime
                    Dim arr_3(1) As String
                    Do While reader.Read()
                        If Not IsDBNull(reader(0)) And Not IsDBNull(reader(2)) Then
                            arr_1(counter) = userName(reader(0)) 'user id
                            arr_2(counter) = reader(1) 'logindate
                            arr_3(counter) = description(reader(2)) 'conflicttype
                            counter += 1
                            ReDim Preserve arr_1(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            ReDim Preserve arr_2(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                            ReDim Preserve arr_3(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        End If
                    Loop
                    ViewState("user_name") = arr_1
                    ViewState("logindate") = arr_2
                    ViewState("description") = arr_3
                    ViewState("counter") = counter
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            End If
            table_fill()
            If ViewState("counter") = 0 Then
                LabelIsData.Text = "No data for current time period"
                DataValues.Visible = False
                If sum_rep Then
                    Me.Visible = False
                Else
                    Me.Visible = True
                End If
            Else
                Me.Visible = True
                LabelIsData.Text = ""
                DataValues.Visible = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                Dim cellNew3 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("user_name")(i)
                cellNew2.Text = ViewState("logindate")(i)
                cellNew3.Text = ViewState("description")(i)
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                rowNew.Controls.Add(cellNew3)
                DataValues.Controls.Add(rowNew)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Descriptions of conflict's types
        ''' </summary>
        ''' <param name="ct"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function description(ByVal ct As Integer) As String
            Select Case ct
                Case -9
                    Return "User profile attributes changed by admin or the user itself"
                Case -7
                    Return "Authorizations of user modified indirectly via group membershipment"
                Case -6
                    Return "Authorizations of user modified"
                Case -5
                    Return "Account lock has been resetted by admin"
                Case -4
                    Return "User profile modified by the user itself"
                Case 4
                    Return "User profile modified by admin"
                Case 5
                    Return "User password modified by admin"
                Case 6
                    Return "User password modified by the user itself"
                Case Else
                    Return Nothing
            End Select
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.ViewTraceLogControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - last 50 entries of each selected user
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ViewTraceLogControl
        Inherits BaseControl

        Dim UserID As String
        Dim UserFromDropList As String
        Dim PrintView As Boolean

        Protected LabelTitle, LabelIsData As Label
        Protected TraceTable As Table


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the user from a dropdown list
        ''' </summary>
        ''' <param name="serverIP"></param>
        ''' <param name="drpText"></param>
        ''' <param name="drpValue"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal serverIP As String, ByVal drpText As String, ByVal drpValue As String, ByVal IsPrintView As Boolean)
            PrintView = IsPrintView
            UserFromDropList = drpText
            UserID = drpValue
            If UserFromDropList <> "(Select)" Then
                ViewTraceTable(serverIP)
                LabelTitle.Text = UserFromDropList & " - last 50 entries"
            Else
                LabelTitle.Visible = False
                TraceTable.Visible = False
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of data from database
        ''' </summary>
        ''' <param name="serverIP"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ViewTraceTable(ByVal serverIP As String)
            If PrintView Then
                Dim constr As String = cammWebManager.ConnectionString
                Dim myCon As New SqlConnection(constr)
                Dim selectSQL1 As String
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select top 50 * from log where userid= @UserID " & serverIP & " order by logindate desc"
                Dim cmd1 As New SqlCommand(selectSQL1, myCon)
                cmd1.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID
                cmd1.CommandTimeout = 0
                Dim app As Integer
                Dim url As String
                Dim reader1 As SqlDataReader = Nothing
                Try
                    myCon.Open()
                    reader1 = cmd1.ExecuteReader()
                    Dim counter As Integer
                    Dim arr_1(1) As String
                    Dim arr_2(1) As String
                    Dim arr_3(1) As DateTime
                    Dim arr_4(1) As String
                    Dim arr_5(1) As String
                    Do While reader1.Read()
                        If reader1(5) Is System.DBNull.Value Then
                            app = -1
                        Else
                            app = reader1(5) 'application id
                        End If
                        If reader1(6) Is System.DBNull.Value Then
                            url = "&nbsp;"
                        Else
                            url = reader1(6) 'URL
                        End If
                        arr_1(counter) = appName(app)
                        arr_2(counter) = url
                        arr_3(counter) = reader1(2) 'logindate
                        arr_4(counter) = reader1(3) 'remote IP
                        arr_5(counter) = description(reader1(7)) 'conflicttype
                        counter += 1
                        ReDim Preserve arr_1(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve arr_2(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve arr_3(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve arr_4(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve arr_5(counter) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Loop
                    ViewState("app_name") = arr_1
                    ViewState("url") = arr_2
                    ViewState("logindate") = arr_3
                    ViewState("remoteip") = arr_4
                    ViewState("description") = arr_5
                    ViewState("counter") = counter
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader1 Is Nothing AndAlso Not reader1.IsClosed Then
                        reader1.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
            End If
            table_fill()
            If ViewState("counter") = 0 Then
                LabelIsData.Text = "No data"
                TraceTable.Visible = False
                LabelTitle.Visible = False
            Else
                LabelIsData.Text = ""
                TraceTable.Visible = True
                LabelTitle.Visible = True
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creation of the table
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub table_fill()
            For i As Integer = 0 To ViewState("counter") - 1
                Dim rowNew As New TableRow
                Dim cellNew1 As New TableCell
                Dim cellNew2 As New TableCell
                Dim cellNew3 As New TableCell
                Dim cellNew4 As New TableCell
                Dim cellNew5 As New TableCell
                If i Mod 2 = 0 Then
                    rowNew.BackColor = Color.Silver
                Else
                    rowNew.BackColor = Color.Gainsboro
                End If
                cellNew1.Text = ViewState("app_name")(i)
                cellNew2.Text = ViewState("url")(i)
                cellNew3.Text = ViewState("logindate")(i)
                cellNew4.Text = ViewState("remoteip")(i)
                cellNew5.Text = ViewState("description")(i)
                rowNew.Controls.Add(cellNew1)
                rowNew.Controls.Add(cellNew2)
                rowNew.Controls.Add(cellNew3)
                rowNew.Controls.Add(cellNew4)
                rowNew.Controls.Add(cellNew5)
                TraceTable.Controls.Add(rowNew)
            Next
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Extraction of the application's name from camm Web-Manager
        ''' </summary>
        ''' <param name="app_id"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function appName(ByVal app_id As Integer) As String
            Try
                Dim app_info As New CompuMaster.camm.webmanager.WMSystem.SecurityObjectInformation(app_id, cammWebManager, True)
                appName = Server.HtmlEncode(app_info.Name)
            Catch ex As Exception
                appName = "&nbsp;"
            End Try
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Descriptions of conflict's types
        ''' </summary>
        ''' <param name="ct"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function description(ByVal ct As Integer) As String
            Select Case ct
                Case -99
                    description = "Debug"
                Case -98
                    description = "Unsuccessfull login"
                Case -95
                    description = "Login locked temporary (PW check failed too often)"
                Case -31
                    description = "User deleted"
                Case -29
                    description = "User account temporary locked on document validation"
                Case -28
                    description = "User account disabled on document validation"
                Case -27
                    description = "Missing authorization on document validation"
                Case -26
                    description = "No valid login data (e.g. PW)"
                Case -25
                    description = "Missing login on document validation"
                Case -9
                    description = "User profile attributes changed by admin or the user itself"
                Case -8
                    description = "Authorizations of group modified"
                Case -7
                    description = "Authorizations of user modified indirectly via group membershipment"
                Case -6
                    description = "Authorizations of user modified"
                Case -5
                    description = "Account lock has been resetted by admin"
                Case -4
                    description = "User profile modified by the user itself"
                Case 0
                    description = "Page hit"
                Case 1
                    description = "User created by the user itself"
                Case 2
                    description = "Password requested for sending via e-mail"
                Case 3
                    description = "User created by admin"
                Case 4
                    description = "User profile modified by admin"
                Case 5
                    description = "User password modified by admin"
                Case 6
                    description = "User password modified by the user itself"
                Case 97
                    description = "Preparation for GUID Login"
                Case 98
                    description = "Login"
                Case 99
                    description = "Logout"
                Case -70
                    description = "Runtime information"
                Case -71
                    description = "Runtime warning"
                Case -72
                    description = "Runtime exception"
                Case -80
                    description = "Application information"
                Case -81
                    description = "Application warning"
                Case -82
                    description = "Application exception"
                Case Else
                    description = ""
            End Select
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.NumberClicksDifferentIntervals
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - number of clicks of selected applications for different intervals
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class NumberClicksDifferentIntervals
        Inherits BaseControl

        Dim wd As String
        Dim ServerIP As String
        Dim Arr_value(1) As Integer
        Dim Arr_xaxis(1) As String
        Dim Ymax As Double
        Dim Ystep As Double
        Dim App As String
        Dim Arr_name(1000) As String

        Protected LabelDescription, LabelIsPeriodTooLong As Label
        Protected ApplicationDropList, IntervalDropList As DropDownList
        Protected Barchart1 As BarChart
        Protected Linechart1 As LineChart
        Protected cellApp, cellInt As TableCell
        Protected IntervalTable As Table
        Protected ReportNameCell As TableCell

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Title and description
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim title As String = "Number of clicks for different intervals"
            LabelDescription.Text = "This graphic shows the number of clicks of selected applications for the current period. " & _
                 "They are grouped depending on the selected interval (line chart for days, bar chart for weekdays, months and years)."
            ReportNameCell.Controls.Add(guiTable(title))
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creating of dropdown lists with application names and intervals
        ''' </summary>
        ''' <param name="DateFrom1"></param>
        ''' <param name="DateTo1"></param>
        ''' <param name="serverIP1"></param>
        ''' <param name="IsPrintView"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub LoadData(ByVal DateFrom1 As DateTime, ByVal DateTo1 As DateTime, ByVal serverIP1 As String, ByVal IsPrintView As Boolean)
            DateFrom = DateFrom1
            DateTo = DateTo1
            ServerIP = serverIP1
            Dim constr As String = cammWebManager.ConnectionString
            Dim myCon As New SqlConnection(constr)
            Dim selectSQL As String
            selectSQL = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select title from applications where " & _
                "systemapp<>1 group by title"
            Dim cmd1 As New SqlCommand(selectSQL, myCon)
            cmd1.CommandTimeout = 0
            Dim reader As SqlDataReader = Nothing
            Dim mode As String
            Dim i As Integer
            If Me.ViewState("_DataAlreadyLoaded") = False Then
                Dim arr_len As Integer
                Try
                    myCon.Open()
                    reader = cmd1.ExecuteReader()
                    Do While reader.Read()
                        Arr_name(arr_len) = reader(0) 'title
                        arr_len += 1
                    Loop
                    arr_len -= 1
                Catch ex As Exception
                    cammWebManager.Log.Warn(ex)
                Finally
                    If Not reader Is Nothing AndAlso Not reader.IsClosed Then
                        reader.Close()
                    End If
                    If Not cmd1 Is Nothing Then
                        cmd1.Dispose()
                    End If
                    If Not myCon Is Nothing Then
                        If myCon.State <> ConnectionState.Closed Then
                            myCon.Close()
                        End If
                        myCon.Dispose()
                    End If
                End Try
                Me.ViewState("name") = Arr_name
                Me.ViewState("len") = arr_len
                Me.ViewState("_DataAlreadyLoaded") = True
                IntervalDropList.SelectedIndex = 2
            End If
            Dim a As Integer = ApplicationDropList.SelectedIndex
            ApplicationDropList.Items.Clear()
            ApplicationDropList.Items.Add("(All applications)")
            ApplicationDropList.Items(0).Value = "all"
            For i = 0 To Me.ViewState("len")
                ApplicationDropList.Items.Add(Me.ViewState("name")(i))
            Next
            ApplicationDropList.SelectedIndex = a
            mode = IntervalDropList.SelectedItem.Text
            Dim CurrentDate1 As DateTime = DateTo.AddMonths(-3)
            Dim CurrentDate2 As DateTime = DateTo.AddMonths(-24)
            If CurrentDate1 > DateFrom And mode = "Day" Then
                mode = "Month"
                IntervalDropList.SelectedIndex = 2
                LabelIsPeriodTooLong.Text = "Timeperiod for the mode ""Day"" is too long!"
            ElseIf CurrentDate2 > DateFrom And mode = "Month" Then
                mode = "Year"
                IntervalDropList.SelectedIndex = 3
                LabelIsPeriodTooLong.Text = "Timeperiod for the mode ""Month"" is too long!"
            Else
                LabelIsPeriodTooLong.Text = ""
            End If
            cellApp.Text = "<b>Application: </b>" & ApplicationDropList.SelectedItem.Text
            cellInt.Text = "<b>Interval: </b>" & IntervalDropList.SelectedItem.Text
            If IsPrintView Then
                ApplicationDropList.Visible = True
                IntervalDropList.Visible = True
                IntervalTable.Visible = False
            Else
                ApplicationDropList.Visible = False
                IntervalDropList.Visible = False
                IntervalTable.Visible = True
            End If
            If IsPrintView Then arrays(mode)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the data for charts
        ''' </summary>
        ''' <param name="mode"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub arrays(ByVal mode As String)
            Dim diff As TimeSpan
            Dim days As Integer
            Dim max As Integer
            Dim k As Integer
            Dim scale_x As Integer
            diff = DateTo.Subtract(DateFrom)
            days = diff.Days
            axis_X(days, mode, k, scale_x)
            If ApplicationDropList.SelectedItem.Text = "(All applications)" Then
                App = " and applicationid not in (select id from applications where systemapp=1)"
            Else
                App = " and title = N'" & Replace(ApplicationDropList.SelectedItem.Text, "'", "''") & "'"
            End If
            CurrentDate = DateFrom
            If mode = "Day" Then
                For i As Integer = 0 To scale_x - 1
                    For j As Integer = 0 To k - 1
                        ReDim Preserve Arr_value(i * k + j) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        ReDim Preserve Arr_xaxis(i * k + j) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                        If j = 0 Then
                            Arr_xaxis(i * k + j) = CurrentDate.ToString("dd.MM.yy")
                        Else
                            Arr_xaxis(i * k + j) = ""
                        End If
                        Arr_value(i * k + j) = clicks(mode)
                        If Arr_value(i * k + j) > max Then max = Arr_value(i * k + j)
                        CurrentDate = CurrentDate.AddDays(1)
                    Next
                Next
                axis_Y(max)
                Barchart1.Visible = False
                Linechart1.Visible = True
                Linechart1.showlinechart(Arr_xaxis, Arr_value, Ymax, Ystep, Color.Red)
            ElseIf mode = "Month" Then
                For i As Integer = 0 To scale_x - 1
                    ReDim Preserve Arr_xaxis(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    ReDim Preserve Arr_value(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Arr_xaxis(i) = CurrentDate.ToString("MMM yy")
                    Arr_value(i) = clicks(mode)
                    If Arr_value(i) > max Then max = Arr_value(i)
                    CurrentDate = CurrentDate.AddMonths(1)
                Next
                axis_Y(max)
                Barchart1.Visible = True
                Linechart1.Visible = False
                Barchart1.showbarchart(Arr_xaxis, Arr_value, Ymax, Ystep, Color.Blue)
            ElseIf mode = "Weekday" Then
                Dim weekday() As String = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}
                For i As Integer = 0 To scale_x - 1
                    wd = i + 1
                    ReDim Preserve Arr_xaxis(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    ReDim Preserve Arr_value(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Arr_xaxis(i) = weekday(i)
                    Arr_value(i) = clicks(mode)
                    If Arr_value(i) > max Then max = Arr_value(i)
                Next
                axis_Y(max)
                Barchart1.Visible = True
                Linechart1.Visible = False
                Barchart1.showbarchart(Arr_xaxis, Arr_value, Ymax, Ystep, Color.Green)
            Else
                For i As Integer = 0 To scale_x - 1
                    ReDim Preserve Arr_xaxis(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    ReDim Preserve Arr_value(i) 'TODO: POOR PERFORMANCE --> REQUIRED REWRITE TO REDIM WITH PRESERVE ONLY ONCE! by JW on 2010-09-14
                    Arr_xaxis(i) = CurrentDate.Year
                    Arr_value(i) = clicks(mode)
                    If Arr_value(i) > max Then max = Arr_value(i)
                    CurrentDate = CurrentDate.AddYears(1)
                Next
                axis_Y(max)
                Barchart1.Visible = True
                Linechart1.Visible = False
                Barchart1.showbarchart(Arr_xaxis, Arr_value, Ymax, Ystep, Color.Purple)
            End If
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the data for horizontal axis
        ''' </summary>
        ''' <param name="days"></param>
        ''' <param name="mode"></param>
        ''' <param name="k"></param>
        ''' <param name="scale_x"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub axis_X(ByVal days As String, ByVal mode As String, ByRef k As Integer, ByRef scale_x As Integer)
            Dim m As Integer
            Dim i As Integer
            If mode = "Day" Then
                m = days
                If m / 12 < 1 Then
                    scale_x = m + 1
                    k = 1
                ElseIf m Mod 12 = 0 Then
                    scale_x = 13
                    k = m / 12
                Else
                    k = Int(m / 12) + 1
                    CurrentDate = DateFrom
                    Do While CurrentDate <= DateTo
                        CurrentDate = CurrentDate.AddDays(k)
                        i += 1
                    Loop
                    scale_x = i
                End If
            ElseIf mode = "Month" Then
                CurrentDate = DateFrom
                scale_x = 1
                Do While DateTo.Month <> CurrentDate.Month Or DateTo.Year <> CurrentDate.Year
                    scale_x += 1
                    CurrentDate = CurrentDate.AddMonths(1)
                Loop
            ElseIf mode = "Year" Then
                CurrentDate = DateFrom
                scale_x = 1
                Do While DateTo.Year <> CurrentDate.Year
                    scale_x += 1
                    CurrentDate = CurrentDate.AddYears(1)
                Loop
            End If
            If mode = "Weekday" Then scale_x = 7
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Calculation of the interval for the vertical axis
        ''' </summary>
        ''' <param name="cl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub axis_Y(ByVal cl As Single)
            Dim i As Integer
            Do While cl >= 10
                cl /= 10
                i += 1
            Loop
            Ymax = (Int(cl) + 1) * 10 ^ i
            Ystep = 10 ^ i
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Number of application's clicks
        ''' </summary>
        ''' <param name="mode"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Function clicks(ByVal mode As String) As Integer
            Dim constr As String = cammWebManager.ConnectionString
            Dim myCon As New SqlConnection(constr)
            Dim selectSQL1 As String
            If mode = "Day" Then
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                     "Select count(log.id) from log,applications where " & _
                            "logindate>=@DateFrom" & _
                            " and logindate<@DateTo " & _
                            ServerIP & App & " and conflicttype=0" & _
                            " and log.applicationid=applications.id" & _
                            " and userid not in (select id_user from memberships where id_group=6)"
            ElseIf mode = "Month" Then
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(log.id) from log,applications where " & _
                           "logindate>=@DateFrom" & _
                           " and logindate<=@DateTo" & _
                           " and month(logindate)=" & CurrentDate.Month & _
                           " and year(logindate)=" & CurrentDate.Year & _
                           ServerIP & App & " and conflicttype=0" & _
                           " and log.applicationid=applications.id" & _
                           " and userid not in (select id_user from memberships where id_group=6)"
            ElseIf mode = "Weekday" Then
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(log.id) from log,applications " & _
                            "where datepart(dw,logindate)=" & wd & _
                            " and logindate between @DateFrom and @DateTo " & _
                            ServerIP & App & " and conflicttype=0" & _
                            " and log.applicationid=applications.id" & _
                            " and userid not in (select id_user from memberships where id_group=6)" & _
                            " group by datepart(dw,logindate)"
            Else
                selectSQL1 = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; " & vbNewLine & _
                                    "Select count(log.id) from log,applications where " & _
                            "logindate>=@DateFrom" & _
                            " and logindate<=@DateTo" & _
                            " and year(logindate)=" & CurrentDate.Year & _
                            ServerIP & App & " and conflicttype=0" & _
                            " and log.applicationid=applications.id" & _
                            " and userid not in (select id_user from memberships where id_group=6)"
            End If
            Dim cmd1 As New SqlCommand(selectSQL1, myCon)
            cmd1.CommandTimeout = 0
            If mode = "Day" Then
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = CurrentDate
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = CurrentDate.AddDays(1)
            Else
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo.AddDays(1)
            End If
            Try
                myCon.Open()
                clicks = cmd1.ExecuteScalar()
            Catch ex As Exception
                cammWebManager.Log.Warn(ex)
            Finally
                If Not cmd1 Is Nothing Then
                    cmd1.Dispose()
                End If
                If Not myCon Is Nothing Then
                    If myCon.State <> ConnectionState.Closed Then
                        myCon.Close()
                    End If
                    myCon.Dispose()
                End If
            End Try
        End Function

    End Class


    ''' -----------------------------------------------------------------------------
    ''' Project	 : cammWMLogs
    ''' Class	 : camm.WebManager.Modules.LogAnalysis.Controls.LogMenu
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control - menu navigation
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Sergeev]	28.09.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class LogMenu
        Inherits BaseControl

        Protected Navigation As Table

        Dim WithEvents RAMenu As ComponentArt.Web.UI.Menu

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim rowNew As New TableRow
            Dim MyMenuTCell As New TableCell
            MyMenuTCell.Controls.Add(Me.BuildMenu())
            rowNew.Controls.Add(MyMenuTCell)
            Navigation.Controls.Add(rowNew)
            Navigation.CellPadding = 0
            Navigation.CellSpacing = 0
            Navigation.Width = Unit.Percentage(100)
            Navigation.CssClass = "TopGroup"
        End Sub

        Private Sub PopulateSubItem(ByVal dbRow As System.Data.DataRow, ByVal itm As ComponentArt.Web.UI.MenuItem)
            Dim childRow As System.Data.DataRow = Nothing
            Dim childitm As New ComponentArt.Web.UI.MenuItem
            'TODO: How the heck should this work? Access to a datarow item of a datarow that is nothing.
            childitm = CreateItem(childRow("MenuText").ToString(), childRow("WebPage").ToString(), True)
            itm.Items.Add(childitm)
            PopulateSubItem(childRow, childitm)
        End Sub

        Private Function CreateItem(ByVal MenuText As String, ByVal MenuID As String, ByVal WebPage As String) As ComponentArt.Web.UI.MenuItem
            Dim itm As New ComponentArt.Web.UI.MenuItem
            itm.Text = MenuText
            itm.ID = MenuID
            itm.NavigateUrl = WebPage
            Return itm
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creating of the menu
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Sergeev]	28.09.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        <CLSCompliantAttribute(False)> Public Function BuildMenu() As ComponentArt.Web.UI.Menu
            RAMenu = New ComponentArt.Web.UI.Menu
            RAMenu.ID = "LogMenu"
            RAMenu.DefaultGroupCssClass = "MenuGroup"

            Dim itemTop As ComponentArt.Web.UI.MenuItem
            Dim itemChildBase As ComponentArt.Web.UI.MenuItem
            Dim itemChildBase2 As ComponentArt.Web.UI.MenuItem
            Dim itemLook As New ComponentArt.Web.UI.ItemLook

            'add item css
            RAMenu.DefaultGroupCssClass = "MenuGroup"
            itemLook.CssClass = "MenuItem"
            RAMenu.ItemLooks.Add(itemLook)

            'add first column to menu control
            itemTop = New ComponentArt.Web.UI.MenuItem
            itemTop = CreateItem("Summaries", "groupSummaries", "")
            itemTop.Width = Web.UI.WebControls.Unit.Pixel(100)
            itemTop.CssClass = "TopMenuItem"
            With itemTop.Items
                .Add(CreateItem("Marketing department", "itemMarketingDep", "report_marketing.aspx"))
                .Add(CreateItem("Sales department", "itemSalesDep", "report_sales.aspx"))
                .Add(CreateItem("Security administration", "itemSecurityAdm", "report_security_administration.aspx"))
                .Add(CreateItem("Web-Master", "itemWebmaster", "report_webmaster.aspx"))
                .Add(CreateItem("Network administration", "itemNetworkAdm", "report_network_administration.aspx"))
            End With
            RAMenu.Items.Add(itemTop)

            'add second column to menu control
            itemTop = New ComponentArt.Web.UI.MenuItem
            itemTop = CreateItem("Current activity", "itemCurrentAct", "current_activity.aspx")
            itemTop.Width = Web.UI.WebControls.Unit.Pixel(100)
            RAMenu.Items.Add(itemTop)

            'add third column to menu control
            itemTop = New ComponentArt.Web.UI.MenuItem
            itemTop = CreateItem("Detailed information", "groupInformation", "")
            itemTop.Width = Web.UI.WebControls.Unit.Pixel(100)
            itemTop.CssClass = "TopMenuItem"
            itemChildBase = New ComponentArt.Web.UI.MenuItem
            itemChildBase = CreateItem("Clicks", "groupClicks", "")
            itemChildBase.CssClass = "TopMenuItem"
            With itemChildBase.Items
                .Add(CreateItem("Application clicks", "itemAppClicks", "application_clicks.aspx"))
                .Add(CreateItem("User clicks", "itemUserClicks", "user_clicks.aspx"))
                .Add(CreateItem("Application clicks by users", "itemAppClicksUsers", "application_clicks_by_users.aspx"))
                .Add(CreateItem("Application history", "itemAppHist", "application_history.aspx"))
                .Add(CreateItem("Server groups history", "itemServerGroupHist", "server_group_history.aspx"))
            End With
            itemTop.Items.Add(itemChildBase)

            itemChildBase = New ComponentArt.Web.UI.MenuItem
            itemChildBase = CreateItem("Users", "groupUsers", "")
            itemChildBase.CssClass = "TopMenuItem"
            itemChildBase2 = New ComponentArt.Web.UI.MenuItem
            itemChildBase2 = CreateItem("New users", "groupNewUsers", "")
            itemChildBase2.CssClass = "TopMenuItem"
            With itemChildBase2.Items
                .Add(CreateItem("Overview", "itemNUOverview", "new_users_overview.aspx"))
                .Add(CreateItem("List of users", "itemNUList", "new_users_list.aspx"))
                .Add(CreateItem("Contact details", "itemNUDetails", "new_users_details.aspx"))
            End With
            itemChildBase.Items.Add(itemChildBase2)

            itemChildBase2 = New ComponentArt.Web.UI.MenuItem
            itemChildBase2 = CreateItem("Updated user profiles", "groupUpdatedUsers", "")
            itemChildBase2.CssClass = "TopMenuItem"
            With itemChildBase2.Items
                .Add(CreateItem("Overview", "itemUUOverview", "updated_profiles_overview.aspx"))
                .Add(CreateItem("List of users", "itemDUList", "updated_profiles_list.aspx"))
            End With
            itemChildBase.Items.Add(itemChildBase2)

            itemChildBase2 = New ComponentArt.Web.UI.MenuItem
            itemChildBase2 = CreateItem("Deleted users", "groupDeletedUsers", "")
            itemChildBase2.CssClass = "TopMenuItem"
            With itemChildBase2.Items
                .Add(CreateItem("Overview", "itemDUOverview", "deleted_users_overview.aspx"))
                .Add(CreateItem("List of users", "itemDUList", "deleted_users_list.aspx"))
            End With
            itemChildBase.Items.Add(itemChildBase2)

            With itemChildBase.Items
                .Add(CreateItem("Trace log", "itemAppViewTraceLog", "view_trace_log.aspx"))
                .Add(CreateItem("Latest logon dates", "itemAppLatestLogonDates", "latest_logon_dates.aspx"))
            End With
            itemTop.Items.Add(itemChildBase)

            With itemTop.Items
                .Add(CreateItem("Redirections", "itemAppredirections", "redirections.aspx"))
                .Add(CreateItem("Event log", "itemEventLog", "event_log.aspx"))
            End With

            RAMenu.Items.Add(itemTop)

            Return RAMenu
        End Function

    End Class

End Namespace
