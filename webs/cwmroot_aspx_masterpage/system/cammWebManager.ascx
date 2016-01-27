<%@ Control debug="false" CodeFile="/portal/config.vb" Language="vb" AutoEventWireup="false" Inherits="Instance" %>
<%@ Assembly Src="/portal/custom_internationalization.vb" %>
<%@ Register TagPrefix="WebManager" Namespace="CompuMaster.camm" Assembly="cammWM" %>
<script lang="vb" runat="server"> 

    Protected Overrides Sub LoadConfiguration()
        Me.Internationalization = New CustomSettingsAndData
        SetupAdditionalConfiguration()
    End Sub

</script>
