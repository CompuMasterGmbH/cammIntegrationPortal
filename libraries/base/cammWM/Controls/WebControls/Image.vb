'Copyright 2002-2016 CompuMaster GmbH, http://www.compumaster.de and/or its affiliates. All rights reserved.
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

Namespace CompuMaster.camm.WebManager.Controls.WebControls

    <System.Runtime.InteropServices.ComVisible(False)> Public Class Image
        Inherits System.Web.UI.WebControls.Image
        Implements IControl

        Private _webmanager As cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        Public Property cammWebManager() As cammWebManager Implements IControl.cammWebManager
            Get
                If Not _webmanager Is Nothing Then 'Save a few checks in following code block
                    Return _webmanager
                End If
                If _webmanager Is Nothing AndAlso Not Me.Page Is Nothing AndAlso GetType(Pages.Page).IsInstanceOfType(Me.Page) AndAlso Not CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation Is Nothing Then
                    _webmanager = CType(Me.Page, Pages.Page).cammWebManagerWithMasterPageLookupButWithoutJitCreation
                End If
                If _webmanager Is Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _webmanager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Page, "cammWebManager", Nothing), cammWebManager)
                        If _webmanager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                Return _webmanager
            End Get
            Set(ByVal Value As cammWebManager)
                _webmanager = Value
            End Set
        End Property

        Private ReadOnly Property Control() As System.Web.UI.Control Implements IControl.Control
            Get
                Return CType(Me, System.Web.UI.Control)
            End Get
        End Property

        ''' <summary>
        '''     Search for the server form in the list of parent controls
        ''' </summary>
        ''' <returns>The control of the server form if it's existant</returns>
        Public Function LookupParentServerForm() As System.Web.UI.HtmlControls.HtmlForm
            Static Result As System.Web.UI.HtmlControls.HtmlForm
            If Result Is Nothing Then
                Result = Utils.LookupParentServerForm(Me)
            End If
            Return Result
        End Function

    End Class

End Namespace