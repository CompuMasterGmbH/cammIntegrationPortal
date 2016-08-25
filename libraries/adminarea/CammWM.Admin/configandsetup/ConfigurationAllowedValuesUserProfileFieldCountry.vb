'Copyright 2012-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    Public Class ConfigurationAllowedValuesUserProfileFieldCountry
        Inherits Page

        Protected WithEvents RadioButtonListSetupOfAllowRule As RadioButtonList
        Protected WithEvents RadioButtonListValueSeparator As RadioButtonList
        Protected WithEvents ButtonImportGithubDatasetsCountryCodesByName As Button
        Protected WithEvents ButtonImportGithubDatasetsCountryCodesByISO3166_1_Alpha2 As Button
        Protected WithEvents ButtonSave As Button
        Protected WithEvents CheckboxAllowEmptyValue As CheckBox
        Protected WithEvents TextboxAllowedValues As TextBox
        Protected WithEvents PanelPage As Panel
        Protected LabelInfoMessage As Label
        Protected LabelErrorMessage As Label

        Private _AllowedValues As System.Collections.Generic.List(Of String)
        ''' <summary>
        ''' All allowed values incl. blank value if selected
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property AllowedValues As System.Collections.Generic.List(Of String)
            Get
                If _AllowedValues Is Nothing Then
                    If Me.IsPostBack = False Then
                        'Load current config
                        Dim ConfigSetting As System.Collections.Generic.List(Of String) = WMSystem.UserInformation.CentralConfig_AllowedValues_FieldCountry(Me.cammWebManager)
                        If ConfigSetting Is Nothing Then
                            _AllowedValues = New System.Collections.Generic.List(Of String)
                        Else
                            _AllowedValues = ConfigSetting
                        End If
                    Else
                        'Fill based on current textarea data
                        Dim TextboxValues As New System.Collections.Generic.List(Of String)(Me.TextboxAllowedValues.Text.Split(ControlChars.Cr, ControlChars.Lf, "|"c))
                        TextboxValues.Sort()
                        _AllowedValues = New System.Collections.Generic.List(Of String)
                        For MyCounter As Integer = 0 To TextboxValues.Count - 1
                            If MyCounter = 0 Then
                                _AllowedValues.Add(TextboxValues(MyCounter))
                            ElseIf TextboxValues(MyCounter - 1) <> TextboxValues(MyCounter) Then 'not equal to previous item
                                _AllowedValues.Add(TextboxValues(MyCounter))
                            End If
                        Next
                        'Cleanup empty entries
                        For MyCounter As Integer = _AllowedValues.Count - 1 To 0 Step -1
                            If Trim(_AllowedValues(MyCounter)) = "" Then
                                _AllowedValues.RemoveAt(MyCounter)
                            End If
                        Next
                        If Me.CheckboxAllowEmptyValue.Checked Then
                            _AllowedValues.Insert(0, "")
                        End If
                    End If
                    _AllowedValues.Sort()
                End If
                Return _AllowedValues
            End Get
        End Property

        Private _IsBlankValueAllowedInitialized As Boolean = False
        Public Property IsBlankValueAllowed As Boolean
            Get
                Dim Result As Boolean
                If _IsBlankValueAllowedInitialized = False And Me.IsPostBack = False Then
                    If AllowedValues.Contains("") Then
                        Result = True
                    Else
                        Result = False
                    End If
                    Me.IsBlankValueAllowed = Result
                    Return Result
                Else
                    'Fill based on current textarea data
                    Return Me.CheckboxAllowEmptyValue.Checked
                End If
            End Get
            Set(value As Boolean)
                Me.CheckboxAllowEmptyValue.Checked = value
                _IsBlankValueAllowedInitialized = True
            End Set
        End Property

        Private Sub ButtonImportGithubDatasetsCountryCodesByISO3166_1_Alpha2_Click(sender As Object, e As EventArgs) Handles ButtonImportGithubDatasetsCountryCodesByISO3166_1_Alpha2.Click
            Try
                Dim CsvOriginUrl As String = "https://raw.githubusercontent.com/datasets/country-codes/master/data/country-codes.csv"
                Dim CountryCodes As DataTable = CompuMaster.camm.WebManager.Administration.Tools.Data.Csv.ReadDataTableFromCsvFile(CsvOriginUrl, True, System.Text.Encoding.UTF8, System.Globalization.CultureInfo.InvariantCulture, """"c, False, False)
                For MyCounter As Integer = 0 To CountryCodes.Rows.Count - 1
                    Me.AllowedValues.Add(Utils.Nz(CountryCodes.Rows(MyCounter)("ISO3166-1-Alpha-2"), ""))
                Next
                Me.AllowedValues.Sort()
            Catch ex As Exception
                Me.LabelErrorMessage.Text = Server.HtmlEncode(ex.Message)
            End Try
        End Sub


        Private Sub ButtonImportGithubDatasetsCountryCodesByName_Click(sender As Object, e As EventArgs) Handles ButtonImportGithubDatasetsCountryCodesByName.Click
            Try
                Dim CsvOriginUrl As String = "https://raw.githubusercontent.com/datasets/country-codes/master/data/country-codes.csv"
                Dim CountryCodes As DataTable = CompuMaster.camm.WebManager.Administration.Tools.Data.Csv.ReadDataTableFromCsvFile(CsvOriginUrl, True, System.Text.Encoding.UTF8, System.Globalization.CultureInfo.InvariantCulture, """"c, False, False)
                For MyCounter As Integer = 0 To CountryCodes.Rows.Count - 1
                    Me.AllowedValues.Add(Utils.Nz(CountryCodes.Rows(MyCounter)("name"), ""))
                Next
                Me.AllowedValues.Sort()
            Catch ex As Exception
                Me.LabelErrorMessage.Text = Server.HtmlEncode(ex.Message)
            End Try
        End Sub

        Private Sub ConfigurationAllowedValuesUserProfileFieldCountry_Load(sender As Object, e As EventArgs) Handles Me.Load
            Me.Form.DefaultButton = Me.ButtonSave.UniqueID
            Me.LabelInfoMessage.EnableViewState = False
            Me.LabelErrorMessage.EnableViewState = False

            If Me.IsPostBack = False Then
                'Load current config
                If Me.AllowedValues.Count > 0 OrElse Me.IsBlankValueAllowed Then
                    Me.RadioButtonListSetupOfAllowRule.SelectedValue = "1"
                Else
                    Me.RadioButtonListSetupOfAllowRule.SelectedValue = "0"
                End If
                Dim DummyForBlankCheckboxToBeCheckedIfBlankValueAllowed As Boolean = Me.IsBlankValueAllowed
            End If
            RadioButtonListSetupOfAllowRule_SelectedIndexChanged(Nothing, Nothing)

        End Sub

        Private Sub RadioButtonListSetupOfAllowRule_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RadioButtonListSetupOfAllowRule.SelectedIndexChanged
            If Me.RadioButtonListSetupOfAllowRule.SelectedValue = "1" Then
                Me.PanelPage.Visible = True
            Else
                Me.PanelPage.Visible = False
            End If
        End Sub

        Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
            Try
                If Me.RadioButtonListSetupOfAllowRule.SelectedValue = "1" Then
                    'Check for max length of country field, current max = 60 chars
                    For MyCounter As Integer = 0 To Me.AllowedValues.Count - 1
                        If Me.AllowedValues(MyCounter).Length > 60 Then
                            Throw New Exception("Country field is limited to max. 60 chars, following value requires " & Me.AllowedValues(MyCounter).Length & " chars: " & Me.AllowedValues(MyCounter))
                        End If
                    Next
                    'Check for existing data collissions
                    Dim CurrentlyUsedValuesInUserInfosButNotAllowed As New System.Collections.Generic.List(Of String)
                    Dim CurrentlyUsedValuesInUserInfos As System.Collections.Generic.List(Of String) = Me.LoadCurrentlyUsedValuesAsInUserInfos
                    For MyCounter As Integer = 0 To CurrentlyUsedValuesInUserInfos.Count - 1
                        If Me.AllowedValues.Contains(CurrentlyUsedValuesInUserInfos(MyCounter)) = False Then
                            CurrentlyUsedValuesInUserInfosButNotAllowed.Add(CurrentlyUsedValuesInUserInfos(MyCounter))
                        End If
                    Next
                    If CurrentlyUsedValuesInUserInfosButNotAllowed.Count > 0 Then
                        Throw New Exception("User profiles currently use these values which wouldn't be allowed: """ & Strings.Join(CurrentlyUsedValuesInUserInfosButNotAllowed.ToArray, """, """) & """")
                        Else
                        WMSystem.UserInformation.CentralConfig_AllowedValues_FieldCountrySetup(Me.cammWebManager, Me.AllowedValues)
                    End If
                Else
                    WMSystem.UserInformation.CentralConfig_AllowedValues_FieldCountrySetup(Me.cammWebManager, New System.Collections.Generic.List(Of String))
                End If
                Me.LabelInfoMessage.Text = Server.HtmlEncode("Configuration saved successfully.")
            Catch ex As Exception
                Me.LabelErrorMessage.Text = Server.HtmlEncode(ex.Message)
            End Try
        End Sub

        Private Sub ConfigurationAllowedValuesUserProfileFieldCountry_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
            Dim TextboxContent As New System.Text.StringBuilder
            Dim Values As System.Collections.Generic.List(Of String) = Me.AllowedValues
            Dim ValueSeparator As String
            If Me.RadioButtonListValueSeparator.SelectedValue = "1" Then
                ValueSeparator = "|"
            Else
                ValueSeparator = vbNewLine
            End If

            For MyCounter As Integer = 0 To Values.Count - 1
                If Values(MyCounter) <> "" Then
                    TextboxContent.Append(Values(MyCounter))
                    If MyCounter <> Values.Count - 1 Then TextboxContent.Append(ValueSeparator)
                End If
            Next
            Me.TextboxAllowedValues.Text = TextboxContent.ToString
        End Sub

        Private Function LoadCurrentlyUsedValuesAsInUserInfos() As System.Collections.Generic.List(Of String)
            Dim Sql As String = "SELECT IsNull(Land, N'') FROM dbo.Benutzer GROUP BY IsNull(Land, N'')"
            Dim MyCmd As New SqlClient.SqlCommand(Sql, New SqlClient.SqlConnection(Me.cammWebManager.ConnectionString))
            Return CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.ExecuteReaderAndPutFirstColumnIntoGenericList(Of String)(MyCmd, WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider.Automations.AutoOpenAndCloseAndDisposeConnection)
        End Function

    End Class

End Namespace
