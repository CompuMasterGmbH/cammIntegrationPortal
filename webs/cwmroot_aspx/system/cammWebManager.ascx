<%@ Control debug="false" Language="vb" AutoEventWireup="false" Inherits="CompuMaster.camm.WebManager.Controls.cammWebManager" %>
<%@ Register TagPrefix="WebManager" Namespace="CompuMaster.camm" Assembly="cammWM" %>
<!--#include virtual="/sysdata/config.vb"-->
<!--#include virtual="/sysdata/custom_internationalization.vb"-->
<script language="vb" runat="server"> 

        Protected Overrides Sub LoadConfiguration()
            SetupAdditionalConfiguration()
        End Sub
        Public Overrides Sub LoadLanguageData(LanguageID As Integer)
            LoadCustomizedLanguageStrings(LanguageID)
        End Sub

</script>
