'Copyright 2001-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie können es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder späteren veröffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es nützlich sein wird, aber OHNE JEDE GEWÄHRLEISTUNG, bereitgestellt; sogar ohne die implizite Gewährleistung der MARKTFÄHIGKEIT oder EIGNUNG FÜR EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License für weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden für Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Explicit On
Option Strict On

NameSpace CompuMaster.camm.WebManager
    Public Class WMSettingsAndData

        Private Function GetLocalizedString(ByVal Name As String) As String
            Dim Result As String
            Dim ResMngr As Resources.ResourceManager

            Dim MyCachedLanguageData As System.Collections.Specialized.NameValueCollection
            If Not System.Web.HttpContext.Current Is Nothing AndAlso Not System.Web.HttpContext.Current.Application("WebManager.LocalizationInfos") Is Nothing Then
                MyCachedLanguageData = CType(System.Web.HttpContext.Current.Application("WebManager.LocalizationInfos"), System.Collections.Specialized.NameValueCollection)
                Result = MyCachedLanguageData(Name)
            Else
                ResMngr = New Resources.ResourceManager("cammWM.StringRes", System.Reflection.Assembly.GetExecutingAssembly)
                ResMngr.IgnoreCase = True
                Result = ResMngr.GetString(Name)
                ResMngr.ReleaseAllResources()
            End If

            Return (Result)

        End Function

#Region "WebManager Essentials"
        Public Event LanguageDataLoaded(ByVal LanguageID As Integer)

        Public User_Auth_Config_Paths_UserAuthSystem As String = "/"
        Public User_Auth_Config_Paths_Login As String = "/sysdata/login/"
        Public User_Auth_Config_Paths_Administration As String = "/sysdata/admin/"
        Public User_Auth_Config_Paths_StandardIncludes As String = "/system/"
        Public User_Auth_Config_Paths_SystemData As String = "/sysdata/"

        Public User_Auth_Config_Files_Administration_DefaultPageInAdminEMails As String = "memberships.aspx"
        Public User_Auth_Validation_NoRefererURL As String
        Public User_Auth_Validation_LogonScriptURL As String
        Public User_Auth_Validation_AfterLogoutURL As String
        Public User_Auth_Validation_AccessErrorScriptURL As String
        Public User_Auth_Validation_CreateUserAccountInternalURL As String
        Public User_Auth_Validation_TerminateOldSessionScriptURL As String
        Public User_Auth_Validation_CheckLoginURL As String
        Public User_Auth_Config_UserAuthMasterServer As String
        Public User_Auth_Config_CurServerURL As String
        Public OfficialServerGroup_URL As String
        Public OfficialServerGroup_AdminURL As String
        Public OfficialServerGroup_Title As String
        Public OfficialServerGroup_Company_FormerTitle As String
#End Region

#Region "Declarations"
        Public Logon_AskForForcingLogon As String
        Public UserManagementSalutationFormulaUnformalUndefinedGender As String
        Public UserManagementSalutationFormulaInMailsUndefinedGender As String
        Public UserManagementSalutationFormulaUndefinedGender As String
        Public UserManagementSalutationFormulaInMailsGroup As String
        Public UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender As String
        Public UserManagementSalutationFormulaUnformalGroup As String
        Public UserManagementSalutationFormulaUnformalWithFirstNameGroup As String
        Public UserManagementSalutationFormulaGroup As String
        Public UserManagementSalutationFormulaInMailsFeminin As String
        Public UserManagementSalutationFormulaFeminin As String
        Public UserManagementSalutationFormulaInMailsMasculin As String
        Public UserManagementSalutationFormulaMasculin As String
        Public UserManagementSalutationFormulaUnformalFeminin As String
        Public UserManagementSalutationFormulaUnformalMasculin As String
        Public UserManagementSalutationFormulaUnformalWithFirstNameFeminin As String
        Public UserManagementSalutationFormulaUnformalWithFirstNameMasculin As String
        Public ErrorRequiredField As String
        Public SystemButtonYes As String
        Public SystemButtonNo As String
        Public SystemButtonOkay As String
        Public SystemButtonCancel As String
        Public ErrorUserOrPasswordWrong As String
        Public ErrorServerConfigurationError As String
        Public ErrorNoAuthorization As String
        Public ErrorAlreadyLoggedOn As String
        Public ErrorLoggedOutBecauseLoggedOnAtAnotherMachine As String
        Public InfoUserLoggedOutSuccessfully As String
        Public ErrorLogonFailedTooOften As String
        Public ErrorEmptyPassword As String
        Public ErrorUnknown As String
        Public ErrorEmptyField As String
        Public ErrorWrongNetwork As String
        Public ErrorUserAlreadyExists As String
        Public ErrorLoginCreatedSuccessfully As String
        Public ErrorSendPWWrongLoginOrEmailAddress As String
        Public ErrorCookiesMustNotBeDisabled As String
        Public ErrorTimoutOrLoginFromAnotherStation As String
        Public ErrorApplicationConfigurationIsEmpty As String
        Public UserManagementEMailColumnTitleLogin As String
        Public UserManagementEMailColumnTitleCompany As String
        Public UserManagementEMailColumnTitleName As String
        Public UserManagementEMailColumnTitleEMailAddress As String
        Public UserManagementEMailColumnTitleStreet As String
        Public UserManagementEMailColumnTitleZIPCode As String
        Public UserManagementEMailColumnTitleLocation As String
        Public UserManagementEMailColumnTitleState As String
        Public UserManagementEMailColumnTitleCountry As String
        Public UserManagementEMailColumnTitle1stLanguage As String
        Public UserManagementEMailColumnTitle2ndLanguage As String
        Public UserManagementEMailColumnTitle3rdLanguage As String
        Public UserManagementEMailColumnTitleComesFrom As String
        Public UserManagementEMailColumnTitleMotivation As String
        Public UserManagementEMailColumnTitleCustomerNo As String
        Public UserManagementEMailColumnTitleSupplierNo As String
        Public UserManagementEMailColumnTitleComment As String
        Public UserManagementEMailTextDearMr As String
        Public UserManagementEMailTextDearMs As String
        Public UserManagementEMailTextDearUndefinedGender As String
        Public UserManagementSalutationUnformalMasculin As String
        Public UserManagementSalutationUnformalFeminin As String
        Public UserManagementSalutationUnformalUndefinedGender As String
        Public UserManagementEMailTextRegards As String
        Public UserManagementEMailTextSubject As String
        Public UserManagementEMailTextSubject4AdminNewUser As String
        Public UserManagementAddressesMr As String
        Public UserManagementAddressesMs As String
        Public UserManagementMasterServerAvailableInNearFuture As String
        Public HighlightTextIntro As String
        Public HighlightTextTechnicalSupport As String
        Public HighlightTextExtro As String
        Public WelcomeTextWelcomeMessage As String
        Public WelcomeTextFeedbackToContact As String
        Public WelcomeTextIntro As String
        Public NavAreaNameYourProfile As String
        Public NavAreaNameLogin As String
        Public NavLinkNameUpdatePasswort As String
        Public NavLinkNameUpdateProfile As String
        Public NavLinkNameLogout As String
        Public NavLinkNameLogin As String
        Public NavLinkNamePasswordRecovery As String
        Public NavLinkNameNewUser As String
        Public NavPointUpdatedHint As String
        Public NavPointTemporaryHiddenHint As String
        Public Banner_Help As String
        Public Banner_HeadTitle As String
        Public Banner_BodyTitle As String
        Public Banner_Feedback As String
        Public StatusLineLoggedInAs As String
        Public StatusLineUsername As String
        Public StatusLinePassword As String
        Public StatusLineSubmit As String
        Public StatusLineContactUs As String
        Public StatusLineLegalNote As String
        Public StatusLineEditorial As String
        Public StatusLineDataprotection As String
        Public StatusLineCopyright_AllRightsReserved As String
        Public META_CurrentContentLanguage As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public META_DescriptionContent As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public META_PageTopicContent As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public META_KeywordContent As String
        Public SendPassword_Descr_FollowingError As String
        Public SendPassword_Descr_LoginDenied As String
        Public SendPassword_Descr_Title As String
        Public SendPassword_Descr_LoginName As String
        Public SendPassword_Descr_Email As String
        Public SendPassword_Descr_Submit As String
        Public SendPassword_Descr_RequiredFields As String
        Public SendPassword_Descr_BackToLogin As String
        Public SendPassword_Descr_PasswordSentTo As String
        Public SendPassword_Descr_FurtherCommentWithContactAddress As String
        Public AccessError_Descr_FollowingError As String
        Public AccessError_Descr_BackToLogin As String
        Public AccessError_Descr_LoginDenied As String
        Public Logon_Connecting_InProgress As String
        Public Logon_BodyTitle As String
        Public Logon_HeadTitle As String
        Public Logon_Connecting_LoginTimeout As String
        Public Logon_Connecting_RecommendationOnTimeout As String
        Public Logon_BodyPrompt2User As String
        Public Logon_BodyFormUserName As String
        Public Logon_BodyFormUserPassword As String
        Public Logon_BodyFormSubmit As String
        Public Logon_SSO_ADS_PageTitle As String
        Public Logon_SSO_ADS_IdentifiedUserName As String
        Public Logon_SSO_ADS_LabelTakeAnAction As String
        Public Logon_SSO_ADS_RadioRegisterExisting As String
        Public Logon_SSO_ADS_RadioRegisterNew As String
        Public Logon_SSO_ADS_RadioDoNothing As String
        Public Logon_SSO_ADS_ContactUs As String
        Public Logon_SSO_ADS_ButtonNext As String
        Public Logon_SSO_ADS_LabelRegisterExistingLoginName As String
        Public Logon_SSO_ADS_LabelRegisterExistingPassword As String
        Public Logon_SSO_ADS_LabelRegisterNewPassword2 As String
        Public Logon_SSO_ADS_LabelRegisterNewEMail As String
        Public Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo As String
        Public Logon_BodyFormCreateNewAccount As String
        Public Logon_BodyExplanation As String
        Public CreateAccount_Descr_FollowingError As String
        Public CreateAccount_Descr_LoginDenied As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public CreateAccount_Descr_Title As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public CreateAccount_Descr_LoginName As String
        Public CreateAccount_Descr_Submit As String
        Public CreateAccount_Descr_RequiredFields As String
        Public CreateAccount_Descr_BackToLogin As String
        Public CreateAccount_Descr_PageTitle As String
        Public CreateAccount_Descr_UserLogin As String
        Public CreateAccount_Descr_NewLoginName As String
        Public CreateAccount_Descr_NewLoginPassword As String
        Public CreateAccount_Descr_NewLoginPasswordConfirmation As String
        Public CreateAccount_Descr_Address As String
        Public CreateAccount_Descr_Company As String
        Public CreateAccount_Descr_Addresses As String
        Public CreateAccount_Descr_PleaseSelect As String
        Public CreateAccount_Descr_AcademicTitle As String
        Public CreateAccount_Descr_FirstName As String
        Public CreateAccount_Descr_LastName As String
        Public CreateAccount_Descr_NameAddition As String
        Public CreateAccount_Descr_Email As String
        Public CreateAccount_Descr_Street As String
        Public CreateAccount_Descr_ZIPCode As String
        Public CreateAccount_Descr_Location As String
        Public CreateAccount_Descr_State As String
        Public CreateAccount_Descr_Country As String
        Public CreateAccount_Descr_Motivation As String
        Public CreateAccount_Descr_MotivItemDealer As String
        Public CreateAccount_Descr_MotivItemWebSiteVisitor As String
        Public UpdateProfile_Descr_MotivItemDealer As String
        Public UpdateProfile_Descr_MotivItemWebSiteVisitor As String
        Public CreateAccount_Descr_MotivItemJournalist As String
        Public CreateAccount_Descr_MotivItemOther As String
        Public UpdateProfile_Descr_MotivItemJournalist As String
        Public UpdateProfile_Descr_MotivItemOther As String
        Public CreateAccount_Descr_WhereHeard As String
        Public CreateAccount_Descr_WhereItemResellerDealer As String
        Public CreateAccount_Descr_WhereItemFriend As String
        Public UpdateProfile_Descr_WhereItemResellerDealer As String
        Public UpdateProfile_Descr_WhereItemFriend As String
        Public CreateAccount_Descr_WhereItemExhibition As String
        Public CreateAccount_Descr_WhereItemFromUsOurselves As String
        Public CreateAccount_Descr_WhereItemMagazines As String
        Public UpdateProfile_Descr_WhereItemExhibition As String
        Public UpdateProfile_Descr_WhereItemFromUsOurselves As String
        Public UpdateProfile_Descr_WhereItemMagazines As String
        Public CreateAccount_Descr_WhereItemSearchEnginge As String
        Public CreateAccount_Descr_UserDetails As String
        Public CreateAccount_Descr_WhereItemOther As String
        Public UpdateProfile_Descr_WhereItemSearchEnginge As String
        Public UpdateProfile_Descr_WhereItemOther As String
        Public CreateAccount_Descr_Comments As String
        Public ResetPW_Descr_PleaseSpecifyNewPW As String
        Public CreateAccount_Descr_1stPreferredLanguage As String
        Public CreateAccount_Descr_RequestAdditionalAuthorizations As String
        Public CreateAccount_Descr_2ndPreferredLanguage As String
        Public CreateAccount_Descr_3rdPreferredLanguage As String
        Public CreateAccount_Descr_CustomerSupplierData As String
        Public CreateAccount_Descr_MotivItemSupplier As String
        Public CreateAccount_Descr_SupplierNo As String
        Public CreateAccount_Descr_CustomerNo As String
        Public UpdateProfile_Descr_MotivItemSupplier As String
        Public CreateAccount_ErrorJS_InputValue As String
        Public CreateAccount_ErrorJS_Length As String
        Public UpdateProfile_ErrorJS_InputValue As String
        Public UpdateProfile_ErrorJS_Length As String
        Public UpdatePW_Descr_Title As String
        Public UpdatePW_ErrMsg_ConfirmationFailed As String
        Public UpdatePW_ErrMsg_InsertAllRequiredPWFields As String
        Public UpdatePW_ErrMsg_Undefined As String
        Public UpdatePW_ErrMsg_Success As String
        Public UpdatePW_ErrMsg_WrongOldPW As String
        Public UpdatePW_ErrMsg_InsertAllRequiredFields As String
        Public UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW As String
        Public UpdatePW_Descr_CurrentPW As String
        Public UpdatePW_Descr_NewPW As String
        Public UpdatePW_Descr_NewPWConfirm As String
        Public UpdatePW_Descr_Submit As String
        Public UpdatePW_Descr_RequiredFields As String
        Public UpdatePW_Error_PasswordComplexityPolicy As String
        Public UserJustCreated_Descr_AccountCreated As String
        Public UserJustCreated_Descr_LookAroundNow As String
        Public UserJustCreated_Descr_PleaseNote As String
        Public UserJustCreated_Descr_Title As String
        Public UpdateProfile_Descr_Title As String
        Public UpdateProfile_Descr_Mobile As String
        Public UpdateProfile_Descr_Fax As String
        Public UpdateProfile_Descr_Phone As String
        Public UpdateProfile_Descr_PositionInCompany As String
        Public UpdateProfile_ErrMsg_InsertAllRequiredFields As String
        Public UpdateProfile_ErrMsg_MistypedPW As String
        Public UpdateProfile_ErrMsg_Undefined As String
        Public UpdateProfile_ErrMsg_Success As String
        Public UpdateProfile_ErrMsg_LogonTooOften As String
        Public UpdateProfile_ErrMsg_NotAllowed As String
        Public UpdateProfile_ErrMsg_PWRequired As String
        Public UpdateProfile_Descr_Address As String
        Public UpdateProfile_Descr_Company As String
        Public UpdateProfile_Descr_Addresses As String
        Public UpdateProfile_Descr_PleaseSelect As String
        Public UpdateProfile_Abbrev_Mister As String
        Public UpdateProfile_Abbrev_Miss As String
        Public UpdateProfile_Descr_AcademicTitle As String
        Public UpdateProfile_Descr_FirstName As String
        Public UpdateProfile_Descr_LastName As String
        Public UpdateProfile_Descr_NameAddition As String
        Public UpdateProfile_Descr_EMail As String
        Public UpdateProfile_Descr_Street As String
        Public UpdateProfile_Descr_ZIPCode As String
        Public UpdateProfile_Descr_Location As String
        Public UpdateProfile_Descr_State As String
        Public UpdateProfile_Descr_Country As String
        Public UpdateProfile_Descr_UserDetails As String
        Public UpdateProfile_Descr_1stLanguage As String
        Public UpdateProfile_Descr_2ndLanguage As String
        Public UpdateProfile_Descr_3rdLanguage As String
        Public UpdateProfile_Descr_Authentification As String
        Public UpdateProfile_Descr_Password As String
        Public UpdateProfile_Descr_Submit As String
        Public UpdateProfile_Descr_RequiredFields As String
        Public UpdateProfile_Descr_CustomerSupplierData As String
        Public UpdateProfile_Descr_CustomerNo As String
        Public UpdateProfile_Descr_SupplierNo As String
        Public CreateAccount_MsgEMailWelcome As String
        Public CreateAccount_MsgEMailWelcome_WithPassword As String
        Public CreateAccount_MsgEMail4Admin As String
        Public UserManagement_NewUser_TextWelcome As String
        Public UserManagement_NewUser_HTMLWelcome As String
        Public UserManagement_NewUser_MsgEMail4Admin As String
        Public SendPassword_EMailMessage As String
        Public SendPasswordResetLink_EMailMessage As String
        Public UserManagement_ResetPWByAdmin_EMailMsg As String
        Public UserManagement_NewUser_SubjectAuthCheckSuccessfull As String
        Public UserManagement_NewUser_TextAuthCheckSuccessfull As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public UserManagementEMailTextDearMsWithAcademicTitle As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public UserManagementEMailTextDearMrWithAcademicTitle As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public UserManagementPleaseAssignMemberships As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public UserManagementResetPasswordBySecurityAdmin As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public UserManagementDonTForgetToChangePassword As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public HighlightTexteMedia As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public DoNotRedirectToLoginPathWithRefererDataIfNotLoggedIn As String
        <Obsolete("Field not required any more, no content has been set up for this field"), System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> Public UserManagementAdjustMembershipsHere As String
#End Region

        Public Overridable Function GetLanguageIDOfCultureName(ByVal CultureName As String) As Integer
            Return GetLanguageIDOfBrowserSetting(CultureName)
        End Function

        Friend Overridable Function GetLanguageIDOfBrowserSetting(SettingValue As String) As Integer
            If SettingValue = Nothing Then
                Return Nothing
            End If

            Dim Result As Integer
            Select Case SettingValue.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                Case "af"
                    Result = 12
                Case "af-af"
                    Result = 12
                Case "ar"
                    Result = 21
                Case "ar-ae"
                    Result = 494
                Case "ar-ar"
                    Result = 21
                Case "ar-bh"
                    Result = 481
                Case "ar-dz"
                    Result = 480
                Case "ar-eg"
                    Result = 479
                Case "ar-iq"
                    Result = 482
                Case "ar-jo"
                    Result = 484
                Case "ar-kw"
                    Result = 486
                Case "ar-lb"
                    Result = 487
                Case "ar-ly"
                    Result = 488
                Case "ar-ma"
                    Result = 489
                Case "ar-om"
                    Result = 490
                Case "ar-qa"
                    Result = 485
                Case "ar-sa"
                    Result = 491
                Case "ar-sy"
                    Result = 492
                Case "ar-tn"
                    Result = 493
                Case "ar-ye"
                    Result = 483
                Case "as"
                    Result = 28
                Case "as-as"
                    Result = 28
                Case "az"
                    Result = 36
                Case "az-az"
                    Result = 36
                Case "be"
                    Result = 47
                Case "be-be"
                    Result = 47
                Case "bg"
                    Result = 65
                Case "bg-bg"
                    Result = 65
                Case "bn"
                    Result = 49
                Case "bn-bn"
                    Result = 49
                Case "ca"
                    Result = 71
                Case "ca-ca"
                    Result = 71
                Case "cs"
                    Result = 75
                Case "cs-cs"
                    Result = 75
                Case "da"
                    Result = 104
                Case "da-da"
                    Result = 104
                Case "de"
                    Result = 2
                Case "de-at"
                    Result = 512
                Case "de-ch"
                    Result = 513
                Case "de-de"
                    Result = 558
                Case "de-li"
                    Result = 510
                Case "de-lu"
                    Result = 511
                Case "el"
                    Result = 164
                Case "el-el"
                    Result = 164
                Case "en"
                    Result = 1
                Case "en-au"
                    Result = 514
                Case "en-bz"
                    Result = 515
                Case "en-ca"
                    Result = 519
                Case "en-en"
                    Result = 1
                Case "en-gb"
                    Result = 516
                Case "en-ie"
                    Result = 517
                Case "en-jm"
                    Result = 518
                Case "en-nz"
                    Result = 521
                Case "en-ph"
                    Result = 522
                Case "en-tt"
                    Result = 524
                Case "en-us"
                    Result = 478
                Case "en-za"
                    Result = 523
                Case "en-zw"
                    Result = 525
                Case "es"
                    Result = 4
                Case "es-ar"
                    Result = 530
                Case "es-bo"
                    Result = 531
                Case "es-cl"
                    Result = 532
                Case "es-co"
                    Result = 540
                Case "es-cr"
                    Result = 533
                Case "es-do"
                    Result = 534
                Case "es-ec"
                    Result = 535
                Case "es-es"
                    Result = 560
                Case "es-gt"
                    Result = 537
                Case "es-hn"
                    Result = 538
                Case "es-mx"
                    Result = 541
                Case "es-ni"
                    Result = 542
                Case "es-pa"
                    Result = 543
                Case "es-pe"
                    Result = 545
                Case "es-pr"
                    Result = 546
                Case "es-py"
                    Result = 544
                Case "es-sv"
                    Result = 536
                Case "es-us"
                    Result = 549
                Case "es-uy"
                    Result = 548
                Case "es-ve"
                    Result = 550
                Case "et"
                    Result = 127
                Case "et-et"
                    Result = 127
                Case "eu"
                    Result = 43
                Case "eu-eu"
                    Result = 43
                Case "fa"
                    Result = 555
                Case "fa-fa"
                    Result = 555
                Case "fi"
                    Result = 136
                Case "fi-fi"
                    Result = 136
                Case "fo"
                    Result = 554
                Case "fo-fo"
                    Result = 554
                Case "fr"
                    Result = 3
                Case "fr-be"
                    Result = 500
                Case "fr-ca"
                    Result = 501
                Case "fr-ch"
                    Result = 504
                Case "fr-fr"
                    Result = 559
                Case "fr-lu"
                    Result = 502
                Case "fr-mc"
                    Result = 503
                Case "gd"
                    Result = 153
                Case "gd-gd"
                    Result = 153
                Case "gl"
                    Result = 556
                Case "gl-gl"
                    Result = 556
                Case "gu"
                    Result = 166
                Case "gu-gu"
                    Result = 166
                Case "he"
                    Result = 171
                Case "he-he"
                    Result = 171
                Case "hi"
                    Result = 175
                Case "hi-hi"
                    Result = 175
                Case "hr"
                    Result = 179
                Case "hr-hr"
                    Result = 179
                Case "hu"
                    Result = 180
                Case "hu-hu"
                    Result = 180
                Case "hy"
                    Result = 23
                Case "hy-hy"
                    Result = 23
                Case "id"
                    Result = 194
                Case "id-id"
                    Result = 194
                Case "is"
                    Result = 185
                Case "is-is"
                    Result = 185
                Case "it"
                    Result = 200
                Case "it-ch"
                    Result = 505
                Case "it-it"
                    Result = 200
                Case "ja"
                    Result = 202
                Case "ja-ja"
                    Result = 202
                Case "ka"
                    Result = 551
                Case "ka-ka"
                    Result = 551
                Case "kk"
                    Result = 216
                Case "kk-kk"
                    Result = 216
                Case "kn"
                    Result = 210
                Case "kn-kn"
                    Result = 210
                Case "ko"
                    Result = 228
                Case "kok"
                    Result = 225
                Case "ko-ko"
                    Result = 228
                Case "kz"
                    Result = 223
                Case "kz-kz"
                    Result = 223
                Case "lt"
                    Result = 246
                Case "lt-lt"
                    Result = 246
                Case "lv"
                    Result = 242
                Case "lv-lv"
                    Result = 242
                Case "mk"
                    Result = 277
                Case "mk-mk"
                    Result = 277
                Case "ml"
                    Result = 263
                Case "ml-ml"
                    Result = 263
                Case "mn"
                    Result = 286
                Case "mn-mn"
                    Result = 286
                Case "mr"
                    Result = 268
                Case "mr-mr"
                    Result = 268
                Case "ms"
                    Result = 506
                Case "ms-ms"
                    Result = 506
                Case "mt"
                    Result = 280
                Case "mt-mt"
                    Result = 280
                Case "nb-no"
                    Result = 315
                Case "ne"
                    Result = 306
                Case "ne-ne"
                    Result = 306
                Case "nl"
                    Result = 311
                Case "nl-be"
                    Result = 552
                Case "nl-nl"
                    Result = 311
                Case "nn-no"
                    Result = 314
                Case "no"
                    Result = 313
                Case "no-no"
                    Result = 313
                Case "or"
                    Result = 325
                Case "or-or"
                    Result = 325
                Case "pa"
                    Result = 335
                Case "pa-pa"
                    Result = 335
                Case "pl"
                    Result = 343
                Case "pl-pl"
                    Result = 343
                Case "pt"
                    Result = 345
                Case "pt-br"
                    Result = 508
                Case "pt-pt"
                    Result = 345
                Case "rm"
                    Result = 509
                Case "rm-rm"
                    Result = 509
                Case "ro"
                    Result = 357
                Case "ro-md"
                    Result = 526
                Case "ro-ro"
                    Result = 357
                Case "ru"
                    Result = 359
                Case "ru-md"
                    Result = 527
                Case "ru-ru"
                    Result = 359
                Case "sa"
                    Result = 367
                Case "sa-sa"
                    Result = 367
                Case "sb"
                    Result = 402
                Case "sb-sb"
                    Result = 402
                Case "sk"
                    Result = 383
                Case "sk-sk"
                    Result = 383
                Case "sl"
                    Result = 384
                Case "sl-sl"
                    Result = 384
                Case "sq"
                    Result = 400
                Case "sq-sq"
                    Result = 400
                Case "sr"
                    Result = 529
                Case "sr-sr"
                    Result = 529
                Case "sv"
                    Result = 411
                Case "sv-fi"
                    Result = 528
                Case "sv-sv"
                    Result = 411
                Case "sw"
                    Result = 410
                Case "sw-sw"
                    Result = 410
                Case "sx"
                    Result = 557
                Case "sx-sx"
                    Result = 557
                Case "syr"
                    Result = 412
                Case "ta"
                    Result = 415
                Case "ta-ta"
                    Result = 415
                Case "te"
                    Result = 417
                Case "te-te"
                    Result = 417
                Case "th"
                    Result = 423
                Case "th-th"
                    Result = 423
                Case "tn"
                    Result = 435
                Case "tn-tn"
                    Result = 435
                Case "tr"
                    Result = 440
                Case "tr-tr"
                    Result = 440
                Case "ts"
                    Result = 436
                Case "ts-ts"
                    Result = 436
                Case "tt"
                    Result = 416
                Case "tt-tt"
                    Result = 416
                Case "uk"
                    Result = 447
                Case "uk-uk"
                    Result = 447
                Case "ur"
                    Result = 450
                Case "ur-ur"
                    Result = 450
                Case "uz"
                    Result = 451
                Case "uz-uz"
                    Result = 451
                Case "vi"
                    Result = 454
                Case "vi-vi"
                    Result = 454
                Case "xh"
                    Result = 465
                Case "xh-xh"
                    Result = 465
                Case "yi"
                    Result = 468
                Case "yi-yi"
                    Result = 468
                Case "zh"
                    Result = 80
                Case "zh-cn"
                    Result = 499
                Case "zh-hk"
                    Result = 495
                Case "zh-mo"
                    Result = 496
                Case "zh-sg"
                    Result = 497
                Case "zh-tw"
                    Result = 498
                Case "zh-zh"
                    Result = 80
                Case "zu"
                    Result = 476
                Case "zu-zu"
                    Result = 476
            End Select

            Return Result

        End Function

        Private CurrentlyLoadedLanguagesStrings As Integer
        Public Overridable Sub LoadLanguageStrings(IDLanguage As Integer)

            If CurrentlyLoadedLanguagesStrings <> Nothing AndAlso CurrentlyLoadedLanguagesStrings = IDLanguage Then
                'Language data is already loaded
                Exit Sub
            End If

            'Try to get a supported, alternative LanguageID if IDLanguage isn't supported directly
            Dim MyIDLanguage As Integer
            If IsSupportedLanguage(IDLanguage) Then
                MyIDLanguage = IDLanguage
            Else
                MyIDLanguage = GetAlternativelySupportedLanguageID(IDLanguage)
            End If

            Select Case MyIDLanguage
                Case 359
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("ru-RU")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("ru-RU")
                    UpdateProfile_Descr_Title = "Изменить профиль пользователя"
                    UpdateProfile_Descr_PositionInCompany = "Должность"
                    UpdateProfile_Descr_Phone = "Телефон"
                    UpdateProfile_Descr_Fax = "Факс"
                    UpdateProfile_Descr_Mobile = "Сотовый"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Для продолжения введите значения во все нужные поля!"
                    UpdateProfile_ErrMsg_MistypedPW = "Неверный пароль. Соблюдайте написание заглавных и строчных букв!"
                    UpdateProfile_ErrMsg_Undefined = "Неожиданное возвращённое значение! - Обратитесь к нашему Веб-мастеру!"
                    UpdateProfile_ErrMsg_Success = "Ваш профиль пользователя успешно изменён!"
                    UpdateProfile_ErrMsg_LogonTooOften = "Процесс регистрации слишком часто заканчивался неудачей; учётная запись пользователя временно деактивирована.<br>Повторите попытку через некоторое время!"
                    UpdateProfile_ErrMsg_NotAllowed = "У вас недостаточные права доступа для выполнения этого действия!"
                    UpdateProfile_ErrMsg_PWRequired = "Введите также пароль, чтобы обновить профиль!"
                    UpdateProfile_Descr_Address = "Адрес"
                    UpdateProfile_Descr_Company = "Фирма"
                    UpdateProfile_Descr_Addresses = "Обращение"
                    UpdateProfile_Descr_PleaseSelect = "(Выберите!)"
                    UpdateProfile_Abbrev_Mister = "Г-на"
                    UpdateProfile_Abbrev_Miss = "Г-жа"
                    UpdateProfile_Descr_AcademicTitle = "Учёное звание (например, ""Д-р"")"
                    UpdateProfile_Descr_FirstName = "Имя"
                    UpdateProfile_Descr_LastName = "Фамилия"
                    UpdateProfile_Descr_NameAddition = "Доп. имя"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_UserDetails = "Данные пользователя"
                    UpdateProfile_Descr_1stLanguage = "1-й язык"
                    UpdateProfile_Descr_2ndLanguage = "2-й язык"
                    UpdateProfile_Descr_3rdLanguage = "3-й язык"
                    UpdateProfile_Descr_Authentification = "Подтвердите изменения вводом пароля"
                    UpdateProfile_Descr_Password = "Пароль"
                    UpdateProfile_Descr_Submit = "Обновить профиль"
                    UpdateProfile_Descr_RequiredFields = "* обязательные поля"
                    UpdateProfile_Descr_CustomerSupplierData = "Данные заказчика или поставщика"
                    UpdateProfile_Descr_CustomerNo = "№ заказчика"
                    UpdateProfile_Descr_SupplierNo = "№ поставщика"
                    UserJustCreated_Descr_AccountCreated = "Ваша учётная запись успешно создана!"
                    UserJustCreated_Descr_LookAroundNow = "Теперь вы можете продолжить и познакомиться с окружением."
                    UserJustCreated_Descr_PleaseNote = "Помните: В данный момент вы являетесь <font color=""#336699"">членом общего раздела</font>. Членство в дополнительных группах и дополнительные права доступа вы получите через 3 - 4 рабочих дня."
                    UserJustCreated_Descr_Title = "Добро пожаловать!"
                    UpdatePW_Descr_Title = "Сброс пароля"
                    UpdatePW_ErrMsg_ConfirmationFailed = "Подтверждение пароля не соответствует новому паролю. Пароль и подтверждение пароля должны совпадать. Помните также, что при вводе пароля заглавные и строчные буквы различаются."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Введите старый и новый пароль. Помните также, что при вводе пароля заглавные и строчные буквы различаются."
                    UpdatePW_ErrMsg_Undefined = "Обнаружена неизвестная ошибка!"
                    UpdatePW_ErrMsg_Success = "Пароль успешно изменён!"
                    UpdatePW_ErrMsg_WrongOldPW = "Не удалось изменить пароль. Проверьте правильность ввода текущего пароля."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Введите данные во все обязательные поля, чтобы завершить процедуру изменения пароля!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Введите свой текущий и новый пароль:"
                    UpdatePW_Descr_CurrentPW = "Текущий пароль"
                    UpdatePW_Descr_NewPW = "Новый пароль"
                    UpdatePW_Descr_NewPWConfirm = "Повторить новый пароль"
                    UpdatePW_Descr_Submit = "Сохранить изменения"
                    UpdatePW_Descr_RequiredFields = "* обязательные поля"
                    CreateAccount_Descr_CustomerSupplierData = "Данные заказчика или поставщика"
                    CreateAccount_Descr_CustomerNo = "№ заказчика"
                    CreateAccount_Descr_SupplierNo = "№ поставщика"
                    CreateAccount_Descr_FollowingError = "Возникла следующая ошибка:"
                    CreateAccount_Descr_LoginDenied = "В доступе отказано!"
                    CreateAccount_Descr_Submit = "Создать учётную запись"
                    CreateAccount_Descr_RequiredFields = "обязательные поля"
                    CreateAccount_Descr_BackToLogin = "Обратно в окно регистрации"
                    CreateAccount_Descr_PageTitle = "Создать новую учётную запись"
                    CreateAccount_Descr_UserLogin = "Регистрационные данные"
                    CreateAccount_Descr_NewLoginName = "Ваше новое имя пользователя"
                    CreateAccount_Descr_NewLoginPassword = "Ваш новый пароль"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Подтверждение пароля"
                    CreateAccount_Descr_Address = "Данные адреса"
                    CreateAccount_Descr_Company = "Фирма"
                    CreateAccount_Descr_Addresses = "Обращение"
                    CreateAccount_Descr_PleaseSelect = "(Выберите!)"
                    CreateAccount_Descr_AcademicTitle = "Учёное звание (например, ""Д-р"")"
                    CreateAccount_Descr_FirstName = "Имя"
                    CreateAccount_Descr_LastName = "Фамилия"
                    CreateAccount_Descr_NameAddition = "Доп. имя"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Улица"
                    CreateAccount_Descr_ZIPCode = "Индекс"
                    CreateAccount_Descr_Location = "Город"
                    CreateAccount_Descr_State = "Область"
                    CreateAccount_Descr_Country = "Страна"
                    CreateAccount_Descr_Motivation = "Каков ваш мотив для регистрации"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Посетитель Веб-сайта"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Посетитель Веб-сайта"
                    CreateAccount_Descr_MotivItemDealer = "Дилер"
                    UpdateProfile_Descr_MotivItemDealer = "Дилер"
                    CreateAccount_Descr_MotivItemJournalist = "Журналист"
                    UpdateProfile_Descr_MotivItemJournalist = "Журналист"
                    CreateAccount_Descr_MotivItemOther = "Прочее, укажите"
                    UpdateProfile_Descr_MotivItemOther = "Прочее, укажите"
                    CreateAccount_Descr_WhereHeard = "Где вы узнали о Secured Area"
                    CreateAccount_Descr_WhereItemFriend = "Рекомендация друга"
                    UpdateProfile_Descr_WhereItemFriend = "Рекомендация друга"
                    CreateAccount_Descr_WhereItemResellerDealer = "Торговец/Дилер"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Торговец/Дилер"
                    CreateAccount_Descr_WhereItemExhibition = "Выставка/Ярмарка"
                    UpdateProfile_Descr_WhereItemExhibition = "Выставка/Ярмарка"
                    CreateAccount_Descr_WhereItemMagazines = "Журнал"
                    UpdateProfile_Descr_WhereItemMagazines = "Журнал"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "Рекомендация сотрудника"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "Рекомендация сотрудника"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Поисковый сервер, укажите"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Поисковый сервер, укажите"
                    CreateAccount_Descr_WhereItemOther = "Прочее, укажите"
                    UpdateProfile_Descr_WhereItemOther = "Прочее, укажите"
                    CreateAccount_Descr_UserDetails = "Данные пользователя"
                    CreateAccount_Descr_Comments = "Комментарии"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Запросы дополнительных прав доступа"
                    CreateAccount_Descr_1stPreferredLanguage = "1-й язык"
                    CreateAccount_Descr_2ndPreferredLanguage = "2-й язык"
                    CreateAccount_Descr_3rdPreferredLanguage = "3-й язык"
                    CreateAccount_ErrorJS_InputValue = "Введите значение в поле \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Введите значение в поле \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Введите значение длиной не менее [n:0] знаков в поле \""[n:1]\""."
                    UpdateProfile_ErrorJS_Length = "Введите значение длиной не менее [n:0] знаков в поле \""[n:1]\""."
                    Banner_Help = "Справка"
                    Banner_HeadTitle = "Регистрация в Secured Area"
                    Banner_BodyTitle = "Регистрация в " & OfficialServerGroup_Title
                    Banner_Feedback = "Обратная связь"
                    Logon_HeadTitle = "Регистрация в Secured Area"
                    Logon_AskForForcingLogon = "Внимание! Один сеанс вашей работы уже зарегистрирован. Вы хотите прервать его и начать новый?"
                    Logon_BodyTitle = "Регистрация в" & OfficialServerGroup_Title
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Вы идентифицированы как пользователь <strong>{0} ({1})</strong>."
                    Logon_SSO_ADS_LabelRegisterNewEMail = "Адрес e-mail:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Пароль (повторить):"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Пароль:"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Регистрационное имя:"
                    Logon_SSO_ADS_ButtonNext = "Готово"
                    Logon_SSO_ADS_ContactUs = "Со всеми вопросами <a href=""mailto:{0}"">обращайтесь к нам</a>."
                    Logon_SSO_ADS_RadioDoNothing = "Если идентификация неверна или если вы хотите продолжить без регистрации в системе, продолжите работу как анонимный пользователь (это диалоговое окно будет снова предложено вам позже)."
                    Logon_SSO_ADS_RadioRegisterNew = "Зарегистрируйтесь под <strong>новой</strong> учётной записью"
                    Logon_SSO_ADS_RadioRegisterExisting = "Зарегистрируйтесь под одной из <strong>уже существующих</strong> учётных записей"
                    Logon_SSO_ADS_LabelTakeAnAction = "Что вы хотите сделать?"
                    Logon_SSO_ADS_IdentifiedUserName = "Вы идентифицированы как пользователь <strong>{0}</strong>."
                    Logon_SSO_ADS_PageTitle = "Создание автоматического входа в систему"
                    Logon_BodyPrompt2User = "Введите своё имя пользователя и соответствующий пароль, чтобы войти в " & OfficialServerGroup_Title & ".<br><em>Помните также, что имя пользователя и пароль могут отличаться от других данных доступа, полученных вами для других разделов.</em>"
                    Logon_BodyFormUserName = "Имя пользователя"
                    Logon_BodyFormUserPassword = "Пароль"
                    Logon_BodyFormSubmit = "Логин"
                    Logon_BodyFormCreateNewAccount = "Создать учётную запись"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Вы ещё не стали членом? Создайте свою собственную учётную запись для раздела " & OfficialServerGroup_Title & "!</STRONG><BR>" & _
                                    "Если у Вас ещё нет данных доступа, Вы можете создать прямо сейчас </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>" & _
                                    "</FONT></A><FONT face=Arial size=2>. " & _
                                    "Не создавайте других&nbsp;данных доступа, " & _
                                    "если Вы уже делали это в прошлом. При возникновении трудностей при регистрации обратитесь в наш <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>Support Service Center</FONT></A> " & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Забыли пароль? Мы отправим Вам пароль " & _
                                    "</B><BR>Вы уже получили рабочие данные доступа, не потеряли пароль?" & _
                                    "</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>Здесь Вы получите свой пароль по электронной почте </FONT></A><FONT " & _
                                    "face=Arial size=2>. Помните, что сообщение отправляется только по адресу, первоначально указанному Вами" & _
                                    ".<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Вы не получили ответа на свой вопрос?</strong><br>Если Вам требуется дополнительная поддержка, обратитесь к нам в </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2></FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    Logon_Connecting_InProgress = "Выполняется соединение с сервером…"
                    Logon_Connecting_RecommendationOnTimeout = "Если эта проблема снова возникнет, обращайтесь к нам."
                    Logon_Connecting_LoginTimeout = "Превышение времени при входе в систему."
                    AccessError_Descr_FollowingError = "Возникла следующая ошибка:"
                    AccessError_Descr_LoginDenied = "В регистрации было отказано!"
                    AccessError_Descr_BackToLogin = "Обратно в окно регистрации"
                    SendPassword_Descr_FollowingError = "Возникла следующая ошибка:"
                    SendPassword_Descr_LoginDenied = "В регистрации было отказано!"
                    SendPassword_Descr_Title = "Запрос пароля Secured Area по электронной почте"
                    SendPassword_Descr_LoginName = "Имя пользователя"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Отправить e-mail"
                    SendPassword_Descr_RequiredFields = "обязательные поля"
                    SendPassword_Descr_BackToLogin = "Назад в окно регистрации"
                    SendPassword_Descr_PasswordSentTo = "Пароль был отправлен по адресу {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "Ваш пароль Secured Area отправляется на ваш сохранённый адрес электронной почты.<BR>Если Вы не получите этого электронного сообщения в ближайшие 24 часа, обратитесь по адресу <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "RU"
                    StatusLineUsername = "Пользователь"
                    StatusLinePassword = "Пароль"
                    StatusLineSubmit = "Логин"
                    StatusLineEditorial = "О нас"
                    StatusLineContactUs = "Контакты"
                    StatusLineDataprotection = "Защита данных"
                    StatusLineLoggedInAs = "Зарегистрирован(а) как"
                    StatusLineLegalNote = "Защита данных и правовые рекомендации"
                    StatusLineCopyright_AllRightsReserved = "Все права защищена."
                    NavAreaNameYourProfile = "Ваш профиль"
                    NavLinkNameUpdatePasswort = "Изменить пароль"
                    NavLinkNameUpdateProfile = "Изменить данные пользователя"
                    NavLinkNameLogout = "Отмена регистрации"
                    NavLinkNameLogin = "Логин"
                    NavPointUpdatedHint = "Здесь создано что-то новое или обновлено что-то существующее"
                    NavPointTemporaryHiddenHint = "Это прриложение временно деактивировано для других пользователей. Зачастую это говорит о том, что это приложение пока находится на стадии разработки."
                    SystemButtonYes = "Да"
                    SystemButtonNo = "Нет"
                    SystemButtonOkay = "ОК"
                    SystemButtonCancel = "Отмена"
                    ErrorUserOrPasswordWrong = "Имя пользователя или пароль неверны или неверно введены, либо в доступе отказано!<p>Проверьте <ul><li>написание имени пользователя и пароля (заглавные и строчные буквы в пароле различаются!),</li><li>чтобы использовать правильную комбинацию имени пользователя и пароля. (Возможно, Вы уже получили от нас другие имена пользователя/пароли, которые недействительны для данного раздела.)</li></ul>"
                    ErrorServerConfigurationError = "Этот сервер неверно установлен. Проконсультируйтесь со своим администратором."
                    ErrorNoAuthorization = "У вас нет права доступа к данному разделу."
                    ErrorAlreadyLoggedOn = "Вы уже зарегистрированы! Сначала отмените свою регистрацию на другом своём рабочем месте!<br><font color=""red"">Если Вы уверены, что Вы нигде более не зарегистрированы, отправьте нам короткое сообщение по адресу <a href=""mailto:[n:0]"">[n:1]</a> и назовите свою учётную запись.</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Ваша регистрация на этом рабочем месте отменена, так как Вы зарегистрировались на другой станции.<br>"
                    ErrorLogonFailedTooOften = "Процесс регистрации слишком часто заканчивался неудачей, Ваша учётная запись временно деактивирована.<br>Повторите попытку немного позже!"
                    ErrorEmptyPassword = "Не забудьте указать ещё один пароль!<br>Если Вы забыли свой пароль, Вы можете снова запросить его по электронной почте. Для этого отправьте дополнительные подробности ниже в тексте."
                    ErrorUnknown = "Неожиданная ошибка! - Обратитесь в наш <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Введите значения во все поля, помеченные звёздочкой <em>(*)</em>!"
                    ErrorWrongNetwork = "У вас нет прав на регистрацию по текущему сетевому соединению."
                    ErrorUserAlreadyExists = "Учётная запись уже существует. Выберите другое имя!"
                    ErrorLoginCreatedSuccessfully = "Профиль пользователя успешно создан!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Неверное имя пользователя или неверный адрес электронной почты.<br>Введите правильные значения, внесённые ранее в Ваш профиль пользователя."
                    ErrorCookiesMustNotBeDisabled = "Ваш браузер не поддерживает Cookies, либо Cookies были деактивированы в настройках безопасности Вашего браузера."
                    ErrorTimoutOrLoginFromAnotherStation = "Ваша регистрация была отменена, так как была достигнута максимальная продолжительность сеанса, либо была выполнена регистрация с другой рабочей станции."
                    ErrorApplicationConfigurationIsEmpty = "Это приложение не содержит действительных имён приложений. Обратитесь к производителю."
                    InfoUserLoggedOutSuccessfully = "Отмена Вашей регистрации успешно выполнена. Спасибо за посещение."
                    UserManagementEMailColumnTitleLogin = "Регистрационное имя: "
                    UserManagementEMailColumnTitleCompany = "Фирма: "
                    UserManagementEMailColumnTitleName = "Имя: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Улица: "
                    UserManagementEMailColumnTitleZIPCode = "Индекс: "
                    UserManagementEMailColumnTitleLocation = "Город: "
                    UserManagementEMailColumnTitleState = "Область: "
                    UserManagementEMailColumnTitleCountry = "Страна: "
                    UserManagementEMailColumnTitle1stLanguage = "1-й язык: "
                    UserManagementEMailColumnTitle2ndLanguage = "2-й язык: "
                    UserManagementEMailColumnTitle3rdLanguage = "3-й язык: "
                    UserManagementEMailColumnTitleComesFrom = "От: "
                    UserManagementEMailColumnTitleMotivation = "Мотивания: "
                    UserManagementEMailColumnTitleCustomerNo = "№ заказчика: "
                    UserManagementEMailColumnTitleSupplierNo = "№ поставщика.: "
                    UserManagementEMailColumnTitleComment = "Комментарий: "
                    UserManagementAddressesMr = "Г-н "
                    UserManagementAddressesMs = "Г-жа "
                    UserManagementSalutationUnformalMasculin = "Здравствуйте "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Дамы и господа, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Уважаемый г-н "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Привет, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Здравствуйте "
                    UserManagementSalutationFormulaUnformalGroup = "Привет, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Уважаемая г-жа "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagementEMailTextRegards = "С уважением,"
                    UserManagementEMailTextSubject = "Ваши данные доступа к Secured Area"
                    UserManagementEMailTextSubject4AdminNewUser = "Secured Area - Новый пользователь"
                    UserManagementMasterServerAvailableInNearFuture = "Внимание: Этот сервер будет доступен только в ближайшем будущем."
                    CreateAccount_MsgEMailWelcome_WithPassword = "Добро пожаловать в Secured Area! Здесь Вы найдёте ежедневную местную информацию." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваше регистрационное имя для Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Ваш пароль: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Не откладывая, сохраните пароль в надёжном месте. Это необходимо, чтобы никто другой (например, хакер) не имел Вашего пароля и не мог им воспользоваться!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Воспользуйтесь преимуществами комплексной поддержки! Вас ожидает многое. Просто познакомьтесь с этим!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваша учётная запись открывает Вам доступ к различным защищённым приложениям. Процесс регистрации при входе в Secured Area уже завершён. Однако Ваши права доступа в полном объёме будут действовать лишь через 3 - 4 рабочих дня. Войти в Secured Area Вы можете по следующему URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMailWelcome = "Добро пожаловать в Secured Area! Здесь Вы найдёте ежедневную местную информацию." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваше регистрационное имя для Secured Area: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Воспользуйтесь преимуществами комплексной поддержки! Вас ожидает многое. Просто познакомьтесь с этим!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваша учётная запись открывает Вам доступ к различным защищённым приложениям. Процесс регистрации при входе в Secured Area уже завершён. Однако Ваши права доступа в полном объёме будут действовать лишь через 3 - 4 рабочих дня. Войти в Secured Area Вы можете по следующему URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMail4Admin = "Следующий пользователь вновь зарегистрировался в разделе Secured Area." & ChrW(13) & ChrW(10) & _
                "Предоставьте необходимые права доступа!" & ChrW(13) & ChrW(10) & _
                "Для настройки прав доступа посетите " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextWelcome = "Добро пожаловать в Secured Area! Здесь Вы найдёте ежедневную местную информацию." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "От нашего администратора для Вас создана учётная запись для раздела Secured Area. Этот сервис, разумеется, является для Вас бесплатным." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваше регистрационное имя для Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Ваш пароль для Secured Area: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Не откладывая, сохраните пароль в надёжном месте. Это необходимо, чтобы никто другой (например, хакер) не имел Вашего пароля и не мог им воспользоваться!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Воспользуйтесь преимуществами комплексной поддержки! Вас ожидает многое. Просто познакомьтесь с этим!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваша учётная запись открывает Вам доступ к различным защищённым приложениям. Процесс регистрации при входе в Secured Area уже завершён. Однако Ваши права доступа в полном объёме будут действовать лишь через 3 - 4 рабочих дня. Войти в Secured Area Вы можете по следующему URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Добро пожаловать в Secured Area! Здесь Вы найдёте ежедневную местную информацию.</p>" & _
                "<p>От нашего администратора для Вас создана учётная запись для раздела Secured Area. Этот сервис, разумеется, является для Вас бесплатным.</p>" & _
                "<p><strong>Ваше регистрационное имя для Secured Area: <font color=""red"">[n:0]</font><br>" & _
                "Ваш пароль для Secured Area: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Не откладывая, сохраните пароль в надёжном месте. Это необходимо, чтобы никто другой (например, хакер) не имел Вашего пароля и не мог им воспользоваться!</p>" & _
                "<p>Воспользуйтесь преимуществами комплексной поддержки! Вас ожидает многое. Просто познакомьтесь с этим!" & _
                "<p>Ваша учётная запись открывает Вам доступ к различным защищённым приложениям. Процесс регистрации при входе в Secured Area уже завершён. Однако Ваши права доступа в полном объёме будут действовать лишь через 3 - 4 рабочих дня. Войти в Secured Area Вы можете по следующему URL:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    UserManagement_NewUser_MsgEMail4Admin = "Следующий пользователь создан Вами или одним из Ваших коллег в разделе Secured Area." & ChrW(13) & ChrW(10) & _
                "Предоставьте необходимые права доступа!" & ChrW(13) & ChrW(10) & _
                "Для настройки прав доступа посетите " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Добро пожаловать в Secured Area!"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Добро пожаловать в Secured Area! Здесь Вы найдёте ежедневную местную информацию." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваши права доступа для пользователя ""[n:0]"" переданы. Войти в Secured Area Вы можете по следующему URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Мы будем рады снова увидеть Вас здесь."
                    SendPassword_EMailMessage = "Ниже приведены данные Вашего профиля пользователя. Сохраните этот адрес электронной почты с именем пользователя и/или паролем и держите эту информацию в секрете." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваш пароль для Secured Area: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Не откладывая, сохраните пароль в надёжном месте. Это необходимо, чтобы никто другой (например, хакер) не имел Вашего пароля и не мог им воспользоваться!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Ваша учётная запись открывает Вам доступ к различным защищённым приложениям. Процесс регистрации при входе в Secured Area уже завершён. Войти в Secured Area Вы можете по следующему URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "Ответственный за безопасность сбросил настройки Вашей учётной записи. Ниже приведены новые данные Вашего профиля пользователя. Сохраните этот адрес электронной почты с именем пользователя и/или паролем и держите эту информацию в секрете." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ваш пароль для Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Не откладывая, сохраните пароль в надёжном месте. Это необходимо, чтобы никто другой (например, хакер) не имел Вашего пароля и не мог им воспользоваться!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Ваша учётная запись открывает Вам доступ к различным защищённым приложениям. Процесс регистрации при входе в Secured Area уже завершён. Войти в Secured Area Вы можете по следующему URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Воспользуйтесь преимуществами комплексной поддержки!"
                    HighlightTextTechnicalSupport = "Мы сейчас начнём, и многое изменится."
                    HighlightTextExtro = "Вас многое ожидает. Воспользуйтесь этой возможностью!"
                    WelcomeTextWelcomeMessage = "Добро пожаловать в Secured Area!"
                    WelcomeTextFeedbackToContact = "Вам нужны дополнительные функции? Нет проблем! Отправьте нам свои комментарии по сети Extranet по адресу <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "Здесь Вы найдёте ежедневную местную информацию."
                    UpdateProfile_Descr_Street = "Улица"
                    UpdateProfile_Descr_ZIPCode = "Индекс"
                    UpdateProfile_Descr_Location = "Город"
                    UpdateProfile_Descr_State = "Область"
                    UpdateProfile_Descr_Country = "Страна"
                    UpdatePW_Error_PasswordComplexityPolicy = "Пароль должен содержать не менее 3 символов. Его не следует составлять из Вашего имени или фамилии."
                    ErrorRequiredField = "Обязательное поле"
                    UserManagementEMailTextDearUndefinedGender = "Уважаемый "
                    UserManagementSalutationUnformalUndefinedGender = "Привет "
                    NavAreaNameLogin = "Регистрация"
                    NavLinkNamePasswordRecovery = "Забыли пароль?"
                    NavLinkNameNewUser = "Создать новую учётную запись"
                Case 345
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("pt-PT")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("pt-PT")
                    Logon_AskForForcingLogon = "Atenção: Voçê já tem iniciado uma outra sessão.Voçê quer cancelar esta sessão e iniciar uma outra?"
                    UserManagementSalutationUnformalMasculin = "Olá "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Senhoras e senhores, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Estimado Sr. "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Oi, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Olá "
                    UserManagementSalutationFormulaUnformalGroup = "Oi, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Estimada Sra. "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagement_NewUser_TextWelcome = "Bem-vindo à nossa Área Protegida. O lugar a visitar todos dias!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "As suas autorizaões para o utilizador foram criadas." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua nome do utilizador é: [n:0]    " & ChrW(13) & ChrW(10) & _
                "A sua senha para a Área Protegida é: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Por favor, não se esqueçaa de alterar a sua senha o quanto antes, para evitar o acesso e utilização por parte de terceiros!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Aproveite das vantagens de um suporte extensivo. Há muita informação a sua espera no nosso Extranet. Esteja à vontade para surfar neste site e para o explorar na totalidade." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
"A sua conta permite o acesso a diversos programas por agora, deverá já ter concluído o processo de associação da nossa Área Protegida. Autorizações adicionais e filiações serão definidas nos próximos 3 - 4 dias. De seguida, por favor, revisite a URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Bem-vindo à nossa Área Protegida! O lugar a visitar todos os dias!</p>" & _
                "<p>Voçê foi adicionado gratuitamente à Área Protegida pela nossa administração.</p>" & _
                "<p><strong>A sua nome do utilizador é: <font color=""red"">[n:0]</font><br>" & _
                "A sua senha para a Área Protegida é: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Por favor, não se esqueça de alterar a senha, o quanto antes, para evitar o acesso e utilização por parte de terceiros!</p>" & _
                "<p>Aproveite das vantagens de um suporte extensivo.Há muito informação na Extranet. Esteja à vontade para surfar neste site e para o explorar na totalidade.</p>" & _
                "<p>A sua conta permite o acesso a diversos programas protegidos. Por agora, deverá já ter concluído o processo de associação da nossa Área Protegida. Autorizações adicionais e filiações serão definidas nos próximos 3 - 4 dias De seguida, por favor, revisite a URL:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    UpdateProfile_Descr_PositionInCompany = "Cargo profissional"
                    UpdateProfile_Descr_Phone = "Telefone"
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Mobile = "Celular"
                    UpdateProfile_Descr_Title = "Alterar o perfile"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Por favor, introduzir valores em todos os campos obrigatórios para continuar!"
                    UpdateProfile_ErrMsg_MistypedPW = "Nome do utilizador ou da senha mal dactilografado ou mal soletrado ou acesso negado!"
                    UpdateProfile_ErrMsg_Undefined = "Valor de retorno inesperado! - Por favor, contacte o webmaster!"
                    UpdateProfile_ErrMsg_Success = "O seu perfil foi actualizado com sucesso!"
                    UpdateProfile_ErrMsg_LogonTooOften = "O processo de início de sessão falhou demasiadas vezes, a conta foi desactivada.<br>Por favor, tente de novo mais tarde!"
                    UpdateProfile_ErrMsg_NotAllowed = "Voçê não tem autorização para aceder a este documento!"
                    UpdateProfile_ErrMsg_PWRequired = "Por favor, introduza a sua senha para alterar o seu perfil!"
                    UpdateProfile_Descr_Address = "Endereço"
                    UpdateProfile_Descr_Company = "Empresa"
                    UpdateProfile_Descr_Addresses = "Saudação"
                    UpdateProfile_Descr_PleaseSelect = "(Seleccione!)"
                    UpdateProfile_Abbrev_Mister = "Sr."
                    UpdateProfile_Abbrev_Miss = "Sra."
                    UpdateProfile_Descr_AcademicTitle = "Título acadêmico (p. ex. ""Dr."")"
                    UpdateProfile_Descr_FirstName = "Nome"
                    UpdateProfile_Descr_LastName = "Sobrenome"
                    UpdateProfile_Descr_NameAddition = "Adicionar nome"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_Street = "Rua"
                    UpdateProfile_Descr_ZIPCode = "Código postal"
                    UpdateProfile_Descr_Location = "Cidade"
                    UpdateProfile_Descr_State = "Estado"
                    UpdateProfile_Descr_Country = "País"
                    UpdateProfile_Descr_UserDetails = "Detalhes do utilizador"
                    UpdateProfile_Descr_1stLanguage = "Primeira lingua preferida"
                    UpdateProfile_Descr_2ndLanguage = "Segunda lingua preferida"
                    UpdateProfile_Descr_3rdLanguage = "Terceira lingua preferida"
                    UpdateProfile_Descr_Authentification = "Confirme as alterações com a sua senha"
                    UpdateProfile_Descr_Password = "Senha"
                    UpdateProfile_Descr_Submit = "Atualizar o perfil"
                    UpdateProfile_Descr_RequiredFields = "* campos obrigatórios"
                    UpdateProfile_Descr_CustomerSupplierData = "Dados do cliente/fornecedor"
                    UpdateProfile_Descr_CustomerNo = "No. do cliente"
                    UpdateProfile_Descr_SupplierNo = "No. do fornecedor"
                    UserJustCreated_Descr_AccountCreated = "A sua conta de utilizador foi criada com sucesso!"
                    UserJustCreated_Descr_LookAroundNow = "Pode, agora, prosseguir e explorar."
                    UserJustCreated_Descr_PleaseNote = "Por favor atente: Agora <font color=""#336699"">voçê é um membro público</font>.Autorizações adicionais e filiações serão definidas nos próximos 3 - 4 dias."
                    UserJustCreated_Descr_Title = "Área protegida – Bem-vindo!"
                    UpdatePW_Descr_Title = "Resetear a senha"
                    UpdatePW_ErrMsg_ConfirmationFailed = "A confirmação da senha falhou! Por favor, verifique se reescreveu a senha correctamente. Por favor, preste igualmente atenção às letras maiùsculas e minùsculas."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Por favor, digite a sua senha atual e uma nova, diferente dessa. Por favor, preste igualmente atenção às letras maiúsculas e minúsculas."
                    UpdatePW_ErrMsg_Undefined = "Foi detectado um erro indefinido!"
                    UpdatePW_ErrMsg_Success = "A senha foi alterada com sucesso!"
                    UpdatePW_ErrMsg_WrongOldPW = "Não foi possível alterar a senha! Por favor, verifique se introduziou a senha correcta."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Por favor, introduza valores em todos os campos obrigatórios para concluir a alteração da senha!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Entre sua senha atual e em seguida a nova:"
                    UpdatePW_Descr_CurrentPW = "Senha atual"
                    UpdatePW_Descr_NewPW = "Nova senha"
                    UpdatePW_Descr_NewPWConfirm = "Confirme a nova senha"
                    UpdatePW_Descr_Submit = "Atualizar o perfil"
                    UpdatePW_Descr_RequiredFields = "* campos obrigatórios"
                    UpdatePW_Error_PasswordComplexityPolicy = "A senha deve ser composta, no mínimo, por 3 caracteres. Não pode utilizar elementos do seu nome."
                    CreateAccount_Descr_CustomerSupplierData = "Dados do cliente/fornecedor"
                    CreateAccount_Descr_CustomerNo = "Cliente No."
                    CreateAccount_Descr_SupplierNo = "No. do forncedor"
                    CreateAccount_Descr_FollowingError = "Ocorreu a seguinte falha:"
                    CreateAccount_Descr_LoginDenied = "O início de sessão foi negado!"
                    CreateAccount_Descr_Submit = "Criar conta de utente"
                    CreateAccount_Descr_RequiredFields = "campos obrigatórios"
                    CreateAccount_Descr_BackToLogin = "Voltar ao início de sessão"
                    CreateAccount_Descr_PageTitle = "Criar a nova conta de utente"
                    CreateAccount_Descr_UserLogin = "Início de sessão do utente"
                    CreateAccount_Descr_NewLoginName = "O seu novo nome de utente"
                    CreateAccount_Descr_NewLoginPassword = "A sua nova senha"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Confirme a sua senha"
                    CreateAccount_Descr_Address = "Endereço"
                    CreateAccount_Descr_Company = "Empresa"
                    CreateAccount_Descr_Addresses = "Saudação"
                    CreateAccount_Descr_PleaseSelect = "(Seleccione!)"
                    CreateAccount_Descr_AcademicTitle = "Título acadêmico (p. ex. ""Dr."")"
                    CreateAccount_Descr_FirstName = "Nome"
                    CreateAccount_Descr_LastName = "Sobrenome"
                    CreateAccount_Descr_NameAddition = "Adicionar nome"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Rua"
                    CreateAccount_Descr_ZIPCode = "Código postal"
                    CreateAccount_Descr_Location = "Cidade"
                    CreateAccount_Descr_State = "Estado"
                    CreateAccount_Descr_Country = "País"
                    CreateAccount_Descr_Motivation = "Qual é o motivo para o seu registo"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Visitante do site Web"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Visitante do site Web"
                    CreateAccount_Descr_MotivItemDealer = "Negociante"
                    UpdateProfile_Descr_MotivItemDealer = "Negociante"
                    CreateAccount_Descr_MotivItemJournalist = "Jornalista"
                    UpdateProfile_Descr_MotivItemJournalist = "Jornalista"
                    CreateAccount_Descr_MotivItemOther = "Outro, por favor especificar"
                    UpdateProfile_Descr_MotivItemOther = "Outro, por favor especificar"
                    CreateAccount_Descr_WhereHeard = "Onde obteve conhecimento sobre a nossa Área Protegida"
                    CreateAccount_Descr_WhereItemFriend = "Amigo"
                    UpdateProfile_Descr_WhereItemFriend = "Amigo"
                    CreateAccount_Descr_WhereItemResellerDealer = "Revendedor/negociante"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Revendedor/negociante"
                    CreateAccount_Descr_WhereItemExhibition = "Exibição"
                    UpdateProfile_Descr_WhereItemExhibition = "Exibição"
                    CreateAccount_Descr_WhereItemMagazines = "Jornais"
                    UpdateProfile_Descr_WhereItemMagazines = "Jornais"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "de nós"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "de nós"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Motor de busca, por favor especifique"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Motor de busca, por favor especifique"
                    CreateAccount_Descr_WhereItemOther = "Outro, por favor especificar"
                    UpdateProfile_Descr_WhereItemOther = "Outro, por favor especificar"
                    CreateAccount_Descr_UserDetails = "Detalhes do utente"
                    CreateAccount_Descr_Comments = "Comentarios"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Pedidos para autorizações adicionais"
                    CreateAccount_Descr_1stPreferredLanguage = "Primeira lingua preferida"
                    CreateAccount_Descr_2ndPreferredLanguage = "Segunda lingua preferida"
                    CreateAccount_Descr_3rdPreferredLanguage = "Terceira lingua preferida"
                    CreateAccount_ErrorJS_InputValue = "Por favor, inserira um valor no campo \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Por favor, inserira um valor no campo \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Por favor, inserira um valor no campo com pelo menos [n:0] caractéres no campo \""[n:1]\""."
                    UpdateProfile_ErrorJS_Length = "Por favor, inserira um valor no campo com pelo menos [n:0] caractéres no campo \""[n:1]\""."
                    Banner_Help = "Ajuda"
                    Banner_HeadTitle = "Área protegida – Início de sessão"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Início de sessão"
                    Banner_Feedback = "Feedback"
                    Logon_Connecting_RecommendationOnTimeout = "Se o problema ocorrer mais uma vez, por favor, <a href=""mailto:[0]"">contácte-nos</a>."
                    Logon_Connecting_LoginTimeout = "Limite de tempo de início de sessãu excedido."
                    Logon_HeadTitle = "Área protegida - Início de sessão"
                    Logon_Connecting_InProgress = "Voçê vai ser ligado com o servidor…"
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Início de sessão"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Foi identificado como utilizador <strong>{0} ({1})</strong>."
                    Logon_SSO_ADS_LabelRegisterNewEMail = "Endereço e-mail:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Digitar novamente a senha"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Senha:"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Nome do início da sessão:"
                    Logon_SSO_ADS_ButtonNext = "Continue"
                    Logon_SSO_ADS_ContactUs = "Se tiver mais perguntas, <a href=""mailto:{0}"">contácte-nos</a>."
                    Logon_SSO_ADS_RadioDoNothing = "Se a identificação estiver errada ou se pretender continuar sem inicar sessão, agora (perguntar de novo mais tarde)."
                    Logon_SSO_ADS_RadioRegisterNew = "Registe-se para uma <strong>new</strong> conta"
                    Logon_SSO_ADS_RadioRegisterExisting = "Registe-se para uma conta que já <strong>existe</strong>"
                    Logon_SSO_ADS_LabelTakeAnAction = "O que é que queria saber?"
                    Logon_SSO_ADS_IdentifiedUserName = "Foi identificado como utilizador <strong>{0}</strong>."
                    Logon_SSO_ADS_PageTitle = "Se a identificação estiver errada ou se pretender continuar sem inicar sessão, agora"
                    Logon_BodyPrompt2User = "Introduza por favor seu username e senha para aceder à " & OfficialServerGroup_Title & ".<br><em>Por favor, observe que esta nome do utilizador e senha online estão separadas e podem ser diferentes das que já criou para outras das nossas áreas.</em>"
                    Logon_BodyFormUserName = "Nome do utilizador"
                    Logon_BodyFormUserPassword = "Senha"
                    Logon_BodyFormSubmit = "Início da sessão"
                    Logon_BodyFormCreateNewAccount = "Criar uma nova conta"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Você ainda não é membro? Críe a sua conta agora para  entrar na " & OfficialServerGroup_Title & "</STRONG><BR>" & _
 "Clique aqui para </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>criar uma conta agora</FONT></A><FONT face=Arial size=2>. " & _
 "Atenção, se você já criou uma, não crie outra. Se tiver problemas ao iniciar a sessão, contacte a nossa " & _
 " <A " & _
 "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>central de atendimento</FONT></A>" & _
 ". <BR> &nbsp;</FONT></P></TD></TR>" & _
 "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Esqueçeu a sua senha? " & _
 "</B><BR>Você digitou uma conta válida mas esqueceu-se da senha? " & _
 "</FONT> <A " & _
 "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
 "face=Arial size=2>Clique aqui e a sua senha lhe será enviada por email.</FONT></A><FONT " & _
 "face=Arial size=2> Atenção, a senha lhe será enviada ao e-mail indicado inicialmente.<br> &nbsp;</FONT></P></TD></TR>" & _
 "<TR><TD VALIGN=""TOP""><A " & _
 "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Ainda está com problemas de acesso?</strong><br>Se precisar de mais assistência, </FONT><A " & _
 "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>contate a gente</FONT></A><FONT " & _
 "size=2>.</FONT></P></TD></TR></TABLE>"
                    AccessError_Descr_FollowingError = "Ocorreu a seguinte falha:"
                    AccessError_Descr_LoginDenied = "O início de sessão foi negado!"
                    AccessError_Descr_BackToLogin = "Voltar ao início de sessão"
                    SendPassword_Descr_FollowingError = "Ocorreu o seguinte erro:"
                    SendPassword_Descr_LoginDenied = "O início de sessão foi negado!"
                    SendPassword_Descr_Title = "Pedir a senha da Área Protegida por correio electrónico"
                    SendPassword_Descr_LoginName = "Nome do início da sessão"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Enviar e-mail"
                    SendPassword_Descr_RequiredFields = "campos obrigatórios"
                    SendPassword_Descr_BackToLogin = "Voltar ao início da sessão"
                    SendPassword_Descr_PasswordSentTo = "A sua senha foi enviada a {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "A sua senha da Área Protegida será enviada apenas para o seu endereço de correio electrónico registrado.<BR>Se não receber a mensagem de correio electrónico no prazo de vinte e quatro (24) horas, por favor entre em contacto connosco <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "PT"
                    StatusLineUsername = "Utilizador"
                    StatusLinePassword = "Senha"
                    StatusLineSubmit = "Ir"
                    StatusLineEditorial = "Editorial"
                    StatusLineContactUs = "Contácte-nos"
                    StatusLineDataprotection = "Declaração de privacidade"
                    StatusLineLoggedInAs = "Registado como"
                    StatusLineLegalNote = "Declaração de privacidade e advertências jurídicas"
                    StatusLineCopyright_AllRightsReserved = "Todos os direitos reservados."
                    NavAreaNameYourProfile = "O seu perfil"
                    NavLinkNameUpdatePasswort = "Alterar senha"
                    NavLinkNameUpdateProfile = "Alterar o perfil"
                    NavLinkNameLogout = "Terninar a sessão"
                    NavLinkNameLogin = "Início da sessão"
                    NavPointUpdatedHint = "Aqui estão alguns itens novos ou actualizados"
                    NavPointTemporaryHiddenHint = "O acesso a esta aplicação foi temporariamente bloqueado a outros utilizadores. Esta aplicação pode estar em construção."
                    SystemButtonYes = "Sim"
                    SystemButtonNo = "Não"
                    SystemButtonOkay = "Ok"
                    SystemButtonCancel = "Cancelar"
                    ErrorUserOrPasswordWrong = "Nome do utilizador ou da senha mal dactilografado ou mal soletrado ou acesso negado!<p>Por favor, verifique que <ul><li>a ortografia do nome do utilizador e da senha (a própria senha varia entre grande e pequena!)</li><li>é o nome correcto do utilizador/da senha (talvez tenha recibido senha para os outros empregados que não trabalham aqui)</li></ul>"
                    ErrorServerConfigurationError = "Este servidor ainda não foi configurado correctamente. Consulte o seu administrador."
                    ErrorNoAuthorization = "Voçê não é autorizado para entrar nesta área."
                    ErrorAlreadyLoggedOn = "Já iniciou sessão! Por favor, termine primeiro a sessão no outro terminal!<br><font color=""red"">Se estiver certo que não tem iniciada a sessão díga-nos através do <a href=""mailto:[n:0]"">[n:1]</a>!</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "A sua sessão foi terminada por ter iniciado sessão noutro terminal.<br>"
                    ErrorLogonFailedTooOften = "O processo de início de sessão falhou demasiadas vezes, a conta foi desactivada.<br>Por favor, tente de novo mais tarde!"
                    ErrorEmptyPassword = "Não se esqueça de introduzir a senha!<br>Se não sober a sua senha, tente mais uma vez consegui-lo por e-mail. Considere os detalhes na parte inferior de este documento."
                    ErrorRequiredField = "Campo obrigatório"
                    ErrorUnknown = "Erro imprevisto! - Contacte <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Por favor, introduza valores em todos os campos obrigatórios marcados com asterixo <em>(*)</em>!"
                    ErrorWrongNetwork = "Você não tem autorização para ligar através da rede actual."
                    ErrorUserAlreadyExists = "Já existe um início de sessão com este nome. Seleccione um outro nome para iniciar a sessão!"
                    ErrorLoginCreatedSuccessfully = "A conta para o início da sessão foi criada com éxito!"
                    ErrorSendPWWrongLoginOrEmailAddress = "InÍcio da sessão errado ou endereço E-Mail errado.<br>Introduza os valores correctos para iniciar o processo com a sua senha."
                    ErrorCookiesMustNotBeDisabled = "O seu browser não suporta cookies ou os cookies estão desactivados por causa das políticas de segurança do seu browser."
                    ErrorTimoutOrLoginFromAnotherStation = "Limite de tempo de sessão excedido ou iniciou sessão a partir de outro terminal."
                    ErrorApplicationConfigurationIsEmpty = "Esta aplicação ainda não foi configurada. Contacte o fabricante desta aplicação."
                    InfoUserLoggedOutSuccessfully = "Sessão terminada com sucesso. Agradecemos a sua visita."
                    UserManagementEMailColumnTitleLogin = "Nome do utilizador: "
                    UserManagementEMailColumnTitleCompany = "Empresa: "
                    UserManagementEMailColumnTitleName = "Nome: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Rua: "
                    UserManagementEMailColumnTitleZIPCode = "Código postal: "
                    UserManagementEMailColumnTitleLocation = "Cidade: "
                    UserManagementEMailColumnTitleState = "Estado: "
                    UserManagementEMailColumnTitleCountry = "País: "
                    UserManagementEMailColumnTitle1stLanguage = "Primeira lingua preferida: "
                    UserManagementEMailColumnTitle2ndLanguage = "Segunda lingua preferida: "
                    UserManagementEMailColumnTitle3rdLanguage = "Terceira lingua preferida: "
                    UserManagementEMailColumnTitleComesFrom = "Vem de: "
                    UserManagementEMailColumnTitleMotivation = "Motivo: "
                    UserManagementEMailColumnTitleCustomerNo = "No. de cliente: "
                    UserManagementEMailColumnTitleSupplierNo = "No. do fornecedor: "
                    UserManagementEMailColumnTitleComment = "Comentario: "
                    UserManagementAddressesMr = "Sr. "
                    UserManagementAddressesMs = "Sra. "
                    UserManagementEMailTextRegards = "Cumprimentos"
                    UserManagementEMailTextSubject = "A sua nome do utilizador"
                    UserManagementEMailTextSubject4AdminNewUser = "Área Protegida - novo utilizador"
                    UserManagementMasterServerAvailableInNearFuture = "Atenção: Este Servidor estará disponível apenas num futuro próximo."
                    CreateAccount_MsgEMailWelcome_WithPassword = "Bem-vindo à nossa Área Protegida! O lugar a visitar todos os dias!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua nome do utilizador é: [n:0]    " & ChrW(13) & ChrW(10) & _
                "A sua senha é: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Por favor, não se esqueça de alterar a senha, o quanto antes, para evitar o acesso e utilização por parte de terceiros!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Aproveite das vantagens de um suporte extensivo! Há muita informação à sua espera na nossa Extranet. Esteja à vontade para surfar neste site e para o explorar na totalidade." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua conta permite o acesso a diversos programas protegidos. Por agora, deverá já ter concluído o processo de associação da nossa Área Protegida. Autorizações adicionais e filiações serão definidas nos próximos 3 - 4 dias. De seguida, por favor, revisite a URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMailWelcome = "Bem-vindo à nossa Área Protegida! O lugar a visitar todos os dias!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua nome do utilizador é: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Aproveite das vantangens de um suporte extensivo. Há muita informação à sua espera na nossa Extranet. Esteja à vontade para surfar neste site e para o explorar na totalidade." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua conta permite o acesso a diversos programas protegidos. Por agora, deverá já ter concluído o processo de associação da nossa Área Protegida. Autorizações adicionais e filiações serão definidas nos próximos 3 - 4 dias. De seguida, por favor, revisite a URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMail4Admin = "O novo utilizador seguinte foi adicionado à Área Protegida." & ChrW(13) & ChrW(10) & _
                "Por favor, atribua autorizações relacionadas!" & ChrW(13) & ChrW(10) & _
                "Para rectificar autorizações, por favor, visite " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_MsgEMail4Admin = "O novo utilizador seguinte foi adicionado à Área Protegida por si ou por um colega seu." & ChrW(13) & ChrW(10) & _
                "Por favor, atribua autorizações relacionadas!" & ChrW(13) & ChrW(10) & _
                "Para rectificar autorizações, por favor, visite " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Bem-vindo à nossa Área Protegida! O lugar a visitar todos os dias!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Foi criada a sua autorização de utilizador ""[n:0]"". Visite-nos em:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Aguardamos ansiosamente pela oportunidade de o servir no ciberespaço."
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Bem-vindo à nossa Área Protegida!"
                    SendPassword_EMailMessage = "Abaixo encontra informações como novo membro. Por favor, guarde esta mensagem de confirmação como um registo do seu nome de utilizador e senha. Considere esta informação confidencial e proceda em conformidade." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua senha da Área Protegida é: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Por favor, não se esqueça de Alterar a senha, o quianto antes, para evitar o acesso e utilização por parte de terceiro!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Uma conta permite o acesso a diversos programas protegidos. De seguida, por favor, revisite a URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "O administrador reiniciou a sua conta. Abaixo encontra informações como novo membro. Por favor, guarde esta mensagem de confirmação como um registo do seu nome de utilizador e senha. Considere esta informação confidencial e proceda em conformidade." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "A sua palavra-passa para a Área Protegida é: [n:0]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Por favor, não se esqueça de alterar a sua senha, o quanto antes, para evitar o acesso e utilização por parte de terceiros!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Uma conta permite o acesso a diversos programas protegidos. Por agora, deverá já ter concluído o processo de associação da nossa Área Protegida. De seguida, por favor, revisite a URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Aproveite as vantagens do suporte extensivo!"
                    HighlightTextTechnicalSupport = "Estamos agora a começar e tudo está em constante mutação."
                    HighlightTextExtro = "Há muita informação à sua espera na nossa Extranet. Esteja à vontade para surfar neste site e para o explorar na totalidade."
                    WelcomeTextWelcomeMessage = "Bem-vindo à nossa Área Protegida!"
                    WelcomeTextFeedbackToContact = "Do you need additional features? Please don't hesitate to send comments on the Extranet to <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "O lugar a visitar todos os dias!"
                    UserManagementEMailTextDearUndefinedGender = "Caro/Prezada "
                    UserManagementSalutationUnformalUndefinedGender = "Olá "
                    NavAreaNameLogin = "Início da sessão"
                    NavLinkNamePasswordRecovery = "Esqueceu a senha?"
                    NavLinkNameNewUser = "Criar uma nova conta de utilizador"
                    CreateAccount_Descr_MotivItemSupplier = "Fornecedor"
                    UpdateProfile_Descr_MotivItemSupplier = "Fornecedor"
                Case 343
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("pl-PL")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("pl-PL")
                    UpdateProfile_Descr_Fax = "Faks"
                    UpdateProfile_Descr_Mobile = "Telefon komórkowy"
                    UpdateProfile_Descr_Phone = "Telefon"
                    UpdateProfile_Descr_PositionInCompany = "Pozycja"
                    UpdateProfile_Descr_Title = "Zmiana profilu użytkownika"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Do kontynuacji wprowadź wartości do wszystkich wymaganych pól!"
                    UpdateProfile_ErrMsg_MistypedPW = "Nieprawidłowe hasło. Zwróć uwagę na pisownię dużymi i małymi literami!"
                    UpdateProfile_ErrMsg_Undefined = "Nieoczekiwana wartość odpowiedzi! - Skontaktuj się z naszym webmasterem!"
                    UpdateProfile_ErrMsg_Success = "Twój profil użytkownika został pomyślnie zmieniony!"
                    UpdateProfile_ErrMsg_LogonTooOften = "Proces logowania nie powiódł się zbyt wiele razy, dlatego konto użytkownika zostało dezaktywowane.<br>Spróbuj jeszcze raz później!"
                    UpdateProfile_ErrMsg_NotAllowed = "Nie masz wystarczającego uprawnienia do wykonania tej akcji!"
                    UpdateProfile_ErrMsg_PWRequired = "Wprowadź także hasło, aby zaktualizować profil!"
                    UpdateProfile_Descr_Address = "Adres"
                    UpdateProfile_Descr_Company = "Firma"
                    UpdateProfile_Descr_Addresses = "Tytuł"
                    UpdateProfile_Descr_PleaseSelect = "(wybierz!)"
                    UpdateProfile_Abbrev_Mister = "Pan"
                    UpdateProfile_Abbrev_Miss = "Pani"
                    UpdateProfile_Descr_AcademicTitle = "Tytuł naukowy (np. ""dr"")"
                    UpdateProfile_Descr_FirstName = "Imię"
                    UpdateProfile_Descr_LastName = "Nazwisko"
                    UpdateProfile_Descr_NameAddition = "Dodatek do nazwiska"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_UserDetails = "Szczegóły użytkownika"
                    UpdateProfile_Descr_1stLanguage = "1. preferowany język"
                    UpdateProfile_Descr_2ndLanguage = "2. preferowany język"
                    UpdateProfile_Descr_3rdLanguage = "3. preferowany język"
                    UpdateProfile_Descr_Authentification = "Potwierdź zmiany podając swoje hasło"
                    UpdateProfile_Descr_Password = "Hasło"
                    UpdateProfile_Descr_Submit = "Aktualizuj profil"
                    UpdateProfile_Descr_RequiredFields = "* wymagane pola"
                    UpdateProfile_Descr_CustomerSupplierData = "Dane klienta bądź dostawcy"
                    UpdateProfile_Descr_CustomerNo = "Nr klienta"
                    UpdateProfile_Descr_SupplierNo = "Nr dostawcy"
                    UserJustCreated_Descr_AccountCreated = "Twoje konto użytkownika zostało pomyślnie utworzone!"
                    UserJustCreated_Descr_LookAroundNow = "Możesz kontynuować i rozglądnąć się tutaj."
                    UserJustCreated_Descr_PleaseNote = "Należy pamiętać: chwilowo jesteś <fontcolour=""#336699"">członkiem części publicznej<font>. Dodatkowe członkowstwa i prawa dostępu otrzymasz w przeciągu następnych 3 - 4 dni roboczych."
                    UserJustCreated_Descr_Title = "Witamy!"
                    UpdatePW_Descr_Title = "Resetowanie hasła"
                    UpdatePW_ErrMsg_ConfirmationFailed = "Potwierdzenie hasła nie zgadza się z nowym hasłem. Upewnij się, że wprowadziłeś identyczne frazy w polu hasła i potwierdzenia hasła. Należy także pamiętać, że przy wpisywaniu hasła rozróżniane są duże i małe litery."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Wpisz stare i nowe hasło. Należy także pamiętać, że przy wpisywaniu hasła rozróżniane są duże i małe litery."
                    UpdatePW_ErrMsg_Undefined = "Wystąpił nieznany błąd!"
                    UpdatePW_ErrMsg_Success = "Hasło zostało pomyślnie zmienione!"
                    UpdatePW_ErrMsg_WrongOldPW = "Zmiana hasła była niemożliwa. Sprawdź, czy prawidłowo wprowadziłeś aktualne hasło."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Wypełnij wszystkie wymagane pola, aby zakończyć zmienianie hasła!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Wpisz aktualne i nowe hasło:"
                    UpdatePW_Descr_CurrentPW = "Aktualne hasło"
                    UpdatePW_Descr_NewPW = "Nowe hasło"
                    UpdatePW_Descr_NewPWConfirm = "Powtórz nowe hasło"
                    UpdatePW_Descr_Submit = "Zapisz zmiany"
                    UpdatePW_Descr_RequiredFields = "* wymagane pola"
                    CreateAccount_Descr_CustomerSupplierData = "Dane klienta bądź dostawcy"
                    CreateAccount_Descr_CustomerNo = "Nr klienta"
                    CreateAccount_Descr_SupplierNo = "Nr dostawcy"
                    CreateAccount_Descr_FollowingError = "Wystąpił następujący błąd:"
                    CreateAccount_Descr_LoginDenied = "Logowanie nie zostało zaakceptowane!"
                    CreateAccount_Descr_Submit = "Załóż konto użytkownika"
                    CreateAccount_Descr_RequiredFields = "wymagane pola"
                    CreateAccount_Descr_BackToLogin = "Powrót do logowania"
                    CreateAccount_Descr_PageTitle = "Załóż nowe konto użytkownika"
                    CreateAccount_Descr_UserLogin = "Dane logowania"
                    CreateAccount_Descr_NewLoginName = "Twoja nowa nazwa użytkownika"
                    CreateAccount_Descr_NewLoginPassword = "Twoje nowe hasło"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Potwierdzenie hasła"
                    CreateAccount_Descr_Address = "Dane adresowe"
                    CreateAccount_Descr_Company = "Firma"
                    CreateAccount_Descr_Addresses = "Tytuł"
                    CreateAccount_Descr_PleaseSelect = "(wybierz!)"
                    CreateAccount_Descr_AcademicTitle = "Tytuł naukowy (np. ""dr"")"
                    CreateAccount_Descr_FirstName = "Imię"
                    CreateAccount_Descr_LastName = "Nazwisko"
                    CreateAccount_Descr_NameAddition = "Dodatek do nazwiska"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Ulica"
                    CreateAccount_Descr_ZIPCode = "Kod poczt."
                    CreateAccount_Descr_Location = "Miejscowość"
                    CreateAccount_Descr_State = "Kraj związkowy/kanton"
                    CreateAccount_Descr_Country = "Kraj"
                    CreateAccount_Descr_Motivation = "Co skłoniło Cię do rejestracji"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "odwiedzający stronę internetową"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "odwiedzający stronę internetową"
                    CreateAccount_Descr_MotivItemDealer = "dealer"
                    UpdateProfile_Descr_MotivItemDealer = "dealer"
                    CreateAccount_Descr_MotivItemJournalist = "dziennikarz"
                    UpdateProfile_Descr_MotivItemJournalist = "dziennikarz"
                    CreateAccount_Descr_MotivItemOther = "inne, proszę podać"
                    UpdateProfile_Descr_MotivItemOther = "inne, proszę podać"
                    CreateAccount_Descr_WhereHeard = "Skąd dowiedziałeś się o Secured Area"
                    CreateAccount_Descr_WhereItemFriend = "polecenie przyjaciela"
                    UpdateProfile_Descr_WhereItemFriend = "polecenie przyjaciela"
                    CreateAccount_Descr_WhereItemResellerDealer = "dystrybutor, dealer"
                    UpdateProfile_Descr_WhereItemResellerDealer = "dystrybutor, dealer"
                    CreateAccount_Descr_WhereItemExhibition = "wystawa, targi"
                    UpdateProfile_Descr_WhereItemExhibition = "wystawa, targi"
                    CreateAccount_Descr_WhereItemMagazines = "czasopismo"
                    UpdateProfile_Descr_WhereItemMagazines = "czasopismo"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "informacja pracownika"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "informacja pracownika"
                    CreateAccount_Descr_WhereItemSearchEnginge = "wyszukiwarka internetowa, prosimy podać"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "wyszukiwarka internetowa, prosimy podać"
                    CreateAccount_Descr_WhereItemOther = "inne, proszę podać"
                    UpdateProfile_Descr_WhereItemOther = "inne, proszę podać"
                    CreateAccount_Descr_UserDetails = "Informacje użytkownika"
                    CreateAccount_Descr_Comments = "Komentarze"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Zapytania dotyczące dodatkowych uprawnień"
                    CreateAccount_Descr_1stPreferredLanguage = "1. preferowany język"
                    CreateAccount_Descr_2ndPreferredLanguage = "2. preferowany język"
                    CreateAccount_Descr_3rdPreferredLanguage = "3. preferowany język"
                    CreateAccount_ErrorJS_InputValue = "Wpisz wartość w polu \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Wpisz wartość w polu \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Wpisz wartość o co najmniej [n:0] znakach w polu \""[n:1]\""."
                    UpdateProfile_ErrorJS_Length = "Wpisz wartość o co najmniej [n:0] znakach w polu \""[n:1]\""."
                    Banner_Help = "Pomoc"
                    Banner_HeadTitle = "Login do Secured Area"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Logon"
                    Banner_Feedback = "Feedback"
                    Logon_HeadTitle = "Logowanie do Secured Area"
                    Logon_AskForForcingLogon = "Uwaga! Jesteś już zalogowany w innej sesji. Chcesz ją zakończyć i rozpocząć nową?"
                    Logon_BodyTitle = "Logowanie do " & OfficialServerGroup_Title
                    Logon_SSO_ADS_ButtonNext = "Zakończ"
                    Logon_SSO_ADS_ContactUs = "W razie pytań prosimy o <a href=""mailto:{0}"">kontakt</a>."
                    Logon_SSO_ADS_IdentifiedUserName = "Zostałeś zidentyfikowany jako użytkownik <strong>{0}</strong>."
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Zostałeś zidentyfikowany jako użytkownik <strong>{0} ({1})</strong>."
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Nazwa użytkownika:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Hasło:"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "Adres e-mail:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Hasło (powtórzenie):"
                    Logon_SSO_ADS_LabelTakeAnAction = "Co chcesz zrobić?"
                    Logon_SSO_ADS_PageTitle = "Konfiguracja automatycznego logowania"
                    Logon_SSO_ADS_RadioDoNothing = "Jeżeli identyfikacja jest nieprawidłowa lub jeśli nie chcesz się logować, kontynuuj jako anonimowy użytkownik (ten dialog pojawi się później ponownie)."
                    Logon_SSO_ADS_RadioRegisterExisting = "Zaloguj się do <strong>już istniejącego</strong> konta użytkownika"
                    Logon_SSO_ADS_RadioRegisterNew = "Zarejestruj się z <strong>nowym</strong> kontem użytkownika"
                    Logon_BodyPrompt2User = "Wpisz nazwę użytkownika i przynależne hasło, aby uzyskać dostęp do " & OfficialServerGroup_Title & ".<br><em>Należy także pamiętać, że nazwa użytkownika i hasło mogą różnić się od innych danych, otrzymanych do innych obszarów.</em>"
                    Logon_BodyFormUserName = "Nazwa użytkownika"
                    Logon_BodyFormUserPassword = "Hasło"
                    Logon_BodyFormSubmit = "Login"
                    Logon_BodyFormCreateNewAccount = "Załóż konto użytkownika"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Jeszcze nie jesteś członkiem? Załóż własne konto dostępowe do obszaru " & OfficialServerGroup_Title & "!</STRONG><BR>" & _
                                    "Jeżeli nie masz jeszcze danych dostępowych, możesz założyć teraz </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>" & _
                                    "</FONT></A><FONT face=Arial size=2>. " & _
                                    "Nie zakładaj innych danych dostępowych," & _
                                    "jeżeli założyłeś już konto w przeszłości. W razie problemów z logowaniem proszę skontaktować się z naszym <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>Support Service Center</FONT></A> " & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Zapomniałeś hasła? Wyślemy Ci hasło " & _
                                    "</B><BR>Dostałeś już dane logowania, ale zapomniałeś hasła?" & _
                                    "</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>Tu dostaniesz hasło mailem</FONT></A><FONT " & _
                                    "face=Arial size=2>. Należy pamiętać, że mail zostanie wysłany tylko na ten adres e-mail, który został pierwotnie wpisany przy zakładaniu konta" & _
                                    ".<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Nie uzyskałeś tu odpowiedzi na pytanie?</strong><br>Jeżeli potrzebujesz dodatkowej pomocy, napisz do nas </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2></FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    Logon_Connecting_InProgress = "Zostaniesz połączony z serwerem…"
                    Logon_Connecting_LoginTimeout = "Przekroczenie limitu czasu przy logowaniu."
                    Logon_Connecting_RecommendationOnTimeout = "Jeżeli problem pojawi się ponownie, prosimy o <a href=""mailto:[0]"">kontakt</a> z nami."
                    AccessError_Descr_FollowingError = "Wystąpił następujący błąd:"
                    AccessError_Descr_LoginDenied = "Logowanie nie zostało zaakceptowane!"
                    AccessError_Descr_BackToLogin = "Powrót do logowania"
                    SendPassword_Descr_FollowingError = "Wystąpił następujący błąd:"
                    SendPassword_Descr_LoginDenied = "Logowanie nie zostało zaakceptowane!"
                    SendPassword_Descr_Title = "Żądanie mailem hasła do Secured Area"
                    SendPassword_Descr_LoginName = "Nazwa użytkownika"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Wyślij e-mail"
                    SendPassword_Descr_RequiredFields = "wymagane pola"
                    SendPassword_Descr_BackToLogin = "Powrót do logowania"
                    SendPassword_Descr_PasswordSentTo = "Hasło zostało wysłane do {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "Twoje hasło do Secured Area zostało wysłane na podany adres e-mail.<BR>Jeżeli mail nie dotrze w przeciągu następnych 24 godzin, prosimy o kontakt na adres <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "PL"
                    StatusLineUsername = "Użytkownik"
                    StatusLinePassword = "Hasło"
                    StatusLineSubmit = "Login"
                    StatusLineEditorial = "Stopka redakcyjna"
                    StatusLineContactUs = "Kontakt"
                    StatusLineDataprotection = "Ochrona danych"
                    StatusLineLoggedInAs = "Zalogowany jako"
                    StatusLineLegalNote = "Ochrona danych i informacje prawne"
                    StatusLineCopyright_AllRightsReserved = "Wszelkie prawa zastrzeżone."
                    NavAreaNameYourProfile = "Twój profil"
                    NavLinkNameUpdatePasswort = "Zmiana hasła"
                    NavLinkNameUpdateProfile = "Zmiana danych użytkownika"
                    NavLinkNameLogout = "Wylogowanie"
                    NavLinkNameLogin = "Login"
                    NavPointUpdatedHint = "Stworzono tu coś nowego bądź zaktualizowano coś istniejącego"
                    NavPointTemporaryHiddenHint = "Ta aplikacja jest tymczasowo dezaktywowana dla innych użytkowników. Często wskazuje to na fakt, że aplikacja znajduje się jeszcze w fazie rozwojowej."
                    SystemButtonYes = "Tak"
                    SystemButtonNo = "Nie"
                    SystemButtonOkay = "OK"
                    SystemButtonCancel = "Anuluj"
                    ErrorUserOrPasswordWrong = "Nazwa użytkownika bądź hasło nie są prawidłowe lub zostały źle wpisane albo dostęp został odrzucony!<p>Sprawdź <ul><li>pisownię hasła i nazwy użytkownika (hasło rozróżnia pomiędzy dużymi i małymi literami!)</li><li>aby używana była prawidłowa kombinacja nazwy użytkownika i hasła. (Ewentualnie otrzymałeś już od nas inne nazwy użytkownika i hasła, które nie obowiązują jednak dla tego obszaru.)</li></ul>"
                    ErrorServerConfigurationError = "Serwer nie został jeszcze prawidłowo skonfigurowany. Skonsultuj się z administratorem."
                    ErrorNoAuthorization = "Nie masz uprawnień do dostępu do tego obszaru."
                    ErrorAlreadyLoggedOn = "Jesteś już zalogowany! Najpierw wyloguj się na innym stanowisku pracy!<br><font color=""red"">Jeżeli jesteś pewien, że nie jesteś nigdzie zalogowany, wyślij krótką informację na adres <a href=""mailto:[n:0]"">[n:1]</a> i podaj swoją nazwę użytkownika.</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Zostałeś wylogowany na tym satnowisku pracy, gdyż zalogowałeś się na innej stacji roboczej.<br>"
                    ErrorLogonFailedTooOften = "Proces logowania nie powiódł się zbyt wiele razy, dlatego konto użytkownika zostało tymczasowo dezaktywowane.<br>Spróbuj ponownie później!"
                    ErrorEmptyPassword = "Nie zapomnij wpisać jeszcze jednego hasła!<br>Jeżeli zapomniałeś hasła, możesz zażądać jego przesłanie mailem. Patrz szczegółowe informacje dalej w tekście."
                    ErrorUnknown = "Nieoczekiwany błąd! - Skontaktuj się z naszym <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Wpisz wartości do wszystkich pól, zaznaczonych gwiazdką <em>(*)</em>!"
                    ErrorWrongNetwork = "Nie masz uprawnień do zalogowania się przez aktualne połączenie sieciowe."
                    ErrorUserAlreadyExists = "To konto użytkownika już istnieje. Wybierz inną nazwę użytkownika!"
                    ErrorLoginCreatedSuccessfully = "Profil użytkownika został pomyślnie utworzony!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Nieprawidłowa nazwa użytkownika lub nieprawidłowy adres e-mail.<br>Wprowadź prawidłowe wartości, odpowiadające wartościom zapisanym w Twoim profilu użytkownika."
                    ErrorCookiesMustNotBeDisabled = "Twoja przeglądarka nie obsługuje zapisywania plików cookie lub funkcja ta została zdezaktywowana przez ustawienia bezpieczeństwa przeglądarki."
                    ErrorTimoutOrLoginFromAnotherStation = "Zostałeś wylogowany, gdyż osiągnięty został maksymalny czas sesji albo zostało dokonane logowanie z innej stacji roboczej."
                    ErrorApplicationConfigurationIsEmpty = "Ta aplikacja nie otrzymała prawidłowej nazwy aplikacji. Proszę skontaktować się z producentem."
                    InfoUserLoggedOutSuccessfully = "Zostałeś wylogowany. Dziękujemy za wizytę."
                    UserManagementEMailColumnTitleLogin = "Nazwa użytkownika: "
                    UserManagementEMailColumnTitleCompany = "Firma: "
                    UserManagementEMailColumnTitleName = "Nazwisko: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Ulica: "
                    UserManagementEMailColumnTitleZIPCode = "Kod poczt.: "
                    UserManagementEMailColumnTitleLocation = "Miejscowość: "
                    UserManagementEMailColumnTitleState = "Państwo: "
                    UserManagementEMailColumnTitleCountry = "Kraj: "
                    UserManagementEMailColumnTitle1stLanguage = "1. preferowany język: "
                    UserManagementEMailColumnTitle2ndLanguage = "2. preferowany język: "
                    UserManagementEMailColumnTitle3rdLanguage = "3. preferowany język: "
                    UserManagementEMailColumnTitleComesFrom = "Przyszło od: "
                    UserManagementEMailColumnTitleMotivation = "Motywacja: "
                    UserManagementEMailColumnTitleCustomerNo = "Nr klienta: "
                    UserManagementEMailColumnTitleSupplierNo = "Nr dostawcy: "
                    UserManagementEMailColumnTitleComment = "Komentarz: "
                    UserManagementAddressesMr = "Pan "
                    UserManagementAddressesMs = "Pani "
                    UserManagementSalutationUnformalMasculin = "Witamy "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Panie i Panowie, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Szanowny Panie "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Witam, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Witamy "
                    UserManagementSalutationFormulaUnformalGroup = "Witam, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Szanowna Pani "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagementEMailTextRegards = "Z poważaniem"
                    UserManagementEMailTextSubject = "Twoje dane dostępowe do Secured Area"
                    UserManagementEMailTextSubject4AdminNewUser = "Secured Area - nowy użytkownik"
                    UserManagementMasterServerAvailableInNearFuture = "Uwaga: ten serwer będzie dostępny dopiero w najbliższej przyszłości."
                    CreateAccount_MsgEMailWelcome = "Witamy w Secured Area! Tu znajdziesz codzienne, globalne informacje." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twój login do Secured Area: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Skorzystaj z zalet naszej szerokiej pomocy! Zaglądnij czasami do nas!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twoje konto użytkownika zapewnia dostęp do różnych chronionych aplikacji. Proces rejestracji do Secured Area został już zakończony. Pełne uprawnienia będą jednak ważne dopiero po 3 - 4 dniach roboczych. Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Witamy w Secured Area! Tu znajdziesz codzienne, globalne informacje." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twój login do Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Twoje hasło: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Pamiętaj, aby jak najszybciej zmienić to hasło. Jest to konieczne, aby nikt inny (np. haker) nie posiadał i nie nadużywał Twojego hasła!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Skorzystaj z zalet szerokiej pomocy! Już teraz dużo na Ciebie czeka. Zaglądnij czasami do nas!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twoje konto użytkownika zapewnia dostęp do różnych chronionych aplikacji. Proces rejestracji do Secured Area został już zakończony. Pełne uprawnienia będą jednak ważne dopiero po 3 - 4 dniach roboczych. Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMail4Admin = "Następujący użytkownik zalogował się jako nowy w obszarze Secured Area." & ChrW(13) & ChrW(10) & _
                "Przypisz mu wymagane prawa dostępu!" & ChrW(13) & ChrW(10) & _
                "Aby ustawić prawa dostępu, odwiedź " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextWelcome = "Witamy w Secured Area! Tu znajdziesz codzienne, globalne informacje." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twoje konto w obszarze Secured Area zostało skonfigurowane przez naszego administratora. Ten serwis jest oczywiście bezpłatny." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twój login do Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Twoje hasło do Secured Area: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Pamiętaj, aby jak najszybciej zmienić to hasło. Jest to konieczne, aby nikt inny (np. haker) nie posiadał i nie nadużywał Twojego hasła!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Skorzystaj z zalet szerokiej pomocy! Już teraz dużo na Ciebie czeka. Zaglądnij czasami do nas!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twoje konto użytkownika zapewnia dostęp do różnych chronionych aplikacji. Proces rejestracji do Secured Area został już zakończony. Pełne uprawnienia będą jednak ważne dopiero po 3 - 4 dniach roboczych. Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Witamy w Secured Area! Tu znajdziesz codzienne, globalne informacje.</p>" & _
                "<p>Twoje konto w obszarze Secured Area zostało skonfigurowane przez naszego administratora. Ten serwis jest oczywiście bezpłatny.</p>" & _
                "<p><strong>Twój login do Secured Area: <font color=""red"">[n:0]</font><br>" & _
                "Twoje hasło do Secured Area: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Pamiętaj, aby jak najszybciej zmienić to hasło. Jest to konieczne, aby nikt inny (np. haker) nie posiadał i nie nadużywał Twojego hasła!</p>" & _
                "<p>Skorzystaj z zalet szerokiej pomocy! Już teraz dużo na Ciebie czeka. Schauen Sie doch einfach mal vorbei!" & _
                "<p>Twoje konto użytkownika zapewnia dostęp do różnych chronionych aplikacji. Proces rejestracji do Secured Area został już zakończony. Pełne uprawnienia będą jednak ważne dopiero po 3 - 4 dniach roboczych. Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    UserManagement_NewUser_MsgEMail4Admin = "Ten użytkownik został założony przez Ciebie lub kolegę w obszarze Secured Area." & ChrW(13) & ChrW(10) & _
                "Przypisz mu wymagane prawa dostępu!" & ChrW(13) & ChrW(10) & _
                "Aby ustawić prawa dostępu, odwiedź " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Witamy w Secured Area!"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Witamy w Secured Area! Tu znajdziesz codzienne, globalne informacje." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Zostały Ci przydzielone prawa dostępu dla użytkownika ""[n:0]"". Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Cieszymy się z ponownego spotkania z Tobą na naszych stronach."
                    SendPassword_EMailMessage = "Poniżej znajdziesz dane Twojego profilu użytkownika. Zachowaj ten mail z danymi użytkownika lub hasłem i traktuj te informacje poufnie." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twoje hasło do Secured Area: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Pamiętaj, aby jak najszybciej zmienić to hasło. Jest to konieczne, aby nikt inny (np. haker) nie posiadał i nie nadużywał Twojego hasła!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Twoje konto użytkownika zapewnia dostęp do różnych chronionych aplikacji. Proces rejestracji do Secured Area został już zakończony. Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "Pełnomocnik do spraw bezpieczeństwa zresetował Twoje konto użytkownika. Poniżej znajdziesz nowe dane profilu użytkownika. Zachowaj ten mail z danymi użytkownika i hasłem i traktuj te informacje poufnie." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Twoje hasło do Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Pamiętaj, aby jak najszybciej zmienić to hasło. Jest to konieczne, aby nikt inny (np. haker) nie posiadał i nie nadużywał Twojego hasła!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Twoje konto użytkownika zapewnia dostęp do różnych chronionych aplikacji. Proces rejestracji do Secured Area został już zakończony. Przez następujący adres URL masz jednak możliwość dostępu do Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Skorzystaj z zalet szerokiej pomocy!"
                    HighlightTextTechnicalSupport = "Dopiero zaczynamy i dużo się jeszcze zmieni."
                    HighlightTextExtro = "Jednak już teraz czeka na Ciebie dużo interesujących informacji. Skorzystaj z okazji!"
                    WelcomeTextWelcomeMessage = "Witamy w Secured Area!"
                    WelcomeTextFeedbackToContact = "Potrzebujesz dodatkowych funkcji? Nie ma problemu! Wyślij nam swoje komenatrze dotyczące sieci Extranet na adres <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "Tu znajdziesz codzienne, globalne informacje."
                    UpdateProfile_Descr_Street = "Ulica"
                    UpdateProfile_Descr_ZIPCode = "Kod poczt."
                    UpdateProfile_Descr_Location = "Miejscowość"
                    UpdateProfile_Descr_State = "Państwo"
                    UpdateProfile_Descr_Country = "Kraj"
                    UpdatePW_Error_PasswordComplexityPolicy = "Hasło musi się składać z co najmniej 3 znaków. Nie możesz używać elementów swojego nazwiska."
                    ErrorRequiredField = "Required field"
                    UserManagementEMailTextDearUndefinedGender = "Szanowna/Szanowny "
                    UserManagementSalutationUnformalUndefinedGender = "Witam "
                    NavAreaNameLogin = "Logowanie"
                    NavLinkNamePasswordRecovery = "Zapomniane hasło?"
                    NavLinkNameNewUser = "Załóż nowe konto"
                    CreateAccount_Descr_MotivItemSupplier = "Dostawca"
                    UpdateProfile_Descr_MotivItemSupplier = "Dostawca"
                Case 202
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("ja-JP")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("ja-JP")
                    Logon_AskForForcingLogon = "ご注意: 既に他のセッションにログオン中です。 このセッションをキャンセルして新たなセッションを始動しますか?"
                    UserManagementSalutationUnformalMasculin = "こんにちは "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Dear Sirs, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "様 "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "こんにちは！ "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "こんにちは "
                    UserManagementSalutationFormulaUnformalGroup = "こんにちは！ "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "様 "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagement_NewUser_TextWelcome = "Secured Areaへようこそ! ここでは毎日最新情報をお届けしています!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Secured Area用ユーザーアカウントが設定されました。このサービスは無料です。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のログイン名: [n:0]    " & ChrW(13) & ChrW(10) & _
                "お客様のSecured Area用パスワード: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "できるだけ早くパスワードを変更してください。このパスワードを誰かに教えたり使用させたりしないでください!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "広範囲におよぶサポートのメリットを是非ご利用ください! 当社のエクストラネットでは多数の情報をご提供しています。 ご自由にこのサイト上の情報をご覧になり、ご探索ください。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のアカウントは様々な保護プログラムへのアクセスを可能としています。 Secured Areaの登録プロセスは完了しています。 完全な登録承認は3～4日以内に完了します。 その後、以下のURLを改めてご利用ください:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Secured Areaへようこそ! ここでは毎日最新情報をお届けしています!</p>" & _
                "<p>Secured Area用ユーザーアカウントが設定されました。このサービスは無料です。</p>" & _
                "<p><strong>お客様のログイン名: <font color=""red"">[n:0]</font><br>" & _
                "お客様のSecured Area用パスワード: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>できるだけ早くパスワードを変更してください。このパスワードを誰かに教えたり使用させたりしないでください。!</p>" & _
                "<p>広範囲におよぶサポートのメリットを是非ご利用ください! 当社のエクストラネットでは多数の情報をご提供しています。ご自由にこのサイト上の情報をご覧になり、ご探索ください。</p>" & _
                "<p>お客様のアカウントは様々な保護プログラムへのアクセスを可能としています。Secured Areaの登録プロセスは完了しています。完全な登録承認は3～4日以内に完了します。その後、以下のURLを改めてご利用ください:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    UpdateProfile_Descr_Title = "プロフィールの変更"
                    UpdateProfile_Descr_Mobile = "携帯電話"
                    UpdateProfile_Descr_Fax = "ファックス"
                    UpdateProfile_Descr_Phone = "電話"
                    UpdateProfile_Descr_PositionInCompany = "役職"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "継続するためには、全ての必要欄に記入してください!"
                    UpdateProfile_ErrMsg_MistypedPW = "パスワードが間違っています!"
                    UpdateProfile_ErrMsg_Undefined = "値が不適切です! - webmasterにご連絡ください!"
                    UpdateProfile_ErrMsg_Success = "プロフィールの更新が完了しました!"
                    UpdateProfile_ErrMsg_LogonTooOften = "ログオンプロセスに何度も失敗しました。現在このアカウントは使用不可能となっています。<br>後ほど再試行してください!"
                    UpdateProfile_ErrMsg_NotAllowed = "このドキュメントへのアクセスは禁止されています!"
                    UpdateProfile_ErrMsg_PWRequired = "パスワードを入力し、プロフィールを修正してください!"
                    UpdateProfile_Descr_Address = "所在地"
                    UpdateProfile_Descr_Company = "会社名"
                    UpdateProfile_Descr_Addresses = "性別"
                    UpdateProfile_Descr_PleaseSelect = "(選択してください!)"
                    UpdateProfile_Abbrev_Mister = "男性"
                    UpdateProfile_Abbrev_Miss = "女性"
                    UpdateProfile_Descr_AcademicTitle = "学位（例 博士）"
                    UpdateProfile_Descr_FirstName = "名"
                    UpdateProfile_Descr_LastName = "姓"
                    UpdateProfile_Descr_NameAddition = "補足名"
                    UpdateProfile_Descr_EMail = "メール"
                    UpdateProfile_Descr_Street = "住所"
                    UpdateProfile_Descr_ZIPCode = "郵便番号"
                    UpdateProfile_Descr_Location = "市郡区名"
                    UpdateProfile_Descr_State = "都道府県名"
                    UpdateProfile_Descr_Country = "国名"
                    UpdateProfile_Descr_UserDetails = "ユーザー詳細"
                    UpdateProfile_Descr_1stLanguage = "第1言語"
                    UpdateProfile_Descr_2ndLanguage = "第2言語"
                    UpdateProfile_Descr_3rdLanguage = "第3言語"
                    UpdateProfile_Descr_Authentification = "パスワードを入力してこれらの変更内容を確定してください"
                    UpdateProfile_Descr_Password = "パスワード"
                    UpdateProfile_Descr_Submit = "プロフィール更新"
                    UpdateProfile_Descr_RequiredFields = "* 記入必要項目"
                    UpdateProfile_Descr_CustomerSupplierData = "顧客/サプライヤーデータ"
                    UpdateProfile_Descr_CustomerNo = "顧客番号"
                    UpdateProfile_Descr_SupplierNo = "サプライヤー番号"
                    UserJustCreated_Descr_AccountCreated = "ユーザーアカウントの作成が完了しました!"
                    UserJustCreated_Descr_LookAroundNow = "すぐに本サイトの閲覧が可能となります。"
                    UserJustCreated_Descr_PleaseNote = "備考: ここでは、 <font color=""#336699"">一般会員としてのステータスでご利用いただけます</font>。 正式な登録承認およびメンバーシップ承認は3～4日以内に完了します。"
                    UserJustCreated_Descr_Title = "Secured Areaへようこそ!"
                    UpdatePW_Descr_Title = "パスワードのリセット"
                    UpdatePW_ErrMsg_ConfirmationFailed = "パスワード確認が失敗しました! ご入力になったパスワードが正しいかを確認してください。 大文字と小文字の区別にご注意ください。"
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "現在有効となっているパスワードと新パスワードを入力してください。 大文字と小文字の区別にご注意ください。"
                    UpdatePW_ErrMsg_Undefined = "未定義のエラーが発生しました!"
                    UpdatePW_ErrMsg_Success = "パスワード変更が完了しました!"
                    UpdatePW_ErrMsg_WrongOldPW = "パスワードの変更が行なえませんでした! 現在有効となっているパスワードを正しく入力したかを再度確認してください。"
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "パスワード変更を完了するためには、全ての必要項目に記入してください!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "現在有効となっているパスワードと新パスワードを特定してください:"
                    UpdatePW_Descr_CurrentPW = "現在有効となっているパスワード"
                    UpdatePW_Descr_NewPW = "新パスワード"
                    UpdatePW_Descr_NewPWConfirm = "新パスワードの確定"
                    UpdatePW_Descr_Submit = "プロフィール更新"
                    UpdatePW_Descr_RequiredFields = "* 記入必要項目"
                    UpdatePW_Error_PasswordComplexityPolicy = "パスワードには3文字以上の文字が含まれていることが必要です。ご自身の名前やその一部を使用しないでください。"
                    CreateAccount_Descr_CustomerSupplierData = "顧客/サプライヤーデータ"
                    CreateAccount_Descr_CustomerNo = "顧客番号"
                    CreateAccount_Descr_SupplierNo = "サプライヤー番号"
                    CreateAccount_Descr_FollowingError = "以下のエラーが発生しています:"
                    CreateAccount_Descr_LoginDenied = "ログインが拒否されました!"
                    CreateAccount_Descr_Submit = "ユーザーアカウントの作成"
                    CreateAccount_Descr_RequiredFields = "記入必要項目"
                    CreateAccount_Descr_BackToLogin = "ログインに戻る"
                    CreateAccount_Descr_PageTitle = "新ユーザーの作成"
                    CreateAccount_Descr_UserLogin = "ユーザーログイン"
                    CreateAccount_Descr_NewLoginName = "お客様の新ログイン名"
                    CreateAccount_Descr_NewLoginPassword = "お客様の新パスワード"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "お客様のパスワードの確定"
                    CreateAccount_Descr_Address = "所在地"
                    CreateAccount_Descr_Company = "会社名"
                    CreateAccount_Descr_Addresses = "性別"
                    CreateAccount_Descr_PleaseSelect = "(選択してください!)"
                    CreateAccount_Descr_AcademicTitle = "学位（例 博士）"
                    CreateAccount_Descr_FirstName = "名"
                    CreateAccount_Descr_LastName = "姓"
                    CreateAccount_Descr_NameAddition = "補足名"
                    CreateAccount_Descr_Email = "メール"
                    CreateAccount_Descr_Street = "住所"
                    CreateAccount_Descr_ZIPCode = "郵便番号"
                    CreateAccount_Descr_Location = "市郡区名"
                    CreateAccount_Descr_State = "都道府県名"
                    CreateAccount_Descr_Country = "国名"
                    CreateAccount_Descr_Motivation = "ご登録の動機をお答えください"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "ウェブサイト訪問者"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "ウェブサイト訪問者"
                    CreateAccount_Descr_MotivItemDealer = "ディーラー"
                    UpdateProfile_Descr_MotivItemDealer = "ディーラー"
                    CreateAccount_Descr_MotivItemJournalist = "記者"
                    UpdateProfile_Descr_MotivItemJournalist = "記者"
                    CreateAccount_Descr_MotivItemOther = "その他（詳細）"
                    UpdateProfile_Descr_MotivItemOther = "その他（詳細）"
                    CreateAccount_Descr_WhereHeard = "当社のSecured Areaについてはどこでお知りになりましたか"
                    CreateAccount_Descr_WhereItemFriend = "友人"
                    UpdateProfile_Descr_WhereItemFriend = "友人"
                    CreateAccount_Descr_WhereItemResellerDealer = "再販業者/ディーラー"
                    UpdateProfile_Descr_WhereItemResellerDealer = "再販業者/ディーラー"
                    CreateAccount_Descr_WhereItemExhibition = "展示会"
                    UpdateProfile_Descr_WhereItemExhibition = "展示会"
                    CreateAccount_Descr_WhereItemMagazines = "雑誌"
                    UpdateProfile_Descr_WhereItemMagazines = "雑誌"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "当社から"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "当社から"
                    CreateAccount_Descr_WhereItemSearchEnginge = "検索エンジン（詳細）"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "検索エンジン（詳細）"
                    CreateAccount_Descr_WhereItemOther = "その他（詳細）"
                    UpdateProfile_Descr_WhereItemOther = "その他（詳細）"
                    CreateAccount_Descr_UserDetails = "ユーザー詳細"
                    CreateAccount_Descr_Comments = "コメント"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "アクセス権追加要求"
                    CreateAccount_Descr_1stPreferredLanguage = "第1言語"
                    CreateAccount_Descr_2ndPreferredLanguage = "第2言語"
                    CreateAccount_Descr_3rdPreferredLanguage = "第3言語"
                    CreateAccount_ErrorJS_InputValue = "\""[n:0]\""の欄に値を入力してください。"
                    UpdateProfile_ErrorJS_InputValue = "\""[n:0]\""の欄に値を入力してください。"
                    CreateAccount_ErrorJS_Length = "\""[n:1]\""の欄に[n:0] 文字以上の値を入力してください。"
                    UpdateProfile_ErrorJS_Length = "\""[n:1]\""の欄に[n:0] 文字以上の値を入力してください。"
                    Banner_Help = "ヘルプ"
                    Banner_HeadTitle = "Secured Area - ログイン"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - ログオン"
                    Banner_Feedback = "フィードバック"
                    Logon_Connecting_InProgress = "サーバーに接続しています…"
                    Logon_HeadTitle = "Secured Area - ログオン"
                    Logon_Connecting_LoginTimeout = "ログイン時間切れ。"
                    Logon_Connecting_RecommendationOnTimeout = "この問題が再発生した場合、当社までご連絡ください。"
                    Logon_BodyTitle = OfficialServerGroup_Title & " - ログオン"
                    Logon_SSO_ADS_PageTitle = "自動ログオンの設定"
                    Logon_SSO_ADS_IdentifiedUserName = "ユーザー <strong>{0}</strong>として識別されました。"
                    Logon_SSO_ADS_LabelTakeAnAction = "これから何をなさいますか?"
                    Logon_SSO_ADS_RadioRegisterExisting = "<strong>既存</strong>アカウントのの登録"
                    Logon_SSO_ADS_RadioRegisterNew = " <strong>新</strong>アカウントの登録"
                    Logon_SSO_ADS_RadioDoNothing = "識別が正しく行なわれなかった場合、または引き続きログインせずにご利用になりたい場合には、匿名ユーザーとして引き続きご利用ください（この質問は再度表示されます）。"
                    Logon_SSO_ADS_ContactUs = "ご質問等がございましたら<a href=""mailto:{0}"">までご連絡ください</a>。"
                    Logon_SSO_ADS_ButtonNext = "継続"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "ログイン名:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "パスワード:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "ユーザーパスワードの再入力:"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "メールアドレス:"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "<strong>{0} ({1})</strong>ユーザーとして識別されました。"
                    Logon_BodyPrompt2User = "ユーザーログイン名を入力し、" & OfficialServerGroup_Title & "にアクセスしてください。<br><em>このログイン名およびパスワードは独立したものであり、当社の他のエリア用にお知らせしたログイン名およびパスワードとは異なることがあります。</em>"
                    Logon_BodyFormUserName = "ログイン名"
                    Logon_BodyFormUserPassword = "パスワード"
                    Logon_BodyFormSubmit = "ログイン"
                    Logon_BodyFormCreateNewAccount = "新アカウントの作成"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>会員登録はまだですか? " & OfficialServerGroup_Title & "へのアクセスを可能とする新アカウントを作成してください</STRONG><BR>" & _
                                    "まだアカウントをお持ちでなければ、</FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>create " & _
                                    "ここで作成することができます</FONT></A><FONT face=Arial size=2>。 " & _
                                    "もし過去に既にアカウントを作成された場合、 ここでは他の&nbsp;" & _
                                    "アカウントを作成しないでください ログインの際に問題が生じた際には、 " & _
                                    "お客様担当の <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>サポートサービスセンター</FONT></A>" & _
                                    "までご連絡ください。 <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>パスワードを忘れましたか? お客様のパスワードを " & _
                                    "メールでご送付ください</B><BR>有効な&nbsp;アカウントをお知らせします。ただし、パスワードをお忘れになった場合はこの機能は " & _
                                    "ご使用いただけません。</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>ここでパスワードのメール照会を受け付けています</FONT></A><FONT " & _
                                    "face=Arial size=2>。 お客様のパスワードは " & _
                                    "この&nbsp;アカウントの作成時に " & _
                                    "ご登録になったメールアドレスに送付されます。 <br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>問題はまだ解決されていませんか?</strong><br>さらにサポートをご希望の方は、 </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>何なりとご連絡ください</FONT></A><FONT " & _
                                    "size=2>。</FONT></P></TD></TR></TABLE>"
                    AccessError_Descr_FollowingError = "以下のエラーが発生しています:"
                    AccessError_Descr_LoginDenied = "ログインが拒否されました!"
                    AccessError_Descr_BackToLogin = "ログインに戻る"
                    SendPassword_Descr_FollowingError = "以下のエラーが発生しています:"
                    SendPassword_Descr_LoginDenied = "ログインが拒否されました!"
                    SendPassword_Descr_Title = "Secured Areaパスワードのメール照会を希望"
                    SendPassword_Descr_LoginName = "ログイン名"
                    SendPassword_Descr_Email = "メール"
                    SendPassword_Descr_Submit = "メール送信"
                    SendPassword_Descr_RequiredFields = "記入必要項目"
                    SendPassword_Descr_BackToLogin = "ログインに戻る"
                    SendPassword_Descr_PasswordSentTo = "お客様のパスワードは{0}に送付されました。"
                    SendPassword_Descr_FurtherCommentWithContactAddress = "お客様のSecured Areaパスワードはご登録中のメールアドレスにのみ送付されます。<BR>24時間以内にメールが届かない場合には、<a href=""mailto:{0}"">{1}</a>までご連絡ください。"
                    META_CurrentContentLanguage = "JP"
                    StatusLineUsername = "ユーザー"
                    StatusLinePassword = "パスワード"
                    StatusLineSubmit = "Go!"
                    StatusLineEditorial = "発行者"
                    StatusLineContactUs = "ご連絡先"
                    StatusLineDataprotection = "プライバシー宣言"
                    StatusLineLoggedInAs = "ログインステータス"
                    StatusLineLegalNote = "プライバシー宣言および法的事項"
                    StatusLineCopyright_AllRightsReserved = "無断複写・転載を禁じます。"
                    NavAreaNameYourProfile = "お客様のプロフィール"
                    NavLinkNameUpdatePasswort = "パスワードの変更"
                    NavLinkNameUpdateProfile = "プロフィールの変更"
                    NavLinkNameLogout = "ログアウト"
                    NavLinkNameLogin = "ログオン"
                    NavPointUpdatedHint = "更新内容"
                    NavPointTemporaryHiddenHint = "現在このアプリケーションは他のユーザーにはご使用いただけない状態となっています。現在このアプリケーションは工事中です。"
                    SystemButtonYes = "はい"
                    SystemButtonNo = "いいえ"
                    SystemButtonOkay = "OK"
                    SystemButtonCancel = "キャンセル"
                    ErrorUserOrPasswordWrong = "ユーザー名、パスワードまたはアクセスが拒否されました!<p> <ul><li>ご入力になったユーザー名とパスワードが正しいか（パスワード上の大文字および小文字を区別する必要があります!)、および</li><li>、ユーザー名とパスワードの組み合わせが正しいか（他の場所でご使用中のパスワードをここで入力された可能性があります）を確認してください</li></ul>"
                    ErrorServerConfigurationError = "このサーバーは正しく設定されていません。サーバー管理者までご連絡ください。"
                    ErrorNoAuthorization = "このエリアへのアクセスは禁止されています。"
                    ErrorAlreadyLoggedOn = "ログオンは既に完了しています! 他のステーションをまずログアウトしてください!<br><font color=""red"">ログオンしたかどうか不明な場合には、 当社<a href=""mailto:[n:0]"">[n:1]</a>までご連絡ください!</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "他のステーションへのログオンが行なわれたため、このセッションを終了します。<br>"
                    ErrorLogonFailedTooOften = "ログオンプロセスに何度も失敗しました。現在このアカウントは使用不可能となっています。<br>後ほど再試行してください!"
                    ErrorEmptyPassword = "パスワードの入力を忘れないでください!<br>パスワードをお忘れになった場合には、 メールで照会してください。詳細情報は本ドキュメントの下に記載されています。"
                    ErrorRequiredField = "このフィールドは必須です"
                    ErrorUnknown = "予期せぬエラーが発生しました! - <a href=""mailto:support@camm.biz"">トラブルセンターまでご連絡ください</a>!"
                    ErrorEmptyField = "星印のついた項目には全て記入してください <em>(*)</em>!"
                    ErrorWrongNetwork = "現在ご使用中のネットワークによる接続は禁止されています。"
                    ErrorUserAlreadyExists = "このログイン名は既に存在します。他のログイン名を選択してください!"
                    ErrorLoginCreatedSuccessfully = "ログインアカウントの作成が完了しました!"
                    ErrorSendPWWrongLoginOrEmailAddress = "ログインまたはメールアドレスが間違っています。<br>正しい値を入力し、パスワード送付プロセスを開始してください。"
                    ErrorCookiesMustNotBeDisabled = "現在ご使用中のブラウザではcookieをサポートしていないか、ブラウザのセキュリティポリシーではcookieの使用が不可能となっています。"
                    ErrorTimoutOrLoginFromAnotherStation = "セッションが時間切れとなりました。または他のステーションからのログインがありました。"
                    ErrorApplicationConfigurationIsEmpty = "このアプリケーションは正しく設定されていません。アプリケーションメーカーにご連絡ください。"
                    InfoUserLoggedOutSuccessfully = "ログアウトが完了しました。ご利用ありがとうございました。"
                    UserManagementEMailColumnTitleLogin = "ログイン名: "
                    UserManagementEMailColumnTitleCompany = "会社名: "
                    UserManagementEMailColumnTitleName = "氏名: "
                    UserManagementEMailColumnTitleEMailAddress = "メール: "
                    UserManagementEMailColumnTitleStreet = "住所: "
                    UserManagementEMailColumnTitleZIPCode = "郵便番号: "
                    UserManagementEMailColumnTitleLocation = "市郡区名: "
                    UserManagementEMailColumnTitleState = "都道府県名: "
                    UserManagementEMailColumnTitleCountry = "国名: "
                    UserManagementEMailColumnTitle1stLanguage = "第1言語: "
                    UserManagementEMailColumnTitle2ndLanguage = "第2言語: "
                    UserManagementEMailColumnTitle3rdLanguage = "第3言語: "
                    UserManagementEMailColumnTitleComesFrom = "出身地: "
                    UserManagementEMailColumnTitleMotivation = "動機: "
                    UserManagementEMailColumnTitleCustomerNo = "顧客番号: "
                    UserManagementEMailColumnTitleSupplierNo = "サプライヤー番号: "
                    UserManagementEMailColumnTitleComment = "コメント: "
                    UserManagementAddressesMr = "様 "
                    UserManagementAddressesMs = "様 "
                    UserManagementEMailTextRegards = "敬具"
                    UserManagementEMailTextSubject = "お客様のログイン名"
                    UserManagementEMailTextSubject4AdminNewUser = "Secured Area - 新ユーザー"
                    UserManagementMasterServerAvailableInNearFuture = "ご注意: このサーバーのご使用は近日中に可能となります。"
                    CreateAccount_MsgEMailWelcome = "Secured Areaへようこそ! ここでは毎日最新情報をお届けしています!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のログイン名: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "広範囲におよぶサポートのメリットを是非ご利用ください! 当社のエクストラネットでは多数の情報をご提供しています。 ご自由にこのサイト上の情報をご覧になり、ご探索ください。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のアカウントは様々な保護プログラムへのアクセスを可能としています。Secured Areaの登録プロセスは完了しています。完全な登録承認は3～4日以内に完了します。その後、以下のURLを改めてご利用ください: " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Secured Areaへようこそ! ここでは毎日最新情報をお届けしています!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のログイン名: [n:0]    " & ChrW(13) & ChrW(10) & _
                "お客様のパスワード: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "できるだけ早くパスワードを変更してください。このパスワードを誰かに教えたり使用させたりしないでください。!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "広範囲におよぶサポートのメリットを是非ご利用ください! 当社のエクストラネットでは多数の情報をご提供しています。ご自由にこのサイト上の情報をご覧になり、ご探索ください。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のアカウントは様々な保護プログラムへのアクセスを可能としています。Secured Areaの登録プロセスは完了しています。完全な登録承認は3～4日以内に完了します。その後、以下のURLを改めてご利用ください:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMail4Admin = "以下のユーザーがSecured Areaに新登録されました。" & ChrW(13) & ChrW(10) & _
                "関連する権限を譲渡してください!" & ChrW(13) & ChrW(10) & _
                "権限の調整には" & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " をご利用ください!"
                    UserManagement_NewUser_MsgEMail4Admin = "お客様ご自身または同僚の方により、以下のユーザーがSecured Areaに新登録されました。" & ChrW(13) & ChrW(10) & _
                "関連する権限を譲渡してください!" & ChrW(13) & ChrW(10) & _
                "権限の調整には" & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & "をご利用ください!"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "当社のSecured Areaへようこそ! ここでは毎日最新情報をお届けしています!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "ユーザー""[n:0]""への権限が作成されました。 ご自由に以下をご利用ください:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "サイバースペース上で皆様にお会いできるのを楽しみにしています。"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "当社のSecured Areaへようこそ!"
                    SendPassword_EMailMessage = "以下には新会員情報が記載されています。 この確認メールおよびお客様のユーザー名、パスワードを記録しておいてください。 なお、本情報は秘密情報としてお取り扱いください。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のSecured Area用パスワード: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "できるだけ早くパスワードを変更してください。このパスワードを誰かに教えたり使用させたりしないでください!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "アカウントは様々な保護プログラムへのアクセスを可能としています。当社のSecured Areaへの登録プロセスは完了しています。その後、以下のURLを改めてご利用ください:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "管理者がお客様のアカウントをリセットしました。 以下には新会員情報が記載されています。この確認メールおよびお客様のユーザー名、パスワードを記録しておいてください。なお、本情報は秘密情報としてお取り扱いください。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "お客様のSecured Area用パスワード: [n:0]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
                "できるだけ早くパスワードを変更してください。このパスワードを誰かに教えたり使用させたりしないでください!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "アカウントは様々な保護プログラムへのアクセスを可能としています。当社のSecured Areaへの登録プロセスは完了しています。その後、以下のURLを改めてご利用ください:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
"[n:1]"
                    HighlightTextIntro = "広範囲におよぶサポートのメリットを是非ご利用ください!"
                    HighlightTextTechnicalSupport = "当社のサービスはまだまだ始まったばかり。状況は毎日変わります。"
                    HighlightTextExtro = "当社のエクストラネットでは多数の情報をご提供しています。ご自由にこのサイト上の情報をご覧になり、ご探索ください。"
                    WelcomeTextWelcomeMessage = "当社のSecured Areaへようこそ!"
                    WelcomeTextFeedbackToContact = "他にご希望になる機能はありますか? エクストラネットに関するご要望・ご指摘は<a href=""mailto:[n:0]"">[n:1]</a>で承っています!"
                    WelcomeTextIntro = "ここでは毎日最新情報をお届けしています!"
                    UserManagementEMailTextDearUndefinedGender = "Dear "
                    UserManagementSalutationUnformalUndefinedGender = "Hello "
                    NavAreaNameLogin = "ログイン"
                    NavLinkNamePasswordRecovery = "パスワードをお忘れですか?"
                    NavLinkNameNewUser = "新ユーザーアカウントの作成"
                    CreateAccount_Descr_MotivItemSupplier = "サプライヤー"
                    UpdateProfile_Descr_MotivItemSupplier = "サプライヤー"
                Case 200
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("it-IT")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("it-IT")
                    UpdateProfile_Descr_Title = "Modificare il profilo"
                    UpdateProfile_Descr_Mobile = "Cellulare"
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Phone = "Telefono"
                    UpdateProfile_Descr_PositionInCompany = "Posizione"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Per continuare compili tutti i campi obbligatori!"
                    UpdateProfile_ErrMsg_MistypedPW = "Password errata. La preghiamo di controllare anche le maiuscole e le minuscole!"
                    UpdateProfile_ErrMsg_Undefined = "Risposta imprevista! - La preghiamo di contattare il nostro WebMaster!"
                    UpdateProfile_ErrMsg_Success = "Il Suo profilo utente è stato modificato con successo!"
                    UpdateProfile_ErrMsg_LogonTooOften = "Il processo di connessione non è riuscito troppe volte; il conto è stato disabilitato.<br>La preghiamo di riprovare più tardi!"
                    UpdateProfile_ErrMsg_NotAllowed = "Lei non è autorizzato a svolgere questa azione!"
                    UpdateProfile_ErrMsg_PWRequired = "La preghiamo di introdurre la Sua password per aggiornare il profilo!"
                    UpdateProfile_Descr_Address = "Indirizzo"
                    UpdateProfile_Descr_Company = "Ditta"
                    UpdateProfile_Descr_Addresses = "Appellativo"
                    UpdateProfile_Descr_PleaseSelect = "(Si prega di selezionare!)"
                    UpdateProfile_Abbrev_Mister = "Signor"
                    UpdateProfile_Abbrev_Miss = "Signora"
                    UpdateProfile_Descr_AcademicTitle = "Titolo accademico (ad es. ""Dr."")"
                    UpdateProfile_Descr_FirstName = "Nome"
                    UpdateProfile_Descr_LastName = "Cognome"
                    UpdateProfile_Descr_NameAddition = "Secondo nome"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_UserDetails = "Dati utente"
                    UpdateProfile_Descr_1stLanguage = "1a lingua preferita"
                    UpdateProfile_Descr_2ndLanguage = "2a lingua preferita"
                    UpdateProfile_Descr_3rdLanguage = "3a lingua preferita"
                    UpdateProfile_Descr_Authentification = "Per favore confermi le modifiche con la Sua password"
                    UpdateProfile_Descr_Password = "Password"
                    UpdateProfile_Descr_Submit = "Aggiornare il profilo"
                    UpdateProfile_Descr_RequiredFields = "* campi obbligatori"
                    UpdateProfile_Descr_CustomerSupplierData = "Dati cliente o fornitore"
                    UpdateProfile_Descr_CustomerNo = "Cod.cliente."
                    UpdateProfile_Descr_SupplierNo = "Cod.fornitore"
                    UserJustCreated_Descr_AccountCreated = "Il Suo conto è stato creato con successo!"
                    UserJustCreated_Descr_LookAroundNow = "Può continuare a navigare."
                    UserJustCreated_Descr_PleaseNote = "Osservi: momentaneamente Lei è <font color=""#336699"">membro del settore pubblico</font>. Riceverà altri abbonamenti ed altri diritti d'accesso entro i prossimi 3 - 4 giorni lavorativi."
                    UserJustCreated_Descr_Title = "Benvenuto!"
                    UpdatePW_Descr_Title = "Reset della password"
                    UpdatePW_ErrMsg_ConfirmationFailed = "La conferma della password non corrisponde a quella nuova. Si assicuri che la password e la conferma della password siano uguali. Osservi anche che per la password ci sono differenze se si utilizza il maiuscolo o il minuscolo."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "La preghiamo di inserire la vecchia password e la nuova. Osservi anche che per quanto riguarda la password c'è differenza tra le lettere maiuscole e le minuscole."
                    UpdatePW_ErrMsg_Undefined = "Rilevato errore sconosciuto!"
                    UpdatePW_ErrMsg_Success = "La password è stata modificata con successo!"
                    UpdatePW_ErrMsg_WrongOldPW = "Non è stato possibile modificare la password. Controlli se ha inserito correttamente la password attuale."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Per favore, compili tutti i campi obbligatori per concludere la modifica della password!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Le preghiamo di indicare la Sua password attuale e la Sua nuova password:"
                    UpdatePW_Descr_CurrentPW = "Password attuale"
                    UpdatePW_Descr_NewPW = "Nuova password"
                    UpdatePW_Descr_NewPWConfirm = "Conferma nuova password"
                    UpdatePW_Descr_Submit = "Salvare le modifiche"
                    UpdatePW_Descr_RequiredFields = "* campi obbligatori"
                    CreateAccount_Descr_CustomerSupplierData = "Dati clienti o fornitori"
                    CreateAccount_Descr_CustomerNo = "Codice cliente"
                    CreateAccount_Descr_SupplierNo = "Cod.fornitore."
                    CreateAccount_Descr_FollowingError = "Si è verificato il seguente errore:"
                    CreateAccount_Descr_LoginDenied = "Connessione negata!"
                    CreateAccount_Descr_Submit = "Creare un account utente"
                    CreateAccount_Descr_RequiredFields = "Campi obbligatori"
                    CreateAccount_Descr_BackToLogin = "Ritorna alla connessione"
                    CreateAccount_Descr_PageTitle = "Creare un nuovo utente"
                    CreateAccount_Descr_UserLogin = "Connessione"
                    CreateAccount_Descr_NewLoginName = "Il Suo nuovo nome utente"
                    CreateAccount_Descr_NewLoginPassword = "La Sua nuova password"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Conferma password"
                    CreateAccount_Descr_Address = "Indirizzo"
                    CreateAccount_Descr_Company = "Azienda"
                    CreateAccount_Descr_Addresses = "Appellativo"
                    CreateAccount_Descr_PleaseSelect = "(Si prega di selezionare!)"
                    CreateAccount_Descr_AcademicTitle = "Titolo accademico (ad es. ""Dr."")"
                    CreateAccount_Descr_FirstName = "Nome"
                    CreateAccount_Descr_LastName = "Cognome"
                    CreateAccount_Descr_NameAddition = "Secondo nome"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Via"
                    CreateAccount_Descr_ZIPCode = "CAP"
                    CreateAccount_Descr_Location = "Città"
                    CreateAccount_Descr_State = "Stato"
                    CreateAccount_Descr_Country = "Paese"
                    CreateAccount_Descr_Motivation = "Qual è la Sua motivazione per la registrazione"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Visitatore sito web"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Visitatore sito web"
                    CreateAccount_Descr_MotivItemDealer = "Concessionario"
                    UpdateProfile_Descr_MotivItemDealer = "Concessionario"
                    CreateAccount_Descr_MotivItemJournalist = "Giornalista"
                    UpdateProfile_Descr_MotivItemJournalist = "Giornalista"
                    CreateAccount_Descr_MotivItemOther = "Altro, si prega di indicare"
                    UpdateProfile_Descr_MotivItemOther = "Altro, si prega di indicare"
                    CreateAccount_Descr_WhereHeard = "Dove ha saputo della nostra Secured Area"
                    CreateAccount_Descr_WhereItemFriend = "Da un amico"
                    UpdateProfile_Descr_WhereItemFriend = "Da un amico"
                    CreateAccount_Descr_WhereItemResellerDealer = "Rivenditore/Concessionario"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Rivenditore/Concessionario"
                    CreateAccount_Descr_WhereItemExhibition = "Esposizione/Fiera"
                    UpdateProfile_Descr_WhereItemExhibition = "Esposizione/Fiera"
                    CreateAccount_Descr_WhereItemMagazines = "Giornale"
                    UpdateProfile_Descr_WhereItemMagazines = "Giornale"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "Da noi"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "Da noi"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Motore di ricerca, si prega di indicare"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Motore di ricerca, si prega di indicare"
                    CreateAccount_Descr_WhereItemOther = "Altro, si prega di indicare"
                    UpdateProfile_Descr_WhereItemOther = "Altro, si prega di indicare"
                    CreateAccount_Descr_UserDetails = "Dati utente"
                    CreateAccount_Descr_Comments = "Commenti"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Richieste di autorizzazioni supplementari"
                    CreateAccount_Descr_1stPreferredLanguage = "1a lingua preferita"
                    CreateAccount_Descr_2ndPreferredLanguage = "2a lingua preferita"
                    CreateAccount_Descr_3rdPreferredLanguage = "3a lingua preferita"
                    CreateAccount_ErrorJS_InputValue = "Si prega di introdurre un valore nel campo \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Si prega di introdurre un valore nel campo \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Si prega di introdurre un valore con almeno [n:0] cifre nel campo \""[n:1]\""."
                    UpdateProfile_ErrorJS_Length = "Si prega di introdurre un valore con almeno [n:0] cifre nel campo \""[n:1]\""."
                    Banner_Help = "Aiuto"
                    Banner_HeadTitle = "Secured Area - Logon"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Logon"
                    Banner_Feedback = "Feedback"
                    Logon_HeadTitle = "Secured Area - Connessione"
                    Logon_AskForForcingLogon = "Attenzione! Lei ha già un'altra sessione aperta. Vuole chiuderla ed aprirne una nuova?"
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Connessione"
                    Logon_SSO_ADS_PageTitle = "Impostazione del login automatico"
                    Logon_SSO_ADS_IdentifiedUserName = "Lei è stato identificato come utente <strong>{0}</strong>."
                    Logon_SSO_ADS_LabelTakeAnAction = "Cosa vorrebbe fare?"
                    Logon_SSO_ADS_RadioRegisterExisting = "Si registri per un conto utente <strong>già esistente</strong>"
                    Logon_SSO_ADS_RadioRegisterNew = "Si registri con un conto utente <strong>nuovo</strong>"
                    Logon_SSO_ADS_RadioDoNothing = "Se l'identificazione non fosse corretta o se volesse continuare senza login, proceda come utente anonimo (questa finestra di dialogo Le verrà mostrata successivamente)."
                    Logon_SSO_ADS_ContactUs = "Se dovesse avere domande, <a href=""mailto:{0}"">ci contatti</a>."
                    Logon_SSO_ADS_ButtonNext = "Continua"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Nome di login:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Password:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Ridigitare la password:"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "indirizzo e-mail:"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Lei è stato identificato come utente <strong>{0} ({1})</strong>."
                    Logon_BodyPrompt2User = "Per favore inserisca il Suo nome utente e la relativa password, per accedere all'" & OfficialServerGroup_Title & ".<br><em>Osservi che il nome utente e la password possono essere diversi da altri dati di accesso da Lei ricevuti per altri settori.</em>"
                    Logon_BodyFormUserName = "Nome utente"
                    Logon_BodyFormUserPassword = "Password"
                    Logon_BodyFormSubmit = "Login"
                    Logon_BodyFormCreateNewAccount = "Creare un nuovo conto"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Non è ancora un membro? Crei comunque il Suo conto per il settore " & OfficialServerGroup_Title & "!</STRONG><BR>" & _
                                    "Se non possiede ancora i dati di accesso, può crearlo adesso </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>" & _
                                    "</FONT></A><FONT face=Arial size=2>. " & _
                                    "La preghiamo di non creare altri dati di accesso, " & _
                                    "se ne ha già creati in passato. Se dovesse incontrare difficoltà nella connessione, ci contatti tramite il nostro <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>Support Service Center</FONT></A> " & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Ha dimenticato la password? Gliela inviamo " & _
                                    "</B><BR>Lei ha già ricevuto dati di accesso validi, tuttavia non ha più la password?" & _
                                    "</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2Qui riceve la Sua password via e-mail</FONT></A><FONT " & _
                                    "face=Arial size=2>. La preghiamo di osservare che l'e-mail viene inviata all'indirizzo e-mail da Lei originariamente indicato" & _
                                    ".<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Non ha ottenuto una risposta?</strong><br>Se avesse necessità di un ulteriore supporto, può contattarci </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2></FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    Logon_Connecting_InProgress = "Lei sarà collegato al server…"
                    Logon_Connecting_LoginTimeout = "Superamento del tempo di attesa in fase di login."
                    Logon_Connecting_RecommendationOnTimeout = "Se questo problema dovesse ripetersi, ci <a href=""mailto:[0]"">contatti</a>."
                    AccessError_Descr_FollowingError = "Si è verificato il seguente errore:"
                    AccessError_Descr_LoginDenied = "Connessione negata!"
                    AccessError_Descr_BackToLogin = "Ritorna alla connessione"
                    SendPassword_Descr_FollowingError = "Der folgende Fehler trat auf:"
                    SendPassword_Descr_LoginDenied = "Connessione negata!"
                    SendPassword_Descr_Title = "Richiedere la password Secured Area via e-mail"
                    SendPassword_Descr_LoginName = "Nome utente"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Inviare e-mail"
                    SendPassword_Descr_RequiredFields = "Campi obbligatori"
                    SendPassword_Descr_BackToLogin = "Ritorno alla connessione"
                    SendPassword_Descr_PasswordSentTo = "La password è stata inviata a {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "La Sua password Secured Area verrà ora inviata all'indirizzo e-mail registrato.<BR>Se non dovesse ricevere questa e-mail entro le prossime 24 ore, contatti <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "IT"
                    StatusLineUsername = "Utente"
                    StatusLinePassword = "Password"
                    StatusLineSubmit = "Login"
                    StatusLineEditorial = "Impressum"
                    StatusLineContactUs = "Contatto"
                    StatusLineDataprotection = "Protezione dei dati"
                    StatusLineLoggedInAs = "Connesso come"
                    StatusLineLegalNote = "Protezione dei dati ed informazioni legali"
                    StatusLineCopyright_AllRightsReserved = "Tutti i diritti riservati."
                    NavAreaNameYourProfile = "Il Suo profilo"
                    NavLinkNameUpdatePasswort = "Modificare la password"
                    NavLinkNameUpdateProfile = "Modificare il profilo"
                    NavLinkNameLogout = "Disconnettere"
                    NavLinkNameLogin = "Login"
                    NavPointUpdatedHint = "Qui troverà oggetti nuovi o aggiornati"
                    NavPointTemporaryHiddenHint = "Questa applicazione è disattivata temporaneamente per altri utenti. Spesso questo indica che questa applicazione si trova ancora in costruzione."
                    SystemButtonYes = "Sì"
                    SystemButtonNo = "No"
                    SystemButtonOkay = "Ok"
                    SystemButtonCancel = "Annulla"
                    ErrorUserOrPasswordWrong = "Il nome utente o la password non sono corretti o sono scritti in maniera errata o l'accesso è stato negato!<p>La preghiamo di controllare <ul><li>l'ortografia del nome utente e della password (controllare le lettere maiuscole e quelle minuscole!)</li><li>verificare che nome utente e password siano corretti. (Forse ha già ricevuto da noi un altro nome utente/un'altra password che tuttavia non sono validi per questo campo.)</li></ul>"
                    ErrorServerConfigurationError = "Questo server non è impostato correttamente. La preghiamo di consultare il Suo amministratore."
                    ErrorNoAuthorization = "Lei non è autorizzato ad accedere a questo settore."
                    ErrorAlreadyLoggedOn = "Lei è già collegato! Chiuda prima la sessione nell'altra postazione!<br><font color=""red"">Se è sicuro di non avere alcuna connessione aperta, ci invii una breve e-mail <a href=""mailto:[n:0]"">[n:1]</a> menzionando i suoi nomi per il login.</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "La Sua sessione di lavoro è stata chiusa perché si è collegato da un'altra postazione.<br>"
                    ErrorLogonFailedTooOften = "Il processo di connessione non è riuscito troppe volte, il Suo conto è stato disattivato.<br>Riprovi più tardi!"
                    ErrorEmptyPassword = "Non dimentichi di inserire una password!<br>Se non la ricorda, può richiedercela via mail. La preghiamo di consultare il testo nella parte bassa della pagina per ulteriori informazioni."
                    ErrorUnknown = "Errore imprevisto! - La preghiamo di contattare il nostro <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Inserisca i valori in tutti i campi contrassegnati da un asterisco <em>(*)</em>!"
                    ErrorWrongNetwork = "Lei non è autorizzato a collegarsi tramite l'attuale rete di collegamento."
                    ErrorUserAlreadyExists = "Il conto utente esiste già. Selezioni un altro nome per il login!"
                    ErrorLoginCreatedSuccessfully = "Il profilo utente è stato creato con successo!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Nome utente errato o indirizzo e-mail errato.<br>La preghiamo di indicare dati corretti, come sono indicati nel Suo profilo utente."
                    ErrorCookiesMustNotBeDisabled = "Il Suo browser non supporta i cookies o i cookies sono stati disattivati a causa delle impostazioni di sicurezza del Suo Browser."
                    ErrorTimoutOrLoginFromAnotherStation = "Sessione scaduta o connessione di un'altra postazione."
                    ErrorApplicationConfigurationIsEmpty = "Questa applicazione non è stata ancora configurata. La preghiamo di contattare il fabbricante."
                    InfoUserLoggedOutSuccessfully = "Lei si è disconnesso con successo. La ringraziamo per la Sua visita."
                    UserManagementEMailColumnTitleLogin = "ID in linea: "
                    UserManagementEMailColumnTitleCompany = "Ditta: "
                    UserManagementEMailColumnTitleName = "Nome: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Via: "
                    UserManagementEMailColumnTitleZIPCode = "CAP: "
                    UserManagementEMailColumnTitleLocation = "Città: "
                    UserManagementEMailColumnTitleState = "Stato: "
                    UserManagementEMailColumnTitleCountry = "Paese: "
                    UserManagementEMailColumnTitle1stLanguage = "1a lingua preferita: "
                    UserManagementEMailColumnTitle2ndLanguage = "2a lingua preferita: "
                    UserManagementEMailColumnTitle3rdLanguage = "3a lingua preferita: "
                    UserManagementEMailColumnTitleComesFrom = "Da: "
                    UserManagementEMailColumnTitleMotivation = "Motivazione: "
                    UserManagementEMailColumnTitleCustomerNo = "Cod.cliente: "
                    UserManagementEMailColumnTitleSupplierNo = "Cod.fornitore: "
                    UserManagementEMailColumnTitleComment = "Commento: "
                    UserManagementAddressesMr = "Sig. "
                    UserManagementAddressesMs = "Sig.ra "
                    UserManagementSalutationUnformalMasculin = "Ciao "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Signore e Signori, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Gentilissimo Signor "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Ciao, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Ciao "
                    UserManagementSalutationFormulaUnformalGroup = "Ciao, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Gentilissima Signora "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagementEMailTextRegards = "Cordiali saluti"
                    UserManagementEMailTextSubject = "I Suoi dati di accesso alla Secured Area"
                    UserManagementEMailTextSubject4AdminNewUser = "Secured Area - Nuovo utente"
                    UserManagementMasterServerAvailableInNearFuture = "Attenzione: questo server sarà disponibile a breve."
                    CreateAccount_MsgEMailWelcome_WithPassword = "Benvenuto nella Secured Area! Qui trova informazioni quotidiane e locali." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Il Suo ID per la Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "La Sua password: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Non dimentichi di modificare la password al più presto possibile. Questo è necessario per garantire, che nessun altro (ad es. hacker) si impossessi della Sua password e la possa utilizzare!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Approfitti dei vantaggi di un supporto ampio! Una grande quantità di informazioni la sta già aspettando. Guardi!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Il Suo conto Le offre l'accesso a diverse applicazioni protette. Il processo di connessione per la Secured Area si è concluso. Tutte le Sue autorizzazioni tuttavia saranno valide in ca. 3 - 4 giorni lavorativi. Può accedere con il seguente URL alla Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMailWelcome = "Benvenuto nella Secured Area! Qui troverà informazioni quotidiane e locali." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Il Suo ID per la Secured Area: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Richieda i vantaggi del nostro vasto supporto! Guardi qui!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Il Suo conto Le offre l'accesso a diverse applicazioni protette. Il processo di connessione per la Secured Area si è concluso. Tutte le Sue autorizzazioni tuttavia saranno valide in ca. 3 - 4 giorni lavorativi. Può accedere con il seguente URL alla Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMail4Admin = "Il seguente utente si è registrato alla Secured Area." & ChrW(13) & ChrW(10) & _
                "Si prega di assegnare le autorizzazioni corrispondenti!" & ChrW(13) & ChrW(10) & _
                "Per configurare le autorizzazioni di accesso, visiti " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextWelcome = "Benvenuto nella Secured Area! Qui Lei trova informazioni quotidiane e locali." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Un conto per il settore Secured Area è stato realizzato per Lei dalla nostra amministrazione. Questo servizio per Lei è naturalmente gratuito." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Il Suo ID per la Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "La Sua password per la Secured Area è: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Non dimentichi di modificare la password il più presto possibile. Questo è necessario per assicurare che nessun altro (ad es. hacker) si impossessi della Sua password e la utilizzi!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Richieda i vantaggi di un supporto ampio! Tante informazioni l'aspettano già. Guardi!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Il Suo conto Le offre l'accesso alle diverse applicazioni protette. Il processo di connessione per la Secured Area è già concluso. Le sue autorizzazioni saranno tuttavia valide entro ca. 3 - 4 giorni lavorativi. Con il seguente URL Lei può accedere alla Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Benvenuto nella Secured Area! Qui Lei trova informazioni quotidiane e locali.</p>" & _
                "<p>Le è stato allestito un conto per il settore Secured Area da parte della nostra amministrazione. Questo servizio per Lei è naturalmente gratuito.</p>" & _
                "<p><strong>Il Suo ID per la Secured Area: <font color=""red"">[n:0]</font><br>" & _
                "La Sua password per la Secured Area è: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Non dimentichi di modificare la password al più presto possibile. Questo è necessario per garantire che nessun altro (ad es. un hacker) si impossessi della Sua password e la possa utilizzare!</p>" & _
                "<p>Richieda i vantaggi di un supporto ampio! La aspettano già tante informazioni. Guardi!" & _
                "<p>Il Suo conto Le offre l'accesso alle diverse applicazioni protette. Il processo di collegamento per la Secured Area è già concluso. Tutte le sue autorizzazioni diventeranno tuttavia valide solo in ca. 3 - 4 giorni lavorativi. Può accedere con il seguente URL alla Secured Area:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    UserManagement_NewUser_MsgEMail4Admin = "Il seguente utente è stato registrato da Lei o da un Suo collega nel settore Secured Area." & ChrW(13) & ChrW(10) & _
                "La preghiamo di assegnare i diritti d'accesso necessari!" & ChrW(13) & ChrW(10) & _
                "Per impostare i diritti d'accesso, La preghiamo di visitare " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Benvenuto nella Secured Area!"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Benvenuto nella Secured Area! Qui Lei trova informazioni quotidiane e locali." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "I Suoi diritti d'accesso per l'utente ""[n:0]"" sono stati attribuiti. Con il seguente URL Lei può accedere alla Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Arrivederci."
                    SendPassword_EMailMessage = "Di seguito trova i dati del Suo profilo utente. Conservi questa e-mail con nome utente e/o password e tratti queste informazioni in maniera riservata." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "La Sua password per la Secured Area è: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Non dimentichi di modificare la Sua password al più presto possibile. Questo è necessario per assicurare che nessun altro (ad es. hacker) si impossessi della Sua password e la possa utilizzare!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Un conto utente Le offre l'accesso alle diverse applicazioni protette. Il processo di connessione per la Secured Area è già concluso. Può accedere alla Secured Area con il seguente URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "Il responsabile della sicurezza ha resettato il Suo conto. Di seguito trova i nuovi dati del profilo utente. La preghiamo di conservare questa mail con nome utente e password e di trattare queste informazioni in maniera riservata." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "La Sua password per la Secured Area è: [n:0]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Non dimentichi di modificare la password il più presto possibile. Questo è necessario per assicurare che nessun altro (ad es. hacker) si impossessi della Sua password e la utilizzi!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Un conto Le offre l'accesso alle diverse applicazioni protette. Il processo di connessione per la Secured Area è già concluso. Con il seguente URL può accedere alla Secured Area:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Approfitti dei vantaggi di un ampio supporto!"
                    HighlightTextTechnicalSupport = "Stiamo cominciando a cambiare le cose."
                    HighlightTextExtro = "Una quantità innumerevole di informazione L'aspetta già. Ne approfitti!"
                    WelcomeTextWelcomeMessage = "Benvenuto nella Secured Area!"
                    WelcomeTextFeedbackToContact = "Necessita di altre funzioni? Nessun problema! Ci invii i Suoi commenti sulla extranet a <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "Qui Lei trova informazioni quotidiane e locali."
                    UpdateProfile_Descr_Street = "Via"
                    UpdateProfile_Descr_ZIPCode = "CAP"
                    UpdateProfile_Descr_Location = "Città"
                    UpdateProfile_Descr_State = "Stato"
                    UpdateProfile_Descr_Country = "Paese"
                    UpdatePW_Error_PasswordComplexityPolicy = "La password deve essere composta da almeno 3 caratteri. Non possono essere utilizzati componenti del Suo nome."
                    ErrorRequiredField = "Campo obbligatorio"
                    UserManagementEMailTextDearUndefinedGender = "Gentile/Caro "
                    UserManagementSalutationUnformalUndefinedGender = "Ciao "
                    NavAreaNameLogin = "Login"
                    NavLinkNamePasswordRecovery = "Ha dimenticato la password?"
                    NavLinkNameNewUser = "Creare un nuovo conto"
                    CreateAccount_Descr_MotivItemSupplier = "Fornitore "
                    UpdateProfile_Descr_MotivItemSupplier = "Fornitore "
                Case 180
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("hu-HU")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("hu-HU")
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Mobile = "Mobiltelefon"
                    UpdateProfile_Descr_Phone = "Telefon"
                    UpdateProfile_Descr_PositionInCompany = "Helyzet"
                    UpdateProfile_Descr_Title = "Felhasználó profilt megváltoztatni"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Adja meg az adatokat minden szükséges mezőbe!"
                    UpdateProfile_ErrMsg_MistypedPW = "Hibás jelszó ügyeljen a kis- és nagybetűkre!"
                    UpdateProfile_ErrMsg_Undefined = "Nem várt visszaadott érték - lépjen kapcsolatba honlap szerkesztőnkkel!"
                    UpdateProfile_ErrMsg_Success = "Felhasználói profilját sikeresen megváltoztattukt!"
                    UpdateProfile_ErrMsg_LogonTooOften = "A bejelentkezési folyamat gyakran hibás. A felhasználói kontó átmenetileg deaktiválva.<br> Próbálja később!"
                    UpdateProfile_ErrMsg_NotAllowed = "Önnek nincs elég jogosultsága ezt az akciót keresztülvinni!"
                    UpdateProfile_ErrMsg_PWRequired = "Adja meg jelszavát is a profil aktualizálásához!"
                    UpdateProfile_Descr_Address = "Cím"
                    UpdateProfile_Descr_Company = "Cég"
                    UpdateProfile_Descr_Addresses = "Megszólítás"
                    UpdateProfile_Descr_PleaseSelect = "(Tárcsázzon)"
                    UpdateProfile_Abbrev_Mister = "Úr"
                    UpdateProfile_Abbrev_Miss = "Hölgy"
                    UpdateProfile_Descr_AcademicTitle = "Akadémiai cím (pl ""Dr."")"
                    UpdateProfile_Descr_FirstName = "Előnév"
                    UpdateProfile_Descr_LastName = "Utónéve"
                    UpdateProfile_Descr_NameAddition = "Név toldalék"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_UserDetails = "Felhasználói részletek"
                    UpdateProfile_Descr_1stLanguage = "1. előnyben részesített nyelv"
                    UpdateProfile_Descr_2ndLanguage = "2. előnyben részesített nyelv"
                    UpdateProfile_Descr_3rdLanguage = "3. előnyben részesített nyelv"
                    UpdateProfile_Descr_Authentification = "Jelszavával igazolja a változásokat"
                    UpdateProfile_Descr_Password = "Jelszó"
                    UpdateProfile_Descr_Submit = "Profilt naprakészre tenni"
                    UpdateProfile_Descr_RequiredFields = "* szükséges mezők"
                    UpdateProfile_Descr_CustomerSupplierData = "Vevő és szállító adatok"
                    UpdateProfile_Descr_CustomerNo = "Vevőszám."
                    UpdateProfile_Descr_SupplierNo = "Szállítói szám"
                    UserJustCreated_Descr_AccountCreated = "Az felhasználói kontója sikeresen létrehozva!"
                    UserJustCreated_Descr_LookAroundNow = "Tud haladni, nézzen körül egy kicsit."
                    UserJustCreated_Descr_PleaseNote = "Ügyeljen rá: jelenleg a nyitott tartomány <font color=""#336699"">tagja</font>. Kiegészítő tagságot és hozzáférhetőséget 3-4 munkanap múlva kap."
                    UserJustCreated_Descr_Title = "Üdvözöljük!"
                    UpdatePW_Descr_Title = "Jelszót visszaállítani"
                    UpdatePW_ErrMsg_ConfirmationFailed = "A jelszó visszaigazolása nem stimmel az új jelszóval. A jelszót és a jelszó igazolást egyszerre adja meg ügyeljen arra, hogy megkülönböztesse a nagy-és kis betűket."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Adja meg régi és új jelszavát. Ügyeljen arra, hogy megkülönbözesse a nagy-és kis betűket."
                    UpdatePW_ErrMsg_Undefined = "Ismeretlen hiba felfedezve!"
                    UpdatePW_ErrMsg_Success = "Jelszava eredményesen megváltoztatva!"
                    UpdatePW_ErrMsg_WrongOldPW = "A jelszót nem lehetett megváltoztatni. Ellenőrizze, hogy jelszavát pontosan adta-e meg."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Adjon meg mindn szükséges mezőt, hogy a jelszó változást lezárhassuk!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Adja meg aktuális és új jelszavát:"
                    UpdatePW_Descr_CurrentPW = "Aktuális jelszót"
                    UpdatePW_Descr_NewPW = "Új jelszót"
                    UpdatePW_Descr_NewPWConfirm = "Új jelszót megismételni"
                    UpdatePW_Descr_Submit = "Változásokat menteni"
                    UpdatePW_Descr_RequiredFields = "* szükséges mezők"
                    CreateAccount_Descr_CustomerSupplierData = "Vevő- ill. szállítói adatok"
                    CreateAccount_Descr_CustomerNo = "Vevőszám"
                    CreateAccount_Descr_SupplierNo = "Szállító száma"
                    CreateAccount_Descr_FollowingError = "A következő hiba lépett fel:"
                    CreateAccount_Descr_LoginDenied = "Bejelentkezés megtagadva!"
                    CreateAccount_Descr_Submit = "Fehasználói adatbank készítése"
                    CreateAccount_Descr_RequiredFields = "szükséges mezők"
                    CreateAccount_Descr_BackToLogin = "Vissza a bejelentkezáshez"
                    CreateAccount_Descr_PageTitle = "Új felhasználói kontót előállítani"
                    CreateAccount_Descr_UserLogin = "Bejelentkezési adatok"
                    CreateAccount_Descr_NewLoginName = "Az Ön új felhasználói neve"
                    CreateAccount_Descr_NewLoginPassword = "Az Ön új jelszava"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Jelszóigazolás"
                    CreateAccount_Descr_Address = "Címadatok"
                    CreateAccount_Descr_Company = "Cég"
                    CreateAccount_Descr_Addresses = "Megszólítás"
                    CreateAccount_Descr_PleaseSelect = "Kérjük válasszon"
                    CreateAccount_Descr_AcademicTitle = "Akadémiai cím (pl. ""Dr."")"
                    CreateAccount_Descr_FirstName = "Keresztnév"
                    CreateAccount_Descr_LastName = "Vezetéknév"
                    CreateAccount_Descr_NameAddition = "Név kiegészítés"
                    CreateAccount_Descr_Email = "E-mail"
                    CreateAccount_Descr_Street = "Utca"
                    CreateAccount_Descr_ZIPCode = "Irányító szám"
                    CreateAccount_Descr_Location = "Helység"
                    CreateAccount_Descr_State = "Tartomány/Kanton"
                    CreateAccount_Descr_Country = "Ország"
                    CreateAccount_Descr_Motivation = "Mi motiválja a regisztrálásra"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Weboldal látogató"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Weboldal látogató"
                    CreateAccount_Descr_MotivItemDealer = "Kereskedő"
                    UpdateProfile_Descr_MotivItemDealer = "Kereskedő"
                    CreateAccount_Descr_MotivItemJournalist = "Újságíró"
                    UpdateProfile_Descr_MotivItemJournalist = "Újságíró"
                    CreateAccount_Descr_MotivItemOther = "Egyebek, kérjük megadni"
                    UpdateProfile_Descr_MotivItemOther = "Egyebek, kérjük megadni"
                    CreateAccount_Descr_WhereHeard = "Hogy figyelt fel a biztonsági zónára"
                    CreateAccount_Descr_WhereItemFriend = "Egy barát javaslatára"
                    UpdateProfile_Descr_WhereItemFriend = "Egy barát javaslatára"
                    CreateAccount_Descr_WhereItemResellerDealer = "Viszonteladó/Kereskedő"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Viszonteladó/Kereskedő"
                    CreateAccount_Descr_WhereItemExhibition = "Kiállítás/Vásár"
                    UpdateProfile_Descr_WhereItemExhibition = "Kiállítás/Vásár"
                    CreateAccount_Descr_WhereItemMagazines = "Újság"
                    UpdateProfile_Descr_WhereItemMagazines = "Újság"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "Egy kolléga említésére"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "Egy kolléga említésére"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Keresőgép, kérjük megadni"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Keresőgép, kérjük megadni"
                    CreateAccount_Descr_WhereItemOther = "Egyebek, kérjük megadni"
                    UpdateProfile_Descr_WhereItemOther = "Egyebek, kérjük megadni"
                    CreateAccount_Descr_UserDetails = "Felhasználói adatok"
                    CreateAccount_Descr_Comments = "Kommentárok"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Kérdések kiegészítő jogosultságokhoz"
                    CreateAccount_Descr_1stPreferredLanguage = "1. kedvezményezett nyelv"
                    CreateAccount_Descr_2ndPreferredLanguage = "2. kedvezményezett nyelv"
                    CreateAccount_Descr_3rdPreferredLanguage = "3. kedvezményezett nyelv"
                    CreateAccount_ErrorJS_InputValue = "Adjon meg egy értéket a \""[n:0]\"" mezőbe."
                    UpdateProfile_ErrorJS_InputValue = "Adjon meg egy értéket a \""[n:0]\"" mezőbe."
                    CreateAccount_ErrorJS_Length = "Adjon meg egy értéket minimum [n:0] jellel a \""[n:1]\"" mezőbe."
                    UpdateProfile_ErrorJS_Length = "Adjon meg egy értéket minimum [n:0] jellel a \""[n:1]\"" mezőbe."
                    Banner_Help = "Segítség"
                    Banner_HeadTitle = "Biztonsági terület - Logon"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Logon"
                    Banner_Feedback = "Visszacsatolás"
                    Logon_HeadTitle = "Secured Area - Bejelentkezés"
                    Logon_AskForForcingLogon = "Figyelem! Ön már egy rendszerbe bejelentkezett! Be akarja fejezni, vagy újra kezdeni?"
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Bejelentkezés"
                    Logon_SSO_ADS_ButtonNext = "Készre állítani"
                    Logon_SSO_ADS_ContactUs = "Ha kérdései vannak, lépjen velünk kapcsolatba!, <a href=""mailto:{0}""></a>."
                    Logon_SSO_ADS_IdentifiedUserName = "Önt mint <strong>{0} ({1})</strong> nevű felhasználót azonosították"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Önt mint <strong>{0} ({1})</strong> nevű felhasználót azonosították"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Login-név:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Jelszó:"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "e-mail cím:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Jelszó (ismétlés):"
                    Logon_SSO_ADS_LabelTakeAnAction = "Mit szeretne tenni??"
                    Logon_SSO_ADS_PageTitle = "Az automatikus Login berendezése"
                    Logon_SSO_ADS_RadioDoNothing = "Ha az azonosítás nem korrekt vagy login nélkül akar tovább menni, lépjen be anoním felhasználóként (Ezt a párbeszédet később megmutatjuk Önnek)"
                    Logon_SSO_ADS_RadioRegisterExisting = "Regisztrálja magát <strong>már egy létező</strong> felhasználói kontóra!"
                    Logon_SSO_ADS_RadioRegisterNew = "Regisztrálja magát egy <strong>új</strong> felhasználói kontóra!"
                    Logon_BodyPrompt2User = "Adja meg felhasználói nevét és az ehhez tertozó jelszót, hogy beléphessen a következőbe: " & OfficialServerGroup_Title & " .<br><em>Ügyeljen rá, hogy a felhasználói nevet és a jelszót meg lehessen különböztetni az egyéb hozzáférési adatoktól, melyeket másik tartományokra kapott.</em>"
                    Logon_BodyFormUserName = "Felhasználói név"
                    Logon_BodyFormUserPassword = "Jelszó"
                    Logon_BodyFormSubmit = "Login"
                    Logon_BodyFormCreateNewAccount = "Felhasználói kontót előállítani"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Még nem tag?Állítsa elő saját hozzáférési kontóját a következőhöz: " & OfficialServerGroup_Title & "!</STRONG><BR>" & _
"Ha nincsenek hozzáférési adatai, most előállíthatja azokat: </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>jetzt " & _
"</FONT></A><FONT face=Arial size=2>. " & _
"Ne állítson elő másik hozzáférési adatokat " & _
"ha a múltban valamit előállított. Ha nehézségei vannak a bejelentkezéskor lépjen velünk kapcsolatbar <A " & _
"href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>Support Service Center</FONT></A> " & _
". <BR> &nbsp;</FONT></P></TD></TR>" & _
"<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Elfelejtette jelszavát? Küldjük jelszavát " & _
"</B><BR>Már megkapta érvényes hozzáférési adatait, mégse tudja jelszavát?" & _
"</FONT> <A " & _
"href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
"face=Arial size=2>Jelszavát E-mailen keresztül kapja meg</FONT></A><FONT " & _
"face=Arial size=2>Ügyeljen rá, hogy e mailek csak arra a címre menjenek, melyet Ön eredetileg megadott.  " & _
".<br> &nbsp;</FONT></P></TD></TR>" & _
"<TR><TD VALIGN=""TOP""><A " & _
"href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Nem kapott itt kérdésére választ?</strong><br>Ha kiegészitő segítségre van szüksége, </FONT><A " & _
"href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>lépjen velünk kapcsolatba</FONT></A><FONT " & _
"size=2>.</FONT></P></TD></TR></TABLE>"
                    Logon_Connecting_InProgress = "Ön össze van kötve a szerverrel…"
                    Logon_Connecting_LoginTimeout = "Időtúllépés a loginnál."
                    Logon_Connecting_RecommendationOnTimeout = "Ha ez a probléma még egyszer fellép, lépjen velünk kapcsolatba."
                    AccessError_Descr_FollowingError = "A következő hiba lépett fel:"
                    AccessError_Descr_LoginDenied = "Bejelentkezés megtagadva!"
                    AccessError_Descr_BackToLogin = "Vissza a bejelentkezáshez"
                    SendPassword_Descr_FollowingError = "A következő hiba lépett fel:"
                    SendPassword_Descr_LoginDenied = "Bejelentkezése elutasítva!"
                    SendPassword_Descr_Title = "Biztonsági terület jelszavát kérni e-mail-en keresztül"
                    SendPassword_Descr_LoginName = "Felhasználó név"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "e-mail-t elküldeni"
                    SendPassword_Descr_RequiredFields = "szükséges mezők"
                    SendPassword_Descr_BackToLogin = "Vissza a bejelentkezéshez"
                    SendPassword_Descr_PasswordSentTo = "Jelszó elküldve {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "Biztonsági területi jelszavát csak az Ön által megadott e-mail címre küldjük.<BR>Ha 24 órán belül nem kapja meg ezt az e-mail üzenetet, keresse <a href=""mailto:{0}"">{1}</a>-t."
                    META_CurrentContentLanguage = "HU"
                    StatusLineUsername = "Felhasználó"
                    StatusLinePassword = "Jelszó"
                    StatusLineSubmit = "Login"
                    StatusLineEditorial = "Impresszum"
                    StatusLineContactUs = "Kapcsolat"
                    StatusLineDataprotection = "Adatvédelem"
                    StatusLineLoggedInAs = "Bejelentkezve mint"
                    StatusLineLegalNote = "Adatvédelem és jogi utalások"
                    StatusLineCopyright_AllRightsReserved = "Adatvédelem."
                    NavAreaNameYourProfile = "Az Ön profilja"
                    NavLinkNameUpdatePasswort = "Jelszót megváltoztatni"
                    NavLinkNameUpdateProfile = "Felhasználói adatokat megváltoztatni"
                    NavLinkNameLogout = "Lejelentkezni"
                    NavLinkNameLogin = "Login"
                    NavPointUpdatedHint = "Itt valami újat állítottak elő vagy valami meglévőt aktualizáltak"
                    NavPointTemporaryHiddenHint = "Ez az alkalmazás átmenetileg minden felhasználó részére deaktiválva. Ez gyakran egy indiz, ahol az alkalmazás még a fejlesztési fázisban van."
                    SystemButtonYes = "Igen"
                    SystemButtonNo = "Nem"
                    SystemButtonOkay = "Rendben"
                    SystemButtonCancel = "Megszakítani"
                    ErrorUserOrPasswordWrong = "A felhasználó név vagy a jeszó nem korrekt vagy elirták vagy a hozzáférést elutasították!<p>Ellenőrizze a felhasználó név és a jelszó írását. <ul><li> (a jelszó maga megkülönbözteti a nagy és a kis betűket) </li><li>hogy Ö egy érvényes felhasználó nevet vagy jelszót használt.(Vagy tőlünk kapott felhasználó nevet és jelszót, melyek ebben a tartományban még nem érvényesek) </li></ul>"
                    ErrorServerConfigurationError = "Ez a szerver nincs korrekten berendezve. Konzultáljon az adminisztrátorával."
                    ErrorNoAuthorization = "Nincs hozzáférési joga ehhez a területhez."
                    ErrorAlreadyLoggedOn = "Ön már bejelentkezett. Jelentkezzen le a másik munkahelyről!S<br><font color=""red"">Ha biztos benne, hogy soha többé nem jelentkezik, küldjön egy rövid e-mailt    <a href=""mailto:[n:0]"">[n:1]</a> és nevezze meg loginnevét.</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Lejelentkezett erről a munkahelyről, mert egymásik állomásra már bejelentkezett.<br>"
                    ErrorLogonFailedTooOften = "A bejelentkezési folyamat túl gyakran hibás, ezért az Ön kontóját átmenetileg deaktiválták .<br>Később próbálja meg még egyszer!"
                    ErrorEmptyPassword = "Kérjük ne felejtsen el még egy jelszót megadni!<br>Ha nem tudja a jelszavát, e-mailben kérheti. További részleteket a későbbiekben talál."
                    ErrorUnknown = "Nem várt hiba! Lépjen kapcsolatba a következővel: <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Adjon meg értékeket minden olyan mezőbe, melyek csillaggal vannak ellátva <em>(*)</em>!"
                    ErrorWrongNetwork = "Ön nem jogosult az aktuális hálózati kapcsolaton bejelentkezni."
                    ErrorUserAlreadyExists = "Már létezik a felhasználói kontója. Válasszon egy másik bejelentkező nevet!"
                    ErrorLoginCreatedSuccessfully = "A felhasználó profilját sikeresen előállította!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Hibás felhasználói név vagy e-mail cím.<br>Adjon meg korrekt értékeket, ahogy az a felhasználó profiljában van."
                    ErrorCookiesMustNotBeDisabled = "Az Ön böngészője nem támogatja a cookie-ket vagy a cookie-ket biztonsági okokból deaktiválták az Ön böngészőjén."
                    ErrorTimoutOrLoginFromAnotherStation = "Lejelentkezett, mert eléte a maximális időtartamot vagy egy login a másik munkaállomásról lejelentette."
                    ErrorApplicationConfigurationIsEmpty = "Ez az alkalmazás nem tartalmaz érvényes alkalmazói nevet. Lépjen kapcsolatba a gyártóval."
                    InfoUserLoggedOutSuccessfully = "Sikeresen lejelentkezett. Köszönjük látogatását."
                    UserManagementEMailColumnTitleLogin = "Bejelentkezési név: "
                    UserManagementEMailColumnTitleCompany = "Cég: "
                    UserManagementEMailColumnTitleName = "Név: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Utca: "
                    UserManagementEMailColumnTitleZIPCode = "Irányító szám: "
                    UserManagementEMailColumnTitleLocation = "Helység: "
                    UserManagementEMailColumnTitleState = "Állam: "
                    UserManagementEMailColumnTitleCountry = "Ország: "
                    UserManagementEMailColumnTitle1stLanguage = "1. előnyben részesített nyelv: "
                    UserManagementEMailColumnTitle2ndLanguage = "2. előnyben részesített nyelv: "
                    UserManagementEMailColumnTitle3rdLanguage = "3. előnyben részesített nyelv: "
                    UserManagementEMailColumnTitleComesFrom = "Jön a: "
                    UserManagementEMailColumnTitleMotivation = "Motiváció: "
                    UserManagementEMailColumnTitleCustomerNo = "Vevőszám.: "
                    UserManagementEMailColumnTitleSupplierNo = "Szállítói szám.: "
                    UserManagementEMailColumnTitleComment = "Kommentár: "
                    UserManagementAddressesMr = "Úr "
                    UserManagementAddressesMs = "Hölgy "
                    UserManagementSalutationUnformalMasculin = "Tisztelt "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationNameOnly} Úr, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly} Úr, "
                    UserManagementSalutationFormulaInMailsGroup = "Hölgyeim és Uraim, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Tisztelt "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Szia, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Tisztelt "
                    UserManagementSalutationFormulaUnformalGroup = "Szia, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationNameOnly} Úrhölgy, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly} Úrhölgy, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationNameOnly} Úrhölgy"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationNameOnly} Úr"
                    UserManagementEMailTextDearMs = "Tisztelt "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagementEMailTextRegards = "Baráti üdvözlettel"
                    UserManagementEMailTextSubject = "Az Ön hozzáférési adatai a biztonsági területre"
                    UserManagementEMailTextSubject4AdminNewUser = "Biztonsági terület - Új felhasználó"
                    UserManagementMasterServerAvailableInNearFuture = "Figyelem: ez a szerver csk a közel jövőben lesz elérhető."
                    CreateAccount_MsgEMailWelcome = "Üdvözöljük a biztonsági területen. Ittnapi és helyhez kötött információkat talál." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön bejelentkezési neve a biztonsági területre: [n:0]" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Vegye igénybe átfogó támogatásunk előnyeit! íegyszerűen csak pillantson bele!" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön felhasználói kontója hozzáférést kínál a különböző védett területekhez. A bejelentkezési folyamat a biztonsági területre már lezárva. Teljes jogai csak 3-4 munkanap múlva lesznek érvényesek. A következő URL-el tud belépni a biztonsági területre. :" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Üdvözöljük a biztonsági területen. Itt napi és helyhez kötött információkat talál." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön bejelentkezési neve a biztonsági területre: [n:0]    " & ChrW(13) & ChrW(10) & _
"Jelszó: [n:1]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Kérjük ne felejtse el jelszavát amilyen gyorsan csak lehet, megváltoztatni. Ez azért szükséges, hogy senki idegen (pl. hacker) ne birtokolhass vagy használhassa jelszavát." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Használja ki átfogó támogatásunk előnyeit! Már most vár néhány dolog Önre. Egyszerűen csak pillantson bele!" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön felhasználási kontója hozzáférést kínál a különböző védett területekhez..A bejelentkezési folyamat a biztonsági területrer már lezárva. Teljes jogai csak 3-4 munkanap múlva lesznek érvényesek. A következő URL-el tud belépni a biztonsági területre:" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:2]"
                    CreateAccount_MsgEMail4Admin = "Akövetkező felhasználó bejelentkezett a biztonsági területre." & ChrW(13) & ChrW(10) & _
"Adja meg a szükséges hozzáférési jogokat!" & ChrW(13) & ChrW(10) & _
"A hozzáférési jogokat nlétrehozni látogassa meg a  " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextWelcome = "Üdvözöljük a biztonsági területen! Itt napi és helyhez kötött információkat talál." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Ön irodánktól egy berendezett felhasználói kontót kapott a biztonsági területre. Ez a szolgáltatás Önnek természetesen ingyenes." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az ön bejelentkezési neve a biztonsági területre: [n:0]    " & ChrW(13) & ChrW(10) & _
"Az Önjelszava a biztonsági területret: [n:1]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Ne felejtse el jelszavát amilyen gyorsan csak lehet megváltoztatni. Ez azért szükséges, hogy jelszavát senki idegen (pl. hacker) ne tudja birtokolni vagy használni!" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Vegye igénybe átfogó támogatásunk előnyeit! Néhány dolog már most vár Önre!. Egyszerűen nézzen körül!" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön felhasználói kontója hozzáférést kínál különböző védett alkalmazásokhoz. A bejelentkezési folyamat a biztonsági területre már lezárva. Az Ön teljes jogosultsága csak 3-4 munkanap múlva lesz érvényes. A következő URL-el tud belépni a biztonsági területre:" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Üdvözöljük a biztonsági területen! Itt napi és helyhez kötött információkat talál.</p>" & _
"Irodánktól egy berendezett felhasználói kontót kapott a biztonsági területre. Ez a szolgáltatás Önnek természetesen ingyenes.</p>" & _
"<p>Az Ön bejelentkezési neve a biztonsági területre: Area: <font color=""red"">[n:0]</font><br>" & _
"<p>Az Ön jelszava a biztonsági területre: <font color=""red"">[n:1]</font></strong></p>" & _
"<p>Ne felejtse el jelszavát amilyen gyorsan csak lehet megváltoztatni. Ez azért szükséges, hogy jelszavát seni (pl. Hacker) ne birtokolhassa és használhassa</p>" & _
"Vegye igénybe átfogó támogatásunk előnyeit! Néhány dolog már most vár Önre. Egyszerűen nézzen körüli!" & _
"<p>Az Ön felhasználói kontója hozzáférédt kínál a különböző védett alkalmazásokhoz. A bejelentkezési folyamat a biztonsági területre már lezárva. Az Ön teljes jogosultsága csak 3-4 munkanap múlva lesz érvényes. A következő URL-el tud belépni a biztonsági területre:<br>" & _
"<ul><strong>[n:2]</strong></ul></p>"
                    UserManagement_NewUser_MsgEMail4Admin = "A következő felhasználó az Ön vagy valamelyik kollégája részéről jelentkezett be a biztonsági területre." & ChrW(13) & ChrW(10) & _
"Adja meg a szükséges hozzáférési jogokat" & ChrW(13) & ChrW(10) & _
"A hozzáférési jogokat megadni látogassa meg a következőt: " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Üdvözöljük a biztonsági területen!"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Üdvözöljük a biztonsági területen! Itt naprakész és helyhez kötött információkat talál." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön hozzáférési jogai ""[n:0]"" biztosítva. A következő URL-el tud belépni a biztonsági területre:" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:1]" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Örülnénk a viszontlátásnak."
                    SendPassword_EMailMessage = "A következőkben találja a felhasználó profil adatait. Őrizze meg ezt az e-mait a felhasználó névvel / jelszóval és kezelje ezt az információt bizalmasan." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Az Ön jelszava a biztonsági területhez: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
"Ne felejtse el jelszavát aamilyen gyorsan csak lehet, megváltoztatni. Ez azért szükséges, hogy jelszavát senki idegen (pl. hacker) ne tudja birtokolni vagy használni!" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Ez a felhsználói kontó hozzáférést kínál a különböző védett adatokhoz. A bejelentkezés a biztonsági területre már lezárva. A következő URL-el tudja elérni a biztonsági területet:" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:1]"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    UserManagement_ResetPWByAdmin_EMailMsg = "A biztonsági megbizott visszaállította az Ön felhasználói kontóját. A következőkben látja új felhasználási kontójának profil adatait.Őrizze meg ezt az e-mailt a felhasználó névveé jelszóval, kezelje ezt az információt bizalmasan!." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"AZ Ön jelszava a biztonsági területret: [n:0]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Ne felejtse el jelszavát amilyen gyorsan csak lehet megváltoztani. Ez azért szükséges, hogy senki idegen (pl. hacker) ne birtokolhass vagy használhassa jelszavát." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Egy felhasználói konto hozzáférést biztosít a különböző védett alkalmazásokhoz. A bejelentkezési folíamat a biztonsági területre már lezárvaA következő URL-el tud belépni a biztonsági területre. :" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:1]"
                    HighlightTextIntro = "Használja ki átfogó támogatásunk előnyeit!"
                    HighlightTextTechnicalSupport = "Éppen indulunk, néhány dolog meg fog változni."
                    HighlightTextExtro = "Néhány dolog már vár Önre! Használja ki a lehetőséget!"
                    WelcomeTextWelcomeMessage = "Üdvözöljük a biztonsági területen!"
                    WelcomeTextFeedbackToContact = "Szüksége va még további funkciókra? Nem probléma! Küldje el véleményét az extrnethez a következőre: <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "Itt napi és helyhez kötött információkat talál."
                    UpdateProfile_Descr_Street = "Utca"
                    UpdateProfile_Descr_ZIPCode = "Irányítószám"
                    UpdateProfile_Descr_Location = "Helységt"
                    UpdateProfile_Descr_State = "Állam"
                    UpdateProfile_Descr_Country = "Ország"
                    UpdatePW_Error_PasswordComplexityPolicy = "A jelszónak legalább 3 jegyűnek kell lennie. Nevének részei nem használhatók."
                    ErrorRequiredField = "Szükséges mező"
                    UserManagementEMailTextDearUndefinedGender = "Kedves "
                    UserManagementSalutationUnformalUndefinedGender = "Kedves "
                    NavAreaNameLogin = "Bejelentkezni"
                    NavLinkNamePasswordRecovery = "Elfelejtette jelszavát?"
                    NavLinkNameNewUser = "Új kontót készíteni"
                    CreateAccount_Descr_MotivItemSupplier = "Szállító"
                    UpdateProfile_Descr_MotivItemSupplier = "Szállító"
                Case 80
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("zh-CN")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("zh-CN")
                    Logon_AskForForcingLogon = "注意：您已经登录到另一会话。您是否要取消这一会话而开始一新的会话？"
                    UserManagementSalutationUnformalMasculin = "你好"
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Dear Sirs, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "尊敬的先生"
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "嗨！"
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "你好"
                    UserManagementSalutationFormulaUnformalGroup = "嗨！"
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "尊敬的女士"
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagement_NewUser_TextWelcome = "欢迎来到安全区域！这是每天都应该来的地方！" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "我们的管理员已经将您免费地添加到了安全区域。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的登录名称是：[n:0]    " & ChrW(13) & ChrW(10) & _
                "您的安全区域密码是：[n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "为了防止他人得到并使用密码，请您不要忘记更改您的密码！" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您在这里可获得到广泛的支持而获益！在我们的外联网中您可以找到大量的有用信息。您可以在此自由冲浪，检索信息。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的帐号容许您访问多个不同的安全项目。至此为止，您已经完成了登录安全区域的必要步骤。在此后的3至4天内，您将可以使用您的整个权限。届时请访问网址：" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>欢迎来到安全区域！这是每天都应该来的地方！</p>" & _
                "<p>我们的管理员已经将您免费地添加到了安全区域。</p>" & _
                "<p><strong>您的登录名称是：<font color=""red"">[n:0]</font><br>" & _
                "您的安全区域密码是：<font color=""red"">[n:1]</font></strong></p>" & _
                "<p>为了防止他人得到并使用密码，请您不要忘记更改您的密码！</p>" & _
                "<p>您在这里可获得到广泛的支持而获益！在我们的外联网中您可以找到大量的有用信息。您可以在此自由冲浪，检索信息。</p>" & _
                "<p>您的帐号容许您访问多个不同的安全项目。至此为止，您已经完成了登录安全区域的必要步骤。在此后的3至4天内，您将可以使用您的整个权限。届时请访问网址：<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    UpdateProfile_Descr_Title = "更改特征"
                    UpdateProfile_Descr_PositionInCompany = "职务"
                    UpdateProfile_Descr_Phone = "电话"
                    UpdateProfile_Descr_Fax = "传真"
                    UpdateProfile_Descr_Mobile = "手机"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "请填写所有必填项，然后继续！"
                    UpdateProfile_ErrMsg_MistypedPW = "您的密码输入有误！"
                    UpdateProfile_ErrMsg_Undefined = "意外返回值！请和网站管理员联系！"
                    UpdateProfile_ErrMsg_Success = "您的特征已经成功更新！"
                    UpdateProfile_ErrMsg_LogonTooOften = "登录过程失败次数过多，帐号被停用。<br>请稍候再试！"
                    UpdateProfile_ErrMsg_NotAllowed = "您无权访问本文件！"
                    UpdateProfile_ErrMsg_PWRequired = "请提交您的密码以便可以更改您的特征！"
                    UpdateProfile_Descr_Address = "地址"
                    UpdateProfile_Descr_Company = "公司"
                    UpdateProfile_Descr_Addresses = "称呼"
                    UpdateProfile_Descr_PleaseSelect = "（请选择！）"
                    UpdateProfile_Abbrev_Mister = "先生"
                    UpdateProfile_Abbrev_Miss = "女士"
                    UpdateProfile_Descr_AcademicTitle = "学位（如""博士""）"
                    UpdateProfile_Descr_FirstName = "名"
                    UpdateProfile_Descr_LastName = "姓"
                    UpdateProfile_Descr_NameAddition = "附加名"
                    UpdateProfile_Descr_EMail = "电子信箱"
                    UpdateProfile_Descr_Street = "街道"
                    UpdateProfile_Descr_ZIPCode = "邮政编码"
                    UpdateProfile_Descr_Location = "地市"
                    UpdateProfile_Descr_State = "州"
                    UpdateProfile_Descr_Country = "国家"
                    UpdateProfile_Descr_UserDetails = "用户细节"
                    UpdateProfile_Descr_1stLanguage = "第一首选语言"
                    UpdateProfile_Descr_2ndLanguage = "第二首选语言"
                    UpdateProfile_Descr_3rdLanguage = "第三首选语言"
                    UpdateProfile_Descr_Authentification = "请用您的密码确认更改"
                    UpdateProfile_Descr_Password = "密码"
                    UpdateProfile_Descr_Submit = "更新特征"
                    UpdateProfile_Descr_RequiredFields = "*代表必填项"
                    UpdateProfile_Descr_CustomerSupplierData = "客户/供应商数据"
                    UpdateProfile_Descr_CustomerNo = "客户号"
                    UpdateProfile_Descr_SupplierNo = "供应商号"
                    UserJustCreated_Descr_AccountCreated = "您的用户帐号已经成功创建！"
                    UserJustCreated_Descr_LookAroundNow = "您现在可以继续浏览。"
                    UserJustCreated_Descr_PleaseNote = "请您注意：当前<font color=""#336699"">您是公共成员</font>。附加权限和成员身份将在此后的3至4天内设置。"
                    UserJustCreated_Descr_Title = "欢迎访问安全区域！"
                    UpdatePW_Descr_Title = "重置密码"
                    UpdatePW_ErrMsg_ConfirmationFailed = "密码确认失败！请确保您输入的密码是正确的。请注意密码是区分大小写的。"
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "请输入您当前的密码和一个不同于当前密码的新密码。请注意密码是区分大小写的。"
                    UpdatePW_ErrMsg_Undefined = "检测到未定义的错误！"
                    UpdatePW_ErrMsg_Success = "密码已成功更改！"
                    UpdatePW_ErrMsg_WrongOldPW = "无法更改密码！请确保您输入的当前密码是正确的。"
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "要更改密码必须填写所有必填项！"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "请指定您当前的密码和新密码："
                    UpdatePW_Descr_CurrentPW = "当前密码"
                    UpdatePW_Descr_NewPW = "新密码"
                    UpdatePW_Descr_NewPWConfirm = "确认新密码"
                    UpdatePW_Descr_Submit = "更新特征"
                    UpdatePW_Descr_RequiredFields = "*代表必填项"
                    UpdatePW_Error_PasswordComplexityPolicy = "密码必须至少由三个字符组成，并且不能含有您名字的组成成分。"
                    CreateAccount_Descr_CustomerSupplierData = "客户/供应商数据"
                    CreateAccount_Descr_CustomerNo = "客户号"
                    CreateAccount_Descr_SupplierNo = "供应商号"
                    CreateAccount_Descr_FollowingError = "发生以下错误："
                    CreateAccount_Descr_LoginDenied = "登录被拒绝！"
                    CreateAccount_Descr_Submit = "创建用户帐号"
                    CreateAccount_Descr_RequiredFields = "必填项"
                    CreateAccount_Descr_BackToLogin = "返回到登录页面"
                    CreateAccount_Descr_PageTitle = "创建新用户"
                    CreateAccount_Descr_UserLogin = "用户登录"
                    CreateAccount_Descr_NewLoginName = "您的新登录名称"
                    CreateAccount_Descr_NewLoginPassword = "您的新密码"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "确认您的密码"
                    CreateAccount_Descr_Address = "地址"
                    CreateAccount_Descr_Company = "公司"
                    CreateAccount_Descr_Addresses = "称呼"
                    CreateAccount_Descr_PleaseSelect = "（请选择！）"
                    CreateAccount_Descr_AcademicTitle = "学位（如""博士""）"
                    CreateAccount_Descr_FirstName = "名"
                    CreateAccount_Descr_LastName = "姓"
                    CreateAccount_Descr_NameAddition = "附加名"
                    CreateAccount_Descr_Email = "电子信箱"
                    CreateAccount_Descr_Street = "街道"
                    CreateAccount_Descr_ZIPCode = "邮政编码"
                    CreateAccount_Descr_Location = "地市"
                    CreateAccount_Descr_State = "州"
                    CreateAccount_Descr_Country = "国家"
                    CreateAccount_Descr_Motivation = "您注册的动机是什么"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "网站访客"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "网站访客"
                    CreateAccount_Descr_MotivItemDealer = "经销商"
                    UpdateProfile_Descr_MotivItemDealer = "经销商"
                    CreateAccount_Descr_MotivItemJournalist = "新闻记者"
                    UpdateProfile_Descr_MotivItemJournalist = "新闻记者"
                    CreateAccount_Descr_MotivItemOther = "其他，请说明"
                    UpdateProfile_Descr_MotivItemOther = "其他，请说明"
                    CreateAccount_Descr_WhereHeard = "您从何处得知我们的安全区域"
                    CreateAccount_Descr_WhereItemFriend = "朋友"
                    UpdateProfile_Descr_WhereItemFriend = "朋友"
                    CreateAccount_Descr_WhereItemResellerDealer = "零售商/经销商"
                    UpdateProfile_Descr_WhereItemResellerDealer = "零售商/经销商"
                    CreateAccount_Descr_WhereItemExhibition = "展览会"
                    UpdateProfile_Descr_WhereItemExhibition = "展览会"
                    CreateAccount_Descr_WhereItemMagazines = "杂志"
                    UpdateProfile_Descr_WhereItemMagazines = "杂志"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "从我们自己这里"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "从我们自己这里"
                    CreateAccount_Descr_WhereItemSearchEnginge = "搜索引擎，请说明"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "搜索引擎，请说明"
                    CreateAccount_Descr_WhereItemOther = "其他，请说明"
                    UpdateProfile_Descr_WhereItemOther = "其他，请说明"
                    CreateAccount_Descr_UserDetails = "用户细节"
                    CreateAccount_Descr_Comments = "注释"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "申请附加权限"
                    CreateAccount_Descr_1stPreferredLanguage = "第一首选语言"
                    CreateAccount_Descr_2ndPreferredLanguage = "第二首选语言"
                    CreateAccount_Descr_3rdPreferredLanguage = "第三首选语言"
                    CreateAccount_ErrorJS_InputValue = "请将一值输入字段\""[n:0]\""。"
                    UpdateProfile_ErrorJS_InputValue = "请将一值输入字段\""[n:0]\""。"
                    CreateAccount_ErrorJS_Length = "请将一至少为[n:0]字符的值输入字段\""[n:1]\""。"
                    UpdateProfile_ErrorJS_Length = "请将一至少为[n:0]字符的值输入字段\""[n:1]\""。"
                    Banner_Help = "帮助"
                    Banner_HeadTitle = "安全区域登录"
                    Banner_BodyTitle = OfficialServerGroup_Title & "登录"
                    Banner_Feedback = "反馈"
                    Logon_Connecting_RecommendationOnTimeout = "如果再发生同样问题，请和我们联系。"
                    Logon_Connecting_LoginTimeout = "登录超时。"
                    Logon_HeadTitle = "安全区域登录"
                    Logon_Connecting_InProgress = "正在连接服务器…"
                    Logon_BodyTitle = OfficialServerGroup_Title & "登录"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "您被识别为用户<strong>{0} ({1})</strong>。"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "电子信箱："
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "重新输入密码："
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "密码"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "登录名称："
                    Logon_SSO_ADS_ButtonNext = "继续"
                    Logon_SSO_ADS_ContactUs = "如果您有任何问题，请<a href=""mailto:{0}"">和我们联系</a>。"
                    Logon_SSO_ADS_RadioDoNothing = "如果识别错误，或者您决定不登录而继续，则作为匿名用户继续（以后将重新询问您）。"
                    Logon_SSO_ADS_RadioRegisterNew = "注册一个<strong>新</strong>帐号"
                    Logon_SSO_ADS_RadioRegisterExisting = "为一个<strong>现有的</strong>帐户注册"
                    Logon_SSO_ADS_LabelTakeAnAction = "现在您想做什么？"
                    Logon_SSO_ADS_IdentifiedUserName = "您被识别为用户<strong>{0}</strong>。"
                    Logon_SSO_ADS_PageTitle = "设置自动登录"
                    Logon_BodyPrompt2User = "输入您的登录名称和密码访问" & OfficialServerGroup_Title & "。<br><em>请注意，此登录名称和密码是专有的，可能和您已经从我们这里获得的用于其他区域的登录名称以及密码不一样。</em>"
                    Logon_BodyFormUserName = "登录名称"
                    Logon_BodyFormUserPassword = "密码"
                    Logon_BodyFormSubmit = "登录"
                    Logon_BodyFormCreateNewAccount = "创建新帐号"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>您还不是会员？创建一个新的帐号即可访问" & OfficialServerGroup_Title & "！</STRONG><BR>" & _
                                    "如果您还没有帐号，您现在可以</FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>create " & _
                                    "一个</FONT></A><FONT face=Arial size=2>。" & _
                                    "如果您以前已经创建了一个帐号，则请不要创建另一个&nbsp;" & _
                                    "帐号。如果您登录" & _
                                    "有困难，请和您的<A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>支持服务中心</FONT></A>" & _
                                    "联系。<BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>您忘记了密码？将您的密码发送到" & _
                                    "您的电子信箱</B><BR>可能您提供了有效的帐号，但是您忘记了" & _
                                    "您的密码。</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>在此您可以请求将您的密码发送到您的电子信箱</FONT></A><FONT " & _
                                    "face=Arial size=2>。请注意，您的密码" & _
                                    "将通过电子邮件发送到您在创建本帐号时所指定的" & _
                                    "电子信箱。<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>您还有其他问题？</strong><br>如果您需要其他支持，欢迎您</FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>和我们联系</FONT></A><FONT " & _
                                    "size=2>。</FONT></P></TD></TR></TABLE>"
                    AccessError_Descr_FollowingError = "发生以下错误："
                    AccessError_Descr_LoginDenied = "登录被拒绝！"
                    AccessError_Descr_BackToLogin = "返回到登录页面"
                    SendPassword_Descr_FollowingError = "发生以下错误："
                    SendPassword_Descr_LoginDenied = "登录被拒绝！"
                    SendPassword_Descr_Title = "请求通过电子邮件发送安全区域密码"
                    SendPassword_Descr_LoginName = "登录名称"
                    SendPassword_Descr_Email = "电子邮件"
                    SendPassword_Descr_Submit = "发送电子邮件"
                    SendPassword_Descr_RequiredFields = "必填项"
                    SendPassword_Descr_BackToLogin = "返回到登录页面"
                    SendPassword_Descr_PasswordSentTo = "您的密码已经发送到{0}。"
                    SendPassword_Descr_FurtherCommentWithContactAddress = "您的安全区域密码将只发送到已经记录的您的电子信箱。<BR>如果您在二十四（24）小时内没有收到电子邮件，请和<a href=""mailto:{0}"">{1}</a>联系。"
                    META_CurrentContentLanguage = "ZH"
                    StatusLineUsername = "用户"
                    StatusLinePassword = "密码"
                    StatusLineSubmit = "Go!"
                    StatusLineEditorial = "编者的话"
                    StatusLineContactUs = "联系我们"
                    StatusLineDataprotection = "隐私声明"
                    StatusLineLoggedInAs = "已登录，作为"
                    StatusLineLegalNote = "隐私声明和法律说明"
                    StatusLineCopyright_AllRightsReserved = "版权所有。"
                    NavAreaNameYourProfile = "您的特征"
                    NavLinkNameUpdatePasswort = "更改密码"
                    NavLinkNameUpdateProfile = "更改特征"
                    NavLinkNameLogout = "注销"
                    NavLinkNameLogin = "登录"
                    NavPointUpdatedHint = "这里有新内容或内容有更新"
                    NavPointTemporaryHiddenHint = "本应用对其他用户暂时锁定。本应用可能正在建造。"
                    SystemButtonYes = "是"
                    SystemButtonNo = "否"
                    SystemButtonOkay = "确认"
                    SystemButtonCancel = "取消"
                    ErrorUserOrPasswordWrong = "用户名称或密码输入有误，或访问被拒绝！<p>请您确保<ul><li>正确地输入用户名称和密码（密码是分大小下的！）</li><li>使用了正确的用户名称和密码组合（或许您已经有了访问其他资源的密码，但这些密码在这里无效）</li></ul>"
                    ErrorServerConfigurationError = "本服务器还没有正确配置。请向您的系统管理员咨询。"
                    ErrorNoAuthorization = "您没有访问本区域的权限。"
                    ErrorAlreadyLoggedOn = "您已经登录！请先在其他工作站注销！<br><font color=""red"">如果您确实没有登录，请通过<a href=""mailto:[n:0]"">[n:1]</a>通知我们！</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "您的会话已经终止，因为您已经在其他工作站登录。<br>"
                    ErrorLogonFailedTooOften = "登录过程失败次数过多，帐号被停用。<br>请稍候再试！"
                    ErrorEmptyPassword = "请别忘了输入密码！<br>如果您忘记了密码，可以尝试通过电子邮件重新获得密码。详情请参阅本文档的末尾。"
                    ErrorRequiredField = "所需場"
                    ErrorUnknown = "意外错误！请和<a href=""mailto:support@camm.biz"">故障处理中心</a>联系！"
                    ErrorEmptyField = "请完整填写所有带有星号<em>(*)</em>的各项！"
                    ErrorWrongNetwork = "不允许通过您当前所在的网络进行连接。"
                    ErrorUserAlreadyExists = "此登录名称已经存在。请另外选择一个登录名称！"
                    ErrorLoginCreatedSuccessfully = "登录帐号已经成功创建！"
                    ErrorSendPWWrongLoginOrEmailAddress = "登录名称或电子信箱有误。<br>请输入正确的值，以便启动发送密码的过程。"
                    ErrorCookiesMustNotBeDisabled = "您的浏览器不支持cookies或者由于安全策略已经停用。"
                    ErrorTimoutOrLoginFromAnotherStation = "会话超时或从另一工作站登录。"
                    ErrorApplicationConfigurationIsEmpty = "本应用还未配置。请和本应用的制造商联系。"
                    InfoUserLoggedOutSuccessfully = "您已经成功注销。谢谢您的关注。"
                    UserManagementEMailColumnTitleLogin = "登录名称："
                    UserManagementEMailColumnTitleCompany = "公司："
                    UserManagementEMailColumnTitleName = "姓名："
                    UserManagementEMailColumnTitleEMailAddress = "电子信箱："
                    UserManagementEMailColumnTitleStreet = "街道："
                    UserManagementEMailColumnTitleZIPCode = "邮政编码："
                    UserManagementEMailColumnTitleLocation = "地市："
                    UserManagementEMailColumnTitleState = "州："
                    UserManagementEMailColumnTitleCountry = "国家："
                    UserManagementEMailColumnTitle1stLanguage = "第一首选语言："
                    UserManagementEMailColumnTitle2ndLanguage = "第二首选语言："
                    UserManagementEMailColumnTitle3rdLanguage = "第三首选语言："
                    UserManagementEMailColumnTitleComesFrom = "来自于："
                    UserManagementEMailColumnTitleMotivation = "动机："
                    UserManagementEMailColumnTitleCustomerNo = "客户号："
                    UserManagementEMailColumnTitleSupplierNo = "供应商号："
                    UserManagementEMailColumnTitleComment = "注释："
                    UserManagementAddressesMr = "先生"
                    UserManagementAddressesMs = "女士"
                    UserManagementEMailTextRegards = "顺致问候"
                    UserManagementEMailTextSubject = "您的登录名称"
                    UserManagementEMailTextSubject4AdminNewUser = "安全区域新用户"
                    UserManagementMasterServerAvailableInNearFuture = "注意：本服务器只能在稍后才可用。"
                    CreateAccount_MsgEMailWelcome_WithPassword = "欢迎来到安全区域！这是每天都应该来的地方！" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的登录名称是：[n:0]    " & ChrW(13) & ChrW(10) & _
                "您的密码是：[n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "为了防止他人得到并使用密码，请您不要忘记更改您的密码！" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "您在这里可获得到广泛的支持而获益！在我们的外联网中您可以找到大量的有用信息。您可以在此自由冲浪，检索信息。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的帐号容许您访问多个不同的安全项目。至此为止，您已经完成了登录安全区域的必要步骤。在此后的3至4天内，您将可以使用您的整个权限。届时请访问网址：" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMailWelcome = "欢迎来到安全区域！这是每天都应该来的地方！" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的登录名称是：[n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您在这里可获得到广泛的支持而获益！在我们的外联网中您可以找到大量的有用信息。您可以在此自由冲浪，检索信息。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的帐号容许您访问多个不同的安全项目。至此为止，您已经完成了登录安全区域的必要步骤。在此后的3至4天内，您将可以使用您的整个权限。届时请访问网址：" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMail4Admin = "以下新用户加入了安全区域。" & ChrW(13) & ChrW(10) & _
                "请分配相应的权限！" & ChrW(13) & ChrW(10) & _
                "设置权限请访问" & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & "！"
                    UserManagement_NewUser_MsgEMail4Admin = "您或您的同事将以下用户加入了安全区域。" & ChrW(13) & ChrW(10) & _
                "请分配相应的权限！" & ChrW(13) & ChrW(10) & _
                "设置权限请访问" & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & "！"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "欢迎来到安全区域！这是每天都应该来的地方！" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的用户""[n:0]""权限已经创建。欢迎访问：" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "我们乐意为您提供服务。"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "欢迎来到我们的安全区域！"
                    SendPassword_EMailMessage = "以下是您的新会员信息。却妥善保管这一确认邮件。邮件中含有您的用户名和密码，因此请相应地保密保存。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的安全区域密码是：[n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "为了防止他人得到并使用密码，请您不要忘记更改您的密码！" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "一个帐号容许您访问多个不同的安全项目。至此为止，您已经完成了登录安全区域的必要步骤。请访问网址：" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "管理员重置了您的帐号。以下是您的新会员信息。却妥善保管这一确认邮件。邮件中含有您的用户名和密码，因此请相应地保密保存。" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "您的安全区域密码是：[n:0]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
                "为了防止他人得到并使用密码，请您不要忘记更改您的密码！" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "一个帐号容许您访问多个不同的安全项目。至此为止，您已经完成了登录安全区域的必要步骤。请访问网址：" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "获得到广泛的支持而获益！"
                    HighlightTextTechnicalSupport = "我们刚刚起步，而事物每天都处在变化之中。"
                    HighlightTextExtro = "在我们的外联网中您可以找到大量的有用信息。您可以在此自由冲浪，检索信息！"
                    WelcomeTextWelcomeMessage = "欢迎来到我们的安全区域！"
                    WelcomeTextFeedbackToContact = "您是否还需要其他功能？如您对外联网有任何建议，欢迎您向<a href=""mailto:[n:0]"">[n:1]</a>提出！"
                    WelcomeTextIntro = "每天都应该来的地方！"
                    UserManagementEMailTextDearUndefinedGender = "Dear "
                    UserManagementSalutationUnformalUndefinedGender = "Hello "
                    NavAreaNameLogin = "登录"
                    NavLinkNamePasswordRecovery = "您忘记了密码？"
                    NavLinkNameNewUser = "创建新的用户帐号"
                    CreateAccount_Descr_MotivItemSupplier = "供应商"
                    UpdateProfile_Descr_MotivItemSupplier = "供应商"
                Case 4
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("es-ES")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("es-ES")
                    Logon_AskForForcingLogon = "Atención: Ya está registrado en otra sesión abierta. ¿Desea cancelar esa sesión e iniciar otra nueva?"
                    UserManagementSalutationUnformalMasculin = "Hola "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Señoras y señores, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Estimado Sr. "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "¡Hola! "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Hola "
                    UserManagementSalutationFormulaUnformalGroup = "¡Hola! "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Estimada Sra. "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagement_NewUser_TextWelcome = "Bienvenido al Área Segura. Un lugar donde ir todos los días." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Nuestra administración le ha admitido en el Área Segura de forma gratuita." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Su ID en línea es: [n:0]    " & ChrW(13) & ChrW(10) & _
"Su contraseña para el Área segura es: [n:1]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Por favor no olvide cambiar la contraseña por una propia lo antes posible, para asegurarse de que nadie más la tenga y pueda usarla." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Obtenga todas las ventajas de una amplia asistencia. Tiene una gran cantidad de información esperándole en nuestra red externa. Navegue por esta página cuanto quiera para explorarla de principio a fin." & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"Su cuenta le permite acceder a varios programas de seguridad. Llegado este punto debe haber completado el proceso de integración en el Área segura. La plena habilitación de las autorizaciones entrará en vigor en los próximos 3 - 4 días. Visite de nuevo la página:" & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
"[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Bienvenido al Área Segura. Un lugar donde ir todos los días.</p>" & _
                "<p>Nuestra administración le ha admitido en el Área Segura de forma gratuita.</p>" & _
                "<p><strong>Su ID en línea es: <font color=""red"">[n:0]</font><br>" & _
                "Su contraseña para el Área segura es: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Por favor no olvide cambiar la contraseña por una propia lo antes posible, para asegurarse de que nadie más la tenga y pueda usarla.</p>" & _
                "<p>Obtenga todas las ventajas de una extensa asistencia. Tiene una gran cantidad de información esperándole en nuestra red externa. Navegue por esta página cuanto quiera para explorarla de principio a fin.</p>" & _
                "<p>Su cuenta le permite acceder a varios programas de seguridad. Llegado este punto debe haber completado el proceso de integración en el Área segura. La plena habilitación de las autorizaciones entrará en vigor en los próximos 3 - 4 días. Visite de nuevo la página:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    UpdateProfile_Descr_Phone = "Teléfono"
                    UpdateProfile_Descr_Title = "Cambiar perfil"
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Mobile = "Tel. móvil"
                    UpdateProfile_Descr_PositionInCompany = "Posición"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Para continuar, rellene todos los campos obligatorios."
                    UpdateProfile_ErrMsg_MistypedPW = "Contraseña con erratas o escrita de modo erróneo. Tenga en cuenta la diferenciación entre mayúsculas y minúsculas."
                    UpdateProfile_ErrMsg_Undefined = "Valor inesperado de respuesta. Póngase en contacto con el administrador del sitio web."
                    UpdateProfile_ErrMsg_Success = "Su perfil ha sido actualizado."
                    UpdateProfile_ErrMsg_LogonTooOften = "El proceso de registro ha fallado demasiadas veces. La cuenta ha sido deshabilitada.<br>Por favor, inténtelo más tarde."
                    UpdateProfile_ErrMsg_NotAllowed = "No tiene autorización a acceder a este documento."
                    UpdateProfile_ErrMsg_PWRequired = "Para modificar su perfil necesita introducir su contraseña."
                    UpdateProfile_Descr_Address = "Dirección"
                    UpdateProfile_Descr_Company = "Compañía"
                    UpdateProfile_Descr_Addresses = "Saludo"
                    UpdateProfile_Descr_PleaseSelect = "(Seleccione por favor)"
                    UpdateProfile_Abbrev_Mister = "Sr."
                    UpdateProfile_Abbrev_Miss = "Sra."
                    UpdateProfile_Descr_AcademicTitle = "Título académico (p.ej. ""Dr."")"
                    UpdateProfile_Descr_FirstName = "Nombre"
                    UpdateProfile_Descr_LastName = "Apellidos"
                    UpdateProfile_Descr_NameAddition = "Complemento de nombre"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_Street = "Calle"
                    UpdateProfile_Descr_ZIPCode = "Código postal"
                    UpdateProfile_Descr_Location = "Lugar"
                    UpdateProfile_Descr_State = "Estado"
                    UpdateProfile_Descr_Country = "País"
                    UpdateProfile_Descr_UserDetails = "Detalles del usuario"
                    UpdateProfile_Descr_1stLanguage = "1er idioma preferido"
                    UpdateProfile_Descr_2ndLanguage = "2° idioma preferido"
                    UpdateProfile_Descr_3rdLanguage = "3er idioma preferido"
                    UpdateProfile_Descr_Authentification = "Confirme los cambios mediante su contraseña"
                    UpdateProfile_Descr_Password = "Contraseña"
                    UpdateProfile_Descr_Submit = "Actualizar perfil"
                    UpdateProfile_Descr_RequiredFields = "* Campos obligatorios"
                    UpdateProfile_Descr_CustomerSupplierData = "Datos de cliente/ proveedor"
                    UpdateProfile_Descr_CustomerNo = "N° de cliente"
                    UpdateProfile_Descr_SupplierNo = "N° de proveedor"
                    UserJustCreated_Descr_AccountCreated = "La cuenta de usuario ha sido creada."
                    UserJustCreated_Descr_LookAroundNow = "Ahora puede continuar y darse una vuelta para echar un vistazo."
                    UserJustCreated_Descr_PleaseNote = "Tenga en cuenta lo siguiente: En este momento <font color=""#336699"">Ud. es un miembro público</font>. Las autorizaciones y afiliaciones adicionales se le enviarán dentro de un plazo de 3 - 4 días."
                    UserJustCreated_Descr_Title = "¡Bienvenido al Área Segura!"
                    UpdatePW_Descr_Title = "Restablecer contraseña"
                    UpdatePW_ErrMsg_ConfirmationFailed = "¡La confirmación de la contraseña ha fallado! Verifique que la ha tecleado correctamente. Tenga en cuenta la diferenciación entre mayúsculas y minúsculas."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Introduzca por favor su contraseña actual y una nueva diferente. Tenga en cuenta la diferenciación entre mayúsculas y minúsculas."
                    UpdatePW_ErrMsg_Undefined = "Detectado error indefinido."
                    UpdatePW_ErrMsg_Success = "La contraseña ha sido modificada."
                    UpdatePW_ErrMsg_WrongOldPW = "La contraseña no ha podido modificarse Verifique por favor que ha introducido la contraseña actual correcta."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Para completar el cambio de contraseña, rellene todos los campos obligatorios."
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Por favor anote su contraseña actual y la nueva:"
                    UpdatePW_Descr_CurrentPW = "Contraseña actual"
                    UpdatePW_Descr_NewPW = "Contraseña nueva"
                    UpdatePW_Descr_NewPWConfirm = "Confirme la nueva contraseña"
                    UpdatePW_Descr_Submit = "Actualizar perfil"
                    UpdatePW_Descr_RequiredFields = "* Campos obligatorios"
                    UpdatePW_Error_PasswordComplexityPolicy = "La contraseña debe tener 3 caracteres como mínimo. No está permitido que use elementos de su nombre."
                    CreateAccount_Descr_CustomerSupplierData = "Datos del cliente/ proveedor"
                    CreateAccount_Descr_CustomerNo = "N° de cliente"
                    CreateAccount_Descr_SupplierNo = "N° de proveedor"
                    CreateAccount_Descr_FollowingError = "Ha ocurrido el siguiente error:"
                    CreateAccount_Descr_LoginDenied = "La apertura de sesión ha sido denegada."
                    CreateAccount_Descr_Submit = "Crear cuenta de usuario"
                    CreateAccount_Descr_RequiredFields = "Campos obligatorios"
                    CreateAccount_Descr_BackToLogin = "Volver a login"
                    CreateAccount_Descr_PageTitle = "Crear un nuevo usuario"
                    CreateAccount_Descr_UserLogin = "Login del usuario"
                    CreateAccount_Descr_NewLoginName = "Su nuevo nombre de registro"
                    CreateAccount_Descr_NewLoginPassword = "Su nueva contraseña"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Confirme la contraseña"
                    CreateAccount_Descr_Address = "Dirección"
                    CreateAccount_Descr_Company = "Compañía"
                    CreateAccount_Descr_Addresses = "Saludo"
                    CreateAccount_Descr_PleaseSelect = "(Seleccione por favor)"
                    CreateAccount_Descr_AcademicTitle = "Título académico (p.ej. ""Dr."")"
                    CreateAccount_Descr_FirstName = "Nombre"
                    CreateAccount_Descr_LastName = "Apellidos"
                    CreateAccount_Descr_NameAddition = "Complemento de nombre"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Calle"
                    CreateAccount_Descr_ZIPCode = "Código postal"
                    CreateAccount_Descr_Location = "Lugar"
                    CreateAccount_Descr_State = "Estado"
                    CreateAccount_Descr_Country = "País"
                    CreateAccount_Descr_Motivation = "¿Por qué motivo desea registrarse?"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Visitante de la página web"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Visitante de la página web"
                    CreateAccount_Descr_MotivItemDealer = "Distribuidor"
                    UpdateProfile_Descr_MotivItemDealer = "Distribuidor"
                    CreateAccount_Descr_MotivItemJournalist = "Periodista"
                    UpdateProfile_Descr_MotivItemJournalist = "Periodista"
                    CreateAccount_Descr_MotivItemOther = "Otros, que especifico a continuación"
                    UpdateProfile_Descr_MotivItemOther = "Otros, que especifico a continuación"
                    CreateAccount_Descr_WhereHeard = "¿Donde se ha enterado de nuestro Área Segura?"
                    CreateAccount_Descr_WhereItemFriend = "Amigo"
                    UpdateProfile_Descr_WhereItemFriend = "Amigo"
                    CreateAccount_Descr_WhereItemResellerDealer = "Mayorista / distribuidor"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Mayorista / distribuidor"
                    CreateAccount_Descr_WhereItemExhibition = "Feria"
                    UpdateProfile_Descr_WhereItemExhibition = "Feria"
                    CreateAccount_Descr_WhereItemMagazines = "Revistas"
                    UpdateProfile_Descr_WhereItemMagazines = "Revistas"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "De nosotros mismos"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "De nosotros mismos"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Motor buscador, que especifico a continuación"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Motor buscador, que especifico a continuación"
                    CreateAccount_Descr_WhereItemOther = "Otros, que especifico a continuación"
                    UpdateProfile_Descr_WhereItemOther = "Otros, que especifico a continuación"
                    CreateAccount_Descr_UserDetails = "Detalles del usuario"
                    CreateAccount_Descr_Comments = "Comentarios"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Solicitudes de autorizaciones adicionales"
                    CreateAccount_Descr_1stPreferredLanguage = "1er idioma preferido"
                    CreateAccount_Descr_2ndPreferredLanguage = "2° idioma preferido"
                    CreateAccount_Descr_3rdPreferredLanguage = "3er idioma preferido"
                    CreateAccount_ErrorJS_InputValue = "Introduzca un valor en el campo \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Introduzca un valor en el campo \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Introduzca un valor en el campo \""[n:1]\"" con por lo menos [n:0] caracteres."
                    UpdateProfile_ErrorJS_Length = "Introduzca un valor en el campo \""[n:1]\"" con por lo menos [n:0] caracteres."
                    Banner_Help = "Ayuda"
                    Banner_HeadTitle = "Inicio de sesión en el Área Segura"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Inicio de sesión"
                    Banner_Feedback = "Retroalimentación"
                    Logon_Connecting_RecommendationOnTimeout = "Si este problema se produce de nuevo, <a href=""mailto:[0]"">contáctenos</a>."
                    Logon_Connecting_InProgress = "Se está estableciendo la conexión al servidor…"
                    Logon_HeadTitle = "Logon en Área segura"
                    Logon_Connecting_LoginTimeout = "Exceso de tiempo de espera de login."
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Inicio de sesión"
                    Logon_SSO_ADS_RadioDoNothing = "Si la identificación no es correcta o si desea continuar sin registrarse, ahora (pregunte de nuevo más tarde)."
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Nombre de login:"
                    Logon_SSO_ADS_ButtonNext = "Continuar"
                    Logon_SSO_ADS_PageTitle = "Establecimiento de su registro automático"
                    Logon_SSO_ADS_IdentifiedUserName = "El sistema le tiene identificado como el usuario <strong>{0}</strong>."
                    Logon_SSO_ADS_LabelTakeAnAction = "¿Qué quiere hacer ahora?"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Contraseña:"
                    Logon_SSO_ADS_RadioRegisterNew = "Registrarse para una cuenta <strong>nueva</strong>"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "El sistema le tiene identificado como el usuario <strong>{0} ({1})</strong>."
                    Logon_SSO_ADS_RadioRegisterExisting = "Registrarse para una cuenta <strong>ya existente</strong>"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "Correo electrónico:"
                    Logon_SSO_ADS_ContactUs = "Si tiene alguna duda, <a href=""mailto:{0}"">consúltenos</a>."
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Vuelva a introducir la contraseña:"
                    Logon_BodyPrompt2User = "Introduzca su nombre de registro y su contraseña para acceder al " & OfficialServerGroup_Title & ".<br><em> Tenga en cuenta que el ID en línea y la contraseña van por separado y pueden ser diferentes de los que ya tenga para otras áreas nuestras.</em>"
                    Logon_BodyFormUserName = "ID en línea"
                    Logon_BodyFormUserPassword = "Contraseña"
                    Logon_BodyFormSubmit = "Login"
                    Logon_BodyFormCreateNewAccount = "Crear una nueva cuenta"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>¿No es miembro aún? Cree una cuenta nueva para acceder al " & OfficialServerGroup_Title & "</STRONG><BR>" & _
                                    "Si no tiene cuenta, puede </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>crear " & _
                                    "una ahora</FONT></A><FONT face=Arial size=2>. " & _
                                    "Por favor no cree otra&nbsp;" & _
                                    "cuenta si ha creado una antes. Si tiene " & _
                                    "dificultadas para registrarse, póngase en contacto con su <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>centro de servicio y asistencia</FONT></A>" & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>¿Ha olvidado su contraseña? Envie su contraseña a " & _
                                    "a su propio buzón por e-mail</B><BR>Puede que haya indicado una cuenta&nbsp;válida pero ha olvidado " & _
                                    "la contraseña.</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>Aquí puede hacerse enviar su contraseña por e-mail</FONT></A><FONT " & _
                                    "face=Arial size=2>. Tenga en cuenta que su " & _
                                    "contraseña se enviará por e-mail a la dirección que proporcionó en el momento de " & _
                                    "crear esta&nbsp; cuenta.<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>¿Tiene todavía algún problema?</strong><br>Si requiere ayuda adicional, consulte a </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>consúltenos</FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    AccessError_Descr_FollowingError = "Ha ocurrido el siguiente error:"
                    AccessError_Descr_LoginDenied = "La apertura de sesión ha sido denegada."
                    AccessError_Descr_BackToLogin = "Volver a login"
                    SendPassword_Descr_FollowingError = "Ha ocurrido el siguiente error:"
                    SendPassword_Descr_LoginDenied = "La apertura de sesión ha sido denegada."
                    SendPassword_Descr_Title = "Solicitar vía e-mail una contraseña para el Área Segura"
                    SendPassword_Descr_LoginName = "Nombre de login"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Enviar correo electrónico"
                    SendPassword_Descr_RequiredFields = "Campos obligatorios"
                    SendPassword_Descr_BackToLogin = "Volver a login"
                    SendPassword_Descr_PasswordSentTo = "Su contraseña le ha sido enviada {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "El sistema le enviará su contraseña de Área Segura a la dirección de e-mail que tenemos almacenada.<BR>Si no recibe el mensaje por correo electrónico en un plazo de veinticuatro (24) horas, póngase por favor en contacto con nosotros <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "ES"
                    StatusLineUsername = "Usuario"
                    StatusLinePassword = "Contraseña"
                    StatusLineSubmit = "Ejecutar"
                    StatusLineEditorial = "Editorial"
                    StatusLineContactUs = "Contáctenos"
                    StatusLineDataprotection = "Cláusula de privacidad"
                    StatusLineLoggedInAs = "Abierta sesión como"
                    StatusLineLegalNote = "Cláusula de privacidad y aviso legal"
                    StatusLineCopyright_AllRightsReserved = "Quedan reservados todos los derechos."
                    NavAreaNameYourProfile = "Su perfil"
                    NavLinkNameUpdatePasswort = "Modificar la contraseña"
                    NavLinkNameUpdateProfile = "Cambiar perfil"
                    NavLinkNameLogout = "Cerrar sesión"
                    NavLinkNameLogin = "Iniciar sesión"
                    NavPointUpdatedHint = "Aquí encontrará objetos nuevos o actualizados"
                    NavPointTemporaryHiddenHint = "Esta aplicación ha sido bloqueada temporalmente por otros usuarios. Esta aplicación puede que esté en construcción."
                    SystemButtonYes = "Sí"
                    SystemButtonNo = "No"
                    SystemButtonOkay = "Aceptar"
                    SystemButtonCancel = "Cancelar"
                    ErrorUserOrPasswordWrong = "Se han cometido errores al introducir el nombre de usuario o contraseña o se ha denegado el acceso.<p>Verifique por favor<ul><li>la ortografía del nombre de usuario y la contraseña (la contraseña distingue entre las letras mayúsculas y minúsculas)</li><li>y asegúrese de que está usando el nombre de usuario/ contraseña correctos (puede que tenga contraseñas para otros recursos nuestros, pero aquí no le van a funcionar)</li></ul>"
                    ErrorServerConfigurationError = "Este servidor no ha sido aún configurado correctamente. Consulte a su administrador."
                    ErrorNoAuthorization = "No tiene autorización para acceder a este área."
                    ErrorAlreadyLoggedOn = "Ya tiene una sesión abierta. Cierre por favor antes la sesión en la otra estación!<br><font color=""red"">Si está seguro de que no tiene ninguna otra sesión abierta, avísenos a través de <a href=""mailto:[n:0]"">[n:1]</a>!</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Su sesión ha terminado porque tenía abierta sesión en otra estación.<br>"
                    ErrorLogonFailedTooOften = "El proceso de registro ha fallado demasiadas veces. La cuenta ha sido deshabilitada.<br>Por favor, inténtelo más tarde."
                    ErrorEmptyPassword = "No se olvide de introducir una contraseña. Si no recuerda su contraseña, intente solicitarla por e-mail. Si desea más detalles, consulte el final de este documento."
                    ErrorRequiredField = "Campo obligatorio"
                    ErrorUnknown = "¡Error inesperado! - Establezca por favor contacto con el <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Rellene todos los campos marcados con un asterisco <em>(*)</em>."
                    ErrorWrongNetwork = "No tiene autorización a conectarse a través de su conexión de red actual."
                    ErrorUserAlreadyExists = "Ya hay un registro de usuario con ese nombre. Elija por favor otro nombre de login."
                    ErrorLoginCreatedSuccessfully = "La cuenta de registro ha sido creada."
                    ErrorSendPWWrongLoginOrEmailAddress = "Login o dirección de e-mail incorrectos.<br>Introduzca datos correctos para que pueda iniciarse el proceso de envío de la contraseña."
                    ErrorCookiesMustNotBeDisabled = "Su navegador no admite cookies o las cookies están deshabilitadas por la estrategia de seguridad del navegador."
                    ErrorTimoutOrLoginFromAnotherStation = "Excedido tiempo de espera de esta sesión o registro en otra estación."
                    ErrorApplicationConfigurationIsEmpty = "Esta aplicación aún no ha sido configurada. Contacte con el autor de esta aplicación."
                    InfoUserLoggedOutSuccessfully = "Ha cerrado su sesión. Le agradecemos su asistencia."
                    UserManagementEMailColumnTitleLogin = "ID en línea: "
                    UserManagementEMailColumnTitleCompany = "Compañía: "
                    UserManagementEMailColumnTitleName = "Nombre: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Calle: "
                    UserManagementEMailColumnTitleZIPCode = "Código postal: "
                    UserManagementEMailColumnTitleLocation = "Lugar: "
                    UserManagementEMailColumnTitleState = "Estado: "
                    UserManagementEMailColumnTitleCountry = "País: "
                    UserManagementEMailColumnTitle1stLanguage = "1er idioma preferido: "
                    UserManagementEMailColumnTitle2ndLanguage = "2° idioma preferido: "
                    UserManagementEMailColumnTitle3rdLanguage = "3er idioma preferido: "
                    UserManagementEMailColumnTitleComesFrom = "Procedencia: "
                    UserManagementEMailColumnTitleMotivation = "Motivación: "
                    UserManagementEMailColumnTitleCustomerNo = "N° de cliente: "
                    UserManagementEMailColumnTitleSupplierNo = "N° de proveedor: "
                    UserManagementEMailColumnTitleComment = "Comentario: "
                    UserManagementAddressesMr = "Sr. "
                    UserManagementAddressesMs = "Sra. "
                    UserManagementEMailTextRegards = "Atentamente"
                    UserManagementEMailTextSubject = "Su ID en línea para el Área Segura"
                    UserManagementEMailTextSubject4AdminNewUser = "Área Segura - Nuevo usuario"
                    UserManagementMasterServerAvailableInNearFuture = "Atención: Este servidor estará disponible en breve."
                    CreateAccount_MsgEMailWelcome = "Bienvenido al Área Segura. Un lugar donde ir todos los días." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Su ID en línea es: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Obtenga todas las ventajas de una extensa asistencia. Tiene una gran cantidad de información esperándole en nuestra red externa. Navegue por esta página cuanto quiera para explorarla de principio a fin." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Su cuenta le permite acceder a varios programas de seguridad. Llegado este punto debe haber completado el proceso de integración en el Área segura. La plena habilitación de las autorizaciones entrará en vigor en los próximos 3 - 4 días. Visite de nuevo la página:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Bienvenido al Área Segura. Un lugar donde ir todos los días." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Su ID en línea es: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Su contraseña es: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Por favor no olvide cambiar la contraseña por una propia lo antes posible, para asegurarse de que nadie más la tenga y pueda usarla." & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Obtenga todas las ventajas de una extensa asistencia. Tiene una gran cantidad de información esperándole en nuestra red externa. Navegue por esta página cuanto quiera para explorarla de principio a fin." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Su cuenta le permite acceder a varios programas de seguridad. Llegado este punto debe haber completado el proceso de integración en el Área segura. La plena habilitación de las autorizaciones entrará en vigor en los próximos 3 - 4 días. Visite de nuevo la página:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMail4Admin = "El nuevo usuario siguiente se ha incorporado al Área Segura." & ChrW(13) & ChrW(10) & _
                "Asigne por favor las autorizaciones correspondientes." & ChrW(13) & ChrW(10) & _
                "Para configurar las autorizaciones, vaya a " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " ."
                    UserManagement_NewUser_MsgEMail4Admin = "El siguiente usuario nuevo ha sido incorporado al Área Segura por Ud. o por uno de sus compañeros." & ChrW(13) & ChrW(10) & _
                "Asigne por favor las autorizaciones correspondientes." & ChrW(13) & ChrW(10) & _
                "Para configurar las autorizaciones, vaya a " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " ."
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Bienvenido a nuestro Área Segura. Un lugar donde ir todos los días." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Sus autorizaciones para el usuario ""[n:0]"" han sido creadas. Con mucho gusto puede visitarnos en:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Estamos a su servicio con mucho gusto en el ciberespacio."
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "¡Bienvenido a nuestro Área Segura!"
                    SendPassword_EMailMessage = "Debajo consta información para Ud. en calidad de nuevo miembro. Guarde este correo de confirmación como documentación de su nombre de usuario y contraseña. Considere que esta información tiene carácter confidencial y trátela como tal." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Su contraseña para el área segura es: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Por favor no olvide cambiar la contraseña por una propia lo antes posible, para asegurarse de que nadie más la tenga y pueda usarla." & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Una cuenta le permite acceder a varios programas de seguridad. Llegado este punto debe haber completado el proceso de integración en nuestro Área segura. Visite de nuevo la página:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "El administrador ha restablecido su cuenta. Debajo consta información para Ud. en calidad de nuevo miembro. Guarde este correo de confirmación como documentación de su nombre de usuario y contraseña. Considere que esta información tiene carácter confidencial y trátela como tal." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Su contraseña para el área segura es: [n:0]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Por favor no olvide cambiar la contraseña por una propia lo antes posible, para asegurarse de que nadie más la tenga y pueda usarla." & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Una cuenta le permite acceder a varios programas de seguridad. Llegado este punto debe haber completado el proceso de integración en nuestro Área segura. Visite de nuevo la página:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Obtenga todas las ventajas de una amplia asistencia."
                    HighlightTextTechnicalSupport = "Acabamos de empezar y las cosas cambian todos los días."
                    HighlightTextExtro = "Tiene una gran cantidad de información esperándole en nuestra red externa. Navegue por esta página cuanto quiera para explorarla de principio a fin."
                    WelcomeTextWelcomeMessage = "¡Bienvenido a nuestro Área Segura!"
                    WelcomeTextFeedbackToContact = "¿Necesita funciones adicionales? No dude en hacernos llegar sus comentarios en la red externa a <a href=""mailto:[n:0]"">[n:1]</a>."
                    WelcomeTextIntro = "Un lugar donde ir todos los días."
                    UserManagementEMailTextDearUndefinedGender = "Estimado/Querida "
                    UserManagementSalutationUnformalUndefinedGender = "Hola "
                    NavAreaNameLogin = "Login"
                    NavLinkNamePasswordRecovery = "¿Ha olvidado la contraseña?"
                    NavLinkNameNewUser = "Crear una nueva cuenta de usuario"
                    CreateAccount_Descr_MotivItemSupplier = "Proveedor "
                    UpdateProfile_Descr_MotivItemSupplier = "Proveedor "
                Case 3
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("fr-FR")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("fr-FR")
                    Logon_AskForForcingLogon = "Attention: Vous êtes déjà connecté dans une autre session. Désirez-vous terminer cette session pour vous reconnecter dans une autre?"
                    UserManagementSalutationUnformalMasculin = "Allo "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Mesdames et Messieurs, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Cher M "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Salut, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Allo "
                    UserManagementSalutationFormulaUnformalGroup = "Salut, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Chère Mme "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagement_NewUser_TextWelcome = "Bienvenue dans le domaine sécurisé! Vous trouverez ci-joint des informations journalières et libres." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Nous vous avons attribué un compte utilisateur pour le domaine sécurisé, et ce service est gratuit pour vous." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre nom de connexion au domaine sécurisé: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Votre mot de passe pour le domaine sécurisé est: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Veuillez ne pas oublier de changer votre mot de passe le plus rapidement possible. Ceci est impératif pour s'assurer qu'aucune autre personne (par ex un hacker) ne possède et n'utilise votre mot de passe!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Bénéficiez des avantages d'un soutien solide! Aujourd'hui, tout dépend de vous!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre compte utilisateur vous offre l'accès aux diverses applications protégées. La procédure d'annonce pouyr le domaine sécurisé est déjà terminée. Vos droits complets ne seront cependant validés que dans 3 - 4 jours. Vous pouvez entrer dans le domaine sécurisé avec l'URL suivant:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Bienvenue dans le domaine sécurisé! Vous trouverez ci-joint des informations journalières et libres.</p>" & _
                "<p>Nous vous avons attribué un compte utilisateur pour le domaine sécurisé, et ce service est gratuit pour vous.</p>" & _
                "<p><strong>Votre nom de connexion au domaine sécurisé: <font color=""red"">[n:0]</font><br>" & _
                "Votre mot de passe pour le domaine sécurisé est: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Veuillez ne pas oublier de changer votre mot de passe le plus rapidement possible. Ceci est impératif pour s'assurer qu'aucune autre personne (par ex un hacker) ne possède et n'utilise votre mot de passe!</p>" & _
                "<p>Bénéficiez des avantages d'un soutien solide! Aujourd'hui, tout dépend de vous!" & _
                "<p>Votre compte utilisateur vous offre l'accès aux diverses applications protégées. La procédure d'annonce pouyr le domaine sécurisé est déjà terminée. Vos droits complets ne seront cependant validés que dans 3 - 4 jours. Vous pouvez entrer dans le domaine sécurisé avec l'URL suivant:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    UpdateProfile_Descr_Title = "Modifier le profil utilisateur"
                    UpdateProfile_Descr_Mobile = "Téléphone portable"
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Phone = "Téléphone"
                    UpdateProfile_Descr_PositionInCompany = "Position"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Pour continuer, veuillez renseigner les champs requis!"
                    UpdateProfile_ErrMsg_MistypedPW = "Mot de passe incorrect. Veuillez respecter l'écriture majuscule et minuscule!"
                    UpdateProfile_ErrMsg_Undefined = "Valeur de retour inattendue! – Veuillez contacter l'administrateur!"
                    UpdateProfile_ErrMsg_Success = "Votre profil utilisateur a été modifié avec succès!"
                    UpdateProfile_ErrMsg_LogonTooOften = "Le processus de connexion a trop souvent échoué ; le compte a été désactivé.<br>Veuillez réessayer plus tard!"
                    UpdateProfile_ErrMsg_NotAllowed = "Nous n'avez pas de permission pour accéder à ce document!"
                    UpdateProfile_ErrMsg_PWRequired = "Veuillez également envoyer votre mot de passe pou modifier votre profil utilisateur!"
                    UpdateProfile_Descr_Address = "Adresse"
                    UpdateProfile_Descr_Company = "Société"
                    UpdateProfile_Descr_Addresses = "Titre"
                    UpdateProfile_Descr_PleaseSelect = "(Veuillez choisir!)"
                    UpdateProfile_Abbrev_Mister = "M"
                    UpdateProfile_Abbrev_Miss = "Mme"
                    UpdateProfile_Descr_AcademicTitle = "Titre universitaire (par ex. ""Dr."")"
                    UpdateProfile_Descr_FirstName = "Prénom"
                    UpdateProfile_Descr_LastName = "Nom"
                    UpdateProfile_Descr_NameAddition = "Nom complémentaire"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_Street = "Rue"
                    UpdateProfile_Descr_ZIPCode = "Code postal"
                    UpdateProfile_Descr_Location = "Lieu"
                    UpdateProfile_Descr_State = "Etat"
                    UpdateProfile_Descr_Country = "Pays"
                    UpdateProfile_Descr_UserDetails = "Détails utilisateur"
                    UpdateProfile_Descr_1stLanguage = "1ère langue de préférence"
                    UpdateProfile_Descr_2ndLanguage = "2ème langue de préférence"
                    UpdateProfile_Descr_3rdLanguage = "3ème langue de préférence"
                    UpdateProfile_Descr_Authentification = "Veuillez confirmer les modifications avec votre mot de passe"
                    UpdateProfile_Descr_Password = "Mot de passe"
                    UpdateProfile_Descr_Submit = "Actualisation du profil"
                    UpdateProfile_Descr_RequiredFields = "* champs requis"
                    UpdateProfile_Descr_CustomerSupplierData = "Données clients/fournisseur"
                    UpdateProfile_Descr_CustomerNo = "Client no."
                    UpdateProfile_Descr_SupplierNo = "Fournisseur no."
                    UserJustCreated_Descr_AccountCreated = "Votre compte utilisateur a été créé avec succès!"
                    UserJustCreated_Descr_LookAroundNow = "Vous pouvez continuer et naviguer."
                    UserJustCreated_Descr_PleaseNote = "Remarque: actuellement vous êtes <font color=""#336699"">un membre public</font>. Les abonnements et les droits d'accès additionnels vous seront accordés dans prochains 3 à 4 jours."
                    UserJustCreated_Descr_Title = "Bienvenue dans l'espace sécurité!"
                    UpdatePW_Descr_Title = "Réinitialiser le mot de passe"
                    UpdatePW_ErrMsg_ConfirmationFailed = "La confirmation du mot de passe ne correspond pas au nouveau mot de passe! Veillez entrer les mêmes mots de passe en respectant l'écriture majuscule et minuscule."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Veuillez entrer votre mot de passe actuel et votre nouveau mot de passe. Respecter l'écriture majuscule et minuscule."
                    UpdatePW_ErrMsg_Undefined = "Erreur non définie détectée!"
                    UpdatePW_ErrMsg_Success = "Le mot de passe a été modifié avec succès!"
                    UpdatePW_ErrMsg_WrongOldPW = "Impossible de modifier le mot de passe! Veuillez vérifier si le mot de passe actuel a été correctement entré."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Veuillez renseigner les champs requis pour compléter la modification du mot de passe!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Veuillez entrer votre mot de passe actuel et le nouveau mot de passe:"
                    UpdatePW_Descr_CurrentPW = "Mot de passe actuel"
                    UpdatePW_Descr_NewPW = "Nouveau mot de passe"
                    UpdatePW_Descr_NewPWConfirm = "Confirmer le nouveau mot de passe"
                    UpdatePW_Descr_Submit = "Sauvegarder les modifications"
                    UpdatePW_Descr_RequiredFields = "* champs requis"
                    UpdatePW_Error_PasswordComplexityPolicy = "Le mot de passe doit se composer d'au moins 3 caractères. Aucun élément de votre nom ne peut être utilisé."
                    CreateAccount_Descr_CustomerSupplierData = "Données clients/fournisseur"
                    CreateAccount_Descr_CustomerNo = "Client no."
                    CreateAccount_Descr_SupplierNo = "Fournisseur no."
                    CreateAccount_Descr_FollowingError = "L'erreur suivante s'est produite:"
                    CreateAccount_Descr_LoginDenied = "La connexion a été refusée!"
                    CreateAccount_Descr_Submit = "Créer un compte utilisateur"
                    CreateAccount_Descr_RequiredFields = "champs requis"
                    CreateAccount_Descr_BackToLogin = "Retour à la connexion"
                    CreateAccount_Descr_PageTitle = "Créer un nouveau utilisateur"
                    CreateAccount_Descr_UserLogin = "Connexion"
                    CreateAccount_Descr_NewLoginName = "Votre nouveau nom d'utilisateur"
                    CreateAccount_Descr_NewLoginPassword = "Votre nouveau mot de passe"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Confirmer votre mot de passe"
                    CreateAccount_Descr_Address = "Adresse"
                    CreateAccount_Descr_Company = "Société"
                    CreateAccount_Descr_Addresses = "Titre"
                    CreateAccount_Descr_PleaseSelect = "(Veuillez choisir!)"
                    CreateAccount_Descr_AcademicTitle = "Titre universitaire (par ex. ""Dr."")"
                    CreateAccount_Descr_FirstName = "Prénom"
                    CreateAccount_Descr_LastName = "Nom"
                    CreateAccount_Descr_NameAddition = "Nom complémentaire"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Rue"
                    CreateAccount_Descr_ZIPCode = "Code postal"
                    CreateAccount_Descr_Location = "Lieu"
                    CreateAccount_Descr_State = "État/Canton"
                    CreateAccount_Descr_Country = "Pays"
                    CreateAccount_Descr_Motivation = "Quelle est la raison pour votre inscription"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Visiteur du site web"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Visiteur du site web"
                    CreateAccount_Descr_MotivItemDealer = "Revendeur"
                    UpdateProfile_Descr_MotivItemDealer = "Revendeur"
                    CreateAccount_Descr_MotivItemJournalist = "Journaliste"
                    UpdateProfile_Descr_MotivItemJournalist = "Journaliste"
                    CreateAccount_Descr_MotivItemOther = "Autres, veuillez spécifier"
                    UpdateProfile_Descr_MotivItemOther = "Autres, veuillez spécifier"
                    CreateAccount_Descr_WhereHeard = "Je me suis intéressé à cette zone par"
                    CreateAccount_Descr_WhereItemFriend = "un ami"
                    UpdateProfile_Descr_WhereItemFriend = "un ami"
                    CreateAccount_Descr_WhereItemResellerDealer = "un revendeur/commercial"
                    UpdateProfile_Descr_WhereItemResellerDealer = "un revendeur/commercial"
                    CreateAccount_Descr_WhereItemExhibition = "un salon"
                    UpdateProfile_Descr_WhereItemExhibition = "un salon"
                    CreateAccount_Descr_WhereItemMagazines = "un magazine"
                    UpdateProfile_Descr_WhereItemMagazines = "un magazine"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "de nous-mêmes"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "de nous-mêmes"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Moteur de recherche, veuillez spécifier"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Moteur de recherche, veuillez spécifier"
                    CreateAccount_Descr_WhereItemOther = "Autres, veuillez spécifier"
                    UpdateProfile_Descr_WhereItemOther = "Autres, veuillez spécifier"
                    CreateAccount_Descr_UserDetails = "Données utilisateur"
                    CreateAccount_Descr_Comments = "Commentaires"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Demandes d'autorisations supplémentaires"
                    CreateAccount_Descr_1stPreferredLanguage = "1ère langue de préférence"
                    CreateAccount_Descr_2ndPreferredLanguage = "2ème langue de préférence"
                    CreateAccount_Descr_3rdPreferredLanguage = "3ème langue de préférence"
                    CreateAccount_ErrorJS_InputValue = "Veuillez renseigner le champ \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Veuillez renseigner le champ \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Veuillez entrer une valeur avec au moins [n:0] caractères dans le champ \""[n:1]\""."
                    UpdateProfile_ErrorJS_Length = "Veuillez entrer une valeur avec au moins [n:0] caractères dans le champ \""[n:1]\""."
                    Banner_Help = "Aide"
                    Banner_HeadTitle = "Espace sécurité - Connexion"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Connexion"
                    Banner_Feedback = "Feedback"
                    Logon_Connecting_InProgress = "Vous allez être connecté au serveur…"
                    Logon_HeadTitle = "Secured Area - Connexion"
                    Logon_Connecting_LoginTimeout = "Login temps écoulé."
                    Logon_Connecting_RecommendationOnTimeout = "Si ce problème apparaît de nouveau, veuillez nous <a href=""mailto:[0]"">contacter</a>."
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Connexion"
                    Logon_SSO_ADS_PageTitle = "Setup de votre logon automatique"
                    Logon_SSO_ADS_IdentifiedUserName = "Vous avez été identifié comme utilisateur <strong>{0}</strong>."
                    Logon_SSO_ADS_LabelTakeAnAction = "Que souhaitez-vous faire maintenant?"
                    Logon_SSO_ADS_RadioRegisterExisting = "Vous enregistrer pour un compte <strong>existant déjà</strong>"
                    Logon_SSO_ADS_RadioRegisterNew = "Vous enregistrer pour un <strong>nouveau</strong> compte"
                    Logon_SSO_ADS_RadioDoNothing = "Si l’identification est fausse ou si vous souhaitez opérer sans login, maintenant (reposer la question ultérieurement)."
                    Logon_SSO_ADS_ContactUs = "Pour toute question, veuillez <a href=""mailto:{0}"">nous contacter</a>."
                    Logon_SSO_ADS_ButtonNext = "Continuer"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Nom de login:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Mot de passe:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Retaper le mot de passe:"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "Adresse email:"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Vous avez été identifié comme utilisateur <strong>{0} ({1})</strong>."
                    Logon_BodyPrompt2User = "Entrez votre nom d'utilisateur et votre mot de passe pour accéder au " & OfficialServerGroup_Title & ".<br><em>Remarque: votre identificateur en ligne et votre mot de passe peuvent différer d'autres données d'accès que vous avez obtenus pour d'autres zones.</em>"
                    Logon_BodyFormUserName = "Identificateur en ligne"
                    Logon_BodyFormUserPassword = "Mot de passe"
                    Logon_BodyFormSubmit = "Connexion"
                    Logon_BodyFormCreateNewAccount = "Créer un nouveau compte utilisateur"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Pas encore inscrit? Créez votre propre compte pour accéder à " & OfficialServerGroup_Title & "</STRONG><BR>" & _
                                    "Si vous n'avez pas encore de compte, vous pouvez </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>maintenant le créer " & _
                                    "</FONT></A><FONT face=Arial size=2>. " & _
                                    "Veuillez ne pas créer d'autres&nbsp;" & _
                                    "comptes, si vous en avez déjà créer dans le passé. En cas de " & _
                                    "problèmes de connexion, veuillez contacter votre <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>support service center</FONT></A>" & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Mot de passe oublié? Nous vous enverrons votre mot de passe " & _
                                    "par e-mail</B><BR>Vous avez déjà un compte valide&nbsp;et oublié " & _
                                    "votre mot de passe.</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>Ici, vous pouvez obtenir votre mot de passe par e-mail</FONT></A><FONT " & _
                                    "face=Arial size=2>. Remarque: " & _
                                    "votre mot de passe vous sera envoyée à l'adresse e-mail d'origine " & _
                                    "Créer ce&nbsp; compte.<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Problèmes pas encore résolus?</strong><br>Si vous nécessitez une assistance supplémentaire, n'hésitez pas à </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>nous contacter</FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    AccessError_Descr_FollowingError = "L'erreur suivante s'est produite:"
                    AccessError_Descr_LoginDenied = "La connexion a été refusée!"
                    AccessError_Descr_BackToLogin = "Retour à la connexion"
                    SendPassword_Descr_FollowingError = "L'erreur suivante s'est produite:"
                    SendPassword_Descr_LoginDenied = "La connexion a été refusée!"
                    SendPassword_Descr_Title = "Demande du mot de passe pour l'espace sécurité via e-mail"
                    SendPassword_Descr_LoginName = "Nom d'utilisateur"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Envoyer e-mail"
                    SendPassword_Descr_RequiredFields = "Champs requis"
                    SendPassword_Descr_BackToLogin = "Retour à la connexion"
                    SendPassword_Descr_PasswordSentTo = "Votre mot de passe a été envoyé à {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "Votre mot de passe pour l'espace sécurité ne vous sera envoyée qu'à l'adresse e-mail enregistrée.<BR>Veuillez nous contacter, si vous n'avez pas reçu cet e-mail dans les prochaines 24 heures <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "FR"
                    StatusLineUsername = "Utilisateur"
                    StatusLinePassword = "Mot de passe"
                    StatusLineSubmit = "OK"
                    StatusLineEditorial = "Editorial"
                    StatusLineContactUs = "Nous contacter"
                    StatusLineDataprotection = "Protection des données"
                    StatusLineLoggedInAs = "Connecté sous"
                    StatusLineLegalNote = "Protection des données et informations légales"
                    StatusLineCopyright_AllRightsReserved = "Tous droits réservés."
                    NavAreaNameYourProfile = "Ouverture de session"
                    NavLinkNameUpdatePasswort = "Modifier le mot de passe"
                    NavLinkNameUpdateProfile = "Modifier le profil"
                    NavLinkNameLogout = "Déconnecter"
                    NavLinkNameLogin = "Connecter"
                    NavPointUpdatedHint = "Ici, vous trouverez de nouvelles créations ou des actualisations"
                    NavPointTemporaryHiddenHint = "Cette application est momentanément désactivées pour d'autres utilisateurs. Il est également possible que l'application soit en construction."
                    SystemButtonYes = "Oui"
                    SystemButtonNo = "Non"
                    SystemButtonOkay = "OK"
                    SystemButtonCancel = "Annuler"
                    ErrorUserOrPasswordWrong = "Nom d'utilisateur ou mot de passe incorrect ou accès refusé!<p>Veuillez vérifier <ul><li>l'écriture du nom et du mot de passe (veillez aux lettres majuscules et minuscules du mot de passe!)</li><li>que vous avez utilisé la constellation correcte du nom et du mot de passe (vous avez éventuellement déjà reçu d'autres noms/mots de passe qui ne sont pas valables pour cette zone)</li></ul>"
                    ErrorServerConfigurationError = "Ce serveur n'est pas encore correctement configuré. Veuillez contacter votre administrateur."
                    ErrorNoAuthorization = "Vous n'avez pas de droits d'accès à cette zone."
                    ErrorAlreadyLoggedOn = "Vous être déjà connecté! Veuillez vous déconnecter auparavant de votre autre poste de travail!<br><font color=""red"">Si vous n'êtes pas sûr de vous avoir déconnecté de tous vos postes, envoyez-nous une court e-mail à <a href=""mailto:[n:0]"">[n:1]</a>!</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Votre session de travail a été terminée parce que vous vous avez connecté à une autre station.<br>"
                    ErrorLogonFailedTooOften = "Le processus de connexion a trop souvent échoué ; le compte a été désactivé.<br>Veuillez réessayer plus tard!"
                    ErrorEmptyPassword = "N'oubliez pas d'entrer un mot de passe!<br>Si vous avez oublié votre mot de passe, nous vous pouvons le demander par e-mail. Consultez le texte en bas de page pour plus de détails."
                    ErrorRequiredField = "Champ obligatoire"
                    ErrorUnknown = "Erreur inattendue! - Veuillez contacter le <a href=""mailto:support@camm.biz"">Service en ligne</a>!"
                    ErrorEmptyField = "Veuillez renseigner tous les champs avec un astérisque <em>(*)</em>!"
                    ErrorWrongNetwork = "Vous n'avez pas de droit pour vous connecter sur l'actuel réseau de connexion."
                    ErrorUserAlreadyExists = "Ce compte utilisateur existe déjà. Veuillez choisir un autre nom d'utilisateur!"
                    ErrorLoginCreatedSuccessfully = "Le compte de connexion a été créé avec succès!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Adresse e-mail incorrecte.<br>Veuillez entrer les données correctement telles qu'elles ont été enregistrées dans votre profil utilisateur."
                    ErrorCookiesMustNotBeDisabled = "Votre moteur de recherche ne prend pas les cookies en charge ou elles ont été désactivées en raison du niveau de sécurité."
                    ErrorTimoutOrLoginFromAnotherStation = "Session expirée ou connexion d'une autre station."
                    ErrorApplicationConfigurationIsEmpty = "Cette application n'est pas encore configurée. Veuillez contacter le fabricant de l'application."
                    InfoUserLoggedOutSuccessfully = "Vous avez été déconnecté avec succès. Merci de votre visite."
                    UserManagementEMailColumnTitleLogin = "Identificateur en ligne: "
                    UserManagementEMailColumnTitleCompany = "Société: "
                    UserManagementEMailColumnTitleName = "Nom: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Rue: "
                    UserManagementEMailColumnTitleZIPCode = "Code postal: "
                    UserManagementEMailColumnTitleLocation = "Lieu: "
                    UserManagementEMailColumnTitleState = "Etat: "
                    UserManagementEMailColumnTitleCountry = "Pays: "
                    UserManagementEMailColumnTitle1stLanguage = "1ère langue de préférence: "
                    UserManagementEMailColumnTitle2ndLanguage = "2ème langue de préférence: "
                    UserManagementEMailColumnTitle3rdLanguage = "3ème langue de préférence: "
                    UserManagementEMailColumnTitleComesFrom = "En provenance de: "
                    UserManagementEMailColumnTitleMotivation = "Raison: "
                    UserManagementEMailColumnTitleCustomerNo = "Client no.: "
                    UserManagementEMailColumnTitleSupplierNo = "Fournisseur no.: "
                    UserManagementEMailColumnTitleComment = "Commentaire: "
                    UserManagementAddressesMr = "M "
                    UserManagementAddressesMs = "Mme "
                    UserManagementEMailTextRegards = "Cordialement"
                    UserManagementEMailTextSubject = "Votre identificateur en ligne"
                    UserManagementEMailTextSubject4AdminNewUser = "Espace sécurité - Nouvel utilisateur"
                    UserManagementMasterServerAvailableInNearFuture = "Attention: ce serveur sera prochainement disponible."
                    CreateAccount_MsgEMailWelcome = "Bienvenue dans l'espace sécurité! L'endroit à visiter quotidiennement!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre identificateur en ligne pour l'espace sécurité: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Profitez des avantages d'une assistance étendue! Des informations en grande quantité attendent d'être consultées. N'hésitez pas à les explorer." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre compte utilisateur vous offre la possibilité d'accéder à des applications sécurisées variées. Le processus d'inscription est déjà terminé. Vos droits d'accès vous serons accordés dans les 3 - 4 prochains jours. Vous pourrez alors visiter l'espace sécurité par l'URL suivant:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Bienvenue dans l'espace sécurité! L'endroit à visiter quotidiennement!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre identificateur en ligne pour l'espace sécurité: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Votre mot de passe: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "N'oubliez pas de changer votre mot de passe le plus vite possible pour être certain que vous êtes le seul à l'utiliser!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Profitez des avantages d'une assistance étendue! Des informations en grande quantité attendent d'être consultées. N'hésitez pas à les explorer." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre compte utilisateur vous offre la possibilité d'accéder à des applications sécurisées variées. Le processus d'inscription est déjà terminé. Vos droits d'accès vous serons accordés dans les 3 - 4 prochains jours. Vous pourrez alors visiter l'espace sécurité par l'URL suivant:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMail4Admin = "Le nouvel utilisateur suivant s'est inscrit à Secured Area." & ChrW(13) & ChrW(10) & _
                "Veuillez attribuer les droits d'accès requis!" & ChrW(13) & ChrW(10) & _
                "Pour régler les droits d'accès, veuillez visiter " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_MsgEMail4Admin = "Le nouvel utilisateur suivant a été inscrit à l'espace sécurité par vous ou par un de vos collègues." & ChrW(13) & ChrW(10) & _
                "Veuillez attribuer les droits d'accès requis!" & ChrW(13) & ChrW(10) & _
                "Pour régler les droits d'accès, veuillez visiter " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Bienvenue dans l'espace sécurité! L'endroit à visiter quotidiennement!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Vos droits d'Accès pour l'utilisateur ""[n:0]"" ont été attribués. N'hésitez pas à nous visiter à:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Nous sommes heureux de pouvoir vous servir dans l'espace cybernétique."
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Bienvenue dans notre espace sécurité!"
                    SendPassword_EMailMessage = "Ci-joint, les données de votre profil utilisateur. Veuillez conserver le présent e-mail de confirmation avec le nom d'utilisateur et/ou le mot de passe et utiliser ces informations de manière confidentielle." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre mot de passe pour l'espace sécurité: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "N'oubliez pas de changer votre mot de passe le plus vite possible pour être certain que vous êtes le seul à l'utiliser!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Votre compte utilisateur vous offre la possibilité d'accéder à des applications sécurisées variées. Le processus d'inscription est déjà terminé. Vous pourrez alors visiter l'espace sécurité par l'URL suivant:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "L'administrateur a réinitialisé votre compte utilisateur. Ci-joint, les données de votre nouveau profil utilisateur. Veuillez conserver le présent e-mail de confirmation avec le nom d'utilisateur et/ou le mot de passe et utiliser ces informations de manière confidentielle." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Votre mot de passe pour l'espace sécurité: [n:0]    " & ChrW(13) & ChrW(10) & _
                "N'oubliez pas de changer votre mot de passe le plus vite possible pour être certain que vous êtes le seul à l'utiliser!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Votre compte utilisateur vous offre la possibilité d'accéder à des applications sécurisées variées. Le processus d'inscription est déjà terminé. Vous pourrez alors visiter l'espace sécurité par l'URL suivant:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Profitez des avantages d'une assistance étendue!"
                    HighlightTextTechnicalSupport = "Nous sommes juste en train de démarrer pour changer les choses."
                    HighlightTextExtro = "Une quantité innombrable d'informations vous sont dès maintenant proposées dans notre Extranet. Profitez-en!"
                    WelcomeTextWelcomeMessage = "Bienvenue dans notre espace sécurité!"
                    WelcomeTextFeedbackToContact = "Nous désirez d'autres fonctions? Pas de problème! N'hésitez pas à envoyer vos commentaires au sujet de l'Extranet à <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "L'endroit à visiter quotidiennement!"
                    UserManagementEMailTextDearUndefinedGender = "Cher/Chère "
                    UserManagementSalutationUnformalUndefinedGender = "Hâllo "
                    NavAreaNameLogin = "Connecter"
                    NavLinkNamePasswordRecovery = "Oublié le mot de passe?"
                    NavLinkNameNewUser = "Créer nouveau compte"
                    CreateAccount_Descr_MotivItemSupplier = "Fournisseur"
                    UpdateProfile_Descr_MotivItemSupplier = "Fournisseur"
                Case 2
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("de-DE")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("de-DE")
                    UpdateProfile_Descr_Title = "Benutzerprofil abändern"
                    UpdateProfile_Descr_Mobile = "Handy"
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Phone = "Telefon"
                    UpdateProfile_Descr_PositionInCompany = "Position"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Bitte geben Sie zum Fortfahren Werte in alle benötigten Felder ein!"
                    UpdateProfile_ErrMsg_MistypedPW = "Falsches Passwort. Bitte beachten Sie auch die Groß- und Kleinschreibung!"
                    UpdateProfile_ErrMsg_Undefined = "Unerwarteter Rückgabewert! - Bitte kontaktieren Sie unseren WebMaster!"
                    UpdateProfile_ErrMsg_Success = "Ihr Benutzerprofil wurde erfolgreich abgeändert!"
                    UpdateProfile_ErrMsg_LogonTooOften = "Anmeldeprozess schlug zu oft fehl; das Benutzerkonto wurde vorrübergehend deaktiviert.<br>Bitte versuchen Sie es etwas später wieder!"
                    UpdateProfile_ErrMsg_NotAllowed = "Sie haben nicht ausreichende Berechtigungen zum Durchführen dieser Aktion!"
                    UpdateProfile_ErrMsg_PWRequired = "Bitte geben Sie auch Ihr Passwort mit an um das Profil zu aktualisieren!"
                    UpdateProfile_Descr_Address = "Adresse"
                    UpdateProfile_Descr_Company = "Firma"
                    UpdateProfile_Descr_Addresses = "Anrede"
                    UpdateProfile_Descr_PleaseSelect = "(Bitte wählen!)"
                    UpdateProfile_Abbrev_Mister = "Herr"
                    UpdateProfile_Abbrev_Miss = "Frau"
                    UpdateProfile_Descr_AcademicTitle = "Akademischer Titel (z. B. ""Dr."")"
                    UpdateProfile_Descr_FirstName = "Vorname"
                    UpdateProfile_Descr_LastName = "Nachname"
                    UpdateProfile_Descr_NameAddition = "Namenszusatz"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_UserDetails = "Benutzerdetails"
                    UpdateProfile_Descr_1stLanguage = "1. bevorzugte Sprache"
                    UpdateProfile_Descr_2ndLanguage = "2. bevorzugte Sprache"
                    UpdateProfile_Descr_3rdLanguage = "3. bevorzugte Sprache"
                    UpdateProfile_Descr_Authentification = "Bitte bestätigen Sie die Änderungen mit Ihrem Passwort"
                    UpdateProfile_Descr_Password = "Passwort"
                    UpdateProfile_Descr_Submit = "Profil updaten"
                    UpdateProfile_Descr_RequiredFields = "* benötigte Felder"
                    UpdateProfile_Descr_CustomerSupplierData = "Kunden- bzw. Lieferantendaten"
                    UpdateProfile_Descr_CustomerNo = "Kunden-Nr."
                    UpdateProfile_Descr_SupplierNo = "Lieferanten-Nr."
                    UserJustCreated_Descr_AccountCreated = "Ihr Benutzerkonto wurde erfolgreich erstellt!"
                    UserJustCreated_Descr_LookAroundNow = "Sie können nun fortfahren und sich hier ein wenig umsehen."
                    UserJustCreated_Descr_PleaseNote = "Bitte beachten Sie: Momentan sind Sie <font color=""#336699"">Mitglied des öffentlichen Bereiches</font>. Zusätzliche Mitgliedschaften und Zugriffsrechte erhalten Sie innerhalb der nächsten 3 - 4 Werktage."
                    UserJustCreated_Descr_Title = "Willkommen!"
                    UpdatePW_Descr_Title = "Zurücksetzen des Passwortes"
                    UpdatePW_ErrMsg_ConfirmationFailed = "Die Passwortbestätigung stimmt nicht mit dem neuen Passwort überein. Bitte stellen Sie sicher, dass Sie das Passwort und die Passwortbestätigung gleich eingeben. Beachten Sie auch, dass beim Passwort zwischen Groß- und Kleinschreibung unterschieden wird."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Bitte geben Sie Ihr altes und Ihr neues Passwort ein. Beachten Sie auch, dass beim Passwort zwischen Groß- und Kleinschreibung unterschieden wird."
                    UpdatePW_ErrMsg_Undefined = "Unbekannter Fehler entdeckt!"
                    UpdatePW_ErrMsg_Success = "Das Passwort wurde erfolgreich geändert!"
                    UpdatePW_ErrMsg_WrongOldPW = "Das Passwort konnte nicht geändert werden. Bitte überprüfen Sie, ob Sie das aktuelle Passwort korrekt angegeben haben."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Bitte geben Sie alle benötigten Felder an, um die Passwortänderung abzuschließen!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Bitte geben Sie Ihr aktuelles und Ihr neues Kennwort an:"
                    UpdatePW_Descr_CurrentPW = "Aktuelles Passwort"
                    UpdatePW_Descr_NewPW = "Neues Passwort"
                    UpdatePW_Descr_NewPWConfirm = "Neues Passwort wiederholen"
                    UpdatePW_Descr_Submit = "Änderungen speichern"
                    UpdatePW_Descr_RequiredFields = "* benötigte Felder"
                    CreateAccount_Descr_CustomerSupplierData = "Kunden- bzw. Lieferantendaten"
                    CreateAccount_Descr_CustomerNo = "Kunden-Nr."
                    CreateAccount_Descr_SupplierNo = "Lieferanten-Nr."
                    CreateAccount_Descr_FollowingError = "Der folgende Fehler trat auf:"
                    CreateAccount_Descr_LoginDenied = "Login wurde verweigert!"
                    CreateAccount_Descr_Submit = "Benutzerkonto anlegen"
                    CreateAccount_Descr_RequiredFields = "benötigte Felder"
                    CreateAccount_Descr_BackToLogin = "Zurück zur Anmeldung"
                    CreateAccount_Descr_PageTitle = "Neues Benutzerkonto erstellen"
                    CreateAccount_Descr_UserLogin = "Anmeldedaten"
                    CreateAccount_Descr_NewLoginName = "Ihr neuer Benutzername"
                    CreateAccount_Descr_NewLoginPassword = "Ihr neues Passwort"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Passwortbestätigung"
                    CreateAccount_Descr_Address = "Adressangaben"
                    CreateAccount_Descr_Company = "Firma"
                    CreateAccount_Descr_Addresses = "Anrede"
                    CreateAccount_Descr_PleaseSelect = "(Bitte wählen!)"
                    CreateAccount_Descr_AcademicTitle = "Akademischer Titel (z. B. ""Dr."")"
                    CreateAccount_Descr_FirstName = "Vorname"
                    CreateAccount_Descr_LastName = "Nachname"
                    CreateAccount_Descr_NameAddition = "Namenszusatz"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Straße"
                    CreateAccount_Descr_ZIPCode = "PLZ"
                    CreateAccount_Descr_Location = "Ort"
                    CreateAccount_Descr_State = "Staat"
                    CreateAccount_Descr_Country = "Land"
                    CreateAccount_Descr_Motivation = "Was ist Ihre Motivation zur Registrierung"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Website Besucher"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Website Besucher"
                    CreateAccount_Descr_MotivItemDealer = "Händler"
                    UpdateProfile_Descr_MotivItemDealer = "Händler"
                    CreateAccount_Descr_MotivItemJournalist = "Journalist"
                    UpdateProfile_Descr_MotivItemJournalist = "Journalist"
                    CreateAccount_Descr_MotivItemOther = "Sonstiges, bitte angeben"
                    UpdateProfile_Descr_MotivItemOther = "Sonstiges, bitte angeben"
                    CreateAccount_Descr_WhereHeard = "Wie wurden Sie auf die Secured Area aufmerksam"
                    CreateAccount_Descr_WhereItemFriend = "Empfehlung eines Freundes"
                    UpdateProfile_Descr_WhereItemFriend = "Empfehlung eines Freundes"
                    CreateAccount_Descr_WhereItemResellerDealer = "Wiederverkäufer/Händler"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Wiederverkäufer/Händler"
                    CreateAccount_Descr_WhereItemExhibition = "Ausstellung/Messe"
                    UpdateProfile_Descr_WhereItemExhibition = "Ausstellung/Messe"
                    CreateAccount_Descr_WhereItemMagazines = "Zeitschrift"
                    UpdateProfile_Descr_WhereItemMagazines = "Zeitschrift"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "Hinweis eines Mitarbeiters"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "Hinweis eines Mitarbeiters"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Suchmaschine, bitte angeben"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Suchmaschine, bitte angeben"
                    CreateAccount_Descr_WhereItemOther = "Sonstiges, bitte angeben"
                    UpdateProfile_Descr_WhereItemOther = "Sonstiges, bitte angeben"
                    CreateAccount_Descr_UserDetails = "Benutzerangaben"
                    CreateAccount_Descr_Comments = "Kommentare"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Anfragen für zusätzliche Berechtigungen"
                    CreateAccount_Descr_1stPreferredLanguage = "1. bevorzugte Sprache"
                    CreateAccount_Descr_2ndPreferredLanguage = "2. bevorzugte Sprache"
                    CreateAccount_Descr_3rdPreferredLanguage = "3. bevorzugte Sprache"
                    CreateAccount_ErrorJS_InputValue = "Bitte geben Sie einen Wert in das Feld \""[n:0]\"" ein."
                    UpdateProfile_ErrorJS_InputValue = "Bitte geben Sie einen Wert in das Feld \""[n:0]\"" ein."
                    CreateAccount_ErrorJS_Length = "Bitte geben Sie einen Wert mit mindestens [n:0] Zeichen in das Feld \""[n:1]\"" ein."
                    UpdateProfile_ErrorJS_Length = "Bitte geben Sie einen Wert mit mindestens [n:0] Zeichen in das Feld \""[n:1]\"" ein."
                    Banner_Help = "Hilfe"
                    Banner_HeadTitle = "Secured Area - Logon"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Logon"
                    Banner_Feedback = "Feedback"
                    Logon_HeadTitle = "Secured Area - Anmeldung"
                    Logon_AskForForcingLogon = "Achtung! Sie sind bereits mit einer Sitzung eingeloggt. Wollen Sie diese beenden und eine neue beginnen?"
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Anmeldung"
                    Logon_SSO_ADS_PageTitle = "Einrichtung des automatischen Logins"
                    Logon_SSO_ADS_IdentifiedUserName = "Sie wurden als Benutzer <strong>{0}</strong> identifiziert."
                    Logon_SSO_ADS_LabelTakeAnAction = "Was möchten Sie tun?"
                    Logon_SSO_ADS_RadioRegisterExisting = "Registrieren Sie sich für ein <strong>bereits existierendes</strong> Benutzerkonto"
                    Logon_SSO_ADS_RadioRegisterNew = "Registrieren Sie sich mit einem <strong>neuen</strong> Benutzerkonto"
                    Logon_SSO_ADS_RadioDoNothing = "Sollte die Identifizierung nicht korrekt sein oder wenn Sie ohne Login weiter möchten, fahren Sie als anonymer Benutzer fort (dieser Dialog wird Ihnen später wieder angezeigt)."
                    Logon_SSO_ADS_ContactUs = "Sollten Sie Fragen haben, <a href=""mailto:{0}"">kontaktieren Sie uns</a> bitte."
                    Logon_SSO_ADS_ButtonNext = "Fertig stellen"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Login-Name:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Passwort:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Passwort (Wiederholung):"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "e-mail Adresse:"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "Sie wurden als Benutzer <strong>{0} ({1})</strong> identifiziert."
                    Logon_BodyPrompt2User = "Bitte geben Sie Ihren Benutzernamen und das dazugehörige Passwort ein, um die " & OfficialServerGroup_Title & " zu betreten.<br><em>Beachten Sie bitte auch, dass Benutzername und Passwort sich von anderen Zugangsdaten unterscheiden können, welche Sie für andere Bereiche erhalten haben.</em>"
                    Logon_BodyFormUserName = "Benutzername"
                    Logon_BodyFormUserPassword = "Passwort"
                    Logon_BodyFormSubmit = "Login"
                    Logon_BodyFormCreateNewAccount = "Benutzerkonto erstellen"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Noch kein Mitglied? Erstellen Sie sich doch Ihr eigenes Zugangskonto für den Bereich " & OfficialServerGroup_Title & "!</STRONG><BR>" & _
                                    "Falls Sie noch keine Zugangsdaten besitzen, können Sie sie </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>jetzt erstellen" & _
                                    "</FONT></A><FONT face=Arial size=2>. " & _
                                    "Bitte erstellen Sie keine anderen&nbsp;Zugangsdaten, " & _
                                    "wenn Sie bereits in der Vergangenheit welche erstellt hatten. Sollten Sie Schwierigkeiten bei der Anmeldung haben, kontaktieren Sie bitte unser <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>Support Service Center</FONT></A> " & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Passwort vergessen? Wir senden Ihnen Ihr Passwort " & _
                                    "</B><BR>Sie haben bereits gültige Zugangsdaten zugeteilt bekommen, haben jedoch das Passwort nicht mehr?" & _
                                    "</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>Hier erhalten Sie Ihr Passwort per e-mail</FONT></A><FONT " & _
                                    "face=Arial size=2>. Bitte beachten Sie, dass das e-mail nur an die e-mail-Adresse geschickt wird, welche von Ihnen ursprünglich angegeben wurde" & _
                                    ".<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Wurde Ihre Frage hier nicht beantwortet?</strong><br>Sollten Sie zusätzliche Unterstützung benötigen, können Sie uns gerne </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>kontaktieren</FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    Logon_Connecting_InProgress = "Sie werden mit dem Server verbunden…"
                    Logon_Connecting_LoginTimeout = "Zeitüberschreitung beim Login."
                    Logon_Connecting_RecommendationOnTimeout = "Sollte dieses Problem wiederholt auftreten, nehmen Sie bitte mit uns <a href=""mailto:[0]"">Kontakt</a> auf."
                    AccessError_Descr_FollowingError = "Der folgende Fehler trat auf:"
                    AccessError_Descr_LoginDenied = "Anmeldung wurde verweigert!"
                    AccessError_Descr_BackToLogin = "Zurück zur Anmeldung"
                    SendPassword_Descr_FollowingError = "Der folgende Fehler trat auf:"
                    SendPassword_Descr_LoginDenied = "Anmeldung wurde verweigert!"
                    SendPassword_Descr_Title = "Anfordern des Secured Area Passwortes via e-mail"
                    SendPassword_Descr_LoginName = "Benutzername"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "e-mail abschicken"
                    SendPassword_Descr_RequiredFields = "benötigte Felder"
                    SendPassword_Descr_BackToLogin = "Zurück zur Anmeldung"
                    SendPassword_Descr_PasswordSentTo = "Das Passwort wurde verschickt nach {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "Ihr Secured Area Passwort wird nur an Ihre hinterlegte e-mail Adresse gesendet.<BR>Sollten Sie diese e-mail Nachricht nicht innerhalb der nächsten 24 Stunden erhalten, so kontaktieren Sie bitte <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "DE"
                    StatusLineUsername = "Benutzer"
                    StatusLinePassword = "Passwort"
                    StatusLineSubmit = "Login"
                    StatusLineEditorial = "Impressum"
                    StatusLineContactUs = "Kontakt"
                    StatusLineDataprotection = "Datenschutz"
                    StatusLineLoggedInAs = "Angemeldet als"
                    StatusLineLegalNote = "Datenschutz und rechtliche Hinweise"
                    StatusLineCopyright_AllRightsReserved = "Alle Rechte vorbehalten."
                    NavAreaNameYourProfile = "Ihr Profil"
                    NavLinkNameUpdatePasswort = "Passwort ändern"
                    NavLinkNameUpdateProfile = "Benutzerdaten ändern"
                    NavLinkNameLogout = "Abmelden"
                    NavLinkNameLogin = "Login"
                    NavPointUpdatedHint = "Hier wurde etwas neu erstellt oder etwas vorhandenes aktualisiert"
                    NavPointTemporaryHiddenHint = "Diese Anwendung ist vorrübergehend für andere Benutzer deaktiviert. Häufig ist dies ein Indiz, dass sich diese Anwendung noch in der Entwicklungsphase befindet."
                    SystemButtonYes = "Ja"
                    SystemButtonNo = "Nein"
                    SystemButtonOkay = "Okay"
                    SystemButtonCancel = "Abbrechen"
                    ErrorUserOrPasswordWrong = "Der Benutzername oder das Passwort ist nicht korrekt oder wurde falsch geschrieben oder der Zugriff wurde verweigert!<p>Bitte überprüfen Sie <ul><li>die Schreibweise vom Benutzernamen und dem Passwort (das Passwort selbst unterscheidet zwischen großen und kleinen Buchstaben!)</li><li>dass Sie eine gültige Konstellation von Benutzernamen und Passwort verwenden. (Eventuell haben Sie bereits andere Benutzernamen/Passwörter von uns erhalten, welche jedoch nicht für diesen Bereich gültig sind.)</li></ul>"
                    ErrorServerConfigurationError = "Dieser Server ist noch nicht korrekt eingerichtet. Bitte konsultieren Sie Ihren Administrator."
                    ErrorNoAuthorization = "Sie haben keine Berechtigung, auf diesen Bereich zuzugreifen."
                    ErrorAlreadyLoggedOn = "Sie sind bereits angemeldet! Bitte melden Sie sich zuerst an Ihrem anderen Arbeitsplatz ab!<br><font color=""red"">Wenn Sie sicher sind, dass Sie nirgends mehr angemeldet sind, senden Sie uns bitte eine kurze e-mail an <a href=""mailto:[n:0]"">[n:1]</a> und nennen Sie uns Ihren Loginnamen.</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Sie wurden an diesem Arbeitsplatz abgemeldet, da Sie sich an einer anderen Station angemeldet haben.<br>"
                    ErrorLogonFailedTooOften = "Der Anmeldeprozess ist zu oft fehlgeschlagen, Ihr Konto wurde vorrübergehend deaktiviert.<br>Bitte versuchen Sie es später noch ein mal!"
                    ErrorEmptyPassword = "Bitte vergessen Sie nicht noch ein Passwort anzugeben!<br>Wenn Sie Ihr Passwort nicht mehr wissen, können Sie es über e-mail neu anfordern. Bitte sehen Sie hierzu weitere Details weiter unten im Text."
                    ErrorUnknown = "Unerwarteter Fehler! - Bitte kontaktieren Sie unser <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Bitte geben Sie Werte in alle Felder ein, welche mit einem Sternchen <em>(*)</em> versehen sind!"
                    ErrorWrongNetwork = "Sie haben keine Berechtigung, sich über Ihre aktuelle Netzwerkverbindung anzumelden."
                    ErrorUserAlreadyExists = "Das Benutzerkonto existiert bereits. Bitte wählen Sie einen anderen Anmeldenamen!"
                    ErrorLoginCreatedSuccessfully = "Das Benutzerprofil wurde erfolgreich erstellt!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Falscher Benutzername oder falsche e-mail Adresse.<br>Bitte geben Sie die korrekten Werte an, so wie sie in Ihrem Benutzerprofil hinterlegt sind."
                    ErrorCookiesMustNotBeDisabled = "Ihr Browser unterstützt keine Cookies oder Cookies wurden aufgrund von Sicherheitseinstellungen in Ihrem Browser deaktiviert."
                    ErrorTimoutOrLoginFromAnotherStation = "Sie wurden abgemeldet, da die maximale Sitzungsdauer erreicht wurde oder ein Login von einer anderen Arbeitsstation vorgenommen wurde."
                    ErrorApplicationConfigurationIsEmpty = "Diese Anwendung enthielt keinen gültigen Anwendungsnamen. Bitte setzen Sie sich mit dem Hersteller in Verbindung."
                    InfoUserLoggedOutSuccessfully = "Sie wurden erfolgreich abgemeldet. Wir bedanken uns für Ihren Besuch."
                    UserManagementEMailColumnTitleLogin = "Anmeldename: "
                    UserManagementEMailColumnTitleCompany = "Firma: "
                    UserManagementEMailColumnTitleName = "Name: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Straße: "
                    UserManagementEMailColumnTitleZIPCode = "PLZ: "
                    UserManagementEMailColumnTitleLocation = "Ort: "
                    UserManagementEMailColumnTitleState = "Staat: "
                    UserManagementEMailColumnTitleCountry = "Land: "
                    UserManagementEMailColumnTitle1stLanguage = "1. bevorzugte Sprache: "
                    UserManagementEMailColumnTitle2ndLanguage = "2. bevorzugte Sprache: "
                    UserManagementEMailColumnTitle3rdLanguage = "3. bevorzugte Sprache: "
                    UserManagementEMailColumnTitleComesFrom = "Kommt von: "
                    UserManagementEMailColumnTitleMotivation = "Motivation: "
                    UserManagementEMailColumnTitleCustomerNo = "Kunden-Nr.: "
                    UserManagementEMailColumnTitleSupplierNo = "Lieferanten-Nr.: "
                    UserManagementEMailColumnTitleComment = "Kommentar: "
                    UserManagementAddressesMr = "Herr "
                    UserManagementAddressesMs = "Frau "
                    UserManagementSalutationUnformalMasculin = "Hallo "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Sehr geehrte Damen und Herren, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Sehr geehrter Herr "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Hallo zusammen, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Hallo "
                    UserManagementSalutationFormulaUnformalGroup = "Hallo zusammen, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Sehr geehrte Frau "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagementEMailTextRegards = "Mit freundlichen Grüßen"
                    UserManagementEMailTextSubject = "Ihre Zugangsdaten für die Secured Area"
                    UserManagementEMailTextSubject4AdminNewUser = "Secured Area - Neuer Benutzer"
                    UserManagementMasterServerAvailableInNearFuture = "Achtung: Dieser Server wird erst in naher Zukunft verfügbar sein."
                    CreateAccount_MsgEMailWelcome = "Willkommen bei der Secured Area! Hier finden Sie tägliche und ortsungebundene Informationen." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Anmeldename für die Secured Area: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Nehmen Sie die Vorteile unserer umfangreichen Unterstützung in Anspruch! Schauen Sie doch einfach mal vorbei!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Benutzerkonto bietet Ihnen Zugang zu den diversen geschützten Anwendungen. Der Anmeldeprozess für die Secured Area ist bereits abgeschlossen. Ihre vollen Berechtigungen werden jedoch erst in ca. 3 - 4 Werktagen gültig sein. Sie können mit folgender URL in die Secured Area einsteigen:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Willkommen bei der Secured Area! Hier finden Sie tägliche und ortsungebundene Informationen." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Anmeldename für die Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Ihr Passwort: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Bitte vergessen Sie nicht, das Passwort sobald wie möglich abzuändern. Dies ist nötig um sicherzustellen, dass niemand anderes (z. B. Hacker) Ihr Passwort besitzt und benutzen kann!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Nehmen Sie die Vorteile einer umfangreichen Unterstützung in Anspruch! Es wartet bereits jetzt einiges auf Sie. Schauen Sie doch einfach mal vorbei!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Benutzerkonto bietet Ihnen Zugang zu den diversen geschützten Anwendungen. Der Anmeldeprozess für die Secured Area ist bereits abgeschlossen. Ihre vollen Berechtigungen werden jedoch erst in ca. 3 - 4 Werktagen gültig sein. Sie können mit folgender URL in die Secured Area einsteigen:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMail4Admin = "Der folgende Benutzer hat sich am Secured Area-Bereich neu angemeldet." & ChrW(13) & ChrW(10) & _
                "Bitte vergeben Sie die benötigten Zugriffsrechte!" & ChrW(13) & ChrW(10) & _
                "Um Zugriffsrechte einzustellen, besuchen Sie bitte " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextWelcome = "Willkommen bei der Secured Area! Hier finden Sie tägliche und ortsungebundene Informationen." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Sie haben ein Benutzerkonto für den Bereich Secured Area von unserer Verwaltung eingerichtet bekommen. Dieser Service ist für Sie natürlich kostenlos." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Anmeldename für die Secured Area: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Ihr Passwort für die Secured Area lautet: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Bitte vergessen Sie nicht, das Passwort sobald wie möglich abzuändern. Dies ist nötig um sicherzustellen, dass niemand anderes (z. B. Hacker) Ihr Passwort besitzt und benutzen kann!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Nehmen Sie die Vorteile einer umfangreichen Unterstützung Anspruch! Es wartet bereits jetzt einiges auf Sie. Schauen Sie doch einfach mal vorbei!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Benutzerkonto bietet Ihnen Zugang zu den diversen geschützten Anwendungen. Der Anmeldeprozess für die Secured Area ist bereits abgeschlossen. Ihre vollen Berechtigungen werden jedoch erst in ca. 3 - 4 Werktagen gültig sein. Sie können mit folgender URL in die Secured Area einsteigen:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Willkommen bei der Secured Area! Hier finden Sie tägliche und ortsungebundene Informationen.</p>" & _
                "<p>Sie haben ein Benutzerkonto für den Bereich Secured Area von unserer Verwaltung eingerichtet bekommen. Dieser Service ist für Sie natürlich kostenlos.</p>" & _
                "<p><strong>Ihr Anmeldename für die Secured Area: <font color=""red"">[n:0]</font><br>" & _
                "Ihr Passwort für die Secured Area lautet: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Bitte vergessen Sie nicht, das Passwort sobald wie möglich abzuändern. Dies ist nötig um sicherzustellen, dass niemand anderes (z. B. Hacker) Ihr Passwort besitzt und benutzen kann!</p>" & _
                "<p>Nehmen Sie die Vorteile einer umfangreichen Unterstützung Anspruch! Es wartet bereits jetzt einiges auf Sie. Schauen Sie doch einfach mal vorbei!" & _
                "<p>Ihr Benutzerkonto bietet Ihnen Zugang zu den diversen geschützten Anwendungen. Der Anmeldeprozess für die Secured Area ist bereits abgeschlossen. Ihre vollen Berechtigungen werden jedoch erst in ca. 3 - 4 Werktagen gültig sein. Sie können mit folgender URL in die Secured Area einsteigen:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    UserManagement_NewUser_MsgEMail4Admin = "Der folgende Benutzer wurde von Ihnen oder einem Ihrer Kollegen im Secured Area-Bereich angelegt." & ChrW(13) & ChrW(10) & _
                "Bitte vergeben Sie die benötigten Zugriffsrechte!" & ChrW(13) & ChrW(10) & _
                "Um Zugriffsrechte einzustellen, besuchen Sie bitte " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Willkommen in der Secured Area!"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Willkommen bei der Secured Area! Hier finden Sie tägliche und ortsungebundene Informationen." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihre Zugriffsrechte für den Benutzer ""[n:0]"" wurden zugewiesen. Sie können mit folgender URL in die Secured Area einsteigen:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Wir freuen uns auf ein Wiedersehen hier mit Ihnen."
                    SendPassword_EMailMessage = "Im Folgenden finden Sie Ihre Benutzerprofildaten. Bitte bewahren Sie diese e-mail mit Benutzernamen und/oder Passwort auf und behandeln Sie diese Informationen vertraulich." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Passwort für die Secured Area lautet: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Bitte vergessen Sie nicht, das Passwort sobald wie möglich abzuändern. Dies ist nötig um sicherzustellen, dass niemand anderes (z. B. Hacker) Ihr Passwort besitzt und benutzen kann!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "Ein Benutzerkonto bietet Ihnen Zugang zu den diversen geschützten Anwendungen. Der Anmeldeprozess für die Secured Area ist bereits abgeschlossen. Sie können mit folgender URL in die Secured Area einsteigen:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Bitte geben Sie hier Ihr neues Passwort an:"
                    SendPasswordResetLink_EMailMessage = "Sie können Ihr Passwort über folgenden Link zurücksetzen: [n:0]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "Der Sicherheitsbeauftragte hat Ihr Benutzerkonto zurückgesetzt. Im Folgenden sehen Sie Ihre neuen Benutzerprofildaten. Bitte bewahren Sie diese e-mail mit Benutzernamen und Passwort auf und behandeln Sie diese Informationen vertraulich." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Ihr Passwort für die Secured Area lautet: [n:0]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Bitte vergessen Sie nicht, das Passwort sobald wie möglich abzuändern. Dies ist nötig um sicherzustellen, dass niemand anderes (z. B. Hacker) Ihr Passwort besitzt und benutzen kann!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Ein Benutzerkonto bietet Ihnen Zugang zu den diversen geschützten Anwendungen. Der Anmeldeprozess für die Secured Area ist bereits abgeschlossen. Sie können mit folgender URL in die Secured Area einsteigen:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Nutzen Sie die Vorteile einer umfangreichen Unterstützung!"
                    HighlightTextTechnicalSupport = "Wir starten gerade durch, und es wird sich noch manches ändern."
                    HighlightTextExtro = "Aber es wartet bereits jetzt schon einiges auf Sie. Nutzen Sie die Gelegenheit!"
                    WelcomeTextWelcomeMessage = "Willkommen in der Secured Area!"
                    WelcomeTextFeedbackToContact = "Benötigen Sie noch weitere Funktionen? Kein Problem! Senden Sie uns Ihre Kommentare zum Extranet an <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "Hier finden Sie tägliche und ortsungebundene Informationen."
                    UpdateProfile_Descr_Street = "Straße"
                    UpdateProfile_Descr_ZIPCode = "PLZ"
                    UpdateProfile_Descr_Location = "Ort"
                    UpdateProfile_Descr_State = "Staat"
                    UpdateProfile_Descr_Country = "Land"
                    UpdatePW_Error_PasswordComplexityPolicy = "Das Passwort muss aus mindestens 3 Zeichen bestehen. Es dürfen keine Bestandteile Ihres Namens verwendet werden."
                    ErrorRequiredField = "Benötigtes Feld"
                    UserManagementEMailTextDearUndefinedGender = "Sehr geehrte(r) "
                    UserManagementSalutationUnformalUndefinedGender = "Hallo "
                    NavAreaNameLogin = "Anmelden"
                    NavLinkNamePasswordRecovery = "Passwort vergessen?"
                    NavLinkNameNewUser = "Neues Konto anlegen"
                    CreateAccount_Descr_MotivItemSupplier = "Lieferant"
                    UpdateProfile_Descr_MotivItemSupplier = "Lieferant"
                Case Else
                    System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en-GB")
                    System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-GB")
                    Logon_AskForForcingLogon = "Attention: You are already logged on with another session. Do you want to cancel this session and start a new one?"
                    UserManagementSalutationUnformalMasculin = "Hello "
                    UserManagementSalutationFormulaUnformalMasculin = "{SalutationUnformalMasculin}{SalutationMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsMasculin = "{SalutationInMailsMasculin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsGroup = "Dear Sirs, "
                    UserManagementSalutationFormulaGroup = ""
                    UserManagementSalutationFormulaUnformalWithFirstNameMasculin = "{SalutationUnformalMasculin}{FirstName}, "
                    UserManagementEMailTextDearMr = "Dear Mr. "
                    UserManagementSalutationFormulaUnformalWithFirstNameGroup = "Hello together, "
                    UserManagementSalutationFormulaUnformalUndefinedGender = "{SalutationUnformalUndefinedGender}{FullName}, "
                    UserManagementSalutationUnformalFeminin = "Hello "
                    UserManagementSalutationFormulaUnformalGroup = "Hello together, "
                    UserManagementSalutationFormulaUnformalFeminin = "{SalutationUnformalFeminin}{SalutationFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaInMailsFeminin = "{SalutationInMailsFeminin}{SalutationNameOnly}, "
                    UserManagementSalutationFormulaUndefinedGender = "{FullName}"
                    UserManagementSalutationFormulaFeminin = "{SalutationFeminin}{SalutationNameOnly}"
                    UserManagementSalutationFormulaUnformalWithFirstNameFeminin = "{SalutationUnformalFeminin}{FirstName}, "
                    UserManagementSalutationFormulaMasculin = "{SalutationMasculin}{SalutationNameOnly}"
                    UserManagementEMailTextDearMs = "Dear Ms. "
                    UserManagementSalutationFormulaUnformalWithFirstNameUndefinedGender = "{SalutationUnformalUndefinedGender}{FirstName}, "
                    UserManagementSalutationFormulaInMailsUndefinedGender = "{SalutationInMailsUndefinedGender}{FullName}, "
                    UserManagement_NewUser_TextWelcome = "Welcome to Secured Area! The place to go on every day!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "You have been added to the Secured Area by our administration free of charge." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your login name is: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Your Secured Area password is: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Please don't forget to amend the password yourself as soon as possible to ensure nobody else has got it and can use it!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Get the advantages of extensive support! There is a lot of information waiting for you in our Extranet. Please feel free to surf through this site and explore it all." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your account provides access to various secured programs. By now, you should have completed the association process of Secured Area. Your full authorizations will be applicable in the next 3 - 4 days. Then please revisit the URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    UserManagement_NewUser_HTMLWelcome = "<p>Welcome to Secured Area! The place to go on every day!</p>" & _
                "<p>You have been added to the Secured Area by our administration free of charge.</p>" & _
                "<p><strong>Your login name is: <font color=""red"">[n:0]</font><br>" & _
                "Your Secured Area password is: <font color=""red"">[n:1]</font></strong></p>" & _
                "<p>Please don't forget to amend the password yourself as soon as possible to ensure nobody else has got it and can use it!</p>" & _
                "<p>Get the advantages of extensive support! There is a lot of information waiting for you in our Extranet. Please feel free to surf through this site and explore it all.</p>" & _
                "<p>Your account provides access to various secured programs. By now, you should have completed the association process of Secured Area. Your full authorizations will be applicable in the next 3 - 4 days. Then please revisit the URL:<br>" & _
                "<ul><strong>[n:2]</strong></ul></p>"
                    SendPasswordResetLink_EMailMessage = "You can reset your password using the following link: [n:0]"
                    ResetPW_Descr_PleaseSpecifyNewPW = "Please specifiy your new password here:"
                    UpdateProfile_Descr_Title = "Change Profile"
                    UpdateProfile_Descr_Mobile = "Mobile"
                    UpdateProfile_Descr_Fax = "Fax"
                    UpdateProfile_Descr_Phone = "Phone"
                    UpdateProfile_Descr_PositionInCompany = "Position"
                    UpdateProfile_ErrMsg_InsertAllRequiredFields = "Please input values into all required fields to continue!"
                    UpdateProfile_ErrMsg_MistypedPW = "Mistyped or misspelled password!"
                    UpdateProfile_ErrMsg_Undefined = "Unexpected return value! - Please contact the webmaster!"
                    UpdateProfile_ErrMsg_Success = "Your profile has been updated successfully!"
                    UpdateProfile_ErrMsg_LogonTooOften = "Logon process has failed too often, account has been disabled.<br>Please try again later!"
                    UpdateProfile_ErrMsg_NotAllowed = "You are not allowed to access this document!"
                    UpdateProfile_ErrMsg_PWRequired = "Please submit your password to modify your profile!"
                    UpdateProfile_Descr_Address = "Address"
                    UpdateProfile_Descr_Company = "Company"
                    UpdateProfile_Descr_Addresses = "Salutation"
                    UpdateProfile_Descr_PleaseSelect = "(Please select!)"
                    UpdateProfile_Abbrev_Mister = "Mr."
                    UpdateProfile_Abbrev_Miss = "Ms."
                    UpdateProfile_Descr_AcademicTitle = "Academic Title (e.g. ""Dr."")"
                    UpdateProfile_Descr_FirstName = "First name"
                    UpdateProfile_Descr_LastName = "Last name"
                    UpdateProfile_Descr_NameAddition = "Name affix"
                    UpdateProfile_Descr_EMail = "e-mail"
                    UpdateProfile_Descr_Street = "Street"
                    UpdateProfile_Descr_ZIPCode = "ZIP Code"
                    UpdateProfile_Descr_Location = "Location"
                    UpdateProfile_Descr_State = "State"
                    UpdateProfile_Descr_Country = "Country"
                    UpdateProfile_Descr_UserDetails = "User details"
                    UpdateProfile_Descr_1stLanguage = "1st preferred language"
                    UpdateProfile_Descr_2ndLanguage = "2nd preferred language"
                    UpdateProfile_Descr_3rdLanguage = "3rd preferred language"
                    UpdateProfile_Descr_Authentification = "Please confirm these changes with your password"
                    UpdateProfile_Descr_Password = "Password"
                    UpdateProfile_Descr_Submit = "Update profile"
                    UpdateProfile_Descr_RequiredFields = "* required fields"
                    UpdateProfile_Descr_CustomerSupplierData = "Customer/supplier data"
                    UpdateProfile_Descr_CustomerNo = "Customer no."
                    UpdateProfile_Descr_SupplierNo = "Supplier no."
                    UserJustCreated_Descr_AccountCreated = "Your user account has been created successfully!"
                    UserJustCreated_Descr_LookAroundNow = "You can proceed and view around, now."
                    UserJustCreated_Descr_PleaseNote = "Please note: At this time <font color=""#336699"">you are a public member</font>. Additional authorizations and memberships will be set in the next 3 - 4 days."
                    UserJustCreated_Descr_Title = "Secured Area - Welcome!"
                    UpdatePW_Descr_Title = "Reset password"
                    UpdatePW_ErrMsg_ConfirmationFailed = "Password confirmation failed! Please ensure you retyped the password correctly. Please pay attention to upper and lower case as well."
                    UpdatePW_ErrMsg_InsertAllRequiredPWFields = "Please input your current password and a new, different one. Please pay attention to upper and lower case as well."
                    UpdatePW_ErrMsg_Undefined = "Undefined error detected!"
                    UpdatePW_ErrMsg_Success = "Password changed successfully!"
                    UpdatePW_ErrMsg_WrongOldPW = "Password couldn't be changed! Please verify you have entered your correct current password."
                    UpdatePW_ErrMsg_InsertAllRequiredFields = "Please input values into all required fields to complete the password change!"
                    UpdatePW_Descr_PleaseSpecifyCurrendAndOldPW = "Please specify your current and your new password:"
                    UpdatePW_Descr_CurrentPW = "Current password"
                    UpdatePW_Descr_NewPW = "New password"
                    UpdatePW_Descr_NewPWConfirm = "Confirm new password"
                    UpdatePW_Descr_Submit = "Update profile"
                    UpdatePW_Descr_RequiredFields = "* required fields"
                    UpdatePW_Error_PasswordComplexityPolicy = "The password must consist of at least 3 characters. No elements of your name may be used."
                    CreateAccount_Descr_CustomerSupplierData = "Customer/supplier data"
                    CreateAccount_Descr_CustomerNo = "Customer no."
                    CreateAccount_Descr_SupplierNo = "Supplier no."
                    CreateAccount_Descr_FollowingError = "The following error has occured:"
                    CreateAccount_Descr_LoginDenied = "Login has been denied!"
                    CreateAccount_Descr_Submit = "Create user account"
                    CreateAccount_Descr_RequiredFields = "required fields"
                    CreateAccount_Descr_BackToLogin = "Back to login"
                    CreateAccount_Descr_PageTitle = "Create new user"
                    CreateAccount_Descr_UserLogin = "User login"
                    CreateAccount_Descr_NewLoginName = "Your new login name"
                    CreateAccount_Descr_NewLoginPassword = "Your new password"
                    CreateAccount_Descr_NewLoginPasswordConfirmation = "Confirm your password"
                    CreateAccount_Descr_Address = "Address"
                    CreateAccount_Descr_Company = "Company"
                    CreateAccount_Descr_Addresses = "Salutation"
                    CreateAccount_Descr_PleaseSelect = "(Please select!)"
                    CreateAccount_Descr_AcademicTitle = "Academic Title (e.g. ""Dr."")"
                    CreateAccount_Descr_FirstName = "First name"
                    CreateAccount_Descr_LastName = "Last name"
                    CreateAccount_Descr_NameAddition = "Name affix"
                    CreateAccount_Descr_Email = "e-mail"
                    CreateAccount_Descr_Street = "Street"
                    CreateAccount_Descr_ZIPCode = "ZIP code"
                    CreateAccount_Descr_Location = "Location"
                    CreateAccount_Descr_State = "State"
                    CreateAccount_Descr_Country = "Country"
                    CreateAccount_Descr_Motivation = "What is your motivation to register"
                    CreateAccount_Descr_MotivItemWebSiteVisitor = "Website Visitor"
                    UpdateProfile_Descr_MotivItemWebSiteVisitor = "Website Visitor"
                    CreateAccount_Descr_MotivItemDealer = "Dealer"
                    UpdateProfile_Descr_MotivItemDealer = "Dealer"
                    CreateAccount_Descr_MotivItemJournalist = "Journalist"
                    UpdateProfile_Descr_MotivItemJournalist = "Journalist"
                    CreateAccount_Descr_MotivItemOther = "Other, please specifiy"
                    UpdateProfile_Descr_MotivItemOther = "Other, please specifiy"
                    CreateAccount_Descr_WhereHeard = "Where have you heard about our Secured Area"
                    CreateAccount_Descr_WhereItemFriend = "Friend"
                    UpdateProfile_Descr_WhereItemFriend = "Friend"
                    CreateAccount_Descr_WhereItemResellerDealer = "Reseller/Dealer"
                    UpdateProfile_Descr_WhereItemResellerDealer = "Reseller/Dealer"
                    CreateAccount_Descr_WhereItemExhibition = "Exhibition"
                    UpdateProfile_Descr_WhereItemExhibition = "Exhibition"
                    CreateAccount_Descr_WhereItemMagazines = "Magazines"
                    UpdateProfile_Descr_WhereItemMagazines = "Magazines"
                    CreateAccount_Descr_WhereItemFromUsOurselves = "from ourselves"
                    UpdateProfile_Descr_WhereItemFromUsOurselves = "from ourselves"
                    CreateAccount_Descr_WhereItemSearchEnginge = "Search engine, please specify"
                    UpdateProfile_Descr_WhereItemSearchEnginge = "Search engine, please specify"
                    CreateAccount_Descr_WhereItemOther = "Other, please specifiy"
                    UpdateProfile_Descr_WhereItemOther = "Other, please specifiy"
                    CreateAccount_Descr_UserDetails = "User details"
                    CreateAccount_Descr_Comments = "Comments"
                    CreateAccount_Descr_RequestAdditionalAuthorizations = "Requests for additional authorizations"
                    CreateAccount_Descr_1stPreferredLanguage = "1st preferred language"
                    CreateAccount_Descr_2ndPreferredLanguage = "2nd preferred language"
                    CreateAccount_Descr_3rdPreferredLanguage = "3rd preferred language"
                    CreateAccount_ErrorJS_InputValue = "Please input a value into field \""[n:0]\""."
                    UpdateProfile_ErrorJS_InputValue = "Please input a value into field \""[n:0]\""."
                    CreateAccount_ErrorJS_Length = "Please input a value with at least [n:0] characters into field \""[n:1]\""."
                    UpdateProfile_ErrorJS_Length = "Please input a value with at least [n:0] characters into field \""[n:1]\""."
                    Banner_Help = "Help"
                    Banner_HeadTitle = "Secured Area - Logon"
                    Banner_BodyTitle = OfficialServerGroup_Title & " - Logon"
                    Banner_Feedback = "Feedback"
                    Logon_Connecting_InProgress = "You will be connected to the server…"
                    Logon_HeadTitle = "Secured Area - Logon"
                    Logon_Connecting_LoginTimeout = "Login timed out."
                    Logon_Connecting_RecommendationOnTimeout = "If this problem takes place again, please <a href=""mailto:[0]"">contact</a> us."
                    Logon_BodyTitle = OfficialServerGroup_Title & " - Logon"
                    Logon_SSO_ADS_PageTitle = "Setup of your automatic logon"
                    Logon_SSO_ADS_IdentifiedUserName = "You've been identified as user <strong>{0}</strong>."
                    Logon_SSO_ADS_LabelTakeAnAction = "What do you want to do now?"
                    Logon_SSO_ADS_RadioRegisterExisting = "Register for an <strong>already existing</strong> account"
                    Logon_SSO_ADS_RadioRegisterNew = "Register for a <strong>new</strong> account"
                    Logon_SSO_ADS_RadioDoNothing = "If the identification is wrong or you want to proceed without login now, continue as an anonymous user (you'll be asked later again)."
                    Logon_SSO_ADS_ContactUs = "If you have any questions please <a href=""mailto:{0}"">contact us</a>."
                    Logon_SSO_ADS_ButtonNext = "Continue"
                    Logon_SSO_ADS_LabelRegisterExistingLoginName = "Login name:"
                    Logon_SSO_ADS_LabelRegisterExistingPassword = "Password:"
                    Logon_SSO_ADS_LabelRegisterNewPassword2 = "Retype password:"
                    Logon_SSO_ADS_LabelRegisterNewEMail = "e-mail address:"
                    Logon_SSO_ADS_IdentifiedUserNameWithAdsUserInfo = "You've been identified as user <strong>{0} ({1})</strong>."
                    Logon_BodyPrompt2User = "Enter your login name and password to access the " & OfficialServerGroup_Title & ".<br><em>Please note that this login name and password are separate and may differ from those you've already got for our other areas.</em>"
                    Logon_BodyFormUserName = "Login name"
                    Logon_BodyFormUserPassword = "Password"
                    Logon_BodyFormSubmit = "Login"
                    Logon_BodyFormCreateNewAccount = "Create new account"
                    Logon_BodyExplanation = "<TABLE BORDER=""0"" CELLPADDING=""3"" CELLSPACING=""0""><TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/handshake.gif"" border=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><STRONG>Not a member? Create a new account to gain access to the " & OfficialServerGroup_Title & "!</STRONG><BR>" & _
                                    "If you haven't got an account, you can </FONT><A href=""" & User_Auth_Config_Paths_SystemData & "account_register.aspx""><FONT face=Arial size=2>create " & _
                                    "one now</FONT></A><FONT face=Arial size=2>. " & _
                                    "Please do not create another&nbsp;" & _
                                    "account if you have already created one in the past. If you have got " & _
                                    "difficulties logging on then please contact your <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>support service center</FONT></A>" & _
                                    ". <BR> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/passwort.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><B>Forgotten the password? Send your Password to " & _
                                    "you via e-mail</B><BR>You may have supplied a valid&nbsp;account but have forgotten " & _
                                    "your password.</FONT> <A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "account_sendpassword.aspx""><FONT " & _
                                    "face=Arial size=2>Here you can get your password e-mailed to you now</FONT></A><FONT " & _
                                    "face=Arial size=2>. Note that your " & _
                                    "password will be e-mailed to the address you supplied when you originally " & _
                                    "created this&nbsp; account.<br> &nbsp;</FONT></P></TD></TR>" & _
                                    "<TR><TD VALIGN=""TOP""><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><IMG WIDTH=""40"" HEIGHT=""40"" SRC=""" & User_Auth_Config_UserAuthMasterServer & User_Auth_Config_Paths_StandardIncludes & "images/help.gif"" BORDER=""0""></A></TD><TD VALIGN=""TOP""><P><FONT face=Arial size=2><strong>Still having trouble?</strong><br>If you require additional support, please don't hesitate to </FONT><A " & _
                                    "href=""" & User_Auth_Config_Paths_SystemData & "feedback.aspx""><FONT face=Arial size=2>contact us</FONT></A><FONT " & _
                                    "size=2>.</FONT></P></TD></TR></TABLE>"
                    AccessError_Descr_FollowingError = "The following error has occured:"
                    AccessError_Descr_LoginDenied = "Login has been denied!"
                    AccessError_Descr_BackToLogin = "Back to login"
                    SendPassword_Descr_FollowingError = "The following error has occured:"
                    SendPassword_Descr_LoginDenied = "Login has been denied!"
                    SendPassword_Descr_Title = "Request Secured Area password by e-mail"
                    SendPassword_Descr_LoginName = "Login name"
                    SendPassword_Descr_Email = "e-mail"
                    SendPassword_Descr_Submit = "Send e-mail"
                    SendPassword_Descr_RequiredFields = "required fields"
                    SendPassword_Descr_BackToLogin = "Back to login"
                    SendPassword_Descr_PasswordSentTo = "Your password has been sent to {0}."
                    SendPassword_Descr_FurtherCommentWithContactAddress = "Your Secured Area password will be sent only to your e-mail address on record.<BR>If you do not receive the e-mail message within twenty-four (24) hours, please contact <a href=""mailto:{0}"">{1}</a>."
                    META_CurrentContentLanguage = "EN"
                    StatusLineUsername = "User"
                    StatusLinePassword = "Password"
                    StatusLineSubmit = "Go!"
                    StatusLineEditorial = "Editorial"
                    StatusLineContactUs = "Contact us"
                    StatusLineDataprotection = "Privacy statement"
                    StatusLineLoggedInAs = "Logged in as"
                    StatusLineLegalNote = "Privacy statement and legal note"
                    StatusLineCopyright_AllRightsReserved = "All rights reserved."
                    NavAreaNameYourProfile = "Your profile"
                    NavLinkNameUpdatePasswort = "Change password"
                    NavLinkNameUpdateProfile = "Change profile"
                    NavLinkNameLogout = "Logout"
                    NavLinkNameLogin = "Logon"
                    NavPointUpdatedHint = "Here are some items new or updated"
                    NavPointTemporaryHiddenHint = "This application has been locked down temporary for other users. This application might be under construction."
                    SystemButtonYes = "Yes"
                    SystemButtonNo = "No"
                    SystemButtonOkay = "Okay"
                    SystemButtonCancel = "Cancel"
                    ErrorUserOrPasswordWrong = "Mistyped or misspelled user name or password or access denied!<p>Please verify <ul><li>the spelling of user name and password (the password itself differs between big and small letters!)</li><li>that you are using the correct user name/password constellation (maybe you've alread got passwords for our other resources, but they won't work here)</li></ul>"
                    ErrorServerConfigurationError = "This server has not been correctly configured yet. Please consult your administrator."
                    ErrorNoAuthorization = "You don't have any authorization to access this area."
                    ErrorAlreadyLoggedOn = "Already logged on! Please logout first on the other station!<br><font color=""red"">If you are sure that you aren't logged on please tell us via <a href=""mailto:[n:0]"">[n:1]</a>!</font>"
                    ErrorLoggedOutBecauseLoggedOnAtAnotherMachine = "Your session has been terminated because you've logged on at another station.<br>"
                    ErrorLogonFailedTooOften = "Logon process has failed too often, account has been disabled.<br>Please try again later!"
                    ErrorEmptyPassword = "Please don't forget to input a password!<br>If you don't know your password any more try to gain it via e-mail. Please take a look at the bottom of this document for further details."
                    ErrorRequiredField = "Required field"
                    ErrorUnknown = "Unexpected error! - Please contact the <a href=""mailto:support@camm.biz"">Trouble Center</a>!"
                    ErrorEmptyField = "Please input values into all fields marked with an asterisk <em>(*)</em>!"
                    ErrorWrongNetwork = "You are not allowed to connect via your current network connection."
                    ErrorUserAlreadyExists = "A login with this name already exists. Please choose another login name!"
                    ErrorLoginCreatedSuccessfully = "The login account has been created successfully!"
                    ErrorSendPWWrongLoginOrEmailAddress = "Wrong login or e-mail address.<br>Please input the correct values to start the process sending your password."
                    ErrorCookiesMustNotBeDisabled = "Your browser doesn't support cookies or cookies are disabled because of your browser's security policies."
                    ErrorTimoutOrLoginFromAnotherStation = "Session timed out or login from another station."
                    ErrorApplicationConfigurationIsEmpty = "This application hasn't been configured yet. Please contact the manufacturer of this application."
                    InfoUserLoggedOutSuccessfully = "You've been logged out successfully. We thank you for your attendance."
                    UserManagementEMailColumnTitleLogin = "Login name: "
                    UserManagementEMailColumnTitleCompany = "Company: "
                    UserManagementEMailColumnTitleName = "Name: "
                    UserManagementEMailColumnTitleEMailAddress = "e-mail: "
                    UserManagementEMailColumnTitleStreet = "Street: "
                    UserManagementEMailColumnTitleZIPCode = "ZIP code: "
                    UserManagementEMailColumnTitleLocation = "Location: "
                    UserManagementEMailColumnTitleState = "State: "
                    UserManagementEMailColumnTitleCountry = "Country: "
                    UserManagementEMailColumnTitle1stLanguage = "1st preferred language: "
                    UserManagementEMailColumnTitle2ndLanguage = "2nd preferred language: "
                    UserManagementEMailColumnTitle3rdLanguage = "3rd preferred language: "
                    UserManagementEMailColumnTitleComesFrom = "Comes From: "
                    UserManagementEMailColumnTitleMotivation = "Motivation: "
                    UserManagementEMailColumnTitleCustomerNo = "CustomerNo: "
                    UserManagementEMailColumnTitleSupplierNo = "SupplierNo: "
                    UserManagementEMailColumnTitleComment = "Comment: "
                    UserManagementAddressesMr = "Mr. "
                    UserManagementAddressesMs = "Ms. "
                    UserManagementEMailTextRegards = "Sincerely"
                    UserManagementEMailTextSubject = "Your login name"
                    UserManagementEMailTextSubject4AdminNewUser = "Secured Area - New User"
                    UserManagementMasterServerAvailableInNearFuture = "Attention: This Server will be available only in the near future."
                    CreateAccount_MsgEMailWelcome = "Welcome to Secured Area! The place to go on every day!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your login name is: [n:0]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Get the advantages of extensive support! There is a lot of information waiting for you in our Extranet. Please feel free to surf through this site and explore it all." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your account provides access to various secured programs. By now, you should have completed the association process of Secured Area. Your full authorizations will be applicable in the next 3 - 4 days. Then please revisit the URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    CreateAccount_MsgEMailWelcome_WithPassword = "Welcome to Secured Area! The place to go on every day!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your login name is: [n:0]    " & ChrW(13) & ChrW(10) & _
                "Your password is: [n:1]    " & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Please don't forget to amend the password yourself as soon as possible to ensure nobody else has got it and can use it!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "Get the advantages of extensive support! There is a lot of information waiting for you in our Extranet. Please feel free to surf through this site and explore it all." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your account provides access to various secured programs. By now, you should have completed the association process of Secured Area. Your full authorizations will be applicable in the next 3 - 4 days. Then please revisit the URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:2]"
                    CreateAccount_MsgEMail4Admin = "The following new user has joined to the Secured Area." & ChrW(13) & ChrW(10) & _
                "Please assign related authorizations!" & ChrW(13) & ChrW(10) & _
                "To adjust authorizations please visit " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_MsgEMail4Admin = "The following new user has been joined to the Secured Area by you or one of your collegues." & ChrW(13) & ChrW(10) & _
                "Please assign related authorizations!" & ChrW(13) & ChrW(10) & _
                "To adjust authorizations please visit " & OfficialServerGroup_AdminURL & User_Auth_Config_Files_Administration_DefaultPageInAdminEMails & " !"
                    UserManagement_NewUser_TextAuthCheckSuccessfull = "Welcome to our Secured Area! The place to go on every day!" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your authorizations for user ""[n:0]"" have been created. Please feel free to visit us at:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "We are looking forward to serving you in cyberspace."
                    UserManagement_NewUser_SubjectAuthCheckSuccessfull = "Welcome to our Secured Area!"
                    SendPassword_EMailMessage = "Below is your new membership information. Please keep this confirmation mail as a record of your username and password. Consider this information confidential and treat accordingly." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your Secured Area password is: [n:0]" & ChrW(13) & ChrW(10) & ChrW(13) & ChrW(10) & _
                "Please don't forget to amend the password yourself as soon as possible to ensure nobody else has got it and can use it!" & ChrW(13) & ChrW(10) & _
         ChrW(13) & ChrW(10) & _
                "An account provides access to various secured programmes. By now, you should have completed the association process of our Secured Area. Then please revisit the URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    UserManagement_ResetPWByAdmin_EMailMsg = "The administrator has reset your account. Below is your new membership information. Please keep this confirmation mail as a record of your username and password. Consider this information confidential and treat accordingly." & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "Your Secured Area password is: [n:0]    " & ChrW(13) & ChrW(10) & _
ChrW(13) & ChrW(10) & _
                "Please don't forget to amend the password yourself as soon as possible to ensure nobody else has got it and can use it!" & ChrW(13) & ChrW(10) & _
             ChrW(13) & ChrW(10) & _
                "An account provides access to various secured programs. By now, you should have completed the association process of our Secured Area. Then please revisit the URL:" & ChrW(13) & ChrW(10) & _
                ChrW(13) & ChrW(10) & _
                "[n:1]"
                    HighlightTextIntro = "Get the advantages of extensive support!"
                    HighlightTextTechnicalSupport = "We are just starting and things are changing every day."
                    HighlightTextExtro = "There is a lot of information waiting for you in our Extranet. Please feel free to surf through this site and explore it all!"
                    WelcomeTextWelcomeMessage = "Welcome to our Secured Area!"
                    WelcomeTextFeedbackToContact = "Do you need additional features? Please don't hesitate to send comments on the Extranet to <a href=""mailto:[n:0]"">[n:1]</a>!"
                    WelcomeTextIntro = "The place to go on every day!"
                    UserManagementEMailTextDearUndefinedGender = "Dear "
                    UserManagementSalutationUnformalUndefinedGender = "Hello "
                    NavAreaNameLogin = "Login"
                    NavLinkNamePasswordRecovery = "Password forgotten?"
                    NavLinkNameNewUser = "Create new user account"
                    CreateAccount_Descr_MotivItemSupplier = "Supplier "
                    UpdateProfile_Descr_MotivItemSupplier = "Supplier "

            End Select

            RaiseEvent LanguageDataLoaded(IDLanguage)

            CurrentlyLoadedLanguagesStrings = IDLanguage

        End Sub

        Public Overridable Function GetAlternativelySupportedLanguageID(marketID As Integer) As Integer
            Dim Result As Integer

            Select Case marketID
                Case 552
                    Result = 553
                Case 528, 4002
                    Result = 411
                Case 527, 4003
                    Result = 359
                Case 526
                    Result = 357
                Case 508, 4001, 10020, 25009, 25010, 25015
                    Result = 345
                Case 314, 315
                    Result = 313
                Case 505
                    Result = 200
                Case 495, 496, 497, 498, 499
                    Result = 80
                Case 479, 480, 481, 482, 483, 484, 485, 486, 487, 488, 489, 490, 491, 492, 493, 494, 10003, 10009
                    Result = 21
                Case 530, 531, 532, 533, 534, 535, 536, 537, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550, 560, 10001, 10015, 10016, 10019, 10021, 25005, 25006, 25013, 25016
                    Result = 4
                Case 500, 501, 502, 503, 504, 559, 10023, 25007, 25008, 25012, 25014
                    Result = 3
                Case 510, 511, 512, 513, 558, 25003, 25004
                    Result = 2
                Case 478, 514, 515, 516, 517, 518, 519, 521, 522, 523, 524, 525, 4004, 10000, 10002, 10004, 10005, 10006, 10007, 10008, 10010, 10011, 10012, 10013, 10014, 10017, 10018, 10022, 25001, 25002, 25011
                    Result = 1
                Case Else
                    Result = marketID 'The language ID is the market ID
            End Select

            Return Result

        End Function

        Public Overridable Function GetCurLongDate(MyLanguage As Integer) As String
            Dim LongDate As String
            Dim WeekdayDescr As String = Nothing
            Dim MonthDescr As String = Nothing

            Select Case MyLanguage
                Case 359
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Понедельник"
                        Case 2 : WeekdayDescr = "Вторник"
                        Case 3 : WeekdayDescr = "Среда"
                        Case 4 : WeekdayDescr = "Четверг"
                        Case 5 : WeekdayDescr = "Пятница"
                        Case 6 : WeekdayDescr = "Суббота"
                        Case 7 : WeekdayDescr = "Воскресенье"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "Январь"
                        Case 2 : MonthDescr = "Февраль"
                        Case 3 : MonthDescr = "Март"
                        Case 4 : MonthDescr = "Апрель"
                        Case 5 : MonthDescr = "Май"
                        Case 6 : MonthDescr = "Июнь"
                        Case 7 : MonthDescr = "Июль"
                        Case 8 : MonthDescr = "Август"
                        Case 9 : MonthDescr = "Сентябрь"
                        Case 10 : MonthDescr = "Октябрь"
                        Case 11 : MonthDescr = "Ноябрь"
                        Case 12 : MonthDescr = "Декабрь"
                    End Select
                    LongDate = WeekdayDescr & ", " & Day(Now) & ". " & MonthDescr & " " & Year(Now)
                Case 345
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Segunda-feira"
                        Case 2 : WeekdayDescr = "Terça-feira"
                        Case 3 : WeekdayDescr = "Quarta-feira"
                        Case 4 : WeekdayDescr = "Quinta-feira"
                        Case 5 : WeekdayDescr = "Sexta-feira"
                        Case 6 : WeekdayDescr = "Sábado"
                        Case 7 : WeekdayDescr = "Domingo"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "Janeiro"
                        Case 2 : MonthDescr = "Fevereiro"
                        Case 3 : MonthDescr = "Março"
                        Case 4 : MonthDescr = "Avril"
                        Case 5 : MonthDescr = "Mayo"
                        Case 6 : MonthDescr = "Junho"
                        Case 7 : MonthDescr = "Julho"
                        Case 8 : MonthDescr = "Agosto"
                        Case 9 : MonthDescr = "Setembro"
                        Case 10 : MonthDescr = "Outubro"
                        Case 11 : MonthDescr = "Novembro"
                        Case 12 : MonthDescr = "Dizembro"
                    End Select
                    LongDate = WeekdayDescr & ", " & Now.Day & " de " & MonthDescr & " de " & Now.Year
                Case 343
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Poniedziałek"
                        Case 2 : WeekdayDescr = "Wtorek"
                        Case 3 : WeekdayDescr = "Środa"
                        Case 4 : WeekdayDescr = "Czwartek"
                        Case 5 : WeekdayDescr = "Piątek"
                        Case 6 : WeekdayDescr = "Sobota"
                        Case 7 : WeekdayDescr = "Niedziela"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "stycznia"
                        Case 2 : MonthDescr = "lutego"
                        Case 3 : MonthDescr = "marca"
                        Case 4 : MonthDescr = "kwietnia"
                        Case 5 : MonthDescr = "maja"
                        Case 6 : MonthDescr = "czerwca"
                        Case 7 : MonthDescr = "lipca"
                        Case 8 : MonthDescr = "sierpnia"
                        Case 9 : MonthDescr = "września"
                        Case 10 : MonthDescr = "października"
                        Case 11 : MonthDescr = "listopada"
                        Case 12 : MonthDescr = "grudnia"
                    End Select
                    LongDate = WeekdayDescr & ", " & Day(Now) & " " & MonthDescr & " " & Year(Now)
                Case 202
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "月曜日"
                        Case 2 : WeekdayDescr = "火曜日"
                        Case 3 : WeekdayDescr = "水曜日"
                        Case 4 : WeekdayDescr = "木曜日"
                        Case 5 : WeekdayDescr = "金曜日"
                        Case 6 : WeekdayDescr = "土曜日"
                        Case 7 : WeekdayDescr = "日曜日"
                    End Select
                    'No month descriptions available in Japanese, so use just the number here
                    LongDate = Year(Now) & "年" & Month(Now) & "月" & Day(Now) & "日, " & WeekdayDescr
                Case 200
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Lunedì"
                        Case 2 : WeekdayDescr = "Martedì"
                        Case 3 : WeekdayDescr = "Mercoledì"
                        Case 4 : WeekdayDescr = "Giovedì"
                        Case 5 : WeekdayDescr = "Venerdì"
                        Case 6 : WeekdayDescr = "Sabato"
                        Case 7 : WeekdayDescr = "Domenica"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "Gennaio"
                        Case 2 : MonthDescr = "Febbraio"
                        Case 3 : MonthDescr = "Marzo"
                        Case 4 : MonthDescr = "Aprile"
                        Case 5 : MonthDescr = "Maggio"
                        Case 6 : MonthDescr = "Giugno"
                        Case 7 : MonthDescr = "Luglio"
                        Case 8 : MonthDescr = "Agosto"
                        Case 9 : MonthDescr = "Settembre"
                        Case 10 : MonthDescr = "Ottobre"
                        Case 11 : MonthDescr = "Novembre"
                        Case 12 : MonthDescr = "Dicembre"
                    End Select
                    LongDate = WeekdayDescr & ", " & Day(Now) & ". " & MonthDescr & " " & Year(Now)
                Case 180
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "hétfô"
                        Case 2 : WeekdayDescr = "kedd"
                        Case 3 : WeekdayDescr = "szerda"
                        Case 4 : WeekdayDescr = "csütörtök"
                        Case 5 : WeekdayDescr = "péntek"
                        Case 6 : WeekdayDescr = "szombat"
                        Case 7 : WeekdayDescr = "vasárnap"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "január"
                        Case 2 : MonthDescr = "február"
                        Case 3 : MonthDescr = "március"
                        Case 4 : MonthDescr = "április"
                        Case 5 : MonthDescr = "május"
                        Case 6 : MonthDescr = "június"
                        Case 7 : MonthDescr = "július"
                        Case 8 : MonthDescr = "augusztus"
                        Case 9 : MonthDescr = "szeptember"
                        Case 10 : MonthDescr = "október"
                        Case 11 : MonthDescr = "vovember"
                        Case 12 : MonthDescr = "december"
                    End Select
                    LongDate = WeekdayDescr & ", " & Day(Now) & ". " & MonthDescr & " " & Year(Now)
                Case 80
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "星期一"
                        Case 2 : WeekdayDescr = "星期二"
                        Case 3 : WeekdayDescr = "星期三"
                        Case 4 : WeekdayDescr = "星期四"
                        Case 5 : WeekdayDescr = "星期五"
                        Case 6 : WeekdayDescr = "星期六"
                        Case 7 : WeekdayDescr = "星期天"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "1月"
                        Case 2 : MonthDescr = "2月"
                        Case 3 : MonthDescr = "3月"
                        Case 4 : MonthDescr = "4月"
                        Case 5 : MonthDescr = "5月"
                        Case 6 : MonthDescr = "6月"
                        Case 7 : MonthDescr = "7月"
                        Case 8 : MonthDescr = "8月"
                        Case 9 : MonthDescr = "9月"
                        Case 10 : MonthDescr = "10月"
                        Case 11 : MonthDescr = "11月"
                        Case 12 : MonthDescr = "12月"
                    End Select
                    LongDate = Year(Now) & "年" & MonthDescr & Day(Now) & "日，" & WeekdayDescr
                Case 4
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Lunes"
                        Case 2 : WeekdayDescr = "Martes"
                        Case 3 : WeekdayDescr = "Miércoles"
                        Case 4 : WeekdayDescr = "Jueves"
                        Case 5 : WeekdayDescr = "Viernes"
                        Case 6 : WeekdayDescr = "Sábado"
                        Case 7 : WeekdayDescr = "Domingo"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "Enero"
                        Case 2 : MonthDescr = "Febrero"
                        Case 3 : MonthDescr = "Marzo"
                        Case 4 : MonthDescr = "Abril"
                        Case 5 : MonthDescr = "Mayo"
                        Case 6 : MonthDescr = "Junio"
                        Case 7 : MonthDescr = "Julio"
                        Case 8 : MonthDescr = "Agosto"
                        Case 9 : MonthDescr = "Septiembre"
                        Case 10 : MonthDescr = "Octubre"
                        Case 11 : MonthDescr = "Noviembre"
                        Case 12 : MonthDescr = "Diciembre"
                    End Select
                    LongDate = WeekdayDescr & ", " & Now.Day & " de " & MonthDescr & " de " & Now.Year
                Case 3
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Lundi"
                        Case 2 : WeekdayDescr = "Mardi"
                        Case 3 : WeekdayDescr = "Mercredi"
                        Case 4 : WeekdayDescr = "Jeudi"
                        Case 5 : WeekdayDescr = "Vendredi"
                        Case 6 : WeekdayDescr = "Samedi"
                        Case 7 : WeekdayDescr = "Dimanche"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "Janvier"
                        Case 2 : MonthDescr = "Février"
                        Case 3 : MonthDescr = "Mars"
                        Case 4 : MonthDescr = "Avril"
                        Case 5 : MonthDescr = "Mai"
                        Case 6 : MonthDescr = "Juin"
                        Case 7 : MonthDescr = "Juillet"
                        Case 8 : MonthDescr = "Août"
                        Case 9 : MonthDescr = "Septembre"
                        Case 10 : MonthDescr = "Octobre"
                        Case 11 : MonthDescr = "Novembre"
                        Case 12 : MonthDescr = "Décembre"
                    End Select
                    LongDate = WeekdayDescr & ", " & Day(Now) & " " & MonthDescr & " " & Year(Now)
                Case 2
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Montag"
                        Case 2 : WeekdayDescr = "Dienstag"
                        Case 3 : WeekdayDescr = "Mittwoch"
                        Case 4 : WeekdayDescr = "Donnerstag"
                        Case 5 : WeekdayDescr = "Freitag"
                        Case 6 : WeekdayDescr = "Samstag"
                        Case 7 : WeekdayDescr = "Sonntag"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "Januar"
                        Case 2 : MonthDescr = "Februar"
                        Case 3 : MonthDescr = "März"
                        Case 4 : MonthDescr = "April"
                        Case 5 : MonthDescr = "Mai"
                        Case 6 : MonthDescr = "Juni"
                        Case 7 : MonthDescr = "Juli"
                        Case 8 : MonthDescr = "August"
                        Case 9 : MonthDescr = "September"
                        Case 10 : MonthDescr = "Oktober"
                        Case 11 : MonthDescr = "November"
                        Case 12 : MonthDescr = "Dezember"
                    End Select
                    LongDate = WeekdayDescr & ", " & Day(Now) & ". " & MonthDescr & " " & Year(Now)
                Case Else
                    Select Case Weekday(Now, vbMonday)
                        Case 1 : WeekdayDescr = "Monday"
                        Case 2 : WeekdayDescr = "Tuesday"
                        Case 3 : WeekdayDescr = "Wednesday"
                        Case 4 : WeekdayDescr = "Thursday"
                        Case 5 : WeekdayDescr = "Friday"
                        Case 6 : WeekdayDescr = "Saturday"
                        Case 7 : WeekdayDescr = "Sunday"
                    End Select
                    Select Case Month(Now)
                        Case 1 : MonthDescr = "January"
                        Case 2 : MonthDescr = "February"
                        Case 3 : MonthDescr = "March"
                        Case 4 : MonthDescr = "April"
                        Case 5 : MonthDescr = "May"
                        Case 6 : MonthDescr = "June"
                        Case 7 : MonthDescr = "July"
                        Case 8 : MonthDescr = "August"
                        Case 9 : MonthDescr = "September"
                        Case 10 : MonthDescr = "October"
                        Case 11 : MonthDescr = "November"
                        Case 12 : MonthDescr = "December"
                    End Select
                    LongDate = WeekdayDescr & " " & MonthDescr & " " & Day(Now) & ", " & Year(Now)
            End Select
            GetCurLongDate = LongDate

        End Function

        Public Overridable Function IsSupportedLanguage(ByVal LanguageID As Integer) As Boolean

            'Returns True if the language is INTERNALLY supported
            'The alternative languages don't play a role, here!

            Select Case LanguageID
                Case 359
                    Return True
                Case 345
                    Return True
                Case 343
                    Return True
                Case 202
                    Return True
                Case 200
                    Return True
                Case 180
                    Return True
                Case 80
                    Return True
                Case 4
                    Return True
                Case 3
                    Return True
                Case 2
                    Return True
                Case 1
                    Return True
                Case Else
                    Return False
            End Select
        End Function

    End Class
End NameSpace