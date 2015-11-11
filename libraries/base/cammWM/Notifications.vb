Option Explicit On 
Option Strict On

Imports System.Web
Imports CompuMaster.camm.WebManager.WMSystem

Namespace CompuMaster.camm.WebManager.Notifications

    ''' <summary>
    '''     The common interface for all notification providers
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Interface INotifications

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Welcome notification e-mail for the self registered user with mention of the password 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="password">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal userInfo As UserInformation, ByVal password As String)

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Welcome notification e-mail for the self registered user 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal userInfo As UserInformation)

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Welcome notification e-mail for the user created by a security administrator with mention of the password 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="password">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_Welcome_UserHasBeenCreated(ByVal userInfo As UserInformation, ByVal password As String)

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for every security administrator to review and authorize a new user account
        ''' </summary>
        ''' <param name="userInfoToBeReviewed">The user information object to be reviewed</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForSecurityAdministration_ReviewNewUserAccount(ByVal userInfoToBeReviewed As UserInformation)


        '/// will be called by System_SetUserPassword method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user with mention of the password when the password has been changed by a security administrator
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="newPassword">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_ResettedPassword(ByVal userInfo As UserInformation, ByVal newPassword As String)


        '/// will be called by the appropriate formular
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user who has forgotten his password
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_ForgottenPassword(ByVal userInfo As UserInformation)

        ''' <summary>
        '''    Notifcation e-mail for the user. This will send a link to a formular in which the user can set a new password
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <param name="resetLinkUrl"></param>
        ''' <remarks></remarks>
        Sub NotificationForUser_PasswordResetLink(ByVal userInfo As UserInformation, ByVal resetLinkUrl As String)


        '/// will be called by the appropriate admin formular
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user that he has got his first authorizations and that it makes sense now to revisit us again
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_AuthorizationsSet(ByVal userInfo As UserInformation)

        '/// will be called by the appropriate admin formular
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user that he has to activate his account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Sub NotificationForUser_ActivationRequired(ByVal userInfo As UserInformation)

        Sub SendSupportContractHasExpiredMessage(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date)
        Sub SendLicenceHasExpiredMessage(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date)
        Sub SendUpdateContractHasExpiredMessage(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date)

    End Interface

#Region "Notifications"

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMNotifications
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     The default notification e-mails of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    '''     Use this class to modify/customize the e-mails to all your users
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DefaultNotifications
        Implements INotifications

        Protected cammWebManager As WMSystem

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
        Public Sub New(ByVal webManager As WMSystem)
            cammWebManager = webManager
        End Sub

        Protected Overridable Function UserSalutation(ByVal userInfo As WMSystem.UserInformation) As String
            Return userInfo.SalutationInMails
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Get a string with all logon servers for a user
        ''' </summary>
        ''' <param name="webManager">An instance of camm Web-Manager</param>
        ''' <param name="userID">A user ID</param>
        ''' <returns>A string with all relative server groups; every server group is placed in a new line.</returns>
        ''' <remarks>
        '''     If there is only 1 server group available, the returned string contains only the simply URL of the master server of this server group.
        '''     Are there 2 or more server groups available then each URL of the corresponding master server is followed by the server group title in parenthesis.
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function GetUserLogonServers(ByVal webManager As IWebManager, ByVal userID As Long) As String
            Return CompuMaster.camm.WebManager.DataLayer.Current.GetUserLogonServers(webManager, userID)
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
            If Utils.IsRightToLeftCulture(System.Globalization.CultureInfo.CurrentUICulture) Then
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
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="password">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Overloads Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal userInfo As UserInformation, ByVal password As String) Implements INotifications.NotificationForUser_Welcome_UserRegisteredByHimself

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            Dim MainSubject As String, eMailBody As String, eMailHTMLBody As String
            MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
            eMailBody = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome_WithPassword, userInfo.LoginName, password, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome_WithPassword, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(userInfo.LoginName) & "</strong></font>", "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(password) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"

            If cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal) = False Then
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
        ''' <param name="userInfo">The user information object</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Overloads Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal userInfo As UserInformation) Implements INotifications.NotificationForUser_Welcome_UserRegisteredByHimself

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            Dim MainSubject As String, eMailBody As String, eMailHTMLBody As String
            MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
            eMailBody = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome, userInfo.LoginName, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.CreateAccount_MsgEMailWelcome, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(userInfo.LoginName) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"

            If cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal) = False Then
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
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="password">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_Welcome_UserHasBeenCreated(ByVal userInfo As UserInformation, ByVal password As String) Implements INotifications.NotificationForUser_Welcome_UserHasBeenCreated

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            Dim MainSubject As String, eMailBody As String, eMailHTMLBody As String
            MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
            eMailBody = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextWelcome, userInfo.LoginName, password, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextWelcome, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(userInfo.LoginName) & "</strong></font>", "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(password) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"

            Dim errors As String = String.Empty
            If cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal, , , , , errors) = False Then
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
        ''' <param name="userInfoToBeReviewed">The user information object to be reviewed</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForSecurityAdministration_ReviewNewUserAccount(ByVal userInfoToBeReviewed As UserInformation) Implements INotifications.NotificationForSecurityAdministration_ReviewNewUserAccount
            Dim MailLanguage As Integer
            Dim Success As Boolean
            Dim CommentOfNewUser As String = Nothing
            If Not userInfoToBeReviewed.AdditionalFlags Is Nothing Then
                CommentOfNewUser = userInfoToBeReviewed.AdditionalFlags("OnCreationComment")
            End If

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID
            'Creation of the e-mails for the security admins
            Dim MySecurityAdmins As New GroupInformation(WMSystem.SpecialGroups.Group_SecurityAdministrators, cammWebManager)
            If Not MySecurityAdmins Is Nothing AndAlso Not MySecurityAdmins.Members Is Nothing Then
                For Each MySecurityAdmin As UserInformation In MySecurityAdmins.Members
                    MailLanguage = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(MySecurityAdmin)
                    cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)
                    'GetLanguageTitles
                    Dim email4SecurityAdminMsgHTMLBody As String
                    Dim email4SecurityAdminMsgBody As String
                    Dim emailIntroduction As String
                    If Not HttpContext.Current Is Nothing AndAlso cammWebManager.CurrentUserID(SpecialUsers.User_Anonymous) = SpecialUsers.User_Anonymous Then
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
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleLogin & userInfoToBeReviewed.LoginName & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleCompany & userInfoToBeReviewed.Company & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleName & userInfoToBeReviewed.FullName & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleEMailAddress & userInfoToBeReviewed.EMailAddress & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleStreet & userInfoToBeReviewed.Street & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleZIPCode & userInfoToBeReviewed.ZipCode & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleLocation & userInfoToBeReviewed.Location & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleState & userInfoToBeReviewed.State & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleCountry & userInfoToBeReviewed.Country & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitle1stLanguage & userInfoToBeReviewed.PreferredLanguage1.LanguageName_English & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitle2ndLanguage & userInfoToBeReviewed.PreferredLanguage2.LanguageName_English & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitle3rdLanguage & userInfoToBeReviewed.PreferredLanguage3.LanguageName_English & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleComesFrom & userInfoToBeReviewed.AdditionalFlags("ComesFrom") & ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleMotivation & userInfoToBeReviewed.AdditionalFlags("Motivation") & ControlChars.CrLf & _
                        ControlChars.CrLf & _
                        cammWebManager.Internationalization.UserManagementEMailColumnTitleComment & ControlChars.CrLf & CommentOfNewUser
                    email4SecurityAdminMsgHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                        "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(MySecurityAdmin)) & "</p>" & _
                        "<p>" & Utils.HighlightLinksInMessage(Utils.HTMLEncodeLineBreaks(emailIntroduction)) & "</p>" & _
                        "<table border=""0"">" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleLogin & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.LoginName) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleCompany & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.Company) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleName & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.FullName) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleEMailAddress & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.EMailAddress) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleStreet & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.Street) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleZIPCode & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.ZipCode) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleLocation & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.Location) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleState & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.State) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleCountry & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.Country) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitle1stLanguage & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.PreferredLanguage1.LanguageName_English) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitle2ndLanguage & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.PreferredLanguage2.LanguageName_English) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitle3rdLanguage & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.PreferredLanguage3.LanguageName_English) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleComesFrom & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.AdditionalFlags("ComesFrom")) & "</td></tr>" & _
                        "<tr><td>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleMotivation & "</td><td>" & System.Web.HttpUtility.HtmlEncode(userInfoToBeReviewed.AdditionalFlags("Motivation")) & "</td></tr>" & _
                        CType(IIf(CommentOfNewUser <> "", "<tr><td colspan=""2"">&nbsp;</td></tr>" & _
                        "<tr><td colspan=""2""><h4>" & cammWebManager.Internationalization.UserManagementEMailColumnTitleComment & "</h4>" & Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(CommentOfNewUser)) & "</td></tr>", ""), String) & _
                        "</table></body></html>"
                    Success = cammWebManager.MessagingEMails.SendEMail(MySecurityAdmin.FullName, MySecurityAdmin.EMailAddress, cammWebManager.Internationalization.UserManagementEMailTextSubject4AdminNewUser, email4SecurityAdminMsgBody, email4SecurityAdminMsgHTMLBody, "", "")
                Next
            End If

            'Restore the current language profile
            cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

        End Sub

        Private alreadyNotifiedMailsExpiredContract As New ArrayList

        Public Sub SendSupportContractHasExpiredMessage(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date) Implements INotifications.SendSupportContractHasExpiredMessage
            Dim body As String = "Dear Sirs," & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your camm Web-Manager support and maintenance subscription expired on " & expirationDate.ToShortDateString() & "." & ChrW(13) & ChrW(10) & _
                "Don't forget to renew your support and maintenance subscription at http://www.camm.biz/redir/?R=35" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Some of your possible benefits:" & ChrW(13) & ChrW(10) & _
                "- Always updated camm Web-Manager software available" & ChrW(13) & ChrW(10) & _
                "- Support for your employees and developers by e-mail, ticket system and phone" & ChrW(13) & ChrW(10) & _
                "- Support for your individual compliance requirements to match your local law" & ChrW(13) & ChrW(10) & _
                "- You pay only for your actual needs - never be over-licensed" & ChrW(13) & ChrW(10) & _
                "- Individually tailored licenses for efficient support - never be sub-license" & ChrW(13) & ChrW(10) & _
                "- Fair conditions for your standard requirements as well as for your individual needs" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Please note:" & ChrW(13) & ChrW(10) & _
                "- You need an active subscription to receive support and maintenance. Using outdated software could be a risk to your server infrastructure in case of required security updates. " & ChrW(13) & ChrW(10) & _
                "- Optional updates might contain important updates supporting your compliance with local law (e.g. data protection rules)." & ChrW(13) & ChrW(10) & _
                "- Optional updates might contain important updates with support for latest browser platforms." & ChrW(13) & ChrW(10) & _
                "- A missing support and maintenance subscription could extend the waiting period for immediately needed support - System failures could result." & ChrW(13) & ChrW(10)
            cammWebManager.MessagingEMails.SendEMail(recipientName, recipientEmail, "Your camm Web-Manager support and maintenance contract expired", body, Nothing, "", "")
        End Sub
        Public Sub SendLicenceHasExpiredMessage(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date) Implements INotifications.SendLicenceHasExpiredMessage
            Dim body As String = "Dear Sirs, " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your camm Web-Manager license expires on " & expirationDate.ToShortDateString() & "." & ChrW(13) & ChrW(10) & _
                "Please renew your license on time to continue using your product without interruption." & ChrW(13) & ChrW(10) & _
                "http://www.camm.biz/redir/?R=33" & ChrW(13) & ChrW(10) & _
                 ChrW(13) & ChrW(10) & _
                "Please note:" & ChrW(13) & ChrW(10) & _
                "- When your license expires, your current license edition will be reset to the Demo License. Your current feature set will automatically be reduced to the standard feature set of the that license." & ChrW(13) & ChrW(10) & _
                "- You need an active subscription to receive latest updates. Using outdated software could be a risk to your server infrastructure in case of required security updates." & ChrW(13) & ChrW(10) & _
                "- Optional updates might contain important updates supporting you for your compliance with local law (e.g. data protection rules)." & ChrW(13) & ChrW(10) & _
                "- Optional updates might contain important updates with support for latest browser platforms."
            cammWebManager.MessagingEMails.SendEMail(recipientName, recipientEmail, "Your camm Web-Manager license is about to expire", body, Nothing, "", "")
        End Sub

        Public Sub SendUpdateContractHasExpiredMessage(ByVal recipientName As String, ByVal recipientEmail As String, ByVal expirationDate As Date) Implements INotifications.SendUpdateContractHasExpiredMessage
            Dim body As String = "Dear Sirs, " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your camm Web-Manager update subscription expired on " & expirationDate.ToShortDateString() & "." & ChrW(13) & ChrW(10) & _
                "Please renew your update contract on time to be able to download the most current product version with latest patches and enhancements." & ChrW(13) & ChrW(10) & _
                "http://www.camm.biz/redir/?R=34" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Please note:" & ChrW(13) & ChrW(10) & _
                "- You need an active subscription to receive latest updates. Using outdated software could be a risk to your server infrastructure in case of required security updates." & ChrW(13) & ChrW(10) & _
                "- Optional updates might contain important updates supporting you for your compliance with local law (e.g. data protection rules)." & ChrW(13) & ChrW(10) & _
                "- Optional updates might contain important updates with support for latest browser platforms."
            cammWebManager.MessagingEMails.SendEMail(recipientName, recipientEmail, "Your camm Web-Manager update contract expired", body, Nothing, "", "")
        End Sub

        '/// will be called by System_SetUserPassword method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user with mention of the password when the password has been changed by a security administrator
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="newPassword">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_ResettedPassword(ByVal userInfo As UserInformation, ByVal newPassword As String) Implements INotifications.NotificationForUser_ResettedPassword
            Dim MainSubject As String
            Dim eMailBody As String
            Dim eMailHTMLBody As String
            Dim Success As Boolean

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
            eMailBody = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.UserManagement_ResetPWByAdmin_EMailMsg, newPassword, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.UserManagement_ResetPWByAdmin_EMailMsg, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(newPassword) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"
            Dim ErrorDetailsBuffer As String = String.Empty
            Success = cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", "", "", Nothing, Nothing, CompuMaster.camm.WebManager.Messaging.EMails.Sensitivity.Status_Personal, , , , , ErrorDetailsBuffer)

            'Restore the current language profile
            cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

            If Success = False Then
                If cammWebManager.DebugLevel >= DebugLevels.Medium_LoggingOfDebugInformation Then
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
        ''' <param name="userInfo">The user information object</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_ForgottenPassword(ByVal userInfo As UserInformation) Implements INotifications.NotificationForUser_ForgottenPassword
            Dim PW As String = cammWebManager.System_GetUserPassword(userInfo.LoginName, userInfo.EMailAddress)
            Dim MainSubject As String
            Dim eMailBody As String
            Dim eMailHTMLBody As String
            Dim Success As Boolean

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
            eMailBody = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.SendPassword_EMailMessage, PW, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.SendPassword_EMailMessage, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(PW) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"
            Success = cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal)

            'Restore the current language profile
            cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

            If Success = False Then
                Throw New Exception("Error sending mail")
            End If

        End Sub

        Public Overridable Sub NotificationForUser_PasswordResetLink(ByVal userInfo As UserInformation, ByVal resetLinkUrl As String) Implements INotifications.NotificationForUser_PasswordResetLink
            Dim MainSubject As String
            Dim eMailBody As String
            Dim eMailHTMLBody As String
            Dim Success As Boolean

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            MainSubject = cammWebManager.Internationalization.UserManagementEMailTextSubject
            eMailBody = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.SendPasswordResetLink_EMailMessage, resetLinkUrl) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf

            resetLinkUrl = System.Web.HttpUtility.HtmlEncode(resetLinkUrl)

            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.SendPasswordResetLink_EMailMessage, "<a href=""" & resetLinkUrl & """>" & resetLinkUrl & "</a>")) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"
            Success = cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, "", "", CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal)

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
        ''' <param name="userInfo"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_AuthorizationsSet(ByVal userInfo As UserInformation) Implements INotifications.NotificationForUser_AuthorizationsSet

            Dim Success As Boolean

            'Backup current language ID
            Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            'Which language should be used for creation of mail which should be in the preferred language of the user
            Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            'Create and send the mail
            Dim MainSubject As String = cammWebManager.Internationalization.UserManagement_NewUser_SubjectAuthCheckSuccessfull
            Dim eMailBody As String = UserSalutation(userInfo) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextAuthCheckSuccessfull, userInfo.LoginName, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
                ControlChars.CrLf & _
                cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
                cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            Dim eMailHTMLBody As String
            eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
                "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
                "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextAuthCheckSuccessfull, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(userInfo.LoginName) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
                "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
                "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
                "</body></html>"
            cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, Nothing, Nothing, CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal)

            'Restore the current language profile
            cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

            If Success = False Then
                Throw New Exception("Error sending mail")
            End If

        End Sub

        '/// will be called by the appropriate admin formular
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user that he has to activate his account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub NotificationForUser_ActivationRequired(ByVal userInfo As WMSystem.UserInformation) Implements INotifications.NotificationForUser_ActivationRequired
            'TODO: Implementation
            Throw New NotImplementedException("NotificationForUser_ActivationRequired")

            'Dim Success As Boolean

            ''Backup current language ID
            'Dim BackupOfCurrentLanguageID As Integer = cammWebManager.UI.MarketID

            ''Which language should be used for creation of mail which should be in the preferred language of the user
            'Dim MailLanguage As Integer = cammWebManager.System_Get1stPreferredLanguageWhichIsSupportedByTheSystem(userInfo)
            'cammWebManager.Internationalization.LoadLanguageStrings(MailLanguage)

            ''Create and send the mail
            'Dim MainSubject As String = cammWebManager.Internationalization.UserManagement_NewUser_SubjectAuthCheckSuccessfull
            'Dim eMailBody As String = UserSalutation(userInfo) & ControlChars.CrLf & _
            '    ControlChars.CrLf & _
            '    Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextAuthCheckSuccessfull, userInfo.LoginName, GetUserLogonServers(cammWebManager, userInfo.ID)) & ControlChars.CrLf & _
            '    ControlChars.CrLf & _
            '    cammWebManager.Internationalization.UserManagementEMailTextRegards & ControlChars.CrLf & _
            '    cammWebManager.StandardEMailAccountName & ControlChars.CrLf
            'Dim eMailHTMLBody As String
            'eMailHTMLBody = "" & HtmlTagOpener() & "<head><style>BODY { font-family: Arial, Helvetica } </style></head><body>" & _
            '    "<p>" & System.Web.HttpUtility.HtmlEncode(UserSalutation(userInfo)) & "</p>" & _
            '    "<p>" & Utils.HTMLEncodeLineBreaks(Utils.sprintf(cammWebManager.Internationalization.UserManagement_NewUser_TextAuthCheckSuccessfull, "<font color=""#FF0000""><strong>" & System.Web.HttpUtility.HtmlEncode(userInfo.LoginName) & "</strong></font>", Utils.HighlightLinksInMessage(GetUserLogonServers(cammWebManager, userInfo.ID)))) & "</p>" & _
            '    "<p>" & cammWebManager.Internationalization.UserManagementEMailTextRegards & "<br>" & _
            '    "<em>" & System.Web.HttpUtility.HtmlEncode(cammWebManager.StandardEMailAccountName) & "</em></p>" & _
            '    "</body></html>"
            'cammWebManager.MessagingEMails.SendEMail(userInfo.FullName, userInfo.EMailAddress, MainSubject, eMailBody, eMailHTMLBody, Nothing, Nothing, CType(Nothing, Messaging.EMailAttachment()), CType(Nothing, Messaging.EMails.Priority), Messaging.EMails.Sensitivity.Status_Personal)

            ''Restore the current language profile
            'cammWebManager.Internationalization.LoadLanguageStrings(BackupOfCurrentLanguageID)

            'If Success = False Then
            '    Throw New Exception("Error sending mail")
            'End If

        End Sub

    End Class

    Public Class NoNotifications
        Implements INotifications

        Protected cammWebManager As WMSystem

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
        Public Sub New(ByVal webManager As WMSystem)
            cammWebManager = webManager
        End Sub

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Welcome notification e-mail for the self registered user with mention of the password 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="password">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Overloads Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal userInfo As UserInformation, ByVal password As String) Implements INotifications.NotificationForUser_Welcome_UserRegisteredByHimself
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
        Public Overridable Overloads Sub NotificationForUser_Welcome_UserRegisteredByHimself(ByVal userInfo As UserInformation) Implements INotifications.NotificationForUser_Welcome_UserRegisteredByHimself
        End Sub

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Welcome notification e-mail for the user created by a security administrator with mention of the password 
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="password">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_Welcome_UserHasBeenCreated(ByVal userInfo As UserInformation, ByVal password As String) Implements INotifications.NotificationForUser_Welcome_UserHasBeenCreated
        End Sub

        '/// will be called by System_SetUserInfo method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for every security administrator to review and authorize a new user account
        ''' </summary>
        ''' <param name="userInfoToBeReviewed">The user information object to be reviewed</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForSecurityAdministration_ReviewNewUserAccount(ByVal userInfoToBeReviewed As UserInformation) Implements INotifications.NotificationForSecurityAdministration_ReviewNewUserAccount
        End Sub

        '/// will be called by System_SetUserPassword method
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user with mention of the password when the password has been changed by a security administrator
        ''' </summary>
        ''' <param name="userInfo">The user information object</param>
        ''' <param name="newPassword">The new password</param>
        ''' <remarks>
        '''     This method will be called by the standard mechanisms of camm Web-Manager
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_ResettedPassword(ByVal userInfo As UserInformation, ByVal newPassword As String) Implements INotifications.NotificationForUser_ResettedPassword
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
        Public Overridable Sub NotificationForUser_ForgottenPassword(ByVal userInfo As UserInformation) Implements INotifications.NotificationForUser_ForgottenPassword
        End Sub

        Public Overridable Sub NotificationForUser_PassordResetLink(ByVal userInfo As UserInformation, ByVal resetLinkUrl As String) Implements INotifications.NotificationForUser_PasswordResetLink
        End Sub
        '/// will be called by the appropriate admin formular
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user that he has got his first authorizations and that it makes sense now to revisit us again
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overridable Sub NotificationForUser_AuthorizationsSet(ByVal userInfo As UserInformation) Implements INotifications.NotificationForUser_AuthorizationsSet
        End Sub

        '/// will be called by the appropriate admin formular
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Notification e-mail for the user that he has to activate his account
        ''' </summary>
        ''' <param name="userInfo"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	06.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub NotificationForUser_ActivationRequired(ByVal userInfo As WMSystem.UserInformation) Implements INotifications.NotificationForUser_ActivationRequired
        End Sub

        Public Sub SendSupportContractHasExpiredMessage(recipientName As String, recipientEmail As String, expirationDate As Date) Implements INotifications.SendSupportContractHasExpiredMessage
        End Sub

        Public Sub SendLicenceHasExpiredMessage(recipientName As String, recipientEmail As String, expirationDate As Date) Implements INotifications.SendLicenceHasExpiredMessage
        End Sub

        Public Sub SendUpdateContractHasExpiredMessage(recipientName As String, recipientEmail As String, expirationDate As Date) Implements INotifications.SendUpdateContractHasExpiredMessage
        End Sub
    End Class

#End Region

End Namespace

Namespace CompuMaster.camm.WebManager

    '<Obsolete("Use Notifications instead")> _
    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.WMNotifications
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     The default notification e-mails of camm Web-Manager
    ''' </summary>
    ''' <remarks>
    '''     Use this class to modify/customize the e-mails to all your users
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	06.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class WMNotifications
        Inherits Notifications.DefaultNotifications

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
        Public Sub New(ByVal webManager As WMSystem)
            MyBase.New(WebManager)
        End Sub

    End Class

End Namespace
