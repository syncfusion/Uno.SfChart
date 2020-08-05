using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents a control that display the label, ticks and lines for axis in 2D. 
    /// </summary>
    /// <seealso cref="Syncfusion.UI.Xaml.Charts.ChartAxis" />
    public abstract partial class ChartAxisBase2D : ChartAxis
    {
        #region Dependency Property Registration
  
        // Using a DependencyProperty as the backing store for AutoScrollingMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollingModeProperty =
            DependencyProperty.Register(
                "AutoScrollingMode",
                typeof(ChartAutoScrollingMode),
                typeof(ChartAxisBase2D),
                new PropertyMetadata(ChartAutoScrollingMode.End, OnAutoScrollingPropertyChanged));

        // Using a DependencyProperty as the backing store for AutoScrollingDelta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollingDeltaProperty =
            DependencyProperty.Register(
                "AutoScrollingDelta", 
                typeof(double), 
                typeof(ChartAxisBase2D), new PropertyMetadata(double.NaN, OnAutoScrollingPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ZoomPosition"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomPositionProperty =
            DependencyProperty.Register(
                "ZoomPosition", 
                typeof(double), 
                typeof(ChartAxisBase2D),
                new PropertyMetadata(0d, OnZoomPositionChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="LabelBorderBrush"/> property 
        /// </summary>
        public static readonly DependencyProperty LabelBorderBrushProperty =
            DependencyProperty.Register("LabelBorderBrush", typeof(Brush), typeof(ChartAxisBase2D), new PropertyMetadata(null));


        /// <summary>
        /// The Dependency Property for <see cref="MultiLevelLabelsBorderType"/>
        /// </summary>
        public static readonly DependencyProperty MultiLevelLabelsBorderTypeProperty =
            DependencyProperty.Register("MultiLevelLabelsBorderType", typeof(BorderType), typeof(ChartAxisBase2D), new PropertyMetadata(BorderType.Rectangle, OnAxisPropertyChanged));

        /// <summary>
        /// The Dependency property <see cref="ShowLabelBorder"/>
        /// </summary>
        public static readonly DependencyProperty ShowLabelBorderProperty =
            DependencyProperty.Register("ShowLabelBorder", typeof(bool), typeof(ChartAxisBase2D), new PropertyMetadata(false, OnAxisPropertyChanged));

        /// <summary>
        /// The Dependency property of <see cref="StartAngle"/> property
        /// </summary>
        public static readonly DependencyProperty PolarAngleProperty =
            DependencyProperty.Register("PolarAngle", typeof(ChartPolarAngle), typeof(ChartAxisBase2D), new PropertyMetadata(ChartPolarAngle.Rotate270, OnAxisPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="ZoomFactor"/> property.
        /// </summary>
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register(
                "ZoomFactor", 
                typeof(double), 
                typeof(ChartAxisBase2D),
                new PropertyMetadata(1d, OnZoomFactorChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="IncludeStripLineRange"/> property.
        /// </summary>
        public static readonly DependencyProperty IncludeStriplineRangeProperty =
            DependencyProperty.Register(
                "IncludeStripLineRange", 
                typeof(bool), 
                typeof(ChartAxisBase2D),
                new PropertyMetadata(false, OnIncludeStripLineRangeChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="StripLines"/> property.
        /// </summary>
        public static readonly DependencyProperty StripLinesProperty =
            DependencyProperty.Register(
                "StripLines",
                typeof(ChartStripLines),
                typeof(ChartAxisBase2D),
                new PropertyMetadata(null, OnStripLinesChanged));

        /// <summary>
        /// The DependencyProperty of <see cref="MultiLevelLabels"/> property
        /// </summary>
        public static readonly DependencyProperty MultiLevelLabelsProperty =
            DependencyProperty.Register("MultiLevelLabels", typeof(ChartMultiLevelLabels), typeof(ChartAxisBase2D), new PropertyMetadata(null, OnMultiLevelLabelsChanged));

        /// <summary>
        ///  The DependencyProperty for <see cref="EnableScrollBarResizing"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableScrollBarResizingProperty =
            DependencyProperty.Register(
                "EnableScrollBarResizing", 
                typeof(bool),
                typeof(ChartAxis),
                new PropertyMetadata(true, OnScrollBarResizableChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="EnableScrollBar"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableScrollBarProperty =
            DependencyProperty.Register(
                "EnableScrollBar",
                typeof(bool),
                typeof(ChartAxis),
                new PropertyMetadata(false, OnEnableScrollBarValueChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="DeferredScrolling"/> property.
        /// </summary>
        public static readonly DependencyProperty DeferredScrollingProperty =
            DependencyProperty.Register(
                "DeferredScrolling", 
                typeof(bool),
                typeof(ChartAxis),
                new PropertyMetadata(false));

#if NETFX_CORE

        /// <summary>
        /// The DependencyProperty for <see cref="EnableTouchMode"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableTouchModeProperty =
            DependencyProperty.Register(
                "EnableTouchMode", 
                typeof(bool),
                typeof(ChartAxis),
                new PropertyMetadata(true, OnEnableTouchModeChanged));

#else

        /// <summary>
        /// The DependencyProperty for <see cref="EnableTouchMode"/> property.
        /// </summary>
        public static readonly DependencyProperty EnableTouchModeProperty =
            DependencyProperty.Register(
                "EnableTouchMode", 
                typeof(bool), 
                typeof(ChartAxis),
                new PropertyMetadata(false, OnEnableTouchModeChanged));

#endif

        /// <summary>
        /// The DependencyProperty for <see cref="LabelBorderWidth"/> property 
        /// </summary>
        public static readonly DependencyProperty LabelBorderWidthProperty =
            DependencyProperty.Register(
                "LabelBorderWidth", 
                typeof(double), 
                typeof(ChartAxisBase2D), 
                new PropertyMetadata(1d, OnLabelBorderWidthPropertyChanged));
        
        internal static readonly DependencyProperty AxisStyleProperty = DependencyProperty.Register(
                "AxisStyle",
                typeof(Style),
                typeof(ChartAxisBase2D),
                new PropertyMetadata(null, OnAxisPropertyChanged));
        
        #endregion

        #region Fields

        #region Internal Fields

        internal ChartCartesianAxisPanel axisPanel;

        internal SfChartResizableBar sfChartResizableBar;

        #endregion

        #region Private Fields

        IAsyncAction updateStripLinesAction;

        private Panel labelsPanel;

        private Panel multiLevelLabelsPanel;

        private Panel elementsPanel;

        double rangeEnd = 1;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for ChartAxisBase2D
        /// </summary>
        public ChartAxisBase2D()
        {
            DefaultStyleKey = typeof(ChartAxisBase2D);
            SetBinding();
            StripLines = new ChartStripLines();
            MultiLevelLabels = new ChartMultiLevelLabels();
        }

        #endregion

        #region Events

        public event EventHandler<AxisLabelClickedEventArgs> LabelClicked;

        #endregion

        #region Properties

        #region Public Properties
        
        /// <summary>
        /// Gets or sets the mode to determine whether the axis should be auto scrolled at start or end position by using <see cref="ChartAutoScrollingMode"/> enum type.
        /// </summary>
        public ChartAutoScrollingMode AutoScrollingMode
        {
            get { return (ChartAutoScrollingMode)GetValue(AutoScrollingModeProperty); }
            set { SetValue(AutoScrollingModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value that determines the range of value to be visible during auto scrolling. This is a bindable property.
        /// </summary>
        /// <value>This property takes the double value</value>
        public double AutoScrollingDelta
        {
            get { return (double)GetValue(AutoScrollingDeltaProperty); }
            set { SetValue(AutoScrollingDeltaProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the decided range of values to be displayed from the zoomed range.
        /// </summary>
        /// <remarks>This scrolls or pan the chart to particular range.</remarks>
        public double ZoomPosition
        {
            get { return (double)GetValue(ZoomPositionProperty); }
            set { SetValue(ZoomPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LabelBorderBrush
        /// </summary>
        public Brush LabelBorderBrush
        {
            get { return (Brush)GetValue(LabelBorderBrushProperty); }
            set { SetValue(LabelBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the BorderType for Multi level labels
        /// </summary>
        public BorderType MultiLevelLabelsBorderType
        {
            get { return (BorderType)GetValue(MultiLevelLabelsBorderTypeProperty); }
            set { SetValue(MultiLevelLabelsBorderTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the border around axis label
        /// </summary>
        public bool ShowLabelBorder
        {
            get { return (bool)GetValue(ShowLabelBorderProperty); }
            set { SetValue(ShowLabelBorderProperty, value); }
        }

        /// Gets or sets the start angle Polar or radar series
        /// </summary>
        public ChartPolarAngle PolarAngle
        {
            get { return (ChartPolarAngle)GetValue(PolarAngleProperty); }
            set { SetValue(PolarAngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets zoom factor. It determines delta of visible range to be displayed.
        /// </summary>
        /// <value>Value must fall within 0 to 1.</value>
        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the axis to include the annotation range, while calculating the axis range.
        /// </summary>
        /// <remarks>This feature is to avoid cropping or missing of annotation, which doesn't falls inside the axis range.
        /// </remarks>
        public bool IncludeStripLineRange
        {
            get { return (bool)GetValue(IncludeStriplineRangeProperty); }
            set { SetValue(IncludeStriplineRangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the striplines collection for the SfChart.
        /// </summary>
        public ChartStripLines StripLines
        {
            get { return (ChartStripLines)GetValue(StripLinesProperty); }
            set { SetValue(StripLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the multi level labels collection
        /// </summary>
        public ChartMultiLevelLabels MultiLevelLabels
        {
            get { return (ChartMultiLevelLabels)GetValue(MultiLevelLabelsProperty); }
            set { SetValue(MultiLevelLabelsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the legacy style for the scroll bar.
        /// </summary>
        public bool EnableScrollBarResizing
        {
            get { return (bool)GetValue(EnableScrollBarResizingProperty); }
            set { SetValue(EnableScrollBarResizingProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable the scrollbar for the SfChart.
        /// </summary>
        public bool EnableScrollBar
        {
            get { return (bool)GetValue(EnableScrollBarProperty); }
            set { SetValue(EnableScrollBarProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable scrollbar to suspend value updates for every thumb value changes.
        /// </summary>
        public bool DeferredScrolling
        {
            get { return (bool)GetValue(DeferredScrollingProperty); }
            set { SetValue(DeferredScrollingProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LabelBorderWidth
        /// </summary>
        public double LabelBorderWidth
        {
            get { return (double)GetValue(LabelBorderWidthProperty); }
            set { SetValue(LabelBorderWidthProperty, value); }
        }

#if NETFX_CORE

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable touch mode for the scroll bar.
        /// </summary>
        public bool EnableTouchMode
        {
            get { return (bool)GetValue(EnableTouchModeProperty); }
            set { SetValue(EnableTouchModeProperty, value); }
        }

#else

        /// <summary>
        /// Gets or sets a value indicating whether to enable or disable touch mode for the scroll bar.
        /// </summary>
        public bool EnableTouchMode
        {
            get { return (bool)GetValue(EnableTouchModeProperty); }
            set { SetValue(EnableTouchModeProperty, value); }
        }
        
#endif     

        #endregion

        #region Internal Properties

        internal bool CanAutoScroll { get; set; } 

        internal bool IsZoomed
        {
            get
            {
                return ZoomFactor != 1d;
            }
        }

        /// <summary>
        /// Gets or sets the style for axis it will get notify to update the chart.
        /// </summary>
        internal Style AxisStyle
        {
            get { return (Style)GetValue(AxisStyleProperty); }
            set { SetValue(AxisStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets zoom factor. Value must fall within 0 to 1. It determines delta of visible range.
        /// </summary>
        internal double ActualZoomFactor
        {
            get
            {
                return (ZoomFactor == 0) ? 0.01 : ZoomFactor;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public Override Methods

        public override double ValueToPolarCoefficient(double value)
        {
            double result = double.NaN;

            var start = VisibleRange.Start;
            var delta = VisibleRange.Delta;

            result = (value - start) / delta;
            if (VisibleLabels.Count > 0)
                result *= 1 - 1 / (double)VisibleLabels.Count; // WRT-2476-Polar and radar series segments are plotted before the Actual position
            else
                result *= 1 - 1 / (delta + 1);

            return ValueBasedOnAngle(result);
        }

        public override double PolarCoefficientToValue(double value)
        {
            double result = double.NaN;

            value = ValueBasedOnAngle(value);

            value /= 1 - 1 / (VisibleRange.Delta + 1);

            result = VisibleRange.Start + VisibleRange.Delta * value;

            return result;
        }

        #endregion

        #region Internal Override Methods
        
        /// <summary>
        ///  Update Auto Scrolling Delta value  based on auto scrolling delta mode option.
        /// </summary>
        /// <param name="actualRange">The actual range of the axis.</param>
        /// <param name="scrollingDelta">The scroll delta region to be captured.</param>
        internal virtual void UpdateAutoScrollingDelta(double scrollingDelta)
        {
            if (AutoScrollingMode == ChartAutoScrollingMode.Start)
            {
                VisibleRange = new DoubleRange(ActualRange.Start, ActualRange.Start + scrollingDelta);
                ZoomFactor = (float)(VisibleRange.Delta / ActualRange.Delta);
                ZoomPosition = 0;
            }
            else
            {
                VisibleRange = new DoubleRange(ActualRange.End - scrollingDelta, ActualRange.End);
                ZoomFactor = (float)(VisibleRange.Delta / ActualRange.Delta);
                ZoomPosition = 1 - ZoomFactor;
            }
        }

        internal override void Dispose()
        {
            if (LabelClicked != null)
            {
                foreach (var handler in LabelClicked.GetInvocationList())
                {
                    LabelClicked -= (handler as EventHandler<AxisLabelClickedEventArgs>);
                }

                LabelClicked = null;
            }

            if (AssociatedAxes != null)
            {
                AssociatedAxes.Clear();
            }

            LabelsSource = null;
            this.Area = null;

#if NETFX_CORE

            if (sfChartResizableBar != null)
            {
                sfChartResizableBar.Dispose();
                sfChartResizableBar = null;
            }

#endif
                       
            DisposePanels();

            PrefixLabelTemplate = null;
            PostfixLabelTemplate = null;
            LabelTemplate = null;
            
            DisposeEvents();
            base.Dispose();
        }

        internal override void CreateLineRecycler()
        {
            if (this.Area != null && (Area as SfChart).GridLinesPanel != null)
            {
                GridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
                MinorGridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
            }
        }

        internal override void ComputeDesiredSize(Size size)
        {
            this.ClearValue(HeightProperty);
            this.ClearValue(WidthProperty);
            if (sfChartResizableBar != null)
            {
                sfChartResizableBar.ClearValue(Control.WidthProperty);
                sfChartResizableBar.ClearValue(Control.HeightProperty);
            }

            AvailableSize = size;
            CalculateRangeAndInterval(size);
            if (Visibility != Visibility.Collapsed || Area.AreaType == ChartAreaType.PolarAxes)
            {
                ApplyTemplate();
                if (axisPanel != null)
                {
                    UpdatePanels();
                    UpdateLabels();
                    ComputedDesiredSize = axisPanel.ComputeSize(size);
                }
            }
            else
            {
                ActualPlotOffset = PlotOffset;
                InsidePadding = 0;
                UpdateLabels();
                ComputedDesiredSize = Orientation == Orientation.Horizontal
                                          ? new Size(size.Width, 0)
                                          : new Size(0, size.Height);
            }
        }

        #endregion

        #region Internal Methods

        internal void ChangeStyle(bool modeValue)
        {
            if (sfChartResizableBar != null)
            {
                if (modeValue == true)
                {
                    if (OpposedPosition)
                    {
                        VisualStateManager.GoToState(this, "OpposedTouchModeStyle", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "TouchModeStyle", true);
                    }
                }
                else
                {
                    VisualStateManager.GoToState(this, "CommonStyle", true);
                }
#if NETFX_CORE
                if (axisPanel != null)
                    axisPanel.InvalidateMeasure();
#endif
                sfChartResizableBar.UpdateResizable(EnableScrollBarResizing);
            }
        }

        internal DoubleRange CalculateRange(DoubleRange actualRange, double zoomPosition, double zoomFactor)
        {
            DoubleRange baseRange = actualRange;
            DoubleRange range = new DoubleRange();

            double start = ActualRange.Start + ZoomPosition * actualRange.Delta;
            ////Exception thrown, while using ZoomFactor and ZoomPosition as 0/WPF-14141

            double end = start + ActualZoomFactor * actualRange.Delta;

            if (start < baseRange.Start)
            {
                end = end + (baseRange.Start - start);
                start = baseRange.Start;
            }

            if (end > baseRange.End || (this.EnableScrollBar && sfChartResizableBar != null && Math.Round(end) == baseRange.End && sfChartResizableBar.RangeEnd == rangeEnd
               && this.RegisteredSeries.Count > 0 && !(this.RegisteredSeries[0] as ChartSeries).IsActualTransposed))
            {
                start = start - (end - baseRange.End);
                end = baseRange.End;
            }

            rangeEnd = sfChartResizableBar != null ? sfChartResizableBar.RangeEnd : 1;

            range = new DoubleRange(start, end);
            return range;
        }

        // To calculate the co-efficient value based on the start angle
        internal double ValueBasedOnAngle(double result)
        {
            if (PolarAngle == ChartPolarAngle.Rotate270)
                result = this.IsInversed ? result : (1d - result);
            else if (PolarAngle == ChartPolarAngle.Rotate0)
                result = this.IsInversed ? (0.75d + result) : (0.75d - result);
            else if (PolarAngle == ChartPolarAngle.Rotate90)
                result = this.IsInversed ? (0.5d + result) : (0.5d - result);
            else if (PolarAngle == ChartPolarAngle.Rotate180)
                result = this.IsInversed ? (0.25d + result) : (0.25d - result);
            result = result < 0 ? result + 1d : result;
            result = result > 1 ? result - 1d : result;
            return result;
        }

        internal void SetLabelDownArguments(object source)
        {
            if (LabelClicked == null) return;
            var element = source as FrameworkElement;
            if (element != null)
            {
                var parent = GetParent(element);
                if (parent != null)
                {
                    AxisLabelClickedEventArgs args = new AxisLabelClickedEventArgs();
                    args.AxisLabel = parent.Tag as ChartAxisLabel;
                    if (args.AxisLabel != null)
                    {
                        LabelClicked(this, args);
                    }
                }
            }
        }

        #endregion

        #region Protected Internal Methods    

        /// <summary>
        /// Calculates the visible range.
        /// </summary>
        protected internal override void CalculateVisibleRange(Size avalableSize)
        {
            VisibleRange = ActualRange;
            VisibleInterval = ActualInterval;
            if (ZoomFactor < 1 || ZoomPosition > 0)
            {
                VisibleRange = CalculateRange(ActualRange, ZoomPosition, ZoomFactor);
            }
        }

        protected internal override void OnAxisBoundsChanged(ChartAxisBoundsEventArgs args)
        {
            base.OnAxisBoundsChanged(args);
            if (sfChartResizableBar != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    sfChartResizableBar.Width = ArrangeRect.Width;
                    if (Area.VisibleSeries.Count != 0)
                    {
                        this.Width = ArrangeRect.Width;
                    }
                }
                else
                {
                    sfChartResizableBar.Height = ArrangeRect.Height;
                    if (Area.VisibleSeries.Count != 0)
                    {
                        this.Height = ArrangeRect.Height;
                    }
                }
            }

            if (axisPanel != null)
            {
                axisPanel.ArrangeElements(new Size(ArrangeRect.Width, ArrangeRect.Height));
            }
        }

        #endregion

        #region Protected Override Methods

        protected override DependencyObject CloneAxis(DependencyObject obj)
        {
            var axis = obj as ChartAxisBase2D;
            axis.ZoomFactor = ZoomFactor;
            axis.ZoomPosition = ZoomPosition;
            axis.IncludeStripLineRange = IncludeStripLineRange;
            axis.DeferredScrolling = DeferredScrolling;
            axis.EnableScrollBar = EnableScrollBar;
            axis.EnableScrollBarResizing = EnableScrollBarResizing;
            axis.EnableTouchMode = EnableTouchMode;
            axis.LabelBorderBrush = LabelBorderBrush;
            axis.LabelBorderWidth = LabelBorderWidth;
            axis.MultiLevelLabelsBorderType = MultiLevelLabelsBorderType;
            axis.ShowLabelBorder = ShowLabelBorder;
            foreach (ChartStripLine stripline in StripLines)
            {
                axis.StripLines.Add((ChartStripLine)stripline.Clone());
            }

            foreach (ChartMultiLevelLabel label in MultiLevelLabels)
            {
                axis.MultiLevelLabels.Add((ChartMultiLevelLabel)label.Clone());
            }

            return base.CloneAxis(obj);
        }

        /// <summary>
        /// Invoke to render chart Axis.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            {
                ClearItems();
                base.OnApplyTemplate();
                sfChartResizableBar = this.GetTemplateChild("sfchartResizableBar") as SfChartResizableBar;
                if (sfChartResizableBar != null)
                {
                    sfChartResizableBar.Visibility = EnableScrollBar ? Visibility.Visible : Visibility.Collapsed;
                    sfChartResizableBar.Axis = this;
                    Binding binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath("Orientation");
                    BindingOperations.SetBinding(sfChartResizableBar, ResizableScrollBar.OrientationProperty, binding);

#if NETFX_CORE
                    // For IR 17848 - Designer crashes when rebuild the SfChart
                    bool _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
                    if (!_isInDesignMode)
#endif
                        if (this.EnableScrollBar)
                        {
                            ChangeStyle(this.EnableTouchMode);
                        }
                }

                labelsPanel = this.GetTemplateChild("axisLabelsPanel") as Panel;
                elementsPanel = this.GetTemplateChild("axisElementPanel") as Panel;
                multiLevelLabelsPanel = this.GetTemplateChild("axisMultiLevelLabelsPanels") as Panel;
                axisPanel = this.GetTemplateChild("axisPanel") as ChartCartesianAxisPanel;
                if (axisPanel != null)
                    axisPanel.Axis = this; // Null exception thrown while bind the StripLine through view model-WPF-14439
#if WINDOWS_UAP && CHECKLATER

                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    axisPanel.Children.Remove(sfChartResizableBar);
                    sfChartResizableBar = null;
                }
#endif
                if (axisPanel != null)
                {
                    foreach (var stripLine in StripLines)
                    {
                        if (!axisPanel.Children.Contains(stripLine))
                            axisPanel.Children.Add(stripLine);
                    }

                    var linearAxis = axisPanel.Axis as NumericalAxis;
                    if (linearAxis != null && linearAxis.IsSecondaryAxis)
                    {
                        foreach (var breaks in linearAxis.AxisScaleBreaks)
                        {
                            if (!axisPanel.Children.Contains(breaks))
                                axisPanel.Children.Add(breaks);
                        }
                    }
                }

                UpdatePanels();
                headerContent = this.GetTemplateChild("headerContent") as ContentControl;
                if (HeaderTemplate == null && headerContent != null && HeaderStyle != null)
                {
                    if (HeaderStyle.Foreground != null)
                        headerContent.Foreground = HeaderStyle.Foreground;
                    if (HeaderStyle.FontSize != 0.0)
                        headerContent.FontSize = HeaderStyle.FontSize;
                    if (HeaderStyle.FontFamily != null)
                        headerContent.FontFamily = HeaderStyle.FontFamily;
                }

                if (axisElementsUpdateRequired)
                {
                    if (axisElementsPanel != null)
                        axisElementsPanel.UpdateElements();
                    if (axisLabelsPanel != null)
                        axisLabelsPanel.UpdateElements();
                    if (axisMultiLevelLabelsPanel != null && MultiLevelLabels.Count > 0)
                        axisMultiLevelLabelsPanel.UpdateElements();
                    axisElementsUpdateRequired = false;
                }

                if (this.Area != null && (Area as SfChart).GridLinesPanel != null)
                {
                    GridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
                    MinorGridLinesRecycler = new UIElementsRecycler<Line>((Area as SfChart).GridLinesPanel);
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static void OnAutoScrollingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxisBase2D = d as ChartAxisBase2D;
            if (chartAxisBase2D != null)
            {
                chartAxisBase2D.CanAutoScroll = true;
                chartAxisBase2D.OnPropertyChanged();
            }
        }

        private static void OnAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxisBase2D = d as ChartAxisBase2D;
            if (chartAxisBase2D != null)
            {
                chartAxisBase2D.OnPropertyChanged();
            }
        }

        private static void OnLabelBorderWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = (d as ChartAxisBase2D);
            if (e.OldValue != null && Convert.ToDouble(e.OldValue) > 0
                && Convert.ToDouble(e.NewValue) == 0)
                axis.OnLabelBorderWidthChanged();
            axis.OnPropertyChanged();
        }

        private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxisBase2D axis = (d as ChartAxisBase2D);
            var linearAxis = d as NumericalAxis;
            if (linearAxis != null && linearAxis.BreakExistence())
            {
                linearAxis.ClearBreakElements();
            }

            axis.OnZoomDataChanged(e);
            if (axis.Area != null && (axis.Area as SfChart).zoomingToolBar != null)
                axis.EnableZoomingToolBarState();
        }

        private static void OnZoomPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAxisBase2D axis = (d as ChartAxisBase2D);
            var linearAxis = d as NumericalAxis;
            if (linearAxis != null && linearAxis.BreakExistence())
            {
                linearAxis.ClearBreakElements();
            }

            axis.OnZoomDataChanged(e);
            if (axis.Area != null && (axis.Area as SfChart).zoomingToolBar != null)
                axis.EnableZoomingToolBarState();
        }

        private static void OnIncludeStripLineRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as ChartAxis;
            if (axis != null && axis.Area != null)
                (axis.Area as SfChart).UpdateStripLines();
        }

        private static void OnStripLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartAxisBase2D).OnStripLinesChanged(args.NewValue as ChartStripLines, args.OldValue as ChartStripLines);
        }

        private static void OnMultiLevelLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartAxisBase2D).OnMultiLevelLabelsCollectionChanged(e.OldValue as ChartMultiLevelLabels, e.NewValue as ChartMultiLevelLabels);
        }

        private static void OnScrollBarResizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            (d as ChartAxisBase2D).OnScrollBarResizableChanged();
        }

        private static void OnEnableScrollBarValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = d as ChartAxisBase2D;
            if (chartAxis.Area != null && chartAxis.sfChartResizableBar != null)
            {
                chartAxis.sfChartResizableBar.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
                chartAxis.ChangeStyle(chartAxis.EnableTouchMode);
                chartAxis.Area.ScheduleUpdate();
            }
        }

        private static void OnEnableTouchModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var axis = d as ChartAxisBase2D;
#if NETFX_CORE
            // For IR 17848 - Designer crashes when rebuild the SfChart
            bool _isInDesignMode = Windows.ApplicationModel.DesignMode.DesignModeEnabled;
            if (!(bool)e.NewValue && _isInDesignMode)
                axis.EnableTouchMode = true;
            else
            {
                if (!_isInDesignMode)
                    axis.ChangeStyle((bool)e.NewValue);
            }
#else
            axis.ChangeStyle((bool)e.NewValue);
#endif
            if (axis.EnableScrollBar && axis.Area != null)
                axis.Area.ScheduleUpdate();
        }

        private static FrameworkElement GetParent(FrameworkElement element)
        {
            if (!(element.Tag is ChartAxisLabel))
            {
                var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
                while (parent != null && !(parent.Tag is ChartAxisLabel))
                {
                    parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
                }

                if (parent == null)
                    return null;
                else
                    return parent;
            }
            else
                return element;
        }

        #endregion

        #region Private Methods

        private void DisposeEvents()
        {
            if (MultiLevelLabels != null)
            {
                MultiLevelLabels.CollectionChanged -= MultiLevelLabls_CollectionChanged;
                MultiLevelLabels.Clear(); 
                MultiLevelLabels = null;
            }

            if (StripLines != null)
            {
                StripLines.CollectionChanged -= OnStripLinesCollectionChanged;
                StripLines.Clear();
            }

            if(RegisteredSeries != null)
            {
                RegisteredSeries.CollectionChanged -= OnRegisteredSeriesCollectionChanged;
                RegisteredSeries.Clear();
            }

            if (ValueToCoefficientCalc != null)
            {
                foreach(var handler in ValueToCoefficientCalc.GetInvocationList())
                {
                    ValueToCoefficientCalc -= handler as ValueToCoefficientHandler;
                }

                ValueToCoefficientCalc = null;
            }

            if (CoefficientToValueCalc != null)
            {
                foreach (var handler in CoefficientToValueCalc.GetInvocationList())
                {
                    CoefficientToValueCalc -= handler as ValueToCoefficientHandler;
                }

                CoefficientToValueCalc = null;
            }
                       
        }

        internal void DisposePanels()
        {
            if (axisPanel != null)
            {
                if (axisPanel.LayoutCalc != null)
                {
                    axisPanel.LayoutCalc.Clear();
                }

                axisPanel.Axis = null;
                axisPanel.Children.Clear();
                axisPanel = null;
            }

            if (labelsPanel != null)
            {
                labelsPanel.Children.Clear();
                labelsPanel = null;
            }

            if (multiLevelLabelsPanel != null)
            {
                multiLevelLabelsPanel.Children.Clear();
                multiLevelLabelsPanel = null;
            }

            if (elementsPanel != null)
            {
                elementsPanel.Children.Clear();
                elementsPanel = null;
            }

            if (axisLabelsPanel != null)
            {
                var circularAxisPanel = axisLabelsPanel as ChartCircularAxisPanel;
                if (circularAxisPanel != null)
                {
                    circularAxisPanel.Axis = null;
                }

                var cartesianAxisPanel = axisLabelsPanel as ChartCartesianAxisLabelsPanel;
                if (cartesianAxisPanel != null)
                {
                    if (cartesianAxisPanel.children != null)
                    {
                        cartesianAxisPanel.children.Clear();
                    }

                    cartesianAxisPanel.Dispose();
                }

                axisLabelsPanel = null;
            }

            var cartesianElementsPanel = axisElementsPanel as ChartCartesianAxisElementsPanel;
            if (cartesianElementsPanel != null)
            {
                if (axisElementsPanel.Children != null)
                {
                    axisElementsPanel.Children.Clear();
                }

                cartesianElementsPanel.Dispose();
                axisElementsPanel = null;

            }

            var cartesianMultiLevelLabels = axisMultiLevelLabelsPanel as MultiLevelLabelsPanel;
            if (cartesianMultiLevelLabels != null)
            {
                cartesianMultiLevelLabels.Dispose();
            }
            cartesianMultiLevelLabels = null;

            if(GridLinesRecycler != null)
            {
                foreach (var gridLine in GridLinesRecycler)
                {
                    gridLine.ClearUIValues();
                }

                GridLinesRecycler.Clear();
                GridLinesRecycler = null;
            }

            if (MinorGridLinesRecycler != null)
            {
                foreach (var minorGridLine in MinorGridLinesRecycler)
                {
                    minorGridLine.ClearUIValues();
                }

                MinorGridLinesRecycler.Clear();
                MinorGridLinesRecycler = null;
            }
        }

        private void OnLabelBorderWidthChanged()
        {
            if (axisLabelsPanel != null)
                axisLabelsPanel.UpdateElements();
        }

        /// <summary>
        /// This method is used enables or diasable zooming tool bar item based on zoom position and zoom factor
        /// </summary>
        private void EnableZoomingToolBarState()
        {
            if (this.ZoomPosition > 0 || this.ZoomFactor < 1)
                (this.Area as SfChart).ChangeToolBarState();
            else
                (this.Area as SfChart).ResetToolBarState();
        }

        private void OnZoomDataChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Area != null)
            {
#if NETFX_CORE
#if CHECKLATER
                if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    this.Area.CompositionScheduleUpdate();
                else
#endif
#endif
                this.Area.ScheduleUpdate();
            }
        }

        private void OnStripLinesChanged(ChartStripLines newValue, ChartStripLines oldValue)
        {
            if (newValue != null)
            {
                newValue.CollectionChanged += OnStripLinesCollectionChanged;
                foreach (var stripLine in newValue)
                {
                    stripLine.PropertyChanged += OnStripLinesPropertyChanged;
                    if (stripLine.Parent is Panel)
                    {
                        (stripLine.Parent as Panel).Children.Remove(stripLine);
                    }
                    if (axisPanel != null && !axisPanel.Children.Contains(stripLine))
                    {
                        axisPanel.Children.Add(stripLine);
                    }
                }
            }

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnStripLinesCollectionChanged;
                foreach (var stripLine in oldValue)
                {
                    stripLine.PropertyChanged -= OnStripLinesPropertyChanged;
                    if (axisPanel != null && axisPanel.Children.Contains(stripLine))
                    {
                        axisPanel.Children.Remove(stripLine);
                    }
                }
            }

            UpdateStripsLines();
        }

        private void OnMultiLevelLabelsCollectionChanged(ChartMultiLevelLabels oldValues, ChartMultiLevelLabels newValues)
        {
            if (oldValues != null)
            {
                (oldValues as ChartMultiLevelLabels).CollectionChanged -= MultiLevelLabls_CollectionChanged;
                if (oldValues.Count > 0)
                {
                    if (axisMultiLevelLabelsPanel != null)
                        axisMultiLevelLabelsPanel.DetachElements();
                }
            }

            if (newValues != null)
                (newValues as ChartMultiLevelLabels).CollectionChanged +=
                    MultiLevelLabls_CollectionChanged;
            if (Area != null)
                OnPropertyChanged();
        }

        private void MultiLevelLabls_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                    foreach (ChartMultiLevelLabel item in e.NewItems)
                        if (item != null)
                            item.PropertyChanged += Item_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                    foreach (ChartMultiLevelLabel item in e.OldItems)
                        if (item != null)
                            item.PropertyChanged -= Item_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                    foreach (ChartMultiLevelLabel item in e.OldItems)
                        if (item != null)
                            item.PropertyChanged -= Item_PropertyChanged;

                if (e.NewItems != null)
                    foreach (ChartMultiLevelLabel item in e.NewItems)
                        if (item != null)
                            item.PropertyChanged += Item_PropertyChanged;
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (axisMultiLevelLabelsPanel != null)
                    axisMultiLevelLabelsPanel.DetachElements();
                axisMultiLevelLabelsPanel = new MultiLevelLabelsPanel(multiLevelLabelsPanel)
                {
                    Axis = this
                };
            }

            if (Area != null)
                OnPropertyChanged();
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Area != null)
                OnPropertyChanged();
        }

        private void OnScrollBarResizableChanged()
        {
            if (sfChartResizableBar != null)
                sfChartResizableBar.UpdateResizable(EnableScrollBarResizing);
        }

        private void SetBinding()
        {
            var binding = new Binding
            {
                Source = this,
                Path = new PropertyPath("Visibility")
            };
            BindingOperations.SetBinding(this, AxisVisibilityProperty, binding);

            binding = new Binding
            {
                Path = new PropertyPath("Style"),
                Source = this
            };
            BindingOperations.SetBinding(this, AxisStyleProperty, binding);
        }

        private void OnStripLinesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ChartStripLine item in e.NewItems)
                {
                    item.PropertyChanged += OnStripLinesPropertyChanged;
                    if (axisPanel != null && !axisPanel.Children.Contains(item))
                    {
                        axisPanel.Children.Add(item);
                    }
                }

                if (Area != null)
                    UpdateStripsLines();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ChartStripLine item in e.OldItems)
                {
                    item.PropertyChanged -= OnStripLinesPropertyChanged;
                    if (axisPanel != null && axisPanel.Children.Contains(item))
                    {
                        axisPanel.Children.Remove(item);
                    }
                }

                UpdateStripsLines();
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (axisPanel != null)
                {
                    UIElement[] axisPanelCollection = new UIElement[axisPanel.Children.Count];
                    axisPanel.Children.CopyTo(axisPanelCollection, 0);
                    foreach (ChartStripLine item in axisPanelCollection.Where(it => (it is ChartStripLine)))
                    {
                        if (axisPanel.Children.Contains(item))
                        {
                            item.PropertyChanged -= OnStripLinesPropertyChanged;
                            axisPanel.Children.Remove(item);
                        }
                    }

                    axisPanelCollection = null;
                    UpdateStripsLines();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var item = e.OldItems[0] as ChartStripLine;
                if (item != null && axisPanel != null && axisPanel.Children.Contains(item))
                {
                    item.PropertyChanged -= OnStripLinesPropertyChanged;
                    axisPanel.Children.Remove(item);
                }

                item = e.NewItems[0] as ChartStripLine;
                if (item != null && axisPanel != null && !axisPanel.Children.Contains(item))
                {
                    item.PropertyChanged += OnStripLinesPropertyChanged;
                    axisPanel.Children.Add(item);
                }

                UpdateStripsLines();
            }
        }

        private void UpdateStripsLines()
        {
            if (Area != null)
                (Area as SfChart).UpdateStripLines();

            updateStripLinesAction = null;
        }

        private void ClearItems()
        {
            if (sfChartResizableBar != null)
            {
                sfChartResizableBar = null;
            }

            if (GridLinesRecycler != null)
            {
                GridLinesRecycler.Clear();
                GridLinesRecycler = null;
            }

            if (MinorGridLinesRecycler != null)
            {
                MinorGridLinesRecycler.Clear();
                MinorGridLinesRecycler = null;
            }

            if (labelsPanel != null)
            {
                labelsPanel.Children.Clear();
                labelsPanel = null;
            }

            if (elementsPanel != null)
            {
                elementsPanel.Children.Clear();
                elementsPanel = null;
            }

            if (axisPanel != null)
            {
                axisPanel.Children.Clear();
                axisPanel = null;
            }

            if (axisMultiLevelLabelsPanel != null)
                axisMultiLevelLabelsPanel.DetachElements();

            if (axisLabelsPanel != null)
            {
                axisLabelsPanel.Children.Clear();
                axisLabelsPanel = null;
            }

            if (axisElementsPanel != null)
            {
                axisElementsPanel.Children.Clear();
                axisElementsPanel = null;
            }

            if (headerContent != null)
                headerContent = null;
        }
        
        private void OnStripLinesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (updateStripLinesAction == null && Area != null)
            {
                updateStripLinesAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, UpdateStripsLines);
            }
        }

        void UpdatePanels()
        {
            if (axisPanel != null)
            {
                if (AxisLayoutPanel is ChartPolarAxisLayoutPanel
                    && !(axisLabelsPanel is ChartCircularAxisPanel))
                {
                    if (axisLabelsPanel != null)
                    {
                        axisLabelsPanel.DetachElements();
                    }

                    if (axisElementsPanel != null)
                    {
                        axisElementsPanel.DetachElements();
                    }

                    axisPanel.LayoutCalc.Clear();
                    axisLabelsPanel = new ChartCircularAxisPanel(labelsPanel)
                    {
                        Axis = this
                    };
                    axisPanel.LayoutCalc.Add(axisLabelsPanel as ILayoutCalculator);
                }
                else if (!(AxisLayoutPanel is ChartPolarAxisLayoutPanel)
                    && !(axisLabelsPanel is ChartCartesianAxisLabelsPanel))
                {
                    if (axisLabelsPanel != null)
                    {
                        axisLabelsPanel.DetachElements();
                    }

                    if (axisElementsPanel != null)
                    {
                        axisElementsPanel.DetachElements();
                    }

                    axisPanel.LayoutCalc.Clear();
                    if (axisMultiLevelLabelsPanel != null)
                        axisMultiLevelLabelsPanel.DetachElements();

                    if (labelsPanel != null)
                    {
                        axisLabelsPanel = new ChartCartesianAxisLabelsPanel(labelsPanel)
                        {
                            Axis = this
                        };
                        axisPanel.LayoutCalc.Add(axisLabelsPanel as ILayoutCalculator);
                    }

                    if (elementsPanel != null)
                    {
                        axisElementsPanel = new ChartCartesianAxisElementsPanel(elementsPanel)
                        {
                            Axis = this
                        };
                        axisPanel.LayoutCalc.Add(axisElementsPanel as ILayoutCalculator);
                    }

                    if (MultiLevelLabels != null && MultiLevelLabels.Count > 0
                        && multiLevelLabelsPanel != null)
                    {
                        axisMultiLevelLabelsPanel = new MultiLevelLabelsPanel(multiLevelLabelsPanel)
                        {
                            Axis = this
                        };
                    }
                }
                else if (MultiLevelLabels != null && MultiLevelLabels.Count > 0
                      && multiLevelLabelsPanel != null && axisLabelsPanel is ChartCartesianAxisLabelsPanel && ((axisMultiLevelLabelsPanel != null &&
                      axisMultiLevelLabelsPanel.Panel == null) || axisMultiLevelLabelsPanel == null))
                {
                    axisMultiLevelLabelsPanel = new MultiLevelLabelsPanel(multiLevelLabelsPanel)
                    {
                        Axis = this
                    };
                }
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Class implementation for axis label clicked event arguments.
    /// </summary>
    public partial class AxisLabelClickedEventArgs : EventArgs
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the chart axis label.
        /// </summary>
        public ChartAxisLabel AxisLabel
        {
            get;
            internal set;
        }

        #endregion

        #endregion
    }
}
