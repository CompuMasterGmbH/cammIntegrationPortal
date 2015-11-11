<%
'*** Benötigt vorgeschaltete Anmeldung ***

Function System_GetCurUserID ()

	System_GetCurUserID = CInt(System_GetUserID(Session("System_Username")))

End Function

'--------------------------------------------------------------------------------------
'System_GetUserID
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
'
'MyLoginName : LoginName des Benutzers als String
'
'Return      : UserID als Long oder Null wenn nicht vorhanden
'--------------------------------------------------------------------------------------
Function System_GetUserID (MyLoginName)
Dim MyDBConn
Dim MyCmdObj
Dim MyRecSet

	on error resume next

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
    With MyCmdObj

		.CommandText = "Public_GetUserID"
		.CommandType = adCmdStoredProc

	    .Parameters.Append .CreateParameter("@Username", adVarWChar, adParamInput, 50, CStr(Trim(Mid(MyLoginName,1,20))))

    End With

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	Set MyRecSet = MyCmdObj.Execute

	If err<>0 then
		System_GetUserID = Null
	Else
		System_GetUserID = MyRecSet.Fields(0).Value
	End If

	MyRecSet.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

	on error goto 0

End Function

'--------------------------------------------------------------------------------------
'SetUserDetail
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
'
'MyUserID   : ID des Benutzers als Long
'MyProperty : Gewünschte Eigenschaft als String
'MyNewValue : Zu speichernder Wert als String/Null
'
'Return     : Bei Erfolg True, ansonsten False
'--------------------------------------------------------------------------------------
Function System_SetUserDetail (MyUserID, MyProperty, MyNewValue)
	System_SetUserDetail = System_SetUserDetailA (MyUserID, MyProperty, MyNewValue, False)
End Function

Function System_SetUserDetailA (MyUserID, MyProperty, MyNewValue, DoNotLogSuccess)
Dim MyDBConn
Dim MyCmdObj

	on error resume next

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
    With MyCmdObj

		.CommandText = "Public_SetUserDetailData"
		.CommandType = adCmdStoredProc

        .Parameters.Append .CreateParameter("@IDUser", adInteger, adParamInput, 4, MyUserID)
        .Parameters.Append .CreateParameter("@Type", adVarWChar, adParamInput, 50, CStr(Mid(Trim(MyProperty),1,50)))
        .Parameters.Append .CreateParameter("@Value", adVarWChar, adParamInput, 255, CStr(Mid(Trim(MyNewValue),1,255)))
	If DoNotLogSuccess = True Then
	        .Parameters.Append .CreateParameter("@DoNotLogSuccess", adBoolean, adParamInput, 1, True)
	End If

    End With

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	MyCmdObj.Execute

	If err<>0 then
		System_SetUserDetailA = False
	Else
		System_SetUserDetailA = True
	End If

	MyDBConn.Close

	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

	on error goto 0

End Function

'--------------------------------------------------------------------------------------
'GetUserDetail
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
'
'MyUserID   : ID des Benutzers als Long
'MyProperty : Gewünschte Eigenschaft als String
'
'Return     : Liefert das Ergebnis als String/Null zurück
'--------------------------------------------------------------------------------------
Function System_GetUserDetail (MyUserID, MyProperty)
Dim MyDBConn
Dim MyCmdObj
Dim MyRecSet

	'Create connection
	Set MyDBConn = Server.CreateObject("ADODB.Connection")
	MyDBConn.Open(User_Auth_Validation_DSN)

	'Open command object with one parameter
	Set MyCmdObj = Server.CreateObject("ADODB.Command")

	'Get parameter value and append parameter
    With MyCmdObj

		.CommandText = "Public_GetUserDetailData"
		.CommandType = adCmdStoredProc

        .Parameters.Append .CreateParameter("@IDUser", adInteger, adParamInput, 4, MyUserID)
        .Parameters.Append .CreateParameter("@Type", adVarWChar, adParamInput, 80, CStr(Mid(Trim(MyProperty),1,80)))

    End With

	'Create recordset by executing the command
	Set MyCmdObj.ActiveConnection = MyDBConn
	Set MyRecSet = MyCmdObj.Execute

	If MyRecSet.EOF then
		System_GetUserDetail = Null
	Else
		System_GetUserDetail = MyRecSet.Fields(0).Value
	End If

	MyRecSet.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

End Function

'--------------------------------------------------------------------------------------
'System_Get1stPreferredLanguageWhichIsSupportedByTheSystem
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
'
'MyUserID   : ID des Benutzers als Long
'
'Return     : Liefert die ID der ersten unterstützten vom Benutzer bevorzugten Sprache zurück
'--------------------------------------------------------------------------------------
Function System_Get1stPreferredLanguageWhichIsSupportedByTheSystem (MyUserID)
dim BufferedValue

	'1stPreferredLanguage
	BufferedValue = System_GetUserDetail(MyUserID, "1stPreferredLanguage")
	If IsNull(BufferedValue) Then BufferedValue = 0 Else BufferedValue = CLng(BufferedValue)
	If System_IsValidLanguageID(BufferedValue) = True Then
		System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = BufferedValue
	Else
		'2ndPreferredLanguage
		BufferedValue = System_GetUserDetail(MyUserID, "2ndPreferredLanguage")
		If IsNull(BufferedValue) Then BufferedValue = 0 Else BufferedValue = CLng(BufferedValue)
		If System_IsValidLanguageID(BufferedValue) = True Then
			System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = BufferedValue
		Else
			'3rdPreferredLanguage
			BufferedValue = System_GetUserDetail(MyUserID, "3rdPreferredLanguage")
			If IsNull(BufferedValue) Then BufferedValue = 0 Else BufferedValue = CLng(BufferedValue)
			If System_IsValidLanguageID(BufferedValue) = True Then
				System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = BufferedValue
			Else
				'English
				System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = 1
			End If
		End If
	End If

	If BufferedValue = 0 then
		System_Get1stPreferredLanguageWhichIsSupportedByTheSystem = 1
	end if

End Function

'--------------------------------------------------------------------------------------
'GetUserAddresses
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
'
'MyUserID   : ID des Benutzers als Long
'
'Return     : Liefert den String "Herr"/"Frau" in der entsprechenden Sprache zurück
'--------------------------------------------------------------------------------------
Function System_GetUserAddresses (MyUserID)
dim BufferedValue

	BufferedValue = System_GetUserDetail(MyUserID, "Sex")
	If LCase(BufferedValue) = "m" Then
		System_GetUserAddresses = UserManagementAddressesMr
	Else
		System_GetUserAddresses = UserManagementAddressesMs
	End If

End Function

'--------------------------------------------------------------------------------------
'System_GetUserFormOfAddress
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2001-03-23
'
'MyUserID   : ID des Benutzers als Long
'
'Return     : Liefert den String "Sehr geehrte(r) Herr/Frau" in der entsprechenden Sprache zurück
'--------------------------------------------------------------------------------------
Function System_GetUserFormOfAddress (MyUserID)
dim BufferedValue

	BufferedValue = System_GetUserDetail(MyUserID, "Sex")
	If LCase(BufferedValue) = "m" Then
		System_GetUserFormOfAddress = UserManagementEMailTextDearMr
	Else
		System_GetUserFormOfAddress = UserManagementEMailTextDearMs
	End If

End Function

'--------------------------------------------------------------------------------------
'System_GetCurUserCompleteName
'--------------------------------------------------------------------------------------
'Ersteller: Jochen Wezel, CompuMaster GmbH, 2003-01-17
'
'Return     : Liefert den Namen, z. B. "Jochen Wezel", als String in zurück
'--------------------------------------------------------------------------------------
Function System_GetCurUserCompleteName ()
Dim MyDBConn
Dim MyCmdObj
Dim MyRecSet
Dim MyResult

		'Create connection
		Set MyDBConn = Server.CreateObject("ADODB.Connection")
		MyDBConn.Open(User_Auth_Validation_DSN)

		'Open command object with one parameter
		Set MyCmdObj = Server.CreateObject("ADODB.Command")
		Set MyCmdObj.ActiveConnection = MyDBConn

		'Get parameter value and append parameter
		With MyCmdObj

			.CommandText = "Public_GetCompleteName"
			.CommandType = adCmdStoredProc

		    .Parameters.Append .CreateParameter("@Username", adVarWChar, adParamInput, 50, Session("System_Username"))

		End With

		'Create recordset by executing the command
		Set MyRecSet = MyCmdObj.Execute

		If (MyRecSet.BOF And MyRecSet.EOF) Then
			MyResult = ""
		ElseIf IsNull(MyRecSet.Fields(0)) = True Then
			MyResult = ""
		Else
			MyResult = MyRecSet.Fields(0)
		End If

	System_GetCurUserCompleteName = MyResult

	MyRecset.Close
	MyDBConn.Close

	Set MyRecSet = Nothing
	Set MyCmdObj = Nothing
	Set MyDBConn = Nothing

End Function

Function GetUserFirstName (UserId)
dim User_Auth_Validation_Cmd
dim User_Auth_Validation_RecSet

	'Create connection
	dim User_Auth_Validation_DBConn
	Set User_Auth_Validation_DBConn = Server.CreateObject("ADODB.Connection")
	User_Auth_Validation_DBConn.Open(User_Auth_Validation_DSN)

	' Open command object with one parameter.
	Set User_Auth_Validation_Cmd = Server.CreateObject("ADODB.Command")
	User_Auth_Validation_Cmd.CommandText = "SELECT * FROM Benutzer Where ID = " & CLng(UserID)
	User_Auth_Validation_Cmd.CommandType = adCmdText

	' Create recordset by executing the command.
	Set User_Auth_Validation_Cmd.ActiveConnection = User_Auth_Validation_DBConn
	Set User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.Execute

	If Not User_Auth_Validation_RecSet.eof Then
		GetUserFirstName = trim(User_Auth_Validation_RecSet.fields("Vorname").value)
	Else
		GetUserFirstName = Null
	End If

	User_Auth_Validation_RecSet.Close
	User_Auth_Validation_DBConn.Close

End Function

Function GetUserLastName (UserId)
dim User_Auth_Validation_Cmd
dim User_Auth_Validation_RecSet

	'Create connection
	dim User_Auth_Validation_DBConn
	Set User_Auth_Validation_DBConn = Server.CreateObject("ADODB.Connection")
	User_Auth_Validation_DBConn.Open(User_Auth_Validation_DSN)

	' Open command object with one parameter.
	Set User_Auth_Validation_Cmd = Server.CreateObject("ADODB.Command")
	User_Auth_Validation_Cmd.CommandText = "SELECT * FROM Benutzer Where ID = " & CLng(UserID)
	User_Auth_Validation_Cmd.CommandType = adCmdText

	' Create recordset by executing the command.
	Set User_Auth_Validation_Cmd.ActiveConnection = User_Auth_Validation_DBConn
	Set User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.Execute

	If Not User_Auth_Validation_RecSet.eof Then
		GetUserLastName = trim(User_Auth_Validation_RecSet.fields("Nachname").value)
	Else
		GetUserLastName = Null
	End If

	User_Auth_Validation_RecSet.Close
	User_Auth_Validation_DBConn.Close

End Function

Function GetUserAccessLevelID (UserId)
dim User_Auth_Validation_Cmd
dim User_Auth_Validation_RecSet

	'Create connection
	dim User_Auth_Validation_DBConn
	Set User_Auth_Validation_DBConn = Server.CreateObject("ADODB.Connection")
	User_Auth_Validation_DBConn.Open(User_Auth_Validation_DSN)

	' Open command object with one parameter.
	Set User_Auth_Validation_Cmd = Server.CreateObject("ADODB.Command")
	User_Auth_Validation_Cmd.CommandText = "SELECT * FROM Benutzer Where ID = " & CLng(UserID)
	User_Auth_Validation_Cmd.CommandType = adCmdText

	' Create recordset by executing the command.
	Set User_Auth_Validation_Cmd.ActiveConnection = User_Auth_Validation_DBConn
	Set User_Auth_Validation_RecSet = User_Auth_Validation_Cmd.Execute

	If Not User_Auth_Validation_RecSet.eof Then
		GetUserAccessLevelID = trim(User_Auth_Validation_RecSet.fields("AccountAccessability").value)
	Else
		GetUserAccessLevelID = Null
	End If

	User_Auth_Validation_RecSet.Close
	User_Auth_Validation_DBConn.Close

End Function

%>