Option Explicit On 
Option Strict On

Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Collections
Imports System.Globalization
Imports System.Reflection

#If NotYetImplemented Then
Namespace CompuMaster.camm.WebManager.Tools.Reflector

#Region "Recommended easy2use methods"
    'Public Class DetailedInformation

    '    Public SelectedAssembly As String
    '    Public SelectedNameSpace As String
    '    Public ModuleName As New ArrayList

    '    Protected Sub New(ByVal ObjectToReflectOn As Object)

    '    End Sub

    '    Protected Sub New(ByVal TypeToReflectOn As System.Type)

    '        Dim ConfigSettings As NameValueCollection = CType(Context.GetConfig("system.web/ClassBrowser"), NameValueCollection)
    '        Dim I As Integer

    '        For I = 0 To ConfigSettings.Count - 1
    '            ModuleName.Add(ConfigSettings(I).ToString())
    '        Next

    '        If Request.QueryString("namespace") Is Nothing Then
    '            SelectedNameSpace = "System"
    '        Else
    '            SelectedNameSpace = Request.QueryString("namespace")
    '        End If

    '        If Request.QueryString("assembly") Is Nothing Or Request.QueryString("assembly") = "" Then
    '            SelectedAssembly = "mscorlib"
    '        Else
    '            SelectedAssembly = Request.QueryString("assembly")
    '        End If

    '        If Not Request.QueryString("class") Is Nothing And Not Request.QueryString("assembly") Is Nothing Then
    '            DisplayClass(Request.QueryString("assembly"), Request.QueryString("class"))
    '        Else
    '            DisplayClassList(SelectedNameSpace)
    '        End If
    '    End Sub

    '    Private Sub DisplayNamespaces()

    '        Dim NameSpaceList As New ArrayList
    '        Dim NameSpaceHash As New Hashtable

    '        Dim Y As Integer
    '        For Y = 0 To ModuleName.Count - 1

    '            Dim CorRuntime() As System.Reflection.Module = System.Reflection.Assembly.Load(ModuleName(Y).ToString()).GetModules()
    '            Dim CorClasses() As Type = CorRuntime(0).GetTypes()

    '            Dim X As Integer
    '            For X = 0 To CorClasses.Length - 1
    '                If Not CorClasses(X).Namespace Is Nothing Then
    '                    If Not NameSpaceHash.ContainsKey(CorClasses(X).Namespace) And CorClasses(X).IsPublic Then
    '                        NameSpaceHash.Add(CorClasses(X).Namespace, "")
    '                        NameSpaceList.Add(CorClasses(X).Namespace)
    '                    End If
    '                End If
    '            Next
    '        Next

    '        NameSpaceList.Sort()
    '        Namespace1.DataSource = NameSpaceList
    '        Namespace1.DataBind()
    '    End Sub

    '    Private Sub DisplayClassList(ByVal CurrentNameSpace As String)

    '        Dim ClassList As New ArrayList
    '        Dim InterfaceList As New ArrayList
    '        Dim Y As Integer

    '        For Y = 0 To ModuleName.Count - 1

    '            Dim CorRuntime() As System.Reflection.Module = System.Reflection.Assembly.Load(ModuleName(Y).ToString()).GetModules()
    '            Dim CorClasses() As Type = CorRuntime(0).GetTypes()
    '            Dim X As Integer

    '            For X = 0 To CorClasses.Length - 1

    '                If CorClasses(X).Namespace = CurrentNameSpace And CorClasses(X).IsPublic Then
    '                    Dim props As New SortTable("GetType")
    '                    props("GetType") = CorClasses(X).Name
    '                    props("Namespace") = CorClasses(X).Namespace
    '                    props("Assembly") = CorClasses(X).Assembly.ToString()

    '                    If CorClasses(X).IsInterface Then
    '                        InterfaceList.Add(props)
    '                    Else
    '                        ClassList.Add(props)
    '                    End If
    '                End If
    '            Next
    '        Next

    '        If InterfaceList.Count > 0 Then IHeader.Visible = True

    '        If ClassList.Count > 0 Then CHeader.Visible = True

    '        ClassList.Sort()
    '        Classes.DataSource = ClassList
    '        Classes.DataBind()

    '        InterfaceList.Sort()
    '        Interfaces.DataSource = InterfaceList
    '        Interfaces.DataBind()
    '    End Sub

    '    Private Sub DisplayClass(ByVal asmName As String, ByVal className As String)

    '        If asmName Is Nothing Or asmName = "" Then
    '            DisplayClassList(SelectedNameSpace)
    '            Return
    '        End If

    '        Dim a As System.Reflection.Assembly = System.Reflection.Assembly.Load(asmName)
    '        Dim ClassType As Type = a.GetType(SelectedNameSpace.ToString() & "." & className, False, True)

    '        If ClassType Is Nothing Then
    '            DisplayClassList(SelectedNameSpace)
    '            Return
    '        End If

    '        Dim SubClassDetails As ArrayList = New DisplaySubClasses(ClassType, ModuleName)
    '        Dim ConstructorDetails As DisplayConstructors = New DisplayConstructors(ClassType)
    '        Dim FieldDetails As DisplayFields = New DisplayFields(ClassType)
    '        Dim PropertyDetails As DisplayProperties = New DisplayProperties(ClassType)
    '        Dim MethodDetails As DisplayMethods = New DisplayMethods(ClassType, className)
    '        Dim SuperClassDetails As DisplaySuperclasses = New DisplaySuperclasses(ClassType)
    '        Dim InterfaceDetails As DisplayInterfaces = New DisplayInterfaces(ClassType)
    '        Dim EventDetails As DisplayEvents = New DisplayEvents(ClassType)

    '        If ConstructorDetails.Count <> 0 Then Constructors.DataSource = ConstructorDetails
    '        If SubClassDetails.Count <> 0 Then SubClasses.DataSource = SubClassDetails
    '        If FieldDetails.Count <> 0 Then Fields.DataSource = FieldDetails
    '        If PropertyDetails.Count <> 0 Then Properties.DataSource = PropertyDetails
    '        If MethodDetails.Count <> 0 Then Methods.DataSource = MethodDetails
    '        If InterfaceDetails.Count <> 0 Then Interface1.DataSource = InterfaceDetails
    '        If SuperClassDetails.Count <> 0 Then SuperClasses.DataSource = SuperClassDetails
    '        If EventDetails.Count <> 0 Then Events.DataSource = EventDetails

    '        DataBind()

    '        If ClassType.IsInterface Then
    '            spnClassName.InnerHtml = "Interface " & SelectedNameSpace & "." & className
    '        Else
    '            spnClassName.InnerHtml = "Class " & SelectedNameSpace & "." & className
    '        End If

    '        NameSpacePanel.Visible = False
    '        ClassPanel.Visible = True
    '    End Sub

    '    Public Function GetInfo() As DataSet

    '    End Function
    'End Class
#End Region

#Region "META data container"
    Public Class SortTable : Inherits Hashtable : Implements IComparable
        Public sortField As String

        Sub New()
            sortField = Nothing
        End Sub

        Sub New(ByVal sField As String)
            sortField = sField
        End Sub

        Public Function CompareTo(ByVal B As Object) As Integer Implements IComparable.CompareTo
            If sortField Is Nothing Then
                Return 0
            End If
            Return (String.Compare(CStr(Me(sortField)), CStr((CType(B, SortTable))(sortField)), False, CultureInfo.InvariantCulture))
        End Function
    End Class
#End Region

#Region "META data extraction"
    Public Class DisplayEvents : Inherits ArrayList
        Public Sub New(ByVal classType As Type)
            Dim eventInfos() As System.Reflection.EventInfo = classType.GetEvents()

            If eventInfos Is Nothing Then Return

            Dim eventTable As ArrayList = New ArrayList
            Dim X As Integer

            For X = 0 To eventInfos.Length - 1
                Dim eventDetails As New SortTable("Name")

                eventDetails("Assembly") = eventInfos(X).EventHandlerType.Assembly.ToString()
                eventDetails("Name") = eventInfos(X).Name
                eventDetails("Type") = eventInfos(X).EventHandlerType.Name
                eventDetails("GetType") = eventInfos(X).EventHandlerType.Name
                eventDetails("Namespace") = eventInfos(X).EventHandlerType.Namespace

                If eventInfos(X).IsMulticast Then
                    eventDetails("Access") = "multicast "
                End If

                Me.Add(eventDetails)
            Next
            Me.Sort()
        End Sub
    End Class

    Public Class DisplayFields : Inherits ArrayList
        Public Sub New(ByVal classType As Type)
            Dim fieldInfos() As System.Reflection.FieldInfo = classType.GetFields()

            If fieldInfos Is Nothing Then Return

            Dim fieldTable As New ArrayList
            Dim X As Integer

            For X = 0 To fieldInfos.Length - 1
                Dim fieldDetails As New SortTable("Name")

                fieldDetails("Assembly") = fieldInfos(X).GetType().Assembly.ToString()
                fieldDetails("Name") = fieldInfos(X).Name
                fieldDetails("Type") = fieldInfos(X).FieldType.Name

                If (fieldInfos(X).FieldType.IsArray And fieldInfos(X).FieldType.Name <> "Array") Or fieldInfos(X).FieldType.IsPointer Then
                    fieldDetails("GetType") = fieldInfos(X).FieldType.GetElementType().Name
                    fieldDetails("Namespace") = fieldInfos(X).FieldType.GetElementType().Namespace
                Else
                    fieldDetails("GetType") = fieldInfos(X).FieldType.Name
                    fieldDetails("Namespace") = fieldInfos(X).FieldType.Namespace
                End If

                If fieldInfos(X).IsPublic Then
                    fieldDetails("Access") = "public "
                ElseIf fieldInfos(X).IsPrivate Then
                    fieldDetails("Access") = "private "
                ElseIf fieldInfos(X).IsFamily Then
                    fieldDetails("Access") = "protected "
                End If

                If fieldInfos(X).IsStatic Then
                    fieldDetails("Access") = CStr(fieldDetails("Access")) & "static "
                End If

                If fieldInfos(X).IsLiteral Then
                    fieldDetails("Access") = CStr(fieldDetails("Access")) & "const "
                End If

                Me.Add(fieldDetails)
            Next
            Me.Sort()
        End Sub
    End Class

    Public Class DisplayConstructors : Inherits ArrayList
        Public Sub New(ByVal classType As Type)
            Dim constructorInfos() As System.Reflection.ConstructorInfo = classType.GetConstructors()

            If constructorInfos Is Nothing Then Return

            Dim X As Integer
            For X = 0 To constructorInfos.Length - 1
                Dim constructorDetails As New SortTable

                constructorDetails("Assembly") = constructorInfos(X).GetType().Assembly.ToString()
                constructorDetails("Name") = classType.Name

                If constructorInfos(X).IsPublic Then
                    constructorDetails("Access") = "public "
                ElseIf constructorInfos(X).IsPrivate Then
                    constructorDetails("Access") = "private "
                ElseIf constructorInfos(X).IsFamily Then
                    constructorDetails("Access") = "protected "
                End If

                Dim paramInfos() As System.Reflection.ParameterInfo = constructorInfos(X).GetParameters()

                If Not paramInfos Is Nothing Then
                    Dim paramTable As New ArrayList
                    Dim Y As Integer

                    For Y = 0 To paramInfos.Length - 1
                        Dim paramDetails As New SortTable
                        paramDetails("Assembly") = paramInfos(Y).GetType().Assembly.ToString()
                        paramDetails("ParamName") = paramInfos(Y).Name
                        paramDetails("ParamType") = paramInfos(Y).ParameterType.Name
                        If ((paramInfos(Y).ParameterType.IsArray And paramInfos(Y).ParameterType.Name <> "Array") Or paramInfos(Y).ParameterType.IsPointer) Then
                            paramDetails("GetType") = paramInfos(Y).ParameterType.GetElementType().Name
                            paramDetails("Namespace") = paramInfos(Y).ParameterType.GetElementType().Namespace
                        Else
                            paramDetails("GetType") = paramInfos(Y).ParameterType.Name
                            paramDetails("Namespace") = paramInfos(Y).ParameterType.Namespace
                        End If
                        paramTable.Add(paramDetails)
                    Next

                    constructorDetails("Params") = paramTable
                End If

                Me.Add(constructorDetails)
            Next
        End Sub
    End Class

    Public Class DisplayProperties : Inherits ArrayList
        Public Sub New(ByVal classType As Type)
            Dim propertyInfos() As System.Reflection.PropertyInfo = classType.GetProperties()

            If propertyInfos Is Nothing Then Return

            Dim propertyTable As New ArrayList
            Dim X As Integer
            For X = 0 To propertyInfos.Length - 1
                Dim propertyDetails As New SortTable("Name")

                If Not propertyInfos(X).GetGetMethod() Is Nothing Then
                    If (propertyInfos(X).GetGetMethod().ReturnType.IsArray And propertyInfos(X).GetGetMethod().ReturnType.Name <> "Array") Or propertyInfos(X).GetGetMethod().ReturnType.IsPointer Then
                        propertyDetails("GetType") = propertyInfos(X).GetGetMethod().ReturnType.GetElementType().Name
                        propertyDetails("Namespace") = propertyInfos(X).GetGetMethod().ReturnType.GetElementType().Namespace
                    Else
                        propertyDetails("GetType") = propertyInfos(X).GetGetMethod().ReturnType.Name
                        propertyDetails("Namespace") = propertyInfos(X).GetGetMethod().ReturnType.Namespace
                    End If
                    propertyDetails("Type") = propertyInfos(X).GetGetMethod().ReturnType.Name
                    propertyDetails("Assembly") = propertyInfos(X).GetGetMethod().ReturnType.Assembly.ToString()
                    propertyDetails("Name") = propertyInfos(X).Name

                    If propertyInfos(X).GetGetMethod().IsPublic Then
                        propertyDetails("Visibility") = "public"
                    ElseIf propertyInfos(X).GetGetMethod().IsPrivate Then
                        propertyDetails("Visibility") = "private"
                    ElseIf propertyInfos(X).GetGetMethod().IsFamily Then
                        propertyDetails("Visibility") = "protected"
                    End If

                    If propertyInfos(X).GetGetMethod().IsStatic Then
                        propertyDetails("Visibility") = CStr(propertyDetails("Visibility")) & " static"
                    End If

                    If propertyInfos(X).GetSetMethod() Is Nothing Then
                        propertyDetails("Access") = "(Get)"
                    Else
                        propertyDetails("Access") = "(Get , Set)"
                    End If

                    Dim paramInfos() As System.Reflection.ParameterInfo = propertyInfos(X).GetGetMethod().GetParameters()

                    If Not paramInfos Is Nothing Then
                        Dim paramTable As New ArrayList
                        Dim Y As Integer

                        For Y = 0 To paramInfos.Length - 1
                            Dim paramDetails As New SortTable
                            paramDetails("Assembly") = paramInfos(y).GetType().Assembly.ToString()
                            paramDetails("ParamName") = paramInfos(y).Name
                            paramDetails("ParamType") = paramInfos(y).ParameterType.Name
                            If (paramInfos(y).ParameterType.IsArray And paramInfos(y).ParameterType.Name <> "Array") Or paramInfos(y).ParameterType.IsPointer Then
                                paramDetails("GetType") = paramInfos(y).ParameterType.GetElementType().Name
                                paramDetails("Namespace") = paramInfos(y).ParameterType.GetElementType().Namespace
                            Else
                                paramDetails("GetType") = paramInfos(y).ParameterType.Name
                                paramDetails("Namespace") = paramInfos(y).ParameterType.Namespace
                            End If
                            paramTable.Add(paramDetails)
                        Next
                        propertyDetails("Params") = paramTable
                    End If
                ElseIf Not propertyInfos(X).GetSetMethod() Is Nothing Then

                    propertyDetails("GetType") = propertyInfos(X).GetSetMethod().ReturnType.Name
                    propertyDetails("Namespace") = propertyInfos(X).GetSetMethod().ReturnType.Namespace

                    propertyDetails("Type") = propertyInfos(X).GetSetMethod().ReturnType.Name
                    propertyDetails("Assembly") = propertyInfos(X).GetSetMethod().ReturnType.Assembly.ToString()
                    propertyDetails("Name") = propertyInfos(X).Name

                    If propertyInfos(X).GetSetMethod().IsPublic Then
                        propertyDetails("Visibility") = "public"
                    ElseIf propertyInfos(X).GetSetMethod().IsPrivate Then
                        propertyDetails("Visibility") = "private"
                    ElseIf propertyInfos(X).GetSetMethod().IsFamily Then
                        propertyDetails("Visibility") = "protected"
                    End If

                    If propertyInfos(X).GetSetMethod().IsStatic Then
                        propertyDetails("Visibility") = CStr(propertyDetails("Visibility")) & " static"
                    End If

                    propertyDetails("Access") = "( Set )"
                    Dim paramInfos() As System.Reflection.ParameterInfo = propertyInfos(X).GetSetMethod().GetParameters()

                    If Not paramInfos Is Nothing Then
                        Dim paramTable As New ArrayList
                        Dim Y As Integer

                        For Y = 0 To paramInfos.Length - 1
                            Dim paramDetails As New SortTable
                            paramDetails("Assembly") = paramInfos(y).GetType().Assembly.ToString()
                            paramDetails("ParamName") = paramInfos(y).Name
                            paramDetails("ParamType") = paramInfos(y).ParameterType.Name
                            If (paramInfos(y).ParameterType.IsArray And paramInfos(y).ParameterType.Name <> "Array") Or paramInfos(y).ParameterType.IsPointer Then
                                paramDetails("GetType") = paramInfos(y).ParameterType.GetElementType().Name
                                paramDetails("Namespace") = paramInfos(y).ParameterType.GetElementType().Namespace
                            Else
                                paramDetails("GetType") = paramInfos(y).ParameterType.Name
                                paramDetails("Namespace") = paramInfos(y).ParameterType.Namespace
                            End If
                            paramTable.Add(paramDetails)
                        Next

                        propertyDetails("Params") = paramTable
                    End If
                End If
                Me.Add(propertyDetails)
            Next
            Me.Sort()
        End Sub
    End Class

    Public Class DisplayMethods : Inherits ArrayList
        Public Sub New(ByVal classType As Type, ByVal myclassname As String)
            Dim methodInfos() As System.Reflection.MethodInfo = classType.GetMethods()

            If methodInfos Is Nothing Then Return

            Dim X As Integer
            For X = 0 To methodInfos.Length - 1
                If String.Compare(myclassname, methodInfos(X).DeclaringType.Name, False, CultureInfo.InvariantCulture) = 0 And (methodInfos(X).IsPublic Or methodInfos(X).IsFamily) And Not methodInfos(X).IsSpecialName Then
                    Dim MethodDetails As New SortTable("Name")

                    MethodDetails("Assembly") = methodInfos(X).GetType().Assembly.ToString()
                    MethodDetails("Name") = methodInfos(X).Name
                    MethodDetails("Type") = methodInfos(X).ReturnType.Name
                    If (methodInfos(X).ReturnType.IsArray And methodInfos(X).ReturnType.Name <> "Array") Or methodInfos(X).ReturnType.IsPointer Then
                        Dim ReturnElementType As Type = methodInfos(X).ReturnType.GetElementType()
                        Do While ReturnElementType.IsArray
                            ReturnElementType = ReturnElementType.GetElementType()
                        Loop

                        MethodDetails("GetType") = ReturnElementType.Name
                        MethodDetails("Namespace") = ReturnElementType.Namespace
                    Else
                        MethodDetails("GetType") = methodInfos(X).ReturnType.Name
                        MethodDetails("Namespace") = methodInfos(X).ReturnType.Namespace
                    End If

                    If methodInfos(X).IsPublic Then
                        MethodDetails("Access") = "public "
                    ElseIf methodInfos(X).IsPrivate Then
                        MethodDetails("Access") = "private "
                    ElseIf methodInfos(X).IsFamily Then
                        MethodDetails("Access") = "protected "
                    End If

                    If methodInfos(X).IsStatic Then
                        MethodDetails("Access") = CStr(MethodDetails("Access")) & "static "
                    End If

                    Dim paramInfos() As System.Reflection.ParameterInfo = methodInfos(X).GetParameters()

                    If Not paramInfos Is Nothing Then
                        Dim paramTable As New ArrayList

                        Dim Y As Integer
                        For Y = 0 To paramInfos.Length - 1
                            Dim paramDetails As New SortTable
                            paramDetails("Assembly") = paramInfos(Y).GetType().Assembly.ToString()
                            paramDetails("ParamName") = paramInfos(Y).Name
                            paramDetails("ParamType") = paramInfos(Y).ParameterType.Name

                            If (paramInfos(Y).ParameterType.IsArray And paramInfos(Y).ParameterType.Name <> "Array") Or paramInfos(Y).ParameterType.IsPointer Then
                                paramDetails("GetType") = paramInfos(Y).ParameterType.GetElementType().Name
                                paramDetails("Namespace") = paramInfos(Y).ParameterType.GetElementType().Namespace
                            Else
                                paramDetails("GetType") = paramInfos(Y).ParameterType.Name
                                paramDetails("Namespace") = paramInfos(Y).ParameterType.Namespace
                            End If
                            paramTable.Add(paramDetails)
                        Next

                        MethodDetails("Params") = paramTable
                    End If

                    Me.Add(MethodDetails)
                End If
            Next
            Me.Sort()
        End Sub
    End Class

    Public Class DisplayInterfaces : Inherits ArrayList
        Public Sub New(ByVal classType As Type)
            Dim classInterfaces() As Type = classType.GetInterfaces()
            Dim X As Integer
            For X = 0 To classInterfaces.Length - 1

                Dim interfaceDetails As New SortTable

                interfaceDetails("Assembly") = classInterfaces(X).Assembly.ToString()
                interfaceDetails("FullName") = classInterfaces(X).FullName
                interfaceDetails("GetType") = classInterfaces(X).Name
                interfaceDetails("Namespace") = classInterfaces(X).Namespace
                Me.Add(interfaceDetails)
            Next
        End Sub
    End Class

    Public Class DisplaySuperclasses : Inherits ArrayList
        Public Sub New(ByVal classType As Type)
            Dim SuperClass As Type
            Dim classDetails As New SortTable

            classDetails("Assembly") = classType.Assembly.ToString()
            classDetails("FullName") = classType.FullName
            classDetails("GetType") = classType.Name
            classDetails("Namespace") = classType.Namespace

            Me.Add(classDetails)

            Do While Not classType.BaseType Is Nothing
                Dim superclassDetails As New SortTable
                SuperClass = classType.BaseType
                classType = SuperClass

                superclassDetails("Assembly") = SuperClass.Assembly.ToString()
                superclassDetails("FullName") = SuperClass.FullName
                superclassDetails("GetType") = SuperClass.Name
                superclassDetails("Namespace") = SuperClass.Namespace

                Me.Add(superclassDetails)
            Loop
            Me.Reverse()
        End Sub
    End Class

    Public Class DisplaySubClasses : Inherits ArrayList
        Private classType As Type
        Private CorRuntime() As System.Reflection.Module
        Private CorClasses() As Type
        Private myclassname As String
        Private classInterfaces() As Type

        Public Sub New(ByVal classType As Type, ByVal ModuleName As ArrayList)
            Me.classType = classType
            myclassname = classType.FullName
            If classType.IsInterface Then
                Dim Y As Integer
                For Y = 0 To ModuleName.Count - 1
                    CorRuntime = System.Reflection.Assembly.Load(ModuleName(y).ToString()).GetModules()
                    CorClasses = CorRuntime(0).GetTypes()

                    Dim X As Integer
                    For X = 0 To CorClasses.Length - 1
                        classType = CorClasses(x)
                        classInterfaces = classType.GetInterfaces()

                        Dim I As Integer
                        For I = 0 To classInterfaces.Length - 1

                            If String.Compare(myclassname, classInterfaces(I).FullName, False, CultureInfo.InvariantCulture) = 0 Then
                                Dim subclassDetails As New SortTable("FullName")
                                subclassDetails("Assembly") = classType.Assembly.ToString()
                                subclassDetails("FullName") = classType.FullName
                                subclassDetails("GetType") = classType.Name
                                subclassDetails("Namespace") = classType.Namespace
                                Me.Add(subclassDetails)
                            End If
                        Next
                    Next
                Next
            Else
                Dim Y As Integer
                For Y = 0 To ModuleName.Count - 1
                    CorRuntime = System.Reflection.Assembly.Load(ModuleName(y).ToString()).GetModules()
                    CorClasses = CorRuntime(0).GetTypes()

                    Dim X As Integer
                    For X = 0 To CorClasses.Length - 1
                        classType = CorClasses(x).BaseType

                        If Not classType Is Nothing Then
                            If String.Compare(classType.FullName, myclassname, False, CultureInfo.InvariantCulture) = 0 Then
                                Dim subclassDetails As New SortTable("FullName")
                                subclassDetails("Assembly") = CorClasses(x).Assembly.ToString()
                                subclassDetails("FullName") = CorClasses(x).FullName
                                subclassDetails("GetType") = CorClasses(x).Name
                                subclassDetails("Namespace") = CorClasses(x).Namespace
                                Me.Add(subclassDetails)
                            End If
                        End If
                    Next
                Next
            End If
            Me.Sort()
        End Sub
    End Class
#End Region

End Namespace
#End If
