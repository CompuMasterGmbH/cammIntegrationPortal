'Copyright 2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Imports System.Web.UI

Namespace CompuMaster.camm.WebManager.Controls.WebControls

    ''' <summary>
    ''' Show content based on existing or missing user authorizations
    ''' </summary>
#If NetFramework <> "1_1" Then
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:ConditionalContent ID=""ConditionalContent"" runat=""server"" SecurityObject="""" NotSecurityObject=""""></{0}:WebManager>"), System.Web.UI.Themeable(False)> Public Class ConditionalContent
#Else
    <System.Runtime.InteropServices.ComVisible(False), ToolboxData("<{0}:ConditionalContent ID=""ConditionalContent"" runat=""server"" SecurityObject="""" NotSecurityObject=""""></{0}:WebManager>")> Public Class ConditionalContent
#End If

        Inherits Controls.Control

        Public Sub New()
            'Me.Visible = False 'Default to invisible if nothing else has been specified by code or tag declaration
        End Sub

        Private _SecurityObject As String
        ''' <summary>
        ''' A security object for the positive default rule to switch the content of this control to visible mode if the current user has got access to the specified security object
        ''' </summary>
        Public Property SecurityObject As String
            Get
                Return _SecurityObject
            End Get
            Set(value As String)
                _SecurityObject = value
            End Set
        End Property

        Private _NotSecurityObject As String
        ''' <summary>
        ''' A security object for the negative default rule to switch the content of this control to visible mode if the current user has NOT got access to the specified security object
        ''' </summary>
        Public Property NotSecurityObject As String
            Get
                Return _NotSecurityObject
            End Get
            Set(value As String)
                _NotSecurityObject = value
            End Set
        End Property

        Private _AlwaysVisibleSecurityObject As String
        ''' <summary>
        ''' A security object which is allowed to see the content regardless of the default rules (e.g. always visible for website editors)
        ''' </summary>
        Public Property AlwaysVisibleSecurityObject As String
            Get
                Return _AlwaysVisibleSecurityObject
            End Get
            Set(value As String)
                _AlwaysVisibleSecurityObject = value
            End Set
        End Property

        Private Sub CheckForVisibility()
            TraceWriteToPage("CheckForVisibility Begin")
            If Me.cammWebManager Is Nothing Then
                Throw New NullReferenceException("ConditionalContent control hasn't got a reference to a camm Web-Manager instance, but it is required")
            ElseIf Trim(Me.SecurityObject) = Nothing AndAlso Trim(Me.NotSecurityObject) = Nothing Then
                'both check values empty --> not allowed situation --> critical exception
                Throw New NullReferenceException("SecurityObject or NotSecurityObject property must be not empty")
            Else
                'either positive or negative rule must match - or both in case parameters for both rules are present
                Dim ShowContentByPermissionRule As Boolean = False
                If Trim(Me.SecurityObject) = Nothing OrElse cammWebManager.IsUserAuthorized(Me.SecurityObject) Then
                    ShowContentByPermissionRule = True
                    TraceWriteToPage("ShowContentByPermissionRule(" & Me.SecurityObject & ")=" & ShowContentByPermissionRule)
                End If
                Dim ShowContentByMissingPermissionRule As Boolean = False
                If Trim(Me.NotSecurityObject) = Nothing OrElse cammWebManager.IsUserAuthorized(Me.NotSecurityObject) = False Then
                    ShowContentByMissingPermissionRule = True
                    TraceWriteToPage("ShowContentByMissingPermissionRule(" & Me.NotSecurityObject & ")=" & ShowContentByMissingPermissionRule)
                End If
                Dim ShowContentByAlwaysPermissionRule As Boolean = False
                If Trim(Me.AlwaysVisibleSecurityObject) <> Nothing AndAlso cammWebManager.IsUserAuthorized(Me.AlwaysVisibleSecurityObject) = False Then
                    ShowContentByAlwaysPermissionRule = True
                    TraceWriteToPage("ShowContentByAlwaysPermissionRule(" & Me.AlwaysVisibleSecurityObject & ")=" & ShowContentByAlwaysPermissionRule)
                End If
                Me.Visible = ShowContentByAlwaysPermissionRule OrElse (ShowContentByPermissionRule And ShowContentByMissingPermissionRule)
                TraceWriteToPage("Visible=" & Me.Visible)
            End If
        End Sub

        Private Sub TraceWriteToPage(message As String)
            Me.TraceWriteToPageWithCategory(message, "ConditionalContent:" & Me.UniqueID)
        End Sub
        Private Sub TraceWriteToPageWithCategory(message As String, category As String)
            If Me.cammWebManager.DebugLevel >= WMSystem.DebugLevels.Medium_LoggingOfDebugInformation Then
                Me.Page.Response.Write("<li>" & category & " | " & message & "</li>")
            End If
            If Me.Page.Trace.IsEnabled Then
                Trace.Write(message, category)
            End If
        End Sub

        Protected Overrides Sub OnPreRender(e As EventArgs)
            TraceWriteToPage("OnPreRender Begin @ Visible=" & Me.Visible.ToString)
            MyBase.OnPreRender(e)
            TraceWriteToPage("OnPreRender visibility check start")
            Me.CheckForVisibility()
            TraceWriteToPage("OnPreRender visibility check completed")
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            TraceWriteToPage("OnLoad Begin")
            TraceWriteToPage("Tracing enabled: " & Me.Page.Trace.IsEnabled.ToString)
            TraceWriteToPage("OnLoad begin")
            Trace.Write("OnLoad begin", "ConditionalContent:" & Me.ID)
            MyBase.OnLoad(e)
            'TraceWriteToPage("OnLoad visibility check start")
            'Trace.Write("OnLoad visibility check start", "ConditionalContent:" & Me.ID)
            'Me.CheckForVisibility(Nothing, e)
            'TraceWriteToPage("OnLoad visibility check completed")
            'Trace.Write("OnLoad visibility check completed", "ConditionalContent:" & Me.ID)
        End Sub

    End Class

End Namespace