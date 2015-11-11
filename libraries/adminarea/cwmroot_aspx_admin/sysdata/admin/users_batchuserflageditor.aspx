<!--#include virtual="/system/admin/users_batchuserflageditor.aspx"-->
<script runat="server">
    Public Overrides Function IsProtectedFlag(ByVal flagName As String) As Boolean
        Dim result As Boolean = false
        Dim protectedFlagNames As String() = New String() {"ebbes"}
        For myCounter As Integer = 0 To protectedFlagNames.Length - 1
            If flagName.ToLower.StartsWith(protectedFlagNames(myCounter).ToLower) Then
                result = true
            End If
        Next
        Return result
    End Function
</script>