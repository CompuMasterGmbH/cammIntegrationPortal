'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Modules.Feedback.Pages

    ''' <summary>
    '''     Collect eedback from the users
    ''' </summary>
    Friend Class NamespaceDoc
    End Class

    ''' <summary>
    '''     A feedback form always requires some logic to collect user inputs and to send them via e-mail to a defined address.
    ''' </summary>
    ''' <remarks>
    '''     <para>Only server control data (web controls, html controls) will be collected, but no normal HTML input tags which are not running at server.</para>
    '''     <para>The controls can be placed by you yourself. If they contain user input data, it will be added to the collected data list.</para>
    '''     <para>You only should ensure that you also create a Button or LinkButton which calls the method <see cref="CompuMaster.camm.WebManager.Modules.Feedback.Pages.FeedbackForm.CollectDataAndSendEMail">CollectDataAndSendEMail</see> to collect and send the data.</para>
    ''' </remarks>
    <System.Runtime.InteropServices.ComVisible(False)> Public Class FeedbackForm
        Inherits CompuMaster.camm.WebManager.Pages.Page
        ''' <summary>
        '''     Loop through a control collection and collect all information a user has made
        ''' </summary>
        ''' <param name="page">A page instance with some controls to be parsed/collected</param>
        ''' <param name="results">The reference to the results hashtable</param>
        Public Sub CollectAllInputControls(ByVal page As System.Web.UI.Page, ByVal results As Hashtable)
            CollectAllInputControls(page.Controls, results, Nothing)
        End Sub
        ''' <summary>
        '''     Loop through a control collection and collect all information a user has made
        ''' </summary>
        ''' <param name="control">A single control to be parsed/collected</param>
        ''' <param name="results">The reference to the results hashtable</param>
        ''' <param name="prefix">An optional prefix for the control name in the results hashtable</param>
        Private Sub CollectAllInputControls(ByVal control As UI.Control, ByVal results As Hashtable, ByVal prefix As String)
            CollectAllInputControls(control.Controls, results, prefix)
        End Sub
        ''' <summary>
        '''     Loop through a control collection and collect all information a user has made
        ''' </summary>
        ''' <param name="controls">A single control to be parsed/collected</param>
        ''' <param name="results">The reference to the results hashtable</param>
        ''' <param name="prefix">An optional prefix for the control name in the results hashtable</param>
        Private Sub CollectAllInputControls(ByVal controls As UI.ControlCollection, ByVal results As Hashtable, ByVal prefix As String)

            For Each MyControl As UI.Control In controls

                'Generic Web Controls/HTML Controls for input values
                If MyControl.GetType Is GetType(UI.WebControls.CheckBox) Then
                    AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.CheckBox).Checked, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.RadioButton) Then
                    AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.RadioButton).Checked, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.CheckBoxList) Then
                    Dim ListValues As String = Nothing
                    For MyCounter As Integer = 0 To CType(MyControl, UI.WebControls.CheckBoxList).Items.Count - 1
                        If CType(MyControl, UI.WebControls.CheckBoxList).Items(MyCounter).Selected Then
                            If ListValues = Nothing Then
                                ListValues = CType(MyControl, UI.WebControls.CheckBoxList).Items(MyCounter).Value
                            Else
                                ListValues &= "; " & CType(MyControl, UI.WebControls.CheckBoxList).Items(MyCounter).Value
                            End If
                        End If
                    Next
                    AddResult(results, MyControl.ID, ListValues, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.RadioButtonList) Then
                    AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.RadioButtonList).SelectedValue, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.TextBox) Then
                    AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.TextBox).Text, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.ListBox) Then
                    If CType(MyControl, UI.WebControls.ListBox).SelectionMode = UI.WebControls.ListSelectionMode.Single Then
                        AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.ListBox).SelectedValue, prefix)
                    Else 'Multiple
                        Dim ListValues As String = Nothing
                        For MyCounter As Integer = 0 To CType(MyControl, UI.WebControls.ListBox).Items.Count - 1
                            If CType(MyControl, UI.WebControls.ListBox).Items(MyCounter).Selected Then
                                If ListValues = Nothing Then
                                    ListValues = CType(MyControl, UI.WebControls.ListBox).Items(MyCounter).Value
                                Else
                                    ListValues &= "; " & CType(MyControl, UI.WebControls.ListBox).Items(MyCounter).Value
                                End If
                            End If
                        Next
                        AddResult(results, MyControl.ID, ListValues, prefix)
                    End If
                ElseIf MyControl.GetType Is GetType(UI.WebControls.DropDownList) Then
                    AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.DropDownList).SelectedValue, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.Calendar) Then
                    AddResult(results, MyControl.ID, CType(MyControl, UI.WebControls.Calendar).SelectedDate, prefix)
                ElseIf MyControl.GetType Is GetType(UI.WebControls.Button) Then
                    'Do nothing
                ElseIf MyControl.GetType Is GetType(UI.WebControls.LinkButton) Then
                    'Do nothing
                ElseIf MyControl.GetType Is GetType(UI.WebControls.Label) Then
                    'Do nothing
                ElseIf MyControl.GetType Is GetType(UI.LiteralControl) Then
                    'Do nothing
                Else
                    'Parse sub controls
                    CollectAllInputControls(MyControl, results, prefix)
                End If

            Next
        End Sub
        ''' <summary>
        '''     Add values to the results collection
        ''' </summary>
        ''' <param name="results">The results hashtable</param>
        ''' <param name="controlID">The control name to identify the value</param>
        ''' <param name="value">The value</param>
        ''' <param name="keyPrefix">An optional prefix for the control name, regulary used for controls in controls</param>
        Private Sub AddResult(ByVal results As Hashtable, ByVal controlID As String, ByVal value As Object, ByVal keyPrefix As String)
            If controlID = Nothing Then
                Throw New ArgumentNullException(controlID)
            Else
                If keyPrefix = Nothing Then
                    results.Add(controlID, value)
                Else
                    results.Add(keyPrefix & "_" & controlID, value)
                End If
            End If
        End Sub
        ''' <summary>
        '''     The subject of the form in the e-mail
        ''' </summary>
        ''' <value></value>
        Public Property Subject() As String
            Get
                If CType(ViewState("Subject"), String) = Nothing Then
                    Return "Feedback from " & Me.Request.Url.ToString
                Else
                    Return CType(ViewState("Subject"), String)
                End If
            End Get
            Set(ByVal Value As String)
                ViewState("Subject") = Value
            End Set
        End Property
        ''' <summary>
        '''     The introduction of your e-mail
        ''' </summary>
        ''' <value></value>
        Public Property MessageIntroText() As String
            Get
                Return CType(ViewState("MessageIntroText"), String)
            End Get
            Set(ByVal Value As String)
                ViewState("MessageIntroText") = Value
            End Set
        End Property
        ''' <summary>
        '''     The finish of your e-mail
        ''' </summary>
        ''' <value></value>
        Public Property MessageFinishText() As String
            Get
                Return CType(ViewState("MessageFinishText"), String)
            End Get
            Set(ByVal Value As String)
                ViewState("MessageFinishText") = Value
            End Set
        End Property
        ''' <summary>
        '''     Collect all data and process the results
        ''' </summary>
        ''' <param name="emailAddress">The address where the form data shall be sent to</param>
        ''' <returns>An error description if an exception occured</returns>
        ''' <remarks>
        '''     The list of the form fields will be sorted alphabetically, that's why it's recommended to name all controls IDs in a way (e. g. "00_FirstName", "01_FamilyName", etc.) which allows you to get a well sorted list of form data.
        ''' </remarks>
        Public Function CollectDataAndSendEMail(ByVal emailAddress As String) As String
            Return CollectDataAndSendEMail(emailAddress, Nothing)
        End Function
        ''' <summary>
        '''     Collect all data and process the results
        ''' </summary>
        ''' <param name="emailAddress">The address where the form data shall be sent to</param>
        ''' <param name="inputFormPanelOrContainer">The control where to start the search for input fields (Nothing to search on the whole Page)</param>
        ''' <returns>An error description if an exception occured or null (Nothing in VisualBasic) if it was successful</returns>
        ''' <remarks>
        '''     The list of the form fields will be sorted alphabetically, that's why it's recommended to name all controls IDs in a way (e. g. "00_FirstName", "01_FamilyName", etc.) which allows you to get a well sorted list of form data.
        ''' </remarks>
        Private Function CollectDataAndSendEMail(ByVal emailAddress As String, ByVal inputFormPanelOrContainer As UI.Control) As String

            Dim Results As New Hashtable
            If inputFormPanelOrContainer Is Nothing Then
                CollectAllInputControls(Me.Page, Results, Nothing)
            Else
                CollectAllInputControls(inputFormPanelOrContainer, Results, Nothing)
            End If

            Dim ResultsSorted As New SortedList(Results)
            Dim TextData As String = Nothing, HtmlData As String = Nothing

            'Prepare the form data elements
            HtmlData = "<table border=""0"">" & vbNewLine
            For Each MyKey As Object In ResultsSorted.Keys
                TextData &= System.Web.HttpUtility.HtmlEncode(MyKey.ToString.PadLeft(30)) & ": " & System.Web.HttpUtility.HtmlEncode(CType(Results(MyKey), String)) & vbNewLine
                HtmlData &= "<tr><th align=""left"" valign=""top"">" & System.Web.HttpUtility.HtmlEncode(MyKey.ToString) & ":&nbsp;&nbsp;</th><td valign=""top"">" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(System.Web.HttpUtility.HtmlEncode(CType(Results(MyKey), String))) & "<td></tr>" & vbNewLine
            Next
            HtmlData &= "</table>" & vbNewLine

            'Append intro/extro text elements
            If MessageIntroText <> Nothing Then
                TextData = MessageIntroText & vbNewLine & vbNewLine & TextData
                HtmlData = "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(MessageIntroText) & "</p>" & vbNewLine & HtmlData & vbNewLine
            End If
            If MessageFinishText <> Nothing Then
                TextData = TextData & vbNewLine & vbNewLine & MessageFinishText
                HtmlData = HtmlData & "<p>" & CompuMaster.camm.WebManager.Utils.HTMLEncodeLineBreaks(MessageFinishText) & "</p>" & vbNewLine
            End If
            HtmlData = "<font face=""Arial"">" & HtmlData & "</font>" 'use Arial font by default

            'Send the e-mail
            Dim bufErrors As String = ""
            If Me.cammWebManager.MessagingEMails.SendEMail(emailAddress, emailAddress, Subject, TextData, HtmlData, cammWebManager.StandardEMailAccountName, cammWebManager.StandardEMailAccountAddress, CType(Nothing, Messaging.EMailAttachment()), Messaging.EMails.Priority.Normal, Messaging.EMails.Sensitivity.Status_Normal, False, False, Nothing, Nothing, bufErrors) Then
                'No errors
                Return Nothing
            Else
                Return bufErrors
            End If

        End Function
        ''' <summary>
        '''     Collect all data and send them to the e-mail address and show/hide the panels of the form dependent on the success of the e-mail transmission
        ''' </summary>
        ''' <param name="emailAddress">The address where the form data shall be sent to</param>
        ''' <param name="formContent">A control or panel which contains the input form and which shall be made invisible after the user has submitted the form data</param>
        ''' <param name="successContent">A control or panel which shall be shown when all things went fine</param>
        ''' <param name="submissionFailureContent">A control or panel which will be shown when an error occured. Don't forget to assign this method's return value to a text label if you want to show the error description to the user.</param>
        ''' <returns>An error description if an exception occured</returns>
        ''' <remarks>
        '''     The list of the form fields will be sorted alphabetically, that's why it's recommended to name all controls IDs in a way (e. g. "00_FirstName", "01_FamilyName", etc.) which allows you to get a well sorted list of form data.
        ''' </remarks>
        Public Function CollectDataAndSendEMail(ByVal emailAddress As String, ByVal formContent As UI.Control, ByVal successContent As UI.Control, ByVal submissionFailureContent As UI.Control) As String

            Dim errors As String = CollectDataAndSendEMail(emailAddress, formContent)

            'Switch visibility of controls
            formContent.Visible = False
            If errors = Nothing Then
                successContent.Visible = True
                submissionFailureContent.Visible = False
            Else
                successContent.Visible = False
                submissionFailureContent.Visible = True
            End If

            Return errors

        End Function

    End Class

End Namespace