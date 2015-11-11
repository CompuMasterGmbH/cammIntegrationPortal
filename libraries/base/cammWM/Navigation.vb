Option Explicit On 
Option Strict On

Namespace CompuMaster.camm.WebManager.Navigation

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Provides logic and functionality to support Cyberakt's AspNetMenu control
    ''' </summary>
    ''' <remarks>
    '''     AspNetMenu is a control of Cyberakt and requires separate licencing
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	26.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <CLSCompliantAttribute(False)> Public Class ASPNetMenu

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fills a menu
        ''' </summary>
        ''' <param name="NavigationData">The navigation items which shall appear in the navigation</param>
        ''' <param name="ASPNetMenuControl">The control where to add navigation items</param>
        ''' <param name="MenuGroup">The sub group where the navigation elements shall be added to. Leave empty if you want to use the top level group</param>
        ''' <remarks>
        '''     AspNetMenu is a control of Cyberakt and requires separate licencing
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal NavigationData As DataTable, ByVal ASPNetMenuControl As CYBERAKT.WebControls.Navigation.ASPnetMenu, Optional ByVal MenuGroup As CYBERAKT.WebControls.Navigation.MenuGroup = Nothing)

            If MenuGroup Is Nothing Then
                MenuGroup = ASPNetMenuControl.TopGroup
            End If

            'Optimizing output
            SetupDefaults(ASPNetMenuControl)

            'Fill required intermediate levels
            VerifyDataAndFillMissingElements(NavigationData)

            'Start filling in root level
            FillSubElements(NavigationData, "\", MenuGroup)

            'Add SEO-optimized noscript-tag - in case the control has already been added to a parent control's control collection
            Dim InsertIndexPosition As Integer = Utils.LookupControlIndexAtParentControl(ASPNetMenuControl)
            If InsertIndexPosition > -1 Then
                ASPNetMenuControl.Parent.Controls.AddAt(InsertIndexPosition, New System.Web.UI.LiteralControl(Utils.SeoNavigation(NavigationData)))
            End If

        End Sub

        ''' <summary>
        ''' Add a control in front of the menu control which renders all links in a search engine optimized style
        ''' </summary>
        ''' <param name="ASPNetMenuControl"></param>
        ''' <remarks></remarks>
        Public Sub AddRenderingForSeoOptimization(ByVal ASPNetMenuControl As CYBERAKT.WebControls.Navigation.ASPnetMenu)
            'Add SEO-optimized noscript-tag - in case the control has already been added to a parent control's control collection
            Dim InsertIndexPosition As Integer = Utils.LookupControlIndexAtParentControl(ASPNetMenuControl)
            If InsertIndexPosition > -1 Then
                ASPNetMenuControl.Parent.Controls.AddAt(InsertIndexPosition, New System.Web.UI.LiteralControl(SeoNavigation(ASPNetMenuControl)))
            Else
                Throw New Exception("Menu control must already be assigned to a parent control before calling this method")
            End If
        End Sub

        ''' <summary>
        ''' Render an SEO optimized navigation inside of a noscript tag
        ''' </summary>
        ''' <param name="ASPNetMenuControl">The navigation items</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SeoNavigation(ByVal ASPNetMenuControl As CYBERAKT.WebControls.Navigation.ASPnetMenu) As String
            Return Utils.SeoNavigation(CreateNavTableFromAspNetMenuControl(ASPNetMenuControl))
        End Function

        ''' <summary>
        ''' Create a navigation data table based on the currently provided menu structure
        ''' </summary>
        ''' <param name="menuControl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateNavTableFromAspNetMenuControl(ByVal menuControl As CYBERAKT.WebControls.Navigation.ASPnetMenu) As System.Data.DataTable
            Dim navTable As System.Data.DataTable = Utils.CreateEmptyDataTable()
            CreateNavTableFromAspNetMenuControl(navTable, menuControl.TopGroup)
            Return navTable
        End Function

        ''' <summary>
        ''' Add navigation entries for every menu group level
        ''' </summary>
        ''' <param name="navTable"></param>
        ''' <param name="group"></param>
        ''' <remarks></remarks>
        Private Sub CreateNavTableFromAspNetMenuControl(ByVal navTable As System.Data.DataTable, ByVal group As CYBERAKT.WebControls.Navigation.MenuGroup)
            For MyCounter As Integer = 0 To group.Items.Count - 1
                Dim navRow As System.Data.DataRow = navTable.NewRow
                navRow("Title") = group.Items(MyCounter).Label
                navRow("URLAutoCompleted") = group.Items(MyCounter).URL
                navRow("IsHtmlEncoded") = True
                navRow("Tooltip") = group.Items(MyCounter).Alt
                navTable.Rows.Add(navRow)
                If Not group.Items(MyCounter).SubGroup Is Nothing Then
                    CreateNavTableFromAspNetMenuControl(navTable, group.Items(MyCounter).SubGroup)
                End If
            Next
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the navigation control with all data into the specified menugroup
        ''' </summary>
        ''' <param name="NavigationData">The data</param>
        ''' <param name="BaseLevel">The base level where to start</param>
        ''' <param name="MenuGroup">The menu group, the top group if nothing specified</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FillSubElements(ByVal NavigationData As DataTable, ByVal BaseLevel As String, ByVal MenuGroup As CYBERAKT.WebControls.Navigation.MenuGroup)

            Dim MySubElements As DataTable
            MySubElements = Navigation.SubCategories(NavigationData, BaseLevel)
            Dim MyRows As DataRow() = MySubElements.Select("", "Sort ASC, Title ASC")

            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim CurCategory As String = Navigation.ValidNavigationPath(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Title"), ""))
                If CurCategory <> "" Then 'AndAlso Not Navigation.ValueAlreadyExistsInDataTable(MySubElements, "Title", CurCategory) Then
                    'Retrieve the menu title
                    Dim MyMenuTitle As String = CurCategory
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        MyMenuTitle &= " (X)"
                    End If
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsHtmlEncoded"), False) = False Then
                        MyMenuTitle = System.Web.HttpUtility.HtmlEncode(MyMenuTitle)
                    End If
                    'Create the new menu item
                    Dim MyMenuItem As CYBERAKT.WebControls.Navigation.MenuItem
                    MyMenuItem = CreateNavigationItem(MenuGroup, MyMenuTitle, False)
                    'Fill menu item detail info
                    MyMenuItem.ID = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ID"), CType(Nothing, String))
                    MyMenuItem.URL = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("URLAutoCompleted"), CType(Nothing, String)))
                    MyMenuItem.URLAbsolute = True
                    MyMenuItem.URLTarget = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Target"), CType(Nothing, String)))
                    MyMenuItem.Alt = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Tooltip"), CType(Nothing, String)))
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        If MyMenuItem.Alt = "" Then
                            MyMenuItem.Alt = "Disabled for development purposes"
                        Else
                            MyMenuItem.Alt &= vbNewLine & vbNewLine & "Disabled for development purposes"
                        End If
                    End If
                    MyMenuItem.IsForceHighlighted = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsNew"), False) Or CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsUpdated"), False)
                    MyMenuItem.LeftIcon = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrc"), CType(Nothing, String)))
                    MyMenuItem.LeftIconOver = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcOver"), CType(Nothing, String)))
                    MyMenuItem.LeftIconDown = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcDown"), CType(Nothing, String)))
                    MyMenuItem.LeftIconWidth = CType(CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconWidth"), CType(Nothing, String))), Integer)
                    MyMenuItem.LeftIconHeight = CType(CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconHeight"), CType(Nothing, String))), Integer)
                    MyMenuItem.RightIcon = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrc"), CType(Nothing, String)))
                    MyMenuItem.RightIconOver = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcOver"), CType(Nothing, String)))
                    MyMenuItem.RightIconDown = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcDown"), CType(Nothing, String)))
                    MyMenuItem.RightIconWidth = CType(CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconWidth"), CType(Nothing, String))), Integer)
                    MyMenuItem.RightIconHeight = CType(CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconHeight"), CType(Nothing, String))), Integer)
                    MyMenuItem.NoWrap = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("NoWrap"), False)
                    MyMenuItem.CssClass = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClass"), CType(Nothing, String)))
                    MyMenuItem.CssClassOver = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassOver"), CType(Nothing, String)))
                    MyMenuItem.CssClassDown = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassDown"), CType(Nothing, String)))
                    MyMenuItem.ClientSideOnClick = CompuMaster.camm.WebManager.Utils.StringNotEmptyOrNothing(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ClientSideOnClick"), CType(Nothing, String)))
                End If

                'Recursively search for subelelements
                FillSubElements(NavigationData, CurCategory, MenuGroup)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a navigation item in the menu control
        ''' </summary>
        ''' <param name="NavMenuGroup"></param>
        ''' <param name="TitlePath"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateNavigationItem(ByVal NavMenuGroup As CYBERAKT.WebControls.Navigation.MenuGroup, ByVal TitlePath As String, ByVal IsCurrentlySelectedPath As Boolean) As CYBERAKT.WebControls.Navigation.MenuItem

            Dim newGroup As CYBERAKT.WebControls.Navigation.MenuGroup = NavMenuGroup
            Dim newItem As CYBERAKT.WebControls.Navigation.MenuItem = Nothing
            Dim CurCategory As String
            Dim Result As CYBERAKT.WebControls.Navigation.MenuItem = Nothing
            For Each CurCategory In TitlePath.Split("\"c)
                CurCategory = Trim(CurCategory)
                If CurCategory <> "" Then
                    If Not newItem Is Nothing Then
                        'In previous loops, we've created a new item, but this hasn't got a sub group yet
                        'But since we've got an additional sub category, we have to add a sub group here
                        If newItem.SubGroup Is Nothing Then
                            newGroup = newItem.AddSubGroup
                            ApplyDefaultOverridesForGroups(newItem)
                        Else
                            newGroup = newItem.SubGroup
                        End If
                        'clear newitem because it will be overwritten in the next lines
                        newItem = Nothing
                    End If
                    For Each MyItem As CYBERAKT.WebControls.Navigation.MenuItem In newGroup.Items
                        If MyItem.Label = CurCategory Then
                            'Item already exists, use this
                            newItem = MyItem
                            Exit For
                        End If
                    Next
                    If newItem Is Nothing Then
                        'add new item
                        newItem = newGroup.Items.Add()
                        newItem.Label = CurCategory
                        Result = newItem
                    Else
                        'use existing one
                        Result = newItem
                    End If
                    Result.IsForceHighlighted = IsCurrentlySelectedPath
                End If
            Next
            'Return the last written MenuItem for further modifications by the calling method
            Return Result

        End Function

        Private Sub ApplyDefaultOverridesForGroups(ByVal NavMenuItem As CYBERAKT.WebControls.Navigation.MenuItem)
            ApplyDefaults(NavMenuItem, _DefaultOverridesForGroups)
        End Sub

        Private Sub ApplyItemDefaults(ByVal NavMenuItem As CYBERAKT.WebControls.Navigation.MenuItem)
            ApplyDefaults(NavMenuItem, _DefaultsForNewMenuItems)
        End Sub

        Private Sub ApplyDefaults(ByVal NavMenuItem As CYBERAKT.WebControls.Navigation.MenuItem, ByVal DefaultValues As Collections.Specialized.NameValueCollection)
            For Each MyKey As String In DefaultValues.AllKeys
                Select Case MyKey.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    Case "label"
                        NavMenuItem.Label = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "id"
                        NavMenuItem.ID = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "alt"
                        NavMenuItem.Alt = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "url"
                        NavMenuItem.URL = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "urlabsolute"
                        NavMenuItem.URLAbsolute = CType(_DefaultOverridesForGroups(MyKey), Boolean)
                    Case "urltarget"
                        NavMenuItem.URLTarget = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "width"
                        NavMenuItem.Width = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "height"
                        NavMenuItem.Height = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "nowrap"
                        NavMenuItem.NoWrap = CType(_DefaultOverridesForGroups(MyKey), Boolean)
                    Case "lefticonsrc"
                        NavMenuItem.LeftIcon = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "lefticonsrcdown"
                        NavMenuItem.LeftIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "lefticonheight"
                        NavMenuItem.LeftIconHeight = CType(_DefaultOverridesForGroups(MyKey), Integer)
                    Case "lefticonsrcover"
                        NavMenuItem.LeftIconOver = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "lefticonwidth"
                        NavMenuItem.LeftIconWidth = CType(_DefaultOverridesForGroups(MyKey), Integer)
                    Case "righticonsrc"
                        NavMenuItem.RightIcon = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "righticonsrcdown"
                        NavMenuItem.RightIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "righticonheight"
                        NavMenuItem.RightIconHeight = CType(_DefaultOverridesForGroups(MyKey), Integer)
                    Case "righticonsrcover"
                        NavMenuItem.RightIconOver = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "righticonwidth"
                        NavMenuItem.RightIconWidth = CType(_DefaultOverridesForGroups(MyKey), Integer)
                    Case "cssclass"
                        NavMenuItem.CssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "css classover"
                        NavMenuItem.CssClassOver = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "cssclassdown"
                        NavMenuItem.CssClassDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "clientsideonclick"
                        NavMenuItem.ClientSideOnClick = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "subgroupwidth"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.Width = CType(_DefaultOverridesForGroups(MyKey), String)
                        End If
                    Case "subgroupheight"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.Height = CType(_DefaultOverridesForGroups(MyKey), String)
                        End If
                    Case "subgrouporientation"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.Orientation = CType(_DefaultOverridesForGroups(MyKey), String)
                        End If
                    Case "subgroupitemspacing"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.ItemSpacing = CType(_DefaultOverridesForGroups(MyKey), Integer)
                        End If
                    Case "subgroupcssclass"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.CssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        End If
                    Case "subgroupexpanddirection"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.ExpandDirection = CType(_DefaultOverridesForGroups(MyKey), String)
                        End If
                    Case "subgroupexpandeffect"
                        If Not NavMenuItem.SubGroup Is Nothing Then
                            NavMenuItem.SubGroup.ExpandEffect = CType(_DefaultOverridesForGroups(MyKey), String)
                        End If
                    Case Else
                        Throw New NotImplementedException("Invalid default key """ & MyKey & """")
                End Select
            Next
        End Sub

        Dim _DefaultOverridesForGroups As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultOverridesForGroups() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultOverridesForGroups
            End Get
        End Property

        Dim _DefaultsForNewMenuItems As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultsForNewMenuItems() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultsForNewMenuItems
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Setup some defaults for the current navigation menu
        ''' </summary>
        ''' <param name="ASPNetMenuControl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetupDefaults(ByVal ASPNetMenuControl As CYBERAKT.WebControls.Navigation.ASPnetMenu)
            ASPNetMenuControl.ClientScriptLocation = "/system/nav/aspnetmenu_client/"
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Navigation.ComponentArtMenu
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Provides logic and functionality to support ComponentArt's Web.UI.Menu control
    ''' </summary>
    ''' <remarks>
    '''     Web.UI.Menu is a control of ComponentArt and requires separate licencing
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	26.07.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ComponentArtMenu

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fills a menu
        ''' </summary>
        ''' <param name="NavigationData">The navigation items which shall appear in the navigation</param>
        ''' <param name="MenuControl">The control where to add navigation items</param>
        ''' <param name="MenuGroup">The sub group where the navigation elements shall be added to. Leave empty if you want to use the top level group</param>
        ''' <remarks>
        '''     AspNetMenu is a control of Cyberakt and requires separate licencing
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal NavigationData As DataTable, ByVal MenuControl As ComponentArt.Web.UI.Menu, Optional ByVal MenuGroup As ComponentArt.Web.UI.MenuItemCollection = Nothing)

            If MenuGroup Is Nothing Then
                MenuGroup = MenuControl.Items
            End If

            'Optimizing output
            SetupDefaults(MenuControl)

            'Fill required intermediate levels
            VerifyDataAndFillMissingElements(NavigationData)

            'Start filling in root level
            FillSubElements(NavigationData, "\", MenuGroup)

            'Add SEO-optimized noscript-tag - in case the control has already been added to a parent control's control collection
            Dim InsertIndexPosition As Integer = Utils.LookupControlIndexAtParentControl(MenuControl)
            If InsertIndexPosition > -1 Then
                MenuControl.Parent.Controls.AddAt(InsertIndexPosition, New System.Web.UI.LiteralControl(Utils.SeoNavigation(NavigationData)))
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the navigation control with all data into the specified menugroup
        ''' </summary>
        ''' <param name="NavigationData">The data</param>
        ''' <param name="BaseLevel">The base level where to start</param>
        ''' <param name="MenuGroup">The menu group, the top group if nothing specified</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FillSubElements(ByVal NavigationData As DataTable, ByVal BaseLevel As String, ByVal MenuGroup As ComponentArt.Web.UI.MenuItemCollection)

            Dim MySubElements As DataTable
            MySubElements = Navigation.SubCategories(NavigationData, BaseLevel)
            Dim MyRows As DataRow() = MySubElements.Select("", "Sort ASC, Title ASC")

            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim CurCategory As String = Navigation.ValidNavigationPath(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Title"), ""))
                If CurCategory <> "" Then 'AndAlso Not Navigation.ValueAlreadyExistsInDataTable(MySubElements, "Title", CurCategory) Then
                    'Retrieve the menu title
                    Dim MyMenuTitle As String = CurCategory
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        MyMenuTitle &= " (X)"
                    End If
                    Dim htmlServerTemplate As String = Nothing
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsHtmlEncoded"), False) = False Then
                        'it is plain text!
                        'MyMenuTitle &= "äöÜßÿウェブサイトへようこそ◘vµ┌Ñ☺5Ê‗®Ñ<hr>&nbsp;&amp;"
                        'even if the ComponentArt control doesn't allow HTML code like "<" and ">", it requires the text characters to be correctly HTML encoded
                        'MyMenuTitle = System.Web.HttpUtility.HtmlEncode(MyMenuTitle)
                        htmlServerTemplate = "PlainTextHtml"
                    Else
                        'it is HTML code!
                        htmlServerTemplate = "RenderHtml"
                    End If
                    'Create the new menu item
                    Dim MyMenuItem As ComponentArt.Web.UI.MenuItem
                    MyMenuItem = CreateNavigationItem(MenuGroup, MyMenuTitle, False)
                    If Not htmlServerTemplate Is Nothing Then
                        MyMenuItem.ServerTemplateId = htmlServerTemplate
                    End If
                    'Fill menu item detail info
                    MyMenuItem.ID = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ID"), CType(Nothing, String))
                    MyMenuItem.NavigateUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("URLAutoCompleted"), CType(Nothing, String))
                    MyMenuItem.Target = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Target"), CType(Nothing, String))
                    MyMenuItem.ToolTip = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Tooltip"), CType(Nothing, String))
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        If MyMenuItem.ToolTip = "" Then
                            MyMenuItem.ToolTip = "Disabled for development purposes"
                        Else
                            MyMenuItem.ToolTip &= vbNewLine & vbNewLine & "Disabled for development purposes"
                        End If
                    End If
                    'MyMenuItem.IsForceHighlighted = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsNew"), False) Or Nz(MyRows(MyCounter)("IsUpdated"), False)
                    MyMenuItem.Look.LeftIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrc"), CType(Nothing, String))
                    MyMenuItem.Look.HoverLeftIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcOver"), CType(Nothing, String))
                    'MyMenuItem.Look.LeftIconDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcDown"))
                    MyMenuItem.Look.LeftIconWidth = System.Web.UI.WebControls.Unit.Parse(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconWidth"), CType(Nothing, String)))
                    MyMenuItem.Look.LeftIconHeight = System.Web.UI.WebControls.Unit.Parse(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconHeight"), CType(Nothing, String)))
                    MyMenuItem.Look.RightIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrc"), CType(Nothing, String))
                    MyMenuItem.Look.HoverRightIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcOver"), CType(Nothing, String))
                    'MyMenuItem.Look.RightIconDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcDown"))
                    MyMenuItem.Look.RightIconWidth = System.Web.UI.WebControls.Unit.Parse(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconWidth"), CType(Nothing, String)))
                    MyMenuItem.Look.RightIconHeight = System.Web.UI.WebControls.Unit.Parse(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconHeight"), CType(Nothing, String)))
                    MyMenuItem.TextWrap = Not CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("NoWrap"), False)
                    MyMenuItem.CssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClass"), CType(Nothing, String))
                    MyMenuItem.Look.HoverCssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassOver"), CType(Nothing, String))
                    'MyMenuItem.CssClassDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassDown"))
                    MyMenuItem.ClientSideCommand = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ClientSideOnClick"), CType(Nothing, String))
                End If

                'Recursively search for subelelements
                FillSubElements(NavigationData, CurCategory, MenuGroup)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a navigation item in the menu control
        ''' </summary>
        ''' <param name="NavMenuGroup"></param>
        ''' <param name="TitlePath"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateNavigationItem(ByVal NavMenuGroup As ComponentArt.Web.UI.MenuItemCollection, ByVal TitlePath As String, ByVal IsCurrentlySelectedPath As Boolean) As ComponentArt.Web.UI.MenuItem

            Dim newGroup As ComponentArt.Web.UI.MenuItemCollection = NavMenuGroup
            Dim newItem As ComponentArt.Web.UI.MenuItem = Nothing
            Dim CurCategory As String
            Dim Result As ComponentArt.Web.UI.MenuItem = Nothing
            For Each CurCategory In TitlePath.Split("\"c)
                CurCategory = Trim(CurCategory)
                If CurCategory <> "" Then
                    If Not newItem Is Nothing Then
                        'In previous loops, we've created a new item, but this hasn't got a sub group yet
                        'But since we've got an additional sub category, we have to add a sub group here
                        If newItem.Items.Count = Nothing Then
                            newGroup = newItem.Items
                            ApplyDefaultOverridesForGroups(newItem)
                        Else
                            newGroup = newItem.Items
                        End If
                        'clear newitem because it will be overwritten in the next lines
                        newItem = Nothing
                    End If
                    For MyCounter As Integer = 0 To newGroup.Count - 1
                        Dim MyItem As ComponentArt.Web.UI.MenuItem = newGroup.Item(MyCounter)
                        If MyItem.Text = CurCategory Then
                            'Item already exists, use this
                            newItem = MyItem
                            Exit For
                        End If
                    Next
                    If newItem Is Nothing Then
                        'add new item
                        newItem = New ComponentArt.Web.UI.MenuItem
                        newGroup.Add(newItem)
                        newItem.Text = CurCategory
                        Result = newItem
                    Else
                        'use existing one
                        Result = newItem
                    End If
                    'Result.IsForceHighlighte = IsCurrentlySelectedPath
                End If
            Next
            'Return the last written MenuItem for further modifications by the calling method
            Return Result

        End Function

        Private Sub ApplyDefaultOverridesForGroups(ByVal NavMenuItem As ComponentArt.Web.UI.MenuItem)
            ApplyDefaults(NavMenuItem, _DefaultOverridesForGroups)
        End Sub

        Private Sub ApplyItemDefaults(ByVal NavMenuItem As ComponentArt.Web.UI.MenuItem)
            ApplyDefaults(NavMenuItem, _DefaultsForNewMenuItems)
        End Sub

        Private Sub ApplyDefaults(ByVal NavMenuItem As ComponentArt.Web.UI.MenuItem, ByVal DefaultValues As Collections.Specialized.NameValueCollection)
            For Each MyKey As String In DefaultValues.AllKeys
                Select Case MyKey.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    Case "label"
                        NavMenuItem.Text = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "id"
                        NavMenuItem.ID = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "alt"
                        NavMenuItem.ToolTip = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "url"
                        NavMenuItem.NavigateUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "urltarget"
                        NavMenuItem.Target = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "width"
                        If CType(_DefaultOverridesForGroups(MyKey), String) <> "" Then
                            NavMenuItem.Width = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        End If
                    Case "height"
                        If CType(_DefaultOverridesForGroups(MyKey), String) <> "" Then
                            NavMenuItem.Height = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        End If
                    Case "nowrap"
                        NavMenuItem.TextWrap = Not CType(_DefaultOverridesForGroups(MyKey), Boolean)
                    Case "lefticonsrc"
                        NavMenuItem.Look.LeftIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "lefticonsrcdown"
                        '    NavMenuItem.Look.LeftIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "lefticonheight"
                        NavMenuItem.Look.LeftIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "lefticonsrcover"
                        NavMenuItem.Look.HoverLeftIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "lefticonwidth"
                        NavMenuItem.Look.LeftIconWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "righticonsrc"
                        NavMenuItem.Look.RightIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "righticonsrcdown"
                        '    NavMenuItem.Look.RightIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "righticonheight"
                        NavMenuItem.Look.RightIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "righticonsrcover"
                        NavMenuItem.Look.HoverRightIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "righticonwidth"
                        NavMenuItem.Look.RightIconWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "cssclass"
                        NavMenuItem.CssClass = CType(_DefaultOverridesForGroups(MyKey), String)

                    Case "css classover"
                        NavMenuItem.Look.HoverCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "cssclassdown"
                        '        NavMenuItem.CssClassDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "clientsideonclick"
                        NavMenuItem.ClientSideCommand = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "subgroupwidth"
                        NavMenuItem.SubGroupWidth = System.Web.UI.WebControls.Unit.Parse(CType(_DefaultOverridesForGroups(MyKey), String))
                        NavMenuItem.DefaultSubGroupWidth = System.Web.UI.WebControls.Unit.Parse(CType(_DefaultOverridesForGroups(MyKey), String))
                    Case "subgroupheight"
                        NavMenuItem.SubGroupHeight = System.Web.UI.WebControls.Unit.Parse(CType(_DefaultOverridesForGroups(MyKey), String))
                        NavMenuItem.DefaultSubGroupHeight = System.Web.UI.WebControls.Unit.Parse(CType(_DefaultOverridesForGroups(MyKey), String))
                    Case "subgrouporientation"
                        NavMenuItem.SubGroupOrientation = CType(_DefaultOverridesForGroups(MyKey), ComponentArt.Web.UI.GroupOrientation)
                        NavMenuItem.DefaultSubGroupOrientation = CType(_DefaultOverridesForGroups(MyKey), ComponentArt.Web.UI.GroupOrientation)
                    Case "subgroupitemspacing"
                        NavMenuItem.SubGroupItemSpacing = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        NavMenuItem.DefaultSubGroupItemSpacing = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "subgroupcssclass"
                        NavMenuItem.SubGroupCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        NavMenuItem.DefaultSubGroupCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "subgroupexpanddirection"
                        NavMenuItem.SubGroupExpandDirection = CType(_DefaultOverridesForGroups(MyKey), ComponentArt.Web.UI.GroupExpandDirection)
                        NavMenuItem.DefaultSubGroupExpandDirection = CType(_DefaultOverridesForGroups(MyKey), ComponentArt.Web.UI.GroupExpandDirection)
                        'Case "subgroupexpandeffect"
                        '    NavMenuItem.SubGroupExpandEffect = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case Else
                        Throw New NotImplementedException("Invalid default key """ & MyKey & """")
                End Select
            Next
        End Sub

        Dim _DefaultOverridesForGroups As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultOverridesForGroups() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultOverridesForGroups
            End Get
        End Property

        Dim _DefaultsForNewMenuItems As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultsForNewMenuItems() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultsForNewMenuItems
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Setup some defaults for the current navigation menu
        ''' </summary>
        ''' <param name="MenuControl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetupDefaults(ByVal MenuControl As ComponentArt.Web.UI.Menu)
            MenuControl.ClientScriptLocation = "/system/nav/componentart_webui_client/"
            Dim htmlservertemplate As New ComponentArt.Web.UI.NavigationCustomTemplate
            htmlservertemplate.Template = New RenderHtmlServerTemplate
            htmlservertemplate.ID = "RenderHtml"
            MenuControl.ServerTemplates.Add(htmlservertemplate)
            Dim plaintextservertemplate As New ComponentArt.Web.UI.NavigationCustomTemplate
            plaintextservertemplate.Template = New RenderPlainTextServerTemplate
            plaintextservertemplate.ID = "PlainTextHtml"
            MenuControl.ServerTemplates.Add(plaintextservertemplate)
        End Sub

    End Class

    Public Class ComponentArtSiteMap

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fills a menu
        ''' </summary>
        ''' <param name="NavigationData">The navigation items which shall appear in the navigation</param>
        ''' <param name="MenuControl">The control where to add navigation items</param>
        ''' <param name="MenuGroup">The sub group where the navigation elements shall be added to. Leave empty if you want to use the top level group</param>
        ''' <remarks>
        '''     AspNetMenu is a control of Cyberakt and requires separate licencing
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal NavigationData As DataTable, ByVal MenuControl As ComponentArt.Web.UI.SiteMap, Optional ByVal MenuGroup As ComponentArt.Web.UI.SiteMapNodeCollection = Nothing)

            If MenuGroup Is Nothing Then
                MenuGroup = MenuControl.Nodes
            End If

            'Optimizing output
            SetupDefaults(MenuControl)

            'Fill required intermediate levels
            VerifyDataAndFillMissingElements(NavigationData)

            'Start filling in root level
            FillSubElements(NavigationData, "\", MenuGroup)

            'Add SEO-optimized noscript-tag - in case the control has already been added to a parent control's control collection
            Dim InsertIndexPosition As Integer = Utils.LookupControlIndexAtParentControl(MenuControl)
            If InsertIndexPosition > -1 Then
                MenuControl.Parent.Controls.AddAt(InsertIndexPosition, New System.Web.UI.LiteralControl(Utils.SeoNavigation(NavigationData)))
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the navigation control with all data into the specified menugroup
        ''' </summary>
        ''' <param name="NavigationData">The data</param>
        ''' <param name="BaseLevel">The base level where to start</param>
        ''' <param name="MenuGroup">The menu group, the top group if nothing specified</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FillSubElements(ByVal NavigationData As DataTable, ByVal BaseLevel As String, ByVal MenuGroup As ComponentArt.Web.UI.SiteMapNodeCollection)

            Dim MySubElements As DataTable
            MySubElements = Navigation.SubCategories(NavigationData, BaseLevel)
            Dim MyRows As DataRow() = MySubElements.Select("", "Sort ASC, Title ASC")

            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim CurCategory As String = Navigation.ValidNavigationPath(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Title"), ""))
                If CurCategory <> "" Then 'AndAlso Not Navigation.ValueAlreadyExistsInDataTable(MySubElements, "Title", CurCategory) Then
                    'Retrieve the menu title
                    Dim MyMenuTitle As String = CurCategory
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        MyMenuTitle &= " (X)"
                    End If
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsHtmlEncoded"), False) = False Then
                        'is plain text!
                        'MyMenuTitle &= "äöÜßÿウェブサイトへようこそ◘vµ┌Ñ☺5Ê‗®Ñ<hr>&nbsp;&amp;"
                        'even if the ComponentArt control doesn't allow HTML code like "<" and ">", it requires the text characters to be correctly HTML encoded
                        MyMenuTitle = System.Web.HttpUtility.HtmlEncode(MyMenuTitle)
                    End If
                    'Create the new menu item
                    Dim MyMenuItem As ComponentArt.Web.UI.SiteMapNode
                    MyMenuItem = CreateNavigationItem(MenuGroup, MyMenuTitle, False)
                    'Fill menu item detail info
                    MyMenuItem.ID = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ID"), CType(Nothing, String))
                    MyMenuItem.NavigateUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("URLAutoCompleted"), CType(Nothing, String))
                    MyMenuItem.Target = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Target"), CType(Nothing, String))
                    MyMenuItem.ToolTip = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Tooltip"), CType(Nothing, String))
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        If MyMenuItem.ToolTip = "" Then
                            MyMenuItem.ToolTip = "Disabled for development purposes"
                        Else
                            MyMenuItem.ToolTip &= vbNewLine & vbNewLine & "Disabled for development purposes"
                        End If
                    End If
                    'MyMenuItem.IsForceHighlighted = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsNew"), False) Or Nz(MyRows(MyCounter)("IsUpdated"), False)
                    'MyMenuItem.Look.LeftIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrc"))
                    'MyMenuItem.Look.HoverLeftIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcOver"))
                    'MyMenuItem.Look.LeftIconDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcDown"))
                    'MyMenuItem.Look.LeftIconWidth = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconWidth"))
                    'MyMenuItem.Look.LeftIconHeight = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconHeight"))
                    'MyMenuItem.Look.RightIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrc"))
                    'MyMenuItem.Look.HoverRightIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcOver"))
                    'MyMenuItem.Look.RightIconDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcDown"))
                    'MyMenuItem.Look.RightIconWidth = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconWidth"))
                    'MyMenuItem.Look.RightIconHeight = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconHeight"))
                    'MyMenuItem.TextWrap = Not Nz(MyRows(MyCounter)("NoWrap"), False)
                    MyMenuItem.CssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClass"), CType(Nothing, String))
                    'MyMenuItem.Look.HoverCssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassOver"))
                    'MyMenuItem.CssClassDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassDown"))
                    MyMenuItem.ClientSideCommand = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ClientSideOnClick"), CType(Nothing, String))
                End If

                'Recursively search for subelelements
                FillSubElements(NavigationData, CurCategory, MenuGroup)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a navigation item in the menu control
        ''' </summary>
        ''' <param name="NavMenuGroup"></param>
        ''' <param name="TitlePath"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateNavigationItem(ByVal NavMenuGroup As ComponentArt.Web.UI.SiteMapNodeCollection, ByVal TitlePath As String, ByVal IsCurrentlySelectedPath As Boolean) As ComponentArt.Web.UI.SiteMapNode

            Dim newGroup As ComponentArt.Web.UI.SiteMapNodeCollection = NavMenuGroup
            Dim newItem As ComponentArt.Web.UI.SiteMapNode = Nothing
            Dim CurCategory As String
            Dim Result As ComponentArt.Web.UI.SiteMapNode = Nothing
            For Each CurCategory In TitlePath.Split("\"c)
                CurCategory = Trim(CurCategory)
                If CurCategory <> "" Then
                    If Not newItem Is Nothing Then
                        'In previous loops, we've created a new item, but this hasn't got a sub group yet
                        'But since we've got an additional sub category, we have to add a sub group here
                        If newItem.Nodes.Count = Nothing Then
                            newGroup = newItem.Nodes
                            ApplyDefaultOverridesForGroups(newItem)
                        Else
                            newGroup = newItem.Nodes
                        End If
                        'clear newitem because it will be overwritten in the next lines
                        newItem = Nothing
                    End If
                    For MyCounter As Integer = 0 To newGroup.Count - 1
                        Dim MyItem As ComponentArt.Web.UI.SiteMapNode = newGroup.Item(MyCounter)
                        If MyItem.Text = CurCategory Then
                            'Item already exists, use this
                            newItem = MyItem
                            Exit For
                        End If
                    Next
                    If newItem Is Nothing Then
                        'add new item
                        newItem = New ComponentArt.Web.UI.SiteMapNode
                        newGroup.Add(newItem)
                        newItem.Text = CurCategory
                        Result = newItem
                    Else
                        'use existing one
                        Result = newItem
                    End If
                    'Result.IsForceHighlighte = IsCurrentlySelectedPath
                End If
            Next
            'Return the last written MenuItem for further modifications by the calling method
            Return Result

        End Function

        Private Sub ApplyDefaultOverridesForGroups(ByVal NavMenuItem As ComponentArt.Web.UI.SiteMapNode)
            ApplyDefaults(NavMenuItem, _DefaultOverridesForGroups)
        End Sub

        Private Sub ApplyItemDefaults(ByVal NavMenuItem As ComponentArt.Web.UI.SiteMapNode)
            ApplyDefaults(NavMenuItem, _DefaultsForNewMenuItems)
        End Sub

        Private Sub ApplyDefaults(ByVal NavMenuItem As ComponentArt.Web.UI.SiteMapNode, ByVal DefaultValues As Collections.Specialized.NameValueCollection)
            For Each MyKey As String In DefaultValues.AllKeys
                Select Case MyKey.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    Case "label"
                        NavMenuItem.Text = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "id"
                        NavMenuItem.ID = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "alt"
                        NavMenuItem.ToolTip = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "url"
                        NavMenuItem.NavigateUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "urltarget"
                        NavMenuItem.Target = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "width"
                        If CType(_DefaultOverridesForGroups(MyKey), String) <> "" Then
                            NavMenuItem.Width = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        End If
                    Case "height"
                        If CType(_DefaultOverridesForGroups(MyKey), String) <> "" Then
                            NavMenuItem.Height = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        End If
                        'Case "nowrap"
                        '    NavMenuItem.TextWrap = Not CType(_DefaultOverridesForGroups(MyKey), Boolean)
                        'Case "lefticonsrc"
                        '    NavMenuItem.Look.LeftIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "lefticonsrcdown"
                        '    NavMenuItem.Look.LeftIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "lefticonheight"
                        '    NavMenuItem.Look.LeftIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "lefticonsrcover"
                        '    NavMenuItem.Look.HoverLeftIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "lefticonwidth"
                        '    NavMenuItem.Look.LeftIconWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "righticonsrc"
                        '    NavMenuItem.Look.RightIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "righticonsrcdown"
                        '    NavMenuItem.Look.RightIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "righticonheight"
                        '    NavMenuItem.Look.RightIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "righticonsrcover"
                        '    NavMenuItem.Look.HoverRightIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "righticonwidth"
                        '    NavMenuItem.Look.RightIconWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "cssclass"
                        NavMenuItem.CssClass = CType(_DefaultOverridesForGroups(MyKey), String)

                        'Case "css classover"
                        '    NavMenuItem.Look.HoverCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "cssclassdown"
                        '        NavMenuItem.CssClassDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "clientsideonclick"
                        NavMenuItem.ClientSideCommand = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupwidth"
                        '    NavMenuItem.SubGroupWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        '    NavMenuItem.DefaultSubGroupWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "subgroupheight"
                        '    NavMenuItem.SubGroupHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        '    NavMenuItem.DefaultSubGroupHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "subgrouporientation"
                        '    NavMenuItem.SubGroupOrientation = CType(_DefaultOverridesForGroups(MyKey), String)
                        '    NavMenuItem.DefaultSubGroupOrientation = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupitemspacing"
                        '    NavMenuItem.SubGroupItemSpacing = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        '    NavMenuItem.DefaultSubGroupItemSpacing = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "subgroupcssclass"
                        '    NavMenuItem.SubGroupCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        '    NavMenuItem.DefaultSubGroupCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupexpanddirection"
                        '    NavMenuItem.SubGroupExpandDirection = CType(_DefaultOverridesForGroups(MyKey), String)
                        '    NavMenuItem.DefaultSubGroupExpandDirection = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupexpandeffect"
                        '    NavMenuItem.SubGroupExpandEffect = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case Else
                        Throw New NotImplementedException("Invalid default key """ & MyKey & """")
                End Select
            Next
        End Sub

        Dim _DefaultOverridesForGroups As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultOverridesForGroups() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultOverridesForGroups
            End Get
        End Property

        Dim _DefaultsForNewMenuItems As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultsForNewMenuItems() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultsForNewMenuItems
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Setup some defaults for the current navigation menu
        ''' </summary>
        ''' <param name="MenuControl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetupDefaults(ByVal MenuControl As ComponentArt.Web.UI.SiteMap)
            MenuControl.ClientScriptLocation = "/system/nav/componentart_webui_client/"
        End Sub

    End Class

    Public Class ComponentArtTreeView

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fills a menu
        ''' </summary>
        ''' <param name="NavigationData">The navigation items which shall appear in the navigation</param>
        ''' <param name="MenuControl">The control where to add navigation items</param>
        ''' <param name="MenuGroup">The sub group where the navigation elements shall be added to. Leave empty if you want to use the top level group</param>
        ''' <remarks>
        '''     AspNetMenu is a control of Cyberakt and requires separate licencing
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub Fill(ByVal NavigationData As DataTable, ByVal MenuControl As ComponentArt.Web.UI.TreeView, Optional ByVal MenuGroup As ComponentArt.Web.UI.TreeViewNodeCollection = Nothing)

            If MenuGroup Is Nothing Then
                MenuGroup = MenuControl.Nodes
            End If

            'Optimizing output
            SetupDefaults(MenuControl)

            'Fill required intermediate levels
            VerifyDataAndFillMissingElements(NavigationData)

            'Start filling in root level
            FillSubElements(NavigationData, "\", MenuGroup)

            'Add SEO-optimized noscript-tag - in case the control has already been added to a parent control's control collection
            Dim InsertIndexPosition As Integer = Utils.LookupControlIndexAtParentControl(MenuControl)
            If InsertIndexPosition > -1 Then
                MenuControl.Parent.Controls.AddAt(InsertIndexPosition, New System.Web.UI.LiteralControl(Utils.SeoNavigation(NavigationData)))
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fill the navigation control with all data into the specified menugroup
        ''' </summary>
        ''' <param name="NavigationData">The data</param>
        ''' <param name="BaseLevel">The base level where to start</param>
        ''' <param name="MenuGroup">The menu group, the top group if nothing specified</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FillSubElements(ByVal NavigationData As DataTable, ByVal BaseLevel As String, ByVal MenuGroup As ComponentArt.Web.UI.TreeViewNodeCollection)

            Dim MySubElements As DataTable
            MySubElements = Navigation.SubCategories(NavigationData, BaseLevel)
            Dim MyRows As DataRow() = MySubElements.Select("", "Sort ASC, Title ASC")

            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim CurCategory As String = Navigation.ValidNavigationPath(CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Title"), ""))
                If CurCategory <> "" Then 'AndAlso Not Navigation.ValueAlreadyExistsInDataTable(MySubElements, "Title", CurCategory) Then
                    'Retrieve the menu title
                    Dim MyMenuTitle As String = CurCategory
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        MyMenuTitle &= " (X)"
                    End If
                    Dim htmlServerTemplate As String = Nothing
                    'ComponentArt.Web.UI since V2007
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsHtmlEncoded"), False) = False Then
                        'it is plain text!
                        'MyMenuTitle &= "äöÜßÿウェブサイトへようこそ◘vµ┌Ñ☺5Ê‗®Ñ<hr>&nbsp;&amp;"
                        'even if the ComponentArt control doesn't allow HTML code like "<" and ">", it requires the text characters to be correctly HTML encoded
                        MyMenuTitle = System.Web.HttpUtility.HtmlEncode(MyMenuTitle)
                    Else
                        'it is HTML code!
                        htmlServerTemplate = "RenderHtml"
                    End If
                    'Create the new menu item
                    Dim MyMenuItem As ComponentArt.Web.UI.TreeViewNode
                    MyMenuItem = CreateNavigationItem(MenuGroup, MyMenuTitle, False)
                    If Not htmlServerTemplate Is Nothing Then
                        'MyMenuItem.TemplateId = "RenderHtml"
                        MyMenuItem.ServerTemplateId = "RenderHtml"
                    End If
                    'Fill menu item detail info
                    MyMenuItem.ID = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ID"), CType(Nothing, String))
                    MyMenuItem.NavigateUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("URLAutoCompleted"), CType(Nothing, String))
                    MyMenuItem.Target = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Target"), CType(Nothing, String))
                    MyMenuItem.ToolTip = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Tooltip"), CType(Nothing, String))
                    If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsDisabledInStandardSituations"), False) = True Then
                        If MyMenuItem.ToolTip = "" Then
                            MyMenuItem.ToolTip = "Disabled for development purposes"
                        Else
                            MyMenuItem.ToolTip &= vbNewLine & vbNewLine & "Disabled for development purposes"
                        End If
                    End If
                    'MyMenuItem.IsForceHighlighted = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("IsNew"), False) Or Nz(MyRows(MyCounter)("IsUpdated"), False)
                    'MyMenuItem.Look.LeftIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrc"))
                    'MyMenuItem.Look.HoverLeftIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcOver"))
                    'MyMenuItem.Look.LeftIconDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconSrcDown"))
                    'MyMenuItem.Look.LeftIconWidth = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconWidth"))
                    'MyMenuItem.Look.LeftIconHeight = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("LeftIconHeight"))
                    'MyMenuItem.Look.RightIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrc"))
                    'MyMenuItem.Look.HoverRightIconUrl = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcOver"))
                    'MyMenuItem.Look.RightIconDown = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconSrcDown"))
                    'MyMenuItem.Look.RightIconWidth = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconWidth"))
                    'MyMenuItem.Look.RightIconHeight = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("RightIconHeight"))
                    'MyMenuItem.TextWrap = Not Nz(MyRows(MyCounter)("NoWrap"), False)
                    MyMenuItem.CssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClass"), CType(Nothing, String))
                    MyMenuItem.HoverCssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassOver"), CType(Nothing, String))
                    MyMenuItem.SelectedCssClass = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("CssClassDown"), CType(Nothing, String))
                    MyMenuItem.ClientSideCommand = CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("ClientSideOnClick"), CType(Nothing, String))
                End If

                'Recursively search for subelelements
                FillSubElements(NavigationData, CurCategory, MenuGroup)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Creates a navigation item in the menu control
        ''' </summary>
        ''' <param name="NavMenuGroup"></param>
        ''' <param name="TitlePath"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateNavigationItem(ByVal NavMenuGroup As ComponentArt.Web.UI.TreeViewNodeCollection, ByVal TitlePath As String, ByVal IsCurrentlySelectedPath As Boolean) As ComponentArt.Web.UI.TreeViewNode

            Dim newGroup As ComponentArt.Web.UI.TreeViewNodeCollection = NavMenuGroup
            Dim newItem As ComponentArt.Web.UI.TreeViewNode = Nothing
            Dim CurCategory As String
            Dim Result As ComponentArt.Web.UI.TreeViewNode = Nothing
            For Each CurCategory In TitlePath.Split("\"c)
                CurCategory = Trim(CurCategory)
                If CurCategory <> "" Then
                    If Not newItem Is Nothing Then
                        'In previous loops, we've created a new item, but this hasn't got a sub group yet
                        'But since we've got an additional sub category, we have to add a sub group here
                        If newItem.Nodes.Count = Nothing Then
                            newGroup = newItem.Nodes
                            ApplyDefaultOverridesForGroups(newItem)
                        Else
                            newGroup = newItem.Nodes
                        End If
                        'clear newitem because it will be overwritten in the next lines
                        newItem = Nothing
                    End If
                    For MyCounter As Integer = 0 To newGroup.Count - 1
                        Dim MyItem As ComponentArt.Web.UI.TreeViewNode = newGroup.Item(MyCounter)
                        If MyItem.Text = CurCategory Then
                            'Item already exists, use this
                            newItem = MyItem
                            Exit For
                        End If
                    Next
                    If newItem Is Nothing Then
                        'add new item
                        newItem = New ComponentArt.Web.UI.TreeViewNode
                        newGroup.Add(newItem)
                        newItem.Text = CurCategory
                        Result = newItem
                    Else
                        'use existing one
                        Result = newItem
                    End If
                    'Result.IsForceHighlighte = IsCurrentlySelectedPath
                End If
            Next
            'Return the last written MenuItem for further modifications by the calling method
            Return Result

        End Function

        Private Sub ApplyDefaultOverridesForGroups(ByVal NavMenuItem As ComponentArt.Web.UI.TreeViewNode)
            ApplyDefaults(NavMenuItem, _DefaultOverridesForGroups)
        End Sub

        Private Sub ApplyItemDefaults(ByVal NavMenuItem As ComponentArt.Web.UI.TreeViewNode)
            ApplyDefaults(NavMenuItem, _DefaultsForNewMenuItems)
        End Sub

        Private Sub ApplyDefaults(ByVal NavMenuItem As ComponentArt.Web.UI.TreeViewNode, ByVal DefaultValues As Collections.Specialized.NameValueCollection)
            For Each MyKey As String In DefaultValues.AllKeys
                Select Case MyKey.ToLower(System.Globalization.CultureInfo.InvariantCulture)
                    Case "label"
                        NavMenuItem.Text = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "id"
                        NavMenuItem.ID = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "alt"
                        NavMenuItem.ToolTip = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "url"
                        NavMenuItem.NavigateUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "urltarget"
                        NavMenuItem.Target = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "width"
                        If CType(_DefaultOverridesForGroups(MyKey), String) <> "" Then
                            NavMenuItem.Width = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        End If
                    Case "height"
                        If CType(_DefaultOverridesForGroups(MyKey), String) <> "" Then
                            NavMenuItem.Height = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        End If
                        'Case "nowrap"
                        '    NavMenuItem.TextWrap = Not CType(_DefaultOverridesForGroups(MyKey), Boolean)
                        'Case "lefticonsrc"
                        '    NavMenuItem.Look.LeftIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "lefticonsrcdown"
                        '    NavMenuItem.Look.LeftIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "lefticonheight"
                        '    NavMenuItem.Look.LeftIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "lefticonsrcover"
                        '    NavMenuItem.Look.HoverLeftIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "lefticonwidth"
                        '    NavMenuItem.Look.LeftIconWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "righticonsrc"
                        '    NavMenuItem.Look.RightIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "righticonsrcdown"
                        '    NavMenuItem.Look.RightIconDown = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "righticonheight"
                        '    NavMenuItem.Look.RightIconHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "righticonsrcover"
                        '    NavMenuItem.Look.HoverRightIconUrl = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "righticonwidth"
                        '    NavMenuItem.Look.RightIconWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                    Case "cssclass"
                        NavMenuItem.CssClass = CType(_DefaultOverridesForGroups(MyKey), String)

                        'Case "css classover"
                        '    NavMenuItem.Look.HoverCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "cssclassdown"
                        '        NavMenuItem.CssClassDown = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case "clientsideonclick"
                        NavMenuItem.ClientSideCommand = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupwidth"
                        '    NavMenuItem.SubGroupWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        '    NavMenuItem.DefaultSubGroupWidth = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "subgroupheight"
                        '    NavMenuItem.SubGroupHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        '    NavMenuItem.DefaultSubGroupHeight = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), String))
                        'Case "subgrouporientation"
                        '    NavMenuItem.SubGroupOrientation = CType(_DefaultOverridesForGroups(MyKey), String)
                        '    NavMenuItem.DefaultSubGroupOrientation = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupitemspacing"
                        '    NavMenuItem.SubGroupItemSpacing = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        '    NavMenuItem.DefaultSubGroupItemSpacing = System.Web.UI.WebControls.Unit.Pixel(CType(_DefaultOverridesForGroups(MyKey), Integer))
                        'Case "subgroupcssclass"
                        '    NavMenuItem.SubGroupCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        '    NavMenuItem.DefaultSubGroupCssClass = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupexpanddirection"
                        '    NavMenuItem.SubGroupExpandDirection = CType(_DefaultOverridesForGroups(MyKey), String)
                        '    NavMenuItem.DefaultSubGroupExpandDirection = CType(_DefaultOverridesForGroups(MyKey), String)
                        'Case "subgroupexpandeffect"
                        '    NavMenuItem.SubGroupExpandEffect = CType(_DefaultOverridesForGroups(MyKey), String)
                    Case Else
                        Throw New NotImplementedException("Invalid default key """ & MyKey & """")
                End Select
            Next
        End Sub

        Dim _DefaultOverridesForGroups As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultOverridesForGroups() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultOverridesForGroups
            End Get
        End Property

        Dim _DefaultsForNewMenuItems As New Collections.Specialized.NameValueCollection
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A collection with all defaults required for this navigation menu when a menu group has to be created on the fly
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	04.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property DefaultsForNewMenuItems() As Collections.Specialized.NameValueCollection
            Get
                Return _DefaultsForNewMenuItems
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Setup some defaults for the current navigation menu
        ''' </summary>
        ''' <param name="MenuControl"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub SetupDefaults(ByVal MenuControl As ComponentArt.Web.UI.TreeView)
            MenuControl.ClientScriptLocation = "/system/nav/componentart_webui_client/"
            Dim htmlservertemplate As New ComponentArt.Web.UI.NavigationCustomTemplate
            htmlservertemplate.Template = New RenderHtmlServerTemplate
            htmlservertemplate.ID = "RenderHtml"
            'MenuControl.Templates.Add(htmlservertemplate)
            MenuControl.ServerTemplates.Add(htmlservertemplate)
        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Navigation.RenderHtmlServerTemplate
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     HTML template for ComponentArt.Web.UI controls
    ''' </summary>
    ''' <remarks>
    '''     Since Web.UI 2006.1, the Text property will encode all content; that's why there is the additional requirement for an HTML template allowing the Text property to contain regular HTML code
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	26.07.2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Friend Class RenderHtmlServerTemplate
        Implements System.Web.UI.ITemplate

        Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
            Dim templateContainer As ComponentArt.Web.UI.NavigationTemplateContainer
            templateContainer = CType(container, ComponentArt.Web.UI.NavigationTemplateContainer)
            container.Controls.Add(New System.Web.UI.LiteralControl(templateContainer.Attributes("Text")))
        End Sub
    End Class

    ''' <summary>
    '''     Plain text template for ComponentArt.Web.UI controls
    ''' </summary>
    ''' <remarks>
    '''     Since Web.UI 2006.1, the Text property will encode all content; that's why there is the additional requirement for an HTML template allowing the Text property to contain regular HTML code
    ''' </remarks>
    Friend Class RenderPlainTextServerTemplate
        Implements System.Web.UI.ITemplate

        Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
            Dim templateContainer As ComponentArt.Web.UI.NavigationTemplateContainer
            templateContainer = CType(container, ComponentArt.Web.UI.NavigationTemplateContainer)
            container.Controls.Add(New System.Web.UI.LiteralControl(HttpUtility.HtmlEncode(templateContainer.Attributes("Text"))))
        End Sub
    End Class

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Common utilities for creation of a navigation menu
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminwezel]	05.08.2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Module Utils

        ''' <summary>
        ''' Render an SEO optimized navigation inside of a noscript tag
        ''' </summary>
        ''' <param name="navitems">The navigation items</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SeoNavigation(ByVal navitems As System.Data.DataTable) As String
            Dim BasicNav As New System.Text.StringBuilder()
            BasicNav.Append("<noscript><div class=""SeoNavigation"">")
            For MyCounter As Integer = 0 To navitems.Rows.Count - 1
                Dim NavUrl As String = CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("UrlAutoCompleted"), "")
                Dim NavTarget As String = CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Target"), "")
                Dim NavTooltipTitle As String = CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Tooltip"), "")
                If NavUrl <> "" Then
                    BasicNav.Append("<a href=""")
                    BasicNav.Append(NavUrl)
                    BasicNav.Append("""")
                    If NavTarget <> "" Then
                        BasicNav.Append(" target=""" & NavTarget & """")
                    End If
                    If NavTooltipTitle <> "" Then
                        BasicNav.Append(" title=""" & NavTooltipTitle & """")
                    End If
                    BasicNav.Append(">")
                    If CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("IsHtmlEncoded"), False) = True Then
                        BasicNav.Append(TitleLastLevel(CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Title"), "")))
                    Else
                        BasicNav.Append(HttpUtility.HtmlEncode(TitleLastLevel(CompuMaster.camm.WebManager.Utils.Nz(navitems.Rows(MyCounter)("Title"), ""))))
                    End If
                    BasicNav.Append("</a><br>")
                End If
            Next
            BasicNav.Append("</div></noscript>")
            Return BasicNav.ToString()
        End Function

        ''' <summary>
        ''' Lookup the last title level from a back-slash-separated title hierarchy
        ''' </summary>
        ''' <param name="fullTitleHierarchy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TitleLastLevel(ByVal fullTitleHierarchy As String) As String
            If fullTitleHierarchy = Nothing Then
                Return fullTitleHierarchy
            ElseIf fullTitleHierarchy.LastIndexOf("\"c) >= 0 Then
                Return Mid(fullTitleHierarchy, fullTitleHierarchy.LastIndexOf("\"c))
            Else
                Return fullTitleHierarchy
            End If
        End Function

        ''' <summary>
        ''' Lookup the index of the given control in the control collection of the parent control
        ''' </summary>
        ''' <param name="control">A control</param>
        ''' <returns>An index number or -1 if no parent control is available</returns>
        ''' <remarks></remarks>
        Public Function LookupControlIndexAtParentControl(ByVal control As System.Web.UI.Control) As Integer
            If control.Parent Is Nothing Then
                Return -1
            Else
                For MyCounter As Integer = 0 To control.Parent.Controls.Count - 1
                    If control Is control.Parent.Controls(MyCounter) Then
                        Return MyCounter
                    End If
                Next
                Return -1
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create an empty data table which can contain navigation items
        ''' </summary>
        ''' <returns>A DataTable with all required DataColumns</returns>
        ''' <remarks>
        '''     The table has got following columns:
        ''' <list>
        '''     <item>ID: An ID to reidentify the navigation item</item>
        '''     <item>Title: A back slash ("\") separated string with the complete path for this navigation item
        '''         <example>News\Company\Investments</example></item>
        '''     <item>Sort: An integer value for sorting purposes</item>
        '''     <item>Tooltip: the tooltip text of an element of the navigation</item>
        ''' </list>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	26.07.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CreateEmptyDataTable() As DataTable
            Dim Result As New DataTable
            Result.Columns.Add(New DataColumn("ID", GetType(System.String)))
            Result.Columns.Add(New DataColumn("Title", GetType(System.String)))
            Result.Columns.Add(New DataColumn("Sort", GetType(Integer)))
            Result.Columns.Add(New DataColumn("Tooltip", GetType(System.String)))
            Result.Columns.Add(New DataColumn("IsNew", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("IsUpdated", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("URLPreDefinition", GetType(System.String)))
            Result.Columns.Add(New DataColumn("URLAutoCompleted", GetType(System.String)))
            Result.Columns.Add(New DataColumn("Target", GetType(System.String)))
            Result.Columns.Add(New DataColumn("IsHtmlEncoded", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("IsDisabledInStandardSituations", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("LeftIconSrc", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconSrcOver", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconSrcDown", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconWidth", GetType(System.String)))
            Result.Columns.Add(New DataColumn("LeftIconHeight", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconSrc", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconSrcOver", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconSrcDown", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconWidth", GetType(System.String)))
            Result.Columns.Add(New DataColumn("RightIconHeight", GetType(System.String)))
            Result.Columns.Add(New DataColumn("NoWrap", GetType(System.Boolean)))
            Result.Columns.Add(New DataColumn("CssClass", GetType(System.String)))
            Result.Columns.Add(New DataColumn("CssClassOver", GetType(System.String)))
            Result.Columns.Add(New DataColumn("CssClassDown", GetType(System.String)))
            Result.Columns.Add(New DataColumn("ClientSideOnClick", GetType(System.String)))

            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Verify the data to contain nothing invalid or duplicates and add required intermediate levels
        ''' </summary>
        ''' <param name="navData">The table with the navigation elements</param>
        ''' <remarks>
        '''     Other checked things are e. g. no singles back-slashes in category or that the URLAutoCompleted contains at least the URLPreDefinition value
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub VerifyDataAndFillMissingElements(ByVal navData As DataTable)
            Dim ColumnIndexTitle As Integer = navData.Columns("Title").Ordinal

            VerifyDataAndFillMissingElementsFixTitleContent(navData, ColumnIndexTitle)
            VerifyDataAndFillMissingElementsAddMissingHierarchyLevels(navData, ColumnIndexTitle)
            VerifyDataAndFillMissingAutoCompletedURLs(navData)
            VerifyDataAndFillMissingElementsRemoveDuplicates(navData, ColumnIndexTitle)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Fix title contents
        ''' </summary>
        ''' <param name="navdata"></param>
        ''' <param name="columnIndexTitle"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingElementsFixTitleContent(ByVal navdata As DataTable, ByVal columnIndexTitle As Integer)

            'Fix title content by truncating trailing backslashes, etc.
            Dim MyRows As DataRow() = navdata.Select("", "Title ASC")

            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim MyCurrentRow As DataRow = MyRows(MyCounter)
                Dim MyCurrentTitle As String = CompuMaster.camm.WebManager.Utils.Nz(MyCurrentRow(columnIndexTitle), "")
                'fix leading but prohibited back slashes in front of a non-root value
                While MyCurrentTitle.Length > 1 AndAlso Mid(MyCurrentTitle, 1, 1) = "\"
                    MyCurrentTitle = Mid(MyCurrentTitle, 2)
                    MyCurrentRow(columnIndexTitle) = MyCurrentTitle
                End While
                'fix trailing but prohibited back slashes in front of a non-root value
                While MyCurrentTitle.Length > 1 AndAlso Right(MyCurrentTitle, 1) = "\"
                    MyCurrentTitle = Mid(MyCurrentTitle, 1, MyCurrentTitle.Length - 1)
                    MyCurrentRow(columnIndexTitle) = MyCurrentTitle
                End While
                If MyCurrentTitle = "\" Then
                    MyCurrentTitle = ""
                    MyCurrentRow(columnIndexTitle) = MyCurrentTitle
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Add missing hierarchy levels
        ''' </summary>
        ''' <param name="navData"></param>
        ''' <param name="columnIndexTitle"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingElementsAddMissingHierarchyLevels(ByVal navData As DataTable, ByVal columnIndexTitle As Integer)

            Dim ColumnIndexID As Integer = navData.Columns("ID").Ordinal
            Dim MyRows As DataRow() = navData.Select("", "Title ASC")

            'Fill hashtable with all available hierarchy titles
            Dim ht As New Hashtable
            For Each r As DataRow In MyRows
                If Not ht.ContainsKey(r(columnIndexTitle)) Then
                    ht.Add(CompuMaster.camm.WebManager.Utils.Nz(r(columnIndexTitle), ""), Nothing)
                End If
            Next

            'Find missing intermediate levels by splitting every hierarchy title, reconstruction of every sublevel and adding to the hashtable if not yet there
            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim MyCurrentRow As DataRow
                Dim MyCurrentTitle As String
                'update references to current/previous row
                MyCurrentRow = MyRows(MyCounter)
                MyCurrentTitle = CompuMaster.camm.WebManager.Utils.Nz(MyCurrentRow(columnIndexTitle), "")
                'comparisons to detect missing intermediate levels
                Dim MyCurrentTitleSubParts As String() = MyCurrentTitle.Split("\"c)
                For MyLevelCounter As Integer = 0 To MyCurrentTitleSubParts.Length - 1
                    Dim NewEntry As String = String.Join("\", MyCurrentTitleSubParts, 0, MyLevelCounter + 1)
                    If Not ht.ContainsKey(NewEntry) Then
                        'missing intermediate level found - add it now
                        Dim MyNewRow As DataRow = navData.NewRow()
                        MyNewRow(columnIndexTitle) = NewEntry
                        navData.Rows.Add(MyNewRow)
                        ht.Add(NewEntry, Nothing)
                    End If
                Next
            Next
            ht = Nothing

        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Copy missing values from base navigation URL to auto-completed URL field
        ''' </summary>
        ''' <param name="navdata"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingAutoCompletedURLs(ByVal navdata As DataTable)

            Dim MyRows As DataRow() = navdata.Select("", "Title ASC")

            'Copy the URLPreDefinition value to the URLAutoCompleted field if there hasn`t been a value yet
            Dim ColumnIndexUrlAutoCompleted As Integer = navdata.Columns("URLAutoCompleted").Ordinal
            Dim ColumnIndexUrlPredefined As Integer = navdata.Columns("URLPreDefinition").Ordinal
            For MyCounter As Integer = 0 To MyRows.Length - 1
                If CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)(ColumnIndexUrlAutoCompleted), "") = "" Then
                    MyRows(MyCounter)(ColumnIndexUrlAutoCompleted) = MyRows(MyCounter)(ColumnIndexUrlPredefined)
                End If
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Remove any doubled rows
        ''' </summary>
        ''' <param name="navData"></param>
        ''' <param name="columnIndexTitle"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	22.04.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub VerifyDataAndFillMissingElementsRemoveDuplicates(ByVal navData As DataTable, ByVal columnIndexTitle As Integer)
            'Remove now all duplicates found 
            Dim MyRows As DataRow() = navData.Select("", "Title ASC")
            Dim MyCurrentRow As DataRow
            Dim MyPreviousRow As DataRow
            MyCurrentRow = Nothing
            MyPreviousRow = Nothing
            For MyCounter As Integer = 0 To MyRows.Length - 1
                If Not MyCurrentRow Is Nothing AndAlso MyCurrentRow.RowState = DataRowState.Detached Then
                    'if we are in a second round and the old (currently called as "MyCurrentRow" because 
                    'it would be switched in the next line) row has been deleted, then keep the value of the very previous row
                Else
                    MyPreviousRow = MyCurrentRow
                End If
                MyCurrentRow = MyRows(MyCounter)
                Dim MyPreviousTitle As String
                Dim MyCurrentTitle As String
                If MyPreviousRow Is Nothing OrElse IsDBNull(MyPreviousRow(columnIndexTitle)) Then
                    MyPreviousTitle = Nothing
                Else
                    MyPreviousTitle = CompuMaster.camm.WebManager.Utils.Nz(MyPreviousRow(columnIndexTitle), CType(Nothing, String))
                End If
                If IsDBNull(MyCurrentRow(columnIndexTitle)) Then
                    MyCurrentTitle = Nothing
                Else
                    MyCurrentTitle = CompuMaster.camm.WebManager.Utils.Nz(MyCurrentRow(columnIndexTitle), CType(Nothing, String))
                End If
                If MyCurrentTitle = MyPreviousTitle Or MyCurrentTitle = "" Then
                    'this is a dubplicate, mark to remove this line
                    navData.Rows.Remove(MyCurrentRow)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Is a searched category member of another list of categories at a defined sub level?
        ''' </summary>
        ''' <param name="searchForCategory">The searched category name</param>
        ''' <param name="listOfCategories">The semi-colon separated list of categories</param>
        ''' <param name="searchWithinSubelements">The path of the sub level where the searched category shall be found</param>
        ''' <returns>True if the searched category is already there</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	18.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsCategoryInListOfCategories(ByVal searchForCategory As String, ByVal listOfCategories As String, ByVal searchWithinSubelements As Boolean) As Boolean
            Dim CurCategory As String
            If Trim(listOfCategories) = "" Then
                listOfCategories = "\"
            End If
            If Trim(searchForCategory) = "" Then
                searchForCategory = "\"
            End If
            If Left(searchForCategory, 1) = "\" AndAlso searchForCategory.Length > 1 Then
                'truncate unneeded, trailing slash
                searchForCategory = Mid(searchForCategory, 2)
            End If
            If Right(searchForCategory, 1) = "\" AndAlso searchForCategory.Length > 1 Then
                'truncate unneeded, trailing slash
                searchForCategory = Mid(searchForCategory, 1, searchForCategory.Length - 1)
            End If
            If searchWithinSubelements = True AndAlso searchForCategory = "\" Then
                'root level is always there
                Return True
            End If
            For Each CurCategory In listOfCategories.Split(";"c)
                CurCategory = Trim(CurCategory)
                If CurCategory = "" Then
                    CurCategory = "\"
                End If
                If Right(CurCategory, 1) = "\" AndAlso CurCategory.Length > 1 Then
                    'truncate unneeded, trailing slash
                    CurCategory = Mid(CurCategory, 1, CurCategory.Length - 1)
                End If
                If searchWithinSubelements = True Then
                    'searched category found by same path or by any corresponding sub paths
                    If CurCategory = searchForCategory OrElse Mid(CurCategory, 1, searchForCategory.Length + 1) = searchForCategory & "\" Then
                        Return True
                    End If
                Else
                    'exact match required
                    If CurCategory = searchForCategory Then
                        Return True
                    End If
                End If
            Next
            Return False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     A list of category paths for all possibilities levels
        ''' </summary>
        ''' <param name="CategoryPath">The category to split</param>
        ''' <returns>A sorted list of category paths for all possibilities levels</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function CategoryPathElements(ByVal CategoryPath As String) As SortedList
            CategoryPath = ValidNavigationPath(CategoryPath)
            If CategoryPath = "\" Then
                'no elements in path
                Return New SortedList
            End If
            Dim Categories As New Collections.SortedList
            Dim CurCategoryPath As String = Nothing
            Dim CurCategory As String
            For Each CurCategory In CategoryPath.Split("\"c)
                CurCategory = Trim(CurCategory)
                If CurCategory <> "" Then
                    If CurCategoryPath = "" Then
                        CurCategoryPath = CurCategory
                    Else
                        CurCategoryPath &= "\" & CurCategory
                    End If
                    Categories.Add(CurCategoryPath, CurCategory)
                End If
            Next
            Return Categories
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieve all rows of the data table which are direct sub elements of a specified base level
        ''' </summary>
        ''' <param name="NavData">The data table containing all navigation data</param>
        ''' <param name="CurrentBaseLevel">The base level where to search for child elements</param>
        ''' <returns>All child rows which match the specified base level</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function SubCategories(ByVal NavData As DataTable, ByVal CurrentBaseLevel As String) As DataTable
            CurrentBaseLevel = ValidNavigationPath(CurrentBaseLevel)
            If CurrentBaseLevel = "\" Then
                CurrentBaseLevel = ""
            End If
            Dim MyTable As DataTable = NavData
            Dim Result As DataTable = CreateEmptyDataTable()
            If MyTable Is Nothing Then
                Return Result
            End If

            Dim MyRows As DataRow() = MyTable.Select("", "Sort ASC, Title ASC")
            For MyCounter As Integer = 0 To MyRows.Length - 1
                Dim CurNewsCategories As String = CType(MyRows(MyCounter)("Title"), System.String)
                Dim CurCategory As String = CurNewsCategories
                'For Each CurCategory In CurNewsCategories.Split(";")
                CurCategory = ValidNavigationPath(CurCategory)
                Dim EndPos As Integer = InStr(Len(CurrentBaseLevel & "\") + 1, CurCategory, "\")
                Dim NextCategoryLevelName As String 'next sub element without base category
                Dim NextCategoryLevelTitle As String 'complete nav title path
                If EndPos = 0 Then
                    NextCategoryLevelName = CurCategory
                Else
                    NextCategoryLevelName = Mid(CurCategory, 1, EndPos - 1)
                End If
                If NextCategoryLevelName = "" Then
                    NextCategoryLevelName = "\"
                End If
                If InStr(NextCategoryLevelName, CurrentBaseLevel) <> 1 Then
                    NextCategoryLevelTitle = ""
                Else
                    If CurrentBaseLevel = "" Then
                        NextCategoryLevelTitle = Mid(NextCategoryLevelName, 1)
                    Else
                        NextCategoryLevelTitle = Mid(NextCategoryLevelName, Len(CurrentBaseLevel & "\") + 1)
                    End If
                End If
                If NextCategoryLevelName <> "\" AndAlso NextCategoryLevelTitle <> "" AndAlso CompuMaster.camm.WebManager.Utils.Nz(MyRows(MyCounter)("Title"), CType(Nothing, String)) = NextCategoryLevelName Then
                    'Copy data row
                    Dim MyNewRow As DataRow
                    MyNewRow = Result.NewRow
                    For Each MyCol As DataColumn In Result.Columns
                        If Not MyTable.Columns(MyCol.ColumnName) Is Nothing Then
                            'only if the source column exists
                            MyNewRow(MyCol.ColumnName) = MyRows(MyCounter)(MyCol.ColumnName)
                        End If
                    Next
                    Result.Rows.Add(MyNewRow)
                End If
            Next
            'Next
            Return Result
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Looks up for a value in a defined column of a data table
        ''' </summary>
        ''' <param name="DataTable">The data table where to search in</param>
        ''' <param name="ColumnName">The column of the data table where to search in</param>
        ''' <param name="Value">The value to search for</param>
        ''' <returns>True if the value already exists in the list of active rows</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValueAlreadyExistsInDataTable(ByVal DataTable As DataTable, ByVal ColumnName As String, ByVal Value As Long) As Boolean
            Dim MyRows As DataRow() = DataTable.Select("[" & ColumnName.Replace("]", "[]") & "]=" & Value)
            If MyRows.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Looks up for a value in a defined column of a data table
        ''' </summary>
        ''' <param name="DataTable">The data table where to search in</param>
        ''' <param name="ColumnName">The column of the data table where to search in</param>
        ''' <param name="Value">The value to search for</param>
        ''' <returns>True if the value already exists in the list of active rows</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValueAlreadyExistsInDataTable(ByVal DataTable As DataTable, ByVal ColumnName As String, ByVal Value As String) As Boolean
            Dim MyRows As DataRow() = DataTable.Select("[" & ColumnName.Replace("]", "[]") & "]='" & Value.Replace("'", "''") & "'")
            If MyRows.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Looks up for a value in a defined column of a data table
        ''' </summary>
        ''' <param name="DataTable">The data table where to search in</param>
        ''' <param name="ColumnName">The column of the data table where to search in</param>
        ''' <param name="Value">The value to search for</param>
        ''' <returns>True if the value already exists in the list of active rows</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValueAlreadyExistsInDataTable(ByVal DataTable As DataTable, ByVal ColumnName As String, ByVal Value As DBNull) As Boolean
            Dim MyRows As DataRow() = DataTable.Select("[" & ColumnName.Replace("]", "[]") & "]=NULL")
            If MyRows.Length = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Return the upper level of a category path
        ''' </summary>
        ''' <param name="category">A category path</param>
        ''' <returns>The parent category or a backslash for the root value</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ParentCategory(ByVal category As String) As String

            If category = "" OrElse category = "\" Then
                'No parent path available, we're already there!
                Return Nothing
            End If
            category = ValidNavigationPath(category)

            Dim EndPos As Integer = InStrRev(category, "\")
            Dim NextCategoryLevelName As String
            If EndPos = 0 Then
                NextCategoryLevelName = "\"
            Else
                NextCategoryLevelName = Mid(category, 1, EndPos - 1)
            End If
            Return NextCategoryLevelName

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Validates a correct construction of a category title
        ''' </summary>
        ''' <param name="path">The path to be validated</param>
        ''' <returns>The path wihtout starting or trailing back slashes or one backslash in case of the root level</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend Function ValidNavigationPath(ByVal path As String) As String

            Dim Result As String = Trim(path)

            If Result = "" Then
                'No parent path available, we're already there!
                Result = "\"
            End If

            If Left(Result, 1) = "\" AndAlso Result.Length > 1 Then
                'truncate unneeded, beginning slash
                Result = Mid(Result, 2)
            End If
            If Right(Result, 1) = "\" AndAlso Result.Length > 1 Then
                'truncate unneeded, trailing slash
                Result = Mid(Result, 1, Result.Length - 1)
            End If

            Return Result

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Retrieves the sub categories
        ''' </summary>
        ''' <param name="basePath">A string containing the path which should be removed</param>
        ''' <param name="completePath">The complete path of a category</param>
        ''' <returns>The relative sub path between the basePath and the completePath. Empty if the basePath is not part of the completePath.</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminwezel]	05.08.2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function RelativeCategory(ByVal basePath As String, ByVal completePath As String) As String
            If Mid(basePath, 1, 1) = "\" Then
                'remove starting back slahes
                basePath = Mid(basePath, 2)
            End If
            If Mid(completePath, 1, 1) = "\" Then
                'remove starting back slahes
                completePath = Mid(completePath, 2)
            End If
            If basePath <> "" Then
                'ensure a trailing back slash
                If Right(basePath, 1) <> "\" Then
                    basePath &= "\"
                End If
            Else
                'if nothing let it be one back slash
                basePath = "\"
            End If
            If completePath = "" Then
                'if nothing let it be one back slash
                completePath = "\"
            Else
                'ensure a trailing back slash
                If Right(completePath, 1) <> "\" Then
                    completePath &= "\"
                End If
            End If
            'retrieve the sub path
            Dim Result As String
            If completePath = basePath Then
                Result = "\" 'gets cutted later
            ElseIf basePath = "\" Then
                'starting from root, the result is always the complete path
                Result = completePath
            ElseIf InStr(completePath, basePath) <> 1 Then
                Result = "\" 'don't return completePath here even if this might make sense. But we're not accepting the result to say go to root level first. We must return a relative value.
            Else
                Result = Mid(completePath, basePath.Length + 1)
            End If
            'return without the trailing slash
            Return Mid(Result, 1, Result.Length - 1)
        End Function

    End Module

End Namespace