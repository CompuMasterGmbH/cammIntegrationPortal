<%@ Application  %>
<script language=vb runat=server>

    Dim cammWebManager As New CompuMaster.camm.WebManager.WMSystem

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        Me.cammWebManager.DownloadHandler.CleanUp()
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        Me.cammWebManager.DownloadHandler.CleanUpUnregisteredFiles()
    End Sub

</script>