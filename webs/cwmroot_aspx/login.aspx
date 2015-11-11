<%@ Page Inherits="CompuMaster.camm.WebManager.Pages.Page" language="VB" %>
<%@ Import Namespace="Microsoft.VisualBasic" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<%
dim returnUrl as string = Request.QueryString("ReturnUrl")
if mid(returnUrl, 1, 1) = "/" Then
    returnUrl = cammWebManager.Internationalization.User_Auth_Config_CurServerURL & ReturnUrl
End If
cammWebManager.RedirectToLogonPage (returnUrl, "WebForms Authentification requested", Nothing)
%>