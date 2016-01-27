''' <summary>
'''     The default notification e-mails of camm Web-Manager
''' </summary>
''' <remarks>
'''     Use this class to modify/customize the e-mails to all your users
''' </remarks>
''' <history>
''' 	[adminwezel]	06.07.2004	Created
''' </history>
Public Class CustomNotifications
    Inherits CompuMaster.camm.WebManager.Notifications.DefaultNotifications

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Creates a new WMNotifications class
    ''' </summary>
    ''' <param name="webManager">The camm Web-Manager instance this class shall work with</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub New(ByVal webManager As CompuMaster.camm.WebManager.WMSystem)
        MyBase.New(webManager)
    End Sub

    Protected Overrides Function UserSalutation(ByVal userInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation) As String
        Return userInfo.SalutationInMails
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The HTML tag with correct left-to-right or right-to-left direction
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' Arabic, hebrew and some other languages require a right-to-left writing instead of the default left-to-right. This method creates an HTML tag based on the current UI culture.
    ''' </remarks>
    ''' <history>
    ''' 	[wezel]	12.12.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Function HtmlTagOpener() As String
        If CompuMaster.camm.WebManager.Utils.IsRightToLeftCulture(System.Globalization.CultureInfo.CurrentUICulture) Then
            Return "<html dir=""rtl"">"
        Else
            Return "<html dir=""ltr"">"
        End If
    End Function

    '/// will be called by System_SetUserInfo method
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Welcome notification e-mail for the self registered user with mention of the password 
    ''' </summary>
    ''' <param name="UserInfo">The user information object</param>
    ''' <param name="Password">The new password</param>
    ''' <remarks>
    '''     This method will be called by the standard mechanisms of camm Web-Manager
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overloads Overrides Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation, ByVal Password As String)

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Which language should be used for creation of mail which should be in the preferred language of the user
        Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(UserInfo)
        cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

        Dim MainSubject As String, eMailBody As String, eMailHTMLBody As String
        MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
        eMailBody = UserSalutation(UserInfo) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome_WithPassword, UserInfo.LoginName, Password, GetUserLogonServers(cammWebManager, UserInfo.ID)) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            cammWebManager.StandardEMailAccountName & ControlChars.CrLf
        eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(UserInfo)) & "</p>" & _
            "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome_WithPassword, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(UserInfo.LoginName) & "</strong></font>", "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(Password) & "</strong></font>", CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, UserInfo.ID)))) & "</p>" & _
            "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            "</body></html>"

        If cammWebManager.MessagingEMails.SendEMail(UserInfo.FullName, UserInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal) = False Then
            If MailLanguage <> BackupOfCurrentLanguageID Then
                'Restore the current language profile
                'Language data has been switched; switch back now
                cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)
            End If
            Throw New Exception("Error sending mail to new user")
        End If

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

    End Sub

    '/// will be called by System_SetUserInfo method
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Welcome notification e-mail for the self registered user 
    ''' </summary>
    ''' <param name="UserInfo">The user information object</param>
    ''' <remarks>
    '''     This method will be called by the standard mechanisms of camm Web-Manager
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overloads Overrides Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation)

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Which language should be used for creation of mail which should be in the preferred language of the user
        Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(UserInfo)
        cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

        Dim MainSubject As String, eMailBody As String, eMailHTMLBody As String
        MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
        eMailBody = UserSalutation(UserInfo) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome, UserInfo.LoginName, GetUserLogonServers(cammWebManager, UserInfo.ID)) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            cammWebManager.StandardEMailAccountName & ControlChars.CrLf
        eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(UserInfo)) & "</p>" & _
            "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(UserInfo.LoginName) & "</strong></font>", CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, UserInfo.ID)))) & "</p>" & _
            "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            "</body></html>"

        If cammWebManager.MessagingEMails.SendEMail(UserInfo.FullName, UserInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal) = False Then
            If MailLanguage <> BackupOfCurrentLanguageID Then
                'Restore the current language profile
                'Language data has been switched; switch back now
                cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)
            End If
            Throw New Exception("Error sending mail to new user")
        End If

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

    End Sub

    '/// will be called by System_SetUserInfo method
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Welcome notification e-mail for the user created by a security administrator with mention of the password 
    ''' </summary>
    ''' <param name="UserInfo">The user information object</param>
    ''' <param name="Password">The new password</param>
    ''' <remarks>
    '''     This method will be called by the standard mechanisms of camm Web-Manager
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Sub NotificationForUser_Welcome_UserHasBeenCreated(ByVal UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation, ByVal Password As String)

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Which language should be used for creation of mail which should be in the preferred language of the user
        Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(UserInfo)
        cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

        Dim MainSubject As String, eMailBody As String, eMailHTMLBody As String
        MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
        eMailBody = UserSalutation(UserInfo) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextWelcome, UserInfo.LoginName, Password, GetUserLogonServers(cammWebManager, UserInfo.ID)) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            cammWebManager.StandardEMailAccountName & ControlChars.CrLf
        eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(UserInfo)) & "</p>" & _
            "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextWelcome, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(UserInfo.LoginName) & "</strong></font>", "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(Password) & "</strong></font>", CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, UserInfo.ID)))) & "</p>" & _
            "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            "</body></html>"

        Dim errors As String = String.Empty
        If cammWebManager.MessagingEMails.SendEMail(UserInfo.FullName, UserInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal, , , , , errors) = False Then
            If MailLanguage <> BackupOfCurrentLanguageID Then
                'Restore the current language profile
                'Language data has been switched; switch back now
                cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)
            End If
            If errors = Nothing Then
                Throw New Exception("Error sending mail to new user")
            Else
                Throw New Exception("Error sending mail to new user (" & errors & ")")
            End If
        End If

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

    End Sub

    '/// will be called by System_SetUserInfo method
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Notification e-mail for every security administrator to review and authorize a new user account
    ''' </summary>
    ''' <param name="UserInfoToBeReviewed">The user information object to be reviewed</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Sub NotificationForSecurityAdministration_ReviewNewUserAccount(ByVal UserInfoToBeReviewed As CompuMaster.camm.WebManager.WMSystem.UserInformation)
        Dim MailLanguage As Integer
        Dim Success As Boolean
        Dim CommentOfNewUser As String = Nothing
        If Not UserInfoToBeReviewed.AdditionalFlags Is Nothing Then
            CommentOfNewUser = UserInfoToBeReviewed.AdditionalFlags("OnCreationComment")
        End If

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Creation of the e-mails for the security admins
        Dim MySecurityAdmins As New CompuMaster.camm.WebManager.WMSystem.GroupInformation(CompuMaster.camm.WebManager.WMSystem.SpecialGroups.Group_SecurityAdministrators, cammWebManager)
        If Not MySecurityAdmins Is Nothing AndAlso Not MySecurityAdmins.Members Is Nothing Then
            For Each MySecurityAdmin As CompuMaster.camm.WebManager.WMSystem.UserInformation In MySecurityAdmins.Members
                MailLanguage = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(MySecurityAdmin)
                cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)
                'GetLanguageTitles
                Dim email4SecurityAdminMsgHTMLBody As String
                Dim email4SecurityAdminMsgBody As String
                Dim emailIntroduction As String
                If Not System.Web.HttpContext.Current Is Nothing AndAlso cammWebManager.System_GetCurUserID(CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous) = CompuMaster.camm.WebManager.WMSystem.SpecialUsers.User_Anonymous Then
                    'No user logged in --> we have created our own account, now (the session wouldn't get the user information before returning from this method)
                    'User has created his account himself
                    emailIntroduction = cammWebManager.Internationalization.CreateAccount_MsgEMail4Admin
                Else
                    'Account has been created by current logged in admin or one of his collegues
                    emailIntroduction = cammWebManager.Internationalization.UserManagement_NewUser_MsgEMail4Admin
                End If
                email4SecurityAdminMsgBody = UserSalutation(MySecurityAdmin) & ControlChars.CrLf & _
                    ControlChars.CrLf & _
                    emailIntroduction & ControlChars.CrLf & _
                    ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleLogin & UserInfoToBeReviewed.LoginName & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleCompany & UserInfoToBeReviewed.Company & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleName & UserInfoToBeReviewed.FullName & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleEMailAddress & UserInfoToBeReviewed.EMailAddress & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleStreet & UserInfoToBeReviewed.Street & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleZIPCode & UserInfoToBeReviewed.ZipCode & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleLocation & UserInfoToBeReviewed.Location & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleState & UserInfoToBeReviewed.State & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleCountry & UserInfoToBeReviewed.Country & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitle1stLanguage & UserInfoToBeReviewed.PreferredLanguage1.LanguageName_English & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitle2ndLanguage & UserInfoToBeReviewed.PreferredLanguage2.LanguageName_English & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitle3rdLanguage & UserInfoToBeReviewed.PreferredLanguage3.LanguageName_English & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleComesFrom & UserInfoToBeReviewed.AdditionalFlags("ComesFrom") & ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleMotivation & UserInfoToBeReviewed.AdditionalFlags("Motivation") & ControlChars.CrLf & _
                    ControlChars.CrLf & _
                    cammWebManager.Internationalization.UserManagementEMailColumnTitleComment & ControlChars.CrLf & CommentOfNewUser
                email4SecurityAdminMsgHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                    "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(MySecurityAdmin)) & "</p>" & _
                    "<p>" & CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(emailIntroduction)) & "</p>" & _
                    "<table border=""0"">" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleLogin & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.LoginName) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleCompany & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.Company) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleName & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.FullName) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleEMailAddress & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.EMailAddress) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleStreet & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.Street) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleZIPCode & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.ZipCode) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleLocation & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.Location) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleState & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.State) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleCountry & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.Country) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitle1stLanguage & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.PreferredLanguage1.LanguageName_English) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitle2ndLanguage & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.PreferredLanguage2.LanguageName_English) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitle3rdLanguage & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.PreferredLanguage3.LanguageName_English) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleComesFrom & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.AdditionalFlags("ComesFrom")) & "</td></tr>" & _
                    "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleMotivation & "</td><td>" & System.Web.HttpUtility.HtmlEncode(UserInfoToBeReviewed.AdditionalFlags("Motivation")) & "</td></tr>" & _
                    CType(IIf(CommentOfNewUser <> "", "<tr><td colspan=""2"">&nbsp;</td></tr>" & _
                    "<tr><td colspan=""2""><h4>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleComment & "</h4>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(CommentOfNewUser)) & "</td></tr>", ""), String) & _
                    "</table></body></html>"
                Success = cammWebManager.MessagingEMails.SendEMail(MySecurityAdmin.FullName, MySecurityAdmin.EMailAddress, cammWebManager.Internationalization.UserManagementEMailTextSubject4AdminNewUser, email4SecurityAdminMsgBody, email4SecurityAdminMsgHTMLBody, "", "")
            Next
        End If

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

    End Sub

    '/// will be called by System_SetUserPassword method
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Notification e-mail for the user with mention of the password when the password has been changed by a security administrator
    ''' </summary>
    ''' <param name="UserInfo">The user information object</param>
    ''' <param name="NewPassword">The new password</param>
    ''' <remarks>
    '''     This method will be called by the standard mechanisms of camm Web-Manager
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Sub NotificationForUser_ResettedPassword(ByVal UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation, ByVal NewPassword As String)
        Dim MainSubject As String
        Dim eMailBody As String
        Dim eMailHTMLBody As String
        Dim Success As Boolean

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Which language should be used for creation of mail which should be in the preferred language of the user
        Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(UserInfo)
        cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

        MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
        eMailBody = UserSalutation(UserInfo) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UserManagement_ResetPWByAdmin_EMailMsg, NewPassword, GetUserLogonServers(cammWebManager, UserInfo.ID)) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            cammWebManager.StandardEMailAccountName & ControlChars.CrLf
        eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(UserInfo)) & "</p>" & _
            "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UserManagement_ResetPWByAdmin_EMailMsg, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(NewPassword) & "</strong></font>", CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, UserInfo.ID)))) & "</p>" & _
            "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            "</body></html>"
        Dim ErrorDetailsBuffer As String = String.Empty
        Success = cammWebManager.MessagingEMails.SendEMail(UserInfo.FullName, UserInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", "", "", Nothing, Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal, , , , , ErrorDetailsBuffer)

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

        If Success = False Then
            If cammWebManager.DebugLevel >= CompuMaster.camm.WebManager.WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                Throw New Exception("Error sending mail: " & ErrorDetailsBuffer)
            Else
                Throw New Exception("Error sending mail")
            End If
        End If

    End Sub

    '/// will be called by the appropriate formular
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Notification e-mail for the user who has forgotten his password
    ''' </summary>
    ''' <param name="UserInfo">The user information object</param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Sub NotificationForUser_ForgottenPassword(ByVal UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation)
        Dim PW As String = cammWebManager.System_GetUserPassword(UserInfo.LoginName, UserInfo.EMailAddress)
        Dim MainSubject As String
        Dim eMailBody As String
        Dim eMailHTMLBody As String
        Dim Success As Boolean

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Which language should be used for creation of mail which should be in the preferred language of the user
        Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(UserInfo)
        cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

        MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
        eMailBody = UserSalutation(UserInfo) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.SendPassword_EMailMessage, PW, GetUserLogonServers(cammWebManager, UserInfo.ID)) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            cammWebManager.StandardEMailAccountName & ControlChars.CrLf
        eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(UserInfo)) & "</p>" & _
            "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.SendPassword_EMailMessage, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(PW) & "</strong></font>", CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, UserInfo.ID)))) & "</p>" & _
            "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            "</body></html>"
        Success = cammWebManager.MessagingEMails.SendEMail(UserInfo.FullName, UserInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal)

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

        If Success = False Then
            Throw New Exception("Error sending mail")
        End If

    End Sub

    '/// will be called by the appropriate admin formular
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Notification e-mail for the user that he has got his first authorizations and that it makes sense now to revisit us again
    ''' </summary>
    ''' <param name="UserInfo"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Overrides Sub NotificationForUser_AuthorizationsSet(ByVal UserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation)

        Dim Success As Boolean

        'Backup current language ID
        Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UIMarket

        'Which language should be used for creation of mail which should be in the preferred language of the user
        Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(UserInfo)
        cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

        'Create and send the mail
        Dim MainSubject As String = cammWebManager.Internationalization.UserManagement_NewUser_SubjectAuthCheckSuccessfull
        Dim eMailBody As String = UserSalutation(UserInfo) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextAuthCheckSuccessfull, UserInfo.LoginName, GetUserLogonServers(cammWebManager, UserInfo.ID)) & ControlChars.CrLf & _
            ControlChars.CrLf & _
            cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            cammWebManager.StandardEMailAccountName & ControlChars.CrLf
        Dim eMailHTMLBody As String
        eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(UserInfo)) & "</p>" & _
            "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(CompuMaster.camm.WebManager.Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextAuthCheckSuccessfull, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(UserInfo.LoginName) & "</strong></font>", CompuMaster.camm.WebManager.Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, UserInfo.ID)))) & "</p>" & _
            "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            "</body></html>"
        cammWebManager.MessagingEMails.SendEMail(UserInfo.FullName, UserInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, Nothing, Nothing, CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMailAttachment()), CType(Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Priority), CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal)

        'Restore the current language profile
        cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

        If Success = False Then
            Throw New Exception("Error sending mail")
        End If

    End Sub

End Class