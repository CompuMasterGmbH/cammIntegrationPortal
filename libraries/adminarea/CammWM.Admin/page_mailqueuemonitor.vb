'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden für Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Strict Off
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Namespace CompuMaster.camm.WebManager.Controls.Administration

#Region " Public Class ActionControl "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Controls.Administration.MailQueueMonitorActionControl
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Processes action events show email text, resend email, accept failure.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[patil]	25.11.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class MailQueueMonitorActionControl
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        Protected WithEvents HyperLinkShowEmailText As System.Web.UI.WebControls.HyperLink
        Protected WithEvents LinkbuttonResend As System.Web.UI.WebControls.LinkButton
        Protected WithEvents LinkbuttonFailure As System.Web.UI.WebControls.LinkButton
        Protected WithEvents LinkbuttonSendThisEmailToMe As System.Web.UI.WebControls.LinkButton

        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Select Case Me._SecurityRole
                Case CompuMaster.camm.WebManager.Pages.Administration.MailQueueMonitor.SecurityRoles.Supervisor
                    Me.LinkbuttonFailure.ToolTip = "Accept failure"
                    Me.LinkbuttonResend.ToolTip = "Send this e-mail again"
                    Me.LinkbuttonSendThisEmailToMe.ToolTip = "Send this e-mail to me"
                    Select Case Me._QueueState
                        Case Messaging.QueueMonitoring.QueueStates.FailureAfter1Trial, Messaging.QueueMonitoring.QueueStates.FailureAfter2Trials, Messaging.QueueMonitoring.QueueStates.FailureAfterLastTrial
                            Me.LinkbuttonResend.Visible = True
                            Me.LinkbuttonFailure.Visible = True
                            Me.LinkbuttonSendThisEmailToMe.Visible = True
                        Case Messaging.QueueMonitoring.QueueStates.FailureAccepted
                            Me.LinkbuttonResend.Visible = True
                            Me.LinkbuttonFailure.Visible = False
                            Me.LinkbuttonSendThisEmailToMe.Visible = False
                        Case Else
                            Me.LinkbuttonResend.Visible = False
                            Me.LinkbuttonFailure.Visible = False
                            Me.LinkbuttonSendThisEmailToMe.Visible = False
                    End Select
                Case Pages.Administration.MailQueueMonitor.SecurityRoles.SecurityOperator
                    Me.LinkbuttonFailure.ToolTip = "Accept failure"
                    Me.LinkbuttonResend.ToolTip = "Send this e-mail again"
                    Me.LinkbuttonSendThisEmailToMe.ToolTip = "Send this e-mail to me"
                    Select Case Me._QueueState
                        Case Messaging.QueueMonitoring.QueueStates.FailureAfter1Trial, Messaging.QueueMonitoring.QueueStates.FailureAfter2Trials, Messaging.QueueMonitoring.QueueStates.FailureAfterLastTrial
                            Me.LinkbuttonResend.Visible = True
                            Me.LinkbuttonFailure.Visible = True
                            Me.LinkbuttonSendThisEmailToMe.Visible = False
                        Case Messaging.QueueMonitoring.QueueStates.FailureAccepted
                            Me.LinkbuttonResend.Visible = True
                            Me.LinkbuttonFailure.Visible = False
                            Me.LinkbuttonSendThisEmailToMe.Visible = False
                        Case Else
                            Me.LinkbuttonResend.Visible = False
                            Me.LinkbuttonFailure.Visible = False
                            Me.LinkbuttonSendThisEmailToMe.Visible = False
                    End Select

                Case Else
                    Me.LinkbuttonFailure.Visible = False
                    Me.LinkbuttonResend.Visible = False
                    Me.LinkbuttonSendThisEmailToMe.Visible = False
            End Select

            If Me._IsAuthorisedToSeeEmailText Then
                Me.HyperLinkShowEmailText.ToolTip = "Show e-mail text"
                Me.HyperLinkShowEmailText.Attributes.Add("onclick", ("window.open('mailqueue_monitor_showemail.aspx?eid=" & Me._EmailID.ToString & "', '1', 'width=550, height=350, resizable=yes scrollbars=yes'); return (false);"))
            Else
                Me.HyperLinkShowEmailText.Visible = False
            End If

        End Sub

        Private _EmailID As Integer
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Unique ID of email analysis record
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend WriteOnly Property EmailID() As Integer
            Set(ByVal Value As Integer)
                Me._EmailID = Value
            End Set
        End Property

        Private _QueueState As Messaging.QueueMonitoring.QueueStates
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Email queue state
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend WriteOnly Property QueueState() As Messaging.QueueMonitoring.QueueStates
            Set(ByVal Value As Messaging.QueueMonitoring.QueueStates)
                Me._QueueState = Value
            End Set
        End Property

        Private _IsAuthorisedToSeeEmailText As Boolean
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is user authorised to view/moderate this email
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend WriteOnly Property IsAuthorisedToSeeEmailText() As Boolean
            Set(ByVal Value As Boolean)
                Me._IsAuthorisedToSeeEmailText = Value
            End Set
        End Property

        Private _SecurityRole As Pages.Administration.MailQueueMonitor.SecurityRoles
        Friend WriteOnly Property SecurityRole() As Pages.Administration.MailQueueMonitor.SecurityRoles
            Set(ByVal Value As Pages.Administration.MailQueueMonitor.SecurityRoles)
                Me._SecurityRole = Value
            End Set
        End Property

        Friend ResultPage As Pages.Administration.MailQueueMonitor

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Event to resend the email
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkbuttonResend_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkbuttonResend.Click

            Dim d As New Pages.Administration.MailQueueMonitor.DataService
            d.ConnectionString = cammWebManager.ConnectionString
            Me.cammWebManager.MessagingQueueMonitoring.UpdateQueueState(Me._EmailID, Messaging.QueueMonitoring.QueueStates.Queued)

            Me.ResultPage.LoadData(cammWebManager.ConnectionString)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Accecpts email failure and updates record in database
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkbuttonFailure_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkbuttonFailure.Click
            Me.cammWebManager.MessagingQueueMonitoring.UpdateQueueState(Me._EmailID, Messaging.QueueMonitoring.QueueStates.FailureAccepted)
            Me.ResultPage.LoadData(cammWebManager.ConnectionString)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sends this email to the Supervisor.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	05.12.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkbuttonSendThisEmailToMe_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkbuttonSendThisEmailToMe.Click
            Dim Mail As Messaging.MailMessage = Me.cammWebManager.MessagingQueueMonitoring.LoadMailMessage(Me._EmailID)

            Dim userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation = Me.cammWebManager.CurrentUserInfo

            Me.cammWebManager.MessagingEMails.QueueEMail(userInfo.FullName, _
                userInfo.EMailAddress, _
                Mail.Subject, _
                Mail.BodyPlainText, _
                Mail.BodyHtml, _
                userInfo.FullName, _
                userInfo.EMailAddress)

            'set status to Failure accepted
            Me.cammWebManager.MessagingQueueMonitoring.UpdateQueueState(Me._EmailID, Messaging.QueueMonitoring.QueueStates.FailureAccepted)

            Me.ResultPage.LoadData(cammWebManager.ConnectionString)

        End Sub

    End Class
#End Region

End Namespace

Namespace CompuMaster.camm.WebManager.Pages.Administration

#Region " Public Class MailQueueMonitorDatePicker "
    ''' <summary>
    '''     Used to choose date
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Class MailQueueMonitorDatePicker
        Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

#Region " Protected "
        Protected WithEvents CalendarDatePicker As Calendar
        Protected TextDate As TextBox
        Protected WithEvents PanelShowDatePicker As System.Web.UI.WebControls.Panel
        Protected WithEvents ButtonOK As Button
#End Region

        Protected Overridable Function ServerFormClientID() As String
            Return Request.QueryString("FormName")
        End Function

        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            ' Hier Benutzercode zur Seiteninitialisierung einf?gen

            Dim tbn As String = HttpContext.Current.Request.QueryString("tbn")
            Dim myScript As String = "<script language=""javascript""> function ReturnDate() { window.opener.document.getElementById('" & tbn & "').value = document.getElementById('TextDate').value; } </script> "
#If VS2015OrHigher = True Then
#Disable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
            Me.Page.RegisterClientScriptBlock("ReturnDate", myScript)
#If VS2015OrHigher = True Then
#Enable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
            ButtonOK.Attributes.Add("onClick", "javascript:window.close();")
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     displays date choosen by user in text box
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub Calendar_SelectionChanged(ByVal s As Object, ByVal e As EventArgs) Handles CalendarDatePicker.SelectionChanged
            TextDate.Text = CalendarDatePicker.SelectedDate.ToShortDateString
        End Sub

    End Class
#End Region

#Region " Public Class MailQueueMonitorShowEmail "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitorShowEmail
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Displays email content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[patil]	25.11.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class MailQueueMonitorShowEmail
        Inherits CompuMaster.camm.WebManager.pages.Administration.Page

        Protected LabelTitle As System.Web.UI.WebControls.Label
        Protected LabelEmailText As System.Web.UI.WebControls.Label
        Protected ButtonClose As System.Web.UI.WebControls.Button
        Protected LabelMessageHtmlFormat As System.Web.UI.WebControls.Label
        Protected LabelHtmlText As System.Web.UI.WebControls.Label
        Protected LabelMessageTextFormat As System.Web.UI.WebControls.Label

        Protected AreaHtmlText As System.Web.UI.HtmlControls.HtmlTableCell

        Private data As New Pages.Administration.MailQueueMonitor.DataService

        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            data.ConnectionString = cammWebManager.ConnectionString
            Me.ButtonClose.Attributes.Add("onclick", "javascript:window.close();")

            If HttpContext.Current.Request.QueryString("eid") = Nothing Then
                Me.LabelEmailText.Text = ""
            Else
                Try
                    Dim emailID As Integer = CInt(HttpContext.Current.Request.QueryString("eid"))
                    Dim Mail As Messaging.MailMessage = Me.cammWebManager.MessagingQueueMonitoring.LoadMailMessage(emailID)

                    If Me.IsAuthorisedToSeeEmailText(Mail.FromAddress, Mail.To, Mail.Cc, Mail.Bcc) Then
                        If Trim(Mail.BodyHtml) = "" Then
                            Me.AreaHtmlText.Visible = False
                        Else
                            Me.LabelHtmlText.Text = Mail.BodyHtml
                        End If

                        Me.LabelEmailText.Text = CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(Server.HtmlEncode(Mail.BodyPlainText))
                    Else
                        Me.AreaHtmlText.Visible = False
                        Me.LabelHtmlText.Text = ""
                        Me.LabelEmailText.Text = ""
                    End If
                Catch ex As Exception
                    Me.AreaHtmlText.Visible = False
                    Me.LabelHtmlText.Text = ""
                    Me.LabelEmailText.Text = ""
                End Try
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for current user authorization to see email body
        ''' </summary>
        ''' <param name="fromAddress">Senders address</param>
        ''' <param name="toAddress">receipients address</param>
        ''' <param name="cc">receipients address in CC</param>
        ''' <param name="bcc">receipients address Bcc</param>
        ''' <returns>Returns True if user is authorised</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function IsAuthorisedToSeeEmailText(ByVal fromAddress As String, ByVal toAddress As String, ByVal cc As String, ByVal bcc As String) As Boolean
            Dim result As Boolean = False

            Select Case New Pages.Administration.MailQueueMonitor().SecurityRole(MyBase.cammWebManager)
                Case MailQueueMonitor.SecurityRoles.Supervisor
                    result = True
                Case MailQueueMonitor.SecurityRoles.SecurityOperator, MailQueueMonitor.SecurityRoles.User
                    If fromAddress <> Nothing Then
                        If fromAddress.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If toAddress <> Nothing Then
                        If toAddress.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If cc <> Nothing Then
                        If cc.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If bcc <> Nothing Then
                        If bcc.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                Case Else
                    result = False
            End Select

            Return result
        End Function

    End Class
#End Region

#Region " Public Class MailQueueMonitor "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Pages.Administration.MailQueueActivity
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     This page allows the supervisors to view the activity of the e-mail queue and to restart a mail item when there were some problems
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	21.11.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class MailQueueMonitor
        Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

#Region " Protected Variables "
        Protected LabelTitle As System.Web.UI.WebControls.Label

        Protected LabelPossibilitiesToFiler As System.Web.UI.WebControls.Label
        Protected WithEvents TextBoxID As System.Web.UI.WebControls.TextBox
        Protected WithEvents TextboxFromAddress As System.Web.UI.WebControls.TextBox
        Protected WithEvents TextboxSubject As System.Web.UI.WebControls.TextBox
        Protected WithEvents DropDownListState As System.Web.UI.WebControls.DropDownList
        Protected WithEvents CheckBoxListState As System.Web.UI.WebControls.CheckBoxList
        Protected WithEvents TextboxToAddress As System.Web.UI.WebControls.TextBox
        Protected WithEvents TextboxFrom As System.Web.UI.WebControls.TextBox
        Protected WithEvents HyperlinkCalendarFrom As System.Web.UI.WebControls.HyperLink
        Protected WithEvents HyperLinkCalendarTo As System.Web.UI.WebControls.HyperLink
        Protected WithEvents RadioButtonListToCcBcc As System.Web.UI.WebControls.RadioButtonList
        Protected WithEvents TextboxTo As System.Web.UI.WebControls.TextBox

        Protected WithEvents TextboxCreateReportName As System.Web.UI.WebControls.TextBox

        Protected WithEvents ButtonFilterNow As System.Web.UI.WebControls.Button
        Protected WithEvents ButtonReset As System.Web.UI.WebControls.Button
        Protected WithEvents ButtonExportReport As System.Web.UI.WebControls.Button

        Protected WithEvents LinkButtonState As System.Web.UI.WebControls.LinkButton
        Protected WithEvents LinkButtonSender As System.Web.UI.WebControls.LinkButton
        Protected WithEvents LinkButtonSubject As System.Web.UI.WebControls.LinkButton
        Protected WithEvents LinkButtonID As System.Web.UI.WebControls.LinkButton
        Protected WithEvents LinkButtonSentTime As System.Web.UI.WebControls.LinkButton

        Protected WithEvents HyperLinkDownloadCenter As System.Web.UI.WebControls.HyperLink

        Protected WithEvents TableAnalysis As System.Web.UI.WebControls.Table

#End Region

        Private data As New DataService
        Private boolFaliureStatus As Boolean

#Region " Sub PageOnLoad "
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Controls on web page are initialized 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
                Me.ButtonFilterNow.Attributes.Add("onclick", "javascript:return checkFilterNowValidity();")
                Me.InitializeControls()
                data.ConnectionString = cammWebManager.ConnectionString

                If Not Page.IsPostBack Then
                    _LoadData(cammWebManager.ConnectionString)
                    LoadData(cammWebManager.ConnectionString)
                Else
                    If Not CachedDataFromViewState Is Nothing Then
                        Dim plain As String = CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.ConvertToPlainTextTable(CachedDataFromViewState)
                        Me.AddAnalysis(CachedDataFromViewState)
                        'Else
                        '    _LoadData()
                        '    LoadData()
                    End If
                End If

                If CheckBoxListState.Items.Count > 0 Then
                    boolFaliureStatus = CheckFaliureStatus()
                    If Not boolFaliureStatus Then
                        TableAnalysis.Rows(0).Cells(8).Visible = False
                    Else
                        TableAnalysis.Rows(0).Cells(8).Visible = True
                    End If
                End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Email analysis done and added to page
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub PageOnPreRender(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
                LoadData(cammWebManager.ConnectionString)
                TextboxFrom.Text = Trim(HttpContext.Current.Request.Form("TextboxFrom"))
                TextboxTo.Text = Trim(HttpContext.Current.Request.Form("TextboxTo"))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     The ClientID of the server form to access it with JavaScript
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.02.2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Function ServerFormClientID() As String
            Return Me.LookupServerForm().ClientID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load the data
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	29.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub _LoadData(ByVal ConnectionString As String)
            Dim analysis As DataTable
            If Page.IsPostBack OrElse CachedDataFromViewState Is Nothing Then
                Me.DataToFilter = Me.GetDataToFilterFromWebPage
                analysis = FilteredAnalysisDataTable(ConnectionString)
            Else
                analysis = Me.DefaultAnalysisDataTable(ConnectionString)
            End If
            Me.AddAnalysis(analysis)
            CachedDataFromViewState = analysis
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Load the data from the database when it hasn't already been loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	29.11.2005	Created
        ''' </history>
        ''' ---------------------------------------------------------------------------
        Friend Sub LoadData(ByVal ConnectionString As String)
            Static DataAlreadyLoaded As Boolean
            If DataAlreadyLoaded = False Then
                _LoadData(ConnectionString)
                DataAlreadyLoaded = True
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Cache the output data from the old request for the new post-back-request
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	29.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property CachedDataFromViewState() As DataTable
            Get
                If viewstate("DataSet") Is Nothing Then
                    Return Nothing
                Else
                    Dim ds As DataSet = viewstate("DataSet")
                    If ds.Tables.Count = 1 Then
                        Return ds.Tables(0)
                    Else
                        Return Nothing
                    End If
                End If
            End Get
            Set(ByVal Value As DataTable)
                Dim ds As New DataSet
                ds.Tables.Add(Value)
                viewstate("DataSet") = ds
            End Set
        End Property

#Region " Authorization "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Security roles in Mail Queue Monitor
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Enum SecurityRoles As Byte
            Supervisor = 11
            SecurityOperator = 12
            User = 13
        End Enum

        Private _SecurityRole As SecurityRoles
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Security role of current user
        ''' </summary>
        ''' <param name="cammWebManager"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function SecurityRole(ByVal cammWebManager As CompuMaster.camm.WebManager.WMSystem) As SecurityRoles
            Dim result As SecurityRoles

            If Me._SecurityRole = Nothing Then
                If cammWebManager.System_IsSuperVisor(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                    result = SecurityRoles.Supervisor
                ElseIf cammWebManager.System_IsSecurityOperator(cammWebManager.CurrentUserID(WMSystem.SpecialUsers.User_Anonymous)) Then
                    result = SecurityRoles.SecurityOperator
                Else
                    result = SecurityRoles.User
                End If
            Else
                result = Me._SecurityRole
            End If

            Return result
        End Function

#End Region
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Initializes controls on web page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InitializeControls()
            If Not Me.Page.IsPostBack Then
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.WaitingForReleaseBeforeQueuing.ToString, CByte(Messaging.QueueMonitoring.QueueStates.WaitingForReleaseBeforeQueuing).ToString, False)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.Queued.ToString, CByte(Messaging.QueueMonitoring.QueueStates.Queued).ToString, False)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.Sending.ToString, CByte(Messaging.QueueMonitoring.QueueStates.Sending).ToString, False)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.Cancelled.ToString, CByte(Messaging.QueueMonitoring.QueueStates.Cancelled).ToString, False)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.Successfull.ToString, CByte(Messaging.QueueMonitoring.QueueStates.Successfull).ToString, False)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.FailureAfter1Trial.ToString, CByte(Messaging.QueueMonitoring.QueueStates.FailureAfter1Trial).ToString, True)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.FailureAfter2Trials.ToString, CByte(Messaging.QueueMonitoring.QueueStates.FailureAfter2Trials).ToString, True)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.FailureAfterLastTrial.ToString, CByte(Messaging.QueueMonitoring.QueueStates.FailureAfterLastTrial).ToString, True)
                Me.AddListItem(Me.CheckBoxListState, Messaging.QueueMonitoring.QueueStates.FailureAccepted.ToString, CByte(Messaging.QueueMonitoring.QueueStates.FailureAccepted).ToString, False)

                Dim item As ListItem
                item = New ListItem("All", "all")
                item.Selected = True
                Me.RadioButtonListToCcBcc.Items.Add(item)
                Me.RadioButtonListToCcBcc.Items.Add(New ListItem("To", "to"))
                Me.RadioButtonListToCcBcc.Items.Add(New ListItem("Cc", "cc"))
                Me.RadioButtonListToCcBcc.Items.Add(New ListItem("Bcc", "bcc"))
                data.ConnectionString = cammWebManager.ConnectionString

                HyperLinkCalendarTo.Attributes("onClick") = "window.open('mailqueue_monitor_datepicker.aspx?tbn=" & Me.TextboxTo.ClientID & "', 'mainqueue_monitor_datepicker_to', 'alwaysRaised=yes, left=200, top=200, screenX=200, screenY= 200, width=200, height=195, resizable=no scrollbars=no').focus(); return (false);"
                HyperlinkCalendarFrom.Attributes("onClick") = "window.open('mailqueue_monitor_datepicker.aspx?tbn=" & Me.TextboxFrom.ClientID & "', 'mainqueue_monitor_datepicker_from', 'alwaysRaised=yes, left=200, top=200, screenX=200, screenY= 200, width=200, height=195, resizable=no scrollbars=no').focus(); return (false);"
            End If
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checkboxlist add item
        ''' </summary>
        ''' <param name="ctrl">Control to which items to be added</param>
        ''' <param name="text">text of item</param>
        ''' <param name="value">value of item</param>
        ''' <param name="checked">denotes whether item is selected or not</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddListItem(ByVal ctrl As ListControl, ByVal text As String, ByVal value As String, ByVal checked As Boolean)
            Dim item As ListItem = New ListItem
            item.Text = text
            item.Value = value
            item.Attributes.Add("alt", value)
            item.Selected = checked
            ctrl.Items.Add(item)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Default analysis as Data table
        ''' </summary>
        ''' <returns>DataTable</returns>
        Private Function DefaultAnalysisDataTable(ByVal ConnectionString As String) As DataTable
            Dim result As New DataTable
            With result.Columns
                .Add("AddHiddenArea", GetType(System.Boolean))
                .Add("ToAddressCount", GetType(System.Int32))
                .Add("EmailID", GetType(System.Int32))
                .Add("Subject", GetType(System.String))
                .Add("State", GetType(Messaging.QueueMonitoring.QueueStates))
                .Add("Sender", GetType(System.String))
                .Add("ToAddress", GetType(System.String))
                .Add("SentTime", GetType(System.DateTime))
                .Add("FromAddress", GetType(System.String))
                .Add("Cc", GetType(System.String))
                .Add("Bcc", GetType(System.String))
                .Add("ToType", GetType(MailQueueMonitor.ToType))
                .Add("DRowUserID", GetType(System.Int64))
            End With

            Dim statesToFilter As String = Me.SelectedStatesToFilter
            Dim logDT As DataTable = Me.data.LoadMailMessages(ConnectionString, Me.DataToFilter.EmailID, statesToFilter, Me.DataToFilter.FromDate, Me.DataToFilter.ToDate)
            If logDT.Rows.Count = 0 Then
                Return result
            End If

            Dim rowCounter As Integer = 1
            For Each dRow As DataRow In logDT.Rows
                Dim queueState As Messaging.QueueMonitoring.QueueStates = CType(dRow("State"), Messaging.QueueMonitoring.QueueStates)

                Dim xmlData As String = CStr(dRow("Data"))
                Dim mail As New Messaging.MailMessage(xmlData, Me.cammWebManager)
                If Me.DoesLog_eMailMessageToBeListedForCurrentUser(mail.FromAddress, mail.To, mail.Cc, mail.Bcc) Then
                    Dim addRow As Boolean = Me.IsAddRowToFilteredTable(mail)

                    If addRow Then
                        Dim addHiddenArea As Boolean = False
                        If Trim(mail.Bcc) <> "" Then
                            addHiddenArea = True
                        End If
                        If Trim(mail.Cc) <> "" Then
                            addHiddenArea = True
                        End If

                        Dim toAddressCounter As Integer = 0
                        If mail.To.IndexOf(";") > 0 Then
                            For Each toAddrress As String In mail.To.Split(";"c)
                                If Trim(toAddrress) <> "" Then
                                    toAddressCounter += 1
                                End If
                            Next
                        End If
                        If toAddressCounter > 1 Then
                            addHiddenArea = True
                        End If

                        'Dim sender As String = "<" & Trim(mail.FromName) & "> " & Trim(mail.FromAddress)
                        Dim fromNm As String = Nothing
                        If Trim(mail.FromName).Length > 0 Then
                            fromNm = """" & Trim(mail.FromName) & """"
                        End If
                        Dim sender As String = fromNm & " <" & Trim(mail.FromAddress) & ">"
                        'sender = Trim(sender)

                        Dim row As DataRow = result.NewRow
                        row("AddHiddenArea") = CBool(addHiddenArea)
                        row("ToAddressCount") = CInt(toAddressCounter)
                        row("EmailID") = CInt(dRow("ID"))
                        If mail.Subject Is Nothing Then row("Subject") = String.Empty Else row("Subject") = mail.Subject.ToString.Trim
                        row("State") = CType(queueState, Messaging.QueueMonitoring.QueueStates)
                        row("Sender") = sender.ToString.Trim
                        If mail.To Is Nothing Then row("ToAddress") = String.Empty Else row("ToAddress") = mail.To.ToString.Trim
                        row("SentTime") = CDate(dRow("DateTime"))
                        If mail.FromAddress Is Nothing Then row("FromAddress") = String.Empty Else row("FromAddress") = mail.FromAddress.ToString.Trim
                        If mail.Cc Is Nothing Then row("Cc") = String.Empty Else row("Cc") = mail.Cc.ToString.Trim
                        If mail.Bcc Is Nothing Then row("Bcc") = String.Empty Else row("Bcc") = mail.Bcc.ToString.Trim
                        row("ToType") = CType(Me.DataToFilter.ToType, MailQueueMonitor.ToType)
                        row("DRowUserID") = CLng(dRow("UserID"))

                        result.Rows.Add(row)

                        rowCounter += 1
                    End If
                End If

                If rowCounter >= 500 Then
                    Exit For
                End If
            Next

            Return result
        End Function

        ''' <summary>
        '''     Adds analysis to web page
        ''' </summary>
        ''' <param name="analysis">DataTable holding analysis data</param>
        Private Sub AddAnalysis(ByVal analysis As DataTable)
            'Remove any old content, first (except of the very first line containing the head line descriptions)
            For MyDelCounter As Integer = Me.TableAnalysis.Rows.Count - 1 To 1 Step -1
                Me.TableAnalysis.Rows.RemoveAt(MyDelCounter)
            Next
            'Fill output table with new content
            Dim myCounter As Integer = 1
            For Each dRow As DataRow In analysis.Select("", Me.SortString)
                Dim AddHiddenArea As Boolean = dRow("AddHiddenArea")
                Dim ToAddressCount As Integer = dRow("ToAddressCount")
                Dim Counter As Integer = myCounter
                Dim EMailID As Integer = Server.HtmlEncode(dRow("EmailID"))
                Dim Subject As String = Server.HtmlEncode(dRow("Subject"))
                Dim State As Messaging.QueueMonitoring.QueueStates = dRow("State")
                Dim Sender As String = Server.HtmlEncode(dRow("Sender"))
                Dim ToAddress As String = Server.HtmlEncode(dRow("ToAddress"))
                Dim SentTime As DateTime = Server.HtmlEncode(dRow("SentTime"))
                Dim fromAddress As String = Server.HtmlEncode(dRow("FromAddress"))
                Dim cc As String = Server.HtmlEncode(Utils.Nz(dRow("Cc"), ""))
                Dim bcc As String = Server.HtmlEncode(Utils.Nz(dRow("Bcc"), ""))
                Dim ToType As ToType = dRow("ToType")

                myCounter = Me.AddRowToAnalysisTable(AddHiddenArea, ToAddressCount, Counter, EMailID, Subject, _
                   State, Sender, ToAddress, SentTime, fromAddress, cc, bcc, ToType)
            Next
        End Sub




        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Filtered analysis as data table
        ''' </summary>
        ''' <returns>DataTable with filtered analysis data</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function FilteredAnalysisDataTable(ByVal ConnectionString As String) As DataTable
            Dim result As New DataTable
            With result.Columns
                .Add("AddHiddenArea", GetType(System.Boolean))
                .Add("ToAddressCount", GetType(System.Int32))
                .Add("EmailID", GetType(System.Int32))
                .Add("Subject", GetType(System.String))
                .Add("State", GetType(Messaging.QueueMonitoring.QueueStates))
                .Add("Sender", GetType(System.String))
                .Add("ToAddress", GetType(System.String))
                .Add("SentTime", GetType(System.DateTime))
                .Add("FromAddress", GetType(System.String))
                .Add("Cc", GetType(System.String))
                .Add("Bcc", GetType(System.String))
                .Add("ToType", GetType(MailQueueMonitor.ToType))
                .Add("DRowUserID", GetType(System.Int64))
            End With

            Dim statesToFilter As String = Me.SelectedStatesToFilter
            Dim logDT As DataTable = Me.data.LoadMailMessages(ConnectionString, Me.DataToFilter.EmailID, statesToFilter, Me.DataToFilter.FromDate, Me.DataToFilter.ToDate)
            If logDT.Rows.Count = 0 Then
                Return result
            End If

            Dim rowCounter As Integer = 1
            For Each dRow As DataRow In logDT.Rows
                Dim queueState As Messaging.QueueMonitoring.QueueStates = CType(dRow("State"), Messaging.QueueMonitoring.QueueStates)

                Dim xmlData As String = CStr(dRow("Data"))
                Dim mail As New Messaging.MailMessage(xmlData, Me.cammWebManager)
                If Me.DoesLog_eMailMessageToBeListedForCurrentUser(mail.FromAddress, mail.To, mail.Cc, mail.Bcc) Then
                    Dim addRow As Boolean = Me.IsAddRowToFilteredTable(mail)

                    If addRow Then
                        Dim addHiddenArea As Boolean = False
                        If Trim(mail.Bcc) <> "" Then
                            addHiddenArea = True
                        End If
                        If Trim(mail.Cc) <> "" Then
                            addHiddenArea = True
                        End If

                        Dim toAddressCounter As Integer = 0
                        If mail.To.IndexOf(";") > 0 Then
                            For Each toAddrress As String In mail.To.Split(";"c)
                                If Trim(toAddrress) <> "" Then
                                    toAddressCounter += 1
                                End If
                            Next
                        End If
                        If toAddressCounter > 1 Then
                            addHiddenArea = True
                        End If

                        'Dim sender As String = "<" & Trim(mail.FromName) & "> " & Trim(mail.FromAddress)
                        Dim fromNm As String = Nothing
                        If Trim(mail.FromName).Length > 0 Then
                            fromNm = """" & Trim(mail.FromName) & """"
                        End If
                        Dim sender As String = fromNm & " <" & Trim(mail.FromAddress) & ">"
                        'sender = Trim(sender)

                        Dim row As DataRow = result.NewRow
                        row("AddHiddenArea") = CBool(addHiddenArea)
                        row("ToAddressCount") = CInt(toAddressCounter)
                        row("EmailID") = CInt(dRow("ID"))
                        If mail.Subject Is Nothing Then row("Subject") = String.Empty Else row("Subject") = mail.Subject.ToString.Trim
                        row("State") = CType(queueState, Messaging.QueueMonitoring.QueueStates)
                        row("Sender") = sender.ToString.Trim
                        If mail.To Is Nothing Then row("ToAddress") = String.Empty Else row("ToAddress") = mail.To.ToString.Trim
                        row("SentTime") = CDate(dRow("DateTime"))
                        If mail.FromAddress Is Nothing Then row("FromAddress") = String.Empty Else row("FromAddress") = mail.FromAddress.ToString.Trim
                        If mail.Cc Is Nothing Then row("Cc") = String.Empty Else row("Cc") = mail.Cc.ToString.Trim
                        If mail.Bcc Is Nothing Then row("Bcc") = String.Empty Else row("Bcc") = mail.Bcc.ToString.Trim
                        row("ToType") = CType(Me.DataToFilter.ToType, MailQueueMonitor.ToType)
                        row("DRowUserID") = CLng(dRow("UserID"))

                        result.Rows.Add(row)
                        rowCounter += 1
                    End If
                End If

                If rowCounter >= 500 Then
                    Exit For
                End If
            Next

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     If current email data fits in filtering condition then returns true else false.
        ''' </summary>
        ''' <param name="mail">object of Type Mail</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function IsAddRowToFilteredTable(ByVal mail As Messaging.MailMessage) As Boolean
            Dim result As Boolean = True

            If Me.DataToFilter.Subject <> Nothing Then
                If mail.Subject.ToLower.IndexOf(Me.DataToFilter.Subject.ToLower) < 0 Then
                    result = False
                End If
            End If
            If Me.DataToFilter.FromAddress <> Nothing Then
                If mail.FromAddress.ToLower.IndexOf(Me.DataToFilter.FromAddress.ToLower) < 0 And mail.FromName.ToLower.IndexOf(Me.DataToFilter.FromAddress.ToLower) < 0 Then
                    result = False
                End If
            End If
            If Me.DataToFilter.ToAddress <> Nothing Then
                If mail.To.ToLower.IndexOf(Me.DataToFilter.ToAddress.ToLower) < 0 Then
                    result = False
                End If
            End If
            If Me.DataToFilter.ToType = ToType.Cc Then
                If Trim(mail.Cc) = "" Then
                    result = False
                ElseIf mail.Cc.ToLower.IndexOf(Me.DataToFilter.ToAddress.ToLower) < 0 Then
                    result = False
                End If
            End If
            If Me.DataToFilter.ToType = ToType.Bcc Then
                If Trim(mail.Bcc) = "" Then
                    result = False
                ElseIf mail.Bcc.ToLower.IndexOf(Me.DataToFilter.ToAddress.ToLower) < 0 Then
                    result = False
                End If
            End If

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sort string for sorting purpose
        ''' </summary>
        ''' <returns>String to sort analysis</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SortString() As String
            Dim result As String

            Select Case Me.CurrentSortID
                Case Sorting.StateASC
                    result = "State ASC"
                Case Sorting.StateDESC
                    result = "State DESC"
                Case Sorting.SenderASC
                    result = "Sender ASC"
                Case Sorting.SenderDESC
                    result = "Sender DESC"
                Case Sorting.SubjectASC
                    result = "Subject ASC"
                Case Sorting.SubjectDESC
                    result = "Subject DESC"
                Case Sorting.EmailIDASC
                    result = "EmailID ASC"
                Case Sorting.EmailIDDESC
                    result = "EmailID DESC"
                Case Sorting.SentTimeASC
                    result = "SentTime ASC"
                Case Sorting.SentTimeDESC
                    result = "SentTime DESC"
                Case Else
                    result = "State ASC"
            End Select

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Defines current sorting id
        ''' </summary>
        ''' <returns>Sorting ID</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property CurrentSortID() As MailQueueMonitor.Sorting
            Get
                Return CType(viewstate("CurrentSortID"), MailQueueMonitor.Sorting)
            End Get
            Set(ByVal Value As MailQueueMonitor.Sorting)
                viewstate("CurrentSortID") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Adds TableRow to Analysis table on web page
        ''' </summary>
        ''' <param name="addHiddenArea">add hidden area or not</param>
        ''' <param name="toAddressCount">receipient address count in To field</param>
        ''' <param name="counter">Analysis Table on webpage row counter</param>
        ''' <param name="emailID">unique email id of log_emailMessage </param>
        ''' <param name="subject">Subject of email</param>
        ''' <param name="state">state of an email</param>
        ''' <param name="sender">Sender of an email</param>
        ''' <param name="toAddress">receipient address</param>
        ''' <param name="sentTime">email sent time</param>
        ''' <param name="fromAddress"></param>
        ''' <param name="cc">receipient address as CC</param>
        ''' <param name="bcc">receipient address as Bcc</param>
        ''' <param name="toType">receipient type defined for filter purpose</param>
        ''' <returns>Return integer - next row location to be added in Analysis table at webpage</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function AddRowToAnalysisTable(ByVal addHiddenArea As Boolean, ByVal toAddressCount As Integer, ByVal counter As Integer, ByVal emailID As Integer, ByVal subject As String, ByVal state As Messaging.QueueMonitoring.QueueStates, ByVal sender As String, ByVal toAddress As String, ByVal sentTime As DateTime, ByVal fromAddress As String, ByVal cc As String, ByVal bcc As String, ByVal toType As MailQueueMonitor.ToType) As Integer
            Dim result As Integer

            Dim action As Controls.Administration.MailQueueMonitorActionControl
            action = CType(LoadControl("mailqueue_monitor_action.ascx"), Controls.Administration.MailQueueMonitorActionControl)
            action.ID = "ActionControl_" & emailID
            action.EmailID = emailID
            action.QueueState = state
            action.IsAuthorisedToSeeEmailText = Me.IsAuthorisedToSeeEmailText(fromAddress, toAddress, cc, bcc)
            action.SecurityRole = Me.SecurityRole(MyBase.cammWebManager)
            action.ResultPage = Me

            Dim myCounter As Integer = counter
            If addHiddenArea Then
                Dim myToAddress As String = toAddress
                If toAddressCount > 1 Then
                    myToAddress = Nothing
                Else
                End If
                If toType = toType.To AndAlso toAddressCount <= 1 Then
                    Me.TableAnalysis.Rows.AddAt(myCounter, New AnalysisRow(cammWebManager.ConnectionString, emailID, subject, state.ToString, sender, myToAddress, fromAddress, cc, bcc, sentTime, action, False, boolFaliureStatus))
                Else
                    If toType = toType.All Then
                        Me.TableAnalysis.Rows.AddAt(myCounter, New AnalysisRow(cammWebManager.ConnectionString, emailID, subject, state.ToString, sender, myToAddress, fromAddress, cc, bcc, sentTime, action, , boolFaliureStatus))
                    Else
                        Me.TableAnalysis.Rows.AddAt(myCounter, New AnalysisRow(cammWebManager.ConnectionString, emailID, subject, state.ToString, sender, Nothing, fromAddress, cc, bcc, sentTime, action, , boolFaliureStatus))
                    End If
                End If

                Select Case toType
                    Case toType.To
                        Me.TableAnalysis.Rows.AddAt(myCounter + 1, New HideDisplayRow(emailID, Nothing, Nothing, toAddress))
                    Case toType.Cc
                        Me.TableAnalysis.Rows.AddAt(myCounter + 1, New HideDisplayRow(emailID, cc, Nothing, Nothing))
                    Case toType.Bcc
                        Me.TableAnalysis.Rows.AddAt(myCounter + 1, New HideDisplayRow(emailID, Nothing, bcc, Nothing))
                    Case toType.All
                        Me.TableAnalysis.Rows.AddAt(myCounter + 1, New HideDisplayRow(emailID, cc, bcc, toAddress))
                End Select

                Me.TableAnalysis.Rows.AddAt(myCounter + 2, New AddLine)

                result = myCounter + 3
            Else
                If toType = toType.Cc Or toType = toType.Bcc Then
                    Me.TableAnalysis.Rows.AddAt(myCounter, New AnalysisRow(cammWebManager.ConnectionString, emailID, subject, state.ToString, sender, fromAddress, cc, bcc, Nothing, sentTime, action, False, boolFaliureStatus))
                Else
                    Me.TableAnalysis.Rows.AddAt(myCounter, New AnalysisRow(cammWebManager.ConnectionString, emailID, subject, state.ToString, sender, toAddress, fromAddress, cc, bcc, sentTime, action, False, boolFaliureStatus))
                End If
                Me.TableAnalysis.Rows.AddAt(myCounter + 1, New AddLine)

                result = myCounter + 2
            End If

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Checks for current user authorization to see email body
        ''' </summary>
        ''' <param name="fromAddress">Senders address</param>
        ''' <param name="toAddress">receipients address</param>
        ''' <param name="cc">receipients address in CC</param>
        ''' <param name="bcc">receipients address Bcc</param>
        ''' <returns>Returns True if user is authorised</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function IsAuthorisedToSeeEmailText(ByVal fromAddress As String, ByVal toAddress As String, ByVal cc As String, ByVal bcc As String) As Boolean
            Dim result As Boolean = False

            Select Case Me.SecurityRole(MyBase.cammWebManager)
                Case SecurityRoles.Supervisor
                    result = True
                Case SecurityRoles.SecurityOperator, SecurityRoles.User
                    If fromAddress <> Nothing Then
                        If fromAddress.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If toAddress <> Nothing Then
                        If toAddress.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If cc <> Nothing Then
                        If cc.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If bcc <> Nothing Then
                        If bcc.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                Case Else
                    result = False
            End Select

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     does Log_e-mail message listed for current user
        ''' </summary>
        ''' <param name="fromAddress">Senders address</param>
        ''' <param name="toAddress">receipients address</param>
        ''' <param name="cc">receipients address in CC</param>
        ''' <param name="bcc">receipients address Bcc</param>
        ''' <returns>Returns True if user is authorised</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function DoesLog_eMailMessageToBeListedForCurrentUser(ByVal fromAddress As String, ByVal toAddress As String, ByVal cc As String, ByVal bcc As String) As Boolean
            Dim result As Boolean = False
            Select Case Me.SecurityRole(MyBase.cammWebManager)
                Case SecurityRoles.Supervisor
                    result = True
                Case SecurityRoles.SecurityOperator
                    result = True
                Case SecurityRoles.User
                    If fromAddress <> Nothing Then
                        If fromAddress.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If toAddress <> Nothing Then
                        If toAddress.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If cc <> Nothing Then
                        If cc.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
                    If bcc <> Nothing Then
                        If bcc.ToLower.IndexOf(MyBase.cammWebManager.CurrentUserInfo.EMailAddress.ToLower) > -1 Then
                            result = True
                        End If
                    End If
            End Select
            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Returns all selected states from web page, to filter purpose
        ''' </summary>
        ''' <returns>all selected states for filter purpose as string</returns>
        ''' <remarks>
        '''     States as a string separated with comma used in database query e.g. state in (1, 2)
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function SelectedStatesToFilter() As String
            Dim result As String = ""

            If Me.DataToFilter.State.WaitingForReleaseBeforeQueuing Then
                result &= "0, "
            End If
            If Me.DataToFilter.State.Queued Then
                result &= "1, "
            End If
            If Me.DataToFilter.State.Sending Then
                result &= "2, "
            End If
            If Me.DataToFilter.State.Cancelled Then
                result &= "3, "
            End If
            If Me.DataToFilter.State.Successfull Then
                result &= "4, "
            End If
            If Me.DataToFilter.State.FailureAfter1Trial Then
                result &= "5, "
            End If
            If Me.DataToFilter.State.FailureAfter2Trials Then
                result &= "6, "
            End If
            If Me.DataToFilter.State.FailureAfterLastTrial Then
                result &= "7, "
            End If
            If Me.DataToFilter.State.FailureAccepted Then
                result &= "8, "
            End If

            result = Trim(result)
            If result = Nothing Then
                result = "NULL"
            ElseIf result.LastIndexOf(","c) = result.Length - 1 Then
                result = result.Substring(0, result.Length - 1)
            End If

            Return result
        End Function

        Private Function CheckFaliureStatus() As Boolean
            If Me.CheckBoxListState.Items(5).Selected OrElse Me.CheckBoxListState.Items(6).Selected OrElse Me.CheckBoxListState.Items(7).Selected OrElse Me.CheckBoxListState.Items(8).Selected Then Return True
            Return False
        End Function

#End Region

#Region " Functions "
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Default analysis to for CSV file
            ''' </summary>
            ''' <returns>Datatable with default analysis data</returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	25.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
        Private Function GetDefaultAnalysis(ByVal ConnString As String) As DataTable
            Dim result As New DataTable
            With result.Columns
                .Add("State", GetType(System.String))
                .Add("Receipient", GetType(System.String))
                .Add("Sender", GetType(System.String))
                .Add("Subject", GetType(System.String))
                .Add("ID", GetType(System.Int32))
                .Add("Sent Time", GetType(System.DateTime))
                .Add("ErrorDetails", GetType(System.String))
            End With

            Dim logDT As DataTable = Me.data.LoadMailMessages(ConnString)
            If logDT.Rows.Count = 0 Then
                Return result
            End If

            Dim rowCounter As Integer = 1
            For Each dRow As DataRow In logDT.Rows
                Dim queueState As Messaging.QueueMonitoring.QueueStates = CType(dRow("State"), Messaging.QueueMonitoring.QueueStates)

                Select Case queueState
                    Case Messaging.QueueMonitoring.QueueStates.FailureAfter1Trial, Messaging.QueueMonitoring.QueueStates.FailureAfter2Trials, Messaging.QueueMonitoring.QueueStates.FailureAfterLastTrial
                        Dim xmlData As String = CStr(dRow("Data"))
                        Dim mail As New Messaging.MailMessage(xmlData, MyBase.cammWebManager)
                        If Me.DoesLog_eMailMessageToBeListedForCurrentUser(mail.FromAddress, mail.To, mail.Cc, mail.Bcc) Then
                            Dim row As DataRow = result.NewRow
                            row("State") = queueState.ToString
                            Dim receipientText As String = ""
                            receipientText &= "To: " & mail.To & Microsoft.VisualBasic.vbCr
                            If mail.Cc <> "" Then
                                For Each str As String In mail.Cc.Split(";"c)
                                    receipientText &= "Cc: " & str & Microsoft.VisualBasic.vbCr
                                Next
                            End If
                            If mail.Bcc <> "" Then
                                For Each str As String In mail.Bcc.Split(";"c)
                                    receipientText &= "Bcc: " & str & Microsoft.VisualBasic.vbCr
                                Next
                            End If
                            If receipientText.LastIndexOf(Microsoft.VisualBasic.vbCr) = (receipientText.Length - Microsoft.VisualBasic.vbCr.Length) Then
                                receipientText = receipientText.Substring(0, receipientText.LastIndexOf(Microsoft.VisualBasic.vbCr))
                            End If
                            row("Receipient") = receipientText

                            'Dim sender As String =  Trim(mail.FromName) & " <" & Trim(mail.FromAddress) & ">" 
                            Dim fromNm As String = Nothing
                            If Trim(mail.FromName).Length > 0 Then
                                fromNm = """" & Trim(mail.FromName) & """"
                            End If
                            Dim sender As String = fromNm & " <" & Trim(mail.FromAddress) & ">"
                            row("Sender") = sender
                            row("Subject") = mail.Subject
                            row("ID") = CInt(dRow("ID"))
                            row("Sent Time") = CDate(dRow("DateTime"))
                            row("ErrorDetails") = Utils.Nz(dRow("ErrorDetails"), "")

                            result.Rows.Add(row)
                            rowCounter += 1
                        End If
                    Case Else

                End Select
                If rowCounter >= 500 Then
                    Exit For
                End If
            Next

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Filtered analysis for CSV file
        ''' </summary>
        ''' <returns>Datatable with filtered analysis data</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetFilteredAnalysis(ByVal ConnString As String) As DataTable
            Dim result As New DataTable
            With result.Columns
                .Add("State", GetType(System.String))
                .Add("Receipient", GetType(System.String))
                .Add("Sender", GetType(System.String))
                .Add("Subject", GetType(System.String))
                .Add("ID", GetType(System.Int32))
                .Add("Sent Time", GetType(System.DateTime))
                .Add("ErrorDetails", GetType(System.String))
            End With

            Dim statesToFilter As String = Me.SelectedStatesToFilter
            Dim logDT As DataTable = Me.data.LoadMailMessages(ConnString, Me.DataToFilter.EmailID, statesToFilter, Me.DataToFilter.FromDate, Me.DataToFilter.ToDate)
            If logDT.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim rowCounter As Integer = 1
            For Each dRow As DataRow In logDT.Rows
                Dim queueState As Messaging.QueueMonitoring.QueueStates = CType(dRow("State"), Messaging.QueueMonitoring.QueueStates)

                Dim xmlData As String = CStr(dRow("Data"))
                Dim mail As New Messaging.MailMessage(xmlData, MyBase.cammWebManager)
                If Me.DoesLog_eMailMessageToBeListedForCurrentUser(mail.FromAddress, mail.To, mail.Cc, mail.Bcc) Then
                    Dim addRow As Boolean = Me.IsAddRowToFilteredTable(mail)
                    If addRow Then
                        Dim row As DataRow = result.NewRow
                        row("State") = queueState.ToString
                        Dim receipientText As String = ""
                        receipientText &= "To: " & mail.To & Microsoft.VisualBasic.vbCr

                        Select Case Me.DataToFilter.ToType
                            Case ToType.To
                            Case ToType.Cc
                                If mail.Cc <> "" Then
                                    For Each str As String In mail.Cc.Split(";"c)
                                        receipientText &= "Cc: " & str & Microsoft.VisualBasic.vbCr
                                    Next
                                End If
                            Case ToType.Bcc
                                If mail.Bcc <> "" Then
                                    For Each str As String In mail.Bcc.Split(";"c)
                                        receipientText &= "Bcc: " & str & Microsoft.VisualBasic.vbCr
                                    Next
                                End If
                            Case ToType.All
                                If mail.Cc <> "" Then
                                    For Each str As String In mail.Cc.Split(";"c)
                                        receipientText &= "Cc: " & str & Microsoft.VisualBasic.vbCr
                                    Next
                                End If
                                If mail.Bcc <> "" Then
                                    For Each str As String In mail.Bcc.Split(";"c)
                                        receipientText &= "Bcc: " & str & Microsoft.VisualBasic.vbCr
                                    Next
                                End If
                        End Select
                        If receipientText.LastIndexOf(Microsoft.VisualBasic.vbCr) = (receipientText.Length - Microsoft.VisualBasic.vbCr.Length) Then
                            receipientText = receipientText.Substring(0, receipientText.LastIndexOf(Microsoft.VisualBasic.vbCr))
                        End If
                        row("Receipient") = receipientText

                        Dim sender As String = mail.FromName & " <" & mail.FromAddress & ">"
                        row("Sender") = sender
                        row("Subject") = mail.Subject
                        row("ID") = CInt(dRow("ID"))
                        row("Sent Time") = CDate(dRow("DateTime"))
                        row("ErrorDetails") = Utils.Nz(dRow("ErrorDetails"), "")

                        result.Rows.Add(row)
                        rowCounter += 1
                    End If
                End If

                If rowCounter >= 500 Then
                    Exit For
                End If
            Next

            Return result
        End Function

#End Region

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     email receipient type
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Enum ToType As Byte
            [To] = 1
            Cc = 2
            Bcc = 3
            All = 4
        End Enum
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     ID to use sort by
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Enum Sorting As Byte
            StateASC = 1
            StateDESC = 2
            SenderASC = 3
            SenderDESC = 4
            SubjectASC = 5
            SubjectDESC = 6
            EmailIDASC = 7
            EmailIDDESC = 8
            SentTimeASC = 9
            SentTimeDESC = 10
        End Enum

#Region " Properties "
        Private _DataToFilter As DataToFilterClass
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Property to hold Data to filter in session.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property DataToFilter() As DataToFilterClass
            Get
                Return _DataToFilter
            End Get
            Set(ByVal Value As DataToFilterClass)
                _DataToFilter = Value
            End Set
        End Property
#End Region

#Region " Events "
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrives data entered by user from webpage
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetDataToFilterFromWebPage() As DataToFilterClass
            Dim result As New DataToFilterClass
            If CStr(Trim(HttpContext.Current.Request.Form("TextBoxID"))) <> String.Empty Then
                result.EmailID = CInt(Trim(HttpContext.Current.Request.Form("TextBoxID")))
            End If

            result.FromAddress = CStr(Trim(HttpContext.Current.Request.Form("TextboxFromAddress")))
            result.Subject = CStr(Trim(HttpContext.Current.Request.Form("TextboxSubject")))

            'If HttpContext.Current.Request.Form("CheckBoxListState:0") = "on" Then
            '    result.State.WaitingForReleaseBeforeQueuing = True
            'Else
            '    result.State.WaitingForReleaseBeforeQueuing = False
            'End If
          

            'If HttpContext.Current.Request.Form("CheckBoxListState:1") = "on" Then
            '    result.State.Queued = True
            'Else
            '    result.State.Queued = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:2") = "on" Then
            '    result.State.Sending = True
            'Else
            '    result.State.Sending = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:3") = "on" Then
            '    result.State.Cancelled = True
            'Else
            '    result.State.Cancelled = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:4") = "on" Then
            '    result.State.Successfull = True
            'Else
            '    result.State.Successfull = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:5") = "on" Then
            '    result.State.FailureAfter1Trial = True
            'Else
            '    result.State.FailureAfter1Trial = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:6") = "on" Then
            '    result.State.FailureAfter2Trials = True
            'Else
            '    result.State.FailureAfter2Trials = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:7") = "on" Then
            '    result.State.FailureAfterLastTrial = True
            'Else
            '    result.State.FailureAfterLastTrial = False
            'End If
            'If HttpContext.Current.Request.Form("CheckBoxListState:8") = "on" Then
            '    result.State.FailureAccepted = True
            'Else
            '    result.State.FailureAccepted = False
            'End If
            If CheckBoxListState.Items(0).Selected = True Then
                result.State.WaitingForReleaseBeforeQueuing = True
            Else
                result.State.WaitingForReleaseBeforeQueuing = False
            End If

            If CheckBoxListState.Items(1).Selected = True Then
                result.State.Queued = True
            Else
                result.State.Queued = False
            End If
            If CheckBoxListState.Items(2).Selected = True Then
                result.State.Sending = True
            Else
                result.State.Sending = False
            End If
            If CheckBoxListState.Items(3).Selected = True Then
                result.State.Cancelled = True
            Else
                result.State.Cancelled = False
            End If
            If CheckBoxListState.Items(4).Selected = True Then
                result.State.Successfull = True
            Else
                result.State.Successfull = False
            End If
            If CheckBoxListState.Items(5).Selected = True Then
                result.State.FailureAfter1Trial = True
            Else
                result.State.FailureAfter1Trial = False
            End If
            If CheckBoxListState.Items(6).Selected = True Then
                result.State.FailureAfter2Trials = True
            Else
                result.State.FailureAfter2Trials = False
            End If
            If CheckBoxListState.Items(7).Selected = True Then
                result.State.FailureAfterLastTrial = True
            Else
                result.State.FailureAfterLastTrial = False
            End If
            If CheckBoxListState.Items(8).Selected = True Then
                result.State.FailureAccepted = True
            Else
                result.State.FailureAccepted = False
            End If


            result.Subject = CStr(Trim(HttpContext.Current.Request.Form("TextboxSubject")))

            result.ToAddress = CStr(Trim(HttpContext.Current.Request.Form("TextboxToAddress")))

            Select Case HttpContext.Current.Request.Form("RadioButtonListToCcBcc")
                Case "to"
                    result.ToType = ToType.To
                Case "cc"
                    result.ToType = ToType.Cc
                Case "bcc"
                    result.ToType = ToType.Bcc
                Case "all"
                    result.ToType = ToType.All
                Case Else
                    result.ToType = ToType.All
            End Select

            Dim myDate As String = String.Empty
            If CStr(Trim(HttpContext.Current.Request.Form("TextboxFrom"))) <> String.Empty Then
                myDate = CStr(Trim(HttpContext.Current.Request.Form("TextboxFrom")))
                result.FromDate = New DateTime(CInt(myDate.Substring(6, 4)), CInt(myDate.Substring(3, 2)), CInt(myDate.Substring(0, 2)))
            Else
                result.FromDate = Nothing
            End If

            myDate = String.Empty
            If CStr(Trim(HttpContext.Current.Request.Form("TextboxTo"))) <> String.Empty Then
                myDate = CStr(Trim(HttpContext.Current.Request.Form("TextboxTo")))
                result.ToDate = New DateTime(CInt(myDate.Substring(6, 4)), CInt(myDate.Substring(3, 2)), CInt(myDate.Substring(0, 2)))
            Else
                result.ToDate = Nothing
            End If

            Return result
        End Function
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Reset controls
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ButtonReset_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonReset.Click
            HttpContext.Current.Response.Redirect(Utils.ScriptNameWithoutPath)
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Export email analysis as CSV file
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ButtonExportReport_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles ButtonExportReport.Click
            Dim data As DataTable
            Me.DataToFilter = Me.GetDataToFilterFromWebPage
            If Me.DataToFilter Is Nothing Then
                data = Me.GetDefaultAnalysis(cammWebManager.ConnectionString)
            Else
                If Me.DataToFilter.EmailID = Nothing AndAlso _
                     Me.DataToFilter.FromAddress = Nothing AndAlso _
                     Me.DataToFilter.FromDate = Nothing AndAlso _
                     Me.DataToFilter.Subject = Nothing AndAlso _
                     Me.DataToFilter.ToAddress = Nothing AndAlso _
                     Me.DataToFilter.ToDate = Nothing AndAlso _
                     Me.DataToFilter.ToType = ToType.All AndAlso _
                     Me.DataToFilter.State Is Nothing Then

                    data = Me.GetDefaultAnalysis(cammWebManager.ConnectionString)
                Else
                    data = Me.GetFilteredAnalysis(cammWebManager.ConnectionString)
                End If
            End If

            If Not data Is Nothing AndAlso data.Rows.Count > 0 Then
                CompuMaster.camm.WebManager.Administration.Export.SendExportFileAsCsv(MyBase.cammWebManager, data, "UTF-8", "MailQueue")
            End If
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sort email analysis by State 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkButtonState_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkButtonState.Click
            Dim sortID As MailQueueMonitor.Sorting = Sorting.StateASC
            Select Case Me.CurrentSortID
                Case Sorting.StateASC
                    sortID = Sorting.StateDESC
                Case Sorting.StateDESC
                    sortID = Sorting.StateASC
            End Select

            Me.CurrentSortID = sortID
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sort email analysis by Sender
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkButtonSender_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkButtonSender.Click
            Dim sortID As MailQueueMonitor.Sorting = Sorting.SenderASC
            Select Case Me.CurrentSortID
                Case Sorting.SenderASC
                    sortID = Sorting.SenderDESC
                Case Sorting.SenderDESC
                    sortID = Sorting.SenderASC
            End Select

            Me.CurrentSortID = sortID
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sorts email analysis by Subject
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkButtonSubject_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkButtonSubject.Click
            Dim sortID As MailQueueMonitor.Sorting = Sorting.SubjectASC
            Select Case Me.CurrentSortID
                Case Sorting.SubjectASC
                    sortID = Sorting.SubjectDESC
                Case Sorting.SubjectDESC
                    sortID = Sorting.SubjectASC
            End Select

            Me.CurrentSortID = sortID
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sorts email analysis by email id
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkButtonID_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkButtonID.Click
            Dim sortID As MailQueueMonitor.Sorting = Sorting.EmailIDASC
            Select Case Me.CurrentSortID
                Case Sorting.EmailIDASC
                    sortID = Sorting.EmailIDDESC
                Case Sorting.EmailIDDESC
                    sortID = Sorting.EmailIDASC
            End Select

            Me.CurrentSortID = sortID
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Sorts email analysis by Sent time
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LinkButtonSentTime_Clicked(ByVal sender As Object, ByVal e As EventArgs) Handles LinkButtonSentTime.Click
            Dim sortID As MailQueueMonitor.Sorting = Sorting.SentTimeASC
            Select Case Me.CurrentSortID
                Case Sorting.SentTimeASC
                    sortID = Sorting.SentTimeDESC
                Case Sorting.SentTimeDESC
                    sortID = Sorting.SentTimeASC
            End Select

            Me.CurrentSortID = sortID
        End Sub

#End Region

#Region " Private Class DataToFilterClass "
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitor.DataToFilterClass
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     To hold data to filter
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class DataToFilterClass
            Public EmailID As Integer
            Public State As States
            Public FromAddress As String
            Public ToAddress As String
            Public ToType As MailQueueMonitor.ToType
            Public Subject As String
            Public FromDate As DateTime
            Public ToDate As DateTime

            Public Sub New()
                Me.State = New States
            End Sub
            ''' -----------------------------------------------------------------------------
            ''' Project	 : camm WebManager
            ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitor.DataToFilterClass.States
            ''' 
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Used to hold the status of State for filter purpose
            ''' </summary>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	25.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Class States
                Public WaitingForReleaseBeforeQueuing As Boolean
                Public Queued As Boolean
                Public Sending As Boolean
                Public Cancelled As Boolean
                Public Successfull As Boolean
                Public FailureAfter1Trial As Boolean
                Public FailureAfter2Trials As Boolean
                Public FailureAfterLastTrial As Boolean
                Public FailureAccepted As Boolean
            End Class


        End Class
#End Region

#Region " Private Class AnalysisRow "
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitor.AnalysisRow
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' To display filtered data in tablerow format
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	28.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class AnalysisRow
            Inherits System.Web.UI.WebControls.TableRow
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="emailID">emailID of Log_eMailMessage</param>
            ''' <param name="subject">Subject of an email</param>
            ''' <param name="state">state of an email</param>
            ''' <param name="fromAddress">sender address</param>
            ''' <param name="toAddress">receipient address</param>
            ''' <param name="sentTime">email sent time</param>
            ''' <param name="action">action Control </param>
            ''' <param name="addHideDisplayButton">add hide dislplay button or not</param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	28.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal ConnString As String, ByVal emailID As Integer, ByVal subject As String, ByVal state As String, ByVal fromAddress As String, ByVal toAddress As String, ByVal fromEmailId As String, ByVal cc As String, ByVal bcc As String, ByVal sentTime As String, Optional ByVal action As Controls.Administration.MailQueueMonitorActionControl = Nothing, Optional ByVal addHideDisplayButton As Boolean = True, Optional ByVal FaliureStatus As Boolean = True)
                Me.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top

                Me.AddHideDisplayButtonCell(emailID, addHideDisplayButton) '1
                If Not action Is Nothing Then
                    Me.AddCellEmailTextLink(action)
                Else
                    Me.AddCell("")
                End If
                Me.AddCell(state) '3
                'If toAddress = "" Then
                '    Me.AddCell("")
                'Else
                '    Me.AddCell("<b>To: </b> " & toAddress.Replace("<", "&lt;").Replace(">", "&gt;"))
                'End If
                'Me.AddCell("<b>From: </b> " & fromAddress) '5
                Me.AddSenderLink("To:", emailID, toAddress, cc, bcc)
                Me.AddSenderLink("From:", emailID, fromAddress, cc, bcc)

                Me.AddCell(subject) '6
                Me.AddCell(emailID.ToString) '7
                Me.AddCell(sentTime) '8
                If FaliureStatus Then '9
                    Dim data As New Pages.Administration.MailQueueMonitor.DataService
                    Dim Value As String = data.LoadErroDetails(ConnString, emailID.ToString)
                    If Value Is Nothing OrElse Trim(Value.ToString) = "" Then
                        Me.AddCell("&nbsp;")
                    Else
                        Me.AddCellErrorDetailsLink(emailID.ToString)
                    End If
                Else
                    Me.AddCell("&nbsp;")
                End If
            End Sub

            Private Sub AddCell(ByVal text As String)
                Dim tCell As New TableCell
                tCell.Text = text
                Me.Cells.Add(tCell)
            End Sub

            Private Sub AddCellEmailTextLink(ByVal action As Controls.Administration.MailQueueMonitorActionControl)
                Dim tCell As New TableCell
                tCell.Controls.Add(action)
                Me.Cells.Add(tCell)
            End Sub

            Private Sub AddCellErrorDetailsLink(ByVal emailID As String)
                Try
                    Dim tCell As New TableCell
                    Dim anchor As New HyperLink
                    anchor.ToolTip = "Show error details"
                    anchor.Attributes.Add("onclick", ("window.open('mailqueue_monitor_errordetails.aspx?ID=" & emailID.ToString & "', '1', 'width=550, height=350, resizable=yes scrollbars=yes'); return (false);"))

                    anchor.Attributes.Add("Href", "#")
                    anchor.Text = "Error details"
                    tCell.Controls.Add(anchor)
                    Me.Cells.Add(tCell)
                Catch ex As Exception
                End Try
            End Sub

            Private Sub AddSenderLink(ByVal strTitle As String, ByVal emailID As String, ByVal strEmailID As String, ByVal cc As String, ByVal bcc As String)
                Try
                    Dim tCell As New TableCell
                    Dim anchor As New HyperLink
                    anchor.ToolTip = "Show error details"
                    ' anchor.Attributes.Add("onclick", ("window.open('UpdateEmailDetail.aspx?ID=" + emailID.ToString + "&From=" + senderId + "&To=" + toAddress + "&CC=" + cc + "&Bcc=" + bcc + "', '1', 'width=550, height=350, resizable=yes scrollbars=yes'); return (false);"))
                    anchor.Attributes.Add("onclick", ("window.open('UpdateEmailDetail.aspx?ID=" + emailID.ToString + "', '1', 'width=350, height=250, resizable=yes scrollbars=yes'); return (false);"))
                    'Dim strLink As String = "UpdateEmailDetail.aspx?ID=" + emailID.ToString
                    anchor.Attributes.Add("Href", "#")
                    strEmailID = strEmailID.ToString.Replace("\", "")
                    anchor.Text = "<b>" & strTitle & "</b> " & strEmailID
                    tCell.Controls.Add(anchor)
                    Me.Cells.Add(tCell)
                Catch ex As Exception
                End Try
            End Sub

            Private Sub AddHideDisplayButtonCell(ByVal emailID As Integer, ByVal addHideDisplayButton As Boolean)
                Dim text As String
                text = "<input type=""button"" id=""ToogleMe" & emailID & """ value=""+"" onclick=""javascript:toggleHideDisplay('" & "" & "AreaEmailID" & emailID & "', 'ToogleMe" & emailID & "');"" />"
                Dim tCell As New TableCell
                If addHideDisplayButton Then
                    tCell.Text = text
                Else
                    tCell.Text = ""
                End If

                Me.Cells.Add(tCell)
            End Sub

        End Class
#End Region

#Region " Private Class HideDisplayRow "
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitor.HideDisplayRow
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     To display filtered data in tablerow format.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class HideDisplayRow
            Inherits System.Web.UI.WebControls.TableRow
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="emailID">email id of log_emailMessage</param>
            ''' <param name="cc">receipient address as CC</param>
            ''' <param name="bcc">receipient address as Bcc</param>
            ''' <param name="toAddress"></param>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	28.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Sub New(ByVal emailID As Integer, ByVal cc As String, ByVal bcc As String, Optional ByVal toAddress As String = Nothing)
                Me.AddCell() '1
                Me.AddCell() '2
                Me.AddCell() '3
                Me.AddCell(emailID, cc, bcc, toAddress)
                Me.AddCell() '5
                Me.AddCell() '6
                Me.AddCell() '7
                Me.AddCell() '8
            End Sub

            Private Sub AddCell()
                Dim tCell As New TableCell
                Me.Cells.Add(tCell)
            End Sub

            Private Sub AddCell(ByVal emailID As Integer, ByVal cc As String, ByVal bcc As String, Optional ByVal toAddress As String = Nothing)
                Dim tCell As New TableCell
                'tCell.ColumnSpan = 2
                Dim myPanel As New System.Web.UI.WebControls.Panel
                myPanel.ID = "AreaEmailID" & emailID
                myPanel.CssClass = "HideDisplay"
                myPanel.Controls.Add(New HideDisplay(cc, bcc, toAddress))
                tCell.Controls.Add(myPanel)
                Me.Cells.Add(tCell)
            End Sub

            Private Class HideDisplay
                Inherits System.Web.UI.WebControls.Table

                Public Sub New(ByVal cc As String, ByVal bcc As String, Optional ByVal toAddress As String = Nothing)
                    Dim tempArray As New ArrayList
                    If toAddress <> Nothing Then
                        tempArray.Clear()
                        For Each addr As String In toAddress.Split(";"c)
                            If Trim(addr) <> "" Then
                                If Not tempArray.Contains(Trim(addr)) Then
                                    tempArray.Add(Trim(addr))
                                End If
                            End If
                        Next
                        tempArray.Sort()
                        If tempArray.Count > 1 Then
                            For Each addr As String In tempArray
                                Dim tRow As New TableRow
                                Dim tCell As TableCell
                                tCell = New TableCell
                                tCell.Text = "<b>To: </b>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(HttpContext.Current.Server.HtmlEncode(addr))
                                tRow.Cells.Add(tCell)

                                Me.Rows.Add(tRow)
                            Next
                        End If
                    End If
                    If cc <> Nothing Then
                        tempArray.Clear()
                        For Each addr As String In cc.Split(";"c)
                            If Trim(addr) <> "" Then
                                If Not tempArray.Contains(Trim(addr)) Then
                                    tempArray.Add(Trim(addr))
                                End If
                            End If
                        Next
                        tempArray.Sort()
                        For Each addr As String In tempArray
                            Dim tRow As New TableRow
                            Dim tCell As TableCell
                            tCell = New TableCell
                            tCell.Text = "<b>Cc: </b>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(HttpContext.Current.Server.HtmlEncode(addr))
                            tRow.Cells.Add(tCell)

                            Me.Rows.Add(tRow)
                        Next
                    End If
                    If bcc <> Nothing Then
                        tempArray.Clear()
                        For Each addr As String In bcc.Split(";"c)
                            If Trim(addr) <> "" Then
                                If Not tempArray.Contains(Trim(addr)) Then
                                    tempArray.Add(Trim(addr))
                                End If
                            End If
                        Next
                        tempArray.Sort()
                        For Each addr As String In tempArray
                            Dim tRow As New TableRow
                            Dim tCell As TableCell
                            tCell = New TableCell
                            tCell.Text = "<b>Bcc: </b>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(HttpContext.Current.Server.HtmlEncode(addr))
                            tRow.Cells.Add(tCell)

                            Me.Rows.Add(tRow)
                        Next
                    End If
                End Sub

            End Class

        End Class
#End Region

#Region " Private Class AddLine "
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitor.AddLine
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Adds row as a line
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Class AddLine
            Inherits System.Web.UI.WebControls.TableRow

            Public Sub New()
                Me.Height = Unit.Pixel(1)
                Me.BackColor = System.Drawing.Color.Black
                Dim tCell As New TableCell
                tCell.Height = Unit.Pixel(1)
                tCell.ColumnSpan = 9
                Me.Cells.Add(tCell)
            End Sub

        End Class
#End Region

#Region " Friend Class DataService "
        ''' -----------------------------------------------------------------------------
        ''' Project	 : camm WebManager
        ''' Class	 : camm.WebManager.Pages.Administration.MailQueueMonitor.DataService
        ''' 
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Data layer class
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[patil]	25.11.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Class DataService
            Private _ConnectionString As String
            Public Property ConnectionString() As String
                Get
                    Dim result As String
                    If Me._ConnectionString = Nothing Then
#If VS2015OrHigher = True Then
#Disable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
                        Me._ConnectionString = System.Configuration.ConfigurationSettings.AppSettings("WebManager.ConnectionString")
#If VS2015OrHigher = True Then
#Enable Warning BC40000 ' Der Typ oder Member ist veraltet.
#End If
                        result = Me._ConnectionString
                    Else
                        result = Me._ConnectionString
                    End If
                    If result = String.Empty Or result = Nothing Then
                        Throw New Exception("Connection String Property is not initialized.")
                    End If
                    Return result
                End Get
                Set(ByVal Value As String)
                    _ConnectionString = Value
                End Set
            End Property
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Load the mail messages from the database
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	28.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LoadMailMessages(ByVal ConnectionString As String) As DataTable
                Dim query As String
                query = "SELECT Top 100 [ID], [UserID], [Data], [State], [DateTime], ErrorDetails FROM [Log_eMailMessages] "

                Return CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New System.Data.SqlClient.SqlCommand(query, New System.Data.SqlClient.SqlConnection(ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            End Function

            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Load the mail messages from the database
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	28.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LoadErroDetails(ByVal ConnString As String, ByVal emailid As String) As String
                Dim query As String
                query = "SELECT IsNull(ErrorDetails,'') FROM [Log_eMailMessages] Where ID = " + emailid.ToString
                Return Trim(CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteScalar(New System.Data.SqlClient.SqlCommand(query, New System.Data.SqlClient.SqlConnection(ConnString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection))
            End Function
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Load the mail messages from the database
            ''' </summary>
            ''' <param name="emailID">emailID of Log_emailMessage</param>
            ''' <param name="statesToFilter">state to filter as string</param>
            ''' <param name="fromDate">date from which emails to be filtered</param>
            ''' <param name="toDate">date to which emails to be filtered</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	28.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LoadMailMessages(ByVal ConnectionString As String, ByVal emailID As Integer, ByVal statesToFilter As String, ByVal fromDate As DateTime, ByVal toDate As DateTime) As DataTable
                Dim query As String
                query = "SELECT [ID], [UserID], [Data], [State], [DateTime], ErrorDetails FROM [Log_eMailMessages] " & vbNewLine & _
                       ""
                If emailID <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [ID] = @emailID "
                    Else
                        query &= " where [ID] = @emailID "
                    End If
                End If

                If statesToFilter = "" Then
                Else
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [State] in (" & statesToFilter & ") "
                    Else
                        query &= " where [State] in (" & statesToFilter & ") "
                    End If
                End If

                If fromDate <> Nothing AndAlso toDate = Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] > @FromDate "
                    Else
                        query &= " where [DateTime] > @FromDate "
                    End If
                ElseIf fromDate <> Nothing AndAlso toDate <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] > @FromDate AND [DateTime] <= @ToDate "
                    Else
                        query &= " where [DateTime] > @FromDate AND [DateTime] <= @ToDate "
                    End If
                ElseIf fromDate = Nothing AndAlso toDate <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] <= @ToDate "
                    Else
                        query &= " where [DateTime] <= @ToDate "
                    End If
                ElseIf fromDate = Nothing AndAlso toDate = Nothing Then
                End If

                'Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(Me.ConnectionString)) 'commented by mukesh on 28.01.2009
                Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(ConnectionString))
                If query.IndexOf("@ToDate") > -1 Then
                    MyCmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.AddDays(1)
                End If
                If query.IndexOf("@FromDate") > -1 Then
                    MyCmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate
                End If
                MyCmd.Parameters.Add("@emailID", SqlDbType.Int).Value = emailID

                Return CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                'Return CompuMaster.CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New System.Data.SqlClient.SqlCommand(query, New System.Data.SqlClient.SqlConnection(Me.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            End Function

            Public Function LoadMailMessages(ByVal ConnString As String, ByVal emailID As Integer, ByVal statesToFilter As String, ByVal fromDate As DateTime, ByVal toDate As DateTime, ByVal iTop As Integer) As DataTable
                Dim query As String
                query = "SELECT Top 100 [ID], [UserID], [Data], [State], [DateTime], ErrorDetails FROM [Log_eMailMessages] " & vbNewLine & _
                       ""
                If emailID <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [ID] = @emailID "
                    Else
                        query &= " where [ID] = @emailID "
                    End If
                End If

                If statesToFilter = "" Then
                Else
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [State] in (" & statesToFilter & ") "
                    Else
                        query &= " where [State] in (" & statesToFilter & ") "
                    End If
                End If

                If fromDate <> Nothing AndAlso toDate = Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] > @FromDate "
                    Else
                        query &= " where [DateTime] > @FromDate "
                    End If
                ElseIf fromDate <> Nothing AndAlso toDate <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] > @FromDate AND [DateTime] <= @ToDate "
                    Else
                        query &= " where [DateTime] > @FromDate AND [DateTime] <= @ToDate "
                    End If
                ElseIf fromDate = Nothing AndAlso toDate <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] <= @ToDate "
                    Else
                        query &= " where [DateTime] <= @ToDate "
                    End If
                ElseIf fromDate = Nothing AndAlso toDate = Nothing Then
                End If

                Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(ConnString))
                If query.IndexOf("@ToDate") > -1 Then
                    MyCmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.AddDays(1)
                End If
                If query.IndexOf("@FromDate") > -1 Then
                    MyCmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate
                End If
                MyCmd.Parameters.Add("@emailID", SqlDbType.Int).Value = emailID

                Return CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                'Return CompuMaster.CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New System.Data.SqlClient.SqlCommand(query, New System.Data.SqlClient.SqlConnection(Me.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

            End Function
            ''' -----------------------------------------------------------------------------
            ''' <summary>
            '''     Load the mail messages from the database
            ''' </summary>
            ''' <param name="emailID">emailID of Log_emailMessage</param>
            ''' <param name="state">state of an email</param>
            ''' <param name="fromDate">date from which email to be filtered</param>
            ''' <param name="toDate">data to which email to be filtered</param>
            ''' <returns></returns>
            ''' <remarks>
            ''' </remarks>
            ''' <history>
            ''' 	[patil]	28.11.2005	Created
            ''' </history>
            ''' -----------------------------------------------------------------------------
            Public Function LoadMailMessages(ByVal ConnString As String, ByVal emailID As Integer, ByVal state As Byte, ByVal fromDate As DateTime, ByVal toDate As DateTime) As DataTable
                Dim query As String
                query = "SELECT [ID], [UserID], [Data], [State], [DateTime], ErrorDetails FROM [Log_eMailMessages] " & vbNewLine & _
                       ""
                If emailID <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [ID] = @emailID "
                    Else
                        query &= " where [ID] = @emailID "
                    End If
                End If

                If state = 255 Then
                Else
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [State] = @State "
                    Else
                        query &= " where [State] = @State "
                    End If
                End If

                If fromDate <> Nothing AndAlso toDate = Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] > @FromDate "
                    Else
                        query &= " where [DateTime] > @FromDate "
                    End If
                ElseIf fromDate <> Nothing AndAlso toDate <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] > @FromDate AND [DateTime] <= @ToDate "
                    Else
                        query &= " where [DateTime] > @FromDate AND [DateTime] <= @ToDate "
                    End If
                ElseIf fromDate = Nothing AndAlso toDate <> Nothing Then
                    If query.IndexOf("where") > 0 Then
                        query &= " AND [DateTime] <= @ToDate "
                    Else
                        query &= " where [DateTime] <= @ToDate "
                    End If
                ElseIf fromDate = Nothing AndAlso toDate = Nothing Then
                End If

                Dim MyCmd As New SqlClient.SqlCommand(query, New SqlClient.SqlConnection(ConnString))
                If query.IndexOf("@ToDate") > -1 Then
                    MyCmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toDate.AddDays(1)
                End If
                If query.IndexOf("@FromDate") > -1 Then
                    MyCmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = fromDate
                End If
                MyCmd.Parameters.Add("@emailID", SqlDbType.Int).Value = emailID
                MyCmd.Parameters.Add("@State", SqlDbType.Int).Value = state

                Return CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")

                'Return CompuMaster.CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New System.Data.SqlClient.SqlCommand(query, New System.Data.SqlClient.SqlConnection(Me.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            End Function

        End Class
#End Region

    End Class
#End Region

#Region " Public Class DisplayText"
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : CompuMaster.camm.WebManager.Pages.Administration.DisplayText
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     This page displays text 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	21.11.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DisplayText
        Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

#Region "Vriable Declaration"
        Protected txtMsg As Label
        Protected ButtonClose As Button
        Dim Value As String
        Private data As New Pages.Administration.MailQueueMonitor.DataService
#End Region

#Region "Page Events"
        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                If Not IsPostBack Then
                    If HttpContext.Current.Request.QueryString("ID") Is Nothing Then
                        txtMsg.Text = "No error details"
                    Else
                        Dim id As String = Trim(HttpContext.Current.Request.QueryString("ID"))
                        Value = data.LoadErroDetails(cammWebManager.ConnectionString, id)
                        If Value Is Nothing OrElse Trim(Value.ToString) = "" Then
                            txtMsg.Text = "No error details"
                        Else
                            txtMsg.Text = Value.ToString
                        End If
                    End If
                End If

                Me.ButtonClose.Attributes.Add("onclick", "javascript:window.close();")
            Catch ex As Exception
            End Try
        End Sub
#End Region

    End Class
#End Region

#Region " Public Class UpdateEmailDetail "
    ''' <summary>
    '''     This page updates email details
    ''' </summary>
    Public Class UpdateEmailDetail
        Inherits CompuMaster.camm.WebManager.Pages.Administration.Page

        Protected txtFrom, txtTo, txtCc, txtBcc As TextBox
        Protected lblMsg As Label
        Dim ds As DataSet
        Protected WithEvents btnSend, btnClose As Button
        Dim mail As Messaging.MailMessage

        Private Sub PageOnLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            btnSend.Attributes.Add("onclick", "return ValidateEmailId('" + txtTo.ClientID + "','" + txtCc.ClientID + "','" + txtBcc.ClientID + "');")
            btnClose.Attributes.Add("onclick", "javascript:window.close();")
            Dim emailId As String = Request.QueryString("ID")
            Dim dtEmail As DataTable = CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.FillDataTable(New SqlCommand("SELECT [ID], [Data] FROM [Log_eMailMessages] where [ID] =" + emailId, New SqlConnection(cammWebManager.ConnectionString)), CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection, "data")
            Dim xmlData As String = CStr(dtEmail.Rows(0)("Data"))
            mail = New Messaging.MailMessage(xmlData, Me.cammWebManager)
            ds = New DataSet

            ds.ReadXml(New System.IO.StringReader(dtEmail.Rows(0)("Data").ToString))
            If Not IsPostBack Then
                txtFrom.Text = FilterEmailAddress(Trim(mail.FromAddress))
                txtTo.Text = FilterEmailAddress(Trim(mail.To))
                txtBcc.Text = FilterEmailAddress(Trim(mail.Bcc))
                txtCc.Text = FilterEmailAddress(Trim(mail.Cc))
            End If
            'cammWebManager.MessagingEMails.SendEMail(.Get("FileMonitoringSystem.RecipientEmailAddress"), "", "", .Get("FileMonitoringSystem.EmailSubject"), msgTextBody, "", .Get("FileMonitoringSystem.SenderFullName"), .Get("FileMonitoringSystem.SenderEmailAddress"))
        End Sub

        Private Function FilterEmailAddress(ByVal strEmailInfo As String) As String
            If strEmailInfo.IndexOf("<") > 0 AndAlso strEmailInfo.IndexOf(">") > 0 Then
                strEmailInfo = strEmailInfo.Substring(strEmailInfo.IndexOf("<") + 1)
                strEmailInfo = strEmailInfo.Substring(0, strEmailInfo.IndexOf(">"))
            End If

            Return strEmailInfo
        End Function

        Private Sub btnSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSend.Click
            If (sendMail()) Then
                UpdateState()
                Response.Write("<script language='javascript'>window.close();</script>")
                btnClose.Visible = True
                btnSend.Visible = False
            Else
                lblMsg.Text = "Mail-server/Queue-service is not configured."
                btnSend.Visible = True
                btnClose.Visible = False
            End If
        End Sub
        Private Sub UpdateState()
            Dim emailId As String = Request.QueryString("ID")

            If Not ds.Tables("message") Is Nothing AndAlso ds.Tables("message").Rows.Count > 0 Then
                ds.Tables("message").Select("key='FromAddress'")(0)("value") = txtFrom.Text.Trim
                ds.Tables("message").Select("key='To'")(0)("value") = txtTo.Text.Trim
                ds.Tables("message").Select("key='Cc'")(0)("value") = txtCc.Text.Trim
                ds.Tables("message").Select("key='Bcc'")(0)("value") = txtBcc.Text.Trim
                ds.AcceptChanges()

                Dim MyCmd As New SqlCommand("Update Log_eMailMessages set state=4,data=@Data where ID=@eMailID", New SqlConnection(cammWebManager.ConnectionString))
                MyCmd.Parameters.Add("@Data", SqlDbType.NText).Value = CompuMaster.camm.WebManager.Administration.Tools.Data.DataTables.ConvertDatasetToXml(ds)
                MyCmd.Parameters.Add("@eMailId", SqlDbType.Int).Value = emailId

                CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteNonQuery(MyCmd, CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
            End If
        End Sub

        Function sendMail() As Boolean
            Return cammWebManager.MessagingEMails.SendEMail(Trim(txtTo.Text), Trim(txtCc.Text), Trim(txtBcc.Text), mail.Subject, "", mail.BodyHtml, Trim(txtFrom.Text), Trim(txtFrom.Text))
        End Function
    End Class
#End Region

End Namespace