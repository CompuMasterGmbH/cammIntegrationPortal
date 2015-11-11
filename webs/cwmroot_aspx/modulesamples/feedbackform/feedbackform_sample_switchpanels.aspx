<%@ Page Language="VB" Inherits="CompuMaster.camm.WebManager.Modules.Feedback.Pages.FeedbackForm" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<script runat="server">
   
    'Inherited properties by base class:
    'protected InputFormTable as htmlcontrols.genericcontrol
    'protected Success as panel
    'protected Failure as panel
    'protected FailureException as label

    Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        If Page.IsValid Then
            Subject = Nothing
            MessageIntroText = "Anbei erhalten Sie die Angaben von einem Benutzer, welcher das Test-Formular ausgefüllt hat"
            MessageFinishText = "Das wars..."
            CollectDataAndSendEMail(cammWebManager.DevelopmentEMailAccountAddress, InputFormTable, Success, Failure)
        End If
    End Sub

</script>
<camm:WebManager id="cammWebManager" runat="server" />
<html>
<head>
</head>
<body>
    <form runat="server">
    <p runat="server" id="InputFormTable">
        <h3>
            Those controls will be colleted for the e-mail to
            <%= cammWebManager.DevelopmentEMailAccountAddress %>:</h3>
        <p>
            <a href="/modulesamples/sourcecodeviewer/srcview.aspx?path=/modulesamples/feedbackform/feedbackform_sample_switchpanels.src">
                View source code of the page</a></p>
        <table style="width: 450px; height: 289px">
            <tbody>
                <tr>
                    <td>
                        Table<asp:Label ID="Label1" runat="server">Label</asp:Label>
                    </td>
                    <td>
                        <p>
                            <asp:CheckBox ID="CheckBox1" TabIndex="1" runat="server" Text="chkbox1"></asp:CheckBox>
                        </p>
                    </td>
                    <td>
                        <asp:Calendar ID="Calendar1" TabIndex="2" runat="server"></asp:Calendar>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                    </td>
                    <td>
                        <asp:CheckBoxList ID="CheckBoxList1" TabIndex="3" runat="server">
                            <asp:ListItem Value="ttttttt" Selected="True">ttttttt</asp:ListItem>
                            <asp:ListItem Value="test" Selected="True">test</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                    <td>
                        <asp:ListBox ID="ListBox1" TabIndex="4" runat="server">
                            <asp:ListItem Value="afdsdads">afdsdads</asp:ListItem>
                            <asp:ListItem Value="adfdfsadf">adfdfsadf</asp:ListItem>
                            <asp:ListItem Value="adsfd" Selected="True">adsfd</asp:ListItem>
                        </asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <p>
                            <asp:TextBox runat="server" ID="Message" TextMode="MultiLine" />
                        </p>
                    </td>
                    <td>
                        HINT: Place validator controls as required into your form!
                    </td>
                    <td>
                        <asp:ListBox ID="ListBox2" TabIndex="5" runat="server" SelectionMode="Multiple">
                            <asp:ListItem Value="afdsdads">afdsdads</asp:ListItem>
                            <asp:ListItem Value="adfdfsadf" Selected="True">adfdfsadf</asp:ListItem>
                            <asp:ListItem Value="adsfd" Selected="True">adsfd</asp:ListItem>
                        </asp:ListBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton ID="RadioButton1" TabIndex="6" runat="server" Text="radio1"></asp:RadioButton>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="RadioButtonList1" TabIndex="7" runat="server">
                            <asp:ListItem Value="aaaaaaaa">aaaaaaaa</asp:ListItem>
                            <asp:ListItem Value="bbbbbbbbb" Selected="True">bbbbbbbbb</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td>
                        <asp:DropDownList ID="DropDownList1" TabIndex="8" runat="server">
                            <asp:ListItem Value="adfsadfd">adfsadfd</asp:ListItem>
                            <asp:ListItem Value="asfdds">asfdds</asp:ListItem>
                            <asp:ListItem Value="aaaaaaaaaaaaaa" Selected="True">aaaaaaaaaaaaaa</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </tbody>
        </table>
        <asp:Button ID="Button1" OnClick="Button1_Click" runat="server" Text="Button"></asp:Button>
    </p>
    <asp:Table ID="Table1" runat="server" Height="124px" Width="244px">
        <asp:TableRow>
            <asp:TableCell></asp:TableCell>
            <asp:TableCell>
                <asp:ListBox ID="ListBox3" TabIndex="4" runat="server">
                    <asp:ListItem Value="afdsdads">afdsdads</asp:ListItem>
                    <asp:ListItem Value="adfdfsadf">adfdfsadf</asp:ListItem>
                    <asp:ListItem Value="adsfd" Selected="True">adsfd</asp:ListItem>
                </asp:ListBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell></asp:TableCell>
            <asp:TableCell></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell></asp:TableCell>
            <asp:TableCell></asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <asp:Panel runat="server" ID="Success" Visible="false">
        All things went fine :D
    </asp:Panel>
    <asp:Panel runat="server" ID="Failure" Visible="false">
        <asp:Label runat="server" ID="FailureException" />
    </asp:Panel>
    </form>
</body>
</html>
