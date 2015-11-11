Option Explicit On 
Option Strict On

Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports CompuMaster.camm.WebManager.Modules.Charts.NetChart

Namespace CompuMaster.camm.WebManager.Modules.Charts.Pages

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Methods for creation of charts
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	04.02.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ImageCreation

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Bar graphs or pie charts
        ''' </summary>
        ''' <param name="imageWidth">The width of the image which shall be created</param>
        ''' <param name="imageHeight">The height of the image which shall be created</param>
        ''' <param name="chartType">A string with either "bar" or "pie"</param>
        ''' <param name="xValues">Values for the X coordinate, separated by a "|"</param>
        ''' <param name="yValues">Values for the Y coordinate, separated by a "|"</param>
        ''' <param name="xUnit">Unit for the X achse</param>
        ''' <param name="yUnit">Unit for the Y achse</param>
        ''' <param name="backgroundColor">The background color for the image</param>
        ''' <param name="numberFormat">The formatting style for numbers</param>
        ''' <param name="legend">Show the legend?</param>
        ''' <param name="ingraphValues">Write values into the graph?</param>
        ''' <param name="drawTotals">Calculate and draw the totals (the summary of the values)</param>
        ''' <param name="noShortLabels">Prevents the chart engine from creating abbreviations</param>
        ''' <returns>A memory stream which contains the image data in PNG file format</returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	04.02.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function SimpleBarOrPieChart(ByVal imageWidth As Integer, ByVal imageHeight As Integer, ByVal chartType As String, ByVal xValues As String, ByVal yValues As String, ByVal xUnit As String, ByVal yUnit As String, ByVal backgroundColor As Color, ByVal numberFormat As String, ByVal legend As Boolean, ByVal ingraphValues As Boolean, ByVal drawTotals As Boolean, ByVal noShortLabels As Boolean) As MemoryStream

            'Create the graph
            Dim memStream As New MemoryStream
            If Not (xValues Is Nothing) And Not (yValues Is Nothing) Then

                Dim StockBitMap As Bitmap

                Select Case chartType
                    Case "bar"
                        Dim bar As New BarGraph(backgroundColor)

                        bar.VerticalLabel = yUnit
                        bar.VerticalTickCount = 5
                        bar.ShowLegend = legend
                        bar.ShowData = ingraphValues
                        bar.Height = imageHeight
                        bar.Width = imageWidth
                        bar.CreateShortLabels = Not noShortLabels

                        bar.CollectDataPoints(xValues.Split("|".ToCharArray()), yValues.Split("|".ToCharArray()))
                        StockBitMap = bar.Draw()
                    Case Else
                        Dim pc As New PieChart(backgroundColor)

                        pc.CollectDataPoints(xValues.Split("|".ToCharArray()), yValues.Split("|".ToCharArray()))
                        pc.UnitY = yUnit
                        If Not numberFormat Is Nothing Then
                            pc.NumberFormat = numberFormat
                        End If
                        pc.DrawTotal = drawTotals
                        pc.Height = imageHeight
                        pc.Width = imageWidth

                        StockBitMap = pc.Draw()
                End Select

                ' Render BitMap Stream Back To Client
                StockBitMap.Save(memStream, ImageFormat.Png)
            End If
            Return memStream

        End Function

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Charts.Pages.ChartGenerator
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A chart generator which is based on System.Web.UI.Page
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	04.02.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public Class ChartGenerator
        Inherits System.Web.UI.Page

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Dim RequestParameters As ChartParameters = Utils.EnvironmentParameters(Request)

            Dim ImageData As MemoryStream = Nothing
            Try
                ImageData = ImageCreation.SimpleBarOrPieChart(RequestParameters.Width, RequestParameters.Height, RequestParameters.ChartType, RequestParameters.XValues, RequestParameters.YValues, RequestParameters.XUnit, RequestParameters.YUnit, RequestParameters.BgColor, RequestParameters.NumberFormat, RequestParameters.Legend, RequestParameters.IngraphValue, RequestParameters.DrawTotal, RequestParameters.NoShortLabels)
                ' set return type to png image format
                Response.ContentType = "image/png"
                ImageData.WriteTo(Response.OutputStream)
            Finally
                If Not ImageData Is Nothing Then
                    ImageData.Close()
                End If
            End Try

        End Sub

    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Charts.Pages.ChartGenerator2
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     A performant chart generator which is based on camm Web-Manager's download handler to deliver the created images
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	04.02.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    <System.Runtime.InteropServices.ComVisible(False)> Public Class ChartGenerator2
        Inherits CompuMaster.camm.WebManager.Pages.Page

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

            Dim RequestParameters As ChartParameters = Utils.EnvironmentParameters(Request)

            'Prepare a unique filename for all given parameters
            Dim fileName As String = CompuMaster.camm.WebManager.Utils.JoinNameValueCollectionToString(Request.QueryString, "&", "=", "")
            fileName &= ".png"
            fileName = CompuMaster.camm.WebManager.DownloadHandler.ComputeHashedFilenameFromLongFilename(CompuMaster.camm.WebManager.DownloadHandler.FileSystemCompatibleString(fileName))

            If Not Me.cammWebManager.DownloadHandler.DownloadFileAlreadyExists(DownloadHandler.DownloadLocations.PublicCache, "webmanager", fileName) Then
                Dim ImageData As MemoryStream = Nothing
                Try
                    ImageData = ImageCreation.SimpleBarOrPieChart(RequestParameters.Width, RequestParameters.Height, RequestParameters.ChartType, RequestParameters.XValues, RequestParameters.YValues, RequestParameters.XUnit, RequestParameters.YUnit, RequestParameters.BgColor, RequestParameters.NumberFormat, RequestParameters.Legend, RequestParameters.IngraphValue, RequestParameters.DrawTotal, RequestParameters.NoShortLabels)
                    'Use downloadhanlder to send the file data - use public cache
                    Dim ImageRawFile As New CompuMaster.camm.WebManager.DownloadHandler.RawDataSingleFile

                    ImageRawFile.Filename = fileName
                    ImageRawFile.Data = ImageData.GetBuffer
                    ImageRawFile.MimeType = "image/png"
                    Me.cammWebManager.DownloadHandler.Add(ImageRawFile, "charts")
                    Me.cammWebManager.DownloadHandler.ProcessDownload(DownloadHandler.DownloadLocations.PublicCache, "webmanager")

                    ' set return type to png image format
                    'Response.ContentType = "image/png"
                    'ImageData.WriteTo(Response.OutputStream)
                Finally
                    If Not ImageData Is Nothing Then
                        ImageData.Close()
                    End If
                End Try
            Else
                CompuMaster.camm.WebManager.Utils.RedirectTemporary(Me.Context, Me.cammWebManager.DownloadHandler.CreateDownloadLink(DownloadHandler.DownloadLocations.PublicCache, "webmanager", fileName))
            End If

        End Sub

    End Class

    ''' <summary>
    ''' A class representing the parameters for the current request for creation of the desired chart
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class ChartParameters
        Public XValues, YValues, ChartType, XUnit, YUnit As String
        Public NumberFormat As String = Nothing
        Public BgColor As Color = Color.White
        Public Legend, IngraphValue As Boolean
        Public Height As Integer = 300
        Public Width As Integer = 300
        Public DrawTotal As Boolean = True
        Public NoShortLabels As Boolean
    End Class

    ''' <summary>
    ''' Charting utility class
    ''' </summary>
    ''' <remarks></remarks>
    Friend Class Utils

        ''' <summary>
        ''' Lookup the values from the parameters collection of the current request
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function EnvironmentParameters(ByVal request As System.Web.HttpRequest) As ChartParameters
            Dim Result As New ChartParameters
            ' Get input parameters from query string
            Result.ChartType = request.QueryString("chartType")
            Result.XValues = request.QueryString("xValues")
            Result.YValues = request.QueryString("yValues")
            Result.XUnit = request.QueryString("xUnit")
            Result.YUnit = request.QueryString("yUnit")
            If Not request.QueryString("noshortlabels") Is Nothing Then
                Try
                    Result.NoShortLabels = CType(request.QueryString("noshortlabels"), Boolean)
                Catch
                End Try
            End If
            If request.QueryString("bgcolor") <> "" Then
                Try
                    Result.BgColor = System.Drawing.ColorTranslator.FromHtml("#" & request.QueryString("bgcolor"))
                Catch
                    Result.BgColor = Color.White
                End Try
            End If
            If Not request.QueryString("numberformat") Is Nothing Then
                Try
                    Result.NumberFormat = request.QueryString("numberformat")
                Catch
                End Try
            End If
            If request.QueryString("height") <> "" Then
                Try
                    Result.Height = CType(request.QueryString("height"), Integer)
                Catch
                End Try
            End If
            If request.QueryString("width") <> "" Then
                Try
                    Result.Width = CType(request.QueryString("width"), Integer)
                Catch
                End Try
            End If
            If request.QueryString("legend") <> "" Then
                Try
                    Result.Legend = Convert.ToBoolean(request.QueryString("legend"))
                Catch
                End Try
            End If
            If Not request.QueryString("drawtotal") Is Nothing Then
                Try
                    Result.DrawTotal = Convert.ToBoolean(request.QueryString("drawtotal"))
                Catch
                End Try
            End If
            If request.QueryString("ingraphvalues") <> "" Then
                Try
                    Result.IngraphValue = Convert.ToBoolean(request.QueryString("ingraphvalues"))
                Catch
                End Try
            End If
            Return Result
        End Function

    End Class

End Namespace
