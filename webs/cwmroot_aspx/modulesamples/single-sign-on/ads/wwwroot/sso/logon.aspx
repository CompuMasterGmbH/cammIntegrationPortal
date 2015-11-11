<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.Login.LoginWithActiveDirectoryUser" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="CompuMaster.camm.WebManager" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<script runat="server">

        Protected Overrides ReadOnly Property ForceLogin() As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides ReadOnly Property RedirectUrlWhenLoginOkayButAlreadyLoggedOn() As String
            Get
                Return Nothing
            End Get
        End Property

</script>
<asp:Placeholder runat="server" visible="false" id="RegisterUserDocument">
<html>

<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8">
<title><asp:Literal runat="server" id="PageTitle">Automatic logon</asp:Literal></title>
<link rel="stylesheet" type="text/css" href="/sysdata/style_standard.css.css">
<style>
body {
	BACKGROUND-COLOR: #FFFFFF;
	scrollbar-3d-light-color: #EEEEEE;
	scrollbar-arrow-color: #004470;
	scrollbar-base-color: #C1C1C1;
	scrollbar-dark-shadow-color: #EEEEEE;
	scrollbar-track-color: #EEEEEE;
	scrollbar-face-color: #EEEEEE;
	scrollbar-highlight-color: #EEEEEE;
	scrollbar-shadow-color: #EEEEEE;
	margin-top: 5px;
	margin-right: 5px;
	margin-bottom: 5px;
	margin-left: 15px;
	font-family: Arial, Helvetica, sans-serif;
	font-size: 12px;
	color: #000000;
}
h1 { font-size:13px; margin-top:0px; margin-bottom:0px; font-weight: bold; color:#004470; }
h2 { font-size:12px; font-weight: bold; color:#447799; }
td {
	font-family: Arial, Helvetica, sans-serif;
	font-size: 12px;
	color: #000000;
}
</style>
</head>

<body>
<form runat="server" name="SSOAction">
<table border="0" cellpadding="0" style="border-collapse: collapse" width="100%" id="table1" height="100%">
  <tr>
    <td align="center">
    <table border="1" cellpadding="5" cellspacing="0" width="640" id="table2" height="480" bordercolor="#CCCCCC">
      <tr>
        <td valign="top">
        <table border="0" cellpadding="0" style="border-collapse: collapse" width="100%" id="table3">
          <tr>
            <td>
            <table id="table4" cellSpacing="0" cellPadding="0" width="100%" border="0">
              <tr>
                <td>
                <h1><asp:Label runat="server" id="FormTitle">Setup of your automatic logon</asp:Label><br>
                <img height="1" hspace="0" src="images/h1_line.gif" width="343" border="0"></h1>
                </td>
                <td vAlign="top" align="right" rowspan="4"><img border="0" src="images/handshake_150x87.jpg"></td>
              </tr>
              <tr>
                <td valign="top"><h2><asp:Label runat="server" id="FormSubTitle">Server group name</asp:Label></h2></td>
              </tr>
            </table>
            </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
          </tr>
          <tr>
            <td height="100%">
				<p><asp:Label runat="server" ID="IdentifiedUserName" /></p>
				<p><asp:Label runat="server" ID="LabelTakeAnAction" />
				<ul>
					<asp:radiobutton Runat="server" GroupName="RadioAction" id="RadioRegisterExisting" Text="Register for an <strong>existing</strong> account" /><br>
						<asp:Panel Runat="server" ID="PanelRegisterExisting">
						<ul>
							<table border="0"><tr><td><asp:Label runat="server" ID="LabelRegisterExistingLoginName" /></td><td><asp:TextBox Runat="server" onChange="document.forms[0].RadioRegisterExisting.checked = true;" ID="LoginNameRegisterExisting" /></td></tr>
							<tr><td><asp:Label runat="server" ID="LabelRegisterExistingPassword" /></td><td><asp:TextBox Runat="server" onChange="document.forms[0].RadioRegisterExisting.checked = true;" ID="LoginPasswordRegisterExisting" TextMode="Password" /></td></tr></table>
						</ul>
						</asp:Panel>
					<asp:radiobutton Runat="server" GroupName="RadioAction" id="RadioRegisterNew" Text="Register for a <strong>new</strong> account" /><br>
						<asp:Panel Runat="server" ID="PanelRegisterNew">
						<ul>
							<table border="0"><tr><td><asp:Label runat="server" ID="LabelRegisterNewLoginName" /></td><td><asp:TextBox Runat="server" onChange="document.forms[0].RadioRegisterNew.checked = true;" ID="LoginNameRegisterNew" /></td></tr>
							<tr><td><asp:Label runat="server" ID="LabelRegisterNewPassword" /></td><td><asp:TextBox Runat="server" onChange="document.forms[0].RadioRegisterNew.checked = true;" ID="LoginPasswordRegisterNew" TextMode="Password" /></td></tr>
							<tr><td><asp:Label runat="server" ID="LabelRegisterNewPassword2" /></td><td><asp:TextBox Runat="server" onChange="document.forms[0].RadioRegisterNew.checked = true;" ID="LoginPassword2RegisterNew" TextMode="Password" /></td></tr>
							<tr><td><asp:Label runat="server" ID="LabelRegisterNewEMail" /></td><td><asp:TextBox Runat="server" onChange="document.forms[0].RadioRegisterNew.checked = true;" ID="EMailAddressRegisterNew" /></td></tr></table>
						</ul>
						</asp:Panel>
					<asp:radiobutton Runat="server" GroupName="RadioAction" id="RadioDoNothing" Text="If the identification is wrong or you want to proceed without login, now (ask later again)" /><br>
				</ul>
				<font color="#FF0000"><asp:Label Runat="server" ID="Messages" /></font>
				</p>
				<p><asp:Button runat="server" text="Continue" id="ButtonNext" /><br>&nbsp;</p>
			</td>
          </tr>
          <tr>
            <td height="100%"><asp:Label Runat="server" ID="ContactUs" /></td>
          </tr>
        </table>
        </td>
      </tr>
    </table>
    </td>
  </tr>
</table>
</form>
</body>

</html>
</asp:Placeholder>