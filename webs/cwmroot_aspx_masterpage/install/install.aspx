<%@ Page Language="VB" debug="true" %>
<%@ import Namespace="System.Data" %>
<%@ import Namespace="System.Data.SqlClient" %>
<%@ import Namespace="System.Data.OleDb" %>
<script runat="server">

    'Last update on 2007-09-07

    Public Const SetupPackageName As String = "WebManager"
    Public Const SetupPackageTitle As String = "Web-Manager"
    
    Dim WithEvents DBSetup As New CompuMaster.camm.WebManager.Setup.DatabaseSetup(SetupPackageName, SetupPackageTitle)

    Private Sub PageOnInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Init
        Server.ScriptTimeout = 900 '15 minutes
        DBSetup.UpdatesOnly = False
    End Sub

    Private Sub PageOnPreRender(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
        LabelLogItems.Text = DBSetup.GetLogData.Replace(vbNewLine, "<br>" & vbNewLine)
    End Sub

    Sub Page_OnInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            Me.TextBoxAuthPassword.Text = ""
            Me.TextBoxAuthUser.Text = "sa"
            Me.TextBoxCompanyFormerName.Text = "YourCompany Ltd."
            Me.TextBoxCompanyName.Text = "YourCompany"
            Me.TextBoxCompanyURL.Text = "http://www.yourcompany.com/"
            Me.TextBoxDBCatalog.Text = "camm WebManager"
            Me.TextBoxDBServer.Text = "localhost"
            Me.TextBoxPort.Text = ""
            Me.TextBoxProtocol.Text = "http"
            Me.TextBoxServerIP.Text = "localhost"
            Me.TextBoxServerName.Text = "MyServer"
            Me.TextBoxSGroupContact.Text = "onlineservice@yourcompany.com"
            Me.TextBoxSGroupNavTitle.Text = ""
            Me.TextBoxSGroupTitle.Text = ""
        End If
    End Sub
    
    Public Function GetConnectionString_ServerAdministration()
        Return ("SERVER=" & Me.TextBoxDBServer.Text & ";PWD=" & Me.TextBoxAuthPassword.Text & ";UID=" & Me.TextBoxAuthUser.Text)
    End Function
    
    Public Function GetConnectionString()
        Return ("SERVER=" & Me.TextBoxDBServer.Text & ";PWD=" & Me.TextBoxAuthPassword.Text & ";UID=" & Me.TextBoxAuthUser.Text & ";DATABASE=" & Me.TextBoxDBCatalog.Text)
    End Function
    
    Sub btnInstallDB_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DropExistingSqlDatabase As Boolean
        If CheckBoxDropExistingSqlDatabase.Checked = True Then
            DropExistingSqlDatabase = True
        Else
            DropExistingSqlDatabase = False
        End If

        Try
            If CheckBoxNewDB.Checked = True Then
                If DBSetup.CreateDatabase(GetConnectionString(), GetConnectionString_ServerAdministration(), TextBoxDBCatalog.Text, Not DropExistingSqlDatabase) Then
                    DBSetup.WriteToLog("Database created successfully!<br>")
                Else
                    Throw New Exception("Error: Database couldn't be created!<br>")
                End If
            Else
                If DBSetup.InitDatabase(GetConnectionString()) Then
                    DBSetup.WriteToLog("Database (re-)initialized successfully!<br>")
                Else
                    Throw New Exception("Error: Database couldn't be initialized!<br>")
                End If
            End If

            Dim Replacements As New Specialized.NameValueCollection
            Replacements = DBSetup.GetWebManagerReplacements(TextBoxServerIP.Text, TextBoxProtocol.Text, TextBoxServerName.Text, TextBoxPort.Text, TextBoxSGroupTitle.Text, TextBoxSGroupNavTitle.Text, TextBoxSGroupContact.Text, TextBoxCompanyName.Text, TextBoxCompanyFormerName.Text, TextBoxCompanyURL.Text)
            DBSetup.DoUpdates(GetConnectionString(), Replacements)
        Catch ex As Exception
            DBSetup.WriteErrorMessage(ex.ToString.Replace(vbNewLine, "<br>"))
        End Try
        
    End Sub

    Public Sub DisplayWarning() Handles DBSetup.WarningsQueueChanged
        DBSetup.WriteToLog("<span class=""red""><b>Install Warning:</b> " & DBSetup.Warnings.Replace(vbNewLine, "<br>") & "</span>")
        DBSetup.Warnings = Nothing
    End Sub

    Public Sub DisplayNewStatus_ProgressTask() Handles DBSetup.ProgressTaskStatusChanged
        Dim Msg As String = "Progress Task: " & DBSetup.ProgressOfTasks.CurrentStepTitle
        Msg += " (Step " & DBSetup.ProgressOfTasks.CurrentStepNumber.ToString
        Msg += " of " & DBSetup.ProgressOfTasks.StepsTotal.ToString & ")"
        DBSetup.WriteToLog(Msg & vbNewLine)
    End Sub

    Public Sub DisplayNewStatus_Step() Handles DBSetup.StepStatusChanged
        DBSetup.WriteToLog("Step: " & DBSetup.CurrentStepTitle & vbNewLine)
    End Sub

    Public Sub DisplayNewStatus_ProgressStep() Handles DBSetup.ProgressStepStatusChanged
        DBSetup.WriteToLog("Progress Step: " & DBSetup.ProgressOfSteps.CurrentStepTitle & " (Step " & DBSetup.ProgressOfSteps.CurrentStepNumber & " of " & DBSetup.ProgressOfSteps.StepsTotal & ")")
    End Sub

</script>
<html>
<head><title>camm Web-Manager Online Database Installer</title>
<style type="text/css"><!--
Body
{
    color: black; 
    font-family: Arial, Helvetica, sans-serif;
    font-size: 12px;
    background-color: #dedede;
}

H1
{
    color: black; 
    font-family: Arial, Helvetica, sans-serif;
    font-size: 20px;
}

Input
{
    border: 1px solid black;
}

.TextInput
{
    border: 1px solid black;
    width: 150px;
}

.Tdleft
{
    color: black; 
    font-family: Arial, Helvetica, sans-serif;
    font-size: 12px;
    width: 300px;
}

.InfoText
{
    font-size: 11px;
}
-->
</style>
</head>
<body>
    <form runat="server" action="">
            <table>
                    <tr>
                        <td>
                            <h1>camm Web-Manager Online Database Installer</h1>
                            <p class="InfoText"><span style="color: Red;"><b>Note! </b></span> If you've got TCP access to the database server from your workstation, you can use the camm Web-Manager Windows-Installation-Wizard <i>(provided with the camm Web-Manager CD)</i> as well for a more convenient installation process.</p>
                            <p class="InfoText"><span style="color: Red;"><b>Important note: </b></span> By installing this software, you agree to be bound to the following <a href="licence.rtf">licence terms</a>.</p>
                            </td>
                    </tr>
            </table>
            <hr />
            <table>
                    <tr>
                        <td class="TdLeft">
                            Create new SQL database on database server</td>
                        <td>
                            <asp:CheckBox id="CheckBoxNewDB" runat="server" Checked="true"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Drop an existing SQL database, first</td>
                        <td>
                            <asp:CheckBox id="CheckBoxDropExistingSqlDatabase" runat="server" Checked="true"></asp:CheckBox>
                        </td>
                    </tr>
           </table>
            <hr />
            <table>                    
                    <tr>
                        <td class="TdLeft">
                            Database name</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxDBCatalog" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Server</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxDBServer" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            User</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxAuthUser" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Password</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxAuthPassword" runat="server" TextMode="Password"></asp:TextBox>
                        </td>
                    </tr>
           </table>
            <hr />
            <table>
                    <tr>
                        <td class="TdLeft">
                            WebServer Server Name</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxServerName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Protocol</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxProtocol" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Port</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxPort" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Server IP / Host Header
                            Name</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxServerIP" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            First Server Group Title
                            (Administration Area)</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxSGroupTitle" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            First Server Group Title
                            (Navigation)</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxSGroupNavTitle" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            General Website URL</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxCompanyURL" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Standard Contact Person
                            (eMail)</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxSGroupContact" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Company Name</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxCompanyName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="TdLeft">
                            Extended Company Name</td>
                        <td>
                            <asp:TextBox cssclass="TextInput" id="TextBoxCompanyFormerName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
           </table>
            <hr />
            <table>
                    <tr>
                        <td class="TdLeft">
                        </td>
                        <td>
                            <asp:Button Width="150px" id="btnInstallDB" onclick="btnInstallDB_Click" runat="server" Text="(Re-)Install DB"></asp:Button>
                        </td>
                    </tr>
            </table>

            <table>
                    <tr>
                        <td class="TdLeft"><asp:Label runat="server" ID="LabelLogItems" />
                        </td>
                    </tr>
            </table>
            
    </form>
</body>
</html>
