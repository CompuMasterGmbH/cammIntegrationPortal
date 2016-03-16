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

Imports System.Web
Imports System.Data
Imports System.Reflection
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls
Imports CompuMaster.camm.WebManager.Administration.Tools.Data.DataQuery.AnyIDataProvider

Namespace CompuMaster.camm.WebManager.Controls.Administration
    ''' <summary>
    '''     The floating menu on the right bottom which allows navigation to top of the current page or a history go back
    ''' </summary>
    ''' <remarks>
    ''' </remarks> 
    Public Class FloatingMenu
        Inherits System.Web.UI.UserControl

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

        Private Sub ControlLoad(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            Dim Script As New System.Text.StringBuilder
            If HRef = Nothing Then
                Script.Append("<div id=""RePos"" style=""position:absolute; visibility:show; left:0px; top:-200px; z-index:2""><table width=""60"" bgcolor=""#FFFFFF"" border=""2"" bordercolor=""#E1E1E1"" cellspacing=""0"" cellpadding=""2""><tr><td><font face=""Arial"" size=""1""><a href=""#top"">Go to top</a></font></td></tr></table></div>")
            Else
                Script.Append("<div id=""RePos"" style=""position:absolute; visibility:show; left:0px; top:-200px; z-index:2""><table width=""60"" bgcolor=""#FFFFFF"" border=""2"" bordercolor=""#E1E1E1"" cellspacing=""0"" cellpadding=""2""><tr><td><font face=""Arial"" size=""1""><a href=""#top"">Go to top</a><br><A href=""" & HRef & """>Overview</A></font></td></tr></table></div>")
            End If
            Script.Append(vbNewLine)
            Script.Append("<script language=""JavaScript"">")
            Script.Append(vbNewLine)
            Script.Append("<!--")
            Script.Append(vbNewLine)
            Script.Append("movex = 0;")
            Script.Append(vbNewLine)
            Script.Append("movey = 0;")
            Script.Append(vbNewLine)
            Script.Append("xdiff = 0;")
            Script.Append(vbNewLine)
            Script.Append("ydiff = 0;")
            Script.Append(vbNewLine)
            Script.Append("ystart = 0;")
            Script.Append(vbNewLine)
            Script.Append("xstart = 0;")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("function setVariables()")
            Script.Append(vbNewLine)
            Script.Append("{")
            Script.Append(vbNewLine)
            Script.Append("    if (navigator.appName == ""Netscape"")")
            Script.Append(vbNewLine)
            Script.Append("    {")
            Script.Append(vbNewLine)
            Script.Append("        v = "".top="";")
            Script.Append(vbNewLine)
            Script.Append("        h = "".left="";")
            Script.Append(vbNewLine)
            Script.Append("        dS = ""document."";")
            Script.Append(vbNewLine)
            Script.Append("        sD = """";")
            Script.Append(vbNewLine)
            Script.Append("        y = ""window.pageYOffset"";")
            Script.Append(vbNewLine)
            Script.Append("        x = ""window.pageXOffset"";")
            Script.Append(vbNewLine)
            Script.Append("    }")
            Script.Append(vbNewLine)
            Script.Append("    else")
            Script.Append(vbNewLine)
            Script.Append("    {")
            Script.Append(vbNewLine)
            Script.Append("        h = "".pixelLeft="";")
            Script.Append(vbNewLine)
            Script.Append("        v = "".pixelTop="";")
            Script.Append(vbNewLine)
            Script.Append("        dS = """";")
            Script.Append(vbNewLine)
            Script.Append("        sD = "".style"";")
            Script.Append(vbNewLine)
            Script.Append("        y = ""document.body.scrollTop"";")
            Script.Append(vbNewLine)
            Script.Append("        x = ""document.body.scrollLeft"";")
            Script.Append(vbNewLine)
            Script.Append("    }")
            Script.Append(vbNewLine)
            Script.Append("    xyz = 500;")
            Script.Append(vbNewLine)
            Script.Append("    object = ""RePos"";")
            Script.Append(vbNewLine)
            Script.Append("    checkLocationA();")
            Script.Append(vbNewLine)
            Script.Append("}")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("function checkLocation()")
            Script.Append(vbNewLine)
            Script.Append("{")
            Script.Append(vbNewLine)
            Script.Append("    if (navigator.appName == ""Netscape"")")
            Script.Append(vbNewLine)
            Script.Append("    {")
            Script.Append(vbNewLine)
            Script.Append("        iW = ""window.innerWidth - 15 "";")
            Script.Append(vbNewLine)
            Script.Append("        iH = ""window.innerHeight"";")
            Script.Append(vbNewLine)
            Script.Append("    }")
            Script.Append(vbNewLine)
            Script.Append("    else")
            Script.Append(vbNewLine)
            Script.Append("    {")
            Script.Append(vbNewLine)
            Script.Append("        iW = ""document.body.clientWidth"";")
            Script.Append(vbNewLine)
            Script.Append("        iH = ""document.body.clientHeight"";")
            Script.Append(vbNewLine)
            Script.Append("    }")
            Script.Append(vbNewLine)
            Script.Append("    innerX = eval(iW) - 80;")
            Script.Append(vbNewLine)
            Script.Append("    innerY = eval(iH) - 60;")
            Script.Append(vbNewLine)
            Script.Append("    yy = eval(y);")
            Script.Append(vbNewLine)
            Script.Append("    xx = eval(x);")
            Script.Append(vbNewLine)
            Script.Append("    ydiff = ystart - yy;")
            Script.Append(vbNewLine)
            Script.Append("    xdiff = xstart - xx;")
            Script.Append(vbNewLine)
            Script.Append("    if ((ydiff < (-1)) || (ydiff > (1))) movey = Math.round(ydiff / 10), ystart -= movey;")
            Script.Append(vbNewLine)
            Script.Append("    if ((xdiff < (-1)) || (xdiff > (1))) movex = Math.round(xdiff / 10), xstart -= movex;")
            Script.Append(vbNewLine)
            Script.Append("    eval(dS + object + sD + v + (ystart + innerY));")
            Script.Append(vbNewLine)
            Script.Append("    eval(dS + object + sD + h + (xstart + innerX));")
            Script.Append(vbNewLine)
            Script.Append("    setTimeout(""checkLocation()"", 10);")
            Script.Append(vbNewLine)
            Script.Append("}")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("function checkLocationA()")
            Script.Append(vbNewLine)
            Script.Append("{")
            Script.Append(vbNewLine)
            Script.Append("    ystart = eval(y);")
            Script.Append(vbNewLine)
            Script.Append("    xstart=eval(x);")
            Script.Append(vbNewLine)
            Script.Append("}")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("function switchLogo(abc)")
            Script.Append(vbNewLine)
            Script.Append("{")
            Script.Append(vbNewLine)
            Script.Append("    if (abc == ""menu"")")
            Script.Append(vbNewLine)
            Script.Append("    {")
            Script.Append(vbNewLine)
            Script.Append("        eval(dS + object + sD + v + 0);")
            Script.Append(vbNewLine)
            Script.Append("        eval(dS + object + sD + h + (-200));")
            Script.Append(vbNewLine)
            Script.Append("        object = abc;")
            Script.Append(vbNewLine)
            Script.Append("    }")
            Script.Append(vbNewLine)
            Script.Append("    else")
            Script.Append(vbNewLine)
            Script.Append("    {")
            Script.Append(vbNewLine)
            Script.Append("        xyz = setTimeout(""delayLogo()"", 2000)")
            Script.Append(vbNewLine)
            Script.Append("    }")
            Script.Append(vbNewLine)
            Script.Append("}")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("function delayLogo()")
            Script.Append(vbNewLine)
            Script.Append("{")
            Script.Append(vbNewLine)
            Script.Append("    eval(dS + object + sD + v + 0);")
            Script.Append(vbNewLine)
            Script.Append("    eval(dS + object + sD + h + (-200));")
            Script.Append(vbNewLine)
            Script.Append("    object = 'logo';")
            Script.Append(vbNewLine)
            Script.Append("}")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("setVariables();")
            Script.Append(vbNewLine)
            Script.Append("checkLocation();")
            Script.Append(vbNewLine)
            Script.Append("")
            Script.Append(vbNewLine)
            Script.Append("//-->")
            Script.Append(vbNewLine)
            Script.Append("</script>")
            Script.Append(vbNewLine)
            Literal.Text = Script.ToString
        End Sub

    End Class

    Public Class AdministrativeDelegates
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        Public adjustdelegates, adjustdelegatesSecurity As HtmlAnchor
        Public GroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation
        Public SecurityObjectInfo As CompuMaster.camm.WebManager.WMSystem.SecurityObjectInformation

        Public Function CurrentAdminIsPrivilegedForItemAdministration(itemType As CompuMaster.camm.WebManager.Pages.Administration.Page.AdministrationItemType, authorizationType As CompuMaster.camm.WebManager.Pages.Administration.Page.AuthorizationTypeEffective, itemID As Integer) As Boolean
            Return CType(Me.Page, CompuMaster.camm.WebManager.Pages.Administration.Page).CurrentAdminIsPrivilegedForItemAdministration(itemType, authorizationType, itemID)
        End Function
    End Class

    Public Class GroupsAdditionalInformation
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        Public MyGroupInfo As CompuMaster.camm.WebManager.WMSystem.GroupInformation
    End Class

    Public Class UsersAdditionalInformation
        Inherits CompuMaster.camm.WebManager.Controls.UserControl

        Public MyUserInfo As CompuMaster.camm.WebManager.WMSystem.UserInformation

        Public Function SortAuthorizations(ByVal left As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation, ByVal right As CompuMaster.camm.WebManager.WMSystem.Authorizations.UserAuthorizationInformation) As Integer
            Return left.SecurityObjectInfo.DisplayName.CompareTo(right.SecurityObjectInfo.DisplayName)
        End Function

        Public Function SortMemberships(ByVal left As CompuMaster.camm.WebManager.WMSystem.GroupInformation, ByVal right As CompuMaster.camm.WebManager.WMSystem.GroupInformation) As Integer
            Return left.Description.CompareTo(right.Description)
        End Function





    End Class

    Public Class AsseblyInfo
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
