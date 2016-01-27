<%@ Page language="VB" %>
<%@ Register TagPrefix="camm" TagName="WebManager" Src="~/system/cammWebManager.ascx" %>
<camm:WebManager id="cammWebManager" runat="server" />
<!--#include virtual="/sysdata/includes/standardtemplate_top.aspx"-->
<script runat="server">

Dim MyLangID as integer

sub Page_Load (sender as object, e as eventargs)

	MyLangID = cammWebManager.UIMarket
	Select Case MyLangID
		Case 3,2,1: 'DoNothing - Supported languages
		Case Else
			MyLangID = cammWebManager.Internationalization.GetAlternativelySupportedLanguageID (MyLangID)
	End Select

	Select Case MyLangID
		case 2: 'DEU
			cammWebManager.PageTitle = "Hilfe"
		Case Else: 'ENG
			cammWebManager.PageTitle = "Help"
	End Select

end sub

</script>
<%
	Select Case MyLangID
		case 2: 'DEU
%>
				      <div align="center"><center>
				      <table border="0" width="600" cellspacing="0" cellpadding="0"><TBODY>
				        <tr>
				          <td width="50%" valign="top"><b><font face="Times New Roman" size="5" color="#00426b">Hilfe</font></b></td>
				        </tr>
				        <tr>
				          <td width="100%" valign="top">
				            <P><FONT face=Arial size=2>Sollten Sie allgemeine oder spezielle Fragen zu dieser WebSite haben,
				            kontaktieren Sie bitte unser Serviceteam <ul><li>&uuml;ber <A href="mailto:<%= cammWebManager.StandardEMailAccountAddress %>"><%= cammWebManager.StandardEMailAccountAddress %></A>.</li>
				            </ul>
				            </FONT></P></td>
				        </tr>
				        </TBODY></table>
				        </center></div>
<%
		Case Else: 'ENG
%>
				      <div align="center"><center>
				      <table border="0" width="600" cellspacing="0" cellpadding="0"><TBODY>
				        <tr>
				          <td width="50%" valign="top"><b><font face="Times New Roman" size="5" color="#00426b">Help</font></b>
				        </tr>
				        <tr>
				          <td width="100%" valign="top">
				            <P><FONT face=Arial size=2></P>
				            <P>Should you have general or special questions
				            to this website, please contact our service team
				            <ul><li>via <A href="mailto:<%= cammWebManager.StandardEMailAccountAddress %>"><%= cammWebManager.StandardEMailAccountAddress %></A>.</li>
				            </ul>
				            </FONT></P></td>
				        </tr>
				        </TBODY></table>
				        </center></div>
<%
	End Select
%>
<!--#include virtual="/sysdata/includes/standardtemplate_bottom.aspx"-->