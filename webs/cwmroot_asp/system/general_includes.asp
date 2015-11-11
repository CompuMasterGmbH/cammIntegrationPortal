<%
Function NZ (CheckValue, IfNullValue)

	If IsNull(CheckValue) Then
		NZ = IfNullValue
	Else
		NZ = CheckValue
	End IF

End Function

Function IIf(Statement, TruePart, FalsePart)
	If Statement then
		IIf = TruePart
	Else
		IIf = FalsePart
	End If
End Function
%>