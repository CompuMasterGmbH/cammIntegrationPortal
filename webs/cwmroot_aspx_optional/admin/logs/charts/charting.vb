Option Explicit On 
Option Strict On

Imports System
Imports System.Drawing
Imports System.Collections
Imports System.Diagnostics
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D

'egineered on base of ASPNetStarterKit.Charting
Namespace CompuMaster.camm.WebManager.Modules.Charts.NetChart

#Region "Implemented chart styles"

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Charts.NetChart.PieChart
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     PieChart Class
    ''' </summary>
    ''' <remarks>
    '''     This class uses GDI+ to render Bar Chart.
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	30.03.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PieChart
        Inherits Chart
        Private _chartItems As ArrayList = New ArrayList
        Private _backgroundColor As Color = Color.White
        Private _borderColor As Color = Color.FromArgb(63, 63, 63)
        Private _total As Double
        Private _legendWidth As Integer
        Private _legendHeight As Integer
        Private _legendFontHeight As Integer
        Private _legendFontStyle As String = "Verdana"
        Private _UnitY As String
        Private _NumberFormat As String = "#,##0"
        Private _DrawTotal As Boolean = True
        Private _height As Integer = 250
        Private _width As Integer = 625
        Private _perimeter As Integer = CType(_height * 0.97, Integer)
        Private _bufferSpace As Integer
        Private _legendFontSize As Integer
        Private _AutoSizing As Boolean = True
        Private _graphLegendSpacer As Integer
        Dim LegendSquareSize As Integer
        Dim LegendPadding As Integer
        Dim _legendYStart As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal bgColor As Color)
            _backgroundColor = bgColor
        End Sub

        Public Property UnitY() As String
            Get
                Return _UnitY
            End Get
            Set(ByVal Value As String)
                _UnitY = Value
            End Set
        End Property
        Public Property DrawTotal() As Boolean
            Get
                Return _DrawTotal
            End Get
            Set(ByVal Value As Boolean)
                _DrawTotal = Value
            End Set
        End Property
        Public Property Height() As Integer
            Get
                Return _height
            End Get
            Set(ByVal Value As Integer)
                _height = Value
            End Set
        End Property
        Public Property Width() As Integer
            Get
                Return _width
            End Get
            Set(ByVal Value As Integer)
                _width = Value
            End Set
        End Property
        Public Property NumberFormat() As String
            Get
                Return _NumberFormat
            End Get
            Set(ByVal Value As String)
                _NumberFormat = Value
            End Set
        End Property
        Public Property BackgroundColor() As Color
            Get
                Return _backgroundColor
            End Get
            Set(ByVal Value As Color)
                _backgroundColor = Value
            End Set
        End Property
        Public Property AutoSizing() As Boolean
            Get
                Return _AutoSizing
            End Get
            Set(ByVal Value As Boolean)
                _AutoSizing = Value
            End Set
        End Property
        Public Property LegendFontSize() As Integer
            Get
                Return _legendFontSize
            End Get
            Set(ByVal Value As Integer)
                _legendFontSize = Value
            End Set
        End Property
        Public Property LegendFontStyle() As String
            Get
                Return _legendFontStyle
            End Get
            Set(ByVal Value As String)
                _legendFontStyle = Value
            End Set
        End Property
        Public Property BorderSpacing() As Integer
            Get
                Return _graphLegendSpacer
            End Get
            Set(ByVal Value As Integer)
                _graphLegendSpacer = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method collects all data points and calculate all the necessary dimensions to draw the chart.  It is the first method called before invoking the Draw() method.
        ''' </summary>
        ''' <param name="xValues"></param>
        ''' <param name="yValues"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub CollectDataPoints(ByVal xValues() As String, ByVal yValues() As String)
            _total = 0.0F

            Dim i As Integer
            For i = 0 To xValues.Length - 1
                Dim ftemp As Single = CType(Val(yValues(i)), Single)
                _chartItems.Add(New ChartItem(xValues(i), xValues.ToString(), ftemp, 0, 0, Color.AliceBlue))
                _total += ftemp
            Next i

            Dim nextStartPos As Single = 0.0F
            Dim counter As Integer = 0
            Dim item As ChartItem
            For Each item In _chartItems
                item.StartPos = nextStartPos
                item.SweepSize = CType(item.Value / _total * 360, Single)
                nextStartPos = item.StartPos + item.SweepSize
                counter = counter + 1
                item.ItemColor = GetColor(counter)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method returns a bitmap to the calling function.  This is the method that actually draws the pie chart and the legend with it.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function Draw() As Bitmap

            CalculateLegendWidthHeight()

            Dim perimeter As Integer = _perimeter
            Dim pieRect As New Rectangle(3, CType((_height - perimeter) / 2, Integer), perimeter, perimeter)
            'Dim bmp As New Bitmap(perimeter + _bufferSpace * 0 + _legendWidth, perimeter)
            Dim bmp As New Bitmap(Width, Height)
            Dim grp As Graphics = Graphics.FromImage(bmp)

            'Paint Back ground
            grp.FillRectangle(New SolidBrush(_backgroundColor), 0, 0, _width, _height)

            'Align text to the right
            Dim sf As New StringFormat
            sf.Alignment = StringAlignment.Far

            'Calculate additional values
            Dim fontLegend As New Font(_legendFontStyle, _legendFontSize, FontStyle.Regular, GraphicsUnit.Pixel)
            Dim fontLegendBold As New Font(_legendFontStyle, _legendFontSize, FontStyle.Bold, GraphicsUnit.Pixel)

            'Draw all wedges and legends
            Dim i As Integer
            For i = 0 To _chartItems.Count - 1
                Dim item As ChartItem = CType(_chartItems(i), ChartItem)
                Dim brs As New SolidBrush(item.ItemColor)
                grp.FillPie(brs, pieRect, item.StartPos, item.SweepSize)
                grp.FillRectangle(brs, perimeter + _bufferSpace, _legendYStart + i * _legendFontHeight, LegendSquareSize, LegendSquareSize)
                grp.DrawString(item.Label, fontLegend, New SolidBrush(Color.Black), CType(perimeter + _bufferSpace + LegendSquareSize, Single), CType(_legendYStart + (i + 0.16) * _legendFontHeight, Single))
                grp.DrawString(item.Value.ToString(_NumberFormat) & CType(IIf(UnitY <> "", " " & UnitY, ""), String), fontLegend, New SolidBrush(Color.Black), perimeter + _bufferSpace + _legendWidth, CType(_legendYStart + (i + 0.16) * _legendFontHeight, Single), sf)
            Next i

            'draws the border around Pie
            grp.DrawEllipse(New Pen(_borderColor, 2), pieRect)

            'draw border around legend
            grp.DrawRectangle(New Pen(_borderColor, 1), perimeter + _bufferSpace - LegendPadding, _legendYStart - LegendPadding, _legendWidth + 2 * LegendPadding, _legendHeight + CInt(1.5 * LegendPadding))

            'Draw Total under legend
            If _DrawTotal Then
                grp.DrawString("Total", fontLegendBold, New SolidBrush(Color.Black), CType(perimeter + _bufferSpace + LegendSquareSize, Single), CType(_legendYStart + (_chartItems.Count + 0.66) * _legendFontHeight, Single))
                grp.DrawString(_total.ToString(_NumberFormat) & CType(IIf(UnitY <> "", " " & UnitY, ""), String), fontLegendBold, New SolidBrush(Color.Black), perimeter + _bufferSpace + _legendWidth, CType(_legendYStart + (_chartItems.Count + 0.66) * _legendFontHeight, Single), sf)
            End If

            grp.SmoothingMode = SmoothingMode.AntiAlias
            grp.Dispose()
            Return bmp
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method calculates the space required to draw the chart legend.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CalculateLegendWidthHeight()

            'Calculate additional private variables
            _perimeter = CType(System.Math.Min(_height * 0.97, _width / 2), Integer)
            _bufferSpace = CType(_perimeter * 0.25, Integer)
            _legendFontSize = CType(_perimeter / 20, Integer)
            _graphLegendSpacer = CType(_perimeter / 20, Integer)
            LegendPadding = CType(_graphLegendSpacer / 2, Integer)
            Dim fontLegend As New Font(_legendFontStyle, _legendFontSize, FontStyle.Regular, GraphicsUnit.Pixel)
            _legendFontHeight = CType(fontLegend.Height * 1.33, Integer) '+33%
            LegendSquareSize = CInt((fontLegend.Height + _legendFontHeight) / 2)
            _legendHeight = _legendFontHeight * _chartItems.Count
            '_legendHeight += _legendFontHeight - fontLegend.Height 'make it only nice at the bottom by adding only a few lines of pixels
            If _DrawTotal Then
                'preserve space for totals
                _legendHeight += CType(_legendFontHeight * 1.5, Integer)
            End If
            _legendWidth = CType(_width - (_perimeter + _bufferSpace + _graphLegendSpacer), Integer)
            _legendYStart = CType((_height - _legendHeight) / 2, Integer)

            If _AutoSizing Then
                'Do automatic sizing
                If _legendHeight > _perimeter Then
                    _perimeter = _legendHeight
                End If
            Else
                'Manual sizing of font sizes, etc.
            End If
        End Sub
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Charts.NetChart.BarGraph
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     BarGraph Class
    ''' </summary>
    ''' <remarks>
    '''     This class uses GDI+ to render Bar Chart.
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	30.03.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class BarGraph
        Inherits Chart
        Private _graphLegendSpacer As Single = 15.0F
        Private _labelFontSize As Integer = 7
        Private _legendFontSize As Integer = 9
        Private _legendRectangleSize As Single = 10.0F
        Private _spacer As Single = 5.0F

        ' Overall related members
        Private _backColor As Color
        Private _fontFamily As String
        Private _longestTickValue As String = String.Empty ' Used to calculate max value width
        Private _maxTickValueWidth As Single ' Used to calculate left offset of bar graph
        Private _totalHeight As Single
        Private _totalWidth As Single

        ' Graph related members
        Private _barWidth As Single
        Private _bottomBuffer As Single ' Space from bottom to x axis
        Private _displayBarData As Boolean
        Private _fontColor As Color
        Private _graphHeight As Single
        Private _graphWidth As Single
        Private _maxValue As Single = 0.0F ' = final tick value * tick count
        Private _scaleFactor As Single ' = _maxValue / _graphHeight
        Private _spaceBtwBars As Single ' For now same as _barWidth
        Private _topBuffer As Single ' Space from top to the top of y axis
        Private _xOrigin As Single ' x position where graph starts drawing
        Private _yOrigin As Single ' y position where graph starts drawing
        Private _yLabel As String
        Private _yTickCount As Integer
        Private _yTickValue As Double ' Value for each tick = _maxValue/_yTickCount
        ' Legend related members
        Private _displayLegend As Boolean
        Private _legendWidth As Single
        Private _longestLabel As String = String.Empty ' Used to calculate legend width
        Private _maxLabelWidth As Single = 0.0F


        Public Property FontFamily() As String
            Get
                Return _fontFamily
            End Get
            Set(ByVal Value As String)
                _fontFamily = Value
            End Set
        End Property

        Public WriteOnly Property BackgroundColor() As Color
            Set(ByVal Value As Color)
                _backColor = Value
            End Set
        End Property

        Public WriteOnly Property BottomBuffer() As Integer
            Set(ByVal Value As Integer)
                _bottomBuffer = Convert.ToSingle(Value)
            End Set
        End Property

        Public WriteOnly Property FontColor() As Color
            Set(ByVal Value As Color)
                _fontColor = Value
            End Set
        End Property

        Public Property Height() As Integer
            Get
                Return Convert.ToInt32(_totalHeight)
            End Get
            Set(ByVal Value As Integer)
                _totalHeight = Convert.ToSingle(Value)
            End Set
        End Property

        Public Property Width() As Integer
            Get
                Return Convert.ToInt32(_totalWidth)
            End Get
            Set(ByVal Value As Integer)
                _totalWidth = Convert.ToSingle(Value)
            End Set
        End Property

        Public Property ShowLegend() As Boolean
            Get
                Return _displayLegend
            End Get
            Set(ByVal Value As Boolean)
                _displayLegend = Value
            End Set
        End Property

        Public Property ShowData() As Boolean
            Get
                Return _displayBarData
            End Get
            Set(ByVal Value As Boolean)
                _displayBarData = Value
            End Set
        End Property

        Public WriteOnly Property TopBuffer() As Integer
            Set(ByVal Value As Integer)
                _topBuffer = Convert.ToSingle(Value)
            End Set
        End Property

        Public Property VerticalLabel() As String
            Get
                Return _yLabel
            End Get
            Set(ByVal Value As String)
                _yLabel = Value
            End Set
        End Property

        Public Property VerticalTickCount() As Integer
            Get
                Return _yTickCount
            End Get
            Set(ByVal Value As Integer)
                _yTickCount = Value
            End Set
        End Property

        Public Sub New()
            AssignDefaultSettings()
        End Sub

        Public Sub New(ByVal bgColor As Color)
            AssignDefaultSettings()
            BackgroundColor = bgColor
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method collects all data points and calculate all the necessary dimensions to draw the bar graph.  It is the method called before invoking the Draw() method. 
        ''' </summary>
        ''' <param name="labels">x values</param>
        ''' <param name="values">y values</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Sub CollectDataPoints(ByVal labels() As String, ByVal values() As String)
            If labels.Length = values.Length Then
                Dim i As Integer
                For i = 0 To labels.Length - 1
                    'Dim temp As Single = Convert.ToSingle(values(i))
                    Dim temp As Single = CType(Val(values(i)), Single)
                    Dim shortLbl As String
                    If CreateShortLabels Then
                        shortLbl = MakeShortLabel(labels(i))
                    Else
                        shortLbl = labels(i)
                    End If

                    ' For now put 0.0 for start position and sweep size
                    DataPoints.Add(New ChartItem(shortLbl, labels(i), temp, 0.0F, 0.0F, GetColor(i + 1)))

                    ' Find max value from data; this is only temporary _maxValue
                    If _maxValue < temp Then
                        _maxValue = temp
                    End If
                    ' Find the longest description
                    If _displayLegend Then
                        Dim currentLbl As String
                        If CreateShortLabels Then
                            currentLbl = labels(i) + " (" + shortLbl + ")"
                        Else
                            currentLbl = labels(i)
                        End If

                        Dim currentWidth As Single = CalculateImgFontWidth(currentLbl, _legendFontSize, FontFamily)
                        If _maxLabelWidth < currentWidth Then
                            _longestLabel = currentLbl
                            _maxLabelWidth = currentWidth
                        End If
                    End If
                Next i

            Else
                Throw New Exception("X data count is different from Y data count")
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method collects all data points and calculate all the necessary dimensions to draw the bar graph.  It is the method called before invoking the Draw() method. 
        ''' </summary>
        ''' <param name="values">y values</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overloads Sub CollectDataPoints(ByVal values() As String)
            Dim labels As String() = values
            CollectDataPoints(labels, values)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method returns a bar graph bitmap to the calling function.  It is called after all dimensions and data points are calculated.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Function Draw() As Bitmap

            'Handle the border spacing
            If Me.VerticalLabel <> "" Then
                _topBuffer += 15
            End If
            _bottomBuffer += 15

            'Calculate the variables
            CalculateTickAndMax()
            CalculateGraphDimension()
            CalculateBarWidth(DataPoints.Count, _graphWidth)
            CalculateSweepValues()

            Dim height As Integer = Convert.ToInt32(_totalHeight)
            Dim width As Integer = Convert.ToInt32(_totalWidth)

            Dim bmp As New Bitmap(width, height)
            Dim graph As Graphics = Graphics.FromImage(bmp)
            graph.CompositingQuality = CompositingQuality.HighQuality
            graph.SmoothingMode = SmoothingMode.AntiAlias

            ' Set the background: need to draw one pixel larger than the bitmap to cover all area
            graph.FillRectangle(New SolidBrush(_backColor), -1, -1, bmp.Width + 1, bmp.Height + 1)

            DrawVerticalLabelArea(graph)
            DrawBars(graph)
            DrawXLabelArea(graph)
            If _displayLegend Then
                DrawLegend(graph)
            End If
            graph.Dispose()

            Return bmp
        End Function


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method draws all the bars for the graph.
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DrawBars(ByVal graph As Graphics)
            Dim brsFont As New SolidBrush(_fontColor)
            Dim valFont As New Font(_fontFamily, _labelFontSize)
            Dim sfFormat As New StringFormat
            sfFormat.Alignment = StringAlignment.Center
            Dim i As Integer = 0

            ' Draw bars and the value above each bar
            Dim item As ChartItem
            For Each item In DataPoints
                Dim barBrush As New SolidBrush(item.ItemColor)
                Dim itemY As Single = _yOrigin + _graphHeight - item.SweepSize

                ' When drawing, all position is relative to (_xOrigin, _yOrigin)
                graph.FillRectangle(barBrush, _xOrigin + item.StartPos, itemY, _barWidth, item.SweepSize)

                ' Draw data value
                If _displayBarData Then
                    Dim startX As Single = _xOrigin + i * (_barWidth + _spaceBtwBars) ' This draws the value on center of the bar
                    Dim startY As Single = itemY - 2.0F - valFont.Height ' Positioned on top of each bar by 2 pixels
                    Dim recVal As New RectangleF(startX, startY, _barWidth + _spaceBtwBars, valFont.Height)
                    graph.DrawString(item.Value.ToString("#,###.##"), valFont, brsFont, recVal, sfFormat)
                End If
                i += 1
            Next item
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method draws the y label, tick marks, tick values, and the y axis.
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DrawVerticalLabelArea(ByVal graph As Graphics)
            Dim lblFont As New Font(_fontFamily, _labelFontSize)
            Dim brs As New SolidBrush(_fontColor)
            Dim lblFormat As New StringFormat
            lblFormat.Alignment = StringAlignment.Near
            Dim pen As New pen(_fontColor)

            ' Draw vertical label at the top of y-axis and place it in the middle top of y-axis
            Dim recVLabel As New RectangleF(0.0F, _yOrigin - 2 * _spacer - lblFont.Height, _xOrigin * 2, lblFont.Height)
            Dim sfVLabel As New StringFormat
            sfVLabel.Alignment = StringAlignment.Center
            graph.DrawString(_yLabel, lblFont, brs, recVLabel, sfVLabel)

            ' Draw all tick values and tick marks
            Dim i As Integer
            For i = 0 To _yTickCount - 1
                Dim currentY As Single = CType(_topBuffer + i * _yTickValue / _scaleFactor, Single) ' Position for tick mark
                Dim labelY As Single = CType(currentY - lblFont.Height / 2, Single) ' Place label in the middle of tick
                Dim lblRec As New RectangleF(_spacer, labelY, _maxTickValueWidth, lblFont.Height)
                Dim currentTick As Single = CType(_maxValue - i * _yTickValue, Single) ' Calculate tick value from top to bottom
                graph.DrawString(currentTick.ToString("#,###.##"), lblFont, brs, lblRec, lblFormat) ' Draw tick value  
                graph.DrawLine(pen, _xOrigin, currentY, _xOrigin - 4.0F, currentY) ' Draw tick mark
            Next i

            ' Draw y axis
            graph.DrawLine(pen, _xOrigin, _yOrigin, _xOrigin, _yOrigin + _graphHeight)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method draws x axis and all x labels
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DrawXLabelArea(ByVal graph As Graphics)
            Dim lblFont As New Font(_fontFamily, _labelFontSize)
            Dim brs As New SolidBrush(_fontColor)
            Dim lblFormat As New StringFormat
            lblFormat.Alignment = StringAlignment.Center
            Dim pen As New pen(_fontColor)

            ' Draw x axis
            graph.DrawLine(pen, _xOrigin, _yOrigin + _graphHeight, _xOrigin + _graphWidth, _yOrigin + _graphHeight)

            Dim currentX As Single
            Dim currentY As Single = _yOrigin + _graphHeight + 2.0F ' All x labels are drawn 2 pixels below x-axis
            Dim labelWidth As Single = _barWidth + _spaceBtwBars ' Fits exactly below the bar
            Dim i As Integer = 0

            ' Draw x labels
            Dim item As ChartItem
            For Each item In DataPoints
                currentX = _xOrigin + i * labelWidth
                Dim recLbl As New RectangleF(currentX, currentY, labelWidth, lblFont.Height)
                Dim lblString As String
                If _displayLegend Then
                    lblString = item.Label
                Else
                    lblString = item.Description
                End If
                graph.DrawString(lblString, lblFont, brs, recLbl, lblFormat)
                i += 1
            Next item
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method determines where to place the legend box. It draws the legend border, legend description, and legend color code.
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DrawLegend(ByVal graph As Graphics)
            Dim lblFont As New Font(_fontFamily, _legendFontSize)
            Dim brs As New SolidBrush(_fontColor)
            Dim lblFormat As New StringFormat
            lblFormat.Alignment = StringAlignment.Near
            Dim pen As New pen(_fontColor)

            ' Calculate Legend drawing start point
            Dim startX As Single = _xOrigin + _graphWidth + _graphLegendSpacer
            Dim startY As Single = _yOrigin

            Dim xColorCode As Single = startX + _spacer
            Dim xLegendText As Single = xColorCode + _legendRectangleSize + _spacer
            Dim legendHeight As Single = 0.0F
            Dim i As Integer
            For i = 0 To DataPoints.Count - 1
                Dim point As ChartItem = DataPoints(i)
                Dim [text] As String
                If CreateShortLabels Then
                    [text] = point.Description + " (" + point.Label + ")"
                Else
                    [text] = point.Description
                End If
                Dim currentY As Single = startY + _spacer + i * (lblFont.Height + _spacer)
                legendHeight += lblFont.Height + _spacer

                ' Draw legend description
                graph.DrawString([text], lblFont, brs, xLegendText, currentY, lblFormat)

                ' Draw color code
                graph.FillRectangle(New SolidBrush(DataPoints(i).ItemColor), xColorCode, currentY + 3.0F, _legendRectangleSize, _legendRectangleSize)
            Next i

            ' Draw legend border
            graph.DrawRectangle(pen, startX, startY, _legendWidth, legendHeight + _spacer)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method calculates all measurement aspects of the bar graph from the given data points
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CalculateGraphDimension()
            FindLongestTickValue()

            ' Need to add another character for spacing; this is not used for drawing, just for calculation
            _longestTickValue += "0"
            _maxTickValueWidth = CalculateImgFontWidth(_longestTickValue, _labelFontSize, FontFamily)
            Dim leftOffset As Single = _spacer + _maxTickValueWidth
            Dim rtOffset As Single = 0.0F

            If _displayLegend Then
                _legendWidth = _spacer + _legendRectangleSize + _spacer + _maxLabelWidth + _spacer
                rtOffset = _graphLegendSpacer + _legendWidth + _spacer
            Else
                rtOffset = _spacer ' Make graph in the middle
            End If
            _graphHeight = _totalHeight - _topBuffer - _bottomBuffer ' Buffer spaces are used to print labels
            _graphWidth = _totalWidth - leftOffset - rtOffset
            _xOrigin = leftOffset
            _yOrigin = _topBuffer

            ' Once the correct _maxValue is determined, then calculate _scaleFactor
            _scaleFactor = _maxValue / _graphHeight
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method determines the longest tick value from the given data points. The result is needed to calculate the correct graph dimension.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub FindLongestTickValue()
            Dim currentTick As Single
            Dim tickString As String
            Dim i As Integer
            For i = 0 To _yTickCount - 1
                currentTick = CType(_maxValue - i * _yTickValue, Single)
                tickString = currentTick.ToString("#,###.##")
                If _longestTickValue.Length < tickString.Length Then
                    _longestTickValue = tickString
                End If
            Next i
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method calculates the image width in pixel for a given text
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="size"></param>
        ''' <param name="family"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CalculateImgFontWidth(ByVal [text] As String, ByVal size As Integer, ByVal family As String) As Single
            Dim bmp As Bitmap = Nothing
            Dim graph As Graphics = Nothing
            Dim font As New font(family, size)

            ' Calculate the size of the string.
            bmp = New Bitmap(1, 1, PixelFormat.Format32bppArgb)
            graph = Graphics.FromImage(bmp)
            Dim oSize As SizeF = graph.MeasureString([text], font)

            graph.Dispose()
            bmp.Dispose()

            Return oSize.Width
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method creates abbreviation from long description; used for making legend
        ''' </summary>
        ''' <param name="text"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function MakeShortLabel(ByVal [text] As String) As String
            Dim label As String = [text]
            If [text].Length > 2 Then
                Dim midPostition As Integer = Convert.ToInt32(Math.Floor(([text].Length / 2)))
                label = [text].Substring(0, 1) + [text].Substring(midPostition, 1) + [text].Substring([text].Length - 1, 1)
            End If
            Return label
        End Function

        Private _CreateShortLabels As Boolean = True
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Create short labels (default = true)
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property CreateShortLabels() As Boolean
            Get
                Return _CreateShortLabels
            End Get
            Set(ByVal Value As Boolean)
                _CreateShortLabels = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method calculates the max value and each tick mark value for the bar graph.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CalculateTickAndMax()
            Dim tempMax As Double = 0.0F

            ' Give graph some head room first about 10% of current max
            _maxValue *= 1.1F

            If _maxValue <> 0.0F Then
                ' Find a rounded value nearest to the current max value
                ' Calculate this max first to give enough space to draw value on each bar
                Dim exp As Double = Convert.ToDouble(Math.Floor(Math.Log10(_maxValue)))
                tempMax = Convert.ToDouble((Math.Ceiling((_maxValue / Math.Pow(10, exp))) * Math.Pow(10, exp)))
            Else
                tempMax = 1.0F
            End If
            ' Once max value is calculated, tick value can be determined; tick value should be a whole number
            _yTickValue = tempMax / _yTickCount
            Dim expTick As Double = Convert.ToDouble(Math.Floor(Math.Log10(_yTickValue)))
            _yTickValue = Convert.ToDouble((Math.Ceiling((_yTickValue / Math.Pow(10, expTick))) * Math.Pow(10, expTick)))

            ' Re-calculate the max value with the new tick value
            _maxValue = CType(_yTickValue * _yTickCount, Single)
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method calculates the height for each bar in the graph
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CalculateSweepValues()
            ' Called when all values and scale factor are known
            ' All values calculated here are relative from (_xOrigin, _yOrigin)
            Dim i As Integer = 0
            Dim item As ChartItem
            For Each item In DataPoints
                ' This implementation does not support negative value
                If item.Value >= 0 Then
                    item.SweepSize = item.Value / _scaleFactor
                End If
                ' (_spaceBtwBars/2) makes half white space for the first bar
                item.StartPos = _spaceBtwBars / 2 + i * (_barWidth + _spaceBtwBars)
                i += 1
            Next item
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method calculates the width for each bar in the graph
        ''' </summary>
        ''' <param name="dataCount"></param>
        ''' <param name="barGraphWidth"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CalculateBarWidth(ByVal dataCount As Integer, ByVal barGraphWidth As Single)
            ' White space between each bar is the same as bar width itself
            _barWidth = barGraphWidth / (dataCount * 2) ' Each bar has 1 white space 
            _spaceBtwBars = _barWidth
        End Sub


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     This method assigns default value to the bar graph properties and is only called from BarGraph constructors
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[adminsupport]	30.03.2005	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AssignDefaultSettings()
            ' default values
            _totalWidth = 700.0F
            _totalHeight = 450.0F
            _fontFamily = "Verdana"
            _backColor = Color.White
            _fontColor = Color.Black
            _topBuffer = 8.0F
            _bottomBuffer = 3.0F
        End Sub
    End Class

#End Region

#Region "Base classes"

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Charts.NetChart.Chart
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Chart Class
    ''' </summary>
    ''' <remarks>
    '''     Base class implementation for BarChart and PieChart
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	30.03.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class Chart
        Private _colorLimit As Integer = 12

        Private _color As Color() = {Color.Chocolate, Color.YellowGreen, Color.Olive, Color.DarkKhaki, Color.Sienna, Color.PaleGoldenrod, Color.Peru, Color.Tan, Color.Khaki, Color.DarkGoldenrod, Color.Maroon, Color.OliveDrab}

        ' Represent collection of all data points for the chart
        Private _dataPoints As New ChartItemsCollection

        ' The implementation of this method is provided by derived classes
        Public MustOverride Function Draw() As Bitmap

        Public Property DataPoints() As ChartItemsCollection
            Get
                Return _dataPoints
            End Get
            Set(ByVal Value As ChartItemsCollection)
                _dataPoints = Value
            End Set
        End Property

        Public Sub SetColor(ByVal index As Integer, ByVal NewColor As Color)
            If index < _colorLimit Then
                _color(index) = NewColor
            Else
                Throw New Exception("Color Limit is " + _colorLimit.ToString)
            End If
        End Sub

        Public Function GetColor(ByVal index As Integer) As Color
            If index <= _colorLimit Then
                Return _color(index - 1)
            Else
                Throw New Exception("Columns limit of " + _colorLimit.ToString & " exceeded")
            End If
        End Function
    End Class

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     This class represents a data point in a chart
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	30.03.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ChartItem
        Private _label As String
        Private _description As String
        Private _value As Single
        Private _color As Color
        Private _startPos As Single
        Private _sweepSize As Single


        Private Sub New()
        End Sub

        Public Sub New(ByVal label As String, ByVal desc As String, ByVal data As Single, ByVal start As Single, ByVal sweep As Single, ByVal clr As Color)
            _label = label
            _description = desc
            _value = data
            _startPos = start
            _sweepSize = sweep
            _color = clr
        End Sub


        Public Property Label() As String
            Get
                Return _label
            End Get
            Set(ByVal Value As String)
                _label = Value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property

        Public Property Value() As Single
            Get
                Return _value
            End Get
            Set(ByVal Value As Single)
                _value = Value
            End Set
        End Property

        Public Property ItemColor() As Color
            Get
                Return _color
            End Get
            Set(ByVal Value As Color)
                _color = Value
            End Set
        End Property

        Public Property StartPos() As Single
            Get
                Return _startPos
            End Get
            Set(ByVal Value As Single)
                _startPos = Value
            End Set
        End Property

        Public Property SweepSize() As Single
            Get
                Return _sweepSize
            End Get
            Set(ByVal Value As Single)
                _sweepSize = Value
            End Set
        End Property
    End Class

    ''' -----------------------------------------------------------------------------
    ''' Project	 : camm WebManager
    ''' Class	 : camm.WebManager.Modules.Charts.NetChart.ChartItemsCollection
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Custom Collection for ChartItems
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[adminsupport]	30.03.2005	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ChartItemsCollection
        Inherits CollectionBase

        Default Public Property Item(ByVal index As Integer) As ChartItem
            Get
                Return CType(List(index), ChartItem)
            End Get
            Set(ByVal Value As ChartItem)
                List(index) = Value
            End Set
        End Property

        Public Function Add(ByVal value As ChartItem) As Integer
            Return List.Add(value)
        End Function


        Public Function IndexOf(ByVal value As ChartItem) As Integer
            Return List.IndexOf(value)
        End Function


        Public Function Contains(ByVal value As ChartItem) As Boolean
            Return List.Contains(value)
        End Function


        Public Sub Remove(ByVal value As ChartItem)
            List.Remove(value)
        End Sub
    End Class

#End Region

End Namespace
