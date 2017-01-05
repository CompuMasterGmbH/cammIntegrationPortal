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

Option Strict On
Option Explicit On

Imports System.Web.UI.WebControls

Namespace CompuMaster.camm.WebManager.Pages.Administration

    ''' <summary>
    '''     Activation of markets
    ''' </summary>
    Public Class MarketActivations
        Inherits Page

        Protected Markets As DataGrid
        ''' <summary>
        '''     The market which shall be changed
        ''' </summary>
        ''' <value></value>
        Protected ReadOnly Property Market() As Integer
            Get
                If Request.QueryString("Market") <> "" Then
                    Return CType(Request.QueryString("Market"), Integer)
                Else
                    Return Nothing
                End If
            End Get
        End Property
        ''' <summary>
        '''     Shall the market be activated?
        ''' </summary>
        ''' <value></value>
        Protected ReadOnly Property MarketActivated() As Boolean
            Get
                If Request.QueryString("MarketActivated") <> "" Then
                    Return CType(Request.QueryString("MarketActivated"), Boolean)
                Else
                    Throw New ArgumentNullException("MarketActivated")
                End If
            End Get
        End Property

        Private Sub MarketActivations_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'Execute update if requested
            If Market <> Nothing Then
                cammWebManager.System_SetLanguageState(Market, MarketActivated)
            End If

            'Load data and fill the datagrid
            Dim AllMarkets As CompuMaster.camm.WebManager.WMSystem.LanguageInformation()
            AllMarkets = cammWebManager.System_GetLanguagesInfo(Nothing, True, False)

            Dim MarketsTable As New DataTable("Markets")
            MarketsTable.Columns.Add("ID", GetType(Integer))
            MarketsTable.Columns.Add("Name", GetType(String))
            MarketsTable.Columns.Add("Activated", GetType(Boolean))

            For MyCounter As Integer = 0 To AllMarkets.Length - 1
                Dim MyRow As DataRow = MarketsTable.NewRow
                MyRow(0) = AllMarkets(MyCounter).ID
                MyRow(1) = AllMarkets(MyCounter).LanguageName_English
                MyRow(2) = AllMarkets(MyCounter).IsActive
                MarketsTable.Rows.Add(MyRow)
            Next

            Markets.AutoGenerateColumns = False
            Markets.DataSource = MarketsTable
            Markets.Columns(0).HeaderText = "ID"
            Markets.Columns(1).HeaderText = "Name"
            Markets.Columns(2).HeaderText = "Activated"
            Markets.DataBind()
        End Sub

    End Class

End Namespace