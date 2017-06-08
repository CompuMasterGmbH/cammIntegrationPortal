Option Explicit On
Option Strict Off

Imports System.Data
Imports System.Drawing
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Modules.LogAnalysis.Pages

    ''' <summary>
    '''     Base page for all log analysis
    ''' </summary>
    ''' <remarks>
    '''     Common tasks are the definition of the security object name and the providing of common calendar and button controls
    ''' </remarks>
    Public Class Page
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
        Protected TitleTRow As TableRow
        Protected AppNameCell, TitleTCell As TableCell
        Protected ButtonPrintView As Button
        Protected PrintViewTable As Table
        Protected WithEvents CalendarFrom, CalendarTo As Calendar
        Protected CellServerGroupsPrintTable, CellTo, CellFrom As TableCell
        Protected CheckBoxCell As HtmlControls.HtmlTableCell
        Protected SelectTable As HtmlControls.HtmlTable
        Protected PrintPage As HtmlControls.HtmlInputButton

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
            TitleTRow.Visible = True
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
            ButtonPrintView.Visible = False
            IsPrintView = False
            SelectTable.Visible = False
            PrintViewTable.Visible = True
            PrintPage.Visible = True
            TitleTRow.Visible = False
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
    Public Class EventLogInfo
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
            selectSQL = "Select * from log where id= @id"
            Dim myCon As New SqlClient.SqlConnection(constr)
            Dim cmd1 As New SqlClient.SqlCommand(selectSQL, myCon)
            cmd1.Parameters.Add("@ID", SqlDbType.Int).Value = CType(id, Integer)
            cmd1.CommandTimeout = 0
            Dim reader As SqlClient.SqlDataReader = Nothing
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
                Dim app_info As New CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation(app_id, cammWebManager, True)
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
            CellFrom.Text = "<b>From: </b>" & TextboxDateFrom.Text
            CellTo.Text = "<b>To: </b>" & TextboxDateTo.Text
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
            TitleTRow.Visible = True
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
            ButtonPrintView.Visible = False
            IsPrintView = False
            SelectTable.Visible = False
            printViewTable.Visible = True
            PrintPage.Visible = True
            TitleTRow.Visible = False
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

        Private _UserNames As New System.Collections.Generic.Dictionary(Of Long, String)
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
            If _UserNames.ContainsKey(user_id) = True Then
                Return _UserNames(user_id)
            Else
                Dim Result As String
                WM = cammWebManager
                Try
                    Dim ln As String = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).LoginName)
                    Result = Server.HtmlEncode(cammWebManager.System_GetUserInfo(user_id).FullName) & " (" & ln & ")"
                Catch ex As Exception
                    If user_id = -1 Then
                        Result = "Unknown user"
                    Else
                        Dim UserInfo As New CompuMaster.camm.WebManager.WMSystem.UserInformation(user_id, WM, True)
                        Result = Server.HtmlEncode(UserInfo.FullName) & " (ID " & user_id & ")"
                    End If
                End Try
                If _UserNames.ContainsKey(user_id) = False Then
                    _UserNames.Add(user_id, Result)
                End If
                Return Result
            End If
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
                cft(0) = WMSystem.Logging_ConflictTypes.RuntimeWarning '-71
            End If
            If CheckboxRuntimeExceptions.Checked Then
                chk_title(1) = CheckboxRuntimeExceptions.Text
                chk_state(1) = True
                cft(1) = WMSystem.Logging_ConflictTypes.RuntimeException '-72
            End If
            If CheckboxRuntimeInformation.Checked Then
                chk_title(2) = CheckboxRuntimeInformation.Text
                chk_state(2) = True
                cft(2) = WMSystem.Logging_ConflictTypes.RuntimeInformation '-70
            End If
            If CheckboxApplicationWarnings.Checked Then
                chk_title(3) = CheckboxApplicationWarnings.Text
                chk_state(3) = True
                cft(3) = WMSystem.Logging_ConflictTypes.ApplicationWarning '-81
            End If
            If CheckboxApplicationExceptions.Checked Then
                chk_title(4) = CheckboxApplicationExceptions.Text
                chk_state(4) = True
                cft(4) = WMSystem.Logging_ConflictTypes.ApplicationException '-82
            End If
            If CheckboxApplicationInformation.Checked Then
                chk_title(5) = CheckboxApplicationInformation.Text
                chk_state(5) = True
                cft(5) = WMSystem.Logging_ConflictTypes.ApplicationInformation '-80
            End If
            If CheckboxLogin.Checked Then
                chk_title(6) = CheckboxLogin.Text
                chk_state(6) = True
                cft(6) = WMSystem.Logging_ConflictTypes.Login '98
            End If
            If ChecboxkLogout.Checked Then
                chk_title(7) = ChecboxkLogout.Text
                chk_state(7) = True
                cft(7) = WMSystem.Logging_ConflictTypes.Logout '99
            End If
            If CheckboxDebug.Checked Then
                chk_title(8) = CheckboxDebug.Text
                chk_state(8) = True
                cft(8) = WMSystem.Logging_ConflictTypes.Debug '-99
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
            selectSQL = "Select id,userid,logindate,conflicttype,conflictdescription,isnull(reviewedandclosed,0) from log where " &
               "logindate between @DateFrom and @DateTo " &
               serverIP & " and (" & conflict &
               ") and isnull(reviewedandclosed,0)=0 order by logindate desc"
            Dim myCon As New SqlClient.SqlConnection(constr)
            Dim cmd1 As New SqlClient.SqlCommand(selectSQL, myCon)
            Dim cmd2 As New SqlClient.SqlCommand(UpdateSQL, myCon)
            cmd1.CommandTimeout = 0
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
            cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
            Dim reader As SqlClient.SqlDataReader = Nothing
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
                CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.CloseAndDisposeConnection(myCon)
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
                    Return "{unexpected type: " & id & "}"
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
            LabelDescription.Text = "This chart shows all selected events for current periods. Every row refers to the " &
                  "page with information about a user which has called a event, date, server group and application. " &
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
                e.Item.Cells(1).Attributes.Add("onclick", "popup('event_log_info.aspx?id=" & e.Item.Cells(0).Text & "&nm=" & e.Item.Cells(1).Text & "&ct=" & e.Item.Cells(2).Text & "');")
                e.Item.Cells(2).Attributes.Add("onclick", "popup('event_log_info.aspx?id=" & e.Item.Cells(0).Text & "&nm=" & e.Item.Cells(1).Text & "&ct=" & e.Item.Cells(2).Text & "');")
                e.Item.Cells(3).Attributes.Add("onclick", "popup('event_log_info.aspx?id=" & e.Item.Cells(0).Text & "&nm=" & e.Item.Cells(1).Text & "&ct=" & e.Item.Cells(2).Text & "');")
                counter += 1
            End If
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
            LabelDescription.Text = "All redirections for the selected periods are shown here. The number of clicks " &
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
                selectSQL1 = "Select description, clicks from redirects_toaddr, " &
                   "(Select id_redirector, count(*) as clicks from redirects_log " &
                   "where accessdatetime between @DateFrom and @DateTo" &
                   " group by id_redirector) as t" &
                   " where redirects_toaddr.id=id_redirector" & vbNewLine &
                   "ORDER by " & sortList
                Dim myCon As New SqlClient.SqlConnection(constr)
                Dim cmd1 As New SqlClient.SqlCommand(selectSQL1, myCon)
                cmd1.CommandTimeout = 0
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateFrom", SqlDbType.DateTime)).Value = DateFrom
                cmd1.Parameters.Add(New SqlClient.SqlParameter("@DateTo", SqlDbType.DateTime)).Value = DateTo
                Dim reader As SqlClient.SqlDataReader = Nothing
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
                diagrImg.ImageUrl = "/system/admin/images/blue.png"
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

End Namespace
