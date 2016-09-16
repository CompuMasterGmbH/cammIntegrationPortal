'Copyright 2005-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
'---------------------------------------------------------------
'This file is part of camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
'camm Integration Portal (camm Web-Manager) is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Affero General Public License for more details.
'You should have received a copy of the GNU Affero General Public License along with camm Integration Portal (camm Web-Manager). If not, see <http://www.gnu.org/licenses/>.
'Alternatively, the camm Integration Portal (or camm Web-Manager) can be licensed for closed-source / commercial projects from CompuMaster GmbH, <http://www.camm.biz/>.
'
'Diese Datei ist Teil von camm Integration Portal (camm Web-Manager).
'camm Integration Portal (camm Web-Manager) ist Freie Software: Sie k�nnen es unter den Bedingungen der GNU Affero General Public License, wie von der Free Software Foundation, Version 3 der Lizenz oder (nach Ihrer Wahl) jeder sp�teren ver�ffentlichten Version, weiterverbreiten und/oder modifizieren.
'camm Integration Portal (camm Web-Manager) wird in der Hoffnung, dass es n�tzlich sein wird, aber OHNE JEDE GEW�HRLEISTUNG, bereitgestellt; sogar ohne die implizite Gew�hrleistung der MARKTF�HIGKEIT oder EIGNUNG F�R EINEN BESTIMMTEN ZWECK. Siehe die GNU Affero General Public License f�r weitere Details.
'Sie sollten eine Kopie der GNU Affero General Public License zusammen mit diesem Programm erhalten haben. Wenn nicht, siehe <http://www.gnu.org/licenses/>.
'Alternativ kann camm Integration Portal (oder camm Web-Manager) lizenziert werden f�r Closed-Source / kommerzielle Projekte von  CompuMaster GmbH, <http://www.camm.biz/>.

Option Strict On
Option Explicit On

Imports System.Reflection
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

Namespace CompuMaster.camm.WebManager.Controls.Administration

    Public Class UserControl
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        ''' <summary>
        ''' Safely lookup the name of a group
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Protected Function SafeLookupGroupName(id As Integer) As String
            Try
                If id = -1 Then
                    Return "{Anonymous}"
                Else
                    Return New CompuMaster.camm.WebManager.WMSystem.GroupInformation(id, CType(Me.cammWebManager, WMSystem)).Name
                End If
            Catch ex As Exception
                Return "[?:" & id & "] (" & ex.Message & ")"
            End Try
        End Function

        ''' <summary>
        ''' Safely lookup the name of a user
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Protected Function SafeLookupUserFullName(id As Long) As String
            Return Me.SafeLookupUserFullName(id, False)
        End Function

        ''' <summary>
        ''' Safely lookup the name of a user
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="additionallyWithLoginName">True to enable additional output of login name, e.g. &quot;User Full Name (Login name)&quot;</param>
        ''' <returns></returns>
        Protected Function SafeLookupUserFullName(id As Long, additionallyWithLoginName As Boolean) As String
            Return CompuMaster.camm.WebManager.Administration.Utils.FormatUserNameSafely(CType(Me.cammWebManager, WMSystem), id, additionallyWithLoginName)
        End Function

    End Class

    ''' <summary>
    '''     The floating menu on the right bottom which allows navigation to top of the current page or a history go back
    ''' </summary>
    Public Class FloatingMenu
        Inherits UserControl

        Public Sub New()
            Me.AnchorText = "Overview"
        End Sub

        Public Property AnchorText() As String
            Get
                Return Utils.Nz(ViewState("AnchorText"), String.Empty)
            End Get
            Set(ByVal Value As String)
                ViewState("AnchorText") = Value
            End Set
        End Property

        Public Property AnchorTitle() As String
            Get
                Return Utils.Nz(ViewState("AnchorTitle"), String.Empty)
            End Get
            Set(ByVal Value As String)
                ViewState("AnchorTitle") = Value
            End Set
        End Property

        Public Property HRef() As String
            Get
                Return Utils.Nz(ViewState("HRef"), String.Empty)
            End Get
            Set(ByVal Value As String)
                ViewState("HRef") = Value
            End Set
        End Property

        Private Literal As Literal
        Private Sub ControlInit(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
            Literal = New Literal
            Me.Controls.Add(Literal)
        End Sub

        Private Sub ControlPreRender(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.PreRender
            Dim Script As New System.Text.StringBuilder
            If HRef = Nothing Then
                Script.Append("<div id=""RePos"" style=""float: left; width: 80px; height: 60px; line-height: 60px; position: fixed; right: 0px; bottom: 20px; z-index: 10000; background-color: #ffffff; border: solid 2px #E1E1E1; font-family: Arial; font-size: 8pt; text-align: center;"">")
                Script.Append("<a href=""#top"">Go to top</a>")
                Script.Append("</div>")
            Else
                Script.Append("<div id=""RePos"" style=""float: left; width: 80px; height: 60px; line-height: 30px; position: fixed; right: 0px; bottom: 20px; z-index: 10000; background-color: #ffffff; border: solid 2px #E1E1E1; font-family: Arial; font-size: 8pt; text-align: center;"">")
                Script.Append("<a href=""#top"">Go to top</a>")
                Script.Append("<br>")
                Script.Append("<a href=""" & HRef & """")
                If Me.AnchorTitle <> Nothing Then Script.Append(" title=""" & Server.HtmlEncode(Me.AnchorTitle) & """")
                Script.Append(">")
                Script.Append(Server.HtmlEncode(Me.AnchorText))
                Script.Append("</a>")
                Script.Append("</div>")
            End If
            Literal.Text = Script.ToString
        End Sub

    End Class

    Public Class AdministrativeDelegates
        Inherits UserControl

        Public adjustdelegates, adjustdelegatesSecurity As HtmlAnchor
        Public GroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation
        Public SecurityObjectInfo As CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation

        Public Function CurrentAdminIsPrivilegedForItemAdministration(itemType As CompuMaster.camm.WebManager.Pages.Administration.Page.AdministrationItemType, authorizationType As CompuMaster.camm.WebManager.Pages.Administration.Page.AuthorizationTypeEffective, itemID As Integer) As Boolean
            Return CType(Me.Page, CompuMaster.camm.WebManager.Pages.Administration.Page).CurrentAdminIsPrivilegedForItemAdministration(itemType, authorizationType, itemID)
        End Function
    End Class

    Public Class GroupsAdditionalInformation
        Inherits UserControl

        Public MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation
    End Class

    Public Class UsersAdditionalInformation
        Inherits UserControl

        Public MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation

        Public Function SortAuthorizations(ByVal left As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation, ByVal right As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation) As Integer
            Dim DisplayNameLeft As String, DisplayNameRight As String
            Try
                'following statement might fail on invalid references to security objects (e.g. security object deleted in table, but authorization entry never deleted for some reasons)
                DisplayNameLeft = left.SecurityObjectInfo.DisplayName
            Catch
                DisplayNameLeft = "{ERROR: Invalid ID " & left.SecurityObjectID & "}"
            End Try
            Try
                'following statement might fail on invalid references to security objects (e.g. security object deleted in table, but authorization entry never deleted for some reasons)
                DisplayNameRight = right.SecurityObjectInfo.DisplayName
            Catch
                DisplayNameRight = "{ERROR: Invalid ID " & right.SecurityObjectID & "}"
            End Try
            Return DisplayNameLeft.CompareTo(DisplayNameRight)
        End Function

        Public Function SortMemberships(ByVal left As CompuMaster.camm.WebManager.WMSystem.GroupInformation, ByVal right As CompuMaster.camm.WebManager.WMSystem.GroupInformation) As Integer
            Return left.Description.CompareTo(right.Description)
        End Function





    End Class

    Public Class AssemblyInfo
        '''<summary>
        '''Returns assembly (title and version) information
        '''</summary>
        Public Function GetAssemblyInfo() As String
            Dim curAsm As System.Reflection.Assembly
            Dim attrTitle As AssemblyTitleAttribute
            'get the currently executing assembly instance
            curAsm = System.Reflection.Assembly.GetExecutingAssembly()
            'get the custom attributes in the assembly
            Dim attrs() As Object
            Dim result As String = ""

            attrs = curAsm.GetCustomAttributes(GetType(AssemblyTitleAttribute), False)
            If attrs.Length > 0 Then
                attrTitle = CType(attrs(0), Reflection.AssemblyTitleAttribute)
                result = attrTitle.Title.ToString
                If curAsm.GetName.Version.ToString.Trim <> "" Then result += " v." & curAsm.GetName.Version.ToString.Trim & ""
            End If

            Return result
        End Function
    End Class

End Namespace
