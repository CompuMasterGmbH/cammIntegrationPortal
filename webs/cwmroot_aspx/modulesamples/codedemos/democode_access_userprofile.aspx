<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.UserAccount.ChangeUserProfile" %>
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" SecurityObject="@@Public" runat="server" />
<script runat="server">

	Sub Page_Load()
		Me.Username.Text = Me.cammWebManager.CurrentUserLoginname
		Me.UserFullname.Text = Me.cammWebManager.CurrentUserInfo.FullName & " / " & me.cammWebManager.CurrentUserInfo.SalutationUnformal
		Me.AuthorizedForAdmin.Text = Me.cammWebManager.IsUserAuthorized("System - User Administration - Users")
		Me.LastAccess.Text = Me.cammWebManager.CurrentUserInfo.AdditionalFlags("DemoTest_LastAccessDate")
		Me.CurrentDateTime.Text = Now

		'Change user profile
		Me.cammWebManager.CurrentUserInfo.AdditionalFlags("DemoTest_LastAccessDate") = Now
		Me.cammWebManager.CurrentUserInfo.Save
	End Sub

</script>
<p>Current user logon name: <asp:Label runat="server" id="username" /></p>
<p>Full name / informal salutation in market/language &quot;<%= Me.cammWebManager.UIMarketInfo.LanguageName_English %>&quot;: <asp:Label runat="server" id="userFullName" /></p>
<p>Has he been authorized for application "System - User Administration - Users": <asp:Label runat="server" id="AuthorizedForAdmin" /></p>
<p>Last previous access to this demo page: <asp:Label runat="server" id="LastAccess" /></p>
<p>Current server date/time: <asp:Label runat="server" id="CurrentDatetime" /></p>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->