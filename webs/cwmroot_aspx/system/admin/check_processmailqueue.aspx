<%@ Page Language="VB" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Pages.ProtectedPage" %>

<!DOCTYPE html>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" SecurityObject="System - User Administration - ServerSetup" />
<script runat="server">
    Public Sub MailQueueProcess1Item()
        cammWebManager.MessagingQueueMonitoring.ProcessOneMail()
    End Sub
    Public Sub MailQueueProcess10Item()
        For MyCounter As Integer = 1 To 10
            cammWebManager.MessagingQueueMonitoring.ProcessOneMail()
        Next
    End Sub
    Public Sub MailQueueProcess100Item()
        For MyCounter As Integer = 1 To 100
            cammWebManager.MessagingQueueMonitoring.ProcessOneMail()
        Next
    End Sub
    Public Sub AutoSendLoad(sender As Object, e As EventArgs) Handles Me.Load
        If Request.QueryString("auto") = "1" Then
            Me.CheckboxAutosend.Checked = True '"Restore" viewstate
        End If
    End Sub
    Public Sub AutoSendPreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Me.CheckboxAutosend.Checked Then MailQueueProcess1Item()
        Me.refresh.Content = "3; url=" & Request.Path & "?auto=1"
        If Me.CheckboxAutosend.Checked Then
            Me.refresh.Visible = True
        Else
            Me.refresh.Visible = False
        End If
    End Sub
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta runat="server" ID="refresh" http-equiv="refresh" visible="false" content="" />
    <title>E-Mail queue - Manual triggering</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>E-Mail queue on <%= Now %> - Manual triggering</h1>
            <p>Current no. of items in e-mail queue: <%= cammWebManager.MessagingQueueMonitoring.MailsInQueue() %></p>
            <h2>Process e-mail queue</h2>
            <p>
                <asp:Button runat="server" ID="ProcessQueue1Item" Text="Process next 1 e-mail from queue" OnClick="MailQueueProcess1Item" />
                <asp:Button runat="server" ID="ProcessQueue10Item" Text="Process next 10 e-mails from queue" OnClick="MailQueueProcess10Item" />
                <asp:Button runat="server" ID="ProcessQueue100Item" Text="Process next 100 e-mails from queue" OnClick="MailQueueProcess100Item" />
                <br />
                <asp:Checkbox runat="server" ID="CheckboxAutosend" Text="Automatically process next 1 e-mail from queue (at interval 3 seconds)" AutoPostBack="true" EnableViewState="true" />
            </p>
        </div>
    </form>
</body>
</html>
