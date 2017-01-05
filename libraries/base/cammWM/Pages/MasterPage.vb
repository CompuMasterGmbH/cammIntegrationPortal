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

Namespace CompuMaster.camm.WebManager.Pages

#If NetFramework <> "1_1" Then
    Public Class MasterPage
        Inherits System.Web.UI.MasterPage

        Private _WebManager As CompuMaster.camm.WebManager.Controls.cammWebManager
        Private AlreadyTryedToLookUpCammWebManager As Boolean
        ''' <summary>
        ''' The cammWebManager instance created by a cammWebManager control on this master page or one of its parent master pages
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Public Property cammWebManager As CompuMaster.camm.WebManager.Controls.cammWebManager
            Get
                If Not _WebManager Is Nothing Then 'Save a few checks in following code block
                    Return _WebManager
                End If
                'Look in parent master pages
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso GetType(MasterPage).IsInstanceOfType(Me.Master) AndAlso CType(Me.Master, MasterPage).cammWebManager IsNot Nothing Then
                    _WebManager = CType(CType(Me.Master, MasterPage).cammWebManager, CompuMaster.camm.WebManager.Controls.cammWebManager)
                End If
                'Look in parent master pages with shadowed cammWebManager property (VS2010 designer automatically inserts protected property cammWebManager even if already existing by inheriting from this master page
                If _WebManager Is Nothing AndAlso Me.Master IsNot Nothing AndAlso AlreadyTryedToLookUpCammWebManager = False Then
                    Try
                        _WebManager = CType(CompuMaster.camm.WebManager.Controls.cammWebManager.GetField(Me.Master, "cammWebManager", Nothing), Controls.cammWebManager)
                        If _WebManager Is Nothing Then
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (MasterPage)", "Auto lookup of camm WebManager failed: no CWM instance found")
                        Else
                            HttpContext.Current.Trace.Write("camm WebManager BaseControl (MasterPage)", "Auto lookup of camm WebManager successfull")
                        End If
                    Catch ex As Exception
                        HttpContext.Current.Trace.Warn("camm WebManager BaseControl (MasterPage)", "Auto lookup of camm WebManager failed: " & ex.Message & ex.StackTrace)
                    Finally
                        AlreadyTryedToLookUpCammWebManager = True
                    End Try
                End If
                'Look in main page - but without JIT creation there and without lookup again in master page
                If _WebManager Is Nothing AndAlso Me.Page IsNot Nothing AndAlso GetType(Page).IsInstanceOfType(Me.Page) AndAlso CType(Me.Page, Page)._WebManager IsNot Nothing Then
                    _WebManager = CType(Me.Page, Page)._WebManager
                End If
                Return _WebManager
            End Get
            Set(value As CompuMaster.camm.WebManager.Controls.cammWebManager)
                _WebManager = value
            End Set
        End Property

    End Class
#End If

End Namespace