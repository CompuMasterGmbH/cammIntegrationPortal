<!--#include virtual="/system/definitions.aspx"-->
<SCRIPT LANGUAGE="VB" RUNAT="SERVER">
Sub System_CheckForAccessAuthorization(ApplicationName as string, Optional intReserved as object = Nothing, Optional strServerIP as string = Nothing)
	cammWebManager.System_CheckForAccessAuthorization(ApplicationName , intReserved , strServerIP )    
End Sub
</SCRIPT>
