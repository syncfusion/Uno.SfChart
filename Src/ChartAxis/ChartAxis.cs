using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Data.Xml.Dom;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Enables plotting of data points in a chart control.
    /// </summary>
    /// <remarks>
    /// The Chart requires a minimum of two axes namely primary axis and secondary axis to plot data points. 
    /// Values / data in the chart are plotted against these two axes. 
    /// Chart WINRT also supports adding multiple axes to the chart and the series can be drawn with 
    /// reference to any x-axis,y-axis added to <see cref="SfChart"/>
    /// </remarks>
    /// <example>
    /// <code language="XAML">
    ///     &lt;syncfusion:SfChart&gt;
    ///           &lt;syncfusion:SfChart.PrimaryAxis&gt;
    ///                 &lt;syncfusion:ChartAxis/&gt;
    ///           &lt;/syncfusion:SfChart.PrimaryAxis&gt;
    ///           &lt;syncfusion:SfChart.SecondaryAxis &gt;
    ///                 &lt;syncfusion:ChartAxis/&gt;                      
    ///           &lt;/syncfusion:SfChart.SecondaryAxis&gt;
    ///     &lt;/syncfusion:SfChart &gt;
    /// </code>
    /// <code language="C#">
    ///     ChartAxis xAxis = new ChartAxis();
    ///         chartArea.PrimaryAxis = xAxis;
    ///     ChartAxis yAxis = new ChartAxis();
    ///         chartArea.SecondaryAxis = yAxis;
    /// </code>
    /// </example>
    [TemplateVisualStateAttribute(Name = "CommonStyle", GroupName = "StyleMode")]
    [TemplateVisualStateAttribute(Name = "TouchModeStyle", GroupName = "StyleMode")]
    [TemplateVisualStateAttribute(Name = "VerticalTouchModeStyle", GroupName = "StyleMode")]
    public abstract partial class ChartAxis : Control, ICloneable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="MaximumLabels"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumLabelsProperty =
            DependencyProperty.Register(
                "MaximumLabels", 
                typeof(int), 
                typeof(ChartAxis),
                new PropertyMetadata(3, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HeaderPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderPositionProperty =
            DependencyProperty.Register(
                "HeaderPosition", 
                typeof(AxisHeaderPosition),
                typeof(ChartAxis),
                new PropertyMetadata(AxisHeaderPosition.Near, new PropertyChangedCallback(OnHeaderPositionChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="PositionPath"/> property.
        /// </summary>
        public static readonly DependencyProperty PositionPathProperty =
          DependencyProperty.Register(
              "PositionPath",
              typeof(string),
              typeof(ChartAxis),
              new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ContentPath"/> property.
        /// </summary>
        public static readonly DependencyProperty ContentPathProperty =
          DependencyProperty.Register(
              "ContentPath", 
              typeof(string),
              typeof(ChartAxis),
              new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelFormat"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register(
                "LabelFormat", 
                typeof(string), 
                typeof(ChartAxis),
                new PropertyMetadata(string.Empty, OnPropertyChanged));
        
        /// <summary>
        ///The DependencyProperty for <see cref="LabelsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelsSourceProperty =
          DependencyProperty.Register(
              "LabelsSource", 
              typeof(object), 
              typeof(ChartAxis),
              new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="PostfixLabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PostfixLabelTemplateProperty =
            DependencyProperty.Register(
                "PostfixLabelTemplate", 
                typeof(DataTemplate),
                typeof(ChartAxis),
                new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));
        
        /// <summary>
        /// The DependencyProperty for <see cref="PrefixLabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PrefixLabelTemplateProperty =
            DependencyProperty.Register(
                "PrefixLabelTemplate", 
                typeof(DataTemplate),
                typeof(ChartAxis),
                new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="PlotOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty PlotOffsetProperty =
            DependencyProperty.Register(
                "PlotOffset", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(0d, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="AxisLineOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty AxisLineOffsetProperty =
            DependencyProperty.Register(
                "AxisLineOffset", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(0d, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelsPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelsPositionProperty =
            DependencyProperty.Register(
                "LabelsPosition", 
                typeof(AxisElementPosition), 
                typeof(ChartAxis),
                new PropertyMetadata(AxisElementPosition.Outside, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelsIntersectAction"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelsIntersectActionProperty =
            DependencyProperty.Register(
                "LabelsIntersectAction", 
                typeof(AxisLabelsIntersectAction),
                typeof(ChartAxis), 
                new PropertyMetadata(AxisLabelsIntersectAction.Hide, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelExtent"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelExtentProperty =
            DependencyProperty.Register(
                "LabelExtent", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(0.0, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelRotationAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelRotationAngleProperty =
            DependencyProperty.Register(
                "LabelRotationAngle", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(0d, new PropertyChangedCallback(OnLabelRotationChanged)));

        /// <summary>
        ///The DependencyProperty for <see cref="AxisLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty AxisLineStyleProperty =
            DependencyProperty.Register(
                "AxisLineStyle", 
                typeof(Style),
                typeof(ChartAxis),
                null);
        
        /// <summary>
        /// The DependencyProperty for <see cref="OpposedPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
            DependencyProperty.Register(
                "OpposedPosition", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(false, OnOpposedPositionChanged));

#if NETFX_CORE

        /// <summary>
        ///The DependencyProperty for <see cref="DesiredIntervalsCount"/> property.
        /// </summary>
        public static readonly DependencyProperty DesiredIntervalsCountProperty =
            DependencyProperty.Register(
                "DesiredIntervalsCount", 
                typeof(object), 
                typeof(ChartAxis),
                new PropertyMetadata(null, OnDesiredIntervalsCountPropertyChanged));

#else

        /// <summary>
        ///The DependencyProperty for <see cref="DesiredIntervalsCount"/> property.
        /// </summary>
        public static readonly DependencyProperty DesiredIntervalsCountProperty =
            DependencyProperty.Register(
                "DesiredIntervalsCount", 
                typeof(int?), 
                typeof(ChartAxis),
                new PropertyMetadata(null, OnDesiredIntervalsCountPropertyChanged));

#endif

        /// <summary>
        /// The DependencyProperty for <see cref="ThumbLabelVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty ThumbLabelVisibilityProperty =
            DependencyProperty.Register(
                "ThumbLabelVisibility", 
                typeof(Visibility), 
                typeof(ChartAxis),
                new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        ///The DependencyProperty for <see cref="ThumbLabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ThumbLabelTemplateProperty =
            DependencyProperty.Register(
                "ThumbLabelTemplate", 
                typeof(DataTemplate),
                typeof(ChartAxis),
                new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="Header"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header", 
                typeof(object), 
                typeof(ChartAxis),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HeaderStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(
                "HeaderStyle",
                typeof(LabelStyle),
                typeof(ChartAxis),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="HeaderTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate", 
                typeof(DataTemplate),
                typeof(ChartAxis),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="TickLineSize"/> property.
        /// </summary>
        public static readonly DependencyProperty TickLineSizeProperty =
            DependencyProperty.Register(
                "TickLineSize", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(6d, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="IsInversed"/> property.
        /// </summary>
        public static readonly DependencyProperty IsInversedProperty =
            DependencyProperty.Register(
                "IsInversed", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(false, OnIsInversedChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Origin"/> property.
        /// </summary>
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register(
                "Origin", 
                typeof(double), 
                typeof(ChartAxis),
                new PropertyMetadata(0d, new PropertyChangedCallback(OnPropertyChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowOrigin"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowOriginProperty =
            DependencyProperty.Register(
                "ShowOrigin", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(false, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="TickLinesPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty TickLinesPositionProperty =
            DependencyProperty.Register(
                "TickLinesPosition", 
                typeof(AxisElementPosition),
                typeof(ChartAxis),
                new PropertyMetadata(AxisElementPosition.Outside, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ShowAxisNextToOrigin"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowAxisNextToOriginProperty =
            DependencyProperty.Register(
                "ShowAxisNextToOrigin", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(false, new PropertyChangedCallback(OnShowAxisNextToOriginChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="EdgeLabelsDrawingMode"/> property.
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsDrawingModeProperty =
            DependencyProperty.Register(
                "EdgeLabelsDrawingMode",
                typeof(EdgeLabelsDrawingMode),
                typeof(ChartAxis),
                new PropertyMetadata(EdgeLabelsDrawingMode.Center, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="EdgeLabelsVisibilityMode"/> property.
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsVisibilityModeProperty =
            DependencyProperty.Register(
                "EdgeLabelsVisibilityMode", 
                typeof(EdgeLabelsVisibilityMode),
                typeof(ChartAxis), 
                new PropertyMetadata(EdgeLabelsVisibilityMode.Default, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="MajorGridLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorGridLineStyleProperty =
            DependencyProperty.Register("MajorGridLineStyle", typeof(Style), typeof(ChartAxis), null);

        /// <summary>
        /// The DependencyProperty for <see cref="MinorGridLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MinorGridLineStyleProperty =
            DependencyProperty.Register(
                "MinorGridLineStyle", 
                typeof(Style), 
                typeof(ChartAxis),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="MajorTickLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MajorTickLineStyleProperty =
            DependencyProperty.Register(
                "MajorTickLineStyle", 
                typeof(Style), 
                typeof(ChartAxis),
                new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="MinorTickLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty MinorTickLineStyleProperty =
            DependencyProperty.Register("MinorTickLineStyle", typeof(Style), typeof(ChartAxis), null);

        /// <summary>
        /// The DependencyProperty for <see cref="OriginLineStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty OriginLineStyleProperty =
            DependencyProperty.Register("OriginLineStyle", typeof(Style), typeof(ChartAxis), null);

        /// <summary>
        /// The DependencyProperty for <see cref="ShowTrackBallInfo"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTrackBallInfoProperty =
            DependencyProperty.Register(
                "ShowTrackBallInfo",
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(false));

        /// <summary>
        ///The DependencyProperty for <see cref="TrackBallLabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty TrackBallLabelTemplateProperty =
            DependencyProperty.Register(
                "TrackBallLabelTemplate", 
                typeof(DataTemplate), 
                typeof(ChartAxis),
                null);

        /// <summary>
        /// The DependencyProperty for <see cref="CrosshairLabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty CrosshairLabelTemplateProperty =
            DependencyProperty.Register(
                "CrosshairLabelTemplate", 
                typeof(DataTemplate),
                typeof(ChartAxis),
                null);

        /// <summary>
        /// The DependencyProperty for <see cref="ShowGridLines"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowGridLinesProperty =
            DependencyProperty.Register(
                "ShowGridLines", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(true, new PropertyChangedCallback(OnShowGridLinePropertyChanged)));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelStyleProperty =
            DependencyProperty.Register(
                "LabelStyle", 
                typeof(LabelStyle),
                typeof(ChartAxis),
                new PropertyMetadata(null, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="LabelTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register(
                "LabelTemplate", 
                typeof(DataTemplate), 
                typeof(ChartAxis),
                new PropertyMetadata(null, new PropertyChangedCallback(OnLabelTemplateChanged)));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableAutoIntervalOnZooming"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableAutoIntervalOnZoomingProperty =
            DependencyProperty.Register(
                "EnableAutoIntervalOnZooming", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(true));
        
        /// <summary>
        /// The DependencyProperty for <see cref="AxisVisibility"/> property.
        /// </summary>
        internal static readonly DependencyProperty AxisVisibilityProperty =
            DependencyProperty.Register(
                "AxisVisibility",
                typeof(Visibility), 
                typeof(ChartAxis),
                new PropertyMetadata(Visibility.Visible, OnPropertyChanged));
        
        /// <summary>
        /// The DependencyProperty for <see cref="ActualTrackBallLabelTemplate"/> property.
        /// </summary>
        internal static readonly DependencyProperty ActualTrackBallLabelTemplateProperty =
            DependencyProperty.Register(
                "ActualTrackBallLabelTemplate", 
                typeof(DataTemplate),
                typeof(ChartAxis), 
                null);

        /// <summary>
        /// The DependencyProperty for <see cref="Orientation"/> property.
        /// </summary>
        internal static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(ChartAxis),
                new PropertyMetadata(Orientation.Horizontal, OnAxisOrientationChanged));

        /// <summary>
        /// The DependencyProperty for the <see cref="RangeStyles"/> property.
        /// </summary>
        public static readonly DependencyProperty RangeStylesProperty =
            DependencyProperty.Register(
                "RangeStyles",
                typeof(ChartAxisRangeStyleCollection),
                typeof(ChartAxis),
                new PropertyMetadata(null, RangeStylesPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ValueType"/> property.
        /// </summary> 
        internal static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register(
                "ValueType",
                typeof(ChartValueType), 
                typeof(ChartAxis),
                new PropertyMetadata(ChartValueType.Double, OnValuetypeChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="AxisLabelAlignment"/> property.
        /// </summary>
        internal static readonly DependencyProperty AxisLabelAlignmentProperty =
            DependencyProperty.Register(
                "AxisLabelAlignment",
                typeof(LabelAlignment),
                typeof(ChartAxis),
                new PropertyMetadata(LabelAlignment.Center, OnPropertyChanged));

        #endregion

        #region Fields

        #region Internal Fields

        /// <summary>
        /// Initializes c_intervalDivs
        /// </summary>
        internal static readonly int[] c_intervalDivs = new int[] { 10, 5, 2, 1 };

        internal ObservableCollection<ChartAxisLabel> m_VisibleLabels;
        
        internal bool smallTicksRequired = false;
        
        internal bool IsDefaultRange = false;

        internal bool IsSecondaryAxis = false;

        internal bool DisableScrollbar;

        internal bool axisElementsUpdateRequired = false;

        internal ILayoutCalculator AxisLayoutPanel;

        internal ILayoutCalculator axisLabelsPanel;

        internal MultiLevelLabelsPanel axisMultiLevelLabelsPanel;

        internal ILayoutCalculator axisElementsPanel;

        internal UIElementsRecycler<Line> GridLinesRecycler;

        internal UIElementsRecycler<Line> MinorGridLinesRecycler;

        internal ContentControl headerContent;

        internal List<double> m_smalltickPoints = new List<double>();
        
        internal bool isManipulated = false;

        #endregion
        
        #region Protected Fields

        /// <summary>
        /// CRoundDecimals const variable declarations
        /// </summary>
        protected const int CRoundDecimals = 11;

        /// <summary>
        /// MaxPixelsCount  variable declarations
        /// </summary>
        protected double MaxPixelsCount = 100;

        /// <summary>
        /// isInversed variable declarations
        /// </summary>
        protected bool isInversed = false;

        #endregion

        #region Private Fields

        private Rect renderedRect;

        private ChartAxisLabelCollection m_customLabels;

        private ObservableCollection<ISupportAxes> registeredSeries;

        private List<ChartAxis> associatedAxes;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Reviewed")]
        IAsyncAction checkRegisterAction;

        private double actualPlotOffset = 0;

        /// <summary>
        /// Contains  actual Range WithoutPadding
        /// </summary>
        private DoubleRange m_actualRange = DoubleRange.Empty;

        /// <summary>
        /// Contains Actual VisibleRange
        /// </summary>
        DoubleRange m_actualVisibleRange = DoubleRange.Empty;

        Rect arrangeRect;

        bool isChecked = false;

        #endregion

        #endregion
        
        #region Constructor

        /// <summary>
        /// Called when instance created for ChartAxis
        /// </summary>
        public ChartAxis()
        {
            ValueToCoefficientCalc = ValueToCoefficient;
            CoefficientToValueCalc = CoefficientToValue;
            DefaultStyleKey = typeof(ChartAxis);
            m_VisibleLabels = new ObservableCollection<ChartAxisLabel>();
            m_VisibleLabels.CollectionChanged += m_VisibleLabels_CollectionChanged;
            Binding visibilityBinding = new Binding();
            visibilityBinding.Source = this;
            visibilityBinding.Path = new PropertyPath("Visibility");
            BindingOperations.SetBinding(this, AxisVisibilityProperty, visibilityBinding);
        }

        #endregion

        #region Delegates

        internal delegate double ValueToCoefficientHandler(double value);

        #endregion

        #region Events

        #region Public Events

        public event EventHandler<ChartAxisBoundsEventArgs> AxisBoundsChanged;

        /// <summary>
        /// Occurs when [actual range changed].
        /// </summary>
        public event EventHandler<ActualRangeChangedEventArgs> ActualRangeChanged;

        /// <summary>
        /// Occurs when the labels is created.
        /// </summary>
        public event EventHandler<LabelCreatedEventArgs> LabelCreated;

        #endregion

        #region Internal Events

        internal event EventHandler<VisibleRangeChangedEventArgs> VisibleRangeChanged;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets IsLogarithmic property
        /// </summary>
        public bool IsLogarithmic { get; internal set; }

        /// <summary>
        /// Gets or sets the plot offset value
        /// </summary>
        public double ActualPlotOffset
        {
            get { return actualPlotOffset; }
            internal set { actualPlotOffset = value; }
        }
        
        /// <summary>
        /// Gets or sets the maximum no of label to be displayed per 100 pixels.
        /// </summary>
        /// <remarks>
        /// This property used to avoid more no of labels on auto range calculation.
        /// </remarks>
        public int MaximumLabels
        {
            get { return (int)GetValue(MaximumLabelsProperty); }
            set { SetValue(MaximumLabelsProperty, value); }
        }
        
        /// <summary>
        /// Gets the visible range of the axis.
        /// </summary>
        public DoubleRange VisibleRange
        {
            get
            {
                return m_actualVisibleRange;
            }

            protected internal set
            {
                DoubleRange oldRange = m_actualVisibleRange;
                m_actualVisibleRange = value;
                OnAxisVisibleRangeChanged(new VisibleRangeChangedEventArgs() { OldRange = oldRange, NewRange = value });
            }
        }

        /// <summary>
        /// Gets or sets the position for Axis header, when enabling the ShowAxisNextToOrigin property.
        /// </summary>
        public AxisHeaderPosition HeaderPosition
        {
            get { return (AxisHeaderPosition)GetValue(HeaderPositionProperty); }
            set { SetValue(HeaderPositionProperty, value); }
        }
        
        /// <summary>
        /// Gets the bounds of the chart axis size.
        /// </summary>
        public Rect ArrangeRect
        {
            get
            {
                return arrangeRect;
            }

            internal set
            {
                arrangeRect = value;
                if (Orientation == Orientation.Horizontal)
                {
                    var axis = (this as ChartAxisBase3D);
                    double width = Math.Max(0, arrangeRect.Width - (ActualPlotOffset * 2));

                    if ((axis != null && axis.IsZAxis))
                    {
                        RenderedRect = new Rect(ActualPlotOffset, arrangeRect.Top, width, arrangeRect.Height);
                    }
                    else
                    {
                        RenderedRect = new Rect(arrangeRect.Left + ActualPlotOffset, arrangeRect.Top, width, arrangeRect.Height);
                    }
                }
                else
                {
                    double height = Math.Max(0, arrangeRect.Height - (ActualPlotOffset * 2));
                    RenderedRect = new Rect(arrangeRect.Left, arrangeRect.Top + ActualPlotOffset, arrangeRect.Width, height);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the property path to be bind with axis label position.
        /// </summary>
        /// <remarks>While using custom label source, the position(index) for labels will get from this property.
        /// </remarks>
        public string PositionPath
        {
            get { return (string)GetValue(PositionPathProperty); }
            set { SetValue(PositionPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the property path to be bind with axis label content(text).
        /// </summary>
        /// <remarks>While using custom label source, the label text will get from this property.
        /// </remarks>
        public string ContentPath
        {
            get { return (string)GetValue(ContentPathProperty); }
            set { SetValue(ContentPathProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the label formatting for the axis labels.
        /// </summary>
        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom labels collection to be displayed in axis .
        /// </summary>
        public object LabelsSource
        {
            get { return (object)GetValue(LabelsSourceProperty); }
            set { SetValue(LabelsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the axis label postfix.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate PostfixLabelTemplate
        {
            get { return (DataTemplate)GetValue(PostfixLabelTemplateProperty); }
            set { SetValue(PostfixLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the axis label postfix.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate PrefixLabelTemplate
        {
            get { return (DataTemplate)GetValue(PrefixLabelTemplateProperty); }
            set { SetValue(PrefixLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the offset value for applying the padding to the plot area.
        /// </summary>
        public double PlotOffset
        {
            get { return (double)GetValue(PlotOffsetProperty); }
            set { SetValue(PlotOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the offset value for applying padding to the axis line.
        /// </summary>
        public double AxisLineOffset
        {
            get { return (double)GetValue(AxisLineOffsetProperty); }
            set { SetValue(AxisLineOffsetProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position for the axis labels. Either inside or outside of the plot area.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.AxisElementPosition"/>
        /// </value>
        public AxisElementPosition LabelsPosition
        {
            get { return (AxisElementPosition)GetValue(LabelsPositionProperty); }
            set { SetValue(LabelsPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value which decides the mechanism to avoid the axis labels overlapping. 
        /// The overlapping labels can be hided, rotated or placed on next row.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.AxisLabelsIntersectAction"/>
        /// </value>
        public AxisLabelsIntersectAction LabelsIntersectAction
        {
            get { return (AxisLabelsIntersectAction)GetValue(LabelsIntersectActionProperty); }
            set { SetValue(LabelsIntersectActionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the extension width for the axis label.
        /// </summary>
        public double LabelExtent
        {
            get { return (double)GetValue(LabelExtentProperty); }
            set { SetValue(LabelExtentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the rotation angle to the axis label content.
        /// </summary>
        /// <remarks>
        /// The default value is 0 and the label will be rotated with center as origin.
        /// </remarks>
        public double LabelRotationAngle
        {
            get { return (double)GetValue(LabelRotationAngleProperty); }
            set { SetValue(LabelRotationAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the axis line.
        /// </summary>
        public Style AxisLineStyle
        {
            get { return (Style)GetValue(AxisLineStyleProperty); }
            set { SetValue(AxisLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the axis to position opposite to its actual position. That is, to the other side of plot area.
        /// </summary>
        public bool OpposedPosition
        {
            get { return (bool)GetValue(OpposedPositionProperty); }
            set { SetValue(OpposedPositionProperty, value); }
        }

#if NETFX_CORE

        /// <summary>
        /// Gets or sets the interval for the axis auto range calculation, if <see cref="Interval"/> is not set explicitly.
        /// </summary>
        public object DesiredIntervalsCount
        {
            get { return (object)GetValue(DesiredIntervalsCountProperty); }
            set { SetValue(DesiredIntervalsCountProperty, value); }
        }

#else

        /// <summary>
        /// Gets or sets the interval for the axis auto range calculation, if <see cref="Interval"/> is not set explicitly.
        /// </summary>
        public int? DesiredIntervalsCount
        {
            get { return (int?)GetValue(DesiredIntervalsCountProperty); }
            set { SetValue(DesiredIntervalsCountProperty, value); }
        }

#endif

        /// <summary>
        /// Gets or sets visibility of label.
        /// </summary>
        public Visibility ThumbLabelVisibility
        {
            get { return (Visibility)GetValue(ThumbLabelVisibilityProperty); }
            set { SetValue(ThumbLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the scroll bar thumb.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate ThumbLabelTemplate
        {
            get { return (DataTemplate)GetValue(ThumbLabelTemplateProperty); }
            set { SetValue(ThumbLabelTemplateProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the header for the chart axis.
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the chart axis header.
        /// </summary>
        /// <value><see cref="Syncfusion.UI.Xaml.Charts.LabelStyle"/>
        /// </value>
        public LabelStyle HeaderStyle
        {
            get { return (LabelStyle)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the chart header.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size for the axis tick lines.
        /// </summary>
        /// <remarks>
        /// The default value is 6.
        /// </remarks>
        public double TickLineSize
        {
            get { return (double)GetValue(TickLineSizeProperty); }
            set { SetValue(TickLineSizeProperty, value); }
        }

        /// <summary>
        /// Gets the collection of axis labels in the visible region.
        /// </summary>
        public ObservableCollection<ChartAxisLabel> VisibleLabels
        {
            get { return m_VisibleLabels; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis should be reversed. 
        /// When reversed, the axis will render points from right to left if horizontal, top to bottom when vertical and clockwise if radial.
        /// </summary>
        public bool IsInversed
        {
            get { return (bool)GetValue(IsInversedProperty); }
            set { SetValue(IsInversedProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the origin value where its associated axis should place.
        /// </summary>
        public double Origin
        {
            get { return (double)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the origin line or not.
        /// </summary>
        public bool ShowOrigin
        {
            get { return (bool)GetValue(ShowOriginProperty); }
            set { SetValue(ShowOriginProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indcating whether the tick line position, either inside or outside.
        /// </summary>
        public AxisElementPosition TickLinesPosition
        {
            get { return (AxisElementPosition)GetValue(TickLinesPositionProperty); }
            set { SetValue(TickLinesPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether axis can be positioned across the plot area.
        /// </summary>
        public bool ShowAxisNextToOrigin
        {
            get { return (bool)GetValue(ShowAxisNextToOriginProperty); }
            set { SetValue(ShowAxisNextToOriginProperty, value); }
        }

        /// <summary>
        /// Gets or sets mode which decides the mechanism for extreme(edge) labels.
        /// It can be position center, hide, etc.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.EdgeLabelsDrawingMode"/>
        /// </value>
        public EdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get { return (EdgeLabelsDrawingMode)GetValue(EdgeLabelsDrawingModeProperty); }
            set { SetValue(EdgeLabelsDrawingModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the edge labels visibility mode for hiding the edge labels on zooming.
        /// </summary>
        public EdgeLabelsVisibilityMode EdgeLabelsVisibilityMode
        {
            get { return (EdgeLabelsVisibilityMode)GetValue(EdgeLabelsVisibilityModeProperty); }
            set { SetValue(EdgeLabelsVisibilityModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the major grid lines.
        /// </summary>
        public Style MajorGridLineStyle
        {
            get { return (Style)GetValue(MajorGridLineStyleProperty); }
            set { SetValue(MajorGridLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the minor grid lines.
        /// </summary>
        public Style MinorGridLineStyle
        {
            get { return (Style)GetValue(MinorGridLineStyleProperty); }
            set { SetValue(MinorGridLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the major tick line style.
        /// </summary>
        public Style MajorTickLineStyle
        {
            get { return (Style)GetValue(MajorTickLineStyleProperty); }
            set { SetValue(MajorTickLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the minor tick line style.
        /// </summary>
        public Style MinorTickLineStyle
        {
            get { return (Style)GetValue(MinorTickLineStyleProperty); }
            set { SetValue(MinorTickLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for origin line when enable the ShowOrigin property.
        /// </summary>
        public Style OriginLineStyle
        {
            get { return (Style)GetValue(OriginLineStyleProperty); }
            set { SetValue(OriginLineStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show track ball label for this axis.
        /// </summary>
        public bool ShowTrackBallInfo
        {
            get { return (bool)GetValue(ShowTrackBallInfoProperty); }
            set { SetValue(ShowTrackBallInfoProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the trackball tooltip label.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate TrackBallLabelTemplate
        {
            get { return (DataTemplate)GetValue(TrackBallLabelTemplateProperty); }
            set { SetValue(TrackBallLabelTemplateProperty, value); }
        }


        /// <summary>
        /// Gets or sets the custom template for the Crosshair labels.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        /// <example>
        /// <code language="XAML">
        ///     &lt;syncfusion:SfChart.PrimaryAxis&gt;
        ///         &lt;syncfusion:ChartAxis CrosshairLabelTemplate="{StaticResource crossHairTemplateX}"/&gt;
        ///     &lt;/syncfusion:SfChart.PrimaryAxis&gt;
        ///     &lt;syncfusion:SfChart.SecondaryAxis&gt;
        ///         &lt;syncfusion:ChartAxis CrosshairLabelTemplate="{StaticResource crossHairTemplateY}"/&gt;
        ///     &lt;/syncfusion:SfChart.SecondaryAxis&gt;
        /// </code>
        /// <code language="C#">
        ///     primaryAxis.ShowTrackBallInfo = true;
        ///     primaryAxis.CrosshairLabelTemplate = dataTemplateX;
        ///     secondaryAxis.ShowTrackBallInfo= true;
        ///     secondaryAxis.CrosshairLabelTemplate = dataTemplateY;
        /// </code>
        /// </example>
        public DataTemplate CrosshairLabelTemplate
        {
            get { return (DataTemplate)GetValue(CrosshairLabelTemplateProperty); }
            set { SetValue(CrosshairLabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis gird lines can be display or not.
        /// </summary>
        public bool ShowGridLines
        {
            get { return (bool)GetValue(ShowGridLinesProperty); }
            set { SetValue(ShowGridLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the auto interval calculation while zooming.
        /// </summary>
        /// <remarks>While zooming, the range and interval will change with respect to zoom position or zoom factor
        /// </remarks>
        public bool EnableAutoIntervalOnZooming
        {
            get { return (bool)GetValue(EnableAutoIntervalOnZoomingProperty); }
            set { SetValue(EnableAutoIntervalOnZoomingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style for the axis labels.
        /// </summary>
        public LabelStyle LabelStyle
        {
            get { return (LabelStyle)GetValue(LabelStyleProperty); }
            set { SetValue(LabelStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the custom template for the axis labels.
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        /// <summary>
        /// Gets the axis custom labels collection.
        /// </summary>
        public ChartAxisLabelCollection CustomLabels
        {
            get
            {
                if (m_customLabels == null)
                {
                    m_customLabels = new ChartAxisLabelCollection();
                    m_customLabels.CollectionChanged += OnCustomLabelsCollectionChanged;
                }
                return m_customLabels;
            }
        }

        #endregion

        #region Internal Properties
        
        internal Size ComputedDesiredSize { get; set; }

        internal double VisibleInterval { get; set; }

        internal double ActualInterval { get; set; }

        internal bool IsRangeCalculating { get; set; }

        internal DoubleRange ActualRange
        {
            get { return m_actualRange; }
            set { m_actualRange = value; }
        }

        internal Rect RenderedRect
        {
            get
            {
                return renderedRect;
            }

            set
            {
                Rect oldRect = renderedRect;
                renderedRect = value;
                OnAxisBoundsChanged(new ChartAxisBoundsEventArgs() { NewBounds = value, OldBounds = oldRect });
            }
        }

        internal Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a collection of the ChartAxisRangeStyle to customize the axis gridlines.
        /// </summary>
        public ChartAxisRangeStyleCollection RangeStyles
        {
            get { return (ChartAxisRangeStyleCollection)GetValue(RangeStylesProperty); }
            set { SetValue(RangeStylesProperty, value); }
        }

        internal ChartValueType ValueType
        {
            get { return (ChartValueType)GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }

        internal DataTemplate ActualTrackBallLabelTemplate
        {
            get { return (DataTemplate)GetValue(ActualTrackBallLabelTemplateProperty); }
            set { SetValue(ActualTrackBallLabelTemplateProperty, value); }
        }

        internal Visibility AxisVisibility
        {
            get { return (Visibility)GetValue(AxisVisibilityProperty); }
            set { SetValue(AxisVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Alignment for the axis labels. Either Start, Center or End of the Axis area.
        /// </summary>
        /// <value>
        /// <see cref="Syncfusion.UI.Xaml.Charts.LabelAlignment"/>
        /// </value>
        internal LabelAlignment AxisLabelAlignment
        {
            get { return (LabelAlignment)GetValue(AxisLabelAlignmentProperty); }
            set { SetValue(AxisLabelAlignmentProperty, value); }
        }

        internal ObservableCollection<ISupportAxes> RegisteredSeries
        {
            get
            {
                if (registeredSeries == null)
                {
                    registeredSeries = new ObservableCollection<ISupportAxes>();
                    associatedAxes = new List<ChartAxis>();
                    registeredSeries.CollectionChanged += OnRegisteredSeriesCollectionChanged;
                }
                return registeredSeries;
            }
        }

        internal List<ChartAxis> AssociatedAxes
        {
            get { return associatedAxes; }
            set { associatedAxes = value; }
        }

        internal bool IsLabelRotateRequired { get; set; }
        
        internal bool IsDefault { get; set; }

        internal bool IsScrolling { get; set; }

        internal double InsidePadding { get; set; }

        internal ChartBase Area { get; set; }

        internal List<double> SmallTickPoints
        {
            get { return m_smalltickPoints; }
        }

        internal Size AvailableSize { get; set; }

        /// <summary>
        /// Gets or sets the stripLine range corresponding to x axis, this property is used to include range of StripLine to axis.
        /// </summary>
        internal DoubleRange StriplineXRange { get; set; }

        /// <summary>
        /// Gets or sets the stripLine range corresponding to y axis, this property is used to include range of StripLine to axis.
        /// </summary>
        internal DoubleRange StriplineYRange { get; set; }

        internal ValueToCoefficientHandler ValueToCoefficientCalc { get; set; }

        internal ValueToCoefficientHandler CoefficientToValueCalc { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Public Virtual Methods

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToCoefficientCalc(double)"/>
        public virtual double CoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            result = VisibleRange.Start + VisibleRange.Delta * value;

            return result;
        }

        /// <summary>
        /// Converts Coefficient of Value related to chart control to Polar/Radar type axis unit.
        /// </summary>
        /// <param name="value"> Polar/Radar type axis Coefficient Value</param>
        /// <returns> The value of point on Polar/Radar type axis</returns>
        public virtual double PolarCoefficientToValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? value : 1d - value;

            value /= 1 - 1 / (VisibleRange.Delta + 1);

            result = VisibleRange.Start + VisibleRange.Delta * value;

            return result;
        }

        /// <summary>
        /// Converts co-ordinate of point related to chart control to axis units. It returns actual value instead of visible value.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        public virtual double CoefficientToActualValue(double value)
        {
            double result = double.NaN;

            value = this.IsInversed ? 1d - value : value;

            result = ActualRange.Start + ActualRange.Delta * value;

            return result;
        }

        /// <summary>
        /// Converts value of passed point co-ordinate to control related co-ordinate.
        /// </summary>
        /// <param name="value">The value of point on axis.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.CoefficientToValueCalc"/>      
        public virtual double ValueToCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;
            if (delta == 0)
                return -1;
            result = (value - start) / delta;

            return this.isInversed ? 1d - result : result;
        }

        /// <summary>
        /// Converts co-ordinate of point related to chart control to Polar/Radar type axis unit.
        /// </summary>
        /// <param name="value">The absolute point value.</param>
        /// <returns>The value of point on axis.</returns>
        /// <seealso cref="ChartAxis.ValueToPolarCoefficient"/>      
        public virtual double ValueToPolarCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;
            if (VisibleLabels.Count > 0)
                result *= 1 - 1 / (double)VisibleLabels.Count;//WRT-2476-Polar and radar series segments are plotted before the Actual position
            else
                result *= 1 - 1 / (delta + 1);

            return this.isInversed ? result : 1d - result;
        }

        /// <summary>
        /// Converts value of passed point co-ordinate to control related co-ordinate.
        /// </summary>
        /// <param name="value">The value of point on axis.</param>
        /// <param name="isInversed">The value indicates whether <see cref="ChartAxis.IsInversed"/> is e/></param>
        /// <returns>Co-ordinate of point related to chart control.</returns>
        /// <seealso cref="ChartAxis.CoefficientToValueCalc"/>      
        public virtual double ValueToCoefficient(double value, bool isInversed)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;
            return isInversed ? 1d - result : result;
        }

        /// <summary>
        /// Return Object value from the given position value
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual object GetLabelContent(double position)
        {
            if ((CustomLabels.Count > 0 || LabelsSource != null))
            {
                return GetCustomLabelContent(position) ?? GetActualLabelContent(position);
            }
            else
            {
                return GetActualLabelContent(position);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the actual rect co-ordinates of an ChartAxis.
        /// </summary>
        /// <returns>returns rect</returns>      
        public Rect GetRenderedRect()
        {
            return RenderedRect;
        }

        /// <summary>
        /// Gets the rect co-ordinates of an axis excluding its value of LabelOffset and AxisLineOffset.
        /// </summary>
        /// <returns>returns rect</returns>      
        public Rect GetArrangeRect()
        {
            return ArrangeRect;
        }

        /// <summary>
        /// Clone the axis
        /// </summary>
        /// <returns></returns>
        public DependencyObject Clone()
        {
            return CloneAxis(null);
        }

        #endregion
        
        #region Internal Static Methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
                if (source is CollectionViewSource)
                {
                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
                        result = GetSourceList(cvs.View.CollectionGroups);
                    }
                }
                else if (source is ICollectionView)
                {
                    var sourceList = ((ICollectionView)source).CollectionGroups;
                    result = GetSourceList(sourceList);
                }
                else
                {
                    result = source as IEnumerable;
                }
            }

            return result;
        }

        #endregion

        #region Internal Virtual Methods

        internal virtual void ComputeDesiredSize(Size size)
        {

        }

        internal virtual void CreateLineRecycler()
        {

        }
        
        internal virtual object GetActualLabelContent(double position)
        {
            return Math.Round(position, CRoundDecimals).ToString(this.LabelFormat, CultureInfo.CurrentCulture);
        }

        internal virtual void Dispose()
        {
            DisposeCustomLabels();
            DisposeEvents();
            DisposeRegisteredSeries();
            DisposeVisibleLabels();
            AssociatedAxes.Clear();
            LabelStyle = null;
            Area = null;           
        }

#endregion

        #region Internal Methods

        internal double PixelToCoefficientValue(double value)
        {
            double actualSize = (Orientation == Orientation.Horizontal) ? renderedRect.Width : renderedRect.Height;
            return (value * (VisibleRange.End - VisibleRange.Start)) / actualSize;
        }

        /// <summary>
        /// Occurs when label is created.
        /// </summary>
        /// <param name="args"></param>
        internal void OnLabelCreated(LabelCreatedEventArgs args)
        {
            if (LabelCreated != null && args != null)
            {
                LabelCreated(this, args);
            }
        }

        internal object GetCustomLabelContent(double position)
        {
            string customLabelContent = null;
            foreach (var label in VisibleLabels)
            {
                if (label.Position == position)
                {
                    customLabelContent = label.LabelContent.ToString();
                }
            }

            return customLabelContent;
        }

        /// <summary>
        /// Calculates actual range and actual interval
        /// </summary>
        /// <param name="availableSize"></param>
        internal void CalculateRangeAndInterval(Size availableSize)
        {
            var axis2D = this as ChartAxisBase2D;
            var isAutoScrollEnabled = axis2D != null && !(this is TimeSpanAxis) && !double.IsNaN(axis2D.AutoScrollingDelta);

            if(!IsScrolling)
            {
                DoubleRange range = CalculateActualRange();

                if (range.IsEmpty)
                {
                    range = new DoubleRange(0, 1);
                    IsDefaultRange = true;
                }
                else
                    IsDefaultRange = false;

                if (range.Start == range.End)
                    range = new DoubleRange(range.Start, range.End + 1);

                ActualInterval = CalculateActualInterval(range, availableSize);

                ActualRange = ApplyRangePadding(range, ActualInterval);

                if (isAutoScrollEnabled && axis2D.AutoScrollingDelta >= 0 && axis2D.CanAutoScroll)
                {
                    axis2D.UpdateAutoScrollingDelta(axis2D.AutoScrollingDelta);
                    axis2D.CanAutoScroll = false;
                }              
            }
            else if (isAutoScrollEnabled)
            {
                CalculateActualRange();
            }

            if (ActualRangeChanged != null)
                ApplyCustomVisibleRange(availableSize);
            else
                CalculateVisibleRange(availableSize);
        }

        /// <summary>
        /// Recalculates visible range and visible labels.
        /// </summary>
        internal void Invalidate()
        {
            if (RegisteredSeries.Count > 0
                && RegisteredSeries[0] is PolarRadarSeriesBase)
            {
                CalculateRangeAndInterval(AvailableSize);
                UpdateLabels();
            }
        }

        internal void UpdateLabels()
        {
            if (MaximumLabels <= 0)
                return;
            if (VisibleRange.Delta > 0)
            {
                VisibleLabels.Clear();
                m_smalltickPoints.Clear();

                if (this.CustomLabels.Count > 0)
                {
                    PopulateVisibleLabelForCustomLabels();
                }
                else if (this.LabelsSource != null)
                {
                    PopulateVisibleLabelsForLabelSource();
                }
                else
                {
                    GenerateVisibleLabels();
                    if (isInversed)
                    {
                        if (m_VisibleLabels != null)
                            m_VisibleLabels.CollectionChanged -= m_VisibleLabels_CollectionChanged;
                        m_VisibleLabels = new ObservableCollection<ChartAxisLabel>(VisibleLabels.Reverse());
                        m_VisibleLabels.CollectionChanged += m_VisibleLabels_CollectionChanged;
                    }

                }

                if (axisLabelsPanel != null)
                {
                    if (axisElementsPanel != null)
                        axisElementsPanel.UpdateElements();
                    axisLabelsPanel.UpdateElements();
                    if (axisMultiLevelLabelsPanel != null)
                        axisMultiLevelLabelsPanel.UpdateElements();
                }
                else
                {
                    axisElementsUpdateRequired = true;
                }
            }
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        internal double ValueToPoint(double value)
        {
            if (Area != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    return RenderedRect.Left
                        + (ValueToCoefficientCalc(value) * RenderedRect.Width);
                }
                return RenderedRect.Top + (1 - ValueToCoefficientCalc(value)) * RenderedRect.Height;
            }
            return double.NaN;
        }

        /// <summary>
        /// Converts point to value.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The double point to value</returns>
        internal double PointToValue(Point point)
        {
            if (Area != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    return CoefficientToValueCalc((point.X - (RenderedRect.Left)) / RenderedRect.Width);
                }

                return CoefficientToValueCalc(1d - ((point.Y - (RenderedRect.Top)) / RenderedRect.Height));
            }

            return double.NaN;
        }

        internal void OnVisibleRangeChanged(object sender, VisibleRangeChangedEventArgs e)
        {
            if (registeredSeries != null)
            {
                foreach (var series in registeredSeries)
                {
                    var cartesianSeries = series as CartesianSeries;
                    if (cartesianSeries != null)
                        cartesianSeries.OnVisibleRangeChanged(sender, e);
                }
            }
        }

        #endregion
        
        #region Protected Internal Virtual Methods

        protected internal virtual void OnAxisBoundsChanged(ChartAxisBoundsEventArgs args)
        {
            if (AxisBoundsChanged != null && args != null)
            {
                AxisBoundsChanged(this, args);
            }
        }

        protected internal virtual void OnAxisVisibleRangeChanged(VisibleRangeChangedEventArgs args)
        {
            if (VisibleRangeChanged != null && args != null)
                VisibleRangeChanged(this, args);
        }

        /// <summary>
        /// Method implementation for Add SamllTicksPoint
        /// </summary>
        /// <param name="position"></param>
        protected internal virtual void AddSmallTicksPoint(double position)
        {
        }

        /// <summary>
        /// Method implementation for Add smallTicks to axis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="interval"></param>
        protected internal virtual void AddSmallTicksPoint(double position, double interval)
        {
        }

        /// <summary>
        /// Calculates actual interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal virtual double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            return 1.0;
        }

        /// <summary>
        /// Calculates nice interval
        /// </summary>
        /// <param name="actualRange"></param>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected internal virtual double CalculateNiceInterval(DoubleRange actualRange, Size availableSize)
        {
            double delta = actualRange.Delta;

            double actualDesiredIntervalsCount = GetActualDesiredIntervalsCount(availableSize);

            double niceInterval = delta / actualDesiredIntervalsCount;

            if (DesiredIntervalsCount != null)
                return niceInterval;

            double minInterval = Math.Pow(10, Math.Floor(Math.Log10(niceInterval)));

            foreach (int mul in c_intervalDivs)
            {
                double currentInterval = minInterval * mul;
                if (actualDesiredIntervalsCount < (delta / currentInterval))
                {
                    break;
                }

                niceInterval = currentInterval;
            }

            if (niceInterval <= 10 && actualRange.Start < 0 && this.RegisteredSeries.Count > 0 && RegisteredSeries.All(series => series is ChartSeriesBase && (series as ChartSeriesBase).IsStacked100))
                niceInterval = 20;
            return niceInterval;
        }

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal virtual void CalculateVisibleRange(Size availableSize)
        {
            VisibleRange = ActualRange;
            VisibleInterval = ActualInterval;
        }

        #endregion

        #region Protected Internal Methods
        
        /// <summary>
        /// Returns the maximum desired intervals count.
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected internal double GetActualDesiredIntervalsCount(Size availableSize)
        {
            double size = Orientation == Orientation.Horizontal
                ? availableSize.Width : availableSize.Height;

            double actualDesiredIntervalsCount = DesiredIntervalsCount != null
#if NETFX_CORE
 ? Convert.ToInt32(DesiredIntervalsCount)
#else
 ? DesiredIntervalsCount.Value
#endif
 : 0;

            if (DesiredIntervalsCount == null)
            {
                double adjustedDesiredIntervalsCount = (Orientation == Orientation.Horizontal ? 0.54 : 1.0) *
                                                              MaximumLabels;
                actualDesiredIntervalsCount = Math.Max(size * adjustedDesiredIntervalsCount / MaxPixelsCount, 1.0);
            }

            return actualDesiredIntervalsCount;
        }

        #endregion

        #region Protected Virtual Methods

        protected virtual void OnPropertyChanged()
        {
            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }
        }

        /// <summary>
        /// Method implementation for Generate Labels in ChartAxis
        /// </summary>
        protected virtual void GenerateVisibleLabels()
        {
            double interval = VisibleInterval;
            double position = VisibleRange.Start - (VisibleRange.Start % ActualInterval);

            for (; position <= VisibleRange.End; position += interval)
            {
                if (VisibleRange.Inside(position))
                {
                    VisibleLabels.Add(new ChartAxisLabel(position, GetActualLabelContent(position), position));
                }
            }
        }

        internal void SetRangeForAxisStyle()
        {
            if (RangeStyles != null && RangeStyles.Count > 0)
            {
                foreach (var item in RangeStyles)
                {
                    if (item.Start == null && item.End == null)
                    {
                        item.Range = DoubleRange.Empty;
                    }
                    else
                    {
                        double start = item.Start == null ? VisibleRange.Start : ChartDataUtils.ObjectToDouble(item.Start);
                        double end = item.End == null ? VisibleRange.End : ChartDataUtils.ObjectToDouble(item.End);
                        item.Range = new DoubleRange(start, end);
                    }
                }
            }
        }

        /// <summary>
        /// Calculates actual range
        /// </summary>
        /// <returns></returns>
        protected virtual DoubleRange CalculateActualRange()
        {
            if (Area != null)
            {
                var technicalIndicators = new List<ChartSeriesBase>();

                if (Area is SfChart && (Area as SfChart).TechnicalIndicators != null)
                {
                    foreach (ChartSeries indicator in (Area as SfChart).TechnicalIndicators)
                    {
                        technicalIndicators.Add(indicator as ChartSeriesBase);
                    }
                }

                var seriesRange = (Area is SfChart ? Area.VisibleSeries.Union(technicalIndicators).OfType<ISupportAxes>() :
                    Area.VisibleSeries.OfType<ISupportAxes>())
                    .Select
                    (
                        series =>
                        {
                            if (series.ActualXAxis == this)
                                return series.XRange;
                            if (series.ActualYAxis == this)
                                return series.YRange;

                            var xyzDataSeries3D = series as XyzDataSeries3D;
                            if (xyzDataSeries3D != null && xyzDataSeries3D.ActualZAxis == this)
                                return xyzDataSeries3D.ZRange;
                            return DoubleRange.Empty;
                        }
                    ).Sum();

                var chartAxisBase2D = this as ChartAxisBase2D;
                if (chartAxisBase2D != null && chartAxisBase2D.IncludeStripLineRange)
                {
                    if (this.Orientation == Orientation.Horizontal)
                        return StriplineXRange + seriesRange;
                    if (this.Orientation == Orientation.Vertical)
                        return StriplineYRange + seriesRange;
                }

                return seriesRange;
            }

            return DoubleRange.Empty;
        }

        /// <summary>
        /// Apply padding based on interval
        /// </summary>
        /// <param name="range"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        protected virtual DoubleRange ApplyRangePadding(DoubleRange range, double interval)
        {
            if (RegisteredSeries.Count > 0 && RegisteredSeries[0] is PolarRadarSeriesBase)
            {
                double minimum = Math.Floor(range.Start / interval) * interval;
                double maximum = Math.Ceiling(range.End / interval) * interval;
                return new DoubleRange(minimum, maximum);
            }

            return range;
        }

        protected virtual DependencyObject CloneAxis(DependencyObject obj)
        {
            ChartAxis newAxis = obj as ChartAxis;
            ChartCloning.CloneControl(this, newAxis);
            SfChart.SetRow(newAxis, SfChart.GetRow(this));
            SfChart.SetColumn(newAxis, SfChart.GetRow(this));
            newAxis.ContentPath = this.ContentPath;
            newAxis.Header = this.Header;
            newAxis.HeaderTemplate = this.HeaderTemplate;
            newAxis.HeaderStyle = this.HeaderStyle;
            newAxis.HeaderPosition = this.HeaderPosition;
            newAxis.IsInversed = this.IsInversed;
            newAxis.LabelExtent = this.LabelExtent;
            newAxis.LabelFormat = this.LabelFormat;
            newAxis.LabelRotationAngle = this.LabelRotationAngle;
            newAxis.LabelsIntersectAction = this.LabelsIntersectAction;
            newAxis.LabelsPosition = this.LabelsPosition;
            newAxis.LabelsSource = this.LabelsSource;
            newAxis.LabelTemplate = this.LabelTemplate;
            newAxis.LabelStyle = this.LabelStyle;
            newAxis.MajorGridLineStyle = this.MajorGridLineStyle;
            newAxis.MajorTickLineStyle = this.MajorTickLineStyle;
            newAxis.MinorGridLineStyle = this.MinorGridLineStyle;
            newAxis.MinorTickLineStyle = this.MinorTickLineStyle;
            newAxis.OriginLineStyle = this.OriginLineStyle;
            newAxis.MaximumLabels = this.MaximumLabels;
            newAxis.OpposedPosition = this.OpposedPosition;
            newAxis.PositionPath = this.PositionPath;
            newAxis.PostfixLabelTemplate = this.PostfixLabelTemplate;
            newAxis.PrefixLabelTemplate = this.PrefixLabelTemplate;
            newAxis.ShowAxisNextToOrigin = this.ShowAxisNextToOrigin;
            newAxis.ShowGridLines = this.ShowGridLines;
            newAxis.ShowOrigin = this.ShowOrigin;
            newAxis.TickLineSize = this.TickLineSize;
            newAxis.EnableAutoIntervalOnZooming = this.EnableAutoIntervalOnZooming;
            newAxis.ShowTrackBallInfo = this.ShowTrackBallInfo;
            newAxis.TrackBallLabelTemplate = this.TrackBallLabelTemplate;
            newAxis.ActualTrackBallLabelTemplate = this.ActualTrackBallLabelTemplate;
            newAxis.EdgeLabelsDrawingMode = this.EdgeLabelsDrawingMode;
            newAxis.EdgeLabelsVisibilityMode = this.EdgeLabelsVisibilityMode;
            newAxis.TickLinesPosition = this.TickLinesPosition;
            newAxis.Origin = this.Origin;
            newAxis.DesiredIntervalsCount = this.DesiredIntervalsCount;
            newAxis.Orientation = this.Orientation;
            newAxis.AxisLineStyle = this.AxisLineStyle;
            newAxis.AxisLineOffset = this.AxisLineOffset;
            newAxis.CrosshairLabelTemplate = this.CrosshairLabelTemplate;
            newAxis.HeaderPosition = this.HeaderPosition;
            newAxis.ThumbLabelTemplate = this.ThumbLabelTemplate;
            newAxis.ThumbLabelVisibility = this.ThumbLabelVisibility;

            return newAxis;
        }

        protected virtual void OnRegisteredSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ISupportAxes series = null;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                series = e.NewItems[0] as ISupportAxes;
                if (series == null) return;

                ChartAxis yAxis = series.ActualYAxis;

                ChartAxis xAxis = series.ActualXAxis;

                if (xAxis != null && yAxis != null)
                {
                    if (xAxis.associatedAxes != null && !xAxis.associatedAxes.Contains(yAxis))
                    {
                        CheckAxes(xAxis);
                        xAxis.associatedAxes.Add(yAxis);
                    }

                    if (yAxis.associatedAxes != null && !yAxis.associatedAxes.Contains(xAxis))
                    {
                        CheckAxes(yAxis);
                        yAxis.associatedAxes.Add(xAxis);
                    }

                    var chartAxisBase3D = this as ChartAxisBase3D;
                    if (chartAxisBase3D != null && chartAxisBase3D.IsZAxis)
                    {
                        ChartAxis zAxis = this;

                        if (zAxis.associatedAxes != null && !zAxis.associatedAxes.Contains(yAxis))
                        {
                            CheckAxes(zAxis);

                            if (xAxis.Orientation == Orientation.Vertical)
                                zAxis.associatedAxes.Add(xAxis);
                            else
                                zAxis.associatedAxes.Add(yAxis);
                        }
                    }
                }

                ScheduleCheck();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                series = e.OldItems[0] as ISupportAxes;
                if (series == null) return;
                var yAxis = series.ActualYAxis;
                var xAxis = series.ActualXAxis;
                if (Area != null && xAxis != null && yAxis != null && xAxis.associatedAxes != null && yAxis.associatedAxes != null)
                {
                    if (Area is SfChart3D || series is FinancialTechnicalIndicator
                        || xAxis.Equals(Area.InternalPrimaryAxis) || yAxis.Equals(Area.InternalSecondaryAxis))
                    {
                        if (!Area.Axes.Contains(yAxis))
                            xAxis.associatedAxes.Remove(yAxis);
                        if (!Area.Axes.Contains(xAxis))
                            yAxis.associatedAxes.Remove(xAxis);
                    }
                    else
                    {
                        xAxis.associatedAxes.Remove(yAxis);
                        yAxis.associatedAxes.Remove(xAxis);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (associatedAxes != null)
                    associatedAxes.Clear();

                if (Area != null)
                {
                    if (Area is SfChart)
                    {
                        ChartSeriesCollection seriesCollection = (Area as SfChart).Series;
                        ObservableCollection<ChartSeries> technicalIndicatorCollection = (Area as SfChart).TechnicalIndicators;
                        //When SeriesCollection is replaced with another collection then associated should be cleared.
                        if (seriesCollection == null || seriesCollection.Count == 0)
                        {
                            if (Area.InternalPrimaryAxis != null
                                && Area.InternalPrimaryAxis.AssociatedAxes != null)
                                Area.InternalPrimaryAxis.AssociatedAxes.Clear();

                            if (Area.InternalSecondaryAxis != null
                                && Area.InternalSecondaryAxis.AssociatedAxes != null)
                                Area.InternalSecondaryAxis.AssociatedAxes.Clear();
                        }
                        else
                        {
                            foreach (ChartSeries chartSeries in seriesCollection)
                                RemoveAssociatedAxis(chartSeries.ActualXAxis, chartSeries.ActualYAxis);
                        }

                        if (technicalIndicatorCollection != null)
                            foreach (ChartSeries chartSeries in technicalIndicatorCollection)
                                RemoveAssociatedAxis(chartSeries.ActualXAxis, chartSeries.ActualYAxis);
                    }
                    else
                    {
                        ChartSeries3DCollection seriesCollection = (Area as SfChart3D).Series;

                        if (seriesCollection != null)
                            foreach (ChartSeries3D chartSeries in seriesCollection)
                                RemoveAssociatedAxis(chartSeries.ActualXAxis, chartSeries.ActualYAxis);
                    }
                }
            }
        }

        #endregion

        #region Protected Override Methods
        
        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Area != null && Area.VisibleSeries.Count > 0 && Area.VisibleSeries[0] is AccumulationSeriesBase)
                return new Size(0, 0);

            base.MeasureOverride(availableSize);

            return new Size(ArrangeRect.Width, ArrangeRect.Height);
        }

        #endregion

        #region Private Static Methods

        private static void OnHeaderPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
                axis.Area.ScheduleUpdate();
        }
        
        private static void OnAxisOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = d as ChartAxis;
            chartAxis.ScheduleCheck();
            chartAxis.OnPropertyChanged();
        }

        private static void RangeStylesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).RangeStylesChanged(e);
        }

        internal void RangeStylesChanged(DependencyPropertyChangedEventArgs args)
        {
            ChartAxisRangeStyleCollection oldRangeStyles = args.OldValue as ChartAxisRangeStyleCollection;
            ChartAxisRangeStyleCollection newRangeStyles = args.NewValue as ChartAxisRangeStyleCollection;
            if (oldRangeStyles != null)
                oldRangeStyles.CollectionChanged -= RangeStyles_CollectionChanged;

            if (newRangeStyles != null)
                newRangeStyles.CollectionChanged += RangeStyles_CollectionChanged;
			
			OnPropertyChanged();
        }

        private void RangeStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged();
        }

        private static void OnOpposedPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as ChartAxisBase2D;
            if (axis != null)
            {
#if NETFX_CORE || WPF
#if NETFX_CORE
                //For IR 17848 -  	Designer crashes when rebuild the SfChart
                if (!DesignMode.DesignModeEnabled)
#endif
                    axis.ChangeStyle(axis.EnableTouchMode);
#endif
            }

            (d as ChartAxis).OnPropertyChanged();
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).OnPropertyChanged();
        }

        private static void OnIsInversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).OnIsInversedChanged(e);
        }

        private static void OnShowGridLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxis).OnShowGridLines((bool)e.NewValue);
        }

        private static void OnShowAxisNextToOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis.Area != null)
            {
                axis.Area.ScheduleUpdate();
            }
        }

        private static void OnLabelRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            axis.IsLabelRotateRequired = true;
            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        private static void OnValuetypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis;
            if (axis.ValueType == ChartValueType.Logarithmic)
            {
                axis.IsLogarithmic = true;
            }
        }

        private static void OnDesiredIntervalsCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxis axis = d as ChartAxis; // WPF- 17871 DesiredIntervalCount property dynamically not set
            if (axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        /// <summary>
        /// Iterates the XMLNodes to poulate the collection in a IList source
        /// </summary>
        /// <param name="itemsSource"></param>
        /// <param name="source"></param>
        private static void CreateXmlSourceListWrapper(IXmlNode itemsSource, ref IList source)
        {
            IXmlNode xmlData = itemsSource as IXmlNode;
            if (xmlData != null)
            {
                source.Add(xmlData);
                CreateXmlSourceListWrapper(xmlData.NextSibling, ref source);
            }
        }
        #endregion

        #region Private Methods
        
        private void DisposeVisibleLabels()
        {
            if (m_VisibleLabels != null)
            {
                foreach (var visibleLabel in m_VisibleLabels)
                {
                    visibleLabel.ChartAxis = null;
                    visibleLabel.LabelContent = null;
                    visibleLabel.LabelStyle = null;
                    visibleLabel.PrefixLabelTemplate = null;
                    visibleLabel.PostfixLabelTemplate = null;
                }

                m_VisibleLabels.CollectionChanged -= m_VisibleLabels_CollectionChanged;
                m_VisibleLabels.Clear();
                m_VisibleLabels = null;
            }
        }

        private void DisposeEvents()
        {
            if (this.VisibleRangeChanged != null)
            {
                foreach (var handler in VisibleRangeChanged.GetInvocationList())
                {
                    this.VisibleRangeChanged -= handler as EventHandler<VisibleRangeChangedEventArgs>;
                }

                this.VisibleRangeChanged = null;
            }

            if (this.LabelCreated != null)
            {
                foreach (var handler in LabelCreated.GetInvocationList())
                {
                    this.LabelCreated -= handler as EventHandler<LabelCreatedEventArgs>;
                }

                this.LabelCreated = null;
            }

            if (this.AxisBoundsChanged != null)
            {
                foreach (var handler in AxisBoundsChanged.GetInvocationList())
                {
                    this.AxisBoundsChanged -= handler as EventHandler<ChartAxisBoundsEventArgs>;
                }

                this.AxisBoundsChanged = null;
            }

            if (ActualRangeChanged != null)
            {
                foreach (var handler in ActualRangeChanged.GetInvocationList())
                {
                    this.ActualRangeChanged -= handler as EventHandler<ActualRangeChangedEventArgs>;
                }

                ActualRangeChanged = null;
            }
        }

        private void DisposeCustomLabels()
        {
            if (m_customLabels != null)
            {
                m_customLabels.CollectionChanged -= OnCustomLabelsCollectionChanged;
                m_customLabels.Clear();
            }
        }

        private void DisposeRegisteredSeries()
        {
            if (registeredSeries != null)
            {
                registeredSeries.CollectionChanged -= OnRegisteredSeriesCollectionChanged;
                registeredSeries.Clear();
            }
        }

        private void OnIsInversedChanged(DependencyPropertyChangedEventArgs e)
        {
            isInversed = this.IsInversed;
            if (this.Area != null)
            {
                this.Area.ScheduleUpdate();
            }
        }

        private void OnShowGridLines(bool value)
        {
            if (Area != null && Area.GridLinesLayout != null)
            {
                if (this is ChartAxisBase3D)
                {
                    if (!value && GridLinesRecycler != null)
                    {
                        for (int i = 0; i < GridLinesRecycler.Count; i++)
                            (GridLinesRecycler[i] as Line).Visibility = Visibility.Collapsed;
                    }
                    else if (value && VisibleLabels.Count > 0)
                    {
                        for (int i = 0; i < GridLinesRecycler.Count; i++)
                            (GridLinesRecycler[i] as Line).Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (!value && GridLinesRecycler != null)
                    {
                        GridLinesRecycler.Clear();
                    }
                    else if (value && VisibleLabels.Count > 0)
                    {
                        if (!double.IsInfinity(Area.AvailableSize.Height) && !double.IsInfinity(Area.AvailableSize.Width))
                        {
                            Area.GridLinesLayout.UpdateElements();
                            Area.GridLinesLayout.Measure(Area.AvailableSize);
                            Area.GridLinesLayout.Arrange(Area.AvailableSize);
                        }
                    }
                }
            }
        }

        private void ApplyCustomVisibleRange(Size availableSize)
        {
            var rangeChangeArgs = new ActualRangeChangedEventArgs(this) { IsScrolling = IsScrolling, ActualMinimum = ActualRange.Start, ActualMaximum = ActualRange.End, ActualInterval = ActualInterval };
            ActualRangeChanged(this, rangeChangeArgs);

            var customActualRange = rangeChangeArgs.GetActualRange();
            if (customActualRange != ActualRange)
            {
                ActualRange = customActualRange;
                ActualInterval = CalculateActualInterval(customActualRange, availableSize);
            }

            var visibleRange = rangeChangeArgs.GetVisibleRange();
            if (visibleRange.IsEmpty)
            {
                CalculateVisibleRange(availableSize);
            }
            else
            {
                VisibleRange = visibleRange;
                VisibleInterval = EnableAutoIntervalOnZooming
                                        ? CalculateNiceInterval(VisibleRange, availableSize)
                                        : ActualInterval;

                var chartAxisBase2D = this as ChartAxisBase2D;

                if (chartAxisBase2D != null)
                {
                    chartAxisBase2D.ZoomPosition = (VisibleRange.Start - ActualRange.Start) / ActualRange.Delta;
                    chartAxisBase2D.ZoomFactor = (VisibleRange.End - VisibleRange.Start) / ActualRange.Delta;
                }
            }
        }

        /// <summary>
        /// Called when the label is created.
        /// </summary>
        /// <param name="axisLabel"></param>
        private void RaiseLabelCreated(ChartAxisLabel axisLabel)
        {
            LabelCreatedEventArgs visibleLabelChangedEventArgs = new LabelCreatedEventArgs();
            visibleLabelChangedEventArgs.AxisLabel = axisLabel;
            OnLabelCreated(visibleLabelChangedEventArgs);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void PopulateVisibleLabelsForLabelSource()
        {
            object labelsSource = null;

            if (!(this.LabelsSource is Windows.Data.Xml.Dom.XmlElement))
            {
                labelsSource = this.LabelsSource;
            }
            else
            {
                IList source = new List<IXmlNode>();
                CreateXmlSourceListWrapper((this.LabelsSource as Windows.Data.Xml.Dom.XmlElement), ref source);
                labelsSource = source;
            }

            foreach (object obj in GetSourceList(labelsSource))
            {
                double position = (int)ChartDataUtils.GetPositionalPathValue(obj, this.PositionPath);
                if (this.VisibleRange.Inside(position))
                {
                    if (string.IsNullOrEmpty(this.ContentPath))
                    {
                        if (this is DateTimeAxis || this is DateTimeCategoryAxis)
                            VisibleLabels.Add(new DateTimeAxisLabel(position, obj, position));
                        else
                            VisibleLabels.Add(new ChartAxisLabel(position, obj, position));
                    }
                    else
                    {
                        if (this is DateTimeAxis || this is DateTimeCategoryAxis)
                            VisibleLabels.Add(new DateTimeAxisLabel(position, ChartDataUtils.GetObjectByPath(obj, this.ContentPath), position));
                        else
                            VisibleLabels.Add(new ChartAxisLabel(position, ChartDataUtils.GetObjectByPath(obj, this.ContentPath), position));
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Custom Labels to Visible Labels Collection
        /// </summary>        
        private void PopulateVisibleLabelForCustomLabels()
        {
            GenerateVisibleLabels();

            var visibleLabelsCollection = VisibleLabels.ToDictionary(label => label.Position);

            VisibleLabels.Clear();

            foreach (ChartAxisLabel label in m_customLabels
                .Where(label => this.VisibleRange.Inside(label.Position)))
            {
                visibleLabelsCollection[label.Position] = new ChartAxisLabel()
                {
                    Position = label.Position,
                    LabelContent = label.LabelContent,
                };
            }

            foreach (var label in visibleLabelsCollection.Values)
            {
                VisibleLabels.Add(label);
            }
        }


        /// <summary>
        /// Occurs when visible labels collection is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_VisibleLabels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var item = e.NewItems[0] as ChartAxisLabel;
                item.ChartAxis = this;

                if (this.LabelCreated != null)
                {
                    RaiseLabelCreated(item);
                }
            }
        }

        private void RemoveAssociatedAxis(ChartAxis xAxis, ChartAxis yAxis)
        {
            if (xAxis != null && yAxis != null)
            {
                if (yAxis.associatedAxes != null && yAxis.AssociatedAxes.Contains(this))
                    yAxis.AssociatedAxes.Remove(this);
                if (xAxis.associatedAxes != null && xAxis.AssociatedAxes.Contains(this))
                    xAxis.AssociatedAxes.Remove(this);
            }
        }

        private void CheckAxes(ChartAxis chartAxis)
        {
            if (Area != null)
            {
                List<ChartAxis> axes = chartAxis.associatedAxes.ToList();
                foreach (ChartAxis axis in axes)
                {
                    if (!Area.Axes.Contains(axis))
                        chartAxis.AssociatedAxes.Remove(axis);
                }
            }
        }

        private void ScheduleCheck()
        {
            if (!isChecked)
            {
                if (DesignMode.DesignModeEnabled)
                    CheckRegisterSeries();
                else
                    checkRegisterAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, CheckRegisterSeries);

                isChecked = true;
            }
        }

        private void CheckRegisterSeries()
        {
            foreach (ChartSeriesBase chartSeries in RegisteredSeries)
            {
                if ((RegisteredSeries[0] as ChartSeriesBase).IsActualTransposed != (chartSeries as ChartSeriesBase).IsActualTransposed)
                {
                    throw new InvalidOperationException(RegisteredSeries[0].GetType() + " " + SfChartResourceWrapper.AxisIncompatibleExceptionMessage + " " + chartSeries.GetType());
                }
            }

            isChecked = false;
        }

        void OnCustomLabelsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateLabels();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents an axis label element.
    /// </summary>
    public partial class ChartAxisLabel
    {
        #region Fields

        #region Private Fields

        private LabelAlignment axisLabelAlignment = LabelAlignment.Center;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ChartAxisLabel()
        {
        }

        /// <summary>
        /// Called when instance created for ChartAxisLabel with following arguments
        /// </summary>
        /// <param name="position"></param>
        /// <param name="labelContent"></param>
        /// <param name="actualValue"></param>
        public ChartAxisLabel(double position, object labelContent, double actualValue)
        {
            this.Position = position;
            this.LabelContent = labelContent;
            this.ActualValue = actualValue;
        }

        /// <summary>
        /// Called when instance created for ChartAxisLabel with following arguments
        /// </summary>
        /// <param name="position"></param>
        /// <param name="labelContent"></param>
        public ChartAxisLabel(double position, object labelContent)
        {
            this.Position = position;
            this.LabelContent = labelContent;
        }

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets LabelContent property
        /// </summary>
        public object LabelContent { get; set; }

        /// <summary>
        /// Gets or sets Position property
        /// </summary>
        public double Position { get; set; }

        /// <summary>
        /// Gets or sets PrefixLabelTemplate
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate PrefixLabelTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets PostfixLabelTemplate property
        /// </summary>
        /// <value>
        /// <see cref="System.Windows.DataTemplate"/>
        /// </value>
        public DataTemplate PostfixLabelTemplate
        {
            get;
            set;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets actual value used for XForms.UWP
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal double ActualValue { get; set; }

        /// <summary>
        /// Gets or sets LabelStyle for individual Axis Used for xForms.uwp
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal object LabelStyle { get; set; }

        /// <summary>
        ///Used for xForms.UWP to return true if label content is changed
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal bool LabelContentChanged { get; set; }

        /// <summary>
        /// Gets or sets ChartAxis Property used for xForms.uwp
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal ChartAxis ChartAxis { get; set; }

        /// <summary>
        /// Gets or sets Alignment property for individual axis label.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Reviewed")]
        internal LabelAlignment AxisLabelAlignment
        {
            get { return axisLabelAlignment; }
            set
            {
                if (axisLabelAlignment == value) return;
                axisLabelAlignment = value;
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents an date time axis label element.
    /// </summary>
    public partial class DateTimeAxisLabel : ChartAxisLabel
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DateTimeAxisLabel()
        {
        }

        /// <summary>
        /// Called when instance created for DateTimeAxisLabel with following arguments
        /// </summary>
        /// <param name="position"></param>
        /// <param name="labelContent"></param>
        /// <param name="actualValue"></param>
        public DateTimeAxisLabel(double position, object labelContent, double actualValue)
                                : base(position, labelContent, actualValue)
        {
        }

        /// <summary>
        /// Called when instance created for DateTimeAxisLabel with following arguments
        /// </summary>
        /// <param name="position"></param>
        /// <param name="labelContent"></param>
        public DateTimeAxisLabel(double position, object labelContent)
                                : base(position, labelContent)
        {
        }

        #endregion

        #region Properties   

        public DateTimeIntervalType IntervalType
        {
            get;
            internal set;
        }

        public bool IsTransition
        {
            get;
            internal set;
        }

        #endregion
    }

    /// <summary>
    /// Represents chart series bounds changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like old bounds and new bounds.
    /// </remarks>
    public partial class ChartAxisBoundsEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        public Rect NewBounds { get; set; }
        public Rect OldBounds { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Represents chart series bounds changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like old bounds and new bounds.
    /// </remarks>
    public partial class VisibleRangeChangedEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        public DoubleRange NewRange { get; set; }
        public DoubleRange OldRange { get; set; }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for Label created event arguments.
    /// </summary>
    public partial class LabelCreatedEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the chart axis label.
        /// </summary>
        public ChartAxisLabel AxisLabel
        {
            get;
            set;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// Represents chart axis actual range changed event arguments.
    /// </summary>
    public partial class ActualRangeChangedEventArgs : EventArgs
    {
        #region Fields

        #region Private Fields

        private readonly ChartAxis axis;
        
        object actualMinimum;

        object actualMaximum;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActualRangeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public ActualRangeChangedEventArgs(ChartAxis axis)
        {
            this.axis = axis;
        }

        #endregion

        #region Properties

        public bool IsScrolling { get; set; }

        /// <summary>
        /// Gets or sets the actual minimum.
        /// </summary>
        /// <value>
        /// The actual minimum.
        /// </value>
        public object ActualMinimum
        {
            get { return actualMinimum; }
            set { actualMinimum = GetConvertedValue(value); }
        }

        /// <summary>
        /// Gets or sets the actual maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public object ActualMaximum
        {
            get { return actualMaximum; }
            set { actualMaximum = GetConvertedValue(value); }
        }

        /// <summary>
        /// Gets or sets the visible minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public object VisibleMinimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public object VisibleMaximum { get; set; }

        /// <summary>
        /// Gets or sets the actual interval
        /// </summary>
        public double ActualInterval
        {
            get;
            internal set;
        }

        #endregion

        #region Methods
        
        #region Internal Methods

        internal DoubleRange GetVisibleRange()
        {
            double start, end;

            if (VisibleMinimum == null && VisibleMaximum == null)
                return DoubleRange.Empty;

            if (VisibleMaximum == null)
            {
                end = ToDouble(ActualMaximum);
                start = ToDouble(VisibleMinimum);
            }
            else if (VisibleMinimum == null)
            {
                end = ToDouble(VisibleMaximum);
                start = ToDouble(ActualMinimum);
            }
            else
            {
                end = ToDouble(VisibleMaximum);
                start = ToDouble(VisibleMinimum);
            }

            var actualRange = GetActualRange();
            if (start < actualRange.Start)
                start = actualRange.Start;
            if (end > actualRange.End)
                end = actualRange.End;
            if (start == end)
                end += 1;
            return new DoubleRange(start, end);
        }

        internal DoubleRange GetActualRange()
        {
            return new DoubleRange(ToDouble(ActualMinimum), ToDouble(ActualMaximum));
        }

        #endregion

        #region Private Methods
        
        private object GetConvertedValue(object actualValue)
        {
            if (!(actualValue is double)) return actualValue;
            if (axis is DateTimeAxis)
            {
                return ((double)actualValue).FromOADate();
            }
            else if (axis is TimeSpanAxis)
            {
                return TimeSpan.FromMilliseconds((double)actualValue);
            }
            else if (axis is LogarithmicAxis)
            {
                return Math.Pow((axis as LogarithmicAxis).LogarithmicBase, (double)actualValue);
            }
            else
                return actualValue;
        }

        private double ToDouble(object actualValue)
        {
            if (actualValue is DateTime)
            {
                return ((DateTime)actualValue).ToOADate();
            }
            else if (actualValue is TimeSpan)
            {
                return ((TimeSpan)actualValue).TotalMilliseconds;
            }
            else if (axis is LogarithmicAxis)
            {
                return Math.Log((double)actualValue, (axis as LogarithmicAxis).LogarithmicBase);
            }
            else
            {
                return Convert.ToDouble(actualValue);
            }
        }
        
        #endregion

        #endregion                 
    }
}
