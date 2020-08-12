using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;
using Windows.UI.Input;
using Windows.UI.Core;
using System.Resources;

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents the Chart control which is used to visualize the data graphically.
    /// </summary>
    /// <remarks>
    /// The Chart is often used to make it easier to
    /// understand large amount of data and the relationship between different parts
    /// of the data. Chart can usually be read more quickly than the raw data that they
    /// come from. <para> Certain <see cref="ChartSeriesBase" /> are more useful for
    /// presenting a given data set than others. For example, data that presents
    /// percentages in different groups (such as "satisfied, not satisfied, unsure") are
    /// often displayed in a <see cref="PieSeries" /> chart, but are more easily
    /// understood when presented in a horizontal <see cref="BarSeries" /> chart.
    /// On the other hand, data that represents numbers that change over a period of
    /// time (such as "annual revenue from 2011 to 2012") might be best shown as a <see
    /// cref="LineSeries" /> chart. </para>
    /// </remarks>
    /// <seealso cref="ChartSeriesBase"/>
    /// <seealso cref="ChartLegend"/>
    /// <seealso cref="ChartAxis"/>

    [ContentProperty(Name = "Series")]
    public partial class SfChart : ChartBase, IDisposable
    {
        #region Dependency Property Registration

        /// <summary>
        /// The DependencyProperty for <see cref="PrimaryAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty =
            DependencyProperty.Register(
                "PrimaryAxis", 
                typeof(ChartAxisBase2D),
                typeof(SfChart),
                new PropertyMetadata(null, OnPrimaryAxisChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="SecondaryAxis"/> property.
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty =
            DependencyProperty.Register(
                "SecondaryAxis", 
                typeof(RangeAxisBase), 
                typeof(SfChart),
                new PropertyMetadata(null, OnSecondaryAxisChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Watermark"/> property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(
                "Watermark", 
                typeof(Watermark), 
                typeof(SfChart),
                new PropertyMetadata(null, OnWatermarkChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="AreaBorderBrush"/> property.
        /// </summary>
        public static readonly DependencyProperty AreaBorderBrushProperty =
            DependencyProperty.Register("AreaBorderBrush", typeof(Brush), typeof(SfChart), new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="AreaBorderThickness"/> property.
        /// </summary>
        public static readonly DependencyProperty AreaBorderThicknessProperty =
            DependencyProperty.Register(
                "AreaBorderThickness", 
                typeof(Thickness),
                typeof(SfChart),
                new PropertyMetadata(new Thickness(1)));

        /// <summary>
        /// The DependencyProperty for <see cref="AreaBackground"/> property.
        /// </summary>
        public static readonly DependencyProperty AreaBackgroundProperty =
            DependencyProperty.Register("AreaBackground", typeof(Brush), typeof(SfChart), new PropertyMetadata(null));

        /// <summary>
        /// The DependencyProperty for <see cref="Series"/> property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                "Series", 
                typeof(ChartSeriesCollection), 
                typeof(SfChart),
                new PropertyMetadata(null, OnSeriesPropertyCollectionChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="TechnicalIndicators"/> property.
        /// </summary>
        internal static readonly DependencyProperty TechnicalIndicatorsProperty =
            DependencyProperty.Register(
                "TechnicalIndicators",
                typeof(ObservableCollection<ChartSeries>), 
                typeof(SfChart),
                new PropertyMetadata(null, OnTechnicalIndicatorsPropertyChanged));

        /// <summary>
        /// The DependencyProperty for <see cref="Annotations"/> property.
        /// </summary>
        public static readonly DependencyProperty AnnotationsProperty =
            DependencyProperty.Register(
                "Annotations", 
                typeof(AnnotationCollection), 
                typeof(SfChart),
                new PropertyMetadata(null, OnAnnotationsChanged));

        #endregion

        #region Fields

        #region Internal Fields

        internal IAsyncAction renderSeriesAction;

        internal Panel chartAxisPanel;

        internal int currentBitmapPixel = -1;

        internal Point adorningCanvasPoint;

        internal bool isBitmapPixelsConverted;

        internal bool HoldUpdate = false;

        internal ZoomingToolBar zoomingToolBar;

        internal ChartZoomPanBehavior chartZoomBehavior;

        internal WriteableBitmap fastRenderSurface;

        #endregion

        #region Private Fields

        private Panel gridLinesPanel;

        private bool clearPixels = false;

        private Panel seriesPresenter;

        private ChartRootPanel rootPanel;

        List<double> sumItems = new List<double>();

        private ChartSeries previousSeries; //This field used in UpdateBitmapTooltip for storing the series as temp.

#if NETFX_CORE
        bool isTap;
#endif

        private ChartBehaviorsCollection behaviors;

        private Panel internalCanvas = null;

#if __IOS__ || __ANDROID__
        private Border fastRenderDevice = new Border{ Child = new Image() };
#else
        private Image fastRenderDevice = new Image();
#endif

        private byte[] fastBuffer;

        private Stream fastRenderSurfaceStream;
#endregion

#endregion

#region Constructor

        /// <summary>
        /// Called when instance created for SfChart
        /// </summary>
        public SfChart()
        {

#if NETCORE && SyncfusionLicense
            Windows.Shared.LicenseHelper.ValidateLicense();
#endif

#if UNIVERSALWINDOWS && SyncfusionLicense
            SfChart.ValidateLicense();
#endif

            DefaultStyleKey = typeof(SfChart);
            UpdateAction = UpdateAction.Invalidate;
            Series = new ChartSeriesCollection();
            TechnicalIndicators = new ObservableCollection<ChartSeries>();
            VisibleSeries = new ChartVisibleSeriesCollection();
            Axes = new ChartAxisCollection();
            DependentSeriesAxes = new List<ChartAxis>();
            Annotations = new AnnotationCollection();
#if !Uno
            Printing = new Printing(this);
#endif
            Axes.CollectionChanged += Axes_CollectionChanged;
            Behaviors = new ChartBehaviorsCollection(this);
            ColorModel = new ChartColorModel(this.Palette);
            InitializeLegendItems();
        }

        private void Axes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                var axis = e.NewItems[0] as ChartAxisBase2D;
                if (axis != null && axis.Area == null)
                {
                    axis.Area = this;
                }
               
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                var axis = e.OldItems[0] as ChartAxisBase2D;
                if (axis != null)
                {
                    axis.Area = null;
                }
            }

            ScheduleUpdate();
        }

#endregion

#region Events

        /// <summary>
        /// Occurs when chart is zoomed.
        /// </summary>
        public event EventHandler<ZoomChangedEventArgs> ZoomChanged;

        /// <summary>
        /// Occurs during chart zooming.
        /// </summary>
        public event EventHandler<ZoomChangingEventArgs> ZoomChanging;

        /// <summary>
        /// Occurs at the start of selection zooming.
        /// </summary>
        public event EventHandler<SelectionZoomingStartEventArgs> SelectionZoomingStart;

        /// <summary>
        /// Occurs during selection zooming.
        /// </summary>
        public event EventHandler<SelectionZoomingDeltaEventArgs> SelectionZoomingDelta;

        /// <summary>
        /// Occurs at the end of selection zooming.
        /// </summary>
        public event EventHandler<SelectionZoomingEndEventArgs> SelectionZoomingEnd;

        /// <summary>
        /// Occurs when panning is completed.
        /// </summary>
        public event EventHandler<PanChangedEventArgs> PanChanged;

        /// <summary>
        /// Occurs when panning takes place.
        /// </summary>
        public event EventHandler<PanChangingEventArgs> PanChanging;

        /// <summary>
        /// Occurs when the zoom is reset.
        /// </summary>
        public event EventHandler<ResetZoomEventArgs> ResetZooming;


#endregion

#region Properties

#region Public Properties

        /// <summary>
        /// Gets or sets primary axis.
        /// </summary>
        public ChartAxisBase2D PrimaryAxis
        {
            get { return (ChartAxisBase2D)GetValue(PrimaryAxisProperty); }
            set { SetValue(PrimaryAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets secondary axis.
        /// </summary>
        public RangeAxisBase SecondaryAxis
        {
            get { return (RangeAxisBase)GetValue(SecondaryAxisProperty); }
            set { SetValue(SecondaryAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart watermark.
        /// </summary>
        public Watermark Watermark
        {
            get { return (Watermark)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color to paint the outline of chart area
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush AreaBorderBrush
        {
            get { return (Brush)GetValue(AreaBorderBrushProperty); }
            set { SetValue(AreaBorderBrushProperty, value); }
        }

        /// <summary>
        /// Gets or sets the outline thickness of chart area.
        /// </summary>
        public Thickness AreaBorderThickness
        {
            get { return (Thickness)GetValue(AreaBorderThicknessProperty); }
            set { SetValue(AreaBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color to paint the Background of chart area
        /// </summary>
        /// <value>
        /// The <see cref="System.Windows.Media.Brush"/> value.
        /// </value>
        public Brush AreaBackground
        {
            get { return (Brush)GetValue(AreaBackgroundProperty); }
            set { SetValue(AreaBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the collection of chart behaviors
        /// </summary>
        public ChartBehaviorsCollection Behaviors
        {
            get { return behaviors; }
            set { behaviors = value; }
        }

        /// <summary>
        /// Gets or sets collection of series
        /// </summary>
        public ChartSeriesCollection Series
        {
            get { return (ChartSeriesCollection)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets technical indicators
        /// </summary>
        internal ObservableCollection<ChartSeries> TechnicalIndicators
        {
            get { return (ObservableCollection<ChartSeries>)GetValue(TechnicalIndicatorsProperty); }
            set { SetValue(TechnicalIndicatorsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the annotations.
        /// </summary>
        /// <value>
        /// The annotations.
        /// </value>
        public AnnotationCollection Annotations
        {
            get { return (AnnotationCollection)GetValue(AnnotationsProperty); }
            set { SetValue(AnnotationsProperty, value); }
        }

#endregion

#region Internal Properties

        internal bool IsMultipleArea
        {
            get
            {
                return this.RowDefinitions.Count > 1 || this.ColumnDefinitions.Count > 1;
            }
        }        

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed. Suppression is OK here")]
        internal ResourceManager ResourceManager
        {
            set
            {
                SR.ResourceManager = value;
            }
        }

        internal Panel InternalCanvas
        {
            get
            {
                return internalCanvas;
            }
            set
            {
                internalCanvas = value;
            }
        }

        internal Panel GridLinesPanel
        {
            get
            {
                return gridLinesPanel;
            }
        }

        internal bool CanRenderToBuffer
        {
            get;
            set;
        }

        internal Canvas ChartAnnotationCanvas
        {
            get;
            set;
        }

        internal Canvas SeriesAnnotationCanvas
        {
            get;
            set;
        }

        internal AnnotationManager AnnotationManager
        {
            get;
            set;
        }

        internal Canvas BottomAdorningCanvas
        {
            get;
            set;
        }

#endregion

#region Private Properties
        
        private bool disposed;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        bool HasBitmapSeries
        {
            get
            {
                return VisibleSeries.Any(ser => ser.IsBitmapSeries);
            }
        }

#endregion

#endregion

#region Methods

        /// <summary>
        /// Performs application-defined tasks accociated with freeing, releasing, or resetting unmanaged resource in <see cref="SfChart"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;

            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            DisposeAnnotation();

            if (previousSeries != null)
            {
                previousSeries.Dispose();
                previousSeries = null;
            }

            DisposeSeriesAndIndicators();
            DisposeBehaviors();
            DisposeZoomEvents();
            DisposeSelectionEvents();
            DisposeLegend();
            DisposeAxis();

            SizeChanged -= OnSfChartSizeChanged;
            SizeChanged -= OnSizeChanged;

            DisposeRowColumnsDefinitions();
            DisposePanels();

            Watermark = null;
            RootPanelDesiredSize = null;
            GridLinesLayout = null;
            ChartAxisLayoutPanel = null;
            SelectionBehaviour = null;
#if !Uno
            if (Printing != null)
            {
                Printing.Chart = null;
                Printing = null;
            }
#endif
        }

        private void DisposePanels()
        {
            if (rootPanel != null)
            {
                rootPanel.Area = null;
                rootPanel = null;
            }

            var cartesianAxisLayoutPanel = ChartAxisLayoutPanel as ChartCartesianAxisLayoutPanel;
            if (cartesianAxisLayoutPanel != null)
            {
                cartesianAxisLayoutPanel.Area = null;
                cartesianAxisLayoutPanel.Children.Clear();
                cartesianAxisLayoutPanel = null;
            }
            else
            {
                var polarAxisLayoutPanel = ChartAxisLayoutPanel as ChartPolarAxisLayoutPanel;
                if (polarAxisLayoutPanel != null)
                {
                    polarAxisLayoutPanel.Area = null;
                    polarAxisLayoutPanel.PolarAxis = null;
                    polarAxisLayoutPanel.CartesianAxis = null;
                    polarAxisLayoutPanel.Children.Clear();
                    polarAxisLayoutPanel = null;
                }
            }

            var cartesialGridLinesPanel = GridLinesLayout as ChartCartesianGridLinesPanel;
            if (cartesialGridLinesPanel != null)
            {
                cartesialGridLinesPanel.Area = null;
                cartesialGridLinesPanel.Children.Clear();
                cartesialGridLinesPanel = null;
            }
            else
            {
                var polarGridLinesPanl = GridLinesLayout as ChartPolarGridLinesPanel;
                if (polarGridLinesPanl != null)
                {
                    polarGridLinesPanl.Dispose();
                    polarGridLinesPanl = null;
                }
            }

            if (ChartDockPanel != null && ChartDockPanel.Children.Count > 0)
            {
                ChartDockPanel.RootElement = null;
                ChartDockPanel.Children.Clear();
                ChartDockPanel = null;
            }
        }

        private void DisposeAxis()
        {
            if (Axes != null)
            {
                foreach (var axis in Axes)
                {
                    axis.Dispose();
                }
                Axes.Clear();
                Axes.CollectionChanged -= Axes_CollectionChanged;
                Axes = null;
            }

            PrimaryAxis = null;
            SecondaryAxis = null;
            InternalPrimaryAxis = null;
            InternalSecondaryAxis = null;
            InternalDepthAxis = null;
        }

        private void DisposeSeriesAndIndicators()
        {
            if (VisibleSeries != null)
            {
                VisibleSeries.Clear();
            }

            if (ActualSeries != null)
            {
                ActualSeries.Clear();
            }

            if (SelectedSeriesCollection != null)
            {
                SelectedSeriesCollection.Clear();
            }

            CurrentSelectedSeries = null;
            PreviousSelectedSeries = null;

            var seriesColl = Series;
            var indicatorColl = TechnicalIndicators;

            Series = null;
            TechnicalIndicators = null;

            if (seriesColl != null)
            {
                foreach (var series in seriesColl)
                {
                    series.Dispose();
                }
            }

            if (indicatorColl != null)
            {
                foreach (var indicator in indicatorColl)
                {
                    indicator.Dispose();
                }
            }
        }

        private void DisposeBehaviors()
        {
            if (behaviors != null)
            {
                foreach (var behavior in behaviors)
                {
                    var zoomPanBehavior = behavior as ChartZoomPanBehavior;

                    if (zoomPanBehavior != null)
                    {
                        zoomPanBehavior.DisposeZoomEventArguments();
                    }

                    behavior.Dispose();
                }

                behaviors.Area = null;
            }

            if (zoomingToolBar != null)
            {
                zoomingToolBar.Dispose();
                zoomingToolBar = null;
            }

            chartZoomBehavior = null;
            SelectionBehaviour = null;
        }

        private void DisposeLegend()
        {
            if (Legend != null)
            {
                if (LegendItems != null)
                {
                    foreach (var legendItem in LegendItems)
                    {
                        foreach (var item in legendItem)
                        {
                            item.Dispose();
                        }

                        legendItem.Clear();
                    }

                    LegendItems.Clear();
                }

                var chartLegend = Legend as ChartLegend;
                if (chartLegend is ChartLegend)
                {
                    chartLegend.Dispose();
                    chartLegend = null;
                }
                else
                {
                    var legendCollection = Legend as ChartLegendCollection;
                    if (legendCollection != null)
                    {
                        foreach (var legend in legendCollection)
                        {
                            legend.Dispose();
                        }

                        legendCollection.CollectionChanged -= LegendCollectionChanged;
                        legendCollection.Clear();
                    }
                }
            }
        }

        private void DisposeZoomEvents()
        {
            if (ZoomChanged != null)
            {
                foreach (var handler in ZoomChanged.GetInvocationList())
                {                    
                    ZoomChanged -= handler as EventHandler<ZoomChangedEventArgs>;
                }

                ZoomChanged = null;
            }

            if (ZoomChanging != null)
            {
                foreach (var handler in ZoomChanging.GetInvocationList())
                {
                    ZoomChanging -= handler as EventHandler<ZoomChangingEventArgs>;
                }

                ZoomChanging = null;
            }

            if (SelectionZoomingStart != null)
            {
                foreach (var handler in SelectionZoomingStart.GetInvocationList())
                {
                    SelectionZoomingStart -= handler as EventHandler<SelectionZoomingStartEventArgs>;
                }

                SelectionZoomingStart = null;
            }

            if (SelectionZoomingDelta != null)
            {
                foreach (var handler in SelectionZoomingDelta.GetInvocationList())
                {
                    SelectionZoomingDelta -= handler as EventHandler<SelectionZoomingDeltaEventArgs>;
                }

                SelectionZoomingDelta = null;
            }

            if (SelectionZoomingEnd != null)
            {
                foreach (var handler in SelectionZoomingEnd.GetInvocationList())
                {
                    SelectionZoomingEnd -= handler as EventHandler<SelectionZoomingEndEventArgs>;
                }

                SelectionZoomingEnd = null;
            }

            if (PanChanged != null)
            {
                foreach (var handler in PanChanged.GetInvocationList())
                {
                    PanChanged -= handler as EventHandler<PanChangedEventArgs>;
                }

                PanChanged = null;
            }

            if (PanChanging != null)
            {
                foreach (var handler in PanChanging.GetInvocationList())
                {
                    PanChanging -= handler as EventHandler<PanChangingEventArgs>;
                }

                PanChanging = null;
            }

            if (ResetZooming != null)
            {
                foreach (var handler in ResetZooming.GetInvocationList())
                {
                    ResetZooming -= handler as EventHandler<ResetZoomEventArgs>;
                }

                ResetZooming = null;
            }
        }

        private void DisposeAnnotation()
        {
            if (Annotations != null)
            {
                foreach (var annotation in Annotations)
                {
                    annotation.Dispose();
                }
                Annotations.CollectionChanged -= OnAnnotationsCollectionChanged;
                Annotations.Clear();
                Annotations = null;
            }

            if (AnnotationManager != null)
            {
                AnnotationManager.Dispose();
                AnnotationManager = null;
            }
        }

#region Public Override Methods

        /// <summary>
        /// Method used to highlight selected index series.
        /// </summary>
        /// <param name="args"></param>
        public override void SeriesSelectedIndexChanged(int newIndex, int oldIndex)
        {
            //Reset the oldIndex series Interior
            if (oldIndex < Series.Count && oldIndex >= 0
                && SelectionBehaviour.SelectionStyle == SelectionStyle.Single)
            {
                ChartSeries series = Series[oldIndex] as ChartSeries;
                if (SelectedSeriesCollection.Contains(series))
                    SelectedSeriesCollection.Remove(series);

                OnResetSeries(series);
            }

            if (newIndex >= 0 && GetEnableSeriesSelection() && newIndex < VisibleSeries.Count)
            {
                ChartSeries series = Series[newIndex] as ChartSeries;

                if (!SelectedSeriesCollection.Contains(series))
                    SelectedSeriesCollection.Add(series);

                //For adornment selection implementation
                if (series.adornmentInfo is ChartAdornmentInfo && series.adornmentInfo.HighlightOnSelection)
                {
                    List<int> indexes = (from adorment in series.Adornments
                                         select series.Adornments.IndexOf(adorment)).ToList();

                    series.AdornmentPresenter.UpdateAdornmentSelection(indexes, false);
                }

                //Set the SeriestSelectionBrush to newIndex series Interior
                foreach (var segment in series.Segments)
                {
                    segment.BindProperties();
                    segment.IsSelectedSegment = true;
                }

                if (series.IsBitmapSeries)
                    UpdateBitmapSeries(series, false);

                ChartSelectionChangedEventArgs selectionChnagedEventArgs = new ChartSelectionChangedEventArgs()
                {
                    SelectedSegment = null,
                    SelectedSeries = series,
                    SelectedSeriesCollection = SelectedSeriesCollection,
                    SelectedIndex = newIndex,
                    PreviousSelectedIndex = oldIndex,
                    IsDataPointSelection = false,
                    IsSelected = true,
                    PreviousSelectedSegment = null,
                    PreviousSelectedSeries = null
                };

                if (oldIndex != -1)
                    selectionChnagedEventArgs.PreviousSelectedSeries = Series[oldIndex];

                //Raise the selection changed event
                OnSelectionChanged(selectionChnagedEventArgs);

            }
            else if (newIndex == -1)
            {
                OnSelectionChanged(new ChartSelectionChangedEventArgs()
                {
                    SelectedSegment = null,
                    SelectedSeries = null,
                    SelectedSeriesCollection = SelectedSeriesCollection,
                    SelectedIndex = newIndex,
                    PreviousSelectedIndex = oldIndex,
                    IsDataPointSelection = false,
                    IsSelected = false,
                    PreviousSelectedSegment = null,
                    PreviousSelectedSeries = Series[oldIndex]
                });
            }
        }

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        public override double ValueToPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return (axis.RenderedRect.Left - axis.Area.SeriesClipRect.Left)
                        + (axis.ValueToCoefficientCalc(value) * axis.RenderedRect.Width);
                }
                return (axis.RenderedRect.Top - axis.Area.SeriesClipRect.Top) + (1 - axis.ValueToCoefficientCalc(value)) * axis.RenderedRect.Height;
            }

            return double.NaN;
        }

#endregion

#region Public Methods

        /// <summary>
        /// Converts Value to point.
        /// </summary>
        /// <param name="axis">The Chart axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed")]
        public double ValueToPointRelativeToAnnotation(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return (axis.RenderedRect.Left)
                        + (axis.ValueToCoefficientCalc(value) * axis.RenderedRect.Width);
                }
                return (axis.RenderedRect.Top) + (1 - axis.ValueToCoefficientCalc(value)) * axis.RenderedRect.Height;
            }

            return double.NaN;
        }

#endregion

#region Internal Static Methods

        /// <summary>
        /// Method used to gets the byte value of given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        internal static int ConvertColor(Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            return col;
        }

        internal static double PointToAnnotationValue(ChartAxis axis, Point point)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return axis.CoefficientToValueCalc((point.X - axis.RenderedRect.Left) / axis.RenderedRect.Width);
                }
                else
                {
                    return axis.CoefficientToValueCalc(1d - ((point.Y - axis.RenderedRect.Top) / axis.RenderedRect.Height));
                }

            }

            return double.NaN;
        }

#endregion

#region Internal Override Methods

        internal override DependencyObject CloneChart()
        {
            SfChart chart = new SfChart();
            ChartCloning.CloneControl(this, chart);
            chart.Height = double.IsNaN(this.Height) ? this.ActualHeight : this.Height;
            chart.Width = double.IsNaN(this.Width) ? this.ActualWidth : this.Width;
            chart.Header = this.Header;
            chart.Palette = this.Palette;
            chart.AxisThickness = this.AxisThickness;
            chart.AreaBorderBrush = this.AreaBorderBrush;
            chart.AreaBackground = this.AreaBackground;
            chart.AreaBorderThickness = this.AreaBorderThickness;
            chart.SideBySideSeriesPlacement = this.SideBySideSeriesPlacement;
            if (this.PrimaryAxis != null && this.PrimaryAxis is ChartAxisBase2D)
                chart.PrimaryAxis = (ChartAxisBase2D)(this.PrimaryAxis as ICloneable).Clone();
            if (this.SecondaryAxis != null && this.SecondaryAxis is RangeAxisBase)
                chart.SecondaryAxis = (RangeAxisBase)(this.SecondaryAxis as ICloneable).Clone();
            if (this.Legend != null)
            {
                var legends = this.Legend as ChartLegendCollection;

                if (legends != null)
                {
                    var clonedLegends = new ChartLegendCollection();

                    foreach (var legend in legends)
                    {
                        var clonedLegend = (ChartLegend)(legend as ICloneable).Clone();
                        clonedLegends.Add(clonedLegend);
                    }

                    chart.Legend = clonedLegends;
                }
                else
                {
                    chart.Legend = (ChartLegend)(this.Legend as ICloneable).Clone();
                }
            }
            foreach (ChartSeriesBase series in this.Series)
            {
                chart.Series.Add((ChartSeries)(series as ICloneable).Clone());
            }
            foreach (ChartRowDefinition rowDefinition in this.RowDefinitions)
            {
                chart.RowDefinitions.Add((ChartRowDefinition)(rowDefinition as ICloneable).Clone());
            }
            foreach (ChartColumnDefinition columnDefinition in this.ColumnDefinitions)
            {
                chart.ColumnDefinitions.Add((ChartColumnDefinition)(columnDefinition as ICloneable).Clone());
            }
            foreach (Annotation annotation in Annotations)
            {
                chart.Annotations.Add((Annotation)annotation.Clone());
            }
            foreach (FinancialTechnicalIndicator indicators in this.TechnicalIndicators)
            {
                chart.TechnicalIndicators.Add((FinancialTechnicalIndicator)indicators.Clone());
            }
            foreach (ChartBehavior behavior in this.Behaviors)
            {
                chart.Behaviors.Add((ChartBehavior)(behavior as ICloneable).Clone());
            }
            chart.UpdateArea(true);
            return chart;
        }

        internal override void UpdateArea(bool forceUpdate)
        {
            if (updateAreaAction != null || forceUpdate)
            {
                if (disposed)
                    return;

                if (AreaType == ChartAreaType.CartesianAxes)
                {
                    if (ColumnDefinitions.Count == 0)
                        ColumnDefinitions.Add(new ChartColumnDefinition());
                    if (RowDefinitions.Count == 0)
                        RowDefinitions.Add(new ChartRowDefinition());
                }
                if (VisibleSeries == null)
                    return;


                if ((UpdateAction & UpdateAction.Create) == UpdateAction.Create)
                {
                    foreach (ChartSeriesBase series in VisibleSeries)
                    {
                        if (!series.IsPointGenerated)
                            series.GeneratePoints();
                        if (series.ShowTooltip)
                            ShowTooltip = true;
                    }

                    //Initialize default axes for SfChart when PrimaryAxis or SecondayAxis is not set
                    InitializeDefaultAxes();

                    // For stacked grouping scenario
                    if (AreaType == ChartAreaType.CartesianAxes)
                    {
                        foreach (ChartSeriesBase series in VisibleSeries)
                        {
                            ISupportAxes2D cartesianSeries = series as ISupportAxes2D;
                            if (cartesianSeries.XAxis == null && InternalPrimaryAxis != null
                                && !InternalPrimaryAxis.RegisteredSeries.Contains(cartesianSeries))
                                InternalPrimaryAxis.RegisteredSeries.Add(cartesianSeries);
                            if (cartesianSeries.YAxis == null && InternalSecondaryAxis != null
                                && !InternalSecondaryAxis.RegisteredSeries.Contains(cartesianSeries))
                                InternalSecondaryAxis.RegisteredSeries.Add(cartesianSeries);
                        }
                    }

                    if (Series != null && Series.Count > 0)
                    {
                        if (InternalPrimaryAxis == null || !Axes.Contains(InternalPrimaryAxis))
                            InternalPrimaryAxis = Series[0].ActualXAxis;
                        if (InternalSecondaryAxis == null || !Axes.Contains(InternalSecondaryAxis))
                            InternalSecondaryAxis = Series[0].ActualYAxis;
                    }

                    //Add selected index while loading 
                    if (!IsChartLoaded && SelectionBehaviour != null)
                    {
                        foreach (var series in VisibleSeries)
                        {
                            var segmentSelectableSeries = series as ISegmentSelectable;
                            if (segmentSelectableSeries != null && segmentSelectableSeries.SelectedIndex >= 0
                                && GetEnableSegmentSelection())
                            {
                                int index = segmentSelectableSeries.SelectedIndex;
                                if (!series.SelectedSegmentsIndexes.Contains(index))
                                    series.SelectedSegmentsIndexes.Add(index);
                            }
                        }

                        if (GetEnableSeriesSelection() && SeriesSelectedIndex >= 0)
                        {
                            ChartSeriesBase series = VisibleSeries[SeriesSelectedIndex];

                            if (!SelectedSeriesCollection.Contains(series))
                                SelectedSeriesCollection.Add(series);
                        }
                    }

                    if (((InternalPrimaryAxis is CategoryAxis) && (!(InternalPrimaryAxis as CategoryAxis).IsIndexed)))
                        CategoryAxisHelper.GroupData(VisibleSeries);

                    foreach (ChartSeriesBase series in VisibleSeries)
                    {
                        series.Invalidate();
                    }
                    if (ShowTooltip && this.Tooltip == null)
                        this.Tooltip = new ChartTooltip();

                    if (TechnicalIndicators != null && AreaType == ChartAreaType.CartesianAxes)
                    {
                        foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                        {
                            if (!indicator.IsPointGenerated)
                            {
                                if (indicator.ItemsSource == null && VisibleSeries.Count > 0
                                    && this.Series != null && this.Series.Count > 0)
                                {
                                    ChartSeriesBase series = this.Series[indicator.Name] ?? this.Series[0];
                                    indicator.SetSeriesItemSource(series);
                                }
                                else
                                    indicator.GeneratePoints();
                            }
                            indicator.Invalidate();
                        }
                    }
                }

                if (IsUpdateLegend && (this.ChartDockPanel != null))
                {
                    UpdateLegend(Legend, false);
                    IsUpdateLegend = false;
                }
                if ((UpdateAction & UpdateAction.UpdateRange) == UpdateAction.UpdateRange)
                {
                    foreach (ChartSeriesBase series in VisibleSeries)
                    {
                        series.UpdateRange();
                    }

                    if (TechnicalIndicators != null)
                    {
                        foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                        {
                            indicator.UpdateRange();
                        }
                    }
                }


                if (RootPanelDesiredSize != null)
                {
                    if ((UpdateAction & UpdateAction.Layout) == UpdateAction.Layout)
                        LayoutAxis(RootPanelDesiredSize.Value);
                    UpdateLegendArrangeRect();
                    if ((UpdateAction & UpdateAction.Render) == UpdateAction.Render)
                    {
                        if (!IsChartLoaded)
                        {
                            ScheduleRenderSeries();
                            IsChartLoaded = true;

                            //Raise the SelectionChanged event when SeriesSelectedIndex is set at chart load time.
                            if (SeriesSelectedIndex >= 0 && VisibleSeries.Count > 0 && GetEnableSeriesSelection())
                                RaiseSeriesSelectionChangedEvent();
                        }

                        else if (renderSeriesAction == null)
                        {
                            RenderSeries();
                        }
                    }
                }

                UpdateAction = UpdateAction.Invalidate;
                updateAreaAction = null;

                if (Behaviors != null)
                {
                    foreach (var behavior in Behaviors)
                    {
                        behavior.OnLayoutUpdated();
                    }
                }
            }
        }

        internal override void UpdateAxisLayoutPanels()
        {
            if (internalCanvas != null)
                internalCanvas.Clip = null;
            AxisThickness = new Thickness(0);
            if (ChartAxisLayoutPanel != null)
            {
                ChartAxisLayoutPanel.DetachElements();
            }

            if (GridLinesLayout != null)
            {
                GridLinesLayout.DetachElements();
            }

            if (chartAxisPanel != null)
            {
                if (AreaType == ChartAreaType.PolarAxes)
                {
                    ChartAxisLayoutPanel = new ChartPolarAxisLayoutPanel(chartAxisPanel)
                    {
                        Area = this
                    };
                    ChartAxisLayoutPanel.UpdateElements();
                    GridLinesLayout = new ChartPolarGridLinesPanel(gridLinesPanel)
                    {
                        Area = this
                    };
                }
                else if (AreaType == ChartAreaType.CartesianAxes)
                {
                    ChartAxisLayoutPanel = new ChartCartesianAxisLayoutPanel(chartAxisPanel)
                    {
                        Area = this
                    };
                    ChartAxisLayoutPanel.UpdateElements();
                    GridLinesLayout = new ChartCartesianGridLinesPanel(gridLinesPanel)
                    {
                        Area = this
                    };
                }
                else
                {
                    ChartAxisLayoutPanel = null;
                    GridLinesLayout = null;
                }
            }
        }

        /// <summary>
        /// Converts Value to Log point.
        /// </summary>
        /// <param name="axis">The Logarithmic axis .</param>
        /// <param name="value">The value.</param>
        /// <returns>The double value to point</returns>
        internal override double ValueToLogPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                var logarithmicAxis = axis as LogarithmicAxis;
                value = logarithmicAxis != null ? Math.Log(value, logarithmicAxis.LogarithmicBase) : value;
                return ValueToPoint(axis, value);
            }
            return double.NaN;
        }

#endregion

#region Internal Methods

        internal void AnnotationsChanged(DependencyPropertyChangedEventArgs args)
        {
            AnnotationCollection newAnnotations = args.NewValue as AnnotationCollection;
            AnnotationCollection oldAnnotations = args.OldValue as AnnotationCollection;

            if (oldAnnotations != null)
                (oldAnnotations as AnnotationCollection).CollectionChanged -= OnAnnotationsCollectionChanged;

            if (newAnnotations != null)
                (newAnnotations as AnnotationCollection).CollectionChanged += OnAnnotationsCollectionChanged;

            if (newAnnotations != null && newAnnotations.Count > 0)
            {
                if (this.AnnotationManager != null)
                    AnnotationManager.Annotations = newAnnotations;
                else if (IsTemplateApplied)
                {
                    AnnotationManager = new AnnotationManager() { Chart = this, Annotations = newAnnotations };

                    //Updating the annotation clips manually in dynamic case, since it is updated only at chart schedule update.
                    UpdateAnnotationClips();
                }
            }
        }

        //This case is mainly for the annotation adding dynamically.
        internal void OnAnnotationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AnnotationManager == null && IsTemplateApplied)
            {
                AnnotationManager = new AnnotationManager() { Chart = this, Annotations = Annotations };

                //Updating the annotation clips manually in dynamic case, since it is updated only at chart schedule update.
                UpdateAnnotationClips();

                //Removing the CollectionChanged since the annotation manager's collection is hooked while setting Annotations.
                (sender as AnnotationCollection).CollectionChanged -= OnAnnotationsCollectionChanged;
            }
        }

        internal void ChangeToolBarState()
        {
            if (zoomingToolBar.ItemsSource != null)
            {
                var toolBarItems = (from item in zoomingToolBar.ItemsSource as List<ZoomingToolBarItem> where item is ZoomOut || item is ZoomReset select item).ToList();
                foreach (var item in toolBarItems)
                {
                    item.IsEnabled = true;
                    item.IconBackground = item.EnableColor;
                }
            }
        }

        internal void ResetToolBarState()
        {
			// For Uno prefer ItemsSource because Items is not well supported - https://github.com/unoplatform/uno/issues/1045
			foreach (ZoomingToolBarItem item in zoomingToolBar.ItemsSource as List<ZoomingToolBarItem>)
            {
                if ((item is ZoomOut) || (item is ZoomReset))
                {
                    item.IconBackground = item.DisableColor;
                    item.IsEnabled = false;
                }
                else if (item is ZoomPan)
                {
                    item.IconBackground = item.DisableColor;
                }
                else if (item is SelectionZoom)
                {
                    item.IconBackground = item.EnableColor;
                    chartZoomBehavior.InternalEnableSelectionZooming = true;
                }
            }
        }

        internal void OnResetSeries(ChartSeries series)
        {
            foreach (var segment in series.Segments)
            {
                segment.BindProperties();
                segment.IsSelectedSegment = false;
            }

            if (series.IsBitmapSeries)
                UpdateBitmapSeries(series, true);

            if (series.adornmentInfo is ChartAdornmentInfo)
                series.AdornmentPresenter.ResetAdornmentSelection(null, true);

            foreach (var index in series.SelectedSegmentsIndexes)
            {
                var segmentSelectableSeries = series as ISegmentSelectable;
                if (segmentSelectableSeries != null && index > -1 && GetEnableSegmentSelection())
                {
                    if (series.adornmentInfo is ChartAdornmentInfo && series.adornmentInfo.HighlightOnSelection)
                        series.UpdateAdornmentSelection(index);

                    if (series.IsBitmapSeries)
                    {
                        series.dataPoint = series.GetDataPoint(index);

                        if (series.dataPoint != null && segmentSelectableSeries.SegmentSelectionBrush != null)
                        {
                            //Generate pixels for the particular data point
                            if (series.Segments.Count > 0) series.GeneratePixels();

                            //Set the SegmentSelectionBrush to the selected segment pixels
                            series.OnBitmapSelection(series.selectedSegmentPixels, segmentSelectableSeries.SegmentSelectionBrush, true);
                        }
                    }
                }
            }
        }

        internal void CreateFastRenderSurface()
        {
            if (this.seriesPresenter != null && this.seriesPresenter.Children.Contains(fastRenderDevice) &&
                !SeriesClipRect.IsEmpty && SeriesClipRect.Width > 0 && SeriesClipRect.Height > 0)
            {
                if (fastRenderDevice != null)
                {
                    fastRenderDevice.PointerMoved -= FastRenderDevicePointerMoved;
                    fastRenderDevice.PointerExited -= FastRenderDevicePointerExited;
                    fastRenderDevice.Tapped -= FastRenderDevice_Tapped;
                    this.fastRenderSurface = new WriteableBitmap((int)SeriesClipRect.Width, (int)SeriesClipRect.Height);

#if __IOS__ || __ANDROID__
                    var dev = (Image)this.fastRenderDevice.Child;
#else
                    var dev = this.fastRenderDevice;
#endif
                    dev.Height = SeriesClipRect.Height;
                    dev.Width = SeriesClipRect.Width;
                    dev.Source = this.fastRenderSurface;

#if NETFX_CORE
                    this.fastRenderSurfaceStream = this.fastRenderSurface.PixelBuffer.AsStream();
                    CreateBuffer(new Size(SeriesClipRect.Width, SeriesClipRect.Height));
#endif
                    fastRenderDevice.PointerMoved += FastRenderDevicePointerMoved;
                    fastRenderDevice.PointerExited += FastRenderDevicePointerExited;
                    fastRenderDevice.Tapped += FastRenderDevice_Tapped;
                }
            }
        }

        internal void AddOrRemoveBitmap()
        {
            if (this.seriesPresenter != null && this.seriesPresenter.Children.Contains(fastRenderDevice) && !HasBitmapSeries)
            {
                this.seriesPresenter.Children.Remove(fastRenderDevice);
                this.fastRenderSurface = null;
#if NETFX_CORE
                this.fastRenderSurfaceStream = null;
#endif
                this.fastBuffer = null;
            }
            else if (this.seriesPresenter != null && !this.seriesPresenter.Children.Contains(fastRenderDevice) && HasBitmapSeries)
            {
                this.seriesPresenter.Children.Insert(0, fastRenderDevice);
                if (fastRenderSurface == null)
                    this.CreateFastRenderSurface();
            }
        }

        /// <summary>
        /// Method used to update selection in bitmap series.
        /// </summary>
        /// <param name="series"></param>
        internal void UpdateBitmapSeries(ChartSeries bitmapSeries, bool isReset)
        {
            int seriesIndex = Series.IndexOf(bitmapSeries);

            if (!isBitmapPixelsConverted)
                ConvertBitmapPixels();

            //Gets the upper series from the selected series
            var upperSeriesCollection = (from series in Series
                                         where Series.IndexOf(series) > seriesIndex
                                         select series).ToList();

            //Gets the upper series pixels in to single collection
            foreach (var series in upperSeriesCollection)
            {
                bitmapSeries.upperSeriesPixels.UnionWith(series.Pixels);
            }

            {
                byte[] buffer = GetFastBuffer();
                int j = 0;
                Color uiColor;
                Brush brush = GetSeriesSelectionBrush(bitmapSeries);

                if (isReset)
                {
                    if (bitmapSeries is FastLineBitmapSeries || bitmapSeries is FastStepLineBitmapSeries
                        || bitmapSeries is FastRangeAreaBitmapSeries)
                    {
                        ScheduleRenderSeries();
                    }
                    else
                    {
                        for (int i = 0; i < bitmapSeries.DataCount; i++)
                        {
                            bitmapSeries.dataPoint = bitmapSeries.GetDataPoint(i);

                            if (bitmapSeries.dataPoint != null)
                            {
                                //Generate pixels for the particular data point
                                if (bitmapSeries.Segments.Count > 0) bitmapSeries.GeneratePixels();

                                if (bitmapSeries is FastHiLoOpenCloseBitmapSeries)
                                {
                                    uiColor = ((bitmapSeries.Segments[0] as FastHiLoOpenCloseSegment).GetSegmentBrush(i));
                                }
                                else if (bitmapSeries is FastCandleBitmapSeries)
                                {
                                    uiColor = ((bitmapSeries.Segments[0] as FastCandleBitmapSegment).GetSegmentBrush(i));
                                }
                                else
                                    uiColor = ((SolidColorBrush)bitmapSeries.GetInteriorColor(i)).Color;

                                foreach (var pixel in bitmapSeries.selectedSegmentPixels)
                                {
                                    if (!bitmapSeries.upperSeriesPixels.Contains(pixel))
                                    {
                                        if (j == 0)
                                        {
                                            buffer[pixel] = uiColor.B;
                                            j = j + 1;
                                        }
                                        else if (j == 1)
                                        {
                                            buffer[pixel] = uiColor.G;
                                            j = j + 1;
                                        }
                                        else if (j == 2)
                                        {
                                            buffer[pixel] = uiColor.R;
                                            j = j + 1;
                                        }
                                        else if (j == 3)
                                        {
                                            buffer[pixel] = uiColor.A;
                                            j = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (brush != null)
                {
                    uiColor = (brush as SolidColorBrush).Color;

                    foreach (var pixel in bitmapSeries.Pixels)
                    {
                        if (!bitmapSeries.upperSeriesPixels.Contains(pixel))
                        {
                            if (j == 0)
                            {
                                buffer[pixel] = uiColor.B;
                                j = j + 1;
                            }
                            else if (j == 1)
                            {
                                buffer[pixel] = uiColor.G;
                                j = j + 1;
                            }
                            else if (j == 2)
                            {
                                buffer[pixel] = uiColor.R;
                                j = j + 1;
                            }
                            else if (j == 3)
                            {
                                buffer[pixel] = uiColor.A;
                                j = 0;
                            }
                        }
                    }
                }

                RenderToBuffer();
            }

            bitmapSeries.upperSeriesPixels.Clear();
        }

        internal void UpdateStripLines()
        {
            if (GridLinesLayout != null && (GridLinesLayout is ChartCartesianGridLinesPanel))
            {
                (GridLinesLayout as ChartCartesianGridLinesPanel).UpdateStripLines();
                ScheduleUpdate();
            }
            else if (GridLinesLayout != null && (GridLinesLayout is ChartPolarGridLinesPanel))
            {
                (GridLinesLayout as ChartPolarGridLinesPanel).UpdateStripLines();
                ScheduleUpdate();
            }
        }

        /// <summary>
        /// Set default axes for SfChart
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal void InitializeDefaultAxes()
        {
#if NETFX_CORE
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
#endif
          
                if (PrimaryAxis != null && !Axes.Contains(PrimaryAxis))
                    ClearValue(PrimaryAxisProperty);
                if (SecondaryAxis != null && !Axes.Contains(SecondaryAxis))
                    ClearValue(SecondaryAxisProperty);
#if NETFX_CORE
            }
#endif
            if (PrimaryAxis == null || PrimaryAxis.IsDefault)
            {
                if ((Series == null || Series.Count == 0) && (TechnicalIndicators == null 
                    || TechnicalIndicators.Count == 0))
                {
                    if (PrimaryAxis == null)
                        PrimaryAxis = new NumericalAxis() { IsDefault = true };
                }
                else
                {
                    if (Series != null)
                    {
                        var chartSeries = (from series in Series
                                           where (series is HistogramSeries) || (series is CartesianSeries && ((series as CartesianSeries).XAxis == null))
                                           || (series is PolarRadarSeriesBase && (series as PolarRadarSeriesBase).XAxis == null)
                                           select series).ToList();
                        if (chartSeries.Count != 0)
                        {
                            //get the XAxisValueType from the each series in Series collection 
                            var valueTypes = (from series in Series
                                              where (series is HistogramSeries) ||
                                              (series is CartesianSeries && (series.ActualXAxis == null || !Axes.Contains(series.ActualXAxis)
                                              || series.ActualXAxis.IsDefault) || series is PolarRadarSeriesBase && (series.ActualXAxis == null || !Axes.Contains(series.ActualXAxis) || (series.ActualXAxis.IsDefault)))
                                              select series.XAxisValueType).ToList();

                            if (valueTypes.Count > 0)
                                SetPrimaryAxis(valueTypes[0]);//Set PrimaryAxis for SfChart based on XAxisValueType
                            else
                            {
                                PrimaryAxis = PrimaryAxis != null ? Series[0].ActualXAxis as ChartAxisBase2D : PrimaryAxis;
                                if (Annotations != null)
                                    foreach (var annotation in Annotations)
                                        annotation.SetAxisFromName();
                            }
                        }
                        else if (TechnicalIndicators != null)
                        {
                            chartSeries = (from series in TechnicalIndicators
                                           where (series is FinancialTechnicalIndicator && (series as FinancialTechnicalIndicator).YAxis == null)
                                           select series).ToList();
                            if (chartSeries.Count != 0)
                            {
                                var valueTypes = (from series in TechnicalIndicators
                                                  where (series is FinancialTechnicalIndicator && (series.ActualXAxis == null || !Axes.Contains(series.ActualXAxis)
                                                  || series.ActualXAxis.IsDefault))
                                                  select series.XAxisValueType).ToList();
                                if (valueTypes.Count > 0)
                                    SetPrimaryAxis(valueTypes[0]);
                            }
                            else
                                this.ClearValue(PrimaryAxisProperty);
                        }
                        else
                            this.ClearValue(PrimaryAxisProperty);
                    }
                }
            }

            if (SecondaryAxis == null || SecondaryAxis.IsDefault)
            {
                if ((Series == null || Series.Count == 0) && (TechnicalIndicators == null ||
                     TechnicalIndicators.Count == 0))
                {
                    if (SecondaryAxis == null)
                        SecondaryAxis = new NumericalAxis() { IsDefault = true };
                }
                else
                {
                    if (Series != null)
                    {
                        var chartSeries = (from series in Series
                                           where (series is HistogramSeries) || (series is CartesianSeries && ((series as CartesianSeries).YAxis == null))
                                           || (series is PolarRadarSeriesBase && (series as PolarRadarSeriesBase).YAxis == null)
                                           select series).ToList();
                        if (chartSeries.Count != 0)
                        {
                            var seriesCollection = (from series in Series
                                                    where (series is HistogramSeries) ||
                                                    (series is CartesianSeries && (series.ActualYAxis == null || !Axes.Contains(series.ActualYAxis)
                                                    || series.ActualYAxis.IsDefault) || (series is PolarRadarSeriesBase && (series.ActualYAxis == null || !Axes.Contains(series.ActualYAxis)
                                                    || series.ActualYAxis.IsDefault)))
                                                    select series).ToList();

                            if (seriesCollection.Count > 0 && SecondaryAxis == null)
                            {
                                if (Axes.Contains(InternalSecondaryAxis))
                                    Axes.Remove(InternalSecondaryAxis);
                                SecondaryAxis = new NumericalAxis() { IsDefault = true };
                            }
                            else
                            {
                                SecondaryAxis = SecondaryAxis != null ? SecondaryAxis : Series[0].ActualYAxis as RangeAxisBase;
                                if (Annotations != null)
                                    foreach (var annotation in Annotations)
                                        annotation.SetAxisFromName();
                            }
                        }
                        else if (TechnicalIndicators != null)
                        {
                            chartSeries = (from series in TechnicalIndicators
                                               where (series is FinancialTechnicalIndicator && (series as FinancialTechnicalIndicator).XAxis == null)
                                               select series).ToList();
                            if (chartSeries.Count != 0)
                            {
                                var seriesCollection = (from series in TechnicalIndicators
                                                        where (series is FinancialTechnicalIndicator && (series.ActualYAxis == null || !Axes.Contains(series.ActualYAxis)
                                                        || series.ActualYAxis.IsDefault))
                                                        select series).ToList();
                                if (seriesCollection.Count > 0 && SecondaryAxis == null)
                                    SecondaryAxis = new NumericalAxis() { IsDefault = true };
                                else
                                {
                                    SecondaryAxis = SecondaryAxis != null ? SecondaryAxis : TechnicalIndicators[0].ActualYAxis as RangeAxisBase;
                                }
                            }
                            else
                                this.ClearValue(SecondaryAxisProperty);
                        }
                        else
                            this.ClearValue(SecondaryAxisProperty);
                    }
                }
            }
        }

        /// <summary>
        /// Method is used to convert list collection in to hashset.
        /// </summary>
        internal void ConvertBitmapPixels()
        {
            foreach (var series in Series)
            {
                if (series.bitmapPixels.Count > 0)
                    series.Pixels = new HashSet<int>(series.bitmapPixels);
                series.bitmapPixels.Clear();
            }

            isBitmapPixelsConverted = true;
        }

        /// <summary>
        ///Set PrimaryAxis for SfChart
        /// </summary>
        internal void SetPrimaryAxis(ChartValueType type)
        {
            if (PrimaryAxis == null && Axes.Contains(InternalPrimaryAxis))
                Axes.Remove(InternalPrimaryAxis);
            switch (type)
            {
                case ChartValueType.Double:
                    if (PrimaryAxis == null || PrimaryAxis.GetType() != typeof(NumericalAxis))
                        PrimaryAxis = new NumericalAxis() { IsDefault = true };
                    break;
                case ChartValueType.DateTime:
                    if (PrimaryAxis == null || PrimaryAxis.GetType() != typeof(DateTimeAxis))
                        PrimaryAxis = new DateTimeAxis() { IsDefault = true };
                    break;
                case ChartValueType.String:
                    if (PrimaryAxis == null || PrimaryAxis.GetType() != typeof(CategoryAxis))
                        PrimaryAxis = new CategoryAxis() { IsDefault = true };
                    break;
                case ChartValueType.TimeSpan:
                    if (PrimaryAxis == null || PrimaryAxis.GetType() != typeof(TimeSpanAxis))
                        PrimaryAxis = new TimeSpanAxis() { IsDefault = true };
                    break;
            }
        }

        internal void RenderSeries()
        {
            if (RootPanelDesiredSize != null)
            {
                clearPixels = true;

                byte[] previousBuffer = this.fastBuffer;


                var size = AreaType != ChartAreaType.None
                               ? new Size(SeriesClipRect.Width, SeriesClipRect.Height)
                               : RootPanelDesiredSize.Value;


                if (VisibleSeries != null)
                {
                    isBitmapPixelsConverted = false;
                    foreach (ChartSeriesBase series in VisibleSeries.Where(item => item.Visibility == Visibility.Visible))
                    {
                        series.UpdateOnSeriesBoundChanged(size);
                    }
                }

                if (TechnicalIndicators != null)
                {
                    foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                    {
                        indicator.UpdateOnSeriesBoundChanged(size);
                    }
                }

                if (!CanRenderToBuffer)
                    this.fastBuffer = previousBuffer;
                RenderToBuffer();
            }

            renderSeriesAction = null;
            StackedValues = null;
        }

        internal double AreaValueToPoint(ChartAxis axis, double value)
        {
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return axis.ValueToPoint(value) + GetOffetValue(axis);
                }
                return axis.ValueToPoint(value) + GetOffetValue(axis);

            }
            return double.NaN;
        }

        internal byte[] GetFastBuffer()
        {
            return this.fastBuffer;
        }

        internal WriteableBitmap GetFastRenderSurface()
        {
            return this.fastRenderSurface;
        }

        internal void CreateBuffer(Size size)
        {
            CanRenderToBuffer = false;
            this.fastBuffer = new byte[(int)(size.Width) * (int)(size.Height) * 4];
        }

        internal void ClearBuffer()
        {
            if (clearPixels)
            {
                if (this.fastRenderSurfaceStream != null)
                {
                    CreateBuffer(new Size(SeriesClipRect.Width, SeriesClipRect.Height));
                    this.fastRenderSurface.Clear(this.fastRenderSurfaceStream, this.fastBuffer);
                    this.fastRenderSurface.Invalidate();
                }

                clearPixels = false;
            }
        }

        internal void RenderToBuffer()
        {
            if (this.fastRenderSurfaceStream != null && this.fastBuffer != null)
            {
                this.fastRenderSurfaceStream.Position = 0;
                this.fastRenderSurfaceStream.Write(this.fastBuffer, 0, this.fastBuffer.Count());
                this.fastRenderSurface.Invalidate();
            }

            CanRenderToBuffer = false;
        }

        internal void ScheduleRenderSeries()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                RenderSeries();
            else if (renderSeriesAction == null)
                renderSeriesAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, RenderSeries);
        }

        internal double GetPercentage(IList<ISupportAxes> seriesColl, double item, int index, bool reCalculation)
        {
            double totalValues = 0;
            if (reCalculation)
            {
                if (index == 0)
                    sumItems.Clear();
                foreach (var stackingSeries in seriesColl)
                {
                    StackingSeriesBase stackingChart = stackingSeries as StackingSeriesBase;
                    if (!stackingChart.IsSeriesVisible) continue;
                    if (stackingChart != null && stackingChart.YValues.Count != 0)
                    {
                        double value = index < stackingChart.YValues.Count ? stackingChart.YValues[index] : 0;
                        if (double.IsNaN(value))
                            value = 0;
                        totalValues += Math.Abs(value);
                    }
                }
                sumItems.Add(totalValues);
            }
            if (sumItems.Count != 0)
                item = (item / sumItems[index]) * 100;
            return item;
        }

#endregion

#region Protected Internal Override Methods

        /// <summary>
        /// Raises the <see cref="E:SeriesBoundsChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ChartSeriesBoundsEventArgs"/> instance containing the event data.</param>
        protected internal override void OnSeriesBoundsChanged(ChartSeriesBoundsEventArgs args)
        {
            CreateFastRenderSurface();

            if (InternalCanvas != null)
            {
                InternalCanvas.Clip = new RectangleGeometry()
                {
                    Rect =
                            new Rect(0, 0, SeriesClipRect.Width + 0.5, SeriesClipRect.Height + 0.5)
                };
            }
            base.OnSeriesBoundsChanged(args);
        }

#endregion

#region Protected Internal Virtual Methods

        /// <summary>
        /// Occurs when zooming is done.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnZoomChanged(ZoomChangedEventArgs args)
        {
            if (ZoomChanged != null && args != null)
            {
                ZoomChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when zooming takes place.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnZoomChanging(ZoomChangingEventArgs args)
        {
            if (ZoomChanging != null && args != null)
            {
                ZoomChanging(this, args);
            }
        }

        /// <summary>
        /// Occurs at the start of selection zooming.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnSelectionZoomingStart(SelectionZoomingStartEventArgs args)
        {
            if (SelectionZoomingStart != null && args != null)
            {
                SelectionZoomingStart(this, args);
            }
        }

        /// <summary>
        /// Occurs at the end of selection zooming.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnSelectionZoomingEnd(SelectionZoomingEndEventArgs args)
        {
            if (SelectionZoomingEnd != null && args != null)
            {
                SelectionZoomingEnd(this, args);
            }
        }

        /// <summary>
        /// Occurs while selection zooming.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnSelectionZoomingDelta(SelectionZoomingDeltaEventArgs args)
        {
            if (SelectionZoomingDelta != null && args != null)
            {
                SelectionZoomingDelta(this, args);
            }
        }

        /// <summary>
        /// Occurs when panning is completed.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnPanChanged(PanChangedEventArgs args)
        {
            if (PanChanged != null && args != null)
            {
                PanChanged(this, args);
            }
        }

        /// <summary>
        /// Occurs when panning takes place.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnPanChanging(PanChangingEventArgs args)
        {
            if (PanChanging != null && args != null)
            {
                PanChanging(this, args);
            }
        }

        /// <summary>
        /// Occurs when zoom is reset.
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void OnResetZoom(ResetZoomEventArgs args)
        {
            if (ResetZooming != null && args != null)
            {
                ResetZooming(this, args);
            }
        }

#endregion

#region Protected Internal Methods

        protected internal void AddZoomToolBar(ZoomingToolBar chartZoomingToolBar, ChartZoomPanBehavior zoomBehavior)
        {
            zoomingToolBar = chartZoomingToolBar;
            chartZoomBehavior = zoomBehavior;
            this.ToolkitCanvas.Children.Add(chartZoomingToolBar);
        }

        protected internal void RemoveZoomToolBar(ZoomingToolBar chartZoomingToolBar)
        {
            this.ToolkitCanvas.Children.Remove(chartZoomingToolBar);
            zoomingToolBar = null;
            chartZoomBehavior = null;
        }

#endregion

#region Protected Override Methods

        /// <summary>
        /// Invoke to render sfchart
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.SizeChanged += OnSfChartSizeChanged;
            if (seriesPresenter != null &&
                seriesPresenter.Children.Contains(fastRenderDevice))
            {
                seriesPresenter.Children.Remove(fastRenderDevice);
            }

            seriesPresenter = GetTemplateChild("seriesPresenter") as Panel;
            chartAxisPanel = GetTemplateChild("PART_chartAxisPanel") as Panel;
            gridLinesPanel = GetTemplateChild("gridLines") as Panel;
            InternalCanvas = GetTemplateChild("InternalCanvas") as Panel;
            AdorningCanvas = GetTemplateChild("adorningCanvas") as Canvas;
            ChartDockPanel = GetTemplateChild("Part_DockPanel") as ChartDockPanel;
            rootPanel = GetTemplateChild("LayoutRoot") as ChartRootPanel;
            rootPanel.Area = this;
            ChartAnnotationCanvas = this.GetTemplateChild("Part_ChartAnnotationCanvas") as Canvas;
            SeriesAnnotationCanvas = this.GetTemplateChild("Part_SeriesAnnotationCanvas") as Canvas;

            if (Annotations != null && Annotations.Count > 0)
                AnnotationManager = new AnnotationManager { Chart = this, Annotations = this.Annotations };

            BottomAdorningCanvas = this.GetTemplateChild("bottomAdorningCanvas") as Canvas;
            ToolkitCanvas = this.GetTemplateChild("Part_ToolkitCanvas") as Canvas;
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.AdorningCanvas = AdorningCanvas;
                behavior.BottomAdorningCanvas = BottomAdorningCanvas;
                behavior.InternalAttachElements();
            }

            if (Series != null)
            {
                foreach (ChartSeries series in Series)
                {
                    series.Area = this;
                    if (series.ShowTooltip)
                        ShowTooltip = true;
                }
            }
            if (ShowTooltip)
                this.Tooltip = new ChartTooltip();
            if (TechnicalIndicators != null)
            {
                foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                {
                    indicator.Area = this;
                }
            }
            foreach (ChartAxis axis in Axes)
            {
                axis.Area = this;
            }
            UpdateAxisLayoutPanels();

            if (seriesPresenter != null)
            {
                AddOrRemoveBitmap();
                if (Series != null)
                {
                    foreach (ChartSeriesBase series in Series)
                    {
                        this.seriesPresenter.Children.Add(series);

                        var doughnutSeries = series as DoughnutSeries;

                        if (doughnutSeries != null && doughnutSeries.CenterView != null)
                        {
                            if (!this.seriesPresenter.Children.Contains(doughnutSeries.CenterView))
                                this.seriesPresenter.Children.Add(doughnutSeries.CenterView);
                        }
                    }
                }
                if (TechnicalIndicators != null)
                {
                    foreach (FinancialTechnicalIndicator indicator in TechnicalIndicators)
                    {
                        if (!this.seriesPresenter.Children.Contains(indicator))
                            this.seriesPresenter.Children.Add(indicator);
                    }
                }
            }


            UpdateLegend(Legend, true);
            if (Watermark != null)
                AddOrRemoveWatermark(Watermark, null);
            IsTemplateApplied = true;
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        /// <param name="availableSize"></param>
        protected override Size MeasureOverride(Size availableSize)
        {
            bool needForceSizeChanged = false;
            double width = availableSize.Width, height = availableSize.Height;

            if (double.IsInfinity(width))
            {
                width = ActualWidth == 0d ? 500d : ActualWidth;
                needForceSizeChanged = true;
            }
            if (double.IsInfinity(height))
            {
                height = ActualHeight == 0d ? 300d : ActualHeight;
                needForceSizeChanged = true;
            }
            if (needForceSizeChanged)
            {
                SizeChanged -= OnSizeChanged;
                SizeChanged += OnSizeChanged;
                AvailableSize = new Size(width, height);
            }
            else
                AvailableSize = availableSize;

            return base.MeasureOverride(AvailableSize);
        }


        /// <summary>
        /// called when lost focus from the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnLostFocus(e);
            }

            base.OnLostFocus(e);
        }
        /// <summary>
        /// Called when got focus in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnGotFocus(e);
            }

            base.OnGotFocus(e);
        }

        /// <summary>
        /// Called when Pointercapture lost in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerCaptureLost(e);
            }

            base.OnPointerCaptureLost(e);
        }

        /// <summary>
        /// Called when Tapped in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnTapped(e);
            }

            base.OnTapped(e);
        }

        /// <summary>
        /// Called when RightTap click in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnRightTapped(e);
            }

            base.OnRightTapped(e);
        }

        /// <summary>
        /// Called when Pointerwheel changed in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerWheelChanged(e);
            }

            base.OnPointerWheelChanged(e);
        }

        /// <summary>
        /// Called when PointerExited from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerExited(e);
            }

            base.OnPointerExited(e);
        }

        /// <summary>
        /// Called when pointer entered in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerEntered(e);
            }

            base.OnPointerEntered(e);
        }

        /// <summary>
        /// Called when PointerCancelled in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerCanceled(e);
            }

            base.OnPointerCanceled(e);
        }

        /// <summary>
        /// called when pointer key up in the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnKeyUp(e);
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Called when key down in the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnKeyDown(e);
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Called when holding the pointer in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHolding(HoldingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnHolding(e);
            }

            base.OnHolding(e);
        }

        /// <summary>
        /// called when ManipulationStarting in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationStarting(e);
            }

            base.OnManipulationStarting(e);
        }
        /// <summary>
        /// called when Manipulation Started from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationStarted(e);
            }

            base.OnManipulationStarted(e);
        }

        /// <summary>
        /// called when manipulation InertiaStarting in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationInertiaStarting(e);
            }

            base.OnManipulationInertiaStarting(e);
        }

        /// <summary>
        /// Called when manipulation completed in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationCompleted(e);
            }

            base.OnManipulationCompleted(e);
        }

        /// <summary>
        /// Called when Manipulation delta changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnManipulationDelta(Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnManipulationDelta(e);
            }

            base.OnManipulationDelta(e);
        }

        /// <summary>
        /// Called when pointer pressed in sfchart
        /// </summary>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (Annotations.FirstOrDefault(item => item is ShapeAnnotation && (item as ShapeAnnotation).CanDrag) != null
                || Behaviors.FirstOrDefault(item => item is ChartZoomPanBehavior) != null)
                ManipulationMode = ManipulationModes.Scale
                                   | ManipulationModes.TranslateRailsX
                                   | ManipulationModes.TranslateRailsY
                                   | ManipulationModes.TranslateX
                                   | ManipulationModes.TranslateY
                                   | ManipulationModes.TranslateInertia
                                   | ManipulationModes.Rotate;
            else if (ManipulationMode != ManipulationModes.System)
                ManipulationMode = ManipulationModes.System;

            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerPressed(e);
            }

            foreach (ChartAxisBase2D axis in Axes)
            {
                var pointerPoint = e.GetCurrentPoint(chartAxisPanel);

                if (this.AreaType == ChartAreaType.PolarAxes)
                {
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        if (!(e.OriginalSource is ChartCartesianAxisPanel))
                            axis.SetLabelDownArguments(e.OriginalSource);
                    }
                }
                else
                {
                    if (!(e.OriginalSource is ChartCartesianAxisPanel))
                    {
                        pointerPoint = e.GetCurrentPoint(chartAxisPanel);
                        var labelsPanel = (axis.axisLabelsPanel as ChartCartesianAxisLabelsPanel);
                        if (labelsPanel != null)
                        {
                            var rect = labelsPanel.Bounds;
                            if (rect.Contains(pointerPoint.Position))
                                axis.SetLabelDownArguments(e.OriginalSource);
                        }
                    }
                }
            }
            base.OnPointerPressed(e);
        }

        /// <summary>
        /// Called when pointer moved from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerMoved(e);
            }

            base.OnPointerMoved(e);
        }
        /// <summary>
        /// Called when pointer released from sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnPointerReleased(e);
            }

            base.OnPointerReleased(e);
        }
        /// <summary>
        /// Called when Double Tapped the Keys in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDoubleTapped(e);
            }

            base.OnDoubleTapped(e);
        }

#if CHECKLATER
        /// <summary>
        /// Called when drop the pointer in sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrop(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDrop(e);
            }

            base.OnDrop(e);
        }
        /// <summary>
        /// Called when Drag over from the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragOver(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDragOver(e);
            }

            base.OnDragOver(e);
        }
        /// <summary>
        /// Called when Drag leave from the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDragLeave(e);
            }

            base.OnDragLeave(e);
        }
        /// <summary>
        /// Called when drag enter into the sfchart
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            foreach (ChartBehavior behavior in Behaviors)
            {
                behavior.OnDragEnter(e);
            }

            base.OnDragEnter(e);
        }
#endif
#endregion

#region Private Static Methods

        private static void OnPrimaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;

            var oldAxis = e.OldValue as ChartAxis;

            SfChart chartArea = d as SfChart;

            if (chartAxis != null)
            {
                chartAxis.Area = chartArea;
                chartAxis.Orientation = Orientation.Horizontal;
                chartArea.InternalPrimaryAxis = (ChartAxis)e.NewValue;
                chartAxis.VisibleRangeChanged += chartAxis.OnVisibleRangeChanged;
            }

            if (oldAxis != null)
            {
                if (chartArea != null && chartArea.Axes != null && chartArea.Axes.Contains(oldAxis))
                {
                    chartArea.Axes.RemoveItem(oldAxis, chartArea.DependentSeriesAxes.Contains(oldAxis));
                    chartArea.DependentSeriesAxes.Remove(oldAxis);
                }

                oldAxis.VisibleRangeChanged -= oldAxis.OnVisibleRangeChanged;
                oldAxis.RegisteredSeries.Clear();
            }

            if (chartArea.Series != null && chartAxis != null)
                foreach (var series in chartArea.Series)
                {
                    var cartesianSeries = series as CartesianSeries;
                    if (cartesianSeries != null && cartesianSeries.XAxis == null)
                    {
                        CheckSeriesTransposition(series);
                        chartAxis.RegisteredSeries.Add((ISupportAxes)series);
                    }
                }

            if (chartArea.TechnicalIndicators != null && chartAxis != null)
                foreach (var series in chartArea.TechnicalIndicators)
                {
                    var financialTechnicalIndicator = series as FinancialTechnicalIndicator;
                    if (financialTechnicalIndicator != null && financialTechnicalIndicator.XAxis == null)
                    {
                        CheckSeriesTransposition(series);
                        chartAxis.RegisteredSeries.Add((ISupportAxes)series);
                    }
                }

            chartArea.OnAxisChanged(e);

        }

        private static void OnSecondaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;

            var oldAxis = e.OldValue as ChartAxis;

            SfChart chartArea = d as SfChart;

            if (chartAxis != null)
            {
                chartAxis.Area = chartArea;
                chartAxis.Orientation = Orientation.Vertical;
                chartArea.InternalSecondaryAxis = (ChartAxis)e.NewValue;
                chartAxis.IsSecondaryAxis = true;
            }

            if (oldAxis != null)
            {
                var axis = oldAxis as NumericalAxis;
                if (axis != null && axis.AxisScaleBreaks != null && axis.AxisScaleBreaks.Count >= 0)
                    axis.ClearBreakElements();
                if (chartArea != null && chartArea.Axes != null && chartArea.Axes.Contains(oldAxis))
                {
                    chartArea.Axes.RemoveItem(oldAxis, chartArea.DependentSeriesAxes.Contains(oldAxis));
                    chartArea.DependentSeriesAxes.Remove(oldAxis);
                }
                    
                oldAxis.RegisteredSeries.Clear();
            }

            if (chartArea.Series != null && chartAxis != null)
                foreach (var series in chartArea.Series)
                {
                    var cartesianSeries = series as CartesianSeries;
                    if (cartesianSeries != null && cartesianSeries.YAxis == null)
                    {
                        CheckSeriesTransposition(series);
                        chartAxis.RegisteredSeries.Add((ISupportAxes)series);
                    }
                }

            if (chartArea.TechnicalIndicators != null && chartAxis != null)
                foreach (var series in chartArea.TechnicalIndicators)
                {
                    var financialTechnicalIndicator = series as FinancialTechnicalIndicator;
                    if (financialTechnicalIndicator != null && financialTechnicalIndicator.YAxis == null)
                    {
                        CheckSeriesTransposition(series);
                        chartAxis.RegisteredSeries.Add((ISupportAxes)series);
                    }
                }

            chartArea.OnAxisChanged(e);

        }

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChart).OnWaterMarkChanged(e);
        }

        private static void OnSeriesPropertyCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChart).OnSeriesPropertyCollectionChanged(e);
        }

        private static void OnTechnicalIndicatorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SfChart).OnTechnicalIndicatorsPropertyChanged(e);
        }

        private static void OnAnnotationsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfChart).AnnotationsChanged(args);
        }

        private static void CheckSeriesTransposition(ChartSeries series)
        {
            if (series.ActualXAxis == null || series.ActualYAxis == null) return;
            series.ActualXAxis.Orientation = series.IsActualTransposed ? Orientation.Vertical : Orientation.Horizontal;
            series.ActualYAxis.Orientation = series.IsActualTransposed ? Orientation.Horizontal : Orientation.Vertical;
        }

#endregion

#region Private Methods

        void FastRenderDevicePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint mousePoint = e.GetCurrentPoint(fastRenderDevice);

            currentBitmapPixel = (fastRenderSurface.PixelWidth *
                (int)mousePoint.Position.Y + (int)mousePoint.Position.X) * 4;

            adorningCanvasPoint = e.GetCurrentPoint(GetAdorningCanvas()).Position;
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
                UpdateBitmapToolTip();
        }

        private void OnWaterMarkChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ChartDockPanel != null)
            {
                AddOrRemoveWatermark(e.NewValue as Watermark, e.OldValue as Watermark);
            }
        }

        private void OnSeriesPropertyCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                RemoveVisualChild();
                AddOrRemoveBitmap();
                ChartSeriesCollection seriesCollection = (e.OldValue as ChartSeriesCollection);
                seriesCollection.CollectionChanged -= OnSeriesCollectionChanged;
                foreach (ChartSeries series in seriesCollection)
                {
                    var cartesianSeries = series as CartesianSeries;
                    if (cartesianSeries != null)
                    {
                        cartesianSeries.XAxis = null;
                        cartesianSeries.YAxis = null;
                    }
                }
            }
            if (Series != null)
            {
                Series.CollectionChanged += OnSeriesCollectionChanged;
                if (Series.Count > 0)
                {
                    if (Series[0] is PolarSeries || Series[0] is RadarSeries)
                        AreaType = ChartAreaType.PolarAxes;
                    else if (Series[0] is AccumulationSeriesBase)
                        AreaType = ChartAreaType.None;
                    else
                        AreaType = ChartAreaType.CartesianAxes;

                    foreach (ChartSeries series in Series)
                    {
                        series.UpdateLegendIconTemplate(false);
                        series.Area = this;

                        var supportAxisSeries = series as ISupportAxes;
                        if (series.ActualXAxis != null)
                        {
                            series.ActualXAxis.Area = this;
                            if (supportAxisSeries != null && !series.ActualXAxis.RegisteredSeries.Contains(supportAxisSeries))
                                series.ActualXAxis.RegisteredSeries.Add(supportAxisSeries);
                            if (!this.Axes.Contains(series.ActualXAxis))
                            {
                                this.Axes.Add(series.ActualXAxis);
                                this.DependentSeriesAxes.Add(series.ActualXAxis);
                            }
                        }
                        if (series.ActualYAxis != null)
                        {
                            series.ActualYAxis.Area = this;
                            if (supportAxisSeries != null && !series.ActualYAxis.RegisteredSeries.Contains(supportAxisSeries))
                                series.ActualYAxis.RegisteredSeries.Add(supportAxisSeries);
                            if (!this.Axes.Contains(series.ActualYAxis))
                            {
                                this.Axes.Add(series.ActualYAxis);
                                this.DependentSeriesAxes.Add(series.ActualYAxis);
                            }
                        }
                        CheckSeriesTransposition(series);
                        if (this.seriesPresenter != null && !this.seriesPresenter.Children.Contains(series))
                        {
                            this.seriesPresenter.Children.Add(series);
                        }
                        if (series.IsSeriesVisible)
                        {
                            if (AreaType == ChartAreaType.PolarAxes && (series is PolarSeries || series is RadarSeries))
                            {
                                VisibleSeries.Add(series);
                            }
                            else if (AreaType == ChartAreaType.None && (series is AccumulationSeriesBase))
                            {
                                VisibleSeries.Add(series);
                            }
                            else if (AreaType == ChartAreaType.CartesianAxes && (series is CartesianSeries || series is HistogramSeries))
                            {
                                VisibleSeries.Add(series);
                            }
                        }
                        base.ActualSeries.Add(series);
                    }
                }
                UpdateLegend(Legend, false);
                AddOrRemoveBitmap();
            }
            else
                UpdateLegend(Legend, true);

            ScheduleUpdate();
        }

        private void RemoveVisualChild()
        {
            if (seriesPresenter != null)
            {
                for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                {
                    if (seriesPresenter.Children[i] is AdornmentSeries ||
                        seriesPresenter.Children[i] is HistogramSeries)
                    {
                        var series = seriesPresenter.Children[i] as ISupportAxes;
                        if (series != null)
                        {
                            var cartesianSeries = series as CartesianSeries;
                            if (cartesianSeries != null)
                            {
                                if (cartesianSeries.ActualXAxis != null)
                                    cartesianSeries.ActualXAxis.RegisteredSeries.Clear();

                                if (cartesianSeries.ActualYAxis != null)
                                    cartesianSeries.ActualYAxis.RegisteredSeries.Clear();

                                if (InternalPrimaryAxis == cartesianSeries.XAxis)
                                    InternalPrimaryAxis = null;
                                if (InternalSecondaryAxis == cartesianSeries.YAxis)
                                    InternalSecondaryAxis = null;
                            }
                        }
                        seriesPresenter.Children.RemoveAt(i);
                    }
                }
            }
            VisibleSeries.Clear();
            ActualSeries.Clear();
        }

        private void OnTechnicalIndicatorsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                if (seriesPresenter != null)
                {
                    AddOrRemoveBitmap();
                    for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                    {
                        if (seriesPresenter.Children[i] is FinancialTechnicalIndicator)
                        {
                            var series = seriesPresenter.Children[i] as ISupportAxes2D;
                            series.XAxis = null;
                            series.YAxis = null;
                            seriesPresenter.Children.RemoveAt(i);
                        }
                    }
                }

                (e.OldValue as ObservableCollection<ChartSeries>).CollectionChanged -= OnTechnicalIndicatorsCollectionChanged;
            }

            if (TechnicalIndicators != null)
            {
                TechnicalIndicators.CollectionChanged += OnTechnicalIndicatorsCollectionChanged;
                if (TechnicalIndicators.Count > 0)
                {
                    FinancialTechnicalIndicator financialIndicator;
                    foreach (var indicator in TechnicalIndicators)
                    {
                        indicator.Area = this;
                        financialIndicator = indicator as FinancialTechnicalIndicator;
                        if (financialIndicator.XAxis != null && !Axes.Contains(financialIndicator.XAxis))
                        {
                            financialIndicator.XAxis.Area = this;
                            Axes.Add(financialIndicator.XAxis);
                            if (!financialIndicator.XAxis.RegisteredSeries.Contains(financialIndicator))
                                financialIndicator.XAxis.RegisteredSeries.Add(financialIndicator);
                        }
                        if (financialIndicator.YAxis != null && !Axes.Contains(financialIndicator.YAxis))
                        {
                            financialIndicator.YAxis.Area = this;
                            Axes.Add(financialIndicator.YAxis);
                            if (!financialIndicator.YAxis.RegisteredSeries.Contains(financialIndicator))
                                financialIndicator.YAxis.RegisteredSeries.Add(financialIndicator);
                        }

                        if (seriesPresenter != null && !seriesPresenter.Children.Contains(indicator))
                        {
                            seriesPresenter.Children.Add(indicator);
                        }
                    }
                }
                AddOrRemoveBitmap();
            }
            ScheduleUpdate();
        }

        private void UpdateAnnotationClips()
        {
            //Updating the clips for the annotations.
            foreach (var annotation in Annotations)
            {
                annotation.SetAxisFromName();
            }

            this.ChartAnnotationCanvas.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0, 0), AvailableSize) };
            this.SeriesAnnotationCanvas.Clip = new RectangleGeometry() { Rect = this.SeriesClipRect };
        }

        private void OnAxisChanged(DependencyPropertyChangedEventArgs e)
        {
            var chartAxis = e.NewValue as ChartAxis;

            if (Axes != null && chartAxis != null && !Axes.Contains(chartAxis))
            {
                chartAxis.Area = this;
                Axes.Insert(0, chartAxis);
                DependentSeriesAxes.Add(chartAxis);
            }

            if (AnnotationManager != null)
                AnnotationManager.Annotations = Annotations;
            ScheduleUpdate();
        }

#if NETFX_CORE

        private void FastRenderDevice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                UpdateBitmapToolTip();
                isTap = true;
            }
        }
        void FastRenderDevicePointerExited(object sender, PointerRoutedEventArgs e)
        {
            for (int i = VisibleSeries.Count - 1; i >= 0; i--)
            {
                ChartSeries series = VisibleSeries[i] as ChartSeries;

                if (!isTap)
                {
                    if (series.ShowTooltip)
                    {
                        series.RemoveTooltip();
                    }
                }
            }
            isTap = false;
        }
#else
        void fastRenderDevice_MouseLeave(object sender, MouseEventArgs e)

        {
            for (int i = VisibleSeries.Count - 1; i >= 0; i--)
            {
                ChartSeries series = VisibleSeries[i] as ChartSeries;

                if (series.ShowTooltip)
                {
                    series.RemoveTooltip();
                }
            }
        }

#endif
        private void OnTechnicalIndicatorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (seriesPresenter != null)
                {
                    for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                    {
                        if (seriesPresenter.Children[i] is FinancialTechnicalIndicator)
                        {
                            var series = seriesPresenter.Children[i] as ISupportAxes;
                            UnRegisterSeries(series);
                            seriesPresenter.Children.RemoveAt(i);
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                FinancialTechnicalIndicator indicator = e.OldItems[0] as FinancialTechnicalIndicator;
                if (indicator == null) return;
                if (indicator.ActualYAxis.RegisteredSeries != null &&
                    indicator.ActualYAxis.RegisteredSeries.Contains(indicator))
                {
                    indicator.YAxis = null;
                    indicator.XAxis = null;
                }
                if (seriesPresenter.Children.Contains(indicator))
                    seriesPresenter.Children.Remove(indicator);
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (FinancialTechnicalIndicator indicator in e.NewItems)
                {
                    indicator.UpdateLegendIconTemplate(false);
                    indicator.Area = this;
                    if (indicator.XAxis != null && !this.Axes.Contains(indicator.XAxis))
                    {
                        indicator.XAxis.Area = this;
                        this.Axes.Add(indicator.XAxis);
                        if (!indicator.XAxis.RegisteredSeries.Contains(indicator))
                            indicator.XAxis.RegisteredSeries.Add(indicator);
                    }
                    if (indicator.YAxis != null && !this.Axes.Contains(indicator.YAxis))
                    {
                        indicator.YAxis.Area = this;
                        this.Axes.Add(indicator.YAxis);
                        if (!indicator.YAxis.RegisteredSeries.Contains(indicator))
                            indicator.YAxis.RegisteredSeries.Add(indicator);
                    }
                    if (this.seriesPresenter != null && !this.seriesPresenter.Children.Contains(indicator))
                    {
                        this.seriesPresenter.Children.Add(indicator);
                    }
                }
            }
            AddOrRemoveBitmap();
            ScheduleUpdate();
        }

        void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (seriesPresenter != null)
                {
                    for (int i = seriesPresenter.Children.Count - 1; i >= 0; i--)
                    {
                        if (seriesPresenter.Children[i] is AdornmentSeries ||
                            seriesPresenter.Children[i] is HistogramSeries)
                        {
                            if (seriesPresenter.Children[i] is ISupportAxes)
                            {
                                var series = seriesPresenter.Children[i] as ISupportAxes;
                                UnRegisterSeries(series);
                            }

                            var currentSeries = seriesPresenter.Children[i];
                            var doughnutSeries = currentSeries as DoughnutSeries;
                            if (doughnutSeries != null)
                            {
                                DoughnutSeries.RemoveCenterView(doughnutSeries.CenterView);
                            }
                            
                            seriesPresenter.Children.Remove(currentSeries);
                        }
                    }
                }
                else
                {
                    foreach (var axis in Axes)
                    {
                        axis.RegisteredSeries.Clear();
                    }
                }

                if (LegendItems != null)
                {
                    if (Legend is ChartLegendCollection)
                        foreach (var item in LegendItems)
                            item.Clear();
                    else if (LegendItems.Count > 0)
                        LegendItems[0].Clear();
                }
                ActualSeries.Clear();
                VisibleSeries.Clear();
                SelectedSeriesCollection.Clear();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                ChartSeriesBase series = e.OldItems[0] as ChartSeriesBase;

                if (VisibleSeries.Contains(series))
                    VisibleSeries.Remove(series);
                if (ActualSeries.Contains(series))
                    ActualSeries.Remove(series);
                if (SelectedSeriesCollection.Contains(series))
                    SelectedSeriesCollection.Remove(series);

                UnRegisterSeries(series as ISupportAxes);
                if (series is AccumulationSeriesBase || series is CircularSeriesBase)
                {
                    if (Legend != null && LegendItems != null)
                    {
                        var filterSeriesLegend = LegendItems.Where(item => item.Where(it => it.Series == series).Count() > 0).ToList();
                        if (filterSeriesLegend.Count > 0)
                        {
                            filterSeriesLegend[0].Clear();
                        }
                    }
                }
                else
                {
                    if (Legend != null && LegendItems != null)
                    {
                        var filterSeriesLegend = LegendItems.Where(item => item.Where(it => it.Series == series).Count() > 0).ToList();

                        if (filterSeriesLegend.Count > 0)
                        {
                            var itemIndex = filterSeriesLegend[0].IndexOf(filterSeriesLegend[0].Where(item => item.Series == series).FirstOrDefault());
                            var index = LegendItems.IndexOf(filterSeriesLegend[0]);
                            if (filterSeriesLegend[0].Count() > 0 && LegendItems[index].Contains(filterSeriesLegend[0][itemIndex]))
                            {
                                LegendItems[index].Remove(filterSeriesLegend[0][itemIndex]);
                                if (series is CartesianSeries)
                                    foreach (var item in (series as CartesianSeries).Trendlines)
                                    {
                                        var containlegendtrenditem = LegendItems[index].Where(it => it.Trendline == item).ToList();
                                        if (containlegendtrenditem.Count() > 0 && LegendItems[index].Contains(containlegendtrenditem[0]))
                                        {
                                            LegendItems[index].Remove(containlegendtrenditem[0]);
                                        }
                                    }
                            }
                        }
                    }
                }
                if (this.seriesPresenter != null)
                    this.seriesPresenter.Children.Remove(series);
                series.RemoveTooltip();

                var doughnutSeries = series as DoughnutSeries;
                if(doughnutSeries != null)
                {
                    DoughnutSeries.RemoveCenterView(doughnutSeries.CenterView);
                }

                if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    if (Series.Count > 0)
                    {
                        if (Series[0] is PolarRadarSeriesBase)
                            AreaType = ChartAreaType.PolarAxes;
                        else if (Series[0] is AccumulationSeriesBase)
                            AreaType = ChartAreaType.None;
                        else
                            AreaType = ChartAreaType.CartesianAxes;
                        UpdateVisibleSeries(Series, e.NewStartingIndex);
                    }
                }
                else if (VisibleSeries.Count == 0 && Series.Count > 0)
                {
                    if (Series[0] is PolarRadarSeriesBase)
                        AreaType = ChartAreaType.PolarAxes;
                    else if (Series[0] is AccumulationSeriesBase)
                        AreaType = ChartAreaType.None;
                    else
                        AreaType = ChartAreaType.CartesianAxes;
                }
                else if (VisibleSeries.Count == 0 && Series.Count == 0)
                {
                    AreaType = ChartAreaType.CartesianAxes;
                }

                //WP-795: update the remaining series Chart Palette while remove the series
                if (Palette != ChartColorPalette.None)
                {
                    foreach (var segment in VisibleSeries.SelectMany(ser => ser.Segments))
                    {
                        segment.BindProperties();
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewStartingIndex == 0)
                {
                    if (Series[0] is PolarRadarSeriesBase)
                        AreaType = ChartAreaType.PolarAxes;
                    else if (Series[0] is AccumulationSeriesBase)
                        AreaType = ChartAreaType.None;
                    else
                        AreaType = ChartAreaType.CartesianAxes;
                }
                if (e.OldItems == null && GetEnableSeriesSelection()
                    && SeriesSelectedIndex < Series.Count && SeriesSelectedIndex != -1)
                {
                    SelectedSeriesCollection.Add(Series[SeriesSelectedIndex]);
                }

                UpdateVisibleSeries(e.NewItems, e.NewStartingIndex);
            }
            var canvas = this.GetAdorningCanvas();
            if (canvas != null)
            {
                if (canvas.Children.Contains((this.Tooltip as ChartTooltip)))
                    canvas.Children.Remove(this.Tooltip as ChartTooltip);
            }
            IsUpdateLegend = true;
            AddOrRemoveBitmap();
            this.ScheduleUpdate();
            SBSInfoCalculated = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void UnRegisterSeries(ISupportAxes series)
        {
            if (series != null)
            {
                if (series is CartesianSeries)
                {
                    CartesianSeries cartesianSeries = series as CartesianSeries;
                    if (cartesianSeries.YAxis == null && InternalSecondaryAxis != null)
                        InternalSecondaryAxis.RegisteredSeries.Remove(series);
                    else
                    {
                        if (InternalSecondaryAxis == cartesianSeries.YAxis)
                            InternalSecondaryAxis = null;
                        cartesianSeries.ClearValue(CartesianSeries.YAxisProperty);
                    }

                    if (cartesianSeries.XAxis == null)
                        InternalPrimaryAxis.RegisteredSeries.Remove(series);
                    else
                    {
                        if (InternalPrimaryAxis == cartesianSeries.XAxis)
                            InternalPrimaryAxis = null;
                        cartesianSeries.ClearValue(CartesianSeries.XAxisProperty);
                    }
                }
                else if (series is FinancialTechnicalIndicator)
                {
                    FinancialTechnicalIndicator financialIndicator = series as FinancialTechnicalIndicator;
                    if (financialIndicator.YAxis == null)
                        InternalSecondaryAxis.RegisteredSeries.Remove(series);
                    else
                    {
                        if (financialIndicator.YAxis.Equals(SecondaryAxis))
                            this.ClearValue(SecondaryAxisProperty);
                        financialIndicator.ClearValue(FinancialTechnicalIndicator.YAxisProperty);
                    }

                    if (financialIndicator.XAxis == null)
                        InternalPrimaryAxis.RegisteredSeries.Remove(series);
                    else
                    {
                        if (financialIndicator.XAxis == PrimaryAxis)
                            this.ClearValue(PrimaryAxisProperty);
                        financialIndicator.ClearValue(FinancialTechnicalIndicator.XAxisProperty);
                    }
                }
            }
            if (InternalPrimaryAxis != null && InternalSecondaryAxis != null
                && InternalPrimaryAxis.RegisteredSeries.Count == 0 && InternalSecondaryAxis.RegisteredSeries.Count == 0
                && (series != null && (series as ChartSeries).IsActualTransposed))
            {
                InternalPrimaryAxis.Orientation = Orientation.Horizontal;
                InternalSecondaryAxis.Orientation = Orientation.Vertical;
            }
        }

        private void UpdateVisibleSeries(IList seriesColl, int seriesIndex)
        {
            foreach (ChartSeries series in seriesColl)
            {
                if (series == null) continue;
                series.UpdateLegendIconTemplate(false);
                series.Area = this;

                CheckSeriesTransposition(series);

                var supportAxisSeries = series as ISupportAxes;
                if (series.ActualXAxis != null)
                {
                    series.ActualXAxis.Area = this;

                    if (supportAxisSeries != null && !series.ActualXAxis.RegisteredSeries.Contains(supportAxisSeries))
                        series.ActualXAxis.RegisteredSeries.Add(supportAxisSeries);
                    if (!this.Axes.Contains(series.ActualXAxis))
                    {
                        Axes.Add(series.ActualXAxis);
                        DependentSeriesAxes.Add(series.ActualXAxis);
                    }
                }
                if (series.ActualYAxis != null)
                {
                    series.ActualYAxis.Area = this;

                    if (supportAxisSeries != null && !series.ActualYAxis.RegisteredSeries.Contains(supportAxisSeries))
                        series.ActualYAxis.RegisteredSeries.Add(supportAxisSeries);
                    if(!this.Axes.Contains(series.ActualYAxis))
                    {
                        Axes.Add(series.ActualYAxis);
                        DependentSeriesAxes.Add(series.ActualYAxis);
                    }
                }
                if (seriesPresenter != null && !this.seriesPresenter.Children.Contains(series))
                {
                    seriesPresenter.Children.Insert(seriesIndex, series);
                    var doughnutSeries = series as DoughnutSeries;

                    if (doughnutSeries != null && doughnutSeries.CenterView != null)
                    {
                        if (!seriesPresenter.Children.Contains(doughnutSeries.CenterView))
                            seriesPresenter.Children.Add(doughnutSeries.CenterView);
                    }
                }
                if (series.IsSeriesVisible)
                {
                    int count = VisibleSeries.Count;
                    int visibleSeriesIndex = seriesIndex > count ? count : seriesIndex;

                    if (AreaType == ChartAreaType.PolarAxes && series is PolarRadarSeriesBase && !VisibleSeries.Contains(series))
                    {
                        VisibleSeries.Insert(visibleSeriesIndex, series);
                    }
                    else if (AreaType == ChartAreaType.None && series is AccumulationSeriesBase && !VisibleSeries.Contains(series))
                    {
                        VisibleSeries.Insert(visibleSeriesIndex, series);
                    }
                    else if (AreaType == ChartAreaType.CartesianAxes && (series is CartesianSeries || series is HistogramSeries) && !VisibleSeries.Contains(series))
                    {
                        VisibleSeries.Insert(visibleSeriesIndex, series);
                    }
                }
                if (!base.ActualSeries.Contains(series))
                    base.ActualSeries.Insert(seriesIndex, series);
            }
        }

        private void AddOrRemoveWatermark(Watermark newWatermark, Watermark oldWatermark)
        {
            if (this.ChartDockPanel.Children.Contains(oldWatermark))
                this.ChartDockPanel.Children.Remove(oldWatermark);
            if (newWatermark != null && !this.rootPanel.Children.Contains(newWatermark))
            {
                this.Watermark.SetValue(ChartDockPanel.DockProperty, ChartDock.Floating);
                this.ChartDockPanel.Children.Insert(0, newWatermark);//WRT-2656-Need to Change the Default ZIndex of Watermark.
            }
        }

        /// <summary>
        /// This method is to update bitmap series tooltip 
        /// </summary>
        private void UpdateBitmapToolTip()
        {
            for (int i = VisibleSeries.Count - 1; i >= 0; i--)
            {
                ChartSeries series = VisibleSeries[i] as ChartSeries;

                if (series.ShowTooltip && series.IsHitTestSeries())
                {
                    //Gets the current mouse position chart data point
                    ChartDataPointInfo datapoint = series.GetDataPoint(adorningCanvasPoint);

                    if (datapoint != null)
                    {
                        series.mousePos = adorningCanvasPoint;
                        series.UpdateSeriesTooltip(datapoint);
                        previousSeries = series;
                    }
                    break;
                }
                if (previousSeries != null)
                {
                    previousSeries.RemoveTooltip();
                }
            }
            currentBitmapPixel = -1;
        }

        void OnSfChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var behavior in Behaviors)
            {
                behavior.OnSizeChanged(e);
            }
        }

        private double GetOffetValue(ChartAxis axis)
        {
            if (Legend != null && Legend is ChartLegend)
            {
                if ((Legend as ChartLegend).DockPosition == ChartDock.Left || (Legend as ChartLegend).DockPosition == ChartDock.Top)
                {
                    if (axis.Orientation == Orientation.Horizontal)
                        return (ChartDockPanel.DesiredSize.Width - rootPanel.DesiredSize.Width);
                    return (ChartDockPanel.DesiredSize.Height - rootPanel.DesiredSize.Height);
                }
            }
            return 0d;
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != AvailableSize)
                InvalidateMeasure();
        }

        private void LayoutAxis(Size availableSize)
        {
            if (ChartAxisLayoutPanel != null)
            {
                ChartAxisLayoutPanel.UpdateElements();
                ChartAxisLayoutPanel.Measure(availableSize);
                ChartAxisLayoutPanel.Arrange(availableSize);
            }

            foreach (var item in ColumnDefinitions)
            {
                if (gridLinesPanel != null && item != null && item.BorderLine != null && !gridLinesPanel.Children.Contains(item.BorderLine))
                    gridLinesPanel.Children.Add(item.BorderLine);
            }
            foreach (var item in RowDefinitions)
            {
                if (gridLinesPanel != null && item != null && item.BorderLine != null && !gridLinesPanel.Children.Contains(item.BorderLine))
                    gridLinesPanel.Children.Add(item.BorderLine);
            }

            if (GridLinesLayout != null)
            {
                GridLinesLayout.UpdateElements();
                GridLinesLayout.Measure(availableSize);
                GridLinesLayout.Arrange(availableSize);
            }
            if (AdorningCanvas != null)
            {
                foreach (var axis in Axes)
                {
                    var linearAxis = axis as NumericalAxis;
                    if (linearAxis != null && linearAxis.AxisScaleBreaks.Count > 0 && linearAxis.IsSecondaryAxis)
                    {
                        linearAxis.DrawScaleBreakLines(axis);
                    }
                }
            }
        }

#endregion

#endregion
    }

    /// <summary>
    /// Represents chart series bounds changed event arguments.
    /// </summary>
    ///<remarks>
    /// It contains information like old bounds and new bounds.
    /// </remarks>
    public partial class ChartSeriesBoundsEventArgs : EventArgs
    {
#region Constructor

        public ChartSeriesBoundsEventArgs()
        {

        }

#endregion

#region Properties

#region Public Properties

        public Rect NewBounds { get; set; }
        public Rect OldBounds { get; set; }
#endregion

#endregion
    }

    static class NamespaceDoc
    {

    }
}
