using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents <see cref="ChartAutoScrollingMode"/> mode of axis.
    /// </summary>
    public enum ChartAutoScrollingMode
    {
        /// <summary>
        /// Indicates AutoScrollingDelta calculated in axis start position. 
        /// </summary>
        Start,

        /// <summary>
        /// Indicates AutoScrollingDelta calculated in axis end position. 
        /// </summary>
        End
    }

    /// <summary>
    /// Represents the doughnut series cap style.
    /// </summary>
    public enum DoughnutCapStyle
    {
        /// <summary>
        /// The both edges are flat.
        /// </summary>
        BothFlat = 0,

        /// <summary>
        /// The both edges are curve.
        /// </summary>
        BothCurve = 1,

        /// <summary>
        /// The start edge only curve.
        /// </summary>
        StartCurve = 2,

        /// <summary>
        /// The end edge only curve.
        /// </summary>
        EndCurve = 3,
    }

    internal enum EmptyStroke
    {
        Left = 1,
        Right = 2,
        Both = 3,
        None = 0
    }

    public enum DragType
    {
        X,
        Y,
        XY
    }

    public enum BoxPlotMode
    {
        Exclusive,
        Inclusive,
        Normal
    }
	
	public enum ScaleBreakPosition
    {
        DataCount,
        Scale,
        Percent
    }

    /// <summary>
    /// Circular series segment grouping based on group mode.
    /// </summary>
    public enum PieGroupMode
    {
        /// <summary>
        /// Circular series segment grouping based on value.
        /// </summary>
        Value,

        /// <summary>
        /// Circular series segment grouping based on percentage.
        /// </summary>
        Percentage,

        /// <summary>
        /// Circular series segment grouping based on angle.
        /// </summary>
        Angle
    }

    public enum BreakLineType
    {
        StraightLine,
        Wave        
    }

    internal enum UIElementLeftShift
    {
        LeftShift,
        RightShift,
        LeftHalfShift,
        RightHalfShift,
        Default
    }

    internal enum UIElementTopShift
    {
        TopShift,
        BottomShift,
        TopHalfShift,
        BottomHalfShift,
        Default
    }

    internal enum AxisPosition3D
    {
        FrontLeft,
        DepthFrontLeft,

        FrontRight,
        DepthFrontRight,

        BackRight,
        DepthBackRight,

        BackLeft,
        DepthBackLeft
    }

    #region Segment SelectionMode
    public enum SelectionMode
    {
        MouseClick,
        MouseMove
    }
    #endregion

    #region dragAndDrop
    public enum SnapToPoint
    {
        None,
        Round,
        Floor,
        Ceil
    }
    #endregion

    #region DateTimeRangePadding
    public enum DateTimeRangePadding
    {
        Auto,
        None,
        Round,
        Additional
    }
    #endregion

    #region MACDType
    public enum MACDType
    {
        Line,
        Histogram,
        Both
    }
    #endregion

    #region SelectionStyle
    public enum SelectionStyle
    {
        Single,
        Multiple,
    }

    public enum SelectionStyle3D
    {
        Single,
        Multiple,
    }
    #endregion

    #region RangeNavigator BarPosition

    public enum BarPosition
    {
        Inside,
        Outside
    }

    #endregion

    [Flags]
    public enum Day
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64
    }
    [Flags]
    public enum ZoomToolBarItems
    {
        All=1,
        ZoomIn=2,
        ZoomOut=4,
        Reset=8,
        SelectZooming=16
    }
    public enum NumericalPadding
    {
        Auto,//When SecondaryAxis is NumericalAxis then RangePadding should be Round
        None,
        Round,
        Normal,
        Additional
    }

 public enum ErrorBarType 
    {
        Fixed,
        Percentage,
        StandardDeviation,
        StandardErrors,
        Custom
    }

    public enum ErrorBarMode
    {
        Horizontal,
        Vertical,
        Both
    }
    
    public enum ErrorBarDirection
    {
        Both,
        Minus,
        Plus
    }

    /// <summary>
    /// Legend position in chart area.
    /// </summary>
    public enum LegendPosition
    {
        Inside,
        Outside
       
    }

 	public enum TrendlineType
    {
        Linear,
        Exponential,
        Power,
        Logarithmic,
        Polynomial  
    }

    #region DateTimeintervalType
    /// <summary>
    /// A date time interval.
    /// </summary>
    public enum DateTimeIntervalType
    {
        /// <summary>
        /// Automatically determine interval.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Interval type is milliseconds.
        /// </summary>
        Milliseconds = 1,

        /// <summary>
        /// Interval type is seconds.
        /// </summary>
        Seconds = 2,

        /// <summary>
        /// Interval type is minutes.
        /// </summary>
        Minutes = 3,

        /// <summary>
        /// Interval type is hours.
        /// </summary>
        Hours = 4,

        /// <summary>
        /// Interval type is days.
        /// </summary>
        Days = 5,

        /// <summary>
        /// Interval type is months.
        /// </summary>
        Months = 6,

        /// <summary>
        /// Interval type is years.
        /// </summary>
        Years = 7,
    }
    #endregion

    #region enum ChartValueType
    /// <summary>
    /// Specifies the different values that are natively used.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
   
    public enum ChartValueType
    {
        /// <summary>
        ///  <see cref="Double"/> value
        /// </summary>
        Double,

        /// <summary>
        ///  <see cref="DateTime"/> value
        /// </summary>
        DateTime,

        /// <summary>
        ///   <see cref="String"/> value
        /// </summary>
        String,

        /// <summary>
        ///   <see cref="TimeSpan"/> value
        /// </summary>
        TimeSpan,

        /// <summary>
        ///   Logarithmic value
        /// </summary>
        Logarithmic
    }


    #endregion

    #region Rendering modes
    /// <summary>
    /// Specifies the rendering mode to be used to render the chart series.
    /// </summary>
    
    public enum RenderingMode
    {
        /// <summary>
        /// Default element will be used to render the series
        /// </summary>
        Default,

        /// <summary>
        /// WriteableBitmap will be used to render the series
        /// </summary>
        WriteableBitmap,

        /// <summary>
        /// DirectX will be used to render the series
        /// </summary>
        DirectX
    }
    #endregion

    #region Legend icon symbol
    /// <summary>
    /// Represents the Icon for the Chartlegend
    /// </summary>  
    /// <seealso cref="ChartSeriesBase"/>
   
    public enum ChartLegendIcon
    {
        /// <summary>
        /// Default behaviour
        /// </summary>
        None,

        /// <summary>
        /// Represents the Icon of Series type
        /// </summary>
        SeriesType,

        /// <summary>
        /// Represents the Rectangular Icon
        /// </summary>
        Rectangle,

        /// <summary>
        ///Represents the Straight Line
        /// </summary>       
        StraightLine,

        /// <summary>
        /// Represents the VerticalLine
        /// </summary>       
        VerticalLine,

        /// <summary>
        /// Represents the Circle
        /// </summary>       
        Circle,

        /// <summary>
        /// Represents the Diamond
        /// </summary>
        Diamond,

        /// <summary>
        /// Represents the Pentagon
        /// </summary>      
        Pentagon,

        /// <summary>
        /// Represents the Hexagon
        /// </summary>       
        Hexagon,

        /// <summary>
        /// Represents the Triangle
        /// </summary>
        Triangle,

        /// <summary>
        /// Represents the Inverted Triangle
        /// </summary>   
        InvertedTriangle,

        /// <summary>
        /// Represents the Cross
        /// </summary>       
        Cross,
        
        /// <summary>
        /// Represents the Plus
        /// </summary>       
        Plus,
    }
    #endregion

    public enum TrackballLabelDisplayMode
    {
        NearestPoint,
        FloatAllPoints,
        GroupAllPoints
    }
    

    #region enum Direction
    /// <summary>
    /// Represents sorting direction
    /// </summary>
   
    public enum Direction
    {
        /// <summary>
        /// Orders the items in increasing order.
        /// </summary>
        Ascending,
        /// <summary>
        /// Orders the items in decreasing order.
        /// </summary>
        Descending
    }
    #endregion

    #region enum SortingAxis

    /// <summary>
    /// Represents Sorting Axis.
    /// </summary>
  
    public enum SortingAxis
    {
        /// <summary>
        /// Sorting will be done based on values related to x-axis.
        /// </summary>
        X,
        /// <summary>
        /// Sorting will be done based on values related to y-axis.
        /// </summary>
        Y,
    }
    #endregion

    #region Adornment symbols
    /// <summary>
    /// Represents the adornments marker symbol types.
    /// </summary>
   
    public enum ChartSymbol
    {
        /// <summary>
        /// Custom option to set User-defined SymbolTemplates
        /// </summary>
        Custom,

        /// <summary>
        /// Renders Ellipse symbol
        /// </summary>
        Ellipse,

        /// <summary>
        /// Renders Cross symbol
        /// </summary>
        Cross,

        /// <summary>
        /// Renders Diamond symbol
        /// </summary>
        Diamond,

        /// <summary>
        /// Renders Hexagon symbol
        /// </summary>
        Hexagon,

        /// <summary>
        /// Renders HorizontalLine symbol
        /// </summary>
        HorizontalLine,

        /// <summary>
        /// Renders InvertedTriangle symbol
        /// </summary>
        InvertedTriangle,

        /// <summary>
        /// Renders Pentagon symbol
        /// </summary>
        Pentagon,

        /// <summary>
        /// Renders Plus symbol
        /// </summary>
        Plus,

        /// <summary>
        /// Renders Square symbol
        /// </summary>
        Square,

        /// <summary>
        /// Renders Traingle symbol
        /// </summary>
        Triangle,

        /// <summary>
        /// Renders VerticalLine symbol
        /// </summary>
        VerticalLine,
    }

    #endregion

    public enum LabelPlacement
    {
        OnTicks,
        BetweenTicks
    }

    public enum ActualLabelPosition
    {
        Top,
        Left,
        Right,
        Bottom
    }

    public enum LabelAlignment
    {
        Center,
        Far,
        Near
    }

    public enum BorderType
    {
        Brace,
        None,
        Rectangle, 
        WithoutTopAndBottomBorder,   
    }

    public enum ChartPolarAngle
    {
        Rotate0,
        Rotate90,
        Rotate180,
        Rotate270
    }
    #region Axis element position
    /// <summary>
    /// Represents axis elements position in <see cref="ChartAxis"/> elements panel.
    /// </summary>
   
    public enum AxisElementPosition
    {
        /// <summary>
        /// Positions the elements above the axis line.
        /// </summary>
        Inside,
        /// <summary>
        /// Positions the elements below the axis line.
        /// </summary>
        Outside
    }
    #endregion

    #region Axis header position
    /// <summary>
    /// Represents the axis header position<see cref="ChartAxis"/>.
    /// </summary>

    public enum AxisHeaderPosition
    {
        /// <summary>
        /// Positions the header near the axis.
        /// </summary>
        Near,
        /// <summary>
        /// Positions the header far away from the axis.
        /// </summary>
        Far
    }
    #endregion

    #region Aggregation functions
    /// <summary>
    /// Represents the aggregation functions<see cref="ChartAxis"/>.
    /// </summary>

    public enum AggregateFunctions
    {
        Average,
        Count,
        Max,
        Min,
        Sum,
        None
    }
    #endregion

    #region EdgelabelsDrawingMode
    /// <summary>
    /// Represents the modes for placing edge labels in <see cref="ChartAxis"/>.
    /// </summary>

    public enum EdgeLabelsDrawingMode
    {
        /// <summary>
        /// Value indicating that the edge label should appear at the center of its GridLines.
        /// </summary>
        Center,

        /// <summary>
        /// Value indicating that edge labels should be shifted to either left or right so that it comes within the area of Chart.
        /// </summary>
        Shift,

        /// <summary>
        /// Value indicating that the edge labels should be fit within the area of <see cref="SfChart"/>.
        /// </summary>
        Fit,

        /// <summary>
        /// Value indicating that the edge labels will be hidden.
        /// </summary>
        Hide,

    }
    #endregion

    #region EdgeLabelsVisibilityMode
    /// <summary>
    /// Represents the visibility for edge label<see cref="ChartAxis"/>.
    /// </summary>
   
    public enum EdgeLabelsVisibilityMode
    {
        /// <summary>
        /// Value indicating that default behavior of axis.
        /// </summary>
        Default,

        /// <summary>
        /// Value indicating that edge labels should be visible all cases.
        /// </summary>
        AlwaysVisible,

        /// <summary>
        /// Value indicating that edge labels should be visible in non zoom mode.
        /// </summary>
        Visible,

    }
    #endregion

    #region LabelsIntersectAction

    /// <summary>
    ///  Specifies the options for the action that is to be taken when labels intersect each other.
    /// </summary>
    /// <seealso cref="ChartAxis"/>
  
    public enum AxisLabelsIntersectAction
    {
        /// <summary>
        /// No special action is taken. Labels may intersect.
        /// </summary>
        None,

        /// <summary>
        /// Labels are wrapped into multiple rows to avoid intersection.
        /// </summary>
        MultipleRows,

        /// <summary>
        /// Labels are hidden to avoid intersection.
        /// </summary>
        Hide,

        /// <summary>
        /// Labels are rotated to avoid intersection.
        /// </summary>
        Auto,

        /// <summary>
        /// Labels are wrapped to next line to aviod intersection.
        /// </summary>
        Wrap

    }
    #endregion

    #region Color Palette
    /// <summary>
    /// Represents the different types of color palette available in <see cref="SfChart"/> library.
    /// </summary>
   
    public enum ChartColorPalette
    {
        /// <summary>
        /// No palette will be set
        /// </summary>
        None,

        /// <summary>
        /// Metro palette will be set.
        /// </summary>
        /// 
        Metro,

        /// <summary>
        /// Custom palette will be set, and color values will be taken from <see cref="ChartColorModel.CustomBrushes"/> collection.
        /// </summary>   
        Custom,

        /// <summary>
        /// AutumnBrights palette will be set
        /// </summary>
        AutumnBrights,

        /// <summary>
        /// FloraHues palette will be set
        /// </summary>
        FloraHues,

        /// <summary>
        /// Pineapple palette will be set
        /// </summary>
        Pineapple,

#if WINDOWS_UAP
        /// <summary>
        /// TomatoSpectram palette will be set
        /// </summary>

        TomatoSpectrum,
#else
         /// <summary>
        /// TomatoSpectram palette will be set
        /// </summary>

        TomotoSpectrum,
#endif

        /// <summary>
        /// RedChrome palette will be set
        /// </summary>
        RedChrome,

        /// <summary>
        /// PurpleChrome palette will be set
        /// </summary>
        PurpleChrome,

        /// <summary>
        /// BlueChrome palette will be set
        /// </summary>
        BlueChrome,

        /// <summary>
        /// GreenChrome palette will be set
        /// </summary>
        GreenChrome,

        /// <summary>
        /// Elite palette will be set
        /// </summary>
        Elite,

        /// <summary>
        /// SandyBeach palette will be set
        /// </summary>
        SandyBeach,

        /// <summary>
        /// LightCandy palette will be set
        /// </summary>
        LightCandy
    }

#endregion

#region RangePadding
    /// <summary>
    /// Represents the modes of range padding.
    /// </summary>
    internal enum RangePaddingMode
    {
        None,

        Normal,

        Additional
    }
#endregion

#region ChartLegendAlignment
    /// <summary>
    /// A custom <see cref="SfChart"/> alignment to handle both horizontal and vertical alignment types in a generalized way. 
    /// </summary>
   
    public enum ChartAlignment
    {
        /// <summary>
        /// Positions the element as like setting left/top alignment.
        /// </summary>
        Near,
        /// <summary>
        /// Positions the element as like setting right/bottom alignment.
        /// </summary>
        Far,
        /// <summary>
        /// Positions the element with center alignment.
        /// </summary>
        Center,
        /// <summary>
        /// Positions the element with default alignment when the series is transposed.
        /// </summary>
        Auto
    }
#endregion

#region ChartSeriesDrawType
    /// <summary>
    /// Represents modes of drawing radar and polar types.
    /// </summary>
   
    public enum ChartSeriesDrawType
    {
        /// <summary>
        /// Draw the Filled Area in the Polar Chart type
        /// </summary>
        Area,

        /// <summary>
        /// Draw the Lines in the Polar chart type
        /// </summary>
        Line,
    }

#endregion

#region Orientation
    /// <summary>
    /// Represents modes of Chart orientation
    /// </summary>
   
    public enum ChartOrientation
    {
        /// <summary>
        /// Orienatation will be automatically analyzed based on the panel's docking position.
        /// </summary>
        Default,
        /// <summary>
        /// Horizontal Orientation will be set.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical Orientation will be set.
        /// </summary>
        Vertical

    }
#endregion

#region ChartAreaType

    /// <summary>
    /// Identifies axes types enumeration.
    /// </summary>
    /// <example>
    /// Intended for internal use
    /// </example>
    /// <seealso>
    ///     <cref>ChartArea</cref>
    /// </seealso>
   
    public enum ChartAreaType
    {
        /// <summary>
        /// Represents No axis.
        /// </summary>
        None,

        /// <summary>
        /// Cartesian axis.
        /// </summary>
        CartesianAxes,

        /// <summary>
        /// Polar axis.
        /// </summary>
        PolarAxes
    }
#endregion

#region ChartUnitType
    /// <summary>
    /// Represents modes for chart rows/columns space allocations.
    /// </summary>
  
    public enum ChartUnitType
    {
        /// <summary>
        /// Height/Width will be auto adjusted.
        /// </summary>
        Star,
        /// <summary>
        /// Height/Width will be based on the pixel units given.
        /// </summary>
        Pixels
    }

#endregion

#region zoom mode
    /// <summary>
    /// Represents zooming modes of <see cref="SfChart"/>
    /// </summary>
    
    public enum ZoomMode
    {
        /// <summary>
        /// Zooming will be done along x-axis
        /// </summary>
        X,
        /// <summary>
        /// Zooming will be done along y-axis
        /// </summary>
        Y,
        /// <summary>
        /// Zooming will be done along both axis.
        /// </summary>
        XY
    }

#endregion

#region
    /// <summary>
    /// Represents label position modes available for PieSeries adornments.
    /// </summary>
    public enum CircularSeriesLabelPosition
    {
        /// <summary>
        /// PieSeries adornment labels will be  placed inside over the PieSeries.
        /// </summary>
        Inside,
        /// <summary>
        /// PieSeries adornment labels will be  placed just outside over the PieSeries.
        /// </summary>
        Outside,
        /// <summary>
        /// PieSeries adornment labels will be placed outside over the PieSeries at a certain distance.
        /// </summary>
        OutsideExtended
    }

    /// <summary>
    /// Adornment connector line mode
    /// </summary>
    public enum ConnectorMode
    {
        Bezier,
        Line,
        StraightLine,
    }

    /// <summary>
    /// Defines adornment connector line drawing position.
    /// </summary>
    public enum ConnectorLinePosition 
    {
        Auto,
        Center,
    }

    #endregion
    #region AdornmnetsLabelPosition
    /// <summary>
    /// Represents the positioning of Adornment labels.
    /// </summary>

    public enum AdornmentsLabelPosition
    {
        /// <summary>
        /// Positions the Adornment labels at Default.
        /// </summary>
        Default,

        /// <summary>
        /// Positions the Adornment labels at Auto.
        /// </summary>
        Auto,

        /// <summary>
        /// Positions the Adornment labels at Inner.
        /// </summary>
        Inner,

        /// <summary>
        /// Positions the Adornment labels at Outer.
        /// </summary>
        Outer,

        /// <summary>
        /// Positions the Adornment labels at Center.
        /// </summary>
        Center

    }

#endregion

#region AdornmnetsPosition
    /// <summary>
    /// Represents modes for positioning Chart adornments.
    /// </summary>
    /// <remarks>
    /// AdornementPosition value cannot be specified for all series types.
    /// The values in adornments position will be applicable only to certain series
    /// </remarks>
   
    public enum AdornmentsPosition
    {
        /// <summary>
        /// Positions the adornment at the top edge point of a chart segment.
        /// </summary>
        Top,
        /// <summary>
        /// Positions the adornment at the bottom edge point of a chart segment.
        /// </summary>
        Bottom,

        /// <summary>
        /// Positions the adornment at the center point of a chart segment.
        /// </summary>
        TopAndBottom

    }

#endregion

#region enum LabelContent

    /// <summary>
    /// Enumeration represents series adornments label content.
    /// </summary>
    /// <seealso>
    ///     <cref>AdornmentInfo</cref>
    /// </seealso>
   
    public enum LabelContent
    {
        /// <summary>
        /// Identifies that label should contain X value of series' point.
        /// </summary>
        XValue,

        /// <summary>
        /// Identifies that label should contain Y value of series' point.
        /// </summary>
        YValue,

        /// <summary>
        /// Identifies that label should contain percentage value of series' point among other points.
        /// </summary>
        Percentage,

        /// <summary>
        /// Identifies that label should contain value of Y of total values.
        /// </summary>
        YofTot,

        /// <summary>
        /// Identifies that label should contain <see cref="DateTime"/> value.
        /// </summary>
        DateTime,

        /// <summary>
        /// Label's content will be retrieved from the <see>
        /// <cref>ChartAdornmentInfo.LabelContentPath</cref>
        /// </see>
        /// property.
        /// </summary>
        LabelContentPath
    }
#endregion

#region EmptyPointStyle

    /// <summary>
    /// Represents modes of displaying empty points.
    /// </summary>
    public enum EmptyPointStyle
    {
        /// <summary>
        /// The empty point segment resembles the shape of a normal segment.
        /// Fills the empty point segments with the color value specified in series <see cref="ChartSeriesBase.EmptyPointInterior"/> property.
        /// </summary>
        Interior,

        /// <summary>
        /// The empty point segment resembles the shape of a symbol control.       
        /// </summary>
        Symbol,

        /// <summary>
        /// The empty point segment resembles the shape of a symbol control.
        ///Fills the symbol segments with the color value specified in series <see cref="ChartSeriesBase.EmptyPointInterior"/> property.
        /// </summary>
        SymbolAndInterior

    }
#endregion

#region EmptyPointValue

    /// <summary>
    /// Represents modes for handling empty points.
    /// </summary> 
    public enum EmptyPointValue
    {
        /// <summary>
        /// Validates the empty points in a series and sets the points y-value to Zero.
        /// </summary>
        Zero,

        /// <summary>
        /// Validates the empty points in a series and sets the points y-value to an average value based on its neighbouring points.
        /// </summary>
        Average
    }
#endregion

#region FunnelMode

    /// <summary>
    /// Lists the funnel mode options.
    /// </summary>
    /// <seealso>
    ///     <cref>ChartFunnelType</cref>
    /// </seealso>
   
    public enum ChartFunnelMode
    {
        /// <summary>
        /// The specified Y value is used to compute the width of the corresponding block.
        /// </summary>
        ValueIsWidth,

        /// <summary>
        /// The specified Y value is used to compute the height of the corresponding block.
        /// </summary>
        ValueIsHeight
    }
#endregion

#region PyramidMode

    /// <summary>
    /// Specifies the mode in which the Y values should be interpreted in the Pyramid chart.
    /// </summary>
    /// <seealso>
    ///     <cref>ChartPyramidType</cref>
    /// </seealso>
    public enum ChartPyramidMode
    {
        /// <summary>
        /// The Y values are proportional to the length of the sides of the pyramid.
        /// </summary>
        Linear,

        /// <summary>
        /// The Y values are proportional to the surface area of the corresponding blocks.
        /// </summary>
        Surface
    }
#endregion

#region IntervalType
    /// <summary>
    /// Specifies the Interval type in which the navigator values should be displayed.
    /// </summary>
    /// <seealso cref="ChartPyramidType"/>
    public enum NavigatorIntervalType
    {
        /// <summary>
        /// One year interval.
        /// </summary>
        Year,

        /// <summary>
        /// One Quarter interval
        /// </summary>
        Quarter,

        /// <summary>
        /// One Month interval
        /// </summary>
        Month,

        /// <summary>
        /// One Week interval
        /// </summary>
        Week,

        /// <summary>
        /// One Day interval
        /// </summary>
        Day,

        /// <summary>
        /// One Day interval
        /// </summary>
        Hour

    }
#endregion

#region NavigatorRangePadding
    public enum NavigatorRangePadding
    {
        None,
        Round
    }
#endregion

#region Annotations

    public enum CoordinateUnit
    {
        /// <summary>
        /// The pixel mode for the CoordinateUnit of Annotation
        /// </summary>
        Pixel,

        /// <summary>
        /// The axis mode for the CoordianteUint of Annotation
        /// </summary>
        Axis
    }

    public enum AxisMode
    {
        Horizontal,
        Vertical,
        All
    }

    public enum LineCap
    {
        None,
        Arrow
    }

    public enum ToolTipLabelPlacement
    {
        Left,  
        Right, 
        Top, 
        Bottom 
    }

    
#endregion

#region UpdateAction
    
    [Flags]
    internal enum UpdateAction
    {
        Create = 2,
        UpdateRange = 4,
        Layout= 8,
        Render = 16,
        LayoutAndRender = Layout | Render,
        UpdateRangeAndArrange = UpdateRange | Layout | Render,
        Invalidate = Create | UpdateRange | Layout | Render,
    }

#endregion

#region Surface Type
   /// <summary>
   /// Specifies the type of surface
   /// </summary>
    public enum SurfaceType
    {
        Surface,
        WireframeSurface,
        Contour,
        WireframeContour
    }
    
#endregion

#region Camera Projection

    /// <summary>
    /// Specifies the mode of surface projection
    /// </summary>
    public enum CameraProjection
    {
        /// <summary>
        /// Represents Perspective CameraProjection
        /// </summary>
        Perspective,

        /// <summary>
        /// Represents Orthographic CameraProjection
        /// </summary>
        Orthographic
    }

    #endregion
    #region Comparison Mode
    /// <summary>
    /// Specifies which price need to consider for fluctuation detection
    /// </summary>
    public enum FinancialPrice
    {
        High,
        Low,
        Open,
        Close,
        None
    }
    #endregion

    #region WaterfallSegment type

    /// <summary>
    /// Specifies which type segment consider for rendering.
    /// </summary>
    internal enum WaterfallSegmentType
    {
        Positive,
        Negative,
        Sum
    }

    #endregion

    #region Spline type
    /// <summary>
    /// Specifies the type of spline.
    /// </summary>
    public enum SplineType
    {
        Natural,
        Monotonic,
        Cardinal,
        Clamped
    }
    #endregion
}
