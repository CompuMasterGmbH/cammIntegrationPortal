<script runat="server">
    Sub BindData(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not Page.IsPostBack Then
            Dim ActiveMarkets As CompuMaster.camm.WebManager.WMSystem.LanguageInformation()
            ActiveMarkets = cammWebManager.System_GetLanguagesInfo(New Integer() {}, False, True)
            Dim SList As New System.Collections.SortedList(ActiveMarkets.Length)
            For MyCounter As Integer = 0 To ActiveMarkets.Length - 1
                SList.Add(ActiveMarkets(MyCounter).LanguageName_English, ActiveMarkets(MyCounter).ID)
            Next
            DropDownListAvailableMarkets.DataSource = SList

            'Load/Bind data
            DropDownListAvailableMarkets.DataBind()
        
            'Select current control values
            For MyCounter As Integer = 0 To DropDownListAvailableMarkets.Items.Count - 1
                If DropDownListAvailableMarkets.Items(MyCounter).Value = cammWebManager.UI.MarketID.ToString Then
                    DropDownListAvailableMarkets.SelectedIndex = MyCounter
                End If
            Next
        End If
    End Sub

    Sub DropDownListAvailableMarkets_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Response.Redirect(cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL & "?Lang=" & Me.DropDownListAvailableMarkets.SelectedValue)
    End Sub
    
</script>
<table border="0" width="100%" background="/sysdata/images/banner/bg_stripe_extranet.gif" cellspacing="0" cellpadding="0" height="55">
  <tr>
    <td colspan="4" height="38" valign="top"><a href="<%= cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyWebSiteURL") %>" target="_blank"><img alt="<%= cammWebManager.System_GetServerConfig(cammWebManager.CurrentServerIdentString, "AreaCompanyWebSiteTitle") %>" src="/sysdata/images/banner/logo_extranet.gif" border="0" width="175" height="38"></a></td>
    <form id="Form1" runat="server"><td colspan="3" height="38" valign="top" align="right">
    
    <asp:DropDownList ID="DropDownListAvailableMarkets" runat="server" OnSelectedIndexChanged="DropDownListAvailableMarkets_SelectedIndexChanged" DataValueField="Value" DataTextField="Key" AutoPostBack=true>
    </asp:DropDownList>
    
      <p align="right"><MAP name=EarthTabsMap>
      <area alt="English language" shape="rect" target="_top" coords="61, 22, 95, 38" href="<%= cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL %>?Lang=1">
      <area alt="Deutsche Sprache" shape="rect" target="_top" coords="20, 22, 56, 37" href="<%= cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL %>?Lang=2">
      <area alt="French language" shape="rect" target="_top" coords="100, 22, 131, 37" href="<%= cammWebManager.Internationalization.User_Auth_Validation_NoRefererURL %>?Lang=3"></MAP>
      <img useMap="#EarthTabsMap" border="0" src="/sysdata/images/banner/flags_on_earth.gif" width="134" height="38"></p></td></form>
  </tr>
  <tr>
    <td bgcolor="#FFFF66" valign="middle" height="17"><font size="2" face="Arial"><b><nobr>&nbsp;&nbsp;
      <%= cammWebManager.Internationalization.GetCurLongDate(cammWebManager.UIMarket) %></nobr></b></font></td>
    <td bgcolor="#FFFF66" valign="middle" height="17">&nbsp;</td>
    <td bgcolor="#FFFF66" colspan="2" valign="middle" height="17"><a href="<%= Response.ApplyAppPathModifier("/sysdata/help.aspx") %>"><font size="2" face="Arial"><b><%= cammWebManager.Internationalization.Banner_Help %></b></font></a></td>
    <td bgcolor="#FFFF66" valign="middle" height="17"><a href="<%= Response.ApplyAppPathModifier("/sysdata/feedback.aspx") %>"><font size="2" face="Arial"><b><%= cammWebManager.Internationalization.Banner_Feedback %></b></font></a></td>
    <td bgcolor="#FFFF66" valign="middle" height="17"></td>
    <td bgcolor="#FFFF66" align="right" valign="middle" height="17"><a<% If Session("System_Username") <> "" Then Response.Write (" target=""_top""") %> href="<% If Session("System_Username") <> "" Then Response.Write (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL & "?Action=Logout") Else Response.Write (cammWebManager.Internationalization.User_Auth_Validation_LogonScriptURL) %>"><font size="2" face="Arial"><b><% If Session("System_Username") = "" Then Response.Write (cammWebManager.Internationalization.NavLinkNameLogin) Else Response.Write (cammWebManager.Internationalization.NavLinkNameLogout) %></b></font></a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
  </tr>
</table>